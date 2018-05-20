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
    [Order(5)]
    public class PerpendicularLineCreator : FigureCreator
    {
        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreatePerpendicularLine(Drawing, FoundDependencies);
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.LinePoint;
        }

        public override string Name => "Perpendicular";

        public override string HintText => "Click a line and then click a point.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Line(0, 0, 1, 1)
                .Line(0.3, 1, 1, 0.3)
                .Point(0.35, 0.35)
                .Canvas;
        }
    }
}