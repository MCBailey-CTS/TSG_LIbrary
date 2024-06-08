using System.Collections.Generic;
using NXOpen;

namespace TSG_Library.Utilities
{
    public class ComparerPoint3d : EqualityComparer<Point3d>
    {
        private readonly double __tolerance;

        public ComparerPoint3d(double tolerance = .001)
        {
            __tolerance = tolerance;
        }

        public override bool Equals(Point3d x, Point3d y)
        {
            return Math.Abs(x.X - y.X) < __tolerance
                   && Math.Abs(x.Y - y.Y) < __tolerance
                   && Math.Abs(x.Z - y.Z) < __tolerance;
        }

        public override int GetHashCode(Point3d obj)
        {
            var hash = 17;

            hash = hash * 23 + obj.X.GetHashCode();
            hash = hash * 23 + obj.Y.GetHashCode();
            hash = hash * 23 + obj.Z.GetHashCode();

            return hash;
        }
    }
}
// 266