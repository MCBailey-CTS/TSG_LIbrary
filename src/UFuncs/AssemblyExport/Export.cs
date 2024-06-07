using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Drawings;
using NXOpen.Layer;
using NXOpen.UF;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Utilities
{
    public class Export
    {
        private const string ExportImportExe =
            @"U:\NX110\Concept\NX110library\Ufunc\ExportImportData\ExportImportData.exe";

        private const string Step214Ug = @"C:\Program Files\Siemens\NX 11.0\STEP214UG\step214ug.exe";

        public const string _printerCts = "\\\\ctsfps1.cts.toolingsystemsgroup.com\\CTS Office MFC";

        public static readonly int[] Layers = { 1, 94, 100, 111, 200, 230 };

        public static void Design(
            Part topLevelAssembly,
            Component[] __components,
            string outgoingDirectoryName = null,
            bool isRto = false,
            bool zipAssembly = false,
            bool pdf4Views = false,
            bool dwg4Views = false,
            bool stp999 = false,
            bool stpDetails = false,
            bool stpSee3DData = false,
            bool dwgBurnout = false,
            bool parasolid = false,
            bool paraCasting = false,
            bool print4Views = false,
            bool isChange = false)
        {
            using (session_.using_display_part_reset())
            {
                try
                {
                    var components = __components;
                    TheUFSession.Ui.SetPrompt("Filtering components to export.");
                    var folder = GFolder.create(topLevelAssembly.FullPath);

                    if (!components.All(comp => comp.OwningPart.Tag == topLevelAssembly.Tag))
                        throw new InvalidOperationException(
                            "All valid components must be under the top level display part.");

                    var isSixDigit = folder.customer_number.Length == 6;
                    var hashedParts = new HashSet<Part>();

                    foreach (var comp in components)
                    {
                        if (!(comp.Prototype is Part part))
                            continue;

                        hashedParts.Add(part);
                    }

                    var validParts = hashedParts.ToArray();

                    const string sevenZip = @"C:\Program Files\7-Zip\7z.exe";

                    if (!File.Exists(sevenZip))
                        throw new FileNotFoundException($"Could not find \"{sevenZip}\".");

                    var parentFolder = isSixDigit
                        ? folder.dir_design_information
                        : folder.dir_outgoing;

                    var exportDirectory = string.IsNullOrEmpty(outgoingDirectoryName)
                        ? null
                        : $"{parentFolder}\\{outgoingDirectoryName}";

                    if (!(isRto && isSixDigit) && zipAssembly && exportDirectory != null &&
                        Directory.Exists(exportDirectory))
                        switch (MessageBox.Show($@"{exportDirectory} already exisits, would you like to overwrite it?",
                                    @"Warning", MessageBoxButtons.YesNo))
                        {
                            case DialogResult.Yes:
                                Directory.Delete(exportDirectory, true);
                                break;
                            default:
                                return;
                        }

                    if (!(isRto && isSixDigit))
                        if (!string.IsNullOrEmpty(outgoingDirectoryName))
                            Directory.CreateDirectory(exportDirectory);

                    // If this is an RTO, then we need to delete the data files in the appropriate op folders.
                    if (isRto && !isChange)
                        try
                        {
                            DeleteOpFolders(__display_part_, folder);
                        }
                        catch (Exception ex)
                        {
                            ex._PrintException();
                        }

                    if (exportDirectory != null)
                        Directory.CreateDirectory(exportDirectory);

                    var detailRegex = new Regex(Regex_Detail, RegexOptions.IgnoreCase);
                    validParts = validParts.DistinctBy(part => part.Leaf).ToArray();

                    var exportDict = SortPartsForExport(validParts);

                    if (!CheckSizeDescriptions(exportDict["PDF_4-VIEW"]))
                        switch (MessageBox.Show(
                                    "At least one block did not match its' description. Would you like to continue?",
                                    "Warning", MessageBoxButtons.YesNo))
                        {
                            case DialogResult.Yes:
                                break;
                            default:
                                return;
                        }

                    __display_part_.__Save();

                    var stop_watch = new Stopwatch();

                    stop_watch.Start();

                    try
                    {
                        /////////////////////                   
                        Process assemblyProcess = null;

                        try
                        {
                            // Sets up the strip.
                            if (isRto || stpDetails || zipAssembly)
                                using (session_.using_display_part_reset())
                                {
                                    SetUpStrip(folder);
                                }
                        }
                        catch (Exception ex)
                        {
                            ex._PrintException();
                        }

                        UpdateParts(
                            isRto,
                            pdf4Views, stpDetails, dwg4Views, exportDict["PDF_4-VIEW"],
                            print4Views, exportDict["PDF_4-VIEW"],
                            dwgBurnout, exportDict["DWG_BURNOUT"],
                            stp999, exportDict["STP_999"],
                            stpSee3DData, exportDict["STP_SEE3D"],
                            paraCasting, exportDict["X_T_CASTING"],
                            components);

                        var dict = new Dictionary<string, Process>();

                        if (isRto && detailRegex.IsMatch(topLevelAssembly.Leaf))
                        {
                            var stpPath = CreatePath(folder, topLevelAssembly, "-Step-Assembly", ".stp");
                            ;

                            var output = stpPath;

                            var dir = Path.GetDirectoryName(output);

                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            Stp(topLevelAssembly.FullPath, stpPath, FilePath_ExternalStep_Assembly_def);
                        }

                        // Prints the parts with 4-Views.
                        if (print4Views)
                            using (session_.using_display_part_reset())
                            {
                                PrintPdfs(exportDict["PDF_4-VIEW"]);
                            }

                        // Gets the processes that will create the pdf 4-Views.
                        if (isRto || pdf4Views)
                            foreach (var part in exportDict["PDF_4-VIEW"])
                                try
                                {
                                    if (part.Leaf.EndsWith("000"))
                                        continue;

                                    var pdfPath = CreatePath(folder, part, "-Pdf-4-Views", ".pdf");

                                    var dir = Path.GetDirectoryName(pdfPath);

                                    if (!Directory.Exists(dir))
                                        Directory.CreateDirectory(dir);

                                    if (File.Exists(pdfPath))
                                        File.Delete(pdfPath);

                                    Pdf(part, "4-VIEW", pdfPath);
                                }
                                catch (Exception ex)
                                {
                                    ex._PrintException();
                                }

                        // If this is a RTO then 
                        if (isRto || stpDetails)
                            foreach (var part in exportDict["PDF_4-VIEW"])
                                try
                                {
                                    var stpPath = CreatePath(folder, part, "-Step-Details", ".stp");

                                    var dir = Path.GetDirectoryName(stpPath);

                                    if (!Directory.Exists(dir))
                                        Directory.CreateDirectory(dir);

                                    if (File.Exists(stpPath))
                                        File.Delete(stpPath);

                                    Stp(part.FullPath, stpPath, FilePath_ExternalStep_Detail_def);
                                }
                                catch (Exception ex)
                                {
                                    ex._PrintException();
                                }
                        else if (zipAssembly)
                            try
                            {
                                var path = $"{exportDirectory}\\{topLevelAssembly.Leaf}.stp";

                                var dir = Path.GetDirectoryName(path);

                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);

                                Stp(topLevelAssembly.FullPath, path, FilePath_ExternalStep_Detail_def);
                            }
                            catch (Exception ex)
                            {
                                ex._PrintException();
                            }

                        //// Gets the processes that create stp see 3d data.
                        //if (isRto || stpSee3DData)
                        //    foreach (NXOpen.Part part in exportDict["STP_SEE3D"])
                        //        try
                        //        {
                        //            string output = CreatePath(folder, part, "-Step-See-3D-Data", ".stp");

                        //            string dir = Path.GetDirectoryName(output);

                        //            if (!Directory.Exists(dir))
                        //                Directory.CreateDirectory(dir);

                        //            Stp(part.FullPath, output, FilePath_ExternalStep_Detail_def);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex._PrintException();
                        //        }

                        //// Gets the processes that create stp 999 details.
                        //if (isRto || stp999)
                        //    foreach (NXOpen.Part part in exportDict["STP_999"])
                        //        try
                        //        {
                        //            string output = CreatePath(folder, part, "-Step-999", ".stp");

                        //            string dir = Path.GetDirectoryName(output);

                        //            if (!Directory.Exists(dir))
                        //                Directory.CreateDirectory(dir);

                        //            Stp(part.FullPath, output, FilePath_ExternalStep_Detail_def);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex._PrintException();
                        //        }

                        //// Gets the processes that create burnout dwgs.
                        //if (isRto || dwgBurnout)
                        //    foreach (NXOpen.Part part in exportDict["DWG_BURNOUT"])
                        //        try
                        //        {
                        //            string output = CreatePath(folder, part, "-Dwg-Burnouts", ".dwg");

                        //            string dir = Path.GetDirectoryName(output);

                        //            if (!Directory.Exists(dir))
                        //                Directory.CreateDirectory(dir);

                        //            Dwg(part.FullPath, "BURNOUT", output);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex._PrintException();
                        //        }

                        //// Gets the processes that create 4-Views dwgs.
                        //if (dwg4Views)
                        //    foreach (NXOpen.Part part in exportDict["DWG_4-VIEW"])
                        //        try
                        //        {
                        //            string output = CreatePath(folder, part, "-Dwg-4-Views", ".dwg");

                        //            if (!Directory.Exists(output))
                        //                Directory.CreateDirectory(output);

                        //            Dwg(part.FullPath, "4-VIEW", output);
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex._PrintException();
                        //        }

                        //// Gets the processes that creates parasolids.
                        //if (parasolid)
                        //    foreach (NXOpen.Part part in exportDict["X_T"])
                        //        try
                        //        {
                        //            string output = CreatePath(folder, part, "-Parasolids", ".x_t");
                        //            Process process = CreateParasolidProcess(part, output);
                        //            dict.Add(output, process);
                        //            string op = part._AskDetailOp();

                        //            string output1 = CreatePath(folder, part, "-Parasolids", ".stp");

                        //            Process process1 =
                        //                CreateStpProcess(part.FullPath, output1, FilePath_ExternalStep_Detail_def);

                        //            process1.Start();

                        //            process1.WaitForExit();
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex._PrintException();
                        //        }

                        // Creates casting parasolids.
                        if (isRto || paraCasting)
                            foreach (var castingPart in exportDict["X_T_CASTING"])
                                try
                                {
                                    CreateCasting(castingPart, folder);
                                }
                                catch (Exception ex)
                                {
                                    ex._PrintException();
                                }

                        var expectedFiles = new HashSet<string>(dict.Keys);

                        var directoriesToExport = new HashSet<string>(expectedFiles.Select(Path.GetDirectoryName));

                        CreateDirectoriesDeleteFiles(expectedFiles);

                        var zipPath = $"{exportDirectory}\\{topLevelAssembly.Leaf}_NX.7z";

                        if ((isRto && !isSixDigit) || (zipAssembly && !isRto))
                        {
                            assemblyProcess = Assembly(topLevelAssembly, false, zipPath);
                            assemblyProcess.Start();
                        }

                        prompt_("Validating Stp Files.");

                        WriteStpCyanFiles(expectedFiles);

                        prompt_("Zipping up data folders.");

                        // Gets all the data folders that were created and zips them up and places them in the proper outgoingData folderWithCtsNumber if this is an RTO.
                        if (isRto && !isSixDigit)
                            ZipUpDataFolders(directoriesToExport, exportDirectory);

                        if (isRto && !isSixDigit)
                            ZipupDirectories(sevenZip, directoriesToExport, zipPath);

                        foreach (var file_key in dict.Keys)
                            try
                            {
                                var process = dict[file_key];

                                if (File.Exists(file_key))
                                    continue;

                                prompt_($"Recreating: {file_key}");

                                process.Start();

                                process.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                ex._PrintException();
                            }

                        // Checks to make sure that any expected data files were actually created.
                        if (expectedFiles.Count > 0)
                            ErrorCheck(isRto, zipAssembly, expectedFiles);

                        // Moves the sim report to the out going folderWithCtsNumber if one exists.
                        if (isRto && !isSixDigit && !(exportDirectory is null))
                            MoveSimReport(folder, exportDirectory);

                        // Moves the stock list to the outgoing folderWithCtsNumber if one exists.
                        if (isRto && !isSixDigit && !(exportDirectory is null))
                            MoveStocklist(folder, topLevelAssembly.Leaf, exportDirectory);

                        if (!(exportDirectory is null))
                            ZipupDataDirectories(exportDirectory, assemblyProcess);

                        /////////////////////////
                    }
                    finally
                    {
                        stop_watch.Stop();

                        print_($"{stop_watch.Elapsed.Minutes}:{stop_watch.Elapsed.Seconds}");
                    }
                }
                finally
                {
                    UFSession.GetUFSession().Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                    UFSession.GetUFSession().Disp.RegenerateDisplay();
                }
            }
        }

        public static void DeleteOpFolders(Part part, GFolder folder)
        {
            if (folder is null)
                throw new ArgumentException();

            // Matches the {part.Leaf} to 000 regex.
            var top_match = Regex.Match(part.Leaf, Regex_Op000Holder, RegexOptions.IgnoreCase);

            // If the {match} is not a success, then {part} is not a "000".
            if (!top_match.Success)
                throw new Exception($"Part \"{part.FullPath}\" is not a 000.");

            // Gets the op of the {part}.
            var op = top_match.Groups["opNum"].Value;

            // The set that holds the data directories to delete.
            var directoriesToDelete = new HashSet<string>();

            switch (op)
            {
                // Matches the 900 000's.
                case "900":
                {
                    directoriesToDelete.Add($"{folder.dir_op("900")}\\{folder.customer_number}-900-Step-Assembly");

                    foreach (var component in part.ComponentAssembly.RootComponent.GetChildren())
                    {
                        var match = Regex.Match(component.DisplayName, Regex_Lwr);

                        if (!match.Success)
                            continue;

                        var assembly_op = match.Groups["opNum"].Value;

                        if (assembly_op.Length % 2 != 0)
                            continue;

                        for (var i = 0; i < assembly_op.Length - 1; i += 2)
                        {
                            var temp_op = assembly_op[i] + "" + assembly_op[i + 1] + "0";

                            var directory = folder.dir_op(temp_op);

                            directoriesToDelete.Add(
                                $"{directory}\\{folder.customer_number}-{temp_op}-Parasolids-Castings");
                            directoriesToDelete.Add($"{directory}\\{folder.customer_number}-{temp_op}-Pdf-4-Views");
                            directoriesToDelete.Add($"{directory}\\{folder.customer_number}-{temp_op}-Step-Details");
                            directoriesToDelete.Add($"{directory}\\{folder.customer_number}-{temp_op}-Step-Assembly");
                            directoriesToDelete.Add($"{directory}\\{folder.customer_number}-{temp_op}-Dwg-Burnouts");
                            directoriesToDelete.Add(
                                $"{directory}\\{folder.customer_number}-{temp_op}-Step-See-3D-Data");
                        }
                    }
                }
                    break;

                // This matches all the regular op 010, 020, 030 and so 000's.
                case var assemblyOp000 when assemblyOp000.Length == 3:
                {
                    // Gets the assembly folderWithCtsNumber correpsonding to the {assemblyOp000}.
                    var assemblyFolder = folder.dir_op(assemblyOp000);

                    // If the directory {assemblyFolder} doesn't exist, then we want to throw.
                    if (!Directory.Exists(assemblyFolder))
                        throw new DirectoryNotFoundException($"Could not find directory \"{assemblyFolder}\".");

                    foreach (var directory in Directory.GetDirectories(assemblyFolder))
                    {
                        var dirName = Path.GetFileName(directory);

                        if (dirName == null)
                            continue;

                        if (!dirName.StartsWith($"{folder.customer_number}-{assemblyOp000}"))
                            continue;

                        // Adds the {directory} to the {directoriesToDelete}.
                        directoriesToDelete.Add(directory);
                    }
                }
                    break;

                // Matches the assembly holder by ensuring that the op has an even amount of characters.
                case var assemblyHolder when assemblyHolder.Length % 2 == 0:
                {
                    // A list to hold the ops.
                    var opList = new List<string>();

                    // Gets the character array of the {assemblyHolder}.
                    var charArray = assemblyHolder.ToCharArray();

                    // Grab the op characters two at a time.
                    for (var i = 0; i < charArray.Length; i += 2)
                        opList.Add(charArray[i] + "" + charArray[i + 1] + "0");

                    foreach (var assemblyOp in opList)
                    {
                        // Gets the assembly folderWithCtsNumber correpsonding to the {assemblyOp000}.
                        var assemblyFolder = folder.dir_op(assemblyOp);

                        // If the directory {assemblyFolder} doesn't exist, then we want to throw.
                        if (!Directory.Exists(assemblyFolder))
                            throw new DirectoryNotFoundException($"Could not find directory \"{assemblyFolder}\".");

                        foreach (var directory in Directory.GetDirectories(assemblyFolder))
                        {
                            var dirName = Path.GetFileName(directory);

                            if (dirName == null)
                                continue;

                            if (!dirName.StartsWith($"{folder.customer_number}-{assemblyOp}"))
                                continue;

                            // Adds the {directory} to the {directoriesToDelete}.
                            directoriesToDelete.Add(directory);
                        }
                    }
                }
                    break;
            }

            foreach (var dir in directoriesToDelete)
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
        }


        public static void WriteStpCyanFiles(IEnumerable<string> expectedFiles)
        {
            foreach (var expected in expectedFiles)
            {
                if (!expected.EndsWith(".stp") || !File.Exists(expected))
                    continue;

                var fileText = File.ReadAllText(expected);

                if (!fileText.Contains("Cyan"))
                    continue;

                File.WriteAllText(expected, fileText.Replace("Cyan", "cyan"));
            }
        }

        public static void CreateDirectoriesDeleteFiles(IEnumerable<string> expectedFiles)
        {
            foreach (var file in expectedFiles)
                try
                {
                    var directory = Path.GetDirectoryName(file);
                    Directory.CreateDirectory(directory ?? throw new Exception());
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
        }

        public static bool CheckSizeDescriptions(IEnumerable<Part> partsInBom)
        {
            var allPassed = true;
            foreach (var part in partsInBom)
                try
                {
                    if (!SizeDescription1.Validate(part, out var message))
                    {
                        if (message == "Part does not contain a Dynamic Block.")
                            continue;
                        allPassed = false;
                        print_($"{part.Leaf}:\n{message}\n");
                    }
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }

            return allPassed;
        }

        public static string CreatePath(GFolder folder, Part part, string directoryTag, string extension)
        {
            var directory =
                $"{folder.dir_job}\\{folder.customer_number}-{part.__AskDetailOp()}\\{folder.customer_number}-{part.__AskDetailOp()}{directoryTag}";
            var stpPath = $"{directory}\\{part.Leaf}{extension}";
            return stpPath;
        }

        public static Process CreateParasolidProcess(Part part, string parasolidPath)
        {
            return new Process
            {
                StartInfo =
                {
                    Arguments = $"\"{ExportImportExe}\" PARASOLID \"{part.FullPath}\" \"{parasolidPath}\"",
                    FileName = RunManaged,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        public static Process CreateStpProcess(string partPath, string stpPath, string defPath)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = $"O=\"{stpPath}\" D=\"{defPath}\" \"{partPath}\"",
                    FileName = $"\"{Step214Ug}\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        public static void SetLayers()
        {
            __display_part_.Layers.SetState(1, State.WorkLayer);


            for (var i = 2; i <= 256; i++)
                if (Layers.Contains(i))
                    __display_part_.Layers.SetState(i, State.Selectable);
                else
                    __display_part_.Layers.SetState(i, State.Hidden);
        }

        public static void CreateCasting(Part part, GFolder folder)
        {
            using (session_.using_display_part_reset())
            {
                __display_part_ = part;

                var op = part.__AskDetailOp();

                var castingDirectory =
                    $"{folder.dir_job}\\{folder.customer_number}-{op}\\{folder.customer_number}-{op}-Parasolids-Castings";

                if (!Directory.Exists(castingDirectory))
                    Directory.CreateDirectory(castingDirectory);

                try
                {
                    var step_path = $"{castingDirectory}\\{part.Leaf}.stp";

                    if (File.Exists(step_path))
                        File.Delete(step_path);

                    using (session_.using_lock_ug_updates())

                    {
                        foreach (var child in __display_part_.ComponentAssembly.RootComponent.GetChildren())
                        {
                            if (child.Layer == 96)
                                continue;

                            if (child.IsSuppressed)
                                continue;

                            child.Suppress();
                        }
                    }


                    var theSession = Session.GetSession();
                    var workPart = theSession.Parts.Work;
                    var displayPart = theSession.Parts.Display;
                    // ----------------------------------------------
                    //   Menu: File->Export->STEP...
                    // ----------------------------------------------
                    Session.UndoMarkId markId1;
                    markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");

                    StepCreator stepCreator1;
                    stepCreator1 = theSession.DexManager.CreateStepCreator();

                    stepCreator1.ExportAs = StepCreator.ExportAsOption.Ap214;

                    stepCreator1.BsplineTol = 0.001;

                    stepCreator1.SettingsFile = "C:\\Program Files\\Siemens\\NX1899\\step214ug\\ugstep214.def";

                    stepCreator1.BsplineTol = 0.001;

                    stepCreator1.InputFile = part.FullPath;

                    theSession.SetUndoMarkName(markId1, "Export STEP File Dialog");

                    Session.UndoMarkId markId2;
                    markId2 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Export STEP File");

                    theSession.DeleteUndoMark(markId2, null);

                    Session.UndoMarkId markId3;
                    markId3 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Export STEP File");

                    stepCreator1.OutputFile = step_path;

                    stepCreator1.FileSaveFlag = false;

                    stepCreator1.LayerMask = "1,96";

                    stepCreator1.ProcessHoldFlag = true;

                    NXObject nXObject1;
                    nXObject1 = stepCreator1.Commit();

                    theSession.DeleteUndoMark(markId3, null);

                    theSession.SetUndoMarkName(markId1, "Export STEP File");

                    stepCreator1.Destroy();
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }


                var castingPath = $"{castingDirectory}\\{part.Leaf}.x_t";

                if (File.Exists(castingPath))
                    File.Delete(castingPath);

                var tagBodies = part.Bodies
                    .ToArray()
                    .OfType<Body>()
                    .Where(body => body.Layer == 1)
                    .Select(body => body.Tag)
                    .ToList();

                if (tagBodies.Count == 0)
                {
                    print_($"Did not find any solid bodies on layer 1 in part {part.Leaf}");

                    return;
                }

                if (!(part.ComponentAssembly.RootComponent is null))
                    foreach (var child in part.ComponentAssembly.RootComponent.GetChildren())
                    {
                        if (child.IsSuppressed)
                            continue;

                        if (child.Layer != 96)
                            continue;

                        if (child.ReferenceSet == "Empty")
                            continue;

                        foreach (var __body in child._Members().OfType<Body>().Where(__b => __b.IsSolidBody))
                            tagBodies.Add(__body.Tag);
                    }

                var castingFile =
                    $"{folder.dir_job}\\{folder.customer_number}-{op}\\{folder.customer_number}-{op}-Parasolids-Castings\\{part.Leaf}.x_t";

                TheUFSession.Ps.ExportData(tagBodies.ToArray(), castingFile);
            }

            var FullPath = part.FullPath;

            part.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);

            session_.find_or_open(FullPath);
        }

        public static void SetLayersInBlanksAndLayoutsAndAddDummies(Part snapStrip010)
        {
            if (!Regex.IsMatch(snapStrip010.Leaf, Regex_Strip, RegexOptions.IgnoreCase))
                throw new ArgumentException(@"Must be an op 010 strip.", nameof(snapStrip010));

            using (session_.using_display_part_reset())
            {
                var blankNameRegex = new Regex("^BLANK-([0-9]{1,})$");

                var layoutNameRegex = new Regex("^LAYOUT-([0-9]{1,})$");

                var layoutPart = __display_part_.ComponentAssembly.RootComponent._Descendants()
                    .Select(component => component.Prototype)
                    .OfType<Part>()
                    .FirstOrDefault(component => Regex.IsMatch(component.Leaf, Regex_Layout, RegexOptions.IgnoreCase));

                var blankPart = __display_part_.ComponentAssembly.RootComponent._Descendants()
                    .Select(component => component.Prototype)
                    .OfType<Part>()
                    .FirstOrDefault(component => Regex.IsMatch(component.Leaf, Regex_Blank, RegexOptions.IgnoreCase));

                var layoutLayers = new HashSet<int>();

                var blankLayers = new HashSet<int>();

                foreach (var child in __display_part_.ComponentAssembly.RootComponent._Descendants())
                {
                    if (!(child.Prototype is Part))
                        continue;

                    if (child.IsSuppressed)
                        continue;

                    var blankMatch = blankNameRegex.Match(child.Name);
                    var layoutMatch = layoutNameRegex.Match(child.Name);

                    if (blankMatch.Success)
                    {
                        var layer = int.Parse(blankMatch.Groups[1].Value) + 10;
                        blankLayers.Add(layer);
                    }

                    if (!layoutMatch.Success) continue;
                    {
                        var layer = int.Parse(layoutMatch.Groups[1].Value) * 10;
                        layoutLayers.Add(layer);
                        layoutLayers.Add(layer + 1);
                    }
                }

                if (blankLayers.Count != 0 && blankPart != null)
                {
                    __display_part_ = blankPart;
                    __work_part_ = __display_part_;
                    AddDummy(blankPart, blankLayers);
                    TheUFSession.Ui.SetPrompt($"Saving: {blankPart.Leaf}.");
                    Session.GetSession().Parts.Display
                        .Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
                }

                if (layoutLayers.Count != 0 && layoutPart != null)
                {
                    __display_part_ = layoutPart;
                    __work_part_ = __display_part_;
                    AddDummy(layoutPart, layoutLayers);
                    TheUFSession.Ui.SetPrompt($"Saving: {layoutPart.Leaf}.");
                    session_.Parts.Display.Save(BasePart.SaveComponents.False, BasePart.CloseAfterSave.False);
                }
            }

            snapStrip010.__Save();
        }

        private static void AddDummy(Part part, IEnumerable<int> layers)
        {
            TheUFSession.Ui.SetPrompt($"Setting layers in {__display_part_.Leaf}.");
            var layerArray = layers.ToArray();
            __display_part_.Layers.SetState(1, State.WorkLayer);

            for (var i = 2; i < +256; i++)
                __display_part_.Layers.SetState(i, layerArray.Contains(i)
                    ? State.Selectable
                    : State.Hidden);

            __display_part_.Layers.SetState(layerArray.Min(), State.WorkLayer);
            __display_part_.Layers.SetState(1, State.Hidden);

            if (!(part.ComponentAssembly.RootComponent is null))
            {
                var validChild = part.ComponentAssembly.RootComponent
                    .GetChildren()
                    .Where(component => component._IsLoaded())
                    .FirstOrDefault(component => !component.IsSuppressed);

                if (validChild != null)
                    return;
            }

            var dummyPart = session_.find_or_open(DummyPath);
            TheUFSession.Ui.SetPrompt($"Adding dummy file to {part.Leaf}.");
            __work_part_.ComponentAssembly.AddComponent(dummyPart, "Entire Part", "DUMMY", _Point3dOrigin,
                _Matrix3x3Identity, 1, out _);
        }

        public static void CheckAssemblyDummyFiles()
        {
            TheUFSession.Ui.SetPrompt("Checking Dummy files exist.");

            if (__display_part_.ComponentAssembly.RootComponent == null)
                return;

            foreach (var childOfStrip in __display_part_.ComponentAssembly.RootComponent.GetChildren())
            {
                if (childOfStrip.IsSuppressed)
                    continue;

                if (!childOfStrip._IsLoaded())
                    continue;

                if (!Regex.IsMatch(childOfStrip.DisplayName, Regex_PressAssembly, RegexOptions.IgnoreCase))
                    continue;

                var pressComponent = childOfStrip;

                if (pressComponent.GetChildren().Length == 0)
                    throw new InvalidOperationException(
                        $"A press exists in your assembly without any children. {pressComponent._AssemblyPathString()}");

                switch (pressComponent.GetChildren().Length)
                {
                    case 1:
                        throw new InvalidOperationException(
                            $"A press exists in your assembly with only one child. Expecting a ram and a bolster. {pressComponent._AssemblyPathString()}");
                    case 2:
                        foreach (var childOfPress in pressComponent.GetChildren())
                        {
                            if (!childOfPress._IsLoaded())
                                throw new InvalidOperationException(
                                    $"The child of a press must be loaded. {childOfPress._AssemblyPathString()}");

                            if (childOfPress.IsSuppressed)
                                throw new InvalidOperationException(
                                    $"The child of a press cannot be suppressed. {childOfPress._AssemblyPathString()}");

                            if (childOfPress.GetChildren().Length != 0 && childOfPress.GetChildren()
                                    .Select(component => component)
                                    .Any(component => !component.IsSuppressed && component.Prototype is Part))
                                continue;

                            throw new InvalidOperationException(
                                $"The child of a bolster or ram under a press must be the Dummy file: {DummyPath}. {childOfPress._AssemblyPathString()}");
                        }

                        break;
                }
            }
        }

        public static void ZipupDataDirectories(string exportDirectory, Process assemblyProcess)
        {
            try
            {
                var stpFilesInOutGoingFolder =
                    Directory.GetFiles(exportDirectory, "*.stp", SearchOption.TopDirectoryOnly);

                if (stpFilesInOutGoingFolder.Length != 0)
                {
                    var displayName = Path.GetFileNameWithoutExtension(stpFilesInOutGoingFolder.First());

                    var stpZipProcess = Create7ZipProcess($"{exportDirectory}\\{displayName}_STP.7z",
                        stpFilesInOutGoingFolder);

                    stpZipProcess.Start();

                    stpZipProcess.WaitForExit();

                    foreach (var file in stpFilesInOutGoingFolder)
                        try
                        {
                            if (File.Exists(file)) File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            ex._PrintException();
                        }
                }

                assemblyProcess?.WaitForExit();
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        /// <summary>
        ///     Gets the file paths of all Sim Reports with the specified <paramref name="ctsJobNumber" />.
        /// </summary>
        /// <param name="ctsJobNumber">The job number.</param>
        /// <returns>An array of SimReport file paths.</returns>
        public static string[] GetReports(string ctsJobNumber)
        {
            return (from directory in Directory.GetDirectories("X:\\")
                let directoryName = Path.GetFileName(directory)
                where directoryName != null
                let match = new Regex("^Reports-([0-9]{4})-([0-9]{4})$").Match(directoryName)
                where match.Success
                let startRange = int.Parse(match.Groups[1].Value)
                let endRange = int.Parse(match.Groups[2].Value)
                let jobNumber = int.Parse(ctsJobNumber)
                where startRange <= jobNumber && jobNumber <= endRange
                from report in Directory.GetFiles(directory, "*.7z")
                let fileName = Path.GetFileNameWithoutExtension(report)
                where fileName.StartsWith(ctsJobNumber)
                select report).ToArray();
        }

        public static IDictionary<string, ISet<Part>> SortPartsForExport(IEnumerable<Part> validParts)
        {
            var detailRegex = new Regex(Regex_Detail, RegexOptions.IgnoreCase);

            IDictionary<string, ISet<Part>> exportDict = new Dictionary<string, ISet<Part>>
            {
                { "PDF_4-VIEW", new HashSet<Part>() },
                { "DWG_4-VIEW", new HashSet<Part>() },
                { "STP_DETAIL", new HashSet<Part>() },
                { "DWG_BURNOUT", new HashSet<Part>() },
                { "STP_999", new HashSet<Part>() },
                { "STP_SEE3D", new HashSet<Part>() },
                { "X_T_CASTING", new HashSet<Part>() },
                { "X_T", new HashSet<Part>() }
            };

            foreach (var part in validParts)
            {
                var match = detailRegex.Match(part.Leaf);

                if (!match.Success)
                    continue;

                if (part.Leaf._IsAssemblyHolder())
                    continue;

                if (part.Leaf.EndsWith("000"))
                    continue;

                if (part.__HasDrawingSheet("4-VIEW"))
                {
                    exportDict["PDF_4-VIEW"].Add(part);
                    exportDict["DWG_4-VIEW"].Add(part);
                }

                if (part.__HasDrawingSheet("BURNOUT"))
                    exportDict["DWG_BURNOUT"].Add(part);

                if (part.__IsSee3DData())
                    exportDict["STP_SEE3D"].Add(part);

                if (part.__Is999())
                    exportDict["STP_999"].Add(part);

                if (part.__IsCasting())
                    exportDict["X_T_CASTING"].Add(part);

                if (part.__HasReferenceSet("BODY"))
                    exportDict["X_T"].Add(part);
            }

            return exportDict;
        }

        public static void UpdateParts(
            bool isRto,
            bool pdf4Views, bool stpDetails, bool dwg4Views, IEnumerable<Part> partsWith4ViewsNoAssemblyHolders,
            bool print4Views, IEnumerable<Part> partsWith4Views,
            bool dwgBurnout, IEnumerable<Part> burnoutParts,
            bool stp999, IEnumerable<Part> nine99Parts,
            bool stpSee3DData, IEnumerable<Part> see3DDataParts,
            bool paraCasting, IEnumerable<Part> castingParts,
            Component[] selected_components)
        {
            using (session_.using_display_part_reset())
            {
                ISet<Part> selected_parts =
                    new HashSet<Part>(selected_components.Select(__c => __c.Prototype).OfType<Part>());

                ISet<Part> partsToUpdate = new HashSet<Part>();

                if (isRto || pdf4Views || dwg4Views || stpDetails)
                    foreach (var part in partsWith4ViewsNoAssemblyHolders)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (isRto || dwgBurnout)
                    foreach (var part in burnoutParts)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (isRto || print4Views)
                    foreach (var part in partsWith4Views)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (isRto || paraCasting)
                    foreach (var part in castingParts)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (isRto || stp999)
                    foreach (var part in nine99Parts)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (isRto || stpSee3DData)
                    foreach (var part in see3DDataParts)
                        if (selected_parts.Contains(part))
                            partsToUpdate.Add(part);

                if (partsToUpdate.Count > 0)
                    UpdateParts(partsToUpdate.ToArray());
            }
        }

        public static void ZipupDirectories(string sevenZip, IEnumerable<string> directoriesToExport, string zipPath)
        {
            try
            {
                // Constructs the arguments that deletes the sub data folders in the newly created assembly zip folderWithCtsNumber.
                var arguments =
                    $"d \"{zipPath}\" -r {directoriesToExport.Select(Path.GetFileName).Where(dir => dir != null).Aggregate("", (s1, s2) => $"{s1} \"{s2}\"")}";

                // Starts the actual delete process.
                var deleteProcess = Process.Start(sevenZip, arguments);

                // Waits for the {deleteProcess} to finish.
                deleteProcess?.WaitForExit();
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        public static void MoveSimReport(GFolder folder, string exportDirectory)
        {
            try
            {
                if (!folder.is_cts_job())
                    return;

                if (GetReports(folder.cts_number).Length != 0)
                    foreach (var report in GetReports(folder.cts_number))
                    {
                        var reportName = Path.GetFileName(report);

                        if (reportName == null)
                            continue;

                        var exportedReportPath = $"{exportDirectory}\\{reportName}";

                        if (File.Exists(exportedReportPath))
                        {
                            print_($"Sim report \"{exportedReportPath}\" already exists.");
                            continue;
                        }

                        File.Copy(report, exportedReportPath);
                    }
                else
                    print_("Unable to find a sim report to transfer.");
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        public static void LaunchProcesses(int numberOfProcesses, params Process[] processes)
        {
            var processesCompleted = 0;

            try
            {
                var length = numberOfProcesses > processes.Length
                    ? processes.Length
                    : numberOfProcesses;
                var hashProcesses = new HashSet<Process>();
                for (var i = 0; i < length; i++)
                {
                    processes[i].Start();
                    hashProcesses.Add(processes[i]);
                    prompt_($"{processesCompleted} of {processes.Length}");
                }

                for (var i = length; processesCompleted < processes.Length; i++)
                {
                    var first = hashProcesses.FirstOrDefault(process => process != null && process.HasExited);
                    if (first == null)
                    {
                        i--;
                        continue;
                    }

                    hashProcesses.Remove(first);
                    processesCompleted++;
                    if (i < processes.Length)
                    {
                        var nextProcess = processes[i];
                        nextProcess.Start();
                        hashProcesses.Add(nextProcess);
                    }

                    prompt_($"{processesCompleted} of {processes.Length}");
                }

                processes.ToList().ForEach(proc => proc.WaitForExit());
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }

            TheUFSession.Ui.SetPrompt("All processes have finished.");
        }

        public static void UpdateParts(params Part[] parts)
        {
            var validParts = parts.Where(part => !part.Leaf._IsAssemblyHolder()).DistinctBy(part => part.Leaf)
                .ToArray();

            for (var i = 0; i < validParts.Length; i++)
                try
                {
                    var report = $"Updating: {i + 1} of {validParts.Length}. ";
                    var part = validParts[i];
                    TheUFSession.Ui.SetPrompt($"{report}Setting Display Part to {part.Leaf}. ");
                    __display_part_ = part;
                    __work_part_ = __display_part_;

                    if (part.__IsCasting() && !(part.ComponentAssembly.RootComponent is null))
                        // If it is a casting then it cannot contain a child that is a lift lug and set to entire part.
                        if ((from child in part.ComponentAssembly.RootComponent.GetChildren()
                                where child.Prototype is Part
                                where child._Prototype().FullPath.Contains("LiftLugs")
                                where child.ReferenceSet != Refset_Empty
                                select child)
                            .Any(child => child.ReferenceSet == Refset_EntirePart))
                            print_(
                                $"Casting part {__display_part_.Leaf} contains a Lift Lug that is set to Entire Part. Casting Part cannot be made.");

                    TheUFSession.Ui.SetPrompt($"{report}Setting layers in {part.Leaf}.");
                    SetLayers();
                    TheUFSession.Ui.SetPrompt($"{report}Finding DisplayableObjects in {part.Leaf}.");
                    var objects = new List<DisplayableObject>();

                    foreach (var layer in Layers)
                        objects.AddRange(__display_part_.Layers.GetAllObjectsOnLayer(layer)
                            .OfType<DisplayableObject>());

                    TheUFSession.Ui.SetPrompt($"{report}Unblanking objects in {part.Leaf}.");
                    session_.DisplayManager.UnblankObjects(objects.ToArray());

                    foreach (DraftingView view in __display_part_.DraftingViews)
                        view.Update();

                    TheUFSession.Ui.SetPrompt($"{report}Switching back to modeling in {part.Leaf}.");
                    session_.ApplicationSwitchImmediate("UG_APP_MODELING");
                    __work_part_.Drafting.ExitDraftingApplication();
                    TheUFSession.Ui.SetPrompt($"{report}Saving {part.Leaf}.");
                    part.__Save();
                }
                catch (Exception ex)
                {
                    ex._PrintException(validParts[i].Leaf);
                }
        }

        public static void SetUpStrip(GFolder folder)
        {
            try
            {
                var strip_010 = folder.file_strip("010");

                if (File.Exists(strip_010))
                    session_.find_or_open(strip_010);

                var op010Strip = session_.find_or_open(strip_010);

                SetLayersInBlanksAndLayoutsAndAddDummies(op010Strip);

                CheckAssemblyDummyFiles();

                op010Strip.__Save();
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        public static Process Assembly(Part snapPart, bool waitForProcess, string zipPath)
        {
            var filePaths = new HashSet<string>();

            foreach (var component in GetAssembly(snapPart.ComponentAssembly.RootComponent))
                filePaths.Add(component._Prototype().FullPath);

            return Create7ZipProcess(zipPath, filePaths.ToArray());
        }

        /// <summary>Gets all the components under the given <paramref name="snapComponent" />.</summary>
        /// <remarks>The program will not go past components that are suppressed or end with "simulation"</remarks>
        /// <param name="snapComponent">The component to get the descendants from.</param>
        private static IEnumerable<Component> GetAssembly(Component snapComponent)
        {
            if (!snapComponent._IsLoaded())
                yield break;

            if (snapComponent.DisplayName.ToLower().EndsWith("-simulation"))
                yield break;

            if (snapComponent.IsSuppressed)
                yield break;

            yield return snapComponent;

            foreach (var child in snapComponent.GetChildren())
            foreach (var comp in GetAssembly(child))
                yield return comp;
        }

        public static void PrintPdfs(IEnumerable<Part> partsWith4Views)
        {
            using (session_.using_suppress_display())
            {
                try
                {
                    UFSession.GetUFSession().Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                    UFSession.GetUFSession().Disp.RegenerateDisplay();
                    Print4Views(partsWith4Views);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
            }
        }

        public static void ErrorCheck(bool isRto, bool zipAssembly, IEnumerable<string> expectedFiles)
        {
            var enumerable = expectedFiles as string[] ?? expectedFiles.ToArray();

            var fileCreatedCount = enumerable.Where(File.Exists).Count();

            print_($"Created {fileCreatedCount} file(s).");

            if (!isRto && !zipAssembly)
                print_(
                    "Created files will have to be manually moved to outgoingData folderWithCtsNumber if that is desired. (Example: RTO)");

            var filesThatWereNotCreated = enumerable.Where(s => !File.Exists(s)).ToList();

            var errorList = new List<Tuple<string, string>>();

            foreach (var file in filesThatWereNotCreated)
            {
                var extension = Path.GetExtension(file);

                if (extension == null)
                    continue;

                var errorFilePath = file.Replace(extension, ".err");

                if (File.Exists(errorFilePath))
                {
                    var fileContents = File.ReadAllLines(errorFilePath);

                    errorList.Add(new Tuple<string, string>(file, fileContents[0]));

                    File.Delete(errorFilePath);
                }
                else
                {
                    errorList.Add(new Tuple<string, string>(file, "Unknown error."));
                }
            }

            if (errorList.Count <= 0)
                return;

            print_("Files that were not created.");

            errorList.ForEach(print_);
        }

        public static void ZipUpDataFolders(IEnumerable<string> directoriesToExport, string exportDirectory)
        {
            LaunchProcesses(10, (
                from directory in directoriesToExport
                let directoryName = Path.GetFileName(directory)
                select Create7ZipProcess($"{exportDirectory}\\{directoryName}.7z", directory)
            ).ToArray());
        }

        public static void Print4Views(IEnumerable<Part> allParts)
        {
            try
            {
                bool IsNotAssembly(Part part)
                {
                    var name = Path.GetFileNameWithoutExtension(part.FullPath);

                    if (name == null)
                        return false;

                    name = name.ToLower();

                    if (name.EndsWith("000") || name.EndsWith("lsh") || name.EndsWith("ush") || name.EndsWith("lwr") ||
                        name.EndsWith("upr"))
                        return false;

                    return !name.Contains("lsp") && !name.Contains("usp");
                }

                var parts = allParts.Where(part => Regex.IsMatch(part.Leaf, Regex_Detail, RegexOptions.IgnoreCase))
                    .Where(IsNotAssembly)
                    .Where(part => part.DraftingDrawingSheets.ToArray().Any(__d => __d.Name.ToUpper() == "4-VIEW"))
                    .ToList();

                parts.Sort((part1, part2) => string.Compare(part1.Leaf, part2.Leaf, StringComparison.Ordinal));

                for (var i = 0; i < parts.Count; i++)
                {
                    var part = parts[i];

                    TheUFSession.Ui.SetPrompt($"{i + 1} of {parts.Count}. Printing 4-VIEW of {part.Leaf}.");

                    __display_part_ = part;

                    __work_part_ = __display_part_;

                    var printBuilder = __work_part_.PlotManager.CreatePrintBuilder();

                    using (new Destroyer(printBuilder))
                    {
                        printBuilder.Copies = 1;

                        printBuilder.ThinWidth = 1.0;

                        printBuilder.NormalWidth = 2.0;

                        printBuilder.ThickWidth = 3.0;

                        printBuilder.Output = PrintBuilder.OutputOption.WireframeBlackWhite;

                        printBuilder.ShadedGeometry = true;

                        DrawingSheet drawingSheet = __work_part_.DraftingDrawingSheets.FindObject("4-VIEW");

                        drawingSheet.Open();

                        printBuilder.SourceBuilder.SetSheets(new NXObject[] { drawingSheet });

                        printBuilder.PrinterText = "\\\\ctsfps1.cts.toolingsystemsgroup.com\\CTS Office MFC";

                        printBuilder.Orientation = PrintBuilder.OrientationOption.Landscape;

                        printBuilder.Paper = PrintBuilder.PaperSize.Letter;

                        printBuilder.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        public static void MoveStocklist(GFolder folder, string topDisplayName, string exportDirectory)
        {
            try
            {
                var stocklist = (from file in Directory.GetFiles(folder.dir_stocklist)
                    let name = Path.GetFileNameWithoutExtension(file)
                    where name != null
                    where name.EndsWith($"{topDisplayName}-stocklist")
                    select file).SingleOrDefault();

                if (stocklist is null)
                {
                    print_($"Could not find a stocklist named: {topDisplayName}-stocklist");
                    return;
                }

                File.Copy(stocklist, $"{exportDirectory}\\{Path.GetFileName(stocklist)}");
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        public static Process Create7ZipProcess(string zipPath, params string[] filesToZip)
        {
            var tempFile = $"{Path.GetTempPath()}zipData{filesToZip.GetHashCode()}.txt";

            using (var fs = File.Open(tempFile, FileMode.Create))
            {
                fs.Close();
            }

            using (var writer = new StreamWriter(tempFile))
            {
                filesToZip.ToList().ForEach(writer.WriteLine);
            }

            return new Process
            {
                EnableRaisingEvents = false,
                StartInfo =
                {
                    CreateNoWindow = false,
                    FileName = "C:\\Program Files\\7-Zip\\7z",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    Arguments = $"a -t7z \"{zipPath}\" \"@{tempFile}\" -mx9"
                }
            };
        }

        public static void SevenZip(string path, bool wait, params string[] fileNames)
        {
            var directory = Path.GetDirectoryName(path);

            var str = directory + "\\" + "zipData.txt";

            try
            {
                using (var fileStream = File.Open(str, FileMode.Create))
                {
                    fileStream.Close();
                }

                using (var streamWriter = new StreamWriter(str))
                {
                    foreach (var fileName in fileNames)
                        streamWriter.WriteLine(fileName);
                }

                SevenZip(path, wait, str);
            }
            finally
            {
                File.Delete(str);
            }
        }

        public static void SevenZip(string path, bool wait, string textFileToRead)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(@"Invalid path.", nameof(path));

            if (File.Exists(path))
                throw new IOException("The specified output_path already exists.");

            if (!File.Exists(textFileToRead))
                throw new FileNotFoundException();

            var fileToRead = "a -t7z \"" + path + "\" \"@" + textFileToRead + "\" -mx9";

            var process = new Process
            {
                EnableRaisingEvents = false,
                StartInfo =
                {
                    FileName = "C:\\Program Files\\7-Zip\\7z",
                    Arguments = fileToRead
                }
            };

            process.Start();

            if (!wait)
                return;

            process.WaitForExit();

            print_(File.Exists(path)
                ? $"Successfully created \"{path}\"."
                : $"Unsuccessfully created \"{path}\".");
        }

        public static void Pdf(Part part, string drawingSheetName, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!filePath.EndsWith(".pdf"))
                throw new InvalidOperationException("File path for PDF must end with \".pdf\".");

            if (File.Exists(filePath))
                throw new ArgumentOutOfRangeException("output_path", "PDF \"" + filePath + "\" already exists.");

            //We can use SingleOrDefault here because NX will prevent the naming of two drawing sheets the exact same string.
            var sheet = part.DrawingSheets
                            .ToArray()
                            .SingleOrDefault(drawingSheet => drawingSheet.Name == drawingSheetName)
                        ??
                        throw new ArgumentException(
                            $@"Part ""{part.Leaf}"" does not have a sheet named ""{drawingSheetName}"".",
                            "drawingSheetName");

            __display_part_ = part;
            session_.set_display_to_work();
            SetLayers();

            var pdfBuilder = part.PlotManager.CreatePrintPdfbuilder();

            using (session_.using_builder_destroyer(pdfBuilder))
            {
                pdfBuilder.Scale = 1.0;
                pdfBuilder.Size = PrintPDFBuilder.SizeOption.ScaleFactor;
                pdfBuilder.OutputText = PrintPDFBuilder.OutputTextOption.Polylines;
                pdfBuilder.Units = PrintPDFBuilder.UnitsOption.English;
                pdfBuilder.XDimension = 8.5;
                pdfBuilder.YDimension = 11.0;
                pdfBuilder.RasterImages = true;
                pdfBuilder.Colors = PrintPDFBuilder.Color.BlackOnWhite;
                pdfBuilder.Watermark = "";
                UFSession.GetUFSession().Draw.IsObjectOutOfDate(sheet.Tag, out var flag);

                if (flag)
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

        public static void Stp(string partPath, string output_path, string settings_file)
        {
            if (!output_path.EndsWith(".stp"))
                throw new InvalidOperationException("File path for STP must end with \".stp\".");

            if (File.Exists(output_path))
                throw new ArgumentOutOfRangeException("output_path", "STP \"" + output_path + "\" already exists.");

            if (!File.Exists(partPath))
                throw new FileNotFoundException("Could not find file location \"" + partPath + "\".");

            session_.find_or_open(partPath);

            var stepCreator = Session.GetSession().DexManager.CreateStepCreator();

            using (session_.using_builder_destroyer(stepCreator))
            {
                stepCreator.ExportAs = StepCreator.ExportAsOption.Ap214;
                stepCreator.SettingsFile = settings_file;
                stepCreator.ObjectTypes.Solids = true;
                stepCreator.OutputFile = output_path;
                stepCreator.BsplineTol = 0.0254;
                stepCreator.ObjectTypes.Annotations = true;
                stepCreator.ExportFrom = StepCreator.ExportFromOption.ExistingPart;
                stepCreator.InputFile = partPath;
                stepCreator.FileSaveFlag = false;
                stepCreator.LayerMask = "1, 96";
                stepCreator.ProcessHoldFlag = true;
                stepCreator.Commit();
            }

            var switchFilePath = output_path.Replace(".stp", ".log");

            if (File.Exists(switchFilePath))
                File.Delete(switchFilePath);
        }

        public static void Dwg(string partPath, string drawingSheetName, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (File.Exists(filePath))
                throw new ArgumentOutOfRangeException("output_path", "DWG \"" + filePath + "\" already exists.");

            var part = session_.find_or_open(partPath);

            var sheet = part.DrawingSheets
                            .ToArray()
                            .SingleOrDefault(drawingSheet => drawingSheet.Name == drawingSheetName)
                        ??
                        throw new ArgumentException(
                            $"Part \"{part.Leaf}\" does not have a sheet named \"{drawingSheetName}\".",
                            "drawingSheetName");

            UFSession.GetUFSession().Draw.IsObjectOutOfDate(sheet.Tag, out var flag);

            if (flag)
            {
                SetLayers();
                UFSession.GetUFSession().Draw.UpdOutOfDateViews(sheet.Tag);
                part.__Save();
            }

            var dxfdwgCreator1 = session_.DexManager.CreateDxfdwgCreator();
            using (session_.using_builder_destroyer(dxfdwgCreator1))
            {
                dxfdwgCreator1.ExportData = DxfdwgCreator.ExportDataOption.Drawing;
                dxfdwgCreator1.AutoCADRevision = DxfdwgCreator.AutoCADRevisionOptions.R2004;
                dxfdwgCreator1.ViewEditMode = true;
                dxfdwgCreator1.FlattenAssembly = true;
                dxfdwgCreator1.SettingsFile = "C:\\Program Files\\Siemens\\NX 11.0\\dxfdwg\\dxfdwg.def";
                dxfdwgCreator1.ExportFrom = DxfdwgCreator.ExportFromOption.ExistingPart;
                dxfdwgCreator1.OutputFileType = DxfdwgCreator.OutputFileTypeOption.Dwg;
                dxfdwgCreator1.ObjectTypes.Curves = true;
                dxfdwgCreator1.ObjectTypes.Annotations = true;
                dxfdwgCreator1.ObjectTypes.Structures = true;
                dxfdwgCreator1.FlattenAssembly = false;
                dxfdwgCreator1.ExportData = DxfdwgCreator.ExportDataOption.Drawing;
                dxfdwgCreator1.InputFile = part.FullPath;
                dxfdwgCreator1.ProcessHoldFlag = true;
                dxfdwgCreator1.OutputFile = filePath;
                dxfdwgCreator1.WidthFactorMode = DxfdwgCreator.WidthfactorMethodOptions.AutomaticCalculation;
                dxfdwgCreator1.LayerMask = "1-256";
                dxfdwgCreator1.DrawingList = drawingSheetName;
                dxfdwgCreator1.Commit();
            }

            var switchFilePath = filePath.Replace(".dwg", ".log");

            if (File.Exists(switchFilePath))
                File.Delete(switchFilePath);
        }
    }
}