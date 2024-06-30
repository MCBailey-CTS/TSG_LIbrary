using System.Collections.Generic;
using NXOpen;

namespace TSG_Library.Utilities
{
    public class EqualityPos : IEqualityComparer<Point3d>
    {
        private const double Tolerance = 0.001;


        public bool Equals(Point3d x, Point3d y)
        {
            return System.Math.Abs(x.X - y.X) < Tolerance
                   && System.Math.Abs(x.Y - y.Y) < Tolerance
                   && System.Math.Abs(x.Z - y.Z) < Tolerance;
        }

        public int GetHashCode(Point3d obj)
        {
            unchecked // integer overflows are accepted here
            {
                int hash = 17;
                hash = hash * 23 + obj.X.GetHashCode();
                hash = hash * 23 + obj.Y.GetHashCode();
                hash = hash * 23 + obj.Z.GetHashCode();
                return hash;
            }
        }
    }
}
// 266