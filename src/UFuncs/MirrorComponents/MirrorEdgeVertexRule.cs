using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorEdgeVertexRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeVertex;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            var mirroredComp = (Component)dict[originalComp];

            var mirroredPart = mirroredComp.__Prototype();

            var mirroredFeature = (Feature)dict[originalFeature];

            ((EdgeVertexRule)originalRule).GetData(out var originalStartEdge, out var isFromStart);

            var originalBody = originalStartEdge.GetBody();

            Body mirrorBody;

            if(!dict.ContainsKey(originalBody))
            {
                mirroredFeature.Suppress();

                originalFeature.Suppress();

                var originalOwningFeature = originalComp.__Prototype().Features.GetParentFeatureOfBody(originalBody);

                var mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                if(mirrorOwningFeature.GetBodies().Length != 1)
                    throw new InvalidOperationException("Invalid number of bodies for feature");

                mirrorBody = mirrorOwningFeature.GetBodies()[0];
            }
            else
            {
                mirrorBody = (Body)dict[originalBody];
            }

            var newStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

            var newEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

            var mirrorEdge = mirrorBody.GetEdges().FirstOrDefault(edge => edge.__HasEndPoints(newStart, newEnd));

            if(mirrorEdge is null)
                throw new InvalidOperationException("Could not find mirror edge");

            return mirroredPart.ScRuleFactory.CreateRuleEdgeVertex(mirrorEdge, isFromStart);
        }
    }
}