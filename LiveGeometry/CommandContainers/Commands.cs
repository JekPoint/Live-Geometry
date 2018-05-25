using System.Windows.Input;
using InputGesture = Catel.Windows.Input.InputGesture;

namespace LiveGeometry.CommandContainers
{
    internal static class Commands
    {
        #region File
        public const string NewProject = "NewProject";
        public static readonly InputGesture NewProjectInputGesture = new InputGesture(Key.N, ModifierKeys.Control);

        public const string OpenProject = "OpenProject";
        public static readonly InputGesture OpenProjectInputGesture = new InputGesture(Key.O, ModifierKeys.Control);

        public const string SaveProject = "SaveProject";
        public static readonly InputGesture SaveProjectInputGesture = new InputGesture(Key.S, ModifierKeys.Control);
        #endregion

        #region Editor
        public const string Undo = "Undo";
        public static readonly InputGesture UndoInputGesture = new InputGesture(Key.Z, ModifierKeys.Control);

        public const string Redo = "Redo";
        public static readonly InputGesture RedoInputGesture = new InputGesture(Key.Y, ModifierKeys.Control);

        public const string Cut = "Cut";
        public static readonly InputGesture CutInputGesture = new InputGesture(Key.X, ModifierKeys.Control);

        public const string Copy = "Copy";
        public static readonly InputGesture CopyInputGesture = new InputGesture(Key.C, ModifierKeys.Control);

        public const string Paste = "Paste";
        public static readonly InputGesture PasteInputGesture = new InputGesture(Key.V, ModifierKeys.Control);

        public const string PasteFrom = "Paste from ...";

        public const string Delete = "Delete";
        public static readonly InputGesture DeleteInputGesture = new InputGesture(Key.Delete);

        public const string Lock = "Lock";

        public const string SelectAll = "Select all";
        public static readonly InputGesture SelectAllInputGesture = new InputGesture(Key.A, ModifierKeys.Control);
        #endregion

        #region View

        public const string FigureList = "Figure List";
        public static readonly InputGesture FigureListInputGesture = new InputGesture(Key.L, ModifierKeys.Shift);

        #endregion

        internal const string Extension = "lgf";
        internal const string LgfFileFilter = "Live Geometry file (*." + Extension + ")|*." + Extension;
        internal const string PngFileFilter = "PNG image (*.png)|*.png";
        internal const string BmpFileFilter = "BMP image (*.bmp)|*.bmp";
        internal const string DxfFileFilter = "DXF file (*.dxf)|*.dxf";
        internal const string AllFileFilter = "All files (*.*)|*.*";
        internal const string DrawingFileFilter = LgfFileFilter + "|" + DxfFileFilter + "|" + AllFileFilter;

        internal const string FileFilter = LgfFileFilter
                                          + "|" + PngFileFilter
                                          + "|" + BmpFileFilter
                                          + "|" + AllFileFilter;
    }
}
        