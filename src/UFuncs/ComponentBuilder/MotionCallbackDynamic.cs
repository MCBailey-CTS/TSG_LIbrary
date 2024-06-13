using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.UF;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
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

                if (_displayPart.PartUnits == BasePart.Units.Inches)
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


        private void MotionCallbackNegYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (Line addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YStartPoint(yDistance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, yDistance, "Y", true);
        }


        private void MotionCallbackPosYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (Line addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YEndPoint(yDistance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, yDistance, "Y", true);
        }


        private void MotionCallbackNegXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (Line addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (Line xAxisLine in allxAxisLines) XStartPoint(xDistance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, xDistance, "X", true);
        }


        private void MotionCallbackPosZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (Line addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZEndPoint(zDistance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, zDistance, "Z", true);
        }


        private void MotionCallbackPosXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (Line posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (Line xAxisLine in allxAxisLines) XEndPoint(xDistance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, xDistance, "X", true);
        }


        private void MotionCallbackDyanmic(double[] position)
        {
            Point pointPrototype = _udoPointHandle.IsOccurrence
                ? (Point)_udoPointHandle.Prototype
                : _udoPointHandle;

            List<NXObject> doNotMovePts = new List<NXObject>();
            List<NXObject> movePtsHalf = new List<NXObject>();
            List<NXObject> movePtsFull = new List<NXObject>();

            MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull,
                pointPrototype.Name.Contains("POS"));

            List<Line> posXObjs = new List<Line>();
            List<Line> negXObjs = new List<Line>();
            List<Line> posYObjs = new List<Line>();
            List<Line> negYObjs = new List<Line>();
            List<Line> posZObjs = new List<Line>();
            List<Line> negZObjs = new List<Line>();

            foreach (Line eLine in _edgeRepLines)
            {
                switch (eLine.Name)
                {
                    case "YBASE1":
                    case "YCEILING1":
                    case "ZBASE1":
                    case "ZBASE3":
                        negXObjs.Add(eLine);
                        break;
                }

                switch (eLine.Name)
                {
                    case "YBASE2":
                    case "YCEILING2":
                    case "ZBASE2":
                    case "ZBASE4":
                        posXObjs.Add(eLine);
                        break;
                }

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                    eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                    eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                    eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" || eLine.Name == "YCEILING1" ||
                    eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }

            List<Line> allxAxisLines = new List<Line>();
            List<Line> allyAxisLines = new List<Line>();
            List<Line> allzAxisLines = new List<Line>();

            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
            }

            // get the distance from the selected point to the cursor location

            double[] pointStart =
                { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];

            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, posXObjs, negXObjs, allxAxisLines,
                    mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, posYObjs, negYObjs, allyAxisLines,
                    mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, posZObjs, negZObjs, allzAxisLines,
                    mappedPoint, mappedCursor);
        }


        private void MotionCallbackXDynamic(
            Point pointPrototype,
            List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull,
            List<Line> posXObjs,
            List<Line> negXObjs,
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

            if (pointPrototype.Name == "POSX")
                MotionCallbackPosXDyanmic(movePtsHalf, movePtsFull, posXObjs, allxAxisLines, xDistance);
            else
                MotionCallbackNegXDyanmic(movePtsHalf, movePtsFull, negXObjs, allxAxisLines, xDistance);
        }

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> negZObjs, List<Line> allzAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] != mappedCursor[2])
            {
                double zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                if (zDistance >= _gridSpace)
                {
                    if (mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                    _distanceMoved += zDistance;

                    if (pointPrototype.Name == "POSZ")
                        MotionCallbackPosZDyanmic(movePtsHalf, movePtsFull, posZObjs, allzAxisLines, zDistance);
                    else
                        MotionCallbackNegZDyanmic(movePtsHalf, movePtsFull, negZObjs, allzAxisLines, zDistance);

                    ModelingView mView1 = (ModelingView)_displayPart.Views.WorkView;
                    _displayPart.Views.WorkView.Orient(mView1.Matrix);
                    _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                }
            }
        }

        private void MotionCallbackYDynamic(Point pointPrototype, List<NXObject> movePtsHalf,
            List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> negYObjs, List<Line> allyAxisLines,
            double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                double yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    if (pointPrototype.Name == "POSY")
                        MotionCallbackPosYDyanmic(movePtsHalf, movePtsFull, posYObjs, allyAxisLines, yDistance);
                    else
                        MotionCallbackNegYDyanmic(movePtsHalf, movePtsFull, negYObjs, allyAxisLines, yDistance);
                }
            }
        }

        private void MotionCallbackNegZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (Line addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZStartPoint(zDistance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, zDistance, "Z", true);
        }


        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            List<NXObject> moveAll = new List<NXObject>();

            foreach (Point namedPts in _workPart.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            foreach (Line eLine in _edgeRepLines) moveAll.Add(eLine);

            // get the distance from the selected point to the cursor location

            double[] pointStart = _udoPointHandle.Coordinates.__ToArray();

            double[] mappedPoint = new double[3];
            double[] mappedCursor = new double[3];

            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackNotDynamicX(moveAll, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackNotDynamicY(moveAll, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackNotDynamicZ(moveAll, mappedPoint, mappedCursor);
        }

        private void MotionCallbackNotDynamicX(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0]) return;
            double xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace) return;
            if (mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

            _distanceMoved += xDistance;

            MoveObjects(moveAll.ToArray(), xDistance, "X");
        }

        private void MotionCallbackNotDynamicY(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                double yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    MoveObjects(moveAll.ToArray(), yDistance, "Y");
                }
            }
        }

        private void MotionCallbackNotDynamicZ(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
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
            MoveObjects(moveAll.ToArray(), zDistance, "Z");
            ModelingView mView1 = (ModelingView)_displayPart.Views.WorkView;
            _displayPart.Views.WorkView.Orient(mView1.Matrix);
            _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
        }
    }
}