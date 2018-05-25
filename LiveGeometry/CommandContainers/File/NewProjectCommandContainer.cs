using Catel.MVVM;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.CommandContainers.File
{
    internal class NewProjectCommandContainer : CommandContainerBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public NewProjectCommandContainer(ICommandManager commandManager, IDrawingHostServices drawingHostServices) 
            : base(Commands.NewProject, commandManager)
        {
            _drawingHostServices = drawingHostServices;
        }

        protected override void Execute(object parameter)
        {
            _drawingHostServices.NewDocument();
        }
    }
}
