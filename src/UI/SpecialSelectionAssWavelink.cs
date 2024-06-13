using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using static NXOpen.Selection;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Utilities
{
    public sealed class SpecialSelectionAssWavelink<T> where T : DisplayableObject
    {
        // ReSharper disable once InconsistentNaming
        private readonly UFUi.Mask[] Masks;

        // ReSharper disable once InconsistentNaming
        private readonly string Prompt;

        // ReSharper disable once InconsistentNaming
        private readonly SelectionScope Scope;

        // ReSharper disable once InconsistentNaming
        private readonly Predicate<T> SelectionPredicate;

        internal SpecialSelectionAssWavelink(Predicate<T> selectionPredicate, SelectionScope scope, string prompt,
            params UFUi.Mask[] masks)
        {
            SelectionPredicate = selectionPredicate;
            Scope = scope;
            Masks = masks;
            Prompt = prompt;
        }

        internal T SelectSingle()
        {
            return Select(false)[0];
        }

        internal List<T> SelectMultiple()
        {
            return Select(true).ToList();
        }

        private T[] Select(bool multiple)
        {
            try
            {
                UFUi.SelInitFnT initialProcess = null;
                if(SelectionPredicate != null) initialProcess = InitialProcess;
                ufsession_.Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
                T[] result;
                if(multiple)
                {
                    ufsession_.Ui.SelectWithClassDialog(Prompt, Prompt, (int)Scope, initialProcess, IntPtr.Zero,
                        out var _, out var _, out Tag[] selectedObj);
                    result = selectedObj.Select(x => (T)NXObjectManager.Get(x)).ToArray();
                }
                else
                {
                    var cursor = new double[3];
                    ufsession_.Ui.SelectWithSingleDialog(Prompt, Prompt, (int)Scope, initialProcess, IntPtr.Zero,
                        out var _, out Tag selectedObj, cursor, out _);
                    result = new[] { (T)NXObjectManager.Get(selectedObj) };
                }

                result.ToList().ForEach(x => x.Unhighlight());
                return result;
            }
            finally
            {
                ufsession_.Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
            }
        }

        private int FilterProcess(Tag _object, int[] type, IntPtr userData, IntPtr select)
        {
            if(!(NXObjectManager.Get(_object) is T obj))
                return 0;
            try
            {
                return SelectionPredicate(obj) ? 1 : 0;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return 0;
        }

        private int InitialProcess(IntPtr select, IntPtr userData)
        {
            if(Masks != null && Masks.Any())
                ufsession_.Ui.SetSelMask(select, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, Masks);
            ufsession_.Ui.SetSelProcs(select, FilterProcess, null, userData);
            return UFConstants.UF_UI_SEL_SUCCESS;
        }
    }
}