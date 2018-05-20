using System.ComponentModel;

namespace DynamicGeometry.PropertyGrid
{
    public interface INotifyPropertyChanging
    {
        event PropertyChangedEventHandler PropertyChanging;
    }
}