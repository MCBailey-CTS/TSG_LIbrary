﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.Drafting;
using NXOpen.Drawings;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.Preferences;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Properties;
using TSG_Library.Ui;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.UFuncs.AssemblyAutoDetailForm.CtsDimensionData.EndPointAssociativity;
using DecimalPointCharacter = NXOpen.Annotations.DecimalPointCharacter;
using Selection = TSG_Library.Ui.Selection;
using View = NXOpen.View;

namespace TSG_Library.UFuncs
{
    //[RevisionEntry("1.1", "2015", "05", "18")]
    //[Revision("1.1.1",
    //    "Added filter for Scrap Chutes to have 4 view created.  Description attribute has to be set to \"CHUTE TO SUIT\".")]
    //[RevisionEntry("1.2", "2016", "11", "17")]
    //[Revision("1.2.1", "Changed Line 1142 from \"TSG STANDARD\" to \"STANDARD\" - to work with the GE Border file.")]
    //[RevisionEntry("1.3", "2017", "06", "05")]
    //[Revision("1.3.1", "Fixed issue with hole charting coming in awkwardly.")]
    //[RevisionEntry("1.4", "2017", "06", "15")]
    //[Revision("1.4.1", "Now cuts the display and still able to have hole charts come in at the proper orientation.")]
    //[RevisionEntry("1.45", "2017", "08", "02")]
    //[Revision("1.45.1", "Changed criteria for HOLECHARTS.")]
    //[Revision("1.45.1.1",
    //    "It now will accept any input the contains the word HOLECHART instead of equaling HOLECHART.")]
    //[Revision("1.45.1.2", "Performs ToUpper() before check.")]
    //[RevisionEntry("1.50", "2017", "08", "22")]
    //[Revision("1.50.1", "Signed so it can run outside of CTS.")]
    //[RevisionEntry("1.60", "2017", "09", "08")]
    //[Revision("1.60.1", "Added validation check")]
    //[RevisionEntry("1.61", "2017", "12", "07")]
    //[Revision("1.61.1", "Added a message to the portion of code that creates the detail sheets.")]
    //[Revision("1.61.1.1",
    //    "If any exception is thrown during this process the user will be informed that some sort of error occurred during that component detail creation. They should then check to see if they can determine what happened the.")]
    //[RevisionEntry("1.62", "2017", "12", "13")]
    //[Revision("1.62.1", "CIT 2017-0049")]
    //[Revision("1.62.2", "Edited the functionality of the “Delete-4-Views” checkbox.")]
    //[Revision("1.62.3",
    //    "It now will delete all “Notes” on layer 230 in the selected parts. Effectively deleting the hole charts.")]
    //[RevisionEntry("1.7", "2018", "02", "09")]
    //[Revision("1.7.1", "Set the option to extract edges upon the creation of drawing sheet to none.")]
    //[Revision("1.7.2",
    //    "Edited the functionality of updating the drawing sheets. It will now skip drawing sheets in parts that are not out of date.")]
    //[RevisionEntry("1.8", "2018", "11", "26")]
    //[Revision("1.8.1", "Put a try catch surrounding the process that goes out and finds details to actual process.")]
    //[Revision("1.8.2", "Removed, minimize, maximize, and cancel buttons.")]
    //[RevisionEntry("1.81", "2018", "11", "29")]
    //[Revision("1.81.1", "Fixed bug that occurred when the “Select” button was pressed and caused an exception.")]
    //[RevisionEntry("1.82", "2020", "09", "03")]
    //[Revision("1.82.1", "Fixed issue where the maximum shoe op level was 305.")]
    //[RevisionEntry("1.9", "2021", "05", "27")]
    //[Revision("1.9.1", "The ConceptControlFile now points to \"U:\\nxFiles\\UfuncFiles\\ConceptControlFile.ucf\"")]
    //[RevisionEntry("2.0", "2021", "11", "17")]
    //[Revision("2.0.1", "Added 'Shaded Views' checkbox to form.")]
    //[Revision("2.0.2", "When checked 'Shaded Views' will now produce a 4-View with the block shaded and colored.")]
    //[RevisionEntry("3.0", "2022", "01", "11")]
    //[Revision("3.0.1", "Added drill chart check box to form.")]
    //[Revision("3.0.2", "If checked, then the program will add a drill chart to the 4-View")]
    //[RevisionEntry("3.1", "2022", "10", "05")]
    //[Revision("3.1.1",
    //    "Details that have a DESCRIPTION attribute value of 'Diemaker To Alter' will now create a 4-VIEW.")]
    //[Revision("3.1.2", "Bodies on Layer 111 will now show up in the 4-VIEW and will be included with the dimensions.")]
    //[RevisionEntry("3.2", "2022", "10", "10")]
    //[Revision("3.2.1",
    //    "Details now will move all non solid body on layers 1 and 111 to layer 169 so they won't show up in the 4-View")]
    //[Revision("3.2.2", "Before 4-VIEW is created, the program runs the same MakePlanView method from AddFasteners.")]
    //[RevisionEntry("3.3", "2022", "11", "02")]
    //[Revision("3.3.1", "Fixed bug that resulted in non Dynamic Block parts not getting a 4-View.")]
    //[RevisionEntry("3.4", "2022", "12", "01")]
    //[Revision("3.4.1", "Fixed bug where details were getting changed to shaded without edges.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    [UFunc(ufunc_assembly_auto_detail)]
    public partial class AssemblyAutoDetailForm : _UFuncForm
    {
        //private static NXOpen.Part _workPart = session_.Parts.Work;
        //private static NXOpen.Part _originalDisplayPart = __display_part_;
        private static readonly List<Component> _allComponents = new List<Component>();
        private static readonly IDictionary<string, List<string>> _holeChart = new Dictionary<string, List<string>>();
        private static string _borderFile;
        private static string _sizeFile;

        public AssemblyAutoDetailForm()
        {
            InitializeComponent();
            Location = Settings.Default.assembly_auto_detail_form_window_location;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = AssemblyFileVersion;
            _sizeFile = PerformStreamReaderString(FilePathUcf, ":HOLESIZE_FILE_LOCATION:",
                ":END_HOLESIZE_FILE_LOCATION:");
            _borderFile =
                PerformStreamReaderString(FilePathUcf, ":BORDER_FILE_LOCATION:", ":END_BORDER_FILE_LOCATION:");

            string[] lines = File.ReadAllLines(_sizeFile)
                .Where(__s => !__s.StartsWith("--"))
                .Where(__s => !string.IsNullOrEmpty(__s))
                .Where(__s => !string.IsNullOrWhiteSpace(__s))
                .ToArray();

            for (int i = 0; i < lines.Length - 1; i += 2)
                _holeChart[lines[i + 1]] = lines[i].Split(',').ToList();

            SetFormDefaults();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.assembly_auto_detail_form_window_location = Location;
            Settings.Default.Save();
        }

        private void ButtonSelectBorderFile_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = "G:\\0Library\\Drafting";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            _borderFile = dialogResult == DialogResult.OK
                ? openFileDialog.FileName
                : PerformStreamReaderString(FilePathUcf, ":BORDER_FILE_LOCATION:", ":END_BORDER_FILE_LOCATION:");
        }

        private void CheckBox_Clicked(object sender, EventArgs e)
        {
            bool flag = chkDetailSheet.Checked || chkHoleChart.Checked || chkUpdateViews.Checked ||
                        chkDrillChart.Checked;
            btnSelect.Enabled = flag;
            btnSelectAll.Enabled = flag;
        }

        private void CheckBoxDelete4Views_CheckedChanged(object sender, EventArgs e)
        {
            chkHoleChart.Checked = false;
            chkUpdateViews.Checked = false;

            if (chkDelete4Views.Checked)
                chkDetailSheet.Checked = false;

            chkDetailSheet.Enabled = !chkDelete4Views.Checked;
            chkHoleChart.Enabled = !chkDelete4Views.Checked;
            chkUpdateViews.Enabled = !chkDelete4Views.Checked;
            btnSelect.Enabled = chkDelete4Views.Checked;
            btnSelectAll.Enabled = chkDelete4Views.Checked;
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    _allComponents.Clear();

                    Component[] selectComponents = Selection.SelectManyComponents();

                    if (selectComponents.Length == 0)
                        return;

                    _allComponents.AddRange(selectComponents);

                    List<Component> trimmedComponents = _allComponents.Where(comp => !comp.IsSuppressed)
                        .Distinct(new EqualityDisplayName())
                        .ToList();

                    trimmedComponents.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));

                    DetailComponents(trimmedComponents);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void ButtonSelectAll_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    if (__display_part_.ComponentAssembly.RootComponent is null)
                        return;

                    _allComponents.Clear();
                    GetChildComponents(__display_part_.ComponentAssembly.RootComponent);

                    if (_allComponents.Count == 0)
                    {
                        print_("No valid components");
                        return;
                    }

                    List<CtsAttributes> materials = CreateMaterialList();
                    List<Component> passedComponents = new List<Component>();

                    foreach (Component comp in _allComponents)
                        try
                        {
                            foreach (NXObject.AttributeInformation attr in comp.GetUserAttributes())
                            {
                                if (attr.Title.ToUpper() == "MATERIAL")
                                {
                                    string value = comp.GetUserAttributeAsString("MATERIAL",
                                        NXObject.AttributeType.String,
                                        -1);

                                    if (value != "")
                                        passedComponents.AddRange(from matAttr in materials
                                                                  where matAttr.AttrValue == value
                                                                  select comp);
                                    else
                                        print_($"\n{comp.DisplayName} : MATERIAL attribute equals nothing");
                                }

                                // Rev1.1 Change - Added for Scrap Chutes

                                if (attr.Title.ToUpper() != "DESCRIPTION")
                                    continue;

                                string dValue = comp.GetUserAttributeAsString("DESCRIPTION",
                                    NXObject.AttributeType.String,
                                    -1);

                                if (dValue is null)
                                {
                                    print_("////////////////////////////////////");
                                    print_($"Component: {comp.DisplayName} has a null DESCRIPTION value.");
                                    print_(comp._AssemblyPathString());
                                    print_("////////////////////////////////////");
                                    continue;
                                }

                                if (dValue.ToUpper() == "CHUTE TO SUIT")
                                    passedComponents.Add(comp);

                                if (dValue.ToLower().Contains("diemaker to alter"))
                                    passedComponents.Add(comp);
                                // End of Rev1.1 change                             
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException(comp.DisplayName);
                        }

                    if (passedComponents.Count == 0)
                    {
                        print_("No valid components");
                        return;
                    }

                    Component[] selectDeselectComps = passedComponents.ToArray();
                    passedComponents = Preselect.GetUserSelections(selectDeselectComps);

                    if (passedComponents.Count == 0)
                    {
                        print_("No valid components");
                        return;
                    }

                    passedComponents = passedComponents.Where(comp => !comp.IsSuppressed)
                        .Distinct(new EqualityDisplayName())
                        .ToList();

                    passedComponents.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));
                    DetailComponents(passedComponents);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void DetailComponents(IEnumerable<Component> selectedComponents)
        {
            using (session_.__UsingDisplayPartReset())
            {
                Component[] componentArray = selectedComponents.ToArray();

                for (int index = 0; index < componentArray.Length; index++)
                {
                    Component detailComp = componentArray[index];

                    try
                    {
                        prompt_($"({index + 1} of {componentArray.Length})");
                        DetailComponent(detailComp);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException(detailComp.DisplayName);
                    }
                }

                SetFormDefaults();
            }
        }

        public static void SetWcsToWorkPart()
        {
            Block dynamic = __work_part_.__DynamicBlock();

            if (__work_part_.Tag == __display_part_.Tag)
            {
                __display_part_.WCS.SetOriginAndMatrix(dynamic.__Origin(), dynamic.__Orientation());
                return;
            }

            using (__work_component_.__UsingReferenceSetReset())
            {
                __work_component_.__ReferenceSet("Entire Part");

                CartesianCoordinateSystem system = __work_part_.CoordinateSystems.CreateCoordinateSystem(
                    _Point3dOrigin,
                    dynamic.__Orientation(),
                    false);

                NXObject ject = __work_component_.FindOccurrence(system);

                if (!(ject is CartesianCoordinateSystem cart))
                    throw new Exception();

                __display_part_.WCS.SetOriginAndMatrix(_Point3dOrigin, cart.Orientation.Element);
                ufsession_.Obj.DeleteObject(system.Tag);
            }
        }

        public static ModelingView GetPlanView()
        {
            ModelingView planView = null;

            foreach (ModelingView view in __work_part_.ModelingViews)
                if (view.Name == ViewPlan)
                {
                    planView = view;
                    break;
                }

            return planView;
        }

        public static void MakePlanView(CartesianCoordinateSystem csys)
        {
            const string l1 = "L1";
            const string top = "Top";
            const string plan = "PLAN";
            __display_part_ = __work_part_;
            ModelingView planView = GetPlanView();
            ModelingView modelingView1, modelingView2;
            Layout layout;

            if (planView != null)
            {
                layout = __work_part_.Layouts.FindObject(l1);
                modelingView1 = __work_part_.ModelingViews.WorkView;
                modelingView2 = __work_part_.ModelingViews.FindObject(top);
                layout.ReplaceView(modelingView1, modelingView2, true);
                ModelingView tempView = __work_part_.ModelingViews.FindObject(plan);
                ufsession_.Obj.DeleteName(tempView.Tag);
            }

            ufsession_.View.SetViewMatrix("", 3, csys.Tag, null);
            View newView = __display_part_.Views.SaveAs(__display_part_.ModelingViews.WorkView, plan, false, false);
            layout = __display_part_.Layouts.FindObject(l1);
            modelingView1 = (ModelingView)newView;
            modelingView2 = __display_part_.ModelingViews.FindObject(top);
            layout.ReplaceView(modelingView1, modelingView2, true);
            session_.__DeleteObjects(csys);
        }

        private void DetailComponent(Component detailComp)
        {
            print_("////////////////////////");

            if (!detailComp.__IsLoaded())
            {
                print_($"Component {detailComp.DisplayName} is not loaded");
                print_("4-VIEW will not be created for this component");

                return;
            }

            using (session_.__UsingSuppressDisplay())
            {
                __display_part_ = detailComp.__Prototype();

                try
                {
                    if (__work_part_.__HasDynamicBlock())
                        SetWcsToWorkPart();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                try
                {
                    MakePlanView(__display_part_.WCS.Save());
                }
                catch (NXException ex) when (ex.ErrorCode == 925019)
                {
                    print_($"Could not replace view in {__work_part_.Leaf}");
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                if (__display_part_.Layers.GetState(111) != State.WorkLayer)
                    __display_part_.Layers.SetState(111, State.Selectable);

                using (new ResetShadeRendering())
                {
                    PreferencesBuilder preferencesBuilder1 = __work_part_.SettingsManager.CreatePreferencesBuilder();

                    using (new Destroyer(preferencesBuilder1))
                    {
                        if (chkColorDetailSheet.Checked)
                        {
                            preferencesBuilder1.ViewStyle.ViewStyleShading.RenderingStyle = ShadingRenderingStyleOption.FullyShaded;
                            preferencesBuilder1.Commit();
                        }
                    }

                    ufsession_.Draw.SetDisplayState(1);
                    __display_part_.ModelingViews.WorkView.RenderingStyle = View.RenderingStyleType.ShadedWithEdges;

                    foreach (ReferenceSet dispRefSet in __display_part_.GetAllReferenceSets())
                    {
                        if (dispRefSet.Name != "BODY")
                            continue;

                        foreach (NXObject refMember in dispRefSet.AskMembersInReferenceSet())
                        {
                            if (!(refMember is Arc || refMember is Line || refMember is Ellipse || refMember is Spline))
                                continue;

                            Curve lineLayer = (Curve)refMember;

                            if (lineLayer.Layer != 1)
                                continue;

                            NXObject[] remMember = { refMember };
                            dispRefSet.RemoveObjectsFromReferenceSet(remMember);
                            lineLayer.Layer = 10;
                        }
                    }

                    if (chkHoleChart.Checked)
                        try
                        {
                            // set WCS to "PLAN" view and set "Top" as the work view
                            SetCsysToPlanView();
                            SetHoleChartLayers();
                            // get work view and part units and set lettering preferences
                            View workView = __display_part_.Views.WorkView;
                            SetLetteringPreferences(__display_part_.PartUnits);

                            List<NXObject> deleteNote = __display_part_.Notes
                                .Cast<Note>()
                                .Where(holeNote => holeNote.Layer == 230)
                                .Cast<NXObject>()
                                .ToList();

                            if (deleteNote.Count > 0)
                                session_.__DeleteObjects(deleteNote.ToArray());

                            foreach (Body solidBody in __display_part_.Bodies)
                            {
                                solidBody.Unblank();

                                if (solidBody.Layer != 1 && solidBody.Layer != 94 && solidBody.Layer != 96)
                                    continue;

                                foreach (Face cylFace in solidBody.GetFaces())
                                {
                                    const int cylinder = 16;
                                    double[] point = new double[3];
                                    double[] dir = new double[3];
                                    double[] box = new double[6];
                                    ufsession_.Modl.AskFaceData(cylFace.Tag, out int type, point, dir, box,
                                        out double radius, out _, out _);

                                    if (type != cylinder)
                                        continue;

                                    // Revision 1.45
                                    if (!cylFace.Name.ToUpper().Contains("HOLECHART"))
                                        continue;

                                    Point3d cfOrigin = new Point3d(point[0], point[1], point[2]);
                                    string[] cfNote = new string[1];
                                    double multiplier = __display_part_.PartUnits == BasePart.Units.Inches ? 1.0 : 25.4;

                                    foreach (string key in _holeChart.Keys)
                                        foreach (string value in _holeChart[key])
                                        {
                                            double holeDiameter = Convert.ToDouble(value);
                                            double faceDiameter = Convert.ToDouble(radius * 2);
                                            double difference = holeDiameter * .000000001;
                                            if (System.Math.Abs(holeDiameter * multiplier - faceDiameter) <=
                                                difference * multiplier)
                                            {
                                                cfNote[0] = key;
                                                break;
                                            }
                                        }

                                    CreateHoleChartNote(cfOrigin, cfNote, workView.Tag);
                                }
                            }

                            SetDefaultLayers();
                            print_($"HoleCharted {__display_part_.Leaf}");
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }

                    if (chkDetailSheet.Checked)
                        try
                        {

                            List<DisplayableObject> objectsToMove = __display_part_.Layers.GetAllObjectsOnLayer(1)
                                .OfType<DisplayableObject>()
                                .ToList();

                            __display_part_.Layers.MoveDisplayableObjects(169, objectsToMove.ToArray());
                            SetHoleChartLayers();
                            SetCsysToPlanView();

                            // delete existing 4-VIEW
                            List<NXObject> deleteView = __display_part_.DrawingSheets
                                .Cast<DrawingSheet>()
                                .Where(dwg => dwg.Name == "4-VIEW")
                                .Cast<NXObject>()
                                .ToList();

                            if (deleteView.Count > 0)
                                session_.__DeleteObjects(deleteView.ToArray());

                            string[] drillChart = null;

                            if (chkDrillChart.Checked)
                            {
#pragma warning disable CS0612 // Type or member is obsolete
                                drillChart = DrillChart.Main();
#pragma warning restore CS0612 // Type or member is obsolete
                                print_($"Drill Charted in {__display_part_.Leaf}");
                            }

                            CreateDetailSheet(drillChart);
                            SetDefaultLayers();
                            print_($"Created 4-View in {__display_part_.Leaf}");
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }

                    if (chkUpdateViews.Checked)
                        try
                        {
                            // update all views
                            SetDefaultLayers();

                            foreach (DrawingSheet dwg in __display_part_.DrawingSheets)
                            {
                                // Revision 1.7 – 2018 / 02 / 09
                                ufsession_.Draw.IsObjectOutOfDate(dwg.Tag, out bool outOfDate);

                                if (!outOfDate)
                                    continue;

                                dwg.Open();

                                foreach (DraftingView drfView in dwg.GetDraftingViews())
                                {
                                    drfView.Update();
                                    print_($"Updated view {drfView.Name} in {__display_part_.Leaf}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException();
                        }

                    if (!chkDelete4Views.Checked)
                        return;
                    // Revision • 1.62 – 2017 / 12 / 13
                    try
                    {
                        __display_part_.Notes
                            .ToArray()
                            .Where(note => note.Layer == 230)
                            .ToList()
                            .ForEach(__n => ufsession_.Obj.DeleteObject(__n.Tag));
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                    // delete existing 4-VIEW
                    List<NXObject> deleteView1 = __display_part_.DrawingSheets.Cast<DrawingSheet>()
                        .Where(dwg => dwg.Name == "4-VIEW")
                        .Cast<NXObject>()
                        .ToList();

                    if (deleteView1.Count > 0)
                        session_.__DeleteObjects(deleteView1.ToArray());
                }
            }
        }

        private static DraftingView ImportPlanView(double xPlacement, double yPlacement)
        {
            BaseViewBuilder baseViewBuilder = __work_part_.DraftingViews.CreateBaseViewBuilder(null);

            using (new Destroyer(baseViewBuilder))
            {
                ModelingView planView = null;
                ModelingView topView = null;
                bool isPlan = false;
                foreach (ModelingView mView in __work_part_.ModelingViews)
                    switch (mView.Name)
                    {
                        case "PLAN":
                            isPlan = true;
                            planView = mView;
                            break;
                        case "Top":
                            topView = mView;
                            break;
                    }

                baseViewBuilder.SelectModelView.SelectedView = isPlan ? planView : topView;
                baseViewBuilder.Style.ViewStyleBase.Part = __work_part_;
                baseViewBuilder.Style.ViewStyleBase.PartName = __work_part_.FullPath;
                PartLoadStatus loadStatus = __work_part_.LoadFully();
                loadStatus.Dispose();
                baseViewBuilder.Style.ViewStyleGeneral.UVGrid = false;
                baseViewBuilder.Style.ViewStyleGeneral.AutomaticAnchorPoint = false;
                baseViewBuilder.Style.ViewStyleGeneral.Centerlines = false;
                Point3d point = new Point3d(xPlacement, yPlacement, 0.0);
                baseViewBuilder.Placement.Placement.SetValue(null, __work_part_.Views.WorkView, point);
                NXObject nXObject1 = baseViewBuilder.Commit();
                DraftingView nPlanView = (DraftingView)nXObject1;
                return nPlanView;
            }
        }

        private static void AssignDraftObject(DraftingView drfView, CtsDimensionData dataToUse,
            ref UFDrf.Object dataToAssign)
        {
            dataToAssign.object_tag = dataToUse.DimEntity;
            dataToAssign.object_view_tag = drfView.Tag;
            dataToAssign.object_assoc_modifier = dataToUse.ExtPointId;
        }

        private static void DimensionView(DraftingView drfView, IReadOnlyList<double> drfViewPosition, double viewScale,
            IReadOnlyList<double> size, BasePart.Units partUnits)
        {
            // get all displayable objects for the current drafting view
            drfView.Update();
            Matrix3x3 viewMatrix = drfView.Matrix;
            List<DisplayableObject> objInView = new List<DisplayableObject>();
            Tag visObj = NXOpen.Tag.Null;

            do
            {
                ufsession_.View.CycleObjects(drfView.Tag, UFView.CycleObjectsEnum.VisibleObjects, ref visObj);

                if (visObj == NXOpen.Tag.Null)
                    continue;

                ufsession_.Obj.AskTypeAndSubtype(visObj, out _, out _);
                DisplayableObject visEdge = (DisplayableObject)NXObjectManager.Get(visObj);
                objInView.Add(visEdge);
            }
            while (visObj != NXOpen.Tag.Null);

            // ask extreme of all displayed edges and then create CtsDimensionData for each edge
            List<CtsDimensionData> dimData = new List<CtsDimensionData>();

            foreach (DisplayableObject dispObj in objInView)
            {
                double[] drfLocation = new double[2];
                double[] extremePoint = new double[3];

                switch (dispObj)
                {
                    case Edge _:
                        double[] dirVectorX = viewMatrix.__AxisX().__ToArray();
                        double[] dirVectorY = viewMatrix.__AxisY().__ToArray();
                        double[] dirVectorZ = viewMatrix.__AxisZ().__ToArray();

                        ufsession_.Modl.AskExtreme(
                            dispObj.Tag,
                            dirVectorX,
                            dirVectorY,
                            dirVectorZ,
                            out _,
                            extremePoint);
                        break;
                    case Line _:
                        Line line = (Line)dispObj;
                        extremePoint = line.StartPoint.__ToArray();
                        break;
                    default:
                        continue;
                }

                ufsession_.View.MapModelToDrawing(drfView.Tag, extremePoint, drfLocation);

                CtsDimensionData dimObject = new CtsDimensionData
                {
                    DimXvalue = drfLocation[0],
                    DimYvalue = drfLocation[1],
                    DimEntity = dispObj.Tag,
                    Type = dispObj.GetType().ToString()
                };

                dimData.Add(dimObject);
            }

            // sort all CtsDimensionData objects to find mins and max
            if (dimData.Count <= 0)
                return;

            CtsDimensionData[] objInfo = dimData.ToArray();
            Array.Sort(objInfo, CtsDimensionData.SortXdescending());

            CtsDimensionData CreateCtsData(int index, CtsDimensionData.ExtremePointId id)
            {
                return new CtsDimensionData
                {
                    DimEntity = objInfo[index].DimEntity,
                    DimXvalue = objInfo[index].DimXvalue,
                    DimYvalue = objInfo[index].DimYvalue,
                    ExtPointId = (int)id,
                    Type = objInfo[index].Type
                };
            }

            CtsDimensionData minX = CreateCtsData(objInfo.Length - 1, CtsDimensionData.ExtremePointId.MinX);
            CtsDimensionData maxX = CreateCtsData(0, CtsDimensionData.ExtremePointId.MaxX);
            Array.Sort(objInfo, CtsDimensionData.SortYdescending());
            CtsDimensionData minY = CreateCtsData(objInfo.Length - 1, CtsDimensionData.ExtremePointId.MinY);
            CtsDimensionData maxY = CreateCtsData(0, CtsDimensionData.ExtremePointId.MaxY);
            minY.ExtPointId = (int)CtsDimensionData.ExtremePointId.MaxY;
            maxY.Type = objInfo[0].Type;

            switch (drfView.Name)
            {
                case "4-VIEW-BOTTOM":
                    {
                        // get minX greatest Y vertex
                        FindEndPoints(ref minX, out Point3d vertex1, out Point3d vertex2);

                        double[] drfVertex1 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex1.__ToArray(), drfVertex1);

                        double[] drfVertex2 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex2.__ToArray(), drfVertex2);

                        minX.DimXvalue = drfVertex1[0] <= drfVertex2[0] ? drfVertex1[0] : drfVertex2[0];
                        minX.ExtPointId = drfVertex1[0] <= drfVertex2[0] ? (int)FirstEndPoint : (int)LastEndPoint;
                        minX.DimYvalue = drfVertex1[1] >= drfVertex2[1] ? drfVertex1[1] : drfVertex2[1];

                        // get maxX greatest Y vertex
                        FindEndPoints(ref maxX, out Point3d vertex3, out Point3d vertex4);

                        double[] drfVertex3 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex3.__ToArray(), drfVertex3);

                        double[] drfVertex4 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex4.__ToArray(), drfVertex4);

                        maxX.DimXvalue = drfVertex3[0] >= drfVertex4[0] ? drfVertex3[0] : drfVertex4[0];
                        maxX.ExtPointId = drfVertex3[0] >= drfVertex4[0] ? (int)FirstEndPoint : (int)LastEndPoint;
                        maxX.DimYvalue = drfVertex3[1] >= drfVertex4[1] ? drfVertex3[1] : drfVertex4[1];

                        // get minY greatest X vertex
                        FindEndPoints(ref minY, out Point3d vertex5, out Point3d vertex6);

                        double[] drfVertex5 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex5.__ToArray(), drfVertex5);

                        double[] drfVertex6 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex6.__ToArray(), drfVertex6);
                        minY.DimXvalue = drfVertex5[0] >= drfVertex6[0] ? drfVertex5[0] : drfVertex6[0];
                        minY.DimYvalue = drfVertex5[1] <= drfVertex6[1] ? drfVertex5[1] : drfVertex6[1];
                        minY.ExtPointId = drfVertex5[0] >= drfVertex6[0]
                            ? drfVertex5[1] <= drfVertex6[1] ? (int)FirstEndPoint : (int)LastEndPoint
                            : drfVertex5[1] <= drfVertex6[1]
                                ? (int)FirstEndPoint
                                : (int)LastEndPoint;

                        // get maxY greatest X vertex
                        FindEndPoints(ref maxY, out Point3d vertex7, out Point3d vertex8);

                        double[] drfVertex7 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex7.__ToArray(), drfVertex7);

                        double[] drfVertex8 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex8.__ToArray(), drfVertex8);

                        maxY.DimXvalue = drfVertex7[0] >= drfVertex8[0] ? drfVertex7[0] : drfVertex8[0];
                        maxY.DimYvalue = drfVertex7[0] >= drfVertex8[0]
                            ? drfVertex7[1] >= drfVertex8[1] ? drfVertex7[1] : drfVertex8[1]
                            : drfVertex7[1] >= drfVertex8[1]
                                ? drfVertex7[1]
                                : drfVertex8[1];
                        maxY.ExtPointId = drfVertex7[0] >= drfVertex8[0]
                            ? drfVertex7[1] >= drfVertex8[1] ? (int)FirstEndPoint : (int)LastEndPoint
                            : drfVertex7[1] >= drfVertex8[1]
                                ? (int)FirstEndPoint
                                : (int)LastEndPoint;
                        break;
                    }
                case "4-VIEW-RIGHT":
                    {
                        // get minY least X vertex
                        FindEndPoints(ref minY, out Point3d vertex1, out Point3d vertex2);
                        double[] drfVertex1 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex1.__ToArray(), drfVertex1);
                        double[] drfVertex2 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex2.__ToArray(), drfVertex2);
                        minY.DimXvalue = drfVertex1[0] <= drfVertex2[0] ? drfVertex1[0] : drfVertex2[0];
                        minY.DimYvalue = drfVertex1[0] <= drfVertex2[0]
                            ? drfVertex1[1] <= drfVertex2[1] ? drfVertex1[1] : drfVertex2[1]
                            : drfVertex1[1] <= drfVertex2[1]
                                ? drfVertex1[1]
                                : drfVertex2[1];
                        minY.ExtPointId = drfVertex1[0] <= drfVertex2[0]
                            ? drfVertex1[1] <= drfVertex2[1] ? (int)FirstEndPoint : (int)LastEndPoint
                            : drfVertex1[1] <= drfVertex2[1]
                                ? (int)FirstEndPoint
                                : (int)LastEndPoint;
                        // get maxY least X vertex
                        FindEndPoints(ref maxY, out Point3d vertex3, out Point3d vertex4);
                        double[] drfVertex3 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex3.__ToArray(), drfVertex3);
                        double[] drfVertex4 = new double[2];
                        ufsession_.View.MapModelToDrawing(drfView.Tag, vertex4.__ToArray(), drfVertex4);
                        if (drfVertex3[0] <= drfVertex4[0])
                            maxY.DimXvalue = drfVertex3[0];
                        else
                            minY.DimXvalue = drfVertex4[0];
                        maxY.DimYvalue = drfVertex3[0] <= drfVertex4[0]
                            ? drfVertex3[1] >= drfVertex4[1] ? drfVertex3[1] : drfVertex4[1]
                            : drfVertex3[1] >= drfVertex4[1]
                                ? drfVertex3[1]
                                : drfVertex4[1];
                        maxY.ExtPointId = drfVertex3[0] <= drfVertex4[0]
                            ? drfVertex3[1] >= drfVertex4[1] ? (int)FirstEndPoint : (int)LastEndPoint
                            : drfVertex3[1] >= drfVertex4[1]
                                ? (int)FirstEndPoint
                                : (int)LastEndPoint;
                        break;
                    }
            }

            double dimSpace = partUnits == BasePart.Units.Inches ? .75 : 20;
            double xDistance = size[0];
            double zDistance = size[2];

            // orthoRight dimension placement and text value
            UFDrf.Text orthoRightDimText = new UFDrf.Text();
            double[] orthoRightDimOrigin = new double[3];
            orthoRightDimOrigin[0] = drfViewPosition[0] - zDistance / 2 - dimSpace * viewScale;
            orthoRightDimOrigin[1] = drfViewPosition[1];
            orthoRightDimOrigin[2] = 0;

            // orthoBottom horizontal dimension placement and text value
            UFDrf.Text orthoBtmDimText = new UFDrf.Text();
            double[] orthoBtmHorizDimOrigin = new double[3];
            orthoBtmHorizDimOrigin[0] = drfViewPosition[0];
            orthoBtmHorizDimOrigin[1] = drfViewPosition[1] + zDistance / 2 + dimSpace * viewScale;
            orthoBtmHorizDimOrigin[2] = 0;

            // orthoBottom vertical dimension placement and text value
            UFDrf.Text orthoBtmVertDimText = new UFDrf.Text();
            double[] orthoBtmVertDimOrigin = new double[3];
            orthoBtmVertDimOrigin[0] = drfViewPosition[0] + xDistance / 2 + dimSpace * viewScale;
            orthoBtmVertDimOrigin[1] = drfViewPosition[1];
            orthoBtmVertDimOrigin[2] = 0;
            UFDrf.Object drfObjMaxX = new UFDrf.Object();
            UFDrf.Object drfObjMinX = new UFDrf.Object();
            UFDrf.Object drfObjMaxY = new UFDrf.Object();
            UFDrf.Object drfObjMinY = new UFDrf.Object();
            ufsession_.Drf.InitObjectStructure(ref drfObjMaxX);
            ufsession_.Drf.InitObjectStructure(ref drfObjMinX);
            ufsession_.Drf.InitObjectStructure(ref drfObjMaxY);
            ufsession_.Drf.InitObjectStructure(ref drfObjMinY);

            if (drfView.Name == "4-VIEW-BOTTOM")
            {
                AssignDraftObject(drfView, minX, ref drfObjMinX);
                AssignDraftObject(drfView, maxX, ref drfObjMaxX);
                AssignDraftObject(drfView, minY, ref drfObjMinY);
                AssignDraftObject(drfView, maxY, ref drfObjMaxY);
                ufsession_.Drf.CreateVerticalDim(ref drfObjMinY, ref drfObjMaxY, ref orthoBtmVertDimText,
                    orthoBtmVertDimOrigin, out _);
                ufsession_.Drf.CreateHorizontalDim(ref drfObjMinX, ref drfObjMaxX, ref orthoBtmDimText,
                    orthoBtmHorizDimOrigin, out _);
            }

            if (drfView.Name != "4-VIEW-RIGHT")
                return;

            AssignDraftObject(drfView, minY, ref drfObjMinY);
            AssignDraftObject(drfView, maxY, ref drfObjMaxY);

            ufsession_.Drf.CreateVerticalDim(
                ref drfObjMinY,
                ref drfObjMaxY,
                ref orthoRightDimText,
                orthoRightDimOrigin,
                out _);
        }

        private static void FindEndPoints(ref CtsDimensionData maxY, out Point3d vertex3, out Point3d vertex4)
        {
            if (maxY.DimEntity.__ToTaggedObject() is Line line1)
            {
                vertex3 = line1.StartPoint;
                vertex4 = line1.EndPoint;
                return;
            }

            Edge edge = (Edge)NXObjectManager.Get(maxY.DimEntity);
            edge.GetVertices(out vertex3, out vertex4);
        }


        private static void CreateHoleChartNote(Point3d noteOrigin, string[] holeDia, Tag view)
        {
            Session.UndoMarkId markIdNote = session_.SetUndoMark(Session.MarkVisibility.Invisible, "CreateOrNull Annotation");
            LetteringPreferences letteringPreferences1 = __work_part_.Annotations.Preferences.GetLetteringPreferences();

            UserSymbolPreferences userSymbolPreferences1 = __work_part_.Annotations.NewUserSymbolPreferences(
                UserSymbolPreferences.SizeType.ScaleAspectRatio,
                1.0,
                1.0);

            Note note1 = __work_part_.Annotations.CreateNote(
                holeDia,
                noteOrigin,
                AxisOrientation.Horizontal,
                letteringPreferences1,
                userSymbolPreferences1);

            note1.SetName("HOLECHARTNOTE");
            ufsession_.View.ConvertToModel(view, note1.Tag);
            letteringPreferences1.Dispose();
            userSymbolPreferences1.Dispose();
            session_.UpdateManager.DoUpdate(markIdNote);
            session_.DeleteUndoMark(markIdNote, "CreateOrNull Annotation");
        }

        private static void SetLayerVisibility(Tag viewToSet)
        {
            View viewObj = (View)NXObjectManager.Get(viewToSet);
            StateInfo[] layerVisibleInView = new StateInfo[256];

            for (int i = 0; i < layerVisibleInView.Length - 1; i++)
            {
                layerVisibleInView[i].Layer = i + 1;
                layerVisibleInView[i].State = State.Hidden;
            }

            layerVisibleInView[0].Layer = 1;
            layerVisibleInView[0].State = State.Visible;
            layerVisibleInView[95].Layer = 96;
            layerVisibleInView[95].State = State.Visible;
            layerVisibleInView[111].State = State.Visible;
            __display_part_.Layers.SetObjectsVisibilityOnLayer(viewObj, layerVisibleInView, true);
        }

        private static TextCfw CreateCfw(int color)
        {
            return new TextCfw(color, 1, LineWidth.Normal);
        }

        private static void SetLetteringPreferences(BasePart.Units units)
        {
            double size = units == BasePart.Units.Inches ? .125 : 3.175;

            using (LetteringPreferences letteringPreferences =
                   __display_part_.Annotations.Preferences.GetLetteringPreferences())
            {
                Lettering generalText = new Lettering
                {
                    Size = size,
                    CharacterSpaceFactor = 1.0,
                    AspectRatio = 1.0,
                    LineSpaceFactor = 1.0,
                    Cfw = CreateCfw(31),
                    Italic = false
                };

                letteringPreferences.SetGeneralText(generalText);

                __display_part_.Annotations.Preferences.SetLetteringPreferences(letteringPreferences);
            }
        }

        private void SetDraftingPreferences(BasePart.Units units, double scale)
        {
            double unitMultiplier = units == BasePart.Units.Inches ? 1.0 : 25.4;

            using (LetteringPreferences letteringPreferences1 =
                   __work_part_.Annotations.Preferences.GetLetteringPreferences())
            {
                Lettering CreateLettering(double value)
                {
                    return new Lettering
                    {
                        Size = value * unitMultiplier * scale,
                        CharacterSpaceFactor = 0.9,
                        AspectRatio = 1.0,
                        LineSpaceFactor = 1.0,
                        Cfw = CreateCfw(7),
                        Italic = false
                    };
                }

                letteringPreferences1.SetDimensionText(CreateLettering(.125));
                letteringPreferences1.SetAppendedText(CreateLettering(.0625));
                letteringPreferences1.SetToleranceText(CreateLettering(.0625));
                letteringPreferences1.SetGeneralText(CreateLettering(.125));
                __work_part_.Annotations.Preferences.SetLetteringPreferences(letteringPreferences1);
            }

            using (LineAndArrowPreferences lineAndArrowPreferences1 = __work_part_.Annotations.Preferences.GetLineAndArrowPreferences())
            {
                LineCfw Cfw(int color)
                {
                    return new LineCfw(color, DisplayableObject.ObjectFont.Solid,
                        LineWidth.Thin);
                }

                lineAndArrowPreferences1.SetFirstExtensionLineCfw(Cfw(141));
                lineAndArrowPreferences1.SetFirstArrowheadCfw(Cfw(173));
                lineAndArrowPreferences1.SetFirstArrowLineCfw(Cfw(173));
                lineAndArrowPreferences1.SetSecondExtensionLineCfw(Cfw(173));
                lineAndArrowPreferences1.SetSecondArrowheadCfw(Cfw(173));
                lineAndArrowPreferences1.SetSecondArrowLineCfw(Cfw(173));
                lineAndArrowPreferences1.ArrowheadLength = .125 * unitMultiplier;
                lineAndArrowPreferences1.ArrowheadIncludedAngle = 30.0;
                lineAndArrowPreferences1.StubLength = 0.25 * unitMultiplier;
                lineAndArrowPreferences1.TextToLineDistance = 0.0625 * unitMultiplier;
                lineAndArrowPreferences1.LinePastArrowDistance = 0.125 * unitMultiplier;
                lineAndArrowPreferences1.FirstPosToExtLineDist = 0.0625 * unitMultiplier;
                lineAndArrowPreferences1.SecondPosToExtLineDist = 0.0625 * unitMultiplier;
                lineAndArrowPreferences1.DatumLengthPastArrow = 0.0625 * unitMultiplier;
                lineAndArrowPreferences1.TextOverStubSpaceFactor = 0.1 * unitMultiplier;
                __display_part_.Annotations.Preferences.SetLineAndArrowPreferences(lineAndArrowPreferences1);
            }

            using (DimensionPreferences dimensionPreferences = __display_part_.Annotations.Preferences.GetDimensionPreferences())
            {
                dimensionPreferences.ExtensionLineDisplay = ExtensionLineDisplay.Two;
                dimensionPreferences.ArrowDisplay = ArrowDisplay.Two;
                dimensionPreferences.TextPlacement = TextPlacement.Automatic;
                dimensionPreferences.TextOrientation = TextOrientation.Horizontal;
                LinearTolerance linearTolerance1 = __display_part_.Annotations.Preferences.GetLinearTolerances();
                linearTolerance1.PrimaryDimensionDecimalPlaces = units == BasePart.Units.Inches ? 3 : 2;
                __display_part_.Annotations.Preferences.SetLinearTolerances(linearTolerance1);

                using (UnitsFormatPreferences unitsFormatPreferences = dimensionPreferences.GetUnitsFormatPreferences())
                {
                    if (chkDualDimensions.Checked)
                        unitsFormatPreferences.DualDimensionPlacement = DualDimensionPlacement.Below;

                    unitsFormatPreferences.PrimaryDimensionUnit = units == BasePart.Units.Inches
                        ? DimensionUnit.Inches
                        : DimensionUnit.Millimeters;

                    unitsFormatPreferences.PrimaryDimensionTextFormat = DimensionTextFormat.Decimal;
                    unitsFormatPreferences.DecimalPointCharacter = DecimalPointCharacter.Period;
                    unitsFormatPreferences.DisplayTrailingZeros = true;
                    unitsFormatPreferences.TolerancePlacement = TolerancePlacement.After;
                    dimensionPreferences.SetUnitsFormatPreferences(unitsFormatPreferences);
                    __work_part_.Annotations.Preferences.SetDimensionPreferences(dimensionPreferences);
                }
            }
        }

        private static void SetViewPreferences()
        {
            ViewPreferences viewPreferences = __work_part_.ViewPreferences;
            viewPreferences.HiddenLines.HiddenlineColor = 0;
            viewPreferences.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Dashed;
            viewPreferences.VisibleLines.VisibleColor = 0;
            viewPreferences.VisibleLines.VisibleWidth = NXOpen.Preferences.Width.Original;
            viewPreferences.SmoothEdges.SmoothEdgeColor = 0;
            viewPreferences.SmoothEdges.SmoothEdgeFont = NXOpen.Preferences.Font.Invisible;
            viewPreferences.SmoothEdges.SmoothEdgeWidth = NXOpen.Preferences.Width.Original;
            viewPreferences.General.AutomaticAnchorPoint = false;
            viewPreferences.General.Centerlines = false;
            __work_part_.Preferences.ColorSettingVisualization.MonochromeDisplay = false;
            __work_part_.Preferences.Drafting.DisplayBorders = false;
        }

        private static void SetCsysToPlanView()
        {
            // Revision 1.4 – 2017 – 06 – 15
            // This entire method was rewritten.
            try
            {
                Layout layout = __work_part_.Layouts.FindObject("L1");
                ModelingView backView = __work_part_.ModelingViews.FindObject("Back");
                layout.ReplaceView(__work_part_.ModelingViews.WorkView, backView, true);

                string viewName = __display_part_.ModelingViews.ToArray().Any(__k => __k.Name == "PLAN")
                    ? "PLAN"
                    : "Top";

                ModelingView view = __display_part_.__FindModelingView(viewName);
                layout.ReplaceView(backView, view, true);
                __display_part_.WCS.SetOriginAndMatrix(view.Origin, view.Matrix);
            }
            catch (NXException ex) when (ex.ErrorCode == 925019)
            {
                print_($"Could not replace view in {__work_part_.Leaf}");
            }
            catch (Exception ex)
            {
                ex.__PrintException($"Part: {__work_part_.Leaf}");
            }
            finally
            {
                __work_part_.ModelingViews.WorkView.UpdateDisplay();
            }
        }

        private void GetChildComponents(Component assembly)
        {
            foreach (Component child in assembly.GetChildren())
            {
                if (child.IsSuppressed)
                {
                    if (!IsNameValid(child))
                        continue;
                    print_($"{child.DisplayName} is suppressed");
                    continue;
                }

                if (assembly.GetChildren() is null)
                    continue;

                if (!IsNameValid(child))
                {
                    GetChildComponents(child);
                    continue;
                }

                Tag instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);

                if (instance == NXOpen.Tag.Null)
                    continue;

                ufsession_.Assem.AskPartNameOfChild(instance, out string partName);

                if (ufsession_.Part.IsLoaded(partName) == 1)
                {
                    _allComponents.Add(child);
                    GetChildComponents(child);
                    continue;
                }

                ufsession_.Cfi.AskFileExist(partName, out int status);

                if (status != 0)
                    continue;

                ufsession_.Part.OpenQuiet(partName, out Tag partOpen, out _);

                if (partOpen == NXOpen.Tag.Null)
                    continue;

                _allComponents.Add(child);
                GetChildComponents(child);
            }
        }

        public static bool IsNameValid(Component comp)
        {
            return Regex.IsMatch(comp.DisplayName, RegexDetail);
        }

        public static List<CtsAttributes> CreateMaterialList()
        {
            string getMaterial = PerformStreamReaderString(FilePathUcf, ":MATERIAL_ATTRIBUTE_NAME:",
                ":END_MATERIAL_ATTRIBUTE_NAME:");
            List<CtsAttributes> compMaterials =
                PerformStreamReaderList(FilePathUcf, ":COMPONENT_MATERIALS:", ":END_COMPONENT_MATERIALS:");
            foreach (CtsAttributes cMaterial in compMaterials)
                cMaterial.AttrName = getMaterial != string.Empty ? getMaterial : "MATERIAL";
            return compMaterials;
        }

        private static string PerformStreamReaderString(string path, string startSearchString, string endSearchString)
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                string content = sr.ReadToEnd();
                sr.Close();
                string[] startSplit = Regex.Split(content, startSearchString);
                string[] endSplit = Regex.Split(startSplit[1], endSearchString);
                string textSetting = endSplit[0];
                textSetting = textSetting.Replace("\r\n", string.Empty);
                return textSetting.Length > 0 ? textSetting : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private static List<CtsAttributes> PerformStreamReaderList(string path, string startSearchString,
            string endSearchString)
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                string content = sr.ReadToEnd();
                sr.Close();
                string[] startSplit = Regex.Split(content, startSearchString);
                string[] endSplit = Regex.Split(startSplit[1], endSearchString);
                string textData = endSplit[0];
                string[] splitData = Regex.Split(textData, "\r\n");
                List<CtsAttributes> compData = (from sData in splitData
                                                where sData != string.Empty
                                                select new CtsAttributes { AttrValue = sData }).ToList();
                return compData.Count > 0 ? compData : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private static void SetHoleChartLayers()
        {
            __display_part_.Layers.SetState(230, State.WorkLayer);
            SetLayers(1, 94, 96, 200, 231);
        }

        private static void SetDefaultLayers()
        {
            __display_part_.Layers.SetState(1, State.WorkLayer);
            SetLayers(94, 96, 99, 100, 111, 200, 230);
        }

        private static void SetLayers(params int[] layers)
        {
            using (StateCollection layerState = __display_part_.Layers.GetStates())
            {
                foreach (Category category in __display_part_.LayerCategories)
                    if (category.Name == "ALL")
                        layerState.SetStateOfCategory(category, State.Hidden);
                __display_part_.Layers.SetStates(layerState, true);
            }

            layers.Where(layer => __display_part_.Layers.GetState(layer) != State.WorkLayer)
                .ToList()
                .ForEach(layer => __display_part_.Layers.SetState(layer, State.Selectable));
        }

        private void SetFormDefaults()
        {
            chkHoleChart.Checked = false;
            chkDetailSheet.Checked = false;
            chkUpdateViews.Checked = false;
            chkDelete4Views.Checked = false;
            chkHoleChart.Checked = false;
            btnSelect.Enabled = false;
            btnSelectAll.Enabled = false;
            _allComponents.Clear();
        }

        private void ChkChart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDrillChart.Checked)
                chkHoleChart.Checked = false;

            if (chkHoleChart.Checked)
                chkDrillChart.Checked = false;
        }

        private double[][] GetMinsMaxs(Body body)
        {
            double[] minCorner = new double[3];
            double[] distances = new double[3];

            ufsession_.Modl.AskBoundingBoxAligned(body.Tag, __display_part_.WCS.CoordinateSystem.Tag, false, minCorner,
                new double[3, 3], distances);

            return new[]
            {
                minCorner,
                distances
            };
        }

        private void CreateDetailSheet(string[] drillChart)
        {
            PreferencesBuilder preferencesBuilder1 = __work_part_.SettingsManager.CreatePreferencesBuilder();

            using (new Destroyer(preferencesBuilder1))
            {
                preferencesBuilder1.ViewStyle.ViewStyleGeneral.ViewRepresentation =
                    GeneralViewRepresentationOption.PreNx85Exact;
                preferencesBuilder1.ViewStyle.ViewStyleGeneral.ExtractedEdges = GeneralExtractedEdgesOption.None;
                preferencesBuilder1.Commit();
            }

            try
            {
                const double baseWidthXDir = 11;
                const double fitSheetXDir = .7;
                const double baseHeightYDir = 8.5;
                const double fitSheetYDir = .7;
                const double borderSpace = 1;
                const double viewSpace = 2;
                const double viewMinFromBtm = 1;
                double scale = 1;
                const double increment = .125;
                int bodyCount = __display_part_.Bodies.Cast<Body>().Count(sBody1 => sBody1.Layer == 1);

                if (bodyCount != 1)
                {
                    print_(
                        $"DetailPart Sheet will not be created.  {__display_part_.FullPath} : Solid bodies on layer one = {bodyCount}");
                    return;
                }

                Body sBody = __display_part_.__SolidBodyLayer1OrNull();

                if (sBody is null)
                {
                    print_($"Could not find solid body on layer 1 :' {__display_part_.Leaf}'");
                    return;
                }

                BasePart.Units units = __display_part_.PartUnits;
                double scaledWidth;
                double scaledHeight;
                double[] minCorner = new double[3];
                double[] distances = new double[3];

                // import 4-VIEW border part
                ImportPartModes modes = new ImportPartModes
                {
                    layer_mode = 1,
                    group_mode = 1,
                    csys_mode = 0,
                    plist_mode = 0,
                    view_mode = 0,
                    cam_mode = false,
                    use_search_dirs = false
                };

                double[][] results = GetMinsMaxs(sBody);

                minCorner = results[0];
                distances = results[1];

                Body[] layer111SolidBodies = __display_part_.Bodies
                    .ToArray()
                    .Where(b => b.IsSolidBody && b.Layer == 111)
                    .ToArray();

                foreach (Body body111 in layer111SolidBodies)
                {
                    double[][] tempResult = GetMinsMaxs(body111);
                    double[] minCorner1 = tempResult[0];
                    double[] distances1 = tempResult[1];
                    distances[0] = Math.Max(distances[0], distances1[0]);
                    distances[1] = Math.Max(distances[1], distances1[1]);
                    distances[2] = Math.Max(distances[2], distances1[2]);
                    minCorner[0] = Math.Min(minCorner[0], minCorner1[0]);
                    minCorner[1] = Math.Min(minCorner[1], minCorner1[1]);
                    minCorner[2] = Math.Min(minCorner[2], minCorner1[2]);
                }

                double unitMultiplier = units == BasePart.Units.Inches ? 1.0 : 25.4;
                double measureWidth =
                    (distances[0] + distances[2] + borderSpace * unitMultiplier * 2 + viewSpace * unitMultiplier) /
                    unitMultiplier;
                double measureHeight = (distances[1] + distances[2] + borderSpace * unitMultiplier +
                                        viewSpace * unitMultiplier +
                                        viewMinFromBtm * unitMultiplier) / unitMultiplier;

                if (measureWidth > baseWidthXDir * fitSheetXDir || measureHeight > baseHeightYDir * fitSheetYDir)
                    do
                    {
                        scale += increment;
                        scaledWidth = baseWidthXDir * fitSheetXDir * scale;
                        scaledHeight = baseHeightYDir * fitSheetYDir * scale;
                    }
                    while (scaledWidth < measureWidth || scaledHeight < measureHeight);

                if (__display_part_.Expressions.ToArray().All(expression => expression.Name != "borderScale"))
                    using (session_.__UsingDoUpdate("Expression"))
                    {
                        __work_part_.Expressions.CreateWithUnits($"borderScale={scale}", null);
                    }

                double sheetWidth = baseWidthXDir * unitMultiplier * scale;
                double sheetHeight = baseHeightYDir * unitMultiplier * scale;

                DrawingSheet.Unit drawingSheetUnits = units == BasePart.Units.Inches
                    ? DrawingSheet.Unit.Inches
                    : DrawingSheet.Unit.Millimeters;

                DrawingSheet fourViewSheet = __display_part_.DraftingDrawingSheets.InsertSheet(
                    "4-VIEW",
                    drawingSheetUnits,
                    sheetHeight,
                    sheetWidth,
                    1,
                    1,
                    DrawingSheet.ProjectionAngleType.ThirdAngle);

                SetDraftingPreferences(units, scale);
                SetViewPreferences();
                ufsession_.Part.Import(_borderFile, ref modes, new double[] { 1, 0, 0, 0, 1, 0 },
                    new double[] { 0, 0, 0 }, scale, out _);
                Part temp = __display_part_;

                try
                {
                    if (chkDrillChart.Checked && drillChart.Length > 0)
                    {
                        Note note = (Note)session_.__FindByName(@"GRUMBLEGRUMBLE");

                        note.SetText(drillChart);
                    }
                    else
                    {
                        session_.__DeleteObjects(session_.__FindByName(@"GRUMBLEGRUMBLE"));
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                double xDistance = distances[0];
                double yDistance = distances[1];
                double zDistance = distances[2];
                double[] pvRefPoint = new double[2];

                pvRefPoint[0] = sheetWidth * .333 > xDistance
                    ? sheetWidth * .333
                    : borderSpace * unitMultiplier * scale + xDistance / 2;

                pvRefPoint[1] = sheetHeight * .667 > yDistance
                    ? sheetHeight * .667
                    : (viewMinFromBtm * unitMultiplier + viewSpace * unitMultiplier) * scale + zDistance +
                      yDistance / 2;

                DraftingView planDrfView = ImportPlanView(pvRefPoint[0], pvRefPoint[1]);
                planDrfView.Update();
                double[] orthogonalBtm = new double[2];
                double[] orthogonalRight = new double[2];
                orthogonalRight[0] = pvRefPoint[0] + xDistance / 2 + viewSpace * unitMultiplier * scale + zDistance / 2;
                orthogonalRight[1] = pvRefPoint[1];
                ufsession_.Draw.AddOrthographicView(fourViewSheet.Tag, planDrfView.Tag, UFDraw.ProjDir.ProjectRight,
                    orthogonalRight, out Tag rOrthoTag);
                DraftingView rightDrfView = (DraftingView)NXObjectManager.Get(rOrthoTag);
                rightDrfView.SetName("4-VIEW-RIGHT");
                SetLayerVisibility(rOrthoTag);
                StateInfo stateArray = new StateInfo(111, State.Visible);
                __display_part_.Layers.SetObjectsVisibilityOnLayer(rightDrfView, new[] { stateArray }, true);
                orthogonalBtm[0] = pvRefPoint[0];
                orthogonalBtm[1] = pvRefPoint[1] - yDistance / 2 - viewSpace * unitMultiplier * .667 * scale -
                                   zDistance / 2;
                ufsession_.Draw.AddOrthographicView(fourViewSheet.Tag, planDrfView.Tag, UFDraw.ProjDir.ProjectBelow,
                    orthogonalBtm, out Tag bOrthoTag);
                DraftingView btmDrfView = (DraftingView)NXObjectManager.Get(bOrthoTag);
                btmDrfView.SetName("4-VIEW-BOTTOM");
                SetLayerVisibility(bOrthoTag);
                __display_part_.Layers.SetObjectsVisibilityOnLayer(btmDrfView, new[] { stateArray }, true);

                foreach (NXObject.AttributeInformation attributeNote in __display_part_.GetUserAttributes())
                {
                    if (attributeNote.Title.ToUpper() != "DESCRIPTION")
                        continue;

                    string descValue =
                        __display_part_.GetUserAttributeAsString(attributeNote.Title, NXObject.AttributeType.String,
                            -1);

                    if (descValue.ToUpper() == "NITROGEN PLATE SYSTEM")
                        continue;

                    DimensionView(btmDrfView, orthogonalBtm, scale, distances, units);
                    DimensionView(rightDrfView, orthogonalRight, scale, distances, units);
                }

                bool isWireTaper = false;
                bool isWaitForDev = false;

                foreach (NXObject.AttributeInformation attrNote in __display_part_.GetUserAttributes())
                {
                    if (attrNote.Title.ToUpper() == "WTN")
                    {
                        string wtnValue =
                            __display_part_.GetUserAttributeAsString(attrNote.Title, NXObject.AttributeType.String, -1);

                        if (wtnValue.ToUpper() == "YES")
                            isWireTaper = true;
                    }

                    if (attrNote.Title.ToUpper() != "WFTD")
                        continue;

                    string wfftValue =
                        __display_part_.GetUserAttributeAsString(attrNote.Title, NXObject.AttributeType.String, -1);

                    if (wfftValue.ToUpper() == "YES")
                        isWaitForDev = true;
                }

                List<NXObject> addToDelete = new List<NXObject>();

                foreach (Note drfNote in __display_part_.Notes)
                {
                    if (drfNote.Layer != 200)
                        continue;

                    string[] noteText = drfNote.GetText();

                    //Changed from "TSG STANDARD" to "STANDARD" - to work with the GE Border file. 2016-11-16 Duane VW
                    if (noteText[0].Contains("STANDARD") && isWireTaper == false)
                        addToDelete.Add(drfNote);

                    if (noteText[0].Contains("WAITING FOR FINAL TRIM") && isWaitForDev == false)
                        addToDelete.Add(drfNote);
                }

                if (addToDelete.Count != 0)
                    session_.__DeleteObjects(addToDelete.ToArray());
            }
            catch (Exception ex)
            {
                ex.__PrintException(__display_part_.Leaf);
            }
        }

        public struct CtsDimensionData : IComparable
        {
            public CtsDimensionData(string objType, Tag objectTag, double x, double y, ExtremePointId extremeId) : this()
            {
                Type = objType;
                DimEntity = objectTag;
                DimXvalue = x;
                DimYvalue = y;
            }

            public string Type { get; set; }

            public Tag DimEntity { get; set; }

            public double DimXvalue { get; set; }

            public double DimYvalue { get; set; }

            public int ExtPointId { get; set; }


            int IComparable.CompareTo(object obj)
            {
                return string.CompareOrdinal(Type, ((CtsDimensionData)obj).Type);
            }

            private class SortXdescending_ : IComparer
            {
                int IComparer.Compare(object x, object x1)
                {
                    if (x is null && x1 is null) return 0;
                    if (x is null ^ x1 is null) return 1;
                    if (((CtsDimensionData)x).DimXvalue > ((CtsDimensionData)x1).DimXvalue) return -1;
                    return ((CtsDimensionData)x).DimXvalue < ((CtsDimensionData)x1).DimXvalue ? 1 : 0;
                }
            }

            private class SortYdescending_ : IComparer
            {
                public int Compare(object y, object y1)
                {
                    if (y is null && y1 is null) return 0;
                    if (y is null ^ y1 is null) return 1;
                    if (((CtsDimensionData)y).DimYvalue > ((CtsDimensionData)y1).DimYvalue) return -1;
                    return ((CtsDimensionData)y).DimYvalue < ((CtsDimensionData)y1).DimYvalue ? 1 : 0;
                }
            }

            public static IComparer SortXdescending()
            {
                return new SortXdescending_();
            }

            public static IComparer SortYdescending()
            {
                return new SortYdescending_();
            }

            public enum ExtremePointId
            {
                None,
                MinX,
                MaxX,
                MinY,
                MaxY,
                MinZ,
                MaxZ
            }

            public enum EndPointAssociativity
            {
                None = 0,
                FirstEndPoint = UFConstants.UF_DRF_first_end_point,
                LastEndPoint = UFConstants.UF_DRF_last_end_point
            }
        }

        [Obsolete]
        public static class DrillChart
        {
            private const string HoleChartText = @"U:\nxFiles\UfuncFiles\HoleChart.txt";

            private static Part __display_part_ => Session.GetSession().Parts.Display;

            public static string[] Main()
            {
                string[] lines = File.ReadAllLines(HoleChartText)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Where(s => !s.StartsWith("//"))
                    .ToArray();

                IList<string[]> holeChart = new List<string[]>();

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] split = lines[i].Split('\t');

                    holeChart.Add(split);
                }

                // Get the solid body on layer 1
                Body solidBody = __display_part_.__SolidBodyLayer1OrNull();

                if (solidBody is null)
                    throw new ArgumentException("Display part does not have solid body on layer 1");

                IDictionary<double, Tuple<int[], IList<Face>, string[]>> dict =
                    new Dictionary<double, Tuple<int[], IList<Face>, string[]>>();

                foreach (Face face in solidBody.GetFaces())
                {
                    if (face.SolidFaceType != Face.FaceType.Cylindrical)
                        continue;

                    if (!face.Name.ToUpper().Contains("HOLECHART"))
                        continue;

                    double[] point = new double[3];
                    double[] dir = new double[3];
                    double[] box = new double[6];

                    TheUFSession.Modl.AskFaceData(face.Tag, out int _, point, dir, box, out double radius, out _, out _);

                    double diameter = radius * 2; // * 25.4;

                    string[] actualLine =
                    (
                        from line in holeChart
                        let tempRadius = double.Parse(line[1])
                        where System.Math.Abs(tempRadius - diameter) < .000000001
                        select line
                    ).FirstOrDefault();

                    if (actualLine is null)
                    {
                        print_($"Couldn't find hole chart: {diameter}");

                        continue;
                    }

                    if (!dict.ContainsKey(diameter))
                        dict.Add(diameter,
                            new Tuple<int[], IList<Face>, string[]>(new[] { 0 }, new List<Face>(), actualLine));

                    dict[diameter].Item1[0]++;

                    dict[diameter].Item2.Add(face);
                }

                session_.__DeleteObjects(__display_part_.Layers.GetAllObjectsOnLayer(230).OfType<Note>().ToArray());

                char letter = 'A';

                IList<IList<string>> actualLines = new List<IList<string>>();

                foreach (double diameter in dict.Keys)
                {
                    Tuple<int[], IList<Face>, string[]> tuple = dict[diameter];

                    int count = tuple.Item1[0];

                    IList<string> list = new List<string>();

                    IList<Face> faces = tuple.Item2;

                    string[] message = tuple.Item3;

                    list.Add($"{letter} ");

                    string temp = message.Length == 3 ? $"{message[2]} " : $"{message[0]} ";


                    string[] split = Regex.Split(temp, "FOR\\s");

                    list.Add($"{split[0]}FOR");
                    list.Add(split[1]);


                    //list.Add(temp);

                    list.Add($"QTY {count}");

                    actualLines.Add(list);


                    foreach (Face face in faces)
                    {
                        double[] point = new double[3];
                        double[] dir = new double[3];
                        double[] box = new double[6];

                        TheUFSession.Modl.AskFaceData(face.Tag, out int _, point, dir, box, out double _, out _, out _);

                        using (session_.__UsingDoUpdate())
                        {
                            using (LetteringPreferences letteringPreferences1 =
                                   __work_part_.Annotations.Preferences.GetLetteringPreferences())
                            using (UserSymbolPreferences userSymbolPreferences1 =
                                   __work_part_.Annotations.NewUserSymbolPreferences(
                                       UserSymbolPreferences.SizeType.ScaleAspectRatio,
                                       1.0,
                                       1.0))
                            {
                                userSymbolPreferences1.SetLengthAndHeight(.125, .125);

                                Note note1 = __work_part_.Annotations.CreateNote(
                                    new[] { $"{letter}" },
                                    point.__ToPoint3d(),
                                    AxisOrientation.Horizontal,
                                    letteringPreferences1,
                                    userSymbolPreferences1);

                                note1.Layer = 230;

                                note1.RedisplayObject();

                                note1.SetName("HOLECHARTNOTE");
                                TheUFSession.View.ConvertToModel(__display_part_.ModelingViews.WorkView.Tag, note1.Tag);

                                DraftingNoteBuilder draftingNoteBuilder1 =
                                    __work_part_.Annotations.CreateDraftingNoteBuilder(note1);

                                using (session_.__UsingBuilderDestroyer(draftingNoteBuilder1))
                                {
                                    draftingNoteBuilder1.Style.LetteringStyle.GeneralTextSize = .125;
                                    draftingNoteBuilder1.Commit();
                                }
                            }
                        }
                    }

                    letter++;
                }

                IList<string> note = new List<string>();

                foreach (IList<string> t in actualLines)
                {
                    string _letter = t[0];
                    string drill = t[1];
                    string fastenr = t[2];
                    string quantity = t[3];

                    note.Add($"{_letter}{drill}");
                    note.Add($"{fastenr}{quantity}".ToUpper());
                    note.Add("");


                    string s = t.Aggregate("", (current, k) => current + k);

                    note.Add(s);
                }


                //            WriteLine(str);

                //           var session_ = NXOpen.Session.GetSession();
                //            var workPart = session_.Parts.Work;
                //            var displayPart = session_.Parts.Display;
                //            var drawingSheet1 = workPart.DrawingSheets.FindObject("4-VIEW");
                //            drawingSheet1.Open();
                //
                //            session_.ApplicationSwitchImmediate("UG_APP_DRAFTING");
                //
                //            workPart.Drafting.EnterDraftingApplication();
                //            
                //            var  point1 = new NXOpen.Point3d(91.560795454545442, 71.080042613636351, 0.0);
                //
                //
                //
                //            session_.create.Note(point1, Snap.Orientation.Identity, new TextStyle(), note.ToArray());

                return note.ToArray();
                //
            }
        }
    }
}
// 1877