using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Drawings;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Layer;
using NXOpen.Positioning;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Disposable;
using TSG_Library.Enum;
using TSG_Library.Exceptions;
using TSG_Library.Geom;
using TSG_Library.UFuncs;
using TSG_Library.Utilities;
using static NXOpen.Session;
using Curve = NXOpen.Curve;
using Type = NXOpen.GeometricUtilities.Type;
using Unit = TSG_Library.Enum.Unit;
using static NXOpen.UF.UFConstants;
using TSG_Library.Attributes;

namespace TSG_Library
{
    [ExtensionsAspect]
    public static class Extensions
    {
        public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        private const int lowerA = 97;

        private const int lowerZ = 122;

        private const int upperA = 65;

        private const int upperZ = 90;


        //
        // Summary:
        //     Calculates curve derivatives (and position) at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter at which to evaluate
        //
        //   order:
        //     Order of highest derivative returned (zero for position alone)
        //
        // Returns:
        //     Array of derivative vectors -- [0] is position, [1] is first derivative, etc.
        //
        //
        // Remarks:
        //     The [0] element of the returned array represents a location on the curve, even
        //     though it is contained in a Vector variable. In many cases, you (the caller)
        //     should cast this Vector to a Position variable before using it further.
        //
        //     The maximum value allowed for order is 3. Usinga value greater than 3 will result
        //     in a run-time error.
        [Obsolete(nameof(NotImplementedException))]
        public static Point3d[] __Derivatives(double value, int order)
        {
            //bool flag = ObjectType == ObjectTypes.Type.Arc;
            //UFEval eval = ufsession_.Eval;
            //eval.Initialize2(NXOpenTag, out var evaluator);
            //double[] array = new double[3];
            //double[] array2 = array;
            //double[] array3 = new double[3 * order];
            //value /= Factor;
            //eval.Evaluate(evaluator, order, value, array2, array3);
            //eval.Free(evaluator);
            //Vector[] array4 = new Vector[order + 1];
            //ref Vector reference = ref array4[0];
            //reference = array2;
            //for (int i = 0; i < order; i++)
            //{
            //    ref Vector reference2 = ref array4[i + 1];
            //    reference2 = new Vector(array3[3 * i], array3[3 * i + 1], array3[3 * i + 2]);
            //    if (flag)
            //    {
            //        ref Vector reference3 = ref array4[i + 1];
            //        reference3 = System.Math.Pow(System.Math.PI / 180.0, i + 1) * array4[i + 1];
            //    }
            //}

            //return array4;
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Transforms/copies an NX.Curve
        //
        // Parameters:
        //   xform:
        //     Transform to be applied
        //
        // Returns:
        //     A transformed copy of NX.Curve
        [Obsolete(nameof(NotImplementedException))]
        public static Curve __Copy(double[] xform)
        {
            //NXOpen.Curve nXOpenCurve = NXOpenCurve;
            //NXObject nXObject = NXObject.Wrap(nXOpenCurve.Tag);
            //NXObject nXObject2 = nXObject.Copy(xform);
            //TaggedObject nXOpenTaggedObject = nXObject2.NXOpenTaggedObject;
            //NXOpen.Curve nxopenCurve = (NXOpen.Curve)nXOpenTaggedObject;
            //return CreateCurve(nxopenCurve);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Transforms/copies an object
        //
        // Parameters:
        //   xform:
        //     Transform to be applied
        //
        // Returns:
        //     Output object
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The object is an edge. Edges cannot be copied.
        //
        //   T:System.ArgumentException:
        //     The object is a face. Faces cannot be copied.
        //
        //   T:System.ArgumentException:
        //     A feature cannot be copied unless all of its ancestors are copied too.
        //
        //   T:System.ArgumentException:
        //     A transform that does not preserve angles cannot be applied to a coordinate system.
        //
        // Remarks:
        //     To create a transformation, use the functions in the Snap.Geom.Transform class.


        internal static EqualityComparer<Point3d> __Point3dEqualityComparer(double tolerance = .001)
        {
            return new ComparerPoint3d(tolerance);
        }


        internal static Section CreateSection(params Point[] points)
        {
            var part = __work_part_;
            var section = part.Sections.CreateSection(ChainingTolerance, DistanceTolerance, AngleTolerance);
            section.AllowSelfIntersection(false);
            section.SetAllowedEntityTypes(Section.AllowTypes.CurvesAndPoints);
            var rules = CreateSelectionIntentRule(points);
            section.AddToSection(rules, points[0], null, null, _Point3dOrigin, Section.Mode.Create);
            return section;
        }


        internal static SelectionIntentRule[] CreateSelectionIntentRule(params ICurve[] icurves)
        {
            var list = new List<SelectionIntentRule>();

            for (var i = 0; i < icurves.Length; i++)
                if (icurves[i] is Curve curve)
                {
                    var curves = new Curve[1] { curve };
                    var item = __work_part_.ScRuleFactory.CreateRuleCurveDumb(curves);
                    list.Add(item);
                }
                else
                {
                    var edges = new Edge[1] { (Edge)icurves[i] };
                    var item2 = __work_part_.ScRuleFactory.CreateRuleEdgeDumb(edges);
                    list.Add(item2);
                }

            return list.ToArray();
        }

        //internal static SelectionIntentRule[] CreateSelectionIntentRule(params Point[] points)
        //{
        //    var array = new Point[points.Length];

        //    for (var i = 0; i < array.Length; i++)
        //        array[i] = points[i];

        //    var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
        //    return new SelectionIntentRule[1] { curveDumbRule };
        //}

        /// <summary>Creates an extrude feature</summary>
        /// <param name="section">The "section" to be extruded</param>
        /// <param name="axis">Extrusion direction (vector magnitude not significant)</param>
        /// <param name="extents">Extents of the extrusion (measured from input curves)</param>
        /// <param name="draftAngle">Draft angle, in degrees (positive angle gives larger sections in direction of<br />axis)</param>
        /// <param name="offset">If true, means that offset values are being provided</param>
        /// <param name="offsetValues">Offset distances for section curves</param>
        /// <param name="createSheet">If true, forces creation of a sheet body</param>
        /// <returns>An NX.Extrude object</returns>
        internal static Extrude __CreateExtrude(
            Section section,
            Vector3d axis,
            double[] extents,
            double draftAngle,
            bool offset,
            double[] offsetValues,
            bool createSheet)
        {
            var part = __work_part_;
            var extrudeBuilder = part.Features.CreateExtrudeBuilder(null);
            extrudeBuilder.DistanceTolerance = DistanceTolerance;
            extrudeBuilder.BooleanOperation.Type = BooleanOperation.BooleanType.Create;

            if (createSheet)
                extrudeBuilder.FeatureOptions.BodyType = FeatureOptions.BodyStyle.Sheet;

            extrudeBuilder.Limits.StartExtend.Value.RightHandSide = extents[0].ToString();
            extrudeBuilder.Limits.EndExtend.Value.RightHandSide = extents[1].ToString();
            extrudeBuilder.Offset.Option = Type.NoOffset;

            if (offset)
            {
                extrudeBuilder.Offset.Option = Type.NonsymmetricOffset;
                extrudeBuilder.Offset.StartOffset.RightHandSide = offsetValues[0].ToString();
                extrudeBuilder.Offset.EndOffset.RightHandSide = offsetValues[1].ToString();
            }

            var num = double.Parse(draftAngle.ToString());

            extrudeBuilder.Draft.DraftOption = System.Math.Abs(num) < 0.001
                ? SimpleDraft.SimpleDraftType.NoDraft
                : SimpleDraft.SimpleDraftType.SimpleFromProfile;

            extrudeBuilder.Draft.FrontDraftAngle.RightHandSide = $"{num}";
            extrudeBuilder.Section = section;
            var origin = new Point3d(30.0, 0.0, 0.0);
            var vector = new Vector3d(axis.X, axis.Y, axis.Z);
            var direction = part.Directions.CreateDirection(origin, vector, SmartObject.UpdateOption.WithinModeling);
            extrudeBuilder.Direction = direction;
            var extrude = (Extrude)extrudeBuilder.CommitFeature();
            extrudeBuilder.Destroy();
            return extrude;
        }


        /// <summary>Clips a ray to the model bounding box (the 1 KM cube), giving a line</summary>
        /// <param name="ray">The ray</param>
        /// <returns>Line end-points</returns>
        [Obsolete(nameof(NotImplementedException))]
        internal static Point3d[] __ClipRay(Geom.Curve.Ray ray)
        {
            var num = -1000000.0;
            var num2 = 1000000.0;
            var num3 = 1E-16;
            var num4 = 200.0 * MetersToPartUnits;
            var x = ray.Axis.X;
            var x2 = ray.Origin.X;
            if (System.Math.Abs(x) > num3)
            {
                var val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                var val2 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val2, num2);
            }

            x = ray.Axis.Y;
            x2 = ray.Origin.Y;
            if (System.Math.Abs(x) > num3)
            {
                var val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                var val3 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val3, num2);
            }

            x = ray.Axis.Z;
            x2 = ray.Origin.Z;
            if (System.Math.Abs(x) > num3)
            {
                var val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                var val4 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val4, num2);
            }

            var position = ray.Origin._Add(ray.Axis._Multiply(num));
            var position2 = ray.Origin._Add(ray.Axis._Multiply(num2));
            return new Point3d[2] { position, position2 };
            throw new NotImplementedException();
        }


        [Obsolete(nameof(NotImplementedException))]
        public static bool __IsReverseDirection(OffsetCurveBuilder builder, ICurve[] icurves, Point3d pos,
            Vector3d helpVector)
        {
            //int num = -1;
            //double num2 = -1.0;

            //for (int i = 0; i < icurves.Length; i++)
            //{
            //    double num3 = -1.0;
            //    num3 = Compute.Distance(pos, (NXObject)icurves[i]);

            //    if (num3 > num2)
            //    {
            //        num2 = num3;
            //        num = i;
            //    }
            //}

            //builder.ComputeOffsetDirection(seedPoint: (icurves[num] is NXOpen.Edge)
            //    ? Compute.ClosestPoints(pos, (NXOpen.Curve)icurves[num]).Point2
            //    : Compute.ClosestPoints(pos, (Edge)icurves[num]).Point2, seedEntity: icurves[num], offsetDirection: out Vector3d offsetDirection, startPoint: out Point3d _);

            //return helpVector._Multiply(offsetDirection) < 0.0;
            throw new NotImplementedException();
        }


        /// <summary>Copies an NX.Arc (with a null transform)</summary>
        /// <returns>A copy of the input arc</returns>
        /// <remarks>
        ///     <para>
        ///         The new arc will be on the same layer as the original one.
        ///     </para>
        /// </remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc _Copy(this Arc arc)
        {
            var xform = Transform.CreateTranslation(0.0, 0.0, 0.0);
            return arc._Copy(xform);
        }

        /// <summary>Transforms/copies an NX.Arc</summary>
        /// <param name="xform">Transform to be applied</param>
        /// <returns>A transformed copy of NX.Arc</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     The transform would convert the arc to an ellipse. Please use Curve.Copy
        ///     instead
        /// </exception>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc _Copy(this Arc arc, Transform xform)
        {
            //NXOpen.TaggedObject taggedObject = ((NXObject)NXOpenArc).Copy(xform);
            //NXOpen.Ellipse ellipse = taggedObject as NXOpen.Ellipse;
            //if (ellipse != null)
            //{
            //    session_.delete_objects(ellipse);
            //    throw new ArgumentException("The transform would convert the arc to an ellipse. Please use Curve.Copy instead");
            //}

            //return (NXOpen.Arc)taggedObject;
            throw new NotImplementedException();
        }

        /// <summary>Divide an arc at an array of parameter values</summary>
        /// <param name="parameters">The parameter values at which the arc should be divided</param>
        /// <returns>An array of <see cref="NXOpen.Arc">NXOpen.Arc</see> objects</returns>
        /// <remarks>The function will create new arcs by dividing the original one.</remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc[] Divide(params double[] parameters)
        {
            //Curve[] curveArray = base.Divide(parameters);
            //return ArcArray(curveArray);
            throw new NotImplementedException();
        }

        /// <summary>Divide an arc at an intersection with another curve</summary>
        /// <param name="boundingCurve">Bounding curve to be used to divide the given arc</param>
        /// <param name="helpPoint">A point near the desired dividing point</param>
        /// <returns>An array of two <see cref="NXOpen.Arc">NXOpen.Arc</see> objects</returns>
        /// <remarks>The function will create two new arcs by dividing the original one.</remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc[] Divide(ICurve boundingCurve, Point3d helpPoint)
        {
            //Curve[] curveArray = base.Divide(boundingCurve, helpPoint);
            //return ArcArray(curveArray);

            throw new NotImplementedException();
        }

        /// <summary>Divide an arc at an intersection with a given face</summary>
        /// <param name="face">A face to be used to divide the given arc</param>
        /// <param name="helpPoint">A point near the desired dividing point</param>
        /// <returns>An array of two <see cref="NXOpen.Arc">NXOpen.Arc</see> objects</returns>
        /// <remarks>The function will create two new arcs by dividing the original one.</remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc[] Divide(Face face, Point3d helpPoint)
        {
            //Curve[] curveArray = base.Divide(face, helpPoint);
            //return ArcArray(curveArray);
            throw new NotImplementedException();
        }

        /// <summary>Divide an arc at an intersection with a given plane</summary>
        /// <param name="geomPlane">A plane to be used to divide the given arc</param>
        /// <param name="helpPoint">A point near the desired dividing point</param>
        /// <returns>An array of two <see cref="NXOpen.Arc">NXOpen.Arc</see> objects</returns>
        /// <remarks>The function will create two new arcs by dividing the original one</remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc[] Divide(Surface.Plane geomPlane, Point3d helpPoint)
        {
            //Curve[] curveArray = base.Divide(geomPlane, helpPoint);
            //return ArcArray(curveArray);
            throw new NotImplementedException();
        }

        /// To avoid having four identical copies of the same code
        [Obsolete(nameof(NotImplementedException))]
        private static Arc[] ArcArray(Curve[] curveArray)
        {
            var array = new Arc[curveArray.Length];
            for (var i = 0; i < curveArray.Length; i++) array[i] = (Arc)curveArray[i];

            return array;
            throw new NotImplementedException();
        }

        #region Builder

        public static Destroyer _using_builder(this Builder builder)
        {
            return new Destroyer(builder);
        }

        #endregion

        #region ComponentAssembly

        public static void __Check0(ComponentAssembly comp_ass)
        {
            //comp_ass.ActiveArrangement
            //comp_ass.AddComponent
            //comp_ass.AddComponents
            //comp_ass.Arrangements
            //comp_ass.CloseComponents
            //comp_ass.CopyComponents
            //comp_ass.DeleteMatingConditions
            //comp_ass.GetNonGeometricState
            //comp_ass.GetSuppressedState
            //comp_ass.GetSuppressionExpression
            //comp_ass.MapComponentFromParent
            //comp_ass.MapComponentsFromSubassembly
            //comp_ass.MoveComponent
            //comp_ass.OpenComponents
            //comp_ass.Positioner
            //comp_ass.ReleaseSuppression
            //comp_ass.RemoveComponent
            //comp_ass.ReorderChildrenOfParent
            //comp_ass.ReorderComponents
            //comp_ass.ReplaceReferenceSet
            //comp_ass.ReplaceReferenceSetInOwners
            //comp_ass.RootComponent
            //comp_ass.SetEmptyRefset
            //comp_ass.SetEntirePartRefset
            //comp_ass.SetNonGeometricState
            //comp_ass.ShowComponentsInIsolateView
            //comp_ass.SubstituteComponent
            //comp_ass.SuppressComponents
            //comp_ass.UnsuppressComponents
        }

        #endregion

        #region Conic

        public static void Temp(this Conic obj)
        {
            //obj.CenterPoint
            //obj.GetOrientation
            //obj.IsClosed
            //obj.IsReference
            //obj.Matrix
            //obj.ProtectFromDelete
            //obj.ReleaseDeleteProtection
            //obj.RotationAngle
            //obj.SetOrientation
            //obj.SetParameters
        }

        #endregion

        #region Ellipse

        [Obsolete]
        public static Arc Help()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Int

        public static string _PadInt(this int integer, int padLength)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException(nameof(integer), @"You cannot pad a negative integer.");

            if (padLength < 1)
                throw new ArgumentOutOfRangeException(nameof(padLength),
                    @"You cannot have a pad length of less than 1.");

            var integerString = $"{integer}";

            var counter = 0;

            while (integerString.Length < padLength)
            {
                integerString = $"0{integerString}";

                if (counter++ > 100)
                    throw new TimeoutException(nameof(_PadInt));
            }

            return integerString;
        }

        #endregion

        #region NXMatrix

        public static Matrix3x3 __Element(this NXMatrix obj)
        {
            return obj.Element;
        }

        #endregion

        #region Parabola

        [Obsolete]
        public static Parabola _Copy(this Parabola parabola)
        {
            //if (parabola.IsOccurrence)
            //    throw new ArgumentException($@"Cannot copy {nameof(parabola)} that is an occurrence.", nameof(parabola));

            //NXOpen.Point3d centerPosition = parabola.CenterPoint;

            //double focalLength = parabola.FocalLength;

            //double minimumDy = parabola.MinimumDY;

            //double maximumDy = parabola.MaximumDY;

            //double rotationAngle = parabola.RotationAngle;

            //NXOpen.NXMatrix matrix = parabola.Matrix._Copy();

            //return parabola._OwningPart().Curves.CreateParabola(centerPosition, focalLength, minimumDy, maximumDy, rotationAngle, matrix);

            throw new NotImplementedException();
        }

        #endregion

        #region ScCollector

        public static void Temp(this ScCollector scCollector)
        {
            scCollector.CopyCollector();
            //scCollector.Destroy
            //scCollector.GetMultiComponent
            //scCollector.GetNonFeatureMode
            //scCollector.GetObjects
            //scCollector.GetObjectsSortedById
            //scCollector.GetRules
            //scCollector.RemoveMissingParents
            //scCollector.RemoveRule
            //scCollector.RemoveRules
            //scCollector.ReplaceRules
            //scCollector.SetInterpart
            //scCollector.SetMultiComponent
            //scCollector.SetNonFeatureMode
        }

        #endregion

        #region SmartObject

        //public void RemoveParameters()

        //public void ReplaceParameters(SmartObject otherSo)

        //public void Evaluate


        //public void SetVisibility(VisibilityOption visibility)


        //public void ProtectFromDelete


        //public void ReleaseDeleteProtection


        public static void Temp(this SmartObject obj)
        {
            //obj.CenterPoint
            //obj.GetOrientation
            //obj.IsClosed
            //obj.IsReference
            //obj.Matrix
            //obj.ProtectFromDelete
            //obj.ReleaseDeleteProtection
            //obj.RotationAngle
            //obj.SetOrientation
            //obj.SetParameters
        }

        #endregion

        #region Spline

        public static Spline _Copy(this Spline spline)
        {
            //if (spline.OwningPart.Tag != _WorkPart.Tag)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)}.", nameof(spline));

            //if (spline.IsOccurrence)
            //    throw new ArgumentException($@"Cannot copy {nameof(spline)} that is an occurrence.", nameof(spline));

            //return new Spline_(spline.Tag).Copy();
            throw new NotImplementedException();
        }

        #endregion

        #region TreeNode

        public static TreeNode _SetText(this TreeNode node, string text)
        {
            node.Text = text;
            return node;
        }

        #endregion

        public class ComparerPoint3d : EqualityComparer<Point3d>
        {
            private readonly double __tolerance;

            public ComparerPoint3d(double tolerance = .001)
            {
                __tolerance = tolerance;
            }

            public override bool Equals(Point3d x, Point3d y)
            {
                return Math.Abs(x.X - y.X) < __tolerance
                       && Math.Abs(x.Y - y.Y) < __tolerance
                       && Math.Abs(x.Z - y.Z) < __tolerance;
            }

            public override int GetHashCode(Point3d obj)
            {
                var hash = 17;

                hash = hash * 23 + obj.X.GetHashCode();
                hash = hash * 23 + obj.Y.GetHashCode();
                hash = hash * 23 + obj.Z.GetHashCode();

                return hash;
            }
        }


        #region WCS

        public static Matrix3x3 __Orientation(this WCS wcs)
        {
            wcs.CoordinateSystem.GetDirections(out var xDir, out var yDir);
            return xDir._ToMatrix3x3(yDir);
        }


        public static void __Orientation(this WCS wcs, Matrix3x3 matrix)
        {
            wcs.SetOriginAndMatrix(wcs.Origin, matrix);
        }

        public static Vector3d __AxisX(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisX();
        }

        public static Vector3d __AxisY(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisY();
        }

        public static Vector3d __AxisZ(this WCS wcs)
        {
            return wcs.CoordinateSystem.__Orientation().Element._AxisZ();
        }

        #endregion

        #region DatumCsys

        public static DatumPlane __DatumPlaneXY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[0]);
        }

        public static DatumPlane __DatumPlaneXZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane __DatumPlaneYZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[1]);
        }

        public static Vector3d __Vector3dX(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_.__GetTaggedObject(daxes[0]);
            return axis.Direction;
        }

        public static Vector3d __Vector3dY(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_.__GetTaggedObject(daxes[1]);
            return axis.Direction;
        }

        public static Vector3d __Vector3dZ(this DatumCsys datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out var daxes, out _);
            var axis = (DatumAxis)session_.__GetTaggedObject(daxes[2]);
            return axis.Direction;
        }

        #endregion

        #region DatumAxisFeature

        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature _Mirror(
            this DatumAxisFeature original,
            Surface.Plane plane)
        {
            _ = original.DatumAxis.__Origin().__Mirror(plane);
            _ = original.DatumAxis.Direction._Mirror(plane);
            _ = original.__OwningPart().Features.CreateDatumAxisBuilder(null);

            //using(session_.using_builder_destroyer(builder))
            //{
            //    builder.
            //}

            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature _Mirror(
            this DatumAxisFeature datumAxisFeature,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Vector3d

        /// <summary>Map a vector from Absolute coordinates to Work coordinates</summary>
        /// <param name="absVector">The components of the given vector wrt the Absolute Coordinate System (ACS)</param>
        /// <returns>The components of the given vector wrt the Work Coordinate System (WCS)</returns>
        public static Vector3d MapAcsToWcs(Vector3d absVector)
        {
            var axisX = __display_part_.WCS.__AxisX();
            var axisY = __display_part_.WCS.__AxisY();
            var axisZ = __display_part_.WCS.__AxisZ();
            var _x = axisX._Multiply(absVector);
            var _y = axisY._Multiply(absVector);
            var _z = axisZ._Multiply(absVector);
            return new Vector3d(_x, _y, _z);
        }

        /// <summary>
        ///     Map a vector from one coordinate system to another
        /// </summary>
        /// <param name="inputVector">The components of the given vector wrt the input coordinate system</param>
        /// <param name="inputCsys">The input coordinate system</param>
        /// <param name="outputCsys">The output coordinate system</param>
        /// <returns>The components of the given vector wrt the output coordinate system</returns>
        public static Vector3d __MapCsysToCsys(this Vector3d inputVector,
            CartesianCoordinateSystem inputCsys, CartesianCoordinateSystem outputCsys)
        {
            var axisX = inputCsys.__Orientation().Element._AxisX();
            var axisY = inputCsys.__Orientation().Element._AxisY();
            var axisZ = inputCsys.__Orientation().Element._AxisZ();
            var _x = inputVector.X.__Multiply(axisX);
            var _y = inputVector.Y.__Multiply(axisY);
            var _z = inputVector.Z.__Multiply(axisZ);
            var vector = _x.__Add(_y, _z);
            var x = vector._Multiply(outputCsys.__Orientation().Element._AxisX());
            var y = vector._Multiply(outputCsys.__Orientation().Element._AxisY());
            var z = vector._Multiply(outputCsys.__Orientation().Element._AxisZ());
            return new Vector3d(x, y, z);
        }

        public static Vector3d __Add(this Vector3d vector, params Vector3d[] vectors)
        {
            var result = vector._Copy();

            foreach (var v in vectors)
                result = result._Add(v);

            return result;
        }


        /// <summary>Calculates the cross product (vector product) of two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Cross product</returns>
        /// <remarks>
        ///     <para>
        ///         As is well known, order matters: Cross(u,v) = - Cross(v,u)
        ///     </para>
        /// </remarks>
        public static Vector3d Cross(this Vector3d u, Vector3d v)
        {
            return new Vector3d(u.Y * v.Z - u.Z * v.Y, u.Z * v.X - u.X * v.Z, u.X * v.Y - u.Y * v.X);
        }

        /// <summary>Calculates the unitized cross product (vector product) of two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Unitized cross product</returns>
        /// <remarks>
        ///     <para>
        ///         If the cross product is the zero vector, then each component
        ///         of the returned vector will be NaN (not a number).
        ///     </para>
        /// </remarks>
        public static Vector3d _UnitCross(this Vector3d u, Vector3d v)
        {
            return u.__Cross(v)._Unit();
        }

        /// <summary>Unitizes a given vector</summary>
        /// <param name="u">Vector to be unitized</param>
        /// <returns>Unit vector in same direction</returns>
        /// <remarks>
        ///     <para>
        ///         If the input is the zero vector is zero, then each component
        ///         of the returned vector will be NaN (not a number).
        ///     </para>
        /// </remarks>
        public static Vector3d _Unit(this Vector3d u)
        {
            var num = 1.0 / u._Norm();
            return new Vector3d(num * u.X, num * u.Y, num * u.Z);
        }

        /// <summary>Calculates the norm squared (length squared) of a vector</summary>
        /// <param name="u">The vector</param>
        /// <returns>Norm (length) squared of vector</returns>
        public static double _Norm2(this Vector3d u)
        {
            return u.X * u.X + u.Y * u.Y + u.Z * u.Z;
        }

        public static Vector3d _Divide(this Vector3d vector, double divisor)
        {
            return new Vector3d(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }

        /// <summary>Calculates the norm (length) of a vector</summary>
        /// <param name="u">The vector</param>
        /// <returns>Norm (length) of vector</returns>
        public static double _Norm(this Vector3d u)
        {
            return Math.Sqrt(u.X * u.X + u.Y * u.Y + u.Z * u.Z);
        }

        public static Matrix3x3 _ToMatrix3x3(this Vector3d axisZ)
        {
            var u = !(System.Math.Abs(axisZ.X) < System.Math.Abs(axisZ.Y))
                ? new Vector3d(0.0, 1.0, 0.0)
                : new Vector3d(1.0, 0.0, 0.0);

            axisZ = axisZ._Unit();
            var v = u.__Cross(axisZ)._Unit();
            var vector = axisZ.__Cross(v)._Unit();
            return new Matrix3x3
            {
                Xx = v.X,
                Xy = v.Y,
                Xz = v.Z,
                Yx = vector.X,
                Yy = vector.Y,
                Yz = vector.Z,
                Zx = axisZ.X,
                Zy = axisZ.Y,
                Zz = axisZ.Z
            };
        }

        public static Vector3d _Mirror(this Vector3d vector, Surface.Plane plane)
        {
            var transform = Transform.CreateReflection(plane);
            return vector._Copy(transform);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Vector3d _Mirror(
            this Vector3d vector,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static Matrix3x3 _ToMatrix3x3(this Vector3d xDir, Vector3d yDir)
        {
            var array = new double[9];
            ufsession_.Mtx3.Initialize(xDir._ToArray(), yDir._ToArray(), array);
            return array._ToMatrix3x3();
        }

        public static Matrix3x3 _ToMatrix3x3(this Vector3d axisX, Vector3d axisY,
            Vector3d axisZ)
        {
            var element = default(Matrix3x3);
            element.Xx = axisX.X;
            element.Xy = axisX.Y;
            element.Xz = axisX.Z;
            element.Yx = axisY.X;
            element.Yy = axisY.Y;
            element.Yz = axisY.Z;
            element.Zx = axisZ.X;
            element.Zy = axisZ.Y;
            element.Zz = axisZ.Z;
            return element;
        }

        public static Vector3d _AskPerpendicular(this Vector3d vec)
        {
            var __vec_perp = new double[3];
            ufsession_.Vec3.AskPerpendicular(vec._ToArray(), __vec_perp);
            return new Vector3d(__vec_perp[0], __vec_perp[1], __vec_perp[2]);
        }

        public static Vector3d _Add(this Vector3d vec0, Vector3d vec1)
        {
            return new Vector3d(vec0.X + vec1.X, vec0.Y + vec1.Y, vec0.Z + vec1.Z);
        }

        public static Vector3d _Subtract(this Vector3d vec0, Vector3d vec1)
        {
            var negate = vec1._Negate();
            return vec0._Add(negate);
        }

        public static Vector3d _Negate(this Vector3d vec0)
        {
            return new Vector3d(-vec0.X, -vec0.Y, -vec0.Z);
        }

        public static double[] _Array(this Vector3d vector)
        {
            return new[] { vector.X, vector.Y, vector.Z };
        }

        /// <summary>Calculates the angle in degrees between two vectors</summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>The angle, theta, in degrees, where 0 ≤ theta ≤ 180</returns>
        public static double _Angle(this Vector3d u, Vector3d v)
        {
            var val = u._Unit()._Multiply(v._Unit());
            val = System.Math.Min(1.0, val);
            val = System.Math.Max(-1.0, val);
            return System.Math.Acos(val) * 180.0 / System.Math.PI;
        }

        public static bool _IsPerpendicularTo(this Vector3d vec1, Vector3d vec2, double tolerance = .0001)
        {
            ufsession_.Vec3.IsPerpendicular(vec1._ToArray(), vec2._ToArray(), tolerance, out var result);
            return result == 1;
        }

        public static double[] _ToArray(this Vector3d vector3d)
        {
            return new[] { vector3d.X, vector3d.Y, vector3d.Z };
        }

        //
        // Summary:
        //     Transforms/copies a vector
        //
        // Parameters:
        //   xform:
        //     The transformation to apply
        //
        // Returns:
        //     A transformed copy of the original input vector
        public static Vector3d _Copy(this Vector3d vector, Transform xform)
        {
            var matrix = xform.Matrix;
            var x = vector.X;
            var y = vector.Y;
            var z = vector.Z;
            var x2 = x * matrix[0] + y * matrix[1] + z * matrix[2];
            var y2 = x * matrix[4] + y * matrix[5] + z * matrix[6];
            var z2 = x * matrix[8] + y * matrix[9] + z * matrix[10];
            return new Vector3d(x2, y2, z2);
        }

        /// <summary>Copies a vector</summary>
        /// <param name="vector">The vector copy from</param>
        /// <returns>A copy of the original input vector</returns>
        public static Vector3d _Copy(this Vector3d vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        public static bool _IsEqual(this Vector3d vec0, Vector3d vec1, double tolerance = .001)
        {
            ufsession_.Vec3.IsEqual(vec0._ToArray(), vec1._ToArray(), tolerance, out var is_equal);

            switch (is_equal)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(is_equal);
            }
        }

        public static bool _IsEqualTo(this Vector3d vector1, Vector3d vector2, double tolerance = .01)
        {
            // Compares the two vectors. If they are equal, then {isEqual} == 1, else {isEqual} == 0.
            ufsession_.Vec3.IsEqual(vector1._ToArray(), vector2._ToArray(), tolerance, out var isEqual);

            switch (isEqual)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(isEqual);
            }
        }

        /// <summary>
        ///     Returns a 3x3 matrix formed from two input 3D vectors. The two<br />
        ///     input vectors are normalized and the y-direction vector is made<br />
        ///     orthogonal to the x-direction vector before taking the cross product<br />
        ///     (x_vec X y_vec) to generate the z-direction vector.
        /// </summary>
        /// <param name="x_vec">Vector for the X-direction of matrix</param>
        /// <param name="y_vec">Vector for theYX-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 _Initialize(this Vector3d x_vec, Vector3d y_vec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.Initialize(x_vec._ToArray(), y_vec._ToArray(), mtx);
            return mtx._ToMatrix3x3();
        }

        /// <summary>
        ///     Returns a 3x3 matrix with the given X-direction vector and having<br />
        ///     arbitrary Y- and Z-direction vectors.
        /// </summary>
        /// <param name="x_vec">Vector for the X-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 _InitializeX(Vector3d x_vec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.InitializeX(x_vec._ToArray(), mtx);
            return mtx._ToMatrix3x3();
        }

        /// <summary>
        ///     Returns a 3x3 matrix with the given Z-direction vector and having<br />
        ///     arbitrary X- and Y-direction vectors.
        /// </summary>
        /// <param name="z_vec">Vector for the Z-direction of matrix</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x3 _InitializeZ(Vector3d z_vec)
        {
            var mtx = new double[9];
            ufsession_.Mtx3.InitializeZ(z_vec._ToArray(), mtx);
            return mtx._ToMatrix3x3();
        }

        public static Point3d _Add(this Vector3d vector, Point3d point)
        {
            return point._Add(vector);
        }

        /// <summary>Map a vector from Work coordinates to Absolute coordinates</summary>
        /// <param name="workVector">The components of the given vector wrt the Work Coordinate System (WCS)</param>
        /// <returns>The components of the given vector wrt the Absolute Coordinate System (ACS)</returns>
        /// <remarks>
        ///     <para>
        ///         If you are given vector components relative to the WCS, then you will need to
        ///         use this function to "map" them to the ACS before using them in SNAP functions.
        ///     </para>
        /// </remarks>
        public static Vector3d MapWcsToAcs(Vector3d workVector)
        {
            var axisX = __display_part_.WCS.__AxisX();
            var axisY = __display_part_.WCS.__AxisY();
            var axisZ = __display_part_.WCS.__AxisZ();
            return axisX._Multiply(workVector.X)._Add(axisY._Multiply(workVector.Y))
                ._Add(axisZ._Multiply(workVector.Z));
        }

        public static Vector3d _Multiply(this Vector3d vector3d, double scale)
        {
            var scaled_vec = new double[3];
            ufsession_.Vec3.Scale(scale, vector3d._ToArray(), scaled_vec);
            return scaled_vec._ToVector3d();
        }

        public static double _Multiply(this Vector3d __vec0, Vector3d __vec1)
        {
            return __vec0.X * __vec1.X + __vec0.Y * __vec1.Y + __vec0.Z * __vec1.Z;
        }

        public static Matrix3x3 MapWcsToAcs(Matrix3x3 orientation)
        {
            var axisX = MapWcsToAcs(orientation._AxisX());
            var axisY = MapWcsToAcs(orientation._AxisY());
            return axisX._ToMatrix3x3(axisY);
        }

        public static Vector3d _MirrorMap(this Vector3d vector, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            var origin = fromComponent._Origin();
            var orientation = fromComponent._Orientation();
            __display_part_.WCS.SetOriginAndMatrix(origin, orientation);
            var newStart = MapWcsToAcs(vector);
            __display_part_.WCS.SetOriginAndMatrix(toComponent._Origin(), toComponent._Orientation());
            return MapAcsToWcs(newStart);
        }

        public static Matrix3x3 _Mtx3_Initialize(Vector3d x_vec, Vector3d y_vec)
        {
            var matrix = new double[9];
            ufsession_.Mtx3.Initialize(x_vec._ToArray(), y_vec._ToArray(), matrix);
            return matrix._ToMatrix3x3();
        }

        public static Vector3d __Cross(this Vector3d vec1, Vector3d vec2)
        {
            var cross_product = new double[3];
            ufsession_.Vec3.Cross(vec1._ToArray(), vec2._ToArray(), cross_product);
            return cross_product._ToVector3d();
        }

        #endregion

        #region Point3d

        /// <summary>
        ///     Creates a cone feature, given cone base position, direction, base diameter, top<br />
        ///     diameter and height
        /// </summary>
        /// <param name="axisPoint">The cone base position of base arc</param>
        /// <param name="direction">The cone direction vector from base to top</param>
        /// <param name="baseDiameter">The cone base diameter. The cone base diameter cannot equal its top diameter</param>
        /// <param name="topDiameter">The cone top diameter. The cone top diameter cannot equal its base diameter</param>
        /// <param name="height">The cone height</param>
        /// <returns>An NX.Cone feature object</returns>
        internal static Cone __CreateConeFromDiametersHeight(
            this BasePart basePart,
            Point3d axisPoint,
            Vector3d direction,
            double baseDiameter,
            double topDiameter,
            double height)
        {
            basePart.__AssertIsWorkPart();
            var builder = basePart.Features.CreateConeBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                builder.Type = ConeBuilder.Types.DiametersAndHeight;
                var direction2 = basePart.Directions.CreateDirection(axisPoint, direction,
                    SmartObject.UpdateOption.WithinModeling);
                var axis = builder.Axis;
                axis.Direction = direction2;
                axis.Point = basePart.Points.CreatePoint(axisPoint);
                builder.BaseDiameter.RightHandSide = baseDiameter.ToString();
                builder.TopDiameter.RightHandSide = topDiameter.ToString();
                builder.Height.RightHandSide = height.ToString();
                return (Cone)builder.Commit();
            }
        }

        /// <summary>Constructs a NXOpen.Arc parallel to the XY-plane</summary>
        /// <param name="center">Center point (in absolute coordinates)</param>
        /// <param name="radius">Radius</param>
        /// <param name="angle1">Start angle (in degrees)</param>
        /// <param name="angle2">End angle (in degrees)</param>
        /// <returns>A <see cref="NXOpen.Arc">NXOpen.Arc</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         The arc will have its center at the given point, and will be parallel to the XY-plane.
        ///     </para>
        ///     <para>
        ///         If the center point does not lie in the XY-plane, then the arc will not, either.
        ///     </para>
        /// </remarks>
        [Obsolete]
        public static Arc __Arc(Point3d center, double radius, double angle1, double angle2)
        {
            //NXOpen.Vector3d axisX = _Vector3dX();
            //NXOpen.Vector3d axisY = _Vector3dY();
            //return CreateArc(center, axisX, axisY, radius, angle1, angle2);
            throw new NotImplementedException();
        }

        /// <summary>Creates an NX.Block from origin, matrix, xLength, yLength, zLength</summary>
        /// <param name="origin">The corner-point of the block (in absolute coordinates</param>
        /// <param name="matrix">Orientation (see remarks)</param>
        /// <param name="xLength">Length in x-direction</param>
        /// <param name="yLength">Length in y-direction</param>
        /// <param name="zLength">Length in z-direction</param>
        /// <returns>An NX.Block object</returns>
        internal static Block __CreateBlock(
            this BasePart basePart,
            Point3d origin,
            Matrix3x3 matrix,
            double xLength,
            double yLength,
            double zLength)
        {
            basePart.__AssertIsWorkPart();
            var wcsOrientation = __display_part_.WCS.__Orientation();
            __display_part_.WCS.__Orientation(matrix);
            var builder = __work_part_.Features.CreateBlockFeatureBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                builder.SetOriginAndLengths(origin, xLength.ToString(), yLength.ToString(), zLength.ToString());
                __display_part_.WCS.__Orientation(wcsOrientation);
                return (Block)builder.Commit();
            }
        }

        /// <summary>Constructs a circle from center, normal, radius</summary>
        /// <param name="center">Center point (in absolute coordinates)</param>
        /// <param name="axisZ">Unit vector normal to plane of circle</param>
        /// <param name="radius">Radius</param>
        /// <returns>A <see cref="T:NXOpen.Arc">NXOpen.Arc</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         This function gives you no control over how the circle is parameterized. You don't
        ///         provide the X-axis, so you can't specify at which point the angle parameter is zero
        ///     </para>
        /// </remarks>
        public static Arc __CreateCircle(this BasePart basePart, Point3d center,
            Vector3d axisZ, double radius)
        {
            var orientation = axisZ._ToMatrix3x3();
            var axisX = orientation._AxisX();
            var axisY = orientation._AxisY();
            return basePart.__CreateArc(center, axisX, axisY, radius, 0.0, 360.0);
        }

        /// <summary>Creates a circle through three points</summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <returns>Circle (360 degrees) passing through the 3 points</returns>
        [Obsolete]
        public static Arc __CreateCircle(this BasePart basePart, Point3d p1, Point3d p2,
            Point3d p3)
        {
            basePart.__AssertIsWorkPart();
            var vector = p2._Subtract(p1);
            var vector2 = p3._Subtract(p1);
            var num = vector._Multiply(vector);
            var num2 = vector._Multiply(vector2);
            var num3 = vector2._Multiply(vector2);
            var num4 = num * num3 - num2 * num2;
            var num5 = (num * num3 - num2 * num3) / (2.0 * num4);
            var num6 = (num * num3 - num * num2) / (2.0 * num4);
            var vector3 = vector._Multiply(num5)._Add(vector2._Multiply(num6));
            var center = vector3._Add(p1);
            var radius = vector3._Norm();
            var orientation = vector.__Cross(vector2)._ToMatrix3x3();
            var axisX = orientation._AxisX();
            var axisY = orientation._AxisY();
            return basePart.__CreateArc(center, axisX, axisY, radius, 0.0, 360.0);
        }


        /// <summary>Creates a coordinate system</summary>
        /// <param name="origin"></param>
        /// <param name="matrix"></param>
        /// <returns>An NX.CoordinateSystem object</returns>
        internal static CoordinateSystem __CreateCoordinateSystem(this BasePart basePart,
            Point3d origin, NXMatrix matrix)
        {
            basePart.__AssertIsWorkPart();
            var array = origin._ToArray();
            ufsession_.Csys.CreateCsys(array, matrix.Tag, out var csys_id);
            var objectFromTag = (NXObject)session_.__GetTaggedObject(csys_id);
            var coordinateSystem = (CoordinateSystem)objectFromTag;
            return coordinateSystem;
        }

        /// <summary>Constructs a CoordinateSystem from an origin and three axis vectors</summary>
        /// <param name="origin">Origin position</param>
        /// <param name="axisX">X axis</param>
        /// <param name="axisY">Y axis</param>
        /// <param name="axisZ">Z axis</param>
        /// <remarks>
        ///     Assumes that the three axis vectors are orthogonal, and does not perform any<br />
        ///     checking.
        /// </remarks>
        /// <returns>An NX.CoordinateSystem object</returns>
        internal static CoordinateSystem __CreateCoordinateSystem(
            this BasePart basePart,
            Point3d origin,
            Vector3d axisX,
            Vector3d axisY,
            Vector3d axisZ)
        {
            basePart.__AssertIsWorkPart();
            var array = origin._ToArray();
            var matrix = __work_part_.NXMatrices.Create(axisX._ToMatrix3x3(axisY, axisZ));
            var nXOpenTag = matrix.Tag;
            ufsession_.Csys.CreateCsys(array, nXOpenTag, out var csys_id);
            var objectFromTag = (NXObject)session_.__GetTaggedObject(csys_id);
            var coordinateSystem = (CoordinateSystem)objectFromTag;
            return coordinateSystem;
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __Mirror(
            this Point3d point,
            Surface.Plane plane)
        {
            var transform = Transform.CreateReflection(plane);
            return point.__Copy(transform);
        }

        //
        // Summary:
        //     Copies a position
        //
        // Returns:
        //     A copy of the original input position
        public static Point3d __Copy(this Point3d point)
        {
            return new Point3d(point.X, point.Y, point.Z);
        }

        //
        // Summary:
        //     Transforms/copies a position
        //
        // Parameters:
        //   xform:
        //     The transformation to apply
        //
        // Returns:
        //     A transformed copy of the original input position
        //
        // Remarks:
        //     To create a transformation, use the functions in the Snap.Geom.Transform class.
        public static Point3d __Copy(this Point3d point, Transform xform)
        {
            var matrix = xform.Matrix;
            var x = point.X;
            var y = point.Y;
            var z = point.Z;
            var x2 = x * matrix[0] + y * matrix[1] + z * matrix[2] + matrix[3];
            var y2 = x * matrix[4] + y * matrix[5] + z * matrix[6] + matrix[7];
            var z2 = x * matrix[8] + y * matrix[9] + z * matrix[10] + matrix[11];
            return new Point3d(x2, y2, z2);
        }


        //public static Point3d __MapCsysToCsys(
        //    this Point3d inputCoords,
        //    CoordinateSystem inputCsys,
        //    CoordinateSystem outputCsys)
        //{
        //    var origin = inputCsys.Origin;
        //    var axisX = inputCsys.__Orientation().Element._AxisX();
        //    var axisY = inputCsys.__Orientation().Element._AxisY();
        //    var axisZ = inputCsys.__Orientation().Element._AxisZ();

        //    var _x = axisX._Multiply(inputCoords.X);
        //    var _y = axisY._Multiply(inputCoords.Y);
        //    var _z = axisZ._Multiply(inputCoords.Z);

        //    var vector = origin._Add(_x)._Add(_y)._Add(_z)._Subtract(outputCsys.Origin);
        //    var x = vector._Multiply(outputCsys.__Orientation().Element._AxisX());
        //    var y = vector._Multiply(outputCsys.__Orientation().Element._AxisY());
        //    var z = vector._Multiply(outputCsys.__Orientation().Element._AxisZ());
        //    return new Point3d(x, y, z);
        //}


        [Obsolete(nameof(NotImplementedException))]
        public static Point3d _Mirror(
            this Point3d point,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }


        public static Point3d _MirrorMap(this Point3d position, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent._Origin(), fromComponent._Orientation());

            var newStart = MapWcsToAcs(position);

            __display_part_.WCS.SetOriginAndMatrix(toComponent._Origin(), toComponent._Orientation());

            return MapAcsToWcs(newStart);
        }

        public static int __ToHashCode(this Point3d p)
        {
            var hash = 17;

            hash = hash * 23 + p.X.GetHashCode();
            hash = hash * 23 + p.Y.GetHashCode();
            hash = hash * 23 + p.Z.GetHashCode();

            return hash;
        }

        public static Point3d __MapCsysToCsys(
            this Point3d inputCoords,
            CoordinateSystem inputCsys,
            CoordinateSystem outputCsys)
        {
            var origin = inputCsys.Origin;
            var axisX = inputCsys.__Orientation().Element._AxisX();
            var axisY = inputCsys.__Orientation().Element._AxisY();
            var axisZ = inputCsys.__Orientation().Element._AxisZ();
            var _x = axisX._Multiply(inputCoords.X);
            var _y = axisY._Multiply(inputCoords.Y);
            var _z = axisZ._Multiply(inputCoords.Z);
            var position = origin._Add(_x)._Add(_y)._Add(_z);
            var vector = position._Subtract(outputCsys.Origin);
            var x = vector._Multiply(outputCsys.__Orientation().Element._AxisX());
            var y = vector._Multiply(outputCsys.__Orientation().Element._AxisY());
            var z = vector._Multiply(outputCsys.__Orientation().Element._AxisZ());
            return new Point3d(x, y, z);
        }

        public static Point3d _Add(this Point3d point, Vector3d vector)
        {
            var x = point.X + vector.X;
            var y = point.Y + vector.Y;
            var z = point.Z + vector.Z;
            return new Point3d(x, y, z);
        }


        public static Vector3d _Subtract(this Point3d start, Point3d end)
        {
            var x = start.X - end.X;
            var y = start.Y - end.Y;
            var z = start.Z - end.Z;
            return new Vector3d(x, y, z);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d _Subtract(this Point3d start, Vector3d end)
        {
            //double x = start.X - end.X;
            //double y = start.Y - end.Y;
            //double z = start.Z - end.Z;
            //return new NXOpen.Vector3d(x, y, z);
            throw new NotImplementedException();
        }

        /// <summary>Creates a datum axis from a position and a vector</summary>
        /// <param name="origin">The base point of the datum axis</param>
        /// <param name="direction">The direction vector of the datum axis (length doesn't matter)</param>
        /// <returns>An NX.DatumAxis object</returns>
        internal static DatumAxisFeature __CreateDatumAxis(this BasePart basePart,
            Point3d origin, Vector3d direction)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var datumAxisBuilder = part.Features.CreateDatumAxisBuilder(null);

            using (session_.using_builder_destroyer(datumAxisBuilder))
            {
                datumAxisBuilder.Type = DatumAxisBuilder.Types.PointAndDir;
                var vector = part.Directions.CreateDirection(origin, direction,
                    SmartObject.UpdateOption.WithinModeling);
                datumAxisBuilder.IsAssociative = true;
                datumAxisBuilder.IsAxisReversed = false;
                datumAxisBuilder.Vector = vector;
                datumAxisBuilder.Point = part.Points.CreatePoint(origin);
                var feature = datumAxisBuilder.CommitFeature();
                return (DatumAxisFeature)feature;
            }
        }

        /// <summary>Creates a datum axis from two positions</summary>
        /// <param name="startPoint">The start point of the axis</param>
        /// <param name="endPoint">The end point of the axis</param>
        /// <returns>An NX.DatumAxis object</returns>
        internal static DatumAxisFeature __CreateDatumAxis(this BasePart basePart,
            Point3d startPoint, Point3d endPoint)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var datumAxisBuilder = part.Features.CreateDatumAxisBuilder(null);

            using (session_.using_builder_destroyer(datumAxisBuilder))
            {
                datumAxisBuilder.Type = DatumAxisBuilder.Types.TwoPoints;
                datumAxisBuilder.IsAssociative = true;
                datumAxisBuilder.IsAxisReversed = false;
                datumAxisBuilder.Point1 = part.Points.CreatePoint(startPoint);
                datumAxisBuilder.Point2 = part.Points.CreatePoint(endPoint);
                var feature = datumAxisBuilder.CommitFeature();
                return (DatumAxisFeature)feature;
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        private static double ArcPercentage(ICurve curve, Point3d point1, Point3d point2)
        {
            //double arc_length = 0.0;
            //double num = curve.MaxU - curve.MinU;
            //double start_param = (curve.Parameter(point1) - curve.MinU) / num;
            //double end_param = (curve.Parameter(point2) - curve.MinU) / num;
            //UFEval eval = ufsession_.Eval;
            //eval.Initialize2(curve.NXOpenTag, out var evaluator);
            //double[] limits = new double[2] { 0.0, 1.0 };
            //eval.AskLimits(evaluator, limits);
            //eval.Free(evaluator);
            //ufsession_.Curve.AskArcLength(curve.NXOpenTag, start_param, end_param, ModlUnits.ModlMmeter, out arc_length);
            //return arc_length / curve.ArcLength * 100.0;
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a datum csys object
        /// </summary>
        /// <param name="origin">The origin of the csys</param>
        /// <param name="matrix">The orientation of the csys</param>
        /// <returns>An NX.DatumCsys object</returns>
        internal static DatumCsys __CreateDatumCsys(this BasePart basePart,
            Point3d origin, Matrix3x3 matrix)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var builder = part.Features.CreateDatumCsysBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                var xform = part.Xforms.CreateXform(origin, matrix._AxisX(), matrix._AxisY(),
                    SmartObject.UpdateOption.WithinModeling, 1.0);
                var csys =
                    part.CoordinateSystems.CreateCoordinateSystem(xform,
                        SmartObject.UpdateOption.WithinModeling);
                builder.Csys = csys;
                return (DatumCsys)builder.Commit();
            }
        }

        /// <summary>
        ///     Creates a datum csys object
        /// </summary>
        /// <param name="origin">The origin of the csys</param>
        /// <param name="axisX">The axis in x direction</param>
        /// <param name="axisY">The axis in y direction</param>
        /// <returns>An NX.DatumCsys object</returns>
        internal static DatumCsys __CreateDatumCsys(this BasePart basePart,
            Point3d origin, Vector3d axisX, Vector3d axisY)
        {
            return basePart.__CreateDatumCsys(origin, axisX._ToMatrix3x3(axisY));
        }

        /// <summary>Creates a datum axis from two points</summary>
        /// <param name="startPoint">The start point of the datum axis</param>
        /// <param name="endPoint">The end point of the datum axis</param>
        /// <returns> A <see cref="T:Snap.NX.DatumAxis">Snap.NX.DatumAxis</see> object</returns>
        [Obsolete(nameof(NotImplementedException))]
        public static DatumAxisFeature DatumAxis(Point3d startPoint, Point3d endPoint)
        {
            return __work_part_.__CreateDatumAxis(startPoint, endPoint);
        }


        /// <summary>Creates a Snap.NX.DatumPlane feature from a given position and orientation</summary>
        /// <param name="origin">Origin of the datum plane</param>
        /// <param name="orientation">Orientation of the datum plane</param>
        /// <returns> A <see cref="T:Snap.NX.DatumPlane">Snap.NX.DatumPlane</see> object</returns>
        [Obsolete]
        public static DatumPlaneFeature DatumPlane(Point3d origin, Matrix3x3 orientation)
        {
            //return __CreateFixedDatumPlane(origin, orientation);
            throw new NotImplementedException();
        }


        /// <summary>Creates a datum plane from a point and a vector</summary>
        /// <param name="position">NXOpen.Point3d of the datum plane</param>
        /// <param name="direction">The normal vector of the datum plane</param>
        /// <returns>An NX.DatumPlane object</returns>
        internal static DatumPlaneFeature __CreateDatumPlaneFeature(this BasePart basePart,
            Point3d position, Vector3d direction)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var datumPlaneBuilder = part.Features.CreateDatumPlaneBuilder(null);
            var plane = datumPlaneBuilder.GetPlane();
            plane.SetMethod(PlaneTypes.MethodType.PointDir);
            var direction2 = __work_part_.Directions.CreateDirection(position, direction,
                SmartObject.UpdateOption.WithinModeling);
            plane.SetGeometry(new NXObject[2]
            {
                part.Points.CreatePoint(position),
                direction2
            });
            plane.Evaluate();
            var datumPlaneFeature =
                (DatumPlaneFeature)datumPlaneBuilder.CommitFeature();
            datumPlaneBuilder.Destroy();
            return datumPlaneFeature;
            throw new NotImplementedException();
        }

        /// <summary>Creates a datum plane from a point and an orientation</summary>
        /// <param name="origin">Origin of the datum plane</param>
        /// <param name="orientation">Orientation of the datum plane</param>
        /// <returns>An NX.DatumPlane object</returns>
        internal static DatumPlaneFeature __CreateFixedDatumPlaneFeature(
            this BasePart basePart,
            Point3d origin,
            Matrix3x3 orientation)
        {
            basePart.__AssertIsWorkPart();
            return (DatumPlaneFeature)__work_part_.Datums.CreateFixedDatumPlane(origin, orientation)
                .Feature;
        }

        /// <summary>Creates a rectangle (an array of four lines) from given center and side lengths</summary>
        /// <param name="center">Center location</param>
        /// <param name="width">Width in the x-direction</param>
        /// <param name="height">Height in the y-direction</param>
        /// <returns>Array of four lines</returns>
        [Obsolete]
        public static Line[] __CreateRectangle(this BasePart basePart, Point3d center,
            double width, double height)
        {
            basePart.__AssertIsWorkPart();
            var vector = new Vector3d(width / 2.0, 0.0, 0.0);
            var vector2 = new Vector3d(0.0, height / 2.0, 0.0);
            var position = center._Add(vector)._Subtract(vector2); // center + vector - vector2;
            var position2 = center._Add(vector)._Add(vector2);
            var position3 = center._Subtract(vector)._Add(vector2);
            var position4 = center._Subtract(vector)._Subtract(vector2);
            return __work_part_.PolyLine(position, position2, position3, position4, position);
        }

        /// <summary>Creates a rectangle from two diagonal points</summary>
        /// <param name="bottomLeft">The point at the (xmin, ymin) corner</param>
        /// <param name="topRight">The point at the (xmax, ymax) corner</param>
        /// <returns>Array of four lines</returns>
        [Obsolete(nameof(NotImplementedException))]
        public static Line[] Rectangle(Point3d bottomLeft, Point3d topRight)
        {
            //NXOpen.Point3d center = (bottomLeft + topRight) / 2.0;
            //double width = topRight.X - bottomLeft.X;
            //double height = topRight.Y - bottomLeft.Y;
            //return Rectangle(center, width, height);
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Creates a polyline (an array of lines connecting given positions)
        /// </summary>
        /// <param name="basePart">The base part to place the lines in.<br />Must be the work part</param>
        /// <param name="points">Array of positions forming the vertices of the polyline</param>
        /// <returns>Array of lines forming the segments of the polyline</returns>
        /// <remarks>
        ///     The i-th line has startPoint = points[i] and endPoint = points[i+1].
        /// </remarks>
        public static Line[] PolyLine(this BasePart basePart, params Point3d[] points)
        {
            var num = points.Length;
            var array = new Line[num - 1];

            for (var i = 0; i < num - 1; i++)
                array[i] = Line(points[i], points[i + 1]);

            return array;
        }

        //
        // Summary:
        //     Creates a polygon (an array of lines forming a closed figure)
        //
        // Parameters:
        //   points:
        //     Array of positions forming the vertices of the polygon
        //
        // Returns:
        //     Array of lines forming the sides of the polygon
        //
        // Remarks:
        //     The i-th line has startPoint = points[i] and endPoint = points[i+1]. The last
        //     line closes the figure -- it has startPoint = points[n-1] and endPoint = points[0].
        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Line[] Polygon(params Point3d[] points)
        {
            var num = points.Length;
            var array = new Line[num];
            for (var i = 0; i < num - 1; i++) array[i] = Line(points[i], points[i + 1]);

            array[num - 1] = Line(points[num - 1], points[0]);
            return array;
        }


        public static bool _IsEqualTo(this Point3d point3d, Point3d other)
        {
            return point3d._IsEqualTo(other._ToArray());
        }

        public static bool _IsEqualTo(this Point3d point3d, double[] array)
        {
            return Math.Abs(point3d.X - array[0]) < .001
                   && Math.Abs(point3d.Y - array[1]) < .001
                   && Math.Abs(point3d.Z - array[2]) < .001;
        }

        public static double[] _ToArray(this Point3d point3d)
        {
            return new[] { point3d.X, point3d.Y, point3d.Z };
        }


        //
        // Summary:
        //     Projects a position onto a plane (along the plane normal)
        //
        // Parameters:
        //   plane:
        //     The plane onto which we want to project
        //
        // Returns:
        //     The projected position
        [Obsolete(nameof(NotImplementedException))]
        public static Point3d Project(this Point3d point, Surface.Plane plane)
        {
            //Vector normal = plane.Normal;
            //Vector vector =  (point - plane.Origin) * normal * normal;
            //return this - vector;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Projects a position onto a ray (along a ray normal)
        //
        // Parameters:
        //   ray:
        //     The ray onto which we want to project
        //
        // Returns:
        //     The projected position
        [Obsolete(nameof(NotImplementedException))]
        public static Point3d Project(this Point3d point, Geom.Curve.Ray ray)
        {
            //Vector axis = ray.Axis;
            //double num = (this - ray.Origin) * axis;
            //return ray.Origin + num * ray.Axis;
            throw new NotImplementedException();
        }

        public static double[] _Array(this Point3d point)
        {
            return new[] { point.X, point.Y, point.Z };
        }


        /// <summary>Creates a line through a given point, tangent to a curve</summary>
        /// <param name="basePoint">Point through which the line passes</param>
        /// <param name="icurve">A curve or edge</param>
        /// <param name="helpPoint">A point near the desired tangency point</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         The line will start at the given point, and end at the tangent point on the curve.
        ///     </para>
        ///     <para>
        ///         The help point does not have to lie on the curve, just somewhere near the desired tangency point.
        ///     </para>
        ///     <para>
        ///         If the base point lies on the curve, then an infinite line is generated. In this case, the help point has no
        ///         influence.
        ///     </para>
        ///     <para>
        ///         If the base point and the curve are not in the same plane, then the base point is projected to the plane of the
        ///         curve,
        ///         and a tangent line is generated between the projected point and the given curve.
        ///     </para>
        /// </remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Line LineTangent(Point3d basePoint, ICurve icurve, Point3d helpPoint)
        {
            //IsLinearCurve(icurve);
            //if (Compute.Distance(basePoint, (Snap.NX.NXObject)icurve) < 1E-05)
            //{
            //    double value = icurve.Parameter(basePoint);
            //    NXOpen.Vector3d axis = icurve.Tangent(value);
            //    NXOpen.Point3d[] array = Compute.ClipRay(new Snap.Geom.Curve.Ray(basePoint, axis));
            //    return Line(array[0], array[1]);
            //}

            //double value2 = icurve.Parameter(icurve._StartPoint());
            //Surface.Plane plane = new Surface.Plane(icurve._StartPoint(), icurve.Binormal(value2));
            //NXOpen.Point3d p = basePoint.Project(plane);
            //NXOpen.Point3d position = helpPoint.Project(plane);
            //var workPart = __work_part_;
            //AssociativeLine associativeLine = null;
            //AssociativeLineBuilder associativeLineBuilder = workPart.NXOpenPart.BaseFeatures.CreateAssociativeLineBuilder(associativeLine);
            //associativeLineBuilder.StartPointOptions = AssociativeLineBuilder.StartOption.Point;
            //associativeLineBuilder.StartPoint.Value = Point(p);
            //associativeLineBuilder.EndPointOptions = AssociativeLineBuilder.EndOption.Tangent;
            //associativeLineBuilder.EndTangent.SetValue(icurve, null, position);
            //associativeLineBuilder.Associative = false;
            //associativeLineBuilder.Commit();
            //NXOpen.NXObject nXObject = associativeLineBuilder.GetCommittedObjects()[0];
            //associativeLineBuilder.Destroy();
            //return new Snap.NX.Line((NXOpen.Line)nXObject);
            throw new NotImplementedException();
        }


        ///// <summary>Creates an NX.Point object from given coordinates</summary>
        ///// <param name="p">NXOpen.Point3d</param>
        ///// <returns>An invisible NXOpen.Point object (a "smart point")</returns>
        //internal static NXOpen.Point CreatePointInvisible(NXOpen.Point3d p)
        //{
        //    return CreatePointInvisible(p.X, p.Y, p.Z);
        //}


        /// <summary>Creates an NX.Arc from three points</summary>
        /// <param name="startPoint">Start point</param>
        /// <param name="pointOn">Point that the arc passes through</param>
        /// <param name="endPoint">End point</param>
        /// <returns>An NX.Arc object</returns>
        internal static Arc __CreateArc(this BasePart basePart, Point3d startPoint,
            Point3d pointOn, Point3d endPoint)
        {
            return basePart.Curves.CreateArc(startPoint, pointOn, endPoint, false, out _);
        }


        /// <summary>Map a position from Work coordinates to Absolute coordinates</summary>
        /// <param name="workCoords">The coordinates of the given point wrt the Work Coordinate System (WCS)</param>
        /// <returns>The coordinates of the given point wrt the Absolute Coordinate System (ACS)</returns>
        /// <remarks>
        ///     <para>
        ///         If you are given point coordinates relative to the WCS, then you will need to
        ///         use this function to "map" them to the ACS before using them in SNAP functions.
        ///     </para>
        /// </remarks>
        public static Point3d MapWcsToAcs(Point3d workCoords)
        {
            var input_csys = 3;
            var output_csys = 1;
            var numArray = new double[3];
            ufsession_.Csys.MapPoint(input_csys, workCoords._ToArray(), output_csys, numArray);
            return numArray._ToPoint3d();
        }


        /// <summary>Map a position from Absolute coordinates to Work coordinates</summary>
        /// <param name="absCoords">The coordinates of the given point wrt the Absolute Coordinate System (ACS)</param>
        /// <returns>The coordinates of the given point wrt the Work Coordinate System (WCS)</returns>
        public static Point3d MapAcsToWcs(Point3d absCoords)
        {
            var output_csys = 3;
            var input_csys = 1;
            var numArray = new double[3];
            ufsession_.Csys.MapPoint(input_csys, absCoords._ToArray(), output_csys, numArray);
            return numArray._ToPoint3d();
        }


        public static double __Distance(this Point3d p, Point3d q)
        {
            return p._Subtract(q)._Norm();
        }

        public static double __Distance2(this Point3d p, Point3d q)
        {
            return p._Subtract(q)._Norm2();
        }

        public static Point3d __MidPoint(this Point3d p, Point3d q)
        {
            var x = (p.X + q.X) / 2;
            var y = (p.Y + q.Y) / 2;
            var z = (p.Z + q.Z) / 2;
            return new Point3d(x, y, z);
        }

        #endregion

        #region Rules

        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule _Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        public static Body[] _Data(this BodyDumbRule rule)
        {
            rule.GetData(out var bodies);
            return bodies;
        }

        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule _Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static EdgeTangentRule __CreateRuleEdgeTangent(
            this BasePart basePart,
            Edge startEdge,
            Edge endEdge = null,
            bool isFromStart = false,
            double? angleTolerance = null,
            bool hasSameConvexity = false,
            bool allowLaminarEdge = false)
        {
            var tol = angleTolerance ?? AngleTolerance;
            return basePart.ScRuleFactory.CreateRuleEdgeTangent(startEdge, endEdge, isFromStart, tol, hasSameConvexity,
                allowLaminarEdge);
        }


        public static void Rule(this BasePart basePart)
        {
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
        }

        [Obsolete]
        public static ApparentChainingRule __CreateRuleApparentChainingRule(this BasePart basePart)
        {
            //basePart.ScRuleFactory.CreateRuleApparentChaining()
            throw new NotImplementedException();
        }

        public static CurveDumbRule __CreateRuleBaseCurveDumb(this BasePart basePart,
            params IBaseCurve[] ibaseCurves)
        {
            return basePart.ScRuleFactory.CreateRuleBaseCurveDumb(ibaseCurves);
        }

        public static BodyDumbRule __CreateRuleBodyDumb(this BasePart basePart,
            params Body[] bodies)
        {
            return basePart.ScRuleFactory.CreateRuleBodyDumb(bodies);
        }

        public static void __CreateRuleBodyFeature(this BasePart basePart)
        {
        }

        public static void __CreateRuleBodyGroup(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveChain(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveDumb(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveDumbFromPoints(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveFeature(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveFeatureChain(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveFeatureTangent(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveGroup(this BasePart basePart)
        {
        }

        public static void __CreateRuleCurveTangent(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeBody(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeBoundary(this BasePart basePart)
        {
        }

        [Obsolete]
        public static EdgeChainRule __CreateRuleEdgeChain(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public static EdgeDumbRule __CreateRuleEdgeDumb(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        public static void __CreateRuleEdgeFace(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeFeature(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeIntersect(this BasePart basePart)
        {
        }


        public static void __CreateRuleEdgeMultipleSeedTangent(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeSheetBoundary(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeTangent(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeVertex(this BasePart basePart)
        {
        }

        public static void __CreateRuleEdgeVertexTangent(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceAdjacent(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceAllBlend(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceAndAdjacentFaces(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceBody(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceBossPocket(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceConnectedBlend(this BasePart basePart)
        {
        }

        public static void __CreateRuleFaceDatum(this BasePart basePart)
        {
        }

        public static FaceDumbRule __CreateRuleFaceDumb(this BasePart basePart,
            params Face[] faces)
        {
            return basePart.ScRuleFactory.CreateRuleFaceDumb(faces);
        }

        [Obsolete]
        public static FaceFeatureRule __CreateRuleFaceFeature(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public static FaceTangentRule __CreateRuleFaceTangent(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region DatumAxis

        public static Vector3d __Direction(this DatumAxis datumAxis)
        {
            return datumAxis.Direction;
        }

        public static Point3d __Origin(this DatumAxis datumAxis)
        {
            return datumAxis.Origin;
        }

        #endregion

        #region DisplayableObject

        public static IDisposable __UsingRedisplayObject(this DisplayableObject displayableObject)
        {
            return new RedisplayObject(displayableObject);
        }

        public static int __Color(this DisplayableObject obj)
        {
            return obj.Color;
        }

        public static void __Color(this DisplayableObject obj, int color, bool redisplayObj = true)
        {
            obj.Color = color;

            if (redisplayObj)
                obj.__RedisplayObject();
        }

        public static void __Translucency(this DisplayableObject obj, int translucency, bool redisplayObj = true)
        {
            ufsession_.Obj.SetTranslucency(obj.Tag, translucency);

            if (redisplayObj)
                obj.__RedisplayObject();
        }

        public static int __Layer(this DisplayableObject displayableObject)
        {
            return displayableObject.Layer;
        }

        public static void __Layer(this DisplayableObject displayableObject, int layer, bool redisplayObj = true)
        {
            displayableObject.Layer = layer;

            if (redisplayObj)
                displayableObject.__RedisplayObject();
        }

        public static void __RedisplayObject(this DisplayableObject obj)
        {
            obj.RedisplayObject();
        }

        public static void __Blank(this DisplayableObject obj)
        {
            obj.Blank();
        }

        public static void __Unblank(this DisplayableObject obj)
        {
            obj.Unblank();
        }

        public static void __Highlight(this DisplayableObject obj)
        {
            obj.Highlight();
        }


        public static void __Unhighlight(this DisplayableObject obj)
        {
            obj.Unhighlight();
        }

        public static bool __IsBlanked(DisplayableObject obj)
        {
            return obj.IsBlanked;
        }

        #endregion

        #region Arc

        [Obsolete(nameof(NotImplementedException))]
        public static Arc __Mirror(
            this Arc arc,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Arc __Mirror(
            this Arc arc,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static Point3d __CenterPoint(this Arc arc)
        {
            return arc.CenterPoint;
        }

        public static double __EndAngle(this Arc arc)
        {
            return arc.EndAngle;
        }

        [Obsolete]
        public static bool __IsClosed(this Arc arc)
        {
            //return arc.IsClosed;
            throw new NotImplementedException();
        }

        public static bool __IsReference(this Arc arc)
        {
            return arc.IsReference;
        }

        public static NXMatrix __Matrix(this Arc arc)
        {
            return arc.Matrix;
        }

        public static Matrix3x3 __Orientation(this Arc arc)
        {
            return arc.__Matrix().__Element();
        }

        public static double __Radius(this Arc arc)
        {
            return arc.Radius;
        }

        #endregion

        #region BasePart

        public enum CurveOrientations
        {
            /// <summary>Along the tangent vector of the curve at the point</summary>
            Tangent,

            /// <summary>Along the normal vector of the curve at the point</summary>
            Normal,

            /// <summary>Along the binormal vector of the curve at the point</summary>
            Binormal
        }
        //public static NXOpen.Assemblies.Component __AddComponent(
        //   this NXOpen.BasePart basePart,
        //   NXOpen.BasePart partToAdd,
        //   string referenceSet = "Entire Part",
        //   string componentName = null,
        //   NXOpen.Point3d? origin = null,
        //   NXOpen.Matrix3x3? orientation = null,
        //   int layer = 1)
        //{
        //    basePart.__AssertIsWorkPart();
        //    NXOpen.Point3d __origin = origin ?? new NXOpen.Point3d(0d, 0d, 0d);
        //    NXOpen.Matrix3x3 __orientation = orientation ?? new NXOpen.Matrix3x3
        //    {
        //        Xx = 1,
        //        Xy = 0,
        //        Xz = 0,
        //        Yx = 0,
        //        Yy = 1,
        //        Yz = 0,
        //        Zx = 0,
        //        Zy = 0,
        //        Zz = 1,
        //    };

        //    string __name = componentName ?? basePart.Leaf;
        //    return basePart.ComponentAssembly.AddComponent(partToAdd, referenceSet, __name, __origin, __orientation, layer, out _);
        //}

        public static void __NXOpenBasePart(BasePart base_part)
        {
            //base_part.Annotations
            //base_part.Arcs
            //base_part.Axes
            //base_part.CanBeDisplayPart
            //base_part.Close
            //base_part.ComponentAssembly
            //base_part.CoordinateSystems
            //base_part.CreateReferenceSet
            //base_part.Curves
            //base_part.Datums
            //base_part.Directions
            //base_part.Displayed
            //base_part.Ellipses
            //base_part.Expressions
            //base_part.BaseFeatures
            //base_part.DeleteReferenceSet
            //base_part.Features
            //base_part.FullPath
            //base_part.GetAllReferenceSets
            //base_part.GetHistoryInformation
            //base_part.GetMinimallyLoadedParts
            //base_part.Grids
            //base_part.HasAnyMinimallyLoadedChildren
            //base_part.HasWriteAccess
            //base_part.Hyperbolas
            //base_part.IsFullyLoaded
            //base_part.IsModified
            //base_part.IsReadOnly
            //base_part.LayerCategories
            //base_part.Layers
            //base_part.Leaf
            //base_part.Lines
            //base_part.LoadFully
            //base_part.LoadThisPartFully
            //base_part.LoadThisPartPartially
            //base_part.ModelingViews
            //base_part.NXMatrices
            //base_part.Parabolas
            //base_part.PartLoadState
            //base_part.PartUnits
            //base_part.Planes
            //base_part.Points
            //base_part.Reopen
            //base_part.ReverseBlankAll
            //base_part.Save
            //base_part.SaveAs
            //base_part.ScCollectors
            //base_part.ScRuleFactory
            //base_part.Sections
            //base_part.Undisplay
            //base_part.UniqueIdentifier
            //base_part.UnitCollection
            //base_part.UserDefinedObjectManager
            //base_part.Views
            //base_part.WCS
        }

        /// <summary>
        ///     Creates a cylinder feature, given base point, direction, height and diameter<br />
        ///     expressions
        /// </summary>
        /// <param name="basePart">THe part to place the Cyclinder in</param>
        /// <param name="axisPoint">The point at base of cylinder</param>
        /// <param name="axisVector">The cylinder axis vector (length doesn't matter)</param>
        /// <param name="height">The cylinder height</param>
        /// <param name="diameter">The cylinder diameter</param>
        /// <returns>An NX.Cylinder feature</returns>
        internal static Cylinder __CreateCylinder(
            this BasePart basePart,
            Point3d axisPoint,
            Vector3d axisVector,
            double height,
            double diameter)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var cylinderBuilder = part.Features.CreateCylinderBuilder(null);

            using (session_.using_builder_destroyer(cylinderBuilder))
            {
                cylinderBuilder.Type = CylinderBuilder.Types.AxisDiameterAndHeight;
                cylinderBuilder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                cylinderBuilder.Diameter.RightHandSide = diameter.ToString();
                cylinderBuilder.Height.RightHandSide = height.ToString();
                var origin = _Point3dOrigin;
                var direction = part.Directions.CreateDirection(origin, axisVector,
                    SmartObject.UpdateOption.WithinModeling);
                cylinderBuilder.Axis.Direction = direction;
                cylinderBuilder.Axis.Point = part.Points.CreatePoint(axisPoint);
                var cylinder = (Cylinder)cylinderBuilder.Commit();
                return cylinder;
            }
        }


        public static SetWorkPartContextQuietly __UsingSetWorkPartQuietly(this BasePart basePart)
        {
            return new SetWorkPartContextQuietly(basePart);
        }


        public static Point __CreatePoint(this BasePart part, Point3d point3D)
        {
            return part.Points.CreatePoint(point3D);
        }

        public static bool __HasDrawingSheet(this BasePart part, string drawingSheetName)
        {
            return ((Part)part).__HasDrawingSheet(drawingSheetName, StringComparison.Ordinal);
        }

        public static TreeNode __TreeNode(this BasePart part)
        {
            return new TreeNode(part.Leaf) { Tag = part };
        }

        public static string __AskDetailOp(this BasePart part)
        {
            return part.Leaf._AskDetailOp();
        }

        public static bool __IsPartDetail(this BasePart part)
        {
            return part.Leaf._IsPartDetail();
        }


        //ufsession_.Part.CheckPartWritable
        //ufsession_.Part.AskUnits
        //ufsession_.Part.AskStatus
        //ufsession_.Part.AddToRecentFileList
        //ufsession_.Part.AskNthPart
        //ufsession_.Part.AskNumParts
        //ufsession_.Part.ClearHistoryList
        //ufsession_.Part.CloseAll();

        //public static TreeNode _TreeNode(this NXOpen.BasePart part) => new TreeNode(part.Leaf) { Tag = part };

        public static bool __HasDrawingSheet(
            this BasePart part,
            string drawingSheetName,
            StringComparison stringComparison)
        {
            return part.__ToPart().DrawingSheets.ToArray()
                .Any(sheet => string.Equals(sheet.Name, drawingSheetName, stringComparison));
        }

        public static Part __ToPart(this BasePart basePart)
        {
            return (Part)basePart;
        }


        public static Expression __FindExpressionOrNull(this BasePart part, string expressionName)
        {
            return part.__FindExpressionOrNull(expressionName, StringComparison.OrdinalIgnoreCase);
        }

        public static Expression __FindExpressionOrNull(this BasePart part, string expressionName,
            StringComparison stringComparison)
        {
            return part.Expressions.Cast<Expression>()
                .SingleOrDefault(expression => expression.Name.Equals(expressionName, stringComparison));
        }

        //public static bool _HasModelingView(this NXOpen.BasePart part, string modelingViewName)
        //{
        //    return part._FindModelingViewOrNull(modelingViewName) != null;
        //}

        public static ModelingView __FindModelingViewOrNull(this BasePart part, string modelingViewName)
        {
            return part.ModelingViews.ToArray().SingleOrDefault(view => view.Name == modelingViewName);
        }

        public static ModelingView __FindModelingView(this BasePart part, string modelingViewName)
        {
            return part.__FindModelingViewOrNull(modelingViewName) ?? throw new Exception(
                $"Could not find modeling view in part \"{part.Leaf}\" with name \"{modelingViewName}\".");
        }

        [Obsolete]
        public static IEnumerable<BasePart> __DescendantParts(this BasePart nxPart)
        {
            //return nxPart.ComponentAssembly?.RootComponent._Descendants(true, true, true)
            //            .Where(__c => __c._IsLoaded())
            //            .Select(__c => __c._Prototype())
            //            .DistinctBy(__p => __p.Tag)
            //            ??
            //            new NXOpen.BasePart[] { nxPart };
            throw new NotImplementedException();
        }

        public static bool __IsCasting(this BasePart part)
        {
            if (!part.__IsPartDetail())
                return false;

            var materials = Ucf.StaticRead(Ucf.ConceptControlFile, ":CASTING_MATERIALS:",
                ":END_CASTING_MATERIALS:", StringComparison.OrdinalIgnoreCase).ToArray();
            const string material = "MATERIAL";

            if (!Regex.IsMatch(part.Leaf, Regex_Detail))
                return false;

            if (!part.HasUserAttribute(material, NXObject.AttributeType.String, -1))
                return false;

            var materialValue = part.GetUserAttributeAsString(material, NXObject.AttributeType.String, -1);

            return materialValue != null && materials.Any(s => materialValue.Contains(s));
        }

        /// <summary>Constructs a fillet arc from three points</summary>
        /// <param name="p0">First point</param>
        /// <param name="pa">Apex point</param>
        /// <param name="p1">Last point</param>
        /// <param name="radius">Radius</param>
        /// <returns>An NXOpen.Arc representing the fillet</returns>
        /// <remarks>
        ///     <para>
        ///         The fillet will be tangent to the lines p0-pa and pa-p1.
        ///         Its angular span will we be less than 180 degrees.
        ///     </para>
        /// </remarks>
        internal static Arc __CreateArcFillet(
            this BasePart basePart,
            Point3d p0,
            Point3d pa,
            Point3d p1,
            double radius)
        {
            var arc = Geom.Curve.Arc.Fillet(p0, pa, p1, radius);
            return basePart.__CreateArc(arc.Center, arc.AxisX, arc.AxisY, arc.Radius, arc.StartAngle, arc.EndAngle);
        }

        /// <summary>
        ///     Creates a <see cref="NXOpen.Features.BooleanFeature" />
        /// </summary>
        /// <param name="target">Target body</param>
        /// <param name="toolBodies">Array of tool bodies</param>
        /// <param name="booleanType">Type of boolean operation (unions, subtract, etc.)</param>
        /// <returns>NX.Boolean feature formed by operation</returns>
        internal static BooleanFeature __CreateBoolean(this BasePart basePart,
            Body target, Body[] toolBodies, Feature.BooleanType booleanType)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var builder = part.Features.CreateBooleanBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Operation = booleanType;
                builder.Target = target;
                foreach (var body in toolBodies)
                    //#pragma warning disable CS0618 // Type or member is obsolete
                    builder.Tools.Add(body);
                //#pragma warning restore CS0618 // Type or member is obsolete
                var boolean =
                    (BooleanFeature)(Feature)builder.Commit();
                return boolean;
            }
        }

        public static void __AssertIsDisplayPart(this BasePart basePart)
        {
            if (session_.Parts.Display.Tag != basePart.Tag)
                throw new ArgumentException($"Part must be display part to {nameof(__AssertIsDisplayPart)}");
        }

        public static void __AssertIsDisplayOrWorkPart(this BasePart basePart)
        {
            if (session_.Parts.Display.Tag != basePart.Tag || session_.Parts.Work.Tag != basePart.Tag)
                throw new PartIsNotWorkOrDisplayException();
        }

        public static TaggedObject[] __CycleByName(this BasePart basePart, string name)
        {
            basePart.__AssertIsWorkPart();
            var _tag = Tag.Null;
            var list = new List<TaggedObject>();

            while (true)
            {
                ufsession_.Obj.CycleByName(name, ref _tag);

                if (_tag == Tag.Null)
                    break;

                list.Add(session_.GetObjectManager().GetTaggedObject(_tag));
            }

            return list.ToArray();
        }

        /// <summary>Creates an NX.Block with two points and height</summary>
        /// <param name="matrix">Orientation (see remarks)</param>
        /// <param name="originPoint">The origin-point of the block (in absolute coordinates</param>
        /// <param name="cornerPoint">The corner-point of the block</param>
        /// <param name="height">Height</param>
        /// <returns>An NX.Block object</returns>
        internal static Block __CreateBlock(this BasePart basePart, Matrix3x3 matrix,
            Point3d originPoint, Point3d cornerPoint, double height)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var wcsOrientation = __display_part_.WCS.__Orientation();
            __display_part_.WCS.__Orientation(matrix);
            var builder = part.Features.CreateBlockFeatureBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Type = BlockFeatureBuilder.Types.TwoPointsAndHeight;
                builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                builder.SetTwoPointsAndHeight(originPoint, cornerPoint, height.ToString());
                __display_part_.WCS.__Orientation(wcsOrientation);
                return (Block)builder.Commit();
            }
        }

        /// <summary>Creates an NX.Block from two diagonal points</summary>
        /// <param name="matrix">Orientation</param>
        /// <param name="originPoint">The origin-point of the block (in absolute coordinates</param>
        /// <param name="cornerPoint">The corner-point of the block </param>
        /// <returns>An NX.Block object</returns>
        internal static Block __CreateBlock(this BasePart basePart, Matrix3x3 matrix,
            Point3d originPoint, Point3d cornerPoint)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var wcsOrientation = __wcs_.Orientation.Element;
            __wcs_.SetDirections(matrix._AxisX(), matrix._AxisY());
            var builder = part.Features.CreateBlockFeatureBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Type = BlockFeatureBuilder.Types.DiagonalPoints;
                builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                builder.SetTwoDiagonalPoints(originPoint, cornerPoint);
                var block = (Block)builder.Commit();
                __wcs_.SetDirections(wcsOrientation._AxisX(), Wcs.__Orientation().Element._AxisY());
                return block;
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        public static TaggedObject[] __CycleObjsInPart1(this BasePart basePart, string name)
        {
            //basePart._AssertIsWorkPart();
            //NXOpen.Tag _tag = NXOpen.Tag.Null;
            //var list = new List<NXOpen.TaggedObject>();

            //while (true)
            //{
            //    ufsession_.Obj.CycleObjsInPart1(name, ref _tag);

            //    if (_tag == NXOpen.Tag.Null)
            //        break;

            //    list.Add(session_.GetObjectManager().GetTaggedObject(_tag));
            //}

            //return list.ToArray();
            throw new NotImplementedException();
        }

        public static void __Reopen(this BasePart basePart, ReopenScope scope = ReopenScope.PartAndSubassemblies,
            ReopenMode mode = ReopenMode.ReopenAll)
        {
            ufsession_.Part.Reopen(basePart.Tag, (int)scope, (int)mode, out _);
        }


        public static bool __IsModified(this BasePart basePart)
        {
            return ufsession_.Part.IsModified(basePart.Tag);
        }

        public static DatumCsys __AbsoluteDatumCsys(this BasePart basePart)
        {
            return basePart.Features
                .OfType<DatumCsys>()
                .First();
        }

        public static Feature __CreateTextFeature(
            this BasePart part,
            string note,
            Point3d origin,
            Matrix3x3 orientation,
            double length,
            double height,
            string font,
            TextBuilder.ScriptOptions script)
        {
            var textBuilder = part.Features.CreateTextBuilder(null);

            using (new Destroyer(textBuilder))
            {
                textBuilder.PlanarFrame.AnchorLocation =
                    RectangularFrameBuilder.AnchorLocationType.MiddleCenter;
                textBuilder.PlanarFrame.WScale = 100.0;
                textBuilder.PlanarFrame.Length.RightHandSide = $"{length}";
                textBuilder.PlanarFrame.Height.RightHandSide = $"{height}";
                textBuilder.PlanarFrame.WScale = 75d;
                textBuilder.SelectFont(font, script);
                textBuilder.TextString = note;
                var point2 = part.Points.CreatePoint(origin);
                var csys =
                    part.CoordinateSystems.CreateCoordinateSystem(origin, orientation, true);
                textBuilder.PlanarFrame.CoordinateSystem = csys;
                textBuilder.PlanarFrame.UpdateOnCoordinateSystem();
                var workView = session_.Parts.Work.ModelingViews.WorkView;
                textBuilder.PlanarFrame.AnchorLocator.SetValue(point2, workView, origin);
                return (Text)textBuilder.Commit();
            }
        }

        public static void __SetWcsToAbsolute(this BasePart part)
        {
            part.WCS.SetOriginAndMatrix(_Point3dOrigin, _Matrix3x3Identity);
        }

        public static void __Save(this BasePart part, bool save_child_components = false,
            bool close_after_save = false)
        {
            part.Save(
                save_child_components ? BasePart.SaveComponents.True : BasePart.SaveComponents.False,
                close_after_save ? BasePart.CloseAfterSave.True : BasePart.CloseAfterSave.False);
        }

        /// <summary>Creates a layer category with a given name</summary>
        /// <param name="name">Name of layer category</param>
        /// <param name="description">Description of layer category</param>
        /// <param name="layers">Layers to be placed into the category</param>
        /// <returns>An NX.Category object</returns>
        /// <exception cref="ArgumentException"></exception>
        internal static Category __CreateCategory(this BasePart basePart, string name,
            string description, params int[] layers)
        {
            basePart.__AssertIsWorkPart();

            try
            {
                return __work_part_.LayerCategories.CreateCategory(name, description, layers);
            }
            catch (NXException ex)
            {
                if (ex.ErrorCode == 3515007)
                {
                    var message = "A category with the given name already exists.";
                    var ex2 = new ArgumentException(message, ex);
                    throw ex2;
                }

                throw new ArgumentException("unknown error", ex);
            }
        }

        /// <summary>
        ///     Creates a Snap.NX.Chamfer feature
        /// </summary>
        /// <param name="edge">Edge used to chamfer</param>
        /// <param name="distance">Offset distance</param>
        /// <param name="offsetFaces">
        ///     The offsetting method used to determine the size of the chamfer If true, the<br />
        ///     edges of the chamfer face will be constructed by offsetting the faces adjacent<br />
        ///     to the selected edge If false, the edges of the chamfer face will be constructed<br />
        ///     by offsetting the selected edge along the adjacent faces
        /// </param>
        /// <returns>An NX.Chamfer object</returns>
        internal static Chamfer __CreateChamfer(this BasePart basePart, Edge edge,
            double distance, bool offsetFaces)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var chamferBuilder = part.Features.CreateChamferBuilder(null);

            using (session_.using_builder_destroyer(chamferBuilder))
            {
                chamferBuilder.Option = ChamferBuilder.ChamferOption.SymmetricOffsets;

                chamferBuilder.Method = offsetFaces
                    ? ChamferBuilder.OffsetMethod.FacesAndTrim
                    : ChamferBuilder.OffsetMethod.EdgesAlongFaces;

                chamferBuilder.FirstOffset = distance.ToString();
                chamferBuilder.SecondOffset = distance.ToString();
                chamferBuilder.Tolerance = DistanceTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var edgeTangentRule = part.__CreateRuleEdgeTangent(edge);
                var rules = new SelectionIntentRule[1] { edgeTangentRule };
                scCollector.ReplaceRules(rules, false);
                chamferBuilder.SmartCollector = scCollector;
                var feature = chamferBuilder.CommitFeature();
                return (Chamfer)feature;
            }
        }

        /// <summary>
        ///     Creates a chamfer object
        /// </summary>
        /// <param name="edge">Edge used to chamfer</param>
        /// <param name="distance1">Offset distance1</param>
        /// <param name="distance2">Offset distance2</param>
        /// <param name="offsetFaces">
        ///     The offsetting method used to determine the size of the chamfer If true, the<br />
        ///     edges of the chamfer face will be constructed by offsetting the faces adjacent<br />
        ///     to the selected edge If false, the edges of the chamfer face will be constructed<br />
        ///     by offsetting the selected edge along the adjacent faces
        /// </param>
        /// <returns>An NX.Chamfer object</returns>
        internal static Chamfer __CreateChamfer(this BasePart basePart, Edge edge,
            double distance1, double distance2, bool offsetFaces)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var chamferBuilder = part.Features.CreateChamferBuilder(null);

            using (session_.using_builder_destroyer(chamferBuilder))
            {
                chamferBuilder.Option = ChamferBuilder.ChamferOption.TwoOffsets;
                chamferBuilder.Method = offsetFaces
                    ? ChamferBuilder.OffsetMethod.FacesAndTrim
                    : ChamferBuilder.OffsetMethod.EdgesAlongFaces;
                chamferBuilder.FirstOffset = distance1.ToString();
                chamferBuilder.SecondOffset = distance2.ToString();
                chamferBuilder.Tolerance = DistanceTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var edgeTangentRule = part.ScRuleFactory.CreateRuleEdgeTangent(edge, null,
                    false, AngleTolerance, false, false);
                var rules = new SelectionIntentRule[1] { edgeTangentRule };
                scCollector.ReplaceRules(rules, false);
                chamferBuilder.SmartCollector = scCollector;
                var feature = chamferBuilder.CommitFeature();
                return (Chamfer)feature;
            }
        }

        /// <summary>Creates a chamfer object</summary>
        /// <param name="edge">Edge used to chamfer</param>
        /// <param name="distance">Offset distance</param>
        /// <param name="angle">Offset angle</param>
        /// <returns>An NX.Chamfer object</returns>
        internal static Chamfer __CreateChamfer(this BasePart basePart, Edge edge,
            double distance, double angle)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var chamferBuilder = part.Features.CreateChamferBuilder(null);

            using (session_.using_builder_destroyer(chamferBuilder))
            {
                chamferBuilder.Option = ChamferBuilder.ChamferOption.OffsetAndAngle;
                chamferBuilder.FirstOffset = distance.ToString();
                chamferBuilder.Angle = angle.ToString();
                chamferBuilder.Tolerance = DistanceTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var edgeTangentRule = part.ScRuleFactory.CreateRuleEdgeTangent(edge, null,
                    true, AngleTolerance, false, false);
                var rules = new SelectionIntentRule[1] { edgeTangentRule };
                scCollector.ReplaceRules(rules, false);
                chamferBuilder.SmartCollector = scCollector;
                chamferBuilder.AllInstances = false;
                var feature = chamferBuilder.CommitFeature();
                return (Chamfer)feature;
            }
        }

        /// <summary>Creates a datum axis object</summary>
        /// <param name="icurve">Icurve is an edge or a curve</param>
        /// <param name="arcLengthPercent">Percent arcLength, in range 0 to 100</param>
        /// <param name="curveOrientation">The curve orientation used by axis</param>
        /// <returns>An NX.DatumAxis object</returns>
        internal static DatumAxisFeature __CreateDatumAxis(
            this BasePart basePart,
            ICurve icurve,
            double arcLengthPercent,
            CurveOrientations curveOrientation)
        {
            basePart.__AssertIsWorkPart();
            var datumAxisBuilder = __work_part_.Features.CreateDatumAxisBuilder(null);

            using (session_.using_builder_destroyer(datumAxisBuilder))
            {
                datumAxisBuilder.Type = DatumAxisBuilder.Types.OnCurveVector;
                datumAxisBuilder.ArcLength.IsPercentUsed = true;
                datumAxisBuilder.ArcLength.Expression.RightHandSide = arcLengthPercent.ToString();
                datumAxisBuilder.CurveOrientation =
                    (DatumAxisBuilder.CurveOrientations)curveOrientation;
                datumAxisBuilder.IsAssociative = true;
                datumAxisBuilder.IsAxisReversed = false;
                datumAxisBuilder.Curve.Value = icurve;
                datumAxisBuilder.ArcLength.Path.Value = (TaggedObject)icurve;
                datumAxisBuilder.ArcLength.Update(OnPathDimensionBuilder.UpdateReason.Path);
                return (DatumAxisFeature)datumAxisBuilder.Commit();
            }
        }

        /// <summary>Creates an edge blend object</summary>
        /// <param name="edges">Edge array used to blend</param>
        /// <param name="radius">Radius of a regular blend</param>
        /// <returns>An NX.EdgeBlend object</returns>
        internal static EdgeBlend __CreateEdgeBlend(this BasePart basePart, double radius,
            Edge[] edges)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var edgeBlendBuilder = part.Features.CreateEdgeBlendBuilder(null);

            using (session_.using_builder_destroyer(edgeBlendBuilder))
            {
                _ = edgeBlendBuilder.LimitsListData;
                var scCollector = part.ScCollectors.CreateCollector();
                var array = new Edge[edges.Length];

                for (var i = 0; i < array.Length; i++)
                    array[i] = edges[i];

                var edgeMultipleSeedTangentRule =
                    part.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(
                        array,
                        AngleTolerance,
                        true);

                var rules = new SelectionIntentRule[1] { edgeMultipleSeedTangentRule };
                scCollector.ReplaceRules(rules, false);
                edgeBlendBuilder.Tolerance = DistanceTolerance;
                edgeBlendBuilder.AllInstancesOption = false;
                edgeBlendBuilder.RemoveSelfIntersection = true;
                edgeBlendBuilder.ConvexConcaveY = false;
                edgeBlendBuilder.RollOverSmoothEdge = true;
                edgeBlendBuilder.RollOntoEdge = true;
                edgeBlendBuilder.MoveSharpEdge = true;
                edgeBlendBuilder.TrimmingOption = false;
                edgeBlendBuilder.OverlapOption = EdgeBlendBuilder.Overlap.AnyConvexityRollOver;
                edgeBlendBuilder.BlendOrder = EdgeBlendBuilder.OrderOfBlending.ConvexFirst;
                edgeBlendBuilder.SetbackOption = EdgeBlendBuilder.Setback.SeparateFromCorner;
                edgeBlendBuilder.AddChainset(scCollector, radius.ToString());
                var feature = edgeBlendBuilder.CommitFeature();
                return (EdgeBlend)feature;
            }
        }

        /// <summary>Creates an extract object</summary>
        /// <param name="faces">The face array which will be extracted</param>
        /// <returns>An NX.ExtractFace feature</returns>
        internal static ExtractFace __CreateExtractFace(this BasePart basePart,
            Face[] faces)
        {
            basePart.__AssertIsDisplayPart();
            var part = __work_part_;
            var builder = part.Features.CreateExtractFaceBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;
                var array = new Face[faces.Length];

                for (var i = 0; i < faces.Length; i++)
                    array[i] = faces[i];

                var faceDumbRule = part.ScRuleFactory.CreateRuleFaceDumb(array);
                var rules = new SelectionIntentRule[1] { faceDumbRule };
                builder.FaceChain.ReplaceRules(rules, false);
                var extract = (ExtractFace)builder.CommitFeature();
                return extract;
            }
        }

        public static Extrude __CreateExtrude(
            this BasePart part,
            Curve[] curves,
            Vector3d direction,
            double start,
            double end)
        {
            part.__AssertIsWorkPart();
            var builder = part.Features.CreateExtrudeBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                SelectionIntentRule[] rules = { part.ScRuleFactory.CreateRuleCurveDumb(curves) };
                builder.Section = __display_part_.Sections.CreateSection();
                builder.Section.AddToSection(rules, null, null, null, _Point3dOrigin, Section.Mode.Create,
                    false);
                builder.Limits.StartExtend.Value.Value = start;
                builder.Limits.EndExtend.Value.Value = end;

                builder.Direction = part.Directions.CreateDirection(
                    curves[0].__StartPoint(),
                    direction,
                    SmartObject.UpdateOption.WithinModeling);

                return (Extrude)builder.Commit();
            }
        }


        public static CartesianCoordinateSystem __CreateCsys(
            this BasePart part,
            Point3d origin,
            Matrix3x3 orientation,
            bool makeTemporary = true)
        {
            // Creates and NXMatrix with the orientation of the {orientation}.
            var matrix = part.__OwningPart().NXMatrices.Create(orientation);
            // The tag to hold the csys.
            Tag csysId;

            if (makeTemporary)
                ufsession_.Csys.CreateTempCsys(origin._ToArray(), matrix.Tag, out csysId);
            else
                ufsession_.Csys.CreateCsys(origin._ToArray(), matrix.Tag, out csysId);

            return (CartesianCoordinateSystem)NXObjectManager.Get(csysId);
        }

        public static CartesianCoordinateSystem __CreateCsys(this BasePart part,
            bool makeTemporary = true)
        {
            return part.__CreateCsys(_Point3dOrigin, _Matrix3x3Identity, makeTemporary);
        }

        internal static OffsetCurve __CreateOffsetLine(
            this BasePart basePart,
            ICurve icurve,
            Point point,
            string height,
            string angle,
            bool reverseDirection)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Draft;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.PointOnOffsetPlane = point;
                offsetCurveBuilder.DraftHeight.RightHandSide = height;
                offsetCurveBuilder.DraftAngle.RightHandSide = angle;
                offsetCurveBuilder.ReverseDirection = reverseDirection;
                var section = offsetCurveBuilder.CurvesToOffset;
                section.AddICurve(icurve);
                var feature = offsetCurveBuilder.CommitFeature();
                offsetCurveBuilder.CurvesToOffset.CleanMappingData();
                return feature as OffsetCurve;
            }
        }


        private static void __VarBlend(
            this BasePart part,
            Edge edge,
            double[] arclengthPercents,
            double[] radii)
        {
            part.__AssertIsWorkPart();
            var edgeBlendBuilder = part.Features.CreateEdgeBlendBuilder(null);

            using (session_.using_builder_destroyer(edgeBlendBuilder))
            {
                var scCollector = part.ScCollectors.CreateCollector();
                var edges = new Edge[1] { edge };
                var edgeDumbRule = part.ScRuleFactory.CreateRuleEdgeDumb(edges);
                var rules = new SelectionIntentRule[1] { edgeDumbRule };
                scCollector.ReplaceRules(rules, false);
                edgeBlendBuilder.AddChainset(scCollector, "5");
                var parameter = arclengthPercents[0].ToString();
                var parameter2 = arclengthPercents[1].ToString();
                var radius = radii[0].ToString();
                var radius2 = radii[1].ToString();
                Point smartPoint = null;
                edgeBlendBuilder.AddVariablePointData(edge, parameter, radius, "3.333", "0.6", smartPoint,
                    false, false);
                edgeBlendBuilder.AddVariablePointData(edge, parameter2, radius2, "3.333", "0.6", smartPoint,
                    false, false);
                edgeBlendBuilder.Tolerance = DistanceTolerance;
                edgeBlendBuilder.AllInstancesOption = false;
                edgeBlendBuilder.RemoveSelfIntersection = true;
                edgeBlendBuilder.PatchComplexGeometryAreas = true;
                edgeBlendBuilder.LimitFailingAreas = true;
                edgeBlendBuilder.ConvexConcaveY = false;
                edgeBlendBuilder.RollOverSmoothEdge = true;
                edgeBlendBuilder.RollOntoEdge = true;
                edgeBlendBuilder.MoveSharpEdge = true;
                edgeBlendBuilder.TrimmingOption = false;
                edgeBlendBuilder.OverlapOption = EdgeBlendBuilder.Overlap.AnyConvexityRollOver;
                edgeBlendBuilder.BlendOrder = EdgeBlendBuilder.OrderOfBlending.ConvexFirst;
                edgeBlendBuilder.SetbackOption = EdgeBlendBuilder.Setback.SeparateFromCorner;
                edgeBlendBuilder.CommitFeature();
            }
        }


        public static Component __RootComponentOrNull(this BasePart nxPart)
        {
            return nxPart.ComponentAssembly?.RootComponent;
        }

        public static Component __RootComponent(this BasePart nxPart)
        {
            return nxPart.__RootComponentOrNull() ?? throw new ArgumentException();
        }

        public static void __Save(this BasePart part)
        {
            part.__Save(BasePart.SaveComponents.True);
        }

        public static void __Save(this BasePart part, BasePart.SaveComponents saveComponents)
        {
            part.Save(saveComponents, BasePart.CloseAfterSave.False);
        }


        public static bool __IsSee3DData(this BasePart part)
        {
            var regex = new Regex("SEE(-|_| )3D(-|_| )DATA");

            if (!part.HasUserAttribute("DESCRIPTION", NXObject.AttributeType.String, -1))
                return false;

            var descriptionValue =
                part.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);
            return descriptionValue != null && regex.IsMatch(descriptionValue.ToUpper());
        }


        /// <summary>Creates a tube feature, given spine and inner/outer diameters</summary>
        /// <param name="spine">The centerline (spine) of the tube</param>
        /// <param name="outerDiameter">Outer diameter</param>
        /// <param name="innerDiameter">Inner diameter</param>
        /// <param name="createBsurface">If true, creates a single b-surface for the inner and outer faces of the tube</param>
        /// <returns>An NX.Tube object</returns>
        internal static Tube __CreateTube(
            this BasePart basePart,
            Curve spine,
            double outerDiameter,
            double innerDiameter,
            bool createBsurface)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var tubeBuilder = part.Features.CreateTubeBuilder(null);

            using (session_.using_builder_destroyer(tubeBuilder))
            {
                tubeBuilder.Tolerance = DistanceTolerance;
                tubeBuilder.OuterDiameter.RightHandSide = outerDiameter.ToString();
                tubeBuilder.InnerDiameter.RightHandSide = innerDiameter.ToString();
                tubeBuilder.OutputOption = TubeBuilder.Output.MultipleSegments;

                if (createBsurface)
                    tubeBuilder.OutputOption = TubeBuilder.Output.SingleSegment;

                var section = tubeBuilder.PathSection;
                section.AddICurve(spine);
                tubeBuilder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                var tube = (Tube)tubeBuilder.CommitFeature();
                return tube;
            }
        }


        /// <summary>Creates an NX.Point object from given coordinates</summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <returns>An invisible NXOpen.Point object (a "smart point")</returns>
        internal static Point __CreatePointInvisible(this BasePart basePart, double x, double y, double z)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var coordinates = new Point3d(x, y, z);
            var point = part.Points.CreatePoint(coordinates);
            point.SetVisibility(SmartObject.VisibilityOption.Invisible);
            return point;
        }


        /// <summary>Creates an NX.Point object</summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <returns>An NX.Point object</returns>
        internal static Point __CreatePoint(this BasePart basePart, double x, double y, double z)
        {
            basePart.__AssertIsWorkPart();
            var uFSession = ufsession_;
            var point_coords = new double[3] { x, y, z };
            uFSession.Curve.CreatePoint(point_coords, out var point);
            return (Point)session_.__GetTaggedObject(point);
        }

        /// <summary>Creates a point from xy-coordinates (assumes z=0)</summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>A <see cref="T:Snap.NX.Point">Snap.NX.Point</see> object</returns>
        public static Point __CreatePoint(this BasePart basePart, double x, double y)
        {
            return basePart.__CreatePoint(x, y, 0.0);
        }


        public static void __Close(this BasePart part, bool close_whole_tree = false,
            bool close_modified = false)
        {
            var __close_whole_tree = close_whole_tree
                ? BasePart.CloseWholeTree.True
                : BasePart.CloseWholeTree.False;

            var __close_modified = close_modified
                ? BasePart.CloseModified.CloseModified
                : BasePart.CloseModified.DontCloseModified;

            part.Close(__close_whole_tree, __close_modified, null);
        }


        ///// <summary>Constructs a NXOpen.Arc from center, rotation matrix, radius, angles</summary>
        ///// <param name="center">Center point (in absolute coordinates)</param>
        ///// <param name="matrix">Orientation</param>
        ///// <param name="radius">Radius</param>
        ///// <param name="angle1">Start angle (in degrees)</param>
        ///// <param name="angle2">End angle (in degrees)</param>
        ///// <returns>A <see cref="NXOpen.Arc">NXOpen.Arc</see> object</returns>
        //public static NXOpen.Arc Arc(
        //    this NXOpen.BasePart part,
        //    NXOpen.Point3d center,
        //    NXOpen.Matrix3x3 matrix,
        //    double radius,
        //    double angle1,
        //    double angle2)
        //{
        //    NXOpen.Vector3d axisX = matrix._AxisX();
        //    NXOpen.Vector3d axisY = matrix._AxisY();
        //    return part.__CreateArc(center, axisX, axisY, radius, angle1, angle2);
        //}


        public static PointFeature __CreatePointFeature(this BasePart basePart,
            Point3d point3D)
        {
            var point = basePart.__CreatePoint(point3D);
            var builder = basePart.BaseFeatures.CreatePointFeatureBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Point = point;
                return (PointFeature)builder.Commit();
            }
        }


        public static DatumAxis __CreateFixedDatumAxis(this BasePart part, Point3d start,
            Point3d end)
        {
            return part.Datums.CreateFixedDatumAxis(start, end);
        }


        public static ScCollector __CreateScCollector(this BasePart part,
            params SelectionIntentRule[] intentRules)
        {
            if (intentRules.Length == 0)
                throw new ArgumentException($"Cannot create {nameof(ScCollector)} from 0 {nameof(intentRules)}.",
                    nameof(intentRules));

            var collector = part.ScCollectors.CreateCollector();
            collector.ReplaceRules(intentRules, false);
            return collector;
        }

        public static Section __CreateSection(
            this BasePart part,
            SelectionIntentRule[] intentRules,
            NXObject seed,
            NXObject startConnector = null,
            NXObject endConnector = null,
            Point3d helpPoint = default,
            Section.Mode mode = Section.Mode.Create)
        {
            if (intentRules.Length == 0)
                throw new ArgumentException($"Cannot create {nameof(Section)} from 0 {nameof(intentRules)}.",
                    nameof(intentRules));

            var section = part.Sections.CreateSection();
            section.AddToSection(intentRules, seed, startConnector, endConnector, helpPoint, mode);
            return section;
        }

        public static DatumAxis __CreateFixedDatumAxis(this BasePart part, Point3d origin,
            Vector3d vector)
        {
            return part.Datums.CreateFixedDatumAxis(origin, origin._Add(vector));
        }


        /// <summary>
        ///     Created an offset face object
        /// </summary>
        /// <param name="faces">Offset faces</param>
        /// <param name="distance">Offset distace</param>
        /// <param name="direction">Offset direction</param>
        /// <returns>An NX.OffsetFace object</returns>
        internal static OffsetFace __CreateOffsetFace(this BasePart basePart,
            Face[] faces, double distance, bool direction)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetFaceBuilder = part.Features.CreateOffsetFaceBuilder(null);

            using (session_.using_builder_destroyer(offsetFaceBuilder))
            {
                offsetFaceBuilder.Distance.RightHandSide = distance.ToString();
                var array = new SelectionIntentRule[faces.Length];

                for (var i = 0; i < faces.Length; i++)
                {
                    var boundaryFaces = new Face[0];
                    array[i] = part.ScRuleFactory.CreateRuleFaceTangent(faces[i], boundaryFaces);
                }

                offsetFaceBuilder.FaceCollector.ReplaceRules(array, false);
                offsetFaceBuilder.Direction = direction;
                var offsetFace = (OffsetFace)offsetFaceBuilder.Commit();
                return offsetFace;
            }
        }

        /// <summary>Creates a line tangent to two curves</summary>
        /// <param name="icurve1">The first curve or edge</param>
        /// <param name="helpPoint1">A point near the desired tangency point on the first curve</param>
        /// <param name="icurve2">The second curve or edge</param>
        /// <param name="helpPoint2">A point near the desired tangency point on the second curve</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         The line will start and end at the tangency points on the two curves.
        ///     </para>
        ///     <para>
        ///         The help points do not have to lie on the curves, just somewhere near the desired tangency points.
        ///     </para>
        ///     <para>
        ///         If the two given curves are not coplanar, then the second curve is projected to the plane of the first curve,
        ///         and a tangent line is constructed between the first curve and the projected one.
        ///     </para>
        /// </remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Line __CreateLineTangent(
            this BasePart basePart,
            ICurve icurve1,
            Point3d helpPoint1,
            ICurve icurve2,
            Point3d helpPoint2)
        {
            basePart.__AssertIsWorkPart();
            icurve1.__IsLinearCurve();
            icurve1.__IsPlanar();
            icurve2.__IsLinearCurve();
            icurve2.__IsPlanar();
            var value = icurve1.__Parameter(icurve1.__StartPoint());
            var plane = new Surface.Plane(icurve1.__StartPoint(), icurve1.__Binormal(value));
            helpPoint1 = helpPoint1.Project(plane);
            helpPoint2 = helpPoint2.Project(plane);
            var workPart = __work_part_;
            AssociativeLine associativeLine = null;
            var associativeLineBuilder =
                workPart.BaseFeatures.CreateAssociativeLineBuilder(associativeLine);

            using (session_.using_builder_destroyer(associativeLineBuilder))
            {
                associativeLineBuilder.StartPointOptions = AssociativeLineBuilder.StartOption.Tangent;
                associativeLineBuilder.StartTangent.SetValue(icurve1, null, helpPoint1);
                associativeLineBuilder.EndPointOptions = AssociativeLineBuilder.EndOption.Tangent;
                associativeLineBuilder.EndTangent.SetValue(icurve2, null, helpPoint2);
                associativeLineBuilder.Associative = false;
                associativeLineBuilder.Commit();
                var nXObject = associativeLineBuilder.GetCommittedObjects()[0];
                return (Line)nXObject;
            }
        }

        /// <summary>Creates a line at a given angle, tangent to a given curve</summary>
        /// <param name="icurve">A curve or edge lying in a plane parallel to the XY-plane</param>
        /// <param name="angle">An angle measured relative to the X-axis</param>
        /// <param name="helpPoint">A point near the desired tangency point</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         The line created has "infinite" length; specifically, its length is
        ///         limited by the bounding box of the model.
        ///     </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     The input curve is a line, is non-planar, or does not lie in a plane
        ///     parallel to the XY-plane
        /// </exception>
        [Obsolete(nameof(NotImplementedException))]
        public static Line __CreateLineTangent(this BasePart basePart, ICurve icurve, double angle,
            Point3d helpPoint)
        {
            //basePart.__AssertIsWorkPart();
            //icurve.__IsLinearCurve();
            //icurve.__IsPlanar();
            //NXOpen.Session.UndoMarkId markId = SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Snap_TangentLineAngle999");
            //NXOpen.Part workPart = __work_part_;
            //NXOpen.Features.AssociativeLine associativeLine = null;
            //NXOpen.Features.AssociativeLineBuilder associativeLineBuilder = workPart.BaseFeatures.CreateAssociativeLineBuilder(associativeLine);

            //using (session_.using_builder_destroyer(associativeLineBuilder))
            //{
            //    associativeLineBuilder.StartPointOptions = NXOpen.Features.AssociativeLineBuilder.StartOption.Tangent;
            //    associativeLineBuilder.StartTangent.SetValue(icurve, null, helpPoint);
            //    associativeLineBuilder.EndPointOptions = NXOpen.Features.AssociativeLineBuilder.EndOption.AtAngle;
            //    associativeLineBuilder.EndAtAngle.Value = DatumAxis(_Point3dOrigin, _Point3dOrigin._Add(_Vector3dX())).DatumAxis;
            //    associativeLineBuilder.EndAngle.RightHandSide = angle.ToString();
            //    associativeLineBuilder.Associative = false;
            //    associativeLineBuilder.Commit();
            //    NXOpen.Line obj = (NXOpen.Line)associativeLineBuilder.GetCommittedObjects()[0];
            //    NXOpen.Point3d position = obj.StartPoint;
            //    NXOpen.Point3d position2 = obj.EndPoint;
            //    NXOpen.Point3d[] array = Compute.ClipRay(new Geom.Curve.Ray(position, position2._Subtract(position)));
            //    UndoToMark(markId, "Snap_TangentLineAngle999");
            //    DeleteUndoMark(markId, "Snap_TangentLineAngle999");
            //    return Line(array[0], array[1]);
            //}

            throw new NotImplementedException();
        }

        /// <summary>Creates an offset curve feature from given curves, direction, distance</summary>
        /// <param name="curves">Array of curves to be offset</param>
        /// <param name="distance">Offset distance</param>
        /// <param name="reverseDirection">
        ///     If true, reverse direction of offset. The default direction is the normal of<br />
        ///     the array of curves.
        /// </param>
        /// <remarks>
        ///     The resulting NX.OffsetCurve object may consist of many curves. Use the Curves<br />
        ///     property of this object to get the curves themselves.<br /><br />
        ///     This function doesn't accept a single line as input. Please us the function OffsetLine<br />
        ///     if you want to offset a single line.
        /// </remarks>
        /// <returns>A Snap.NX.OffsetCurve object</returns>
        [Obsolete(
            "Deprecated in NX8.5, please use Snap.Create.OffsetCurve(double, double, NXOpen.Point3d, NXOpen.Vector3d, params NX.ICurve[]) instead.")]
        internal static OffsetCurve __CreateOffsetCurve(this BasePart basePart,
            ICurve[] curves, double distance, bool reverseDirection)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Distance;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.TrimMethod = OffsetCurveBuilder.TrimOption.ExtendTangents;
                offsetCurveBuilder.CurvesToOffset.DistanceTolerance = DistanceTolerance;
                offsetCurveBuilder.CurvesToOffset.AngleTolerance = AngleTolerance;
                offsetCurveBuilder.CurvesToOffset.ChainingTolerance = ChainingTolerance;
                offsetCurveBuilder.CurvesToOffset.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                offsetCurveBuilder.CurvesToOffset.AllowSelfIntersection(true);
                offsetCurveBuilder.OffsetDistance.RightHandSide = distance.ToString();
                offsetCurveBuilder.ReverseDirection = reverseDirection;
                var section = offsetCurveBuilder.CurvesToOffset;

                for (var i = 0; i < curves.Length; i++)
                    section.AddICurve(curves);

                var feature = offsetCurveBuilder.CommitFeature();
                return feature as OffsetCurve;
            }
        }

        /// <summary>Creates an offset curve feature from given curves, direction, distance</summary>
        /// <param name="icurves">Array of curves to be offset</param>
        /// <param name="height">Draft height</param>
        /// <param name="angle">Draft angle</param>
        /// <param name="reverseDirection">
        ///     If true, reverse direction of offset.<br />
        ///     The default direction is close to the normal of the array of curves.
        /// </param>
        /// <remarks>
        ///     The resulting NX.OffsetCurve object may consist of many curves. Use the Curves<br />
        ///     property of this object to get the curves themselves.<br /><br />
        ///     This function doesn't accept a single line as input. Please us the function OffsetLine<br />
        ///     if you want to offset a single line.
        /// </remarks>
        /// <returns>A Snap.NX.OffsetCurve object</returns>
        internal static OffsetCurve __CreateOffsetCurve(
            this BasePart basePart,
            ICurve[] icurves,
            double height,
            double angle,
            bool reverseDirection)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Draft;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.DraftHeight.RightHandSide = height.ToString();
                offsetCurveBuilder.DraftAngle.RightHandSide = angle.ToString();
                offsetCurveBuilder.ReverseDirection = reverseDirection;
                var section = offsetCurveBuilder.CurvesToOffset;

                for (var i = 0; i < icurves.Length; i++)
                    section.AddICurve(icurves);

                var feature = offsetCurveBuilder.CommitFeature();
                offsetCurveBuilder.CurvesToOffset.CleanMappingData();
                return feature as OffsetCurve;
            }
        }

        /// <summary>
        ///     Creates an offset curve feature from given curves, direction, distance
        /// </summary>
        /// <remarks>
        ///     This function doesn't accept an array consisting of a single line as input.
        ///     The resulting NX.OffsetCurve object may consist of many curves. Use the Curves
        ///     property of this object to get the curves themselves.
        ///     Offsets of lines and arcs will again be lines and arcs, respectively. Offsets
        ///     of splines and ellipses will be splines that approximate the exact offsets to
        ///     with DistanceTolerance.
        /// </remarks>
        /// <param name="curves">Array of base curves to be offset</param>
        /// <param name="distance">Offset distance</param>
        /// <param name="helpPoint">A help point on the base curves</param>
        /// <param name="helpVector">The offset direction (roughly) at the help point</param>
        /// <returns>A Snap.NX.OffsetCurve object</returns>
        internal static OffsetCurve __CreateOffsetCurve(
            this BasePart basePart,
            ICurve[] curves,
            double distance,
            Point3d helpPoint,
            Vector3d helpVector)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Distance;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.CurveFitData.AngleTolerance = AngleTolerance;
                offsetCurveBuilder.CurvesToOffset.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                offsetCurveBuilder.CurvesToOffset.AllowSelfIntersection(true);
                offsetCurveBuilder.OffsetDistance.RightHandSide = distance.ToString();
                var section = offsetCurveBuilder.CurvesToOffset;
                section.AddICurve(curves);
#pragma warning disable CS0618 // Type or member is obsolete
                offsetCurveBuilder.ReverseDirection =
                    __IsReverseDirection(offsetCurveBuilder, curves, helpPoint, helpVector);
#pragma warning restore CS0618 // Type or member is obsolete
                var feature = offsetCurveBuilder.CommitFeature();
                return feature as OffsetCurve;
            }
        }

        /// <summary>
        ///     Creates an offset curve feature from given curves, direction, distance
        /// </summary>
        /// <remarks>
        ///     The resulting NX.OffsetCurve object may consist of many curves. Use the Curves<br />
        ///     property of this object to get the curves themselves.<br /><br />
        ///     Offsets of lines and arcs will again be lines and arcs, respectively. Offsets
        ///     of splines and ellipses will be splines that approximate the exact offsets to
        ///     with DistanceTolerance.<br /><br />
        ///     This function doesn't accept an array consisting of a single line as input.
        ///     <remarks />
        ///     <param name="curves">Array of curves to be offset</param>
        ///     <param name="height">Draft height</param>
        ///     <param name="angle">Draft angle</param>
        ///     <param name="helpPoint">A help point on the base curves</param>
        ///     <param name="helpVector">The offset direction (roughly) at the help point</param>
        ///     <returns>A Snap.NX.OffsetCurve object</returns>
        internal static OffsetCurve __CreateOffsetCurve(
            this BasePart basePart,
            ICurve[] curves,
            double height,
            double angle,
            Point3d helpPoint,
            Vector3d helpVector)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Draft;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.DraftHeight.RightHandSide = height.ToString();
                offsetCurveBuilder.DraftAngle.RightHandSide = angle.ToString();
                var section = offsetCurveBuilder.CurvesToOffset;
                section.AddICurve(curves);
#pragma warning disable CS0618 // Type or member is obsolete
                offsetCurveBuilder.ReverseDirection =
                    __IsReverseDirection(offsetCurveBuilder, curves, helpPoint, helpVector);
#pragma warning restore CS0618 // Type or member is obsolete
                var feature = offsetCurveBuilder.CommitFeature();
                offsetCurveBuilder.CurvesToOffset.CleanMappingData();
                return feature as OffsetCurve;
            }
        }


        /// <summary>
        ///     Creates an offset curve feature from given curve, direction, distance
        /// </summary>
        /// <param name="icurve">Array of curves to be offset</param>
        /// <param name="point">Point on offset plane</param>
        /// <param name="distance">Offset Distance</param>
        /// <param name="reverseDirection">
        ///     If true, reverse direction of offset. The default direction is the normal of<br />
        ///     the array of curves.
        /// </param>
        /// <remarks>
        ///     The resulting NX.OffsetCurve object may consist of many curves. Use the Curves<br />
        ///     property of this object to get the curves themselves.<br /><br />
        ///     This function only accept a single line as input. Please us the function Offset<br />
        ///     if you want to offset a non-linear curve.
        /// </remarks>
        /// <returns>A Snap.NX.OffsetCurve object</returns>
        internal static OffsetCurve __CreateOffsetLine(
            this BasePart basePart,
            ICurve icurve,
            Point point,
            string distance,
            bool reverseDirection)
        {
            basePart.__AssertIsWorkPart();
            var part = __work_part_;
            var offsetCurveBuilder = part.Features.CreateOffsetCurveBuilder(null);

            using (session_.using_builder_destroyer(offsetCurveBuilder))
            {
                offsetCurveBuilder.Type = OffsetCurveBuilder.Types.Distance;
                offsetCurveBuilder.InputCurvesOptions.InputCurveOption =
                    CurveOptions.InputCurve.Retain;
                offsetCurveBuilder.CurveFitData.Tolerance = DistanceTolerance;
                offsetCurveBuilder.OffsetDistance.RightHandSide = distance;
                offsetCurveBuilder.ReverseDirection = reverseDirection;
                offsetCurveBuilder.PointOnOffsetPlane = point;
                var section = offsetCurveBuilder.CurvesToOffset;
                section.AddICurve(icurve);
                var feature = offsetCurveBuilder.CommitFeature();
                offsetCurveBuilder.CurvesToOffset.CleanMappingData();
                return feature as OffsetCurve;
            }
        }


        /// <summary>Creates a fillets with two given curves</summary>
        /// <param name="curve1">First curve for the fillet</param>
        /// <param name="curve2">Second curve for the fillet</param>
        /// <param name="radius">Radius of the fillet</param>
        /// <param name="center">Approximate fillet center expressed as absolute coordinates</param>
        /// <param name="doTrim">If true, indicates that the input curves should get trimmed by the fillet</param>
        /// <returns>An NX.Arc object</returns>
        internal static Arc __CreateArcFillet(
            this BasePart basePart,
            Curve curve1,
            Curve curve2,
            double radius,
            Point3d center,
            bool doTrim)
        {
            basePart.__AssertIsWorkPart();
            var curve_objs = new Tag[2] { curve1.Tag, curve2.Tag };
            var array = center._ToArray();
            var array2 = new int[3];
            var arc_opts = new int[3];
            if (doTrim)
            {
                array2[0] = 1;
                array2[1] = 1;
            }
            else
            {
                array2[0] = 0;
                array2[1] = 0;
            }

            ufsession_.Curve.CreateFillet(0, curve_objs, array, radius, array2, arc_opts, out var fillet_obj);
            return (Arc)session_.__GetTaggedObject(fillet_obj);
        }

        public static void __AssertIsWorkPart(this BasePart basePart)
        {
            if (__work_part_.Tag != basePart.Tag)
                throw new AssertWorkPartException(basePart);
        }


        /// <summary>Creates a Snap.NX.TrimBody feature</summary>
        /// <param name="targetBody">The target body will be trimmed</param>
        /// <param name="toolBody">The sheet body used to trim target body</param>
        /// <param name="direction">The default direction is the normal of the sheet body.</param>
        /// <returns>A Snap.NX.TrimBody feature</returns>
        internal static TrimBody2 __CreateTrimBody(this BasePart basePart,
            Body targetBody, Body toolBody, bool direction)
        {
            var part = __work_part_;
            var trimBody2Builder = part.Features.CreateTrimBody2Builder(null);

            using (session_.using_builder_destroyer(trimBody2Builder))
            {
                trimBody2Builder.Tolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = ChainingTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var bodies = new Body[1] { targetBody };
                var bodyDumbRule = part.ScRuleFactory.CreateRuleBodyDumb(bodies);
                var rules = new SelectionIntentRule[1] { bodyDumbRule };
                scCollector.ReplaceRules(rules, false);
                trimBody2Builder.TargetBodyCollector = scCollector;
                var rules2 = new SelectionIntentRule[1]
                    { part.ScRuleFactory.CreateRuleFaceBody(toolBody) };
                trimBody2Builder.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(rules2,
                    false);
                trimBody2Builder.BooleanTool.ReverseDirection = direction;
                return (TrimBody2)trimBody2Builder.Commit();
            }
        }

        /// <summary>Creates a Snap.NX.TrimBody feature</summary>
        /// <param name="targetBody">The target body will be trimmed</param>
        /// <param name="toolDatumPlane">The datum plane used to trim target body</param>
        /// <param name="direction">Trim direction. The default direction is the normal of the datum plane.</param>
        /// <returns>A Snap.NX.TrimBody feature</returns>
        internal static TrimBody2 __CreateTrimBody(this BasePart basePart,
            Body targetBody, DatumPlane toolDatumPlane, bool direction)
        {
            var part = __work_part_;
            var trimBody2Builder = part.Features.CreateTrimBody2Builder(null);

            using (session_.using_builder_destroyer(trimBody2Builder))
            {
                trimBody2Builder.Tolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = ChainingTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var bodies = new Body[1] { targetBody };
                var bodyDumbRule = part.ScRuleFactory.CreateRuleBodyDumb(bodies);
                var rules = new SelectionIntentRule[1] { bodyDumbRule };
                scCollector.ReplaceRules(rules, false);
                trimBody2Builder.TargetBodyCollector = scCollector;
                var array = new SelectionIntentRule[1];
                var faces = new DatumPlane[1] { toolDatumPlane };
                array[0] = part.ScRuleFactory.CreateRuleFaceDatum(faces);
                trimBody2Builder.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(array,
                    false);
                trimBody2Builder.BooleanTool.ReverseDirection = direction;
                var feature = trimBody2Builder.CommitFeature();
                return (TrimBody2)feature;
            }
        }


        /// <summary>Creates a "widget" body for examples and testing</summary>
        /// <returns>An NX.Body</returns>
        /// <remarks>
        ///     <para>
        ///         The widget object is shown in the pictures below.
        ///     </para>
        ///     <para>
        ///         <img src="../Images/widget.png" />
        ///     </para>
        ///     <para>
        ///         This object is useful for examples and testing because it has many different
        ///         types of faces and edges. The faces are named, for easy reference in example
        ///         code, as outlined below.
        ///     </para>
        ///     <para>
        ///         <list type="table">
        ///             <listheader>
        ///                 <term>Face Name</term>
        ///                 <description>Face Description</description>
        ///             </listheader>
        ///             <item>
        ///                 <term>CYAN_REVOLVED</term><description>Revolved face on left end</description>
        ///             </item>
        ///             <item>
        ///                 <term>MAGENTA_TORUS_BLEND</term><description>Toroidal face with minor radius = 10</description>
        ///             </item>
        ///             <item>
        ///                 <term>TEAL_CONE</term><description>Large conical face</description>
        ///             </item>
        ///             <item>
        ///                 <term>YELLOW_SPHERE</term><description>Spherical face with diameter = 15</description>
        ///             </item>
        ///             <item>
        ///                 <term>PINK_CYLINDER_HOLE</term><description>Cylindrical hole with diameter = 20</description>
        ///             </item>
        ///             <item>
        ///                 <term>RED_BSURFACE_BLEND</term>
        ///                 <description>B-surface representing a variable radius blend</description>
        ///             </item>
        ///             <item>
        ///                 <term>BLUE_CYLINDER_VERTICAL</term>
        ///                 <description>Vertical cylindrical face with diameter = 30</description>
        ///             </item>
        ///             <item>
        ///                 <term>TAN_PLANE_TOP</term><description>Planar face on top</description>
        ///             </item>
        ///             <item>
        ///                 <term>ORANGE_BLEND</term><description>Blend face with radius = 7</description>
        ///             </item>
        ///             <item>
        ///                 <term>GREEN_EXTRUDED</term><description>Extruded face on right-hand end</description>
        ///             </item>
        ///             <item>
        ///                 <term>GREY_PLANE_BACK</term><description>Large planar face on back</description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        [Obsolete(nameof(NotImplementedException))]
        public static Body __Widget()
        {
            //NXOpen.Point3d origin = _Point3dOrigin;
            //NXOpen.Vector3d axisX = Extensions._Vector3dX();
            //NXOpen.Vector3d axisY = Extensions._Vector3dY();
            //NXOpen.Vector3d axisZ = Extensions._Vector3dZ();
            //double num = 48.0;
            //double num2 = 40.0;
            //double num3 = 30.0;
            //double[] diameters = new double[2] { num, num2 };
            //double num4 = 80.0;
            //double num5 = 2.0 * num3;
            //NXOpen.Body body = Cone(origin, axisY, diameters, 90).GetBodies()[0];
            //NXOpen.Body body2 = Cylinder(new NXOpen.Point3d(0.0, num4 / 2.0, 0.0), axisZ, num5, num3).GetBodies()[0];
            //NXOpen.Line line = Line(0.0 - num, 0.0, 0.9 * num, 0.0 - num, num4, 0.7 * num);
            //NXOpen.Body body3 = ExtrudeSheet(new NXOpen.ICurve[1] { line }, axisX, 200);
            //NXOpen.Line line2 = Line(0.0, -10.0, -2.0 * num, 0.0, num4 + 10.0, -2.0 * num);
            //NXOpen.Body body4 = ExtrudeSheet(new NXOpen.ICurve[1] { line2 }, axisZ, 200);
            //TrimBody(body2, body3, direction: true);
            //NXOpen.Body body5 = TrimBody(body2, body4, direction: false).GetBodies()[0];
            //NXOpen.Body body6 = Unite(TrimBody(body, body4, direction: false), body5);
            //NXOpen.Edge[] edges = body6.Edges;
            //foreach (NXOpen.Edge edge in edges)
            //{
            //    double[] arclengthPercents = new double[2] { 0.0, 100.0 };
            //    double[] radii = new double[2] { 9.0, 4.0 };
            //    bool flag = edge.SolidEdgeType == NXOpen.Edge.EdgeType.Intersection;
            //    bool flag2 = edge.Vertices.Length > 1;
            //    if (flag && flag2)
            //    {
            //        VarBlend(edge, arclengthPercents, radii);
            //    }
            //}

            //NXOpen.Point3d position = new NXOpen.Point3d(0.0, 0.0, num / 2.0);
            //NXOpen.Point3d position2 = new NXOpen.Point3d(0.0, 5.0, 0.0);
            //NXOpen.Point3d position3 = new NXOpen.Point3d(0.0, 0.0, (0.0 - num) / 2.0);
            //NXOpen.Spline spline = BezierCurve(position, position2, position3);
            //NXOpen.Body body7 = ExtrudeSheet(new NXOpen.ICurve[1] { spline }, -axisX, num).Body;
            //NXOpen.Body targetBody = TrimBody(body6, body7, direction: true);
            //position = new NXOpen.Point3d(0.0, num4 + 10.0, 0.0);
            //position2 = new NXOpen.Point3d(0.0, num4, 20.0);
            //position3 = new NXOpen.Point3d(0.0, num4 - 15.0, 25.0);
            //NXOpen.Spline spline2 = BezierCurve(position, position2, position3);
            //NXOpen.Body body8 = RevolveSheet(new NXOpen.ICurve[1] { spline2 }, _Point3dOrigin, Extensions._AxisY());
            //NXOpen.Body body9 = TrimBody(targetBody, body8, direction: true);
            //NXOpen.Edge edge2 = null;
            //NXOpen.Edge edge3 = null;
            //edges = body9.Edges;
            //foreach (NXOpen.Edge edge4 in edges)
            //{
            //    bool num6 = edge4.SolidEdgeType == NXOpen.Edge.EdgeType.Circular;
            //    bool flag3 = edge4.ArcLength > 1.2 * num2;
            //    if (num6 && flag3)
            //    {
            //        edge2 = edge4;
            //    }

            //    if (edge4.ObjectSubType == ObjectTypes.SubType.EdgeIntersection)
            //    {
            //        edge3 = edge4;
            //    }
            //}

            //EdgeBlend(10, edge2);
            //EdgeBlend(7, edge3);
            //NXOpen.Body body10 = Cylinder(new NXOpen.Point3d(0.0 - num, 0.3 * num4, 0.0), axisX, 200, 20).Body;
            //NXOpen.Body body11 = Subtract(body9, body10).Body;
            //NXOpen.Body body12 = Sphere(new NXOpen.Point3d((0.0 - num2) / 2.0, 0.7 * num4, 0.0), 15).Body;
            //Snap.NX.Feature feature = Unite(body11, body12);
            //NXOpen.Body body13 = feature.Body;
            //Snap.NX.Feature.Orphan(feature);
            //Snap.NX.NXObject.Delete(body7, body4, body3, body8);
            //Snap.NX.NXObject.Delete(line2, line, spline, spline2);
            //body13.Color = System.Drawing.Color.Black;
            //NXOpen.Face[] faces = body13.Faces;
            //foreach (NXOpen.Face face in faces)
            //{
            //    if (face.SolidFaceType == Face.FaceType.Planar)
            //    {
            //        face.Color = System.Drawing.Color.Tan;
            //        face.Name = "TAN_PLANE_TOP";
            //        if (System.Math.Abs(face.Normal(0.5, 0.5).X) > 0.9)
            //        {
            //            face.Color = System.Drawing.Color.Gray;
            //            face.Name = "GREY_PLANE_BACK";
            //        }
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceCylinder)
            //    {
            //        face.Color = System.Drawing.Color.Blue;
            //        face.Name = "BLUE_CYLINDER_VERTICAL";
            //        if (face.Edges.Length == 2)
            //        {
            //            face.Color = System.Drawing.Color.Salmon;
            //            face.Name = "PINK_CYLINDER_HOLE";
            //        }
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceCone)
            //    {
            //        face.Color = System.Drawing.Color.Teal;
            //        face.Name = "TEAL_CONE";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceBsurface)
            //    {
            //        face.Color = System.Drawing.Color.Red;
            //        face.Name = "RED_BSURFACE_BLEND";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceTorus)
            //    {
            //        face.Color = System.Drawing.Color.Magenta;
            //        face.Name = "MAGENTA_TORUS_BLEND";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceBlend)
            //    {
            //        face.Color = System.Drawing.Color.Orange;
            //        face.Name = "ORANGE_BLEND";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceExtruded)
            //    {
            //        face.Color = System.Drawing.Color.Green;
            //        face.Name = "GREEN_EXTRUDED";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceSphere)
            //    {
            //        face.Color = System.Drawing.Color.Yellow;
            //        face.Name = "YELLOW_SPHERE";
            //    }

            //    if (face.ObjectSubType == ObjectTypes.SubType.FaceRevolved)
            //    {
            //        face.Color = System.Drawing.Color.Cyan;
            //        face.Name = "CYAN_REVOLVED";
            //    }
            //}

            //return body13;
            throw new NotImplementedException();
        }

        /// <summary>Creates a BoundedPlane object</summary>
        /// <param name="boundingCurves">Array of curves forming the boundary</param>
        /// <returns> A <see cref="T:Snap.NX.BoundedPlane">Snap.NX.BoundedPlane</see> object</returns>
        /// <remarks>
        ///     <para>
        ///         The boundary curves are input in a single list, which can include both periphery curves
        ///         and curves bounding holes. The order of the curves in the array does not matter.
        ///     </para>
        /// </remarks>
        internal static BoundedPlane __CreateBoundedPlane(this BasePart basePart,
            params Curve[] boundingCurves)
        {
            basePart.__AssertIsWorkPart();

            if (boundingCurves.Length == 0)
                throw new ArgumentOutOfRangeException();

            var part = __work_part_;
            var boundedPlaneBuilder = part.Features.CreateBoundedPlaneBuilder(null);

            using (boundedPlaneBuilder._using_builder())
            {
                boundedPlaneBuilder.BoundingCurves.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                boundedPlaneBuilder.BoundingCurves.AllowSelfIntersection(false);
                var section = boundedPlaneBuilder.BoundingCurves;
                section.AddICurve(boundingCurves);
                return (BoundedPlane)boundedPlaneBuilder.Commit();
            }
        }


        /// <summary>Unites an array of tool bodies with a target body</summary>
        /// <param name="targetBody">Target body</param>
        /// <param name="toolBodies">Array of tool bodies</param>
        /// <returns><see cref="T:Snap.NX.Boolean">Snap.NX.Boolean</see> feature formed by uniting tools with target</returns>
        public static BooleanFeature __CreateUnite(this BasePart basePart,
            Body targetBody, params Body[] toolBodies)
        {
            return basePart.__CreateBoolean(targetBody, toolBodies, Feature.BooleanType.Unite);
        }

        /// <summary>Subtracts an array of tool bodies from a target body</summary>
        /// <param name="targetBody">Target body</param>
        /// <param name="toolBodies">Array of tool bodies</param>
        /// <returns><see cref="T:Snap.NX.Boolean">Snap.NX.Boolean</see> feature formed by subtracting tools from target</returns>
        public static BooleanFeature __CreateSubtract(this BasePart basePart,
            Body targetBody, params Body[] toolBodies)
        {
            return basePart.__CreateBoolean(targetBody, toolBodies, Feature.BooleanType.Subtract);
        }

        /// <summary>Intersects an array of tool bodies with a target body</summary>
        /// <param name="targetBody">Target body</param>
        /// <param name="toolBodies">Array of tool bodies</param>
        /// <returns><see cref="T:Snap.NX.Boolean">Snap.NX.Boolean</see> feature formed by intersecting tools with target</returns>
        public static BooleanFeature __CreateIntersect(this BasePart basePart,
            Body targetBody, params Body[] toolBodies)
        {
            return basePart.__CreateBoolean(targetBody, toolBodies, Feature.BooleanType.Intersect);
        }


        public static void __SetAsDisplayPart(this BasePart part)
        {
            __display_part_ = (Part)part;
        }

        public static bool __HasReferenceSet(this BasePart part, string referenceSet)
        {
            return part.GetAllReferenceSets().SingleOrDefault(set => set.Name == referenceSet) != null;
        }

        public static ReferenceSet __FindReferenceSetOrNull(this BasePart nxPart, string refsetTitle)
        {
            return nxPart.GetAllReferenceSets().SingleOrDefault(set => set.Name == refsetTitle);
        }

        public static ReferenceSet __FindReferenceSet(this BasePart nxPart, string refsetTitle)
        {
            return nxPart.__FindReferenceSetOrNull(refsetTitle)
                   ??
                   throw new Exception(
                       $"Could not find reference set with title \"{refsetTitle}\" in part \"{nxPart.Leaf}\".");
        }


        public static Block __DynamicBlockOrNull(this BasePart nxPart)
        {
            return nxPart.Features
                .OfType<Block>()
                .SingleOrDefault(block => block.Name == "DYNAMIC BLOCK");
        }

        public static Block __DynamicBlock(this BasePart nxPart)
        {
            return nxPart.__DynamicBlockOrNull()
                   ??
                   throw new Exception("Could not find ");
        }

        public static bool __HasDynamicBlock(this BasePart part)
        {
            return part.__DynamicBlockOrNull() != null;
        }

        public static bool __Is999(this BasePart part)
        {
            var match = Regex.Match(part.Leaf, Regex_Detail);
            //GFolderWithCtsNumber.DetailPart.DetailExclusiveRegex.Match(nxPart.Leaf);
            if (!match.Success) return false;
            var detailNumber = int.Parse(match.Groups[3].Value);
            return detailNumber >= 990 && detailNumber <= 1000;
        }


        public static bool __IsJckScrew(this BasePart part)
        {
            return part.Leaf.ToLower().EndsWith("-jck-screw");
        }

        public static bool __IsJckScrewTsg(this BasePart part)
        {
            return part.Leaf.ToLower().EndsWith("-jck-screw-tsg");
        }

        public static bool __IsShcs(this BasePart part)
        {
            return part.Leaf.ToLower().Contains("-shcs-");
        }

        public static bool __IsFastener(this BasePart part)
        {
            return part.__IsDwl() || part.__IsJckScrew() || part.__IsJckScrewTsg() || part.__IsShcs();
        }

        public static bool __IsDwl(this BasePart part)
        {
            return part.Leaf.ToLower().Contains("-dwl-");
        }

        public static void __SetStringAttribute(this BasePart part, string title, string value)
        {
            part.SetUserAttribute(title, -1, value, Update.Option.Now);
        }

        //public static NXOpen.Assemblies.Component __RootComponent(this NXOpen.BasePart part)
        //{
        //    return part.ComponentAssembly.RootComponent;
        //}

        //public static bool __HasDynamicBlock(this NXOpen.BasePart part)
        //{
        //    return part.Features
        //                .ToArray()
        //                .OfType<NXOpen.Features.Block>()
        //                .Any(__b => __b.Name == "DYNAMIC BLOCK");
        //}

        /// <summary>
        ///     Returns all objects in a given part on all layers regardless of their
        ///     current status or displayability.This includes temporary (system created) objects.
        ///     It does not return expressions.
        ///     This function returns the tag of the object that was found. To continue
        ///     cycling, pass the returned object in as the second argument to this
        ///     function.
        ///     NOTE: You are strongly advised to avoid doing anything to non-alive
        ///     objects unless you are familiar with their use.NX may delete or reuse
        ///     these objects at any time. Some of these objects do not get filed with
        ///     the part.
        ///     NOTE: This routine is invalid for partially loaded parts.If you call this
        ///     function using a partially loaded part, it returns a NULL_TAG from
        ///     the beginning of the cycle.
        ///     Do not attempt to delete objects when cycling the database in a loop. Problems
        ///     can occur when trying to read the next object when the current object has been
        ///     deleted. To delete objects, save an array with the objects in it, and then
        ///     when you have completed cycling, use UF_OBJ_delete_array_of_objects to delete
        ///     the saved array of objects.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="_object"></param>
        /// <returns></returns>
        [Obsolete(nameof(NotImplementedException))]
        public static TaggedObject[] __CycleAll(this BasePart part)
        {
            //var objects = new List<NXOpen.TaggedObject>();

            //ufsession_.Obj.CycleAll

            throw new NotImplementedException();
        }

        //public static BodyDumbRule __CreateRuleBodyDumb(this BasePart part, params Body[] bodies)
        //{
        //    if (bodies.Length == 0)
        //        throw new ArgumentException($"Cannot create rule from 0 {nameof(bodies)}.", nameof(bodies));

        //    return part.ScRuleFactory.CreateRuleBodyDumb(bodies);
        //}

        public static CurveDumbRule __CreateRuleCurveDumb(this BasePart part,
            params Curve[] curves)
        {
            if (curves.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(curves)}.", nameof(curves));

            return part.ScRuleFactory.CreateRuleCurveDumb(curves);
        }

        public static EdgeDumbRule __CreateRuleEdgeDumb(this BasePart part, params Edge[] edges)
        {
            if (edges.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(edges)}.", nameof(edges));

            return part.ScRuleFactory.CreateRuleEdgeDumb(edges);
        }

        //public static FaceDumbRule __CreateRuleFaceDumb(this BasePart part, params Face[] faces)
        //{
        //    if (faces.Length == 0)
        //        throw new ArgumentException($"Cannot create rule from 0 {nameof(faces)}.", nameof(faces));

        //    return part.ScRuleFactory.CreateRuleFaceDumb(faces);
        //}

        public static EdgeFeatureRule __CreateRuleEdgeFeature(this BasePart part,
            params Feature[] features)
        {
            if (features.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(features)}.", nameof(features));

            return part.ScRuleFactory.CreateRuleEdgeFeature(features);
        }

        public static FaceFeatureRule __CreateRuleFaceFeature(this BasePart part,
            params Feature[] features)
        {
            if (features.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(features)}.", nameof(features));

            return part.ScRuleFactory.CreateRuleFaceFeature(features);
        }

        public static CurveFeatureRule __CreateRuleCurveFeature(this BasePart part,
            params Feature[] features)
        {
            if (features.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(features)}.", nameof(features));

            return part.ScRuleFactory.CreateRuleCurveFeature(features);
        }

        public static CurveFeatureChainRule __CreateRuleCurveFeatureChain(
            this BasePart part,
            Feature[] features,
            Curve startCurve,
            Curve endCurve = null,
            bool isFromSeedStart = true,
            double gapTolerance = 0.001)
        {
            if (features.Length == 0)
                throw new ArgumentException($"Cannot create rule from 0 {nameof(features)}.", nameof(features));

            return part.ScRuleFactory.CreateRuleCurveFeatureChain(features, startCurve, endCurve, isFromSeedStart,
                gapTolerance);
        }


        /// <summary>Constructs a circle from center, rotation matrix, radius</summary>
        /// <param name="center">Center point (in absolute coordinates)</param>
        /// <param name="matrix">Orientation matrix</param>
        /// <param name="radius">Radius</param>
        /// <returns>A <see cref="Arc_">Arc_</see> object</returns>
        [Obsolete]
        public static Arc __CreateCircle(this BasePart part, Point3d center,
            Matrix3x3 matrix, double radius)
        {
            //return part.__CreateArc(part, center, matrix._AxisX(), matrix._AxisY(), radius, 0.0, 360.0);
            throw new NotImplementedException();
        }

        /// <summary>Creates an NX.Arc from center, axes, radius, angles in degrees</summary>
        /// <param name="center">Center point (in absolute coordinates)</param>
        /// <param name="axisX">Unit NXOpen.Vector3d along X-axis (where angle = 0)</param>
        /// <param name="axisY">Unit NXOpen.Vector3d along Y-axis (where angle = 90)</param>
        /// <param name="radius">Radius</param>
        /// <param name="angle1">Start angle (in degrees)</param>
        /// <param name="angle2">End angle (in degrees)</param>
        /// <returns>An NX.Arc object</returns>
        public static Arc __CreateArc(
            this BasePart basePart,
            Point3d center,
            Vector3d axisX,
            Vector3d axisY,
            double radius,
            double angle1,
            double angle2)
        {
            basePart.__AssertIsWorkPart();
            var radians1 = DegreesToRadians(angle1);
            var radians2 = DegreesToRadians(angle2);
            var ori = axisX._ToMatrix3x3(axisY);
            var matrix = basePart.NXMatrices.Create(ori);
            return basePart.Curves.CreateArc(center, matrix, radius, radians1, radians2);
        }

        //public static NXOpen.Features.BooleanFeature __CreateSubtract(this NXOpen.BasePart part, NXOpen.Body target, params NXOpen.Body[] tools)
        //{
        //    NXOpen.Features.BooleanBuilder booleanBuilder = part.Features.CreateBooleanBuilder(null);

        //    using (session_.using_builder_destroyer(booleanBuilder))
        //    {
        //        booleanBuilder.Operation = NXOpen.Features.Feature.BooleanType.Subtract;
        //        booleanBuilder.Target = target;
        //        booleanBuilder.ToolBodyCollector = part.ScCollectors.CreateCollector();
        //        NXOpen.SelectionIntentRule[] rules = new[] { part.ScRuleFactory.CreateRuleBodyDumb(tools) };
        //        booleanBuilder.ToolBodyCollector.ReplaceRules(rules, false);
        //        return (NXOpen.Features.BooleanFeature)booleanBuilder.Commit();
        //    }
        //}


        public static void __CreateInterpartExpression(
            this BasePart part,
            Expression source_expression,
            string destination_name)
        {
            if (part.Tag != __display_part_.Tag)
                throw new Exception("Part must be displayed part to create interpart expressions");

            var
                builder = __display_part_.Expressions.CreateInterpartExpressionsBuilder();

            using (new Destroyer(builder))
            {
                builder.SetExpressions(new[] { source_expression }, new[] { destination_name });
                builder.Commit();
            }
        }


        //public static NXOpen.Positioning.ComponentConstraint constrain_occ_proto_distance_abs_csys(
        //    this NXOpen.BasePart part,
        //    NXOpen.Assemblies.Component component,
        //    string exp_xy,
        //    string exp_xz,
        //    string exp_yz)
        //{
        //    if (__work_part_.Tag != __display_part_.Tag)
        //        throw new Exception("Display part must be Work part");

        //    if (!__occ_plane.IsOccurrence)
        //        throw new Exception("Occurrence plane for constraint was actually a prototype.");

        //    if (__proto_plane.IsOccurrence)
        //        throw new Exception("Prototype plane for constraint was actually an occurrence.");

        //    part._AssertIsDisplayPart();
        //    NXOpen.Session.UndoMarkId markId3 = session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");
        //    NXOpen.Positioning.ComponentPositioner component_positioner1 = part.ComponentAssembly.Positioner;
        //    component_positioner1.ClearNetwork();
        //    component_positioner1.BeginAssemblyConstraints();
        //    NXOpen.Positioning.Network component_network1 = component_positioner1.EstablishNetwork();
        //    NXOpen.Positioning.ComponentConstraint componentConstraint1 = (NXOpen.Positioning.ComponentConstraint)component_positioner1.CreateConstraint(true);
        //    componentConstraint1.ConstraintType = NXOpen.Positioning.Constraint.Type.Distance;
        //    create_const_ref_occ(componentConstraint1, __occ_plane);
        //    create_const_ref_proto(componentConstraint1, __proto_plane);
        //    componentConstraint1.SetExpression(__distance_or_expression_name);
        //    component_network1.AddConstraint(componentConstraint1);
        //    component_network1.Solve();
        //    component_positioner1.ClearNetwork();
        //    session_.UpdateManager.AddToDeleteList(component_network1);
        //    session_.UpdateManager.DoUpdate(markId3);
        //    component_positioner1.EndAssemblyConstraints();
        //    session_.DisplayManager.BlankObjects(new[] { componentConstraint1.GetDisplayedConstraint() });
        //    return componentConstraint1;
        //}

        public static ComponentConstraint __ConstrainOccProtoDistance(
            this BasePart part,
            DatumPlane __occ_plane,
            DatumPlane __proto_plane,
            string __distance_or_expression_name)
        {
            if (__work_part_.Tag != __display_part_.Tag)
                throw new Exception("Display part must be Work part");

            if (!__occ_plane.IsOccurrence)
                throw new Exception("Occurrence plane for constraint was actually a prototype.");

            if (__proto_plane.IsOccurrence)
                throw new Exception("Prototype plane for constraint was actually an occurrence.");

            part.__AssertIsDisplayPart();
            var markId3 = session_.SetUndoMark(MarkVisibility.Visible, "Start");
            var component_positioner1 = part.ComponentAssembly.Positioner;
            component_positioner1.ClearNetwork();
            component_positioner1.BeginAssemblyConstraints();
            var component_network1 = component_positioner1.EstablishNetwork();
            var componentConstraint1 =
                (ComponentConstraint)component_positioner1.CreateConstraint(true);
            componentConstraint1.ConstraintType = Constraint.Type.Distance;
            componentConstraint1.__CreateConstRefOcc(__occ_plane);
            componentConstraint1.__CreateConstRefProto(__proto_plane);
            componentConstraint1.SetExpression(__distance_or_expression_name);
            component_network1.AddConstraint(componentConstraint1);
            component_network1.Solve();
            component_positioner1.ClearNetwork();
            session_.UpdateManager.AddToDeleteList(component_network1);
            session_.UpdateManager.DoUpdate(markId3);
            component_positioner1.EndAssemblyConstraints();
            session_.DisplayManager.BlankObjects(new[] { componentConstraint1.GetDisplayedConstraint() });
            return componentConstraint1;
        }

        /// <summary>
        ///     Creates a TrimBody feature by trimming with a face
        /// </summary>
        /// <param name="part">The part to create the feature in</param>
        /// <param name="targetBody">The target body to be trimmed</param>
        /// <param name="toolFace">The face used to trim the target body</param>
        /// <param name="direction">Trim direction. The default direction is the normal of the face.</param>
        /// <returns>A Snap.NX.TrimBody object</returns>
        public static TrimBody2 __CreateTrimBody(this BasePart part, Body targetBody,
            Face toolFace, bool direction)
        {
            part.__AssertIsWorkPart();
            var trimBody2Builder = part.Features.CreateTrimBody2Builder(null);

            using (session_.using_builder_destroyer(trimBody2Builder))
            {
                trimBody2Builder.Tolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = DistanceTolerance;
                trimBody2Builder.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = ChainingTolerance;
                var scCollector = part.ScCollectors.CreateCollector();
                var bodies = new Body[1] { targetBody };
                var bodyDumbRule = part.ScRuleFactory.CreateRuleBodyDumb(bodies);
                var rules = new SelectionIntentRule[1] { bodyDumbRule };
                scCollector.ReplaceRules(rules, false);
                trimBody2Builder.TargetBodyCollector = scCollector;
                var rules2 = new SelectionIntentRule[1]
                    { part.ScRuleFactory.CreateRuleFaceBody(toolFace.GetBody()) };
                trimBody2Builder.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(rules2,
                    false);
                trimBody2Builder.BooleanTool.ReverseDirection = direction;
                return (TrimBody2)trimBody2Builder.Commit();
            }
        }

        public static CartesianCoordinateSystem __AbsoluteCsys(this BasePart part)
        {
            ufsession_.Modl.AskDatumCsysComponents(part.__AbsoluteDatumCsys().Tag, out var csys_tag, out _,
                out _, out _);
            return (CartesianCoordinateSystem)session_.__GetTaggedObject(csys_tag);
        }

        public static void __ReplaceRefSets(this BasePart part, Component[] __components,
            string __refset_name)
        {
            part.ComponentAssembly.ReplaceReferenceSetInOwners(__refset_name, __components);
        }

        public static Expression __FindExpression(this BasePart part, string __expression_name)
        {
            try
            {
                return part.Expressions.FindObject(__expression_name);
            }
            catch (NXException ex) when (ex.ErrorCode == 3520016)
            {
                throw NXException.Create(ex.ErrorCode,
                    $"Could not find expression '{__expression_name}' in part {part.Leaf}");
            }
        }

        public static void __Fit(this BasePart part)
        {
            part.ModelingViews.WorkView.Fit();
        }

        public static void __RightClickOpenAssemblyWhole(this BasePart part)
        {
            if (session_.Parts.Display is null)
                throw new Exception("There is no open display part to right click open assembly");

            if (__work_part_.Tag != __display_part_.Tag)
                throw new Exception("DisplayPart does not equal __work_part_");

            if (__display_part_.Tag != part.Tag)
                throw new Exception($"Part {part.Leaf} is not the current display part");

            __display_part_.ComponentAssembly.OpenComponents(
                ComponentAssembly.OpenOption.WholeAssembly,
                new[] { part.ComponentAssembly.RootComponent },
                out _);
        }

        public static Component __AddComponent(
            this BasePart part,
            string path,
            string referenceSet = "Entire Part",
            string componentName = null,
            Point3d? origin = null,
            Matrix3x3? orientation = null,
            int layer = 1)
        {
            var __origin = origin ?? _Point3dOrigin;
            var __orientation = orientation ?? _Matrix3x3Identity;
            return part.__AddComponent(session_.find_or_open(path), referenceSet, componentName, __origin,
                __orientation, layer);
        }


        public static Component __AddComponent(
            this BasePart part,
            BasePart prototype,
            string referenceSet = "Entire Part",
            string componentName = null,
            Point3d? origin = null,
            Matrix3x3? orientation = null,
            int layer = 1)
        {
            var __origin = origin ?? _Point3dOrigin;
            var __orientation = orientation ?? _Matrix3x3Identity;
            return part.ComponentAssembly.AddComponent(prototype, referenceSet, componentName, __origin, __orientation,
                layer, out _);
        }


        /// <summary>Constructs a NXOpen.Arc from center, axes, radius, angles</summary>
        /// <param name="center">Center point (in absolute coordinates)</param>
        /// <param name="axisX">Unit vector along X-axis (where angle = 0)</param>
        /// <param name="axisY">Unit vector along Y-axis (where angle = 90)</param>
        /// <param name="radius">Radius</param>
        /// <param name="angle1">Start angle (in degrees)</param>
        /// <param name="angle2">End angle (in degrees)</param>
        /// <returns> A <see cref="NXOpen.Arc">NXOpen.Arc</see> object</returns>
        internal static Arc __CreateArc1(
            this BasePart basePart,
            Point3d center,
            Vector3d axisX,
            Vector3d axisY,
            double radius,
            double angle1,
            double angle2)
        {
            basePart.__AssertIsWorkPart();
            var startAngle = DegreesToRadians(angle1);
            var endAngle = DegreesToRadians(angle2);
            return __work_part_.Curves.CreateArc(center, axisX, axisY, radius, startAngle, endAngle);
        }

        public static PartCollection.SdpsStatus __SetActiveDisplay(this BasePart __part)
        {
            if (session_.Parts.AllowMultipleDisplayedParts != PartCollection.MultipleDisplayedPartStatus.Enabled)
                throw new Exception("Session does not allow multiple displayed parts");

            return session_.Parts.SetActiveDisplay(
                __part,
                DisplayPartOption.AllowAdditional,
                PartDisplayPartWorkPartOption.UseLast,
                out _);
        }

        /// <summary>Constructs a fillet arc from three points</summary>
        /// <param name="p0">First point</param>
        /// <param name="pa">Apex point</param>
        /// <param name="p1">Last point</param>
        /// <param name="radius">Radius</param>
        /// <returns>A Geom.Arc representing the fillet</returns>
        /// <remarks>
        ///     <para>
        ///         The fillet will be tangent to the lines p0-pa and pa-p1.
        ///         Its angular span will we be less than 180 degrees.
        ///     </para>
        /// </remarks>
        [Obsolete]
        public static Arc __CreateFillet(
            this BasePart part,
            Point3d p0,
            Point3d pa,
            Point3d p1,
            double radius)
        {
            //NXOpen.Vector3d vector = p0._Subtract(pa)._Unit();
            //NXOpen.Vector3d vector2 = p1._Subtract(pa)._Unit();
            //NXOpen.Vector3d vector3 = vector._Add(vector2);
            //double num = vector._Angle(vector2) / 2.0;
            //double num2 = radius / Math.SinD(num);
            //NXOpen.Point3d center = vector3._Multiply(num2)._Add(pa);
            //NXOpen.Vector3d vector4 = vector3._Negate();
            //NXOpen.Vector3d vector5 = vector2._Subtract(vector)._Unit();
            //double num3 = 90.0 - num;
            //double startAngle = 0.0 - num3;
            //double endAngle = num3;
            //return part.__CreateArc(center, vector4, vector5, radius, startAngle, endAngle);
            throw new NotImplementedException();
        }


        ///// <summary>Creates a datum axis with a given origin and direction</summary>
        ///// <param name="origin">The origin of the datum axis</param>
        ///// <param name="direction">The direction of the datum axis</param>
        ///// <returns> A <see cref="T:Snap.NX.DatumAxis">Snap.NX.DatumAxis</see> object</returns>
        //[Obsolete(nameof(NotImplementedException))]
        //public static DatumAxisFeature __CreateDatumAxis(this BasePart part,
        //    Point3d origin, Vector3d direction)
        //{
        //    //return CreateDatumAxis(origin, direction);
        //    throw new NotImplementedException();
        //}

        public static NXMatrix __CreateNXMatrix(this BasePart basePart, Matrix3x3 matrix)
        {
            return basePart.NXMatrices.Create(matrix);
        }

        public static CoordinateSystem __CreateCsys(this BasePart basePart, Vector3d vector3D)
        {
            var orientation = basePart.__CreateNXMatrix(vector3D._ToMatrix3x3());

            return basePart.__CreateCoordinateSystem(_Point3dOrigin, orientation);
        }

        #endregion

        #region Block

        [Obsolete(nameof(NotImplementedException))]
        public static Block __Mirror(
            this Block block,
            Surface.Plane plane)
        {
            var origin = block.__Origin().__Mirror(plane);
            var orientation = block.__Orientation()._Mirror(plane);
            var length = block.__Width();
            var width = block.__Length();
            var height = block.__Height();
            var builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.Length.Value = length;
                builder.Width.Value = width;
                builder.Height.Value = height;
                builder.Origin = origin;
                builder.SetOrientation(orientation._AxisX(), orientation._AxisY());
                return (Block)builder.Commit();
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Block __Mirror(
            this Block block,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static Point3d __Origin(this Block block)
        {
            var builder = block.OwningPart.Features.CreateBlockFeatureBuilder(block);

            using (session_.using_builder_destroyer(builder))
            {
                return builder.Origin;
            }
        }

        public static Matrix3x3 __Orientation(this Block block)
        {
            var builder = block.OwningPart.Features.CreateBlockFeatureBuilder(block);

            using (session_.using_builder_destroyer(builder))
            {
                builder.GetOrientation(out var xAxis, out var yAxis);
                return xAxis._ToMatrix3x3(yAxis);
            }
        }

        public static double __Length(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.using_builder_destroyer(builder))
            {
                return builder.Length.Value;
            }
        }

        public static double __Width(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.using_builder_destroyer(builder))
            {
                return builder.Width.Value;
            }
        }

        public static double __Height(this Block block)
        {
            var
                builder = block.__OwningPart().Features.CreateBlockFeatureBuilder(block);

            using (session_.using_builder_destroyer(builder))
            {
                return builder.Height.Value;
            }
        }

        #endregion

        #region Body

        public static void NXOpen_Body(Body body)
        {
            //body.Density
            //body.GetEdges
            //body.GetFaces
            //body.GetFeatures
            //body.IsConvergentBody
            //body.IsSheetBody
            //body.IsSolidBody
            //body.OwningPart.Features.GetAssociatedFeaturesOfBody
            //body.OwningPart.Features.GetParentFeatureOfBody
        }

        public static Box3d _Box3d(this Body body)
        {
            var array = new double[3];
            var array2 = new double[3, 3];
            var array3 = new double[3];
            var tag = body.Tag;
            ufsession_.Modl.AskBoundingBoxExact(tag, Tag.Null, array, array2, array3);
            var position = array._ToPoint3d();
            var vector = new Vector3d(array2[0, 0], array2[0, 1], array2[0, 2]);
            var vector2 = new Vector3d(array2[1, 0], array2[1, 1], array2[1, 2]);
            var vector3 = new Vector3d(array2[2, 0], array2[2, 1], array2[2, 2]);
            var maxXYZ = position._Add(vector._Multiply(array3[0]))._Add(vector2._Multiply(array3[1]))
                ._Add(vector3._Multiply(array3[2]));
            return new Box3d(position, maxXYZ);
        }

        public static BooleanFeature _Subtract(this Body target, params Body[] toolBodies)
        {
            return target.OwningPart.__CreateBoolean(target, toolBodies, Feature.BooleanType.Subtract);
        }

        public static Body _Prototype(this Body body)
        {
            return (Body)body.Prototype;
        }

        public static bool _InterferesWith(this Body target, Component component)
        {
            if (target.OwningPart.Tag != component.OwningPart.Tag)
                throw new ArgumentException("Body and component are not in the same assembly.");

            return target._InterferesWith(component.solid_body_memebers());
        }

        public static bool _InterferesWith(this Body target, params Body[] toolBodies)
        {
            //if (toolBodies.Any(__b=>__b.OwningPart.Tag != target.OwningPart.Tag))
            //    throw new ArgumentException("At least one tool body is not in the same assembly as the target body.");

            var solid_bodies = toolBodies.Select(__b => __b.Tag).ToArray();
            var results = new int[solid_bodies.Length];
            ufsession_.Modl.CheckInterference(target.Tag, solid_bodies.Length, solid_bodies, results);

            for (var i = 0; i < solid_bodies.Length; i++)
                if (results[i] == 1)
                    return true;

            return false;
        }

        #endregion

        #region CartesianCoordinateSystem

        public static DatumPlane _DatumPlaneXZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[2]);
        }

        public static DatumPlane _DatumPlaneYZ(this CartesianCoordinateSystem datumCsys)
        {
            ufsession_.Modl.AskDatumCsysComponents(datumCsys.Tag, out _, out _, out _, out var dplanes);
            return (DatumPlane)session_.__GetTaggedObject(dplanes[1]);
        }

        #endregion

        #region Chamfer

        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer _Mirror(
            this Chamfer chamfer,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Chamfer _Mirror(
            this Chamfer chamfer,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Collections

        [Obsolete(nameof(NotImplementedException))]
        public static IDictionary<double, List<Face>> ToILookIDict<T, K>(
            this IEnumerable<T> obj, Func<T, K> value)
        {
            throw new NotImplementedException();
        }

        public static IDictionary<TKey, List<TValue>> ToILookIDict<TKey, TValue>(
            this IEnumerable<TValue> source,
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            // Creates the dictionary with the default equality comparer if one was not provided.
            var dictionary =
                new Dictionary<TKey, List<TValue>>(keyComparer ?? EqualityComparer<TKey>.Default);

            foreach (var value in source)
            {
                // Gets the key from the specified key selector.
                var key = keySelector(value);

                // Checks to see if the dictionary contains the {key}.
                if (!dictionary.ContainsKey(key))
                    // If it doesn't we need to add it with an initialized {List<TValue>}.
                    dictionary[key] = new List<TValue>();

                // Adds the current {value} to the specified {key} list.
                dictionary[key].Add(value);
            }

            return dictionary;
        }

        #endregion

        #region Component

        public static IEnumerable<Component> __DescendantsAll(
            this Component component)
        {
            return from descendant in component.__Descendants(true, true, true) select descendant;
        }

        public static IEnumerable<Component> __Descendants(
            this Component rootComponent,
            bool includeRoot = true,
            bool includeSuppressed = false,
            bool includeUnloaded = false)
        {
            if (includeRoot)
                yield return rootComponent;

            var children = rootComponent.GetChildren();

            for (var index = 0; index < children.Length; index++)
            {
                if (children[index].IsSuppressed && !includeSuppressed)
                    continue;

                if (!children[index].__IsLoaded() && !includeUnloaded)
                    continue;

                foreach (var descendant in children[index]
                             .__Descendants(includeRoot, includeSuppressed, includeUnloaded))
                    yield return descendant;
            }
        }

        public static bool __IsLoaded(this Component component)
        {
            return component.Prototype is Part;
        }

        public static IEnumerable<NXObject> __Members(this Component component)
        {
            var uFSession = ufsession_;
            var tag = Tag.Null;
            var list = new List<NXObject>();

            do
            {
                tag = uFSession.Assem.CycleEntsInPartOcc(component.Tag, tag);

                if (tag == Tag.Null)
                    continue;

                try
                {
                    var nXObject = session_.__GetTaggedObject(tag) as NXObject;

                    if (nXObject is null)
                        continue;

                    list.Add(nXObject);
                }
                catch (Exception ex)
                {
                    ex._PrintException();
                }
            } while (tag != 0);

            return list;
        }

        public static Tag __InstanceTag(this Component component)
        {
            return ufsession_.Assem.AskInstOfPartOcc(component.Tag);
        }

        public static Component __ProtoChildComp(this Component component)
        {
            var instance = component.__InstanceTag();
            var root_component = component.OwningComponent._Prototype().ComponentAssembly.RootComponent.Tag;
            var proto_child_fastener_tag = ufsession_.Assem.AskPartOccOfInst(root_component, instance);
            return (Component)session_.__GetTaggedObject(proto_child_fastener_tag);
        }

        public static ExtractFace __CreateLinkedBody(this Component child)
        {
            var builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);

            using (session_.using_builder_destroyer(builder))
            {
                builder.ExtractFaceBuilder.ParentPart = ExtractFaceBuilder.ParentPartType.OtherPart;
                builder.Type = WaveLinkBuilder.Types.BodyLink;
                builder.ExtractFaceBuilder.Associative = true;
                var scCollector1 = builder.ExtractFaceBuilder.ExtractBodyCollector;
                builder.ExtractFaceBuilder.FeatureOption =
                    ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
                var bodies1 = new Body[1];
                var bodyDumbRule1 =
                    __work_part_.ScRuleFactory.CreateRuleBodyDumb(child.solid_body_memebers(), false);
                var rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                builder.ExtractFaceBuilder.FixAtCurrentTimestamp = false;
                return (ExtractFace)builder.Commit();
            }
        }

        public static bool _IsJckScrewTsg(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw-tsg");
        }

        public static bool _IsDwl(this Component part)
        {
            return part.DisplayName.ToLower().Contains("-dwl-");
        }

        public static bool _IsJckScrew(this Component part)
        {
            return part.DisplayName.ToLower().EndsWith("-jck-screw");
        }

        public static NXObject find_occurrence(this Component component, NXObject proto)
        {
            return component.FindOccurrence(proto);
        }

        //public static void reference_set(this NXOpen.Assemblies.Component component, string reference_set)
        //{
        //    component.DirectOwner.ReplaceReferenceSet(component, reference_set);
        //}

        public static CartesianCoordinateSystem _AbsoluteCsysOcc(this Component component)
        {
            return (CartesianCoordinateSystem)component.FindOccurrence(component._Prototype().__AbsoluteCsys());
        }

        public static DatumPlane _AbsOccDatumPlaneXY(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneXY());
        }

        public static DatumPlane _AbsOccDatumPlaneXZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneXZ());
        }

        public static DatumPlane _AbsOccDatumPlaneYZ(this Component component)
        {
            return (DatumPlane)component.FindOccurrence(component._Prototype().__AbsoluteDatumCsys()
                .__DatumPlaneYZ());
        }

        public static Component find_component_(this Component component,
            string __journal_identifier)
        {
            try
            {
                return (Component)component.FindObject(__journal_identifier);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(3520016);
                throw new Exception(
                    $"Could not find component with journal identifier: '{__journal_identifier}' in component '{component.DisplayName}'");
            }
        }


        public static Component _InstOfPartOcc(this Component component)
        {
            var instance = ufsession_.Assem.AskInstOfPartOcc(component.Tag);
            return (Component)session_.__GetTaggedObject(instance);
        }


        public static bool _IsShcs(this Component component)
        {
            return component.DisplayName._IsShcs();
        }

        public static bool _IsFastener(this Component component)
        {
            return component.DisplayName._IsFastener();
        }

        public static void replace_component(this Component component, string path, string name,
            bool replace_all)
        {
            var replace_builder =
                __work_part_.AssemblyManager.CreateReplaceComponentBuilder();

            using (session_.using_builder_destroyer(replace_builder))
            {
                replace_builder.ComponentNameType =
                    ReplaceComponentBuilder.ComponentNameOption.AsSpecified;
                replace_builder.ComponentsToReplace.Add(component);
                replace_builder.ReplaceAllOccurrences = replace_all;
                replace_builder.ComponentName = name;
                replace_builder.ReplacementPart = path;
                replace_builder.SetComponentReferenceSetType(
                    ReplaceComponentBuilder.ComponentReferenceSet.Maintain,
                    null);
                replace_builder.Commit();
            }
        }

        public static void delete_self_and_constraints(this Component component)
        {
            var constraints = component.GetConstraints();

            if (constraints.Length > 0)
                session_.delete_objects(constraints);

            session_.delete_objects(component);
        }


        public static string __AssemblyPathString(this Component component)
        {
            return $"{component._AssemblyPath().Aggregate("{ ", (str, cmp) => $"{str}{cmp.DisplayName}, ")}}}";
        }

        public static IDisposable using_reference_set_reset(this Component component)
        {
            return new ReferenceSetReset(component);
        }

        public static Part __prototype(this Component comp)
        {
            return comp._Prototype();
        }

        public static ReferenceSet[] __reference_sets(this Part part)
        {
            return part.GetAllReferenceSets();
        }

        public static ReferenceSet reference_sets(this Part part, string refset_name)
        {
            return part.GetAllReferenceSets().Single(__ref => __ref.Name == refset_name);
        }

        public static Component _ToComponent(this Tag __tag)
        {
            return (Component)session_.__GetTaggedObject(__tag);
        }

        public static void __DeleteInstanceUserAttribute(this Component component, string title)
        {
            throw new NotImplementedException();
        }

        public static ComponentAssembly __DirectOwner(this Component component)
        {
            return component.DirectOwner;
        }


        public static void NXOpen_Assemblies_Component(Component component)
        {
            //component.DisplayName
            //component.FindObject
            //component.FindOccurrence
            //component.FixConstraint
            //component.GetArrangements
            //component.GetChildren
            //component.GetConstraints
            //component.GetDegreesOfFreedom
            //component.GetInstanceStringUserAttribute
            //component.GetLayerOption
            //component.GetNonGeometricState
            //component.GetPosition
            //component.GetPositionOverrideParent
            //component.GetPositionOverrideType
            //component.HasInstanceUserAttribute
            //component.IsFixed
            //component.IsPositioningIsolated
            //component.IsSuppressed
            //component.Parent
            //component.Prototype
            //component.RedisplayObject
            //component.ReferenceSet
            //component.SetInstanceUserAttribute
            //component.Suppress
            //component.SuppressingArrangement
            //component.UsedArrangement
        }


        //public static NXOpen.Point3d _Origin(this NXOpen.Assemblies.Component component)
        //{
        //    // ReSharper disable once UnusedVariable
        //    component.GetPosition(out NXOpen.Point3d position, out NXOpen.Matrix3x3 orientation);
        //    return position;
        //}

        //public static Orientation_ _Orientation(this NXOpen.Assemblies.Component component)
        //{
        //    // ReSharper disable once UnusedVariable
        //    component.GetPosition(out NXOpen.Point3d position, out NXOpen.Matrix3x3 orientation);
        //    return orientation;
        //}


        public static bool _IsLeaf(this Component component)
        {
            return component.GetChildren().Length == 0;
        }

        public static bool _IsRoot(this Component component)
        {
            return component.Parent is null;
        }

        public static void _ReferenceSet(this Component component, string referenceSetTitle)
        {
            if (!(component.Prototype is Part part))
                throw new ArgumentException($"The given component \"{component.DisplayName}\" is not loaded.");

            //   part._RightClickOpen
            switch (referenceSetTitle)
            {
                case Refset_EntirePart:
                case Refset_Empty:
                    component.DirectOwner.ReplaceReferenceSet(component, referenceSetTitle);
                    break;
                default:
                    _ = part.__FindReferenceSetOrNull(referenceSetTitle)
                        ??
                        throw new InvalidOperationException(
                            $"Cannot set component \"{component.DisplayName}\" to the reference set \"{referenceSetTitle}\".");

                    component.DirectOwner.ReplaceReferenceSet(component, referenceSetTitle);
                    break;
            }
        }

        public static Part _Prototype(this Component component)
        {
            return (Part)component.Prototype;
        }

        public static Body[] solid_body_memebers(this Component component)
        {
            return component.__Members()
                .OfType<Body>()
                .Where(__b => __b.IsSolidBody)
                .ToArray();
        }

        public static TreeNode _TreeNode(this Component component)
        {
            return new TreeNode(component.DisplayName) { Tag = component };
        }

        public static Matrix3x3 _Orientation(this Component component)
        {
            component.GetPosition(out _, out var orientation);
            return orientation;
        }

        public static void _SetWcsToComponent(this Component component)
        {
            __display_part_.WCS.SetOriginAndMatrix(component._Origin(), component._Orientation());
        }

        public static Point3d _Origin(this Component component)
        {
            component.GetPosition(out var origin, out _);
            return origin;
        }

        //public static NXOpen.Assemblies.Component find_component_or_null(this NXOpen.Assemblies.Component component , string journal_identifier)
        //{
        //    try
        //    {

        //    }catch (Exception e) { }
        //}

        #endregion

        #region ComponentConstraint

        public static void __Check0(ComponentConstraint componentConstraint)
        {
            //componentConstraint.Automatic
            //componentConstraint.ConstraintAlignment
            //componentConstraint.ConstraintSecondAlignment
            //componentConstraint.ConstraintType
            //componentConstraint.CreateConstraintReference
            //componentConstraint.DeleteConstraintReference
            //componentConstraint.EditConstraintReference
            //componentConstraint.Expression
            //componentConstraint.ExpressionDriven
            //componentConstraint.FlipAlignment
            //componentConstraint.GetConstraintStatus
            //componentConstraint.GetDisplayedConstraint
            //componentConstraint.GetInherited
            //componentConstraint.GetReferences
            //componentConstraint.Persistent
            //componentConstraint.Print
            //componentConstraint.Renew
            //componentConstraint.ReverseDirection
            //componentConstraint.SetAlignmentHint
            //componentConstraint.SetExpression
        }


        public static ConstraintReference __CreateConstRefOcc(
            this ComponentConstraint __constraint,
            NXObject __occ_object)
        {
            return __constraint.CreateConstraintReference(
                __occ_object.OwningComponent,
                __occ_object,
                false,
                false,
                false);
        }

        public static ConstraintReference __CreateConstRefProto(
            this ComponentConstraint __constraint,
            NXObject __proto_object)
        {
            return __constraint.CreateConstraintReference(
                __proto_object.OwningPart.ComponentAssembly,
                __proto_object,
                false,
                false,
                false);
        }

        #endregion

        #region Constants

        /// <summary>
        ///     The path to the Concept Control File.
        /// </summary>
        public const string ControlFile = @"U:\NX110\Concept\ConceptControlFile.UCF";

        //public const string DetailNameRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailName>[0-9-A-Za-z]+)$";

        //public const string DetailNumberRegex = @"^(?<jobNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        public const string View_Plan = "PLAN";


        /// <summary>The layer to put tooling holes on.</summary>
        /// <remarks>Layer = 97</remarks>
        public const int Layer_DwlTooling = 97;

        /// <summary>The layer to place regular fasteners on.</summary>
        /// <remarks>Layer = 99</remarks>
        public const int Layer_Fastener = 99;

        /// <summary>The layer to put handling holes and wire taps on.</summary>
        /// <remarks>Layer = 98</remarks>
        public const int Layer_ShcsHandlingWireTap = 98;


        /// <summary>The regular expression that matches a n assembly folderWithCtsNumber.</summary>
        /// <remarks>Expressions = "^(?<customerNum>\d+)-(?<opNum>\d+)$"</opNum></customerNum></remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string Regex_AssemblyFolder = @"^(?<customerNum>\d+)-(?<opNum>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Bhcs = @"^(?<diameter>\d+)(?:mm)?-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_BhcsEnglish = @"^(?<diameter>\d+)-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric blind headed cap screw (bhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-bhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_BhcsMetric = @"^(?<diameter>\d+)mm-bhcs-(?<length>\d+)$";


        /// <summary>The regular expression that matches a blank.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-blank$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Blank = @"^(?<customerNum>\d+)-(?<opNum>\d+)-blank$";


        /// <summary>The regular expression pattern that matches a metric castle nut.</summary>
        /// <remarks>Expressions = "^Hexagon Castle Nut M(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_CastleNutMetric = @"^Hexagon Castle Nut M(?<diameter>\d+)$";


        /// <summary>The regular expression that matches a detail.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-(?&lt;detailNum&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Detail = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>\d+)$";


        /// <summary>The regular expression that matches a dieset control.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-dieset-control$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_DiesetControl = @"^(?<customerNum>\d+)-(?<opNum>\d+)-dieset-control$";


        /// <summary>The regular expression that matches a double.</summary>
        /// <remarks>Expressions = "(?&lt;double&gt;\\d+\\.\\d+|\\d+\\.|\\.\\d+|\\d+)"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>double</description>
        ///     </item>
        /// </list>
        public const string Regex_DoubleDecimal = @"(?<double>\d+\.\\d+|\\d+\\.|\\.\\d+|\\d+)";


        /// <summary>The regular expression pattern that matches a metric or an english dowel (dwl).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Dwl = @"^(?<diameter>\d+)(?:mm)?-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english dowel (dwl).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlEnglish = @"^(?<diameter>\d+)-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric dowel lock (dwl-lck).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-dwl-lck-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlLckMetric = @"^(?<diameter>\d+)mm-dwl-lck-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric misumi dowel lock (SWA).</summary>
        /// <remarks>Expressions = "^SWA(?&lt;diameter&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlLckMisumiMetric = @"^SWA(?<diameter>\d+)$";


        /// <summary>
        ///     The regular expression pattern that matches a metric dowel (dwl).
        ///     <para />
        /// </summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-dwl-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_DwlMetric = @"^(?<diameter>\d+)mm-dwl-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Fhcs = @"^(?<diameter>\d+)(?:mm)?-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_FhcsEnglish = @"^(?<diameter>\d+)-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric flat headed cap screw (fhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-fhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_FhcsMetric = @"^(?<diameter>\d+)mm-fhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric or an english jack screw (jck-screw).</summary>
        /// <remarks>"^_?(?&lt;diameter&gt;\d+)(?:mm)?-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrew = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw$";


        /// <summary>The regular expression pattern that matches an english jack screw (jck-screw).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewEnglish = @"^_(?<diameter>\d+)-jck-screw$";


        /// <summary>The regular expression pattern that matches a metric jack screw (jck-screw).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-jck-screw$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewMetric = @"^(?<diameter>\d+)mm-jck-screw$";


        /// <summary>The regular expression pattern that matches a metric or an english tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^_?(?&lt;diameter&gt;\d+)(?:mm)?-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsg = @"^_?(?<diameter>\d+)(?:mm)?-jck-screw-tsg$";


        /// <summary>The regular expression pattern that matches an english tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^_(?&lt;diameter&gt;\d+)-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsgEnglish = @"^_(?<diameter>\d+)-jck-screw-tsg$";


        /// <summary>The regular expression pattern that matches a metric tsg jack screw (jck-screw-tsg).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-jck-screw-tsg$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_JckScrewTsgMetric = @"^(?<diameter>\d+)mm-jck-screw-tsg$";


        /// <summary>The regular expression that matches a layout.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-layout$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Layout = @"^(?<customerNum>\d+)-(?<opNum>\d+)-layout$";


        /// <summary>The regular expression pattern that matches either a metric or an english low head cap screw (lhcs).</summary>
        /// <remarks>"^(?&lt;diameter&gt;\d+)(?:mm)?-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Lhcs = @"^(?<diameter>\d+)(?:mm)?-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english low head cap screw (lhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_LhcsEnglish = @"^(?<diameter>\d+)-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric low head cap screw (lhcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-lhcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_LhcsMetric = @"^(?<diameter>\d+)mm-lhcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric lock washer.</summary>
        /// <remarks>Expressions = "^lock washer for (?&lt;diameter&gt;\d+)mm shcs$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        /// </list>
        public const string Regex_LockWasherMetric = @"^lock washer for (?<diameter>\d+)mm shcs$";


        /// <summary>The regular expression that matches a lower shoe (lsh).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-lsh$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Lsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsh$";


        /// <summary>The regular expression that matches either a lower shoe (lsh) or an upper shoe (ush).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>shoe</description>
        ///     </item>
        /// </list>
        public const string Regex_LshUsh = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<shoe>lsh|ush)$";


        /// <summary>The regular expression that matches a lower sub plate (lsp).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Lsp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lsp(?<extraOpNum>\d+)?.*$";


        /// <summary>The regular expression that matches either a lower sub plate (lsp) or an upper sub plate (usp).</summary>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>subPlate</description>
        ///     </item>
        ///     <item>
        ///         <description>extraOpNum</description>
        ///     </item>
        /// </list>
        public const string Regex_LspUsp =
            @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<subPlate>lsp|usp)(?<extraOpNum>\d+)?$";


        /// <summary>The regular expression that matches a lower assembly holder (lwr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-lwr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Lwr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-lwr$";


        /// <summary>The regular expression that matches either a lower (lwr) or upper (upr) assembly holder (upr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-upr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_LwrUpr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-[lwr|upr]$";


        /// <summary>The regular expression that matches a mathdata part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-mathdata$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Mathdata = @"^(?<customerNum>\d+)-mathdata$";


        /// <summary>The regular expression that matches a master part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-Master$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Master = @"^(?<customerNum>\d+)-Master$";


        /// <summary>The regular expression that matches a history part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-History$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_History = @"^(?<customerNum>\d+)-History$";


        /// <summary>The regular expression pattern that matches a metric or an english nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-M?(?&lt;threadCount&gt;\d+(?:\.\d+)?)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_Nut = @"^nut-(?<diameter>\d+)-M?(?<threadCount>\d+(?:\.\d+)?)$";


        /// <summary>The regular expression pattern that matches an english nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-(?&lt;threadCount&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_NutEnglish = @"^nut-(?<diameter>\d+)-(?<threadCount>\d+)$";


        /// <summary>The regular expression pattern that matches a metric nut.</summary>
        /// <remarks>Expressions = "^nut-(?&lt;diameter&gt;\d+)-M(?&lt;threadCount&gt;\d+(?:\.\d+)?)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>threadCount</description>
        ///     </item>
        /// </list>
        public const string Regex_NutMetric = @"^nut-(?<diameter>\d+)-M(?<threadCount>\d+(?:\.\d+)?)$";


        /// <summary>The regular expression that matches a top level "000" part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-000$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Op000Holder = @"^(?<customerNum>\d+)-(?<opNum>\d+)-000$";


        /// <summary>The regular expression that matches a press assembly.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-Press-(?&lt;opNum&gt;\d+)-Assembly$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_PressAssembly = @"^(?<customerNum>\d+)-Press-(?<opNum>\d+)-Assembly$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket head cap screw (shcs).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Shcs = @"^(?<diameter>\d+)(?:mm)?-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket head cap screw (shcs).</summary>
        /// <remarks>Expression - "^(?&lt;diameter&gt;\d+)-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShcsEnglish = @"^(?<diameter>\d+)-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket head cap screw (shcs).</summary>
        /// <remarks>Expression = "^(?&lt;diameter&gt;\d+)mm-shcs-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShcsMetric = @"^(?<diameter>\d+)mm-shcs-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Shss = @"^(?<diameter>\d+)(?:mm)?-shss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShssEnglish = @"^(?<diameter>\d+)-shss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket head shoulder screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-shss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_ShssMetric = @"^(?<diameter>\d+)mm-shss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a simulation part.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-simulation$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Simulation = @"^(?<customerNum>\d+)-simulation$";


        /// <summary>The regular expression pattern that matches either a metric or an english socket set screw (sss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)(?:mm)?-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_Sss = @"^(?<diameter>\d+)(?:mm)?-sss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches an english socket set screw (shss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_SssEnglish = @"^(?<diameter>\d+)-sss-(?<length>\d+)$";


        /// <summary>The regular expression pattern that matches a metric socket set screw (sss).</summary>
        /// <remarks>Expressions = "^(?&lt;diameter&gt;\d+)mm-sss-(?&lt;length&gt;\d+)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>diameter</description>
        ///     </item>
        ///     <item>
        ///         <description>length</description>
        ///     </item>
        /// </list>
        public const string Regex_SssMetric = @"^(?<diameter>\d+)mm-sss-(?<length>\d+)$";


        /// <summary>The regular expression that matches a strip.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-strip$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description></description>
        ///     </item>
        /// </list>
        public const string Regex_Strip = @"^(?<customerNum>\d+)-(?<opNum>\d+)-strip$";


        /// <summary>The regular expression that matches a strip control.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-strip-control$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        /// </list>
        public const string Regex_StripControl = @"^(?<customerNum>\d+)-strip-control$";


        /// <summary>The regular expression that matches an upper assembly holder (upr).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-upr$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Upr = @"^(?<customerNum>\d+)-(?<opNum>\d+)-upr$";


        /// <summary>The regular expression that matches an upper shoe (ush).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-ush$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        /// </list>
        public const string Regex_Ush = @"^(?<customerNum>\d+)-(?<opNum>\d+)-ush$";


        /// <summary>The regular expression that matches an upper sub plate (usp).</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-usp(?&lt;extraOpNum&gt;\d+)?$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_Usp = @"^(?<customerNum>\d+)-(?<opNum>\d+)-usp(?<extraOpNum>\d+)?.*$";


        /// <summary>The regular expression that matches a 999 block/detail.</summary>
        /// <remarks>Expressions = "^(?&lt;customerNum&gt;\d+)-(?&lt;opNum&gt;\d+)-(?&lt;detailNum&gt;99\d)$"</remarks>
        /// <list type="number">
        ///     <item>
        ///         <description>customerNum</description>
        ///     </item>
        ///     <item>
        ///         <description>opNum</description>
        ///     </item>
        ///     <item>
        ///         <description>detailNum</description>
        ///     </item>
        ///     >
        /// </list>
        public const string Regex_999 = @"^(?<customerNum>\d+)-(?<opNum>\d+)-(?<detailNum>99\d)$";

        public const string FilePath_WireTap =
            FilePath_0LibraryFasteners + "\\Metric\\SocketHeadCapScrews\\008\\8mm-shcs-020.prt";

        public const string FilePath_0Library = "G:\\0Library";

        public const string FilePath_0Press = "G:\\0Press";

        public const string FilePath_0LibraryFasteners = FilePath_0Library + "\\Fasteners";

        public const string FilePath_DxfDwgDef = "C:\\Program Files\\Siemens\\NX 11.0\\dxfdwg\\dxfdwg.def";

        public const string FilePath_Ucf = @"U:\nxFiles\UfuncFiles\ConceptControlFile.ucf";

        public const string FilePath_Step214Ug = "C:\\Program Files\\Siemens\\NX 11.0\\NXBIN\\step214ug.exe";

        public const string FilePath_ExternalStep_AllLayers_def =
            "U:\\nxFiles\\Step Translator\\ExternalStep_AllLayers.def";

        public const string FilePath_ExternalStep_Design_def = "U:\\nxFiles\\Step Translator\\ExternalStep_Design.def";

        public const string FilePath_ExternalStep_Assembly_def =
            @"U:\nxFiles\Step Translator\ExternalStep_Assembly.def";

        public const string FilePath_ExternalStep_Detail_def = @"U:\nxFiles\Step Translator\ExternalStep_Detail.def";

        /// <summary>Returns the path to the folderWithCtsNumber 0Components.</summary>
        public const string Folder0Components = "G:\\0Library";

        /// <summary>Returns the path to the folderWithCtsNumber seedFiles.</summary>
        public const string FolderSeedFiles = Folder0Components + "\\seedFiles";

        /// <summary>Returns the path to the seed layout folderWithCtsNumber.</summary>
        public const string FolderSeedLayout = FolderSeedFiles + "\\layout";

        /// <summary>Returns the path to the XXXX-010-blank part file.</summary>
        public const string SeedBlank = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-blank.prt";

        /// <summary>Returns the path to the XXXX-010-strip part file.</summary>
        public const string SeedStrip010 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-strip.prt";

        /// <summary>Returns the path to the XXXX-020-layout part file.</summary>
        public const string SeedLayout020 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-020-layout.prt";

        /// <summary>Returns the path to the XXXX-010-layout part file.</summary>
        public const string SeedLayout010 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-010-layout.prt";

        /// <summary>Returns the path to the XXXX-900-strip part file.</summary>
        public const string SeedStrip900 = "G:\\0Library\\SeedFiles\\Layout\\XXXX-900-strip.prt";

        /// <summary>Returns the path to the XXXX-strip-control part file.</summary>
        public const string SeedStripControl = "G:\\0Library\\SeedFiles\\Layout\\XXXX-strip-control.prt";

        /// <summary>Returns the path to the seed-press part file.</summary>
        public const string SeedPress = "G:\\0Library\\SeedFiles\\Press\\seed-press.prt";

        /// <summary>Returns the path to the XXXX-Press-01-Assembly part file.</summary>
        public const string SeedPress01 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-01-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-02-Assembly part file.</summary>
        public const string SeedPress02 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-02-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-03-Assembly part file.</summary>
        public const string SeedPress03 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-03-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-04-Assembly part file.</summary>
        public const string SeedPress04 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-04-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-05-Assembly part file.</summary>
        public const string SeedPress05 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-05-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-06-Assembly part file.</summary>
        public const string SeedPress06 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-06-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-07-Assembly part file.</summary>
        public const string SeedPress07 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-07-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-08-Assembly part file.</summary>
        public const string SeedPress08 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-08-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-09-Assembly part file.</summary>
        public const string SeedPress09 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-09-Assembly.prt";

        /// <summary>Returns the path to the XXXX-Press-10-Assembly part file.</summary>
        public const string SeedPress10 = "G:\\0Library\\SeedFiles\\Press\\XXXX-Press-10-Assembly.prt";

        /// <summary>Returns the path to the Dummy part file.</summary>
        public const string DummyPath = "G:\\0Library\\SeedFiles\\Components\\Dummy.prt";

        /// <summary>Returns the path to the seed-prog-base part file.</summary>
        public const string SeedProgBase = "G:\\0Library\\SeedFiles\\Components\\seed-base.prt";

        /// <summary>Returns the path to the seed 0000-simulation part file.</summary>
        //public const string SeedSimulation = "G:\\0start\\simulation\\0000-simulation.prt";
        public const string SeedSimulation = "G:\\0Template\\0start\\Simulation\\0000-simulation.prt";


        /// <summary>Returns the path to the seed xxxx-History part file.</summary>
        public const string SeedHistory = "G:\\0start\\Math Data\\History\\xxxx-History.prt";

        /// <summary>Returns the path to the seed xxxx-Master part file.</summary>
        public const string SeedMaster = "G:\\0start\\Math Data\\Go\\xxxx-Master.prt";

        /// <summary>Returns the path to the run_managed executable file.</summary>
        public const string RunManaged = "C:\\Program Files\\Siemens\\NX 11.0\\NXBIN\\run_managed.exe";

        /// <summary>Returns the path to the seed dieset control file.</summary>
        public const string SeedDiesetControl = "G:\\0Library\\SeedFiles\\Assembly\\seed-dieset-control.prt";


        public const int Color_Trim = 186;

        public const int Color_Finish = 42;


        public const string Refset_EntirePart = "Entire Part";

        public const string Refset_Empty = "Empty";

        public const string Refset_Body = "BODY";

        public const string Refset_SubTool = "SUB_TOOL";

        public const string Refset_CBore = "CBORE";

        public const string Refset_ShortTap = "SHORT-TAP";

        public const string Refset_Ream = "REAM";

        public const string Refset_ReamShort = "REAM_SHORT";

        public const string Refset_BodyEdge = "BODY_EDGE";

        public const string Refset_ClrHole = "CLR_HOLE";

        public const string Refset_ClrScrewHead = "CLR_SCREW_HEAD";

        public const string Refset_HardTapClr = "HARD_TAP_CLR";

        public const string Refset_SlotCBore = "SLOT_CBORE";

        public const string Refset_CBoreBlind = "CBORE_BLIND";

        public const string Refset_CBoreBlindOpp = "CBORE_BLIND_OPP";

        public const string Refset_Grid = "GRID";

        public const string Refset_Tap = "TAP";

        public const string Refset_Handling = "HANDLING";

        public const string Refset_Tooling = "TOOLING";

        public const string Refset_BlindReam = "BLIND_REAM";

        public const string Refset_WireTap = "WIRE_TAP";


        public const double Tolerance_English = .001;

        public const double Tolerance_Metric = Tolerance_English * 25.4;


        public const string ShcsCBoreHolechart = "SHCS_CBORE_HOLECHART";

        public const string ShcsCBoreDepth = "SHCS_CBORE_DEPTH";

        public const string ShcsCBoreHeadDia = "SHCS_CBORE_HEAD_DIA";

        public const string ShcsShortTapHolechart = "SHCS_SHORT_TAP_HOLECHART";

        public const string ShcsTapHolechart = "SHCS_TAP_HOLECHART";

        public const string ShcsShortTapDepth = "SHCS_SHORT_TAP_DEPTH";

        public const string ShcsTapDepth = "SHCS_TAP_DEPTH";

        #endregion

        #region ConstraintReference

        public static bool __GeometryDirectionReversed(ConstraintReference cons)
        {
            return cons.GeometryDirectionReversed;
        }

        public static NXObject __GetGeometry(ConstraintReference cons)
        {
            return cons.GetGeometry();
        }

        public static bool __GetHasPerpendicularVector(ConstraintReference cons)
        {
            return cons.GetHasPerpendicularVector();
        }

        public static NXObject __GetMovableObject(ConstraintReference cons)
        {
            return cons.GetMovableObject();
        }

        public static NXObject __GetPrototypeGeometry(ConstraintReference cons)
        {
            return cons.GetPrototypeGeometry();
        }

        public static Vector3d __GetPrototypePerpendicularVector(ConstraintReference cons)
        {
            return cons.GetPrototypePerpendicularVector();
        }

        public static bool __GetUsesGeometryAxis(ConstraintReference cons)
        {
            return cons.GetUsesGeometryAxis();
        }

        public static Point3d __HelpPoint(ConstraintReference cons)
        {
            return cons.HelpPoint;
        }

        public static ConstraintReference.ConstraintOrder __Order(
            ConstraintReference cons)
        {
            return cons.Order;
        }

        public static ConstraintReference.GeometryType __SolverGeometryType(
            ConstraintReference cons)
        {
            return cons.SolverGeometryType;
        }

        #endregion

        #region CoordinateSystem

        public static Vector3d _AxisX(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element._AxisX();
        }

        public static Vector3d _AxisY(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element._AxisY();
        }

        public static Vector3d _AxisZ(this CoordinateSystem coordinateSystem)
        {
            return coordinateSystem.Orientation.Element._AxisZ();
        }

        public static void __GetDirections(this CoordinateSystem obj)
        {
            obj.GetDirections(out var xDirection, out var yDirection);
        }

        [Obsolete]
        public static void __GetSolverCardSyntax(this CoordinateSystem obj)
        {
        }

        public static bool __IsTemporary(this CoordinateSystem obj)
        {
            return obj.IsTemporary;
        }

        [Obsolete]
        public static void __Label(this CoordinateSystem obj)
        {
        }

        public static NXMatrix __Orientation(this CoordinateSystem obj)
        {
            return obj.Orientation;
        }

        public static void __Orientation(this CoordinateSystem obj, Vector3d xDir, Vector3d yDir)
        {
            obj.SetDirections(xDir, yDir);
        }

        public static Point3d __Origin(this CoordinateSystem obj)
        {
            return obj.Origin;
        }

        #endregion

        #region Csys

        //ufsession_.Csys.AskCsysInfo
        //ufsession_.Csys.AskMatrixOfObject
        //ufsession_.Csys.AskMatrixValues
        //ufsession_.Csys.AskWcs

        #endregion

        #region Curve

        /// <summary>
        ///     Calculates the first derivative vector on the curve at a given parameter value
        /// </summary>
        /// <param name="curve">The curve</param>
        /// <param name="value">Parameter value</param>
        /// <returns>First derivative vector (not unitized)</returns>
        /// <remarks>
        ///     The vector returned is usually not a unit vector. In fact, it may even have zero<br />
        ///     length, in certain unusual cases.
        /// </remarks>
        public static Vector3d Derivative(this Curve curve, double value)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[3];
            var point = array;
            var array2 = new double[3];
            var array3 = array2;
            value /= Factor;
            eval.Evaluate(evaluator, 1, value, point, array3);
            eval.Free(evaluator);
            var vector = array3._ToVector3d();
            return vector._Divide(Factor);
        }

        //
        // Summary:
        //     Calculates the parameter value at a point on the curve
        //
        // Parameters:
        //   point:
        //     The point
        //
        // Returns:
        //     Parameter value at the point (not unitized)
        //
        // Remarks:
        //     The Parameter function and the Position function are designed to work together
        //     smoothly -- each of these functions is the "reverse" of the other. So, if c is
        //     any curve and t is any parameter value, then
        //     c.Parameter(c.Position(t)) = t
        //     Also, if p is any point on the curve c, then
        //     c.Position(c.Parameter(p)) = p
        public static double __Parameter(this Curve curve, Point3d point)
        {
            var nXOpenTag = curve.Tag;
            var array = point._ToArray();
            var direction = 1;
            var offset = 0.0;
            var tolerance = 0.0001;
            var point_along_curve = new double[3];
            ufsession_.Modl.AskPointAlongCurve2(array, nXOpenTag, offset, direction, tolerance, point_along_curve,
                out var parameter);
            return (1.0 - parameter) * curve.__MinU() + parameter * curve.__MaxU();
        }

        //
        // Summary:
        //     The lower-limit parameter value (at the start-point of the curve)
        //
        // Remarks:
        //     If c is a curve, then c.Position(c.MinU) = c.StartPoint
        public static double __MinU(this Curve curve)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[0];
        }

        //
        // Summary:
        //     The upper-limit parameter value (at the end-point of the curve)
        //
        // Remarks:
        //     If c is a curve, then c.Position(c.MaxU) = c.EndPoint
        public static double __MaxU(this Curve curve)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[1];
        }

        public static Curve _Copy(this Curve curve)
        {
            switch (curve)
            {
                case Line line:
                    return line._Copy();
                case Arc arc:
#pragma warning disable CS0618 // Type or member is obsolete
                    return arc._Copy();
#pragma warning restore CS0618 // Type or member is obsolete
                case Ellipse ellipse:
                    return ellipse._Copy();
                case Parabola parabola:
#pragma warning disable CS0612 // Type or member is obsolete
                    return parabola._Copy();
#pragma warning restore CS0612 // Type or member is obsolete
                case Hyperbola hyperbola:
                    return hyperbola._Copy();
                case Spline spline:
                    return spline._Copy();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //
        // Summary:
        //     Calculates a point on the curve at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     The point corresponding to the given parameter value
        //
        // Remarks:
        //     If you want to calculate several points on a curve, the PositionArray functions
        //     might be more useful. The following example shows how the Position function can
        //     be used to calculate a sequence of points along a curve.
        public static Point3d Position(this Curve curve, double value)
        {
            //using(session_.using_evaluator(curve.Tag))
            //{

            //}

            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[3];
            var array2 = array;
            var array3 = new double[3];
            var derivatives = array3;
            value /= Factor;
            eval.Evaluate(evaluator, 0, value, array2, derivatives);
            eval.Free(evaluator);
            return array2._ToPoint3d();
        }

        //
        // Summary:
        //     Copies an array of NX.Curve (with no transform)
        //
        // Parameters:
        //   original:
        //     Original NX.Curve array
        //
        // Returns:
        //     A copy of the input curves
        //
        // Remarks:
        //     The new curves will be on the same layers as the original ones.
        //
        //     The function will throw an NXOpen.NXException, if the copy operation cannot be
        //     performed.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve[] Copy(this Curve curve, params Curve[] original)
        {
            var array = new Curve[original.Length];
            for (var i = 0; i < original.Length; i++) array[i] = original[i].Copy();

            return array;
        }

        //
        // Summary:
        //     Trim a curve to a parameter interval
        //
        // Parameters:
        //   lowerParam:
        //     The lower-limit parameter value
        //
        //   upperParam:
        //     The upper-limit parameter value
        [Obsolete(nameof(NotImplementedException))]
        public static void Trim(this Curve curve, double lowerParam, double upperParam)
        {
            //Part workPart = __work_part_;
            //TrimCurve trimCurve = null;
            //TrimCurveBuilder trimCurveBuilder = workPart.NXOpenPart.Features.CreateTrimCurveBuilder(trimCurve);
            //trimCurveBuilder.InteresectionMethod = TrimCurveBuilder.InteresectionMethods.UserDefined;
            //trimCurveBuilder.InteresectionDirectionOption = TrimCurveBuilder.InteresectionDirectionOptions.RelativeToWcs;
            //trimCurveBuilder.CurvesToTrim.AllowSelfIntersection(allowSelfIntersection: true);
            //trimCurveBuilder.CurvesToTrim.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);
            //trimCurveBuilder.CurveOptions.Associative = false;
            //trimCurveBuilder.CurveOptions.InputCurveOption = CurveOptions.InputCurve.Replace;
            //trimCurveBuilder.CurveExtensionType = TrimCurveBuilder.CurveExtensionTypes.Natural;
            //Point point = Create.Point(Position(lowerParam));
            //Point point2 = Create.Point(Position(upperParam));
            //Section section = Section.CreateSection(point);
            //Section section2 = Section.CreateSection(point2);
            //trimCurveBuilder.CurveList.Add(base.NXOpenTaggedObject, null, StartPoint);
            //SelectionIntentRule[] rules = Section.CreateSelectionIntentRule(this);
            //trimCurveBuilder.CurvesToTrim.AddToSection(rules, (NXOpen.Curve)this, null, null, StartPoint, NXOpen.Section.Mode.Create, chainWithinFeature: false);
            //trimCurveBuilder.FirstBoundingObject.Add(section.NXOpenSection);
            //trimCurveBuilder.SecondBoundingObject.Add(section2.NXOpenSection);
            //trimCurveBuilder.Commit();
            //trimCurveBuilder.Destroy();
            //section.NXOpenSection.Destroy();
            //section2.NXOpenSection.Destroy();
            //NXObject.Delete(point);
            //NXObject.Delete(point2);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Divide a curve at an array of parameter values
        //
        // Parameters:
        //   parameters:
        //     The parameter values at which the curve should be divided
        //
        // Returns:
        //     An array of Snap.NX.Curve objects
        //
        // Remarks:
        //     The function will create new curves, and the original one will be unchanged.
        //     If you want to modify the extents of an existing curve, please use the Snap.NX.Curve.Trim
        //     function.
        //
        //     SNAP also provides functions for dividing specific types of curves, which may
        //     be more convenient, in many cases. Links to these functions are provided in the
        //     "See Also" section below.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve[] Divide(this Curve curve, params double[] parameters)
        {
            //Curve curve = Copy();
            //Part workPart = __work_part_;
            //DivideCurveBuilder divideCurveBuilder = workPart.NXOpenPart.BaseFeatures.CreateDivideCurveBuilder(null);
            //divideCurveBuilder.Type = DivideCurveBuilder.Types.ByBoundingObjects;
            //BoundingObjectBuilder[] array = new BoundingObjectBuilder[parameters.Length];
            //Point[] array2 = new Point[parameters.Length];
            //for (int i = 0; i < parameters.Length; i++)
            //{
            //    array[i] = workPart.NXOpenPart.CreateBoundingObjectBuilder();
            //    array[i].BoundingPlane = null;
            //    array[i].BoundingObjectMethod = BoundingObjectBuilder.Method.ProjectPoint;
            //    array2[i] = Create.Point(curve.Position(parameters[i]));
            //    array[i].BoundingProjectPoint = array2[i];
            //    divideCurveBuilder.BoundingObjects.Append(array[i]);
            //}

            //NXOpen.View workView = workPart.NXOpenPart.ModelingViews.WorkView;
            //divideCurveBuilder.DividingCurve.SetValue(curve, workView, curve.StartPoint);
            //divideCurveBuilder.Commit();
            //NXOpen.NXObject[] committedObjects = divideCurveBuilder.GetCommittedObjects();
            //divideCurveBuilder.Destroy();
            //Curve[] array3 = new Curve[committedObjects.Length];
            //for (int j = 0; j < array3.Length; j++)
            //{
            //    array3[j] = CreateCurve((NXOpen.Curve)committedObjects[j]);
            //}

            //NXObject.Delete(array2);
            //return array3;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Divide a curve at its intersection with another curve
        //
        // Parameters:
        //   boundingCurve:
        //     Bounding curve to be used to divide the given curve
        //
        //   helpPoint:
        //     A point near the desired dividing point
        //
        // Returns:
        //     An array of two Snap.NX.Curve objects
        //
        // Remarks:
        //     This function will create two new curves, and the original one will be unchanged.
        //     If you want to modify the extents of an existing curve, please use the Snap.NX.Curve.Trim
        //     function.
        //
        //     SNAP also provides functions for dividing specific types of curves, which may
        //     be more convenient, in many cases. Links to these functions are provided in the
        //     "See Also" section below.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve[] Divide(this Curve curve, ICurve boundingCurve,
            Point3d helpPoint)
        {
            //Compute.IntersectionResult intersectionResult = Compute.IntersectInfo(this, boundingCurve, helpPoint);
            //return Divide(intersectionResult.CurveParameter);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Divide a curve at its intersection with a given face
        //
        // Parameters:
        //   face:
        //     A face to be used to divide the given curve
        //
        //   helpPoint:
        //     A point near the desired dividing point
        //
        // Returns:
        //     An array of two Snap.NX.Curve objects
        //
        // Remarks:
        //     This function will create two new curves, and the original one will be unchanged.
        //     If you want to modify the extents of an existing curve, please use the Snap.NX.Curve.Trim
        //     function.
        //
        //     SNAP also provides functions for dividing specific types of curves, which may
        //     be more convenient, in many cases. Links to these functions are provided in the
        //     "See Also" section below.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve[] Divide(this Curve curve, Face face, Point3d helpPoint)
        {
            //Compute.IntersectionResult intersectionResult = Compute.IntersectInfo(this, face, helpPoint);
            //return Divide(intersectionResult.CurveParameter);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Divide a curve at its intersection with a given plane
        //
        // Parameters:
        //   geomPlane:
        //     A plane to be used to divide the given curve
        //
        //   helpPoint:
        //     A point near the desired dividing point
        //
        // Returns:
        //     An array of two Snap.NX.Curve objects
        //
        // Remarks:
        //     This function will create two new curves, and the original one will be unchanged.
        //     If you want to modify the extents of an existing curve, please use the Snap.NX.Curve.Trim
        //     function.
        //
        //     SNAP also provides functions for dividing specific types of curves, which may
        //     be more convenient, in many cases. Links to these functions are provided in the
        //     "See Also" section below.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve[] Divide(this Curve curve, /* Surface.Plane geomPlane,*/
            Point3d helpPoint)
        {
            //Compute.IntersectionResult intersectionResult = Compute.IntersectInfo(this, geomPlane, helpPoint);
            //return Divide(intersectionResult.CurveParameter);
            throw new NotImplementedException();
        }

        public static bool _IsClosed(this Curve curve)
        {
            var result = ufsession_.Modl.AskCurveClosed(curve.Tag);

            switch (result)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(result);
            }
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapValue = nxopenValue
        //     * Factor
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenValue = snapValue
        //     / Factor
        internal static double __Factor(this Curve _)
        {
            return 1.0;
        }

        /// <summary>The lower u-value (at the start point of the curve)</summary>
        /// <param name="curve">The curve</param>
        /// <returns>The value at the start of the curve</returns>
        public static double _MinU(this Curve curve)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return 1.0 * array[0];
        }

        /// <summary>The upper u-value (at the start point of the curve)</summary>
        /// <param name="curve">The curve</param>
        /// <returns>The value at the end of the curve</returns>
        public static double _MaxU(this Curve curve)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return 1.0 * array[1];
        }

        /// <summary>Calculates a point on the icurve at a given parameter value</summary>
        /// <param name="curve">The curve</param>
        /// <param name="value">The parameter value</param>
        /// <returns>The <see cref="NXOpen.Point3d" /></returns>
        public static Point3d _Position(this Curve curve, double value)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[3];
            var array2 = array;
            var array3 = new double[3];
            var derivatives = array3;
            value /= 1.0;
            eval.Evaluate(evaluator, 0, value, array2, derivatives);
            eval.Free(evaluator);
            return array2._ToPoint3d();
        }

        public static Point3d _StartPoint(this Curve curve)
        {
            return curve._Position(curve._MinU());
        }

        public static Point3d _EndPoint(this Curve curve)
        {
            return curve._Position(curve._MaxU());
        }

        public static Point3d _MidPoint(this Curve curve)
        {
            var max = curve._MaxU();
            var min = curve._MinU();
            var diff = max - min;
            var quotient = diff / 2;
            var total = max - quotient;
            return curve._Position(total);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static void /* Box3d*/ Box(this Curve curve)
        {
            //double[] array = new double[3];
            //double[,] array2 = new double[3, 3];
            //double[] array3 = new double[3];
            //Tag tag = curve.Tag;
            //ufsession_.Modl.AskBoundingBoxExact(tag, Tag.Null, array, array2, array3);
            //var minXYZ = _Point3dOrigin;
            //var vector = new NXOpen.Vector3d(array2[0, 0], array2[0, 1], array2[0, 2]);
            //var vector2 = new NXOpen.Vector3d(array2[1, 0], array2[1, 1], array2[1, 2]);
            //var vector3 = new NXOpen.Vector3d(array2[2, 0], array2[2, 1], array2[2, 2]);
            //var maxXYZ = new NXOpen.Point3d((array + array3[0] * vector + array3[1] * vector2 + array3[2] * vector3).Array);
            //return new Box3d(minXYZ, maxXYZ);
            throw new NotImplementedException();
        }

        public static bool IsClosed(this Curve curve)
        {
            ufsession_.Modl.AskCurvePeriodicity(curve.Tag, out var status);
            switch (status)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw NXException.Create(status);
            }
        }

        //
        // Summary:
        //     Calculates unit normal at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit normal vector
        //
        // Remarks:
        //     The normal lies in the "osculating plane" of the curve at the given parameter
        //     value (the plane that most closely matches the curve). So, if the curve is planar,
        //     the normal always lies in the plane of the curve.
        //
        //     The normal always points towards the center of curvature of the curve. So, if
        //     the curve has an inflexion, the normal will flip from one side to the other,
        //     which may be undesirable.
        //
        //     The normal is the cross product of the binormal and the tangent: N = B*T
        public static Vector3d Normal(this Curve curve, double value)
        {
            //UFEval eval = ufsession_.Eval;
            //eval.Initialize2(NXOpenTag, out var evaluator);
            //double[] array = new double[3];
            //double[] point = array;
            //double[] array2 = new double[3];
            //double[] tangent = array2;
            //double[] array3 = new double[3];
            //double[] array4 = array3;
            //double[] array5 = new double[3];
            //double[] binormal = array5;
            //value /= Factor;
            //eval.EvaluateUnitVectors(evaluator, value, point, tangent, array4, binormal);
            //eval.Free(evaluator);
            //return new Vector(array4);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Calculates the unit binormal at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit binormal
        //
        // Remarks:
        //     The binormal is normal to the "osculating plane" of the curve at the given parameter
        //     value (the plane that most closely matches the curve). So, if the curve is planar,
        //     the binormal is normal to the plane of the curve.
        //
        //     The binormal is the cross product of the tangent and the normal: B = Cross(T,N).
        [Obsolete(nameof(NotImplementedException))]
        public static Vector3d Binormal(this Curve curve, double value)
        {
            //UFEval eval = ufsession_.Eval;
            //eval.Initialize2(NXOpenTag, out var evaluator);
            //double[] array = new double[3];
            //double[] point = array;
            //double[] array2 = new double[3];
            //double[] tangent = array2;
            //double[] array3 = new double[3];
            //double[] normal = array3;
            //double[] array4 = new double[3];
            //double[] array5 = array4;
            //value /= Factor;
            //eval.EvaluateUnitVectors(evaluator, value, point, tangent, normal, array5);
            //eval.Free(evaluator);
            //return new Vector(array5);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Calculates curvature at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Curvature value (always non-negative)
        //
        // Remarks:
        //     Curvature is the reciprocal of radius of curvature. So, on a straight line, curvature
        //     is zero and radius of curvature is infinite. On a circle of radius 5, curvature
        //     is 0.2 (and radius of curvature is 5, of course).
        public static double Curvature(this Curve curve, double value)
        {
            //Vector[] array = Derivatives(value, 2);
            //double num = Vector.Norm(Vector.Cross(array[1], array[2]));
            //double num2 = Vector.Norm(array[1]);
            //double num3 = num2 * num2 * num2;
            //return num / num3;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Calculates the parameter value at a point on the curve
        //
        // Parameters:
        //   point:
        //     The point
        //
        // Returns:
        //     Parameter value at the point (not unitized)
        //
        // Remarks:
        //     The Parameter function and the Position function are designed to work together
        //     smoothly -- each of these functions is the "reverse" of the other. So, if c is
        //     any curve and t is any parameter value, then
        //
        //     c.Parameter(c.Position(t)) = t
        //
        //     Also, if p is any point on the curve c, then
        //
        //     c.Position(c.Parameter(p)) = p
        public static double Parameter(this Curve curve, Point3d point)
        {
            var nXOpenTag = curve.Tag;
            var array = point._ToArray();
            var direction = 1;
            var offset = 0.0;
            var tolerance = 0.0001;
            var point_along_curve = new double[3];
            ufsession_.Modl.AskPointAlongCurve2(array, nXOpenTag, offset, direction, tolerance, point_along_curve,
                out var parameter);
            return (1.0 - parameter) * curve._MinU() + parameter * curve._MaxU();
        }

        /// <summary>
        ///     Calculates the parameter value defined by an arclength step along a curve
        /// </summary>
        /// <param name="curve">The curve to find parameter value along</param>
        /// <param name="baseParameter">The curve parameter value at the starting location</param>
        /// <param name="arclength">The arclength increment along the curve (the length of our step)</param>
        /// <remarks>
        ///     This function returns the curve parameter value at the far end of a "step" along<br />
        ///     a curve. The start of the step is defined by a given parameter value, and the<br />
        ///     size of the step is given by an arclength along the curve. The arclength step<br />
        ///     may be positive or negative.
        /// </remarks>
        /// <returns>The curve parameter value at the far end of the step</returns>
        public static double Parameter(this Curve curve, double baseParameter, double arclength)
        {
            var direction = 1;

            if (arclength < 0.0)
                direction = -1;

            var array = curve.Position(baseParameter)._ToArray();
            var tolerance = 0.0001;
            var point_along_curve = new double[3];
            var uFSession = ufsession_;

            uFSession.Modl.AskPointAlongCurve2(
                array,
                curve.Tag,
                Math.Abs(arclength),
                direction,
                tolerance,
                point_along_curve,
                out var parameter);

            return parameter * (curve._MaxU() - curve._MinU()) + curve._MinU();
        }

        //
        // Summary:
        //     Calculates the parameter value at a fractional arclength value along a curve
        //
        //
        // Parameters:
        //   arclengthFraction:
        //     Fractional arclength along the curve
        //
        // Returns:
        //     Parameter value
        //
        // Remarks:
        //     The input is a fractional arclength. A value of 0 corresponds to the start of
        //     the curve, a value of 1 corresponds to the end-point, and values between 0 and
        //     1 correspond to interior points along the curve.
        //
        //     You can input arclength values outside the range 0 to 1, and this will return
        //     parameter values corresponding to points on the extension of the curve.
        public static double Parameter(this Curve curve, double arclengthFraction)
        {
            var arclength = curve.GetLength() * arclengthFraction;
            return curve.Parameter(curve._MinU(), arclength);
        }

        //
        // Summary:
        //     Copies an NX.Curve (with a null transform)
        //
        // Returns:
        //     A copy of the input curve
        //
        // Remarks:
        //     The new curve will be on the same layer as the original one.
        [Obsolete(nameof(NotImplementedException))]
        public static Curve Copy(this Curve curve)
        {
            //Transform xform = Transform.CreateTranslation(0.0, 0.0, 0.0);
            //return Copy(xform);
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Calculates points on a curve at given parameter values
        //
        // Parameters:
        //   values:
        //     Parameter values
        //
        // Returns:
        //     The points corresponding to the given parameter values
        //
        // Remarks:
        //     Calling this function is typically around 3× as fast as calling the Curve.Position
        //     function in a loop.
        [Obsolete(nameof(NotImplementedException))]
        public static Point3d[] _PositionArray(this Curve curve, double[] values)
        {
            throw new NotImplementedException();
            //UFEval eval = ufsession_.Eval;
            //eval.Initialize2(curve.Tag, out var evaluator);
            //double[] array = new double[3];
            //double[] array2 = array;
            //double[] array3 = new double[3];
            //double[] derivatives = array3;
            //double num = 1.0 / Factor;
            //var array4 = new NXOpen.Point3d[values.LongLength];
            //for (long num2 = 0L; num2 < values.LongLength; num2++)
            //{
            //    double parm = values[num2] * num;
            //    eval.Evaluate(evaluator, 0, parm, array2, derivatives);
            //    ref Position reference = ref array4[num2];
            //    reference = new Position(array2);
            //}

            //eval.Free(evaluator);
            //return array4;
        }


        //
        // Summary:
        //     Calculates the unit tangent vector at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit tangent vector
        //
        // Remarks:
        //     This function successfully calculates a unit tangent vector even in those (rare)
        //     cases where the derivative vector of the curve has zero length.
        public static Vector3d Tangent(this Curve curve, double value)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out var evaluator);
            var array = new double[3];
            var point = array;
            var array2 = new double[3];
            var array3 = array2;
            var array4 = new double[3];
            var normal = array4;
            var array5 = new double[3];
            var binormal = array5;
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, point, array3, normal, binormal);
            eval.Free(evaluator);
            return array3._ToVector3d();
        }


        [Obsolete(nameof(NotImplementedException))]
        public static Curve _Mirror(
            this Curve original,
            Surface.Plane plane)
        {
            //if(original is NXOpen.Line line)
            //    return line._Mi
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Curve _Mirror(
            this Curve bodyDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dimension

        //public static bool _IsAssociative(this Dimension dim)
        //{
        //    return !dim.IsRetained;
        //}

        //[Obsolete]
        //public static DrawingSheet _OwningDrawingSheet(this Dimension dim)
        //{
        //    throw new NotImplementedException();
        //    // ufsession_.View.AskViewDependentStatus(dim.Tag, out _, out string drawingSheetName);
        //    // return dim._OwningPart().DrawingSheets.ToArray().Single(__s=>__s.Name == drawingSheetName); 
        //    //dim._OwningPart().Dimensions.

        //    //dim.

        //    // ufsession_.View.AskViewDependentStatus(dim.Tag, out _, out string drawingSheetName);
        //    //print_(drawingSheetName);
        //    //foreach (NXOpen.Drawings.DrawingSheet s in dim._OwningPart().DrawingSheets)
        //    //    print_(s.Name);
        //    // return dim._OwningPart().DrawingSheets.ToArray().Single(__s => __s.Name.ToLower().Contains(drawingSheetName.ToLower()));
        //}
        //#endregion

        //#region DisplayedConstraint
        //public static Constraint __GetConstraint(
        //           this DisplayedConstraint displayedConstraint)
        //{
        //    return displayedConstraint.GetConstraint();
        //}

        //public static Part __GetConstraintPart(this DisplayedConstraint displayedConstraint)
        //{
        //    return displayedConstraint.GetConstraintPart();
        //}

        //public static Component __GetContextComponent(
        //    this DisplayedConstraint displayedConstraint)
        //{
        //    return displayedConstraint.GetContextComponent();
        //}

        #endregion

        #region DoubleArray

        private static Matrix3x3 _ToMatrix3x3(this double[] array)
        {
            return new Matrix3x3
            {
                Xx = array[0],
                Xy = array[1],
                Xz = array[2],
                Yx = array[3],
                Yy = array[4],
                Yz = array[5],
                Zx = array[6],
                Zy = array[7],
                Zz = array[8]
            };
        }

        public static Point3d _ToPoint3d(this double[] array)
        {
            return new Point3d(array[0], array[1], array[2]);
        }

        public static Vector3d _ToVector3d(this double[] array)
        {
            return new Vector3d(array[0], array[1], array[2]);
        }

        public static Vector3d __Multiply(this double d, Vector3d vector)
        {
            return vector._Multiply(d);
        }

        #endregion

        #region Edge

        public static Point3d _StartPoint(this Edge edge)
        {
            edge.GetVertices(out var vertex1, out _);
            return vertex1;
        }

        public static Point3d _EndPoint(this Edge edge)
        {
            edge.GetVertices(out _, out var vertex2);
            return vertex2;
        }

        public static Vector3d _NormalVector(this Edge edge)
        {
            if (edge.SolidEdgeType != Edge.EdgeType.Linear)
                throw new ArgumentException("Cannot ask for the vector of a non linear edge");

            return edge._StartPoint()._Subtract(edge._EndPoint());
        }

        /// <summary>
        ///     Gets the start and end positions of the {edge}.
        /// </summary>
        /// <remarks>{[0]=StartPoint, [1]=EndPoint}</remarks>
        /// <param name="edge">The edge to get the positions from.</param>
        /// <returns>The edge positions.</returns>
        public static bool _HasEndPoints(this Edge edge, Point3d pos1, Point3d pos2)
        {
            if (edge._StartPoint()._IsEqualTo(pos1) && edge._EndPoint()._IsEqualTo(pos2))
                return true;

            if (edge._StartPoint()._IsEqualTo(pos2) && edge._EndPoint()._IsEqualTo(pos1))
                return true;

            return false;
        }

        public static Curve _ToCurve(this Edge edge)
        {
            ufsession_.Modl.CreateCurveFromEdge(edge.Tag, out var ugcrv_id);
            return (Curve)session_.__GetTaggedObject(ugcrv_id);
        }

        //
        // Summary:
        //     The lower u-value -- the parameter value at the start-point of the edge
        public static double _MinU(this Edge edge)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(edge.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[0];
        }

        //
        // Summary:
        //     The upper u-value -- the parameter value at the end-point of the edge
        public static double _MaxU(this Edge edge)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(edge.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[1];
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapValue = nxopenValue
        //     * Factor
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenValue = snapValue
        //     / Factor
        internal static double __Factor(this Edge edge)
        {
            if (edge.SolidEdgeType == Edge.EdgeType.Elliptical ||
               edge.SolidEdgeType == Edge.EdgeType.Circular)
                return 180.0 / System.Math.PI;

            return 1.0;
        }

        //
        // Summary:
        //     Calculates curvature at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Curvature value (always non-negative)
        //
        // Remarks:
        //     Curvature is the reciprocal of radius of curvature. So, on a straight line, curvature
        //     is zero and radius of curvature is infinite. On a circle of radius 5, curvature
        //     is 0.2 (and radius of curvature is 5, of course).
        [Obsolete]
        public static double Curvature(double value)
        {
            //Vector[] array = Derivatives(value, 2);
            //double num = Vector.Norm(Vector.Cross(array[1], array[2]));
            //double num2 = Vector.Norm(array[1]);
            //double num3 = num2 * num2 * num2;
            //return num / num3;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Calculates the parameter value at a point on the edge
        //
        // Parameters:
        //   point:
        //     The point
        //
        // Returns:
        //     Parameter value at the point
        [Obsolete]
        public static double __Parameter(this Edge edge, Point3d point)
        {
            throw new NotImplementedException();
            //UFSession uFSession = Globals.UFSession;
            //Tag nXOpenTag = NXOpenTag;
            //double[] array = point.Array;
            //int direction = 1;
            //double offset = 0.0;
            //double[] point_along_curve = new double[3];
            //double tolerance = Globals.DistanceTolerance / 10.0;
            //double result = 0.0;
            //double parameter = 0.0;
            //ObjectTypes.SubType objectSubType = ObjectSubType;
            //bool flag = objectSubType == ObjectTypes.SubType.EdgeIntersection;
            //bool flag2 = objectSubType == ObjectTypes.SubType.EdgeIsoCurve;
            //bool flag3 = false;
            //if (flag || flag2)
            //{
            //    CURVE_t val = default(CURVE_t);
            //    EDGE.ask_curve(PsEdge, &val);
            //    if (CURVE_t.op_Implicit(val) != ENTITY_t.op_Implicit(ENTITY_t.@null))
            //    {
            //        flag3 = true;
            //        double partUnitsToMeters = UnitConversion.PartUnitsToMeters;
            //        double num = point.X * partUnitsToMeters;
            //        double num2 = point.Y * partUnitsToMeters;
            //        double num3 = point.Z * partUnitsToMeters;
            //        VECTOR_t val2 = default(VECTOR_t);
            //        ((VECTOR_t)(ref val2))._002Ector(num, num2, num3);
            //        CURVE.parameterise_vector(val, val2, &result);
            //    }
            //}

            //if (!flag3)
            //{
            //    uFSession.Modl.AskPointAlongCurve2(array, nXOpenTag, offset, direction, tolerance, point_along_curve, out parameter);
            //    result = (1.0 - parameter) * MinU + parameter * MaxU;
            //}

            //return result;
        }

        //
        // Summary:
        //     Calculates the parameter value defined by an arclength step along an edge
        //
        // Parameters:
        //   baseParameter:
        //     The curve parameter value at the starting location
        //
        //   arclength:
        //     The arclength increment along the edge (the length of our step)
        //
        // Returns:
        //     The curve parameter value at the far end of the step
        //
        // Remarks:
        //     This function returns the curve parameter value at the far end of a "step" along
        //     an edge. The start of the step is defined by a given parameter value, and the
        //     size of the step is given by an arclength along the edge. The arclength step
        //     may be positive or negative.
        [Obsolete]
        public static double __Parameter(this Edge edge, double baseParameter, double arclength)
        {
            //int direction = 1;
            //if (arclength < 0.0)
            //{
            //    direction = -1;
            //}

            //double parameter = 0.0;
            //double tolerance = 0.0001;
            //double[] point_along_curve = new double[3];
            //double[] array = edge.Position(baseParameter).Array;
            //ufsession_.Modl.AskPointAlongCurve2(array, edge.Tag, System.Math.Abs(arclength), direction, tolerance, point_along_curve, out parameter);
            //return parameter * (edge.__MaxU() - edge.__MinU()) + edge.__MinU();
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Calculates the parameter value at a fractional arclength value along an edge
        //
        // Parameters:
        //   arclengthFraction:
        //     Fractional arclength along the edge
        //
        // Returns:
        //     Parameter value
        //
        // Remarks:
        //     The input is a fractional arclength. A value of 0 corresponds to the start of
        //     the edge, a value of 1 corresponds to the end-point, and values between 0 and
        //     1 correspond to interior points along the edge.
        //     You can input arclength values outside the range 0 to 1, and this will return
        //     parameter values corresponding to points on the extension of the edge.
        [Obsolete]
        public static double __Parameter(double arclengthFraction)
        {
            //double arclength = ArcLength * arclengthFraction;
            //return Parameter(MinU, arclength);
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     The lower u-value -- the parameter value at the start-point of the edge
        public static double __MinU(this Edge edge)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(edge.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[0];
        }

        //
        // Summary:
        //     The upper u-value -- the parameter value at the end-point of the edge
        public static double __MaxU(this Edge edge)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(edge.Tag, out var evaluator);
            var array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return Factor * array[1];
        }

        //
        // Summary:
        //     Calculates the unit binormal at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit binormal
        //
        // Remarks:
        //     The binormal is normal to the "osculating plane" of the curve at the given parameter
        //     value (the plane that most closely matches the curve). So, if the curve is planar,
        //     the binormal is normal to the plane of the curve.
        //     The binormal is the cross product of the tangent and the normal: B = Cross(T,N).
        public static Vector3d __Binormal(this Edge edge, double value)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(edge.Tag, out var evaluator);
            var point = new double[3];
            var tangent = new double[3];
            var normal = new double[3];
            var array = new double[3];
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, point, tangent, normal, array);
            eval.Free(evaluator);
            return array._ToVector3d();
        }

        #endregion

        #region EdgeBlend

        [Obsolete(nameof(NotImplementedException))]
        public static EdgeBlend _Mirror(
            this EdgeBlend edgeBlend,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static EdgeBlend _Mirror(
            this EdgeBlend edgeBlend,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Exception

        public static void _PrintException(this Exception ex)
        {
            ex._PrintException(null);
        }

        public static void _PrintException(this Exception ex, string userMessage)
        {
            _ = ex.GetType();
            _ = ex.Message;

            //int __nx_error_code = -1;

            //var error_lines = new List<string>();


            print_("///////////////////////////////////////////");

            if (!string.IsNullOrEmpty(userMessage))
                print_($"UserMessage: {userMessage}");

            print_($"Message: {ex.Message}");

            print_($"Exception: {ex.GetType()}");

            if (ex is NXException nx)
                print_($"Error Code: {nx.ErrorCode}");

            var methods = ex.StackTrace.Split('\n');

            //for (int i = 0; i < methods.Length; i++)
            //{
            //    string line = methods[i];

            //    int lastIndex = line.LastIndexOf('\\');

            //    if (lastIndex < 0)
            //        continue;

            //    string substring = line.Substring(lastIndex);

            //    print_($"[{i}]: {substring}");

            //    error_lines.Add($"[{i}]: {substring}");
            //}

            for (var i = 0; i < methods.Length; i++)
                //string line = ;
                //int lastIndex = line.LastIndexOf('\\');
                //if (lastIndex < 0)
                //    continue;
                //string substring = line.Substring(lastIndex);
                print_($"[{i}]: {methods[i]}");
            //error_lines.Add($"[{i}]: {substring}");
            print_("///////////////////////////////////////////");
            //foreach(var)
            //using (var cnn = new SqlConnection(conn_str))
            //{
            //    cnn.Open();

            //    using (var sql = new SqlCommand
            //    {
            //        Connection = cnn,

            //        CommandText = $@"insert into ufunc_exceptions
            //                        (ufunc_exception_type)
            //                        values
            //                        ('{ex.Message}')"


            //    })
            //        sql.ExecuteScalar();
            //}
        }

        #endregion

        #region ExtractFace

        [Obsolete]
        public static void _Layer(this ExtractFace ext)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public static void _Layer(this ExtractFace ext, int layer)
        {
            throw new NotImplementedException();
        }

        public static bool _IsLinkedBody(this ExtractFace extractFace)
        {
            return extractFace.FeatureType == "LINKED_BODY";
        }

        /// <summary>
        ///     Gets a value indicating whether or not this {extractFace} is broken.
        /// </summary>
        /// <remarks>Returns true if {extractFace} is broken, false otherwise.</remarks>
        /// <param name="nxExtractFace">The extractFace.</param>
        /// <returns>True or false.</returns>
        public static bool _IsBroken(this ExtractFace nxExtractFace)
        {
            UFSession.GetUFSession().Wave.IsLinkBroken(nxExtractFace.Tag, out var isBroken);
            return isBroken;
        }

        public static Tag _XFormTag(this ExtractFace extractFace)
        {
            ufsession_.Wave.AskLinkXform(extractFace.Tag, out var xform);
            return xform;
        }

        #endregion

        #region Extrude

        [Obsolete(nameof(NotImplementedException))]
        public static Extrude _Mirror(
            this Extrude extrude,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Extrude _Mirror(
            this Extrude extrude,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Face

        public static Point3d[] _EdgePositions(this Face __face)
        {
            //List<NXOpen.Point3d> points = new List<NXOpen.Point3d>();

            //foreach (NXOpen.Edge edge in __face.GetEdges())
            //{
            //    edge.GetVertices(out NXOpen.Point3d poin1, out NXOpen.Point3d point2);

            //    points.Add(poin1);
            //    points.Add(point2);
            //}

            //return points.ToHashSet(new EqualityPosition()).ToArray();

            throw new NotImplementedException();
        }


        public static Vector3d _NormalVector(this Face __face)
        {
            var point = new double[3];
            var dir = new double[3];
            var box = new double[6];
            ufsession_.Modl.AskFaceData(__face.Tag, out var type, point, dir, box, out var radius,
                out var rad_data, out var norm_dir);

            if (type != 22)
                throw new InvalidOperationException("Cannot ask for the normal of a non planar face");

            return dir._ToVector3d();
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapU = nxopenU * FactorU
        //
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenU = snapU / FactorU
        [Obsolete("Need to check what type of face first. Look at Snap")]
        internal static double __FactorU(this Face _)
        {
            throw new NotImplementedException();
            //return 180.0 / System.Math.PI;
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapV = nxopenV * FactorV
        //
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenV = snapV / FactorV
        [Obsolete("Need to check what type of face first. Look at Snap")]
        internal static double __FactorV(this Face face)
        {
            //switch(face.SolidFaceType) { }


            //return UnitConversion.MetersToPartUnits;
            throw new NotImplementedException();
        }

        public static bool _IsPlanar(this Face face)
        {
            return face.SolidFaceType == Face.FaceType.Planar;
        }

        //
        // Summary:
        //     Finds surface (u,v) parameters at (or nearest to) a given point
        //
        // Parameters:
        //   point:
        //     The given point (which should be on or near to the surface)
        //
        // Returns:
        //     Surface (u,v) parameters at (or near to) the given point
        //
        // Remarks:
        //     The Parameters function and the Position function are designed to work together
        //     smoothly -- each of these functions is the "reverse" of the other. So, if f is
        //     any face and (u,v) is any pair of parameter values, then
        //
        //     f.Parameters(f.Position(u,v)) = (u,v)
        //
        //     Also, if p is any point on the face f, then
        //
        //     f.Position(c.Parameters(p)) = p
        //
        //     Note that this function finds parameter values on the underlying surface of the
        //     face. So, the values returned may correspond to a surface point that is actually
        //     outside the face.
        public static double[] Parameters(this Face face, Point3d point)
        {
            var evalsf = ufsession_.Evalsf;
            evalsf.Initialize2(face.Tag, out var evaluator);
            var array = point._ToArray();
            var srf_pos = default(UFEvalsf.Pos3);
            evalsf.FindClosestPoint(evaluator, array, out srf_pos);
            evalsf.Free(out evaluator);
            var uv = srf_pos.uv;
#pragma warning disable CS0618 // Type or member is obsolete
            uv[0] = face.__FactorU() * uv[0];
            uv[1] = face.__FactorV() * uv[1];
#pragma warning restore CS0618 // Type or member is obsolete
            return uv;
        }

        #endregion

        #region Feature

        public static string __FeatureType(Feature feat)
        {
            return feat.FeatureType;
        }

        public static Feature[] __GetAllChildren(Feature feat)
        {
            return feat.GetAllChildren();
        }

        public static Body[] __GetBodies(Feature feat)
        {
            return feat.GetBodies();
        }

        public static Feature[] __GetChildren(Feature feat)
        {
            return feat.GetChildren();
        }

        public static Edge[] __GetEdges(Feature feat)
        {
            return feat.GetEdges();
        }

        public static NXObject[] __GetEntities(Feature feat)
        {
            return feat.GetEntities();
        }

        public static Expression[] __GetExpressions(Feature feat)
        {
            return feat.GetExpressions();
        }

        public static Face[] __GetFaces(Feature feat)
        {
            return feat.GetFaces();
        }

        public static NXColor __GetFeatureColor(Feature feat)
        {
            return feat.GetFeatureColor();
        }

        public static Feature[] __GetParents(Feature feat)
        {
            return feat.GetParents();
        }

        public static Section[] __GetSections(Feature feat)
        {
            return feat.GetSections();
        }

        public static void __HideBody(Feature feat)
        {
            feat.HideBody();
        }

        public static void __HideParents(Feature feat)
        {
            feat.Unsuppress();
        }

        public static void __Highlight(Feature feat)
        {
            feat.Highlight();
        }

        public static void __LoadParentPart(Feature feat)
        {
            feat.LoadParentPart();
        }

        public static void __MakeCurrentFeature(Feature feat)
        {
            feat.MakeCurrentFeature();
        }

        public static void __RemoveParameters(Feature feat)
        {
            feat.RemoveParameters();
        }

        public static void __ShowBody(Feature feat, bool moveCurves)
        {
            feat.ShowBody(moveCurves);
        }

        public static void __ShowParents(Feature feat, bool moveCurves)
        {
            feat.ShowParents(moveCurves);
        }

        public static void __Suppress(Feature feat)
        {
            feat.Suppress();
        }

        public static int __Timestamp(Feature feat)
        {
            return feat.Timestamp;
        }

        public static void __Unhighlight(Feature feat)
        {
            feat.Unhighlight();
        }

        public static void __Unsuppress(Feature feat)
        {
            feat.Unsuppress();
        }

        #endregion

        #region FileNew

        //
        // Summary:
        //     Get the application name now required by NXOpen (since NX9) from the Snap enum
        //     value
        //
        // Parameters:
        //   fileNew:
        //     An NXOpen fileNew object
        //
        //   templateType:
        //     Template type
        //
        // Returns:
        //     Application name (template type name, really)
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        private static string GetAppName(FileNew fileNew, Templates templateType)
        {
            var result = "GatewayTemplate";
            if (templateType == Templates.AeroSheetMetal) result = SafeAppName(fileNew, "AeroSheetMetalTemplate");

            if (templateType == Templates.Assembly) result = SafeAppName(fileNew, "AssemblyTemplate");

            if (templateType == Templates.Modeling) result = SafeAppName(fileNew, "ModelTemplate");

            if (templateType == Templates.NXSheetMetal) result = SafeAppName(fileNew, "NXSheetMetalTemplate");

            if (templateType == Templates.RoutingElectrical) result = SafeAppName(fileNew, "RoutingElectricalTemplate");

            if (templateType == Templates.RoutingLogical) result = SafeAppName(fileNew, "RoutingLogicalTemplate");

            if (templateType == Templates.RoutingMechanical) result = SafeAppName(fileNew, "RoutingMechanicalTemplate");

            if (templateType == Templates.ShapeStudio) result = SafeAppName(fileNew, "StudioTemplate");

            return result;
        }

        //
        // Summary:
        //     Get the names of the available template files
        //
        // Parameters:
        //   fileNew:
        //     An NXOpen fileNew object
        //
        //   templateType:
        //     Template type
        //
        //   unit:
        //     Part units
        //
        // Returns:
        //     The appropriate template file
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="templateType"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static string GetTemplateFileName(FileNew fileNew, Templates templateType, Units unit)
        {
            var result = "Blank";
            if (unit == Units.MilliMeters)
            {
                if (templateType == Templates.AeroSheetMetal)
                    result = SafeTemplateName(fileNew, "aero-sheet-metal-mm-template.prt");

                if (templateType == Templates.Assembly) result = SafeTemplateName(fileNew, "assembly-mm-template.prt");

                if (templateType == Templates.Modeling)
                    result = SafeTemplateName(fileNew, "model-plain-1-mm-template.prt");

                if (templateType == Templates.NXSheetMetal)
                    result = SafeTemplateName(fileNew, "sheet-metal-mm-template.prt");

                if (templateType == Templates.RoutingElectrical)
                    result = SafeTemplateName(fileNew, "routing-elec-mm-template.prt");

                if (templateType == Templates.RoutingLogical)
                    result = SafeTemplateName(fileNew, "routing-logical-mm-template.prt");

                if (templateType == Templates.RoutingMechanical)
                    result = SafeTemplateName(fileNew, "routing-mech-mm-template.prt");

                if (templateType == Templates.ShapeStudio)
                    result = SafeTemplateName(fileNew, "shape-studio-mm-template.prt");
            }
            else
            {
                if (templateType == Templates.AeroSheetMetal)
                    result = SafeTemplateName(fileNew, "aero-sheet-metal-inch-template.prt");

                if (templateType == Templates.Assembly) result = SafeTemplateName(fileNew, "assembly-inch-template.prt");

                if (templateType == Templates.Modeling)
                    result = SafeTemplateName(fileNew, "model-plain-1-inch-template.prt");

                if (templateType == Templates.NXSheetMetal)
                    result = SafeTemplateName(fileNew, "sheet-metal-inch-template.prt");

                if (templateType == Templates.RoutingElectrical)
                    result = SafeTemplateName(fileNew, "routing-elec-inch-template.prt");

                if (templateType == Templates.RoutingLogical)
                    result = SafeTemplateName(fileNew, "routing-logical-inch-template.prt");

                if (templateType == Templates.RoutingMechanical)
                    result = SafeTemplateName(fileNew, "routing-mech-inch-template.prt");

                if (templateType == Templates.ShapeStudio)
                    result = SafeTemplateName(fileNew, "shape-studio-inch-template.prt");
            }

            return result;
        }

        //
        // Summary:
        //     Check that an application name is OK
        //
        // Parameters:
        //   fileNew:
        //     A fileNew object
        //
        //   testName:
        //     The name to be validated
        //
        // Returns:
        //     The input name, if it's OK, otherwise "GatewayTemplate"
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private static string SafeAppName(FileNew fileNew, string testName)
        {
            var applicationNames = fileNew.GetApplicationNames();
            var result = "GatewayTemplate";
            if (Array.IndexOf(applicationNames, testName) > -1) result = testName;

            return result;
        }

        //
        // Summary:
        //     Check that a template file name is OK
        //
        // Parameters:
        //   fileNew:
        //     A fileNew object
        //
        //   testName:
        //     The name to be validated
        //
        // Returns:
        //     The input name, if it's OK, otherwise "Blank"
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private static string SafeTemplateName(FileNew fileNew, string testName)
        {
            var availableTemplates = fileNew.GetAvailableTemplates();
            var result = "Blank";

            if (Array.IndexOf(availableTemplates, testName) > -1)
                result = testName;

            return result;
        }

        #endregion

        #region Globals

        /// <summary>A function that evaluates a position at a point on a curve</summary>
        /// <param name="data">Data item to be used in evaluation</param>
        /// <param name="t">Parameter value at which to evaluate (in range 0 to 1)</param>
        /// <returns>NXOpen.Point3d on curve at given parameter value</returns>
        /// <remarks>
        ///     <para>
        ///         You use a CurvePositionFunction when constructing approximating curves using
        ///         the
        ///         <see cref="M:Snap.Create.BezierCurveFit(Snap.Create.CurvePositionFunction,System.Object,System.Int32)">BezierCurveFit</see>
        ///         function.
        ///     </para>
        /// </remarks>
        /// <seealso cref="M:Snap.Create.BezierCurveFit(Snap.Create.CurvePositionFunction,System.Object,System.Int32)">BezierCurveFit</seealso>
        public delegate Point3d CurvePositionFunction(object data, double t);

        /// <summary>A function that evaluates a position at a location on a surface</summary>
        /// <param name="data">Data item to be used in evaluation</param>
        /// <param name="uv">Parameter values at which to evaluate (in range [0,1] x [0,1])</param>
        /// <returns>NXOpen.Point3d on surface at given parameter value</returns>
        /// <remarks>
        ///     <para>
        ///         You use a SurfacePositionFunction when constructing approximating surfaces using
        ///         the
        ///         <see
        ///             cref="M:Snap.Create.BezierPatchFit(Snap.Create.SurfacePositionFunction,System.Object,System.Int32,System.Int32)">
        ///             BezierPatchFit
        ///         </see>
        ///         function.
        ///     </para>
        /// </remarks>
        /// <seealso
        ///     cref="M:Snap.Create.BezierPatchFit(Snap.Create.SurfacePositionFunction,System.Object,System.Int32,System.Int32)">
        ///     BezierPatchFit
        /// </seealso>
        public delegate Point3d SurfacePositionFunction(object data, params double[] uv);

        public const string _printerCts = "\\\\ctsfps1.cts.toolingsystemsgroup.com\\CTS Office MFC";

        public const string _simActive = "P:\\CTS_SIM\\Active";

        public static readonly IDictionary<string, ISet<string>> PrefferedDict = new Dictionary<string, ISet<string>>
        {
            ["006"] = new HashSet<string>
            {
                "6mm-shcs-010",
                "6mm-shcs-012.prt",
                "6mm-shcs-016.prt",
                "6mm-shcs-020.prt",
                "6mm-shcs-025.prt",
                "6mm-shcs-030.prt",
                "6mm-shcs-035.prt"
            },

            ["008"] = new HashSet<string>
            {
                "8mm-shcs-012",
                "8mm-shcs-016",
                "8mm-shcs-020",
                "8mm-shcs-025",
                "8mm-shcs-030",
                "8mm-shcs-035",
                "8mm-shcs-040",
                "8mm-shcs-045",
                "8mm-shcs-050",
                "8mm-shcs-055",
                "8mm-shcs-060",
                "8mm-shcs-065"
            },

            ["010"] = new HashSet<string>
            {
                "10mm-shcs-020",
                "10mm-shcs-030",
                "10mm-shcs-040",
                "10mm-shcs-050",
                "10mm-shcs-070",
                "10mm-shcs-090"
            },

            ["012"] = new HashSet<string>
            {
                "12mm-shcs-030",
                "12mm-shcs-040",
                "12mm-shcs-050",
                "12mm-shcs-070",
                "12mm-shcs-090",
                "12mm-shcs-110"
            },

            ["016"] = new HashSet<string>
            {
                "16mm-shcs-040",
                "16mm-shcs-055",
                "16mm-shcs-070",
                "16mm-shcs-090",
                "16mm-shcs-110"
            },

            ["020"] = new HashSet<string>
            {
                "20mm-shcs-050",
                "20mm-shcs-070",
                "20mm-shcs-090",
                "20mm-shcs-110",
                "20mm-shcs-150"
            },

            ["0375"] = new HashSet<string>
            {
                "0375-shcs-075",
                "0375-shcs-125",
                "0375-shcs-200",
                "0375-shcs-300",
                "0375-shcs-400"
            },

            ["0500"] = new HashSet<string>
            {
                "0500-shcs-125",
                "0500-shcs-200",
                "0500-shcs-300",
                "0500-shcs-400",
                "0500-shcs-500"
            },

            ["0625"] = new HashSet<string>
            {
                "0625-shcs-125",
                "0625-shcs-200",
                "0625-shcs-300",
                "0625-shcs-400",
                "0625-shcs-500"
            },

            ["0750"] = new HashSet<string>
            {
                "0750-shcs-200",
                "0750-shcs-300",
                "0750-shcs-400",
                "0750-shcs-500",
                "0750-shcs-650"
            }
        };

        public static UI TheUISession = UI.GetUI();


        internal static readonly UFSession ufsession_ = UFSession.GetUFSession();

        public static Session session_ = GetSession();

        public static double Factor => 1.0;

        public static Point3d _Point3dOrigin
        {
            get { return new[] { 0d, 0d, 0d }._ToPoint3d(); }
        }

        public static Matrix3x3 _Matrix3x3Identity
        {
            get
            {
                var array = new double[9];
                ufsession_.Mtx3.Identity(array);
                return array._ToMatrix3x3();
            }
        }

        /// <summary>
        ///     Multiply by this number to convert Part Units to Millimeters (1 or 25.4)
        /// </summary>
        internal static double PartUnitsToMillimeters => MillimetersPerUnit;

        /// <summary>
        ///     Multiply by this number to convert Millimeters to Part Units (1 or 0.04)
        /// </summary>
        internal static double MillimetersToPartUnits => 1.0 / PartUnitsToMillimeters;

        /// <summary>
        ///     Multiply by this number to convert Part Units to Inches (1 or 0.04)
        /// </summary>
        internal static double PartUnitsToInches => InchesPerUnit;

        /// <summary>
        ///     Multiply by this number to convert Inches to Part Units (either 1 or 25.4)
        /// </summary>
        internal static double InchesToPartUnits => 1.0 / PartUnitsToInches;

        //
        // Summary:
        //     Multiply by this number to convert Part Units to Meters, to go to Parasolid (0.001
        //     or 0.0254)
        internal static double PartUnitsToMeters => 0.001 * PartUnitsToMillimeters;

        //
        // Summary:
        //     Multiply by this number to convert Meters to Part Units, when coming from Parasolid
        //     (1000 or 40)
        internal static double MetersToPartUnits => 1000.0 * MillimetersToPartUnits;

        //
        // Summary:
        //     Multiply by this number to convert Part Units to Points (for font sizes)
        internal static double PartUnitsToPoints => PartUnitsToInches * 72.0;

        //
        // Summary:
        //     Multiply by this number to convert Points to Part Units (for font sizes)
        internal static double PointsToPartUnits => 1.0 / 72.0 * InchesToPartUnits;


        /// <summary>
        ///     Returns the current workComponent origin in terms of the current DisplayPart.
        ///     If the workPart equals the current displayPart then returns the Absolute Origin.
        /// </summary>
        public static Point3d WorkCompOrigin => throw
            //if (TSG_Library.Extensions.__work_part_.Tag == TSG_Library.Extensions.DisplayPart.Tag) return BaseOrigin;
            //if (!(NXOpen.Session.GetSession().Parts.WorkComponent != null)) throw new System.Exception("NullWorkComponentException");
            //return NXOpen.Assemblies.Component.Wrap(NXOpen.Session.GetSession().Parts.WorkComponent.Tag).Position;
            new NotImplementedException();

        //"CTS Office MFC on ctsfps1.cts.toolingsystemsgroup.com";

        public static string PrinterCts { get; } = _printerCts;

        public static string SimActive { get; } = _simActive;


        public static Part __display_part_
        {
            get => GetSession().Parts.Display;

            set => GetSession().Parts.SetDisplay(value, false, false, out _);
        }

        public static Part __work_part_
        {
            get => GetSession().Parts.Work;

            set => GetSession().Parts.SetWork(value);
        }

        public static Part _WorkPart
        {
            get => GetSession().Parts.Work;

            set => GetSession().Parts.SetWork(value);
        }

        public static UFSession _UFSession => ufsession_;

        public static UI uisession_ => UI.GetUI();

        public static CartesianCoordinateSystem __wcs_
        {
            get
            {
                ufsession_.Csys.AskWcs(out var wcs_id);
                return (CartesianCoordinateSystem)session_.__GetTaggedObject(wcs_id);
            }
            set => ufsession_.Csys.SetWcs(value.Tag);
        }


        public static Component __work_component_
        {
            get => GetSession().Parts.WorkComponent is null
                ? null
                : GetSession().Parts.WorkComponent;

            set => GetSession().Parts.SetWorkComponent(
                value,
                PartCollection.RefsetOption.Current,
                PartCollection.WorkComponentOption.Given,
                out _);
        }


        public static UFSession uf_ => ufsession_;

        public static UFSession TheUFSession { get; } = ufsession_;

        public static string TodaysDate
        {
            get
            {
                var day = DateTime.Today.Day < 10
                    ? '0' + DateTime.Today.Day.ToString()
                    : DateTime.Today.Day.ToString();
                var month = DateTime.Today.Month < 10
                    ? '0' + DateTime.Today.Month.ToString()
                    : DateTime.Today.Month.ToString();
                return DateTime.Today.Year + "-" + month + "-" + day;
            }
        }

        public static int __process_id => Process.GetCurrentProcess().Id;

        public static string __user_name => Environment.UserName;


        /// <summary>The work layer (the layer on which newly-created objects should be placed)</summary>
        /// <remarks>
        ///     <para>
        ///         When you change the work layer, the previous work layer is given the status "Selectable".
        ///     </para>
        /// </remarks>
        public static int WorkLayer
        {
            get => __work_part_.Layers.WorkLayer;
            set => __work_part_.Layers.WorkLayer = value;
        }


        /// <summary>Millimeters Per Unit (either 1 or 25.4)</summary>
        /// <remarks>
        ///     <para>
        ///         A constant representing the number of millimeters in one part unit.
        ///     </para>
        ///     <para>If UnitType == Millimeter, then MillimetersPerUnit = 1.</para>
        ///     <para>If UnitType == Inch, then MillimetersPerUnit = 25.4</para>
        /// </remarks>
        public static double MillimetersPerUnit => UnitType != Unit.Millimeter ? 25.4 : 1.0;

        /// <summary>Inches per part unit (either 1 or roughly 0.04)</summary>
        /// <remarks>
        ///     <para>
        ///         A constant representing the number of inches in one part unit.
        ///     </para>
        ///     <para>If UnitType = Millimeter, then InchesPerUnit = 0.0393700787402</para>
        ///     <para>If UnitType = Inch, then InchesPerUnit = 1.</para>
        /// </remarks>
        public static double InchesPerUnit => UnitType != Unit.Millimeter ? 1.0 : 5.0 / sbyte.MaxValue;

        /// <summary>Distance tolerance</summary>
        /// <remarks>
        ///     <para>
        ///         This distance tolerance is the same one that you access via Preferences → Modeling Preferences in interactive
        ///         NX.
        ///         In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        ///         The distance tolerance (together with the angle tolerance) controls the accuracy of this approximation, unless
        ///         you specify some over-riding tolerance within the function itself. For example, when you offset a curve, NX
        ///         will construct a spline curve that approximates the true offset to within the current distance tolerance.
        ///     </para>
        /// </remarks>
        public static double DistanceTolerance
        {
            get => __work_part_.Preferences.Modeling.DistanceToleranceData;
            set => __work_part_.Preferences.Modeling.DistanceToleranceData = value;
        }

        /// <summary>Angle tolerance, in degrees</summary>
        /// <remarks>
        ///     <para>
        ///         This angle tolerance is the same one that you access via Preference → Modeling Preferences in interactive NX.
        ///         In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        ///         The angle tolerance (together with the distance tolerance) controls the accuracy of this approximation, unless
        ///         you specify some over-riding tolerance within the function itself. For example, when you create a Through Curve
        ///         Mesh
        ///         feature in NX, the resulting surface will match the input curves to within the current distance and angle
        ///         tolerances.
        ///     </para>
        ///     <para>
        ///         The angle tolerance is expressed in degrees.
        ///     </para>
        /// </remarks>
        public static double AngleTolerance
        {
            get => __work_part_.Preferences.Modeling.AngleToleranceData;
            set => __work_part_.Preferences.Modeling.AngleToleranceData = value;
        }

        /// <summary>The chaining tolerance used in building "section" objects</summary>
        /// <remarks>
        ///     <para>
        ///         Most modeling features seem to set this internally to 0.95*DistanceTolerance,
        ///         so that's what we use here.
        ///     </para>
        /// </remarks>
        internal static double ChainingTolerance => 0.95 * DistanceTolerance;

        /// <summary>
        ///     If true, indicates that the modeling mode is set to History mode
        ///     (as opposed to History-free mode).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the same setting that you access via
        ///         Insert → Synchronous Modeling → History Mode in interactive NX.
        ///         Please refer to the NX documentation for a discussion of the History and
        ///         History-free modeling modes.
        ///     </para>
        ///     <para>
        ///         To create features in SNAP code, you must first set HistoryMode to True.
        ///     </para>
        /// </remarks>
        public static bool HistoryMode
        {
            get => __work_part_.Preferences.Modeling.GetHistoryMode();
            set
            {
                if (value)
                    __work_part_.Preferences.Modeling.SetHistoryMode();
                else
                    __work_part_.Preferences.Modeling.SetHistoryFreeMode();
            }
        }

        /// <summary>The unit type of the work part</summary>
        /// <remarks>
        ///     <para>
        ///         This property only gives the type of the unit.
        ///         To get a Snap.NX.Unit object, please use the
        ///         <see cref="P:TSG_Library.PartUnit">TSG_Library.PartUnit</see>
        ///         property, instead.
        ///     </para>
        /// </remarks>
        public static Unit UnitType
        {
            get
            {
                var workPart = __work_part_;
                var part_units = 0;
                ufsession_.Part.AskUnits(workPart.Tag, out part_units);
                return part_units == 1 ? Unit.Millimeter : Unit.Inch;
            }
        }

        /// <summary>
        ///     The work coordinate system (Wcs) of the work part
        /// </summary>
        public static CartesianCoordinateSystem Wcs
        {
            get
            {
                var uFSession = ufsession_;
                uFSession.Csys.AskWcs(out var wcs_id);
                var objectFromTag = (NXObject)session_.__GetTaggedObject(wcs_id);
                var csys = (CartesianCoordinateSystem)objectFromTag;
                return csys;
            }
            set
            {
                var nXOpenTag = value.Tag;
                ufsession_.Csys.SetWcs(nXOpenTag);
            }
        }

        //
        // Summary:
        //     The orientation of the Wcs of the work part
        public static Matrix3x3 WcsOrientation => __wcs_.Orientation.Element;

        //set
        //{
        //    __wcs_.set
        //    NXOpen.Part __work_part_ = __work_part_;
        //    __work_part_.WCS.SetOriginAndMatrix(Wcs.Origin, value);
        //}
        /// <summary>
        /// </summary>
        public static CartesianCoordinateSystem WcsCoordinateSystem =>
            GetSession()
                .Parts.Display.CoordinateSystems
                .CreateCoordinateSystem(Wcs.Origin, __display_part_.WCS.__Orientation(), true);

        public static Vector3d _Vector3dX()
        {
            return new[] { 1d, 0d, 0d }._ToVector3d();
        }

        public static Vector3d _Vector3dY()
        {
            return new[] { 0d, 1d, 0d }._ToVector3d();
        }

        public static Vector3d _Vector3dZ()
        {
            return new[] { 0d, 0d, 1d }._ToVector3d();
        }

        public static void print_(object __object)
        {
            print_($"{__object}");
        }

        public static void prompt_(string message)
        {
            uf_.Ui.SetPrompt(message);
        }

        public static void print_(bool __bool)
        {
            print_($"{__bool}");
        }

        public static void print_(int __int)
        {
            print_($"{__int}");
        }

        public static void print_(string message)
        {
            var lw = GetSession().ListingWindow;

            if (!lw.IsOpen)
                lw.Open();

            lw.WriteLine(message);
        }

        /// <summary>The length unit of the work part</summary>
        /// <remarks>
        /// <para>
        /// This will be either Snap.NX.Unit.Millimeter or Snap.NX.Unit.Inch
        /// </para>
        /// </remarks>
        //public static Unit PartUnit
        //{
        //    get
        //    {
        //        Unit unit = Unit.Millimeter;

        //        if (UnitType == Unit.Inch)
        //            unit = Unit.Inch;

        //        return unit;
        //    }
        //}

        /// <summary>Creates an Undo mark</summary>
        /// <param name="markVisibility">Indicates the visibility of the undo mark</param>
        /// <param name="name">The name to be assigned to the Undo mark</param>
        /// <returns>The ID of the newly created Undo mark</returns>
        /// <remarks>
        ///     <para>
        ///         Creating an Undo mark gives you a way to save the state of the NX session. Then,
        ///         at some later time, you can "roll back" to the Undo mark to restore NX to the saved state.
        ///         This is useful for error recovery, and for reversing any temporary changes you have made
        ///         (such as creation of temporary objects).
        ///     </para>
        ///     <para>
        ///         If you create a visible Undo mark, the name you assign will be shown in
        ///         the Undo List on the NX Edit menu.
        ///     </para>
        ///     <para>
        ///         Please refer to the NX/Open Programmer's Guide for more
        ///         information about Undo marks.
        ///     </para>
        /// </remarks>
        public static UndoMarkId SetUndoMark(MarkVisibility markVisibility, string name)
        {
            return GetSession().SetUndoMark(markVisibility, name);
        }

        /// <summary>Deletes an Undo mark</summary>
        /// <param name="markId">The ID of the Undo mark</param>
        /// <param name="markName">The name of the Undo mark.</param>
        /// <remarks>
        ///     <para>
        ///         You can access an Undo mark either using its ID or its name.
        ///         The system will try to find the mark using the given ID, first.
        ///         If this fails (because you have provided an incorrect ID), the system
        ///         will try again to find the mark based on its name.
        ///     </para>
        /// </remarks>
        public static void DeleteUndoMark(UndoMarkId markId, string markName)
        {
            GetSession().DeleteUndoMark(markId, markName);
        }

        /// <summary>Roll back to an existing Undo mark</summary>
        /// <param name="markId">The ID of the Undo mark to roll back to</param>
        /// <param name="markName">The name of the Undo mark.</param>
        /// <remarks>
        ///     <para>
        ///         You can access an Undo mark either using its ID or its name.
        ///         The system will try to find the mark using the given ID, first.
        ///         If this fails (because you have provided an incorrect ID), the system
        ///         will try again to find the mark based on its name.
        ///     </para>
        /// </remarks>
        public static void UndoToMark(UndoMarkId markId, string markName)
        {
            GetSession().UndoToMark(markId, markName);
        }

        public static string op_10_010(int __op)
        {
            if (__op == 0)
                return "000";
            if (__op < 10)
                throw new Exception("op integer must be 0 or greater than 9");
            if (__op < 100)
                return $"0{__op}";
            return $"{__op}";
        }

        public static string op_020_010(string __op)
        {
            return op_10_010(int.Parse(__op) - 10);
        }

        /// <summary>Get the number of objects on a specified layer</summary>
        /// <param name="layer">The layer number</param>
        /// <returns>The number of objects on the specified layer</returns>
        public static int LayerObjectCount(int layer)
        {
            return __work_part_.Layers.GetAllObjectsOnLayer(layer).Length;
        }

        /// <summary>Converts degrees to radians</summary>
        /// <param name="angle">An angle measured in degrees</param>
        /// <returns>The same angle measured in radians</returns>
        public static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        /// <summary>Converts radians to degrees</summary>
        /// <param name="angle">An angle measured in radians</param>
        /// <returns>The same angle measured in degrees</returns>
        public static double RadiansToDegrees(double angle)
        {
            return angle * 180.0 / Math.PI;
        }

        #endregion

        #region Hyperbola

        #endregion

        #region ICurve

        public static double __Parameter(this ICurve icurve, Point3d point)
        {
            switch (icurve)
            {
                case Curve __curve__:
                    return __Parameter(__curve__, point);
                case Edge __edge__:
                    return __Parameter(__edge__, point);
                default:
                    throw new ArgumentException("Unknown curve type");
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __StartPoint(this ICurve icurve)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __EndPoint(this ICurve icurve)
        {
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Calculates the unit binormal at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit binormal
        //
        // Remarks:
        //     The binormal is normal to the "osculating plane" of the curve at the given parameter
        //     value (the plane that most closely matches the curve). So, if the curve is planar,
        //     the binormal is normal to the plane of the curve.
        //
        //     The binormal is the cross product of the tangent and the normal: B = Cross(T,N).
        public static Vector3d __Binormal(this ICurve curve, double value)
        {
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.__Tag(), out var evaluator);
            var array = new double[3];
            var point = array;
            var array2 = new double[3];
            var tangent = array2;
            var array3 = new double[3];
            var normal = array3;
            var array4 = new double[3];
            var array5 = array4;
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, point, tangent, normal, array5);
            eval.Free(evaluator);
            return array5._ToVector3d();
        }

        public static Tag __Tag(this ICurve curve)
        {
            if (curve is TaggedObject taggedObject)
                return taggedObject.Tag;

            throw new ArgumentException("Curve was not a tagged object");
        }

        public static bool __IsCurve(this ICurve icurve)
        {
            return icurve is Curve;
        }

        public static bool __IsEdge(this ICurve icurve)
        {
            return icurve is Edge;
        }

        [Obsolete]
        public static void __IsLinearCurve(this ICurve icurve)
        {
            //if (icurve.ObjectType == ObjectTypes.Type.Line)
            //{
            //    throw new ArgumentException("The input curve is a straight line.");
            //}

            //if (icurve.ObjectType == ObjectTypes.Type.Edge && icurve.ObjectSubType == ObjectTypes.SubType.EdgeLine)
            //{
            //    throw new ArgumentException("The input curve is a straight line.");
            //}
            throw new NotImplementedException();
        }

        public static void __IsPlanar(this ICurve icurve)
        {
            var data = new double[6];
            ufsession_.Modl.AskObjDimensionality(icurve.__Tag(), out var dimensionality, data);

            if (dimensionality == 3)
                throw new ArgumentException("The input curve is not planar.");
        }

        [Obsolete]
        public static void __IsParallelToXYPlane(this ICurve icurve)
        {
            //double num = Vector.Angle(icurve.Binormal(icurve.MinU), Vector.AxisZ);
            //if (num > 1E-06 || num > 179.999999)
            //{
            //    throw new ArgumentException("The input curve does not lie in a plane parallel to X-Y plane.");
            //}
            throw new NotImplementedException();
        }

        #endregion

        #region IntegerArray

        public static Point3d _ToPoint3d(this int[] array)
        {
            return new Point3d(array[0], array[1], array[2]);
        }

        public static Vector3d _ToVector3d(this int[] array)
        {
            return new Vector3d(array[0], array[1], array[2]);
        }

        //public static Vector3d _ToVector3d(this int[] array)
        //{
        //    return new Vector3d(array[0], array[1], array[2]);
        //}

        #endregion

        #region Line

        [Obsolete(nameof(NotImplementedException))]
        public static Line _Mirror(
            this Line line,
            Surface.Plane plane)
        {
            var start = line.StartPoint.__Mirror(plane);
            var end = line.EndPoint.__Mirror(plane);
            return line.__OwningPart().Curves.CreateLine(start, end);
        }


        public static Line _Copy(this Line line)
        {
            if (line.IsOccurrence)
                throw new ArgumentException($@"Cannot copy {nameof(line)} that is an occurrence.", nameof(line));

            return line.__OwningPart().Curves.CreateLine(line._StartPoint(), line._EndPoint());
        }

        public static Point3d _StartPoint(this Line line)
        {
            return line.StartPoint;
        }

        public static Point3d _EndPoint(this Line line)
        {
            return line.EndPoint;
        }


        /// <summary>Construct a line, given x,y,z coordinates of its end-points</summary>
        /// <param name="x0">X-coordinate of start point</param>
        /// <param name="y0">Y-coordinate of start point</param>
        /// <param name="z0">Z-coordinate of start point</param>
        /// <param name="x1">X-coordinate of end   point</param>
        /// <param name="y1">Y-coordinate of end   point</param>
        /// <param name="z1">Z-coordinate of end   point</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        public static Line Line(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return Line(new Point3d(x0, y0, z0), new Point3d(x1, y1, z1));
        }

        /// <summary>Construct a line, given x,y coordinates of its end-points (z assumed zero)</summary>
        /// <param name="x0">X-coordinate of start point</param>
        /// <param name="y0">Y-coordinate of start point</param>
        /// <param name="x1">X-coordinate of end   point</param>
        /// <param name="y1">Y-coordinate of end   point</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        public static Line Line(double x0, double y0, double x1, double y1)
        {
            return Line(new Point3d(x0, y0, 0.0), new Point3d(x1, y1, 0.0));
        }

        /// <summary>Creates a line between two positions</summary>
        /// <param name="p0">NXOpen.Point3d for start of line</param>
        /// <param name="p1">NXOpen.Point3d for end of line</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        public static Line Line(Point3d p0, Point3d p1)
        {
            return __work_part_.Curves.CreateLine(p0, p1);
        }

        /// <summary>Creates a line between two points (NX.Point objects)</summary>
        /// <param name="pt0">Point at start of line</param>
        /// <param name="pt1">Point at end of line</param>
        /// <returns>A <see cref="T:Snap.NX.Line">Snap.NX.Line</see> object</returns>
        public static Line Line(Point pt0, Point pt1)
        {
            return CreateLine(pt0.Coordinates.X, pt0.Coordinates.Y, pt0.Coordinates.Z, pt1.Coordinates.X,
                pt1.Coordinates.Y, pt1.Coordinates.Z);
        }

        /// <summary>Creates an NX.Line object</summary>
        /// <param name="x0">X-coordinate of start point</param>
        /// <param name="y0">Y-coordinate of start point</param>
        /// <param name="z0">Z-coordinate of start point</param>
        /// <param name="x1">X-coordinate of end point</param>
        /// <param name="y1">Y-coordinate of end point</param>
        /// <param name="z1">Z-coordinate of end point</param>
        /// <returns>An NX.Line object</returns>
        internal static Line CreateLine(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            var startPoint = new Point3d(x0, y0, z0);
            var endPoint = new Point3d(x1, y1, z1);
            return __work_part_.Curves.CreateLine(startPoint, endPoint);
        }

        #endregion

        #region Masks

        public static Selection.MaskTriple[] mask = new Selection.MaskTriple[5];

        public static Selection.MaskTriple componentType =
            new Selection.MaskTriple(UF_component_type, 0, 0);

        public static Selection.MaskTriple edgeType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE);

        public static Selection.MaskTriple bodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static Selection.MaskTriple solidBodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static Selection.MaskTriple pointType = new Selection.MaskTriple(UF_caegeom_type, 0, 0);
        public static Selection.MaskTriple planeType = new Selection.MaskTriple(UF_plane_type, 0, 0);

        public static Selection.MaskTriple datumPlaneType =
            new Selection.MaskTriple(UF_datum_plane_type, 0, 0);

        public static Selection.MaskTriple datumCsysType =
            new Selection.MaskTriple(UF_csys_normal_subtype, 0, UF_csys_wcs_subtype);

        public static Selection.MaskTriple splineType = new Selection.MaskTriple(UF_spline_type, 0, 0);

        public static Selection.MaskTriple handlePointYpe =
            new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);

        public static Selection.MaskTriple objectType =
            new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);

        public static Selection.MaskTriple faceType =
            new Selection.MaskTriple(UF_face_type, UF_bounded_plane_type, 0);

        public static Selection.MaskTriple featureType =
            new Selection.MaskTriple(UF_feature_type, UF_feature_subtype, 0);

        public static Selection.MaskTriple objColorType =
            new Selection.MaskTriple(UF_solid_type, UF_face_type, UF_UI_SEL_FEATURE_ANY_FACE);

        /// <summary>
        ///     Returns the mask for an NXOpen.Assemblies.Component.
        /// </summary>
        public static UFUi.Mask ComponentMask =>
            new UFUi.Mask
            {
                object_type = UF_component_type,
                object_subtype = 0,
                solid_type = 0
            };

        public static UFUi.Mask BodyMask
        {
            get
            {
                var mask1 = new UFUi.Mask
                { object_type = 70, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }

        #endregion

        #region Matrix3x3

        /// <summary>
        ///     Maps an orientation from one coordinate system to another.
        /// </summary>
        /// <param name="orientation">The components of the given Orientation with the input coordinate system.</param>
        /// <param name="inputCsys">The input coordinate system.</param>
        /// <param name="outputCsys">The output coordinate system.</param>
        /// <returns>The components of the Orientation with the output coordinate system.</returns>
        public static Matrix3x3 MapCsysToCsys(
            Matrix3x3 orientation,
            CartesianCoordinateSystem inputCsys,
            CartesianCoordinateSystem outputCsys)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            var mappedXVector = __MapCsysToCsys(orientation._AxisX(), inputCsys, outputCsys);
            var mappedYVector = __MapCsysToCsys(orientation._AxisY(), inputCsys, outputCsys);
#pragma warning restore CS0612 // Type or member is obsolete
            return mappedXVector._ToMatrix3x3(mappedYVector);
        }


        public static Matrix3x3 _MirrorMap(this Matrix3x3 orientation, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            var newXVector = _MirrorMap(orientation._AxisY(), plane, fromComponent, toComponent);

            var newYVector = _MirrorMap(orientation._AxisX(), plane, fromComponent, toComponent);

            return newXVector._ToMatrix3x3(newYVector);
        }

        public static Vector3d _AxisY(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Yx, matrix.Yy, matrix.Yz);
        }

        public static Vector3d _AxisZ(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Zx, matrix.Zy, matrix.Zz);
        }

        public static Matrix3x3 __MapAcsToWcs(this Matrix3x3 __ori)
        {
            var x_vec = MapAcsToWcs(__ori._AxisX())._ToArray();
            var y_vec = MapAcsToWcs(__ori._AxisY())._ToArray();
            var z_vec = MapAcsToWcs(__ori._AxisZ())._ToArray();
            return x_vec.Concat(y_vec).Concat(z_vec).ToArray()._ToMatrix3x3();
        }


        public static double[] _Array(this Matrix3x3 matrix)
        {
            return new[]
            {
                matrix.Xx, matrix.Xy, matrix.Xz,
                matrix.Yx, matrix.Yy, matrix.Yz,
                matrix.Zx, matrix.Zy, matrix.Zz
            };
        }

        public static Vector3d _AxisX(this Matrix3x3 matrix)
        {
            return new Vector3d(matrix.Xx, matrix.Xy, matrix.Xz);
        }

        public static Matrix3x3 _Mirror(this Matrix3x3 matrix, Surface.Plane plane)
        {
            var new_y = matrix._AxisX()._Mirror(plane);
            var new_x = matrix._AxisY()._Mirror(plane);
            return new_x._ToMatrix3x3(new_y);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Matrix3x3 _Mirror(
            this Matrix3x3 vector,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }


        public static Matrix3x3 _Copy(this Matrix3x3 matrix)
        {
            var mtx_dst = new double[9];
            ufsession_.Mtx3.Copy(matrix._Array(), mtx_dst);
            return mtx_dst._ToMatrix3x3();
        }

        public static double[,] _ToTwoDimArray(this Matrix3x3 matrix)
        {
            return new double[3, 3]
            {
                { matrix.Xx, matrix.Xy, matrix.Xz },
                { matrix.Yx, matrix.Yy, matrix.Yz },
                { matrix.Zx, matrix.Zy, matrix.Zz }
            };
        }

        public static double _Determinant(this Matrix3x3 matrix)
        {
            ufsession_.Mtx3.Determinant(matrix._Array(), out var determinant);
            return determinant;
        }

        #endregion

        #region Modl

        #endregion

        #region NXObject

        public static void __DeleteUserAttribute(
            this NXObject nxobject,
            string title,
            Update.Option update_option = Update.Option.Now)
        {
            nxobject.DeleteUserAttribute(NXObject.AttributeType.String, title, true, update_option);
        }

        public static void NXOpen_NXObject(NXObject nxobject)
        {
            //nxobject.DeleteUserAttribute
            //nxobject.DeleteUserAttributes
            //nxobject.FindObject
            //nxobject.GetStringAttribute
            //nxobject.HasUserAttribute
            //nxobject.IsOccurrence
            //nxobject.JournalIdentifier
            //nxobject.OwningComponent
            //nxobject.OwningPart
            //nxobject.Print
            //nxobject.Prototype
            //nxobject.SetName
            //nxobject.Name
            //nxobject.SetUserAttribute
        }

        public static string __GetAttribute(this NXObject nXObject, string title)
        {
            return nXObject.GetUserAttributeAsString(title, NXObject.AttributeType.String, -1);
        }

        public static bool __HasAttribute(this NXObject nXObject, string title)
        {
            return nXObject.HasUserAttribute(title, NXObject.AttributeType.String, -1);
        }

        public static void __DeleteAttribute(this NXObject nXObject, string title)
        {
            nXObject.DeleteUserAttribute(NXObject.AttributeType.String, title, true, Update.Option.Now);
        }

        public static void __SetAttribute(this NXObject nxobject, string title, string value)
        {
            nxobject.SetUserAttribute(title, -1, value, Update.Option.Now);
        }

        public static Part __OwningPart(this NXObject nXObject)
        {
            return (Part)nXObject.OwningPart;
        }

        public static string __GetStringAttribute(this NXObject part, string title)
        {
            return part.GetUserAttributeAsString(title, NXObject.AttributeType.String, -1);
        }

        public static NXObject.AttributeInformation[] __GetAttributes(this NXObject component)
        {
            return component.GetUserAttributes();
        }

        #endregion

        #region Obj

        //public static partial class Extensions_
        //ufsession_.Obj.CycleByNameAndType
        //ufsession_.Obj.CycleByNameAndTypeExtended
        //ufsession_.Obj.CycleObjsInPart
        //ufsession_.Obj.CycleObjsInPart1
        //ufsession_.Obj.CycleTypedObjsInPart

        #endregion

        #region Part

        public static DrawingSheet __GetDrawingSheetOrNull(
            this Part part,
            string drawingSheetName,
            StringComparison stringComparison)
        {
            foreach (DrawingSheet drawingSheet in part.DrawingSheets)
                if (drawingSheet.Name.Equals(drawingSheetName, stringComparison))
                    return drawingSheet;

            return null;
        }

        public static Body __SolidBodyLayer1OrNull(this Part part)
        {
            var bodiesOnLayer1 = part.Bodies
                .OfType<Body>()
                .Where(body => !body.IsOccurrence)
                .Where(body => body.IsSolidBody)
                .Where(body => body.Layer == 1)
                .ToArray();

            return bodiesOnLayer1.Length == 1 ? bodiesOnLayer1[0] : null;
        }

        public static Body __SingleSolidBodyOnLayer1(this Part part)
        {
            var __solid_bodies__on_layer_1 = part.Bodies
                .ToArray()
                .Where(__body => __body.IsSolidBody)
                .Where(__body => __body.Layer == 1)
                .ToArray();

            switch (__solid_bodies__on_layer_1.Length)
            {
                case 0:
                    throw new InvalidOperationException($"Could not find a solid body on layer 1 in part {part.Leaf}");
                case 1:
                    return __solid_bodies__on_layer_1[0];
                default:
                    throw new InvalidOperationException(
                        $"Found {__solid_bodies__on_layer_1.Length} solid bodies on layer 1 in part {part.Leaf}");
            }
        }

        #endregion

        #region Section

        [Obsolete(nameof(NotImplementedException))]
        public static Section _Mirror(
            this Section section,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Section _Mirror(
            this Section section,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        //internal static SelectionIntentRule[] CreateSelectionIntentRule(params ICurve[] icurves)
        //{
        //    var list = new List<SelectionIntentRule>();

        //    for (var i = 0; i < icurves.Length; i++)
        //        if (icurves[i] is Curve curve)
        //        {
        //            var curves = new Curve[1] { curve };
        //            var item = __work_part_.ScRuleFactory.CreateRuleCurveDumb(curves);
        //            list.Add(item);
        //        }
        //        else
        //        {
        //            var edges = new Edge[1] { (Edge)icurves[i] };
        //            var item2 = __work_part_.ScRuleFactory.CreateRuleEdgeDumb(edges);
        //            list.Add(item2);
        //        }

        //    return list.ToArray();
        //}

        internal static void AddICurve(this Section section, params ICurve[] icurves)
        {
            var nXOpenSection = section;
            nXOpenSection.AllowSelfIntersection(false);

            for (var i = 0; i < icurves.Length; i++)
            {
                var rules = CreateSelectionIntentRule(icurves[i]);

                nXOpenSection.AddToSection(
                    rules,
                    (NXObject)icurves[i],
                    null,
                    null,
                    _Point3dOrigin,
                    Section.Mode.Create,
                    false);
            }
        }

        internal static SelectionIntentRule[] CreateSelectionIntentRule(params Point[] points)
        {
            var array = new Point[points.Length];

            for (var i = 0; i < array.Length; i++)
                array[i] = points[i];

            var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
            return new SelectionIntentRule[1] { curveDumbRule };
        }


        internal static void AddPoints(this Section section, params Point[] points)
        {
            var nXOpenSection = section;
            nXOpenSection.AllowSelfIntersection(false);
            var rules = CreateSelectionIntentRule(points);

            nXOpenSection.AddToSection(
                rules,
                points[0],
                null,
                null,
                _Point3dOrigin,
                Section.Mode.Create,
                false);
        }

        public static void Temp(this Section obj)
        {
            //obj.AddChainBetweenIntersectionPoints
            //obj.
        }

        #endregion

        #region Session

        /// <summary>Creates a new part</summary>
        /// <param name="pathName">The full path of the new part</param>
        /// <param name="templateType">The type of template to be used</param>
        /// <param name="unitType">The type of the unit</param>
        /// <returns>A <see cref="T:Snap.NX.Part">Snap.NX.Part</see> object</returns>
        public static Part Part(string pathName, Templates templateType, Units unitType)
        {
            return CreatePart(pathName, templateType, unitType);
        }


        //
        // Summary:
        //     Creates a new part
        //
        // Parameters:
        //   pathName:
        //     The full pathname of the part
        //
        //   templateType:
        //     The type of the template to be used to create the part
        //
        //   unitType:
        //     The type of the unit to be used
        //
        // Returns:
        //     An NX.Part object
        /// <summary>
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="templateType"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        internal static Part CreatePart(string pathName, Templates templateType, Units unitType)
        {
            var nXOpenPart = __work_part_;
            var nXOpenPart2 = __display_part_;
            var fileNew = session_.Parts.FileNew();
            fileNew.ApplicationName = GetAppName(fileNew, templateType);
            fileNew.TemplateFileName = GetTemplateFileName(fileNew, templateType, unitType);
            fileNew.UseBlankTemplate = fileNew.TemplateFileName == "Blank";

            fileNew.Units = unitType == Units.MilliMeters
                ? NXOpen.Part.Units.Millimeters
                : NXOpen.Part.Units.Inches;

            fileNew.NewFileName = pathName;
            fileNew.MasterFileName = "";
            fileNew.MakeDisplayedPart = true;
            var nXObject = fileNew.Commit();
            fileNew.Destroy();

            if (nXOpenPart != null)
            {
                __work_part_ = nXOpenPart;
                __display_part_ = nXOpenPart2;
            }

            return (Part)nXObject;
        }

        public static LockUiFromCustom using_lock_ui_from_custom(this Session _)
        {
            return new LockUiFromCustom();
        }

        public static string select_menu_item_14gt(this Session session_, string title, string[] items)
        {
            IList<string[]> separated = new List<string[]>();

            var list_items = items.ToList();

            const string __next__ = "...NEXT...";
            const int max = 14;

            if (items.Length == max)
                using (session_.using_lock_ui_from_custom())
                {
                    var picked_item = ufsession_.Ui.DisplayMenu(
                        title,
                        0,
                        items,
                        items.Length);

                    switch (picked_item)
                    {
                        case 0:
                            throw new Exception("Picked item was 0");
                        case 1:
                            throw new InvalidOperationException("Back was selected");
                        case 2:
                            throw new InvalidOperationException("Cancel was selected");
                        case 5:
                            return items[0];
                        case 6:
                            return items[1];
                        case 7:
                            return items[2];
                        case 8:
                            return items[3];
                        case 9:
                            return items[4];
                        case 10:
                            return items[5];
                        case 11:
                            return items[6];
                        case 12:
                            return items[7];
                        case 13:
                            return items[8];
                        case 14:
                            return items[9];
                        case 15:
                            return items[10];
                        case 16:
                            return items[11];
                        case 17:
                            return items[12];
                        case 18:
                            return items[13];
                        case 19:
                            throw new InvalidOperationException("Unable to display menu");
                        default:
                            throw new InvalidOperationException($"Unknown picked item: {picked_item}");
                    }
                }


            while (list_items.Count > 0)
            {
                var set_of_items = new string[max];

                set_of_items[set_of_items.Length - 1] = __next__;

                var end_index = set_of_items.Length - 1;

                if (list_items.Count < max)
                {
                    set_of_items = new string[list_items.Count];
                    end_index = list_items.Count;
                }

                for (var i = 0; i < end_index; i++)
                {
                    set_of_items[i] = list_items[0];
                    list_items.RemoveAt(0);
                }

                separated.Add(set_of_items);
            }


            var current_set_index = 0;

            while (true)
                using (session_.using_lock_ui_from_custom())
                {
                    var picked_item = ufsession_.Ui.DisplayMenu(
                        title,
                        0,
                        separated[current_set_index],
                        separated[current_set_index].Length);

                    switch (picked_item)
                    {
                        case 0:
                            throw new Exception("Picked item was 0");
                        case 1:
                            if (current_set_index > 0)
                                current_set_index--;
                            continue;
                        case 2:
                            throw new InvalidOperationException("Cancel was selected");
                        case 5:
                            return separated[current_set_index][0];
                        case 6:
                            return separated[current_set_index][1];
                        case 7:
                            return separated[current_set_index][2];
                        case 8:
                            return separated[current_set_index][3];
                        case 9:
                            return separated[current_set_index][4];
                        case 10:
                            return separated[current_set_index][5];
                        case 11:
                            return separated[current_set_index][6];
                        case 12:
                            return separated[current_set_index][7];
                        case 13:
                            return separated[current_set_index][8];
                        case 14:
                            return separated[current_set_index][9];
                        case 15:
                            return separated[current_set_index][10];
                        case 16:
                            return separated[current_set_index][11];
                        case 17:
                            return separated[current_set_index][12];
                        case 18:
                            if (separated[current_set_index][13] == __next__ && current_set_index + 1 < separated.Count)
                            {
                                current_set_index++;
                                continue;
                            }

                            if (current_set_index + 1 == separated.Count)
                                continue;

                            return separated[current_set_index][13];
                        case 19:
                            throw new InvalidOperationException("Unable to display menu");
                        default:
                            throw new InvalidOperationException($"Unknown picked item: {picked_item}");
                    }
                }
        }

        public static DoUpdate using_do_update(this Session _)
        {
            return new DoUpdate();
        }

        public static DoUpdate using_do_update(this Session _, string undo_text)
        {
            return new DoUpdate(undo_text);
        }

        //public static NXOpen.CartesianCoordinateSystem CreateCoordinateSystem(NXOpen.Point3d origin, NXOpen.NXMatrix matrix)
        //{
        //    ufsession_.Csys.CreateCsys(origin._ToArray(), matrix.Tag, out NXOpen.Tag csys_id);
        //    return (NXOpen.CartesianCoordinateSystem)session_._GetTaggedObject(csys_id);
        //}

        public static IDisposable using_form_show_hide(this Session _, Form __form,
            bool hide_form = true)
        {
            if (hide_form)
                __form.Hide();

            return new FormHideShow(__form);
        }

        //public static IDisposable using_form_show_hide(this NXOpen.Session _, System.Windows.Forms.Form __form)
        //    => new FormHideShow(__form);

        public static Part find_or_open(this Session session, string __path_or_leaf)
        {
            try
            {
                if (ufsession_.Part.IsLoaded(__path_or_leaf) == 0)
                    return session.Parts.Open(__path_or_leaf, out _);

                return (Part)session.Parts.FindObject(__path_or_leaf);
            }
            catch (NXException ex) when (ex.ErrorCode == 1020038)
            {
                throw NXException.Create(ex.ErrorCode, $"Invalid file format: '{__path_or_leaf}'");
            }
            catch (NXException ex) when (ex.ErrorCode == 1020001)
            {
                throw NXException.Create(ex.ErrorCode, $"File not found: '{__path_or_leaf}'");
            }
        }

        public static void _SaveAll(this Session session)
        {
            session.Parts.SaveAll(out _, out _);
            ;
        }

        public static void _CloseAll(this Session _)
        {
            ufsession_.Part.CloseAll();
        }


        public static void set_display_to_work(this Session _)
        {
            __work_part_ = __display_part_;
        }

        public static bool work_is_display(this Session _)
        {
            return __display_part_.Tag == __work_part_.Tag;
        }

        public static bool work_is_not_display(this Session session)
        {
            return !session.work_is_display();
        }

        public static Destroyer using_builder_destroyer(this Session _, Builder __builder)
        {
            return new Destroyer(__builder);
        }

        public static DisplayPartReset using_display_part_reset(this Session _)
        {
            return new DisplayPartReset();
        }

        public static SuppressDisplayReset using_suppress_display(this Session _)
        {
            return new SuppressDisplayReset();
        }

        public static IDisposable using_lock_ug_updates(this Session _)
        {
            return new LockUpdates();
        }

        public static IDisposable __UsingRegenerateDisplay(this Session _)
        {
            return new RegenerateDisplay();
        }

        public static void __DeleteObjects(this Session session_, params Tag[] __objects_to_delete)
        {
            delete_objects(session_, __objects_to_delete.Select(session_.__GetTaggedObject).ToArray());
        }

        public static TaggedObject __GetTaggedObject(this Session session, Tag tag)
        {
            return session.GetObjectManager().GetTaggedObject(tag);
        }

        public static PartCollection.SdpsStatus __SetActiveDisplay(this Session session_,
            Part __part)
        {
            if (session_.Parts.AllowMultipleDisplayedParts != PartCollection.MultipleDisplayedPartStatus.Enabled)
                throw new Exception("Session does not allow multiple displayed parts");

            return session_.Parts.SetActiveDisplay(
                __part,
                DisplayPartOption.AllowAdditional,
                PartDisplayPartWorkPartOption.UseLast,
                out _);
        }


        public static void __SelectSingleObject(
            this Session _,
            string message,
            string title,
            int scope,
            UFUi.SelInitFnT init_proc,
            IntPtr user_data,
            out int response,
            out Tag _object,
            double[] cursor,
            out Tag view)
        {
            ufsession_.Ui.SelectWithSingleDialog(message, title, scope, init_proc, user_data, out response, out _object,
                cursor, out view);
        }

        public static void _SelectSingleObject(
            this Session session,
            UFUi.SelInitFnT init_proc,
            IntPtr user_data,
            out int response,
            out Tag _object)
        {
            var cursor = new double[3];

            session.__SelectSingleObject(
                "Select Component Message",
                "Select Component Title",
                UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                init_proc,
                user_data,
                out response,
                out _object,
                cursor,
                out _);
        }

        public static Initialize2EvaluatorFree using_evaluator(this Session _, Tag tag)
        {
            return new Initialize2EvaluatorFree(tag);
        }

        public static TaggedObject find_by_name(this Session session, string __name)
        {
            return session.find_all_by_name(__name).First();
        }

        public static IEnumerable<TaggedObject> find_all_by_name(this Session _, string __name)
        {
            var __tag = Tag.Null;

            do
            {
                ufsession_.Obj.CycleByName(__name, ref __tag);

                if (__tag != Tag.Null)
                    yield return session_.__GetTaggedObject(__tag);
            } while (__tag != Tag.Null);
        }

        public static void delete_objects(this Session session_,
            params TaggedObject[] __objects_to_delete)
        {
            var mark =
                session_.SetUndoMark(MarkVisibility.Visible, "Delete Operation");

            session_.UpdateManager.ClearDeleteList();

            session_.UpdateManager.ClearErrorList();

            session_.UpdateManager.AddObjectsToDeleteList(__objects_to_delete);

            session_.UpdateManager.DoUpdate(mark);

            session_.DeleteUndoMark(mark, "");
        }

        //public static void print_(object __object) => print_($"{__object}");

        //public static void prompt_(string message) => ufsession_.Ui.SetPrompt(message);

        //public static void print_(bool __bool) => print_($"{__bool}");

        //public static void print_(int __int) => print_($"{__int}");

        //public static void print_(string message)
        //{
        //    NXOpen.ListingWindow lw = session_.ListingWindow;

        //    if (!lw.IsOpen)
        //        lw.Open();

        //    lw.WriteLine(message);
        //}

        //private static readonly NXOpen.UF.UFSession uf = NXOpen.UF.ufsession_.GetUFSession();

        public static IDisposable using_gc_handle(this Session _, GCHandle __handle)
        {
            return new GCHandleFree(__handle);
        }

        #endregion

        #region Sql

        public const string conn_str =
            "Data Source=tsgapps2.toolingsystemsgroup.com;Initial Catalog=CTSAPP;User ID=CTSAPP;Password=RI4SU9d2JxH8LcrxSDPS";


        public static object ExecuteScalar(string command_text)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                {
                    Connection = cnn,

                    CommandText = command_text
                })
                {
                    return sql.ExecuteScalar();
                }
            }
        }

        public static int ExecuteScalarInt(string command_text)
        {
            return Convert.ToInt32(Convert.ToDecimal(ExecuteScalar(command_text)));
        }

        public static SqlDataReader ExecuteDatReader(string command_text)
        {
            using (var cnn = new SqlConnection(conn_str))
            {
                cnn.Open();

                using (var sql = new SqlCommand
                {
                    Connection = cnn,

                    CommandText = command_text
                })
                {
                    return sql.ExecuteReader();
                }
            }
        }

        #endregion

        #region String

        private static bool FastenerInfo(string file, string regex, out string diameter, out string length)
        {
            if (file.Contains("\\"))
                file = Path.GetFileNameWithoutExtension(file);

            var match = Regex.Match(file, regex, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                diameter = string.Empty;

                length = string.Empty;

                return false;
            }

            diameter = match.Groups["diameter"].Value;

            length = match.Groups["length"].Value;

            return true;
        }


        public static bool _IsBlhcs_(this string leaf_or_display_name)
        {
            return leaf_or_display_name.ToLower().Contains("0375-bhcs-062")
                   || leaf_or_display_name.ToLower().Contains("10mm-bhcs-016");
        }

        public static bool _IsLayout_(this string leaf_or_display_name)
        {
            return leaf_or_display_name.ToLower().Contains("-layout");
        }

        public static bool _IsShcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-shcs-");
        }

        public static bool _IsShcs_(this Component component)
        {
            return component.DisplayName._IsShcs_();
        }

        public static bool _IsShcs_(this Part part)
        {
            return part.Leaf._IsShcs_();
        }

        public static bool _IsDwl_(this string leaf)
        {
            return leaf.ToLower().Contains("-dwl-");
        }

        public static bool _IsDwl_(this Component component)
        {
            return component.DisplayName._IsDwl_();
        }

        public static bool _IsDwl_(this Part part)
        {
            return part.Leaf._IsDwl_();
        }

        public static bool _IsJckScrew_(this string leaf)
        {
            return !leaf._IsJckScrewTsg_() && leaf.ToLower().Contains("-jck-screw-");
        }

        public static bool _IsJckScrew_(this Component component)
        {
            return component.DisplayName._IsJckScrew_();
        }

        public static bool _IsJckScrew_(this Part part)
        {
            return part.Leaf._IsJckScrew_();
        }

        public static bool _IsJckScrewTsg_(this string leaf)
        {
            return leaf.ToLower().Contains("-jck-screw-tsg");
        }

        public static bool _IsJckScrewTsg_(this Component component)
        {
            return component.DisplayName._IsJckScrewTsg_();
        }

        public static bool _IsJckScrewTsg_(this Part part)
        {
            return part.Leaf._IsJckScrewTsg_();
        }

        public static bool _IsLhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-lhcs-");
        }

        public static bool _IsSss_(this string leaf)
        {
            return leaf.ToLower().Contains("-sss-");
        }

        public static bool _IsBhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-bhcs-");
        }

        public static bool _IsFhcs_(this string leaf)
        {
            return leaf.ToLower().Contains("-fhcs-");
        }

        public static bool _IsFastener_(this string leaf_or_display_name)
        {
            return leaf_or_display_name._IsShcs_()
                   || leaf_or_display_name._IsDwl_()
                   || leaf_or_display_name._IsJckScrew_()
                   || leaf_or_display_name._IsJckScrewTsg_();
        }

        public static bool _IsFastenerExtended_(this string leaf_or_display_name)
        {
            return leaf_or_display_name._IsFastener()
                   || leaf_or_display_name._IsLhcs_()
                   || leaf_or_display_name._IsSss_()
                   || leaf_or_display_name._IsBhcs_()
                   || leaf_or_display_name._IsFhcs_()
                   || leaf_or_display_name._IsBlhcs_();
        }

        public static string _AskDetailNumber(this string file)
        {
            var leaf = Path.GetFileNameWithoutExtension(file);
            var match = Regex.Match(leaf, "^\\d+-\\d+-(?<detail>\\d+)$");

            if (!match.Success)
                throw new FormatException("Could not find detail number.");

            return match.Groups["detail"].Value;
        }

        public static bool _IsShcs(this string file)
        {
            return file._IsShcs(out _);
        }

        public static bool _IsDwl(this string file)
        {
            return file._IsDwl(out _);
        }

        public static bool _IsJckScrew(this string file)
        {
            return file._IsJckScrew(out _);
        }

        public static bool _IsJckScrewTsg(this string file)
        {
            return file._IsJckScrewTsg(out _);
        }

        public static bool _IsShcs(
            this string file,
            out string diameter,
            out string length)
        {
            return FastenerInfo(file, Regex_Shcs, out diameter, out length);
        }

        public static bool _IsDwl(
            this string file,
            out string diameter,
            out string length)
        {
            return FastenerInfo(file, Regex_Dwl, out diameter, out length);
        }

        public static bool _IsFastener(this string file)
        {
            return file._IsFastener(out _);
        }

        public static bool _IsShcs(
            this string file,
            out string diameter)
        {
            return file._IsShcs(out diameter, out _);
        }

        public static bool _IsDwl(
            this string file,
            out string diameter)
        {
            return file._IsDwl(out diameter, out _);
        }

        public static bool _IsJckScrew(
            this string file,
            out string diameter)
        {
            return FastenerInfo(file, Regex_JckScrew, out diameter, out _);
        }

        public static bool _IsJckScrewTsg(
            this string file,
            out string diameter)
        {
            return FastenerInfo(file, Regex_JckScrewTsg, out diameter, out _);
        }

        public static bool _IsFastener(this string file, out string diameter)
        {
            if (file._IsShcs(out diameter))
                return true;

            if (file._IsDwl(out diameter))
                return true;

            return file._IsJckScrew(out diameter) || file._IsJckScrewTsg(out diameter);
        }

        public static bool _IsLoaded(this string partName)
        {
            var status = ufsession_.Part.IsLoaded(partName);

            switch (status)
            {
                case 0: // not loaded
                    return false;
                case 1: // fully loaded
                case 2: // partially loaded
                    return true;
                default:
                    throw NXException.Create(status);
            }
        }

        public static bool _IsDetail(this string str)
        {
            var leaf = Path.GetFileNameWithoutExtension(str);

            if (leaf is null)
                return false;

            return Regex.IsMatch(leaf, "^\\d+-\\d+-\\d+$");
        }


        public static string _AskDetailOp(this string path)
        {
            var leaf = Path.GetFileNameWithoutExtension(path);

            var match = Regex.Match(leaf, "^\\d+-(?<op>\\d+)-\\d+$");

            if (!match.Success)
                throw new Exception($"could not find an op: '{leaf}'");

            return match.Groups["op"].Value;
        }


        public static bool IsAssemblyHolder(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            str = Path.GetFileNameWithoutExtension(str);
            var startIndex = str.LastIndexOf('-');

            if (startIndex < 0)
                return false;

            var str1 = str.Substring(startIndex);

            var strArray1 = new string[5]
            {
                "lwr",
                "upr",
                "lsh",
                "ush",
                "000"
            };

            var strArray2 = new string[2] { "lsp", "usp" };
            return strArray1.Any(str1.EndsWith) ||
                   strArray2.Any(str1.Contains);
        }

        public static bool IsFastener(string path)
        {
            return IsScrew(path) || IsDowel(path) || IsJigJackTsg(path) || IsJigJack(path);
        }

        public static bool IsScrew(string path)
        {
            return path.Contains("shcs");
        }

        public static bool IsDowel(string path)
        {
            return path.Contains("dwl");
        }

        public static bool IsJigJack(string path)
        {
            return path.Contains("jck-screw") && !path.Contains("tsg");
        }

        public static bool IsJigJackTsg(string path)
        {
            return path.Contains("jck-screw-tsg");
        }

        public static bool _IsPartDetail(this string partLeaf)
        {
            return Regex.IsMatch(partLeaf, DetailNumberRegex);
        }

        public static bool _IsAssemblyHolder(this string str)
        {
            return str._IsLsh() || str._IsUsh() || str._IsLwr() || str._IsUpr() || str._IsLsp() || str._IsUsp() ||
                   str._Is000();
        }

        public static bool _IsLsh(this string str)
        {
            return Regex.IsMatch(str, Regex_Lsh, RegexOptions.IgnoreCase);
        }

        public static bool _IsUsh(this string str)
        {
            return Regex.IsMatch(str, Regex_Ush, RegexOptions.IgnoreCase);
        }

        public static bool _IsLsp(this string str)
        {
            return Regex.IsMatch(str, Regex_Lsp, RegexOptions.IgnoreCase);
        }

        public static bool _IsUsp(this string str)
        {
            return Regex.IsMatch(str, Regex_Usp, RegexOptions.IgnoreCase);
        }

        public static bool _IsLwr(this string str)
        {
            return Regex.IsMatch(str, Regex_Lwr, RegexOptions.IgnoreCase);
        }

        public static bool _IsUpr(this string str)
        {
            return Regex.IsMatch(str, Regex_Upr, RegexOptions.IgnoreCase);
        }

        public static bool _Is000(this string str)
        {
            return Regex.IsMatch(str, Regex_Op000Holder, RegexOptions.IgnoreCase);
        }

        #endregion

        #region Tag

        public static TaggedObject _ToTaggedObject(this Tag tag)
        {
            return session_.GetObjectManager().GetTaggedObject(tag);
        }

        public static TSource _To<TSource>(this Tag tag) where TSource : TaggedObject
        {
            return (TSource)tag._ToTaggedObject();
        }

        #endregion

        #region Trns

        //ufsession_.Trns.CreateCsysMappingMatrix
        //ufsession_.Trns.CreateReflectionMatrix
        //ufsession_.Trns.CreateRotationMatrix
        //ufsession_.Trns.CreateScalingMatrix
        //ufsession_.Trns.CreateTranslationMatrix
        //ufsession_.Trns.
        //ufsession_.Trns.TransformObjects
        //ufsession_.Trns.CreateCsysMappingMatrix
        //ufsession_.Trns.CreateReflectionMatrix
        //ufsession_.Trns.CreateRotationMatrix
        //ufsession_.Trns.CreateScalingMatrix
        //ufsession_.Trns.CreateTranslationMatrix
        //ufsession_.Trns.MapPosition
        //ufsession_.Trns.MultiplyMatrices
        //ufsession_.Trns.TransformObjects

        #endregion
    }
}
// 4007