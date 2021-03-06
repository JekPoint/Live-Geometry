﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicGeometry.UI.CommandSystem;

namespace DynamicGeometry.UI.Ribbon
{
    public class ToolButton : UserControl, ICommandObserver
    {
        public DrawingHost DrawingHost { get; set; }
        public TabPanel ParentPanel { get; set; }

        protected ButtonGrid buttonGrid;

        public virtual void Click()
        {

        }

        public virtual FrameworkElement CloneIcon()
        {
            if (buttonGrid == null)
            {
                return null;
            }
            var image = buttonGrid.Icon as Image;
            if (image != null)
            {
                var result = new Image();
                result.Source = image.Source;
                result.Stretch = Stretch.None;
                return result;
            }
            return null;
        }

        public void CommandRemoved()
        {
            var parent = Parent as Panel;
            if (parent != null)
            {
                parent.Children.Remove(this);
            }
        }

        public virtual void EnabledChanged(bool newEnabledState)
        {
            if (this.IsEnabled != newEnabledState)
            {
                this.IsEnabled = newEnabledState;
            }
        }

        public void IconChanged(FrameworkElement icon)
        {
            buttonGrid.Icon = icon;
        }
    }
}