using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Data;
using Catel.MVVM;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Serialization;
using DynamicGeometry.UI;
using LiveGeometry.Services.Interfaces;
using Drawing = DynamicGeometry.Drawing;

namespace LiveGeometry.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DrawingHostServices : ModelBase, IDrawingHostServices
    {
        private const string DxfExtension = "dxf";

        public DrawingHostServices() 
        {
            MainDrawingHost = new DrawingHost();
            AddBehaviors();
            InitializeCommands();
        }

        #region MainDrawingHost

        /// <summary>
        /// Gets or sets the PropertyName value.
        /// </summary>
        [Model]
        public DrawingHost MainDrawingHost
        {
            get => GetValue<DrawingHost>(PropertyNameProperty);
            private set => SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        /// PropertyName property data.
        /// </summary>
        public static readonly PropertyData PropertyNameProperty = RegisterProperty("PropertyName", typeof(DrawingHost));

        #endregion

        public void OpenFile(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                switch (Path.GetExtension(filePath))
                {
                    case "." + DxfExtension:
                        MainDrawingHost.DrawingControl.Drawing =
                            (new DxfDrawingDeserializer().ReadDrawing(filePath, MainDrawingHost.DrawingControl));

                        break;
                    default:
                        MainDrawingHost.DrawingControl.Drawing = Drawing.Load(filePath, MainDrawingHost.DrawingControl);

                        break;
                }
            }
        }

        public void SaveToBitmap(string filePath)
        {
            var actualExtension = filePath.Substring(filePath.LastIndexOf('.')).ToLower();
            switch (actualExtension)
            {
                case ".png":
                    SaveToPng(filePath);
                    break;
                case ".bmp":
                    SaveToBmp(filePath);
                    break;
                default:
                    MainDrawingHost.CurrentDrawing.Save(filePath);
                    break;
            }
        }

        public void NewDocument()
        {
            MainDrawingHost.Clear();
        }

        public void Copy()
        {
            MainDrawingHost.CurrentDrawing.Copy();
        }

        public void Paste()
        {
            MainDrawingHost.CurrentDrawing.Paste();
        }

        public void PasteFrom(string filePath)
        {
            MainDrawingHost.CurrentDrawing.PasteFrom(filePath);
        }

        public void LockSelected()
        {
            MainDrawingHost.CurrentDrawing.LockSelected();
        }

        public void FigureList()
        {
            MainDrawingHost.CommandShowFigureExplorer.Execute();
        }

        private void AddBehaviors()
        {
            var behaviors = Behavior.LoadBehaviors(typeof(Dragger).Assembly).ToList();
            Behavior.Default = behaviors.First(b => b is Dragger);
            foreach (var behavior in behaviors)
            {
                MainDrawingHost.AddToolButton(behavior);
            }
        }

        private void InitializeCommands()
        {
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleGrid);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleOrtho);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleSnapToGrid);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleSnapToPoint);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleSnapToCenter);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandToggleLabelNewPoints);
            MainDrawingHost.AddToolbarButton(MainDrawingHost.CommandTogglePolar);
        }

        private void SaveToPng(string filePath)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(MainDrawingHost.DrawingControl, filePath, encoder);
        }

        private void SaveToBmp(string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(MainDrawingHost.DrawingControl, fileName, encoder);
        }
        
        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap(
                (int)visual.ActualWidth,
                (int)visual.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
    }
}
    