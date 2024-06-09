using System;
using NXOpen.UF;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Disposable
{
    public class SuppressDisplayReset : IDisposable
    {
        public SuppressDisplayReset()
        {
            ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_SUPPRESS_DISPLAY);
        }


        public void Dispose()
        {
            ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }
    }
}