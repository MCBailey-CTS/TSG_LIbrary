using NXOpen;
using NXOpen.Features;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static DatumPlane _DatumPlaneXY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_._GetTaggedObject(dplanes[0]);
        }

        public static DatumPlane _DatumPlaneXZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_._GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane _DatumPlaneYZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_._GetTaggedObject(dplanes[1]);
        }

        public static Vector3d _Vector3dX(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_._GetTaggedObject(daxes[0]);
            return axis.Direction;
        }

        public static Vector3d _Vector3dY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_._GetTaggedObject(daxes[1]);
            return axis.Direction;
        }

        public static Vector3d _Vector3dZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_._GetTaggedObject(daxes[2]);
            return axis.Direction;
        }
    }
}