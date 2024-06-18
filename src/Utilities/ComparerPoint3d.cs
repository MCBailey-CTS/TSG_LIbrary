using System.Collections.Generic;
using NXOpen;

namespace TSG_Library.Utilities
{
    // ReSharper disable once UnusedType.Global
    public class ComparerPoint3d : EqualityComparer<Point3d>
    {
        private readonly double _tolerance;

        public ComparerPoint3d(double tolerance = .001)
        {
            _tolerance = tolerance;
        }

        public override bool Equals(Point3d x, Point3d y)
        {
            return Math.Abs(x.X - y.X) < _tolerance
                   && Math.Abs(x.Y - y.Y) < _tolerance
                   && Math.Abs(x.Z - y.Z) < _tolerance;
        }

        public override int GetHashCode(Point3d obj)
        {
            int hash = 17;

            hash = hash * 23 + obj.X.GetHashCode();
            hash = hash * 23 + obj.Y.GetHashCode();
            hash = hash * 23 + obj.Z.GetHashCode();

            return hash;
        }
    }
}
// 266