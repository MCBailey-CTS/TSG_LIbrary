using System;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Utilities;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.__Extensions_;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace TSG_Library.Ui
{
    internal class Selection
    {
        public static readonly NXOpen.Selection.MaskTriple ComponentMask = CreateMask(UF_component_type, 0, 0);

        public static readonly NXOpen.Selection.MaskTriple SolidBodyMask =
            CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly NXOpen.Selection.MaskTriple SheetBodyMask =
            CreateMask(UF_solid_type, 0, UF_UI_SEL_FEATURE_SHEET_BODY);

        public static readonly NXOpen.Selection.MaskTriple LineMask = CreateMask(UF_line_type, 0, 0);

        public static readonly NXOpen.Selection.MaskTriple FaceMask = CreateMask(UF_solid_type, 0,
            UF_UI_SEL_FEATURE_ANY_FACE);

        public static readonly NXOpen.Selection.MaskTriple SplineMask = CreateMask(UF_spline_type, 0, 0);

        private static NXOpen.Selection.MaskTriple CreateMask(int objectType, int objectSubtype, int solidType)
        {
            return new NXOpen.Selection.MaskTriple
            {
                Type = objectType,
                Subtype = objectSubtype,
                SolidBodySubtype = solidType
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
                out TaggedObject[] objects);

            return objects.Cast<Curve>().ToArray();
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
                out TaggedObject @object,
                out _);

            return (Body)@object;
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
            int FilterProc(Tag @object, int[] type, IntPtr userData, IntPtr select)
            {
                try
                {
                    if (!(@object.__ToTaggedObject() is TSource sourceObj))
                        return UF_UI_SEL_REJECT;

                    return pred != null && !pred(sourceObj) ? UF_UI_SEL_REJECT : UF_UI_SEL_ACCEPT;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                return UF_UI_SEL_REJECT;
            }

            int InitProc(IntPtr select, IntPtr userData)
            {
                try
                {
                    ufsession_.Ui.SetSelProcs(select, FilterProc, null, userData);

                    if (!(masks is null) && masks.Length > 0)
                        ufsession_.Ui.SetSelMask(
                            select,
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
                double[] cursor = new double[3];

                ufsession_.Ui.SelectWithSingleDialog(
                    message,
                    title,
                    UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                    InitProc,
                    IntPtr.Zero,
                    out int response,
                    out Tag @object,
                    cursor,
                    out _);

                switch (response)
                {
                    case UF_UI_BACK:
                    case UF_UI_CANCEL:
                    case UF_UI_OK:
                    case UF_UI_OBJECT_SELECTED:
                    case UF_UI_OBJECT_SELECTED_BY_NAME:
                        break;
                }


                TSource obj = @object.__To<TSource>();

                if (obj is DisplayableObject disp)
                    disp.Unhighlight();

                return obj;
            }
        }

        public static TSource[] SelectMultipleTaggedObjects<TSource>(
            string message = "Select Multiple Objects",
            string title = "Select Multiple Objects",
            UFUi.Mask[] masks = null,
            Predicate<TSource> pred = null) where TSource : TaggedObject
        {
            int FilterProc(Tag @object, int[] type, IntPtr userData, IntPtr select)
            {
                try
                {
                    if (!(@object.__ToTaggedObject() is TSource sourceObj))
                        return UF_UI_SEL_REJECT;

                    if (pred is null)
                        return UF_UI_SEL_ACCEPT;

                    return !pred(sourceObj) ? UF_UI_SEL_REJECT : UF_UI_SEL_ACCEPT;
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }

                return UF_UI_SEL_REJECT;
            }

            int InitProc(IntPtr select, IntPtr userData)
            {
                try
                {
                    ufsession_.Ui.SetSelProcs(select, FilterProc, null, userData);

                    if (!(masks is null) && masks.Length > 0)
                        ufsession_.Ui.SetSelMask(
                            select,
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
                // ReSharper disable once UnusedVariable
                double[] cursor = new double[3];

                ufsession_.Ui.SelectWithClassDialog(
                    message,
                    title,
                    UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                    InitProc,
                    IntPtr.Zero,
                    out int response,
                    // ReSharper disable once UnusedVariable
                    out int count,
                    out Tag[] objects);

                switch (response)
                {
                    case UF_UI_CANCEL:
                    case UF_UI_OK:
                        break;
                }

                TSource[] objs = objects.Select(o => o.__To<TSource>()).ToArray();

                objs.OfType<DisplayableObject>().ToList().ForEach(o => o.Unhighlight());

                return objs.ToArray();
            }
        }
    }
}