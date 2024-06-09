using NXOpen;
using NXOpen.Positioning;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region ConstraintReference

        public static bool __GeometryDirectionReversed(ConstraintReference cons)
        {
            return cons.GeometryDirectionReversed;
        }

        public static NXObject __GetGeometry(ConstraintReference cons)
        {
            return cons.GetGeometry();
        }

        public static bool __GetHasPerpendicularVector(ConstraintReference cons)
        {
            return cons.GetHasPerpendicularVector();
        }

        public static NXObject __GetMovableObject(ConstraintReference cons)
        {
            return cons.GetMovableObject();
        }

        public static NXObject __GetPrototypeGeometry(ConstraintReference cons)
        {
            return cons.GetPrototypeGeometry();
        }

        public static Vector3d __GetPrototypePerpendicularVector(ConstraintReference cons)
        {
            return cons.GetPrototypePerpendicularVector();
        }

        public static bool __GetUsesGeometryAxis(ConstraintReference cons)
        {
            return cons.GetUsesGeometryAxis();
        }

        public static Point3d __HelpPoint(ConstraintReference cons)
        {
            return cons.HelpPoint;
        }

        public static ConstraintReference.ConstraintOrder __Order(
            ConstraintReference cons)
        {
            return cons.Order;
        }

        public static ConstraintReference.GeometryType __SolverGeometryType(
            ConstraintReference cons)
        {
            return cons.SolverGeometryType;
        }

        #endregion
    }
}