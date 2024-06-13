using NXOpen;
using NXOpen.Positioning;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region ComponentConstraint

        public static void __Check0(ComponentConstraint componentConstraint)
        {
            //componentConstraint.Automatic
            //componentConstraint.ConstraintAlignment
            //componentConstraint.ConstraintSecondAlignment
            //componentConstraint.ConstraintType
            //componentConstraint.CreateConstraintReference
            //componentConstraint.DeleteConstraintReference
            //componentConstraint.EditConstraintReference
            //componentConstraint.Expression
            //componentConstraint.ExpressionDriven
            //componentConstraint.FlipAlignment
            //componentConstraint.GetConstraintStatus
            //componentConstraint.GetDisplayedConstraint
            //componentConstraint.GetInherited
            //componentConstraint.GetReferences
            //componentConstraint.Persistent
            //componentConstraint.Print
            //componentConstraint.Renew
            //componentConstraint.ReverseDirection
            //componentConstraint.SetAlignmentHint
            //componentConstraint.SetExpression
        }


        public static ConstraintReference __CreateConstRefOcc(
            this ComponentConstraint constraint,
            NXObject occObject)
        {
            return constraint.CreateConstraintReference(
                occObject.OwningComponent,
                occObject,
                false,
                false,
                false);
        }

        public static ConstraintReference __CreateConstRefProto(
            this ComponentConstraint constraint,
            NXObject protoObject)
        {
            return constraint.CreateConstraintReference(
                protoObject.OwningPart.ComponentAssembly,
                protoObject,
                false,
                false,
                false);
        }

        #endregion
    }
}