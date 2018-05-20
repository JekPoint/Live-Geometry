namespace DynamicGeometry.Figures.Lines
{
    public class ParallelLine : LineTwoPoints
    {
        public override PointPair Coordinates
        {
            get
            {
                PointPair coordinates;
                var parentLine = Dependencies.Line(0);
                var point = Point(1);

                coordinates = new PointPair()
                {
                    P1 = point,
                    P2 = point.Plus(parentLine.P2.Minus(parentLine.P1))
                };
                return coordinates;
            }
        }
    }
}