using System;
using NXOpen;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static Line _Mirror(
            this Line line,
            Surface.Plane plane)
        {
            var start = line.StartPoint._Mirror(plane);
            var end = line.EndPoint._Mirror(plane);
            return line.__OwningPart().Curves.CreateLine(start, end);
        }


        public static Line _Copy(this Line line)
        {
            if(line.IsOccurrence)
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
    }
}