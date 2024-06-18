using System;
using NXOpen;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedVariable
// ReSharper disable InconsistentNaming

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region CoordinateSystem

        public static Vector3d __AxisX(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element.__AxisX();
        }

        public static Vector3d __AxisY(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element.__AxisY();
        }

        public static Vector3d __AxisZ(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element.__AxisZ();
        }

        public static void __GetDirections(this CoordinateSystem obj)
        {
            obj.GetDirections(out Vector3d xDirection, out Vector3d yDirection);
        }

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

        #endregion
    }
}