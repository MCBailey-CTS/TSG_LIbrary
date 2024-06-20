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
        public static void Mirror()
        {
            try
            {
                var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
                var toComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("902"));

                // chamfer
                var frChamfer = (ApexRangeChamfer)frComp.__Prototype().Features.ToArray()[1];

                ApexRangeChamferBuilder chamferBuilder = frComp.__Prototype().Features.DetailFeatureCollection.CreateApexRangeChamferBuilder((ApexRangeChamfer)frChamfer);

                TaggedObject[] objects;

                using (new Destroyer(chamferBuilder))
                    objects = chamferBuilder.EdgeManager.EdgeChainSetList
                        .GetContents()
                        .SelectMany(k => k.Edges.GetObjects())
                        .ToArray();


                foreach (TaggedObject obj in objects)
                    switch (obj)
                    {
                        case Edge frEdge:
                            {
                                Session theSession = NXOpen.Session.GetSession();
                                Part workPart = theSession.Parts.Work;
                                Part displayPart = theSession.Parts.Display;
                                NXOpen.Assemblies.Component component1 = frComp;
                                //PartLoadStatus partLoadStatus1;
                                //theSession.Parts.SetWorkComponent(component1, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus1);

                                __work_component_ = component1;
                                //workPart = theSession.Parts.Work; // 001449-010-109
                                //partLoadStatus1.Dispose();
                                //workPart.PmiManager.RestoreUnpastedObjects();
                                //Session.UndoMarkId markId2;
                                //markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");
                                NXOpen.Assemblies.Component component2 = toComp;
                                PartLoadStatus partLoadStatus2;
                                theSession.Parts.SetWorkComponent(component2, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus2);
                                workPart = theSession.Parts.Work; // 001449-010-902
                                partLoadStatus2.Dispose();
                                Session.UndoMarkId markId3;
                                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");
                                NXObject[] features1 = new NXObject[1];
                                ApexRangeChamfer apexRangeChamfer1 = frChamfer;
                                features1[0] = apexRangeChamfer1;
                                CopyPasteBuilder copyPasteBuilder1;
                                copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);

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
                                    scCollector1 = workPart.ScCollectors.CreateCollector();
                                    scCollector1.SetAllowRefCurves(false);
                                    Block block1 = ((Block)workPart.Features.FindObject("BLOCK(0)"));
                                    Edge edge1 = ((Edge)block1.FindObject("EDGE * 1 * 6 {(3.75,-8.502,-0)(1.875,-8.502,-0)(-0,-8.502,0) BLOCK(0)}"));
                                    SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                                    rules1[0] = workPart.ScRuleFactory.CreateRuleEdgeDumb(new[] {edge1 });
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