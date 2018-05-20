using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class ComplexTypeEditorFactory : BaseValueEditorFactory<ComplexTypeEditor>
    {
        public ComplexTypeEditorFactory()
        {
            LoadOrder = 3;
        }

        public override bool SupportsValue(IValueProvider property)
        {
            return true;
        }
    }

    public class ComplexTypeEditor : PropertyGrid, IValueEditor
    {
        public ComplexTypeEditor()
        {
            ValueDiscoveryStrategy = new ExcludeByDefaultValueDiscoveryStrategy();
            Expanded = false;
            this.Margin = new Thickness(32, 0, 0, 0);
        }

        IValueProvider mValue;
        public IValueProvider Value
        {
            get => mValue;
            set
            {
                if (mValue == value)
                {
                    return;
                }
                if (mValue != null)
                {
                    mValue.ValueChanged -= mValue_ValueChanged;
                }
                mValue = value;
                if (mValue != null)
                {
                    mValue.ValueChanged += mValue_ValueChanged;
                    var attribute = value.GetAttribute<PropertyGridComplexTypeStateAttribute>();
                    if (attribute != null && attribute.State == ComplexTypeState.Expanded)
                    {
                        mExpanded = true;
                    }
                }
                OnValueSet(mValue);
            }
        }

        void mValue_ValueChanged()
        {
            OnValueSet(mValue);
        }

        void OnValueSet(IValueProvider value)
        {
            object selection = null;
            if (value != null)
            {
                selection = value.GetValue<object>();
            }
            Selection = selection;
        }

        const int iconSize = 10;
        const double glyphSize = 0.2;

        Canvas GetPlusIcon()
        {
            var canvas = GetMinusIcon();
            var line = new Line();
            line.X1 = 5;
            line.X2 = 5;
            line.Y1 = 3;
            line.Y2 = 8;
            canvas.Width = 10;
            canvas.Height = canvas.Width;
            canvas.Children.Add(line);
            return canvas;
        }

        Canvas GetMinusIcon()
        {
            var canvas = new Canvas();
            var line = new Line();
            line.X1 = 3;
            line.X2 = 8;
            line.Y1 = 5;
            line.Y2 = 5;
            canvas.Width = 10;
            canvas.Height = canvas.Width;
            canvas.Children.Add(line);
            return canvas;
        }

        Border expandCollapse;

        protected override void AddHeader()
        {
            var header = new StackPanel();
            header.Margin = new Thickness(-32, 0, 0, 0);
            header.Orientation = Orientation.Horizontal;
            header.VerticalAlignment = VerticalAlignment.Center;
            expandCollapse = new Border();
            expandCollapse.Background = new SolidColorBrush(Colors.White);
            expandCollapse.BorderBrush = new SolidColorBrush(Colors.Black);
            expandCollapse.BorderThickness = new Thickness(1);
            expandCollapse.MouseLeftButtonDown += expandCollapse_Click;
            UpdateExpandCollapseGlyph();
            expandCollapse.HorizontalAlignment = HorizontalAlignment.Center;
            expandCollapse.VerticalAlignment = VerticalAlignment.Center;
            expandCollapse.Margin = new Thickness(4);
            expandCollapse.Padding = new Thickness();
            header.Children.Add(expandCollapse);

            var name = new TextBlock();
            name.Margin = new Thickness(4, 0, 4, 0);
            name.VerticalAlignment = VerticalAlignment.Center;
            name.Text = Value.DisplayName;
            header.Children.Add(name);

            var value = Value.GetValue<object>();
            var contentsText = (value ?? "").ToString();
            if (!string.IsNullOrEmpty(contentsText) && value != null && contentsText != value.GetType().ToString())
            {
                var contents = new TextBlock();
                contents.MaxWidth = 200;
                contents.MaxHeight = 30;
                contents.Margin = new Thickness(16, 0, 0, 0);
                contents.VerticalAlignment = VerticalAlignment.Center;
                contents.Text = contentsText;
                header.Children.Add(contents);
            }
            this.Children.Add(header);
        }

        protected override void AddChildren()
        {
            var controls = CreateObjectControls(Selection);
            if (controls.IsEmpty())
            {
                expandCollapse.Visibility = Visibility.Collapsed;
                return;
            }
            foreach (var control in controls)
            {
                this.Children.Add(control);
            }
        }

        void expandCollapse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Expanded = !Expanded;
            UpdateExpandCollapseGlyph();
        }

        void UpdateExpandCollapseGlyph()
        {
            expandCollapse.Child = Expanded ? GetMinusIcon() : GetPlusIcon();
        }
    }
}