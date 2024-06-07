using NXOpen;
using TSG_Library.Attributes;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __DatumAxis__
    {
        public static Vector3d __Direction(this DatumAxis datumAxis)
        {
            return datumAxis.Direction;
        }

        public static Point3d __Origin(this DatumAxis datumAxis)
        {
            return datumAxis.Origin;
        }
    }
}