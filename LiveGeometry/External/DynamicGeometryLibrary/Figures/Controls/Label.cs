using System;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;
using DynamicGeometry.Figures.Circles;

namespace DynamicGeometry.Figures.Controls
{
    public class Label : LabelBase, IAngleProvider, ILengthProvider
    {
        public Label()
        {
            ShouldProcessText = true;
        }

        public double Value
        {
            get
            {
                double result = 0;
                double.TryParse(
                        ProcessedText,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out result);
                return result;
            }
        }

        public double Angle => Value;

        public double Length => Value;

        public override void ReadXml(XElement element)
        {
            base.ReadXml(element);
            text = element.ReadString("Text").Replace(@"\n", Environment.NewLine);
            var x = element.ReadDouble("X");
            var y = element.ReadDouble("Y");
            MoveTo(new Point(x, y));
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            var coordinates = Coordinates;
            writer.WriteAttributeString("Text", Text.Replace("\n", "").Replace("\r", @"\n"));
            writer.WriteAttributeString("X", coordinates.X.ToStringInvariant());
            writer.WriteAttributeString("Y", coordinates.Y.ToStringInvariant());
        }
    }
}
