namespace TSG_Library.Utilities
{
    public class CtsAttributes
    {
        private string _attrName;
        private string _attrValue;

        public CtsAttributes()
        {
        }

        public CtsAttributes(string attributeName, string attributeValue)
        {
            AttrName = attributeName;
            AttrValue = attributeValue;
        }

        public string AttrName
        {
            get => _attrName;
            set => _attrName = value ?? "NO NAME";
        }

        public string AttrValue
        {
            get => _attrValue;
            set => _attrValue = value ?? "NO VALUE";
        }

        public override string ToString()
        {
            return _attrValue;
        }
    }
}