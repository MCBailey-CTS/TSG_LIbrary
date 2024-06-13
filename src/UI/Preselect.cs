using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using NXOpen.Utilities;
using static NXOpen.UF.UFConstants;

namespace TSG_Library.Ui
{
    public static class Preselect
    {
        public static List<Component> GetUserSelections(Component[] compList)
        {
            SelectAndDeselectComponents("Select and Deselect Components", ref compList);

            return compList.ToList();
        }

        public static int PreselectComponents(IntPtr select, IntPtr userData)
        {
            PreselectData preselectedData = (PreselectData)Marshal.PtrToStructure(userData, new PreselectData().GetType());

            if(preselectedData.ItemCount > 0)
                UFSession.GetUFSession().Ui
                    .AddToSelList(select, preselectedData.ItemCount, preselectedData.Items, true);

            UFUi.Mask[] maskTriples = new UFUi.Mask[1];
            maskTriples[0].object_type = UF_component_type;
            maskTriples[0].object_subtype = UF_component_subtype;
            maskTriples[0].solid_type = 0;

            UFSession.GetUFSession().Ui
                .SetSelMask(select, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, 1, maskTriples);

            return UF_UI_SEL_SUCCESS;
        }

        public static void SelectAndDeselectComponents(string prompt, ref Component[] theComponents)
        {
            PreselectData preselectComponentsData = new PreselectData { Items = null, ItemCount = 0 };

            if(theComponents != null)
            {
                Tag[] compTags = new Tag[theComponents.Length];
                for (var ii = 0; ii < theComponents.Length; ii++) compTags[ii] = theComponents[ii].Tag;

                preselectComponentsData.Items = compTags;
                preselectComponentsData.ItemCount = theComponents.Length;
            }

            IntPtr preselectIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(preselectComponentsData));

            Marshal.StructureToPtr(preselectComponentsData, preselectIntPtr, false);

            UFSession.GetUFSession().Ui.SetCursorView(0);
            UFSession.GetUFSession().Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

            UFSession.GetUFSession().Ui.SelectWithClassDialog("Select Components", prompt,
                UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY, PreselectComponents, preselectIntPtr, out _, out var cnt,
                out Tag[] theTags);

            UFSession.GetUFSession().Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

            theComponents = new Component[cnt];

            for (var ii = 0; ii < cnt; ii++)
            {
                UFSession.GetUFSession().Disp.SetHighlight(theTags[ii], 0);
                theComponents[ii] = (Component)NXObjectManager.Get(theTags[ii]);
            }
        }

        public struct PreselectData
        {
            public int ItemCount;
            public Tag[] Items;
        }
    }
}