using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Extrude

        [Obsolete(nameof(NotImplementedException))]
        public static Extrude __Mirror(
            this Extrude extrude,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Extrude __Mirror(
            this Extrude extrude,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}