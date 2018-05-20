using GuiLabs.Undo;

namespace DynamicGeometry.PropertyGrid.Editors
{
    public interface IValueEditor
    {
        IValueProvider Value { get; set; }
        ActionManager ActionManager { get; set; }
    }
}
