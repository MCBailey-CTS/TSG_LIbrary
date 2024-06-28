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
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;
using Part = NXOpen.Part;
using TSG_Library.Extensions;
using NXOpen.UF;
using NXOpenUI;
using static NXOpen.MenuBar.MenuBarManager;
using System.Linq.Expressions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_component_builder)]
    public partial class ComponentBuilder : _UFuncForm
    {

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
        private static bool _isDynamic;
        private static Point _udoPointHandle;
        private static double _gridSpace;
        private static Point3d _workCompOrigin;
        private static Matrix3x3 _workCompOrientation;
        private static readonly List<string> _nonValidNames = new List<string>();
        private static readonly List<Line> _edgeRepLines = new List<Line>();
        private static double _distanceMoved;
        private static Component _updateComponent;
        private static Body _editBody;
        private static bool _isNewSelection = true;
        private static bool _isUprParallel;
        private static bool _isLwrParallel;
        private static string _parallelHeightExp = string.Empty;
        private static string _parallelWidthExp = string.Empty;

        public ComponentBuilder()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label1.Text = AssemblyFileVersion;

            if (_displayPart != null)
                WorkPartChanged1(_displayPart);

            chk4Digits.Checked = Settings.Default.comp_builder_4_digits;
            chkAnyAssembly.Checked = Settings.Default.com_builder_any_assembly;

            Location = Settings.Default.udoComponentBuilderWindowLocation;
            toolTip1.SetToolTip(buttonAquamarine, CtsComponentColor.AquaMarine.ToString());
            toolTip1.SetToolTip(buttonDarkDullGreen, CtsComponentColor.DarkDullGreen.ToString());
            toolTip1.SetToolTip(buttonDarkWeakMagenta, CtsComponentColor.DarkWeakMagenta.ToString());
            toolTip1.SetToolTip(buttonDarkWeakRed, CtsComponentColor.DarkWeakRed.ToString());
            toolTip1.SetToolTip(buttonMedAzureBlue, CtsComponentColor.MediumAzureBlue.ToString());
            toolTip1.SetToolTip(buttonObscureDullGreen, CtsComponentColor.ObscureDullGreen.ToString());
            toolTip1.SetToolTip(buttonPurple, CtsComponentColor.Purple.ToString());
            toolTip1.SetToolTip(buttonDarkDullBlue, CtsComponentColor.DarkDullBlue.ToString());

            string getName = FilePathUcf.PerformStreamReaderString(":DETAIL_TYPE_ATTRIBUTE_NAME:",
                ":END_DETAIL_TYPE_ATTRIBUTE_NAME:");

            string getMaterial = FilePathUcf.PerformStreamReaderString(":MATERIAL_ATTRIBUTE_NAME:",
                ":END_MATERIAL_ATTRIBUTE_NAME:");

            _compNames = FilePathUcf.PerformStreamReaderList(":COMPONENT_NAMES:", ":END_COMPONENT_NAMES:");

            foreach (CtsAttributes cName in _compNames)
                cName.AttrName = getName != string.Empty ? getName : "DETAIL NAME";

            _compMaterials =
               FilePathUcf.PerformStreamReaderList(":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");

            foreach (CtsAttributes cMaterial in _compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _compTolerances =
                FilePathUcf.PerformStreamReaderList(":COMPONENT_TOLERANCES:", ":END_COMPONENT_TOLERANCES:");

            foreach (CtsAttributes cTolerance in _compTolerances)
                cTolerance.AttrName = "TOLERANCE";

            _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
            _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
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

                int gridIndex = comboBoxGrid.Items.IndexOf(gridSetting);
                comboBoxGrid.SelectedIndex = gridIndex;
            }
        }

        private void SetWorkPlane(bool snapToGrid)
        {
            _workPart = session_.Parts.Work;
            _displayPart = session_.Parts.Display; ;
            session_.__SetWorkPlane(double.Parse((string)comboBoxGrid.SelectedItem), snapToGrid, false);
        }

        public int Startup()
        {
            if (_registered != 0)
                return 0;

            ComponentBuilder mForm = this;
            _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(mForm.WorkPartChanged1);
            _registered = 1;
            return 0;
        }

        public void WorkPartChanged1(BasePart oldWorkPart)
        {
            try
            {
                if (!_isSaveAs)
                {
                    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                    ResetForm(_workPart);
                    LoadGridSizes();
                    listBoxMaterial.Enabled = false;
                    groupBoxColor.Enabled = false;
                }

                Component workComp = __work_component_;


                if (workComp is null)
                    if (session_.Parts.Work.ComponentAssembly.RootComponent != null)
                        workComp = session_.Parts.Work.ComponentAssembly.RootComponent;
                    else
                        return;

                AssemblyComponent ass = Check(workComp);

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

                string displayName = component.DisplayName.ToUpper();

                if (displayName.Contains("LWR")
                    || displayName.Contains("LSP")
                    || displayName.Contains("LSH")
                    || displayName.Contains("LAD")
                    || displayName.Contains("LFTR")
                    || displayName.Contains("LOWER"))
                    return AssemblyComponent.Lower;

                if (displayName.Contains("UPR")
                    || displayName.Contains("USP")
                    || displayName.Contains("USH")
                    || displayName.Contains("UAD")
                    || displayName.Contains("UPPER"))
                    return AssemblyComponent.Upper;

                if (component.Parent is null)
                    return AssemblyComponent.None;

                component = component.Parent;
            }
        }


        private void UpdateSessionButton_Click(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            try
            {
                _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
                UpdateFormText();
                ResetForm(_workPart);
                groupBoxColor.Enabled = false;
                changeColorCheckBox.Enabled = true;
                changeColorCheckBox.Checked = false;
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

                bool isConverted = int.TryParse(textBoxDetailNumber.Text, out int compName);

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

                bool flag = listBoxMaterial.Text == "HRS PLT"
                            || listBoxMaterial.Text == "4140 PLT"
                            || listBoxMaterial.Text == "HRS";

                groupBoxColor.Enabled = true;
                //textBoxUserMaterial.Text = string.Empty;
                changeColorCheckBox.Checked = false;
                changeColorCheckBox.Enabled = false;
                buttonAutoUpr.Enabled = true;
                buttonAutoLwr.Enabled = true;
                buttonUprRetAssm.Enabled = true;
                buttonLwrRetAssm.Enabled = true;

                if (!flag)
                    return;

                bool boolFlag = listBoxMaterial.Text != "HRS PLT"
                                && listBoxMaterial.Text != "4140 PLT";

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

        private void ButtonAutoLwr_Click(object sender, EventArgs e)
        {
            try
            {
                _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
                checkBoxUpperComp.Checked = false;
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                List<Body> bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                {
                    updateSessionButton.PerformClick();
                    return;
                }

                foreach (Body selectedBody in bodies)
                {
                    selectedBody.Unhighlight();
                    bool isMetric = false;
                    double[] minCorner = new double[3];
                    double[,] directions = new double[3, 3];
                    double[] distances = new double[3];
                    ufsession_.Modl.AskBoundingBoxExact(selectedBody.Tag, _displayPart.WCS.CoordinateSystem.Tag,
                        minCorner, directions, distances);
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
            Point3d blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] - 1.25, -1.50);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), "1.50");
        }

        private void AuotLowrMetric(double[] minCorner, double[] distances)
        {
            for (int i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            Point3d blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] - 31.75, -38.1);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "38.1");
        }

        private void ButtonAutoUpr_Click(object sender, EventArgs e)
        {
            try
            {
                _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
                checkBoxUpperComp.Checked = true;
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                List<Body> bodies = SelectMultipleBodies();
                if (bodies.Count > 0)
                {
                    foreach (Body selectedBody in bodies)
                    {
                        selectedBody.Unhighlight();
                        bool isMetric = false;
                        double[] minCorner = new double[3];
                        double[,] directions = new double[3, 3];
                        double[] distances = new double[3];
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
            Point3d blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] + 1.25 + distances[1],
                3.50);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(),
                (distances[1] + 2.50).ToString(), "1.50");
        }

        private void AutoUpperMetric(double[] minCorner, double[] distances)
        {
            for (int i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            Point3d blankCompOrigin = new Point3d(minCorner[0] - 31.75, minCorner[1] + 31.75 + distances[1],
                88.9);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 63.5).ToString(),
                (distances[1] + 63.5).ToString(), "38.1");
        }

        private void ButtonLwrRetAssm_Click(object sender, EventArgs e)
        {
            try
            {
                _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;

                checkBoxUpperComp.Checked = false;

                // set WCS to absolute
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                // select bodies

                List<Body> bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                {
                    updateSessionButton.PerformClick();
                    return;
                }

                foreach (Body selectedBody in bodies)
                {
                    selectedBody.Unhighlight();

                    // get bounding box info

                    bool isMetric = false;
                    double[] minCorner = new double[3];
                    double[,] directions = new double[3, 3];
                    double[] distances = new double[3];

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
            int selectedName = comboBoxCompName.SelectedIndex;
            int selctedMaterial = listBoxMaterial.SelectedIndex;
            // settings for trim
            Point3d blankCompOrigin = new Point3d(minCorner[0] - .625, minCorner[1] - .625, -3.26);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 1.25).ToString(), (distances[1] + 1.25).ToString(),
                "3.50");
            //settings for retainer
            int nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;
            int materialIndex = -1;

            foreach (CtsAttributes compMaterial in listBoxMaterial.Items)
                if (compMaterial.AttrValue == "4140")
                    materialIndex = listBoxMaterial.Items.IndexOf(compMaterial);

            listBoxMaterial.SetSelected(materialIndex, true);
            blankCompOrigin = new Point3d(minCorner[0] - 1.25, minCorner[1] - 1.25, -3.26);
            SetComponentColor();
            CreateComponent(blankCompOrigin, (distances[0] + 2.50).ToString(), (distances[1] + 2.50).ToString(),
                "1.375");
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
            for (int i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            int selectedName = comboBoxCompName.SelectedIndex;
            int selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            Point3d blankCompOrigin = new Point3d(minCorner[0] - 15.875, minCorner[1] - 15.875, -82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 31.75).ToString(),
                (distances[1] + 31.75).ToString(), "88.9");

            //settings for retainer

            int nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            int materialIndex = -1;

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
                _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;

                checkBoxUpperComp.Checked = true;

                // set WCS to absolute            
                _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                // select bodies

                List<Body> bodies = SelectMultipleBodies();

                if (bodies.Count <= 0)
                    return;

                foreach (Body selectedBody in bodies)
                {
                    selectedBody.Unhighlight();

                    // get bounding box info

                    bool isMetric = false;
                    double[] minCorner = new double[3];
                    double[,] directions = new double[3, 3];
                    double[] distances = new double[3];

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
            int selectedName = comboBoxCompName.SelectedIndex;
            int selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            Point3d blankCompOrigin = new Point3d(minCorner[0] - .625, minCorner[1] + .625 + distances[1],
                3.26);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 1.25).ToString(),
                (distances[1] + 1.25).ToString(), "3.50");

            //settings for retainer

            int nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            int materialIndex = -1;

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
            for (int i = 0; i < 3; i++)
            {
                minCorner[i] *= 25.4;
                distances[i] *= 25.4;
            }

            int selectedName = comboBoxCompName.SelectedIndex;
            int selctedMaterial = listBoxMaterial.SelectedIndex;

            // settings for trim

            Point3d blankCompOrigin = new Point3d(minCorner[0] - 15.875,
                minCorner[1] + 15.875 + distances[1], 82.804);

            SetComponentColor();

            CreateComponent(blankCompOrigin, (distances[0] + 31.75).ToString(),
                (distances[1] + 31.75).ToString(), "88.9");

            //settings for retainer

            int nameIndex = -1;

            foreach (CtsAttributes compName in comboBoxCompName.Items)
                if (compName.AttrValue == "RETAINER")
                    nameIndex = comboBoxCompName.Items.IndexOf(compName);

            comboBoxCompName.SelectedIndex = nameIndex;

            int materialIndex = -1;

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

            BasePart.Units units = new BasePart.Units();
            BasePart.Units assmUnits = _displayPart.PartUnits;

            using (session_.__UsingFormShowHide(this))
                try
                {
                    Component compSaveAs = SelectOneComponent("Select Component to SaveAs");

                    while (compSaveAs != null)
                    {
                        if (!IsNameValid(_workPart))
                        {
                            compSaveAs = null;
                            _isNameReset = false;
                            _isSaveAs = false;
                            break;
                        }

                        bool isNumberValid = FormatDetailNumber();

                        string compName = textBoxDetailNumber.Text;

                        if (!isNumberValid && string.IsNullOrEmpty(compName))
                        {
                            compSaveAs = null;
                            _isNameReset = false;
                            _isSaveAs = false;
                            break;
                        }

                        //if (!IsSaveAllowed(compSaveAs))
                        //{
                        //    MessageBox.Show(
                        //        $"Save As is not allowed on this component {compSaveAs.DisplayName}");
                        //    compSaveAs = null;
                        //    _isNameReset = false;
                        //    _isSaveAs = false;
                        //    break;
                        //}

                        Part selectedPart = (Part)compSaveAs.Prototype;
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
                        __display_part_ = _originalDisplayPart;

                    __work_part_ = _originalWorkPart;
                    UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error, ex.Message);
                    _workPart = session_.Parts.Work;
                    _displayPart = session_.Parts.Display;
                    _originalWorkPart = _workPart;
                    _originalDisplayPart = _displayPart;
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
            session_.Parts.SetDisplay(selectedPart, false, false, out PartLoadStatus partLoadStatus1);
            partLoadStatus1.Dispose();
            _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

            int indexOf = _originalWorkPart.FullPath.LastIndexOf("-");
            string fullName = _originalWorkPart.FullPath.Remove(indexOf + 1);
            string saveAs = $"{fullName}{compName}.prt";

            PartSaveStatus partSaveStatus1 = _workPart.SaveAs(saveAs);
            partSaveStatus1.Dispose();

            // Get body ref set of original work part

            Tag cycleRefSet = NXOpen.Tag.Null;
            Tag bodyRefSet = NXOpen.Tag.Null;

            do
            {
                ufsession_.Obj.CycleObjsInPart(_originalWorkPart.Tag, UF_reference_set_type,
                    ref cycleRefSet);
                if (cycleRefSet == NXOpen.Tag.Null)
                    break;
                ufsession_.Obj.AskName(cycleRefSet, out string name);

                if (name == "BODY")
                    bodyRefSet = cycleRefSet;
            }
            while (cycleRefSet != NXOpen.Tag.Null);

            Part saveAsPart = (Part)compSaveAs.Prototype;
            ufsession_.Assem.AskOccsOfPart(_originalWorkPart.Tag, saveAsPart.Tag,
                out Tag[] partOccs);
            foreach (Tag occurrence in partOccs)
            {
                ufsession_.Obj.SetName(occurrence, compName);
                Tag partInstance = ufsession_.Assem.AskInstOfPartOcc(occurrence);
                ufsession_.Assem.RenameInstance(partInstance, compName);
            }

            if (bodyRefSet != NXOpen.Tag.Null)
                ufsession_.Assem.AddRefSetMembers(bodyRefSet, partOccs.Length, partOccs);

            // set attribute for "DETAIL NUMBER" to new component name

            foreach (NXObject.AttributeInformation attr in _workPart.GetUserAttributes())
                if (attr.Title == "DETAIL NUMBER")
                    _workPart.SetUserAttribute(attr.Title, -1, compName,
                        NXOpen.Update.Option.Now);

            session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                out PartLoadStatus partLoadStatus2);
            session_.Parts.SetWork(_originalWorkPart);
            partLoadStatus2.Dispose();
            _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
            _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            _isNameReset = false;
        }

        private void SaveAsSameUnits(Component compSaveAs, string compName)
        {
            _isSaveAs = true;

            session_.Parts.SetWork((Part)compSaveAs.Prototype);
            _workPart = session_.Parts.Work;
            _displayPart = session_.Parts.Display;

            int indexOf = _originalWorkPart.FullPath.LastIndexOf("-");
            string fullName = _originalWorkPart.FullPath.Remove(indexOf + 1);
            string saveAs = $"{fullName}{compName}.prt";

            PartSaveStatus partSaveStatus1 = _workPart.SaveAs(saveAs);
            partSaveStatus1.Dispose();

            // Get body ref set of original work part

            Tag cycleRefSet = NXOpen.Tag.Null;
            Tag bodyRefSet = NXOpen.Tag.Null;

            do
            {
                ufsession_.Obj.CycleObjsInPart(_originalWorkPart.Tag, UF_reference_set_type,
                    ref cycleRefSet);
                if (cycleRefSet == NXOpen.Tag.Null)
                    break;
                ufsession_.Obj.AskName(cycleRefSet, out string name);

                if (name == "BODY")
                    bodyRefSet = cycleRefSet;
            }
            while (cycleRefSet != NXOpen.Tag.Null);

            // Change name of all occurrences and add to parent assembly "BODY" ref set

            Part saveAsPart = (Part)compSaveAs.Prototype;
            ufsession_.Assem.AskOccsOfPart(_originalWorkPart.Tag, saveAsPart.Tag,
                out Tag[] partOccs);
            foreach (Tag occurrence in partOccs)
            {
                ufsession_.Obj.SetName(occurrence, compName);
                Tag partInstance = ufsession_.Assem.AskInstOfPartOcc(occurrence);
                ufsession_.Assem.RenameInstance(partInstance, compName);
            }

            if (bodyRefSet != NXOpen.Tag.Null)
                ufsession_.Assem.AddRefSetMembers(bodyRefSet, partOccs.Length, partOccs);

            // set attribute for "DETAIL NUMBER" to new component name

            foreach (NXObject.AttributeInformation attr in _workPart.GetUserAttributes())
                if (attr.Title == "DETAIL NUMBER")
                    _workPart.SetUserAttribute(attr.Title, -1, compName,
                        NXOpen.Update.Option.Now);

            session_.Parts.SetWork(_originalWorkPart);

            _workPart = session_.Parts.Work;
            _displayPart = session_.Parts.Display;

            _originalWorkPart = _workPart;
            _originalDisplayPart = _displayPart;

            _isNameReset = false;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Copy Component");
                Component copyComponent = SelectOneComponent("Select Component to Copy");

                if (copyComponent is null)
                    return;

                Part part = (Part)copyComponent.Prototype;
                int next = 0;
                string originalPath = part.FullPath;
                string copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 4)}({next}).prt";

                while (File.Exists(copyPath))
                {
                    next += 1;
                    copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 4)}({next}).prt";
                }

                int indexOf = _workPart.FullPath.LastIndexOf("\\");
                string compare1 = _workPart.FullPath.Remove(indexOf + 1);
                string compare2 = copyPath.Remove(indexOf + 1);

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
                BasePart basePart1 = session_.Parts.OpenBase(copyPath, out PartLoadStatus partLoadStatus1);
                partLoadStatus1.Dispose();
                Part partToAdd = (Part)basePart1;
                copyComponent.GetPosition(out Point3d origin, out Matrix3x3 orientation);
                int layer = copyComponent.Layer;

                _workPart.ComponentAssembly.AddComponent(partToAdd, "BODY", copyComponent.DisplayName,
                    origin, orientation, layer,
                    out PartLoadStatus partLoadStatus2);

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
                    ufsession_.Ui.AskInfoUnits(out int infoUnits);

                    Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;

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
                    //string userMaterial = textBoxUserMaterial.Text;
                    int compNameIndex = comboBoxCompName.SelectedIndex;
                    int compMaterialIndex = listBoxMaterial.SelectedIndex;
                    bool isUpper = checkBoxUpperComp.Checked;
                    CtsAttributes selectedName = (CtsAttributes)comboBoxCompName.SelectedItem;
                    CtsAttributes selectecMaterial = new CtsAttributes();
                    if (listBoxMaterial.SelectedIndex == -1)
                    {
                        selectecMaterial.AttrName = "MATERIAL";
                        //selectecMaterial.AttrValue = textBoxUserMaterial.Text;
                    }
                    else
                    {
                        selectecMaterial = (CtsAttributes)listBoxMaterial.SelectedItem;
                    }

                    //var numberIsValid = FormatDetailNumber();
                    if (true)
                    {
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
                        Point3d prevOrigin = _displayPart.WCS.CoordinateSystem.Origin;
                        Matrix3x3 prevOrientation = _displayPart.WCS.CoordinateSystem.Orientation.Element;
                        _displayPart.WCS.Visibility = false;
                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                        Point3d uprCompOrigin = new Point3d();
                        Matrix3x3 uprCompOrientation = new Matrix3x3();
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
                        BlockFeatureBuilder blockFeatureBuilder1 = _workPart.Features.CreateBlockFeatureBuilder(null);
                        blockFeatureBuilder1.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                        Body[] targetBodies1 = new Body[1];
                        targetBodies1[0] = null;
                        blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);
                        blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                        Point3d originPoint1;
                        if (checkBoxUpperComp.Checked)
                        {
                            Vector3d xAxis = new Vector3d(uprCompOrientation.Xx, uprCompOrientation.Xy,
                                uprCompOrientation.Xz);
                            Vector3d yAxis = new Vector3d(uprCompOrientation.Yx, uprCompOrientation.Yy,
                                uprCompOrientation.Yz);
                            originPoint1 = uprCompOrigin;
                            blockFeatureBuilder1.SetOrientation(xAxis, yAxis);
                        }
                        else if (dispUnits == Part.Units.Inches)
                        {
                            Point3d lwrCompOrigin = new Point3d(_displayPart.WCS.CoordinateSystem.Origin.X,
                                _displayPart.WCS.CoordinateSystem.Origin.Y,
                                _displayPart.WCS.CoordinateSystem.Origin.Z - 1.50);
                            originPoint1 = lwrCompOrigin;
                        }
                        else
                        {
                            Point3d lwrCompOrigin = new Point3d(_displayPart.WCS.CoordinateSystem.Origin.X,
                                _displayPart.WCS.CoordinateSystem.Origin.Y,
                                _displayPart.WCS.CoordinateSystem.Origin.Z - 38.1);
                            originPoint1 = lwrCompOrigin;
                        }

                        if (dispUnits == Part.Units.Inches)
                            blockFeatureBuilder1.SetOriginAndLengths(originPoint1, "4", "4", "1.5");
                        else
                            blockFeatureBuilder1.SetOriginAndLengths(originPoint1, "101.6", "101.6", "38.1");
                        blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, null);
                        Feature feature1 = blockFeatureBuilder1.CommitFeature();
                        feature1.SetName("DYNAMIC BLOCK");
                        blockFeatureBuilder1.Destroy();
                        _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();
                        session_.SetUndoMark(Session.MarkVisibility.Visible, "Create New Component");
                        FileNew fileNew1 = session_.Parts.FileNew();
                        fileNew1.TemplateFileName = "Blank";
#pragma warning disable CS0618
                        fileNew1.Application = FileNewApplication.Gateway;
#pragma warning restore CS0618
                        fileNew1.Units = (Part.Units)_displayPart.PartUnits;
                        int indexBslash = _workPart.FullPath.LastIndexOf("\\");
                        int indexDash = _workPart.FullPath.LastIndexOf("-");
                        string filePath = _workPart.FullPath.Remove(indexBslash + 1);
                        string assmName = _workPart.FullPath.Substring(indexBslash + 1, indexDash - 1 - indexBslash);
                        string fileName = textBoxDetailNumber.Text;
                        fileNew1.NewFileName = filePath + /*"\\" +*/ assmName + "-" + fileName + ".prt";
                        fileNew1.MasterFileName = "";
                        fileNew1.UseBlankTemplate = true;
                        fileNew1.MakeDisplayedPart = false;
                        CreateNewComponentBuilder createNewComponentBuilder1 =
                            _workPart.AssemblyManager.CreateNewComponentBuilder();
                        createNewComponentBuilder1.ReferenceSet =
                            CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;
                        createNewComponentBuilder1.ReferenceSetName = "Entire Part";
                        createNewComponentBuilder1.LayerOption =
                            CreateNewComponentBuilder.ComponentLayerOptionType.Work;
                        createNewComponentBuilder1.NewComponentName = fileName;
                        createNewComponentBuilder1.ComponentOrigin =
                            CreateNewComponentBuilder.ComponentOriginType.Absolute;
                        Block blockFromFeature = (Block)feature1;
                        Body[] blockBody = blockFromFeature.GetBodies();
                        Body body1 = blockBody[0];
                        createNewComponentBuilder1.ObjectForNewComponent.Add(body1);
                        createNewComponentBuilder1.NewFile = fileNew1;
                        createNewComponentBuilder1.Commit();
                        createNewComponentBuilder1.Destroy();
                        Component[] assmComponents = _workPart.ComponentAssembly.RootComponent.GetChildren();
                        BasePart basePart1 = (BasePart)assmComponents[0].Prototype;
                        session_.Parts.SetDisplay(basePart1, false, false, out PartLoadStatus partLoadStatus1);
                        partLoadStatus1.Dispose();
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        checkBoxUpperComp.Checked = isUpper;
                        Session.UndoMarkId makeExpressions =
                            session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

                        string stringUnit = _displayPart.PartUnits == BasePart.Units.Inches ? "Inch" : "MilliMeter";
                        Unit unit1 = _workPart.UnitCollection.FindObject(stringUnit);
                        _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                        _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                        _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);


                        _workPart.Expressions.CreateExpression("String", "Burnout=\"no\"");
                        //if (checkBoxGrind.Checked)
                        //{
                        //    _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                        //    if (comboBoxTolerance.SelectedIndex != -1)
                        //        _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"" + comboBoxTolerance.Text + "\"");
                        //    else
                        //        _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        //}
                        //else
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");

                            _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }

                        //if (checkBoxBurnDirX.Checked) _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
                        //else if (checkBoxBurnDirY.Checked) _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
                        //else if (checkBoxBurnDirZ.Checked) _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
                        //else 
                        _workPart.Expressions.CreateExpression("String", "BurnDir=\"none\"");

                        session_.UpdateManager.DoUpdate(makeExpressions);
                        _workPart.Layers.WorkLayer = 15;
                        Feature nullFeaturesFeature2 = null;
                        ExtractFaceBuilder extractFaceBuilder1 =
                            _workPart.Features.CreateExtractFaceBuilder(nullFeaturesFeature2);
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
                        session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                            out PartLoadStatus partLoadStatus2);
                        partLoadStatus2.Dispose();
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        _displayPart.WCS.SetOriginAndMatrix(prevOrigin, prevOrientation);
                        _displayPart.WCS.Visibility = true;
                        assmComponents[0].RedisplayObject();
                        session_.Parts.Work.ComponentAssembly.ReplaceReferenceSet(assmComponents[0], "BODY");
                        Tag cycleRefSet = NXOpen.Tag.Null;
                        do
                        {
                            ufsession_.Obj.CycleObjsInPart(_workPart.Tag, UF_reference_set_type, ref cycleRefSet);
                            if (cycleRefSet == NXOpen.Tag.Null)
                                break;
                            ufsession_.Obj.AskName(cycleRefSet, out string name);
                            if (name != "BODY") continue;
                            Tag[] refSetMembers = { assmComponents[0].Tag };
                            ufsession_.Assem.AddRefSetMembers(cycleRefSet, refSetMembers.Length, refSetMembers);
                            ufsession_.Assem.ReplaceRefset(refSetMembers.Length, refSetMembers, "BODY");
                        }
                        while (cycleRefSet != NXOpen.Tag.Null);
                    }

                    UpdateFormText();
                    //if (userMaterial.Length != 0)
                    //{
                    //    textBoxUserMaterial.Text = userMaterial;
                    //    comboBoxCompName.SelectedIndex = compNameIndex;
                    //    listBoxMaterial.SelectedIndex = -1;
                    //    listBoxMaterial.Enabled = false;
                    //    groupBoxColor.Enabled = true;
                    //}
                    //else
                    {
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = compMaterialIndex;
                        listBoxMaterial.Enabled = true;
                        groupBoxColor.Enabled = true;
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
                    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                    UI.GetUI().NXMessageBox.Show("Caught exception in Create Component", NXMessageBox.DialogType.Error,
                        ex.Message);
                    session_.UndoToLastVisibleMark();
                    List<NXObject> delObjects = _workPart.Features.Cast<Feature>()
                        .Where(delFeature => delFeature.Name == "DYNAMIC BLOCK").Cast<NXObject>().ToList();
                    delObjects.AddRange(_workPart.Lines.Cast<Line>().Where(nLine => nLine.Name != string.Empty));
                    delObjects.AddRange(_workPart.Points.Cast<Point>().Where(nPoint => nPoint.Name != string.Empty));
                    if (delObjects.Count > 0)
                        session_.__DeleteObjects(delObjects.ToArray());
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

                    ufsession_.Ui.AskInfoUnits(out int infoUnits);
                    Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;

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
                    //string userMaterial = textBoxUserMaterial.Text;
                    int compNameIndex = comboBoxCompName.SelectedIndex;
                    int compMaterialIndex = listBoxMaterial.SelectedIndex;
                    bool isUpper = checkBoxUpperComp.Checked;

                    CtsAttributes selectedName = (CtsAttributes)comboBoxCompName.SelectedItem;
                    CtsAttributes selectecMaterial = new CtsAttributes();

                    if (listBoxMaterial.SelectedIndex != -1)
                    {
                        selectecMaterial = (CtsAttributes)listBoxMaterial.SelectedItem;
                    }
                    else
                    {
                        selectecMaterial.AttrName = "MATERIAL";
                        //selectecMaterial.AttrValue = textBoxUserMaterial.Text;
                    }

                    bool numberIsValid = FormatDetailNumber();
                    if (numberIsValid)
                    {
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;

                        _displayPart.WCS.Visibility = false;
                        _displayPart.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                        Point3d uprCompOrigin = new Point3d();
                        Matrix3x3 uprCompOrientation = new Matrix3x3();

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

                        BlockFeatureBuilder blockFeatureBuilder1 = _workPart.Features.CreateBlockFeatureBuilder(null);


                        blockFeatureBuilder1.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                        Body[] targetBodies1 = new Body[1];
                        Body nullBody = null;
                        targetBodies1[0] = null;
                        blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);

                        blockFeatureBuilder1.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                        Point3d originPoint1;

                        if (checkBoxUpperComp.Checked)
                        {
                            Vector3d xAxis = new Vector3d(uprCompOrientation.Xx, uprCompOrientation.Xy,
                                uprCompOrientation.Xz);
                            Vector3d yAxis = new Vector3d(uprCompOrientation.Yx, uprCompOrientation.Yy,
                                uprCompOrientation.Yz);

                            originPoint1 = uprCompOrigin;
                            blockFeatureBuilder1.SetOrientation(xAxis, yAxis);
                        }
                        else
                        {
                            Point3d lwrCompOrigin = new Point3d(bodyOrigin.X, bodyOrigin.Y, bodyOrigin.Z);
                            originPoint1 = lwrCompOrigin;
                        }

                        blockFeatureBuilder1.SetOriginAndLengths(originPoint1, length, width, height);

                        blockFeatureBuilder1.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);

                        Feature feature1 = blockFeatureBuilder1.CommitFeature();

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

                        int indexBslash = _workPart.FullPath.LastIndexOf("\\");
                        int indexDash = _workPart.FullPath.LastIndexOf("-");
                        string filePath = _workPart.FullPath.Remove(indexBslash + 1);
                        string assmName = _workPart.FullPath.Substring(indexBslash + 1, indexDash - 1 - indexBslash);
                        string fileName = textBoxDetailNumber.Text;

                        fileNew1.NewFileName = $"{filePath}\\{assmName}-{fileName}.prt";

                        fileNew1.MasterFileName = "";

                        fileNew1.UseBlankTemplate = true;

                        fileNew1.MakeDisplayedPart = false;

                        CreateNewComponentBuilder createNewComponentBuilder1 =
                            _workPart.AssemblyManager.CreateNewComponentBuilder();

                        createNewComponentBuilder1.ReferenceSet =
                            CreateNewComponentBuilder.ComponentReferenceSetType.EntirePartOnly;

                        createNewComponentBuilder1.ReferenceSetName = "Entire Part";

                        createNewComponentBuilder1.LayerOption =
                            CreateNewComponentBuilder.ComponentLayerOptionType.Work;

                        createNewComponentBuilder1.NewComponentName = fileName;

                        createNewComponentBuilder1.ComponentOrigin =
                            CreateNewComponentBuilder.ComponentOriginType.Absolute;

                        // get the body from the block that has been created

                        Block blockFromFeature = (Block)feature1;

                        Body[] blockBody = blockFromFeature.GetBodies();

                        Body body1 = blockBody[0];

                        bool added1;
                        added1 = createNewComponentBuilder1.ObjectForNewComponent.Add(body1);

                        createNewComponentBuilder1.NewFile = fileNew1;

                        createNewComponentBuilder1.Commit();

                        createNewComponentBuilder1.Destroy();

                        Component[] assmComponents = _workPart.ComponentAssembly.RootComponent.GetChildren();

                        BasePart basePart1 = (BasePart)assmComponents[0].Prototype;
                        session_.Parts.SetDisplay(basePart1, false, false, out PartLoadStatus partLoadStatus1);
                        partLoadStatus1.Dispose();

                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

                        // create component description attributes

                        checkBoxUpperComp.Checked = isUpper;

                        Session.UndoMarkId makeExpressions =
                            session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

                        if (_displayPart.PartUnits == BasePart.Units.Inches)
                        {
                            Unit unit1 = _workPart.UnitCollection.FindObject("Inch");

                            _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
                        }
                        else
                        {
                            Unit unit1 = _workPart.UnitCollection.FindObject("MilliMeter");

                            _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                            _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
                        }

                        _workPart.Expressions.CreateExpression("String", "Burnout=\"no\"");
                        //if (checkBoxGrind.Checked)
                        //{
                        //    _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                        //    if (comboBoxTolerance.SelectedIndex != -1)
                        //        _workPart.Expressions.CreateExpression("String",
                        //            $"GrindTolerance=\"{comboBoxTolerance.Text}\"");
                        //    else
                        //        _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        //}
                        //else
                        {
                            _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");

                            _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                        }

                        //if (checkBoxBurnDirX.Checked)
                        //    _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
                        //else if (checkBoxBurnDirY.Checked)
                        //    _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
                        //else if (checkBoxBurnDirZ.Checked)
                        //    _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
                        //else
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

                        session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                            out PartLoadStatus partLoadStatus2);
                        partLoadStatus2.Dispose();

                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

                        _displayPart.WCS.Visibility = true;
                        assmComponents[0].RedisplayObject();

                        session_.Parts.Work.ComponentAssembly.ReplaceReferenceSet(assmComponents[0], "BODY");

                        Tag cycleRefSet = NXOpen.Tag.Null;

                        do
                        {
                            ufsession_.Obj.CycleObjsInPart(_workPart.Tag, UF_reference_set_type, ref cycleRefSet);
                            if (cycleRefSet == NXOpen.Tag.Null)
                                break;
                            ufsession_.Obj.AskName(cycleRefSet, out string name);

                            if (name != "BODY") continue;
                            Tag[] refSetMembers = { assmComponents[0].Tag };
                            ufsession_.Assem.AddRefSetMembers(cycleRefSet, refSetMembers.Length, refSetMembers);
                            ufsession_.Assem.ReplaceRefset(refSetMembers.Length, refSetMembers, "BODY");
                        }
                        while (cycleRefSet != NXOpen.Tag.Null);
                    }

                    UpdateFormText();

                    //if (userMaterial.Length != 0)
                    //{
                    //    //textBoxUserMaterial.Text = userMaterial;
                    //    comboBoxCompName.SelectedIndex = compNameIndex;
                    //    listBoxMaterial.SelectedIndex = -1;
                    //    listBoxMaterial.Enabled = false;
                    //    groupBoxColor.Enabled = true;
                    //}
                    //else
                    {
                        comboBoxCompName.SelectedIndex = compNameIndex;
                        listBoxMaterial.SelectedIndex = compMaterialIndex;
                        listBoxMaterial.Enabled = true;
                        groupBoxColor.Enabled = true;
                    }
                }
                catch (NXException ex)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    session_.Parts.SetWork(_originalWorkPart);
                    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                    UI.GetUI().NXMessageBox.Show("Caught exception in Create Component", NXMessageBox.DialogType.Error,
                        ex.Message);
                    session_.UndoToLastVisibleMark();
                    List<NXObject> delObjects = _workPart.Features.Cast<Feature>()
                        .Where(delFeature => delFeature.Name == "DYNAMIC BLOCK").Cast<NXObject>().ToList();
                    delObjects.AddRange(_workPart.Lines.Cast<Line>().Where(nLine => nLine.Name != string.Empty));
                    delObjects.AddRange(_workPart.Points.Cast<Point>().Where(nPoint => nPoint.Name != string.Empty));
                    if (delObjects.Count > 0)
                        session_.__DeleteObjects(delObjects.ToArray());
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

                    BasePart dispComp = (BasePart)_changeColorComponent.Prototype;
                    session_.Parts.SetDisplay(dispComp, false, false, out PartLoadStatus displayLoadStatus);
                    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                    displayLoadStatus.Dispose();

                    List<Body> bodyCollection = _displayPart.Bodies.Cast<Body>()
                        .Where(solidBody => solidBody.Layer == 1)
                        .ToList();

                    if (bodyCollection.Count == 1)
                    {
                        // Get selected solid body feature faces

                        List<Face> featureFaces = new List<Face>();

                        ufsession_.Modl.AskBodyFeats(bodyCollection[0].Tag, out Tag[] features);

                        ufsession_.Modl.AskFeatType(features[0], out string featureType);
                        if (featureType == "EXTRUDE" || featureType == "BLOCK" || featureType == "BREP")
                        {
                            ufsession_.Modl.AskFeatFaces(features[0], out Tag[] facesOfFeature);

                            featureFaces.AddRange(facesOfFeature.Select(face => (Face)NXObjectManager.Get(face)));
                        }

                        // Convert allFaces array to allFacesList to allow removal of feature faces

                        Face[] allFaces = bodyCollection[0].GetFaces();
                        List<Face> allFacesList = allFaces.ToList();

                        List<Face> faces = new List<Face>();
                        List<int> colors = new List<int>();

                        foreach (Face face in featureFaces)
                            allFacesList.Remove(face);

                        // Fill face list and color list with remaining faces

                        foreach (Face selectedFace in allFacesList)
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

                        for (int i = 0; i < faces.Count; i++)
                        {
                            faces[i].Color = colors[i];
                            faces[i].RedisplayObject();
                        }

                        changeColorCheckBox.Checked = false;

                        session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                            out PartLoadStatus displayLoadStatus1);
                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();
                        session_.Parts.SetWork(_originalWorkPart);
                        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                        _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
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
                    session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                        out PartLoadStatus displayLoadStatus2);
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    session_.Parts.SetWork(_originalWorkPart);
                    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
                    _originalWorkPart = _workPart; _originalDisplayPart = _displayPart; ;
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
            Face[] bodyFaces = block.GetFaces();

            foreach (Face bFace in bodyFaces)
            {
                if (bFace.SolidFaceType != Face.FaceType.Planar)
                    continue;

                double[] vec1 =
                {
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zx,
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zy,
                    _displayPart.WCS.CoordinateSystem.Orientation.Element.Zz
                };

                double[] point = new double[3];
                double[] vec2 = new double[3];
                double[] box = new double[6];
                int isEqualVec;
                ufsession_.Modl.AskFaceData(bFace.Tag, out _, point, vec2, box, out _, out _, out _);
                ufsession_.Vec3.IsParallel(vec1, vec2, .001, out int isParallel);

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
            string compNameIncrement = textBoxDetailNumber.Text;
            bool isConverted = int.TryParse(compNameIncrement, out int compNameResult);
            int next = compNameResult;
            string originalPath = _originalWorkPart.FullPath;
            int indexLastDash = originalPath.LastIndexOf("-");
            string filePath = $"{_originalWorkPart.FullPath.Remove(indexLastDash + 1)}{compNameIncrement}.prt";
            int status = ufsession_.Part.IsLoaded(filePath);

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

        private void UpdateFormText()
        {
            int indexOf = _workPart.FullPath.LastIndexOf("\\");
            string fullName = _workPart.FullPath.Substring(indexOf + 1);
            string formatName = fullName.Substring(0, fullName.Length - 4);
            Text = formatName;
        }

        private void ResetForm(Part wp)
        {
            _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

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

            List<int> names = wp.ComponentAssembly.RootComponent.GetChildren()
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

            int lastComponentName = names[names.Count - 1] + 1;

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

            int lastDirIndex = partToCheck.FullPath.LastIndexOf("\\");
            string subAssemName = partToCheck.FullPath.Substring(lastDirIndex + 1);

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
            int lastDash = comp.DisplayName.LastIndexOf('-');
            if (lastDash != -1)
            {
                string subAssemName =
                    comp.DisplayName.Substring(lastDash + 1, comp.DisplayName.Length - (lastDash + 1));
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
            buttonAutoUpr.Enabled = false;
            buttonAutoLwr.Enabled = false;
            buttonUprRetAssm.Enabled = false;
            buttonLwrRetAssm.Enabled = false;
            // create list of component names
            int selectedName;
            selectedName = comboBoxCompName.SelectedIndex;
            comboBoxCompName.Items.Clear();

            foreach (CtsAttributes name in _compNames)
                comboBoxCompName.Items.Add(name);

            if (selectedName != -1)
                comboBoxCompName.SelectedIndex = selectedName;

            // create list of material types
            listBoxMaterial.Items.Clear();
            //textBoxUserMaterial.Text = string.Empty;

            foreach (CtsAttributes matl in _compMaterials)
                listBoxMaterial.Items.Add(matl);

            LoadGridSizes();
        }

        private void NameCheckFailed()
        {
            textBoxDetailNumber.Enabled = false;
            comboBoxCompName.Enabled = false;
            listBoxMaterial.Enabled = false;
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
            saveAsButton.Enabled = true;
            copyButton.Enabled = true;
        }


        private void CreateRefSetsCategories()
        {
            Session.UndoMarkId markId1;
            markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Reference Sets");

            bool isBodyRefSet = false;
            ReferenceSet referenceSet1 = null;

            foreach (ReferenceSet bRef in _workPart.GetAllReferenceSets())
            {
                if (bRef.Name != "BODY") continue;
                isBodyRefSet = true;
                referenceSet1 = bRef;
            }

            if (isBodyRefSet)
            {
                ReferenceSet referenceSet2 = _workPart.CreateReferenceSet();

                referenceSet2.SetName("SUB_TOOL");

                referenceSet1.SetAddComponentsAutomatically(false, false);
                referenceSet2.SetAddComponentsAutomatically(false, false);

                NXObject[] objects1 = new NXObject[1];
                NXObject[] objects2 = new NXObject[1];

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

                NXObject[] objects1 = new NXObject[1];
                NXObject[] objects2 = new NXObject[1];

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

            int[] bodyLayers = new int[1];
            bodyLayers[0] = 1;
            _workPart.LayerCategories.CreateCategory("BODY", "", bodyLayers);

            int[] burnoutLayers = new int[6];
            burnoutLayers[0] = 100;
            burnoutLayers[1] = 101;
            burnoutLayers[2] = 102;
            burnoutLayers[3] = 103;
            burnoutLayers[4] = 104;
            burnoutLayers[5] = 105;
            _workPart.LayerCategories.CreateCategory("BURNOUT", "", burnoutLayers);

            int[] constructionLayers = new int[9];
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

            int[] datumLayers = new int[1];
            datumLayers[0] = 255;
            _workPart.LayerCategories.CreateCategory("DATUM", "", datumLayers);

            int[] fastenerLayers = new int[1];
            fastenerLayers[0] = 99;
            _workPart.LayerCategories.CreateCategory("FASTENERS", "", fastenerLayers);

            int[] handlingLayers = new int[1];
            handlingLayers[0] = 98;
            _workPart.LayerCategories.CreateCategory("HANDLINGHOLES", "", handlingLayers);

            int[] holeChartLayers = new int[1];
            holeChartLayers[0] = 230;
            _workPart.LayerCategories.CreateCategory("HOLECHARTTEXT", "", holeChartLayers);

            int[] nestedBlkLayers = new int[1];
            nestedBlkLayers[0] = 96;
            _workPart.LayerCategories.CreateCategory("NESTEDBLOCKS", "", nestedBlkLayers);

            int[] orderBlkLayers = new int[1];
            orderBlkLayers[0] = 250;
            _workPart.LayerCategories.CreateCategory("ORDERBLOCK", "", orderBlkLayers);

            int[] subToolLayers = new int[6];
            subToolLayers[0] = 15;
            subToolLayers[1] = 16;
            subToolLayers[2] = 17;
            subToolLayers[3] = 18;
            subToolLayers[4] = 19;
            subToolLayers[5] = 20;
            _workPart.LayerCategories.CreateCategory("SUBTOOL", "", subToolLayers);

            int[] titleBlkLayers = new int[1];
            titleBlkLayers[0] = 200;
            _workPart.LayerCategories.CreateCategory("TITLEBLOCK", "", titleBlkLayers);

            int[] toolingHoleLayers = new int[1];
            toolingHoleLayers[0] = 97;
            _workPart.LayerCategories.CreateCategory("TOOLINGHOLES", "", toolingHoleLayers);

            int[] wireStartHoleLayers = new int[1];
            wireStartHoleLayers[0] = 94;
            _workPart.LayerCategories.CreateCategory("WIRESTARTHOLE", "", wireStartHoleLayers);

            _displayPart.Layers.SetState(3, State.WorkLayer);
        }

        private void CreateAutoSizeUdo()
        {
            //: 1535022'

            UserDefinedClass myUdOclass = null;

            try
            {
                myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");
            }
            catch (NXException ex) when (ex.ErrorCode == 1535022)
            {
                AutoSizeComponent.initializeUDO(false);
                myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");
            }

            if (myUdOclass is null)
                return;

            UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

            if (currentUdo.Length != 0)
                return;

            BasePart myBasePart = _workPart;
            UserDefinedObjectManager myUdOmanager = myBasePart.UserDefinedObjectManager;
            UserDefinedObject myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
            UserDefinedObject.LinkDefinition[] myLinks = new UserDefinedObject.LinkDefinition[1];
            int numBodies = _workPart.Bodies.Cast<Body>().Count(body => body.Layer == 1);

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

        //private void SetWcsToWorkPart(Component compRefCsys)
        //{
        //    session_.SetUndoMark(Session.MarkVisibility.Visible, "SetWcsToWorkPart");

        //    if (compRefCsys is null)
        //    {
        //        foreach (Feature featBlk in _workPart.Features)
        //        {
        //            if (featBlk.FeatureType != "BLOCK" || featBlk.Name != "DYNAMIC BLOCK")
        //                continue;

        //            Block block1 = (Block)featBlk;
        //            BlockFeatureBuilder blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
        //            Point3d bOrigin = blockFeatureBuilderMatch.Origin;
        //            blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);
        //            double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
        //            double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
        //            double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
        //            double[] initMatrix = new double[9];
        //            ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
        //            ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
        //            ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
        //            CartesianCoordinateSystem setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);
        //            _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin, setTempCsys.Orientation.Element);
        //        }

        //        return;
        //    }

        //    BasePart compBase = (BasePart)compRefCsys.Prototype;

        //    session_.Parts.SetDisplay(compBase, false, false, out PartLoadStatus setDispLoadStatus);
        //    setDispLoadStatus.Dispose();
        //    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

        //    bool isBlockComp = false;

        //    foreach (Feature featBlk in _workPart.Features)
        //    {
        //        if (featBlk.FeatureType != "BLOCK") continue;
        //        if (featBlk.Name != "DYNAMIC BLOCK") continue;
        //        isBlockComp = true;

        //        Block block1 = (Block)featBlk;

        //        BlockFeatureBuilder blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
        //        Point3d bOrigin = blockFeatureBuilderMatch.Origin;
        //        blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

        //        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
        //        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
        //        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
        //        double[] initMatrix = new double[9];
        //        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
        //        ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
        //        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
        //        CartesianCoordinateSystem setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

        //        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin, setTempCsys.Orientation.Element);

        //        CartesianCoordinateSystem featBlkCsys = _displayPart.WCS.Save();
        //        featBlkCsys.SetName("EDITCSYS");
        //        featBlkCsys.Layer = 254;

        //        NXObject[] addToBody = { featBlkCsys };

        //        foreach (ReferenceSet bRefSet in _displayPart.GetAllReferenceSets())
        //            if (bRefSet.Name == "BODY")
        //                bRefSet.AddObjectsToReferenceSet(addToBody);

        //        session_.Parts.SetDisplay(_originalDisplayPart, false, false, out PartLoadStatus setDispLoadStatus1);
        //        setDispLoadStatus1.Dispose();

        //        session_.Parts.SetWorkComponent(compRefCsys, out PartLoadStatus partLoadStatusWorkComp);
        //        partLoadStatusWorkComp.Dispose();
        //        _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;

        //        foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
        //        {
        //            if (wpCsys.Layer != 254)
        //                continue;

        //            if (wpCsys.Name != "EDITCSYS")
        //                continue;

        //            NXObject csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);
        //            CartesianCoordinateSystem editCsys = (CartesianCoordinateSystem)csysOccurrence;

        //            if (editCsys != null)
        //                _displayPart.WCS.SetOriginAndMatrix(editCsys.Origin, editCsys.Orientation.Element);

        //            session_.__DeleteObjects(editCsys);
        //        }
        //    }

        //    if (isBlockComp)
        //        return;

        //    __display_part_ = _originalDisplayPart;
        //    __work_component_ = compRefCsys;
        //    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out _);
        //    _workPart = session_.Parts.Work; _displayPart = session_.Parts.Display; ;
        //}

        private static List<Body> SelectMultipleBodies()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);
            List<Body> bodySelection = new List<Body>();

            Selection.Response sel = TheUISession.SelectionManager.SelectTaggedObjects("Select Bodies", "Select Bodies",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject[] selectedBodyArray);

            if (sel != Selection.Response.Ok)
                return bodySelection;

            bodySelection.AddRange(selectedBodyArray.Cast<Body>());
            return bodySelection;
        }

        //private Component SelectOneComponent(string prompt)
        //{
        //    Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
        //    mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
        //    Selection.Response sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
        //        Selection.SelectionScope.AnyInAssembly,
        //        Selection.SelectionAction.ClearAndEnableSpecific,
        //        false, false, mask, out TaggedObject selectedComp, out _);

        //    if (!((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName)))
        //        return null;

        //    Component compSelection = (Component)selectedComp;
        //    return compSelection;
        //}






        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.udoComponentBuilderGridIncrement = comboBoxGrid.Text;
            Settings.Default.comp_builder_4_digits = chk4Digits.Checked;
            Settings.Default.com_builder_any_assembly = chkAnyAssembly.Checked;
            Settings.Default.Save();
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
        }

        private void chkDigits_CheckedChanged(object sender, EventArgs e)
        {
            WorkPartChanged1(__work_part_);
        }

        private static bool ConvertUnits(double[] distances)
        {
            if (_workPart.PartUnits != BasePart.Units.Millimeters)
                return false;

            for (int i = 0; i < distances.Length; i++)
                distances[i] /= 25.4d;

            return true;
        }


        private static void RoundAndTruncate(double number, out double roundValue, out double truncateValue,
            out double fractionValue)
        {
            roundValue = Math.Round(number, 3);
            truncateValue = Math.Truncate(roundValue);
            fractionValue = roundValue - truncateValue;
        }


        private static void Distance0(double[] distances)
        {
            for (int i = 0; i < 3; i++)
            {
                RoundAndTruncate(distances[i], out double roundValue, out double truncateValue,
                    out double fractionValue);

                if (Math.Abs(fractionValue) <= Tolerance)
                {
                    distances[i] = roundValue;
                    continue;
                }

                for (double ii = .125; ii <= 1; ii += .125)
                {
                    if (!(fractionValue <= ii))
                        continue;

                    double finalValue = truncateValue + ii;
                    distances[i] = finalValue;
                    break;
                }
            }
        }

        private static void MinCorner3(double[] minCorner)
        {
            for (int i = 0; i < 3; i++)
            {
                bool isNegative = false;

                if (minCorner[i] < 0)
                {
                    minCorner[i] *= -1;
                    isNegative = true;
                }

                RoundAndTruncate(minCorner[i], out double roundValue, out double truncateValue,
                    out double fractionValue);

                if (Math.Abs(fractionValue) <= Tolerance)
                {
                    minCorner[i] = roundValue;
                    continue;
                }

                for (double ii = .125; ii <= 1; ii += .125)
                {
                    if (!(fractionValue <= ii))
                        continue;

                    double finalValue = truncateValue + ii;

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

        private enum AssemblyComponent
        {
            Lower,
            Upper,
            None
        }

        private void btnMakeUnique_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = Ui.Selection.SelectSingleComponent();

                if (selected is null)
                    return;

                var parent = selected.Parent;
                var op = parent.DisplayName.__AskDetailOp();
                var owning_ass = parent.__Prototype();
                var builder = __work_part_.AssemblyManager.CreateMakeUniquePartBuilder();

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    var detail = textBoxDetailNumber.Text;

                    if (string.IsNullOrEmpty(detail))
                    {
                        print_("please type a detail number");
                        return;
                    }

                    var folder = GFolder.Create(parent.__Prototype().FullPath);
                    var new_path = folder.file_detail0(op, detail);
                    builder.SelectedComponents.Add(selected);
                    selected.__Prototype().SetMakeUniqueName(new_path);
                    builder.Commit();
                    parent.GetChildren().Where(c => c.DisplayName.EndsWith(detail)).ToList().ForEach(c => c.SetName(detail));
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public enum CtsComponentColor
        {
            Orange = 11,
            LightRedOrange = 113,
            DarkDullGreen = 101,
            ObscureDullGreen = 144,
            LightDullAzure = 92,
            MediumAzureCyan = 98,
            MediumAzureBlue = 134,
            AquaMarine = 14,
            DarkWeakRed = 167,
            DarkWeakMagenta = 166,
            Purple = 12,
            DarkHardOrange = 84,
            DarkDullBlue = 171,
            LightGrey = 87,
            DarkGrey = 130
        }

        internal class CtsComponentType
        {
            public CtsComponentType(string name, CtsComponentColor color)
            {
                ColorName = name;
                ComponentColor = (int)color;
            }

            public CtsComponentType(string attrName, string componentName)
            {
                AttributeName = attrName;
                ComponentName = componentName;
            }

            public CtsComponentType(string attrName, string material, string componentName, CtsComponentColor color)
            {
                AttributeName = attrName;
                Material = material;
                ComponentName = componentName;
                ComponentColor = (int)color;
            }

            public CtsComponentType(string attrName, string material, string componentName, CtsComponentColor color,
                bool burnout, bool grind, bool dieset)
            {
                AttributeName = attrName;
                Material = material;
                ComponentName = componentName;
                ComponentColor = (int)color;
                IsBurnout = burnout;
                IsDieset = dieset;
                IsGrind = grind;
            }

            public string AttributeName { get; set; }

            public string Material { get; set; }

            public string ComponentName { get; set; }

            public string ColorName { get; set; }

            public int ComponentColor { get; set; }

            public bool IsBurnout { get; set; }

            public bool IsDieset { get; set; }

            public bool IsGrind { get; set; }

            public override string ToString()
            {
                return AttributeName == null
                    ? string.Format("{0}", ColorName)
                    : string.Format("{0}", ComponentName);
            }
        }

        public enum CtsSubtoolColor
        {
            Blue = 211,
            Yellow = 42
        }

        private void buttonEditDynamic_Click(object sender, EventArgs e)
        {
            try
            {
                SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component for Dynamic Edit");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    EditDynamicDisplayPart(editComponent);
                else
                    EditDynamicWorkPart(editComponent);
            }
            catch (Exception ex)
            {
                EnableForm();
                ex.__PrintException();
            }
        }

        private void buttonEditMove_Click(object sender, EventArgs e)
        {
            try
            {
                SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Move");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    if (!__work_part_.__HasDynamicBlock())
                    {
                        ResetNonBlockError();
                        NXMessage("Not a block component");
                        return;
                    }

                    if (_isNewSelection)
                    {
                        CreateEditData(editComponent);
                        _isNewSelection = false;
                    }
                }
                else
                {
                    _updateComponent = editComponent;

                    if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                        return;

                    if (!IsBlockComponent(editComponent))
                    {
                        ResetNonBlockError();
                        NXMessage("Not a block component");
                        return;
                    }
                }

                DisableForm();
                List<Point> pHandle = SelectHandlePoint();
                _isDynamic = false;

                while (pHandle.Count == 1)
                {
                    _distanceMoved = 0;
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];
                    __display_part_.WCS.Visibility = false;
                    string message = "Select New Position";
                    double[] screenPos = new double[3];
                    IntPtr motionCbData = IntPtr.Zero;
                    IntPtr clientData = IntPtr.Zero;

                    using (session_.__UsingLockUiFromCustom())
                    {
                        SetModelingView();

                        ufsession_.Ui.SpecifyScreenPosition(
                            message,
                            MotionCallback,
                            motionCbData,
                            screenPos,
                            out Tag viewTag,
                            out int response
                        );

                        if (response != UF_UI_PICK_RESPONSE)
                            continue;

                        UpdateDynamicHandles();
                        ShowDynamicHandles();
                        ShowTemporarySizeText();
                        pHandle = SelectHandlePoint();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                EnableForm();
            }
        }

        private void buttonEditMatch_Click(object sender, EventArgs e) => EditMatch();

        private void buttonEditSize_Click(object sender, EventArgs e) => EditSize();

        private void buttonEditAlign_Click(object sender, EventArgs e) => EditAlign();

        private void buttonAlignComponent_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    NXMessage("This function is not allowed in this context, must be at assembly level");
                    return;
                }

                _updateComponent = editComponent;

                if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                    return;

                if (!IsBlockComponent(editComponent))
                {
                    ResetNonBlockError();
                    NXMessage("Not a Block Component");
                    return;
                }

                var pHandle = SelectHandlePoint();
                _isDynamic = true;

                while (pHandle.Count == 1)
                {
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];

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

                    UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                    double[] basePoint = new double[3];
                    int response;

                    using (session_.__UsingLockUiFromCustom())
                        ufsession_.Ui.PointConstruct(
                            "Select Reference Point",
                            ref pbMethod,
                            out Tag selection,
                            basePoint,
                            out response
                        );

                    if (response != UF_UI_OK)
                    {
                        pHandle = UpdateCreateSelect(editComponent);
                        continue;
                    }

                    double[] mappedBase = basePoint.__ToPoint3d().__MapAcsToWcs().__ToArray();
                    double[] mappedPoint = pointPrototype.Coordinates.__MapAcsToWcs().__ToArray();
                    double distance;
                    int index;
                    string letter = $"{pointPrototype.Name.ToCharArray()[3]}";

                    switch (letter)
                    {
                        case "X":
                            index = 0;
                            break;
                        case "Y":
                            index = 1;
                            break;
                        case "Z":
                            index = 2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    distance = Math.Abs(mappedPoint[index] - mappedBase[index]);

                    if (mappedBase[index] < mappedPoint[index])
                        distance *= -1;

                    MoveObjects(movePtsFull.ToArray(), distance, letter);
                    pHandle = UpdateCreateSelect(editComponent);
                }
            }
        }

        private void buttonAlignEdgeDistance_Click(object sender, EventArgs e) => AlignEdgeDistance();

        private void buttonApply_Click(object sender, EventArgs e) => Apply();

        private void EditMatch()
        {
            try
            {
                SetDispUnits();

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

                EditMatch();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                EnableForm();
            }
        }

        private void AlignEdgeDistance()
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align Edge");

                AlignEdgeDistance();
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
                SetDispUnits();


                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                EdgeAlign();
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

        private void EdgeAlign()
        {
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

            if (!IsBlockComponent(editComponent))
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
                            throw new ArgumentOutOfRangeException();
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

        private void EditDynamicWorkPart(Component editComponent)
        {
            print_("here");
            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;

            bool isBlockComponent;

            using (session_.__UsingSuppressDisplay())
                isBlockComponent = IsBlockComponent(editComponent);

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return;
            }

            DisableForm();
            var pHandle = SelectHandlePoint();
            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                string message = "Select New Position";
                double[] screenPos = new double[3];
                IntPtr motionCbData = IntPtr.Zero;
                __display_part_.WCS.Visibility = false;
                SetModelingView();
                EditDynamic(ref pHandle, message, screenPos, motionCbData);
            }

            EnableForm();
        }

        private void EditDynamicDisplayPart(Component editComponent)
        {
            if (!__work_part_.__HasDynamicBlock())
            {
                ResetNonBlockError();
                NXMessage("Not a block component");
                return;
            }

            DisableForm();

            if (_isNewSelection)
                using (session_.__UsingSuppressDisplay())
                {
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }

            List<Point> pHandle = SelectHandlePoint();
            _isDynamic = true;

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
                NewMethod();

                using (session_.__UsingLockUiFromCustom())
                {
                    ufsession_.Ui.SpecifyScreenPosition(
                        message,
                        MotionCallback,
                        motionCbData,
                        screenPos,
                        out viewTag,
                        out int response
                    );

                    if (response != UF_UI_PICK_RESPONSE)
                        continue;

                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }
            }

            EnableForm();
        }

        private static void NewMethod()
        {
            ModelingView mView = (ModelingView)__display_part_.Views.WorkView;
            __display_part_.Views.WorkView.Orient(mView.Matrix);
            __display_part_.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
        }

        private void EditDynamic()
        {

        }

        private void EditSizeWork(Component editComponent)
        {
            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;

            if (IsBlockComponent1(editComponent))
            {
                //UpdateDynamicBlock(editComponent);
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
                return;
            }
        }


        private void EditSize()
        {
            try
            {
                SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Edit Size");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    EditSize(editComponent);
                else
                    EditSizeWork(editComponent);
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

        private void EditSize(Component editComponent)
        {
            if (!__work_part_.__HasDynamicBlock())
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

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                Point3d blockOrigin;
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
                    out _,
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
                        string letter = $"{pointPrototype.Name.ToCharArray()[3]}";
                        double distance;
                        List<Line> lineObjs;
                        List<Line> axisLines;

                        switch (pointPrototype.Name)
                        {
                            case "POSX":
                                lineObjs = posXObjs;
                                axisLines = allxAxisLines;
                                distance = editSize - blockLength;
                                break;
                            case "NEGX":
                                lineObjs = negXObjs;
                                axisLines = allxAxisLines;
                                distance = blockLength - editSize;
                                break;
                            case "POSY":
                                lineObjs = posYObjs;
                                axisLines = allyAxisLines;
                                distance = editSize - blockWidth;
                                break;
                            case "NEGY":
                                lineObjs = negYObjs;
                                axisLines = allyAxisLines;
                                distance = blockWidth - editSize;
                                break;
                            case "POSZ":
                                lineObjs = posZObjs;
                                axisLines = allzAxisLines;
                                distance = editSize - blockHeight;
                                break;
                            case "NEGZ":
                                lineObjs = negZObjs;
                                axisLines = allzAxisLines;
                                distance = blockHeight - editSize;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        movePtsFull.AddRange(lineObjs);

                        EditSizeOrAlign(
                            distance,
                            movePtsHalf,
                            movePtsFull,
                            axisLines,
                            letter,
                            pointPrototype.Name.StartsWith("POS")
                        );

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
            return;
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
                if (Math.Abs(distance) < 0.001)
                    return;

                __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                MoveObjectBuilder builder = __work_part_.BaseFeatures.CreateMoveObjectBuilder(null);

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    // look at other enum values.
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


        private void SetWcsWorkComponent(Component compRefCsys)
        {
            try
            {
                var _originalDisplayPart = __display_part_;

                if (compRefCsys != null)
                {
                    BasePart compBase = (BasePart)compRefCsys.Prototype;

                    session_.Parts.SetDisplay(
                        compBase,
                        false,
                        false,
                        out PartLoadStatus setDispLoadStatus
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
                            _isUprParallel = exp.RightHandSide.Contains("yes");

                        if (exp.Name == "lwrParallel")
                            _isLwrParallel = exp.RightHandSide.Contains("yes");
                    }

                    if (__work_part_.__HasDynamicBlock())
                    {

                        Block block1 = (Block)__work_part_.__DynamicBlock();

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch =
                            __work_part_.Features.CreateBlockFeatureBuilder(block1);
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

                        blockFeatureBuilderMatch.GetOrientation(
                            out Vector3d xAxis,
                            out Vector3d yAxis
                        );

                        double[] initOrigin = bOrigin.__ToArray();
                        double[] xVector = new double[] { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = new double[] { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] initMatrix = new double[9];
                        TheUFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                        TheUFSession.Csys.CreateMatrix(
                            initMatrix,
                            out Tag tempMatrix
                        );
                        TheUFSession.Csys.CreateTempCsys(
                            initOrigin,
                            tempMatrix,
                            out Tag tempCsys
                        );

                        CartesianCoordinateSystem setTempCsys = tempCsys.__To<CartesianCoordinateSystem>();

                        __display_part_.WCS.SetOriginAndMatrix(
                            setTempCsys.Origin,
                            setTempCsys.Orientation.Element
                        );

                        CartesianCoordinateSystem featBlkCsys = __display_part_.WCS.Save();
                        featBlkCsys.SetName("EDITCSYS");
                        featBlkCsys.Layer = 254;

                        NXObject[] addToBody = new NXObject[] { featBlkCsys };

                        foreach (ReferenceSet bRefSet in __display_part_.GetAllReferenceSets())
                            if (bRefSet.Name == "BODY")
                                bRefSet.AddObjectsToReferenceSet(addToBody);

                        __display_part_ = _originalDisplayPart;
                        __work_component_ = compRefCsys;

                        foreach (CartesianCoordinateSystem wpCsys in __work_part_.CoordinateSystems)
                        {
                            if (wpCsys.Layer == 254)
                            {
                                if (wpCsys.Name == "EDITCSYS")
                                {
                                    NXObject csysOccurrence;
                                    csysOccurrence =
                                        session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                    CartesianCoordinateSystem editCsys =
                                        (CartesianCoordinateSystem)csysOccurrence;

                                    if (editCsys != null)
                                    {
                                        __display_part_.WCS.SetOriginAndMatrix(
                                            editCsys.Origin,
                                            editCsys.Orientation.Element
                                        );
                                        _workCompOrigin = editCsys.Origin;
                                        _workCompOrientation = editCsys.Orientation.Element;
                                    }

                                    Session.UndoMarkId markDeleteObjs;
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
                else
                {
                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;

                    foreach (NXOpen.Expression exp in __work_part_.Expressions)
                    {
                        if (exp.Name == "uprParallel")
                            _isUprParallel = exp.RightHandSide.Contains("yes");

                        if (exp.Name == "lwrParallel")
                            _isLwrParallel = exp.RightHandSide.Contains("yes");
                    }

                    foreach (Feature featBlk in __work_part_.Features)
                    {
                        if (featBlk.FeatureType == "BLOCK")
                        {
                            if (featBlk.Name == "DYNAMIC BLOCK")
                            {
                                Block block1 = (Block)featBlk;

                                BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch =
                                    __work_part_.Features.CreateBlockFeatureBuilder(block1);
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

                                blockFeatureBuilderMatch.GetOrientation(
                                    out Vector3d xAxis,
                                    out Vector3d yAxis
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
                                    out Tag tempMatrix
                                );
                                TheUFSession.Csys.CreateTempCsys(
                                    initOrigin,
                                    tempMatrix,
                                    out Tag tempCsys
                                );
                                CartesianCoordinateSystem setTempCsys =
                                    (CartesianCoordinateSystem)
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
            using (session_.__UsingSuppressDisplay())
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
                    BlockFeatureBuilder blockFeatureBuilderMatch = __work_part_.Features.CreateBlockFeatureBuilder(block1);

                    //Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                    double mLength = blockFeatureBuilderMatch.Length.Value;
                    double mWidth = blockFeatureBuilderMatch.Width.Value;
                    double mHeight = blockFeatureBuilderMatch.Height.Value;

                    __work_part_ = __display_part_;

                    if (Math.Abs(mLength) < .001 || Math.Abs(mWidth) < .001 || Math.Abs(mHeight) < .001)
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


        private bool IsBlockComponent(Component editComponent)
        {
            if (!_isNewSelection)
                return true;

            __work_component_ = editComponent;

            if (!__work_part_.__HasDynamicBlock())
                return false;

            CreateEditData(editComponent);
            _isNewSelection = false;
            return true;
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
            foreach (NXOpen.Expression exp in __work_part_.Expressions)
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








        private void Dynamic(List<Point> pHandle)
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

                using (session_.__UsingLockUiFromCustom())
                {
                    ufsession_.Ui.SpecifyScreenPosition(
                        message,
                        MotionCallback,
                        motionCbData,
                        screenPos,
                        out viewTag,
                        out int response
                    );

                    if (response != UF_UI_PICK_RESPONSE)
                        continue;

                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }
            }
        }




        private void EditDynamic(
          ref List<Point> pHandle,
          string message,
          double[] screenPos,
          IntPtr motionCbData
      )
        {
            using (session_.__UsingLockUiFromCustom())
            {
                ufsession_.Ui.SpecifyScreenPosition(
                    message,
                    MotionCallback,
                    motionCbData,
                    screenPos,
                    out Tag viewTag,
                    out int response
                );

                if (response != UF_UI_PICK_RESPONSE)
                    return;

                UpdateDynamicHandles();
                ShowDynamicHandles();
                //ShowTemporarySizeText();
                pHandle = SelectHandlePoint();
            }
        }

        private void NewMethod49(
            ref List<Point> pHandle,
            string message,
            double[] screenPos,
            IntPtr motionCbData
        )
        {
            using (session_.__UsingLockUiFromCustom())
            {
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
            }
        }

        private void NewMethod53(
            ref List<Point> pHandle,
            string message,
            double[] screenPos,
            IntPtr motionCbData
        )
        {
            using (session_.__UsingLockUiFromCustom())
            {
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
                    ShowDynamicHandles();
                    ShowTemporarySizeText();
                    pHandle = SelectHandlePoint();
                }
            }
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
        }

        private void ResetNonBlockError()
        {
            EnableForm();
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
        }

        private void DeleteHandles()
        {
            using (session_.__UsingDoUpdate())
            {
                UserDefinedClass myUdOclass = GetDynamicHandle();

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
                        Point blkFeatBuilderPoint = __work_part_.Points.CreatePoint(blkOrigin);
                        blkFeatBuilderPoint.SetCoordinates(blkOrigin);
                        builder.OriginPoint = blkFeatBuilderPoint;
                        Point3d originPoint1 = blkOrigin;
                        builder.SetOriginAndLengths(originPoint1, length, width, height);
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
                UserDefinedClass myUdOclass = GetDynamicHandle();

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
                UserDefinedClass myUdOclass = GetDynamicHandle();

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

        private bool IsBlockComponent1(Component editComponent)
        {
            if (!_isNewSelection)
                return true;

            __work_component_ = editComponent;

            if (!__work_part_.__HasDynamicBlock())
                return false;

            CreateEditData(editComponent);
            _isNewSelection = false;
            return true;
        }


        private void UpdateDynamicHandles()
        {
            UserDefinedClass myUdOclass = GetDynamicHandle();

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

        public static UserDefinedClass GetDynamicHandle()
        {
            try
            {
                return session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");
            }
            catch (NXException ex) when (ex.ErrorCode == 1535022)
            {
                DynamicHandle.initializeUDO(false);
                return session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");
            }
        }

        private void ShowDynamicHandles()
        {
            try
            {
                UserDefinedClass myUdOclass = GetDynamicHandle();


                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

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


        private void MotionCallback(double[] position, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            try
            {
                if (_isDynamic)
                    MotionCallbackDyanmic(position);
                else
                {
                    Point pointPrototype = _udoPointHandle.IsOccurrence
                        ? (Point)_udoPointHandle.Prototype
                        : _udoPointHandle;

                    List<NXObject> moveAll = new List<NXObject>();

                    foreach (Point namedPts in __work_part_.Points)
                        if (namedPts.Name != "")
                            moveAll.Add(namedPts);

                    moveAll.AddRange(_edgeRepLines);
                    // get the distance from the selected point to the cursor location
                    __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                    __display_part_.WCS.Visibility = true;

                    //return;

                    double[] mappedPoint = _udoPointHandle.Coordinates.__MapAcsToWcs().__ToArray();
                    double[] mappedCursor = position.__ToPoint3d().__MapAcsToWcs().__ToArray();
                    string dir_xyz = $"{pointPrototype.Name.ToCharArray()[3]}";
                    int index;

                    switch (dir_xyz)
                    {
                        case "X":
                            index = 0;
                            break;
                        case "Y":
                            index = 1;
                            break;
                        case "Z":
                            index = 2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    __display_part_.WCS.Visibility = true;

                    if (mappedPoint[index] != mappedCursor[index])
                    {
                        double distance = Math.Sqrt(Math.Pow(mappedPoint[index] - mappedCursor[index], 2));

                        if (distance >= _gridSpace)
                        {
                            if (mappedCursor[index] < mappedPoint[index])
                                distance *= -1;

                            _distanceMoved += distance;

                            MoveObjects(moveAll.ToArray(), distance, dir_xyz);

                            if (dir_xyz == "Z")
                                SetModelingView();
                        }
                    }
                }

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

            MotionCallbackDynamic1(pointPrototype, out _, out var movePtsHalf, out var movePtsFull,
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
                    throw new ArgumentOutOfRangeException();
            }

            Point3d mappedAddX = MapWcsToAbsolute(add);

            try
            {

                if (isStart)

                    line.SetStartPoint(mappedAddX);
                else
                    line.SetEndPoint(mappedAddX);
            }
            catch (NXException ex) when (ex.ErrorCode == 1710006)
            {
                print_("///////////////////");
                print_(line.StartPoint);
                print_(line.EndPoint);
                print_(mappedAddX);

                //System.Diagnostics.Debugger.Launch();
            }
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


        private void SetDispUnits()
        {
            buttonApply.Enabled = true;
            Part.Units dispUnits = (Part.Units)__display_part_.PartUnits;
            SetDispUnits(dispUnits);
        }


        private void EditBlock()
        {
            if (Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;

            buttonApply.Enabled = false;

            LoadGridSizes();

            if (string.IsNullOrEmpty(comboBoxGrid.Text))
                if (!(Session.GetSession().Parts.Work is null))
                    comboBoxGrid.SelectedItem = __work_part_.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");
            _registered = Startup();
        }
    }
}
// 6827