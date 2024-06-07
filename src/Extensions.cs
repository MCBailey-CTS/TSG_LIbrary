using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using TSG_Library.Geom;
using static TSG_Library.Extensions.Extensions_;
using Curve = NXOpen.Curve;
using Type = NXOpen.GeometricUtilities.Type;

namespace TSG_Library
{
    public static class Extensions6
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
        public static Point3d[] Derivatives(double value, int order)
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
        public static Curve Copy(double[] xform)
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


        internal static EqualityComparer<Point3d> Point3d_EqualityComparer(double tolerance = .001)
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

        internal static SelectionIntentRule[] CreateSelectionIntentRule(params Point[] points)
        {
            var array = new Point[points.Length];

            for (var i = 0; i < array.Length; i++)
                array[i] = points[i];

            var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
            return new SelectionIntentRule[1] { curveDumbRule };
        }

        /// <summary>Creates an extrude feature</summary>
        /// <param name="section">The "section" to be extruded</param>
        /// <param name="axis">Extrusion direction (vector magnitude not significant)</param>
        /// <param name="extents">Extents of the extrusion (measured from input curves)</param>
        /// <param name="draftAngle">Draft angle, in degrees (positive angle gives larger sections in direction of<br />axis)</param>
        /// <param name="offset">If true, means that offset values are being provided</param>
        /// <param name="offsetValues">Offset distances for section curves</param>
        /// <param name="createSheet">If true, forces creation of a sheet body</param>
        /// <returns>An NX.Extrude object</returns>
        internal static Extrude CreateExtrude(
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
        internal static Point3d[] ClipRay(Geom.Curve.Ray ray)
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
        public static bool IsReverseDirection(OffsetCurveBuilder builder, ICurve[] icurves, Point3d pos,
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
    }
}
// 4007