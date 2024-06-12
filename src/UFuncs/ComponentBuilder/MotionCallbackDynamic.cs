﻿using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.__Extensions_;
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

                foreach (var eLine in _edgeRepLines)
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
                        $"X = {editBlkLength:0.000}  Y = {editBlkWidth:0.000}  Z = {$"{editBlkHeight:0.000}"}  Distance Moved =  {$"{_distanceMoved:0.000}"}");
                    return;
                }

                editBlkLength /= 25.4;
                editBlkWidth /= 25.4;
                editBlkHeight /= 25.4;

                var convertDistMoved = _distanceMoved / 25.4;

                ufsession_.Ui.SetPrompt($"X = {editBlkLength:0.000}  " +
                    $"Y = {editBlkWidth:0.000}  " +
                    $"Z = {editBlkHeight:0.000}  " +
                    $"Distance Moved =  {convertDistMoved:0.000}");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }









        private void MotionCallbackNegYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                YStartPoint(yDistance, yAxisLine);
            }

            NewMethod11(movePtsHalf, movePtsFull, yDistance);
        }





        private void MotionCallbackPosYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                YEndPoint(yDistance, yAxisLine);
            }

            NewMethod18(movePtsHalf, movePtsFull, yDistance);
        }



        private void MotionCallbackNegXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                XStartPoint(xDistance, xAxisLine);
            }

            NewMethod20(movePtsHalf, movePtsFull, xDistance);
        }



        private void MotionCallbackPosZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                ZEndPoint(zDistance, zAxisLine);
            }

            NewMethod14(movePtsHalf, movePtsFull, zDistance);
        }





        private void MotionCallbackPosXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                XEndPoint(xDistance, xAxisLine);
            }

            NewMethod22(movePtsHalf, movePtsFull, xDistance);
        }


        private void MotionCallbackDyanmic(double[] position)
        {
            var pointPrototype = _udoPointHandle.IsOccurrence
                                    ? (Point)_udoPointHandle.Prototype
                                    : _udoPointHandle;

            var doNotMovePts = new List<NXObject>();
            var movePtsHalf = new List<NXObject>();
            var movePtsFull = new List<NXObject>();

            if (pointPrototype.Name.Contains("POS"))
                MotionCallbackDynamicPos(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
            else
                MotionCallbackDynamicNeg(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);

            var posXObjs = new List<Line>();
            var negXObjs = new List<Line>();
            var posYObjs = new List<Line>();
            var negYObjs = new List<Line>();
            var posZObjs = new List<Line>();
            var negZObjs = new List<Line>();

            foreach (var eLine in _edgeRepLines)
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

            var allxAxisLines = new List<Line>();
            var allyAxisLines = new List<Line>();
            var allzAxisLines = new List<Line>();

            foreach (var eLine in _edgeRepLines)
            {
                if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
            }

            // get the distance from the selected point to the cursor location

            double[] pointStart =
                { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

            var mappedPoint = new double[3];
            var mappedCursor = new double[3];

            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, posXObjs, negXObjs, allxAxisLines, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, posYObjs, negYObjs, allyAxisLines, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, posZObjs, negZObjs, allzAxisLines, mappedPoint, mappedCursor);
        }





        private void MotionCallbackXDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> negXObjs, List<Line> allxAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0])
                return;

            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

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

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> negZObjs, List<Line> allzAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] != mappedCursor[2])
            {
                var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                if (zDistance >= _gridSpace)
                {
                    if (mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                    _distanceMoved += zDistance;

                    if (pointPrototype.Name == "POSZ")
                    {
                        MotionCallbackPosZDyanmic(movePtsHalf, movePtsFull, posZObjs, allzAxisLines, zDistance);
                    }
                    else
                    {
                        MotionCallbackNegZDyanmic(movePtsHalf, movePtsFull, negZObjs, allzAxisLines, zDistance);
                    }

                    var mView1 = (ModelingView)_displayPart.Views.WorkView;
                    _displayPart.Views.WorkView.Orient(mView1.Matrix);
                    _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                }
            }
        }

        private void MotionCallbackYDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> negYObjs, List<Line> allyAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    if (pointPrototype.Name == "POSY")
                    {
                        MotionCallbackPosYDyanmic(movePtsHalf, movePtsFull, posYObjs, allyAxisLines, yDistance);
                    }
                    else
                    {
                        MotionCallbackNegYDyanmic(movePtsHalf, movePtsFull, negYObjs, allyAxisLines, yDistance);
                    }
                }
            }
        }

        private void MotionCallbackNegZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                ZStartPoint(zDistance, zAxisLine);
            }

            NewMethod15(movePtsHalf, movePtsFull, zDistance);
        }


        //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$




        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            var moveAll = new List<NXObject>();

            foreach (Point namedPts in _workPart.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            foreach (var eLine in _edgeRepLines) moveAll.Add(eLine);

            // get the distance from the selected point to the cursor location

            double[] pointStart =
                { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

            var mappedPoint = new double[3];
            var mappedCursor = new double[3];

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
            if (mappedPoint[0] == mappedCursor[0])
            {
                return;
            }
            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace)
            {
                return;
            }
            if (mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

            _distanceMoved += xDistance;

            MoveObjects(moveAll.ToArray(), xDistance, "X");
        }

        private void MotionCallbackNotDynamicY(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

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

            var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

            if (zDistance < _gridSpace)
                return;

            if (mappedCursor[2] < mappedPoint[2])
                zDistance *= -1;

            _distanceMoved += zDistance;
            MoveObjects(moveAll.ToArray(), zDistance, "Z");
            var mView1 = (ModelingView)_displayPart.Views.WorkView;
            _displayPart.Views.WorkView.Orient(mView1.Matrix);
            _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
        }





    }
}
