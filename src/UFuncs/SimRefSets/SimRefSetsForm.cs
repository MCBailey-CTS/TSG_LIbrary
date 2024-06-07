using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions.Extensions_;
using static NXOpen.Session;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_sim_ref_sets)]
    [RevisionLog("Sim Ref Sets")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS")]
    [RevisionEntry("1.2", "2017", "09", "08")]
    [Revision("1.2.1", "Added validation check")]
    [RevisionEntry("1.3", "2017", "10", "26")]
    [Revision("1.3.1", "Fixed bug where the Master-Tip reference sets were being delete from the simulation file.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class SimRefSetsForm : _UFuncForm
    {
        public SimRefSetsForm()
        {
            InitializeComponent();
        }

        private void Button_Selected(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                try
                {
                    SetUndoMark(MarkVisibility.Visible, "SimRefSetMaker");

                    Component[] nxComponents;

                    if (btnAddAll == sender)
                        nxComponents = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                            .Where(component => !component.DisplayName.Contains("Master"))
                            .ToArray();
                    else if (btnAddTo == sender)
                        nxComponents = Selection.SelectManyComponents().ToArray();
                    else
                        return;

                    if (nxComponents.Length == 0)
                        return;

                    if (chkDelete.Checked)
                    {
                        var listOfRefs = __display_part_.GetAllReferenceSets()
                            // Revision • 1.3 – 2017 – 10 – 26 
                            .Where(set => !set.Name.ToUpper().Contains("MASTER"))
                            .ToList();

                        listOfRefs.ForEach(set => print_(set.Name));
                        listOfRefs.ForEach(__display_part_.DeleteReferenceSet);
                    }

                    MakeRefSets(nxComponents);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
            }
        }


        internal static void MakeRefSets(params Component[] nxComponents)
        {
            if (nxComponents.Length == 0)
                throw new Exception("No ref sets made for 0 components");

            var validChildrenDisplayNames = __display_part_.ComponentAssembly.RootComponent
                .GetChildren()
                .Where(__c => !__c.IsSuppressed)
                .Select(__c => __c.Name)
                .ToArray();

            foreach (var nxComp in nxComponents)
                try
                {
                    // Checks to make sure that there aren't two components with the same "Component Name".
                    var nxCompName = nxComp.Name;
                    var componentsWithSameName = validChildrenDisplayNames.Where(s => nxCompName == s).ToArray();

                    switch (componentsWithSameName.Length)
                    {
                        case 0:
                            //This should never be reached.
                            throw new Exception("Thrown from switch statement.");
                        case 1:
                            break;
                        default:
                            // This occurs when they select a component who has the same name as a different component under the Simulation Part.
                            throw new InvalidDataException("You have more than one component named " + nxCompName +
                                                           ". Reference set will not be made.");
                    }

                    // Gets the name for the to be created reference set.
                    var newRefSetName = GetNewReferenceSetName(nxComp);

                    // If null then it means the "Component.Name" of nxComp was weird and was unable to determine what the name of the reference set should be called.
                    if (newRefSetName is null)
                        continue;

                    // Checks to make sure that there aren't two components with the same "Component Name".
                    var alreadyNamedRefSet = __display_part_
                        .GetAllReferenceSets()
                        .SingleOrDefault(set => set.Name == newRefSetName);

                    // If this true then that means they have selected a component whose "Component.Name"
                    // is already the name of a reference set in the Simulation file.
                    // User will be prompted to choose whether or not they want to delete the existing
                    // reference set and create a new one, or just continue on and not override the reference set.
                    if (!(alreadyNamedRefSet is null))
                    {
                        var result = MessageBox.Show(
                            $@"A reference set named {newRefSetName} already exists. Would you like to override it?",
                            @"Warning",
                            MessageBoxButtons.YesNo);

                        switch (result)
                        {
                            case DialogResult.Yes:
                                session_.delete_objects(alreadyNamedRefSet);
                                if (__work_part_.GetAllReferenceSets().Any(set => set.Name == newRefSetName))
                                    print_("Reference Set " + newRefSetName + " was unable to be deleted");
                                else
                                    print_("Reference Set " + newRefSetName + " was successfully deleted.");
                                break;
                            case DialogResult.No:
                                print_("Reference Set " + newRefSetName + " was not overridden.");
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }


                    var referenceSet = __display_part_.__FindReferenceSetOrNull(newRefSetName);

                    if (referenceSet is null)
                    {
                        referenceSet = __display_part_.CreateReferenceSet();
                        referenceSet.SetName(newRefSetName);
                    }

                    referenceSet.AddObjectsToReferenceSet(new NXObject[] { nxComp });

                    PrintReferenceSetResult(newRefSetName);
                }

                catch (Exception ex)
                {
                    ex._PrintException();
                }
        }

        private static string GetNewReferenceSetName(Component nxComp)
        {
            if (nxComp.Name.ToUpper().Contains("MASTER"))
                return nxComp.Name;

            var firstHyphen = nxComp.Name.IndexOf('-');

            if (firstHyphen >= 0)
                return nxComp.Name.Substring(firstHyphen + 1);

            print_(
                $"Could not find a hyphen in the \"Component.Name\" of {nxComp.Name}. Reference Set {nxComp.Name} will not be made.");
            return null;
        }

        /// <summary>
        ///     Prints whether or not a reference set with the given name was successfully created.
        /// </summary>
        /// <param name="newRefSetName">The name of the reference set.</param>
        private static void PrintReferenceSetResult(string newRefSetName)
        {
            if (__work_part_.GetAllReferenceSets().Any(set => set.Name == newRefSetName))
                print_($"Reference Set {newRefSetName} was successfully created.");
            else
                print_($"Reference Set {newRefSetName} was not able to be created.");
        }
    }
}