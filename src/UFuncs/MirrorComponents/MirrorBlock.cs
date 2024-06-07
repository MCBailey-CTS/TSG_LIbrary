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
            var mirrorFeature = (Feature)dict[originalFeature];
            mirrorFeature.Unsuppress();
            var mirrorComp = (Component)dict[originalComp];
            var originalBlock = (Block)originalFeature;
            var mirrorBlock = (Block)mirrorFeature;

            if (!TryMatchFaces(originalBlock.GetFaces(), mirrorBlock.GetFaces(), plane, originalComp, mirrorComp,
                    out var facePairs))
                throw new Exception($"Unable to match faces in {originalFeature.GetFeatureName()}");

            if (!TryMatchEdges(originalBlock.GetEdges(), mirrorBlock.GetEdges(), plane, originalComp, mirrorComp,
                    out var edgePairs))
                throw new Exception($"Unable to match edges in {originalFeature.GetFeatureName()}");

            for (var index = 0; index < facePairs.Length; index += 2)
            {
                var originalFace = facePairs[index];
                var mirrorFace = facePairs[index + 1];
                mirrorFace.__Color(originalFace.Color);
                mirrorFace.RedisplayObject();
                originalFace.SetName($"Face {index}");
                mirrorFace.SetName($"Face {index}");
                dict.Add(originalFace, mirrorFace);
            }

            for (var index = 0; index < edgePairs.Length; index += 2)
            {
                var originalEdge = edgePairs[index];
                var mirrorEdge = edgePairs[index + 1];
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
                var originalEdge = originalEdges1.First();
                originalEdges1.Remove(originalEdge);

                if (!TryMatchEdge(originalEdge, mirrorEdges1, plane, originalComp, mirrorComp, out var mirrorEdge))
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
                var originalFace = originalFaces1.First();
                originalFaces1.Remove(originalFace);

                if (!TryMatchFace(originalFace, mirrorFaces1, plane, originalComp, mirrorComp, out var mirrorFace))
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
            var expectedMirrorVector = originalFace._NormalVector()
                ._MirrorMap(plane, originalComp, mirrorComp)
                ._Unit();

            foreach (var tempMirrorFace in mirrorFaces)
            {
                var mirrorVector = tempMirrorFace._NormalVector()._Unit();

                if (!mirrorVector._IsEqualTo(expectedMirrorVector))
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
            var expectedStartPoint = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirrorComp);
            var expectedEndPoint = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirrorComp);

            foreach (var tempMirrorEdge in mirrorEdges)
                if (tempMirrorEdge._HasEndPoints(expectedStartPoint, expectedEndPoint))
                {
                    mirrorEdge = tempMirrorEdge;
                    return true;
                }

            mirrorEdge = null;
            return false;
        }
    }
}