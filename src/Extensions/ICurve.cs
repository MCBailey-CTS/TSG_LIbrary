using System;
using NXOpen;
using NXOpen.UF;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region ICurve

        public static double __Parameter(this ICurve icurve, Point3d point)
        {
            switch (icurve)
            {
                case Curve __curve__:
                    return __curve__.__Parameter(point);
                case Edge __edge__:
#pragma warning disable CS0612 // Type or member is obsolete
                    return __edge__.__Parameter(point);
#pragma warning restore CS0612 // Type or member is obsolete
                default:
                    throw new ArgumentException("Unknown curve type");
            }
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __StartPoint(this ICurve icurve)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Point3d __EndPoint(this ICurve icurve)
        {
            throw new NotImplementedException();
        }


        //
        // Summary:
        //     Calculates the unit binormal at a given parameter value
        //
        // Parameters:
        //   value:
        //     Parameter value
        //
        // Returns:
        //     Unit binormal
        //
        // Remarks:
        //     The binormal is normal to the "osculating plane" of the curve at the given parameter
        //     value (the plane that most closely matches the curve). So, if the curve is planar,
        //     the binormal is normal to the plane of the curve.
        //
        //     The binormal is the cross product of the tangent and the normal: B = Cross(T,N).
        public static Vector3d __Binormal(this ICurve curve, double value)
        {
            UFEval eval = ufsession_.Eval;
            eval.Initialize2(curve.__Tag(), out IntPtr evaluator);
            double[] array = new double[3];
            double[] array2 = new double[3];
            double[] array3 = new double[3];
            double[] array4 = new double[3];
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, array, array2, array3, array4);
            eval.Free(evaluator);
            return array4.__ToVector3d();
        }

        public static Tag __Tag(this ICurve curve)
        {
            if (curve is TaggedObject taggedObject)
                return taggedObject.Tag;

            throw new ArgumentException("Curve was not a tagged object");
        }

        public static bool __IsCurve(this ICurve icurve)
        {
            return icurve is Curve;
        }

        public static bool __IsEdge(this ICurve icurve)
        {
            return icurve is Edge;
        }

        [Obsolete]
        public static void __IsLinearCurve(this ICurve icurve)
        {
            //if (icurve.ObjectType == ObjectTypes.Type.Line)
            //{
            //    throw new ArgumentException("The input curve is a straight line.");
            //}

            //if (icurve.ObjectType == ObjectTypes.Type.Edge && icurve.ObjectSubType == ObjectTypes.SubType.EdgeLine)
            //{
            //    throw new ArgumentException("The input curve is a straight line.");
            //}
            throw new NotImplementedException();
        }

        public static void __IsPlanar(this ICurve icurve)
        {
            double[] data = new double[6];
            ufsession_.Modl.AskObjDimensionality(icurve.__Tag(), out int dimensionality, data);

            if (dimensionality == 3)
                throw new ArgumentException("The input curve is not planar.");
        }

        [Obsolete]
        public static void __IsParallelToXYPlane(this ICurve icurve)
        {
            //double num = Vector.Angle(icurve.Binormal(icurve.MinU), Vector.AxisZ);
            //if (num > 1E-06 || num > 179.999999)
            //{
            //    throw new ArgumentException("The input curve does not lie in a plane parallel to X-Y plane.");
            //}
            throw new NotImplementedException();
        }

        #endregion
    }
}