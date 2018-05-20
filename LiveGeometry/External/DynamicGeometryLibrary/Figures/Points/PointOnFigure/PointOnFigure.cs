using System.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicGeometry.Figures.Lists;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.Figures.Points.PointOnFigure
{
    public partial class PointOnFigure : FreePoint, IPoint
    {
        public override void ReadXml(System.Xml.Linq.XElement element)
        {
            base.ReadXml(element);
            Parameter = element.ReadDouble("Parameter");
            Recalculate();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeDouble("Parameter", Parameter);
        }

        protected override System.Windows.Shapes.Shape CreateShape()
        {
            var result = Factory.CreateDependentPointShape();
            result.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 255, 128));
            return result;
        }

        [PropertyGridVisible]
        public double Parameter { get; set; }

        public override double X => base.X;

        public override double Y => base.Y;

        public bool UseHitTestingForExistence = true;   // Broadens usefullness of PointOnFigure. Used in Tabula.

        public ILinearFigure LinearFigure => (ILinearFigure)Dependencies.First();

        public override void MoveToCore(Point newPosition)
        {
            var figure = LinearFigure;
            Parameter = figure.GetNearestParameterFromPoint(newPosition);
            newPosition = figure.GetPointFromParameter(Parameter);
            base.MoveToCore(newPosition);
        }

        public override void Recalculate()
        {
            if (!Dependencies.Exists())
            {
                Exists = false;
                return;
            }

            var figure1 = LinearFigure;
            var p = figure1.GetPointFromParameter(Parameter);
            var HitTestFailed = (UseHitTestingForExistence) ? LinearFigure.HitTest(p) == null : false;
            if (!p.Exists() || HitTestFailed)
            {
                Exists = false;
                return;
            }

            Exists = true;
            Coordinates = p;
        }

        public static bool CanBeOnFigure(IFigure figure)
        {
            return figure is ILinearFigure;
        }
    }
}

