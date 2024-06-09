using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.UFuncUtilities.DesignCheckUtilities
{
    [Obsolete]
    public class SuppressedFeatures : IDesignCheck
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

        public IEnumerable<Feature> AskNXObjects(Part part)
        {
            var featureNames = new HashSet<string>();

            //var featuresToIgnore = new HashSet<NXOpen.Features.Feature>();

            foreach (var feature in part.Features.GetFeatures())
            {
                if(!featureNames.Add(feature.GetFeatureName()))
                    continue;

                yield return feature;
            }
        }

        public string AskString(Feature nxobject)
        {
            return nxobject.GetFeatureName();
        }

        [Obsolete]
        public TreeNode PerformCheck(Part part)
        {
            throw new NotImplementedException();
            //// If the nxobject is not suppresseds then we can pass.
            //if (!nxobject.Suppressed)
            //    return Tuple.Create(Result.Pass, (IEnumerable<string>)new string[] { "Feature is not suppressed." });

            //// Check to see if the feature is suppressed by expression,
            //// if is, then we can pass
            //TheUFSession.Modl.AskSuppressExpTag(nxobject.Tag, out NXOpen.Tag expTag);

            //if (expTag != NXOpen.Tag.Null)
            //    return Tuple.Create(Result.Pass, (IEnumerable<string>)new string[] { "Feature suppressed by expression." });

            //// If we get here, then the feature is supprssed and not by expression.
            //// therefore we need to check if the any of its parent features are suppressed by expression.
            //// If there is then we can ignore this feature.
            //foreach (NXOpen.Features.Feature parentFeature in nxobject.GetParents())
            //{
            //    TheUFSession.Modl.AskSuppressExpTag(parentFeature.Tag, out NXOpen.Tag parentExpTag);

            //    if (parentExpTag != NXOpen.Tag.Null)
            //        return Tuple.Create(Result.Ignore, (IEnumerable<string>)new string[] { "Parent feature is suppressed by expression." });
            //}


            //// This means the user manually suppressed it which may cause false errors in other checks.
            //return Tuple.Create(Result.Fail, (IEnumerable<string>)new[] { "Feature is suppressed, but not by expression." });
        }
    }
}