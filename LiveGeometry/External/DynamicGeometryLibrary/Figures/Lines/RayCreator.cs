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
    [Order(2)]
    public class RayCreator : FigureCreator
    {
        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateRay(Drawing, FoundDependencies);
        }

        public override string Name => "Ray";

        public override string HintText => "Click (and release) twice to create a ray.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Line(0.25, 0.75, 1, 0)
                .Point(0.25, 0.75)
                .Point(0.75, 0.25)
                .Canvas;
        }
    }
}