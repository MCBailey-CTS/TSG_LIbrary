using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_delete_unused_curves)]
    //[RevisionLog("Delete Unused Curves")]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class DeleteUnusedCurves : _UFunc
    {
        public override void execute()
        {
            string message = ufunc_rev_name;

            foreach (Curve delete in Selection.SelectCurves(message, message))
            {
                if (delete.IsOccurrence)
                {
                    TheUFSession.Modl.AskObjectFeat(delete.__Prototype().Tag, out Tag featTag);

                    if (featTag == Tag.Null)
                        delete.__Prototype().__Delete();
                }
                else
                {
                    TheUFSession.Modl.AskObjectFeat(delete.Tag, out Tag featTag);

                    if (featTag == Tag.Null)
                        delete.__Delete();
                }
            }
        }
    }
}