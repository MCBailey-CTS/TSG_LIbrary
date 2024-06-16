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


        private void ButtonEditMove_Click(object sender, EventArgs e) => EditMove();

        private void EditMove()
        {
            try
            {
                bool isBlockComponent = SetDispUnits();
                NewMethod2();

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditMoveDisplay(isBlockComponent, editComponent)
                    : EditMoveWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


          private bool EditMoveDisplay(bool isBlockComponent, Component editComponent)
  {
      if (__display_part_.FullPath.Contains("mirror"))
      {
          NewMethod35();
          return isBlockComponent;
      }

      isBlockComponent = __work_part_.__HasDynamicBlock();

      if (isBlockComponent)
      {
          DisableForm();

          if (_isNewSelection)
          {
              CreateEditData(editComponent);
              _isNewSelection = false;
          }
      }

      if (!isBlockComponent)
      {
          ResetNonBlockError();
          NewMethod34();
          return isBlockComponent;
      }

      List<Point> pHandle = new List<Point>();
      pHandle = SelectHandlePoint();
      _isDynamic = false;

      pHandle = NewMethod(pHandle);

      EnableForm();

      return isBlockComponent;
  }



  
    }
}
// 2045