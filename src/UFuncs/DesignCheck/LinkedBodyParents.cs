using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class LinkedBodyParents : IDesignCheck
    {
        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    return true;
        //}

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return DCResult.fail;
        }

        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
        }
    }
}