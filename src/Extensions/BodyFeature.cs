using System.Linq;
using NXOpen.Features;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static void _Layer(this BodyFeature bodyFeature, int layer)
        {
            bodyFeature.GetBodies().ToList().ForEach(__b => __b.__Layer(layer));
        }
    }
}