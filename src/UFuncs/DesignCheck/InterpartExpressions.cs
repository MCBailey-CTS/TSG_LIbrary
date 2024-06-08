using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public class InterpartExpressions : IDesignCheck
    {
        //public TreeNode PerformCheck(NXOpen.Part part)
        //{
        //    GFolder folder = GFolder.create(part.FullPath);
        //    TreeNode part_node = new TreeNode(part.Leaf) { Tag = part };

        //    foreach (NXOpen.Expression expression in part.Expressions.ToArray())
        //        try
        //        {
        //            if (!expression.IsInterpartExpression)
        //                continue;

        //            expression.GetInterpartExpressionNames(out string partName, out _);

        //            if (partName.StartsWith(folder.dir_job))
        //                continue;

        //            TreeNode exp_node = new TreeNode(expression.Name) { Tag = expression };
        //            part_node.Nodes.Add(exp_node);
        //        }
        //        catch (Exception ex)
        //        {
        //            ex._PrintException();
        //        }

        //    return part_node;
        //}

        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            try
            {
                return !(GFolder.create_or_null(part) is null);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return false;
                ;
            }
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            var folder = GFolder.create(part.FullPath);
            result_node = new TreeNode(part.Leaf) { Tag = part };
            var passed = true;

            foreach (var expression in part.Expressions.ToArray())
                try
                {
                    if(!expression.IsInterpartExpression)
                        continue;

                    expression.GetInterpartExpressionNames(out var partName, out _);

                    if(partName.StartsWith(folder.dir_job))
                        continue;

                    var exp_node = new TreeNode(expression.Name) { Tag = expression };
                    result_node.Nodes.Add(exp_node);
                    passed = false;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

            return passed;
        }
    }
}