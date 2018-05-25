using Catel.MVVM;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.Editor
{
    internal class CutCommandContainer : CommandContainerBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public CutCommandContainer(ICommandManager commandManager, IDrawingHostServices drawingHostServices) 
            : base(Commands.Cut, commandManager)
        {
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            
        }
    }
}
