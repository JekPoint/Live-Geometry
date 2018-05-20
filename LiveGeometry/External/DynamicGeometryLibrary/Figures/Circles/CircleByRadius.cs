using System.Windows;
using DynamicGeometry.Figures.Shapes;

namespace DynamicGeometry.Figures.Circles
{
    public class CircleByRadius : CircleBase, IShapeWithInterior
    {
        public override Point Center => Point(2);

        public override double Radius => Point(0).Distance(Point(1));
    }
}
