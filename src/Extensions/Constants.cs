// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
namespace TSG_Library.Extensions
{
    public partial class Extensions
    {
        #region Constants

        /// <summary>
        ///     The path to the Concept Control File.
        /// </summary>
        public const string ControlFile = @"U:\NX110\Concept\ConceptControlFile.UCF";

        //public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        //public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        public const string ViewPlan = "PLAN";


        /// <summary>The layer to put tooling holes on.</summary>
        /// <remarks>Layer = 97</remarks>
        public const int LayerDwlTooling = 97;

        /// <summary>The layer to place regular fasteners on.</summary>
        /// <remarks>Layer = 99</remarks>
        public const int LayerFastener = 99;

        /// <summary>The layer to put handling holes and wire taps on.</summary>
        /// <remarks>Layer = 98</remarks>
        public const int LayerShcsHandlingWireTap = 98;


        /// <summary>The regular expression that matches a n assembly folderWithCtsNumber.</summary>
        /// <remarks>Expressions = "^(?<customerNum>\d+)-(?<opNum>\d+)$"</opNum></customerNum></remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string RegexAssemblyFolder = @"^(?<customerNum>\d+)-(?<opNum>\d+)$";


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
        public const string RegexBhcs = @"^(?<diameter>\d+)(?:mm)?-bhcs-(?<length>\d+)$";


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
        public const string RegexBhcsEnglish = @"^(?<diameter>\d+)-bhcs-(?<length>\d+)$";


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
        public const string RegexBhcsMetric = @"^(?<diameter>\d+)mm-bhcs-(?<length>\d+)$";


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
        public const string RegexBlank = @"^(?<customerNum>\d+)-(?<opNum>\d+)-blank$";


        /// <summary>The regular expression pattern that matches a metric castle nut.</summary>
        /// <remarks>Expressions = "^Hexagon Castle Nut M(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string RegexCastleNutMetric = @"^Hexagon Castle Nut M(?<diameter>\d+)$";


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
        public const string RegexDetail = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


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
        public const string RegexDiesetControl = @"^(?<customerNum>\d+)-(?<opNum>\d+)-dieset-control$";


        /// <summary>The regular expression that matches a double.</summary>
        /// <remarks>Expressions = "(?&lt;double&gt;\\d+\\.\\d+|\\d+\\.|\\.\\d+|\\d+)"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>double</description>
        ///     </item>
        /// </list>
        public const string RegexDoubleDecimal = @"(?<double>\d+\.\\d+|\\d+\\.|\\.\\d+|\\d+)";


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
        public const string RegexDwl = @"^(?<diameter>\d+)(?:mm)?-dwl-(?<length>\d+)$";


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
        public const string RegexDwlEnglish = @"^(?<diameter>\d+)-dwl-(?<length>\d+)$";


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
        public const string RegexDwlLckMetric = @"^(?<diameter>\d+)mm-dwl-lck-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric misumi dowel lock (SWA).</summary>
        /// <remarks>Expressions = "^SWA(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string RegexDwlLckMisumiMetric = @"^SWA(?<diameter>\d+)$";


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
        public const string RegexDwlMetric = @"^(?<diameter>\d+)mm-dwl-(?<length>\d+)$";


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
        public const string RegexFhcs = @"^(?<diameter>\d+)(?:mm)?-fhcs-(?<length>\d+)$";


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
        public const string RegexFhcsEnglish = @"^(?<diameter>\d+)-fhcs-(?<length>\d+)$";


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
        public const string RegexFhcsMetric = @"^(?<diameter>\d+)mm-fhcs-(?<length>\d+)$";


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
        public const string RegexJckScrew = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw$";


        /// <summary>The regular expression pattern that matches an english jack screw (jck-screw).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string RegexJckScrewEnglish = @"^_(?<diameter>\d+)-jck-screw$";


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
        public const string RegexJckScrewMetric = @"^(?<diameter>\d+)mm-jck-screw$";


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
        public const string RegexJckScrewTsg = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw-tsg$";


        /// <summary>The regular expression pattern that matches an english tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string RegexJckScrewTsgEnglish = @"^_(?<diameter>\d+)-jck-screw-tsg$";


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
        public const string RegexJckScrewTsgMetric = @"^(?<diameter>\d+)mm-jck-screw-tsg$";


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
        public const string RegexLayout = @"^(?<customerNum>\d+)-(?<opNum>\d+)-layout$";


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
        public const string RegexLhcs = @"^(?<diameter>\d+)(?:mm)?-lhcs-(?<length>\d+)$";


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
        public const string RegexLhcsEnglish = @"^(?<diameter>\d+)-lhcs-(?<length>\d+)$";


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
        public const string RegexLhcsMetric = @"^(?<diameter>\d+)mm-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric lock washer.</summary>
        /// <remarks>Expressions = "^lock washer for (?&lt;diameter&gt;\d+)mm shcs$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string RegexLockWasherMetric = @"^lock washer for (?<diameter>\d+)mm shcs$";


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
        public const string RegexLsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsh$";


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
        public const string RegexLshUsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<shoe>lsh|ush)$";


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
        public const string RegexLsp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsp(?<extraOpNum>\d+)?.*$";


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
        public const string RegexLspUsp =
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
        public const string RegexLwr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lwr$";


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
        public const string RegexLwrUpr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-[lwr|upr]$";


        /// <summary>The regular expression that matches a mathdata part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-mathdata$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string RegexMathdata = @"^(?<customerNum>\d+)-mathdata$";


        /// <summary>The regular expression that matches a master part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-Master$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string RegexMaster = @"^(?<customerNum>\d+)-Master$";


        /// <summary>The regular expression that matches a history part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-History$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string RegexHistory = @"^(?<customerNum>\d+)-History$";


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
        public const string RegexNut = @"^nut-(?<diameter>\d+)-M?(?<threadCount>\d+(?:\.\d+)?)$";


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
        public const string RegexNutEnglish = @"^nut-(?<diameter>\d+)-(?<threadCount>\d+)$";


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
        public const string RegexNutMetric = @"^nut-(?<diameter>\d+)-M(?<threadCount>\d+(?:\.\d+)?)$";


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
        public const string RegexOp000Holder = @"^(?<customerNum>\d+)-(?<opNum>\d+)-000$";


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
        public const string RegexPressAssembly = @"^(?<customerNum>\d+)-Press-(?<opNum>\d+)-Assembly$";


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
        public const string RegexShcs = @"^(?<diameter>\d+)(?:mm)?-shcs-(?<length>\d+)$";


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
        public const string RegexShcsEnglish = @"^(?<diameter>\d+)-shcs-(?<length>\d+)$";


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
        public const string RegexShcsMetric = @"^(?<diameter>\d+)mm-shcs-(?<length>\d+)$";


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
        public const string RegexShss = @"^(?<diameter>\d+)(?:mm)?-shss-(?<length>\d+)$";


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
        public const string RegexShssEnglish = @"^(?<diameter>\d+)-shss-(?<length>\d+)$";


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
        public const string RegexShssMetric = @"^(?<diameter>\d+)mm-shss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a simulation part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-simulation$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        internal const string RegexSimulation = @"^(?<customerNum>\d+)-simulation$";


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
        public const string RegexSss = @"^(?<diameter>\d+)(?:mm)?-sss-(?<length>\d+)$";


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
        public const string RegexSssEnglish = @"^(?<diameter>\d+)-sss-(?<length>\d+)$";


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
        public const string RegexSssMetric = @"^(?<diameter>\d+)mm-sss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a strip.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-strip$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string RegexStrip = @"^(?<customerNum>\d+)-(?<opNum>\d+)-strip$";


        /// <summary>The regular expression that matches a strip control.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-strip-control$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string RegexStripControl = @"^(?<customerNum>\d+)-strip-control$";


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
        public const string RegexUpr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-upr$";


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
        public const string RegexUsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-ush$";


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
        public const string RegexUsp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-usp(?<extraOpNum>\d+)?.*$";


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
        public const string Regex999 = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>99\d)$";

        public const string FilePathWireTap =
            FilePath0LibraryFasteners + "\\Metric\\SocketHeadCapScrews\\008\\8mm-shcs-020.prt";

        public const string FilePath0Library = "G:\\0Library";

        public const string FilePath0Press = "G:\\0Press";

        public const string FilePath0LibraryFasteners = FilePath0Library + "\\Fasteners";

        public const string FilePathDxfDwgDef = "C:\\Program Files\\Siemens\\NX 11.0\\dxfdwg\\dxfdwg.def";

        public const string FilePathUcf = @"U:\nxFiles\UfuncFiles\ConceptControlFile.ucf";

        public const string FilePathStep214Ug = "C:\\Program Files\\Siemens\\NX 11.0\\NXBIN\\step214ug.exe";

        public const string FilePathExternalStepAllLayersDef =
            "U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def";

        public const string FilePathExternalStepDesignDef = "U:\\nxFiles\\Step Translator\\ExternalStep_Design.def";

        public const string FilePathExternalStepAssemblyDef =
            @"U:\nxFiles\Step Translator\ExternalStep_Assembly.def";

        public const string FilePathExternalStepDetailDef = @"U:\nxFiles\Step Translator\ExternalStep_Detail.def";

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


        public const int ColorTrim = 186;

        public const int ColorFinish = 42;


        public const string RefsetEntirePart = "Entire Part";

        public const string RefsetEmpty = "Empty";

        public const string RefsetBody = "BODY";

        public const string RefsetSubTool = "SUB_TOOL";

        public const string RefsetCBore = "CBORE";

        public const string RefsetShortTap = "SHORT-TAP";

        public const string RefsetReam = "REAM";

        public const string RefsetReamShort = "REAM_SHORT";

        public const string RefsetBodyEdge = "BODY_EDGE";

        public const string RefsetClrHole = "CLR_HOLE";

        public const string RefsetClrScrewHead = "CLR_SCREW_HEAD";

        public const string RefsetHardTapClr = "HARD_TAP_CLR";

        public const string RefsetSlotCBore = "SLOT_CBORE";

        public const string RefsetCBoreBlind = "CBORE_BLIND";

        public const string RefsetCBoreBlindOpp = "CBORE_BLIND_OPP";

        public const string RefsetGrid = "GRID";

        public const string RefsetTap = "TAP";

        public const string RefsetHandling = "HANDLING";

        public const string RefsetTooling = "TOOLING";

        public const string RefsetBlindReam = "BLIND_REAM";

        public const string RefsetWireTap = "WIRE_TAP";


        public const double ToleranceEnglish = .001;

        public const double ToleranceMetric = ToleranceEnglish * 25.4;


        public const string ShcsCBoreHolechart = "SHCS_CBORE_HOLECHART";

        public const string ShcsCBoreDepth = "SHCS_CBORE_DEPTH";

        public const string ShcsCBoreHeadDia = "SHCS_CBORE_HEAD_DIA";

        public const string ShcsShortTapHolechart = "SHCS_SHORT_TAP_HOLECHART";

        public const string ShcsTapHolechart = "SHCS_TAP_HOLECHART";

        public const string ShcsShortTapDepth = "SHCS_SHORT_TAP_DEPTH";

        public const string ShcsTapDepth = "SHCS_TAP_DEPTH";

        public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        private const int LowerA = 97;

        private const int LowerZ = 122;

        private const int UpperA = 65;

        private const int UpperZ = 90;

        #endregion
    }
}