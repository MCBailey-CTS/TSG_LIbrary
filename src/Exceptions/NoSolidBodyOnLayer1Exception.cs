using System;
using NXOpen;

namespace TSG_Library.Exceptions
{
    [Serializable]
    internal class NoSolidBodyOnLayer1Exception : Exception
    {
        private readonly Part __part;

        public NoSolidBodyOnLayer1Exception(Part __part)
        {
            this.__part = __part;
        }

        public override string Message => $"Did not find a solid body on layer 1 in part {__part.Leaf}";
    }
}