using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities;

namespace TSG_Library.UFuncs.MirrorComponents
{
    public class MirrorIntersect : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "INTERSECT";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
        }
    }
}