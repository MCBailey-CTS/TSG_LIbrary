using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class SizeDescription : IDesignCheck
    {
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

        /*

:SIZE_DESCRIPTION_REGEX:
([0-9]*\.[0-9]{2,3}).*\s*X\s*([0-9]*\.[0-9]{2,3}).*\s*X\s*([0-9]*\.[0-9]{2,3}).*
:END:

:SIZE_DESCRIPTION_BURN_REGEX:
//^Burn\s*(?<BurnHeight>\d+\.?\d*)
^Burn\s*(?<BurnHeight>(\d+\.\d+)|(\d+\.)|(\.\d+))
:END:


        */
        /// <inheritdoc />
        /// <summary>
        /// A Design Check the checks to make sure that every part that has a Dynamic Block has a Description Attribute that matches the Length, Width, and Height 
        /// </summary>
        //[IgnoreRegex("SIZE_DESCRIPTION_IGNORE_PATHS_REGEX")]
        //[TitleRegex("SIZE_TITLES", "DESCRIPTION_IGNORE_VALUES")]
        //[Values("SIZE_DESCRIPTION_TITLES_1", "CBORE_DEPTH_COMPONENT_MATERIALS", AttributeCheckType.Match)]
        //[Titles("SIZE_TITLES", CheckType.DesignCheck)]
        //[SetWorkPart]
        //[IgnoreComponentOf999]
        //[RequireDynamicBlock(CheckType.DesignCheck)]
        /// <summary>The Regular Expression pattern to match the description attribute values to 3 values.</summary>
        //private readonly string _regex3XPattern = Model.Ucf.SingleValue("SIZE_DESCRIPTION_REGEX");

        /// <summary>The Regular Expression pattern to match the description attribute value to the find the height value of the block.</summary>
        //private readonly string _regexBurnPattern = Model.Ucf.SingleValue("SIZE_DESCRIPTION_BURN_REGEX");

        /// <summary>The Regular Expression pattern to match decimals/doubles.</summary>
        //private readonly string _regexDoublePattern = Model.Ucf.SingleValue("DOUBLE_DECIMAL_REQUIRE_PERIOD_REGEX");
        [Obsolete]
        private bool CheckAutoUpdate(Part part, string descriptionAttributeValue /*, out DesignCheckResult result*/)
        {
            //result = new DesignCheckResult(Result.Pass, part, this, "");

            //if (!part.HasUserAttribute("AUTO UPDATE", NXOpen. NXObject.AttributeType.String, -1))
            //   return true;

            //string attValue = part.__GetAttribute("AUTO UPDATE").ToUpper();

            //switch (attValue)
            //{
            //   case "ON":
            //       return true;
            //   case "OFF":
            //       // If this statement is true then it means that this "part"'s description attribute value doesn't contain any decimal/double values.
            //       if (!Regex.IsMatch(descriptionAttributeValue, _regexDoublePattern))
            //           result = new DesignCheckResult(Result.Ignore, part, this,
            //               "Auto Update value is set to off and Description value doesn't contain any doubles/decimals.");
            //       else
            //           result = new DesignCheckResult(Result.Fail, part, this,
            //               "Auto Update value is set to off and Description value contains doubles/decimals values.");
            //       return false;
            //   default:
            //       result = new DesignCheckResult(Result.Fail, part, this,
            //           $"Auto Update value is invalid: {attValue}.");
            //       return false;
            //}
            throw new NotImplementedException();
        }

        [Obsolete]
        public IEnumerable<TreeNode> Validate(Part part)
        {
            throw new NotImplementedException();
            //string descriptionAttributeValue = part.GetStringUserAttribute(Model.Ucf.SingleValue("SIZE_TITLES"), -1);

            //if (!CheckAutoUpdate(part, descriptionAttributeValue, out DesignCheckResult autoUpdateResult))
            //{
            //    yield return autoUpdateResult;
            //    yield break;
            //}

            //string tempString = $"Actual Description Value : \"{descriptionAttributeValue}\"";

            //_tolerance = part.PartUnits == BasePart.Units.Millimeters
            //    ? Model.MetricTolerance
            //    : Model.EnglishTolerance;

            //Match burnMatch = Regex.Match(descriptionAttributeValue, _regexBurnPattern, RegexOptions.IgnoreCase);

            //if (burnMatch.Success)
            //{
            //    // If it matches then we want to check this part using the Burnout method instead of the normal way.
            //    foreach (DesignCheckResult burnoutResult in CheckBurnout(burnMatch, part))
            //        yield return burnoutResult;
            //    yield break;
            //}

            //Match descriptionMatch = Regex.Match(descriptionAttributeValue, _regex3XPattern, RegexOptions.IgnoreCase);

            //if (!CheckDescriptionMatch(descriptionMatch, out string message))
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this, message, tempString);
            //    yield break;
            //}

            //Body solidBodyLayer1 = part._SolidBodyLayer1OrNull();

            //if (solidBodyLayer1 is null)
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this, "Part doesn't have a single solid body on layer 1");
            //    yield break;
            //}

            //// Get the dynamic block of "part".
            //Block dynamicBlock = part._DynamicBlockOrNull();

            //// If "part" does not contain a DYNAMIC BLOCK, then we want to fail the "part".
            //if (dynamicBlock == null)
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this, "Part does not contain a Dynamic Block.");
            //    yield break;
            //}

            //NXMatrix nMatrix = part.NXMatrices.Create(dynamicBlock._Orientation());
            //TheUFSession.Csys.CreateTempCsys(dynamicBlock._Origin()._ToArray(), nMatrix.Tag, out Tag tagCsys);
            //double[] minCorner = new double[3];
            //double[,] directions = new double[3, 3];
            //double[] distances = new double[3];
            //double[] grindDistances = new double[3];

            //TheUFSession.Modl.AskBoundingBoxExact(solidBodyLayer1.Tag, tagCsys, minCorner, directions, distances);

            //double addX = part._FindExpressionOrNull("AddX")?.Value ?? .000;
            //double addY = part._FindExpressionOrNull("AddY")?.Value ?? .000;
            //double addZ = part._FindExpressionOrNull("AddZ")?.Value ?? .000;

            //double[] boundingArray = new[]
            //{
            //  System.Math.Round(AskSteelSize(addX + distances[0], part), 2, MidpointRounding.AwayFromZero),
            //  System.Math.Round(AskSteelSize(addY + distances[1], part), 2, MidpointRounding.AwayFromZero),
            //  System.Math.Round(AskSteelSize(addZ + distances[2], part), 2, MidpointRounding.AwayFromZero),
            //}.OrderBy(d => d).ToArray();

            //// The actual array of values that are contained within the 
            //// value of the Description attribute within in this current "part".
            //// *Note the values are ordered by ascending. They are not in the order of X, Y, Z.
            //// This may change in the future.
            //double[] descriptionArray = new[]
            //{
            //    double.Parse(descriptionMatch.Groups[1].Value),
            //    double.Parse(descriptionMatch.Groups[2].Value),
            //    double.Parse(descriptionMatch.Groups[3].Value),
            //}.OrderBy(d => d).ToArray();

            //List<ObjectNode> errorNodes = new List<ObjectNode>();

            //for (int i = 0; i < 3; i++)
            //{
            //    double descValue = descriptionArray[i];
            //    double boundValue = boundingArray[i];
            //    double resultingValue = descValue - boundValue;
            //    double absValue = Math.Abs(resultingValue);
            //    if (absValue < _tolerance) continue;
            //    string nodeText = $"Value mismatch, Description: {descriptionArray[i]:f2}, Actual: {boundingArray[i]:f2}";
            //    errorNodes.Add(new ObjectNode(nodeText));
            //    errorNodes.Add(new ObjectNode(tempString));
            //    errorNodes.Add(new ObjectNode($"Body Bounding Box: {boundingArray[0]:0.0000} X {boundingArray[1]:0.0000} X {boundingArray[2]:0.0000}"));
            //}

            //yield return new DesignCheckResult(errorNodes.Count == 0, part, this, errorNodes);
        }

        private static bool CheckDescriptionMatch(Match descriptionMatch, out string message)
        {
            if (!descriptionMatch.Success)
            {
                message = "Description attribute did not match regular expression.";
                return false;
            }

            if (descriptionMatch.Groups.Count < 4)
            {
                message = "Description attribute matched less than the required group set.";
                return false;
            }

            if (descriptionMatch.Groups.Count > 4)
            {
                message = "Description attribute matched more than the required group set.";
                return false;
            }

            message = "Match passed.";
            return true;
        }

        /// <summary>Validates the given part using the Burnout pattern.</summary>
        /// <param name="descriptionMatch">
        ///     The match. It is assuming that this match cam from the pattern
        ///     <see cref="_regexBurnPattern" />.
        /// </param>
        /// <param name="part">The part to check.</param>
        /// <returns>The results from the validation.</returns>
        public IEnumerable<TreeNode> CheckBurnout(Match descriptionMatch, Part part)
        {
            throw new NotImplementedException();
            //// Get the dynamic block of "part".
            //Block dynamicBlock = part._DynamicBlockOrNull();

            //// If "part" does not contain a DYNAMIC BLOCK, then we want to fail the "part".
            //if (dynamicBlock == null)
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this, "Part does not contain a Dynamic Block.");
            //    yield break;
            //}

            //Expression burnoutExpression = part._FindExpressionOrNull("Burnout");

            //if (burnoutExpression == null)
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this,
            //        "Part does not contain a \"Burnout\" expression.");
            //    yield break;
            //}

            //if (burnoutExpression.StringValue != "yes")
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this,
            //        "Parts \"Burnout\" expression is set to \"no\".");
            //    yield break;
            //}

            //// todo: put this into the Ucf.
            //Expression burnDirExpression = part._FindExpressionOrNull("BurnDir");

            //if (burnDirExpression == null)
            //{
            //    yield return new DesignCheckResult(Result.Fail, part, this,
            //        "Part does not contain a \"BurnDir\" expression.");
            //    yield break;
            //}

            //double descriptionHeightValue = double.Parse(descriptionMatch.Groups["BurnHeight"].Value);

            //double adjustedValue = descriptionHeightValue;

            //if (Math.Abs(descriptionHeightValue - adjustedValue) < _tolerance)
            //    yield return new DesignCheckResult(true, part, this);
            //else
            //    yield return new DesignCheckResult(Result.Fail, part, this,
            //        $"Value mismatch, Description: {descriptionHeightValue:f2}, Actual: {adjustedValue:f2}");
        }

        private double AskSteelSize(double distance, BasePart part)
        {
            //double roundValue = Math.Round(distance, 3);
            //double truncateValue = Math.Truncate(roundValue);
            //double fractionValue = roundValue - truncateValue;

            //// If it doesn't seem to be working you might have any issue with metric vs english,
            //// or you can revert the code back to the original line before you changed to float-point comparison.
            //if (!(Math.Abs(fractionValue) > _tolerance))
            //    return roundValue;

            //for (double ii = .125; ii <= 1; ii += .125)
            //    if (fractionValue <= ii)
            //    {
            //        double roundedFraction = ii;
            //        double finalValue = truncateValue + roundedFraction;
            //        return finalValue;
            //    }

            //throw new Exception($"Ask Steel Size, Part: {part.Leaf}. {nameof(distance)}: {distance}");

            throw new NotImplementedException();
        }

        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
        }
    }
}