using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using static TSG_Library.Extensions.__Extensions_;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.Utilities
{
    public class SimDataDeletion
    {
        public const string _simActive = "P:\\CTS_SIM\\Active";

        public static string SimActive { get; } = _simActive;

        public void Execute()
        {
            var selComponents = Selection.SelectManyComponents().ToList();

            if(selComponents.Count <= 0)
                return;

            var filesToDelete = new HashSet<string>();

            var selectedDisplayNames = new HashSet<string>(selComponents.Select(component => component.DisplayName));

            foreach (var selectedDisplayName in selectedDisplayNames)

                if(selectedDisplayName.ToLower().Contains("master") ||
                   selectedDisplayName.ToLower().Contains("history"))
                    print_($"Deleting the {selectedDisplayName} is forbidden.");


            var folder = GFolder.create(__work_part_.FullPath);

            var currentDisplayNameJob = __display_part_.Leaf.Replace("-simulation", "");

            // This gets all the files to delete that are located within the JobFolder on the GDrive.
            foreach (var file in Directory.EnumerateFiles(folder.dir_job, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);

                if(!selectedDisplayNames.Contains(fileName))
                    continue;

                filesToDelete.Add(file);
            }

            var simDir = $"{SimActive}\\{Path.GetFileNameWithoutExtension(folder.dir_job)}";

            var dictionary = new Dictionary<string, string>();

            foreach (var displayName in selectedDisplayNames)
                switch (folder.customer_number.Length)
                {
                    case 6:
                        var tsgMatch = Regex.Match(displayName, "-(?<tsgLevel>tsg\\d+)");

                        if(!tsgMatch.Success)
                            continue;

                        dictionary.Add(displayName, $"{simDir}-{tsgMatch.Groups["tsgLevel"].Value}");
                        continue;
                    default:

                        var ecMatch = Regex.Match(displayName, "-(?<engineeringChange>5[0-9]{2})[-]*");

                        if(ecMatch.Success)
                        {
                            dictionary.Add(displayName, $"{simDir} {ecMatch.Groups["engineeringChange"].Value}");
                            continue;
                        }

                        dictionary.Add(displayName, simDir);
                        continue;
                }

            foreach (var pair in dictionary)
            foreach (var file in Directory.EnumerateFiles(pair.Value, "*", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);

                if(fileName == null)
                    continue;

                if(fileName != pair.Key)
                    continue;

                filesToDelete.Add(file);
            }


            var simDeleteConfirm = new SimDataDeleteConfirm(filesToDelete);

            switch (simDeleteConfirm.ShowDialog())
            {
                case DialogResult.OK:
                    DeleteFiles(filesToDelete);

                    CloseAndDelete(selComponents);

                    __display_part_.__Save();
                    break;
                default:
                    return;
            }

            //foreach (var file in filesToDelete)
            //    print_($"Deleted file: {file}");
        }

        private static void DeleteFiles(IEnumerable<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
                File.Delete(file);
        }

        private static void CloseAndDelete(List<Component> selComponents)
        {
            foreach (var comp in selComponents.Select(__c => __c))
            {
                if(comp.Prototype is Part part)
                    part.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);

                session_.__DeleteObjects(comp);
            }
        }
    }
}