namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class DoubleEditorFactory : BaseValueEditorFactory<DoubleEditor, double>
    {
        public DoubleEditorFactory()
        {
            LoadOrder = 2;
        }
    }

    public class DoubleEditor : StringEditor
    {
        protected override ValidationResult Validate(object value)
        {
            var result = new ValidationResult();
            double doubleResult;
            var source = value.ToString();
            if (!string.IsNullOrEmpty(source) && double.TryParse(source, out doubleResult))
            {
                result.IsValid = true;
                result.Value = doubleResult;
            };
            return result;
        }
    }
}
