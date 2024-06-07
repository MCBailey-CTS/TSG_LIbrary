namespace TSG_Library.UFuncs
{
    public class FastenerListItem
    {
        public FastenerListItem(string __text, string __value)
        {
            Text = __text;
            Value = __value;
        }

        public string Text { get; set; }

        public string Value { get; }
    }
}