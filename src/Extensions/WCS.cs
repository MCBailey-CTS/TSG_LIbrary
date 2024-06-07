using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static Matrix3x3 _Orientation(this WCS wcs)
        {
            wcs.CoordinateSystem.GetDirections(out var xDir, out var yDir);
            return xDir._ToMatrix3x3(yDir);
        }


        public static void _Orientation(this WCS wcs, Matrix3x3 matrix)
        {
            wcs.SetOriginAndMatrix(wcs.Origin, matrix);
        }

        public static Vector3d _AxisX(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisX();
        }

        public static Vector3d _AxisY(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisY();
        }

        public static Vector3d _AxisZ(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisZ();
        }
    }
}