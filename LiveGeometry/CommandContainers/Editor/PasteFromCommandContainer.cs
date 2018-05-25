using Catel.MVVM;
using Catel.Services;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.Editor
{
    internal class PasteFromCommandContainer : CommandContainerBase
    {
        private readonly IOpenFileService _openFileService;
        private readonly IDrawingHostServices _drawingHostServices;

        public PasteFromCommandContainer(ICommandManager commandManager, IOpenFileService openFileService, IDrawingHostServices drawingHostServices)
            : base(Commands.PasteFrom, commandManager)
        {
            _openFileService = openFileService;
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _openFileService.Filter = Commands.DrawingFileFilter;
            if (_openFileService.DetermineFile())
            {
                _drawingHostServices.PasteFrom(_openFileService.FileName);
            }
        }
    }
}
    