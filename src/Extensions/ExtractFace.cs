using NXOpen;
using NXOpen.Features;
using NXOpen.UF;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static bool _IsLinkedBody(this ExtractFace extractFace)
        {
            return extractFace.FeatureType == "LINKED_BODY";
        }

        /// <summary>
        ///     Gets a value indicating whether or not this {extractFace} is broken.
        /// </summary>
        /// <remarks>Returns true if {extractFace} is broken, false otherwise.</remarks>
        /// <param name="nxExtractFace">The extractFace.</param>
        /// <returns>True or false.</returns>
        public static bool _IsBroken(this ExtractFace nxExtractFace)
        {
            UFSession.GetUFSession().Wave.IsLinkBroken(nxExtractFace.Tag, out var isBroken);
            return isBroken;
        }

        public static Tag _XFormTag(this ExtractFace extractFace)
        {
            ufsession_.Wave.AskLinkXform(extractFace.Tag, out var xform);
            return xform;
        }
    }
}