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
        private const string Burnouts_DrawingSheetName = "BURNOUT";
        private const string Burnouts_Titles_1 = "MATERIAL";

        private static readonly string[] Burnouts_Values_1 =
        {
            "HRS PLT",
            "4140 PH PLT",
            "4140 PLT"
        };

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();

            if (!part.__HasDrawingSheet(Burnouts_DrawingSheetName))
            {
                result_node.Nodes.Add($"Did not have a drawing sheet named {Burnouts_DrawingSheetName}");
                return DCResult.ignore;
            }

            if (!part.__HasAttribute(Burnouts_Titles_1))
            {
                result_node.Nodes.Add($"Did not have attribute {Burnouts_Titles_1}");
                return DCResult.fail;
            }

            var attValue = part.__GetAttribute(Burnouts_Titles_1);

            if (Burnouts_Values_1.All(__v => __v.ToUpper() != attValue.ToUpper()))
            {
                result_node.Nodes.Add($"Invalid BURNOUT MATERIAL: {attValue}");
                return DCResult.fail;
            }

            return DCResult.pass;
        }
    }
}