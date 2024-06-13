using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Exceptions;
using TSG_Library.Extensions;
using TSG_Library.Forms;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.Selection;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs.AssemblyWavelink
{
    internal class __UFunc__ : _UFunc
    {
        /// <summary>Returns true if it is not an assembly holder.</summary>
        public static Predicate<Component> IsNotAssemblyHolder_LinkNoAssemblies =
            snapComponent => !IsAssemblyHolderOverride_LinkNoAssemblies(snapComponent);

        ///// <summary>
        ///// Determines if <paramref name="snapComp"/> has a subtool reference set.
        ///// </summary>
        ///// <param name="snapComp">The Snap Component</param>
        ///// <returns>Returns true if it has a subtool reference set, false otherwise.</returns>
        //public static bool HasSubToolRefset(NXOpen.Assemblies.Component snapComp)
        //    =>
        //    snapComp.__Prototype()
        //    .GetAllReferenceSets()
        //    .Any(referenceSet => SubToolNames.Contains(referenceSet.Name));

        //public static IEnumerable<string> SubToolNames => new[] { "SUBTOOL", "SUB_TOOL", "SUB TOOL", "SUB-TOOL" };

        public static Predicate<Component> IsNotAssemblyHolder_LinkSubTool =
            snapComponent => !IsAssemblyHolderOverride_LinkSubTool(snapComponent);

        private static readonly Predicate<Body> WaveLinkBooleanTargetFilter =
            body
                =>
                body.IsOccurrence
                && body.OwningComponent != null
                && IsNotAssemblyHolder_LinkNoAssemblies(body.OwningComponent)
                && !body.OwningComponent.__IsFastener();

        public override void execute()
        {
            new Form().Show();
        }

        public static void WavelinkSubtool()
        {
            //using (session_.using_form_show_hide(this))
            //{
            Body[] targets = Selection.SelectManySolidBodies();

            if (targets.Length == 0)
                return;

            Component[] tools = Selection.SelectManyComponents();

            if (tools.Length == 0)
                return;

            foreach (Body target in targets)
                using (session_.__usingDisplayPartReset())
                {
                    try
                    {
                        __work_component_ = target.OwningComponent;

                        foreach (Component tool in tools)
                        {
                            ReferenceSet[] reference_sets = tool.__Prototype().GetAllReferenceSets()
                                .Where(__r => __r.Name.ToLower().Contains("sub"))
                                .Where(__r => __r.Name.ToLower().Contains("tool"))
                                .ToArray();

                            switch (reference_sets.Length)
                            {
                                case 0:
                                    print_($"Didn't find any sub tool reference sets for {tool.DisplayName}");
                                    continue;
                                case 1:
                                    tool.__ReferenceSet(reference_sets[0].Name);
                                    break;
                                default:
                                    print_($"Found more than one sub tool reference set for {tool.DisplayName}");
                                    continue;
                            }


                            ExtractFace linked_body = tool.__CreateLinkedBody();

                            target.OwningPart.__CreateBoolean(target, linked_body.GetBodies(),
                                Feature.BooleanType.Subtract);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }
            //}
        }


        /// <summary>
        ///     Creates a linked body of each <paramref name="snapToolComponents" /> into the owning parts of
        ///     <paramref name="snapTargetBodies" />.
        ///     <paramref name="booleanType" /> will determine what type of operation to perform between each target body and the
        ///     linked bodies that were created.
        /// </summary>
        /// <param name="booleanType">The type of operation to perform, set to link if no boolean operation is desired.</param>
        /// <param name="snapTargetBodies">
        ///     The collection of target bodies, whose owning parts will each get a linked body wave
        ///     into them.
        /// </param>
        /// <param name="snapToolComponents">
        ///     The collection of tool components, who will have their current component
        ///     body waved into each target bodies owning part.
        /// </param>
        public static void Perform_Link(
            Feature.BooleanType booleanType,
            IEnumerable<Body> snapTargetBodies,
            IEnumerable<Component> snapToolComponents,
            bool blank)
        {
            Component[] snapToolsArray = snapToolComponents.ToArray();

            foreach (Body target in snapTargetBodies)
            {
                ExtractFace[] extractedBodies = snapToolsArray
                    .SelectMany(x => SpecialWaveLink(target.OwningComponent, x))
                    .ToArray();
                string currentRefset = target.OwningComponent.ReferenceSet;

                if (currentRefset != Refset_EntirePart
                    && target.OwningComponent.Prototype is Part prototype
                    && prototype.__FindReferenceSetOrNull(currentRefset) is ReferenceSet refset)
                    using (refset.__UsingRedisplayObject())
                    {
                        refset.AddObjectsToReferenceSet(extractedBodies.SelectMany(face => face.GetBodies()).ToArray());
                    }

                if (__display_part_.PartUnits != target.OwningComponent.__Prototype().PartUnits)
                    continue;

                __work_component_ = target.OwningComponent;
                PerformBoolean(booleanType, target, extractedBodies);
            }

            if (!blank)
                return;

            foreach (Component tool in snapToolsArray)
                tool.Blank();
        }


        /// <summary>
        ///     Adds an extra filter to the supplies <paramref name="predicate" /> to include not allowing the selection of
        ///     <paramref name="bodies" /> owning components.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="bodies">The bodies whose owning components are not selectable.</param>
        /// <returns>Returns a new Predicate.</returns>
        public static Predicate<Component> CheckPredicateLink(Predicate<Component> predicate, IEnumerable<Body> bodies)
        {
            Body[] bodyArray = bodies.ToArray();

            return !bodyArray.Any()
                ? predicate
                : x => predicate(x) && bodyArray.All(t => t.OwningComponent.Tag != x.Tag);
        }

        public static IEnumerable<Body> GetCurrentBodiesLink(Component snapComponent)
        {
            //Revision log 2.2.2 (6/1/2017)
            //Makes it so that all bodies in a given reference set are used in stead of the first one.
            Tag subTool;
            Tag cycleObjOcc = Tag.Null;

            do
            {
                subTool = ufsession_.Assem.CycleEntsInPartOcc(snapComponent.Tag, cycleObjOcc);

                if (subTool != Tag.Null)
                {
                    ufsession_.Obj.AskTypeAndSubtype(subTool, out int toolType, out int toolSubType);

                    if (toolType == UFConstants.UF_solid_type && toolSubType == UFConstants.UF_solid_body_subtype)
                        yield return (Body)session_.__GetTaggedObject(subTool);
                }

                cycleObjOcc = subTool;
            }
            while (subTool != Tag.Null);
        }

        /// <summary>
        ///     Creates a boolean operation between the <paramref name="targetBody" /> and the <paramref name="linkedBodies" />.
        /// </summary>
        /// <remarks>If <paramref name="buttonsBooleans" /> is set to Link, no boolean operation will be performed.</remarks>
        /// <param name="buttonsBooleans"></param>
        /// <param name="targetBody"></param>
        /// <param name="linkedBodies"></param>
        public static void PerformBoolean(
            Feature.BooleanType buttonsBooleans,
            Body targetBody,
            IEnumerable<ExtractFace> linkedBodies)
        {
            if (buttonsBooleans == Feature.BooleanType.Create)
                return;

            Func<BasePart, Body, Body[], BooleanFeature> booleanFunc;

            switch (buttonsBooleans)
            {
                case Feature.BooleanType.Subtract:
                    booleanFunc = __Extensions_.__CreateSubtract;
                    break;
                case Feature.BooleanType.Unite:
                    booleanFunc = __Extensions_.__CreateUnite;
                    break;
                case Feature.BooleanType.Intersect:
                    booleanFunc = __Extensions_.__CreateIntersect;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonsBooleans), buttonsBooleans, null);
            }

            ExtractFace[] linkedBodyArray = linkedBodies.ToArray();

            foreach (ExtractFace link in linkedBodyArray)
                try
                {
                    Body[] bodies = link.GetBodies();

                    if (bodies.Length != 1)
                    {
                        print_($"Found more than on body in linked body {link.GetFeatureName()}");
                        continue;
                    }

                    Body toolBody = bodies[0];

                    if (targetBody.__InterferesWith(toolBody))
                        //if (IsInterfering_Link(targetBody, ))
                        booleanFunc(targetBody.OwningPart, targetBody, new[] { toolBody });
                    else
                        UnableToSubtract_Link(link, buttonsBooleans);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
        }

        /// <summary>Creates a selection dialog for picking Target Boolean Bodies.</summary>
        /// <remarks>Allows the selection of bodies that are not assembly holders.</remarks>
        /// <returns>Returns the selected bodies.</returns>
        /// <exception cref="NothingSelectedException">When the selection is ended without selecting.</exception>
        public static IEnumerable<Body> SelectTargets(string TargetPrompt, Predicate<Body> pred)
        {
            List<Body> targets = TypeMulti(pred, prompt: TargetPrompt, masks: BodyMask);

            if (targets.Count == 0)
                throw new NothingSelectedException();

            targets.ForEach(x => x.Unhighlight());
            List<Body> targetBodies = targets.ToList();

            if (targetBodies.Count == 0)
                throw new NothingSelectedException();

            return targetBodies;
        }

        /// <summary>
        ///     Creates a selection dialog for picking Tool Boolean components.
        /// </summary>
        /// <param name="selectedBodies">
        ///     The array of bodies whose owning components will not be selectable.
        ///     Leave null if you don't want any extra filtering.
        /// </param>
        /// <returns>Returns the selected components.</returns>
        /// <exception cref="NothingSelectedException">When the selection is ended without selecting.</exception>
        /// <exception cref="ArgumentException">When <paramref name="selectedBodies" /> is not null, but has a length of 0.</exception>
        public static IEnumerable<Component> SelectTools(
            string ToolPrompt,
            Predicate<Component> ToolFilter,
            params Body[] selectedBodies)
        {
            //Creates a new predicate from the ToolFilter and the new selectedBodies.
            Predicate<Component> selectionPredicate = CheckPredicateLink(ToolFilter, selectedBodies);
            List<Component> selectedComponents = ComponentMultiSnap(selectionPredicate, prompt: ToolPrompt);

            //If nothing is selected, exit out.
            if (selectedComponents.Count == 0)
                throw new NothingSelectedException();

            IEnumerable<Component> toolComponents = selectedComponents;
            return toolComponents;
        }

        /// <summary>
        ///     Prints to the Lw that the Link body was unable to be subtracted out of a block.
        /// </summary>
        /// <param name="link">The linked body in question.</param>
        /// <param name="booleanType"></param>
        public static void UnableToSubtract_Link(ExtractFace link, Feature.BooleanType booleanType)
        {
            try
            {
                ufsession_.Wave.AskLinkedFeatureInfo(link.Tag, out UFWave.LinkedFeatureInfo info);
                ufsession_.Wave.AskLinkXform(link.Tag, out Tag xform);
                ufsession_.So.AskAssyCtxtPartOcc(xform, __work_component_.Tag, out Tag partOcc);
                Component comp = (Component)session_.__GetTaggedObject(partOcc);
                //var firstPartOfMessage = "Was unable to subtract " + info.source_part_name;
                string firstPartOfMessage = $"Was unable to {booleanType} {info.source_part_name}";
                string fileName = Path.GetFileNameWithoutExtension(__work_part_.FullPath);
                string endingPartOfMessage = $" : out of {fileName}";
                string message = comp.DisplayName._IsFastenerExtended_()
                    ? $"{firstPartOfMessage} in {comp.OwningComponent.DisplayName}{endingPartOfMessage}"
                    : firstPartOfMessage + endingPartOfMessage;
                print_(message);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        /// <summary>Determines is a component is an assembly holder.</summary>
        /// <remarks>Added 2.1</remarks>
        /// <param name="snapComponent">The component.</param>
        /// <returns>
        ///     Returns true if the displayName ends with "lwr", "upr", "lsh", "ush",  "000" or contains "lsp", "usp", false
        ///     otherwise.
        /// </returns>
        public static bool IsAssemblyHolderOverride_LinkNoAssemblies(Component snapComponent)
        {
            try
            {
                int hyphenIndex = snapComponent.DisplayName.LastIndexOf('-');
                if (hyphenIndex < 0) /**/ return false;
                string substring = snapComponent.DisplayName.Substring(hyphenIndex);
                string[] endsWith = { "lwr", "upr", "lsh", "ush", "000" };
                string[] contains = { "lsp", "usp" };
                return endsWith.Any(substring.EndsWith) || contains.Any(substring.Contains);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return false;
        }


        public static List<Component> ComponentMultiSnap(Predicate<Component> predicate = null,
            SelectionScope scope = SelectionScope.UseDefault, bool keepHighlighed = false, string prompt = "Select")
        {
            Predicate<Component> predicate1 = null;
            if (predicate != null)
                predicate1 = x => predicate(x);

            return TypeMulti(predicate1, scope, (keepHighlighed ? 1 : 0) != 0, prompt, ComponentMask);
        }

        /// <summary>
        ///     Creates a linked body.
        /// </summary>
        /// <param name="snapTargetComponent">The component in which the new linked body will be created in.</param>
        /// <param name="snapToolBody">The body that should be extracted for the Linked Body.</param>
        /// <returns>The <see cref="Snap.NX.ExtractFace" /> that was created.</returns>
        public static ExtractFace SpecialWaveLink(Component snapTargetComponent, Body snapToolBody)
        {
            Tag objectInPart = snapTargetComponent.__Prototype().Tag;
            Tag fromPartOcc = snapToolBody.OwningComponent.Tag;
            Tag toPartOcc = snapTargetComponent.Tag;
            Tag body = snapToolBody.__Prototype().Tag;
            ufsession_.So.CreateXformAssyCtxt(objectInPart, fromPartOcc, toPartOcc, out Tag xform);
            ufsession_.Wave.CreateLinkedBody(body, xform, objectInPart, false, out Tag linkedFeature);
            ExtractFace extract = (ExtractFace)session_.__GetTaggedObject(linkedFeature);
            extract.__Layer(100);
            print_($"Created {extract.GetFeatureName()} in {extract.OwningPart.Leaf}");
            return extract;
        }


        /// <summary>
        ///     Creates a linked body.
        /// </summary>
        /// <param name="snapTargetComponent">The component in which the new linked body will be created in.</param>
        /// <param name="snapToolComponent">The component in which the its current body state will be extracted.</param>
        /// <returns>The <see cref="Snap.NX. ExtractFace" /> that was created.</returns>
        public static IEnumerable<ExtractFace> SpecialWaveLink(Component snapTargetComponent,
            Component snapToolComponent)
        {
            IEnumerable<Body> bodies = GetCurrentBodiesLink(snapToolComponent);
            foreach (Body body in bodies)
                yield return SpecialWaveLink(snapTargetComponent, body);
        }

        /// <summary>Determines is a component is an assembly holder.</summary>
        /// <remarks>Added 2.1</remarks>
        /// <param name="snapComponent">The component.</param>
        /// <returns>
        ///     Returns true if the displayName ends with "lwr", "upr", "lsh", "ush",  "000" or contains "lsp", "usp", false
        ///     otherwise.
        /// </returns>
        public static bool IsAssemblyHolderOverride_LinkSubTool(Component snapComponent)
        {
            try
            {
                int hyphenIndex = snapComponent.DisplayName.LastIndexOf('-');
                if (hyphenIndex < 0) /**/ return false;
                string substring = snapComponent.DisplayName.Substring(hyphenIndex);
                string[] endsWith = { "lwr", "upr", "lsh", "ush", "000" };
                string[] contains = { "lsp", "usp" };
                return endsWith.Any(substring.EndsWith) || contains.Any(substring.Contains);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return false;
        }

        public static void WaveLinkFasteners(bool blank_tools, string shcs_ref_set, string dwl_ref_set)
        {
            using (session_.__usingDisplayPartReset())
            {
                try
                {
                    Predicate<Component> toolfilter = x =>
                        !x.DisplayName._IsLayout_() &&
                        !IsAssemblyHolder(x) &&
                        !x.DisplayName._IsFastenerExtended_();

                    Body target = Selection.SelectSingleTaggedObject<Body>(
                        "Select Target Body",
                        "select Target Body",
                        new[] { BodyMask },
                        b => b.IsOccurrence);

                    Component[] tools = Selection.SelectMultipleTaggedObjects(
                        "Select Tool Components",
                        "Select Tool Components",
                        new[] { ComponentMask },
                        toolfilter);

                    __work_component_ = target.OwningComponent;

                    foreach (Component tool in tools)
                        try
                        {
                            foreach (Component child in tool.GetChildren())
                                try
                                {
                                    if (child._IsJckScrewTsg_() || child._IsJckScrew_())
                                        continue;

                                    using (child.__UsingReferenceSetReset())
                                    {
                                        string original_ref_set = child.ReferenceSet;
                                        Component proto_child_fastener = child.__ProtoChildComp();

                                        if (proto_child_fastener.Layer != Layer_Fastener)
                                            continue;

                                        try
                                        {
                                            string[] ref_sets = new[] { shcs_ref_set, dwl_ref_set }
                                                .Where(__r => !string.IsNullOrEmpty(__r))
                                                .ToArray();

                                            string[] fastener_ref_set_names = proto_child_fastener.__Prototype()
                                                .GetAllReferenceSets()
                                                .Select(__r => __r.Name)
                                                .Intersect(ref_sets)
                                                .ToArray();

                                            switch (fastener_ref_set_names.Length)
                                            {
                                                case 0:
                                                    if ((!(shcs_ref_set is null) && child._IsShcs_()) ||
                                                        (!(dwl_ref_set is null) && child._IsDwl_()))
                                                        print_(
                                                            $"Coulnd't find any valid ref sets for {child.DisplayName}");
                                                    continue;
                                                case 1:
                                                    child.__ReferenceSet(fastener_ref_set_names[0]);
                                                    break;
                                                default:
                                                    print_(
                                                        $"Found more than one valid ref set for {child.DisplayName}");
                                                    continue;
                                            }

                                            if (!target.__InterferesWith(child))
                                            {
                                                print_(
                                                    $"Could not subtract fastener {child.DisplayName}, no interference.");
                                                continue;
                                            }

                                            ExtractFace ext = child.__CreateLinkedBody();
                                            ext.__Layer(96);

                                            print_("//////////////////");
                                            print_($"Created {ext.GetFeatureName()} in {ext.OwningPart.Leaf}");
                                            BooleanFeature boolean_feature = target.__Subtract(ext.GetBodies());
                                            print_(
                                                $"Created {boolean_feature.GetFeatureName()} in {ext.OwningPart.Leaf}");
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.__PrintException(child.DisplayName);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.__PrintException();
                                }

                            if (blank_tools)
                                tool.Blank();
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }
                }
                catch (NothingSelectedException)
                {
                    //Control jump
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        public static void WaveLinkBoolean(bool blank_tools, Feature.BooleanType booleanType)
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "Assembly Wavelink");
            using (session_.__usingDisplayPartReset())
            using (session_.__UsingRegenerateDisplay())
            {
                try
                {
                    Body[] targets = SelectTargets("Select Boolean Targets", WaveLinkBooleanTargetFilter).ToArray();
                    Component[] tools =
                        SelectTools("Select Boolean Tools", IsNotAssemblyHolder_LinkNoAssemblies, targets)
                            .ToArray();
                    var referenceSets = tools.Select(snapComponent => new { snapComponent, snapComponent.ReferenceSet })
                        .ToList();
                    RefSetForm formCts = new RefSetForm(tools);

                    if (booleanType != Feature.BooleanType.Create)
                        formCts.RemoveReferenceSet("BODY");

                    formCts.ShowDialog();

                    if (!formCts.IsSelected)
                        return;

                    string selectedReferenceSet = formCts.SelectedReferenceSetName;
                    formCts.Dispose();
                    tools.ToList().ForEach(__c => __c.__ReferenceSet(selectedReferenceSet));
                    Perform_Link(booleanType, targets, tools, blank_tools);
                    referenceSets.ToList().ForEach(item => item.snapComponent.__ReferenceSet(item.ReferenceSet));
                }
                catch (NothingSelectedException)
                {
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        public static List<T> TypeMulti<T>(
            Predicate<T> predicate = null,
            SelectionScope scope = SelectionScope.UseDefault,
            bool keepHighlighed = false,
            string prompt = "Select",
            params UFUi.Mask[] masks) where T : DisplayableObject
        {
            return new SpecialSelectionAssWavelink<T>(predicate, scope, prompt, masks).SelectMultiple();
        }


        /// <summary>Determines is a component is an assembly holder.</summary>
        /// <remarks>Added 2.1</remarks>
        /// <param name="nxComp">The component.</param>
        /// <returns>
        ///     Returns true if the displayName endswith "lwr", "upr", "lsh", "ush",  "000" or contains "lsp", "usp", false
        ///     otherwise.
        /// </returns>
        public static bool IsAssemblyHolder(Component nxComp)
        {
            return nxComp != null && IsAssemblyHolder(nxComp.DisplayName);
        }

        public static bool IsAssemblyHolder(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            str = Path.GetFileNameWithoutExtension(str);
            int startIndex = str.LastIndexOf('-');

            if (startIndex < 0)
                return false;

            string str1 = str.Substring(startIndex);

            string[] strArray1 = new string[5]
            {
                "lwr",
                "upr",
                "lsh",
                "ush",
                "000"
            };

            string[] strArray2 = new string[2] { "lsp", "usp" };
            return strArray1.Any(str1.EndsWith) || strArray2.Any(str1.Contains);
        }
    }
}