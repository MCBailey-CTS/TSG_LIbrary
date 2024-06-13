using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public abstract class BaseMirrorFeature : IMirrorFeature
    {
        public abstract string FeatureType { get; }

        public Point3d MirrorMap(Point3d position, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            fromComponent.__SetWcsToComponent();
            Point3d val = position.__MapWcsToAcs()._Mirror(plane);
            toComponent.__SetWcsToComponent();
            return val.__MapAcsToWcs();
        }

        public Vector3d MirrorMap(Vector3d vector, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            fromComponent.__SetWcsToComponent();
            Vector3d val = vector.__MapWcsToAcs()._Mirror(plane);
            toComponent.__SetWcsToComponent();
            return val.__MapAcsToWcs();
        }

        public Matrix3x3 MirrorMap(Matrix3x3 orientation, Surface.Plane plane, Component fromComponent, Component toComponent)
        {
            Vector3d val = MirrorMap(orientation.__AxisY(), plane, fromComponent, toComponent);
            Vector3d val2 = MirrorMap(orientation.__AxisX(), plane, fromComponent, toComponent);
            return val.__ToMatrix3x3(val2);
        }

        public abstract void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane, Component originalComp);

        public bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            if (edgePoints.Count != mirrorFace.GetEdges().Length)
            {
                return false;
            }

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
    }




}