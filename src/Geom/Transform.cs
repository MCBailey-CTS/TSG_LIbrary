using System;
using NXOpen;
using NXOpen.UF;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Geom
{
    /// <summary>
    ///     Class for building and applying NX transformations
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
    ///         functions.
    ///     </para>
    /// </remarks>
    public class Transform
    {
        /// <summary>Constructor, given an array of doubles (the matrix elements)</summary>
        /// <param name="array">Array of 12 doubles (the matrix elements)</param>
        /// <remarks>
        ///     <para>
        ///         The transform moves a given point (x, y, z) to a new location (xnew, ynew, znew) as follows:
        ///     </para>
        ///     <para>
        ///         xnew   =   x*m[0]   +   y*m[1]   +   z*m[2]     + m[3]
        ///     </para>
        ///     <para>
        ///         ynew   =   x*m[4]   +   y*m[5]   +   z*m[6]     + m[7]
        ///     </para>
        ///     <para>
        ///         znew   =   x*m[8]   +   y*m[9]   +   z*m[10]     + m[11]
        ///     </para>
        /// </remarks>
        private Transform(double[] array)
        {
            Matrix = array;
        }

        /// <summary>Array of 12 doubles (the matrix elements)</summary>
        /// <remarks>
        ///     <para>
        ///         The matrix is an array of 12 doubles.
        ///         Applying the transform works as follows:
        ///     </para>
        ///     <para>
        ///         <c>      xnew = x*m[0] + y*m[1] + z*m[2]   + m[3]</c><br />
        ///         <c>      ynew = x*m[4] + y*m[5] + z*m[6]   + m[7]</c><br />
        ///         <c>      znew = x*m[8] + y*m[9] + z*m[10]  + m[11]</c><br />
        ///     </para>
        /// </remarks>
        public double[] Matrix { get; internal set; }

        /// <summary>Creates a transform that performs translation</summary>
        /// <param name="v">Translation vector</param>
        /// <returns>Transform that performs the translation</returns>
        /// <remarks>
        ///     <para>
        ///         The transform can be applied to Positions and NX objects by calling the appropriate Move or Copy functions.
        ///         Note that translating vectors doesn't really make sense.
        ///     </para>
        /// </remarks>
        public static Transform CreateTranslation(Vector3d v)
        {
            return CreateTranslation(v.X, v.Y, v.Z);
        }

        /// <summary>Creates a transform that performs translation</summary>
        /// <param name="dx">Displacement in the X-direction</param>
        /// <param name="dy">Displacement in the Y-direction</param>
        /// <param name="dz">Displacement in the Z-direction</param>
        /// <returns>Transform that performs the translation</returns>
        /// <remarks>
        ///     <para>
        ///         The transform can be applied to Positions and NX objects by calling the appropriate Move or Copy functions.
        ///         Note that translating vectors doesn't really make sense.
        ///     </para>
        /// </remarks>
        public static Transform CreateTranslation(double dx, double dy, double dz)
        {
            return new Transform(new double[12] { 1.0, 0.0, 0.0, dx, 0.0, 1.0, 0.0, dy, 0.0, 0.0, 1.0, dz });
        }

        /// <summary>Creates an identify translation</summary>
        /// <returns>Transform that performs an identity (null) translation</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateTranslation()
        {
            return CreateTranslation(0.0, 0.0, 0.0);
        }

        /// <summary>Creates a transform that performs rotation around a given axis</summary>
        /// <param name="basePoint">Point on the axis</param>
        /// <param name="axis">Vector along the axis</param>
        /// <param name="angle">Angle of rotation (in degrees)</param>
        /// <returns>Transform that performs the rotation</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateRotation(Point3d basePoint, Vector3d axis, double angle)
        {
            var array = new double[12];
            ufsession_.Trns.CreateRotationMatrix(basePoint.__ToArray(), axis.__ToArray(), ref angle, array, out var _);
            return new Transform(array);
        }

        /// <summary>Creates a transform that performs 2D rotation around a point in the XY-plane</summary>
        /// <param name="basePoint">Point (presumably on the XY-plane</param>
        /// <param name="angle">Angle of rotation (in degrees)</param>
        /// <returns>Transform that performs the rotation</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateRotation(Point3d basePoint, double angle)
        {
            return CreateRotation(basePoint, __Vector3dZ(), angle);
        }

        /// <summary>Creates a transform that performs rotation defined by an orientation</summary>
        /// <param name="orientation">The orientation</param>
        /// <returns>Transform that performs the rotation</returns>
        /// <remarks>
        ///     <para>
        ///         The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///         functions.
        ///     </para>
        ///     <para>
        ///         The transform returned is a rotation around a line through the origin that would move the ACS axes so as to
        ///         align them with the
        ///         axes of the given orientation. In NX terms, this is a "reposition" transformation.
        ///     </para>
        /// </remarks>
        /// <returns>Transform that performs the rotation</returns>
        [Obsolete(nameof(NotImplementedException))]
        public static Transform CreateRotation(Matrix3x3 orientation)
        {
            throw new NotImplementedException();
            //double[,] array = orientation._ToTwoDimArray();
            //return new Transform(new double[12]
            //{
            //    array[0, 0],
            //    array[1, 0],
            //    array[2, 0],
            //    0.0,
            //    array[0, 1],
            //    array[1, 1],
            //    array[2, 1],
            //    0.0,
            //    array[0, 2],
            //    array[1, 2],
            //    array[2, 2],
            //    0.0
            //});
        }

        //public static double[,] _ToTwoDimArray(this NXOpen.Matrix3x3 matrix)
        //{
        //    return new Transform(new double[12]
        //    {
        //        matrix.Xx,
        //        matrix.Xy,
        //        matrix.Xz,
        //        0.0,
        //        matrix.Yx,
        //        matrix.Yy,
        //        matrix.Yz,
        //        0.0,
        //        matrix.Zx,
        //        matrix.Zy,
        //        matrix.Zz,
        //        0.0
        //    });
        //}

        /// <summary>Creates a transform that performs a (non-uniform) scaling operation</summary>
        /// <param name="basePoint">The basePoint for the scaling operation</param>
        /// <param name="scaleFactors">The scaling factors for the X, Y, Z directions</param>
        /// <returns>Transform that performs the scaling operation</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateScale(Point3d basePoint, double[] scaleFactors)
        {
            var array = new double[12];
            var type = 2;
            ufsession_.Trns.CreateScalingMatrix(ref type, scaleFactors, basePoint.__ToArray(), array, out var _);
            return new Transform(array);
        }

        /// <summary>Creates a transform that performs a (uniform) scaling operation</summary>
        /// <param name="basePoint">The basePoint for the scaling operation</param>
        /// <param name="scaleFactor">The scaling factor</param>
        /// <returns>Transform that performs the scaling operation</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateScale(Point3d basePoint, double scaleFactor)
        {
            UFSession uFSession = ufsession_;
            var array = new double[12];
            var type = 1;
            var scales = new double[3] { scaleFactor, 1.0, 1.0 };
            uFSession.Trns.CreateScalingMatrix(ref type, scales, basePoint.__ToArray(), array, out var _);
            return new Transform(array);
        }

        /// <summary>Creates a transform that performs reflection through a given plane</summary>
        /// <param name="plane">The plane of reflection</param>
        /// <returns>Transform that performs the reflection</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateReflection(Surface.Plane plane)
        {
            return CreateReflection(plane.Normal, plane.D);
        }

        /// <summary>Creates a transform that performs reflection through a given plane</summary>
        /// <param name="normal">The normal of the plane (unit vector)</param>
        /// <param name="d">Signed distance from origin to plane (plane equation is X*Normal = d)</param>
        /// <returns>Transform that performs the reflection</returns>
        /// <remarks>
        ///     The transform can be applied to Positions, Vectors, and NX objects by calling the appropriate Move or Copy
        ///     functions.
        /// </remarks>
        public static Transform CreateReflection(Vector3d normal, double d)
        {
            var array = new double[12];
            var x = normal.X;
            var y = normal.Y;
            var z = normal.Z;
            array[0] = 1.0 - 2.0 * x * x;
            array[1] = -2.0 * x * y;
            array[2] = -2.0 * x * z;
            array[3] = 2.0 * x * d;
            array[4] = -2.0 * y * x;
            array[5] = 1.0 - 2.0 * y * y;
            array[6] = -2.0 * y * z;
            array[7] = 2.0 * y * d;
            array[8] = -2.0 * z * x;
            array[9] = -2.0 * z * y;
            array[10] = 1.0 - 2.0 * z * z;
            array[11] = 2.0 * z * d;
            return new Transform(array);
        }

        /// <summary>Forms the composition of two transforms (one followed by the other)</summary>
        /// <param name="xform1">First transform</param>
        /// <param name="xform2">Second transform</param>
        /// <returns>Composition of two transforms (first one followed by second one)</returns>
        /// <remarks>
        ///     <para>
        ///         The order of the transforms matters. Composition(xform1, xform2) is not the
        ///     </para>
        ///     same as Composition(xform2, xform1)
        /// </remarks>
        public static Transform Composition(Transform xform1, Transform xform2)
        {
            UFSession uFSession = ufsession_;
            var array = new double[12];
            uFSession.Trns.MultiplyMatrices(xform1.Matrix, xform2.Matrix, array);
            return new Transform(array);
        }
    }
}