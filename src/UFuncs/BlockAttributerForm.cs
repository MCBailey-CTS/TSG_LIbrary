using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_block_attributer)]
    public partial class BlockAttributerForm : _UFuncForm
    {
        private const string V = "\"" + "yes" + "\"";
        private const string V1 = "\"" + "no" + "\"";
        private const string V2 = "\"" + "none" + "\"";
        private const string V3 = "\"" + "X" + "\"";
        private const string V4 = "\"" + "Y" + "\"";
        private const string V5 = "\"" + "Z" + "\"";
        private static readonly UI TheUi = UI.GetUI();
        private static Part _workPart = session_.Parts.Work;
        private static Part __display_part_ = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = __display_part_;
        private static List<CtsAttributes> _customDescriptions = new List<CtsAttributes>();
        private static List<CtsAttributes> _purchasedMaterials = new List<CtsAttributes>();
        private static List<CtsAttributes> _compNames = new List<CtsAttributes>();
        private static List<CtsAttributes> _compMaterials = new List<CtsAttributes>();
        private static List<CtsAttributes> _burnCompMaterials = new List<CtsAttributes>();
        private static List<CtsAttributes> _compTolerances = new List<CtsAttributes>();
        private static List<Component> _allSelectedComponents = new List<Component>();
        private static List<Component> _selectedComponents = new List<Component>();
        private static Component _selComp;
        private static Body _sizeBody;
        private static bool _isMeasureBody;
        private static bool _isCustom;
        private static bool _isSelectMultiple;
        private static bool _allowBoundingBox;

        public BlockAttributerForm() => InitializeComponent();

        //===========================================
        //  Form Events
        //===========================================

        [Obsolete("need to change settings path from NX110-Concept, also cmove to using indivdual files")]
        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"{AssemblyFileVersion} - Block Attributer";
            // Set window location
            Location = Settings.Default.block_attributer_form_window_location;
            const string settingsPath = "U:\\NX110\\Concept";
            string[] settingsFile = Directory.GetFiles(settingsPath, "*.UCF");

            if (settingsFile.Length != 1)
            {
                UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "*.UCF file does not exist");
                return;
            }

            string getDescription = settingsFile[0].PerformStreamReaderString(":DESCRIPTION_ATTRIBUTE_NAME:", ":END_DESCRIPTION_ATTRIBUTE_NAME:");
            string getName = settingsFile[0].PerformStreamReaderString(":DETAIL_TYPE_ATTRIBUTE_NAME:", ":END_DETAIL_TYPE_ATTRIBUTE_NAME:");
            string getMaterial = settingsFile[0].PerformStreamReaderString(":MATERIAL_ATTRIBUTE_NAME:", ":END_MATERIAL_ATTRIBUTE_NAME:");
            _customDescriptions = settingsFile[0].PerformStreamReaderList(":CUSTOM_DESCRIPTIONS:", ":END_CUSTOM_DESCRIPTIONS:");

            foreach (CtsAttributes cDescription in _customDescriptions)
                cDescription.AttrName = getDescription != string.Empty ? getDescription : "DESCRIPTION";

            _compNames = settingsFile[0].PerformStreamReaderList(":COMPONENT_NAMES:", ":END_COMPONENT_NAMES:");

            foreach (CtsAttributes cName in _compNames)
                cName.AttrName = getName != string.Empty ? getName : "DETAIL NAME";

            _compMaterials = settingsFile[0].PerformStreamReaderList(":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");

            foreach (CtsAttributes cMaterial in _compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _burnCompMaterials = settingsFile[0].PerformStreamReaderList(":COMPONENT_BURN_MATERIALS:", ":END_COMPONENT_BURN_MATERIALS:");

            foreach (CtsAttributes cMaterial in _burnCompMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _purchasedMaterials = settingsFile[0].PerformStreamReaderList(":PURCHASED_MATERIALS:", ":END_PURCHASED_MATERIALS:");

            foreach (CtsAttributes purMaterial in _purchasedMaterials)
                purMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _compTolerances = settingsFile[0].PerformStreamReaderList(":COMPONENT_TOLERANCES:", ":END_COMPONENT_TOLERANCES:");

            foreach (CtsAttributes cTolerance in _compTolerances)
                cTolerance.AttrName = "TOLERANCE";

            LoadDefaultFormData();
            groupBoxDescription.Enabled = false;
            groupBoxMaterial.Enabled = false;
            groupBoxAddStock.Enabled = false;
            groupBoxBurnSettings.Enabled = false;
            groupBoxAttributes.Enabled = false;
        }

        public void WorkPartChanged1(BasePart p)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.block_attributer_form_window_location = Location;
            Settings.Default.Save();
        }

        private void comboBoxDescription_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDescription.Text == string.Empty)
                return;

            textBoxDescription.Clear();
            textBoxDescription.Text = comboBoxDescription.Text;
            comboBoxDescription.SelectedIndex = -1;
            comboBoxDescription.Text = string.Empty;
            textBoxDescription.Focus();
        }

        private void comboBoxPurMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPurMaterials.SelectedIndex == -1)
                return;

            textBoxMaterial.Text = string.Empty;
            comboBoxCustomMaterials.SelectedIndex = -1;
        }

        private void comboBoxCustomMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCustomMaterials.SelectedIndex == -1)
                return;

            textBoxMaterial.Text = string.Empty;
            comboBoxPurMaterials.SelectedIndex = -1;
            CtsAttributes material = (CtsAttributes)comboBoxCustomMaterials.SelectedItem;

            if (material.AttrValue == "STEELCRAFT")
                textBoxDescription.Text = "NITROGEN PLATE SYSTEM";
        }

        private void comboBoxAddx_SelectedIndexChanged(object sender, EventArgs e) => UpdateBoundingBox();

        private void comboBoxAddy_SelectedIndexChanged(object sender, EventArgs e) => UpdateBoundingBox();

        private void comboBoxAddz_SelectedIndexChanged(object sender, EventArgs e) => UpdateBoundingBox();

        private void checkBoxBurnDirX_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirX.Checked)
                return;

            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void checkBoxBurnDirY_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirY.Checked)
                return;

            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void checkBoxBurnDirZ_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxBurnDirZ.Checked)
                return;

            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirY.Checked = false;
        }

        private void checkBoxBurnout_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxBurnout.Checked)
            {
                checkBoxBurnDirZ.Checked = true;
                comboBoxMaterial.Items.Clear();
                comboBoxMaterial.Items.AddRange(_burnCompMaterials.ToArray());
                return;
            }

            if (!checkBoxBurnout.Checked)
            {
                checkBoxBurnDirZ.Checked = false;
                comboBoxMaterial.Items.Clear();
                comboBoxMaterial.Items.Add(_compMaterials.ToArray());
                return;
            }

            if (checkBoxGrind.Checked)
                return;

            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void checkBoxGrind_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGrind.Checked)
            {
                comboBoxTolerance.Enabled = true;
                comboBoxTolerance.SelectedIndex = 0;
                checkBoxBurnDirZ.Checked = true;
                return;
            }

            if (checkBoxBurnout.Checked)
            {
                comboBoxTolerance.Enabled = false;
                comboBoxTolerance.SelectedIndex = -1;
                return;
            }

            comboBoxTolerance.Enabled = false;
            comboBoxTolerance.SelectedIndex = -1;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirZ.Checked = false;
        }

        private void textBoxMaterial_Click(object sender, EventArgs e)
        {
            comboBoxPurMaterials.SelectedIndex = -1;
            comboBoxCustomMaterials.SelectedIndex = -1;
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDescription.Text == string.Empty)
            {
                groupBoxDescription.Text = "Description";
                return;
            }

            if (_selComp is null)
                return;

            if (_selComp.__Prototype().__HasDynamicBlock())
                groupBoxDescription.Text = "Description = Auto Update Off";
        }

        private void textBoxDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Tab))
                textBoxMaterial.Focus();
        }

        private void textBoxMaterial_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Tab))
                buttonApply.Focus();

            if (e.KeyCode.Equals(Keys.Return))
                buttonApply.PerformClick();
        }

        private void buttonSelectCustom_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                buttonReset.PerformClick();
                groupBoxDescription.Enabled = true;
                groupBoxMaterial.Enabled = true;
                groupBoxAttributes.Enabled = true;
                comboBoxMaterial.Enabled = false;
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;
                _isCustom = true;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selComp = SelectOneComponent();

                NXObject obj = _selComp != null
                    ? (NXObject)_selComp
                    : _workPart;

                NXObject.AttributeInformation[] attrInfo = obj.__GetAttributes();

                if (attrInfo.Length == 0)
                    return;

                foreach (NXObject.AttributeInformation attr in obj.__GetAttributes())
                {
                    if (attr.Title == "DESCRIPTION")
                        textBoxDescription.Text = obj.__GetStringAttribute(attr.Title) != ""
                            ? obj.__GetStringAttribute(attr.Title)
                            : "NO DESCRIPTION";

                    if (attr.Title == "MATERIAL")
                        textBoxMaterial.Text = obj.__GetStringAttribute(attr.Title) != ""
                            ? obj.__GetStringAttribute(attr.Title)
                            : "NO MATERIAL";
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                textBoxDescription.Focus();
                groupBoxBlockExpressions.Enabled = false;
                buttonApply.Enabled = true;
            }
        }

        private void buttonSelectMultiple_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                buttonReset.PerformClick();
                buttonSelectCustom.Enabled = false;
                comboBoxMaterial.Enabled = false;
                groupBoxDescription.Enabled = true;
                groupBoxMaterial.Enabled = true;
                groupBoxAttributes.Enabled = true;
                groupBoxBlockExpressions.Enabled = false;
                _isCustom = true;
                _isMeasureBody = false;
                _isSelectMultiple = true;
                _allSelectedComponents = SelectMultipleComponents();

                if (_allSelectedComponents.Count > 0)
                    _selectedComponents = GetOneComponentOfMany(_allSelectedComponents);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void buttonSelectDiesetOn_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                using (session_.__UsingSuppressDisplay())
                    foreach (Component diesetComp in _selectedComponents)
                    {
                        __display_part_ = diesetComp.__Prototype();
                        Expression noteExp = null;
                        bool isExpression = false;
                        NewMethod26(ref noteExp, ref isExpression);
                        string description = string.Empty;

                        description = NewMethod73(description);

                        if (description == "")
                        {
                            NewMethod1(noteExp, isExpression);
                            continue;
                        }

                        if (!description.ToLower().Contains("dieset"))
                        {
                            description += " DIESET";
                            __work_part_.__SetAttribute("DESCRIPTION", description);
                        }

                        NewMethod(noteExp, isExpression);
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

      
        private void buttonSelectDiesetOff_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                NewMethod76();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

       

        private void buttonSelectWeldmentOn_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                NewMethod73();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

       

        private void buttonSelectWeldmentOff_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                NewMethod74();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

     

        private void buttonSelectBlockComp_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.Enabled = true;
                comboBoxMaterial.SelectedIndex = -1;
                groupBoxAddStock.Enabled = true;
                groupBoxBurnSettings.Enabled = true;
                groupBoxAttributes.Enabled = true;
                buttonSelectDiesetOn.Enabled = false;
                buttonSelectDiesetOff.Enabled = false;
                buttonSelectWeldmentOn.Enabled = false;
                buttonSelectWeldmentOff.Enabled = false;
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;
                _isMeasureBody = false;
                _isCustom = false;
                _isSelectMultiple = false;
                bool isNamedExpression = false;

                Expression AddX = null,
                    AddY = null,
                    AddZ = null,
                    BurnDir = null,
                    Burnout = null,
                    Grind = null,
                    GrindTolerance = null;

                double xValue = 0,
                    yValue = 0,
                    zValue = 0;

                string burnDirValue = string.Empty,
                    burnoutValue = string.Empty,
                    grindValue = string.Empty,
                    grindTolValue = string.Empty;

                bool unitsMatch = true;
                _sizeBody = SelectOneComponentBody();

                //Revision 1.01 – 12/29/16
                //Added a dialog for the “Select Block” process
                if (_sizeBody is null)
                    return;

                const string autoUpdate = "AUTO UPDATE";

                Part owningPart = _sizeBody.IsOccurrence
                    ? (Part)_sizeBody.OwningComponent.Prototype
                    : (Part)_sizeBody.OwningPart;

                if (owningPart.__HasAttribute(autoUpdate) && owningPart.__GetStringAttribute(autoUpdate) == "OFF")
                {
                    const string message = "Auto Update is currently off.\nClicking yes will turn it on.\nClicking no will cancel the current selection process.";
                    DialogResult dislogResult = MessageBox.Show(message, "Continue?", MessageBoxButtons.YesNo);

                    if (dislogResult == DialogResult.No)
                        return;

                    owningPart.SetUserAttribute(autoUpdate, -1, "ON", NXOpen.Update.Option.Now);
                    print_($"{autoUpdate} has been set to \"On\".");
                }

                if (_sizeBody is null)
                {
                    buttonReset.PerformClick();
                    return;
                }

                _selComp = _sizeBody.OwningComponent;
                unitsMatch = NewMethod52(unitsMatch);

                if (!unitsMatch)
                {
                    UI.GetUI().NXMessageBox.Show(
                        "Caught exception : Select Block",
                        NXMessageBox.DialogType.Error,
                        "Part units do not match");

                    buttonReset.PerformClick();
                    return;
                }

                if (!_workPart.__HasDynamicBlock())
                {
                    session_.Parts.SetWork(__display_part_);
                    buttonReset.PerformClick();

                    UI.GetUI().NXMessageBox.Show(
                        "Caught exception : Select Block",
                        NXMessageBox.DialogType.Error,
                        "Not a block component\r\n" + "Select Measure");

                    return;
                }

                SetWcsToWorkPart(_selComp);

                // get named expressions
                NewMethod27(
                    ref isNamedExpression, ref AddX, ref AddY, ref AddZ,
                    ref BurnDir, ref Burnout, ref Grind,
                    ref GrindTolerance, ref xValue, ref yValue,
                    ref zValue, ref burnDirValue, ref burnoutValue,
                    ref grindValue, ref grindTolValue);

                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                grindValue = grindValue.Replace("\"", string.Empty);
                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                if (isNamedExpression)
                {
                    NewMethod28(AddX, AddY, AddZ);
                    NewMethod29(xValue, yValue, zValue, burnDirValue, burnoutValue, grindValue, grindTolValue);
                    return;
                }

                CreateCompExpressions();

                // get named expressions
                NewMethod4(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);

                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                grindValue = grindValue.Replace("\"", string.Empty);
                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                if (!isNamedExpression)
                {
                    session_.Parts.SetWork(__display_part_);
                    buttonReset.PerformClick();

                    UI.GetUI().NXMessageBox.Show(
                        "Caught exception : Select Block",
                        NXMessageBox.DialogType.Error,
                        "Expressions not created or do not exist");

                    return;
                }

                NewMethod5(AddX);
                NewMethod6(AddY);
                NewMethod7(AddZ);
                NewMethod30(burnDirValue, burnoutValue, grindValue, grindTolValue);
                NewMethod8(xValue, yValue, zValue);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                groupBoxCustom.Enabled = false;
                buttonApply.Enabled = true;
            }
        }

        private bool NewMethod52(bool unitsMatch)
        {
            if (_selComp is null)
                return unitsMatch;

            _selComp.Unhighlight();

            if (__display_part_.PartUnits != _selComp.__Prototype().PartUnits)
                return false;

            __work_component_ = _selComp;
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;
            return unitsMatch;
        }

        private void buttonMeasure_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                groupBoxAddStock.Enabled = true;
                groupBoxBurnSettings.Enabled = true;
                groupBoxAttributes.Enabled = true;
                buttonSelectDiesetOn.Enabled = false;
                buttonSelectDiesetOff.Enabled = false;
                buttonSelectWeldmentOn.Enabled = false;
                buttonSelectWeldmentOff.Enabled = false;

                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;

                _isMeasureBody = true;
                _isCustom = false;
                _isSelectMultiple = false;

                bool isNamedExpression = false;

                Expression AddX = null,
                    AddY = null,
                    AddZ = null,
                    BurnDir = null,
                    Burnout = null,
                    Grind = null,
                    GrindTolerance = null;

                double xValue = 0,
                    yValue = 0,
                    zValue = 0;

                string burnDirValue = string.Empty,
                    burnoutValue = string.Empty,
                    grindValue = string.Empty,
                    grindTolValue = string.Empty;

                bool unitsMatch = true;

                UI.GetUI()
                    .NXMessageBox.Show("Measure Body", NXMessageBox.DialogType.Information,
                        "1. Set expression values\r\n" + "2. Orient WCS\r\n" + "3. Select Apply");

                _sizeBody = SelectOneComponentBody();

                if (_sizeBody is null)
                {
                    buttonReset.PerformClick();
                    return;
                }

                _selComp = _sizeBody.OwningComponent;
                unitsMatch = NewMethod53(unitsMatch);

                if (!unitsMatch)
                {
                    UI.GetUI().NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                        "Part units do not match");
                    return;
                }

                if (_workPart.__HasDynamicBlock())
                {
                    session_.Parts.SetWork(__display_part_);
                    buttonReset.PerformClick();

                    UI.GetUI()
                        .NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                            "Component is a block component\r\n" + "Select Block");

                    return;
                }

                // get named expressions

                NewMethod9(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);

                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                grindValue = grindValue.Replace("\"", string.Empty);
                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                if (isNamedExpression)
                {
                    NewMethod10(AddX);
                    NewMethod11(AddY);
                    NewMethod12(AddZ);
                    if (burnoutValue.ToLower() == "yes")
                        checkBoxBurnout.Checked = true;
                    else
                        checkBoxBurnout.Checked = false;
                    if (grindValue.ToLower() == "yes")
                        checkBoxGrind.Checked = true;
                    else
                        checkBoxGrind.Checked = false;
                    if (burnDirValue.ToLower() == "x")
                        checkBoxBurnDirX.Checked = true;
                    if (burnDirValue.ToLower() == "y")
                        checkBoxBurnDirY.Checked = true;
                    if (burnDirValue.ToLower() == "z")
                        checkBoxBurnDirZ.Checked = true;

                    foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                        if (grindTolValue == tolSetting.AttrValue)
                            comboBoxTolerance.SelectedItem = tolSetting;

                    NewMethod14(xValue, yValue, zValue);
                    return;
                }

                CreateCompExpressions();
                NewMethod15(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);

                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                grindValue = grindValue.Replace("\"", string.Empty);
                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                if (!isNamedExpression)
                {
                    session_.Parts.SetWork(__display_part_);
                    buttonReset.PerformClick();

                    UI.GetUI()
                        .NXMessageBox.Show("Caught exception : Select Block",
                            NXMessageBox.DialogType.Error,
                            "Expressions not created or do not exist");

                    return;
                }

                NewMethod16(AddX);
                NewMethod17(AddY);
                NewMethod18(AddZ);
                NewMethod19(burnDirValue, burnoutValue, grindValue, grindTolValue);
                NewMethod20(xValue, yValue, zValue);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                groupBoxCustom.Enabled = false;
                buttonApply.Enabled = true;
            }
        }

        private bool NewMethod53(bool unitsMatch)
        {
            if (_selComp is null)
                return unitsMatch;

            _selComp.Unhighlight();

            if (__display_part_.PartUnits != _selComp.__Prototype().PartUnits)
                return false;

            __work_component_ = _selComp;
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;
            return unitsMatch;
        }

        private void buttonSetAutoUpdateOn_Click(object sender, EventArgs e)
        {
            try
            {
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;

                if (session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent") is null)
                    return;

                List<Component> selectedComps = SelectMultipleComponents();

                if (selectedComps.Count <= 0)
                {
                    UserDefinedClass udoAutoSizeClass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");
                    UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(udoAutoSizeClass);

                    if (currentUdo.Length == 1)
                    {
                        NewMethod58(currentUdo);
                    }

                    UpdateBlockDescription();
                    return;
                }

                using (session_.__UsingSuppressDisplay())
                using (session_.__UsingDisplayPartReset())
                    foreach (Component sComp in selectedComps)
                    {
                        sComp.Unhighlight();
                        __display_part_ = sComp.__Prototype();
                        __work_part_ = sComp.__Prototype();
                        _workPart = session_.Parts.Work;
                        __display_part_ = session_.Parts.Display;
                        UserDefinedClass udoClass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");
                        UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(udoClass);

                        if (currentUdo.Length == 1)
                        {
                            NewMethod59(currentUdo);
                        }

                        UpdateBlockDescription();
                    }

                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

    

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;
            _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;

            session_.Parts.SetWork(__display_part_);

            if (_selectedComponents.Count > 0)
                foreach (Component comp in _selectedComponents)
                    comp.Unhighlight();

            if (_selComp != null)
                _selComp.Unhighlight();

            __display_part_.Views.Refresh();
            _selectedComponents.Clear();
            _isMeasureBody = false;
            _isCustom = false;
            _isSelectMultiple = false;
            _allowBoundingBox = false;
            groupBoxDescription.Enabled = false;
            groupBoxMaterial.Enabled = false;
            groupBoxAddStock.Enabled = false;
            groupBoxBurnSettings.Enabled = false;
            groupBoxAttributes.Enabled = false;
            groupBoxCustom.Enabled = true;
            groupBoxBlockExpressions.Enabled = true;
            buttonSelectCustom.Enabled = true;
            textBoxDescription.Text = string.Empty;
            textBoxMaterial.Text = string.Empty;
            comboBoxMaterial.Text = string.Empty;
            comboBoxDescription.SelectedIndex = -1;
            comboBoxPurMaterials.SelectedIndex = -1;
            comboBoxCustomMaterials.SelectedIndex = -1;
            labelBlockX.Text = "X = ";
            labelBlockY.Text = "Y = ";
            labelBlockZ.Text = "Z = ";
            comboBoxAddx.SelectedIndex = -1;
            comboBoxAddy.SelectedIndex = -1;
            comboBoxAddz.SelectedIndex = -1;
            checkBoxBurnout.Checked = false;
            checkBoxGrind.Checked = false;
            checkBoxBurnDirX.Checked = false;
            checkBoxBurnDirY.Checked = false;
            checkBoxBurnDirZ.Checked = false;
            buttonSelectDiesetOn.Enabled = true;
            buttonSelectDiesetOff.Enabled = true;
            buttonSelectWeldmentOn.Enabled = true;
            buttonSelectWeldmentOff.Enabled = true;
            comboBoxTolerance.SelectedIndex = -1;
            comboBoxMaterial.SelectedIndex = -1;
            comboBoxName.SelectedIndex = -1;
            comboBoxWireDev.SelectedIndex = -1;
            comboBoxWireTaper.SelectedIndex = -1;
            comboBoxDieset.SelectedIndex = -1;
            comboBoxWeldment.SelectedIndex = -1;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "Block Attributer");

            try
            {
                if (_isSelectMultiple)
                {
                    if (_selectedComponents.Count > 0)
                        foreach (Component sComp in _selectedComponents)
                        {
                            _selComp = sComp;
                            UpdateCompAttributes();
                            sComp.Unhighlight();
                        }

                    buttonReset.PerformClick();
                    return;
                }

                UpdateCompExpressions();
                _selComp?.Unhighlight();

                if (_isMeasureBody)
                {
                    MeasureComponentBody();
                    buttonReset.PerformClick();
                    return;
                }

                UpdateBlockDescription();
                UpdateCompAttributes();
                buttonReset.PerformClick();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                comboBoxName.Text = "";
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        //===========================================
        //  Methods
        //===========================================

        private void LoadDefaultFormData()
        {
            comboBoxDescription.Items.Clear();
            comboBoxPurMaterials.Items.Clear();
            comboBoxAddx.Items.Clear();
            comboBoxAddy.Items.Clear();
            comboBoxAddz.Items.Clear();
            comboBoxTolerance.Items.Clear();
            comboBoxMaterial.Items.Clear();
            comboBoxName.Items.Clear();
            comboBoxWireDev.Items.Clear();
            comboBoxWireTaper.Items.Clear();
            comboBoxDieset.Items.Clear();
            comboBoxWeldment.Items.Clear();
            comboBoxDescription.Items.AddRange(_customDescriptions.ToArray());
            comboBoxPurMaterials.Items.AddRange(_purchasedMaterials.ToArray());
            comboBoxCustomMaterials.Items.AddRange(_compMaterials.ToArray());

            for (double i = 0; i < 1.125; i += .125)
            {
                CtsAttributes addStock = new CtsAttributes("", string.Format("{0:f3}", i));

                if (addStock.AttrValue.StartsWith("0"))
                    addStock.AttrValue = addStock.AttrValue.Remove(0, 1);

                comboBoxAddx.Items.Add(addStock);
                comboBoxAddy.Items.Add(addStock);
                comboBoxAddz.Items.Add(addStock);
            }

            // Revision • 1.02 – 2017/11/10
            ///////////////////////////////
            comboBoxAddx.SelectedIndex = 0;
            comboBoxAddy.SelectedIndex = 0;
            comboBoxAddz.SelectedIndex = 0;
            ///////////////////////////////

            comboBoxTolerance.Items.AddRange(_compTolerances.ToArray());
            comboBoxMaterial.Items.AddRange(_compMaterials.ToArray());
            comboBoxName.Items.Add(_compNames.ToArray());
            CtsAttributes wireDev1 = new CtsAttributes("WFTD", "YES");
            comboBoxWireDev.Items.Add(wireDev1);
            CtsAttributes wireDev2 = new CtsAttributes("WFTD", "NO");
            comboBoxWireDev.Items.Add(wireDev2);
            CtsAttributes wireTaper1 = new CtsAttributes("WTN", "YES");
            comboBoxWireTaper.Items.Add(wireTaper1);
            CtsAttributes wireTaper2 = new CtsAttributes("WTN", "NO");
            comboBoxWireTaper.Items.Add(wireTaper2);
            CtsAttributes dieset1 = new CtsAttributes("DIESET NOTE", "YES");
            comboBoxDieset.Items.Add(dieset1);
            CtsAttributes dieset2 = new CtsAttributes("DIESET NOTE", "NO");
            comboBoxDieset.Items.Add(dieset2);
            CtsAttributes weldment1 = new CtsAttributes("WELDMENT NOTE", "YES");
            comboBoxWeldment.Items.Add(weldment1);
            CtsAttributes weldment2 = new CtsAttributes("WELDMENT NOTE", "NO");
            comboBoxWeldment.Items.Add(weldment2);
        }

        private void UpdateCompExpressions()
        {
            using (session_.__UsingDoUpdate("Expression"))
            {

                // get named expressions
                bool isNamedExpression = false;

                Expression AddX = null,
                    AddY = null,
                    AddZ = null,
                    BurnDir = null,
                    Burnout = null,
                    Grind = null,
                    GrindTolerance = null;

                double xValue = 0,
                    yValue = 0,
                    zValue = 0;

                string burnDirValue = string.Empty,
                    burnoutValue = string.Empty,
                    grindValue = string.Empty,
                    grindTolValue = string.Empty;

                NewMethod21(
                    ref isNamedExpression, ref AddX, ref AddY,
                    ref AddZ, ref BurnDir, ref Burnout,
                    ref Grind, ref GrindTolerance, ref xValue,
                    ref yValue, ref zValue, ref burnDirValue,
                    ref burnoutValue, ref grindValue, ref grindTolValue);

                if (isNamedExpression)
                {
                    if (comboBoxAddx.SelectedIndex >= 0)
                        AddX.RightHandSide = comboBoxAddx.Text;

                    if (comboBoxAddy.SelectedIndex >= 0)
                        AddY.RightHandSide = comboBoxAddy.Text;

                    if (comboBoxAddz.SelectedIndex >= 0)
                        AddZ.RightHandSide = comboBoxAddz.Text;

                    if (checkBoxBurnout.Checked)
                        Burnout.RightHandSide = V;
                    else
                        Burnout.RightHandSide = V1;

                    if (checkBoxGrind.Checked)
                    {
                        Grind.RightHandSide = V;

                        if (comboBoxTolerance.SelectedIndex != -1)
                            GrindTolerance.RightHandSide = "\"" + comboBoxTolerance.Text + "\"";
                        else
                            GrindTolerance.RightHandSide = V2;
                    }
                    else
                    {
                        Grind.RightHandSide = V1;
                        GrindTolerance.RightHandSide = V2;
                    }

                    if (checkBoxBurnDirX.Checked)
                        BurnDir.RightHandSide = V3;
                    else if (checkBoxBurnDirY.Checked)
                        BurnDir.RightHandSide = V4;
                    else if (checkBoxBurnDirZ.Checked)
                        BurnDir.RightHandSide = V5;
                    else
                        BurnDir.RightHandSide = V2;
                }
            }
        }

        private void CreateCompExpressions()
        {
            using (session_.__UsingDoUpdate("Expression"))
            {
                if (_workPart.PartUnits == BasePart.Units.Inches)
                    NewMethod54();
                else
                    NewMethod55();

                if (checkBoxBurnout.Checked)
                    _ = _workPart.Expressions.CreateExpression("String", $"Burnout=\"yes\"");
                else
                    _ = _workPart.Expressions.CreateExpression("String", "Burnout=\"no\"");

                if (checkBoxGrind.Checked)
                {
                    _ = _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                    if (comboBoxTolerance.SelectedIndex != -1)
                        _ = _workPart.Expressions.CreateExpression("String", $"GrindTolerance=\"{comboBoxTolerance.Text}\"");
                    else
                        _ = _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                }
                else
                {
                    _ = _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");
                    _ = _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
                }

                if (checkBoxBurnDirX.Checked)
                    _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
                else if (checkBoxBurnDirY.Checked)
                    _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
                else if (checkBoxBurnDirZ.Checked)
                    _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
                else
                    _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"none\"");
            }
        }

      

        private string NewMethod60()
        {
            return $"AddZ={comboBoxAddz.Text}";
        }

      

        private void MeasureComponentBody()
        {
            try
            {
                if (_selComp != null)
                    __work_part_ = _selComp.__Prototype();

                CartesianCoordinateSystem tempCsys = __display_part_.WCS.Save();
                bool isMetric = false;

                if (_workPart.PartUnits == BasePart.Units.Millimeters)
                    isMetric = true;

                if (tempCsys != null)
                {
                    // get named expressions
                    bool isNamedExpression = false;

                    Expression AddX = null,
                        AddY = null,
                        AddZ = null,
                        BurnDir = null,
                        Burnout = null,
                        Grind = null,
                        GrindTolerance = null,
                        Dieset = null;

                    //Weldment = null;
                    double xValue = 0,
                        yValue = 0,
                        zValue = 0;

                    string burnDirValue = string.Empty,
                        burnoutValue = string.Empty,
                        grindValue = string.Empty,
                        grindTolValue = string.Empty,
                        diesetValue = string.Empty;

                    NewMethod22(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref Dieset, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue, ref diesetValue);

                    burnDirValue = burnDirValue.Replace("\"", string.Empty);
                    burnoutValue = burnoutValue.Replace("\"", string.Empty);
                    grindValue = grindValue.Replace("\"", string.Empty);
                    grindTolValue = grindTolValue.Replace("\"", string.Empty);
                    diesetValue = diesetValue.Replace("\"", string.Empty);

                    if (isNamedExpression)
                    {
                        // get bounding box of solid body

                        double[] minCorner = new double[3];
                        double[,] directions = new double[3, 3];
                        double[] distances = new double[3];
                        double[] grindDistances = new double[3];

                        ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                            distances);
                        ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                            grindDistances);

                        // add stock values

                        distances[0] += xValue;
                        distances[1] += yValue;
                        distances[2] += zValue;

                        if (isMetric)
                            for (int i = 0; i < distances.Length; i++)
                                distances[i] /= 25.4d;

                        if (burnoutValue.ToLower() == "no")
                            NewMethod23(distances);

                        double xDist = distances[0];
                        double yDist = distances[1];
                        double zDist = distances[2];

                        double xGrindDist = grindDistances[0];
                        double yGrindDist = grindDistances[1];
                        double zGrindDist = grindDistances[2];

                        Array.Sort(distances);
                        Array.Sort(grindDistances);

                        if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                            _workPart.__SetAttribute("DESCRIPTION", NewMethod56(distances));

                        else if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                        {
                            if (burnDirValue.ToLower() == "x")
                                NewMethod38(grindTolValue, distances, grindDistances, xGrindDist);

                            if (burnDirValue.ToLower() == "y")
                                NewMethod39(grindTolValue, distances, grindDistances, yGrindDist);

                            if (burnDirValue.ToLower() == "z")
                                NewMethod40(grindTolValue, distances, grindDistances, zGrindDist);
                        }
                        else if (grindValue.ToLower() == "yes")
                            NewMethod41(burnDirValue, grindTolValue, xGrindDist, yGrindDist, zGrindDist);
                        else
                            NewMethod42(burnDirValue, yDist, zDist);

                        NewMethod75(diesetValue);
                    }
                    else
                    {
                        double[] distances = NewMethod24(tempCsys, isMetric);

                        // Create the description attribute
                        Array.Sort(distances);

                        _workPart.__SetAttribute("DESCRIPTION", NewMethod57(distances));

                        NewMethod77(diesetValue);
                    }

                    tempCsys.__Delete();
                    UpdateCompAttributes();
                    ufsession_.Disp.RegenerateDisplay();

                    if (_selComp == null)
                        __display_part_.Views.Regenerate();

                    __display_part_ = _originalDisplayPart;
                    _workPart = session_.Parts.Work;
                    __display_part_ = session_.Parts.Display;
                }
                else
                {
                    TheUi.NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                        "Coordinate System not found " + _workPart.FullPath);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private void UpdateBlockDescription()
        {
            try
            {
                //UpdateSessionParts();

                bool isMetric = false;

                BasePart basePart = _workPart;
                BasePart.Units partUnits = basePart.PartUnits;

                if (partUnits == BasePart.Units.Millimeters)
                    isMetric = true;


                if (_workPart.__HasDynamicBlock())
                {
                    Block block1 = (Block)_workPart.__DynamicBlock();
                    Body[] sizeBody = block1.GetBodies();

                    BlockFeatureBuilder blockFeatureBuilderSize;
                    blockFeatureBuilderSize = _workPart.Features.CreateBlockFeatureBuilder(block1);

                    blockFeatureBuilderSize.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                    double[] initOrigin =
                    {
                        blockFeatureBuilderSize.Origin.X, blockFeatureBuilderSize.Origin.Y,
                        blockFeatureBuilderSize.Origin.Z
                    };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    double[] initMatrix = new double[9];
                    Tag tempCsys = NXOpen.Tag.Null;
                    ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                    ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                    ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out tempCsys);

                    if (tempCsys != NXOpen.Tag.Null)
                    {
                        // get named expressions

                        bool isNamedExpression = false;

                        double xValue = 0,
                            yValue = 0,
                            zValue = 0;

                        string burnDirValue = string.Empty,
                            burnoutValue = string.Empty,
                            grindValue = string.Empty,
                            grindTolValue = string.Empty,
                            diesetValue = string.Empty;
                        NewMethod25(ref isNamedExpression, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue, ref diesetValue);

                        burnDirValue = burnDirValue.Replace("\"", string.Empty);
                        burnoutValue = burnoutValue.Replace("\"", string.Empty);
                        grindValue = grindValue.Replace("\"", string.Empty);
                        grindTolValue = grindTolValue.Replace("\"", string.Empty);
                        diesetValue = diesetValue.Replace("\"", string.Empty);

                        if (isNamedExpression)
                        {
                            // get bounding box of solid body

                            double[] minCorner = new double[3];
                            double[,] directions = new double[3, 3];
                            double[] distances = new double[3];
                            double[] grindDistances = new double[3];

                            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                distances);
                            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                grindDistances);

                            // add stock values

                            distances[0] += xValue;
                            distances[1] += yValue;
                            distances[2] += zValue;

                            double trueX = distances[0];
                            double trueY = distances[1];
                            double trueZ = distances[2];

                            NewMethod31(isMetric, burnoutValue, distances);

                            double xDist = distances[0];
                            double yDist = distances[1];
                            double zDist = distances[2];

                            double xGrindDist = grindDistances[0];
                            double yGrindDist = grindDistances[1];
                            double zGrindDist = grindDistances[2];

                            Array.Sort(distances);
                            Array.Sort(grindDistances);

                            CtsAttributes text = (CtsAttributes)comboBoxTolerance.SelectedItem;
                            if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                            {
                                _workPart.SetUserAttribute("DESCRIPTION", -1,
                                    $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}",
                                    NXOpen.Update.Option.Now);
                            }
                            else if (text != null && text.AttrValue.ToLower().Contains("cleanup"))
                            {
                                switch (burnDirValue)
                                {
                                    case "X":
                                    case "x":
                                        _workPart.SetUserAttribute("DESCRIPTION", -1,
                                            $"BURN {AskSteelSize(trueX, _workPart):f2} {text}",
                                            NXOpen.Update.Option.Later);

                                        break;
                                    case "Y":
                                    case "y":
                                        _workPart.SetUserAttribute("DESCRIPTION", -1,
                                            $"BURN {AskSteelSize(trueY, _workPart):f2} {text}",
                                            NXOpen.Update.Option.Later);
                                        break;
                                    case "Z":
                                    case "z":
                                        _workPart.SetUserAttribute("DESCRIPTION", -1,
                                            $"BURN {AskSteelSize(trueZ, _workPart):f2} {text}",
                                            NXOpen.Update.Option.Later);
                                        break;
                                }
                            }
                            else if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                            {
                                const double tolerance = .001;

                                // ReSharper disable once SwitchStatementMissingSomeCases
                                switch (burnDirValue.ToLower())
                                {
                                    case "x":
                                        NewMethod67(grindTolValue, distances, grindDistances, xGrindDist, tolerance);
                                        break;
                                    case "y":
                                        NewMethod68(grindTolValue, distances, grindDistances, yGrindDist, tolerance);
                                        break;
                                    case "z":
                                        NewMethod69(grindTolValue, distances, grindDistances, zGrindDist, tolerance);
                                        break;
                                }
                            }
                            else
                            {
                                if (grindValue.ToLower() == "yes")
                                    // ReSharper disable once SwitchStatementMissingSomeCases
                                    switch (burnDirValue.ToLower())
                                    {
                                        case "x":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"BURN {xGrindDist:f3} {grindTolValue}", NXOpen.Update.Option.Now);
                                            break;
                                        case "y":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"BURN {yGrindDist:f3} {grindTolValue}", NXOpen.Update.Option.Now);
                                            break;
                                        case "z":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"BURN {zGrindDist:f3} {grindTolValue}", NXOpen.Update.Option.Now);
                                            break;
                                    }
                                else
                                    // ReSharper disable once SwitchStatementMissingSomeCases
                                    switch (burnDirValue.ToLower())
                                    {
                                        case "x":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1, $"BURN {xDist:f2}",
                                                NXOpen.Update.Option.Now);
                                            break;
                                        case "y":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1, $"BURN {yDist:f2}",
                                                NXOpen.Update.Option.Now);
                                            break;
                                        case "z":
                                            _workPart.SetUserAttribute("DESCRIPTION", -1, $"BURN {zDist:f2}",
                                                NXOpen.Update.Option.Now);
                                            break;
                                    }
                            }

                            if (diesetValue == "yes")
                            {
                                string description =
                                    _workPart.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);
                                //description += " DIESET";
                                _workPart.SetUserAttribute("DESCRIPTION", -1, $"{description} DIESET",
                                    NXOpen.Update.Option.Now);
                            }
                        }
                        else
                        {
                            // get bounding box of solid body

                            double[] distances = NewMethod32(isMetric, sizeBody, tempCsys);

                            // Create the description attribute

                            Array.Sort(distances);


                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}", NXOpen.Update.Option.Now);
                            //_workPart.__SetAttribute($"DESCRIPTION", $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}");

                            if (diesetValue == "yes")
                            {
                                string description = _workPart.GetUserAttributeAsString("DESCRIPTION",
                                    NXObject.AttributeType.String, -1);
                                description += " DIESET";

                                //_workPart.SetUserAttribute("DESCRIPTION",-1, description + " DIESET", NXOpen.Update.Option.Now);

                                _workPart.SetUserAttribute("DESCRIPTION", -1, description, NXOpen.Update.Option.Now);
                            }
                        }
                    }
                    else
                    {
                        TheUi.NXMessageBox.Show("Update Block Description", NXMessageBox.DialogType.Error,
                            "Description update failed " + _workPart.FullPath);
                    }
                }

                // If the work part does not have a {"DESCRIPTION"} attribute then we want to return;.
                if (!_workPart.HasUserAttribute("DESCRIPTION", NXObject.AttributeType.String, -1))
                    return;

                // The string value of the {"DESCRIPTION"} attribute.
                string descriptionAtt =
                    _workPart.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);

                Expression[] expressions = _workPart.Expressions.ToArray();

                // Checks to see if the {_workPart} contains an expression with value {"yes"} and name of {lwrParallel} or {uprParallel}.
                if (expressions.Any(exp =>
                        (exp.Name.ToLower() == "lwrparallel" || exp.Name.ToLower() == "uprparallel") &&
                        exp.StringValue.ToLower() == "yes"))
                    // Appends {"Parallel"} to the end of the {"DESCRIPTION"} attribute string value and then sets the it to be the value of the {"DESCRIPTION"} attribute.
                    _workPart.SetUserAttribute("DESCRIPTION", -1, descriptionAtt + " PARALLEL",
                        NXOpen.Update.Option.Now);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

      

        private void UpdateCompAttributes()
        {
            if (!_isCustom)
            {
                if (comboBoxMaterial.Text != "")
                    _workPart.__SetAttribute("MATERIAL", comboBoxMaterial.Text);
                if (comboBoxName.Text != "")
                    _workPart.__SetAttribute("DETAIL NAME", comboBoxName.Text);
                if (comboBoxWireTaper.Text != "")
                    _workPart.__SetAttribute("WTN", comboBoxWireTaper.Text);
                if (comboBoxWireDev.Text != "")
                    _workPart.__SetAttribute("WFTD", comboBoxWireDev.Text);
                if (comboBoxDieset.Text != "")
                    if (comboBoxDieset.Text == "YES")
                    {
                        NewMethod47();
                    }
                    else
                    {
                        NewMethod48();
                    }

                //Add Weldment stuff
                if (comboBoxWeldment.Text != "")
                    if (comboBoxWeldment.Text == "YES")
                    {
                        NewMethod49();
                    }
                    else
                    {
                        NewMethod50();
                    }
                //End of add weldment stuff
                return;
            }
            else
            {
                if (_selComp != null)
                {
                    Part selCompProto = (Part)_selComp.Prototype;

                    if (textBoxDescription.Text != "")
                    {
                        selCompProto.__SetAttribute("DESCRIPTION", textBoxDescription.Text);

                        using (selCompProto.__UsingSetWorkPartQuietly())
                        {
                            _workPart = session_.Parts.Work;
                            __display_part_ = session_.Parts.Display;
                            bool isBlockComp = false;

                            if (_selComp != null)
                            {
                                Part compProto = (Part)_selComp.Prototype;

                                foreach (Feature featDynamic in compProto.Features)
                                    if (featDynamic.FeatureType == "BLOCK")
                                        if (featDynamic.Name == "DYNAMIC BLOCK")
                                            isBlockComp = true;

                                if (isBlockComp)
                                    TheUi.NXMessageBox.Show("Custom Description", NXMessageBox.DialogType.Warning,
                                        "Block component = Auto Update Off");
                            }

                            SetAutoUpdateOff();
                        }

                        _workPart = session_.Parts.Work;
                        __display_part_ = session_.Parts.Display;
                    }

                    if (textBoxMaterial.Text != "")
                    {
                        selCompProto.__SetAttribute("MATERIAL", textBoxMaterial.Text);
                    }
                    else
                    {
                        if (comboBoxPurMaterials.Text != "")
                            selCompProto.__SetAttribute("MATERIAL", comboBoxPurMaterials.Text);
                        if (comboBoxCustomMaterials.Text != "")
                            selCompProto.__SetAttribute("MATERIAL", comboBoxCustomMaterials.Text);
                    }

                    if (comboBoxMaterial.Text != "")
                        selCompProto.__SetAttribute("MATERIAL", comboBoxMaterial.Text);

                    if (comboBoxName.Text != "")
                        selCompProto.__SetAttribute("DETAIL NAME", comboBoxName.Text);
                    if (comboBoxWireTaper.Text != "")
                        selCompProto.__SetAttribute("WTN", comboBoxWireTaper.Text);
                    if (comboBoxWireDev.Text != "")
                        selCompProto.__SetAttribute("WFTD", comboBoxWireDev.Text);
                    if (comboBoxDieset.Text != "")
                        if (comboBoxDieset.Text == "YES")
                        {
                            NewMethod111(selCompProto);
                        }
                        else
                        {
                            NewMethod110(selCompProto);
                        }

                    //Add Weldment stuff
                    if (comboBoxWeldment.Text != "")
                        if (comboBoxWeldment.Text == "YES")
                        {
                            NewMethod109(selCompProto);
                        }
                        else
                        {
                            NewMethod107(selCompProto);
                        }
                    //End of add Weldment stuff
                }
                else
                {
                    NewMethod108();
                }
            }
        }

        private static void NewMethod111(Part selCompProto)
        {
            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

            ufsession_.Assem.SetWorkPart(prevWp);
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            string description = selCompProto.__GetStringAttribute("DESCRIPTION");

            if (!description.ToLower().Contains("dieset"))
            {
                description += " DIESET";
                selCompProto.__SetAttribute("DESCRIPTION", description);
            }
        }

        private static void NewMethod110(Part selCompProto)
        {
            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

            ufsession_.Assem.SetWorkPart(prevWp);
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            string description = selCompProto.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("DIESET", "");
            selCompProto.__SetAttribute("DESCRIPTION", description);
        }

        private static void NewMethod109(Part selCompProto)
        {
            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

            ufsession_.Assem.SetWorkPart(prevWp);
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            string description = selCompProto.__GetStringAttribute("DESCRIPTION");

            if (!description.ToLower().Contains("weldment"))
            {
                description += " WELDMENT";
                selCompProto.__SetAttribute("DESCRIPTION", description);
            }
        }

        private void NewMethod108()
        {
            if (textBoxDescription.Text != "")
            {
                _workPart.__SetAttribute("DESCRIPTION", textBoxDescription.Text);
                SetAutoUpdateOff();
            }

            if (textBoxMaterial.Text != "")
            {
                _workPart.__SetAttribute("MATERIAL", textBoxMaterial.Text);
            }
            else
            {
                if (comboBoxPurMaterials.Text != "")
                    _workPart.__SetAttribute("MATERIAL", comboBoxPurMaterials.Text);
                if (comboBoxCustomMaterials.Text != "")
                    _workPart.__SetAttribute("MATERIAL", comboBoxCustomMaterials.Text);
            }

            if (comboBoxName.Text != "")
                _workPart.__SetAttribute("DETAIL NAME", comboBoxName.Text);

            if (comboBoxWireTaper.Text != "")
                _workPart.__SetAttribute("WTN", comboBoxWireTaper.Text);

            if (comboBoxWireDev.Text != "")
                _workPart.__SetAttribute("WFTD", comboBoxWireDev.Text);

            if (comboBoxDieset.Text != "")
                if (comboBoxDieset.Text == "YES")
                    NewMethod43();
                else
                    NewMethod44();

            //Add more Weldment Stuff
            if (comboBoxWeldment.Text != "")
                if (comboBoxWeldment.Text == "YES")
                    NewMethod45();
                else
                    NewMethod46();
            //End of more Weldment stuff
        }

        private static void NewMethod107(Part selCompProto)
        {
            //                            NXOpen.Tag prevWp = NXOpen.Tag.Null;
            //#pragma warning disable CS0618 // Type or member is obsolete
            //                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
            //#pragma warning restore CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartContextQuietly(selCompProto.Tag, out IntPtr prevWp);
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

            ufsession_.Assem.RestoreWorkPartContextQuietly(ref prevWp);
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            string description = selCompProto.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("WELDMENT", "");
            selCompProto.__SetAttribute("DESCRIPTION", description);
        }

        private void SetAutoUpdateOff()
        {
            try
            {
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;
                _originalWorkPart = _workPart; _originalDisplayPart = __display_part_; ;


                if (session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent") is null)
                    return;
                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent"));

                if (currentUdo.Length != 1)
                    return;

                UserDefinedObject myUDO = currentUdo[0];
                int[] updateFlag = myUDO.GetIntegers();
                int[] updateOff = { 0 };
                myUDO.SetIntegers(updateOff);
                _workPart.__SetAttribute("AUTO UPDATE", "OFF");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private Component SelectOneComponent()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;
            sel = TheUi.SelectionManager.SelectTaggedObject("Select component to get attributes", "Select Component",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, true, mask, out TaggedObject selectedComp, out _);

            if (sel == Selection.Response.Ok || sel == Selection.Response.ObjectSelected ||
                sel == Selection.Response.ObjectSelectedByName)
                compSelection = (Component)selectedComp;

            return compSelection;
        }

        private Body SelectOneComponentBody()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);
            Selection.Response sel;
            Body returnBody = null;
            sel = TheUi.SelectionManager.SelectTaggedObject("Select Body", "Select Body",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject selectedBody, out _);

            if (sel == Selection.Response.Ok || sel == Selection.Response.ObjectSelected ||
                sel == Selection.Response.ObjectSelectedByName)
                returnBody = (Body)selectedBody;

            return returnBody;
        }

        private List<Component> SelectMultipleComponents()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            List<Component> compsSelection = new List<Component>();

            sel = TheUi.SelectionManager.SelectTaggedObjects("Select Components", "Select Components",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, true, mask, out TaggedObject[] selectedCompArray);

            if (sel == Selection.Response.Ok)
                foreach (TaggedObject comp in selectedCompArray)
                {
                    Component component = (Component)comp;
                    compsSelection.Add(component);
                }

            return compsSelection;
        }

        private List<Component> GetOneComponentOfMany(List<Component> compList)
        {
            List<Component> oneComp = new List<Component>();

            oneComp.Add(compList[0]);

            foreach (Component comp in compList)
            {
                Component foundComponent = oneComp.Find(delegate (Component c)
                {
                    return c.DisplayName == comp.DisplayName;
                });
                if (foundComponent == null)
                    oneComp.Add(comp);
            }

            if (oneComp.Count != 0)
                return oneComp;
            return null;
        }

        private void SetWcsToWorkPart(Component compRefCsys)
        {
            try
            {
                if (compRefCsys is null)
                {
                    NewMethod70();
                    return;
                }

                NewMethod71(compRefCsys);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod71(Component compRefCsys)
        {

            BasePart compBase = (BasePart)compRefCsys.Prototype;

            session_.Parts.SetDisplay(compBase, false, false, out PartLoadStatus setDispLoadStatus);
            setDispLoadStatus.Dispose();
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            if (_workPart.__HasDynamicBlock())
            {
                Block block1 = (Block)_workPart.__DynamicBlock();

                BlockFeatureBuilder blockFeatureBuilderMatch;
                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                double mLength = blockFeatureBuilderMatch.Length.Value;
                double mWidth = blockFeatureBuilderMatch.Width.Value;
                double mHeight = blockFeatureBuilderMatch.Height.Value;

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

                __display_part_.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                    setTempCsys.Orientation.Element);

                CartesianCoordinateSystem featBlkCsys = __display_part_.WCS.Save();
                featBlkCsys.SetName("EDITCSYS");
                featBlkCsys.Layer = 254;

                NXObject[] addToBody = { featBlkCsys };

                foreach (ReferenceSet bRefSet in __display_part_.GetAllReferenceSets())
                    if (bRefSet.Name == "BODY")
                        bRefSet.AddObjectsToReferenceSet(addToBody);

                session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                    out PartLoadStatus setDispLoadStatus1);
                setDispLoadStatus1.Dispose();

                session_.Parts.SetWorkComponent(compRefCsys, PartCollection.RefsetOption.Current,
                    PartCollection.WorkComponentOption.Given,
                    out PartLoadStatus partLoadStatusWorkComp);
                partLoadStatusWorkComp.Dispose();
                _workPart = session_.Parts.Work;
                __display_part_ = session_.Parts.Display;

                foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                    if (wpCsys.Layer == 254)
                        if (wpCsys.Name == "EDITCSYS")
                        {
                            NXObject csysOccurrence;
                            csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                            CartesianCoordinateSystem editCsys =
                                (CartesianCoordinateSystem)csysOccurrence;

                            if (editCsys != null)
                                __display_part_.WCS.SetOriginAndMatrix(editCsys.Origin,
                                    editCsys.Orientation.Element);

                            Session.UndoMarkId markDeleteObjs;
                            markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

                            session_.UpdateManager.AddToDeleteList(wpCsys);

                            int errsDelObjs;
                            errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);
                        }
            }


        }

        private static void NewMethod70()
        {
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

                        __display_part_.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);
                    }
        }

        private Point3d MapWcsToAbsolute(Point3d pointToMap)
        {
            Point3d mappedPoint;
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
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
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            mappedPoint.X = output[0];
            mappedPoint.Y = output[1];
            mappedPoint.Z = output[2];
            return mappedPoint;
        }

        private Point3d MapWorkCoordsToWcs(Point3d pointToMap)
        {
            Point3d mappedPoint;
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            mappedPoint.X = output[0];
            mappedPoint.Y = output[1];
            mappedPoint.Z = output[2];
            return mappedPoint;
        }

        private void ShowTemporarySizeText(double length, Point3d start, Point3d end)
        {
            Tag view = __display_part_.Views.WorkView.Tag;
            UFDisp.ViewType viewType = UFDisp.ViewType.UseWorkView;
            string dim = string.Empty;

            if (__display_part_.PartUnits == BasePart.Units.Inches)
            {
                double roundDim = System.Math.Round(length, 3);
                dim = string.Format("{0:0.000}", roundDim);
            }
            else
            {
                double roundDim = System.Math.Round(length, 3);
                dim = string.Format("{0:0.000}", roundDim / 25.4);
            }

            double[] midPoint = new double[3];
            UFObj.DispProps dispProps = new UFObj.DispProps();
            dispProps.color = 31;
            double charSize;
            int font = 1;

            if (__display_part_.PartUnits == BasePart.Units.Inches)
                charSize = .125;
            else
                charSize = 3.175;

            midPoint[0] = (start.X + end.X) / 2;
            midPoint[1] = (start.Y + end.Y) / 2;
            midPoint[2] = (start.Z + end.Z) / 2;

            ufsession_.Disp.DisplayTemporaryText(view, viewType, dim, midPoint, UFDisp.TextRef.Middlecenter,
                ref dispProps, charSize, font);
        }

        private void CreateTempBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            Tag prevWork = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartQuietly(__display_part_.Tag, out prevWork);
#pragma warning restore CS0618 // Type or member is obsolete

            Point3d mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);

            UFObj.DispProps dispProps = new UFObj.DispProps
            {
                color = 7
            };

            UFCurve.Line lineData1 = new UFCurve.Line();

            Point3d endPointX1 = mappedStartPoint1.__AddX(lineLength);
            Point3d mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            lineData1 = NewMethod104(wcsOrigin, ref dispProps, mappedEndPointX1);

            ShowTemporarySizeText(lineLength, wcsOrigin, mappedEndPointX1);

            Point3d endPointY1 = mappedStartPoint1.__AddY(lineWidth);
            Point3d mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            _ = NewMethod103(wcsOrigin, ref dispProps, mappedEndPointY1);

            ShowTemporarySizeText(lineWidth, wcsOrigin, mappedEndPointY1);

            Point3d mappedEndPointZ1 = MapWcsToAbsolute(mappedStartPoint1.__AddZ(lineHeight));
            lineData1 = NewMethod102(wcsOrigin, ref dispProps, mappedEndPointZ1);

            ShowTemporarySizeText(lineHeight, wcsOrigin, mappedEndPointZ1);

            Point3d endPointX2 = MapAbsoluteToWcs(mappedEndPointY1).__AddX(lineLength);
            Point3d mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            lineData1 = NewMethod101(ref dispProps, mappedEndPointY1, mappedEndPointX2);
            lineData1 = NewMethod100(ref dispProps, mappedEndPointX1, mappedEndPointX2);

            Point3d mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);

            Point3d endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            Point3d mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            lineData1 = NewMethod99(ref dispProps, mappedEndPointZ1, mappedEndPointX1Ceiling);

            Point3d endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            Point3d mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            lineData1 = NewMethod86(mappedEndPointZ1, mappedEndPointY1Ceiling);

            dispProps = DisplayTemporaryLine(dispProps, lineData1);

            Point3d mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);

            Point3d endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            Point3d mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            lineData1 = NewMethod98(ref dispProps, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            lineData1 = NewMethod97(ref dispProps, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            lineData1 = NewMethod96(ref dispProps, mappedEndPointX1, mappedEndPointX1Ceiling);
            lineData1 = NewMethod95(ref dispProps, mappedEndPointY1, mappedEndPointY1Ceiling);
            lineData1 = NewMethod94(ref dispProps, mappedEndPointX2, mappedEndPointX2Ceiling);

            if (_selComp != null)
            {
                __work_component_ = _selComp;
                return;
            }

            __work_part_ = prevWork.__To<Part>();
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;

            //==================================================================================================================
        }

        private static UFCurve.Line NewMethod104(Point3d wcsOrigin, ref UFObj.DispProps dispProps, Point3d mappedEndPointX1)
        {
            UFCurve.Line lineData1 = NewMethod93(wcsOrigin, mappedEndPointX1);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod103(Point3d wcsOrigin, ref UFObj.DispProps dispProps, Point3d mappedEndPointY1)
        {
            UFCurve.Line lineData1 = NewMethod91(wcsOrigin, mappedEndPointY1);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod102(Point3d wcsOrigin, ref UFObj.DispProps dispProps, Point3d mappedEndPointZ1)
        {
            UFCurve.Line lineData1 = NewMethod90(wcsOrigin, mappedEndPointZ1);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod101(ref UFObj.DispProps dispProps, Point3d mappedEndPointY1, Point3d mappedEndPointX2)
        {
            UFCurve.Line lineData1 = NewMethod89(mappedEndPointY1, mappedEndPointX2);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod100(ref UFObj.DispProps dispProps, Point3d mappedEndPointX1, Point3d mappedEndPointX2)
        {
            UFCurve.Line lineData1 = NewMethod88(mappedEndPointX1, mappedEndPointX2);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod99(ref UFObj.DispProps dispProps, Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod87(mappedEndPointZ1, mappedEndPointX1Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod98(ref UFObj.DispProps dispProps, Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod85(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod97(ref UFObj.DispProps dispProps, Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod84(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod96(ref UFObj.DispProps dispProps, Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod83(mappedEndPointX1, mappedEndPointX1Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod95(ref UFObj.DispProps dispProps, Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod82(mappedEndPointY1, mappedEndPointY1Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod94(ref UFObj.DispProps dispProps, Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1 = NewMethod81(mappedEndPointX2, mappedEndPointX2Ceiling);
            dispProps = DisplayTemporaryLine(dispProps, lineData1);
            return lineData1;
        }

        private static UFCurve.Line NewMethod93(Point3d wcsOrigin, Point3d mappedEndPointX1)
        {
            UFCurve.Line lineData1;
            double[] startX1 = wcsOrigin.__ToArray();
            double[] endX1 = mappedEndPointX1.__ToArray();
            lineData1.start_point = startX1;
            lineData1.end_point = endX1;
            return lineData1;
        }



        private static UFCurve.Line NewMethod91(Point3d wcsOrigin, Point3d mappedEndPointY1)
        {
            UFCurve.Line lineData1;
            double[] startY1 = wcsOrigin.__ToArray();
            double[] endY1 = mappedEndPointY1.__ToArray();
            lineData1.start_point = startY1;
            lineData1.end_point = endY1;
            return lineData1;
        }

        private static UFCurve.Line NewMethod90(Point3d wcsOrigin, Point3d mappedEndPointZ1)
        {
            UFCurve.Line lineData1;
            double[] startZ1 = wcsOrigin.__ToArray();
            double[] endZ1 = mappedEndPointZ1.__ToArray();
            lineData1.start_point = startZ1;
            lineData1.end_point = endZ1;
            return lineData1;
        }

        private static UFCurve.Line NewMethod89(Point3d mappedEndPointY1, Point3d mappedEndPointX2)
        {
            UFCurve.Line lineData1;
            double[] startX2 = mappedEndPointY1.__ToArray();
            double[] endX2 = mappedEndPointX2.__ToArray();
            lineData1.start_point = startX2;
            lineData1.end_point = endX2;
            return lineData1;
        }

        private static UFCurve.Line NewMethod88(Point3d mappedEndPointX1, Point3d mappedEndPointX2)
        {
            UFCurve.Line lineData1;
            double[] startY2 = mappedEndPointX1.__ToArray();
            double[] endY2 = mappedEndPointX2.__ToArray();
            lineData1.start_point = startY2;
            lineData1.end_point = endY2;
            return lineData1;
        }

        private static UFCurve.Line NewMethod87(Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startX3 = mappedEndPointZ1.__ToArray();
            double[] endX3 = mappedEndPointX1Ceiling.__ToArray();
            lineData1.start_point = startX3;
            lineData1.end_point = endX3;
            return lineData1;
        }

        private static UFCurve.Line NewMethod86(Point3d mappedEndPointZ1, Point3d mappedEndPointY1Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startY3 = mappedEndPointZ1.__ToArray();
            double[] endY3 = mappedEndPointY1Ceiling.__ToArray();
            lineData1.start_point = startY3;
            lineData1.end_point = endY3;
            return lineData1;
        }

        private static UFCurve.Line NewMethod85(Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startX4 = mappedEndPointY1Ceiling.__ToArray();
            double[] endX4 = mappedEndPointX2Ceiling.__ToArray();
            lineData1.start_point = startX4;
            lineData1.end_point = endX4;
            return lineData1;
        }

        private static UFCurve.Line NewMethod84(Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startY4 = mappedEndPointX1Ceiling.__ToArray();
            double[] endY4 = mappedEndPointX2Ceiling.__ToArray();
            lineData1.start_point = startY4;
            lineData1.end_point = endY4;
            return lineData1;
        }

        private static UFCurve.Line NewMethod83(Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startZ2 = mappedEndPointX1.__ToArray();
            double[] endZ2 = mappedEndPointX1Ceiling.__ToArray();
            lineData1.start_point = startZ2;
            lineData1.end_point = endZ2;
            return lineData1;
        }

        private static UFCurve.Line NewMethod82(Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startZ3 = mappedEndPointY1.__ToArray();
            double[] endZ3 = mappedEndPointY1Ceiling.__ToArray();
            lineData1.start_point = startZ3;
            lineData1.end_point = endZ3;
            return lineData1;
        }

        private static UFCurve.Line NewMethod81(Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling)
        {
            UFCurve.Line lineData1;
            double[] startZ4 = mappedEndPointX2.__ToArray();
            double[] endZ4 = mappedEndPointX2Ceiling.__ToArray();
            lineData1.start_point = startZ4;
            lineData1.end_point = endZ4;
            return lineData1;
        }




        private void UpdateBoundingBox()
        {
            if (_allowBoundingBox)
            {
                __display_part_.Views.Refresh();

                // get named expressions

                bool isNamedExpression = false;

                Expression AddX = null,
                    AddY = null,
                    AddZ = null;

                double xValue = 0,
                    yValue = 0,
                    zValue = 0;

                foreach (Expression exp in _workPart.Expressions.ToArray())
                {
                    if (exp.Name == "AddX")
                    {
                        isNamedExpression = true;
                        AddX = exp;
                        xValue = exp.Value;
                    }

                    if (exp.Name == "AddY")
                    {
                        isNamedExpression = true;
                        AddY = exp;
                        yValue = exp.Value;
                    }

                    if (exp.Name == "AddZ")
                    {
                        isNamedExpression = true;
                        AddZ = exp;
                        zValue = exp.Value;
                    }
                }

                if (isNamedExpression)
                {
                    _workPart.Expressions.Edit(AddX, comboBoxAddx.Text);
                    xValue = AddX.Value;

                    _workPart.Expressions.Edit(AddY, comboBoxAddy.Text);
                    yValue = AddY.Value;

                    _workPart.Expressions.Edit(AddZ, comboBoxAddz.Text);
                    zValue = AddZ.Value;

                    // get bounding box info
                    double[] distances = NewMethod33(xValue, yValue, zValue);

                    CreateTempBlockLines(__display_part_.WCS.Origin, distances[0], distances[1], distances[2]);
                }
            }
        }



        //private void UpdateSessionParts()
        //{
        //    _workPart = session_.Parts.Work;
        //    __display_part_ = session_.Parts.Display;
        //}



        private void DeleteNxObjects(List<NXObject> objsToDelete)
        {
            Session.UndoMarkId markDeleteObjs;
            markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

            session_.UpdateManager.AddObjectsToDeleteList(objsToDelete.ToArray());

            _ = session_.UpdateManager.DoUpdate(markDeleteObjs);
        }



        #region finding expression

        private static UFObj.DispProps DisplayTemporaryLine(UFObj.DispProps dispProps, UFCurve.Line lineData1)
        {
            ufsession_.Disp.DisplayTemporaryLine(
                            __display_part_.Views.WorkView.Tag,
                            UFDisp.ViewType.UseWorkView,
                            lineData1.start_point,
                            lineData1.end_point,
                            ref dispProps);
            return dispProps;
        }

        private static void NewMethod27(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }
        }





        private static void NewMethod4(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }
        }

        private static void NewMethod15(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }
        }




        private static void NewMethod9(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }
        }




        private static void NewMethod21(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }
        }


        private static void NewMethod22(ref bool isNamedExpression, ref Expression AddX, ref Expression AddY, ref Expression AddZ, ref Expression BurnDir, ref Expression Burnout, ref Expression Grind, ref Expression GrindTolerance, ref Expression Dieset, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue, ref string diesetValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }

                if (exp.Name == "DiesetNote")
                {
                    Dieset = exp;
                    diesetValue = exp.RightHandSide;
                }
            }
        }



        private static void NewMethod25(ref bool isNamedExpression, ref double xValue, ref double yValue, ref double zValue, ref string burnDirValue, ref string burnoutValue, ref string grindValue, ref string grindTolValue, ref string diesetValue)
        {
            foreach (Expression exp in _workPart.Expressions.ToArray())
            {
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    grindTolValue = exp.RightHandSide;
                }

                if (exp.Name == "DiesetNote") diesetValue = exp.RightHandSide;
            }
        }


        #endregion

        /////////////////////////////////////////////////////////////////////

        #region rounding
        private void NewMethod8(double xValue, double yValue, double zValue)
        {
            // get bounding box info

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (_workPart.PartUnits == BasePart.Units.Millimeters)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            CreateTempBlockLines(__display_part_.WCS.Origin, distances[0], distances[1],
                distances[2]);
            _allowBoundingBox = true;
        }


        private static double[] NewMethod3(double xValue, double yValue, double zValue)
        {
            // get bounding box info

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (_workPart.PartUnits == BasePart.Units.Millimeters)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            return distances;
        }




        private void NewMethod20(double xValue, double yValue, double zValue)
        {
            // get bounding box info

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (_workPart.PartUnits == BasePart.Units.Millimeters)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            CreateTempBlockLines(__display_part_.WCS.Origin, distances[0], distances[1],
                distances[2]);
            _allowBoundingBox = true;
        }



        private void NewMethod14(double xValue, double yValue, double zValue)
        {
            // get bounding box info

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (_workPart.PartUnits == BasePart.Units.Millimeters)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            CreateTempBlockLines(__display_part_.WCS.Origin, distances[0], distances[1],
                distances[2]);
            _allowBoundingBox = true;
        }



        private static double[] NewMethod24(CartesianCoordinateSystem tempCsys, bool isMetric)
        {
            // get bounding box of solid body

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                distances);

            if (isMetric)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            return distances;
        }

        private static void NewMethod23(double[] distances)
        {
            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }
        }

        private static double[] NewMethod32(bool isMetric, Body[] sizeBody, Tag tempCsys)
        {
            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                distances);

            if (isMetric)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            return distances;
        }

        private static void NewMethod31(bool isMetric, string burnoutValue, double[] distances)
        {
            if (isMetric)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            if (burnoutValue.ToLower() == "no")
                for (int i = 0; i < 3; i++)
                {
                    double roundValue = System.Math.Round(distances[i], 3);
                    double truncateValue = System.Math.Truncate(roundValue);
                    double fractionValue = roundValue - truncateValue;
                    if (fractionValue != 0)
                        for (double ii = .125; ii <= 1; ii += .125)
                        {
                            if (fractionValue <= ii)
                            {
                                double finalValue = truncateValue + ii;
                                distances[i] = finalValue;
                                break;
                            }
                        }
                    else
                        distances[i] = roundValue;
                }
        }

        private double AskSteelSize(double distance, BasePart part)
        {
            //            if (part.Leaf.Contains("200"))
            //                Debugger.Launch();
            double roundValue = System.Math.Round(distance, 3);
            double truncateValue = System.Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;

            // If it doesn't seem to be working you might have any issue with metric vs english,
            // or you can revert the code back to the orignal line before you changed to float-point comparison.
            if (System.Math.Abs(fractionValue) > .001)
            {
                for (double ii = .125; ii <= 1; ii += .125)
                    if (fractionValue <= ii)
                    {
                        double finalValue = truncateValue + ii;
                        return finalValue;
                    }
            }
            else
            {
                return roundValue;
            }

            throw new Exception($"Ask Steel Size, Part: {part.Leaf}. {nameof(distance)}: {distance}");
        }

        private static double[] NewMethod33(double xValue, double yValue, double zValue)
        {
            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, __display_part_.WCS.CoordinateSystem.Tag,
                minCorner, directions, distances);

            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (_workPart.PartUnits == BasePart.Units.Millimeters)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }

            return distances;
        }



        #endregion


        #region other stuff
        private static void NewMethod50()
        {
            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("WELDMENT", "");
            _workPart.__SetAttribute("DESCRIPTION", description);
        }

        private static void NewMethod49()
        {
            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");

            if (!description.ToLower().Contains("weldment"))
            {
                description += " WELDMENT";
                _workPart.__SetAttribute("DESCRIPTION", description);
            }
        }

        private static void NewMethod48()
        {
            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("DIESET", "");
            _workPart.__SetAttribute("DESCRIPTION", description);
        }

        private static void NewMethod47()
        {
            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");

            if (!description.ToLower().Contains("dieset"))
            {
                description += " DIESET";
                _workPart.__SetAttribute("DESCRIPTION", description);
            }
        }

        private static void NewMethod46()
        {
            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("WELDMENT", "");
            _workPart.__SetAttribute("DESCRIPTION", description);
        }

        private static void NewMethod45()
        {
            Expression Weldment = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "WeldmentNote")
                    Weldment = exp;

            if (Weldment != null)
                Weldment.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            if (!description.ToLower().Contains("weldment"))
            {
                description += " WELDMENT";
                _workPart.__SetAttribute("DESCRIPTION", description);
            }
        }

        private static void NewMethod44()
        {
            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"no\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            description = description.Replace("DIESET", "");
            _workPart.__SetAttribute("DESCRIPTION", description);
        }

        private static void NewMethod43()
        {
            Expression Dieset = null;

            foreach (Expression exp in _workPart.Expressions.ToArray())
                if (exp.Name == "DiesetNote")
                    Dieset = exp;

            if (Dieset != null)
                Dieset.RightHandSide = "\"yes\"";
            else
                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

            string description = _workPart.__GetStringAttribute("DESCRIPTION");
            if (!description.ToLower().Contains("dieset"))
            {
                description += " DIESET";
                _workPart.__SetAttribute("DESCRIPTION", description);
            }
        }
        #endregion






        #region CTS Attributes



        private void NewMethod12(Expression AddZ)
        {
            foreach (CtsAttributes addZ in comboBoxAddz.Items)
            {
                if (AddZ.RightHandSide == addZ.AttrValue)
                {
                    comboBoxAddz.SelectedItem = addZ;

                    break;
                }

                comboBoxAddz.SelectedIndex = 0;
            }
        }

        private void NewMethod11(Expression AddY)
        {
            foreach (CtsAttributes addY in comboBoxAddy.Items)
            {
                if (AddY.RightHandSide == addY.AttrValue)
                {
                    comboBoxAddy.SelectedItem = addY;

                    break;
                }

                comboBoxAddy.SelectedIndex = 0;
            }
        }

        private void NewMethod10(Expression AddX)
        {
            foreach (CtsAttributes addX in comboBoxAddx.Items)
            {
                if (AddX.RightHandSide == addX.AttrValue)
                {
                    comboBoxAddx.SelectedItem = addX;

                    break;
                }

                comboBoxAddx.SelectedIndex = 0;
            }
        }

        private void NewMethod18(Expression AddZ)
        {
            foreach (CtsAttributes addZ in comboBoxAddz.Items)
            {
                if (AddZ.RightHandSide == addZ.AttrValue)
                {
                    comboBoxAddz.SelectedItem = addZ;

                    break;
                }

                comboBoxAddz.SelectedIndex = 0;
            }
        }

        private void NewMethod17(Expression AddY)
        {
            foreach (CtsAttributes addY in comboBoxAddy.Items)
            {
                if (AddY.RightHandSide == addY.AttrValue)
                {
                    comboBoxAddy.SelectedItem = addY;

                    break;
                }

                comboBoxAddy.SelectedIndex = 0;
            }
        }

        private void NewMethod16(Expression AddX)
        {
            foreach (CtsAttributes addX in comboBoxAddx.Items)
            {
                if (AddX.RightHandSide == addX.AttrValue)
                {
                    comboBoxAddx.SelectedItem = addX;

                    break;
                }

                comboBoxAddx.SelectedIndex = 0;
            }
        }


        private void NewMethod28(Expression AddX, Expression AddY, Expression AddZ)
        {
            foreach (CtsAttributes addX in comboBoxAddx.Items)
                try
                {
                    if (AddX.RightHandSide == addX.AttrValue)
                    {
                        comboBoxAddx.SelectedItem = addX;

                        break;
                    }

                    comboBoxAddx.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    UI.GetUI().NXMessageBox.Show("DJ", NXMessageBox.DialogType.Error, ex.Message);
                }

            foreach (CtsAttributes addY in comboBoxAddy.Items)
            {
                if (AddY.RightHandSide == addY.AttrValue)
                {
                    comboBoxAddy.SelectedItem = addY;

                    break;
                }

                comboBoxAddy.SelectedIndex = 0;
            }

            foreach (CtsAttributes addZ in comboBoxAddz.Items)
            {
                if (AddZ.RightHandSide == addZ.AttrValue)
                {
                    comboBoxAddz.SelectedItem = addZ;

                    break;
                }

                comboBoxAddz.SelectedIndex = 0;
            }
        }



        private void NewMethod7(Expression AddZ)
        {
            foreach (CtsAttributes addZ in comboBoxAddz.Items)
            {
                if (AddZ.RightHandSide == addZ.AttrValue)
                {
                    comboBoxAddz.SelectedItem = addZ;

                    break;
                }

                comboBoxAddz.SelectedIndex = 0;
            }
        }

        private void NewMethod6(Expression AddY)
        {
            foreach (CtsAttributes addY in comboBoxAddy.Items)
            {
                if (AddY.RightHandSide == addY.AttrValue)
                {
                    comboBoxAddy.SelectedItem = addY;

                    break;
                }

                comboBoxAddy.SelectedIndex = 0;
            }
        }

        private void NewMethod5(Expression AddX)
        {
            foreach (CtsAttributes addX in comboBoxAddx.Items)
            {
                if (AddX.RightHandSide == addX.AttrValue)
                {
                    comboBoxAddx.SelectedItem = addX;

                    break;
                }

                comboBoxAddx.SelectedIndex = 0;
            }
        }



        #endregion


        private void NewMethod30(string burnDirValue, string burnoutValue, string grindValue, string grindTolValue)
        {
            if (burnoutValue.ToLower() == "yes")
                checkBoxBurnout.Checked = true;
            else
                checkBoxBurnout.Checked = false;
            if (grindValue.ToLower() == "yes")
                checkBoxGrind.Checked = true;
            else
                checkBoxGrind.Checked = false;
            if (burnDirValue.ToLower() == "x")
                checkBoxBurnDirX.Checked = true;
            if (burnDirValue.ToLower() == "y")
                checkBoxBurnDirY.Checked = true;
            if (burnDirValue.ToLower() == "z")
                checkBoxBurnDirZ.Checked = true;
            foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                if (grindTolValue == tolSetting.AttrValue)
                    comboBoxTolerance.SelectedItem = tolSetting;
        }

        private void NewMethod29(double xValue, double yValue, double zValue, string burnDirValue, string burnoutValue, string grindValue, string grindTolValue)
        {
            if (burnoutValue.ToLower() == "yes")
                checkBoxBurnout.Checked = true;
            else
                checkBoxBurnout.Checked = false;
            if (grindValue.ToLower() == "yes")
                checkBoxGrind.Checked = true;
            else
                checkBoxGrind.Checked = false;
            if (burnDirValue.ToLower() == "x")
                checkBoxBurnDirX.Checked = true;
            if (burnDirValue.ToLower() == "y")
                checkBoxBurnDirY.Checked = true;
            if (burnDirValue.ToLower() == "z")
                checkBoxBurnDirZ.Checked = true;
            foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                if (grindTolValue == tolSetting.AttrValue)
                    comboBoxTolerance.SelectedItem = tolSetting;

            double[] distances = NewMethod3(xValue, yValue, zValue);

            CreateTempBlockLines(__display_part_.WCS.Origin, distances[0], distances[1],
                distances[2]);
            _allowBoundingBox = true;
        }


        private void NewMethod19(string burnDirValue, string burnoutValue, string grindValue, string grindTolValue)
        {
            if (burnoutValue.ToLower() == "yes")
                checkBoxBurnout.Checked = true;
            else
                checkBoxBurnout.Checked = false;
            if (grindValue.ToLower() == "yes")
                checkBoxGrind.Checked = true;
            else
                checkBoxGrind.Checked = false;
            if (burnDirValue.ToLower() == "x")
                checkBoxBurnDirX.Checked = true;
            if (burnDirValue.ToLower() == "y")
                checkBoxBurnDirY.Checked = true;
            if (burnDirValue.ToLower() == "z")
                checkBoxBurnDirZ.Checked = true;
            foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                if (grindTolValue == tolSetting.AttrValue)
                    comboBoxTolerance.SelectedItem = tolSetting;
        }








    }

}
// 4010