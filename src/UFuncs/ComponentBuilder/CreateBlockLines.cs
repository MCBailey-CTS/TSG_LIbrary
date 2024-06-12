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
            Point3d mappedEndPointX1 = CreateBlockLinesEndPointX1(wcsOrigin, lineLength, mappedStartPoint1, lineColor);
            Point3d mappedEndPointY1 = CreateBlockLinesEndPointY1(wcsOrigin, lineWidth, mappedStartPoint1, lineColor);
            Point3d mappedEndPointZ1 = CreateBlockLinesEndPointZ1(wcsOrigin, lineHeight, mappedStartPoint1, lineColor);
            Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineLength, lineColor, mappedEndPointX1, mappedEndPointY1, "YBASE2");
            Point3d mappedEndPointX1Ceiling;
            CreateBlockLinesEndPointX1Ceiling(lineLength, lineColor, mappedEndPointZ1, out Point3d mappedStartPoint3, out mappedEndPointX1Ceiling);
            Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineWidth, lineColor, mappedEndPointZ1, mappedStartPoint3);
            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineLength, lineColor, mappedEndPointY1Ceiling, mappedStartPoint4);
            CreateBlockLinesYCeiling2(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");
            CreateBlockLinesZBase2(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
            CreateBlockLinesZBase3(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
            CreateBlockLinesZBase4(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");
        }

        private static void CreateBlockLines(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointY1Ceiling, string name)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointY1Ceiling);
            yCeiling1.SetName(name);
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private static void CreateBlockLinesZBase4(int lineColor, Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling, string name)
        {
            var zBase4 = _workPart.Curves.CreateLine(mappedEndPointX2, mappedEndPointX2Ceiling);
            zBase4.SetName(name);
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);
        }

        private static void CreateBlockLinesZBase3(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling, string name)
        {
            var zBase3 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointY1Ceiling);
            zBase3.SetName(name);
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);
        }

        private static void CreateBlockLinesZBase2(int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling, string name)
        {
            var zBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX1Ceiling);
            zBase2.SetName(name);
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);
        }

        private static void CreateBlockLinesYCeiling2(int lineColor, Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling, string name)
        {
            var yCeiling2 = _workPart.Curves.CreateLine(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            yCeiling2.SetName(name);
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);
        }




        private static void CreateBlockLineYCeiling2(int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling, string name)
        {
            var xCeiling2 = _workPart.Curves.CreateLine(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            xCeiling2.SetName(name);
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);
        }


        private static void CreateBlockLinesXBase2(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointX2, string name)
        {
            var xbase2 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointX2);
            xbase2.SetName(name);
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);
        }

        private static void CreateBlockLinesZBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointZ1, string name)
        {
            var zBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointZ1);
            zBase1.SetName(name);
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);
        }

        private static void CreateBlockLinesYBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointY1, string name)
        {
            var yBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointY1);
            yBase1.SetName(name);
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);
        }

        private static void CreateBlockLinesXBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointX1, string name)
        {
            var xBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointX1);
            xBase1.SetName(name);
            xBase1.Color = lineColor;
            xBase1.RedisplayObject();
            _edgeRepLines.Add(xBase1);
        }

        private Point3d CreateBlockLinesEndPointX2(double lineLength, int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointY1, string name)
        {
            Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineLength, lineColor, mappedEndPointY1);

            var yBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX2);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
            return mappedEndPointX2;
        }

        private void CreateBlockLinesEndPointX1Ceiling(double lineLength, int lineColor, Point3d mappedEndPointZ1, out Point3d mappedStartPoint3, out Point3d mappedEndPointX1Ceiling)
        {
            mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            var endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlovkLinesXCeiling2(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");
        }

        private static void CreateBlovkLinesXCeiling2(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling, string name)
        {
            var xCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointX1Ceiling);
            xCeiling1.SetName(name);
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);
        }


        private Point3d CreateBlockLinesEndPOontX2(double lineLength, int lineColor, Point3d mappedEndPointY1)
        {
            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLinesXBase2(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            return mappedEndPointX2;
        }

        private Point3d CreateBlockLinesEndPointZ1(Point3d wcsOrigin, double lineHeight, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLinesZBase1(wcsOrigin, lineColor, mappedEndPointZ1, "ZBASE1");
            return mappedEndPointZ1;
        }
        private Point3d CreateBlockLinesEndPointY1(Point3d wcsOrigin, double lineWidth, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointY1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y + lineWidth, mappedStartPoint1.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLinesYBase1(wcsOrigin, lineColor, mappedEndPointY1, "YBASE1");
            return mappedEndPointY1;
        }

        private Point3d CreateBlockLinesEndPointX2Ceiling(double lineLength, int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedStartPoint4)
        {
            var endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            CreateBlockLineYCeiling2(lineColor, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling, "XCEILING2");
            return mappedEndPointX2Ceiling;
        }
        private Point3d CreateBlockLineEndPointY1Ceiling(double lineWidth, int lineColor, Point3d mappedEndPointZ1, Point3d mappedStartPoint3)
        {
            var endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLines(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling, "YCEILING1");
            return mappedEndPointY1Ceiling;
        }

        private Point3d CreateBlockLinesEndPointX1(Point3d wcsOrigin, double lineLength, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointX1 = new Point3d(mappedStartPoint1.X + lineLength, mappedStartPoint1.Y, mappedStartPoint1.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLinesXBase1(wcsOrigin, lineColor, mappedEndPointX1, "XBASE1");
            return mappedEndPointX1;
        }
    }
}
