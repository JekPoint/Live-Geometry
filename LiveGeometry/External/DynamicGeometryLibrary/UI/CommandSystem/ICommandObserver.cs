using System.Windows;

namespace DynamicGeometry.UI.CommandSystem
{
    public interface ICommandObserver
    {
        void CommandRemoved();
        void EnabledChanged(bool newEnabledState);
        void IconChanged(FrameworkElement icon);
    }
}
