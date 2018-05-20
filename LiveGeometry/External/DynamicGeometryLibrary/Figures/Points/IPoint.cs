using System.Windows;

namespace DynamicGeometry.Figures.Points
{
    public partial interface IPoint : IFigure
    {
        Point Coordinates { get; }
    }
}