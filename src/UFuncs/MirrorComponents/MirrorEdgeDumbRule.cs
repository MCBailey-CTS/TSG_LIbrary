using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
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
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            Feature mirroredFeature = (Feature)dict[originalFeature];

            ((EdgeDumbRule)originalRule).GetData(out Edge[] originalEdges);

            mirroredFeature.Suppress();

            originalFeature.Suppress();

            //            Snap.InfoWindow.WriteLine(originalFeature.GetFeatureName());
            //            
            //            throw new NotImplementedException();

            IList<Edge> newEdges = new List<Edge>();

            foreach (Edge originalEdge in originalEdges)
            {
                Point3d finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                Point3d finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                mirroredPart.Curves.CreateLine(finalStart, finalEnd);

                foreach (Body body in mirroredPart.Bodies.ToArray())
                foreach (Edge e in body.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newEdges.Add(e);
            }

            mirroredFeature.Unsuppress();

            originalFeature.Unsuppress();

            return mirroredPart.ScRuleFactory.CreateRuleEdgeDumb(newEdges.ToArray());
        }
    }
}