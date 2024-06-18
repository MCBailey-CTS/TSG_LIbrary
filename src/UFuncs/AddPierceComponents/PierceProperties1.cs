using System.Text.RegularExpressions;

namespace TSG_Library.UFuncs
{
    public partial class AddPierceComponentsForm
    {
        public static class PierceProperties
        {
            /// <summary>
            ///     The path to the "Smart Button Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartButtonMetric => "G:\\0Library\\Button\\Smart Button Metric.prt";

            /// <summary>
            ///     The path to the "Smart Button English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartButtonEnglish => "G:\\0Library\\Button\\Smart Button English.prt";

            /// <summary>
            ///     The path to the "Smart Punch Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartPunchMetric => "G:\\0Library\\PunchPilot\\Metric\\Smart Punch Metric.prt";

            /// <summary>
            ///     The path to the "Smart Punch English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartPunchEnglish => "G:\\0Library\\PunchPilot\\English\\Smart Punch English.prt";

            /// <summary>
            ///     The path to the "Smart Ball Lock Retainer Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartBallLockRetainerMetric =>
                "G:\\0Library\\Retainers\\Metric\\Smart Ball Lock Retainer Metric.prt";

            /// <summary>
            ///     The path to the "Smart Ball Lock Retainer English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartBallLockRetainerEnglish =>
                "G:\\0Library\\Retainers\\English\\Smart Ball Lock Retainer English.prt";

            /// <summary>
            ///     Returns the name of the reference set to be used for AddPierceComponents.
            /// </summary>
            public static string SlugRefsetName => "SLUG BUTTON ALIGN";

            // ReSharper disable once StringLiteralTypo
            public static string RetainerAlignPunchFaceName => "ALIGNPUNCH";

            // ReSharper disable once StringLiteralTypo
            public static string PunchTopFaceName => "PUNCHTOPFACE";

            public static string PiercedP => "PIERCED_P";

            public static string PiercedW => "PIERCED_W";

            public static string PiercedType => "PIERCED_TYPE";

            public static Regex PiercedFaceRegex => new Regex("^PIERCED_([0-9]{1,})$");
        }
    }
}