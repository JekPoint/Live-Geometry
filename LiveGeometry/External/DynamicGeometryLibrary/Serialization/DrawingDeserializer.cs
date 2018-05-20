using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using DynamicGeometry.Extensibility;
using DynamicGeometry.Figures;
using DynamicGeometry.PropertyGrid;
using DynamicGeometry.Styles;

namespace DynamicGeometry.Serialization
{
    public partial class DrawingDeserializer
    {
        public static Drawing OpenDrawing(Canvas canvas, XElement element)
        {
            var drawing = new Drawing(canvas);
            var deserializer = new DrawingDeserializer();
            deserializer.ReadDrawing(drawing, element);

            if (!deserializer.IsSuccess)
            {
                throw new Exception(deserializer.GetErrorReport());
            }

            return drawing;
        }

        public static Drawing OpenDrawing(Canvas canvas, string savedDrawing)
        {
            Check.NotEmpty(savedDrawing);

            var element = XElement.Parse(savedDrawing);
            return OpenDrawing(canvas, element);
        }

        public virtual void ReadDrawing(Drawing drawing, XElement element)
        {
            Check.NotNull(drawing, "drawing");
            Check.NotNull(element, "element");
            drawing.Version = element.ReadDouble("Version");    // Defaults to 0 if Version attribute does not exist.
            ReadStyles(drawing, element);
            var figuresNode = element.Element("Figures");
            if (figuresNode == null)
            {
                // Perhaps notify user that no figures were found.
            }
            else
            {
                var figures = ReadFigures(figuresNode, drawing);
                foreach (var figure in figures)
                {
                    Actions.Actions.Add(drawing, figure);
                }
            }
            ReadViewport(drawing, element);
            DrawingUpdater updater;
#if TABULA
            updater = new TABDrawingUpdater();
#else
            updater = new DrawingUpdater();
#endif
            updater.UpdateIfNecessary(drawing);
            drawing.Recalculate();
            //drawing.CoordinateSystem.MoveTo(drawing.Figures.OfType<IPoint>().Midpoint().Minus());
        }

        List<string> errors = new List<string>();

        public void ReportError(string error)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                throw new Exception(error);
            }

            errors.Add(error);
        }

        public bool IsSuccess => errors.Count == 0;

        public string GetErrorReport()
        {
            return string.Join("\n", errors.ToArray());
        }

        private void ReadViewport(Drawing drawing, XElement element)
        {
            var viewportNode = element.Element("Viewport");
            if (viewportNode == null)
            {
                return;
            }

            var minX = viewportNode.ReadDouble("Left");
            var maxX = viewportNode.ReadDouble("Right");
            var minY = viewportNode.ReadDouble("Bottom");
            var maxY = viewportNode.ReadDouble("Top");
            drawing.CoordinateGrid.Locked = viewportNode.ReadBool("Locked", false);
            drawing.CoordinateGrid.Visible = viewportNode.ReadBool("Grid", Settings.Instance.ShowGrid);
            if (drawing.CoordinateGrid.Visible)
            {
                drawing.CoordinateGrid.ShowAxes = viewportNode.ReadBool("Axes", true);
            }
            drawing.CoordinateSystem.SetViewport(minX, maxX, minY, maxY);
            var styleName = viewportNode.ReadString("Style");    // Don't know who uses this.  I don't. - David
            if (!styleName.IsEmpty() && drawing.StyleManager != null)
            {
                var style = drawing.StyleManager[styleName];
                if (style != null)
                {
                    var wpfStyle = style.GetWpfStyle();
                    drawing.Canvas.Apply(wpfStyle);
                }
            }
            else if (viewportNode.ReadString("Color") != null)
            {
                drawing.Canvas.Background = new SolidColorBrush(viewportNode.ReadString("Color").ToColor());
            }
            else 
            {
                drawing.Canvas.Background = Brushes.White;
            }
        }

        public virtual void ReadStyles(Drawing drawing, XElement element)
        {
            var stylesNode = element.Element("Styles");
            drawing.StyleManager.Clear();
            if (stylesNode == null)
            {
                drawing.StyleManager.AddDefaultStyles();
                return;
            }

            foreach (var styleNode in stylesNode.Elements())
            {
                var style = ReadStyle(styleNode);
                drawing.StyleManager.Add(style);
            }
        }

        private IFigureStyle ReadStyle(XElement styleNode)
        {
            return MEFHost.Instance.SerializationService.Read<IFigureStyle>(styleNode);
        }

        private IValueDiscoveryStrategy valueDiscovery = new IncludeByDefaultValueDiscoveryStrategy();

        public virtual void ReadDrawing(Drawing drawing, string savedDrawing)
        {
            var element = XElement.Parse(savedDrawing);
            ReadDrawing(drawing, element);
        }

        public virtual void ReadFigureList(IList<IFigure> figureList, XElement element, Drawing drawing)
        {
            if (element.Name == "Drawing")
            {
                element = element.Element("Figures");
            }

            var figures = ReadFigures(element, drawing);
            figureList.AddRange(figures);
        }

        protected virtual IEnumerable<IFigure> ReadFigures(XElement figuresNode)
        {
            return ReadFigures(figuresNode, null);
        }

        public virtual IEnumerable<IFigure> ReadFigures(XElement figuresNode, Drawing drawing)
        {
            var figures = new Dictionary<string, IFigure>();
            return ReadFigures(figuresNode, drawing, figures);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="figuresNode"></param>
        /// <param name="drawing"></param>
        /// <param name="figures">The dictionary is on purpose - we pass an existing dictionary during macro creation</param>
        /// <returns></returns>
        public virtual IEnumerable<IFigure> ReadFigures(XElement figuresNode, Drawing drawing, Dictionary<string, IFigure> figures)
        {
            var result = new List<IFigure>();
            var nameBlacklist = new List<string>();
            var nodeMap = new Dictionary<string, XElement>();
            foreach (var figureNode in figuresNode.Elements())
            {
                var name = figureNode.ReadString("Name");
                // Temporary fix! This prevents an error when duplicate names occur.    - D.H.
                if (!nodeMap.ContainsKey(name))
                {
                    nodeMap.Add(name, figureNode);
                }
            }

            foreach (var figureName in nodeMap.Keys)
            {
                ReadFigure(figureName, figures, nameBlacklist, nodeMap, drawing, result.Add);
            }

            return result;
        }

        private static Dictionary<string, Type> mFigureTypes;
        public static Dictionary<string, Type> FigureTypes
        {
            get
            {
                if (mFigureTypes == null)
                {
                    mFigureTypes = new Dictionary<string, Type>();
                    foreach (var assembly in MEFHost.Instance.Assemblies)
                    {
                        foreach (var type in assembly.GetTypes()
                            .Where(t => typeof(IFigure).IsAssignableFrom(t)))
                        {
                            mFigureTypes.Add(type.Name, type);
                        }
                    }
                }

                return mFigureTypes;
            }
        }

        public static Type FindType(string typeName)
        {
            if (FigureTypes.ContainsKey(typeName))
            {
                return FigureTypes[typeName];
            }

            return null;
        }

        public virtual void ReadFigure(
            string figureName,
            Dictionary<string, IFigure> alreadyDeserializedFigures,
            List<string> nameBlacklist,
            Dictionary<string, XElement> nodeMap,
            Drawing drawing,
            Action<IFigure> callbackWhenCreated)
        {
            if (alreadyDeserializedFigures.ContainsKey(figureName))
            {
                return;
            }

            var figureNode = nodeMap[figureName];

            var type = FindType(figureNode.Name.LocalName);
            if (type == null)
            {
                ReportError(string.Format("Type {0} not found.", figureNode.Name.LocalName));
                return;
            }

            var dependencyNames = figureNode.Elements("Dependency").Select(e => e.ReadString("Name")).ToArray();
            foreach (var dependencyName in dependencyNames)
            {
                ReadFigure(dependencyName, alreadyDeserializedFigures, nameBlacklist, nodeMap, drawing, callbackWhenCreated);
            }

            var dependencies = new List<IFigure>();
            foreach (var dependencyName in dependencyNames)
            {
                IFigure existingDependency = null;
                if (!alreadyDeserializedFigures.TryGetValue(dependencyName, out existingDependency))
                {
                    throw new Exception(dependencyName);
                }

                dependencies.Add(existingDependency);
            }

            var instance = InstantiateFigure(type, drawing, dependencies);
            if (!GenerateNewNames)
            {
                instance.Name = figureName;
            }
            if (drawing.Figures[instance.Name] != null)
            {
                instance.GenerateNewNameIfNecessary(drawing, nameBlacklist);
            }

            nameBlacklist.Add(instance.Name);
            alreadyDeserializedFigures.Add(figureName, instance);

            try
            {
                instance.ReadXml(figureNode);
            }
            catch (Exception ex)
            {
                ReportError(ex.ToString());
                callbackWhenCreated(instance);
                return;
            }

            callbackWhenCreated(instance);
        }

        IFigure InstantiateFigure(Type type, Drawing drawing, IList<IFigure> dependencies)
        {
            IFigure instance = null;

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            if (defaultCtor != null)
            {
                instance = Activator.CreateInstance(type) as IFigure;
                instance.Drawing = drawing;
                instance.Dependencies = dependencies;
            }
            else
            {
                var ctorWithDrawingAndDependencies = type.GetConstructor(new Type[] { typeof(Drawing), typeof(IList<IFigure>) });
                if (ctorWithDrawingAndDependencies != null)
                {
                    instance = Activator.CreateInstance(type, drawing, dependencies) as IFigure;
                }
            }

            return instance;
        }

        public bool GenerateNewNames { get; set; }
    }
}
