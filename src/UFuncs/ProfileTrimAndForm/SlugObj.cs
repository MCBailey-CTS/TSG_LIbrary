using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Features;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    public struct SlugObj
    {
        public Extrude Slug { get; }

        public Vector3d Vector { get; }

        public ISet<Curve> Polyline { get; }

        public Face TopFace { get; }

        public Point3d Origin => Polyline.First().__StartPoint();

        [Obsolete(nameof(NotImplementedException))]
        public SlugObj(Extrude extrudeSp, Vector3d vector, ISet<Curve> polyline) : this()
        {
            extrudeSp.OwningPart.__AssertIsWorkPart();
            Slug = extrudeSp;
            Vector = vector;
            Polyline = polyline;

            Face[] validFaces = extrudeSp.GetFaces()
                .Where(face => face.__IsPlanar())
                .Where(face =>
                    vector.__IsEqualTo(face.__NormalVector()) || vector.__IsEqualTo(face.__NormalVector().__Negate()))
                .ToArray();

            CoordinateSystem coordSystem = __work_part_.__CreateCsys(vector);
            CoordinateSystem abs = __work_part_.__CreateCsys(__Vector3dZ());

            TopFace = (Face)validFaces.MaxBy(face1 =>
                face1.GetEdges().First().__StartPoint().__MapCsysToCsys(abs, coordSystem).Z);
            Face maxFace = null;
            var maxZ = double.MinValue;

            foreach (Face face1 in validFaces)
            {
                var other = face1.GetEdges().First().__StartPoint().__MapCsysToCsys(abs, coordSystem).Z;

                if(other > maxZ)
                    maxFace = face1;
            }

            TopFace = maxFace;
        }
    }
}