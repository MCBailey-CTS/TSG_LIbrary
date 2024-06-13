using System.Collections.Generic;
using System.Linq;
using NXOpen;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_category_ref_sets)]
    public class CategoryRefSets : _UFunc
    {
        public override void execute()
        {
            Part workPart = session_.Parts.Work;
            Tag bodyRefSet = Tag.Null;
            Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Category Reference Sets");
            Tag cycleRefSet = Tag.Null;
            List<Tag> refSetTag = new List<Tag>();
            var isBodyRefSet = false;

            do
            {
                ufsession_.Obj.CycleObjsInPart(workPart.Tag, UF_reference_set_type, ref cycleRefSet);

                if(cycleRefSet == Tag.Null)
                    break;

                refSetTag.Add(cycleRefSet);
            } while (cycleRefSet != Tag.Null);

            foreach (Tag refSet in refSetTag)
            {
                ufsession_.Obj.AskName(refSet, out var refSetName);

                if(refSetName != "BODY")
                    continue;

                isBodyRefSet = true;
                bodyRefSet = refSet;
                break;
            }

            if(isBodyRefSet)
            {
                //----------------------------------------------
                // remove all but body and fasteners from body refset
                //----------------------------------------------

                // get the body ref set tag
                // get a list of all members of the body ref set and remove all
                // get the body.tag from the body on layer 1
                // add body
                // add fasteners


                ufsession_.Assem.AskRefSetMembers(bodyRefSet, out var memberCount, out Tag[] members);
                ufsession_.Assem.RemoveRefSetMembers(bodyRefSet, memberCount, members);

                List<Tag> wpBody = (from Body body in workPart.Bodies
                        where body.Layer == 1 && body.Tag != Tag.Null
                        select body.Tag)
                    .ToList();

                if(wpBody.Count == 1)
                {
                    ufsession_.Assem.AddRefSetMembers(bodyRefSet, wpBody.Count, wpBody.ToArray());

                    if(workPart.ComponentAssembly.RootComponent is null)
                        return;

                    List<Tag> compList = (from component in workPart.ComponentAssembly.RootComponent.GetChildren()
                        where component.Layer == 99 && component.Tag != Tag.Null
                        select component.Tag).ToList();

                    ufsession_.Assem.AddRefSetMembers(bodyRefSet, compList.Count, compList.ToArray());
                }
                else
                {
                    print_("You have more than one solid body on layer one");
                    session_.SetUndoMarkVisibility(markId1, "", Session.MarkVisibility.Invisible);
                    session_.UndoToMark(markId1, "Category Reference Sets");
                }

                return;
            }

            if(workPart.Bodies.ToArray().Length == 0)
            {
                if(workPart.LayerCategories.ToArray().Length != 1)
                    return;

                CreateCategoryNames(workPart);
                const string bodyRefSetName = "BODY";
                const string subRefSetName = "SUB_TOOL";
                const int numOfBodyMembers = 0;
                const int numOfSubMembers = 0;
                var origin = new double[3];
                ufsession_.Csys.AskWcs(out Tag wcs);
                ufsession_.Csys.AskCsysInfo(wcs, out Tag wcsMatrix,
                    origin); // get origin of current work coordinate system
                var matrixValue = new double[9];
                ufsession_.Csys.AskMatrixValues(wcsMatrix, matrixValue); // gets the matrix values
                ufsession_.Assem.CreateRefSet(bodyRefSetName, origin, matrixValue, new Tag[] { }, numOfBodyMembers,
                    out bodyRefSet);
                ufsession_.Assem.CreateRefSet(subRefSetName, origin, matrixValue, new Tag[] { }, numOfSubMembers,
                    out Tag _);
                return;
            }

            if(workPart.LayerCategories.ToArray().Length == 1)
            {
                CreateCategoryNames(workPart);
                const string bodyRefSetName = "BODY";
                const string subRefSetName = "SUB_TOOL";
                const int numOfBodyMembers = 1;
                const int numOfSubMembers = 0;
                var origin = new double[3];
                ufsession_.Csys.AskWcs(out Tag wcs);
                ufsession_.Csys.AskCsysInfo(wcs, out Tag wcsMatrix,
                    origin); // get origin of current work coordinate system
                var matrixValue = new double[9];
                ufsession_.Csys.AskMatrixValues(wcsMatrix, matrixValue); // gets the matrix values

                List<Tag> bodyArray = (from Body body in workPart.Bodies
                        where body.Layer == 1 && body.Tag != Tag.Null
                        select body.Tag)
                    .ToList();

                if(bodyArray.Count == 1)
                {
                    ufsession_.Assem.CreateRefSet(bodyRefSetName, origin, matrixValue, bodyArray.ToArray(),
                        numOfBodyMembers, out bodyRefSet);
                    ufsession_.Assem.CreateRefSet(subRefSetName, origin, matrixValue, new Tag[] { }, numOfSubMembers,
                        out Tag _);
                }
                else
                {
                    print_("You have more than one solid body on layer one");
                }
            }
            else
            {
                //----------------------------------------------
                //  CreateProgressive Body and Sub Tool reference sets
                //----------------------------------------------
                const string bodyRefSetName = "BODY";
                const string subRefSetName = "SUB_TOOL";
                const int numOfBodyMembers = 1;
                const int numOfSubMembers = 0;
                var origin = new double[3];
                ufsession_.Csys.AskWcs(out Tag wcs);
                ufsession_.Csys.AskCsysInfo(wcs, out Tag wcsMatrix,
                    origin); // get origin of current work coordinate system
                var matrixValue = new double[9];
                ufsession_.Csys.AskMatrixValues(wcsMatrix, matrixValue); // gets the matrix values

                List<Tag> bodyArray = (from Body body in workPart.Bodies
                        where body.Layer == 1 && body.Tag != Tag.Null
                        select body.Tag)
                    .ToList();

                if(bodyArray.Count == 1)
                {
                    ufsession_.Assem.CreateRefSet(bodyRefSetName, origin, matrixValue, bodyArray.ToArray(),
                        numOfBodyMembers, out bodyRefSet);
                    ufsession_.Assem.CreateRefSet(subRefSetName, origin, matrixValue, new Tag[] { }, numOfSubMembers,
                        out Tag _);

                    if(workPart.ComponentAssembly.RootComponent is null)
                        return;

                    List<Tag> compList = (from component in workPart.ComponentAssembly.RootComponent.GetChildren()
                        where component.Layer == 99 && component.Tag != Tag.Null
                        select component.Tag).ToList();

                    ufsession_.Assem.AddRefSetMembers(bodyRefSet, compList.Count, compList.ToArray());
                }
                else
                {
                    print_("You have more than one solid body on layer one");
                }
            }
        }

        public static void CreateCategoryNames(Part wp)
        {
            wp.LayerCategories.CreateCategory("BODY", "", new[] { 1 });
            wp.LayerCategories.CreateCategory("BURNOUT", "", new[] { 100, 101, 102, 103, 104, 105 });
            wp.LayerCategories.CreateCategory("CONSTRUCTION", "", new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            wp.LayerCategories.CreateCategory("DATUM", "", new[] { 255 });
            wp.LayerCategories.CreateCategory("FASTENERS", "", new[] { 99 });
            wp.LayerCategories.CreateCategory("HANDLINGHOLES", "", new[] { 98 });
            wp.LayerCategories.CreateCategory("HOLECHARTTEXT", "", new[] { 230 });
            wp.LayerCategories.CreateCategory("NESTEDBLOCKS", "", new[] { 96 });
            wp.LayerCategories.CreateCategory("ORDERBLOCK", "", new[] { 250 });
            wp.LayerCategories.CreateCategory("SUBTOOL", "", new[] { 15, 16, 17, 18, 19, 20 });
            wp.LayerCategories.CreateCategory("TITLEBLOCK", "", new[] { 200 });
            wp.LayerCategories.CreateCategory("TOOLINGHOLES", "", new[] { 97 });
            wp.LayerCategories.CreateCategory("WIRESTARTHOLE", "", new[] { 94 });
        }
    }
}