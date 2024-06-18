using System;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;
// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedMember.Global

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Section

        [Obsolete(nameof(NotImplementedException))]
        public static Section __Mirror(
            this Section section,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Section __Mirror(
            this Section section,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        //internal static SelectionIntentRule[] CreateSelectionIntentRule(params ICurve[] icurves)
        //{
        //    var list = new List<SelectionIntentRule>();

        //    for (var i = 0; i < icurves.Length; i++)
        //        if (icurves[i] is Curve curve)
        //        {
        //            var curves = new Curve[1] { curve };
        //            var item = __work_part_.ScRuleFactory.CreateRuleCurveDumb(curves);
        //            list.Add(item);
        //        }
        //        else
        //        {
        //            var edges = new Edge[1] { (Edge)icurves[i] };
        //            var item2 = __work_part_.ScRuleFactory.CreateRuleEdgeDumb(edges);
        //            list.Add(item2);
        //        }

        //    return list.ToArray();
        //}

        internal static void __AddICurve(this Section section, params ICurve[] icurves)
        {
            section.AllowSelfIntersection(false);

            for (int i = 0; i < icurves.Length; i++)
            {
                SelectionIntentRule[] rules = section.__OwningPart().__CreateSelectionIntentRule(icurves[i]);

                section.AddToSection(
                    rules,
                    (NXObject)icurves[i],
                    null,
                    null,
                    _Point3dOrigin,
                    Section.Mode.Create,
                    false);
            }
        }

        internal static SelectionIntentRule[] __CreateSelectionIntentRule(params Point[] points)
        {
            Point[] array = new Point[points.Length];

            for (int i = 0; i < array.Length; i++)
                array[i] = points[i];

            CurveDumbRule curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
            return new SelectionIntentRule[1] { curveDumbRule };
        }


        internal static void __AddPoints(this Section section, params Point[] points)
        {
            section.AllowSelfIntersection(false);
            SelectionIntentRule[] rules = __CreateSelectionIntentRule(points);

            section.AddToSection(
                rules,
                points[0],
                null,
                null,
                _Point3dOrigin,
                Section.Mode.Create,
                false);
        }

        public static void __Temp(this Section obj)
        {
            //obj.AddChainBetweenIntersectionPoints
            //obj.
        }

        #endregion
    }
}