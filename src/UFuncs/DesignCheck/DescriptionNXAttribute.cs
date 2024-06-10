using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class DescriptionNXAttribute : IDesignCheck
    {
        //[Obsolete]
        //public bool IsPartValidForCheck(Part part, out string message)
        //{
        //    message = "";
        //    return true;
        //}

        public DCResult PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return DCResult.fail;
        }

        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
            //TreeNode part_node = new TreeNode(part.Leaf) { Tag = part };

            //string requiredAttributeTitle = Model.Ucf.SingleValue(GetType()
            //    .GetCustomAttributes(typeof(TitlesAttribute), true).OfType<TitlesAttribute>().Single().UcfTitlesHeader);

            //if (part is null) throw new ArgumentNullException(nameof(part));
            //TheUFSession.Assem.AskOccsOfPart(__display_part_.Tag, part.Tag, out Tag[] partOccsTags);

            //List<Tuple<Component, IEnumerable<string>>> failedComponents = new List<Tuple<Component, IEnumerable<string>>>();

            //foreach (Tag partOcc in partOccsTags)
            //{
            //    Component comp = (Component)NXOpen.Utilities.NXObjectManager.Get(partOcc);

            //    Part prototype = comp._Prototype();
            //    string partDescription = prototype.GetUserAttributeAsString(requiredAttributeTitle, NXObject.AttributeType.String, -1);
            //    string compDescription = comp.GetUserAttributeAsString(requiredAttributeTitle, NXObject.AttributeType.String, -1);

            //    if (string.Equals(partDescription, compDescription, StringComparison.CurrentCultureIgnoreCase))
            //        continue;

            //    failedComponents.Add(new Tuple<Component, IEnumerable<string>>(comp, new[] { "Component attribute does not equal Part attribute" }));

            //}


            //var failedComponents = (from partOcc in partOccsTags
            //                        select (NXOpen.Assemblies.Component)NXOpen.Utilities.NXObjectManager.Get(partOcc)
            //    into component
            //                        let prototype = (NXOpen.Part)component.Prototype
            //                        let partDescription = prototype.GetUserAttributeAsString(requiredAttributeTitle, NXOpen.NXObject.AttributeType.String, -1)
            //                        let compDescription = component.GetUserAttributeAsString(requiredAttributeTitle, NXOpen.NXObject.AttributeType.String, -1)
            //                        where !string.Equals(partDescription, compDescription, StringComparison.CurrentCultureIgnoreCase)
            //                        select (component, new[] { "Component attribute does not equal Part attribute" })).Select(dummy => (Tuple<NXOpen.Assemblies.Component, IEnumerable<string>>)dummy).ToList();

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
        }

        /*

        :SIZE_DESCRIPTION_TITLES_1:
MATERIAL
:END:
        */

        ////[Titles("DESCRIPTION_TITLES")]
        //[Obsolete]
        //public IEnumerable<TreeListNode> Validate(Part part)
        //{
        //    throw new NotImplementedException();

        //}
    }
}