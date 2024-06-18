using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class WireTaperNotes : IDesignCheck
    {
        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    return part.__HasAttribute("WTN") && part.__GetAttribute("WTN") == "YES";
        //}

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return DCResult.fail;
        }

        public TreeNode PerformCheck(Part part)
        {
            if (part.__HasAttribute("DETAIL NAME"))
                throw new InvalidOperationException($"Part {part.Leaf} doesn't have a 'DETAIL NAME' attribute");

            if (part.__GetAttribute("DETAIL NAME") == "TRIM")
                return null;

            return part.__TreeNode();
        }
    }
}