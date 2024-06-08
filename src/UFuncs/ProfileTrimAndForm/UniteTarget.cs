using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Features;

namespace TSG_Library.UFuncs
{
    public struct UniteTarget
    {
        public Extrude Unite { get; }

        public IEnumerable<SlugObj> Slugs { get; }

        private Vector3d Vector => Slugs.First().Vector;

        public Face TopMostFace => throw
            //NXOpen.Vector3d unitizedVector = Vector.Unitize();
            //NXOpen.Face[] validFaces = Unite.GetFaces()
            //    .Where(face => face.SolidFaceType == NXOpen.Face.FaceType.Planar)
            //    .Where(face => unitizedVector._IsParallelTo(face._NormalVector()))
            //    .ToArray();
            //NXOpen.CartesianCoordinateSystem coordSystem =  unitizedVector.create_coordinate_system();
            //NXOpen.CartesianCoordinateSystem abs = NXOpen.Vector3d.z_unit().create_coordinate_system();
            //Face_ maxFace = null;
            //double maxZ = double.MinValue;
            //foreach (Face_ face1 in validFaces)
            //{
            //    double other = NXOpen.CartesianCoordinateSystem.__MapCsysToCsys(face1.edges.First().start_point, abs, coordSystem).z;
            //    if (other > maxZ)
            //        maxFace = face1;
            //}
            //return maxFace;
            new NotImplementedException();

        public TaggedObject PadObj { get; private set; }

        private readonly List<TaggedObject> _punches;

        public IEnumerable<TaggedObject> Punches => _punches;

        public void AddPunch(TaggedObject tagged)
        {
            _punches.Add(tagged);
        }

        public void SetPadObj(TaggedObject tagged)
        {
            PadObj = tagged;
        }

        public UniteTarget(Extrude extrude, IEnumerable<SlugObj> slugs) :
            this()
        {
            Unite = extrude;
            Slugs = slugs;
            _punches = new List<TaggedObject>();
        }
    }
}