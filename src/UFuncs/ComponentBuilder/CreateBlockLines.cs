using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            var lineColor = 7;
            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
            Point3d mappedEndPointX1 = CreateBlockLinesEndPointX1(lineColor, lineLength, wcsOrigin, mappedStartPoint1);
            Point3d mappedEndPointY1 = CreateBlockLinesEndPointY1(lineColor, lineWidth, wcsOrigin, mappedStartPoint1, "YBASE1");
            Point3d mappedEndPointZ1 = CreateBlockLinesEndPointZ1(lineColor, lineHeight, wcsOrigin, mappedStartPoint1, "ZBASE1");
            Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineColor, lineLength, mappedEndPointX1, mappedEndPointY1, "YBASE2");
            Point3d mappedEndPointX1Ceiling;
            CreateBlockLinesEndPointX1Ceiling(lineColor, lineLength, mappedEndPointZ1, out Point3d mappedStartPoint3, out mappedEndPointX1Ceiling);
            Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineColor, lineWidth, mappedEndPointZ1, mappedStartPoint3);
            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineColor, lineLength, mappedEndPointY1Ceiling, mappedStartPoint4);
            CreateBlockLinesYCeiling2(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");
            CreateBlockLinesZBase2(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
            CreateBlockLinesZBase3(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
            CreateBlockLinesZBase4(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");
        }

        private static void CreateBlockLines(int lineColor, Point3d start, Point3d end, string name)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(start, end);
            yCeiling1.SetName(name);
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private static void CreateBlockLinesZBase4(int lineColor, Point3d start, Point3d end, string name)
        {
            var zBase4 = _workPart.Curves.CreateLine(start, end);
            zBase4.SetName(name);
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);
        }

        private static void CreateBlockLinesZBase3(int lineColor, Point3d start, Point3d end, string name)
        {
            var zBase3 = _workPart.Curves.CreateLine(start, end);
            zBase3.SetName(name);
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);
        }

        private static void CreateBlockLinesZBase2(int lineColor, Point3d start, Point3d end, string name)
        {
            var zBase2 = _workPart.Curves.CreateLine(start, end);
            zBase2.SetName(name);
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);
        }

        private static void CreateBlockLinesYCeiling2(int lineColor, Point3d start, Point3d end, string name)
        {
            var yCeiling2 = _workPart.Curves.CreateLine(start, end);
            yCeiling2.SetName(name);
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);
        }




        private static void CreateBlockLineYCeiling2(int lineColor, Point3d start, Point3d end, string name)
        {
            var xCeiling2 = _workPart.Curves.CreateLine(start, end);
            xCeiling2.SetName(name);
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);
        }


        private static void CreateBlockLinesXBase2(int lineColor, Point3d start, Point3d end, string name)
        {
            var xbase2 = _workPart.Curves.CreateLine(start, end);
            xbase2.SetName(name);
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);
        }

        private static void CreateBlockLinesZBase1(int lineColor, Point3d start, Point3d end, string name)
        {
            var zBase1 = _workPart.Curves.CreateLine(start, end);
            zBase1.SetName(name);
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);
        }

        private static void CreateBlockLinesYBase1(int lineColor, Point3d start, Point3d end, string name)
        {
            var yBase1 = _workPart.Curves.CreateLine(start, end);
            yBase1.SetName(name);
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);
        }

        private static void CreateBlockLinesXBase1(int lineColor, Point3d start, Point3d end, string name)
        {
            var xBase1 = _workPart.Curves.CreateLine(start, end);
            xBase1.SetName(name);
            xBase1.Color = lineColor;
            xBase1.RedisplayObject();
            _edgeRepLines.Add(xBase1);
        }

        private Point3d CreateBlockLinesEndPointX2(int lineColor, double lineLength, Point3d mappedEndPointX1, Point3d mappedEndPointY1, string name)
        {
            Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineColor, lineLength, mappedEndPointY1);

            NewMethod76(lineColor, mappedEndPointX1, mappedEndPointX2, name);
            return mappedEndPointX2;
        }

        private static void NewMethod76(int lineColor, Point3d start, Point3d end, string name)
        {
            var yBase2 = _workPart.Curves.CreateLine(start, end);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
        }

        private void CreateBlockLinesEndPointX1Ceiling(int lineColor, double lineLength, Point3d mappedEndPointZ1, out Point3d mappedStartPoint3, out Point3d mappedEndPointX1Ceiling)
        {
            mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            var endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlovkLinesXCeiling2(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");
        }

        private static void CreateBlovkLinesXCeiling2(int lineColor, Point3d start, Point3d end, string name)
        {
            var xCeiling1 = _workPart.Curves.CreateLine(start, end);
            xCeiling1.SetName(name);
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);
        }


        private Point3d CreateBlockLinesEndPOontX2(int lineColor, double lineLength, Point3d mappedEndPointY1)
        {
            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLinesXBase2(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            return mappedEndPointX2;
        }

        private Point3d CreateBlockLinesEndPointZ1(int lineColor, double lineHeight, Point3d start, Point3d end, string name)
        {
            var endPointZ1 = end.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLinesZBase1(lineColor, start, mappedEndPointZ1, name);
            return mappedEndPointZ1;
        }
        private Point3d CreateBlockLinesEndPointY1(int lineColor, double lineWidth, Point3d start, Point3d end, string name)
        {
            var endPointY1 = new Point3d(end.X, end.Y + lineWidth, end.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLinesYBase1(lineColor, start, mappedEndPointY1, name);
            return mappedEndPointY1;
        }

        private Point3d CreateBlockLinesEndPointX2Ceiling(int lineColor, double lineLength, Point3d start, Point3d end)
        {
            var endPointX2Ceiling = end.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            CreateBlockLineYCeiling2(lineColor, start, mappedEndPointX2Ceiling, "XCEILING2");
            return mappedEndPointX2Ceiling;
        }
        private Point3d CreateBlockLineEndPointY1Ceiling(int lineColor, double lineWidth, Point3d start, Point3d end)
        {
            var endPointY1Ceiling = end.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLines(lineColor, start, mappedEndPointY1Ceiling, "YCEILING1");
            return mappedEndPointY1Ceiling;
        }

        private Point3d CreateBlockLinesEndPointX1(int lineColor, double lineLength, Point3d start, Point3d end)
        {
            var endPointX1 = new Point3d(end.X + lineLength, end.Y, end.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLinesXBase1(lineColor, start, mappedEndPointX1, "XBASE1");
            return mappedEndPointX1;
        }
    }
}
