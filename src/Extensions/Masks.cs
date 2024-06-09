using NXOpen;
using NXOpen.UF;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Masks

        public static Selection.MaskTriple[] mask = new Selection.MaskTriple[5];

        public static Selection.MaskTriple componentType =
            new Selection.MaskTriple(UF_component_type, 0, 0);

        public static Selection.MaskTriple edgeType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE);

        public static Selection.MaskTriple bodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static Selection.MaskTriple solidBodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static Selection.MaskTriple pointType = new Selection.MaskTriple(UF_caegeom_type, 0, 0);
        public static Selection.MaskTriple planeType = new Selection.MaskTriple(UF_plane_type, 0, 0);

        public static Selection.MaskTriple datumPlaneType =
            new Selection.MaskTriple(UF_datum_plane_type, 0, 0);

        public static Selection.MaskTriple datumCsysType =
            new Selection.MaskTriple(UF_csys_normal_subtype, 0, UF_csys_wcs_subtype);

        public static Selection.MaskTriple splineType = new Selection.MaskTriple(UF_spline_type, 0, 0);

        public static Selection.MaskTriple handlePointYpe =
            new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);

        public static Selection.MaskTriple objectType =
            new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);

        public static Selection.MaskTriple faceType =
            new Selection.MaskTriple(UF_face_type, UF_bounded_plane_type, 0);

        public static Selection.MaskTriple featureType =
            new Selection.MaskTriple(UF_feature_type, UF_feature_subtype, 0);

        public static Selection.MaskTriple objColorType =
            new Selection.MaskTriple(UF_solid_type, UF_face_type, UF_UI_SEL_FEATURE_ANY_FACE);

        /// <summary>
        ///     Returns the mask for an NXOpen.Assemblies.Component.
        /// </summary>
        public static UFUi.Mask ComponentMask =>
            //[IgnoreExtensionAspect]
            new UFUi.Mask
            {
                object_type = UF_component_type,
                object_subtype = 0,
                solid_type = 0
            };

        public static UFUi.Mask BodyMask
        {
            //[IgnoreExtensionAspect]
            get
            {
                var mask1 = new UFUi.Mask
                    { object_type = 70, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }

        #endregion
    }
}