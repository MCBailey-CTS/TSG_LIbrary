using System;
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
    public class MirrorEdgeBlend : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "BLEND";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            //originalFeature.Suppress();


            Component mirroredComp = (Component)dict[originalComp];

            // ReSharper disable once UnusedVariable
            Part mirroredPart = mirroredComp.__Prototype();

            Part originalPart = originalComp.__Prototype();

            //originalFeature.Suppress();


            // throw new NotImplementedException();

            EdgeBlend mirroredFeature = (EdgeBlend)dict[originalFeature];

            IDictionary<int, Tuple<SelectionIntentRule[], Expression>> chainSetDict =
                new Dictionary<int, Tuple<SelectionIntentRule[], Expression>>();

            EdgeBlendBuilder builder = originalPart.Features.CreateEdgeBlendBuilder(originalFeature);

            using (new Destroyer(builder))
            {
                int chainSetLength = builder.GetNumberOfValidChainsets();

                for (int i = 0; i < chainSetLength; i++)
                {
                    builder.GetChainset(i, out ScCollector originalCollector, out Expression radius);

                    originalCollector.GetRules(out SelectionIntentRule[] originalRules);

                    IList<SelectionIntentRule> newRules = originalRules.Select(originalRule =>
                        BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict)).ToList();

                    chainSetDict.Add(i, Tuple.Create(newRules.ToArray(), radius));
                }
            }

            Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Fine");

            EditWithRollbackManager editWithRollbackManager1 =
                _WorkPart.Features.StartEditWithRollbackManager(mirroredFeature, markId1);

            using (new Destroyer(editWithRollbackManager1))
            {
                EdgeBlendBuilder edgeBlendBuilder1 = _WorkPart.Features.CreateEdgeBlendBuilder(mirroredFeature);

                using (new Destroyer(edgeBlendBuilder1))
                {
                    for (int i = 0; i < edgeBlendBuilder1.GetNumberOfValidChainsets(); i++)
                    {
                        edgeBlendBuilder1.GetChainsetAndStatus(i, out ScCollector scCollector1, out Expression _,
                            out bool _);

                        SelectionIntentRule[] newRules = chainSetDict[i].Item1;

                        scCollector1.ReplaceRules(newRules, false);
                    }

                    edgeBlendBuilder1.CommitFeature();
                }
            }

            originalFeature.Unsuppress();

            mirroredFeature.Unsuppress();
        }
    }
}