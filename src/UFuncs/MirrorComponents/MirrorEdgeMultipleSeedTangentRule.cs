using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
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
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            Feature mirroredFeature = (Feature)dict[originalFeature];

            ((EdgeMultipleSeedTangentRule)originalRule).GetData(out Edge[] originalSeedEdges, out var angleTolerance,
                out var hasSameConvexity);

            IList<Edge> newEdges = new List<Edge>();

            foreach (Edge originalEdge in originalSeedEdges)
            {
                Body originalBody = originalEdge.GetBody();

                Body mirrorBody;

                if(!dict.ContainsKey(originalBody))
                {
                    mirroredFeature.Suppress();

                    originalFeature.Suppress();

                    Feature originalOwningFeature =
                        originalComp.__Prototype().Features.GetParentFeatureOfBody(originalBody);

                    BodyFeature mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                    if(mirrorOwningFeature.GetBodies().Length != 1)
                        throw new InvalidOperationException("Invalid number of bodies for feature");

                    mirrorBody = mirrorOwningFeature.GetBodies()[0];
                }
                else
                {
                    mirrorBody = (Body)dict[originalBody];
                }

                Point3d finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                Point3d finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                foreach (Edge e in mirrorBody.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newEdges.Add(e);
            }

            return mirroredPart.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(newEdges.ToArray(), angleTolerance,
                hasSameConvexity);
        }
    }
}