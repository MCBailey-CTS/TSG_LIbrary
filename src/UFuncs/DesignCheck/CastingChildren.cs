using System;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class CastingChildren : IDesignCheck
    {
        public static string[] CASTING_MATERIALS_VALUES =
        {
            "GM190",
            "GM238",
            "GM241",
            "GM245",
            "GM246",
            "GM338",
            "S0050A",
            "S0030A",
            "G2500",
            "G3500",
            "D4512",
            "D5506",
            "D6510",
            "CARMO CAST",
            "GM68M",
            "S0030"
        };

        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    if(!part.__IsPartDetail())
        //        return false;

        //    if(!part.__HasAttribute("MATERIAL"))
        //    {
        //        message = $"Part {part.Leaf} doesn't have a MATERIAL attribute";
        //        return false;
        //    }
        //    //throw new InvalidOperationException($"Part {part.Leaf} doesn't have a MATERIAL attribute");

        //    var material_value = part.__GetAttribute("MATERIAL").ToUpper();

        //    return CASTING_MATERIALS_VALUES.Any(__value => __value.ToUpper() == material_value);
        //}

        //public TreeNode PerformCheck(NXOpen.Part part)
        //{
        //}

        [Obsolete]
        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();

            if(!part.__IsPartDetail())
            {
                result_node.Nodes.Add("Is not a part detail");
                return DCResult.ignore;
            }

            if(!part.__HasAttribute("MATERIAL"))
            {
                result_node.Nodes.Add("No MATERIAL attribute");
                return DCResult.ignore;
            }


            result_node = part.__TreeNode();
            return DCResult.fail;

            //if (part.ComponentAssembly.RootComponent.GetChildren().Length == 0)
            //    return null;

            //bool flag = (from child in part.ComponentAssembly.RootComponent.GetChildren()
            //             where child.Prototype is NXOpen.Part
            //             where child.__Prototype().FullPath.ToLower().Contains("liftlugs")
            //             where child.ReferenceSet != "Empty"
            //             select child).All(child => child.ReferenceSet != "Entire Part");

            //if (flag)
            //    return null;

            //return new TreeNode(part.Leaf) { Tag = part };
            //throw new NotImplementedException();
        }
    }
}