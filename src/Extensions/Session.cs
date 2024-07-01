using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Preferences;
using NXOpen.UF;
using TSG_Library.Disposable;
using TSG_Library.Enum;
using static NXOpen.Session;
using static NXOpen.UF.UFConstants;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Session

        //
        // Summary:
        //     Creates a new part
        //
        // Parameters:
        //   pathName:
        //     The full pathname of the part
        //
        //   templateType:
        //     The type of the template to be used to create the part
        //
        //   unitType:
        //     The type of the unit to be used
        //
        // Returns:
        //     An NX.Part object
        /// <summary>
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="templateType"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        internal static Part __CreatePart(string pathName, Templates templateType, Units unitType)
        {
            Part nXOpenPart = __work_part_;
            Part nXOpenPart2 = __display_part_;
            FileNew fileNew = session_.Parts.FileNew();
            fileNew.ApplicationName = __GetAppName(fileNew, templateType);
            fileNew.TemplateFileName = __GetTemplateFileName(fileNew, templateType, unitType);
            fileNew.UseBlankTemplate = fileNew.TemplateFileName == "Blank";

            fileNew.Units =
                unitType == Units.MilliMeters ? Part.Units.Millimeters : Part.Units.Inches;

            fileNew.NewFileName = pathName;
            fileNew.MasterFileName = "";
            fileNew.MakeDisplayedPart = true;
            NXObject nXObject = fileNew.Commit();
            fileNew.Destroy();

            if (nXOpenPart != null)
            {
                __work_part_ = nXOpenPart;
                __display_part_ = nXOpenPart2;
            }

            return (Part)nXObject;
        }

        public static TSG_Library.Disposable.Rollback __UsingRollback(
            this Session session,
            NXOpen.Features.Feature feat
        )
        { return new Rollback(feat, "Rollback"); }

        public static void __SetWorkPlane(
            this Session _,
            double gridSpace,
            bool snapToGrid,
            bool showGrid
        )
        {
            WorkPlane workPlane1 = __display_part_.Preferences.Workplane;

            if (workPlane1 is null)
                return;

            workPlane1.GridType = WorkPlane.Grid.Rectangular;
            workPlane1.GridIsNonUniform = false;
            WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(gridSpace, 1, 1);
            workPlane1.SetRectangularUniformGridSize(gridSize1);
            workPlane1.ShowGrid = showGrid;
            workPlane1.ShowLabels = false;
            workPlane1.SnapToGrid = snapToGrid;
            workPlane1.GridOnTop = false;
            workPlane1.RectangularShowMajorLines = false;
            workPlane1.PolarShowMajorLines = false;
            workPlane1.GridColor = 7;
        }

        public static LockUiFromCustom __UsingLockUiFromCustom(this Session _)
        {
            return new LockUiFromCustom();
        }

        // ReSharper disable once ParameterHidesMember
        public static string __SelectMenuItem14gt(
            this Session session_,
            string title,
            string[] items
        )
        {
            IList<string[]> separated = new List<string[]>();

            List<string> list_items = items.ToList();

            const string __next__ = "...NEXT...";
            const int max = 14;

            if (items.Length == max)
                using (session_.__UsingLockUiFromCustom())
                {
                    int picked_item = ufsession_.Ui.DisplayMenu(title, 0, items, items.Length);

                    switch (picked_item)
                    {
                        case 0:
                            throw new Exception("Picked item was 0");
                        case 1:
                            throw new InvalidOperationException("Back was selected");
                        case 2:
                            throw new InvalidOperationException("Cancel was selected");
                        case 5:
                            return items[0];
                        case 6:
                            return items[1];
                        case 7:
                            return items[2];
                        case 8:
                            return items[3];
                        case 9:
                            return items[4];
                        case 10:
                            return items[5];
                        case 11:
                            return items[6];
                        case 12:
                            return items[7];
                        case 13:
                            return items[8];
                        case 14:
                            return items[9];
                        case 15:
                            return items[10];
                        case 16:
                            return items[11];
                        case 17:
                            return items[12];
                        case 18:
                            return items[13];
                        case 19:
                            throw new InvalidOperationException("Unable to display menu");
                        default:
                            throw new InvalidOperationException(
                                $"Unknown picked item: {picked_item}"
                            );
                    }
                }

            while (list_items.Count > 0)
            {
                string[] set_of_items = new string[max];

                set_of_items[set_of_items.Length - 1] = __next__;

                int end_index = set_of_items.Length - 1;

                if (list_items.Count < max)
                {
                    set_of_items = new string[list_items.Count];
                    end_index = list_items.Count;
                }

                for (int i = 0; i < end_index; i++)
                {
                    set_of_items[i] = list_items[0];
                    list_items.RemoveAt(0);
                }

                separated.Add(set_of_items);
            }

            int current_set_index = 0;

            while (true)
                using (session_.__UsingLockUiFromCustom())
                {
                    int picked_item = ufsession_.Ui.DisplayMenu(
                        title,
                        0,
                        separated[current_set_index],
                        separated[current_set_index].Length
                    );

                    switch (picked_item)
                    {
                        case 0:
                            throw new Exception("Picked item was 0");
                        case 1:
                            if (current_set_index > 0)
                                current_set_index--;
                            continue;
                        case 2:
                            throw new InvalidOperationException("Cancel was selected");
                        case 5:
                            return separated[current_set_index][0];
                        case 6:
                            return separated[current_set_index][1];
                        case 7:
                            return separated[current_set_index][2];
                        case 8:
                            return separated[current_set_index][3];
                        case 9:
                            return separated[current_set_index][4];
                        case 10:
                            return separated[current_set_index][5];
                        case 11:
                            return separated[current_set_index][6];
                        case 12:
                            return separated[current_set_index][7];
                        case 13:
                            return separated[current_set_index][8];
                        case 14:
                            return separated[current_set_index][9];
                        case 15:
                            return separated[current_set_index][10];
                        case 16:
                            return separated[current_set_index][11];
                        case 17:
                            return separated[current_set_index][12];
                        case 18:
                            if (
                                separated[current_set_index][13] == __next__
                                && current_set_index + 1 < separated.Count
                            )
                            {
                                current_set_index++;
                                continue;
                            }

                            if (current_set_index + 1 == separated.Count)
                                continue;

                            return separated[current_set_index][13];
                        case 19:
                            throw new InvalidOperationException("Unable to display menu");
                        default:
                            throw new InvalidOperationException(
                                $"Unknown picked item: {picked_item}"
                            );
                    }
                }
        }

        public static DoUpdate __UsingDoUpdate(this Session _)
        {
            return new DoUpdate();
        }

        public static DoUpdate __UsingDoUpdate(this Session _, string undo_text)
        {
            return new DoUpdate(undo_text);
        }

        public static IDisposable __UsingFormShowHide(
            this Session _,
            Form __form,
            bool hide_form = true
        )
        {
            if (hide_form)
                __form.Hide();

            return new FormHideShow(__form);
        }

        public static Part __FindOrOpen(this Session session, string __path_or_leaf)
        {
            try
            {
                if (ufsession_.Part.IsLoaded(__path_or_leaf) == 0)
                    return session.Parts.Open(__path_or_leaf, out _);

                return (Part)session.Parts.FindObject(__path_or_leaf);
            }
            catch (NXException ex) when (ex.ErrorCode == 1020038)
            {
                throw NXException.Create(ex.ErrorCode, $"Invalid file format: '{__path_or_leaf}'");
            }
            catch (NXException ex) when (ex.ErrorCode == 1020001)
            {
                throw NXException.Create(ex.ErrorCode, $"File not found: '{__path_or_leaf}'");
            }
        }

        public static void __SaveAll(this Session session)
        {
            session.Parts.SaveAll(out _, out _);
            ;
        }

        public static void __CloseAll(this Session _)
        {
            ufsession_.Part.CloseAll();
        }

        public static void __SetDisplayToWork(this Session _)
        {
            __work_part_ = __display_part_;
        }

        public static bool __WorkIsDisplay(this Session _)
        {
            return __display_part_.Tag == __work_part_.Tag;
        }

        public static bool __IsAssemblyHolder(this string str)
        {
            return str.__IsLsh()
                || str.__IsUsh()
                || str.__IsLwr()
                || str.__IsUpr()
                || str.__IsLsp()
                || str.__IsUsp()
                || str.__Is000();
        }

        public static bool __WorkIsNotDisplay(this Session session)
        {
            return !session.__WorkIsDisplay();
        }

        public static Destroyer __UsingBuilderDestroyer(this Session _, Builder __builder)
        {
            return new Destroyer(__builder);
        }

        public static DisplayPartReset __UsingDisplayPartReset(this Session _)
        {
            return new DisplayPartReset();
        }

        public static SuppressDisplayReset __UsingSuppressDisplay(this Session _)
        {
            return new SuppressDisplayReset();
        }

        public static IDisposable __UsingLockUgUpdates(this Session _)
        {
            return new LockUpdates();
        }

        public static IDisposable __UsingRegenerateDisplay(this Session _)
        {
            return new RegenerateDisplay();
        }

        public static TaggedObject __GetTaggedObject(this Session session, Tag tag)
        {
            return session.GetObjectManager().GetTaggedObject(tag);
        }

        public static PartCollection.SdpsStatus __SetActiveDisplay(
            this Session session_1,
            Part __part
        )
        {
            if (
                session_1.Parts.AllowMultipleDisplayedParts
                != PartCollection.MultipleDisplayedPartStatus.Enabled
            )
                throw new Exception("Session does not allow multiple displayed parts");

            return session_1.Parts.SetActiveDisplay(
                __part,
                DisplayPartOption.AllowAdditional,
                PartDisplayPartWorkPartOption.UseLast,
                out _
            );
        }

        public static void __Delete(this ReferenceSet refst) =>
            refst.OwningPart.DeleteReferenceSet(refst);

        public static void __SelectSingleObject(
            this Session _,
            string message,
            string title,
            int scope,
            UFUi.SelInitFnT init_proc,
            IntPtr user_data,
            out int response,
            out Tag _object,
            double[] cursor,
            out Tag view
        )
        {
            ufsession_.Ui.SelectWithSingleDialog(
                message,
                title,
                scope,
                init_proc,
                user_data,
                out response,
                out _object,
                cursor,
                out view
            );
        }

        public static void __SelectSingleObject(
            this Session session,
            UFUi.SelInitFnT init_proc,
            IntPtr user_data,
            out int response,
            out Tag _object
        )
        {
            double[] cursor = new double[3];

            session.__SelectSingleObject(
                "Select Component Message",
                "Select Component Title",
                UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                init_proc,
                user_data,
                out response,
                out _object,
                cursor,
                out _
            );
        }

        public static Initialize2EvaluatorFree __UsingEvaluator(this Session _, Tag tag) =>
            new Initialize2EvaluatorFree(tag);

        public static TaggedObject __FindByName(this Session session, string __name) =>
            session.__FindAllByName(__name).First();

        public static IEnumerable<TaggedObject> __FindAllByName(this Session _, string __name)
        {
            Tag __tag = Tag.Null;

            do
            {
                ufsession_.Obj.CycleByName(__name, ref __tag);

                if (__tag != Tag.Null)
                    yield return session_.__GetTaggedObject(__tag);
            } while (__tag != Tag.Null);
        }

        public static void __DeleteObjects(
            this Session session_,
            params TaggedObject[] __objects_to_delete
        )
        {
            using (GetSession().__UsingDoUpdate())
                session_.UpdateManager.AddObjectsToDeleteList(__objects_to_delete);
        }

        public static void __Delete(this Tag tag) =>
            session_.__DeleteObjects(tag.__ToTaggedObject());

        public static void __Delete(this TaggedObject taggedObject) =>
            session_.__DeleteObjects(taggedObject);

        public static IDisposable __UsingGCHandle(this Session _, GCHandle __handle) =>
            new GCHandleFree(__handle);

        public static BasePart __New(this Session session, string file_path, BasePart.Units units)
        {
            using (session.__UsingDisplayPartReset())
            {
                int units_;

                switch (units)
                {
                    case BasePart.Units.Inches:
                        units_ = 2;
                        break;
                    case BasePart.Units.Millimeters:
                        units_ = 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ufsession_.Part.New(file_path, units_, out Tag part);
                return part.__To<BasePart>();
            }
        }

        #endregion


        public static NXOpen.ICurve SelectCurveOrEdge(Predicate<NXOpen.ICurve> pred = null)
        {
            throw new NotImplementedException();
        }

        // public static NXOpen.Curve SelectCurve(
        //     string message = "Select Curve",
        //     string title = "Select Curve")
        // {
        //     NXOpen.Edge e = null;
        //     e.SolidEdgeType == 

        //     throw new NotImplementedException();
        // }


        public static NXOpen.Edge SelectCurveOrEdge() => null;
        public static NXOpen.Edge SelectEdgeLinear() => null;
        public static NXOpen.Edge SelectEdge() => null;
        public static NXOpen.Edge SelectEdgeCircular() => null;
        public static NXOpen.Edge SelectECurve() => null;
        public static NXOpen.Edge SelectLine() => null;
        public static NXOpen.Edge SelectArc() => null;


    }
}
