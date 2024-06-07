using NXOpen;
using NXOpen.Features;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static string __FeatureType(Feature feat)
        {
            return feat.FeatureType;
        }

        public static Feature[] __GetAllChildren(Feature feat)
        {
            return feat.GetAllChildren();
        }

        public static Body[] __GetBodies(Feature feat)
        {
            return feat.GetBodies();
        }

        public static Feature[] __GetChildren(Feature feat)
        {
            return feat.GetChildren();
        }

        public static Edge[] __GetEdges(Feature feat)
        {
            return feat.GetEdges();
        }

        public static NXObject[] __GetEntities(Feature feat)
        {
            return feat.GetEntities();
        }

        public static Expression[] __GetExpressions(Feature feat)
        {
            return feat.GetExpressions();
        }

        public static Face[] __GetFaces(Feature feat)
        {
            return feat.GetFaces();
        }

        public static NXColor __GetFeatureColor(Feature feat)
        {
            return feat.GetFeatureColor();
        }

        public static Feature[] __GetParents(Feature feat)
        {
            return feat.GetParents();
        }

        public static Section[] __GetSections(Feature feat)
        {
            return feat.GetSections();
        }

        public static void __HideBody(Feature feat)
        {
            feat.HideBody();
        }

        public static void __HideParents(Feature feat)
        {
            feat.Unsuppress();
        }

        public static void __Highlight(Feature feat)
        {
            feat.Highlight();
        }

        public static void __LoadParentPart(Feature feat)
        {
            feat.LoadParentPart();
        }

        public static void __MakeCurrentFeature(Feature feat)
        {
            feat.MakeCurrentFeature();
        }

        public static void __RemoveParameters(Feature feat)
        {
            feat.RemoveParameters();
        }

        public static void __ShowBody(Feature feat, bool moveCurves)
        {
            feat.ShowBody(moveCurves);
        }

        public static void __ShowParents(Feature feat, bool moveCurves)
        {
            feat.ShowParents(moveCurves);
        }

        public static void __Suppress(Feature feat)
        {
            feat.Suppress();
        }

        public static int __Timestamp(Feature feat)
        {
            return feat.Timestamp;
        }

        public static void __Unhighlight(Feature feat)
        {
            feat.Unhighlight();
        }

        public static void __Unsuppress(Feature feat)
        {
            feat.Unsuppress();
        }
    }
}