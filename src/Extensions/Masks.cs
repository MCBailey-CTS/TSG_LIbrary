using NXOpen;
using NXOpen.UF;
using static NXOpen.UF.UFConstants;
// ReSharper disable UnusedMember.Global

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Masks

        public static Selection.MaskTriple[] Mask = new Selection.MaskTriple[5];

        public static Selection.MaskTriple ComponentType =
            new Selection.MaskTriple(UF_component_type, 0, 0);

        public static Selection.MaskTriple EdgeType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE);

        public static Selection.MaskTriple BodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static Selection.MaskTriple SolidBodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static Selection.MaskTriple PointType = new Selection.MaskTriple(UF_caegeom_type, 0, 0);
        public static Selection.MaskTriple PlaneType = new Selection.MaskTriple(UF_plane_type, 0, 0);

        public static Selection.MaskTriple DatumPlaneType =
            new Selection.MaskTriple(UF_datum_plane_type, 0, 0);

        public static Selection.MaskTriple DatumCsysType =
            new Selection.MaskTriple(UF_csys_normal_subtype, 0, UF_csys_wcs_subtype);

        public static Selection.MaskTriple SplineType = new Selection.MaskTriple(UF_spline_type, 0, 0);

        public static Selection.MaskTriple HandlePointYpe =
            new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);

        public static Selection.MaskTriple ObjectType =
            new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);

        public static Selection.MaskTriple FaceType =
            new Selection.MaskTriple(UF_face_type, UF_bounded_plane_type, 0);

        public static Selection.MaskTriple FeatureType =
            new Selection.MaskTriple(UF_feature_type, UF_feature_subtype, 0);

        public static Selection.MaskTriple ObjColorType =
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
                UFUi.Mask mask1 = new UFUi.Mask
                    { object_type = 70, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }

        #endregion
    }
}