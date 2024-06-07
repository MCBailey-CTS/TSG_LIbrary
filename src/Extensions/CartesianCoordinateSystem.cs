using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static DatumPlane _DatumPlaneXZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_._GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane _DatumPlaneYZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_._GetTaggedObject(dplanes[1]);
        }
    }
}