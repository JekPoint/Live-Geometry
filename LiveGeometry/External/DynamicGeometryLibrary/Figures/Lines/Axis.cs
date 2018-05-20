using System.Windows.Media;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Shapes;

namespace DynamicGeometry.Figures.Lines
{
    public class Axis : CompositeFigure
    {
        public LineTwoPoints Line { get; set; }
        public Arrow Arrow { get; set; }

        public Axis()
        {
            Line = new LineTwoPoints();
            Line.SetZIndex(ZOrder.Axes);

            //Arrow = new Arrow();
            //Arrow.Dependencies.Add(Line);
            //Children.Add(Line, Arrow);
            Children.Add(Line);
        }

        public override IFigure HitTest(System.Windows.Point point, System.Predicate<IFigure> filter)
        {
            return null;
        }

        public static Color Color => Color.FromArgb(255, 128, 128, 255);
    }
}
