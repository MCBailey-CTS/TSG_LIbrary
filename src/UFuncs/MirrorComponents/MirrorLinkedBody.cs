using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorLinkedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "LINKED_BODY";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            Feature mirroredFeature = (Feature)dict[originalFeature];

            ExtractFace mirroredLinkedBody = (ExtractFace)mirroredFeature;

            Component mirroredComp = (Component)dict[originalComp];

            if(!mirroredLinkedBody.__IsBroken())
                return;

            ExtractFace originalLinkedBody = (ExtractFace)originalFeature;

            // Check to see if they are both broken. Then we can continue.
            if(originalLinkedBody.__IsBroken())
                return;

            Tag xform = originalLinkedBody.__XFormTag();

            _UFSession.So.AskAssyCtxtPartOcc(xform, originalComp.Tag, out Tag fromPartOcc);

            Component fromComp = (Component)session_.GetObjectManager().GetTaggedObject(fromPartOcc);

            _UFSession.Wave.AskLinkedFeatureGeom(originalLinkedBody.Tag, out Tag linkedGeom);

            _UFSession.Wave.AskLinkedFeatureInfo(linkedGeom, out UFWave.LinkedFeatureInfo nameStore);

            if(fromComp is null)
                throw new MirrorException(
                    $"Linked component was null in {originalFeature.__OwningPart().Leaf} from {originalFeature.GetFeatureName()}");

            Component[] parentComponents = fromComp._AssemblyPath().ToArray();

            Component parent = parentComponents[parentComponents.Length - 2];

            var originalRefsets = new string[parentComponents.Length];

            for (var i = 0; i < originalRefsets.Length; i++)
                originalRefsets[i] = parentComponents[i].ReferenceSet;

            using (new ReferenceSetReset(parent))
            {
                parent.__ReferenceSet("Entire Part");

                ILibraryComponent[] libComps =
                {
                    new MirrorSmartButton(),
                    new MirrorSmartKey(),
                    new MirrorSmartStockEjector(),
                    new MirrorSmartStandardLiftersGuidedKeepersMetric()
                };

                foreach (ILibraryComponent libComp in libComps)
                    if(libComp.IsLibraryComponent(fromComp))
                    {
                        libComp.Mirror(plane, mirroredComp, originalLinkedBody, fromComp, dict);

                        originalFeature.Unsuppress();

                        mirroredFeature.Unsuppress();

                        return;
                    }

                if(!fromComp.DisplayName.Contains("layout") && !fromComp.DisplayName.Contains("blank"))
                    return;

                _WorkPart = __display_part_;

                using (new ReferenceSetReset(fromComp))
                {
                    fromComp.__ReferenceSet("Entire Part");

                    _WorkPart = originalComp.__Prototype();

                    Body[] bodies;

                    ExtractFaceBuilder tempExtractBuilder = _WorkPart.Features.CreateExtractFaceBuilder(originalLinkedBody);

                    using (new Destroyer(tempExtractBuilder))
                    {
                        bodies = tempExtractBuilder.ExtractBodyCollector
                            .GetObjects()
                            .OfType<NXObject>()
                            .Select(fromComp.FindOccurrence)
                            .OfType<Body>()
                            .ToArray();
                    }

                    __work_component_ = mirroredComp;

                    Session.UndoMarkId markId4 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");

                    EditWithRollbackManager rollbackManager = _WorkPart.Features.StartEditWithRollbackManager(mirroredLinkedBody, markId4);

                    using (new Destroyer(rollbackManager))
                    {
                        ExtractFaceBuilder extractBuilder = _WorkPart.Features.CreateExtractFaceBuilder(mirroredLinkedBody);

                        using (new Destroyer(extractBuilder))
                        {
                            BodyDumbRule bodyDumbRule1 = _WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodies, true);

                            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];

                            rules1[0] = bodyDumbRule1;

                            extractBuilder.ExtractBodyCollector.ReplaceRules(rules1, false);

                            extractBuilder.Associative = true;

                            extractBuilder.Commit();
                        }

                        session_.Preferences.Modeling.UpdatePending = false;
                    }

                    originalFeature.Unsuppress();

                    mirroredFeature.Unsuppress();

                    _WorkPart = __display_part_;
                }
            }
        }
    }
}