using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorEdgeMultipleSeedTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeMultipleSeedTangent;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface. Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeMultipleSeedTangentRule)originalRule).GetData(out var seedEdges, out var angleTolerance, out var hasSameConvexity);
            IList<Edge> list = new List<Edge>();
            Edge[] array = seedEdges;
            foreach (Edge edge in array)
            {
                Body body = edge.GetBody();
                Body body2;
                if (!dict.ContainsKey(body))
                {
                    feature.Suppress();
                    originalFeature.Suppress();
                    Feature parentFeatureOfBody = originalComp.__Prototype().Features.GetParentFeatureOfBody(body);
                    BodyFeature bodyFeature = (BodyFeature)dict[parentFeatureOfBody];
                    if (bodyFeature.GetBodies().Length != 1)
                    {
                        throw new InvalidOperationException("Invalid number of bodies for feature");
                    }

                    body2 = bodyFeature.GetBodies()[0];
                }
                else
                {
                    body2 = (Body)dict[body];
                }

                Point3d pos = edge.__StartPoint()._MirrorMap(plane, originalComp, component);
                Point3d pos2 = edge.__EndPoint()._MirrorMap(plane, originalComp, component);
                Edge[] edges = body2.GetEdges();
                foreach (Edge edge2 in edges)
                {
                    if (edge2.__HasEndPoints(pos, pos2))
                    {
                        list.Add(edge2);
                    }
                }
            }

            return part.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(list.ToArray(), angleTolerance, hasSameConvexity);
        }
    }




}