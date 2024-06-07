using System;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Disposable
{
    public class LockUiFromCustom : IDisposable
    {
        public LockUiFromCustom()
        {
            ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
        }

        public void Dispose()
        {
            ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
        }
    }
}