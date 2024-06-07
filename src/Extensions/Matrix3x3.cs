using System;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        /// <summary>
        ///     Maps an orientation from one coordinate system to another.
        /// </summary>
        /// <param name="orientation">The components of the given Orientation with the input coordinate system.</param>
        /// <param name="inputCsys">The input coordinate system.</param>
        /// <param name="outputCsys">The output coordinate system.</param>
        /// <returns>The components of the Orientation with the output coordinate system.</returns>
        public static Matrix3x3 MapCsysToCsys(
            Matrix3x3 orientation,
            CartesianCoordinateSystem inputCsys,
            CartesianCoordinateSystem outputCsys)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            var mappedXVector = __MapCsysToCsys(orientation._AxisX(), inputCsys, outputCsys);
            var mappedYVector = __MapCsysToCsys(orientation._AxisY(), inputCsys, outputCsys);
#pragma warning restore CS0612 // Type or member is obsolete
            return mappedXVector._ToMatrix3x3(mappedYVector);
        }


        public static Matrix3x3 _MirrorMap(this Matrix3x3 orientation, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            var newXVector = _MirrorMap(orientation._AxisY(), plane, fromComponent, toComponent);

            var newYVector = _MirrorMap(orientation._AxisX(), plane, fromComponent, toComponent);

            return newXVector._ToMatrix3x3(newYVector);
        }

        public static Vector3d _AxisY(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Yx, matrix.Yy, matrix.Yz);
        }

        public static Vector3d _AxisZ(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Zx, matrix.Zy, matrix.Zz);
        }

        public static Matrix3x3 __MapAcsToWcs(this Matrix3x3 __ori)
        {
            var x_vec = MapAcsToWcs(__ori._AxisX())._ToArray();
            var y_vec = MapAcsToWcs(__ori._AxisY())._ToArray();
            var z_vec = MapAcsToWcs(__ori._AxisZ())._ToArray();
            return x_vec.Concat(y_vec).Concat(z_vec).ToArray()._ToMatrix3x3();
        }


        public static double[] _Array(this Matrix3x3 matrix)
        {
            return new[]
            {
                matrix.Xx, matrix.Xy, matrix.Xz,
                matrix.Yx, matrix.Yy, matrix.Yz,
                matrix.Zx, matrix.Zy, matrix.Zz
            };
        }

        public static Vector3d _AxisX(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Xx, matrix.Xy, matrix.Xz);
        }

        public static Matrix3x3 _Mirror(this Matrix3x3 matrix, Surface.Plane plane)
        {
            var new_y = matrix._AxisX()._Mirror(plane);
            var new_x = matrix._AxisY()._Mirror(plane);
            return new_x._ToMatrix3x3(new_y);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Matrix3x3 _Mirror(
            this Matrix3x3 vector,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }


        public static Matrix3x3 _Copy(this Matrix3x3 matrix)
        {
            var mtx_dst = new double[9];
            ufsession_.Mtx3.Copy(matrix._Array(), mtx_dst);
            return mtx_dst._ToMatrix3x3();
        }

        public static double[,] _ToTwoDimArray(this Matrix3x3 matrix)
        {
            return new double[3, 3]
            {
                { matrix.Xx, matrix.Xy, matrix.Xz },
                { matrix.Yx, matrix.Yy, matrix.Yz },
                { matrix.Zx, matrix.Zy, matrix.Zz }
            };
        }

        public static double _Determinant(this Matrix3x3 matrix)
        {
            ufsession_.Mtx3.Determinant(matrix._Array(), out var determinant);
            return determinant;
        }
    }
}