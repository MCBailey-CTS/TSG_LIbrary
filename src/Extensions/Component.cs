using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Positioning;
using NXOpen.UF;
using TSG_Library.Disposable;
using TSG_Library.UFuncs;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Component

        public static IEnumerable<Component> __DescendantsAll(
            this Component component)
        {
            return from descendant in component.__Descendants(true, true, true) select descendant;
        }

        public static IEnumerable<Component> __Descendants(
            this Component rootComponent,
            bool includeRoot = true,
            bool includeSuppressed = false,
            bool includeUnloaded = false)
        {
            if (includeRoot)
                yield return rootComponent;

            Component[] children = rootComponent.GetChildren();

            for (int index = 0; index < children.Length; index++)
            {
                if (children[index].IsSuppressed && !includeSuppressed)
                    continue;

                if (!children[index].__IsLoaded() && !includeUnloaded)
                    continue;

                foreach (Component descendant in children[index]
                             .__Descendants(includeRoot, includeSuppressed, includeUnloaded))
                    yield return descendant;
            }
        }

        public static bool __IsLoaded(this Component component)
        {
            return component.Prototype is Part;
        }

        public static IEnumerable<NXObject> __Members(this Component component)
        {
            UFSession uFSession = ufsession_;
            Tag tag = Tag.Null;
            List<NXObject> list = new List<NXObject>();

            do
            {
                tag = uFSession.Assem.CycleEntsInPartOcc(component.Tag, tag);

                if (tag == Tag.Null)
                    continue;

                try
                {
                    NXObject nXObject = session_.__GetTaggedObject(tag) as NXObject;

                    if (nXObject is null)
                        continue;

                    list.Add(nXObject);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
            while (tag != 0);

            return list;
        }

        public static Tag __InstanceTag(this Component component)
        {
            return ufsession_.Assem.AskInstOfPartOcc(component.Tag);
        }

        public static Component __ProtoChildComp(this Component component)
        {
            Tag instance = component.__InstanceTag();
            Tag root_component = component.OwningComponent.__Prototype().ComponentAssembly.RootComponent.Tag;
            Tag proto_child_fastener_tag = ufsession_.Assem.AskPartOccOfInst(root_component, instance);
            return (Component)session_.__GetTaggedObject(proto_child_fastener_tag);
        }

        public static ExtractFace __CreateLinkedBody(this Component child)
        {
            WaveLinkBuilder builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.ExtractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;
                builder.Type = WaveLinkBuilder.Types.BodyLink;
                builder.ExtractFaceBuilder.Associative = true;
                ScCollector scCollector1 = builder.ExtractFaceBuilder.ExtractBodyCollector;
                builder.ExtractFaceBuilder.FeatureOption =
                    ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
                Body[] bodies1 = new Body[1];
                BodyDumbRule bodyDumbRule1 =
                    __work_part_.ScRuleFactory.CreateRuleBodyDumb(child.__SolidBodyMembers(), false);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                builder.ExtractFaceBuilder.FixAtCurrentTimestamp = false;
                return (ExtractFace)builder.Commit();
            }
        }

        public static bool __IsJckScrewTsg(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw-tsg");
        }

        public static bool __IsDwl(this Component part)
        {
            return part.DisplayName.ToLower().Contains("-dwl-");
        }

        public static bool __IsJckScrew(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw");
        }

        public static NXObject __FindOccurrence(this Component component, NXObject proto)
        {
            return component.FindOccurrence(proto);
        }

        //public static void reference_set(this NXOpen.Assemblies.Component component, string reference_set)
        //{
        //    component.DirectOwner.ReplaceReferenceSet(component, reference_set);
        //}

        public static CartesianCoordinateSystem __AbsoluteCsysOcc(this Component component)
        {
            return (CartesianCoordinateSystem)component.FindOccurrence(component.__Prototype().__AbsoluteCsys());
        }

        public static DatumPlane __AbsOccDatumPlaneXY(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component.__Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneXY());
        }

        public static DatumPlane __AbsOccDatumPlaneXZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component.__Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneXZ());
        }

        public static DatumPlane __AbsOccDatumPlaneYZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component.__Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneYZ());
        }

        public static Component __FindComponent(this Component component,
            string __journal_identifier)
        {
            try
            {
                return (Component)component.FindObject(__journal_identifier);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(3520016);
                throw new Exception(
                    $"Could not find component with journal identifier: '{__journal_identifier}' in component '{component.DisplayName}'");
            }
        }


        public static Component __InstOfPartOcc(this Component component)
        {
            Tag instance = ufsession_.Assem.AskInstOfPartOcc(component.Tag);
            return (Component)session_.__GetTaggedObject(instance);
        }


        public static bool __IsShcs(this Component component)
        {
            return component.DisplayName.__IsShcs();
        }

        public static bool __IsFastener(this Component component)
        {
            return component.DisplayName.__IsFastener();
        }

        public static void __ReplaceComponent(this Component component, string path, string name,
            bool replace_all)
        {
            ReplaceComponentBuilder replace_builder =
                __work_part_.AssemblyManager.CreateReplaceComponentBuilder();

            using (session_.__UsingBuilderDestroyer(replace_builder))
            {
                replace_builder.ComponentNameType =
                    ReplaceComponentBuilder.ComponentNameOption.AsSpecified;
                replace_builder.ComponentsToReplace.Add(component);
                replace_builder.ReplaceAllOccurrences = replace_all;
                replace_builder.ComponentName = name;
                replace_builder.ReplacementPart = path;
                replace_builder.SetComponentReferenceSetType(
                    ReplaceComponentBuilder.ComponentReferenceSet.Maintain,
                    null);
                replace_builder.Commit();
            }
        }

        public static void __DeleteSelfAndConstraints(this Component component)
        {
            ComponentConstraint[] constraints = component.GetConstraints();

            if (constraints.Length > 0)
                session_.__DeleteObjects(constraints);

            session_.__DeleteObjects(component);
        }

        //public static void __SetWcsToComponent(this Component comp)
        //{
        //    __display_part_.WCS.SetOriginAndMatrix(comp.__Origin(), comp.__Orientation());
        //}


        public static string __AssemblyPathString(this Component component)
        {
            return $"{component._AssemblyPath().Aggregate("{ ", (str, cmp) => $"{str}{cmp.DisplayName}, ")}}}";
        }

        public static IDisposable __UsingReferenceSetReset(this Component component)
        {
            return new ReferenceSetReset(component);
        }

        public static Part __Prototype(this Component comp)
        {
            return (Part)comp.Prototype;
        }

        public static ReferenceSet[] __ReferenceSets(this Part part)
        {
            return part.GetAllReferenceSets();
        }

        public static ReferenceSet __ReferenceSets(this Part part, string refset_name)
        {
            return part.GetAllReferenceSets().Single(__ref => __ref.Name == refset_name);
        }

        public static Component __ToComponent(this Tag __tag)
        {
            return (Component)session_.__GetTaggedObject(__tag);
        }

        public static void __DeleteInstanceUserAttribute(this Component component, string title)
        {
            throw new NotImplementedException();
        }

        public static ComponentAssembly __DirectOwner(this Component component)
        {
            return component.DirectOwner;
        }


        public static void __NXOpenAssembliesComponent(Component component)
        {
            //component.DisplayName
            //component.FindObject
            //component.FindOccurrence
            //component.FixConstraint
            //component.GetArrangements
            //component.GetChildren
            //component.GetConstraints
            //component.GetDegreesOfFreedom
            //component.GetInstanceStringUserAttribute
            //component.GetLayerOption
            //component.GetNonGeometricState
            //component.GetPosition
            //component.GetPositionOverrideParent
            //component.GetPositionOverrideType
            //component.HasInstanceUserAttribute
            //component.IsFixed
            //component.IsPositioningIsolated
            //component.IsSuppressed
            //component.Parent
            //component.Prototype
            //component.RedisplayObject
            //component.ReferenceSet
            //component.SetInstanceUserAttribute
            //component.Suppress
            //component.SuppressingArrangement
            //component.UsedArrangement
        }


        //public static NXOpen.Point3d _Origin(this NXOpen.Assemblies.Component component)
        //{
        //    // ReSharper disable once UnusedVariable
        //    component.GetPosition(out NXOpen.Point3d position, out NXOpen.Matrix3x3 orientation);
        //    return position;
        //}

        //public static Orientation_ _Orientation(this NXOpen.Assemblies.Component component)
        //{
        //    // ReSharper disable once UnusedVariable
        //    component.GetPosition(out NXOpen.Point3d position, out NXOpen.Matrix3x3 orientation);
        //    return orientation;
        //}


        public static bool __IsLeaf(this Component component)
        {
            return component.GetChildren().Length == 0;
        }

        public static bool __IsRoot(this Component component)
        {
            return component.Parent is null;
        }

        public static void __ReferenceSet(this Component component, string referenceSetTitle)
        {
            if (!(component.Prototype is Part part))
                throw new ArgumentException($"The given component \"{component.DisplayName}\" is not loaded.");

            //   part._RightClickOpen
            switch (referenceSetTitle)
            {
                case RefsetEntirePart:
                case RefsetEmpty:
                    component.DirectOwner.ReplaceReferenceSet(component, referenceSetTitle);
                    break;
                default:
                    _ = part.__FindReferenceSetOrNull(referenceSetTitle)
                        ??
                        throw new InvalidOperationException(
                            $"Cannot set component \"{component.DisplayName}\" to the reference set \"{referenceSetTitle}\".");

                    component.DirectOwner.ReplaceReferenceSet(component, referenceSetTitle);
                    break;
            }
        }

        //public static Part __Prototype(this Component component)
        //{
        //    return (Part)component.Prototype;
        //}

        public static Body[] __SolidBodyMembers(this Component component)
        {
            return component.__Members()
                .OfType<Body>()
                .Where(__b => __b.IsSolidBody)
                .ToArray();
        }

        public static TreeNode __TreeNode(this Component component)
        {
            return new TreeNode(component.DisplayName) { Tag = component };
        }

        public static Matrix3x3 __Orientation(this Component component)
        {
            component.GetPosition(out _, out Matrix3x3 orientation);
            return orientation;
        }

        public static void __SetWcsToComponent(this Component component)
        {
            __display_part_.WCS.SetOriginAndMatrix(component.__Origin(), component.__Orientation());
        }

        public static Point3d __Origin(this Component component)
        {
            component.GetPosition(out Point3d origin, out _);
            return origin;
        }

        //public static NXOpen.Assemblies.Component find_component_or_null(this NXOpen.Assemblies.Component component , string journal_identifier)
        //{
        //    try
        //    {

        //    }catch (Exception e) { }
        //}

        #endregion
    }
}