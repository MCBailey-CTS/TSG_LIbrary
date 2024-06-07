using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Positioning;
using TSG_Library.Attributes;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __DisplayedConstraint__
    {
        public static Constraint __GetConstraint(
            this DisplayedConstraint displayedConstraint)
        {
            return displayedConstraint.GetConstraint();
        }

        public static Part __GetConstraintPart(this DisplayedConstraint displayedConstraint)
        {
            return displayedConstraint.GetConstraintPart();
        }

        public static Component __GetContextComponent(
            this DisplayedConstraint displayedConstraint)
        {
            return displayedConstraint.GetContextComponent();
        }
    }
}