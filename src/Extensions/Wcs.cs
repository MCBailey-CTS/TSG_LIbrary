using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region WCS

        public static Matrix3x3 __Orientation(this WCS wcs)
        {
            wcs.CoordinateSystem.GetDirections(out Vector3d xDir, out Vector3d yDir);
            return xDir.__ToMatrix3x3(yDir);
        }


        public static void __Orientation(this WCS wcs, Matrix3x3 matrix)
        {
            wcs.SetOriginAndMatrix(wcs.Origin, matrix);
        }

        public static Vector3d __AxisX(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element.__AxisX();
        }

        public static Vector3d __AxisY(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element.__AxisY();
        }

        public static Vector3d __AxisZ(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element.__AxisZ();
        }

        #endregion
    }
}