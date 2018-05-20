using Catel.Data;
using Catel.Services;
using DynamicGeometry.UI;
using LiveGeometry.Services.Interfaces;

namespace LiveGeometry.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class MainWindowViewModel : ViewModelBase
    {
        private const string Extension = "lgf";
        private const string LgfFileFilter = "Live Geometry file (*." + Extension + ")|*." + Extension;
        private const string PngFileFilter = "PNG image (*.png)|*.png";
        private const string BmpFileFilter = "BMP image (*.bmp)|*.bmp";
        private const string DxfFileFilter = "DXF file (*.dxf)|*.dxf";
        private const string AllFileFilter = "All files (*.*)|*.*";
        private const string DrawingFileFilter = LgfFileFilter + "|" + DxfFileFilter + "|" + AllFileFilter;

        private const string FileFilter = LgfFileFilter
                                  + "|" + PngFileFilter
                                  + "|" + BmpFileFilter
                                  + "|" + AllFileFilter;

        private readonly IDrawingHostServices _drawingHostServices;
        private readonly ISaveFileService _saveFileService;
        private readonly IOpenFileService _openFileService;

        public MainWindowViewModel(IDrawingHostServices drawingHostServices, ISaveFileService saveFileService, IOpenFileService openFileService)
        {
            _drawingHostServices = drawingHostServices;
            _saveFileService = saveFileService;
            _openFileService = openFileService;
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

        #region Undo command

        private Command _undoCommand;

        /// <summary>
        /// Gets the Undo command.
        /// </summary>
        public Command UndoCommand => _undoCommand ?? (_undoCommand = new Command(Undo));

        /// <summary>
        /// Method to invoke when the Undo command is executed.
        /// </summary>
        private void Undo()
        {
            DrawingHost.DrawingControl.Undo();
        }

        #endregion

        #region Redo command

        private Command _redoCommand;

        /// <summary>
        /// Gets the Redo command.
        /// </summary>
        public Command RedoCommand => _redoCommand ?? (_redoCommand = new Command(Redo));

        /// <summary>
        /// Method to invoke when the Redo command is executed.
        /// </summary>
        private void Redo()
        {
            DrawingHost.DrawingControl.Redo();
        }

        #endregion

        #region SelectAll command

        private Command _selectAllCommand;

        /// <summary>
        /// Gets the SelectAll command.
        /// </summary>
        public Command SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new Command(SelectAll));

        /// <summary>
        /// Method to invoke when the SelectAll command is executed.
        /// </summary>
        private void SelectAll()
        {
            DrawingHost.CurrentDrawing.SelectAll();
        }

        #endregion

        #region CommandName command

        private Command _commandNameCommand;

        /// <summary>
        /// Gets the CommandName command.
        /// </summary>
        public Command CommandNameCommand => _commandNameCommand ?? (_commandNameCommand = new Command(CommandName));

        /// <summary>
        /// Method to invoke when the CommandName command is executed.
        /// </summary>
        private void CommandName()
        {
            // TODO: Handle command logic here
        }

        #endregion

        #region Restart command

        private Command _restartCommand;

        /// <summary>
        /// Gets the Restart command.
        /// </summary>
        public Command RestartCommand => _restartCommand ?? (_restartCommand = new Command(Restart));

        /// <summary>
        /// Method to invoke when the Restart command is executed.
        /// </summary>
        private void Restart()
        {
            DrawingHost.CurrentDrawing.Behavior.Restart();
        }

        #endregion

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

        #region SaveContent command

        private Command _saveContentCommand;

        /// <summary>
        /// Gets the SaveContent command.
        /// </summary>
        public Command SaveContentCommand => _saveContentCommand ?? (_saveContentCommand = new Command(SaveContent));

        /// <summary>
        /// Method to invoke when the SaveContent command is executed.
        /// </summary>
        private void SaveContent()
        {
            _saveFileService.Filter = FileFilter;
            _saveFileService.AddExtension = true;
            if (!_saveFileService.DetermineFile())
                return;

            _drawingHostServices.SaveToBitmap(_saveFileService.FileName);
        }

        #endregion

        #region Openfile command

        private Command _openfileCommand;

        /// <summary>
        /// Gets the Openfile command.
        /// </summary>
        public Command OpenfileCommand
        {
            get { return _openfileCommand ?? (_openfileCommand = new Command(Openfile)); }
        }

        /// <summary>
        /// Method to invoke when the Openfile command is executed.
        /// </summary>
        private void Openfile()
        {
            _openFileService.Filter = DrawingFileFilter;
            if (_openFileService.DetermineFile())
            {
                _drawingHostServices.OpenFile(_openFileService.FileName);
            }
        }

        #endregion

        #region NewDocument command

        private Command _newDocumentCommand;

        /// <summary>
        /// Gets the NewDocument command.
        /// </summary>
        public Command NewDocumentCommand
        {
            get { return _newDocumentCommand ?? (_newDocumentCommand = new Command(NewDocument)); }
        }

        /// <summary>
        /// Method to invoke when the NewDocument command is executed.
        /// </summary>
        private void NewDocument()
        {
            _drawingHostServices.NewDocument();
        }

        #endregion

        #region Copy command

        private Command _copyCommand;

        /// <summary>
        /// Gets the Copy command.
        /// </summary>
        public Command CopyCommand
        {
            get { return _copyCommand ?? (_copyCommand = new Command(Copy)); }
        }

        /// <summary>
        /// Method to invoke when the Copy command is executed.
        /// </summary>
        private void Copy()
        {
            _drawingHostServices.Copy();
        }

        #endregion

        #region Paste command

        private Command _pasteCommand;

        /// <summary>
        /// Gets the Paste command.
        /// </summary>
        public Command PasteCommand
        {
            get { return _pasteCommand ?? (_pasteCommand = new Command(Paste)); }
        }

        /// <summary>
        /// Method to invoke when the Paste command is executed.
        /// </summary>
        private void Paste()
        {
            _drawingHostServices.Paste();
        }

        #endregion

        #region PasteFrom command

        private Command _pasteFromCommand;

        /// <summary>
        /// Gets the PasteFrom command.
        /// </summary>
        public Command PasteFromCommand
        {
            get { return _pasteFromCommand ?? (_pasteFromCommand = new Command(PasteFrom)); }
        }

        /// <summary>
        /// Method to invoke when the PasteFrom command is executed.
        /// </summary>
        private void PasteFrom()
        {
            _openFileService.Filter = DrawingFileFilter;
            if (_openFileService.DetermineFile())
            {
                _drawingHostServices.PasteFrom(_openFileService.FileName);
            }
        }

        #endregion

        #region LockSelected command

        private Command _lockSelectedCommand;

        /// <summary>
        /// Gets the LockSelected command.
        /// </summary>
        public Command LockSelectedCommand
        {
            get { return _lockSelectedCommand ?? (_lockSelectedCommand = new Command(LockSelected)); }
        }

        /// <summary>
        /// Method to invoke when the LockSelected command is executed.
        /// </summary>
        private void LockSelected()
        {
            _drawingHostServices.LockSelected();
        }

        #endregion

        #region FigureList command

        private Command _figureListCommand;

        /// <summary>
        /// Gets the FigureList command.
        /// </summary>
        public Command FigureListCommand
        {
            get { return _figureListCommand ?? (_figureListCommand = new Command(FigureList)); }
        }

        /// <summary>
        /// Method to invoke when the FigureList command is executed.
        /// </summary>
        private void FigureList()
        {
            _drawingHostServices.FigureList();
        }

        #endregion
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}
