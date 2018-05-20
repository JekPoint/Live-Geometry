using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.UI;

namespace DynamicGeometry.Figures.Lines
{
    [Category(BehaviorCategories.Lines)]
    [Order(6)]
    public class SegmentBisectorCreator : FigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateSegmentBisector(Drawing, FoundDependencies);
        }

        public override void MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var underMouse = Drawing.Figures.HitTest<Segment>(Coordinates(e));
            if (underMouse != null
                && underMouse.Dependencies.Count() == 2
                && Drawing.Figures.HitTest<IPoint>(Coordinates(e)) == null)
            {
                FoundDependencies.AddRange(underMouse.Dependencies);
            }
            base.MouseDown(sender, e);
        }

        public override string Name => "Segment Bisector";

        public override string HintText => "Click two points or a segment to create a bisector line.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Line(0.25, 0.75, 0.75, 0.25)
                .Point(0.25, 0.75)
                .Point(0.75, 0.25)
                .Line(0, 0, 1, 1)
                .Canvas;
        }
    }
}