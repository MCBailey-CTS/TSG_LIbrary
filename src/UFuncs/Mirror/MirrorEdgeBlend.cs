using System;
using System.Collections.Generic;
using System.Linq;
using CTS_Library;
using CTS_Library.Extensions;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorEdgeBlend : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "BLEND";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane, Component originalComp)
        {
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Part part2 = originalComp._Prototype();
            EdgeBlend edgeBlend = (EdgeBlend)dict[originalFeature];
            IDictionary<int, Tuple<SelectionIntentRule[], Expression>> dictionary = new Dictionary<int, Tuple<SelectionIntentRule[], Expression>>();
            EdgeBlendBuilder edgeBlendBuilder = part2.Features.CreateEdgeBlendBuilder(originalFeature);
            using (new Destroyer(edgeBlendBuilder))
            {
                int numberOfValidChainsets = edgeBlendBuilder.GetNumberOfValidChainsets();
                for (int i = 0; i < numberOfValidChainsets; i++)
                {
                    edgeBlendBuilder.GetChainset(i, out var collector, out var radius);
                    collector.GetRules(out var rules);
                    IList<SelectionIntentRule> source = rules.Select((SelectionIntentRule originalRule) => BaseMirrorRule.MirrorRule(originalRule, originalFeature, plane, originalComp, dict)).ToList();
                    dictionary.Add(i, Tuple.Create(source.ToArray(), radius));
                }
            }

            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Fine");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(edgeBlend, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                EdgeBlendBuilder edgeBlendBuilder2 = Globals._WorkPart.Features.CreateEdgeBlendBuilder(edgeBlend);
                using (new Destroyer(edgeBlendBuilder2))
                {
                    for (int j = 0; j < edgeBlendBuilder2.GetNumberOfValidChainsets(); j++)
                    {
                        edgeBlendBuilder2.GetChainsetAndStatus(j, out var collector2, out var _, out var _);
                        SelectionIntentRule[] item = dictionary[j].Item1;
                        collector2.ReplaceRules(item, createRulesWoUpdate: false);
                    }

                    edgeBlendBuilder2.CommitFeature();
                }
            }

            originalFeature.Unsuppress();
            edgeBlend.Unsuppress();
        }
    }



}