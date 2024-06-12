using System;
using System.Collections.Generic;
using System.Linq;
using CTS_Library.Extensions;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorBlock : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "BLOCK";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
            originalFeature.Unsuppress();
            Feature feature = (Feature)dict[originalFeature];
            feature.Unsuppress();
            Component mirrorComp = (Component)dict[originalComp];
            Block block = (Block)originalFeature;
            Block block2 = (Block)feature;
            if (!TryMatchFaces(block.GetFaces(), block2.GetFaces(), plane, originalComp, mirrorComp, out var facePairs))
            {
                throw new Exception("Unable to match faces in " + originalFeature.GetFeatureName());
            }

            if (!TryMatchEdges(block.GetEdges(), block2.GetEdges(), plane, originalComp, mirrorComp, out var edgePairs))
            {
                throw new Exception("Unable to match edges in " + originalFeature.GetFeatureName());
            }

            for (int i = 0; i < facePairs.Length; i += 2)
            {
                Face face = facePairs[i];
                Face face2 = facePairs[i + 1];
                face2._SetDisplayColor(face.Color);
                face2.RedisplayObject();
                face.SetName($"Face {i}");
                face2.SetName($"Face {i}");
                dict.Add(face, face2);
            }

            for (int j = 0; j < edgePairs.Length; j += 2)
            {
                Edge edge = edgePairs[j];
                Edge edge2 = edgePairs[j + 1];
                edge.SetName($"Edge {j / 2}");
                edge2.SetName($"Edge {j / 2}");
                dict.Add(edge, edge2);
            }

            originalFeature.Unsuppress();
            feature.Unsuppress();
        }

        public static bool TryMatchEdges(IEnumerable<Edge> originalEdges, IEnumerable<Edge> mirrorEdges, Plane plane, Component originalComp, Component mirrorComp, out Edge[] edgePairs)
        {
            IList<Edge> list = new List<Edge>();
            ISet<Edge> set = Extensions_Linq.ToHashSet(originalEdges);
            ISet<Edge> set2 = Extensions_Linq.ToHashSet(mirrorEdges);
            while (set.Count > 0)
            {
                Edge edge = set.First();
                set.Remove(edge);
                if (!TryMatchEdge(edge, set2, plane, originalComp, mirrorComp, out var mirrorEdge))
                {
                    edgePairs = null;
                    return false;
                }

                set2.Remove(mirrorEdge);
                list.Add(edge);
                list.Add(mirrorEdge);
            }

            edgePairs = list.ToArray();
            return true;
        }

        public static bool TryMatchFaces(IEnumerable<Face> originalFaces, IEnumerable<Face> mirrorFaces, Plane plane, Component originalComp, Component mirrorComp, out Face[] facePairs)
        {
            IList<Face> list = new List<Face>();
            ISet<Face> set = Extensions_Linq.ToHashSet(originalFaces);
            ISet<Face> set2 = Extensions_Linq.ToHashSet(mirrorFaces);
            while (set.Count > 0)
            {
                Face face = set.First();
                set.Remove(face);
                if (!TryMatchFace(face, set2, plane, originalComp, mirrorComp, out var mirrorFace))
                {
                    facePairs = null;
                    return false;
                }

                set2.Remove(mirrorFace);
                list.Add(face);
                list.Add(mirrorFace);
            }

            facePairs = list.ToArray();
            return true;
        }

        public static bool TryMatchFace(Face originalFace, IEnumerable<Face> mirrorFaces, Plane plane, Component originalComp, Component mirrorComp, out Face mirrorFace)
        {
            Vector3d val = originalFace.__NormalVector()._MirrorMap(plane, originalComp, mirrorComp);
            Vector3d vector = val.__Unit();
            foreach (Face mirrorFace2 in mirrorFaces)
            {
                val = mirrorFace2.__NormalVector();
                Vector3d vector2 = val.__Unit();
                if (!vector2.__IsEqualTo(vector))
                {
                    continue;
                }

                mirrorFace = mirrorFace2;
                return true;
            }

            mirrorFace = null;
            return false;
        }

        public static bool TryMatchEdge(Edge originalEdge, IEnumerable<Edge> mirrorEdges, Plane plane, Component originalComp, Component mirrorComp, out Edge mirrorEdge)
        {
            Point3d pos = originalEdge.__StartPoint()._MirrorMap(plane, originalComp, mirrorComp);
            Point3d pos2 = originalEdge.__EndPoint()._MirrorMap(plane, originalComp, mirrorComp);
            foreach (Edge mirrorEdge2 in mirrorEdges)
            {
                if (mirrorEdge2.__HasEndPoints(pos, pos2))
                {
                    mirrorEdge = mirrorEdge2;
                    return true;
                }
            }

            mirrorEdge = null;
            return false;
        }
    }



}