using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Geom;
using Curve = NXOpen.Curve;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
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
        public static Vector3d __Derivative(this Curve curve, double value)
        {
            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out IntPtr evaluator);
            double[] array = new double[3];
            double[] array2 = new double[3];
            value /= Factor;
            eval.Evaluate(evaluator, 1, value, array, array2);
            eval.Free(evaluator);
            Vector3d vector = array2.__ToVector3d();
            return vector.__Divide(Factor);
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
            Tag nXOpenTag = curve.Tag;
            double[] array = point.__ToArray();
            int direction = 1;
            double offset = 0.0;
            double tolerance = 0.0001;
            double[] point_along_curve = new double[3];
            ufsession_.Modl.AskPointAlongCurve2(array, nXOpenTag, offset, direction, tolerance, point_along_curve,
                out double parameter);
            return (1.0 - parameter) * curve.__MinU() + parameter * curve.__MaxU();
        }

        ////
        //// Summary:
        ////     The lower-limit parameter value (at the start-point of the curve)
        ////
        //// Remarks:
        ////     If c is a curve, then c.Position(c.MinU) = c.StartPoint
        //public static double __MinU(this Curve curve)
        //{
        //    var eval = ufsession_.Eval;
        //    eval.Initialize2(curve.Tag, out var evaluator);
        //    var array = new double[2] { 0.0, 1.0 };
        //    eval.AskLimits(evaluator, array);
        //    eval.Free(evaluator);
        //    return Factor * array[0];
        //}

        ////
        //// Summary:
        ////     The upper-limit parameter value (at the end-point of the curve)
        ////
        //// Remarks:
        ////     If c is a curve, then c.Position(c.MaxU) = c.EndPoint
        //public static double __MaxU(this Curve curve)
        //{
        //    var eval = ufsession_.Eval;
        //    eval.Initialize2(curve.Tag, out var evaluator);
        //    var array = new double[2] { 0.0, 1.0 };
        //    eval.AskLimits(evaluator, array);
        //    eval.Free(evaluator);
        //    return Factor * array[1];
        //}

        public static Curve __Copy(this Curve curve)
        {
            switch (curve)
            {
                case Line line:
                    return line.__Copy();
                case Arc arc:
#pragma warning disable CS0618 // Type or member is obsolete
                    return arc.__Copy();
#pragma warning restore CS0618 // Type or member is obsolete
                case Ellipse ellipse:
                    return ellipse.__Copy();
                case Parabola parabola:
#pragma warning disable CS0612 // Type or member is obsolete
                    return parabola.__Copy();
#pragma warning restore CS0612 // Type or member is obsolete
                case Hyperbola hyperbola:
                    //return hyperbola.__Copy();
                    throw new NotImplementedException();
                case Spline spline:
                    return spline.__Copy();
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
        public static Point3d __Position(this Curve curve, double value)
        {
            //using(session_.using_evaluator(curve.Tag))
            //{

            //}

            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out IntPtr evaluator);
            double[] array = new double[3];
            double[] array3 = new double[3];
            value /= Factor;
            eval.Evaluate(evaluator, 0, value, array, array3);
            eval.Free(evaluator);
            return array.__ToPoint3d();
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
        public static Curve[] __Copy(this Curve curve, params Curve[] original)
        {
            Curve[] array = new Curve[original.Length];
            for (int i = 0; i < original.Length; i++) array[i] = original[i].__Copy();

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
        public static void __CreateTrim(this Curve curve, double lowerParam, double upperParam)
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
        public static Curve[] __Divide(this Curve curve, params double[] parameters)
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
        public static Curve[] __Divide(this Curve curve, ICurve boundingCurve,
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
        public static Curve[] __Divide(this Curve curve, Face face, Point3d helpPoint)
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
        public static Curve[] __Divide(this Curve curve, /* Surface.Plane geomPlane,*/
            Point3d helpPoint)
        {
            //Compute.IntersectionResult intersectionResult = Compute.IntersectInfo(this, geomPlane, helpPoint);
            //return Divide(intersectionResult.CurveParameter);
            throw new NotImplementedException();
        }

        public static bool __IsClosed(this Curve curve)
        {
            int result = ufsession_.Modl.AskCurveClosed(curve.Tag);

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
        public static double __MinU(this Curve curve)
        {
            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out IntPtr evaluator);
            double[] array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return 1.0 * array[0];
        }

        /// <summary>The upper u-value (at the start point of the curve)</summary>
        /// <param name="curve">The curve</param>
        /// <returns>The value at the end of the curve</returns>
        public static double __MaxU(this Curve curve)
        {
            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out IntPtr evaluator);
            double[] array = new double[2] { 0.0, 1.0 };
            eval.AskLimits(evaluator, array);
            eval.Free(evaluator);
            return 1.0 * array[1];
        }

        ///// <summary>Calculates a point on the icurve at a given parameter value</summary>
        ///// <param name="curve">The curve</param>
        ///// <param name="value">The parameter value</param>
        ///// <returns>The <see cref="NXOpen.Point3d" /></returns>
        //public static Point3d _Position(this Curve curve, double value)
        //{
        //    var eval = ufsession_.Eval;
        //    eval.Initialize2(curve.Tag, out var evaluator);
        //    var array = new double[3];
        //    var array2 = array;
        //    var array3 = new double[3];
        //    var derivatives = array3;
        //    value /= 1.0;
        //    eval.Evaluate(evaluator, 0, value, array2, derivatives);
        //    eval.Free(evaluator);
        //    return array2.__ToPoint3d();
        //}

        public static Point3d __StartPoint(this Curve curve)
        {
            return curve.__Position(curve.__MinU());
        }

        public static Point3d __EndPoint(this Curve curve)
        {
            return curve.__Position(curve.__MaxU());
        }

        public static Point3d __MidPoint(this Curve curve)
        {
            double max = curve.__MaxU();
            double min = curve.__MinU();
            double diff = max - min;
            double quotient = diff / 2;
            double total = max - quotient;
            return curve.__Position(total);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static void /* Box3d*/ __Box(this Curve curve)
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

        //public static bool __IsClosed(this Curve curve)
        //{
        //    ufsession_.Modl.AskCurvePeriodicity(curve.Tag, out var status);
        //    switch (status)
        //    {
        //        case 0:
        //            return false;
        //        case 1:
        //            return true;
        //        default:
        //            throw NXException.Create(status);
        //    }
        //}

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
        public static Vector3d __Normal(this Curve curve, double value)
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
        public static Vector3d __Binormal(this Curve curve, double value)
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
        public static double __Curvature(this Curve curve, double value)
        {
            //Vector[] array = Derivatives(value, 2);
            //double num = Vector.Norm(Vector.Cross(array[1], array[2]));
            //double num2 = Vector.Norm(array[1]);
            //double num3 = num2 * num2 * num2;
            //return num / num3;
            throw new NotImplementedException();
        }

        ////
        //// Summary:
        ////     Calculates the parameter value at a point on the curve
        ////
        //// Parameters:
        ////   point:
        ////     The point
        ////
        //// Returns:
        ////     Parameter value at the point (not unitized)
        ////
        //// Remarks:
        ////     The Parameter function and the Position function are designed to work together
        ////     smoothly -- each of these functions is the "reverse" of the other. So, if c is
        ////     any curve and t is any parameter value, then
        ////
        ////     c.Parameter(c.Position(t)) = t
        ////
        ////     Also, if p is any point on the curve c, then
        ////
        ////     c.Position(c.Parameter(p)) = p
        //public static double __Parameter(this Curve curve, Point3d point)
        //{
        //    var nXOpenTag = curve.Tag;
        //    var array = point._ToArray();
        //    var direction = 1;
        //    var offset = 0.0;
        //    var tolerance = 0.0001;
        //    var point_along_curve = new double[3];
        //    ufsession_.Modl.AskPointAlongCurve2(array, nXOpenTag, offset, direction, tolerance, point_along_curve,
        //        out var parameter);
        //    return (1.0 - parameter) * curve.__MinU() + parameter * curve.__MaxU();
        //}

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
        public static double __Parameter(this Curve curve, double baseParameter, double arclength)
        {
            int direction = 1;

            if (arclength < 0.0)
                direction = -1;

            double[] array = curve.__Position(baseParameter).__ToArray();
            double tolerance = 0.0001;
            double[] point_along_curve = new double[3];
            UFSession uFSession = ufsession_;

            uFSession.Modl.AskPointAlongCurve2(
                array,
                curve.Tag,
                Math.Abs(arclength),
                direction,
                tolerance,
                point_along_curve,
                out double parameter);

            return parameter * (curve.__MaxU() - curve.__MinU()) + curve.__MinU();
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
        public static double __Parameter(this Curve curve, double arclengthFraction)
        {
            double arclength = curve.GetLength() * arclengthFraction;
            return curve.__Parameter(curve.__MinU(), arclength);
        }

        ////
        //// Summary:
        ////     Copies an NX.Curve (with a null transform)
        ////
        //// Returns:
        ////     A copy of the input curve
        ////
        //// Remarks:
        ////     The new curve will be on the same layer as the original one.
        //[Obsolete(nameof(NotImplementedException))]
        //public static Curve __Copy(this Curve curve)
        //{
        //    //Transform xform = Transform.CreateTranslation(0.0, 0.0, 0.0);
        //    //return Copy(xform);
        //    throw new NotImplementedException();
        //}


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
        public static Point3d[] __PositionArray(this Curve curve, double[] values)
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
        public static Vector3d __Tangent(this Curve curve, double value)
        {
            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.Tag, out IntPtr evaluator);
            double[] array = new double[3];
            double[] array2 = new double[3];
            double[] array4 = new double[3];
            double[] array5 = new double[3];
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, array, array2, array4, array5);
            eval.Free(evaluator);
            return array2.__ToVector3d();
        }


        [Obsolete(nameof(NotImplementedException))]
        public static Curve __Mirror(
            this Curve original,
            Surface.Plane plane)
        {
            //if(original is NXOpen.Line line)
            //    return line._Mi
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Curve __Mirror(
            this Curve bodyDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}