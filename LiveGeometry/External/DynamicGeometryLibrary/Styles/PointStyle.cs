using System.Windows;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.Styles
{
    [StyleFor(typeof(IPoint))]
    public class PointStyle : ShapeStyle
    {
        public override FrameworkElement GetSampleGlyph()
        {
            var point = Factory.CreatePointShape();
            point.Apply(this.GetWpfStyle());
            point.Tag = this;
            return point;
        }

        double size = 10.0;
        [PropertyGridVisible]
        [Domain(3, 100)]
        public double Size
        {
            get => size;
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        protected override void ApplyToWpfStyle(Style existingStyle, IFigure figure)
        {
            base.ApplyToWpfStyle(existingStyle, figure);
            var size = Size;
            if (figure != null && figure.Selected && Settings.ChangePointAppearanceWhenSelected)
            {
                size += 3;
            }
            existingStyle.Setters.Add(new Setter(FrameworkElement.WidthProperty, size));
            existingStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, size));
        }
    }
}