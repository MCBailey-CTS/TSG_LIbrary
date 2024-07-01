using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.Drawings;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Ui;
using TSG_Library.Utilities;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions.Extensions;
using Selection = TSG_Library.Ui.Selection;
using View = NXOpen.View;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_cycle_components)]
    public partial class CycleComponentsForm : _UFuncForm
    {
        private static Part _originalDisplayPart = __display_part_;
        private static readonly List<Component> AllComponents = new List<Component>();
        private static List<Component> _selComponents = new List<Component>();
        private static readonly List<string> DisplayName = new List<string>();
        private static List<CtsAttributes> _compMaterials = new List<CtsAttributes>();
        private static bool _isBurnout;
        private static bool _is4View;

        private bool next_last_clicked = true;

        public CycleComponentsForm()
        {
            InitializeComponent();
        }

        private void MainForm1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.cycle_components_form_window_location = Location;
            Settings.Default.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Text = AssemblyFileVersion;
                Location = Settings.Default.cycle_components_form_window_location;
                _compMaterials = CreateMaterialList();
                materialComboBox.Items.AddRange(_compMaterials.ToArray());
                materialSelectButton.Enabled = false;
                materialComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    _originalDisplayPart = __display_part_;
                    _selComponents = new List<Component>();
                    DisplayName?.Clear();
                    selCompListBox.Items.Clear();
                    _selComponents = Selection.SelectManyComponents().ToList();

                    if (_selComponents.Count == 0)
                    {
                        _selComponents = new List<Component>();
                        DisplayName?.Clear();
                        selCompListBox.Items.Clear();
                        return;
                    }

                    foreach (Component component in _selComponents)
                        DisplayName?.Add(component.DisplayName);

                    if (!(DisplayName is null))
                        foreach (string name in DisplayName)
                            selCompListBox.Items.Add(name);

                    selCompListBox.SelectedIndex = 0;
                    string dispComponent = (string)selCompListBox.SelectedItem;

                    foreach (Component component in _selComponents.Where(component => component.DisplayName == dispComponent))
                        SetDisplayPart(component);

                    selectButton.Enabled = false;
                    selectAllButton.Enabled = false;
                    materialComboBox.Enabled = false;
                    materialSelectButton.Enabled = false;
                    burnoutCompButton.Enabled = false;
                    fourViewButton.Enabled = false;
                    buttonNonAssocDimsSymbols.Enabled = false;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    _originalDisplayPart = __display_part_;
                    _selComponents = new List<Component>();
                    AllComponents.Clear();
                    DisplayName.Clear();
                    selCompListBox.Items.Clear();

                    if (__display_part_.ComponentAssembly.RootComponent is null)
                        return;

                    GetChildComponents(__display_part_.ComponentAssembly.RootComponent);

                    if (AllComponents.Count == 0)
                        return;

                    List<CtsAttributes> materials = CreateMaterialList();
                    List<Component> passedComponents = new List<Component>();

                    foreach (Component comp in AllComponents)
                        foreach (NXObject.AttributeInformation attr in comp.GetUserAttributes())
                        {
                            if (attr.Title.ToUpper() != "MATERIAL")
                                continue;

                            string value = comp.GetUserAttributeAsString("MATERIAL", NXObject.AttributeType.String, -1);

                            if (value != "")
                                passedComponents.AddRange(from matAttr in materials
                                                          where matAttr.AttrValue == value
                                                          select comp);
                        }

                    if (passedComponents.Count == 0)
                        return;

                    Component[] selectDeselectComps = passedComponents.ToArray();
                    passedComponents = Preselect.GetUserSelections(selectDeselectComps);

                    if (passedComponents.Count == 0)
                        return;

                    _selComponents = GetOneComponentOfMany(passedComponents);
                    _selComponents.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));

                    foreach (Component compName in _selComponents)
                        DisplayName.Add(compName.DisplayName);

                    selCompListBox.Items.AddRange(DisplayName.ToArray());

                    selCompListBox.SelectedIndex = 0;
                    string dispComponent = (string)selCompListBox.SelectedItem;

                    foreach (Component component in _selComponents.Where(component =>
                                 component.DisplayName == dispComponent))
                        SetDisplayPart(component);

                    selectButton.Enabled = false;
                    selectAllButton.Enabled = false;
                    materialComboBox.Enabled = false;
                    materialSelectButton.Enabled = false;
                    burnoutCompButton.Enabled = false;
                    fourViewButton.Enabled = false;
                    buttonNonAssocDimsSymbols.Enabled = false;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void MaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox.SelectedIndex != -1)
                materialSelectButton.Enabled = true;
        }

        private void MaterialSelectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _originalDisplayPart = __display_part_;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();
                materialComboBox.Enabled = false;

                if (__display_part_.ComponentAssembly.RootComponent is null)
                    return;

                GetChildComponents(__display_part_.ComponentAssembly.RootComponent);

                if (AllComponents.Count == 0)
                    return;

                List<Component> selOneComp = GetOneComponentOfMany(AllComponents);

                foreach (Component comp in
                         from comp in selOneComp
                         from attrAll in comp.GetUserAttributes()
                         where attrAll.Title.ToUpper() == "MATERIAL"
                         let materialType = materialComboBox.Text
                         let attrValue = comp.GetUserAttributeAsString(attrAll.Title, NXObject.AttributeType.String, -1)
                         where attrValue == materialType
                         select comp)
                {
                    _selComponents.Add(comp);
                    DisplayName.Add(comp.DisplayName);
                }

                if (DisplayName.Count != 0)
                {
                    selCompListBox.Items.AddRange(DisplayName.ToArray());
                    selCompListBox.SelectedIndex = 0;
                    string dispComponent = (string)selCompListBox.SelectedItem;

                    foreach (Component component in _selComponents)
                        if (component.DisplayName == dispComponent)
                            SetDisplayPart(component);

                    selectButton.Enabled = false;
                    selectAllButton.Enabled = false;
                    materialSelectButton.Enabled = false;
                    burnoutCompButton.Enabled = false;
                    fourViewButton.Enabled = false;
                    buttonNonAssocDimsSymbols.Enabled = false;
                    return;
                }

                print_($"There are no components with the material type {materialComboBox.Text}");
                selectButton.Enabled = true;
                selectAllButton.Enabled = true;
                materialComboBox.SelectedIndex = -1;
                materialComboBox.Enabled = true;
                materialSelectButton.Enabled = false;
                burnoutCompButton.Enabled = true;
                fourViewButton.Enabled = true;
                buttonNonAssocDimsSymbols.Enabled = true;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static bool AreFullyLoaded(IEnumerable<Component> sourceCompSp)
        {
            return sourceCompSp.Where(component => !component.IsSuppressed).All(__c => __c.Prototype is Part);
        }

        public static bool IsAssemblyLoaded(Component compSp)
        {
            return AreFullyLoaded(compSp.__DescendantsAll());
        }

        private void ButtonBrokenLinks_Click(object sender, EventArgs e)
        {
            try
            {
                if (__work_part_.ComponentAssembly.RootComponent is null)
                {
                    print_("No Assembly in current __work_part_.");
                    return;
                }

                // Revision 2017 – 08 – 16
                if (!IsAssemblyLoaded(__work_part_.ComponentAssembly.RootComponent))
                    print_("Assembly is not fully loaded. Please fully load it to ensure accurate results.");

                _originalDisplayPart = __display_part_;
                _selComponents = new List<Component>();
                selCompListBox.Items.Clear();

                IEnumerable<Component> descendants = __display_part_.ComponentAssembly.RootComponent
                    .__DescendantsAll()
                    .Where(__c => !__c.IsSuppressed);

                HashSet<Component> hashSet = new HashSet<Component>(descendants.Distinct(new EqualityDisplayName()));
                Component[] trimmedComponents = hashSet.Select(component => component).ToArray();

                foreach (Component component in trimmedComponents)
                {
                    if (!(component.Prototype is Part part))
                        continue;

                    foreach (Feature feature in part.Features)
                    {
                        if (!(feature is ExtractFace))
                            continue;

                        ufsession_.Wave.IsLinkBroken(feature.Tag, out bool flag);

                        if (!flag || selCompListBox.Items.Contains(component.DisplayName))
                            continue;

                        selCompListBox.Items.Add(component.DisplayName);
                        _selComponents.Add(component);
                    }
                }

                if (selCompListBox.Items.Count > 0)
                    __display_part_ = (Part)_selComponents[0].Prototype;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void BurnoutCompButton_Click(object sender, EventArgs e)
        {
            try
            {
                _originalDisplayPart = __display_part_;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();
                _isBurnout = true;

                if (__display_part_.ComponentAssembly.RootComponent is null)
                    return;

                GetChildComponents(__display_part_.ComponentAssembly.RootComponent);
                if (AllComponents.Count == 0)
                    return;

                List<Component> selOneComp = GetOneComponentOfMany(AllComponents);
                HashSet<string> materials = new HashSet<string>();
                //const string path = @"U:\NX110\Concept\ConceptControlFile.UCF";
                const string start = ":BURN_MATERIALS:";
                const string end = ":END_BURN_MATERIALS:";
                IEnumerable<string> result = Ucf.StaticRead(FilePathUcf, start, end, StringComparison.InvariantCultureIgnoreCase);

                foreach (string temp in result)
                    materials.Add(temp);

                foreach (Component comp in
                         from comp in selOneComp
                         from attrAll in comp.GetUserAttributes()
                         where attrAll.Title.ToUpper() == "MATERIAL"
                         let attrValue = comp.GetUserAttributeAsString(attrAll.Title, NXObject.AttributeType.String, -1)
                         where materials.Contains(attrValue)
                         select comp)
                {
                    _selComponents.Add(comp);
                    DisplayName.Add(comp.DisplayName);
                }

                if (DisplayName.Count == 0)
                {
                    print_("There are no components with HRS PLT / 4140 PLT");
                    DisplayName.Clear();
                    selCompListBox.Items.Clear();
                    return;
                }

                selCompListBox.Items.AddRange(DisplayName.ToArray());
                selCompListBox.SelectedIndex = 0;
                string dispComponent = (string)selCompListBox.SelectedItem;

                foreach (Component component in _selComponents.Where(
                             component => component.DisplayName == dispComponent))
                    SetDisplayPart(component);

                if (_isBurnout)
                    SetBurnoutComponentState();

                selectButton.Enabled = false;
                selectAllButton.Enabled = false;
                materialComboBox.Enabled = false;
                materialSelectButton.Enabled = false;
                burnoutCompButton.Enabled = false;
                fourViewButton.Enabled = false;
                buttonNonAssocDimsSymbols.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void FourViewButton_Click(object sender, EventArgs e)
        {
            try
            {
                _originalDisplayPart = __display_part_;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();
                _is4View = true;

                if (__display_part_.ComponentAssembly.RootComponent is null)
                    return;

                GetChildComponents(__display_part_.ComponentAssembly.RootComponent);

                if (AllComponents.Count == 0)
                    return;

                List<Component> selOneComp = GetOneComponentOfMany(AllComponents);

                foreach (Component comp in selOneComp)
                {
                    if (!(comp.Prototype is Part))
                        continue;

                    Part compPart = (Part)comp.Prototype;
                    DrawingSheetCollection drawingCollection = compPart.DrawingSheets;

                    if (drawingCollection is null)
                        continue;

                    foreach (DrawingSheet drawingSht in drawingCollection)
                    {
                        if (drawingSht.Name != "4-VIEW")
                            continue;

                        _selComponents.Add(comp);
                        DisplayName.Add(comp.DisplayName);
                    }
                }

                if (DisplayName.Count == 0)
                {
                    print_("There are no components with a 4-VIEW");
                    _selComponents = new List<Component>();
                    DisplayName.Clear();
                    selCompListBox.Items.Clear();
                    return;
                }

                selCompListBox.Items.AddRange(DisplayName.ToArray());

                selCompListBox.SelectedIndex = 0;
                string dispComponent = (string)selCompListBox.SelectedItem;

                foreach (Component component in _selComponents.Where(
                             component => component.DisplayName == dispComponent))
                    SetDisplayPart(component);

                if (_is4View)
                    Set4ViewComponentState();

                selectButton.Enabled = false;
                selectAllButton.Enabled = false;
                materialComboBox.Enabled = false;
                materialSelectButton.Enabled = false;
                burnoutCompButton.Enabled = false;
                fourViewButton.Enabled = false;
                buttonNonAssocDimsSymbols.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonNonAssocDimsSymbols_Click(object sender, EventArgs e)
        {
            try
            {
                _originalDisplayPart = __display_part_;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();

                if (__display_part_.ComponentAssembly.RootComponent is null)
                    return;

                GetChildComponents(__display_part_.ComponentAssembly.RootComponent);
                GetAssemblyComponents(__display_part_.ComponentAssembly.RootComponent);

                if (AllComponents.Count == 0)
                    return;

                List<Component> selOneComp = GetOneComponentOfMany(AllComponents);

                foreach (Component comp in selOneComp)
                {
                    if (!(comp.Prototype is Part))
                        continue;

                    //Get all dims and symbols to check the associativity
                    Part part = (Part)comp.Prototype;
                    DimensionCollection dims = part.Dimensions;

                    if ((from dimension in dims.ToArray() select dimension.IsRetained).Any(isRetained => isRetained))
                    {
                        _selComponents.Add(comp);
                        DisplayName.Add(comp.DisplayName);
                    }

                    Tag drfEnt = NXOpen.Tag.Null;
                    bool isSymbolRetained = false;
                    ufsession_.Obj.CycleObjsInPart(part.Tag, UF_drafting_entity_type, ref drfEnt);

                    while (drfEnt != NXOpen.Tag.Null && isSymbolRetained == false)
                    {
                        ufsession_.Obj.AskTypeAndSubtype(drfEnt, out int type, out int subtype);

                        if (type == UF_drafting_entity_type && subtype == UF_draft_id_symbol_subtype)
                        {
                            double[] origin = new double[3];
                            ufsession_.Drf.AskIdSymbolInfo(drfEnt, out _, origin, out UFDrf.IdSymbolInfo[] symbolInfo);

                            foreach (UFDrf.IdSymbolInfo info in symbolInfo)
                            {
                                if ((from leaderInfo in info.leader_info
                                     let numAssocObj = 0
                                     select leaderInfo.num_assoc_objs).All(numAssocObj => numAssocObj != 0))
                                    continue;

                                isSymbolRetained = true;
                                _selComponents.Add(comp);
                                DisplayName.Add(comp.DisplayName);
                            }
                        }

                        ufsession_.Obj.CycleObjsInPart(part.Tag, UF_drafting_entity_type, ref drfEnt);
                    }
                }

                if (_selComponents.Count == 0)
                {
                    print_("There are no components with Non-Associative dimensions or id symbols");
                    DisplayName.Clear();
                    selCompListBox.Items.Clear();
                    return;
                }

                selCompListBox.Items.AddRange(DisplayName.ToArray());
                Show();
                selCompListBox.SelectedIndex = 0;
                string dispComponent = (string)selCompListBox.SelectedItem;

                foreach (Component component in _selComponents.Where(
                             component => component.DisplayName == dispComponent))
                    SetDisplayPart(component);

                selectButton.Enabled = false;
                selectAllButton.Enabled = false;
                materialComboBox.Enabled = false;
                materialSelectButton.Enabled = false;
                burnoutCompButton.Enabled = false;
                fourViewButton.Enabled = false;
                buttonNonAssocDimsSymbols.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SelCompListBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string DisplayName1 = (string)selCompListBox.SelectedItem;
                foreach (Component component in
                         _selComponents.Where(component => component.DisplayName == DisplayName1))
                {
                    SetDisplayPart(component);
                    SetDefaultLayers();

                    if (!removeCompCheckBox.Checked)
                        continue;

                    selCompListBox.Items.Clear();
                    DisplayName.Remove(DisplayName1);
                    selCompListBox.Items.AddRange(DisplayName.ToArray());

                    if (selCompListBox.Items.Count != 0)
                        selCompListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonOutputInfo_Click(object sender, EventArgs e)
        {
            print_("");
            print_("Cycle Components");
            print_("====================================================================");

            foreach (string selectComponent in selCompListBox.Items)
                print_(selectComponent + "\n");

            print_("====================================================================");
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            next_last_clicked = false;
            PreviousButton();
        }

        private void PreviousButton()
        {
            try
            {
                int prevIndex = selCompListBox.SelectedIndex - 1;

                if (prevIndex <= -1)
                    return;

                SetDefaultLayers();
                string currentName = (string)selCompListBox.SelectedItem;
                selCompListBox.SelectedIndex = prevIndex;
                string prevName = (string)selCompListBox.SelectedItem;

                foreach (Component component in _selComponents.Where(component => component.DisplayName == prevName))
                {
                    SetDisplayPart(component);

                    if (checkBoxDefaultLayers.Checked)
                    {
                        SetDefaultLayers();
                    }

                    if (_isBurnout)
                        SetBurnoutComponentState();

                    if (_is4View)
                        Set4ViewComponentState();

                    if (!removeCompCheckBox.Checked)
                        continue;

                    selCompListBox.Items.Clear();
                    DisplayName.Remove(currentName);
                    selCompListBox.Items.AddRange(DisplayName.ToArray());
                    int indexOfName = selCompListBox.FindString(prevName);
                    selCompListBox.SelectedIndex = indexOfName;
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            next_last_clicked = true;
            NextButton();
        }

        private void NextButton()
        {
            try
            {
                int nextIndex = selCompListBox.SelectedIndex + 1;

                if (nextIndex >= selCompListBox.Items.Count)
                    return;

                SetDefaultLayers();
                string currentName = (string)selCompListBox.SelectedItem;
                selCompListBox.SelectedIndex = nextIndex;
                string nextName = (string)selCompListBox.SelectedItem;

                foreach (Component component in _selComponents.Where(component => component.DisplayName == nextName))
                {
                    SetDisplayPart(component);

                    if (checkBoxDefaultLayers.Checked)
                    {
                        SetDefaultLayers();
                    }

                    if (_isBurnout)
                        SetBurnoutComponentState();

                    if (_is4View)
                        Set4ViewComponentState();

                    if (!removeCompCheckBox.Checked)
                        continue;

                    selCompListBox.Items.Clear();
                    DisplayName.Remove(currentName);

                    foreach (string name in DisplayName)
                        selCompListBox.Items.Add(name);

                    int indexOfName = selCompListBox.FindString(nextName);
                    selCompListBox.SelectedIndex = indexOfName;
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            try
            {
                const int modeling = 1;
                ufsession_.Draw.SetDisplayState(modeling);
                Part part2 = _originalDisplayPart;
                __display_part_ = part2;
                _originalDisplayPart = __display_part_;
                _isBurnout = false;
                _is4View = false;
                selectButton.Enabled = true;
                selectAllButton.Enabled = true;
                materialComboBox.SelectedIndex = -1;
                materialComboBox.Enabled = true;
                materialSelectButton.Enabled = false;
                burnoutCompButton.Enabled = true;
                fourViewButton.Enabled = true;
                buttonNonAssocDimsSymbols.Enabled = true;
                removeCompCheckBox.Checked = false;
                checkBoxDefaultLayers.Checked = false;
                checkBoxUpdateViews.Checked = false;
                _selComponents = new List<Component>();
                AllComponents.Clear();
                DisplayName.Clear();
                selCompListBox.Items.Clear();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static List<CtsAttributes> CreateBurnoutMaterialList()
        {
            string getMaterial = FilePathUcf.PerformStreamReaderString(":COMPONENT_BURN_MATERIALS:",
                ":END_COMPONENT_BURN_MATERIALS:");
            List<CtsAttributes> compMaterials =
               FilePathUcf. PerformStreamReaderList(":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");
            foreach (CtsAttributes cMaterial in compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";
            return compMaterials;
        }

        private static List<CtsAttributes> CreateMaterialList()
        {
            _ = new List<CtsAttributes>();
            string getMaterial = FilePathUcf.PerformStreamReaderString(":MATERIAL_ATTRIBUTE_NAME:",
                ":END_MATERIAL_ATTRIBUTE_NAME:");
            List<CtsAttributes> compMaterials =
                FilePathUcf.PerformStreamReaderList( ":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");
            foreach (CtsAttributes cMaterial in compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";
            return compMaterials;
        }

      

        public void SetDisplayPart(Component comp)
        {
            try
            {
                __display_part_ = (Part)comp.Prototype;
                __display_part_ = Session.GetSession().Parts.Display;
                ufsession_.Draw.SetDisplayState(1);
                __display_part_.ModelingViews.WorkView.Orient(View.Canned.Top, View.ScaleAdjustment.Fit);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void GetChildComponents(Component assembly)
        {
            try
            {
                foreach (Component child in assembly.GetChildren())
                {
                    if (child.IsSuppressed)
                        continue;

                    bool isValid = child.DisplayName.__IsDetail();

                    if (!isValid)
                    {
                        GetChildComponents(child);
                        continue;
                    }

                    Tag instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);

                    if (instance == NXOpen.Tag.Null)
                        continue;

                    ufsession_.Assem.AskPartNameOfChild(instance, out string partName);
                    int partLoad = ufsession_.Part.IsLoaded(partName);

                    if (partLoad == 1)
                    {
                        AllComponents.Add(child);
                        GetChildComponents(child);
                        continue;
                    }

                    ufsession_.Cfi.AskFileExist(partName, out int status);

                    if (status != 0)
                        continue;

                    ufsession_.Part.OpenQuiet(partName, out Tag partOpen, out _);

                    if (partOpen == NXOpen.Tag.Null)
                        continue;

                    AllComponents.Add(child);
                    GetChildComponents(child);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void GetAssemblyComponents(Component assembly)
        {
            try
            {
                foreach (Component child in assembly.GetChildren())
                {
                    if (child.IsSuppressed)
                        continue;

                    bool isValid = IsAssemblyNameValid(child);

                    if (!isValid)
                    {
                        GetAssemblyComponents(child);
                        return;
                    }

                    Tag instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);

                    if (instance == NXOpen.Tag.Null)
                        continue;

                    ufsession_.Assem.AskPartNameOfChild(instance, out string partName);
                    int partLoad = ufsession_.Part.IsLoaded(partName);

                    if (partLoad == 1)
                    {
                        AllComponents.Add(child);
                        GetAssemblyComponents(child);
                        continue;
                    }

                    ufsession_.Part.OpenQuiet(partName, out Tag partOpen, out _);

                    if (partOpen == NXOpen.Tag.Null)
                        continue;

                    AllComponents.Add(child);
                    GetAssemblyComponents(child);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static bool IsAssemblyNameValid(Component comp)
        {
            int lastIndex = comp.DisplayName.LastIndexOf('-');
            string subAssemName = comp.DisplayName.Remove(0, lastIndex + 1);

            if (subAssemName.Contains("lsp") || subAssemName.Contains("usp"))
                return true;

            switch (subAssemName)
            {
                case "lsh":
                case "ush":
                case "upr":
                case "lwr":
                    return true;
                default:
                    return false;
            }
        }

        private static List<Component> GetOneComponentOfMany(List<Component> compList)
        {
            List<Component> oneComp = new List<Component> { compList[0] };
            foreach (Component comp in from comp in compList
                                       let foundComponent = oneComp.Find(c => c.DisplayName == comp.DisplayName)
                                       where foundComponent == null
                                       select comp) oneComp.Add(comp);

            return oneComp.Count != 0 ? oneComp : null;
        }

      

     

        private static void SetDefaultLayers()
        {
            try
            {
                __display_part_.Layers.SetState(1, State.WorkLayer);
                using (StateCollection layerState = __display_part_.Layers.GetStates())
                {
                    foreach (Category category in __display_part_.LayerCategories)
                        if (category.Name == "ALL")
                            layerState.SetStateOfCategory(category, State.Hidden);
                    __display_part_.Layers.SetStates(layerState, true);
                }

                List<int> layers = new List<int>(new[] { 96, 99, 100, 200, 230 });

                if (__display_part_.Leaf.ToLower().Contains("layout"))
                    layers.Add(10);

                layers.Where(i => __display_part_.Layers.GetState(i) != State.WorkLayer)
                    .ToList()
                    .ForEach(i => __display_part_.Layers.SetState(i, State.Selectable));
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetBurnoutComponentState()
        {
            try
            {
                const int modeling = 1;
                ufsession_.Draw.SetDisplayState(modeling);
                __display_part_.Layers.SetState(100, State.WorkLayer);
                StateCollection layerState = __display_part_.Layers.GetStates();

                foreach (Category category in __display_part_.LayerCategories)
                    if (category.Name == "ALL")
                        layerState.SetStateOfCategory(category, State.Hidden);

                __display_part_.Layers.SetStates(layerState, true);
                layerState.Dispose();
                __display_part_.Layers.SetState(1, State.Selectable);

                foreach (DrawingSheet dwgSheet in __display_part_.DrawingSheets)
                {
                    if (dwgSheet.Name != "BURNOUT")
                        continue;

                    dwgSheet.Open();

                    if (!checkBoxUpdateViews.Checked)
                        continue;

                    foreach (DraftingView view in dwgSheet.GetDraftingViews())
                        view.Update();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void Set4ViewComponentState()
        {
            try
            {
                const int modeling = 1;
                ufsession_.Draw.SetDisplayState(modeling);
                __display_part_.Layers.SetState(1, State.WorkLayer);
                StateCollection layerState = __display_part_.Layers.GetStates();

                foreach (Category category in __display_part_.LayerCategories)
                    if (category.Name == "ALL")
                        layerState.SetStateOfCategory(category, State.Hidden);

                __display_part_.Layers.SetStates(layerState, true);
                layerState.Dispose();

                new[] { 94, 96, 99, 200, 230 }.Where(i => __display_part_.Layers.GetState(i) != State.WorkLayer)
                    .ToList()
                    .ForEach(i => __display_part_.Layers.SetState(i, State.Selectable));

                foreach (DrawingSheet dwgSheet in __display_part_.DrawingSheets)
                {
                    if (dwgSheet.Name != "4-VIEW")
                        continue;

                    dwgSheet.Open();

                    if (!checkBoxUpdateViews.Checked)
                        continue;

                    foreach (DraftingView view in dwgSheet.GetDraftingViews())
                        view.Update();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void selCompListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                //print_("Space bar");

                if (next_last_clicked)
                    NextButton();
                else
                    PreviousButton();
            }
        }
    }
}