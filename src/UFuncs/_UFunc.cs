using NXOpen.UF;
using System;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    public abstract class _UFunc : _IUFunc
    {
        public const string conn_str =
            "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";

        public const string ufunc_design_check = "design-check";
        public const string ufunc_add_fasteners = "add-fasteners";
        public const string ufunc_simulation_data_builder = "simulation-data-builder";
        public const string ufunc_sim_ref_sets = "sim-ref-sets";
        public const string ufunc_strip_ref_setter = "strip-ref-setter";
        public const string ufunc_add_pierce_components = "add-pierce-components";
        public const string ufunc_delete_unused_curves = "delete-unused-curves";
        public const string ufunc_drain_hole_creator = "drain-hole-creator";
        public const string ufunc_export_sim_package = "export-sim-package";
        public const string ufunc_assembly_auto_detail = "assembly-auto-detail";
        public const string ufunc_change_link_subtract_ref_set = "change-link-subtract-ref-set";
        public const string ufunc_load_color_palette = "load-color-palette";
        public const string ufunc_assembly_color_code_atd = "assembly-color-code-atd";
        public const string ufunc_detail_number_note = "detail-number-note";
        public const string ufunc_copy_paste_clean_up = "copy-paste-clean-up";
        public const string ufunc_copy_ref_sets = "copy-ref-sets";
        public const string ufunc_cycle_components = "cycle-components";
        public const string ufunc_delete_layer_50 = "delete-layer-50";
        public const string ufunc_assembly_color_code = "assembly-color-code";
        public const string ufunc_category_ref_sets = "category-ref-sets";
        public const string ufunc_create_burnout = "create-burnout";
        public const string ufunc_extract_free_edge_curves = "extract-free-edge-curves";
        public const string ufunc_f_shape_exporter = "f-shape-exporter";
        public const string ufunc_component_builder = "component-builder";
        public const string ufunc_copy_to_layer_50 = "copy-to-layer-50";
        public const string ufunc_layout_ref_sets = "layout-ref-sets";
        public const string ufunc_etching = "etching";
        public const string ufunc_clone_strip = "clone-strip";
        public const string ufunc_circle_from_spline = "circle-from-spline";
        public const string ufunc_assembly_color_code_bentler = "assembly-color-code-bentler";
        public const string ufunc_clone_assembly = "clone-assembly";
        public const string ufunc_assembly_export_design_data = "assembly-export-design-data";
        public const string ufunc_proposal_data_wizard = "proposal-data-wizard";
        public const string ufunc_auto_size_component = "auto-size-component";
        public const string ufunc_block_attributer = "block-attributer";
        public const string ufunc_wire_start_hole = "wire-start-hole";
        public const string ufunc_profile_trim_and_form = "profile-trim-and-form";
        public const string ufunc_wire_taper_development = "wire-taper-development";
        public const string ufunc_bill_of_material = "bill-of-material";
        public const string ufunc_spline_to_curves = "spline-to-curves";
        public const string ufunc_export_strip = "export-strip";
        public const string ufunc_show_only = "show-only";
        public const string ufunc_assembly_wavelink = "assembly-wavelink";
        public const string ufunc_clean_job_directory = "clean-job-directory";

        protected _UFunc()
        {
            try
            {
                ufunc_name = GetType().Name;

                string syslogmessage = $"////////////////////////////////////////////////////////\n" +
                    $"{ufunc_name}\n" +
                    $"{Extensions.Extensions.AssemblyFileVersion}\n" +
                    $"{typeof(_UFunc).Assembly.Location}\n" +
                    $"////////////////////////////////////////////////////////";

                UFSession.GetUFSession().UF.PrintSyslog(syslogmessage, false);
                
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public int revision => throw new NotImplementedException();

        public string ufunc_name { get; set; }

        public string ufunc_rev_name => $"{Extensions.Extensions.AssemblyFileVersion} - {ufunc_name}";

        public abstract void execute();
        //public const string ufunc_ = "";
        //public const string ufunc_ = "";
        //public const string ufunc_ = "";
        //public const string ufunc_ = "";
    }
}