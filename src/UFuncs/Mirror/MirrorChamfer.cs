using System.Collections.Generic;
using System.Linq;
using CTS_Library;
using CTS_Library.Extensions;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorChamfer : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "CHAMFER";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            originalFeature.Suppress();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            Part part = component._Prototype();
            ChamferBuilder chamferBuilder = originalFeature._OwningPart().Features.CreateChamferBuilder(originalFeature);
            IList<SelectionIntentRule> list = new List<SelectionIntentRule>();
            using (new Destroyer(chamferBuilder))
            {
                chamferBuilder.SmartCollector.GetRules(out var rules);
                SelectionIntentRule[] array = rules;
                foreach (SelectionIntentRule originalRule in array)
                {
                    list.Add(BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict));
                }
            }

            Globals._WorkPart = component._Prototype();
            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(feature, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                chamferBuilder = Globals._WorkPart.Features.CreateChamferBuilder(feature);
                using (new Destroyer(chamferBuilder))
                {
                    chamferBuilder.ReverseOffsets = !chamferBuilder.ReverseOffsets;
                    chamferBuilder.SmartCollector.ReplaceRules(list.ToArray(), createRulesWoUpdate: false);
                    chamferBuilder.CommitFeature();
                }
            }
        }
    }



}