﻿using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Features;
using TSG_Library.UFuncs.MirrorComponents.Features;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror
{
    public class Program
    {
        public static void Mirror208_3001(Surface.Plane plane, Component frComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component toComp = (Component)dict[frComp];
            frComp.__ReferenceSet("Entire Part");
            toComp.__ReferenceSet("Entire Part");

            try
            {
                IMirrorFeature[] source = new IMirrorFeature[]
                {
                    //new MirrorLinkedBody(),
                    //new MirrorEdgeBlend(),
                    new MirrorChamfer(),
                    //new MirrorSubtract(),
                    //new MirrorIntersect(),
                    //new MirrorUnite(),
                    new MirrorBlock(),
                    //new MirrorExtractedBody(),
                    //new MirrorExtrude(),
                    //new MirrorOffsetFace(),
                };
                Part part = frComp.__Prototype();
                Part part2 = toComp.__Prototype();
                frComp.__Prototype().Features.SuppressFeatures(frComp.__Prototype().Features.GetFeatures());
                toComp.__Prototype().Features.SuppressFeatures(toComp.__Prototype().Features.GetFeatures());
                Body[] array = part.Bodies.ToArray();
                Body[] array2 = part2.Bodies.ToArray();
                for (int i = 0; i < array.Length; i++) dict.Add(array[i], array2[i]);

                foreach (Feature originalFeature in part.Features)
                    try
                    {
                        print_($"{originalFeature.GetFeatureName()} - {originalFeature.FeatureType}");

                        IMirrorFeature mirrorFeature =
                            source.SingleOrDefault(mirror => mirror.FeatureType == originalFeature.FeatureType);
                        if (mirrorFeature != null)
                        {
                            mirrorFeature.Mirror(originalFeature, dict, plane, frComp);
                            continue;
                        }

                        print_("///////////////////////////");
                        print_("Unable to mirror:");
                        print_("Type: " + originalFeature.FeatureType);
                        print_("Name: " + originalFeature.GetFeatureName());
                        print_("Part: " + originalFeature.OwningPart.Leaf);
                        print_("///////////////////////////");
                    }
                    catch (MirrorException ex)
                    {
                        print_("////////////////////////////////////////////////////");
                        print_(part.Leaf);
                        print_("Unable to mirror " + originalFeature.GetFeatureName());
                        print_(ex.Message);
                        print_("////////////////////////////////////////////////////");
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                //__work_part_ = __display_part_;
                //foreach (Part item in __display_part_.__DescendantParts())
                //    foreach (Expression expression3 in item.Expressions)
                //    {
                //        if (expression3.Status != Expression.StatusOption.Broken) continue;

                //        Expression[] array3 = expression3.__OwningPart().Expressions.ToArray();
                //        foreach (Expression expression2 in array3)
                //            if (expression2.Tag != expression3.Tag && expression3.Name == expression2.RightHandSide)
                //                item.Expressions.Delete(expression2);

                //        item.Expressions.Delete(expression3);
                //        print_("Deleted broken expression \"" + expression3.Name + "\" in part \"" +
                //               expression3.__OwningPart().Leaf + "\"");
                //    }
            }
            catch (Exception ex2)
            {
                ex2.__PrintException();
            }
            finally
            {
                Session.UndoMarkId undoMarkId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                session_.UpdateManager.DoInterpartUpdate(undoMarkId);
                session_.UpdateManager.DoAssemblyConstraintsUpdate(undoMarkId);
            }
        }


        public static void MirrorStatic()
        {
            print_("in here");
            //var plane = new Surface.Plane(_Point3dOrigin, __Vector3dY());
            var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
            frComp.__SetWcsToComponent();
            //var toOrigin = frComp.__Origin().__Mirror(plane);
            //var toOrientation = frComp.__Orientation().__Mirror(plane);
            //string toFilePath = "H:\\CTS\\001449 (mirror)\\001449-010\\001449-010-900.prt";
            //var toPart = session_.__New(toFilePath, frComp.__Prototype().PartUnits);
            //var toComp = __display_part_.__AddComponent(toPart, origin: toOrigin, orientation: toOrientation);


            IDictionary<TaggedObject, TaggedObject> mirrorDict = new Dictionary<TaggedObject, TaggedObject>();

            foreach (Feature oriFeat in frComp.__Prototype().Features)
                using (session_.__UsingDisplayPartReset())
                {
                    print_("/////////////////////////////////");
                    print_(oriFeat.GetFeatureName());
                    __work_part_ = oriFeat.__OwningPart();

                    switch (oriFeat.FeatureType)
                    {
                        case "BLOCK":
                            //var toBlock = MirrorBlock(oriFeat, frComp, toComp);
                            //mirrorDict.Add(oriFeat, toBlock);
                            break;
                        case "EXTRACT_BODY":
                            {
                                var builder = __work_part_.Features.CreateExtractFaceBuilder(oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    var collector = builder.ExtractBodyCollector;

                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }
                            break;

                        case "LINKED_BODY":
                            {
                                var builder = __work_part_.Features.CreateExtractFaceBuilder(oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    var collector = builder.ExtractBodyCollector;

                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;
                        case "CHAMFER":
                            {
                                var builder = __work_part_.Features.DetailFeatureCollection.CreateApexRangeChamferBuilder((ApexRangeChamfer)oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    var collector = builder.EdgeManager.EdgeChainSetList.GetContents()[0].Edges;


                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;

                        case "BLEND":
                            {
                                var builder = __work_part_.Features.CreateEdgeBlendBuilder(oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    builder.GetChainset(0, out var collector, out var rad);


                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;

                        case "OFFSET":
                            {
                                var builder = __work_part_.Features.CreateOffsetFaceBuilder(oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    var collector = builder.FaceCollector;


                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;

                        case "SUBTRACT":
                            {
                                var builder = __work_part_.Features.CreateBooleanBuilderUsingCollector((BooleanFeature)oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    var collector = builder.ToolBodyCollector;


                                    collector.GetRules(out var rules);

                                    foreach (var rule in rules)
                                        print_(rule.Type);
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;

                        case "EXTRUDE":
                            {
                                var builder = __work_part_.Features.CreateExtrudeBuilder(oriFeat);

                                using (session_.__UsingBuilderDestroyer(builder))
                                {
                                    builder.Section.GetSectionData(out var data);

                                    print_(data.Length);

                                    foreach (var d in data)
                                    {
                                        d.GetRules(out var rules);

                                        foreach (var rule in rules)
                                            print_(rule.Type);
                                    }
                                }
                            }

                            //ExtractFace toExtractBody = MirrorExtractBody(oriFeat, frComp, toComp, mirrorDict);
                            //mirrorDict.Add(oriFeat, toExtractBody);
                            break;

                    }
                }


        }

        private static ExtractFace MirrorExtractBody(Feature oriFeat, Component frComp, Component toComp, IDictionary<TaggedObject, TaggedObject> mirrorDict)
        {
            __work_component_ = frComp;
            CopyPasteBuilder copyPasteBuilder1 = __work_part_.Features.CreateCopyPasteBuilder2(new[] { oriFeat });

            using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
            {
                __work_component_ = toComp;
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                featureReferencesBuilder1.AutomaticMatch(true);
                MatchedReferenceBuilder[] matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                var parent = oriFeat.GetParents()[0];
                ScCollector scCollector1 = __work_part_.ScCollectors.CreateCollector();
                Feature[] features2 = new Feature[1];
                features2[0] = (Feature)mirrorDict[parent];
                BodyFeatureRule bodyFeatureRule1 = __work_part_.ScRuleFactory.CreateRuleBodyFeature(features2, true, toComp);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyFeatureRule1;
                scCollector1.ReplaceRules(rules1, false);
                matchedReferenceData1[0].MatchedEntity = scCollector1;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                NXObject nXObject1 = copyPasteBuilder1.Commit();
                return (ExtractFace)nXObject1;
            }
        }

        public static Block MirrorBlock(Feature oriFeat, Component frComp, Component toComp)
        {
            __work_component_ = toComp;
            CopyPasteBuilder copyPasteBuilder1 = __work_part_.Features.CreateCopyPasteBuilder2(new[] { oriFeat });
            Block toBlock;

            using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
            {
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                MatchedReferenceBuilder[] matchedReferenceData1 = copyPasteBuilder1.GetFeatureReferences().GetMatchedReferences();
                toBlock = (Block)copyPasteBuilder1.Commit();
            }

            Session.UndoMarkId markId6 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            EditWithRollbackManager editWithRollbackManager1 = __work_part_.Features.StartEditWithRollbackManager(toBlock, markId6);

            try
            {
                BlockFeatureBuilder blockFeatureBuilder1 = __work_part_.Features.CreateBlockFeatureBuilder(toBlock);

                using (session_.__UsingBuilderDestroyer(blockFeatureBuilder1))
                {
                    var new_width = blockFeatureBuilder1.Length.GetFormula();
                    var new_length = blockFeatureBuilder1.Width.GetFormula();
                    var height = blockFeatureBuilder1.Height.GetFormula();
                    var origin = blockFeatureBuilder1.Origin;
                    blockFeatureBuilder1.SetOriginAndLengths(origin, new_length, new_width, height);
                    blockFeatureBuilder1.CommitFeature();
                }
            }
            finally
            {
                editWithRollbackManager1.UpdateFeature(false);
                editWithRollbackManager1.Stop();
                editWithRollbackManager1.Destroy();
            }

            return toBlock;
        }




        public static void MirrorStatic1()
        {
            var plane = new Surface.Plane(_Point3dOrigin, __Vector3dY());
            var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
            frComp.__SetWcsToComponent();
            var toOrigin = frComp.__Origin().__Mirror(plane);
            var toOrientation = frComp.__Orientation().__Mirror(plane);
            string toFilePath = "H:\\CTS\\001449 (mirror)\\001449-010\\001449-010-900.prt";
            var toPart = session_.__New(toFilePath, frComp.__Prototype().PartUnits);
            var toComp = __display_part_.__AddComponent(toPart, origin: toOrigin, orientation: toOrientation);


            IDictionary<TaggedObject, TaggedObject> mirrorDict = new Dictionary<TaggedObject, TaggedObject>();




            __work_part_ = __display_part_;

            // mirrors block
            //{
            //    __work_component_ = toComp;
            //    NXObject[] features1 = new NXObject[1];
            //    Part part1 = frComp.__Prototype();
            //    Block block1 = (Block)part1.Features.FindObject("BLOCK(0)");
            //    features1[0] = block1;
            //    CopyPasteBuilder copyPasteBuilder1 = __work_part_.Features.CreateCopyPasteBuilder2(features1);
            //    Block block2;

            //    using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
            //    {
            //        copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
            //        MatchedReferenceBuilder[] matchedReferenceData1 = copyPasteBuilder1.GetFeatureReferences().GetMatchedReferences();
            //        NXObject nXObject1 = copyPasteBuilder1.Commit();
            //        block2 = (Block)nXObject1;
            //    }

            //    Session.UndoMarkId markId6 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
            //    EditWithRollbackManager editWithRollbackManager1 = __work_part_.Features.StartEditWithRollbackManager(block2, markId6);

            //    try
            //    {
            //        BlockFeatureBuilder blockFeatureBuilder1 = __work_part_.Features.CreateBlockFeatureBuilder(block2);

            //        using (session_.__UsingBuilderDestroyer(blockFeatureBuilder1))
            //        {
            //            var new_width = blockFeatureBuilder1.Length.GetFormula();
            //            var new_length = blockFeatureBuilder1.Width.GetFormula();
            //            var height = blockFeatureBuilder1.Height.GetFormula();
            //            var origin = blockFeatureBuilder1.Origin;
            //            blockFeatureBuilder1.SetOriginAndLengths(origin, new_length, new_width, height);
            //            blockFeatureBuilder1.CommitFeature();
            //        }
            //    }
            //    finally
            //    {
            //        editWithRollbackManager1.UpdateFeature(false);
            //        editWithRollbackManager1.Stop();
            //        editWithRollbackManager1.Destroy();
            //    }
            //}

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component component1 = frComp;
                __work_component_ = component1;
                workPart = theSession.Parts.Work; // 001449-010-109
                Component component2 = toComp;
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("EXTRACT_BODY(1)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                ExtractFace extractFace2;
                Block block1;

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                {
                    copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                    featureReferencesBuilder1.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                    copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                    ScCollector scCollector1 = workPart.ScCollectors.CreateCollector();
                    Feature[] features2 = new Feature[1];
                    block1 = (Block)workPart.Features.FindObject("BLOCK(0)");
                    features2[0] = block1;
                    BodyFeatureRule bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features2, true, component2);
                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                    rules1[0] = bodyFeatureRule1;
                    scCollector1.ReplaceRules(rules1, false);
                    matchedReferenceData1[0].MatchedEntity = scCollector1;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                    featureReferencesBuilder1.AutomaticMatch(false);
                    NXObject nXObject1 = copyPasteBuilder1.Commit();
                    extractFace2 = (ExtractFace)nXObject1;
                }

                __work_component_ = component1;
                workPart = theSession.Parts.Work; // 001449-010-109
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features3 = new NXObject[1];
                ApexRangeChamfer apexRangeChamfer1 = (ApexRangeChamfer)part1.Features.FindObject("CHAMFER(60)");
                features3[0] = apexRangeChamfer1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features3);

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder2))
                {
                    copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder2;
                    featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                    featureReferencesBuilder2.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData2;
                    matchedReferenceData2 = featureReferencesBuilder2.GetMatchedReferences();
                    copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial;
                    copyPasteBuilder2.CopyResolveGeometry = true;
                    copyPasteBuilder2.Associative = true;
                    ScCollector scCollector2;
                    scCollector2 = workPart.ScCollectors.CreateCollector();
                    scCollector2.SetAllowRefCurves(false);
                    Edge edge1 = (Edge)block1.FindObject("EDGE * 1 * 6 {(8.5,3.75,0)(8.5,1.875,0)(8.5,0,0) BLOCK(0)}");
                    Edge nullNXOpen_Edge = null;
                    EdgeTangentRule edgeTangentRule1;
                    edgeTangentRule1 = workPart.ScRuleFactory.CreateRuleEdgeTangent(edge1, nullNXOpen_Edge, false, 0.5, false, false);
                    SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                    rules2[0] = edgeTangentRule1;
                    scCollector2.ReplaceRules(rules2, false);
                    matchedReferenceData2[0].MatchedEntity = scCollector2;
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                    featureReferencesBuilder2.AutomaticMatch(false);
                    copyPasteBuilder2.CopyResolveGeometry = false;
                    copyPasteBuilder2.Associative = false;
                    NXObject nXObject2;
                    nXObject2 = copyPasteBuilder2.Commit();
                    ApexRangeChamfer apexRangeChamfer2 = (ApexRangeChamfer)nXObject2;
                    Expression[] expressions2;
                    expressions2 = apexRangeChamfer2.GetExpressions();
                }
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component component1 = frComp;
                __work_component_ = component1;
                workPart = theSession.Parts.Work; // 001449-010-109
                Component[] components1 = new Component[1];
                components1[0] = component1;
                component1.__ReferenceSet("Entire Part");
                Component component2 = toComp;
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                OffsetFace offsetFace1 = (OffsetFace)part1.Features.FindObject("OFFSET(61)");
                features1[0] = offsetFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                {
                    copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder1;
                    featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                    featureReferencesBuilder1.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData1;
                    matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                    copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial;
                    copyPasteBuilder1.CopyResolveGeometry = true;
                    copyPasteBuilder1.Associative = true;
                    ScCollector scCollector1;
                    scCollector1 = workPart.ScCollectors.CreateCollector();
                    ExtractFace extractFace1 = (ExtractFace)workPart.Features.FindObject("EXTRACT_BODY(1)");
                    Face face1 = (Face)extractFace1.FindObject("FACE 12 {(8.5,1.875,-1) EXTRACT_BODY(1)}");
                    Face[] boundaryFaces1 = new Face[0];
                    FaceTangentRule faceTangentRule1;
                    faceTangentRule1 = workPart.ScRuleFactory.CreateRuleFaceTangent(face1, boundaryFaces1, 0.5);
                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                    rules1[0] = faceTangentRule1;
                    scCollector1.ReplaceRules(rules1, false);
                    matchedReferenceData1[0].MatchedEntity = scCollector1;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                    featureReferencesBuilder1.AutomaticMatch(false);
                    copyPasteBuilder1.CopyResolveGeometry = false;
                    copyPasteBuilder1.Associative = false;
                    NXObject nXObject1;
                    nXObject1 = copyPasteBuilder1.Commit();
                    OffsetFace offsetFace2 = (OffsetFace)nXObject1;
                }
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component2 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Show");
                DisplayableObject[] objects1 = new DisplayableObject[1];
                Body body1 = (Body)component2.FindObject("PROTO#.Bodies|BLOCK(0)");
                objects1[0] = body1;
                theSession.DisplayManager.ShowObjects(objects1, DisplayManager.LayerSetting.ChangeLayerToSelectable);
                displayPart.ModelingViews.WorkView.FitAfterShowOrHide(View.ShowOrHideType.ShowOnly);
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Hide");
                DisplayableObject[] objects2 = new DisplayableObject[1];
                Body body2 = (Body)component2.FindObject("PROTO#.Bodies|EXTRACT_BODY(1)");
                objects2[0] = body2;
                theSession.DisplayManager.BlankObjects(objects2);
                displayPart.ModelingViews.WorkView.FitAfterShowOrHide(View.ShowOrHideType.HideOnly);
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                EdgeBlend edgeBlend1 = (EdgeBlend)part1.Features.FindObject("BLEND(62)");
                features1[0] = edgeBlend1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                featureReferencesBuilder1.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId5, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial;
                copyPasteBuilder1.CopyResolveGeometry = true;
                copyPasteBuilder1.Associative = true;
                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                scCollector1.SetAllowRefCurves(false);
                Edge[] seedEdges1 = new Edge[1];
                Block block1 = (Block)workPart.Features.FindObject("BLOCK(0)");
                Edge edge1 = (Edge)block1.FindObject("EDGE * 2 * 5 {(0,0,-2)(4.25,0,-2)(8.5,0,-2) BLOCK(0)}");
                seedEdges1[0] = edge1;
                EdgeMultipleSeedTangentRule edgeMultipleSeedTangentRule1;
                edgeMultipleSeedTangentRule1 = workPart.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(seedEdges1, 0.5, false);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = edgeMultipleSeedTangentRule1;
                scCollector1.ReplaceRules(rules1, false);
                scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.LaminarEdge);
                matchedReferenceData1[0].MatchedEntity = scCollector1;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.CopyResolveGeometry = false;
                copyPasteBuilder1.Associative = false;
                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                EdgeBlend edgeBlend2 = (EdgeBlend)nXObject1;
                Expression[] expressions1;
                expressions1 = edgeBlend2.GetExpressions();
                copyPasteBuilder1.Destroy();

            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component component1 = frComp;
                __work_component_ = component1;
                workPart = theSession.Parts.Work; // 001449-010-109
                Component component2 = toComp;
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-900
                Component component3 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-strip 1");
                Component[] components1 = new Component[1];
                components1[0] = component3;
                component3.UpdateStructure(components1, 2, true);
                Component[] components2 = new Component[1];
                Component component4 = (Component)component3.FindObject("COMPONENT 001449-010-layout 2");
                components2[0] = component4;
                ErrorList errorList1;
                errorList1 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("OP-020-LWR-3D", components2);
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(63)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                {
                    copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder1;
                    featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                    featureReferencesBuilder1.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData1;
                    matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                    copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                    ScCollector scCollector1;
                    scCollector1 = workPart.ScCollectors.CreateCollector();
                    scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                    scCollector1.SetInterpart(true);
                    Body[] bodies1 = new Body[1];
                    Body body1 = (Body)component4.FindObject("PROTO#.Bodies|EXTRUDE(32)");
                    bodies1[0] = body1;
                    BodyDumbRule bodyDumbRule1;
                    bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                    rules1[0] = bodyDumbRule1;
                    scCollector1.ReplaceRules(rules1, false);
                    matchedReferenceData1[0].MatchedEntity = scCollector1;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                    featureReferencesBuilder1.AutomaticMatch(false);
                    NXObject nXObject1;
                    nXObject1 = copyPasteBuilder1.Commit();
                    ExtractFace extractFace2 = (ExtractFace)nXObject1;
                    Expression[] expressions1;
                    expressions1 = extractFace2.GetExpressions();
                }

                __work_component_ = component1;
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(64)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);
                copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder2;
                featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                featureReferencesBuilder2.AutomaticMatch(true);
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder2.GetMatchedReferences();
                copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                copyPasteBuilder2.CopyResolveGeometry = true;
                copyPasteBuilder2.Associative = true;
                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();
                Body[] bodies2 = new Body[1];
                Body body2 = (Body)workPart.Bodies.FindObject("LINKED_BODY(5)");
                bodies2[0] = body2;
                BodyDumbRule bodyDumbRule2;
                bodyDumbRule2 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies2, true);
                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = bodyDumbRule2;
                scCollector2.ReplaceRules(rules2, false);
                matchedReferenceData2[0].MatchedEntity = scCollector2; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                Body body3 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                matchedReferenceData2[1].MatchedEntity = body3;
                matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                matchedReferenceData2[1].ReverseDirection = false; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.CopyResolveGeometry = false;
                copyPasteBuilder2.Associative = false;
                NXObject nXObject2;
                nXObject2 = copyPasteBuilder2.Commit();
                BooleanFeature booleanFeature2 = (BooleanFeature)nXObject2;
                Expression[] expressions2;
                expressions2 = booleanFeature2.GetExpressions();
                copyPasteBuilder2.Destroy();
                Component nullNXOpen_Assemblies_Component = null;
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                Component[] components3 = new Component[1];
                components3[0] = component4;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("OP-020-BODY", components3);
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Change Displayed Part");
                Part part1 = (Part)theSession.Parts.FindObject("001449-010-900");
                PartLoadStatus partLoadStatus1;
                PartCollection.SdpsStatus status1;
                status1 = theSession.Parts.SetActiveDisplay(part1, DisplayPartOption.AllowAdditional, PartDisplayPartWorkPartOption.UseLast, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-900
                displayPart = theSession.Parts.Display; // 001449-010-900
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                ExtractFace extractFace1 = (ExtractFace)workPart.Features.FindObject("LINKED_BODY(5)");
                EditWithRollbackManager editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(extractFace1, markId2);

                try
                {

                    Session.UndoMarkId markId3;
                    markId3 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                    ExtractFaceBuilder extractFaceBuilder1;
                    extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(extractFace1);
                    DisplayableObject[] replacementobjects1 = new DisplayableObject[1];
                    Part part2 = (Part)theSession.Parts.FindObject("001449-010-layout");
                    Body body1 = (Body)part2.Bodies.FindObject("EXTRUDE(32)");
                    replacementobjects1[0] = body1;
                    extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                    theSession.SetUndoMarkName(markId3, "WAVE Geometry Linker Dialog");
                    extractFaceBuilder1.InheritDisplayProperties = true;
                    TaggedObject nullNXOpen_TaggedObject = null;
                    extractFaceBuilder1.SourcePartOccurrence = nullNXOpen_TaggedObject;
                    NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                    extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                    NXObject nXObject1;
                    nXObject1 = extractFaceBuilder1.Commit();
                    extractFaceBuilder1.Destroy();
                }
                finally
                {
                    editWithRollbackManager1.UpdateFeature(false);
                    editWithRollbackManager1.Stop();
                    theSession.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager1.Destroy();
                }

                workPart.Undisplay();
                Part part3 = (Part)theSession.Parts.FindObject("001449-010-lsp1");
                PartLoadStatus partLoadStatus2;
                PartCollection.SdpsStatus status2;
                status2 = theSession.Parts.SetActiveDisplay(part3, DisplayPartOption.AllowAdditional, PartDisplayPartWorkPartOption.UseLast, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                displayPart = theSession.Parts.Display; // 001449-010-lsp1
                partLoadStatus2.Dispose();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Copy");
                NXOpen.Gateway.CopyCutBuilder copyCutBuilder1;
                copyCutBuilder1 = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();
                copyCutBuilder1.CanCopyAsSketch = true;
                copyCutBuilder1.IsCut = false;
                copyCutBuilder1.ToClipboard = true;
                copyCutBuilder1.DestinationFilename = null;
                NXObject[] objects1 = new NXObject[1];
                Arc arc1 = (Arc)component1.FindObject("PROTO#.Arcs|HANDLE R-20341");
                objects1[0] = arc1;
                copyCutBuilder1.SetObjects(objects1);
                NXObject nXObject1;
                nXObject1 = copyCutBuilder1.Commit();
                copyCutBuilder1.Destroy();
                Component component2 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                NXOpen.Gateway.PasteBuilder pasteBuilder1;
                pasteBuilder1 = workPart.ClipboardOperationsManager.CreatePasteBuilder();
                NXObject nXObject2;
                nXObject2 = pasteBuilder1.Commit();
                pasteBuilder1.Destroy();
                PartLoadStatus partLoadStatus3;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus3);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus3.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Copy");
                NXOpen.Gateway.CopyCutBuilder copyCutBuilder2;
                copyCutBuilder2 = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();

                using (session_.__UsingBuilderDestroyer(copyCutBuilder2))
                {
                    copyCutBuilder2.CanCopyAsSketch = true;
                    copyCutBuilder2.IsCut = false;
                    copyCutBuilder2.ToClipboard = true;
                    copyCutBuilder2.DestinationFilename = null;
                    NXObject[] objects2 = new NXObject[1];
                    Line line1 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20336");
                    objects2[0] = line1;
                    copyCutBuilder2.SetObjects(objects2);
                    NXObject nXObject3;
                    nXObject3 = copyCutBuilder2.Commit();
                }

                PartLoadStatus partLoadStatus4;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus4);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus4.Dispose();
                NXOpen.Gateway.PasteBuilder pasteBuilder2;
                pasteBuilder2 = workPart.ClipboardOperationsManager.CreatePasteBuilder();

                using (session_.__UsingBuilderDestroyer(pasteBuilder2))
                {
                    NXObject nXObject4;
                    nXObject4 = pasteBuilder2.Commit();

                }

                Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component nullNXOpen_Assemblies_Component = null;
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                partLoadStatus5.Dispose();
                theSession.SetUndoMarkName(markId9, "Make Work Part");
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder1;
                addComponentBuilder1 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner1;
                componentPositioner1 = workPart.ComponentAssembly.Positioner;
                componentPositioner1.ClearNetwork();
                Arrangement arrangement1 = (Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
                componentPositioner1.PrimaryArrangement = arrangement1;
                componentPositioner1.BeginAssemblyConstraints();
                bool allowInterpartPositioning1;
                allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network1;
                network1 = componentPositioner1.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
                componentNetwork1.MoveObjectsState = true;
                Component nullNXOpen_Assemblies_Component = null;
                componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId1, "Add Component Dialog");
                componentNetwork1.MoveObjectsState = true;
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                NXOpen.Assemblies.ProductInterface.InterfaceObject nullNXOpen_Assemblies_ProductInterface_InterfaceObject = null;
                addComponentBuilder1.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                addComponentBuilder1.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                addComponentBuilder1.SetCount(1);
                addComponentBuilder1.SetScatterOption(false);
                addComponentBuilder1.ReferenceSet = "Unknown";
                addComponentBuilder1.Layer = 0;
                addComponentBuilder1.ReferenceSet = "Entire Part";
                addComponentBuilder1.Layer = 0;
                BasePart[] partstouse1 = new BasePart[1];
                Part part1 = (Part)theSession.Parts.FindObject("001449-010-146");
                partstouse1[0] = part1;
                addComponentBuilder1.SetPartsToAdd(partstouse1);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects1;
                addComponentBuilder1.GetAllProductInterfaceObjects(out productinterfaceobjects1);
                NXObject[] movableObjects1 = new NXObject[1];
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-146 1");
                movableObjects1[0] = component1;
                componentNetwork1.SetMovingGroup(movableObjects1);
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Add Component");
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "AddComponent on_apply");
                componentNetwork1.Solve();
                componentPositioner1.ClearNetwork();
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);
                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);
                componentPositioner1.EndAssemblyConstraints();
                NXOpen.PDM.LogicalObject[] logicalobjects1;
                addComponentBuilder1.GetLogicalObjectsHavingUnassignedRequiredAttributes(out logicalobjects1);
                addComponentBuilder1.ComponentName = "001449-010-146";
                NXObject nXObject1;
                nXObject1 = addComponentBuilder1.Commit();
                ErrorList errorList1;
                errorList1 = addComponentBuilder1.GetOperationFailures();
                errorList1.Dispose();
                theSession.DeleteUndoMark(markId3, null);
                theSession.SetUndoMarkName(markId1, "Add Component");
                addComponentBuilder1.Destroy();
                Arrangement nullNXOpen_Assemblies_Arrangement = null;
                componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMark(markId2, null);
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder2;
                addComponentBuilder2 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner2;
                componentPositioner2 = workPart.ComponentAssembly.Positioner;
                componentPositioner2.ClearNetwork();
                componentPositioner2.PrimaryArrangement = arrangement1;
                componentPositioner2.BeginAssemblyConstraints();
                bool allowInterpartPositioning2;
                allowInterpartPositioning2 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network2;
                network2 = componentPositioner2.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
                componentNetwork2.MoveObjectsState = true;
                componentNetwork2.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId5, "Add Component Dialog");
                componentNetwork2.MoveObjectsState = true;
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                addComponentBuilder2.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                addComponentBuilder2.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                addComponentBuilder2.SetCount(1);
                addComponentBuilder2.SetScatterOption(false);
                addComponentBuilder2.ReferenceSet = "Unknown";
                addComponentBuilder2.Layer = 0;
                addComponentBuilder2.SetInitialLocationType(AddComponentBuilder.LocationType.DisplayedPartWCS);
                componentPositioner2.ClearNetwork();
                addComponentBuilder2.Destroy();
                componentPositioner2.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                componentPositioner2.EndAssemblyConstraints();
                theSession.UndoToMark(markId5, null);
                theSession.DeleteUndoMark(markId5, null);
                Component[] components1 = new Component[1];
                Component component2 = (Component)nXObject1;
                components1[0] = component2;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components1);
                errorList2.Dispose();
                NXOpen.Positioning.ComponentPositioner componentPositioner3;
                componentPositioner3 = workPart.ComponentAssembly.Positioner;
                componentPositioner3.ClearNetwork();
                componentPositioner3.PrimaryArrangement = arrangement1;
                componentPositioner3.BeginMoveComponent();
                bool allowInterpartPositioning3;
                allowInterpartPositioning3 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network3;
                network3 = componentPositioner3.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork3 = (NXOpen.Positioning.ComponentNetwork)network3;
                componentNetwork3.MoveObjectsState = true;
                componentNetwork3.DisplayComponent = nullNXOpen_Assemblies_Component;
                componentNetwork3.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                componentNetwork3.RemoveAllConstraints();
                NXObject[] movableObjects2 = new NXObject[1];
                movableObjects2[0] = component2;
                componentNetwork3.SetMovingGroup(movableObjects2);
                componentNetwork3.Solve();
                componentNetwork3.MoveObjectsState = true;
                componentNetwork3.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                bool loaded1;
                loaded1 = componentNetwork3.IsReferencedGeometryLoaded();
                componentNetwork3.BeginDrag();
                Vector3d translation3 = new Vector3d(0.0, 0.0, 0.40000000000000013);
                componentNetwork3.DragByTranslation(translation3);
                Vector3d translation4 = new Vector3d(0.0, 0.0, 0.50000000000000022);
                componentNetwork3.DragByTranslation(translation4);
                Vector3d translation5 = new Vector3d(0.0, 0.0, 0.59999999999999987);
                componentNetwork3.DragByTranslation(translation5);
                componentNetwork3.EndDrag();
                componentNetwork3.ResetDisplay();
                componentNetwork3.ApplyToModel();
                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along Y-axis");
                bool loaded2;
                loaded2 = componentNetwork3.IsReferencedGeometryLoaded();
                componentNetwork3.BeginDrag();
                Vector3d translation7 = new Vector3d(0.0, -1.5, 0.0);
                componentNetwork3.DragByTranslation(translation7);
                Vector3d translation8 = new Vector3d(0.0, -2.0, 0.0);
                componentNetwork3.DragByTranslation(translation8);
                Vector3d translation9 = new Vector3d(0.0, -2.5, 0.0);
                componentNetwork3.DragByTranslation(translation9);
                Vector3d translation10 = new Vector3d(0.0, -2.0, 0.0);
                componentNetwork3.DragByTranslation(translation10);
                Vector3d translation11 = new Vector3d(0.0, -1.5, 0.0);
                componentNetwork3.DragByTranslation(translation11);
                componentNetwork3.EndDrag();
                componentNetwork3.ResetDisplay();
                componentNetwork3.ApplyToModel();
                Matrix3x3 rotMatrix4 = new Matrix3x3();
                rotMatrix4.Xx = 0.52480415047542583;
                rotMatrix4.Xy = -0.82452978608681193;
                rotMatrix4.Xz = -0.21149760163983919;
                rotMatrix4.Yx = 0.45911763564272562;
                rotMatrix4.Yy = 0.064958868120022661;
                rotMatrix4.Yz = 0.88599737138119872;
                rotMatrix4.Zx = -0.71679257828579834;
                rotMatrix4.Zy = -0.56207737662016144;
                rotMatrix4.Zz = 0.412646849504757;
                Point3d translation12 = new Point3d(1.1883802111873609, -0.91737792208807678, 35.699053608540403);
                workPart.ModelingViews.WorkView.SetRotationTranslationScale(rotMatrix4, translation12, 0.51844685632451726);
                Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along X-axis");
                bool loaded3;
                loaded3 = componentNetwork3.IsReferencedGeometryLoaded();
                componentNetwork3.BeginDrag();
                Vector3d translation13 = new Vector3d(0.70000000000000007, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation13);
                Vector3d translation14 = new Vector3d(0.80000000000000004, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation14);
                Vector3d translation15 = new Vector3d(0.90000000000000002, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation15);
                Vector3d translation16 = new Vector3d(1.0, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation16);
                Vector3d translation17 = new Vector3d(1.2000000000000002, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation17);
                Vector3d translation18 = new Vector3d(1.4000000000000004, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation18);
                Vector3d translation19 = new Vector3d(1.6000000000000001, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation19);
                Vector3d translation20 = new Vector3d(1.8000000000000003, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation20);
                Vector3d translation21 = new Vector3d(2.0, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation21);
                Vector3d translation22 = new Vector3d(2.2000000000000002, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation22);
                Vector3d translation23 = new Vector3d(2.3000000000000003, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation23);
                Vector3d translation24 = new Vector3d(2.4000000000000004, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation24);
                Vector3d translation25 = new Vector3d(2.5, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation25);
                Vector3d translation26 = new Vector3d(2.6000000000000001, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation26);
                Vector3d translation27 = new Vector3d(2.7000000000000002, 0.0, 0.0);
                componentNetwork3.DragByTranslation(translation27);
                componentNetwork3.EndDrag();
                componentNetwork3.ResetDisplay();
                componentNetwork3.ApplyToModel();
                Session.UndoMarkId markId14;
                markId14 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                theSession.DeleteUndoMark(markId14, null);
                Session.UndoMarkId markId15;
                markId15 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                componentNetwork3.Solve();
                componentPositioner3.ClearNetwork();
                int nErrs3;
                nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork3);
                componentPositioner3.DeleteNonPersistentConstraints();
                int nErrs4;
                nErrs4 = theSession.UpdateManager.DoUpdate(session_.SetUndoMark(Session.MarkVisibility.Visible, "update"));
                theSession.DeleteUndoMark(markId15, null);
                componentPositioner3.EndMoveComponent();
                componentPositioner3.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder1;
                addComponentBuilder1 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner1;
                componentPositioner1 = workPart.ComponentAssembly.Positioner;
                componentPositioner1.ClearNetwork();
                componentPositioner1.BeginAssemblyConstraints();
                bool allowInterpartPositioning1;
                allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network1;
                network1 = componentPositioner1.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
                componentNetwork1.MoveObjectsState = true;
                componentNetwork1.DisplayComponent = component1;
                theSession.SetUndoMarkName(markId2, "Add Component Dialog");
                componentNetwork1.MoveObjectsState = true;
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                NXOpen.Assemblies.ProductInterface.InterfaceObject nullNXOpen_Assemblies_ProductInterface_InterfaceObject = null;
                addComponentBuilder1.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                addComponentBuilder1.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                addComponentBuilder1.SetCount(1);
                addComponentBuilder1.SetScatterOption(false);
                addComponentBuilder1.ReferenceSet = "Unknown";
                addComponentBuilder1.Layer = 0;
                addComponentBuilder1.ReferenceSet = "Entire Part";
                addComponentBuilder1.Layer = 0;
                BasePart[] partstouse1 = new BasePart[1];
                Part part1 = (Part)theSession.Parts.FindObject("6mm-shcs-012-2x");
                partstouse1[0] = part1;
                addComponentBuilder1.SetPartsToAdd(partstouse1);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects1;
                addComponentBuilder1.GetAllProductInterfaceObjects(out productinterfaceobjects1);
                Arrangement arrangement1 = (Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
                componentPositioner1.PrimaryArrangement = arrangement1;
                NXObject[] movableObjects1 = new NXObject[1];
                Component component2 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 6mm-shcs-012-2x 1");
                movableObjects1[0] = component2;
                componentNetwork1.SetMovingGroup(movableObjects1);
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Transform Origin");
                bool loaded1;
                loaded1 = componentNetwork1.IsReferencedGeometryLoaded();
                componentNetwork1.BeginDrag();
                Vector3d translation1 = new Vector3d(3.3759999999999999, 2.1269999999999998, -1.6338582677165352);
                componentNetwork1.DragByTranslation(translation1);
                componentNetwork1.EndDrag();
                componentNetwork1.ResetDisplay();
                componentNetwork1.ApplyToModel();
                componentNetwork1.Solve();
                componentPositioner1.ClearNetwork();
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);
                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId3);
                componentPositioner1.EndAssemblyConstraints();
                NXOpen.PDM.LogicalObject[] logicalobjects1;
                addComponentBuilder1.GetLogicalObjectsHavingUnassignedRequiredAttributes(out logicalobjects1);
                addComponentBuilder1.ComponentName = "6MM-SHCS-012-2X";
                NXObject nXObject1;
                nXObject1 = addComponentBuilder1.Commit();
                ErrorList errorList1;
                errorList1 = addComponentBuilder1.GetOperationFailures();
                errorList1.Dispose();
                theSession.SetUndoMarkName(markId2, "Add Component");
                addComponentBuilder1.Destroy();
                Arrangement nullNXOpen_Assemblies_Arrangement = null;
                componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                Component[] components1 = new Component[1];
                Component component3 = (Component)component1.FindObject("COMPONENT 6mm-shcs-012-2x 1");
                components1[0] = component3;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components1);
                errorList2.Dispose();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Component component2 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                Component[] components1 = new Component[1];
                Component component3 = (Component)component2.FindObject("COMPONENT 6mm-shcs-012-2x 1");
                components1[0] = component3;
                ErrorList errorList1;
                errorList1 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("CBORE", components1);
                errorList1.Dispose();
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(83)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                copyPasteBuilder1.CopyResolveGeometry = true;
                copyPasteBuilder1.Associative = true;
                featureReferencesBuilder1.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId5, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                copyPasteBuilder1.CopyResolveGeometry = false;
                copyPasteBuilder1.Associative = false;
                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                scCollector1.SetInterpart(true);
                Body[] bodies1 = new Body[1];
                Body body1 = (Body)component3.FindObject("PROTO#.Bodies|REVOLVED(8)");
                bodies1[0] = body1;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                matchedReferenceData1[0].MatchedEntity = scCollector1;
                matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();
                scCollector2.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                scCollector2.SetInterpart(true);
                Body[] bodies2 = new Body[1];
                bodies2[0] = body1;
                BodyDumbRule bodyDumbRule2;
                bodyDumbRule2 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies2, true);
                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = bodyDumbRule2;
                scCollector2.ReplaceRules(rules2, false);
                matchedReferenceData2[0].MatchedEntity = scCollector2;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                ExtractFace extractFace2 = (ExtractFace)nXObject1;
                Expression[] expressions1;
                expressions1 = extractFace2.GetExpressions();
                copyPasteBuilder1.Destroy();
                PartLoadStatus partLoadStatus3;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus3);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus3.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                PartLoadStatus partLoadStatus4;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus4);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus4.Dispose();
                Session.UndoMarkId markId10;
                markId10 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(84)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);
                copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder2;
                featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                copyPasteBuilder2.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder2.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId10, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData3;
                matchedReferenceData3 = featureReferencesBuilder2.GetMatchedReferences();
                copyPasteBuilder2.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData4;
                matchedReferenceData4 = featureReferencesBuilder2.GetMatchedReferences();
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                copyPasteBuilder2.CopyResolveGeometry = true;
                copyPasteBuilder2.Associative = true;
                ScCollector scCollector3;
                scCollector3 = workPart.ScCollectors.CreateCollector();
                Feature[] features3 = new Feature[1];
                features3[0] = extractFace2;
                BodyFeatureRule bodyFeatureRule1;
                bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features3, true, component2);
                SelectionIntentRule[] rules3 = new SelectionIntentRule[1];
                rules3[0] = bodyFeatureRule1;
                scCollector3.ReplaceRules(rules3, false);
                matchedReferenceData4[0].MatchedEntity = scCollector3; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                Body body2 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].MatchedEntity = body2;
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].ReverseDirection = false; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.CopyResolveGeometry = false;
                copyPasteBuilder2.Associative = false;
                NXObject nXObject2;
                nXObject2 = copyPasteBuilder2.Commit();
                BooleanFeature booleanFeature2 = (BooleanFeature)nXObject2;
                Expression[] expressions2;
                expressions2 = booleanFeature2.GetExpressions();
                copyPasteBuilder2.Destroy();
                Component[] components2 = new Component[1];
                components2[0] = component2;
                component2.UpdateStructure(components2, 2, true);
                Component[] components3 = new Component[1];
                components3[0] = component3;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components3);
                errorList2.Dispose();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder1;
                addComponentBuilder1 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner1;
                componentPositioner1 = workPart.ComponentAssembly.Positioner;
                componentPositioner1.ClearNetwork();
                Arrangement arrangement1 = (Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
                componentPositioner1.PrimaryArrangement = arrangement1;
                componentPositioner1.BeginAssemblyConstraints();
                bool allowInterpartPositioning1;
                allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network1;
                network1 = componentPositioner1.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
                componentNetwork1.MoveObjectsState = true;
                Component nullNXOpen_Assemblies_Component = null;
                componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId1, "Add Component Dialog");
                componentNetwork1.MoveObjectsState = true;
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                NXOpen.Assemblies.ProductInterface.InterfaceObject nullNXOpen_Assemblies_ProductInterface_InterfaceObject = null;
                addComponentBuilder1.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                addComponentBuilder1.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                addComponentBuilder1.SetCount(1);
                addComponentBuilder1.SetScatterOption(false);
                addComponentBuilder1.ReferenceSet = "Unknown";
                addComponentBuilder1.Layer = 0;
                addComponentBuilder1.ReferenceSet = "Entire Part";
                addComponentBuilder1.Layer = 0;
                BasePart[] partstouse1 = new BasePart[1];
                Part part1 = (Part)theSession.Parts.FindObject("001449-010-137");
                partstouse1[0] = part1;
                addComponentBuilder1.SetPartsToAdd(partstouse1);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects1;
                addComponentBuilder1.GetAllProductInterfaceObjects(out productinterfaceobjects1);
                NXObject[] movableObjects1 = new NXObject[1];
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-137 1");
                movableObjects1[0] = component1;
                componentNetwork1.SetMovingGroup(movableObjects1);
                componentNetwork1.Solve();
                componentPositioner1.ClearNetwork();
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);
                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);
                componentPositioner1.EndAssemblyConstraints();
                NXOpen.PDM.LogicalObject[] logicalobjects1;
                addComponentBuilder1.GetLogicalObjectsHavingUnassignedRequiredAttributes(out logicalobjects1);
                addComponentBuilder1.ComponentName = "001449-010-137";
                NXObject nXObject1;
                nXObject1 = addComponentBuilder1.Commit();
                ErrorList errorList1;
                errorList1 = addComponentBuilder1.GetOperationFailures();
                errorList1.Dispose();
                addComponentBuilder1.Destroy();
                Arrangement nullNXOpen_Assemblies_Arrangement = null;
                componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMark(markId2, null);
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder2;
                addComponentBuilder2 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner2;
                componentPositioner2 = workPart.ComponentAssembly.Positioner;
                componentPositioner2.ClearNetwork();
                componentPositioner2.PrimaryArrangement = arrangement1;
                componentPositioner2.BeginAssemblyConstraints();
                bool allowInterpartPositioning2;
                allowInterpartPositioning2 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network2;
                network2 = componentPositioner2.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
                componentNetwork2.MoveObjectsState = true;
                componentNetwork2.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId6, "Add Component Dialog");
                componentNetwork2.MoveObjectsState = true;
                Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                addComponentBuilder2.SetSynchDisplayProperties(false);
                addComponentBuilder2.ReferenceSet = "Entire Part";
                addComponentBuilder2.Layer = 0;
                BasePart[] partstouse2 = new BasePart[1];
                Part part2 = (Part)theSession.Parts.FindObject("001449-010-106");
                partstouse2[0] = part2;
                addComponentBuilder2.SetPartsToAdd(partstouse2);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects2;
                addComponentBuilder2.GetAllProductInterfaceObjects(out productinterfaceobjects2);
                NXObject[] movableObjects2 = new NXObject[1];
                Component component2 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-106 1");
                movableObjects2[0] = component2;
                componentNetwork2.SetMovingGroup(movableObjects2);
                addComponentBuilder2.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                componentNetwork2.EmptyMovingGroup();
                addComponentBuilder2.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                Point3d point1 = new Point3d(0.0, 0.0, 0.0);
                Matrix3x3 orientation1 = new Matrix3x3();
                orientation1.Xx = 1.0;
                orientation1.Xy = 0.0;
                orientation1.Xz = 0.0;
                orientation1.Yx = 0.0;
                orientation1.Yy = 1.0;
                orientation1.Yz = 0.0;
                orientation1.Zx = 0.0;
                orientation1.Zy = 0.0;
                orientation1.Zz = 1.0;
                addComponentBuilder2.SetInitialLocationAndOrientation(point1, orientation1);
                NXObject[] movableObjects3 = new NXObject[1];
                movableObjects3[0] = component2;
                componentNetwork2.SetMovingGroup(movableObjects3);
                componentNetwork2.EmptyMovingGroup();
                addComponentBuilder2.SetCount(1);
                NXObject[] movableObjects4 = new NXObject[1];
                movableObjects4[0] = component2;
                componentNetwork2.SetMovingGroup(movableObjects4);
                addComponentBuilder2.SetScatterOption(false);
                addComponentBuilder2.ReferenceSet = "Entire Part";
                addComponentBuilder2.Layer = 0;
                componentNetwork2.Solve();
                componentPositioner2.ClearNetwork();
                int nErrs3;
                nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork2);
                int nErrs4;
                nErrs4 = theSession.UpdateManager.DoUpdate(markId7);
                componentPositioner2.EndAssemblyConstraints();
                NXOpen.PDM.LogicalObject[] logicalobjects2;
                addComponentBuilder2.GetLogicalObjectsHavingUnassignedRequiredAttributes(out logicalobjects2);
                addComponentBuilder2.ComponentName = "001449-010-106";
                NXObject nXObject2;
                nXObject2 = addComponentBuilder2.Commit();
                ErrorList errorList2;
                errorList2 = addComponentBuilder2.GetOperationFailures();
                errorList2.Dispose();
                theSession.SetUndoMarkName(markId6, "Add Component");
                addComponentBuilder2.Destroy();
                componentPositioner2.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                Matrix3x3 rotMatrix1 = new Matrix3x3();
                rotMatrix1.Xx = 0.46725280404396713;
                rotMatrix1.Xy = -0.87233307654665238;
                rotMatrix1.Xz = 0.14390907085969351;
                rotMatrix1.Yx = -0.87128847682620003;
                rotMatrix1.Yy = -0.4819633006723239;
                rotMatrix1.Yz = -0.092562232875626496;
                rotMatrix1.Zx = 0.15010398816464651;
                rotMatrix1.Zy = -0.082136352291108772;
                rotMatrix1.Zz = -0.9852524612348722;
                Point3d translation1 = new Point3d(1.3283810395041544, -3.1316456555914858, 19.835791641122963);
                workPart.ModelingViews.WorkView.SetRotationTranslationScale(rotMatrix1, translation1, 0.34299703669937609);
                Matrix3x3 rotMatrix2 = new Matrix3x3();
                rotMatrix2.Xx = 0.46725280404396713;
                rotMatrix2.Xy = -0.87233307654665238;
                rotMatrix2.Xz = 0.14390907085969351;
                rotMatrix2.Yx = -0.87106113479445502;
                rotMatrix2.Yy = -0.48208661134486691;
                rotMatrix2.Yz = -0.094047852780495642;
                rotMatrix2.Zx = 0.15141768907115272;
                rotMatrix2.Zy = -0.081409475644259349;
                rotMatrix2.Zz = -0.98511176051840599;
                Point3d translation2 = new Point3d(1.3283810395041544, -3.1441572904398734, 19.836967438591984);
                workPart.ModelingViews.WorkView.SetRotationTranslationScale(rotMatrix2, translation2, 0.34299703669937609);
                Session.UndoMarkId markId11;
                markId11 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder3;
                addComponentBuilder3 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner3;
                componentPositioner3 = workPart.ComponentAssembly.Positioner;
                componentPositioner3.ClearNetwork();
                componentPositioner3.PrimaryArrangement = arrangement1;
                componentPositioner3.BeginAssemblyConstraints();
                bool allowInterpartPositioning3;
                allowInterpartPositioning3 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network3;
                network3 = componentPositioner3.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork3 = (NXOpen.Positioning.ComponentNetwork)network3;
                componentNetwork3.MoveObjectsState = true;
                componentNetwork3.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId11, "Add Component Dialog");
                componentNetwork3.MoveObjectsState = true;
                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                addComponentBuilder3.SetSynchDisplayProperties(false);
                addComponentBuilder3.ReferenceSet = "Entire Part";
                addComponentBuilder3.Layer = 0;
                BasePart[] partstouse3 = new BasePart[1];
                Part part3 = (Part)theSession.Parts.FindObject("001449-010-147");
                partstouse3[0] = part3;
                addComponentBuilder3.SetPartsToAdd(partstouse3);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects3;
                addComponentBuilder3.GetAllProductInterfaceObjects(out productinterfaceobjects3);
                NXObject[] movableObjects5 = new NXObject[1];
                Component component3 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-147 1");
                movableObjects5[0] = component3;
                componentNetwork3.SetMovingGroup(movableObjects5);
                addComponentBuilder3.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                componentNetwork3.EmptyMovingGroup();
                addComponentBuilder3.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                Point3d point2 = new Point3d(0.0, 0.0, 0.0);
                Matrix3x3 orientation2 = new Matrix3x3();
                orientation2.Xx = 1.0;
                orientation2.Xy = 0.0;
                orientation2.Xz = 0.0;
                orientation2.Yx = 0.0;
                orientation2.Yy = 1.0;
                orientation2.Yz = 0.0;
                orientation2.Zx = 0.0;
                orientation2.Zy = 0.0;
                orientation2.Zz = 1.0;
                addComponentBuilder3.SetInitialLocationAndOrientation(point2, orientation2);
                NXObject[] movableObjects6 = new NXObject[1];
                movableObjects6[0] = component3;
                componentNetwork3.SetMovingGroup(movableObjects6);
                componentNetwork3.EmptyMovingGroup();
                addComponentBuilder3.SetCount(1);
                NXObject[] movableObjects7 = new NXObject[1];
                movableObjects7[0] = component3;
                componentNetwork3.SetMovingGroup(movableObjects7);
                addComponentBuilder3.SetScatterOption(false);
                addComponentBuilder3.ReferenceSet = "Entire Part";
                addComponentBuilder3.Layer = 0;
                Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Transform Origin");
                bool loaded1;
                loaded1 = componentNetwork3.IsReferencedGeometryLoaded();
                componentNetwork3.BeginDrag();
                Vector3d translation3 = new Vector3d(2.5019999999999998, 3.6239999999999997, 0.0);
                componentNetwork3.DragByTranslation(translation3);
                componentNetwork3.EndDrag();
                componentNetwork3.ResetDisplay();
                componentNetwork3.ApplyToModel();
                componentPositioner3.ClearNetwork();
                addComponentBuilder3.Destroy();
                componentPositioner3.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                componentPositioner3.EndAssemblyConstraints();
                theSession.DeleteUndoMark(markId12, null);
                theSession.UndoToMark(markId11, null);
                theSession.DeleteUndoMark(markId11, null);
                Session.UndoMarkId markId14;
                markId14 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                AddComponentBuilder addComponentBuilder4;
                addComponentBuilder4 = workPart.AssemblyManager.CreateAddComponentBuilder();
                NXOpen.Positioning.ComponentPositioner componentPositioner4;
                componentPositioner4 = workPart.ComponentAssembly.Positioner;
                componentPositioner4.ClearNetwork();
                componentPositioner4.PrimaryArrangement = arrangement1;
                componentPositioner4.BeginAssemblyConstraints();
                bool allowInterpartPositioning4;
                allowInterpartPositioning4 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network4;
                network4 = componentPositioner4.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork4 = (NXOpen.Positioning.ComponentNetwork)network4;
                componentNetwork4.MoveObjectsState = true;
                componentNetwork4.DisplayComponent = nullNXOpen_Assemblies_Component;
                theSession.SetUndoMarkName(markId14, "Add Component Dialog");
                componentNetwork4.MoveObjectsState = true;
                Session.UndoMarkId markId15;
                markId15 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Assembly Constraints Update");
                addComponentBuilder4.SetComponentAnchor(nullNXOpen_Assemblies_ProductInterface_InterfaceObject);
                addComponentBuilder4.SetInitialLocationType(AddComponentBuilder.LocationType.Snap);
                addComponentBuilder4.SetCount(1);
                addComponentBuilder4.SetScatterOption(false);
                addComponentBuilder4.ReferenceSet = "Unknown";
                addComponentBuilder4.Layer = 0;
                addComponentBuilder4.ReferenceSet = "Entire Part";
                addComponentBuilder4.Layer = 0;
                BasePart[] partstouse4 = new BasePart[1];
                partstouse4[0] = part3;
                addComponentBuilder4.SetPartsToAdd(partstouse4);
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] productinterfaceobjects4;
                addComponentBuilder4.GetAllProductInterfaceObjects(out productinterfaceobjects4);
                NXObject[] movableObjects8 = new NXObject[1];
                Component component4 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-147 1");
                movableObjects8[0] = component4;
                componentNetwork4.SetMovingGroup(movableObjects8);
                Session.UndoMarkId markId16;
                markId16 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Add Component");
                theSession.DeleteUndoMark(markId16, null);
                Session.UndoMarkId markId17;
                markId17 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Add Component");
                Session.UndoMarkId markId18;
                markId18 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "AddComponent on_apply");
                componentNetwork4.Solve();
                componentPositioner4.ClearNetwork();
                int nErrs5;
                nErrs5 = theSession.UpdateManager.AddToDeleteList(componentNetwork4);
                int nErrs6;
                nErrs6 = theSession.UpdateManager.DoUpdate(markId15);
                componentPositioner4.EndAssemblyConstraints();
                NXOpen.PDM.LogicalObject[] logicalobjects3;
                addComponentBuilder4.GetLogicalObjectsHavingUnassignedRequiredAttributes(out logicalobjects3);
                addComponentBuilder4.ComponentName = "001449-010-147";
                NXObject nXObject3;
                nXObject3 = addComponentBuilder4.Commit();
                ErrorList errorList3;
                errorList3 = addComponentBuilder4.GetOperationFailures();
                errorList3.Dispose();
                theSession.DeleteUndoMark(markId17, null);
                theSession.SetUndoMarkName(markId14, "Add Component");
                addComponentBuilder4.Destroy();
                componentPositioner4.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMark(markId15, null);
                Matrix3x3 rotMatrix3 = new Matrix3x3();
                rotMatrix3.Xx = 0.11938993207029078;
                rotMatrix3.Xy = -0.9852060866775989;
                rotMatrix3.Xz = 0.12294312056256769;
                rotMatrix3.Yx = 0.98787778672386928;
                rotMatrix3.Yy = 0.13025218088322552;
                rotMatrix3.Yz = 0.084450268636123232;
                rotMatrix3.Zx = -0.099214528259729873;
                rotMatrix3.Zy = 0.11137026599849177;
                rotMatrix3.Zz = 0.98881400740160474;
                Point3d translation4 = new Point3d(-0.75448283264057026, 0.7463054441218282, 36.46163302348787);
                workPart.ModelingViews.WorkView.SetRotationTranslationScale(rotMatrix3, translation4, 0.38269528128643227);
                Session.UndoMarkId markId19;
                markId19 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId20;
                markId20 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components1 = new Component[1];
                Component component5 = (Component)nXObject2;
                components1[0] = component5;
                ErrorList errorList4;
                errorList4 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components1);
                errorList4.Dispose();
                theSession.DeleteUndoMark(markId19, null);
                Session.UndoMarkId markId21;
                markId21 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId22;
                markId22 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components2 = new Component[1];
                Component component6 = (Component)nXObject3;
                components2[0] = component6;
                ErrorList errorList5;
                errorList5 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components2);
                errorList5.Dispose();
                theSession.DeleteUndoMark(markId21, null);
                Session.UndoMarkId markId23;
                markId23 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId24;
                markId24 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components3 = new Component[1];
                Component component7 = (Component)nXObject1;
                components3[0] = component7;
                ErrorList errorList6;
                errorList6 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components3);
                errorList6.Dispose();
                theSession.DeleteUndoMark(markId23, null);
                Session.UndoMarkId markId25;
                markId25 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXOpen.Positioning.ComponentPositioner componentPositioner5;
                componentPositioner5 = workPart.ComponentAssembly.Positioner;
                componentPositioner5.ClearNetwork();
                componentPositioner5.PrimaryArrangement = arrangement1;
                componentPositioner5.BeginMoveComponent();
                bool allowInterpartPositioning5;
                allowInterpartPositioning5 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network5;
                network5 = componentPositioner5.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork5 = (NXOpen.Positioning.ComponentNetwork)network5;
                componentNetwork5.MoveObjectsState = true;
                componentNetwork5.DisplayComponent = nullNXOpen_Assemblies_Component;
                componentNetwork5.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                componentNetwork5.RemoveAllConstraints();
                NXObject[] movableObjects9 = new NXObject[1];
                movableObjects9[0] = component7;
                componentNetwork5.SetMovingGroup(movableObjects9);
                componentNetwork5.Solve();
                theSession.SetUndoMarkName(markId25, "Move Component Dialog");
                componentNetwork5.MoveObjectsState = true;
                componentNetwork5.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                Session.UndoMarkId markId26;
                markId26 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component Update");
                Session.UndoMarkId markId27;
                markId27 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along Y-axis");
                bool loaded2;
                loaded2 = componentNetwork5.IsReferencedGeometryLoaded();
                componentNetwork5.BeginDrag();
                Vector3d translation5 = new Vector3d(0.0, -4.2999999999999998, 0.0);
                componentNetwork5.DragByTranslation(translation5);
                Vector3d translation6 = new Vector3d(0.0, -4.8000000000000007, 0.0);
                componentNetwork5.DragByTranslation(translation6);
                Vector3d translation7 = new Vector3d(0.0, -5.4000000000000004, 0.0);
                componentNetwork5.DragByTranslation(translation7);
                Vector3d translation8 = new Vector3d(0.0, -6.3000000000000007, 0.0);
                componentNetwork5.DragByTranslation(translation8);
                Vector3d translation9 = new Vector3d(0.0, -6.9000000000000004, 0.0);
                componentNetwork5.DragByTranslation(translation9);
                Vector3d translation10 = new Vector3d(0.0, -7.2000000000000002, 0.0);
                componentNetwork5.DragByTranslation(translation10);
                Vector3d translation11 = new Vector3d(0.0, -7.3000000000000007, 0.0);
                componentNetwork5.DragByTranslation(translation11);
                componentNetwork5.EndDrag();
                componentNetwork5.ResetDisplay();
                componentNetwork5.ApplyToModel();
                Session.UndoMarkId markId28;
                markId28 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along X-axis");
                bool loaded3;
                loaded3 = componentNetwork5.IsReferencedGeometryLoaded();
                componentNetwork5.BeginDrag();
                Vector3d translation12 = new Vector3d(0.80000000000000004, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation12);
                Vector3d translation13 = new Vector3d(1.0, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation13);
                Vector3d translation14 = new Vector3d(1.2000000000000002, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation14);
                Vector3d translation15 = new Vector3d(1.4000000000000001, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation15);
                Vector3d translation16 = new Vector3d(1.6000000000000001, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation16);
                Vector3d translation17 = new Vector3d(1.7000000000000002, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation17);
                Vector3d translation18 = new Vector3d(1.8, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation18);
                Vector3d translation19 = new Vector3d(1.9000000000000001, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation19);
                Vector3d translation20 = new Vector3d(2.0, 0.0, 0.0);
                componentNetwork5.DragByTranslation(translation20);
                componentNetwork5.EndDrag();
                componentNetwork5.ResetDisplay();
                componentNetwork5.ApplyToModel();
                Session.UndoMarkId markId29;
                markId29 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Transform Origin");
                bool loaded4;
                loaded4 = componentNetwork5.IsReferencedGeometryLoaded();
                componentNetwork5.BeginDrag();
                Vector3d translation21 = new Vector3d(-3.2480000000000002, 7.0490000000000004, 0.0);
                componentNetwork5.DragByTranslation(translation21);
                componentNetwork5.EndDrag();
                componentNetwork5.ResetDisplay();
                componentNetwork5.ApplyToModel();
                Session.UndoMarkId markId30;
                markId30 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Transform Origin");
                bool loaded5;
                loaded5 = componentNetwork5.IsReferencedGeometryLoaded();
                componentNetwork5.BeginDrag();
                Vector3d translation22 = new Vector3d(3.625, -7.375, 0.0);
                componentNetwork5.DragByTranslation(translation22);
                componentNetwork5.EndDrag();
                componentNetwork5.ResetDisplay();
                componentNetwork5.ApplyToModel();
                Session.UndoMarkId markId31;
                markId31 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                theSession.DeleteUndoMark(markId31, null);
                Session.UndoMarkId markId32;
                markId32 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                componentNetwork5.Solve();
                componentPositioner5.ClearNetwork();
                int nErrs7;
                nErrs7 = theSession.UpdateManager.AddToDeleteList(componentNetwork5);
                componentPositioner5.DeleteNonPersistentConstraints();
                int nErrs8;
                nErrs8 = theSession.UpdateManager.DoUpdate(markId26);
                theSession.DeleteUndoMark(markId32, null);
                theSession.SetUndoMarkName(markId25, "Move Component");
                componentPositioner5.EndMoveComponent();
                componentPositioner5.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMarksUpToMark(markId26, null, false);
                Session.UndoMarkId markId33;
                markId33 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXOpen.Positioning.ComponentPositioner componentPositioner6;
                componentPositioner6 = workPart.ComponentAssembly.Positioner;
                componentPositioner6.ClearNetwork();
                componentPositioner6.PrimaryArrangement = arrangement1;
                componentPositioner6.BeginMoveComponent();
                bool allowInterpartPositioning6;
                allowInterpartPositioning6 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network6;
                network6 = componentPositioner6.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork6 = (NXOpen.Positioning.ComponentNetwork)network6;
                componentNetwork6.MoveObjectsState = true;
                componentNetwork6.DisplayComponent = nullNXOpen_Assemblies_Component;
                componentNetwork6.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                componentNetwork6.RemoveAllConstraints();
                NXObject[] movableObjects10 = new NXObject[1];
                movableObjects10[0] = component6;
                componentNetwork6.SetMovingGroup(movableObjects10);
                componentNetwork6.Solve();
                theSession.SetUndoMarkName(markId33, "Move Component Dialog");
                componentNetwork6.MoveObjectsState = true;
                componentNetwork6.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                Session.UndoMarkId markId34;
                markId34 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component Update");
                Session.UndoMarkId markId35;
                markId35 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Rotate About Z-axis");
                bool loaded6;
                loaded6 = componentNetwork6.IsReferencedGeometryLoaded();
                componentNetwork6.BeginDrag();
                Vector3d translation23 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation1 = new Matrix3x3();
                rotation1.Xx = 0.93969262078590854;
                rotation1.Xy = 0.34202014332566844;
                rotation1.Xz = 0.0;
                rotation1.Yx = -0.34202014332566844;
                rotation1.Yy = 0.93969262078590854;
                rotation1.Yz = 0.0;
                rotation1.Zx = 0.0;
                rotation1.Zy = 0.0;
                rotation1.Zz = 1.0;
                componentNetwork6.DragByTransform(translation23, rotation1);
                Vector3d translation24 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation2 = new Matrix3x3();
                rotation2.Xx = 0.90630778703665027;
                rotation2.Xy = 0.42261826174069905;
                rotation2.Xz = 0.0;
                rotation2.Yx = -0.42261826174069905;
                rotation2.Yy = 0.90630778703665027;
                rotation2.Yz = 0.0;
                rotation2.Zx = 0.0;
                rotation2.Zy = 0.0;
                rotation2.Zz = 1.0;
                componentNetwork6.DragByTransform(translation24, rotation2);
                Vector3d translation25 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation3 = new Matrix3x3();
                rotation3.Xx = 0.81915204428899224;
                rotation3.Xy = 0.5735764363510456;
                rotation3.Xz = 0.0;
                rotation3.Yx = -0.5735764363510456;
                rotation3.Yy = 0.81915204428899224;
                rotation3.Yz = 0.0;
                rotation3.Zx = 0.0;
                rotation3.Zy = 0.0;
                rotation3.Zz = 1.0;
                componentNetwork6.DragByTransform(translation25, rotation3);
                Vector3d translation26 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation4 = new Matrix3x3();
                rotation4.Xx = 0.76604444311897857;
                rotation4.Xy = 0.64278760968653881;
                rotation4.Xz = 0.0;
                rotation4.Yx = -0.64278760968653881;
                rotation4.Yy = 0.76604444311897857;
                rotation4.Yz = 0.0;
                rotation4.Zx = 0.0;
                rotation4.Zy = 0.0;
                rotation4.Zz = 1.0;
                componentNetwork6.DragByTransform(translation26, rotation4);
                Vector3d translation27 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation5 = new Matrix3x3();
                rotation5.Xx = 0.64278760968654014;
                rotation5.Xy = 0.76604444311897746;
                rotation5.Xz = 0.0;
                rotation5.Yx = -0.76604444311897746;
                rotation5.Yy = 0.64278760968654014;
                rotation5.Yz = 0.0;
                rotation5.Zx = 0.0;
                rotation5.Zy = 0.0;
                rotation5.Zz = 1.0;
                componentNetwork6.DragByTransform(translation27, rotation5);
                Vector3d translation28 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation6 = new Matrix3x3();
                rotation6.Xx = 0.50000000000000111;
                rotation6.Xy = 0.86602540378443815;
                rotation6.Xz = 0.0;
                rotation6.Yx = -0.86602540378443815;
                rotation6.Yy = 0.50000000000000111;
                rotation6.Yz = 0.0;
                rotation6.Zx = 0.0;
                rotation6.Zy = 0.0;
                rotation6.Zz = 1.0;
                componentNetwork6.DragByTransform(translation28, rotation6);
                Vector3d translation29 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation7 = new Matrix3x3();
                rotation7.Xx = 0.3420201433256701;
                rotation7.Xy = 0.93969262078590809;
                rotation7.Xz = 0.0;
                rotation7.Yx = -0.93969262078590809;
                rotation7.Yy = 0.3420201433256701;
                rotation7.Yz = 0.0;
                rotation7.Zx = 0.0;
                rotation7.Zy = 0.0;
                rotation7.Zz = 1.0;
                componentNetwork6.DragByTransform(translation29, rotation7);
                Vector3d translation30 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation8 = new Matrix3x3();
                rotation8.Xx = 0.17364817766693191;
                rotation8.Xy = 0.98480775301220802;
                rotation8.Xz = 0.0;
                rotation8.Yx = -0.98480775301220802;
                rotation8.Yy = 0.17364817766693191;
                rotation8.Yz = 0.0;
                rotation8.Zx = 0.0;
                rotation8.Zy = 0.0;
                rotation8.Zz = 1.0;
                componentNetwork6.DragByTransform(translation30, rotation8);
                Vector3d translation31 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation9 = new Matrix3x3();
                rotation9.Xx = 0.087155742747659803;
                rotation9.Xy = 0.99619469809174566;
                rotation9.Xz = 0.0;
                rotation9.Yx = -0.99619469809174566;
                rotation9.Yy = 0.087155742747659803;
                rotation9.Yz = 0.0;
                rotation9.Zx = 0.0;
                rotation9.Zy = 0.0;
                rotation9.Zz = 1.0;
                componentNetwork6.DragByTransform(translation31, rotation9);
                Vector3d translation32 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation10 = new Matrix3x3();
                rotation10.Xx = 1.6930901125533637e-15;
                rotation10.Xy = 1.0000000000000002;
                rotation10.Xz = 0.0;
                rotation10.Yx = -1.0000000000000002;
                rotation10.Yy = 1.6930901125533637e-15;
                rotation10.Yz = 0.0;
                rotation10.Zx = 0.0;
                rotation10.Zy = 0.0;
                rotation10.Zz = 1.0;
                componentNetwork6.DragByTransform(translation32, rotation10);
                Vector3d translation33 = new Vector3d(0.0, 0.0, 0.0);
                Matrix3x3 rotation11 = new Matrix3x3();
                rotation11.Xx = 1.6930901125533637e-15;
                rotation11.Xy = 1.0000000000000002;
                rotation11.Xz = 0.0;
                rotation11.Yx = -1.0000000000000002;
                rotation11.Yy = 1.6930901125533637e-15;
                rotation11.Yz = 0.0;
                rotation11.Zx = 0.0;
                rotation11.Zy = 0.0;
                rotation11.Zz = 1.0;
                componentNetwork6.DragByTransform(translation33, rotation11);
                componentNetwork6.EndDrag();
                componentNetwork6.ResetDisplay();
                componentNetwork6.ApplyToModel();
                Session.UndoMarkId markId36;
                markId36 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along X-axis");
                bool loaded7;
                loaded7 = componentNetwork6.IsReferencedGeometryLoaded();
                componentNetwork6.BeginDrag();
                Vector3d translation34 = new Vector3d(4.7406523151494187e-15, -2.8000000000000007, 0.0);
                componentNetwork6.DragByTranslation(translation34);
                Vector3d translation35 = new Vector3d(5.5871973714261006e-15, -3.3000000000000012, 0.0);
                componentNetwork6.DragByTranslation(translation35);
                Vector3d translation36 = new Vector3d(6.6030514389581195e-15, -3.9000000000000012, 0.0);
                componentNetwork6.DragByTranslation(translation36);
                Vector3d translation37 = new Vector3d(7.9575235290008092e-15, -4.7000000000000011, 0.0);
                componentNetwork6.DragByTranslation(translation37);
                Vector3d translation38 = new Vector3d(8.8040685852774911e-15, -5.2000000000000011, 0.0);
                componentNetwork6.DragByTranslation(translation38);
                Vector3d translation39 = new Vector3d(8.9733775965328296e-15, -5.3000000000000016, 0.0);
                componentNetwork6.DragByTranslation(translation39);
                Vector3d translation40 = new Vector3d(9.1426866077881651e-15, -5.4000000000000012, 0.0);
                componentNetwork6.DragByTranslation(translation40);
                Vector3d translation41 = new Vector3d(9.6506136415541729e-15, -5.7000000000000011, 0.0);
                componentNetwork6.DragByTranslation(translation41);
                Vector3d translation42 = new Vector3d(9.9892316640648469e-15, -5.9000000000000012, 0.0);
                componentNetwork6.DragByTranslation(translation42);
                Vector3d translation43 = new Vector3d(1.0327849686575519e-14, -6.1000000000000023, 0.0);
                componentNetwork6.DragByTranslation(translation43);
                Vector3d translation44 = new Vector3d(9.8199226528095115e-15, -5.8000000000000016, 0.0);
                componentNetwork6.DragByTranslation(translation44);
                Vector3d translation45 = new Vector3d(9.6506136415541729e-15, -5.7000000000000011, 0.0);
                componentNetwork6.DragByTranslation(translation45);
                Vector3d translation46 = new Vector3d(9.4813046302988375e-15, -5.6000000000000014, 0.0);
                componentNetwork6.DragByTranslation(translation46);
                Vector3d translation47 = new Vector3d(9.1426866077881651e-15, -5.4000000000000012, 0.0);
                componentNetwork6.DragByTranslation(translation47);
                Vector3d translation48 = new Vector3d(8.8040685852774911e-15, -5.2000000000000011, 0.0);
                componentNetwork6.DragByTranslation(translation48);
                Vector3d translation49 = new Vector3d(8.6347595740221556e-15, -5.1000000000000014, 0.0);
                componentNetwork6.DragByTranslation(translation49);
                Vector3d translation50 = new Vector3d(8.4654505627668186e-15, -5.0000000000000009, 0.0);
                componentNetwork6.DragByTranslation(translation50);
                componentNetwork6.EndDrag();
                componentNetwork6.ResetDisplay();
                componentNetwork6.ApplyToModel();
                Session.UndoMarkId markId37;
                markId37 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along Y-axis");
                bool loaded8;
                loaded8 = componentNetwork6.IsReferencedGeometryLoaded();
                componentNetwork6.BeginDrag();
                Vector3d translation51 = new Vector3d(1.5000000000000004, 2.6645352591003757e-15, 0.0);
                componentNetwork6.DragByTranslation(translation51);
                Vector3d translation52 = new Vector3d(1.6000000000000005, 2.6645352591003757e-15, 0.0);
                componentNetwork6.DragByTranslation(translation52);
                Vector3d translation53 = new Vector3d(1.9000000000000006, 3.5527136788005009e-15, 0.0);
                componentNetwork6.DragByTranslation(translation53);
                Vector3d translation54 = new Vector3d(2.1000000000000005, 3.5527136788005009e-15, 0.0);
                componentNetwork6.DragByTranslation(translation54);
                componentNetwork6.EndDrag();
                componentNetwork6.ResetDisplay();
                componentNetwork6.ApplyToModel();
                Session.UndoMarkId markId38;
                markId38 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                theSession.DeleteUndoMark(markId38, null);
                Session.UndoMarkId markId39;
                markId39 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                componentNetwork6.Solve();
                componentPositioner6.ClearNetwork();
                int nErrs9;
                nErrs9 = theSession.UpdateManager.AddToDeleteList(componentNetwork6);
                componentPositioner6.DeleteNonPersistentConstraints();
                int nErrs10;
                nErrs10 = theSession.UpdateManager.DoUpdate(markId34);
                theSession.DeleteUndoMark(markId39, null);
                theSession.SetUndoMarkName(markId33, "Move Component");
                componentPositioner6.EndMoveComponent();
                componentPositioner6.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMarksUpToMark(markId34, null, false);
                Session.UndoMarkId markId40;
                markId40 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXOpen.Positioning.ComponentPositioner componentPositioner7;
                componentPositioner7 = workPart.ComponentAssembly.Positioner;
                componentPositioner7.ClearNetwork();
                componentPositioner7.PrimaryArrangement = arrangement1;
                componentPositioner7.BeginMoveComponent();
                bool allowInterpartPositioning7;
                allowInterpartPositioning7 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network7;
                network7 = componentPositioner7.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork7 = (NXOpen.Positioning.ComponentNetwork)network7;
                componentNetwork7.MoveObjectsState = true;
                componentNetwork7.DisplayComponent = nullNXOpen_Assemblies_Component;
                componentNetwork7.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                componentNetwork7.RemoveAllConstraints();
                NXObject[] movableObjects11 = new NXObject[1];
                movableObjects11[0] = component5;
                componentNetwork7.SetMovingGroup(movableObjects11);
                componentNetwork7.Solve();
                theSession.SetUndoMarkName(markId40, "Move Component Dialog");
                componentNetwork7.MoveObjectsState = true;
                componentNetwork7.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                Session.UndoMarkId markId41;
                markId41 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component Update");
                Session.UndoMarkId markId42;
                markId42 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along X-axis");
                bool loaded9;
                loaded9 = componentNetwork7.IsReferencedGeometryLoaded();
                componentNetwork7.BeginDrag();
                Vector3d translation55 = new Vector3d(1.8, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation55);
                Vector3d translation56 = new Vector3d(2.2000000000000002, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation56);
                Vector3d translation57 = new Vector3d(2.8000000000000003, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation57);
                Vector3d translation58 = new Vector3d(3.2000000000000002, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation58);
                Vector3d translation59 = new Vector3d(3.5, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation59);
                Vector3d translation60 = new Vector3d(3.7000000000000002, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation60);
                Vector3d translation61 = new Vector3d(3.8000000000000003, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation61);
                Vector3d translation62 = new Vector3d(3.9000000000000004, 0.0, 0.0);
                componentNetwork7.DragByTranslation(translation62);
                componentNetwork7.EndDrag();
                componentNetwork7.ResetDisplay();
                componentNetwork7.ApplyToModel();
                Session.UndoMarkId markId43;
                markId43 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along Y-axis");
                bool loaded10;
                loaded10 = componentNetwork7.IsReferencedGeometryLoaded();
                componentNetwork7.BeginDrag();
                Vector3d translation63 = new Vector3d(0.0, -1.7000000000000002, 0.0);
                componentNetwork7.DragByTranslation(translation63);
                Vector3d translation64 = new Vector3d(0.0, -2.1000000000000001, 0.0);
                componentNetwork7.DragByTranslation(translation64);
                Vector3d translation65 = new Vector3d(0.0, -2.6000000000000001, 0.0);
                componentNetwork7.DragByTranslation(translation65);
                Vector3d translation66 = new Vector3d(0.0, -3.2000000000000002, 0.0);
                componentNetwork7.DragByTranslation(translation66);
                Vector3d translation67 = new Vector3d(0.0, -3.7000000000000002, 0.0);
                componentNetwork7.DragByTranslation(translation67);
                Vector3d translation68 = new Vector3d(0.0, -4.1000000000000005, 0.0);
                componentNetwork7.DragByTranslation(translation68);
                Vector3d translation69 = new Vector3d(0.0, -4.5, 0.0);
                componentNetwork7.DragByTranslation(translation69);
                Vector3d translation70 = new Vector3d(0.0, -4.9000000000000004, 0.0);
                componentNetwork7.DragByTranslation(translation70);
                Vector3d translation71 = new Vector3d(0.0, -5.3000000000000007, 0.0);
                componentNetwork7.DragByTranslation(translation71);
                Vector3d translation72 = new Vector3d(0.0, -5.6000000000000005, 0.0);
                componentNetwork7.DragByTranslation(translation72);
                Vector3d translation73 = new Vector3d(0.0, -6.0, 0.0);
                componentNetwork7.DragByTranslation(translation73);
                Vector3d translation74 = new Vector3d(0.0, -6.2000000000000002, 0.0);
                componentNetwork7.DragByTranslation(translation74);
                componentNetwork7.EndDrag();
                componentNetwork7.ResetDisplay();
                componentNetwork7.ApplyToModel();
                Session.UndoMarkId markId44;
                markId44 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                theSession.DeleteUndoMark(markId44, null);
                Session.UndoMarkId markId45;
                markId45 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                componentNetwork7.Solve();
                componentPositioner7.ClearNetwork();
                int nErrs11;
                nErrs11 = theSession.UpdateManager.AddToDeleteList(componentNetwork7);
                componentPositioner7.DeleteNonPersistentConstraints();
                int nErrs12;
                nErrs12 = theSession.UpdateManager.DoUpdate(markId41);
                theSession.DeleteUndoMark(markId45, null);
                theSession.SetUndoMarkName(markId40, "Move Component");
                componentPositioner7.EndMoveComponent();
                componentPositioner7.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMarksUpToMark(markId41, null, false);
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component nullNXOpen_Assemblies_Component = null;
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                partLoadStatus2.Dispose();
                theSession.SetUndoMarkName(markId2, "Make Work Part");
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components1 = new Component[1];
                Component component2 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-146 1");
                Component component3 = (Component)component2.FindObject("COMPONENT 8mm-shcs-020 1");
                components1[0] = component3;
                ErrorList errorList1;
                errorList1 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("TAP", components1);
                errorList1.Dispose();
                theSession.DeleteUndoMark(markId3, null);
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component4 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus3;
                theSession.Parts.SetWorkComponent(component4, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus3);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus3.Dispose();
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(70)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder1.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId6, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                Section section1;
                section1 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section2;
                section2 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section1.Destroy();
                section2.Destroy();
                copyPasteBuilder1.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                scCollector1.SetInterpart(true);
                Body[] bodies1 = new Body[1];
                Body body1 = (Body)component3.FindObject("PROTO#.Bodies|CYLINDER(9)");
                bodies1[0] = body1;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                matchedReferenceData2[0].MatchedEntity = scCollector1;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                //Session.UndoMarkId markId7;
                Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Paste Feature");
                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                theSession.DeleteUndoMark(markId8, null);
                theSession.SetUndoMarkName(markId6, "Paste Feature");
                ExtractFace extractFace2 = (ExtractFace)nXObject1;
                Expression[] expressions1;
                expressions1 = extractFace2.GetExpressions();
                copyPasteBuilder1.Destroy();
                Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(extractFace2, markId9);
                Session.UndoMarkId markId10;
                markId10 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                ExtractFaceBuilder extractFaceBuilder1;
                extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(extractFace2);
                DisplayableObject[] replacementobjects1 = new DisplayableObject[1];
                Component component5 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-146 2");
                Component component6 = (Component)component5.FindObject("COMPONENT 8mm-shcs-020 1");
                Body body2 = (Body)component6.FindObject("PROTO#.Bodies|CYLINDER(9)");
                replacementobjects1[0] = body2;
                extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                theSession.SetUndoMarkName(markId10, "WAVE Geometry Linker Dialog");
                extractFaceBuilder1.Associative = false;
                Session.UndoMarkId markId11;
                markId11 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                theSession.DeleteUndoMark(markId11, null);
                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                TaggedObject nullNXOpen_TaggedObject = null;
                extractFaceBuilder1.SourcePartOccurrence = nullNXOpen_TaggedObject;
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                NXObject nXObject2;
                nXObject2 = extractFaceBuilder1.Commit();
                theSession.DeleteUndoMark(markId12, null);
                theSession.SetUndoMarkName(markId10, "WAVE Geometry Linker");
                extractFaceBuilder1.Destroy();
                theSession.DeleteUndoMark(markId10, null);
                editWithRollbackManager1.UpdateFeature(false);
                editWithRollbackManager1.Stop();
                theSession.Preferences.Modeling.UpdatePending = false;
                editWithRollbackManager1.Destroy();
                Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                ExtractFace extractFace3 = (ExtractFace)nXObject2;
                EditWithRollbackManager editWithRollbackManager2;
                editWithRollbackManager2 = workPart.Features.StartEditWithRollbackManager(extractFace3, markId13);
                Session.UndoMarkId markId14;
                markId14 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                ExtractFaceBuilder extractFaceBuilder2;
                extractFaceBuilder2 = workPart.Features.CreateExtractFaceBuilder(extractFace3);
                DisplayableObject[] replacementobjects2 = new DisplayableObject[0];
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects2);
                theSession.SetUndoMarkName(markId14, "WAVE Geometry Linker Dialog");
                Body[] bodies2 = new Body[1];
                bodies2[0] = body1;
                BodyDumbRule bodyDumbRule2;
                bodyDumbRule2 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies2, true);
                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = bodyDumbRule2;
                extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules2, false);
                DisplayableObject[] replacementobjects3 = new DisplayableObject[0];
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects3);
                DisplayableObject[] replacementobjects4 = new DisplayableObject[1];
                replacementobjects4[0] = body1;
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects4);
                Part part2 = (Part)theSession.Parts.FindObject("8mm-shcs-020");
                BodyFeature bodyFeature1 = (BodyFeature)part2.Features.FindObject("CHAMFER(12)");
                extractFaceBuilder2.FrecAtTimeStamp = bodyFeature1;
                extractFaceBuilder2.Associative = true;
                Session.UndoMarkId markId15;
                markId15 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                theSession.DeleteUndoMark(markId15, null);
                Session.UndoMarkId markId16;
                markId16 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                extractFaceBuilder2.SourcePartOccurrence = nullNXOpen_TaggedObject;
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects2 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                extractFaceBuilder2.SetProductInterfaceObjects(selectedobjects2);
                NXObject nXObject3;
                nXObject3 = extractFaceBuilder2.Commit();
                theSession.DeleteUndoMark(markId16, null);
                theSession.SetUndoMarkName(markId14, "WAVE Geometry Linker");
                extractFaceBuilder2.Destroy();
                theSession.DeleteUndoMark(markId14, null);
                editWithRollbackManager2.UpdateFeature(false);
                editWithRollbackManager2.Stop();
                theSession.Preferences.Modeling.UpdatePending = false;
                editWithRollbackManager2.Destroy();
                Component[] components2 = new Component[1];
                components2[0] = component4;
                component4.UpdateStructure(components2, 2, true);
                Session.UndoMarkId markId17;
                markId17 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus4;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus4);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus4.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                Session.UndoMarkId markId18;
                markId18 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(component4, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus5.Dispose();
                Session.UndoMarkId markId19;
                markId19 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(71)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);
                copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder2;
                featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                copyPasteBuilder2.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder2.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId19, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData3;
                matchedReferenceData3 = featureReferencesBuilder2.GetMatchedReferences();
                Section section3;
                section3 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section4;
                section4 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section5;
                section5 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section3.Destroy();
                section4.Destroy();
                section5.Destroy();
                copyPasteBuilder2.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData4;
                matchedReferenceData4 = featureReferencesBuilder2.GetMatchedReferences();
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                copyPasteBuilder2.CopyResolveGeometry = true;
                copyPasteBuilder2.Associative = true;
                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();
                Feature[] features3 = new Feature[1];
                ExtractFace extractFace4 = (ExtractFace)nXObject3;
                features3[0] = extractFace4;
                BodyFeatureRule bodyFeatureRule1;
                bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features3, true, component4);
                SelectionIntentRule[] rules3 = new SelectionIntentRule[1];
                rules3[0] = bodyFeatureRule1;
                scCollector2.ReplaceRules(rules3, false);
                matchedReferenceData4[0].MatchedEntity = scCollector2; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                Body body3 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].MatchedEntity = body3;
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].ReverseDirection = false; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.CopyResolveGeometry = false;
                copyPasteBuilder2.Associative = false;
                Session.UndoMarkId markId20;
                markId20 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Paste Feature");
                theSession.DeleteUndoMark(markId20, null);
                Session.UndoMarkId markId21;
                markId21 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Paste Feature");
                NXObject nXObject4;
                nXObject4 = copyPasteBuilder2.Commit();
                theSession.DeleteUndoMark(markId21, null);
                theSession.SetUndoMarkName(markId19, "Paste Feature");
                BooleanFeature booleanFeature2 = (BooleanFeature)nXObject4;
                Expression[] expressions2;
                expressions2 = booleanFeature2.GetExpressions();
                copyPasteBuilder2.Destroy();
                Session.UndoMarkId markId22;
                markId22 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus6;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus6);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                partLoadStatus6.Dispose();
                theSession.SetUndoMarkName(markId22, "Make Work Part");
                Component[] components3 = new Component[1];
                components3[0] = component1;
                component1.UpdateStructure(components3, 2, true);
                Session.UndoMarkId markId23;
                markId23 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId24;
                markId24 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components4 = new Component[1];
                components4[0] = component3;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components4);
                errorList2.Dispose();
                theSession.DeleteUndoMark(markId23, null);
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Copy");
                NXOpen.Gateway.CopyCutBuilder copyCutBuilder1;
                copyCutBuilder1 = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();
                copyCutBuilder1.CanCopyAsSketch = true;
                copyCutBuilder1.IsCut = false;
                copyCutBuilder1.ToClipboard = true;
                copyCutBuilder1.DestinationFilename = null;
                NXObject[] objects1 = new NXObject[4];
                Line line1 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20337");
                objects1[0] = line1;
                Line line2 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20338");
                objects1[1] = line2;
                Line line3 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20336");
                objects1[2] = line3;
                Line line4 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20339");
                objects1[3] = line4;
                copyCutBuilder1.SetObjects(objects1);
                NXObject nXObject1;
                nXObject1 = copyCutBuilder1.Commit();
                copyCutBuilder1.Destroy();
                theSession.DeleteUndoMark(markId2, null);
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component2 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Paste");
                NXOpen.Gateway.PasteBuilder pasteBuilder1;
                pasteBuilder1 = workPart.ClipboardOperationsManager.CreatePasteBuilder();
                NXObject nXObject2;
                nXObject2 = pasteBuilder1.Commit();
                pasteBuilder1.Destroy();
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus3;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus3);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus3.Dispose();
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Copy");
                NXOpen.Gateway.CopyCutBuilder copyCutBuilder2;
                copyCutBuilder2 = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();
                copyCutBuilder2.CanCopyAsSketch = true;
                copyCutBuilder2.IsCut = false;
                copyCutBuilder2.ToClipboard = true;
                copyCutBuilder2.DestinationFilename = null;
                NXObject[] objects2 = new NXObject[1];
                Line line5 = (Line)component1.FindObject("PROTO#.Lines|HANDLE R-20340");
                objects2[0] = line5;
                copyCutBuilder2.SetObjects(objects2);
                NXObject nXObject3;
                nXObject3 = copyCutBuilder2.Commit();
                copyCutBuilder2.Destroy();
                Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus4;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus4);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus4.Dispose();
                Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Paste");
                NXOpen.Gateway.PasteBuilder pasteBuilder2;
                pasteBuilder2 = workPart.ClipboardOperationsManager.CreatePasteBuilder();
                NXObject nXObject4;
                nXObject4 = pasteBuilder2.Commit();
                pasteBuilder2.Destroy();
                Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(component1, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus5.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                Session.UndoMarkId markId10;
                markId10 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus6;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus6);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus6.Dispose();
                Session.UndoMarkId markId11;
                markId11 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                Extrude extrude1 = (Extrude)part1.Features.FindObject("EXTRUDE(87)");
                features1[0] = extrude1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder1.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId11, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                copyPasteBuilder1.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                Section section4;
                section4 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section4.Clear();
                section4.Clear();
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                copyPasteBuilder1.CopyResolveGeometry = true;
                copyPasteBuilder1.Associative = true;
                section4.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);
                section4.Clear();
                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "section mark");
                Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, null);
                IBaseCurve[] curves1 = new IBaseCurve[1];
                Arc arc1 = (Arc)workPart.Arcs.FindObject("ENTITY 5 1 1");
                curves1[0] = arc1;
                CurveDumbRule curveDumbRule1;
                curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = curveDumbRule1;
                NXObject nullNXOpen_NXObject = null;
                Point3d helpPoint1 = new Point3d(13.195298067890992, -4.8082369036210464, 0.0);
                section4.AddToSection(rules1, arc1, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint1, Section.Mode.Create, false);
                theSession.DeleteUndoMark(markId13, null);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                theSession.DeleteUndoMark(markId12, null);
                IBaseCurve[] curves2 = new IBaseCurve[1];
                Line line6 = (Line)workPart.Lines.FindObject("ENTITY 3 6 1");
                curves2[0] = line6;
                CurveDumbRule curveDumbRule2;
                curveDumbRule2 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves2);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = curveDumbRule2;
                Point3d helpPoint2 = new Point3d(14.973043195357299, -2.6699941733691785, 9.0899510141184692e-15);
                section4.AddToSection(rules2, line6, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint2, Section.Mode.Create, false);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                IBaseCurve[] curves3 = new IBaseCurve[1];
                Line line7 = (Line)workPart.Lines.FindObject("ENTITY 3 4 1");
                curves3[0] = line7;
                CurveDumbRule curveDumbRule3;
                curveDumbRule3 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves3);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules3 = new SelectionIntentRule[1];
                rules3[0] = curveDumbRule3;
                Point3d helpPoint3 = new Point3d(14.837778387448598, 0.71864938141959622, 1.3322676295501878e-15);
                section4.AddToSection(rules3, line7, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint3, Section.Mode.Create, false);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                IBaseCurve[] curves4 = new IBaseCurve[1];
                Line line8 = (Line)workPart.Lines.FindObject("ENTITY 3 3 1");
                curves4[0] = line8;
                CurveDumbRule curveDumbRule4;
                curveDumbRule4 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves4);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules4 = new SelectionIntentRule[1];
                rules4[0] = curveDumbRule4;
                Point3d helpPoint4 = new Point3d(13.556267750982716, 1.759563278925574, 1.5543122344752192e-15);
                section4.AddToSection(rules4, line8, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint4, Section.Mode.Create, false);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                Session.UndoMarkId markId20;
                markId20 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "section mark");
                Session.UndoMarkId markId21;
                markId21 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, null);
                IBaseCurve[] curves5 = new IBaseCurve[1];
                Line line9 = (Line)workPart.Lines.FindObject("ENTITY 3 2 1");
                curves5[0] = line9;
                CurveDumbRule curveDumbRule5;
                curveDumbRule5 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves5);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules5 = new SelectionIntentRule[1];
                rules5[0] = curveDumbRule5;
                Point3d helpPoint5 = new Point3d(12.076090929898651, 1.2829818694167157, 9.9920072216264089e-16);
                section4.AddToSection(rules5, line9, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint5, Section.Mode.Create, false);
                theSession.DeleteUndoMark(markId21, null);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                theSession.DeleteUndoMark(markId20, null);
                Session.UndoMarkId markId22;
                markId22 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "section mark");
                Session.UndoMarkId markId23;
                markId23 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, null);
                IBaseCurve[] curves6 = new IBaseCurve[1];
                Line line10 = (Line)workPart.Lines.FindObject("ENTITY 3 1 1");
                curves6[0] = line10;
                CurveDumbRule curveDumbRule6;
                curveDumbRule6 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves6);
                section4.AllowSelfIntersection(true);
                SelectionIntentRule[] rules6 = new SelectionIntentRule[1];
                rules6[0] = curveDumbRule6;
                Point3d helpPoint6 = new Point3d(11.516644257324145, -1.7190451370963968, 7.6605388699135801e-15);
                section4.AddToSection(rules6, line10, nullNXOpen_NXObject, nullNXOpen_NXObject, helpPoint6, Section.Mode.Create, false);
                theSession.DeleteUndoMark(markId23, null);
                matchedReferenceData2[0].MatchedEntity = section4; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                featureReferencesBuilder1.AutomaticMatch(false);
                theSession.DeleteUndoMark(markId22, null);
                copyPasteBuilder1.CopyResolveGeometry = false;
                copyPasteBuilder1.Associative = false;
                copyPasteBuilder1.CopyResolveGeometry = true;
                copyPasteBuilder1.Associative = true;
                NXObject nXObject5;
                nXObject5 = copyPasteBuilder1.Commit();
                Extrude extrude2 = (Extrude)nXObject5;
                Expression[] expressions1;
                expressions1 = extrude2.GetExpressions();
                copyPasteBuilder1.Destroy();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXOpen.Positioning.ComponentPositioner componentPositioner1;
                componentPositioner1 = workPart.ComponentAssembly.Positioner;
                componentPositioner1.ClearNetwork();
                Arrangement arrangement1 = (Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
                componentPositioner1.PrimaryArrangement = arrangement1;
                componentPositioner1.BeginMoveComponent();
                bool allowInterpartPositioning1;
                allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;
                NXOpen.Positioning.Network network1;
                network1 = componentPositioner1.EstablishNetwork();
                NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
                componentNetwork1.MoveObjectsState = true;
                Component nullNXOpen_Assemblies_Component = null;
                componentNetwork1.DisplayComponent = nullNXOpen_Assemblies_Component;
                componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                componentNetwork1.RemoveAllConstraints();
                NXObject[] movableObjects1 = new NXObject[1];
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-137 1");
                movableObjects1[0] = component1;
                componentNetwork1.SetMovingGroup(movableObjects1);
                componentNetwork1.Solve();
                theSession.SetUndoMarkName(markId1, "Move Component Dialog");
                componentNetwork1.MoveObjectsState = true;
                componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;
                Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component Update");
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Translate Along Z-axis");
                bool loaded1;
                loaded1 = componentNetwork1.IsReferencedGeometryLoaded();
                componentNetwork1.BeginDrag();
                Vector3d translation1 = new Vector3d(0.0, 0.0, -0.20000000000000001);
                componentNetwork1.DragByTranslation(translation1);
                Vector3d translation2 = new Vector3d(0.0, 0.0, -0.30000000000000004);
                componentNetwork1.DragByTranslation(translation2);
                Vector3d translation3 = new Vector3d(0.0, 0.0, -0.40000000000000002);
                componentNetwork1.DragByTranslation(translation3);
                Vector3d translation4 = new Vector3d(0.0, 0.0, -0.5);
                componentNetwork1.DragByTranslation(translation4);
                Vector3d translation5 = new Vector3d(0.0, 0.0, -0.70000000000000007);
                componentNetwork1.DragByTranslation(translation5);
                Vector3d translation6 = new Vector3d(0.0, 0.0, -0.80000000000000004);
                componentNetwork1.DragByTranslation(translation6);
                componentNetwork1.EndDrag();
                componentNetwork1.ResetDisplay();
                componentNetwork1.ApplyToModel();
                Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                theSession.DeleteUndoMark(markId4, null);
                Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Move Component");
                componentNetwork1.Solve();
                componentPositioner1.ClearNetwork();
                int nErrs1;
                nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);
                componentPositioner1.DeleteNonPersistentConstraints();
                int nErrs2;
                nErrs2 = theSession.UpdateManager.DoUpdate(markId2);
                theSession.DeleteUndoMark(markId5, null);
                theSession.SetUndoMarkName(markId1, "Move Component");
                componentPositioner1.EndMoveComponent();
                Arrangement nullNXOpen_Assemblies_Arrangement = null;
                componentPositioner1.PrimaryArrangement = nullNXOpen_Assemblies_Arrangement;
                theSession.DeleteUndoMarksUpToMark(markId2, null, false);
                Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component2 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component3 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component3, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components1 = new Component[1];
                components1[0] = component1;
                ErrorList errorList1;
                errorList1 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("SUBTOOL", components1);
                errorList1.Dispose();
                theSession.DeleteUndoMark(markId8, null);
                Session.UndoMarkId markId10;
                markId10 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(65)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder1.AutomaticMatch(true);
                theSession.SetUndoMarkName(markId10, "Paste Feature Dialog");
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                Section section1;
                section1 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section2;
                section2 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section1.Destroy();
                section2.Destroy();
                copyPasteBuilder1.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                scCollector1.SetInterpart(true);
                Body[] bodies1 = new Body[1];
                Body body1 = (Body)component1.FindObject("PROTO#.Bodies|EXTRACT_BODY(4)");
                bodies1[0] = body1;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                matchedReferenceData2[0].MatchedEntity = scCollector1;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                Session.UndoMarkId markId11;
                markId11 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Paste Feature");
                theSession.DeleteUndoMark(markId11, null);
                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Paste Feature");
                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                theSession.DeleteUndoMark(markId12, null);
                theSession.SetUndoMarkName(markId10, "Paste Feature");
                ExtractFace extractFace2 = (ExtractFace)nXObject1;
                Expression[] expressions1;
                expressions1 = extractFace2.GetExpressions();
                copyPasteBuilder1.Destroy();
                Session.UndoMarkId markId13;
                markId13 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Replace Reference Set");
                Session.UndoMarkId markId14;
                markId14 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Replace Reference Set");
                Component[] components2 = new Component[1];
                components2[0] = component1;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components2);
                errorList2.Dispose();
                theSession.DeleteUndoMark(markId13, null);
                Session.UndoMarkId markId15;
                markId15 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus3;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus3);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus3.Dispose();
                workPart.PmiManager.RestoreUnpastedObjects();
                Session.UndoMarkId markId16;
                markId16 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                PartLoadStatus partLoadStatus4;
                theSession.Parts.SetWorkComponent(component3, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus4);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus4.Dispose();
                Session.UndoMarkId markId17;
                markId17 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(extractFace2, markId17);
                Session.UndoMarkId markId18;
                markId18 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                ExtractFaceBuilder extractFaceBuilder1;
                extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(extractFace2);
                DisplayableObject[] replacementobjects1 = new DisplayableObject[1];
                Component component4 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-137 2");
                Body body2 = (Body)component4.FindObject("PROTO#.Bodies|EXTRACT_BODY(4)");
                replacementobjects1[0] = body2;
                extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                theSession.SetUndoMarkName(markId18, "WAVE Geometry Linker Dialog");
                extractFaceBuilder1.Associative = false;
                Session.UndoMarkId markId19;
                markId19 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                theSession.DeleteUndoMark(markId19, null);
                Session.UndoMarkId markId20;
                markId20 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "WAVE Geometry Linker");
                TaggedObject nullNXOpen_TaggedObject = null;
                extractFaceBuilder1.SourcePartOccurrence = nullNXOpen_TaggedObject;
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                NXObject nXObject2;
                nXObject2 = extractFaceBuilder1.Commit();
                theSession.DeleteUndoMark(markId20, null);
                theSession.SetUndoMarkName(markId18, "WAVE Geometry Linker");
                extractFaceBuilder1.Destroy();
                theSession.DeleteUndoMark(markId18, null);
                editWithRollbackManager1.UpdateFeature(false);
                editWithRollbackManager1.Stop();
                theSession.Preferences.Modeling.UpdatePending = false;
                editWithRollbackManager1.Destroy();
                Component[] components3 = new Component[1];
                components3[0] = component1;
                ErrorList errorList3;
                errorList3 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("SUBTOOL", components3);
                errorList3.Dispose();
                Session.UndoMarkId markId23;
                markId23 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                ExtractFace extractFace3 = (ExtractFace)nXObject2;
                EditWithRollbackManager editWithRollbackManager2;
                editWithRollbackManager2 = workPart.Features.StartEditWithRollbackManager(extractFace3, markId23);
                Session.UndoMarkId markId24;
                markId24 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                ExtractFaceBuilder extractFaceBuilder2;
                extractFaceBuilder2 = workPart.Features.CreateExtractFaceBuilder(extractFace3);
                DisplayableObject[] replacementobjects2 = new DisplayableObject[0];
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects2);
                theSession.SetUndoMarkName(markId24, "WAVE Geometry Linker Dialog");
                Body[] bodies2 = new Body[1];
                bodies2[0] = body1;
                BodyDumbRule bodyDumbRule2;
                bodyDumbRule2 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies2, true);
                SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                rules2[0] = bodyDumbRule2;
                extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules2, false);
                DisplayableObject[] replacementobjects3 = new DisplayableObject[0];
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects3);
                DisplayableObject[] replacementobjects4 = new DisplayableObject[1];
                replacementobjects4[0] = body1;
                extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects4);
                Part part2 = (Part)theSession.Parts.FindObject("001449-010-137");
                OffsetFace offsetFace1 = (OffsetFace)part2.Features.FindObject("OFFSET(5)");
                extractFaceBuilder2.FrecAtTimeStamp = offsetFace1;
                extractFaceBuilder2.Associative = true;
                extractFaceBuilder2.InheritDisplayProperties = true;
                extractFaceBuilder2.SourcePartOccurrence = nullNXOpen_TaggedObject;
                NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects2 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                extractFaceBuilder2.SetProductInterfaceObjects(selectedobjects2);
                NXObject nXObject3;
                nXObject3 = extractFaceBuilder2.Commit();
                extractFaceBuilder2.Destroy();
                editWithRollbackManager2.UpdateFeature(false);
                editWithRollbackManager2.Stop();
                theSession.Preferences.Modeling.UpdatePending = false;
                editWithRollbackManager2.Destroy();
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(67)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);
                copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder2;
                featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                copyPasteBuilder2.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder2.AutomaticMatch(true);
                MatchedReferenceBuilder[] matchedReferenceData3;
                matchedReferenceData3 = featureReferencesBuilder2.GetMatchedReferences();
                copyPasteBuilder2.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData4;
                matchedReferenceData4 = featureReferencesBuilder2.GetMatchedReferences();
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                copyPasteBuilder2.CopyResolveGeometry = true;
                copyPasteBuilder2.Associative = true;
                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();
                Feature[] features3 = new Feature[1];
                ExtractFace extractFace4 = (ExtractFace)nXObject3;
                features3[0] = extractFace4;
                BodyFeatureRule bodyFeatureRule1;
                bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features3, true, component3);
                SelectionIntentRule[] rules3 = new SelectionIntentRule[1];
                rules3[0] = bodyFeatureRule1;
                scCollector2.ReplaceRules(rules3, false);
                matchedReferenceData4[0].MatchedEntity = scCollector2; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                Body body3 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].MatchedEntity = body3;
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].ReverseDirection = false; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.CopyResolveGeometry = false;
                copyPasteBuilder2.Associative = false;
                NXObject nXObject4;
                nXObject4 = copyPasteBuilder2.Commit();
                BooleanFeature booleanFeature2 = (BooleanFeature)nXObject4;
                Expression[] expressions2;
                expressions2 = booleanFeature2.GetExpressions();
                copyPasteBuilder2.Destroy();
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                partLoadStatus5.Dispose();
                Component[] components4 = new Component[1];
                components4[0] = component1;
                ErrorList errorList4;
                errorList4 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components4);
                errorList4.Dispose();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component[] components1 = new Component[1];
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-147 1");
                components1[0] = component1;
                ErrorList errorList1;
                errorList1 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("SUB_TOOL", components1);
                errorList1.Dispose();
                Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                Component component2 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1");
                PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component2, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                Component component3 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1");
                PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component3, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(85)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder1;
                featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.InputForOriginalParent;
                featureReferencesBuilder1.AutomaticMatch(true);
                MatchedReferenceBuilder[] matchedReferenceData1;
                matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                Section section1;
                section1 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section2;
                section2 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section1.Destroy();
                section2.Destroy();
                copyPasteBuilder1.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData2;
                matchedReferenceData2 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                copyPasteBuilder1.SelectOption = CopyPasteBuilder.ParentSelectOption.SmartObject;
                Section section3;
                section3 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                Section section4;
                section4 = workPart.Sections.CreateSection(0.00038000000000000002, 0.00040000000000000002, 0.5);
                section3.Destroy();
                section4.Destroy();
                copyPasteBuilder1.UpdateBuilder();
                MatchedReferenceBuilder[] matchedReferenceData3;
                matchedReferenceData3 = featureReferencesBuilder1.GetMatchedReferences();
                featureReferencesBuilder1.AutomaticMatch(false);
                copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData3[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                ScCollector scCollector1;
                scCollector1 = workPart.ScCollectors.CreateCollector();
                scCollector1.AddEvaluationFilter(ScEvaluationFiltertype.SleepyEntity);
                scCollector1.SetInterpart(true);
                Body[] bodies1 = new Body[1];
                Body body1 = (Body)component1.FindObject("PROTO#.Bodies|EXTRUDE(13)");
                bodies1[0] = body1;
                BodyDumbRule bodyDumbRule1;
                bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = bodyDumbRule1;
                scCollector1.ReplaceRules(rules1, false);
                matchedReferenceData3[0].MatchedEntity = scCollector1;
                matchedReferenceData3[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser;
                featureReferencesBuilder1.AutomaticMatch(false);
                NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();
                ExtractFace extractFace2 = (ExtractFace)nXObject1;
                Expression[] expressions1;
                expressions1 = extractFace2.GetExpressions();
                copyPasteBuilder1.Destroy();
                Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(extractFace2, markId8);
                NXObject nXObject2;

                try
                {
                    Session.UndoMarkId markId9;
                    markId9 = theSession.SetUndoMark(Session.MarkVisibility.Invisible, "Start");
                    ExtractFaceBuilder extractFaceBuilder1;
                    extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(extractFace2);

                    using (session_.__UsingBuilderDestroyer(extractFaceBuilder1))
                    {
                        DisplayableObject[] replacementobjects1 = new DisplayableObject[1];
                        Component component4 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-147 2");
                        Body body2 = (Body)component4.FindObject("PROTO#.Bodies|EXTRUDE(13)");
                        replacementobjects1[0] = body2;
                        extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                        extractFaceBuilder1.Associative = false;
                        TaggedObject nullNXOpen_TaggedObject = null;
                        extractFaceBuilder1.SourcePartOccurrence = nullNXOpen_TaggedObject;
                        NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                        extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                        nXObject2 = extractFaceBuilder1.Commit();
                    }
                }
                finally
                {
                    editWithRollbackManager1.UpdateFeature(false);
                    editWithRollbackManager1.Stop();
                    theSession.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager1.Destroy();
                }

                Session.UndoMarkId markId12;
                markId12 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                ExtractFace extractFace3 = (ExtractFace)nXObject2;
                EditWithRollbackManager editWithRollbackManager2;
                editWithRollbackManager2 = workPart.Features.StartEditWithRollbackManager(extractFace3, markId12);
                NXObject nXObject3;

                try
                {
                    ExtractFaceBuilder extractFaceBuilder2;
                    extractFaceBuilder2 = workPart.Features.CreateExtractFaceBuilder(extractFace3);

                    using (session_.__UsingBuilderDestroyer(extractFaceBuilder2))
                    {
                        DisplayableObject[] replacementobjects2 = new DisplayableObject[0];
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects2);
                        Body[] bodies2 = new Body[1];
                        bodies2[0] = body1;
                        BodyDumbRule bodyDumbRule2;
                        bodyDumbRule2 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies2, true);
                        SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                        rules2[0] = bodyDumbRule2;
                        extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules2, false);
                        DisplayableObject[] replacementobjects3 = new DisplayableObject[0];
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects3);
                        DisplayableObject[] replacementobjects4 = new DisplayableObject[1];
                        replacementobjects4[0] = body1;
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects4);
                        Part part2 = (Part)theSession.Parts.FindObject("001449-010-147");
                        EdgeBlend edgeBlend1 = (EdgeBlend)part2.Features.FindObject("BLEND(16)");
                        extractFaceBuilder2.FrecAtTimeStamp = edgeBlend1;
                        extractFaceBuilder2.Associative = true;
                        extractFaceBuilder2.InheritDisplayProperties = true;
                        extractFaceBuilder2.SourcePartOccurrence = null;
                        NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects2 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                        extractFaceBuilder2.SetProductInterfaceObjects(selectedobjects2);
                        nXObject3 = extractFaceBuilder2.Commit();
                    }
                }
                finally
                {
                    editWithRollbackManager2.UpdateFeature(false);
                    editWithRollbackManager2.Stop();
                    theSession.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager2.Destroy();
                }

                Session.UndoMarkId markId16;
                markId16 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-109
                Session.UndoMarkId markId17;
                markId17 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");
                __work_component_ = component3;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(86)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);
                copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                FeatureReferencesBuilder featureReferencesBuilder2;
                featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                featureReferencesBuilder2.AutomaticMatch(true);
                MatchedReferenceBuilder[] matchedReferenceData4;
                matchedReferenceData4 = featureReferencesBuilder2.GetMatchedReferences();
                copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                copyPasteBuilder2.CopyResolveGeometry = true;
                copyPasteBuilder2.Associative = true;
                ScCollector scCollector2;
                scCollector2 = workPart.ScCollectors.CreateCollector();
                Feature[] features3 = new Feature[1];
                ExtractFace extractFace4 = (ExtractFace)nXObject3;
                features3[0] = extractFace4;
                BodyFeatureRule bodyFeatureRule1;
                bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features3, true, component3);
                SelectionIntentRule[] rules3 = new SelectionIntentRule[1];
                rules3[0] = bodyFeatureRule1;
                scCollector2.ReplaceRules(rules3, false);
                matchedReferenceData4[0].MatchedEntity = scCollector2; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                matchedReferenceData4[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[0] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                featureReferencesBuilder2.AutomaticMatch(false);
                Body body3 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].MatchedEntity = body3;
                matchedReferenceData4[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                matchedReferenceData4[1].ReverseDirection = false; // WARNING: use of matchedReferenceData4[1] may be unreliable on replay
                featureReferencesBuilder2.AutomaticMatch(false);
                copyPasteBuilder2.CopyResolveGeometry = false;
                copyPasteBuilder2.Associative = false;
                NXObject nXObject4;
                nXObject4 = copyPasteBuilder2.Commit();
                BooleanFeature booleanFeature2 = (BooleanFeature)nXObject4;
                Expression[] expressions2;
                expressions2 = booleanFeature2.GetExpressions();
                copyPasteBuilder2.Destroy();
                Component nullNXOpen_Assemblies_Component = null;
                PartLoadStatus partLoadStatus5;
                theSession.Parts.SetWorkComponent(nullNXOpen_Assemblies_Component, PartCollection.RefsetOption.Current, PartCollection.WorkComponentOption.Visible, out partLoadStatus5);
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                partLoadStatus5.Dispose();
                Component[] components2 = new Component[1];
                components2[0] = component1;
                ErrorList errorList2;
                errorList2 = workPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components2);
                errorList2.Dispose();
            }

            __work_part_ = __display_part_;

            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                Part displayPart = theSession.Parts.Display;
                Component[] components1 = new Component[1];
                Component component1 = (Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-106 1");
                component1.__ReferenceSet("SUBTOOL");
                Component component2 = frComp;
                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-109
                Component component3 = toComp;
                __work_component_ = component3;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(66)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
                ExtractFace extractFace2;

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                {
                    copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder1;
                    featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                    featureReferencesBuilder1.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData1;
                    matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                    copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                    NXObject nXObject1;
                    nXObject1 = copyPasteBuilder1.Commit();
                    extractFace2 = (ExtractFace)nXObject1;
                    Expression[] expressions1;
                    expressions1 = extractFace2.GetExpressions();
                }
                Session.UndoMarkId markId8 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(extractFace2, markId8);
                NXObject nXObject2;

                try
                {
                    ExtractFaceBuilder extractFaceBuilder1 = workPart.Features.CreateExtractFaceBuilder(extractFace2);

                    using (session_.__UsingBuilderDestroyer(extractFaceBuilder1))
                    {
                        DisplayableObject[] replacementobjects1 = new DisplayableObject[1];
                        Component component4 = (Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-106 2");
                        Body body1 = (Body)component4.FindObject("PROTO#.Bodies|CYLINDER(44)");
                        replacementobjects1[0] = body1;
                        extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                        extractFaceBuilder1.Associative = false;
                        extractFaceBuilder1.SourcePartOccurrence = null;
                        NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                        extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                        nXObject2 = extractFaceBuilder1.Commit();
                    }
                }
                finally
                {

                    editWithRollbackManager1.UpdateFeature(false);
                    editWithRollbackManager1.Stop();
                    theSession.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager1.Destroy();
                }

                var markId12 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                ExtractFace extractFace3 = (ExtractFace)nXObject2;
                EditWithRollbackManager editWithRollbackManager2;
                editWithRollbackManager2 = workPart.Features.StartEditWithRollbackManager(extractFace3, markId12);
                NXObject nxObject3;
                try
                {
                    ExtractFaceBuilder extractFaceBuilder2 = workPart.Features.CreateExtractFaceBuilder(extractFace3);

                    using (session_.__UsingBuilderDestroyer(extractFaceBuilder2))
                    {

                        DisplayableObject[] replacementobjects2 = new DisplayableObject[0];
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects2);
                        Body[] bodies1 = new Body[1];
                        Body body2 = (Body)component1.FindObject("PROTO#.Bodies|CYLINDER(44)");
                        bodies1[0] = body2;
                        BodyDumbRule bodyDumbRule1;
                        bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                        SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                        rules1[0] = bodyDumbRule1;
                        extractFaceBuilder2.ExtractBodyCollector.ReplaceRules(rules1, false);
                        DisplayableObject[] replacementobjects3 = new DisplayableObject[0];
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects3);
                        DisplayableObject[] replacementobjects4 = new DisplayableObject[1];
                        replacementobjects4[0] = body2;
                        extractFaceBuilder2.ReplacementAssistant.SetNewParents(replacementobjects4);
                        Part part2 = (Part)theSession.Parts.FindObject("001449-010-106");
                        Chamfer chamfer1 = (Chamfer)part2.Features.FindObject("CHAMFER(60)");
                        extractFaceBuilder2.FrecAtTimeStamp = chamfer1;
                        extractFaceBuilder2.Associative = true;
                        extractFaceBuilder2.InheritDisplayProperties = true;
                        extractFaceBuilder2.SourcePartOccurrence = null;
                        NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects2 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                        extractFaceBuilder2.SetProductInterfaceObjects(selectedobjects2);
                        nxObject3 = extractFaceBuilder2.Commit();
                    }
                }
                finally
                {

                    editWithRollbackManager2.UpdateFeature(false);
                    editWithRollbackManager2.Stop();
                    theSession.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager2.Destroy();
                }

                __work_component_ = component2;
                workPart = theSession.Parts.Work; // 001449-010-109
                __work_component_ = component3;
                workPart = theSession.Parts.Work; // 001449-010-900
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(68)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = workPart.Features.CreateCopyPasteBuilder2(features2);

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder2))
                {
                    copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                    featureReferencesBuilder2.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData2 = featureReferencesBuilder2.GetMatchedReferences();
                    copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    copyPasteBuilder2.CopyResolveGeometry = true;
                    copyPasteBuilder2.Associative = true;
                    ScCollector scCollector1 = workPart.ScCollectors.CreateCollector();
                    Feature[] features3 = new Feature[1];
                    ExtractFace extractFace4 = (ExtractFace)nxObject3;
                    features3[0] = extractFace4;
                    BodyFeatureRule bodyFeatureRule1;
                    bodyFeatureRule1 = workPart.ScRuleFactory.CreateRuleBodyFeature(features3, true, component3);
                    SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                    rules2[0] = bodyFeatureRule1;
                    scCollector1.ReplaceRules(rules2, false);
                    matchedReferenceData2[0].MatchedEntity = scCollector1; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    featureReferencesBuilder2.AutomaticMatch(false);
                    Body body3 = (Body)workPart.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    matchedReferenceData2[1].MatchedEntity = body3;
                    matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    matchedReferenceData2[1].ReverseDirection = false; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    featureReferencesBuilder2.AutomaticMatch(false);
                    copyPasteBuilder2.CopyResolveGeometry = false;
                    copyPasteBuilder2.Associative = false;
                    NXObject nXObject4;
                    nXObject4 = copyPasteBuilder2.Commit();
                    BooleanFeature booleanFeature2 = (BooleanFeature)nXObject4;
                    Expression[] expressions2;
                    expressions2 = booleanFeature2.GetExpressions();
                }

                __work_component_ = null;
                workPart = theSession.Parts.Work; // 001449-010-lsp1
                component1.__ReferenceSet("BODY");
            }

            __work_part_ = __display_part_;

            {
                Component component1 = frComp;
                __work_component_ = component1;
                __work_part_ = session_.Parts.Work; // 001449-010-109
                Component component2 = toComp;
                __work_component_ = component2;
                __work_part_ = session_.Parts.Work; // 001449-010-900
                NXObject[] features1 = new NXObject[1];
                Part part1 = frComp.__Prototype();
                ExtractFace extractFace1 = (ExtractFace)part1.Features.FindObject("LINKED_BODY(69)");
                features1[0] = extractFace1;
                CopyPasteBuilder copyPasteBuilder1;
                copyPasteBuilder1 = __work_part_.Features.CreateCopyPasteBuilder2(features1);
                copyPasteBuilder1.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                ExtractFace extractFace2;

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder1))
                {
                    FeatureReferencesBuilder featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
                    featureReferencesBuilder1.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
                    copyPasteBuilder1.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.BySystem;
                    matchedReferenceData1[0].MatchedEntity = null;
                    matchedReferenceData1[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Unresolved;
                    copyPasteBuilder1.CopyResolveGeometry = true;
                    NXObject nXObject1 = copyPasteBuilder1.Commit();
                    extractFace2 = (ExtractFace)nXObject1;
                    Expression[] expressions1 = extractFace2.GetExpressions();
                }

                Body[] bodies1 = new Body[1];
                Component component3 = (Component)__display_part_.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-146 1");
                Component component4 = (Component)component3.FindObject("COMPONENT 8mm-shcs-020 2");
                NXObject nXObject2;
                var markId6 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Redefine Feature");
                EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = __work_part_.Features.StartEditWithRollbackManager(extractFace2, markId6);

                try
                {
                    ExtractFaceBuilder extractFaceBuilder1;
                    extractFaceBuilder1 = __work_part_.Features.CreateExtractFaceBuilder(extractFace2);

                    using (session_.__UsingBuilderDestroyer(extractFaceBuilder1))
                    {
                        DisplayableObject[] replacementobjects1 = new DisplayableObject[0];
                        extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects1);
                        Body body1 = (Body)component4.FindObject("PROTO#.Bodies|CYLINDER(9)");
                        bodies1[0] = body1;
                        BodyDumbRule bodyDumbRule1;
                        bodyDumbRule1 = __work_part_.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);
                        SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                        rules1[0] = bodyDumbRule1;
                        extractFaceBuilder1.ExtractBodyCollector.ReplaceRules(rules1, false);
                        DisplayableObject[] replacementobjects2 = new DisplayableObject[0];
                        extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects2);
                        DisplayableObject[] replacementobjects3 = new DisplayableObject[1];
                        replacementobjects3[0] = body1;
                        extractFaceBuilder1.ReplacementAssistant.SetNewParents(replacementobjects3);
                        Part part2 = (Part)session_.Parts.FindObject("8mm-shcs-020");
                        BodyFeature bodyFeature1 = (BodyFeature)part2.Features.FindObject("CHAMFER(12)");
                        extractFaceBuilder1.FrecAtTimeStamp = bodyFeature1;
                        extractFaceBuilder1.Associative = true;
                        extractFaceBuilder1.InheritDisplayProperties = true;
                        extractFaceBuilder1.FixAtCurrentTimestamp = false;
                        TaggedObject nullNXOpen_TaggedObject = null;
                        extractFaceBuilder1.SourcePartOccurrence = nullNXOpen_TaggedObject;
                        NXOpen.Assemblies.ProductInterface.InterfaceObject[] selectedobjects1 = new NXOpen.Assemblies.ProductInterface.InterfaceObject[0];
                        extractFaceBuilder1.SetProductInterfaceObjects(selectedobjects1);
                        nXObject2 = extractFaceBuilder1.Commit();
                    }
                }
                finally
                {
                    editWithRollbackManager1.UpdateFeature(false);
                    editWithRollbackManager1.Stop();
                    session_.Preferences.Modeling.UpdatePending = false;
                    editWithRollbackManager1.Destroy();
                }

                __work_component_ = component1;
                __work_part_ = session_.Parts.Work; // 001449-010-109
                __work_component_ = component2;
                __work_part_ = session_.Parts.Work; // 001449-010-900
                NXObject[] features2 = new NXObject[1];
                BooleanFeature booleanFeature1 = (BooleanFeature)part1.Features.FindObject("SUBTRACT(72)");
                features2[0] = booleanFeature1;
                CopyPasteBuilder copyPasteBuilder2;
                copyPasteBuilder2 = __work_part_.Features.CreateCopyPasteBuilder2(features2);

                using (session_.__UsingBuilderDestroyer(copyPasteBuilder2))
                {
                    copyPasteBuilder2.SetBuilderVersion((CopyPasteBuilder.BuilderVersion)7);
                    FeatureReferencesBuilder featureReferencesBuilder2;
                    featureReferencesBuilder2 = copyPasteBuilder2.GetFeatureReferences();
                    copyPasteBuilder2.CopyResolveGeometry = true;
                    featureReferencesBuilder2.AutomaticMatch(true);
                    MatchedReferenceBuilder[] matchedReferenceData2;
                    matchedReferenceData2 = featureReferencesBuilder2.GetMatchedReferences();
                    copyPasteBuilder2.ExpressionOption = CopyPasteBuilder.ExpressionTransferOption.CreateNew;
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.Initial; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    copyPasteBuilder2.Associative = true;
                    ScCollector scCollector1;
                    scCollector1 = __work_part_.ScCollectors.CreateCollector();
                    Feature[] features3 = new Feature[1];
                    ExtractFace extractFace3 = (ExtractFace)nXObject2;
                    features3[0] = extractFace3;
                    BodyFeatureRule bodyFeatureRule1;
                    bodyFeatureRule1 = __work_part_.ScRuleFactory.CreateRuleBodyFeature(features3, true, component2);
                    SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
                    rules2[0] = bodyFeatureRule1;
                    scCollector1.ReplaceRules(rules2, false);
                    matchedReferenceData2[0].MatchedEntity = scCollector1; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    matchedReferenceData2[0].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[0] may be unreliable on replay
                    Body body2 = (Body)__work_part_.Bodies.FindObject("BLOCK(0)"); // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    matchedReferenceData2[1].MatchedEntity = body2;
                    matchedReferenceData2[1].MatchedStatus = MatchedReferenceBuilder.ResolvedStatus.ByUser; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    matchedReferenceData2[1].ReverseDirection = false; // WARNING: use of matchedReferenceData2[1] may be unreliable on replay
                    featureReferencesBuilder2.AutomaticMatch(false);
                    copyPasteBuilder2.CopyResolveGeometry = false;
                    copyPasteBuilder2.Associative = false;
                    NXObject nXObject3;
                    nXObject3 = copyPasteBuilder2.Commit();
                    BooleanFeature booleanFeature2 = (BooleanFeature)nXObject3;
                    Expression[] expressions2;
                    expressions2 = booleanFeature2.GetExpressions();
                }
                Component[] components1 = new Component[1];
                components1[0] = component2;
                __work_component_ = null;
                __work_part_ = session_.Parts.Work; // 001449-010-lsp1
                Component[] components2 = new Component[1];
                components2[0] = component3;
                Component[] components3 = new Component[1];
                components3[0] = component4;
                ErrorList errorList1;
                errorList1 = __work_part_.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", components3);
                errorList1.Dispose();
            }

            __work_part_ = __display_part_;
        }

    }
}// 7642