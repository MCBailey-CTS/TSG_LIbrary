using System;

namespace TSG_Library.Attributes
{
    public class RevisionLogAttribute : Attribute
    {
        public RevisionLogAttribute(string revisionName)
        {
            RevisionName = revisionName;
        }

        public string RevisionName { get; }
    }
}