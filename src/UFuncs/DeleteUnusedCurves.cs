using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions_;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_delete_unused_curves)]
    [RevisionLog("Delete Unused Curves")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public class DeleteUnusedCurves : _UFunc
    {
        public override void execute()
        {
            using (session_.using_do_update())
            {
                var selCurves = Selection.SelectCurves();
                var delCurves = new List<Tag>();

                foreach (var delete in selCurves)
                {
                    TheUFSession.Modl.AskObjectFeat(delete.Tag, out var featTag);

                    if (featTag == Tag.Null)
                        delCurves.Add(delete.Tag);
                }

                foreach (var curveObj in delCurves.Select(curve => (NXObject)NXObjectManager.Get(curve)))
                    session_.UpdateManager.AddToDeleteList(curveObj);
            }

            __display_part_.Views.Refresh();
        }
    }
}