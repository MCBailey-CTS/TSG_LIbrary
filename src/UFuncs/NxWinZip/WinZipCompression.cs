namespace TSG_Library.Utilities
{
    public class WinZipCompression
    {
        public WinZipCompression()
        {
        }

        public WinZipCompression(string displayAs, string winZipArg)
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