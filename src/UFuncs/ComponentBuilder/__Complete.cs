using NXOpen.UF;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using NXOpen.UserDefinedObjects;
using NXOpen.Features;
using NXOpen.GeometricUtilities;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void ShowTemporarySizeText()
        {
            _displayPart.Views.Refresh();

            foreach (var eLine in _edgeRepLines)
            {
                if (eLine.Name != "XBASE1" && eLine.Name != "YBASE1" && eLine.Name != "ZBASE1")
                    continue;

                var dim = _displayPart.PartUnits == BasePart.Units.Inches
                    ? $"{Math.Round(eLine.GetLength(), 3):0.000}"
                    : $"{Math.Round(eLine.GetLength(), 3) / 25.4:0.000}";

                var midPoint = new double[3];
                var dispProps = new UFObj.DispProps { color = 31 };
                midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;
                var mappedPoint = new double[3];
                ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, midPoint, UF_CSYS_ROOT_COORDS, mappedPoint);

                ufsession_.Disp.DisplayTemporaryText(
                    _displayPart.Views.WorkView.Tag,
                    UFDisp.ViewType.UseWorkView,
                    dim,
                    mappedPoint,
                    UFDisp.TextRef.Middlecenter,
                    ref dispProps,
                    _displayPart.PartUnits == BasePart.Units.Inches ? .125 : 3.175,
                    1);
            }
        }
        private static void CreatePointBlkOrigin()
        {
            var pointLocationOrigin = _displayPart.WCS.Origin;
            var point1Origin = _workPart.Points.CreatePoint(pointLocationOrigin);
            point1Origin.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1Origin.Blank();
            point1Origin.SetName("BLKORIGIN");
            point1Origin.Layer = _displayPart.Layers.WorkLayer;
            point1Origin.RedisplayObject();
        }


        private static void SetDispUnits(Part.Units dispUnits)
        {
            if (dispUnits == Part.Units.Millimeters)
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.GMmNDegC);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults.GMmNDegC);
            }
            else
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.LbmInLbfDegF);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                    .LbmInLbfDegF);
            }
        }


        private static Point CreatePoint(double[] pointOnFace, string name)
        {
            var pointLocation = pointOnFace.__ToPoint3d();
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName(name);
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }


        private void DeleteHandles()
        {
            using (session_.__UsingDoUpdate())
            {
                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                    session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
                }

                foreach (Point namedPt in _workPart.Points)
                    if (namedPt.Name != "")
                        session_.UpdateManager.AddToDeleteList(namedPt);

                foreach (var dLine in _edgeRepLines)
                    session_.UpdateManager.AddToDeleteList(dLine);
            }

            _edgeRepLines.Clear();
        }

        private void MoveObjects(NXObject[] objsToMove, double distance, string deltaXyz)
        {
            try
            {
                if (distance == 0)
                    return;

                _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                MoveObjectBuilder builder = _workPart.BaseFeatures.CreateMoveObjectBuilder(null);

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    builder.TransformMotion.DistanceAngle.OrientXpress.AxisOption = OrientXpressBuilder.Axis.Passive;
                    builder.TransformMotion.DistanceAngle.OrientXpress.PlaneOption = OrientXpressBuilder.Plane.Passive;
                    builder.TransformMotion.OrientXpress.AxisOption = OrientXpressBuilder.Axis.Passive;
                    builder.TransformMotion.OrientXpress.PlaneOption = OrientXpressBuilder.Plane.Passive;
                    builder.TransformMotion.Option = ModlMotion.Options.DeltaXyz;
                    builder.TransformMotion.DeltaEnum = ModlMotion.Delta.ReferenceWcsWorkPart;

                    if (deltaXyz == "X")
                    {
                        builder.TransformMotion.DeltaXc.RightHandSide = distance.ToString();
                        builder.TransformMotion.DeltaYc.RightHandSide = "0";
                        builder.TransformMotion.DeltaZc.RightHandSide = "0";
                    }

                    if (deltaXyz == "Y")
                    {
                        builder.TransformMotion.DeltaXc.RightHandSide = "0";
                        builder.TransformMotion.DeltaYc.RightHandSide = distance.ToString();
                        builder.TransformMotion.DeltaZc.RightHandSide = "0";
                    }

                    if (deltaXyz == "Z")
                    {
                        builder.TransformMotion.DeltaXc.RightHandSide = "0";
                        builder.TransformMotion.DeltaYc.RightHandSide = "0";
                        builder.TransformMotion.DeltaZc.RightHandSide = distance.ToString();
                    }

                    builder.ObjectToMoveObject.Add(objsToMove);
                    builder.Commit();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if (spacing == 0)
                return cursor;

            return _displayPart.PartUnits == BasePart.Units.Inches
                ? RoundEnglish(spacing, cursor)
                : RoundMetric(spacing, cursor);
        }

        private static double RoundEnglish(double spacing, double cursor)
        {
            var round = Math.Abs(cursor);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing; ii <= 1; ii += spacing)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round;
        }

        private static double RoundMetric(double spacing, double cursor)
        {
            var round = Math.Abs(cursor / 25.4);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round * 25.4;
        }

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1, string name)
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
            var lineColor = 7;

            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
            var endPointX1 = mappedStartPoint1.__AddX(lineLength);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointX1, "XBASE1");

            var endPointY1 = mappedStartPoint1.__AddY(lineWidth);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointY1, "YBASE1");

            var endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLine(lineColor, wcsOrigin, mappedEndPointZ1, "ZBASE1");

            //==================================================================================================================

            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLine(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            CreateBlockLine(lineColor, mappedEndPointX1, mappedEndPointX2, "YBASE2");

            //==================================================================================================================

            var mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            var endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            var mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");

            var endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLine(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling, "YCEILING1");

            //==================================================================================================================

            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            var endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);

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
            var yBase2 = _workPart.Curves.CreateLine(start, end);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
        }
  private static double MapAndConvert(double inputDist, double[] mappedBase, double[] mappedPoint, int index)
  {
      var distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

      if (mappedBase[index] < mappedPoint[index])
      {
          distance *= -1;
          distance += inputDist;
      }
      else
      {
          distance -= inputDist;
      }

      return distance;
  }


    }
}
