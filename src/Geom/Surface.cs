using System;
using NXOpen;
using static TSG_Library.Extensions;

namespace TSG_Library.Geom
{
    public class Surface
    {
        /// <summary>Represents a non-persistent B-surface object -- not stored in NX</summary>
        /// <remarks>
        ///     <para>
        ///         A Snap.Geom.Surface.Bsurface is very similar to a Snap.NX.Bsurface, of course.
        ///         The big difference is that the latter is an object in an NX part file, whereas
        ///         the former is a non-persistent object that only exists in memory while your
        ///         program is running. Geom objects are completely independent of the NX database,
        ///         and can even be used outside of NX sessions. For this reason, computations on Geom
        ///         objects are often much faster than similar computations on NX objects.
        ///     </para>
        ///     <para>
        ///         The representation used here consists of
        ///         <list type="bullet">
        ///             <item>Poles    -- A 2D array of nu x nv 3D poles (control vertices)</item>
        ///             <item>KnotsU   -- An array of nu+ku knot values : u[0], ... , u[nu+ku-1]</item>
        ///             <item>KnotsV   -- An array of nv+kv knot values : v[0], ... , u[nv+kv-1]</item>
        ///             <item>Weights  -- A 2D array of nu x nv weight values</item>
        ///         </list>
        ///     </para>
        ///     <para>
        ///         The degrees of the surface in the u and v directions are determined by the sizes of the given arrays:
        ///         <list type="bullet">
        ///             <item>
        ///                 <c>DegreeU  = knotsU.Length - Poles.GetLength(0) - 1</c>
        ///             </item>
        ///             <item>
        ///                 <c>DegreeV  = knotsV.Length - Poles.GetLength(1) - 1</c>
        ///             </item>
        ///         </list>
        ///     </para>
        ///     <para>
        ///         Poles p[0,0], p[0,1], ... ,p[0,nv-1] lie along edge u = 0  (0 &lt; v &lt; 1)
        ///     </para>
        ///     <para>
        ///         Poles p[0,0], p[1,0], ... ,p[nu-1,0] lie along edge v = 0  (0 &lt; u &lt; 1)
        ///     </para>
        ///     So, in particular
        ///     <list type="bullet">
        ///         <item>p[0,0]   = S(0,0)</item>
        ///         <item>p[0,nv-1]  = S(0,1)</item>
        ///         <item>p[nu-1,0]  = S(1,0)</item>
        ///         <item>p[nu-1,nv-1] = S(1,1)</item>
        ///     </list>
        /// </remarks>
        public class Bsurface : Surface
        {
            /// <summary>Constructs a polynomial Geom.Bsurface from poles and knot sequences</summary>
            /// <param name="poles">A 2D array of nu x nv 3D positions representing poles</param>
            /// <param name="knotsU">Knots for u-direction -- an array of nu+ku knot values : u[0], ... , u[nu+ku-1]</param>
            /// <param name="knotsV">Knots for v-direction -- an array of nv+kv knot values : v[0], ... , v[nv+kv-1]</param>
            /// <remarks>
            ///     <para>
            ///         The degrees of the surface in the u and v directions are determined by the sizes of the given arrays:
            ///         DegreeU  = knotsU.Length - Poles.GetLength(0) - 1
            ///         DegreeV  = knotsV.Length - Poles.GetLength(1) - 1
            ///     </para>
            ///     <para>
            ///         Poles p[0,0], p[0,1], ... ,p[0,mv] lie along edge u = 0  (0 &lt; v &lt; 1)
            ///         Poles p[0,0], p[1,0], ... ,p[mu,0] lie along edge v = 0  (0 &lt; u &lt; 1)
            ///     </para>
            /// </remarks>
            internal Bsurface(Point3d[,] poles, double[] knotsU, double[] knotsV)
            {
                Poles = poles;
                KnotsU = knotsU;
                KnotsV = knotsV;
                var length = poles.GetLength(0);
                var length2 = poles.GetLength(1);
                Weights = UnitWeights(length, length2);
            }

            /// <summary>Constructs a rational Geom.Bsurface from poles, weights, and knot sequences</summary>
            /// <param name="poles">A 2D array of nu x nv 3D vectors representing poles</param>
            /// <param name="weights">A 2D array of nu x nv values representing weights</param>
            /// <param name="knotsU">Knots for u-direction -- an array of nu+ku knot values : u[0], ... , u[nu+ku-1]</param>
            /// <param name="knotsV">Knots for v-direction -- an array of nv+kv knot values : v[0], ... , v[nv+kv-1]</param>
            /// <remarks>
            ///     <para>
            ///         The order ku in the u-direction can be inferred from the sizes of the given arrays:
            ///         nu  = number of poles in u-direction = Poles.GetLength(0)
            ///         npku = nu + ku = number of u-knots = knotsU.Length
            ///         Then ku = npku - ku. Also, as usual, degree is given by mu = ku - 1. Similarly for the v-direction.
            ///         Poles p[0,0], p[0,1], ... ,p[0,mv] lie along edge u = 0  (0 &lt; v &lt; 1)
            ///         Poles p[0,0], p[1,0], ... ,p[mu,0] lie along edge v = 0  (0 &lt; u &lt; 1)
            ///     </para>
            /// </remarks>
            internal Bsurface(Point3d[,] poles, double[,] weights, double[] knotsU, double[] knotsV)
            {
                Poles = poles;
                Weights = weights;
                KnotsU = knotsU;
                KnotsV = knotsV;
            }

            /// <summary> Array of poles (3D) </summary>
            public Point3d[,] Poles { get; set; }

            /// <summary> Array of weight values</summary>
            /// <remarks>
            ///     <para>
            ///     </para>
            ///     The weights must be positive. This is not checked.
            /// </remarks>
            public double[,] Weights { get; set; }

            /// <summary> Array of knot values in u-direction </summary>
            /// <remarks> There will be nu + ku knots, with indices 0,1,2,...,nu + ku - 1, where ku = Order </remarks>
            public double[] KnotsU { get; set; }

            /// <summary> Array of knot values in v-direction </summary>
            /// <remarks> There will be nv + kv knots, with indices 0,1,2,...,nv + kv - 1, where kv = Order </remarks>
            public double[] KnotsV { get; set; }

            /// <summary>The order of the b-surface in the u-direction</summary>
            public int OrderU => KnotsU.Length - Poles.GetLength(0);

            /// <summary>The order of the b-surface in the v-direction</summary>
            public int OrderV => KnotsV.Length - Poles.GetLength(1);

            /// <summary>The degree of the b-surface in the u-direction</summary>
            public int DegreeU => OrderU - 1;

            /// <summary>The degree of the b-surface in the v-direction</summary>
            public int DegreeV => OrderV - 1;

            /// <summary>Constructs a polynomial Geom.Bsurface from poles and knot sequences</summary>
            /// <param name="poles">A 2D array of nu x nv 3D positions representing poles</param>
            /// <param name="knotsU">Knots for u-direction -- an array of nu+ku knot values : u[0], ... , u[nu+ku-1]</param>
            /// <param name="knotsV">Knots for v-direction -- an array of nv+kv knot values : v[0], ... , v[nv+kv-1]</param>
            /// <returns>A polynomial Geom.Bsurface</returns>
            public static Bsurface Create(Point3d[,] poles, double[] knotsU, double[] knotsV)
            {
                return new Bsurface(poles, knotsU, knotsV);
            }

            /// <summary>Constructs a rational Geom.Bsurface from poles, weights, and knot sequences</summary>
            /// <param name="poles">A 2D array of nu x nv 3D vectors representing poles</param>
            /// <param name="weights">A 2D array of nu x nv values representing weights</param>
            /// <param name="knotsU">Knots for u-direction -- an array of nu+ku knot values : u[0], ... , u[nu+ku-1]</param>
            /// <param name="knotsV">Knots for v-direction -- an array of nv+kv knot values : v[0], ... , v[nv+kv-1]</param>
            /// <returns>A rational Geom.Bsurface</returns>
            public static Bsurface Create(Point3d[,] poles, double[,] weights, double[] knotsU, double[] knotsV)
            {
                return new Bsurface(poles, weights, knotsU, knotsV);
            }

            /// <summary>Creates a Geom.Bsurface that is a rational Bezier patch</summary>
            /// <param name="poles">A 2D array of (mu+1) x (mv+1) 3D positions representing poles</param>
            /// <param name="weights">A 2D array of (mu+1) x (mv+1) values representing weights</param>
            /// <returns>A rational Bezier patch of degree mu x mv</returns>
            /// <seealso cref="M:Snap.Create.BezierPatch(Snap.Position[0:,0:],System.Double[0:,0:])">Snap.Create.BezierPatch</seealso>
            [Obsolete(nameof(NotImplementedException))]
            public static Bsurface CreateBezier(Point3d[,] poles, double[,] weights)
            {
                //int m = poles.GetLength(0) - 1;
                //int m2 = poles.GetLength(1) - 1;
                //double[] knotsU = Math.SplineMath.BezierKnots(m);
                //double[] knotsV = Math.SplineMath.BezierKnots(m2);
                //return new Bsurface(poles, weights, knotsU, knotsV);
                throw new NotImplementedException();
            }

            /// <summary>Creates a Geom.Bsurface that is polynomial Bezier patch</summary>
            /// <param name="poles">A 2D array of (mu+1) x (mv+1) 3D positions representing poles</param>
            /// <returns>A polynomial Bezier patch of degree mu x mv</returns>
            /// <seealso cref="M:Snap.Create.BezierPatch(Snap.Position[0:,0:])">Snap.Create.BezierPatch</seealso>
            [Obsolete(nameof(NotImplementedException))]
            public static Bsurface CreateBezier(Point3d[,] poles)
            {
                //int m = poles.GetLength(0) - 1;
                //int m2 = poles.GetLength(1) - 1;
                //double[] knotsU = Math.SplineMath.BezierKnots(m);
                //double[] knotsV = Math.SplineMath.BezierKnots(m2);
                //return new Bsurface(poles, knotsU, knotsV);
                throw new NotImplementedException();
            }

            /// <summary>Evaluates a point on a Snap.Geom.Surface.Bsurface at given (u,v) parameter values</summary>
            /// <param name="spanU">Span index in u-direction</param>
            /// <param name="spanV">Span index in v-direction</param>
            /// <param name="uv">The (u,v) parameter values</param>
            /// <returns>Point on surface at the given (u,v) parameter values</returns>
            /// <exception cref="T:System.ArgumentException">
            ///     The (u,v) parameter array has the wrong length. You must input
            ///     two separate values, or an array of length 2.
            /// </exception>
            /// <seealso cref="T:Snap.NX.Face">Snap.NX.Face</seealso>
            /// <seealso cref="M:Snap.NX.Face.Parameters(Snap.Position)">Snap.NX.Face.Parameters</seealso>
            [Obsolete(nameof(NotImplementedException))]
            internal Point3d Position(int spanU, int spanV, params double[] uv)
            {
                //if (uv.Length != 2)
                //{
                //    throw new ArgumentException("The uv array must have length = 2");
                //}

                //int orderU = OrderU;
                //int num = orderU - 1;
                //int orderV = OrderV;
                //int num2 = orderV - 1;
                //double tau = uv[0];
                //double tau2 = uv[1];
                //double[] array = Math.SplineMath.EvaluateBasisFunctions(KnotsU, orderU, tau, spanU);
                //double[] array2 = Math.SplineMath.EvaluateBasisFunctions(KnotsV, orderV, tau2, spanV);
                //Position[] array3 = new Position[num2 + 1];
                //double[] array4 = new double[num2 + 1];
                //for (int i = 0; i <= num2; i++)
                //{
                //    array3[i] = new Position(0.0, 0.0, 0.0);
                //    array4[i] = 0.0;
                //    for (int j = 0; j <= num; j++)
                //    {
                //        double num3 = Weights[spanU - num + j, spanV - num2 + i];
                //        array3[i] += array[j] * num3 * Poles[spanU - num + j, spanV - num2 + i];
                //        array4[i] += array[j] * num3;
                //    }
                //}

                //Position position = new Position(0.0, 0.0, 0.0);
                //double num4 = 0.0;
                //for (int i = 0; i <= num2; i++)
                //{
                //    position += array2[i] * array3[i];
                //    num4 += array2[i] * array4[i];
                //}

                //return new Position(position.X / num4, position.Y / num4, position.Z / num4);
                throw new NotImplementedException();
            }

            /// <summary>Evaluates a point on a Snap.Geom.Surface.Bsurface at given (u,v) parameter values</summary>
            /// <param name="uv">The (u,v) parameter values</param>
            /// <returns>Point on surface at the given (u,v) parameter values</returns>
            /// <remarks>
            ///     <para>
            ///         You can get exactly the same results by using the
            ///         <see cref="M:Snap.NX.Face.Position(System.Double[])">Snap.NX.Face.Position</see>.
            ///         However, this function is much faster (often by a factor of 20x or so).
            ///     </para>
            /// </remarks>
            /// <exception cref="T:System.ArgumentException">
            ///     The (u,v) parameter array has the wrong length. You must input
            ///     two separate values, or an array of length 2.
            /// </exception>
            /// <seealso cref="M:Snap.NX.Face.Position(System.Double[])">Snap.NX.Face.Position</seealso>
            [Obsolete(nameof(NotImplementedException))]
            public Point3d Position(params double[] uv)
            {
                //if (uv.Length != 2)
                //{
                //    throw new ArgumentException("The uv array must have length = 2");
                //}

                //double tau = uv[0];
                //double tau2 = uv[1];
                //int spanU = Math.SplineMath.FindSpan(KnotsU, OrderU, tau);
                //int spanV = Math.SplineMath.FindSpan(KnotsV, OrderV, tau2);
                //return Position(spanU, spanV, uv);
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Constructs weights array full of 1's (for polynomial surfaces)
            /// </summary>
            /// <param name="m">First dimension</param>
            /// <param name="n">Second dimension</param>
            /// <returns>Array of weights all equal to 1</returns>
            private static double[,] UnitWeights(int m, int n)
            {
                var array = new double[m, n];
                for (var i = 0; i < m; i++)
                {
                    var num = 0;
                    while (i < n)
                    {
                        array[i, num] = 1.0;
                        n++;
                    }
                }

                return array;
            }
        }

        /// <summary>Represents a non-persistent blending surface -- not stored in NX</summary>
        public class Blend : Surface
        {
            /// <summary>Constructs a blending surface</summary>
            /// <param name="radius">The radius of blending surface</param>
            internal Blend(double radius)
            {
                Radius = radius;
            }

            /// <summary>Radius of the blending surface</summary>
            public double Radius { get; set; }
        }

        /// <summary>Represents a non-persistent conical surface -- not stored in NX</summary>
        public class Cone : Surface
        {
            private Vector3d axisVector;

            /// <summary>Constructs a conical surface</summary>
            /// <param name="axisPoint">The cone base position of base arc</param>
            /// <param name="axisVector">The cone axis vector from base to top</param>
            /// <param name="radius">The radius at axis point</param>
            /// <param name="halfAngle">
            ///     The cone half-angle in degree which is half of the coning angle. The cone half-angle cannot
            ///     greater than Atan(baseDiameter / (2.0 * height))
            /// </param>
            internal Cone(Point3d axisPoint, Vector3d axisVector, double radius, double halfAngle)
            {
                AxisVector = axisVector;
                AxisPoint = axisPoint;
                Radius = radius;
                HalfAngle = halfAngle;
            }

            /// <summary>Axis vector of the conical surface</summary>
            public Vector3d AxisVector
            {
                get => axisVector;
                set => axisVector = value.__Unit();
            }

            /// <summary>Axis point of the conical surface</summary>
            public Point3d AxisPoint { get; set; }

            /// <summary>Radius at axis point</summary>
            public double Radius { get; set; }

            /// <summary>Half-angle, in degrees</summary>
            public double HalfAngle { get; set; }
        }

        /// <summary>Represents a non-persistent cylindrical surface -- not stored in NX</summary>
        public class Cylinder : Surface
        {
            private Vector3d axisVector;

            /// <summary>Constructs a cylindrical surface</summary>
            /// <param name="axisPoint">The point at base of cylinder</param>
            /// <param name="axisVector">The cylinder axis vector (length doesn't matter)</param>
            /// <param name="diameter">The cylinder diameter</param>
            internal Cylinder(Point3d axisPoint, Vector3d axisVector, double diameter)
            {
                AxisVector = axisVector;
                AxisPoint = axisPoint;
                Diameter = diameter;
            }

            /// <summary>Axis vector of the cylindrical surface</summary>
            public Vector3d AxisVector
            {
                get => axisVector;
                set => axisVector = value.__Unit();
            }

            /// <summary>Axis point of the cylindrical surface</summary>
            public Point3d AxisPoint { get; set; }

            /// <summary>Diameter of the cylindrical surface</summary>
            public double Diameter { get; set; }
        }

        /// <summary>Represents a non-persistent extruded surface -- not stored in NX</summary>
        public class Extrude : Surface
        {
            private Vector3d direction;

            /// <summary>Constructs an extruded surface through given vector</summary>
            /// <param name="direction">Extruded direction</param>
            internal Extrude(Vector3d direction)
            {
                Direction = direction;
            }

            /// <summary>Axis vector of the extruded surface</summary>
            public Vector3d Direction
            {
                get => direction;
                set => direction = value.__Unit();
            }
        }

        /// <summary>Represents a non-persistent offset surface -- not stored in NX</summary>
        public class Offset : Surface
        {
            /// <summary>Constructs an offset surface</summary>
            /// <param name="distance">Offset distance</param>
            internal Offset(double distance)
            {
                Distance = distance;
            }

            /// <summary>Offset Distance</summary>
            public double Distance { get; set; }
        }

        /// <summary>Represents a non-persistent infinite plane -- not stored in NX</summary>
        /// <remarks>
        ///     <para>
        ///         The plane equation is X*Normal = D.
        ///     </para>
        ///     <para>
        ///         The point D*Normal lies on the plane.
        ///     </para>
        /// </remarks>
        public class Plane : Surface
        {
            private Vector3d normal;

            /// <summary>Constructs a plane through a given point and normal</summary>
            /// <param name="point">Point lying on plane</param>
            /// <param name="normal">Normal vector</param>
            /// <remarks>
            ///     <para>
            ///     </para>
            ///     The normal vector does not need to have unit length.
            /// </remarks>
            public Plane(Point3d point, Vector3d normal)
            {
                Normal = normal;
                D = point.__Subtract(_Point3dOrigin).__Multiply(Normal);
            }

            /// <summary>Constructs a plane from algebraic coefficients (a*x + b*y + c*z = d)</summary>
            /// <param name="a">Coefficient of x</param>
            /// <param name="b">Coefficient of y</param>
            /// <param name="c">Coefficient of z</param>
            /// <param name="d">Constant term (on right-hand side)</param>
            /// <remarks>The plane has equation  ax + by + cz = d</remarks>
            public Plane(double a, double b, double c, double d)
            {
                var vector = new Vector3d(a, b, c);
                var num = vector.__Norm();
                Normal = vector.__Divide(num);
                D = d / num;
            }

            /// <summary>Constructs a plane through three points</summary>
            /// <param name="p0">First point</param>
            /// <param name="p1">Second point</param>
            /// <param name="p2">Third point</param>
            [Obsolete(nameof(NotImplementedException))]
            public Plane(Point3d p0, Point3d p1, Point3d p2)
            {
                //Normal = Vector.UnitCross(p1 - p0, p2 - p0);
                //D = (p0 - Position.Origin) * Normal;
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Constructs a plane given normal and distance
            /// </summary>
            /// <param name="normal">Normal vector of plane</param>
            /// <param name="distance">Signed distance from origin to plane, along the normal vector</param>
            /// <remarks>
            ///     <para>
            ///         The plane equation is X*Normal = distance.
            ///     </para>
            ///     <para>The point distance*Normal lies on the plane.</para>
            ///     <para>
            ///         The input vector is not required to have unit length; it will
            ///         be unitized inside this function.
            ///     </para>
            /// </remarks>
            public Plane(Vector3d normal, double distance)
            {
                Normal = normal;
                D = distance;
            }

            /// <summary> Normal vector (a unit vector) </summary>
            /// <remarks>
            ///     <para>
            ///         When you set this property, the vector you use does not need to be
            ///         a unit vector; the input vector will be unitized internally.
            ///         On the other hand, when you get the value of this property, you will
            ///         always receive a unit vector. This means that the vector you get
            ///         may not be the same one you used when setting.
            ///     </para>
            /// </remarks>
            public Vector3d Normal
            {
                get => normal;
                set => normal = value.__Unit();
            }

            /// <summary> Signed distance from origin to plane, measured along the plane normal</summary>
            public double D { get; set; }

            /// <summary>Origin of plane : the point at D*Normal</summary>
            /// <remarks>
            ///     <para>
            ///     </para>
            ///     This is the point on the plane that is closest to the origin.
            /// </remarks>
            public Point3d Origin => Normal.__Multiply(D).__Add(_Point3dOrigin);
        }

        /// <summary>Represents a non-persistent revolved surface -- not stored in NX</summary>
        public class Revolve : Surface
        {
            private Vector3d axisVector;

            /// <summary>Constructs a revolved surface</summary>
            /// <param name="axisPoint">Point on the axis of revolution</param>
            /// <param name="axisVector">Vector along the axis of revolution (magnitude doesn't matter)</param>
            internal Revolve(Vector3d axisVector, Point3d axisPoint)
            {
                AxisVector = axisVector;
                AxisPoint = axisPoint;
            }

            /// <summary>Axis vector of the revolved surface </summary>
            public Vector3d AxisVector
            {
                get => axisVector;
                set => axisVector = value.__Unit();
            }

            /// <summary>Axis point of the revolved surface</summary>
            public Point3d AxisPoint { get; set; }
        }

        /// <summary>Represents a non-persistent spherical surface -- not stored in NX</summary>
        public class Sphere : Surface
        {
            /// <summary>Constructs a spherical surface through a point and the radius</summary>
            /// <param name="center">Center of spherical surface</param>
            /// <param name="diameter">Diameter of spherical surface</param>
            internal Sphere(Point3d center, double diameter)
            {
                Center = center;
                Diameter = diameter;
            }

            /// <summary>Diameter of the spherical surface</summary>
            public double Diameter { get; set; }

            /// <summary>Center of the spherical surface</summary>
            public Point3d Center { get; set; }
        }

        /// <summary>Represents a non-persistent toroidal surface -- not stored in NX</summary>
        public class Torus : Surface
        {
            private Vector3d axisVector;

            /// <summary>Constructs a toroidal surface through a vector, a point , major radius and minor radius</summary>
            /// <param name="axisPoint">Axis point</param>
            /// <param name="axisVector">Vector along axis of revolution (length doesn't matter)</param>
            /// <param name="majorRadius">Major radius (radius of "spine" circle)</param>
            /// <param name="minorRadius">Minor radius (radius of "section" circle)</param>
            internal Torus(Vector3d axisVector, Point3d axisPoint, double majorRadius, double minorRadius)
            {
                AxisVector = axisVector;
                AxisPoint = axisPoint;
                MajorRadius = majorRadius;
                MinorRadius = minorRadius;
            }

            /// <summary>Axis Vector of the toroidal surface</summary>
            public Vector3d AxisVector
            {
                get => axisVector;
                set => axisVector = value.__Unit();
            }

            /// <summary>Axis point of the toroidal surface</summary>
            public Point3d AxisPoint { get; set; }

            /// <summary>Major radius of the toroidal surface</summary>
            public double MajorRadius { get; set; }

            /// <summary>Minor radius of the toroidal surface</summary>
            public double MinorRadius { get; set; }
        }
    }
}