using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class FolderLocations : IDesignCheck
    {
        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            return true;
        }


        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            var folder = GFolder.create_or_null(part);

            if(!(folder is null))
            {
                result_node = part.__TreeNode()._SetText($"{part.Leaf} -> not in folder");
                return false;
            }

            string[] regexes =
            {
                "G:\\0Library",
                "G:\\0Press",
                "W:\\"
            };

            foreach (var str in regexes)
                if(part.FullPath.ToLower().StartsWith(str.ToLower()))
                {
                    result_node = part.__TreeNode();
                    return true;
                }

            result_node = part.__TreeNode();
            return false;
        }
    }
}