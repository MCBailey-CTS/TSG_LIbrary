using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Layer;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static TSG_Library.UFuncs._UFunc;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_export_strip)]
    [RevisionEntry("1.0", "2017", "10", "26")]
    [Revision("1.0.1", "Created for NX.")]
    [RevisionEntry("1.01", "2017", "08", "30")]
    [Revision("1.01.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("1.02", "2017", "08", "30")]
    [Revision("1.02.1", "Fixed bug where PDF could not be exported.")]
    [Revision("1.02.1.1",
        "Solution: We are now forcing the drawing sheets open before operations are performed on them.")]
    [RevisionEntry("1.03", "2017", "11", "27")]
    [Revision("1.03.1", "Updated the settings files used for DWG and STP creation.")]
    [Revision("1.03.1.1", "They now both use NX11 settings files.")]
    [RevisionEntry("1.04", "2018", "01", "08")]
    [Revision("1.04.1", "Restructured code for better maintainability.")]
    [Revision("1.04.2", "Removed DWG checkbox along with the DWG code.")]
    [Revision("1.04.3", "Added a “Complete” message when the program is finished executing.")]
    [Revision("1.04.4",
        "Fixed bug where the part was unable to export if the selection came back with instances of NXOpen.Annotations.Hatch.")]
    [RevisionEntry("1.05", "2018", "01", "22")]
    [Revision("1.05.1", "Changed the load constructor.")]
    [Revision("1.05.1.1",
        "Now if the user launches the Ufunc from a 010-strip or a 900-strip, the text box will default to the (date-strip) or (date-flowchart) respectively.")]
    [Revision("1.05.2", "Added the date to the .stp file create.")]
    [Revision("1.05.3", "Fixed issue where the reference sets were screwing up the step file.")]
    [Revision("1.05.3.1",
        "If the user runs the ufunc from a 900-strip, the ufunc will use the Session.GetSession().Parts.Work.FullPath as the file path for the step file.")]
    [Revision("1.05.3.2",
        "If the user runs the ufunc from a 010-strip and checks the step file box, regardless of whether or not the user selects to export a part or not. The ufunc will export the part and use the exported part as the part to use for the step file. And at the end if the user had not check the export part box, the ufunc will delete the newly exported part before the folder gets zipped up.")]
    [RevisionEntry("2.0", "2018", "02", "14")]
    [Revision("2.0.1",
        "For Revision 1.05, we are no longer using the exported part as a file to use for the step file.")]
    [Revision("2.0.2",
        "We are no longer exporting an actual .prt file. The ufunc is now zipping up the strip assembly and placing it in the outgoing export folder.")]
    [Revision("2.0.3",
        "Updated the .def file being used for the step translator to \"U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def\"")]
    [Revision("2.0.4",
        "For the strip we are turning on layers (1, 6, 10, 200, 201, 202, 254), and then turning all other layers off.")]
    [Revision("2.0.5", "For strips only.")]
    [Revision("2.0.5.1",
        "Every fully loaded and unsuppressed component under the strip must have at least one fully loaded unsuppressed component under itself. This takes care of the bug the step translator is having when step files are converted to catia.")]
    [Revision("2.0.5.2",
        "If any press found as a child of the strip doesn't have a fully loaded component as well as the dummy file under it the program will not run. \"G:\\0Library\\SeedFiles\\Components\\Dummy.prt\"")]
    [Revision("2.0.5.3",
        "Foreach layout component under the strip, the ufunc gets the layout number and turns the appropriate layers in the actual layout. The same is done for the blanks.")]
    [RevisionEntry("2.1", "2018", "03", "05")]
    [Revision("2.1.1", "Added version number to form.")]
    [Revision("2.1.2", "Fixed bug where on certain strips layer 6 would be turned off.")]
    [RevisionEntry("2.2", "2018", "04", "03")]
    [Revision("2.2.1", "Fixed bug where the drawing sheet was not being updated before it is being printed.")]
    [RevisionEntry("2.3", "2019", "07", "11")]
    [Revision("2.3.1", "Updated to use the updated GFolder.")]
    [RevisionEntry("2.4", "2019", "08", "28")]
    [Revision("2.4.1", "GFolder updated to allow old job number under non cts folder.")]
    [RevisionEntry("2.5", "2020", "01", "17")]
    [Revision("2.5.1", "Edited output path. 4 digit jobs unchanged.")]
    [Revision("2.5.2", "6 digit jobs will now place their output folder in the Layout folder.")]
    [RevisionEntry("2.6", "2020", "09", "01")]
    [Revision("2.6.1", "Updated to use the new Printer definition.")]
    [RevisionEntry("2.7", "2020", "12", "16")]
    [Revision("2.7.1",
        "When exporting a strip, it will now go through all the children of the part to export a step of. Sets layers to selectable and unblanks objects.")]
    [RevisionEntry("2.8", "2021", "03", "04")]
    [Revision("2.8.1", "Changed export location for 6 digit jobs.")]
    [Revision("2.8.2", "The location will now export to {JobFolder\\Layout\\Go}.")]
    [Revision("2.8.3", "The step files will now be created using the same way as AssemblyExportDesignData.")]
    [RevisionEntry("2.9", "2021", "03", "08")]
    [Revision("2.9.1", "Added copy check box to form.")]
    [Revision("2.9.2",
        "Checking the copy check box will now copy contents of the created export strip directory to the \"Process and Sim Data for Design\" directory.")]
    [RevisionEntry("3.0", "2021", "03", "10")]
    [Revision("3.0.1",
        "Checking the copy check box will only copy the created zip file created to the \"Process and Sim Data for Design\".")]
    [Revision("3.0.2",
        "Fixed issue where the a save was called for each component of the blank or layout in the strip.")]
    [Revision("3.0.3", "Moved the update process to the beginning of the program. Instead of being per operation.")]
    [RevisionEntry("3.1", "2021", "05", "27")]
    [Revision("3.1.1", "When the user checks the checkbox \"Process Sim Data For Design\", the program will now")]
    [Revision("3.1.2", "go through the \"Process Sim Data For Design\" directory and get all the .7z files.")]
    [Revision("3.1.3",
        "The program will then prompt the user to keep or delete the zip files before the newly created zip file is moved to the directory.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class ExportStripForm : _UFuncForm
    {
        public const string Regex_Strip = "^(?<customerNum>\\d+)-(?<opNum>\\d+)-strip$";
        public const string DummyPath = "G:\\0Library\\SeedFiles\\Components\\Dummy.prt";
        public const string Regex_PressAssembly = "^(?<customerNum>\\d+)-Press-(?<opNum>\\d+)-Assembly$";
        public const string Regex_Layout = "^(?<customerNum>\\d+)-(?<opNum>\\d+)-layout$";
        public const string Regex_Blank = "^(?<customerNum>\\d+)-(?<opNum>\\d+)-blank$";


        public ExportStripForm()
        {
            InitializeComponent();

            bool Is010()
            {
                //get
                //{
                var opRegex = new Regex("^[0-9]{4,}-([0-9]{3,})");

                var match = opRegex.Match(__display_part_.Leaf);

                if(!match.Success)
                    return false;

                return match.Groups[1].Value == "010";
                //}
            }


            bool Is900()
            {
                //get
                //{
                var opRegex = new Regex("^[0-9]{4,}-([0-9]{3,})");

                var match = opRegex.Match(__display_part_.Leaf);

                if(!match.Success)
                    return false;
                return match.Groups[1].Value == "900";
                //}
            }

            if(Session.GetSession().Parts.Display is null)
                txtInput.Text = $@"{TodaysDate}-";
            else if(Is010())
                txtInput.Text = $@"{TodaysDate}-Strip";
            else if(Is900())
                txtInput.Text = $@"{TodaysDate}-FlowChart";

            txtInput.Focus();

            chkPart.Checked = true;
            chkSTP.Checked = true;
            chkPDF.Checked = true;
            btnAccept.Enabled = true;
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();
                Export(chkSTP.Checked, (int)numUpDownCopies.Value, chkPart.Checked, chkPDF.Checked, txtInput.Text,
                    chkCopy.Checked);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                print_("Complete");
                Show();
            }
        }

        public static void CheckAssemblyDummyFiles()
        {
            if(__display_part_.ComponentAssembly.RootComponent is null)
                return;

            foreach (var childOfStrip in __display_part_.ComponentAssembly.RootComponent.GetChildren())
            {
                if(childOfStrip.IsSuppressed)
                    continue;

                if(!(childOfStrip.Prototype is Part))
                    continue;

                if(!Regex.IsMatch(childOfStrip.DisplayName, Regex_PressAssembly, RegexOptions.IgnoreCase))
                    continue;


                if(childOfStrip.GetChildren().Length == 0)
                    throw new Exception(
                        $"A press exists in your assembly without any children. {childOfStrip._AssemblyPathString()}");


                if(childOfStrip.GetChildren().Length == 1)
                    throw new Exception(
                        $"A press exists in your assembly with only one child. Expecting a ram and a bolster. {childOfStrip._AssemblyPathString()}");

                if(childOfStrip.GetChildren().Length != 2)
                    continue;

                foreach (var childOfPress in childOfStrip.GetChildren())
                {
                    if(!(childOfPress.Prototype is Part))
                        throw new Exception(
                            $"The child of a press must be loaded. {childOfPress._AssemblyPathString()}");

                    if(childOfPress.IsSuppressed)
                        throw new Exception(
                            $"The child of a press cannot be suppressed. {childOfPress._AssemblyPathString()}");

                    if(childOfPress.GetChildren().Length != 0 && childOfPress.GetChildren()
                           .Any(component => !component.IsSuppressed && component.Prototype is Part))
                        continue;

                    throw new Exception(
                        $"The child of a bolster or ram under a press must be the Dummy file: {DummyPath}{childOfPress._AssemblyPathString()}");
                }

                break;
            }
        }


        public static void UpdateForStp()
        {
            var partsToSave = new HashSet<Part>();

            var currentD = __display_part_;
            var currentW = session_.Parts.Work;


            try
            {
                foreach (var child in session_.Parts.Work.ComponentAssembly.RootComponent.GetChildren())
                {
                    if(child.IsSuppressed)
                        continue;

                    if(!(child.Prototype is Part))
                        continue;

                    session_.Parts.SetDisplay((Part)child.Prototype, false, false, out _);

                    switch (child.ReferenceSet)
                    {
                        case "Empty":
                            continue;

                        default:
                        {
                            var proto = (Part)child.Prototype;

                            var referenceSet = proto.GetAllReferenceSets()
                                .FirstOrDefault(refset => refset.Name == child.ReferenceSet);

                            if(referenceSet is null)
                                continue;

                            var objectsInReferenceSet = referenceSet.AskMembersInReferenceSet();

                            foreach (var obj in objectsInReferenceSet)
                            {
                                if(!(obj is DisplayableObject disp))
                                    continue;

                                if(disp.Layer <= 1 || disp.Layer >= 256)
                                    continue;

                                if(proto.Layers.GetState(disp.Layer) == State.WorkLayer)
                                    continue;

                                proto.Layers.SetState(disp.Layer, State.Selectable);
                            }

                            session_.DisplayManager.UnblankObjects(objectsInReferenceSet.OfType<DisplayableObject>()
                                .ToArray());

                            partsToSave.Add(proto);
                        }
                            continue;
                    }
                }
            }
            finally
            {
                session_.Parts.SetDisplay(currentD, false, false, out _);
                session_.Parts.SetWork(currentW);
            }

            foreach (var part in partsToSave)
                part.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
        }

        private static void NewMethod(HashSet<Part> partsToSave, Component child)
        {
            var proto = (Part)child.Prototype;
            var referenceSet = proto.GetAllReferenceSets().First(refset => refset.Name == child.ReferenceSet);

            var objectsInReferenceSet = referenceSet.AskMembersInReferenceSet();

            foreach (var obj in objectsInReferenceSet)
            {
                if(!(obj is DisplayableObject disp))
                    continue;

                if(disp.Layer <= 1 || disp.Layer >= 256)
                    continue;

                if(proto.Layers.GetState(disp.Layer) == State.WorkLayer)
                    continue;

                proto.Layers.SetState(disp.Layer, State.Selectable);
            }

            session_.DisplayManager.UnblankObjects(objectsInReferenceSet.OfType<DisplayableObject>().ToArray());

            partsToSave.Add(proto);
        }

        private static void Export(bool chkSTP, int numUpDownCopies, bool chkPart, bool chkPDF, string txtInput,
            bool chkCopy)
        {
            var folder = GFolder.create(__display_part_.FullPath);

            __display_part_.SetUserAttribute("DATE", -1, TodaysDate, NXOpen.Update.Option.Now);

            if(folder is null)
                throw new Exception("The current displayed part is not in a GFolder.");

            if(chkSTP)
            {
                var currentD = __display_part_;
                var currentW = session_.Parts.Work;


                try
                {
                    CheckAssemblyDummyFiles();

                    var _currentD = __display_part_;
                    var _currentW = session_.Parts.Work;

                    try
                    {
                        SetLayersInBlanksAndLayoutsAndAddDummies(__display_part_);
                    }
                    finally
                    {
                        session_.Parts.SetDisplay(_currentD, false, false, out _);
                        session_.Parts.SetWork(_currentW);
                    }
                }
                finally
                {
                    session_.Parts.SetDisplay(currentD, false, false, out _);
                    session_.Parts.SetWork(currentW);
                }
            }


            if(numUpDownCopies > 0)
                ExportStripPrintDrawing(numUpDownCopies);

            if(!chkPart && !chkPDF && !chkSTP)
                return;

            if(chkPDF)
                UpdateForPdf();

            //if (chkSTP)


            var outgoingFolderName = folder.customer_number.Length == 6
                ? $"{folder.dir_layout}\\{txtInput}"
                : $"{folder.dir_outgoing}\\{txtInput}";

            if(Directory.Exists(outgoingFolderName))
            {
                if(MessageBox.Show(
                       $@"Folder {outgoingFolderName} already exists, is it okay to overwrite the files in the folder?",
                       @"Warning",
                       MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                Directory.Delete(outgoingFolderName, true);
            }

            Directory.CreateDirectory(outgoingFolderName);

            uf_.Ui.SetPrompt($"Export Strip: Setting layers in {__display_part_.Leaf}.");

            __work_part_.Layers.SetState(1, State.WorkLayer);

            for (var i = 2; i <= 256; i++)
                __work_part_.Layers.SetState(i, State.Hidden);

            new[] { 6, 10, 200, 201, 202, 254 }.ToList()
                .ForEach(i => __work_part_.Layers.SetState(i, State.Selectable));

            const string regex = "^\\d+-(?<op>\\d+)-.+$";

            var op = Regex.Match(session_.Parts.Work.Leaf, regex, RegexOptions.IgnoreCase).Groups["op"].Value;
            var commonString = $"{folder.customer_number}-{op}-strip {TodaysDate}";

            uf_.Ui.SetPrompt(chkPart
                ? "Exporting \".prt\" file."
                : "Finding objects to export.");

            if(chkPDF)
            {
                uf_.Ui.SetPrompt("Exporting PDF......");

                var outputPath = ExportPDF(outgoingFolderName, commonString);

                print_(File.Exists(outputPath)
                    ? $"Successfully created \"{outputPath}\"."
                    : $"Did not successfully create \"{outputPath}\".");
            }

            if(chkPart)
            {
                var outputPath = ExportStripPart(outgoingFolderName);

                print_(File.Exists(outputPath)
                    ? $"Successfully created \"{outputPath}\"."
                    : $"Did not successfully create \"{outputPath}\".");
            }


            if(chkSTP)
            {
                UpdateForStp();

                ExportStripStp(outgoingFolderName);
                //string outputPath = $"{outgoingFolderName}\\{session_.Parts.Work.Leaf}-{TodaysDate}.stp";
                ////session_.Execute(@"C:\Repos\NXJournals\JOURNALS\export_strip.py", "ExportStrip", "export_stp", new object[] { outputPath });
                ////NXOpen.UF.UFSession.GetUFSession().Ui.SetPrompt("Exporting Step File.......");


                //NXOpen.StepCreator step = session_.DexManager.CreateStepCreator();

                //try
                //{
                //    //step.ExportDestination = NXOpen.BaseCreator.ExportDestinationOption.NativeFileSystem;

                //    //step.SettingsFile = "U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def";

                //    //step.

                //    //step.ExportAs = NXOpen.StepCreator.ExportAsOption.Ap214;

                //    //step.InputFile = session_.Parts.Work.FullPath;

                //    //step.OutputFile = outputPath;

                //    //step.ProcessHoldFlag = true;

                //    step.Commit();
                //}
                //finally
                //{
                //    step.Destroy();
                //}

                //print_(File.Exists(outputPath)
                //    ? $"Successfully created \"{outputPath}\"."
                //: $"Did not successfully create \"{outputPath}\".");
            }

            uf_.Ui.SetPrompt($"Zipping up {outgoingFolderName}.");

            var filesToZip = Directory.GetFiles($"{outgoingFolderName}");

            var zipFileName = $"{folder.customer_number}-{txtInput}.7z";


            var zipFile = $"{outgoingFolderName}\\{zipFileName}";

            if(filesToZip.Length != 0)
            {
                var zip = new Zip7(zipFile, filesToZip);

                zip.Start();

                zip.WaitForExit();
            }

            if(chkCopy)
            {
                if(!Directory.Exists(folder.dir_process_sim_data_design))
                    Directory.CreateDirectory(folder.dir_process_sim_data_design);

                var zipFiles = Directory.GetFiles(folder.dir_process_sim_data_design, "*.7z",
                    SearchOption.TopDirectoryOnly);

                foreach (var zipFile1 in zipFiles)
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (MessageBox.Show($@"Delete file ""{zipFile1}""", @"Warning", MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            File.Delete(zipFile1);
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            throw new ArgumentException();
                    }

                File.Copy(zipFile, $"{folder.dir_process_sim_data_design}\\{zipFileName}", true);
            }

            session_.ApplicationSwitchImmediate("UG_APP_MODELING");

            session_.Parts.Work.Drafting.ExitDraftingApplication();
        }

        public static void UpdateForPdf()
        {
            var sheets = session_.Parts.Work.DrawingSheets.ToArray();

            switch (sheets.Length)
            {
                case 0:
                    throw new InvalidOperationException("Current work part doesn't not have a sheet to print.");
                case 1:
                    session_.ApplicationSwitchImmediate("UG_APP_DRAFTING");
                    sheets[0].Open();
                    uf_.Draw.IsObjectOutOfDate(sheets[0].Tag, out var outOfDate);
                    if(outOfDate)
                        uf_.Draw.UpdOutOfDateViews(sheets[0].Tag);
                    sheets[0].OwningPart.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
                    break;
                default:
                    throw new InvalidOperationException("Current work part contains more than 1 sheet.");
            }
        }

        public static string ExportPDF(string outPutPath, string fileName)
        {
            var sheets = session_.Parts.Work.DrawingSheets.ToArray();

            ExportStripPdf(session_.Parts.Work.FullPath, sheets[0].Name, $"{outPutPath}\\{fileName}.pdf");

            return $"{outPutPath}\\{fileName}.pdf";
        }

        private static void AddDummy(Part part, IEnumerable<int> layers)
        {
            Prompt($"Setting layers in {__display_part_.Leaf}.");

            var layerArray = layers.ToArray();

            __display_part_.Layers.WorkLayer = 1;

            for (var i = 2; i < +256; i++)
                if(layerArray.Contains(i))
                    __display_part_.Layers.SetState(i, State.Selectable);
                else
                    __display_part_.Layers.SetState(i, State.Hidden);

            __display_part_.Layers.SetState(layerArray.Min(), State.WorkLayer);

            __display_part_.Layers.SetState(1, State.Hidden);

            if(part.ComponentAssembly.RootComponent != null)
            {
                var validChild = part.ComponentAssembly.RootComponent
                    .GetChildren()
                    .Where(component => component.Prototype is Part)
                    .FirstOrDefault(component => !component.IsSuppressed);

                if(validChild != null)
                    return;
            }

            var dummyPart = session_.__FindOrOpen(DummyPath);

            Prompt($"Adding dummy file to {part.Leaf}.");

            session_.Parts.Work.ComponentAssembly.AddComponent(dummyPart, "Entire Part", "DUMMY", _Point3dOrigin,
                _Matrix3x3Identity, 1, out var status);

            status.Dispose();
        }

        public static void Prompt(string message)
        {
            UFSession.GetUFSession().Ui.SetPrompt(message);
        }

        private void SetAcceptButtonEnabled()
        {
            btnAccept.Enabled = chkPart.Checked || chkPDF.Checked || chkSTP.Checked || numUpDownCopies.Value > 0;
        }

        private void ChkBoxes_CheckedChanged(object sender, EventArgs e)
        {
            SetAcceptButtonEnabled();
        }

        private void NumUpDownCopies_ValueChanged(object sender, EventArgs e)
        {
            SetAcceptButtonEnabled();
        }

        public static List<Component> Descendants(Component parent)
        {
            var _list = new List<Component>
            {
                parent
            };

            foreach (var _child in parent.GetChildren())
                _list.AddRange(Descendants(_child));

            return _list;
        }

        public static void SetLayersInBlanksAndLayoutsAndAddDummies(Part snapStrip010)
        {
            if(!Regex.IsMatch(snapStrip010.Leaf, Regex_Strip, RegexOptions.IgnoreCase))
                throw new ArgumentException(@"Must be an op 010 strip.", nameof(snapStrip010));

            var currentD = __display_part_;
            var currentW = session_.Parts.Work;

            try
            {
                var blankNameRegex = new Regex("^BLANK-([0-9]{1,})$");

                var layoutNameRegex = new Regex("^LAYOUT-([0-9]{1,})$");

                var layoutPart = Descendants(__display_part_.ComponentAssembly.RootComponent)
                    .Select(component => component.Prototype)
                    .OfType<Part>()
                    .FirstOrDefault(component => Regex.IsMatch(component.Leaf, Regex_Layout, RegexOptions.IgnoreCase));

                //var blankPart = TSG_Library.Extensions.DisplayPart.RootComponent.Children
                var blankPart = Descendants(__display_part_.ComponentAssembly.RootComponent)
                    .Select(component => component.Prototype)
                    .OfType<Part>()
                    .FirstOrDefault(component => Regex.IsMatch(component.Leaf, Regex_Blank, RegexOptions.IgnoreCase));

                var layoutLayers = new HashSet<int>();

                var blankLayers = new HashSet<int>();

                foreach (var child in Descendants(__display_part_.ComponentAssembly.RootComponent))
                {
                    if(!(child.Prototype is Part))
                        continue;

                    if(child.IsSuppressed)
                        continue;

                    var blankMatch = blankNameRegex.Match(child.Name);

                    var layoutMatch = layoutNameRegex.Match(child.Name);

                    if(blankMatch.Success)
                        blankLayers.Add(int.Parse(blankMatch.Groups[1].Value) + 10);

                    if(!layoutMatch.Success)
                        continue;

                    var layer = int.Parse(layoutMatch.Groups[1].Value) * 10;

                    layoutLayers.Add(layer);

                    layoutLayers.Add(layer + 1);
                }

                if(blankLayers.Count != 0 && blankPart != null)
                {
                    session_.Parts.SetDisplay(blankPart, false, false, out _);


                    session_.Parts.SetWork(__display_part_);

                    AddDummy(blankPart, blankLayers);

                    Prompt($"Saving: {blankPart.Leaf}.");

                    __display_part_.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
                }

                if(layoutLayers.Count != 0 && layoutPart != null)
                {
                    session_.Parts.SetDisplay(layoutPart, false, false, out _);


                    session_.Parts.SetWork(__display_part_);

                    AddDummy(layoutPart, layoutLayers);

                    Prompt($"Saving: {layoutPart.Leaf}.");

                    __display_part_.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
                }
            }
            finally
            {
                session_.Parts.SetDisplay(currentD, false, false, out _);
                session_.Parts.SetWork(currentW);
            }

            snapStrip010.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
        }

        public static void ExportStripStp(string outgoingFolderName)
        {
            var outputPath = $"{outgoingFolderName}\\{session_.Parts.Work.Leaf}-{TodaysDate}.stp";

            var step = session_.DexManager.CreateStepCreator();

            using (session_.__UsingBuilderDestroyer(step))
            {
                step.ExportDestination = BaseCreator.ExportDestinationOption.NativeFileSystem;

                step.SettingsFile = "U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def";

                step.ExportAs = StepCreator.ExportAsOption.Ap214;

                step.InputFile = session_.Parts.Work.FullPath;

                step.OutputFile = outputPath;

                step.ProcessHoldFlag = true;

                step.Commit();
            }

            print_(File.Exists(outputPath)
                ? $"Successfully created \"{outputPath}\"."
                : $"Did not successfully create \"{outputPath}\".");
        }

        public static void ExportStripPdf(string partPath, string drawingSheetName, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);


            if(!filePath.EndsWith(".pdf"))
                throw new Exception("File path for PDF must end with \".pdf\".");

            if(File.Exists(filePath))
                throw new Exception($"PDF '{filePath}' already exists.");

            var part = session_.__FindOrOpen(partPath);

            //We can use SingleOrDefault here because NX will prevent the naming of two drawing sheets the exact same string.
            var sheet = part.DrawingSheets
                            .ToArray()
                            .SingleOrDefault(drawingSheet => drawingSheet.Name == drawingSheetName)
                        ??
                        throw new Exception($@"Part '{partPath}' does not have a sheet named '{drawingSheetName}'.");

            __display_part_ = part;
            __work_part_ = __display_part_;

            var pdfBuilder = part.PlotManager.CreatePrintPdfbuilder();

            using (session_.__UsingBuilderDestroyer(pdfBuilder))
            {
                pdfBuilder.Scale = 1.0;
                pdfBuilder.Size = PrintPDFBuilder.SizeOption.ScaleFactor;
                pdfBuilder.OutputText = PrintPDFBuilder.OutputTextOption.Polylines;
                pdfBuilder.Units = PrintPDFBuilder.UnitsOption.English;
                pdfBuilder.XDimension = 8.5;
                pdfBuilder.YDimension = 11.0;
                pdfBuilder.RasterImages = true;
                pdfBuilder.Colors = PrintPDFBuilder.Color.AsDisplayed;
                pdfBuilder.Watermark = "";
                UFSession.GetUFSession().Draw.IsObjectOutOfDate(sheet.Tag, out var flag);
                if(flag)
                {
                    UFSession.GetUFSession().Draw.UpdOutOfDateViews(sheet.Tag);
                    part.__Save();
                }

                sheet.Open();
                pdfBuilder.SourceBuilder.SetSheets(new NXObject[] { sheet });
                pdfBuilder.Filename = filePath;
                pdfBuilder.Commit();
            }
        }

        public static string ExportStripPart(string outgoingFolderPath)
        {
            var assemblyPartPaths = __display_part_.ComponentAssembly.RootComponent.__Descendants()
                .Where(component => !component.IsSuppressed)
                .Select(component => component.Prototype)
                .OfType<Part>()
                .Distinct()
                .Select(part => part.FullPath)
                .ToArray();

            var zip = new Zip7($"{outgoingFolderPath}\\NX StripAssembly.7z", assemblyPartPaths);
            zip.Start();
            zip.WaitForExit();
            return $"{outgoingFolderPath}\\NX StripAssembly.7z";
        }

        public static void ExportStripPrintDrawing(int copies)
        {
            if(copies == 0)
                return;

            var _WorkPart = Session.GetSession().Parts.Work;

            var TheUFSession = UFSession.GetUFSession();

            //session_.Execute(@"C:\Repos\NXJournals\JOURNALS\export_strip.py", "ExportStrip", "print_drawing_sheet", new object[] { copies });
            var printBuilder1 = _WorkPart.PlotManager.CreatePrintBuilder();

            try
            {
                printBuilder1.ThinWidth = 1.0;
                printBuilder1.NormalWidth = 2.0;
                printBuilder1.ThickWidth = 3.0;
                printBuilder1.Copies = copies;
                printBuilder1.RasterImages = true;
                printBuilder1.Output = PrintBuilder.OutputOption.WireframeBlackWhite;
                var sheets = _WorkPart.DrawingSheets.ToArray();
                switch (sheets.Length)
                {
                    case 0:
                        print_("Current work part doesn't not have a sheet to print.");
                        return;
                    case 1:
                        session_.ApplicationSwitchImmediate("UG_APP_DRAFTING");
                        sheets[0].Open();
                        TheUFSession.Draw.IsObjectOutOfDate(sheets[0].Tag, out var outOfDate);
                        if(outOfDate)
                            try
                            {
                                TheUFSession.Draw.UpdOutOfDateViews(sheets[0].Tag);
                                sheets[0].OwningPart.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }

                        printBuilder1.SourceBuilder.SetSheets(new NXObject[] { sheets[0] });
                        break;
                    default:
                        print_("Current work part contains more than 1 sheet.");
                        return;
                }

                printBuilder1.PrinterText = "\\\\ctsfps1.cts.toolingsystemsgroup.com\\CTS Office MFC";
                printBuilder1.Orientation = PrintBuilder.OrientationOption.Landscape;
                printBuilder1.Paper = PrintBuilder.PaperSize.Inch11x17;
                printBuilder1.Commit();
            }
            finally
            {
                printBuilder1.Destroy();
            }
        }

        private void ExportStripForm_Load(object sender, EventArgs e)
        {
        }
    }

    public static class TempExt
    {
        public static string _AssemblyPathString(this Component component)
        {
            return $"{component._AssemblyPath().Aggregate("{ ", (str, cmp) => $"{str}{cmp.DisplayName}, ")}}}";
        }

        public static IEnumerable<Component> _AssemblyPath(this Component component)
        {
            do
            {
                yield return component;
            } while ((component = component.Parent) != null);
        }
    }
}
// 732