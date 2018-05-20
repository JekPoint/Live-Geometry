using System;
using DynamicGeometry.Figures;

namespace DynamicGeometry.Expressions
{
    public interface ICompilerService
    {
        CompileResult CompileFunction(Drawing drawing, string functionText);
        CompileResult CompileExpression(
            Drawing drawing,
            string expressionText,
            Predicate<IFigure> isFigureAllowed);
    }
}
