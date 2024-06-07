using System;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static void NXOpen_Body(Body body)
        {
            //body.Density
            //body.GetEdges
            //body.GetFaces
            //body.GetFeatures
            //body.IsConvergentBody
            //body.IsSheetBody
            //body.IsSolidBody
            //body.OwningPart.Features.GetAssociatedFeaturesOfBody
            //body.OwningPart.Features.GetParentFeatureOfBody
        }

        public static Box3d _Box3d(this Body body)
        {
            var array = new double[3];
            var array2 = new double[3, 3];
            var array3 = new double[3];
            var tag = body.Tag;
            ufsession_.Modl.AskBoundingBoxExact(tag, Tag.Null, array, array2, array3);
            var position = array._ToPoint3d();
            var vector = new Vector3d(array2[0, 0], array2[0, 1], array2[0, 2]);
            var vector2 = new Vector3d(array2[1, 0], array2[1, 1], array2[1, 2]);
            var vector3 = new Vector3d(array2[2, 0], array2[2, 1], array2[2, 2]);
            var maxXYZ = position._Add(vector._Multiply(array3[0]))._Add(vector2._Multiply(array3[1]))
                ._Add(vector3._Multiply(array3[2]));
            return new Box3d(position, maxXYZ);
        }

        public static BooleanFeature _Subtract(this Body target, params Body[] toolBodies)
        {
            return target.OwningPart.__CreateBoolean(target, toolBodies, Feature.BooleanType.Subtract);
        }

        public static Body _Prototype(this Body body)
        {
            return (Body)body.Prototype;
        }

        public static bool _InterferesWith(this Body target, Component component)
        {
            if(target.OwningPart.Tag != component.OwningPart.Tag)
                throw new ArgumentException("Body and component are not in the same assembly.");

            return target._InterferesWith(component.solid_body_memebers());
        }

        public static bool _InterferesWith(this Body target, params Body[] toolBodies)
        {
            //if (toolBodies.Any(__b=>__b.OwningPart.Tag != target.OwningPart.Tag))
            //    throw new ArgumentException("At least one tool body is not in the same assembly as the target body.");

            var solid_bodies = toolBodies.Select(__b => __b.Tag).ToArray();
            var results = new int[solid_bodies.Length];
            ufsession_.Modl.CheckInterference(target.Tag, solid_bodies.Length, solid_bodies, results);

            for (var i = 0; i < solid_bodies.Length; i++)
                if(results[i] == 1)
                    return true;

            return false;
        }
    }
}