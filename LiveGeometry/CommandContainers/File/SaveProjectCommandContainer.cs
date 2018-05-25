using Catel.MVVM;
using Catel.Services;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.File
{
    internal class SaveProjectCommandContainer : CommandContainerBase
    {
        private readonly ISaveFileService _saveFileService;
        private readonly IDrawingHostServices _drawingHostServices;

        public SaveProjectCommandContainer(ICommandManager commandManager, ISaveFileService saveFileService, IDrawingHostServices drawingHostServices) 
            : base(Commands.SaveProject, commandManager)
        {
            _saveFileService = saveFileService;
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _saveFileService.Filter = Commands.FileFilter;
            _saveFileService.AddExtension = true;
            if (!_saveFileService.DetermineFile())
                return;

            _drawingHostServices.SaveToBitmap(_saveFileService.FileName);
        }
    }
}
