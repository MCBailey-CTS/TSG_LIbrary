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
        //private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        //{
        //    var lineColor = 7;
        //    var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
        //    Point3d mappedEndPointX1 = CreateBlockLinesEndPointX1(lineColor, lineLength, wcsOrigin, mappedStartPoint1, "XBASE1");
        //    Point3d mappedEndPointY1 = CreateBlockLinesEndPointY1(lineColor, lineWidth, wcsOrigin, mappedStartPoint1, "YBASE1");
        //    Point3d mappedEndPointZ1 = CreateBlockLinesEndPointZ1(lineColor, lineHeight, wcsOrigin, mappedStartPoint1, "ZBASE1");
        //    var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);
        //    Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineColor, lineLength, mappedEndPointX1, mappedEndPointY1, "XBASE2");
        //    Point3d mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
        //    Point3d endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
        //    Point3d mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
        //    CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");

        //    Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineColor, lineWidth, mappedEndPointZ1, mappedStartPoint3, "YCEILING1");
        //    Point3d mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
        //    Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineColor, lineLength, mappedEndPointY1Ceiling, mappedStartPoint4, "XCEILING2");
        //    CreateBlockLine(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");
        //    CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
        //    CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
        //    CreateBlockLine(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");
        //}

        //private static void CreateBlockLine(int lineColor, Point3d start, Point3d end, string name)
        //{
        //    var yCeiling1 = _workPart.Curves.CreateLine(start, end);
        //    yCeiling1.SetName(name);
        //    yCeiling1.Color = lineColor;
        //    yCeiling1.RedisplayObject();
        //    _edgeRepLines.Add(yCeiling1);
        //}

        //private Point3d CreateBlockLinesEndPOontX2(int lineColor, double lineLength, Point3d mappedEndPointY1, string name)
        //{
        //    var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);
        //    var endPointX2 = mappedStartPoint2.__AddX(lineLength);
        //    var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
        //    CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointX2, name);
        //    return mappedEndPointX2;
        //}

        //private Point3d CreateBlockLinesEndPointZ1(int lineColor, double lineHeight, Point3d start, Point3d end, string name)
        //{
        //    var endPointZ1 = end.__AddZ(lineHeight);
        //    var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
        //    CreateBlockLine(lineColor, start, mappedEndPointZ1, name);
        //    return mappedEndPointZ1;
        //}
        //private Point3d CreateBlockLinesEndPointY1(int lineColor, double lineWidth, Point3d start, Point3d end, string name)
        //{
        //    var endPointY1 = new Point3d(end.X, end.Y + lineWidth, end.Z);
        //    var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
        //    CreateBlockLine(lineColor, start, mappedEndPointY1, name);
        //    return mappedEndPointY1;
        //}

        //private Point3d CreateBlockLinesEndPointX2Ceiling(int lineColor, double lineLength, Point3d start, Point3d end, string name)
        //{
        //    var endPointX2Ceiling = end.__AddX(lineLength);
        //    var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
        //    CreateBlockLine(lineColor, start, mappedEndPointX2Ceiling, name);
        //    return mappedEndPointX2Ceiling;
        //}
        //private Point3d CreateBlockLineEndPointY1Ceiling(int lineColor, double lineWidth, Point3d start, Point3d end, string name)
        //{
        //    var endPointY1Ceiling = end.__AddY(lineWidth);
        //    var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
        //    CreateBlockLine(lineColor, start, mappedEndPointY1Ceiling, name);
        //    return mappedEndPointY1Ceiling;
        //}

        //private Point3d CreateBlockLinesEndPointX1(int lineColor, double lineLength, Point3d start, Point3d end, string name)
        //{
        //    var endPointX1 = end.__AddX(lineLength);
        //    var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
        //    CreateBlockLine(lineColor, start, mappedEndPointX1, name);
        //    return mappedEndPointX1;
        //}


        //private Point3d CreateBlockLinesEndPointX2(int lineColor, double lineLength, Point3d mappedEndPointX1, Point3d mappedEndPointY1, string name)
        //{
        //    //Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineColor, lineLength, mappedEndPointY1, name);

           
        //    var endPointX2 = mappedStartPoint2.__AddX(lineLength);
        //    var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
        //    CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX2, name);
        //    return mappedEndPointX2;
        //}



        //$$$$$$$$$$$$$$$$$$ redo

        private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            var lineColor = 7;

            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
            var endPointX1 = mappedStartPoint1.__AddX(lineLength);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            NewMethod83(wcsOrigin, lineColor, mappedEndPointX1);

            var endPointY1 = mappedStartPoint1.__AddY(lineWidth);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            NewMethod84(wcsOrigin, lineColor, mappedEndPointY1);

            var endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            NewMethod85(wcsOrigin, lineColor, mappedEndPointZ1);

            //==================================================================================================================

            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            NewMethod86(lineColor, mappedEndPointY1, mappedEndPointX2);
            NewMethod87(lineColor, mappedEndPointX1, mappedEndPointX2);

            //==================================================================================================================

            var mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);

            var endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            var mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            NewMethod82(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling);

            var endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            NewMethod81(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling);

            //==================================================================================================================

            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            var endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);

            NewMethod79(lineColor, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            NewMethod80(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);

            //==================================================================================================================

            NewMethod78(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling);
            NewMethod77(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling);
            NewMethod76(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling);

            //==================================================================================================================
        }

        private static void NewMethod87(int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointX2)
        {
            var yBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX2);
            yBase2.SetName("YBASE2");
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
        }

        private static void NewMethod86(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointX2)
        {
            var xbase2 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointX2);
            xbase2.SetName("XBASE2");
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);
        }

        private static void NewMethod85(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointZ1)
        {
            var zBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointZ1);
            zBase1.SetName("ZBASE1");
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);
        }

        private static void NewMethod84(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointY1)
        {
            var yBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointY1);
            yBase1.SetName("YBASE1");
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);
        }

        private static void NewMethod83(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointX1)
        {
            var xBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointX1);
            xBase1.SetName("XBASE1");
            xBase1.Color = lineColor;
            xBase1.RedisplayObject();
            _edgeRepLines.Add(xBase1);
        }

        private static void NewMethod82(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling)
        {
            var xCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointX1Ceiling);
            xCeiling1.SetName("XCEILING1");
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);
        }

        private static void NewMethod81(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointY1Ceiling)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointY1Ceiling);
            yCeiling1.SetName("YCEILING1");
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private static void NewMethod80(int lineColor, Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            var yCeiling2 = _workPart.Curves.CreateLine(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            yCeiling2.SetName("YCEILING2");
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);
        }

        private static void NewMethod79(int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            var xCeiling2 = _workPart.Curves.CreateLine(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            xCeiling2.SetName("XCEILING2");
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);
        }

        private static void NewMethod78(int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling)
        {
            var zBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX1Ceiling);
            zBase2.SetName(@"ZBASE2");
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);
        }

        private static void NewMethod77(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling)
        {
            var zBase3 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointY1Ceiling);
            zBase3.SetName("ZBASE3");
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);
        }

        private static void NewMethod76(int lineColor, Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling)
        {
            var zBase4 = _workPart.Curves.CreateLine(mappedEndPointX2, mappedEndPointX2Ceiling);
            zBase4.SetName("ZBASE4");
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);
        }
    }
}
