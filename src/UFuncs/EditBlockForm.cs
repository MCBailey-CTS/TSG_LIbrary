﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using Part = NXOpen.Part;
using static NXOpen.UF.UFConstants;
using TSG_Library.Properties;
using System.Globalization;
using NXOpenUI;
using NXOpen.UF;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Preferences;
using NXOpen.Utilities;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {
        #region variables

        //private static Part _originalWorkPart = __work_part_;
        //private static Part _originalDisplayPart = __display_part_;
        private static bool _isDynamic;
        private static Point _udoPointHandle;
        private static double _gridSpace;
        private static Point3d _workCompOrigin;
        private static Matrix3x3 _workCompOrientation;
        private static readonly List<string> _nonValidNames = new List<string>();
        private static readonly List<Line> _edgeRepLines = new List<Line>();
        private static double _distanceMoved;
        private static int _registered;
        private static int _idWorkPartChanged1;
        private static Component _updateComponent;
        private static Body _editBody;
        private static bool _isNewSelection = true;
        private static bool _isUprParallel;
        private static bool _isLwrParallel;
        private static string _parallelHeightExp = string.Empty;
        private static string _parallelWidthExp = string.Empty;

        #endregion

        #region form events

        public EditBlockForm() => InitializeComponent();

        private void EditBlockForm_Load(object sender, EventArgs e) => EditBlock();

        private void ComboBoxGridBlock_SelectedIndexChanged(object sender, EventArgs e) =>
            SelectGrid();

        private void ButtonExit_Click(object sender, EventArgs e) => Exit();

        private void ButtonApply_Click(object sender, EventArgs e) => Apply();

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

        private void ButtonViewWcs_Click(object sender, EventArgs e) => ViewWcs();

        private void ButtonEditMatch_Click(object sender, EventArgs e) => EditMatch();

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e) => AlignEdgeDistance();

        private void ButtonAlignComponent_Click(object sender, EventArgs e) => AlignComponent();

        private void ButtonEditDynamic_Click(object sender, EventArgs e) => EditDynamic();

        private void ButtonEditSize_Click(object sender, EventArgs e) => EditSize();

        private void ButtonEditMove_Click(object sender, EventArgs e) => EditMove();

        private void ButtonEditAlign_Click(object sender, EventArgs e) => EditAlign();

        private void ButtonReset_Click(object sender, EventArgs e) => Reset();

        #endregion


        #region form methods

        private void Apply()
        {
            UpdateDynamicBlock(_updateComponent);
            __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            __display_part_.WCS.Visibility = true;
            session_.DeleteAllUndoMarks();
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            buttonApply.Enabled = false;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
        }

        private void Reset()
        {
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            __work_part_ = __display_part_;
        }

        private void Exit()
        {
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
            Close();
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();

            using (this)
                new ComponentBuilder().Show();
        }


        private void ViewWcs()
        {
            CoordinateSystem coordSystem = __display_part_.WCS.CoordinateSystem;
            Matrix3x3 orientation = coordSystem.Orientation.Element;
            __display_part_.Views.WorkView.Orient(orientation);
        }

        private void AlignComponent()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                AlignComponent(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
            }
        }

        private void AlignComponent(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            Component editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                Show();
                NXMessage("This function is not allowed in this context, must be at assembly level");
                return;
            }

            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                Show();
                throw new NotImplementedException("NXMessage");
            }

            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;

            isBlockComponent = IsBlockComponent(isBlockComponent, editComponent);

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                throw new NotImplementedException("This is a NXMessage ");
            }

            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();

            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();

                _udoPointHandle = pHandle[0];

                Hide();

                Point pointPrototype = _udoPointHandle.IsOccurrence
                    ? (Point)_udoPointHandle.Prototype
                    : _udoPointHandle;

                List<NXObject> movePtsFull = new List<NXObject>();

                foreach (Point nPoint in __work_part_.Points)
                    if (
                        nPoint.Name.Contains("X")
                        || nPoint.Name.Contains("Y")
                        || nPoint.Name.Contains("Z")
                        || nPoint.Name.Contains("BLKORIGIN")
                    )
                        movePtsFull.Add(nPoint);

                foreach (Line nLine in __work_part_.Lines)
                    if (
                        nLine.Name.Contains("X")
                        || nLine.Name.Contains("Y")
                        || nLine.Name.Contains("Z")
                    )
                        movePtsFull.Add(nLine);

                string message = "Select Reference Point";
                UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                Tag selection = NXOpen.Tag.Null;
                double[] basePoint = new double[3];
                int response;

                using (session_.__UsingLockUiFromCustom())
                    ufsession_.Ui.PointConstruct(
                        message,
                        ref pbMethod,
                        out selection,
                        basePoint,
                        out response
                    );

                if (response == UF_UI_OK)
                {
                    double[] mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(
                        UF_CSYS_ROOT_COORDS,
                        basePoint,
                        UF_CSYS_ROOT_WCS_COORDS,
                        mappedBase
                    );

                    double[] pPrototype = pointPrototype.Coordinates.__ToArray();
                    double[] mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(
                        UF_CSYS_ROOT_COORDS,
                        pPrototype,
                        UF_CSYS_ROOT_WCS_COORDS,
                        mappedPoint
                    );

                    double distance;

                    switch (pointPrototype.Name)
                    {
                        case "POSX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "NEGX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "POSY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "NEGY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "POSZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        case "NEGZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2])
                                distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

                pHandle = UpdateCreateSelect(editComponent);
            }

            Show();
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
            print_(inputDist);
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, index);
            print_(distance);
            foreach (Line zAxisLine in lines)
                SetLineEndPoints(distance, zAxisLine, !isPosEnd, dir_xyz);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private void AlignEdgeDistance(bool isBlockComponent)
        {
            using (session_.__UsingFormShowHide(this))
            {
                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    NXMessage("This function is not allowed in this context");
                    return;
                }

                Part checkPartName = (Part)editComponent.Prototype;
                _updateComponent = editComponent;

                if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                    return;

                isBlockComponent = IsBlockComponent(isBlockComponent, editComponent);

                if (!isBlockComponent)
                {
                    ResetNonBlockError();
                    NXMessage("Not a block component");
                    return;
                }

                List<Point> pHandle = SelectHandlePoint();

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


                    MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf,
                        out var movePtsFull, pointPrototype.Name.Contains("POS"));
                    GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs,
                        out var negZObjs);

                    AskAxisLines(out List<Line> allxAxisLines, out List<Line> allyAxisLines, out List<Line> allzAxisLines);

                    string message = "Select Reference Point";
                    UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                    Tag selection = NXOpen.Tag.Null;
                    double[] basePoint = new double[3];
                    int response;

                    using (session_.__UsingLockUiFromCustom())
                        ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint, out response);

                    if (response == UF_UI_OK)
                    {
                        bool isDistance;

                        string output = NXInputBox.GetInputString("Enter offset value", "Enter offset value", ".004");

                        bool isMetric = output.ToLower().Contains("mm");

                        var nomm = output.ToLower().Replace("mm", "");

                        isDistance = double.TryParse(nomm, out double inputDist);

                        if (isDistance)
                        {
                            if (isMetric)
                                inputDist = inputDist / 25.4;

                            double[] mappedBase = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint, UF_CSYS_ROOT_WCS_COORDS,
                                mappedBase);

                            double[] pPrototype = pointPrototype.Coordinates.__ToArray();
                            double[] mappedPoint = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype, UF_CSYS_ROOT_WCS_COORDS,
                                mappedPoint);

                            double distance;
                            print_(pointPrototype.Name);
                            switch (pointPrototype.Name)
                            {
                                case "POSX":
                                    movePtsFull.AddRange(posXObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist,
                                        mappedBase, mappedPoint, 0, "X", true);
                                    break;
                                case "NEGX":
                                    movePtsFull.AddRange(negXObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist,
                                        mappedBase, mappedPoint, 0, "X", false);
                                    break;
                                case "POSY":
                                    movePtsFull.AddRange(posYObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist,
                                        mappedBase, mappedPoint, 1, "Y", true);
                                    break;
                                case "NEGY":
                                    movePtsFull.AddRange(negYObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist,
                                        mappedBase, mappedPoint, 1, "Y", false);
                                    break;
                                case "POSZ":
                                    movePtsFull.AddRange(posZObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist,
                                        mappedBase, mappedPoint, 2, "Z", true);
                                    break;
                                case "NEGZ":
                                    movePtsFull.AddRange(negZObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist,
                                        mappedBase, mappedPoint, 2, "Z", false);
                                    break;
                            }
                        }
                        else
                        {
                            //Show();
                            NXMessage("Invalid input");
                        }
                    }

                    pHandle = UpdateCreateSelect(editComponent);
                }
            }
        }

        private void AlignEdgeDistance()
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align Edge");

                AlignEdgeDistance(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
            }
        }


        private void EditMatch(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            Component editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                EnableForm();
                NXMessage("This function is not allowed in this context");
                return;
            }

            Part checkPartName = (Part)editComponent.Prototype;
            isBlockComponent = checkPartName.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return;
            }

            DisableForm();

            if (checkPartName.FullPath.Contains("mirror"))
            {
                EnableForm();
                NXMessage("Mirrored Component");
                return;
            }

            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
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
            __work_component_ = matchComponent;
            isBlockComponent = __work_part_.__HasDynamicBlock();

            if (isBlockComponent)
                EditMatch(editComponent, matchComponent);
            else
            {
                ResetNonBlockError();
                NXMessage("Can not match to the selected component");
            }

            EnableForm();

        }

        private void EditMatch(Component editComponent, Component matchComponent)
        {
            DisableForm();

            SetWcsToWorkPart(matchComponent);

            if (__work_part_.__HasDynamicBlock())
            {
                // get current block feature
                Block block1 = (Block)__work_part_.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatchFrom;
                blockFeatureBuilderMatchFrom = __work_part_.Features.CreateBlockFeatureBuilder(
                    block1
                );
                Point3d blkOrigin = blockFeatureBuilderMatchFrom.Origin;
                string length = blockFeatureBuilderMatchFrom.Length.RightHandSide;
                string width = blockFeatureBuilderMatchFrom.Width.RightHandSide;
                string height = blockFeatureBuilderMatchFrom.Height.RightHandSide;
                blockFeatureBuilderMatchFrom.GetOrientation(
                    out Vector3d xAxisMatch,
                    out Vector3d yAxisMatch
                );

                __work_part_ = __display_part_;
                ;
                double[] origin = new double[3];
                double[] matrix = new double[9];
                double[,] transform = new double[4, 4];

                ufsession_.Assem.AskComponentData(
                    matchComponent.Tag,
                    out string partName,
                    out string refSetName,
                    out string instanceName,
                    origin,
                    matrix,
                    transform
                );

                Tag eInstance = ufsession_.Assem.AskInstOfPartOcc(editComponent.Tag);
                ufsession_.Assem.RepositionInstance(eInstance, origin, matrix);

                __work_component_ = editComponent;

                foreach (Feature featDynamic in __work_part_.Features)
                    if (featDynamic.Name == "DYNAMIC BLOCK")
                    {
                        Block block2 = (Block)featDynamic;

                        BlockFeatureBuilder blockFeatureBuilderMatchTo;
                        blockFeatureBuilderMatchTo =
                            __work_part_.Features.CreateBlockFeatureBuilder(block2);

                        blockFeatureBuilderMatchTo.BooleanOption.Type = BooleanOperation
                            .BooleanType
                            .Create;

                        Body[] targetBodies1 = new Body[1];
                        Body nullBody = null;
                        targetBodies1[0] = nullBody;
                        blockFeatureBuilderMatchTo.BooleanOption.SetTargetBodies(targetBodies1);

                        blockFeatureBuilderMatchTo.Type = BlockFeatureBuilder
                            .Types
                            .OriginAndEdgeLengths;

                        Point blkFeatBuilderPoint = __work_part_.Points.CreatePoint(blkOrigin);
                        blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                        blockFeatureBuilderMatchTo.OriginPoint = blkFeatBuilderPoint;

                        Point3d originPoint1 = blkOrigin;

                        blockFeatureBuilderMatchTo.SetOriginAndLengths(
                            originPoint1,
                            length,
                            width,
                            height
                        );

                        blockFeatureBuilderMatchTo.SetOrientation(xAxisMatch, yAxisMatch);

                        blockFeatureBuilderMatchTo.SetBooleanOperationAndTarget(
                            Feature.BooleanType.Create,
                            nullBody
                        );

                        Feature feature1;
                        feature1 = blockFeatureBuilderMatchTo.CommitFeature();

                        blockFeatureBuilderMatchFrom.Destroy();
                        blockFeatureBuilderMatchTo.Destroy();

                        //__work_part_ = _originalWorkPart;

                        __display_part_.WCS.Visibility = true;
                        __display_part_.Views.Refresh();
                    }
            }

            MoveComponent(editComponent);

            EnableForm();
        }


        private void EditAlign()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();


                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                EdgeAlign(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
            }
        }

        private void EdgeAlign(bool isBlockComponent)
        {
            System.Diagnostics.Debugger.Launch();

            if (_editBody is null)
                return;

            Component editComponent = _editBody.OwningComponent;

            if (editComponent is null)
            {
                Show();
                NXMessage("This function is not allowed in this context");
                return;
            }

            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;
            
            isBlockComponent = IsBlockComponent(isBlockComponent, editComponent);

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return;
            }

            UpdateDynamicBlock(editComponent);
            CreateEditData(editComponent);
            List<Point> pHandle = SelectHandlePoint();
            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                Hide();

                Point pointPrototype = _udoPointHandle.IsOccurrence
                    ? (Point)_udoPointHandle.Prototype
                    : _udoPointHandle;

                MotionCallbackDynamic1(
                    pointPrototype,
                    out var doNotMovePts,
                    out var movePtsHalf,
                    out var movePtsFull,
                    pointPrototype.Name.Contains("POS")
                );
                GetLines(
                    out var posXObjs,
                    out var negXObjs,
                    out var posYObjs,
                    out var negYObjs,
                    out var posZObjs,
                    out var negZObjs
                );
                AskAxisLines(out List<Line> allxAxisLines, out List<Line> allyAxisLines, out List<Line> allzAxisLines);

                string message = "Select Reference Point";
                UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                Tag selection = NXOpen.Tag.Null;
                double[] basePoint = new double[3];
                int response;

                using (session_.__UsingLockUiFromCustom())
                    ufsession_.Ui.PointConstruct(
                        message,
                        ref pbMethod,
                        out selection,
                        basePoint,
                        out response
                    );

                if (response == UF_UI_OK)
                {
                    double[] mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(
                        UF_CSYS_ROOT_COORDS,
                        basePoint,
                        UF_CSYS_ROOT_WCS_COORDS,
                        mappedBase
                    );
                    double[] pPrototype = pointPrototype.Coordinates.__ToArray();
                    double[] mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(
                        UF_CSYS_ROOT_COORDS,
                        pPrototype,
                        UF_CSYS_ROOT_WCS_COORDS,
                        mappedPoint
                    );
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
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allxAxisLines,
                                "X",
                                true
                            );
                            break;
                        case "NEGX":
                            movePtsFull.AddRange(negXObjs);
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allxAxisLines,
                                "X",
                                false
                            );
                            break;
                        case "POSY":
                            movePtsFull.AddRange(posYObjs);
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allyAxisLines,
                                "Y",
                                true
                            );
                            break;
                        case "NEGY":
                            movePtsFull.AddRange(negYObjs);
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allyAxisLines,
                                "Y",
                                false
                            );
                            break;
                        case "POSZ":
                            movePtsFull.AddRange(posZObjs);
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allzAxisLines,
                                "Z",
                                true
                            );
                            break;
                        case "NEGZ":
                            movePtsFull.AddRange(negZObjs);
                            EditSizeOrAlign(
                                distance,
                                movePtsHalf,
                                movePtsFull,
                                allzAxisLines,
                                "Z",
                                false
                            );
                            break;
                    }
                }

                pHandle = UpdateCreateSelect(editComponent);
            }

            Show();
        }


        private void EditDynamic(bool isBlockComponent)
        {
            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return;
            }

            DisableForm();
            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = true;

            pHandle = NewMethod6(pHandle);

            EnableForm();
        }

        private void EditDynamicWorkPart(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
                throw new InvalidOperationException("Mirror COmponent");

            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;

            isBlockComponent = IsBlockComponent(isBlockComponent, editComponent);
            EditDynamic(isBlockComponent);
            return;
        }

        private void EditDynamicDisplayPart(bool isBlockComponent, Component editComponent)
        {
            if (__display_part_.FullPath.Contains("mirror"))
            {
                NXMessage("Mirrored Component");
                return;
            }

            isBlockComponent = __work_part_.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
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


        private void EditDynamic()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

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
            }
        }


        private void EditMove()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Move");

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
        }

        private bool EditMoveWork(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                NXMessage("Mirrored Component");
                return isBlockComponent;
            }

            _updateComponent = editComponent;

            BasePart.Units assmUnits = __display_part_.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return isBlockComponent;

            isBlockComponent = IsBlockComponent(isBlockComponent, editComponent);

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
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


        private bool EditMoveDisplay(bool isBlockComponent, Component editComponent)
        {
            if (__display_part_.FullPath.Contains("mirror"))
            {
                NXMessage("Mirrored Component");
                return isBlockComponent;
            }

            isBlockComponent = __work_part_.__HasDynamicBlock();

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
                NXMessage( "Not a block component");
                return isBlockComponent;
            }

            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = false;

            pHandle = NewMethod(pHandle);

            EnableForm();

            return isBlockComponent;
        }

        private bool EditSizeWork(bool isBlockComponent, Component editComponent)
        {
            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return isBlockComponent;

            isBlockComponent = NewMethod51(isBlockComponent, editComponent);

            if (isBlockComponent)
            {
                UpdateDynamicBlock(editComponent);
                CreateEditData(editComponent);
                DisableForm();
                var pHandle = SelectHandlePoint();
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

                        if (eLine.Name == "YBASE1")
                            blockWidth = eLine.GetLength();

                        if (eLine.Name == "ZBASE1")
                            blockHeight = eLine.GetLength();
                    }

                    Point pointPrototype = _udoPointHandle.IsOccurrence
                        ? (Point)_udoPointHandle.Prototype
                        : _udoPointHandle;

                    MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf,
                        out var movePtsFull, pointPrototype.Name.Contains("POS"));
                    GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs,
                        out var negZObjs);
                    AskAxisLines(out List<Line> allxAxisLines, out List<Line> allyAxisLines, out List<Line> allzAxisLines);

                    EditSizeForm sizeForm = null;
                    double convertLength = blockLength / 25.4;
                    double convertWidth = blockWidth / 25.4;
                    double convertHeight = blockHeight / 25.4;
                    sizeForm = ShowEditSizeFormDialog(blockLength, blockWidth, blockHeight, pointPrototype, sizeForm,
                        convertLength, convertWidth, convertHeight);

                    if (sizeForm.DialogResult == DialogResult.OK)
                    {
                        double editSize = sizeForm.InputValue;

                        if (__display_part_.PartUnits == BasePart.Units.Millimeters)
                            editSize *= 25.4;

                        if (editSize > 0)
                            switch (pointPrototype.Name)
                            {
                                case "POSX":
                                    movePtsFull.AddRange(posXObjs);
                                    EditSizeOrAlign(editSize - blockLength, movePtsHalf, movePtsFull, allxAxisLines,
                                        "X", true);
                                    break;
                                case "NEGX":
                                    movePtsFull.AddRange(negXObjs);
                                    EditSizeOrAlign(blockLength - editSize, movePtsHalf, movePtsFull, allxAxisLines,
                                        "X", false);
                                    break;
                                case "POSY":
                                    movePtsFull.AddRange(posYObjs);
                                    EditSizeOrAlign(editSize - blockWidth, movePtsHalf, movePtsFull, allyAxisLines, "Y",
                                        true);
                                    break;
                                case "NEGY":
                                    movePtsFull.AddRange(negYObjs);
                                    EditSizeOrAlign(blockWidth - editSize, movePtsHalf, movePtsFull, allyAxisLines, "Y",
                                        false);
                                    break;
                                case "POSZ":
                                    movePtsFull.AddRange(posZObjs);
                                    EditSizeOrAlign(editSize - blockHeight, movePtsHalf, movePtsFull, allzAxisLines,
                                        "Z", true);
                                    break;
                                case "NEGZ":
                                    movePtsFull.AddRange(negZObjs);
                                    EditSizeOrAlign(blockHeight - editSize, movePtsHalf, movePtsFull, allzAxisLines,
                                        "Z", false);
                                    break;
                                default:
                                    throw new InvalidOperationException(pointPrototype.Name);
                            }
                    }

                    sizeForm.Close();
                    sizeForm.Dispose();
                    pHandle = UpdateCreateSelect(editComponent);
                }

                EnableForm();
            }
            else
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return isBlockComponent;
            }

            return isBlockComponent;
        }


        private void EditSize()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

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
        }

        private void EditSizeOrAlign(
            double distance,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allzAxisLines,
            string dir_xyz,
            bool isPosEnd
        )
        {
            foreach (Line zAxisLine in allzAxisLines)
                SetLineEndPoints(distance, zAxisLine, !isPosEnd, dir_xyz);

            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
        }

        private bool EditSize(bool isBlockComponent, Component editComponent)
        {
            if (!__work_part_.__HasDynamicBlock())
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
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

                    if (eLine.Name == "YBASE1")
                        blockWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1")
                        blockHeight = eLine.GetLength();
                }

                Point pointPrototype = _udoPointHandle.IsOccurrence
                    ? (Point)_udoPointHandle.Prototype
                    : _udoPointHandle;

                MotionCallbackDynamic1(
                    pointPrototype,
                    out var doNotMovePts,
                    out var movePtsHalf,
                    out var movePtsFull,
                    pointPrototype.Name.Contains("POS")
                );

                GetLines(
                    out var posXObjs,
                    out var negXObjs,
                    out var posYObjs,
                    out var negYObjs,
                    out var posZObjs,
                    out var negZObjs
                );
                AskAxisLines(out List<Line> allxAxisLines, out List<Line> allyAxisLines, out List<Line> allzAxisLines);

                EditSizeForm sizeForm = null;
                double convertLength = blockLength / 25.4;
                double convertWidth = blockWidth / 25.4;
                double convertHeight = blockHeight / 25.4;
                sizeForm = ShowEditSizeFormDialog(
                    blockLength,
                    blockWidth,
                    blockHeight,
                    pointPrototype,
                    sizeForm,
                    convertLength,
                    convertWidth,
                    convertHeight
                );

                if (sizeForm.DialogResult == DialogResult.OK)
                {
                    double editSize = sizeForm.InputValue;

                    if (__display_part_.PartUnits == BasePart.Units.Millimeters)
                        editSize *= 25.4;

                    if (editSize > 0)
                    {
                        switch (pointPrototype.Name)
                        {
                            case "POSX":
                                movePtsFull.AddRange(posXObjs);

                                EditSizeOrAlign(
                                    editSize - blockLength,
                                    movePtsHalf,
                                    movePtsFull,
                                    allxAxisLines,
                                    "X",
                                    true
                                );
                                break;
                            case "NEGX":
                                movePtsFull.AddRange(negXObjs);

                                EditSizeOrAlign(
                                    blockLength - editSize,
                                    movePtsHalf,
                                    movePtsFull,
                                    allxAxisLines,
                                    "X",
                                    false
                                );
                                break;
                            case "POSY":
                                movePtsFull.AddRange(posYObjs);

                                EditSizeOrAlign(
                                    editSize - blockWidth,
                                    movePtsHalf,
                                    movePtsFull,
                                    allyAxisLines,
                                    "Y",
                                    true
                                );
                                break;
                            case "NEGY":
                                movePtsFull.AddRange(negYObjs);

                                EditSizeOrAlign(
                                    blockWidth - editSize,
                                    movePtsHalf,
                                    movePtsFull,
                                    allyAxisLines,
                                    "Y",
                                    false
                                );
                                break;
                            case "POSZ":
                                movePtsFull.AddRange(posZObjs);

                                EditSizeOrAlign(
                                    editSize - blockHeight,
                                    movePtsHalf,
                                    movePtsFull,
                                    allzAxisLines,
                                    "Z",
                                    true
                                );
                                break;
                            case "NEGZ":
                                movePtsFull.AddRange(negZObjs);

                                EditSizeOrAlign(
                                    blockHeight - editSize,
                                    movePtsHalf,
                                    movePtsFull,
                                    allzAxisLines,
                                    "Z",
                                    false
                                );
                                break;
                        }
                    }
                }

                UpdateDynamicBlock(editComponent);
                sizeForm.Close();
                sizeForm.Dispose();
                __work_component_ = editComponent;
                CreateEditData(editComponent);
                pHandle = SelectHandlePoint();
            }

            EnableForm();
            return isBlockComponent;
        }

        private void ResetNonBlockError()
        {
            EnableForm();
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            ufsession_.Disp.RegenerateDisplay();
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            __work_part_ = __display_part_;
        }

        private void DisableForm()
        {
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

        #endregion


        #region udo methods

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


        private void UpdateDynamicBlock(Component updateComp)
        {
            using (session_.__UsingDisplayPartReset())
                try
                {
                    // set component translucency and update dynamic block

                    if (updateComp != null)
                        updateComp.__Translucency(0);
                    else
                        foreach (Body dispBody in __work_part_.Bodies)
                            if (dispBody.Layer == 1)
                                dispBody.__Translucency(0);

                    Point3d blkOrigin = new Point3d();
                    string length = "";
                    string width = "";
                    string height = "";

                    foreach (Point pPoint in __work_part_.Points)
                    {
                        if (pPoint.Name != "BLKORIGIN")
                            continue;

                        blkOrigin.X = pPoint.Coordinates.X;
                        blkOrigin.Y = pPoint.Coordinates.Y;
                        blkOrigin.Z = pPoint.Coordinates.Z;
                    }

                    foreach (Line blkLine in _edgeRepLines)
                    {
                        if (blkLine.Name == "XBASE1")
                            length = blkLine.GetLength().ToString();

                        if (blkLine.Name == "YBASE1")
                            width = blkLine.GetLength().ToString();

                        if (blkLine.Name == "ZBASE1")
                            height = blkLine.GetLength().ToString();
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

                    __display_part_.WCS.SetOriginAndMatrix(blkOrigin, _workCompOrientation);
                    _workCompOrigin = blkOrigin;

                    __work_component_ = updateComp;

                    double[] fromPoint = { blkOrigin.X, blkOrigin.Y, blkOrigin.Z };
                    double[] mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(
                        UF_CSYS_ROOT_COORDS,
                        fromPoint,
                        UF_CSYS_WORK_COORDS,
                        mappedPoint
                    );

                    blkOrigin.X = mappedPoint[0];
                    blkOrigin.Y = mappedPoint[1];
                    blkOrigin.Z = mappedPoint[2];

                    if (!__work_part_.__HasDynamicBlock())
                        return;

                    Block block2 = __work_part_.__DynamicBlock();
                    BlockFeatureBuilder builder = __work_part_.Features.CreateBlockFeatureBuilder(
                        block2
                    );

                    using (session_.__UsingBuilderDestroyer(builder))
                    {
                        // builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                        // Body[] targetBodies1 = new Body[1];
                        // Body nullBody = null;
                        // targetBodies1[0] = nullBody;
                        // builder.BooleanOption.SetTargetBodies(targetBodies1);
                        // builder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                        Point blkFeatBuilderPoint = __work_part_.Points.CreatePoint(blkOrigin);
                        blkFeatBuilderPoint.SetCoordinates(blkOrigin);
                        builder.OriginPoint = blkFeatBuilderPoint;
                        Point3d originPoint1 = blkOrigin;
                        builder.SetOriginAndLengths(originPoint1, length, width, height);
                        // builder.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);
                        // Feature feature1;
                        builder.CommitFeature();
                    }

                    __work_part_ = __display_part_;
                    DeleteHandles();
                    __display_part_.WCS.Visibility = true;
                    __display_part_.Views.Refresh();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
        }


        private static void CreateUdo(
            UserDefinedObject myUdo,
            UserDefinedObject.LinkDefinition[] myLinks,
            double[] pointOnFace,
            Point point1, string name)
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


        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                UserDefinedClass myUdOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName(
                        "UdoDynamicHandle"
                    );

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo =
                    __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length != 0)
                    return;

                BasePart myBasePart = __work_part_;
                UserDefinedObjectManager myUdOmanager = myBasePart.UserDefinedObjectManager;

                foreach (Face blkFace in editBody.GetFaces())
                {
                    UserDefinedObject myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                    UserDefinedObject.LinkDefinition[] myLinks =
                        new UserDefinedObject.LinkDefinition[1];
                    double[] pointOnFace = new double[3];
                    double[] dir = new double[3];
                    double[] box = new double[6];
                    Matrix3x3 matrix1 = __display_part_.WCS.CoordinateSystem.Orientation.Element;
                    ufsession_.Modl.AskFaceData(
                        blkFace.Tag,
                        out int type,
                        pointOnFace,
                        dir,
                        box,
                        out double radius,
                        out double radData,
                        out int normDir
                    );
                    dir[0] = Math.Round(dir[0], 10);
                    dir[1] = Math.Round(dir[1], 10);
                    dir[2] = Math.Round(dir[2], 10);
                    double[] wcsVectorX =
                    {
                        Math.Round(matrix1.Xx, 10),
                        Math.Round(matrix1.Xy, 10),
                        Math.Round(matrix1.Xz, 10)
                    };
                    double[] wcsVectorY =
                    {
                        Math.Round(matrix1.Yx, 10),
                        Math.Round(matrix1.Yy, 10),
                        Math.Round(matrix1.Yz, 10)
                    };
                    double[] wcsVectorZ =
                    {
                        Math.Round(matrix1.Zx, 10),
                        Math.Round(matrix1.Zy, 10),
                        Math.Round(matrix1.Zz, 10)
                    };
                    double[] wcsVectorNegX = new double[3];
                    double[] wcsVectorNegY = new double[3];
                    double[] wcsVectorNegZ = new double[3];
                    ufsession_.Vec3.Negate(wcsVectorX, wcsVectorNegX);
                    ufsession_.Vec3.Negate(wcsVectorY, wcsVectorNegY);
                    ufsession_.Vec3.Negate(wcsVectorZ, wcsVectorNegZ);
                    // create udo handle points
                    ufsession_.Vec3.IsEqual(dir, wcsVectorX, 0.00, out int isEqualX);

                    if (isEqualX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorY, 0.00, out int isEqualY);

                    if (isEqualY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorZ, 0.00, out int isEqualZ);

                    if (isEqualZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSZ");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegX, 0.00, out int isEqualNegX);

                    if (isEqualNegX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegY, 0.00, out int isEqualNegY);

                    if (isEqualNegY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegZ, 0.00, out int isEqualNegZ);

                    if (isEqualNegZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGZ");
                }

                // create origin point
                CreatePointBlkOrigin();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void CreateUdo(
            UserDefinedObject myUdo,
            UserDefinedObject.LinkDefinition[] myLinks,
            double[] pointOnFace,
            string name
        )
        {
            Point point1 = CreatePoint(pointOnFace, name);
            CreateUdo(myUdo, myLinks, pointOnFace, point1, name);
        }

        private void HideDynamicHandles()
        {
            try
            {
                UserDefinedClass myUdOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName(
                        "UdoDynamicHandle"
                    );

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo =
                    __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (UserDefinedObject dispUdo in currentUdo)
                {
                    int[] setDisplay = { 0 };
                    dispUdo.SetIntegers(setDisplay);
                }

                foreach (Point namedPt in __work_part_.Points)
                    if (namedPt.Name != "")
                        namedPt.Blank();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private bool NewMethod51(bool isBlockComponent, Component editComponent)
        {
            if (_isNewSelection)
            {
                __work_component_ = editComponent;

                if (__work_part_.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;

            return isBlockComponent;
        }


        private void UpdateDynamicHandles()
        {
            UserDefinedClass myUdOclass =
                session_.UserDefinedClassManager.GetUserDefinedClassFromClassName(
                    "UdoDynamicHandle"
                );

            if (myUdOclass is null)
                return;

            UserDefinedObject[] currentUdo = __work_part_.UserDefinedObjectManager.GetUdosOfClass(
                myUdOclass
            );

            if (currentUdo.Length == 0)
                return;

            foreach (Point pointHandle in __work_part_.Points)
                foreach (UserDefinedObject udoHandle in currentUdo)
                    if (pointHandle.Name == udoHandle.Name)
                    {
                        udoHandle.SetDoubles(pointHandle.Coordinates.__ToArray());
                        udoHandle.SetIntegers(new[] { 0 });
                        pointHandle.Unblank();
                    }
        }

        private void ShowDynamicHandles()
        {
            try
            {
                UserDefinedClass myUdOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName(
                        "UdoDynamicHandle"
                    );

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo =
                    __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (UserDefinedObject dispUdo in currentUdo)
                {
                    int[] setDisplay = { 1 };
                    dispUdo.SetIntegers(setDisplay);
                    ufsession_.Disp.AddItemToDisplay(dispUdo.Tag);
                }

                foreach (Point udoPoint in __work_part_.Points)
                    if (udoPoint.Name != "" && udoPoint.Layer == __display_part_.Layers.WorkLayer)
                    {
                        udoPoint.SetVisibility(SmartObject.VisibilityOption.Visible);
                        udoPoint.RedisplayObject();
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private List<Point> UpdateCreateSelect(Component editComponent)
        {
            List<Point> pHandle;
            UpdateDynamicBlock(editComponent);
            __work_component_ = editComponent;
            CreateEditData(editComponent);
            pHandle = SelectHandlePoint();
            return pHandle;
        }


        #endregion

        #region Motion Methods

        private static void MotionCallbackDynamic1(
            Point pointPrototype,
            out List<NXObject> doNotMovePts,
            out List<NXObject> movePtsHalf,
            out List<NXObject> movePtsFull,
            bool isPos)
        {
            doNotMovePts = new List<NXObject>();
            movePtsFull = new List<NXObject>();
            movePtsHalf = new List<NXObject>();

            foreach (Point namedPt in __work_part_.Points)
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


        private void MotionCallbackNotDynamic(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor,
            int index, string dir_xyz)
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
            SetModelingView();
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

                if (__display_part_.PartUnits == BasePart.Units.Inches)
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


            MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf, out var movePtsFull,
                pointPrototype.Name.Contains("POS"));

            GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs,
                out var negZObjs);

            AskAxisLines(out var allxAxisLines, out var allyAxisLines, out var allzAxisLines);

            // get the distance from the selected point to the cursor location

            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            switch (pointPrototype.Name)
            {
                case "POSX":
                    movePtsFull.AddRange(posXObjs);
                    MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, allxAxisLines, mappedPoint,
                        mappedCursor);
                    break;
                case "NEGX":
                    movePtsFull.AddRange(negXObjs);
                    MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, allxAxisLines, mappedPoint,
                        mappedCursor);
                    break;
                case "POSY":
                    movePtsFull.AddRange(posYObjs);
                    MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, allyAxisLines, mappedPoint,
                        mappedCursor, 1);
                    break;
                case "NEGY":
                    movePtsFull.AddRange(negYObjs);
                    MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, allyAxisLines, mappedPoint,
                        mappedCursor, 1);
                    break;
                case "POSZ":
                    movePtsFull.AddRange(posZObjs);
                    MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, allzAxisLines, mappedPoint,
                        mappedCursor);
                    break;
                case "NEGZ":
                    movePtsFull.AddRange(negZObjs);
                    MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, allzAxisLines, mappedPoint,
                        mappedCursor);
                    break;
            }
        }


        private void MotionCallbackXDynamic(
            Point pointPrototype,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
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

            EditSizeOrAlign(xDistance, movePtsHalf, movePtsFull, allxAxisLines, "X", pointPrototype.Name == "POSX");
        }

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> allzAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] == mappedCursor[2])
                return;

            double zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

            if (zDistance < _gridSpace)
                return;

            if (mappedCursor[2] < mappedPoint[2])
                zDistance *= -1;

            _distanceMoved += zDistance;
            EditSizeOrAlign(zDistance, movePtsHalf, movePtsFull, allzAxisLines, "Z", pointPrototype.Name == "POSZ");
            SetModelingView();
        }



        private void MotionCallbackYDynamic(
            Point pointPrototype,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allyAxisLines,
            double[] mappedPoint,
            double[] mappedCursor,
            int index)
        {
            if (mappedPoint[index] == mappedCursor[index])
                return;

            double yDistance = Math.Sqrt(Math.Pow(mappedPoint[index] - mappedCursor[index], 2));

            if (yDistance < _gridSpace)
                return;

            if (mappedCursor[index] < mappedPoint[index])
                yDistance *= -1;

            _distanceMoved += yDistance;
            EditSizeOrAlign(yDistance, movePtsHalf, movePtsFull, allyAxisLines, "Y", pointPrototype.Name == "POSY");
        }

        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            List<NXObject> moveAll = new List<NXObject>();

            foreach (Point namedPts in __work_part_.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            moveAll.AddRange(_edgeRepLines);
            // get the distance from the selected point to the cursor location
            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 0, "X");

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 1, "Y");

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 2, "Z");
        }

        #endregion

        #region Miscellaneous Methods

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


        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if (__display_part_.PartUnits == BasePart.Units.Inches)
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
                    comboBoxGridBlock.SelectedIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);
                    break;
                }
        }

        private void SetWorkPlaneOn()
        {
            try
            {
                WorkPlane workPlane1;
                workPlane1 = __display_part_.Preferences.Workplane;

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
                    NXMessage("WorkPlane is null.  Reset Modeling State");
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
                WorkPlane workPlane1;
                workPlane1 = __display_part_.Preferences.Workplane;

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
                    NXMessage("WorkPlane is null.  Reset Modeling State");
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

        private void SetLineEndPoints(double distance, Line line, bool isStart, string dir_xyz)
        {
            Point3d point = isStart
                ? line.StartPoint
                : line.EndPoint;

            Point3d mappedPoint = MapAbsoluteToWcs(point);
            Point3d add;

            switch (dir_xyz)
            {
                case "X":
                    add = mappedPoint.__AddX(distance);
                    break;
                case "Y":
                    add = mappedPoint.__AddY(distance);
                    break;
                case "Z":
                    add = mappedPoint.__AddZ(distance);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            Point3d mappedAddX = MapWcsToAbsolute(add);

            if (isStart)

                line.SetStartPoint(mappedAddX);
            else
                line.SetEndPoint(mappedAddX);
        }

        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if (spacing == 0)
                return cursor;

            return __display_part_.PartUnits == BasePart.Units.Inches
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


        private static void GetLines(
            out List<Line> posXObjs, out List<Line> negXObjs,
            out List<Line> posYObjs, out List<Line> negYObjs,
            out List<Line> posZObjs, out List<Line> negZObjs)
        {
            posXObjs = new List<Line>();
            negXObjs = new List<Line>();
            posYObjs = new List<Line>();
            negYObjs = new List<Line>();
            posZObjs = new List<Line>();
            negZObjs = new List<Line>();

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


        private bool SetDispUnits()
        {
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            Part.Units dispUnits = (Part.Units)__display_part_.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }


        private void EditBlock()
        {
            if (Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;

            buttonApply.Enabled = false;

            LoadGridSizes();

            if (string.IsNullOrEmpty(comboBoxGridBlock.Text))
                if (!(Session.GetSession().Parts.Work is null))
                    comboBoxGridBlock.SelectedItem = __work_part_.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");
            _registered = Startup();
        }

        private void SelectGrid()
        {
            if (comboBoxGridBlock.Text == "0.000")
            {
                _ = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                _ = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }

            Settings.Default.EditBlockFormGridIncrement = comboBoxGridBlock.Text;
            Settings.Default.Save();
        }

        private void EditMatch()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection)
                {
                    if (_updateComponent is null)
                        SelectWithFilter_("Select Component - Match From");
                    else
                    {
                        UpdateDynamicBlock(_updateComponent);
                        __display_part_.WCS.SetOriginAndMatrix(
                            _workCompOrigin,
                            _workCompOrientation
                        );
                        __display_part_.WCS.Visibility = true;
                        _isNewSelection = true;
                    }
                }
                else
                {
                    UpdateDynamicBlock(_updateComponent);
                    __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                    __display_part_.WCS.Visibility = true;
                    _isNewSelection = true;
                }

                EditMatch(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            EnableForm();
        }


        private static void SelectWithFilter_(string message)
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter(message);
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private void ShowTemporarySizeText()
        {
            __display_part_.Views.Refresh();

            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name != "XBASE1" && eLine.Name != "YBASE1" && eLine.Name != "ZBASE1")
                    continue;

                string dim =
                    __display_part_.PartUnits == BasePart.Units.Inches
                        ? $"{Math.Round(eLine.GetLength(), 3):0.000}"
                        : $"{Math.Round(eLine.GetLength(), 3) / 25.4:0.000}";

                double[] midPoint = new double[3];
                UFObj.DispProps dispProps = new UFObj.DispProps { color = 31 };
                midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;
                double[] mappedPoint = new double[3];
                ufsession_.Csys.MapPoint(
                    UF_CSYS_WORK_COORDS,
                    midPoint,
                    UF_CSYS_ROOT_COORDS,
                    mappedPoint
                );

                ufsession_.Disp.DisplayTemporaryText(
                    __display_part_.Views.WorkView.Tag,
                    UFDisp.ViewType.UseWorkView,
                    dim,
                    mappedPoint,
                    UFDisp.TextRef.Middlecenter,
                    ref dispProps,
                    __display_part_.PartUnits == BasePart.Units.Inches ? .125 : 3.175,
                    1
                );
            }
        }

        private static void CreatePointBlkOrigin()
        {
            Point3d pointLocationOrigin = __display_part_.WCS.Origin;
            Point point1Origin = __work_part_.Points.CreatePoint(pointLocationOrigin);
            point1Origin.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1Origin.Blank();
            point1Origin.SetName("BLKORIGIN");
            point1Origin.Layer = __display_part_.Layers.WorkLayer;
            point1Origin.RedisplayObject();
        }

        private static void SetDispUnits(Part.Units dispUnits)
        {
            if (dispUnits == Part.Units.Millimeters)
            {
                __display_part_.UnitCollection.SetDefaultDataEntryUnits(
                    UnitCollection.UnitDefaults.GMmNDegC
                );
                __display_part_.UnitCollection.SetDefaultObjectInformationUnits(
                    UnitCollection.UnitDefaults.GMmNDegC
                );
            }
            else
            {
                __display_part_.UnitCollection.SetDefaultDataEntryUnits(
                    UnitCollection.UnitDefaults.LbmInLbfDegF
                );
                __display_part_.UnitCollection.SetDefaultObjectInformationUnits(
                    UnitCollection.UnitDefaults.LbmInLbfDegF
                );
            }
        }

        private static Point CreatePoint(double[] pointOnFace, string name)
        {
            Point3d pointLocation = pointOnFace.__ToPoint3d();
            Point point1 = __work_part_.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName(name);
            point1.Layer = __display_part_.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private void MoveObjects(NXObject[] objsToMove, double distance, string deltaXyz)
        {
            try
            {
                if (distance == 0)
                    return;

                __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                MoveObjectBuilder builder = __work_part_.BaseFeatures.CreateMoveObjectBuilder(null);

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    builder.TransformMotion.DistanceAngle.OrientXpress.AxisOption =
                        OrientXpressBuilder.Axis.Passive;
                    builder.TransformMotion.DistanceAngle.OrientXpress.PlaneOption =
                        OrientXpressBuilder.Plane.Passive;
                    builder.TransformMotion.OrientXpress.AxisOption = OrientXpressBuilder
                        .Axis
                        .Passive;
                    builder.TransformMotion.OrientXpress.PlaneOption = OrientXpressBuilder
                        .Plane
                        .Passive;
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

        private static double MapAndConvert(
            double inputDist,
            double[] mappedBase,
            double[] mappedPoint,
            int index
        )
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

        private void MoveObjects(
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            double xDistance,
            string dir_xyz,
            bool showTemporary
        )
        {
            if (!(dir_xyz == "X" || dir_xyz == "Y" || dir_xyz == "Z"))
                throw new ArgumentException($"Invalid direction '{dir_xyz}'");

            MoveObjects(movePtsFull.ToArray(), xDistance, dir_xyz);
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, dir_xyz);

            if (showTemporary)
                ShowTemporarySizeText();
        }

        private static EditSizeForm ShowEditSizeFormDialog(
         double blockLength,
         double blockWidth,
         double blockHeight,
         Point pointPrototype,
         EditSizeForm sizeForm,
         double convertLength,
         double convertWidth,
         double convertHeight
     )
        {
            if (__display_part_.PartUnits == BasePart.Units.Inches)
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
            }
        }

        private static void SetWcsDisplayPart()
        {
            _isUprParallel = false;
            _isLwrParallel = false;
            _parallelHeightExp = string.Empty;

            SetParallelExpressions();

            if (__work_part_.__HasDynamicBlock())
            {
                Block block1 = (Block)__work_part_.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatch;
                blockFeatureBuilderMatch = __work_part_.Features.CreateBlockFeatureBuilder(block1);
                Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                //string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                //string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                //string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                //double mLength = blockFeatureBuilderMatch.Length.Value;
                //double mWidth = blockFeatureBuilderMatch.Width.Value;
                //double mHeight = blockFeatureBuilderMatch.Height.Value;
                SetParallels();

                blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                double[] initMatrix = new double[9];
                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                CartesianCoordinateSystem setTempCsys = (CartesianCoordinateSystem)
                    NXObjectManager.Get(tempCsys);

                __display_part_.WCS.SetOriginAndMatrix(
                    setTempCsys.Origin,
                    setTempCsys.Orientation.Element
                );
                _workCompOrigin = setTempCsys.Origin;
                _workCompOrientation = setTempCsys.Orientation.Element;
            }
        }


        private void SetWcsWorkComponent(NXOpen.Assemblies.Component compRefCsys)
        {
            try
            {
                var _originalDisplayPart = __display_part_;

                if (compRefCsys != null)
                {
                    NXOpen.BasePart compBase = (NXOpen.BasePart)compRefCsys.Prototype;

                    session_.Parts.SetDisplay(
                        compBase,
                        false,
                        false,
                        out NXOpen.PartLoadStatus setDispLoadStatus
                    );
                    // setDispLoadStatus.Dispose();
                    // UpdateSessionParts();

                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;
                    _parallelWidthExp = string.Empty;

                    foreach (NXOpen.Expression exp in __work_part_.Expressions)
                    {
                        if (exp.Name == "uprParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                            {
                                _isUprParallel = true;
                            }
                            else
                            {
                                _isUprParallel = false;
                            }
                        }

                        if (exp.Name == "lwrParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                            {
                                _isLwrParallel = true;
                            }
                            else
                            {
                                _isLwrParallel = false;
                            }
                        }
                    }

                    foreach (NXOpen.Features.Feature featBlk in __work_part_.Features)
                    {
                        if (featBlk.FeatureType == "BLOCK")
                        {
                            if (featBlk.Name == "DYNAMIC BLOCK")
                            {
                                NXOpen.Features.Block block1 = (NXOpen.Features.Block)featBlk;

                                NXOpen.Features.BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch =
                                    __work_part_.Features.CreateBlockFeatureBuilder(block1);
                                NXOpen.Point3d bOrigin = blockFeatureBuilderMatch.Origin;
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

                                blockFeatureBuilderMatch.GetOrientation(
                                    out NXOpen.Vector3d xAxis,
                                    out NXOpen.Vector3d yAxis
                                );

                                double[] initOrigin = new double[]
                                {
                                    bOrigin.X,
                                    bOrigin.Y,
                                    bOrigin.Z
                                };
                                double[] xVector = new double[] { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = new double[] { yAxis.X, yAxis.Y, yAxis.Z };
                                double[] initMatrix = new double[9];
                                TheUFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                                TheUFSession.Csys.CreateMatrix(
                                    initMatrix,
                                    out NXOpen.Tag tempMatrix
                                );
                                TheUFSession.Csys.CreateTempCsys(
                                    initOrigin,
                                    tempMatrix,
                                    out NXOpen.Tag tempCsys
                                );
                                NXOpen.CartesianCoordinateSystem setTempCsys =
                                    (NXOpen.CartesianCoordinateSystem)
                                    NXOpen.Utilities.NXObjectManager.Get(tempCsys);

                                __display_part_.WCS.SetOriginAndMatrix(
                                    setTempCsys.Origin,
                                    setTempCsys.Orientation.Element
                                );

                                NXOpen.CartesianCoordinateSystem featBlkCsys =
                                    __display_part_.WCS.Save();
                                featBlkCsys.SetName("EDITCSYS");
                                featBlkCsys.Layer = 254;

                                NXOpen.NXObject[] addToBody = new NXOpen.NXObject[] { featBlkCsys };

                                foreach (
                                    NXOpen.ReferenceSet bRefSet in __display_part_.GetAllReferenceSets()
                                )
                                {
                                    if (bRefSet.Name == "BODY")
                                    {
                                        bRefSet.AddObjectsToReferenceSet(addToBody);
                                    }
                                }

                                session_.Parts.SetDisplay(
                                    _originalDisplayPart,
                                    false,
                                    false,
                                    out NXOpen.PartLoadStatus setDispLoadStatus1
                                );
                                setDispLoadStatus1.Dispose();
                                session_.Parts.SetWorkComponent(
                                    compRefCsys,
                                    out NXOpen.PartLoadStatus partLoadStatusWorkComp
                                );
                                partLoadStatusWorkComp.Dispose();
                                // UpdateSessionParts();

                                foreach (
                                    NXOpen.CartesianCoordinateSystem wpCsys in __work_part_.CoordinateSystems
                                )
                                {
                                    if (wpCsys.Layer == 254)
                                    {
                                        if (wpCsys.Name == "EDITCSYS")
                                        {
                                            NXOpen.NXObject csysOccurrence;
                                            csysOccurrence =
                                                session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                            NXOpen.CartesianCoordinateSystem editCsys =
                                                (NXOpen.CartesianCoordinateSystem)csysOccurrence;

                                            if (editCsys != null)
                                            {
                                                __display_part_.WCS.SetOriginAndMatrix(
                                                    editCsys.Origin,
                                                    editCsys.Orientation.Element
                                                );
                                                _workCompOrigin = editCsys.Origin;
                                                _workCompOrientation = editCsys.Orientation.Element;
                                            }

                                            NXOpen.Session.UndoMarkId markDeleteObjs;
                                            markDeleteObjs = session_.SetUndoMark(
                                                NXOpen.Session.MarkVisibility.Invisible,
                                                ""
                                            );

                                            session_.UpdateManager.AddToDeleteList(wpCsys);

                                            int errsDelObjs;
                                            errsDelObjs = session_.UpdateManager.DoUpdate(
                                                markDeleteObjs
                                            );

                                            session_.DeleteUndoMark(markDeleteObjs, "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;

                    foreach (NXOpen.Expression exp in __work_part_.Expressions)
                    {
                        if (exp.Name == "uprParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                            {
                                _isUprParallel = true;
                            }
                            else
                            {
                                _isUprParallel = false;
                            }
                        }

                        if (exp.Name == "lwrParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                            {
                                _isLwrParallel = true;
                            }
                            else
                            {
                                _isLwrParallel = false;
                            }
                        }
                    }

                    foreach (NXOpen.Features.Feature featBlk in __work_part_.Features)
                    {
                        if (featBlk.FeatureType == "BLOCK")
                        {
                            if (featBlk.Name == "DYNAMIC BLOCK")
                            {
                                NXOpen.Features.Block block1 = (NXOpen.Features.Block)featBlk;

                                NXOpen.Features.BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch =
                                    __work_part_.Features.CreateBlockFeatureBuilder(block1);
                                NXOpen.Point3d bOrigin = blockFeatureBuilderMatch.Origin;
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

                                blockFeatureBuilderMatch.GetOrientation(
                                    out NXOpen.Vector3d xAxis,
                                    out NXOpen.Vector3d yAxis
                                );

                                double[] initOrigin = new double[]
                                {
                                    bOrigin.X,
                                    bOrigin.Y,
                                    bOrigin.Z
                                };
                                double[] xVector = new double[] { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = new double[] { yAxis.X, yAxis.Y, yAxis.Z };
                                double[] initMatrix = new double[9];
                                TheUFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                                TheUFSession.Csys.CreateMatrix(
                                    initMatrix,
                                    out NXOpen.Tag tempMatrix
                                );
                                TheUFSession.Csys.CreateTempCsys(
                                    initOrigin,
                                    tempMatrix,
                                    out NXOpen.Tag tempCsys
                                );
                                NXOpen.CartesianCoordinateSystem setTempCsys =
                                    (NXOpen.CartesianCoordinateSystem)
                                    NXOpen.Utilities.NXObjectManager.Get(tempCsys);

                                __display_part_.WCS.SetOriginAndMatrix(
                                    setTempCsys.Origin,
                                    setTempCsys.Orientation.Element
                                );
                                _workCompOrigin = setTempCsys.Origin;
                                _workCompOrientation = setTempCsys.Orientation.Element;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private void CreateEditData(Component setCompTranslucency)
        {
            try
            {
                // set component translucency

                if (setCompTranslucency != null)
                    setCompTranslucency.__Translucency(75);
                else
                    foreach (Body dispBody in __work_part_.Bodies)
                        if (dispBody.Layer == 1)
                            dispBody.__Translucency(75);

                SetWcsToWorkPart(setCompTranslucency);

                if (!__work_part_.__HasDynamicBlock())
                    return;

                // get current block feature
                Block block1 = __work_part_.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatch;
                blockFeatureBuilderMatch = __work_part_.Features.CreateBlockFeatureBuilder(block1);
                Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                double mLength = blockFeatureBuilderMatch.Length.Value;
                double mWidth = blockFeatureBuilderMatch.Width.Value;
                double mHeight = blockFeatureBuilderMatch.Height.Value;

                __work_part_ = __display_part_;

                if (mLength == 0 || mWidth == 0 || mHeight == 0)
                    return;

                // create edit block feature
                Feature nullFeaturesFeature = null;
                BlockFeatureBuilder blockFeatureBuilderEdit;
                blockFeatureBuilderEdit = __display_part_.Features.CreateBlockFeatureBuilder(
                    nullFeaturesFeature
                );

                blockFeatureBuilderEdit.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                Body[] targetBodies1 = new Body[1];
                Body nullBody = null;
                targetBodies1[0] = nullBody;
                blockFeatureBuilderEdit.BooleanOption.SetTargetBodies(targetBodies1);

                blockFeatureBuilderEdit.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                Point3d originPoint1 = __display_part_.WCS.Origin;

                blockFeatureBuilderEdit.SetOriginAndLengths(
                    originPoint1,
                    mLength.ToString(),
                    mWidth.ToString(),
                    mHeight.ToString()
                );

                blockFeatureBuilderEdit.SetBooleanOperationAndTarget(
                    Feature.BooleanType.Create,
                    nullBody
                );

                Feature feature1;
                feature1 = blockFeatureBuilderEdit.CommitFeature();

                feature1.SetName("EDIT BLOCK");

                Block editBlock = (Block)feature1;
                Body[] editBody = editBlock.GetBodies();

                CreateBlockLines(originPoint1, mLength, mWidth, mHeight);

                blockFeatureBuilderMatch.Destroy();
                blockFeatureBuilderEdit.Destroy();

                __work_part_.FacetedBodies.DeleteTemporaryFacesAndEdges();

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
                ex.__PrintException();
            }
        }


        private void MoveComponent(Component compToMove)
        {
            try
            {
                if (compToMove is null)
                    return;

                if (compToMove.__Prototype().PartUnits != __display_part_.PartUnits)
                    return;

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


        private bool IsBlockComponent(bool isBlockComponent, Component editComponent)
        {
            if (_isNewSelection)
            {
                __work_component_ = editComponent;

                if (__work_part_.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;

            return isBlockComponent;
        }

        private static void AskAxisLines(
          out List<Line> allxAxisLines,
          out List<Line> allyAxisLines,
          out List<Line> allzAxisLines
      )
        {
            allxAxisLines = new List<Line>();
            allyAxisLines = new List<Line>();
            allzAxisLines = new List<Line>();
            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name.StartsWith("X"))
                    allxAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Y"))
                    allyAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Z"))
                    allzAxisLines.Add(eLine);
            }
        }



        private static void SetModelingView()
        {
            ModelingView mView = (ModelingView)__display_part_.Views.WorkView;
            __display_part_.Views.WorkView.Orient(mView.Matrix);
            __display_part_.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
        }


        private static void SetParallelExpressions()
        {
            foreach (Expression exp in __work_part_.Expressions)
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
        }

        private static void SetParallels()
        {
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
                __display_part_.WCS.Visibility = false;
                ModelingView mView = (ModelingView)__display_part_.Views.WorkView;
                __display_part_.Views.WorkView.Orient(mView.Matrix);
                __display_part_.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
                ufsession_.Ui.SpecifyScreenPosition(
                    message,
                    MotionCallback,
                    motionCbData,
                    screenPos,
                    out viewTag,
                    out int response
                );

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
                __display_part_.WCS.Visibility = false;
                SetModelingView();
                viewTag = NewMethod47(ref pHandle, message, screenPos, motionCbData);
            }

            return pHandle;
        }


        private Tag NewMethod47(
          ref List<Point> pHandle,
          string message,
          double[] screenPos,
          IntPtr motionCbData
      )
        {
            ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

            ufsession_.Ui.SpecifyScreenPosition(
                message,
                MotionCallback,
                motionCbData,
                screenPos,
                out Tag viewTag,
                out int response
            );

            if (response == UF_UI_PICK_RESPONSE)
            {
                UpdateDynamicHandles();
                ShowDynamicHandles();
                pHandle = SelectHandlePoint();
            }

            ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            return viewTag;
        }








        private List<Point> NewMethod4(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                __display_part_.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                viewTag = NewMethod49(ref pHandle, message, screenPos, motionCbData);
            }

            return pHandle;
        }

        private Tag NewMethod49(
            ref List<Point> pHandle,
            string message,
            double[] screenPos,
            IntPtr motionCbData
        )
        {
            ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
            SetModelingView();
            ufsession_.Ui.SpecifyScreenPosition(
                message,
                MotionCallback,
                motionCbData,
                screenPos,
                out Tag viewTag,
                out int response
            );

            if (response == UF_UI_PICK_RESPONSE)
            {
                UpdateDynamicHandles();
                ShowDynamicHandles();
                ShowTemporarySizeText();
                pHandle = SelectHandlePoint();
            }

            ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            return viewTag;
        }




        private List<Point> NewMethod(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                __display_part_.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;
                viewTag = NewMethod53(ref pHandle, message, screenPos, motionCbData);
            }

            return pHandle;
        }

        private Tag NewMethod53(
            ref List<Point> pHandle,
            string message,
            double[] screenPos,
            IntPtr motionCbData
        )
        {
            ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
            SetModelingView();
            ufsession_.Ui.SpecifyScreenPosition(
                message,
                MotionCallback,
                motionCbData,
                screenPos,
                out Tag viewTag,
                out int response
            );

            if (response == UF_UI_PICK_RESPONSE)
            {
                UpdateDynamicHandles();
                ShowDynamicHandles();
                ShowTemporarySizeText();
                pHandle = SelectHandlePoint();
            }

            ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            return viewTag;
        }



        private List<Point> NewMethod10(List<Point> pHandle)
        {
            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                __display_part_.WCS.Visibility = false;
                string message = "Select New Position";
                double[] screenPos = new double[3];
                Tag viewTag = NXOpen.Tag.Null;
                IntPtr motionCbData = IntPtr.Zero;
                IntPtr clientData = IntPtr.Zero;

                viewTag = NewMethod52(ref pHandle, message, screenPos, motionCbData);
            }

            return pHandle;
        }

        private Tag NewMethod52(
            ref List<Point> pHandle,
            string message,
            double[] screenPos,
            IntPtr motionCbData
        )
        {
            Tag viewTag;
            using (session_.__UsingLockUiFromCustom())
            {
                SetModelingView();
                ufsession_.Ui.SpecifyScreenPosition(
                    message,
                    MotionCallback,
                    motionCbData,
                    screenPos,
                    out viewTag,
                    out int response
                );

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }
            }

            return viewTag;
        }




        private static void NXMessage(string message)
        {
            TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, message);
        }


        #endregion
    }
}
// 4839