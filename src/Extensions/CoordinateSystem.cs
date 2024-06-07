using System;
using NXOpen;
using TSG_Library.Attributes;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __CoordinateSystem__
    {
        //public static NXOpen.Vector3d _AxisX(this NXOpen.CoordinateSystem coordinateSystem)
        //{
        //    return coordinateSystem.Orientation.Element._AxisX();
        //}

        //public static NXOpen.Vector3d _AxisY(this NXOpen.CoordinateSystem coordinateSystem)
        //{
        //    return coordinateSystem.Orientation.Element._AxisY();
        //}

        //public static NXOpen.Vector3d _AxisZ(this NXOpen.CoordinateSystem coordinateSystem)
        //{
        //    return coordinateSystem.Orientation.Element._AxisZ();
        //}

        //public static void __GetDirections(this NXOpen.CoordinateSystem obj, )
        //{
        //    obj.GetDirections(out var xDirection, out var yDirection);
        //}

        [Obsolete]
        public static void __GetSolverCardSyntax(this CoordinateSystem obj)
        {
        }

        public static bool __IsTemporary(this CoordinateSystem obj)
        {
            return obj.IsTemporary;
        }

        [Obsolete]
        public static void __Label(this CoordinateSystem obj)
        {
        }

        public static NXMatrix __Orientation(this CoordinateSystem obj)
        {
            return obj.Orientation;
        }

        public static void __Orientation(this CoordinateSystem obj, Vector3d xDir, Vector3d yDir)
        {
            obj.SetDirections(xDir, yDir);
        }

        public static Point3d __Origin(this CoordinateSystem obj)
        {
            return obj.Origin;
        }
    }
}