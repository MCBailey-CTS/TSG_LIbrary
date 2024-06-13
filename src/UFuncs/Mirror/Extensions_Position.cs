using System;
using System.Collections.Generic;
using System.Linq;
using CTS_Library.Extensions;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public static class Extensions_Position
    {
        //public static bool _IsEqualTo(this Point3d position1, Point3d position2, double tolerance)
        //{
        //    //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //    //IL_0007: Unknown result type (might be due to invalid IL or missing references)
        //    return new EqualityPosition(tolerance).Equals(position1, position2);
        //}

        //public static bool _IsEqualTo(this Point3d position1, Point3d position2)
        //{
        //    //IL_0000: Unknown result type (might be due to invalid IL or missing references)
        //    //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //    return position1._IsEqualTo(position2, 0.01);
        //}

        //public static bool _IsEqualTo(this Point3d vector1, Point3d vector2, double tolerance = 0.01)
        //{
        //    ufsession_.Vec3.IsEqual(((Vector)(ref vector1)).Array, ((Vector)(ref vector2)).Array, tolerance, out var is_equal);
        //    return is_equal switch
        //    {
        //        0 => false,
        //        1 => true,
        //        _ => throw NXException.Create(is_equal),
        //    };
        //}

        public static Point3d _MidPoint(this Point3d position1, Point3d position2)
        {
            return new Point3d((position1.X + position2.X) / 2.0, (position1.Y + position2.Y) / 2.0, (position1.Z + position2.Z) / 2.0);
        }

        //public static Point3d _MidPoint(this Point3d position1, Point3d position2)
        //{
        //    return new Point3d((position1.X + position2.X) / 2.0, (position1.Y + position2.Y) / 2.0, (position1.Z + position2.Z) / 2.0);
        //}

        public static Point _CreatePoint(this Part part, Point3d origin)
        {
            return part.Points.CreatePoint(origin);
        }

        public static Point3d _AveragePosition(this Point3d[] positions)
        {
            if (positions == null)
            {
                throw new ArgumentNullException("positions");
            }

            if (positions.Length == 0)
            {
                throw new ArgumentOutOfRangeException("positions");
            }

            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            for (int i = 0; i < positions.Length; i++)
            {
                num += positions[i].X;
                num2 += positions[i].Y;
                num3 += positions[i].Z;
            }

            return new Point3d(num, num2, num3);
        }

        public static Point3d _AveragePosition(this NXOpen. Curve[] curves)
        {
            return curves.SelectMany((NXOpen.Curve c) => (IEnumerable<Point3d>)(object)new Point3d[2]
            {
                c.__StartPoint(),
                c.__EndPoint()
            }).ToArray()._AveragePosition();
        }

        [Obsolete]
        public static Point3d _Transform(this Point3d original, Transform transform)
        {
            throw new NotImplementedException();
            //return ((Point3d)(ref original)).Copy(transform);
        }

        public static double[] _Array(this Point3d point3D)
        {
            return new double[3] { point3D.X, point3D.Y, point3D.Z };
        }

        public static Point3d _Mirror(this Point3d original, Surface. Plane plane)
        {
            Transform val = Transform.CreateReflection(plane);
            return original.__Copy(val);
        }

        public static Point3d _MirrorMap(this Point3d origin,Surface. Plane mirrorPlane, Component originalComp, Component newComp)
        {
            originalComp.__SetWcsToComponent();
            Point3d original = origin.__MapWcsToAcs();
            Point3d val = original._Mirror(mirrorPlane);
            newComp._SetWcsToComponent();
            return val.__MapAcsToWcs();
        }

        //public static Point3d _MirrorMap(this Point3d origin, Plane mirrorPlane, Component originalComp, Component newComp)
        //{
        //    return _MirrorMap(origin, mirrorPlane, originalComp, newComp);
        //}
    }



}