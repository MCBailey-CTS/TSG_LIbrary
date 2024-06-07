using System;
using NXOpen.UF;

namespace TSG_Library.Disposable
{
    public class RegenerateDisplay : IDisposable
    {
        public void Dispose()
        {
            UFSession.GetUFSession().Disp.RegenerateDisplay();
        }
    }
}