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
using static TSG_Library.Extensions;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set window location
            Location = Settings.Default.block_attributer_form_window_location;
            const string settingsPath = "U:\\NX110\\Concept";
            var settingsFile = Directory.GetFiles(settingsPath, "*.UCF");
            if(settingsFile.Length == 1)
            {
                var getDescription = PerformStreamReaderString(settingsFile[0], ":DESCRIPTION_ATTRIBUTE_NAME:",
                    ":END_DESCRIPTION_ATTRIBUTE_NAME:");
                var getName = PerformStreamReaderString(settingsFile[0], ":DETAIL_TYPE_ATTRIBUTE_NAME:",
                    ":END_DETAIL_TYPE_ATTRIBUTE_NAME:");
                var getMaterial = PerformStreamReaderString(settingsFile[0], ":MATERIAL_ATTRIBUTE_NAME:",
                    ":END_MATERIAL_ATTRIBUTE_NAME:");

                _customDescriptions = PerformStreamReaderList(settingsFile[0], ":CUSTOM_DESCRIPTIONS:",
                    ":END_CUSTOM_DESCRIPTIONS:");
                foreach (var cDescription in _customDescriptions)
                    cDescription.AttrName = getDescription != string.Empty ? getDescription : "DESCRIPTION";

                _compNames = PerformStreamReaderList(settingsFile[0], ":COMPONENT_NAMES:", ":END_COMPONENT_NAMES:");
                foreach (var cName in _compNames)
                    cName.AttrName = getName != string.Empty ? getName : "DETAIL NAME";

                _compMaterials = PerformStreamReaderList(settingsFile[0], ":COMPONENT_MATERIALS:",
                    ":END_COMPONENT_MATERIALS:");
                foreach (var cMaterial in _compMaterials)
                    cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

                _burnCompMaterials = PerformStreamReaderList(settingsFile[0], ":COMPONENT_BURN_MATERIALS:",
                    ":END_COMPONENT_BURN_MATERIALS:");
                foreach (var cMaterial in _burnCompMaterials)
                    cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

                _purchasedMaterials = PerformStreamReaderList(settingsFile[0], ":PURCHASED_MATERIALS:",
                    ":END_PURCHASED_MATERIALS:");
                foreach (var purMaterial in _purchasedMaterials)
                    purMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";

                _compTolerances = PerformStreamReaderList(settingsFile[0], ":COMPONENT_TOLERANCES:",
                    ":END_COMPONENT_TOLERANCES:");
                foreach (var cTolerance in _compTolerances)
                    cTolerance.AttrName = "TOLERANCE";

                LoadDefaultFormData();

                groupBoxDescription.Enabled = false;
                groupBoxMaterial.Enabled = false;
                groupBoxAddStock.Enabled = false;
                groupBoxBurnSettings.Enabled = false;
                groupBoxAttributes.Enabled = false;
            }
            else
            {
                UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "*.UCF file does not exist");
            }
        }

        public void WorkPartChanged1(BasePart p)
        {
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(WindowState == FormWindowState.Normal)
            {
                Settings.Default.block_attributer_form_window_location = Location;
                Settings.Default.Save();
            }
            else
            {
                //Settings.Default.WindowSize = RestoreBounds.Size;
                //Settings.Default.WindowLocation = Location;
                Settings.Default.Save();
            }
        }

        private void comboBoxDescription_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxDescription.Text == string.Empty) return;
            textBoxDescription.Clear();
            textBoxDescription.Text = comboBoxDescription.Text;
            comboBoxDescription.SelectedIndex = -1;
            comboBoxDescription.Text = string.Empty;
            textBoxDescription.Focus();
        }

        private void comboBoxPurMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxPurMaterials.SelectedIndex != -1)
            {
                textBoxMaterial.Text = string.Empty;
                comboBoxCustomMaterials.SelectedIndex = -1;
            }
        }

        private void comboBoxCustomMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxCustomMaterials.SelectedIndex != -1)
            {
                textBoxMaterial.Text = string.Empty;
                comboBoxPurMaterials.SelectedIndex = -1;

                var material = (CtsAttributes)comboBoxCustomMaterials.SelectedItem;

                if(material.AttrValue == "STEELCRAFT")
                    textBoxDescription.Text = "NITROGEN PLATE SYSTEM";
            }
        }

        private void comboBoxAddx_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBoundingBox();
        }

        private void comboBoxAddy_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBoundingBox();
        }

        private void comboBoxAddz_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBoundingBox();
        }

        private void checkBoxBurnDirX_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxBurnDirX.Checked)
            {
                checkBoxBurnDirY.Checked = false;
                checkBoxBurnDirZ.Checked = false;
            }
        }

        private void checkBoxBurnDirY_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxBurnDirY.Checked)
            {
                checkBoxBurnDirX.Checked = false;
                checkBoxBurnDirZ.Checked = false;
            }
        }

        private void checkBoxBurnDirZ_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxBurnDirZ.Checked)
            {
                checkBoxBurnDirX.Checked = false;
                checkBoxBurnDirY.Checked = false;
            }
        }

        private void checkBoxBurnout_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxBurnout.Checked)
            {
                checkBoxBurnDirZ.Checked = true;
                comboBoxMaterial.Items.Clear();
                //Add code here to populate list for Burn Components
                foreach (var custBurnMatl in _burnCompMaterials)
                    comboBoxMaterial.Items.Add(custBurnMatl);
            }
            else if(!checkBoxBurnout.Checked)
            {
                checkBoxBurnDirZ.Checked = false;
                comboBoxMaterial.Items.Clear();
                foreach (var matl in _compMaterials)
                    comboBoxMaterial.Items.Add(matl);
            }
            else
            {
                if(!checkBoxGrind.Checked)
                {
                    checkBoxBurnDirX.Checked = false;
                    checkBoxBurnDirY.Checked = false;
                    checkBoxBurnDirZ.Checked = false;
                }
            }
        }

        private void checkBoxGrind_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxGrind.Checked)
            {
                comboBoxTolerance.Enabled = true;
                comboBoxTolerance.SelectedIndex = 0;
                checkBoxBurnDirZ.Checked = true;
            }
            else
            {
                if(!checkBoxBurnout.Checked)
                {
                    comboBoxTolerance.Enabled = false;
                    comboBoxTolerance.SelectedIndex = -1;
                    checkBoxBurnDirX.Checked = false;
                    checkBoxBurnDirY.Checked = false;
                    checkBoxBurnDirZ.Checked = false;
                }
                else
                {
                    comboBoxTolerance.Enabled = false;
                    comboBoxTolerance.SelectedIndex = -1;
                }
            }
        }

        private void textBoxMaterial_Click(object sender, EventArgs e)
        {
            comboBoxPurMaterials.SelectedIndex = -1;
            comboBoxCustomMaterials.SelectedIndex = -1;
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            if(textBoxDescription.Text != string.Empty)
            {
                var isBlockComp = false;

                if(_selComp != null)
                {
                    var compProto = (Part)_selComp.Prototype;

                    foreach (Feature featDynamic in compProto.Features)
                        if(featDynamic.FeatureType == "BLOCK")
                            if(featDynamic.Name == "DYNAMIC BLOCK")
                                isBlockComp = true;
                    if(isBlockComp)
                        groupBoxDescription.Text = "Description = Auto Update Off";
                }
            }
            else
            {
                groupBoxDescription.Text = "Description";
            }
        }

        private void textBoxDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Tab))
                textBoxMaterial.Focus();
        }

        private void textBoxMaterial_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Tab))
                buttonApply.Focus();
            if(e.KeyCode.Equals(Keys.Return))
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

                if(_selComp != null)
                {
                    var attrInfo = _selComp.__GetAttributes();

                    if(attrInfo.Length != 0)
                        foreach (var attr in _selComp.__GetAttributes())
                        {
                            if(attr.Title == "DESCRIPTION")
                                if(_selComp.__GetStringAttribute(attr.Title) != "")
                                    textBoxDescription.Text = _selComp.__GetStringAttribute(attr.Title);
                                else
                                    textBoxDescription.Text = "NO DESCRIPTION";
                            if(attr.Title == "MATERIAL")
                                if(_selComp.__GetStringAttribute(attr.Title) != "")
                                    textBoxMaterial.Text = _selComp.__GetStringAttribute(attr.Title);
                                else
                                    textBoxMaterial.Text = "NO MATERIAL";
                        }

                    textBoxDescription.Focus();
                    groupBoxBlockExpressions.Enabled = false;
                    buttonApply.Enabled = true;
                }
                else
                {
                    var attrInfo = _workPart.__GetAttributes();

                    if(attrInfo.Length != 0)
                        foreach (var attr in _workPart.__GetAttributes())
                        {
                            if(attr.Title == "DESCRIPTION")
                                if(_workPart.__GetStringAttribute(attr.Title) != "")
                                    textBoxDescription.Text = _workPart.__GetStringAttribute(attr.Title);
                                else
                                    textBoxDescription.Text = "NO DESCRIPTION";
                            if(attr.Title == "MATERIAL")
                                if(_workPart.__GetStringAttribute(attr.Title) != "")
                                    textBoxMaterial.Text = _workPart.__GetStringAttribute(attr.Title);
                                else
                                    textBoxMaterial.Text = "NO MATERIAL";
                        }

                    groupBoxBlockExpressions.Enabled = false;
                    buttonApply.Enabled = true;
                }
            }
            catch (NXException ex)
            {
                TheUi.NXMessageBox.Show("Caught exception : Select Custom", NXMessageBox.DialogType.Error, ex.Message);
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

                if(_allSelectedComponents.Count > 0)
                    _selectedComponents = GetOneComponentOfMany(_allSelectedComponents);
            }
            catch (NXException ex)
            {
                TheUi.NXMessageBox.Show("Caught exception : Select Multiple", NXMessageBox.DialogType.Error,
                    ex.Message);
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

                UpdateSessionParts();
                UpdateOriginalParts();

                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;

                _selectedComponents = SelectMultipleComponents();

                if(_selectedComponents.Count > 0)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    foreach (var diesetComp in _selectedComponents)
                    {
                        var makeDisp = (BasePart)diesetComp.Prototype;
                        session_.Parts.SetDisplay(makeDisp, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();
                        UpdateSessionParts();

                        Expression noteExp = null;
                        var isExpression = false;

                        foreach (Expression exp in _workPart.Expressions)
                            if(exp.Name == "DiesetNote")
                            {
                                isExpression = true;
                                noteExp = exp;
                            }

                        var isDescription = false;
                        var description = string.Empty;

                        foreach (var attr in _workPart.__GetAttributes())
                            if(attr.Title == "DESCRIPTION")
                                isDescription = true;

                        if(isDescription)
                            description = _workPart.__GetStringAttribute("DESCRIPTION");
                        else
                            _workPart.__SetAttribute("DESCRIPTION", "NO DESCRIPTION");

                        if(description != "")
                        {
                            if(!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }

                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"yes\"";
                            }
                            else
                            {
                                var diesetExp = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");
                            }
                        }
                        else
                        {
                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"yes\"";
                            }
                            else
                            {
                                var diesetExp = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");
                            }
                        }
                    }

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                    partLoadStatus2.Dispose();
                    UpdateSessionParts();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                }
            }
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                TheUi.NXMessageBox.Show("Caught exception : Select Dieset", NXMessageBox.DialogType.Error, ex.Message);
                ufsession_.Disp.RegenerateDisplay();
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

                if(_selectedComponents.Count > 0)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    foreach (var diesetComp in _selectedComponents)
                    {
                        var makeDisp = (BasePart)diesetComp.Prototype;
                        session_.Parts.SetDisplay(makeDisp, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();
                        UpdateSessionParts();

                        Expression noteExp = null;
                        var isExpression = false;

                        foreach (Expression exp in _workPart.Expressions)
                            if(exp.Name == "DiesetNote")
                            {
                                isExpression = true;
                                noteExp = exp;
                            }

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");

                        if(description != "")
                        {
                            description = description.Replace(" DIESET", "");
                            _workPart.__SetAttribute("DESCRIPTION", description);

                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"no\"";
                            }
                            else
                            {
                                var diesetExp = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");
                            }
                        }
                        else
                        {
                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"no\"";
                            }
                            else
                            {
                                var diesetExp = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");
                            }
                        }
                    }

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                    partLoadStatus2.Dispose();
                    UpdateSessionParts();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                }
            }
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                TheUi.NXMessageBox.Show("Caught exception : Select Dieset", NXMessageBox.DialogType.Error, ex.Message);
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

                UpdateSessionParts();
                UpdateOriginalParts();

                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;

                _selectedComponents = SelectMultipleComponents();

                if(_selectedComponents.Count > 0)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    foreach (var weldmentComp in _selectedComponents)
                    {
                        var makeDisp = (BasePart)weldmentComp.Prototype;
                        session_.Parts.SetDisplay(makeDisp, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();
                        UpdateSessionParts();

                        Expression noteExp = null;
                        var isExpression = false;

                        foreach (Expression exp in _workPart.Expressions)
                            if(exp.Name == "WeldmentNote")
                            {
                                isExpression = true;
                                noteExp = exp;
                            }

                        var isDescription = false;
                        var description = string.Empty;

                        foreach (var attr in _workPart.__GetAttributes())
                            if(attr.Title == "DESCRIPTION")
                                isDescription = true;

                        if(isDescription)
                            description = _workPart.__GetStringAttribute("DESCRIPTION");
                        else
                            _workPart.__SetAttribute("DESCRIPTION", "NO DESCRIPTION");

                        if(description != "")
                        {
                            if(!description.ToLower().Contains("weldment"))
                            {
                                description += " WELDMENT";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }

                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"yes\"";
                            }
                            else
                            {
                                var weldmentExp =
                                    _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");
                            }
                        }
                        else
                        {
                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"yes\"";
                            }
                            else
                            {
                                var weldmentExp =
                                    _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");
                            }
                        }
                    }

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                    partLoadStatus2.Dispose();
                    UpdateSessionParts();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                }
            }
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                TheUi.NXMessageBox.Show("Caught exception : Select Weldment", NXMessageBox.DialogType.Error,
                    ex.Message);
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

                UpdateSessionParts();
                UpdateOriginalParts();

                _isCustom = false;
                _isMeasureBody = false;
                _isSelectMultiple = false;

                _selectedComponents = SelectMultipleComponents();

                if(_selectedComponents.Count > 0)
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    foreach (var weldmentComp in _selectedComponents)
                    {
                        var makeDisp = (BasePart)weldmentComp.Prototype;
                        session_.Parts.SetDisplay(makeDisp, false, false, out var partLoadStatus1);
                        partLoadStatus1.Dispose();
                        UpdateSessionParts();

                        Expression noteExp = null;
                        var isExpression = false;

                        foreach (Expression exp in _workPart.Expressions)
                            if(exp.Name == "WeldmentNote")
                            {
                                isExpression = true;
                                noteExp = exp;
                            }

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");

                        if(description != "")
                        {
                            description = description.Replace(" WELDMENT", "");
                            _workPart.__SetAttribute("DESCRIPTION", description);

                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"no\"";
                            }
                            else
                            {
                                var weldmentExp =
                                    _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");
                            }
                        }
                        else
                        {
                            if(isExpression)
                            {
                                noteExp.RightHandSide = "\"no\"";
                            }
                            else
                            {
                                var weldmentExp =
                                    _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");
                            }
                        }
                    }

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var partLoadStatus2);
                    partLoadStatus2.Dispose();
                    UpdateSessionParts();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                }
            }
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                TheUi.NXMessageBox.Show("Caught exception : Select Weldment", NXMessageBox.DialogType.Error,
                    ex.Message);
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

                var isNamedExpression = false;

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
                var unitsMatch = true;

                _sizeBody = SelectOneComponentBody();


                //Revision 1.01 – 12/29/16
                //Added a dialog for the “Select Block” process

                if(_sizeBody == null) return;
                const string autoUpdate = "AUTO UPDATE";
                var owningPart = _sizeBody.IsOccurrence
                    ? (Part)_sizeBody.OwningComponent.Prototype
                    : (Part)_sizeBody.OwningPart;
                var hasAutoUpdate = owningPart.HasUserAttribute(autoUpdate, NXObject.AttributeType.String, -1);
                if(hasAutoUpdate)
                {
                    var autoUpdateValue = owningPart.__GetStringAttribute(autoUpdate);
                    if(autoUpdateValue == "OFF")
                    {
                        const string message =
                            "Auto Update is currently off.\nClicking yes will turn it on.\nClicking no will cancel the current selection process.";
                        var dislogResult = MessageBox.Show(message, "Continue?", MessageBoxButtons.YesNo);

                        if(dislogResult == DialogResult.No)
                            return;

                        owningPart.SetUserAttribute(autoUpdate, -1, "ON", NXOpen.Update.Option.Now);
                        var listingWindow = Session.GetSession().ListingWindow;
                        listingWindow.Open();
                        listingWindow.WriteLine($"{autoUpdate} has been set to \"On\".");
                    }
                }
                //End revision 1.01


                if(_sizeBody != null)
                {
                    _selComp = _sizeBody.OwningComponent;

                    if(_selComp != null)
                    {
                        _selComp.Unhighlight();

                        var makeWork = (Part)_selComp.Prototype;
                        var wpUnits = makeWork.PartUnits;

                        if(__display_part_.PartUnits == wpUnits)
                        {
                            session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                                PartCollection.WorkComponentOption.Given,
                                out var loadStatus1);
                            loadStatus1.Dispose();

                            UpdateSessionParts();
                        }
                        else
                        {
                            unitsMatch = false;
                        }
                    }

                    if(unitsMatch)
                    {
                        var isBlockComp = false;

                        foreach (Feature featDynamic in _workPart.Features)
                            if(featDynamic.FeatureType == "BLOCK")
                                if(featDynamic.Name == "DYNAMIC BLOCK")
                                    isBlockComp = true;
                        if(isBlockComp)
                        {
                            SetWcsToWorkPart(_selComp);

                            // get named expressions

                            foreach (var exp in _workPart.Expressions.ToArray())
                            {
                                if(exp.Name == "AddX")
                                {
                                    isNamedExpression = true;
                                    AddX = exp;
                                    xValue = exp.Value;
                                }

                                if(exp.Name == "AddY")
                                {
                                    isNamedExpression = true;
                                    AddY = exp;
                                    yValue = exp.Value;
                                }

                                if(exp.Name == "AddZ")
                                {
                                    isNamedExpression = true;
                                    AddZ = exp;
                                    zValue = exp.Value;
                                }

                                if(exp.Name == "BurnDir")
                                {
                                    isNamedExpression = true;
                                    BurnDir = exp;
                                    burnDirValue = exp.RightHandSide;
                                }

                                if(exp.Name == "Burnout")
                                {
                                    isNamedExpression = true;
                                    Burnout = exp;
                                    burnoutValue = exp.RightHandSide;
                                }

                                if(exp.Name == "Grind")
                                {
                                    isNamedExpression = true;
                                    Grind = exp;
                                    grindValue = exp.RightHandSide;
                                }

                                if(exp.Name == "GrindTolerance")
                                {
                                    isNamedExpression = true;
                                    GrindTolerance = exp;
                                    grindTolValue = exp.RightHandSide;
                                }
                            }

                            burnDirValue = burnDirValue.Replace("\"", string.Empty);
                            burnoutValue = burnoutValue.Replace("\"", string.Empty);
                            grindValue = grindValue.Replace("\"", string.Empty);
                            grindTolValue = grindTolValue.Replace("\"", string.Empty);

                            if(isNamedExpression)
                            {
                                foreach (CtsAttributes addX in comboBoxAddx.Items)
                                    try
                                    {
                                        if(AddX.RightHandSide == addX.AttrValue)
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
                                    if(AddY.RightHandSide == addY.AttrValue)
                                    {
                                        comboBoxAddy.SelectedItem = addY;

                                        break;
                                    }

                                    comboBoxAddy.SelectedIndex = 0;
                                }

                                foreach (CtsAttributes addZ in comboBoxAddz.Items)
                                {
                                    if(AddZ.RightHandSide == addZ.AttrValue)
                                    {
                                        comboBoxAddz.SelectedItem = addZ;

                                        break;
                                    }

                                    comboBoxAddz.SelectedIndex = 0;
                                }

                                if(burnoutValue.ToLower() == "yes")
                                    checkBoxBurnout.Checked = true;
                                else
                                    checkBoxBurnout.Checked = false;
                                if(grindValue.ToLower() == "yes")
                                    checkBoxGrind.Checked = true;
                                else
                                    checkBoxGrind.Checked = false;
                                if(burnDirValue.ToLower() == "x")
                                    checkBoxBurnDirX.Checked = true;
                                if(burnDirValue.ToLower() == "y")
                                    checkBoxBurnDirY.Checked = true;
                                if(burnDirValue.ToLower() == "z")
                                    checkBoxBurnDirZ.Checked = true;
                                foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                                    if(grindTolValue == tolSetting.AttrValue)
                                        comboBoxTolerance.SelectedItem = tolSetting;


                                // get bounding box info

                                var minCorner = new double[3];
                                var directions = new double[3, 3];
                                var distances = new double[3];

                                ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                                    __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

                                // add stock values

                                distances[0] += xValue;
                                distances[1] += yValue;
                                distances[2] += zValue;

                                if(_workPart.PartUnits == BasePart.Units.Millimeters)
                                    for (var i = 0; i < distances.Length; i++)
                                        distances[i] /= 25.4d;

                                for (var i = 0; i < 3; i++)
                                {
                                    var roundValue = System.Math.Round(distances[i], 3);
                                    var truncateValue = System.Math.Truncate(roundValue);
                                    var fractionValue = roundValue - truncateValue;
                                    if(fractionValue != 0)
                                        for (var ii = .125; ii <= 1; ii += .125)
                                        {
                                            if(fractionValue <= ii)
                                            {
                                                var roundedFraction = ii;
                                                var finalValue = truncateValue + roundedFraction;
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
                            else
                            {
                                CreateCompExpressions();

                                // get named expressions

                                foreach (var exp in _workPart.Expressions.ToArray())
                                {
                                    if(exp.Name == "AddX")
                                    {
                                        isNamedExpression = true;
                                        AddX = exp;
                                        xValue = exp.Value;
                                    }

                                    if(exp.Name == "AddY")
                                    {
                                        isNamedExpression = true;
                                        AddY = exp;
                                        yValue = exp.Value;
                                    }

                                    if(exp.Name == "AddZ")
                                    {
                                        isNamedExpression = true;
                                        AddZ = exp;
                                        zValue = exp.Value;
                                    }

                                    if(exp.Name == "BurnDir")
                                    {
                                        isNamedExpression = true;
                                        BurnDir = exp;
                                        burnDirValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "Burnout")
                                    {
                                        isNamedExpression = true;
                                        Burnout = exp;
                                        burnoutValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "Grind")
                                    {
                                        isNamedExpression = true;
                                        Grind = exp;
                                        grindValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "GrindTolerance")
                                    {
                                        isNamedExpression = true;
                                        GrindTolerance = exp;
                                        grindTolValue = exp.RightHandSide;
                                    }
                                }

                                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                                grindValue = grindValue.Replace("\"", string.Empty);
                                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                                if(isNamedExpression)
                                {
                                    foreach (CtsAttributes addX in comboBoxAddx.Items)
                                    {
                                        if(AddX.RightHandSide == addX.AttrValue)
                                        {
                                            comboBoxAddx.SelectedItem = addX;

                                            break;
                                        }

                                        comboBoxAddx.SelectedIndex = 0;
                                    }

                                    foreach (CtsAttributes addY in comboBoxAddy.Items)
                                    {
                                        if(AddY.RightHandSide == addY.AttrValue)
                                        {
                                            comboBoxAddy.SelectedItem = addY;

                                            break;
                                        }

                                        comboBoxAddy.SelectedIndex = 0;
                                    }

                                    foreach (CtsAttributes addZ in comboBoxAddz.Items)
                                    {
                                        if(AddZ.RightHandSide == addZ.AttrValue)
                                        {
                                            comboBoxAddz.SelectedItem = addZ;

                                            break;
                                        }

                                        comboBoxAddz.SelectedIndex = 0;
                                    }

                                    if(burnoutValue.ToLower() == "yes")
                                        checkBoxBurnout.Checked = true;
                                    else
                                        checkBoxBurnout.Checked = false;
                                    if(grindValue.ToLower() == "yes")
                                        checkBoxGrind.Checked = true;
                                    else
                                        checkBoxGrind.Checked = false;
                                    if(burnDirValue.ToLower() == "x")
                                        checkBoxBurnDirX.Checked = true;
                                    if(burnDirValue.ToLower() == "y")
                                        checkBoxBurnDirY.Checked = true;
                                    if(burnDirValue.ToLower() == "z")
                                        checkBoxBurnDirZ.Checked = true;
                                    foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                                        if(grindTolValue == tolSetting.AttrValue)
                                            comboBoxTolerance.SelectedItem = tolSetting;

                                    // get bounding box info

                                    var minCorner = new double[3];
                                    var directions = new double[3, 3];
                                    var distances = new double[3];

                                    ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                                        __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

                                    // add stock values

                                    distances[0] += xValue;
                                    distances[1] += yValue;
                                    distances[2] += zValue;

                                    if(_workPart.PartUnits == BasePart.Units.Millimeters)
                                        for (var i = 0; i < distances.Length; i++)
                                            distances[i] /= 25.4d;

                                    for (var i = 0; i < 3; i++)
                                    {
                                        var roundValue = System.Math.Round(distances[i], 3);
                                        var truncateValue = System.Math.Truncate(roundValue);
                                        var fractionValue = roundValue - truncateValue;
                                        if(fractionValue != 0)
                                            for (var ii = .125; ii <= 1; ii += .125)
                                            {
                                                if(fractionValue <= ii)
                                                {
                                                    var roundedFraction = ii;
                                                    var finalValue = truncateValue + roundedFraction;
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
                                .NXMessageBox.Show("Caught exception : Select Block", NXMessageBox.DialogType.Error,
                                    "Not a block component\r\n" + "Select Measure");
                        }
                    }
                    else
                    {
                        UI.GetUI().NXMessageBox.Show("Caught exception : Select Block", NXMessageBox.DialogType.Error,
                            "Part units do not match");
                        buttonReset.PerformClick();
                    }
                }
                else
                {
                    buttonReset.PerformClick();
                }
            }
            catch (NXException ex)
            {
                TheUi.NXMessageBox.Show("Caught exception : Select Block", NXMessageBox.DialogType.Error, ex.Message);
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

                var isNamedExpression = false;

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

                var unitsMatch = true;

                UI.GetUI()
                    .NXMessageBox.Show("Measure Body", NXMessageBox.DialogType.Information,
                        "1. Set expression values\r\n" + "2. Orient WCS\r\n" + "3. Select Apply");

                _sizeBody = SelectOneComponentBody();

                if(_sizeBody != null)
                {
                    _selComp = _sizeBody.OwningComponent;

                    if(_selComp != null)
                    {
                        _selComp.Unhighlight();

                        var makeWork = (Part)_selComp.Prototype;
                        var wpUnits = makeWork.PartUnits;

                        if(__display_part_.PartUnits == wpUnits)
                        {
                            session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                                PartCollection.WorkComponentOption.Given,
                                out var loadStatus1);
                            loadStatus1.Dispose();

                            UpdateSessionParts();
                        }
                        else
                        {
                            unitsMatch = false;
                        }
                    }

                    if(unitsMatch)
                    {
                        var isBlockComp = false;

                        foreach (Feature featDynamic in _workPart.Features)
                            if(featDynamic.FeatureType == "BLOCK")
                                if(featDynamic.Name == "DYNAMIC BLOCK")
                                    isBlockComp = true;
                        if(!isBlockComp)
                        {
                            // get named expressions

                            foreach (var exp in _workPart.Expressions.ToArray())
                            {
                                if(exp.Name == "AddX")
                                {
                                    isNamedExpression = true;
                                    AddX = exp;
                                    xValue = exp.Value;
                                }

                                if(exp.Name == "AddY")
                                {
                                    isNamedExpression = true;
                                    AddY = exp;
                                    yValue = exp.Value;
                                }

                                if(exp.Name == "AddZ")
                                {
                                    isNamedExpression = true;
                                    AddZ = exp;
                                    zValue = exp.Value;
                                }

                                if(exp.Name == "BurnDir")
                                {
                                    isNamedExpression = true;
                                    BurnDir = exp;
                                    burnDirValue = exp.RightHandSide;
                                }

                                if(exp.Name == "Burnout")
                                {
                                    isNamedExpression = true;
                                    Burnout = exp;
                                    burnoutValue = exp.RightHandSide;
                                }

                                if(exp.Name == "Grind")
                                {
                                    isNamedExpression = true;
                                    Grind = exp;
                                    grindValue = exp.RightHandSide;
                                }

                                if(exp.Name == "GrindTolerance")
                                {
                                    isNamedExpression = true;
                                    GrindTolerance = exp;
                                    grindTolValue = exp.RightHandSide;
                                }
                            }

                            burnDirValue = burnDirValue.Replace("\"", string.Empty);
                            burnoutValue = burnoutValue.Replace("\"", string.Empty);
                            grindValue = grindValue.Replace("\"", string.Empty);
                            grindTolValue = grindTolValue.Replace("\"", string.Empty);

                            if(isNamedExpression)
                            {
                                foreach (CtsAttributes addX in comboBoxAddx.Items)
                                {
                                    if(AddX.RightHandSide == addX.AttrValue)
                                    {
                                        comboBoxAddx.SelectedItem = addX;

                                        break;
                                    }

                                    comboBoxAddx.SelectedIndex = 0;
                                }

                                foreach (CtsAttributes addY in comboBoxAddy.Items)
                                {
                                    if(AddY.RightHandSide == addY.AttrValue)
                                    {
                                        comboBoxAddy.SelectedItem = addY;

                                        break;
                                    }

                                    comboBoxAddy.SelectedIndex = 0;
                                }

                                foreach (CtsAttributes addZ in comboBoxAddz.Items)
                                {
                                    if(AddZ.RightHandSide == addZ.AttrValue)
                                    {
                                        comboBoxAddz.SelectedItem = addZ;

                                        break;
                                    }

                                    comboBoxAddz.SelectedIndex = 0;
                                }

                                if(burnoutValue.ToLower() == "yes")
                                    checkBoxBurnout.Checked = true;
                                else
                                    checkBoxBurnout.Checked = false;
                                if(grindValue.ToLower() == "yes")
                                    checkBoxGrind.Checked = true;
                                else
                                    checkBoxGrind.Checked = false;
                                if(burnDirValue.ToLower() == "x")
                                    checkBoxBurnDirX.Checked = true;
                                if(burnDirValue.ToLower() == "y")
                                    checkBoxBurnDirY.Checked = true;
                                if(burnDirValue.ToLower() == "z")
                                    checkBoxBurnDirZ.Checked = true;
                                foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                                    if(grindTolValue == tolSetting.AttrValue)
                                        comboBoxTolerance.SelectedItem = tolSetting;

                                // get bounding box info

                                var minCorner = new double[3];
                                var directions = new double[3, 3];
                                var distances = new double[3];

                                ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                                    __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

                                // add stock values

                                distances[0] += xValue;
                                distances[1] += yValue;
                                distances[2] += zValue;

                                if(_workPart.PartUnits == BasePart.Units.Millimeters)
                                    for (var i = 0; i < distances.Length; i++)
                                        distances[i] /= 25.4d;

                                for (var i = 0; i < 3; i++)
                                {
                                    var roundValue = System.Math.Round(distances[i], 3);
                                    var truncateValue = System.Math.Truncate(roundValue);
                                    var fractionValue = roundValue - truncateValue;
                                    if(fractionValue != 0)
                                        for (var ii = .125; ii <= 1; ii += .125)
                                        {
                                            if(fractionValue <= ii)
                                            {
                                                var roundedFraction = ii;
                                                var finalValue = truncateValue + roundedFraction;
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
                            else
                            {
                                CreateCompExpressions();

                                foreach (var exp in _workPart.Expressions.ToArray())
                                {
                                    if(exp.Name == "AddX")
                                    {
                                        isNamedExpression = true;
                                        AddX = exp;
                                        xValue = exp.Value;
                                    }

                                    if(exp.Name == "AddY")
                                    {
                                        isNamedExpression = true;
                                        AddY = exp;
                                        yValue = exp.Value;
                                    }

                                    if(exp.Name == "AddZ")
                                    {
                                        isNamedExpression = true;
                                        AddZ = exp;
                                        zValue = exp.Value;
                                    }

                                    if(exp.Name == "BurnDir")
                                    {
                                        isNamedExpression = true;
                                        BurnDir = exp;
                                        burnDirValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "Burnout")
                                    {
                                        isNamedExpression = true;
                                        Burnout = exp;
                                        burnoutValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "Grind")
                                    {
                                        isNamedExpression = true;
                                        Grind = exp;
                                        grindValue = exp.RightHandSide;
                                    }

                                    if(exp.Name == "GrindTolerance")
                                    {
                                        isNamedExpression = true;
                                        GrindTolerance = exp;
                                        grindTolValue = exp.RightHandSide;
                                    }
                                }

                                burnDirValue = burnDirValue.Replace("\"", string.Empty);
                                burnoutValue = burnoutValue.Replace("\"", string.Empty);
                                grindValue = grindValue.Replace("\"", string.Empty);
                                grindTolValue = grindTolValue.Replace("\"", string.Empty);

                                if(isNamedExpression)
                                {
                                    foreach (CtsAttributes addX in comboBoxAddx.Items)
                                    {
                                        if(AddX.RightHandSide == addX.AttrValue)
                                        {
                                            comboBoxAddx.SelectedItem = addX;

                                            break;
                                        }

                                        comboBoxAddx.SelectedIndex = 0;
                                    }

                                    foreach (CtsAttributes addY in comboBoxAddy.Items)
                                    {
                                        if(AddY.RightHandSide == addY.AttrValue)
                                        {
                                            comboBoxAddy.SelectedItem = addY;

                                            break;
                                        }

                                        comboBoxAddy.SelectedIndex = 0;
                                    }

                                    foreach (CtsAttributes addZ in comboBoxAddz.Items)
                                    {
                                        if(AddZ.RightHandSide == addZ.AttrValue)
                                        {
                                            comboBoxAddz.SelectedItem = addZ;

                                            break;
                                        }

                                        comboBoxAddz.SelectedIndex = 0;
                                    }

                                    if(burnoutValue.ToLower() == "yes")
                                        checkBoxBurnout.Checked = true;
                                    else
                                        checkBoxBurnout.Checked = false;
                                    if(grindValue.ToLower() == "yes")
                                        checkBoxGrind.Checked = true;
                                    else
                                        checkBoxGrind.Checked = false;
                                    if(burnDirValue.ToLower() == "x")
                                        checkBoxBurnDirX.Checked = true;
                                    if(burnDirValue.ToLower() == "y")
                                        checkBoxBurnDirY.Checked = true;
                                    if(burnDirValue.ToLower() == "z")
                                        checkBoxBurnDirZ.Checked = true;
                                    foreach (CtsAttributes tolSetting in comboBoxTolerance.Items)
                                        if(grindTolValue == tolSetting.AttrValue)
                                            comboBoxTolerance.SelectedItem = tolSetting;

                                    // get bounding box info

                                    var minCorner = new double[3];
                                    var directions = new double[3, 3];
                                    var distances = new double[3];

                                    ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag,
                                        __display_part_.WCS.CoordinateSystem.Tag, minCorner, directions, distances);

                                    // add stock values

                                    distances[0] += xValue;
                                    distances[1] += yValue;
                                    distances[2] += zValue;

                                    if(_workPart.PartUnits == BasePart.Units.Millimeters)
                                        for (var i = 0; i < distances.Length; i++)
                                            distances[i] /= 25.4d;

                                    for (var i = 0; i < 3; i++)
                                    {
                                        var roundValue = System.Math.Round(distances[i], 3);
                                        var truncateValue = System.Math.Truncate(roundValue);
                                        var fractionValue = roundValue - truncateValue;
                                        if(fractionValue != 0)
                                            for (var ii = .125; ii <= 1; ii += .125)
                                            {
                                                if(fractionValue <= ii)
                                                {
                                                    var roundedFraction = ii;
                                                    var finalValue = truncateValue + roundedFraction;
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
            catch (NXException ex)
            {
                TheUi.NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error, ex.Message);
            }
        }

        private void buttonSetAutoUpdateOn_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                var myUDOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");

                if(myUDOclass != null)
                {
                    var selectedComps = SelectMultipleComponents();

                    if(selectedComps.Count > 0)
                    {
                        ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                        foreach (var sComp in selectedComps)
                        {
                            sComp.Unhighlight();

                            var wpComp = (Part)sComp.Prototype;

                            ufsession_.Part.SetDisplayPart(wpComp.Tag);
                            ufsession_.Assem.SetWorkPart(wpComp.Tag);
                            UpdateSessionParts();

                            UserDefinedObject[] currentUdo;
                            currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUDOclass);

                            if(currentUdo.Length == 1)
                            {
                                var myUDO = currentUdo[0];

                                var updateFlag = myUDO.GetIntegers();

                                int[] updateOn = { 1 };
                                myUDO.SetIntegers(updateOn);
                                _workPart.__SetAttribute("AUTO UPDATE", "ON");
                            }

                            UpdateBlockDescription();
                        }

                        ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                        ufsession_.Disp.RegenerateDisplay();

                        ufsession_.Part.SetDisplayPart(_originalDisplayPart.Tag);
                        ufsession_.Assem.SetWorkPart(_originalWorkPart.Tag);
                        UpdateSessionParts();
                    }
                    else
                    {
                        UserDefinedObject[] currentUdo;
                        currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUDOclass);

                        if(currentUdo.Length == 1)
                        {
                            var myUDO = currentUdo[0];

                            var updateFlag = myUDO.GetIntegers();

                            int[] updateOn = { 1 };
                            myUDO.SetIntegers(updateOn);
                            _workPart.__SetAttribute("AUTO UPDATE", "ON");
                        }

                        UpdateBlockDescription();
                    }
                }
            }
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                UI.GetUI().NXMessageBox.Show("Caught exception : Auto Update On", NXMessageBox.DialogType.Error,
                    ex.Message);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();

            session_.Parts.SetWork(__display_part_);

            if(_selectedComponents.Count > 0)
                foreach (var comp in _selectedComponents)
                    comp.Unhighlight();

            if(_selComp != null)
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
            _ = session_.SetUndoMark(Session.MarkVisibility.Visible, "Block Attributer");
            try
            {
                if(_isSelectMultiple)
                {
                    if(_selectedComponents.Count > 0)
                        foreach (var sComp in _selectedComponents)
                        {
                            _selComp = sComp;
                            UpdateCompAttributes();
                            sComp.Unhighlight();
                        }

                    buttonReset.PerformClick();
                }
                else
                {
                    UpdateCompExpressions();

                    if(_selComp != null)
                        _selComp.Unhighlight();

                    if(_isMeasureBody)
                    {
                        MeasureComponentBody();
                        buttonReset.PerformClick();
                    }
                    else
                    {
                        UpdateBlockDescription();
                        UpdateCompAttributes();
                        buttonReset.PerformClick();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            comboBoxName.Text = "";
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

            foreach (var desc in _customDescriptions)
                comboBoxDescription.Items.Add(desc);
            foreach (var purMatl in _purchasedMaterials)
                comboBoxPurMaterials.Items.Add(purMatl);
            foreach (var custMatl in _compMaterials)
                comboBoxCustomMaterials.Items.Add(custMatl);

            for (double i = 0; i < 1.125; i += .125)
            {
                var addStock = new CtsAttributes("", string.Format("{0:f3}", i));

                if(addStock.AttrValue.StartsWith("0"))
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

            foreach (var tol in _compTolerances)
                comboBoxTolerance.Items.Add(tol);
            foreach (var matl in _compMaterials)
                comboBoxMaterial.Items.Add(matl);
            foreach (var name in _compNames)
                comboBoxName.Items.Add(name);

            var wireDev1 = new CtsAttributes("WFTD", "YES");
            comboBoxWireDev.Items.Add(wireDev1);
            var wireDev2 = new CtsAttributes("WFTD", "NO");
            comboBoxWireDev.Items.Add(wireDev2);

            var wireTaper1 = new CtsAttributes("WTN", "YES");
            comboBoxWireTaper.Items.Add(wireTaper1);
            var wireTaper2 = new CtsAttributes("WTN", "NO");
            comboBoxWireTaper.Items.Add(wireTaper2);

            var dieset1 = new CtsAttributes("DIESET NOTE", "YES");
            comboBoxDieset.Items.Add(dieset1);
            var dieset2 = new CtsAttributes("DIESET NOTE", "NO");
            comboBoxDieset.Items.Add(dieset2);

            var weldment1 = new CtsAttributes("WELDMENT NOTE", "YES");
            comboBoxWeldment.Items.Add(weldment1);
            var weldment2 = new CtsAttributes("WELDMENT NOTE", "NO");
            comboBoxWeldment.Items.Add(weldment2);
        }

        private void UpdateCompExpressions()
        {
            Session.UndoMarkId updateExpressions;
            updateExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

            // get named expressions

            var isNamedExpression = false;

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

            foreach (var exp in _workPart.Expressions.ToArray())
            {
                if(exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    AddX = exp;
                    xValue = exp.Value;
                }

                if(exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    AddY = exp;
                    yValue = exp.Value;
                }

                if(exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    AddZ = exp;
                    zValue = exp.Value;
                }

                if(exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    BurnDir = exp;
                    burnDirValue = exp.RightHandSide;
                }

                if(exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    Burnout = exp;
                    burnoutValue = exp.RightHandSide;
                }

                if(exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    Grind = exp;
                    grindValue = exp.RightHandSide;
                }

                if(exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    GrindTolerance = exp;
                    grindTolValue = exp.RightHandSide;
                }
            }

            if(isNamedExpression)
            {
                if(comboBoxAddx.SelectedIndex >= 0)
                    AddX.RightHandSide = comboBoxAddx.Text;

                if(comboBoxAddy.SelectedIndex >= 0)
                    AddY.RightHandSide = comboBoxAddy.Text;

                if(comboBoxAddz.SelectedIndex >= 0)
                    AddZ.RightHandSide = comboBoxAddz.Text;

                if(checkBoxBurnout.Checked)
                    Burnout.RightHandSide = "\"" + "yes" + "\"";
                else
                    Burnout.RightHandSide = "\"" + "no" + "\"";
                if(checkBoxGrind.Checked)
                {
                    Grind.RightHandSide = "\"" + "yes" + "\"";

                    if(comboBoxTolerance.SelectedIndex != -1)
                        GrindTolerance.RightHandSide = "\"" + comboBoxTolerance.Text + "\"";
                    else
                        GrindTolerance.RightHandSide = "\"" + "none" + "\"";
                }
                else
                {
                    Grind.RightHandSide = "\"" + "no" + "\"";

                    GrindTolerance.RightHandSide = "\"" + "none" + "\"";
                }

                if(checkBoxBurnDirX.Checked)
                    BurnDir.RightHandSide = "\"" + "X" + "\"";
                else if(checkBoxBurnDirY.Checked)
                    BurnDir.RightHandSide = "\"" + "Y" + "\"";
                else if(checkBoxBurnDirZ.Checked)
                    BurnDir.RightHandSide = "\"" + "Z" + "\"";
                else
                    BurnDir.RightHandSide = "\"" + "none" + "\"";
                _ = session_.UpdateManager.DoUpdate(updateExpressions);
            }
        }

        private void CreateCompExpressions()
        {
            Session.UndoMarkId makeExpressions;
            makeExpressions = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Expression");

            if(_workPart.PartUnits == BasePart.Units.Inches)
            {
                var unit1 = _workPart.UnitCollection.FindObject("Inch");

                if(comboBoxAddx.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddX=" + comboBoxAddx.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                if(comboBoxAddy.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddY=" + comboBoxAddy.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                if(comboBoxAddz.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=" + comboBoxAddz.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
            }
            else
            {
                var unit1 = _workPart.UnitCollection.FindObject("MilliMeter");

                if(comboBoxAddx.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddX=" + comboBoxAddx.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddX=.000", unit1);
                if(comboBoxAddy.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddY=" + comboBoxAddy.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddY=.000", unit1);
                if(comboBoxAddz.SelectedIndex > 0)
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=" + comboBoxAddz.Text, unit1);
                else
                    _ = _workPart.Expressions.CreateWithUnits("AddZ=.000", unit1);
            }

            if(checkBoxBurnout.Checked)
                _ = _workPart.Expressions.CreateExpression("String", "Burnout=\"yes\"");
            else
                _ = _workPart.Expressions.CreateExpression("String", "Burnout=\"no\"");
            if(checkBoxGrind.Checked)
            {
                _ = _workPart.Expressions.CreateExpression("String", "Grind=\"yes\"");

                if(comboBoxTolerance.SelectedIndex != -1)
                    _ = _workPart.Expressions.CreateExpression("String",
                        "GrindTolerance=\"" + comboBoxTolerance.Text + "\"");
                else
                    _ = _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
            }
            else
            {
                _ = _workPart.Expressions.CreateExpression("String", "Grind=\"no\"");
                _ = _workPart.Expressions.CreateExpression("String", "GrindTolerance=\"none\"");
            }

            if(checkBoxBurnDirX.Checked)
                _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"X\"");
            else if(checkBoxBurnDirY.Checked)
                _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"Y\"");
            else if(checkBoxBurnDirZ.Checked)
                _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"Z\"");
            else
                _ = _workPart.Expressions.CreateExpression("String", "BurnDir=\"none\"");

            _ = session_.UpdateManager.DoUpdate(makeExpressions);
        }

        private void MeasureComponentBody()
        {
            try
            {
                if(_selComp != null)
                    session_.Parts.SetWork((Part)_selComp.Prototype);

                var tempCsys = __display_part_.WCS.Save();

                var isMetric = false;

                if(_workPart.PartUnits == BasePart.Units.Millimeters)
                    isMetric = true;
                if(tempCsys != null)
                {
                    // get named expressions

                    var isNamedExpression = false;

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

                    foreach (var exp in _workPart.Expressions.ToArray())
                    {
                        if(exp.Name == "AddX")
                        {
                            isNamedExpression = true;
                            AddX = exp;
                            xValue = exp.Value;
                        }

                        if(exp.Name == "AddY")
                        {
                            isNamedExpression = true;
                            AddY = exp;
                            yValue = exp.Value;
                        }

                        if(exp.Name == "AddZ")
                        {
                            isNamedExpression = true;
                            AddZ = exp;
                            zValue = exp.Value;
                        }

                        if(exp.Name == "BurnDir")
                        {
                            isNamedExpression = true;
                            BurnDir = exp;
                            burnDirValue = exp.RightHandSide;
                        }

                        if(exp.Name == "Burnout")
                        {
                            isNamedExpression = true;
                            Burnout = exp;
                            burnoutValue = exp.RightHandSide;
                        }

                        if(exp.Name == "Grind")
                        {
                            isNamedExpression = true;
                            Grind = exp;
                            grindValue = exp.RightHandSide;
                        }

                        if(exp.Name == "GrindTolerance")
                        {
                            isNamedExpression = true;
                            GrindTolerance = exp;
                            grindTolValue = exp.RightHandSide;
                        }

                        if(exp.Name == "DiesetNote")
                        {
                            Dieset = exp;
                            diesetValue = exp.RightHandSide;
                        }
                    }

                    burnDirValue = burnDirValue.Replace("\"", string.Empty);
                    burnoutValue = burnoutValue.Replace("\"", string.Empty);
                    grindValue = grindValue.Replace("\"", string.Empty);
                    grindTolValue = grindTolValue.Replace("\"", string.Empty);
                    diesetValue = diesetValue.Replace("\"", string.Empty);

                    if(isNamedExpression)
                    {
                        // get bounding box of solid body

                        var minCorner = new double[3];
                        var directions = new double[3, 3];
                        var distances = new double[3];
                        var grindDistances = new double[3];

                        ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                            distances);
                        ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                            grindDistances);

                        // add stock values

                        distances[0] += xValue;
                        distances[1] += yValue;
                        distances[2] += zValue;

                        if(isMetric)
                            for (var i = 0; i < distances.Length; i++)
                                distances[i] /= 25.4d;

                        if(burnoutValue.ToLower() == "no")
                            for (var i = 0; i < 3; i++)
                            {
                                var roundValue = System.Math.Round(distances[i], 3);
                                var truncateValue = System.Math.Truncate(roundValue);
                                var fractionValue = roundValue - truncateValue;
                                if(fractionValue != 0)
                                    for (var ii = .125; ii <= 1; ii += .125)
                                    {
                                        if(fractionValue <= ii)
                                        {
                                            var roundedFraction = ii;
                                            var finalValue = truncateValue + roundedFraction;
                                            distances[i] = finalValue;
                                            break;
                                        }
                                    }
                                else
                                    distances[i] = roundValue;
                            }

                        var xDist = distances[0];
                        var yDist = distances[1];
                        var zDist = distances[2];

                        var xGrindDist = grindDistances[0];
                        var yGrindDist = grindDistances[1];
                        var zGrindDist = grindDistances[2];

                        Array.Sort(distances);
                        Array.Sort(grindDistances);

                        if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                        {
                            _workPart.__SetAttribute("DESCRIPTION",
                                string.Format("{0:f2}", distances[0]) + " X " + string.Format("{0:f2}", distances[1]) +
                                " X " +
                                string.Format("{0:f2}", distances[2]));
                        }
                        else if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                        {
                            if(burnDirValue.ToLower() == "x")
                            {
                                if(xGrindDist == grindDistances[0])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f3}", grindDistances[0]) + " " + grindTolValue + " X " +
                                        string.Format("{0:f2}", distances[1]) +
                                        " X " +
                                        string.Format("{0:f2}", distances[2]));
                                if(xGrindDist == grindDistances[1])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f3}", grindDistances[1]) + " " + grindTolValue +
                                        " X " +
                                        string.Format("{0:f2}", distances[2]));
                                if(xGrindDist == grindDistances[2])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f2}", distances[1]) + " X " +
                                        string.Format("{0:f3}", grindDistances[2]) + " " + grindTolValue);
                            }

                            if(burnDirValue.ToLower() == "y")
                            {
                                if(yGrindDist == grindDistances[0])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        $"{string.Format("{0:f3}", grindDistances[0])} {grindTolValue} X {string.Format("{0:f2}", distances[1])} X {string.Format("{0:f2}", distances[2])}");
                                if(yGrindDist == grindDistances[1])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f3}", grindDistances[1]) + " " + grindTolValue +
                                        " X " +
                                        string.Format("{0:f2}", distances[2]));
                                if(yGrindDist == grindDistances[2])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f2}", distances[1]) + " X " +
                                        string.Format("{0:f3}", grindDistances[2]) + " " + grindTolValue);
                            }

                            if(burnDirValue.ToLower() == "z")
                            {
                                if(zGrindDist == grindDistances[0])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f3}", grindDistances[0]) + " " + grindTolValue + " X " +
                                        string.Format("{0:f2}", distances[1]) +
                                        " X " +
                                        string.Format("{0:f2}", distances[2]));
                                if(zGrindDist == grindDistances[1])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f3}", grindDistances[1]) + " " + grindTolValue +
                                        " X " +
                                        string.Format("{0:f2}", distances[2]));
                                if(zGrindDist == grindDistances[2])
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        string.Format("{0:f2}", distances[0]) + " X " +
                                        string.Format("{0:f2}", distances[1]) + " X " +
                                        string.Format("{0:f3}", grindDistances[2]) + " " + grindTolValue);
                            }
                        }
                        else
                        {
                            if(grindValue.ToLower() == "yes")
                            {
                                if(burnDirValue.ToLower() == "x")
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        "BURN " + string.Format("{0:f3}", xGrindDist) + " " + grindTolValue);
                                if(burnDirValue.ToLower() == "y")
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        "BURN " + string.Format("{0:f3}", yGrindDist) + " " + grindTolValue);
                                if(burnDirValue.ToLower() == "z")
                                    _workPart.__SetAttribute("DESCRIPTION",
                                        "BURN " + string.Format("{0:f3}", zGrindDist) + " " + grindTolValue);
                            }
                            else
                            {
                                if(burnDirValue.ToLower() == "x")
                                    _workPart.__SetAttribute("DESCRIPTION", $"BURN {"{xDist:f2}"}");
                                if(burnDirValue.ToLower() == "y")
                                    _workPart.__SetAttribute("DESCRIPTION", "BURN " + string.Format("{0:f2}", yDist));
                                if(burnDirValue.ToLower() == "z")
                                    _workPart.__SetAttribute("DESCRIPTION", "BURN " + string.Format("{0:f2}", zDist));
                            }
                        }

                        if(diesetValue == "yes")
                        {
                            var description = _workPart.__GetStringAttribute("DESCRIPTION");

                            if(!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                    }
                    else
                    {
                        // get bounding box of solid body

                        var minCorner = new double[3];
                        var directions = new double[3, 3];
                        var distances = new double[3];

                        ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, tempCsys.Tag, minCorner, directions,
                            distances);

                        if(isMetric)
                            for (var i = 0; i < distances.Length; i++)
                                distances[i] /= 25.4d;

                        for (var i = 0; i < 3; i++)
                        {
                            var roundValue = System.Math.Round(distances[i], 3);
                            var truncateValue = System.Math.Truncate(roundValue);
                            var fractionValue = roundValue - truncateValue;
                            if(fractionValue != 0)
                                for (var ii = .125; ii <= 1; ii += .125)
                                {
                                    if(fractionValue <= ii)
                                    {
                                        var roundedFraction = ii;
                                        var finalValue = truncateValue + roundedFraction;
                                        distances[i] = finalValue;
                                        break;
                                    }
                                }
                            else
                                distances[i] = roundValue;
                        }

                        // Create the description attribute

                        Array.Sort(distances);

                        _workPart.__SetAttribute("DESCRIPTION",
                            string.Format("{0:f2}", distances[0]) + " X " + string.Format("{0:f2}", distances[1]) +
                            " X " +
                            string.Format("{0:f2}", distances[2]));

                        if(diesetValue == "yes")
                        {
                            var description = _workPart.__GetStringAttribute("DESCRIPTION");

                            if(!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                    }

                    var delObj = new List<NXObject>();
                    delObj.Add(tempCsys);
                    DeleteNxObjects(delObj);

                    UpdateCompAttributes();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();

                    if(_selComp == null)
                        __display_part_.Views.Regenerate();

                    session_.Parts.SetDisplay(_originalDisplayPart, false, false, out var setDispLoadStatus1);
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
            catch (NXException ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                TheUi.NXMessageBox.Show("Caught exception : Measure", NXMessageBox.DialogType.Error, ex.Message);
            }
        }

        private void UpdateBlockDescription()
        {
            try
            {
                //UpdateSessionParts();

                var isMetric = false;

                BasePart basePart = _workPart;
                var partUnits = basePart.PartUnits;

                if(partUnits == BasePart.Units.Millimeters)
                    isMetric = true;


                foreach (Feature featDynamic in _workPart.Features)
                {
                    if(featDynamic.FeatureType != "BLOCK") continue;
                    if(featDynamic.Name != "DYNAMIC BLOCK") continue;
                    var block1 = (Block)featDynamic;
                    var sizeBody = block1.GetBodies();

                    BlockFeatureBuilder blockFeatureBuilderSize;
                    blockFeatureBuilderSize = _workPart.Features.CreateBlockFeatureBuilder(block1);

                    blockFeatureBuilderSize.GetOrientation(out var xAxis, out var yAxis);

                    double[] initOrigin =
                    {
                        blockFeatureBuilderSize.Origin.X, blockFeatureBuilderSize.Origin.Y,
                        blockFeatureBuilderSize.Origin.Z
                    };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    var initMatrix = new double[9];
                    var tempCsys = NXOpen.Tag.Null;
                    ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                    ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                    ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out tempCsys);

                    if(tempCsys != NXOpen.Tag.Null)
                    {
                        // get named expressions

                        var isNamedExpression = false;

                        double xValue = 0,
                            yValue = 0,
                            zValue = 0;

                        string burnDirValue = string.Empty,
                            burnoutValue = string.Empty,
                            grindValue = string.Empty,
                            grindTolValue = string.Empty,
                            diesetValue = string.Empty;

                        foreach (var exp in _workPart.Expressions.ToArray())
                        {
                            if(exp.Name == "AddX")
                            {
                                isNamedExpression = true;
                                xValue = exp.Value;
                            }

                            if(exp.Name == "AddY")
                            {
                                isNamedExpression = true;
                                yValue = exp.Value;
                            }

                            if(exp.Name == "AddZ")
                            {
                                isNamedExpression = true;
                                zValue = exp.Value;
                            }

                            if(exp.Name == "BurnDir")
                            {
                                isNamedExpression = true;
                                burnDirValue = exp.RightHandSide;
                            }

                            if(exp.Name == "Burnout")
                            {
                                isNamedExpression = true;
                                burnoutValue = exp.RightHandSide;
                            }

                            if(exp.Name == "Grind")
                            {
                                isNamedExpression = true;
                                grindValue = exp.RightHandSide;
                            }

                            if(exp.Name == "GrindTolerance")
                            {
                                isNamedExpression = true;
                                grindTolValue = exp.RightHandSide;
                            }

                            if(exp.Name == "DiesetNote") diesetValue = exp.RightHandSide;
                        }

                        burnDirValue = burnDirValue.Replace("\"", string.Empty);
                        burnoutValue = burnoutValue.Replace("\"", string.Empty);
                        grindValue = grindValue.Replace("\"", string.Empty);
                        grindTolValue = grindTolValue.Replace("\"", string.Empty);
                        diesetValue = diesetValue.Replace("\"", string.Empty);

                        if(isNamedExpression)
                        {
                            // get bounding box of solid body

                            var minCorner = new double[3];
                            var directions = new double[3, 3];
                            var distances = new double[3];
                            var grindDistances = new double[3];

                            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                distances);
                            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                grindDistances);

                            // add stock values

                            distances[0] += xValue;
                            distances[1] += yValue;
                            distances[2] += zValue;

                            var trueX = distances[0];
                            var trueY = distances[1];
                            var trueZ = distances[2];

                            if(isMetric)
                                for (var i = 0; i < distances.Length; i++)
                                    distances[i] /= 25.4d;

                            if(burnoutValue.ToLower() == "no")
                                for (var i = 0; i < 3; i++)
                                {
                                    var roundValue = System.Math.Round(distances[i], 3);
                                    var truncateValue = System.Math.Truncate(roundValue);
                                    var fractionValue = roundValue - truncateValue;
                                    if(fractionValue != 0)
                                        for (var ii = .125; ii <= 1; ii += .125)
                                        {
                                            if(fractionValue <= ii)
                                            {
                                                var roundedFraction = ii;
                                                var finalValue = truncateValue + roundedFraction;
                                                distances[i] = finalValue;
                                                break;
                                            }
                                        }
                                    else
                                        distances[i] = roundValue;
                                }

                            var xDist = distances[0];
                            var yDist = distances[1];
                            var zDist = distances[2];

                            var xGrindDist = grindDistances[0];
                            var yGrindDist = grindDistances[1];
                            var zGrindDist = grindDistances[2];

                            Array.Sort(distances);
                            Array.Sort(grindDistances);

                            var text = (CtsAttributes)comboBoxTolerance.SelectedItem;
                            if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                            {
                                _workPart.SetUserAttribute("DESCRIPTION", -1,
                                    $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}",
                                    NXOpen.Update.Option.Now);
                            }
                            else if(text != null && text.AttrValue.ToLower().Contains("cleanup"))
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
                            else if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                            {
                                const double tolerance = .001;

                                // ReSharper disable once SwitchStatementMissingSomeCases
                                switch (burnDirValue.ToLower())
                                {
                                    case "x":
                                        if(System.Math.Abs(xGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(xGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(xGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
                                        break;
                                    case "y":
                                        if(System.Math.Abs(yGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(yGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(yGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
                                        break;
                                    case "z":
                                        if(System.Math.Abs(zGrindDist - grindDistances[0]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(zGrindDist - grindDistances[1]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                                NXOpen.Update.Option.Now);
                                        if(System.Math.Abs(zGrindDist - grindDistances[2]) < tolerance)
                                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                                $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                                NXOpen.Update.Option.Now);
                                        break;
                                }
                            }
                            else
                            {
                                if(grindValue.ToLower() == "yes")
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

                            if(diesetValue != "yes") continue;
                            var description =
                                _workPart.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);
                            //description += " DIESET";
                            _workPart.SetUserAttribute("DESCRIPTION", -1, $"{description} DIESET",
                                NXOpen.Update.Option.Now);
                        }
                        else
                        {
                            // get bounding box of solid body

                            var minCorner = new double[3];
                            var directions = new double[3, 3];
                            var distances = new double[3];

                            ufsession_.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                distances);

                            if(isMetric)
                                for (var i = 0; i < distances.Length; i++)
                                    distances[i] /= 25.4d;

                            for (var i = 0; i < 3; i++)
                            {
                                var roundValue = System.Math.Round(distances[i], 3);
                                var truncateValue = System.Math.Truncate(roundValue);
                                var fractionValue = roundValue - truncateValue;
                                if(fractionValue != 0)
                                    for (var ii = .125; ii <= 1; ii += .125)
                                    {
                                        if(fractionValue <= ii)
                                        {
                                            var roundedFraction = ii;
                                            var finalValue = truncateValue + roundedFraction;
                                            distances[i] = finalValue;
                                            break;
                                        }
                                    }
                                else
                                    distances[i] = roundValue;
                            }

                            // Create the description attribute

                            Array.Sort(distances);


                            _workPart.SetUserAttribute("DESCRIPTION", -1,
                                $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}", NXOpen.Update.Option.Now);
                            //_workPart.__SetAttribute($"DESCRIPTION", $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}");

                            if(diesetValue == "yes")
                            {
                                var description = _workPart.GetUserAttributeAsString("DESCRIPTION",
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
                if(!_workPart.HasUserAttribute("DESCRIPTION", NXObject.AttributeType.String, -1)) return;

                // The string value of the {"DESCRIPTION"} attribute.
                var descriptionAtt =
                    _workPart.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);

                var expressions = _workPart.Expressions.ToArray();

                // Checks to see if the {_workPart} contains an expression with value {"yes"} and name of {lwrParallel} or {uprParallel}.
                if(expressions.Any(exp =>
                       (exp.Name.ToLower() == "lwrparallel" || exp.Name.ToLower() == "uprparallel") &&
                       exp.StringValue.ToLower() == "yes"))
                    // Appends {"Parallel"} to the end of the {"DESCRIPTION"} attribute string value and then sets the it to be the value of the {"DESCRIPTION"} attribute.
                    _workPart.SetUserAttribute("DESCRIPTION", -1, descriptionAtt + " PARALLEL",
                        NXOpen.Update.Option.Now);
            }
            catch (NXException ex)
            {
                TheUi.NXMessageBox.Show("Update Block Description", NXMessageBox.DialogType.Error, ex.Message);
            }
        }

        private double AskSteelSize(double distance, BasePart part)
        {
            //            if (part.Leaf.Contains("200"))
            //                Debugger.Launch();
            var roundValue = System.Math.Round(distance, 3);
            var truncateValue = System.Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;

            // If it doesn't seem to be working you might have any issue with metric vs english,
            // or you can revert the code back to the orignal line before you changed to float-point comparison.
            if(System.Math.Abs(fractionValue) > .001)
            {
                for (var ii = .125; ii <= 1; ii += .125)
                    if(fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        return finalValue;
                    }
            }
            else
            {
                return roundValue;
            }

            throw new Exception($"Ask Steel Size, Part: {part.Leaf}. {nameof(distance)}: {distance}");
        }


        private void UpdateCompAttributes()
        {
            if(_isCustom)
            {
                if(_selComp != null)
                {
                    var selCompProto = (Part)_selComp.Prototype;

                    if(textBoxDescription.Text != "")
                    {
                        selCompProto.__SetAttribute("DESCRIPTION", textBoxDescription.Text);

                        using (selCompProto.__UsingSetWorkPartQuietly())
                        {
                            UpdateSessionParts();
                            var isBlockComp = false;

                            if(_selComp != null)
                            {
                                var compProto = (Part)_selComp.Prototype;

                                foreach (Feature featDynamic in compProto.Features)
                                    if(featDynamic.FeatureType == "BLOCK")
                                        if(featDynamic.Name == "DYNAMIC BLOCK")
                                            isBlockComp = true;

                                if(isBlockComp)
                                    TheUi.NXMessageBox.Show("Custom Description", NXMessageBox.DialogType.Warning,
                                        "Block component = Auto Update Off");
                            }

                            SetAutoUpdateOff();
                        }

                        UpdateSessionParts();
                    }

                    if(textBoxMaterial.Text != "")
                    {
                        selCompProto.__SetAttribute("MATERIAL", textBoxMaterial.Text);
                    }
                    else
                    {
                        if(comboBoxPurMaterials.Text != "")
                            selCompProto.__SetAttribute("MATERIAL", comboBoxPurMaterials.Text);
                        if(comboBoxCustomMaterials.Text != "")
                            selCompProto.__SetAttribute("MATERIAL", comboBoxCustomMaterials.Text);
                    }

                    if(comboBoxMaterial.Text != "")
                        selCompProto.__SetAttribute("MATERIAL", comboBoxMaterial.Text);

                    if(comboBoxName.Text != "")
                        selCompProto.__SetAttribute("DETAIL NAME", comboBoxName.Text);
                    if(comboBoxWireTaper.Text != "")
                        selCompProto.__SetAttribute("WTN", comboBoxWireTaper.Text);
                    if(comboBoxWireDev.Text != "")
                        selCompProto.__SetAttribute("WFTD", comboBoxWireDev.Text);
                    if(comboBoxDieset.Text != "")
                        if(comboBoxDieset.Text == "YES")
                        {
                            var prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Dieset = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if(Dieset != null)
                                Dieset.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            var description = selCompProto.__GetStringAttribute("DESCRIPTION");

                            if(!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                selCompProto.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                        else
                        {
                            var prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Dieset = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if(Dieset != null)
                                Dieset.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            var description = selCompProto.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("DIESET", "");
                            selCompProto.__SetAttribute("DESCRIPTION", description);
                        }

                    //Add Weldment stuff
                    if(comboBoxWeldment.Text != "")
                        if(comboBoxWeldment.Text == "YES")
                        {
                            var prevWp = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
                            ufsession_.Assem.SetWorkPartQuietly(selCompProto.Tag, out prevWp);
#pragma warning restore CS0618 // Type or member is obsolete
                            UpdateSessionParts();

                            Expression Weldment = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if(Weldment != null)
                                Weldment.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

                            ufsession_.Assem.SetWorkPart(prevWp);
                            UpdateSessionParts();

                            var description = selCompProto.__GetStringAttribute("DESCRIPTION");

                            if(!description.ToLower().Contains("weldment"))
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
                            ufsession_.Assem.SetWorkPartContextQuietly(selCompProto.Tag, out var prevWp);
                            UpdateSessionParts();

                            Expression Weldment = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if(Weldment != null)
                                Weldment.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

                            ufsession_.Assem.RestoreWorkPartContextQuietly(ref prevWp);
                            UpdateSessionParts();

                            var description = selCompProto.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("WELDMENT", "");
                            selCompProto.__SetAttribute("DESCRIPTION", description);
                        }
                    //End of add Weldment stuff
                }
                else
                {
                    if(textBoxDescription.Text != "")
                    {
                        _workPart.__SetAttribute("DESCRIPTION", textBoxDescription.Text);
                        SetAutoUpdateOff();
                    }

                    if(textBoxMaterial.Text != "")
                    {
                        _workPart.__SetAttribute("MATERIAL", textBoxMaterial.Text);
                    }
                    else
                    {
                        if(comboBoxPurMaterials.Text != "")
                            _workPart.__SetAttribute("MATERIAL", comboBoxPurMaterials.Text);
                        if(comboBoxCustomMaterials.Text != "")
                            _workPart.__SetAttribute("MATERIAL", comboBoxCustomMaterials.Text);
                    }

                    if(comboBoxName.Text != "")
                        _workPart.__SetAttribute("DETAIL NAME", comboBoxName.Text);
                    if(comboBoxWireTaper.Text != "")
                        _workPart.__SetAttribute("WTN", comboBoxWireTaper.Text);
                    if(comboBoxWireDev.Text != "")
                        _workPart.__SetAttribute("WFTD", comboBoxWireDev.Text);
                    if(comboBoxDieset.Text != "")
                        if(comboBoxDieset.Text == "YES")
                        {
                            Expression Dieset = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if(Dieset != null)
                                Dieset.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

                            var description = _workPart.__GetStringAttribute("DESCRIPTION");
                            if(!description.ToLower().Contains("dieset"))
                            {
                                description += " DIESET";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                        else
                        {
                            Expression Dieset = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "DiesetNote")
                                    Dieset = exp;

                            if(Dieset != null)
                                Dieset.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

                            var description = _workPart.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("DIESET", "");
                            _workPart.__SetAttribute("DESCRIPTION", description);
                        }

                    //Add more Weldment Stuff
                    if(comboBoxWeldment.Text != "")
                        if(comboBoxWeldment.Text == "YES")
                        {
                            Expression Weldment = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if(Weldment != null)
                                Weldment.RightHandSide = "\"yes\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

                            var description = _workPart.__GetStringAttribute("DESCRIPTION");
                            if(!description.ToLower().Contains("weldment"))
                            {
                                description += " WELDMENT";
                                _workPart.__SetAttribute("DESCRIPTION", description);
                            }
                        }
                        else
                        {
                            Expression Weldment = null;

                            foreach (var exp in _workPart.Expressions.ToArray())
                                if(exp.Name == "WeldmentNote")
                                    Weldment = exp;

                            if(Weldment != null)
                                Weldment.RightHandSide = "\"no\"";
                            else
                                _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

                            var description = _workPart.__GetStringAttribute("DESCRIPTION");
                            description = description.Replace("WELDMENT", "");
                            _workPart.__SetAttribute("DESCRIPTION", description);
                        }
                    //End of more Weldment stuff
                }
            }
            else
            {
                if(comboBoxMaterial.Text != "")
                    _workPart.__SetAttribute("MATERIAL", comboBoxMaterial.Text);
                if(comboBoxName.Text != "")
                    _workPart.__SetAttribute("DETAIL NAME", comboBoxName.Text);
                if(comboBoxWireTaper.Text != "")
                    _workPart.__SetAttribute("WTN", comboBoxWireTaper.Text);
                if(comboBoxWireDev.Text != "")
                    _workPart.__SetAttribute("WFTD", comboBoxWireDev.Text);
                if(comboBoxDieset.Text != "")
                    if(comboBoxDieset.Text == "YES")
                    {
                        Expression Dieset = null;

                        foreach (var exp in _workPart.Expressions.ToArray())
                            if(exp.Name == "DiesetNote")
                                Dieset = exp;

                        if(Dieset != null)
                            Dieset.RightHandSide = "\"yes\"";
                        else
                            _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"yes\"");

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");

                        if(!description.ToLower().Contains("dieset"))
                        {
                            description += " DIESET";
                            _workPart.__SetAttribute("DESCRIPTION", description);
                        }
                    }
                    else
                    {
                        Expression Dieset = null;

                        foreach (var exp in _workPart.Expressions.ToArray())
                            if(exp.Name == "DiesetNote")
                                Dieset = exp;

                        if(Dieset != null)
                            Dieset.RightHandSide = "\"no\"";
                        else
                            _ = _workPart.Expressions.CreateExpression("String", "DiesetNote=\"no\"");

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");
                        description = description.Replace("DIESET", "");
                        _workPart.__SetAttribute("DESCRIPTION", description);
                    }

                //Add Weldment stuff
                if(comboBoxWeldment.Text != "")
                    if(comboBoxWeldment.Text == "YES")
                    {
                        Expression Weldment = null;

                        foreach (var exp in _workPart.Expressions.ToArray())
                            if(exp.Name == "WeldmentNote")
                                Weldment = exp;

                        if(Weldment != null)
                            Weldment.RightHandSide = "\"yes\"";
                        else
                            _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"yes\"");

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");

                        if(!description.ToLower().Contains("weldment"))
                        {
                            description += " WELDMENT";
                            _workPart.__SetAttribute("DESCRIPTION", description);
                        }
                    }
                    else
                    {
                        Expression Weldment = null;

                        foreach (var exp in _workPart.Expressions.ToArray())
                            if(exp.Name == "WeldmentNote")
                                Weldment = exp;

                        if(Weldment != null)
                            Weldment.RightHandSide = "\"no\"";
                        else
                            _ = _workPart.Expressions.CreateExpression("String", "WeldmentNote=\"no\"");

                        var description = _workPart.__GetStringAttribute("DESCRIPTION");
                        description = description.Replace("WELDMENT", "");
                        _workPart.__SetAttribute("DESCRIPTION", description);
                    }
                //End of add weldment stuff
            }
        }

        private void SetAutoUpdateOff()
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                var myUDOclass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");

                if(myUDOclass != null)
                {
                    UserDefinedObject[] currentUdo;
                    currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUDOclass);

                    if(currentUdo.Length == 1)
                    {
                        var myUDO = currentUdo[0];

                        var updateFlag = myUDO.GetIntegers();

                        int[] updateOff = { 0 };
                        myUDO.SetIntegers(updateOff);
                        _workPart.__SetAttribute("AUTO UPDATE", "OFF");
                    }
                }
            }
            catch (NXException ex)
            {
                UI.GetUI().NXMessageBox.Show("Caught exception : Auto Update Off", NXMessageBox.DialogType.Error,
                    ex.Message);
            }
        }

        private Component SelectOneComponent()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;
            sel = TheUi.SelectionManager.SelectTaggedObject("Select component to get attributes", "Select Component",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, true, mask, out var selectedComp, out _);

            if(sel == Selection.Response.Ok || sel == Selection.Response.ObjectSelected ||
               sel == Selection.Response.ObjectSelectedByName)
                compSelection = (Component)selectedComp;

            return compSelection;
        }

        private Body SelectOneComponentBody()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);
            Selection.Response sel;
            Body returnBody = null;
            sel = TheUi.SelectionManager.SelectTaggedObject("Select Body", "Select Body",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedBody, out _);

            if(sel == Selection.Response.Ok || sel == Selection.Response.ObjectSelected ||
               sel == Selection.Response.ObjectSelectedByName)
                returnBody = (Body)selectedBody;

            return returnBody;
        }

        private List<Component> SelectMultipleComponents()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            var compsSelection = new List<Component>();

            sel = TheUi.SelectionManager.SelectTaggedObjects("Select Components", "Select Components",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, true, mask, out var selectedCompArray);

            if(sel == Selection.Response.Ok)
                foreach (var comp in selectedCompArray)
                {
                    var component = (Component)comp;
                    compsSelection.Add(component);
                }

            return compsSelection;
        }

        private List<Component> GetOneComponentOfMany(List<Component> compList)
        {
            var oneComp = new List<Component>();

            oneComp.Add(compList[0]);

            foreach (var comp in compList)
            {
                var foundComponent = oneComp.Find(delegate(Component c) { return c.DisplayName == comp.DisplayName; });
                if(foundComponent == null)
                    oneComp.Add(comp);
            }

            if(oneComp.Count != 0)
                return oneComp;
            return null;
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

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                                __display_part_.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                                    setTempCsys.Orientation.Element);

                                var featBlkCsys = __display_part_.WCS.Save();
                                featBlkCsys.SetName("EDITCSYS");
                                featBlkCsys.Layer = 254;

                                NXObject[] addToBody = { featBlkCsys };

                                foreach (var bRefSet in __display_part_.GetAllReferenceSets())
                                    if(bRefSet.Name == "BODY")
                                        bRefSet.AddObjectsToReferenceSet(addToBody);

                                session_.Parts.SetDisplay(_originalDisplayPart, false, false,
                                    out var setDispLoadStatus1);
                                setDispLoadStatus1.Dispose();

                                session_.Parts.SetWorkComponent(compRefCsys, PartCollection.RefsetOption.Current,
                                    PartCollection.WorkComponentOption.Given,
                                    out var partLoadStatusWorkComp);
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

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

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

        private Point3d MapWorkCoordsToWcs(Point3d pointToMap)
        {
            Point3d mappedPoint;
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            var output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            mappedPoint.X = output[0];
            mappedPoint.Y = output[1];
            mappedPoint.Z = output[2];
            return mappedPoint;
        }

        private void ShowTemporarySizeText(double length, Point3d start, Point3d end)
        {
            var view = __display_part_.Views.WorkView.Tag;
            var viewType = UFDisp.ViewType.UseWorkView;
            var dim = string.Empty;

            if(__display_part_.PartUnits == BasePart.Units.Inches)
            {
                var roundDim = System.Math.Round(length, 3);
                dim = string.Format("{0:0.000}", roundDim);
            }
            else
            {
                var roundDim = System.Math.Round(length, 3);
                dim = string.Format("{0:0.000}", roundDim / 25.4);
            }

            var midPoint = new double[3];
            var dispProps = new UFObj.DispProps();
            dispProps.color = 31;
            double charSize;
            var font = 1;

            if(__display_part_.PartUnits == BasePart.Units.Inches)
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
            var prevWork = NXOpen.Tag.Null;
#pragma warning disable CS0618 // Type or member is obsolete
            ufsession_.Assem.SetWorkPartQuietly(__display_part_.Tag, out prevWork);
#pragma warning restore CS0618 // Type or member is obsolete

            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);

            var dispProps = new UFObj.DispProps();
            dispProps.color = 7;
            var lineData1 = new UFCurve.Line();

            var endPointX1 = new Point3d(mappedStartPoint1.X + lineLength, mappedStartPoint1.Y, mappedStartPoint1.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            double[] startX1 = { wcsOrigin.X, wcsOrigin.Y, wcsOrigin.Z };
            double[] endX1 = { mappedEndPointX1.X, mappedEndPointX1.Y, mappedEndPointX1.Z };
            lineData1.start_point = startX1;
            lineData1.end_point = endX1;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);
            ShowTemporarySizeText(lineLength, wcsOrigin, mappedEndPointX1);

            var endPointY1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y + lineWidth, mappedStartPoint1.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            double[] startY1 = { wcsOrigin.X, wcsOrigin.Y, wcsOrigin.Z };
            double[] endY1 = { mappedEndPointY1.X, mappedEndPointY1.Y, mappedEndPointY1.Z };
            lineData1.start_point = startY1;
            lineData1.end_point = endY1;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);
            ShowTemporarySizeText(lineWidth, wcsOrigin, mappedEndPointY1);

            var endPointZ1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y, mappedStartPoint1.Z + lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            double[] startZ1 = { wcsOrigin.X, wcsOrigin.Y, wcsOrigin.Z };
            double[] endZ1 = { mappedEndPointZ1.X, mappedEndPointZ1.Y, mappedEndPointZ1.Z };
            lineData1.start_point = startZ1;
            lineData1.end_point = endZ1;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);
            ShowTemporarySizeText(lineHeight, wcsOrigin, mappedEndPointZ1);

            //==================================================================================================================

            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = new Point3d(mappedStartPoint2.X + lineLength, mappedStartPoint2.Y, mappedStartPoint2.Z);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            double[] startX2 = { mappedEndPointY1.X, mappedEndPointY1.Y, mappedEndPointY1.Z };
            double[] endX2 = { mappedEndPointX2.X, mappedEndPointX2.Y, mappedEndPointX2.Z };
            lineData1.start_point = startX2;
            lineData1.end_point = endX2;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            double[] startY2 = { mappedEndPointX1.X, mappedEndPointX1.Y, mappedEndPointX1.Z };
            double[] endY2 = { mappedEndPointX2.X, mappedEndPointX2.Y, mappedEndPointX2.Z };
            lineData1.start_point = startY2;
            lineData1.end_point = endY2;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            //==================================================================================================================

            var mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);

            var endPointX1Ceiling =
                new Point3d(mappedStartPoint3.X + lineLength, mappedStartPoint3.Y, mappedStartPoint3.Z);
            var mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            double[] startX3 = { mappedEndPointZ1.X, mappedEndPointZ1.Y, mappedEndPointZ1.Z };
            double[] endX3 = { mappedEndPointX1Ceiling.X, mappedEndPointX1Ceiling.Y, mappedEndPointX1Ceiling.Z };
            lineData1.start_point = startX3;
            lineData1.end_point = endX3;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            var endPointY1Ceiling =
                new Point3d(mappedStartPoint3.X, mappedStartPoint3.Y + lineWidth, mappedStartPoint3.Z);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            double[] startY3 = { mappedEndPointZ1.X, mappedEndPointZ1.Y, mappedEndPointZ1.Z };
            double[] endY3 = { mappedEndPointY1Ceiling.X, mappedEndPointY1Ceiling.Y, mappedEndPointY1Ceiling.Z };
            lineData1.start_point = startY3;
            lineData1.end_point = endY3;
            ufsession_.Disp.DisplayTemporaryLine(__display_part_.Views.WorkView.Tag, UFDisp.ViewType.UseWorkView,
                lineData1.start_point, lineData1.end_point,
                ref dispProps);

            //==================================================================================================================

            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);

            var endPointX2Ceiling =
                new Point3d(mappedStartPoint4.X + lineLength, mappedStartPoint4.Y, mappedStartPoint4.Z);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
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

            if(_selComp != null)
            {
                session_.Parts.SetWorkComponent(_selComp, PartCollection.RefsetOption.Current,
                    PartCollection.WorkComponentOption.Given, out var partLoad1);
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
            if(_allowBoundingBox)
            {
                __display_part_.Views.Refresh();

                // get named expressions

                var isNamedExpression = false;

                Expression AddX = null,
                    AddY = null,
                    AddZ = null;

                double xValue = 0,
                    yValue = 0,
                    zValue = 0;

                foreach (var exp in _workPart.Expressions.ToArray())
                {
                    if(exp.Name == "AddX")
                    {
                        isNamedExpression = true;
                        AddX = exp;
                        xValue = exp.Value;
                    }

                    if(exp.Name == "AddY")
                    {
                        isNamedExpression = true;
                        AddY = exp;
                        yValue = exp.Value;
                    }

                    if(exp.Name == "AddZ")
                    {
                        isNamedExpression = true;
                        AddZ = exp;
                        zValue = exp.Value;
                    }
                }

                if(isNamedExpression)
                {
                    _workPart.Expressions.Edit(AddX, comboBoxAddx.Text);
                    xValue = AddX.Value;

                    _workPart.Expressions.Edit(AddY, comboBoxAddy.Text);
                    yValue = AddY.Value;

                    _workPart.Expressions.Edit(AddZ, comboBoxAddz.Text);
                    zValue = AddZ.Value;

                    // get bounding box info

                    var minCorner = new double[3];
                    var directions = new double[3, 3];
                    var distances = new double[3];

                    ufsession_.Modl.AskBoundingBoxExact(_sizeBody.Tag, __display_part_.WCS.CoordinateSystem.Tag,
                        minCorner, directions, distances);

                    // add stock values

                    distances[0] += xValue;
                    distances[1] += yValue;
                    distances[2] += zValue;

                    if(_workPart.PartUnits == BasePart.Units.Millimeters)
                        for (var i = 0; i < distances.Length; i++)
                            distances[i] /= 25.4d;

                    for (var i = 0; i < 3; i++)
                    {
                        var roundValue = System.Math.Round(distances[i], 3);
                        var truncateValue = System.Math.Truncate(roundValue);
                        var fractionValue = roundValue - truncateValue;
                        if(fractionValue != 0)
                            for (var ii = .125; ii <= 1; ii += .125)
                            {
                                if(fractionValue <= ii)
                                {
                                    var roundedFraction = ii;
                                    var finalValue = truncateValue + roundedFraction;
                                    distances[i] = finalValue;
                                    break;
                                }
                            }
                        else
                            distances[i] = roundValue;
                    }

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

        private string PerformStreamReaderString(string path, string startSearchString, string endSearchString)
        {
            try
            {
                var sr = new StreamReader(path);
                var content = sr.ReadToEnd();
                sr.Close();
                string[] startSplit;
                string[] endSplit;
                string textSetting;
                startSplit = Regex.Split(content, startSearchString);
                endSplit = Regex.Split(startSplit[1], endSearchString);
                textSetting = endSplit[0];
                textSetting = textSetting.Replace("\r\n", string.Empty);

                if(textSetting.Length > 0)
                    return textSetting;
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private List<CtsAttributes> PerformStreamReaderList(string path, string startSearchString,
            string endSearchString)
        {
            try
            {
                var sr = new StreamReader(path);
                var content = sr.ReadToEnd();
                sr.Close();
                string[] startSplit;
                string[] endSplit;
                string textData;
                var compData = new List<CtsAttributes>();

                startSplit = Regex.Split(content, startSearchString);
                endSplit = Regex.Split(startSplit[1], endSearchString);
                textData = endSplit[0];

                var splitData = Regex.Split(textData, "\r\n");

                foreach (var sData in splitData)
                    if(sData != string.Empty)
                    {
                        var cData = new CtsAttributes();
                        cData.AttrValue = sData;
                        compData.Add(cData);
                    }

                if(compData.Count > 0)
                    return compData;
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }

    //public static class BlockExtensions
    //{
    //        public static void _SetAttribute(this NXOpen.Part part, string title, string value)
    //        {
    //            part.SetUserAttribute(title, -1, value, NXOpen.Update.Option.Now);
    //        }

    //        public static string _GetStringAttribute(this NXOpen.NXObject part, string title)
    //        {
    //            return part.GetUserAttributeAsString(title, NXOpen.NXObject.AttributeType.String, -1);
    //        }

    //        public static NXOpen.NXObject.AttributeInformation[] _GetAttributeTitlesByType(
    //            this NXOpen.NXObject component,
    //            NXOpen.NXObject.AttributeType att_type)
    //        {
    //#pragma warning disable CS0618 // Type or member is obsolete
    //            return component.GetAttributeTitlesByType(att_type);
    //#pragma warning restore CS0618 // Type or member is obsolete
    //        }


    /*
     *
     *
     * _selComp._GetAttributeTitlesByType(NXOpen.NXObject.AttributeType.String);
     */
    //}
}