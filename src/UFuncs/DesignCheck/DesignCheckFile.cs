namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public static class DesignCheckFile
    {
        public const string DESIGN_CHECK_VIEW_AND_REVISION = "Design Check: 4.0";

        public const string WireTapNotes_PreCheck_Titles = "WTN";

        public const string WireTapNotes_PreCheck_Values = "YES";

        public const string WireTapNotes_Check_Titles = "DETAIL NAME";

        public const string WireTapNotes_Check_Values = "TRIM";

        public const string Burnouts_Titles_1 = "MATERIAL";

        public const string Burnouts_DrawingSheetName = "BURNOUT";

        public const double Metric_Tolerance = 0.0254;

        public const double English_Tolerance = 0.001;

        public const string GFOLDER_JOB_FOLDER_REGEX = "([0-9]{1,}) ([a-z]{1,}-[0-9]{1,})";


        public const string CBoreDepth_EnglishShcsRegex =
            " ^ (?< Diameter > [0 - 9]{ 4,})-shcs - (?< Length > [0 - 9]{ 3,})$";


        // The difference between this and ""DOUBLE_DECIMAL_REGEX"" is that this won't match integers.
        // It requires that a decimal must be present in the number in order to match.
        public const string DOUBLE_DECIMAL_REQUIRE_PERIOD_REGEX = "(\\d+\\.\\d*|\\.\\d*)";

        public const string SIZE_DESCRIPTION_REGEX =
            "([0 - 9]*\\.[0-9]{2,3}).*\\s* X\\s* ([0 - 9]*\\.[0-9]{2,3}).*\\s* X\\s* ([0 - 9]*\\.[0-9]{2,3}).*";


        public const string SIZE_DESCRIPTION_BURN_REGEX =
            "^Burn\\s * (?< BurnHeight > (\\d +\\.\\d +)| (\\d +\\.)| (\\.\\d +))";


        // Regular Text Expression to match metric Shcs with.
        public const string CBoreDepth_MetricShcsRegex = "^(?<Diameter>[0-9]{1,})mm-shcs-(?<Length>[0-9]{3,})$";


        public const string BLOCK_REGEX_999 = @"(?< JobNumber >\d +) - (?< OpNumber >\d +)-99(?< Nine99Number >\d)";

        public const string LINKED_BODY_FEATURE_NAME = "LINKED_BODY";

        public const string OP_NUMBER = "(?!000)(?<OpNumber>[0-9]{3})";

        public const string Detail_Number_Regex = "(?<DetailNumber>[0-9]{3,})";

        public const string GFOLDER_ASSEMBLY_FOLDER_REGEX = "[0-9]{1,}-[0-9]{1,}";

        public const string Customer_Job_Op_Regex = "<rec>Customer_Job_Number_Regex</rec>-<rec>OP_NUMBER</rec>";

        public const string Detail_Structure_Regex =
            "\\\\<rec>Customer_Job_Op_Regex</rec>-<rec>Detail_Number_Regex</rec>\\\\";

        public const string COMPONENT_NAMES_REGEX = "^(?<DetailNumber>[0-9]{3})[A_Z]{1}$";

        public const string Customer_Job_Number_Regex = "(?<CustomerJobNumber>[0-9A-Z_]{4,})";

        public const string _0LIBRARY_REGEX = "^G:\\\\0Library\\\\.*\\.prt$";

        public const string W_DRIVE_REGEX = "^W:\\\\.*\\.prt$";

        public const string CBORE_DEPTH_MATCH_PATHS_REGEX = "[0-9]{4,}-[0-9]{3}-[0-9]{3}";

        public const string DETAIL_STRUCTURE_REGEX = "[0-9]{4,}-[0-9]{3}-[0-9]{3}";

        public const string INTERPART_EXPRESSIONS_GFOLDER_REGEX_MATCH =
            "\\\\([0-9]{1,}) \\([a-z]{1,}-[0-9]{1,}\\)\\\\.*\\.prt$";

        public static string[] Burnouts_Values_1 =
        {
            "HRS PLT",
            "4140 PLT",
            "4140 PH PLT"
        };

        public static string[] SIZE_DESCRIPTION_IGNORE_PATHS_REGEX =
        {
            @"^G:\\0Library\\.*\.prt$",
            @"^W:\\.*\.prt$"
        };

        public static readonly string[] CBORE_DEPTH_IGNORE_PATHS_REGEX =
        {
            "^G:\\\\0Library\\\\.*\\.prt$",
            "^W:\\\\.*\\.prt$"
        };

        private static string Temp => @"";
        //          public const string  = "";
        //          :GFOLDER_REGEXES:
        //          ^G:\\0Library\\.*\.prt$
        //          ^G:\\0Press\\.*\.prt$
        //          ^W:\\.*\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\[0-9]{1,}-[0-9]{3}\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\simulation\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\mathdata\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\grippers\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\layout\\.+\.prt$
        //          :END:
        //          public const string  = "";
        //          :FULLY_LOAD_ASSEMBLY_REGEXES:
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\[0-9]{1,}-[0-9]{3}\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\simulation\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\mathdata\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\grippers\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\layout\\.+\.prt$
        //          :END:
        //          public const string  = "";
        //          :PARTIALLY_LOAD_ASSEMBLY_REGEXES:
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\simulation\\.+\.prt$
        //          ^([a-zA-Z]):\\([0-9]{1,}) \([a-z]{1,}-[0-9]{1,}\)\\mathdata\\.+\.prt$
        //          :END:
        //          public const string  = "";
        //          :CBORE_DEPTH_TITLES_1:
        //          MATERIAL
        //          :END:
        //          public const string  = "";
        //          :SIZE_DESCRIPTION_TITLES_1:
        //          MATERIAL
        //          :END:
        //          public const string  = "";
        //          :CASTING_MATERIALS_TITLES:
        //          MATERIAL
        //          :END:
        //          public const string  = "";
        //          :CASTING_MATERIALS_VALUES:
        //          GM190
        //          GM238
        //          GM241
        //          GM245
        //          GM246
        //          GM338
        //          S0050A
        //          S0030A
        //          G2500
        //          G3500
        //          D4512
        //          D5506
        //          D6510
        //          CARMO CAST
        //          GM68M
        //          S0030
        //          :END:
        //          public const string  = "";
        //          :GuideBushings_GuidePins_Titles:
        //          LibraryPath
        //          :END:
        //          public const string  = "";
        //          :GuideBushings_GuidePins_Values:
        //          GuideBushings
        //          GuidePins
        //          :END:
        //          public const string  = "";
        //          // The following list is identical to the one sitting in the actual UCF file on the UDrive. 
        //          // However we removed ""METALS PLUS"" and ""01 from the list.
        //          :COMPONENT_MATERIALS:
        //          A2
        //          A2 GS
        //          ACUSEAL
        //          ALUM
        //          AMPCO 18
        //          CALDIE
        //          CARMO
        //          CRS
        //          D2
        //          DC53
        //          DIEVAR
        //          GM190/S0050A
        //          GM238/G2500
        //          GM241/G3500
        //          GM245/D4512
        //          GM246/D5506
        //          GM338/D6510
        //          GM68M/S0030
        //          S0030A
        //          HRS
        //          HRS PLT
        //          H13
        //          H13 PREMIUM
        //          O1
        //          O1 GS
        //          O1 DR
        //          Ovar Superior
        //          P20
        //          S7
        //          S7 GS
        //          STEELCRAFT
        //          STRUCTURAL
        //          4140
        //          4140 PH
        //          4140 PH PLT
        //          4140 PLT
        //          6150
        //          OTHER
        //          :END:
        //          public static string[]  CBORE_DEPTH_COMPONENT_MATERIALS=new []{
        //          A2,
        //          A2 GS,
        //          ACUSEAL
        //          ALUM
        //          AMPCO 18
        //          CALDIE
        //          CARMO
        //          CRS
        //          D2
        //          DC53
        //          DIEVAR
        //          GM190 / S0050A
        //          GM238 / G2500
        //          GM241 / G3500
        //          GM245 / D4512
        //          GM246 / D5506
        //          GM338 / D6510
        //          GM68M / S0030
        //          S0030A
        //          HRS
        //          HRS PLT
        //          H13
        //          H13 PREMIUM
        //          O1
        //          O1 DR
        //          Ovar Superior
        //          P20
        //          S7
        //          S7 GS
        //          STEELCRAFT
        //          STRUCTURAL
        //          4140
        //          4140 PH
        //          4140 PH PLT
        //          4140 PLT
        //          6150
        //          OTHER
        //          : END:
        //          public const string  = "";
        //          :DESCRIPTION_TITLES:
        //          DESCRIPTION
        //          :END:
        //          public const string  = "";
        //          :DESCRIPTION_IGNORE_VALUES:
        //          (\d+\.\d+)|(\d+\.)|(\.\d+)
        //          :END:
        //          public const string  = "";
        //          :SIZE_TITLES:
        //          DESCRIPTION
        //          :END:
        //          //(\d+)(?<=(\d+))
        //          //\s*see[-_ ]?3d[-_ ]?data\s*
        //          public const string DOUBLE_DECIMAL_REGEX = "(\\d+\\.?\\d*|\\.\\d*)";
        //                      ";
        //          //([0-9]\.[0-9]*)
        //          //public const string DOUBLE_DECIMAL_REQUIRE_PERIOD_REGEX = "";
        //          //::
        //          //(\d+\.\d*|\.\d*)
        //          //:END:
    }
}