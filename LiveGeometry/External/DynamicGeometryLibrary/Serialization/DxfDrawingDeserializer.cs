using System.Collections.Generic;
using System.Windows.Controls;
using DynamicGeometry.Figures;
using DynamicGeometry.Figures.Lines;
using DynamicGeometry.Figures.Points;
using netDxf;
using netDxf.Entities;

namespace DynamicGeometry.Serialization
{
    public class DxfDrawingDeserializer
    {
        private Drawing _drawing;
        private DxfDocument _doc;

        public Drawing ReadDrawing(string dxfFileName, Canvas canvas)
        {
            _doc = DxfDocument.Load(dxfFileName);

            _drawing = new Drawing(canvas);

            ReadLines();
            ReadPolylines();
            ReadArcs();
            ReadCircles();
            ReadInserts();

            _drawing.Recalculate();
            return _drawing;
        }

        private FreePoint CreateHiddenPoint(double x, double y)
        {
            return new FreePoint
            {
                Drawing = _drawing,
                X = x,
                Y = y,
                Visible = false
            };
        }

        private Segment CreateSegment(IPoint p1, IPoint p2)
        {
            return Factory.CreateSegment(_drawing, new[] { p1, p2 });
        }

        private void ReadLines()
        {
            foreach (var line in _doc.Lines)
            {
                ReadLine(line, 0, 0);
            }
        }

        private void ReadLine(Line line, double x, double y)
        {
            var point1 = CreateHiddenPoint(line.StartPoint.X + x, line.StartPoint.Y + y);
            var point2 = CreateHiddenPoint(line.EndPoint.X + x, line.EndPoint.Y + y);
            var segment = CreateSegment(point1, point2);
            Actions.Actions.Add(_drawing, segment);
        }

        private void ReadPolylines()
        {
            foreach (var item in _doc.Polylines)
            {
                if (item is Polyline polyline)
                {
                    ReadPolyline(polyline.Vertexes, polyline.IsClosed, 0, 0);
                }
            }
        }

        private void ReadPolyline(IEnumerable<PolylineVertex> vertices, bool isClosed, double x, double y)
        {
            IPoint firstPoint = null;
            IPoint previousPoint = null;
            var figures = new List<IFigure>();
            var segments = new List<IFigure>();

            foreach (var vertex in vertices)
            {
                var point = CreateHiddenPoint(vertex.Location.X + x, vertex.Location.Y + y);
                if (firstPoint == null)
                {
                    firstPoint = point;
                }
                if (previousPoint != null)
                {
                    var segment = CreateSegment(previousPoint, point);
                    figures.Add(segment);
                    segments.Add(segment);
                }
                previousPoint = point;
                figures.Add(point);
            }
            if (previousPoint != null && isClosed)
            {
                var segment = CreateSegment(previousPoint, firstPoint);
                figures.Add(segment);
                segments.Add(segment);

                var polygon = Factory.CreatePolygon(_drawing, figures);
                Actions.Actions.Add(_drawing, polygon);
            }

            Actions.Actions.AddMany(_drawing, segments.ToArray());
        }

        private void ReadArcs()
        {
            foreach (var item in _doc.Arcs)
            {
                ReadArc(item, 0, 0);
            }
        }

        private static void ReadArc(Arc arc, double x, double y)
        {
            // TODO :  
        }

        private void ReadCircles()
        {
            foreach (var item in _doc.Circles)
            {
                ReadCircle(item, 0, 0);
            }
        }

        private void ReadCircle(Circle circle, double x, double y)
        {
            var figures = new List<IFigure>
            {
                CreateHiddenPoint(circle.Center.X + x, circle.Center.Y + y),
                CreateHiddenPoint(circle.Center.X + x + circle.Radius, circle.Center.Y + y)
            };

            var figure = Factory.CreateCircleByRadius(_drawing, figures);
        }

        private void ReadInserts()
        {
            foreach (var item in _doc.Inserts)
            {
                ReadInsert(item);
            }
        }

        private void ReadInsert(Insert insert)
        {
            var entities = insert.Block.Entities;

            for (var index = 1; index < entities.Count; index++)
            {
                var entity = entities[index];

                if (entity is Line line)
                    ReadLine(line, insert.Position.X, insert.Position.Y);
                else if (entity is Arc arc)
                    ReadArc(arc, insert.Position.X, insert.Position.Y);
                else if (entity is Circle circle)
                    ReadCircle(circle, insert.Position.X, insert.Position.Y);
                else if (entity is Polyline polyline1)
                {
                    ReadPolyline(polyline1.Vertexes, polyline1.IsClosed, 0, 0);
                }
            }
        }

    }
}
