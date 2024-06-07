using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorEdgeMultipleSeedTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } =
            SelectionIntentRule.RuleType.EdgeMultipleSeedTangent;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            var mirroredComp = (Component)dict[originalComp];

            var mirroredPart = mirroredComp._Prototype();

            var mirroredFeature = (Feature)dict[originalFeature];

            ((EdgeMultipleSeedTangentRule)originalRule).GetData(out var originalSeedEdges, out var angleTolerance,
                out var hasSameConvexity);

            IList<Edge> newEdges = new List<Edge>();

            foreach (var originalEdge in originalSeedEdges)
            {
                var originalBody = originalEdge.GetBody();

                Body mirrorBody;

                if(!dict.ContainsKey(originalBody))
                {
                    mirroredFeature.Suppress();

                    originalFeature.Suppress();

                    var originalOwningFeature = originalComp._Prototype().Features.GetParentFeatureOfBody(originalBody);

                    var mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                    if(mirrorOwningFeature.GetBodies().Length != 1)
                        throw new InvalidOperationException("Invalid number of bodies for feature");

                    mirrorBody = mirrorOwningFeature.GetBodies()[0];
                }
                else
                {
                    mirrorBody = (Body)dict[originalBody];
                }

                var finalStart = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp);

                var finalEnd = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp);

                foreach (var e in mirrorBody.GetEdges())
                    if(e._HasEndPoints(finalStart, finalEnd))
                        newEdges.Add(e);
            }

            return mirroredPart.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(newEdges.ToArray(), angleTolerance,
                hasSameConvexity);
        }
    }
}