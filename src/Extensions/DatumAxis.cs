using NXOpen;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace TSG_Library.Extensions
{
    public static partial class Extensions
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