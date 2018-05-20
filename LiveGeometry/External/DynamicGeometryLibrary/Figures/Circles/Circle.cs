using System.Windows;
using DynamicGeometry.Figures.Shapes;

namespace DynamicGeometry.Figures.Circles
{
    public class Circle : CircleBase, IShapeWithInterior
    {
        public override Point Center => Point(0);

        public override double Radius => Center.Distance(Point(1));

        public override double Inclination => Math.GetAngle(Center, Point(1));
    }
}
