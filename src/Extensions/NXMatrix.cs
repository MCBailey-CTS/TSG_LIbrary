using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region NXMatrix

        public static Matrix3x3 __Element(this NXMatrix obj)
        {
            return obj.Element;
        }

        #endregion
    }
}