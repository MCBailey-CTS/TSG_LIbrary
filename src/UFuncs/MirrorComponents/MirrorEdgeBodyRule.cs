using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    [Obsolete]
    public class MirrorEdgeBodyRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBody;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part mirroredPart = mirroredComp.__Prototype();
            ((EdgeBodyRule)originalRule).GetData(out Body originalBody);
            List<Point3d> mirroredPositions = new List<Point3d>();

            foreach (Edge originalEdge in originalBody.GetEdges())
            {
                Point3d finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);
                Point3d finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);
                mirroredPositions.Add(finalEnd);
                mirroredPositions.Add(finalStart);
            }

            mirroredPositions = mirroredPositions.DistinctBy(__p => __p.__ToHashCode()).ToList();
            Body mirroredBody = null;

            foreach (Body tempMirroredBody in mirroredPart.Bodies.ToArray())
            {
                List<Point3d> bodyPositions = new List<Point3d>();

                foreach (Edge bodyEdge in tempMirroredBody.GetEdges())
                {
                    bodyPositions.Add(bodyEdge.__StartPoint());
                    bodyPositions.Add(bodyEdge.__EndPoint());
                }

                bodyPositions = bodyPositions.DistinctBy(__p => __p.__ToHashCode()).ToList();

                if(bodyPositions.Count != mirroredPositions.Count)
                    continue;

                HashSet<int> mirror_hashes = Enumerable.ToHashSet(mirroredPositions.Select(__p => __p.__ToHashCode()));
                HashSet<int> body_hashes = Enumerable.ToHashSet(bodyPositions.Select(__p => __p.__ToHashCode()));

                if(!mirror_hashes.SetEquals(body_hashes))
                    continue;

                mirroredBody = tempMirroredBody;

                break;
            }

            if(mirroredBody is null)
                throw new ArgumentException("Could not find a matching body");

            return mirroredPart.ScRuleFactory.CreateRuleEdgeBody(mirroredBody);

            throw new NotImplementedException();
        }
    }
}