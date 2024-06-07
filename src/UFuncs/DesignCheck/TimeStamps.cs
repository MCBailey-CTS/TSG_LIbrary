﻿using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    public class TimeStamps : IDesignCheck
    {
        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            return true;
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();

            //bool passed = true;

            return false;
        }

        public TreeNode PerformCheck(Part part)
        {
            var part_node = new TreeNode(part.Leaf) { Tag = part };

            foreach (var feature in part.Features.ToArray())
                try
                {
                    if(!(feature is ExtractFace extract_face)
                       || !extract_face._IsLinkedBody())
                        continue;

                    var builder = extract_face.__OwningPart().Features.CreateExtractFaceBuilder(extract_face);

                    using (session_.using_builder_destroyer(builder))
                    {
                        if(!builder.FixAtCurrentTimestamp)
                            continue;

                        var extract_face_node = new TreeNode(extract_face.GetFeatureName())
                        {
                            Tag = extract_face
                        };

                        part_node.Nodes.Add(extract_face_node);
                    }
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }

            return part_node;
        }
    }
}