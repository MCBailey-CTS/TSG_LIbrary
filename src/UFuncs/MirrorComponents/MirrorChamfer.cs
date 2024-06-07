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
    public class MirrorChamfer : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "CHAMFER";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            var mirroredComp = (Component)dict[originalComp];

            originalFeature.Suppress();

            var mirroredFeature = (Feature)dict[originalFeature];

            mirroredFeature.Suppress();

            var chamferBuilder = originalFeature.__OwningPart().Features.CreateChamferBuilder(originalFeature);

            IList<SelectionIntentRule> newRules = new List<SelectionIntentRule>();

            using (new Destroyer(chamferBuilder))
            {
                chamferBuilder.SmartCollector.GetRules(out var originalRules);

                foreach (var oldRule in originalRules)
                    newRules.Add(BaseMirrorRule.MirrorRule(oldRule, originalFeature, plane, originalComp, dict));
            }

            _WorkPart = mirroredComp._Prototype();

            var markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");

            var editWithRollbackManager1 = _WorkPart.Features.StartEditWithRollbackManager(mirroredFeature, markId1);

            using (new Destroyer(editWithRollbackManager1))
            {
                chamferBuilder = _WorkPart.Features.CreateChamferBuilder(mirroredFeature);

                using (new Destroyer(chamferBuilder))
                {
                    chamferBuilder.ReverseOffsets = !chamferBuilder.ReverseOffsets;

                    chamferBuilder.SmartCollector.ReplaceRules(newRules.ToArray(), false);

                    chamferBuilder.CommitFeature();
                }
            }
        }
    }
}