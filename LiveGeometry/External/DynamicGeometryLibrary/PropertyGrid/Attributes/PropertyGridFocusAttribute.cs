using System;

namespace DynamicGeometry.PropertyGrid.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method,
        AllowMultiple = false, 
        Inherited = true)]
    public class PropertyGridFocusAttribute : Attribute
    {
    }
}
