﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Shapes;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.Styles
{
    [StyleFor(typeof(IShapeWithInterior))]
    public class BackgroundStyle : FigureStyle
    {
        public override FrameworkElement GetSampleGlyph()
        {
            var polygon = Factory.CreatePolygonShape();
            polygon.Points = new PointCollection() 
            {
                new Point(0, 20),
                new Point(10, 0),
                new Point(20, 20)
            };
            polygon.Apply(this.GetWpfStyle());
            polygon.Tag = this;
            return polygon;
        }

        Brush mBackground = new SolidColorBrush(Colors.Yellow);
        [PropertyGridVisible]
        public Brush Background
        {
            get => mBackground;
            set
            {
                mBackground = value;
                OnPropertyChanged("Background");
            }
        }

        bool mIsFilled = true;
        [PropertyGridVisible]
        public bool IsFilled
        {
            get => mIsFilled;
            set
            {
                mIsFilled = value;
                OnPropertyChanged("IsFilled");
            }
        }

        protected override void ApplyToWpfStyle(Style existingStyle, IFigure figure)
        {
            base.ApplyToWpfStyle(existingStyle, figure);
            var brush = Background;
            if (!IsFilled)
            {
                brush = null;
            }

            var fillSetter = new Setter(Canvas.BackgroundProperty, brush);
            existingStyle.Setters.Add(fillSetter);
        }

        public override string ToString()
        {
            return base.ToString() + " " + Background.ToString() + " " + IsFilled.ToString();
        }
    }
}
