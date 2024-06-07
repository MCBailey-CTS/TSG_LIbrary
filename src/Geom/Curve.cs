using System;
using NXOpen;
using TSG_Library.Extensions;

namespace TSG_Library.Geom
{
    /// <summary>
    ///     Represents a non-persistent curve -- not stored in NX
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The most common use for Geom.Curve objects is to return
    ///         information about the geometry of edges. Specifically, the
    ///         Geometry property of an NX.Edge returns a Geom.Curve
    ///         object of the appropriate type, which you can then use
    ///         to obtain geometric information. Examples of this
    ///         usage are given in the
    ///         <see cref="T:Snap.Geom.Curve.Line">Geom.Curve.Line</see>,
    ///         <see cref="T:Snap.Geom.Curve.Arc">Geom.Curve.Arc</see>, and
    ///         <see cref="T:Snap.Geom.Curve.Ellipse">Geom.Curve.Ellipse</see> classes.
    ///     </para>
    /// </remarks>
    public class Curve
    {
        internal Curve()
        {
        }

        /// <summary>Represents a non-persistent infinite line -- not stored in NX</summary>
        public class Ray : Curve
        {
            private Vector3d axis;

            /// <summary>Constructs a ray from a given position and vector</summary>
            /// <param name="origin">Point lying on ray</param>
            /// <param name="axis">Vector along ray</param>
            /// <remarks>
            ///     <para>
            ///     </para>
            ///     The input axis vector is not required to have unit length; it is unitized within this function.
            /// </remarks>
            public Ray(Point3d origin, Vector3d axis)
            {
                Origin = origin;
                Axis = axis._Unit();
            }

            /// <summary> A vector along the ray (a unit vector) </summary>
            /// <remarks>
            ///     <para>
            ///         When you set this property, the vector you use does not need to be
            ///         a unit vector; the input vector will be unitized internally.
            ///         On the other hand, when you get the value of this property, you will
            ///         always receive a unit vector. This means that the vector you get
            ///         may not be the same one you used when setting.
            ///     </para>
            /// </remarks>
            public Vector3d Axis
            {
                get => axis;
                set => axis = value._Unit();
            }

            /// <summary> A position on the ray</summary>
            public Point3d Origin { get; set; }
        }

        /// <summary>Represents a non-persistent line -- not stored in NX</summary>
        public class Line : Curve
        {
            /// <summary>Constructs a line from two points</summary>
            /// <param name="startPoint">Position for start of line</param>
            /// <param name="endPoint">Position for end of line</param>
            internal Line(Point3d startPoint, Point3d endPoint)
            {
                StartPoint = startPoint;
                EndPoint = endPoint;
            }

            /// <summary>The start point of the line</summary>
            public Point3d StartPoint { get; set; }

            /// <summary>The end point of the line</summary>
            public Point3d EndPoint { get; set; }
        }

        /// <summary>Represents a non-persistent circular arc -- not stored in NX</summary>
        public class Arc : Curve
        {
            private Vector3d axisX;

            private Vector3d axisY;

            /// <summary>Constructs an arc from center, axes, radius, angles in degrees</summary>
            /// <param name="center">Center point (in absolute coordinates)</param>
            /// <param name="axisX">Unit vector along X-axis (where angle = 0)</param>
            /// <param name="axisY">Unit vector along Y-axis (where angle = 90)</param>
            /// <param name="radius">Radius</param>
            /// <param name="startAngle">Start angle (in degrees)</param>
            /// <param name="endAngle">End angle (in degrees)</param>
            internal Arc(
                Point3d center,
                Vector3d axisX,
                Vector3d axisY,
                double radius,
                double startAngle, double endAngle)
            {
                Center = center;
                AxisX = axisX;
                AxisY = axisY;
                Radius = radius;
                StartAngle = startAngle;
                EndAngle = endAngle;
            }

            /// <summary> The center of the arc (in absolute coordinates) </summary>
            public Point3d Center { get; set; }

            /// <summary> A unit vector along the X-axis of the arc (where angle = 0)</summary>
            public Vector3d AxisX
            {
                get => axisX;
                set => axisX = value._Unit();
            }

            /// <summary> A unit vector along the Y-axis of the arc (where angle = 90)</summary>
            public Vector3d AxisY
            {
                get => axisY;
                set => axisY = value._Unit();
            }

            /// <summary> The radius of the arc</summary>
            public double Radius { get; set; }

            /// <summary> The start angle of the arc (in degrees) </summary>
            public double StartAngle { get; set; }

            /// <summary> The end angle of the arc (in degrees) </summary>
            public double EndAngle { get; set; }

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
            public static Arc Fillet(Point3d p0, Point3d pa, Point3d p1, double radius)
            {
                var vector = p0._Subtract(pa)._Unit();
                var vector2 = p1._Subtract(pa)._Unit();
                var vector3 = vector._Add(vector2)._Unit();
                var num = vector._Angle(vector2) / 2.0;
                var num2 = radius / Math.SinD(num);
                var center = vector3._Multiply(num2)._Add(pa);
                var vector4 = vector3._Negate();
                var vector5 = vector2._Subtract(vector)._Unit();
                var num3 = 90.0 - num;
                var startAngle = 0.0 - num3;
                var endAngle = num3;
                return new Arc(center, vector4, vector5, radius, startAngle, endAngle);
            }
        }

        /// <summary>Represents a non-persistent ellipse -- not stored in NX</summary>
        public class Ellipse : Curve
        {
            /// <summary>Creates an NX.Ellipse from center, axes, radius, angles in degrees</summary>
            /// <param name="center">Center point (in absolute coordinates)</param>
            /// <param name="axisX">Unit vector along X-axis (where angle = 0)</param>
            /// <param name="axisY">Unit vector along Y-axis (where angle = 90)</param>
            /// <param name="majorRadius">The major radius</param>
            /// <param name="minorRadius">The minor radius</param>
            /// <param name="startAngle">Start angle (in degrees)</param>
            /// <param name="endAngle">End angle (in degrees)</param>
            /// <returns>An NX.Ellipse object</returns>
            internal Ellipse(Point3d center, Vector3d axisX, Vector3d axisY, double majorRadius, double minorRadius,
                double startAngle, double endAngle)
            {
                Center = center;
                RadiusX = majorRadius;
                RadiusY = minorRadius;
                AxisX = axisX;
                AxisY = axisY;
                StartAngle = startAngle;
                EndAngle = endAngle;
            }

            /// <summary>The half-width of the ellipse in its X-direction</summary>
            /// <remarks>
            ///     <para>
            ///         This is the "a" parameter in the standard ellipse equation
            ///         <c>(x^2)/(a^2) + (y^2)/(b^2) = 1</c>.
            ///     </para>
            ///     <para>
            ///         It is sometimes known as the "major" radius, but this name is somewhat misleading since
            ///         it is not necessarily true that RadiusX &gt; RadiusY.
            ///     </para>
            /// </remarks>
            public double RadiusX { get; set; }

            /// <summary>The half-width of the ellipse in its Y-direction</summary>
            /// <remarks>
            ///     <para>
            ///         This is the "b" parameter in the standard ellipse equation
            ///         <c>(x^2)/(a^2) + (y^2)/(b^2) = 1</c>.
            ///     </para>
            ///     <para>
            ///         It is sometimes known as the "minor" radius, but this name is somewhat misleading since
            ///         it is not necessarily true that RadiusY &lt; RadiusX.
            ///     </para>
            /// </remarks>
            public double RadiusY { get; set; }

            /// <summary> Center of ellipse (in absolute coordinates)</summary>
            public Point3d Center { get; set; }

            /// <summary>A unit vector along the X-axis of the ellipse (where angle = 0)</summary>
            public Vector3d AxisX { get; set; }

            /// <summary>A unit vector along the Y-axis of the ellipse (where angle = 90)</summary>
            public Vector3d AxisY { get; set; }

            /// <summary>Start angle (in degrees)</summary>
            public double StartAngle { get; set; }

            /// <summary>End angle (in degrees)</summary>
            public double EndAngle { get; set; }
        }

        /// <summary>Represents a non-persistent spline curve -- not stored in NX</summary>
        public class Spline : Curve
        {
            /// <summary>Creates a rational Geom.Curve.Spline from knots, poles, and weights</summary>
            /// <param name="knots">Knots -- an array of n+k knot values : t[0], ... , t[n+k-1]</param>
            /// <param name="poles">An array of n 3D positions representing poles</param>
            /// <param name="weights">An array of n weight values</param>
            internal Spline(double[] knots, Point3d[] poles, double[] weights)
            {
                Poles = poles;
                Weights = weights;
                Knots = knots;
            }

            /// <summary> Array of poles</summary>
            /// <remarks> There will be n poles, with indices 0,1,2,...,n-1 </remarks>
            public Point3d[] Poles { get; set; }

            /// <summary> Array of weight values</summary>
            /// <remarks>
            ///     <para>
            ///     </para>
            ///     The weights must be positive. This is not checked.
            /// </remarks>
            public double[] Weights { get; set; }

            /// <summary> Array of knot values </summary>
            /// <remarks> There will be n+k knots, with indices 0,1,2,...,n+k-1, where k = Order </remarks>
            public double[] Knots { get; set; }

            /// <summary>The degree of the spline, m (equal to order - 1)</summary>
            public int Degree => Order - 1;

            /// <summary>The order of the spline, k (equal to degree + 1)</summary>
            public int Order
            {
                get
                {
                    var num = Poles.Length;
                    return Knots.Length - num;
                }
            }

            /// <summary>Creates a rational Geom.Curve.Spline from knots, poles, and weights</summary>
            /// <param name="knots">Knots -- an array of n+k knot values : t[0], ... , t[n+k-1]</param>
            /// <param name="poles">An array of n 3D positions representing poles</param>
            /// <param name="weights">An array of n weight values</param>
            /// <returns>A Snap.Geom.Curve.Spline object</returns>
            /// <remarks>
            ///     <para>
            ///         To create a Bezier curve (which is a spline having only a single segment), you can use the
            ///         <see cref="O:Snap.Geom.Curve.Spline.CreateBezier">CreateBezier</see> functions.
            ///     </para>
            /// </remarks>
            /// <seealso cref="O:Snap.Create.Spline">Snap.Create.Spline</seealso>
            /// <seealso cref="O:Snap.Create.BezierCurve">Snap.Create.BezierCurve</seealso>
            public static Spline Create(double[] knots, Point3d[] poles, double[] weights)
            {
                return new Spline(knots, poles, weights);
            }

            /// <summary>Creates a polynomial Geom.Curve.Spline from knots and poles</summary>
            /// <param name="knots">Knots -- an array of n+k knot values : t[0], ... , t[n+k-1]</param>
            /// <param name="poles">An array of n 3D positions representing poles</param>
            /// <returns>A Snap.Geom.Curve.Spline object</returns>
            /// <remarks>
            ///     <para>
            ///         To create a Bezier curve (which is a spline having only a single segment), you can use the
            ///         <see cref="O:Snap.Geom.Curve.Spline.CreateBezier">CreateBezier</see> functions.
            ///     </para>
            /// </remarks>
            /// <seealso cref="O:Snap.Create.SplineThroughPoints">SplineThroughPoints</seealso>
            public static Spline Create(double[] knots, Point3d[] poles)
            {
                var weights = UnitWeights(poles.Length);
                return new Spline(knots, poles, weights);
            }

            /// <summary>Creates a rational Bezier curve from given poles and weights</summary>
            /// <param name="poles">Array of m+1 poles (3D)</param>
            /// <param name="weights">Array of m+1 weights</param>
            /// <returns>A Snap.Geom.Curve.Spline object that is a rational Bezier curve of degree m</returns>
            [Obsolete(nameof(NotImplementedException))]
            public static Spline CreateBezier(Point3d[] poles, double[] weights)
            {
                //return new Spline(Math.SplineMath.BezierKnots(poles.Length - 1), poles, weights);
                throw new NotImplementedException();
            }

            /// <summary>Creates a polynomial Bezier curve from given poles</summary>
            /// <param name="poles">Array of m+1 poles (3D)</param>
            /// <returns>A Snap.Geom.Curve.Spline object that is a polynomial Bezier curve of degree m</returns>
            [Obsolete(nameof(NotImplementedException))]
            public static Spline CreateBezier(Point3d[] poles)
            {
                //int num = poles.Length;
                //double[] knots = Math.SplineMath.BezierKnots(num - 1);
                //double[] weights = UnitWeights(num);
                //return new Spline(knots, poles, weights);
                throw new NotImplementedException();
            }

            /// <summary>Calculates a point on the curve at a given parameter value</summary>
            /// <param name="r">The index of the span on which the given parameter value lies</param>
            /// <param name="t">Parameter value</param>
            /// <returns>The point corresponding to the given parameter value</returns>
            /// <remarks>
            ///     <para>
            ///         The span index is the number r such that t[r] ≤ t &lt; t[r+1].
            ///         Legal values are Degree ≤ r ≤ Knots.Length - 1.
            ///     </para>
            ///     <para>
            ///         If you don't know the span index, you can use the other NXOpen.Point3d function,
            ///         which does not require it as input. But, if you're going to do multiple
            ///         evaluations on a known span, this function is faster.
            ///     </para>
            /// </remarks>
            [Obsolete(nameof(NotImplementedException))]
            public Point3d Position(int r, double t)
            {
                //double[] knots = Knots;
                //NXOpen.Point3d[] poles = Poles;
                //double[] weights = Weights;
                //int order = Order;
                //int num = order - 1;
                //double[] array = NXOpen_. Math.EvaluateBasisFunction(knots, order, t, r);
                //NXOpen.Point3d position = new NXOpen.Point3d(0.0, 0.0, 0.0);
                //double num2 = 0.0;
                //for (int num3 = num; num3 >= 0; num3--)
                //{
                //    int num4 = r - num + num3;
                //    double num5 = weights[num4] * array[num3];
                //    position += num5 * poles[num4];
                //    num2 += num5;
                //}

                //return new NXOpen.Point3d(position / num2);
                throw new NotImplementedException();
            }

            /// <summary>Calculates a point on the curve at a given parameter value</summary>
            /// <param name="t">Parameter value</param>
            /// <returns>The point corresponding to the given parameter value</returns>
            /// <remarks>
            ///     <para>
            ///         You can get exactly the same results by using the
            ///         <see cref="M:Snap.NX.Curve.Position(System.Double)">Snap.NX.Curve.Position</see> function.
            ///         However, this function is typically much faster.
            ///     </para>
            /// </remarks>
            /// <seealso cref="M:Snap.NX.Curve.Position(System.Double)">Snap.NX.Curve.Position</seealso>
            [Obsolete(nameof(NotImplementedException))]
            public Point3d Position(double t)
            {
                //int r = Math.SplineMath.FindSpan(Knots, Order, t);
                //return Position(r, t);
                throw new NotImplementedException();
            }

            private static double[] UnitWeights(int n)
            {
                var array = new double[n];
                for (var i = 0; i < n; i++) array[i] = 1.0;

                return array;
            }
        }
    }
}