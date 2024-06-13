using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.Positioning;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Utilities.GFolder;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_clone_assembly)]
    public partial class CloneAssemblyForm : _UFuncForm
    {
        public const string SEED_DIRECTORY = "G:\\0Library\\SeedFiles\\NX1919";

        public CloneAssemblyForm()
        {
            InitializeComponent();
        }

        private void btnTestExecute_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //System.Diagnostics.Debugger.Launch();

            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    GFolder __folder = Create(txtJobFolder_.Text);

                    string[] ops = txtOps.Text.Replace(Environment.NewLine, " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Trim()
                        .Split(' ')
                        .ToList().ToArray();

                    session_.Parts.LoadOptions.ComponentLoadMethod = LoadOptions.LoadMethod.AsSaved;
                    session_.Parts.LoadOptions.ComponentsToLoad = LoadOptions.LoadComponents.All;
                    session_.Parts.LoadOptions.PartLoadOption = LoadOptions.LoadOption.FullyLoad;
                    session_.Parts.LoadOptions.SetInterpartData(true, LoadOptions.Parent.Partial);

                    GFolder folder = Create(txtJobFolder_.Text);

                    if (rdoTransfer.Checked)
                    {
                        clone_transfer(__folder, txtOps.Text.Replace(Environment.NewLine, " ")
                            .Replace("  ", " ")
                            .Replace("  ", " ")
                            .Replace("  ", " ")
                            .Trim(), chkStartWithBlank.Checked, rdoMetric_.Checked);
                        return;
                    }

                    if (rdoLine.Checked)
                    {
                        clone_line(folder, ops, rdoMetric_.Checked);
                        return;
                    }

                    if (rdoProressive.Checked)
                    {
                        string op = txtOps.Text.Replace("\\n", " ").Trim();

                        if (!File.Exists(folder.file_strip(op)))
                        {
                            print_($"Can't clone prog die without {op}-Strip.");
                            return;
                        }

                        if (!int.TryParse(op, out _))
                        {
                            print_("Invalid op");
                            return;
                        }

                        int press_op = Convert.ToInt32(numudPressDiesetOp.Value);

                        clone_prog(folder, op, rdoMetric_.Checked, press_op);
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
                finally
                {
                    stopwatch.Stop();
                    print_(stopwatch.Elapsed);
                }
            }
        }

        private void rdoTransfer_CheckedChanged(object sender, EventArgs e)
        {
            chkStartWithBlank.Enabled = rdoTransfer.Checked;

            if (!chkStartWithBlank.Enabled)
                chkStartWithBlank.Checked = false;

            if (rdoProressive.Checked)
                txtOps.Text = "010";

            if (rdoLine.Checked)
                txtOps.Text = "010 020 030 040";

            if (rdoTransfer.Checked)
                txtOps.Text = $"301 010 020{Environment.NewLine}302 030 040";
        }

        private void txtJobFolder__TextChanged(object sender, EventArgs e)
        {
        }

        private void txtJobFolder__DoubleClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                SelectedPath = "G:\\"
            };

            dialog.ShowDialog();

            string k = dialog.SelectedPath;

            txtJobFolder_.Text = k;
        }

        private void CloneAssemblyForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.clone_assembly_form_window_location;

            rdoProressive.Checked = true;

            rdoTransfer.Checked = true;

            if (Environment.UserName == "mcbailey")
            {
                txtJobFolder_.Text = @"C:\CTS\003506 (lydall)";

                //txtOps.Text = "301 010";
                //txtOps.Text = "301";
                rdoProressive.Checked = true;

                chkStartWithBlank.Checked = true;
            }
        }

        private void CloneAssemblyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.clone_assembly_form_window_location = Location;
            Settings.Default.Save();
        }

        public static void clone_prog(GFolder __folder, string op, bool units_mm, int press_op_dieset)
        {
            try
            {
                if (!Directory.Exists(__folder.dir_op(op)))
                    Directory.CreateDirectory(__folder.dir_op(op));

                ExecuteCloneProg(__folder, op);

                __display_part_ = session_.__FindOrOpen(__folder.file_detail_000(op));

                replace_strip_layout_in_display(__folder.FileStrip010, "STRIP", true);

                using (new DisplayPartReset())
                {
                    delete_unit_fasteners(__folder, op, units_mm);
                }

                add_presses(__folder);

                using (new DisplayPartReset())
                {
                    try
                    {
                        session_.__FindOrOpen(__folder.path_op_detail(op, "upr")).__SetAsDisplayPart();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent($"COMPONENT {__folder.CustomerNumber}-{op}-usp3 1").Suppress();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent($"COMPONENT {__folder.CustomerNumber}-{op}-usp4 1").Suppress();
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }

                using (new DisplayPartReset())
                {
                    try
                    {
                        session_.__FindOrOpen(__folder.path_op_detail(op, "lwr")).__SetAsDisplayPart();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent($"COMPONENT {__folder.CustomerNumber}-{op}-lsp3 1").Suppress();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent($"COMPONENT {__folder.CustomerNumber}-{op}-lsp4 1").Suppress();
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }

                using (new DisplayPartReset())
                {
                    __display_part_.Layers.MoveDisplayableObjects(256, __display_part_.DisplayedConstraints.ToArray());
                }

                string dieset_control = $"{__folder.CustomerNumber}-{op}-dieset-control.prt";

                using (session_.__usingDisplayPartReset())
                {
                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("010", "002"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("010", "012"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("010", "502"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("010", "512"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);
                }

                using (session_.__usingDisplayPartReset())
                {
                    try
                    {
                        if (press_op_dieset == 0)
                        {
                            print_("Did not hook up dieset control to a press.");
                        }
                        else
                        {
                            string press_op = __Op10To010(press_op_dieset * 10);

                            __display_part_ = session_.__FindOrOpen(dieset_control);
                            __display_part_.Expressions.ChangeInterpartReferences("XXXXX-Press-XX-Assembly.prt",
                                $"{__folder.CustomerNumber}-P{press_op}-Press.prt", true, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }

                __display_part_.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void clone_line(GFolder __folder, string[] ops, bool units_mm)
        {
            try
            {
                foreach (string op in ops)
                {
                    if (!Directory.Exists(__folder.dir_op(op)))
                        Directory.CreateDirectory(__folder.dir_op(op));

                    ExecuteCloneLineOp(__folder, op);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail_000(op));

                    try
                    {
                        //if (!start_with_strip)
                        replace_strip_layout_in_display(__folder.file_layout_t(op), "LAYOUT", true);
                        //else if (op == "010")
                        //    replace_strip_layout_in_display(__folder.file_strip_900, "STRIP", true);
                        //else
                        //    replace_strip_layout_in_display(__folder.file_layout_t(TSG_Library.Extensions.op_020_010(op)), "LAYOUT", true);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                    using (new DisplayPartReset())
                    {
                        delete_unit_fasteners(__folder, op, units_mm);
                    }

                    add_press(__folder, op);

                    session_.__FindOrOpen(__folder.path_op_detail(op, "lsh"))
                        .Features
                        .ToArray()
                        .OfType<DatumCsys>()
                        .ToList()
                        .ForEach(__d => __d.Suppress());
                }

                Session.GetSession().Parts.SetAllowMultipleDisplayedParts(true);

                foreach (string op in ops)
                    session_.__FindOrOpen(__folder.file_detail0(op, "000")).__SetActiveDisplay();

                if (!Directory.Exists(__folder.dir_op("900")))
                    Directory.CreateDirectory(__folder.dir_op("900"));

                ExecuteCloneLine900(__folder);

                session_.__FindOrOpen(__folder.file_detail_000("900")).__SetActiveDisplay();

                try
                {
                    replace_strip_layout_in_display(__folder.FileStrip900, "STRIP", true);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                using (new DisplayPartReset())
                {
                    try
                    {
                        ConstrainOpsToLwrUpr(__folder, ops, "lwr", 1);

                        ConstrainOpsToLwrUpr(__folder, ops, "upr", 101);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }

                using (new DisplayPartReset())
                {
                    __display_part_.ComponentAssembly.RootComponent.GetChildren()
                        .Single(__c => __c.Name.Contains("LWR")).__ReferenceSet("BODY");
                    __display_part_.__Fit();
                    __display_part_.ComponentAssembly.RootComponent.GetChildren()
                        .Single(__c => __c.Name.Contains("UPR")).__ReferenceSet("BODY");
                    __display_part_.__Fit();
                }

                using (new DisplayPartReset())
                {
                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("900", "upr"));

                    __display_part_.__ReferenceSets("BODY").RemoveObjectsFromReferenceSet(__display_part_
                        .ComponentAssembly.RootComponent.GetChildren().Where(__c => __c.Name != "STRIP").ToArray());

                    __display_part_.__Fit();
                }

                using (new DisplayPartReset())
                {
                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0("900", "lwr"));

                    __display_part_.__ReferenceSets("BODY").AddObjectsToReferenceSet(__display_part_.ComponentAssembly
                        .RootComponent.GetChildren().Where(__c => __c.Name != "STRIP").ToArray());

                    __display_part_.__Fit();
                }

                using (new DisplayPartReset())
                {
                    foreach (string op in ops)
                    {
                        Part part = session_.__FindOrOpen(__folder.file_detail_000(op));

                        __display_part_ = part;

                        const string center_line_of_coil = "CenterLineofCoil";

                        Expression orig_exp = session_.__FindOrOpen(__folder.file_strip("900"))
                            .__FindExpression(center_line_of_coil);

                        __display_part_.__CreateInterpartExpression(orig_exp, center_line_of_coil);

                        Expression exp = __display_part_.__FindExpression("p2");

                        exp.SetFormula(center_line_of_coil);

                        __display_part_.Expressions.Rename(exp, $"{exp.GetFormula()}_");

                        __display_part_.Expressions.EditWithUnits(
                            exp,
                            exp.OwningPart.UnitCollection.ToArray().Single(__type => __type.TypeName == "MilliMeter"),
                            exp.GetFormula());

                        part.ModelingViews.WorkView.Fit();

                        part.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
                    }
                }

                using (new DisplayPartReset())
                {
                    add_presses(__folder);
                }

                using (new DisplayPartReset())
                {
                    __display_part_.Layers.MoveDisplayableObjects(256, __display_part_.DisplayedConstraints.ToArray());
                }

                using (new DisplayPartReset())
                {
                    foreach (string op in ops)
                        session_.__FindOrOpen(__folder.file_detail_000(op)).__Save();

                    foreach (string part_file in Directory.GetFiles(__folder.dir_op("900"), "*-000.prt",
                                 SearchOption.TopDirectoryOnly))
                        session_.__FindOrOpen(part_file).__Save();
                }

                using (new DisplayPartReset())
                {
                    Expression exp = __display_part_.__FindExpression("p2");
                    __display_part_.Expressions.Rename(exp, $"{exp.GetFormula()}_");
                    __display_part_.Expressions.EditWithUnits(
                        exp,
                        exp.OwningPart.UnitCollection.ToArray().Single(__type => __type.TypeName == "MilliMeter"),
                        exp.GetFormula());
                    __display_part_.__Fit();
                    __display_part_.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);
                }

                //ops


                using (session_.__usingDisplayPartReset())
                {
                    foreach (string op in ops)
                    {
                        string dieset_control = $"{__folder.CustomerNumber}-{op}-dieset-control.prt";

                        __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "002"));
                        __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                            dieset_control, true, true);

                        __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "012"));
                        __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                            dieset_control, true, true);

                        __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "502"));
                        __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                            dieset_control, true, true);

                        __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "512"));
                        __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                            dieset_control, true, true);
                    }
                }

                using (session_.__usingDisplayPartReset())
                {
                    foreach (string op in ops)
                        try
                        {
                            string dieset_control = $"{__folder.CustomerNumber}-{op}-dieset-control.prt";
                            __display_part_ = session_.__FindOrOpen(dieset_control);
                            __display_part_.Expressions.ChangeInterpartReferences("XXXXX-Press-XX-Assembly.prt",
                                $"{__folder.CustomerNumber}-T{op}-Press.prt", true, true);
                        }
                        catch (Exception ex)
                        {
                            ex.__PrintException($"{__folder.CustomerNumber}-T{op}-Press.prt");
                        }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void clone_transfer(GFolder __folder, string ops, bool chkStartWithBlank, bool units_mm)
        {
            //NXOpen.Session.GetSession().Parts.SetAllowMultipleDisplayedParts(true);

            string[] regex = Regex.Split(ops, "(?<split>3\\d\\d)") ?? throw new Exception("Could not parse ops");
            List<string> op_list = new List<string>(regex)
                .Where(__s => !string.IsNullOrEmpty(__s))
                .Where(__s => !string.IsNullOrWhiteSpace(__s))
                .ToList();

            IDictionary<string, string[]> __dict = new Dictionary<string, string[]>();

            for (int i = 0; i < op_list.Count; i += 2)
            {
                string shoe_key = op_list[i].Trim().Replace("  ", " ");

                string[] ops1 = op_list[i + 1].Trim().Replace("  ", " ").Split();

                __dict[shoe_key] = ops1;
            }

            List<string> all_ops_and_shoes = new List<string>();

            foreach (string key in __dict.Keys)
            {
                all_ops_and_shoes.Add(key);

                foreach (string op in __dict[key])
                    all_ops_and_shoes.Add(op);
            }

            foreach (string op in all_ops_and_shoes)
            {
                if (op.StartsWith("3"))
                {
                    if (!Directory.Exists(__folder.dir_op(op)))
                        Directory.CreateDirectory(__folder.dir_op(op));

                    ExecuteCloneTranShoe(__folder, op);
                    __display_part_ = session_.__FindOrOpen(__folder.file_detail_000(op));

                    using (new DisplayPartReset())
                    {
                        delete_unit_fasteners(__folder, op, units_mm);
                    }

                    continue;
                }

                if (!Directory.Exists(__folder.dir_op(op)))
                    Directory.CreateDirectory(__folder.dir_op(op));

                ExecuteCloneTranOp(__folder, op);
                __display_part_ = session_.__FindOrOpen(__folder.file_detail_000(op));
                __display_part_.__RightClickOpenAssemblyWhole();

                try
                {
                    if (!chkStartWithBlank)
                        replace_strip_layout_in_display(__folder.file_layout_t(op), "LAYOUT", true);
                    else if (op == "010")
                        replace_strip_layout_in_display(__folder.FileStrip900, "STRIP", true);
                    else
                        replace_strip_layout_in_display(__folder.file_layout_t(__Op020To010(op)), "LAYOUT", true);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                const string center_line_of_coil = "CenterLineofCoil";
                Expression orig_exp = session_.__FindOrOpen(__folder.file_strip("900"))
                    .__FindExpression(center_line_of_coil);
                __display_part_.__CreateInterpartExpression(orig_exp, center_line_of_coil);
                Expression exp = __display_part_.__FindExpression("datum_plane_12_offset");
                exp.OwningPart.Expressions.EditExpression(exp, center_line_of_coil);
                add_press(__folder, op);
            }

            //return;


            //NXOpen.Session.GetSession().Parts.SetAllowMultipleDisplayedParts(true);

            foreach (string op in all_ops_and_shoes)
            {
                Part p = session_.__FindOrOpen(__folder.file_detail0(op, "000"));

                __display_part_ = p;

                //NXOpen.Session.GetSession().set_active_display(p);
            }

            if (!Directory.Exists(__folder.dir_op("900")))
                Directory.CreateDirectory(__folder.dir_op("900"));

            foreach (string op in all_ops_and_shoes)
                if (!op.StartsWith("3"))
                    add_press(__folder, op);

            foreach (string key in __dict.Keys)
            {
                string[] tran_ops = __dict[key];

                string assembly_op = "";

                foreach (string tran_op in tran_ops)
                    assembly_op += $"{tran_op.ToCharArray()[1]}{tran_op.ToCharArray()[2]}";

                IDictionary<string, string> tran_assembly_op = new Dictionary<string, string>();

                tran_assembly_op[$"{SEED_DIRECTORY}\\seed-tran-op-000.prt"] =
                    __folder.file_dir_op_op_detail("900", assembly_op, "000");
                tran_assembly_op[$"{SEED_DIRECTORY}\\seed-tran-op-lsp.prt"] =
                    __folder.file_dir_op_op_detail("900", assembly_op, "lwr");
                tran_assembly_op[$"{SEED_DIRECTORY}\\seed-tran-op-usp.prt"] =
                    __folder.file_dir_op_op_detail("900", assembly_op, "upr");

                ExecuteCloneTranAssemblyOp(__folder.dir_op("900"), tran_assembly_op);

                //NXOpen.Session.GetSession().set_active_display(session_.find_or_open(__folder.path_detail1("900", assembly_op, "000")));
                __display_part_ = session_.__FindOrOpen(__folder.path_detail1("900", assembly_op, "000"));

                Component upr_assembly =
                    __display_part_.ComponentAssembly.RootComponent.__FindComponent(
                        $"COMPONENT {__folder.CustomerNumber}-{assembly_op}-upr 1");

                Component lwr_assembly =
                    __display_part_.ComponentAssembly.RootComponent.__FindComponent(
                        $"COMPONENT {__folder.CustomerNumber}-{assembly_op}-lwr 1");

                int feed_direction =
                    int.Parse(
                        $"{session_.__FindOrOpen(__folder.FileStrip900).__FindExpression("FeedDirection").Value}");

                Point3d origin = feed_direction > 0
                    ? new Point3d(10, 0, 0)
                    : new Point3d(-10, 0, 0);

                __display_part_ = upr_assembly.__Prototype();

                foreach (string tran_op in tran_ops)
                    __display_part_.__AddComponent(__folder.path_detail1("900", tran_op, "usp"), origin: origin);

                __display_part_ = lwr_assembly.__Prototype();

                foreach (string tran_op in tran_ops)
                    __display_part_.__AddComponent(__folder.path_detail1("900", tran_op, "lsp"), origin: origin);

                lwr_assembly.__Prototype().__AddComponent(__folder.path_detail1("900", key, "lsh"));

                upr_assembly.__Prototype().__AddComponent(__folder.path_detail1("900", key, "ush"));

                string tmep = __folder.path_detail1("900", assembly_op, "000");

                __display_part_ = session_.__FindOrOpen(tmep);

                if (File.Exists(__folder.FileStrip900))
                    replace_strip_layout_in_display(__folder.FileStrip900, "STRIP", true);
            }

            ExecuteCloneTran900(__folder);

            __display_part_ = session_.__FindOrOpen(__folder.path_detail1("900", "900", "000"));

            try
            {
                __display_part_.ComponentAssembly.RootComponent
                    .__FindComponent("COMPONENT seed-tran-op-lsp 1")
                    .__DeleteSelfAndConstraints();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            try
            {
                __display_part_.ComponentAssembly.RootComponent
                    .__FindComponent("COMPONENT seed-tran-op-usp 1")
                    .__DeleteSelfAndConstraints();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            using (new DisplayPartReset())
            {
                foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                             SearchOption.TopDirectoryOnly))
                {
                    string leaf = Path.GetFileNameWithoutExtension(__file);

                    if (leaf is null)
                        continue;

                    if (leaf.EndsWith("lwr") || leaf.EndsWith("upr"))
                        __display_part_.__AddComponent(__file);
                }
            }

            replace_strip_layout_in_display(__folder.FileStrip900, "STRIP", false);

            foreach (string key in __dict.Keys)
            {
                session_.__FindOrOpen(__folder.file_detail0(key, "000")).__Save();

                foreach (string op in __dict[key])
                    session_.__FindOrOpen(__folder.file_detail0(op, "000")).__Save();
            }


            foreach (string __file in Directory.GetFiles(__folder.DirLayout,
                         $"{__folder.CustomerNumber}-T*-Press.prt"))
            {
                string leaf = Path.GetFileName(__file);

                if (__file is null)
                    continue;

                Match match = Regex.Match(leaf, "^\\d+-T(?<op>\\d+)-Press", RegexOptions.IgnoreCase);

                if (!match.Success)
                    continue;

                add_press(__folder, match.Groups["op"].Value);
            }

            foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*-000.prt",
                         SearchOption.TopDirectoryOnly))
                session_.__FindOrOpen(__file).__Save();

            foreach (Component __child in __display_part_.ComponentAssembly.RootComponent.GetChildren())
            {
                __child.SetName(__child.DisplayName.Replace(__folder.CustomerNumber + "-", ""));

                if (__child.DisplayName.EndsWith("lwr") || __child.DisplayName.EndsWith("upr"))
                {
                    __child.__ReferenceSet("BODY");

                    if (__child.DisplayName.EndsWith("upr"))
                        __child.__Layer(101);
                    else
                        __child.__Layer(1);
                }
            }

            using (new DisplayPartReset())
            {
                foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                             SearchOption.TopDirectoryOnly))
                {
                    string leaf = Path.GetFileNameWithoutExtension(__file);

                    if (leaf is null)
                        continue;

                    if (!leaf.EndsWith("lwr") && !leaf.EndsWith("upr"))
                        continue;

                    bool is_lower = leaf.EndsWith("lwr");
                    __display_part_ = session_.__FindOrOpen(__file);

                    if (is_lower)
                        ConstrainLSH(__folder, chkStartWithBlank);
                    else
                        ConstrainUSH(__folder, chkStartWithBlank);
                }
            }

            using (new DisplayPartReset())
            {
                foreach (Component __c in __display_part_.ComponentAssembly.RootComponent.GetChildren())
                    if (__c.DisplayName.ToLower().EndsWith("-lwr") || __c.DisplayName.ToLower().EndsWith("-upr"))
                    {
                        __c.__ReferenceSet("MATE");
                        constrain_xy(__c, $"{__c.DisplayName}-XY");
                        constrain_xz(__c, $"{__c.DisplayName}-XZ");
                        constrain_yz(__c, $"{__c.DisplayName}-YZ");
                        __c.__ReferenceSet("BODY");
                    }
            }

            using (new DisplayPartReset())
            {
                foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                             SearchOption.TopDirectoryOnly))
                {
                    string leaf = Path.GetFileNameWithoutExtension(__file);

                    if (leaf is null)
                        continue;

                    if (!leaf.EndsWith("lwr") && !leaf.EndsWith("upr"))
                        continue;

                    __display_part_ = session_.__FindOrOpen(__file);

                    foreach (Component __c in __display_part_.ComponentAssembly.RootComponent.GetChildren())
                    {
                        if (__c.DisplayName.ToLower().Contains("strip"))
                            continue;

                        __c.__ReferenceSet("BODY");
                        __display_part_.GetAllReferenceSets().Single(__r => __r.Name == "BODY")
                            .AddObjectsToReferenceSet(new[] { __c });
                    }

                    __display_part_.Undisplay();
                }
            }

            using (new DisplayPartReset())
            {
                foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                             SearchOption.TopDirectoryOnly))
                {
                    string leaf = Path.GetFileNameWithoutExtension(__file);

                    if (leaf is null)
                        continue;

                    if (!leaf.EndsWith("lwr") && !leaf.EndsWith("upr"))
                        continue;

                    __display_part_ = session_.__FindOrOpen(__file);

                    foreach (Expression exp in session_.__FindOrOpen(__file).Expressions.ToArray())
                    {
                        Match match = Regex.Match(exp.GetFormula(), "^(?<name>T\\d{3}[XYZ])$");

                        if (!match.Success)
                            continue;

                        exp.OwningPart.Expressions.Rename(exp, $"{match.Groups["name"].Value}_");

                        UFSession.GetUFSession().Modl.Update();

                        exp.OwningPart.Expressions.EditWithUnits(exp,
                            exp.OwningPart.UnitCollection.ToArray().Single(__type => __type.TypeName == "MilliMeter"),
                            exp.GetFormula());
                    }

                    if (__display_part_.Leaf.ToLower().Contains("upr"))
                        foreach (Component __child in __display_part_.ComponentAssembly.RootComponent.GetChildren())
                        {
                            if (__child.Name.ToLower().Contains("strip"))
                                continue;

                            __child.__Layer(101);
                        }

                    __display_part_.Undisplay();
                }
            }

            using (new DisplayPartReset())
            {
                const string center_line_of_coil = "CenterLineofCoil";
                Expression orig_exp = session_.__FindOrOpen(__folder.file_strip("900"))
                    .__FindExpression(center_line_of_coil);
                __display_part_.__CreateInterpartExpression(orig_exp, center_line_of_coil);
                Expression exp = __display_part_.__FindExpression("datum_plane_12_offset");
                exp.SetFormula(center_line_of_coil);
                __display_part_.Expressions.Rename(exp, $"{exp.GetFormula()}_");
                __display_part_.Expressions.EditWithUnits(
                    exp,
                    exp.OwningPart.UnitCollection.ToArray().Single(__type => __type.TypeName == "MilliMeter"),
                    exp.GetFormula());
            }

            using (new DisplayPartReset())
            {
                __display_part_.Layers.MoveDisplayableObjects(256, __display_part_.DisplayedConstraints.ToArray());
            }

            using (new DisplayPartReset())
            {
                foreach (string op in all_ops_and_shoes)
                    session_.__FindOrOpen(__folder.file_detail_000(op)).__Save();

                foreach (string part_file in Directory.GetFiles(__folder.dir_op("900"), "*-000.prt",
                             SearchOption.TopDirectoryOnly))
                    session_.__FindOrOpen(part_file).__Save();
            }

            print_("/////////////////////////////////////");

            //System.Diagnostics.Debugger.Launch();

            // this is where you constrain the ops
            using (new DisplayPartReset())
            {
                if (chkStartWithBlank)
                    foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                                 SearchOption.TopDirectoryOnly))
                    {
                        string leaf = Path.GetFileNameWithoutExtension(__file);

                        if (leaf is null)
                            continue;

                        if (!leaf.EndsWith("lwr") && !leaf.EndsWith("upr"))
                            continue;

                        __display_part_ = session_.__FindOrOpen(__file);

                        Component[] lsp_usp_comps = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                            .Where(__c =>
                                __c.DisplayName.ToLower().EndsWith("-lsp") ||
                                __c.DisplayName.ToLower().EndsWith("-usp"))
                            .ToArray();

                        __display_part_.ComponentAssembly.ReplaceReferenceSetInOwners("MATE", lsp_usp_comps);

                        Expression p010x_exp =
                            session_.__FindOrOpen(__folder.file_strip("900")).__FindExpression("P010X");
                        __display_part_.__CreateInterpartExpression(p010x_exp, "P010X");
                        Expression p010y_exp =
                            session_.__FindOrOpen(__folder.file_strip("900")).__FindExpression("P010Y");
                        __display_part_.__CreateInterpartExpression(p010y_exp, "P010Y");
                        Expression p010z_exp =
                            session_.__FindOrOpen(__folder.file_strip("900")).__FindExpression("P010Z");
                        __display_part_.__CreateInterpartExpression(p010z_exp, "P010Z");

                        foreach (Component child in lsp_usp_comps)
                        {
                            Match match = Regex.Match(child.DisplayName, "^\\d+-(?<op>\\d+)-(usp|lsp)$",
                                RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                print_("Wasn't a success");
                                continue;
                            }

                            string op = match.Groups["op"].Value;

#pragma warning disable CS0168 // Variable is declared but never used
                            Expression exp;
#pragma warning restore CS0168 // Variable is declared but never used

                            if (op == "010")
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneYZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneYZ(),
                                    "P010X");

                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXZ(),
                                    "P010Y");

                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXY(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXY(),
                                    "P010Z");

                                continue;
                            }

                            string prev = __Op020To010(op);

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneYZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneYZ(),
                                    $"T{prev}X");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXZ(),
                                    $"T{prev}Y");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException($"T{prev}Y - {__display_part_.Leaf} - {child.DisplayName}");
                            }

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXY(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXY(),
                                    $"T{prev}Z");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }
                        }

                        foreach (Expression exp in __display_part_.Expressions.ToArray())
                        {
                            if (exp.IsInterpartExpression)
                                continue;

                            Match match = Regex.Match(exp.GetFormula(), "^(?<pt>P|T)(?<op>\\d+)(?<xyz>X|Y|Z)$");

                            if (!match.Success)
                                continue;

                            exp.OwningPart.Expressions.Rename(exp, $"{exp.GetFormula()}_");

                            exp.OwningPart.Expressions.EditWithUnits(exp,
                                exp.OwningPart.UnitCollection.ToArray()
                                    .Single(__type => __type.TypeName == "MilliMeter"), exp.GetFormula());
                        }

                        //__display_part_.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", lsp_usp_comps);
                    }
                else
                    foreach (string __file in Directory.GetFiles(__folder.dir_op("900"), "*.prt",
                                 SearchOption.TopDirectoryOnly))
                    {
                        string leaf = Path.GetFileNameWithoutExtension(__file);

                        if (leaf is null)
                            continue;

                        if (!leaf.EndsWith("lwr") && !leaf.EndsWith("upr"))
                            continue;

                        __display_part_ = session_.__FindOrOpen(__file);

                        Component[] lsp_usp_comps = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                            .Where(__c =>
                                __c.DisplayName.ToLower().EndsWith("-lsp") ||
                                __c.DisplayName.ToLower().EndsWith("-usp"))
                            .ToArray();

                        __display_part_.ComponentAssembly.ReplaceReferenceSetInOwners("MATE", lsp_usp_comps);

                        foreach (Component child in lsp_usp_comps)
                        {
                            Match match = Regex.Match(child.DisplayName, "^\\d+-(?<op>\\d+)-(usp|lsp)$",
                                RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                print_("Wasn't a success");
                                continue;
                            }

                            string op = match.Groups["op"].Value;

                            string prev = __Op020To010(op);

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneYZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneYZ(),
                                    $"T{op}X");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXZ(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXZ(),
                                    $"T{op}Y");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }

                            try
                            {
                                __display_part_.__ConstrainOccProtoDistance(
                                    child.__AbsOccDatumPlaneXY(),
                                    __display_part_.__AbsoluteDatumCsys().__DatumPlaneXY(),
                                    $"T{op}Z");
                            }
                            catch (Exception ex)
                            {
                                ex.__PrintException();
                            }
                        }

                        foreach (Expression exp in __display_part_.Expressions.ToArray())
                        {
                            if (exp.IsInterpartExpression)
                                continue;

                            Match match = Regex.Match(exp.GetFormula(), "^(?<pt>P|T)(?<op>\\d+)(?<xyz>X|Y|Z)$");

                            if (!match.Success)
                                continue;

                            exp.OwningPart.Expressions.Rename(exp, $"{exp.GetFormula()}_");

                            exp.OwningPart.Expressions.EditWithUnits(exp,
                                exp.OwningPart.UnitCollection.ToArray()
                                    .Single(__type => __type.TypeName == "MilliMeter"), exp.GetFormula());
                        }

                        //__display_part_.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", lsp_usp_comps);
                    }
            }


            using (session_.__usingDisplayPartReset())
            {
                foreach (string op in all_ops_and_shoes)
                {
                    if (!op.StartsWith("3"))
                        continue;

                    string dieset_control = $"{__folder.CustomerNumber}-{op}-dieset-control.prt";

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "002"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "012"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "502"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);

                    __display_part_ = session_.__FindOrOpen(__folder.file_detail0(op, "512"));
                    __display_part_.Expressions.ChangeInterpartReferences("seed-prog-dieset-control.prt",
                        dieset_control, true, true);
                }
            }

            //System.Diagnostics.Debugger.Launch();

            //var exps = session_.find_or_open(__folder.file_strip_900).Expressions.ToArray();

            foreach (string op in all_ops_and_shoes.Append("900"))
            foreach (string part000 in Directory.GetFiles(__folder.dir_op(op), "*-000.prt",
                         SearchOption.TopDirectoryOnly))
                session_.__FindOrOpen(part000).__Save(true);

            print_("Designer will have to link up presses to proper dieset control manualy.");
        }

        public static void add_presses(GFolder __folder)
        {
            try
            {
                List<Part> __press_parts = new List<Part>();

                foreach (string __file in Directory.EnumerateFiles(__folder.DirLayout))
                {
                    if (!__file.ToLower().Contains("press"))
                        continue;
                    __press_parts.Add(session_.__FindOrOpen(__file));
                }

                __display_part_.ComponentAssembly.AddComponents(
                    __press_parts.ToArray(),
                    _Point3dOrigin,
                    _Matrix3x3Identity,
                    245,
                    "Entire Part",
                    1,
                    false,
                    out Component[] __press_comps);

                foreach (Component __press in __press_comps)
                {
                    constrain_xz_to_datum_plane(__press);
                    constrain_xy(__press, "PRESS-XY");
                    constrain_yz(__press, "PRESS-YZ");
                }

                __display_part_.__ReplaceRefSets(__press_comps, "BODY");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void ConstrainOpsToLwrUpr(GFolder __folder, string[] ops, string lwrupr, int layer)
        {
            int feed_direction =
                int.Parse($"{session_.__FindOrOpen(__folder.FileStrip900).__FindExpression("FeedDirection").Value}");

            Point3d origin = feed_direction > 0
                ? new Point3d(10, 0, 0)
                : new Point3d(-10, 0, 0);

            __display_part_ = session_.__FindOrOpen(__folder.file_detail0("900", lwrupr));
            __display_part_.ComponentAssembly.RootComponent.GetChildren()
                .Single(__c => __c.Name == "STRIP")
                .__ReferenceSet("BODY");

            foreach (string op in ops)
            {
                Component comp_lwr = __display_part_.__AddComponent(
                    __folder.file_detail0(op, lwrupr),
                    "MATE",
                    layer: layer,
                    origin: origin);

                string new_interpart_exp_x = $"T{op}X";
                string new_interpart_exp_y = $"T{op}Y";
                string new_interpart_exp_z = $"T{op}Z";
                InterpartExpressionsBuilder builder = __display_part_.Expressions.CreateInterpartExpressionsBuilder();
                Part strip = session_.__FindOrOpen(__folder.FileStrip900);
                try
                {
                    ConstrainStripInterpartExpressions(new_interpart_exp_x, new_interpart_exp_y, new_interpart_exp_z,
                        builder, strip);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                constrain_xy1(comp_lwr, new_interpart_exp_z, "XY");
                constraint_xz_1(comp_lwr, new_interpart_exp_y, "XZ");
                NewMethod21(comp_lwr, new_interpart_exp_x, "YZ");
                comp_lwr.__ReferenceSet("BODY");
            }
        }

        private static void NewMethod21(Component comp_lwr, string new_interpart_exp_x, string suffix)
        {
            DatumPlane datum_plane1 = comp_lwr.__AbsOccDatumPlaneYZ();

            DatumPlane datum_plane2 = __display_part_.__AbsoluteDatumCsys().__DatumPlaneYZ();

            ConstrainUnits(comp_lwr, new_interpart_exp_x, datum_plane1, datum_plane2, suffix);
        }

        private static void constraint_xz_1(Component comp_lwr, string new_interpart_exp_y, string suffix)
        {
            DatumPlane datum_plane1 = comp_lwr.__AbsOccDatumPlaneXZ();
            DatumPlane datum_plane2 = __display_part_.__AbsoluteDatumCsys().__DatumPlaneXZ();
            ConstrainUnits(comp_lwr, new_interpart_exp_y, datum_plane1, datum_plane2, suffix);
        }

        private static void constrain_xy1(Component comp_lwr, string new_interpart_exp_z, string suffix)
        {
            DatumPlane datum_plane1 = comp_lwr.__AbsOccDatumPlaneXY();

            DatumPlane datum_plane2 = __display_part_.__AbsoluteDatumCsys().__DatumPlaneXY();

            ConstrainUnits(comp_lwr, new_interpart_exp_z, datum_plane1, datum_plane2, suffix);
        }

        private static void ConstrainUnits(
            Component comp_upr,
            string new_interpart_exp_x,
            DatumPlane datum_plane1,
            DatumPlane datum_plane2,
            string suffix)
        {
            ComponentConstraint comp_dist_const_yz = __display_part_.__ConstrainOccProtoDistance(
                datum_plane1,
                datum_plane2,
                new_interpart_exp_x);
            comp_dist_const_yz.GetDisplayedConstraint().__Layer(256);
            comp_dist_const_yz.SetName($"{comp_upr.Name}-{suffix}");
            Expression exp = __display_part_.Expressions
                .ToArray()
                .Single(__e => __e.GetFormula() == new_interpart_exp_x);
            exp.OwningPart.Expressions.EditWithUnits(
                exp,
                exp.OwningPart.UnitCollection.ToArray().Single(__type => __type.TypeName == "MilliMeter"),
                exp.GetFormula());
            exp.OwningPart.Expressions.Rename(exp, $"{new_interpart_exp_x}_");
        }

        public static void replace_strip_layout_in_display(string path, string name, bool replace_all)
        {
            __display_part_.ComponentAssembly.RootComponent
                .__FindComponent("COMPONENT seed-strip-layout 1")
                .__ReplaceComponent(path, name, replace_all);
        }

        public static void delete_unit_fasteners(GFolder __folder, string op, bool units_mm)
        {
            try
            {
                Part __current = __display_part_;

                using (session_.__usingDisplayPartReset())
                {
                    Part[] __parallels =
                    {
                        session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-002"),
                        session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-012"),
                        session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-502"),
                        session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-512")
                    };

                    foreach (Part __par in __parallels)
                    {
                        __display_part_ = __par;

                        __display_part_.__RightClickOpenAssemblyWhole();

                        {
                            session_.UpdateManager.ClearErrorList();

                            Session.UndoMarkId _id = session_.SetUndoMark(Session.MarkVisibility.Visible, "parallels");


                            if (!units_mm)
                                session_.UpdateManager.AddObjectsToDeleteList(__display_part_.DisplayedConstraints
                                    .ToArray().Where(__c => __c.Layer == 255).ToArray());
                            else
                                session_.UpdateManager.AddObjectsToDeleteList(__display_part_.DisplayedConstraints
                                    .ToArray().Where(__c => __c.Layer != 255).ToArray());


                            session_.UpdateManager.DoUpdate(_id);
                        }

                        {
                            session_.UpdateManager.ClearErrorList();

                            Session.UndoMarkId _id = session_.SetUndoMark(Session.MarkVisibility.Visible, "parallels");

                            foreach (Component __child in __par.ComponentAssembly.RootComponent.GetChildren())
                            {
                                if (__child.Name.ToUpper().Contains("MM") && !units_mm)
                                {
                                    session_.UpdateManager.AddObjectsToDeleteList(__child.GetConstraints()
                                        .Select(__c => __c.GetDisplayedConstraint()).ToArray());
                                    session_.UpdateManager.AddObjectsToDeleteList(new[] { __child });
                                }

                                if (!__child.Name.ToUpper().Contains("MM") && units_mm)
                                {
                                    session_.UpdateManager.AddObjectsToDeleteList(__child.GetConstraints()
                                        .Select(__c => __c.GetDisplayedConstraint()).ToArray());
                                    session_.UpdateManager.AddObjectsToDeleteList(new[] { __child });
                                }
                            }

                            session_.UpdateManager.DoUpdate(_id);
                        }
                    }
                }

                using (session_.__usingDisplayPartReset())
                {
                    session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-lwrplate").__SetAsDisplayPart();

                    Session theSession = Session.GetSession();

                    Session.UndoMarkId markId2;
                    markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");


                    if (units_mm)
                    {
                        Part workPart = __work_part_;
                        {
                            TaggedObject[] objects1 = new TaggedObject[16];

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;
                            DisplayedConstraint displayedConstraint1 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21229");
                            objects1[0] = displayedConstraint1;
                            DisplayedConstraint displayedConstraint2 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21244");
                            objects1[1] = displayedConstraint2;
                            DisplayedConstraint displayedConstraint3 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21259");
                            objects1[2] = displayedConstraint3;
                            DisplayedConstraint displayedConstraint4 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21274");
                            objects1[3] = displayedConstraint4;
                            DisplayedConstraint displayedConstraint5 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21287");
                            objects1[4] = displayedConstraint5;
                            DisplayedConstraint displayedConstraint6 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21302");
                            objects1[5] = displayedConstraint6;
                            DisplayedConstraint displayedConstraint7 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21316");
                            objects1[6] = displayedConstraint7;
                            DisplayedConstraint displayedConstraint8 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21330");
                            objects1[7] = displayedConstraint8;
                            DisplayedConstraint displayedConstraint9 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21348");
                            objects1[8] = displayedConstraint9;
                            DisplayedConstraint displayedConstraint10 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21357");
                            objects1[9] = displayedConstraint10;
                            DisplayedConstraint displayedConstraint11 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21377");
                            objects1[10] = displayedConstraint11;
                            DisplayedConstraint displayedConstraint12 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21388");
                            objects1[11] = displayedConstraint12;
                            DisplayedConstraint displayedConstraint13 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21407");
                            objects1[12] = displayedConstraint13;
                            DisplayedConstraint displayedConstraint14 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21416");
                            objects1[13] = displayedConstraint14;
                            DisplayedConstraint displayedConstraint15 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21425");
                            objects1[14] = displayedConstraint15;
                            DisplayedConstraint displayedConstraint16 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-21451");
                            objects1[15] = displayedConstraint16;
                            int nErrs1;
                            nErrs1 = session_.UpdateManager.AddObjectsToDeleteList(objects1);

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);
                        }

                        {
                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");
                            TaggedObject[] objects1 = new TaggedObject[8];
                            Component component1 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 7");
                            objects1[0] = component1;
                            Component component2 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 3");
                            objects1[1] = component2;
                            Component component3 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 1");
                            objects1[2] = component3;
                            Component component4 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 8");
                            objects1[3] = component4;
                            Component component5 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 5");
                            objects1[4] = component5;
                            Component component6 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 2");
                            objects1[5] = component6;
                            Component component7 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 6");
                            objects1[6] = component7;
                            Component component8 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 4");
                            objects1[7] = component8;
                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);


                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

                            //theSession.DeleteUndoMark(markId1, null);
                        }
                    }
                    else
                    {
                        {
                            //NXOpen.Session theSession = NXOpen.Session.GetSession();
                            Part workPart = theSession.Parts.Work;
                            Part displayPart = theSession.Parts.Display;
                            // ----------------------------------------------
                            //   Menu: Edit->Delete...
                            // ----------------------------------------------
                            Session.UndoMarkId markId1;
                            markId1 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Delete");

                            theSession.UpdateManager.ClearErrorList();

                            //NXOpen.Session.UndoMarkId markId2;
                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");

                            TaggedObject[] objects1 = new TaggedObject[16];
                            DisplayedConstraint displayedConstraint1 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2213");
                            objects1[0] = displayedConstraint1;
                            DisplayedConstraint displayedConstraint2 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2254");
                            objects1[1] = displayedConstraint2;
                            DisplayedConstraint displayedConstraint3 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2312");
                            objects1[2] = displayedConstraint3;
                            DisplayedConstraint displayedConstraint4 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2354");
                            objects1[3] = displayedConstraint4;
                            DisplayedConstraint displayedConstraint5 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2649");
                            objects1[4] = displayedConstraint5;
                            DisplayedConstraint displayedConstraint6 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2703");
                            objects1[5] = displayedConstraint6;
                            DisplayedConstraint displayedConstraint7 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2762");
                            objects1[6] = displayedConstraint7;
                            DisplayedConstraint displayedConstraint8 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2819");
                            objects1[7] = displayedConstraint8;
                            DisplayedConstraint displayedConstraint9 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3102");
                            objects1[8] = displayedConstraint9;
                            DisplayedConstraint displayedConstraint10 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3160");
                            objects1[9] = displayedConstraint10;
                            DisplayedConstraint displayedConstraint11 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3232");
                            objects1[10] = displayedConstraint11;
                            DisplayedConstraint displayedConstraint12 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3291");
                            objects1[11] = displayedConstraint12;
                            DisplayedConstraint displayedConstraint13 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3572");
                            objects1[12] = displayedConstraint13;
                            DisplayedConstraint displayedConstraint14 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3628");
                            objects1[13] = displayedConstraint14;
                            DisplayedConstraint displayedConstraint15 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3686");
                            objects1[14] = displayedConstraint15;
                            DisplayedConstraint displayedConstraint16 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-3743");
                            objects1[15] = displayedConstraint16;
                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);

                            theSession.DeleteUndoMark(markId1, null);
                        }

                        {
                            //NXOpen.Session theSession = NXOpen.Session.GetSession();
                            Part workPart = theSession.Parts.Work;
                            Part displayPart = theSession.Parts.Display;
                            // ----------------------------------------------
                            //   Menu: Edit->Delete...
                            // ----------------------------------------------
                            Session.UndoMarkId markId1;
                            markId1 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Delete");

                            theSession.UpdateManager.ClearErrorList();

                            //NXOpen.Session.UndoMarkId markId2;
                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");

                            TaggedObject[] objects1 = new TaggedObject[8];
                            Component component1 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 7");
                            objects1[0] = component1;
                            Component component2 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 1");
                            objects1[1] = component2;
                            Component component3 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 6");
                            objects1[2] = component3;
                            Component component4 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 3");
                            objects1[3] = component4;
                            Component component5 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 4");
                            objects1[4] = component5;
                            Component component6 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 8");
                            objects1[5] = component6;
                            Component component7 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 5");
                            objects1[6] = component7;
                            Component component8 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 2");
                            objects1[7] = component8;
                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);

                            theSession.DeleteUndoMark(markId1, null);
                        }
                    }
                }

                using (session_.__usingDisplayPartReset())
                {
                    session_.__FindOrOpen($"{__folder.CustomerNumber}-{op}-uprplate").__SetAsDisplayPart();

                    Session theSession = Session.GetSession();

                    Session.UndoMarkId markId2;
                    markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");


                    if (units_mm)
                    {
                        Part workPart = __work_part_;
                        {
                            TaggedObject[] objects1 = new TaggedObject[17];

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;
                            DisplayedConstraint displayedConstraint1 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2145");
                            objects1[0] = displayedConstraint1;
                            DisplayedConstraint displayedConstraint2 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2163");
                            objects1[1] = displayedConstraint2;
                            DisplayedConstraint displayedConstraint3 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2185");
                            objects1[2] = displayedConstraint3;
                            DisplayedConstraint displayedConstraint4 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2201");
                            objects1[3] = displayedConstraint4;
                            DisplayedConstraint displayedConstraint5 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2220");
                            objects1[4] = displayedConstraint5;
                            DisplayedConstraint displayedConstraint6 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2242");
                            objects1[5] = displayedConstraint6;
                            DisplayedConstraint displayedConstraint7 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2264");
                            objects1[6] = displayedConstraint7;
                            DisplayedConstraint displayedConstraint8 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2288");
                            objects1[7] = displayedConstraint8;
                            DisplayedConstraint displayedConstraint9 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2308");
                            objects1[8] = displayedConstraint9;
                            DisplayedConstraint displayedConstraint10 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2332");
                            objects1[9] = displayedConstraint10;
                            DisplayedConstraint displayedConstraint11 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2352");
                            objects1[10] = displayedConstraint11;
                            DisplayedConstraint displayedConstraint12 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2370");
                            objects1[11] = displayedConstraint12;
                            DisplayedConstraint displayedConstraint13 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2390");
                            objects1[12] = displayedConstraint13;
                            DisplayedConstraint displayedConstraint14 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2408");
                            objects1[13] = displayedConstraint14;
                            DisplayedConstraint displayedConstraint15 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2426");
                            objects1[14] = displayedConstraint15;
                            DisplayedConstraint displayedConstraint16 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2448");
                            objects1[15] = displayedConstraint16;
                            DisplayedConstraint displayedConstraint17 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2471");
                            objects1[16] = displayedConstraint17;

                            int nErrs1;
                            nErrs1 = session_.UpdateManager.AddObjectsToDeleteList(objects1);

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);
                        }

                        {
                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");
                            TaggedObject[] objects1 = new TaggedObject[8];
                            Component component1 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 5");
                            objects1[0] = component1;
                            Component component2 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 7");
                            objects1[1] = component2;
                            Component component3 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 3");
                            objects1[2] = component3;
                            Component component4 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 2");
                            objects1[3] = component4;
                            Component component5 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 4");
                            objects1[4] = component5;
                            Component component6 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 8");
                            objects1[5] = component6;
                            Component component7 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 6");
                            objects1[6] = component7;
                            Component component8 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 1500-shcs-500 1");
                            objects1[7] = component8;

                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);


                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

                            //theSession.DeleteUndoMark(markId1, null);
                        }
                    }
                    else
                    {
                        {
                            Part workPart = theSession.Parts.Work;
                            Part displayPart = theSession.Parts.Display;
                            // ----------------------------------------------
                            //   Menu: Edit->Delete...
                            // ----------------------------------------------
                            Session.UndoMarkId markId1;
                            markId1 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Delete");

                            theSession.UpdateManager.ClearErrorList();

                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");

                            TaggedObject[] objects1 = new TaggedObject[16];
                            DisplayedConstraint displayedConstraint1 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2893");
                            objects1[0] = displayedConstraint1;
                            DisplayedConstraint displayedConstraint2 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2911");
                            objects1[1] = displayedConstraint2;
                            DisplayedConstraint displayedConstraint3 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2498");
                            objects1[2] = displayedConstraint3;
                            DisplayedConstraint displayedConstraint4 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2519");
                            objects1[3] = displayedConstraint4;
                            DisplayedConstraint displayedConstraint5 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2542");
                            objects1[4] = displayedConstraint5;
                            DisplayedConstraint displayedConstraint6 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2560");
                            objects1[5] = displayedConstraint6;
                            DisplayedConstraint displayedConstraint7 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2582");
                            objects1[6] = displayedConstraint7;
                            DisplayedConstraint displayedConstraint8 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2608");
                            objects1[7] = displayedConstraint8;
                            DisplayedConstraint displayedConstraint9 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2637");
                            objects1[8] = displayedConstraint9;
                            DisplayedConstraint displayedConstraint10 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2660");
                            objects1[9] = displayedConstraint10;
                            DisplayedConstraint displayedConstraint11 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2708");
                            objects1[10] = displayedConstraint11;
                            DisplayedConstraint displayedConstraint12 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2728");
                            objects1[11] = displayedConstraint12;
                            DisplayedConstraint displayedConstraint13 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2760");
                            objects1[12] = displayedConstraint13;
                            DisplayedConstraint displayedConstraint14 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2796");
                            objects1[13] = displayedConstraint14;
                            DisplayedConstraint displayedConstraint15 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2817");
                            objects1[14] = displayedConstraint15;
                            DisplayedConstraint displayedConstraint16 =
                                (DisplayedConstraint)workPart.FindObject("HANDLE R-2832");
                            objects1[15] = displayedConstraint16;
                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);

                            theSession.DeleteUndoMark(markId1, null);
                        }

                        {
                            //NXOpen.Session theSession = NXOpen.Session.GetSession();
                            Part workPart = theSession.Parts.Work;
                            Part displayPart = theSession.Parts.Display;
                            // ----------------------------------------------
                            //   Menu: Edit->Delete...
                            // ----------------------------------------------
                            Session.UndoMarkId markId1;
                            markId1 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Delete");

                            theSession.UpdateManager.ClearErrorList();

                            //NXOpen.Session.UndoMarkId markId2;
                            markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Delete");

                            TaggedObject[] objects1 = new TaggedObject[8];
                            Component component1 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 8");
                            objects1[0] = component1;
                            Component component2 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 5");
                            objects1[1] = component2;
                            Component component3 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 4");
                            objects1[2] = component3;
                            Component component4 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 1");
                            objects1[3] = component4;
                            Component component5 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 3");
                            objects1[4] = component5;
                            Component component6 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 7");
                            objects1[5] = component6;
                            Component component7 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 6");
                            objects1[6] = component7;
                            Component component8 =
                                (Component)workPart.ComponentAssembly.RootComponent.FindObject(
                                    "COMPONENT 36mm-shcs-100 2");
                            objects1[7] = component8;
                            int nErrs1;
                            nErrs1 = theSession.UpdateManager.AddObjectsToDeleteList(objects1);

                            Session.UndoMarkId id1;
                            id1 = theSession.NewestVisibleUndoMark;

                            int nErrs2;
                            nErrs2 = theSession.UpdateManager.DoUpdate(id1);

                            theSession.DeleteUndoMark(markId1, null);
                        }
                    }
                }

                using (session_.__usingDisplayPartReset())
                {
                    session_.__FindOrOpen(__folder.file_detail0(op, "lsh")).__SetActiveDisplay();

                    if (units_mm)
                    {
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin English 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin English 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin English 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin English 1")
                            .__DeleteSelfAndConstraints();

                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing English 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing English 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing English 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing English 1")
                            .__DeleteSelfAndConstraints();

                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-E-StopBlock-specify hgt-groove 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-E-StopBlock-specify hgt-groove 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-E-StopBlock-specify hgt-groove 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-E-StopBlock-specify hgt-groove 1")
                            .__DeleteSelfAndConstraints();
                    }
                    else
                    {
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin Metric 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin Metric 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin Metric 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Guide Pin Metric 1")
                            .__DeleteSelfAndConstraints();

                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing Metric 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing Metric 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing Metric 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart Demountable Bushing Metric 1")
                            .__DeleteSelfAndConstraints();

                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-M-StopBlock-specify hgt-groove 4")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-M-StopBlock-specify hgt-groove 3")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-M-StopBlock-specify hgt-groove 2")
                            .__DeleteSelfAndConstraints();
                        __display_part_.ComponentAssembly.RootComponent
                            .__FindComponent("COMPONENT Smart TSG-M-StopBlock-specify hgt-groove 1")
                            .__DeleteSelfAndConstraints();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void add_press(GFolder __folder, string op)
        {
            try
            {
                string press_path = __folder.file_press_t(op);

                if (!File.Exists(press_path))
                {
                    print_("////////////////////////////");
                    print_($"Did not add press to {__display_part_.Leaf}");
                    print_($"Could not find press path {press_path}");
                    print_("////////////////////////////");
                    return;
                }

                Part __press_part = session_.__FindOrOpen(press_path);

                Component __press = __display_part_.ComponentAssembly.AddComponent(
                    __press_part,
                    "Entire Part",
                    $"PRESS-T{op}",
                    _Point3dOrigin,
                    _Matrix3x3Identity,
                    245,
                    out PartLoadStatus __press_comps);

                try
                {
                    constrain_xz_to_datum_plane(__press);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                constrain_xy(__press, "PRESS-XY");

                constrain_yz(__press, "PRESS-YZ");

                //__press.reference_set("BODY");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void ConstrainLSH(GFolder __folder, bool start_with_strip)
        {
            Component comp_lsh = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                .Single(__c => __c.DisplayName.ToLower().EndsWith("-lsh"));

            try
            {
                constrain_xy(comp_lsh, "LSH-XY");
                constrain_xz(comp_lsh, "LSH-XZ");
                constrain_yz(comp_lsh, "LSH-YZ");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            foreach (Component __child in __display_part_.ComponentAssembly.RootComponent.GetChildren())
            {
                if (!__child.DisplayName.EndsWith("-lsp"))
                    continue;

                string actual_op = __child.DisplayName
                    .Replace($"{__folder.CustomerNumber}-", "")
                    .Replace("-lsp", "");

                DatumCsys datum_csys = __child.__Prototype().__AbsoluteDatumCsys();

                string new_interpart_exp_x;
                string new_interpart_exp_y;
                string new_interpart_exp_z;

                string exp_op = actual_op;

                if (start_with_strip)
                    exp_op = __Op020To010(actual_op);

                if (actual_op == "010" && start_with_strip)
                {
                    new_interpart_exp_x = "P010X";
                    new_interpart_exp_y = "P010Y";
                    new_interpart_exp_z = "P010Z";
                }
                else
                {
                    new_interpart_exp_x = $"T{exp_op}X";
                    new_interpart_exp_y = $"T{exp_op}Y";
                    new_interpart_exp_z = $"T{exp_op}Z";
                }

                InterpartExpressionsBuilder builder = __display_part_.Expressions.CreateInterpartExpressionsBuilder();

                // clone transfer

                Part strip = session_.__FindOrOpen(__folder.FileStrip900);

                try
                {
                    ConstrainStripInterpartExpressions(new_interpart_exp_x, new_interpart_exp_y, new_interpart_exp_z,
                        builder, strip);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private static void ConstrainUSH(GFolder __folder, bool start_with_strip)
        {
            Component comp_ush = __display_part_.ComponentAssembly.RootComponent.GetChildren()
                .Single(__c => __c.DisplayName.ToLower().EndsWith("-ush"));

            try
            {
                constrain_xy(comp_ush, "USH-XY");
                constrain_xz(comp_ush, "USH-XZ");
                constrain_yz(comp_ush, "USH-YZ");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            foreach (Component __child in __display_part_.ComponentAssembly.RootComponent.GetChildren())
            {
                if (!__child.DisplayName.EndsWith("-usp"))
                    continue;

                string actual_op = __child.DisplayName
                    .Replace($"{__folder.CustomerNumber}-", "")
                    .Replace("-usp", "");

                DatumCsys datum_csys = __child.__Prototype().__AbsoluteDatumCsys();


                string new_interpart_exp_x;
                string new_interpart_exp_y;
                string new_interpart_exp_z;

                string exp_op = actual_op;

                if (start_with_strip)
                    exp_op = __Op020To010(actual_op);

                if (actual_op == "010" && start_with_strip)
                {
                    new_interpart_exp_x = "P010X";
                    new_interpart_exp_y = "P010Y";
                    new_interpart_exp_z = "P010Z";
                }
                else
                {
                    new_interpart_exp_x = $"T{exp_op}X";
                    new_interpart_exp_y = $"T{exp_op}Y";
                    new_interpart_exp_z = $"T{exp_op}Z";
                }

                InterpartExpressionsBuilder builder = __display_part_.Expressions.CreateInterpartExpressionsBuilder();

                Part strip = session_.__FindOrOpen(__folder.FileStrip900);
                try
                {
                    ConstrainStripInterpartExpressions(new_interpart_exp_x, new_interpart_exp_y, new_interpart_exp_z,
                        builder, strip);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }


        public static void constrain_xz_to_datum_plane(Component __press)
        {
            try
            {
                DatumPlane datum_plane1 = (DatumPlane)__press.FindObject("PROTO#HANDLE R-807");
                DatumPlane datum_plane2 = (DatumPlane)__display_part_.Datums.FindObject("DATUM_PLANE(12)");
                ComponentConstraint constraint =
                    __display_part_.__ConstrainOccProtoDistance(datum_plane1, datum_plane2, "0");
                constraint.GetDisplayedConstraint().SetName("PRESS-XZ");
            }
            catch (NXException __ex)
            {
                if (__ex.ErrorCode != 3520016)
                    throw;

                throw NXException.Create(__ex.ErrorCode, $"Could not find object {__display_part_.Leaf}");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void constrain_xy(Component __press, string constraint_name)
        {
            try
            {
                DatumPlane datum_plane1 = __press.__AbsOccDatumPlaneXY();
                DatumPlane datum_plane2 = __work_part_.__AbsoluteDatumCsys().__DatumPlaneXY();
                ComponentConstraint constraint =
                    __display_part_.__ConstrainOccProtoDistance(datum_plane1, datum_plane2, "0");
                constraint.SetName(constraint_name);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void constrain_xz(Component __press, string constraint_name)
        {
            try
            {
                DatumPlane datum_plane1 = __press.__AbsOccDatumPlaneXZ();
                DatumPlane datum_plane2 = __work_part_.__AbsoluteDatumCsys().__DatumPlaneXZ();
                ComponentConstraint constraint =
                    __display_part_.__ConstrainOccProtoDistance(datum_plane1, datum_plane2, "0");
                constraint.SetName(constraint_name);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void constrain_yz(Component __press, string constraint_name)
        {
            try
            {
                DatumPlane datum_plane1 = __press.__AbsOccDatumPlaneYZ();
                DatumPlane datum_plane2 = __work_part_.__AbsoluteDatumCsys().__DatumPlaneYZ();
                ComponentConstraint constraint =
                    __display_part_.__ConstrainOccProtoDistance(datum_plane1, datum_plane2, "0");
                constraint.SetName(constraint_name);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static void NewMethod43(string prefix, string op_str, Component press_comp, DatumCsys proto_press_csys,
            DatumCsys strip_csys0)
        {
            // constrains the xy planes
            NewMethod25(prefix, op_str, press_comp, proto_press_csys, strip_csys0);

            // constrains the yz planes
            ComponentConstraint constraint_yz = __display_part_.__ConstrainOccProtoDistance(
                press_comp.__AbsOccDatumPlaneYZ(),
                strip_csys0.__DatumPlaneYZ(),
                "0.0");
            constraint_yz.SetName($"PRESS-{prefix}{op_str}-YZ");
            constraint_yz.GetDisplayedConstraint().__Layer(256);

            // constrains the xz planes
            NewMethod27(prefix, op_str, press_comp, proto_press_csys);
        }

        private static void NewMethod27(
            string prefix,
            string op_str,
            Component press_comp,
            DatumCsys proto_press_csys)
        {
            NXObject __entity = __display_part_.Features
                .ToArray()
                .OfType<DatumPlaneFeature>()
                .ElementAt(1)
                .GetEntities()[0];
            ComponentConstraint constraint_xz = __display_part_.__ConstrainOccProtoDistance(
                (DatumPlane)press_comp.__FindOccurrence(proto_press_csys.__DatumPlaneYZ()),
                (DatumPlane)__entity,
                "0.0");
            constraint_xz.SetName($"PRESS-{prefix}{op_str}-XZ");
            constraint_xz.GetDisplayedConstraint().__Layer(256);
            //throw new NotImplementedException();
        }

        private static void NewMethod26(
            string prefix,
            string op_str,
            Component press_comp,
            DatumCsys proto_press_csys,
            DatumPlane plane)
        {
            ComponentConstraint constraint_yz = __display_part_.__ConstrainOccProtoDistance(
                press_comp.__AbsOccDatumPlaneYZ(),
                plane,
                "0.0");
            constraint_yz.SetName($"PRESS-{prefix}{op_str}-YZ");
            constraint_yz.GetDisplayedConstraint().__Layer(256);
        }

        private static void NewMethod25(string prefix, string op_str, Component press_comp, DatumCsys proto_press_csys,
            DatumCsys strip_csys0)
        {
            ComponentConstraint constraint_xy = __display_part_.__ConstrainOccProtoDistance(
                press_comp.__AbsOccDatumPlaneXY(),
                strip_csys0.__DatumPlaneXY(),
                "0.0");
            constraint_xy.SetName($"PRESS-{prefix}{op_str}-XY");
            constraint_xy.GetDisplayedConstraint().__Layer(256);
        }

        public static void add_component_and_constrain(
            string layout_path,
            string op_str,
            int layer,
            string comp_name,
            string prefix,
            string exp_x,
            string exp_y,
            string exp_z)
        {
            try
            {
                Expression expression_x = __display_part_.__FindExpression($"{prefix}{op_str}{exp_x}");
                Expression expression_y = __display_part_.__FindExpression($"{prefix}{op_str}{exp_y}");
                Expression expression_z = __display_part_.__FindExpression($"{prefix}{op_str}{exp_z}");

                Component layout_comp = __display_part_.__AddComponent(
                    layout_path,
                    orientation: _Matrix3x3Identity,
                    referenceSet: "Entire Part",
                    layer: layer,
                    origin: new Point3d(
                        expression_x.Value,
                        expression_y.Value,
                        expression_z.Value
                    ), componentName: comp_name);

                // gets the first feature in the layout_comp.prototype and casts it to a DatumCsysFeature_
                DatumCsys proto_layout_csys = layout_comp.__Prototype()
                    .Features
                    .OfType<DatumCsys>()
                    .First();

                // gets the first feature in the layout_comp.prototype and casts it to a DatumCsysFeature_
                DatumCsys strip_csys = __display_part_.Features.OfType<DatumCsys>().First();

                DatumPlane datum_plane_xy1 = proto_layout_csys.__DatumPlaneXY();
                DatumPlane __plane_occ = (DatumPlane)layout_comp.__FindOccurrence(datum_plane_xy1);
                DatumPlane datum_plane_xy = strip_csys.__DatumPlaneXY();
                NewMethod24(comp_name, expression_z, __plane_occ, datum_plane_xy);

                NewMethod8(comp_name, expression_x, layout_comp, proto_layout_csys, strip_csys);

                // constrain xz
                NewMethod5(comp_name, expression_y, layout_comp, proto_layout_csys, strip_csys);

                replace_interpart_expressions(layout_comp, prefix);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod24(
            string comp_name,
            Expression expression_z,
            DatumPlane __plane_occ,
            DatumPlane datum_plane_xy)
        {
            ComponentConstraint comp_dist_const_xy = __display_part_.__ConstrainOccProtoDistance(
                __plane_occ,
                datum_plane_xy,
                expression_z.Name);
            comp_dist_const_xy.GetDisplayedConstraint().Layer = 256;
            comp_dist_const_xy.SetName($"{comp_name}-XY");
            string new_name = $"{comp_name.Replace("-", "_")}_{expression_z.Name}_";
            comp_dist_const_xy.SetName($"{comp_name}-XY");
            comp_dist_const_xy.SetName(new_name);
        }

        private static void NewMethod8(
            string comp_name,
            Expression expression_x,
            Component layout_comp,
            DatumCsys proto_layout_csys,
            DatumCsys strip_csys)
        {
            ComponentConstraint comp_dist_const_yz = __display_part_.__ConstrainOccProtoDistance(
                layout_comp.__AbsOccDatumPlaneYZ(),
                strip_csys.__DatumPlaneYZ(),
                expression_x.Name);

            comp_dist_const_yz.GetDisplayedConstraint().__Layer(256);
            comp_dist_const_yz.SetName($"{comp_name}-YZ");
            comp_dist_const_yz.Expression.SetName($"{comp_name.Replace("-", "_")}_{expression_x.Name}_");
        }

        private static void NewMethod5(
            string comp_name,
            Expression expression_y,
            Component layout_comp,
            DatumCsys proto_layout_csys,
            DatumCsys strip_csys)
        {
            DatumPlane layout_occ_plane = layout_comp.__AbsOccDatumPlaneXZ();
            ComponentConstraint constraint_xz = __display_part_.__ConstrainOccProtoDistance(
                layout_occ_plane,
                strip_csys.__DatumPlaneXZ(),
                expression_y.Name);
            constraint_xz.SetName($"{comp_name}-XZ");
            constraint_xz.GetDisplayedConstraint().__Layer(256);
            constraint_xz.Expression.SetName($"{comp_name.Replace("-", "_")}_{expression_y.Name}_");
        }

        public static void replace_interpart_expressions(Component layout_comp, string prefix)
        {
            List<string> inter_expression = new List<string>(new[] { "b", "e", "em", "m", "p" });

            if (prefix == "T")
                inter_expression.Add("TP");

            if (prefix == "P")
                inter_expression.Add("pp");

            Expression[] source = inter_expression.Select(__display_part_.__FindExpression)
                .Select(exp => exp)
                .ToArray();

            using (Session.GetSession().__usingDisplayPartReset())
            {
                __display_part_ = layout_comp.__Prototype();
                InterpartExpressionsBuilder builder = __display_part_.Expressions.CreateInterpartExpressionsBuilder();

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    builder.SetExpressions(source, inter_expression.ToArray());
                    builder.Commit();
                }

                UFSession.GetUFSession().Modl.Update();
            }
        }

        public static void add_press(string prefix)
        {
            Match __match = Regex.Match(__display_part_.Leaf, "^(?<cus_num>\\d{6})-.*$");

            if (!__match.Success)
            {
                print_("Could not find customer number");
                return;
            }

            string cust_num = __match.Groups["cus_num"].Value;
            string directory = Path.GetDirectoryName(__display_part_.FullPath);
            string job_folder = Path.GetDirectoryName(directory);
            string layout_folder = $"{job_folder}\\Layout";
            int op_int = 10;

            while (true)
            {
                string op_str = __Op10To010(op_int);
                string press_path = $"{layout_folder}\\{cust_num}-{prefix}{op_str}-Press.prt";

                try
                {
                    if (File.Exists(press_path))
                        continue;

                    File.Copy(XxxxxPressXxAssembly, press_path);

                    Component press_comp = __display_part_.ComponentAssembly.AddComponent(
                        press_path,
                        "Entire Part",
                        $"PRESS-{prefix}{op_str}",
                        _Point3dOrigin,
                        _Matrix3x3Identity,
                        245,
                        out _);

                    __display_part_.Layers.SetState(256, State.WorkLayer);
                    DatumCsys proto_press_csys = press_comp.__Prototype().__AbsoluteDatumCsys();
                    DatumCsys strip_csys0 = __display_part_.__AbsoluteDatumCsys();
                    NewMethod43(prefix, op_str, press_comp, proto_press_csys, strip_csys0);
                    press_comp.__ReferenceSet("BODY");
                    break;
                }
                finally
                {
                    op_int += 10;
                }
            }
        }

        public static void ExecuteCloneProg(GFolder __folder, string op)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.UserName);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-000.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-000.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "000"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-lwr.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-lwr.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-upr.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-upr.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lsh.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lsh.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsh"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-002.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-002.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "002"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-012.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-012.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "012"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrcast.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwrcast.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwrcast"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrplate.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwrplate.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwrplate"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-c-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-c-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-c-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-d-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-d-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-d-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-e-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-e-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-e-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-ush.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-ush.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "ush"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-502.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-502.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "502"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-512.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-512.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "512"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprcast.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-uprcast.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "uprcast"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprplate.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-uprplate.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "uprplate"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-c-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-c-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-c-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-d-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-d-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-d-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-e-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-e-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-e-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-dieset-control.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-dieset-control.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "dieset-control"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-casting-text.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-casting-text.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "casting-text"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-smart-rollover.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-smart-rollover.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-smart-rollover"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-smart-rollover.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-smart-rollover.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-smart-rollover"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-lsp1.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-lsp1.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsp1"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-lsp2.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-lsp2.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsp2"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-lsp3.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-lsp3.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsp3"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-lsp4.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-lsp4.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsp4"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-usp1.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-usp1.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "usp1"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-usp2.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-usp2.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "usp2"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-usp3.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-usp3.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "usp3"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-prog-usp4.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-prog-usp4.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "usp4"));

            clone.SetDefDirectory(__folder.dir_op(op));

            clone.InitNamingFailures(out UFClone.NamingFailures failures);

            clone.PerformClone(ref failures);

            clone.Terminate();

            session_.__FindOrOpen(__folder.path_op_detail("010", "casting-text"));
        }

        public static void ExecuteCloneLine900(GFolder __folder)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.NamingRule);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-900-000.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-900-lwr.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-900-upr.prt");

            clone.SetDefDirectory(__folder.dir_op("900"));

            UFClone.NameRuleDef rule = new UFClone.NameRuleDef
            {
                base_string = "seed-line-900-",
                new_string = $"{__folder.CustomerNumber}-900-",
                type = UFClone.NameRuleType.ReplaceString
            };

            clone.InitNamingFailures(out UFClone.NamingFailures failures);
            clone.SetNameRule(ref rule, ref failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        public static void ExecuteCloneLineOp(GFolder __folder, string op)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.NamingRule);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-000.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-002.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-012.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-502.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-512.prt");

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-casting-text.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-dieset-control.prt");

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lsh.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lsp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrcast.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-c-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-d-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-e-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrplate.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-smart-rollover.prt");

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-ush.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-usp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprcast.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-c-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-d-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-e-clamp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprplate.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-smart-rollover.prt");

            clone.SetDefDirectory(__folder.dir_op(op));

            UFClone.NameRuleDef rule = new UFClone.NameRuleDef
            {
                base_string = "seed-line-op-",
                new_string = $"{__folder.CustomerNumber}-{op}-",
                type = UFClone.NameRuleType.ReplaceString
            };

            clone.InitNamingFailures(out UFClone.NamingFailures failures);
            clone.SetNameRule(ref rule, ref failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        public static void ExecuteCloneTranAssemblyOp(string def_directory, IDictionary<string, string> clone_dict)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.UserName);

            foreach (string key in clone_dict.Keys)
            {
                clone.AddPart(key);
                clone.SetNaming(key, UFClone.NamingTechnique.UserName, clone_dict[key]);
            }

            clone.SetDefDirectory(def_directory);

            clone.InitNamingFailures(out UFClone.NamingFailures failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        public static void ExecuteCloneTranShoe(GFolder __folder, string op)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.UserName);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-tran-shoe-000.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-tran-shoe-000.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "000"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lsh.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lsh.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lsh"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-002.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-002.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "002"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-012.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-012.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "012"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrcast.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwrcast.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwrcast"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwrplate.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwrplate.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwrplate"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-c-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-c-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-c-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-d-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-d-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-d-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-e-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-e-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-e-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-lwr-smart-rollover.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-lwr-smart-rollover.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "lwr-smart-rollover"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-ush.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-ush.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "ush"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-502.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-502.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "502"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-512.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-512.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "512"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprcast.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-uprcast.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "uprcast"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-uprplate.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-uprplate.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "uprplate"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-c-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-c-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-c-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-d-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-d-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-d-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-e-clamp.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-e-clamp.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-e-clamp"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-upr-smart-rollover.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-upr-smart-rollover.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "upr-smart-rollover"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-dieset-control.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-dieset-control.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "dieset-control"));

            clone.AddPart($"{SEED_DIRECTORY}\\seed-line-op-casting-text.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-line-op-casting-text.prt", UFClone.NamingTechnique.UserName,
                __folder.path_op_detail(op, "casting-text"));

            clone.SetDefDirectory(__folder.dir_op(op));

            clone.InitNamingFailures(out UFClone.NamingFailures failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        public static void ExecuteCloneTranOp(GFolder __folder, string op)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.NamingRule);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-tran-op-000.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-tran-op-lsp.prt");
            clone.AddPart($"{SEED_DIRECTORY}\\seed-tran-op-usp.prt");

            clone.SetDefDirectory(__folder.dir_op(op));

            UFClone.NameRuleDef rule = new UFClone.NameRuleDef
            {
                base_string = "seed-tran-op-",
                new_string = $"{__folder.CustomerNumber}-{op}-",
                type = UFClone.NameRuleType.ReplaceString
            };

            clone.InitNamingFailures(out UFClone.NamingFailures failures);
            clone.SetNameRule(ref rule, ref failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        public static void ExecuteCloneTran900(GFolder __folder)
        {
            UFClone clone = UFSession.GetUFSession().Clone;
            clone.Terminate();
            clone.Initialise(UFClone.OperationClass.CloneOperation);
            clone.SetDefNaming(UFClone.NamingTechnique.UserName);

            clone.AddPart($"{SEED_DIRECTORY}\\seed-tran-op-000.prt");
            clone.SetNaming($"{SEED_DIRECTORY}\\seed-tran-op-000.prt", UFClone.NamingTechnique.UserName,
                __folder.path_detail1("900", "900", "000"));

            clone.SetDefDirectory(__folder.dir_op("900"));

            clone.InitNamingFailures(out UFClone.NamingFailures failures);

            clone.PerformClone(ref failures);

            clone.Terminate();
        }

        private static void ConstrainStripInterpartExpressions(
            string new_interpart_exp_x,
            string new_interpart_exp_y,
            string new_interpart_exp_z,
            InterpartExpressionsBuilder builder,
            Part strip)
        {
            using (new Destroyer(builder))
            {
                builder.SetExpressions(new[]
                    {
                        strip.Expressions.ToArray().Single(__e => __e.Name == new_interpart_exp_x),
                        strip.Expressions.ToArray().Single(__e => __e.Name == new_interpart_exp_y),
                        strip.Expressions.ToArray().Single(__e => __e.Name == new_interpart_exp_z)
                    },
                    new[]
                    {
                        new_interpart_exp_x,
                        new_interpart_exp_y,
                        new_interpart_exp_z
                    });

                builder.Commit();
            }
        }
    }
}
// 2643