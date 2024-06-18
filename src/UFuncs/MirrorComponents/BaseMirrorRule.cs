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
    public abstract class BaseMirrorRule : IMirrorRule
    {
        public abstract SelectionIntentRule.RuleType RuleType { get; }

        public abstract SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict);

        public static bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            if (edgePoints.Count != mirrorFace.GetEdges().Length)
                return false;

            HashSet<Edge> faceEdges = new HashSet<Edge>(mirrorFace.GetEdges());

            Edge edge0 = faceEdges.First();
            faceEdges.Remove(edge0);

            Edge edge1 = faceEdges.First();
            faceEdges.Remove(edge1);

            Edge edge2 = faceEdges.First();
            faceEdges.Remove(edge2);

            Edge edge3 = faceEdges.First();
            faceEdges.Remove(edge3);

            ISet<Edge> matchedEdges = new HashSet<Edge>();

            foreach (Tuple<Point3d, Point3d> tuple in edgePoints)
            {
                if (edge0.__HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge0);

                if (edge1.__HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge1);

                if (edge2.__HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge2);

                if (edge3.__HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge3);
            }

            return matchedEdges.Count == mirrorFace.GetEdges().Length;
        }


        public static SelectionIntentRule MirrorRule(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            try
            {
                Component mirroredComp = (Component)dict[originalComp];

                // ReSharper disable once UnusedVariable
                Part mirroredPart = mirroredComp.__Prototype();

                Feature mirroredFeature = (Feature)dict[originalFeature];

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (originalRule.Type)
                {
                    case SelectionIntentRule.RuleType.EdgeVertex:
                        return new MirrorEdgeVertexRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.EdgeBody:
#pragma warning disable CS0612 // Type or member is obsolete
                        return new MirrorEdgeBodyRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);
#pragma warning restore CS0612 // Type or member is obsolete

                    case SelectionIntentRule.RuleType.EdgeTangent:
                        return new MirrorEdgeTangentRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.EdgeDumb:
                        return new MirrorEdgeDumbRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.EdgeBoundary:
                        return new MirrorEdgeBoundaryRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.EdgeMultipleSeedTangent:
                        return new MirrorEdgeMultipleSeedTangentRule().Mirror(originalRule, originalFeature, plane,
                            originalComp, dict);

                    case SelectionIntentRule.RuleType.EdgeChain:
                        return new MirrorEdgeChainRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.FaceDumb:
                        return new MirrorFaceDumbRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.FaceTangent:
                        return new MirrorFaceTangentRule().Mirror(originalRule, originalFeature, plane, originalComp,
                            dict);

                    case SelectionIntentRule.RuleType.FaceAndAdjacentFaces:
                        return new MirrorFaceAndAdjacentFacesRule().Mirror(originalRule, originalFeature, plane,
                            originalComp, dict);

                    default:
                        throw new ArgumentException(
                            $"Unknown rule: \"{originalRule.Type}\", for feature: {mirroredFeature.GetFeatureName()} in part {mirroredFeature.OwningPart.Leaf}");
                }
            }
            catch (NXException nxException) when (nxException.ErrorCode == 630003)
            {
                // Suppressed edge, face, or body.
                throw new MirrorException(nxException.Message);
            }
        }
    }
}