using System.Windows;

namespace DynamicGeometry.Figures.Circles
{
    public abstract partial class CircleBase : EllipseBase, ICircle
    {

        public abstract double Radius
        {
            get;
        }

        public override double Inclination => 0;

        public override double SemiMajor => Radius;

        public override double SemiMinor => Radius;

        public override void UpdateVisual()
        {
            var center = ToPhysical(Center);
            var diameter = ToPhysical(Radius * 2) + shape.StrokeThickness;
            if (shape.Width != diameter)
            {
                shape.Width = diameter;
            }

            if (shape.Height != diameter)
            {
                shape.Height = diameter;
            }

            shape.CenterAt(center);
        }

        public override Point GetPointFromParameter(double parameter)
        {
            if (Settings.PointsOnEllipticalsUseAbsoluteAngle)
            {
                var center = Center;
                var radius = Radius;
                return new Point(
                    center.X + radius * System.Math.Cos(parameter),
                    center.Y + radius * System.Math.Sin(parameter));
            }
            else
            {
                return base.GetPointFromParameter(parameter);
            }
        }
    }
}
