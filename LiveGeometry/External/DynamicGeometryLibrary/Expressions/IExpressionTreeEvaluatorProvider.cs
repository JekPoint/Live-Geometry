﻿using System.Linq.Expressions;

namespace DynamicGeometry.Expressions
{
    public interface IExpressionTreeEvaluatorProvider
    {
        T InterpretFunction<T>(Expression<T> node);
        T InterpretExpression<T>(Expression<T> node);
    }
}
