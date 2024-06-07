using System;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Geom;
using static TSG_Library.Extensions;
using Curve = TSG_Library.Geom.Curve;

namespace TSG_Library
{
    internal class Compute
    {
        //
        // Summary:
        //     Computes the closest points on two given objects (curves, edges, faces, or bodies)
        //
        //
        // Parameters:
        //   nxObject1:
        //     The first object (curve, edge, face, or body)
        //
        //   nxObject2:
        //     The second object (curve, edge, face, or body)
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     One of the input objects is not a curve, edge, face, or body
        //
        // Remarks:
        //     The closest points on the two objects are returned in DistanceResult.Point1 and
        //     DistanceResult.Point2. If you only want to know the distance between the objects
        //     (and not the closest points), then the Snap.Compute.Distance function may be
        //     more convenient.
        public static DistanceResult ClosestPoints(NXObject nxObject1, NXObject nxObject2)
        {
            //if (IsWrongType(nxObject1))
            //{
            //    throw new ArgumentException("The first input object is not a curve, edge, face, or body.");
            //}

            //if (IsWrongType(nxObject2))
            //{
            //    throw new ArgumentException("The second input object is not a curve, edge, face, or body.");
            //}

            //int opt_level = 2;
            //double[] array = new double[3];
            //double[] array2 = new double[3];
            //ufsession_.Modl.AskMinimumDist3(opt_level, nxObject1.Tag, nxObject2.Tag, 0, array, 0, array2, out var _, array, array2, out var _);
            //NXOpen.Point3d position = new NXOpen.Point3d(array);
            //NXOpen.Point3d position2 = new NXOpen.Point3d(array2);
            //return new DistanceResult(position, position2, position. Position.Distance(position, position2));
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the point of an object (curve, edge, face, or body) closest to a given
        //     position
        //
        // Parameters:
        //   point:
        //     Reference point
        //
        //   nxObject:
        //     The object (curve, edge, face, or body)
        //
        // Returns:
        //     A Compute.DistanceResult object which contains the distance and the closest points
        //
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The input object is not a curve, edge, face, or body
        //
        // Remarks:
        //     The closest point of the object is returned in DistanceResult.Point2.
        //
        //     If you only want to know the distance between the objects (and not the closest
        //     points), then the Snap.Compute.Distance function may be more convenient.
        public static DistanceResult ClosestPoints(Point3d point, NXObject nxObject)
        {
            //if (IsWrongType(nxObject))
            //{
            //    throw new ArgumentException("The input object is not a curve, edge, face, or body.");
            //}

            //int opt_level = 2;
            //double[] pt_on_obj = new double[3];
            //double[] array = new double[3];
            //Tag @object = Tag.Null;
            //ufsession_.Modl.AskMinimumDist3(opt_level, @object, nxObject.Tag, 0, point.Array, 0, point.Array, out var _, pt_on_obj, array, out var _);
            //Position position = new Position(array);
            //return new DistanceResult(point, position, Position.Distance(point, position));
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the closest points on an object (curve, edge, face, or body) and a ray
        //
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        //   nxObject:
        //     The object (curve, edge, face, or body)
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The input object is not a curve, edge, face, or body
        //
        // Remarks:
        //     The closest point of the ray is returned in DistanceResult.Point1, and the closest
        //     point of the object is returned in DistanceResult.Point2.
        //
        //     If you only want to know the distance between the objects (and not the closest
        //     points), then the Snap.Compute.Distance function may be more convenient.
        public static DistanceResult ClosestPoints(Curve.Ray ray, NXObject nxObject)
        {
            //if (IsWrongType(nxObject))
            //{
            //    throw new ArgumentException("The input object is not a curve, edge, face, or body.");
            //}

            //Position[] array = ClipRay(ray);
            //NXOpen.Line line = Create.Line(array[0], array[1]);
            //DistanceResult result = ClosestPoints(line, nxObject);
            //NXOpen.NXObject.Delete(line);
            //return result;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the closest points on an object (curve, edge, face, or body) and a plane
        //
        //
        // Parameters:
        //   plane:
        //     The plane
        //
        //   nxObject:
        //     The object (curve, edge, face, or body)
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The input object is not a curve, edge, face, or body
        //
        // Remarks:
        //     The closest point of the plane is returned in DistanceResult.Point1, and the
        //     closest point of the object is returned in DistanceResult.Point2.
        //
        //     If you only want to know the distance between the objects (and not the closest
        //     points), then the Snap.Compute.Distance function may be more convenient.
        public static DistanceResult ClosestPoints(Surface.Plane plane, NXObject nxObject)
        {
            //if (IsWrongType(nxObject))
            //{
            //    throw new ArgumentException("The input object is not a curve, edge, face, or body.");
            //}

            //NXOpen.Point3d[] points = CreateShadow(plane, nxObject);
            //Session.UndoMarkId markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpDistanceMark_999");
            //DistanceResult result = ClosestPoints((Snap.NX.Body)Create.BoundedPlane(Create.Polygon(points)), nxObject);
            //session_.UndoToMark(markId, "TmpDistanceMark_999");
            //return result;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the point on a ray closest to a given point
        //
        // Parameters:
        //   point:
        //     Reference point
        //
        //   ray:
        //     Ray
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Remarks:
        //     The closest point on the ray is returned in DistanceResult.Point2.
        //
        //     If you only want to know the distance between the objects (and not the closest
        //     points), then the Snap.Compute.Distance function may be more convenient.
        public static DistanceResult ClosestPoints(Point3d point, Curve.Ray ray)
        {
            //double num = (point - ray.Origin) * ray.Axis;
            //NXOpen.Point3d position = ray.Origin + num * ray.Axis;
            //return new DistanceResult(point, position, Position.Distance(point, position));
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the point on a plane closest to a given point
        //
        // Parameters:
        //   point:
        //     Reference point
        //
        //   plane:
        //     Plane
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Remarks:
        //     The closest point on the plane is returned in DistanceResult.Point2.
        //
        //     If you only want to know the distance between the objects (and not the closest
        //     points), then the Snap.Compute.Distance function may be more convenient.
        public static DistanceResult ClosestPoints(Point3d point, Surface.Plane plane)
        {
            //double num = point._Subtract(plane.Origin)._Multiply(plane.Normal);
            //NXOpen.Point3d p = point._Subtract( plane.Normal._Multiply(num));
            //return new DistanceResult(point, p, System.Math.Abs(num));
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes the closest points on two rays
        //
        // Parameters:
        //   ray1:
        //     The first ray
        //
        //   ray2:
        //     The second ray
        //
        // Returns:
        //     A Compute.DistanceResult object that contains the distance and the closest points
        //
        //
        // Remarks:
        //     If the two rays are parallel, then all pairs of points on the two rays are equally
        //     close. In this case, the two points that are returned are the ones that are closest
        //     to the origin.
        public static DistanceResult ClosestPoints(Curve.Ray ray1, Curve.Ray ray2)
        {
            //NXOpen.Point3d[] array = ClipRay(ray1);
            //NXOpen.Line line = Create.Line(array[0], array[1]);
            //NXOpen.Point3d[] array2 = ClipRay(ray2);
            //NXOpen.Line line2 = Create.Line(array2[0], array2[1]);
            //DistanceResult distanceResult = ClosestPoints(line, line2);
            //if (Vector.Angle(ray1.Axis, ray2.Axis) < 1E-05)
            //{
            //    NXOpen.Point3d point = ClosestPoints(NXOpen.Point3d.Origin, ray1).Point2;
            //    NXOpen.Point3d point2 = ClosestPoints(NXOpen.Point3d.Origin, ray2).Point2;
            //    distanceResult.Point1 = point;
            //    distanceResult.Point2 = point2;
            //}

            //NXOpen.NXObject.Delete(line, line2);
            //return distanceResult;
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Clips a ray to the model bounding box (the 1 KM cube), giving a line
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        // Returns:
        //     Line end-points
        internal static Point3d[] ClipRay(Curve.Ray ray)
        {
            //double num = -1000000.0;
            //double num2 = 1000000.0;
            //double num3 = 1E-16;
            //double num4 = 200.0 * UnitConversion.MetersToPartUnits;
            //double x = ray.Axis.X;
            //double x2 = ray.Origin.X;
            //if (System.Math.Abs(x) > num3)
            //{
            //    double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    double val2 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    num = System.Math.Max(val, num);
            //    num2 = System.Math.Min(val2, num2);
            //}

            //x = ray.Axis.Y;
            //x2 = ray.Origin.Y;
            //if (System.Math.Abs(x) > num3)
            //{
            //    double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    double val3 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    num = System.Math.Max(val, num);
            //    num2 = System.Math.Min(val3, num2);
            //}

            //x = ray.Axis.Z;
            //x2 = ray.Origin.Z;
            //if (System.Math.Abs(x) > num3)
            //{
            //    double val = System.Math.Min((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    double val4 = System.Math.Max((0.0 - num4 - x2) / x, (num4 - x2) / x);
            //    num = System.Math.Max(val, num);
            //    num2 = System.Math.Min(val4, num2);
            //}

            //NXOpen.Point3d position = ray.Origin + num * ray.Axis;
            //NXOpen.Point3d position2 = ray.Origin + num2 * ray.Axis;
            //return new NXOpen.Point3d[2] { position, position2 };
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Computes a single point of intersection of two curves or edges
        //
        // Parameters:
        //   icurve1:
        //     The first curve or edge
        //
        //   icurve2:
        //     The second curve or edge
        //
        //   nearPoint:
        //     A "help" point near the desired intersection
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersection is
        //     found)
        //
        // Remarks:
        //     If you want to calculate all points of intersection (rather than just one), please
        //     use this function instead.
        //
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect function.
        public static IntersectionResult IntersectInfo(ICurve icurve1, ICurve icurve2, Point3d nearPoint)
        {
            var nXOpenTag = icurve1.__Tag();
            var nXOpenTag2 = icurve2.__Tag();
            var array = nearPoint._ToArray();
            var out_info = default(UFCurve.IntersectInfo);
            ufsession_.Curve.Intersect(nXOpenTag, nXOpenTag2, array, out out_info);
            out_info.curve_parm = IcurveParameter(icurve1, out_info.curve_parm);
            out_info.entity_parms[0] = IcurveParameter(icurve2, out_info.entity_parms[0]);
            return IntersectionResult.Create(out_info);
        }

        private static double IcurveParameter(ICurve icurve, double param)
        {
            if(icurve is NXOpen.Curve)
            {
                var curve = (NXOpen.Curve)icurve;

                if(curve.__Factor() != 1.0)
                    return param * curve.__Factor();

                return param * (curve._MaxU() - curve._MinU()) + curve._MinU();
            }

            var edge = (Edge)icurve;

            if(edge.__Factor() != 1.0)
                return param * edge.__Factor();

            return param * (edge._MaxU() - edge._MinU()) + edge._MinU();
        }

        //
        // Summary:
        //     Computes a single point of intersection of a curve (or edge) and a plane
        //
        // Parameters:
        //   icurve:
        //     The curve or edge
        //
        //   plane:
        //     The plane
        //
        //   nearPoint:
        //     A "help" point near the desired intersection
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersection is
        //     found)
        //
        // Remarks:
        //     If you want to calculate all points of intersection (rather than just one), please
        //     use this function instead.
        //
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect function.
        public static IntersectionResult IntersectInfo(ICurve icurve, Surface.Plane plane, Point3d nearPoint)
        {
            var nXOpenTag = icurve.__Tag();
            var array = plane.Normal._ToArray();
            var array2 = plane.Normal._Multiply(plane.D)._ToArray();
            var markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpIntersectMark_999");
            ufsession_.Modl.CreatePlane(array2, array, out var plane_tag);
            var array3 = nearPoint._ToArray();
            var out_info = default(UFCurve.IntersectInfo);
            ufsession_.Curve.Intersect(nXOpenTag, plane_tag, array3, out out_info);
            session_.UndoToMark(markId, "TmpIntersectMark_999");
            out_info.curve_parm = IcurveParameter(icurve, out_info.curve_parm);
            return IntersectionResult.Create(out_info);
        }

        //
        // Summary:
        //     Computes a single point of intersection of a curve (or edge) and a face
        //
        // Parameters:
        //   icurve:
        //     The curve or edge
        //
        //   face:
        //     The face
        //
        //   nearPoint:
        //     A "help" point near the desired intersection
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersection is
        //     found)
        //
        // Remarks:
        //     If you want to calculate all points of intersection (rather than just one), please
        //     use this function instead.
        //
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect function.
        public static IntersectionResult IntersectInfo(ICurve icurve, Face face, Point3d nearPoint)
        {
            var nXOpenTag = icurve.__Tag();
            var array = nearPoint._ToArray();
            var out_info = default(UFCurve.IntersectInfo);
            ufsession_.Curve.Intersect(nXOpenTag, face.Tag, array, out out_info);
            out_info.curve_parm = IcurveParameter(icurve, out_info.curve_parm);
#pragma warning disable CS0618 // Type or member is obsolete
            out_info.entity_parms = new double[2]
            {
                face.__FactorU() * out_info.entity_parms[0],
                face.__FactorV() * out_info.entity_parms[1]
            };
#pragma warning restore CS0618 // Type or member is obsolete
            return IntersectionResult.Create(out_info);
        }

        //
        // Summary:
        //     Computes a single point of intersection of a ray and a face
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        //   face:
        //     The face
        //
        //   nearPoint:
        //     A "help" point near the desired intersection
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersection is
        //     found)
        //
        // Remarks:
        //     If you want to calculate all points of intersection (rather than just one), please
        //     use this function instead.
        //
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect function.
        [Obsolete("Need to check what type of face first. Look at Snap")]
        public static IntersectionResult IntersectInfo(Curve.Ray ray, Face face, Point3d nearPoint)
        {
            var markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpIntersectMark_999");
            var array = ClipRay(ray);
            var line = __work_part_.Curves.CreateLine(array[0], array[1]);
            var array2 = nearPoint._ToArray();
            var out_info = default(UFCurve.IntersectInfo);
            ufsession_.Curve.Intersect(line.__Tag(), face.Tag, array2, out out_info);
            session_.UndoToMark(markId, "TmpIntersectMark_999");
            var intersectionResult = IntersectionResult.Create(out_info);

            //if (intersectionResult != null)
            //{
            //    intersectionResult.CurveParameter = ((intersectionResult.Position - ray.Origin) * ray.Axis).Value;
            //    intersectionResult.ObjectParameters[0] = intersectionResult.ObjectParameters[0] * face.__FactorU();
            //    intersectionResult.ObjectParameters[1] = intersectionResult.ObjectParameters[1] * face.__FactorV();
            //}

            //return intersectionResult;

            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes all points of intersection of two curves or edges
        //
        // Parameters:
        //   icurve1:
        //     The first curve or edge
        //
        //   icurve2:
        //     The second curve or edge
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersections are
        //     found)
        //
        // Remarks:
        //     If you only want to calculate a single intersection point, please use this function
        //     instead.
        //
        //     If you only need the locations of the intersections, it may be more convenient
        //     to use the Compute.Intersect function.
        [Obsolete("Need to check what type of face first. Look at Snap")]
        public static IntersectionResult[] IntersectInfo(ICurve icurve1, ICurve icurve2)
        {
            //ufsession_.Modl.IntersectCurveToCurve(icurve1._Tag(), icurve2._Tag(), out var num_intersections, out var data);
            //IntersectionResult[] array = null;
            //if (num_intersections != 0)
            //{
            //    array = new IntersectionResult[num_intersections];
            //    for (int i = 0; i < array.Length; i++)
            //    {
            //        array[i] = new IntersectionResult();
            //    }

            //    for (int j = 0; j < num_intersections; j++)
            //    {
            //        array[j].Position = new NXOpen.Point3d(data[5 * j], data[5 * j + 1], data[5 * j + 2]);
            //        array[j].CurveParameter = icurve1.Parameter(array[j].Position.Value);
            //        array[j].ObjectParameters = new double[2];
            //        array[j].ObjectParameters[0] = icurve2.Parameter(array[j].Position.Value);
            //        array[j].ObjectParameters[1] = 0.0;
            //    }
            //}

            //return array;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes all points of intersection of a curve (or edge) and a plane
        //
        // Parameters:
        //   icurve:
        //     The curve or edge
        //
        //   plane:
        //     The plane
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersections are
        //     found)
        //
        // Remarks:
        //     If you only want to calculate a single intersection point, please use this function
        //     instead.
        //
        //     If you only need the locations of the intersections, it may be more convenient
        //     to use the Compute.Intersect functions.
        public static IntersectionResult[] IntersectInfo(ICurve icurve, Surface.Plane plane)
        {
            var markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpIntersectMark_999");
            var plane_tag = Tag.Null;
            ufsession_.Modl.CreatePlane(plane.Origin._ToArray(), plane.Normal._ToArray(), out plane_tag);
            ufsession_.Modl.IntersectCurveToPlane(icurve.__Tag(), plane_tag, out var num_intersections, out var data);
            session_.UndoToMark(markId, "TmpIntersectMark_999");
            IntersectionResult[] array = null;
            if(num_intersections != 0)
            {
                array = new IntersectionResult[num_intersections];
                for (var i = 0; i < array.Length; i++) array[i] = new IntersectionResult();

                for (var j = 0; j < num_intersections; j++)
                {
                    array[j].Position = new Point3d(data[4 * j], data[4 * j + 1], data[4 * j + 2]);
                    array[j].CurveParameter = icurve.__Parameter(array[j].Position);
                }
            }

            return array;
        }

        //
        // Summary:
        //     Computes all points of intersection of a curve (or edge) and a face
        //
        // Parameters:
        //   icurve:
        //     The curve or edge
        //
        //   face:
        //     The face
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersections are
        //     found)
        //
        // Remarks:
        //     If you only want to calculate a single intersection point, please use this function
        //     instead.
        //
        //     If you only need the locations of the intersections, it may be more convenient
        //     to use the Compute.Intersect function.
        public static IntersectionResult[] IntersectInfo(ICurve icurve, Face face)
        {
            ufsession_.Modl.IntersectCurveToFace(icurve.__Tag(), face.Tag, out var num_intersections, out var data);
            IntersectionResult[] array = null;
            if(num_intersections != 0)
            {
                array = new IntersectionResult[num_intersections];
                for (var i = 0; i < array.Length; i++) array[i] = new IntersectionResult();

                for (var j = 0; j < num_intersections; j++)
                {
                    array[j].Position = new Point3d(data[6 * j], data[6 * j + 1], data[6 * j + 2]);
                    array[j].CurveParameter = icurve.__Parameter(array[j].Position);
                    array[j].ObjectParameters = new double[2];
                    array[j].ObjectParameters[0] = face.Parameters(array[j].Position)[0];
                    array[j].ObjectParameters[1] = face.Parameters(array[j].Position)[1];
                }
            }

            return array;
        }

        //
        // Summary:
        //     Computes all points of intersection of a ray and a face
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        //   face:
        //     The face
        //
        // Returns:
        //     The result of the intersection calculation (or Nothing if no intersections are
        //     found)
        //
        // Remarks:
        //     If you only want to calculate a single intersection point, please use this function
        //     instead.
        //
        //     If you only need the locations of the intersections, it may be more convenient
        //     to use the Compute.Intersect function.
        public static IntersectionResult[] IntersectInfo(Curve.Ray ray, Face face)
        {
            var markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpIntersectMark_999");
            var array = ClipRay(ray);
            var line = __work_part_.Curves.CreateLine(array[0], array[1]);
            ufsession_.Modl.IntersectCurveToFace(line.__Tag(), face.Tag, out var num_intersections, out var data);
            IntersectionResult[] array2 = null;

            if(num_intersections != 0)
            {
                array2 = new IntersectionResult[num_intersections];
                for (var i = 0; i < array2.Length; i++) array2[i] = new IntersectionResult();

                for (var j = 0; j < num_intersections; j++)
                {
                    array2[j].Position = new Point3d(data[6 * j], data[6 * j + 1], data[6 * j + 2]);
                    var temp = line.Position(data[6 * j + 3]);
                    array2[j].CurveParameter = temp._Subtract(ray.Origin)._Multiply(ray.Axis);
                    array2[j].ObjectParameters = new double[2];
                    array2[j].ObjectParameters[0] = face.Parameters(array2[j].Position)[0];
                    array2[j].ObjectParameters[1] = face.Parameters(array2[j].Position)[1];
                }

                session_.UndoToMark(markId, "TmpIntersectMark_999");
            }

            return array2;
        }

        //
        // Summary:
        //     Computes a single point of intersection of a ray and a body
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        //   body:
        //     The body
        //
        //   nearPoint:
        //     A "help" point near the desired intersection
        //
        // Returns:
        //     The result of the intersection calculation
        //
        // Remarks:
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect function.
        [Obsolete]
        public static IntersectionResult IntersectInfo(Curve.Ray ray, Body body, Point3d nearPoint)
        {
            var markId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "TmpIntersectMark_999");
            var array = ClipRay(ray);
            var line = __work_part_.Curves.CreateLine(array[0], array[1]);
            var array2 = nearPoint._ToArray();
            var out_info = default(UFCurve.IntersectInfo);
            ufsession_.Curve.Intersect(line.Tag, body.Tag, array2, out out_info);
            session_.UndoToMark(markId, "TmpIntersectMark_999");
            var intersectionResult = IntersectionResult.Create(out_info);

            //if (intersectionResult != null)
            //    intersectionResult.CurveParameter = ((intersectionResult.Position - ray.Origin) * ray.Axis).Value;

            //return intersectionResult;
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Computes a single point of intersection of a ray and a plane
        //
        // Parameters:
        //   ray:
        //     The ray
        //
        //   plane:
        //     The plane
        //
        // Returns:
        //     Location of the intersection point, or Nothing if no intersection exists
        //
        // Remarks:
        //     If the ray lies in the plane, then any point on the ray could be regarded as
        //     an intersection point. In this case, this function returns the point of the ray
        //     that is closest to the origin.
        //
        //     If you only need the location of the intersection, it may be more convenient
        //     to use the Compute.Intersect functions.
        [Obsolete]
        public static IntersectionResult IntersectInfo(Curve.Ray ray, Surface.Plane plane)
        {
            //NXOpen.Point3d? position = null;
            //double num = plane.D - ray.Origin._Subtract(_Point3dOrigin)._Multiply(plane.Normal);
            //double num2 = ray.Axis._Multiply(plane.Normal);

            //if (System.Math.Abs(num2) < 1E-12)
            //{
            //    if (System.Math.Abs(num) < 1E-14)
            //    {
            //        DistanceResult distanceResult = ClosestPoints( _Point3dOrigin, ray);
            //        position = distanceResult.Point2;
            //    }
            //}
            //else
            //{
            //    double num3 = num / num2;
            //    position = ray.Axis._Multiply(num3)._Add(ray.Origin);
            //}

            //if (position is null)
            //    throw new ArgumentException();

            //var temp = (NXOpen.Point3d)position;

            //UFCurve.IntersectInfo info = default(UFCurve.IntersectInfo);
            //info.curve_parm =   ((position - ray.Origin) * ray.Axis).Value;
            //info.curve_point = position.Value.Array;
            //info.entity_parms = new double[2];
            //info.type_of_intersection = 1;
            //return IntersectionResult.Create(info);

            throw new NotImplementedException();
        }

        //
        // Summary:
        //     The result of computing the distance between two objects
        //
        // Remarks:
        //     A DistanceResult is returned from the Snap.Compute.ClosestPoints functions.
        public class DistanceResult
        {
            //
            // Summary:
            //     Construct a result object
            //
            // Parameters:
            //   p1:
            //     The point on the first object
            //
            //   p2:
            //     The point on the second object
            //
            //   distance:
            //     The minimum distance
            internal DistanceResult(Point3d p1, Point3d p2, double distance)
            {
                Point1 = p1;
                Point2 = p2;
                Distance = distance;
            }

            //
            // Summary:
            //     The point on the first object at which the minimum distance is attained
            public Point3d Point1 { get; internal set; }

            //
            // Summary:
            //     The point on the second object at which the minimum distance is attained
            public Point3d Point2 { get; internal set; }

            //
            // Summary:
            //     The minimum distance between the two objects
            public double Distance { get; internal set; }
        }

        internal static class UnitConversion
        {
            //
            // Summary:
            //     Multiply by this number to convert Part Units to Millimeters (1 or 25.4)
            internal static double PartUnitsToMillimeters => MillimetersPerUnit;

            //
            // Summary:
            //     Multiply by this number to convert Millimeters to Part Units (1 or 0.04)
            internal static double MillimetersToPartUnits => 1.0 / PartUnitsToMillimeters;

            //
            // Summary:
            //     Multiply by this number to convert Part Units to Inches (1 or 0.04)
            internal static double PartUnitsToInches => InchesPerUnit;

            //
            // Summary:
            //     Multiply by this number to convert Inches to Part Units (either 1 or 25.4)
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
        }

        //
        // Summary:
        //     Represents the result of an intersection calculation
        public class IntersectionResult
        {
            //
            // Summary:
            //     Location of the intersection point (or Nothing if no intersection was found)
            public Point3d Position { get; internal set; }

            //
            // Summary:
            //     The parameter value of the intersection point on the first object (which is either
            //     a curve or a ray)
            public double CurveParameter { get; internal set; }

            //
            // Summary:
            //     The parameter value(s) of the intersection point on the second object
            //
            // Remarks:
            //     The array always has length = 2, but its elements have different meaning, depending
            //     on the type of the second object:
            //
            //     Face or body -- ObjectParameters[0,1] = the uv-values at the intersection point.
            //
            //
            //     Curve or edge -- ObjectParameters[0] = the curve parameter value at the intersection
            //     point.
            //
            //     Plane or datum plane -- ObjectParameters has no meaningful information
            public double[] ObjectParameters { get; internal set; }

            /// <summary>
            ///     Creates a Snap.Compute.IntersectionResult from an NXOpen.UF.UFCurve.IntersectInfo
            /// </summary>
            /// <param name="info">The NXOpen.UF.UFCurve.IntersectInfo</param>
            /// <remarks>
            ///     Just copies information. Leaves the ObjectParameters field set to Nothing (to<br />
            ///     be filled in later, by the caller)
            /// </remarks>
            internal static IntersectionResult Create(UFCurve.IntersectInfo info)
            {
                if(info.type_of_intersection != 1) return null;

                return new IntersectionResult
                {
                    Position = info.curve_point._ToPoint3d(),
                    CurveParameter = info.curve_parm,
                    ObjectParameters = info.entity_parms
                };
            }
        }
    }
}