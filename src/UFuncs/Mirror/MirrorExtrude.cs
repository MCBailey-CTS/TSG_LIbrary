using System.Collections.Generic;
using CTS_Library.Extensions;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorExtrude : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRUDE";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Part part2 = originalComp._Prototype();
            originalFeature.Suppress();
            Extrude extrude = (Extrude)dict[originalFeature];
            ExtrudeBuilder extrudeBuilder = part2.Features.CreateExtrudeBuilder(originalFeature);
            Point3d val;
            Vector3d val2;
            using (new Destroyer(extrudeBuilder))
            {
                val = Extensions_Position._MirrorMap(extrudeBuilder.Direction.Origin, plane, originalComp, component);
                val2 = Extensions_Vector._MirrorMap(extrudeBuilder.Direction.Vector, plane, originalComp, component);
            }

            extrudeBuilder = part.Features.CreateExtrudeBuilder(extrude);
            using (new Destroyer(extrudeBuilder))
            {
                extrudeBuilder.Direction.Vector = val2;
                extrudeBuilder.Direction.Origin = val;
                extrudeBuilder.CommitFeature();
            }
        }
    }



}