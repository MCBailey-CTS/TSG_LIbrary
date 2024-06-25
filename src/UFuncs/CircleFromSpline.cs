using System;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_circle_from_spline)]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class CircleFromSpline : _UFunc
    {
        public override void execute()
        {
            if (__display_part_ is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            Spline selObject = Selection.SelectSingleSpline(ufunc_rev_name);

            if (selObject is null)
                return;

            do
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "CircleFromSpline");
                Spline spline1 = selObject;

                double[] limits = new double[2];
                double[] pointOnCurve1 = new double[3];
                double[] pointOnCurve2 = new double[3];
                double[] pointOnCurve3 = new double[3];
                double[] derivatives = new double[3];

                ufsession_.Eval.Initialize(spline1.Tag, out IntPtr evaluator);
                ufsession_.Eval.AskLimits(evaluator, limits);

                ufsession_.Eval.Evaluate(evaluator, 0, limits[0], pointOnCurve1, derivatives);
                ufsession_.Eval.Evaluate(evaluator, 0, (limits[1] - limits[0]) * .33, pointOnCurve2, derivatives);
                ufsession_.Eval.Evaluate(evaluator, 0, (limits[1] - limits[0]) * .66, pointOnCurve3, derivatives);

                Point3d startPoint1 = pointOnCurve1.__ToPoint3d();
                Point3d pointOn1 = pointOnCurve2.__ToPoint3d();
                Point3d endPoint1 = pointOnCurve3.__ToPoint3d();

                Arc arc1 = __work_part_.Curves.CreateArc(startPoint1, pointOn1, endPoint1, false, out _);

                ufsession_.Curve.AskArcData(arc1.Tag, out UFCurve.Arc arcData);
                arcData.start_angle = 0;
                arcData.end_angle = 2 * Math.PI;

                ufsession_.Curve.EditArcData(arc1.Tag, ref arcData);

                NXObject[] selectedObjects1 = new NXObject[2];
                selectedObjects1[0] = spline1;
                selectedObjects1[1] = arc1;
                session_.Information.DisplayObjectsDetails(selectedObjects1);

                selObject = Selection.SelectSingleSpline();
            }
            while (!(selObject is null));
        }
    }
}