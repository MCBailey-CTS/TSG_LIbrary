using System;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Geom;
using Curve = NXOpen.Curve;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library
{
    //[ExtensionsAspect]
    public static class Extensions_
    {
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
        public static Point3d[] __Derivatives(this ICurve icurve, double value, int order)
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


        //internal static SelectionIntentRule[] CreateSelectionIntentRule(params Point[] points)
        //{
        //    var array = new Point[points.Length];

        //    for (var i = 0; i < array.Length; i++)
        //        array[i] = points[i];

        //    var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
        //    return new SelectionIntentRule[1] { curveDumbRule };
        //}


        /// <summary>Clips a ray to the model bounding box (the 1 KM cube), giving a line</summary>
        /// <param name="ray">The ray</param>
        /// <returns>Line end-points</returns>
        [Obsolete(nameof(NotImplementedException))]
        internal static Point3d[] __ClipRay(Geom.Curve.Ray ray)
        {
            double num = -1000000.0;
            double num2 = 1000000.0;
            double num3 = 1E-16;
            double num4 = 200.0 * MetersToPartUnits;
            double x = ray.Axis.X;
            double x2 = ray.Origin.X;
            if (System.Math.Abs(x) > num3)
            {
                double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                double val2 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val2, num2);
            }

            x = ray.Axis.Y;
            x2 = ray.Origin.Y;
            if (System.Math.Abs(x) > num3)
            {
                double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                double val3 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val3, num2);
            }

            x = ray.Axis.Z;
            x2 = ray.Origin.Z;
            if (System.Math.Abs(x) > num3)
            {
                double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
                double val4 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
                num = System.Math.Max(val, num);
                num2 = System.Math.Min(val4, num2);
            }

            Point3d position = ray.Origin.__Add(ray.Axis.__Multiply(num));
            Point3d position2 = ray.Origin.__Add(ray.Axis.__Multiply(num2));
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
        public static Arc __Copy(this Arc arc)
        {
            Transform xform = Transform.CreateTranslation(0.0, 0.0, 0.0);
            return arc.__Copy(xform);
        }

        /// <summary>Transforms/copies an NX.Arc</summary>
        /// <param name="xform">Transform to be applied</param>
        /// <returns>A transformed copy of NX.Arc</returns>
        /// <exception cref="T:System.ArgumentException">
        ///     The transform would convert the arc to an ellipse. Please use Curve.Copy
        ///     instead
        /// </exception>
        [Obsolete(nameof(NotImplementedException))]
        public static Arc __Copy(this Arc arc, Transform xform)
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
        public static Arc[] __Divide(this Arc arc, params double[] parameters)
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
        public static Arc[] __Divide(ICurve boundingCurve, Point3d helpPoint)
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
        public static Arc[] __Divide(Face face, Point3d helpPoint)
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
        public static Arc[] __Divide(Surface.Plane geomPlane, Point3d helpPoint)
        {
            //Curve[] curveArray = base.Divide(geomPlane, helpPoint);
            //return ArcArray(curveArray);
            throw new NotImplementedException();
        }

        /// To avoid having four identical copies of the same code
        [Obsolete(nameof(NotImplementedException))]
        private static Arc[] __ArcArray(this Curve[] curveArray)
        {
            Arc[] array = new Arc[curveArray.Length];

            for (int i = 0; i < curveArray.Length; i++)
                array[i] = (Arc)curveArray[i];

            return array;
        }
    }
}
// 4007