using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CTS_Library;
using CTS_Library.Equality;
using CTS_Library.Extensions;
using CTS_Library.UFuncs.MirrorComponents.LibraryComponents;
using CTS_Library.Utilities;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using TSG_Library.Extensions;
using TSG_Library.Geom;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{

    public class MirrorLinkedBody : BaseMirrorFeature
    {
        public override string FeatureType { get; } = "LINKED_BODY";


        public override void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp)
        {
            Feature feature = (Feature)dict[originalFeature];
            ExtractFace extractFace = (ExtractFace)feature;
            Component component = (Component)dict[originalComp];
            if (!extractFace.__IsBroken())
            {
                return;
            }

            ExtractFace extractFace2 = (ExtractFace)originalFeature;
            if (extractFace2._IsBroken())
            {
                return;
            }

            Tag assy_context_xform = extractFace2._XFormTag();
            Globals._UFSession.So.AskAssyCtxtPartOcc(assy_context_xform, originalComp.Tag, out var from_part_occ);
            Component component2 = (Component)Globals._Session.GetObjectManager().GetTaggedObject(from_part_occ);
            Globals._UFSession.Wave.AskLinkedFeatureGeom(extractFace2.Tag, out var linked_geom);
            Globals._UFSession.Wave.AskLinkedFeatureInfo(linked_geom, out var _);
            if (component2 == null)
            {
                throw new MirrorException("Linked component was null in " + originalFeature._OwningPart().Leaf + " from " + originalFeature.GetFeatureName());
            }

            Component[] array = component2._AssemblyPath().ToArray();
            Component component3 = array[array.Length - 2];
            string[] array2 = new string[array.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = array[i].ReferenceSet;
            }

            using (new ReferenceSetReset(component3))
            {
                component3._ReferenceSet("Entire Part");
                ILibraryComponent[] array3 = new ILibraryComponent[4]
                {
                new MirrorSmartButton(),
                new MirrorSmartKey(),
                new MirrorSmartStockEjector(),
                new MirrorSmartStandardLiftersGuidedKeepersMetric()
                };
                ILibraryComponent[] array4 = array3;
                foreach (ILibraryComponent libraryComponent in array4)
                {
                    if (libraryComponent.IsLibraryComponent(component2))
                    {
                        libraryComponent.Mirror(plane, component, extractFace2, component2, dict);
                        originalFeature.Unsuppress();
                        feature.Unsuppress();
                        return;
                    }
                }

                if (!component2.DisplayName.Contains("layout") && !component2.DisplayName.Contains("blank"))
                {
                    return;
                }

                Globals._WorkPart = Globals._DisplayPart;
                using (new ReferenceSetReset(component2))
                {
                    component2._ReferenceSet("Entire Part");
                    Globals._WorkPart = originalComp._Prototype();
                    ExtractFaceBuilder extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace2);
                    Body[] bodies;
                    using (new Destroyer(extractFaceBuilder))
                    {
                        bodies = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<NXObject>().Select(component2.FindOccurrence)
                            .OfType<Body>()
                            .ToArray();
                    }

                    Globals._WorkComponent = component;
                    Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                    EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
                    using (new Destroyer(editWithRollbackManager))
                    {
                        ExtractFaceBuilder extractFaceBuilder2 = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                        using (new Destroyer(extractFaceBuilder2))
                        {
                            BodyDumbRule bodyDumbRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(bodies, includeSheetBodies: true);
                            SelectionIntentRule[] rules = new SelectionIntentRule[1] { bodyDumbRule };
                            extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules, createRulesWoUpdate: false);
                            extractFaceBuilder2.Associative = true;
                            extractFaceBuilder2.Commit();
                        }

                        Globals._Session.Preferences.Modeling.UpdatePending = false;
                    }

                    originalFeature.Unsuppress();
                    feature.Unsuppress();
                    Globals._WorkPart = Globals._DisplayPart;
                }
            }
        }
    }


    public static class Extensions_Vector
    {
        public static bool _IsPerpendicularTo(this Vector3d vec1, Vector3d vec2, double tolerance = 0.0001)
        {
            UFSession.GetUFSession().Vec3.IsPerpendicular(vec1.__ToArray(), vec2.__ToArray(), tolerance, out var is_perp);
            return is_perp == 1;
        }

        public static bool _IsParallelTo(this Vector3d vector1, Vector3d vector2, double tolerance = 0.0001)
        {
            UFSession.GetUFSession().Vec3.IsParallel(vector1.__ToArray(), vector2.__ToArray(), tolerance, out var is_parallel);
            return is_parallel == 1;
        }

        public static Vector3d _UnitVector(this Vector3d vector)
        {
            return vector.__Unit();
        }

        public static double _MagnitudeLength(this Vector3d vector)
        {
            return vector.__Norm();
        }

        public static Vector3d _AbsVector(this Vector3d vector)
        {
            return new Vector3d(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }

        public static Vector3d _Cross(this Vector3d vector1, Vector3d vector2)
        {
            return new Vector3d(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
        }

        public static double _DotProduct(this Vector3d vector1, Vector3d vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public static Orientation _CreateOrientationXVectorZVector(this Vector3d xVector, Vector3d zVector)
        {
            if (!xVector._IsPerpendicularTo(zVector))
            {
                throw new InvalidOperationException("You cannot create an orientation from two vectors that are not perpendicular to each other.");
            }

            Orientation val = new Orientation(xVector, zVector);
            Vector3d axisZ = val.AxisZ;
            return new Orientation(xVector, axisZ);
        }

        public static Orientation _CreateOrientationYVectorZVector(this Vector3d yVector, Vector3d zVector)
        {
            if (!yVector._IsPerpendicularTo(zVector))
            {
                throw new InvalidOperationException("You cannot create an orientation from two vectors that are not perpendicular to each other.");
            }

            Orientation val = new Orientation(yVector, zVector);
            Vector axisZ = val.AxisZ;
            return new Orientation(axisZ, yVector);
        }

        public static Vector _Mirror(this Vector original, Plane plane)
        {
            Transform val = Transform.CreateReflection(plane);
            return ((Vector)(ref original)).Copy(val);
        }

        public static Vector _MirrorMap(this Vector vector, Plane mirrorPlane, Component originalComp, Component newComp)
        {
            originalComp._SetWcsToComponent();
            Vector original = CoordinateSystem.MapWcsToAcs(vector);
            Vector val = original._Mirror(mirrorPlane);
            newComp._SetWcsToComponent();
            return CoordinateSystem.MapAcsToWcs(val);
        }

        public static Vector _MirrorMap(this Vector3d vector, Plane mirrorPlane, Component originalComp, Component newComp)
        {
            return _MirrorMap(new Vector(vector), mirrorPlane, originalComp, newComp);
        }

        public static Vector3d _Mirror(this Vector3d original, Plane plane)
        {
            return Vector.op_Implicit(_Mirror(new Vector(original), plane));
        }

        public static double[] _Array(this Vector3d vector3D)
        {
            return new double[3] { vector3D.X, vector3D.Y, vector3D.Z };
        }
    }

    public abstract class BaseMirrorFeature : IMirrorFeature
    {
        public abstract string FeatureType { get; }

        public Point3d MirrorMap(Point3d position, Plane plane, Component fromComponent, Component toComponent)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            //IL_0028: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_005c: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0062: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Unknown result type (might be due to invalid IL or missing references)
            Globals._DisplayPart.WCS.SetOriginAndMatrix(Point3d.op_Implicit(fromComponent._Origin()), Matrix3x3.op_Implicit(fromComponent._Orientation()));
            Point3d val = CoordinateSystem.MapWcsToAcs(position)._Mirror(plane);
            Globals._DisplayPart.WCS.SetOriginAndMatrix(Point3d.op_Implicit(toComponent._Origin()), Matrix3x3.op_Implicit(toComponent._Orientation()));
            return CoordinateSystem.MapAcsToWcs(val);
        }

        public Vector3d MirrorMap(Vector3d vector, Plane plane, Component fromComponent, Component toComponent)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            //IL_0028: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_005c: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0062: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Unknown result type (might be due to invalid IL or missing references)
            Globals._DisplayPart.WCS.SetOriginAndMatrix(Point3d.op_Implicit(fromComponent._Origin()), Matrix3x3.op_Implicit(fromComponent._Orientation()));
            Vector3d val = CoordinateSystem.MapWcsToAcs(vector)._Mirror(plane);
            Globals._DisplayPart.WCS.SetOriginAndMatrix(Point3d.op_Implicit(toComponent._Origin()), Matrix3x3.op_Implicit(toComponent._Orientation()));
            return CoordinateSystem.MapAcsToWcs(val);
        }

        public Matrix3x3 MirrorMap(Matrix3x3 orientation, Plane plane, Component fromComponent, Component toComponent)
        {
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Expected O, but got Unknown
            Vector3d val = MirrorMap(orientation.AxisY, plane, fromComponent, toComponent);
            Vector3d val2 = MirrorMap(orientation.AxisX, plane, fromComponent, toComponent);
            return new Matrix3x3(val, val2);
        }

        public abstract void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Plane plane, Component originalComp);

        public bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            if (edgePoints.Count != mirrorFace.GetEdges().Length)
            {
                return false;
            }

            HashSet<Edge> hashSet = new HashSet<Edge>(mirrorFace.GetEdges());
            Edge edge = hashSet.First();
            hashSet.Remove(edge);
            Edge edge2 = hashSet.First();
            hashSet.Remove(edge2);
            Edge edge3 = hashSet.First();
            hashSet.Remove(edge3);
            Edge edge4 = hashSet.First();
            hashSet.Remove(edge4);
            ISet<Edge> set = new HashSet<Edge>();
            foreach (Tuple<Point3d, Point3d> edgePoint in edgePoints)
            {
                if (edge.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge);
                }

                if (edge2.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge2);
                }

                if (edge3.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge3);
                }

                if (edge4.__HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge4);
                }
            }

            return set.Count == mirrorFace.GetEdges().Length;
        }
    }



    public interface IMirrorFeature
    {
        string FeatureType { get; }

        void Mirror(Feature originalFeature, IDictionary<TaggedObject, TaggedObject> dict, Surface.Plane plane, Component originalComp);

        Point3d MirrorMap(Point3d position, Surface.Plane plane, Component fromComponent, Component toComponent);

        Vector3d MirrorMap(Vector3d vector, Surface.Plane plane, Component fromComponent, Component toComponent);

        Matrix3x3 MirrorMap(Matrix3x3 orientation, Surface.Plane plane, Component fromComponent, Component toComponent);
    }


    public interface IMirrorRule
    {
        SelectionIntentRule.RuleType RuleType { get; }

        SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict);
    }


    public abstract class BaseMirrorRule : IMirrorRule
    {
        public abstract SelectionIntentRule.RuleType RuleType { get; }

        public static bool EdgePointsMatchFace(Face mirrorFace, IList<Tuple<Point3d, Point3d>> edgePoints)
        {
            //IL_008e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0095: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b8: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00db: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f8: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ff: Unknown result type (might be due to invalid IL or missing references)
            if (edgePoints.Count != mirrorFace.GetEdges().Length)
            {
                return false;
            }

            HashSet<Edge> hashSet = new HashSet<Edge>(mirrorFace.GetEdges());
            Edge edge = hashSet.First();
            hashSet.Remove(edge);
            Edge edge2 = hashSet.First();
            hashSet.Remove(edge2);
            Edge edge3 = hashSet.First();
            hashSet.Remove(edge3);
            Edge edge4 = hashSet.First();
            hashSet.Remove(edge4);
            ISet<Edge> set = new HashSet<Edge>();
            foreach (Tuple<Point3d, Point3d> edgePoint in edgePoints)
            {
                if (edge._HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge);
                }

                if (edge2._HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge2);
                }

                if (edge3._HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge3);
                }

                if (edge4._HasEndPoints(edgePoint.Item1, edgePoint.Item2))
                {
                    set.Add(edge4);
                }
            }

            return set.Count == mirrorFace.GetEdges().Length;
        }

        public abstract SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict);

        public static SelectionIntentRule MirrorRule(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            try
            {
                Component component = (Component)dict[originalComp];
                Part part = component._Prototype();
                Feature feature = (Feature)dict[originalFeature];
                return originalRule.Type switch
                {
                    SelectionIntentRule.RuleType.EdgeVertex => new MirrorEdgeVertexRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeBody => new MirrorEdgeBodyRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeTangent => new MirrorEdgeTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeDumb => new MirrorEdgeDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeBoundary => new MirrorEdgeBoundaryRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeMultipleSeedTangent => new MirrorEdgeMultipleSeedTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.EdgeChain => new MirrorEdgeChainRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.FaceDumb => new MirrorFaceDumbRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.FaceTangent => new MirrorFaceTangentRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    SelectionIntentRule.RuleType.FaceAndAdjacentFaces => new MirrorFaceAndAdjacentFacesRule().Mirror(originalRule, originalFeature, plane, originalComp, dict),
                    _ => throw new ArgumentException($"Unknown rule: \"{originalRule.Type}\", for feature: {feature.GetFeatureName()} in part {feature.OwningPart.Leaf}"),
                };
            }
            catch (NXException ex) when (ex.ErrorCode == 630003)
            {
                throw new MirrorException(ex.Message);
            }
        }
    }


    public class MirrorEdgeVertexRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeVertex;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {

            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeVertexRule)originalRule).GetData(out var startEdge, out var isFromStart);
            Body body = startEdge.GetBody();
            Body body2;
            if (!dict.ContainsKey(body))
            {
                feature.Suppress();
                originalFeature.Suppress();
                Feature parentFeatureOfBody = originalComp._Prototype().Features.GetParentFeatureOfBody(body);
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

            Point3d newStart = startEdge._StartPoint()._MirrorMap(plane, originalComp, component);
            Point3d newEnd = startEdge._EndPoint()._MirrorMap(plane, originalComp, component);
            Edge edge2 = body2.GetEdges().FirstOrDefault((Edge edge) => edge._HasEndPoints(newStart, newEnd));
            if (edge2 == null)
            {
                throw new InvalidOperationException("Could not find mirror edge");
            }

            return part.ScRuleFactory.CreateRuleEdgeVertex(edge2, isFromStart);
        }
    }



    public class MirrorEdgeBodyRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeBody;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            //IL_0060: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Unknown result type (might be due to invalid IL or missing references)
            //IL_0069: Unknown result type (might be due to invalid IL or missing references)
            //IL_0072: Unknown result type (might be due to invalid IL or missing references)
            //IL_0077: Unknown result type (might be due to invalid IL or missing references)
            //IL_007b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0085: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f3: Unknown result type (might be due to invalid IL or missing references)
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeBodyRule)originalRule).GetData(out var body);
            ISet<Point3d> set = new HashSet<Point3d>(new EqualityPos());
            Edge[] edges = body.GetEdges();
            foreach (Edge edge in edges)
            {
                Point3d item = edge._StartPoint()._MirrorMap(plane, originalComp, component);
                Point3d item2 = edge._EndPoint()._MirrorMap(plane, originalComp, component);
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
                    set2.Add(edge2._StartPoint());
                    set2.Add(edge2._EndPoint());
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



    public class MirrorEdgeTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeTangent;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Surface.Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            //IL_0042: Unknown result type (might be due to invalid IL or missing references)
            //IL_004b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0050: Unknown result type (might be due to invalid IL or missing references)
            //IL_0053: Unknown result type (might be due to invalid IL or missing references)
            //IL_005c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0061: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d5: Unknown result type (might be due to invalid IL or missing references)
            //IL_00de: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e3: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f0: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f5: Unknown result type (might be due to invalid IL or missing references)
            //IL_0095: Unknown result type (might be due to invalid IL or missing references)
            //IL_0097: Unknown result type (might be due to invalid IL or missing references)
            //IL_0129: Unknown result type (might be due to invalid IL or missing references)
            //IL_012b: Unknown result type (might be due to invalid IL or missing references)
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeTangentRule)originalRule).GetData(out var startEdge, out var endEdge, out var isFromStart, out var angleTolerance, out var hasSameConvexity);
            Edge edge = null;
            Edge endEdge2 = null;
            Point3d pos = startEdge._StartPoint()._MirrorMap(plane, originalComp, component);
            Point3d pos2 = startEdge._EndPoint()._MirrorMap(plane, originalComp, component);
            Body[] array = part.Bodies.ToArray();
            foreach (Body body in array)
            {
                Edge[] edges = body.GetEdges();
                foreach (Edge edge2 in edges)
                {
                    if (edge2._HasEndPoints(pos, pos2))
                    {
                        edge = edge2;
                    }
                }
            }

            if (endEdge != null)
            {
                pos = endEdge._StartPoint()._MirrorMap(plane, originalComp, component);
                pos2 = endEdge._EndPoint()._MirrorMap(plane, originalComp, component);
                Body[] array2 = part.Bodies.ToArray();
                foreach (Body body2 in array2)
                {
                    Edge[] edges2 = body2.GetEdges();
                    foreach (Edge edge3 in edges2)
                    {
                        if (edge3._HasEndPoints(pos, pos2))
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

            return part.ScRuleFactory.CreateRuleEdgeTangent(edge, endEdge2, isFromStart, angleTolerance, hasSameConvexity);
        }
    }



    public class MirrorEdgeMultipleSeedTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeMultipleSeedTangent;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            //IL_00e1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ea: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ef: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f3: Unknown result type (might be due to invalid IL or missing references)
            //IL_00fc: Unknown result type (might be due to invalid IL or missing references)
            //IL_0101: Unknown result type (might be due to invalid IL or missing references)
            //IL_011b: Unknown result type (might be due to invalid IL or missing references)
            //IL_011d: Unknown result type (might be due to invalid IL or missing references)
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
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
                    Feature parentFeatureOfBody = originalComp._Prototype().Features.GetParentFeatureOfBody(body);
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

                Point3d pos = edge._StartPoint()._MirrorMap(plane, originalComp, component);
                Point3d pos2 = edge._EndPoint()._MirrorMap(plane, originalComp, component);
                Edge[] edges = body2.GetEdges();
                foreach (Edge edge2 in edges)
                {
                    if (edge2._HasEndPoints(pos, pos2))
                    {
                        list.Add(edge2);
                    }
                }
            }

            return part.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(list.ToArray(), angleTolerance, hasSameConvexity);
        }
    }




    public class MirrorEdgeChainRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.EdgeChain;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            //IL_003e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0047: Unknown result type (might be due to invalid IL or missing references)
            //IL_004c: Unknown result type (might be due to invalid IL or missing references)
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0058: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00da: Unknown result type (might be due to invalid IL or missing references)
            //IL_00df: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e3: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ec: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f1: Unknown result type (might be due to invalid IL or missing references)
            //IL_0091: Unknown result type (might be due to invalid IL or missing references)
            //IL_0093: Unknown result type (might be due to invalid IL or missing references)
            //IL_0125: Unknown result type (might be due to invalid IL or missing references)
            //IL_0127: Unknown result type (might be due to invalid IL or missing references)
            Component component = (Component)dict[originalComp];
            Part part = component._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            ((EdgeChainRule)originalRule).GetData(out var startEdge, out var endEdge, out var isFromStart);
            Edge edge = null;
            Edge endEdge2 = null;
            Point3d pos = startEdge._StartPoint()._MirrorMap(plane, originalComp, component);
            Point3d pos2 = startEdge._EndPoint()._MirrorMap(plane, originalComp, component);
            Body[] array = part.Bodies.ToArray();
            foreach (Body body in array)
            {
                Edge[] edges = body.GetEdges();
                foreach (Edge edge2 in edges)
                {
                    if (edge2._HasEndPoints(pos, pos2))
                    {
                        edge = edge2;
                    }
                }
            }

            if (endEdge != null)
            {
                pos = endEdge._StartPoint()._MirrorMap(plane, originalComp, component);
                pos2 = endEdge._EndPoint()._MirrorMap(plane, originalComp, component);
                Body[] array2 = part.Bodies.ToArray();
                foreach (Body body2 in array2)
                {
                    Edge[] edges2 = body2.GetEdges();
                    foreach (Edge edge3 in edges2)
                    {
                        if (edge3._HasEndPoints(pos, pos2))
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



    public class MirrorFaceDumbRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceDumb;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part mirroredPart = mirroredComp._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            ((FaceDumbRule)originalRule).GetData(out var faces);
            IList<Face> source = (from originalFace in faces
                                  select (from originalEdge in originalFace.GetEdges()
                                          let finalStart = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                          let finalEnd = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                          select new Tuple<Point3d, Point3d>(finalStart, finalEnd)).ToList() into edgePoints
                                  from mirrorBody in mirroredPart.Bodies.ToArray()
                                  from mirrorFace in mirrorBody.GetFaces()
                                  where BaseMirrorRule.EdgePointsMatchFace(mirrorFace, edgePoints)
                                  select mirrorFace).ToList();
            return mirroredPart.ScRuleFactory.CreateRuleFaceDumb(source.ToArray());
        }
    }




    public class MirrorFaceTangentRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceTangent;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part part = mirroredComp._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            ((FaceTangentRule)originalRule).GetData(out var startFace, out var endFace, out var _, out var _, out var _);
            IList<Tuple<Point3d, Point3d>> edgePoints = (from edge in startFace.GetEdges()
                                                         let mirrorStart = edge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                         let mirrorEnd = edge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                         select Tuple.Create<Point3d, Point3d>(mirrorStart, mirrorEnd)).ToList();
            IList<Tuple<Point3d, Point3d>> edgePoints2 = (from edge in endFace.GetEdges()
                                                          let mirrorStart = edge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                          let mirrorEnd = edge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                                          select Tuple.Create<Point3d, Point3d>(mirrorStart, mirrorEnd)).ToList();
            Face face = null;
            Face face2 = null;
            Feature parentFeatureOfFace = startFace._OwningPart().Features.GetParentFeatureOfFace(startFace);
            BodyFeature bodyFeature = (BodyFeature)dict[parentFeatureOfFace];
            Body[] bodies = bodyFeature.GetBodies();
            foreach (Body body in bodies)
            {
                if (face != null && face2 != null)
                {
                    break;
                }

                Face[] faces = body.GetFaces();
                foreach (Face face3 in faces)
                {
                    if (face == null && BaseMirrorRule.EdgePointsMatchFace(face3, edgePoints))
                    {
                        face = face3;
                    }
                    else if (face2 == null && BaseMirrorRule.EdgePointsMatchFace(face3, edgePoints2))
                    {
                        face2 = face3;
                    }
                }
            }

            if (face == null)
            {
                throw new ArgumentException("Unable to find start face");
            }

            if (face2 == null)
            {
                throw new ArgumentException("Unable to find end face");
            }

            return part.ScRuleFactory.CreateRuleFaceTangent(face, new Face[1] { face2 });
        }
    }




    public class MirrorFaceAndAdjacentFacesRule : BaseMirrorRule
    {
        public override SelectionIntentRule.RuleType RuleType { get; } = SelectionIntentRule.RuleType.FaceAndAdjacentFaces;


        public override SelectionIntentRule Mirror(SelectionIntentRule originalRule, Feature originalFeature, Plane plane, Component originalComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];
            Part mirroredPart = mirroredComp._Prototype();
            Feature feature = (Feature)dict[originalFeature];
            feature.Suppress();
            ((FaceAndAdjacentFacesRule)originalRule).GetData(out var faces);
            IList<Face> list = (from originalFace in faces
                                select (from originalEdge in originalFace.GetEdges()
                                        let finalStart = originalEdge._StartPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                        let finalEnd = originalEdge._EndPoint()._MirrorMap(plane, originalComp, mirroredComp)
                                        select new Tuple<Point3d, Point3d>(finalStart, finalEnd)).ToList() into edgePoints
                                from mirrorBody in mirroredPart.Bodies.ToArray()
                                from mirrorFace in mirrorBody.GetFaces()
                                where BaseMirrorRule.EdgePointsMatchFace(mirrorFace, edgePoints)
                                select mirrorFace).ToList();
            return mirroredPart.ScRuleFactory.CreateRuleFaceAndAdjacentFaces(list[0]);
        }
    }


    public class MirrorException : Exception
    {
        public MirrorException()
        {
        }

        public MirrorException(string message)
            : base(message)
        {
        }
    }



    public class MirrorSmartKey : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            {
                return false;
            }

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART KEY METRIC";
        }

        public void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_0032: Unknown result type (might be due to invalid IL or missing references)
            //IL_0037: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            //IL_003f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            //IL_004a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0050: Expected O, but got Unknown
            //IL_0070: Unknown result type (might be due to invalid IL or missing references)
            ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            Position val = fromComp._Origin()._Mirror(plane);
            Orientation val2 = fromComp._Orientation()._Mirror(plane);
            Vector axisY = val2.AxisY;
            Vector val3 = -val2.AxisX;
            val2 = new Orientation(axisY, val3);
            PartLoadStatus loadStatus;
            Component newFromComp = fromComp._OwningPart().ComponentAssembly.AddComponent(fromComp._Prototype(), "Entire Part", fromComp.Name, Position.op_Implicit(val), Orientation.op_Implicit(val2), fromComp.Layer, out loadStatus);
            ExtractFaceBuilder extractFaceBuilder = originalLinkedBody._OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            if (array.Length == 0)
            {
                throw new InvalidOperationException("Unable to find bodies for smart key");
            }

            extractFaceBuilder.Destroy();
            loadStatus.Dispose();
            Globals._WorkPart = mirroredComp._Prototype();
            Globals._WorkComponent = mirroredComp;
            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                newFromComp._ReferenceSet("Entire Part");
                using (new Destroyer(extractFaceBuilder))
                {
                    IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
                    SelectionIntentRule selectionIntentRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
                    extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
                    extractFaceBuilder.Associative = true;
                    extractFaceBuilder.CommitFeature();
                }
            }

            newFromComp._ReferenceSet("BODY");
        }
    }


    public class MirrorSmartStockEjector : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            {
                return false;
            }

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART STOCK EJECTORS";
        }

        public void Mirror(Surface.Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {

            ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            Point3d val = fromComp.__Origin().__Mirror(plane);
            Matrix3x3 val2 = fromComp.__Orientation().__Mirror(plane);
            Vector axisY = val2.AxisY;
            Vector val3 = -val2.AxisX;
            val2 = new Orientation(axisY, val3);
            PartLoadStatus loadStatus;
            Component newFromComp = fromComp._OwningPart().ComponentAssembly.AddComponent(fromComp._Prototype(), "Entire Part", fromComp.Name, Position.op_Implicit(val), Orientation.op_Implicit(val2), fromComp.Layer, out loadStatus);
            ExtractFaceBuilder extractFaceBuilder = originalLinkedBody._OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            if (array.Length == 0)
            {
                throw new InvalidOperationException("Unable to find bodies for smart key");
            }

            extractFaceBuilder.Destroy();
            loadStatus.Dispose();
            Globals._WorkPart = mirroredComp._Prototype();
            Globals._WorkComponent = mirroredComp;
            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                newFromComp._ReferenceSet("Entire Part");
                using (new Destroyer(extractFaceBuilder))
                {
                    IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
                    SelectionIntentRule selectionIntentRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
                    extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
                    extractFaceBuilder.Associative = true;
                    extractFaceBuilder.CommitFeature();
                }
            }

            newFromComp._ReferenceSet("BODY");
        }
    }






    public class MirrorSmartStandardLiftersGuidedKeepersMetric : ILibraryComponent
    {
        public bool IsLibraryComponent(Component component)
        {
            if (!component.HasUserAttribute("LIBRARY", NXObject.AttributeType.String, -1))
            {
                return false;
            }

            return component.GetUserAttributeAsString("LIBRARY", NXObject.AttributeType.String, -1).ToUpper() == "SMART STANDARD LIFTERS GUIDED KEEPERS METRIC";
        }

        public void Mirror(Plane plane, Component mirroredComp, ExtractFace originalLinkedBody, Component fromComp, IDictionary<TaggedObject, TaggedObject> dict)
        {
            ExtractFace extractFace = (ExtractFace)dict[originalLinkedBody];
            Point3d val = fromComp.__Origin().__Mirror(plane);
            Orientation val2 = fromComp._Orientation()._Mirror(plane);
            Vector axisY = val2.AxisY;
            Vector val3 = -val2.AxisX;
            val2 = new Orientation(axisY, val3);
            PartLoadStatus loadStatus;
            Component newFromComp = fromComp._OwningPart().ComponentAssembly.AddComponent(fromComp._Prototype(), "Entire Part", fromComp.Name, Position.op_Implicit(val), Orientation.op_Implicit(val2), fromComp.Layer, out loadStatus);
            ExtractFaceBuilder extractFaceBuilder = originalLinkedBody._OwningPart().Features.CreateExtractFaceBuilder(originalLinkedBody);
            Body[] array = extractFaceBuilder.ExtractBodyCollector.GetObjects().OfType<Body>().ToArray();
            if (array.Length == 0)
            {
                throw new InvalidOperationException("Unable to find bodies for smart key");
            }

            extractFaceBuilder.Destroy();
            loadStatus.Dispose();
            Globals._WorkPart = mirroredComp._Prototype();
            Globals._WorkComponent = mirroredComp;
            Session.UndoMarkId featureEditMark = Globals._Session.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager = Globals._WorkPart.Features.StartEditWithRollbackManager(extractFace, featureEditMark);
            using (new Destroyer(editWithRollbackManager))
            {
                extractFaceBuilder = Globals._WorkPart.Features.CreateExtractFaceBuilder(extractFace);
                newFromComp._ReferenceSet("Entire Part");
                using (new Destroyer(extractFaceBuilder))
                {
                    IList<Body> source = array.Select((Body originalBody) => (Body)newFromComp.FindOccurrence(originalBody)).ToList();
                    SelectionIntentRule selectionIntentRule = Globals._WorkPart.ScRuleFactory.CreateRuleBodyDumb(source.ToArray());
                    extractFaceBuilder.ExtractBodyCollector.ReplaceRules(new SelectionIntentRule[1] { selectionIntentRule }, createRulesWoUpdate: false);
                    extractFaceBuilder.Associative = true;
                    extractFaceBuilder.CommitFeature();
                }
            }

            newFromComp._ReferenceSet("BODY");
        }
    }




}