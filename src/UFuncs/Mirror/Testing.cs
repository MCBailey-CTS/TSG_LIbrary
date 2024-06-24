using NXOpen;
using NXOpen.Assemblies;
using NXOpen.CAE;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Rules;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror
{
    public static class Testing
    {

        public static CompositeCurve LinkCurve(Edge edge)
        {
            WaveLinkBuilder builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.FixAtCurrentTimestamp = true;
                builder.CompositeCurveBuilder.Associative = true;
                EdgeTangentRule rule = __work_part_.ScRuleFactory.CreateRuleEdgeTangent(edge, null, true, 0.5, false, false);
                SelectionIntentRule[] rules = new SelectionIntentRule[] { rule };
                builder.CompositeCurveBuilder.Section.AddToSection(rules, edge, null, null, _Point3dOrigin, Section.Mode.Create, false);
                return (CompositeCurve)builder.Commit();
            }
        }

        public static CompositeCurve LinkCurve(NXOpen.Curve curve)
        {
            WaveLinkBuilder builder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);

            using (session_.__UsingBuilderDestroyer(builder))
            {
                builder.FixAtCurrentTimestamp = true;
                builder.CompositeCurveBuilder.Associative = true;
                CurveDumbRule rule = __work_part_.ScRuleFactory.CreateRuleCurveDumb(new[] { curve });
                SelectionIntentRule[] rules = new SelectionIntentRule[] { rule };
                builder.CompositeCurveBuilder.Section.AddToSection(rules, curve, null, null, _Point3dOrigin, Section.Mode.Create, false);
                return (CompositeCurve)builder.Commit();
            }
        }


        public static void Mirror()
        {
            try
            {
                session_.SetUndoMark(Session.MarkVisibility.Visible, "Mirror");
                NXOpen.Assemblies.Component frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
                NXOpen.Assemblies.Component toComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("902"));
                Surface.Plane plane = new Surface.Plane(_Point3dOrigin, __Vector3dY());

                IDictionary<TaggedObject, TaggedObject> dict = new Dictionary<TaggedObject, TaggedObject>
                {
                    { frComp.__Prototype().Features.ToArray()[0], toComp.__Prototype().Features.ToArray()[0] }
                };

                // chamfer
                ApexRangeChamfer frChamfer = (ApexRangeChamfer)frComp.__Prototype().Features.ToArray()[1];

                ApexRangeChamferBuilder chamferBuilder = frComp.__Prototype().Features.DetailFeatureCollection.CreateApexRangeChamferBuilder(frChamfer);

                TaggedObject[] objects;

                using (new Destroyer(chamferBuilder))
                    objects = chamferBuilder.EdgeManager.EdgeChainSetList
                        .GetContents()
                        .SelectMany(k => k.Edges.GetObjects())
                        .ToArray();

                Edge edge = (Edge)objects[0];
                frChamfer.Suppress();

                try
                {
                    __work_component_ = null;
                    CompositeCurve linked_curve = LinkCurve((Edge)frComp.FindOccurrence(edge));
                    NXOpen.Curve entity = (NXOpen.Curve)linked_curve.GetEntities()[0];
                    entity.__RemoveParameters();
                    Transform reflect = Transform.CreateReflection(plane);
                    NXOpen.Curve mirror_ass_curve = entity.Copy(reflect);
                    entity.__Delete();
                    __work_component_ = toComp;
                    CompositeCurve toCompositeCurve = LinkCurve(mirror_ass_curve);
                    NXOpen.Line toCurve = (NXOpen.Line)toCompositeCurve.GetEntities()[0];
                    toCurve.__RemoveParameters();


                    var toDyanmicBLock = toComp.__Prototype().__DynamicBlock();

                    Edge match = null;

                    foreach (var edge1 in toDyanmicBLock.GetEdges())
                    {
                        if (edge1.SolidEdgeType != Edge.EdgeType.Linear)
                        {
                            print_($"Found non linear edge: {edge1.SolidEdgeType}");
                            continue;
                        }

                        var temp = (Line)edge1.__ToCurve();
                        //var result = ufsession_.CurveLineArc.IsLineEqual(temp.Tag, toCurve.Tag);

                        var s0 = toCurve.__StartPoint();
                        var e0 = toCurve.__EndPoint();
                        temp.__Delete();

                        if (edge1.__HasEndPoints(s0, e0))
                        {
                            match = edge1;
                            break;
                        }
                    }

                    if (match is Edge e)
                    {
                        e.SetName("IT WORKED");
                        //CopyApexChamfer(frComp, toComp, e, frChamfer);
                        
                    }



                }
                finally
                {
                    frChamfer.Unsuppress();
                }


            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }




        }



        public static void CopyApexChamfer(Component frComp, Component toComp, Edge edge1, ApexRangeChamfer apexRangeChamfer1)
        {
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;
            NXOpen.Part displayPart = theSession.Parts.Display;
            workPart.PmiManager.RestoreUnpastedObjects();
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");
            NXOpen.Assemblies.Component component1 = frComp;
            NXOpen.PartLoadStatus partLoadStatus1;
            theSession.Parts.SetWorkComponent(component1, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus1);
            workPart = theSession.Parts.Work; // 001449-010-902
            partLoadStatus1.Dispose();
            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");
            NXOpen.NXObject[] features1 = new NXOpen.NXObject[1];
            //NXOpen.Part part1 = ((NXOpen.Part)theSession.Parts.FindObject("001449-010-109"));
            //NXOpen.Features.ApexRangeChamfer apexRangeChamfer1 = ((NXOpen.Features.ApexRangeChamfer)part1.Features.FindObject("CHAMFER(60)"));
            features1[0] = apexRangeChamfer1;
            NXOpen.Features.CopyPasteBuilder copyPasteBuilder1;
            copyPasteBuilder1 = workPart.Features.CreateCopyPasteBuilder2(features1);
            copyPasteBuilder1.SetBuilderVersion((NXOpen.Features.CopyPasteBuilder.BuilderVersion)(7));
            NXOpen.Features.FeatureReferencesBuilder featureReferencesBuilder1;
            featureReferencesBuilder1 = copyPasteBuilder1.GetFeatureReferences();
            NXOpen.Point3d origin1 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            NXOpen.Vector3d normal1 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            NXOpen.Plane plane1;
            plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);
            NXOpen.Unit unit1 = ((NXOpen.Unit)workPart.UnitCollection.FindObject("Inch"));
            NXOpen.Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
            NXOpen.Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);
            featureReferencesBuilder1.AutomaticMatch(true);
            theSession.SetUndoMarkName(markId2, "Paste Feature Dialog");
            NXOpen.Features.MatchedReferenceBuilder[] matchedReferenceData1;
            matchedReferenceData1 = featureReferencesBuilder1.GetMatchedReferences();
            NXOpen.Point3d origin2 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            NXOpen.Vector3d normal2 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            NXOpen.Plane plane2;
            plane2 = workPart.Planes.CreatePlane(origin2, normal2, NXOpen.SmartObject.UpdateOption.WithinModeling);
            NXOpen.Point3d origin3 = new NXOpen.Point3d(0.0, 0.0, 0.0);
            NXOpen.Vector3d normal3 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            NXOpen.Plane plane3;
            plane3 = workPart.Planes.CreatePlane(origin3, normal3, NXOpen.SmartObject.UpdateOption.WithinModeling);
            copyPasteBuilder1.ExpressionOption = NXOpen.Features.CopyPasteBuilder.ExpressionTransferOption.CreateNew;
            matchedReferenceData1[0].MatchedStatus = NXOpen.Features.MatchedReferenceBuilder.ResolvedStatus.Initial;
            copyPasteBuilder1.CopyResolveGeometry = true;
            copyPasteBuilder1.Associative = true;
            NXOpen.ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();
            scCollector1.SetAllowRefCurves(false);
            //NXOpen.Features.Block block1 = ((NXOpen.Features.Block)workPart.Features.FindObject("BLOCK(0)"));
            //NXOpen.Edge edge1 = ((NXOpen.Edge)block1.FindObject("EDGE * 1 * 6 {(3.75,-8.502,-0)(1.875,-8.502,-0)(-0,-8.502,0) BLOCK(0)}"));
            NXOpen.Edge nullNXOpen_Edge = null;
            NXOpen.EdgeTangentRule edgeTangentRule1;
            edgeTangentRule1 = workPart.ScRuleFactory.CreateRuleEdgeTangent(edge1, nullNXOpen_Edge, false, 0.5, false, false);
            NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
            rules1[0] = edgeTangentRule1;
            scCollector1.ReplaceRules(rules1, false);
            matchedReferenceData1[0].MatchedEntity = scCollector1;
            matchedReferenceData1[0].MatchedStatus = NXOpen.Features.MatchedReferenceBuilder.ResolvedStatus.ByUser;
            featureReferencesBuilder1.AutomaticMatch(false);
            copyPasteBuilder1.CopyResolveGeometry = false;
            copyPasteBuilder1.Associative = false;
            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");
            theSession.DeleteUndoMark(markId3, null);
            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");
            NXOpen.TaggedObject taggedObject1;
            NXOpen.TaggedObject taggedObject2;
            NXOpen.TaggedObject taggedObject3;
            //NXOpen.Body body1 = ((NXOpen.Body)taggedObject3);
            //NXOpen.BasePart basePart1;
            //basePart1 = body1.OwningPart;
            //NXOpen.UserDefinedObjects.UserDefinedObject userDefinedObject1;
            //int[] integers1;
            //integers1 = userDefinedObject1.GetIntegers();
            //NXOpen.Part part2;
            //part2 = theSession.Parts.Work;
            //NXOpen.Part part3;
            //part3 = theSession.Parts.Display;
            //NXOpen.BasePart.Units units1;
            //units1 = workPart.PartUnits;
            //string featureType1;
            //featureType1 = block1.FeatureType;
            //string name1;
            //name1 = block1.Name;
            //NXOpen.Body[] bodies1;
            //bodies1 = block1.GetBodies();
            //NXOpen.Features.BlockFeatureBuilder blockFeatureBuilder1;
            //blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(block1);
            //NXOpen.Vector3d xAxis1;
            //NXOpen.Vector3d yAxis1;
            //blockFeatureBuilder1.GetOrientation(out xAxis1, out yAxis1);
            //NXOpen.Point3d origin4;
            //origin4 = blockFeatureBuilder1.Origin;
            //NXOpen.Point3d origin5;
            //origin5 = blockFeatureBuilder1.Origin;
            //NXOpen.Point3d origin6;
            //origin6 = blockFeatureBuilder1.Origin;

            //NXOpen.NXObject nXObject1;
            //nXObject1 = copyPasteBuilder1.Commit();

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "Paste Feature");

            //NXOpen.Features.ApexRangeChamfer apexRangeChamfer3 = ((NXOpen.Features.ApexRangeChamfer)nXObject1);
            //NXOpen.Expression[] expressions1;
            //expressions1 = apexRangeChamfer3.GetExpressions();

            copyPasteBuilder1.Destroy();

            //try
            //{
            //    // Expression is still in use.
            //    workPart.Expressions.Delete(expression2);
            //}
            //catch (NXException ex)
            //{
            //    ex.AssertErrorCode(1050029);
            //}

            //try
            //{
            //    // Expression is still in use.
            //    workPart.Expressions.Delete(expression1);
            //}
            //catch (NXException ex)
            //{
            //    ex.AssertErrorCode(1050029);
            //}

            //plane1.DestroyPlane();

            // ----------------------------------------------
            //   Menu: Tools->Journal->Pause Recording
            // ----------------------------------------------

        }


    }
}