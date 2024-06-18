using NXOpen;
using NXOpen.Features;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region DatumCsys

        public static DatumPlane __DatumPlaneXY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out Tag[] dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[0]);
        }

        public static DatumPlane __DatumPlaneXZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out Tag[] dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane __DatumPlaneYZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out Tag[] dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[1]);
        }

        public static Vector3d __Vector3dX(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out Tag[] daxes, out _);
            DatumAxis axis = (DatumAxis)session_.__GetTaggedObject(daxes[0]);
            return axis.Direction;
        }

        public static Vector3d __Vector3dY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out Tag[] daxes, out _);
            DatumAxis axis = (DatumAxis)session_.__GetTaggedObject(daxes[1]);
            return axis.Direction;
        }

        public static Vector3d __Vector3dZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out Tag[] daxes, out _);
            DatumAxis axis = (DatumAxis)session_.__GetTaggedObject(daxes[2]);
            return axis.Direction;
        }

        #endregion
    }
}