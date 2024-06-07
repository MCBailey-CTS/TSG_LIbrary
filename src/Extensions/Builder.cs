using NXOpen;
using TSG_Library.Disposable;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static Destroyer _using_builder(this Builder builder)
        {
            return new Destroyer(builder);
        }
    }
}