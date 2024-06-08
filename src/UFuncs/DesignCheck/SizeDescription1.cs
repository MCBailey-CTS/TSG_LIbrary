using System;
using System.Linq;
using System.Text.RegularExpressions;
using NXOpen;
using static TSG_Library.Extensions;

namespace TSG_Library.Utilities
{
    public class SizeDescription1
    {
        /// <summary>The Regular Expression pattern to match the description attribute values to 3 values.</summary>
        private const string _regex3XPattern =
            @"([0-9]*\.[0-9]{2,3}).*\s*X\s*([0-9]*\.[0-9]{2,3}).*\s*X\s*([0-9]*\.[0-9]{2,3}).*";

        /// <summary>The Regular Expression pattern to match the description attribute value to the find the height value of the block.</summary>
        //private readonly string _regexBurnPattern = Model.Ucf.SingleValue("SIZE_DESCRIPTION_BURN_REGEX");

        /// <summary>The Regular Expression pattern to match decimals/doubles.</summary>
        private static double _tolerance;

        public static bool Validate(Part part, out string message1)
        {
            try
            {
                if(!part.HasUserAttribute("DESCRIPTION", NXObject.AttributeType.String, -1))
                {
                    message1 = "Did not have a description attribute";
                    return true;
                }

                var descriptionAttributeValue = part.GetStringUserAttribute("DESCRIPTION", -1);

                var tempString = $"Actual Description Value : \"{descriptionAttributeValue}\"";

                _tolerance = part.PartUnits == BasePart.Units.Millimeters
                    ? .0254
                    : .001;

                var descriptionMatch = Regex.Match(descriptionAttributeValue, _regex3XPattern, RegexOptions.IgnoreCase);

                if(!CheckDescriptionMatch(descriptionMatch, out var message))
                {
                    message1 = message;
                    return true;
                }

                var solidBodyLayer1 = part.__SolidBodyLayer1OrNull();

                if(solidBodyLayer1 is null)
                {
                    message1 = "Part doesn't have a single solid body on layer 1";
                    return false;
                }

                var __part = part;

                if(!__part.__HasDynamicBlock())
                {
                    message1 = "Part does not contain a Dynamic Block.";
                    return false;
                }

                // Get the dynamic block of "part".
                var dynamicBlock = part.__DynamicBlock();


                var nMatrix = part.NXMatrices.Create(dynamicBlock.__Orientation());

                var origin = dynamicBlock.__Origin().__ToArray();

                ufsession_.Csys.CreateTempCsys(origin, nMatrix.Tag, out var tagCsys);
                var minCorner = new double[3];
                var directions = new double[3, 3];
                var distances = new double[3];
                var grindDistances = new double[3];

                ufsession_.Modl.AskBoundingBoxExact(solidBodyLayer1.Tag, tagCsys, minCorner, directions, distances);

                double addX, addY, addZ;

                try
                {
                    addX = part.__FindExpressionOrNull("AddX")?.Value ?? .000;
                }
                catch (NXException nxException) when (nxException.ErrorCode == 3270012)
                {
                    message1 = "Invalid AddX expression";

                    return false;
                }

                try
                {
                    addY = part.__FindExpressionOrNull("AddY")?.Value ?? .000;
                }
                catch (NXException nxException) when (nxException.ErrorCode == 3270012)
                {
                    message1 = "Invalid AddY expression";

                    return false;
                }

                try
                {
                    addZ = part.__FindExpressionOrNull("AddZ")?.Value ?? .000;
                }
                catch (NXException nxException) when (nxException.ErrorCode == 3270012)
                {
                    message1 = "Invalid AddZ expression";

                    return false;
                }

                var boundingArray = new[]
                {
                    System.Math.Round(AskSteelSize(addX + distances[0], part), 2, MidpointRounding.AwayFromZero),
                    System.Math.Round(AskSteelSize(addY + distances[1], part), 2, MidpointRounding.AwayFromZero),
                    System.Math.Round(AskSteelSize(addZ + distances[2], part), 2, MidpointRounding.AwayFromZero)
                }.OrderBy(d => d).ToArray();

                // The actual array of values that are contained within the 
                // value of the Description attribute within in this current "part".
                // *Note the values are ordered by ascending. They are not in the order of X, Y, Z.
                // This may change in the future.
                var descriptionArray = new[]
                {
                    double.Parse(descriptionMatch.Groups[1].Value),
                    double.Parse(descriptionMatch.Groups[2].Value),
                    double.Parse(descriptionMatch.Groups[3].Value)
                }.OrderBy(d => d).ToArray();

                for (var i = 0; i < 3; i++)
                {
                    var descValue = descriptionArray[i];
                    var boundValue = boundingArray[i];
                    var resultingValue = descValue - boundValue;
                    var absValue = System.Math.Abs(resultingValue);
                    if(absValue < _tolerance) continue;
                    var nodeText =
                        $"Value mismatch, Description: {descriptionArray[i]:f2}, Actual: {boundingArray[i]:f2}";
                    var stemp = $"{nodeText}\n{tempString}";
                    message1 =
                        $"{stemp}\nBody Bounding Box: {boundingArray[0]:0.0000} X {boundingArray[1]:0.0000} X {boundingArray[2]:0.0000}";
                    return false;
                    //errorNodes.Add(new ObjectNode(nodeText));
                    //errorNodes.Add(new ObjectNode(tempString));
                    //errorNodes.Add(new ObjectNode());
                }

                message1 = "Valid";

                return true;
            }
            catch (Exception ex)
            {
                ex.__PrintException();

                message1 = "Not valid";

                return false;
            }
        }

        private static bool CheckDescriptionMatch(Match descriptionMatch, out string message)
        {
            if(!descriptionMatch.Success)
            {
                message = "Description attribute did not match regular expression.";
                return false;
            }

            if(descriptionMatch.Groups.Count < 4)
            {
                message = "Description attribute matched less than the required group set.";
                return false;
            }

            if(descriptionMatch.Groups.Count > 4)
            {
                message = "Description attribute matched more than the required group set.";
                return false;
            }

            message = "Match passed.";
            return true;
        }

        private static double AskSteelSize(double distance, BasePart part)
        {
            var roundValue = System.Math.Round(distance, 3);
            var truncateValue = System.Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;

            // If it doesn't seem to be working you might have any issue with metric vs english,
            // or you can revert the code back to the original line before you changed to float-point comparison.
            if(!(System.Math.Abs(fractionValue) > _tolerance))
                return roundValue;

            for (var ii = .125; ii <= 1; ii += .125)
                if(fractionValue <= ii)
                {
                    var roundedFraction = ii;
                    var finalValue = truncateValue + roundedFraction;
                    return finalValue;
                }

            throw new Exception($"Ask Steel Size, Part: {part.Leaf}. {nameof(distance)}: {distance}");
        }
    }
}