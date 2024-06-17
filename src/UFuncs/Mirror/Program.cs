using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.CAE.Xyplot;
using NXOpen.Features;
using TSG_Library.Geom;
using TSG_Library.UFuncs.Mirror.Features;
using TSG_Library.UFuncs.MirrorComponents.Features;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs.Mirror
{
    public class Program
    {
        public static void Mirror208_3001(Surface.Plane plane, Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component component = (Component)dict[originalComp];
            try
            {
                IMirrorFeature[] source = new IMirrorFeature[9]
                {
                    new MirrorLinkedBody(),
                    new MirrorEdgeBlend(),
                    new MirrorChamfer(),
                    new MirrorSubtract(),
                    new MirrorIntersect(),
                    new MirrorUnite(),
                    new MirrorBlock(),
                    new MirrorExtractedBody(),
                    new MirrorExtrude()
                };
                Part part = originalComp.__Prototype();
                Part part2 = component.__Prototype();
                originalComp.__Prototype().Features.SuppressFeatures(originalComp.__Prototype().Features.GetFeatures());
                component.__Prototype().Features.SuppressFeatures(component.__Prototype().Features.GetFeatures());
                Body[] array = part.Bodies.ToArray();
                Body[] array2 = part2.Bodies.ToArray();
                for (int i = 0; i < array.Length; i++) dict.Add(array[i], array2[i]);

                foreach (Feature originalFeature in part.Features)
                    try
                    {
                        IMirrorFeature mirrorFeature =
                            source.SingleOrDefault(mirror => mirror.FeatureType == originalFeature.FeatureType);
                        if (mirrorFeature != null)
                        {
                            mirrorFeature.Mirror(originalFeature, dict, plane, originalComp);
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

                __work_part_ = __display_part_;
                foreach (Part item in __display_part_.__DescendantParts())
                    foreach (Expression expression3 in item.Expressions)
                    {
                        if (expression3.Status != Expression.StatusOption.Broken) continue;

                        Expression[] array3 = expression3.__OwningPart().Expressions.ToArray();
                        foreach (Expression expression2 in array3)
                            if (expression2.Tag != expression3.Tag && expression3.Name == expression2.RightHandSide)
                                item.Expressions.Delete(expression2);

                        item.Expressions.Delete(expression3);
                        print_("Deleted broken expression \"" + expression3.Name + "\" in part \"" +
                               expression3.__OwningPart().Leaf + "\"");
                    }
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
            var plane = new Geom.Surface.Plane(_Point3dOrigin, __Vector3dY());
            var frComp = __display_part_.__RootComponent().GetChildren().Single(c => c.DisplayName.EndsWith("109"));
            frComp.__SetWcsToComponent();
            var toOrigin = frComp.__Origin().__Mirror(plane);
            var toOrientation = frComp.__Orientation().__Mirror(plane);
            string toFilePath = "H:\\CTS\\001449 (mirror)\\001449-010\\001449-010-900.prt";
            var toPart = session_.__New(toFilePath, frComp.__Prototype().PartUnits);
            var toComp = __display_part_.__AddComponent(toPart, origin: toOrigin, orientation: toOrientation);


            IDictionary<TaggedObject, TaggedObject> mirrorDict = new Dictionary<TaggedObject, TaggedObject>();


            // Mirrors Csys
            {
                NXOpen.Session theSession = NXOpen.Session.GetSession();
                NXOpen.Part workPart = theSession.Parts.Work;
                NXOpen.Part displayPart = theSession.Parts.Display;
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");

                NXOpen.Assemblies.Component component1 = ((NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1"));
                NXOpen.PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus1);

                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                // ----------------------------------------------
                //   Menu: Edit->Copy
                // ----------------------------------------------
                workPart.PmiManager.RestoreUnpastedObjects();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Copy");

                NXOpen.Gateway.CopyCutBuilder copyCutBuilder1;
                copyCutBuilder1 = workPart.ClipboardOperationsManager.CreateCopyCutBuilder();

                copyCutBuilder1.CanCopyAsSketch = true;

                copyCutBuilder1.IsCut = false;

                copyCutBuilder1.ToClipboard = true;

                copyCutBuilder1.DestinationFilename = null;

                NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1];
                NXOpen.CartesianCoordinateSystem frCsys = ((NXOpen.CartesianCoordinateSystem)component1.FindObject("PROTO#HANDLE R-19764"));
                objects1[0] = frCsys;
                copyCutBuilder1.SetObjects(objects1);

                NXOpen.NXObject nXObject1;
                nXObject1 = copyCutBuilder1.Commit();

                copyCutBuilder1.Destroy();

                theSession.DeleteUndoMark(markId2, null);

                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");

                NXOpen.Assemblies.Component component2 = ((NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1"));
                NXOpen.PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus2);

                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                // ----------------------------------------------
                //   Menu: Edit->Paste
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Paste");

                NXOpen.Gateway.PasteBuilder pasteBuilder1;
                pasteBuilder1 = workPart.ClipboardOperationsManager.CreatePasteBuilder();

                NXOpen.NXObject nXObject2;
                nXObject2 = pasteBuilder1.Commit();


                var toCsys = (CoordinateSystem)pasteBuilder1.GetCommittedObjects()[0];

                pasteBuilder1.Destroy();

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------


                var toOrigin_ = frCsys.__Origin().__MirrorMap(plane, frComp, toComp);

                toCsys.Origin = toOrigin_;
                toCsys.RedisplayObject();
                ufsession_.Modl.Update();

            }



            __work_part_ = __display_part_;

            // mirrors block
            {
                NXOpen.Session theSession = NXOpen.Session.GetSession();
                NXOpen.Part workPart = __work_part_;
                NXOpen.Part displayPart = theSession.Parts.Display;
                NXOpen.Session.UndoMarkId markId1;
                markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");

                NXOpen.Assemblies.Component component1 = ((NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-109 1"));
                NXOpen.PartLoadStatus partLoadStatus1;
                theSession.Parts.SetWorkComponent(component1, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus1);

                workPart = theSession.Parts.Work; // 001449-010-109
                partLoadStatus1.Dispose();
                // ----------------------------------------------
                //   Menu: Edit->Copy
                // ----------------------------------------------
                workPart.PmiManager.RestoreUnpastedObjects();

                NXOpen.Session.UndoMarkId markId2;
                markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Work Part");

                NXOpen.Assemblies.Component component2 = ((NXOpen.Assemblies.Component)displayPart.ComponentAssembly.RootComponent.FindObject("COMPONENT 001449-010-900 1"));
                NXOpen.PartLoadStatus partLoadStatus2;
                theSession.Parts.SetWorkComponent(component2, NXOpen.PartCollection.RefsetOption.Current, NXOpen.PartCollection.WorkComponentOption.Visible, out partLoadStatus2);

                workPart = theSession.Parts.Work; // 001449-010-900
                partLoadStatus2.Dispose();
                theSession.SetUndoMarkName(markId2, "Make Work Part");

                // ----------------------------------------------
                //   Menu: Edit->Paste
                // ----------------------------------------------
                NXOpen.Session.UndoMarkId markId3;
                markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

                NXOpen.NXObject[] features1 = new NXOpen.NXObject[1];
                NXOpen.Part part1 = ((NXOpen.Part)theSession.Parts.FindObject("001449-010-109"));
                NXOpen.Features.Block block1 = ((NXOpen.Features.Block)part1.Features.FindObject("BLOCK(0)"));
                features1[0] = block1;
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

                theSession.SetUndoMarkName(markId3, "Paste Feature Dialog");

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

                NXOpen.Expression expression3;
                expression3 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                NXOpen.Session.UndoMarkId markId4;
                markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");

                theSession.DeleteUndoMark(markId4, null);

                NXOpen.Session.UndoMarkId markId5;
                markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Paste Feature");

                NXOpen.NXObject nXObject1;
                nXObject1 = copyPasteBuilder1.Commit();

                theSession.DeleteUndoMark(markId5, null);

                theSession.SetUndoMarkName(markId3, "Paste Feature");

                NXOpen.Features.Block block2 = ((NXOpen.Features.Block)nXObject1);
                NXOpen.Expression[] expressions1;
                expressions1 = block2.GetExpressions();

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

                plane1.DestroyPlane();

                workPart.Expressions.Delete(expression3);

                //NXOpen.Point3d scaleAboutPoint1 = new NXOpen.Point3d(12.5711226232368, 3.9973344970514826, 0.0);
                //NXOpen.Point3d viewCenter1 = new NXOpen.Point3d(-12.571122623236878, -3.9973344970519444, 0.0);
                //displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.80000000000000004, scaleAboutPoint1, viewCenter1);

                //NXOpen.Point3d scaleAboutPoint2 = new NXOpen.Point3d(14.301417591041867, 4.6435466993133749, 0.0);
                //NXOpen.Point3d viewCenter2 = new NXOpen.Point3d(-14.30141759104195, -4.6435466993138332, 0.0);
                //displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.80000000000000004, scaleAboutPoint2, viewCenter2);

                //NXOpen.Point3d scaleAboutPoint3 = new NXOpen.Point3d(17.258809500300529, 5.6720128408913935, 0.0);
                //NXOpen.Point3d viewCenter3 = new NXOpen.Point3d(-17.258809500300615, -5.6720128408918455, 0.0);
                //displayPart.ModelingViews.WorkView.ZoomAboutPoint(0.80000000000000004, scaleAboutPoint3, viewCenter3);

                //NXOpen.Point3d scaleAboutPoint4 = new NXOpen.Point3d(23.118418096630208, 11.669559492690217, 0.0);
                //NXOpen.Point3d viewCenter4 = new NXOpen.Point3d(-23.11841809663029, -11.66955949269067, 0.0);
                //displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint4, viewCenter4);

                //NXOpen.Point3d scaleAboutPoint5 = new NXOpen.Point3d(18.891996077055314, 9.688769016153163, 0.0);
                //NXOpen.Point3d viewCenter5 = new NXOpen.Point3d(-18.891996077055399, -9.688769016153616, 0.0);
                //displayPart.ModelingViews.WorkView.ZoomAboutPoint(1.25, scaleAboutPoint5, viewCenter5);

                NXOpen.Session.UndoMarkId markId6;
                markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Redefine Feature");

                NXOpen.Features.EditWithRollbackManager editWithRollbackManager1;
                editWithRollbackManager1 = workPart.Features.StartEditWithRollbackManager(block2, markId6);

                NXOpen.Session.UndoMarkId markId7;
                markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

                NXOpen.Features.BlockFeatureBuilder blockFeatureBuilder1;
                blockFeatureBuilder1 = workPart.Features.CreateBlockFeatureBuilder(block2);

                blockFeatureBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

                NXOpen.Body[] targetBodies1 = new NXOpen.Body[1];
                NXOpen.Body nullNXOpen_Body = null;
                targetBodies1[0] = nullNXOpen_Body;
                blockFeatureBuilder1.BooleanOption.SetTargetBodies(targetBodies1);

                theSession.SetUndoMarkName(markId7, "Block Dialog");

                // ----------------------------------------------
                //   Dialog Begin Block
                // ----------------------------------------------
                NXOpen.Expression expression4;
                expression4 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

                NXOpen.Session.UndoMarkId markId8;
                markId8 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                theSession.DeleteUndoMark(markId8, null);

                NXOpen.Session.UndoMarkId markId9;
                markId9 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Block");

                blockFeatureBuilder1.Type = NXOpen.Features.BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                theSession.UpdateManager.LogForUpdate(blockFeatureBuilder1.Length);

                theSession.UpdateManager.LogForUpdate(blockFeatureBuilder1.Width);

                NXOpen.Point point1;
                point1 = blockFeatureBuilder1.OriginPoint;

                blockFeatureBuilder1.OriginPoint = point1;

                NXOpen.Point3d originPoint1 = new NXOpen.Point3d(0.0, 0.0, -2.0);
                blockFeatureBuilder1.SetOriginAndLengths(originPoint1, "p5=8.5", "p7=3.75", "p9=2");

                blockFeatureBuilder1.SetBooleanOperationAndTarget(NXOpen.Features.Feature.BooleanType.Create, nullNXOpen_Body);

                NXOpen.Features.Feature feature1;
                feature1 = blockFeatureBuilder1.CommitFeature();

                theSession.DeleteUndoMark(markId9, null);

                theSession.SetUndoMarkName(markId7, "Block");

                NXOpen.Expression expression5 = blockFeatureBuilder1.Width;
                NXOpen.Expression expression6 = blockFeatureBuilder1.Length;
                NXOpen.Expression expression7 = blockFeatureBuilder1.Height;
                blockFeatureBuilder1.Destroy();

                workPart.Expressions.Delete(expression4);

                theSession.DeleteUndoMark(markId7, null);

                editWithRollbackManager1.UpdateFeature(false);

                editWithRollbackManager1.Stop();

                editWithRollbackManager1.Destroy();

                // ----------------------------------------------
                //   Menu: Tools->Journal->Stop Recording
                // ----------------------------------------------

            }



            __work_part_ = __display_part_;



        }

    }
}