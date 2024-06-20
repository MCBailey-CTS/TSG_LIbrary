using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Attributes;
using static NXOpen.Session;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_strip_ref_setter)]
    public class StripRefSetter : _UFunc
    {
        public override void execute()
        {
            print_(ufunc_rev_name);

            if (__display_part_ is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            // Need to remove the objects from the reference sets first.
            __SetUndoMark(MarkVisibility.Visible, "StripRefSetter");

            Part display = __display_part_;

            string leaf = display.Leaf.ToLower();

            if (!leaf.EndsWith("-strip"))
            {
                print_("Strip Refsetter can only be used on a strip.");
                return;
            }

            NXObject[] layer100Components = display.ComponentAssembly.RootComponent
                .GetChildren()
                .Where(child => child.Layer == 100)
                .Cast<NXObject>()
                .ToArray();

            NXObject[] layer100Objects = display
                .Layers.GetAllObjectsOnLayer(100)
                .Where(obj => obj is Curve || obj is Body)
                .Where(obj => !obj.IsOccurrence)
                .Concat(layer100Components)
                .ToArray();

            NXObject[] layer101Components = display.ComponentAssembly.RootComponent
                .GetChildren()
                .Where(child => child.Layer == 101)
                .Cast<NXObject>()
                .ToArray();

            NXObject[] layer101Objects = display
                .Layers.GetAllObjectsOnLayer(101)
                .Where(obj => obj is Curve || obj is Body)
                .Where(obj => !obj.IsOccurrence)
                .Concat(layer101Components)
                .ToArray();

            NXObject[] layer102Components = display.ComponentAssembly.RootComponent
                .GetChildren()
                .Where(child => child.Layer == 102)
                .Cast<NXObject>()
                .ToArray();

            NXObject[] layer102Objects = display.Layers
                .GetAllObjectsOnLayer(102)
                .Where(obj => obj is Curve || obj is Body)
                .Where(obj => !obj.IsOccurrence)
                .Concat(layer102Components)
                .ToArray();

            NXObject[] presses = display.ComponentAssembly.RootComponent.GetChildren()
                .Where(child => child.Name.ToUpper().Contains("PRESS"))
                .Cast<NXObject>()
                .ToArray();

            display.ComponentAssembly.ReplaceReferenceSetInOwners(
                "BODY_NO_SLUG",
                layer101Components
                    .Concat(layer102Components)
                    .Cast<Component>()
                    .ToArray()
            );

            display.ComponentAssembly.ReplaceReferenceSetInOwners(
                "BODY",
                layer100Components
                    .Concat(presses)
                    .Cast<Component>()
                    .ToArray()
            );

            if (layer100Objects.Length > 0)
            {
                // WORK_PARTS
                const string WORK_PARTS = nameof(WORK_PARTS);
                ReferenceSet work_parts_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == WORK_PARTS);
                if (work_parts_refset is null)
                {
                    work_parts_refset = display.CreateReferenceSet();
                    work_parts_refset.SetName(WORK_PARTS);
                }

                work_parts_refset.AddObjectsToReferenceSet(layer100Objects);
            }

            if (layer101Objects.Length > 0)
            {
                // LIFTED_PARTS
                const string LIFTED_PARTS = nameof(LIFTED_PARTS);
                ReferenceSet lifted_parts_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == LIFTED_PARTS);
                if (lifted_parts_refset is null)
                {
                    lifted_parts_refset = display.CreateReferenceSet();
                    lifted_parts_refset.SetName(LIFTED_PARTS);
                }

                lifted_parts_refset
                    .AddObjectsToReferenceSet(
                        layer101Objects); // set children to body-with-no-slugs reference set before adding
            }

            if (layer100Objects.Length > 0 || layer100Objects.Length > 0 || layer102Objects.Length > 0 ||
                presses.Length > 0)
            {
                // ALL_WITH_PRESSES
                const string ALL_WITH_PRESSES = nameof(ALL_WITH_PRESSES);
                ReferenceSet all_with_presses_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == ALL_WITH_PRESSES);
                if (all_with_presses_refset is null)
                {
                    all_with_presses_refset = display.CreateReferenceSet();
                    all_with_presses_refset.SetName(ALL_WITH_PRESSES);
                }

                all_with_presses_refset
                    .AddObjectsToReferenceSet(layer100Objects); // set children to body reference set before adding
                all_with_presses_refset
                    .AddObjectsToReferenceSet(
                        layer101Objects); // set children to body-no-slugs reference set before adding
                all_with_presses_refset
                    .AddObjectsToReferenceSet(
                        layer102Objects); // set children to body-no-slugs reference set before adding
                all_with_presses_refset
                    .AddObjectsToReferenceSet(presses); // set children to body reference set before adding
                Component[] grippers = display.ComponentAssembly.RootComponent
                    .GetChildren()
                    .Where(child => child.Layer == 235)
                    .ToArray();
                display.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", grippers);
                all_with_presses_refset.AddObjectsToReferenceSet(grippers);
            }

            if (layer100Objects.Length > 0 || layer100Objects.Length > 0 || layer102Objects.Length > 0)
            {
                // ALL_PARTS
                const string ALL_PARTS = nameof(ALL_PARTS);
                ReferenceSet all_parts_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == ALL_PARTS);
                if (all_parts_refset is null)
                {
                    all_parts_refset = display.CreateReferenceSet();
                    all_parts_refset.SetName(ALL_PARTS);
                }

                all_parts_refset
                    .AddObjectsToReferenceSet(layer100Objects); // set children to body reference set before adding
                all_parts_refset
                    .AddObjectsToReferenceSet(
                        layer101Objects); // set children to body-no-slugs reference set before adding
                all_parts_refset
                    .AddObjectsToReferenceSet(
                        layer102Objects); // set children to body-no-slugs reference set before adding
            }

            // TRANSFER_PARTS
            if (layer102Components.Length > 0 && leaf.EndsWith("-900-strip"))
            {
                const string TRANSFER_PARTS = nameof(TRANSFER_PARTS);
                ReferenceSet transfer_parts_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == TRANSFER_PARTS);
                if (transfer_parts_refset is null)
                {
                    transfer_parts_refset = display.CreateReferenceSet();
                    transfer_parts_refset.SetName(TRANSFER_PARTS);
                }

                transfer_parts_refset
                    .AddObjectsToReferenceSet(
                        layer102Components); // set children to body-no-slugs reference set before adding
            }

            if (!leaf.EndsWith("-010-strip") &&
                (layer100Objects.Length > 0 || layer100Objects.Length > 0 || layer102Objects.Length > 0))
            {
                // PROG_ONLY_WORK
                const string PROG_ONLY_WORK = nameof(PROG_ONLY_WORK);
                ReferenceSet prog_only_work_refset =
                    display.GetAllReferenceSets().SingleOrDefault(set => set.Name == PROG_ONLY_WORK);
                if (prog_only_work_refset is null)
                {
                    prog_only_work_refset = display.CreateReferenceSet();
                    prog_only_work_refset.SetName(PROG_ONLY_WORK);
                }

                prog_only_work_refset.AddObjectsToReferenceSet(layer100Objects
                    .Where(obj => !obj.Name.StartsWith("T"))
                    .ToArray()); // set children to body reference set before adding

                prog_only_work_refset.AddObjectsToReferenceSet(layer101Objects
                    .Where(obj => !obj.Name.StartsWith("T"))
                    .ToArray()); // set children to body-no-slugs reference set before adding

                prog_only_work_refset.AddObjectsToReferenceSet(layer102Objects
                    .Where(obj => !obj.Name.StartsWith("T"))
                    .ToArray()); // set children to body-no-slugs reference set before adding
            }

            prompt_("Complete");
        }
    }
}