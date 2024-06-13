using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Utilities;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void SetWcsToWorkPart(Component compRefCsys)
        {
            try
            {
                if (compRefCsys is null)
                    SetWcsDisplayPart();
                else
                    SetWcsWorkComponent(compRefCsys);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

        private static void SetWcsDisplayPart()
        {
            _isUprParallel = false;
            _isLwrParallel = false;
            _parallelHeightExp = string.Empty;

            foreach (Expression exp in _workPart.Expressions)
            {
                if (exp.Name == "uprParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isUprParallel = true;
                    else
                        _isUprParallel = false;
                }

                if (exp.Name == "lwrParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isLwrParallel = true;
                    else
                        _isLwrParallel = false;
                }
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                    {
                        Block block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                        string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        double mLength = blockFeatureBuilderMatch.Length.Value;
                        double mWidth = blockFeatureBuilderMatch.Width.Value;
                        double mHeight = blockFeatureBuilderMatch.Height.Value;

                        if (_isUprParallel)
                        {
                            _parallelHeightExp = "uprParallelHeight";
                            _parallelWidthExp = "uprParallelWidth";
                        }

                        if (_isLwrParallel)
                        {
                            _parallelHeightExp = "lwrParallelHeight";
                            _parallelWidthExp = "lwrParallelWidth";
                        }

                        blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                        CartesianCoordinateSystem setTempCsys =
                            (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);
                        _workCompOrigin = setTempCsys.Origin;
                        _workCompOrientation = setTempCsys.Orientation.Element;
                    }
        }

        private void SetWcsWorkComponent(Component compRefCsys)
        {
            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

            BasePart compBase = (BasePart)compRefCsys.Prototype;

            __display_part_ = (Part)compBase;
            UpdateSessionParts();

            _isUprParallel = false;
            _isLwrParallel = false;
            _parallelHeightExp = string.Empty;
            _parallelWidthExp = string.Empty;

            foreach (Expression exp in _workPart.Expressions)
            {
                if (exp.Name == "uprParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isUprParallel = true;
                    else
                        _isUprParallel = false;
                }

                if (exp.Name == "lwrParallel")
                {
                    if (exp.RightHandSide.Contains("yes"))
                        _isLwrParallel = true;
                    else
                        _isLwrParallel = false;
                }
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                    {
                        Block block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                        string blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        string bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        string bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        double mLength = blockFeatureBuilderMatch.Length.Value;
                        double mWidth = blockFeatureBuilderMatch.Width.Value;
                        double mHeight = blockFeatureBuilderMatch.Height.Value;

                        if (_isUprParallel)
                        {
                            _parallelHeightExp = "uprParallelHeight";
                            _parallelWidthExp = "uprParallelWidth";
                        }

                        if (_isLwrParallel)
                        {
                            _parallelHeightExp = "lwrParallelHeight";
                            _parallelWidthExp = "lwrParallelWidth";
                        }

                        blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                        CartesianCoordinateSystem setTempCsys =
                            (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);

                        CartesianCoordinateSystem featBlkCsys = _displayPart.WCS.Save();
                        featBlkCsys.SetName("EDITCSYS");
                        featBlkCsys.Layer = 254;

                        NXObject[] addToBody = { featBlkCsys };

                        foreach (ReferenceSet bRefSet in _displayPart.GetAllReferenceSets())
                            if (bRefSet.Name == "BODY")
                                bRefSet.AddObjectsToReferenceSet(addToBody);

                        __display_part_ = _originalDisplayPart;
                        __work_component_ = compRefCsys;
                        UpdateSessionParts();

                        foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                            if (wpCsys.Layer == 254)
                                if (wpCsys.Name == "EDITCSYS")
                                {
                                    NXObject csysOccurrence;
                                    csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                    CartesianCoordinateSystem editCsys = (CartesianCoordinateSystem)csysOccurrence;

                                    if (editCsys != null)
                                    {
                                        _displayPart.WCS.SetOriginAndMatrix(editCsys.Origin,
                                            editCsys.Orientation.Element);
                                        _workCompOrigin = editCsys.Origin;
                                        _workCompOrientation = editCsys.Orientation.Element;
                                    }

                                    Session.UndoMarkId markDeleteObjs;
                                    markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
                                    session_.UpdateManager.AddToDeleteList(wpCsys);

                                    int errsDelObjs;
                                    errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

                                    session_.DeleteUndoMark(markDeleteObjs, "");
                                }
                    }

            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }
    }
}