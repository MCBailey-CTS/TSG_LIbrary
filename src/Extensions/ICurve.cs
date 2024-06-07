using System;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.Extensions
{
    [ExtensionsAspect]
    public static class __ICurve__
    {
        public static double __Parameter(this ICurve icurve, Point3d point)
        {
            switch (icurve)
            {
                case Curve __curve__:
                    return __Parameter(__curve__, point);
                case Edge __edge__:
                    return __Parameter(__edge__, point);
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
            var eval = ufsession_.Eval;
            eval.Initialize2(curve.__Tag(), out var evaluator);
            var array = new double[3];
            var point = array;
            var array2 = new double[3];
            var tangent = array2;
            var array3 = new double[3];
            var normal = array3;
            var array4 = new double[3];
            var array5 = array4;
            value /= Factor;
            eval.EvaluateUnitVectors(evaluator, value, point, tangent, normal, array5);
            eval.Free(evaluator);
            return array5._ToVector3d();
        }

        public static Tag __Tag(this ICurve curve)
        {
            if(curve is TaggedObject taggedObject)
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
            var data = new double[6];
            ufsession_.Modl.AskObjDimensionality(icurve.__Tag(), out var dimensionality, data);

            if(dimensionality == 3)
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
    }
}