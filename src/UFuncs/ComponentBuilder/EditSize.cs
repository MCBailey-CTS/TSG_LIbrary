using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private bool EditSizeDisplay(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show(
                    "Caught exception",
                    NXMessageBox.DialogType.Error,
                    "Mirrored Component");

                return isBlockComponent;
            }

            if (!_workPart.__HasDynamicBlock())
            {
                ResetNonBlockError();

                TheUISession.NXMessageBox.Show(
                    "Caught exception",
                    NXMessageBox.DialogType.Error,
                    "Not a block component");

                return isBlockComponent;
            }

            DisableForm();

            if (_isNewSelection)
            {
                CreateEditData(editComponent);

                _isNewSelection = false;
            }

            List<Point> pHandle = SelectHandlePoint();

            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();

                _udoPointHandle = pHandle[0];

                Point3d blockOrigin = new Point3d();
                double blockLength = 0.00;
                double blockWidth = 0.00;
                double blockHeight = 0.00;

                foreach (Line eLine in _edgeRepLines)
                {
                    if (eLine.Name == "XBASE1")
                    {
                        blockOrigin = eLine.StartPoint;
                        blockLength = eLine.GetLength();
                    }

                    if (eLine.Name == "YBASE1") blockWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1") blockHeight = eLine.GetLength();
                }

                Point pointPrototype;

                if (_udoPointHandle.IsOccurrence)
                    pointPrototype = (Point)_udoPointHandle.Prototype;
                else
                    pointPrototype = _udoPointHandle;

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
                    if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" || eLine.Name == "ZBASE1" ||
                        eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                    if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" || eLine.Name == "ZBASE2" ||
                        eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                    if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                        eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                    if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                        eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                    if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                        eLine.Name == "YBASE2") negZObjs.Add(eLine);

                    if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                        eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
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

                EditSizeForm sizeForm = null;
                double convertLength = blockLength / 25.4;
                double convertWidth = blockWidth / 25.4;
                double convertHeight = blockHeight / 25.4;

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    if (pointPrototype.Name.Contains("X"))
                    {
                        sizeForm = new EditSizeForm(blockLength);
                        sizeForm.ShowDialog();
                    }

                    if (pointPrototype.Name.Contains("Y"))
                    {
                        sizeForm = new EditSizeForm(blockWidth);
                        sizeForm.ShowDialog();
                    }

                    if (pointPrototype.Name.Contains("Z"))
                    {
                        sizeForm = new EditSizeForm(blockHeight);
                        sizeForm.ShowDialog();
                    }
                }
                else
                {
                    if (pointPrototype.Name.Contains("X"))
                    {
                        sizeForm = new EditSizeForm(convertLength);
                        sizeForm.ShowDialog();
                    }

                    if (pointPrototype.Name.Contains("Y"))
                    {
                        sizeForm = new EditSizeForm(convertWidth);
                        sizeForm.ShowDialog();
                    }

                    if (pointPrototype.Name.Contains("Z"))
                    {
                        sizeForm = new EditSizeForm(convertHeight);
                        sizeForm.ShowDialog();
                    }
                }

                if (sizeForm.DialogResult == DialogResult.OK)
                {
                    double editSize = sizeForm.InputValue;
                    double distance = 0;

                    if (_displayPart.PartUnits == BasePart.Units.Millimeters) editSize *= 25.4;

                    if (editSize > 0)
                    {
                        if (pointPrototype.Name == "POSX")
                            distance = EditSizeDisplayPosX(blockLength, movePtsHalf, movePtsFull, posXObjs,
                                allxAxisLines, editSize);

                        if (pointPrototype.Name == "NEGX")
                            distance = EditSizeDisplayNegX(blockLength, movePtsHalf, movePtsFull, negXObjs,
                                allxAxisLines, editSize);

                        if (pointPrototype.Name == "POSY")
                            distance = EditSizeDisplayPosY(blockWidth, movePtsHalf, movePtsFull, posYObjs,
                                allyAxisLines, editSize);

                        if (pointPrototype.Name == "NEGY")
                            distance = EditSizeDisplayNegY(blockWidth, movePtsHalf, movePtsFull, negYObjs,
                                allyAxisLines, editSize);

                        if (pointPrototype.Name == "POSZ")
                            distance = EditSizeDisplayPosZ(blockHeight, movePtsHalf, movePtsFull, posZObjs,
                                allzAxisLines, editSize);

                        if (pointPrototype.Name == "NEGZ")
                            distance = EditSizeDisplayNegZ(blockHeight, movePtsHalf, movePtsFull, negZObjs,
                                allzAxisLines, editSize);
                    }
                }

                UpdateDynamicBlock(editComponent);

                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                sizeForm.Close();
                sizeForm.Dispose();

                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                __work_component_ = editComponent;
                UpdateSessionParts();

                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();

                CreateEditData(editComponent);

                pHandle = SelectHandlePoint();
            }

            EnableForm();

            return isBlockComponent;
        }

        private double EditSizeDisplayPosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            double distance = editSize - blockHeight;
            foreach (Line addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZEndPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }


        private double EditSizeNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            double distance = blockHeight - editSize;
            foreach (Line addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZStartPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }

        private double EditSizePosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            double distance = editSize - blockHeight;
            foreach (Line addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZEndPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }


        private double EditSizeNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            double distance = blockWidth - editSize;
            foreach (Line addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YStartPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }

        private double EditSizePosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            double distance = editSize - blockWidth;
            foreach (Line addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YEndPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }


        private double EditSizeNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            double distance = blockLength - editSize;
            foreach (Line addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (Line xAxisLine in allxAxisLines) XStartPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }

        private double EditSizePosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            double distance = editSize - blockLength;
            foreach (Line posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (Line xAxisLine in allxAxisLines) XEndPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }


        private double EditSizeDisplayNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            double distance = blockWidth - editSize;
            foreach (Line addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YStartPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }


        private double EditSizeDisplayPosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            double distance = editSize - blockWidth;
            foreach (Line addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (Line yAxisLine in allyAxisLines) YEndPoint(distance, yAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Y", false);
            return distance;
        }


        private double EditSizeDisplayPosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            double distance = editSize - blockLength;
            foreach (Line posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (Line xAxisLine in allxAxisLines) XEndPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }

        private double EditSizeDisplayNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            double distance = blockLength - editSize;
            foreach (Line addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (Line xAxisLine in allxAxisLines) XStartPoint(distance, xAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "X", false);
            return distance;
        }

        private double EditSizeDisplayNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull,
            List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            double distance = blockHeight - editSize;
            foreach (Line addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (Line zAxisLine in allzAxisLines) ZStartPoint(distance, zAxisLine);

            MoveObjects(movePtsHalf, movePtsFull, distance, "Z", false);
            return distance;
        }
    }
}