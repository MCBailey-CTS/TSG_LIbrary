using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NXOpen;
using NXOpen.Features;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.UFuncs
{
    public struct SlugObj
    {
        public Extrude Slug { get; }

        public Vector3d Vector { get; }

        public ISet<Curve> Polyline { get; }

        public Face TopFace { get; }

        public Point3d Origin => Polyline.First()._StartPoint();

        [Obsolete(nameof(NotImplementedException))]
        public SlugObj(Extrude extrudeSp, Vector3d vector, ISet<Curve> polyline) : this()
        {
            extrudeSp.OwningPart.__AssertIsWorkPart();
            Slug = extrudeSp;
            Vector = vector;
            Polyline = polyline;

            var validFaces = extrudeSp.GetFaces()
                .Where(face => face._IsPlanar())
                .Where(face =>
                    vector._IsEqualTo(face._NormalVector()) || vector._IsEqualTo(face._NormalVector()._Negate()))
                .ToArray();

            var coordSystem = __work_part_.__CreateCsys(vector);
            var abs = __work_part_.__CreateCsys(_Vector3dZ());

            TopFace = (Face)validFaces.MaxBy(face1 =>
                face1.GetEdges().First()._StartPoint().__MapCsysToCsys(abs, coordSystem).Z);
            Face maxFace = null;
            var maxZ = double.MinValue;

            foreach (var face1 in validFaces)
            {
                var other = face1.GetEdges().First().__StartPoint().__MapCsysToCsys(abs, coordSystem).Z;

                if (other > maxZ)
                    maxFace = face1;
            }

            TopFace = maxFace;
        }
    }
}