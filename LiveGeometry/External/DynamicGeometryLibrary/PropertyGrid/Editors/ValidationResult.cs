﻿namespace DynamicGeometry.PropertyGrid.Editors
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public object Value { get; set; }
        public string Error { get; set; }
    }
}