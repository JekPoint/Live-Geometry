using System.Collections.Generic;
using DynamicGeometry.Behaviors;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Lists;

namespace DynamicGeometry.Macros
{
    [Ignore]
    public class MacroResultSelector : FigureSelector
    {
        public MacroResultSelector(Drawing drawing, IList<IFigure> inputs)
        {
            Inputs = inputs;
            Drawing = drawing;
        }

        public IList<IFigure> Inputs { get; set; }

        protected override bool CanSelectFigure(IFigure figure)
        {
            if (Inputs.Contains(figure))
            {
                return false;
            }
            if (DependencyAlgorithms.FigureCompletelyDependsOnFigures(figure, Inputs))
            {
                return true;
            }
            return false;
        }

        protected override void TrySelectFigure(IFigure figure)
        {
            base.TrySelectFigure(figure);
        }
    }
}
