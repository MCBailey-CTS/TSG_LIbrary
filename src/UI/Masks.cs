using System;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Extensions;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.Utilities
{
    public static class Masks
    {
        public const int mask_object_type_spline = UF_spline_type;
        public static Selection.MaskTriple[] Mask = new Selection.MaskTriple[5];
        public static Selection.MaskTriple ComponentType = new Selection.MaskTriple(63, 0, 0);
        public static Selection.MaskTriple EdgeType = new Selection.MaskTriple(70, 0, 1);
        public static Selection.MaskTriple BodyType = new Selection.MaskTriple(70, 0, 0);
        public static Selection.MaskTriple SolidBodyType = new Selection.MaskTriple(70, 0, 35);
        public static Selection.MaskTriple PointType = new Selection.MaskTriple(31, 0, 0);
        public static Selection.MaskTriple PlaneType = new Selection.MaskTriple(46, 0, 0);
        public static Selection.MaskTriple DatumPlaneType = new Selection.MaskTriple(197, 0, 0);
        public static Selection.MaskTriple DatumCsysType = new Selection.MaskTriple(0, 0, 1);
        public static Selection.MaskTriple SplineType = new Selection.MaskTriple(9, 0, 0);
        public static Selection.MaskTriple HandlePointYpe = new Selection.MaskTriple(2, 0, 0);
        public static Selection.MaskTriple ObjectType = new Selection.MaskTriple(70, 0, 0);
        public static Selection.MaskTriple FaceType = new Selection.MaskTriple(71, 22, 0);
        public static Selection.MaskTriple FeatureType = new Selection.MaskTriple(205, 0, 0);
        public static Selection.MaskTriple ObjColorType = new Selection.MaskTriple(70, 71, 20);


        /// <summary>
        ///     Returns the mask for an NXOpen.Assemblies.Component.
        /// </summary>
        //public static NXOpen.UF.UFUi.Mask ComponentMask => new NXOpen.UF.UFUi.Mask
        //{
        //    object_type = NXOpen.UF.UFConstants.UF_component_type,
        //    object_subtype = 0,
        //    solid_type = 0
        //};
        public static Selection.MaskTriple[] mask = new Selection.MaskTriple[5];

        public static Selection.MaskTriple componentType = new Selection.MaskTriple(UF_component_type, 0, 0);

        public static Selection.MaskTriple edgeType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE);

        public static Selection.MaskTriple
            bodyType = new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static Selection.MaskTriple solidBodyType =
            new Selection.MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static Selection.MaskTriple pointType = new Selection.MaskTriple(UF_caegeom_type, 0, 0);
        public static Selection.MaskTriple planeType = new Selection.MaskTriple(UF_plane_type, 0, 0);
        public static Selection.MaskTriple datumPlaneType = new Selection.MaskTriple(UF_datum_plane_type, 0, 0);

        public static Selection.MaskTriple datumCsysType =
            new Selection.MaskTriple(UF_csys_normal_subtype, 0, UF_csys_wcs_subtype);

        public static Selection.MaskTriple splineType = new Selection.MaskTriple(UF_spline_type, 0, 0);

        public static Selection.MaskTriple
            handlePointYpe = new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);

        public static Selection.MaskTriple objectType =
            new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);

        public static Selection.MaskTriple faceType = new Selection.MaskTriple(UF_face_type, UF_bounded_plane_type, 0);

        public static Selection.MaskTriple featureType =
            new Selection.MaskTriple(UF_feature_type, UF_feature_subtype, 0);

        public static Selection.MaskTriple objColorType =
            new Selection.MaskTriple(UF_solid_type, UF_face_type, UF_UI_SEL_FEATURE_ANY_FACE);

        public static readonly UFUi.Mask Line_Mask = CreateMask(UF_line_type, 0, 0);

        public static readonly UFUi.Mask Arc_OpenClosed_Mask = CreateMask(UF_circle_type, 0, 0);


        public static readonly UFUi.Mask Conic_Mask = CreateMask(UF_conic_type, 0, 0);

        public static readonly UFUi.Mask Ellipse_Mask = CreateMask(UF_conic_type, UF_conic_ellipse_subtype, 0);

        public static readonly UFUi.Mask Parabola_Mask = CreateMask(UF_conic_type, UF_conic_parabola_subtype, 0);

        public static readonly UFUi.Mask Hyperbola_Mask = CreateMask(UF_conic_type, UF_conic_hyperbola_subtype, 0);

        public static readonly UFUi.Mask Spline_Mask = CreateMask(UF_spline_type, 0, 0);

        public static readonly UFUi.Mask Component_Mask = CreateMask(UF_component_type, UF_component_subtype, 0);

        public static readonly UFUi.Mask Body_Mask = CreateMask(UF_solid_type, 0, 0);

        public static readonly UFUi.Mask SolidBody_Mask = CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static readonly UFUi.Mask SheetBody_Mask = CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static readonly UFUi.Mask Edge_Mask = CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE);

        public static readonly UFUi.Mask Face_Mask = CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_FACE);

        private static readonly UFSession __uf = UFSession.GetUFSession();

        public static UFUi.Mask Mask_Component { get; } = new UFUi.Mask
            { object_type = UF_component_type, object_subtype = UF_component_subtype, solid_type = 0 };

        public static UFUi.Mask Mask_Body { get; } = new UFUi.Mask
            { object_type = UF_solid_type, object_subtype = 0, solid_type = 0 };

        public static UFUi.Mask Mask_SolidBody { get; } = new UFUi.Mask
            { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_BODY };

        public static UFUi.Mask Mask_SheetBody { get; } = new UFUi.Mask
            { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_SHEET_BODY };

        public static UFUi.Mask Mask_Edge { get; } = new UFUi.Mask
            { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_ANY_EDGE };

        public static UFUi.Mask Mask_Face { get; } = new UFUi.Mask
            { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_ANY_FACE };

        public static UFUi.Mask[] Curve_Mask { get; } = { Line_Mask, Arc_OpenClosed_Mask, Conic_Mask, Spline_Mask };


        /// <summary>
        ///     Returns the mask for an NXOpen.Assemblies.Component.
        /// </summary>
        public static UFUi.Mask ComponentMask
        {
            get
            {
                UFUi.Mask mask1 = new UFUi.Mask { object_type = 63, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }

        public static UFUi.Mask BodyMask
        {
            get
            {
                UFUi.Mask mask1 = new UFUi.Mask { object_type = 70, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }

        public static UFUi.Mask EdgeMask
        {
            get
            {
                UFUi.Mask mask1 = new UFUi.Mask { object_type = 70, object_subtype = 0, solid_type = 1 };
                return mask1;
            }
        }

        public static UFUi.Mask FaceMask
        {
            get
            {
                UFUi.Mask mask1 = new UFUi.Mask { object_type = 70, object_subtype = 0, solid_type = 20 };
                return mask1;
            }
        }

        public static UFUi.Mask CurveMask
        {
            get
            {
                UFUi.Mask mask1 = new UFUi.Mask { object_type = 2, object_subtype = 0, solid_type = 0 };
                return mask1;
            }
        }


        private static UFUi.Mask CreateMask(int object_type, int object_subtype, int solid_type)
        {
            return new UFUi.Mask
            {
                object_type = object_type,
                object_subtype = object_subtype,
                solid_type = solid_type
            };
        }

        public static int mask_for_bodies(IntPtr select_, IntPtr handle)
        {
            UFUi.Mask mask = new UFUi.Mask
                { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_BODY };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });

                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();

                return UF_UI_SEL_FAILURE;
            }
        }

        public static int mask_for_datum_planes(IntPtr select_, IntPtr user_data)
        {
            //UF_UI_mask_t mask = { UF_datum_plane_type, 0, 0 };
            //if (!UF_CALL(UF_UI_set_sel_mask(select,
            //        UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }

        public static int mask_body_type(IntPtr select_, IntPtr user_data)
        {
            //UF_UI_mask_t mask = { UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY };
            //if (!UF_CALL(UF_UI_set_sel_mask(select,
            //        UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)) &&
            //    !UF_CALL(UF_UI_set_sel_procs(select, filter_body_type, NULL, type)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }

        public static int mask_for_components(IntPtr select_, IntPtr user_data)
        {
            UFUi.Mask mask = new UFUi.Mask { object_type = UF_component_type, object_subtype = 0, solid_type = 0 };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });
                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static int mask_for_curves(IntPtr select_, IntPtr handle)
        {
            UFUi.Mask[] mask =
            {
                new UFUi.Mask { object_type = UF_line_type, object_subtype = 0, solid_type = 0 },
                new UFUi.Mask { object_type = UF_circle_type, object_subtype = 0, solid_type = 0 },
                new UFUi.Mask { object_type = UF_conic_type, object_subtype = UF_all_subtype, solid_type = 0 },
                new UFUi.Mask { object_type = UF_spline_type, object_subtype = 0, solid_type = 0 }
            };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 4, mask);
                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();

                return UF_UI_SEL_FAILURE;
            }
        }

        public static Tag ask_next_component(Tag part, Tag comp)
        {
            //int
            //    subtype,
            //    type;
            //while (!UF_CALL(UF_OBJ_cycle_objs_in_part(part, UF_component_type, &comp)) && (comp != NULL_TAG))
            //{
            //    UF_CALL(UF_OBJ_ask_type_and_subtype(comp, &type, &subtype));
            //    if (subtype == UF_part_occurrence_subtype)
            //        return comp;
            //}
            //return comp;
            throw new NotImplementedException();
        }

        public static int mask_for_components5(IntPtr select_, IntPtr user_data)
        {
            //	UF_UI_mask_t
            //		mask = { UF_component_type, 0, 0 };
            //	if (!(UF_UI_set_sel_mask(select, UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //		return (UF_UI_SEL_SUCCESS);
            //	else
            //		return (UF_UI_SEL_FAILURE);
            throw new NotSupportedException();
        }

        public static int mask_for_bodies2(IntPtr select_, IntPtr user_data)
        {
            //UF_UI_mask_t mask = { UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY };
            //if (!UF_CALL(UF_UI_set_sel_mask(select, UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }

        public static int mask_for_bodies1(IntPtr select_, IntPtr user_data)
        {
            //UF_UI_mask_t mask = { UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY };
            //if (!UF_CALL(UF_UI_set_sel_mask(select,
            //        UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }

        public static int mask_for_splines(IntPtr select_, IntPtr handle)
        {
            UFUi.Mask[] mask =
            {
                new UFUi.Mask { object_type = mask_object_type_spline, object_subtype = 0, solid_type = 0 },
                new UFUi.Mask
                    { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_ANY_EDGE }
            };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 2, mask);
                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static bool prompt_for_text(string prompt, string text)
        {
            //int
            //    n,
            //    resp;
            //resp = uc1600(prompt, text, &n);
            //if (resp == 3 || resp == 5) return TRUE;
            //return FALSE;
            throw new NotImplementedException();
        }

        public static int mask_for_points(IntPtr select_, IntPtr handle)
        {
            UFUi.Mask mask = new UFUi.Mask { object_type = UF_point_type, object_subtype = 0, solid_type = 0 };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });
                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static int mask_for_components54(IntPtr select_, IntPtr handle)
        {
            //UF_UI_mask_t mask = { UF_component_type, 0, 0 };
            //if (!UF_CALL(UF_UI_set_sel_mask(select, UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }


        public static int mask_for_edges(IntPtr select_, IntPtr handle)
        {
            //UF_UI_mask_t mask = { UF_solid_type, 0, UF_UI_SEL_FEATURE_ANY_EDGE };
            //if (!UF_CALL(UF_UI_set_sel_mask(select, UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }

        public static int mask_for_bodies98(IntPtr select_, IntPtr handle)
        {
            //UF_UI_mask_t mask = { UF_face_type, 0, UF_UI_SEL_FEATURE_BODY };
            //if (!UF_CALL(UF_UI_set_sel_mask(select, UF_UI_SEL_MASK_CLEAR_AND_ENABLE_SPECIFIC, 1, &mask)))
            //    return (UF_UI_SEL_SUCCESS);
            //else
            //    return (UF_UI_SEL_FAILURE);
            throw new NotImplementedException();
        }


        public static int mask_for_datum_planes6(IntPtr select_, IntPtr handle)
        {
            try
            {
                UFUi.Mask mask = new UFUi.Mask
                    { object_type = UF_datum_plane_type, object_subtype = 0, solid_type = 0 };

                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });

                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static int mask_for_splines9(IntPtr select_, IntPtr handle)
        {
            try
            {
                UFUi.Mask[] mask =
                {
                    new UFUi.Mask
                    {
                        object_type = UF_spline_type,
                        object_subtype = 0,
                        solid_type = 0
                    },
                    new UFUi.Mask
                    {
                        object_type = UF_solid_type,
                        object_subtype = 0,
                        solid_type = UF_UI_SEL_FEATURE_BCURVE_EDGE
                    }
                };

                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 2, mask);

                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static int mask_for_faces(IntPtr select_, IntPtr user_data)
        {
            try
            {
                UFUi.Mask mask = new UFUi.Mask
                    { object_type = UF_solid_type, object_subtype = 0, solid_type = UF_UI_SEL_FEATURE_ANY_FACE };
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });
                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }

        public static int prompt_for_text67(string prompt, string text)
        {
            return __uf.Ui.AskStringInput(prompt, ref text, out _);
        }

        public static int mask_for_components_add_fasteners(IntPtr select_, IntPtr handle)
        {
            UFUi.Mask mask = new UFUi.Mask { object_type = UF_component_type, object_subtype = 0, solid_type = 0 };

            try
            {
                __uf.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, new[] { mask });

                return UF_UI_SEL_SUCCESS;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return UF_UI_SEL_FAILURE;
            }
        }
    }
}