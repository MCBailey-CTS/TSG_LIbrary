using System;
using NXOpen;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Disposable
{
    public class DoUpdate : IDisposable
    {
        private readonly Session.UndoMarkId __undo;

        public DoUpdate() : this("Do Update")
        {
        }

        public DoUpdate(string undo_text)
        {
            __undo = session_.SetUndoMark(Session.MarkVisibility.Visible, undo_text);
        }

        public void Dispose()
        {
            session_.UpdateManager.DoUpdate(__undo);
            session_.DeleteUndoMark(__undo, "");
        }
    }
}