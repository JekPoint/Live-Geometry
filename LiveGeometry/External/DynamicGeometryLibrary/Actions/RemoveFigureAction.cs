﻿using System;
using System.Collections.Generic;
using System.Linq;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.Figures.Points;
using GuiLabs.Undo;

namespace DynamicGeometry.Actions
{
    public class RemoveFigureAction : GeometryAction
    {
        public RemoveFigureAction(Drawing drawing, IFigure figure)
            : base(drawing)
        {
            Figure = figure;
        }

        private IFigure Figure;
        private IFigure[] Deleted;
        private IList<IAction> CustomDependencyRemovers;

        protected override void ExecuteCore()
        {
            CustomDependencyRemovers = new List<IAction>();

            Deleted = Figure.AsEnumerable<IFigure>()
                .TopologicalSort(GetRemovableDependencies)
                .ToArray();

            foreach (var item in CustomDependencyRemovers)
            {
                item.Execute();
            }

            foreach (var item in Deleted)
            {
                Drawing.Figures.Remove(item);
            }

            Drawing.RaiseSelectionChanged(new Drawing.SelectionChangedEventArgs());
        }

        private IEnumerable<IFigure> GetRemovableDependencies(IFigure figure)
        {
            var list = figure.Dependents.ToList();

            foreach (var item in figure.Dependents)
            {
                var customDependencyRemover = item as ISupportRemoveDependency;
                if (customDependencyRemover != null 
                    && customDependencyRemover.CanRemoveDependency(figure))
                {
                    list.Remove(item);
                    CustomDependencyRemovers.Add(customDependencyRemover.GetRemoveDependencyAction(figure));
                }
            }

            return list;
        }

        protected override void UnExecuteCore()
        {
            // Suppress auto labeling to prevent duplicate labels. try-catch added to ensure suppression is always stopped.
            PointBase.SuppressAutoLabelPoints = true;
            try
            {
                foreach (var item in Deleted.Reverse())
                {
                    Drawing.Figures.Add(item);
                }

                foreach (var item in CustomDependencyRemovers.Reverse())
                {
                    item.UnExecute();
                }
            }
            catch (Exception ex)
            {
                Drawing.RaiseError(this, ex);
            }
            PointBase.SuppressAutoLabelPoints = false;
        }
    }
}