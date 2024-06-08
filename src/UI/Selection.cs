using System;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Utilities;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions;

namespace TSG_Library.Ui
{
    internal class Selection
    {
        public static readonly NXOpen.Selection.MaskTriple ComponentMask = CreateMask(UF_component_type, 0, 0);

        public static readonly NXOpen.Selection.MaskTriple SolidBodyMask =
            CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        public static readonly NXOpen.Selection.MaskTriple SheetBodyMask =
            CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static readonly NXOpen.Selection.MaskTriple LineMask = CreateMask(UF_line_type, 0, 0);

        public static readonly NXOpen.Selection.MaskTriple FaceMask = CreateMask(UF_solid_type, 0,
            UF_UI_SEL_FEATURE_ANY_FACE);

        public static readonly NXOpen.Selection.MaskTriple SplineMask = CreateMask(UF_spline_type, 0, 0);

        private static NXOpen.Selection.MaskTriple CreateMask(int object_type, int object_subtype, int solid_type)
        {
            return new NXOpen.Selection.MaskTriple
            {
                Type = object_type,
                Subtype = object_subtype,
                SolidBodySubtype = solid_type
            };
        }

        public static Curve[] SelectCurves()
        {
            UI.GetUI().SelectionManager.SelectTaggedObjects(
                "Select Curves",
                "Select Curves",
                NXOpen.Selection.SelectionScope.AnyInAssembly,
                false,
                new[] { NXOpen.Selection.SelectionType.Curves },
                out var __objects);

            return __objects.Cast<Curve>().ToArray();
        }


        public static Body SelectBody()
        {
            UI.GetUI().SelectionManager.SelectTaggedObject(
                "Select Body",
                "Select Body",
                NXOpen.Selection.SelectionScope.AnyInAssembly,
                NXOpen.Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { SolidBodyMask, SheetBodyMask },
                out var __object,
                out _);

            return (Body)__object;
        }

        public static Component SelectSingleComponent(Predicate<Component> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select single component",
                "Select single component",
                new[] { Masks.ComponentMask },
                predicate);
        }

        public static Component[] SelectManyComponents(Predicate<Component> predicate = null)
        {
            return SelectMultipleTaggedObjects(
                "Select many Components",
                "Select many Components",
                new[] { Masks.ComponentMask },
                predicate);
        }

        public static Face[] SelectManyFaces(Predicate<Face> predicate = null)
        {
            return SelectMultipleTaggedObjects(
                "Select Many Faces",
                "Select Many Faces",
                new[] { Masks.FaceMask },
                predicate);
        }

        public static Face SelectSingleFace(Predicate<Face> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Face",
                "Select Single Face",
                new[] { Masks.FaceMask },
                predicate);
        }

        public static Line SelectSingleLine(Predicate<Line> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Line",
                "Select Single Line",
                new[] { Masks.Line_Mask },
                predicate);
        }

        public static Spline SelectSingleSpline(Predicate<Spline> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Spline",
                "Select Single Spline",
                new[] { Masks.Spline_Mask },
                predicate);
        }

        public static Body SelectSingleSolidBody(Predicate<Body> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Solid Body",
                "Select Single Solid Body",
                new[] { Masks.SolidBody_Mask },
                predicate);
        }

        public static Body SelectSingleSheetBody(Predicate<Body> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Sheet Body",
                "Select Single Sheet Body",
                new[] { Masks.SheetBody_Mask },
                predicate);
        }

        public static Body SelectSingleBody(Predicate<Body> predicate = null)
        {
            return SelectSingleTaggedObject(
                "Select Single Body",
                "Select Single Body",
                new[] { Masks.SheetBody_Mask },
                predicate);
        }

        public static Body[] SelectManySolidBodies(Predicate<Body> predicate = null)
        {
            return SelectMultipleTaggedObjects(
                "Select Many Solid Bodies",
                "Select Many Solid Bodies",
                new[] { Masks.SolidBody_Mask },
                predicate);
        }

        public static Body[] SelectManySheetBodies(Predicate<Body> predicate = null)
        {
            return SelectMultipleTaggedObjects(
                "Select Many Sheet Bodies",
                "Select Many Sheet Bodies",
                new[] { Masks.SolidBody_Mask },
                predicate);
        }

        public static Body[] SelectManyBodies(Predicate<Body> predicate = null)
        {
            return SelectMultipleTaggedObjects(
                "Select Many Bodies",
                "Select Many Bodies",
                new[] { Masks.SolidBody_Mask },
                predicate);
        }

        public static TSource SelectSingleTaggedObject<TSource>(
            string message = "Select Single Object",
            string title = "Select Single Object",
            UFUi.Mask[] masks = null,
            Predicate<TSource> pred = null) where TSource : TaggedObject
        {
            int __filter_proc(Tag _object, int[] type, IntPtr user_data, IntPtr select_)
            {
                try
                {
                    if(!(_object.__ToTaggedObject() is TSource source_obj))
                        return UF_UI_SEL_REJECT;

                    if(!pred(source_obj))
                        return UF_UI_SEL_REJECT;

                    return UF_UI_SEL_ACCEPT;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                return UF_UI_SEL_REJECT;
            }

            int __init_proc(IntPtr __select, IntPtr __user_data)
            {
                try
                {
                    ufsession_.Ui.SetSelProcs(__select, __filter_proc, null, __user_data);

                    if(!(masks is null) && masks.Length > 0)
                        ufsession_.Ui.SetSelMask(
                            __select,
                            UFUi.SelMaskAction.SelMaskClearAndEnableSpecific,
                            masks.Length,
                            masks);

                    return UF_UI_SEL_SUCCESS;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                    return UF_UI_SEL_FAILURE;
                }
            }

            using (session_.__UsingLockUiFromCustom())
            {
                var cursor = new double[3];

                ufsession_.Ui.SelectWithSingleDialog(
                    message,
                    title,
                    UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                    __init_proc,
                    IntPtr.Zero,
                    out var __response,
                    out var __object,
                    cursor,
                    out _);

                switch (__response)
                {
                    case UF_UI_BACK:
                    case UF_UI_CANCEL:
                    case UF_UI_OK:
                    case UF_UI_OBJECT_SELECTED:
                    case UF_UI_OBJECT_SELECTED_BY_NAME:
                        break;
                }


                var __obj = __object.__To<TSource>();

                if(__obj is DisplayableObject disp)
                    disp.Unhighlight();

                return __obj;
            }
        }

        public static TSource[] SelectMultipleTaggedObjects<TSource>(
            string message = "Select Multiple Objects",
            string title = "Select Multiple Objects",
            UFUi.Mask[] masks = null,
            Predicate<TSource> pred = null) where TSource : TaggedObject
        {
            int __filter_proc(Tag _object, int[] type, IntPtr user_data, IntPtr select_)
            {
                try
                {
                    if(!(_object.__ToTaggedObject() is TSource source_obj))
                        return UF_UI_SEL_REJECT;

                    if(pred is null)
                        return UF_UI_SEL_ACCEPT;

                    if(!pred(source_obj))
                        return UF_UI_SEL_REJECT;

                    return UF_UI_SEL_ACCEPT;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                return UF_UI_SEL_REJECT;
            }

            int __init_proc(IntPtr __select, IntPtr __user_data)
            {
                try
                {
                    ufsession_.Ui.SetSelProcs(__select, __filter_proc, null, __user_data);

                    if(!(masks is null) && masks.Length > 0)
                        ufsession_.Ui.SetSelMask(
                            __select,
                            UFUi.SelMaskAction.SelMaskClearAndEnableSpecific,
                            masks.Length,
                            masks);

                    return UF_UI_SEL_SUCCESS;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                    return UF_UI_SEL_FAILURE;
                }
            }

            using (session_.__UsingLockUiFromCustom())
            {
                var cursor = new double[3];

                ufsession_.Ui.SelectWithClassDialog(
                    message,
                    title,
                    UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                    __init_proc,
                    IntPtr.Zero,
                    out var __response,
                    out var count,
                    out var objects);

                switch (__response)
                {
                    case UF_UI_CANCEL:
                    case UF_UI_OK:
                        break;
                }

                var objs = objects.Select(__o => __o.__To<TSource>()).ToArray();

                objs.OfType<DisplayableObject>().ToList().ForEach(__o => __o.Unhighlight());

                return objs.ToArray();
            }
        }
    }
}