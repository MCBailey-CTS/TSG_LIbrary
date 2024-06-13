using NXOpen;

namespace TSG_Library.Geom
{
    /// <summary>Represents a 3D box, with sides aligned with the ACS axes.</summary>
    /// <remarks>
    ///     Usually used to represent a region of space that encloses some object. Every<br />
    ///     NX object of type curve, edge, face, body, feature, or component has a Box property<br />
    ///     that returns its bounding box.<br /><br />
    ///     The rectangle corners are at the points (MinX, MinY, MinZ) and (MaxX, MaxY, MaxZ).
    /// </remarks>
    public class Box3d
    {
        /// <summary>Constructs a Box3D from minimum and maximum x, y, z values</summary>
        /// <param name="minX">Minimum x-value</param>
        /// <param name="minY">Minimum y-value</param>
        /// <param name="minZ">Minimum z-value</param>
        /// <param name="maxX">Maximum x-value</param>
        /// <param name="maxY">Maximum y-value</param>
        /// <param name="maxZ">Maximum z-value</param>
        public Box3d(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
        }

        /// <summary>Constructs a Box3D from given corner positions</summary>
        /// <param name="minXYZ">The "lower" corner position (MinX, MinY, MinZ)</param>
        /// <param name="maxXYZ">The "upper" corner position (MaxX, MaxY, MaxZ)</param>
        public Box3d(Point3d minXYZ, Point3d maxXYZ)
        {
            MinX = minXYZ.X;
            MinY = minXYZ.Y;
            MinZ = minXYZ.Z;
            MaxX = maxXYZ.X;
            MaxY = maxXYZ.Y;
            MaxZ = maxXYZ.Z;
        }

        /// <summary>The lower x-value</summary>
        public double MinX { get; set; }

        /// <summary>The lower y-value</summary>
        public double MinY { get; set; }

        /// <summary>The lower z-value</summary>
        public double MinZ { get; set; }

        /// <summary>The upper x-value</summary>
        public double MaxX { get; set; }

        /// <summary>The upper y-value</summary>
        public double MaxY { get; set; }

        /// <summary>The upper z-value</summary>
        public double MaxZ { get; set; }

        /// <summary>The lower corner of the box (min X, Y, Z values)</summary>
        public Point3d MinXYZ
        {
            get => new Point3d(MinX, MinY, MinZ);
            set
            {
                MinX = value.X;
                MinY = value.Y;
                MinZ = value.Z;
            }
        }

        /// <summary>The upper corner of the box (min X, Y, Z values)</summary>
        public Point3d MaxXYZ
        {
            get => new Point3d(MaxX, MaxY, MaxZ);
            set
            {
                MaxX = value.X;
                MaxY = value.Y;
                MaxZ = value.Z;
            }
        }

        /// <summary>The eight corners of the box</summary>
        /// <remarks>
        ///     The corners are returned in the following order:<br />
        ///     <br />
        ///     • Corners[0] is the point ( MinX, MinY, MinZ)<br />
        ///     • Corners[1] is the point ( MinX, MinY, MaxZ)<br />
        ///     • Corners[2] is the point ( MinX, MaxY, MinZ)<br />
        ///     • Corners[3] is the point ( MinX, MaxY, MaxZ)<br />
        ///     • Corners[4] is the point ( MaxX, MinY, MinZ)<br />
        ///     • Corners[5] is the point ( MaxX, MinY, MaxZ)<br />
        ///     • Corners[6] is the point ( MaxX, MaxY, MinZ)<br />
        ///     • Corners[7] is the point ( MaxX, MaxY, MaxZ)<br />
        /// </remarks>
        public Point3d[] Corners
        {
            get
            {
                Point3d position = new Point3d(MinX, MinY, MinZ);
                Point3d position2 = new Point3d(MinX, MinY, MaxZ);
                Point3d position3 = new Point3d(MinX, MaxY, MinZ);
                Point3d position4 = new Point3d(MinX, MaxY, MaxZ);
                Point3d position5 = new Point3d(MaxX, MinY, MinZ);
                Point3d position6 = new Point3d(MaxX, MinY, MaxZ);
                Point3d position7 = new Point3d(MaxX, MaxY, MinZ);
                Point3d position8 = new Point3d(MaxX, MaxY, MaxZ);
                return new Point3d[8]
                    { position, position2, position3, position4, position5, position6, position7, position8 };
            }
        }

        /// <summary>Combines several 3D boxes (giving the largest overall enclosing box)</summary>
        /// <param name="inputBoxes">The input boxes that are to be combined</param>
        /// <returns>Box enclosing all the input boxes</returns>
        public static Box3d Combine(params Box3d[] inputBoxes)
        {
            double num = double.PositiveInfinity;
            double num2 = double.NegativeInfinity;
            double num3 = num;
            double num4 = num;
            double num5 = num;
            double num6 = num2;
            double num7 = num2;
            double num8 = num2;
            foreach (Box3d box3d in inputBoxes)
                if (box3d != null)
                {
                    num3 = System.Math.Min(num3, box3d.MinX);
                    num4 = System.Math.Min(num4, box3d.MinY);
                    num5 = System.Math.Min(num5, box3d.MinZ);
                    num6 = System.Math.Max(num6, box3d.MaxX);
                    num7 = System.Math.Max(num7, box3d.MaxY);
                    num8 = System.Math.Max(num8, box3d.MaxZ);
                }

            return new Box3d(num3, num4, num5, num6, num7, num8);
        }
    }
}