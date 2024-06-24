using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.Positioning;
using NXOpen.Preferences;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Exceptions;
using TSG_Library.Properties;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(_UFunc.ufunc_add_fasteners)]
    public partial class AddFastenersForm1 : _UFuncForm
    {
        public const string AddFastenersSizeFile2 = @"U:\nxFiles\UfuncFiles\AddFastenerSizes.txt";

        private static BasePart.Units _unit = BasePart.Units.Millimeters;

        private static int WorkPartChangedRegister;

        public static int _translucency = -1;
        private int _1x_2x = 2;

        public AddFastenersForm1()
        {
            InitializeComponent();
        }

        public bool IsFormMetric => menuItemUnits.Text == @"MM";

        public void WorkPartChanged(BasePart basePart = null)
        {
            try
            {
                SetFormToWorkPart();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetFormToWorkPart()
        {
            try
            {
                if (__display_part_ is null)
                    return;

                bool hasSolidBodyOnLayer1 = __work_part_.Bodies
                    .ToArray()
                    .Where(__b => __b.IsSolidBody)
                    .Where(__b => __b.Layer == 1)
                    .ToArray().Length == 1;

                bool hasDynamicBlock = __work_part_.__HasDynamicBlock() || hasSolidBodyOnLayer1;
                menuItemUnits.Enabled = hasDynamicBlock;
                chkCycleAdd.Enabled = hasDynamicBlock;
                btnOrigin.Enabled = hasDynamicBlock;
                btnWireTaps.Enabled = hasDynamicBlock;
                chkReverseCycleAdd.Enabled = hasDynamicBlock;
                mnu2x.Enabled = hasDynamicBlock;
                toolStripMenuItem1.Enabled = hasDynamicBlock;
                ChangEnabled(hasDynamicBlock, listBoxSelection, grpFastenerType, btnOk, btnChangeRefSet, chkSubstitute,
                    btnPlanView);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                if (!(__display_part_ is null))
                    __display_part_.Preferences.Workplane.ShowGrid = false;

                mnuStrMainMenu.Enabled = true;
                menuItemFile.Enabled = true;
                btnSelectComponent.Enabled = true;
                btnReset.Enabled = true;
            }
        }

        private void StripWaveOut_Click(object sender, EventArgs e)
        {
            try
            {
                UFSession uf = ufsession_;

                foreach (Feature feat in __work_part_.Features.ToArray())
                    try
                    {
                        if (!(feat is ExtractFace link))
                            continue;

                        if (!link.__IsLinkedBody())
                            continue;

                        uf.Wave.IsLinkBroken(link.Tag, out bool is_broken);

                        if (is_broken)
                            continue;

                        uf.Wave.AskLinkXform(link.Tag, out Tag xform);

                        uf.So.AskAssyCtxtPartOcc(xform, __work_part_.__RootComponent().Tag, out Tag from_part_occ);

                        if (from_part_occ == NXOpen.Tag.Null)
                            continue;

                        double[] point = new double[3];

                        uf.So.AskPointOfXform(xform, point);

                        Component from_comp = (Component)session_.__GetTaggedObject(from_part_occ);

                        Point3d origin = point.__ToPoint3d();

                        if (!from_comp.__Origin().__IsEqualTo(origin))
                            continue;

                        session_.__DeleteObjects(link);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                WorkPartChanged();
            }
        }

        private void MenuUnits_Click(object sender, EventArgs e)
        {
            if (_unit == BasePart.Units.Inches)
            {
                const string mm = "MM";
                menuItemUnits.Text = mm;
                _unit = BasePart.Units.Millimeters;
            }
            else
            {
                const string @in = "IN";
                menuItemUnits.Text = @in;
                _unit = BasePart.Units.Inches;
            }

            if (rdoTypeScrew.Checked)
                LoadShcs();

            if (rdoTypeDowel.Checked)
                LoadDowel();

            if (rdoTypeJack.Checked)
                LoadJack();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Text = AssemblyFileVersion;
                switch (_1x_2x)
                {
                    case 1:
                        mnu2x.Text = "1x";
                        break;
                    case 2:
                        mnu2x.Text = "2x";
                        break;
                    default:
                        print_("Couldn't determine 1x or 2x");
                        break;
                }

                Location = Settings.Default.add_fasteners_form_window_location;
                WorkPartChangedRegister = session_.Parts.AddWorkPartChangedHandler(WorkPartChanged);
                double[] values = { 0.000d, 0.0625d, 0.125d, 0.250d, 0.500d, 0.750d, 1.000d, 4.000d, 6.000d, 10.000d };
                string[] stringValues =
                    { "Off", "0.0625", "0.125", "0.250", "0.500", "0.750", "1.000", "4.000", "6.000", "10.000" };
                List<GridSpacing> list = values.Select((t, i) => new GridSpacing(t, stringValues[i])).ToList();
                cmbGridSpacing.DataSource = null;
                cmbGridSpacing.DisplayMember = nameof(GridSpacing.StringValue);
                cmbGridSpacing.ValueMember = nameof(GridSpacing.Value);
                cmbGridSpacing.DataSource = list;
                _unit = BasePart.Units.Millimeters;
                const string mm = "MM";
                menuItemUnits.Text = mm;
                rdoTypeScrew.Checked = true;
                LoadShcs();
                Reset();
                cmbPreferred.SelectedIndex = Settings.Default.add_fasteners_preferred_index;
                chkCycleAdd.Checked = Settings.Default.add_fasteners_cycle_add;
                chkReverseCycleAdd.Checked = Settings.Default.add_fasteners_reverse_cycle_add;

                int cmb_index = Settings.Default.add_fasteners_grid_index;

                if (cmb_index == -1)
                    cmb_index = 0;

                cmbGridSpacing.SelectedIndex = cmb_index;
                chkGrid.Checked = Settings.Default.add_fasteners_show_grid;

                if (chkCycleAdd.Checked || chkReverseCycleAdd.Checked)
                    chkSubstitute.Checked = false;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Reset();
                Settings.Default.add_fasteners_grid_index = cmbGridSpacing.SelectedIndex;
                Settings.Default.add_fasteners_show_grid = chkGrid.Checked;
                Settings.Default.add_fasteners_form_window_location = Location;
                Settings.Default.Save();

                if (!(__display_part_ is null) && __work_part_.__HasDynamicBlock())
                    __work_part_.Bodies.ToArray()[0].__Translucency(0);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                session_.Parts.RemoveWorkPartChangedHandler(WorkPartChangedRegister);
            }
        }

        private void BtnViewWcs_Click(object sender, EventArgs e)
        {
            Matrix3x3 coordSystem = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            __display_part_.Views.WorkView.Orient(coordSystem);
        }

        private void CmbGridSpacing_SelectedIndexChanged(object sender, EventArgs e)
        {
            double _grid_spacing = (double)cmbGridSpacing.SelectedValue;

            if (System.Math.Abs(_grid_spacing) < .001)
            {
                __display_part_.Preferences.Workplane.ShowGrid = false;
                __display_part_.Preferences.Workplane.SnapToGrid = false;
                return;
            }

            __display_part_.Preferences.Workplane.SnapToGrid = true;

            WorkPlane.GridSize grid = new WorkPlane.GridSize
            {
                MajorGridSpacing = _grid_spacing,
                MinorLinesPerMajor = 1,
                SnapPointsPerMinor = 1
            };

            __display_part_.Preferences.Workplane.SetRectangularUniformGridSize(grid);
        }

        private void ButtonSelectComponent_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    SelectTarget();

                    listBoxSelection.Enabled = true;
                    btnWireTaps.Enabled = true;
                    chkCycleAdd.Enabled = true;
                    chkReverseCycleAdd.Enabled = true;
                    btnPlanView.Enabled = true;
                    btnOrigin.Enabled = true;
                    btnWireTaps.Enabled = true;
                    menuItemUnits.Enabled = true;
                    btnSelectComponent.Enabled = false;
                    toolStripMenuItem1.Enabled = true;
                    btnChangeRefSet.Enabled = true;
                }
                catch (NothingSelectedException)
                {
                    btnSelectComponent.Enabled = true;
                    Reset();
                }
                catch (NoDynamicBlockException)
                {
                    print_(@"No dynamic block in current Work Part. You will have to move the WCS manually.");
                    Reset();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void LoadShcs()
        {
            string unit_dir = IsFormMetric ? "Metric" : "English";
            listBoxSelection.SelectedIndexChanged -= ListBoxSelection_SelectedIndexChanged;
            listBoxSelection.DataSource = null;
            string suffix;

            switch (_1x_2x)
            {
                case 1:
                    suffix = "";
                    break;
                case 2:
                    suffix = "-2x";
                    break;
                default:
                    print_("Cannot determine which fastener to use");
                    return;
            }

            FastenerListItem[] shcss = Directory
                .GetDirectories($@"G:\0Library\Fasteners\{unit_dir}\SocketHeadCapScrews{suffix}")
                .Select(__f => new FastenerListItem(Path.GetFileNameWithoutExtension(__f), __f))
                .ToArray();

            const string preferred_diameters_path = @"U:\nxFiles\UfuncFiles\AddFastenersPreferredDiameters.txt";
            const string preferred_gm_diameters_path = @"U:\nxFiles\UfuncFiles\AddFastenersPreferredDiametersGm.txt";

            if (cmbPreferred.SelectedIndex == 1)
            {
                HashSet<string> shcs_preferred_dirs = File.ReadAllLines(preferred_diameters_path)
                    .Where(__s => !string.IsNullOrEmpty(__s))
                    .Where(__s => !string.IsNullOrWhiteSpace(__s))
                    .Select(Path.GetDirectoryName)
                    .ToHashSet();

                shcss = shcss.Where(item => shcs_preferred_dirs.Contains(item.Value)).ToArray();
            }
            else if (cmbPreferred.SelectedIndex == 2)
            {
                HashSet<string> shcs_preferred_dirs = File.ReadAllLines(preferred_gm_diameters_path)
                    .Where(__s => !string.IsNullOrEmpty(__s))
                    .Where(__s => !string.IsNullOrWhiteSpace(__s))
                    .Select(Path.GetDirectoryName)
                    .ToHashSet();

                shcss = shcss.Where(item => shcs_preferred_dirs.Contains(item.Value)).ToArray();
            }

            listBoxSelection.DisplayMember = nameof(FastenerListItem.Text);
            listBoxSelection.ValueMember = nameof(FastenerListItem.Value);
            listBoxSelection.DataSource = shcss;
            listBoxSelection.SelectedIndex = -1;
            listBoxSelection.SelectedIndexChanged += ListBoxSelection_SelectedIndexChanged;
        }

        private void LoadDowel()
        {
            string unit_dir = IsFormMetric ? "Metric" : "English";
            listBoxSelection.SelectedIndexChanged -= ListBoxSelection_SelectedIndexChanged;
            listBoxSelection.DataSource = null;

            chkCycleAdd.Checked = false;
            chkReverseCycleAdd.Checked = false;

            FastenerListItem[] dowels = Directory.GetFiles($@"G:\0Library\Fasteners\{unit_dir}\Dowels", "*.prt",
                    SearchOption.TopDirectoryOnly)
                .Select(__f => new FastenerListItem(Path.GetFileNameWithoutExtension(__f), __f))
                .ToArray();

            listBoxSelection.DisplayMember = nameof(FastenerListItem.Text);
            listBoxSelection.ValueMember = nameof(FastenerListItem.Value);
            listBoxSelection.DataSource = dowels;
            listBoxSelection.SelectedIndex = -1;
            listBoxSelection.SelectedIndexChanged += ListBoxSelection_SelectedIndexChanged;
        }

        private void LoadJack()
        {
            string unit_dir = IsFormMetric ? "Metric" : "English";
            listBoxSelection.SelectedIndexChanged -= ListBoxSelection_SelectedIndexChanged;
            listBoxSelection.DataSource = null;
            chkCycleAdd.Checked = false;
            chkReverseCycleAdd.Checked = false;

            FastenerListItem[] jackScrewsTsg = Directory.GetFiles($@"G:\0Library\Fasteners\{unit_dir}\JackScrews",
                    "*-tsg.prt",
                    SearchOption.TopDirectoryOnly)
                .Select(__f => new FastenerListItem(Path.GetFileNameWithoutExtension(__f), __f))
                .OrderBy(__s =>
                {
                    Match match = Regex.Match(__s.Text, "^_?(?<diameter>\\d+)(mm)?-jck-screw-tsg$");

                    if (!match.Success)
                        throw new Exception($"{__s.Text} didn't match regex");

                    return int.Parse(match.Groups["diameter"].Value);
                })
                .ToArray();

            FastenerListItem[] jackScrews = Directory.GetFiles($@"G:\0Library\Fasteners\{unit_dir}\JackScrews",
                    "*-screw.prt",
                    SearchOption.TopDirectoryOnly)
                .Select(__f => new FastenerListItem(Path.GetFileNameWithoutExtension(__f), __f))
                .ToArray();

            jackScrewsTsg = jackScrewsTsg.Concat(jackScrews).ToArray();
            listBoxSelection.DisplayMember = nameof(FastenerListItem.Text);
            listBoxSelection.ValueMember = nameof(FastenerListItem.Value);
            listBoxSelection.DataSource = jackScrewsTsg;
            listBoxSelection.SelectedIndex = -1;
            listBoxSelection.SelectedIndexChanged += ListBoxSelection_SelectedIndexChanged;
        }

        private void Reset()
        {
            try
            {
                btnChangeRefSet.Enabled = false;
                toolStripMenuItem1.Enabled = false;
                chkSubstitute.Checked = false;
                listBoxSelection.SelectedIndex = -1;
                __work_component_?.__Translucency(0);

                if (__work_part_.__HasDynamicBlock())
                {
                    __work_part_.__DynamicBlock().GetBodies()[0].__Translucency(0);
                    toolStripMenuItem1.Enabled = true;
                }
                else if (!__work_part_.__HasDynamicBlock())
                {
                    Body[] bodies = __work_part_.Bodies.ToArray()
                        .Where(__b => __b.IsSolidBody)
                        .Where(__b => __b.Layer == 1)
                        .ToArray();

                    if (bodies.Length == 1)
                        __work_part_.__SingleSolidBodyOnLayer1().__Translucency(0);

                    toolStripMenuItem1.Enabled = true;
                }
                else
                {
                    btnOrigin.Enabled = false;
                    btnPlanView.Enabled = false;
                    btnWireTaps.Enabled = false;
                    btnChangeRefSet.Enabled = true;
                }

                if (__work_part_.Tag != __display_part_.Tag)
                    ufsession_.Assem.SetWorkPart(NXOpen.Tag.Null);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                btnSelectComponent.Enabled = true;
                btnReset.Enabled = true;
            }
        }

        private void BtnOtherOk_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    session_.SetUndoMark(Session.MarkVisibility.Visible, "AddFasteners");

                    if (__work_component_ is null)
                    {
                        // Working at the displayed part
                        WaveIn();
                        MakePlanView(__display_part_.WCS.Save());
                        return;
                    }

                    // Working at the assembly level.
                    Component originalWorkComponent = __work_component_;

                    using (new DisplayPartReset())
                    {
                        __display_part_ = __work_part_;
                        WaveIn();
                        MakePlanView(__display_part_.WCS.Save());
                    }

                    foreach (Component child in originalWorkComponent.GetChildren())
                    {
                        Component protoPartOcc = _GetProtoPartOcc(originalWorkComponent.__Prototype(), child);

                        switch (protoPartOcc.Layer)
                        {
                            case LayerDwlTooling:
                            case LayerShcsHandlingWireTap:
                                break;
                            default:
                                child.__ReferenceSet(protoPartOcc.ReferenceSet);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
                finally
                {
                    Reset();
                    btnReset.Enabled = true;
                    btnSelectComponent.Enabled = true;
                }
            }
        }

        public static void WaveIn()
        {
            try
            {
                Body solid_body_layer_1;

                if (!__work_part_.__HasDynamicBlock())
                {
                    solid_body_layer_1 = __work_part_.__SingleSolidBodyOnLayer1();
                }
                else
                {
                    __work_part_.Layers.SetState(96, State.Visible);
                    solid_body_layer_1 = __work_part_.__DynamicBlock().GetBodies()[0];
                }

                foreach (Component __child in __work_part_.ComponentAssembly.RootComponent.GetChildren())
                    try
                    {
                        WaveIn(__child, solid_body_layer_1);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void WaveIn(Component __child, Body solid_body_layer_1)
        {
            if (__child.IsSuppressed)
                return;

            if (__child.Layer != 99 && __child.Layer != 98 && __child.Layer != 97)
                return;

            if (!__child.HasInstanceUserAttribute("subtract", NXObject.AttributeType.String, -1))
                return;

            string subtract_att = __child.GetUserAttributeAsString("subtract", NXObject.AttributeType.String, -1);
            string subtract_ref_set;

            switch (subtract_att)
            {
                case "HANDLING":
                case "WIRE_TAP":
                    subtract_ref_set = "SHORT-TAP";
                    break;
                case "BLIND_OPP":
                    subtract_ref_set = "CBORE_BLIND_OPP";
                    break;
                case "CLR_DRILL":
                    subtract_ref_set = "CLR_HOLE";
                    break;
                default:
                    subtract_ref_set = subtract_att;
                    break;
            }

            string current = __child.ReferenceSet;

            __child.__ReferenceSet(subtract_ref_set);

            Tag[] tool_bodies = __child.__Members()
                .OfType<Body>()
                .Where(__b => __b.IsSolidBody)
                .Select(__b => __b.Tag)
                .ToArray();

            int[] results = new int[tool_bodies.Length];

            // issue # 252
            ufsession_.Modl.CheckInterference(
                solid_body_layer_1.Tag,
                tool_bodies.Length,
                tool_bodies,
                results);

            if (results.All(__res => __res != 1))
            {
                __child.__ReferenceSet(current);
                return;
            }

            ExtractFace linked_body = CreateLinkedBody(__work_part_, __child);
            linked_body.OwningPart.Layers.MoveDisplayableObjects(96, linked_body.GetBodies());
            linked_body.SetName($"{__child.DisplayName}, {subtract_att}");

            try
            {
                SubtractLinkedBody(__work_part_, solid_body_layer_1, linked_body);
            }
            catch (NXException ex) when (ex.ErrorCode == 670030)
            {
                print_($"Could not subtract {__child.DisplayName} with reference set {subtract_ref_set}");
            }

            switch (subtract_att)
            {
                case "HANDLING":
                case "WIRE_TAP":
                    __child.__Layer(98);
                    break;
                case "TOOLING":
                    __child.__Layer(97);
                    break;
            }

            if (__child.Layer == 99 && subtract_att != "HANDLING" && subtract_att != "WIRE_TAP")
            {
                __child.__ReferenceSet("BODY");
                __work_part_.GetAllReferenceSets().Single(__r => __r.Name == "BODY")
                    .AddObjectsToReferenceSet(new[] { __child });
            }
            else
            {
                __child.__ReferenceSet("Empty");
                __work_part_.__FindReferenceSet("BODY").RemoveObjectsFromReferenceSet(new NXObject[] { __child });
            }
        }

        private static Component _GetProtoPartOcc(Part owningPart, Component partOcc)
        {
            Tag instance = ufsession_.Assem.AskInstOfPartOcc(partOcc.Tag);
            Tag prototypeChildPartOcc =
                ufsession_.Assem.AskPartOccOfInst(owningPart.ComponentAssembly.RootComponent.Tag, instance);
            return (Component)session_.GetObjectManager().GetTaggedObject(prototypeChildPartOcc);
        }

        private static BooleanFeature SubtractLinkedBody(Part owningPart, Body subtractBody, ExtractFace linkedBody)
        {
            BooleanBuilder booleanBuilder = owningPart.Features.CreateBooleanBuilderUsingCollector(null);

            using (new Destroyer(booleanBuilder))
            {
                booleanBuilder.Target = subtractBody;
                ScCollector collector = owningPart.ScCollectors.CreateCollector();

                SelectionIntentRule[] rules =
                {
                    owningPart.ScRuleFactory.CreateRuleBodyDumb(linkedBody.GetBodies())
                };

                collector.ReplaceRules(rules, false);
                booleanBuilder.ToolBodyCollector = collector;
                booleanBuilder.Operation = Feature.BooleanType.Subtract;
                return (BooleanFeature)booleanBuilder.Commit();
            }
        }

        private static ExtractFace CreateLinkedBody(Part owningPart, Component child)
        {
            Body[] toolBodies = child.__Members()
                .OfType<Body>()
                .Where(body => body.IsSolidBody)
                .ToArray();

            ExtractFaceBuilder linkedBodyBuilder = owningPart.Features.CreateExtractFaceBuilder(null);

            using (new Destroyer(linkedBodyBuilder))
            {
                linkedBodyBuilder.Associative = true;
                linkedBodyBuilder.FeatureOption = ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
                linkedBodyBuilder.FixAtCurrentTimestamp = false;
                linkedBodyBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;
                linkedBodyBuilder.Type = ExtractFaceBuilder.ExtractType.Body;

                SelectionIntentRule[] rules =
                {
                    owningPart.ScRuleFactory.CreateRuleBodyDumb(toolBodies)
                };

                linkedBodyBuilder.ExtractBodyCollector.ReplaceRules(rules, false);
                return (ExtractFace)linkedBodyBuilder.Commit();
            }
        }

        private void ChkSubstitute_Click(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;

            if (box == chkSubstitute)
            {
                chkCycleAdd.Enabled = !chkSubstitute.Checked;
                chkReverseCycleAdd.Enabled = !chkSubstitute.Checked;

                if (!chkSubstitute.Checked)
                    return;

                chkReverseCycleAdd.Checked = false;
                chkCycleAdd.Checked = false;
            }
            else if (box == chkCycleAdd)
            {
                chkReverseCycleAdd.Enabled = true;
                chkSubstitute.Enabled = !chkCycleAdd.Checked;

                if (!chkCycleAdd.Checked)
                    return;

                chkReverseCycleAdd.Checked = false;
                chkSubstitute.Checked = false;
            }
            else if (chkReverseCycleAdd.Checked)
            {
                ChangEnabled(false, chkSubstitute);
                chkSubstitute.Checked = false;
                chkSubstitute.Enabled = false;
                chkCycleAdd.Checked = false;
            }
            else if (!chkCycleAdd.Checked)
            {
                chkSubstitute.Enabled = true;
            }
        }

        private void ChkCycleAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCycleAdd.Checked || chkReverseCycleAdd.Checked)
            {
                rdoTypeDowel.Enabled = false;
                rdoTypeJack.Enabled = false;
                chkSubstitute.Checked = false;
                rdoTypeScrew.PerformClick();
            }
            else
            {
                rdoTypeDowel.Enabled = true;
                rdoTypeJack.Enabled = true;
            }

            Settings.Default.add_fasteners_cycle_add = chkCycleAdd.Checked;
            Settings.Default.add_fasteners_reverse_cycle_add = chkReverseCycleAdd.Checked;
            Settings.Default.Save();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        public static void InsertWireTaps()
        {
            session_.SetUndoMark(Session.MarkVisibility.Visible, "WIRE_TAP");

            if (__display_part_.Tag == __work_part_.Tag)
                new[] { 99, 98, 97 }.ToList().ForEach(i => __display_part_.Layers.SetState(i, State.Selectable));

            Part wireTapScrew =
                session_.__FindOrOpen(@"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\008\8mm-shcs-020.prt");
            __display_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
            CartesianCoordinateSystem savedCsys = __display_part_.WCS.Save();
            Matrix3x3 rotateOrientation = savedCsys.Orientation.Element;
            double x = __display_part_.PartUnits == BasePart.Units.Inches ? 1 : 25.4;
            double[] offset1 = { 1.00 * x, .875 * x, 0.00 };
            double[] offset2 = { 3.00 * x, .875 * x, 0.00 };
            double[] mappedOffset1 = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, offset1, UF_CSYS_ROOT_COORDS, mappedOffset1);
            double[] mappedOffset2 = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, offset2, UF_CSYS_ROOT_COORDS, mappedOffset2);
            double[] mappedToWork1 = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, mappedOffset1, UF_CSYS_WORK_COORDS, mappedToWork1);
            double[] mappedToWork2 = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, mappedOffset2, UF_CSYS_WORK_COORDS, mappedToWork2);
            Point3d basePoint1 = new Point3d(mappedToWork1[0], mappedToWork1[1], mappedToWork1[2]);
            Point3d basePoint2 = new Point3d(mappedToWork2[0], mappedToWork2[1], mappedToWork2[2]);
            Component component1 = __work_part_.ComponentAssembly.AddComponent(wireTapScrew, "SHORT-TAP",
                "8mm-shcs-020",
                basePoint1, rotateOrientation, 98, out _);
            Component component2 = __work_part_.ComponentAssembly.AddComponent(wireTapScrew, "SHORT-TAP",
                "8mm-shcs-020",
                basePoint2, rotateOrientation, 98, out _);
            __display_part_.WCS.Rotate(WCS.Axis.XAxis, -90.0);
            const string subtract = "subtract";
            component1.SetInstanceUserAttribute(subtract, -1, "WIRE_TAP", NXOpen.Update.Option.Now);
            component2.SetInstanceUserAttribute(subtract, -1, "WIRE_TAP", NXOpen.Update.Option.Now);
            session_.__DeleteObjects(savedCsys);
        }

        private void BtnWireTaps_Click(object sender, EventArgs e)
        {
            InsertWireTaps();
        }

        private void BtnOrigin_Click(object sender, EventArgs e)
        {
            BlockOrigin();
        }

        private static void ChangEnabled(bool flag, params Control[] controls)
        {
            controls.ToList().ForEach(control => control.Enabled = flag);
        }

        private void ListBoxSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxSelection.SelectedIndex == -1)
                    return;

                string selected_value = (string)listBoxSelection.SelectedValue;

                if (selected_value == nameof(LoadShcs))
                {
                    LoadShcs();
                    return;
                }

                if (selected_value.ToLower().EndsWith(".prt"))
                    try
                    {
                        cmbReferenceSet.Enabled = true;
                        Part fastener_part = session_.__FindOrOpen(selected_value);

                        if (chkSubstitute.Checked)
                        {
                            SubstituteFasteners(fastener_part);
                            return;
                        }

                        if (chkCycleAdd.Checked || chkReverseCycleAdd.Checked)
                        {
                            PlaceFastenersCycle(fastener_part);
                            return;
                        }

                        if (fastener_part.__IsJckScrewTsg())
                        {
                            PlaceFastenersJigJacks(fastener_part);
                            listBoxSelection.Enabled = true;
                            return;
                        }

                        PlaceFasteners(fastener_part);
                        return;
                    }
                    finally
                    {
                        cmbReferenceSet.Enabled = false;
                    }

                listBoxSelection.SelectedIndexChanged -= ListBoxSelection_SelectedIndexChanged;
                listBoxSelection.DataSource = null;

                List<FastenerListItem> shcss = Directory
                    .GetFiles(selected_value, "*.prt", SearchOption.TopDirectoryOnly)
                    .Select(__f => new FastenerListItem(Path.GetFileNameWithoutExtension(__f), __f))
                    .ToList();

                const string preferred_diameters_path = @"U:\nxFiles\UfuncFiles\AddFastenersPreferredDiameters.txt";
                const string preferred_gm_diameters_path =
                    @"U:\nxFiles\UfuncFiles\AddFastenersPreferredDiametersGm.txt";

                if (cmbPreferred.SelectedIndex == 1)
                {
                    HashSet<string> shcs_preferred_dirs = File.ReadAllLines(preferred_diameters_path)
                        .Where(__s => !string.IsNullOrEmpty(__s))
                        .Where(__s => !string.IsNullOrWhiteSpace(__s))
                        .ToHashSet();

                    shcss = shcss.Where(item => shcs_preferred_dirs.Contains(item.Value)).ToList();
                }
                else if (cmbPreferred.SelectedIndex == 2)
                {
                    HashSet<string> shcs_preferred_dirs = File.ReadAllLines(preferred_gm_diameters_path)
                        .Where(__s => !string.IsNullOrEmpty(__s))
                        .Where(__s => !string.IsNullOrWhiteSpace(__s))
                        .ToHashSet();

                    //CTS_Library.UFuncs.MirrorComponents.MainForm

                    shcss = shcss.Where(item => shcs_preferred_dirs.Contains(item.Value)).ToList();
                }

                shcss.Insert(0, new FastenerListItem("Go Back", nameof(LoadShcs)));
                listBoxSelection.DisplayMember = nameof(FastenerListItem.Text);
                listBoxSelection.ValueMember = nameof(FastenerListItem.Value);
                listBoxSelection.DataSource = shcss;
                listBoxSelection.SelectedIndex = -1;
                listBoxSelection.SelectedIndexChanged += ListBoxSelection_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                btnReset.Enabled = true;
            }
        }

        private void PlaceFastenersJigJacks(Part fastener_part)
        {
            try
            {
                btnOk.Enabled = false;
                listBoxSelection.SelectedIndex = -1;
                listBoxSelection.Enabled = false;
                btnChangeRefSet.Enabled = false;
                btnReset.Enabled = false;
                btnPlanView.Enabled = false;
                btnWireTaps.Enabled = false;
                btnOrigin.Enabled = false;
                cmbGridSpacing.SelectedValue = __display_part_.Preferences.Workplane.GetRectangularUniformGridSize()
                    .MajorGridSpacing;
                chkGrid.Checked = __display_part_.Preferences.Workplane.ShowGrid;

                if (__display_part_.Tag == __work_part_.Tag)
                {
                    __display_part_.Layers.SetState(1, State.WorkLayer);

                    new[] { 99, 98, 97, 97 }
                        .ToList()
                        .ForEach(i => __display_part_.Layers.SetState(i, State.Selectable));
                }

                cmbReferenceSet.Items.Clear();
                HashSet<string> refsets = fastener_part.GetAllReferenceSets().Select(__r => __r.Name).ToHashSet();
                refsets.Remove("BODY");
                refsets.Remove("BODY_EDGE");
                refsets.Remove("MODEL");
                cmbReferenceSet.Items.AddRange(refsets.ToArray());
                cmbReferenceSet.SelectedIndex = 0;
                CartesianCoordinateSystem csys = __display_part_.WCS.Save();

                Component addedFastener = __work_part_.ComponentAssembly.AddComponent(
                    fastener_part,
                    "GRID",
                    fastener_part.Leaf,
                    csys.Origin,
                    csys.Orientation.Element,
                    99,
                    out _);

                session_.__DeleteObjects(csys);

                if (MoveComponentWithMouse(addedFastener) != UF_UI_PICK_RESPONSE)
                {
                    session_.__DeleteObjects(addedFastener);
                    return;
                }

                CartesianCoordinateSystem proto_csys = __work_part_.CoordinateSystems.CreateCoordinateSystem(
                    addedFastener.__Origin(),
                    addedFastener.__Orientation(), false);

                if (__work_component_ is null)
                {
                    proto_csys.GetDirections(out Vector3d xDir, out Vector3d yDir);
                    __display_part_.WCS.SetOriginAndMatrix(proto_csys.Origin, xDir.__ToMatrix3x3(yDir));
                    session_.__DeleteObjects(proto_csys);
                }
                else
                {
                    CartesianCoordinateSystem occ_csys =
                        (CartesianCoordinateSystem)__work_component_.FindOccurrence(proto_csys);
                    occ_csys.GetDirections(out Vector3d xDir, out Vector3d yDir);
                    __display_part_.WCS.SetOriginAndMatrix(occ_csys.Origin, xDir.__ToMatrix3x3(yDir));
                    session_.__DeleteObjects(proto_csys);
                }

                string reference_set = cmbReferenceSet.Text;
                addedFastener.SetInstanceUserAttribute("subtract", -1, reference_set, NXOpen.Update.Option.Now);
                cmbGridSpacing.SelectedValue = 1.000;
                PlaceFasteners(addedFastener.__Prototype(), false);
                cmbGridSpacing.SelectedValue = 0.125;

                if (__work_component_ is null)
                {
                    addedFastener.__ReferenceSet("BODY_EDGE");
                }
                else
                {
                    Tag instance = ufsession_.Assem.AskInstOfPartOcc(addedFastener.Tag);
                    Tag other = ufsession_.Assem.AskPartOccOfInst(__work_component_.Tag, instance);
                    Component comp = (Component)session_.GetObjectManager().GetTaggedObject(other);
                    comp.__ReferenceSet("BODY_EDGE");
                }
            }
            finally
            {
                btnOk.Enabled = true;
            }
        }

        public void PlaceFasteners(Part fastener_part, bool reset_ref_sets = true)
        {
            try
            {
                listBoxSelection.SelectedIndexChanged -= ListBoxSelection_SelectedIndexChanged;
                listBoxSelection.SelectedIndex = -1;
                listBoxSelection.Enabled = false;
                btnOk.Enabled = false;
                grpFastenerType.Enabled = false;
                mnuStrMainMenu.Enabled = false;
                btnChangeRefSet.Enabled = false;
                btnPlanView.Enabled = false;
                btnReset.Enabled = false;
                btnWireTaps.Enabled = false;
                chkCycleAdd.Enabled = false;
                chkReverseCycleAdd.Enabled = false;
                chkSubstitute.Enabled = false;
                btnOrigin.Enabled = false;

                try
                {
                    double value = (double)cmbGridSpacing.SelectedValue;

                    if (value > 0.0)
                        __display_part_.Preferences.Workplane.SetRectangularUniformGridSize(
                            new WorkPlane.GridSize
                            { MajorGridSpacing = value, MinorLinesPerMajor = 1, SnapPointsPerMinor = 1 }
                        );
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                //cmbGridSpacing.SelectedValue = __display_part_.Preferences.Workplane.GetRectangularUniformGridSize().MajorGridSpacing;
                chkGrid.Checked = __display_part_.Preferences.Workplane.ShowGrid;

                if (__display_part_.Tag == __work_part_.Tag)
                {
                    __display_part_.Layers.SetState(1, State.WorkLayer);

                    new[] { 99, 98, 97, 97 }
                        .ToList()
                        .ForEach(i => __display_part_.Layers.SetState(i, State.Selectable));
                }

                if (reset_ref_sets)
                {
                    cmbReferenceSet.Items.Clear();
                    HashSet<string> refsets = fastener_part.GetAllReferenceSets().Select(__r => __r.Name).ToHashSet();
                    refsets.Remove("BODY");
                    refsets.Remove("MODEL");
                    cmbReferenceSet.Items.AddRange(refsets.ToArray());
                    cmbReferenceSet.SelectedIndex = 0;

                    if (fastener_part.__IsShcs())
                        cmbReferenceSet.Items.Add("HANDLING");
                }

                CartesianCoordinateSystem csys = __display_part_.WCS.Save();


                Component addedFastener = null;

                while (true)
                {
                    __display_part_.WCS.CoordinateSystem.GetDirections(out Vector3d xDir, out Vector3d yDir);
                    Matrix3x3 ori = xDir.__ToMatrix3x3(yDir);

                    //addedFastener = __work_part_.ComponentAssembly.AddComponent(
                    //    fastener_part,
                    //    "BODY_EDGE",
                    //    fastener_part.Leaf,
                    //    __display_part_.WCS.Origin,
                    //    ori,
                    //    99,
                    //    out _);

                    addedFastener = __work_part_.ComponentAssembly.AddComponent(
                        fastener_part,
                        "BODY_EDGE",
                        fastener_part.Leaf,
                        csys.Origin,
                        csys.Orientation.Element,
                        99,
                        out _);

                    if (MoveComponentWithMouse(addedFastener) != UF_UI_PICK_RESPONSE)
                    {
                        session_.__DeleteObjects(addedFastener);
                        break;
                    }

                    string reference_set = cmbReferenceSet.Text;
                    addedFastener.SetInstanceUserAttribute("subtract", -1, reference_set, NXOpen.Update.Option.Now);

                    switch (reference_set)
                    {
                        case "HANDLING":
                        case "WIRE_TAP":
                            addedFastener.__Layer(98);
                            break;
                        case "TOOLING":
                            addedFastener.__Layer(97);
                            break;
                        default:
                            addedFastener.__Layer(99);
                            break;
                    }
                }

                session_.__DeleteObjects(csys);
            }
            finally
            {
                listBoxSelection.Enabled = true;
                grpFastenerType.Enabled = true;
                mnuStrMainMenu.Enabled = true;
                btnChangeRefSet.Enabled = true;
                btnPlanView.Enabled = true;
                btnReset.Enabled = true;
                btnWireTaps.Enabled = true;
                chkCycleAdd.Enabled = true;
                chkReverseCycleAdd.Enabled = true;
                chkSubstitute.Enabled = true;
                btnOrigin.Enabled = true;
                btnOk.Enabled = true;
                listBoxSelection.SelectedIndex = -1;
                listBoxSelection.SelectedIndexChanged += ListBoxSelection_SelectedIndexChanged;
            }
        }

        public void PlaceFastenersCycle(Part fastener_part)
        {
            try
            {
                string dir = Path.GetDirectoryName(fastener_part.FullPath);
                string leaf = fastener_part.Leaf;
                string[] cycles = CycleAdd1.MetricCyclePair[dir];
                string small_dwl = cycles[0];
                string large_dwl = cycles[1];
                string jack = cycles[2];
                Match match = Regex.Match(leaf, "-shcs-(?<length>\\d+)");
                string actual_dwl = small_dwl;

                if (match.Success)
                {
                    int length = int.Parse(match.Groups["length"].Value);

                    if (length >= CycleAdd1.MetricDelimeter)
                        actual_dwl = large_dwl;
                }

                string shcs = fastener_part.FullPath;
                string dowel = actual_dwl;
                Part shcs_part = session_.__FindOrOpen(shcs);
                Part dowel_part = session_.__FindOrOpen(dowel);
                Part jigjack_part = session_.__FindOrOpen(jack);

                if (chkCycleAdd.Checked)
                    PlaceFasteners(shcs_part);
                else
                    PlaceFastenersJigJacks(jigjack_part);

                PlaceFasteners(dowel_part);

                if (chkCycleAdd.Checked)
                    PlaceFastenersJigJacks(jigjack_part);
                else
                    PlaceFasteners(shcs_part);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void SubstituteFasteners(Part nxPart)
        {
            using (new LockUpdates())
            {
                Component[] fasteners_to_substitue = new Component[0];

                if (nxPart.__IsShcs())
                    fasteners_to_substitue = __work_part_.__RootComponent().GetChildren()
                        .Where(__c => __c._IsShcs_())
                        .ToArray();
                else if (nxPart.__IsDwl())
                    fasteners_to_substitue = __work_part_.__RootComponent().GetChildren()
                        .Where(__c => __c._IsDwl_())
                        .ToArray();
                else if (nxPart.__IsJckScrew())
                    fasteners_to_substitue = __work_part_.__RootComponent().GetChildren()
                        .Where(__c => __c._IsJckScrew_())
                        .ToArray();
                else if (nxPart.__IsJckScrewTsg())
                    fasteners_to_substitue = __work_part_.__RootComponent().GetChildren()
                        .Where(__c => __c._IsJckScrewTsg_())
                        .ToArray();

                if (fasteners_to_substitue.Length == 0)
                {
                    print_($"Couldn't find any fasteners to substitue with {nxPart.Leaf}");
                    return;
                }

                string original_display_name = fasteners_to_substitue[0].DisplayName;

                ReplaceComponentBuilder replaceBuilder = __work_part_.AssemblyManager.CreateReplaceComponentBuilder();

                using (new Destroyer(replaceBuilder))
                {
                    replaceBuilder.ReplacementPart = nxPart.FullPath;
                    replaceBuilder.MaintainRelationships = true;
                    replaceBuilder.ReplaceAllOccurrences = false;
                    replaceBuilder.ComponentNameType = ReplaceComponentBuilder.ComponentNameOption.AsSpecified;
                    replaceBuilder.ComponentsToReplace.Add(fasteners_to_substitue);
                    replaceBuilder.Commit();
                }

                print_($"Substituted fasteners {original_display_name} -> {nxPart.Leaf}");
            }
        }

        private static void MotionCallback(double[] positionArray, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            GCHandle _handle = (GCHandle)clientData;
            Component component1 = (Component)_handle.Target;
            Session theSession = session_;
            Part workPart = theSession.Parts.Work;
            ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner;
            componentPositioner1.ClearNetwork();
            componentPositioner1.BeginMoveComponent();
            _ = theSession.Preferences.Assemblies.InterpartPositioning;
            Network network1;
            network1 = componentPositioner1.EstablishNetwork();
            ComponentNetwork componentNetwork1 = (ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true;
            Component nullNXOpen_Assemblies_Component = null;
            componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component;
            componentNetwork1.NetworkArrangementsMode = ComponentNetwork.ArrangementsMode.Existing;
            componentNetwork1.RemoveAllConstraints();
            NXObject[] movableObjects1 = new NXObject[1];
            movableObjects1[0] = component1;
            componentNetwork1.SetMovingGroup(movableObjects1);
            componentNetwork1.Solve();
            componentNetwork1.MoveObjectsState = true;
            componentNetwork1.NetworkArrangementsMode = ComponentNetwork.ArrangementsMode.Existing;
            componentNetwork1.BeginDrag();
            component1.GetPosition(out Point3d position, out _);
            Vector3d translation1 = new Vector3d(positionArray[0] - position.X, positionArray[1] - position.Y,
                positionArray[2] - position.Z);
            componentNetwork1.DragByTranslation(translation1);
            componentNetwork1.EndDrag();
            componentNetwork1.ResetDisplay();
            componentNetwork1.ApplyToModel();
            componentNetwork1.Solve();
            componentPositioner1.ClearNetwork();
            theSession.UpdateManager.AddToDeleteList(componentNetwork1);
            componentPositioner1.DeleteNonPersistentConstraints();
            componentPositioner1.EndMoveComponent();
            ufsession_.Modl.Update();
        }

        public static int MoveComponentWithMouse(Component snapComponent)
        {
            GCHandle __handle = GCHandle.Alloc(snapComponent);

            using (session_.__UsingGCHandle(__handle))
            using (session_.__UsingLockUiFromCustom())
            {
                double[] screenPos = new double[3];
                ufsession_.Ui.SpecifyScreenPosition("Move Object", MotionCallback, (IntPtr)__handle, screenPos,
                    out Tag _, out int response);
                return response;
            }
        }

        private void BtnChangeRefSet_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    SelectChangeReferenceSet();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        public void SelectChangeReferenceSet()
        {
            try
            {
                Component[] fasteners = Selection.SelectManyComponents();

                if (fasteners.Length == 0)
                    return;

                if (fasteners.Any(__fast => __fast.OwningComponent.Tag != fasteners[0].OwningComponent.Tag))
                {
                    print_("All fasteners must be under the same component");
                    return;
                }

                // remove BODY, GRID, BODY_EDGE, MODEL
                List<string> reference_set_names = fasteners.Select(__fast => __fast.Prototype)
                    .OfType<Part>()
                    .SelectMany(__part => __part.GetAllReferenceSets())
                    .Select(__ref => __ref.Name)
                    .Distinct()
                    .ToList();

                reference_set_names.Remove("BODY");
                reference_set_names.Remove("BODY_EDGE");
                reference_set_names.Remove("GRID");
                reference_set_names.Remove("MODEL");

                // all shcs metric
                if (fasteners.All(__fast => __fast.__IsShcs()) &&
                    fasteners.All(__fast => __fast.Name.ToLower().Contains("mm")))
                {
                    reference_set_names.Add("HANDLING");
                }

                // all shcs english
                else if (fasteners.All(__fast => __fast.__IsShcs()) &&
                         fasteners.All(__fast => !__fast.Name.ToLower().Contains("mm")))
                {
                    reference_set_names.Add("HANDLING");
                }

                // all dwl metric
                else if (fasteners.All(__fast => __fast.__IsDwl()) &&
                         fasteners.All(__fast => __fast.Name.ToLower().Contains("mm")))
                {
                    reference_set_names.Add("TOOLING");
                }

                // all dwl english
                else if (fasteners.All(__fast => __fast.__IsDwl()) &&
                         fasteners.All(__fast => !__fast.Name.ToLower().Contains("mm")))
                {
                    reference_set_names.Add("TOOLING");
                }

                // all jack screw 
                else if (fasteners.All(__fast => __fast.__IsJckScrew()))
                {
                }
                // all jack screw tsg
                else if (fasteners.All(__fast => __fast.__IsJckScrewTsg()))
                {
                }
                else
                {
                    print_("You cannot select different type of fasteners in the same selection");
                    return;
                }

                string selected_ref_set =
                    session_.__SelectMenuItem14gt("Select Reference Set", reference_set_names.ToArray());

                foreach (Component selected_fastener in fasteners)
                    using (session_.__UsingDisplayPartReset())
                    {
                        Component actual_fastener;

                        if (!(__work_component_ is null) || selected_fastener.Parent.Tag !=
                            __display_part_.ComponentAssembly.RootComponent.Tag)
                        {
                            Tag instance = ufsession_.Assem.AskInstOfPartOcc(selected_fastener.Tag);
                            Tag actual_tag = ufsession_.Assem.AskPartOccOfInst(
                                __work_component_.__Prototype().ComponentAssembly.RootComponent.Tag, instance);
                            actual_fastener = (Component)session_.__GetTaggedObject(actual_tag);
                            __display_part_ = actual_fastener.Parent.__Prototype();
                        }
                        else
                        {
                            actual_fastener = selected_fastener;
                        }

                        __display_part_
                            .Features
                            .ToArray()
                            .OfType<ExtractFace>()
                            .Where(__f => __f.FeatureType == "LINKED_BODY")
                            .ToList()
                            .ForEach(__k =>
                            {
                                ufsession_.Wave.IsLinkBroken(__k.Tag, out bool is_broken);

                                if (is_broken)
                                    return;

                                ufsession_.Wave.AskLinkXform(__k.Tag, out Tag xform);
                                ufsession_.So.AskAssyCtxtPartOcc(xform,
                                    __display_part_.ComponentAssembly.RootComponent.Tag, out Tag from_part_occ);

                                if (from_part_occ == NXOpen.Tag.Null)
                                    return;

                                double[] point = new double[3];
                                ufsession_.So.AskPointOfXform(xform, point);
                                Component from_comp =
                                    (Component)session_.GetObjectManager().GetTaggedObject(from_part_occ);

                                if (!from_comp.__Origin().__IsEqualTo(point))
                                    return;

                                if (from_part_occ != actual_fastener.Tag)
                                    return;

                                session_.__DeleteObjects(__k);
                                actual_fastener.SetInstanceUserAttribute("subtract", -1, selected_ref_set,
                                    NXOpen.Update.Option.Now);
                                WaveIn(actual_fastener, __display_part_.__SingleSolidBodyOnLayer1());
                            });
                    }
            }
            catch (NothingSelectedException)
            {
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void BtnPlanView_Click(object sender, EventArgs e)
        {
            const string createPlanView = "Create Plan View?";
            const string areYouSure = "Are you sure?";
            DialogResult result = MessageBox.Show(createPlanView, areYouSure, MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
                MakePlanView(__display_part_.WCS.Save());
        }

        private void ChkGrid_CheckedChanged(object sender, EventArgs e)
        {
            __display_part_.Preferences.Workplane.ShowGrid = chkGrid.Checked;
        }

        private void ForceButtonsOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeAllButtonsEnabled(true);
        }

        public void ChangeAllButtonsEnabled(bool flag)
        {
            ChangEnabled(flag, listBoxSelection, btnSelectComponent, grpFastenerType, chkGrid, cmbGridSpacing,
                btnPlanView, btnOk, btnOrigin, btnWireTaps, btnChangeRefSet, chkSubstitute,
                chkSubstitute, chkCycleAdd, chkReverseCycleAdd);
            mnu2x.Enabled = flag;
            menuItemUnits.Enabled = flag;
        }

        private void CloseAllFastenersMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                const string oLibraryFasteners = "G:\\0Library\\Fasteners";

                if (__display_part_.FullPath.StartsWith(oLibraryFasteners))
                {
                    print_("You cannot run this command with a fastener as your display part.");
                    return;
                }

                List<BasePart> partsToClose = session_.Parts
                    .ToArray()
                    .Where(part => part.FullPath.StartsWith(oLibraryFasteners))
                    .ToList();

                partsToClose.ForEach(part => part.__Close(true, true));
                print_($"Closed {partsToClose.Count} fastener files.");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void MakePlanView(CartesianCoordinateSystem csys)
        {
            const string l1 = "L1";
            const string top = "Top";
            const string plan = "PLAN";
            __display_part_ = __work_part_;
            ModelingView planView =
                session_.Parts.Work.ModelingViews.ToArray().SingleOrDefault(__v => __v.Name == "PLAN");
            ModelingView modelingView1, modelingView2;
            Layout layout;

            if (planView != null)
            {
                layout = __work_part_.Layouts.FindObject(l1);
                modelingView1 = __work_part_.ModelingViews.WorkView;
                modelingView2 = __work_part_.ModelingViews.FindObject(top);
                layout.ReplaceView(modelingView1, modelingView2, true);
                ModelingView tempView = __work_part_.ModelingViews.FindObject(plan);
                session_.__DeleteObjects(tempView);
            }

            ufsession_.View.SetViewMatrix("", 3, csys.Tag, null);
            modelingView1 =
                (ModelingView)__display_part_.Views.SaveAs(__display_part_.ModelingViews.WorkView, plan, false, false);
            modelingView2 = __display_part_.ModelingViews.FindObject(top);
            __display_part_.Layouts.FindObject(l1).ReplaceView(modelingView1, modelingView2, true);
            session_.__DeleteObjects(csys);
        }

        public static ExtractFace GetLinkedBody(Part owningPart, Component child)
        {
            foreach (Feature feature in owningPart.Features)
            {
                if (feature.FeatureType != "LINKED_BODY")
                    continue;

                ufsession_.Wave.AskLinkXform(feature.Tag, out Tag xform);

                if (xform == NXOpen.Tag.Null)
                    continue;

                ufsession_.So.AskAssyCtxtPartOcc(xform, owningPart.ComponentAssembly.RootComponent.Tag,
                    out Tag fromPartOcc);

                if (fromPartOcc == child.Tag)
                    return (ExtractFace)feature;
            }

            return null;
        }

        public static void WaveOut(Component child)
        {
            Part owningPart = (Part)child.OwningPart;

            if (child.Parent.Tag != owningPart.ComponentAssembly.RootComponent.Tag)
                throw new ArgumentException("Can only wave out immediate children.");

            ExtractFace linkedBody = GetLinkedBody(owningPart, child);
            session_.__DeleteObjects(linkedBody);
        }

        public static void WaveOut()
        {
            if (__work_part_.Tag == __display_part_.Tag)
            {
                foreach (Component __child in __work_part_.ComponentAssembly.RootComponent.GetChildren())
                    try
                    {
                        if (__child.Layer != 99 && __child.Layer != 98 && __child.Layer != 97)
                            continue;

                        if (__child.IsSuppressed)
                            continue;

                        if (!__child.__IsFastener())
                            continue;

                        ExtractFace link = GetLinkedBody(__work_part_, __child);
                        WaveOut(__child);
                        session_.__DeleteObjects(link);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                return;
            }

            foreach (Component child in __work_component_.GetChildren())
            {
                if (child.IsSuppressed)
                    continue;

                if (!child.__IsFastener())
                    continue;

                Component protoPartOcc = _GetProtoPartOcc(__work_part_, child);
                WaveOut(protoPartOcc);
            }
        }

        private static void BlockOrigin()
        {
            try
            {
                SetWcsToWorkPart();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void SelectTarget()
        {
            NXOpen.Selection.Response response = UI.GetUI().SelectionManager.SelectTaggedObject(
                "Select A Target Body",
                "Select A Target Body",
                NXOpen.Selection.SelectionScope.AnyInAssembly,
                NXOpen.Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { Selection.SolidBodyMask },
                out TaggedObject _object,
                out _);

            switch (response)
            {
                case NXOpen.Selection.Response.Back:
                case NXOpen.Selection.Response.Cancel:
                    throw new NothingSelectedException();
                case NXOpen.Selection.Response.Ok:
                    if (!__work_part_.__HasDynamicBlock())
                    {
                        __display_part_.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                        Body solid_body = __work_part_.__SingleSolidBodyOnLayer1();
                        solid_body.__Translucency(75);
                        print_("Part does not have a dynamic block. You will need to move the csys manually.");
                        return;
                    }

                    __work_part_.__DynamicBlock().GetBodies()[0].__Translucency(75);
                    __display_part_.WCS.SetOriginAndMatrix(__work_part_.__DynamicBlock().__Origin(),
                        __work_part_.__DynamicBlock().__Orientation());
                    return;
                case NXOpen.Selection.Response.ObjectSelectedByName:
                    throw new InvalidOperationException("Cannot select an object by name");
                case NXOpen.Selection.Response.ObjectSelected:
                    Body selected = (Body)_object;

                    if (selected.IsOccurrence)
                    {
                        Body proto_selected = selected.__Prototype();
                        Part part = selected.OwningComponent.__Prototype();

                        if (!part.__HasDynamicBlock())
                        {
                            __work_component_ = selected.OwningComponent;
                            __work_component_.__Translucency(75);
                            __display_part_.WCS.SetOriginAndMatrix(__work_component_.__Origin(),
                                __work_component_.__Orientation());
                            print_("Part does not have a dynamic block. You will need to move the csys manually.");
                            return;
                        }

                        Point3d origin = part.__DynamicBlock().__Origin();
                        Matrix3x3 orientation = part.__DynamicBlock().__Orientation();
                        CartesianCoordinateSystem csys =
                            part.CoordinateSystems.CreateCoordinateSystem(origin, orientation, false);
                        selected.OwningComponent.__Prototype().__FindReferenceSet("BODY")
                            .AddObjectsToReferenceSet(new[] { csys });
                        CartesianCoordinateSystem occ_csys =
                            (CartesianCoordinateSystem)selected.OwningComponent.FindOccurrence(csys);
                        __display_part_.WCS.SetCoordinateSystemCartesianAtCsys(occ_csys);
                        __work_component_ = selected.OwningComponent;
                        __work_component_.__Translucency(75);
                        session_.__DeleteObjects(occ_csys, csys);
                    }
                    else if (__work_part_.__HasDynamicBlock())
                    {
                        if (__work_part_.__DynamicBlock().GetBodies()[0].Tag != selected.Tag)
                            throw new InvalidOperationException("Selected body that wasn't the dynamic block");

                        __work_part_.__DynamicBlock().GetBodies()[0].__Translucency(75);
                        __display_part_.WCS.SetOriginAndMatrix(__work_part_.__DynamicBlock().__Origin(),
                            __work_part_.__DynamicBlock().__Orientation());
                    }
                    else
                    {
                        Body[] bodies = __work_part_.Bodies.ToArray()
                            .Where(__b => __b.IsSolidBody)
                            .Where(__b => __b.Layer == 1)
                            .ToArray();

                        if (bodies.Length == 1)
                        {
                            __work_part_.__SingleSolidBodyOnLayer1().__Translucency(75);
                            __display_part_.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
                            print_("Part does not have a dynamic block. You will need to move the csys manually.");
                        }
                    }

                    break;
            }
        }

        public static void SetWcsToWorkPart()
        {
            Block dynamicBlock = __work_part_.__DynamicBlock();
            Point3d origin = dynamicBlock.__Origin();
            Matrix3x3 orientation = dynamicBlock.__Orientation();

            if (__work_part_.Tag == __display_part_.Tag)
            {
                __display_part_.WCS.SetOriginAndMatrix(origin, orientation);
                return;
            }

            CartesianCoordinateSystem absCsys =
                __display_part_.CoordinateSystems.CreateCoordinateSystem(_Point3dOrigin, _Matrix3x3Identity, true);

            CartesianCoordinateSystem compCsys = __display_part_.CoordinateSystems.CreateCoordinateSystem(
                __work_component_.__Origin(),
                __work_component_.__Orientation(),
                true);

            Point3d newOrigin = origin.__MapCsysToCsys(compCsys, absCsys);
            Vector3d newXVec = __work_component_.__Orientation().__AxisX().__MapCsysToCsys(compCsys, absCsys);
            Vector3d newYVec = __work_component_.__Orientation().__AxisY().__MapCsysToCsys(compCsys, absCsys);
            Matrix3x3 newOrientation = newXVec.__ToMatrix3x3(newYVec);
            __display_part_.WCS.SetOriginAndMatrix(newOrigin, newOrientation);
        }

        private void mnu2x_Click(object sender, EventArgs e)
        {
            switch (_1x_2x)
            {
                case 1:
                    mnu2x.Text = "2x";
                    _1x_2x = 2;
                    break;
                case 2:
                    mnu2x.Text = "1x";
                    _1x_2x = 1;
                    break;
                default:
                    print_("Couldn't determine 1x or 2x");
                    break;
            }

            if (rdoTypeScrew.Checked)
                LoadShcs();
        }

        private void RdoFastener_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTypeScrew.Checked)
                LoadShcs();

            if (rdoTypeDowel.Checked)
                LoadDowel();

            if (rdoTypeJack.Checked)
                LoadJack();
        }

        private void cmbPreferred_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.add_fasteners_preferred_index = cmbPreferred.SelectedIndex;
            Settings.Default.Save();

            if (rdoTypeScrew.Checked)
                LoadShcs();
        }

        public struct CycleAdd1
        {
            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public static double MetricCycle;

            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public static double EnglishCycle;

            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public double Diameter;

            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public string ShortDowel;

            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public string LongDowel;

            /// <summary>
            /// </summary>
            // ReSharper disable once NotAccessedField.Global
            public string JackCycle;

            /// <summary>
            /// </summary>
            /// <param name="diameter"></param>
            /// <param name="shortDowel"></param>
            /// <param name="longDowel"></param>
            /// <param name="jackCycle"></param>
            public CycleAdd1(double diameter, string shortDowel, string longDowel, string jackCycle)
            {
                Diameter = diameter;
                ShortDowel = shortDowel;
                LongDowel = longDowel;
                JackCycle = jackCycle;
            }

            public static int MetricDelimeter = 30;

            public static int EnglishDelimeter = 125;

            public static IDictionary<string, string[]> MetricCyclePair
            {
                get
                {
                    return new Dictionary<string, string[]>
                {
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\004",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\005",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\006",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\008",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\8mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\010",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\10mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\012",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\016",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\020",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\024",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\030",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews\036",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0006",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0008",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0010",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0250",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0313",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0313-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0375",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0375-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0500",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0625",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0750",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\0875",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1000",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1250",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews\1500",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },


                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\004-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\005-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\006-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-020.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\6mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\6mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\008-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\8mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\8mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\010-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\10mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\10mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\012-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-025.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\12mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\016-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\16mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\020-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\20mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\024-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\030-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\Metric\SocketHeadCapScrews-2x\036-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\Dowels\24mm-dwl-050.prt",
                            @"G:\0Library\Fasteners\Metric\JackScrews\12mm-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0006-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0008-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0010-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0250-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0250-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0250-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0313-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0313-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0313-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0375-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0375-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0375-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0500-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0500-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0625-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0625-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0750-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\0875-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\0750-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1000-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1250-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    },
                    {
                        @"G:\0Library\Fasteners\English\SocketHeadCapScrews-2x\1500-2x",
                        new[]
                        {
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-100.prt",
                            @"G:\0Library\Fasteners\English\Dowels\1000-dwl-200.prt",
                            @"G:\0Library\Fasteners\English\JackScrews\_0500-jck-screw-tsg.prt"
                        }
                    }
                };
                }
            }
        }


        public struct CycleAddStruct
        {
            public CycleAddStruct(string shcsDir, string dwlPathMin, string dwlPathMax, string jckTsgPath)
            {
                ShcsDir = shcsDir;
                DwlPathMin = dwlPathMin;
                DwlPathMax = dwlPathMax;
                JckTsgPath = jckTsgPath;
            }

            public string ShcsDir { get; }

            public string DwlPathMin { get; }

            public string DwlPathMax { get; }

            public string JckTsgPath { get; }

            public static double MetricCycle;

            public static double EnglishCycle;

            public const int METRIC_DELIMETER = 30;

            public const int ENGLISH_DELIMETER = 125;

            private const string metric_shcs_dir = "G:\\0Library\\Fasteners\\Metric\\SocketHeadCapScrews\\";
            private const string metric_dwls_dir = "G:\\0Library\\Fasteners\\Metric\\Dowels\\";
            private const string metric_jck_tsg_dir = "G:\\0Library\\Fasteners\\Metric\\JackScrews\\";

            private const string english_shcs_dir = "G:\\0Library\\Fasteners\\English\\SocketHeadCapScrews\\";
            private const string english_dwls_dir = "G:\\0Library\\Fasteners\\English\\Dowels\\";
            private const string english_jck_tsg_dir = "G:\\0Library\\Fasteners\\English\\JackScrews\\";

            public static readonly IDictionary<string, CycleAddStruct> CycleAddDict = new Dictionary<string, CycleAddStruct>
            {
                [metric_shcs_dir + "004"] = new CycleAddStruct(
                    metric_shcs_dir + "004.prt",
                    metric_dwls_dir + "6mm-dwl-020.prt",
                    metric_dwls_dir + "6mm-dwl-050.prt",
                    metric_jck_tsg_dir + "6mm-jck-screw-tsg.prt.prt"),

                [metric_shcs_dir + "005"] = new CycleAddStruct(
                    metric_shcs_dir + "005.prt",
                    metric_dwls_dir + "6mm-dwl-020.prt",
                    metric_dwls_dir + "6mm-dwl-050.prt",
                    metric_jck_tsg_dir + "6mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "006"] = new CycleAddStruct(
                    metric_shcs_dir + "006.prt",
                    metric_dwls_dir + "6mm-dwl-020.prt",
                    metric_dwls_dir + "6mm-dwl-050.prt",
                    metric_jck_tsg_dir + "6mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "008"] = new CycleAddStruct(
                    metric_shcs_dir + "008.prt",
                    metric_dwls_dir + "8mm-dwl-025.prt",
                    metric_dwls_dir + "8mm-dwl-050.prt",
                    metric_jck_tsg_dir + "8mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "010"] = new CycleAddStruct(
                    metric_shcs_dir + "010.prt",
                    metric_dwls_dir + "10mm-dwl-025.prt",
                    metric_dwls_dir + "10mm-dwl-050.prt",
                    metric_jck_tsg_dir + "10mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "012"] = new CycleAddStruct(
                    metric_shcs_dir + "012.prt",
                    metric_dwls_dir + "12mm-dwl-025.prt",
                    metric_dwls_dir + "12mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "016"] = new CycleAddStruct(
                    metric_shcs_dir + "016.prt",
                    metric_dwls_dir + "16mm-dwl-050.prt",
                    metric_dwls_dir + "16mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "020"] = new CycleAddStruct(
                    metric_shcs_dir + "020.prt",
                    metric_dwls_dir + "20mm-dwl-050.prt",
                    metric_dwls_dir + "20mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "024"] = new CycleAddStruct(
                    metric_shcs_dir + "024.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "030"] = new CycleAddStruct(
                    metric_shcs_dir + "030.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [metric_shcs_dir + "036"] = new CycleAddStruct(
                    metric_shcs_dir + "036.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_dwls_dir + "24mm-dwl-050.prt",
                    metric_jck_tsg_dir + "12mm-jck-screw-tsg.prt"),

                [english_shcs_dir + "0006"] = new CycleAddStruct(
                    english_shcs_dir + "0006.prt",
                    english_dwls_dir + "0250-dwl-100.prt",
                    english_dwls_dir + "0250-dwl-200.prt",
                    english_jck_tsg_dir + "_0250-jck-screw-tsg.prt"),

                [english_shcs_dir + "0008"] = new CycleAddStruct(
                    english_shcs_dir + "0008.prt",
                    english_dwls_dir + "0250-dwl-100.prt",
                    english_dwls_dir + "0250-dwl-200.prt",
                    english_jck_tsg_dir + "_0250-jck-screw-tsg.prt"),

                [english_shcs_dir + "0010"] = new CycleAddStruct(
                    english_shcs_dir + "0010.prt",
                    english_dwls_dir + "0250-dwl-100.prt",
                    english_dwls_dir + "0250-dwl-200.prt",
                    english_jck_tsg_dir + "_0250-jck-screw-tsg.prt"),

                [english_shcs_dir + "0250"] = new CycleAddStruct(
                    english_shcs_dir + "0250.prt",
                    english_dwls_dir + "0250-dwl-100.prt",
                    english_dwls_dir + "0250-dwl-200.prt",
                    english_jck_tsg_dir + "_0250-jck-screw-tsg.prt"),

                [english_shcs_dir + "0313"] = new CycleAddStruct(
                    english_shcs_dir + "0313.prt",
                    english_dwls_dir + "0313-dwl-100.prt",
                    english_dwls_dir + "0313-dwl-200.prt",
                    english_jck_tsg_dir + "_0375-jck-screw-tsg.prt"),

                [english_shcs_dir + "0375"] = new CycleAddStruct(
                    english_shcs_dir + "0375.prt",
                    english_dwls_dir + "0375-dwl-100.prt",
                    english_dwls_dir + "0375-dwl-200.prt",
                    english_jck_tsg_dir + "_0375-jck-screw-tsg.prt"),

                [english_shcs_dir + "0500"] = new CycleAddStruct(
                    english_shcs_dir + "0500.prt",
                    english_dwls_dir + "0500-dwl-100.prt",
                    english_dwls_dir + "0500-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt"),

                [english_shcs_dir + "0750"] = new CycleAddStruct(
                    english_shcs_dir + "0750.prt",
                    english_dwls_dir + "0750-dwl-100.prt",
                    english_dwls_dir + "0750-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt"),

                [english_shcs_dir + "0875"] = new CycleAddStruct(
                    english_shcs_dir + "0875.prt",
                    english_dwls_dir + "0750-dwl-100.prt",
                    english_dwls_dir + "0750-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt"),

                [english_shcs_dir + "1000"] = new CycleAddStruct(
                    english_shcs_dir + "1000.prt",
                    english_dwls_dir + "1000-dwl-100.prt",
                    english_dwls_dir + "1000-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt"),

                [english_shcs_dir + "1250"] = new CycleAddStruct(
                    english_shcs_dir + "1250.prt",
                    english_dwls_dir + "1000-dwl-100.prt",
                    english_dwls_dir + "1000-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt"),

                [english_shcs_dir + "1500"] = new CycleAddStruct(
                    english_shcs_dir + "1500.prt",
                    english_dwls_dir + "1000-dwl-100.prt",
                    english_dwls_dir + "1000-dwl-200.prt",
                    english_jck_tsg_dir + "_0500-jck-screw-tsg.prt")
            };
        }


        public class FastenerListItem
        {
            public FastenerListItem(string __text, string __value)
            {
                Text = __text;
                Value = __value;
            }

            public string Text { get; set; }

            public string Value { get; }
        }



        public struct GridSpacing
        {
            public double Value { get; private set; }

            public string StringValue { get; }

            public GridSpacing(double value, string stringValue)
                : this()
            {
                Value = value;
                StringValue = stringValue;
            }

            public override string ToString()
            {
                return StringValue;
            }
        }
    }
}