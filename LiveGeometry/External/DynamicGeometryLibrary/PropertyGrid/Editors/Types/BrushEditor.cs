using System.Windows;
using System.Windows.Media;
using DynamicGeometry.Controls;
using DynamicGeometry.Controls.ColorPickerHelpers;

namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class BrushEditorFactory
        : BaseValueEditorFactory<BrushEditor, Brush> { }

    public class BrushEditor : LabeledValueEditor, IValueEditor
    {
        public ColorPicker Picker { get; set; }

        protected override UIElement CreateEditor()
        {
            Picker = new ColorPicker();
            Picker.SelectedColorChanging += ColorChanged;
            Picker.VerticalAlignment = VerticalAlignment.Top;
            return Picker;
        }

        void ColorChanged(object sender, SelectedColorEventArgs e)
        {
            SetValue(new SolidColorBrush(e.SelectedColor));
        }

        public override void UpdateEditor()
        {
            var brush = GetValue<Brush>();
            var solidColorBrush = brush as SolidColorBrush;
            if (solidColorBrush != null)
            {
                Picker.SelectedColor = solidColorBrush.Color;
            }
            else
            {
                Picker.SelectedColor = Colors.Black;
            }

            Picker.IsHitTestVisible = Value.CanSetValue;
        }
    }
}