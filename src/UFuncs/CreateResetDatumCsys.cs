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
using static TSG_Library.Extensions.__Extensions_;
using Selection = TSG_Library.Ui.Selection;
using View = NXOpen.View;

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

                if (makeWp is null)
                    return;

                Part wpUnits = (Part)makeWp[0].Prototype;
                BasePart.Units unit = wpUnits.PartUnits;
                BasePart.Units dpUnits = displayPart.PartUnits;

                if (unit != dpUnits)
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

                if (!(makeWp is null))
                {
                    __work_component_ = makeWp[0];
                    UpdateSessionParts();
                }

                CartesianCoordinateSystem cartesianCoordinateSystem1 = displayPart.WCS.Save();
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
                int count = 0;
                int countSuppressed = 0;

                UpdateSessionParts();
                UpdateOriginalParts();

                if (workPart.ComponentAssembly.RootComponent != null)
                    foreach (Component comp in workPart.ComponentAssembly.RootComponent.GetChildren())
                        if (comp.IsSuppressed == false)
                        {
                            if (comp.Prototype == null) continue;
                            Part part = (Part)comp.Prototype;
                            BasePart.Units partUnits = part.PartUnits;
                            BasePart.Units dpUnits = displayPart.PartUnits;

                            if (partUnits != dpUnits) continue;

                            ufsession_.Assem.SetWorkPartContextQuietly(part.Tag, out IntPtr intPtr);

                            UpdateSessionParts();

                            foreach (CoordinateSystem csys in part.CoordinateSystems)
                            {
                                if (csys.Layer != 254) continue;
                                count += 1;

                                ufsession_.Assem.AskOccsOfPart(displayPart.Tag, part.Tag, out Tag[] partOccs);

                                foreach (Tag blankComp in partOccs)
                                {
                                    ufsession_.Obj.AskOwningPart(blankComp, out Tag parentTag);
                                    if (parentTag == displayPart.Tag)
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

                if (count == 0)
                {
                    print_("");
                    print_("There are no valid components in this assembly");
                }

                if (countSuppressed > 0)
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
            DialogResult dialogResult = MessageBox.Show("Are you sure that you want to remove all CTSDATUMCSYS",
                "Verify Remove All", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes) return;
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();

                if (workPart.ComponentAssembly.RootComponent != null)
                {
                    GetChildComponents(workPart.ComponentAssembly.RootComponent);

                    if (children.Count != 0)
                    {
                        oneChild = children.DistinctBy(__c => __c.DisplayName).ToList();

                        ufsession_.Disp.SetDisplay(UFConstants.UF_DISP_SUPPRESS_DISPLAY);

                        foreach (Component comp in oneChild)
                        {
                            Part compProto = (Part)comp.Prototype;

                            ufsession_.Part.SetDisplayPart(compProto.Tag);

                            UpdateSessionParts();

                            // Delete order block on layer 250

                            Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible,
                                "Delete Order Block");

                            foreach (Body orderBlk in displayPart.Bodies)
                                if (orderBlk.Layer == 250)
                                    session_.UpdateManager.AddToDeleteList(orderBlk);

                            session_.UpdateManager.DoUpdate(markId1);

                            // Delete CTS datum csys

                            session_.UpdateManager.ClearErrorList();

                            Session.UndoMarkId markId2 = session_.SetUndoMark(Session.MarkVisibility.Invisible,
                                "Delete CTSDATUMCSYS");

                            foreach (Feature feat in workPart.Features)
                                if (feat.Name == "CTSDATUMCSYS")
                                    session_.UpdateManager.AddToDeleteList(feat);

                            session_.UpdateManager.DoUpdate(markId2);

                            // Delete Plan view

                            bool planExists = false;
                            ModelingViewCollection wpModelingViews = workPart.ModelingViews;
                            foreach (ModelingView mView in wpModelingViews)
                                if (mView.Name == "PLAN")
                                    planExists = true;

                            if (planExists)
                            {
                                Layout layout1 = workPart.Layouts.FindObject("L1");
                                ModelingView modelingView1 = workPart.ModelingViews.WorkView;
                                ModelingView modelingView2 = workPart.ModelingViews.FindObject("TOP");
                                layout1.ReplaceView(modelingView1, modelingView2, true);

                                ModelingView modelingView3 = workPart.ModelingViews.FindObject("PLAN");
                                session_.UpdateManager.AddToDeleteList(modelingView3);

                                Session.UndoMarkId id1 = session_.NewestVisibleUndoMark;

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

                Session.UndoMarkId markId1 =
                    session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete CTSDATUMCSYS");

                bool csysExists = false;
                foreach (Feature feat in workPart.Features)
                {
                    if (feat.Name != "CTSDATUMCSYS") continue;
                    csysExists = true;
                    session_.UpdateManager.AddToDeleteList(feat);
                }

                if (csysExists) session_.UpdateManager.DoUpdate(markId1);

                // Delete Plan view

                bool planExists = false;
                ModelingViewCollection wpModelingViews = workPart.ModelingViews;
                foreach (ModelingView mView in wpModelingViews)
                    if (mView.Name == "PLAN")
                        planExists = true;

                if (planExists)
                {
                    Layout layout1 = workPart.Layouts.FindObject("L1");
                    ModelingView modelingView1 = workPart.ModelingViews.WorkView;
                    ModelingView modelingView2 = workPart.ModelingViews.FindObject("TOP");
                    layout1.ReplaceView(modelingView1, modelingView2, true);

                    ModelingView modelingView3 = workPart.ModelingViews.FindObject("PLAN");
                    session_.UpdateManager.AddToDeleteList(modelingView3);

                    Session.UndoMarkId id1 = session_.NewestVisibleUndoMark;

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
                bool csysExists = false;

                foreach (Feature feat in workPart.Features)
                    if (feat.Name == "CTSDATUMCSYS")
                        csysExists = true;

                if (!csysExists) return;
                {
                    foreach (Feature feat in workPart.Features)
                    {
                        if (feat.Name != "CTSDATUMCSYS") continue;
                        DatumCsys datumCsys = (DatumCsys)feat;

                        ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out Tag smartCsys, out _, out _, out _);

                        double[] wcsOrigin = new double[3];

                        ufsession_.Csys.AskCsysInfo(smartCsys, out Tag matrix, wcsOrigin);

                        double[] matrixValues = new double[9];

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

                        View view1 = workPart.Views.SaveAs(workPart.ModelingViews.WorkView, "PLAN", false, false);

                        Layout layout1 = workPart.Layouts.FindObject("L1");
                        ModelingView modelingView1 = (ModelingView)view1;
                        ModelingView modelingView2 = workPart.ModelingViews.FindObject("TOP");
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

            Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Saved WCS");

            NXObject[] objects1 = new NXObject[1];
            objects1[0] = savedCsys;
            session_.UpdateManager.AddObjectsToDeleteList(objects1);

            session_.UpdateManager.DoUpdate(markId1);
        }

        private static void CreateDatumCsys(CartesianCoordinateSystem savedCsys)
        {
            try
            {
                bool csysExists = false;

                foreach (Feature feat in workPart.Features)
                    if (feat.Name == "CTSDATUMCSYS")
                        csysExists = true;

                if (csysExists == false)
                {
                    DatumCsysBuilder datumCsysBuilder1 = workPart.Features.CreateDatumCsysBuilder(null);

                    BasePart.Units wpUnits = workPart.PartUnits;
                    Unit unit1;
                    Expression expression1;

                    if (wpUnits == BasePart.Units.Inches)
                    {
                        unit1 = workPart.UnitCollection.FindObject("Inch");
                        expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
                    }
                    else
                    {
                        unit1 = workPart.UnitCollection.FindObject("MilliMeter");
                        expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
                    }


                    Scalar scalar1 = workPart.Scalars.CreateScalarExpression(expression1,
                        Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    Expression expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);


                    Scalar scalar2 = workPart.Scalars.CreateScalarExpression(expression2,
                        Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    Expression expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);


                    Scalar scalar3 = workPart.Scalars.CreateScalarExpression(expression3,
                        Scalar.DimensionalityType.Length,
                        SmartObject.UpdateOption.WithinModeling);


                    Offset offset1 = workPart.Offsets.CreateOffsetRectangular(scalar1, scalar2, scalar3,
                        SmartObject.UpdateOption.WithinModeling);

                    Unit unit2 = workPart.UnitCollection.FindObject("Degrees");


                    Expression expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    Scalar scalar4 = workPart.Scalars.CreateScalarExpression(expression4,
                        Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);


                    Expression expression5 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    Scalar scalar5 = workPart.Scalars.CreateScalarExpression(expression5,
                        Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);


                    Expression expression6 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);


                    Scalar scalar6 = workPart.Scalars.CreateScalarExpression(expression6,
                        Scalar.DimensionalityType.Angle,
                        SmartObject.UpdateOption.WithinModeling);

                    Xform xform1 = workPart.Xforms.CreateXform(savedCsys, null, offset1, scalar4,
                        scalar5, scalar6, 0,
                        SmartObject.UpdateOption.WithinModeling, 1.0);

                    CartesianCoordinateSystem cartesianCoordinateSystem2 =
                        workPart.CoordinateSystems.CreateCoordinateSystem(xform1,
                            SmartObject.UpdateOption.WithinModeling);

                    datumCsysBuilder1.Csys = cartesianCoordinateSystem2;

                    datumCsysBuilder1.DisplayScaleFactor = 2.0;

                    NXObject nXObject1 = datumCsysBuilder1.Commit();

                    nXObject1.SetName("CTSDATUMCSYS");

                    datumCsysBuilder1.Destroy();

                    foreach (Point csysPoint in workPart.Points)
                        if (csysPoint.Layer == 254)
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
                foreach (Component child in assembly.GetChildren())
                {
                    if (child.IsSuppressed) continue;
                    bool isValid = child.DisplayName.__IsDetail();

                    if (isValid)
                    {
                        Tag instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);
                        ufsession_.Assem.AskPartNameOfChild(instance, out string partName);
                        int partLoad = ufsession_.Part.IsLoaded(partName);

                        if (partLoad == 1)
                        {
                            children.Add(child);
                            GetChildComponents(child);
                        }
                        else
                        {
                            ufsession_.Part.OpenQuiet(partName, out Tag partOpen, out _);

                            if (partOpen == NXOpen.Tag.Null) continue;
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