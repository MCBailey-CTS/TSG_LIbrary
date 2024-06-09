using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region EdgeBlend

        [Obsolete(nameof(NotImplementedException))]
        public static EdgeBlend __Mirror(
            this EdgeBlend edgeBlend,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static EdgeBlend __Mirror(
            this EdgeBlend edgeBlend,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}