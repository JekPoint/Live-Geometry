using System.Collections.Generic;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.Figures.Shapes
{
    public partial class Polygon : PolygonBase
    {
#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert to Polyline")]
        public void ConvertToPolyline()
        {
            var newPolyLinePoints = new List<IFigure>();
            var verticesToDelete = new List<IFigure>();

            using (Drawing.ActionManager.CreateTransaction())
            {
                foreach (var vertex in this.Dependencies)
                {
                    var vertexPoint = vertex as IPoint;
                    var newVertexPoint = Factory.CreateFreePoint(this.Drawing, vertexPoint.Coordinates);
                    Actions.Actions.Add(Drawing, newVertexPoint);
                    verticesToDelete.Add(vertexPoint);
                    newPolyLinePoints.Add(newVertexPoint);
                }

                // add last point
                newPolyLinePoints.Add(newPolyLinePoints[0]);

                var newPolyline = Factory.CreatePolyline(this.Drawing, newPolyLinePoints);
                Actions.Actions.Add(Drawing, newPolyline);

                // delete main shape
                Actions.Actions.Remove(this);

                foreach (var vertexToDelete in verticesToDelete)
                {
                    Actions.Actions.Remove(vertexToDelete);
                }
            }
        }

#endif
    }
}
