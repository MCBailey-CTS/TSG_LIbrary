using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_clean_job_directory)]
    [RevisionLog("Clean Up job Directory")]
    [RevisionEntry("1.00", "2017", "06", "05")]
    [Revision("1.00.1", "Revision Log Created for NX 11")]
    [RevisionEntry("1.01", "2017", "08", "22")]
    [Revision("1.01.1", "Signed so it can be run outside of CTS")]
    [RevisionEntry("1.02", "2017", "09", "08")]
    [Revision("1.02.1", "Added validation check")]
    [RevisionEntry("2.0", "2018", "03", "22")]
    [Revision("2.0.1", "CIT – 2018 – 0008")]
    [Revision("2.0.2", "Program will now not run unless the assembly is fully loaded.")]
    [Revision("2.0.3", "Program must be ran from a valid “000” detail that is not located in the 900 folder.")]
    [Revision("2.0.3.1", "Therefore assembly holders are not allowed either.")]
    [Revision("2.0.4", "Instead of making a file cleanup folder at the base of the displayed part folder.")]
    [Revision("2.0.4.1",
        "It will now create a file cleanup folder at the base of the current displayed parts GFolder.")]
    [Revision("2.0.5", "In the folder a zip file will be created with all the parts to be cleaned from the job.")]
    [Revision("2.0.5.1", "YYYY-MM-DD-OP-#-Cleanup")]
    [RevisionEntry("2.1", "2019", "08", "28")]
    [Revision("2.1.1", "GFolder updated to allow old job number under non cts folder.")]
    [RevisionEntry("2.2", "2020", "02", "10")]
    [Revision("2.2.1",
        "Updated output folder. 6 digit jobs will now place FileCleanup folder in the Design Information folder. 4 digit jobs are unchanged.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public class CleanJobDirectory : _UFunc
    {
        public override void execute()
        {
            // Creates an instance of GFolderWithCtsNumber using the current Displayed Part as it's source.
            GFolder folder = GFolder.create(__work_part_.FullPath);

            if (folder is null)
            {
                print_("The current work part does not reside within a GFolder.");
                return;
            }

            DialogResult result = MessageBox.Show(@"Are you sure you want to run Clean Job Directory?", @"Alert",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
                return;

            Match match = Regex.Match(__display_part_.Leaf, Regex_Detail);

            if (!match.Success || match.Groups[3].Value != "000")
                throw new InvalidOperationException(
                    "Clean Job Directory must be ran from a -000 not located in a 900 folderWithCtsNumber.");

            string displayedPartOp = match.Groups[2].Value;

            switch (displayedPartOp)
            {
                case var op when op == "900":
                    throw new InvalidOperationException(
                        "Clean Job Directory must be ran from a -000 not located in a 900 folderWithCtsNumber.");
                case var op when op.Length > 3:
                    throw new InvalidOperationException(
                        "Clean Job Directory must be ran from a -000 not located in a 900 folderWithCtsNumber.");
            }

            // Does two operations here.
            // 1. It gets all the part paths that define the currently displayed assembly.
            // 2. It is also checking to make sure that the assembly is fully loaded.
            // Throws exception if assembly is not fully loaded.
            IEnumerable<string> partPathsInAssembly = Find(__display_part_.__RootComponent())
                .Select(component => component.Prototype)
                .OfType<Part>()
                .Select(part => part.FullPath);

            // Creates a hashset of the part paths in the currently displayed assembly.
            HashSet<string> hashedAssemblyPartPaths = new HashSet<string>(partPathsInAssembly);

            // Gets the directory the Displayed Part resides in.
            string displayedPartDirectory = Path.GetDirectoryName(__display_part_.FullPath);
            if (displayedPartDirectory == null) throw new DirectoryNotFoundException($"{__display_part_.FullPath}");
            string[] partFilesInDirectory =
                Directory.GetFiles(displayedPartDirectory, "*.prt", SearchOption.TopDirectoryOnly);

            // Represents the part file paths in the displayed part directory that are not in the current assembly.
            HashSet<string> fileCleanUpSet = new HashSet<string>();
            foreach (string path in partFilesInDirectory)
            {
                bool flag = hashedAssemblyPartPaths.Contains(path);
                TheUFSession.UF.PrintSyslog(
                    $"Checking if {path} is in displayed assembly. Result: {flag}.{Environment.NewLine}", false);
                if (flag) continue;
                flag = fileCleanUpSet.Add(path);
                TheUFSession.UF
                    .PrintSyslog(
                        $"{path} was {(flag ? "" : "not ")} successfully added to the file cleanup set.{Environment.NewLine}",
                        false);
            }

            if (fileCleanUpSet.Count == 0)
            {
                TheUFSession.UF
                    .PrintSyslog(
                        "Did not find any parts in current folderWithCtsNumber that are not loaded in current assembly. Program terminated.",
                        false);
                print_(
                    "Did not find any parts in current folderWithCtsNumber that are not loaded in current assembly.");
                return;
            }

            // Check for a fileCleanup folderWithCtsNumber in the current GFolderWithCtsNumber.
            const string fileCleanUp = "FileCleanup";


            string cleanupDirectory = folder.customer_number.Length == 6
                ? $"{folder.dir_design_information}\\{fileCleanUp}"
                : $"{folder.dir_job}\\{fileCleanUp}";


            bool directoryExists = Directory.Exists(cleanupDirectory);
            if (!directoryExists)
            {
                TheUFSession.UF.PrintSyslog($"Directory {cleanupDirectory} does not exist.{Environment.NewLine}",
                    false);
                TheUFSession.UF.PrintSyslog($"Creating directory {cleanupDirectory}.{Environment.NewLine}", false);
                Directory.CreateDirectory(cleanupDirectory);
                TheUFSession.UF.PrintSyslog(
                    $"Directory {cleanupDirectory} was successfully created.{Environment.NewLine}", false);
            }

            string startDirectory = $"{TodaysDate}-{displayedPartOp}";
            string zipPath;
            for (int i = 0;; i++)
            {
                string tempPath = $"{cleanupDirectory}\\{startDirectory}-{i}-Cleanup.7z";
                if (File.Exists(tempPath)) continue;
                zipPath = tempPath;
                break;
            }

            foreach (string path in fileCleanUpSet)
                TheUFSession.UF.PrintSyslog($"File to be zipped: {path}.{Environment.NewLine}", false);

            TheUFSession.UF.PrintSyslog($"Creating zip file {zipPath}.{Environment.NewLine}", false);
            Export.SevenZip(zipPath, true, fileCleanUpSet.ToArray());

            foreach (string path in fileCleanUpSet)
            {
                TheUFSession.UF.PrintSyslog($"Deleting file: {path}.{Environment.NewLine}", false);
                File.Delete(path);
            }
        }

        private static IEnumerable<Component> Find(Component snapComp)
        {
            if (snapComp is null)
                throw new ArgumentNullException(nameof(snapComp));

            Component nxComponent = snapComp;

            if (!(nxComponent.Prototype is Part) && !nxComponent.IsSuppressed)
                throw new ArgumentException(
                    $@"Please fully load your assembly. {snapComp.DisplayName}{snapComp.OwningComponent.DisplayName} .",
                    nameof(snapComp));

            if (nxComponent.IsSuppressed)
                yield break;

            yield return snapComp;

            foreach (Component child in snapComp.GetChildren())
            foreach (Component descendant in Find(child))
                yield return descendant;
        }
    }
}