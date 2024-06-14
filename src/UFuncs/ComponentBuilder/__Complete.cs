using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void ButtonEditMove_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod11();
                NewMethod2();

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditMoveDisplay(isBlockComponent, editComponent)
                    : EditMoveWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

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

        private void ButtonAlignComponent_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod9();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                AlignComponent(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
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

        private void ButtonEditDynamic_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod8();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component for Dynamic Edit");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    EditDynamicDisplayPart(isBlockComponent, editComponent);
                else
                    EditDynamicWorkPart(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                EnableForm();
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
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

        private void ButtonViewWcs_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            Matrix3x3 orientation = coordSystem.Orientation.Element;
            _displayPart.Views.WorkView.Orient(orientation);
        }


        private void ButtonEditMatch_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod5();

                if (_isNewSelection)
                {
                    if (_updateComponent is null)
                    {
                        SelectWithFilter_("Select Component - Match From");
                    }
                    else
                    {
                        UpdateDynamicBlock(_updateComponent);
                        _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                        _displayPart.WCS.Visibility = true;
                        _isNewSelection = true;
                    }
                }
                else
                {
                    UpdateDynamicBlock(_updateComponent);
                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                    _displayPart.WCS.Visibility = true;
                    _isNewSelection = true;
                }

                EditMatch(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            EnableForm();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
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

        private void ButtonEditSize_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod3();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Edit Size");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditSize(isBlockComponent, editComponent)
                    : EditSizeWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
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

        private void ButtonEditAlign_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod1();


                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                EdgeAlign(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
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

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e)
        {
            try
            {
                bool isBlockComponent = NewMethod13();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align Edge");

                AlignEdgeDistance(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
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

        private void ComboBoxGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Return))
                return;

            if (comboBoxGridBlock.Text == "0.000")
            {
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            UpdateDynamicBlock(_updateComponent);
            __work_part_ = _originalWorkPart;
            UpdateSessionParts();
            UpdateOriginalParts();
            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            _displayPart.WCS.Visibility = true;
            session_.DeleteAllUndoMarks();
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonApply.Enabled = false;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
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


        private double AlignEdgeDistance(
           List<NXObject> movePtsHalf,
           List<NXObject> movePtsFull,
           List<Line> lines,
           double inputDist,
           double[] mappedBase,
           double[] mappedPoint,
           int index,
           string dir_xyz,
           bool isPosEnd)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, index);

            foreach (Line zAxisLine in lines)
                switch (dir_xyz)
                {
                    case "X":
                        if (isPosEnd)
                            XEndPoint(distance, zAxisLine);
                        else
                            XStartPoint(distance, zAxisLine);
                        continue;
                    case "Y":
                        if (isPosEnd)
                            YEndPoint(distance, zAxisLine);
                        else
                            YStartPoint(distance, zAxisLine);
                        continue;
                    case "Z":
                        if (isPosEnd)
                            ZEndPoint(distance, zAxisLine);
                        else
                            ZStartPoint(distance, zAxisLine);
                        continue;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private void AlignEdgeDistance(bool isBlockComponent)
        {
            using (session_.__UsingFormShowHide(this))
            {
                if (_editBody is null)
                    return;

                var editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    TheUISession.NXMessageBox.Show(
                        "Error",
                        NXMessageBox.DialogType.Information,
                        "This function is not allowed in this context");

                    return;
                }

                var checkPartName = (Part)editComponent.Prototype;


                _updateComponent = editComponent;

                var assmUnits = _displayPart.PartUnits;
                var compBase = (BasePart)editComponent.Prototype;
                var compUnits = compBase.PartUnits;

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

                if (!isBlockComponent)
                {
                    ResetNonBlockError();

                    TheUISession.NXMessageBox.Show(
                        "Caught exception",
                        NXMessageBox.DialogType.Error,
                        "Not a block component");

                    return;
                }

                var pHandle = SelectHandlePoint();

                _isDynamic = true;

                while (pHandle.Count == 1)
                {
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];
                    Point pointPrototype;

                    if (_udoPointHandle.IsOccurrence)
                        pointPrototype = (Point)_udoPointHandle.Prototype;
                    else
                        pointPrototype = _udoPointHandle;

                    var doNotMovePts = new List<NXObject>();
                    var movePtsHalf = new List<NXObject>();
                    var movePtsFull = new List<NXObject>();
                    MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));
                    var posXObjs = new List<Line>();
                    var negXObjs = new List<Line>();
                    var posYObjs = new List<Line>();
                    var negYObjs = new List<Line>();
                    var posZObjs = new List<Line>();
                    var negZObjs = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
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

                    var allxAxisLines = new List<Line>();
                    var allyAxisLines = new List<Line>();
                    var allzAxisLines = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name.StartsWith("X"))
                            allxAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Y"))
                            allyAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Z"))
                            allzAxisLines.Add(eLine);
                    }

                    var message = "Select Reference Point";
                    var pbMethod = UFUi.PointBaseMethod.PointInferred;
                    var selection = NXOpen.Tag.Null;
                    var basePoint = new double[3];

                    ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                    ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                        out var response);

                    ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                    if (response == UF_UI_OK)
                    {
                        bool isDistance;

                        isDistance = NXInputBox.ParseInputNumber(
                            "Enter offset value",
                            "Enter offset value",
                            .004,
                            NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture.NumberFormat,
                            out var inputDist);

                        if (isDistance)
                        {
                            var mappedBase = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                                UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                            double[] pPrototype =
                            {
                                            pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                            pointPrototype.Coordinates.Z
                                        };
                            var mappedPoint = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype,
                                UF_CSYS_ROOT_WCS_COORDS, mappedPoint);

                            double distance;

                            if (pointPrototype.Name == "POSX")
                            {
                                foreach (Line posXLine in posXObjs) movePtsFull.Add(posXLine);

                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist, mappedBase, mappedPoint, 0, "X", true);
                            }

                            if (pointPrototype.Name == "NEGX")
                            {
                                foreach (Line addLine in negXObjs) movePtsFull.Add(addLine);

                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist, mappedBase, mappedPoint, 0, "X", false);
                            }

                            if (pointPrototype.Name == "POSY")
                            {
                                foreach (Line addLine in posYObjs) movePtsFull.Add(addLine);

                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist, mappedBase, mappedPoint, 1, "Y", true);
                            }

                            if (pointPrototype.Name == "NEGY")
                            {
                                foreach (Line addLine in negYObjs) movePtsFull.Add(addLine);
                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist, mappedBase, mappedPoint, 1, "Y", false);
                            }

                            if (pointPrototype.Name == "POSZ")
                            {
                                foreach (Line addLine in posZObjs) movePtsFull.Add(addLine);
                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist, mappedBase, mappedPoint, 2, "Z", true);
                            }

                            if (pointPrototype.Name == "NEGZ")
                            {
                                foreach (Line addLine in negZObjs) movePtsFull.Add(addLine);
                                distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist, mappedBase, mappedPoint, 2, "Z", false);
                            }
                        }
                        else
                        {
                            //Show();
                            TheUISession.NXMessageBox.Show("Caught exception",
                                NXMessageBox.DialogType.Error, "Invalid input");
                        }
                    }

                    UpdateDynamicBlock(editComponent);
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                    __work_component_ = editComponent;
                    UpdateSessionParts();
                    CreateEditData(editComponent);
                    pHandle = SelectHandlePoint();
                }
            }
        }

        private bool EditSize(bool isBlockComponent, Component editComponent)
        {
            if (!_workPart.__HasDynamicBlock())
            {
                ResetNonBlockError();

                TheUISession.NXMessageBox.Show(
                    "Caught exception",
                    NXMessageBox.DialogType.Error,
                    "Not a block component");

                return isBlockComponent;
            }

            DisableForm();

            if (_isNewSelection)
            {
                CreateEditData(editComponent);

                _isNewSelection = false;
            }

            List<Point> pHandle = SelectHandlePoint();

            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();

                _udoPointHandle = pHandle[0];

                Point3d blockOrigin = new Point3d();
                double blockLength = 0.00;
                double blockWidth = 0.00;
                double blockHeight = 0.00;

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name == "XBASE1")
                    {
                        blockOrigin = eLine.StartPoint;
                        blockLength = eLine.GetLength();
                    }

                    if (eLine.Name == "YBASE1") blockWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1") blockHeight = eLine.GetLength();
                }

                Point pointPrototype;

                if (_udoPointHandle.IsOccurrence)
                    pointPrototype = (Point)_udoPointHandle.Prototype;
                else
                    pointPrototype = _udoPointHandle;

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

                NewMethod15(posXObjs, negXObjs, posYObjs, negYObjs, posZObjs, negZObjs);

                List<Line> allxAxisLines = new List<Line>();
                List<Line> allyAxisLines = new List<Line>();
                List<Line> allzAxisLines = new List<Line>();

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                }

                EditSizeForm sizeForm = null;
                double convertLength = blockLength / 25.4;
                double convertWidth = blockWidth / 25.4;
                double convertHeight = blockHeight / 25.4;

                sizeForm = NewMethod14(blockLength, blockWidth, blockHeight, pointPrototype, sizeForm, convertLength, convertWidth, convertHeight);

                if (sizeForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    double editSize = sizeForm.InputValue;

                    if (_displayPart.PartUnits == BasePart.Units.Millimeters)
                        editSize *= 25.4;

                    if (editSize > 0)
                    {
                        if (pointPrototype.Name == "POSX")
                        {
                            movePtsFull.AddRange(posXObjs);

                            EditSize(
                               editSize - blockLength,
                               movePtsHalf,
                               movePtsFull,
                               allxAxisLines,
                               "X",
                               true);
                        }

                        if (pointPrototype.Name == "NEGX")
                        {
                            movePtsFull.AddRange(negXObjs);

                            EditSize(
                               blockLength - editSize,
                               movePtsHalf,
                               movePtsFull,
                               allxAxisLines,
                               "X",
                               false);
                        }

                        if (pointPrototype.Name == "POSY")
                        {
                            movePtsFull.AddRange(posYObjs);

                            EditSize(
                               editSize - blockWidth,
                               movePtsHalf,
                               movePtsFull,
                               allyAxisLines,
                               "Y",
                               true);
                        }

                        if (pointPrototype.Name == "NEGY")
                        {
                            movePtsFull.AddRange(negYObjs);

                            EditSize(
                               blockWidth - editSize,
                               movePtsHalf,
                               movePtsFull,
                               allyAxisLines,
                               "Y",
                               false);
                        }

                        if (pointPrototype.Name == "POSZ")
                        {
                            movePtsFull.AddRange(posZObjs);

                            EditSize(
                               editSize - blockHeight,
                               movePtsHalf,
                               movePtsFull,
                               allzAxisLines,
                               "Z",
                               true);
                        }

                        if (pointPrototype.Name == "NEGZ")
                        {
                            movePtsFull.AddRange(negZObjs);

                            EditSize(
                               blockHeight - editSize,
                               movePtsHalf,
                               movePtsFull,
                               allzAxisLines,
                               "Z",
                               false);
                        }
                    }
                }

                UpdateDynamicBlock(editComponent);
                sizeForm.Close();
                sizeForm.Dispose();
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;
                UpdateSessionParts();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                CreateEditData(editComponent);
                pHandle = SelectHandlePoint();
            }

            EnableForm();
            return isBlockComponent;
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

        private double EditSize(
            double distance,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allzAxisLines,
            string dir_xyz,
            bool isPosEnd)
        {
            foreach (Line zAxisLine in allzAxisLines)
                switch (dir_xyz)
                {
                    case "X":
                        if (isPosEnd)
                            XEndPoint(distance, zAxisLine);
                        else
                            XStartPoint(distance, zAxisLine);
                        continue;
                    case "Y":
                        if (isPosEnd)
                            YEndPoint(distance, zAxisLine);
                        else
                            YStartPoint(distance, zAxisLine);
                        continue;
                    case "Z":
                        if (isPosEnd)
                            ZEndPoint(distance, zAxisLine);
                        else
                            ZStartPoint(distance, zAxisLine);
                        continue;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private void EditAlign(
          List<NXObject> movePtsHalf,
          List<NXObject> movePtsFull,
          List<Line> allzAxisLines,
          double distance,
          string dir_xyz,
          bool isPosEnd)
        {
            foreach (Line zAxisLine in allzAxisLines)
                switch (dir_xyz)
                {
                    case "X":
                        if (isPosEnd)
                            XEndPoint(distance, zAxisLine);
                        else
                            XStartPoint(distance, zAxisLine);
                        continue;
                    case "Y":
                        if (isPosEnd)
                            YEndPoint(distance, zAxisLine);
                        else
                            YStartPoint(distance, zAxisLine);
                        continue;
                    case "Z":
                        if (isPosEnd)
                            ZEndPoint(distance, zAxisLine);
                        else
                            ZStartPoint(distance, zAxisLine);
                        continue;
                    default:
                        throw new ArgumentException();
                }

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
        }


        private void EdgeAlign(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            var editComponent = _editBody.OwningComponent;

            if (editComponent is null)
            {
                Show();

                TheUISession.NXMessageBox.Show(
                    "Error",
                    NXMessageBox.DialogType.Information,
                    "This function is not allowed in this context");

                return;
            }

            var checkPartName = (Part)editComponent.Prototype;


            _updateComponent = editComponent;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

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
            {
                isBlockComponent = true;
            }

            if (!isBlockComponent)
            {
                ResetNonBlockError();

                TheUISession.NXMessageBox.Show(
                    "Caught exception",
                    NXMessageBox.DialogType.Error,
                    "Not a block component");

                return;
            }

            UpdateDynamicBlock(editComponent);
            CreateEditData(editComponent);

            var pHandle = SelectHandlePoint();

            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();

                _udoPointHandle = pHandle[0];

                Hide();

                Point pointPrototype;

                if (_udoPointHandle.IsOccurrence)
                    pointPrototype = (Point)_udoPointHandle.Prototype;
                else
                    pointPrototype = _udoPointHandle;

                var doNotMovePts = new List<NXObject>();
                var movePtsHalf = new List<NXObject>();
                var movePtsFull = new List<NXObject>();

                MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                var posXObjs = new List<Line>();
                var negXObjs = new List<Line>();
                var posYObjs = new List<Line>();
                var negYObjs = new List<Line>();
                var posZObjs = new List<Line>();
                var negZObjs = new List<Line>();

                foreach (var eLine in _edgeRepLines)
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

                var allxAxisLines = new List<Line>();
                var allyAxisLines = new List<Line>();
                var allzAxisLines = new List<Line>();

                foreach (var eLine in _edgeRepLines)
                {
                    if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                }

                var message = "Select Reference Point";
                var pbMethod = UFUi.PointBaseMethod.PointInferred;
                var selection = NXOpen.Tag.Null;
                var basePoint = new double[3];

                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                    out var response);

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                if (response == UF_UI_OK)
                {
                    var mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                        UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                    double[] pPrototype =
                    {
                                         pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                         pointPrototype.Coordinates.Z
                                     };
                    var mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype,
                        UF_CSYS_ROOT_WCS_COORDS, mappedPoint);


                    int index;

                    switch (pointPrototype.Name)
                    {
                        case "POSX":
                        case "NEGX":
                            index = 0;
                            break;
                        case "POSY":
                        case "NEGY":
                            index = 1;
                            break;
                        case "POSZ":
                        case "NEGZ":
                            index = 2;
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }

                    double distance;

                    distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

                    if (mappedBase[index] < mappedPoint[index])
                        distance *= -1;

                    switch (pointPrototype.Name)
                    {
                        case "POSX":
                            movePtsFull.AddRange(posXObjs);
                            EditAlign(movePtsHalf, movePtsFull, allxAxisLines, distance, "X", true);
                            break;
                        case "NEGX":
                            movePtsFull.AddRange(negXObjs);
                            EditAlign(movePtsHalf, movePtsFull, allxAxisLines, distance, "X", false);
                            break;
                        case "POSY":
                            movePtsFull.AddRange(posYObjs);
                            EditAlign(movePtsHalf, movePtsFull, allyAxisLines, distance, "Y", true);
                            break;
                        case "NEGY":
                            movePtsFull.AddRange(negYObjs);
                            EditAlign(movePtsHalf, movePtsFull, allyAxisLines, distance, "Y", false);
                            break;
                        case "POSZ":
                            movePtsFull.AddRange(posZObjs);
                            EditAlign(movePtsHalf, movePtsFull, allzAxisLines, distance, "Z", true);
                            break;
                        case "NEGZ":
                            movePtsFull.AddRange(negZObjs);
                            EditAlign(movePtsHalf, movePtsFull, allzAxisLines, distance, "Z", false);
                            break;
                    }
                }

                UpdateDynamicBlock(editComponent);
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;
                UpdateSessionParts();
                CreateEditData(editComponent);
                pHandle = SelectHandlePoint();
            }

            Show();
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



    }
}