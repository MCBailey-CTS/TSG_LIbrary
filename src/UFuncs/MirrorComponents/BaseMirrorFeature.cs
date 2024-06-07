using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public abstract class BaseMirrorFeature : IMirrorFeature
    {
        public Point3d MirrorMap(Point3d position, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent._Origin(), fromComponent._Orientation());

            var newStart = MapWcsToAcs(position)._MirrorMap(plane, fromComponent, toComponent);

            __display_part_.WCS.SetOriginAndMatrix(toComponent._Origin(), toComponent._Orientation());

            return MapAcsToWcs(newStart);
        }

        public Vector3d MirrorMap(Vector3d vector, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent._Origin(), fromComponent._Orientation());

            var newStart = MapWcsToAcs(vector)._MirrorMap(plane, fromComponent, toComponent);

            __display_part_.WCS.SetOriginAndMatrix(toComponent._Origin(), toComponent._Orientation());

            return MapAcsToWcs(newStart);
        }

        public Matrix3x3 MirrorMap(Matrix3x3 orientation, Surface.Plane plane, Component fromComponent,
            Component toComponent)
        {
            var newXVector = MirrorMap(orientation._AxisY(), plane, fromComponent, toComponent);

            var newYVector = MirrorMap(orientation._AxisX(), plane, fromComponent, toComponent);

            return newXVector._ToMatrix3x3(newYVector);
        }

        public abstract string FeatureType { get; }

        public abstract void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp);

        public bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            if(edgePoints.Count != mirrorFace.GetEdges().Length)
                return false;

            var faceEdges = new HashSet<Edge>(mirrorFace.GetEdges());

            var edge0 = faceEdges.First();
            faceEdges.Remove(edge0);

            var edge1 = faceEdges.First();
            faceEdges.Remove(edge1);

            var edge2 = faceEdges.First();
            faceEdges.Remove(edge2);

            var edge3 = faceEdges.First();
            faceEdges.Remove(edge3);

            ISet<Edge> matchedEdges = new HashSet<Edge>();

            foreach (var tuple in edgePoints)
            {
                if(edge0._HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge0);

                if(edge1._HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge1);

                if(edge2._HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge2);

                if(edge3._HasEndPoints(tuple.Item1, tuple.Item2))
                    matchedEdges.Add(edge3);
            }

            return matchedEdges.Count == mirrorFace.GetEdges().Length;
        }
    }
}