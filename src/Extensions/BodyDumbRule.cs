using System;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule _Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        public static Body[] _Data(this BodyDumbRule rule)
        {
            rule.GetData(out var bodies);
            return bodies;
        }

        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule _Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        //public static void __GetData(this NXOpen.BodyDumbRule obj)
        //{
        //    obj.GetData
        //}
    }
}