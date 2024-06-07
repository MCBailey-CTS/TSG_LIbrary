using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions_;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_circle_from_spline)]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public class CircleFromSpline : _UFunc
    {
        public override void execute()
        {
            if (Session.GetSession().Parts.Display is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            var selObject = Selection.SelectSingleSpline();

            if (selObject is null)
                return;

            do
            {
                Session.GetSession().SetUndoMark(Session.MarkVisibility.Visible, "CreateOrNull Arc");
                var spline1 = selObject;

                var limits = new double[2];
                var pointOnCurve1 = new double[3];
                var pointOnCurve2 = new double[3];
                var pointOnCurve3 = new double[3];
                var derivatives = new double[3];

                UFSession.GetUFSession().Eval.Initialize(spline1.Tag, out var evaluator);
                UFSession.GetUFSession().Eval.AskLimits(evaluator, limits);

                UFSession.GetUFSession().Eval.Evaluate(evaluator, 0, limits[0], pointOnCurve1, derivatives);
                UFSession.GetUFSession().Eval
                    .Evaluate(evaluator, 0, (limits[1] - limits[0]) * .33, pointOnCurve2, derivatives);
                UFSession.GetUFSession().Eval
                    .Evaluate(evaluator, 0, (limits[1] - limits[0]) * .66, pointOnCurve3, derivatives);

                var startPoint1 = new Point3d(pointOnCurve1[0], pointOnCurve1[1], pointOnCurve1[2]);
                var pointOn1 = new Point3d(pointOnCurve2[0], pointOnCurve2[1], pointOnCurve2[2]);
                var endPoint1 = new Point3d(pointOnCurve3[0], pointOnCurve3[1], pointOnCurve3[2]);

                var arc1 = Session.GetSession().Parts.Work.Curves
                    .CreateArc(startPoint1, pointOn1, endPoint1, false, out _);

                UFSession.GetUFSession().Curve.AskArcData(arc1.Tag, out var arcData);
                arcData.start_angle = 0;
                arcData.end_angle = 2 * Math.PI;

                UFSession.GetUFSession().Curve.EditArcData(arc1.Tag, ref arcData);

                var selectedObjects1 = new NXObject[2];
                selectedObjects1[0] = spline1;
                selectedObjects1[1] = arc1;
                Session.GetSession().Information.DisplayObjectsDetails(selectedObjects1);

                selObject = Selection.SelectSingleSpline();
            } while (!(selObject is null));
        }
    }
}