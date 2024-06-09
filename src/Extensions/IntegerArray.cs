using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region IntegerArray

        public static Point3d __ToPoint3d(this int[] array)
        {
            return new Point3d(array[0], array[1], array[2]);
        }

        public static Vector3d __ToVector3d(this int[] array)
        {
            return new Vector3d(array[0], array[1], array[2]);
        }

        //public static Vector3d _ToVector3d(this int[] array)
        //{
        //    return new Vector3d(array[0], array[1], array[2]);
        //}

        #endregion
    }
}