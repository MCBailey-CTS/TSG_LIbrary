using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
// ReSharper disable UnusedType.Global

namespace TSG_Library.Ui
{
    public class SpecialSelection1<T> where T : DisplayableObject
    {
        protected const int Accept = UFConstants.UF_UI_SEL_ACCEPT;

        protected const int Reject = UFConstants.UF_UI_SEL_REJECT;

        protected const int Success = UFConstants.UF_UI_SEL_SUCCESS;

        protected const int Failure = UFConstants.UF_UI_SEL_FAILURE;

        // ReSharper disable once NotAccessedField.Global
        protected readonly bool KeepHighlighted;

        protected readonly UFUi.Mask[] Masks;

        protected readonly string Prompt;

        protected readonly NXOpen.Selection.SelectionScope Scope;

        protected readonly Predicate<T> SelectionPredicate;

        protected internal SpecialSelection1(Predicate<T> selectionPredicate, NXOpen.Selection.SelectionScope scope,
            bool keepHighlighted, string prompt, params UFUi.Mask[] masks)
        {
            //if (!masks.Any()) throw new ArgumentException("Collection doesn't contain any elements.", "masks");
            SelectionPredicate = selectionPredicate;
            Scope = scope;
            Masks = masks;
            KeepHighlighted = keepHighlighted;
            Prompt = prompt;
        }

        public static UFUi Ui => UFSession.GetUFSession().Ui;

        protected internal virtual T SelectSingle()
        {
            return Select(false)[0];
        }

        protected internal virtual List<T> SelectMultiple()
        {
            return Select(true).ToList();
        }

        protected virtual T[] Select(bool multiple)
        {
            try
            {
                UFUi.SelInitFnT initialProcess = null;
                if (SelectionPredicate != null) initialProcess = InitialProcess;
                Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
                T[] result;
                if (multiple)
                {
                    Ui.SelectWithClassDialog(Prompt, Prompt, (int)Scope, initialProcess, IntPtr.Zero, out _, out _,
                        out Tag[] selectedObj);
                    result = selectedObj.Select(x => (T)NXObjectManager.Get(x)).ToArray();
                }
                else
                {
                    double[] cursor = new double[3];
                    Ui.SelectWithSingleDialog(Prompt, Prompt, (int)Scope, initialProcess, IntPtr.Zero, out _,
                        out Tag selectedObj, cursor, out _);
                    result = new[] { (T)NXObjectManager.Get(selectedObj) };
                }

                result.ToList().ForEach(x => x.Unhighlight());
                return result;
            }
            finally
            {
                Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
            }
        }

        protected virtual int FilterProcess(Tag @object, int[] type, IntPtr userData, IntPtr select)
        {
            if (!(NXObjectManager.Get(@object) is T taggedObject)) return Reject;

            //Snap.UI.Selection.SelectObject("").Show().

            try
            {
                //accept = 1
                //reject = 0
                int result = SelectionPredicate(taggedObject) ? Accept : Reject;
                return result;
            }
            catch (Exception ex)
            {
                ListingWindow lw = Session.GetSession().ListingWindow;
                if (!lw.IsOpen) lw.Open();
                lw.WriteLine("Exception occurred in the filtering process of " + taggedObject.JournalIdentifier + ".");
                lw.WriteLine("Exception: " + ex.GetType() + " : " + ex.Message);
            }

            return Reject;
        }

        protected virtual int InitialProcess(IntPtr select, IntPtr userData)
        {
            if (Masks != null && Masks.Any())
                Ui.SetSelMask(select, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, Masks);
            Ui.SetSelProcs(select, FilterProcess, null, userData);
            return UFConstants.UF_UI_SEL_SUCCESS;
        }
    }
}