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

  private bool EditSizeWork(bool isBlockComponent, Component editComponent)
  {
      _updateComponent = editComponent;

      if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
          return isBlockComponent;

      isBlockComponent = NewMethod51(isBlockComponent, editComponent);

      if (isBlockComponent)
      {
          UpdateDynamicBlock(editComponent);
          CreateEditData(editComponent);
          DisableForm();
          var pHandle = SelectHandlePoint();
          _isDynamic = true;

          while (pHandle.Count == 1)
                {
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];
                    Point3d blockOrigin = new Point3d();
                    double blockLength = 0.00;
                    double blockWidth = 0.00;
                    double blockHeight = 0.00;

                    foreach (Line eLine in _edgeRepLines)
                    {
                        if (eLine.Name == "XBASE1")
                        {
                            blockOrigin = eLine.StartPoint;
                            blockLength = eLine.GetLength();
                        }

                        if (eLine.Name == "YBASE1")
                            blockWidth = eLine.GetLength();

                        if (eLine.Name == "ZBASE1")
                            blockHeight = eLine.GetLength();
                    }

                    Point pointPrototype = _udoPointHandle.IsOccurrence
                        ? (Point)_udoPointHandle.Prototype
                        : _udoPointHandle;

                    MotionCallbackDynamic1(pointPrototype, out var doNotMovePts, out var movePtsHalf, out var movePtsFull, pointPrototype.Name.Contains("POS"));
                    GetLines(out var posXObjs, out var negXObjs, out var posYObjs, out var negYObjs, out var posZObjs, out var negZObjs);
                    List<Line> allxAxisLines, allyAxisLines, allzAxisLines;
                    NewMethod40(out allxAxisLines, out allyAxisLines, out allzAxisLines);

                    EditSizeForm sizeForm = null;
                    double convertLength = blockLength / 25.4;
                    double convertWidth = blockWidth / 25.4;
                    double convertHeight = blockHeight / 25.4;
                    sizeForm = NewMethod17(blockLength, blockWidth, blockHeight, pointPrototype, sizeForm, convertLength, convertWidth, convertHeight);

                    if (sizeForm.DialogResult == DialogResult.OK)
                    {
                        double editSize = sizeForm.InputValue;

                        if (__display_part_.PartUnits == BasePart.Units.Millimeters)
                            editSize *= 25.4;

                        if (editSize > 0)
                            switch (pointPrototype.Name)
                            {
                                case "POSX":
                                    movePtsFull.AddRange(posXObjs);
                                    EditSizeOrAlign(editSize - blockLength, movePtsHalf, movePtsFull, allxAxisLines, "X", true);
                                    break;
                                case "NEGX":
                                    movePtsFull.AddRange(negXObjs);
                                    EditSizeOrAlign(blockLength - editSize, movePtsHalf, movePtsFull, allxAxisLines, "X", false);
                                    break;
                                case "POSY":
                                    movePtsFull.AddRange(posYObjs);
                                    EditSizeOrAlign(editSize - blockWidth, movePtsHalf, movePtsFull, allyAxisLines, "Y", true);
                                    break;
                                case "NEGY":
                                    movePtsFull.AddRange(negYObjs);
                                    EditSizeOrAlign(blockWidth - editSize, movePtsHalf, movePtsFull, allyAxisLines, "Y", false);
                                    break;
                                case "POSZ":
                                    movePtsFull.AddRange(posZObjs);
                                    EditSizeOrAlign(editSize - blockHeight, movePtsHalf, movePtsFull, allzAxisLines, "Z", true);
                                    break;
                                case "NEGZ":
                                    movePtsFull.AddRange(negZObjs);
                                    EditSizeOrAlign(blockHeight - editSize, movePtsHalf, movePtsFull, allzAxisLines, "Z", false);
                                    break;
                                default:
                                    throw new InvalidOperationException(pointPrototype.Name);
                            }
                    }

                    sizeForm.Close();
                    sizeForm.Dispose();
                    pHandle = NewMethod8(editComponent);
                }

                EnableForm();
      }
      else
      {
          ResetNonBlockError();
          NewMethod28();
          return isBlockComponent;
      }

      return isBlockComponent;
  }

        private List<Point> NewMethod8(Component editComponent)
        {
            List<Point> pHandle;
            UpdateDynamicBlock(editComponent);
            __work_component_ = editComponent;
            CreateEditData(editComponent);
            pHandle = SelectHandlePoint();
            return pHandle;
        }

        private void EditSize()
 {
     try
     {
         bool isBlockComponent = SetDispUnits();

         if (_isNewSelection && _updateComponent is null)
             SelectWithFilter_("Select Component to Edit Size");

         if (_editBody is null)
             return;

         Component editComponent = _editBody.OwningComponent;

         isBlockComponent = editComponent is null
             ? EditSize(isBlockComponent, editComponent)
             : EditSizeWork(isBlockComponent, editComponent);
     }
     catch (Exception ex)
     {
         ex.__PrintException();
     }
 }

    }
}
// 2045