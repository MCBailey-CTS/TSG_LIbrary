using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using System;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;
using NXOpen.Utilities;
using NXOpen.Preferences;
using TSG_Library.Properties;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {



        private void EditDynamicWorkPart(bool isBlockComponent, Component editComponent)
        {
            Part checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
                throw new InvalidOperationException("Mirror COmponent");

            _updateComponent = editComponent;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
                return;

            isBlockComponent = NewMethod12(isBlockComponent, editComponent);
            EditDynamic(isBlockComponent);
            return;
        }

        private void EditDynamicDisplayPart(bool isBlockComponent, Component editComponent)
        {
            if (__display_part_.FullPath.Contains("mirror"))
            {
                NewMethod15();
                return;
            }

            isBlockComponent = __work_part_.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                NewMethod16();
                return;
            }

            DisableForm();

            if (_isNewSelection)
            {
                CreateEditData(editComponent);
                _isNewSelection = false;
            }

            List<Point> pHandle = SelectHandlePoint();
            _isDynamic = true;
            pHandle = NewMethod7(pHandle);
            EnableForm();
            return;
        }


        private void EditDynamic()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component for Dynamic Edit");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    EditDynamicDisplayPart(isBlockComponent, editComponent);
                else
                    EditDynamicWorkPart(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                EnableForm();
                ex.__PrintException();
            }
        }
    }
}
// 2045