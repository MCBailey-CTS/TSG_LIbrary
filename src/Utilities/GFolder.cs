using System;
using System.IO;
using System.Text.RegularExpressions;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.Utilities
{
    public class GFolder
    {
        private const string __0library_seedfiles_nx1919 = "G:\\0Library\\SeedFiles\\NX1919";
        public const string XxxxxPressXxAssembly = __0library_seedfiles_nx1919 + "\\XXXXX-Press-XX-Assembly.prt";
        // ReSharper disable once UnusedMember.Global
        public const string XxxxxXxx000 = __0library_seedfiles_nx1919 + "\\XXXXXX-XXX-000.prt";
        public const string XxxxxStrip = __0library_seedfiles_nx1919 + "\\XXXXXX-Strip.prt";
        // ReSharper disable once UnusedMember.Global
        public const string X000Simulation = "G:\\0Library\\SeedFiles\\NX1919\\0000-simulation.prt";
        public const string XxxxxxOpXxxLayout = __0library_seedfiles_nx1919 + "\\XXXXXX-OP-XXX-Layout.prt";

        public const string StripFlangeCarrierTracking =
            __0library_seedfiles_nx1919 + "\\Strip Flange Carrier_Tracking Tab.prt";

        // ReSharper disable once MemberCanBePrivate.Global
        public GFolder(string dir, string customerNumber)
        {
            DirJob = dir;
            CustomerNumber = customerNumber;
        }

        public string DirProcessSimDataDesign => $"{DirJob}\\Process and Sim Data for Design";

        public string CustomerNumber { get; }

        public string DirLayout => $"{DirJob}\\Layout";

        public string DirJob { get; }

        public string DirSimulation => $"{DirJob}\\Simulation";

        // ReSharper disable once MemberCanBePrivate.Global
        public string LeafPathSim => $"{CustomerNumber}-simulation";

        public string PathSimulation => $"{DirSimulation}\\{LeafPathSim}.prt";

        public string FileStrip900 => file_strip("900");

        public string FileStrip010 => file_strip("010");

        public string DirOutgoing => $"{DirJob}\\Outgoing";

        public string PathStripFlange => $"{DirLayout}\\{CustomerNumber}-Strip Flange Carrier_Tracking Tab.prt";

        public string DirMathData => $"{DirJob}\\Math Data";

        public string DirStocklist => $"{DirDesignInformation}\\NX Stocklist";

        // ReSharper disable once UnusedMember.Global
        public string Company
        {
            get
            {
                string dirLeaf = Path.GetFileNameWithoutExtension(DirJob);

                Match match = Regex.Match(dirLeaf, "^\\d+ \\((?<company>[a-zA-Z]+)-\\d+\\)$");

                if (!match.Success)
                    throw new FormatException($"Could not find a company: {DirJob}");

                return match.Groups["company"].Value;
            }
        }

        public string CtsNumber
        {
            get
            {
                string dirLeaf = Path.GetFileNameWithoutExtension(DirJob);

                Match match = Regex.Match(dirLeaf, "^\\d+ \\([a-zA-Z]+-(?<cts>\\d+)\\)$");

                if (!match.Success)
                    throw new FormatException($"Could not find a cts number: {DirJob}");

                return match.Groups["cts"].Value;
            }
        }

        public string DirDesignInformation => $"{DirJob}\\Design Information";

        public static GFolder Create(string jobFolder)
        {
            string[] split = jobFolder.Split('\\');

            string topDir;

            try
            {
                topDir = $"{split[0]}\\{split[1]}\\{split[2]}";
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException(jobFolder);
            }

            string leaf = split[2];

            string dir = topDir;

            if (!dir.ToLower().Contains("\\cts\\"))
            {
                Match match0 = Regex.Match(leaf, @"^(?<num_0>\d{2,})");

                return new GFolder(dir, match0.Groups["num_0"].Value);
            }

            if (int.TryParse(leaf, out _))
                return new GFolder(dir, leaf);

            {
                Match match = Regex.Match(leaf, @"^(?<cts_number>\d{2,}) \([A-Z]+-[xX]+\)");

                if (match.Success)
                    return new GFolder(dir, match.Groups["cts_number"].Value);
            }

            {
                Match match = Regex.Match(leaf, @"^(?<cts_number>\d{2,}) \([A-Za-z]+-(?<customer_number>\d+)\)");

                if (match.Success)
                    return new GFolder(dir, match.Groups["customer_number"].Value);
            }

            {
                Match match = Regex.Match(leaf, @"^(?<customer_number>\d{2,})");

                if (match.Success)
                    return new GFolder(dir, match.Groups["customer_number"].Value);
            }

            throw new ArgumentException($"Could not make GFolder from '{jobFolder}'");
        }

        public static GFolder create_or_null(Part workPart)
        {
            try
            {
                return Create(workPart.FullPath);
            }
            catch (Exception ex)
            {
                ex.__PrintException($"Exception caught for GFolder '{workPart.FullPath}'");
                return null;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string path_op_op_detail(string dirOp, string detailOp, string detail)
        {
            return dir_op(dirOp) + $"\\{CustomerNumber}-{detailOp}-{detail}.prt";
        }

        public string path_op_detail(string detailOp, string detail)
        {
            return path_op_op_detail(detailOp, detailOp, detail);
        }

        public bool is_cts_job()
        {
            return DirJob.ToLower().Contains("\\cts\\");
        }

        public override string ToString()
        {
            return $"{DirJob} -> {CustomerNumber}";
        }

        public string file_detail0(string op, string detail)
        {
            return $"{dir_op(op)}\\{CustomerNumber}-{op}-{detail}.prt";
        }

        public string file_detail_000(string op)
        {
            return file_detail0(op, "000");
        }

        public string file_layout_t(string op)
        {
            return file_layout("T", op);
        }

        public string file_layout_p(string op)
        {
            return file_layout("P", op);
        }

        public string file_layout(string prefix, string op)
        {
            return $"{DirLayout}\\{leaf_path_layout(prefix, op)}.prt";
        }

        public string file_strip(string op)
        {
            return $"{DirLayout}\\{CustomerNumber}-{op}-Strip.prt";
        }

        public string dir_op(string op)
        {
            return $"{DirJob}\\{CustomerNumber}-{op}";
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string leaf_path_layout(string prefix, string op)
        {
            return $"{CustomerNumber}-{prefix}{op}-Layout";
        }

        public string path_detail1(string dirOp, string op, string detail)
        {
            return $"{dir_op(dirOp)}\\{CustomerNumber}-{op}-{detail}.prt";
        }

        public string file_checker_stock_list()
        {
            return $"{DirStocklist}\\{CustomerNumber.ToUpper()}-checker-stocklist.xlsx";
        }

        public string dir_surfaces()
        {
            return $"{DirSimulation}\\surfaces";
        }

        // ReSharper disable once UnusedMember.Global
        public string file_dieset_control(string op)
        {
            return path_op_detail(op, "dieset-control");
        }

        public string file_press_t(string op)
        {
            return $"{DirLayout}\\{CustomerNumber}-T{op}-Press.prt";
        }

        public string file_dir_op_op_detail(string dirOp, string detailOp, string detail)
        {
            return path_detail1(dirOp, detailOp, detail);
        }
    }
}
// 266