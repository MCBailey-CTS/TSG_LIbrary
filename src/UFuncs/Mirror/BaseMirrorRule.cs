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

        public static SelectionIntentRule MirrorRule(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {

            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            try
            {
                switch (originalRule.Type)
                {
                    case SelectionIntentRule.RuleType.EdgeVertex:
                        return new MirrorEdgeVertexRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeBody:
                        return new MirrorEdgeBodyRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeTangent:
                        return new MirrorEdgeTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeDumb:
                        return new MirrorEdgeDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeBoundary:
                        return new MirrorEdgeBoundaryRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeMultipleSeedTangent:
                        return new MirrorEdgeMultipleSeedTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.EdgeChain:
                        return new MirrorEdgeChainRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.FaceDumb:
                        return new MirrorFaceDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.FaceTangent:
                        return new MirrorFaceTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    case SelectionIntentRule.RuleType.FaceAndAdjacentFaces:
                        return new MirrorFaceAndAdjacentFacesRule().Mirror(originalRule, originalFeature, plane, originalComp, dict);
                    default:
                        throw new ArgumentException($"Unknown rule: \"{originalRule.Type}\", for feature: {feature.GetFeatureName()} in part {feature.OwningPart.Leaf}");
                }

            }
            catch (NXException ex) when (ex.ErrorCode == 630003)
            {
                throw new MirrorException(ex.Message);
            }
        }
    }




}