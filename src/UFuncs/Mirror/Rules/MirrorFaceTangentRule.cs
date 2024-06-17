using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using TSG_Library.UFuncs.MirrorComponents.Features;

namespace TSG_Library.UFuncs.Mirror.Rules
{
    public class MirrorFaceTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceTangent;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature,
            Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part part = mirroredComp.__Prototype();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
#pragma warning disable CS0618 // Type or member is obsolete
            ((FaceTangentRule)originalRule).GetData(out Face startFace, out Face endFace, out bool _, out double _,
                out bool _);
#pragma warning restore CS0618 // Type or member is obsolete
            IList<Tuple<Point3d, Point3d>> edgePoints = (from edge in startFace.GetEdges()
                                                         let mirrorStart = edge.__StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                         let mirrorEnd = edge.__EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                         select Tuple.Create(mirrorStart, mirrorEnd)).ToList();
            IList<Tuple<Point3d, Point3d>> edgePoints2 = (from edge in endFace.GetEdges()
                                                          let mirrorStart = edge.__StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                          let mirrorEnd = edge.__EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                          select Tuple.Create(mirrorStart, mirrorEnd)).ToList();
            Face face = null;
            Face face2 = null;
            Feature parentFeatureOfFace = startFace.__OwningPart().Features.GetParentFeatureOfFace(startFace);
            BodyFeature bodyFeature = (BodyFeature)dict[parentFeatureOfFace];
            Body[] bodies = bodyFeature.GetBodies();
            foreach (Body body in bodies)
            {
                if (face != null && face2 != null) break;

                Face[] faces = body.GetFaces();
                foreach (Face face3 in faces)
                    if (face == null && EdgePointsMatchFace(face3, edgePoints))
                        face = face3;
                    else if (face2 == null && EdgePointsMatchFace(face3, edgePoints2)) face2 = face3;
            }

            if (face == null) throw new ArgumentException("Unable to find start face");

            if (face2 == null) throw new ArgumentException("Unable to find end face");

            return part.ScRuleFactory.CreateRuleFaceTangent(face, new Face[1] { face2 });
        }
    }



}