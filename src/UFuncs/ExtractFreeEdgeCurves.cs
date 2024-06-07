using System.Linq;
using NXOpen;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions_;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [RevisionLog("Extract Free Edge Curves")]
    [RevisionEntry("1.0", "2018", "01", "17")]
    [Revision("1.0.1", "Revision log created for NX 11.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    [UFunc(ufunc_extract_free_edge_curves)]
    internal class ExtractFreeEdgeCurves : _UFunc
    {
        public override void execute()
        {
            if (Session.GetSession().Parts.Display is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            session_.SetUndoMark(Session.MarkVisibility.Visible, "ExtractFreeEdgeCurves");

            // Allows the user to select sheet bodies.
            var selectedSheetBodies = Selection.SelectManySheetBodies();

            // Gets the edges from the selected sheet bodies.
            var edges = selectedSheetBodies.SelectMany(body => body.GetEdges()).ToArray();

            // Gets the free edges, (the edges that are attached to one face).
            var freeEdges = edges.Where(edge => edge.GetFaces().Length == 1).ToArray();

            // Gets the curve representation of the {freeEdges}.
            var freeEdgeCurves = freeEdges.Select(edge => edge._ToCurve()).ToArray();

            // Sets all the free edge curves to the current work layer.
            foreach (var curve in freeEdgeCurves)
                curve.__Layer(WorkLayer);

            print_($"Created {freeEdgeCurves.Length} curves off of free edges.");
        }
    }
}