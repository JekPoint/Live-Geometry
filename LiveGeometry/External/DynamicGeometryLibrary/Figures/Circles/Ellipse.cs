using System.Windows;
using DynamicGeometry.Figures.Shapes;

namespace DynamicGeometry.Figures.Circles
{
    public class Ellipse : EllipseBase, IShapeWithInterior
    {
        public override Point Center => Point(0);

        public override double SemiMajor => Center.Distance(Point(1));

        public override double SemiMinor => Center.Distance(Point(2));

        public override double Inclination => Math.GetAngle(Center, Point(1));
    }
}
