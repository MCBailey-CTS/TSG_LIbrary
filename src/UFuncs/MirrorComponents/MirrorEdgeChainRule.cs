using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
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
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            // ReSharper disable once UnusedVariable
            _ = (Feature)dict[originalFeature];

            ((EdgeChainRule)originalRule).GetData(out Edge originalStartEdge, out Edge originalEndEdge,
                out bool isFromStart);

            Edge newStartEdge = null;

            Edge newEndEdge = null;

            Point3d finalStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

            Point3d finalEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

            foreach (Body body in mirroredPart.Bodies.ToArray())
            foreach (Edge e in body.GetEdges())
                if (e.__HasEndPoints(finalStart, finalEnd))
                    newStartEdge = e;

            if (!(originalEndEdge is null))
            {
                finalStart = originalEndEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                finalEnd = originalEndEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                foreach (Body body in mirroredPart.Bodies.ToArray())
                foreach (Edge e in body.GetEdges())
                    if (e.__HasEndPoints(finalStart, finalEnd))
                        newEndEdge = e;
            }

            if (newStartEdge is null)
                throw new ArgumentException("Could not find start edge");

            return mirroredPart.ScRuleFactory.CreateRuleEdgeChain(newStartEdge, newEndEdge, isFromStart);
        }
    }
}