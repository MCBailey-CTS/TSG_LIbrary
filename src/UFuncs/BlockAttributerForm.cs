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

        //private static int registered = 0;
        //private static int idWorkPartChanged1 = 0;

        public BlockAttributerForm()
        {
            InitializeComponent();
        }

        //===========================================
        //  Form Events
        //===========================================

        [Obsolete("need to change settings path from NX110-Concept, also cmove to using indivdual files")]
        private void MainForm_Load(object sender, EventArgs e)
        {
            // todo: 
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

            string getDescription = settingsFile[0].PerformStreamReaderString(":DESCRIPTION_ATTRIBUTE_NAME:",
                ":END_DESCRIPTION_ATTRIBUTE_NAME:");
            string getName = settingsFile[0].PerformStreamReaderString(":DETAIL_TYPE_ATTRIBUTE_NAME:",
                ":END_DETAIL_TYPE_ATTRIBUTE_NAME:");
            string getMaterial = settingsFile[0].PerformStreamReaderString(":MATERIAL_ATTRIBUTE_NAME:",
                ":END_MATERIAL_ATTRIBUTE_NAME:");

            _customDescriptions = settingsFile[0].PerformStreamReaderList(":CUSTOM_DESCRIPTIONS:",
                ":END_CUSTOM_DESCRIPTIONS:");
            foreach (CtsAttributes cDescription in _customDescriptions)
                cDescription.AttrName = getDescription != string.Empty ? getDescription : "DESCRIPTION";

            _compNames = settingsFile[0].PerformStreamReaderList(":COMPONENT_NAMES:", ":END_COMPONENT_NAMES:");
            foreach (CtsAttributes cName in _compNames)
                cName.AttrName = getName != string.Empty ? getName : "DETAIL NAME";

            _compMaterials = settingsFile[0].PerformStreamReaderList(":COMPONENT_MATERIALS:",
                ":END_COMPONENT_MATERIALS:");
            foreach (CtsAttributes cMaterial in _compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _burnCompMaterials = settingsFile[0].PerformStreamReaderList(":COMPONENT_BURN_MATERIALS:",
                ":END_COMPONENT_BURN_MATERIALS:");
            foreach (CtsAttributes cMaterial in _burnCompMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _purchasedMaterials = settingsFile[0].PerformStreamReaderList(":PURCHASED_MATERIALS:",
                ":END_PURCHASED_MATERIALS:");
            foreach (CtsAttributes purMaterial in _purchasedMaterials)
                purMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

            _compTolerances = settingsFile[0].PerformStreamReaderList(":COMPONENT_TOLERANCES:",
                ":END_COMPONENT_TOLERANCES:");
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
                UpdateSessionParts();
                UpdateOriginalParts();
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
                        if (obj.__GetStringAttribute(attr.Title) != "")
                            textBoxDescription.Text = obj.__GetStringAttribute(attr.Title);
                        else
                            textBoxDescription.Text = "NO DESCRIPTION";

                    if (attr.Title == "MATERIAL")
                        if (obj.__GetStringAttribute(attr.Title) != "")
                            textBoxMaterial.Text = obj.__GetStringAttribute(attr.Title);
                        else
                            textBoxMaterial.Text = "NO MATERIAL";
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

                        bool isDescription = false;
                        string description = string.Empty;
                        isDescription = __work_part_.__HasAttribute("DESCRIPTION");

                        if (isDescription)
                            description = __work_part_.__GetStringAttribute("DESCRIPTION");
                        else
                            __work_part_.__SetAttribute("DESCRIPTION", "NO DESCRIPTION");

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

        private static void NewMethod26(ref Expression noteExp, ref bool isExpression)
        {
            foreach (Expression exp in _workPart.Expressions)
                if (exp.Name == "DiesetNote")
                {
                    isExpression = true;
                    noteExp = exp;
                }
        }

        private static void NewMethod1(Expression noteExp, bool isExpression)
        {
            if (isExpression)
            {
                noteExp.RightHandSide = "\"yes\"";
            }
            else
            {
                Expression diesetExp =
                    _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");
            }
        }

        private static void NewMethod(Expression noteExp, bool isExpression)
        {
            if (isExpression)
            {
                noteExp.RightHandSide = "\"yes\"";
            }
            else
            {
                Expression diesetExp =
                    _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");
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
                UpdateSessionParts();
                UpdateOriginalParts();
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                using (session_.__UsingSuppressDisplay())
                using (session_.__UsingDisplayPartReset())
                    foreach (Component diesetComp in _selectedComponents)
                    {
                        __display_part_ = diesetComp.__Prototype();
                        Expression noteExp = null;
                        bool isExpression = false;
                        NewMethod35(ref noteExp, ref isExpression);
                        string description = _workPart.__GetStringAttribute("DESCRIPTION");
                        description = NewMethod34(noteExp, isExpression, description);
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod35(ref Expression noteExp, ref bool isExpression)
        {
            foreach (Expression exp in _workPart.Expressions)
                if (exp.Name == "DiesetNote")
                {
                    isExpression = true;
                    noteExp = exp;
                }
        }

        private static string NewMethod34(Expression noteExp, bool isExpression, string description)
        {
            if (description != "")
            {
                description = description.Replace(" DIESET", "");
                _workPart.__SetAttribute("DESCRIPTION", description);

                if (isExpression)
                {
                    noteExp.RightHandSide = "\"no\"";
                }
                else
                {
                    Expression diesetExp =
                        _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");
                }
            }
            else
            {
                if (isExpression)
                {
                    noteExp.RightHandSide = "\"no\"";
                }
                else
                {
                    Expression diesetExp =
                        _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");
                }
            }

            return description;
        }

        private void buttonSelectWeldmentOn_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;
                UpdateSessionParts();
                UpdateOriginalParts();
                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;
                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count <= 0)
                    return;

                using (session_.__UsingSuppressDisplay())
                using (session_.__UsingDisplayPartReset())
                    foreach (Component weldmentComp in _selectedComponents)
                    {
                        __display_part_ = weldmentComp.__Prototype();
                        UpdateSessionParts();

                        Expression noteExp = null;
                        bool isExpression = false;

                        NewMethod51(ref noteExp, ref isExpression);

                        bool isDescription = false;
                        string description = string.Empty;

                        foreach (NXObject.AttributeInformation attr in _workPart.__GetAttributes())
                            if (attr.Title == "DESCRIPTION")
                                isDescription = true;

                        if (isDescription)
                            description = _workPart.__GetStringAttribute("DESCRIPTION");
                        else
                            _workPart.__SetAttribute("DESCRIPTION", "NO DESCRIPTION");

                        description = NewMethod36(noteExp, isExpression, description);
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod51(ref Expression noteExp, ref bool isExpression)
        {
            foreach (Expression exp in _workPart.Expressions)
                if (exp.Name == "WeldmentNote")
                {
                    isExpression = true;
                    noteExp = exp;
                }
        }

        private static string NewMethod36(Expression noteExp, bool isExpression, string description)
        {
            if (description != "")
            {
                if (!description.ToLower().Contains("weldment"))
                {
                    description += " WELDMENT";
                    _workPart.__SetAttribute("DESCRIPTION", description);
                }

                if (isExpression)
                {
                    noteExp.RightHandSide = "\"yes\"";
                }
                else
                {
                    Expression weldmentExp =
                        _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");
                }
            }
            else
            {
                if (isExpression)
                {
                    noteExp.RightHandSide = "\"yes\"";
                }
                else
                {
                    Expression weldmentExp =
                        _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");
                }
            }

            return description;
        }

        private void buttonSelectWeldmentOff_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxDescription.Clear();
                textBoxMaterial.Clear();
                comboBoxDescription.SelectedIndex = -1;
                comboBoxMaterial.SelectedIndex = -1;

                UpdateSessionParts();
                UpdateOriginalParts();

                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;

                _selectedComponents = SelectMultipleComponents();

                if (_selectedComponents.Count > 0)
                {
                    using (session_.__UsingDisplayPartReset())
                    using (session_.__UsingSuppressDisplay())
                        foreach (Component weldmentComp in _selectedComponents)
                        {
                            __display_part_ = weldmentComp.__Prototype();
                            Expression noteExp = null;
                            bool isExpression = false;
                            NewMethod2(ref noteExp, ref isExpression);
                            string description = _workPart.__GetStringAttribute("DESCRIPTION");
                            description = NewMethod37(noteExp, isExpression, description);
                        }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static string NewMethod37(Expression noteExp, bool isExpression, string description)
        {
            if (description != "")
            {
                description = description.Replace(" WELDMENT", "");
                _workPart.__SetAttribute("DESCRIPTION", description);

                if (isExpression)
                {
                    noteExp.RightHandSide = "\"no\"";
                }
                else
                {
                    Expression weldmentExp =
                        _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");
                }
            }
            else
            {
                if (isExpression)
                {
                    noteExp.RightHandSide = "\"no\"";
                }
                else
                {
                    Expression weldmentExp =
                        _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");
                }
            }

            return description;
        }

        private static void NewMethod2(ref Expression noteExp, ref bool isExpression)
        {
            foreach (Expression exp in _workPart.Expressions)
                if (exp.Name == "WeldmentNote")
                {
                    isExpression = true;
                    noteExp = exp;
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

                UpdateSessionParts();
                UpdateOriginalParts();

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

                bool hasAutoUpdate = owningPart.HasUserAttribute(autoUpdate, NXObject.AttributeType.String, -1);

                if (hasAutoUpdate)
                {
                    string autoUpdateValue = owningPart.__GetStringAttribute(autoUpdate);
                    if (autoUpdateValue == "OFF")
                    {
                        const string message =
                            "Auto Update is currently off.\nClicking yes will turn it on.\nClicking no will cancel the current selection process.";
                        DialogResult dislogResult = MessageBox.Show(message, "Continue?", MessageBoxButtons.YesNo);

                        if (dislogResult == DialogResult.No)
                            return;

                        owningPart.SetUserAttribute(autoUpdate, -1, "ON", NXOpen.Update.Option.Now);
                        ListingWindow listingWindow = Session.GetSession().ListingWindow;
                        listingWindow.Open();
                        listingWindow.WriteLine($"{autoUpdate} has been set to \"On\".");
                    }
                }

                if (_sizeBody is null)
                {
                    buttonReset.PerformClick();
                    return;
                }

                _selComp = _sizeBody.OwningComponent;

                if (_selComp != null)
                {
                    _selComp.Unhighlight();

                    Part makeWork = (Part)_selComp.Prototype;
                    BasePart.Units wpUnits = makeWork.PartUnits;

                    if (__display_part_.PartUnits == wpUnits)
                    {
                        session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                            PartCollection.WorkComponentOption.Given,
                            out PartLoadStatus loadStatus1);
                        loadStatus1.Dispose();

                        UpdateSessionParts();
                    }
                    else
                        unitsMatch = false;
                }

                if (!unitsMatch)
                {
                    UI.GetUI().NXMessageBox.Show("Caught exception : Select Block", NXMessageBox.DialogType.Error,
                        "Part units do not match");
                    buttonReset.PerformClick();
                    return;
                }

                if (!_workPart.__HasDynamicBlock())
                {
                    session_.Parts.SetWork(__display_part_);
                    buttonReset.PerformClick();

                    UI.GetUI()
                        .NXMessageBox.Show("Caught exception : Select Block", NXMessageBox.DialogType.Error,
                            "Not a block component\r\n" + "Select Measure");
                    return;
                }

                SetWcsToWorkPart(_selComp);
                // get named expressions
                NewMethod27(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);
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

                    UI.GetUI()
                        .NXMessageBox.Show("Caught exception : Select Block",
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

                UpdateSessionParts();
                UpdateOriginalParts();

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

                if (_sizeBody != null)
                {
                    _selComp = _sizeBody.OwningComponent;

                    if (_selComp != null)
                    {
                        _selComp.Unhighlight();

                        Part makeWork = (Part)_selComp.Prototype;
                        BasePart.Units wpUnits = makeWork.PartUnits;

                        if (__display_part_.PartUnits == wpUnits)
                        {
                            session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                                PartCollection.WorkComponentOption.Given,
                                out PartLoadStatus loadStatus1);
                            loadStatus1.Dispose();

                            UpdateSessionParts();
                        }
                        else
                        {
                            unitsMatch = false;
                        }
                    }

                    if (unitsMatch)
                    {
                        bool isBlockComp = false;

                        foreach (Feature featDynamic in _workPart.Features)
                            if (featDynamic.FeatureType == "BLOCK")
                                if (featDynamic.Name == "DYNAMIC BLOCK")
                                    isBlockComp = true;
                        if (!isBlockComp)
                        {
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
                                NewMethod13(burnDirValue, burnoutValue, grindValue);
                                foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                                    if (grindTolValue == tolSetting.AttrValue)
                                        comboBoxTolerance.SelectedItem = tolSetting;
                                NewMethod14(xValue, yValue, zValue);
                            }
                            else
                            {
                                CreateCompExpressions();
                                NewMethod15(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);

                                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                                grindValue = grindValue.Replace("\"", string.Empty);
                                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                                if (isNamedExpression)
                                {
                                    NewMethod16(AddX);
                                    NewMethod17(AddY);
                                    NewMethod18(AddZ);
                                    NewMethod19(burnDirValue, burnoutValue, grindValue, grindTolValue);
                                    NewMethod20(xValue, yValue, zValue);
                                }
                                else
                                {
                                    session_.Parts.SetWork(__display_part_);
                                    buttonReset.PerformClick();

                                    UI.GetUI()
                                        .NXMessageBox.Show("Caught exception : Select Block",
                                            NXMessageBox.DialogType.Error,
                                            "Expressions not created or do not exist");
                                }
                            }

                            groupBoxCustom.Enabled = false;
                            buttonApply.Enabled = true;
                        }
                        else
                        {
                            session_.Parts.SetWork(__display_part_);
                            buttonReset.PerformClick();

                            UI.GetUI()
                                .NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                                    "Component is a block component\r\n" + "Select Block");
                        }
                    }
                    else
                    {
                        UI.GetUI().NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                            "Part units do not match");
                    }
                }
                else
                {
                    buttonReset.PerformClick();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }




        private void buttonSetAutoUpdateOn_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                UserDefinedClass myUDOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");

                if (myUDOclass is null)
                    return;

                List<Component> selectedComps = SelectMultipleComponents();

                if (selectedComps.Count <= 0)
                {
                    UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUDOclass);

                    if (currentUdo.Length == 1)
                    {
                        UserDefinedObject myUDO = currentUdo[0];
                        int[] updateFlag = myUDO.GetIntegers();
                        int[] updateOn = { 1 };
                        myUDO.SetIntegers(updateOn);
                        _workPart.__SetAttribute("AUTO UPDATE", "ON");
                    }

                    UpdateBlockDescription();
                    return;
                }

                using (session_.__UsingSuppressDisplay())
                using (session_.__UsingDisplayPartReset())
                    foreach (Component sComp in selectedComps)
                    {
                        sComp.Unhighlight();

                        Part wpComp = (Part)sComp.Prototype;

                        ufsession_.Part.SetDisplayPart(wpComp.Tag);
                        ufsession_.Assem.SetWorkPart(wpComp.Tag);
                        UpdateSessionParts();

                        UserDefinedObject[] currentUdo;
                        currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUDOclass);

                        if (currentUdo.Length == 1)
                        {
                            UserDefinedObject myUDO = currentUdo[0];

                            int[] updateFlag = myUDO.GetIntegers();

                            int[] updateOn = { 1 };
                            myUDO.SetIntegers(updateOn);
                            _workPart.__SetAttribute("AUTO UPDATE", "ON");
                        }

                        UpdateBlockDescription();
                    }

                UpdateSessionParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();

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
            Session.UndoMarkId updateExpressions;
            updateExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

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

            NewMethod21(ref isNamedExpression, ref AddX, ref AddY, ref AddZ, ref BurnDir, ref Burnout, ref Grind, ref GrindTolerance, ref xValue, ref yValue, ref zValue, ref burnDirValue, ref burnoutValue, ref grindValue, ref grindTolValue);

            if (isNamedExpression)
            {
                if (comboBoxAddx.SelectedIndex >= 0)
                    AddX.RightHandSide = comboBoxAddx.Text;

                if (comboBoxAddy.SelectedIndex >= 0)
                    AddY.RightHandSide = comboBoxAddy.Text;

                if (comboBoxAddz.SelectedIndex >= 0)
                    AddZ.RightHandSide = comboBoxAddz.Text;

                if (checkBoxBurnout.Checked)
                    Burnout.RightHandSide = "\"" + "yes" + "\"";
                else
                    Burnout.RightHandSide = "\"" + "no" + "\"";

                if (checkBoxGrind.Checked)
                {
                    Grind.RightHandSide = "\"" + "yes" + "\"";

                    if (comboBoxTolerance.SelectedIndex != -1)
                        GrindTolerance.RightHandSide = "\"" + comboBoxTolerance.Text + "\"";
                    else
                        GrindTolerance.RightHandSide = "\"" + "none" + "\"";
                }
                else
                {
                    Grind.RightHandSide = "\"" + "no" + "\"";
                    GrindTolerance.RightHandSide = "\"" + "none" + "\"";
                }

                if (checkBoxBurnDirX.Checked)
                    BurnDir.RightHandSide = "\"" + "X" + "\"";
                else if (checkBoxBurnDirY.Checked)
                    BurnDir.RightHandSide = "\"" + "Y" + "\"";
                else if (checkBoxBurnDirZ.Checked)
                    BurnDir.RightHandSide = "\"" + "Z" + "\"";
                else
                    BurnDir.RightHandSide = "\"" + "none" + "\"";

                _ = session_.UpdateManager.DoUpdate(updateExpressions);
            }
        }

        private void CreateCompExpressions()
        {
            Session.UndoMarkId makeExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

            if (_workPart.PartUnits == BasePart.Units.Inches)
            {
                Unit unit1 = _workPart.UnitCollection.FindObject("Inch");

                if (comboBoxAddx.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddX=" + comboBoxAddx.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);

                if (comboBoxAddy.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddY=" + comboBoxAddy.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);

                if (comboBoxAddz.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=" + comboBoxAddz.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
            }
            else
            {
                Unit unit1 = _workPart.UnitCollection.FindObject("MilliMeter");

                if (comboBoxAddx.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddX=" + comboBoxAddx.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);

                if (comboBoxAddy.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddY=" + comboBoxAddy.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);

                if (comboBoxAddz.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=" + comboBoxAddz.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
            }

            if (checkBoxBurnout.Checked)
                _ = _workPart.Expressions.CreateExpression("String", "Burnout=\"yes\"");
            else
                _ = _workPart.Expressions.CreateExpression("String", "Burnout=\"no\"");

            if (checkBoxGrind.Checked)
            {
                _ = _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                if (comboBoxTolerance.SelectedIndex != -1)
                    _ = _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"" + comboBoxTolerance.Text + "\"");
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

            _ = session_.UpdateManager.DoUpdate(makeExpressions);
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
                            _workPart.__SetAttribute("DESCRIPTION", $"{distances[0]:f2} X {$"{distances[1]:f2}"} X {distances[2]:f2}");
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

                        if (diesetValue == "yes")
                        {
                            string description = _workPart.__GetStringAttribute("DESCRIPTION");

                            if (!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                    }
                    else
                    {
                        double[] distances = NewMethod24(tempCsys, isMetric);

                        // Create the description attribute
                        Array.Sort(distances);

                        _workPart.__SetAttribute("DESCRIPTION",
                            string.Format("{0:f2}", distances[0]) + " X " + string.Format("{0:f2}", distances[1]) +
                            " X " +
                            string.Format("{0:f2}", distances[2]));

                        if (diesetValue == "yes")
                        {
                            string description = _workPart.__GetStringAttribute("DESCRIPTION");

                            if (!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                    }

                    List<NXObject> delObj = new List<NXObject>
                    {
                        tempCsys
                    };

                    DeleteNxObjects(delObj);
                    UpdateCompAttributes();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();

                    if (_selComp == null)
                        __display_part_.Views.Regenerate();

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                        out PartLoadStatus setDispLoadStatus1);
                    setDispLoadStatus1.Dispose();
                    UpdateSessionParts();
                }
                else
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    TheUi.NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error,
                        "Coordinate System not found " + _workPart.FullPath);
                }
            }
            catch (Exception ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                ex.__PrintException();
            }
        }

        private static void NewMethod42(string burnDirValue, double yDist, double zDist)
        {
            if (burnDirValue.ToLower() == "x")
                _workPart.__SetAttribute("DESCRIPTION", $"BURN {"{xDist:f2}"}");

            if (burnDirValue.ToLower() == "y")
                _workPart.__SetAttribute("DESCRIPTION", "BURN " + string.Format("{0:f2}", yDist));

            if (burnDirValue.ToLower() == "z")
                _workPart.__SetAttribute("DESCRIPTION", "BURN " + string.Format("{0:f2}", zDist));
        }

        private static void NewMethod41(string burnDirValue, string grindTolValue, double xGrindDist, double yGrindDist, double zGrindDist)
        {
            if (burnDirValue.ToLower() == "x")
                _workPart.__SetAttribute("DESCRIPTION", $"BURN {$"{xGrindDist:f3}"} {grindTolValue}");

            if (burnDirValue.ToLower() == "y")
                _workPart.__SetAttribute("DESCRIPTION", $"BURN {$"{yGrindDist:f3}"} {grindTolValue}");

            if (burnDirValue.ToLower() == "z")
                _workPart.__SetAttribute("DESCRIPTION", $"BURN {$"{zGrindDist:f3}"} {grindTolValue}");
        }

        private static void NewMethod40(string grindTolValue, double[] distances, double[] grindDistances, double zGrindDist)
        {
            if (zGrindDist == grindDistances[0])
                _workPart.__SetAttribute("DESCRIPTION", $"{grindDistances[0]:f3} {grindTolValue} X {$"{distances[1]:f2}"} X {$"{distances[2]:f2}"}");

            if (zGrindDist == grindDistances[1])
                _workPart.__SetAttribute("DESCRIPTION",
                    $"{distances[0]:f2}" + " X " +
                    $"{grindDistances[1]:f3}" + " " + grindTolValue +
                    " X " +
                    $"{distances[2]:f2}");

            if (zGrindDist == grindDistances[2])
                _workPart.__SetAttribute("DESCRIPTION",
                    $"{distances[0]:f2}" + " X " +
                    $"{distances[1]:f2}" + " X " +
                    $"{grindDistances[2]:f3}" + " " + grindTolValue);
        }

        private static void NewMethod39(string grindTolValue, double[] distances, double[] grindDistances, double yGrindDist)
        {
            if (yGrindDist == grindDistances[0])
                _workPart.__SetAttribute("DESCRIPTION",
                    $"{string.Format("{0:f3}", grindDistances[0])} {grindTolValue} X {string.Format("{0:f2}", distances[1])} X {string.Format("{0:f2}", distances[2])}");
            if (yGrindDist == grindDistances[1])
                _workPart.__SetAttribute("DESCRIPTION",
                    string.Format("{0:f2}", distances[0]) + " X " +
                    string.Format("{0:f3}", grindDistances[1]) + " " + grindTolValue +
                    " X " +
                    string.Format("{0:f2}", distances[2]));
            if (yGrindDist == grindDistances[2])
                _workPart.__SetAttribute("DESCRIPTION",
                    string.Format("{0:f2}", distances[0]) + " X " +
                    string.Format("{0:f2}", distances[1]) + " X " +
                    string.Format("{0:f3}", grindDistances[2]) + " " + grindTolValue);
        }

        private static void NewMethod38(string grindTolValue, double[] distances, double[] grindDistances, double xGrindDist)
        {
            if (xGrindDist == grindDistances[0])
                _workPart.__SetAttribute("DESCRIPTION",
                    string.Format("{0:f3}", grindDistances[0]) + " " + grindTolValue + " X " +
                    string.Format("{0:f2}", distances[1]) +
                    " X " +
                    string.Format("{0:f2}", distances[2]));
            if (xGrindDist == grindDistances[1])
                _workPart.__SetAttribute("DESCRIPTION",
                    string.Format("{0:f2}", distances[0]) + " X " +
                    string.Format("{0:f3}", grindDistances[1]) + " " + grindTolValue +
                    " X " +
                    string.Format("{0:f2}", distances[2]));
            if (xGrindDist == grindDistances[2])
                _workPart.__SetAttribute("DESCRIPTION",
                    string.Format("{0:f2}", distances[0]) + " X " +
                    string.Format("{0:f2}", distances[1]) + " X " +
                    string.Format("{0:f3}", grindDistances[2]) + " " + grindTolValue);
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


                foreach (Feature featDynamic in _workPart.Features)
                {
                    if (featDynamic.FeatureType != "BLOCK") continue;
                    if (featDynamic.Name != "DYNAMIC BLOCK") continue;
                    Block block1 = (Block)featDynamic;
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
                                        if (System.Math.Abs(xGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(xGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(xGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
                                        break;
                                    case "y":
                                        if (System.Math.Abs(yGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(yGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(yGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
                                        break;
                                    case "z":
                                        if (System.Math.Abs(zGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(zGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if (System.Math.Abs(zGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
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

                            if (diesetValue != "yes") continue;
                            string description =
                                _workPart.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);
                            //description += " DIESET";
                            _workPart.SetUserAttribute("DESCRIPTION", -1, $"{description} DIESET",
                                NXOpen.Update.Option.Now);
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
                            UpdateSessionParts();
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

                        UpdateSessionParts();
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
                            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Dieset = null;

                            foreach (Expression exp in _workPart.Expressions.ToArray())
                                if (exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if (Dieset != null)
                                Dieset.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            string description = selCompProto.__GetStringAttribute("DESCRIPTION");

                            if (!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                selCompProto.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                        else
                        {
                            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Dieset = null;

                            foreach (Expression exp in _workPart.Expressions.ToArray())
                                if (exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if (Dieset != null)
                                Dieset.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            string description = selCompProto.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("DIESET", "");
                            selCompProto.__SetAttribute("DESCRIPTION", description);
                        }

                    //Add Weldment stuff
                    if (comboBoxWeldment.Text != "")
                        if (comboBoxWeldment.Text == "YES")
                        {
                            Tag prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Weldment = null;

                            foreach (Expression exp in _workPart.Expressions.ToArray())
                                if (exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if (Weldment != null)
                                Weldment.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            string description = selCompProto.__GetStringAttribute("DESCRIPTION");

                            if (!description.ToLower().Contains("weldment"))
                            {
                                description += " WELDMENT";
                                selCompProto.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                        else
                        {
                            //                            NXOpen.Tag prevWp = NXOpen.Tag.Null;
                            //#pragma warning disable CS0618 // Type or member is obsolete
                            //                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
                            //#pragma warning restore CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartContextQuietly(selCompProto.Tag, out IntPtr prevWp);
                            UpdateSessionParts();

                            Expression Weldment = null;

                            foreach (Expression exp in _workPart.Expressions.ToArray())
                                if (exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if (Weldment != null)
                                Weldment.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

                            ufsession_.Assem.RestoreWorkPartContextQuietly(ref prevWp);
                            UpdateSessionParts();

                            string description = selCompProto.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("WELDMENT", "");
                            selCompProto.__SetAttribute("DESCRIPTION", description);
                        }
                    //End of add Weldment stuff
                }
                else
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
                        {
                            NewMethod43();
                        }
                        else
                        {
                            NewMethod44();
                        }

                    //Add more Weldment Stuff
                    if (comboBoxWeldment.Text != "")
                        if (comboBoxWeldment.Text == "YES")
                        {
                            NewMethod45();
                        }
                        else
                        {
                            NewMethod46();
                        }
                    //End of more Weldment stuff
                }
            }
        }



        private void SetAutoUpdateOff()
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();


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
                if (compRefCsys != null)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    BasePart compBase = (BasePart)compRefCsys.Prototype;

                    session_.Parts.SetDisplay(compBase, false, false, out PartLoadStatus setDispLoadStatus);
                    setDispLoadStatus.Dispose();
                    UpdateSessionParts();

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
                                UpdateSessionParts();

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

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                    //Ufs.Disp.Refresh();
                }
                else
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
            }
            catch (NXException ex)
            {
                UI.GetUI().NXMessageBox.Show("Caught exception in SetWcsToWorkPart", NXMessageBox.DialogType.Error,
                    ex.Message);
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
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

            UFObj.DispProps dispProps = new UFObj.DispProps();
            dispProps.color = 7;
            UFCurve.Line lineData1 = new UFCurve.Line();

            Point3d endPointX1 = mappedStartPoint1.__AddX(lineLength);
            Point3d mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            double[] startX1 = wcsOrigin.__ToArray();
            double[] endX1 = mappedEndPointX1.__ToArray();
            lineData1.start_point = startX1;
            lineData1.end_point = endX1;

            ufsession_.Disp.DisplayTemporaryLine(
                __display_part_.Views.WorkView.Tag,
                UFDisp.ViewType.UseWorkView,
                lineData1.start_point,
                lineData1.end_point,
                ref dispProps);

            ShowTemporarySizeText(lineLength, wcsOrigin, mappedEndPointX1);

            Point3d endPointY1 = mappedStartPoint1.__AddY(lineWidth);
            Point3d mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            double[] startY1 = wcsOrigin.__ToArray();
            double[] endY1 = mappedEndPointY1.__ToArray();
            lineData1.start_point = startY1;
            lineData1.end_point = endY1;

            ufsession_.Disp.DisplayTemporaryLine(
                __display_part_.Views.WorkView.Tag,
                UFDisp.ViewType.UseWorkView,
                lineData1.start_point,
                lineData1.end_point,
                ref dispProps);

            ShowTemporarySizeText(lineWidth, wcsOrigin, mappedEndPointY1);
            Point3d endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            Point3d mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            double[] startZ1 = wcsOrigin.__ToArray();
            double[] endZ1 = mappedEndPointZ1.__ToArray();
            lineData1.start_point = startZ1;
            lineData1.end_point = endZ1;

            ufsession_.Disp.DisplayTemporaryLine(
                __display_part_.Views.WorkView.Tag,
                UFDisp.ViewType.UseWorkView,
                lineData1.start_point,
                lineData1.end_point,
                ref dispProps);

            ShowTemporarySizeText(lineHeight, wcsOrigin, mappedEndPointZ1);

            //==================================================================================================================

            Point3d mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);
            Point3d endPointX2 = mappedStartPoint2.__AddX(lineLength);
            Point3d mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            double[] startX2 = mappedEndPointY1.__ToArray();
            double[] endX2 = mappedEndPointX2.__ToArray();
            lineData1.start_point = startX2;
            lineData1.end_point = endX2;

            ufsession_.Disp.DisplayTemporaryLine(
                __display_part_.Views.WorkView.Tag,
                UFDisp.ViewType.UseWorkView,
                lineData1.start_point,
                lineData1.end_point,
                ref dispProps);

            double[] startY2 = mappedEndPointX1.__ToArray();
            double[] endY2 = mappedEndPointX2.__ToArray();
            lineData1.start_point = startY2;
            lineData1.end_point = endY2;

            ufsession_.Disp.DisplayTemporaryLine(
                __display_part_.Views.WorkView.Tag,
                UFDisp.ViewType.UseWorkView,
                lineData1.start_point,
                lineData1.end_point,
                ref dispProps);

            //==================================================================================================================

            Point3d mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);

            Point3d endPointX1Ceiling =
                new Point3d(mappedStartPoint3.X + lineLength, mappedStartPoint3.Y, mappedStartPoint3.Z);
            Point3d mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            double[] startX3 = { mappedEndPointZ1.X, mappedEndPointZ1.Y, mappedEndPointZ1.Z };
            double[] endX3 = { mappedEndPointX1Ceiling.X, mappedEndPointX1Ceiling.Y, mappedEndPointX1Ceiling.Z };
            lineData1.start_point = startX3;
            lineData1.end_point = endX3;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            Point3d endPointY1Ceiling =
                new Point3d(mappedStartPoint3.X, mappedStartPoint3.Y + lineWidth, mappedStartPoint3.Z);
            Point3d mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            double[] startY3 = { mappedEndPointZ1.X, mappedEndPointZ1.Y, mappedEndPointZ1.Z };
            double[] endY3 = { mappedEndPointY1Ceiling.X, mappedEndPointY1Ceiling.Y, mappedEndPointY1Ceiling.Z };
            lineData1.start_point = startY3;
            lineData1.end_point = endY3;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            //==================================================================================================================

            Point3d mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);

            Point3d endPointX2Ceiling =
                new Point3d(mappedStartPoint4.X + lineLength, mappedStartPoint4.Y, mappedStartPoint4.Z);
            Point3d mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            double[] startX4 = { mappedEndPointY1Ceiling.X, mappedEndPointY1Ceiling.Y, mappedEndPointY1Ceiling.Z };
            double[] endX4 = { mappedEndPointX2Ceiling.X, mappedEndPointX2Ceiling.Y, mappedEndPointX2Ceiling.Z };
            lineData1.start_point = startX4;
            lineData1.end_point = endX4;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            double[] startY4 = { mappedEndPointX1Ceiling.X, mappedEndPointX1Ceiling.Y, mappedEndPointX1Ceiling.Z };
            double[] endY4 = { mappedEndPointX2Ceiling.X, mappedEndPointX2Ceiling.Y, mappedEndPointX2Ceiling.Z };
            lineData1.start_point = startY4;
            lineData1.end_point = endY4;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            //==================================================================================================================

            double[] startZ2 = { mappedEndPointX1.X, mappedEndPointX1.Y, mappedEndPointX1.Z };
            double[] endZ2 = { mappedEndPointX1Ceiling.X, mappedEndPointX1Ceiling.Y, mappedEndPointX1Ceiling.Z };
            lineData1.start_point = startZ2;
            lineData1.end_point = endZ2;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            double[] startZ3 = { mappedEndPointY1.X, mappedEndPointY1.Y, mappedEndPointY1.Z };
            double[] endZ3 = { mappedEndPointY1Ceiling.X, mappedEndPointY1Ceiling.Y, mappedEndPointY1Ceiling.Z };
            lineData1.start_point = startZ3;
            lineData1.end_point = endZ3;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            double[] startZ4 = { mappedEndPointX2.X, mappedEndPointX2.Y, mappedEndPointX2.Z };
            double[] endZ4 = { mappedEndPointX2Ceiling.X, mappedEndPointX2Ceiling.Y, mappedEndPointX2Ceiling.Z };
            lineData1.start_point = startZ4;
            lineData1.end_point = endZ4;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            if (_selComp != null)
            {
                session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                    PartCollection.WorkComponentOption.Given, out PartLoadStatus partLoad1);
                partLoad1.Dispose();
            }
            else
            {
                session_.Parts.SetWork((Part)NXObjectManager.Get(prevWork));
                UpdateSessionParts();
            }

            //==================================================================================================================
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



        private void UpdateSessionParts()
        {
            _workPart = session_.Parts.Work;
            __display_part_ = session_.Parts.Display;
        }

        private void UpdateOriginalParts()
        {
            _originalWorkPart = _workPart;
            _originalDisplayPart = __display_part_;
        }

        private void DeleteNxObjects(List<NXObject> objsToDelete)
        {
            Session.UndoMarkId markDeleteObjs;
            markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

            session_.UpdateManager.AddObjectsToDeleteList(objsToDelete.ToArray());

            _ = session_.UpdateManager.DoUpdate(markDeleteObjs);
        }




    }

}
// 4010