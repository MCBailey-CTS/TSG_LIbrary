using System.Linq;
using System.Windows.Forms;
using MoreLinq.Extensions;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_delete_layer_50)]
    internal class DeleteLayer50 : _UFunc
    {
        public override void execute()
        {
            if (Session.GetSession().Parts.Display is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Question?",
                "Are you sure you want to delete all bodies on layer 50-55 in the current assembly",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
                return;

            using (session_.__UsingSuppressDisplay())
            {
                Part part = __work_part_;

                Component[] comps = __work_part_.ComponentAssembly.RootComponent
                    .__Descendants()
                    .Where(__c => !__c.IsSuppressed)
                    .Where(__c => __c.__IsLoaded())
                    .DistinctBy(__c => __c)
                    .ToArray();

                Component[] components = comps.DistinctBy(__c => __c.DisplayName)
                    .ToArray();

                bool foundBodies = false;

                string message = "";

                foreach (Component component in components)
                {
                    if (component.IsSuppressed)
                        continue;

                    if (!component.__IsLoaded())
                        continue;

                    __display_part_ = component.__Prototype();

                    Body[] bodiesToDelete = __work_part_.Bodies
                        .ToArray()
                        .Where(body => body.Layer >= 50
                                       && body.Layer <= 55
                                       && body.Color == 7
                                       && body.LineFont == DisplayableObject.ObjectFont.Phantom)
                        .ToArray();

                    if (bodiesToDelete.Length <= 0)
                        continue;

                    foundBodies = true;

                    message += $"{component.DisplayName}: {bodiesToDelete.Length}\n";

                    session_.__DeleteObjects(bodiesToDelete);
                }

                if (foundBodies)
                {
                    print_("Deleted layer 50 bodies off of these components.");
                    print_(message);
                }
                else
                {
                    print_(
                        "No solids on layers 50-55, with white coloring, and phantom lines found in current assembly.\n");
                }

                __display_part_ = part;
            }
        }
    }
}