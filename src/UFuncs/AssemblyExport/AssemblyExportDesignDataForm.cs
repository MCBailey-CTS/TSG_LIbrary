using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using static TSG_Library.UFuncs._UFunc;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_assembly_export_design_data)]
    public partial class AssemblyExportDesignDataForm : _UFuncForm
    {
        public AssemblyExportDesignDataForm()
        {
            InitializeComponent();
        }

        public static Part __display_part_ => Session.GetSession().Parts.Display;

        public static Part _WorkPart => Session.GetSession().Parts.Work;

        private void ResetForm()
        {
            if (__display_part_ is null)
            {
                Enabled = false;
                return;
            }

            Enabled = true;

            if (!rdoChange.Checked && !rdoRto.Checked && !rdoReview50.Checked && !rdoReview90.Checked &&
                !rdoReview100.Checked && !rdoOther.Checked)
                rdoReview50.Checked = true;

            GFolder folder = GFolder.create_or_null(_WorkPart);

            if (folder is null)
                return;

            if (folder.CustomerNumber.Length == 6)
            {
                if (rdoRto.Checked || rdoChange.Checked)
                {
                    txtFolderName.Text = "";
                    txtFolderName.Enabled = false;
                    return;
                }

                txtFolderName.Enabled = true;

                return;
            }

            txtFolderName.Enabled = true;
        }

        private void BtnData_Clicked(object sender, EventArgs e)
        {
            //var stopwatch = new System.Diagnostics.Stopwatch();

            //stopwatch.Start();


            Hide();
            try
            {
                if (sender == btnDesignAccept)
                    Export_Design();
                else if (sender == btnSelectComponents)
                    ManualExport(false);
                else if (sender == btnSelectAll)
                    ManualExport(true);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                print_("Complete");
                Show();
            }

            //stopwatch.Stop();

            //print_($"Time: {stopwatch.Elapsed.TotalMinutes:f2}");            
        }

        private void ManualExport(bool selectAll)
        {
            Part __display_part_ = Session.GetSession().Parts.Display;

            List<Component> components = selectAll
                ? __display_part_.ComponentAssembly.RootComponent.__Descendants().ToList()
                : Selection.SelectManyComponents().ToList();

            if (components.Count == 0)
                return;

            components.Add(__display_part_.ComponentAssembly.RootComponent);

            Export1.Design(
                __display_part_,
                components.ToArray(),
                null,
                false,
                false,
                chk4ViewPDF.Checked,
                chk4ViewDwg.Checked,
                false,
                chkDetailStep.Checked,
                chkSee3DData.Checked,
                chkBurnout.Checked,
                chkParasolids.Checked,
                chkCastings.Checked,
                chkPrintData.Checked,
                rdoChange.Checked);
        }

        private void Export_Design()
        {
            Component[] components;

            bool isRto = false;

            if (rdoRto.Checked)
            {
                isRto = true;
                components = __display_part_.ComponentAssembly.RootComponent.__Descendants().ToArray();
            }
            else if (rdoChange.Checked)
            {
                isRto = true;
                components = Selection.SelectManyComponents();
            }
            else if (rdoReview50.Checked || rdoReview90.Checked || rdoReview100.Checked || rdoOther.Checked)
            {
                isRto = false;
                components = __display_part_.ComponentAssembly.RootComponent.__Descendants().ToArray();
            }
            else
            {
                throw new ArgumentException();
            }

            if (components.Length == 0)
                return;

            Export1.Design(
                __display_part_,
                components,
                txtFolderName.Text,
                isRto || rdoChange.Checked,
                true,
                print4Views: isRto && chkPrintDesign.Checked,
                isChange: rdoChange.Checked);
        }

        private void Rdo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkPrintDesign.Enabled = rdoRto.Checked || rdoChange.Checked;

                btnDesignAccept.Text = rdoChange.Checked
                    ? "Select"
                    : "Execute";

                if (__display_part_ is null)
                    return;

                GFolder folder = GFolder.create_or_null(_WorkPart);

                if (folder is null)
                    return;

                switch (folder.CustomerNumber.Length)
                {
                    case 6:
                        txtFolderName.Enabled = !rdoChange.Checked && !rdoRto.Checked;
                        break;
                    default:
                        txtFolderName.Enabled = true;
                        break;
                }

                string folderName = $"{TodaysDate}-----{__display_part_.Leaf}-----";

                if (sender == rdoReview50)
                    txtFolderName.Text = folderName + @"50% Review";

                else if (sender == rdoReview90)
                    txtFolderName.Text = folderName + @"90% Review";

                else if (sender == rdoReview100)
                    txtFolderName.Text = folderName + @"100% Review";

                else if (sender == rdoRto)
                    txtFolderName.Text = folderName + @"RTO";

                else if (sender == rdoOther)
                    txtFolderName.Text = folderName + @"-----";

                else if (sender == rdoChange)
                    txtFolderName.Text = folderName + @"-----";
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl.SelectedTab == tabDesign)
                Size = new Size(275, 249);
            else if (tabControl.SelectedTab == tabData)
                Size = new Size(182, 385);
        }

        private void AssemblyExportDesignDataForm_Load(object sender, EventArgs e)
        {
            ResetForm();
            Location = Settings.Default.assembly_export_design_data_form_window_location;
        }

        private void AssemblyExportDesignDataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.assembly_export_design_data_form_window_location = Location;
            Settings.Default.Save();
        }
    }
}