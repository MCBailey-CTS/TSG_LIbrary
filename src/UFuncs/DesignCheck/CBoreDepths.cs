using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using NXOpen.Utilities;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class CBoreDepths : IDesignCheck
    {
        //public static double? EnglishDepths(int diamter, int length)
        //{
        //    switch(diamter)
        //    {
        //        case 4:
        //        case 6:
        //        case 8:
        //        case 10:
        //        case 250:
        //        case 313:
        //        case 375:
        //        case 0500:
        //        case 0625:
        //        case 0750:
        //        case 0875:
        //        case 1000:
        //        case 1250:
        //        case 1500:
        //            switch (length)
        //            {
        //                case 025:
        //                case 037:
        //                case 050:

        //            }

        //            break;


        //    }
        //}

        //IDictionary<int, IDictionary<int, double[]>>  = new Dictionary<>

        //public const string  = "";
        //          // These are in inches.
        //          :CBoreDepth_English:
        //          NA	0004	0006	0008	0010	0250	0313	0375	0500	0625	0750	0875	1000	1250	1500
        //          025	NA	0.043	NA	NA	NA	NA	NA	NA	NA	NA	NA	NA	NA	NA
        //          037	0.112	0.138	0.129	0.09	NA	NA	NA	NA	NA	NA	NA	NA	NA	NA
        //          050	NA	0.138	0.164	0.19	0.125	0.031	NA	NA	NA	NA	NA	NA	NA	NA
        //          062	NA	0.138	0.164	0.19	0.25	0.156	0.062	NA	NA	NA	NA	NA	NA	NA
        //          075	NA	0.138	0.164	0.19	0.25	0.313	0.188	NA	NA	NA	NA	NA	NA	NA
        //          100	NA	0.138	0.164	0.19	0.25	0.313	0.375	0.25	0.063	NA	NA	NA	NA	NA
        //          125	NA	0.138	0.164	0.19	0.25	0.313	0.375	0.5	0.313	0.125	NA	NA	NA	NA
        //          150	NA	0.138	0.164	0.19	0.25	0.313	0.375	0.5	0.625	0.375	0.1875	NA	NA	NA
        //          175	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.625	0.4375	.25	NA	NA
        //          200	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.75	0.6875	.5	0.125	NA
        //          225	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.75	0.875	.75	0.375	NA
        //          250	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	0.625	0.25
        //          275	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	0.875	0.5
        //          300	NA	NA	NA	0.19	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.125	0.75
        //          325	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.0
        //          350	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.25
        //          375	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          400	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          450	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          500	NA	NA	NA	NA	0.25	0.313	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          550	NA	NA	NA	NA	NA	NA	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          600	NA	NA	NA	NA	NA	NA	0.375	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          650	NA	NA	NA	NA	NA	NA	NA	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          700	NA	NA	NA	NA	NA	NA	NA	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          750	NA	NA	NA	NA	NA	NA	NA	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          800	NA	NA	NA	NA	NA	NA	NA	0.5	0.625	0.75	0.875	1	1.25	1.5
        //          :END:


        public const string CBoreDepth_Metric = @"
            // These are in millimeters.
            ::
            NA	4	5	6	8	10	12	16	20	24	30	36
            008	2	0.5	NA	NA	NA	NA	NA	NA	NA	NA	NA
            010	4	2.5	1	NA	NA	NA	NA	NA	NA	NA	NA
            012	4	4.5	3	1.5	NA	NA	NA	NA	NA	NA	NA
            016	4	5	6	4	1.5	NA	NA	NA	NA	NA	NA
            020	4	5	6	8	5	2	NA	NA	NA	NA	NA
            025	4	5	6	8	10	7	1	NA	NA	NA	NA
            030	4	5	6	8	10	12	6	NA	NA	NA	NA
            035	4	5	6	8	10	12	11	5	NA	NA	NA
            040	NA	NA	6	8	10	12	16	10	4	NA	NA
            045	NA	NA	6	8	10	12	16	15	9	NA	NA
            050	NA	NA	6	8	10	12	16	20	14	5	NA
            055	NA	NA	6	8	10	12	16	20	19	10	NA
            060	NA	NA	6	8	10	12	16	20	24	15	6
            065	NA	NA	6	8	10	12	16	20	24	20	11
            070	NA	NA	6	NA	10	12	16	20	24	25	16
            075	NA	NA	NA	NA	10	12	16	20	24	30	21
            080	NA	NA	NA	NA	10	12	16	20	24	30	26
            090	NA	NA	NA	NA	10	12	16	20	24	30	36
            100	NA	NA	NA	NA	10	12	16	20	24	30	36
            110	NA	NA	NA	NA	10	12	16	20	24	30	36
            120	NA	NA	NA	NA	10	12	16	20	24	30	36
            130	NA	NA	NA	NA	10	12	16	20	24	30	36
            140	NA	NA	NA	NA	10	12	16	20	24	30	36
            150	NA	NA	NA	NA	10	12	16	20	24	30	36
            160	NA	NA	NA	NA	NA	NA	16	20	NA	NA	NA
            180	NA	NA	NA	NA	NA	NA	16	20	NA	NA	NA
            :END:
            ";


        /*
        :CBORE_DEPTH_IGNORE_PATHS_REGEX:
^G:\\0Library\\.*\.prt$
^W:\\.*\.prt$
:END:
        */

        /*

        :CBORE_DEPTH_COMPONENT_MATERIALS:

:END:



        */

        /*
         :CBORE_DEPTH_TITLES_1:
MATERIAL
:END:
         */

        private const string ShcsHoleChart = "SHCS_CBORE_HOLECHART";

        public static string[] CBORE_DEPTH_COMPONENT_MATERIALS =
        {
            "A2", "A2 GS", "ACUSEAL", "ALUM", "AMPCO 18", "CALDIE",
            "CARMO",
            "CRS",
            "D2",
            "DC53",
            "DIEVAR",
            "GM190/S0050A",
            "GM238/G2500",
            "GM241/G3500",
            "GM245/D4512",
            "GM246/D5506",
            "GM338/D6510",
            "GM68M/S0030",
            "S0030A",
            "HRS",
            "HRS PLT",
            "H13",
            "H13 PREMIUM",
            "O1",
            "O1 DR",
            "Ovar Superior",
            "P20",
            "S7",
            "S7 GS",
            "STEELCRAFT",
            "STRUCTURAL",
            "4140",
            "4140 PH",
            "4140 PH PLT",
            "4140 PLT",
            "6150",
            "OTHER"
        };

        /// <summary>
        ///     A Design Check to validate that CBore holes have the right clearance based off of the fastener that created the
        ///     cbore hole.
        /// </summary>
#pragma warning disable CS0649 // Field 'CBoreDepths._fastenerMatches' is never assigned to, and will always have its default value null
        private readonly Dictionary<string, Tuple<Dictionary<string, Dictionary<string, string>>, Match>>
            _fastenerMatches;
#pragma warning restore CS0649 // Field 'CBoreDepths._fastenerMatches' is never assigned to, and will always have its default value null

        [Obsolete]
        public CBoreDepths()
        {
            //Dictionary<string, Dictionary<string, string>> metricDepths = ConstructDictionary("CBoreDepth_Metric");
            //Dictionary<string, Dictionary<string, string>> englishDepths = ConstructDictionary("CBoreDepth_English");
            //Regex metricShcsRegex = null; // new Regex(Model.Ucf.SingleValue("CBoreDepth_MetricShcsRegex"));
            //Regex englishShcsRegex = null; // new Regex(Model.Ucf.SingleValue("CBoreDepth_EnglishShcsRegex"));
            //_fastenerMatches = new Dictionary<string, Tuple<Dictionary<string, Dictionary<string, string>>, Match>>();


            //foreach (NXOpen.Assemblies.Component component in Model.AssemblyComponents)
            //{
            //    if (_fastenerMatches.ContainsKey(component.DisplayName)) 
            //        continue;

            //    if (!component.DisplayName.Contains("shcs")) 
            //        continue;

            //    Match metricMatch = metricShcsRegex.Match(component.DisplayName);

            //    if (metricMatch.Success)
            //    {
            //        _fastenerMatches[component.DisplayName] =
            //            new Tuple<Dictionary<string, Dictionary<string, string>>, Match>(metricDepths, metricMatch);
            //        continue;
            //    }

            //    Match englishMatch = englishShcsRegex.Match(component.DisplayName);
            //    if (englishMatch.Success)
            //        _fastenerMatches[component.DisplayName] =
            //            new Tuple<Dictionary<string, Dictionary<string, string>>, Match>(englishDepths, englishMatch);
            //}

            HashSet<ExtractFace> hashedLinkedBodies = new HashSet<ExtractFace>();

            //foreach (NXOpen.Assemblies.Component component in Model.AssemblyComponents)
            //{
            //    if (!(component.Prototype is NXOpen.Part part))
            //        continue;

            //    foreach (NXOpen.Features.Feature feature in part.Features.ToArray())
            //    {
            //        if (!(feature is NXOpen.Features.ExtractFace extractFace)) 
            //            continue;

            //        if (!extractFace._IsLinkedBody()) 
            //            continue;

            //        if (extractFace.GetFaces().FirstOrDefault(face => face.Name == ShcsHoleChart) == null) 
            //            continue;

            //        hashedLinkedBodies.Add(extractFace);
            //    }
            //}
        }

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
        private static Dictionary<string, Dictionary<string, string>> ConstructDictionary(string delimeter)
        {
            throw new NotImplementedException();
#pragma warning disable CS0162 // Unreachable code detected
            string[] lines = null; // Model.Ucf[delimeter]; //     GetValuesFromFile(delimeter);
#pragma warning restore CS0162 // Unreachable code detected
            Dictionary<string, Dictionary<string, string>> dictionary =
                new Dictionary<string, Dictionary<string, string>>();
            string[] firstLineSplit = lines[0].Split('\t');
            // We want to set the length - 1 because the first element in the first row is "NA".
            string[] tempKeyHolder = new string[firstLineSplit.Length];
            // We want to skip the [0]th index because it is "NA".
            for (int i = 1; i < firstLineSplit.Length; i++)
            {
                tempKeyHolder[i] = firstLineSplit[i];
                dictionary[firstLineSplit[i]] = new Dictionary<string, string>();
            }

            // We want to skip the [0]th row because we used that as the keys for the dictionary. 
            for (int rowIndex = 1; rowIndex < lines.Length; rowIndex++)
            {
                string[] split = lines[rowIndex].Split('\t');
                for (int colindex = 1; colindex < dictionary.Count + 1; colindex++)
                {
                    string element = split[colindex];
                    if (!double.TryParse(element, out double unused)) continue;
                    string diameterKey = tempKeyHolder[colindex];
                    Dictionary<string, string> diameterDictionary = dictionary[diameterKey];
                    diameterDictionary.Add(split[0], element);
                }
            }

            return dictionary;
        }


        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();

            //throw new NotImplementedException();
            //List<ObjectNode> errorNodes = new List<ObjectNode>();
#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool allFacesHavePassed = true;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning restore CS0162 // Unreachable code detected

            // Gets all the faces in the parts layer 1 body that is named "SHCS_CBORE_HOLECHART.
            Face[] validFaces = part.Bodies.OfType<Body>()
                .SelectMany(body => body.GetFaces())
                .Where(face => face.Name == ShcsHoleChart)
                .ToArray();

            //if (validFaces.Length == 0)
            //{
            //    yield return new DesignCheckResult(Result.Ignore, part, this,
            //        new ObjectNode("Part did not contain any valid faces."));
            //    yield break;
            //}

            Dictionary<Face, string> dictionary = new Dictionary<Face, string>();

            foreach (Face face in validFaces)
            {
                ufsession_.Modl.AskFaceFeats(face.Tag, out Tag[] features);

                ExtractFace[] extractFaceFeatures = features.Select(NXObjectManager.Get)
                    .OfType<ExtractFace>()
                    .ToArray();

                if (extractFaceFeatures.Length != 1)
                {
                    //InfoWindow.WriteLine();
                    //errorNodes.Add(new ObjectNode("Found invalid face"));
                    //continue;
                }

                ufsession_.Wave.AskLinkedFeatureGeom(extractFaceFeatures[0].Tag, out Tag linkedGeom);
                ufsession_.Wave.AskLinkedFeatureInfo(linkedGeom, out UFWave.LinkedFeatureInfo linkedFeatureInfo);

                if (linkedFeatureInfo.source_part_name == null ||
                    !linkedFeatureInfo.source_part_name.__IsShcs()) continue;
                dictionary.Add(face, linkedFeatureInfo.source_part_name);
            }

            bool partIsMetric = part.PartUnits == BasePart.Units.Millimeters;

            //double tolerance = double.Parse(partIsMetric
            //    ? Model.Ucf.SingleValue("Metric_Tolerance")
            //    : Model.Ucf.SingleValue("English_Tolerance"));

            foreach (KeyValuePair<Face, string> pair in dictionary)
            {
                //if (!_fastenerMatches.ContainsKey(pair.Value))
                //{
                //    errorNodes.Add(
                //        new ObjectNode($"Could not find key for Part: {part.Leaf}, Value: {pair.Value}"));
                //    continue;
                //}

                Tuple<Dictionary<string, Dictionary<string, string>>, Match> tuple = _fastenerMatches[pair.Value];
                string diameter = tuple.Item2.Groups["Diameter"].Value;
                string length = tuple.Item2.Groups["Length"].Value;
                double expectedDepth = double.Parse(tuple.Item1[diameter][length]);
                if (expectedDepth < 0)
                    throw new InvalidOperationException("Depth was less than 0");
                bool shcsIsMetric = pair.Value.Contains("mm");

                if (!shcsIsMetric && partIsMetric)
                    expectedDepth = expectedDepth * 25.4;

                if (shcsIsMetric && !partIsMetric)
                    expectedDepth = expectedDepth / 25.4;

                if (pair.Key.GetEdges().Length != 2)
                {
                    //ObjectNode objNode = new ObjectNode(pair.Value);
                    //objNode.Add("HoleChart face had more or less than 2 edges.");
                    //errorNodes.Add(objNode);
                    //allFacesHavePassed = false;
                    //continue;
                }

                Vector3d binormal = pair.Key.GetEdges()[0].__Binormal(0);
                // Check that both edges only have 3 different faces associated with them.
                // And that only one of them is cylindrical and the other two are planar.
                Face[] associatedFaces = pair.Key.GetEdges()
                    .SelectMany(edge => edge.GetFaces())
                    .Distinct()
                    .ToArray();

                // We only want three faces.
                if (associatedFaces.Length != 3)
                {
                    //ObjectNode objNode = new ObjectNode(pair.Value);
                    //objNode.Add("HoleChart face had more or less than 3 faces.");
                    //errorNodes.Add(objNode);
                    //allFacesHavePassed = false;
                    //continue;
                }

                Face[] planarFaces = associatedFaces
                    .Where(face => face.SolidFaceType == Face.FaceType.Planar)
                    .ToArray();

                //if (!binormal._IsParallelTo(planarFaces[0]._NormalVector()) ||
                //    !binormal._IsParallelTo(planarFaces[1]._NormalVector()))
                //{
                //    //ObjectNode objNode = new ObjectNode(pair.Value);
                //    //objNode.Add("HoleChart face did not have two perpendicular faces..");
                //    //errorNodes.Add(objNode);
                //    //allFacesHavePassed = false;
                //    continue;
                //}

                //double actualDepth = Snap.Compute.Distance(planarFaces[0], planarFaces[1]);

                //if (actualDepth > expectedDepth || Math.Abs(actualDepth - expectedDepth) < tolerance)
                //    continue;

                //ObjectNode expectedDepthNode = new ObjectNode(pair.Value);
                //expectedDepthNode.Add($"Minimum depth: {expectedDepth:0.000} {(partIsMetric ? "mm" : "in")}");
                //expectedDepthNode.Add($" Actual depth: {actualDepth:0.000} {(partIsMetric ? "mm" : "in")}");
                //errorNodes.Add(expectedDepthNode);
                //allFacesHavePassed = false;
            }


            //yield return new DesignCheckResult(allFacesHavePassed, part, this, errorNodes);
        }
    }
}