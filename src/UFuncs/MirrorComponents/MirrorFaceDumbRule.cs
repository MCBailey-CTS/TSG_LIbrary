﻿using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorFaceDumbRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceDumb;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];

            Part mirroredPart = mirroredComp.__Prototype();

            Feature mirroredFeature = (Feature)dict[originalFeature];

            mirroredFeature.Suppress();

            ((FaceDumbRule)originalRule).GetData(out Face[] originalFaces);

            IList<Face> newFaces = (from originalFace in originalFaces
                select (
                    from originalEdge in originalFace.GetEdges()
                    let finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp)
                    let finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp)
                    select new Tuple<Point3d, Point3d>(finalStart, finalEnd)).ToList()
                into edgePoints
                from mirrorBody in mirroredPart.Bodies.ToArray()
                from mirrorFace in mirrorBody.GetFaces()
                where EdgePointsMatchFace(mirrorFace, edgePoints)
                select mirrorFace).ToList();

            return mirroredPart.ScRuleFactory.CreateRuleFaceDumb(newFaces.ToArray());
        }
    }
}