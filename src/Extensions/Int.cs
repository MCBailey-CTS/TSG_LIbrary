using System;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Int

        public static string __PadInt(this int integer, int padLength)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException(nameof(integer), @"You cannot pad a negative integer.");

            if (padLength < 1)
                throw new ArgumentOutOfRangeException(nameof(padLength),
                    @"You cannot have a pad length of less than 1.");

            string integerString = $"{integer}";

            int counter = 0;

            while (integerString.Length < padLength)
            {
                integerString = $"0{integerString}";

                if (counter++ > 100)
                    throw new TimeoutException(nameof(__PadInt));
            }

            return integerString;
        }

        #endregion
    }
}