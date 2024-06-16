using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using System;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;
using NXOpen.Utilities;
using NXOpen.Preferences;
using TSG_Library.Properties;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {


        private void DeleteHandles()
        {
            using (session_.__UsingDoUpdate())
            {
                UserDefinedClass myUdOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo = __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                    session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
                }

                foreach (Point namedPt in __work_part_.Points)
                    if (namedPt.Name != "")
                        session_.UpdateManager.AddToDeleteList(namedPt);

                foreach (Line dLine in _edgeRepLines)
                    session_.UpdateManager.AddToDeleteList(dLine);
            }

            _edgeRepLines.Clear();
        }


        
     

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks,
            double[] pointOnFace, Point point1, string name)
        {
            myUdo.SetName(name);
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }


        private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            int lineColor = 7;

            Point3d mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
            Point3d endPointX1 = mappedStartPoint1.__AddX(lineLength);
            Point3d mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointX1, "XBASE1");

            Point3d endPointY1 = mappedStartPoint1.__AddY(lineWidth);
            Point3d mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointY1, "YBASE1");

            Point3d endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            Point3d mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointZ1, "ZBASE1");

            //==================================================================================================================

            Point3d mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            Point3d endPointX2 = mappedStartPoint2.__AddX(lineLength);
            Point3d mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX2, "YBASE2");

            //==================================================================================================================

            Point3d mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            Point3d endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            Point3d mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");

            Point3d endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            Point3d mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling, "YCEILING1");

            //==================================================================================================================

            Point3d mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            Point3d mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);

            CreateBlockLine(lineColor, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling, "XCEILING2");
            CreateBlockLine(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");

            //==================================================================================================================

            CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
            CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
            CreateBlockLine(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");

            //==================================================================================================================
        }

        private static void CreateBlockLine(int lineColor, Point3d start, Point3d end, string name)
        {
            Line yBase2 = __work_part_.Curves.CreateLine(start, end);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
        }


    }
}
// 2045