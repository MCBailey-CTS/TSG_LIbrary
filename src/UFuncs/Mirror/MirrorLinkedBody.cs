using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorLinkedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "LINKED_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            Feature feature = (Feature)dict[originalFeature];
            ExtractFace extractFace = (ExtractFace)feature;
            Component component = (Component)dict[originalComp];
            if (!extractFace.__IsBroken()) return;

            ExtractFace extractFace2 = (ExtractFace)originalFeature;
            if (extractFace2.__IsBroken()) return;

            Tag assy_context_xform = extractFace2.__XFormTag();
            ufsession_.So.AskAssyCtxtPartOcc(assy_context_xform, originalComp.Tag, out Tag from_part_occ);
            Component component2 = (Component)session_.GetObjectManager().GetTaggedObject(from_part_occ);
            ufsession_.Wave.AskLinkedFeatureGeom(extractFace2.Tag, out Tag linked_geom);
            ufsession_.Wave.AskLinkedFeatureInfo(linked_geom, out UFWave.LinkedFeatureInfo _);
            if (component2 == null)
                throw new MirrorException("Linked component was null in " + originalFeature.__OwningPart().Leaf +
                                          " from " + originalFeature.GetFeatureName());

            Component[] array = component2._AssemblyPath().ToArray();
            Component component3 = array[array.Length - 2];
            string[] array2 = new string[array.Length];
            for (int i = 0; i < array2.Length; i++) array2[i] = array[i].ReferenceSet;

            using (new ReferenceSetReset(component3))
            {
                component3.__ReferenceSet("Entire Part");
                ILibraryComponent[] array3 = new ILibraryComponent[4]
                {
                    new MirrorSmartButton(),
                    new MirrorSmartKey(),
                    new MirrorSmartStockEjector(),
                    new MirrorSmartStandardLiftersGuidedKeepersMetric()
                };
                foreach (ILibraryComponent libraryComponent in array3)
                    if (libraryComponent.IsLibraryComponent(component2))
                    {
                        libraryComponent.Mirror(plane, component, extractFace2, component2, dict);
                        originalFeature.Unsuppress();
                        feature.Unsuppress();
                        return;
                    }

                if (!component2.DisplayName.Contains("layout") && !component2.DisplayName.Contains("blank")) return;

                __work_part_ = __display_part_;
                using (new ReferenceSetReset(component2))
                {
                    component2.__ReferenceSet("Entire Part");
                    __work_part_ = originalComp.__Prototype();
                    ExtractFaceBuilder extractFaceBuilder =
                        __work_part_.Features.CreateExtractFaceBuilder(extractFace2);
                    Body[] bodies;
                    using (new Destroyer(extractFaceBuilder))
                    {
                        bodies = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<NXObject>()
                            .Select(component2.FindOccurrence)
                            .OfType<Body>()
                            .ToArray();
                    }

                    __work_component_ = component;
                    Session.UndoMarkId featureEditMark =
                        session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                    EditWithRollbackManager editWithRollbackManager =
                        __work_part_.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
                    using (new Destroyer(editWithRollbackManager))
                    {
                        ExtractFaceBuilder extractFaceBuilder2 =
                            __work_part_.Features.CreateExtractFaceBuilder(extractFace);
                        using (new Destroyer(extractFaceBuilder2))
                        {
                            BodyDumbRule bodyDumbRule = __work_part_.ScRuleFactory.CreateRuleBodyDumb(bodies, true);
                            SelectionIntentRule[] rules = new SelectionIntentRule[1] { bodyDumbRule };
                            extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules, false);
                            extractFaceBuilder2.Associative = true;
                            extractFaceBuilder2.Commit();
                        }

                        session_.Preferences.Modeling.UpdatePending = false;
                    }

                    originalFeature.Unsuppress();
                    feature.Unsuppress();
                    __work_part_ = __display_part_;
                }
            }
        }
    }
}