﻿using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.Drawings;
using NXOpen.Layer;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc("create-burnout")]
    public partial class CreateBurnoutForm : _UFuncForm
    {
        private static List<Component> _allComponents = new List<Component>();

        private static List<Component> _selComponents = new List<Component>();

        public CreateBurnoutForm()
        {
            InitializeComponent();
        }

        private static Part __display_part_ => Session.GetSession().Parts.Display;

        private void CheckBoxBurnoutSheet_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxBurnoutSheet.Checked)
            {
                checkBoxDeleteBurnout.Checked = false;
                buttonSelect.Enabled = true;
                buttonSelectAll.Enabled = true;
            }
            else

            {
                if (checkBoxDeleteBurnout.Checked) return;
                buttonSelect.Enabled = false;
                buttonSelectAll.Enabled = false;
            }
        }

        private void CheckBoxDeleteBurnout_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDeleteBurnout.Checked)
            {
                checkBoxBurnoutSheet.Checked = false;
                buttonSelect.Enabled = true;
                buttonSelectAll.Enabled = true;
            }
            else
            {
                if (checkBoxBurnoutSheet.Checked) return;
                buttonSelect.Enabled = false;
                buttonSelectAll.Enabled = false;
            }
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            try
            {
                _allComponents.Clear();
                _selComponents.Clear();

                if (sender == buttonSelect)
                {
                    // Select: User selects components.
                    _allComponents = Selection.SelectManyComponents().ToList();

                    if (_allComponents.Count == 0)
                        return;
                }
                else if (sender == buttonSelectAll)
                    // Select All: Iterate through assembly.
                {
                    _allComponents = __display_part_.__RootComponent()
                        .__Descendants()
                        .Where(__c => !__c.IsSuppressed)
                        .ToList();
                }
                else
                {
                    return;
                }

                if (_allComponents.Count != 0)
                {
                    _selComponents = _allComponents.Distinct().ToList();

                    if (_selComponents.Count != 0)
                    {
                        List<Component> burnComponent = (from comp in _selComponents
                            from attrAll in comp.GetUserAttributes()
                            where attrAll.Title.ToUpper() == "MATERIAL"
                            let attrValue = comp.GetStringUserAttribute(attrAll.Title, -1)
                            // Revision 1.02 – 2018 / 01 / 10
                            where attrValue != null
                            let value = attrValue.ToUpper()
                            where value == "HRS PLT" || value == "4140 PLT" || value == "4140 PH PLT"
                            select comp).ToList();
                        if (checkBoxBurnoutSheet.Checked)
                            CreateBurnoutSheet(burnComponent);

                        if (checkBoxDeleteBurnout.Checked)
                            foreach (Component burnComp in burnComponent)
                            {
                                Part compProto = (Part)burnComp.Prototype;
                                ufsession_.Part.SetDisplayPart(compProto.Tag);
                                List<NXObject> deleteView = __display_part_.DrawingSheets
                                    .Cast<DrawingSheet>()
                                    .Where(dwg => dwg.Name == "BURNOUT")
                                    .Cast<NXObject>()
                                    .ToList();
                                session_.__DeleteObjects(deleteView.ToArray());
                            }
                    }
                }

                checkBoxBurnoutSheet.Checked = false;
                checkBoxDeleteBurnout.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void SetAnnotationPreferences()
        {
            LetteringPreferences letteringPreferences1 = __work_part_.Annotations.Preferences.GetLetteringPreferences();

            TextCfw cfw = new TextCfw
            {
                Color = 7,
                Font = 1,
                Width = LineWidth.Thin
            };
            Lettering dimensionText1 = new Lettering
            {
                Size = 0.5,
                CharacterSpaceFactor = 0.9,
                AspectRatio = 0.65,
                LineSpaceFactor = 1.0,
                Cfw = cfw,
                Italic = false
            };
            letteringPreferences1.SetDimensionText(dimensionText1);

            Lettering appendedText1 = new Lettering
            {
                Size = 0.5,
                CharacterSpaceFactor = 0.9,
                AspectRatio = 0.65,
                LineSpaceFactor = 1.0,
                Cfw = cfw,
                Italic = false
            };
            letteringPreferences1.SetAppendedText(appendedText1);

            Lettering toleranceText1 = new Lettering
            {
                Size = 0.125,
                CharacterSpaceFactor = 0.9,
                AspectRatio = 0.65,
                LineSpaceFactor = 1.0,
                Cfw = cfw,
                Italic = false
            };
            letteringPreferences1.SetToleranceText(toleranceText1);

            Lettering generalText1 = new Lettering
            {
                Size = 0.5,
                CharacterSpaceFactor = 0.9,
                AspectRatio = 0.65,
                LineSpaceFactor = 1.0,
                Cfw = cfw,
                Italic = false
            };
            letteringPreferences1.SetGeneralText(generalText1);

            __work_part_.Annotations.Preferences.SetLetteringPreferences(letteringPreferences1);

            letteringPreferences1.Dispose();

            LineAndArrowPreferences lineAndArrowPreferences1 =
                __work_part_.Annotations.Preferences.GetLineAndArrowPreferences();

            LineCfw firstExtensionLineCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetFirstExtensionLineCfw(firstExtensionLineCfw1);

            LineCfw firstArrowheadCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetFirstArrowheadCfw(firstArrowheadCfw1);

            LineCfw firstArrowLineCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetFirstArrowLineCfw(firstArrowLineCfw1);

            LineCfw secondExtensionLineCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetSecondExtensionLineCfw(secondExtensionLineCfw1);

            LineCfw secondArrowheadCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetSecondArrowheadCfw(secondArrowheadCfw1);

            LineCfw secondArrowLineCfw1 = new LineCfw(7, DisplayableObject.ObjectFont.Solid, LineWidth.Thin);
            lineAndArrowPreferences1.SetSecondArrowLineCfw(secondArrowLineCfw1);

            __work_part_.Annotations.Preferences.SetLineAndArrowPreferences(lineAndArrowPreferences1);

            lineAndArrowPreferences1.Dispose();

            DimensionPreferences dimensionPreferences1 = __work_part_.Annotations.Preferences.GetDimensionPreferences();
            dimensionPreferences1.TextPlacement = TextPlacement.Automatic;
            dimensionPreferences1.TrimDimensionLineStyle = TrimDimensionLineStyle.Trim;
            __work_part_.Annotations.Preferences.SetDimensionPreferences(dimensionPreferences1);
            dimensionPreferences1.Dispose();

            UFDrf.UnitsFormatPreferences unitsFormatPreferences1 = new UFDrf.UnitsFormatPreferences
            {
                dimension_linear_units = UFDrf.LinearUnits.Inches,
                linear_fraction_type = UFDrf.FractionType.Decimal,
                decimal_point_character = UFDrf.DecimalPointCharacter.DecimalPointPeriod,
                display_trailing_zeros = true,
                dimension_tolerance_placement = UFDrf.TolerancePlacement.ToleranceAfterDimension,
                dimension_angular_format = UFDrf.AngularUnits.FractionalDegrees,
                dim_angular_format_tolerance = UFDrf.AngularUnits.FractionalDegrees,
                angular_suppress_zeros = UFDrf.AngularSuppressZeros.AngDisplayZeros,
                dual_dimension_format = UFDrf.DualDimensionFormat.NoDualDimension,
                dual_dimension_units = UFDrf.LinearUnits.Inches,
                dual_fraction_type = UFDrf.FractionType.Decimal,
                dual_convert_tolerance = true
            };
            ufsession_.Drf.SetUnitsFormatPreferences(ref unitsFormatPreferences1);
        }

        private static void SetViewPreferences()
        {
            __work_part_.ViewPreferences.HiddenLines.HiddenlineColor = 0;
            __work_part_.ViewPreferences.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Dashed;
            __work_part_.ViewPreferences.VisibleLines.VisibleColor = 0;
            __work_part_.ViewPreferences.VisibleLines.VisibleWidth = NXOpen.Preferences.Width.Original;
            __work_part_.ViewPreferences.SmoothEdges.SmoothEdgeColor = 0;
            __work_part_.ViewPreferences.SmoothEdges.SmoothEdgeFont = NXOpen.Preferences.Font.Invisible;
            __work_part_.ViewPreferences.SmoothEdges.SmoothEdgeWidth = NXOpen.Preferences.Width.Original;
            __work_part_.ViewPreferences.General.AutomaticAnchorPoint = false;
            __work_part_.ViewPreferences.General.Centerlines = false;
            __work_part_.Preferences.ColorSettingVisualization.MonochromeDisplay = false;
        }

        private static void CreateBurnoutSheet(List<Component> nxCompList)
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "CreateOrNull Burnout");

            if (nxCompList.Count == 0)
            {
                print_("");
                print_("There are no burnouts in this assembly");
                return;
            }

            bool isJobNumber = false;
            bool isQty = false;
            bool isDetailNumber = false;
            bool isMaterial = false;
            bool isDescription = false;
            bool isShop = false;

            foreach (Component comp in nxCompList)
            {
                if (!(comp.Prototype is Part))
                {
                    print_("Component: " + comp.DisplayName + " is not fully loaded.");
                    continue;
                }

                Part snapPart = (Part)comp.Prototype;

                bool ValidateExpression(string str)
                {
                    Expression exp = snapPart.__FindExpression(str);

                    if (exp == null)
                    {
                        print_("Component \"" + snapPart.Leaf + "\" is missing expression \"" + str + "\".");
                        return false;
                    }

                    if (exp.RightHandSide != null)
                        return true;

                    print_("Component \"" + snapPart.Leaf + "\" has an invalid valid for expression \"" + str + "\".");
                    return false;
                }

                bool addX = ValidateExpression("AddX");
                bool addY = ValidateExpression("AddY");
                bool addZ = ValidateExpression("AddZ");

                if (!addX || !addY || !addZ)
                    continue;

                foreach (NXObject.AttributeInformation attrBurn in comp.GetUserAttributes())
                {
                    string title = attrBurn.Title.ToUpper();

                    switch (title)
                    {
                        case "JOB NUMBER":
                            string jobNumber =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (jobNumber != "")
                                isJobNumber = true;
                            break;
                        case "QTY":
                            string qty =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (qty != "")
                                isQty = true;
                            break;
                        case "DETAIL NUMBER":
                            string detNumber =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (detNumber != "")
                                isDetailNumber = true;
                            break;
                        case "MATERIAL":
                            string material =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (material != "")
                                isMaterial = true;
                            break;
                        case "DESCRIPTION":
                            string description =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (description != "")
                                isDescription = true;
                            break;
                        case "SHOP":
                            string shop =
                                comp.GetUserAttributeAsString(attrBurn.Title, NXObject.AttributeType.String, -1);
                            if (shop != "")
                                isShop = true;
                            break;
                    }
                }

                if (isJobNumber && isQty && isDetailNumber && isMaterial && isDescription && isShop)
                {
                    Part compProto = (Part)comp.Prototype;
                    ufsession_.Part.SetDisplayPart(compProto.Tag);

                    __display_part_.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);

                    // CreateOrNull burnout drawing sheet

                    bool isBurnout = false;

                    foreach (DrawingSheet dwgSheet in __display_part_.DrawingSheets)
                        if (dwgSheet.Name == "BURNOUT")
                            isBurnout = true;

                    if (isBurnout == false)
                    {
                        __work_part_.DraftingDrawingSheets.InsertSheet("BURNOUT", DrawingSheet.Unit.Inches, 150.0,
                            250.0, 1.0, 1.0,
                            DrawingSheet.ProjectionAngleType.ThirdAngle);

                        Tag burnoutDwg = NXOpen.Tag.Null;
                        ufsession_.Obj.CycleByNameAndType(__display_part_.Tag, "BURNOUT", UFConstants.UF_drawing_type,
                            false, ref burnoutDwg);

                        ufsession_.Draw.AskCurrentDrawing(out Tag currentDwg);

                        ufsession_.Obj.AskName(currentDwg, out string dwgName);

                        if (dwgName != "BURNOUT")
                            ufsession_.Draw.OpenDrawing(burnoutDwg);

                        // Set lettering and view preferences

                        SetAnnotationPreferences();
                        SetViewPreferences();

                        ufsession_.Draw.SetBorderDisplay(false);

                        // Import plan view


                        BaseViewBuilder baseViewBuilder1 = __work_part_.DraftingViews.CreateBaseViewBuilder(null);

                        bool isPlan = false;

                        foreach (ModelingView mView in __work_part_.ModelingViews)
                        {
                            if (mView.Name.ToUpper() != "PLAN") continue;
                            isPlan = true;
                            baseViewBuilder1.SelectModelView.SelectedView = mView;
                        }

                        if (isPlan == false)
                            foreach (ModelingView mView in __work_part_.ModelingViews)
                                if (mView.Name.ToUpper() == "TOP")

                                    baseViewBuilder1.SelectModelView.SelectedView = mView;

                        Point3d point1 = new Point3d(125.0, 75.0, 0.0);
                        baseViewBuilder1.Placement.Placement.SetValue(null, __work_part_.Views.WorkView, point1);

                        NXObject nXObject1 = baseViewBuilder1.Commit();

                        baseViewBuilder1.Destroy();

                        // Set layers visible in view

                        __display_part_.Layers.SetState(100, State.WorkLayer);

                        View importedViewObj = (View)nXObject1;

                        StateInfo[] layerVisibleInView = new StateInfo[256];

                        for (int i = 0; i < layerVisibleInView.Length - 1; i++)
                        {
                            layerVisibleInView[i].Layer = i + 1;
                            layerVisibleInView[i].State = State.Hidden;
                        }

                        layerVisibleInView[99].Layer = 100;
                        layerVisibleInView[99].State = State.Visible;

                        __work_part_.Layers.SetObjectsVisibilityOnLayer(importedViewObj, layerVisibleInView, true);

                        DraftingView[] draftingViews = __work_part_.DraftingViews.ToArray();
                        __work_part_.DraftingViews.UpdateViews(draftingViews);

                        // Add annotation note

                        Session.UndoMarkId markId3 = session_.SetUndoMark(Session.MarkVisibility.Invisible,
                            "CreateOrNull Annotation");

                        LetteringPreferences letteringPreferences2 =
                            __work_part_.Annotations.Preferences.GetLetteringPreferences();

                        UserSymbolPreferences userSymbolPreferences2 =
                            __work_part_.Annotations.NewUserSymbolPreferences(
                                UserSymbolPreferences.SizeType.ScaleAspectRatio, 1.0,
                                1.0);

                        string[] textLines1 = new string[6];
                        textLines1[0] = "<W@SHOP> JOB NO.: <W@JOB NUMBER>";
                        textLines1[1] = "QTY: <W@QTY>";
                        textLines1[2] = "DET #: <W@DETAIL NUMBER>";
                        textLines1[3] = "MATL: <W@MATERIAL>";
                        textLines1[4] = "SIZE: <W@DESCRIPTION>";
                        textLines1[5] = "SEND ALL SLUGS TO <W@SHOP>";
                        Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
                        Note note1 = __work_part_.Annotations.CreateNote(textLines1, origin2,
                            AxisOrientation.Horizontal,
                            letteringPreferences2,
                            userSymbolPreferences2);

                        string[] lines1 = new string[6];
                        lines1[0] = "<W@SHOP> JOB NO.: <W@JOB NUMBER>";
                        lines1[1] = "QTY: <W@QTY>";
                        lines1[2] = "DET #: <W@DETAIL NUMBER>";
                        lines1[3] = "MATL: <W@MATERIAL>";
                        lines1[4] = "SIZE: <W@DESCRIPTION>";
                        lines1[5] = "SEND ALL SLUGS TO <W@SHOP>";
                        note1.SetText(lines1);
                        Point3d origin3 = new Point3d(125.0, 100.0, 0.0);
                        note1.AnnotationOrigin = origin3;

                        session_.UpdateManager.DoUpdate(markId3);

                        Session.UndoMarkId markId4 = session_.SetUndoMark(Session.MarkVisibility.Invisible,
                            "CreateOrNull Annotation");

                        string[] textLines2 = new string[6];
                        textLines2[0] = "<W@SHOP> JOB NO.: <W@JOB NUMBER>";
                        textLines2[1] = "QTY: <W@QTY>";
                        textLines2[2] = "DET #: <W@DETAIL NUMBER>";
                        textLines2[3] = "MATL: <W@MATERIAL>";
                        textLines2[4] = "SIZE: <W@DESCRIPTION>";
                        textLines2[5] = "SEND ALL SLUGS TO <W@SHOP>";
                        Point3d origin4 = new Point3d(125.0, 100.0, 0.0);
                        Note note2 = __work_part_.Annotations.CreateNote(textLines2, origin4,
                            AxisOrientation.Horizontal,
                            letteringPreferences2,
                            userSymbolPreferences2);

                        letteringPreferences2.Dispose();
                        userSymbolPreferences2.Dispose();

                        session_.UpdateManager.AddToDeleteList(note2);

                        session_.UpdateManager.DoUpdate(markId4);

                        session_.DeleteUndoMark(markId3, "CreateOrNull Annotation");

                        session_.DeleteUndoMark(markId4, "CreateOrNull Annotation");

                        __display_part_.Layers.SetState(1, State.WorkLayer);
                        __display_part_.Layers.SetState(100, State.Selectable);
                        __display_part_.Layers.SetState(200, State.Selectable);
                        __display_part_.Layers.SetState(230, State.Selectable);
                    }
                    else
                    {
                        print_("Burnout drawing already exists " + comp.DisplayName);
                    }
                }
                else
                {
                    if (!isDescription)
                        print_($"Attribute Error {comp.DisplayName}: DESCRIPTION");
                    if (!isDetailNumber)
                        print_($"Attribute Error {comp.DisplayName}: DETAIL NUMBER");
                    if (!isJobNumber)
                        print_($"Attribute Error {comp.DisplayName}: JOB NUMBER");
                    if (!isMaterial)
                        print_($"Attribute Error {comp.DisplayName}: MATERIAL");
                    if (!isQty)
                        print_($"Attribute Error {comp.DisplayName}: QTY");
                    if (!isShop)
                        print_($"Attribute Error {comp.DisplayName}: SHOP");

                    return;
                }
            }

            string burnoutList = nxCompList.Aggregate("", (current, comp) => $"{current}{comp.DisplayName}\n");
            print_("");
            print_("==================================================================================");
            print_("List of all burnout components:");
            print_("==================================================================================");
            print_("");
            print_(burnoutList);
            print_("");
            print_("==================================================================================");
        }

        private void CreateBurnoutForm_Load(object sender, EventArgs e)
        {
            Text = $"{AssemblyFileVersion} - Create Burnout";
            Location = Properties.Settings.Default.create_burnout_form_window_location;
        }

        private void CreateBurnoutForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Properties.Settings.Default.create_burnout_form_window_location = Location;
            Properties.Settings.Default.Save();
        }
    }
}
// 546