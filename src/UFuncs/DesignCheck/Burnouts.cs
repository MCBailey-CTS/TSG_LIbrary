using System;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class Burnouts : IDesignCheck
    {
        private static readonly string[] Burnouts_Values_1 =
        {
            "HRS PLT",
            "4140 PH PLT",
            "4140 PLT"
        };

        // what happens when you have a valid material attribute but no BURNOUT drawing sheet.

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();

            if (!part.__HasDrawingSheet("BURNOUT"))
            {
                result_node.Nodes.Add("Did not have a drawing sheet named 'BURNOUT'");
                return DCResult.ignore;
            }

            if (!part.__HasAttribute("MATERIAL"))
            {
                result_node.Nodes.Add($"Did not have attribute 'MATERIAL'");
                return DCResult.fail;
            }

            var attValue = part.__GetAttribute("MATERIAL");

            if (Burnouts_Values_1.All(__v => __v.ToUpper() != attValue.ToUpper()))
            {
                result_node.Nodes.Add($"Invalid BURNOUT MATERIAL: {attValue}");
                return DCResult.fail;
            }

            return DCResult.pass;
        }
    }
}