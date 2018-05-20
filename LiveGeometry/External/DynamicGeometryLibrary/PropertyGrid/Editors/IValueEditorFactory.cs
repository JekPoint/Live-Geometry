﻿namespace DynamicGeometry.PropertyGrid.Editors
{
    public interface IValueEditorFactory
    {
        bool SupportsValue(IValueProvider value);
        IValueEditor CreateEditor(IValueProvider value);
        int LoadOrder { get; set; }
    }
}
