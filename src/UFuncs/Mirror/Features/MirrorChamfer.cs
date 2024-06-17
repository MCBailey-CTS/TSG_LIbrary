using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Rules;
using TSG_Library.UFuncs.MirrorComponents.Features;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror.Features
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
                foreach (SelectionIntentRule originalRule in rules)
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



    public class MirrorExtractedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRACT_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
        }
    }


    public class MirrorExtrude : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRUDE";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Part part2 = originalComp.__Prototype();
            originalFeature.Suppress();
            Extrude extrude = (Extrude)dict[originalFeature];
            ExtrudeBuilder extrudeBuilder = part2.Features.CreateExtrudeBuilder(originalFeature);
            Point3d val;
            Vector3d val2;
            using (new Destroyer(extrudeBuilder))
            {
                val = MirrorMap(extrudeBuilder.Direction.Origin, plane, originalComp, component);
                val2 = MirrorMap(extrudeBuilder.Direction.Vector, plane, originalComp, component);
            }

            extrudeBuilder = part.Features.CreateExtrudeBuilder(extrude);
            using (new Destroyer(extrudeBuilder))
            {
                extrudeBuilder.Direction.Vector = val2;
                extrudeBuilder.Direction.Origin = val;
                extrudeBuilder.CommitFeature();
            }
        }
    }
}