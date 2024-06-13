using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public class TimeStamps : IDesignCheck
    {
        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    return true;
        //}

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();

            //bool passed = true;

            return DCResult.fail;
        }

        public TreeNode PerformCheck(Part part)
        {
            TreeNode part_node = new TreeNode(part.Leaf) { Tag = part };

            foreach (Feature feature in part.Features.ToArray())
                try
                {
                    if (!(feature is ExtractFace extract_face)
                        || !extract_face.__IsLinkedBody())
                        continue;

                    ExtractFaceBuilder builder =
                        extract_face.__OwningPart().Features.CreateExtractFaceBuilder(extract_face);

                    using (session_.__UsingBuilderDestroyer(builder))
                    {
                        if (!builder.FixAtCurrentTimestamp)
                            continue;

                        TreeNode extract_face_node = new TreeNode(extract_face.GetFeatureName())
                        {
                            Tag = extract_face
                        };

                        part_node.Nodes.Add(extract_face_node);
                    }
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

            return part_node;
        }
    }
}