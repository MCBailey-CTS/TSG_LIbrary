using System;
using System.Linq;
using System.Windows.Forms;
using NXOpen;

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

        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            if(!part.__HasAttribute(Burnouts_Titles_1))
                return false;

            var value = part.GetUserAttributeAsString(Burnouts_Titles_1, NXObject.AttributeType.String, -1).ToUpper();
            return Burnouts_Values_1.Any(__m => __m.ToUpper() == value);
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return false;
        }

        public TreeNode PerformCheck(Part part)
        {
            var part_node = part.__TreeNode();
            var passed = part.DrawingSheets.ToArray().Any(__s => __s.Name.ToUpper() == Burnouts_DrawingSheetName);
            part_node.Text = $"{part_node.Text} -> {passed}";
            return part_node;
        }
    }
}