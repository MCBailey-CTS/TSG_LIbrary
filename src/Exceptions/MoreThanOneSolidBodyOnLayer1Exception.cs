using System;
using System.Linq;
using NXOpen;
using static TSG_Library.Extensions;

namespace TSG_Library.Exceptions
{
    [Serializable]
    internal class MoreThanOneSolidBodyOnLayer1Exception : Exception
    {
        private readonly Part __part;

        public MoreThanOneSolidBodyOnLayer1Exception(Part __part)
        {
            this.__part = __part;
        }

        public override string Message
        {
            get
            {
                var solid_bodies = __work_part_
                    .Bodies
                    .ToArray()
                    .Where(__body => __body.IsSolidBody)
                    .Where(__body => __body.Layer == 1)
                    .ToArray();

                return $"Found more than one solid body on layer 1 in part {__part.Leaf}, bodies {solid_bodies.Length}";
            }
        }
    }
}