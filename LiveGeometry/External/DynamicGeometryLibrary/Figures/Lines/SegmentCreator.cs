using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.UI;

namespace DynamicGeometry.Figures.Lines
{
    [Category(BehaviorCategories.Lines)]
    [Order(1)]
    public class SegmentCreator : FigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateSegment(Drawing, FoundDependencies);
        }

        public override string Name => "Segment";

        public override string HintText => "Click (and release) twice to connect two points with a segment.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Line(0.25, 0.75, 0.75, 0.25)
                .Point(0.25, 0.75)
                .Point(0.75, 0.25)
                .Canvas;
        }
    }
}