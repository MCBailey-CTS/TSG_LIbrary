using System;
using NXOpen.UF;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Disposable
{
    public class LockUpdates : IDisposable
    {
        public LockUpdates()
        {
            session_.UpdateManager.SetUpdateLock(true);
        }

        public void Dispose()
        {
            session_.UpdateManager.SetUpdateLock(false);

            UFSession.GetUFSession().Modl.Update();
        }
    }
}