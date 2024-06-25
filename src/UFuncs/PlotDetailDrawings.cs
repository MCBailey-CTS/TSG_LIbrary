using NXOpen.CAE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class PlotDetailDrawings : _UFuncForm
    {
        private static NXOpen.Part workPart = session_.Parts.Work;
        private static NXOpen.Part displayPart = session_.Parts.Display;
        private static NXOpen.Part originalDisplayPart = displayPart;
        private static readonly List<NXOpen.Assemblies.Component> children = new List<NXOpen.Assemblies.Component>();
        private static List<NXOpen.Assemblies.Component> selectedComponents = new List<NXOpen.Assemblies.Component>();
        private static readonly List<string> displayName = new List<string>();


        public PlotDetailDrawings()
        {
            InitializeComponent();
        }

        private void CheckBoxPrintAll_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPrintSelected.Enabled = !checkBoxPrintAll.Checked && !checkBoxPrintAssem.Checked;
            checkBoxPrintSelected.Checked = false;
        }

        private void CheckBoxPrintSelected_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPrintAll.Enabled = !checkBoxPrintSelected.Checked;
            checkBoxPrintAll.Checked = false;

            checkBoxPrintAssem.Enabled = !checkBoxPrintSelected.Checked;
            checkBoxPrintAssem.Checked = false;
        }

        private void CheckBoxPrintAssem_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxPrintSelected.Enabled = !checkBoxPrintAssem.Checked && !checkBoxPrintAll.Checked;
            checkBoxPrintSelected.Checked = false;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            workPart = session_.Parts.Work;
            displayPart = session_.Parts.Display;
            UpdateOriginalParts();
            //UnHighlight.Program.Execute();
            selectedComponents.Clear();
            children.Clear();
            displayName.Clear();

            try
            {
                if (checkBoxPrintAll.Checked)
                {
                    if (displayPart.ComponentAssembly.RootComponent != null)
                    {
                        GetChildComponents(displayPart.ComponentAssembly.RootComponent);

                        if (children.Count != 0)
                        {
                            HashSet<NXOpen.Assemblies.Component> getOneComp = new HashSet<NXOpen.Assemblies.Component>(children, new EqualityDisplayName());

                            foreach (NXOpen.Assemblies.Component comp in getOneComp)
                            {
                                selectedComponents.Add(comp);
                                displayName.Add(comp.DisplayName);
                            }

                            if (selectedComponents.Count != 0)
                            {
                                displayName.Sort();

                                foreach (string name in displayName)
                                    foreach (NXOpen.Assemblies.Component comp in selectedComponents)
                                    {
                                        if (name != comp.DisplayName) continue;
                                        SetDisplayPart(comp);
                                        SetDefaultLayers();
                                        PrintDetailDrawing(name);
                                    }
                            }
                        }
                    }

                    session_.Parts.SetDisplay(originalDisplayPart, false, false, out NXOpen.PartLoadStatus partLoadStatus1);
                    partLoadStatus1.Dispose();

                    workPart = session_.Parts.Work;
                    displayPart = session_.Parts.Display;
                    selectedComponents.Clear();
                    children.Clear();
                    displayName.Clear();
                }

                if (checkBoxPrintSelected.Checked)
                {
                    Hide();

                    selectedComponents = Ui.Selection.SelectComponents().ToList();

                    Show();

                    if (selectedComponents.Count != 0)
                    {
                        HashSet<NXOpen.Assemblies.Component> getOneComp = new HashSet<NXOpen.Assemblies.Component>(selectedComponents, new EqualityDisplayName());

                        foreach (NXOpen.Assemblies.Component comp in getOneComp) displayName.Add(comp.DisplayName);

                        if (selectedComponents.Count != 0)
                        {
                            displayName.Sort();

                            foreach (string name in displayName)
                                foreach (NXOpen.Assemblies.Component comp in getOneComp)
                                {
                                    if (name != comp.DisplayName) continue;
                                    SetDisplayPart(comp);
                                    SetDefaultLayers();
                                    PrintDetailDrawing(name);
                                }
                        }
                    }

                    session_.Parts.SetDisplay(originalDisplayPart, false, false, out NXOpen.PartLoadStatus partLoadStatus1);
                    partLoadStatus1.Dispose();

                    workPart = session_.Parts.Work;
                    displayPart = session_.Parts.Display;
                    selectedComponents.Clear();
                    children.Clear();
                    displayName.Clear();
                }

                if (checkBoxPrintAssem.Checked)
                {
                    if (displayPart.ComponentAssembly.RootComponent != null)
                        if (children.Count != 0)
                        {
                            foreach (NXOpen.Assemblies.Component comp in children)
                            {
                                selectedComponents.Add(comp);
                                displayName.Add(comp.DisplayName);
                            }

                            if (selectedComponents.Count != 0)
                            {
                                displayName.Sort();

                                foreach (string name in displayName)
                                    foreach (NXOpen.Assemblies.Component comp in selectedComponents)
                                    {
                                        if (name != comp.DisplayName) 
                                            continue;

                                        SetDisplayPart(comp);
                                        PrintDetailDrawing(name);
                                    }
                            }
                        }

                    session_.Parts.SetDisplay(originalDisplayPart, false, false, out NXOpen.PartLoadStatus partLoadStatus1);
                    partLoadStatus1.Dispose();

                    workPart = session_.Parts.Work;
                    displayPart = session_.Parts.Display;
                    selectedComponents.Clear();
                    children.Clear();
                    displayName.Clear();
                }

                workPart = session_.Parts.Work;
                displayPart = session_.Parts.Display;
                UpdateOriginalParts();


                //UnHighlight.Program.Execute();

                checkBoxPrintAll.Checked = false;
                checkBoxPrintSelected.Checked = false;
                checkBoxPrintAssem.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        //===========================================
        //  Methods
        //===========================================

        private static void PrintDetailDrawing(string displayPartName)
        {
            NXOpen.Tag drawing = NXOpen.Tag.Null;
            //string drawingName = "SHT1"; CTS EDIT
            const string drawingName = "4-VIEW";

            NXOpen.Tag part = TheUFSession.Part.AskDisplayPart();
            TheUFSession.Obj.CycleByNameAndType(part, drawingName, NXOpen.UF.UFConstants.UF_drawing_type, false, ref drawing);
            if (drawing == NXOpen.Tag.Null)
            {
                print_("No drawing named 4-VIEW in part " + displayPartName + "\n");
            }
            else
            {
                session_.ApplicationSwitchImmediate("UG_APP_DRAFTING");

                NXOpen.Drawings.DrawingSheet sheet = (NXOpen.Drawings.DrawingSheet)session_.GetObjectManager().GetTaggedObject(drawing);

                sheet.Open();

                //sheets[0].Open();
                ufsession_.Draw.IsObjectOutOfDate(sheet.Tag, out bool outOfDate);
                if (outOfDate)
                    try
                    {
                        TheUFSession.Draw.UpdOutOfDateViews(sheet.Tag);
                        sheet.OwningPart.Save(NXOpen.BasePart.SaveComponents.True, NXOpen.BasePart.CloseAfterSave.False);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                NXOpen.PrintBuilder printBuilder1 = workPart.PlotManager.CreatePrintBuilder();

                printBuilder1.Copies = 1;
                printBuilder1.ThinWidth = 1.0;
                printBuilder1.NormalWidth = 2.0;
                printBuilder1.ThickWidth = 3.0;
                printBuilder1.Output = NXOpen.PrintBuilder.OutputOption.WireframeBlackWhite;
                printBuilder1.ShadedGeometry = true;
                NXOpen.NXObject[] sheets1 = new NXOpen.NXObject[1];
#pragma warning disable CS0618 // Type or member is obsolete
                NXOpen.Drawings.DrawingSheet drawingSheet1 = workPart.DrawingSheets.FindObject("4-VIEW");
#pragma warning restore CS0618 // Type or member is obsolete
                sheets1[0] = drawingSheet1;
                printBuilder1.SourceBuilder.SetSheets(sheets1);
                printBuilder1.PrinterText = __PrinterCts;
                printBuilder1.Orientation = NXOpen.PrintBuilder.OrientationOption.Landscape;
                printBuilder1.Paper = NXOpen.PrintBuilder.PaperSize.Letter;

                printBuilder1.Commit();

                printBuilder1.Destroy();
            }
        }


        private static void SetDisplayPart(NXOpen.Assemblies.Component comp)
        {
            NXOpen.Part part1 = (NXOpen.Part)comp.Prototype;
            session_.Parts.SetDisplay(part1, false, false, out NXOpen.PartLoadStatus partLoadStatus1);

            workPart = session_.Parts.Work;
            displayPart = session_.Parts.Display;
            partLoadStatus1.Dispose();
        }


        private void GetChildComponents(NXOpen.Assemblies.Component assembly)
        {
            try
            {
                if (assembly.GetChildren() == null) return;
                foreach (NXOpen.Assemblies.Component child in assembly.GetChildren())
                {
                    if (child.IsSuppressed) continue;
                    bool isValid = child.DisplayName.__IsDetail();

                    if (isValid)
                    {
                        NXOpen.Tag instance = TheUFSession.Assem.AskInstOfPartOcc(child.Tag);

                        if (instance == NXOpen.Tag.Null) continue;
                        TheUFSession.Assem.AskPartNameOfChild(instance, out string partName);
                        int partLoad = TheUFSession.Part.IsLoaded(partName);

                        if (partLoad == 1)
                        {
                            children.Add(child);
                            GetChildComponents(child);
                        }
                        else
                        {
                            TheUFSession.Part.OpenQuiet(partName, out NXOpen.Tag partOpen, out _);
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


        private static void SetDefaultLayers()
        {
            displayPart.Layers.SetState(1, NXOpen.Layer.State.WorkLayer);

            NXOpen.Layer.StateCollection layerState = displayPart.Layers.GetStates();

            foreach (NXOpen.Layer.Category category in displayPart.LayerCategories)
                if (category.Name == "ALL")
                    layerState.SetStateOfCategory(category, NXOpen.Layer.State.Hidden);

            displayPart.Layers.SetStates(layerState, true);

            layerState.Dispose();

            displayPart.Layers.SetState(100, NXOpen.Layer.State.Selectable);
            displayPart.Layers.SetState(200, NXOpen.Layer.State.Selectable);
            displayPart.Layers.SetState(230, NXOpen.Layer.State.Selectable);
        }

        //private static void UpdateSessionParts()
        //{
        //    workPart = session_.Parts.Work;
        //    displayPart = session_.Parts.Display;
        //}

        private static void UpdateOriginalParts()
        {
            originalDisplayPart = displayPart;
        }

        private void PlotDetailDrawings_Load(object sender, EventArgs e)
        {
            Text = AssemblyFileVersion;
            Location = Properties.Settings.Default.plot_detail_drawings_location;
        }

        private void PlotDetailDrawings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.plot_detail_drawings_location = Location;
            Properties.Settings.Default.Save();
        }
    }
}
