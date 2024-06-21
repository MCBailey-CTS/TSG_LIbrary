using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Rules;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror.Features
{
    public class MirrorOffsetFace : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "OFFSET";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Part part2 = originalComp.__Prototype();
            OffsetFace edgeBlend = (OffsetFace)dict[originalFeature];
            IDictionary<int, Tuple<SelectionIntentRule[], Expression>> dictionary =
                new Dictionary<int, Tuple<SelectionIntentRule[], Expression>>();


            var feature = (OffsetFace)dict[originalFeature];

            OffsetFaceBuilder edgeBlendBuilder = part2.Features.CreateOffsetFaceBuilder(originalFeature);

            //using (new Destroyer(edgeBlendBuilder))
            //{
            //    edgeBlendBuilder.FaceCollector

            //    int numberOfValidChainsets = edgeBlendBuilder.GetNumberOfValidChainsets();
            //    for (int i = 0; i < numberOfValidChainsets; i++)
            //    {
            //        edgeBlendBuilder.GetChainset(i, out ScCollector collector, out Expression radius);
            //        collector.GetRules(out SelectionIntentRule[] rules);
            //        IList<SelectionIntentRule> source = rules.Select(originalRule =>
            //            BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict)).ToList();
            //        dictionary.Add(i, Tuple.Create(source.ToArray(), radius));
            //    }
            //}


            //Session.UndoMarkId featureEditMark = session_.SetUndoMark(Session.MarkVisibility.Visible, "Fine");
            //EditWithRollbackManager editWithRollbackManager =
            //    __work_part_.Features.StartEditWithRollbackManager(edgeBlend, featureEditMark);
            //using (new Destroyer(editWithRollbackManager))
            //{
            //    EdgeBlendBuilder edgeBlendBuilder2 = __work_part_.Features.CreateEdgeBlendBuilder(edgeBlend);
            //    using (new Destroyer(edgeBlendBuilder2))
            //    {
            //        for (int j = 0; j < edgeBlendBuilder2.GetNumberOfValidChainsets(); j++)
            //        {
            //            edgeBlendBuilder2.GetChainsetAndStatus(j, out ScCollector collector2, out Expression _,
            //                out bool _);
            //            SelectionIntentRule[] item = dictionary[j].Item1;
            //            collector2.ReplaceRules(item, false);
            //        }

            //        edgeBlendBuilder2.CommitFeature();
            //    }
            //}

            IList<SelectionIntentRule> list = new List<SelectionIntentRule>();
            using (new Destroyer(edgeBlendBuilder))
            {
                edgeBlendBuilder.FaceCollector.GetRules(out SelectionIntentRule[] rules);
                foreach (SelectionIntentRule originalRule in rules)
                    list.Add(BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict));
            }

            __work_part_ = component.__Prototype();
            Session.UndoMarkId featureEditMark = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = __work_part_.Features.StartEditWithRollbackManager(feature, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                edgeBlendBuilder = __work_part_.Features.CreateOffsetFaceBuilder(feature);

                using (new Destroyer(edgeBlendBuilder))
                {
                    var col = __work_part_.ScCollectors.CreateCollector();
                    col.ReplaceRules(list.ToArray(), false);

                    edgeBlendBuilder.FaceCollector.ReplaceRules(list.ToArray(), false);

                    //chamferBuilder..ReverseOffsets = !chamferBuilder.ReverseOffsets;
                    //chamferBuilder.SmartCollector.ReplaceRules(list.ToArray(), false);
                    edgeBlendBuilder.CommitFeature();
                }
            }

            originalFeature.Unsuppress();
            edgeBlend.Unsuppress();
        }
    }
}