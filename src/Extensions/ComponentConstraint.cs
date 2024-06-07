using NXOpen;
using NXOpen.Positioning;
using TSG_Library.Attributes;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __ComponentConstraint__
    {
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
            this ComponentConstraint __constraint,
            NXObject __occ_object)
        {
            return __constraint.CreateConstraintReference(
                __occ_object.OwningComponent,
                __occ_object,
                false,
                false,
                false);
        }

        public static ConstraintReference __CreateConstRefProto(
            this ComponentConstraint __constraint,
            NXObject __proto_object)
        {
            return __constraint.CreateConstraintReference(
                __proto_object.OwningPart.ComponentAssembly,
                __proto_object,
                false,
                false,
                false);
        }
    }
}