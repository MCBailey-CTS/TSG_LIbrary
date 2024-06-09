using System;
using System.Linq;
using NXOpen;
using NXOpen.Drawings;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Part

        public static DrawingSheet __GetDrawingSheetOrNull(
            this Part part,
            string drawingSheetName,
            StringComparison stringComparison)
        {
            foreach (DrawingSheet drawingSheet in part.DrawingSheets)
                if(drawingSheet.Name.Equals(drawingSheetName, stringComparison))
                    return drawingSheet;

            return null;
        }

        public static Body __SolidBodyLayer1OrNull(this Part part)
        {
            var bodiesOnLayer1 = part.Bodies
                .OfType<Body>()
                .Where(body => !body.IsOccurrence)
                .Where(body => body.IsSolidBody)
                .Where(body => body.Layer == 1)
                .ToArray();

            return bodiesOnLayer1.Length == 1 ? bodiesOnLayer1[0] : null;
        }

        public static Body __SingleSolidBodyOnLayer1(this Part part)
        {
            var __solid_bodies__on_layer_1 = part.Bodies
                .ToArray()
                .Where(__body => __body.IsSolidBody)
                .Where(__body => __body.Layer == 1)
                .ToArray();

            switch (__solid_bodies__on_layer_1.Length)
            {
                case 0:
                    throw new InvalidOperationException($"Could not find a solid body on layer 1 in part {part.Leaf}");
                case 1:
                    return __solid_bodies__on_layer_1[0];
                default:
                    throw new InvalidOperationException(
                        $"Found {__solid_bodies__on_layer_1.Length} solid bodies on layer 1 in part {part.Leaf}");
            }
        }

        #endregion
    }
}