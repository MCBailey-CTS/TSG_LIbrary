using System;
using System.Linq;
using NXOpen;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using CurveLengthBuilder = NXOpen.Features.CurveLengthBuilder;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc("blank-development")]
    //[RevisionLog("Blank Development")]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class BlankDevelopment : _UFunc
    {
        public override void execute()
        {
            string message = $"{AssemblyFileVersion} - Blank Development";

            session_.SetUndoMark(Session.MarkVisibility.Visible, "Develop Curve");
            Curve[] lengthObjs = Selection.SelectCurves(message, message);

            // get total length of selected lines
            double addLength = lengthObjs.Select(selTag => (Curve)session_.__GetTaggedObject(selTag.Tag))
                .Select(selCurve => selCurve.GetLength())
                .Sum();

            // get user input for the line to develop
            Point3d cursorLocation = new Point3d();
            Line developObj = Selection.SelectSingleLine();

            if (developObj is null)
                return;

            // get user selection for which end of the line to extend
            double[] cursor = { cursorLocation.X, cursorLocation.Y, cursorLocation.Z };
            AskPositionOnObject(developObj.Tag, cursor);
            AskCloserToStartOrEnd(developObj.Tag, cursor);
            // move closest point distance and direction
            Line devLine = (Line)session_.__GetTaggedObject(developObj.Tag);
            EditCurveLength(devLine, addLength, cursor);
        }

        private static void EditCurveLength(Line editLine, double editLength, double[] cursor)
        {
            Section section1 = __work_part_.Sections.CreateSection(0.0095, 0.01, 0.5);
            CurveLengthBuilder builder = __work_part_.Features.CreateCurvelengthBuilder(null);

            using (session_.__UsingDoUpdate("Edit Curve Length"))
            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.Section = section1;
                builder.DistanceTolerance = 0.01;
                builder.CurveOptions.Associative = false;
                builder.CurveOptions.InputCurveOption = CurveOptions.InputCurve.Replace;
                builder.CurvelengthData.ExtensionMethod = ExtensionMethod.Incremental;
                builder.CurvelengthData.ExtensionSide = ExtensionSide.Symmetric;
                builder.CurvelengthData.ExtensionDirection = ExtensionDirection.Natural;
                section1.DistanceTolerance = 0.01;
                section1.ChainingTolerance = 0.0095;
                section1.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                builder.CurvelengthData.ExtensionSide = ExtensionSide.StartEnd;
                Curve[] curves1 = new Curve[1];
                curves1[0] = editLine;
                CurveDumbRule curveDumbRule1 = __work_part_.ScRuleFactory.CreateRuleCurveDumb(curves1);
                section1.AllowSelfIntersection(true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = curveDumbRule1;
                Point3d helpPoint1 = new Point3d(cursor[0], cursor[1], cursor[2]);
                section1.AddToSection(rules1, editLine, null, null, helpPoint1, Section.Mode.Create, false);
                builder.CurvelengthData.SetStartDistance(editLength.ToString());
                builder.CurvelengthData.SetEndDistance("0");
                builder.CurveOptions.InputCurveOption = CurveOptions.InputCurve.Retain;
                builder.Commit();
            }
        }

        private static void AskCloserToStartOrEnd(Tag curve, double[] cursorLocation)
        {
            double[] limits = new double[2];
            double[] startPoint = new double[3];
            double[] endPoint = new double[3];
            double[] derivatives = new double[3];
            double[] closestPoint = new double[3];
            TheUFSession.Eval.Initialize(curve, out IntPtr evaluator);
            TheUFSession.Eval.AskLimits(evaluator, limits);
            TheUFSession.Eval.Evaluate(evaluator, 0, limits[0], startPoint, derivatives);
            TheUFSession.Eval.Evaluate(evaluator, 0, limits[1], endPoint, derivatives);
            TheUFSession.Eval.EvaluateClosestPoint(evaluator, cursorLocation, out _, closestPoint);
            TheUFSession.Eval.Free(evaluator);
            TheUFSession.Vec3.Distance(closestPoint, startPoint, out _);
            TheUFSession.Vec3.Distance(closestPoint, endPoint, out _);
        }

        private static void MapViewToAbsolute(ref double[] cursorLocation)
        {
            double[] destinationCsys = { 0, 0, 0, 1, 0, 0, 0, 1, 0 };
            double[] refViewCsys = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] outputMatrix = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            TheUFSession.Ui.AskLastPickedView(out string viewName);
            View lastViewPicked = __work_part_.ModelingViews.FindObject(viewName);
            refViewCsys[3] = lastViewPicked.Matrix.Xx;
            refViewCsys[4] = lastViewPicked.Matrix.Xy;
            refViewCsys[5] = lastViewPicked.Matrix.Xz;
            refViewCsys[6] = lastViewPicked.Matrix.Yx;
            refViewCsys[7] = lastViewPicked.Matrix.Yy;
            refViewCsys[8] = lastViewPicked.Matrix.Yz;
            refViewCsys[9] = lastViewPicked.Matrix.Zx;
            refViewCsys[10] = lastViewPicked.Matrix.Zy;
            refViewCsys[11] = lastViewPicked.Matrix.Zz;
            TheUFSession.Trns.CreateCsysMappingMatrix(refViewCsys, destinationCsys, outputMatrix, out _);
            TheUFSession.Trns.MapPosition(cursorLocation, outputMatrix);
        }

        private static void MapAbsoluteToView(ref double[] cursorLocation)
        {
            double[] destinationCsys = { 0, 0, 0, 1, 0, 0, 0, 1, 0 };
            double[] refViewCsys = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] outputMatrix = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            TheUFSession.Ui.AskLastPickedView(out string viewName);
            View lastViewPicked = __work_part_.ModelingViews.FindObject(viewName);
            refViewCsys[3] = lastViewPicked.Matrix.Xx;
            refViewCsys[4] = lastViewPicked.Matrix.Xy;
            refViewCsys[5] = lastViewPicked.Matrix.Xz;
            refViewCsys[6] = lastViewPicked.Matrix.Yx;
            refViewCsys[7] = lastViewPicked.Matrix.Yy;
            refViewCsys[8] = lastViewPicked.Matrix.Yz;
            refViewCsys[9] = lastViewPicked.Matrix.Zx;
            refViewCsys[10] = lastViewPicked.Matrix.Zy;
            refViewCsys[11] = lastViewPicked.Matrix.Zz;
            TheUFSession.Trns.CreateCsysMappingMatrix(destinationCsys, refViewCsys, outputMatrix, out _);
            TheUFSession.Trns.MapPosition(cursorLocation, outputMatrix);
        }

        private static void AskPositionOnObject(Tag obj, double[] cursorLocation)
        {
            double[] cp = { 0, 0, 0 };
            UFCurve.Line lp;
            double[] startPoint = new double[3];
            double[] endPoint = new double[3];
            lp.start_point = startPoint;
            lp.end_point = endPoint;
            MapAbsoluteToView(ref cursorLocation);
            lp.start_point[0] = cursorLocation[0];
            lp.start_point[1] = cursorLocation[1];
            lp.start_point[2] = cursorLocation[2] + 10000;
            lp.end_point[0] = cursorLocation[0];
            lp.end_point[1] = cursorLocation[1];
            lp.end_point[2] = cursorLocation[2] - 10000;
            MapViewToAbsolute(ref lp.start_point);
            MapViewToAbsolute(ref lp.end_point);
            TheUFSession.Curve.CreateLine(ref lp, out Tag aLine);
            TheUFSession.Modl.AskMinimumDist(obj, aLine, 0, cp, 0, cp, out _, cursorLocation, cp);
            TheUFSession.Obj.DeleteObject(aLine);
        }
    }
}