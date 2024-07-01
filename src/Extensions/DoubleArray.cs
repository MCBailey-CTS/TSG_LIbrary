using NXOpen;
// ReSharper disable MemberCanBePrivate.Global

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region DoubleArray

        private static Matrix3x3 __ToMatrix3x3(this double[] array)
        {
            return new Matrix3x3
            {
                Xx = array[0],
                Xy = array[1],
                Xz = array[2],
                Yx = array[3],
                Yy = array[4],
                Yz = array[5],
                Zx = array[6],
                Zy = array[7],
                Zz = array[8]
            };
        }

        public static Point3d __ToPoint3d(this double[] array)
        {
            return new Point3d(array[0], array[1], array[2]);
        }

        public static Vector3d __ToVector3d(this double[] array)
        {
            return new Vector3d(array[0], array[1], array[2]);
        }

        public static Vector3d __Multiply(this double d, Vector3d vector)
        {
            return vector.__Multiply(d);
        }


        public static double RoundToEigth(double value, double tolerance = .001)
        {
            double roundValue = System.Math.Round(value, 3);
            double truncateValue = System.Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;

            if (System.Math.Abs(fractionValue) < tolerance)
                return roundValue;

            for (double ii = .125; ii <= 1; ii += .125)
                if (fractionValue <= ii)
                {
                    return truncateValue + ii;
                }

            return roundValue;
        }


        public static void __RoundTo_125(this double[] distances)
        {
            for (int i = 0; i < 3; i++)
            {
                double roundValue = System.Math.Round(distances[i], 3);
                double truncateValue = System.Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;

                if (fractionValue != 0)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (fractionValue <= ii)
                        {
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    }
                else
                    distances[i] = roundValue;
            }
        }

        // public static double __RoundToNearest(this double num, double nearest = .125, int digits = 3, double tolerance = .001)
        // {
        //     double roundValue = System.Math.Round(num, digits);
        //     double truncateValue = System.Math.Truncate(roundValue);
        //     double fractionValue = roundValue - truncateValue;

        //     if (Math.Abs(fractionValue) < tolerance)
        //         return roundValue;

        //     for (double ii = nearest; ii <= 1; ii += nearest)
        //         if (fractionValue <= ii)
        //             return truncateValue + ii;

        //     throw new 
        // }

        #endregion
    }
}