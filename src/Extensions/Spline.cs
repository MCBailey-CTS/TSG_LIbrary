using System;
using NXOpen;

namespace TSG_Library.Extensions
{
    public partial class Extensions
    {
        #region Spline

        public static Spline __Copy(this Spline spline)
        {
            //if (spline.OwningPart.Tag != _WorkPart.Tag)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)}.", nameof(spline));

            //if (spline.IsOccurrence)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)} that is an occurrence.", nameof(spline));

            //return new Spline_(spline.Tag).Copy();
            throw new NotImplementedException();
        }

        #endregion
    }
}