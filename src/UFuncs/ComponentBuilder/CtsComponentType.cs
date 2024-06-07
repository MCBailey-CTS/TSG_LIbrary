using TSG_Library.Utilities;

namespace TSG_Library.UFuncs
{
    internal class CtsComponentType
    {
        public CtsComponentType(string name, CtsComponentColor color)
        {
            ColorName = name;
            ComponentColor = (int)color;
        }

        public CtsComponentType(string attrName, string componentName)
        {
            AttributeName = attrName;
            ComponentName = componentName;
        }

        public CtsComponentType(string attrName, string material, string componentName, CtsComponentColor color)
        {
            AttributeName = attrName;
            Material = material;
            ComponentName = componentName;
            ComponentColor = (int)color;
        }

        public CtsComponentType(string attrName, string material, string componentName, CtsComponentColor color,
            bool burnout, bool grind, bool dieset)
        {
            AttributeName = attrName;
            Material = material;
            ComponentName = componentName;
            ComponentColor = (int)color;
            IsBurnout = burnout;
            IsDieset = dieset;
            IsGrind = grind;
        }

        public string AttributeName { get; set; }

        public string Material { get; set; }

        public string ComponentName { get; set; }

        public string ColorName { get; set; }

        public int ComponentColor { get; set; }

        public bool IsBurnout { get; set; }

        public bool IsDieset { get; set; }

        public bool IsGrind { get; set; }

        public override string ToString()
        {
            return AttributeName == null
                ? string.Format("{0}", ColorName)
                : string.Format("{0}", ComponentName);
        }
    }
}