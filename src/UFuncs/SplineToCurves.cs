using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using Selection = TSG_Library.Ui.Selection;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_spline_to_curves)]
    public class SplineToCurves : _UFunc
    {
        private const int MaxLength = 100000;

        public override void execute()
        {
            try
            {
                print_($"Launched: {ufunc_rev_name}");
                // generate curve pts tol \/
                double[] tolerances = { 0.005, 0.1, 1.0 }; // 0.005,, was 0.01, 0.1, 1.0
                // create new geom tolerance \/
                double tolerance = 0.05; // 0.05 was 0.125
                double jumpGap = 1.0; // was 2.0
                double[][] pts = new double[MaxLength][];

                for (int i = 0; i < MaxLength; i++)
                    pts[i] = new double[3];

                double[][] xyz = new double[MaxLength][];

                for (int i = 0; i < MaxLength; i++)
                    xyz[i] = new double[3];

                int cc;
                const int currentCurve = 0;
                int currentCurveEnd = 2;
                int numOfCurves = 0;
                int numOfLines = 0;
                int numOfPts = 0;
                double[][] origLineData = new double[200][];

                for (int i = 0; i < 200; i++)
                    origLineData[i] = new double[7];

                int[][] curvesSe = new int[200][];

                for (int i = 0; i < 200; i++)
                    curvesSe[i] = new int[9];

                UFCurve.PtSlopeCrvatr[] pointData = new UFCurve.PtSlopeCrvatr[MaxLength];
                GetPartUnits(tolerances, ref jumpGap, ref tolerance);
                Tag[] curves = Selection.SelectCurves(ufunc_rev_name).Select(curve => curve.Tag).ToArray();
                ufsession_.Disp.Refresh();

                for (cc = 0; cc < curves.Length; cc++)
                    ProcessCurve(tolerances, curves[cc], ref numOfCurves, curvesSe, xyz, origLineData, ref numOfLines);

                GetCurveOrder(numOfCurves, currentCurve, currentCurveEnd, jumpGap, curvesSe, xyz);
                currentCurveEnd = 3;
                GetCurveOrder(numOfCurves, currentCurve, currentCurveEnd, jumpGap, curvesSe, xyz);
                int ret = RecallPointsFillArray(numOfCurves, curvesSe, ref numOfPts, pts, xyz);

                if (ret != 0) 
                    return;

                int ii;

                for (ii = 0; ii < numOfPts; ii++)
                {
                    double[] tempVec = new double[3];
                    ufsession_.Vec3.Copy(pts[ii], tempVec);
                    pointData[ii].point = tempVec;

                    pointData[ii].slope_type = ii == 0 || ii == numOfPts 
                        ? UFConstants.UF_CURVE_SLOPE_AUTO 
                        : UFConstants.UF_CURVE_SLOPE_NONE;

                    pointData[ii].crvatr_type = UFConstants.UF_CURVE_CRVATR_NONE;
                }

                ufsession_.Curve.CreateSplineThruPts(3, 0, numOfPts, pointData, null, 0, out Tag spline);
                ufsession_.Curve.CreateSimplifiedCurve(1, new[] { spline }, tolerance, out _, out _);

                ufsession_.Obj.DeleteObject(spline);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        // 8	the other curves start / end that is closest to this curve's end(3)
        // 7	the other curves start / end that is closest to this curve's start(2)
        // 6	the other curve's array loc that is closest to this curve's end(3)
        // 5	the other curve's array loc that is closest to this curve's start(2)
        // 4	this curve's tag
        // 3	this curve's xyz (end)
        // 2	this curve's xyz (start)
        // 1	extracted  yes(1) / no(0), NOT used.
        // 0	recall order, NOT used.

        private static void GetPartUnits(IList<double> gTolerances, ref double jumpGap, ref double tolerance)
        {
            Tag partTag = ufsession_.Part.AskDisplayPart();
            ufsession_.Part.AskUnits(partTag, out int partUnits);

            if (partUnits != UFConstants.UF_PART_ENGLISH) 
                return;

            jumpGap = 0.07;
            gTolerances[0] = 0.0002;
            tolerance = 0.001;
        }

        private static void GetCurveOrder(
            int numOfCurves,
            int currentCurve,
            int currentCurveEnd,
            double jumpGap,
            IReadOnlyList<int[]> curvesSe,
            IReadOnlyList<double[]> xyz)
        {
            int curveMatch = 0, done = 0;

            while (done == 0)
            {
                double dist = 1000000.0;
                bool endMatch = false;
                bool startMatch = false;
                int jj;

                for (jj = 0; jj < numOfCurves; jj++)
                {
                    if (jj == currentCurve) 
                        continue;

                    double tmpDist;

                    if (curvesSe[jj][7] == 0) // Start of curve has NOT been matched up yet
                    {
                        tmpDist = CheckDistance(xyz[curvesSe[currentCurve][currentCurveEnd]][0],
                            xyz[curvesSe[currentCurve][currentCurveEnd]][1],
                            xyz[curvesSe[currentCurve][currentCurveEnd]][2],
                            xyz[curvesSe[jj][2]][0], xyz[curvesSe[jj][2]][1], xyz[curvesSe[jj][2]][2]);

                        if (tmpDist < dist)
                        {
                            dist = tmpDist;
                            curveMatch = jj;
                            startMatch = true;
                            endMatch = false;
                        }
                    }

                    if (curvesSe[jj][8] != 0) 
                        continue; // End of curve has NOT been matched up yet

                    tmpDist = CheckDistance(xyz[curvesSe[currentCurve][currentCurveEnd]][0],
                        xyz[curvesSe[currentCurve][currentCurveEnd]][1],
                        xyz[curvesSe[currentCurve][currentCurveEnd]][2],
                        xyz[curvesSe[jj][3]][0], xyz[curvesSe[jj][3]][1], xyz[curvesSe[jj][3]][2]);

                    if (!(tmpDist < dist)) 
                        continue;

                    dist = tmpDist;
                    curveMatch = jj;
                    endMatch = true;
                    startMatch = false;
                }

                if (dist >= jumpGap)
                {
                    done = 1;
                    continue;
                }

                if (startMatch)
                {
                    curvesSe[curveMatch][7] = currentCurveEnd;
                    curvesSe[curveMatch][5] = currentCurve;
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (currentCurveEnd)
                    {
                        case 2:
                            curvesSe[currentCurve][5] = curveMatch;
                            curvesSe[currentCurve][7] = 2;
                            break;
                        case 3:
                            curvesSe[currentCurve][6] = curveMatch;
                            curvesSe[currentCurve][8] = 2;
                            break;
                    }

                    currentCurve = curveMatch;
                    currentCurveEnd = 3;
                }
                else if (endMatch)
                {
                    // set the curve we just matched to, = to curve number we compared to
                    curvesSe[curveMatch][6] =currentCurve;
                    // set the curve we just matches to, = to start(2) / end(3)
                    curvesSe[curveMatch][8] =currentCurveEnd; 

                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (currentCurveEnd == 2)
                    {
                        curvesSe[currentCurve][7] = 3; // We have a match to other curves end, so set to (3) end
                        curvesSe[currentCurve][5] = curveMatch; // number of curve closest to this end
                    }
                    else if (currentCurveEnd == 3)
                    {
                        curvesSe[currentCurve][8] = 3; //  We have a match to other curves end, so set to (3) end
                        curvesSe[currentCurve][6] = curveMatch; //  number of curve closest to this end
                    }

                    currentCurve = curveMatch; // The NEW curve to match up end to
                    currentCurveEnd =
                        2; // we matched up the end(3) of this NEW curve, so we will look for match to start(2) of it
                }
            }
        }

        //private static IEnumerable<NXOpen.Curve> SelectCurves()
        //{
        //    Snap.UI.Selection.Dialog selection = Snap.UI.Selection.SelectObject("Select Curves to Convert");
        //    selection.Title = "Select Curves to Convert";
        //    selection.KeepHighlighted = false;
        //    selection.AllowMultiple = true;
        //    selection.IncludeFeatures = false;
        //    selection.Scope = Snap.UI.Selection.Dialog.SelectionScope.AnyInAssembly;
        //    selection.SetCurveFilter(Snap.NX.ObjectTypes.Type.Line, Snap.NX.ObjectTypes.Type.Conic, Snap.NX.ObjectTypes.Type.Spline, Snap.NX.ObjectTypes.Type.Circle);
        //    Snap.UI.Selection.Result result = selection.Show();
        //    foreach (NXOpen.NXObject selectedCurve in result.Objects ?? new Snap.NX.Curve[0]) yield return (NXOpen.Curve)selectedCurve;
        //}

        private static double CheckDistance(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return
                Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
        }

        private static void ProcessCurve(IReadOnlyList<double> pTolerances, Tag curve, ref int numOfCurves,
            IReadOnlyList<int[]> curvesSe,
            IReadOnlyList<double[]> xyz, IReadOnlyList<double[]> origLineData, ref int numOfLines)
        {
            ufsession_.Curve.AskArcLength(curve, 0.0, 1.0, ModlUnits.ModlUnitsPart, out double arcLength);

            if (!(arcLength > 0.050)) return;
            ufsession_.Modl.AskCurvePoints(curve, pTolerances[0], pTolerances[1], pTolerances[2],
                out int np, out double[] pts);


            curvesSe[numOfCurves][4] = (int)curve; // curve tag
            curvesSe[numOfCurves][3] = curvesSe[numOfCurves][2] + (np - 1); // end loc
            curvesSe[numOfCurves + 1][2] = curvesSe[numOfCurves][2] + np; // next start loc
            curvesSe[numOfCurves][0] = -1; // recall order


            ufsession_.Eval.Initialize(curve, out IntPtr eval);
            ufsession_.Eval.IsLine(eval, out bool isLine);
            if (isLine)
            {
                ufsession_.Eval.AskLine(eval, out UFEval.Line uFEvalLine);
                origLineData[numOfLines][0] = uFEvalLine.start[0];
                origLineData[numOfLines][1] = uFEvalLine.start[1];
                origLineData[numOfLines][2] = uFEvalLine.start[2];
                origLineData[numOfLines][3] = uFEvalLine.end[0];
                origLineData[numOfLines][4] = uFEvalLine.end[1];
                origLineData[numOfLines][5] = uFEvalLine.end[2];
                origLineData[numOfLines][6] = uFEvalLine.length;
                ++numOfLines;
            }

            for (int i = 0; i < np; i++)
            {
                int j = i * 3;
                xyz[curvesSe[numOfCurves][2] + i][0] = pts[j];
                xyz[curvesSe[numOfCurves][2] + i][1] = pts[j + 1];
                xyz[curvesSe[numOfCurves][2] + i][2] = pts[j + 2];
            }

            ++numOfCurves;
        }

        private static int RecallPointsFillArray(int numOfCurves, IReadOnlyList<int[]> curvesSe,
            ref int numOfPts, IReadOnlyList<double[]> pts, IReadOnlyList<double[]> xyz)
        {
            bool done = false;
            bool enableClose = false;
            bool makeStart = false;
            double dist = 1.0;
            int ii;
            int endCnt = 0, startCnt = 0;
            int startEndCnt = 0,
                endCurve = 0,
                startCurve = 0,
                startCurveEnd = 0;

            for (ii = 0; ii < numOfCurves; ii++)
            {
                if (curvesSe[ii][7] == 0)
                    ++startCnt;

                if (curvesSe[ii][8] == 0)
                    ++endCnt;
            }

            if (startCnt == 2 && endCnt == 0)
            {
            }
            else if (endCnt == 2 && startCnt == 0)
            {
                makeStart = true;
            }


            for (ii = 0; ii < numOfCurves; ii++)
            {
                if (curvesSe[ii][7] == 0)
                {
                    ++startEndCnt;
                    startCurve = ii;
                    startCurveEnd = 2;
                }

                // ReSharper disable once InvertIf
                if (curvesSe[ii][8] == 0)
                {
                    if (makeStart && startEndCnt == 1)
                    {
                        ++startEndCnt;
                        startCurve = ii;
                        startCurveEnd = 3;
                    }
                    else
                    {
                        ++startEndCnt;
                        endCurve = ii;
                    }
                }
            }


            if (startEndCnt != 0 && startEndCnt != 2) return 1; // 0 = Contour is closed, 2 = Contour is open
            if (startEndCnt == 0)
            {
                startCurve = 0; // Contour is closed, so just start at 1st curve
                startCurveEnd = 2;
                //      endCurve = 654321;        Added this
                curvesSe[0][0] = 0;
            }

            int cc = startCurve;
            int cce = startCurveEnd;

            while (!done)
            {
                int jj;
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (cce == 0 &&
                    cc == endCurve) // current curve end is 0, as in there is NO end, we are on the last curve
                {
                    for (jj = curvesSe[cc][2]; jj <= curvesSe[cc][3]; jj++)
                    {
                        ufsession_.Vec3.Copy(xyz[jj], pts[numOfPts]);
                        ++numOfPts;
                    }

                    done = true;
                }
                else if
                    ((cce == 0 || enableClose) &&
                     cc == endCurve) // current curve end is 0, as in there is NO end, we are on the last curve
                {
                    done = true;
                }

                else if (cce == 2) // current curve end is start(2), so go from start(2) to end(3)
                {
                    for (jj = curvesSe[cc][2]; jj <= curvesSe[cc][3]; jj++)
                    {
                        ufsession_.Vec3.Copy(xyz[jj], pts[numOfPts]);
                        ++numOfPts;
                    }

                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (curvesSe[cc][8] == 0)
                        done = true;
                    else if (curvesSe[cc][8] == 2)
                        dist = CheckDistance(pts[numOfPts - 1][0], pts[numOfPts - 1][1], pts[numOfPts - 1][2],
                            xyz[curvesSe[curvesSe[cc][6]][2]][0], xyz[curvesSe[curvesSe[cc][6]][2]][1],
                            xyz[curvesSe[curvesSe[cc][6]][2]][2]);
                    else if (curvesSe[cc][8] == 3)
                        dist = CheckDistance(pts[numOfPts - 1][0], pts[numOfPts - 1][1], pts[numOfPts - 1][2],
                            xyz[curvesSe[curvesSe[cc][6]][3]][0], xyz[curvesSe[curvesSe[cc][6]][3]][1],
                            xyz[curvesSe[curvesSe[cc][6]][3]][2]);

                    if (enableClose && curvesSe[cc][6] == 0)
                    {
                    }
                    else
                    {
                        if (dist < 0.70)
                            --numOfPts;
                    }

                    cce = curvesSe[cc][8]; // Set cce to the end of next curve
                    cc = curvesSe[cc][6]; // Set cc to the next curve
                }
                else if (cce == 3) // current curve end is end(3), so go from end(3) to start(2)
                {
                    for (jj = curvesSe[cc][3]; jj >= curvesSe[cc][2]; jj--)
                    {
                        ufsession_.Vec3.Copy(xyz[jj], pts[numOfPts]);
                        ++numOfPts;
                    }


                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (curvesSe[cc][7] == 2)
                        dist = CheckDistance(pts[numOfPts - 1][0], pts[numOfPts - 1][1], pts[numOfPts - 1][2],
                            xyz[curvesSe[curvesSe[cc][5]][2]][0], xyz[curvesSe[curvesSe[cc][5]][2]][1],
                            xyz[curvesSe[curvesSe[cc][5]][2]][2]);
                    else if (curvesSe[cc][7] == 3)
                        dist = CheckDistance(pts[numOfPts - 1][0], pts[numOfPts - 1][1], pts[numOfPts - 1][2],
                            xyz[curvesSe[curvesSe[cc][5]][3]][0], xyz[curvesSe[curvesSe[cc][5]][3]][1],
                            xyz[curvesSe[curvesSe[cc][5]][3]][2]);

                    if (enableClose && curvesSe[cc][5] == 0)
                    {
                    }
                    else
                    {
                        if (dist < 0.70)
                            --numOfPts;
                    }


                    cce = curvesSe[cc][7]; // Set cce to the end of next curve
                    cc = curvesSe[cc][5]; // Set cc to the next curve
                }

                //    if (shit > 75)
                //        done = true;
                if (startEndCnt == 0)
                    enableClose = true;
            }

            return 0;
        }
    }
}
// 439