using System;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Rules

        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule __Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        public static Body[] __Data(this BodyDumbRule rule)
        {
            rule.GetData(out Body[] bodies);
            return bodies;
        }

        [Obsolete(nameof(NotImplementedException))]
        public static BodyDumbRule __Mirror(
            this BodyDumbRule bodyDumbRule,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        public static EdgeTangentRule __CreateRuleEdgeTangent(
            this BasePart basePart,
            Edge startEdge,
            Edge endEdge = null,
            bool isFromStart = false,
            double? angleTolerance = null,
            bool hasSameConvexity = false,
            bool allowLaminarEdge = false)
        {
            var tol = angleTolerance ?? AngleTolerance;
            return basePart.ScRuleFactory.CreateRuleEdgeTangent(startEdge, endEdge, isFromStart, tol, hasSameConvexity,
                allowLaminarEdge);
        }


        public static void __Rule(this BasePart basePart)
        {
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
            //basePart.ScRuleFactory.
        }

        [Obsolete]
        public static ApparentChainingRule __CreateRuleApparentChainingRule(this BasePart basePart)
        {
            //basePart.ScRuleFactory.CreateRuleApparentChaining()
            throw new NotImplementedException();
        }

        public static CurveDumbRule __CreateRuleBaseCurveDumb(this BasePart basePart,
            params IBaseCurve[] ibaseCurves)
        {
            return basePart.ScRuleFactory.CreateRuleBaseCurveDumb(ibaseCurves);
        }

        public static BodyDumbRule __CreateRuleBodyDumb(this BasePart basePart,
            params Body[] bodies)
        {
            return basePart.ScRuleFactory.CreateRuleBodyDumb(bodies);
        }

        //public static void __CreateRuleBodyFeature(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleBodyGroup(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveChain(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveDumb(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveDumbFromPoints(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveFeature(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveFeatureChain(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveFeatureTangent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveGroup(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleCurveTangent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeBody(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeBoundary(this BasePart basePart)
        //{
        //}

        //[Obsolete]
        //public static EdgeChainRule __CreateRuleEdgeChain(this BasePart basePart)
        //{
        //    throw new NotImplementedException();
        //}

        //[Obsolete]
        //public static EdgeDumbRule __CreateRuleEdgeDumb(this BasePart basePart)
        //{
        //    throw new NotImplementedException();
        //}

        //public static void __CreateRuleEdgeFace(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeFeature(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeIntersect(this BasePart basePart)
        //{
        //}


        //public static void __CreateRuleEdgeMultipleSeedTangent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeSheetBoundary(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeTangent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeVertex(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleEdgeVertexTangent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceAdjacent(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceAllBlend(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceAndAdjacentFaces(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceBody(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceBossPocket(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceConnectedBlend(this BasePart basePart)
        //{
        //}

        //public static void __CreateRuleFaceDatum(this BasePart basePart)
        //{
        //}

        public static FaceDumbRule __CreateRuleFaceDumb(this BasePart basePart,
            params Face[] faces)
        {
            return basePart.ScRuleFactory.CreateRuleFaceDumb(faces);
        }

        [Obsolete]
        public static FaceFeatureRule __CreateRuleFaceFeature(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public static FaceTangentRule __CreateRuleFaceTangent(this BasePart basePart)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}