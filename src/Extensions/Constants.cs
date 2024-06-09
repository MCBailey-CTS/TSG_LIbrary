namespace TSG_Library.Extensions
{
    public partial class __Extensions_
    {
        #region Constants

        /// <summary>
        ///     The path to the Concept Control File.
        /// </summary>
        public const string ControlFile = @"U:\NX110\Concept\ConceptControlFile.UCF";

        //public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        //public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        public const string View_Plan = "PLAN";


        /// <summary>The layer to put tooling holes on.</summary>
        /// <remarks>Layer = 97</remarks>
        public const int Layer_DwlTooling = 97;

        /// <summary>The layer to place regular fasteners on.</summary>
        /// <remarks>Layer = 99</remarks>
        public const int Layer_Fastener = 99;

        /// <summary>The layer to put handling holes and wire taps on.</summary>
        /// <remarks>Layer = 98</remarks>
        public const int Layer_ShcsHandlingWireTap = 98;


        /// <summary>The regular expression that matches a n assembly folderWithCtsNumber.</summary>
        /// <remarks>Expressions = "^(?<customerNum>\d+)-(?<opNum>\d+)$"</opNum></customerNum></remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string Regex_AssemblyFolder = @"^(?<customerNum>\d+)-(?<opNum>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Bhcs = @"^(?<diameter>\d+)(?:mm)?-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_BhcsEnglish = @"^(?<diameter>\d+)-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_BhcsMetric = @"^(?<diameter>\d+)mm-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression that matches a blank.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-blank$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Blank = @"^(?<customerNum>\d+)-(?<opNum>\d+)-blank$";


        /// <summary>The regular expression pattern that matches a metric castle nut.</summary>
        /// <remarks>Expressions = "^Hexagon Castle Nut M(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_CastleNutMetric = @"^Hexagon Castle Nut M(?<diameter>\d+)$";


        /// <summary>The regular expression that matches a detail.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-(?&lt;detailNum&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Detail = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        /// <summary>The regular expression that matches a dieset control.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-dieset-control$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_DiesetControl = @"^(?<customerNum>\d+)-(?<opNum>\d+)-dieset-control$";


        /// <summary>The regular expression that matches a double.</summary>
        /// <remarks>Expressions = "(?&lt;double&gt;\\d+\\.\\d+|\\d+\\.|\\.\\d+|\\d+)"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>double</description>
        ///     </item>
        /// </list>
        public const string Regex_DoubleDecimal = @"(?<double>\d+\.\\d+|\\d+\\.|\\.\\d+|\\d+)";


        /// <summary>The regular expression pattern that matches a metric or an english dowel (dwl).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Dwl = @"^(?<diameter>\d+)(?:mm)?-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english dowel (dwl).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlEnglish = @"^(?<diameter>\d+)-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric dowel lock (dwl-lck).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-dwl-lck-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlLckMetric = @"^(?<diameter>\d+)mm-dwl-lck-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric misumi dowel lock (SWA).</summary>
        /// <remarks>Expressions = "^SWA(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlLckMisumiMetric = @"^SWA(?<diameter>\d+)$";


        /// <summary>
        ///     The regular expression pattern that matches a metric dowel (dwl).
        ///     <para />
        /// </summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlMetric = @"^(?<diameter>\d+)mm-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Fhcs = @"^(?<diameter>\d+)(?:mm)?-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_FhcsEnglish = @"^(?<diameter>\d+)-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_FhcsMetric = @"^(?<diameter>\d+)mm-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric or an english jack screw (jck-screw).</summary>
        /// <remarks>"^_?(?&lt;diameter&gt;\d+)(?:mm)?-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrew = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw$";


        /// <summary>The regular expression pattern that matches an english jack screw (jck-screw).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewEnglish = @"^_(?<diameter>\d+)-jck-screw$";


        /// <summary>The regular expression pattern that matches a metric jack screw (jck-screw).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewMetric = @"^(?<diameter>\d+)mm-jck-screw$";


        /// <summary>The regular expression pattern that matches a metric or an english tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^_?(?&lt;diameter&gt;\d+)(?:mm)?-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsg = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw-tsg$";


        /// <summary>The regular expression pattern that matches an english tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsgEnglish = @"^_(?<diameter>\d+)-jck-screw-tsg$";


        /// <summary>The regular expression pattern that matches a metric tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsgMetric = @"^(?<diameter>\d+)mm-jck-screw-tsg$";


        /// <summary>The regular expression that matches a layout.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-layout$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Layout = @"^(?<customerNum>\d+)-(?<opNum>\d+)-layout$";


        /// <summary>The regular expression pattern that matches either a metric or an english low head cap screw (lhcs).</summary>
        /// <remarks>"^(?&lt;diameter&gt;\d+)(?:mm)?-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Lhcs = @"^(?<diameter>\d+)(?:mm)?-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english low head cap screw (lhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_LhcsEnglish = @"^(?<diameter>\d+)-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric low head cap screw (lhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_LhcsMetric = @"^(?<diameter>\d+)mm-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric lock washer.</summary>
        /// <remarks>Expressions = "^lock washer for (?&lt;diameter&gt;\d+)mm shcs$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_LockWasherMetric = @"^lock washer for (?<diameter>\d+)mm shcs$";


        /// <summary>The regular expression that matches a lower shoe (lsh).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-lsh$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Lsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsh$";


        /// <summary>The regular expression that matches either a lower shoe (lsh) or an upper shoe (ush).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>shoe</description>
        ///     </item>
        /// </list>
        public const string Regex_LshUsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<shoe>lsh|ush)$";


        /// <summary>The regular expression that matches a lower sub plate (lsp).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Lsp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsp(?<extraOpNum>\d+)?.*$";


        /// <summary>The regular expression that matches either a lower sub plate (lsp) or an upper sub plate (usp).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>subPlate</description>
        ///     </item>
        ///     <item>
        ///         <description>extraOpNum</description>
        ///     </item>
        /// </list>
        public const string Regex_LspUsp =
            @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<subPlate>lsp|usp)(?<extraOpNum>\d+)?$";


        /// <summary>The regular expression that matches a lower assembly holder (lwr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-lwr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Lwr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lwr$";


        /// <summary>The regular expression that matches either a lower (lwr) or upper (upr) assembly holder (upr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-upr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_LwrUpr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-[lwr|upr]$";


        /// <summary>The regular expression that matches a mathdata part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-mathdata$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Mathdata = @"^(?<customerNum>\d+)-mathdata$";


        /// <summary>The regular expression that matches a master part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-Master$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Master = @"^(?<customerNum>\d+)-Master$";


        /// <summary>The regular expression that matches a history part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-History$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_History = @"^(?<customerNum>\d+)-History$";


        /// <summary>The regular expression pattern that matches a metric or an english nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-M?(?&lt;threadCount&gt;\d+(?:\.\d+)?)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_Nut = @"^nut-(?<diameter>\d+)-M?(?<threadCount>\d+(?:\.\d+)?)$";


        /// <summary>The regular expression pattern that matches an english nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-(?&lt;threadCount&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_NutEnglish = @"^nut-(?<diameter>\d+)-(?<threadCount>\d+)$";


        /// <summary>The regular expression pattern that matches a metric nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-M(?&lt;threadCount&gt;\d+(?:\.\d+)?)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_NutMetric = @"^nut-(?<diameter>\d+)-M(?<threadCount>\d+(?:\.\d+)?)$";


        /// <summary>The regular expression that matches a top level "000" part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-000$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Op000Holder = @"^(?<customerNum>\d+)-(?<opNum>\d+)-000$";


        /// <summary>The regular expression that matches a press assembly.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-Press-(?&lt;opNum&gt;\d+)-Assembly$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_PressAssembly = @"^(?<customerNum>\d+)-Press-(?<opNum>\d+)-Assembly$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket head cap screw (shcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Shcs = @"^(?<diameter>\d+)(?:mm)?-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket head cap screw (shcs).</summary>
        /// <remarks>Expression - "^(?&lt;diameter&gt;\d+)-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShcsEnglish = @"^(?<diameter>\d+)-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket head cap screw (shcs).</summary>
        /// <remarks>Expression = "^(?&lt;diameter&gt;\d+)mm-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShcsMetric = @"^(?<diameter>\d+)mm-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Shss = @"^(?<diameter>\d+)(?:mm)?-shss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShssEnglish = @"^(?<diameter>\d+)-shss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShssMetric = @"^(?<diameter>\d+)mm-shss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a simulation part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-simulation$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        internal const string Regex_Simulation = @"^(?<customerNum>\d+)-simulation$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket set screw (sss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Sss = @"^(?<diameter>\d+)(?:mm)?-sss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket set screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_SssEnglish = @"^(?<diameter>\d+)-sss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket set screw (sss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_SssMetric = @"^(?<diameter>\d+)mm-sss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a strip.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-strip$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string Regex_Strip = @"^(?<customerNum>\d+)-(?<opNum>\d+)-strip$";


        /// <summary>The regular expression that matches a strip control.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-strip-control$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_StripControl = @"^(?<customerNum>\d+)-strip-control$";


        /// <summary>The regular expression that matches an upper assembly holder (upr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-upr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Upr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-upr$";


        /// <summary>The regular expression that matches an upper shoe (ush).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-ush$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Ush = @"^(?<customerNum>\d+)-(?<opNum>\d+)-ush$";


        /// <summary>The regular expression that matches an upper sub plate (usp).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-usp(?&lt;extraOpNum&gt;\d+)?$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Usp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-usp(?<extraOpNum>\d+)?.*$";


        /// <summary>The regular expression that matches a 999 block/detail.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-(?&lt;detailNum&gt;99\d)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_999 = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>99\d)$";

        public const string FilePath_WireTap =
            FilePath_0LibraryFasteners + "\\Metric\\SocketHeadCapScrews\\008\\8mm-shcs-020.prt";

        public const string FilePath_0Library = "G:\\0Library";

        public const string FilePath_0Press = "G:\\0Press";

        public const string FilePath_0LibraryFasteners = FilePath_0Library + "\\Fasteners";

        public const string FilePath_DxfDwgDef = "C:\\Program Files\\Siemens\\NX 11.0\\dxfdwg\\dxfdwg.def";

        public const string FilePath_Ucf = @"U:\nxFiles\UfuncFiles\ConceptControlFile.ucf";

        public const string FilePath_Step214Ug = "C:\\Program Files\\Siemens\\NX 11.0\\NXBIN\\step214ug.exe";

        public const string FilePath_ExternalStep_AllLayers_def =
            "U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def";

        public const string FilePath_ExternalStep_Design_def = "U:\\nxFiles\\Step Translator\\ExternalStep_Design.def";

        public const string FilePath_ExternalStep_Assembly_def =
            @"U:\nxFiles\Step Translator\ExternalStep_Assembly.def";

        public const string FilePath_ExternalStep_Detail_def = @"U:\nxFiles\Step Translator\ExternalStep_Detail.def";

        /// <summary>Returns the path to the folderWithCtsNumber 0Components.</summary>
        public const string Folder0Components = "G:\\0Library";

        /// <summary>Returns the path to the folderWithCtsNumber seedFiles.</summary>
        public const string FolderSeedFiles = Folder0Components + "\\seedFiles";

        /// <summary>Returns the path to the seed layout folderWithCtsNumber.</summary>
        public const string FolderSeedLayout = FolderSeedFiles + "\\layout";

        /// <summary>Returns the path to the XXXX-010-blank part file.</summary>
        public const string SeedBlank = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-blank.prt";

        /// <summary>Returns the path to the XXXX-010-strip part file.</summary>
        public const string SeedStrip010 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-strip.prt";

        /// <summary>Returns the path to the XXXX-020-layout part file.</summary>
        public const string SeedLayout020 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-020-layout.prt";

        /// <summary>Returns the path to the XXXX-010-layout part file.</summary>
        public const string SeedLayout010 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-layout.prt";

        /// <summary>Returns the path to the XXXX-900-strip part file.</summary>
        public const string SeedStrip900 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-900-strip.prt";

        /// <summary>Returns the path to the XXXX-strip-control part file.</summary>
        public const string SeedStripControl = "G:\\0Library\\SeedFiles\\Layout\\XXXX-strip-control.prt";

        /// <summary>Returns the path to the seed-press part file.</summary>
        public const string SeedPress = "G:\\0Library\\SeedFiles\\Press\\seed-press.prt";

        /// <summary>Returns the path to the XXXX-Press-01-Assembly part file.</summary>
        public const string SeedPress01 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-01-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-02-Assembly part file.</summary>
        public const string SeedPress02 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-02-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-03-Assembly part file.</summary>
        public const string SeedPress03 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-03-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-04-Assembly part file.</summary>
        public const string SeedPress04 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-04-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-05-Assembly part file.</summary>
        public const string SeedPress05 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-05-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-06-Assembly part file.</summary>
        public const string SeedPress06 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-06-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-07-Assembly part file.</summary>
        public const string SeedPress07 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-07-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-08-Assembly part file.</summary>
        public const string SeedPress08 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-08-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-09-Assembly part file.</summary>
        public const string SeedPress09 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-09-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-10-Assembly part file.</summary>
        public const string SeedPress10 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-10-Assembly.prt";

        /// <summary>Returns the path to the Dummy part file.</summary>
        public const string DummyPath = "G:\\0Library\\SeedFiles\\Components\\Dummy.prt";

        /// <summary>Returns the path to the seed-prog-base part file.</summary>
        public const string SeedProgBase = "G:\\0Library\\SeedFiles\\Components\\seed-base.prt";

        /// <summary>Returns the path to the seed 0000-simulation part file.</summary>
        //public const string SeedSimulation = "G:\\0start\\simulation\\0000-simulation.prt";
        public const string SeedSimulation = "G:\\0Template\\0start\\Simulation\\0000-simulation.prt";


        /// <summary>Returns the path to the seed xxxx-History part file.</summary>
        public const string SeedHistory = "G:\\0start\\Math Data\\History\\xxxx-History.prt";

        /// <summary>Returns the path to the seed xxxx-Master part file.</summary>
        public const string SeedMaster = "G:\\0start\\Math Data\\Go\\xxxx-Master.prt";

        /// <summary>Returns the path to the run_managed executable file.</summary>
        public const string RunManaged = "C:\\Program Files\\Siemens\\NX 11.0\\NXBIN\\run_managed.exe";

        /// <summary>Returns the path to the seed dieset control file.</summary>
        public const string SeedDiesetControl = "G:\\0Library\\SeedFiles\\Assembly\\seed-dieset-control.prt";


        public const int Color_Trim = 186;

        public const int Color_Finish = 42;


        public const string Refset_EntirePart = "Entire Part";

        public const string Refset_Empty = "Empty";

        public const string Refset_Body = "BODY";

        public const string Refset_SubTool = "SUB_TOOL";

        public const string Refset_CBore = "CBORE";

        public const string Refset_ShortTap = "SHORT-TAP";

        public const string Refset_Ream = "REAM";

        public const string Refset_ReamShort = "REAM_SHORT";

        public const string Refset_BodyEdge = "BODY_EDGE";

        public const string Refset_ClrHole = "CLR_HOLE";

        public const string Refset_ClrScrewHead = "CLR_SCREW_HEAD";

        public const string Refset_HardTapClr = "HARD_TAP_CLR";

        public const string Refset_SlotCBore = "SLOT_CBORE";

        public const string Refset_CBoreBlind = "CBORE_BLIND";

        public const string Refset_CBoreBlindOpp = "CBORE_BLIND_OPP";

        public const string Refset_Grid = "GRID";

        public const string Refset_Tap = "TAP";

        public const string Refset_Handling = "HANDLING";

        public const string Refset_Tooling = "TOOLING";

        public const string Refset_BlindReam = "BLIND_REAM";

        public const string Refset_WireTap = "WIRE_TAP";


        public const double Tolerance_English = .001;

        public const double Tolerance_Metric = Tolerance_English * 25.4;


        public const string ShcsCBoreHolechart = "SHCS_CBORE_HOLECHART";

        public const string ShcsCBoreDepth = "SHCS_CBORE_DEPTH";

        public const string ShcsCBoreHeadDia = "SHCS_CBORE_HEAD_DIA";

        public const string ShcsShortTapHolechart = "SHCS_SHORT_TAP_HOLECHART";

        public const string ShcsTapHolechart = "SHCS_TAP_HOLECHART";

        public const string ShcsShortTapDepth = "SHCS_SHORT_TAP_DEPTH";

        public const string ShcsTapDepth = "SHCS_TAP_DEPTH";

        public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        private const int lowerA = 97;

        private const int lowerZ = 122;

        private const int upperA = 65;

        private const int upperZ = 90;

        #endregion
    }
}