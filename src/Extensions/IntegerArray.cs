using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static Point3d _ToPoint3d(this int[] array)
        {
            return new Point3d(array[0], array[1], array[2]);
        }

        public static Vector3d _ToVector3d(this int[] array)
        {
            return new Vector3d(array[0], array[1], array[2]);
        }
    }
}