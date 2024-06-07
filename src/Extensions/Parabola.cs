using System;
using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete]
        public static Parabola _Copy(this Parabola parabola)
        {
            //if (parabola.IsOccurrence)
            //    throw new ArgumentException($@"Cannot copy {nameof(parabola)} that is an occurrence.", nameof(parabola));

            //NXOpen.Point3d centerPosition = parabola.CenterPoint;

            //double focalLength = parabola.FocalLength;

            //double minimumDy = parabola.MinimumDY;

            //double maximumDy = parabola.MaximumDY;

            //double rotationAngle = parabola.RotationAngle;

            //NXOpen.NXMatrix matrix = parabola.Matrix._Copy();

            //return parabola._OwningPart().Curves.CreateParabola(centerPosition, focalLength, minimumDy, maximumDy, rotationAngle, matrix);

            throw new NotImplementedException();
        }
    }
}