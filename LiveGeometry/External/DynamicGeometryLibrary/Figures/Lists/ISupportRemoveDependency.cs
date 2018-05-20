using GuiLabs.Undo;

namespace DynamicGeometry.Figures.Lists
{
    public interface ISupportRemoveDependency
    {
        bool CanRemoveDependency(IFigure dependency);
        IAction GetRemoveDependencyAction(IFigure dependency);
    }
}
