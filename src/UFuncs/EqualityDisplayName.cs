using NXOpen.Assemblies;
using System;
using System.Collections.Generic;

namespace TSG_Library.UFuncs
{
    [Obsolete]
    internal class EqualityDisplayName : IEqualityComparer<Component>
    {
        public bool Equals(Component x, Component y)
        {
            throw new System.NotImplementedException();
        }

        public int GetHashCode(Component obj)
        {
            throw new System.NotImplementedException();
        }
    }
}