using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_copy_to_layer_50)]
    //[RevisionLog("Copy to Layer 50")]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class CopyToLayer50 : _UFunc
    {
        private const int LAYER = 50;

        public override void execute()
        {
            Component[] compList = Selection.SelectManyComponents();

            using (session_.__usingDisplayPartReset())
            {
                foreach (Component comp in compList)
                {
                    print_("/////////////////");
                    Part compPart = (Part)comp.Prototype;

                    BasePart.Units units = compPart.PartUnits;

                    __work_component_ = comp;

                    // if body exists on layer 50, delete body

                    Body[] bodies_on_layer_50 = comp.__Prototype().Bodies
                        .ToArray()
                        .Where(__b => __b.IsSolidBody)
                        .Where(__b => __b.Layer == LAYER)
                        .ToArray();

                    Body[] solid_bodies_layer_1 = comp.__Prototype().Bodies
                        .ToArray()
                        .Where(__b => __b.IsSolidBody)
                        .Where(__b => __b.Layer == 1)
                        .ToArray();

                    switch (solid_bodies_layer_1.Length)
                    {
                        case 0:
                            print_($"Did not a solid body on layer 1 in part {comp.DisplayName}");
                            return;
                        case 1:
                            break;
                        default:
                            print_($"There is more than one solid body on layer 1 in part {comp.DisplayName}");
                            return;
                    }

                    string date = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

                    Body[] layer_50_bodies = comp.__Prototype()
                        .Layers
                        .GetAllObjectsOnLayer(LAYER).OfType<Body>().ToArray();
                    comp.__Prototype().Layers.CopyObjects(LAYER, solid_bodies_layer_1);
                    List<Body> new_layer_50_bodies = comp.__Prototype().Layers.GetAllObjectsOnLayer(LAYER)
                        .Except(layer_50_bodies).OfType<Body>().ToList();

                    foreach (Body body in new_layer_50_bodies)
                    {
                        body.OwningPart.Features.GetParentFeatureOfBody(body).SetName(date);
                        body.Color = 7;
                        body.LineFont = DisplayableObject.ObjectFont.Phantom;
                        body.RedisplayObject();
                    }

                    try
                    {
                        comp.__Prototype().__ReferenceSets("BODY")
                            .RemoveObjectsFromReferenceSet(new_layer_50_bodies.ToArray<NXObject>());
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }

                    comp.__Prototype().Layers.MoveDisplayableObjects(LAYER + 1, layer_50_bodies);

                    foreach (Body body in bodies_on_layer_50)
                    {
                        body.Color = 7;
                        body.LineFont = DisplayableObject.ObjectFont.Phantom;
                        body.RedisplayObject();
                        print_(
                            $"Copied solid body to {comp.__Prototype().Features.GetAssociatedFeature(body).GetFeatureName()} in part {comp.DisplayName}");
                    }

                    try
                    {
                        comp.__Prototype().__ReferenceSets("BODY").RemoveObjectsFromReferenceSet(bodies_on_layer_50);
                    }
                    catch (Exception ex)
                    {
                        ex.__PrintException();
                    }
                }
            }
        }
    }
}