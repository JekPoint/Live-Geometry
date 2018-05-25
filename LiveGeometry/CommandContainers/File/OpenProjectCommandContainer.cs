using Catel.MVVM;
using Catel.Services;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.File
{
    internal class OpenProjectCommandContainer : CommandContainerBase
    {
        private readonly IOpenFileService _openFileService;
        private readonly IDrawingHostServices _drawingHostServices;

        public OpenProjectCommandContainer(ICommandManager commandManager, IOpenFileService openFileService, IDrawingHostServices drawingHostServices) 
            : base(Commands.OpenProject, commandManager)
        {
            _openFileService = openFileService;
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _openFileService.Filter = Commands.DrawingFileFilter;
            if (_openFileService.DetermineFile())
            {
                _drawingHostServices.OpenFile(_openFileService.FileName);
            }
        }
    }
}
