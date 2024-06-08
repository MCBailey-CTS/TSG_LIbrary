using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorEdgeDumbRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeDumb;

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

            ((EdgeDumbRule)originalRule).GetData(out var originalEdges);

            mirroredFeature.Suppress();

            originalFeature.Suppress();

            //            Snap.InfoWindow.WriteLine(originalFeature.GetFeatureName());
            //            
            //            throw new NotImplementedException();

            IList<Edge> newEdges = new List<Edge>();

            foreach (var originalEdge in originalEdges)
            {
                var finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                var finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                mirroredPart.Curves.CreateLine(finalStart, finalEnd);

                foreach (var body in mirroredPart.Bodies.ToArray())
                foreach (var e in body.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newEdges.Add(e);
            }

            mirroredFeature.Unsuppress();

            originalFeature.Unsuppress();

            return mirroredPart.ScRuleFactory.CreateRuleEdgeDumb(newEdges.ToArray());
        }
    }
}