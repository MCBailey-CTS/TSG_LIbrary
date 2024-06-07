using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using NXOpen.Preferences;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    [RevisionLog("Wire Start Hole")]
    [RevisionEntry("1.0", "2016", "03", "03")]
    [Revision("1.0.1", "Released as a single ufunc that does all 3 sizes and added option to Extrude/Subtract.")]
    [RevisionEntry("1.1", "2016", "05", "18")]
    [Revision("1.1.1", "Updated to add start hole at current wcs instead of absolute of part.")]
    [RevisionEntry("2.00", "2017", "06", "05")]
    [Revision("2.00.1", "Recompiled for NX.")]
    [RevisionEntry("2.01", "2017", "08", "16")]
    [Revision("2.01.1", "Fixed issue with not finding a dynamic block with the Extrude/Subtract check box turned on.")]
    [RevisionEntry("2.02", "2017", "08", "22")]
    [Revision("2.02.1", "Signed so it can run outside of CTS")]
    [RevisionEntry("2.03", "2017", "09", "08")]
    [Revision("2.03.1", "Added validation check")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    [UFunc(ufunc_wire_start_hole)]
    public partial class WireStartHoleForm : _UFuncForm
    {
        private const string WireStartPath125 = "G:\\0Library\\CtsComponents\\-125_START_HOLE.prt";
        private const string WireStartPath250 = "G:\\0Library\\CtsComponents\\-250_START_HOLE.prt";
        private const string WireStartPath375 = "G:\\0Library\\CtsComponents\\-375_START_HOLE.prt";

        public int scope_identity;

        public WireStartHoleForm()
        {
            InitializeComponent();
            rdo250.Checked = true;
        }

        private static Part __display_part_ => Session.GetSession().Parts.Display;

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if(__display_part_ is null)
                {
                    print_("No DisplayPart");
                    return;
                }

                if(!__display_part_.__HasDynamicBlock())
                {
                    print_("No Dynamic Block");
                    return;
                }

                var dynamic_block = __display_part_.__DynamicBlock();

                if(chkSubtract.Checked)
                    if(!__work_part_.__HasDynamicBlock())
                    {
                        print_("Current Work Part doesn't not contain a dynamic block");

                        return;
                    }
                    else
                    {
                        __display_part_.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                    }

                var workPlane1 = __display_part_.Preferences.Workplane;

                using (session_.using_form_show_hide(this))
                {
                    try
                    {
#pragma warning disable CS0612 // Type or member is obsolete
                        Execute(dynamic_block, workPlane1);
#pragma warning restore CS0612 // Type or member is obsolete
                    }
                    catch (Exception ex)
                    {
                        ex._PrintException();
                    }
                    finally
                    {
                        workPlane1.ShowGrid = false;
                        workPlane1.SnapToGrid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        [Obsolete]
        private void Execute(Block dynamicBlock, WorkPlane workPlane1)
        {
            //            NXOpen.Session.GetSession().SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Add Wire Start");
            //            const double gridSpace = .0625;
            //            workPlane1.GridType = NXOpen.Preferences.WorkPlane.Grid.Rectangular;
            //            workPlane1.GridIsNonUniform = false;
            //            NXOpen.Preferences.WorkPlane.GridSize gridSize1 = new NXOpen.Preferences.WorkPlane.GridSize(gridSpace, 1, 1);
            //            workPlane1.SetRectangularUniformGridSize(gridSize1);
            //            workPlane1.ShowGrid = true;
            //            workPlane1.ShowLabels = false;
            //            workPlane1.SnapToGrid = true;
            //            workPlane1.GridOnTop = false;
            //            workPlane1.RectangularShowMajorLines = false;
            //            workPlane1.PolarShowMajorLines = false;
            //            workPlane1.GridColor = 7;
            //#pragma warning disable 618
            //            NXOpen.Session.GetSession().Preferences.WorkPlane.ObjectOffWorkPlane = NXOpen.Preferences.SessionWorkPlane.ObjectDisplay.Normal;
            //#pragma warning restore 618
            //            // set layer 94 to selectable
            //            __work_part_.Layers.SetState(94, NXOpen.Layer.State.Selectable);
            //            // get user input for location
            //            bool isPicked;
            //            do
            //            {
            //                NXOpen.Selection.DialogResponse cursorSelection = NXOpen.UI.GetUI().SelectionManager
            //                    .SelectScreenPosition(
            //                    "Pick a location to place start hole",
            //                    out _,
            //                    out NXOpen.Point3d screenPosition);

            //                isPicked = cursorSelection == NXOpen.Selection.DialogResponse.Pick;

            //                if (!isPicked)
            //                    continue;

            //                double radius = 0;

            //                if (rdo125.Checked)
            //                    if (chkSubtract.Checked)
            //                        radius = 0.06250000045;
            //                    else
            //                        ImportWireStart(WireStartPath125, screenPosition);
            //                else if (rdo250.Checked)
            //                    if (chkSubtract.Checked)
            //                        radius = 0.12500000045;
            //                    else
            //                        ImportWireStart(WireStartPath250, screenPosition);
            //                else if (chkSubtract.Checked)
            //                    radius = 0.18750000045;
            //                else
            //                    ImportWireStart(WireStartPath375, screenPosition);

            //                if (!chkSubtract.Checked)
            //                    continue;

            //                NXOpen.Matrix3x3 wcsOr = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            //                NXOpen.Arc arc = __display_part_.Circle(screenPosition, wcsOr, radius);
            //                NXOpen.Curve[] curves = new NXOpen.Curve[] { arc };
            //                NXOpen.Vector3d axisZ = __display_part_.WCS._AxisZ();
            //                NXOpen.Vector3d direction = axisZ;
            //                double start = 10d;
            //                double end = -10d;
            //                NXOpen.Features.Extrude extrude = __work_part_.create_extrude(curves, direction, start, end);


            //                // todo: need to color extrude and set extrude to layer 94
            //                //extrude.._SetDisplayColor(211);

            //                //extrude.layer = 94;
            //                arc.Layer = 94;
            //                arc.RedisplayObject();

            //                NXOpen.Face cylinderFace = extrude.GetFaces().SingleOrDefault(face => face.SolidFaceType == NXOpen.Face.FaceType.Cylindrical);

            //                if (cylinderFace is null)
            //                {
            //                    print_("Was unable to find a cylinder face.");
            //                    continue;
            //                }

            //                cylinderFace.SetName("HOLECHART");

            //                __work_part_.create_subtract(dynamicBlock.GetBodies()[0], extrude.GetBodies());
            //            } while (isPicked);
            throw new NotImplementedException();
        }

        private static void ImportWireStart(string path, Point3d location)
        {
            Importer importer1 = __work_part_.ImportManager.CreatePartImporter();

            using (session_.using_builder_destroyer(importer1))
            {
                var partImporter1 = (PartImporter)importer1;
                partImporter1.FileName = path;
                partImporter1.Scale = 1.0;
                partImporter1.CreateNamedGroup = false;
                partImporter1.ImportViews = false;
                partImporter1.ImportCamObjects = false;
                partImporter1.LayerOption = PartImporter.LayerOptionType.Original;
                partImporter1.DestinationCoordinateSystemSpecification =
                    PartImporter.DestinationCoordinateSystemSpecificationType.Work;
                var element1 = __work_part_.WCS.CoordinateSystem.Orientation.Element;
                var nXMatrix1 = __work_part_.NXMatrices.Create(element1);
                partImporter1.DestinationCoordinateSystem = nXMatrix1;
                var destinationPoint1 = location;
                partImporter1.DestinationPoint = destinationPoint1;
                partImporter1.Commit();
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WireStartHoleForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.wire_start_hole_form_window_location;
        }

        private void WireStartHoleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.wire_start_hole_form_window_location = Location;
            Settings.Default.Save();
        }
    }
}