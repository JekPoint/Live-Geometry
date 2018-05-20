using System;
using System.ComponentModel.Composition;
using DynamicGeometry.Figures;

namespace DynamicGeometry.Expressions.Parser
{
    [Export(typeof(ICompilerService))]
    public class Compiler : ICompilerService
    {
        [Import]
        public IExpressionTreeEvaluatorProvider ExpressionTreeEvaluatorProvider { get; set; }

        public CompileResult CompileFunction(Drawing drawing, string functionText)
        {
            var result = new CompileResult();
            if (string.IsNullOrEmpty(functionText))
            {
                return result;
            }

            var ast = Parse(functionText, result);
            if (!result.Errors.IsEmpty())
            {
                return result;
            }

            var builder = new ExpressionTreeBuilder();
            builder.SetContext(drawing, f => true);
            var expressionTree = builder.CreateFunction(ast, result);
            if (expressionTree == null || !result.Errors.IsEmpty())
            {
                return result;
            }

            var function = ExpressionTreeEvaluatorProvider.InterpretFunction(expressionTree);
            result.Function = function;
            return result;
        }

        public CompileResult CompileExpression(
            Drawing drawing,
            string expressionText,
            Predicate<IFigure> isFigureAllowed)
        {
            var result = new CompileResult();
            if (expressionText.IsEmpty())
            {
                return result;
            }

            var ast = Parse(expressionText, result);
            if (!result.Errors.IsEmpty())
            {
                return result;
            }

            var builder = new ExpressionTreeBuilder();
            builder.SetContext(drawing, isFigureAllowed);
            var expressionTree = builder.CreateExpression(ast, result);
            if (expressionTree == null || !result.Errors.IsEmpty())
            {
                return result;
            }
            var function = ExpressionTreeEvaluatorProvider.InterpretExpression(expressionTree);
            result.Expression = function;
            return result;
        }

        private static Node Parse(string text, CompileResult result)
        {
            var ast = Parser.Parse(text);
            if (!ast.Errors.IsEmpty())
            {
                result.Errors.AddRange(ast.Errors);
            }
            return ast.Root;
        }
    }
}
