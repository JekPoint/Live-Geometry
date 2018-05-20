using DynamicGeometry.UI;

namespace LiveGeometry.Services.Interfaces
{
    public interface IDrawingHostServices
    {
        DrawingHost MainDrawingHost { get; }

        void SaveToBitmap(string filePath);

        void OpenFile(string filePath);

        void NewDocument();

        void Paste();

        void Copy();

        void PasteFrom(string filePath);

        void LockSelected();

        void FigureList();
    }
}
