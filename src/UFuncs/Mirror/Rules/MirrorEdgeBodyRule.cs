using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using TSG_Library.UFuncs.MirrorComponents.Features;
using TSG_Library.Utilities;

namespace TSG_Library.UFuncs.Mirror.Rules
{
    public class MirrorEdgeBodyRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBody;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            Body body = originalRule.__ToEdgeBodyRule().__Data();
            ISet<Point3d> set = new HashSet<Point3d>(new EqualityPos());
            Edge[] edges = body.GetEdges();
            foreach (Edge edge in edges)
            {
                Point3d item = edge.__StartPoint()._MirrorMap(plane, originalComp, component);
                Point3d item2 = edge.__EndPoint()._MirrorMap(plane, originalComp, component);
                set.Add(item2);
                set.Add(item);
            }

            Body body2 = null;
            Body[] array = part.Bodies.ToArray();
            foreach (Body body3 in array)
            {
                ISet<Point3d> set2 = new HashSet<Point3d>(new EqualityPos());
                Edge[] edges2 = body3.GetEdges();
                foreach (Edge edge2 in edges2)
                {
                    set2.Add(edge2.__StartPoint());
                    set2.Add(edge2.__EndPoint());
                }

                if (set2.Count == set.Count && set2.SetEquals(set))
                {
                    body2 = body3;
                    break;
                }
            }

            if (body2 == null) throw new ArgumentException("Could not find a matching body");

            return part.ScRuleFactory.CreateRuleEdgeBody(body2);
        }
    }



}