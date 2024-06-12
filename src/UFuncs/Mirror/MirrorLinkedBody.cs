using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CTS_Library;
using CTS_Library.Equality;
using CTS_Library.Extensions;
using CTS_Library.UFuncs.MirrorComponents.LibraryComponents;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{

    public class MirrorLinkedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "LINKED_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane, Component originalComp)
        {
            Feature feature = (Feature)dict[originalFeature];
            ExtractFace extractFace = (ExtractFace)feature;
            Component component = (Component)dict[originalComp];
            if (!extractFace.__IsBroken())
            {
                return;
            }

            ExtractFace extractFace2 = (ExtractFace)originalFeature;
            if (extractFace2._IsBroken())
            {
                return;
            }

            Tag assy_context_xform = extractFace2._XFormTag();
            Globals._UFSession.So.AskAssyCtxtPartOcc(assy_context_xform, originalComp.Tag, out var from_part_occ);
            Component component2 = (Component)Globals._Session.GetObjectManager().GetTaggedObject(from_part_occ);
            Globals._UFSession.Wave.AskLinkedFeatureGeom(extractFace2.Tag, out var linked_geom);
            Globals._UFSession.Wave.AskLinkedFeatureInfo(linked_geom, out var _);
            if (component2 == null)
            {
                throw new MirrorException("Linked component was null in " + originalFeature._OwningPart().Leaf + " from " + originalFeature.GetFeatureName());
            }

            Component[] array = component2._AssemblyPath().ToArray();
            Component component3 = array[array.Length - 2];
            string[] array2 = new string[array.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = array[i].ReferenceSet;
            }

            using (new ReferenceSetReset(component3))
            {
                component3._ReferenceSet("Entire Part");
                ILibraryComponent[] array3 = new ILibraryComponent[4]
                {
                new MirrorSmartButton(),
                new MirrorSmartKey(),
                new MirrorSmartStockEjector(),
                new MirrorSmartStandardLiftersGuidedKeepersMetric()
                };
                ILibraryComponent[] array4 = array3;
                foreach (ILibraryComponent libraryComponent in array4)
                {
                    if (libraryComponent.IsLibraryComponent(component2))
                    {
                        libraryComponent.Mirror(plane, component, extractFace2, component2, dict);
                        originalFeature.Unsuppress();
                        feature.Unsuppress();
                        return;
                    }
                }

                if (!component2.DisplayName.Contains("layout") && !component2.DisplayName.Contains("blank"))
                {
                    return;
                }

                Globals._WorkPart = Globals._DisplayPart;
                using (new ReferenceSetReset(component2))
                {
                    component2._ReferenceSet("Entire Part");
                    Globals._WorkPart = originalComp._Prototype();
                    ExtractFaceBuilder extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace2);
                    Body[] bodies;
                    using (new Destroyer(extractFaceBuilder))
                    {
                        bodies = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<NXObject>().Select(component2.FindOccurrence)
                            .OfType<Body>()
                            .ToArray();
                    }

                    Globals._WorkComponent = component;
                    Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                    EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
                    using (new Destroyer(editWithRollbackManager))
                    {
                        ExtractFaceBuilder extractFaceBuilder2 = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                        using (new Destroyer(extractFaceBuilder2))
                        {
                            BodyDumbRule bodyDumbRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodies, includeSheetBodies: true);
                            SelectionIntentRule[] rules = new SelectionIntentRule[1] { bodyDumbRule };
                            extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules, createRulesWoUpdate: false);
                            extractFaceBuilder2.Associative = true;
                            extractFaceBuilder2.Commit();
                        }

                        Globals._Session.Preferences.Modeling.UpdatePending = false;
                    }

                    originalFeature.Unsuppress();
                    feature.Unsuppress();
                    Globals._WorkPart = Globals._DisplayPart;
                }
            }
        }
    }




}