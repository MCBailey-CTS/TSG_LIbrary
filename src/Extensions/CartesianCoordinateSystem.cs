using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region CartesianCoordinateSystem

        public static DatumPlane __DatumPlaneXZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out Tag[] dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane __DatumPlaneYZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out Tag[] dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[1]);
        }

        #endregion
    }
}