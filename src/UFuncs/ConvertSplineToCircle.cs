using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("convert-spline-to-circle")]
    public class ConvertSplineToCircle : _UFunc
    {
        [Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void execute()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            Spline[] selectedSplines = SelectSplines();

            if(selectedSplines is null || !selectedSplines.Any())
                return;

            throw new NotImplementedException();

            //foreach (NXOpen.Spline spline in selectedSplines)
            //{
            //    NXOpen.Spline originalSpline = Create.NXOpen.Spline(spline.Knots, spline.Poles, spline.Weights);
            //    List<Curve_> listOfArcs = GetCurves(spline);
            //    Curve_ firstArc = listOfArcs.FirstOrDefault(curve => curve.ObjectType == NXOpen_.ObjectTypes.Type.Circle);
            //    NXOpen.UF.UFSession.GetUFSession().Csys.AskMatrixOfObject(firstArc.Tag, out NXOpen.Tag matrixTag);
            //    NXOpen.NXMatrix nxMatrix = (NXOpen.NXMatrix)NXOpen.Utilities.NXObjectManager.Get(matrixTag);
            //    Orientation_ orientation = new Orientation_(nxMatrix.Element);
            //    NXOpen.Session.GetSession().Parts.Display.WCS.SetOriginAndMatrix(firstArc.StartPoint, orientation);
            //    List<NXOpen.Point3d> listOfPoints = new List<NXOpen.Point3d>();

            //    foreach (Curve_ curve in listOfArcs)
            //        listOfPoints.AddRange(new[] { curve.StartPoint, curve.EndPoint });

            //    NXOpen.Point3d[] wcsPoints = listOfPoints.Select(MapAcsToWcs).ToArray();
            //    NXOpen.Point3d wcsMaxXPoint = wcsPoints[0];

            //    foreach (NXOpen.Point3d position in wcsPoints)
            //        if (wcsMaxXPoint.X < position.X)
            //            wcsMaxXPoint = position;

            //    Point_ absMaxXPoint = Create.Point(MapWcsToAcs(wcsMaxXPoint));
            //    NXOpen.Point3d wcsMaxYPoint = wcsPoints[0];

            //    foreach (NXOpen.Point3d position in wcsPoints)
            //        if (wcsMaxYPoint.Y < position.Y)
            //            wcsMaxYPoint = position;

            //    Point_ absMaxYPoint = Create.Point(MapWcsToAcs(wcsMaxYPoint));
            //    NXOpen.Point3d wcsMinXPoint = wcsPoints[0];

            //    foreach (NXOpen.Point3d position in wcsPoints)
            //        if (wcsMinXPoint.X > position.X)
            //            wcsMinXPoint = position;

            //    Point_ absMinXPoint = Create.Point(MapWcsToAcs(wcsMinXPoint));
            //    NXOpen.Point3d wcsMinYPoint = wcsPoints[0];

            //    foreach (NXOpen.Point3d position in wcsPoints)
            //        if (wcsMinYPoint.Y > position.Y)
            //            wcsMinYPoint = position;

            //    Point_ absMinYPoint = Create.Point(MapWcsToAcs(wcsMinYPoint));
            //    NXOpen.Point3d midPosition = GetMidpoint(absMinYPoint.Position, absMaxYPoint.Position);
            //    Point_ midPoint = Create.Point(midPosition);
            //    NXOpen.Point3d maxXmaxY = MapWcsToAcs(new NXOpen.Point3d(wcsMaxXPoint.X, wcsMaxYPoint.Y, 0));
            //    NXOpen.Point3d maxXminY = MapWcsToAcs(new NXOpen.Point3d(wcsMaxXPoint.X, wcsMinYPoint.Y, 0));
            //    NXOpen.Point3d minXmaxY = MapWcsToAcs(new NXOpen.Point3d(wcsMinXPoint.X, wcsMaxXPoint.Y, 0));
            //    NXOpen.Point3d minXminY = MapWcsToAcs(new NXOpen.Point3d(wcsMinXPoint.X, wcsMinYPoint.Y, 0));

            //    Line_[] lines = CreateLines(midPoint.Position,
            //        absMaxXPoint.Position,
            //        absMaxYPoint.Position,
            //        absMinXPoint.Position,
            //        absMinYPoint.Position,
            //        maxXmaxY,
            //        maxXminY,
            //        minXmaxY,
            //        minXminY);

            //    NXOpen.Point3d[] intersections = GetIntersections(originalSpline, lines);
            //    double[] distances = GetDistances(midPoint.Position, intersections);
            //    double average = Average(distances);
            //    Create.Circle(midPoint.Position, orientation, average);
            //    session_.delete_objects(new TaggedObject_[] { absMaxXPoint, absMaxYPoint, absMinXPoint, absMinYPoint, midPoint });
            //    session_.delete_objects(lines);
            //    session_.delete_objects(listOfArcs.ToArray());
            //    NXOpen.Session.GetSession().Parts.Display.WCS.SetOriginAndMatrix(_Point3dOrigin, Orientation_.Identity);
            //}
        }

        private static Point3d GetMidpoint(Point3d p1, Point3d p2)
        {
            return new Point3d((p1.X + p2.X) / 2, (p2.Y + p1.Y) / 2, (p1.Z + p2.Z) / 2);
        }

        private static Line[] CreateLines(Point3d midPoint, params Point3d[] positions)
        {
            if(positions.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", "positions");

            return positions.Select(position => __work_part_.Curves.CreateLine(midPoint, position)).ToArray();
        }

        private static Point3d[] GetIntersections(Spline originalSpline, params Line[] lines)
        {
            //if (lines.Length == 0)
            //    throw new ArgumentException("Value cannot be an empty collection.", "lines");

            //List<Position> list = new List<Position>();

            //foreach (Line line in lines)
            //{
            //    Position[] intersections = Compute.Intersect(originalSpline, line);

            //    if (intersections == null)
            //        continue;

            //    list.AddRange(intersections);
            //}

            //return list.ToArray();

            throw new NotImplementedException();
        }

        private static double[] GetDistances(Point3d midPoint, params Point3d[] positions)
        {
            if(positions.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", "positions");

            return positions.Select(position => GetDistance(midPoint, position)).ToArray();
        }

        public static double GetDistance(Point3d p1, Point3d p2)
        {
            double distanceHelper(double d1, double d2)
            {
                return System.Math.Pow(d1 - d2, 2);
            }

            var x = distanceHelper(p1.X, p2.X);
            var y = distanceHelper(p1.Y, p2.Y);
            var z = distanceHelper(p1.Z, p2.Z);
            return System.Math.Sqrt(x + y + z);
        }

        private static double Average(params double[] doubles)
        {
            if(doubles.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", "doubles");

            return doubles.Sum() / doubles.Length;
        }

        private static Spline[] SelectSplines()
        {
            //const string splineSelect = "Select NXOpen.Spline";
            //Snap.UI.Selection.Dialog dialog = Snap.UI.Selection.SelectObject(splineSelect);
            //dialog.AllowMultiple = true;
            //dialog.Scope = Snap.UI.Selection.Dialog.SelectionScope.AnyInAssembly;
            //dialog.Title = splineSelect;
            //dialog.IncludeFeatures = false;
            //dialog.KeepHighlighted = false;
            //dialog.SetCurveFilter(Snap.NX.ObjectTypes.Type.Spline);
            //Snap.UI.Selection.Result result = dialog.Show();

            //if (result.Objects == null || !result.Objects.Any())
            //    return null;

            //return result.Objects
            //    .Select(__n => __n.NXOpenTag)
            //    .Select(Create__)
            //    .Cast<NXOpen.Spline>()
            //    .ToArray();

            throw new NotImplementedException();
        }

        private static List<Curve> GetCurves(Spline spline)
        {
            UFSession.GetUFSession().Curve.CreateSimplifiedCurve(
                1,
                new[] { spline.Tag },
                .0005,
                out var segments,
                out Tag[] segmentTags);

            spline.Blank();

            return segmentTags.Select(__t => __t.__To<Curve>()).ToList();
        }
    }
}