namespace DynamicGeometry.Figures.Lines
{
    public interface ILine : IFigure, ILinearFigure
    {
        PointPair Coordinates { get; }
    }
}