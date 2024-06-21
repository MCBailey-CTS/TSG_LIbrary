using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror.Features
{
    public class MirrorExtractedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRACT_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
            originalFeature.Unsuppress();

            dict[originalFeature].Tag.__To<Feature>().Unsuppress();
        }
    }
}