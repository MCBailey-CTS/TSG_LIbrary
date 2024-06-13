using System;
using System.Collections.Generic;

using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorEdgeChainRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeChain;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {

            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeChainRule)originalRule).GetData(out Edge startEdge, out Edge endEdge, out var isFromStart);
            Edge edge = null;
            Edge endEdge2 = null;
            Point3d pos = startEdge.__StartPoint()._MirrorMap(plane, originalComp, component);
            Point3d pos2 = startEdge.__EndPoint()._MirrorMap(plane, originalComp, component);
            Body[] array = part.Bodies.ToArray();
            foreach (Body body in array)
            {
                Edge[] edges = body.GetEdges();
                foreach (Edge edge2 in edges)
                {
                    if (edge2.__HasEndPoints(pos, pos2))
                    {
                        edge = edge2;
                    }
                }
            }

            if (endEdge != null)
            {
                pos = endEdge.__StartPoint()._MirrorMap(plane, originalComp, component);
                pos2 = endEdge.__EndPoint()._MirrorMap(plane, originalComp, component);
                Body[] array2 = part.Bodies.ToArray();
                foreach (Body body2 in array2)
                {
                    Edge[] edges2 = body2.GetEdges();
                    foreach (Edge edge3 in edges2)
                    {
                        if (edge3.__HasEndPoints(pos, pos2))
                        {
                            endEdge2 = edge3;
                        }
                    }
                }
            }

            if (edge == null)
            {
                throw new ArgumentException("Could not find start edge");
            }

            return part.ScRuleFactory.CreateRuleEdgeChain(edge, endEdge2, isFromStart);
        }
    }




}