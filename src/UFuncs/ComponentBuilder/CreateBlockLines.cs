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
            Point3d mappedEndPointX1 = CreateBlockLinesEndPointX1(lineColor, lineLength, wcsOrigin, mappedStartPoint1, "XBASE1");
            Point3d mappedEndPointY1 = CreateBlockLinesEndPointY1(lineColor, lineWidth, wcsOrigin, mappedStartPoint1, "YBASE1");
            Point3d mappedEndPointZ1 = CreateBlockLinesEndPointZ1(lineColor, lineHeight, wcsOrigin, mappedStartPoint1, "ZBASE1");
            Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineColor, lineLength, mappedEndPointX1, mappedEndPointY1, "YBASE2");
            Point3d mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            Point3d endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            Point3d mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");

            Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineColor, lineWidth, mappedEndPointZ1, mappedStartPoint3, "YCEILING1");
            Point3d mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineColor, lineLength, mappedEndPointY1Ceiling, mappedStartPoint4);
            CreateBlockLine(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");
            CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
            CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
            CreateBlockLine(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");
        }

        private static void CreateBlockLine(int lineColor, Point3d start, Point3d end, string name)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(start, end);
            yCeiling1.SetName(name);
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private Point3d CreateBlockLinesEndPOontX2(int lineColor, double lineLength, Point3d mappedEndPointY1)
        {
            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            return mappedEndPointX2;
        }

        private Point3d CreateBlockLinesEndPointZ1(int lineColor, double lineHeight, Point3d start, Point3d end, string name)
        {
            var endPointZ1 = end.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLine(lineColor, start, mappedEndPointZ1, name);
            return mappedEndPointZ1;
        }
        private Point3d CreateBlockLinesEndPointY1(int lineColor, double lineWidth, Point3d start, Point3d end, string name)
        {
            var endPointY1 = new Point3d(end.X, end.Y + lineWidth, end.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLine(lineColor, start, mappedEndPointY1, name);
            return mappedEndPointY1;
        }

        private Point3d CreateBlockLinesEndPointX2Ceiling(int lineColor, double lineLength, Point3d start, Point3d end)
        {
            var endPointX2Ceiling = end.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            CreateBlockLine(lineColor, start, mappedEndPointX2Ceiling, "XCEILING2");
            return mappedEndPointX2Ceiling;
        }
        private Point3d CreateBlockLineEndPointY1Ceiling(int lineColor, double lineWidth, Point3d start, Point3d end, string name)
        {
            var endPointY1Ceiling = end.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLine(lineColor, start, mappedEndPointY1Ceiling, name);
            return mappedEndPointY1Ceiling;
        }

        private Point3d CreateBlockLinesEndPointX1(int lineColor, double lineLength, Point3d start, Point3d end, string name)
        {
            var endPointX1 = end.__AddX(lineLength);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLine(lineColor, start, mappedEndPointX1, name);
            return mappedEndPointX1;
        }


        private Point3d CreateBlockLinesEndPointX2(int lineColor, double lineLength, Point3d mappedEndPointX1, Point3d mappedEndPointY1, string name)
        {
            Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineColor, lineLength, mappedEndPointY1);

            CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX2, name);
            return mappedEndPointX2;
        }

    }
}
