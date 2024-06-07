using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using TSG_Library.Properties;
using TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.UFuncs
{
    //[UFunc(nameof(_UFunc.ufunc_design_check))]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11")]
    [RevisionEntry("1.01", "2017", "08", "22")]
    [Revision("", "Signed so it will run outside of CTS")]
    [RevisionEntry("1.02", "2017", "09", "08")]
    [Revision("", "Added validation check")]
    [RevisionEntry("1.03", "2018", "01", "11")]
    [Revision("", "Added validation of burnouts.")]
    [Revision("", "Material must also equal(HRS PLT, 4140 PLT, or 4140 PH PLT).")]
    [Revision("", "Added validation of castings.")]
    [Revision("",
        "A casting cannot have a child loaded from the LiftLug folder in 0Library and have that child set to Entire Part.")]
    [RevisionEntry("2.0", "2018", "01", "22")]
    [Revision("2.0.1", "Restructured code into an easy to implement inheritance structure.")]
    [Revision("2.0.2", "Now it will be very easy to add a new Check and quickly add it.")]
    [Revision("2.0.3", "All checks derive from the validator class.")]
    [Revision("2.0.4",
        "They will have a PreCheck(a check that tests every part in the assembly to see if the part is valid for this kind of test). ")]
    [Revision("2.0.5", "Then there will be an actual CheckPart that checks the actual criteria.")]
    [Revision("2.0.6", "Added validation of the Position Override property.")]
    [Revision("2.0.7", "Added validation of the Fix at Current Time Stamp.")]
    [Revision("2.0.8",
        "The results not only printed to the InfoWindow, but it will also be written to a file titled the (__display_part_.Leaf-Desgin-Check.txt) and placed in the current Job Folder stock check off list.")]
    [RevisionEntry("2.1", "2018", "01", "30")]
    [Revision("2.1.1",
        "Fixed issue where ValidateCBoreDepths was throwing an NXException with Error Code 640001. The exception was being thrown because in the PreCheck() method of CBoreDepths, the program iterates through the part and finds all the features that are linked bodies from other parts. Well it would throw an exception if it got to a linked body, whose owning part was not loaded and that linked bodies owning part could not be found. In a normal case it would just load the part. But if the linked bodies defining part was no longer there, because it was deleted or renamed, then the program would throw an exception.")]
    [Revision("2.1.2",
        "This opened up another issue. What happens when an exception is thrown during the PreCheck phase? Well now if any exception is thrown from any PreCheck() method the, exception will be caught, and the user will see in the results that a Part threw an exception from a pre check test and the user will notified of the Part and the error message.")]
    [Revision("2.1.3", "Added a check for Half Moons on castings.")]
    [Revision("2.1.4",
        "Checks to make sure that every casting has 4 half-moons. 2 named \"HALFMOON1\" and 2 \"HALFMOON2\". And that those faces are aligned properly.")]
    [Revision("2.1.5",
        "Added to ValidateComponentNames. Component names will be allowed to now have one capital letter after the name. This will take care of the issue where Nitros are showing up in the results.")]
    [RevisionEntry("2.2", "2018", "02", "06")]
    [Revision("2.2.01", "Added the full feature name back into the results for “ValidateFixAtCurrentTimeStamps”.")]
    [Revision("2.2.03", "Therefore the name should be back to “Linked Body (“Feature Number”) “.")]
    [Revision("2.2.04",
        "Adjusted the results for all validators.If a validator could not find any parts in the current displayed assembly, instead of printing out the meaningless results, it just prints out the validators name, followed by the “Did not find any parts valid for this check in the current displayed assembly”.")]
    [Revision("2.2.05",
        "If a validator did find valid parts, and all the found parts passed. The result will simply just be the name of the validator, followed by how many parts passed. ")]
    [Revision("2.2.06", "Added the actual Job Folder path to the “Description” message for “ValidateFolderLocations”.")]
    [Revision("2.2.07", "Changed the “PreCheck” in the base “Validator” virtual instead of being abstract.")]
    [Revision("2.2.08",
        "Now the base “PreCheck” method will return true for any part.Meaning all parts will be tested in the derived type of “Validator”. If there is a PreCheck test that needs to be used, then you must override the method in the derived type.")]
    [Revision("2.2.09", "Added a new functionality….ValidateInterpartExpressions.")]
    [Revision("2.2.10",
        "This check goes through every part in your current assembly and gets a list of Expressions in the Part that have InterPartData.If the part that the expression is referencing is not located in your current job folder, then the part will fail the test and report it.")]
    [Revision("2.2.11",
        "For the Burnout Validator, when a Burnout fails(aka...their material attribute is incorrect for a burnout), the results will also contain the actual material attribute of the failed part.")]
    [Revision("2.2.12", "Changed all parameters in the base “Validator” to use Snap.NX.")]
    [Revision("2.2.13",
        "Now for any validator that derives from “ValidatePartOccurrences” will ignore any part occurrence that is suppressed.")]
    [Revision("2.2.14", "Added an assembly path to “ValidateTimeStamps.")]
    [Revision("2.2.15",
        "Added a dashed line before and after the result of each check to make it easier for the user to read the results.")]
    [Revision("2.2.16", "Edited the error message on the HalfMoon check.")]
    [Revision("2.2.17", "It now will say: “Could not find any named half moons”.")]
    [Revision("2.2.18",
        "Edited how the results are displayed for a Validator that is derived from the “ValidatePartOccurrence”.  It will be displayed like this now.")]
    [Revision("2.2.19", "DisplayName/Leaf of the part.")]
    [Revision("2.2.20", "The name of the component that is a part occurrence of the part.")]
    [Revision("2.2.21", "The display names of the parts used to show the assembly path to find the component.")]
    [Revision("2.2.22", "Added a space in the text file to separate the header and the rest of Design Check results.")]
    [RevisionEntry("3.0", "2018", "02", "09")]
    [Revision("3.0.1",
        "Added a form so that the user can individually select which checks to run and on which components to check.")]
    [Revision("3.0.2", "Will only write text file to stockcheckofflists if the “All Checks” textbox is checked on.")]
    [Revision("3.0.3", "All check boxes will be defaulted on.")]
    [RevisionEntry("3.1", "2018", "02", "20")]
    [Revision("3.1.1", "Not entirely sure if this fixed it, but so far it seems to be working. ")]
    [Revision("3.1.2", "In the ValidateTimeStamps, we are now filtering out any Linked Body that is suppressed.")]
    [Revision("3.1.3", "We are also checking the source part of the linked body to make sure that it is fully loaded.")]
    [Revision("3.1.4",
        "If a source part of a linked body is found to be not fully loaded, then the program gives the user a message in the design check results and continues on as normal.")]
    [RevisionEntry("3.2", "2018", "03", "08")]
    [Revision("3.2.1",
        "Fixed bug that would cause the Session of NX to crash if design check was ran a lot on a session.")]
    [Revision("3.2.2",
        "Basically I was checking the timestamps of linked bodies through an ExtractFaceBuilder, but I was never Destroying the builder after each use.")]
    [Revision("3.2.3", "Thus running out of memory and causing a stack overflow error. ")]
    [RevisionEntry("4.0", "2018", "06", "06")]
    [Revision("4.0.01", "Design Check has been completely redesigned.")]
    [Revision("4.0.02",
        "Instead of printing to the InfoWindow, all the information is pushed out to a form with a TreeView. ")]
    [Revision("4.0.03",
        "The tree view will allow the user to perform actions on the actual treenodes that will trigger events in the current Displayed Assembly.")]
    [Revision("4.0.04", "AssemblyRefSetMisMatch")]
    [Revision("4.0.05",
        "Checks to make sure that won’t be any parts that are translated with the wrong reference set when converting to.stp.")]
    [Revision("4.0.06", "	BrokenLinks")]
    [Revision("4.0.07", "Checks for Linked Bodies in the current assembly that are broken.")]
    [Revision("4.0.08", "Burnout")]
    [Revision("4.0.09",
        "Checks to make sure that any part that is a burnout has a burnout drawing and a valid value for material attribute.")]
    [Revision("4.0.10", "BushingsAndPins")]
    [Revision("4.0.11", "Makes sure that guide bushings and pins are coming from the same vendor.")]
    [Revision("4.0.12", "CastingChildren")]
    [Revision("4.0.13", "Makes sure that and child of a casting is not set to the ReferenceSet of Entire Part.")]
    [Revision("4.0.14", "CastingHalfMoons")]
    [Revision("4.0.15",
        "Makes sure that a casting has properly named half moon faces and that the faces are aligned properly.")]
    [Revision("4.0.16", "CBoreDepths")]
    [Revision("4.0.17", "Checks to make sure that all CBore holes are at the proper level.")]
    [Revision("4.0.18", "Also helps to find fasteners that have blown out the side of hole.")]
    [Revision("4.0.19", "ComponentNames")]
    [Revision("4.0.20", "Ensures that all details are named properly.")]
    [Revision("4.0.21", "DescriptionNXAttribute")]
    [Revision("4.0.22", "Makes sure that all part occurrences of a given part have matching Description values.")]
    [Revision("4.0.23", "Dimensions")]
    [Revision("4.0.24", "Checks for non-associative dimensions.")]
    [Revision("4.0.25", "FolderLocations")]
    [Revision("4.0.26", "Makes sure that all parts in the assembly are being loaded from valid locations.")]
    [Revision("4.0.27", "FullyLoadAssembly")]
    [Revision("4.0.28", "Makes sure that all components in the assembly are loaded.")]
    [Revision("4.0.29", "InterpartExpressions")]
    [Revision("4.0.30", "Makes sure that all expressions come from parts that are within in the current job folder.")]
    [Revision("4.0.31", "JigJacks")]
    [Revision("4.0.32", "Ensures that all parts that have JigJacks have them on a 1-inch grid.")]
    [Revision("4.0.33", "LinkBodyParents")]
    [Revision("4.0.34", "Checks to make sure that the Link Status of every Linked Body is “Up to date”.")]
    [Revision("4.0.35", "PositionOverride")]
    [Revision("4.0.36", "Makes sure that every component in the assembly does not have position override turned on.")]
    [Revision("4.0.37", "SizeDescription")]
    [Revision("4.0.38",
        "Checks to make sure that all details size description actually matches the measurements of the solid on layer 1.")]
    [Revision("4.0.39", "TimeStamps")]
    [Revision("4.0.40",
        "Checks to make sure that there aren’t any linked bodies in the assembly that have the time stamp checked on.")]
    [Revision("4.0.41", "WireTapNotes.")]
    [Revision("4.0.42", "Makes sure that anything with the attribute of WTN = YES, that their DETAIL NAME = TRIM.")]
    [RevisionEntry("5.0", "2021", "03", "24")]
    [Revision("5.0.1", "Created for NX.")]
    [Revision("5.0.2", "Fixed issue where the re-run checks button didn't work correctly.")]
    [Revision("5.0.3",
        "LinkedBodyParents will now ignore Linked Bodies that are broken. They will still be flagged by the BrokenLinks check.")]
    [Revision("5.0.4", "Fixed issue where design check would run again when attempting to unload the program.")]
    [Revision("5.0.5", "Fixed issue where the program could not find the stocklist.")]
    [Revision("5.0.6",
        "When check is being initialized, the program will now check to make sure the assembly is fully loaded.")]
    [Revision("5.0.7",
        "Any part that is unloaded and not under the simulation file, will be flagged and will require the user to run it before the design check can take place.")]
    [Revision("5.0.8",
        "Also it will make sure that all parts not under the simulation file are not only partially loaded.")]
    [Revision("5.0.9", "If a partially loaded part is found, it will be fully loaded automatically.")]
    [RevisionEntry("5.1", "2021", "05", "27")]
    [Revision("5.1.1", "The ConceptControlFile now points to \"U:\\nxFiles\\UfuncFiles\\ConceptControlFile.ucf\"")]
    [RevisionEntry("5.2", "2021", "06", "14")]
    [Revision("5.2.1", "Added a new check: SmartRevisions")]
    [Revision("5.2.2",
        "If a detail has library atrribute whose value start with Smart, if must have a REVISION attribute. Else it will fail.")]
    [Revision("5.2.3",
        "The revision value will be matched to the revison based on the part used to create it in 0Library.")]
    [Revision("5.2.4", "If the values differ then the part will fail.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class DesignCheckForm : Form
    {
        public DesignCheckForm()
        {
            InitializeComponent();
        }


        public void Execute(IDesignCheck[] checkers, params Component[] components_to_check)
        {
            var unloaded_components = components_to_check.Where(__c => !(__c.Prototype is Part)).ToArray();

            var loaded_parts = components_to_check.Select(__c => __c.Prototype)
                .OfType<Part>()
                .Distinct()
                .ToArray();

            foreach (var check in checkers)
                try
                {
                    var check_node = new TreeNode(check.GetType().Name) { Tag = check };
                    var passed_node = new TreeNode("Passed");
                    var failed_node = new TreeNode("Failed");
                    var ignored_node = new TreeNode("Ignored");
                    var exceptions_node = new TreeNode("Exceptions");
                    check_node.Nodes.Add(failed_node);
                    check_node.Nodes.Add(ignored_node);
                    check_node.Nodes.Add(passed_node);
                    check_node.Nodes.Add(exceptions_node);

                    foreach (var part in loaded_parts)
                        try
                        {
                            if (!check.IsPartValidForCheck(part, out var message))
                            {
                                ignored_node.Nodes.Add(part.__TreeNode());
                                continue;
                            }

                            if (check.PerformCheck(part, out var result_node))
                                passed_node.Nodes.Add(result_node);
                            else
                                failed_node.Nodes.Add(result_node);

                            //var objects_to_test = check.Get

                            // Need to change PerformCheck to return a bool, and a message.

                            // create separate static method to use.

                            //check.PerformCheck(part);

                            //print_("1");

                            //check_node.Nodes.Add(check.PerformCheck(part));
                        }
                        catch (Exception ex)
                        {
                            exceptions_node.Nodes.Add(part.__TreeNode());
                            ex._PrintException();
                        }

                    check_node.Text = $"{check_node.Text} -> P: {passed_node.Nodes.Count}, " +
                                      $"I: {ignored_node.Nodes.Count}, " +
                                      $"F: {failed_node.Nodes.Count}, " +
                                      $"E: {exceptions_node.Nodes.Count}";

                    _treeView.Nodes.Add(check_node);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
        }


        private void DesignCheckForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.design_check_window_location;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
#pragma warning disable CS0612 // Type or member is obsolete
                IDesignCheck[] checkers =
                {
                    new BrokenLinks(),
                    new Burnouts(),
                    new BushingsAndPins(),
                    new CastingChildren(),
                    new CastingHalfMoons(),
                    new CBoreDepths(),
                    new ComponentNames(),
                    new DescriptionNXAttribute(),
                    new Dimensions(),
                    new FolderLocations(),
                    new InterpartExpressions(),
                    new JigJacks(),
                    new LinkedBodyParents(),
                    new PositionOverride(),
                    new SizeDescription(),
                    new SmartRevisions(),
                    new SuppressedFeatures(),
                    new TimeStamps(),
                    new WireTaperNotes()
                };
#pragma warning restore CS0612 // Type or member is obsolete


                Execute(checkers, __display_part_.ComponentAssembly.RootComponent._Descendants().ToArray());

                //TreeNode[] check_nodes = new TreeNode[checkers.Length];

                //prompt_($"Gathering Assembly info of {__display_part_.Leaf}");
                //NXOpen.Assemblies.Component[] descendants = __display_part_.ComponentAssembly.RootComponent._Descendants().ToArray();

                //for (int i = 0; i < checkers.Length; i++)
                //    try
                //    {
                //        HashSet<NXOpen.Part> checked_parts = new HashSet<NXOpen.Part>();
                //        IDesignCheck checker = checkers[i];
                //        prompt_($"Running {checker.GetType().Name}: {i + 1} of {checkers.Length}");
                //        check_nodes[i] = new TreeNode(checker.GetType().Name);

                //        foreach (NXOpen.Assemblies.Component component in descendants)
                //        {
                //            if (!(component.Prototype is NXOpen.Part prototype))
                //                continue;

                //            if (checked_parts.Contains(prototype))
                //                continue;

                //            checked_parts.Add(prototype);

                //            try
                //            {
                //                if (!checker.IsPartValidForCheck(prototype))
                //                    continue;

                //                TreeNode result_node = checker.PerformCheck(prototype);

                //                if (result_node.Nodes.Count == 0)
                //                    continue;

                //                check_nodes[i].Nodes.Add(result_node);
                //            }
                //            catch (Exception ex)
                //            {
                //                //ex._PrintException();

                //                //var exp_node = ;

                //                check_nodes[i].Nodes.Add(new TreeNode($"{prototype.Leaf} -> {ex.GetType().Name}"));

                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        ex._PrintException();
                //    }

                //_treeView.Nodes.AddRange(check_nodes);
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
            finally
            {
                stopwatch.Stop();

                Text = $"{Text}, {stopwatch.Elapsed.TotalMinutes}:{stopwatch.Elapsed.Seconds}";
            }
        }

        private void _treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                print_("double clicked11");

                if (!(e.Node.Tag is Part part))
                    return;

                //__display_part_.Acti

                __display_part_ = (Part)e.Node.Tag;
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        private void DesignCheckForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.design_check_window_location = Location;
            Settings.Default.Save();
        }

        private class Comparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var xNode = x as TreeNode;
                var yNode = y as TreeNode;
                if (ReferenceEquals(xNode, yNode)) return 0;
                return xNode is null
                    ? string.CompareOrdinal(null, yNode.Text)
                    : string.CompareOrdinal(xNode.Text, yNode?.Text);
            }
        }


        //public interface IDesignCheck
        //{
        //    System.Windows.Forms.TreeNode PerformCheck(NXOpen.Part part);

        //    bool IsPartValidForCheck(NXOpen.Part part);
        //}


        ///////////////////////////////////////////////
    }
}