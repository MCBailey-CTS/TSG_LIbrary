using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.UFuncs;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static IEnumerable<Component> _DescendantsAll(
            this Component component)
        {
            return from descendant in component._Descendants(true, true, true) select descendant;
        }

        public static IEnumerable<Component> _Descendants(
            this Component rootComponent,
            bool includeRoot = true,
            bool includeSuppressed = false,
            bool includeUnloaded = false)
        {
            if(includeRoot)
                yield return rootComponent;

            var children = rootComponent.GetChildren();

            for (var index = 0; index < children.Length; index++)
            {
                if(children[index].IsSuppressed && !includeSuppressed)
                    continue;

                if(!children[index]._IsLoaded() && !includeUnloaded)
                    continue;

                foreach (var descendant in children[index]
                             ._Descendants(includeRoot, includeSuppressed, includeUnloaded))
                    yield return descendant;
            }
        }

        public static bool _IsLoaded(this Component component)
        {
            return component.Prototype is Part;
        }

        public static IEnumerable<NXObject> _Members(this Component component)
        {
            var uFSession = ufsession_;
            var tag = Tag.Null;
            var list = new List<NXObject>();

            do
            {
                tag = uFSession.Assem.CycleEntsInPartOcc(component.Tag, tag);

                if(tag == Tag.Null)
                    continue;

                try
                {
                    var nXObject = session_._GetTaggedObject(tag) as NXObject;

                    if(nXObject is null)
                        continue;

                    list.Add(nXObject);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
            } while (tag != 0);

            return list;
        }

        public static Tag _InstanceTag(this Component component)
        {
            return ufsession_.Assem.AskInstOfPartOcc(component.Tag);
        }

        public static Component _ProtoChildComp(this Component component)
        {
            var instance = component._InstanceTag();
            var root_component = component.OwningComponent._Prototype().ComponentAssembly.RootComponent.Tag;
            var proto_child_fastener_tag = ufsession_.Assem.AskPartOccOfInst(root_component, instance);
            return (Component)session_._GetTaggedObject(proto_child_fastener_tag);
        }

        public static ExtractFace _CreateLinkedBody(this Component child)
        {
            var builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.ExtractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;
                builder.Type = WaveLinkBuilder.Types.BodyLink;
                builder.ExtractFaceBuilder.Associative = true;
                var scCollector1 = builder.ExtractFaceBuilder.ExtractBodyCollector;
                builder.ExtractFaceBuilder.FeatureOption =
                    ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
                var bodies1 = new Body[1];
                var bodyDumbRule1 =
                    __work_part_.ScRuleFactory.CreateRuleBodyDumb(child.solid_body_memebers(), false);
                var rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                builder.ExtractFaceBuilder.FixAtCurrentTimestamp = false;
                return (ExtractFace)builder.Commit();
            }
        }

        public static bool _IsJckScrewTsg(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw-tsg");
        }

        public static bool _IsDwl(this Component part)
        {
            return part.DisplayName.ToLower().Contains("-dwl-");
        }

        public static bool _IsJckScrew(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw");
        }

        public static NXObject find_occurrence(this Component component, NXObject proto)
        {
            return component.FindOccurrence(proto);
        }

        //public static void reference_set(this NXOpen.Assemblies.Component component, string reference_set)
        //{
        //    component.DirectOwner.ReplaceReferenceSet(component, reference_set);
        //}

        public static CartesianCoordinateSystem _AbsoluteCsysOcc(this Component component)
        {
            return (CartesianCoordinateSystem)component.FindOccurrence(component._Prototype().__AbsoluteCsys());
        }

        public static DatumPlane _AbsOccDatumPlaneXY(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                ._DatumPlaneXY());
        }

        public static DatumPlane _AbsOccDatumPlaneXZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                ._DatumPlaneXZ());
        }

        public static DatumPlane _AbsOccDatumPlaneYZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                ._DatumPlaneYZ());
        }

        public static Component find_component_(this Component component,
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


        public static Component _InstOfPartOcc(this Component component)
        {
            var instance = ufsession_.Assem.AskInstOfPartOcc(component.Tag);
            return (Component)session_._GetTaggedObject(instance);
        }


        public static bool _IsShcs(this Component component)
        {
            return component.DisplayName._IsShcs();
        }

        public static bool _IsFastener(this Component component)
        {
            return component.DisplayName._IsFastener();
        }

        public static void replace_component(this Component component, string path, string name,
            bool replace_all)
        {
            var replace_builder =
                __work_part_.AssemblyManager.CreateReplaceComponentBuilder();

            using (session_.using_builder_destroyer(replace_builder))
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

        public static void delete_self_and_constraints(this Component component)
        {
            var constraints = component.GetConstraints();

            if(constraints.Length > 0)
                session_.delete_objects(constraints);

            session_.delete_objects(component);
        }


        public static string _AssemblyPathString(this Component component)
        {
            return $"{component._AssemblyPath().Aggregate("{ ", (str, cmp) => $"{str}{cmp.DisplayName}, ")}}}";
        }

        public static IDisposable using_reference_set_reset(this Component component)
        {
            return new ReferenceSetReset(component);
        }

        public static Part prototype(this Component comp)
        {
            return comp._Prototype();
        }

        public static ReferenceSet[] reference_sets(this Part part)
        {
            return part.GetAllReferenceSets();
        }

        public static ReferenceSet reference_sets(this Part part, string refset_name)
        {
            return part.GetAllReferenceSets().Single(__ref => __ref.Name == refset_name);
        }

        public static Component _ToComponent(this Tag __tag)
        {
            return (Component)session_._GetTaggedObject(__tag);
        }

        public static void __DeleteInstanceUserAttribute(this Component component, string title)
        {
            throw new NotImplementedException();
        }

        public static ComponentAssembly __DirectOwner(this Component component)
        {
            return component.DirectOwner;
        }


        public static void NXOpen_Assemblies_Component(Component component)
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


        public static bool _IsLeaf(this Component component)
        {
            return component.GetChildren().Length == 0;
        }

        public static bool _IsRoot(this Component component)
        {
            return component.Parent is null;
        }

        public static void _ReferenceSet(this Component component, string referenceSetTitle)
        {
            if(!(component.Prototype is Part part))
                throw new ArgumentException($"The given component \"{component.DisplayName}\" is not loaded.");

            //   part._RightClickOpen
            switch (referenceSetTitle)
            {
                case Refset_EntirePart:
                case Refset_Empty:
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

        public static Part _Prototype(this Component component)
        {
            return (Part)component.Prototype;
        }

        public static Body[] solid_body_memebers(this Component component)
        {
            return component._Members()
                .OfType<Body>()
                .Where(__b => __b.IsSolidBody)
                .ToArray();
        }

        public static TreeNode _TreeNode(this Component component)
        {
            return new TreeNode(component.DisplayName) { Tag = component };
        }

        public static Matrix3x3 _Orientation(this Component component)
        {
            component.GetPosition(out _, out var orientation);
            return orientation;
        }

        public static void _SetWcsToComponent(this Component component)
        {
            __display_part_.WCS.SetOriginAndMatrix(component._Origin(), component._Orientation());
        }

        public static Point3d _Origin(this Component component)
        {
            component.GetPosition(out var origin, out _);
            return origin;
        }

        //public static NXOpen.Assemblies.Component find_component_or_null(this NXOpen.Assemblies.Component component , string journal_identifier)
        //{
        //    try
        //    {

        //    }catch (Exception e) { }
        //}
    }
}