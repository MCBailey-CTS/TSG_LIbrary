using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_layout_ref_sets)]
    internal class LayoutRefSets : _UFunc
    {
        public override void execute()
        {
            if (__display_part_ is null)
            {
                print_("There is no displayed part loaded");
                return;
            }

            session_.SetUndoMark(Session.MarkVisibility.Visible, "LayoutRefsets");

            string text = __display_part_.Leaf.ToLower();

            if (!text.EndsWith("-layout"))
            {
                print_("Layout Refset can only be used on layouts.");
                return;
            }

            int num = 0;

            try
            {
                List<DisplayableObject> list = new List<DisplayableObject>();
                list.AddRange(__display_part_.Curves.ToArray());
                list.AddRange(__display_part_.Points.ToArray());

                list.AddRange(from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    select body);

                NXObject[] array = NewMethod1(list, 10);

                if (array.Length != 0)
                {
                    ReferenceSet referenceSet = __display_part_.GetAllReferenceSets()
                        .SingleOrDefault(set => set.Name == "BODY");
                    if (referenceSet == null)
                    {
                        referenceSet = __display_part_.CreateReferenceSet();
                        referenceSet.SetName("BODY");
                    }

                    referenceSet.AddObjectsToReferenceSet(array);
                }
            }
            catch (Exception ex)
            {
                num++;
                ex.__PrintException();
            }

            try
            {
                List<DisplayableObject> list2 = new List<DisplayableObject>();
                list2.AddRange(__display_part_.Points.ToArray());
                list2.AddRange(from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where body.Color != 75
                    select body);

                NXObject[] array2 = NewMethod1(list2, 10);
            }
            catch (Exception ex2)
            {
                num++;
                ex2.__PrintException();
            }

            try
            {
                NXObject[] array3 = FindBodiesOnLayerInDisplayPart(12);
                MakeRefSet(array3, "LWR-3D");
            }
            catch (Exception ex3)
            {
                ex3.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array4 = FindBodiesOnLayerInDisplayPart(13);
                MakeRefSet(array4, "UPR-3D");
            }
            catch (Exception ex4)
            {
                ex4.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array5 = FindBodiesOnLayerInDisplayPart(14);
                MakeRefSet(array5, "PAD-3D");
            }
            catch (Exception ex5)
            {
                ex5.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array6 = FindBodiesOnLayerInDisplayPart(15);
                MakeRefSet(array6, "LWR-PROFILE");
            }
            catch (Exception ex6)
            {
                ex6.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array7 = FindBodiesOnLayerInDisplayPart(16);
                MakeRefSet(array7, "UPR-PROFILE");
            }
            catch (Exception ex7)
            {
                ex7.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array8 = FindBodiesOnLayerInDisplayPart(17);
                MakeRefSet(array8, "PAD-PROFILE");
            }
            catch (Exception ex8)
            {
                ex8.__PrintException();
                num++;
            }

            try
            {
                NXObject[] array9 = FindBodiesOnLayerInDisplayPart(9);
                MakeRefSet(array9, "INCOMING_BODY");
            }
            catch (Exception ex9)
            {
                ex9.__PrintException();
                num++;
            }

            string[] array10 = new string[9]
            {
                "BODY", "BODY_NO_SLUG", "INCOMING_BODY", "PAD-PROFILE", "UPR-PROFILE", "LWR-PROFILE", "PAD-3D",
                "UPR-3D", "LWR-3D"
            };
            string[] array11 = array10;

            foreach (string refset_name in array11)
            {
                ReferenceSet referenceSet10 = __display_part_.__FindReferenceSetOrNull(refset_name);

                if (referenceSet10 != null && referenceSet10.AskAllDirectMembers().Length == 0 &&
                    referenceSet10.AskMembersInReferenceSet().Length == 0)
                    referenceSet10.OwningPart.DeleteReferenceSet(referenceSet10);
            }

            print_($"Layout Ref Sets completed with {num} error(s)");
        }

        private static void MakeRefSet(NXObject[] array9, string ref_set)
        {
            if (array9.Length != 0)
            {
                ReferenceSet referenceSet9 = __display_part_.GetAllReferenceSets()
                    .SingleOrDefault(refset => refset.Name == ref_set);
                if (referenceSet9 == null)
                {
                    referenceSet9 = __display_part_.CreateReferenceSet();
                    referenceSet9.SetName(ref_set);
                }

                referenceSet9.AddObjectsToReferenceSet(array9);
            }
        }

        private static NXObject[] FindBodiesOnLayerInDisplayPart(int layer)
        {
            return (from body in __display_part_.Bodies.ToArray()
                where body.IsSolidBody
                where !body.IsOccurrence
                where body.Layer == layer
                select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod1(List<DisplayableObject> list2, int layer)
        {
            return (from obj in list2
                where obj.Layer == layer
                where !obj.IsOccurrence
                select obj).Cast<NXObject>().ToArray();
        }
    }
}
// 340