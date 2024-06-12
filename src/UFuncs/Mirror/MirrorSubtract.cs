using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorSubtract : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "SUBTRACT";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
            Feature feature = (Feature)dict[originalFeature];
            originalFeature.Unsuppress();
            feature.Unsuppress();
        }
    }



}