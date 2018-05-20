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
    [Order(3)]
    public class EllipseCreator : FigureCreator
    {
        protected override IEnumerable<IFigure> CreateFigures()
        {
            var ellipse = Factory.CreateEllipse(Drawing, FoundDependencies);
            //ShapeStyle newStyle = new ShapeStyle();
            //newStyle.Color = Colors.Black;
            //Drawing.StyleManager.Add(newStyle);
            //ellipse.Style = newStyle;
            yield return ellipse;
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPointPoint;
        }

        public override string Name => "Ellipse";

        public override string HintText => "Click the ellipse center then a point to define the semi-major axis then a point to define the semi-minor axis.";

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Ellipse(0.5, 0.5, 0.5, 0.3)
                .Point(0.5, 0.5)
                .Point(1.0, 0.5)
                .Point(0.5, 0.2)
                .Canvas;
        }
    }
}