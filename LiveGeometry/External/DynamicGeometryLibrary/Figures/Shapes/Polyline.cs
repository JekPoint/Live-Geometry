using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using DynamicGeometry.Actions;
using DynamicGeometry.Figures.Controls;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Points;
using GuiLabs.Undo;

namespace DynamicGeometry.Figures.Shapes
{
    public class Polyline : ShapeBase<System.Windows.Shapes.Polyline>, ILinearFigure, ISupportRemoveDependency, IPolygonalChain, ILengthProvider
    {
        public Polyline()
        {
            this.mDependencies.CollectionChanged += mDependencies_CollectionChanged;
        }

        /// <summary>
        /// Just for caching purposes, to avoid array allocations on a hotpath
        /// </summary>
        protected IPoint[] vertices;
        protected Point[] vertexCoordinates;
        public Point[] VertexCoordinates => vertexCoordinates;

        public override Point Center => VertexCoordinates.Midpoint();

        public double Length => VertexCoordinates.Distance();

        protected override void OnDependenciesChanged()
        {
            UpdatePointCache();
        }

        private void UpdatePointCache()
        {
            vertices = Dependencies.Where(f => f is IPoint).Cast<IPoint>().ToArray();   // Tolerates non-IPoint dependencies.
            vertexCoordinates = new Point[vertices.Length];
            var cache = new System.Windows.Media.PointCollection();
            Shape.Points = cache;

            for (var i = 0; i < vertices.Length; i++)
            {
                cache.Add(new Point());
            }
        }

        void mDependencies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Drawing != null)
            {
                this.UpdateVisual();
            }
        }

        protected override int DefaultZOrder()
        {
            return (int)ZOrder.Figures;
        }

        protected override System.Windows.Shapes.Polyline CreateShape()
        {
            return Factory.CreatePolylineShape();
        }

        public override void UpdateVisual()
        {
            if (vertices == null)
            {
                UpdatePointCache();
            }

            var points = Shape.Points;
            var coordinateSystem = Drawing.CoordinateSystem;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertexCoordinates[i] = vertices[i].Coordinates;
                points[i] = coordinateSystem.ToPhysical(vertexCoordinates[i]);
            }
        }

        public override IFigure HitTest(System.Windows.Point point)
        {
            var epsilon = ToLogical(Shape.StrokeThickness / 2 + Math.CursorTolerance);
            if (Math.IsPointOnPolygonalChain(Dependencies.ToPoints(), point, epsilon, false))
            {
                return this;
            }
            return null;
        }

        public double GetNearestParameterFromPoint(Point point)
        {
            return Math.GetNearestParameterFromPointOnPolyline(Dependencies.ToPoints(), point);
        }

        public Point GetPointFromParameter(double parameter)
        {
            return Math.GetPointOnPolylineFromParameter(Dependencies.ToPoints(), parameter);
        }

        public virtual Tuple<double, double> GetParameterDomain()
        {
            return Tuple.Create(0.0, 1.0);
        }

        public bool CanRemoveDependency(IFigure dependency)
        {
            var count = Dependencies.Count(d => d == dependency);
            return Dependencies.Count > count + 1 && count > 0;
        }

        public IAction GetRemoveDependencyAction(IFigure dependency)
        {
            return new RemoveItemAction<IFigure>(this.Dependencies, dependency);
        }
    }
}
