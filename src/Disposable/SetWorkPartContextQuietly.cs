using System;
using NXOpen;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.Disposable
{
    public class SetWorkPartContextQuietly : IDisposable
    {
        private IntPtr previous_work_part_context;

        public SetWorkPartContextQuietly(BasePart basePart)
        {
            ufsession_.Assem.SetWorkPartContextQuietly(basePart.Tag, out previous_work_part_context);
        }

        public void Dispose()
        {
            ufsession_.Assem.RestoreWorkPartContextQuietly(ref previous_work_part_context);
        }
    }
}