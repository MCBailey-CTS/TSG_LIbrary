using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_layout_ref_sets)]
    internal class LayoutRefSets : _UFunc
    {
        private static void Prompt(string message)
        {
            UFSession.GetUFSession().Ui.SetPrompt(message);
        }

        public override void execute()
        {
            if(Session.GetSession().Parts.Display is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            session_.SetUndoMark(Session.MarkVisibility.Visible, "LayoutRefsets");

            var display = __display_part_;

            var leaf = display.Leaf.ToLower();

            if(!leaf.EndsWith("-layout"))
            {
                print_("Layout Refset can only be used on layouts.");
                return;
            }

            var errors = 0;

            try
            {
                // BODY
                Prompt("Loading refset BODY");

                var objects = new List<DisplayableObject>();

                objects.AddRange(__display_part_.Curves.ToArray());
                objects.AddRange(__display_part_.Points.ToArray());
                objects.AddRange(__display_part_.Bodies.ToArray().Where(body => body.IsSolidBody));

                var objects_to_add = objects.Where(obj => obj.Layer == 10)
                    .Where(obj => !obj.IsOccurrence)
                    .Cast<NXObject>()
                    .ToArray();

                if(objects_to_add.Length > 0)
                {
                    var body_refset = display.GetAllReferenceSets().SingleOrDefault(set => set.Name == "BODY");

                    if(body_refset is null)
                    {
                        body_refset = display.CreateReferenceSet();
                        body_refset.SetName("BODY");
                    }

                    Prompt("Adding layer 10 bodies with slugs to BODY");

                    body_refset.AddObjectsToReferenceSet(objects_to_add);
                }
            }
            catch (Exception ex)
            {
                errors++;
                ex.__PrintException();
            }

            try
            {
                // BODY_NO_SLUG
                Prompt("Loading refset BODY_NO_SLUG");

                var objects = new List<DisplayableObject>();

                objects.AddRange(__display_part_.Points.ToArray());
                objects.AddRange(__display_part_.Bodies.ToArray().Where(body => body.IsSolidBody)
                    .Where(body => body.Color != 75));

                var objects_to_add = objects.Where(obj => obj.Layer == 10)
                    .Where(obj => !obj.IsOccurrence)
                    .Cast<NXObject>()
                    .ToArray();

                if(objects_to_add.Length > 0)
                {
                    var body_no_slug_refset =
                        display.GetAllReferenceSets().SingleOrDefault(set => set.Name == "BODY_NO_SLUG");

                    if(body_no_slug_refset is null)
                    {
                        body_no_slug_refset = display.CreateReferenceSet();
                        body_no_slug_refset.SetName("BODY_NO_SLUG");
                    }

                    body_no_slug_refset.AddObjectsToReferenceSet(objects_to_add);
                }
            }
            catch (Exception ex)
            {
                errors++;
                ex.__PrintException();
            }

            try
            {
                const string lwr_3d_name = "LWR-3D";

                // offset face pad body
                var lwr3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 12)
                    .Cast<NXObject>()
                    .ToArray();

                if(lwr3dObjects.Length > 0)
                {
                    var lwr3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == lwr_3d_name);

                    if(lwr3dRefset is null)
                    {
                        lwr3dRefset = display.CreateReferenceSet();
                        lwr3dRefset.SetName(lwr_3d_name);
                    }

                    lwr3dRefset.AddObjectsToReferenceSet(lwr3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string upr_3d_name = "UPR-3D";

                // offset face pad body
                var upr3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 13)
                    .Cast<NXObject>()
                    .ToArray();

                if(upr3dObjects.Length > 0)
                {
                    var upr3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == upr_3d_name);

                    if(upr3dRefset is null)
                    {
                        upr3dRefset = display.CreateReferenceSet();
                        upr3dRefset.SetName(upr_3d_name);
                    }

                    upr3dRefset.AddObjectsToReferenceSet(upr3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string pad_3d_name = "PAD-3D";

                // offset face pad body
                var pad3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 14)
                    .Cast<NXObject>()
                    .ToArray();

                if(pad3dObjects.Length > 0)
                {
                    var pad3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_3d_name);

                    if(pad3dRefset is null)
                    {
                        pad3dRefset = display.CreateReferenceSet();
                        pad3dRefset.SetName(pad_3d_name);
                    }

                    pad3dRefset.AddObjectsToReferenceSet(pad3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string pad_3d_name = "LWR-PROFILE";

                // offset face pad body
                var pad3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 15)
                    .Cast<NXObject>()
                    .ToArray();

                if(pad3dObjects.Length > 0)
                {
                    var pad3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_3d_name);

                    if(pad3dRefset is null)
                    {
                        pad3dRefset = display.CreateReferenceSet();
                        pad3dRefset.SetName(pad_3d_name);
                    }

                    pad3dRefset.AddObjectsToReferenceSet(pad3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string pad_3d_name = "UPR-PROFILE";

                // offset face pad body
                var pad3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 16)
                    .Cast<NXObject>()
                    .ToArray();

                if(pad3dObjects.Length > 0)
                {
                    var pad3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_3d_name);

                    if(pad3dRefset is null)
                    {
                        pad3dRefset = display.CreateReferenceSet();
                        pad3dRefset.SetName(pad_3d_name);
                    }

                    pad3dRefset.AddObjectsToReferenceSet(pad3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string pad_3d_name = "PAD-PROFILE";

                // offset face pad body
                var pad3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 17)
                    .Cast<NXObject>()
                    .ToArray();

                if(pad3dObjects.Length > 0)
                {
                    var pad3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_3d_name);

                    if(pad3dRefset is null)
                    {
                        pad3dRefset = display.CreateReferenceSet();
                        pad3dRefset.SetName(pad_3d_name);
                    }

                    pad3dRefset.AddObjectsToReferenceSet(pad3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }

            try
            {
                const string pad_3d_name = "INCOMING_BODY";

                // offset face pad body
                var pad3dObjects = display.Bodies
                    .ToArray()
                    .Where(body => body.IsSolidBody)
                    .Where(body => !body.IsOccurrence)
                    .Where(body => body.Layer == 9)
                    .Cast<NXObject>()
                    .ToArray();

                if(pad3dObjects.Length > 0)
                {
                    var pad3dRefset = display.GetAllReferenceSets()
                        .SingleOrDefault(refset => refset.Name == pad_3d_name);

                    if(pad3dRefset is null)
                    {
                        pad3dRefset = display.CreateReferenceSet();
                        pad3dRefset.SetName(pad_3d_name);
                    }

                    pad3dRefset.AddObjectsToReferenceSet(pad3dObjects);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                errors++;
            }


            string[] temp =
            {
                "BODY",
                "BODY_NO_SLUG",
                "INCOMING_BODY",
                "PAD-PROFILE",
                "UPR-PROFILE",
                "LWR-PROFILE",
                "PAD-3D",
                "UPR-3D",
                "LWR-3D"
            };

            foreach (var refset_name in temp)
            {
                var refset = session_.Parts.Display.GetAllReferenceSets()
                    .SingleOrDefault(__r => __r.Name == refset_name);

                if(refset is null)
                    continue;

                if(refset.AskAllDirectMembers().Length == 0 && refset.AskMembersInReferenceSet().Length == 0)
                    refset.OwningPart.DeleteReferenceSet(refset);
            }

            UFSession.GetUFSession().Ui.SetStatus($"Layout Ref Sets completed with {errors} error(s)");
        }

        //public override void execute()
        //{
        //    try
        //    {
        //        CreateOpReferenceSet(TSG_Library.Extensions.__work_part_);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex._PrintException();
        //    }
        //}


        //private const string Op = "OP-";

        ////  private readonly CTS_Library.Op _op;      
        ////private readonly NXOpen.Part _part;

        ///// <summary>The start of op layers = 10.</summary>       
        //protected const int RefSetLayerStart = 10;

        ///// <summary>The end of op layers = 249.</summary>       
        //protected const int RefSetLayerEnd = 249;

        ///// <summary>The color of the slugs that we want to place in the OP-Body-Reference set  = 75.</summary>       
        //protected const int Color = 75;

        //public static void CreateOpReferenceSet(Part part)
        //{
        //    //string op = part._AskOpOrNull();

        //    var match = Regex.Match(part.Leaf, "-[PTpt](?<op>\\d+)-");


        //    if (!match.Success)
        //        throw new InvalidOperationException($"The resulting op was null from {_WorkPart.Leaf}.");

        //    string op = match.Groups["op"].Value;

        //    if (!int.TryParse(op, out _))
        //        throw new ArgumentException();

        //    DeleteReferenceSets(part);

        //    CreateAllReferenceSets(part);

        //    AddSlugs(part);

        //}

        //public static void DeleteReferenceSets(Part part)
        //{
        //    foreach (ReferenceSet set in part.GetAllReferenceSets())
        //    {
        //        if (!set.Name.Contains(Op)) continue;
        //        print_("Deleted Reference Set: " + set.Name);
        //        TheUFSession.Obj.DeleteObject(set.Tag);
        //    }
        //}

        //public static void CreateAllReferenceSets(Part part)
        //{
        //    //bool foundNullCategory = false;

        //    for (int layer = RefSetLayerStart; layer <= RefSetLayerEnd; layer++)
        //        try
        //        {
        //            string layerCat = GetLayerCategory(part, layer);

        //            if (layerCat is null)
        //            {
        //                //foundNullCategory = true;
        //                //print_("");
        //                continue;
        //            }

        //            if (layer % 10 == 0)
        //            {
        //                CreateReferenceSet(part, layer);
        //            }
        //            else if ((layer - 1) % 10 == 0 && layer != 11)
        //            {
        //                bool flag1 = GetBodies(part, layer + 4).Length > 0;
        //                bool flag2 = GetBodies(part, layer + 5).Length > 0;
        //                bool flag3 = GetBodies(part, layer + 6).Length > 0;
        //                if (!flag1 || !flag2 || !flag3) continue;
        //                ReferenceSet noSlugRefset = part.CreateReferenceSet();
        //                string noSlugName = layerCat.Replace("SLUGS", "NO-SLUGS");
        //                noSlugRefset.SetName(noSlugName);
        //                print_("Created Reference Set: " + noSlugName);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex._PrintException($"Layer: {layer}");
        //        }

        //    //if (foundNullCategory)
        //    //    print_("\nAt least one layer is missing a layer category.\n");

        //    for (int layer = RefSetLayerStart; layer <= RefSetLayerEnd; layer++)
        //        if (layer % 10 != 0)
        //            CreateReferenceSet(part, layer);
        //}

        //public static void CreateReferenceSet(Part part, int layer)
        //{
        //    string layerCat = GetLayerCategory(part, layer);
        //    try
        //    {
        //        if ((layer - 1) % 10 == 0 && layer != 11) return;
        //        Body[] bodies = GetBodies(part, layer);
        //        if (bodies.Length <= 0) return;
        //        if (string.IsNullOrEmpty(layerCat)) return;
        //        ReferenceSet refset = part.CreateReferenceSet();
        //        refset.SetName(layerCat);
        //        refset.AddObjectsToReferenceSet(bodies.Select(x => x).ToArray<NXObject>());
        //        print_("Created Reference Set: " + layerCat + " with " + bodies.Length + " bodies.");
        //    }
        //    catch (Exception ex)
        //    {
        //        ex._PrintException();
        //    }
        //}

        //public static void AddSlugs(Part part)
        //{
        //    for (int layer = RefSetLayerStart + 1; layer < RefSetLayerEnd; layer += 10)
        //    {
        //        string layerCatName = GetLayerCategory(part, layer);
        //        if (string.IsNullOrEmpty(layerCatName)) continue;
        //        if (!_WorkPart._HasReferenceSet(layerCatName)) continue;
        //        ReferenceSet bodyReferenceSet = _WorkPart._FindReferenceSet(layerCatName);
        //        Body[] bodies = GetBodies(part, layer + 1);
        //        if (bodies.Length <= 0) return;
        //        Body[] validBodies = (from body in bodies where body.Color == Color select body).ToArray();
        //        if (validBodies.Length <= 0) continue;
        //        bodyReferenceSet.AddObjectsToReferenceSet(validBodies.Select(x => x).ToArray<NXObject>());
        //        print_(validBodies.Length + " were added to " + layerCatName);
        //    }
        //}

        //public static Body[] GetBodies(Part part, int layer)
        //{
        //    return (from body in new Part_(part.Tag).Bodies
        //            where body.ObjectSubType == Snap.NX.ObjectTypes.SubType.BodySolid
        //            where body.Layer == layer
        //            select body.NXOpenBody).ToArray();
        //}

        //public static string GetLayerCategory(Part part, int layer)
        //{
        //    IEnumerable<string> list = from category in new Part_(part.Tag).Categories
        //                               where category.Name.Contains(Op)
        //                               where category.Layers.Contains(layer)
        //                               select category.Name;
        //    return list.FirstOrDefault();
        //}
    }
}