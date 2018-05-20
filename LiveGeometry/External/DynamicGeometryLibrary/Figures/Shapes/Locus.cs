using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.Figures.Points.PointOnFigure;

namespace DynamicGeometry.Figures.Shapes
{
    public class Locus : Curve, ILinearFigure
    {
        private List<IFigure> figuresToRecalculate;

        int StepCount => 60;

        public Locus()
        {
            for (var i = 0; i < StepCount; i++)
            {
                pathSegments.Add(new LineSegment());
            }
        }

        public override void Recalculate()
        {
            if (figuresToRecalculate == null)
            {
                var point = mDependencies[0] as IPoint;
                var pointOnFigure = mDependencies[1] as PointOnFigure;
                figuresToRecalculate = GetFiguresToRecalculate(pointOnFigure, point);
            }
        }

        protected override void OnDependenciesChanged()
        {
            figuresToRecalculate = null;
        }

        public override void GetPoints(List<Point> result)
        {
            if (figuresToRecalculate == null)
            {
                return;
            }

            result.Capacity = StepCount + 1;

            var point = mDependencies[0] as IPoint;
            var pointOnFigure = mDependencies[1] as PointOnFigure;
            var figure = pointOnFigure.LinearFigure;
            var domain = figure.GetParameterDomain();
            var oldParameter = pointOnFigure.Parameter;
            var steps = StepCount;
            if (steps == 0)
            {
                return;
            }

            var step = (domain.Item2 - domain.Item1) / steps;
            var end = domain.Item2 - step;
            for (var lambda = domain.Item1; lambda < end; lambda += step)
            {
                pointOnFigure.Parameter = lambda;
                for (var i = 0; i < figuresToRecalculate.Count; i++)
                {
                    figuresToRecalculate[i].Recalculate();
                }
                result.Add(point.Coordinates);
            }

            pointOnFigure.Parameter = domain.Item2;
            for (var i = 0; i < figuresToRecalculate.Count; i++)
            {
                figuresToRecalculate[i].Recalculate();
            }
            result.Add(point.Coordinates);

            pointOnFigure.Parameter = oldParameter;
            for (var i = 0; i < figuresToRecalculate.Count; i++)
            {
                figuresToRecalculate[i].Recalculate();
            }
        }

        List<IFigure> GetFiguresToRecalculate(PointOnFigure pointOnFigure, IPoint dependentPoint)
        {
            return DependencyAlgorithms.FindImpactedDependencyChain(pointOnFigure, dependentPoint);
        }
    }
}
