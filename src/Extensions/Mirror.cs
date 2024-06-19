using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG_Library.Geom;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        // public static void __Mirror(this NXOpen.CartesianCoordinateSystem obj, Su)


        public static void __Data(this EdgeTangentRule rule, out Edge start, out Edge end, out bool isFromStart, out double angleTolerance, out bool hasSameConvexity)
        {
            rule.GetData(out start, out end, out isFromStart, out angleTolerance, out hasSameConvexity);
        }

        public static EdgeTangentRule __Mirror(this EdgeTangentRule original, Surface.Plane plane, Component from, Component to)
        {
            original.__Data(out var fStart, out var fEnd, out var isFromStart, out var angleTolerance, out var hasSameConvexity);

            var fStartCurve = fStart.__ToCurve();

            CompositeCurve fStartAssemblyCurve = fStartCurve.__CreateLinkedCurve();

            var actualCurve = (NXOpen.Curve)fStartAssemblyCurve.GetEntities()[0];

            fStartAssemblyCurve.RemoveParameters();

            fStartAssemblyCurve.__Delete();

            var tStartAssemblyCurve = actualCurve.__Mirror(plane);

            __work_component_ = to;

            CompositeCurve tCompositeCurve = tStartAssemblyCurve.__CreateLinkedCurve();

            var tActualCurve = (NXOpen.Curve)tCompositeCurve.GetEntities()[0];
            tCompositeCurve.RemoveParameters();
            tCompositeCurve.__Delete();

            var tStartEdge = to.__Prototype().__MatchCurveToEdge(tActualCurve);

        }

        public static Edge __MatchCurveToEdge(this Part part, NXOpen.Curve curve)
        {
            throw new NotImplementedException();
        }

        public static CompositeCurve __CreateLinkedCurve(this NXOpen.Curve curve)
        {
            throw new NotImplementedException();
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;
            NXOpen.Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
            waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullNXOpen_Features_Feature);

            NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
            waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

            NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
            compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

            NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
            waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

            NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
            waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

            NXOpen.Features.WavePointBuilder wavePointBuilder1;
            wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

            NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
            mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

            NXOpen.GeometricUtilities.CurveFitData curveFitData1;
            curveFitData1 = compositeCurveBuilder1.CurveFitData;

            curveFitData1.Tolerance = 0.001;

            curveFitData1.AngleTolerance = 0.5;

            NXOpen.Section section1 = ((NXOpen.Section)workPart.FindObject("ENTITY 113 4"));
            section1.SetAllowRefCrvs(false);

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            waveLinkBuilder1.FixAtCurrentTimestamp = true;

            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

            mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

            compositeCurveBuilder1.Section.DistanceTolerance = 0.001;

            compositeCurveBuilder1.Section.ChainingTolerance = 0.00095;

            compositeCurveBuilder1.Section.AngleTolerance = 0.5;

            compositeCurveBuilder1.Section.DistanceTolerance = 0.001;

            compositeCurveBuilder1.Section.ChainingTolerance = 0.00095;

            compositeCurveBuilder1.Associative = true;

            compositeCurveBuilder1.MakePositionIndependent = false;

            compositeCurveBuilder1.FixAtCurrentTimestamp = true;

            compositeCurveBuilder1.HideOriginal = false;

            compositeCurveBuilder1.InheritDisplayProperties = false;

            compositeCurveBuilder1.JoinOption = NXOpen.Features.CompositeCurveBuilder.JoinMethod.No;

            compositeCurveBuilder1.Tolerance = 0.001;

            NXOpen.Section section2;
            section2 = compositeCurveBuilder1.Section;

            NXOpen.GeometricUtilities.CurveFitData curveFitData2;
            curveFitData2 = compositeCurveBuilder1.CurveFitData;

            section2.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

            NXOpen.IBaseCurve[] curves1 = new NXOpen.IBaseCurve[1];
            NXOpen.Assemblies.Component component1 = ((NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1"));
            NXOpen.Line line1 = ((NXOpen.Line)component1.FindObject("PROTO#.Lines|HANDLE R-20339"));
            curves1[0] = line1;
            NXOpen.CurveDumbRule curveDumbRule1;
            curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);

            section2.AllowSelfIntersection(false);

            NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            rules1[0] = curveDumbRule1;
            NXOpen.NXObject nullNXOpen_NXObject = null;
            NXOpen.Point3d helpPoint1 = new NXOpen.Point3d(-1.2364300741239278, 10.674635720890926, -5.0515147620444623e-15);
            section2.AddToSection(rules1, line1, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

            theSession.DeleteUndoMark(markId3, null);

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

            NXOpen.NXObject nXObject1;
            nXObject1 = waveLinkBuilder1.Commit();

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

            waveLinkBuilder1.Destroy();

        }
    }
}
