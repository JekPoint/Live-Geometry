namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class PolarEditorFactory : BaseValueEditorFactory<PolarEditor>
    {
        public override bool SupportsValue(IValueProvider value)
        {
            return value.Type == typeof(PolarValue) && base.SupportsValue(value);
        }
    }

    public class PolarEditor : SelectorValueEditor, IValueEditor
    {        
        public override void FillList()
        {
            Items = DynamicGeometry.Settings.Instance.PolarItems;
            base.FillList();
        }

        protected override ValidationResult Validate(object value)
        {

            var ret = new ValidationResult();

            var CPolarVal = new PolarValue();

            var strValue = value.ToString();
            if (Math.IsDoubleValid(strValue))
            {
                ret.IsValid = true;
                CPolarVal.Val = System.Convert.ToDouble(strValue);
                ret.Value = CPolarVal;
            }
            else
            {
                ret.IsValid = false;
            }

            return ret;
        }

        public override void UpdateEditor()
        {
            var value = GetValue();
            var CPolarVal = (PolarValue)value;

            foreach (var item in Items)
            {
                if (item.Equals(CPolarVal.Val.ToString()))
                {
                    Selector.SelectedItem = item;
                }
            }
        }
    }
}
