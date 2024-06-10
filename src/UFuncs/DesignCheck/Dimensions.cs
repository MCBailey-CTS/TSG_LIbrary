using System;
using System.Windows.Forms;
using NXOpen;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    /// <summary>Checks all the dimensions in an assembly are associative and up to date.</summary>
    public class Dimensions : IDesignCheck
    {
        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    return true;
        //}

        [Obsolete]
        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = new TreeNode(part.Leaf) { Tag = part };
            var passed = true;

            foreach (var dimension in part.Dimensions.ToArray())
                //if (dimension._IsAssociative())
                //    continue;
                //passed = false;
                //result_node.Nodes.Add(new TreeNode(dimension._OwningDrawingSheet().Name) { Tag = dimension });
                throw new NotImplementedException();

            return DCResult.fail;
        }
    }
}