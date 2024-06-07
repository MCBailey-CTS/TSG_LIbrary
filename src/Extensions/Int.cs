using System;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_

    {
        public static string _PadInt(this int integer, int padLength)
        {
            if(integer < 0)
                throw new ArgumentOutOfRangeException(nameof(integer), @"You cannot pad a negative integer.");

            if(padLength < 1)
                throw new ArgumentOutOfRangeException(nameof(padLength),
                    @"You cannot have a pad length of less than 1.");

            var integerString = $"{integer}";

            var counter = 0;

            while (integerString.Length < padLength)
            {
                integerString = $"0{integerString}";

                if(counter++ > 100)
                    throw new TimeoutException(nameof(_PadInt));
            }

            return integerString;
        }
    }
}