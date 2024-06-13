using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Utilities;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class ComponentNames : IDesignCheck
    {
        [Obsolete]
        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            return true;
        }

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return DCResult.fail;
        }

        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
#pragma warning disable CS0162 // Unreachable code detected
            TreeNode part_node = new TreeNode(part.Leaf) { Tag = part };
#pragma warning restore CS0162 // Unreachable code detected
            ufsession_.Assem.AskOccsOfPart(__display_part_.Tag, part.Tag, out Tag[] partOccsTags);
            List<Tuple<Component, IEnumerable<string>>> failedComponents = new List<Tuple<Component, IEnumerable<string>>>();

            foreach (Tag partOcc in partOccsTags)
            {
                Component component = (Component)NXObjectManager.Get(partOcc);

                if(component.IsSuppressed)
                    continue;

                var expectedName = Regex.Match(component.DisplayName, "[0-9]{4,}-[0-9]{3}-([0-9]{3})").Groups[1].Value;

                // todo: Look into the Regex.Replace method. You might be able to use it with the regex below.
                // Revision 2.1 – 2018 / 01 /30
                // todo: should be able to combine the following statement into one Regex expression.
                if(component.Name == expectedName || Regex.IsMatch(component.Name, $"^{expectedName}([A-Z]{{1}})$"))
                    continue;

                failedComponents.Add(new Tuple<Component, IEnumerable<string>>(component,
                    new[] { $"Expected Name: {expectedName}", $"Actual Name: {component.Name}" }));
            }

            //if (failedComponents.Count == 0)
            //{
            //    yield return new DesignCheckResult(true, part, this,
            //        new ObjectNode($"All part occurrences passed."));
            //    yield break;
            //}

            //List<ObjectNode> errorObjectNodes = new List<ObjectNode>();
            //foreach (Tuple<Component, IEnumerable<string>> tuple in failedComponents)
            //{
            //    ObjectNode linkedBodyNode = new ObjectNode(tuple.Item1.DisplayName);
            //    linkedBodyNode.AddRange(tuple.Item2.Select(s => new ObjectNode(s)));
            //    errorObjectNodes.Add(linkedBodyNode);
            //}

            //yield return new DesignCheckResult(false, part, this, errorObjectNodes);
            //yield break;

            return part_node;
        }
    }
}