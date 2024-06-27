using NXOpen.Assemblies;
using System;
using System.Collections.Generic;

namespace TSG_Library.UFuncs
{
    internal class EqualityDisplayName : IEqualityComparer<Component>
    {
        public bool Equals(Component x, Component y)
        {
            return x.DisplayName.Equals(y.DisplayName);
        }

        public int GetHashCode(Component obj)
        {
            return obj.DisplayName.GetHashCode();
        }
    }
}