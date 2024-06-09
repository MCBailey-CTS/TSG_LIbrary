using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region DatumAxis

        public static Vector3d __Direction(this DatumAxis datumAxis)
        {
            return datumAxis.Direction;
        }

        public static Point3d __Origin(this DatumAxis datumAxis)
        {
            return datumAxis.Origin;
        }

        #endregion
    }
}