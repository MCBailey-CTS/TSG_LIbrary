using System.Collections.Generic;
using NXOpen;
using NXOpen.UF;
using System;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e) => AlignEdgeDistance();



        private double AlignEdgeDistance(
         List<NXObject> movePtsHalf,
         List<NXObject> movePtsFull,
         List<Line> lines,
         double inputDist,
         double[] mappedBase,
         double[] mappedPoint,
         int index,
         string dir_xyz,
         bool isPosEnd)
        {
            double distance = MapAndConvert(inputDist, mappedBase, mappedPoint, index);

            foreach (Line zAxisLine in lines)
                SetLineEndPoints(distance, zAxisLine, !isPosEnd, dir_xyz);


            MoveObjects(movePtsHalf, movePtsFull, distance, dir_xyz, false);
            return distance;
        }

        private void AlignEdgeDistance(bool isBlockComponent)
        {
            using (session_.__UsingFormShowHide(this))
            {
                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    NewMethod22();
                    return;
                }

                Part checkPartName = (Part)editComponent.Prototype;
                _updateComponent = editComponent;

                if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                    return;

                isBlockComponent = NewMethod1(isBlockComponent, editComponent);

                if (!isBlockComponent)
                {
                    ResetNonBlockError();
                    NewMethod23();
                    return;
                }

                List<Point> pHandle = SelectHandlePoint();

                _isDynamic = true;

                while (pHandle.Count == 1)
                {
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];
                    Point pointPrototype;

                    if (_udoPointHandle.IsOccurrence)
                        pointPrototype = (Point)_udoPointHandle.Prototype;
                    else
                        pointPrototype = _udoPointHandle;


                    MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf, out var movePtsFull, pointPrototype.Name.Contains("POS"));
                    GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs, out var negZObjs);

                    List<Line> allxAxisLines, allyAxisLines, allzAxisLines;
                    NewMethod38(out allxAxisLines, out allyAxisLines, out allzAxisLines);

                    string message = "Select Reference Point";
                    UFUi.PointBaseMethod pbMethod = UFUi.PointBaseMethod.PointInferred;
                    Tag selection = NXOpen.Tag.Null;
                    double[] basePoint = new double[3];
                    int response;

                    using (session_.__UsingLockUiFromCustom())
                        ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint, out response);

                    if (response == UF_UI_OK)
                    {
                        bool isDistance;

                        isDistance = NXInputBox.ParseInputNumber(
                            "Enter offset value",
                            "Enter offset value",
                            .004,
                            NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture.NumberFormat,
                            out double inputDist);

                        if (isDistance)
                        {
                            double[] mappedBase = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint, UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                            double[] pPrototype = pointPrototype.Coordinates.__ToArray();
                            double[] mappedPoint = new double[3];
                            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);

                            double distance;

                            switch (pointPrototype.Name)
                            {
                                case "POSX":
                                    movePtsFull.AddRange(posXObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist, mappedBase, mappedPoint, 0, "X", true);
                                    break;
                                case "NEGX":
                                    movePtsFull.AddRange(negXObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allxAxisLines, inputDist, mappedBase, mappedPoint, 0, "X", false);
                                    break;
                                case "POSY":
                                    movePtsFull.AddRange(posYObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist, mappedBase, mappedPoint, 1, "Y", true);
                                    break;
                                case "NEGY":
                                    movePtsFull.AddRange(negYObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allyAxisLines, inputDist, mappedBase, mappedPoint, 1, "Y", false);
                                    break;
                                case "POSZ":
                                    movePtsFull.AddRange(posZObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist, mappedBase, mappedPoint, 2, "Z", true);
                                    break;
                                case "NEGZ":
                                    movePtsFull.AddRange(negZObjs);
                                    distance = AlignEdgeDistance(movePtsHalf, movePtsFull, allzAxisLines, inputDist, mappedBase, mappedPoint, 2, "Z", false);
                                    break;
                            }
                        }
                        else
                        {
                            //Show();
                            NewMethod24();
                        }
                    }

                    pHandle = NewMethod5(editComponent);
                }
            }
        }

        private List<Point> NewMethod5(Component editComponent)
        {
            List<Point> pHandle;
            UpdateDynamicBlock(editComponent);
            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
            __work_component_ = editComponent;
            CreateEditData(editComponent);
            pHandle = SelectHandlePoint();
            return pHandle;
        }

        private void AlignEdgeDistance()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align Edge");

                AlignEdgeDistance(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
            }
        }


        private bool NewMethod1(bool isBlockComponent, Component editComponent)
        {
            if (_isNewSelection)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;

                if (__work_part_.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;
            return isBlockComponent;
        }
    }
}
// 2045