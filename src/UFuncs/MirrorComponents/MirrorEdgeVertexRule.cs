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
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            Feature mirroredFeature = (Feature)dict[originalFeature];

            ((EdgeVertexRule)originalRule).GetData(out Edge originalStartEdge, out bool isFromStart);

            Body originalBody = originalStartEdge.GetBody();

            Body mirrorBody;

            if (!dict.ContainsKey(originalBody))
            {
                mirroredFeature.Suppress();

                originalFeature.Suppress();

                Feature originalOwningFeature =
                    originalComp.__Prototype().Features.GetParentFeatureOfBody(originalBody);

                BodyFeature mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                if (mirrorOwningFeature.GetBodies().Length != 1)
                    throw new InvalidOperationException("Invalid number of bodies for feature");

                mirrorBody = mirrorOwningFeature.GetBodies()[0];
            }
            else
            {
                mirrorBody = (Body)dict[originalBody];
            }

            Point3d newStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

            Point3d newEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

            Edge mirrorEdge = mirrorBody.GetEdges().FirstOrDefault(edge => edge.__HasEndPoints(newStart, newEnd));

            if (mirrorEdge is null)
                throw new InvalidOperationException("Could not find mirror edge");

            return mirroredPart.ScRuleFactory.CreateRuleEdgeVertex(mirrorEdge, isFromStart);
        }
    }
}