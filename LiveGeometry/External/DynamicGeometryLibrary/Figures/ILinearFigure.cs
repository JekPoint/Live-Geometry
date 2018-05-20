using System.Windows;

namespace DynamicGeometry.Figures
{
    public interface ILinearFigure : IFigure
    {
        double GetNearestParameterFromPoint(Point point);
        Point GetPointFromParameter(double parameter);
        Tuple<double, double> GetParameterDomain();
    }
}