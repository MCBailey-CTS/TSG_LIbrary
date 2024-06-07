using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions;
using static NXOpen.UF.UFConstants;
using static TSG_Library.UFuncs._UFunc;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_simulation_data_builder)]
    [RevisionEntry("1.00", "2017", "06", "05")]
    [Revision("", "Revision Log Created for NX 11")]
    [RevisionEntry("1.01", "2017", "08", "22")]
    [Revision("1.01.1", "Signed so it will run outside of CTS")]
    [RevisionEntry("2.0", "2017", "09", "13")]
    [Revision("2.0.1", "Removed a lot of redundant code.")]
    [Revision("2.0.2", "Added three radio buttons  (.prt, .igs, .stp)")]
    [Revision("2.0.3", "This gives the user the ability to override the desired export type.")]
    [RevisionEntry("2.1", "2017", "09", "27")]
    [Revision("2.1.1", "Fixed bug where original part was being exported versus newly created part.")]
    [RevisionEntry("2.2", "2018", "01", "03")]
    [Revision("2.2.1", "Increased the number of versions from 25 to 50.")]
    [RevisionEntry("2.3", "2018", "04", "26")]
    [Revision("2.3.1",
        "Edited the Regex used to find simulation files and customer numbers in a GFolder. It will allow underscores to be a part of the customer job number.")]
    [Revision("2.3.2",
        "File created from which the simulation regex is read from. This will allow us to be able to change the regex expression without having to edit the code itself.")]
    [Revision("2.3.3", "Also some of combo box elements are generated using the same file.")]
    [RevisionEntry("2.4", "2018", "05", "23")]
    [Revision("2.4.1", "Moved all hardcoded information that populates the combo boxes to the SimulationUcf file.")]
    [RevisionEntry("2.5", "2019", "06", "28")]
    [Revision("2.5.1", "Moved engineering text box and swapped it with the proposal text box.")]
    [Revision("2.5.2", "Changed the proposal text box to be S/P.")]
    [RevisionEntry("2.6", "2019", "07", "04")]
    [Revision("2.6.1", "Incorporated new GFolder into program.")]
    [Revision("2.6.2", "Program will now allow for job tags after the job number.")]
    [RevisionEntry("2.7", "2019", "07", "23")]
    [Revision("2.7.1", "Increased the number of versions to 999.")]
    [Revision("2.7.2", "Fixed issue that caused nx to lock up when the version number is greater than 99.")]
    [Revision("2.7.3", "Incorporated stock list check off regex.")]
    [RevisionEntry("2.8", "2019", "08", "28")]
    [Revision("2.8.1", "GFolder updated to allow old job number under non cts folder.")]
    [RevisionEntry("2.9", "2020", "08", "26")]
    [Revision("2.9.1", "Added delete button.")]
    [Revision("2.9.2", "Incorporated SimDataDeletion into this button.")]
    [Revision("2.9.3", "SimData deletion will no longer be it's own ufunc.")]
    [Revision("2.9.4", "Select button will now be disabled unless the user selects a data/tsg level.")]
    [Revision("2.9.5", "Added a menu with a top most checked box. Allows the user to select the Top Most form option.")]
    [RevisionEntry("3.0", "2020", "09", "03")]
    [Revision("3.0.1", "Updated to use new Sim path, Now: \"P:\\CTS_SIM\\Active\"")]
    [RevisionEntry("3.1", "2020", "09", "24")]
    [Revision("3.1.1",
        "Changed how we find files to overwrite. Chagned from \"Contains\" to \"Equals\" in the \"CheckName\" method in \"Program\".")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    [RevisionEntry("11.2", "2023", "05", "17")]
    [Revision("11.2.1", "Fixed jons issue where it couldn't find the proper sim folder.")]
    public partial class SimulationDataBuilderForm : _UFuncForm
    {
        private const string UnfoldMatlThk = "";

        private const string UnfoldMatType = "";

        private static readonly string TempDir = Environment.GetEnvironmentVariable("TMP");

        public SimulationDataBuilderForm()
        {
            InitializeComponent();
            ResetForm();
            rdoPrt.Checked = true;

            btnSelect.Enabled = false;
        }

        private string NameBuilder
        {
            get
            {
                var temp = "";

                if(txtJobNumber.Text != string.Empty)
                    temp = txtJobNumber.Text;

                if(cmbOperation.Text != string.Empty)
                    temp += cmbOperation.Text == @"Unfold"
                        ? "-00-" + cmbOperation.Text
                        : "-" + cmbOperation.Text;

                if(cmbVersion.Text != string.Empty)
                    temp += "-" + cmbVersion.Text;

                temp += cmbOperation.Text == @"Unfold"
                    ? "-" + UnfoldMatlThk + UnfoldMatType
                    : "-" + cmbSurface.Text;

                if(!string.IsNullOrEmpty(cmbToolSide.Text))
                    temp += $"-{cmbToolSide.Text}";

                if(!string.IsNullOrEmpty(cmbData.Text))
                    temp += $"-{cmbData.Text}";

                if(!string.IsNullOrEmpty(cmbStudyProposal.Text))
                    temp += cmbStudyProposal.Text;

                if(!string.IsNullOrEmpty(cmbEngineeringLevel.Text))
                    temp += $"-{cmbEngineeringLevel.Text}";

                if(!string.IsNullOrEmpty(txtCustomText.Text))
                    temp += $"-{txtCustomText.Text}";

                return temp;
            }
        }

        //public MainForm1() : this(typeof(Program))
        //{
        //}

        private void TopMostCheckBox_Click(object sender, EventArgs e)
        {
            TopMost = topMostCheckBox.Checked;
            //Properties.Settings.Default.SimDataBuilderTopMost = topMostCheckBox.Checked;
            //Properties.Settings.Default.Save();
        }

        //public MainForm1(Type type) : base(type)
        //{
        //    InitializeComponent();
        //    ResetForm();
        //    rdoPrt.Checked = true;

        //    // ReSharper disable once VirtualMemberCallInConstructor
        //    Text = @"3.1";

        //    btnSelect.Enabled = false;
        //}

        private void MainForm_Load(object sender, EventArgs e)
        {
            ControlBox = true;
            MinimizeBox = true;
            MaximizeBox = false;
            //topMostCheckBox.Checked = Properties.Settings.Default.SimDataBuilderTopMost;
            TopMost = topMostCheckBox.Checked;
        }

        private void ComboBoxEngineeringLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            __display_part_.__SetStringAttribute("ENGINEERING LEVEL", cmbEngineeringLevel.Text);
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            try
            {
                var partName = NameBuilder;

                var tsgNum = cmbData.Text;

                if(!CheckName(partName, "" + cmbEngineeringLevel.SelectedItem, (string)cmbOperation.SelectedItem,
                       SimActive, NameBuilder, tsgNum))
                    return;


                // Select the objects to export
                var selObjects = SelectObjects();
                if(selObjects.Length <= 0) return;

                // build export/iges/component name
                var tempPart = $"{TempDir}\\{NameBuilder}";

                // Determines if there are any child components of the Display.RootComponent whose display name matches the currently built name. (NameBuilder)
                var doesExist = false;

                if(__display_part_.ComponentAssembly.RootComponent != null)
                    doesExist = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                        .Any(simComp => simComp.Name == NameBuilder.ToUpper());

                // If the above comment is true and there are components tha exist with the same displayName..
                if(doesExist)
                {
                    // MessageBox prompts user if they want to Replace current file or cancel. 
                    var dResult = MessageBox.Show($@"Replace file {NameBuilder}?", @"File Exist",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if(dResult != DialogResult.OK)
                        return;

                    foreach (var simComp in __display_part_.ComponentAssembly.RootComponent.GetChildren())
                    {
                        if(simComp.Name != NameBuilder.ToUpper())
                            continue;

                        var closeSimPart = (Part)simComp.Prototype;

                        closeSimPart.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);

                        var markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
                        NXObject[] objects1 = { simComp };

                        session_.UpdateManager.AddObjectsToDeleteList(objects1);

                        session_.UpdateManager.DoUpdate(markId1);
                    }
                }

                BuildFilesExportIges(tempPart, selObjects);
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            //System.Diagnostics.Debugger.Launch();

            // If there is no current __display_part_ return.
            if(__display_part_ is null)
                return;

            // Sets the "txtJobNumber" and "txtCustomText" .Text properties to empty. 
            new[] { txtJobNumber, txtCustomText }.ToList().ForEach(box => box.Text = "");

            // Gets the regex expression to be used for matching a simulation file.
            var regex = new Regex("^(?<CustomerNumber>[0-9_]+|[0-9_]+-[0-9]+)-simulation$");

            // Matches the current display part to the regex. If not .Success return.
            var match = regex.Match(__display_part_.Leaf);

            if(!match.Success)
                return;

            // Sets the text of the txtJobNumber text box to the CustomerNumber found by the regex.
            txtJobNumber.Text = match.Groups["CustomerNumber"].Value;

            // Sets all the comboBoxes in the form to have an initial selected index of -1.
            new[] { cmbData, cmbStudyProposal, cmbOperation, cmbVersion, cmbSurface, cmbToolSide, cmbEngineeringLevel }
                .ToList().ForEach(box => box.SelectedIndex = -1);

            // Clears the text from the txtCustomerText box.
            txtCustomText.Clear();

            // Adds the elements from the SimUcfFile with the header "COMBO_OPERATION_ITEMS" to the "cmbOperation" combo box.
            cmbOperation.Items.AddRange(Settings.ComboOperationItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_SURFACE_ITEMS" to the "cmbSurface" combo box.
            cmbSurface.Items.AddRange(Settings.ComboSurfaceItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_PROPOSAL_DATA" to the "cmbProposal" combo box.
            cmbStudyProposal.Items.AddRange(Settings.ComboProposalDataItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_DATA_R_LEVELS" to the "cmbData" combo box.
            cmbData.Items.AddRange(Settings.ComboDataRLevelsItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_TOOL_SIDE_ITEMS" to the "cmbToolSide" combo box.
            cmbToolSide.Items.AddRange(Settings.ComboToolSideItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_VERSION_#" to the "cmbVersion" combo box.
            cmbVersion.Items.AddRange(Settings.ComboVersionItems());

            // Adds the elements from the SimUcfFile with the header "COMBO_EC" to the "cmbEngineeringLevel" combo box.
            cmbEngineeringLevel.Items.AddRange(Settings.ComboEcItems());
        }

        private void BuildFilesExportIges(string partFile, Tag[] tagObjects)
        {
            if(File.Exists(partFile))
                switch (MessageBox.Show($@"Part {partFile} already exists. Do you want to save over it?", @"Question?",
                            MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        File.Delete(partFile);
                        break;
                    default:
                        return;
                }

            var folder = GFolder.create_or_null(__work_part_)
                         ?? throw new InvalidOperationException(
                             "The current work part does not reside within a GFolder.");

            ExportPart(partFile, tagObjects);

            // get path to sim location, check directory structure
            var compSurfaceDirectory = folder.dir_surfaces();

            // export part to simulation\surfaces directory
            if(!Directory.Exists(compSurfaceDirectory))
                Directory.CreateDirectory(compSurfaceDirectory);

            compSurfaceDirectory += $"\\{NameBuilder}.prt";
            ExportPart(compSurfaceDirectory, tagObjects);
            _ = __display_part_.Leaf.Replace("-simulation", "");

            //string simDir = folder.is_cts_job()
            //    ? $"{folder.cts_number} ({folder.company}-{currentDisplayNameJob}){(cmbEngineeringLevel.Text != string.Empty ? " " + cmbEngineeringLevel.Text : "")}-{cmbData.Text}"
            //    : $"{folder.CustomerNumber}-{cmbData.Text}";

            //System.Diagnostics.Debugger.Launch();

            var simDir = $"{Path.GetFileNameWithoutExtension(folder.dir_job)}-{cmbData.Text}";

            var simPathSimDir = $"{SimActive}\\{simDir}";

            if(!Directory.Exists(simPathSimDir))
                Directory.CreateDirectory(simPathSimDir);

            if(!Directory.Exists($"{simPathSimDir}\\data"))
                Directory.CreateDirectory($"{simPathSimDir}\\data");

            if(!Directory.Exists($"{simPathSimDir}\\reports"))
                Directory.CreateDirectory($"{simPathSimDir}\\reports");

            if(!Directory.Exists($"{simPathSimDir}\\data\\fShapes"))
                Directory.CreateDirectory($"{simPathSimDir}\\data\\fShapes");

            AddNewSurface(compSurfaceDirectory, cmbOperation.Text, NameBuilder);

            // export blank iges data
            // Removed code below for exporting iges files according to CTS-CIT Item# 2014-0011 Simulation Data Builder Change - 2014-03 Duane VW
            var simPathDirData = $"{simPathSimDir}\\data";

            if(rdoPrt.Checked)
            {
                if(!Directory.Exists($"{simPathDirData}\\{cmbOperation.SelectedItem}"))
                    Directory.CreateDirectory($"{simPathDirData}\\{cmbOperation.SelectedItem}");

                if(File.Exists($"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.prt"))
                    File.Delete($"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.prt");

                File.Copy(compSurfaceDirectory, $"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.prt");

                print_($"Copied to : {simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.prt");
            }
            else if(rdoIges.Checked)
            {
                var igesDirectory = $"{simPathDirData}\\{cmbOperation.SelectedItem}";

                if(!Directory.Exists(igesDirectory))
                    Directory.CreateDirectory(igesDirectory);

                Iges(compSurfaceDirectory, $"{igesDirectory}\\{NameBuilder}.igs");

                print_($"Created: {igesDirectory}\\{NameBuilder}.igs");
            }
            else if(rdoStp.Checked)
            {
                if(!Directory.Exists($"{simPathDirData}\\{cmbOperation.SelectedItem}"))
                    Directory.CreateDirectory($"{simPathDirData}\\{cmbOperation.SelectedItem}");

                if(File.Exists($"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.stp"))
                    File.Delete($"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.stp");

                Step(compSurfaceDirectory, $"{simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.stp");

                print_($"Created: {simPathDirData}\\{cmbOperation.SelectedItem}\\{NameBuilder}.stp");
            }
        }

        public static void Step(string partPath, string dwgPath)
        {
            try
            {
                if(!File.Exists(partPath))
                    throw new ArgumentException(@"Path to part does not exist.", nameof(partPath));
                if(File.Exists(dwgPath))
                    throw new ArgumentException(@"Path for Step file already exists.", nameof(dwgPath));
                if(File.Exists(dwgPath))
                    throw new ArgumentException(@"Path for Step file already exists.", nameof(dwgPath));
                var stepCreator1 = session_.DexManager.CreateStepCreator();
                stepCreator1.ExportAs = StepCreator.ExportAsOption.Ap214;
                stepCreator1.ExportFrom = StepCreator.ExportFromOption.ExistingPart;
                stepCreator1.ObjectTypes.Solids = true;
                stepCreator1.InputFile = partPath;
                stepCreator1.OutputFile = dwgPath;
                stepCreator1.ObjectTypes.Curves = true;
                stepCreator1.ObjectTypes.Surfaces = true;
                stepCreator1.ObjectTypes.Solids = true;
                stepCreator1.FileSaveFlag = false;
                stepCreator1.ProcessHoldFlag = true;
                stepCreator1.LayerMask = "1-256";
                stepCreator1.Commit();
            }
            catch (Exception ex)
            {
                ex._PrintException("Error when creating Step file for " + partPath);
            }
        }

        public static void Iges(string newPartFile, string igesPath)
        {
            try
            {
                var igesCreator = Session.GetSession().DexManager.CreateIgesCreator();
                igesCreator.ExportModelData = true;
                igesCreator.ExportDrawings = true;
                igesCreator.MapTabCylToBSurf = true;
                igesCreator.BcurveTol = 0.050799999999999998;
                igesCreator.IdenticalPointResolution = 0.001;
                igesCreator.MaxThreeDMdlSpace = 10000.0;
                igesCreator.ObjectTypes.Curves = true;
                igesCreator.ObjectTypes.Surfaces = true;
                igesCreator.ObjectTypes.Annotations = true;
                igesCreator.ObjectTypes.Structures = true;
                igesCreator.ObjectTypes.Solids = true;
                igesCreator.ExportFrom = IgesCreator.ExportFromOption.ExistingPart;
                igesCreator.SettingsFile = "C:\\Program Files\\Siemens\\NX 11.0\\iges\\igesexport.def";
                igesCreator.MaxLineThickness = 2.0;
                igesCreator.SysDefmaxThreeDMdlSpace = true;
                igesCreator.SysDefidenticalPointResolution = true;
                igesCreator.InputFile = newPartFile;
                igesCreator.OutputFile = igesPath;
                igesCreator.FileSaveFlag = false;
                igesCreator.LayerMask = "1-256";
                igesCreator.DrawingList = "";
                igesCreator.ViewList = "Top,Front,Right,Back,Bottom,Left,Isometric,Trimetric,User Defined";
                igesCreator.ProcessHoldFlag = false;
                igesCreator.Commit();
                igesCreator.Destroy();
            }
            catch (Exception ex)
            {
                ex._PrintException("Error when creating Iges file for " + igesPath);
            }
        }

        private void CmbOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(cmbOperation.SelectedIndex < 0)
                    return;

                var operation = (string)cmbOperation.SelectedItem;
                var lowerOp = operation.ToLower();

                if(lowerOp.StartsWith("d") || lowerOp.Contains("refdata"))
                    rdoPrt.Checked = true;
                else
                    rdoIges.Checked = true;

                if(lowerOp.StartsWith("d"))
                {
                    cmbVersion.Enabled = true;
                    cmbStudyProposal.Enabled = true;
                    cmbEngineeringLevel.Enabled = true;
                    cmbSurface.Enabled = true;
                    cmbData.Enabled = true;
                    cmbToolSide.Enabled = true;
                }
                else if(lowerOp.StartsWith("t") || lowerOp == "b")
                {
                    cmbVersion.Enabled = true;
                    cmbStudyProposal.Enabled = true;
                    cmbEngineeringLevel.Enabled = true;
                    cmbSurface.Enabled = false;
                    cmbSurface.SelectedIndex = -1;
                    cmbData.Enabled = true;
                    cmbToolSide.Enabled = false;
                    cmbToolSide.SelectedIndex = -1;
                }
                else
                {
                    switch (lowerOp)
                    {
                        case "midsurfcurve":
                            cmbVersion.Enabled = false;
                            cmbVersion.SelectedIndex = -1;
                            cmbStudyProposal.Enabled = true;
                            cmbEngineeringLevel.Enabled = true;
                            cmbSurface.Enabled = false;
                            cmbSurface.SelectedIndex = -1;
                            cmbData.Enabled = true;
                            cmbToolSide.Enabled = false;
                            cmbToolSide.SelectedIndex = -1;
                            return;
                        case "refdata":
                            cmbVersion.Enabled = false;
                            cmbVersion.SelectedIndex = -1;
                            cmbStudyProposal.Enabled = true;
                            cmbEngineeringLevel.Enabled = true;
                            cmbSurface.Enabled = true;
                            cmbData.Enabled = true;
                            cmbToolSide.Enabled = false;
                            cmbToolSide.SelectedIndex = -1;
                            return;
                        case "unfold":
                            cmbToolSide.Enabled = false;
                            cmbToolSide.SelectedIndex = -1;
                            cmbVersion.Enabled = false;
                            cmbVersion.SelectedIndex = -1;
                            return;
                    }
                }

                var regex = new Regex($"^{txtJobNumber.Text}-{operation}-v(?<version>[0-9]+)");
                var versions = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                    .Select(component => component.DisplayName)
                    .Select(displayName => regex.Match(displayName))
                    .Where(match => match.Success)
                    .Select(match => int.Parse(match.Groups["version"].Value))
                    .ToArray();

                if(versions.Length == 0)
                {
                    cmbVersion.SelectedItem = "v01";
                    return;
                }

                var newMaxVersion = $"v{(versions.Max() + 1)._PadInt(2)}";
                if(cmbVersion.Items.OfType<string>().Contains(newMaxVersion))
                    cmbVersion.SelectedItem = newMaxVersion;
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        private void BtnSimDataDeletion_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();

                new SimDataDeletion().Execute();
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
            finally
            {
                Show();
            }
        }

        private void CmbData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbData.SelectedIndex < 0)
            {
                btnSelect.Enabled = false;
                return;
            }

            btnSelect.Enabled = true;
        }

        public static Tag[] SelectObjects()
        {
            const string prompt = "Select objects to export";

            const string title = "Export data";

            var clientData = IntPtr.Zero;

            TheUFSession.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

            TheUFSession.Ui.SelectWithClassDialog(prompt, title, UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY, null, clientData,
                out _, out _, out var selObjects);

            TheUFSession.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

            foreach (var objs in selObjects)
                TheUFSession.Disp.SetHighlight(objs, 0);

            return selObjects;
        }

        public static void AddNewSurface(string compSurfacePath, string comboBoxText, string nameBuilder)
        {
            var basePart1 = session_.find_or_open(compSurfacePath);

            var layer = 1;

            if(comboBoxText.StartsWith("d"))
                for (var i = 1; i < 10; i++)
                {
                    if(comboBoxText.Contains(i.ToString()))
                        layer = i * 10 + 1;
                }
            else if(comboBoxText.StartsWith("t"))
                for (var i = 1; i < 10; i++)
                {
                    if(comboBoxText.Contains(i.ToString()))
                        layer = i * 10 + 101;
                }
            else if(comboBoxText.StartsWith("b"))
                layer = 101;
            else
                switch (comboBoxText)
                {
                    case @"Unfold":
                        layer = 199;
                        break;
                    case @"MidSurfCurve":
                    case @"RefData":
                        layer = 201;
                        break;
                    default:
                        layer = 1;
                        break;
                }

            var surfacePart = basePart1;

            __display_part_.ComponentAssembly.AddComponent(surfacePart, "Entire Part", nameBuilder, _Point3dOrigin,
                _Matrix3x3Identity, layer, out var sPartLoadStatus);

            print_($"Added Component: {nameBuilder}");

            sPartLoadStatus.Dispose();

            __display_part_.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
        }

        public static bool CheckName(string partName, string engineering, string operation, string simPath,
            string nameBuilder, string tsgNum)
        {
            if(string.IsNullOrEmpty(operation))
                return false;

            var displayComp = __display_part_.ComponentAssembly.RootComponent;

            if(displayComp == null)
                return true;

            var childrenComps = displayComp.GetChildren()
                .Where(component => component.DisplayName == partName)
                .ToArray();

            if(childrenComps.Length == 0)
                return true;

            // If the program gets here then there must already be a child with that name.
            var dResult = MessageBox.Show($@"Replace file {nameBuilder}?", @"File Exist", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if(dResult != DialogResult.OK)
                return false;

            var part = session_.find_or_open(childrenComps[0].DisplayName);

            var partFilePath = part.FullPath;

            part.__Close(true, true);

            File.Delete(partFilePath);

            session_.delete_objects(childrenComps);

            var folder = GFolder.create_or_null(__work_part_);

            if(folder is null)
                throw new InvalidOperationException("The current work part does not reside within a GFolder.");

            var eng = string.IsNullOrEmpty(engineering) ? "" : " " + engineering;

            var simFolder = $"{simPath}\\{Path.GetFileNameWithoutExtension(folder.dir_job)}-{tsgNum}";

            //if (folder.is_cts_job())
            //    simFolder = $"{simPath}\\{folder.cts_number} ({folder.company}-{folder.CustomerNumber})-{tsgNum}";
            //else
            //    simFolder = $"{simPath}\\{folder.CustomerNumber}-{tsgNum}";


            var temp = $"{simFolder}\\data\\{operation}";

            // Revision 3.1
            var files = Directory.GetFiles(temp)
                .Where(path => Path.GetFileNameWithoutExtension(path).ToLower() == partName.ToLower())
                .ToList();

            files.ForEach(File.Delete);

            return true;
        }

        public static void ExportPart(string partFile, params Tag[] tagObjects)
        {
            if(File.Exists($"{partFile}.prt"))
                File.Delete($"{partFile}.prt");

            var exportOptions = new UFPart.ExportOptions
            {
                new_part = true,
                expression_mode = UFPart.ExportExpMode.CopyExpDeeply,
                params_mode = UFPart.ExportParamsMode.RemoveParams
            };

            TheUFSession.Part.ExportWithOptions(partFile, tagObjects.Length, tagObjects, ref exportOptions);

            print_($"Created: {partFile}");
        }

        public static class Settings
        {
            public static object[] ComboOperationItems()
            {
                return new object[]
                {
                    "b", "d1",
                    "d2", "d3",
                    "d4", "d5",
                    "d6", "d7",
                    "d8", "d9",
                    "d10", "d11",
                    "d12", "d13",
                    "d14", "d15",
                    "d16", "RefData",
                    "MidSurfCurve", "t1",
                    "t2", "t3",
                    "t4", "t5",
                    "t6", "t7",
                    "t8", "t9",
                    "t10", "t11",
                    "t12", "t13",
                    "t14", "t15",
                    "t16", "Unfold"
                };
            }

            public static object[] ComboSurfaceItems()
            {
                return new object[] { "lwr", "upr", "Pad Profile" };
            }

            public static object[] ComboVersionItems()
            {
                var list = new List<object>();

                for (var index = 1; index <= 999; index++)
                    list.Add($"v{(index > 9 ? index + "" : $"0{index}")}");

                return list.ToArray();
            }

            public static object[] ComboToolSideItems()
            {
                return new object[] { "lwr", "upr", "post", "pad", "punch", "lifter" };
            }

            public static object[] ComboDataRLevelsItems()
            {
                var list = new List<object>();

                for (var index = 1; index <= 20; index++)
                    list.Add($"tsg{index._PadInt(3)}");

                return list.ToArray();
            }

            public static object[] ComboEcItems()
            {
                var list = new List<object>();

                for (var index = 501; index <= 520; index++)
                    list.Add($"{index}");

                return list.ToArray();
            }


            public static object[] ComboProposalDataItems()
            {
                var list = new List<object>();

                for (var index = 1; index <= 10; index++)
                    list.Add($"s{index}");

                for (var index = 1; index <= 10; index++)
                    list.Add($"p{index}");

                return list.ToArray();
            }
        }
    }
}