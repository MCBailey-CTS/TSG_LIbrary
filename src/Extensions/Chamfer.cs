using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Chamfer

        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer __Mirror(
            this Chamfer chamfer,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer __Mirror(
            this Chamfer chamfer,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}