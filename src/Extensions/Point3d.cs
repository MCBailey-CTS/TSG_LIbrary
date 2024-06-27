using System;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using TSG_Library.Geom;
using Curve = TSG_Library.Geom.Curve;
using static NXOpen.UF.UFConstants;
// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
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
            ConeBuilder builder = basePart.Features.CreateConeBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                builder.Type = ConeBuilder.Types.DiametersAndHeight;
                Direction direction2 = basePart.Directions.CreateDirection(axisPoint, direction,
                    SmartObject.UpdateOption.WithinModeling);
                Axis axis = builder.Axis;
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
            Matrix3x3 wcsOrientation = __display_part_.WCS.__Orientation();
            __display_part_.WCS.__Orientation(matrix);
            BlockFeatureBuilder builder = __work_part_.Features.CreateBlockFeatureBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
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
            Matrix3x3 orientation = axisZ.__ToMatrix3x3();
            Vector3d axisX = orientation.__AxisX();
            Vector3d axisY = orientation.__AxisY();
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
            Vector3d vector = p2.__Subtract(p1);
            Vector3d vector2 = p3.__Subtract(p1);
            double num = vector.__Multiply(vector);
            double num2 = vector.__Multiply(vector2);
            double num3 = vector2.__Multiply(vector2);
            double num4 = num * num3 - num2 * num2;
            double num5 = (num * num3 - num2 * num3) / (2.0 * num4);
            double num6 = (num * num3 - num * num2) / (2.0 * num4);
            Vector3d vector3 = vector.__Multiply(num5).__Add(vector2.__Multiply(num6));
            Point3d center = vector3.__Add(p1);
            double radius = vector3.__Norm();
            Matrix3x3 orientation = vector.__Cross(vector2).__ToMatrix3x3();
            Vector3d axisX = orientation.__AxisX();
            Vector3d axisY = orientation.__AxisY();
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
            double[] array = origin.__ToArray();
            ufsession_.Csys.CreateCsys(array, matrix.Tag, out Tag csys_id);
            NXObject objectFromTag = (NXObject)session_.__GetTaggedObject(csys_id);
            CoordinateSystem coordinateSystem = (CoordinateSystem)objectFromTag;
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
            double[] array = origin.__ToArray();
            NXMatrix matrix = __work_part_.NXMatrices.Create(axisX.__ToMatrix3x3(axisY, axisZ));
            Tag nXOpenTag = matrix.Tag;
            ufsession_.Csys.CreateCsys(array, nXOpenTag, out Tag csys_id);
            NXObject objectFromTag = (NXObject)session_.__GetTaggedObject(csys_id);
            CoordinateSystem coordinateSystem = (CoordinateSystem)objectFromTag;
            return coordinateSystem;
        }

        public static Point3d __Mirror(
            this Point3d point,
            Surface.Plane plane)
        {
            Transform transform = Transform.CreateReflection(plane);
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
            double[] matrix = xform.Matrix;
            double x = point.X;
            double y = point.Y;
            double z = point.Z;
            double x2 = x * matrix[0] + y * matrix[1] + z * matrix[2] + matrix[3];
            double y2 = x * matrix[4] + y * matrix[5] + z * matrix[6] + matrix[7];
            double z2 = x * matrix[8] + y * matrix[9] + z * matrix[10] + matrix[11];
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
        public static Point3d __Mirror(
            this Point3d point,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }


        public static Point3d __MirrorMap(this Point3d position, Surface.Plane plane,
            Component fromComponent, Component toComponent)
        {
            __display_part_.WCS.SetOriginAndMatrix(fromComponent.__Origin(), fromComponent.__Orientation());

            Point3d newStart = __MapWcsToAcs(position);

            __display_part_.WCS.SetOriginAndMatrix(toComponent.__Origin(), toComponent.__Orientation());

            return __MapAcsToWcs(newStart);
        }

        public static int __ToHashCode(this Point3d p)
        {
            int hash = 17;

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
            Point3d origin = inputCsys.Origin;
            Vector3d axisX = inputCsys.__Orientation().Element.__AxisX();
            Vector3d axisY = inputCsys.__Orientation().Element.__AxisY();
            Vector3d axisZ = inputCsys.__Orientation().Element.__AxisZ();
            Vector3d _x = axisX.__Multiply(inputCoords.X);
            Vector3d _y = axisY.__Multiply(inputCoords.Y);
            Vector3d _z = axisZ.__Multiply(inputCoords.Z);
            Point3d position = origin.__Add(_x).__Add(_y).__Add(_z);
            Vector3d vector = position.__Subtract(outputCsys.Origin);
            double x = vector.__Multiply(outputCsys.__Orientation().Element.__AxisX());
            double y = vector.__Multiply(outputCsys.__Orientation().Element.__AxisY());
            double z = vector.__Multiply(outputCsys.__Orientation().Element.__AxisZ());
            return new Point3d(x, y, z);
        }

        public static Point3d __Add(this Point3d point, Vector3d vector)
        {
            double x = point.X + vector.X;
            double y = point.Y + vector.Y;
            double z = point.Z + vector.Z;
            return new Point3d(x, y, z);
        }


        public static Vector3d __Subtract(this Point3d start, Point3d end)
        {
            double x = start.X - end.X;
            double y = start.Y - end.Y;
            double z = start.Z - end.Z;
            return new Vector3d(x, y, z);
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __Subtract(this Point3d start, Vector3d end)
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
            Part part = __work_part_;
            DatumAxisBuilder datumAxisBuilder = part.Features.CreateDatumAxisBuilder(null);

            using (session_.__UsingBuilderDestroyer(datumAxisBuilder))
            {
                datumAxisBuilder.Type = DatumAxisBuilder.Types.PointAndDir;
                Direction vector = part.Directions.CreateDirection(origin, direction,
                    SmartObject.UpdateOption.WithinModeling);
                datumAxisBuilder.IsAssociative = true;
                datumAxisBuilder.IsAxisReversed = false;
                datumAxisBuilder.Vector = vector;
                datumAxisBuilder.Point = part.Points.CreatePoint(origin);
                Feature feature = datumAxisBuilder.CommitFeature();
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
            Part part = __work_part_;
            DatumAxisBuilder datumAxisBuilder = part.Features.CreateDatumAxisBuilder(null);

            using (session_.__UsingBuilderDestroyer(datumAxisBuilder))
            {
                datumAxisBuilder.Type = DatumAxisBuilder.Types.TwoPoints;
                datumAxisBuilder.IsAssociative = true;
                datumAxisBuilder.IsAxisReversed = false;
                datumAxisBuilder.Point1 = part.Points.CreatePoint(startPoint);
                datumAxisBuilder.Point2 = part.Points.CreatePoint(endPoint);
                Feature feature = datumAxisBuilder.CommitFeature();
                return (DatumAxisFeature)feature;
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        private static double __ArcPercentage(ICurve curve, Point3d point1, Point3d point2)
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
            Part part = __work_part_;
            DatumCsysBuilder builder = part.Features.CreateDatumCsysBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                Xform xform = part.Xforms.CreateXform(origin, matrix.__AxisX(), matrix.__AxisY(),
                    SmartObject.UpdateOption.WithinModeling, 1.0);
                CartesianCoordinateSystem csys =
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
            return basePart.__CreateDatumCsys(origin, axisX.__ToMatrix3x3(axisY));
        }

        /// <summary>Creates a Snap.NX.DatumPlane feature from a given position and orientation</summary>
        /// <param name="origin">Origin of the datum plane</param>
        /// <param name="orientation">Orientation of the datum plane</param>
        /// <returns> A <see cref="T:Snap.NX.DatumPlane">Snap.NX.DatumPlane</see> object</returns>
        [Obsolete]
        public static DatumPlaneFeature __DatumPlane(Point3d origin, Matrix3x3 orientation)
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
            Part part = __work_part_;
            DatumPlaneBuilder datumPlaneBuilder = part.Features.CreateDatumPlaneBuilder(null);
            Plane plane = datumPlaneBuilder.GetPlane();
            plane.SetMethod(PlaneTypes.MethodType.PointDir);
            Direction direction2 = __work_part_.Directions.CreateDirection(position, direction,
                SmartObject.UpdateOption.WithinModeling);
            plane.SetGeometry(new NXObject[]
            {
                part.Points.CreatePoint(position),
                direction2
            });
            plane.Evaluate();
            DatumPlaneFeature datumPlaneFeature =
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
        /// <param name="basePart"></param>
        /// <param name="center">Center location</param>
        /// <param name="width">Width in the x-direction</param>
        /// <param name="height">Height in the y-direction</param>
        /// <returns>Array of four lines</returns>
        [Obsolete]
        public static Line[] __CreateRectangle(this BasePart basePart, Point3d center,
            double width, double height)
        {
            basePart.__AssertIsWorkPart();
            Vector3d vector = new Vector3d(width / 2.0, 0.0, 0.0);
            Vector3d vector2 = new Vector3d(0.0, height / 2.0, 0.0);
            Point3d position = center.__Add(vector).__Subtract(vector2); // center + vector - vector2;
            Point3d position2 = center.__Add(vector).__Add(vector2);
            Point3d position3 = center.__Subtract(vector).__Add(vector2);
            Point3d position4 = center.__Subtract(vector).__Subtract(vector2);
            return __work_part_.__PolyLine(position, position2, position3, position4, position);
        }

        /// <summary>Creates a rectangle from two diagonal points</summary>
        /// <param name="bottomLeft">The point at the (xmin, ymin) corner</param>
        /// <param name="topRight">The point at the (xmax, ymax) corner</param>
        /// <returns>Array of four lines</returns>
        [Obsolete(nameof(NotImplementedException))]
        public static Line[] __Rectangle(Point3d bottomLeft, Point3d topRight)
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
        public static Line[] __PolyLine(this BasePart basePart, params Point3d[] points)
        {
            int num = points.Length;
            Line[] array = new Line[num - 1];

            for (int i = 0; i < num - 1; i++)
                array[i] = __CreateLine(points[i], points[i + 1]);

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
        public static Line[] __Polygon(params Point3d[] points)
        {
            int num = points.Length;
            Line[] array = new Line[num];
            for (int i = 0; i < num - 1; i++) array[i] = __CreateLine(points[i], points[i + 1]);

            array[num - 1] = __CreateLine(points[num - 1], points[0]);
            return array;
        }


        public static bool __IsEqualTo(this Point3d point3d, Point3d other)
        {
            return point3d.__IsEqualTo(other.__ToArray());
        }

        public static bool __IsEqualTo(this Point3d point3d, double[] array)
        {
            return Math.Abs(point3d.X - array[0]) < .001
                   && Math.Abs(point3d.Y - array[1]) < .001
                   && Math.Abs(point3d.Z - array[2]) < .001;
        }

        public static double[] __ToArray(this Point3d point3d)
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
        public static Point3d __Project(this Point3d point, Surface.Plane plane)
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
        public static Point3d __Project(this Point3d point, Curve.Ray ray)
        {
            //Vector axis = ray.Axis;
            //double num = (this - ray.Origin) * axis;
            //return ray.Origin + num * ray.Axis;
            throw new NotImplementedException();
        }

        public static double[] __Array(this Point3d point)
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
        public static Line __CreateLineTangent(Point3d basePoint, ICurve icurve, Point3d helpPoint)
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
        /// <param name="basePart"></param>
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
        public static Point3d __MapWcsToAcs(this Point3d workCoords)
        {
            int input_csys = UF_CSYS_ROOT_WCS_COORDS;
            int output_csys = UF_CSYS_ROOT_COORDS;
            double[] numArray = new double[3];
            ufsession_.Csys.MapPoint(input_csys, workCoords.__ToArray(), output_csys, numArray);
            return numArray.__ToPoint3d();
        }


        /// <summary>Map a position from Absolute coordinates to Work coordinates</summary>
        /// <param name="absCoords">The coordinates of the given point wrt the Absolute Coordinate System (ACS)</param>
        /// <returns>The coordinates of the given point wrt the Work Coordinate System (WCS)</returns>
        public static Point3d __MapAcsToWcs(this Point3d absCoords)
        {
            int output_csys = UF_CSYS_ROOT_WCS_COORDS;
            int input_csys = UF_CSYS_ROOT_COORDS;
            double[] numArray = new double[3];
            ufsession_.Csys.MapPoint(input_csys, absCoords.__ToArray(), output_csys, numArray);
            return numArray.__ToPoint3d();
        }


        public static double __Distance(this Point3d p, Point3d q)
        {
            return p.__Subtract(q).__Norm();
        }

        public static double __Distance2(this Point3d p, Point3d q)
        {
            return p.__Subtract(q).__Norm2();
        }

        public static Point3d __MidPoint(this Point3d p, Point3d q)
        {
            double x = (p.X + q.X) / 2;
            double y = (p.Y + q.Y) / 2;
            double z = (p.Z + q.Z) / 2;
            return new Point3d(x, y, z);
        }

        public static Point3d __AddX(this Point3d p, double x)
        {
            return new Point3d(p.X + x, p.Y, p.Z);
        }

        public static Point3d __AddY(this Point3d p, double y)
        {
            return new Point3d(p.X, p.Y + y, p.Z);
        }

        public static Point3d __AddZ(this Point3d p, double z)
        {
            return new Point3d(p.X, p.Y, p.Z + z);
        }

        #endregion
    }
}