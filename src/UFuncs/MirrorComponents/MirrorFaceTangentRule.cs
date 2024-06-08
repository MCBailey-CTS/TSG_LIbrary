using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorFaceTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceTangent;

        public override SelectionIntentRule Mirror(
            SelectionIntentRule originalRule,
            Feature originalFeature,
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            var mirroredComp = (Component)dict[originalComp];

            var mirroredPart = mirroredComp.__Prototype();

            var mirroredFeature = (Feature)dict[originalFeature];

            mirroredFeature.Suppress();

#pragma warning disable 618
            ((FaceTangentRule)originalRule).GetData(out var originalStartFace, out var originalEndFace, out var _,
                out var _, out var _);
#pragma warning restore 618

            IList<Tuple<Point3d, Point3d>> expectedStartFaceEdgePoints = (from edge in originalStartFace.GetEdges()
                let mirrorStart = edge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp)
                let mirrorEnd = edge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp)
                select Tuple.Create(mirrorStart, mirrorEnd)).ToList();

            IList<Tuple<Point3d, Point3d>> expectedEndFaceEdgePoints = (from edge in originalEndFace.GetEdges()
                let mirrorStart = edge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp)
                let mirrorEnd = edge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp)
                select Tuple.Create(mirrorStart, mirrorEnd)).ToList();

            Face mirrorStartFace = null;

            Face mirrorEndFace = null;

            var originalOwningFeatureOfStartFace =
                originalStartFace.__OwningPart().Features.GetParentFeatureOfFace(originalStartFace);

            var mirrorOwningFeatureOfStartFace = (BodyFeature)dict[originalOwningFeatureOfStartFace];

            foreach (var body in mirrorOwningFeatureOfStartFace.GetBodies())
            {
                if(!(mirrorStartFace is null) && !(mirrorEndFace is null))
                    break;

                foreach (var face in body.GetFaces())
                {
                    if(mirrorStartFace is null && EdgePointsMatchFace(face, expectedStartFaceEdgePoints))
                    {
                        mirrorStartFace = face;

                        continue;
                    }

                    if(!(mirrorEndFace is null) || !EdgePointsMatchFace(face, expectedEndFaceEdgePoints))
                        continue;

                    mirrorEndFace = face;
                }
            }

            if(mirrorStartFace is null)
                throw new ArgumentException("Unable to find start face");

            if(mirrorEndFace is null)
                throw new ArgumentException("Unable to find end face");

            return mirroredPart.ScRuleFactory.CreateRuleFaceTangent(mirrorStartFace, new[] { mirrorEndFace });
        }
    }
}