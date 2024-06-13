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
    public class MirrorBlock : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "BLOCK";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            originalFeature.Unsuppress();
            Feature mirrorFeature = (Feature)dict[originalFeature];
            mirrorFeature.Unsuppress();
            Component mirrorComp = (Component)dict[originalComp];
            Block originalBlock = (Block)originalFeature;
            Block mirrorBlock = (Block)mirrorFeature;

            if (!TryMatchFaces(originalBlock.GetFaces(), mirrorBlock.GetFaces(), plane, originalComp, mirrorComp,
                    out Face[] facePairs))
                throw new Exception($"Unable to match faces in {originalFeature.GetFeatureName()}");

            if (!TryMatchEdges(originalBlock.GetEdges(), mirrorBlock.GetEdges(), plane, originalComp, mirrorComp,
                    out Edge[] edgePairs))
                throw new Exception($"Unable to match edges in {originalFeature.GetFeatureName()}");

            for (int index = 0; index < facePairs.Length; index += 2)
            {
                Face originalFace = facePairs[index];
                Face mirrorFace = facePairs[index + 1];
                mirrorFace.__Color(originalFace.Color);
                mirrorFace.RedisplayObject();
                originalFace.SetName($"Face {index}");
                mirrorFace.SetName($"Face {index}");
                dict.Add(originalFace, mirrorFace);
            }

            for (int index = 0; index < edgePairs.Length; index += 2)
            {
                Edge originalEdge = edgePairs[index];
                Edge mirrorEdge = edgePairs[index + 1];
                originalEdge.SetName($"Edge {index / 2}");
                mirrorEdge.SetName($"Edge {index / 2}");
                dict.Add(originalEdge, mirrorEdge);
            }

            originalFeature.Unsuppress();
            mirrorFeature.Unsuppress();
        }

        public static bool TryMatchEdges(
            IEnumerable<Edge> originalEdges,
            IEnumerable<Edge> mirrorEdges,
            Surface.Plane plane,
            Component originalComp,
            Component mirrorComp,
            out Edge[] edgePairs)
        {
            IList<Edge> tempEdgePairs = new List<Edge>();
            ISet<Edge> originalEdges1 = originalEdges.ToHashSet();
            ISet<Edge> mirrorEdges1 = mirrorEdges.ToHashSet();

            while (originalEdges1.Count > 0)
            {
                Edge originalEdge = originalEdges1.First();
                originalEdges1.Remove(originalEdge);

                if (!TryMatchEdge(originalEdge, mirrorEdges1, plane, originalComp, mirrorComp, out Edge mirrorEdge))
                {
                    edgePairs = null;

                    return false;
                }

                mirrorEdges1.Remove(mirrorEdge);
                tempEdgePairs.Add(originalEdge);
                tempEdgePairs.Add(mirrorEdge);
            }

            edgePairs = tempEdgePairs.ToArray();
            return true;
        }

        public static bool TryMatchFaces(
            IEnumerable<Face> originalFaces,
            IEnumerable<Face> mirrorFaces,
            Surface.Plane plane,
            Component originalComp,
            Component mirrorComp,
            out Face[] facePairs)
        {
            IList<Face> tempFacePairs = new List<Face>();
            ISet<Face> originalFaces1 = originalFaces.ToHashSet();
            ISet<Face> mirrorFaces1 = mirrorFaces.ToHashSet();

            while (originalFaces1.Count > 0)
            {
                Face originalFace = originalFaces1.First();
                originalFaces1.Remove(originalFace);

                if (!TryMatchFace(originalFace, mirrorFaces1, plane, originalComp, mirrorComp, out Face mirrorFace))
                {
                    facePairs = null;

                    return false;
                }

                mirrorFaces1.Remove(mirrorFace);
                tempFacePairs.Add(originalFace);
                tempFacePairs.Add(mirrorFace);
            }

            facePairs = tempFacePairs.ToArray();
            return true;
        }

        public static bool TryMatchFace(
            Face originalFace, IEnumerable<Face> mirrorFaces,
            Surface.Plane plane, Component originalComp,
            Component mirrorComp, out Face mirrorFace)
        {
            Vector3d expectedMirrorVector = originalFace.__NormalVector()
                .__MirrorMap(plane, originalComp, mirrorComp)
                .__Unit();

            foreach (Face tempMirrorFace in mirrorFaces)
            {
                Vector3d mirrorVector = tempMirrorFace.__NormalVector().__Unit();

                if (!mirrorVector.__IsEqualTo(expectedMirrorVector))
                    continue;

                mirrorFace = tempMirrorFace;
                return true;
            }

            mirrorFace = null;
            return false;
        }

        public static bool TryMatchEdge(
            Edge originalEdge,
            IEnumerable<Edge> mirrorEdges,
            Surface.Plane plane,
            Component originalComp,
            Component mirrorComp,
            out Edge mirrorEdge)
        {
            Point3d expectedStartPoint = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirrorComp);
            Point3d expectedEndPoint = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirrorComp);

            foreach (Edge tempMirrorEdge in mirrorEdges)
                if (tempMirrorEdge.__HasEndPoints(expectedStartPoint, expectedEndPoint))
                {
                    mirrorEdge = tempMirrorEdge;
                    return true;
                }

            mirrorEdge = null;
            return false;
        }
    }
}