using System;
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


            var mirroredComp = (Component)dict[originalComp];

            // ReSharper disable once UnusedVariable
            var mirroredPart = mirroredComp.__Prototype();

            var originalPart = originalComp.__Prototype();

            //originalFeature.Suppress();


            // throw new NotImplementedException();

            var mirroredFeature = (EdgeBlend)dict[originalFeature];

            IDictionary<int, Tuple<SelectionIntentRule[], Expression>> chainSetDict =
                new Dictionary<int, Tuple<SelectionIntentRule[], Expression>>();

            var builder = originalPart.Features.CreateEdgeBlendBuilder(originalFeature);

            using (new Destroyer(builder))
            {
                var chainSetLength = builder.GetNumberOfValidChainsets();

                for (var i = 0; i < chainSetLength; i++)
                {
                    builder.GetChainset(i, out var originalCollector, out var radius);

                    originalCollector.GetRules(out var originalRules);

                    IList<SelectionIntentRule> newRules = originalRules.Select(originalRule =>
                        BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict)).ToList();

                    chainSetDict.Add(i, Tuple.Create(newRules.ToArray(), radius));
                }
            }

            var markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Fine");

            var editWithRollbackManager1 = _WorkPart.Features.StartEditWithRollbackManager(mirroredFeature, markId1);

            using (new Destroyer(editWithRollbackManager1))
            {
                var edgeBlendBuilder1 = _WorkPart.Features.CreateEdgeBlendBuilder(mirroredFeature);

                using (new Destroyer(edgeBlendBuilder1))
                {
                    for (var i = 0; i < edgeBlendBuilder1.GetNumberOfValidChainsets(); i++)
                    {
                        edgeBlendBuilder1.GetChainsetAndStatus(i, out var scCollector1, out var _, out var _);

                        var newRules = chainSetDict[i].Item1;

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