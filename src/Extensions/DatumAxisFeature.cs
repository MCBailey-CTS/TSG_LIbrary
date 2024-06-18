using System;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region DatumAxisFeature

        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature __Mirror(
            this DatumAxisFeature original,
            Surface.Plane plane)
        {
            _ = original.DatumAxis.__Origin().__Mirror(plane);
            _ = original.DatumAxis.Direction.__Mirror(plane);
            _ = original.__OwningPart().Features.CreateDatumAxisBuilder(null);

            //using(session_.using_builder_destroyer(builder))
            //{
            //    builder.
            //}

            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature __Mirror(
            this DatumAxisFeature datumAxisFeature,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}