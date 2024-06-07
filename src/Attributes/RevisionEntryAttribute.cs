using System;

namespace TSG_Library.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RevisionEntryAttribute : Attribute
    {
        public RevisionEntryAttribute(string revisionLevel, string year, string month, string day)
        {
            RevisionLevel = revisionLevel;
            Year = year;
            Month = month;
            Day = day;
        }

        public string RevisionLevel { get; set; }
        public string Year { get; set; }

        public string Month { get; set; }

        public string Day { get; set; }

        //        public RevisionEntryAttribute(double revisionLevel,     int year, int month, int day)
        //        {
        //            RevisionLevel = revisionLevel;
        //            Year = year;
        //            Month = month;
        //            Day = day;
        //        }
        //        
    }
}