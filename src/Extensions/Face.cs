﻿using System;
using NXOpen;
using NXOpen.UF;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Face

        public static Point3d[] __EdgePositions(this Face __face)
        {
            //List<NXOpen.Point3d> points = new List<NXOpen.Point3d>();

            //foreach (NXOpen.Edge edge in __face.GetEdges())
            //{
            //    edge.GetVertices(out NXOpen.Point3d poin1, out NXOpen.Point3d point2);

            //    points.Add(poin1);
            //    points.Add(point2);
            //}

            //return points.ToHashSet(new EqualityPosition()).ToArray();

            throw new NotImplementedException();
        }


        public static Vector3d __NormalVector(this Face __face)
        {
            double[] point = new double[3];
            double[] dir = new double[3];
            double[] box = new double[6];
            ufsession_.Modl.AskFaceData(__face.Tag, out int type, point, dir, box, out double radius,
                out double rad_data, out int norm_dir);

            if (type != 22)
                throw new InvalidOperationException("Cannot ask for the normal of a non planar face");

            return dir.__ToVector3d();
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapU = nxopenU * FactorU
        //
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenU = snapU / FactorU
        [Obsolete("Need to check what type of face first. Look at Snap")]
        internal static double __FactorU(this Face _)
        {
            throw new NotImplementedException();
            //return 180.0 / System.Math.PI;
        }

        //
        // Summary:
        //     Conversion factor between NX Open and SNAP parameter values. Needed because NX
        //     Open uses radians, where SNAP uses degrees
        //
        // Remarks:
        //     When converting an NX Open parameter to a SNAP parameter, snapV = nxopenV * FactorV
        //
        //
        //     When converting a SNAP parameter to an NX Open parameter, nxopenV = snapV / FactorV
        [Obsolete("Need to check what type of face first. Look at Snap")]
        internal static double __FactorV(this Face face)
        {
            //switch(face.SolidFaceType) { }


            //return UnitConversion.MetersToPartUnits;
            throw new NotImplementedException();
        }

        public static bool __IsPlanar(this Face face)
        {
            return face.SolidFaceType == Face.FaceType.Planar;
        }

        //
        // Summary:
        //     Finds surface (u,v) parameters at (or nearest to) a given point
        //
        // Parameters:
        //   point:
        //     The given point (which should be on or near to the surface)
        //
        // Returns:
        //     Surface (u,v) parameters at (or near to) the given point
        //
        // Remarks:
        //     The Parameters function and the Position function are designed to work together
        //     smoothly -- each of these functions is the "reverse" of the other. So, if f is
        //     any face and (u,v) is any pair of parameter values, then
        //
        //     f.Parameters(f.Position(u,v)) = (u,v)
        //
        //     Also, if p is any point on the face f, then
        //
        //     f.Position(c.Parameters(p)) = p
        //
        //     Note that this function finds parameter values on the underlying surface of the
        //     face. So, the values returned may correspond to a surface point that is actually
        //     outside the face.
        public static double[] __Parameters(this Face face, Point3d point)
        {
            UFEvalsf evalsf = ufsession_.Evalsf;
            evalsf.Initialize2(face.Tag, out IntPtr evaluator);
            double[] array = point.__ToArray();
            evalsf.FindClosestPoint(evaluator, array, out UFEvalsf.Pos3 srf_pos);
            evalsf.Free(out evaluator);
            double[] uv = srf_pos.uv;
#pragma warning disable CS0618 // Type or member is obsolete
            uv[0] = face.__FactorU() * uv[0];
            uv[1] = face.__FactorV() * uv[1];
#pragma warning restore CS0618 // Type or member is obsolete
            return uv;
        }

        #endregion
    }
}