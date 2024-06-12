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
    public class MirrorEdgeBoundaryRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBoundary;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part mirroredPart = mirroredComp.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            ((EdgeBoundaryRule)originalRule).GetData(out var facesOfFeatures);
            IList<Face> source = (from originalFace in facesOfFeatures
                                  select (from originalEdge in originalFace.GetEdges()
                                          let finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp)
                                          let finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp)
                                          select new Tuple<Point3d, Point3d>(finalStart, finalEnd)).ToList() into edgePoints
                                  from mirrorBody in mirroredPart.Bodies.ToArray()
                                  from mirrorFace in mirrorBody.GetFaces()
                                  where BaseMirrorRule.EdgePointsMatchFace(mirrorFace, edgePoints)
                                  select mirrorFace).ToList();
            return mirroredPart.ScRuleFactory.CreateRuleEdgeBoundary(source.ToArray());
        }
    }




}