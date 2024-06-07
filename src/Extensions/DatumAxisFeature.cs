using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature _Mirror(
            this DatumAxisFeature original,
            Surface.Plane plane)
        {
            _ = original.DatumAxis.__Origin()._Mirror(plane);
            _ = original.DatumAxis.Direction._Mirror(plane);
            _ = original.__OwningPart().Features.CreateDatumAxisBuilder(null);

            //using(session_.using_builder_destroyer(builder))
            //{
            //    builder.
            //}

            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature _Mirror(
            this DatumAxisFeature datumAxisFeature,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }
    }
}