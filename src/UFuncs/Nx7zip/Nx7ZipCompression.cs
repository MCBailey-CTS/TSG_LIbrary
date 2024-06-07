namespace TSG_Library.Utilities
{
    public class Nx7ZipCompression
    {
        public Nx7ZipCompression()
        {
        }

        public Nx7ZipCompression(string displayAs, string winZipArg)
        {
            CompressDisplay = displayAs;
            CompressValue = winZipArg;
        }

        public string CompressDisplay { get; }

        public string CompressValue { get; }

        public override string ToString()
        {
            return CompressDisplay;
        }
    }
}