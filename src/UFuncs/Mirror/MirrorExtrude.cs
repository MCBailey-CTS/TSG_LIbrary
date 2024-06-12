using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorExtrude : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "EXTRUDE";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Part part2 = originalComp.__Prototype();
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