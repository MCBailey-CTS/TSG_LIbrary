using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
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
            var mirroredComp = (Component)dict[originalComp];
            var mirroredPart = mirroredComp._Prototype();
            ((EdgeBodyRule)originalRule).GetData(out var originalBody);
            var mirroredPositions = new List<Point3d>();

            foreach (var originalEdge in originalBody.GetEdges())
            {
                var finalStart = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp);
                var finalEnd = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp);
                mirroredPositions.Add(finalEnd);
                mirroredPositions.Add(finalStart);
            }

            mirroredPositions = mirroredPositions.DistinctBy(__p => __p.__ToHashCode()).ToList();
            Body mirroredBody = null;

            foreach (var tempMirroredBody in mirroredPart.Bodies.ToArray())
            {
                var bodyPositions = new List<Point3d>();

                foreach (var bodyEdge in tempMirroredBody.GetEdges())
                {
                    bodyPositions.Add(bodyEdge._StartPoint());
                    bodyPositions.Add(bodyEdge._EndPoint());
                }

                bodyPositions = bodyPositions.DistinctBy(__p => __p.__ToHashCode()).ToList();

                if(bodyPositions.Count != mirroredPositions.Count)
                    continue;

                var mirror_hashes = Enumerable.ToHashSet(mirroredPositions.Select(__p => __p.__ToHashCode()));
                var body_hashes = Enumerable.ToHashSet(bodyPositions.Select(__p => __p.__ToHashCode()));

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