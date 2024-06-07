using System;
using NXOpen;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
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
            if(edge.SolidEdgeType != Edge.EdgeType.Linear)
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
            if(edge._StartPoint()._IsEqualTo(pos1) && edge._EndPoint()._IsEqualTo(pos2))
                return true;

            if(edge._StartPoint()._IsEqualTo(pos2) && edge._EndPoint()._IsEqualTo(pos1))
                return true;

            return false;
        }

        public static Curve _ToCurve(this Edge edge)
        {
            ufsession_.Modl.CreateCurveFromEdge(edge.Tag, out var ugcrv_id);
            return (Curve)session_._GetTaggedObject(ugcrv_id);
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
            if(edge.SolidEdgeType == Edge.EdgeType.Elliptical ||
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
    }
}