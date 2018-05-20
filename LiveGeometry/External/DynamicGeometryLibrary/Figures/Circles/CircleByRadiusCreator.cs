using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.UI;

namespace DynamicGeometry.Figures.Circles
{
    [Category(BehaviorCategories.Circles)]
    [Order(2)]
    public class CircleByRadiusCreator : FigureCreator
    {
        public CircleByRadiusCreator()
        {
            CanReuseDependency = true;
        }

        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateCircleByRadius(Drawing, FoundDependencies);
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPointPoint;
        }

        protected override IFigure CreateIntermediateFigure()
        {
            if (FoundDependencies.Count == 2
                && FoundDependencies[0] is IPoint
                && FoundDependencies[1] is IPoint)
            {
                return Factory.CreateSegment(Drawing, FoundDependencies);
            }
            return null;
        }

        public override string Name => "By Radius";

        public override string HintText => "Click (and release) two points (start and end of a radius) and then click the circle center.";

        public override FrameworkElement CreateIcon()
        {
            const double r = 0.4;
            return IconBuilder.BuildIcon()
                .Circle(r, r, r)
                .Line(0.5, 0.9, 0.5 + r, 0.9)
                .Point(r, r)
                .Point(0.5, 0.9)
                .Point(0.5 + r, 0.9)
                .Canvas;
        }
    }
}