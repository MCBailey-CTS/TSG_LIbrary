using NXOpen.UF;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TSG_Library.Extensions.__Extensions_;
using NXOpen.Assemblies;
using NXOpen.Features;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {



        private bool EditSizeDisplay(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                        isBlockComponent = true;

            if (isBlockComponent)
            {
                DisableForm();

                if (_isNewSelection)
                {
                    CreateEditData(editComponent);

                    _isNewSelection = false;
                }

                var pHandle = new List<Point>();
                pHandle = SelectHandlePoint();

                _isDynamic = true;

                while (pHandle.Count == 1)
                {
                    HideDynamicHandles();

                    _udoPointHandle = pHandle[0];

                    var blockOrigin = new Point3d();
                    var blockLength = 0.00;
                    var blockWidth = 0.00;
                    var blockHeight = 0.00;

                    foreach (var eLine in _edgeRepLines)
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

                    var doNotMovePts = new List<NXObject>();
                    var movePtsHalf = new List<NXObject>();
                    var movePtsFull = new List<NXObject>();

                    if (pointPrototype.Name.Contains("POS"))
                    {
                        NewMethod5(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                    }
                    else
                    {
                        NewMethod8(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                    }

                    var posXObjs = new List<Line>();
                    var negXObjs = new List<Line>();
                    var posYObjs = new List<Line>();
                    var negYObjs = new List<Line>();
                    var posZObjs = new List<Line>();
                    var negZObjs = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
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

                    var allxAxisLines = new List<Line>();
                    var allyAxisLines = new List<Line>();
                    var allzAxisLines = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                    }

                    EditSizeForm sizeForm = null;
                    var convertLength = blockLength / 25.4;
                    var convertWidth = blockWidth / 25.4;
                    var convertHeight = blockHeight / 25.4;

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

                    if (sizeForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        var editSize = sizeForm.InputValue;
                        double distance = 0;

                        if (_displayPart.PartUnits == BasePart.Units.Millimeters) editSize *= 25.4;

                        if (editSize > 0)
                        {
                            if (pointPrototype.Name == "POSX")
                            {
                                distance = EditSizeDisplayPosX(blockLength, movePtsHalf, movePtsFull, posXObjs, allxAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGX")
                            {
                                distance = EditSizeDisplayNegX(blockLength, movePtsHalf, movePtsFull, negXObjs, allxAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "POSY")
                            {
                                distance = EditSizeDisplayPosY(blockWidth, movePtsHalf, movePtsFull, posYObjs, allyAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGY")
                            {
                                distance = EditSizeDisplayNegY(blockWidth, movePtsHalf, movePtsFull, negYObjs, allyAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "POSZ")
                            {
                                distance = EditSizeDisplayPosZ(blockHeight, movePtsHalf, movePtsFull, posZObjs, allzAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGZ")
                            {
                                distance = EditSizeDisplayNegZ(blockHeight, movePtsHalf, movePtsFull, negZObjs, allzAxisLines, editSize);
                            }
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
            }
            else
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
            }

            return isBlockComponent;
        }

        private double EditSizeDisplayPosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = editSize - blockHeight;
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod63(distance, zAxisLine);
            }

            NewMethod64(movePtsHalf, movePtsFull, distance);
            return distance;
        }





        private double EditSizeNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = blockHeight - editSize;
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod49(distance, zAxisLine);
            }

            NewMethod50(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditSizePosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = editSize - blockHeight;
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod47(distance, zAxisLine);
            }

            NewMethod48(movePtsHalf, movePtsFull, distance);
            return distance;
        }



        private double EditSizeNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = blockWidth - editSize;
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod61(distance, yAxisLine);
            }

            NewMethod62(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditSizePosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = editSize - blockWidth;
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod45(distance, yAxisLine);
            }

            NewMethod46(movePtsHalf, movePtsFull, distance);
            return distance;
        }


        private double EditSizeNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = blockLength - editSize;
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod43(distance, xAxisLine);
            }

            NewMethod44(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditSizePosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = editSize - blockLength;
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod41(distance, xAxisLine);
            }

            NewMethod42(movePtsHalf, movePtsFull, distance);
            return distance;
        }




        private double EditSizeDisplayNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = blockWidth - editSize;
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod74(distance, yAxisLine);
            }

            NewMethod73(movePtsHalf, movePtsFull, distance);
            return distance;
        }


        private double EditSizeDisplayPosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = editSize - blockWidth;
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod72(distance, yAxisLine);
            }

            NewMethod71(movePtsHalf, movePtsFull, distance);
            return distance;
        }



        private double EditSizeDisplayPosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = editSize - blockLength;
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod70(distance, xAxisLine);
            }

            NewMethod69(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditSizeDisplayNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = blockLength - editSize;
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod68(distance, xAxisLine);
            }

            NewMethod67(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private double EditSizeDisplayNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = blockHeight - editSize;
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod66(distance, zAxisLine);
            }

            NewMethod65(movePtsHalf, movePtsFull, distance);
            return distance;
        }






    }
}
