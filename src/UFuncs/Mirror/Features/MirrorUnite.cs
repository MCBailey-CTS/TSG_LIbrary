using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.Mirror.Features
{
    public class MirrorUnite : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "UNITE";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict,
            Surface.Plane plane, Component originalComp)
        {
        }
    }
    //}
}