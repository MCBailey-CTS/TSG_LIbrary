using NXOpen;

namespace TSG_Library.UFuncs
{
    public struct ListItem
    {
        public Tag Tag { get; }
        public string TagText { get; }

        public ListItem(Tag tag, string tagText)
        {
            Tag = tag;
            TagText = tagText;
        }

        public override string ToString()
        {
            return TagText;
        }
    }
}