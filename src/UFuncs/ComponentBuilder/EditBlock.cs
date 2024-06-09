using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Preferences;
using NXOpen.UF;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using NXOpenUI;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using Part = NXOpen.Part;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {
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

        public EditBlockForm()
        {
            InitializeComponent();
        }

        private void EditBlockForm_Load(object sender, EventArgs e)
        {
            if(Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;


            buttonApply.Enabled = false;

            LoadGridSizes();


            if(string.IsNullOrEmpty(comboBoxGridBlock.Text))
                if(!(Session.GetSession().Parts.Work is null))
                    comboBoxGridBlock.SelectedItem = Session.GetSession().Parts.Work.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");

            _registered = Startup();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
            Close();
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();
            using (this)
            {
                new ComponentBuilder().Show();
            }
        }

        private void ComboBoxGridBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxGridBlock.Text == "0.000")
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

        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if(_displayPart.PartUnits == BasePart.Units.Inches)
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
                if(gridSetting == Settings.Default.EditBlockFormGridIncrement)
                {
                    var gridIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);

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

                if(workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    var gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
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

                if(workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    var gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
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
            if(_registered == 0)
            {
                var editForm = this;
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

        private void ButtonViewWcs_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            var orientation = coordSystem.Orientation.Element;

            _displayPart.Views.WorkView.Orient(orientation);
        }

        private void ButtonEditConstruction_Click(object sender, EventArgs e)
        {
            try
            {
                buttonExit.Enabled = false;

                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                UpdateSessionParts();
                UpdateOriginalParts();

                var editComponent = SelectOneComponent("Select Component to edit construction");

                if(editComponent != null)
                {
                    var assmUnits = _displayPart.PartUnits;
                    var compBase = (BasePart)editComponent.Prototype;
                    var compUnits = compBase.PartUnits;

                    if(compUnits == assmUnits)
                    {
                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                        var addRefSetPart = (Part)editComponent.Prototype;

                        session_.Parts.SetDisplay(addRefSetPart, false, false, out var partLoadSetDisp1);
                        partLoadSetDisp1.Dispose();
                        UpdateSessionParts();

                        Session.UndoMarkId markId1;
                        markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");

                        var allRefSets = _displayPart.GetAllReferenceSets();

                        foreach (var namedRefSet in allRefSets)
                            if(namedRefSet.Name == "EDIT")
                                _workPart.DeleteReferenceSet(namedRefSet);

                        int nErrs1;
                        nErrs1 = session_.UpdateManager.DoUpdate(markId1);

                        session_.DeleteUndoMark(markId1, "Delete Reference Set");

                        // create edit reference set

                        Session.UndoMarkId markIdEditRefSet;
                        markIdEditRefSet =
                            session_.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Reference Set");

                        var editRefSet = _workPart.CreateReferenceSet();
                        var removeComps = editRefSet.AskAllDirectMembers();
                        editRefSet.RemoveObjectsFromReferenceSet(removeComps);
                        editRefSet.SetAddComponentsAutomatically(false, false);
                        editRefSet.SetName("EDIT");

                        // get all construction objects to add to reference set

                        var constructionObjects = new List<NXObject>();

                        for (var i = 1; i < 11; i++)
                        {
                            var layerObjects = _displayPart.Layers.GetAllObjectsOnLayer(i);

                            foreach (var addObj in layerObjects) constructionObjects.Add(addObj);
                        }

                        editRefSet.AddObjectsToReferenceSet(constructionObjects.ToArray());

                        int nErrs2;
                        nErrs2 = session_.UpdateManager.DoUpdate(markIdEditRefSet);

                        session_.DeleteUndoMark(markIdEditRefSet, "Create New Reference Set");

                        session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadSetDisp2);
                        partLoadSetDisp2.Dispose();
                        UpdateSessionParts();

                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork((Part)editComponent.Prototype);
                        session_.Parts.SetWorkComponent(editComponent, PartCollection.RefsetOption.Current,
                            PartCollection.WorkComponentOption.Visible, out var partLoadStatus3);
                        partLoadStatus3.Dispose();
                        UpdateSessionParts();

                        SetWcsToWorkPart(editComponent);

                        DisplayModification editObjectDisplay;
                        editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                        editObjectDisplay.ApplyToAllFaces = true;
                        editObjectDisplay.NewTranslucency = 75;
                        DisplayableObject[] compObject = { session_.Parts.WorkComponent };
                        editObjectDisplay.Apply(compObject);
                        editObjectDisplay.Dispose();

                        session_.Parts.WorkComponent.RedisplayObject();

                        Component[] setRefComp = { editComponent };
                        _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("EDIT", setRefComp);

                        _displayPart.Layers.WorkLayer = 3;

                        UpdateSessionParts();

                        buttonEditConstruction.Enabled = false;
                        buttonEndEditConstruction.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Component units do not match the display part units");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonEndEditConstruction_Click(object sender, EventArgs e)
        {
            try
            {
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                DisplayModification editObjectDisplay;
                editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                editObjectDisplay.ApplyToAllFaces = true;
                editObjectDisplay.NewTranslucency = 0;
                DisplayableObject[] compObject = { session_.Parts.WorkComponent };
                editObjectDisplay.Apply(compObject);
                editObjectDisplay.Dispose();

                session_.Parts.WorkComponent.RedisplayObject();

                _displayPart.Layers.WorkLayer = 1;

                Session.UndoMarkId markId1;
                markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");

                Component[] setRefComp = { session_.Parts.WorkComponent };
                _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", setRefComp);

                var allRefSets = _workPart.GetAllReferenceSets();

                foreach (var namedRefSet in allRefSets)
                    if(namedRefSet.Name == "EDIT")
                        _workPart.DeleteReferenceSet(namedRefSet);

                int nErrs1;
                nErrs1 = session_.UpdateManager.DoUpdate(markId1);

                session_.DeleteUndoMark(markId1, "Delete Reference Set");

                session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus1);
                session_.Parts.SetWork(_originalWorkPart);

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

        private void ButtonEditDynamic_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId undoDynamic = session_.SetUndoMark(Session.MarkVisibility.Visible, "Dynamic Block Edit");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                //bool areUnitsEqual = false;
                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;

                //if (infoUnits == UF_UI_POUNDS_INCHES && dispUnits == NXOpen.Part.Units.Inches || infoUnits == UF_UI_KILOS_MILLIMETERS && dispUnits == NXOpen.Part.Units.Millimeters)
                //{
                //    areUnitsEqual = true;
                //}

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component for Dynamic Edit");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody != null)
                {
                    var editComponent = _editBody.OwningComponent;

                    if(editComponent != null)
                    {
                        var checkPartName = (Part)editComponent.Prototype;

                        if(!checkPartName.FullPath.Contains("mirror"))
                        {
                            _updateComponent = editComponent;

                            var assmUnits = _displayPart.PartUnits;
                            var compBase = (BasePart)editComponent.Prototype;
                            var compUnits = compBase.PartUnits;

                            if(compUnits == assmUnits)
                            {
                                if(_isNewSelection)
                                {
                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                    session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                    partLoadStatusWorkComp.Dispose();
                                    UpdateSessionParts();

                                    foreach (Feature featBlk in _workPart.Features)
                                        if(featBlk.FeatureType == "BLOCK")
                                            if(featBlk.Name == "DYNAMIC BLOCK")
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

                                if(isBlockComponent)
                                {
                                    DisableForm();

                                    var pHandle = new List<Point>();
                                    pHandle = SelectHandlePoint();

                                    _isDynamic = true;

                                    while (pHandle.Count == 1)
                                    {
                                        _distanceMoved = 0;

                                        HideDynamicHandles();

                                        _udoPointHandle = pHandle[0];

                                        var message = "Select New Position";
                                        var screenPos = new double[3];
                                        var viewTag = NXOpen.Tag.Null;
                                        var motionCbData = IntPtr.Zero;
                                        var clientData = IntPtr.Zero;

                                        _displayPart.WCS.Visibility = false;

                                        var mView = (ModelingView)_displayPart.Views.WorkView;
                                        _displayPart.Views.WorkView.Orient(mView.Matrix);
                                        _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);

                                        ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                        ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                                            screenPos, out viewTag, out var response);

                                        if(response == UF_UI_PICK_RESPONSE)
                                        {
                                            UpdateDynamicHandles();
                                            ShowDynamicHandles();
                                            pHandle = SelectHandlePoint();
                                        }

                                        ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
                                    }

                                    EnableForm();
                                }
                                else
                                {
                                    ResetNonBlockError();
                                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                        "Not a block component");
                                }
                            }
                        }
                        else
                        {
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Mirrored Component");
                        }
                    }
                    else
                    {
                        if(!_displayPart.FullPath.Contains("mirror"))
                        {
                            foreach (Feature featBlk in _workPart.Features)
                                if(featBlk.FeatureType == "BLOCK")
                                    if(featBlk.Name == "DYNAMIC BLOCK")
                                        isBlockComponent = true;

                            if(isBlockComponent)
                            {
                                DisableForm();


                                if(_isNewSelection)
                                {
                                    CreateEditData(editComponent);

                                    _isNewSelection = false;
                                }

                                var pHandle = new List<Point>();
                                pHandle = SelectHandlePoint();

                                _isDynamic = true;

                                while (pHandle.Count == 1)
                                {
                                    _distanceMoved = 0;

                                    HideDynamicHandles();

                                    _udoPointHandle = pHandle[0];

                                    var message = "Select New Position";
                                    var screenPos = new double[3];
                                    var viewTag = NXOpen.Tag.Null;
                                    var motionCbData = IntPtr.Zero;
                                    var clientData = IntPtr.Zero;

                                    _displayPart.WCS.Visibility = false;

                                    var mView = (ModelingView)_displayPart.Views.WorkView;
                                    _displayPart.Views.WorkView.Orient(mView.Matrix);
                                    _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);

                                    ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                    ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                                        screenPos, out viewTag, out var response);

                                    if(response == UF_UI_PICK_RESPONSE)
                                    {
                                        UpdateDynamicHandles();
                                        ShowDynamicHandles();
                                        pHandle = SelectHandlePoint();
                                    }

                                    ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
                                }

                                EnableForm();
                            }
                            else
                            {
                                ResetNonBlockError();
                                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                    "Not a block component");
                            }
                        }
                        else
                        {
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Mirrored Component");
                        }
                    }
                }
                //}
                //else
                //{
                //    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                //    ufsession_.Disp.RegenerateDisplay();
                //    TheUISession.NXMessageBox.Show("Caught exception : Dynamic Edit", NXOpen.NXMessageBox.DialogType.Error, "Analysis --> Units does not match the session units");
                //}
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

        private void ButtonEditMove_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                //bool areUnitsEqual = false;
                var isBlockComponent = false;

                //ufsession_.Ui.AskInfoUnits(out int infoUnits);

                //if (infoUnits == UF_UI_POUNDS_INCHES && dispUnits == NXOpen.Part.Units.Inches || infoUnits == UF_UI_KILOS_MILLIMETERS && dispUnits == NXOpen.Part.Units.Millimeters)
                //{
                //    areUnitsEqual = true;
                //}

                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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


                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component to Move");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody == null)
                    return;

                var editComponent = _editBody.OwningComponent;

                if(editComponent != null)
                {
                    var checkPartName = (Part)editComponent.Prototype;

                    if(!checkPartName.FullPath.Contains("mirror"))
                    {
                        _updateComponent = editComponent;

                        var assmUnits = _displayPart.PartUnits;
                        var compBase = (BasePart)editComponent.Prototype;
                        var compUnits = compBase.PartUnits;

                        if(compUnits == assmUnits)
                        {
                            if(_isNewSelection)
                            {
                                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                partLoadStatusWorkComp.Dispose();
                                UpdateSessionParts();

                                foreach (Feature featBlk in _workPart.Features)
                                    if(featBlk.FeatureType == "BLOCK")
                                        if(featBlk.Name == "DYNAMIC BLOCK")
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

                            if(isBlockComponent)
                            {
                                DisableForm();

                                var pHandle = new List<Point>();
                                pHandle = SelectHandlePoint();

                                _isDynamic = false;

                                while (pHandle.Count == 1)
                                {
                                    _distanceMoved = 0;

                                    HideDynamicHandles();

                                    _udoPointHandle = pHandle[0];

                                    _displayPart.WCS.Visibility = false;

                                    var message = "Select New Position";
                                    var screenPos = new double[3];
                                    var viewTag = NXOpen.Tag.Null;
                                    var motionCbData = IntPtr.Zero;
                                    var clientData = IntPtr.Zero;

                                    ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                    var mView = (ModelingView)_displayPart.Views.WorkView;
                                    _displayPart.Views.WorkView.Orient(mView.Matrix);

                                    _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);

                                    ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                                        screenPos, out viewTag, out var response);

                                    if(response == UF_UI_PICK_RESPONSE)
                                    {
                                        UpdateDynamicHandles();
                                        ShowDynamicHandles();
                                        ShowTemporarySizeText();
                                        pHandle = SelectHandlePoint();
                                    }

                                    ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
                                }

                                EnableForm();
                            }
                            else
                            {
                                ResetNonBlockError();
                                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                    "Not a block component");
                            }
                        }
                    }
                    else
                    {
                        TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                            "Mirrored Component");
                    }
                }
                else
                {
                    if(!_displayPart.FullPath.Contains("mirror"))
                    {
                        foreach (Feature featBlk in _workPart.Features)
                            if(featBlk.FeatureType == "BLOCK")
                                if(featBlk.Name == "DYNAMIC BLOCK")
                                    isBlockComponent = true;

                        if(isBlockComponent)
                        {
                            DisableForm();

                            if(_isNewSelection)
                            {
                                CreateEditData(editComponent);

                                _isNewSelection = false;
                            }
                        }

                        if(isBlockComponent)
                        {
                            var pHandle = new List<Point>();
                            pHandle = SelectHandlePoint();

                            _isDynamic = false;

                            while (pHandle.Count == 1)
                            {
                                _distanceMoved = 0;

                                HideDynamicHandles();

                                _udoPointHandle = pHandle[0];

                                _displayPart.WCS.Visibility = false;

                                var message = "Select New Position";
                                var screenPos = new double[3];
                                var viewTag = NXOpen.Tag.Null;
                                var motionCbData = IntPtr.Zero;
                                var clientData = IntPtr.Zero;

                                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                var mView = (ModelingView)_displayPart.Views.WorkView;
                                _displayPart.Views.WorkView.Orient(mView.Matrix);

                                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);

                                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                                    out viewTag, out var response);

                                if(response == UF_UI_PICK_RESPONSE)
                                {
                                    UpdateDynamicHandles();
                                    ShowDynamicHandles();
                                    ShowTemporarySizeText();
                                    pHandle = SelectHandlePoint();
                                }

                                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
                            }

                            EnableForm();
                        }
                        else
                        {
                            ResetNonBlockError();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Not a block component");
                        }
                    }
                    else
                    {
                        TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                            "Mirrored Component");
                    }
                }
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

        private void ButtonEditMatch_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId matchId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Match Block Edit");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                {
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component - Match From");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
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

                if(_editBody != null)
                {
                    var editComponent = _editBody.OwningComponent;

                    if(editComponent != null)
                    {
                        var checkPartName = (Part)editComponent.Prototype;

                        foreach (Feature featBlk in checkPartName.Features)
                            if(featBlk.FeatureType == "BLOCK")
                                if(featBlk.Name == "DYNAMIC BLOCK")
                                    isBlockComponent = true;

                        if(isBlockComponent)
                        {
                            DisableForm();
                            isBlockComponent = false;

                            if(!checkPartName.FullPath.Contains("mirror"))
                            {
                                _updateComponent = editComponent;

                                var assmUnits = _displayPart.PartUnits;
                                var compBase = (BasePart)editComponent.Prototype;
                                var compUnits = compBase.PartUnits;

                                if(compUnits == assmUnits)
                                {
                                    SelectWithFilter.NonValidCandidates = _nonValidNames;
                                    SelectWithFilter.GetSelectedWithFilter("Select Component - Match To");
                                    var editBodyTo = SelectWithFilter.SelectedCompBody;

                                    if(editBodyTo != null)
                                    {
                                        var matchComponent = editBodyTo.OwningComponent;

                                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                        session_.Parts.SetWorkComponent(matchComponent, out var partLoadMatch);
                                        partLoadMatch.Dispose();
                                        UpdateSessionParts();

                                        foreach (Feature featBlk in _workPart.Features)
                                            if(featBlk.FeatureType == "BLOCK")
                                                if(featBlk.Name == "DYNAMIC BLOCK")
                                                    isBlockComponent = true;

                                        if(isBlockComponent)
                                        {
                                            DisableForm();

                                            SetWcsToWorkPart(matchComponent);

                                            foreach (Feature featBlk in _workPart.Features)
                                                if(featBlk.Name == "DYNAMIC BLOCK")
                                                {
                                                    // get current block feature
                                                    var block1 = (Block)featBlk;

                                                    BlockFeatureBuilder blockFeatureBuilderMatchFrom;
                                                    blockFeatureBuilderMatchFrom =
                                                        _workPart.Features.CreateBlockFeatureBuilder(block1);
                                                    var blkOrigin = blockFeatureBuilderMatchFrom.Origin;
                                                    var length = blockFeatureBuilderMatchFrom.Length.RightHandSide;
                                                    var width = blockFeatureBuilderMatchFrom.Width.RightHandSide;
                                                    var height = blockFeatureBuilderMatchFrom.Height.RightHandSide;
                                                    blockFeatureBuilderMatchFrom.GetOrientation(out var xAxisMatch,
                                                        out var yAxisMatch);

                                                    session_.Parts.SetWork(_displayPart);
                                                    UpdateSessionParts();
                                                    var origin = new double[3];
                                                    var matrix = new double[9];
                                                    var transform = new double[4, 4];

                                                    ufsession_.Assem.AskComponentData(matchComponent.Tag,
                                                        out var partName, out var refSetName, out var instanceName,
                                                        origin, matrix, transform);

                                                    var eInstance =
                                                        ufsession_.Assem.AskInstOfPartOcc(editComponent.Tag);
                                                    ufsession_.Assem.RepositionInstance(eInstance, origin, matrix);

                                                    session_.Parts.SetWorkComponent(editComponent,
                                                        out var partLoadEditComponent);
                                                    partLoadEditComponent.Dispose();
                                                    UpdateSessionParts();

                                                    foreach (Feature featDynamic in _workPart.Features)
                                                        if(featDynamic.Name == "DYNAMIC BLOCK")
                                                        {
                                                            var block2 = (Block)featDynamic;

                                                            BlockFeatureBuilder blockFeatureBuilderMatchTo;
                                                            blockFeatureBuilderMatchTo =
                                                                _workPart.Features.CreateBlockFeatureBuilder(block2);

                                                            blockFeatureBuilderMatchTo.BooleanOption.Type =
                                                                BooleanOperation.BooleanType.Create;

                                                            var targetBodies1 = new Body[1];
                                                            Body nullBody = null;
                                                            targetBodies1[0] = nullBody;
                                                            blockFeatureBuilderMatchTo.BooleanOption.SetTargetBodies(
                                                                targetBodies1);

                                                            blockFeatureBuilderMatchTo.Type = BlockFeatureBuilder.Types
                                                                .OriginAndEdgeLengths;

                                                            var blkFeatBuilderPoint =
                                                                _workPart.Points.CreatePoint(blkOrigin);
                                                            blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                                                            blockFeatureBuilderMatchTo.OriginPoint =
                                                                blkFeatBuilderPoint;

                                                            var originPoint1 = blkOrigin;

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

                                                            session_.Parts.SetWork(_originalWorkPart);
                                                            UpdateSessionParts();

                                                            _displayPart.WCS.Visibility = true;
                                                            _displayPart.Views.Refresh();
                                                        }
                                                }

                                            MoveComponent(editComponent);

                                            EnableForm();
                                        }
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
                                        //buttonBlockFeature.Enabled = true;
                                        buttonReset.Enabled = true;
                                        buttonExit.Enabled = true;
                                    }
                                    else
                                    {
                                        ResetNonBlockError();
                                    }
                                }
                            }
                            else
                            {
                                EnableForm();
                                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                                    NXMessageBox.DialogType.Error, "Mirrored Component");
                            }
                        }
                        else
                        {
                            ResetNonBlockError();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Not a block component");
                        }
                    }
                    else
                    {
                        EnableForm();
                        TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                            NXMessageBox.DialogType.Information, "This function is not allowed in this context");
                    }
                }
                //}
                //else
                //{
                //    EnableForm();
                //    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                //    ufsession_.Disp.RegenerateDisplay();
                //    TheUISession.NXMessageBox.Show("Caught exception : Match Block", NXOpen.NXMessageBox.DialogType.Error, "Analysis --> Units does not match the session units");
                //}
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            EnableForm();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonEditSize_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId sizeId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Edit Block Size");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);

                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component to Edit Size");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody is null)
                    return;

                var editComponent = _editBody.OwningComponent;

                if(editComponent != null)
                {
                    var checkPartName = (Part)editComponent.Prototype;

                    if(!checkPartName.FullPath.Contains("mirror"))
                    {
                        _updateComponent = editComponent;

                        var assmUnits = _displayPart.PartUnits;
                        var compBase = (BasePart)editComponent.Prototype;
                        var compUnits = compBase.PartUnits;

                        if(compUnits == assmUnits)
                        {
                            if(_isNewSelection)
                            {
                                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                partLoadStatusWorkComp.Dispose();
                                UpdateSessionParts();

                                foreach (Feature featBlk in _workPart.Features)
                                    if(featBlk.FeatureType == "BLOCK")
                                        if(featBlk.Name == "DYNAMIC BLOCK")
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

                            if(isBlockComponent)
                            {
                                UpdateDynamicBlock(editComponent);
                                CreateEditData(editComponent);

                                DisableForm();

                                var pHandle = new List<Point>();
                                pHandle = SelectHandlePoint();

                                _isDynamic = true;

                                while (pHandle.Count == 1)
                                {
                                    HideDynamicHandles();

                                    _udoPointHandle = pHandle[0];

                                    var blockOrigin = new Point3d();
                                    var blockLength = 0.00;
                                    var blockWidth = 0.00;
                                    var blockHeight = 0.00;

                                    foreach (var eLine in _edgeRepLines)
                                    {
                                        if(eLine.Name == "XBASE1")
                                        {
                                            blockOrigin = eLine.StartPoint;
                                            blockLength = eLine.GetLength();
                                        }

                                        if(eLine.Name == "YBASE1") blockWidth = eLine.GetLength();

                                        if(eLine.Name == "ZBASE1") blockHeight = eLine.GetLength();
                                    }

                                    Point pointPrototype;

                                    if(_udoPointHandle.IsOccurrence)
                                        pointPrototype = (Point)_udoPointHandle.Prototype;
                                    else
                                        pointPrototype = _udoPointHandle;

                                    var doNotMovePts = new List<NXObject>();
                                    var movePtsHalf = new List<NXObject>();
                                    var movePtsFull = new List<NXObject>();

                                    if(pointPrototype.Name.Contains("POS"))
                                    {
                                        foreach (Point namedPt in _workPart.Points)
                                        {
                                            if(namedPt.Name == "")
                                                continue;

                                            if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                                doNotMovePts.Add(namedPt);
                                            else if(namedPt.Name.Contains("BLKORIGIN"))
                                                doNotMovePts.Add(namedPt);
                                            else
                                                movePtsHalf.Add(namedPt);
                                        }

                                        movePtsFull.Add(pointPrototype);
                                    }
                                    else
                                    {
                                        foreach (Point namedPt in _workPart.Points)
                                        {
                                            if(namedPt.Name == "")
                                                continue;

                                            if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                                doNotMovePts.Add(namedPt);
                                            else if(namedPt.Name.Contains("BLKORIGIN"))
                                                movePtsFull.Add(namedPt);
                                            else
                                                movePtsHalf.Add(namedPt);
                                        }

                                        movePtsFull.Add(pointPrototype);
                                    }

                                    var posXObjs = new List<Line>();
                                    var negXObjs = new List<Line>();
                                    var posYObjs = new List<Line>();
                                    var negYObjs = new List<Line>();
                                    var posZObjs = new List<Line>();
                                    var negZObjs = new List<Line>();

                                    foreach (var eLine in _edgeRepLines)
                                    {
                                        if(eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                        if(eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                                           eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                        if(eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                        if(eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                                           eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                        if(eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                                           eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                                        if(eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                                           eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                                    }

                                    var allxAxisLines = new List<Line>();
                                    var allyAxisLines = new List<Line>();
                                    var allzAxisLines = new List<Line>();

                                    foreach (var eLine in _edgeRepLines)
                                    {
                                        if(eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                                        if(eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                                        if(eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                                    }

                                    EditSizeForm sizeForm = null;

                                    var convertLength = blockLength / 25.4;
                                    var convertWidth = blockWidth / 25.4;
                                    var convertHeight = blockHeight / 25.4;

                                    if(_displayPart.PartUnits == BasePart.Units.Inches)
                                    {
                                        if(pointPrototype.Name.Contains("X"))
                                        {
                                            sizeForm = new EditSizeForm(blockLength);
                                            sizeForm.ShowDialog();
                                        }

                                        if(pointPrototype.Name.Contains("Y"))
                                        {
                                            sizeForm = new EditSizeForm(blockWidth);
                                            sizeForm.ShowDialog();
                                        }

                                        if(pointPrototype.Name.Contains("Z"))
                                        {
                                            sizeForm = new EditSizeForm(blockHeight);
                                            sizeForm.ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        if(pointPrototype.Name.Contains("X"))
                                        {
                                            sizeForm = new EditSizeForm(convertLength);
                                            sizeForm.ShowDialog();
                                        }

                                        if(pointPrototype.Name.Contains("Y"))
                                        {
                                            sizeForm = new EditSizeForm(convertWidth);
                                            sizeForm.ShowDialog();
                                        }

                                        if(pointPrototype.Name.Contains("Z"))
                                        {
                                            sizeForm = new EditSizeForm(convertHeight);
                                            sizeForm.ShowDialog();
                                        }
                                    }

                                    if(sizeForm.DialogResult == DialogResult.OK)
                                    {
                                        var editSize = sizeForm.InputValue;
                                        double distance = 0;

                                        if(_displayPart.PartUnits == BasePart.Units.Millimeters) editSize *= 25.4;

                                        if(editSize > 0)
                                        {
                                            if(pointPrototype.Name == "POSX")
                                            {
                                                distance = editSize - blockLength;

                                                foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                                foreach (var xAxisLine in allxAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                                    var addX = new Point3d(mappedEndPoint.X + distance,
                                                        mappedEndPoint.Y, mappedEndPoint.Z);
                                                    var mappedAddX = MapWcsToAbsolute(addX);
                                                    xAxisLine.SetEndPoint(mappedAddX);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                            }

                                            if(pointPrototype.Name == "NEGX")
                                            {
                                                distance = blockLength - editSize;

                                                foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

                                                foreach (var xAxisLine in allxAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                                                    var addX = new Point3d(mappedStartPoint.X + distance,
                                                        mappedStartPoint.Y, mappedStartPoint.Z);
                                                    var mappedAddX = MapWcsToAbsolute(addX);
                                                    xAxisLine.SetStartPoint(mappedAddX);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                            }

                                            if(pointPrototype.Name == "POSY")
                                            {
                                                distance = editSize - blockWidth;

                                                foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

                                                foreach (var yAxisLine in allyAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                                                    var addY = new Point3d(mappedEndPoint.X,
                                                        mappedEndPoint.Y + distance, mappedEndPoint.Z);
                                                    var mappedAddY = MapWcsToAbsolute(addY);
                                                    yAxisLine.SetEndPoint(mappedAddY);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                            }

                                            if(pointPrototype.Name == "NEGY")
                                            {
                                                distance = blockWidth - editSize;

                                                foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

                                                foreach (var yAxisLine in allyAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                                                    var addY = new Point3d(mappedStartPoint.X,
                                                        mappedStartPoint.Y + distance, mappedStartPoint.Z);
                                                    var mappedAddY = MapWcsToAbsolute(addY);
                                                    yAxisLine.SetStartPoint(mappedAddY);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                            }

                                            if(pointPrototype.Name == "POSZ")
                                            {
                                                distance = editSize - blockHeight;

                                                foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

                                                foreach (var zAxisLine in allzAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                                                    var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                                                        mappedEndPoint.Z + distance);
                                                    var mappedAddZ = MapWcsToAbsolute(addZ);
                                                    zAxisLine.SetEndPoint(mappedAddZ);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                            }

                                            if(pointPrototype.Name == "NEGZ")
                                            {
                                                distance = blockHeight - editSize;

                                                foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

                                                foreach (var zAxisLine in allzAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                                                    var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                                                        mappedStartPoint.Z + distance);
                                                    var mappedAddZ = MapWcsToAbsolute(addZ);
                                                    zAxisLine.SetStartPoint(mappedAddZ);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                            }
                                        }
                                    }

                                    UpdateDynamicBlock(editComponent);

                                    session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                                    session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                                    sizeForm.Close();
                                    sizeForm.Dispose();

                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                    session_.Parts.SetWorkComponent(editComponent, out var partLoadComponent1);
                                    partLoadComponent1.Dispose();
                                    UpdateSessionParts();

                                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                                    ufsession_.Disp.RegenerateDisplay();

                                    CreateEditData(editComponent);

                                    pHandle = SelectHandlePoint();
                                }

                                EnableForm();
                            }
                            else
                            {
                                ResetNonBlockError();
                                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                    "Not a block component");
                            }
                        }
                    }
                    else
                    {
                        TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                            "Mirrored Component");
                    }
                }
                else
                {
                    if(!_displayPart.FullPath.Contains("mirror"))
                    {
                        foreach (Feature featBlk in _workPart.Features)
                            if(featBlk.FeatureType == "BLOCK")
                                if(featBlk.Name == "DYNAMIC BLOCK")
                                    isBlockComponent = true;

                        if(isBlockComponent)
                        {
                            DisableForm();

                            if(_isNewSelection)
                            {
                                CreateEditData(editComponent);

                                _isNewSelection = false;
                            }

                            var pHandle = new List<Point>();
                            pHandle = SelectHandlePoint();

                            _isDynamic = true;

                            while (pHandle.Count == 1)
                            {
                                HideDynamicHandles();

                                _udoPointHandle = pHandle[0];

                                var blockOrigin = new Point3d();
                                var blockLength = 0.00;
                                var blockWidth = 0.00;
                                var blockHeight = 0.00;

                                foreach (var eLine in _edgeRepLines)
                                {
                                    if(eLine.Name == "XBASE1")
                                    {
                                        blockOrigin = eLine.StartPoint;
                                        blockLength = eLine.GetLength();
                                    }

                                    if(eLine.Name == "YBASE1") blockWidth = eLine.GetLength();

                                    if(eLine.Name == "ZBASE1") blockHeight = eLine.GetLength();
                                }

                                Point pointPrototype;

                                if(_udoPointHandle.IsOccurrence)
                                    pointPrototype = (Point)_udoPointHandle.Prototype;
                                else
                                    pointPrototype = _udoPointHandle;

                                var doNotMovePts = new List<NXObject>();
                                var movePtsHalf = new List<NXObject>();
                                var movePtsFull = new List<NXObject>();

                                if(pointPrototype.Name.Contains("POS"))
                                {
                                    foreach (Point namedPt in _workPart.Points)
                                        if(namedPt.Name != "")
                                        {
                                            if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                                doNotMovePts.Add(namedPt);
                                            else if(namedPt.Name.Contains("BLKORIGIN"))
                                                doNotMovePts.Add(namedPt);
                                            else
                                                movePtsHalf.Add(namedPt);
                                        }

                                    movePtsFull.Add(pointPrototype);
                                }
                                else
                                {
                                    foreach (Point namedPt in _workPart.Points)
                                        if(namedPt.Name != "")
                                        {
                                            if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                                doNotMovePts.Add(namedPt);

                                            else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                                doNotMovePts.Add(namedPt);
                                            else if(namedPt.Name.Contains("BLKORIGIN"))
                                                movePtsFull.Add(namedPt);
                                            else
                                                movePtsHalf.Add(namedPt);
                                        }

                                    movePtsFull.Add(pointPrototype);
                                }

                                var posXObjs = new List<Line>();
                                var negXObjs = new List<Line>();
                                var posYObjs = new List<Line>();
                                var negYObjs = new List<Line>();
                                var posZObjs = new List<Line>();
                                var negZObjs = new List<Line>();

                                foreach (var eLine in _edgeRepLines)
                                {
                                    if(eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" || eLine.Name == "ZBASE1" ||
                                       eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                    if(eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" || eLine.Name == "ZBASE2" ||
                                       eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                    if(eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                                       eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                    if(eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                                       eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                    if(eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                                       eLine.Name == "YBASE2") negZObjs.Add(eLine);

                                    if(eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                                       eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                                }

                                var allxAxisLines = new List<Line>();
                                var allyAxisLines = new List<Line>();
                                var allzAxisLines = new List<Line>();

                                foreach (var eLine in _edgeRepLines)
                                {
                                    if(eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                                    if(eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                                    if(eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                                }

                                EditSizeForm sizeForm = null;
                                var convertLength = blockLength / 25.4;
                                var convertWidth = blockWidth / 25.4;
                                var convertHeight = blockHeight / 25.4;

                                if(_displayPart.PartUnits == BasePart.Units.Inches)
                                {
                                    if(pointPrototype.Name.Contains("X"))
                                    {
                                        sizeForm = new EditSizeForm(blockLength);
                                        sizeForm.ShowDialog();
                                    }

                                    if(pointPrototype.Name.Contains("Y"))
                                    {
                                        sizeForm = new EditSizeForm(blockWidth);
                                        sizeForm.ShowDialog();
                                    }

                                    if(pointPrototype.Name.Contains("Z"))
                                    {
                                        sizeForm = new EditSizeForm(blockHeight);
                                        sizeForm.ShowDialog();
                                    }
                                }
                                else
                                {
                                    if(pointPrototype.Name.Contains("X"))
                                    {
                                        sizeForm = new EditSizeForm(convertLength);
                                        sizeForm.ShowDialog();
                                    }

                                    if(pointPrototype.Name.Contains("Y"))
                                    {
                                        sizeForm = new EditSizeForm(convertWidth);
                                        sizeForm.ShowDialog();
                                    }

                                    if(pointPrototype.Name.Contains("Z"))
                                    {
                                        sizeForm = new EditSizeForm(convertHeight);
                                        sizeForm.ShowDialog();
                                    }
                                }

                                if(sizeForm.DialogResult == DialogResult.OK)
                                {
                                    var editSize = sizeForm.InputValue;
                                    double distance = 0;

                                    if(_displayPart.PartUnits == BasePart.Units.Millimeters) editSize *= 25.4;

                                    if(editSize > 0)
                                    {
                                        if(pointPrototype.Name == "POSX")
                                        {
                                            distance = editSize - blockLength;

                                            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                            foreach (var xAxisLine in allxAxisLines)
                                            {
                                                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                                var addX = new Point3d(mappedEndPoint.X + distance, mappedEndPoint.Y,
                                                    mappedEndPoint.Z);
                                                var mappedAddX = MapWcsToAbsolute(addX);
                                                xAxisLine.SetEndPoint(mappedAddX);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                        }

                                        if(pointPrototype.Name == "NEGX")
                                        {
                                            distance = blockLength - editSize;

                                            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

                                            foreach (var xAxisLine in allxAxisLines)
                                            {
                                                var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                                                var addX = new Point3d(mappedStartPoint.X + distance,
                                                    mappedStartPoint.Y, mappedStartPoint.Z);
                                                var mappedAddX = MapWcsToAbsolute(addX);
                                                xAxisLine.SetStartPoint(mappedAddX);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                        }

                                        if(pointPrototype.Name == "POSY")
                                        {
                                            distance = editSize - blockWidth;

                                            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

                                            foreach (var yAxisLine in allyAxisLines)
                                            {
                                                var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                                                var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + distance,
                                                    mappedEndPoint.Z);
                                                var mappedAddY = MapWcsToAbsolute(addY);
                                                yAxisLine.SetEndPoint(mappedAddY);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                        }

                                        if(pointPrototype.Name == "NEGY")
                                        {
                                            distance = blockWidth - editSize;

                                            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

                                            foreach (var yAxisLine in allyAxisLines)
                                            {
                                                var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                                                var addY = new Point3d(mappedStartPoint.X,
                                                    mappedStartPoint.Y + distance, mappedStartPoint.Z);
                                                var mappedAddY = MapWcsToAbsolute(addY);
                                                yAxisLine.SetStartPoint(mappedAddY);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                        }

                                        if(pointPrototype.Name == "POSZ")
                                        {
                                            distance = editSize - blockHeight;

                                            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

                                            foreach (var zAxisLine in allzAxisLines)
                                            {
                                                var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                                                var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                                                    mappedEndPoint.Z + distance);
                                                var mappedAddZ = MapWcsToAbsolute(addZ);
                                                zAxisLine.SetEndPoint(mappedAddZ);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                        }

                                        if(pointPrototype.Name == "NEGZ")
                                        {
                                            distance = blockHeight - editSize;

                                            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

                                            foreach (var zAxisLine in allzAxisLines)
                                            {
                                                var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                                                var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                                                    mappedStartPoint.Z + distance);
                                                var mappedAddZ = MapWcsToAbsolute(addZ);
                                                zAxisLine.SetStartPoint(mappedAddZ);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                        }
                                    }
                                }

                                UpdateDynamicBlock(editComponent);

                                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                                sizeForm.Close();
                                sizeForm.Dispose();

                                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                session_.Parts.SetWorkComponent(editComponent, out var partLoadComponent1);
                                partLoadComponent1.Dispose();
                                UpdateSessionParts();

                                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                                ufsession_.Disp.RegenerateDisplay();

                                CreateEditData(editComponent);

                                pHandle = SelectHandlePoint();
                            }

                            EnableForm();
                        }
                        else
                        {
                            ResetNonBlockError();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Not a block component");
                        }
                    }
                    else
                    {
                        TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                            "Mirrored Component");
                    }
                }
                //}
                //else
                //{
                //    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                //    ufsession_.Disp.RegenerateDisplay();
                //    TheUISession.NXMessageBox.Show("Caught exception : Edit Size", NXOpen.NXMessageBox.DialogType.Error, "Analysis --> Units does not match the session units");
                //}
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonEditAlign_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId alignId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Align Block");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody != null)
                {
                    var editComponent = _editBody.OwningComponent;

                    if(editComponent != null)
                    {
                        var checkPartName = (Part)editComponent.Prototype;

                        if(!checkPartName.FullPath.Contains("mirror"))
                        {
                            _updateComponent = editComponent;

                            var assmUnits = _displayPart.PartUnits;
                            var compBase = (BasePart)editComponent.Prototype;
                            var compUnits = compBase.PartUnits;

                            if(compUnits == assmUnits)
                            {
                                if(_isNewSelection)
                                {
                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                    session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                    partLoadStatusWorkComp.Dispose();
                                    UpdateSessionParts();

                                    foreach (Feature featBlk in _workPart.Features)
                                        if(featBlk.FeatureType == "BLOCK")
                                            if(featBlk.Name == "DYNAMIC BLOCK")
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

                                if(isBlockComponent)
                                {
                                    UpdateDynamicBlock(editComponent);
                                    CreateEditData(editComponent);

                                    var pHandle = new List<Point>();
                                    pHandle = SelectHandlePoint();

                                    _isDynamic = true;

                                    while (pHandle.Count == 1)
                                    {
                                        HideDynamicHandles();

                                        _udoPointHandle = pHandle[0];

                                        Hide();

                                        Point pointPrototype;

                                        if(_udoPointHandle.IsOccurrence)
                                            pointPrototype = (Point)_udoPointHandle.Prototype;
                                        else
                                            pointPrototype = _udoPointHandle;

                                        var doNotMovePts = new List<NXObject>();
                                        var movePtsHalf = new List<NXObject>();
                                        var movePtsFull = new List<NXObject>();

                                        if(pointPrototype.Name.Contains("POS"))
                                        {
                                            foreach (Point namedPt in _workPart.Points)
                                                if(namedPt.Name != "")
                                                {
                                                    if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Y") &&
                                                            pointPrototype.Name.Contains("Y"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Z") &&
                                                            pointPrototype.Name.Contains("Z"))
                                                        doNotMovePts.Add(namedPt);
                                                    else if(namedPt.Name.Contains("BLKORIGIN"))
                                                        doNotMovePts.Add(namedPt);
                                                    else
                                                        movePtsHalf.Add(namedPt);
                                                }

                                            movePtsFull.Add(pointPrototype);
                                        }
                                        else
                                        {
                                            foreach (Point namedPt in _workPart.Points)
                                                if(namedPt.Name != "")
                                                {
                                                    if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Y") &&
                                                            pointPrototype.Name.Contains("Y"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Z") &&
                                                            pointPrototype.Name.Contains("Z"))
                                                        doNotMovePts.Add(namedPt);
                                                    else if(namedPt.Name.Contains("BLKORIGIN"))
                                                        movePtsFull.Add(namedPt);
                                                    else
                                                        movePtsHalf.Add(namedPt);
                                                }

                                            movePtsFull.Add(pointPrototype);
                                        }

                                        var posXObjs = new List<Line>();
                                        var negXObjs = new List<Line>();
                                        var posYObjs = new List<Line>();
                                        var negYObjs = new List<Line>();
                                        var posZObjs = new List<Line>();
                                        var negZObjs = new List<Line>();

                                        foreach (var eLine in _edgeRepLines)
                                        {
                                            if(eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                                               eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                            if(eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                                               eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                            if(eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                                               eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                            if(eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                                               eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                            if(eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                                               eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                                            if(eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                                               eLine.Name == "YCEILING1" ||
                                               eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                                        }

                                        var allxAxisLines = new List<Line>();
                                        var allyAxisLines = new List<Line>();
                                        var allzAxisLines = new List<Line>();

                                        foreach (var eLine in _edgeRepLines)
                                        {
                                            if(eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                                            if(eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                                            if(eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                                        }

                                        var message = "Select Reference Point";
                                        var pbMethod = UFUi.PointBaseMethod.PointInferred;
                                        var selection = NXOpen.Tag.Null;
                                        var basePoint = new double[3];

                                        ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                        ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                                            out var response);

                                        ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                                        if(response == UF_UI_OK)
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

                                            if(pointPrototype.Name == "POSX")
                                            {
                                                distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                if(mappedBase[0] < mappedPoint[0]) distance *= -1;

                                                foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                                foreach (var xAxisLine in allxAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                                    var addX = new Point3d(mappedEndPoint.X + distance,
                                                        mappedEndPoint.Y, mappedEndPoint.Z);
                                                    var mappedAddX = MapWcsToAbsolute(addX);
                                                    xAxisLine.SetEndPoint(mappedAddX);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                            }

                                            if(pointPrototype.Name == "NEGX")
                                            {
                                                distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                if(mappedBase[0] < mappedPoint[0]) distance *= -1;

                                                foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

                                                foreach (var xAxisLine in allxAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                                                    var addX = new Point3d(mappedStartPoint.X + distance,
                                                        mappedStartPoint.Y, mappedStartPoint.Z);
                                                    var mappedAddX = MapWcsToAbsolute(addX);
                                                    xAxisLine.SetStartPoint(mappedAddX);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                            }

                                            if(pointPrototype.Name == "POSY")
                                            {
                                                distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                if(mappedBase[1] < mappedPoint[1]) distance *= -1;

                                                foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

                                                foreach (var yAxisLine in allyAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                                                    var addY = new Point3d(mappedEndPoint.X,
                                                        mappedEndPoint.Y + distance, mappedEndPoint.Z);
                                                    var mappedAddY = MapWcsToAbsolute(addY);
                                                    yAxisLine.SetEndPoint(mappedAddY);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                            }

                                            if(pointPrototype.Name == "NEGY")
                                            {
                                                distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                if(mappedBase[1] < mappedPoint[1]) distance *= -1;

                                                foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

                                                foreach (var yAxisLine in allyAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                                                    var addY = new Point3d(mappedStartPoint.X,
                                                        mappedStartPoint.Y + distance, mappedStartPoint.Z);
                                                    var mappedAddY = MapWcsToAbsolute(addY);
                                                    yAxisLine.SetStartPoint(mappedAddY);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                            }

                                            if(pointPrototype.Name == "POSZ")
                                            {
                                                distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                if(mappedBase[2] < mappedPoint[2]) distance *= -1;

                                                foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

                                                foreach (var zAxisLine in allzAxisLines)
                                                {
                                                    var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                                                    var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                                                        mappedEndPoint.Z + distance);
                                                    var mappedAddZ = MapWcsToAbsolute(addZ);
                                                    zAxisLine.SetEndPoint(mappedAddZ);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                            }

                                            if(pointPrototype.Name == "NEGZ")
                                            {
                                                distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                if(mappedBase[2] < mappedPoint[2]) distance *= -1;

                                                foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

                                                foreach (var zAxisLine in allzAxisLines)
                                                {
                                                    var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                                                    var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                                                        mappedStartPoint.Z + distance);
                                                    var mappedAddZ = MapWcsToAbsolute(addZ);
                                                    zAxisLine.SetStartPoint(mappedAddZ);
                                                }

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                            }
                                        }

                                        UpdateDynamicBlock(editComponent);

                                        session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                                        session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                        session_.Parts.SetWorkComponent(editComponent, out var partLoadComponent1);
                                        partLoadComponent1.Dispose();
                                        UpdateSessionParts();

                                        CreateEditData(editComponent);
                                        pHandle = SelectHandlePoint();
                                    }

                                    Show();
                                }
                                else
                                {
                                    ResetNonBlockError();
                                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                        "Not a block component");
                                }
                            }
                        }
                        else
                        {
                            Show();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Mirrored Component");
                        }
                    }
                    else
                    {
                        Show();
                        TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                            "This function is not allowed in this context");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonAlignComponent_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId alignId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Align Block");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody != null)
                {
                    var editComponent = _editBody.OwningComponent;

                    if(editComponent != null)
                    {
                        var checkPartName = (Part)editComponent.Prototype;

                        if(!checkPartName.FullPath.Contains("mirror"))
                        {
                            _updateComponent = editComponent;

                            var assmUnits = _displayPart.PartUnits;
                            var compBase = (BasePart)editComponent.Prototype;
                            var compUnits = compBase.PartUnits;

                            if(compUnits == assmUnits)
                            {
                                if(_isNewSelection)
                                {
                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                    session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                    partLoadStatusWorkComp.Dispose();
                                    UpdateSessionParts();

                                    foreach (Feature featBlk in _workPart.Features)
                                        if(featBlk.FeatureType == "BLOCK")
                                            if(featBlk.Name == "DYNAMIC BLOCK")
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

                                if(isBlockComponent)
                                {
                                    var pHandle = new List<Point>();
                                    pHandle = SelectHandlePoint();

                                    _isDynamic = true;

                                    while (pHandle.Count == 1)
                                    {
                                        HideDynamicHandles();

                                        _udoPointHandle = pHandle[0];

                                        Hide();

                                        Point pointPrototype;

                                        if(_udoPointHandle.IsOccurrence)
                                            pointPrototype = (Point)_udoPointHandle.Prototype;
                                        else
                                            pointPrototype = _udoPointHandle;

                                        var movePtsFull = new List<NXObject>();

                                        foreach (Point nPoint in _workPart.Points)
                                            if(nPoint.Name.Contains("X") || nPoint.Name.Contains("Y") ||
                                               nPoint.Name.Contains("Z") || nPoint.Name.Contains("BLKORIGIN"))
                                                movePtsFull.Add(nPoint);

                                        foreach (Line nLine in _workPart.Lines)
                                            if(nLine.Name.Contains("X") || nLine.Name.Contains("Y") ||
                                               nLine.Name.Contains("Z"))
                                                movePtsFull.Add(nLine);

                                        var message = "Select Reference Point";
                                        var pbMethod = UFUi.PointBaseMethod.PointInferred;
                                        var selection = NXOpen.Tag.Null;
                                        var basePoint = new double[3];

                                        ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                        ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                                            out var response);

                                        ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                                        if(response == UF_UI_OK)
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

                                            if(pointPrototype.Name == "POSX")
                                            {
                                                distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                if(mappedBase[0] < mappedPoint[0]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                            }

                                            if(pointPrototype.Name == "NEGX")
                                            {
                                                distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                if(mappedBase[0] < mappedPoint[0]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "X");
                                            }

                                            if(pointPrototype.Name == "POSY")
                                            {
                                                distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                if(mappedBase[1] < mappedPoint[1]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                            }

                                            if(pointPrototype.Name == "NEGY")
                                            {
                                                distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                if(mappedBase[1] < mappedPoint[1]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                            }

                                            if(pointPrototype.Name == "POSZ")
                                            {
                                                distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                if(mappedBase[2] < mappedPoint[2]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                            }

                                            if(pointPrototype.Name == "NEGZ")
                                            {
                                                distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                if(mappedBase[2] < mappedPoint[2]) distance *= -1;

                                                MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                            }
                                        }

                                        UpdateDynamicBlock(editComponent);

                                        session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                                        session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                        session_.Parts.SetWorkComponent(editComponent, out var partLoadComponent1);
                                        partLoadComponent1.Dispose();
                                        UpdateSessionParts();

                                        CreateEditData(editComponent);
                                        pHandle = SelectHandlePoint();
                                    }

                                    Show();
                                }
                                else
                                {
                                    ResetNonBlockError();
                                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                        "Not a block component");
                                }
                            }
                        }
                        else
                        {
                            Show();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Mirrored Component");
                        }
                    }
                    else
                    {
                        Show();
                        TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                            "This function is not allowed in this context");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId alignDistanceId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Align Distance");

            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;

                var isBlockComponent = false;

                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;

                if(dispUnits == Part.Units.Millimeters)
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

                //if (areUnitsEqual)
                //{
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(_isNewSelection)
                    if(_updateComponent == null)
                    {
                        SelectWithFilter.NonValidCandidates = _nonValidNames;
                        SelectWithFilter.GetSelectedWithFilter("Select Component to Align Edge");
                        _editBody = SelectWithFilter.SelectedCompBody;
                        _isNewSelection = true;
                    }

                if(_editBody != null)
                {
                    var editComponent = _editBody.OwningComponent;

                    if(editComponent != null)
                    {
                        var checkPartName = (Part)editComponent.Prototype;

                        if(!checkPartName.FullPath.Contains("mirror"))
                        {
                            _updateComponent = editComponent;

                            var assmUnits = _displayPart.PartUnits;
                            var compBase = (BasePart)editComponent.Prototype;
                            var compUnits = compBase.PartUnits;

                            if(compUnits == assmUnits)
                            {
                                if(_isNewSelection)
                                {
                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                    session_.Parts.SetWorkComponent(editComponent, out var partLoadStatusWorkComp);
                                    partLoadStatusWorkComp.Dispose();
                                    UpdateSessionParts();

                                    foreach (Feature featBlk in _workPart.Features)
                                        if(featBlk.FeatureType == "BLOCK")
                                            if(featBlk.Name == "DYNAMIC BLOCK")
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

                                if(isBlockComponent)
                                {
                                    var pHandle = new List<Point>();
                                    pHandle = SelectHandlePoint();

                                    _isDynamic = true;

                                    while (pHandle.Count == 1)
                                    {
                                        HideDynamicHandles();

                                        _udoPointHandle = pHandle[0];

                                        Hide();

                                        Point pointPrototype;

                                        if(_udoPointHandle.IsOccurrence)
                                            pointPrototype = (Point)_udoPointHandle.Prototype;
                                        else
                                            pointPrototype = _udoPointHandle;

                                        var doNotMovePts = new List<NXObject>();
                                        var movePtsHalf = new List<NXObject>();
                                        var movePtsFull = new List<NXObject>();

                                        if(pointPrototype.Name.Contains("POS"))
                                        {
                                            foreach (Point namedPt in _workPart.Points)
                                                if(namedPt.Name != "")
                                                {
                                                    if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Y") &&
                                                            pointPrototype.Name.Contains("Y"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Z") &&
                                                            pointPrototype.Name.Contains("Z"))
                                                        doNotMovePts.Add(namedPt);
                                                    else if(namedPt.Name.Contains("BLKORIGIN"))
                                                        doNotMovePts.Add(namedPt);
                                                    else
                                                        movePtsHalf.Add(namedPt);
                                                }

                                            movePtsFull.Add(pointPrototype);
                                        }
                                        else
                                        {
                                            foreach (Point namedPt in _workPart.Points)
                                                if(namedPt.Name != "")
                                                {
                                                    if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Y") &&
                                                            pointPrototype.Name.Contains("Y"))
                                                        doNotMovePts.Add(namedPt);

                                                    else if(namedPt.Name.Contains("Z") &&
                                                            pointPrototype.Name.Contains("Z"))
                                                        doNotMovePts.Add(namedPt);
                                                    else if(namedPt.Name.Contains("BLKORIGIN"))
                                                        movePtsFull.Add(namedPt);
                                                    else
                                                        movePtsHalf.Add(namedPt);
                                                }

                                            movePtsFull.Add(pointPrototype);
                                        }

                                        var posXObjs = new List<Line>();
                                        var negXObjs = new List<Line>();
                                        var posYObjs = new List<Line>();
                                        var negYObjs = new List<Line>();
                                        var posZObjs = new List<Line>();
                                        var negZObjs = new List<Line>();

                                        foreach (var eLine in _edgeRepLines)
                                        {
                                            if(eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                                               eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                            if(eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                                               eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                            if(eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                                               eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                            if(eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                                               eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                            if(eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                                               eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                                            if(eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                                               eLine.Name == "YCEILING1" ||
                                               eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                                        }

                                        var allxAxisLines = new List<Line>();
                                        var allyAxisLines = new List<Line>();
                                        var allzAxisLines = new List<Line>();

                                        foreach (var eLine in _edgeRepLines)
                                        {
                                            if(eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                                            if(eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                                            if(eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                                        }

                                        var message = "Select Reference Point";
                                        var pbMethod = UFUi.PointBaseMethod.PointInferred;
                                        var selection = NXOpen.Tag.Null;
                                        var basePoint = new double[3];

                                        ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                                        ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                                            out var response);

                                        ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                                        if(response == UF_UI_OK)
                                        {
                                            bool isDistance;

                                            isDistance = NXInputBox.ParseInputNumber("Enter offset value",
                                                "Enter offset value", .004, NumberStyles.AllowDecimalPoint,
                                                CultureInfo.InvariantCulture.NumberFormat, out var inputDist);

                                            if(isDistance)
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

                                                if(pointPrototype.Name == "POSX")
                                                {
                                                    distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                    if(mappedBase[0] < mappedPoint[0])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                                    foreach (var xAxisLine in allxAxisLines)
                                                    {
                                                        var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                                        var addX = new Point3d(mappedEndPoint.X + distance,
                                                            mappedEndPoint.Y, mappedEndPoint.Z);
                                                        var mappedAddX = MapWcsToAbsolute(addX);
                                                        xAxisLine.SetEndPoint(mappedAddX);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                                }

                                                if(pointPrototype.Name == "NEGX")
                                                {
                                                    distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                                    if(mappedBase[0] < mappedPoint[0])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

                                                    foreach (var xAxisLine in allxAxisLines)
                                                    {
                                                        var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                                                        var addX = new Point3d(mappedStartPoint.X + distance,
                                                            mappedStartPoint.Y, mappedStartPoint.Z);
                                                        var mappedAddX = MapWcsToAbsolute(addX);
                                                        xAxisLine.SetStartPoint(mappedAddX);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "X");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                                }

                                                if(pointPrototype.Name == "POSY")
                                                {
                                                    distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                    if(mappedBase[1] < mappedPoint[1])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

                                                    foreach (var yAxisLine in allyAxisLines)
                                                    {
                                                        var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                                                        var addY = new Point3d(mappedEndPoint.X,
                                                            mappedEndPoint.Y + distance, mappedEndPoint.Z);
                                                        var mappedAddY = MapWcsToAbsolute(addY);
                                                        yAxisLine.SetEndPoint(mappedAddY);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                                }

                                                if(pointPrototype.Name == "NEGY")
                                                {
                                                    distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                                    if(mappedBase[1] < mappedPoint[1])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

                                                    foreach (var yAxisLine in allyAxisLines)
                                                    {
                                                        var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                                                        var addY = new Point3d(mappedStartPoint.X,
                                                            mappedStartPoint.Y + distance, mappedStartPoint.Z);
                                                        var mappedAddY = MapWcsToAbsolute(addY);
                                                        yAxisLine.SetStartPoint(mappedAddY);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "Y");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
                                                }

                                                if(pointPrototype.Name == "POSZ")
                                                {
                                                    distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                    if(mappedBase[2] < mappedPoint[2])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

                                                    foreach (var zAxisLine in allzAxisLines)
                                                    {
                                                        var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                                                        var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                                                            mappedEndPoint.Z + distance);
                                                        var mappedAddZ = MapWcsToAbsolute(addZ);
                                                        zAxisLine.SetEndPoint(mappedAddZ);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                                }

                                                if(pointPrototype.Name == "NEGZ")
                                                {
                                                    distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                                    if(mappedBase[2] < mappedPoint[2])
                                                    {
                                                        distance *= -1;
                                                        distance += inputDist;
                                                    }
                                                    else
                                                    {
                                                        distance -= inputDist;
                                                    }

                                                    foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

                                                    foreach (var zAxisLine in allzAxisLines)
                                                    {
                                                        var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                                                        var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                                                            mappedStartPoint.Z + distance);
                                                        var mappedAddZ = MapWcsToAbsolute(addZ);
                                                        zAxisLine.SetStartPoint(mappedAddZ);
                                                    }

                                                    MoveObjects(movePtsFull.ToArray(), distance, "Z");
                                                    MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
                                                }
                                            }
                                            else
                                            {
                                                Show();
                                                TheUISession.NXMessageBox.Show("Caught exception",
                                                    NXMessageBox.DialogType.Error, "Invalid input");
                                            }
                                        }

                                        UpdateDynamicBlock(editComponent);

                                        session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                                        session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                        session_.Parts.SetWorkComponent(editComponent, out var partLoadComponent1);
                                        partLoadComponent1.Dispose();
                                        UpdateSessionParts();

                                        CreateEditData(editComponent);
                                        pHandle = SelectHandlePoint();
                                    }

                                    Show();
                                }
                                else
                                {
                                    ResetNonBlockError();
                                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                        "Not a block component");
                                }
                            }
                        }
                        else
                        {
                            Show();
                            TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                "Mirrored Component");
                        }
                    }
                    else
                    {
                        Show();
                        TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                            "This function is not allowed in this context");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }


        private void ComboBoxGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Return))
            {
                if(comboBoxGridBlock.Text == "0.000")
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
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            //Session.UndoMarkId undoEditBlock = session_.SetUndoMark(Session.MarkVisibility.Visible, "Edit Block");

            UpdateDynamicBlock(_updateComponent);

            session_.Parts.SetWork(_originalWorkPart);
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
            //buttonBlockFeature.Enabled = true;
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

            session_.Parts.SetWork(_displayPart);
        }


        //===========================================
        //  Methods
        //===========================================

        private void MoveComponent(Component compToMove)
        {
            try
            {
                UpdateSessionParts();

                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                if(compToMove != null)
                {
                    var assmUnits = _displayPart.PartUnits;
                    var compBase = (BasePart)compToMove.Prototype;
                    var compUnits = compBase.PartUnits;

                    if(compUnits == assmUnits)
                    {
                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                        session_.Parts.SetWorkComponent(compToMove, out var partLoadComponent);
                        partLoadComponent.Dispose();
                        UpdateSessionParts();

                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();

                        CreateEditData(compToMove);

                        _isNewSelection = false;

                        var pHandle = new List<Point>();
                        pHandle = SelectHandlePoint();

                        _isDynamic = false;

                        while (pHandle.Count == 1)
                        {
                            _distanceMoved = 0;

                            HideDynamicHandles();

                            _udoPointHandle = pHandle[0];

                            _displayPart.WCS.Visibility = false;

                            var message = "Select New Position";
                            var screenPos = new double[3];
                            var viewTag = NXOpen.Tag.Null;
                            var motionCbData = IntPtr.Zero;
                            var clientData = IntPtr.Zero;

                            ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                            var mView = (ModelingView)_displayPart.Views.WorkView;
                            _displayPart.Views.WorkView.Orient(mView.Matrix);

                            _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);

                            ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                                out viewTag, out var response);

                            if(response == UF_UI_PICK_RESPONSE)
                            {
                                UpdateDynamicHandles();
                                ShowDynamicHandles();
                                pHandle = SelectHandlePoint();
                            }

                            ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
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

                if(setCompTranslucency != null)
                {
                    DisplayModification editObjectDisplay;
                    editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                    editObjectDisplay.ApplyToAllFaces = true;
                    editObjectDisplay.NewTranslucency = 75;
                    DisplayableObject[] compObject = { setCompTranslucency };
                    editObjectDisplay.Apply(compObject);
                    editObjectDisplay.Dispose();
                }
                else
                {
                    foreach (Body dispBody in _workPart.Bodies)
                        if(dispBody.Layer == 1)
                        {
                            DisplayModification editObjectDisplay;
                            editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                            editObjectDisplay.ApplyToAllFaces = true;
                            editObjectDisplay.NewTranslucency = 75;
                            DisplayableObject[] bodyObject = { dispBody };
                            editObjectDisplay.Apply(bodyObject);
                            editObjectDisplay.Dispose();
                        }
                }

                SetWcsToWorkPart(setCompTranslucency);

                foreach (Feature featBlk in _workPart.Features)
                    if(featBlk.FeatureType == "BLOCK")
                        if(featBlk.Name == "DYNAMIC BLOCK")
                        {
                            // get current block feature
                            var block1 = (Block)featBlk;

                            BlockFeatureBuilder blockFeatureBuilderMatch;
                            blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                            var bOrigin = blockFeatureBuilderMatch.Origin;
                            var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                            var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                            var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                            var mLength = blockFeatureBuilderMatch.Length.Value;
                            var mWidth = blockFeatureBuilderMatch.Width.Value;
                            var mHeight = blockFeatureBuilderMatch.Height.Value;

                            session_.Parts.SetWork(_displayPart);
                            UpdateSessionParts();

                            if(mLength != 0 && mWidth != 0 && mHeight != 0)
                            {
                                // create edit block feature
                                Feature nullFeaturesFeature = null;
                                BlockFeatureBuilder blockFeatureBuilderEdit;
                                blockFeatureBuilderEdit =
                                    _displayPart.Features.CreateBlockFeatureBuilder(nullFeaturesFeature);

                                blockFeatureBuilderEdit.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                                var targetBodies1 = new Body[1];
                                Body nullBody = null;
                                targetBodies1[0] = nullBody;
                                blockFeatureBuilderEdit.BooleanOption.SetTargetBodies(targetBodies1);

                                blockFeatureBuilderEdit.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                                var originPoint1 = _displayPart.WCS.Origin;

                                blockFeatureBuilderEdit.SetOriginAndLengths(originPoint1, mLength.ToString(),
                                    mWidth.ToString(), mHeight.ToString());

                                blockFeatureBuilderEdit.SetBooleanOperationAndTarget(Feature.BooleanType.Create,
                                    nullBody);

                                Feature feature1;
                                feature1 = blockFeatureBuilderEdit.CommitFeature();

                                feature1.SetName("EDIT BLOCK");

                                var editBlock = (Block)feature1;
                                var editBody = editBlock.GetBodies();

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
                        }
            }
            catch (Exception ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                ex.__PrintException();
            }
        }

        private void UpdateDynamicHandles()
        {
            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if(myUdOclass != null)
            {
                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if(currentUdo.Length != 0)
                {
                    BasePart myBasePart = _workPart;
                    var myUdOmanager = myBasePart.UserDefinedObjectManager;

                    foreach (Point pointHandle in _workPart.Points)
                    foreach (var udoHandle in currentUdo)
                        if(pointHandle.Name == udoHandle.Name)
                        {
                            double[] pointLocation =
                                { pointHandle.Coordinates.X, pointHandle.Coordinates.Y, pointHandle.Coordinates.Z };

                            udoHandle.SetDoubles(pointLocation);
                            int[] displayFlag = { 0 };
                            udoHandle.SetIntegers(displayFlag);

                            pointHandle.Unblank();
                        }
                }
            }
        }

        private Point3d MapWcsToAbsolute(Point3d pointToMap)
        {
            Point3d mappedPoint;
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            var output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, input, UF_CSYS_ROOT_COORDS, output);
            mappedPoint.X = output[0];
            mappedPoint.Y = output[1];
            mappedPoint.Z = output[2];
            return mappedPoint;
        }

        private Point3d MapAbsoluteToWcs(Point3d pointToMap)
        {
            Point3d mappedPoint;
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            var output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            mappedPoint.X = output[0];
            mappedPoint.Y = output[1];
            mappedPoint.Z = output[2];
            return mappedPoint;
        }

        private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);

            var lineColor = 7;

            var endPointX1 = new Point3d(mappedStartPoint1.X + lineLength, mappedStartPoint1.Y, mappedStartPoint1.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            var xBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointX1);
            xBase1.SetName("XBASE1");
            xBase1.Color = lineColor;
            xBase1.RedisplayObject();
            _edgeRepLines.Add(xBase1);

            var endPointY1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y + lineWidth, mappedStartPoint1.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            var yBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointY1);
            yBase1.SetName("YBASE1");
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);

            var endPointZ1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y, mappedStartPoint1.Z + lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            var zBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointZ1);
            zBase1.SetName("ZBASE1");
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);

            //==================================================================================================================

            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = new Point3d(mappedStartPoint2.X + lineLength, mappedStartPoint2.Y, mappedStartPoint2.Z);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            var xbase2 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointX2);
            xbase2.SetName("XBASE2");
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);

            var yBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX2);
            yBase2.SetName("YBASE2");
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);

            //==================================================================================================================

            var mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);

            var endPointX1Ceiling =
                new Point3d(mappedStartPoint3.X + lineLength, mappedStartPoint3.Y, mappedStartPoint3.Z);
            var mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            var xCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointX1Ceiling);
            xCeiling1.SetName("XCEILING1");
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);

            var endPointY1Ceiling =
                new Point3d(mappedStartPoint3.X, mappedStartPoint3.Y + lineWidth, mappedStartPoint3.Z);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            var yCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointY1Ceiling);
            yCeiling1.SetName("YCEILING1");
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);

            //==================================================================================================================

            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);

            var endPointX2Ceiling =
                new Point3d(mappedStartPoint4.X + lineLength, mappedStartPoint4.Y, mappedStartPoint4.Z);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            var xCeiling2 = _workPart.Curves.CreateLine(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            xCeiling2.SetName("XCEILING2");
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);

            var yCeiling2 = _workPart.Curves.CreateLine(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            yCeiling2.SetName("YCEILING2");
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);

            //==================================================================================================================

            var zBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX1Ceiling);
            zBase2.SetName(@"ZBASE2");
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);

            var zBase3 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointY1Ceiling);
            zBase3.SetName("ZBASE3");
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);

            var zBase4 = _workPart.Curves.CreateLine(mappedEndPointX2, mappedEndPointX2Ceiling);
            zBase4.SetName("ZBASE4");
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);

            //==================================================================================================================
        }

        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if(myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo;
                    currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                    if(currentUdo.Length == 0)
                    {
                        BasePart myBasePart = _workPart;
                        var myUdOmanager = myBasePart.UserDefinedObjectManager;

                        foreach (var blkFace in editBody.GetFaces())
                        {
                            var myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                            var myLinks = new UserDefinedObject.LinkDefinition[1];

                            var pointOnFace = new double[3];
                            var dir = new double[3];
                            var box = new double[6];
                            var matrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                            ufsession_.Modl.AskFaceData(blkFace.Tag, out var type, pointOnFace, dir, box,
                                out var radius, out var radData, out var normDir);

                            dir[0] = Math.Round(dir[0], 10);
                            dir[1] = Math.Round(dir[1], 10);
                            dir[2] = Math.Round(dir[2], 10);

                            double[] wcsVectorX =
                                { Math.Round(matrix1.Xx, 10), Math.Round(matrix1.Xy, 10), Math.Round(matrix1.Xz, 10) };
                            double[] wcsVectorY =
                                { Math.Round(matrix1.Yx, 10), Math.Round(matrix1.Yy, 10), Math.Round(matrix1.Yz, 10) };
                            double[] wcsVectorZ =
                                { Math.Round(matrix1.Zx, 10), Math.Round(matrix1.Zy, 10), Math.Round(matrix1.Zz, 10) };

                            var wcsVectorNegX = new double[3];
                            var wcsVectorNegY = new double[3];
                            var wcsVectorNegZ = new double[3];

                            ufsession_.Vec3.Negate(wcsVectorX, wcsVectorNegX);
                            ufsession_.Vec3.Negate(wcsVectorY, wcsVectorNegY);
                            ufsession_.Vec3.Negate(wcsVectorZ, wcsVectorNegZ);

                            // create udo handle points

                            ufsession_.Vec3.IsEqual(dir, wcsVectorX, 0.00, out var isEqualX);

                            if(isEqualX == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("POSX");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("POSX");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }

                            ufsession_.Vec3.IsEqual(dir, wcsVectorY, 0.00, out var isEqualY);

                            if(isEqualY == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("POSY");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("POSY");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }

                            ufsession_.Vec3.IsEqual(dir, wcsVectorZ, 0.00, out var isEqualZ);

                            if(isEqualZ == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("POSZ");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("POSZ");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }

                            ufsession_.Vec3.IsEqual(dir, wcsVectorNegX, 0.00, out var isEqualNegX);

                            if(isEqualNegX == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("NEGX");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("NEGX");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }

                            ufsession_.Vec3.IsEqual(dir, wcsVectorNegY, 0.00, out var isEqualNegY);

                            if(isEqualNegY == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("NEGY");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("NEGY");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }

                            ufsession_.Vec3.IsEqual(dir, wcsVectorNegZ, 0.00, out var isEqualNegZ);

                            if(isEqualNegZ == 1)
                            {
                                var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
                                var point1 = _workPart.Points.CreatePoint(pointLocation);
                                point1.SetVisibility(SmartObject.VisibilityOption.Visible);
                                point1.SetName("NEGZ");
                                point1.Layer = _displayPart.Layers.WorkLayer;
                                point1.RedisplayObject();

                                myUdo.SetName("NEGZ");
                                myUdo.SetDoubles(pointOnFace);
                                int[] displayFlag = { 0 };
                                myUdo.SetIntegers(displayFlag);

                                myLinks[0].AssociatedObject = point1;
                                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                            }
                        }

                        // create origin point

                        var pointLocationOrigin = _displayPart.WCS.Origin;
                        var point1Origin = _workPart.Points.CreatePoint(pointLocationOrigin);
                        point1Origin.SetVisibility(SmartObject.VisibilityOption.Visible);
                        point1Origin.Blank();
                        point1Origin.SetName("BLKORIGIN");
                        point1Origin.Layer = _displayPart.Layers.WorkLayer;
                        point1Origin.RedisplayObject();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ShowDynamicHandles()
        {
            try
            {
                UpdateSessionParts();

                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if(myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo;
                    currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                    if(currentUdo.Length != 0)
                    {
                        foreach (var dispUdo in currentUdo)
                        {
                            int[] setDisplay = { 1 };

                            dispUdo.SetIntegers(setDisplay);

                            ufsession_.Disp.AddItemToDisplay(dispUdo.Tag);
                        }

                        foreach (Point udoPoint in _workPart.Points)
                            if(udoPoint.Name != "" && udoPoint.Layer == _displayPart.Layers.WorkLayer)
                            {
                                udoPoint.SetVisibility(SmartObject.VisibilityOption.Visible);
                                udoPoint.RedisplayObject();
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void UpdateDynamicBlock(Component updateComp)
        {
            try
            {
                // set component translucency and update dynamic block

                if(updateComp != null)
                {
                    DisplayModification editObjectDisplay1;
                    editObjectDisplay1 = session_.DisplayManager.NewDisplayModification();
                    editObjectDisplay1.ApplyToAllFaces = true;
                    editObjectDisplay1.NewTranslucency = 0;
                    DisplayableObject[] compObject1 = { updateComp };
                    editObjectDisplay1.Apply(compObject1);
                    editObjectDisplay1.Dispose();
                }
                else
                {
                    foreach (Body dispBody in _workPart.Bodies)
                        if(dispBody.Layer == 1)
                        {
                            DisplayModification editObjectDisplay;
                            editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                            editObjectDisplay.ApplyToAllFaces = true;
                            editObjectDisplay.NewTranslucency = 0;
                            DisplayableObject[] bodyObject = { dispBody };
                            editObjectDisplay.Apply(bodyObject);
                            editObjectDisplay.Dispose();
                        }
                }

                var blkOrigin = new Point3d();
                var length = "";
                var width = "";
                var height = "";

                foreach (Point pPoint in _workPart.Points)
                {
                    if(pPoint.Name != "BLKORIGIN")
                        continue;

                    blkOrigin.X = pPoint.Coordinates.X;
                    blkOrigin.Y = pPoint.Coordinates.Y;
                    blkOrigin.Z = pPoint.Coordinates.Z;
                }

                foreach (var blkLine in _edgeRepLines)
                {
                    if(blkLine.Name == "XBASE1") length = blkLine.GetLength().ToString();

                    if(blkLine.Name == "YBASE1") width = blkLine.GetLength().ToString();

                    if(blkLine.Name == "ZBASE1") height = blkLine.GetLength().ToString();
                }

                if(_isUprParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                if(_isLwrParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                _displayPart.WCS.SetOriginAndMatrix(blkOrigin, _workCompOrientation);
                _workCompOrigin = blkOrigin;

                session_.Parts.SetWorkComponent(updateComp, out var partLoadComponent);
                partLoadComponent.Dispose();
                UpdateSessionParts();

                double[] fromPoint = { blkOrigin.X, blkOrigin.Y, blkOrigin.Z };
                var mappedPoint = new double[3];
                ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, fromPoint, UF_CSYS_WORK_COORDS, mappedPoint);

                blkOrigin.X = mappedPoint[0];
                blkOrigin.Y = mappedPoint[1];
                blkOrigin.Z = mappedPoint[2];

                foreach (Feature featDynamic in _workPart.Features)
                    if(featDynamic.FeatureType == "BLOCK")
                        if(featDynamic.Name == "DYNAMIC BLOCK")
                        {
                            var block2 = (Block)featDynamic;

                            BlockFeatureBuilder blockFeatureBuilder2;
                            blockFeatureBuilder2 = _workPart.Features.CreateBlockFeatureBuilder(block2);

                            blockFeatureBuilder2.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                            var targetBodies1 = new Body[1];
                            Body nullBody = null;
                            targetBodies1[0] = nullBody;
                            blockFeatureBuilder2.BooleanOption.SetTargetBodies(targetBodies1);

                            blockFeatureBuilder2.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                            var blkFeatBuilderPoint = _workPart.Points.CreatePoint(blkOrigin);
                            blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                            blockFeatureBuilder2.OriginPoint = blkFeatBuilderPoint;

                            var originPoint1 = blkOrigin;

                            blockFeatureBuilder2.SetOriginAndLengths(originPoint1, length, width, height);

                            blockFeatureBuilder2.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);

                            Feature feature1;
                            feature1 = blockFeatureBuilder2.CommitFeature();

                            blockFeatureBuilder2.Destroy();

                            _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                            session_.Parts.SetWork(_displayPart);
                            UpdateSessionParts();

                            // delete udo's / points
                            DeleteHandles();

                            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
                            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                            session_.Parts.SetWork(_originalWorkPart);
                            UpdateSessionParts();

                            _displayPart.WCS.Visibility = true;
                            _displayPart.Views.Refresh();
                        }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void HideDynamicHandles()
        {
            try
            {
                UpdateSessionParts();

                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if(myUdOclass != null)
                {
                    UserDefinedObject[] currentUdo;
                    currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                    if(currentUdo.Length != 0)
                    {
                        foreach (var dispUdo in currentUdo)
                        {
                            int[] setDisplay = { 0 };

                            dispUdo.SetIntegers(setDisplay);
                        }

                        foreach (Point namedPt in _workPart.Points)
                            if(namedPt.Name != "")
                                namedPt.Blank();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void DeleteHandles()
        {
            Session.UndoMarkId markDeleteObjs;
            markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if(myUdOclass != null)
            {
                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
            }

            foreach (Point namedPt in _workPart.Points)
                if(namedPt.Name != "")
                    session_.UpdateManager.AddToDeleteList(namedPt);

            foreach (var dLine in _edgeRepLines) session_.UpdateManager.AddToDeleteList(dLine);

            int errsDelObjs;
            errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

            session_.DeleteUndoMark(markDeleteObjs, "");

            _edgeRepLines.Clear();
        }

        private List<Point> SelectHandlePoint()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);
            Selection.Response sel;
            var pointSelection = new List<Point>();

            sel = TheUISession.SelectionManager.SelectTaggedObject("Select Point", "Select Point",
                Selection.SelectionScope.WorkPart,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedPoint, out var cursor);

            if((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
                pointSelection.Add((Point)selectedPoint);

            return pointSelection;
        }


        private void SetWcsToWorkPart(Component compRefCsys)
        {
            try
            {
                if(compRefCsys != null)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    var compBase = (BasePart)compRefCsys.Prototype;

                    session_.Parts.SetDisplay(compBase, false, false, out var setDispLoadStatus);
                    setDispLoadStatus.Dispose();
                    UpdateSessionParts();

                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;
                    _parallelWidthExp = string.Empty;

                    foreach (Expression exp in _workPart.Expressions)
                    {
                        if(exp.Name == "uprParallel")
                        {
                            if(exp.RightHandSide.Contains("yes"))
                                _isUprParallel = true;
                            else
                                _isUprParallel = false;
                        }

                        if(exp.Name == "lwrParallel")
                        {
                            if(exp.RightHandSide.Contains("yes"))
                                _isLwrParallel = true;
                            else
                                _isLwrParallel = false;
                        }
                    }

                    foreach (Feature featBlk in _workPart.Features)
                        if(featBlk.FeatureType == "BLOCK")
                            if(featBlk.Name == "DYNAMIC BLOCK")
                            {
                                var block1 = (Block)featBlk;

                                BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                                var bOrigin = blockFeatureBuilderMatch.Origin;
                                var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                                var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                                var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                                var mLength = blockFeatureBuilderMatch.Length.Value;
                                var mWidth = blockFeatureBuilderMatch.Width.Value;
                                var mHeight = blockFeatureBuilderMatch.Height.Value;

                                if(_isUprParallel)
                                {
                                    _parallelHeightExp = "uprParallelHeight";
                                    _parallelWidthExp = "uprParallelWidth";
                                }

                                if(_isLwrParallel)
                                {
                                    _parallelHeightExp = "lwrParallelHeight";
                                    _parallelWidthExp = "lwrParallelWidth";
                                }

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                                _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                                    setTempCsys.Orientation.Element);

                                var featBlkCsys = _displayPart.WCS.Save();
                                featBlkCsys.SetName("EDITCSYS");
                                featBlkCsys.Layer = 254;

                                NXObject[] addToBody = { featBlkCsys };

                                foreach (var bRefSet in _displayPart.GetAllReferenceSets())
                                    if(bRefSet.Name == "BODY")
                                        bRefSet.AddObjectsToReferenceSet(addToBody);

                                session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                                    out var setDispLoadStatus1);
                                setDispLoadStatus1.Dispose();
                                session_.Parts.SetWorkComponent(compRefCsys, out var partLoadStatusWorkComp);
                                partLoadStatusWorkComp.Dispose();
                                UpdateSessionParts();

                                foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                                    if(wpCsys.Layer == 254)
                                        if(wpCsys.Name == "EDITCSYS")
                                        {
                                            NXObject csysOccurrence;
                                            csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                            var editCsys = (CartesianCoordinateSystem)csysOccurrence;

                                            if(editCsys != null)
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
                else
                {
                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;

                    foreach (Expression exp in _workPart.Expressions)
                    {
                        if(exp.Name == "uprParallel")
                        {
                            if(exp.RightHandSide.Contains("yes"))
                                _isUprParallel = true;
                            else
                                _isUprParallel = false;
                        }

                        if(exp.Name == "lwrParallel")
                        {
                            if(exp.RightHandSide.Contains("yes"))
                                _isLwrParallel = true;
                            else
                                _isLwrParallel = false;
                        }
                    }

                    foreach (Feature featBlk in _workPart.Features)
                        if(featBlk.FeatureType == "BLOCK")
                            if(featBlk.Name == "DYNAMIC BLOCK")
                            {
                                var block1 = (Block)featBlk;

                                BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                                var bOrigin = blockFeatureBuilderMatch.Origin;
                                var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                                var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                                var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                                var mLength = blockFeatureBuilderMatch.Length.Value;
                                var mWidth = blockFeatureBuilderMatch.Width.Value;
                                var mHeight = blockFeatureBuilderMatch.Height.Value;

                                if(_isUprParallel)
                                {
                                    _parallelHeightExp = "uprParallelHeight";
                                    _parallelWidthExp = "uprParallelWidth";
                                }

                                if(_isLwrParallel)
                                {
                                    _parallelHeightExp = "lwrParallelHeight";
                                    _parallelWidthExp = "lwrParallelWidth";
                                }

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                                _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                                    setTempCsys.Orientation.Element);
                                _workCompOrigin = setTempCsys.Origin;
                                _workCompOrientation = setTempCsys.Orientation.Element;
                            }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
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
            //buttonBlockFeature.Enabled = true;
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

            session_.Parts.SetWork(_displayPart);
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
            //buttonBlockFeature.Enabled = false;
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
            //buttonBlockFeature.Enabled = false;
            buttonReset.Enabled = false;
            buttonExit.Enabled = false;
        }

        private Component SelectOneComponent(string prompt)
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;

            sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedComp, out var cursor);

            if((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
            {
                compSelection = (Component)selectedComp;
                return compSelection;
            }

            return null;
        }

        private void ShowTemporarySizeText()
        {
            _displayPart.Views.Refresh();

            foreach (var eLine in _edgeRepLines)
                if(eLine.Name == "XBASE1" || eLine.Name == "YBASE1" || eLine.Name == "ZBASE1")
                {
                    var view = _displayPart.Views.WorkView.Tag;
                    var viewType = UFDisp.ViewType.UseWorkView;
                    var dim = string.Empty;

                    if(_displayPart.PartUnits == BasePart.Units.Inches)
                    {
                        var roundDim = Math.Round(eLine.GetLength(), 3);
                        dim = $"{roundDim:0.000}";
                    }
                    else
                    {
                        var roundDim = Math.Round(eLine.GetLength(), 3);
                        dim = $"{roundDim / 25.4:0.000}";
                    }

                    var midPoint = new double[3];
                    var dispProps = new UFObj.DispProps
                    {
                        color = 31
                    };
                    double charSize;
                    var font = 1;

                    if(_displayPart.PartUnits == BasePart.Units.Inches)
                        charSize = .125;
                    else
                        charSize = 3.175;

                    midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                    midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                    midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;

                    var mappedPoint = new double[3];

                    ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, midPoint, UF_CSYS_ROOT_COORDS, mappedPoint);

                    ufsession_.Disp.DisplayTemporaryText(view, viewType, dim, mappedPoint, UFDisp.TextRef.Middlecenter,
                        ref dispProps, charSize, font);
                }
        }

        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if(spacing != 0)
            {
                if(_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    var round = Math.Abs(cursor);
                    var roundValue = Math.Round(round, 3);
                    var truncateValue = Math.Truncate(roundValue);
                    var fractionValue = roundValue - truncateValue;
                    if(fractionValue != 0)
                        for (var ii = spacing; ii <= 1; ii += spacing)
                            if(fractionValue <= ii)
                            {
                                var roundedFraction = ii;
                                var finalValue = truncateValue + roundedFraction;
                                round = finalValue;
                                break;
                            }

                    if(cursor < 0) round *= -1;

                    return round;
                }
                else
                {
                    var round = Math.Abs(cursor / 25.4);
                    var roundValue = Math.Round(round, 3);
                    var truncateValue = Math.Truncate(roundValue);
                    var fractionValue = roundValue - truncateValue;
                    if(fractionValue != 0)
                        for (var ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                            if(fractionValue <= ii)
                            {
                                var roundedFraction = ii;
                                var finalValue = truncateValue + roundedFraction;
                                round = finalValue;
                                break;
                            }

                    if(cursor < 0) round *= -1;

                    return round * 25.4;
                }
            }

            return cursor;
        }


        private void MoveObjects(NXObject[] objsToMove, double distance, string deltaXyz)
        {
            try
            {
                if(distance != 0)
                {
                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                    MoveObject nullFeaturesMoveObject = null;

                    MoveObjectBuilder moveObjectBuilder1;
                    moveObjectBuilder1 = _workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeaturesMoveObject);

                    moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.AxisOption =
                        OrientXpressBuilder.Axis.Passive;

                    moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.PlaneOption =
                        OrientXpressBuilder.Plane.Passive;

                    moveObjectBuilder1.TransformMotion.OrientXpress.AxisOption = OrientXpressBuilder.Axis.Passive;

                    moveObjectBuilder1.TransformMotion.OrientXpress.PlaneOption = OrientXpressBuilder.Plane.Passive;

                    Point3d manipulatororigin1;
                    manipulatororigin1 = _displayPart.WCS.Origin;

                    Matrix3x3 manipulatormatrix1;
                    manipulatormatrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                    moveObjectBuilder1.TransformMotion.Option = ModlMotion.Options.DeltaXyz;

                    moveObjectBuilder1.TransformMotion.DeltaEnum = ModlMotion.Delta.ReferenceWcsWorkPart;

                    if(deltaXyz == "X")
                    {
                        moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = distance.ToString();
                        moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                        moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                    }

                    if(deltaXyz == "Y")
                    {
                        moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                        moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = distance.ToString();
                        moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                    }

                    if(deltaXyz == "Z")
                    {
                        moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                        moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                        moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = distance.ToString();
                    }

                    bool added1;
                    added1 = moveObjectBuilder1.ObjectToMoveObject.Add(objsToMove);

                    NXObject nXObject1;
                    nXObject1 = moveObjectBuilder1.Commit();

                    moveObjectBuilder1.Destroy();

                    _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void MotionCallback(double[] position, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            try
            {
                if(_isDynamic)
                {
                    var pointPrototype = _udoPointHandle.IsOccurrence
                        ? (Point)_udoPointHandle.Prototype
                        : _udoPointHandle;

                    var doNotMovePts = new List<NXObject>();

                    var movePtsHalf = new List<NXObject>();

                    var movePtsFull = new List<NXObject>();

                    if(pointPrototype.Name.Contains("POS"))
                    {
                        foreach (Point namedPt in _workPart.Points)
                            if(namedPt.Name != "")
                            {
                                if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                    doNotMovePts.Add(namedPt);

                                else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                    doNotMovePts.Add(namedPt);

                                else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                    doNotMovePts.Add(namedPt);
                                else if(namedPt.Name.Contains("BLKORIGIN"))
                                    doNotMovePts.Add(namedPt);
                                else
                                    movePtsHalf.Add(namedPt);
                            }

                        movePtsFull.Add(pointPrototype);
                    }
                    else
                    {
                        foreach (Point namedPt in _workPart.Points)
                            if(namedPt.Name != "")
                            {
                                if(namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                                    doNotMovePts.Add(namedPt);

                                else if(namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                                    doNotMovePts.Add(namedPt);

                                else if(namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                                    doNotMovePts.Add(namedPt);
                                else if(namedPt.Name.Contains("BLKORIGIN"))
                                    movePtsFull.Add(namedPt);
                                else
                                    movePtsHalf.Add(namedPt);
                            }

                        movePtsFull.Add(pointPrototype);
                    }

                    var posXObjs = new List<Line>();
                    var negXObjs = new List<Line>();
                    var posYObjs = new List<Line>();
                    var negYObjs = new List<Line>();
                    var posZObjs = new List<Line>();
                    var negZObjs = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
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

                        if(eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                           eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                        if(eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                           eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                        if(eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                           eLine.Name == "YBASE2") negZObjs.Add(eLine);

                        if(eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" || eLine.Name == "YCEILING1" ||
                           eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                    }

                    var allxAxisLines = new List<Line>();
                    var allyAxisLines = new List<Line>();
                    var allzAxisLines = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if(eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                        if(eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                        if(eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                    }

                    // get the distance from the selected point to the cursor location

                    double[] pointStart =
                        { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

                    var mappedPoint = new double[3];
                    var mappedCursor = new double[3];

                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

                    if(pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                        if(mappedPoint[0] != mappedCursor[0])
                        {
                            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

                            if(xDistance >= _gridSpace)
                            {
                                if(mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

                                _distanceMoved += xDistance;

                                if(pointPrototype.Name == "POSX")
                                {
                                    foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                    foreach (var xAxisLine in allxAxisLines)
                                    {
                                        var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                        var addX = new Point3d(mappedEndPoint.X + xDistance, mappedEndPoint.Y,
                                            mappedEndPoint.Z);
                                        var mappedAddX = MapWcsToAbsolute(addX);
                                        xAxisLine.SetEndPoint(mappedAddX);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), xDistance, "X");
                                    MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

                                    ShowTemporarySizeText();
                                }
                                else
                                {
                                    foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

                                    foreach (var xAxisLine in allxAxisLines)
                                    {
                                        var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                                        var addX = new Point3d(mappedStartPoint.X + xDistance, mappedStartPoint.Y,
                                            mappedStartPoint.Z);
                                        var mappedAddX = MapWcsToAbsolute(addX);
                                        xAxisLine.SetStartPoint(mappedAddX);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), xDistance, "X");
                                    MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

                                    ShowTemporarySizeText();
                                }
                            }
                        }

                    if(pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                        if(mappedPoint[1] != mappedCursor[1])
                        {
                            var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                            if(yDistance >= _gridSpace)
                            {
                                if(mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                                _distanceMoved += yDistance;

                                if(pointPrototype.Name == "POSY")
                                {
                                    foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

                                    foreach (var yAxisLine in allyAxisLines)
                                    {
                                        var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                                        var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + yDistance,
                                            mappedEndPoint.Z);
                                        var mappedAddY = MapWcsToAbsolute(addY);
                                        yAxisLine.SetEndPoint(mappedAddY);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
                                    MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

                                    ShowTemporarySizeText();
                                }
                                else
                                {
                                    foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

                                    foreach (var yAxisLine in allyAxisLines)
                                    {
                                        var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                                        var addY = new Point3d(mappedStartPoint.X, mappedStartPoint.Y + yDistance,
                                            mappedStartPoint.Z);
                                        var mappedAddY = MapWcsToAbsolute(addY);
                                        yAxisLine.SetStartPoint(mappedAddY);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
                                    MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

                                    ShowTemporarySizeText();
                                }
                            }
                        }

                    if(pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                        if(mappedPoint[2] != mappedCursor[2])
                        {
                            var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                            if(zDistance >= _gridSpace)
                            {
                                if(mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                                _distanceMoved += zDistance;

                                if(pointPrototype.Name == "POSZ")
                                {
                                    foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

                                    foreach (var zAxisLine in allzAxisLines)
                                    {
                                        var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                                        var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                                            mappedEndPoint.Z + zDistance);
                                        var mappedAddZ = MapWcsToAbsolute(addZ);
                                        zAxisLine.SetEndPoint(mappedAddZ);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
                                    MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

                                    ShowTemporarySizeText();
                                }
                                else
                                {
                                    foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

                                    foreach (var zAxisLine in allzAxisLines)
                                    {
                                        var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                                        var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                                            mappedStartPoint.Z + zDistance);
                                        var mappedAddZ = MapWcsToAbsolute(addZ);
                                        zAxisLine.SetStartPoint(mappedAddZ);
                                    }

                                    MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
                                    MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

                                    ShowTemporarySizeText();
                                }

                                var mView1 = (ModelingView)_displayPart.Views.WorkView;
                                _displayPart.Views.WorkView.Orient(mView1.Matrix);
                                _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                            }
                        }
                }
                else
                {
                    Point pointPrototype;

                    if(_udoPointHandle.IsOccurrence)
                        pointPrototype = (Point)_udoPointHandle.Prototype;
                    else
                        pointPrototype = _udoPointHandle;

                    var moveAll = new List<NXObject>();

                    foreach (Point namedPts in _workPart.Points)
                        if(namedPts.Name != "")
                            moveAll.Add(namedPts);

                    foreach (var eLine in _edgeRepLines) moveAll.Add(eLine);

                    // get the distance from the selected point to the cursor location

                    double[] pointStart =
                        { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

                    var mappedPoint = new double[3];
                    var mappedCursor = new double[3];

                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

                    if(pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                        if(mappedPoint[0] != mappedCursor[0])
                        {
                            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

                            if(xDistance >= _gridSpace)
                            {
                                if(mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

                                _distanceMoved += xDistance;

                                MoveObjects(moveAll.ToArray(), xDistance, "X");
                            }
                        }

                    if(pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                        if(mappedPoint[1] != mappedCursor[1])
                        {
                            var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                            if(yDistance >= _gridSpace)
                            {
                                if(mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                                _distanceMoved += yDistance;

                                MoveObjects(moveAll.ToArray(), yDistance, "Y");
                            }
                        }

                    if(pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                        if(mappedPoint[2] != mappedCursor[2])
                        {
                            var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                            if(zDistance >= _gridSpace)
                            {
                                if(mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                                _distanceMoved += zDistance;

                                MoveObjects(moveAll.ToArray(), zDistance, "Z");

                                var mView1 = (ModelingView)_displayPart.Views.WorkView;
                                _displayPart.Views.WorkView.Orient(mView1.Matrix);
                                _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                            }
                        }
                }

                double editBlkLength = 0;
                double editBlkWidth = 0;
                double editBlkHeight = 0;

                foreach (var eLine in _edgeRepLines)
                {
                    if(eLine.Name == "XBASE1") editBlkLength = eLine.GetLength();

                    if(eLine.Name == "YBASE1") editBlkWidth = eLine.GetLength();

                    if(eLine.Name == "ZBASE1") editBlkHeight = eLine.GetLength();
                }

                if(_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    ufsession_.Ui.SetPrompt(
                        $"X = {editBlkLength:0.000}  Y = {editBlkWidth:0.000}  Z = {$"{editBlkHeight:0.000}"}  Distance Moved =  {$"{_distanceMoved:0.000}"}");
                }
                else
                {
                    editBlkLength /= 25.4;
                    editBlkWidth /= 25.4;
                    editBlkHeight /= 25.4;

                    var convertDistMoved = _distanceMoved / 25.4;

                    ufsession_.Ui.SetPrompt("X = " + $"{editBlkLength:0.000}" + "  Y = " + $"{editBlkWidth:0.000}" +
                                            "  Z = " + $"{editBlkHeight:0.000}" + "  Distance Moved =  " +
                                            $"{convertDistMoved:0.000}");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }
    }
}