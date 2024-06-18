using System;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Extensions;

namespace TSG_Library.Disposable
{
    public class Rollback : IDisposable
    {
        private readonly Feature feat;
        private readonly string undo;
        private readonly bool updaetFeature;
        private readonly EditWithRollbackManager rollback;
        public Rollback(Feature feat, string undo, bool updateFeature = false)
        {
            feat.__OwningPart().__AssertIsWorkPart();
            this.feat = feat;
            this.updaetFeature = updateFeature;
            var undo_ = NXOpen.Session.GetSession().SetUndoMark(Session.MarkVisibility.Visible, undo);
            rollback = feat.__OwningPart().Features.StartEditWithRollbackManager(feat, undo_);
        }

        public void Dispose()
        {
            try
            {
                rollback.UpdateFeature(updaetFeature);
                rollback.Stop();
            }
            finally
            {
                rollback.Destroy();
            }
        }
    }
}