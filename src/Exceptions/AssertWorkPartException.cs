using System;
using NXOpen;
using static TSG_Library.Extensions;

namespace TSG_Library.Exceptions
{
    public class AssertWorkPartException : Exception
    {
        public AssertWorkPartException(BasePart basePart)
            : base($"Part {basePart.Leaf} is not the current work part {__work_part_.Leaf}")
        {
        }
    }
}