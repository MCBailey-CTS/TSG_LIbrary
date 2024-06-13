using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorSubtract : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "SUBTRACT";

        public override void Mirror(
            Feature originalFeature,
            IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane,
            Component originalComp)
        {
            Feature mirrorFeature = (Feature)dict[originalFeature];

            originalFeature.Unsuppress();

            mirrorFeature.Unsuppress();
        }
    }
}