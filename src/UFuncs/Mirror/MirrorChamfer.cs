using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorChamfer : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "CHAMFER";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            originalFeature.Suppress();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            Part part = component.__Prototype();
            ChamferBuilder chamferBuilder =
                originalFeature.__OwningPart().Features.CreateChamferBuilder(originalFeature);
            IList<SelectionIntentRule> list = new List<SelectionIntentRule>();
            using (new Destroyer(chamferBuilder))
            {
                chamferBuilder.SmartCollector.GetRules(out SelectionIntentRule[] rules);
                SelectionIntentRule[] array = rules;
                foreach (SelectionIntentRule originalRule in array)
                    list.Add(BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict));
            }

            __work_part_ = component.__Prototype();
            Session.UndoMarkId featureEditMark =
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager =
                __work_part_.Features.StartEditWithRollbackManager(feature, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                chamferBuilder = __work_part_.Features.CreateChamferBuilder(feature);
                using (new Destroyer(chamferBuilder))
                {
                    chamferBuilder.ReverseOffsets = !chamferBuilder.ReverseOffsets;
                    chamferBuilder.SmartCollector.ReplaceRules(list.ToArray(), false);
                    chamferBuilder.CommitFeature();
                }
            }
        }
    }
}