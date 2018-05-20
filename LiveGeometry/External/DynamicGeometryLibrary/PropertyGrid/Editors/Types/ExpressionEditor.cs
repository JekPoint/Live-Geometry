using DynamicGeometry.Expressions;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures;

namespace DynamicGeometry.PropertyGrid.Editors.Types
{
    public class DrawingExpressionEditorFactory
        : BaseValueEditorFactory<ExpressionEditor, DrawingExpression> { }

    public class ExpressionEditor : StringEditor
    {
        protected override ValidationResult Validate(object value)
        {
            var result = new ValidationResult();
            var source = value.ToString();

            var expression = Value as DrawingExpression;

            if (!string.IsNullOrEmpty(source))
            {
                var compileResult = MEFHost.Instance.CompilerService.CompileExpression(
                    expression.ParentFigure.Drawing,
                    source,
                    f => !f.DependsOn(expression.ParentFigure));

                if (compileResult.IsSuccess)
                {
                    result.IsValid = true;
                    result.Value = source;
                    expression.ParentFigure.Drawing.ClearStatus();
                }
                else
                {
                    result.Error = compileResult.ToString();
                    expression.ParentFigure.Drawing.RaiseStatusNotification(result.Error);
                }
            };
            return result;
        }
    }
}
