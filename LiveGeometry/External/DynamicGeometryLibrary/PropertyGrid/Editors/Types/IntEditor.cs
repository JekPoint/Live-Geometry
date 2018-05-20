namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class IntEditorFactory : BaseValueEditorFactory<IntEditor, int> { }

    public class IntEditor : StringEditor
    {
        protected override ValidationResult Validate(object value)
        {
            var result = new ValidationResult();
            var source = value.ToString();
            var intValue = 0;
            if (!string.IsNullOrEmpty(source) && int.TryParse(source, out intValue))
            {
                result.IsValid = true;
                result.Value = intValue;
            };
            return result;
        }
    }
}
