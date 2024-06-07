using System;
using System.IO;
using System.Text.RegularExpressions;
using NXOpen;

namespace TSG_Library.Utilities
{
    public class GFolder
    {
        private const string __0library_seedfiles_nx1919 = "G:\\0Library\\SeedFiles\\NX1919";
        public const string XXXXX_Press_XX_Assembly = __0library_seedfiles_nx1919 + "\\XXXXX-Press-XX-Assembly.prt";
        public const string XXXXX_XXX_000 = __0library_seedfiles_nx1919 + "\\XXXXXX-XXX-000.prt";
        public const string XXXXX_Strip = __0library_seedfiles_nx1919 + "\\XXXXXX-Strip.prt";
        public const string X000_simulation = "G:\\0Library\\SeedFiles\\NX1919\\0000-simulation.prt";
        public const string XXXXXX_OP_XXX_Layout = __0library_seedfiles_nx1919 + "\\XXXXXX-OP-XXX-Layout.prt";

        public const string STRIP_FLANGE_CARRIER_TRACKING =
            __0library_seedfiles_nx1919 + "\\Strip Flange Carrier_Tracking Tab.prt";

        public GFolder(string __dir, string __customer_number)
        {
            dir_job = __dir;
            customer_number = __customer_number;
        }

        public string dir_process_sim_data_design => $"{dir_job}\\Process and Sim Data for Design";

        public string customer_number { get; }

        public string dir_layout => $"{dir_job}\\Layout";

        public string dir_job { get; }

        public string dir_simulation => $"{dir_job}\\Simulation";

        public string leaf_path_sim => $"{customer_number}-simulation";

        public string path_simulation => $"{dir_simulation}\\{leaf_path_sim}.prt";

        public string file_strip_900 => file_strip("900");

        public string file_strip_010 => file_strip("010");

        public string dir_outgoing => $"{dir_job}\\Outgoing";

        public string path_strip_flange => $"{dir_layout}\\{customer_number}-Strip Flange Carrier_Tracking Tab.prt";

        public string dir_math_data => $"{dir_job}\\Math Data";

        public string dir_stocklist => $"{dir_design_information}\\NX Stocklist";

        public string company
        {
            get
            {
                var dir_leaf = Path.GetFileNameWithoutExtension(dir_job);

                var match = Regex.Match(dir_leaf, "^\\d+ \\((?<company>[a-zA-Z]+)-\\d+\\)$");

                if(!match.Success)
                    throw new FormatException($"Could not find a company: {dir_job}");

                return match.Groups["company"].Value;
            }
        }

        public string cts_number
        {
            get
            {
                var dir_leaf = Path.GetFileNameWithoutExtension(dir_job);

                var match = Regex.Match(dir_leaf, "^\\d+ \\([a-zA-Z]+-(?<cts>\\d+)\\)$");

                if(!match.Success)
                    throw new FormatException($"Could not find a cts number: {dir_job}");

                return match.Groups["cts"].Value;
            }
        }

        public string dir_design_information => $"{dir_job}\\Design Information";

        public static GFolder create(string __job_folder)
        {
            var split = __job_folder.Split('\\');

            string top_dir;

            try
            {
                top_dir = $"{split[0]}\\{split[1]}\\{split[2]}";
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException(__job_folder);
            }

            var leaf = split[2];

            var dir = top_dir;

            if(!dir.ToLower().Contains("\\cts\\"))
            {
                var match0 = Regex.Match(leaf, @"^(?<num_0>\d{2,})");

                return new GFolder(dir, match0.Groups["num_0"].Value);
            }

            if(int.TryParse(leaf, out _))
                return new GFolder(dir, leaf);

            {
                var match = Regex.Match(leaf, @"^(?<cts_number>\d{2,}) \([A-Z]+-[xX]+\)");

                if(match.Success)
                    return new GFolder(dir, match.Groups["cts_number"].Value);
            }

            {
                var match = Regex.Match(leaf, @"^(?<cts_number>\d{2,}) \([A-Za-z]+-(?<customer_number>\d+)\)");

                if(match.Success)
                    return new GFolder(dir, match.Groups["customer_number"].Value);
            }

            {
                var match = Regex.Match(leaf, @"^(?<customer_number>\d{2,})");

                if(match.Success)
                    return new GFolder(dir, match.Groups["customer_number"].Value);
            }

            throw new ArgumentException($"Could not make GFolder from '{__job_folder}'");
        }

        public static GFolder create_or_null(Part workPart)
        {
            try
            {
                return create(workPart.FullPath);
            }
            catch (Exception ex)
            {
                ex._PrintException($"Exception caught for GFolder '{workPart.FullPath}'");
                return null;
            }
        }

        public string path_op_op_detail(string dir_op, string detail_op, string detail)
        {
            return this.dir_op(dir_op) + $"\\{customer_number}-{detail_op}-{detail}.prt";
        }

        public string path_op_detail(string detail_op, string detail)
        {
            return path_op_op_detail(detail_op, detail_op, detail);
        }

        public bool is_cts_job()
        {
            return dir_job.ToLower().Contains("\\cts\\");
        }

        public override string ToString()
        {
            return $"{dir_job} -> {customer_number}";
        }

        public string file_detail0(string __op, string __detail)
        {
            return $"{dir_op(__op)}\\{customer_number}-{__op}-{__detail}.prt";
        }

        public string file_detail_000(string op)
        {
            return file_detail0(op, "000");
        }

        public string file_layout_t(string __op)
        {
            return file_layout("T", __op);
        }

        public string file_layout_p(string __op)
        {
            return file_layout("P", __op);
        }

        public string file_layout(string __prefix, string __op)
        {
            return $"{dir_layout}\\{leaf_path_layout(__prefix, __op)}.prt";
        }

        public string file_strip(string __op)
        {
            return $"{dir_layout}\\{customer_number}-{__op}-Strip.prt";
        }

        public string dir_op(string __op)
        {
            return $"{dir_job}\\{customer_number}-{__op}";
        }

        public string leaf_path_layout(string __prefix, string __op)
        {
            return $"{customer_number}-{__prefix}{__op}-Layout";
        }

        public string path_detail1(string __dir_op, string __op, string __detail)
        {
            return $"{dir_op(__dir_op)}\\{customer_number}-{__op}-{__detail}.prt";
        }

        public string file_checker_stock_list()
        {
            return $"{dir_stocklist}\\{customer_number.ToUpper()}-checker-stocklist.xlsx";
        }

        public string dir_surfaces()
        {
            return $"{dir_simulation}\\surfaces";
        }

        public string file_dieset_control(string op)
        {
            return path_op_detail(op, "dieset-control");
        }

        public string file_press_t(string op)
        {
            return $"{dir_layout}\\{customer_number}-T{op}-Press.prt";
        }

        public string file_dir_op_op_detail(string dir_op, string detail_op, string detail)
        {
            return path_detail1(dir_op, detail_op, detail);
        }
    }
}
// 266