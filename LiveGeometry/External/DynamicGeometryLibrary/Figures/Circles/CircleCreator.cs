using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.UI;

namespace DynamicGeometry.Figures.Circles
{
    [Category(BehaviorCategories.Circles)]
    [Order(1)]
    public class CircleCreator : FigureCreator
    {
        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateCircle(Drawing, FoundDependencies);
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        public override string Name => "Circle";

        public override string HintText => "Click the circle center and then click a point on a circle.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Circle(0.5, 0.5, 0.5)
                .Point(0.5, 0.5)
                .Point(0.85, 0.15)
                .Canvas;
        }
    }
}