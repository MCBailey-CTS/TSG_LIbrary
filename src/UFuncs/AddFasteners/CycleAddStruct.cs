//
//using static CTS_Library.Globals;
//using static CTS_Library.Constants;
//using CTS_Library.Utilities;
//using CTS_Library.Forms;

using System.Collections.Generic;

namespace TSG_Library.UFuncs
{
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
}