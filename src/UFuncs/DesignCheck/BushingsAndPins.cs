using System;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class BushingsAndPins : IDesignCheck
    {
        public string[] GuideBushings_GuidePins_Values =
        {
            "GuideBushings",
            "GuidePins"
        };

        public bool IsPartValidForCheck(Part part, out string message)
        {
            message = "";
            return true;
        }

        public bool PerformCheck(Part part, out TreeNode result_node)
        {
            result_node = part.__TreeNode();
            return false;
        }

        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            //    throw new NotImplementedException();
            //}

            ////[DisplayPartOnly]
            //public IEnumerable<TreeNode> Validate(NXOpen.Part nxPart)
            //{
            //    if 

            if(!part.__HasAttribute("LibraryPath"))
                return part.__TreeNode().__SetText("Didn't have library path attribute");

            throw new NotImplementedException("I still don't quite understand what this check is doing");

            //// Gets the title string to be used to find parts.
            //string requiredAttributeTitle = Model.Ucf.SingleValue("GuideBushings_GuidePins_Titles");

            //// Gets the valid value string to be used to find parts.
            //IEnumerable<string> requiredAttributeValues = Model.Ucf["GuideBushings_GuidePins_Values"];

            //Tuple<NXOpen.Part, NXOpen.NXObject.AttributeInformation>[] attributes = Model.AskAssemblyParts(nxPart)
            //    .Select(part => new Tuple<NXOpen.Part, NXOpen.NXObject.AttributeInformation[]>(part, BaseAttAttribute.AskTitlesValuesAttributes(part, new[] { requiredAttributeTitle },
            //        requiredAttributeValues, AttributeCheckType.Contain).ToArray()))
            //    .Where(tuple => tuple.Item2.Length > 0)
            //    .Select((tuple, i) => new Tuple<NXOpen.Part, NXOpen.NXObject.AttributeInformation[]>(tuple.Item1,
            //        BaseAttAttribute.AskTitleAttributes(tuple.Item1, new[] { "MATERIAL" }).ToArray()))
            //    .Where((tuple, i) => tuple.Item2.Length == 1)
            //    .Select((tuple, i) => new Tuple<NXOpen.Part, NXOpen.NXObject.AttributeInformation>(tuple.Item1, tuple.Item2[0]))
            //    .ToArray();

            //// If this is true then the current displayed assembly doesn't contain an parts with the valid title \ values pairings.
            //if (attributes.Length == 0)
            //    // Yield an "ignored" result.
            //    return new[] { new DesignCheckResult(Result.Ignore, __display_part_, this, new ObjectNode("Assembly didn't contain any valid Guide pairings.")) };

            //string tempValue = attributes[0].Item2.StringValue.ToLower();
            //if (attributes.All(tuple => tuple.Item2.StringValue.ToLower() == tempValue))
            //    return new[] { new DesignCheckResult(true, __display_part_, this) };
            //List<ObjectNode> errorNodes = new List<ObjectNode>();

            //foreach (Tuple<NXOpen.Part, NXOpen.NXObject.AttributeInformation> tuple in attributes)
            //{
            //    PartNode owningPartAttribute = new PartNode(tuple.Item1);
            //    AttributeNode objectAttribute = new AttributeNode(tuple.Item2);
            //    objectAttribute.Add($"Value: {tuple.Item2.StringValue}");
            //    owningPartAttribute.Add(objectAttribute);
            //    errorNodes.Add(owningPartAttribute);
            //}

            //return new List<DesignCheckResult>
            //{
            //    new DesignCheckResult(false, __display_part_, this, errorNodes)
            //};
        }
    }
}