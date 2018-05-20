using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Points;

namespace DynamicGeometry.Expressions.Parser
{
    public class ExpressionTreeBuilder
    {
        public ExpressionTreeBuilder()
        {
            Binder = new Binder();
        }

        public Binder Binder { get; set; }
        CompileResult Status { get; set; }

        public Expression<Func<double, double>> CreateFunction(Node root, CompileResult status)
        {
            Status = status;
            var parameter = Expression.Parameter(typeof(double), "x");
            Binder.RegisterParameter(parameter);
            var body = CreateExpressionCore(root);
            if (body == null)
            {
                return null;
            }
            var expressionTree = Expression.Lambda<Func<double, double>>(body, parameter);
            return expressionTree;
        }

        public Expression<Func<double>> CreateExpression(Node root, CompileResult status)
        {
            Status = status;
            var body = CreateExpressionCore(root);
            if (body == null)
            {
                return null;
            }
            // If expression does not return double, we'll get an error. - D.H.
            if (body.Type != typeof(double))
            {
                return null;
            }
            var expressionTree = Expression.Lambda<Func<double>>(body);
            return expressionTree;
        }

        Expression CreateExpressionCore(Node root)
        {
            switch (root.Kind)
            {
                case NodeType.Negation:
                    return CreateUnaryExpression(root);
                case NodeType.Addition:
                case NodeType.Subtraction:
                case NodeType.Multiplication:
                case NodeType.Division:
                case NodeType.Power:
                    return CreateBinaryExpression(root);
                case NodeType.Variable:
                    return CreateIdentifierExpression(root);
                case NodeType.Constant:
                    return CreateLiteralExpression(Convert.ToDouble(root.Token.Text));
                case NodeType.FunctionCall:
                    return CreateCallExpression(root);
                case NodeType.PropertyAccess:
                    return CreatePropertyAccessExpression(root);
                default:
                    return null;
            }
        }

        Expression CreateUnaryExpression(Node root)
        {
            var operand = CreateExpressionCore(root.Children[0]);
            if (operand == null)
            {
                return null;
            }

            return Expression.Negate(operand);
        }

        Expression CreateIdentifierExpression(Node root)
        {
            var text = root.Token.Text;
            var resolveTwoPoints = ResolveTwoPoints(text);
            if (resolveTwoPoints != null)
            {
                return resolveTwoPoints;
            }

            var parameter = Binder.Resolve(text);
            if (parameter == null)
            {
                Status.AddUnknownIdentifierError(text);
            }

            return parameter;
        }

        public Expression ResolveTwoPoints(string twoPoints)
        {
            var drawing = Binder.Drawing;
            if (drawing == null)
            {
                return null;
            }

            var names = drawing.Figures.Where(f => f is PointBase).Select(f => f.Name).ToArray();
            var longestPrefix = "";
            var longestSuffix = "";
            foreach (var name in names)
            {
                if (twoPoints.StartsWith(name, StringComparison.OrdinalIgnoreCase) && name.Length > longestPrefix.Length)
                {
                    longestPrefix = name;
                }
                if (twoPoints.EndsWith(name, StringComparison.OrdinalIgnoreCase) && name.Length > longestSuffix.Length)
                {
                    longestSuffix = name;
                }
            }

            if (longestPrefix.Length + longestSuffix.Length == twoPoints.Length)
            {
                var point1 = drawing.Figures[longestPrefix] as PointBase;
                var point2 = drawing.Figures[longestSuffix] as PointBase;

                if (point1 == null)
                {
                    Status.AddFigureIsNotAPointError(longestPrefix);
                    return null;
                }
                if (point2 == null)
                {
                    Status.AddFigureIsNotAPointError(longestSuffix);
                    return null;
                }
                if (!Binder.FigureAllowed(point1))
                {
                    Status.AddDependencyCycleError(longestPrefix);
                    return null;
                }
                if (!Binder.FigureAllowed(point2))
                {
                    Status.AddDependencyCycleError(longestSuffix);
                    return null;
                }

                var p1 = Expression.Constant(point1);
                var p2 = Expression.Constant(point2);
                var distance = typeof(Math).GetMethod("Distance",
                    new[] { typeof(PointBase), typeof(PointBase) });
                var result = Expression.Call(null, distance, p1, p2);
                Status.Dependencies.Add(point1);
                Status.Dependencies.Add(point2);
                return result;
            }

            return null;
        }

        Expression CreatePropertyAccessExpression(Node root)
        {
            var figureName = root.Children[0].Token.Text;
            var propertyName = root.Children[1].Token.Text;

            var figure = Binder.ResolveFigure(figureName);
            if (figure == null)
            {
                Status.AddUnknownIdentifierError(figureName);
                return null;
            }

            if (!Binder.IsFigureAllowed(figure))
            {
                Status.AddDependencyCycleError(figureName);
                return null;
            }

            var type = figure.GetType();
            var property = type.GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                Status.AddPropertyNotFoundError(figure, propertyName);
                return null;
            }

            Status.Dependencies.Add(figure);
            var figureExpression = Expression.Constant(figure);
            var propertyExpression = Expression.Property(figureExpression, property);
            return propertyExpression;
        }

        Expression CreateCallExpression(Node root)
        {
            var functionName = root.Token.Text;
            var method = Binder.ResolveMethod(functionName);
            if (method == null)
            {
                Status.AddMethodNotFoundError(functionName);
                return null;
            }

            var arguments = root.Children;
            if (arguments.Count == 1)
            {
                var argument = CreateExpressionCore(arguments[0]);
                if (argument == null)
                {
                    return null;
                }

                return Expression.Call(method, argument);
            }

            return CreatePointFunctionCallExpression(method, arguments);
        }

        Expression CreatePointFunctionCallExpression(MethodInfo method, IEnumerable<Node> arguments)
        {
            var points = new List<IPoint>();
            foreach (var node in arguments)
            {
                var pointName = node.Token.Text;
                var point = ResolvePoint(pointName);
                if (point == null)
                {
                    return null;
                }
                points.Add(point);
            }

            if (method.Name == "Area")
            {
                return Expression.Call(method, Expression.Constant(points.ToArray()));
            }

            if (method.GetParameters().Length != points.Count)
            {
                Status.AddIncorrectNumberOfArgumentsError(method, arguments.Count());
                return null;
            }

            var pointArguments = new List<Expression>();
            foreach (var point in points)
            {
                var pointArgument = CreatePointExpression(point);
                if (pointArgument == null)
                {
                    return null;
                }
                pointArguments.Add(pointArgument);
            }

            return Expression.Call(method, pointArguments.ToArray());
        }

        Expression CreatePointExpression(IPoint point)
        {
            var pointExpression = Expression.Constant(point);
            var coordinatesProperty = typeof(IPoint).GetProperty("Coordinates");
            var coordinates = Expression.Property(pointExpression, coordinatesProperty);
            return coordinates;
        }

        IPoint ResolvePoint(string pointName)
        {
            var figure = Binder.ResolveFigure(pointName);
            if (figure == null)
            {
                Status.AddUnknownIdentifierError(pointName);
                return null;
            }
            var point = figure as IPoint;
            if (point == null)
            {
                Status.AddFigureIsNotAPointError(pointName);
                return null;
            }
            Status.Dependencies.Add(point);
            return point;
        }

        Expression CreateLiteralExpression(double arg)
        {
            return Expression.Constant(arg);
        }

        Expression CreateBinaryExpression(Node node)
        {
            var left = CreateExpressionCore(node.Children[0]);
            var right = CreateExpressionCore(node.Children[1]);

            if (left == null || right == null)
            {
                return null;
            }

            switch (node.Kind)
            {
                case NodeType.Addition:
                    return Expression.Add(left, right);
                case NodeType.Subtraction:
                    return Expression.Subtract(left, right);
                case NodeType.Multiplication:
                    return Expression.Multiply(left, right);
                case NodeType.Division:
                    return Expression.Divide(left, right);
                case NodeType.Power:
                    return Expression.Power(left, right);
            }
            return null;
        }

        public void SetContext(Drawing drawing, Predicate<IFigure> isFigureAllowed)
        {
            Binder.Drawing = drawing;
            Binder.FigureAllowed = isFigureAllowed;
        }
    }
}
