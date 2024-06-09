using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Block

        [Obsolete(nameof(NotImplementedException))]
        public static Block __Mirror(
            this Block block,
            Surface.Plane plane)
        {
            var origin = block.__Origin().__Mirror(plane);
            var orientation = block.__Orientation().__Mirror(plane);
            var length = block.__Width();
            var width = block.__Length();
            var height = block.__Height();
            var builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.Length.Value = length;
                builder.Width.Value = width;
                builder.Height.Value = height;
                builder.Origin = origin;
                builder.SetOrientation(orientation.__AxisX(), orientation.__AxisY());
                return (Block)builder.Commit();
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Block __Mirror(
            this Block block,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static Point3d __Origin(this Block block)
        {
            var builder = block.OwningPart.Features.CreateBlockFeatureBuilder(block);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                return builder.Origin;
            }
        }

        public static Matrix3x3 __Orientation(this Block block)
        {
            var builder = block.OwningPart.Features.CreateBlockFeatureBuilder(block);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.GetOrientation(out var xAxis, out var yAxis);
                return xAxis.__ToMatrix3x3(yAxis);
            }
        }

        public static double __Length(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                return builder.Length.Value;
            }
        }

        public static double __Width(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                return builder.Width.Value;
            }
        }

        public static double __Height(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                return builder.Height.Value;
            }
        }

        #endregion
    }
}