using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities
{
    public class MirrorSmartStockEjector : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if(!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
                return false;

            // Check to see if it is a smart key metric
            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() ==
                   "SMART STOCK EJECTORS";
        }

        [Obsolete(nameof(NotImplementedException))]
        public void Mirror(
            Surface.Plane plane,
            Component mirroredComp,
            ExtractFace originalLinkedBody,
            Component fromComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            //    NXOpen.Features.ExtractFace mirroredLinkedBody = (NXOpen.Features.ExtractFace)dict[originalLinkedBody];

            //    NXOpen.Point3d mirroredOrigin = fromComp._Origin()._Mirror(plane);

            //    NXOpen.Matrix3x3 mirroredOrientation = fromComp._Orientation()._Mirror(plane);

            //    NXOpen.Vector3d newXDir = mirroredOrientation._AxisY();

            //    NXOpen.Vector3d newYDir = mirroredOrientation._AxisX()._Negate();

            //    mirroredOrientation = new NXOpen.Matrix3x3(newXDir, newYDir);

            //    NXOpen.Assemblies.Component newFromComp = fromComp.__OwningPart().ComponentAssembly.AddComponent(
            //        fromComp.__Prototype(),
            //        "Entire Part",
            //        fromComp.Name,
            //        mirroredOrigin,
            //        mirroredOrientation,
            //        fromComp.Layer,
            //        out NXOpen.PartLoadStatus status);

            //    NXOpen.Features.ExtractFaceBuilder builder = originalLinkedBody.__OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);

            //    NXOpen.Body[] originalBodies = builder.ExtractBodyCollector.GetObjects().OfType<NXOpen.Body>().ToArray();

            //    if (originalBodies.Length == 0)
            //        throw new System.InvalidOperationException("Unable to find bodies for smart key");

            //    builder.Destroy();

            //    status.Dispose();

            //    __work_part_ = mirroredComp.__Prototype();

            //    Globals.__work_component_ = mirroredComp;

            //    NXOpen.Session.UndoMarkId markId2 = Globals.session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Redefine Feature");

            //    NXOpen.Features.EditWithRollbackManager rollBackManager = __work_part_.Features.StartEditWithRollbackManager(mirroredLinkedBody, markId2);

            //    using (new Destroyer(rollBackManager))
            //    {
            //        builder = __work_part_.Features.CreateExtractFaceBuilder(mirroredLinkedBody);

            //        newFromComp._ReferenceSet(Constants."Entire Part");

            //        using (new Destroyer(builder))
            //        {
            //            // For now we will just assume that every rule must be a DumbBodyRule
            //            IList<NXOpen.Body> mirrorBodies = originalBodies.Select(originalBody => (NXOpen.Body)newFromComp.FindOccurrence(originalBody)).ToList();

            //            NXOpen.SelectionIntentRule mirrorBodyDumbRule = __work_part_.ScRuleFactory.CreateRuleBodyDumb(mirrorBodies.ToArray());

            //            builder.ExtractBodyCollector.ReplaceRules(new[] { mirrorBodyDumbRule }, false);

            //            builder.Associative = true;

            //            builder.CommitFeature();

            //        }
            //    }

            //    newFromComp._ReferenceSet("BODY");
            //}
            throw new NotImplementedException();
        }

        public abstract class BaseMirrorRule : IMirrorRule
        {
            public abstract SelectionIntentRule.RuleType RuleType { get; }

            public abstract SelectionIntentRule Mirror(
                SelectionIntentRule originalRule,
                Feature originalFeature,
                Surface.Plane plane,
                Component originalComp,
                IDictionary<TaggedObject, TaggedObject> dict);

            public static bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
            {
                if(edgePoints.Count != mirrorFace.GetEdges().Length)
                    return false;

                var faceEdges = new HashSet<Edge>(mirrorFace.GetEdges());

                var edge0 = faceEdges.First();
                faceEdges.Remove(edge0);

                var edge1 = faceEdges.First();
                faceEdges.Remove(edge1);

                var edge2 = faceEdges.First();
                faceEdges.Remove(edge2);

                var edge3 = faceEdges.First();
                faceEdges.Remove(edge3);

                ISet<Edge> matchedEdges = new HashSet<Edge>();

                foreach (var tuple in edgePoints)
                {
                    if(edge0.__HasEndPoints(tuple.Item1, tuple.Item2))
                        matchedEdges.Add(edge0);

                    if(edge1.__HasEndPoints(tuple.Item1, tuple.Item2))
                        matchedEdges.Add(edge1);

                    if(edge2.__HasEndPoints(tuple.Item1, tuple.Item2))
                        matchedEdges.Add(edge2);

                    if(edge3.__HasEndPoints(tuple.Item1, tuple.Item2))
                        matchedEdges.Add(edge3);
                }

                return matchedEdges.Count == mirrorFace.GetEdges().Length;
            }


            public static SelectionIntentRule MirrorRule(
                SelectionIntentRule originalRule,
                Feature originalFeature,
                Surface.Plane plane,
                Component originalComp,
                IDictionary<TaggedObject, TaggedObject> dict)
            {
                try
                {
                    var mirroredComp = (Component)dict[originalComp];

                    // ReSharper disable once UnusedVariable
                    var mirroredPart = mirroredComp.__Prototype();

                    var mirroredFeature = (Feature)dict[originalFeature];

                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (originalRule.Type)
                    {
                        case SelectionIntentRule.RuleType.EdgeVertex:
                            return new MirrorEdgeVertexRule().Mirror(originalRule, originalFeature, plane, originalComp,
                                dict);

                        case SelectionIntentRule.RuleType.EdgeBody:
#pragma warning disable CS0612 // Type or member is obsolete
                            return new MirrorEdgeBodyRule().Mirror(originalRule, originalFeature, plane, originalComp,
                                dict);
#pragma warning restore CS0612 // Type or member is obsolete

                        case SelectionIntentRule.RuleType.EdgeTangent:
                            return new MirrorEdgeTangentRule().Mirror(originalRule, originalFeature, plane,
                                originalComp, dict);

                        case SelectionIntentRule.RuleType.EdgeDumb:
                            return new MirrorEdgeDumbRule().Mirror(originalRule, originalFeature, plane, originalComp,
                                dict);

                        case SelectionIntentRule.RuleType.EdgeBoundary:
                            return new MirrorEdgeBoundaryRule().Mirror(originalRule, originalFeature, plane,
                                originalComp, dict);

                        case SelectionIntentRule.RuleType.EdgeMultipleSeedTangent:
                            return new MirrorEdgeMultipleSeedTangentRule().Mirror(originalRule, originalFeature, plane,
                                originalComp, dict);

                        case SelectionIntentRule.RuleType.EdgeChain:
                            return new MirrorEdgeChainRule().Mirror(originalRule, originalFeature, plane, originalComp,
                                dict);

                        case SelectionIntentRule.RuleType.FaceDumb:
                            return new MirrorFaceDumbRule().Mirror(originalRule, originalFeature, plane, originalComp,
                                dict);

                        case SelectionIntentRule.RuleType.FaceTangent:
                            return new MirrorFaceTangentRule().Mirror(originalRule, originalFeature, plane,
                                originalComp, dict);

                        case SelectionIntentRule.RuleType.FaceAndAdjacentFaces:
                            return new MirrorFaceAndAdjacentFacesRule().Mirror(originalRule, originalFeature, plane,
                                originalComp, dict);

                        default:
                            throw new ArgumentException(
                                $"Unknown rule: \"{originalRule.Type}\", for feature: {mirroredFeature.GetFeatureName()} in part {mirroredFeature.OwningPart.Leaf}");
                    }
                }
                catch (NXException nxException) when (nxException.ErrorCode == 630003)
                {
                    // Suppressed edge, face, or body.
                    throw new MirrorException(nxException.Message);
                }
            }
        }

        public interface IMirrorRule
        {
            SelectionIntentRule.RuleType RuleType { get; }

            SelectionIntentRule Mirror(
                SelectionIntentRule originalRule,
                Feature originalFeature,
                Surface.Plane plane,
                Component originalComp,
                IDictionary<TaggedObject, TaggedObject> dict);
        }


        [Obsolete]
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
                throw new NotImplementedException();
                //NXOpen.Assemblies.Component mirroredComp = (NXOpen.Assemblies.Component)dict[originalComp];

                //NXOpen.Part mirroredPart = mirroredComp.__Prototype();

                //// ReSharper disable once UnusedVariable
                //_ = (NXOpen.Features.Feature)dict[originalFeature];

                //((NXOpen.EdgeBodyRule)originalRule).GetData(out NXOpen.Body originalBody);

                //ISet<NXOpen.Point3d> mirroredPositions = new HashSet<NXOpen.Point3d>(Extensions.Point3d_EqualityComparer());

                //foreach (NXOpen.Edge originalEdge in originalBody.GetEdges())
                //{
                //    NXOpen.Point3d finalStart = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp);

                //    NXOpen.Point3d finalEnd = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp);

                //    mirroredPositions.Add(finalEnd);

                //    mirroredPositions.Add(finalStart);
                //}

                //NXOpen.Body mirroredBody = null;

                //foreach (NXOpen.Body tempMirroredBody in mirroredPart.Bodies.ToArray())
                //{
                //    ISet<NXOpen.Point3d> bodyPositions = new HashSet<NXOpen.Point3d>(Extensions.Point3d_EqualityComparer());

                //    foreach (NXOpen.Edge bodyEdge in tempMirroredBody.GetEdges())
                //    {
                //        bodyPositions.Add(bodyEdge._StartPoint());
                //        bodyPositions.Add(bodyEdge._EndPoint());
                //    }

                //    if (bodyPositions.Count != mirroredPositions.Count)
                //        continue;

                //    if (!bodyPositions.SetEquals(mirroredPositions))
                //        continue;

                //    mirroredBody = tempMirroredBody;

                //    break;
                //}

                //if (mirroredBody is null)
                //    throw new ArgumentException("Could not find a matching body");

                //return mirroredPart.ScRuleFactory.CreateRuleEdgeBody(mirroredBody);
            }
        }

        public class MirrorEdgeBoundaryRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBoundary;

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

                ((EdgeBoundaryRule)originalRule).GetData(out var originalFaces);

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

                return mirroredPart.ScRuleFactory.CreateRuleEdgeBoundary(newFaces.ToArray());
            }
        }


        public class MirrorEdgeChainRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeChain;

            public override SelectionIntentRule Mirror(
                SelectionIntentRule originalRule,
                Feature originalFeature,
                Surface.Plane plane,
                Component originalComp,
                IDictionary<TaggedObject, TaggedObject> dict)
            {
                var mirroredComp = (Component)dict[originalComp];

                var mirroredPart = mirroredComp.__Prototype();

                // ReSharper disable once UnusedVariable
                _ = (Feature)dict[originalFeature];

                ((EdgeChainRule)originalRule).GetData(out var originalStartEdge, out var originalEndEdge,
                    out var isFromStart);

                Edge newStartEdge = null;

                Edge newEndEdge = null;

                var finalStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                var finalEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                foreach (var body in mirroredPart.Bodies.ToArray())
                foreach (var e in body.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newStartEdge = e;

                if(!(originalEndEdge is null))
                {
                    finalStart = originalEndEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    finalEnd = originalEndEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    foreach (var body in mirroredPart.Bodies.ToArray())
                    foreach (var e in body.GetEdges())
                        if(e.__HasEndPoints(finalStart, finalEnd))
                            newEndEdge = e;
                }

                if(newStartEdge is null)
                    throw new ArgumentException("Could not find start edge");

                return mirroredPart.ScRuleFactory.CreateRuleEdgeChain(newStartEdge, newEndEdge, isFromStart);
            }
        }

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
                var mirroredComp = (Component)dict[originalComp];

                var mirroredPart = mirroredComp.__Prototype();

                var mirroredFeature = (Feature)dict[originalFeature];

                ((EdgeDumbRule)originalRule).GetData(out var originalEdges);

                mirroredFeature.Suppress();

                originalFeature.Suppress();

                //            Snap.InfoWindow.WriteLine(originalFeature.GetFeatureName());
                //            
                //            throw new NotImplementedException();

                IList<Edge> newEdges = new List<Edge>();

                foreach (var originalEdge in originalEdges)
                {
                    var finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    var finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    mirroredPart.Curves.CreateLine(finalStart, finalEnd);

                    foreach (var body in mirroredPart.Bodies.ToArray())
                    foreach (var e in body.GetEdges())
                        if(e.__HasEndPoints(finalStart, finalEnd))
                            newEdges.Add(e);
                }

                mirroredFeature.Unsuppress();

                originalFeature.Unsuppress();

                return mirroredPart.ScRuleFactory.CreateRuleEdgeDumb(newEdges.ToArray());
            }
        }

        public class MirrorEdgeMultipleSeedTangentRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } =
                SelectionIntentRule.RuleType.EdgeMultipleSeedTangent;

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

                ((EdgeMultipleSeedTangentRule)originalRule).GetData(out var originalSeedEdges, out var angleTolerance,
                    out var hasSameConvexity);

                IList<Edge> newEdges = new List<Edge>();

                foreach (var originalEdge in originalSeedEdges)
                {
                    var originalBody = originalEdge.GetBody();

                    Body mirrorBody;

                    if(!dict.ContainsKey(originalBody))
                    {
                        mirroredFeature.Suppress();

                        originalFeature.Suppress();

                        var originalOwningFeature =
                            originalComp.__Prototype().Features.GetParentFeatureOfBody(originalBody);

                        var mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                        if(mirrorOwningFeature.GetBodies().Length != 1)
                            throw new InvalidOperationException("Invalid number of bodies for feature");

                        mirrorBody = mirrorOwningFeature.GetBodies()[0];
                    }
                    else
                    {
                        mirrorBody = (Body)dict[originalBody];
                    }

                    var finalStart = originalEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    var finalEnd = originalEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    foreach (var e in mirrorBody.GetEdges())
                        if(e.__HasEndPoints(finalStart, finalEnd))
                            newEdges.Add(e);
                }

                return mirroredPart.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(newEdges.ToArray(), angleTolerance,
                    hasSameConvexity);
            }
        }

        public class MirrorEdgeTangentRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeTangent;

            public override SelectionIntentRule Mirror(
                SelectionIntentRule originalRule,
                Feature originalFeature,
                Surface.Plane plane,
                Component originalComp,
                IDictionary<TaggedObject, TaggedObject> dict)
            {
                var mirroredComp = (Component)dict[originalComp];

                var mirroredPart = mirroredComp.__Prototype();

                // ReSharper disable once UnusedVariable
                _ = (Feature)dict[originalFeature];

                ((EdgeTangentRule)originalRule).GetData(out var originalStartEdge, out var originalEndEdge,
                    out var isFromStart, out var angleTolerance, out var hasSameConvexity);

                Edge newStartEdge = null;

                Edge newEndEdge = null;

                var finalStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                var finalEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                foreach (var body in mirroredPart.Bodies.ToArray())
                foreach (var e in body.GetEdges())
                    if(e.__HasEndPoints(finalStart, finalEnd))
                        newStartEdge = e;

                if(!(originalEndEdge is null))
                {
                    finalStart = originalEndEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    finalEnd = originalEndEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                    foreach (var body in mirroredPart.Bodies.ToArray())
                    foreach (var e in body.GetEdges())
                        if(e.__HasEndPoints(finalStart, finalEnd))
                            newEndEdge = e;
                }

                if(newStartEdge is null)
                    throw new ArgumentException("Could not find start edge");

                return mirroredPart.ScRuleFactory.CreateRuleEdgeTangent(newStartEdge, newEndEdge, isFromStart,
                    angleTolerance, hasSameConvexity);
            }
        }


        public class MirrorEdgeVertexRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeVertex;

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

                ((EdgeVertexRule)originalRule).GetData(out var originalStartEdge, out var isFromStart);

                var originalBody = originalStartEdge.GetBody();

                Body mirrorBody;

                if(!dict.ContainsKey(originalBody))
                {
                    mirroredFeature.Suppress();

                    originalFeature.Suppress();

                    var originalOwningFeature =
                        originalComp.__Prototype().Features.GetParentFeatureOfBody(originalBody);

                    var mirrorOwningFeature = (BodyFeature)dict[originalOwningFeature];

                    if(mirrorOwningFeature.GetBodies().Length != 1)
                        throw new InvalidOperationException("Invalid number of bodies for feature");

                    mirrorBody = mirrorOwningFeature.GetBodies()[0];
                }
                else
                {
                    mirrorBody = (Body)dict[originalBody];
                }

                var newStart = originalStartEdge.__StartPoint().__MirrorMap(plane, originalComp, mirroredComp);

                var newEnd = originalStartEdge.__EndPoint().__MirrorMap(plane, originalComp, mirroredComp);

                var mirrorEdge = mirrorBody.GetEdges().FirstOrDefault(edge => edge.__HasEndPoints(newStart, newEnd));

                if(mirrorEdge is null)
                    throw new InvalidOperationException("Could not find mirror edge");

                return mirroredPart.ScRuleFactory.CreateRuleEdgeVertex(mirrorEdge, isFromStart);
            }
        }

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
                var mirroredComp = (Component)dict[originalComp];

                var mirroredPart = mirroredComp.__Prototype();

                var mirroredFeature = (Feature)dict[originalFeature];

                mirroredFeature.Suppress();

                ((FaceDumbRule)originalRule).GetData(out var originalFaces);

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

                var originalOwningFeatureOfStartFace = originalStartFace.__OwningPart().Features
                    .GetParentFeatureOfFace(originalStartFace);

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

        public class MirrorFaceAndAdjacentFacesRule : BaseMirrorRule
        {
            public override SelectionIntentRule.RuleType RuleType { get; } =
                SelectionIntentRule.RuleType.FaceAndAdjacentFaces;

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

                ((FaceAndAdjacentFacesRule)originalRule).GetData(out var originalFaces);

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

                return mirroredPart.ScRuleFactory.CreateRuleFaceAndAdjacentFaces(newFaces[0]);
            }
        }
    }
}