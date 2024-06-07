using System;
using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static Spline _Copy(this Spline spline)
        {
            //if (spline.OwningPart.Tag != _WorkPart.Tag)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)}.", nameof(spline));

            //if (spline.IsOccurrence)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)} that is an occurrence.", nameof(spline));

            //return new Spline_(spline.Tag).Copy();
            throw new NotImplementedException();
        }
    }
}