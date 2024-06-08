using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.UF;
using static TSG_Library.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    public partial class CreateResetDatumCsys : _UFuncForm
    {
        //===========================================
        //  Class Members
        //===========================================

        private static readonly Session session_ = Session.GetSession();
        private static readonly UI theUI = UI.GetUI();
        private static Part workPart = __work_part_;
        private static Part displayPart = __display_part_;
        private static Part originalWorkPart = workPart;
        private static Part originalDisplayPart = displayPart;
        private static List<Component> makeWp = new List<Component>();
        private static readonly List<Component> children = new List<Component>();
        private static List<Component> oneChild = new List<Component>();


        public CreateResetDatumCsys()
        {
            InitializeComponent();
        }

        private void ButtonSingle_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonCreateWcsPlanView.Enabled = true;
                buttonNonDatumCsys.Enabled = false;
                buttonResetAll.Enabled = false;
                makeWp = Selection.SelectManyComponents().ToList();

                if(makeWp is null)
                    return;

                var wpUnits = (Part)makeWp[0].Prototype;
                var unit = wpUnits.PartUnits;
                var dpUnits = displayPart.PartUnits;

                if(unit != dpUnits)
                    return;

                __work_component_ = makeWp[0];
                UpdateSessionParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonCreateWcsPlanView_Click(object sender, EventArgs e)
        {
            buttonResetAll.Enabled = true;
            buttonNonDatumCsys.Enabled = true;

            try
            {
                ResetComponent();

                if(!(makeWp is null))
                {
                    __work_component_ = makeWp[0];
                    UpdateSessionParts();
                }

                var cartesianCoordinateSystem1 = displayPart.WCS.Save();
                using (session_.__UsingSuppressDisplay())
                {
                    ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_SUPPRESS_DISPLAY);

                    ufsession_.Part.SetDisplayPart(NXOpen.Tag.Null);

                    UpdateSessionParts();

                    workPart.Layers.WorkLayer = 254;

                    CreateDatumCsys(cartesianCoordinateSystem1);

                    DeleteSavedCsys(cartesianCoordinateSystem1);

                    workPart.Layers.WorkLayer = 1;

                    workPart.Layers.SetState(254, State.Hidden, false);

                    MakePlanView();

                    ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);

                    ufsession_.Assem.SetWorkPart(originalWorkPart.Tag);

                    UpdateSessionParts();

                    UpdateOriginalParts();

                    ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                buttonSingle.Enabled = true;
                buttonCreateWcsPlanView.Enabled = false;
            }
        }

        private void ButtonNonDatumCsys_Click(object sender, EventArgs e)
        {
            try
            {
                var count = 0;
                var countSuppressed = 0;

                UpdateSessionParts();
                UpdateOriginalParts();

                if(workPart.ComponentAssembly.RootComponent != null)
                    foreach (var comp in workPart.ComponentAssembly.RootComponent.GetChildren())
                        if(comp.IsSuppressed == false)
                        {
                            if(comp.Prototype == null) continue;
                            var part = (Part)comp.Prototype;
                            var partUnits = part.PartUnits;
                            var dpUnits = displayPart.PartUnits;

                            if(partUnits != dpUnits) continue;

                            ufsession_.Assem.SetWorkPartContextQuietly(part.Tag, out var intPtr);

                            UpdateSessionParts();

                            foreach (CoordinateSystem csys in part.CoordinateSystems)
                            {
                                if(csys.Layer != 254) continue;
                                count += 1;

                                ufsession_.Assem.AskOccsOfPart(displayPart.Tag, part.Tag, out var partOccs);

                                foreach (var blankComp in partOccs)
                                {
                                    ufsession_.Obj.AskOwningPart(blankComp, out var parentTag);
                                    if(parentTag == displayPart.Tag)
                                        ufsession_.Obj.SetBlankStatus(blankComp, UFConstants.UF_OBJ_BLANKED);
                                }
                            }

                            ufsession_.Assem.RestoreWorkPartContextQuietly(ref intPtr);

                            UpdateSessionParts();
                        }
                        else
                        {
                            countSuppressed += 1;
                        }

                if(count == 0)
                {
                    print_("");
                    print_("There are no valid components in this assembly");
                }

                if(countSuppressed > 0)
                {
                    print_("");
                    print_("There are suppressed components in this assembly");
                    print_("");
                    print_("CTSDATUMCSYS will not be verified in these components");
                }

                displayPart.Views.UpdateDisplay();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                displayPart.Views.UpdateDisplay();
            }
        }

        private void ButtonResetAll_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure that you want to remove all CTSDATUMCSYS",
                "Verify Remove All", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if(dialogResult != DialogResult.Yes) return;
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                if(workPart.ComponentAssembly.RootComponent != null)
                {
                    GetChildComponents(workPart.ComponentAssembly.RootComponent);

                    if(children.Count != 0)
                    {
                        oneChild = children.DistinctBy(__c => __c.DisplayName).ToList();

                        ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_SUPPRESS_DISPLAY);

                        foreach (var comp in oneChild)
                        {
                            var compProto = (Part)comp.Prototype;

                            ufsession_.Part.SetDisplayPart(compProto.Tag);

                            UpdateSessionParts();

                            // Delete order block on layer 250

                            var markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Order Block");

                            foreach (Body orderBlk in displayPart.Bodies)
                                if(orderBlk.Layer == 250)
                                    session_.UpdateManager.AddToDeleteList(orderBlk);

                            session_.UpdateManager.DoUpdate(markId1);

                            // Delete CTS datum csys

                            session_.UpdateManager.ClearErrorList();

                            var markId2 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete CTSDATUMCSYS");

                            foreach (Feature feat in workPart.Features)
                                if(feat.Name == "CTSDATUMCSYS")
                                    session_.UpdateManager.AddToDeleteList(feat);

                            session_.UpdateManager.DoUpdate(markId2);

                            // Delete Plan view

                            var planExists = false;
                            var wpModelingViews = workPart.ModelingViews;
                            foreach (ModelingView mView in wpModelingViews)
                                if(mView.Name == "PLAN")
                                    planExists = true;

                            if(planExists)
                            {
                                var layout1 = workPart.Layouts.FindObject("L1");
                                var modelingView1 = workPart.ModelingViews.WorkView;
                                var modelingView2 = workPart.ModelingViews.FindObject("TOP");
                                layout1.ReplaceView(modelingView1, modelingView2, true);

                                var modelingView3 = workPart.ModelingViews.FindObject("PLAN");
                                session_.UpdateManager.AddToDeleteList(modelingView3);

                                var id1 = session_.NewestVisibleUndoMark;

                                session_.UpdateManager.DoUpdate(id1);

                                ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);

                                ufsession_.Assem.SetWorkPart(originalDisplayPart.Tag);

                                UpdateSessionParts();
                            }
                            else
                            {
                                ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);

                                ufsession_.Assem.SetWorkPart(originalDisplayPart.Tag);

                                UpdateSessionParts();

                                UpdateOriginalParts();
                            }
                        }

                        children.Clear();
                        oneChild.Clear();

                        ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);
                        ufsession_.Assem.SetWorkPart(originalDisplayPart.Tag);
                        UpdateSessionParts();
                        UpdateOriginalParts();

                        ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                        displayPart.Views.Regenerate();
                    }
                    else
                    {
                        print_("");
                        print_("No valid child components in " + workPart.ComponentAssembly.RootComponent.DisplayName);
                    }
                }
                else
                {
                    print_("");
                    print_("The work part is not an assembly");
                    print_("Reset All Components is not available");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();

                ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);
                ufsession_.Assem.SetWorkPart(originalDisplayPart.Tag);
                UpdateSessionParts();
                UpdateOriginalParts();
                ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                displayPart.Views.Regenerate();
            }
        }


        private static void ResetComponent()
        {
            try
            {
                ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_SUPPRESS_DISPLAY);

                ufsession_.Part.SetDisplayPart(NXOpen.Tag.Null);

                UpdateSessionParts();

                // Delete CTS datum csys

                session_.UpdateManager.ClearErrorList();

                var markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete CTSDATUMCSYS");

                var csysExists = false;
                foreach (Feature feat in workPart.Features)
                {
                    if(feat.Name != "CTSDATUMCSYS") continue;
                    csysExists = true;
                    session_.UpdateManager.AddToDeleteList(feat);
                }

                if(csysExists) session_.UpdateManager.DoUpdate(markId1);

                // Delete Plan view

                var planExists = false;
                var wpModelingViews = workPart.ModelingViews;
                foreach (ModelingView mView in wpModelingViews)
                    if(mView.Name == "PLAN")
                        planExists = true;

                if(planExists)
                {
                    var layout1 = workPart.Layouts.FindObject("L1");
                    var modelingView1 = workPart.ModelingViews.WorkView;
                    var modelingView2 = workPart.ModelingViews.FindObject("TOP");
                    layout1.ReplaceView(modelingView1, modelingView2, true);

                    var modelingView3 = workPart.ModelingViews.FindObject("PLAN");
                    session_.UpdateManager.AddToDeleteList(modelingView3);

                    var id1 = session_.NewestVisibleUndoMark;

                    session_.UpdateManager.DoUpdate(id1);
                }

                ufsession_.Part.SetDisplayPart(originalDisplayPart.Tag);
                ufsession_.Assem.SetWorkPart(originalWorkPart.Tag);
                UpdateSessionParts();
                UpdateOriginalParts();

                ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_UNSUPPRESS_DISPLAY);
                displayPart.Views.Regenerate();
            }
        }

        private static void UpdateSessionParts()
        {
            displayPart = session_.Parts.Display;
            workPart = session_.Parts.Work;
        }

        private static void UpdateOriginalParts()
        {
            originalDisplayPart = displayPart;
            originalWorkPart = workPart;
        }

        private void MakePlanView()
        {
            // Get the datum csys information from CTSDATUMCSYS

            try
            {
                var csysExists = false;

                foreach (Feature feat in workPart.Features)
                    if(feat.Name == "CTSDATUMCSYS")
                        csysExists = true;

                if(!csysExists) return;
                {
                    foreach (Feature feat in workPart.Features)
                    {
                        if(feat.Name != "CTSDATUMCSYS") continue;
                        var datumCsys = (DatumCsys)feat;

                        ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out var smartCsys, out _, out _, out _);

                        var wcsOrigin = new double[3];

                        ufsession_.Csys.AskCsysInfo(smartCsys, out var matrix, wcsOrigin);

                        var matrixValues = new double[9];

                        ufsession_.Csys.AskMatrixValues(matrix, matrixValues);

                        Matrix3x3 orientation;
                        orientation.Xx = matrixValues[0];
                        orientation.Xy = matrixValues[1];
                        orientation.Xz = matrixValues[2];
                        orientation.Yx = matrixValues[3];
                        orientation.Yy = matrixValues[4];
                        orientation.Yz = matrixValues[5];
                        orientation.Zx = matrixValues[6];
                        orientation.Zy = matrixValues[7];
                        orientation.Zz = matrixValues[8];

                        displayPart.Views.WorkView.Orient(orientation);

                        var view1 = workPart.Views.SaveAs(workPart.ModelingViews.WorkView, "PLAN", false, false);

                        var layout1 = workPart.Layouts.FindObject("L1");
                        var modelingView1 = (ModelingView)view1;
                        var modelingView2 = workPart.ModelingViews.FindObject("TOP");
                        layout1.ReplaceView(modelingView1, modelingView2, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void DeleteSavedCsys(CartesianCoordinateSystem savedCsys)
        {
            session_.UpdateManager.ClearErrorList();

            var markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Saved WCS");

            var objects1 = new NXObject[1];
            var cartesianCoordinateSystem1 = savedCsys;
            objects1[0] = cartesianCoordinateSystem1;
            session_.UpdateManager.AddObjectsToDeleteList(objects1);

            session_.UpdateManager.DoUpdate(markId1);
        }

        private static void CreateDatumCsys(CartesianCoordinateSystem savedCsys)
        {
            try
            {
                var csysExists = false;

                foreach (Feature feat in workPart.Features)
                    if(feat.Name == "CTSDATUMCSYS")
                        csysExists = true;

                if(csysExists == false)
                {
                    var datumCsysBuilder1 = workPart.Features.CreateDatumCsysBuilder(null);

                    var wpUnits = workPart.PartUnits;
                    Unit unit1;
                    Expression expression1;

                    if(wpUnits == BasePart.Units.Inches)
                    {
                        unit1 = workPart.UnitCollection.FindObject("Inch");
                        expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
                    }
                    else
                    {
                        unit1 = workPart.UnitCollection.FindObject("MilliMeter");
                        expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
                    }


                    var scalar1 = workPart.Scalars.CreateScalarExpression(expression1, Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    var expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);


                    var scalar2 = workPart.Scalars.CreateScalarExpression(expression2, Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    var expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);


                    var scalar3 = workPart.Scalars.CreateScalarExpression(expression3, Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    var offset1 = workPart.Offsets.CreateOffsetRectangular(scalar1, scalar2, scalar3,
                        SmartObject.UpdateOption.WithinModeling);

                    var unit2 = workPart.UnitCollection.FindObject("Degrees");


                    var expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    var scalar4 = workPart.Scalars.CreateScalarExpression(expression4, Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);


                    var expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    var scalar5 = workPart.Scalars.CreateScalarExpression(expression5, Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);


                    var expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    var scalar6 = workPart.Scalars.CreateScalarExpression(expression6, Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);

                    var cartesianCoordinateSystem1 = savedCsys;

                    var xform1 = workPart.Xforms.CreateXform(cartesianCoordinateSystem1, null, offset1, scalar4,
                        scalar5, scalar6, 0,
                        SmartObject.UpdateOption.WithinModeling, 1.0);

                    var cartesianCoordinateSystem2 =
                        workPart.CoordinateSystems.CreateCoordinateSystem(xform1,
                            SmartObject.UpdateOption.WithinModeling);

                    datumCsysBuilder1.Csys = cartesianCoordinateSystem2;

                    datumCsysBuilder1.DisplayScaleFactor = 2.0;

                    var nXObject1 = datumCsysBuilder1.Commit();

                    nXObject1.SetName("CTSDATUMCSYS");

                    datumCsysBuilder1.Destroy();

                    foreach (Point csysPoint in workPart.Points)
                        if(csysPoint.Layer == 254)
                            csysPoint.Layer = 2;
                }
                else
                {
                    print_("");
                    print_("Error:");
                    print_("A CTSDATUMCSYS already exists");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void GetChildComponents(Component assembly)
        {
            try
            {
                foreach (var child in assembly.GetChildren())
                {
                    if(child.IsSuppressed) continue;
                    var isValid = child.DisplayName.__IsDetail();

                    if(isValid)
                    {
                        var instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);
                        ufsession_.Assem.AskPartNameOfChild(instance, out var partName);
                        var partLoad = ufsession_.Part.IsLoaded(partName);

                        if(partLoad == 1)
                        {
                            children.Add(child);
                            GetChildComponents(child);
                        }
                        else
                        {
                            ufsession_.Part.OpenQuiet(partName, out var partOpen, out _);

                            if(partOpen == NXOpen.Tag.Null) continue;
                            children.Add(child);
                            GetChildComponents(child);
                        }
                    }
                    else
                    {
                        GetChildComponents(child);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }
    }
}