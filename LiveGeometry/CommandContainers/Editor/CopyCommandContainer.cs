using Catel.MVVM;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.Editor
{
    internal class CopyCommandContainer : CommandContainerBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public CopyCommandContainer(ICommandManager commandManager, IDrawingHostServices drawingHostServices) 
            : base(Commands.Copy, commandManager)
        {
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _drawingHostServices.Copy();
        }
    }
}
