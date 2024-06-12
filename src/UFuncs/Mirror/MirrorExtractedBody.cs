using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorExtractedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRACT_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
        }
    }



}