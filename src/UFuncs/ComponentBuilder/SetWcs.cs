using NXOpen.Features;
using NXOpen.UF;
using NXOpen.Utilities;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG_Library.Extensions;
using NXOpen.Assemblies;
using static TSG_Library.Extensions.__Extensions_;
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
                {
                    SetWcsDisplayPart();
                }
                else
                {
                    SetWcsWorkComponent(compRefCsys);
                }
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
                        var block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        var bOrigin = blockFeatureBuilderMatch.Origin;
                        var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        var mLength = blockFeatureBuilderMatch.Length.Value;
                        var mWidth = blockFeatureBuilderMatch.Width.Value;
                        var mHeight = blockFeatureBuilderMatch.Height.Value;

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

                        blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        var initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                        var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);
                        _workCompOrigin = setTempCsys.Origin;
                        _workCompOrientation = setTempCsys.Orientation.Element;
                    }
        }

        private void SetWcsWorkComponent(Component compRefCsys)
        {
            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

            var compBase = (BasePart)compRefCsys.Prototype;

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
                        var block1 = (Block)featBlk;

                        BlockFeatureBuilder blockFeatureBuilderMatch;
                        blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                        var bOrigin = blockFeatureBuilderMatch.Origin;
                        var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                        var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                        var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                        var mLength = blockFeatureBuilderMatch.Length.Value;
                        var mWidth = blockFeatureBuilderMatch.Width.Value;
                        var mHeight = blockFeatureBuilderMatch.Height.Value;

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

                        blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                        double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                        double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                        var initMatrix = new double[9];
                        ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                        ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                        ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                        var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                        _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                            setTempCsys.Orientation.Element);

                        var featBlkCsys = _displayPart.WCS.Save();
                        featBlkCsys.SetName("EDITCSYS");
                        featBlkCsys.Layer = 254;

                        NXObject[] addToBody = { featBlkCsys };

                        foreach (var bRefSet in _displayPart.GetAllReferenceSets())
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

                                    var editCsys = (CartesianCoordinateSystem)csysOccurrence;

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
