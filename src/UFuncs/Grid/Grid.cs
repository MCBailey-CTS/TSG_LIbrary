using System;
using NXOpen;
using NXOpen.Preferences;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.UFuncs
{
    public static class Grid
    {
        public enum GridSize
        {
            Size0062,

            Size0125,

            Size0250,

            Size0500,

            Size1000,

            Size6000
        }

        public const double GridSize0062 = 0.0625d;

        public const double GridSize0125 = 0.125d;

        public const double GridSize0250 = 0.250d;

        public const double GridSize0500 = 0.500d;

        public const double GridSize1000 = 1.000d;

        public const double GridSize6000 = 6.000d;

        public const double GridSize0062Metric = GridSize0062 * 25.4;

        public const double GridSize0125Metric = GridSize0125 * 25.4;

        public const double GridSize0250Metric = GridSize0250 * 25.4;

        public const double GridSize0500Metric = GridSize0500 * 25.4;

        public const double GridSize1000Metric = GridSize1000 * 25.4;

        public const double GridSize6000Metric = GridSize6000 * 25.4;

        private static WorkPlane WorkPlane => __work_part_.Preferences.Workplane;

        public static void SetGrid(GridSize size)
        {
            var multiplier = __display_part_.PartUnits == BasePart.Units.Inches ? 1.0 : 25.4;
            var gridSpacing = Convert(size) * multiplier;
            WorkPlane.SetRectangularUniformGridSize(new WorkPlane.GridSize(gridSpacing, 1, 1));
            var comp = __display_part_.ComponentAssembly.RootComponent;
            comp.DirectOwner.ReplaceReferenceSet(comp, "BODY");
        }

        internal static double Convert(GridSize size)
        {
            switch (size)
            {
                case GridSize.Size0062: return GridSize0062;
                case GridSize.Size0125: return GridSize0125;
                case GridSize.Size0250: return GridSize0250;
                case GridSize.Size0500: return GridSize0500;
                case GridSize.Size1000: return GridSize1000;
                case GridSize.Size6000: return GridSize6000;
                default: throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }
        }
    }
}