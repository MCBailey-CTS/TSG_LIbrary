using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MirrorEdgeBodyRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBody;

        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component component = (Component)dict[originalComp];
            Part part = component.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeBodyRule)originalRule).GetData(out Body body);
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

            if (body2 == null)
            {
                throw new ArgumentException("Could not find a matching body");
            }

            return part.ScRuleFactory.CreateRuleEdgeBody(body2);
        }
    }



    public class EqualityPos : IEqualityComparer<Point3d>
    {
        private const double Tolerance = 0.001;

      
        public bool Equals(Point3d x, Point3d y)
        {
            return Math.Abs(x.X - y.X) < Tolerance 
                && Math.Abs(x.Y - y.Y) < Tolerance 
                && Math.Abs(x.Z - y.Z) < Tolerance;
        }

        public int GetHashCode(Point3d obj)
        {
            unchecked // integer overflows are accepted here
            {
                int hash = 17;
                hash = hash * 23 + obj.X.GetHashCode();
                hash = hash * 23 + obj.Y.GetHashCode();
                hash = hash * 23 + obj.Z.GetHashCode();
                return hash;
            }
        }

    }

}