using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Geom;
using Curve = NXOpen.Curve;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        [Obsolete(nameof(NotImplementedException))]
        public static Section _Mirror(
            this Section section,
            Surface.Plane plane)
        {
            throw new NotImplementedException();
        }

        [Obsolete(nameof(NotImplementedException))]
        public static Section _Mirror(
            this Section section,
            Surface.Plane plane,
            Component from,
            Component to)
        {
            throw new NotImplementedException();
        }

        internal static SelectionIntentRule[] CreateSelectionIntentRule(params ICurve[] icurves)
        {
            var list = new List<SelectionIntentRule>();

            for (var i = 0; i < icurves.Length; i++)
                if(icurves[i] is Curve curve)
                {
                    var curves = new Curve[1] { curve };
                    var item = __work_part_.ScRuleFactory.CreateRuleCurveDumb(curves);
                    list.Add(item);
                }
                else
                {
                    var edges = new Edge[1] { (Edge)icurves[i] };
                    var item2 = __work_part_.ScRuleFactory.CreateRuleEdgeDumb(edges);
                    list.Add(item2);
                }

            return list.ToArray();
        }

        internal static void AddICurve(this Section section, params ICurve[] icurves)
        {
            var nXOpenSection = section;
            nXOpenSection.AllowSelfIntersection(false);

            for (var i = 0; i < icurves.Length; i++)
            {
                var rules = CreateSelectionIntentRule(icurves[i]);

                nXOpenSection.AddToSection(
                    rules,
                    (NXObject)icurves[i],
                    null,
                    null,
                    _Point3dOrigin,
                    Section.Mode.Create,
                    false);
            }
        }

        internal static SelectionIntentRule[] CreateSelectionIntentRule(params Point[] points)
        {
            var array = new Point[points.Length];

            for (var i = 0; i < array.Length; i++)
                array[i] = points[i];

            var curveDumbRule = __work_part_.ScRuleFactory.CreateRuleCurveDumbFromPoints(array);
            return new SelectionIntentRule[1] { curveDumbRule };
        }


        internal static void AddPoints(this Section section, params Point[] points)
        {
            var nXOpenSection = section;
            nXOpenSection.AllowSelfIntersection(false);
            var rules = CreateSelectionIntentRule(points);

            nXOpenSection.AddToSection(
                rules,
                points[0],
                null,
                null,
                _Point3dOrigin,
                Section.Mode.Create,
                false);
        }

        public static void Temp(this Section obj)
        {
            //obj.AddChainBetweenIntersectionPoints
            //obj.
        }
    }
}