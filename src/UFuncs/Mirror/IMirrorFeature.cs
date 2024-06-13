using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public interface IMirrorFeature
    {
        string FeatureType { get; }

        void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane,
            Component originalComp);

        Point3d MirrorMap(Point3d position, Surface.Plane plane, Component fromComponent, Component toComponent);

        Vector3d MirrorMap(Vector3d vector, Surface.Plane plane, Component fromComponent, Component toComponent);

        Matrix3x3 MirrorMap(Matrix3x3 orientation, Surface.Plane plane, Component fromComponent, Component toComponent);
    }
}