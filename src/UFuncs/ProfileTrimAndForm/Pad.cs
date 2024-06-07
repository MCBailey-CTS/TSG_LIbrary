using NXOpen.Features;

namespace TSG_Library.UFuncs
{
    public struct Pad
    {
        public ExtractFace PadObj;

        public UniteTarget UniteTarget;

        public Pad(ExtractFace padObj, UniteTarget uniteTarget)
            : this()
        {
            PadObj = padObj;
            UniteTarget = uniteTarget;
        }
    }
}