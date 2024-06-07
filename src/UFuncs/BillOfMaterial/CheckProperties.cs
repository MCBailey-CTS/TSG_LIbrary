namespace TSG_Library.UFuncUtilities.BomUtilities
{
    public struct CheckProperties
    {
        /// <summary>The path to the template file used by the check department.</summary>
        /// <remarks>Revision 1.6 – 2018 / 01 / 10</remarks>
        public static string CheckerTemplateFilePath => "U:\\nxFiles\\Excel\\CheckerTemplate.xlsx";

        /// <summary>The column in the check template for the detail number.</summary>
        public static int DetailColumn => 5;

        /// <summary>The column in the check template for the required number.</summary>
        public static int RequiredColumn => 6;

        /// <summary>The column in the check template for the description.</summary>
        public static int DescriptionColumn => 7;

        /// <summary>The column in the check template for the material.</summary>
        public static int MaterialColumn => 8;

        /// <summary>The row in the check template to start the details.</summary>
        public static int StartRow => 7;
    }
}