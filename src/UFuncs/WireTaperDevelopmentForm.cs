using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.Drawings;
using NXOpen.Layer;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using static TSG_Library.UFuncs._UFunc;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_wire_taper_development)]
    public partial class WireTaperDevelopmentForm : _UFuncForm
    {
        private static List<Component> allComponents = new List<Component>();

        private static List<Component> selComponents = new List<Component>();

        public WireTaperDevelopmentForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = AssemblyFileVersion;
            radioButtonWtnNone.Checked = true;
            radioButtonTrimDevNone.Checked = true;
            Location = Properties.Settings.Default.wire_taper_development_form_window_location;
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            try
            {
                allComponents = Selection.SelectManyComponents().ToList();

                if (allComponents.Count != 0)
                {
                    selComponents = new HashSet<Component>(allComponents.Distinct(new EqualityDisplayName())).ToList();

                    if (selComponents.Count != 0)
                    {
                        using (session_.__UsingDisplayPartReset())
                        {
                            foreach (Component comp in selComponents.Select(__c => __c))
                            {
                                __display_part_ = (Part)comp.Prototype;

                                SetImportNoteLayers(__display_part_);

                                const int modeling = 1;
                                TheUFSession.Draw.SetDisplayState(modeling);
                                __display_part_.ModelingViews.WorkView.RenderingStyle =
                                    View.RenderingStyleType.StaticWireframe;

                                // open 4-VIEW  and update all views

                                bool is4View = false;

                                foreach (DrawingSheet drwSheet in __display_part_.DrawingSheets)
                                {
                                    if (drwSheet.Name != "4-VIEW") continue;
                                    drwSheet.Open();

                                    is4View = true;

                                    foreach (DraftingView drfView in drwSheet.GetDraftingViews())
                                        drfView.Update();
                                }

                                // set attribute values from form

                                if (radioButtonAddWtn.Checked)
                                    __display_part_.__SetAttribute("WTN", "YES");
                                if (radioButtonAddWfft.Checked)
                                    __display_part_.__SetAttribute("WFTD", "YES");
                                if (radioButtonRemoveWtn.Checked)
                                    __display_part_.__SetAttribute("WTN", "NO");
                                if (radioButtonRemoveWfft.Checked)
                                    __display_part_.__SetAttribute("WFTD", "NO");

                                // delete selected notes from 4-VIEW

                                if (!is4View) continue;
                                List<NXObject> addToDelete = new List<NXObject>();

                                foreach (Note drfNote in __display_part_.Notes)
                                {
                                    if (drfNote.Layer != 200) continue;
                                    if (radioButtonAddWtn.Checked)
                                    {
                                        string[] noteText = drfNote.GetText();

                                        if (noteText[0].Contains("TSG STANDARD"))
                                            addToDelete.Add(drfNote);
                                    }

                                    if (radioButtonRemoveWtn.Checked)
                                    {
                                        string[] noteText = drfNote.GetText();

                                        if (noteText[0].Contains("TSG STANDARD"))
                                            addToDelete.Add(drfNote);
                                    }

                                    if (radioButtonAddWfft.Checked)
                                    {
                                        string[] noteText = drfNote.GetText();

                                        if (noteText[0].Contains("WAITING FOR FINAL TRIM"))
                                            addToDelete.Add(drfNote);
                                    }

                                    if (!radioButtonRemoveWfft.Checked) continue;
                                    {
                                        string[] noteText = drfNote.GetText();

                                        if (noteText[0].Contains("WAITING FOR FINAL TRIM"))
                                            addToDelete.Add(drfNote);
                                    }
                                }

                                if (addToDelete.Count != 0)
                                    session_.__DeleteObjects(addToDelete.ToArray());

                                // get scale expression value
                                double scale = 1.00;

                                foreach (Expression exp in __display_part_.Expressions)
                                    if (exp.Name == "borderScale")
                                        scale = exp.GetValueUsingUnits(Expression.UnitsOption.Expression);

                                // get settings from form and set attributes and import notes

                                if (radioButtonAddWtn.Checked)
                                {
                                    __display_part_.__SetAttribute("WTN", "YES");
                                    const string path = "G:\\0Library\\Drafting\\wire-note.prt";
                                    ImportNote(path, scale);
                                }

                                if (radioButtonAddWfft.Checked)
                                {
                                    __display_part_.__SetAttribute("WFTD", "YES");
                                    const string path = "G:\\0Library\\Drafting\\trim-note.prt";
                                    ImportNote(path, scale);
                                }

                                SetDefaultLayers(__display_part_);
                            }
                        }

                        const int modeling1 = 1;
                        TheUFSession.Draw.SetDisplayState(modeling1);
                    }
                }

                radioButtonWtnNone.Checked = true;
                radioButtonTrimDevNone.Checked = true;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        private static void ImportNote(string notePath, double importScale)
        {
            session_.SetUndoMark(Session.MarkVisibility.Invisible, "Import Part");

            Importer importer1 = __work_part_.ImportManager.CreatePartImporter();

            PartImporter partImporter1 = (PartImporter)importer1;
            partImporter1.FileName = notePath;
            partImporter1.Scale = importScale;
            partImporter1.CreateNamedGroup = false;
            partImporter1.ImportViews = false;
            partImporter1.ImportCamObjects = false;
            partImporter1.LayerOption = PartImporter.LayerOptionType.Work;

            partImporter1.DestinationCoordinateSystemSpecification =
                PartImporter.DestinationCoordinateSystemSpecificationType.Work;

            NXMatrix nXMatrix1 = __work_part_.NXMatrices.Create(_Matrix3x3Identity);

            partImporter1.DestinationCoordinateSystem = nXMatrix1;

            Point3d destinationPoint1 = _Point3dOrigin;
            partImporter1.DestinationPoint = destinationPoint1;

            Session.UndoMarkId markId2 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Import Part Commit");

            partImporter1.Commit();

            session_.DeleteUndoMark(markId2, null);

            partImporter1.Destroy();
        }

        private static void SetImportNoteLayers(Part part)
        {
            part.Layers.SetState(200, State.WorkLayer);

            StateCollection layerState = part.Layers.GetStates();

            foreach (Category category in part.LayerCategories)
                if (category.Name == "ALL")
                    layerState.SetStateOfCategory(category, State.Hidden);

            part.Layers.SetStates(layerState, true);

            layerState.Dispose();

            part.Layers.SetState(1, State.Selectable);
            part.Layers.SetState(94, State.Selectable);
            part.Layers.SetState(231, State.Selectable);
        }

        private static void SetDefaultLayers(Part part)
        {
            part.Layers.SetState(1, State.WorkLayer);

            StateCollection layerState = part.Layers.GetStates();

            foreach (Category category in part.LayerCategories)
                if (category.Name == "ALL")
                    layerState.SetStateOfCategory(category, State.Hidden);

            part.Layers.SetStates(layerState, true);

            layerState.Dispose();

            part.Layers.SetState(94, State.Selectable);
            part.Layers.SetState(99, State.Selectable);
            part.Layers.SetState(200, State.Selectable);
            part.Layers.SetState(230, State.Selectable);
        }

        private void WireTaperDevelopmentForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Properties.Settings.Default.wire_taper_development_form_window_location = Location;
            Properties.Settings.Default.Save();
        }
    }
}