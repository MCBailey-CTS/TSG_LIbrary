using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

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
            Component mirroredComp = (Component)dict[originalComp];

            originalFeature.Suppress();

            Feature mirroredFeature = (Feature)dict[originalFeature];

            mirroredFeature.Suppress();

            ChamferBuilder chamferBuilder = originalFeature.__OwningPart().Features.CreateChamferBuilder(originalFeature);

            IList<SelectionIntentRule> newRules = new List<SelectionIntentRule>();

            using (new Destroyer(chamferBuilder))
            {
                chamferBuilder.SmartCollector.GetRules(out SelectionIntentRule[] originalRules);

                foreach (SelectionIntentRule oldRule in originalRules)
                    newRules.Add(BaseMirrorRule.MirrorRule(oldRule, originalFeature, plane, originalComp, dict));
            }

            _WorkPart = mirroredComp.__Prototype();

            Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");

            EditWithRollbackManager editWithRollbackManager1 = _WorkPart.Features.StartEditWithRollbackManager(mirroredFeature, markId1);

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