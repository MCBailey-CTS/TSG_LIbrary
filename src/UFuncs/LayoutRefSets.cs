using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;
using NXOpen;
using NXOpen.UF;
using NXOpen_;
using TSG_Library.Attributes;
using TSG_Library.Extensions;
using Ufuncs0.Utilities;
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
                Globals.print_("There is no displayed part loaded");
                return;
            }

            session_.SetUndoMark(Session.MarkVisibility.Visible, "LayoutRefsets");
            
            string text = __display_part_.Leaf.ToLower();

            if (!text.EndsWith("-layout"))
            {
                Globals.print_("Layout Refset can only be used on layouts.");
                return;
            }

            int num = 0;

            try
            {
                prompt_("Loading refset BODY");
                List<DisplayableObject> list = new List<DisplayableObject>();
                list.AddRange(__display_part_.Curves.ToArray());
                list.AddRange(__display_part_.Points.ToArray());

                list.AddRange(from body in __display_part_.Bodies.ToArray()
                              where body.IsSolidBody
                              select body);

                NXObject[] array = NewMethod(list);

                if (array.Length != 0)
                {
                    ReferenceSet referenceSet = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet set) => set.Name == "BODY");
                    if (referenceSet == null)
                    {
                        referenceSet = __display_part_.CreateReferenceSet();
                        referenceSet.SetName("BODY");
                    }

                    prompt_("Adding layer 10 bodies with slugs to BODY");
                    referenceSet.AddObjectsToReferenceSet(array);
                }
            }
            catch (Exception ex)
            {
                num++;
                ex._PrintException();
            }

            try
            {
                prompt_("Loading refset BODY_NO_SLUG");
                List<DisplayableObject> list2 = new List<DisplayableObject>();
                list2.AddRange(__display_part_.Points.ToArray());
                list2.AddRange(from body in __display_part_.Bodies.ToArray()
                               where body.IsSolidBody
                               where body.Color != 75
                               select body);
                NXObject[] array2 = NewMethod1(list2);
                if (array2.Length != 0)
                {
                    ReferenceSet referenceSet2 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet set) => set.Name == "BODY_NO_SLUG");
                    if (referenceSet2 == null)
                    {
                        referenceSet2 = __display_part_.CreateReferenceSet();
                        referenceSet2.SetName("BODY_NO_SLUG");
                    }

                    referenceSet2.AddObjectsToReferenceSet(array2);
                }
            }
            catch (Exception ex2)
            {
                num++;
                ex2._PrintException();
            }

            try
            {
                NXObject[] array3 = NewMethod2();
                if (array3.Length != 0)
                {
                    ReferenceSet referenceSet3 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "LWR-3D");
                    if (referenceSet3 == null)
                    {
                        referenceSet3 = __display_part_.CreateReferenceSet();
                        referenceSet3.SetName("LWR-3D");
                    }

                    referenceSet3.AddObjectsToReferenceSet(array3);
                }
            }
            catch (Exception ex3)
            {
                ex3._PrintException();
                num++;
            }

            try
            {
                NXObject[] array4 = NewMethod3();
                if (array4.Length != 0)
                {
                    ReferenceSet referenceSet4 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "UPR-3D");
                    if (referenceSet4 == null)
                    {
                        referenceSet4 = __display_part_.CreateReferenceSet();
                        referenceSet4.SetName("UPR-3D");
                    }

                    referenceSet4.AddObjectsToReferenceSet(array4);
                }
            }
            catch (Exception ex4)
            {
                ex4._PrintException();
                num++;
            }

            try
            {
                NXObject[] array5 = NewMethod4();
                if (array5.Length != 0)
                {
                    ReferenceSet referenceSet5 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "PAD-3D");
                    if (referenceSet5 == null)
                    {
                        referenceSet5 = __display_part_.CreateReferenceSet();
                        referenceSet5.SetName("PAD-3D");
                    }

                    referenceSet5.AddObjectsToReferenceSet(array5);
                }
            }
            catch (Exception ex5)
            {
                ex5._PrintException();
                num++;
            }

            try
            {
                NXObject[] array6 = NewMethod5();
                if (array6.Length != 0)
                {
                    ReferenceSet referenceSet6 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "LWR-PROFILE");
                    if (referenceSet6 == null)
                    {
                        referenceSet6 = __display_part_.CreateReferenceSet();
                        referenceSet6.SetName("LWR-PROFILE");
                    }

                    referenceSet6.AddObjectsToReferenceSet(array6);
                }
            }
            catch (Exception ex6)
            {
                ex6._PrintException();
                num++;
            }

            try
            {
                NXObject[] array7 = NewMethod6();
                if (array7.Length != 0)
                {
                    ReferenceSet referenceSet7 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "UPR-PROFILE");
                    if (referenceSet7 == null)
                    {
                        referenceSet7 = __display_part_.CreateReferenceSet();
                        referenceSet7.SetName("UPR-PROFILE");
                    }

                    referenceSet7.AddObjectsToReferenceSet(array7);
                }
            }
            catch (Exception ex7)
            {
                ex7._PrintException();
                num++;
            }

            try
            {
                NXObject[] array8 = NewMethod7();
                if (array8.Length != 0)
                {
                    ReferenceSet referenceSet8 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "PAD-PROFILE");
                    if (referenceSet8 == null)
                    {
                        referenceSet8 = __display_part_.CreateReferenceSet();
                        referenceSet8.SetName("PAD-PROFILE");
                    }

                    referenceSet8.AddObjectsToReferenceSet(array8);
                }
            }
            catch (Exception ex8)
            {
                ex8._PrintException();
                num++;
            }

            try
            {
                NXObject[] array9 = NewMethod8();
                if (array9.Length != 0)
                {
                    ReferenceSet referenceSet9 = __display_part_.GetAllReferenceSets().SingleOrDefault((ReferenceSet refset) => refset.Name == "INCOMING_BODY");
                    if (referenceSet9 == null)
                    {
                        referenceSet9 = __display_part_.CreateReferenceSet();
                        referenceSet9.SetName("INCOMING_BODY");
                    }

                    referenceSet9.AddObjectsToReferenceSet(array9);
                }
            }
            catch (Exception ex9)
            {
                ex9._PrintException();
                num++;
            }

            string[] array10 = new string[9] { "BODY", "BODY_NO_SLUG", "INCOMING_BODY", "PAD-PROFILE", "UPR-PROFILE", "LWR-PROFILE", "PAD-3D", "UPR-3D", "LWR-3D" };
            string[] array11 = array10;

            foreach (string refset_name in array11)
            {
                ReferenceSet referenceSet10 = __display_part_.__FindReferenceSetOrNull(refset_name);

                if (referenceSet10 != null && referenceSet10.AskAllDirectMembers().Length == 0 && referenceSet10.AskMembersInReferenceSet().Length == 0)
                    referenceSet10.OwningPart.DeleteReferenceSet(referenceSet10);
            }

            print_($"Layout Ref Sets completed with {num} error(s)");
        }

        private static NXObject[] NewMethod8()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 9
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod7()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 17
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod6()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 16
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod5()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 15
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod4()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 14
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod3()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 13
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod2()
        {
            return (from body in __display_part_.Bodies.ToArray()
                    where body.IsSolidBody
                    where !body.IsOccurrence
                    where body.Layer == 12
                    select body).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod1(List<DisplayableObject> list2)
        {
            return (from obj in list2
                    where obj.Layer == 10
                    where !obj.IsOccurrence
                    select obj).Cast<NXObject>().ToArray();
        }

        private static NXObject[] NewMethod(List<DisplayableObject> list)
        {
            return (from obj in list
                    where obj.Layer == 10
                    where !obj.IsOccurrence
                    select obj).Cast<NXObject>().ToArray();
        }
    }
}
// 340