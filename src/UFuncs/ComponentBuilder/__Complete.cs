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


        private bool NewMethod11()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }



        private bool NewMethod9()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }



        private bool NewMethod8()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }





        private bool NewMethod5()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }

        private bool NewMethod3()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }



        private bool NewMethod1()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }




        private static void SelectWithFilter_(string message)
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter(message);
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private bool NewMethod13()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            ufsession_.Ui.AskInfoUnits(out int infoUnits);
            Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }


        private void ShowTemporarySizeText()
        {
            _displayPart.Views.Refresh();

            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name != "XBASE1" && eLine.Name != "YBASE1" && eLine.Name != "ZBASE1")
                    continue;

                string dim = _displayPart.PartUnits == BasePart.Units.Inches
                    ? $"{Math.Round(eLine.GetLength(), 3):0.000}"
                    : $"{Math.Round(eLine.GetLength(), 3) / 25.4:0.000}";

                double[] midPoint = new double[3];
                UFObj.DispProps dispProps = new UFObj.DispProps { color = 31 };
                midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;
                double[] mappedPoint = new double[3];
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
            Point3d pointLocationOrigin = _displayPart.WCS.Origin;
            Point point1Origin = _workPart.Points.CreatePoint(pointLocationOrigin);
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
            Point3d pointLocation = pointOnFace.__ToPoint3d();
            Point point1 = _workPart.Points.CreatePoint(pointLocation);
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
                UserDefinedClass myUdOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                    session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
                }

                foreach (Point namedPt in _workPart.Points)
                    if (namedPt.Name != "")
                        session_.UpdateManager.AddToDeleteList(namedPt);

                foreach (Line dLine in _edgeRepLines)
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
            double round = Math.Abs(cursor);
            double roundValue = Math.Round(round, 3);
            double truncateValue = Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (double ii = spacing; ii <= 1; ii += spacing)
                    if (fractionValue <= ii)
                    {
                        double finalValue = truncateValue + ii;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round;
        }

        private static double RoundMetric(double spacing, double cursor)
        {
            double round = Math.Abs(cursor / 25.4);
            double roundValue = Math.Round(round, 3);
            double truncateValue = Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (double ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                    if (fractionValue <= ii)
                    {
                        double finalValue = truncateValue + ii;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round * 25.4;
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
            Line yBase2 = _workPart.Curves.CreateLine(start, end);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
        }

        private static double MapAndConvert(double inputDist, double[] mappedBase, double[] mappedPoint, int index)
        {
            double distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

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


        private void ZEndPoint(double distance, Line zAxisLine)
        {
            Point3d mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            Point3d addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            Point3d mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }


        private void XStartPoint(double distance, Line xAxisLine)
        {
            Point3d mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            Point3d addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            Point3d mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void YStartPoint(double distance, Line yAxisLine)
        {
            Point3d mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            Point3d addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            Point3d mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }


        private void ZStartPoint(double distance, Line zAxisLine)
        {
            Point3d mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            Point3d addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            Point3d mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }


        private void YEndPoint(double distance, Line yAxisLine)
        {
            Point3d mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            Point3d addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + distance,
                mappedEndPoint.Z);
            Point3d mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void XEndPoint(double distance, Line xAxisLine)
        {
            Point3d mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            Point3d addX = new Point3d(mappedEndPoint.X + distance, mappedEndPoint.Y,
                mappedEndPoint.Z);
            Point3d mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }


        private void MoveObjects(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double xDistance,
            string dir_xyz, bool showTemporary)
        {
            if (!(dir_xyz == "X" || dir_xyz == "Y" || dir_xyz == "Z"))
                throw new ArgumentException($"Invalid direction '{dir_xyz}'");

            MoveObjects(movePtsFull.ToArray(), xDistance, dir_xyz);
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, dir_xyz);

            if (showTemporary)
                ShowTemporarySizeText();
        }


        private static void MotionCallbackDynamic1(
            Point pointPrototype,
            List<NXObject> doNotMovePts,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            bool isPos)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("BLKORIGIN"))
                    {
                        if (isPos)
                            doNotMovePts.Add(namedPt);
                        else
                            movePtsFull.Add(namedPt);
                        continue;
                    }

                    movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }




        private static void NewMethod19(List<Line> posXObjs, List<Line> negXObjs, List<Line> posYObjs, List<Line> negYObjs, List<Line> posZObjs, List<Line> negZObjs)
        {
            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                   eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                   eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "YCEILING1" ||
                   eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }



        private static void NewMethod15(List<Line> posXObjs, List<Line> negXObjs, List<Line> posYObjs, List<Line> negYObjs, List<Line> posZObjs, List<Line> negZObjs)
        {
            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" || eLine.Name == "ZBASE1" ||
                    eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" || eLine.Name == "ZBASE2" ||
                    eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                    eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                    eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                    eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                    eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }

        private static EditSizeForm NewMethod14(double blockLength, double blockWidth, double blockHeight, Point pointPrototype, EditSizeForm sizeForm, double convertLength, double convertWidth, double convertHeight)
        {
            if (_displayPart.PartUnits == BasePart.Units.Inches)
            {
                if (pointPrototype.Name.Contains("X"))
                {
                    sizeForm = new EditSizeForm(blockLength);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Y"))
                {
                    sizeForm = new EditSizeForm(blockWidth);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Z"))
                {
                    sizeForm = new EditSizeForm(blockHeight);
                    sizeForm.ShowDialog();
                }
            }
            else
            {
                if (pointPrototype.Name.Contains("X"))
                {
                    sizeForm = new EditSizeForm(convertLength);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Y"))
                {
                    sizeForm = new EditSizeForm(convertWidth);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Z"))
                {
                    sizeForm = new EditSizeForm(convertHeight);
                    sizeForm.ShowDialog();
                }
            }

            return sizeForm;
        }


        private static void NewMethod18(List<Line> posXObjs, List<Line> negXObjs, List<Line> posYObjs, List<Line> negYObjs, List<Line> posZObjs, List<Line> negZObjs)
        {
            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                   eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                   eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "YCEILING1" ||
                   eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }

        private void MotionCallbackNotDynamic(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor, int index, string dir_xyz)
        {
            if (mappedPoint[index] == mappedCursor[index])
                return;

            double xDistance = Math.Sqrt(Math.Pow(mappedPoint[index] - mappedCursor[index], 2));

            if (xDistance < _gridSpace)
                return;

            if (mappedCursor[index] < mappedPoint[index])
                xDistance *= -1;

            _distanceMoved += xDistance;

            MoveObjects(moveAll.ToArray(), xDistance, dir_xyz);

            if (dir_xyz != "Z")
                return;

            ModelingView mView1 = (ModelingView)_displayPart.Views.WorkView;
            _displayPart.Views.WorkView.Orient(mView1.Matrix);
            _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
        }



        private static void NewMethod16(List<Line> posXObjs, List<Line> negXObjs, List<Line> posYObjs, List<Line> negYObjs, List<Line> posZObjs, List<Line> negZObjs)
        {
            foreach (Line eLine in _edgeRepLines)
            {
                switch (eLine.Name)
                {
                    case "YBASE1":
                    case "YCEILING1":
                    case "ZBASE1":
                    case "ZBASE3":
                        negXObjs.Add(eLine);
                        break;
                }

                switch (eLine.Name)
                {
                    case "YBASE2":
                    case "YCEILING2":
                    case "ZBASE2":
                    case "ZBASE4":
                        posXObjs.Add(eLine);
                        break;
                }

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                    eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                    eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                    eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" || eLine.Name == "YCEILING1" ||
                    eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }

        private void MotionCallback(double[] position, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            try
            {
                if (_isDynamic)
                    MotionCallbackDyanmic(position);
                else
                    MotionCalbackNotDynamic(position);

                double editBlkLength = 0;
                double editBlkWidth = 0;
                double editBlkHeight = 0;

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name == "XBASE1")
                        editBlkLength = eLine.GetLength();

                    if (eLine.Name == "YBASE1")
                        editBlkWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1")
                        editBlkHeight = eLine.GetLength();
                }

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    ufsession_.Ui.SetPrompt(
                        $"X = {editBlkLength:0.000}  " +
                        $"Y = {editBlkWidth:0.000}  " +
                        $"Z = {$"{editBlkHeight:0.000}"}  " +
                        $"Distance Moved =  {$"{_distanceMoved:0.000}"}");
                    return;
                }

                editBlkLength /= 25.4;
                editBlkWidth /= 25.4;
                editBlkHeight /= 25.4;

                double convertDistMoved = _distanceMoved / 25.4;

                ufsession_.Ui.SetPrompt(
                    $"X = {editBlkLength:0.000}  " +
                    $"Y = {editBlkWidth:0.000}  " +
                    $"Z = {editBlkHeight:0.000}  " +
                    $"Distance Moved =  {convertDistMoved:0.000}");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }




        private void MotionCallbackDyanmic(double[] position)
        {
            Point pointPrototype = _udoPointHandle.IsOccurrence
                ? (Point)_udoPointHandle.Prototype
                : _udoPointHandle;

            List<NXObject> doNotMovePts = new List<NXObject>();
            List<NXObject> movePtsHalf = new List<NXObject>();
            List<NXObject> movePtsFull = new List<NXObject>();

            MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull,
                pointPrototype.Name.Contains("POS"));

            List<Line> posXObjs = new List<Line>();
            List<Line> negXObjs = new List<Line>();
            List<Line> posYObjs = new List<Line>();
            List<Line> negYObjs = new List<Line>();
            List<Line> posZObjs = new List<Line>();
            List<Line> negZObjs = new List<Line>();

            NewMethod16(posXObjs, negXObjs, posYObjs, negYObjs, posZObjs, negZObjs);

            List<Line> allxAxisLines = new List<Line>();
            List<Line> allyAxisLines = new List<Line>();
            List<Line> allzAxisLines = new List<Line>();

            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name.StartsWith("X"))
                    allxAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Y"))
                    allyAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Z"))
                    allzAxisLines.Add(eLine);
            }

            // get the distance from the selected point to the cursor location

            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, posXObjs, negXObjs, allxAxisLines,
                    mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, posYObjs, negYObjs, allyAxisLines,
                    mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, posZObjs, negZObjs, allzAxisLines,
                    mappedPoint, mappedCursor);
        }



        private void MotionCallbackXDynamic(
            Point pointPrototype,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> posXObjs,
            List<Line> negXObjs,
            List<Line> allxAxisLines,
            double[] mappedPoint,
            double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0])
                return;

            double xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace)
                return;

            if (mappedCursor[0] < mappedPoint[0])
                xDistance *= -1;

            _distanceMoved += xDistance;

            if (pointPrototype.Name == "POSX")
            {
                movePtsFull.AddRange(posXObjs);
                EditAlign(movePtsHalf, movePtsFull, allxAxisLines, xDistance, "X", true);
            }
            else
            {
                movePtsFull.AddRange(negXObjs);
                EditAlign(movePtsHalf, movePtsFull, allxAxisLines, xDistance, "X", false);
            }
        }

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> negZObjs, List<Line> allzAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] != mappedCursor[2])
            {
                double zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                if (zDistance >= _gridSpace)
                {
                    if (mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                    _distanceMoved += zDistance;

                    if (pointPrototype.Name == "POSZ")
                    {
                        movePtsFull.AddRange(posZObjs);
                        EditAlign(movePtsHalf, movePtsFull, allzAxisLines, zDistance, "Z", true);
                    }
                    else
                    {
                        movePtsFull.AddRange(negZObjs);
                        EditAlign(movePtsHalf, movePtsFull, allzAxisLines, zDistance, "Z", false);
                    }

                    ModelingView mView1 = (ModelingView)_displayPart.Views.WorkView;
                    _displayPart.Views.WorkView.Orient(mView1.Matrix);
                    _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                }
            }
        }

        private void MotionCallbackYDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> negYObjs, List<Line> allyAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                double yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    if (pointPrototype.Name == "POSY")
                    {
                        movePtsFull.AddRange(posYObjs);
                        EditAlign(movePtsHalf, movePtsFull, allyAxisLines, yDistance, "Y", true);
                    }
                    else
                    {
                        movePtsFull.AddRange(negYObjs);
                        EditAlign(movePtsHalf, movePtsFull, allyAxisLines, yDistance, "Y", false);
                    }
                }
            }
        }

        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            List<NXObject> moveAll = new List<NXObject>();

            foreach (Point namedPts in _workPart.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            moveAll.AddRange(_edgeRepLines);
            // get the distance from the selected point to the cursor location
            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 0, "X");

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 1, "Y");

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 2, "Z");
        }




        private static void NewMethod2()
        {
            if (_isNewSelection)
                if (_updateComponent == null)
                    SelectWithFilter_("Select Component to Move");
        }



        private void SetWcsToWorkPart(Component compRefCsys)
        {
            try
            {
                if (compRefCsys is null)
                    SetWcsDisplayPart();
                else
                    SetWcsWorkComponent(compRefCsys);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

        private static void SetWcsDisplayPart()
        {
            _isUprParallel = false;
            _isLwrParallel = false;
            _parallelHeightExp = string.Empty;

            foreach (Expression exp in _workPart.Expressions)
            {
                if (exp.Name == "uprParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isUprParallel = true;
                    else
                        _isUprParallel = false;
                }

                if (exp.Name == "lwrParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isLwrParallel = true;
                    else
                        _isLwrParallel = false;
                }
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                    {
                        Block block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                        string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        double mLength = blockFeatureBuilderMatch.Length.Value;
                        double mWidth = blockFeatureBuilderMatch.Width.Value;
                        double mHeight = blockFeatureBuilderMatch.Height.Value;

                        if (_isUprParallel)
                        {
                            _parallelHeightExp = "uprParallelHeight";
                            _parallelWidthExp = "uprParallelWidth";
                        }

                        if (_isLwrParallel)
                        {
                            _parallelHeightExp = "lwrParallelHeight";
                            _parallelWidthExp = "lwrParallelWidth";
                        }

                        blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                        CartesianCoordinateSystem setTempCsys =
                            (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);
                        _workCompOrigin = setTempCsys.Origin;
                        _workCompOrientation = setTempCsys.Orientation.Element;
                    }
        }

        private void SetWcsWorkComponent(Component compRefCsys)
        {
            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

            BasePart compBase = (BasePart)compRefCsys.Prototype;

            __display_part_ = (Part)compBase;
            UpdateSessionParts();

            _isUprParallel = false;
            _isLwrParallel = false;
            _parallelHeightExp = string.Empty;
            _parallelWidthExp = string.Empty;

            foreach (Expression exp in _workPart.Expressions)
            {
                if (exp.Name == "uprParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isUprParallel = true;
                    else
                        _isUprParallel = false;
                }

                if (exp.Name == "lwrParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isLwrParallel = true;
                    else
                        _isLwrParallel = false;
                }
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                    {
                        Block block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                        string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        double mLength = blockFeatureBuilderMatch.Length.Value;
                        double mWidth = blockFeatureBuilderMatch.Width.Value;
                        double mHeight = blockFeatureBuilderMatch.Height.Value;

                        if (_isUprParallel)
                        {
                            _parallelHeightExp = "uprParallelHeight";
                            _parallelWidthExp = "uprParallelWidth";
                        }

                        if (_isLwrParallel)
                        {
                            _parallelHeightExp = "lwrParallelHeight";
                            _parallelWidthExp = "lwrParallelWidth";
                        }

                        blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                        CartesianCoordinateSystem setTempCsys =
                            (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);

                        CartesianCoordinateSystem featBlkCsys = _displayPart.WCS.Save();
                        featBlkCsys.SetName("EDITCSYS");
                        featBlkCsys.Layer = 254;

                        NXObject[] addToBody = { featBlkCsys };

                        foreach (ReferenceSet bRefSet in _displayPart.GetAllReferenceSets())
                            if (bRefSet.Name == "BODY")
                                bRefSet.AddObjectsToReferenceSet(addToBody);

                        __display_part_ = _originalDisplayPart;
                        __work_component_ = compRefCsys;
                        UpdateSessionParts();

                        foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                            if (wpCsys.Layer == 254)
                                if (wpCsys.Name == "EDITCSYS")
                                {
                                    NXObject csysOccurrence;
                                    csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                    CartesianCoordinateSystem editCsys = (CartesianCoordinateSystem)csysOccurrence;

                                    if (editCsys != null)
                                    {
                                        _displayPart.WCS.SetOriginAndMatrix(editCsys.Origin,
                                            editCsys.Orientation.Element);
                                        _workCompOrigin = editCsys.Origin;
                                        _workCompOrientation = editCsys.Orientation.Element;
                                    }

                                    Session.UndoMarkId markDeleteObjs;
                                    markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
                                    session_.UpdateManager.AddToDeleteList(wpCsys);

                                    int errsDelObjs;
                                    errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

                                    session_.DeleteUndoMark(markDeleteObjs, "");
                                }
                    }

            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }



        private void UpdateSessionParts()
        {
            _workPart = session_.Parts.Work;
            _displayPart = session_.Parts.Display;
        }

        private void UpdateOriginalParts()
        {
            _originalWorkPart = _workPart;
            _originalDisplayPart = _displayPart;
        }

        private void ResetNonBlockError()
        {
            EnableForm();
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            UpdateSessionParts();
            UpdateOriginalParts();
            __work_part_ = _displayPart;
        }

        private void DisableForm()
        {
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = false;
            buttonEditDynamic.Enabled = false;
            buttonEditMove.Enabled = false;
            buttonEditMatch.Enabled = false;
            buttonEditSize.Enabled = false;
            buttonEditAlign.Enabled = false;
            buttonAlignComponent.Enabled = false;
            buttonAlignEdgeDistance.Enabled = false;
            buttonApply.Enabled = false;
            buttonReset.Enabled = false;
            buttonExit.Enabled = false;
        }

        private void EnableForm()
        {
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = false;
            buttonEditDynamic.Enabled = true;
            buttonEditMove.Enabled = true;
            buttonEditMatch.Enabled = true;
            buttonEditSize.Enabled = true;
            buttonEditAlign.Enabled = true;
            buttonAlignComponent.Enabled = true;
            buttonAlignEdgeDistance.Enabled = true;
            buttonApply.Enabled = true;
            buttonReset.Enabled = false;
            buttonExit.Enabled = false;
        }
        private void CreateEditData(Component setCompTranslucency)
        {
            try
            {
                // set component translucency

                if (setCompTranslucency != null)
                    setCompTranslucency.__Translucency(75);
                else
                    foreach (Body dispBody in _workPart.Bodies)
                        if (dispBody.Layer == 1)
                            dispBody.__Translucency(75);

                SetWcsToWorkPart(setCompTranslucency);

                if (!_workPart.__HasDynamicBlock())
                    return;

                // get current block feature
                Block block1 = _workPart.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatch;
                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                double mLength = blockFeatureBuilderMatch.Length.Value;
                double mWidth = blockFeatureBuilderMatch.Width.Value;
                double mHeight = blockFeatureBuilderMatch.Height.Value;

                __work_part_ = _displayPart;
                UpdateSessionParts();

                if (mLength == 0 || mWidth == 0 || mHeight == 0)
                    return;


                // create edit block feature
                Feature nullFeaturesFeature = null;
                BlockFeatureBuilder blockFeatureBuilderEdit;
                blockFeatureBuilderEdit =
                    _displayPart.Features.CreateBlockFeatureBuilder(nullFeaturesFeature);

                blockFeatureBuilderEdit.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                blockFeatureBuilderEdit.BooleanOption.SetTargetBodies(targetBodies1);

                blockFeatureBuilderEdit.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                Point3d originPoint1 = _displayPart.WCS.Origin;

                blockFeatureBuilderEdit.SetOriginAndLengths(originPoint1, mLength.ToString(),
                    mWidth.ToString(), mHeight.ToString());

                blockFeatureBuilderEdit.SetBooleanOperationAndTarget(Feature.BooleanType.Create,
                    nullBody);

                Feature feature1;
                feature1 = blockFeatureBuilderEdit.CommitFeature();

                feature1.SetName("EDIT BLOCK");

                Block editBlock = (Block)feature1;
                Body[] editBody = editBlock.GetBodies();

                CreateBlockLines(originPoint1, mLength, mWidth, mHeight);

                blockFeatureBuilderMatch.Destroy();
                blockFeatureBuilderEdit.Destroy();

                _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                CreateDynamicHandleUdo(editBody[0]);
                ShowDynamicHandles();
                ShowTemporarySizeText();

                Session.UndoMarkId markDeleteObjs1;
                markDeleteObjs1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
                session_.UpdateManager.AddToDeleteList(feature1);
                session_.UpdateManager.DoUpdate(markDeleteObjs1);
                session_.DeleteUndoMark(markDeleteObjs1, "");
            }
            catch (Exception ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                ex.__PrintException();
            }
        }











        private static EditSizeForm NewMethod17(double blockLength, double blockWidth, double blockHeight, Point pointPrototype, EditSizeForm sizeForm, double convertLength, double convertWidth, double convertHeight)
        {
            if (_displayPart.PartUnits == BasePart.Units.Inches)
            {
                if (pointPrototype.Name.Contains("X"))
                {
                    sizeForm = new EditSizeForm(blockLength);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Y"))
                {
                    sizeForm = new EditSizeForm(blockWidth);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Z"))
                {
                    sizeForm = new EditSizeForm(blockHeight);
                    sizeForm.ShowDialog();
                }
            }
            else
            {
                if (pointPrototype.Name.Contains("X"))
                {
                    sizeForm = new EditSizeForm(convertLength);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Y"))
                {
                    sizeForm = new EditSizeForm(convertWidth);
                    sizeForm.ShowDialog();
                }

                if (pointPrototype.Name.Contains("Z"))
                {
                    sizeForm = new EditSizeForm(convertHeight);
                    sizeForm.ShowDialog();
                }
            }

            return sizeForm;
        }

        private static void NewMethod12(List<Line> posXObjs, List<Line> negXObjs, List<Line> posYObjs, List<Line> negYObjs, List<Line> posZObjs, List<Line> negZObjs)
        {
            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                   eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                   eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }



        private void EditDynamicWorkPart(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
                throw new InvalidOperationException("Mirror COmponent");

            _updateComponent = editComponent;
            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return;

            if (_isNewSelection)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;
                UpdateSessionParts();

                if (_workPart.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;

            EditDynamic(isBlockComponent);

            return;
        }

        private void EditDynamicDisplayPart(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return;
            }

            isBlockComponent = _workPart.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            DisableForm();

            if (_isNewSelection)
            {
                CreateEditData(editComponent);
                _isNewSelection = false;
            }

            List<Point> pHandle = SelectHandlePoint();
            _isDynamic = true;
            pHandle = NewMethod7(pHandle);
            EnableForm();
            return;
        }

        private List<Point> NewMethod7(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                _displayPart.WCS.Visibility = false;
                ModelingView mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out int response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            return pHandle;
        }

        private void EditDynamic(bool isBlockComponent)
        {
            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            DisableForm();
            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = true;

            pHandle = NewMethod6(pHandle);

            EnableForm();
        }

        private List<Point> NewMethod6(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                _displayPart.WCS.Visibility = false;
                ModelingView mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out int response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            return pHandle;
        }

        private bool EditMoveWork(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            _updateComponent = editComponent;

            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return isBlockComponent;

            if (_isNewSelection)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;
                UpdateSessionParts();

                if (_workPart.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return isBlockComponent;
            }

            DisableForm();
            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = false;

            pHandle = NewMethod4(pHandle);

            EnableForm();

            return isBlockComponent;
        }



        private List<Point> NewMethod4(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                _displayPart.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
                ModelingView mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out int response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    ShowTemporarySizeText();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            return pHandle;
        }

        private bool EditMoveDisplay(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            isBlockComponent = _workPart.__HasDynamicBlock();

            if (isBlockComponent)
            {
                DisableForm();

                if (_isNewSelection)
                {
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return isBlockComponent;
            }

            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = false;

            pHandle = NewMethod(pHandle);

            EnableForm();

            return isBlockComponent;
        }

        private List<Point> NewMethod(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                _displayPart.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
                ModelingView mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                    out viewTag, out int response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    ShowTemporarySizeText();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            return pHandle;
        }

        private void EditMatch(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            Component editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                EnableForm();
                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                    NXMessageBox.DialogType.Information, "This function is not allowed in this context");
                return;
            }

            Part checkPartName = (Part)editComponent.Prototype;
            isBlockComponent = checkPartName.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            DisableForm();

            if (checkPartName.FullPath.Contains("mirror"))
            {
                EnableForm();
                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                    NXMessageBox.DialogType.Error, "Mirrored Component");
                return;
            }

            _updateComponent = editComponent;

            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return;

            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component - Match To");
            Body editBodyTo = SelectWithFilter.SelectedCompBody;

            if (editBodyTo is null)
            {
                ResetNonBlockError();
                return;
            }

            Component matchComponent = editBodyTo.OwningComponent;
            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
            __work_component_ = matchComponent;
            UpdateSessionParts();
            isBlockComponent = _workPart.__HasDynamicBlock();

            if (isBlockComponent)
                EditMatch(editComponent, matchComponent);
            else
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                    NXMessageBox.DialogType.Error,
                    "Can not match to the selected component");
            }

            EnableForm();

            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
        }

        private void EditMatch(Component editComponent, Component matchComponent)
        {
            DisableForm();

            SetWcsToWorkPart(matchComponent);

            if (_workPart.__HasDynamicBlock())
            {
                // get current block feature
                Block block1 = (Block)_workPart.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatchFrom;
                blockFeatureBuilderMatchFrom =
                    _workPart.Features.CreateBlockFeatureBuilder(block1);
                Point3d blkOrigin = blockFeatureBuilderMatchFrom.Origin;
                string length = blockFeatureBuilderMatchFrom.Length.RightHandSide;
                string width = blockFeatureBuilderMatchFrom.Width.RightHandSide;
                string height = blockFeatureBuilderMatchFrom.Height.RightHandSide;
                blockFeatureBuilderMatchFrom.GetOrientation(out Vector3d xAxisMatch,
                    out Vector3d yAxisMatch);

                __work_part_ = _displayPart; ;
                UpdateSessionParts();
                double[] origin = new double[3];
                double[] matrix = new double[9];
                double[,] transform = new double[4, 4];

                ufsession_.Assem.AskComponentData(matchComponent.Tag,
                    out string partName, out string refSetName, out string instanceName,
                    origin, matrix, transform);

                Tag eInstance =
                    ufsession_.Assem.AskInstOfPartOcc(editComponent.Tag);
                ufsession_.Assem.RepositionInstance(eInstance, origin, matrix);

                __work_component_ = editComponent;
                UpdateSessionParts();

                foreach (Feature featDynamic in _workPart.Features)
                    if (featDynamic.Name == "DYNAMIC BLOCK")
                    {
                        Block block2 = (Block)featDynamic;

                        BlockFeatureBuilder blockFeatureBuilderMatchTo;
                        blockFeatureBuilderMatchTo =
                            _workPart.Features.CreateBlockFeatureBuilder(block2);

                        blockFeatureBuilderMatchTo.BooleanOption.Type =
                            BooleanOperation.BooleanType.Create;

                        Body[] targetBodies1 = new Body[1];
                        Body nullBody = null;
                        targetBodies1[0] = nullBody;
                        blockFeatureBuilderMatchTo.BooleanOption.SetTargetBodies(
                            targetBodies1);

                        blockFeatureBuilderMatchTo.Type = BlockFeatureBuilder.Types
                            .OriginAndEdgeLengths;

                        Point blkFeatBuilderPoint =
                            _workPart.Points.CreatePoint(blkOrigin);
                        blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                        blockFeatureBuilderMatchTo.OriginPoint =
                            blkFeatBuilderPoint;

                        Point3d originPoint1 = blkOrigin;

                        blockFeatureBuilderMatchTo.SetOriginAndLengths(originPoint1,
                            length, width, height);

                        blockFeatureBuilderMatchTo.SetOrientation(xAxisMatch,
                            yAxisMatch);

                        blockFeatureBuilderMatchTo.SetBooleanOperationAndTarget(
                            Feature.BooleanType.Create, nullBody);

                        Feature feature1;
                        feature1 = blockFeatureBuilderMatchTo.CommitFeature();

                        blockFeatureBuilderMatchFrom.Destroy();
                        blockFeatureBuilderMatchTo.Destroy();

                        _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                        session_.Preferences.EmphasisVisualization
                            .WorkPartEmphasis = true;
                        session_.Preferences.Assemblies
                            .WorkPartDisplayAsEntirePart = false;

                        __work_part_ = _originalWorkPart;
                        UpdateSessionParts();

                        _displayPart.WCS.Visibility = true;
                        _displayPart.Views.Refresh();
                    }
            }

            MoveComponent(editComponent);

            EnableForm();
        }





        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if (_displayPart.PartUnits == BasePart.Units.Inches)
            {
                comboBoxGridBlock.Items.Add("0.002");
                comboBoxGridBlock.Items.Add("0.03125");
                comboBoxGridBlock.Items.Add("0.0625");
                comboBoxGridBlock.Items.Add("0.125");
                comboBoxGridBlock.Items.Add("0.250");
                comboBoxGridBlock.Items.Add("0.500");
                comboBoxGridBlock.Items.Add("1.000");
            }
            else
            {
                comboBoxGridBlock.Items.Add("0.0508");
                comboBoxGridBlock.Items.Add("0.79375");
                comboBoxGridBlock.Items.Add("1.00");
                comboBoxGridBlock.Items.Add("1.5875");
                comboBoxGridBlock.Items.Add("3.175");
                comboBoxGridBlock.Items.Add("5.00");
                comboBoxGridBlock.Items.Add("6.35");
                comboBoxGridBlock.Items.Add("12.7");
                comboBoxGridBlock.Items.Add("25.4");
            }

            foreach (string gridSetting in comboBoxGridBlock.Items)
                if (gridSetting == Settings.Default.EditBlockFormGridIncrement)
                {
                    int gridIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);
                    comboBoxGridBlock.SelectedIndex = gridIndex;
                    break;
                }
        }

        private void SetWorkPlaneOn()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = true;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;

#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetWorkPlaneOff()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = false;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;
#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public int Startup()
        {
            if (_registered == 0)
            {
                EditBlockForm editForm = this;
                _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(editForm.WorkPartChanged1);
                _registered = 1;
            }

            return 0;
        }

        public void WorkPartChanged1(BasePart p)
        {
            SetWorkPlaneOff();
            LoadGridSizes();
            SetWorkPlaneOn();
        }







        private List<Point> SelectHandlePoint()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);
            Selection.Response sel;
            List<Point> pointSelection = new List<Point>();

            sel = TheUISession.SelectionManager.SelectTaggedObject("Select Point", "Select Point",
                Selection.SelectionScope.WorkPart,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject selectedPoint, out Point3d cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
                pointSelection.Add((Point)selectedPoint);

            return pointSelection;
        }

        private Component SelectOneComponent(string prompt)
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;

            sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject selectedComp, out Point3d cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
            {
                compSelection = (Component)selectedComp;
                return compSelection;
            }

            return null;
        }





        private void MoveComponent(Component compToMove)
        {
            try
            {
                UpdateSessionParts();

                if (compToMove is null)
                    return;

                BasePart.Units assmUnits = _displayPart.PartUnits;
                BasePart compBase = (BasePart)compToMove.Prototype;
                BasePart.Units compUnits = compBase.PartUnits;

                if (compUnits != assmUnits)
                    return;

                UpdateSessionParts();
                CreateEditData(compToMove);
                _isNewSelection = false;
                List<Point> pHandle = new List<Point>();
                pHandle = SelectHandlePoint();
                _isDynamic = false;

                pHandle = NewMethod10(pHandle);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private List<Point> NewMethod10(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                _displayPart.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;

                using (session_.__UsingLockUiFromCustom())
                {
                    ModelingView mView = (ModelingView)_displayPart.Views.WorkView;
                    _displayPart.Views.WorkView.Orient(mView.Matrix);
                    _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                    ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                        out viewTag, out int response);

                    if (response != UF_UI_PICK_RESPONSE)
                        continue;

                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }
            }

            return pHandle;
        }



        private Point3d MapWcsToAbsolute(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, input, UF_CSYS_ROOT_COORDS, output);
            return output.__ToPoint3d();
        }

        private Point3d MapAbsoluteToWcs(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            return output.__ToPoint3d();
        }



        private void UpdateDynamicBlock(Component updateComp)
        {
            try
            {
                // set component translucency and update dynamic block

                if (updateComp != null)
                    updateComp.__Translucency(0);
                else
                    foreach (Body dispBody in _workPart.Bodies)
                        if (dispBody.Layer == 1)
                            dispBody.__Translucency(0);

                Point3d blkOrigin = new Point3d();
                string length = "";
                string width = "";
                string height = "";

                foreach (Point pPoint in _workPart.Points)
                {
                    if (pPoint.Name != "BLKORIGIN")
                        continue;

                    blkOrigin.X = pPoint.Coordinates.X;
                    blkOrigin.Y = pPoint.Coordinates.Y;
                    blkOrigin.Z = pPoint.Coordinates.Z;
                }

                foreach (Line blkLine in _edgeRepLines)
                {
                    if (blkLine.Name == "XBASE1") length = blkLine.GetLength().ToString();

                    if (blkLine.Name == "YBASE1") width = blkLine.GetLength().ToString();

                    if (blkLine.Name == "ZBASE1") height = blkLine.GetLength().ToString();
                }

                if (_isUprParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                if (_isLwrParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                _displayPart.WCS.SetOriginAndMatrix(blkOrigin, _workCompOrientation);
                _workCompOrigin = blkOrigin;

                __work_component_ = updateComp;
                UpdateSessionParts();

                double[] fromPoint = { blkOrigin.X, blkOrigin.Y, blkOrigin.Z };
                double[] mappedPoint = new double[3];
                ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, fromPoint, UF_CSYS_WORK_COORDS, mappedPoint);

                blkOrigin.X = mappedPoint[0];
                blkOrigin.Y = mappedPoint[1];
                blkOrigin.Z = mappedPoint[2];

                if (!_workPart.__HasDynamicBlock())
                    return;

                Block block2 = _workPart.__DynamicBlock();
                BlockFeatureBuilder builder = _workPart.Features.CreateBlockFeatureBuilder(block2);

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    // builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                    // Body[] targetBodies1 = new Body[1];
                    // Body nullBody = null;
                    // targetBodies1[0] = nullBody;
                    // builder.BooleanOption.SetTargetBodies(targetBodies1);
                    // builder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                    Point blkFeatBuilderPoint = _workPart.Points.CreatePoint(blkOrigin);
                    blkFeatBuilderPoint.SetCoordinates(blkOrigin);
                    builder.OriginPoint = blkFeatBuilderPoint;
                    Point3d originPoint1 = blkOrigin;
                    builder.SetOriginAndLengths(originPoint1, length, width, height);
                    // builder.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);
                    // Feature feature1;
                    builder.CommitFeature();
                }

                __work_part_ = _displayPart;
                UpdateSessionParts();
                DeleteHandles();
                __work_part_ = _originalWorkPart;
                UpdateSessionParts();
                _displayPart.WCS.Visibility = true;
                _displayPart.Views.Refresh();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }





    }
}
// 2045