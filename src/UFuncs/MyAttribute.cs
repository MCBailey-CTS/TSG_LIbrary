using NXOpen;

namespace TSG_Library.UFuncs
{
    public partial class CopyAttributesForm
    {
        internal class MyAttribute
        {
            private static readonly string[] validAttributes =
            {
                "BREAK", "CTS NUMBER", "JOB NUMBER", "DESIGNER", "DATE", "SHOP", "CUSTOMER", "UNITS", "SHOP 1 NAME",
                "EC"
            };


            private NXObject.AttributeInformation attribute;


            public MyAttribute(NXObject.AttributeInformation attribute)
            {
                this.attribute = attribute;
            }

            public NXObject.AttributeInformation Attribute
            {
                get => attribute;
                set => attribute = value;
            }


            public override string ToString()
            {
                return attribute.Title;
            }


            public static bool IsValidAttribute(NXObject.AttributeInformation attribute)
            {
                foreach (string title in validAttributes)
                    if (title == attribute.Title)
                        return true;
                return false;
            }
        }
    }
}