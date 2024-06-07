namespace TSG_Library.UFuncUtilities.BomUtilities
{
    public struct BomProperties
    {
        /// <summary>The column in the Bom template for the detail number.</summary>
        public const int DetailColumn = 1;

        /// <summary>The column in the Bom template for the required number.</summary>
        public const int RequiredColumn = 2;

        /// <summary>The column in the Bom template for the description.</summary>
        public const int DescriptionColumn = 3;

        /// <summary>The column in the Bom template for the material.</summary>
        public const int MaterialColumn = 4;

        /// <summary>The row in the Bom template to start the details.</summary>
        public const int StartRow = 16;
    }
}