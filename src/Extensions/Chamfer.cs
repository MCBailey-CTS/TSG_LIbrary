using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer _Mirror(
            this Chamfer chamfer,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer _Mirror(
            this Chamfer chamfer,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }
    }
}