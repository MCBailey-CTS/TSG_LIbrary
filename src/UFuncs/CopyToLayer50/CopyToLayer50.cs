using System;
using System.Linq;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_copy_to_layer_50)]
    [RevisionLog("Copy to Layer 50")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public class CopyToLayer50 : _UFunc
    {
        private const int LAYER = 50;

        public override void execute()
        {
            var compList = Selection.SelectManyComponents();

            using (session_.using_display_part_reset())
            {
                foreach (var comp in compList)
                {
                    print_("/////////////////");
                    var compPart = (Part)comp.Prototype;

                    var units = compPart.PartUnits;

                    __work_component_ = comp;

                    // if body exists on layer 50, delete body

                    var bodies_on_layer_50 = comp._Prototype().Bodies
                        .ToArray()
                        .Where(__b => __b.IsSolidBody)
                        .Where(__b => __b.Layer == LAYER)
                        .ToArray();

                    var solid_bodies_layer_1 = comp._Prototype().Bodies
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

                    var date = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

                    var layer_50_bodies = comp._Prototype().Layers.GetAllObjectsOnLayer(LAYER).OfType<Body>().ToArray();
                    comp._Prototype().Layers.CopyObjects(LAYER, solid_bodies_layer_1);
                    var new_layer_50_bodies = comp._Prototype().Layers.GetAllObjectsOnLayer(LAYER)
                        .Except(layer_50_bodies).OfType<Body>().ToList();

                    foreach (var body in new_layer_50_bodies)
                    {
                        body.OwningPart.Features.GetParentFeatureOfBody(body).SetName(date);
                        body.Color = 7;
                        body.LineFont = DisplayableObject.ObjectFont.Phantom;
                        body.RedisplayObject();
                    }

                    try
                    {
                        comp._Prototype().reference_sets("BODY")
                            .RemoveObjectsFromReferenceSet(new_layer_50_bodies.ToArray());
                    }
                    catch (Exception ex)
                    {
                        ex._PrintException();
                    }

                    comp._Prototype().Layers.MoveDisplayableObjects(LAYER + 1, layer_50_bodies);

                    foreach (var body in bodies_on_layer_50)
                    {
                        body.Color = 7;
                        body.LineFont = DisplayableObject.ObjectFont.Phantom;
                        body.RedisplayObject();
                        print_(
                            $"Copied solid body to {comp._Prototype().Features.GetAssociatedFeature(body).GetFeatureName()} in part {comp.DisplayName}");
                    }

                    try
                    {
                        comp._Prototype().reference_sets("BODY").RemoveObjectsFromReferenceSet(bodies_on_layer_50);
                    }
                    catch (Exception ex)
                    {
                        ex._PrintException();
                    }
                }
            }
        }
    }
}