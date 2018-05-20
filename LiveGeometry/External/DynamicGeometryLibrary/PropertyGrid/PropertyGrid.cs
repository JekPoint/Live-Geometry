using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicGeometry.PropertyGrid.Attributes;
using DynamicGeometry.PropertyGrid.Editors;
using GuiLabs.Undo;

namespace DynamicGeometry.PropertyGrid
{
    public partial class PropertyGrid : StackPanel
    {
        public PropertyGrid()
        {
            ValueDiscoveryStrategy = new IncludeByDefaultValueDiscoveryStrategy();
        }

        public IValueDiscoveryStrategy ValueDiscoveryStrategy { get; set; }

        private object mSelection;
        public object Selection
        {
            get => mSelection;
            protected set
            {
                if (mSelection != null)
                {
                    UnsubscribeFromPropertyChangeNotifications(mSelection);
                }
                mSelection = value;
                var customContent = mSelection as IPropertyGridContentProvider;
                if (customContent != null)
                {
                    mSelection = customContent.GetContentForPropertyGrid();
                }
                if (mSelection != null)
                {
                    SubscribeToPropertyChangeNotifications(value);
                    UpdateVisibility(true);
                }
                else
                {
                    UpdateVisibility(false);
                }
                UpdateContents();
            }
        }

        public event EventHandler VisibilityChanged;
        public void UpdateVisibility(bool visible)
        {
            Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (VisibilityChanged != null)
            {
                VisibilityChanged(this, null);
            }
        }

        void SubscribeToPropertyChangeNotifications(object instance)
        {
            var objectWithEvent = instance as INotifyPropertyChanged;
            if (objectWithEvent != null)
            {
                objectWithEvent.PropertyChanged += this.SelectionPropertyChanged;
            }
            var supportsHost = instance as IPropertyGridHost;
            if (supportsHost != null)
            {
                supportsHost.PropertyGrid = this;
            }
        }

        void UnsubscribeFromPropertyChangeNotifications(object instance)
        {
            var objectWithEvent = instance as INotifyPropertyChanged;
            if (objectWithEvent != null)
            {
                objectWithEvent.PropertyChanged -= SelectionPropertyChanged;
            }
            var supportsHost = instance as IPropertyGridHost;
            if (supportsHost != null)
            {
                supportsHost.PropertyGrid = null;
            }
        }

        void SelectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FindAndUpdatePropertyEditor(e.PropertyName);
        }

        void FindAndUpdatePropertyEditor(string propertyName)
        {
            if (CurrentProperties == null)
            {
                return;
            }
            foreach (var value in CurrentProperties)
            {
                if (value.Name == propertyName)
                {
                    value.RaiseValueChanged();
                    return;
                }
            }
        }

        private void Show(object newSelection)
        {
            Selection = newSelection;
        }

        public ActionManager ActionManager { get; set; }

        public void Show(object newSelection, ActionManager actionManager)
        {
            ActionManager = actionManager;
            Selection = newSelection;
        }

        public void Show(IEnumerable<object> objects, ActionManager actionManager)
        {
            if (objects == null || objects.Count() == 0)
            {
                Selection = null;
                return;
            }
            ActionManager = actionManager;
            var aggregate = new CompositePropertyProvider(this.ValueDiscoveryStrategy, objects);
            var properties = aggregate.GetProperties();
            if (properties.IsEmpty())
            {
                aggregate = null;
            }
            Selection = aggregate;
        }

        protected bool mExpanded = true;
        public bool Expanded
        {
            get => mExpanded;
            set
            {
                if (mExpanded == value)
                {
                    return;
                }
                mExpanded = value;
                UpdateContents();
            }
        }

        void UpdateContents()
        {
            this.Children.Clear();
            if (Selection == null)
            {
                return;
            }
            AddHeader();
            if (!Expanded)
            {
                return;
            }
            AddChildren();
        }

        protected virtual void AddChildren()
        {
            var controls = CreateObjectControls(Selection);
            if (controls == null)
            {
                return;
            }
            foreach (var control in controls)
            {
                this.Children.Add(control);
            }
        }

        protected virtual void AddHeader()
        {
            var title = Title;
            if (string.IsNullOrEmpty(title))
            {
                title = GetTitleString(Selection);
            }
            Header = GetTitleControl(title);
            this.Children.Add(Header);
        }

        public void UpdateHeader()
        {
            var title = Title;
            if (string.IsNullOrEmpty(title))
            {
                title = GetTitleString(Selection);
            }
            (Header as TextBlock).Text = title;  
        }

        UIElement Header { get; set; }

        static UIElement GetTitleControl(string title)
        {
            return new TextBlock()
            {
                Text = title,
                FontSize = 20,
                Margin = new Thickness(0, 0, 0, 8),
                Foreground = new SolidColorBrush(Settings.PropertyGridTitleColor),
                IsHitTestVisible = false
            };
        }

        public string Title { get; set; }

        protected static string GetTitleString(object editableObject)
        {
            var result = editableObject.ToString();
            var type = editableObject.GetType();
            var attribute = type.GetAttribute<PropertyGridNameAttribute>();
            if (attribute != null)
            {
                result = attribute.Name;
            }
            return result;
        }

        public IEnumerable<IValueProvider> CurrentProperties { get; set; }

        public IEnumerable<UIElement> CreateObjectControls<T>(T editableObject)
        {
            CurrentProperties = GetEditableProperties(editableObject).ToArray();
            var currentEditors = CurrentProperties
                .Select(p => CreatePropertyEditorControl(p, editableObject, ActionManager))
                .Where(c => c != null).ToArray();
            var currentMethods = GetCallableMethods(editableObject);
            currentMethods = currentMethods.Concat(GetExtensionMethods(editableObject));
            var currentMethodButtons = currentMethods
                .Select(m => CreateMethodCallerControl(m, editableObject)).ToArray();

            return currentEditors.Concat(currentMethodButtons).ToArray();
        }

        [ImportMany]
        public IEnumerable<IOperationProvider> ExtensionOperations { get; set; }

        private IEnumerable<IOperationDescription> GetExtensionMethods(object editableObject)
        {
            if (ExtensionOperations == null)
            {
                yield break;
            }

            foreach (var item in ExtensionOperations)
            {
                var operation = item.ProvideOperation(editableObject);
                if (operation != null)
                {
                    yield return operation;
                }
            }
        }

        protected virtual IEnumerable<IValueProvider> GetEditableProperties<T>(T editableObject)
        {
            var discoveryStrategy = 
                DynamicGeometry.PropertyGrid.ValueDiscoveryStrategy.Get(editableObject.GetType())
                ?? this.ValueDiscoveryStrategy;
            var result = discoveryStrategy.GetValues(editableObject);
            return result;
        }

        static UIElement CreateMethodCallerControl(IOperationDescription m, object obj)
        {
            return new MethodCallerButton() { OperationDescription = m, Target = obj };
        }

        private static IEnumerable<IOperationDescription> GetCallableMethods(object editableObject)
        {
            if (editableObject is ICustomMethodProvider)
            {
                return (editableObject as ICustomMethodProvider).GetMethods();
            }

            IEnumerable<MethodInfo> allMethods = editableObject.GetType().GetMethods();
            allMethods = allMethods
                .Where(m => m.ReturnType == typeof(void)
                    && !m.IsSpecialName
                    && m.IsPublic
                    && m.HasAttribute<PropertyGridVisibleAttribute>());

            var result = allMethods.Select(m => (IOperationDescription)MethodDescription.Create(m));

            return result;
        }

        static IEnumerable<IValueEditorFactory> Factories = Reflector.DiscoverTypesAndInstantiate<IValueEditorFactory>();

        static UIElement CreatePropertyEditorControl(IValueProvider p, object obj, ActionManager actionManager)
        {
            var factory = SelectProperFactory(p);
            if (factory != null)
            {
                try
                {
                    var valueEditor = factory.CreateEditor(p);
                    valueEditor.ActionManager = actionManager;
                    var result = valueEditor as UIElement;
                    if (result != null)
                    {
                        HookupEvents(p, result, obj);
                        return result;
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        private static IValueEditorFactory SelectProperFactory(IValueProvider p)
        {
            var candidates = Factories.Where(f => f.SupportsValue(p)).OrderBy(f => f.LoadOrder);
            var candidate = candidates.FirstOrDefault();
            var attribute = p.GetAttribute<PropertyGridPreferredEditorAttribute>();
            if (attribute != null && !attribute.EditorTypeName.IsEmpty())
            {
                var substringCandidates = candidates.Where(f => f.GetType().Name.Contains(attribute.EditorTypeName));
                if (substringCandidates.Count() > 0)
                {
                    candidate = substringCandidates.First();
                }
            }
            return candidate;
        }

        static void HookupEvents(IValueProvider p, UIElement result, object obj)
        {
            var eventAttributes = p
                .GetAttributes<PropertyGridEventAttribute>();
            if (eventAttributes != null)
            {
                foreach (var eventAttribute in eventAttributes)
                {
                    HookupEvent(result, eventAttribute, obj);
                }
            }
        }

        static void HookupEvent(UIElement control, PropertyGridEventAttribute eventAttribute, object model)
        {
            var foundEvent = Reflector.FindEventByName(control.GetType(), eventAttribute.EventName);
            if (foundEvent != null)
            {
                try
                {
                    var d = Delegate.CreateDelegate(foundEvent.EventHandlerType, model, eventAttribute.HandlerName);
                    foundEvent.AddEventHandler(control, d);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public class MessageBoxDialog : IPropertyGridHost
    {
        [PropertyGridVisible]
        public virtual string Message => MessageText;

        public string MessageText { get; set; }

        [PropertyGridVisible]
        public void OK()
        {
            PropertyGrid.Show(null, null);
            OKClicked();
        }

        protected virtual void OKClicked()
        {
        }

        public PropertyGrid PropertyGrid { get; set; }
    }
}
