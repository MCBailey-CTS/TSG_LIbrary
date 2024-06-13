using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using static TSG_Library.UFuncs._UFunc;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    //[RevisionLog("Export Sim Package")]
    //[RevisionEntry("1.0", "2017", "09", "08")]
    //[Revision("1.0.1", "Created fo NX.")]
    //[RevisionEntry("1.1", "2017", "10", "26")]
    //[Revision("1.1.1",
    //    "Fixed bug where when you pressed the select button on the form, it would prompt you to delete the export sim folder regardless if the folder existed or not at that time.")]
    //[RevisionEntry("1.2", "2018", "01", "17")]
    //[Revision("1.2.1",
    //    "Added a “.dxf” check box. Now if the user selects a component to export that follows the Regex (^CustomerJobNumber-B-) and the dxf checkbox is checked then the program will also export a “.dxf” and place it in the same zip folder.")]
    //[RevisionEntry("1.3", "2018", "04", "26")]
    //[Revision("1.3.1",
    //    "Edited the Regex used to find simulation files and customer numbers in a GFolder. It will allow underscores to be a part of the customer job number.")]
    //[Revision("1.3.2",
    //    "File created from which the simulation regex is read from. This will allow us to be able to change the regex expression without having to edit the code itself.")]
    //[RevisionEntry("1.4", "2019", "07", "15")]
    //[Revision("1.4.1", "Updated so that the program works with the updated GFolder.")]
    //[Revision("1.4.2", "Program now prints if it successfully creates or does not create the expected files.")]
    //[RevisionEntry("1.5", "2019", "08", "28")]
    //[Revision("1.5.1", "GFolder updated to allow old job number under non cts folder.")]
    //[RevisionEntry("1.6", "2021", "03", "04")]
    //[Revision("1.6.1", "Changed export location for 6 digit jobs.")]
    //[Revision("1.6.2", "The location will now export to {JobFolder\\Layout\\Go}.")]
    //[Revision("1.6.3", "The step files will now be created using the same way as AssemblyExportDesignData.")]
    //[Revision("1.6.4", "Removed the dxf capability from the form. No longer needed.")]
    //[RevisionEntry("1.7", "2021", "03", "08")]
    //[Revision("1.7.1", "Added copy check box to form.")]
    //[Revision("1.7.2",
    //    "Checking the copy check box will now copy contents of the created sim package to the \"Process and Sim Data for Design\" directory.")]
    //[RevisionEntry("1.8", "2021", "03", "10")]
    //[Revision("1.8.1",
    //    "Checking the copy check box will only copy the created zip file created to the \"Process and Sim Data for Design\".")]
    [UFunc(ufunc_export_sim_package)]
    public partial class ExportSimPackageForm : _UFuncForm
    {
        private const string Step214Ug = @"C:\Program Files\Siemens\NX 11.0\STEP214UG\step214ug.exe";

        private static readonly Ucf SimUcf = new Ucf("U:\\nxFiles\\UfuncFiles\\SimulationControlFile.ucf");

        public ExportSimPackageForm()
        {
            InitializeComponent();
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                // todo

                Match match = Regex.Match(__display_part_.Leaf, SimUcf.SingleValue("SIM_SPECIAL_REGEX"));

                if (!match.Success)
                {
                    print_("Current DisplayPart not in a valid Job Folder.");
                    return;
                }

                string customerNumber = match.Groups["CustomerNumber"].Value;

                GFolder folder = GFolder.create_or_null(__work_part_);

                if (folder is null)
                    throw new InvalidOperationException("The current work part does not reside within a GFolder.");

                string directory =
                    $"{folder.DirSimulation}\\{TodaysDate}-{customerNumber}-Simulation-Package-for-Design";

                if (Directory.Exists(directory))
                {
                    DialogResult result = MessageBox.Show($@"{directory} already exists, would you like to replace it?",
                        @"Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        return;
                }

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // Select Components for data export.
                Component[] selectedComponents = Selection.SelectManyComponents();

                if (selectedComponents.Length == 0)
                    return;

                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                Directory.CreateDirectory(directory);

                foreach (Component comp in selectedComponents)
                    try
                    {
                        string displayName = comp.DisplayName.ToLower();

                        if (!comp.__IsLoaded())
                            continue;

                        if (rdoPrt.Checked)
                            try
                            {
                                string leaf = displayName.Contains("master")
                                    ? comp.Name
                                    : comp.__Prototype().Leaf;

                                string compPath = $"{directory}\\{leaf}.prt";

                                if (File.Exists(compPath))
                                    File.Delete(compPath);

                                File.Copy(comp.__Prototype().FullPath, compPath);

                                print_(File.Exists(compPath)
                                    ? $"Successfully created \"{compPath}\"."
                                    : $"Unsuccessfully created \"{compPath}\".");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }

                        string exportFileNoExtension = $"{directory}\\{comp.__Prototype().Leaf}";

                        try
                        {
                            if (rdoStpIges.Checked
                                && (displayName.Contains("fshape")
                                    || displayName.Contains($"{customerNumber}-t")
                                    || displayName.Contains("pad")
                                    || displayName.Contains("profile")
                                    || displayName.Contains($"{customerNumber}-b"))
                               )
                                Iges(comp.__Prototype().FullPath, $"{exportFileNoExtension}.igs", true);
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }

                        try
                        {
                            if (rdoStpIges.Checked
                                && (displayName.Contains($"{customerNumber}-d")
                                    || displayName.ToLower().Contains("ref-data")
                                    || displayName.ToLower().Contains("refdata")
                                    || displayName.ToLower().Contains("master"))
                               )
                                Export.Stp(comp.__Prototype().FullPath, $"{exportFileNoExtension}.stp",
                                    "U:\\nxFiles\\Step Translator\\214ug.def");
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                // Deletes the log files created.
                Directory.GetFiles(directory, "*.log").ToList().ForEach(File.Delete);

                string[] filesToZip = Directory.GetFiles(directory);

                string zipFileName = $"{customerNumber}-Simulation-Package-for-Design-{TodaysDate}.7z";

                string zipFile = $"{directory}\\{zipFileName}";

                try
                {
                    Export.SevenZip(zipFile, true, filesToZip);

                    filesToZip.ToList().ForEach(File.Delete);

                    if (chkCopy.Checked)
                    {
                        string processDirectory = folder.DirProcessSimDataDesign;

                        if (!Directory.Exists(processDirectory))
                            Directory.CreateDirectory(processDirectory);

                        string directoryName = Path.GetFileNameWithoutExtension(directory);

                        string sourceDir = Path.GetDirectoryName(zipFile);

                        string destDir = $"{processDirectory}\\{directoryName}";

                        Directory.CreateDirectory(destDir);

                        foreach (string file in Directory.GetFiles(directory))
                        {
                            string fileName = Path.GetFileName(file);
                            File.Copy(file, $"{destDir}\\{fileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                try
                {
                    if (chkCopy.Checked)
                    {
                        string processDirectory = folder.DirProcessSimDataDesign;

                        if (!Directory.Exists(processDirectory))
                            Directory.CreateDirectory(processDirectory);

                        if (File.Exists($"{processDirectory}\\{zipFileName}"))
                            File.Delete($"{processDirectory}\\{zipFileName}");

                        File.Copy(zipFile, $"{processDirectory}\\{zipFileName}");
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
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

        public static void Iges(string partPath, string igesPath, bool wait)
        {
            try
            {
                if (File.Exists(igesPath))
                    throw new ArgumentException(@"Path for iges file already exists.", nameof(igesPath));

                IgesCreator igesCreator = Session.GetSession().DexManager.CreateIgesCreator();

                using (session_.__UsingBuilderDestroyer(igesCreator))
                {
                    igesCreator.ExportModelData = true;
                    igesCreator.ExportFrom = IgesCreator.ExportFromOption.ExistingPart;
                    igesCreator.ObjectTypes.Curves = true;
                    igesCreator.ObjectTypes.Surfaces = true;
                    igesCreator.ObjectTypes.Annotations = true;
                    igesCreator.ObjectTypes.Structures = true;
                    igesCreator.ObjectTypes.Solids = true;
                    igesCreator.SettingsFile = "C:\\Program Files\\Siemens\\NX 11.0\\iges\\igesexport.def";
                    igesCreator.ExportDrawings = true;
                    igesCreator.InputFile = partPath;
                    igesCreator.OutputFile = igesPath;
                    igesCreator.FileSaveFlag = false;
                    igesCreator.LayerMask = "1-256";
                    igesCreator.DrawingList = "";
                    igesCreator.ViewList = "Top,Front,Right,Back,Bottom,Left,Isometric,Trimetric,User Defined";
                    igesCreator.ProcessHoldFlag = wait;
                    igesCreator.Commit();
                }

                if (wait)
                    print_(File.Exists(igesPath)
                        ? $"Successfully created \"{igesPath}\"."
                        : $"Unsuccessfully created \"{igesPath}\".");
            }
            catch (Exception ex)
            {
                ex.__PrintException("Error when creating Iges file for " + partPath);
            }
        }

        //public static void SevenZip(string path, bool wait, params string[] fileNames)
        //{
        //    string str = Path.GetRandomFileName() + "zipData.txt";
        //    using (FileStream fileStream = File.Open(str, FileMode.Create)) fileStream.Close();

        //    using (StreamWriter streamWriter = new StreamWriter(str))
        //        foreach (string fileName in fileNames)
        //            streamWriter.WriteLine(fileName);

        //    SevenZip(path, wait, str);
        //}

        //public static void SevenZip(string path, bool wait, string textFileToRead)
        //{
        //    if (string.IsNullOrEmpty(path))
        //        throw new ArgumentException(@"Invalid path.", nameof(path));

        //    if (File.Exists(path))
        //        throw new IOException("The specified output_path already exists.");

        //    if (!File.Exists(textFileToRead))
        //        throw new FileNotFoundException();

        //    string fileToRead = "a -t7z \"" + path + "\" \"@" + textFileToRead + "\" -mx9";

        //    Process process = new Process()
        //    {
        //        EnableRaisingEvents = false,
        //        StartInfo =
        //        {
        //            FileName = "C:\\Program Files\\7-Zip\\7z",
        //            Arguments = fileToRead
        //        }
        //    };

        //    process.Start();

        //    if (!wait)
        //        return;

        //    process.WaitForExit();

        //    print_(File.Exists(path)
        //        ? $"Successfully created \"{path}\"."
        //        : $"Unsuccessfully created \"{path}\".");
        //}

        //public static Process CreateStpProcess(string partPath, string stpPath, string defPath) => new Process
        //{
        //    StartInfo = new ProcessStartInfo
        //    {
        //        Arguments = $"O=\"{stpPath}\" D=\"{defPath}\" \"{partPath}\"",
        //        FileName = $"\"{Step214Ug}\"",
        //        WindowStyle = ProcessWindowStyle.Hidden
        //    }
        //};
    }
}