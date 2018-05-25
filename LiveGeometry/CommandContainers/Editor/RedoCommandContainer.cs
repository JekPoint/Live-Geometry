using Catel.MVVM;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.Editor
{
    internal class RedoCommandContainer : CommandContainerBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public RedoCommandContainer(ICommandManager commandManager, IDrawingHostServices drawingHostServices) 
            : base(Commands.Redo, commandManager)
        {
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _drawingHostServices.MainDrawingHost.DrawingControl.Redo();
        }
    }
}
