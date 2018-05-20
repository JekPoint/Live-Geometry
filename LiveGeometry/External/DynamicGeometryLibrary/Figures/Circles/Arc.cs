using System.Windows.Media;
using System.Windows.Shapes;
using DynamicGeometry.Figures.Shapes;
using DynamicGeometry.PropertyGrid.Attributes;

namespace DynamicGeometry.Figures.Circles
{

    public partial class EllipseArc : EllipseArcBase
    {

        public override int BeginPointIndex => 3;

        public override int EndPointIndex => 4;

        public static void Convert(IArc oldArc, IArc newArc)
        {
            var drawing = oldArc.Drawing;
            newArc.Style = oldArc.Style;
            newArc.Clockwise = oldArc.Clockwise;
            Actions.Actions.ReplaceWithNew(oldArc, newArc);
            drawing.RaiseUserIsAddingFigures(new Drawing.UIAFEventArgs() { Figures = newArc.AsEnumerable<IFigure>() });
        }

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Segment")]
        public virtual void ConvertToEllipseSegment()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseSegment(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Sector")]
        public void ConvertToEllipseSector()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseSector(this.Drawing, this.Dependencies));
        }

#endif

    }

    public partial class CircleArc : CircleArcBase
    {

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Segment")]
        public virtual void ConvertToCircleSegment()
        {
            EllipseArc.Convert(this, Factory.CreateCircleSegment(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Sector")]
        public void ConvertToSector()
        {
            EllipseArc.Convert(this, Factory.CreateCircleSector(this.Drawing, this.Dependencies));
        }

#endif

    }

    // Below are simple implementations of a circle and ellipse segments.  
    // The chord is represented visually but is not a functioning figure in the drawing.
    // A segment could be added to this figure to provide a functioning chord. (SquareCreator is a model to follow.)
    // Implementing this as a composite figure is probably not a good idea. Intersections, pointOnFigure, etc would be ambiguous.
    // Unlike an arc, a circle or ellipse segment has a defined area.
    public partial class CircleSegment : CircleArcBase, IShapeWithInterior
    {
        
        protected override Path CreateShape()
        {
            var result = base.CreateShape();
            Figure.IsClosed = true;
            return result;
        }

        public double Area => Radius.Sqr() * Angle / Math.PI;

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Arc")]
        public void ConvertToArc()
        {
            EllipseArc.Convert(this, Factory.CreateArc(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Sector")]
        public void ConvertToSector()
        {
            EllipseArc.Convert(this, Factory.CreateCircleSector(this.Drawing, this.Dependencies));
        }

#endif

    }

    public partial class EllipseSegment : EllipseArcBase, IShapeWithInterior
    {
        protected override Path CreateShape()
        {
            var result = base.CreateShape();
            Figure.IsClosed = true;
            return result;
        }

        public override int BeginPointIndex => 3;

        public override int EndPointIndex => 4;

        public double Area => double.NaN;

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Arc")]
        public void ConvertToEllipseArc()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseArc(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Sector")]
        public void ConvertToEllipseSector()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseSector(this.Drawing, this.Dependencies));
        }

#endif

    }

    public partial class CircleSector : CircleArcBase, IShapeWithInterior
    {
        private PathFigure PolygonPart;
        private LineSegment Side1;
        private LineSegment Side2;
        protected override Path CreateShape()
        {
            var result = base.CreateShape();
            Side1 = new LineSegment();
            Side2 = new LineSegment();
            PolygonPart = new PathFigure()
            {
                IsClosed = false,
                IsFilled = true,
                Segments = new PathSegmentCollection()
                {
                    Side1,
                    Side2
                }
            };
            (result.Data as PathGeometry).Figures.Add(PolygonPart);
            result.StrokeEndLineCap = PenLineCap.Round;
            result.StrokeStartLineCap = PenLineCap.Round;
            return result;
        }

        public override void UpdateVisual()
        {
            base.UpdateVisual();
            PolygonPart.StartPoint = ToPhysical(BeginLocation);
            Side1.Point = ToPhysical(Center);
            Side2.Point = ToPhysical(EndLocation);
        }

        public double Area
        {
            get
            {
                var segmentArea = Radius.Sqr() * Angle / Math.PI;
                var polygonArea = Math.Area(BeginLocation, Center, EndLocation);
                return segmentArea + polygonArea;
            }
        }

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Arc")]
        public void ConvertToArc()
        {
            EllipseArc.Convert(this, Factory.CreateArc(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Segment")]
        public virtual void ConvertToCircleSegment()
        {
            EllipseArc.Convert(this, Factory.CreateCircleSegment(this.Drawing, this.Dependencies));
        }

#endif

    }

    public partial class EllipseSector : EllipseArcBase, IShapeWithInterior
    {
        private PathFigure PolygonPart;
        private LineSegment Side1;
        private LineSegment Side2;
        protected override Path CreateShape()
        {
            var result = base.CreateShape();
            Side1 = new LineSegment();
            Side2 = new LineSegment();
            PolygonPart = new PathFigure()
            {
                IsClosed = false,
                IsFilled = true,
                Segments = new PathSegmentCollection()
                {
                    Side1,
                    Side2
                }
            };
            (result.Data as PathGeometry).Figures.Add(PolygonPart);
            result.StrokeEndLineCap = PenLineCap.Round;
            result.StrokeStartLineCap = PenLineCap.Round;
            return result;
        }

        
        public override void UpdateVisual()
        {
            base.UpdateVisual();
            PolygonPart.StartPoint = ToPhysical(BeginLocation);
            Side1.Point = ToPhysical(Center);
            Side2.Point = ToPhysical(EndLocation);
        }

        public override int BeginPointIndex => 3;

        public override int EndPointIndex => 4;

        public double Area => double.NaN;

#if !PLAYER && !TABULA

        [PropertyGridVisible]
        [PropertyGridName("Convert To Arc")]
        public void ConvertToEllipseArc()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseArc(this.Drawing, this.Dependencies));
        }

        [PropertyGridVisible]
        [PropertyGridName("Convert To Segment")]
        public virtual void ConvertToEllipseSegment()
        {
            EllipseArc.Convert(this, Factory.CreateEllipseSegment(this.Drawing, this.Dependencies));
        }

#endif

    }

}
