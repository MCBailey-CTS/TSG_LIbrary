namespace TSG_Library
{
    //
    // Summary:
    //     Mostly trigonometric functions that handle angles in degrees, rather than radians
    public static class Math
    {
        public static int PI { get; internal set; }

        //
        // Summary:
        //     Converts degrees to radians
        //
        // Parameters:
        //   angle:
        //     An angle measured in degrees
        //
        // Returns:
        //     The same angle measured in radians
        public static double DegreesToRadians(double angle)
        {
            return angle * System.Math.PI / 180.0;
        }

        //
        // Summary:
        //     Converts radians to degrees
        //
        // Parameters:
        //   angle:
        //     An angle measured in radians
        //
        // Returns:
        //     The same angle measured in degrees
        public static double RadiansToDegrees(double angle)
        {
            return angle * 180.0 / System.Math.PI;
        }


        /// <summary>
        ///     Calculates the sine of an angle given in degrees
        /// </summary>
        /// <param name="angle">The angle, in degrees</param>
        /// <returns>The sine of the given angle</returns>
        /// <remarks>
        ///     <para>
        ///         To calculate the sine of an angle given in radians, use
        ///         <see cref="M:System.Math.Sin(System.Double)">System.Math.Sin</see>
        ///     </para>
        /// </remarks>
        public static double SinD(double angle)
        {
            return Sin(angle * PI / 180.0);
        }


        //
        // Summary:
        //     Calculates the cosine of an angle given in degrees
        //
        // Parameters:
        //   angle:
        //     The angle, in degrees
        //
        // Returns:
        //     The cosine of the given angle
        //
        // Remarks:
        //     To calculate the cosine of an angle given in radians, use System.Math.Cos
        public static double CosD(double angle)
        {
            return System.Math.Cos(angle * System.Math.PI / 180.0);
        }

        //
        // Summary:
        //     Calculates the tangent of an angle given in degrees
        //
        // Parameters:
        //   angle:
        //     The angle, in degrees
        //
        // Returns:
        //     The tangent of the given angle
        //
        // Remarks:
        //     To calculate the tangent of an angle given in radians, use System.Math.Tan
        public static double TanD(double angle)
        {
            return System.Math.Tan(angle * System.Math.PI / 180.0);
        }

        //
        // Summary:
        //     Calculates the arcsine (in degrees) of a given number
        //
        // Parameters:
        //   x:
        //     The given number, which must be in the range -1 ≤ x ≤ 1
        //
        // Returns:
        //     An angle, theta, such that sin(theta) = x, and 0 ≤ theta ≤ 180
        //
        // Remarks:
        //     To calculate the arcsine in radians of a given number, use System.Math.Asin
        //     If x < -1 or x > 1, then the returned value is NaN (Not A Number)
        public static double AsinD(double x)
        {
            return System.Math.Asin(x) * 180.0 / System.Math.PI;
        }

        //
        // Summary:
        //     Calculates the arccosine (in degrees) of a given number
        //
        // Parameters:
        //   x:
        //     The given number, which must be in the range -1 ≤ x ≤ 1
        //
        // Returns:
        //     An angle, theta, such that cos(theta) = x, and 0 ≤ theta ≤ 180
        //
        // Remarks:
        //     To calculate the arccosine in radians of a given number, use System.Math.Acos
        //     If x < -1 or x > 1, then the returned value is NaN (Not A Number)
        public static double AcosD(double x)
        {
            return System.Math.Acos(x) * 180.0 / System.Math.PI;
        }

        //
        // Summary:
        //     Calculates the arctangent (in degrees) of a given number
        //
        // Parameters:
        //   x:
        //     The given number, which must be in the range -1 ≤ x ≤ 1
        //
        // Returns:
        //     An angle, theta, such that tan(theta) = x, and 0 ≤ theta ≤ 180
        //
        // Remarks:
        //     To calculate the arctangent in radians of a given number, use System.Math.Atan
        public static double AtanD(double x)
        {
            return System.Math.Atan(x) * 180.0 / System.Math.PI;
        }

        //
        // Summary:
        //     Calculates the arctangent (in degrees) of a ratio of two given numbers
        //
        // Parameters:
        //   y:
        //     The first given number (which can be regarded as a y-coordinate in the plane)
        //
        //   x:
        //     The second given number (which can be regarded as an x-coordinate in the plane)
        //
        // Returns:
        //     An angle, theta, such that tan(theta) = y/x, sign(theta) = sign(y), and -180
        //     ≤ theta ≤ 180
        //
        // Remarks:
        //     To obtain the same result in radians, use System.Math.Atan2.
        //     For limiting cases where either x or y is zero, please see the standard documentation
        //     for System.Math.Atan2.
        public static double Atan2D(double y, double x)
        {
            return System.Math.Atan2(y, x) * 180.0 / System.Math.PI;
        }

        /// <summary>Find the index of the maximum element in an array</summary>
        /// <param name="values">The array of values</param>
        /// <remarks>
        ///     Note that "largest" means closest to plus-infinity, not furthest from zero.<br />
        ///     So, MaxIndex(-5, 1, 3) is 2, not 0.
        /// </remarks>
        /// <returns>The index of the largest value in the array</returns>
        public static int MaxIndex(params double[] values)
        {
            int num = 0;

            for (int i = 1; i < values.Length; i++)
                if (values[i] > values[num])
                    num = i;

            return num;
        }

        //
        // Summary:
        //     Find the maximum element in an array
        //
        // Parameters:
        //   values:
        //     The array of values
        //
        // Returns:
        //     The largest value in the array
        //
        // Remarks:
        //     Note that "largest" means closest to plus-infinity, not furthest from zero.
        //     So, Max(-5, 1, 3) is 3, not 5.
        public static double Max(params double[] values)
        {
            return values[MaxIndex(values)];
        }

        //
        // Summary:
        //     Find the index of the minimum element in an array
        //
        // Parameters:
        //   values:
        //     The array of values
        //
        // Returns:
        //     The index of the smallest value in the array
        //
        // Remarks:
        //     Note that "smallest" means closest to minus-infinity, not closest to zero.
        //     So, MinIndex(-5, 1, 3) is 0, not 1.
        public static int MinIndex(params double[] values)
        {
            int num = 0;

            for (int i = 1; i < values.Length; i++)
                if (values[i] > values[num])
                    num = i;

            return num;
        }

        //
        // Summary:
        //     Find the minimum element in an array
        //
        // Parameters:
        //   values:
        //     The array of values
        //
        // Returns:
        //     The smallest value in the array
        //
        // Remarks:
        //     Note that "smallest" means closest to minus-infinity, not closest to zero.
        //     So, Min(-5, 1, 3) is -5, not 1.
        public static double Min(params double[] values)
        {
            return values[MaxIndex(values)];
        }

        //
        // Summary:
        //     Find the mean (average) of an array of values
        //
        // Parameters:
        //   values:
        //     The array of values
        //
        // Returns:
        //     The mean of the values in the array
        public static double Mean(params double[] values)
        {
            return Sum(values) / values.Length;
        }

        //
        // Summary:
        //     Find the sum of an array of values
        //
        // Parameters:
        //   values:
        //     The array of values
        //
        // Returns:
        //     The sum of the values in the array
        public static double Sum(params double[] values)
        {
            double num = 0.0;

            foreach (double num2 in values)
                num += num2;

            return num;
        }

      
        //
        // Summary:
        //     Evaluates value of a b-spline basis function
        //
        // Parameters:
        //   knots:
        //     Knot sequence : n+k values t[0], ... , t[n+k-1]
        //
        //   i:
        //     Index of basis function (see below)
        //
        //   k:
        //     Order of the b-spline
        //
        //   t:
        //     Parameter value at which to evaluate
        //
        // Returns:
        //     Value of i-th basis function B(i,k)(t)
        //
        // Remarks:
        //     There are n basis functions, numbered B(0,k), B(1,k), ... , B(n-1,k).
        //
        //     The i-th basis function B(i,k) is non-zero only for t[i] < t < t[i+k]
        public static double EvaluateBasisFunction(double[] knots, int i, int k, double t)
        {
            return EvaluateBasisFunction(knots, i, k, t, 0)[0];
        }

        //
        // Summary:
        //     Evaluates a b-spline basis function and/or optional derivatives
        //
        // Parameters:
        //   knots:
        //     The knot sequence : n+k values t[0], ... , t[n+k-1]
        //
        //   i:
        //     Index of basis function (see below)
        //
        //   k:
        //     Order of the b-spline
        //
        //   tau:
        //     Parameter value at which to evaluate
        //
        //   derivs:
        //     Number of derivatives requested (0 for position alone)
        //
        // Returns:
        //     Position and derivatives
        //
        // Remarks:
        //     Copied from h0916, which in turn came from deBoor's book.
        //
        //     There are n basis functions, numbered B(0,k), B(1,k), ... , B(n-1,k).
        //
        //     The i-th basis function B(i,k) is non-zero only for t[i] < tau < t[i+k]
        private static double[] EvaluateBasisFunction(double[] knots, int i, int k, double tau, int derivs)
        {
            int num = knots.Length;
            double[] array = new double[derivs + 1];
            double num2 = 1E-07;
            double num3 = 1E-11;
            double[,] array2 = new double[25, 25];

            if (derivs >= k)
            {
                for (int j = k; j <= derivs; j++)
                    array[k] = 0.0;

                derivs = k - 1;
            }

            double num4 = !(System.Math.Abs(tau - knots[num - 1]) > num3) ? knots[num - 1] - num3 : tau;

            for (int l = i; l < i + k; l++)
                array2[0, l - i] = knots[l] <= num4 && knots[l + 1] > num4 ? 1.0 : 0.0;

            for (int j = 1; j <= derivs; j++)
            for (int l = 0; l < k; l++)
                array2[j, l] = array2[0, l];

            for (int l = 2; l <= k; l++)
            for (int j = 0; j <= derivs; j++)
            {
                double num5 = knots[i + l - 1] - knots[i];
                double num6 = !(System.Math.Abs(num5) <= num2) ? array2[j, 0] / num5 : 0.0;
                for (int m = 0; m <= k - l; m++)
                {
                    num5 = knots[i + m + l] - knots[i + m + 1];
                    double num7 = !(System.Math.Abs(num5) <= num2) ? array2[j, m + 1] / num5 : 0.0;

                    array2[j, m] = l < k - j + 1
                        ? num6 * (tau - knots[i + m]) + num7 * (knots[i + m + l] - tau)
                        : (2 * k - l - j) * (num6 - num7);

                    num6 = num7;
                }
            }

            for (int j = 0; j <= derivs; j++)
                array[j] = array2[j, 0];

            return array;
        }

        internal static double Sin(double a)
        {
            return System.Math.Sin(a);
        }
    }
}