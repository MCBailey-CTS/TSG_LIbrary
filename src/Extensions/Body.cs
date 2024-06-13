using System;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public partial class __Extensions_
    {
        #region Body

        public static void __NXOpenBody(Body body)
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

        public static Box3d __Box3d(this Body body)
        {
            double[] array = new double[3];
            double[,] array2 = new double[3, 3];
            double[] array3 = new double[3];
            Tag tag = body.Tag;
            ufsession_.Modl.AskBoundingBoxExact(tag, Tag.Null, array, array2, array3);
            Point3d position = array.__ToPoint3d();
            Vector3d vector = new Vector3d(array2[0, 0], array2[0, 1], array2[0, 2]);
            Vector3d vector2 = new Vector3d(array2[1, 0], array2[1, 1], array2[1, 2]);
            Vector3d vector3 = new Vector3d(array2[2, 0], array2[2, 1], array2[2, 2]);
            Point3d maxXYZ = position.__Add(vector.__Multiply(array3[0])).__Add(vector2.__Multiply(array3[1]))
                .__Add(vector3.__Multiply(array3[2]));
            return new Box3d(position, maxXYZ);
        }

        public static BooleanFeature __Subtract(this Body target, params Body[] toolBodies)
        {
            return target.OwningPart.__CreateBoolean(target, toolBodies, Feature.BooleanType.Subtract);
        }

        public static Body __Prototype(this Body body)
        {
            return (Body)body.Prototype;
        }

        public static bool __InterferesWith(this Body target, Component component)
        {
            if (target.OwningPart.Tag != component.OwningPart.Tag)
                throw new ArgumentException("Body and component are not in the same assembly.");

            return target.__InterferesWith(component.__SolidBodyMembers());
        }

        public static bool __InterferesWith(this Body target, params Body[] toolBodies)
        {
            //if (toolBodies.Any(__b=>__b.OwningPart.Tag != target.OwningPart.Tag))
            //    throw new ArgumentException("At least one tool body is not in the same assembly as the target body.");

            Tag[] solid_bodies = toolBodies.Select(__b => __b.Tag).ToArray();
            int[] results = new int[solid_bodies.Length];
            ufsession_.Modl.CheckInterference(target.Tag, solid_bodies.Length, solid_bodies, results);

            for (int i = 0; i < solid_bodies.Length; i++)
                if (results[i] == 1)
                    return true;

            return false;
        }

        #endregion
    }
}