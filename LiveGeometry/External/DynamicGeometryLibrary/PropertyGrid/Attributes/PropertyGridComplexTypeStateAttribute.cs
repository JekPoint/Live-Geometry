using System;

namespace DynamicGeometry.PropertyGrid.Attributes
{
    public class PropertyGridComplexTypeStateAttribute : Attribute
    {
        public PropertyGridComplexTypeStateAttribute(ComplexTypeState state)
        {
            State = state;
        }

        public ComplexTypeState State { get; set; }
    }
}