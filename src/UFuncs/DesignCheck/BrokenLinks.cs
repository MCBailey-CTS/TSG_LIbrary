using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public class BrokenLinks : IDesignCheck
    {
        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "valid";
            return part.__IsPartDetail();
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = new TreeNode(part.Leaf) { Tag = part };

            var passed = true;

            foreach (var feature in part.Features.ToArray())
                try
                {
                    if(!(feature is ExtractFace extract_face)
                       || !extract_face.__IsLinkedBody()
                       || !extract_face.__IsBroken())
                        continue;

                    var extract_face_node = new TreeNode(extract_face.GetFeatureName())
                    {
                        Tag = extract_face
                    };

                    passed = false;

                    result_node.Nodes.Add(extract_face_node);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

            return passed;
        }
    }
}