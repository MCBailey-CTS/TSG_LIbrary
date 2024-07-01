using System.Collections.Generic;

namespace TSG_Library.UFuncs
{
    internal class EqualityLeaf : IEqualityComparer<NXOpen.Part>
    {
        public bool Equals(NXOpen.Part x, NXOpen.Part y)
        {
            return x.Leaf.Equals(y.Leaf);
        }

        public int GetHashCode(NXOpen.Part obj)
        {
            return obj.Leaf.GetHashCode();
        }
    }
}