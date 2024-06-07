using System;
using NXOpen;

namespace TSG_Library.Disposable
{
    public class LockNX : IDisposable
    {
        private readonly UI.Status _startingLockAccess;

        private readonly UI _theUiSession;

        public LockNX() : this(UI.GetUI())
        {
        }

        public LockNX(UI theUiSession)
        {
            _theUiSession = theUiSession;

            _startingLockAccess = _theUiSession.AskLockStatus();

            _theUiSession.LockAccess();
        }

        public void Dispose()
        {
            switch (_startingLockAccess)
            {
                case UI.Status.Lock:
                    _theUiSession.LockAccess();
                    break;
                case UI.Status.Unlock:
                    _theUiSession.UnlockAccess();
                    break;
            }
        }
    }
}