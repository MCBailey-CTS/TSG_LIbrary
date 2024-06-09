using System;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Vector3d

        /// <summary>Map a vector from Absolute coordinates to Work coordinates</summary>
        /// <param name="absVector">The components of the given vector wrt the Absolute Coordinate System (ACS)</param>
        /// <returns>The components of the given vector wrt the Work Coordinate System (WCS)</returns>
        public static Vector3d __MapAcsToWcs(Vector3d absVector)
        {
            var axisX = __display_part_.WCS.__AxisX();
            var axisY = __display_part_.WCS.__AxisY();
            var axisZ = __display_part_.WCS.__AxisZ();
            var x = axisX.__Multiply(absVector);
            var y = axisY.__Multiply(absVector);
            var z = axisZ.__Multiply(absVector);
            return new Vector3d(x, y, z);
        }

        /// <summary>
        ///     Map a vector from one coordinate system to another
        /// </summary>
        /// <param name="inputVector">The components of the given vector wrt the input coordinate system</param>
        /// <param name="inputCsys">The input coordinate system</param>
        /// <param name="outputCsys">The output coordinate system</param>
        /// <returns>The components of the given vector wrt the output coordinate system</returns>
        public static Vector3d __MapCsysToCsys(this Vector3d inputVector,
            CartesianCoordinateSystem inputCsys, CartesianCoordinateSystem outputCsys)
        {
            var axisX = inputCsys.__Orientation().Element.__AxisX();
            var axisY = inputCsys.__Orientation().Element.__AxisY();
            var axisZ = inputCsys.__Orientation().Element.__AxisZ();
            var x = inputVector.X.__Multiply(axisX);
            var y = inputVector.Y.__Multiply(axisY);
            var z = inputVector.Z.__Multiply(axisZ);
            var vector = x.__Add(y, z);
            var x0 = vector.__Multiply(outputCsys.__Orientation().Element.__AxisX());
            var y0 = vector.__Multiply(outputCsys.__Orientation().Element.__AxisY());
            var z0 = vector.__Multiply(outputCsys.__Orientation().Element.__AxisZ());
            return new Vector3d(x0, y0, z0);
        }

        public static Vector3d __Add(this Vector3d vector, params Vector3d[] vectors)
        {
            var result = vector.__Copy();

            foreach (var v in vectors)
                result = result.__Add(v);

            return result;
        }


        /// <summary>Calculates the cross product (vector product) of two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Cross product</returns>
        /// <remarks>
        ///     <para>
        ///         As is well known, order matters: Cross(u,v) = - Cross(v,u)
        ///     </para>
        /// </remarks>
        public static Vector3d __Cross(this Vector3d u, Vector3d v)
        {
            return new Vector3d(u.Y * v.Z - u.Z * v.Y, u.Z * v.X - u.X * v.Z, u.X * v.Y - u.Y * v.X);
        }

        /// <summary>Calculates the unitized cross product (vector product) of two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Unitized cross product</returns>
        /// <remarks>
        ///     <para>
        ///         If the cross product is the zero vector, then each component
        ///         of the returned vector will be NaN (not a number).
        ///     </para>
        /// </remarks>
        public static Vector3d __UnitCross(this Vector3d u, Vector3d v)
        {
            return u.__Cross(v).__Unit();
        }

        /// <summary>Unitizes a given vector</summary>
        /// <param name="u">Vector to be unitized</param>
        /// <returns>Unit vector in same direction</returns>
        /// <remarks>
        ///     <para>
        ///         If the input is the zero vector is zero, then each component
        ///         of the returned vector will be NaN (not a number).
        ///     </para>
        /// </remarks>
        public static Vector3d __Unit(this Vector3d u)
        {
            var num = 1.0 / u.__Norm();
            return new Vector3d(num * u.X, num * u.Y, num * u.Z);
        }

        /// <summary>Calculates the norm squared (length squared) of a vector</summary>
        /// <param name="u">The vector</param>
        /// <returns>Norm (length) squared of vector</returns>
        public static double __Norm2(this Vector3d u)
        {
            return u.X * u.X + u.Y * u.Y + u.Z * u.Z;
        }

        public static Vector3d __Divide(this Vector3d vector, double divisor)
        {
            return new Vector3d(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }

        /// <summary>Calculates the norm (length) of a vector</summary>
        /// <param name="u">The vector</param>
        /// <returns>Norm (length) of vector</returns>
        public static double __Norm(this Vector3d u)
        {
            return Math.Sqrt(u.X * u.X + u.Y * u.Y + u.Z * u.Z);
        }

        public static Matrix3x3 __ToMatrix3x3(this Vector3d axisZ)
        {
            var u = !(System.Math.Abs(axisZ.X) < System.Math.Abs(axisZ.Y))
                ? new Vector3d(0.0, 1.0, 0.0)
                : new Vector3d(1.0, 0.0, 0.0);

            axisZ = axisZ.__Unit();
            var v = u.__Cross(axisZ).__Unit();
            var vector = axisZ.__Cross(v).__Unit();
            return new Matrix3x3
            {
                Xx = v.X,
                Xy = v.Y,
                Xz = v.Z,
                Yx = vector.X,
                Yy = vector.Y,
                Yz = vector.Z,
                Zx = axisZ.X,
                Zy = axisZ.Y,
                Zz = axisZ.Z
            };
        }

        public static Vector3d __Mirror(this Vector3d vector, Surface.Plane plane)
        {
            var transform = Transform.CreateReflection(plane);
            return vector.__Copy(transform);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Vector3d __Mirror(
            this Vector3d vector,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static Matrix3x3 __ToMatrix3x3(this Vector3d xDir, Vector3d yDir)
        {
            var array = new double[9];
            ufsession_.Mtx3.Initialize(xDir.__ToArray(), yDir.__ToArray(), array);
            return array.__ToMatrix3x3();
        }

        public static Matrix3x3 __ToMatrix3x3(this Vector3d axisX, Vector3d axisY,
            Vector3d axisZ)
        {
            var element = default(Matrix3x3);
            element.Xx = axisX.X;
            element.Xy = axisX.Y;
            element.Xz = axisX.Z;
            element.Yx = axisY.X;
            element.Yy = axisY.Y;
            element.Yz = axisY.Z;
            element.Zx = axisZ.X;
            element.Zy = axisZ.Y;
            element.Zz = axisZ.Z;
            return element;
        }

        public static Vector3d __AskPerpendicular(this Vector3d vec)
        {
            var __vec_perp = new double[3];
            ufsession_.Vec3.AskPerpendicular(vec.__ToArray(), __vec_perp);
            return new Vector3d(__vec_perp[0], __vec_perp[1], __vec_perp[2]);
        }

        public static Vector3d __Add(this Vector3d vec0, Vector3d vec1)
        {
            return new Vector3d(vec0.X + vec1.X, vec0.Y + vec1.Y, vec0.Z + vec1.Z);
        }

        public static Vector3d __Subtract(this Vector3d vec0, Vector3d vec1)
        {
            var negate = vec1.__Negate();
            return vec0.__Add(negate);
        }

        public static Vector3d __Negate(this Vector3d vec0)
        {
            return new Vector3d(-vec0.X, -vec0.Y, -vec0.Z);
        }

        public static double[] __Array(this Vector3d vector)
        {
            return new[] { vector.X, vector.Y, vector.Z };
        }

        /// <summary>Calculates the angle in degrees between two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>The angle, theta, in degrees, where 0 ≤ theta ≤ 180</returns>
        public static double __Angle(this Vector3d u, Vector3d v)
        {
            var val = u.__Unit().__Multiply(v.__Unit());
            val = System.Math.Min(1.0, val);
            val = System.Math.Max(-1.0, val);
            return System.Math.Acos(val) * 180.0 / System.Math.PI;
        }

        public static bool __IsPerpendicularTo(this Vector3d vec1, Vector3d vec2, double tolerance = .0001)
        {
            ufsession_.Vec3.IsPerpendicular(vec1.__ToArray(), vec2.__ToArray(), tolerance, out var result);
            return result == 1;
        }

        public static double[] __ToArray(this Vector3d vector3d)
        {
            return new[] { vector3d.X, vector3d.Y, vector3d.Z };
        }

        //
        // Summary:
        //     Transforms/copies a vector
        //
        // Parameters:
        //   xform:
        //     The transformation to apply
        //
        // Returns:
        //     A transformed copy of the original input vector
        public static Vector3d __Copy(this Vector3d vector, Transform xform)
        {
            var matrix = xform.Matrix;
            var x = vector.X;
            var y = vector.Y;
            var z = vector.Z;
            var x2 = x * matrix[0] + y * matrix[1] + z * matrix[2];
            var y2 = x * matrix[4] + y * matrix[5] + z * matrix[6];
            var z2 = x * matrix[8] + y * matrix[9] + z * matrix[10];
            return new Vector3d(x2, y2, z2);
        }

        /// <summary>Copies a vector</summary>
        /// <param name="vector">The vector copy from</param>
        /// <returns>A copy of the original input vector</returns>
        public static Vector3d __Copy(this Vector3d vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        public static bool __IsEqual(this Vector3d vec0, Vector3d vec1, double tolerance = .001)
        {
            ufsession_.Vec3.IsEqual(vec0.__ToArray(), vec1.__ToArray(), tolerance, out var is_equal);

            switch (is_equal)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(is_equal);
            }
        }

        public static bool __IsEqualTo(this Vector3d vector1, Vector3d vector2, double tolerance = .01)
        {
            // Compares the two vectors. If they are equal, then {isEqual} == 1, else {isEqual} == 0.
            ufsession_.Vec3.IsEqual(vector1.__ToArray(), vector2.__ToArray(), tolerance, out var isEqual);

            switch (isEqual)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(isEqual);
            }
        }

        /// <summary>
        ///     Returns a 3x3 matrix formed from two input 3D vectors. The two<br />
        ///     input vectors are normalized and the y-direction vector is made<br />
        ///     orthogonal to the x-direction vector before taking the cross product<br />
        ///     (x_vec X y_vec) to generate the z-direction vector.
        /// </summary>
        /// <param name="xVec">Vector for the X-direction of matrix</param>
        /// <param name="yVec">Vector for theYX-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 __Initialize(this Vector3d xVec, Vector3d yVec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.Initialize(xVec.__ToArray(), yVec.__ToArray(), mtx);
            return mtx.__ToMatrix3x3();
        }

        /// <summary>
        ///     Returns a 3x3 matrix with the given X-direction vector and having<br />
        ///     arbitrary Y- and Z-direction vectors.
        /// </summary>
        /// <param name="xVec">Vector for the X-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 __InitializeX(Vector3d xVec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.InitializeX(xVec.__ToArray(), mtx);
            return mtx.__ToMatrix3x3();
        }

        /// <summary>
        ///     Returns a 3x3 matrix with the given Z-direction vector and having<br />
        ///     arbitrary X- and Y-direction vectors.
        /// </summary>
        /// <param name="z_vec">Vector for the Z-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 __InitializeZ(Vector3d z_vec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.InitializeZ(z_vec.__ToArray(), mtx);
            return mtx.__ToMatrix3x3();
        }

        public static Point3d __Add(this Vector3d vector, Point3d point)
        {
            return point.__Add(vector);
        }

        /// <summary>Map a vector from Work coordinates to Absolute coordinates</summary>
        /// <param name="workVector">The components of the given vector wrt the Work Coordinate System (WCS)</param>
        /// <returns>The components of the given vector wrt the Absolute Coordinate System (ACS)</returns>
        /// <remarks>
        ///     <para>
        ///         If you are given vector components relative to the WCS, then you will need to
        ///         use this function to "map" them to the ACS before using them in SNAP functions.
        ///     </para>
        /// </remarks>
        public static Vector3d __MapWcsToAcs(Vector3d workVector)
        {
            var axisX = __display_part_.WCS.__AxisX();
            var axisY = __display_part_.WCS.__AxisY();
            var axisZ = __display_part_.WCS.__AxisZ();
            return axisX.__Multiply(workVector.X).__Add(axisY.__Multiply(workVector.Y))
                .__Add(axisZ.__Multiply(workVector.Z));
        }

        public static Vector3d __Multiply(this Vector3d vector3d, double scale)
        {
            var scaled_vec = new double[3];
            ufsession_.Vec3.Scale(scale, vector3d.__ToArray(), scaled_vec);
            return scaled_vec.__ToVector3d();
        }

        public static double __Multiply(this Vector3d vec0, Vector3d vec1)
        {
            return vec0.X * vec1.X + vec0.Y * vec1.Y + vec0.Z * vec1.Z;
        }

        public static Matrix3x3 __MapWcsToAcs(Matrix3x3 orientation)
        {
            var axisX = __MapWcsToAcs(orientation.__AxisX());
            var axisY = __MapWcsToAcs(orientation.__AxisY());
            return axisX.__ToMatrix3x3(axisY);
        }

        public static Vector3d __MirrorMap(this Vector3d vector, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            var origin = fromComponent.__Origin();
            var orientation = fromComponent.__Orientation();
            __display_part_.WCS.SetOriginAndMatrix(origin, orientation);
            var newStart = __MapWcsToAcs(vector);
            __display_part_.WCS.SetOriginAndMatrix(toComponent.__Origin(), toComponent.__Orientation());
            return __MapAcsToWcs(newStart);
        }

        public static Matrix3x3 __Mtx3Initialize(Vector3d x_vec, Vector3d y_vec)
        {
            var matrix = new double[9];
            ufsession_.Mtx3.Initialize(x_vec.__ToArray(), y_vec.__ToArray(), matrix);
            return matrix.__ToMatrix3x3();
        }

        #endregion
    }
}