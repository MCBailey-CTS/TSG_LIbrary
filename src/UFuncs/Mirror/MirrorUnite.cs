using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorUnite : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "UNITE";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
        }
    }



}