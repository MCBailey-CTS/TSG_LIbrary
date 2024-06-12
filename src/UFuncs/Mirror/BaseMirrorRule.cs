using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    [Obsolete]
    public abstract class BaseMirrorRule : IMirrorRule
    {
        public abstract SelectionIntentRule.RuleType RuleType { get; }

        public static bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            if (edgePoints.Count != mirrorFace.GetEdges().Length)
                return false;

            HashSet<Edge> hashSet = new HashSet<Edge>(mirrorFace.GetEdges());
            Edge edge = hashSet.First();
            hashSet.Remove(edge);
            Edge edge2 = hashSet.First();
            hashSet.Remove(edge2);
            Edge edge3 = hashSet.First();
            hashSet.Remove(edge3);
            Edge edge4 = hashSet.First();
            hashSet.Remove(edge4);
            ISet<Edge> set = new HashSet<Edge>();
            foreach (Tuple<Point3d, Point3d> edgePoint in edgePoints)
            {
                if (edge.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge);
                }

                if (edge2.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge2);
                }

                if (edge3.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge3);
                }

                if (edge4.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge4);
                }
            }

            return set.Count == mirrorFace.GetEdges().Length;
        }

        public abstract SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict);

        [Obsolete]
        public static SelectionIntentRule MirrorRule(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            throw new NotImplementedException();
            //try
            //{
            //    Component component = (Component)dict[originalComp];
            //    Part part = component._Prototype();
            //    Feature feature = (Feature)dict[originalFeature];
            //    return originalRule.Type switch
            //    {
            //        SelectionIntentRule.RuleType.EdgeVertex => new MirrorEdgeVertexRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeBody => new MirrorEdgeBodyRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeTangent => new MirrorEdgeTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeDumb => new MirrorEdgeDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeBoundary => new MirrorEdgeBoundaryRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeMultipleSeedTangent => new MirrorEdgeMultipleSeedTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.EdgeChain => new MirrorEdgeChainRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.FaceDumb => new MirrorFaceDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.FaceTangent => new MirrorFaceTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        SelectionIntentRule.RuleType.FaceAndAdjacentFaces => new MirrorFaceAndAdjacentFacesRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
            //        _ => throw new ArgumentException($"Unknown rule: \"{originalRule.Type}\", for feature: {feature.GetFeatureName()} in part {feature.OwningPart.Leaf}"),
            //    };
            //}
            //catch (NXException ex) when (ex.ErrorCode == 630003)
            //{
            //    throw new MirrorException(ex.Message);
            //}
        }
    }




}