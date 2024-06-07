using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions;

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
            var mirroredFeature = (Feature)dict[originalFeature];

            var mirroredLinkedBody = (ExtractFace)mirroredFeature;

            var mirroredComp = (Component)dict[originalComp];

            if(!mirroredLinkedBody._IsBroken())
                return;

            var originalLinkedBody = (ExtractFace)originalFeature;

            // Check to see if they are both broken. Then we can continue.
            if(originalLinkedBody._IsBroken())
                return;

            var xform = originalLinkedBody._XFormTag();

            _UFSession.So.AskAssyCtxtPartOcc(xform, originalComp.Tag, out var fromPartOcc);

            var fromComp = (Component)session_.GetObjectManager().GetTaggedObject(fromPartOcc);

            _UFSession.Wave.AskLinkedFeatureGeom(originalLinkedBody.Tag, out var linkedGeom);

            _UFSession.Wave.AskLinkedFeatureInfo(linkedGeom, out var nameStore);

            if(fromComp is null)
                throw new MirrorException(
                    $"Linked component was null in {originalFeature.__OwningPart().Leaf} from {originalFeature.GetFeatureName()}");

            var parentComponents = fromComp._AssemblyPath().ToArray();

            var parent = parentComponents[parentComponents.Length - 2];

            var originalRefsets = new string[parentComponents.Length];

            for (var i = 0; i < originalRefsets.Length; i++)
                originalRefsets[i] = parentComponents[i].ReferenceSet;

            using (new ReferenceSetReset(parent))
            {
                parent._ReferenceSet("Entire Part");

                ILibraryComponent[] libComps =
                {
                    new MirrorSmartButton(),
                    new MirrorSmartKey(),
                    new MirrorSmartStockEjector(),
                    new MirrorSmartStandardLiftersGuidedKeepersMetric()
                };

                foreach (var libComp in libComps)
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
                    fromComp._ReferenceSet("Entire Part");

                    _WorkPart = originalComp._Prototype();

                    Body[] bodies;

                    var tempExtractBuilder = _WorkPart.Features.CreateExtractFaceBuilder(originalLinkedBody);

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

                    var markId4 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");

                    var rollbackManager = _WorkPart.Features.StartEditWithRollbackManager(mirroredLinkedBody, markId4);

                    using (new Destroyer(rollbackManager))
                    {
                        var extractBuilder = _WorkPart.Features.CreateExtractFaceBuilder(mirroredLinkedBody);

                        using (new Destroyer(extractBuilder))
                        {
                            var bodyDumbRule1 = _WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodies, true);

                            var rules1 = new SelectionIntentRule[1];

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