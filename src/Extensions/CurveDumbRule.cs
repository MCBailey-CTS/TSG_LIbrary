using System;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;
using Curve = NXOpen.Curve;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static CurveDumbRule _Mirror(
            this CurveDumbRule curveDumbRule,
            Surface.Plane plane)
        {
            var curves = curveDumbRule._Data();
            _ = curves[0].__OwningPart();

            //var new_curves = curves.Select(__c=>__c._Mirror(plane))


            throw new NotImplementedException();
        }


        public static Curve[] _Data(this CurveDumbRule rule)
        {
            rule.GetData(out var curves);
            return curves;
        }


        [Obsolete(nameof(NotImplementedException))]
        public static CurveDumbRule _Mirror(
            this CurveDumbRule curveDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }
    }
}