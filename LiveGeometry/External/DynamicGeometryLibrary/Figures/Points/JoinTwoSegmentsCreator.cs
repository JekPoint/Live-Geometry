using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures.Lines;
using DynamicGeometry.Figures.Shapes;
using DynamicGeometry.UI;

namespace DynamicGeometry.Figures.Points
{
    [Category(BehaviorCategories.Lines)]
    [Order(11)]
    public class JoinTwoSegmentsCreator : Behavior
    {
        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            var coordinates = Coordinates(e);
            var underMouse = Drawing.Figures.HitTest<FreePoint>(coordinates);
            if (underMouse != null)
            {
                JoinSegments(underMouse);
                // for polyline
                JoinPolyLineSegments(underMouse);
                RemovePointFromPolygons(underMouse);
            }
        }

        void JoinSegments(FreePoint point)
        {
            var dependents = point.Dependents.OfType<Segment>().ToArray();
            if (dependents.Length != 2)
            {
                return;
            }
            var line1 = dependents[0];
            var line2 = dependents[1];

            var otherPoint1 = line1.Dependencies.Without(point).FirstOrDefault();
            var otherPoint2 = line2.Dependencies.Without(point).FirstOrDefault();
            if (otherPoint1 == null || otherPoint2 == null)
            {
                return;
            }

            var segment = Factory.CreateSegment(Drawing, new[] { otherPoint1, otherPoint2 });

            using (Drawing.ActionManager.CreateTransaction())
            {
                RemovePointFromPolygons(point);
                Actions.Actions.Remove(line2);
                Actions.Actions.ReplaceWithNew(line1, segment);
                Actions.Actions.Remove(point);
            }
        }

        void RemovePointFromPolygons(FreePoint point)
        {
            foreach (var polygon in point.Dependents.OfType<PolygonBase>().ToList())
            {
                if (polygon.Dependencies.Count > 3)
                {
                    RemovePointFromPolygon(point, polygon);
                }
            }
        }

        void RemovePointFromPolygon(FreePoint point, PolygonBase polygon)
        {
            Actions.Actions.RemoveDependency(polygon, point);
        }

        public override string Name => "Join segments";

        public override string HintText => "Click a point between two segments to join the other two points.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Line(0.25, 0.75, 0.4, 0.4)
                .Line(0.4, 0.4, 0.75, 0.25)
                .Point(0.25, 0.75)
                .Point(0.75, 0.25)
                .Canvas;
        }

        void JoinPolyLineSegments(FreePoint point)
        {
            var dependents = point.Dependents.OfType<Polyline>().ToArray();

            foreach (object obj in dependents)
            {
                if (obj is Polyline)
                {
                    var polyline = (Polyline)obj;

                    if (polyline.Dependencies.Count <= 3)
                    {
                        return;
                    }

                    var NewPolyLinePoints = new List<IFigure>();

                    // Eliminate deleted point
                    for (var i = 0; i < polyline.Dependencies.Count; i++)
                    {
                        var p1 = polyline.Dependencies[i] as IPoint;

                        if (p1.Coordinates.X != point.Coordinates.X
                            && p1.Coordinates.Y != point.Coordinates.Y)
                        {
                            NewPolyLinePoints.Add(p1);
                        }
                    }

                    // create new polyline
                    var newPolyLine = Factory.CreatePolyline(Drawing, NewPolyLinePoints);
                    using (Drawing.ActionManager.CreateTransaction())
                    {
                        Actions.Actions.Remove(point);
                        Actions.Actions.ReplaceWithNew(polyline, newPolyLine);
                    }
                }
            }
        }
    }
}