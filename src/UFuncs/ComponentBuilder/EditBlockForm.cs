using System;
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

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {

        #region variables
        private static Part _workPart = session_.Parts.Work;
        private static Part _displayPart = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = _displayPart;
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

        private void ButtonEditMove_Click(object sender, EventArgs e) => EditMove();

        private void ButtonEditConstruction_Click(object sender, EventArgs e) => EditConstruction();

        private void ButtonEndEditConstruction_Click(object sender, EventArgs e) => EndEditConstruction();

        private void EditBlockForm_Load(object sender, EventArgs e) => EditBlock();

        private void ComboBoxGridBlock_SelectedIndexChanged(object sender, EventArgs e) => SelectGrid();

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

        private void ButtonReset_Click(object sender, EventArgs e) => Reset();

        private void ButtonEditAlign_Click(object sender, EventArgs e) => EditAlign();

        private void ButtonViewWcs_Click(object sender, EventArgs e) => ViewWcs();

        private void ButtonEditMatch_Click(object sender, EventArgs e) => EditMatch();

        private void ButtonAlignComponent_Click(object sender, EventArgs e) => AlignComponent();

        private void ButtonEditDynamic_Click(object sender, EventArgs e) => EditDynamic();

        private void ButtonEditSize_Click(object sender, EventArgs e) => EditSize();

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e) => AlignEdgeDistance();

        #endregion



        #region form methods

        private void EditMove()
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

        public void EditConstruction()
        {
            buttonExit.Enabled = false;
            UpdateSessionParts();
            UpdateOriginalParts();

            Component editComponent = SelectOneComponent("Select Component to edit construction");

            if (editComponent is null)
                return;

            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
            {
                MessageBox.Show("Component units do not match the display part units");
                return;
            }

            using (session_.__UsingSuppressDisplay())
            {
                Part addRefSetPart = (Part)editComponent.Prototype;
                __display_part_ = addRefSetPart;
                UpdateSessionParts();

                using (session_.__UsingDoUpdate("Delete Reference Set"))
                {
                    ReferenceSet[] allRefSets = _displayPart.GetAllReferenceSets();

                    foreach (ReferenceSet namedRefSet in allRefSets)
                        if (namedRefSet.Name == "EDIT")
                            _workPart.DeleteReferenceSet(namedRefSet);
                }

                // create edit reference set
                using (session_.__UsingDoUpdate("Create New Reference Set"))
                {
                    ReferenceSet editRefSet = _workPart.CreateReferenceSet();
                    NXObject[] removeComps = editRefSet.AskAllDirectMembers();
                    editRefSet.RemoveObjectsFromReferenceSet(removeComps);
                    editRefSet.SetAddComponentsAutomatically(false, false);
                    editRefSet.SetName("EDIT");

                    // get all construction objects to add to reference set
                    List<NXObject> constructionObjects = new List<NXObject>();

                    for (int i = 1; i < 11; i++)
                    {
                        NXObject[] layerObjects = _displayPart.Layers.GetAllObjectsOnLayer(i);
                        foreach (NXObject addObj in layerObjects) constructionObjects.Add(addObj);
                    }

                    editRefSet.AddObjectsToReferenceSet(constructionObjects.ToArray());
                }

                __display_part_ = _originalDisplayPart;
                UpdateSessionParts();
            }
            __work_component_ = editComponent;
            UpdateSessionParts();
            SetWcsToWorkPart(editComponent);
            __work_component_.__Translucency(75);
            Component[] setRefComp = { editComponent };
            _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("EDIT", setRefComp);
            _displayPart.Layers.WorkLayer = 3;
            UpdateSessionParts();
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = true;
        }






        private void EndEditConstruction()
        {
            try
            {
                __work_component_.__Translucency(0);
                _displayPart.Layers.WorkLayer = 1;
                Session.UndoMarkId markId1;
                markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");
                Component[] setRefComp = { session_.Parts.WorkComponent };
                _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", setRefComp);
                ReferenceSet[] allRefSets = _workPart.GetAllReferenceSets();

                foreach (ReferenceSet namedRefSet in allRefSets)
                    if (namedRefSet.Name == "EDIT")
                        _workPart.DeleteReferenceSet(namedRefSet);

                int nErrs1;
                nErrs1 = session_.UpdateManager.DoUpdate(markId1);
                session_.DeleteUndoMark(markId1, "Delete Reference Set");
                __display_part_ = _originalDisplayPart;
                __work_part_ = _originalWorkPart;
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                buttonExit.Enabled = true;
                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                ex.__PrintException();
            }
        }




        private void EditBlock()
        {
            if (Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;

            buttonApply.Enabled = false;

            LoadGridSizes();

            if (string.IsNullOrEmpty(comboBoxGridBlock.Text))
                if (!(Session.GetSession().Parts.Work is null))
                    comboBoxGridBlock.SelectedItem = Session.GetSession().Parts.Work.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");
            _registered = Startup();
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



        private void SelectGrid()
        {
            if (comboBoxGridBlock.Text == "0.000")
            {
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }

            Settings.Default.EditBlockFormGridIncrement = comboBoxGridBlock.Text;
            Settings.Default.Save();
        }


        private void AlignComponent()
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


        private void EditDynamic()
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



        private void EditMatch()
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




        private void EditSize()
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



        private void EditAlign()
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



        private void AlignEdgeDistance()
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




        private void Apply()
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



        private void Reset()
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

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    TheUISession.NXMessageBox.Show(
                        "Error",
                        NXMessageBox.DialogType.Information,
                        "This function is not allowed in this context");

                    return;
                }

                Part checkPartName = (Part)editComponent.Prototype;


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

                if (!isBlockComponent)
                {
                    ResetNonBlockError();

                    TheUISession.NXMessageBox.Show(
                        "Caught exception",
                        NXMessageBox.DialogType.Error,
                        "Not a block component");

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

                    List<NXObject> doNotMovePts = new List<NXObject>();
                    List<NXObject> movePtsHalf = new List<NXObject>();
                    List<NXObject> movePtsFull = new List<NXObject>();
                    MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));
                    List<Line> posXObjs = new List<Line>();
                    List<Line> negXObjs = new List<Line>();
                    List<Line> posYObjs = new List<Line>();
                    List<Line> negYObjs = new List<Line>();
                    List<Line> posZObjs = new List<Line>();
                    List<Line> negZObjs = new List<Line>();
                    NewMethod19(posXObjs, negXObjs, posYObjs, negYObjs, posZObjs, negZObjs);

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

                    string message = "Select Reference Point";
                    UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                    Tag selection = NXOpen.Tag.Null;
                    double[] basePoint = new double[3];

                    ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                    ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                        out int response);

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
                            out double inputDist);

                        if (isDistance)
                        {
                            double[] mappedBase = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                                UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                            double[] pPrototype =
                            {
                                            pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                            pointPrototype.Coordinates.Z
                                        };
                            double[] mappedPoint = new double[3];
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

            Component editComponent = _editBody.OwningComponent;

            if (editComponent is null)
            {
                Show();

                TheUISession.NXMessageBox.Show(
                    "Error",
                    NXMessageBox.DialogType.Information,
                    "This function is not allowed in this context");

                return;
            }

            Part checkPartName = (Part)editComponent.Prototype;


            _updateComponent = editComponent;

            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return;

            isBlockComponent = NewMethod20(isBlockComponent, editComponent);

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

            List<Point> pHandle = SelectHandlePoint();

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

                List<NXObject> doNotMovePts = new List<NXObject>();
                List<NXObject> movePtsHalf = new List<NXObject>();
                List<NXObject> movePtsFull = new List<NXObject>();

                MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                List<Line> posXObjs = new List<Line>();
                List<Line> negXObjs = new List<Line>();
                List<Line> posYObjs = new List<Line>();
                List<Line> negYObjs = new List<Line>();
                List<Line> posZObjs = new List<Line>();
                List<Line> negZObjs = new List<Line>();

                NewMethod18(posXObjs, negXObjs, posYObjs, negYObjs, posZObjs, negZObjs);

                List<Line> allxAxisLines = new List<Line>();
                List<Line> allyAxisLines = new List<Line>();
                List<Line> allzAxisLines = new List<Line>();

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                    if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                }

                string message = "Select Reference Point";
                UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                Tag selection = NXOpen.Tag.Null;
                double[] basePoint = new double[3];

                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                    out int response);

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                if (response == UF_UI_OK)
                {
                    double[] mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                        UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                    double[] pPrototype =
                    {
                                         pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                         pointPrototype.Coordinates.Z
                                     };
                    double[] mappedPoint = new double[3];
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

        private bool NewMethod20(bool isBlockComponent, Component editComponent)
        {
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

            return isBlockComponent;
        }


        #endregion


        #region udo methods

        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                UserDefinedClass myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length != 0)
                    return;

                BasePart myBasePart = _workPart;
                UserDefinedObjectManager myUdOmanager = myBasePart.UserDefinedObjectManager;

                foreach (Face blkFace in editBody.GetFaces())
                {
                    UserDefinedObject myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                    UserDefinedObject.LinkDefinition[] myLinks = new UserDefinedObject.LinkDefinition[1];

                    double[] pointOnFace = new double[3];
                    double[] dir = new double[3];
                    double[] box = new double[6];
                    Matrix3x3 matrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                    ufsession_.Modl.AskFaceData(blkFace.Tag, out int type, pointOnFace, dir, box,
                        out double radius, out double radData, out int normDir);

                    dir[0] = Math.Round(dir[0], 10);
                    dir[1] = Math.Round(dir[1], 10);
                    dir[2] = Math.Round(dir[2], 10);

                    double[] wcsVectorX = { Math.Round(matrix1.Xx, 10), Math.Round(matrix1.Xy, 10), Math.Round(matrix1.Xz, 10) };
                    double[] wcsVectorY = { Math.Round(matrix1.Yx, 10), Math.Round(matrix1.Yy, 10), Math.Round(matrix1.Yz, 10) };
                    double[] wcsVectorZ = { Math.Round(matrix1.Zx, 10), Math.Round(matrix1.Zy, 10), Math.Round(matrix1.Zz, 10) };

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


        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks,
            double[] pointOnFace, string name)
        {
            Point point1 = CreatePoint(pointOnFace, name);
            CreateUdo(myUdo, myLinks, pointOnFace, point1, name);
        }


        private void HideDynamicHandles()
        {
            try
            {
                UpdateSessionParts();
                UserDefinedClass myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (UserDefinedObject dispUdo in currentUdo)
                {
                    int[] setDisplay = { 0 };
                    dispUdo.SetIntegers(setDisplay);
                }

                foreach (Point namedPt in _workPart.Points)
                    if (namedPt.Name != "")
                        namedPt.Blank();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private bool EditSizeWork(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;


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

            if (isBlockComponent)
            {
                UpdateDynamicBlock(editComponent);
                CreateEditData(editComponent);
                DisableForm();
                List<Point> pHandle = new List<Point>();
                pHandle = SelectHandlePoint();
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

                    MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                    List<Line> posXObjs = new List<Line>();
                    List<Line> negXObjs = new List<Line>();
                    List<Line> posYObjs = new List<Line>();
                    List<Line> negYObjs = new List<Line>();
                    List<Line> posZObjs = new List<Line>();
                    List<Line> negZObjs = new List<Line>();
                    NewMethod12(posXObjs, negXObjs, posYObjs, negYObjs, posZObjs, negZObjs);

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
                    sizeForm = NewMethod17(blockLength, blockWidth, blockHeight, pointPrototype, sizeForm, convertLength, convertWidth, convertHeight);

                    if (sizeForm.DialogResult == DialogResult.OK)
                    {
                        double editSize = sizeForm.InputValue;
                        double distance = 0;

                        if (_displayPart.PartUnits == BasePart.Units.Millimeters)
                            editSize *= 25.4;

                        if (editSize > 0)
                            switch (pointPrototype.Name)
                            {
                                case "POSX":
                                    movePtsFull.AddRange(posXObjs);
                                    distance = EditSize(editSize - blockLength, movePtsHalf, movePtsFull, allxAxisLines, "X", true);
                                    break;
                                case "NEGX":
                                    movePtsFull.AddRange(negXObjs);
                                    distance = EditSize(blockLength - editSize, movePtsHalf, movePtsFull, allxAxisLines, "X", false);
                                    break;
                                case "POSY":
                                    movePtsFull.AddRange(posYObjs);
                                    distance = EditSize(editSize - blockWidth, movePtsHalf, movePtsFull, allyAxisLines, "Y", true);
                                    break;
                                case "NEGY":
                                    movePtsFull.AddRange(negYObjs);
                                    distance = EditSize(blockWidth - editSize, movePtsHalf, movePtsFull, allyAxisLines, "Y", false);
                                    break;
                                case "POSZ":
                                    movePtsFull.AddRange(posZObjs);
                                    distance = EditSize(editSize - blockHeight, movePtsHalf, movePtsFull, allzAxisLines, "Z", true);
                                    break;
                                case "NEGZ":
                                    movePtsFull.AddRange(negZObjs);
                                    distance = EditSize(blockHeight - editSize, movePtsHalf, movePtsFull, allzAxisLines, "Z", false);
                                    break;
                                default:
                                    throw new InvalidOperationException(pointPrototype.Name);
                            }
                    }

                    UpdateDynamicBlock(editComponent);
                    sizeForm.Close();
                    sizeForm.Dispose();
                    __work_component_ = editComponent;
                    UpdateSessionParts();
                    CreateEditData(editComponent);
                    pHandle = SelectHandlePoint();
                }

                EnableForm();
            }
            else
            {
                ResetNonBlockError();

                TheUISession.NXMessageBox.Show(
                    "Caught exception",
                    NXMessageBox.DialogType.Error,
                    "Not a block component");

                return isBlockComponent;
            }

            return isBlockComponent;
        }


        private void UpdateDynamicHandles()
        {
            UserDefinedClass myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if (myUdOclass is null)
                return;

            UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

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
                UpdateSessionParts();

                UserDefinedClass myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (UserDefinedObject dispUdo in currentUdo)
                {
                    int[] setDisplay = { 1 };
                    dispUdo.SetIntegers(setDisplay);
                    ufsession_.Disp.AddItemToDisplay(dispUdo.Tag);
                }

                foreach (Point udoPoint in _workPart.Points)
                    if (udoPoint.Name != "" && udoPoint.Layer == _displayPart.Layers.WorkLayer)
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



        private void AlignComponent(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            Component editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                Show();
                TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                    "This function is not allowed in this context");
                return;
            }

            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                Show();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return;
            }

            _updateComponent = editComponent;

            BasePart.Units assmUnits = _displayPart.PartUnits;
            BasePart compBase = (BasePart)editComponent.Prototype;
            BasePart.Units compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return;

            isBlockComponent = NewMethod21(isBlockComponent, editComponent);

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            List<Point> pHandle = new List<Point>();
            pHandle = SelectHandlePoint();

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

                List<NXObject> movePtsFull = new List<NXObject>();

                foreach (Point nPoint in _workPart.Points)
                    if (nPoint.Name.Contains("X") || nPoint.Name.Contains("Y") ||
                       nPoint.Name.Contains("Z") || nPoint.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(nPoint);

                foreach (Line nLine in _workPart.Lines)
                    if (nLine.Name.Contains("X") || nLine.Name.Contains("Y") ||
                       nLine.Name.Contains("Z"))
                        movePtsFull.Add(nLine);

                string message = "Select Reference Point";
                UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                Tag selection = NXOpen.Tag.Null;
                double[] basePoint = new double[3];

                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                    out int response);

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                if (response == UF_UI_OK)
                {
                    double[] mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                        UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                    double[] pPrototype =
                    {
                                                pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                                pointPrototype.Coordinates.Z
                                            };
                    double[] mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype,
                        UF_CSYS_ROOT_WCS_COORDS, mappedPoint);

                    double distance;

                    switch (pointPrototype.Name)
                    {
                        case "POSX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "NEGX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "POSY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "NEGY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "POSZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        case "NEGZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        default:
                            throw new InvalidOperationException();
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

        private bool NewMethod21(bool isBlockComponent, Component editComponent)
        {
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
            return isBlockComponent;
        }

        private void ViewWcs()
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            Matrix3x3 orientation = coordSystem.Orientation.Element;
            _displayPart.Views.WorkView.Orient(orientation);
        }


        #endregion

    }
}
// 4839