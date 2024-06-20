using NXOpen;
using NXOpen.CAE;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.UFuncs.Mirror.Rules;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror
{
    public static class Testing
    {

        public static CompositeCurve LinkCurve(Edge edge)
        {
            WaveLinkBuilder builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);
            builder.FixAtCurrentTimestamp = true;
            builder.CompositeCurveBuilder.Associative = true;
            var edgeTangentRule2 = __work_part_.ScRuleFactory.CreateRuleEdgeTangent(edge, null, true, 0.5, false, false);
            SelectionIntentRule[] rules2 = new SelectionIntentRule[] { edgeTangentRule2 };
            builder.CompositeCurveBuilder.Section.AddToSection(rules2, edge, null, null, _Point3dOrigin, NXOpen.Section.Mode.Create, false);
            NXObject nXObject2;
            return (CompositeCurve)builder.Commit();
        }


        public static void Mirror()
        {
            try
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Mirror");

                var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
                var toComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("902"));

                IDictionary<TaggedObject, TaggedObject> dict = new Dictionary<TaggedObject, TaggedObject>
                {
                    { frComp.__Prototype().Features.ToArray()[0], toComp.__Prototype().Features.ToArray()[0] }
                };

                // chamfer
                var frChamfer = (ApexRangeChamfer)frComp.__Prototype().Features.ToArray()[1];

                ApexRangeChamferBuilder chamferBuilder = frComp.__Prototype().Features.DetailFeatureCollection.CreateApexRangeChamferBuilder((ApexRangeChamfer)frChamfer);

                TaggedObject[] objects;

                using (new Destroyer(chamferBuilder))
                    objects = chamferBuilder.EdgeManager.EdgeChainSetList
                        .GetContents()
                        .SelectMany(k => k.Edges.GetObjects())
                        .ToArray();

                var edge = (Edge)objects[0];

                frChamfer.Suppress();

                __work_component_ = null;

                var linked_curve = LinkCurve((Edge)frComp.FindOccurrence(edge));

                var entity = (NXOpen.Curve)linked_curve.GetEntities()[0];

                entity

                linked_curve.RemoveParameters();

                ufsession_.Modl.Update();

                //{
                //    NXOpen.Session theSession = NXOpen.Session.GetSession();
                //    NXOpen.Part workPart = theSession.Parts.Work;
                //    NXOpen.Part displayPart = theSession.Parts.Display;
                //    // ----------------------------------------------
                //    //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
                //    // ----------------------------------------------
                //    NXOpen.Session.UndoMarkId markId1;
                //    markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                //    NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
                //    NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
                //    waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullNXOpen_Features_Feature);

                //    NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
                //    waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

                //    NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
                //    compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

                //    NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
                //    waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

                //    NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
                //    waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

                //    NXOpen.Features.WavePointBuilder wavePointBuilder1;
                //    wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

                //    NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
                //    extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

                //    NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
                //    mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

                //    NXOpen.GeometricUtilities.CurveFitData curveFitData1;
                //    curveFitData1 = compositeCurveBuilder1.CurveFitData;

                //    curveFitData1.Tolerance = 0.001;

                //    curveFitData1.AngleTolerance = 0.5;

                //    NXOpen.Section section1 = ((NXOpen.Section)workPart.FindObject("ENTITY 113 4"));
                //    section1.SetAllowRefCrvs(false);

                //    extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

                //    extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

                //    extractFaceBuilder1.AngleTolerance = 45.0;

                //    waveLinkBuilder1.FixAtCurrentTimestamp = true;

                //    waveDatumBuilder1.DisplayScale = 2.0;

                //    extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

                //    mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

                //    theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

                //    compositeCurveBuilder1.Section.DistanceTolerance = 0.001;

                //    compositeCurveBuilder1.Section.ChainingTolerance = 0.00095;

                //    compositeCurveBuilder1.Section.AngleTolerance = 0.5;

                //    compositeCurveBuilder1.Section.DistanceTolerance = 0.001;

                //    compositeCurveBuilder1.Section.ChainingTolerance = 0.00095;

                //    compositeCurveBuilder1.Associative = true;

                //    compositeCurveBuilder1.MakePositionIndependent = false;

                //    compositeCurveBuilder1.FixAtCurrentTimestamp = true;

                //    compositeCurveBuilder1.HideOriginal = false;

                //    compositeCurveBuilder1.InheritDisplayProperties = false;

                //    compositeCurveBuilder1.JoinOption = NXOpen.Features.CompositeCurveBuilder.JoinMethod.No;

                //    compositeCurveBuilder1.Tolerance = 0.001;

                //    NXOpen.Section section2;
                //    section2 = compositeCurveBuilder1.Section;

                //    NXOpen.GeometricUtilities.CurveFitData curveFitData2;
                //    curveFitData2 = compositeCurveBuilder1.CurveFitData;

                //    section2.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.CurvesAndPoints);

                //    NXOpen.Session.UndoMarkId markId2;
                //    markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

                //    NXOpen.Session.UndoMarkId markId3;
                //    markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

                //    NXOpen.Assemblies.Component component1 = ((NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1"));
                //    NXOpen.Edge edge1 = ((NXOpen.Edge)component1.FindObject("PROTO#.Features|BLOCK(0)|EDGE * 1 * 3 {(0,0,0)(0,4.25,0)(0,8.5,0) BLOCK(0)}"));
                //    NXOpen.Edge nullNXOpen_Edge = null;
                //    NXOpen.EdgeTangentRule edgeTangentRule1;
                //    edgeTangentRule1 = workPart.ScRuleFactory.CreateRuleEdgeTangent(edge1, nullNXOpen_Edge, true, 0.5, false, false);

                //    section2.AllowSelfIntersection(false);

                //    NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
                //    rules1[0] = edgeTangentRule1;
                //    NXOpen.NXObject nullNXOpen_NXObject = null;
                //    NXOpen.Point3d helpPoint1 = new NXOpen.Point3d(0.0, 5.3019146888059625, 0.0);
                //    section2.AddToSection(rules1, edge1, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

                //    theSession.DeleteUndoMark(markId3, null);

                //    theSession.DeleteUndoMark(markId2, null);

                //    NXOpen.Session.UndoMarkId markId4;
                //    markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

                //    theSession.DeleteUndoMark(markId4, null);

                //    NXOpen.Session.UndoMarkId markId5;
                //    markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

                //    NXOpen.NXObject nXObject1;
                //    nXObject1 = waveLinkBuilder1.Commit();

                //    theSession.DeleteUndoMark(markId5, null);

                //    theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

                //    waveLinkBuilder1.Destroy();

                //    // ----------------------------------------------
                //    //   Menu: Tools->Journal->Stop Recording
                //    // ----------------------------------------------

                //}


                return;





                //var cd = edge2.__ToCurve();


                return;


                foreach (TaggedObject obj in objects)
                    switch (obj)
                    {
                        case Edge frEdge:
                            {
                                Session theSession = NXOpen.Session.GetSession();
                                //Part workPart = theSession.Parts.Work;
                                Part displayPart = theSession.Parts.Display;
                                NXOpen.Assemblies.Component component1 = frComp;
                                __work_component_ = component1;
                                NXOpen.Assemblies.Component component2 = toComp;
                                __work_component_ = component2;
                                //workPart = __work_component_.__Prototype();
                                NXObject[] features1 = new NXObject[1];
                                ApexRangeChamfer apexRangeChamfer1 = frChamfer;
                                features1[0] = apexRangeChamfer1;
                                CopyPasteBuilder copyPasteBuilder1;
                                copyPasteBuilder1 = __work_part_.Features.CreateCopyPasteBuilder2(features1);

                                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                                {
                                    copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)(7));
                                    FeatureReferencesBuilder featureReferencesBuilder1;
                                    featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();

                                    MatchedReferenceBuilder[] matchedReferenceData1;
                                    matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();

                                    copyPasteBuilder1.ExpressionOption = NXOpen.Features.CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                                    matchedReferenceData1[0].MatchedStatus = NXOpen.Features.MatchedReferenceBuilder.ResolvedStatus.BySystem;
                                    featureReferencesBuilder1.AutomaticMatch(false);
                                    copyPasteBuilder1.SelectOption = NXOpen.Features.CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;

                                    copyPasteBuilder1.UpdateBuilder();
                                    MatchedReferenceBuilder[] matchedReferenceData2;
                                    matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                                    copyPasteBuilder1.ExpressionOption = NXOpen.Features.CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                                    ScCollector scCollector1;
                                    scCollector1 = __work_part_.ScCollectors.CreateCollector();
                                    scCollector1.SetAllowRefCurves(false);
                                    Block block1 = ((Block)__work_part_.Features.FindObject("BLOCK(0)"));
                                    Edge edge1 = ((Edge)block1.FindObject("EDGE * 1 * 6 {(3.75,-8.502,-0)(1.875,-8.502,-0)(-0,-8.502,0) BLOCK(0)}"));
                                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                                    rules1[0] = __work_part_.ScRuleFactory.CreateRuleEdgeDumb(new[] { edge1 });
                                    scCollector1.ReplaceRules(rules1, false);
                                    matchedReferenceData2[0].MatchedEntity = scCollector1;
                                    matchedReferenceData2[0].MatchedStatus = NXOpen.Features.MatchedReferenceBuilder.ResolvedStatus.ByUser;
                                    featureReferencesBuilder1.AutomaticMatch(false);
                                    NXObject nXObject1;
                                    nXObject1 = copyPasteBuilder1.Commit();

                                }
                            }
                            continue;
                        case Face frFace:
                            continue;
                        case Curve frCurve:
                            continue;
                        default:
                            print_($"Unknown object: {obj.GetType().Name}");
                            continue;

                    }

                foreach (var t in objects)
                    print_(t.GetType().Name);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }




        }
    }
}