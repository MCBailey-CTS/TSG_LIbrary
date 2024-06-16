using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using System;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;
using NXOpen.Utilities;
using NXOpen.Preferences;
using TSG_Library.Properties;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {



        private static void MotionCallbackDynamic1(
            Point pointPrototype,
            out List<NXObject> doNotMovePts,
            out List<NXObject> movePtsHalf,
            out List<NXObject> movePtsFull,
            bool isPos)
        {
            doNotMovePts = new List<NXObject>();
            movePtsFull = new List<NXObject>();
            movePtsHalf = new List<NXObject>();

            foreach (Point namedPt in __work_part_.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    {
                        doNotMovePts.Add(namedPt);
                        continue;
                    }

                    if (namedPt.Name.Contains("BLKORIGIN"))
                    {
                        if (isPos)
                            doNotMovePts.Add(namedPt);
                        else
                            movePtsFull.Add(namedPt);
                        continue;
                    }

                    movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }




        private void MotionCallbackNotDynamic(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor, int index, string dir_xyz)
        {
            if (mappedPoint[index] == mappedCursor[index])
                return;

            double xDistance = Math.Sqrt(Math.Pow(mappedPoint[index] - mappedCursor[index], 2));

            if (xDistance < _gridSpace)
                return;

            if (mappedCursor[index] < mappedPoint[index])
                xDistance *= -1;

            _distanceMoved += xDistance;

            MoveObjects(moveAll.ToArray(), xDistance, dir_xyz);

            if (dir_xyz != "Z")
                return;
            NewMethod54();
        }

        private void MotionCallback(double[] position, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            try
            {
                if (_isDynamic)
                    MotionCallbackDyanmic(position);
                else
                    MotionCalbackNotDynamic(position);

                double editBlkLength = 0;
                double editBlkWidth = 0;
                double editBlkHeight = 0;

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name == "XBASE1")
                        editBlkLength = eLine.GetLength();

                    if (eLine.Name == "YBASE1")
                        editBlkWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1")
                        editBlkHeight = eLine.GetLength();
                }

                if (__display_part_.PartUnits == BasePart.Units.Inches)
                {
                    ufsession_.Ui.SetPrompt(
                        $"X = {editBlkLength:0.000}  " +
                        $"Y = {editBlkWidth:0.000}  " +
                        $"Z = {$"{editBlkHeight:0.000}"}  " +
                        $"Distance Moved =  {$"{_distanceMoved:0.000}"}");
                    return;
                }

                editBlkLength /= 25.4;
                editBlkWidth /= 25.4;
                editBlkHeight /= 25.4;

                double convertDistMoved = _distanceMoved / 25.4;

                ufsession_.Ui.SetPrompt(
                    $"X = {editBlkLength:0.000}  " +
                    $"Y = {editBlkWidth:0.000}  " +
                    $"Z = {editBlkHeight:0.000}  " +
                    $"Distance Moved =  {convertDistMoved:0.000}");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }




        private void MotionCallbackDyanmic(double[] position)
        {
            Point pointPrototype = _udoPointHandle.IsOccurrence
                ? (Point)_udoPointHandle.Prototype
                : _udoPointHandle;



            MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf, out var movePtsFull,
                pointPrototype.Name.Contains("POS"));

            GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs, out var negZObjs);

            NewMethod41(out var allxAxisLines, out var allyAxisLines, out var allzAxisLines);

            // get the distance from the selected point to the cursor location

            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            switch (pointPrototype.Name)
            {
                case "POSX":
                    movePtsFull.AddRange(posXObjs);
                    MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, allxAxisLines, mappedPoint, mappedCursor);
                    break;
                case "NEGX":
                    movePtsFull.AddRange(negXObjs);
                    MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, allxAxisLines, mappedPoint, mappedCursor);
                    break;
                case "POSY":
                    movePtsFull.AddRange(posYObjs);
                    MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, allyAxisLines, mappedPoint, mappedCursor, 1);
                    break;
                case "NEGY":
                    movePtsFull.AddRange(negYObjs);
                    MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, allyAxisLines, mappedPoint, mappedCursor,1 );
                    break;
                case "POSZ":
                    movePtsFull.AddRange(posZObjs);
                    MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, allzAxisLines, mappedPoint, mappedCursor);
                    break;
                case "NEGZ":
                    movePtsFull.AddRange(negZObjs);
                    MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, allzAxisLines, mappedPoint, mappedCursor);
                    break;
            }
        }



        private void MotionCallbackXDynamic(
           Point pointPrototype,
           List<NXObject> movePtsHalf,
           List<NXObject> movePtsFull,
           List<Line> allxAxisLines,
           double[] mappedPoint,
           double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0])
                return;

            double xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace)
                return;

            if (mappedCursor[0] < mappedPoint[0])
                xDistance *= -1;

            _distanceMoved += xDistance;

            EditSizeOrAlign(xDistance, movePtsHalf, movePtsFull, allxAxisLines,  "X", pointPrototype.Name == "POSX");
        }

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> allzAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] == mappedCursor[2])
                return;

            double zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

            if (zDistance < _gridSpace)
                return;

            if (mappedCursor[2] < mappedPoint[2])
                zDistance *= -1;

            _distanceMoved += zDistance;
            EditSizeOrAlign(zDistance, movePtsHalf, movePtsFull, allzAxisLines, "Z", pointPrototype.Name == "POSZ");
            NewMethod9();
        }

        private static void NewMethod9()
        {
            ModelingView mView1 = (ModelingView)__display_part_.Views.WorkView;
            __display_part_.Views.WorkView.Orient(mView1.Matrix);
            __display_part_.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
        }

        private void MotionCallbackYDynamic(
            Point pointPrototype,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> allyAxisLines,
            double[] mappedPoint,
            double[] mappedCursor, 
            int index)
        {
            if (mappedPoint[index] == mappedCursor[index])
                return;

            double yDistance = Math.Sqrt(Math.Pow(mappedPoint[index] - mappedCursor[index], 2));

            if (yDistance < _gridSpace)
                return;

            if (mappedCursor[index] < mappedPoint[index])
                yDistance *= -1;

            _distanceMoved += yDistance;
            EditSizeOrAlign(yDistance, movePtsHalf, movePtsFull, allyAxisLines, "Y", pointPrototype.Name == "POSY");
        }

        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            List<NXObject> moveAll = new List<NXObject>();

            foreach (Point namedPts in __work_part_.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            moveAll.AddRange(_edgeRepLines);
            // get the distance from the selected point to the cursor location
            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();
            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];
            __display_part_.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 0, "X");

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 1, "Y");

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackNotDynamic(moveAll, mappedPoint, mappedCursor, 2, "Z");
        }



    }
}
// 2045