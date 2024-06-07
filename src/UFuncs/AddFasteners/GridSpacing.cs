namespace TSG_Library.UFuncs
{
    public struct GridSpacing
    {
        public double Value { get; private set; }

        public string StringValue { get; }

        public GridSpacing(double value, string stringValue)
            : this()
        {
            Value = value;
            StringValue = stringValue;
        }

        public override string ToString()
        {
            return StringValue;
        }
    }
}