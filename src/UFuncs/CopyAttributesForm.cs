using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using Point = System.Drawing.Point;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc("copy-attributes")]
    //[RevisionEntry("1.1", "2015", "07", "07")]
    //[Revision("1.1.1", "Fixed issue with quantity counting components with long file names.")]
    //[Revision("1.1.2", "It now only counts components with 3 digit component names.")]
    //[RevisionEntry("1.2", "2016", "05", "20")]
    //[Revision("1.2.1", "Made “SHOP”, “SHOP 1 NAME” & “UNITS” as drop down selections.")]
    //[RevisionEntry("1.21", "2016", "06", "02")]
    //[Revision("1.21.1", "Fixed issue with DETAIL NUMBER Attribute not working.")]
    //[RevisionEntry("1.3", "2017", "06", "26")]
    //[Revision("1.3.1", "Added revision number to form.")]
    //[Revision("1.3.2", "Completely took out CTS_Methods.")]
    //[Revision("1.3.3", "Changed IsNameValid method so that it can include different op numbers.")]
    //[Revision("1.3.3.1", "Most notably RTS 500 numbers.")]
    //[RevisionEntry("1.4", "2017", "08", "22")]
    //[Revision("1.4.1", "Signed so that it will run outside of CTS")]
    //[RevisionEntry("1.5", "2017", "09", "08")]
    //[Revision("1.5.1", "Added validation check")]
    //[RevisionEntry("1.6", "2017", "10", "10")]
    //[Revision("1.6.1", "Changed how the quantity is handled.")]
    //[Revision("1.6.1.1", "Added a check to make sure that a components.OwningComponent.ReferenceSet != “Empty”.")]
    //[RevisionEntry("1.7", "2021", "05", "27")]
    //[Revision("1.7.1", "The ConceptControlFile now points to \"U:\\nxFiles\\UfuncFiles\\ConceptControlFile.ucf\"")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public partial class CopyAttributesForm : _UFuncForm
    {
        private static readonly List<Component> _childComponents = new List<Component>();
        private static List<Component> _selectedComponents = new List<Component>();
        private static NXObject.AttributeInformation[] _attrInfo = __display_part_.GetUserAttributes();
        private static readonly List<MyAttribute> MyAttributes = new List<MyAttribute>();


        private static readonly string[] Shop = InputFromUcf(":CTS_ATTRIBUTE_SHOP:", ":CTS_ATTRIBUTE_SHOP_END:");

        private static readonly string[] Shop1Name =
            InputFromUcf(":CTS_ATTRIBUTE_SHOP1NAME:", ":CTS_ATTRIBUTE_SHOP1NAME_END:");

        private static readonly string[] Units = InputFromUcf(":CTS_ATTRIBUTE_UNITS:", ":CTS_ATTRIBUTE_UNITS_END:");
        private bool _flag;

        public CopyAttributesForm()
        {
            InitializeComponent();
            try
            {
                SetFormDefaults();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            try
            {
                SetUpDropDown();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Size = new Size(257, 664);

            try
            {
                UpdateLoadedAttributes();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void SetUpDropDown()
        {
            attributeCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(6, 17),
                Name = "attributeCombo",
                Size = new Size(188, 20),
                TabIndex = 16,
                Visible = false,
                Enabled = false
            };
            attributeCombo.Items.AddRange(Shop1Name.ToArray<object>());
            groupBox2.Controls.Add(attributeCombo);
        }


        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            _childComponents.Clear();
            _selectedComponents.Clear();

            if (__display_part_.ComponentAssembly.RootComponent == null)
            {
                SetFormDefaults();
                UpdateLoadedAttributes();
                print_("The display part is not an assembly");
                return;
            }

            GetChildComponents(__display_part_.ComponentAssembly.RootComponent);

            if (_childComponents.Count != 0)
            {
                _selectedComponents = GetOneComponentOfMany(_childComponents);
                buttonCopyApply.Enabled = true;
                progressBarLoadComps.Value = 100;
                return;
            }

            SetFormDefaults();
            UpdateLoadedAttributes();
            print_("There are no valid components");
        }


        private void buttonSelect_Click(object sender, EventArgs e)
        {
            try
            {
                _childComponents.Clear();
                _selectedComponents.Clear();

                _childComponents.AddRange(Selection.SelectManyComponents().ToList());


                if (_childComponents.Count == 0)
                {
                    SetFormDefaults();
                    UpdateLoadedAttributes();
                    return;
                }

                if (_childComponents.Count == 0)
                    return;

                _selectedComponents = GetOneComponentOfMany(_childComponents);
                buttonCopyApply.Enabled = true;
                progressBarLoadComps.Value = 100;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void buttonSetAttrDefaults_Click(object sender, EventArgs e)
        {
            __display_part_.__SetAttribute("UNITS", "N/A");
            __display_part_.__SetAttribute("UNITS", "INCH/METRIC");
            __display_part_.__SetAttribute("SHOP 1 NAME", "ENGINEERED/ADVANCED/DIENAMIC");
            __display_part_.__SetAttribute("CUSTOMER", "######");
            __display_part_.__SetAttribute("WFTD", "NO");
            __display_part_.__SetAttribute("WTN", "NO");
            __display_part_.__SetAttribute("SHOP", "CTS/ETS/DTS/ATS");
            __display_part_.__SetAttribute("DATE", "##-##-##");
            __display_part_.__SetAttribute("DESIGNER", "######");
            __display_part_.__SetAttribute("JOB NUMBER", "####-###");
            __display_part_.__SetAttribute("CTS NUMBER", "####-###");
            UpdateLoadedAttributes();
        }

        private void buttonAddAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                NXObject.AttributeInformation info = ((MyAttribute)listBoxAttributes.SelectedItem).Attribute;
                NXObject.AttributeInformation info1 =
                    session_.Parts.Display.GetUserAttribute(info.Title, NXObject.AttributeType.String, 0);
                info1.StringValue = _flag ? attributeCombo.SelectedItem as string : textBoxAttrValue.Text;
                session_.Parts.Display.SetUserAttribute(info1, NXOpen.Update.Option.Later);
                UpdateLoadedAttributes();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void buttonAttrDelete_Click(object sender, EventArgs e)
        {
            foreach (CtsAttributes delAttr in listBoxAttributes.Items)
                if (listBoxAttributes.GetSelected(listBoxAttributes.Items.IndexOf(delAttr)))
                    __display_part_.DeleteUserAttribute(NXObject.AttributeType.String, delAttr.AttrName, true,
                        NXOpen.Update.Option.Now);

            UpdateLoadedAttributes();
            textBoxAttrTitle.Clear();
            textBoxAttrValue.Clear();
        }

        private void textBoxAttrTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Tab)) return;
            textBoxAttrValue.Clear();
            textBoxAttrValue.Focus();
        }

        private void textBoxAttrValue_KeyDown(object sender, KeyEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    buttonAddAttribute.Focus();
                    break;
                case Keys.Return:
                    buttonAddAttribute.PerformClick();
                    break;
            }
        }

        public static string[] InputFromUcf(string start, string end)
        {
            List<string> strings = new List<string>();
            using (StreamReader textFile = new StreamReader(FilePathUcf))
            {
                while (textFile.ReadLine() != start)
                {
                }

                string line;
                while ((line = textFile.ReadLine()) != end) strings.Add(line);
            }

            return strings.ToArray();
        }

        private void listBoxAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxAttributes.SelectedItem is MyAttribute selected)) return;
            textBoxAttrTitle.Text = selected.Attribute.Title;
            switch (selected.Attribute.Title)
            {
                case "SHOP 1 NAME":
                    SetAttributeBox(selected.Attribute.Title, Shop1Name);
                    break;
                case "SHOP":
                    SetAttributeBox(selected.Attribute.Title, Shop);
                    break;
                case "UNITS":
                    SetAttributeBox(selected.Attribute.Title, Units);
                    break;
                default:
                    _flag = false;
                    attributeCombo.Enabled = false;
                    attributeCombo.Visible = false;
                    textBoxAttrValue.Enabled = true;
                    textBoxAttrValue.Visible = true;
                    textBoxAttrValue.Text = selected.Attribute.StringValue;
                    break;
            }
        }

        private void SetAttributeBox(string title, IEnumerable<string> items)
        {
            _flag = true;
            attributeCombo.Enabled = true;
            attributeCombo.Visible = true;
            textBoxAttrValue.Enabled = false;
            textBoxAttrValue.Visible = false;
            attributeCombo.Items.Clear();
            attributeCombo.Items.AddRange(items.ToArray<object>());
            string value = __display_part_.GetUserAttributeAsString(title, NXObject.AttributeType.String, -1);
            attributeCombo.SelectedItem = value;
        }

        private void buttonCopyApply_Click(object sender, EventArgs e)
        {
            List<NXObject.AttributeInformation> attributes = new List<NXObject.AttributeInformation>();
            if (listBoxAttributes.SelectedItems.Count == 0)
                attributes.AddRange(from MyAttribute attribute in listBoxAttributes.Items select attribute.Attribute);
            else
                attributes.AddRange(from MyAttribute attribute in listBoxAttributes.SelectedItems
                    select attribute.Attribute);
            CopyAttributes(attributes.ToArray());
            progressBarCopyAttr.Value = 0;
        }

        private void CopyAttributes(NXObject.AttributeInformation[] attributes)
        {
            progressBarCopyAttr.Maximum = _selectedComponents.Count;
            progressBarCopyAttr.Value = 0;
            foreach (Component comp in _selectedComponents.Select(__c => __c))
            {
                Part part = (Part)comp.Prototype;
                foreach (NXObject.AttributeInformation attribute in attributes)
                    part.SetUserAttribute(attribute, NXOpen.Update.Option.Later);
                UpdateQuantity(part);
                UpdateDetail(part, comp.DisplayName.Substring(comp.DisplayName.Length - 3, 3));
                progressBarCopyAttr.PerformStep();
            }
        }

        public static void UpdateQuantity(Part part)
        {
            UFSession.GetUFSession().Assem.AskOccsOfPart(__display_part_.Tag, part.Tag, out Tag[] partOccs);

            int __count = partOccs.Select(session_.__GetTaggedObject)
                .Cast<Component>()
                .Where(__c => !__c.IsSuppressed)
                .Where(__c => __c.Name.Length == 3)
                .Count(__c => __c.OwningComponent.ReferenceSet != "Empty");

            part.SetUserAttribute("QTY", -1, $"{__count}", NXOpen.Update.Option.Now);
        }

        private static void UpdateDetail(Part part, string detail)
        {
            if (!int.TryParse(detail, out int compNumber))
                return;

            if (compNumber <= 0 || compNumber >= 1000)
                return;

            // Added the following back in to make DETAIL NUMBER work again.  2016-06-02
            NXObject.AttributeInformation detailNumber = new NXObject.AttributeInformation
            {
                Type = NXObject.AttributeType.String,
                Title = "DETAIL NUMBER",
                StringValue = detail
            };

            part.SetUserAttribute(detailNumber, NXOpen.Update.Option.Later);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            SetFormDefaults();
            UpdateLoadedAttributes();
        }

        private void SetFormDefaults()
        {
            _childComponents.Clear();
            _selectedComponents.Clear();
            listBoxAttributes.Items.Clear();
            textBoxAttrTitle.Clear();
            textBoxAttrValue.Clear();
            progressBarLoadComps.Value = 0;
            progressBarCopyAttr.Value = 0;
            buttonCopyApply.Enabled = false;
            CleanUpHighlight();
        }

        private void UpdateLoadedAttributes()
        {
            MyAttributes.Clear();
            listBoxAttributes.Items.Clear();
            textBoxAttrTitle.Clear();
            textBoxAttrValue.Clear();
            _attrInfo = __display_part_.GetUserAttributes();

            if (_attrInfo.Length == 0)
            {
                listBoxAttributes.Items.Add("NO ATTRIBUTES IN DISPLAY PART");
            }
            else
            {
                foreach (NXObject.AttributeInformation attr in _attrInfo)
                    if (MyAttribute.IsValidAttribute(attr))
                        MyAttributes.Add(new MyAttribute(attr));

                foreach (MyAttribute attribute in MyAttributes)
                    listBoxAttributes.Items.Add(attribute);
            }

            textBoxAttrValue.Enabled = true;
            textBoxAttrValue.Visible = true;
            attributeCombo.Enabled = false;
            attributeCombo.Visible = false;
        }

        private void GetChildComponents(Component assembly)
        {
            foreach (Component child in assembly.GetChildren().Select(__c => __c))
            {
                if (child.IsSuppressed)
                {
                    if (!IsNameValid(child))
                        continue;

                    print_($"{child.DisplayName} is suppressed");
                    continue;
                }

                if (!IsNameValid(child))
                {
                    GetChildComponents(child);
                    continue;
                }

                Tag instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);

                if (instance == NXOpen.Tag.Null)
                    continue;

                ufsession_.Assem.AskPartNameOfChild(instance, out string partName);

                if (ufsession_.Part.IsLoaded(partName) == 1)
                {
                    _childComponents.Add(child);
                    GetChildComponents(child);
                    continue;
                }

                UFSession.GetUFSession().Cfi.AskFileExist(partName, out int status);

                if (status != 0)
                    continue;

                ufsession_.Part.OpenQuiet(partName, out Tag partOpen, out _);

                if (partOpen == NXOpen.Tag.Null)
                    continue;

                _childComponents.Add(child);
                GetChildComponents(child);
            }
        }

        private static bool IsNameValid(Component comp)
        {
            // Revision 1.3 � 2017 � 06 � 26
            // Changed IsNameValid method so that it can include different op numbers
            return Regex.IsMatch(comp.DisplayName.ToLower(),
                       "^([0-9]{4,})-([0-9]{3})-([0-9]{3}|lsh|ush|lsp|usp|lwr|upr)$")
                   && !comp.DisplayName.EndsWith("000");
        }

        public void CleanUpHighlight()
        {
            using (PartCleanup partCleanup1 = session_.NewPartCleanup())
            {
                partCleanup1.TurnOffHighlighting = true;
                partCleanup1.DoCleanup();
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

        internal class MyAttribute
        {
            private static readonly string[] validAttributes =
            {
                "BREAK", "CTS NUMBER", "JOB NUMBER", "DESIGNER", "DATE", "SHOP", "CUSTOMER", "UNITS", "SHOP 1 NAME",
                "EC"
            };


            private NXObject.AttributeInformation attribute;


            public MyAttribute(NXObject.AttributeInformation attribute)
            {
                this.attribute = attribute;
            }

            public NXObject.AttributeInformation Attribute
            {
                get => attribute;
                set => attribute = value;
            }


            public override string ToString()
            {
                return attribute.Title;
            }


            public static bool IsValidAttribute(NXObject.AttributeInformation attribute)
            {
                foreach (string title in validAttributes)
                    if (title == attribute.Title)
                        return true;
                return false;
            }
        }
    }
}