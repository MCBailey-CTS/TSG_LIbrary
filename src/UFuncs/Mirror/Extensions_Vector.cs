using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using Vector = NXOpen.Vector3d;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public static class Extensions_Vector
    {
        public static bool _IsPerpendicularTo(this Vector3d vec1, Vector3d vec2, double tolerance = 0.0001)
        {
            UFSession.GetUFSession().Vec3.IsPerpendicular(vec1.__ToArray(), vec2.__ToArray(), tolerance, out var is_perp);
            return is_perp == 1;
        }

        public static bool _IsParallelTo(this Vector3d vector1, Vector3d vector2, double tolerance = 0.0001)
        {
            UFSession.GetUFSession().Vec3.IsParallel(vector1.__ToArray(), vector2.__ToArray(), tolerance, out var is_parallel);
            return is_parallel == 1;
        }

        public static Vector3d _UnitVector(this Vector3d vector)
        {
            return vector.__Unit();
        }

        public static double _MagnitudeLength(this Vector3d vector)
        {
            return vector.__Norm();
        }

        public static Vector3d _AbsVector(this Vector3d vector)
        {
            return new Vector3d(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }

        public static Vector3d _Cross(this Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
        }

        public static double _DotProduct(this Vector3d vector1, Vector3d vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public static Matrix3x3 _CreateOrientationXVectorZVector(this Vector3d xVector, Vector3d zVector)
        {
            if (!xVector._IsPerpendicularTo(zVector))
                throw new InvalidOperationException("You cannot create an orientation from two vectors that are not perpendicular to each other.");

            Matrix3x3 val = xVector.__ToMatrix3x3(zVector);
            Vector3d axisZ = val.__AxisZ();
            return xVector.__ToMatrix3x3(axisZ);
        }

        public static Matrix3x3 _CreateOrientationYVectorZVector(this Vector3d yVector, Vector3d zVector)
        {
            if (!yVector._IsPerpendicularTo(zVector))
                throw new InvalidOperationException("You cannot create an orientation from two vectors that are not perpendicular to each other.");

            Matrix3x3 val = yVector.__ToMatrix3x3(zVector);
            Vector axisZ = val.__AxisZ();
            return axisZ.__ToMatrix3x3(yVector);
        }

        [Obsolete]
        public static Vector3d _Mirror(this Vector original, Surface.Plane plane)
        {
            //Transform val = Transform.CreateReflection(plane);
            //return ((Vector)(ref original)).Copy(val);
            throw new NotImplementedException();
        }

        [Obsolete]
        public static Vector _MirrorMap(this Vector vector, Surface.Plane mirrorPlane, Component originalComp, Component newComp)
        {
            throw new NotImplementedException();
            //originalComp._SetWcsToComponent();
            //Vector original = CoordinateSystem.MapWcsToAcs(vector);
            //Vector val = original._Mirror(mirrorPlane);
            //newComp._SetWcsToComponent();
            //return CoordinateSystem.MapAcsToWcs(val);
        }

        [Obsolete]
        public static Vector _MirrorMap(this Vector3d vector, Plane mirrorPlane, Component originalComp, Component newComp)
        {
            throw new NotImplementedException();
            //return _MirrorMap(new Vector(vector), mirrorPlane, originalComp, newComp);
        }

        [Obsolete]
        public static Vector3d _Mirror(this Vector3d original, Plane plane)
        {
            throw new NotImplementedException();
            //return Vector.op_Implicit(_Mirror(new Vector(original), plane));
        }

        public static double[] _Array(this Vector3d vector3D)
        {
            return new double[3] { vector3D.X, vector3D.Y, vector3D.Z };
        }
    }




}