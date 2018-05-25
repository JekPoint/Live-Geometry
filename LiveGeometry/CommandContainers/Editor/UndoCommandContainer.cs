using Catel.MVVM;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.Editor
{
    internal class UndoCommandContainer : CommandContainerBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public UndoCommandContainer(ICommandManager commandManager, IDrawingHostServices drawingHostServices) 
            : base(Commands.Undo, commandManager)
        {
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
           _drawingHostServices.MainDrawingHost.DrawingControl.Undo();
        }
    }
}
    