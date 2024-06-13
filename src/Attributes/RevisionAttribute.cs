using System;
using System.Collections.Generic;

namespace TSG_Library.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RevisionAttribute : Attribute
    {
        public RevisionAttribute(string revisionLevel, string revisionMessage)
        {
            RevisionLevel = revisionLevel;
            RevisionMessage = revisionMessage;

            string[] split = revisionLevel.Split('.');

            List<int> list = new List<int>();

            foreach (string i in split)
            {
                if (!int.TryParse(i, out int number))
                {
                    continue;
                }

                list.Add(number);
            }

            Levels = list.ToArray();
        }

        public string RevisionLevel { get; }
        public string RevisionMessage { get; }

        public int[] Levels { get; }
    }
}