using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public abstract class BaseMirrorFeature : IMirrorFeature
    {
        public Point3d MirrorMap(Point3d position, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent.__Origin(), fromComponent.__Orientation());

            Point3d newStart = position.__MapWcsToAcs().__MirrorMap(plane, fromComponent, toComponent);

            __display_part_.WCS.SetOriginAndMatrix(toComponent.__Origin(), toComponent.__Orientation());

            return newStart.__MapAcsToWcs();
        }

        public Vector3d MirrorMap(Vector3d vector, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent.__Origin(), fromComponent.__Orientation());

            Vector3d newStart = vector.__MapWcsToAcs().__MirrorMap(plane, fromComponent, toComponent);

            __display_part_.WCS.SetOriginAndMatrix(toComponent.__Origin(), toComponent.__Orientation());

            return newStart.__MapAcsToWcs();
        }

        public Matrix3x3 MirrorMap(Matrix3x3 orientation, Surface.Plane plane, Component fromComponent,
            Component toComponent)
        {
            Vector3d newXVector = MirrorMap(orientation.__AxisY(), plane, fromComponent, toComponent);

            Vector3d newYVector = MirrorMap(orientation.__AxisX(), plane, fromComponent, toComponent);

            return newXVector.__ToMatrix3x3(newYVector);
        }

        public abstract string FeatureType { get; }

        public abstract void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp);

        public bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
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
    }
}