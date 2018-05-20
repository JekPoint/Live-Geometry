using System;

namespace DynamicGeometry.Extensibility
{
    public class OrderAttribute : Attribute
    {
        public double Order { get; set; }

        public OrderAttribute(double order)
        {
            Order = order;
        }
    }
}
