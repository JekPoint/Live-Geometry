using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Points;
using DynamicGeometry.Macros;
using DynamicGeometry.UI;
using DynamicGeometry.UI.Ribbon;

namespace DynamicGeometry.Behaviors
{
    partial class Behavior
    {
        public static event Action<Behavior> NewBehaviorCreated;
        public static event Action<Behavior> BehaviorDeleted;

        public BehaviorToolButton CreateToolButton()
        {
            return new BehaviorToolButton(this);
        }

        public static IEnumerable<Behavior> LoadBehaviors(Assembly assembly)
        {
            var result = new List<Behavior>();
            var basic = typeof(Behavior);

            foreach (var t in assembly.GetTypes())
            {
                if (basic.IsAssignableFrom(t)
                    && !t.IsAbstract
                    && t.GetConstructor(new Type[0]) != null
                    && !t.HasAttribute<IgnoreAttribute>())
                {
                    var instance = Activator.CreateInstance(t) as Behavior;
                    result.Add(instance);
                }
            }

            BehaviorOrderer.Order(result);

            return result;
        }

        protected virtual FreePoint CreatePointAtCurrentPosition(
            Point coordinates)
        {
            var result = Factory.CreateFreePoint(Drawing, coordinates);
            Actions.Actions.Add(Drawing, result);
            return result;
        }

        public static void Add(UserDefinedTool newBehavior)
        {
            if (Behavior.NewBehaviorCreated != null)
            {
                Behavior.NewBehaviorCreated(newBehavior);
            }
            ToolStorage.Instance.AddTool(newBehavior);
        }

        public static void Delete(UserDefinedTool tool)
        {
            if (Behavior.BehaviorDeleted != null)
            {
                Behavior.BehaviorDeleted(tool);
            }
            ToolStorage.Instance.RemoveTool(tool);
        }
    }
}
