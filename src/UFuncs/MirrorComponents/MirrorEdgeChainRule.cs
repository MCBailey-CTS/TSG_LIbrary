using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorEdgeChainRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeChain;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            var mirroredComp = (Component)dict[originalComp];

            var mirroredPart = mirroredComp.__Prototype();

            // ReSharper disable once UnusedVariable
            _ = (Feature)dict[originalFeature];

            ((EdgeChainRule)originalRule).GetData(out var originalStartEdge, out var originalEndEdge,
                out var isFromStart);

            Edge newStartEdge = null;

            Edge newEndEdge = null;

            var finalStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

            var finalEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

            foreach (var body in mirroredPart.Bodies.ToArray())
            foreach (var e in body.GetEdges())
                if(e.__HasEndPoints(finalStart, finalEnd))
                    newStartEdge = e;

            if(!(originalEndEdge is null))
            {
                finalStart = originalEndEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                finalEnd = originalEndEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                foreach (var body in mirroredPart.Bodies.ToArray())
                foreach (var e in body.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newEndEdge = e;
            }

            if(newStartEdge is null)
                throw new ArgumentException("Could not find start edge");

            return mirroredPart.ScRuleFactory.CreateRuleEdgeChain(newStartEdge, newEndEdge, isFromStart);
        }
    }
}