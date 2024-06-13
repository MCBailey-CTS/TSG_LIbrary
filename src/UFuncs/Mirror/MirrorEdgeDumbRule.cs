using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorEdgeDumbRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeDumb;


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
            ((EdgeDumbRule)originalRule).GetData(out var edges);
            feature.Suppress();
            originalFeature.Suppress();
            IList<Edge> list = new List<Edge>();
            Edge[] array = edges;
            foreach (Edge edge in array)
            {
                Point3d val = edge.__StartPoint().__MirrorMap(plane, originalComp, component);
                Point3d val2 = edge.__EndPoint().__MirrorMap(plane, originalComp, component);
                part.Curves.CreateLine(val, val2);
                Body[] array2 = part.Bodies.ToArray();
                foreach (Body body in array2)
                {
                    Edge[] edges2 = body.GetEdges();
                    foreach (Edge edge2 in edges2)
                    {
                        if (edge2.__HasEndPoints(val, val2))
                        {
                            list.Add(edge2);
                        }
                    }
                }
            }

            feature.Unsuppress();
            originalFeature.Unsuppress();
            return part.ScRuleFactory.CreateRuleEdgeDumb(list.ToArray());
        }
    }




}