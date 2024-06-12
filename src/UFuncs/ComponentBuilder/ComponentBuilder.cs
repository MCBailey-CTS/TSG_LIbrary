using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Layer;
using NXOpen.Preferences;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;
using Part = NXOpen.Part;
using MoreLinq;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_component_builder)]
    public partial class ComponentBuilder : _UFuncForm
    {
        private const double GridSpace = 1d;

        public const double Tolerance = .0001;
        private static Part _workPart = session_.Parts.Work;
        private static Part _displayPart = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = _displayPart;
        private static bool _isNameReset = true;
        private static bool _isSaveAs;
        private static CtsComponentType _compColor = new CtsComponentType("DARK GREY", CtsComponentColor.DarkGrey);
        private static Component _changeColorComponent;
        private static int _registered;
        private static int _idWorkPartChanged1;
        private static List<CtsAttributes> _compNames = new List<CtsAttributes>();
        private static List<CtsAttributes> _compMaterials = new List<CtsAttributes>();
        private static List<CtsAttributes> _compTolerances = new List<CtsAttributes>();

        public ComponentBuilder()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_displayPart != null)
                WorkPartChanged1(_displayPart);

            Location = Settings.Default.udoComponentBuilderWindowLocation;
            toolTip1.SetToolTip(buttonAquamarine, CtsComponentColor.AquaMarine.ToString());
            toolTip1.SetToolTip(buttonDarkDullGreen, CtsComponentColor.DarkDullGreen.ToString());
            toolTip1.SetToolTip(buttonDarkWeakMagenta, CtsComponentColor.DarkWeakMagenta.ToString());
            toolTip1.SetToolTip(buttonDarkWeakRed, CtsComponentColor.DarkWeakRed.ToString());
            toolTip1.SetToolTip(buttonMedAzureBlue, CtsComponentColor.MediumAzureBlue.ToString());
            toolTip1.SetToolTip(buttonObscureDullGreen, CtsComponentColor.ObscureDullGreen.ToString());
            toolTip1.SetToolTip(buttonPurple, CtsComponentColor.Purple.ToString());
            toolTip1.SetToolTip(buttonDarkDullBlue, CtsComponentColor.DarkDullBlue.ToString());

            var getName = PerformStreamReaderString(FilePath_Ucf, ":DETAIL_TYPE_ATTRIBUTE_NAME:",
                ":END_DETAIL_TYPE_ATTRIBUTE_NAME:");

            var getMaterial = PerformStreamReaderString(FilePath_Ucf, ":MATERIAL_ATTRIBUTE_NAME:",
                ":END_MATERIAL_ATTRIBUTE_NAME:");

            _compNames = PerformStreamReaderList(FilePath_Ucf, ":COMPONENT_NAMES:", ":END_COMPONENT_NAMES:");

            foreach (var cName in _compNames)
                cName.AttrName = getName != string.Empty ? getName : "DETAIL NAME";

            _compMaterials =
                PerformStreamReaderList(FilePath_Ucf, ":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");

            foreach (var cMaterial in _compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _compTolerances =
                PerformStreamReaderList(FilePath_Ucf, ":COMPONENT_TOLERANCES:", ":END_COMPONENT_TOLERANCES:");

            foreach (var cTolerance in _compTolerances)
                cTolerance.AttrName = "TOLERANCE";

            UpdateSessionParts();
            UpdateOriginalParts();
            ResetForm(_workPart);
            LoadGridSizes();


            if (string.IsNullOrEmpty(comboBoxGrid.Text) && !(session_.Parts.Work is null))
                comboBoxGrid.SelectedItem =
                    session_.Parts.Work.PartUnits == BasePart.Units.Inches ? "0.250" : "6.35";

            listBoxMaterial.Enabled = false;
            groupBoxColor.Enabled = false;
            _registered = Startup();
        }

        private void ComboBoxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetWorkPlane(false);

            if (comboBoxGrid.Text != @"0.000")
                SetWorkPlane(true);

            Settings.Default.udoComponentBuilderGridIncrement = comboBoxGrid.Text;
            Settings.Default.Save();
        }


        private void LoadGridSizes()
        {
            comboBoxGrid.Items.Clear();

            if (_displayPart.PartUnits == BasePart.Units.Inches)
                comboBoxGrid.Items.AddRange(new object[]
                {
                    "0.002",
                    "0.03125",
                    "0.0625",
                    "0.125",
                    "0.250",
                    "0.500",
                    "1.000"
                });
            else
                comboBoxGrid.Items.AddRange(new object[]
                {
                    "0.0508",
                    "0.79375",
                    "1.00",
                    "1.5875",
                    "3.175",
                    "5.00",
                    "6.35",
                    "12.7",
                    "25.4"
                });

            foreach (string gridSetting in comboBoxGrid.Items)
            {
                if (gridSetting != Settings.Default.udoComponentBuilderGridIncrement)
                    continue;

                var gridIndex = comboBoxGrid.Items.IndexOf(gridSetting);
                comboBoxGrid.SelectedIndex = gridIndex;
            }
        }

        private void SetWorkPlane(bool snapToGrid)
        {
            UpdateSessionParts();
            session_.__SetWorkPlane(GridSpace, snapToGrid, false);
        }

        public int Startup()
        {
            if (_registered != 0)
                return 0;

            var mForm = this;
            _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(mForm.WorkPartChanged1);
            _registered = 1;
            return 0;
        }

        public void WorkPartChanged1(BasePart p)
        {
            try
            {
                if (!_isSaveAs)
                {
                    UpdateSessionParts();
                    ResetForm(_workPart);
                    LoadGridSizes();
                    listBoxMaterial.Enabled = false;
                    groupBoxColor.Enabled = false;
                }

                if (__work_component_ is null)
                    if (session_.Parts.Work.ComponentAssembly.RootComponent != null)
                        __work_component_ = session_.Parts.Work.ComponentAssembly.RootComponent;
                    else
                        return;

                var ass = Check(__work_component_);

                switch (ass)
                {
                    case AssemblyComponent.Lower:
                        checkBoxUpperComp.Checked = false;
                        break;
                    case AssemblyComponent.Upper:
                        checkBoxUpperComp.Checked = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private static AssemblyComponent Check(Component component)
        {
            while (true)
            {
                if (component is null || component.IsSuppressed)
                    return AssemblyComponent.None;

                var displayName = component.DisplayName.ToUpper();

                if (displayName.Contains("LWR")
                    || displayName.Contains("LSP")
                    || displayName.Contains("LSH")
                    || displayName.Contains("LAD")
                    || displayName.Contains("LFTR"))
                    return AssemblyComponent.Lower;

                if (displayName.Contains("UPR")
                    || displayName.Contains("USP")
                    || displayName.Contains("USH")
                    || displayName.Contains("UAD"))
                    return AssemblyComponent.Upper;

                if (component.Parent is null)
                    return AssemblyComponent.None;

                component = component.Parent;
            }
        }


        private void UpdateSessionButton_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                UpdateFormText();
                ResetForm(_workPart);
                groupBoxColor.Enabled = false;
                changeColorCheckBox.Enabled = true;
                changeColorCheckBox.Checked = false;
                checkBoxBurnout.Checked = false;
                checkBoxBurnout.Enabled = false;
                checkBoxGrind.Checked = false;
                checkBoxGrind.Enabled = false;
                checkBoxBurnDirX.Checked = false;
                checkBoxBurnDirX.Enabled = false;
                checkBoxBurnDirY.Checked = false;
                checkBoxBurnDirY.Enabled = false;
                checkBoxBurnDirZ.Checked = false;
                checkBoxBurnDirZ.Enabled = false;
                comboBoxTolerance.Text = string.Empty;
                comboBoxTolerance.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void TextBoxDetailNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBoxDetailNumber.Text.Length == 0)
                    return;

                var isConverted = int.TryParse(textBoxDetailNumber.Text, out var compName);

                if (!isConverted)
                    return;

                if (chk4Digits.Checked)
                    comboBoxCompName.Enabled = compName > 0 && compName < 10000;
                else
                    comboBoxCompName.Enabled = compName > 0 && compName < 991;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void TextBoxDetailNumber_MouseClick(object sender, MouseEventArgs e)
        {
            textBoxDetailNumber.Clear();
            _isNameReset = true;
        }

        private void ComboBoxCompName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCompName.SelectedIndex != -1)
                listBoxMaterial.Enabled = true;
        }

        private void ListBoxMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxMaterial.SelectedIndex == -1)
                {
                    groupBoxColor.Enabled = false;
                    changeColorCheckBox.Enabled = true;
                    return;
                }

                var flag = listBoxMaterial.Text == "HRS PLT"
                || listBoxMaterial.Text == "4140 PLT"
                || listBoxMaterial.Text == "HRS";

                comboBoxTolerance.SelectedIndex = flag ? 0 : -1;
                checkBoxGrind.Enabled = flag;
                checkBoxGrind.Checked = flag;
                checkBoxBurnDirX.Enabled = flag;
                checkBoxBurnDirY.Enabled = flag;
                checkBoxBurnDirZ.Enabled = flag;
                checkBoxBurnDirZ.Checked = flag;
                comboBoxTolerance.Enabled = flag;
                groupBoxColor.Enabled = true;
                textBoxUserMaterial.Text = string.Empty;
                changeColorCheckBox.Checked = false;
                changeColorCheckBox.Enabled = false;
                buttonAutoUpr.Enabled = true;
                buttonAutoLwr.Enabled = true;
                buttonUprRetAssm.Enabled = true;
                buttonLwrRetAssm.Enabled = true;
                checkBoxBurnout.Enabled = flag;
                checkBoxBurnout.Checked = flag;

                if (!flag)
                    return;

                var boolFlag = listBoxMaterial.Text != "HRS PLT"
                    && listBoxMaterial.Text != "4140 PLT";

                if (listBoxMaterial.Text != "HRS PLT" && listBoxMaterial.Text != "4140 PLT")
                {
                    checkBoxBurnDirZ.Checked = false;
                    checkBoxGrind.Checked = false;
                }

                checkBoxBurnout.Enabled = !boolFlag;
                checkBoxBurnout.Checked = !boolFlag;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void TextBoxUserMaterial_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var flag = textBoxUserMaterial.Text.Length != 0;
                listBoxMaterial.Enabled = !flag;

                if (flag)
                {
                    listBoxMaterial.SelectedIndex = -1;

                    if (!textBoxDetailNumber.Enabled
                        || comboBoxCompName.SelectedIndex == -1
                        || textBoxUserMaterial.Text == string.Empty)
                        return;

                    groupBoxColor.Enabled = true;
                    changeColorCheckBox.Checked = false;
                }
                else
                    groupBoxColor.Enabled = false;

                changeColorCheckBox.Enabled = !flag;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonObscureDullGreen_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("OBSCURE DULL GREEN", CtsComponentColor.ObscureDullGreen);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonAquamarine_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("AQUAMARINE", CtsComponentColor.AquaMarine);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonMedAzureBlue_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("MEDIUM AZURE BLUE", CtsComponentColor.MediumAzureBlue);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonDarkDullGreen_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("DARK DULL GREEN", CtsComponentColor.DarkDullGreen);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonDarkWeakRed_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("DARK WEAK RED", CtsComponentColor.DarkWeakRed);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonDarkWeakMagenta_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("DARK WEAK MAGENTA", CtsComponentColor.DarkWeakMagenta);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonPurple_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("PURPLE", CtsComponentColor.Purple);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ButtonDarkDullBlue_Click(object sender, EventArgs e)
        {
            _compColor = new CtsComponentType("DARK DULL BLUE", CtsComponentColor.DarkDullBlue);
            if (changeColorCheckBox.Checked) ColorComponent();
            else CreateComponent();
        }

        private void ChangeColorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!changeColorCheckBox.Checked)
                {
                    groupBoxColor.Enabled = false;

                    if (textBoxDetailNumber.Text.Length != 0)
                        comboBoxCompName.Enabled = true;

                    return;
                }

                _changeColorComponent = SelectOneComponent("Select Component to Color");

                if (_changeColorComponent is null)
                    return;

                groupBoxColor.Enabled = true;
                comboBoxCompName.SelectedIndex = -1;
                listBoxMaterial.SelectedIndex = -1;
                comboBoxCompName.Enabled = false;
                listBoxMaterial.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void CheckBoxGrind_CheckedChanged(object sender, EventArgs e)
        {
            var flag = checkBoxGrind.Checked;
            comboBoxTolerance.SelectedIndex = flag ? 0 : -1;
            if (!flag) comboBoxTolerance.Text = string.Empty;
            comboBoxTolerance.Enabled = flag;
        }

        private void CheckBoxBurnDirX_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirX.Checked) return;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void CheckBoxBurnDirY_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirY.Checked) return;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void CheckBoxBurnDirZ_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirZ.Checked) return;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirY.Checked = false;
        }

        private void ButtonViewWcs_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            var orientation = coordSystem.Orientation.Element;
            _displayPart.Views.WorkView.Orient(orientation);
        }

        private void ButtonEditBlock_Click(object sender, EventArgs e)
        {
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();
            Close();
            var blockForm = new EditBlockForm();
            blockForm.Show();
        }

        private void ButtonEditConstruction_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDetailNumber.Clear();
                textBoxDetailNumber.Enabled = false;
                comboBoxCompName.Enabled = false;
                buttonEditBlock.Enabled = false;
                changeColorCheckBox.Enabled = false;
                buttonExit.Enabled = false;
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
                UpdateSessionParts();
                UpdateOriginalParts();
                var editComponent = SelectOneComponent("Select Component to edit construction");

                if (editComponent is null)
                    return;

                var assmUnits = _displayPart.PartUnits;
                var compBase = (BasePart)editComponent.Prototype;
                var compUnits = compBase.PartUnits;

                if (compUnits != assmUnits)
                    return;

                using (session_.__UsingSuppressDisplay())
                {
                    __display_part_ = (Part)editComponent.Prototype;

                    using (session_.__UsingDoUpdate("Delete Reference Set"))
                    {
                        var allRefSets = _displayPart.GetAllReferenceSets();

                        foreach (var namedRefSet in allRefSets)
                            if (namedRefSet.Name == "EDIT")
                                _workPart.DeleteReferenceSet(namedRefSet);
                    }

                    using (session_.__UsingDoUpdate("Create New Reference Set"))
                    {
                        var editRefSet = _workPart.CreateReferenceSet();
                        var removeComps = editRefSet.AskAllDirectMembers();
                        editRefSet.RemoveObjectsFromReferenceSet(removeComps);
                        editRefSet.SetAddComponentsAutomatically(false, false);
                        editRefSet.SetName("EDIT");
                        var constructionObjects = new List<NXObject>();

                        for (var i = 1; i < 11; i++)
                            constructionObjects.AddRange(_displayPart.Layers.GetAllObjectsOnLayer(i));

                        constructionObjects.AddRange(
                            from CartesianCoordinateSystem csys in _displayPart.CoordinateSystems
                            where csys.Layer == 254
                            where csys.Name == "EDITCSYS"
                            select csys);

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
                UpdateFormText();
                buttonEditConstruction.Enabled = false;
                buttonEndEditConstruction.Enabled = true;
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
                __work_component_.__Translucency(0);
                _displayPart.Layers.WorkLayer = 1;
                _isNameReset = true;
                textBoxDetailNumber.Enabled = true;
                comboBoxCompName.Enabled = false;
                comboBoxCompName.SelectedIndex = -1;

                using (session_.__UsingDoUpdate("Delete Reference Set"))
                {
                    _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", new[] { __work_component_ });
                    var allRefSets = _workPart.GetAllReferenceSets();

                    foreach (var namedRefSet in allRefSets)
                        if (namedRefSet.Name == "EDIT")
                            _workPart.DeleteReferenceSet(namedRefSet);
                }
                __display_part_ = _originalDisplayPart;
                __work_part_ = _originalWorkPart;
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                buttonEditBlock.Enabled = true;
                changeColorCheckBox.Enabled = true;
                buttonExit.Enabled = true;
                UpdateSessionParts();
                UpdateOriginalParts();
                UpdateFormText();
                ResetForm(_workPart);
                groupBoxColor.Enabled = false;
                changeColorCheckBox.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonAutoLwr_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                checkBoxUpperComp.Checked = false;
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                var bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                {
                    updateSessionButton.PerformClick();
                    return;
                }

                foreach (var selectedBody in bodies)
                {
                    selectedBody.Unhighlight();
                    var isMetric = false;
                    var minCorner = new double[3];
                    var directions = new double[3, 3];
                    var distances = new double[3];
                    ufsession_.Modl.AskBoundingBoxExact(selectedBody.Tag, _displayPart.WCS.CoordinateSystem.Tag, minCorner, directions, distances);
                    isMetric = ConvertUnits(distances);
                    Distance0(distances);
                    MinCorner3(minCorner);

                    if (isMetric)
                        AuotLowrMetric(minCorner, distances);
                    else
                        AutoLwrEnglish(minCorner, distances);
                }

                updateSessionButton.PerformClick();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private void AutoLwrEnglish(double[] minCorner, double[] distances)
        {
            var blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] - 1.25, -1.50);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), "1.50");
        }

        private void AuotLowrMetric(double[] minCorner, double[] distances)
        {
            for (var i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            var blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] - 31.75, -38.1);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "38.1");
        }

        private void ButtonAutoUpr_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                checkBoxUpperComp.Checked = true;
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                var bodies = SelectMultipleBodies();
                if (bodies.Count > 0)
                {
                    foreach (var selectedBody in bodies)
                    {
                        selectedBody.Unhighlight();
                        var isMetric = false;
                        var minCorner = new double[3];
                        var directions = new double[3, 3];
                        var distances = new double[3];
                        ufsession_.Modl.AskBoundingBoxExact(selectedBody.Tag, _displayPart.WCS.CoordinateSystem.Tag,
                            minCorner, directions, distances);
                        isMetric = ConvertUnits(distances);
                        Distance0(distances);
                        MinCorner3(minCorner);

                        if (isMetric)
                            AutoUpperMetric(minCorner, distances);
                        else
                            AutoUpperEnglish(minCorner, distances);
                    }

                    updateSessionButton.PerformClick();
                }

                checkBoxUpperComp.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private void AutoUpperEnglish(double[] minCorner, double[] distances)
        {
            var blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] + 1.25 + distances[1],
                                            3.50);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), "1.50");
        }

        private void AutoUpperMetric(double[] minCorner, double[] distances)
        {
            for (var i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            var blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] + 31.75 + distances[1],
                88.9);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "38.1");
        }

        private void ButtonLwrRetAssm_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                checkBoxUpperComp.Checked = false;

                // set WCS to absolute
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                // select bodies

                var bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                {
                    updateSessionButton.PerformClick();
                    return;
                }

                foreach (var selectedBody in bodies)
                {
                    selectedBody.Unhighlight();

                    // get bounding box info

                    var isMetric = false;
                    var minCorner = new double[3];
                    var directions = new double[3, 3];
                    var distances = new double[3];

                    ufsession_.Modl.AskBoundingBoxExact(selectedBody.Tag, _displayPart.WCS.CoordinateSystem.Tag,
                        minCorner, directions, distances);
                    isMetric = ConvertUnits(distances);
                    Distance0(distances);
                    MinCorner3(minCorner);

                    if (isMetric)
                        LowerRetainerMetric(minCorner, distances);
                    else
                        LowerRetainerEnglish(minCorner, distances);
                }

                updateSessionButton.PerformClick();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void LowerRetainerEnglish(double[] minCorner, double[] distances)
        {
            var selectedName = comboBoxCompName.SelectedIndex;
            var selctedMaterial = listBoxMaterial.SelectedIndex;
            // settings for trim
            var blankCompOrigin = new Point3d(minCorner[0] - .625, minCorner[1] - .625, -3.26);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 1.25).ToString(), (distances[1] + 1.25).ToString(), "3.50");
            //settings for retainer
            var nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;
            var materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "4140")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);
            blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] - 1.25, -3.26);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(), (distances[1] + 2.50).ToString(), "1.375");
            // settings for backing  plate
            nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "BLOCK")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;
            materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "O1 GS")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);
            blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] - 1.25, -3.50);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(), (distances[1] + 2.50).ToString(), ".24");
            comboBoxCompName.SelectedIndex = selectedName;
            listBoxMaterial.SelectedIndex = selctedMaterial;
        }

        private void LowerRetainerMetric(double[] minCorner, double[] distances)
        {
            for (var i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            var selectedName = comboBoxCompName.SelectedIndex;
            var selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            var blankCompOrigin = new Point3d(minCorner[0] - 15.875, minCorner[1] - 15.875, -82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 31.75).ToString(),
                (distances[1] + 31.75).ToString(), "88.9");

            //settings for retainer

            var nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            var materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "4140")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] - 31.75, -82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "31.75");

            // settings for backing  plate

            nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "BLOCK")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "O1 GS")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] - 31.75, -88.9);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "6.096");

            comboBoxCompName.SelectedIndex = selectedName;
            listBoxMaterial.SelectedIndex = selctedMaterial;
        }

        private void ButtonUprRetAssm_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                checkBoxUpperComp.Checked = true;

                // set WCS to absolute            
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                // select bodies

                var bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                    return;

                foreach (var selectedBody in bodies)
                {
                    selectedBody.Unhighlight();

                    // get bounding box info

                    var isMetric = false;
                    var minCorner = new double[3];
                    var directions = new double[3, 3];
                    var distances = new double[3];

                    ufsession_.Modl.AskBoundingBoxExact(selectedBody.Tag, _displayPart.WCS.CoordinateSystem.Tag,
                        minCorner, directions, distances);

                    isMetric = ConvertUnits(distances);
                    Distance0(distances);
                    MinCorner3(minCorner);

                    if (isMetric)
                        UpperRetainerMetric(minCorner, distances);
                    else
                        UpperRetainerEnglish(minCorner, distances);
                }

                updateSessionButton.PerformClick();

                checkBoxUpperComp.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private void UpperRetainerEnglish(double[] minCorner, double[] distances)
        {
            var selectedName = comboBoxCompName.SelectedIndex;
            var selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            var blankCompOrigin = new Point3d(minCorner[0] - .625, minCorner[1] + .625 + distances[1],
                3.26);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 1.25).ToString(),
                (distances[1] + 1.25).ToString(), "3.50");

            //settings for retainer

            var nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            var materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "4140")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] + 1.25 + distances[1],
                3.26);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), "1.25");

            // settings for backing  plate

            nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "BLOCK")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "O1 GS")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] + 1.25 + distances[1],
                3.50);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), ".24");

            comboBoxCompName.SelectedIndex = selectedName;
            listBoxMaterial.SelectedIndex = selctedMaterial;
        }

        private void UpperRetainerMetric(double[] minCorner, double[] distances)
        {
            for (var i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            var selectedName = comboBoxCompName.SelectedIndex;
            var selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            var blankCompOrigin = new Point3d(minCorner[0] - 15.875,
                minCorner[1] + 15.875 + distances[1], 82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 31.75).ToString(),
                (distances[1] + 31.75).ToString(), "88.9");

            //settings for retainer

            var nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            var materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "4140")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] + 31.75 + distances[1],
                82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "31.75");

            // settings for backing  plate

            nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "BLOCK")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "O1 GS")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);

            blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] + 31.75 + distances[1],
                88.9);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "6.096");

            comboBoxCompName.SelectedIndex = selectedName;
            listBoxMaterial.SelectedIndex = selctedMaterial;
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            if (_displayPart is null)
            {
                print_("No display part");
                return;
            }

            var units = new BasePart.Units();
            var assmUnits = _displayPart.PartUnits;

            try
            {
                var compSaveAs = SelectOneComponent("Select Component to SaveAs");

                while (compSaveAs != null)
                {
                    if (!IsNameValid(_workPart))
                    {
                        compSaveAs = null;
                        _isNameReset = false;
                        _isSaveAs = false;
                        break;
                    }

                    var isNumberValid = FormatDetailNumber();

                    var compName = textBoxDetailNumber.Text;

                    if (!isNumberValid)
                    {
                        compSaveAs = null;
                        _isNameReset = false;
                        _isSaveAs = false;
                        break;
                    }

                    if (!IsSaveAllowed(compSaveAs))
                    {
                        MessageBox.Show(
                            $"Save As is not allowed on this component {compSaveAs.DisplayName}");
                        compSaveAs = null;
                        _isNameReset = false;
                        _isSaveAs = false;
                        break;
                    }

                    var selectedPart = (Part)compSaveAs.Prototype;
                    units = selectedPart.PartUnits;

                    if (units == assmUnits)
                    {
                        SaveAsSameUnits(compSaveAs, compName);

                        compSaveAs = SelectOneComponent("Select Component to SaveAs");
                    }
                    else
                    {
                        SaveAsDiffUnits(compSaveAs, compName, selectedPart);

                        compSaveAs = SelectOneComponent("Select Component to SaveAs");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();

                if (units != assmUnits)
                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);

                session_.Parts.SetWork(_originalWorkPart);
                UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error, ex.Message);
                UpdateSessionParts();
                UpdateOriginalParts();
            }
            finally
            {

                try
                {
                    ResetForm(_workPart);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void SaveAsDiffUnits(Component compSaveAs, string compName, Part selectedPart)
        {
            _isSaveAs = true;

            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
            session_.Parts.SetDisplay(selectedPart, false, false, out var partLoadStatus1);
            partLoadStatus1.Dispose();
            UpdateSessionParts();

            var indexOf = _originalWorkPart.FullPath.LastIndexOf("-");
            var fullName = _originalWorkPart.FullPath.Remove(indexOf + 1);
            var saveAs = $"{fullName}{compName}.prt";

            var partSaveStatus1 = _workPart.SaveAs(saveAs);
            partSaveStatus1.Dispose();

            // Get body ref set of original work part

            var cycleRefSet = NXOpen.Tag.Null;
            var bodyRefSet = NXOpen.Tag.Null;

            do
            {
                ufsession_.Obj.CycleObjsInPart(_originalWorkPart.Tag, UF_reference_set_type,
                    ref cycleRefSet);
                if (cycleRefSet == NXOpen.Tag.Null)
                    break;
                ufsession_.Obj.AskName(cycleRefSet, out var name);

                if (name == "BODY")
                    bodyRefSet = cycleRefSet;
            } while (cycleRefSet != NXOpen.Tag.Null);

            var saveAsPart = (Part)compSaveAs.Prototype;
            ufsession_.Assem.AskOccsOfPart(_originalWorkPart.Tag, saveAsPart.Tag,
                out var partOccs);
            foreach (var occurrence in partOccs)
            {
                ufsession_.Obj.SetName(occurrence, compName);
                var partInstance = ufsession_.Assem.AskInstOfPartOcc(occurrence);
                ufsession_.Assem.RenameInstance(partInstance, compName);
            }

            if (bodyRefSet != NXOpen.Tag.Null)
                ufsession_.Assem.AddRefSetMembers(bodyRefSet, partOccs.Length, partOccs);

            // set attribute for "DETAIL NUMBER" to new component name

            foreach (var attr in _workPart.GetUserAttributes())
                if (attr.Title == "DETAIL NUMBER")
                    _workPart.SetUserAttribute(attr.Title, -1, compName,
                        NXOpen.Update.Option.Now);

            session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                out var partLoadStatus2);
            session_.Parts.SetWork(_originalWorkPart);
            partLoadStatus2.Dispose();
            UpdateSessionParts();
            UpdateOriginalParts();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            _isNameReset = false;
        }

        private void SaveAsSameUnits(Component compSaveAs, string compName)
        {
            _isSaveAs = true;

            session_.Parts.SetWork((Part)compSaveAs.Prototype);
            UpdateSessionParts();

            var indexOf = _originalWorkPart.FullPath.LastIndexOf("-");
            var fullName = _originalWorkPart.FullPath.Remove(indexOf + 1);
            var saveAs = $"{fullName}{compName}.prt";

            var partSaveStatus1 = _workPart.SaveAs(saveAs);
            partSaveStatus1.Dispose();

            // Get body ref set of original work part

            var cycleRefSet = NXOpen.Tag.Null;
            var bodyRefSet = NXOpen.Tag.Null;

            do
            {
                ufsession_.Obj.CycleObjsInPart(_originalWorkPart.Tag, UF_reference_set_type,
                    ref cycleRefSet);
                if (cycleRefSet == NXOpen.Tag.Null)
                    break;
                ufsession_.Obj.AskName(cycleRefSet, out var name);

                if (name == "BODY")
                    bodyRefSet = cycleRefSet;
            } while (cycleRefSet != NXOpen.Tag.Null);

            // Change name of all occurrences and add to parent assembly "BODY" ref set

            var saveAsPart = (Part)compSaveAs.Prototype;
            ufsession_.Assem.AskOccsOfPart(_originalWorkPart.Tag, saveAsPart.Tag,
                out var partOccs);
            foreach (var occurrence in partOccs)
            {
                ufsession_.Obj.SetName(occurrence, compName);
                var partInstance = ufsession_.Assem.AskInstOfPartOcc(occurrence);
                ufsession_.Assem.RenameInstance(partInstance, compName);
            }

            if (bodyRefSet != NXOpen.Tag.Null)
                ufsession_.Assem.AddRefSetMembers(bodyRefSet, partOccs.Length, partOccs);

            // set attribute for "DETAIL NUMBER" to new component name

            foreach (var attr in _workPart.GetUserAttributes())
                if (attr.Title == "DETAIL NUMBER")
                    _workPart.SetUserAttribute(attr.Title, -1, compName,
                        NXOpen.Update.Option.Now);

            session_.Parts.SetWork(_originalWorkPart);

            UpdateSessionParts();

            UpdateOriginalParts();

            _isNameReset = false;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Copy Component");
                var copyComponent = SelectOneComponent("Select Component to Copy");

                if (copyComponent is null)
                    return;

                var part = (Part)copyComponent.Prototype;
                var next = 0;
                var originalPath = part.FullPath;
                var copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 4)}({next}).prt";

                while (File.Exists(copyPath))
                {
                    next += 1;
                    copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 4)}({next}).prt";
                }

                var indexOf = _workPart.FullPath.LastIndexOf("\\");
                var compare1 = _workPart.FullPath.Remove(indexOf + 1);
                var compare2 = copyPath.Remove(indexOf + 1);

                if (compare1 != compare2)
                {
                    MessageBox.Show(
                        "The selected component is not from the work part directory. \n                   Save As before making a copy.");
                    return;
                }

                if (!File.Exists(originalPath))
                {
                    MessageBox.Show("Save the original file to the assembly before making a copy");
                    return;
                }

                File.Copy(originalPath, copyPath);
                var basePart1 = session_.Parts.OpenBase(copyPath, out var partLoadStatus1);
                partLoadStatus1.Dispose();
                var partToAdd = (Part)basePart1;
                copyComponent.GetPosition(out var origin, out var orientation);
                var layer = copyComponent.Layer;

                _workPart.ComponentAssembly.AddComponent(partToAdd, "BODY", copyComponent.DisplayName,
                    origin, orientation, layer,
                    out var partLoadStatus2);

                partLoadStatus2.Dispose();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Close();
        }

        private void CreateComponent()
        {
            try
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Create Component");
                try
                {
                    ufsession_.Ui.AskInfoUnits(out var infoUnits);

                    var dispUnits = (Part.Units)_displayPart.PartUnits;

                    if (dispUnits == Part.Units.Millimeters)
                    {
                        _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.GMmNDegC);
                        _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                            .GMmNDegC);
                    }
                    else
                    {
                        _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.LbmInLbfDegF);
                        _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                            .LbmInLbfDegF);
                    }

                    //NXOpen.Part.Units dispUnits = (NXOpen.Part.Units)_displayPart.PartUnits;
                    //if (infoUnits == UF_UI_POUNDS_INCHES && dispUnits == NXOpen.Part.Units.Inches ||
                    //    infoUnits == UF_UI_KILOS_MILLIMETERS && dispUnits == NXOpen.Part.Units.Millimeters)
                    //{
                    var userMaterial = textBoxUserMaterial.Text;
                    var compNameIndex = comboBoxCompName.SelectedIndex;
                    var compMaterialIndex = listBoxMaterial.SelectedIndex;
                    var isUpper = checkBoxUpperComp.Checked;
                    var isBurn = checkBoxBurnout.Checked;
                    var isGrind = checkBoxGrind.Checked;
                    var isDirX = checkBoxBurnDirX.Checked;
                    var isDirY = checkBoxBurnDirY.Checked;
                    var isDirZ = checkBoxBurnDirZ.Checked;
                    var toleranceIndex = comboBoxTolerance.SelectedIndex;
                    var selectedName = (CtsAttributes)comboBoxCompName.SelectedItem;
                    var selectecMaterial = new CtsAttributes();
                    if (listBoxMaterial.SelectedIndex == -1)
                    {
                        selectecMaterial.AttrName = "MATERIAL";
                        selectecMaterial.AttrValue = textBoxUserMaterial.Text;
                    }
                    else
                    {
                        selectecMaterial = (CtsAttributes)listBoxMaterial.SelectedItem;
                    }

                    //var numberIsValid = FormatDetailNumber();
                    if (true)
                    {
                        UpdateSessionParts();
                        UpdateOriginalParts();
                        var prevOrigin = _displayPart.WCS.CoordinateSystem.Origin;
                        var prevOrientation = _displayPart.WCS.CoordinateSystem.Orientation.Element;
                        _displayPart.WCS.Visibility = false;
                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                        var uprCompOrigin = new Point3d();
                        var uprCompOrientation = new Matrix3x3();
                        if (checkBoxUpperComp.Checked)
                        {
                            _displayPart.WCS.Rotate(WCS.Axis.XAxis, 180);
                            if (dispUnits == Part.Units.Inches)
                            {
                                uprCompOrigin.X = _Point3dOrigin.X;
                                uprCompOrigin.Y = _Point3dOrigin.Y;
                                uprCompOrigin.Z = _Point3dOrigin.Z + 3.50;
                            }
                            else
                            {
                                uprCompOrigin.X = _Point3dOrigin.X;
                                uprCompOrigin.Y = _Point3dOrigin.Y;
                                uprCompOrigin.Z = _Point3dOrigin.Z + 88.9;
                            }

                            uprCompOrientation = _displayPart.WCS.CoordinateSystem.Orientation.Element;
                        }

                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                        _displayPart.Layers.WorkLayer = 1;
                        var blockFeatureBuilder1 = _workPart.Features.CreateBlockFeatureBuilder(null);
                        blockFeatureBuilder1.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                        var targetBodies1 = new Body[1];
                        targetBodies1[0] = null;
                        blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);
                        blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                        Point3d originPoint1;
                        if (checkBoxUpperComp.Checked)
                        {
                            var xAxis = new Vector3d(uprCompOrientation.Xx, uprCompOrientation.Xy,
                                uprCompOrientation.Xz);
                            var yAxis = new Vector3d(uprCompOrientation.Yx, uprCompOrientation.Yy,
                                uprCompOrientation.Yz);
                            originPoint1 = uprCompOrigin;
                            blockFeatureBuilder1.SetOrientation(xAxis, yAxis);
                        }
                        else if (dispUnits == Part.Units.Inches)
                        {
                            var lwrCompOrigin = new Point3d(_displayPart.WCS.CoordinateSystem.Origin.X,
                                _displayPart.WCS.CoordinateSystem.Origin.Y,
                                _displayPart.WCS.CoordinateSystem.Origin.Z - 1.50);
                            originPoint1 = lwrCompOrigin;
                        }
                        else
                        {
                            var lwrCompOrigin = new Point3d(_displayPart.WCS.CoordinateSystem.Origin.X,
                                _displayPart.WCS.CoordinateSystem.Origin.Y,
                                _displayPart.WCS.CoordinateSystem.Origin.Z - 38.1);
                            originPoint1 = lwrCompOrigin;
                        }

                        if (dispUnits == Part.Units.Inches)
                            blockFeatureBuilder1.SetOriginAndLengths(originPoint1, "4", "4", "1.5");
                        else
                            blockFeatureBuilder1.SetOriginAndLengths(originPoint1, "101.6", "101.6", "38.1");
                        blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, null);
                        var feature1 = blockFeatureBuilder1.CommitFeature();
                        feature1.SetName("DYNAMIC BLOCK");
                        blockFeatureBuilder1.Destroy();
                        _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();
                        session_.SetUndoMark(Session.MarkVisibility.Visible, "Create New Component");
                        var fileNew1 = session_.Parts.FileNew();
                        fileNew1.TemplateFileName = "Blank";
#pragma warning disable CS0618
                        fileNew1.Application = FileNewApplication.Gateway;
#pragma warning restore CS0618
                        fileNew1.Units = (Part.Units)_displayPart.PartUnits;
                        var indexBslash = _workPart.FullPath.LastIndexOf("\\");
                        var indexDash = _workPart.FullPath.LastIndexOf("-");
                        var filePath = _workPart.FullPath.Remove(indexBslash + 1);
                        var assmName = _workPart.FullPath.Substring(indexBslash + 1, indexDash - 1 - indexBslash);
                        var fileName = textBoxDetailNumber.Text;
                        fileNew1.NewFileName = filePath + /*"\\" +*/ assmName + "-" + fileName + ".prt";
                        fileNew1.MasterFileName = "";
                        fileNew1.UseBlankTemplate = true;
                        fileNew1.MakeDisplayedPart = false;
                        var createNewComponentBuilder1 = _workPart.AssemblyManager.CreateNewComponentBuilder();
                        createNewComponentBuilder1.ReferenceSet =
                            CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;
                        createNewComponentBuilder1.ReferenceSetName = "Entire Part";
                        createNewComponentBuilder1.LayerOption =
                            CreateNewComponentBuilder.ComponentLayerOptionType.Work;
                        createNewComponentBuilder1.NewComponentName = fileName;
                        createNewComponentBuilder1.ComponentOrigin =
                            CreateNewComponentBuilder.ComponentOriginType.Absolute;
                        var blockFromFeature = (Block)feature1;
                        var blockBody = blockFromFeature.GetBodies();
                        var body1 = blockBody[0];
                        createNewComponentBuilder1.ObjectForNewComponent.Add(body1);
                        createNewComponentBuilder1.NewFile = fileNew1;
                        createNewComponentBuilder1.Commit();
                        createNewComponentBuilder1.Destroy();
                        var assmComponents = _workPart.ComponentAssembly.RootComponent.GetChildren();
                        var basePart1 = (BasePart)assmComponents[0].Prototype;
                        session_.Parts.SetDisplay(basePart1, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();
                        UpdateSessionParts();
                        checkBoxUpperComp.Checked = isUpper;
                        checkBoxBurnout.Checked = isBurn;
                        checkBoxGrind.Checked = isGrind;
                        checkBoxBurnDirX.Checked = isDirX;
                        checkBoxBurnDirY.Checked = isDirY;
                        checkBoxBurnDirZ.Checked = isDirZ;
                        comboBoxTolerance.SelectedIndex = toleranceIndex;
                        var makeExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

                        var stringUnit = _displayPart.PartUnits == BasePart.Units.Inches ? "Inch" : "MilliMeter";
                        var unit1 = _workPart.UnitCollection.FindObject(stringUnit);
                        _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                        _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                        _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);

                        _workPart.Expressions.CreateExpression("String",
                            checkBoxBurnout.Checked ? "Burnout=\"yes\"" : "Burnout=\"no\"");
                        if (checkBoxGrind.Checked)
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                            if (comboBoxTolerance.SelectedIndex != -1)
                                _workPart.Expressions.CreateExpression("String",
                                    "GrindTolerance=\"" + comboBoxTolerance.Text + "\"");
                            else
                                _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }
                        else
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");

                            _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }

                        if (checkBoxBurnDirX.Checked) _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
                        else if (checkBoxBurnDirY.Checked)
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
                        else if (checkBoxBurnDirZ.Checked)
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
                        else _workPart.Expressions.CreateExpression("String", "BurnDir=\"none\"");
                        session_.UpdateManager.DoUpdate(makeExpressions);
                        _workPart.Layers.WorkLayer = 15;
                        Feature nullFeaturesFeature2 = null;
                        var extractFaceBuilder1 = _workPart.Features.CreateExtractFaceBuilder(nullFeaturesFeature2);
                        using (session_.__UsingBuilderDestroyer(extractFaceBuilder1))
                        {
                            extractFaceBuilder1.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;
                            extractFaceBuilder1.Type = ExtractFaceBuilder.ExtractType.Body;
                            extractFaceBuilder1.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;
                            extractFaceBuilder1.FixAtCurrentTimestamp = true;
                            Body body2 = null;
                            foreach (Body blkBody in _workPart.Bodies)
                                if (blkBody.Layer == 1)
                                    body2 = blkBody;
#pragma warning disable CS0618
                            extractFaceBuilder1.BodyToExtract.Add(body2);
#pragma warning restore CS0618
                            extractFaceBuilder1.Commit();
                        }

                        CreateRefSetsCategories();
                        _workPart.Layers.WorkLayer = 1;
                        _workPart.Layers.SetState(15, State.Hidden);
                        try
                        {
                            CreateAutoSizeUdo();
                        }
                        catch (NXException ex) when (ex.ErrorCode == 1535022)
                        {
                            ex.__PrintException();
                        }

                        foreach (Body body in _workPart.Bodies)
                        {
                            if (body.Layer == 1)
                            {
                                body.Color = _compColor.ComponentColor;
                                ColorBaseFace(body, isUpper);
                            }

                            if (body.Layer == 15)
                                body.Color = (int)CtsSubtoolColor.Yellow;
                        }

                        try
                        {
                            _workPart.SetUserAttribute(selectedName.AttrName, -1, selectedName.AttrValue,
                                NXOpen.Update.Option.Later);
                            _workPart.SetUserAttribute(selectecMaterial.AttrName, -1, selectecMaterial.AttrValue,
                                NXOpen.Update.Option.Later);
                            _workPart.SetUserAttribute("BREAK", -1, "###", NXOpen.Update.Option.Later);
                            _workPart.SetUserAttribute("EC", -1, "???", NXOpen.Update.Option.Later);
                        }
                        catch (NXException ex)
                        {
                            UI.GetUI().NXMessageBox.Show("Caught exception : Set Attributes - Name, Material",
                                NXMessageBox.DialogType.Error, ex.Message);
                            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                            ufsession_.Disp.RegenerateDisplay();
                        }

                        _workPart.Layers.SetState(3, State.WorkLayer);
                        _workPart.Preferences.ObjectPreferences.SetColor(PartObject.ObjectType.Solidbody,
                            _compColor.ComponentColor);
                        session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                        partLoadStatus2.Dispose();
                        UpdateSessionParts();
                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        UpdateSessionParts();
                        _displayPart.WCS.SetOriginAndMatrix(prevOrigin, prevOrientation);
                        _displayPart.WCS.Visibility = true;
                        assmComponents[0].RedisplayObject();
                        session_.Parts.Work.ComponentAssembly.ReplaceReferenceSet(assmComponents[0], "BODY");
                        var cycleRefSet = NXOpen.Tag.Null;
                        do
                        {
                            ufsession_.Obj.CycleObjsInPart(_workPart.Tag, UF_reference_set_type, ref cycleRefSet);
                            if (cycleRefSet == NXOpen.Tag.Null)
                                break;
                            ufsession_.Obj.AskName(cycleRefSet, out var name);
                            if (name != "BODY") continue;
                            Tag[] refSetMembers = { assmComponents[0].Tag };
                            ufsession_.Assem.AddRefSetMembers(cycleRefSet, refSetMembers.Length, refSetMembers);
                            ufsession_.Assem.ReplaceRefset(refSetMembers.Length, refSetMembers, "BODY");
                        } while (cycleRefSet != NXOpen.Tag.Null);
                    }

                    UpdateFormText();
                    if (userMaterial.Length != 0)
                    {
                        textBoxUserMaterial.Text = userMaterial;
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = -1;
                        listBoxMaterial.Enabled = false;
                        groupBoxColor.Enabled = true;
                    }
                    else
                    {
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = compMaterialIndex;
                        listBoxMaterial.Enabled = true;
                        groupBoxColor.Enabled = true;
                        checkBoxBurnout.Checked = isBurn;
                        checkBoxGrind.Checked = isGrind;
                        checkBoxBurnDirX.Checked = isDirX;
                        checkBoxBurnDirY.Checked = isDirY;
                        checkBoxBurnDirZ.Checked = isDirZ;
                        comboBoxTolerance.SelectedIndex = toleranceIndex;
                    }
                    //}
                    //else
                    //    NXOpen.UI.GetUI()
                    //        .NXMessageBox.Show("Caught exception in Create Component", NXOpen.NXMessageBox.DialogType.Error,
                    //            "Analysis --> Units does not match the session units");
                }
                catch (NXException ex)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    UpdateSessionParts();
                    UI.GetUI().NXMessageBox.Show("Caught exception in Create Component", NXMessageBox.DialogType.Error,
                        ex.Message);
                    session_.UndoToLastVisibleMark();
                    var delObjects = _workPart.Features.Cast<Feature>()
                        .Where(delFeature => delFeature.Name == "DYNAMIC BLOCK").Cast<NXObject>().ToList();
                    delObjects.AddRange(_workPart.Lines.Cast<Line>().Where(nLine => nLine.Name != string.Empty));
                    delObjects.AddRange(_workPart.Points.Cast<Point>().Where(nPoint => nPoint.Name != string.Empty));
                    if (delObjects.Count > 0) DeleteNxObjects(delObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void CreateComponent(Point3d bodyOrigin, string length, string width, string height)
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "Create Component");
            try
            {
                try
                {
                    //bool areUnitsEqual = false;

                    ufsession_.Ui.AskInfoUnits(out var infoUnits);
                    var dispUnits = (Part.Units)_displayPart.PartUnits;

                    if (dispUnits == Part.Units.Millimeters)
                    {
                        _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.GMmNDegC);
                        _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                            .GMmNDegC);
                    }
                    else
                    {
                        _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.LbmInLbfDegF);
                        _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                            .LbmInLbfDegF);
                    }

                    //if (!areUnitsEqual) return;
                    var userMaterial = textBoxUserMaterial.Text;
                    var compNameIndex = comboBoxCompName.SelectedIndex;
                    var compMaterialIndex = listBoxMaterial.SelectedIndex;
                    var isUpper = checkBoxUpperComp.Checked;
                    var isBurn = checkBoxBurnout.Checked;
                    var isGrind = checkBoxGrind.Checked;
                    var isDirX = checkBoxBurnDirX.Checked;
                    var isDirY = checkBoxBurnDirY.Checked;
                    var isDirZ = checkBoxBurnDirZ.Checked;
                    var toleranceIndex = comboBoxTolerance.SelectedIndex;

                    var selectedName = (CtsAttributes)comboBoxCompName.SelectedItem;
                    var selectecMaterial = new CtsAttributes();

                    if (listBoxMaterial.SelectedIndex != -1)
                    {
                        selectecMaterial = (CtsAttributes)listBoxMaterial.SelectedItem;
                    }
                    else
                    {
                        selectecMaterial.AttrName = "MATERIAL";
                        selectecMaterial.AttrValue = textBoxUserMaterial.Text;
                    }

                    var numberIsValid = FormatDetailNumber();
                    if (numberIsValid)
                    {
                        UpdateSessionParts();
                        UpdateOriginalParts();

                        _displayPart.WCS.Visibility = false;
                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                        var uprCompOrigin = new Point3d();
                        var uprCompOrientation = new Matrix3x3();

                        if (checkBoxUpperComp.Checked)
                        {
                            _displayPart.WCS.Rotate(WCS.Axis.XAxis, 180);

                            uprCompOrigin.X = bodyOrigin.X;
                            uprCompOrigin.Y = bodyOrigin.Y;
                            uprCompOrigin.Z = bodyOrigin.Z;

                            uprCompOrientation = _displayPart.WCS.CoordinateSystem.Orientation.Element;
                        }

                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                        // create block feature 

                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                        _displayPart.Layers.WorkLayer = 1;

                        var blockFeatureBuilder1 = _workPart.Features.CreateBlockFeatureBuilder(null);


                        blockFeatureBuilder1.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                        var targetBodies1 = new Body[1];
                        Body nullBody = null;
                        targetBodies1[0] = null;
                        blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);

                        blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                        Point3d originPoint1;

                        if (checkBoxUpperComp.Checked)
                        {
                            var xAxis = new Vector3d(uprCompOrientation.Xx, uprCompOrientation.Xy,
                                uprCompOrientation.Xz);
                            var yAxis = new Vector3d(uprCompOrientation.Yx, uprCompOrientation.Yy,
                                uprCompOrientation.Yz);

                            originPoint1 = uprCompOrigin;
                            blockFeatureBuilder1.SetOrientation(xAxis, yAxis);
                        }
                        else
                        {
                            var lwrCompOrigin = new Point3d(bodyOrigin.X, bodyOrigin.Y, bodyOrigin.Z);
                            originPoint1 = lwrCompOrigin;
                        }

                        blockFeatureBuilder1.SetOriginAndLengths(originPoint1, length, width, height);

                        blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);

                        var feature1 = blockFeatureBuilder1.CommitFeature();

                        feature1.SetName("DYNAMIC BLOCK");

                        blockFeatureBuilder1.Destroy();

                        _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                        // create component from block

                        session_.SetUndoMark(Session.MarkVisibility.Visible, "Create New Component");

                        FileNew fileNew1;
                        fileNew1 = session_.Parts.FileNew();

                        fileNew1.TemplateFileName = "Blank";
#pragma warning disable CS0618
                        fileNew1.Application = FileNewApplication.Gateway;
#pragma warning restore CS0618
                        fileNew1.Units = (Part.Units)_displayPart.PartUnits;

                        // get file name information

                        var indexBslash = _workPart.FullPath.LastIndexOf("\\");
                        var indexDash = _workPart.FullPath.LastIndexOf("-");
                        var filePath = _workPart.FullPath.Remove(indexBslash + 1);
                        var assmName = _workPart.FullPath.Substring(indexBslash + 1, indexDash - 1 - indexBslash);
                        var fileName = textBoxDetailNumber.Text;

                        fileNew1.NewFileName = $"{filePath}\\{assmName}-{fileName}.prt";

                        fileNew1.MasterFileName = "";

                        fileNew1.UseBlankTemplate = true;

                        fileNew1.MakeDisplayedPart = false;

                        var createNewComponentBuilder1 = _workPart.AssemblyManager.CreateNewComponentBuilder();

                        createNewComponentBuilder1.ReferenceSet =
                            CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;

                        createNewComponentBuilder1.ReferenceSetName = "Entire Part";

                        createNewComponentBuilder1.LayerOption =
                            CreateNewComponentBuilder.ComponentLayerOptionType.Work;

                        createNewComponentBuilder1.NewComponentName = fileName;

                        createNewComponentBuilder1.ComponentOrigin =
                            CreateNewComponentBuilder.ComponentOriginType.Absolute;

                        // get the body from the block that has been created

                        var blockFromFeature = (Block)feature1;

                        var blockBody = blockFromFeature.GetBodies();

                        var body1 = blockBody[0];

                        bool added1;
                        added1 = createNewComponentBuilder1.ObjectForNewComponent.Add(body1);

                        createNewComponentBuilder1.NewFile = fileNew1;

                        createNewComponentBuilder1.Commit();

                        createNewComponentBuilder1.Destroy();

                        var assmComponents = _workPart.ComponentAssembly.RootComponent.GetChildren();

                        var basePart1 = (BasePart)assmComponents[0].Prototype;
                        session_.Parts.SetDisplay(basePart1, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();

                        UpdateSessionParts();

                        // create component description attributes

                        checkBoxUpperComp.Checked = isUpper;
                        checkBoxBurnout.Checked = isBurn;
                        checkBoxGrind.Checked = isGrind;
                        checkBoxBurnDirX.Checked = isDirX;
                        checkBoxBurnDirY.Checked = isDirY;
                        checkBoxBurnDirZ.Checked = isDirZ;
                        comboBoxTolerance.SelectedIndex = toleranceIndex;

                        var makeExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

                        if (_displayPart.PartUnits == BasePart.Units.Inches)
                        {
                            var unit1 = _workPart.UnitCollection.FindObject("Inch");

                            _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
                        }
                        else
                        {
                            var unit1 = _workPart.UnitCollection.FindObject("MilliMeter");

                            _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
                        }

                        _workPart.Expressions.CreateExpression("String",
                            checkBoxBurnout.Checked ? "Burnout=\"yes\"" : "Burnout=\"no\"");
                        if (checkBoxGrind.Checked)
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                            if (comboBoxTolerance.SelectedIndex != -1)
                                _workPart.Expressions.CreateExpression("String",
                                    $"GrindTolerance=\"{comboBoxTolerance.Text}\"");
                            else
                                _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }
                        else
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");

                            _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }

                        if (checkBoxBurnDirX.Checked)
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
                        else if (checkBoxBurnDirY.Checked)
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
                        else if (checkBoxBurnDirZ.Checked)
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
                        else
                            _workPart.Expressions.CreateExpression("String", "BurnDir=\"none\"");

                        session_.UpdateManager.DoUpdate(makeExpressions);

                        // create extracted body with timestamp

                        _workPart.Layers.WorkLayer = 15;

                        Feature nullFeatures_Feature2 = null;

                        ExtractFaceBuilder extractFaceBuilder1;
                        extractFaceBuilder1 = _workPart.Features.CreateExtractFaceBuilder(nullFeatures_Feature2);

                        extractFaceBuilder1.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;

                        extractFaceBuilder1.Type = ExtractFaceBuilder.ExtractType.Body;

                        extractFaceBuilder1.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;

                        extractFaceBuilder1.FixAtCurrentTimestamp = true;

                        Body body2 = null;

                        foreach (Body blkBody in _workPart.Bodies)
                            if (blkBody.Layer == 1)
                                body2 = blkBody;

#pragma warning disable CS0618
                        extractFaceBuilder1.BodyToExtract.Add(body2);
#pragma warning restore CS0618
                        extractFaceBuilder1.Commit();

                        extractFaceBuilder1.Destroy();

                        // create categories and ref sets

                        CreateRefSetsCategories();

                        _workPart.Layers.WorkLayer = 1;
                        _workPart.Layers.SetState(15, State.Hidden);

                        // create auto size UDO

                        try
                        {
                            CreateAutoSizeUdo();
                        }
                        catch (NXException ex) when (ex.ErrorCode == 1535022)
                        {
                            ex.__PrintException();
                        }

                        // update component solid body color

                        foreach (Body body in _workPart.Bodies)
                        {
                            if (body.Layer == 1)
                            {
                                body.Color = _compColor.ComponentColor;
                                ColorBaseFace(body, isUpper);
                            }

                            if (body.Layer == 15)
                                body.Color = (int)CtsSubtoolColor.Yellow;
                        }

                        //workPart.SetAttribute(selectedName.AttrName, selectedName.AttrValue);

                        //workPart.SetAttribute(selectecMaterial.AttrName, selectecMaterial.AttrValue);
                        _workPart.SetUserAttribute(selectedName.AttrName, -1, selectedName.AttrValue,
                            NXOpen.Update.Option.Later);
                        _workPart.SetUserAttribute(selectecMaterial.AttrName, -1, selectecMaterial.AttrValue,
                            NXOpen.Update.Option.Later);
                        _workPart.SetUserAttribute("BREAK", -1, "###", NXOpen.Update.Option.Later);
                        _workPart.SetUserAttribute("EC", -1, "???", NXOpen.Update.Option.Later);

                        _workPart.Layers.SetState(3, State.WorkLayer);
                        _workPart.Preferences.ObjectPreferences.SetColor(PartObject.ObjectType.Solidbody,
                            _compColor.ComponentColor);

                        session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                        partLoadStatus2.Dispose();

                        UpdateSessionParts();

                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        UpdateSessionParts();

                        _displayPart.WCS.Visibility = true;
                        assmComponents[0].RedisplayObject();

                        session_.Parts.Work.ComponentAssembly.ReplaceReferenceSet(assmComponents[0], "BODY");

                        var cycleRefSet = NXOpen.Tag.Null;

                        do
                        {
                            ufsession_.Obj.CycleObjsInPart(_workPart.Tag, UF_reference_set_type, ref cycleRefSet);
                            if (cycleRefSet == NXOpen.Tag.Null)
                                break;
                            ufsession_.Obj.AskName(cycleRefSet, out var name);

                            if (name != "BODY") continue;
                            Tag[] refSetMembers = { assmComponents[0].Tag };
                            ufsession_.Assem.AddRefSetMembers(cycleRefSet, refSetMembers.Length, refSetMembers);
                            ufsession_.Assem.ReplaceRefset(refSetMembers.Length, refSetMembers, "BODY");
                        } while (cycleRefSet != NXOpen.Tag.Null);
                    }

                    UpdateFormText();

                    if (userMaterial.Length != 0)
                    {
                        textBoxUserMaterial.Text = userMaterial;
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = -1;
                        listBoxMaterial.Enabled = false;
                        groupBoxColor.Enabled = true;
                    }
                    else
                    {
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = compMaterialIndex;
                        listBoxMaterial.Enabled = true;
                        groupBoxColor.Enabled = true;
                        checkBoxBurnout.Checked = isBurn;
                        checkBoxGrind.Checked = isGrind;
                        checkBoxBurnDirX.Checked = isDirX;
                        checkBoxBurnDirY.Checked = isDirY;
                        checkBoxBurnDirZ.Checked = isDirZ;
                        comboBoxTolerance.SelectedIndex = toleranceIndex;
                    }
                }
                catch (NXException ex)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    session_.Parts.SetWork(_originalWorkPart);
                    UpdateSessionParts();
                    UI.GetUI().NXMessageBox.Show("Caught exception in Create Component", NXMessageBox.DialogType.Error,
                        ex.Message);
                    session_.UndoToLastVisibleMark();
                    var delObjects = _workPart.Features.Cast<Feature>()
                        .Where(delFeature => delFeature.Name == "DYNAMIC BLOCK").Cast<NXObject>().ToList();
                    delObjects.AddRange(_workPart.Lines.Cast<Line>().Where(nLine => nLine.Name != string.Empty));
                    delObjects.AddRange(_workPart.Points.Cast<Point>().Where(nPoint => nPoint.Name != string.Empty));
                    if (delObjects.Count > 0)
                        DeleteNxObjects(delObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetComponentColor()
        {
            switch (listBoxMaterial.Text)
            {
                case "A2":
                case "A2 GS":
                    _compColor = _compColor.ColorName == "LIGHT RED ORANGE"
                        ? new CtsComponentType("ORANGE", CtsComponentColor.Orange)
                        : new CtsComponentType("LIGHT RED ORANGE", CtsComponentColor.LightRedOrange);
                    break;
                case "ACUSEAL":
                case "ALUM":
                case "O1":
                case "O1 GS":
                case "O1 DR":
                case "STEELCRAFT":
                    _compColor = _compColor.ColorName == "LIGHT GREY"
                        ? new CtsComponentType("DARK GREY", CtsComponentColor.DarkGrey)
                        : new CtsComponentType("LIGHT GREY", CtsComponentColor.LightGrey);
                    break;
                case "AMPCO 18":
                    _compColor = new CtsComponentType("DARK HARD ORANGE", CtsComponentColor.DarkHardOrange);
                    break;
                case "ANGLE IRON":
                case "HRS":
                case "HRS PLT":
                    _compColor = new CtsComponentType("DARK WEAK RED", CtsComponentColor.DarkWeakRed);
                    break;
                case "CRS":
                    _compColor = new CtsComponentType("DARK DULL BLUE", CtsComponentColor.DarkDullBlue);
                    break;
                case "D2":
                    _compColor = _compColor.ColorName == "DARK WEAK MAGENTA"
                        ? new CtsComponentType("PURPLE", CtsComponentColor.Purple)
                        : new CtsComponentType("DARK WEAK MAGENTA", CtsComponentColor.DarkWeakMagenta);
                    break;
                case "DIEVAR":
                case "P20":
                case "S7":
                case "H13":
                    _compColor = _compColor.ColorName == "OBSCURE DULL GREEN"
                        ? new CtsComponentType("DARK DULL GREEN", CtsComponentColor.DarkDullGreen)
                        : new CtsComponentType("OBSCURE DULL GREEN", CtsComponentColor.ObscureDullGreen);
                    break;
                case "GM190/S0050A":
                case "GM238/G2500":
                case "GM241/G3500":
                case "GM245/D4512":
                case "GM246/D5506":
                case "GM338/D6510":
                    _compColor = _compColor.ColorName == "AQUAMARINE"
                        ? new CtsComponentType("MEDIUM AZURE CYAN", CtsComponentColor.MediumAzureCyan)
                        : new CtsComponentType("AQUAMARINE", CtsComponentColor.AquaMarine);
                    break;
                case "4140":
                case "4140 PH":
                case "4140 PLT":
                    _compColor = _compColor.ColorName == "MEDIUM AZURE BLUE"
                        ? new CtsComponentType("LIGHT DULL AZURE", CtsComponentColor.LightDullAzure)
                        : new CtsComponentType("MEDIUM AZURE BLUE", CtsComponentColor.MediumAzureBlue);
                    break;
                default:
                    _compColor = new CtsComponentType("DARK GREY", CtsComponentColor.DarkGrey);
                    break;
            }
        }

        private void ColorComponent()
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "Color Component");
            try
            {
                try
                {
                    if (_changeColorComponent is null)
                        return;

                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    var dispComp = (BasePart)_changeColorComponent.Prototype;
                    session_.Parts.SetDisplay(dispComp, false, false, out var displayLoadStatus);
                    UpdateSessionParts();
                    displayLoadStatus.Dispose();

                    var bodyCollection = _displayPart.Bodies.Cast<Body>().Where(solidBody => solidBody.Layer == 1)
                        .ToList();

                    if (bodyCollection.Count == 1)
                    {
                        // Get selected solid body feature faces

                        var featureFaces = new List<Face>();

                        ufsession_.Modl.AskBodyFeats(bodyCollection[0].Tag, out var features);

                        ufsession_.Modl.AskFeatType(features[0], out var featureType);
                        if (featureType == "EXTRUDE" || featureType == "BLOCK" || featureType == "BREP")
                        {
                            ufsession_.Modl.AskFeatFaces(features[0], out var facesOfFeature);

                            featureFaces.AddRange(facesOfFeature.Select(face => (Face)NXObjectManager.Get(face)));
                        }

                        // Convert allFaces array to allFacesList to allow removal of feature faces

                        var allFaces = bodyCollection[0].GetFaces();
                        var allFacesList = allFaces.ToList();

                        var faces = new List<Face>();
                        var colors = new List<int>();

                        foreach (var face in featureFaces)
                            allFacesList.Remove(face);

                        // Fill face list and color list with remaining faces

                        foreach (var selectedFace in allFacesList)
                        {
                            faces.Add(selectedFace);
                            colors.Add(selectedFace.Color);
                        }

                        // Color solid body

                        bodyCollection[0].Color = _compColor.ComponentColor;
                        bodyCollection[0].RedisplayObject();

                        _displayPart.Preferences.ObjectPreferences.SetColor(PartObject.ObjectType.Solidbody,
                            _compColor.ComponentColor);

                        // Return faces to original color

                        for (var i = 0; i < faces.Count; i++)
                        {
                            faces[i].Color = colors[i];
                            faces[i].RedisplayObject();
                        }

                        changeColorCheckBox.Checked = false;

                        session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var displayLoadStatus1);
                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        UpdateSessionParts();
                        UpdateOriginalParts();
                        _displayPart.Views.Regenerate();
                        displayLoadStatus1.Dispose();
                    }
                    else
                    {
                        UI.GetUI()
                            .NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                $"Component {_changeColorComponent.DisplayName} has more than one solid body on layer 1");
                    }
                }
                catch (NXException ex)
                {
                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var displayLoadStatus2);
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    session_.Parts.SetWork(_originalWorkPart);
                    UpdateSessionParts();
                    UpdateOriginalParts();
                    _displayPart.Views.Regenerate();
                    displayLoadStatus2.Dispose();

                    UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error, ex.Message);
                    session_.UndoToLastVisibleMark();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ColorBaseFace(Body block, bool upperComponent)
        {
            var bodyFaces = block.GetFaces();

            foreach (var bFace in bodyFaces)
            {
                if (bFace.SolidFaceType != Face.FaceType.Planar)
                    continue;

                double[] vec1 =
                {
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zx,
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zy,
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zz
                };

                var point = new double[3];
                var vec2 = new double[3];
                var box = new double[6];
                int isEqualVec;
                ufsession_.Modl.AskFaceData(bFace.Tag, out _, point, vec2, box, out _, out _, out _);
                ufsession_.Vec3.IsParallel(vec1, vec2, .001, out var isParallel);

                if (isParallel != 1)
                    continue;

                if (upperComponent)
                {
                    ufsession_.Vec3.IsEqual(vec1, vec2, .001, out isEqualVec);

                    if (isEqualVec != 1)
                        continue;

                    bFace.Color = 6;
                    bFace.RedisplayObject();
                }
                else
                {
                    ufsession_.Vec3.IsEqual(vec1, vec2, .001, out isEqualVec);

                    if (isEqualVec != 0)
                        continue;

                    bFace.Color = 6;
                    bFace.RedisplayObject();
                }
            }
        }

        private bool FormatDetailNumber()
        {
            var compNameIncrement = textBoxDetailNumber.Text;
            var isConverted = int.TryParse(compNameIncrement, out var compNameResult);
            var next = compNameResult;
            var originalPath = _originalWorkPart.FullPath;
            var indexLastDash = originalPath.LastIndexOf("-");
            var filePath = $"{_originalWorkPart.FullPath.Remove(indexLastDash + 1)}{compNameIncrement}.prt";
            var status = ufsession_.Part.IsLoaded(filePath);

            while (File.Exists(filePath) || status == 1)
            {
                next += 1;
                compNameIncrement = next.ToString().PadLeft(3, '0');
                filePath = $"{_originalWorkPart.FullPath.Remove(indexLastDash + 1)}{compNameIncrement}.prt";
                status = ufsession_.Part.IsLoaded(filePath);
            }

            if (chk4Digits.Checked)
            {
                if (compNameIncrement.Length != 4)
                    return false;

                if (!isConverted)
                    return false;

                if (compNameResult <= 0 || compNameResult >= 10000)
                    return false;

                if (_isNameReset)
                {
                    _isNameReset = false;
                    textBoxDetailNumber.Text = compNameIncrement;
                    return true;
                }

                compNameResult += 1;

                if ((compNameResult > 0) & (compNameResult < 10))
                {
                    textBoxDetailNumber.Text = "000" + compNameResult;
                    return true;
                }

                if ((compNameResult > 9) & (compNameResult < 100))
                {
                    textBoxDetailNumber.Text = "00" + compNameResult;
                    return true;
                }

                if ((compNameResult > 99) & (compNameResult < 1000))
                {
                    textBoxDetailNumber.Text = "0" + compNameResult;
                    return true;
                }

                if (compNameResult <= 999)
                    return false;

                textBoxDetailNumber.Text = compNameResult.ToString();
                return true;
            }
            else
            {

                if (compNameIncrement.Length != 3)
                    return false;

                if (!isConverted)
                    return false;

                if (compNameResult <= 0 || compNameResult >= 991)
                    return false;

                if (_isNameReset)
                {
                    _isNameReset = false;
                    textBoxDetailNumber.Text = compNameIncrement;
                    return true;
                }

                compNameResult += 1;

                if ((compNameResult > 0) & (compNameResult < 10))
                {
                    textBoxDetailNumber.Text = "00" + compNameResult;
                    return true;
                }

                if ((compNameResult > 9) & (compNameResult < 100))
                {
                    textBoxDetailNumber.Text = "0" + compNameResult;
                    return true;
                }

                if (compNameResult <= 100)
                    return false;

                textBoxDetailNumber.Text = compNameResult.ToString();
                return true;
            }


        }

        private void UpdateFormText()
        {
            var indexOf = _workPart.FullPath.LastIndexOf("\\");
            var fullName = _workPart.FullPath.Substring(indexOf + 1);
            var formatName = fullName.Substring(0, fullName.Length - 4);
            Text = formatName;
        }

        private void ResetForm(Part wp)
        {
            UpdateSessionParts();

            if (!IsNameValid(wp))
            {
                textBoxDetailNumber.Clear();
                UpdateFormText();
                InitializeMainForm();
                NameCheckFailed();
                return;
            }

            UpdateFormText();
            InitializeMainForm();
            NameCheckPassed();
            _isNameReset = true;
            _isSaveAs = false;
            //var names = new List<int>();

            if (wp.ComponentAssembly.RootComponent is null)
                return;

            var names = wp.ComponentAssembly.RootComponent.GetChildren()
                .Select(__c => __c.DisplayName)
                .Distinct()
                .Select(__n => Regex.Match(__n, "^\\d+-\\d+-(?<detail>\\d+)$"))
                .Where(__m => __m.Success)
                .Select(__m => __m.Groups["detail"].Value)
                .Select(int.Parse)
                .ToList();

            if (names.Count == 0)
                return;

            names.Sort();

            var lastComponentName = names[names.Count - 1] + 1;

            if (chk4Digits.Checked)
            {
                if (lastComponentName > 0 && lastComponentName < 10)
                    textBoxDetailNumber.Text = $"000{lastComponentName}";

                if (lastComponentName > 9 && lastComponentName < 100)
                    textBoxDetailNumber.Text = $"00{lastComponentName}";

                if (lastComponentName > 99 && lastComponentName < 1000)
                    textBoxDetailNumber.Text = $"0{lastComponentName}";

                if (lastComponentName > 1000)
                    textBoxDetailNumber.Text = $"{lastComponentName}";

                return;
            }

            if ((lastComponentName > 0) & (lastComponentName < 10))
                textBoxDetailNumber.Text = $"00{lastComponentName}";

            if ((lastComponentName > 9) & (lastComponentName < 100))
                textBoxDetailNumber.Text = $"0{lastComponentName}";

            if (lastComponentName > 100)
                textBoxDetailNumber.Text = lastComponentName.ToString();
        }

        private bool IsNameValid(Part partToCheck)
        {
            if (chkAnyAssembly.Checked)
                return true;

            var lastDirIndex = partToCheck.FullPath.LastIndexOf("\\");
            var subAssemName = partToCheck.FullPath.Substring(lastDirIndex + 1);

            return subAssemName.ToLower().Contains("lsp")
                || subAssemName.ToLower().Contains("usp")
                || subAssemName.ToLower().Contains("ush")
                || subAssemName.ToLower().Contains("lsh")
                || subAssemName.ToLower().Contains("lftr")
                || subAssemName.ToLower().Contains("lnitro")
                || subAssemName.ToLower().Contains("unitro")
                || subAssemName.ToLower().Contains("acc")
                || subAssemName.ToLower().Contains("lair")
                || subAssemName.ToLower().Contains("lele");
        }

        private bool IsSaveAllowed(Component comp)
        {
            bool isAllowed;
            var lastDash = comp.DisplayName.LastIndexOf('-');
            if (lastDash != -1)
            {
                var subAssemName = comp.DisplayName.Substring(lastDash + 1, comp.DisplayName.Length - (lastDash + 1));
                switch (subAssemName)
                {
                    case "strip":
                        isAllowed = false;
                        break;
                    case "layout":
                        isAllowed = false;
                        break;
                    case "blank":
                        isAllowed = false;
                        break;
                    case "control":
                        isAllowed = false;
                        break;
                    case "lsh":
                        isAllowed = false;
                        break;
                    case "lsp1":
                    case "lsp2":
                    case "lsp3":
                    case "lsp4":
                    case "lsp5":
                    case "lsp6":
                    case "lsp7":
                    case "lsp8":
                    case "lsp9":
                    case "lsp10":
                    case "lsp11":
                    case "lsp12":
                    case "lsp13":
                    case "lsp14":
                    case "lsp15":
                        isAllowed = false;
                        break;
                    case "ush":
                        isAllowed = false;
                        break;
                    case "usp1":
                    case "usp2":
                    case "usp3":
                    case "usp4":
                    case "usp5":
                    case "usp6":
                    case "usp7":
                    case "usp8":
                    case "usp9":
                    case "usp10":
                    case "usp11":
                    case "usp12":
                    case "usp13":
                    case "usp14":
                    case "usp15":
                        isAllowed = false;
                        break;
                    case "press":
                        isAllowed = false;
                        break;
                    case "upr":
                        isAllowed = false;
                        break;
                    case "lwr":
                        isAllowed = false;
                        break;
                    default:
                        isAllowed = true;
                        break;
                }
            }
            else
            {
                isAllowed = true;
            }

            return isAllowed;
        }

        private void InitializeMainForm()
        {
            buttonEndEditConstruction.Enabled = false;
            buttonAutoUpr.Enabled = false;
            buttonAutoLwr.Enabled = false;
            buttonUprRetAssm.Enabled = false;
            buttonLwrRetAssm.Enabled = false;
            // create list of grind tolerances
            comboBoxTolerance.Items.Clear();

            foreach (var grindTol in _compTolerances)
                comboBoxTolerance.Items.Add(grindTol);

            comboBoxTolerance.SelectedIndex = -1;
            comboBoxTolerance.Enabled = false;
            // create list of component names
            int selectedName;
            selectedName = comboBoxCompName.SelectedIndex;
            comboBoxCompName.Items.Clear();

            foreach (var name in _compNames)
                comboBoxCompName.Items.Add(name);

            if (selectedName != -1)
                comboBoxCompName.SelectedIndex = selectedName;

            // create list of material types
            listBoxMaterial.Items.Clear();
            textBoxUserMaterial.Text = string.Empty;

            foreach (var matl in _compMaterials)
                listBoxMaterial.Items.Add(matl);

            LoadGridSizes();
        }

        private void NameCheckFailed()
        {
            textBoxDetailNumber.Enabled = false;
            comboBoxCompName.Enabled = false;
            listBoxMaterial.Enabled = false;
            checkBoxBurnout.Checked = false;
            checkBoxBurnout.Enabled = false;
            checkBoxGrind.Checked = false;
            checkBoxGrind.Enabled = false;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirX.Enabled = false;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirY.Enabled = false;
            checkBoxBurnDirZ.Checked = false;
            checkBoxBurnDirZ.Enabled = false;
            comboBoxTolerance.Text = string.Empty;
            comboBoxTolerance.SelectedIndex = -1;
            comboBoxTolerance.Enabled = false;
            buttonEditConstruction.Enabled = true;
            saveAsButton.Enabled = false;
            copyButton.Enabled = false;
        }

        private void NameCheckPassed()
        {
            textBoxDetailNumber.Clear();
            textBoxDetailNumber.Enabled = true;
            comboBoxCompName.SelectedIndex = -1;
            comboBoxCompName.Enabled = false;
            listBoxMaterial.SelectedIndex = -1;
            listBoxMaterial.Enabled = false;
            checkBoxBurnout.Checked = false;
            checkBoxBurnout.Enabled = false;
            checkBoxGrind.Checked = false;
            checkBoxGrind.Enabled = false;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirX.Enabled = false;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirY.Enabled = false;
            checkBoxBurnDirZ.Checked = false;
            checkBoxBurnDirZ.Enabled = false;
            comboBoxTolerance.Text = string.Empty;
            comboBoxTolerance.SelectedIndex = -1;
            comboBoxTolerance.Enabled = false;
            buttonEditConstruction.Enabled = true;
            saveAsButton.Enabled = true;
            copyButton.Enabled = true;
        }


        private void CreateRefSetsCategories()
        {
            Session.UndoMarkId markId1;
            markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Reference Sets");

            var isBodyRefSet = false;
            ReferenceSet referenceSet1 = null;

            foreach (var bRef in _workPart.GetAllReferenceSets())
            {
                if (bRef.Name != "BODY") continue;
                isBodyRefSet = true;
                referenceSet1 = bRef;
            }

            if (isBodyRefSet)
            {
                var referenceSet2 = _workPart.CreateReferenceSet();

                referenceSet2.SetName("SUB_TOOL");

                referenceSet1.SetAddComponentsAutomatically(false, false);
                referenceSet2.SetAddComponentsAutomatically(false, false);

                var objects1 = new NXObject[1];
                var objects2 = new NXObject[1];

                Body body1 = null;
                Body extractBody = null;

                foreach (Body sBody in _workPart.Bodies)
                    if (sBody.Layer == 1)
                        body1 = sBody;

                objects1[0] = body1;

                foreach (Body exBody in _workPart.Bodies)
                    if (exBody.Layer == 15)
                        extractBody = exBody;

                objects2[0] = extractBody;

                if (body1 != null)
                    referenceSet1.AddObjectsToReferenceSet(objects1);

                if (extractBody != null)
                    referenceSet2.AddObjectsToReferenceSet(objects2);
                _ = session_.UpdateManager.DoUpdate(markId1);
            }
            else
            {
                referenceSet1 = _workPart.CreateReferenceSet();

                ReferenceSet referenceSet2;
                referenceSet2 = _workPart.CreateReferenceSet();

                referenceSet1.SetName("BODY");
                referenceSet2.SetName("SUB_TOOL");

                referenceSet1.SetAddComponentsAutomatically(false, false);
                referenceSet2.SetAddComponentsAutomatically(false, false);

                var objects1 = new NXObject[1];
                var objects2 = new NXObject[1];

                Body body1 = null;
                Body extractBody = null;

                foreach (Body sBody in _workPart.Bodies)
                    if (sBody.Layer == 1)
                        body1 = sBody;

                objects1[0] = body1;

                foreach (Body exBody in _workPart.Bodies)
                    if (exBody.Layer == 15)
                        extractBody = exBody;

                objects2[0] = extractBody;

                if (body1 != null)
                    referenceSet1.AddObjectsToReferenceSet(objects1);

                if (extractBody != null)
                    referenceSet2.AddObjectsToReferenceSet(objects2);

                session_.UpdateManager.DoUpdate(markId1);
            }

            var bodyLayers = new int[1];
            bodyLayers[0] = 1;
            _workPart.LayerCategories.CreateCategory("BODY", "", bodyLayers);

            var burnoutLayers = new int[6];
            burnoutLayers[0] = 100;
            burnoutLayers[1] = 101;
            burnoutLayers[2] = 102;
            burnoutLayers[3] = 103;
            burnoutLayers[4] = 104;
            burnoutLayers[5] = 105;
            _workPart.LayerCategories.CreateCategory("BURNOUT", "", burnoutLayers);

            var constructionLayers = new int[9];
            constructionLayers[0] = 2;
            constructionLayers[1] = 3;
            constructionLayers[2] = 4;
            constructionLayers[3] = 5;
            constructionLayers[4] = 6;
            constructionLayers[5] = 7;
            constructionLayers[6] = 8;
            constructionLayers[7] = 9;
            constructionLayers[8] = 10;
            _workPart.LayerCategories.CreateCategory("CONSTRUCTION", "", constructionLayers);

            var datumLayers = new int[1];
            datumLayers[0] = 255;
            _workPart.LayerCategories.CreateCategory("DATUM", "", datumLayers);

            var fastenerLayers = new int[1];
            fastenerLayers[0] = 99;
            _workPart.LayerCategories.CreateCategory("FASTENERS", "", fastenerLayers);

            var handlingLayers = new int[1];
            handlingLayers[0] = 98;
            _workPart.LayerCategories.CreateCategory("HANDLINGHOLES", "", handlingLayers);

            var holeChartLayers = new int[1];
            holeChartLayers[0] = 230;
            _workPart.LayerCategories.CreateCategory("HOLECHARTTEXT", "", holeChartLayers);

            var nestedBlkLayers = new int[1];
            nestedBlkLayers[0] = 96;
            _workPart.LayerCategories.CreateCategory("NESTEDBLOCKS", "", nestedBlkLayers);

            var orderBlkLayers = new int[1];
            orderBlkLayers[0] = 250;
            _workPart.LayerCategories.CreateCategory("ORDERBLOCK", "", orderBlkLayers);

            var subToolLayers = new int[6];
            subToolLayers[0] = 15;
            subToolLayers[1] = 16;
            subToolLayers[2] = 17;
            subToolLayers[3] = 18;
            subToolLayers[4] = 19;
            subToolLayers[5] = 20;
            _workPart.LayerCategories.CreateCategory("SUBTOOL", "", subToolLayers);

            var titleBlkLayers = new int[1];
            titleBlkLayers[0] = 200;
            _workPart.LayerCategories.CreateCategory("TITLEBLOCK", "", titleBlkLayers);

            var toolingHoleLayers = new int[1];
            toolingHoleLayers[0] = 97;
            _workPart.LayerCategories.CreateCategory("TOOLINGHOLES", "", toolingHoleLayers);

            var wireStartHoleLayers = new int[1];
            wireStartHoleLayers[0] = 94;
            _workPart.LayerCategories.CreateCategory("WIRESTARTHOLE", "", wireStartHoleLayers);

            _displayPart.Layers.SetState(3, State.WorkLayer);
        }

        private void CreateAutoSizeUdo()
        {
            //: 1535022'

            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");

            if (myUdOclass is null)
                return;

            var currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

            if (currentUdo.Length != 0)
                return;

            BasePart myBasePart = _workPart;
            var myUdOmanager = myBasePart.UserDefinedObjectManager;
            var myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
            var myLinks = new UserDefinedObject.LinkDefinition[1];
            var numBodies = _workPart.Bodies.Cast<Body>().Count(body => body.Layer == 1);

            if (numBodies != 1)
                return;

            foreach (Body body in _workPart.Bodies)
            {
                if (body.Layer != 1)
                    continue;

                myLinks[0].AssociatedObject = body;
                myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
                myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
                int[] updateOff = { 1 };
                myUdo.SetIntegers(updateOff);
            }
        }

        private void SetWcsToWorkPart(Component compRefCsys)
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "SetWcsToWorkPart");

            if (compRefCsys is null)
            {
                foreach (Feature featBlk in _workPart.Features)
                {
                    if (featBlk.FeatureType != "BLOCK" || featBlk.Name != "DYNAMIC BLOCK")
                        continue;

                    var block1 = (Block)featBlk;
                    var blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                    var bOrigin = blockFeatureBuilderMatch.Origin;
                    blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);
                    double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    var initMatrix = new double[9];
                    ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                    ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                    ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                    var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);
                    _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin, setTempCsys.Orientation.Element);
                }

                return;
            }

            var compBase = (BasePart)compRefCsys.Prototype;

            session_.Parts.SetDisplay(compBase, false, false, out var setDispLoadStatus);
            setDispLoadStatus.Dispose();
            UpdateSessionParts();

            var isBlockComp = false;

            foreach (Feature featBlk in _workPart.Features)
            {
                if (featBlk.FeatureType != "BLOCK") continue;
                if (featBlk.Name != "DYNAMIC BLOCK") continue;
                isBlockComp = true;

                var block1 = (Block)featBlk;

                var blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                var bOrigin = blockFeatureBuilderMatch.Origin;
                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                var initMatrix = new double[9];
                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin, setTempCsys.Orientation.Element);

                var featBlkCsys = _displayPart.WCS.Save();
                featBlkCsys.SetName("EDITCSYS");
                featBlkCsys.Layer = 254;

                NXObject[] addToBody = { featBlkCsys };

                foreach (var bRefSet in _displayPart.GetAllReferenceSets())
                    if (bRefSet.Name == "BODY")
                        bRefSet.AddObjectsToReferenceSet(addToBody);

                session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var setDispLoadStatus1);
                setDispLoadStatus1.Dispose();

                session_.Parts.SetWorkComponent(compRefCsys, out var partLoadStatusWorkComp);
                partLoadStatusWorkComp.Dispose();
                UpdateSessionParts();

                foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                {
                    if (wpCsys.Layer != 254)
                        continue;

                    if (wpCsys.Name != "EDITCSYS")
                        continue;

                    var csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);
                    var editCsys = (CartesianCoordinateSystem)csysOccurrence;

                    if (editCsys != null)
                        _displayPart.WCS.SetOriginAndMatrix(editCsys.Origin, editCsys.Orientation.Element);

                    session_.__DeleteObjects(editCsys);
                }
            }

            if (isBlockComp)
                return;

            __display_part_ = _originalDisplayPart;
            __work_component_ = compRefCsys;
            session_.Parts.SetDisplay(_originalDisplayPart, false, false, out _);
            UpdateSessionParts();
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

        private static List<Body> SelectMultipleBodies()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);
            var bodySelection = new List<Body>();

            var sel = TheUISession.SelectionManager.SelectTaggedObjects("Select Bodies", "Select Bodies",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedBodyArray);

            if (sel != Selection.Response.Ok)
                return bodySelection;

            bodySelection.AddRange(selectedBodyArray.Cast<Body>());
            return bodySelection;
        }

        private Component SelectOneComponent(string prompt)
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            var sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedComp, out _);

            if (!((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName)))
                return null;

            var compSelection = (Component)selectedComp;
            return compSelection;
        }

        private void DeleteNxObjects(List<NXObject> objsToDelete)
        {
            var markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
            session_.UpdateManager.AddObjectsToDeleteList(objsToDelete.ToArray());
            session_.UpdateManager.DoUpdate(markDeleteObjs);
        }

        private string PerformStreamReaderString(string path, string startSearchString, string endSearchString)
        {
            var sr = new StreamReader(path);
            var content = sr.ReadToEnd();
            sr.Close();
            var startSplit = Regex.Split(content, startSearchString);
            var endSplit = Regex.Split(startSplit[1], endSearchString);
            var textSetting = endSplit[0];
            textSetting = textSetting.Replace("\r\n", string.Empty);
            return textSetting.Length > 0 ? textSetting : null;
        }

        private List<CtsAttributes> PerformStreamReaderList(string path, string startSearchString,
            string endSearchString)
        {
            var sr = new StreamReader(path);
            var content = sr.ReadToEnd();
            sr.Close();
            var startSplit = Regex.Split(content, startSearchString);
            var endSplit = Regex.Split(startSplit[1], endSearchString);
            var textData = endSplit[0];
            var splitData = Regex.Split(textData, "\r\n");
            var compData = (from sData in splitData
                            where sData != string.Empty
                            select new CtsAttributes { AttrValue = sData }).ToList();
            return compData.Count > 0 ? compData : null;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
            Settings.Default.udoComponentBuilderGridIncrement = comboBoxGrid.Text;
        }

        private enum AssemblyComponent
        {
            Lower,
            Upper,
            None
        }

        private void chkDigits_CheckedChanged(object sender, EventArgs e)
        {
            WorkPartChanged1(__work_part_);
        }

        private static bool ConvertUnits(double[] distances)
        {
            if (_workPart.PartUnits != BasePart.Units.Millimeters)
                return false;

            for (var i = 0; i < distances.Length; i++)
                distances[i] /= 25.4d;

            return true;
        }


        private static void RoundAndTruncate(double number, out double roundValue, out double truncateValue, out double fractionValue)
        {
            roundValue = Math.Round(number, 3);
            truncateValue = Math.Truncate(roundValue);
            fractionValue = roundValue - truncateValue;
        }



        private static void Distance0(double[] distances)
        {
            for (var i = 0; i < 3; i++)
            {
                RoundAndTruncate(distances[i], out var roundValue, out var truncateValue, out var fractionValue);

                if (Math.Abs(fractionValue) <= Tolerance)
                {
                    distances[i] = roundValue;
                    continue;
                }

                for (var ii = .125; ii <= 1; ii += .125)
                {
                    if (!(fractionValue <= ii))
                        continue;

                    var roundedFraction = ii;
                    var finalValue = truncateValue + roundedFraction;
                    distances[i] = finalValue;
                    break;
                }
            }
        }

        private static void MinCorner3(double[] minCorner)
        {
            for (var i = 0; i < 3; i++)
            {
                var isNegative = false;

                if (minCorner[i] < 0)
                {
                    minCorner[i] *= -1;
                    isNegative = true;
                }

                RoundAndTruncate(minCorner[i], out var roundValue, out var truncateValue, out var fractionValue);

                if (Math.Abs(fractionValue) <= Tolerance)
                {
                    minCorner[i] = roundValue;
                    continue;
                }

                for (var ii = .125; ii <= 1; ii += .125)
                {
                    if (!(fractionValue <= ii))
                        continue;

                    var roundedFraction = ii;
                    var finalValue = truncateValue + roundedFraction;

                    if (isNegative)
                        minCorner[i] = finalValue * -1;
                    else
                        minCorner[i] = finalValue;

                    break;
                }
            }
        }

        private void chkAnyAssembly_CheckedChanged(object sender, EventArgs e)
        {
            WorkPartChanged1(__work_part_);
        }
    }
}
// 3292