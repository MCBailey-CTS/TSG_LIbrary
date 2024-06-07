using NXOpen.Positioning;

namespace TSG_Library.Extensions
{
    public static class __Constraint__
    {
        public static void Temp(Constraint con)
        {
            //con.Automatic
            //con.ConstraintAlignment
            //con.ConstraintSecondAlignment
            //con.ConstraintType
            //con.CreateConstraintReference
            //con.CreateCouplerReference
            //con.DeleteConstraintReference
            //con.EditConstraintReference
            //con.EditCouplerReference
            //con.Expression
            //con.ExpressionDriven
            //con.FlipAlignment
            //con.GetConstraintStatus
            //con.GetDisplayedConstraint
            //con.GetReferences
            //con.LowerLimitEnabled
            //con.LowerLimitExpression
            //con.LowerLimitRightHandSide
            //con.OffsetExpression
            //con.OffsetRightHandSide
            //con.Persistent
            //con.ReverseDirection
            //con.SecondExpression
            //con.SecondExpressionDriven
            //con.SecondExpressionRightHandSide
        }

        public static void __SetExpression(this Constraint con, string expression)
        {
            con.SetExpression(expression);
        }

        public static bool __Suppressed(this Constraint con)
        {
            return con.Suppressed;
        }
    }
}