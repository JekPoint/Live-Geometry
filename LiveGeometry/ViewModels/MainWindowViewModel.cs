using Catel.Data;
using DynamicGeometry.UI;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDrawingHostServices _drawingHostServices;

        public MainWindowViewModel(IDrawingHostServices drawingHostServices)
        {
            _drawingHostServices = drawingHostServices;
            DrawingHost = _drawingHostServices.MainDrawingHost;
        }

        public override string Title => "LiveGeometry";

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

        #region DrawingHost property

        /// <summary>
        /// Gets or sets the DrawingHost value.
        /// </summary>
        public DrawingHost DrawingHost
        {
            get => GetValue<DrawingHost>(DrawingHostProperty);
            set => SetValue(DrawingHostProperty, value);
        }

        /// <summary>
        /// DrawingHost property data.
        /// </summary>
        public static readonly PropertyData DrawingHostProperty = RegisterProperty("DrawingHost", typeof(DrawingHost));

        #endregion

        
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #region ExitApplication command

        private Command _exitApplicationCommand;

        /// <summary>
        /// Gets the ExitApplication command.
        /// </summary>
        public Command ExitApplicationCommand => _exitApplicationCommand ?? (_exitApplicationCommand = new Command(ExitApplication));

        /// <summary>
        /// Method to invoke when the ExitApplication command is executed.
        /// </summary>
        private void ExitApplication()
        {
#pragma warning disable 4014
            this.CancelAndCloseViewModelAsync();
#pragma warning restore 4014
        }

        #endregion
        
        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}
