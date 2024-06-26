﻿using NXOpen;
using NXOpen.Features;
using NXOpen.UF;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region ExtractFace

        public static int __Layer(this ExtractFace ext)
        {
            return ext.GetBodies()[0].Layer;
        }

        public static void __Layer(this ExtractFace ext, int layer)
        {
            foreach (Body body in ext.GetBodies())
                body.__Layer(layer);
        }

        public static bool __IsLinkedBody(this ExtractFace extractFace)
        {
            return extractFace.FeatureType == "LINKED_BODY";
        }

        /// <summary>
        ///     Gets a value indicating whether or not this {extractFace} is broken.
        /// </summary>
        /// <remarks>Returns true if {extractFace} is broken, false otherwise.</remarks>
        /// <param name="nxExtractFace">The extractFace.</param>
        /// <returns>True or false.</returns>
        public static bool __IsBroken(this ExtractFace nxExtractFace)
        {
            UFSession.GetUFSession().Wave.IsLinkBroken(nxExtractFace.Tag, out bool isBroken);
            return isBroken;
        }

        public static Tag __XFormTag(this ExtractFace extractFace)
        {
            ufsession_.Wave.AskLinkXform(extractFace.Tag, out Tag xform);
            return xform;
        }

        #endregion
    }
}