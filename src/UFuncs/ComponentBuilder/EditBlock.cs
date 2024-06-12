using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Preferences;
using NXOpen.UF;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using NXOpenUI;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using Part = NXOpen.Part;
using NXOpen.CAE;
using NXOpen.CAM;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {
        private static Part _workPart = session_.Parts.Work;
        private static Part _displayPart = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = _displayPart;
        private static bool _isDynamic;
        private static Point _udoPointHandle;
        private static double _gridSpace;
        private static Point3d _workCompOrigin;
        private static Matrix3x3 _workCompOrientation;
        private static readonly List<string> _nonValidNames = new List<string>();
        private static readonly List<Line> _edgeRepLines = new List<Line>();
        private static double _distanceMoved;
        private static int _registered;
        private static int _idWorkPartChanged1;
        private static Component _updateComponent;
        private static Body _editBody;
        private static bool _isNewSelection = true;
        private static bool _isUprParallel;
        private static bool _isLwrParallel;
        private static string _parallelHeightExp = string.Empty;
        private static string _parallelWidthExp = string.Empty;

        public EditBlockForm()
        {
            InitializeComponent();
        }



        private void ButtonEditDynamic_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod();

                if (_editBody is null)
                    return;

                var editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                {
                    isBlockComponent = EditDynamicDisplayPart(isBlockComponent, editComponent);
                }
                else
                {
                    isBlockComponent = EditDynamicWorkPart(isBlockComponent, editComponent);
                }
            }
            catch (Exception ex)
            {
                EnableForm();
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }



        private void ButtonViewWcs_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            var orientation = coordSystem.Orientation.Element;
            _displayPart.Views.WorkView.Orient(orientation);
        }
        private void ButtonEditMatch_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection)
                    if (_updateComponent is null)
                        NewMethod4();
                    else
                    {
                        UpdateDynamicBlock(_updateComponent);
                        _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                        _displayPart.WCS.Visibility = true;
                        _isNewSelection = true;
                    }
                else
                {
                    UpdateDynamicBlock(_updateComponent);
                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                    _displayPart.WCS.Visibility = true;
                    _isNewSelection = true;
                }

                EditMatch(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            EnableForm();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonEditSize_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod6();

                if (_editBody is null)
                    return;

                var editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditSizeDisplay(isBlockComponent, editComponent)
                    : EditSizeWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

        private void ButtonEditAlign_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                if (_isNewSelection && _updateComponent is null)
                    NewMethod7();

                EdgeAlign(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonEditMove_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);
                NewMethod2();

                if (_editBody is null)
                    return;

                var editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditMoveDisplay(isBlockComponent, editComponent)
                    : EditMoveWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

        private void ButtonAlignComponent_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod10();

                AlignComponent(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                var isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out var infoUnits);
                var dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod12();

                AlignEdgeDistance(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        private void ComboBoxGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Return))
                return;

            if (comboBoxGridBlock.Text == "0.000")
            {
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            UpdateDynamicBlock(_updateComponent);
            __work_part_ = _originalWorkPart;
            UpdateSessionParts();
            UpdateOriginalParts();
            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            _displayPart.WCS.Visibility = true;
            session_.DeleteAllUndoMarks();
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonApply.Enabled = false;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            UpdateSessionParts();
            UpdateOriginalParts();
            __work_part_ = _displayPart;
        }





       
       

        private double EditAlignNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod51(distance, zAxisLine);
            }

            NewMethod52(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod52(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod51(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private double EditAlignPosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod53(distance, zAxisLine);
            }

            NewMethod54(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod54(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod53(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private double EditAlignNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod55(distance, yAxisLine);
            }

            NewMethod56(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod56(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod55(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private double EditAlignPosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                var addY = new Point3d(mappedEndPoint.X,
                    mappedEndPoint.Y + distance, mappedEndPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetEndPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
            return distance;
        }

        private double EditAlignNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod57(distance, xAxisLine);
            }

            NewMethod58(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod58(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod57(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private double EditAlignPosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod59(distance, xAxisLine);
            }

            NewMethod60(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod60(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod59(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private static void NewMethod3(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void NewMethod1(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private bool EditSizeDisplay(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.FeatureType == "BLOCK")
                    if (featBlk.Name == "DYNAMIC BLOCK")
                        isBlockComponent = true;

            if (isBlockComponent)
            {
                DisableForm();

                if (_isNewSelection)
                {
                    CreateEditData(editComponent);

                    _isNewSelection = false;
                }

                var pHandle = new List<Point>();
                pHandle = SelectHandlePoint();

                _isDynamic = true;

                while (pHandle.Count == 1)
                {
                    HideDynamicHandles();

                    _udoPointHandle = pHandle[0];

                    var blockOrigin = new Point3d();
                    var blockLength = 0.00;
                    var blockWidth = 0.00;
                    var blockHeight = 0.00;

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name == "XBASE1")
                        {
                            blockOrigin = eLine.StartPoint;
                            blockLength = eLine.GetLength();
                        }

                        if (eLine.Name == "YBASE1") blockWidth = eLine.GetLength();

                        if (eLine.Name == "ZBASE1") blockHeight = eLine.GetLength();
                    }

                    Point pointPrototype;

                    if (_udoPointHandle.IsOccurrence)
                        pointPrototype = (Point)_udoPointHandle.Prototype;
                    else
                        pointPrototype = _udoPointHandle;

                    var doNotMovePts = new List<NXObject>();
                    var movePtsHalf = new List<NXObject>();
                    var movePtsFull = new List<NXObject>();

                    if (pointPrototype.Name.Contains("POS"))
                    {
                        NewMethod5(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                    }
                    else
                    {
                        NewMethod8(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                    }

                    var posXObjs = new List<Line>();
                    var negXObjs = new List<Line>();
                    var posYObjs = new List<Line>();
                    var negYObjs = new List<Line>();
                    var posZObjs = new List<Line>();
                    var negZObjs = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" || eLine.Name == "ZBASE1" ||
                           eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                        if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" || eLine.Name == "ZBASE2" ||
                           eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                        if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                           eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                        if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                           eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                        if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                           eLine.Name == "YBASE2") negZObjs.Add(eLine);

                        if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                           eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
                    }

                    var allxAxisLines = new List<Line>();
                    var allyAxisLines = new List<Line>();
                    var allzAxisLines = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                        if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
                    }

                    EditSizeForm sizeForm = null;
                    var convertLength = blockLength / 25.4;
                    var convertWidth = blockWidth / 25.4;
                    var convertHeight = blockHeight / 25.4;

                    if (_displayPart.PartUnits == BasePart.Units.Inches)
                    {
                        if (pointPrototype.Name.Contains("X"))
                        {
                            sizeForm = new EditSizeForm(blockLength);
                            sizeForm.ShowDialog();
                        }

                        if (pointPrototype.Name.Contains("Y"))
                        {
                            sizeForm = new EditSizeForm(blockWidth);
                            sizeForm.ShowDialog();
                        }

                        if (pointPrototype.Name.Contains("Z"))
                        {
                            sizeForm = new EditSizeForm(blockHeight);
                            sizeForm.ShowDialog();
                        }
                    }
                    else
                    {
                        if (pointPrototype.Name.Contains("X"))
                        {
                            sizeForm = new EditSizeForm(convertLength);
                            sizeForm.ShowDialog();
                        }

                        if (pointPrototype.Name.Contains("Y"))
                        {
                            sizeForm = new EditSizeForm(convertWidth);
                            sizeForm.ShowDialog();
                        }

                        if (pointPrototype.Name.Contains("Z"))
                        {
                            sizeForm = new EditSizeForm(convertHeight);
                            sizeForm.ShowDialog();
                        }
                    }

                    if (sizeForm.DialogResult == DialogResult.OK)
                    {
                        var editSize = sizeForm.InputValue;
                        double distance = 0;

                        if (_displayPart.PartUnits == BasePart.Units.Millimeters) editSize *= 25.4;

                        if (editSize > 0)
                        {
                            if (pointPrototype.Name == "POSX")
                            {
                                distance = EditSizeDisplayPosX(blockLength, movePtsHalf, movePtsFull, posXObjs, allxAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGX")
                            {
                                distance = EditSizeDisplayNegX(blockLength, movePtsHalf, movePtsFull, negXObjs, allxAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "POSY")
                            {
                                distance = EditSizeDisplayPosY(blockWidth, movePtsHalf, movePtsFull, posYObjs, allyAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGY")
                            {
                                distance = EditSizeDisplayNegY(blockWidth, movePtsHalf, movePtsFull, negYObjs, allyAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "POSZ")
                            {
                                distance = EditSizeDisplayPosZ(blockHeight, movePtsHalf, movePtsFull, posZObjs, allzAxisLines, editSize);
                            }

                            if (pointPrototype.Name == "NEGZ")
                            {
                                distance = EditSizeDisplayNegZ(blockHeight, movePtsHalf, movePtsFull, negZObjs, allzAxisLines, editSize);
                            }
                        }
                    }

                    UpdateDynamicBlock(editComponent);

                    session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                    sizeForm.Close();
                    sizeForm.Dispose();

                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    __work_component_ = editComponent;
                    UpdateSessionParts();

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();

                    CreateEditData(editComponent);

                    pHandle = SelectHandlePoint();
                }

                EnableForm();
            }
            else
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
            }

            return isBlockComponent;
        }

        private double EditSizeDisplayPosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = editSize - blockHeight;
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                    mappedEndPoint.Z + distance);
                var mappedAddZ = MapWcsToAbsolute(addZ);
                zAxisLine.SetEndPoint(mappedAddZ);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
            return distance;
        }



        private static void NewMethod8(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void NewMethod5(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

       

        private static void EditSizePointsNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
            {
                if (namedPt.Name == "")
                    continue;

                if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    doNotMovePts.Add(namedPt);
                else if (namedPt.Name.Contains("BLKORIGIN"))
                    movePtsFull.Add(namedPt);
                else
                    movePtsHalf.Add(namedPt);
            }

            movePtsFull.Add(pointPrototype);
        }

        private static void EditSizePointsPos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
            {
                if (namedPt.Name == "")
                    continue;

                if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                    doNotMovePts.Add(namedPt);

                else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                    doNotMovePts.Add(namedPt);
                else if (namedPt.Name.Contains("BLKORIGIN"))
                    doNotMovePts.Add(namedPt);
                else
                    movePtsHalf.Add(namedPt);
            }

            movePtsFull.Add(pointPrototype);
        }

        private double EditSizeNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = blockHeight - editSize;
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod49(distance, zAxisLine);
            }

            NewMethod50(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod50(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod49(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private double EditSizePosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = editSize - blockHeight;
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod47(distance, zAxisLine);
            }

            NewMethod48(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod48(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod47(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private double EditSizeNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = blockWidth - editSize;
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod61(distance, yAxisLine);
            }

            NewMethod62(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod62(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod61(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private double EditSizePosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = editSize - blockWidth;
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod45(distance, yAxisLine);
            }

            NewMethod46(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod46(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod45(double distance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X,
                mappedEndPoint.Y + distance, mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private double EditSizeNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = blockLength - editSize;
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod43(distance, xAxisLine);
            }

            NewMethod44(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod44(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod43(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private double EditSizePosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = editSize - blockLength;
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod41(distance, xAxisLine);
            }

            NewMethod42(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod42(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod41(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void AlignComponent(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            var editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                Show();
                TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                    "This function is not allowed in this context");
                return;
            }

            var checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                Show();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return;
            }

            _updateComponent = editComponent;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return;

            if (_isNewSelection)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = editComponent;
                UpdateSessionParts();

                if (_workPart.__HasDynamicBlock())
                {
                    isBlockComponent = true;
                    CreateEditData(editComponent);
                    _isNewSelection = false;
                }
            }
            else
                isBlockComponent = true;

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            var pHandle = new List<Point>();
            pHandle = SelectHandlePoint();

            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                HideDynamicHandles();

                _udoPointHandle = pHandle[0];

                Hide();

                Point pointPrototype;

                if (_udoPointHandle.IsOccurrence)
                    pointPrototype = (Point)_udoPointHandle.Prototype;
                else
                    pointPrototype = _udoPointHandle;

                var movePtsFull = new List<NXObject>();

                foreach (Point nPoint in _workPart.Points)
                    if (nPoint.Name.Contains("X") || nPoint.Name.Contains("Y") ||
                       nPoint.Name.Contains("Z") || nPoint.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(nPoint);

                foreach (Line nLine in _workPart.Lines)
                    if (nLine.Name.Contains("X") || nLine.Name.Contains("Y") ||
                       nLine.Name.Contains("Z"))
                        movePtsFull.Add(nLine);

                var message = "Select Reference Point";
                var pbMethod = UFUi.PointBaseMethod.PointInferred;
                var selection = NXOpen.Tag.Null;
                var basePoint = new double[3];

                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.PointConstruct(message, ref pbMethod, out selection, basePoint,
                    out var response);

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

                if (response == UF_UI_OK)
                {
                    var mappedBase = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, basePoint,
                        UF_CSYS_ROOT_WCS_COORDS, mappedBase);

                    double[] pPrototype =
                    {
                                                pointPrototype.Coordinates.X, pointPrototype.Coordinates.Y,
                                                pointPrototype.Coordinates.Z
                                            };
                    var mappedPoint = new double[3];
                    ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pPrototype,
                        UF_CSYS_ROOT_WCS_COORDS, mappedPoint);

                    double distance;

                    switch (pointPrototype.Name)
                    {
                        case "POSX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "NEGX":
                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                            break;
                        case "POSY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "NEGY":
                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Y");
                            break;
                        case "POSZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        case "NEGZ":
                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

                            MoveObjects(movePtsFull.ToArray(), distance, "Z");
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

                UpdateDynamicBlock(editComponent);

                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;
                session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                __work_component_ = editComponent;
                UpdateSessionParts();

                CreateEditData(editComponent);
                pHandle = SelectHandlePoint();
            }

            Show();
        }


       

        

        private double AlignEdgeDistanceNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod38(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod39(distance, zAxisLine);
            }

            NewMethod40(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod40(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod39(double distance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private static double NewMethod38(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private double AlignEdgeDistancePosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod35(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod36(distance, zAxisLine);
            }

            NewMethod37(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod37(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }

        private void NewMethod36(double distance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + distance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private static double NewMethod35(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[2] - mappedBase[2]);
            if (mappedBase[2] < mappedPoint[2])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private double AlignEdgeDistanceNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod32(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod33(distance, yAxisLine);
            }

            NewMethod34(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod34(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod33(double distance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X,
                mappedStartPoint.Y + distance, mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private static double NewMethod32(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private double AlignEdgeDistancePosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod31(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod30(distance, yAxisLine);
            }

            NewMethod29(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private static double NewMethod31(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[1] - mappedBase[1]);
            if (mappedBase[1] < mappedPoint[1])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private void NewMethod30(double distance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X,
                mappedEndPoint.Y + distance, mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void NewMethod29(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private double AlignEdgeDistanceNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = NewMethod28(inputDist, mappedBase, mappedPoint);

            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod26(distance, xAxisLine);
            }

            NewMethod27(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private static double NewMethod28(double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            return distance;
        }

        private void NewMethod27(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod26(double distance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + distance,
                mappedStartPoint.Y, mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private double AlignEdgeDistancePosX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
        {
            var distance = Math.Abs(mappedPoint[0] - mappedBase[0]);
            if (mappedBase[0] < mappedPoint[0])
            {
                distance *= -1;
                distance += inputDist;
            }
            else
            {
                distance -= inputDist;
            }

            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod25(distance, xAxisLine);
            }

            NewMethod24(movePtsHalf, movePtsFull, distance);
            return distance;
        }

        private void NewMethod25(double distance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + distance,
                mappedEndPoint.Y, mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void NewMethod24(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private static void AlignEdgeDistanceNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void AlignEdgeDistancePos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") &&
                            pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") &&
                            pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void SetDispUnits(Part.Units dispUnits)
        {
            if (dispUnits == Part.Units.Millimeters)
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.GMmNDegC);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults.GMmNDegC);
            }
            else
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.LbmInLbfDegF);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                    .LbmInLbfDegF);
            }
        }


        private void DeleteHandles()
        {
            Session.UndoMarkId markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if (myUdOclass != null)
            {
                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
            }

            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                    session_.UpdateManager.AddToDeleteList(namedPt);

            foreach (var dLine in _edgeRepLines)
                session_.UpdateManager.AddToDeleteList(dLine);

            int errsDelObjs;
            errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

            session_.DeleteUndoMark(markDeleteObjs, "");

            _edgeRepLines.Clear();
        }

       

        private void ShowTemporarySizeText()
        {
            _displayPart.Views.Refresh();

            foreach (var eLine in _edgeRepLines)
            {
                if (eLine.Name != "XBASE1" && eLine.Name != "YBASE1" && eLine.Name != "ZBASE1")
                    continue;

                var view = _displayPart.Views.WorkView.Tag;
                var viewType = UFDisp.ViewType.UseWorkView;
                var dim = string.Empty;

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    var roundDim = Math.Round(eLine.GetLength(), 3);
                    dim = $"{roundDim:0.000}";
                }
                else
                {
                    var roundDim = Math.Round(eLine.GetLength(), 3);
                    dim = $"{roundDim / 25.4:0.000}";
                }

                var midPoint = new double[3];
                var dispProps = new UFObj.DispProps
                {
                    color = 31
                };
                double charSize;
                var font = 1;

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                    charSize = .125;
                else
                    charSize = 3.175;

                midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;

                var mappedPoint = new double[3];

                ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, midPoint, UF_CSYS_ROOT_COORDS, mappedPoint);

                ufsession_.Disp.DisplayTemporaryText(view, viewType, dim, mappedPoint, UFDisp.TextRef.Middlecenter,
                    ref dispProps, charSize, font);
            }
        }
        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length != 0)
                    return;

                BasePart myBasePart = _workPart;
                var myUdOmanager = myBasePart.UserDefinedObjectManager;

                foreach (var blkFace in editBody.GetFaces())
                {
                    var myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                    var myLinks = new UserDefinedObject.LinkDefinition[1];

                    var pointOnFace = new double[3];
                    var dir = new double[3];
                    var box = new double[6];
                    var matrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                    ufsession_.Modl.AskFaceData(blkFace.Tag, out var type, pointOnFace, dir, box,
                        out var radius, out var radData, out var normDir);

                    dir[0] = Math.Round(dir[0], 10);
                    dir[1] = Math.Round(dir[1], 10);
                    dir[2] = Math.Round(dir[2], 10);

                    double[] wcsVectorX =
                        { Math.Round(matrix1.Xx, 10), Math.Round(matrix1.Xy, 10), Math.Round(matrix1.Xz, 10) };
                    double[] wcsVectorY =
                        { Math.Round(matrix1.Yx, 10), Math.Round(matrix1.Yy, 10), Math.Round(matrix1.Yz, 10) };
                    double[] wcsVectorZ =
                        { Math.Round(matrix1.Zx, 10), Math.Round(matrix1.Zy, 10), Math.Round(matrix1.Zz, 10) };

                    var wcsVectorNegX = new double[3];
                    var wcsVectorNegY = new double[3];
                    var wcsVectorNegZ = new double[3];

                    ufsession_.Vec3.Negate(wcsVectorX, wcsVectorNegX);
                    ufsession_.Vec3.Negate(wcsVectorY, wcsVectorNegY);
                    ufsession_.Vec3.Negate(wcsVectorZ, wcsVectorNegZ);

                    // create udo handle points

                    ufsession_.Vec3.IsEqual(dir, wcsVectorX, 0.00, out var isEqualX);

                    if (isEqualX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorY, 0.00, out var isEqualY);

                    if (isEqualY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorZ, 0.00, out var isEqualZ);

                    if (isEqualZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSZ");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegX, 0.00, out var isEqualNegX);

                    if (isEqualNegX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegY, 0.00, out var isEqualNegY);

                    if (isEqualNegY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegZ, 0.00, out var isEqualNegZ);

                    if (isEqualNegZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGZ");
                }

                // create origin point

                CreatePointBlkOrigin();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }
        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if (spacing == 0)
                return cursor;

            return _displayPart.PartUnits == BasePart.Units.Inches
                ? RoundEnglish(spacing, cursor)
                : RoundMetric(spacing, cursor);
        }

        private static double RoundEnglish(double spacing, double cursor)
        {
            var round = Math.Abs(cursor);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing; ii <= 1; ii += spacing)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round;
        }

        private static double RoundMetric(double spacing, double cursor)
        {
            var round = Math.Abs(cursor / 25.4);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round * 25.4;
        }

        private void CreateBlockLines(Point3d wcsOrigin, double lineLength, double lineWidth, double lineHeight)
        {
            var lineColor = 7;
            var mappedStartPoint1 = MapAbsoluteToWcs(wcsOrigin);
            Point3d mappedEndPointX1 = CreateBlockLinesEndPointX1(wcsOrigin, lineLength, mappedStartPoint1, lineColor);
            Point3d mappedEndPointY1 = CreateBlockLinesEndPointY1(wcsOrigin, lineWidth, mappedStartPoint1, lineColor);
            Point3d mappedEndPointZ1 = CreateBlockLinesEndPointZ1(wcsOrigin, lineHeight, mappedStartPoint1, lineColor);
            Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineLength, lineColor, mappedEndPointX1, mappedEndPointY1, "YBASE2");
            Point3d mappedEndPointX1Ceiling;
            CreateBlockLinesEndPointX1Ceiling(lineLength, lineColor, mappedEndPointZ1, out Point3d mappedStartPoint3, out mappedEndPointX1Ceiling);
            Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineWidth, lineColor, mappedEndPointZ1, mappedStartPoint3);
            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineLength, lineColor, mappedEndPointY1Ceiling, mappedStartPoint4);
            CreateBlockLinesYCeiling2(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling, "YCEILING2");
            CreateBlockLinesZBase2(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling, "ZBASE2");
            CreateBlockLinesZBase3(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling, "ZBASE3");
            CreateBlockLinesZBase4(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling, "ZBASE4");
        }

        private static void CreateBlockLines(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointY1Ceiling, string name)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointY1Ceiling);
            yCeiling1.SetName(name);
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private static void CreateBlockLinesZBase4(int lineColor, Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling, string name)
        {
            var zBase4 = _workPart.Curves.CreateLine(mappedEndPointX2, mappedEndPointX2Ceiling);
            zBase4.SetName(name);
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);
        }

        private static void CreateBlockLinesZBase3(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling, string name)
        {
            var zBase3 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointY1Ceiling);
            zBase3.SetName(name);
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);
        }

        private static void CreateBlockLinesZBase2(int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling, string name)
        {
            var zBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX1Ceiling);
            zBase2.SetName(name);
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);
        }

        private static void CreateBlockLinesYCeiling2(int lineColor, Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling, string name)
        {
            var yCeiling2 = _workPart.Curves.CreateLine(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            yCeiling2.SetName(name);
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);
        }

        


        private static void CreateBlockLineYCeiling2(int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling, string name)
        {
            var xCeiling2 = _workPart.Curves.CreateLine(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            xCeiling2.SetName(name);
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);
        }

      
        private static void CreateBlockLinesXBase2(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointX2, string name)
        {
            var xbase2 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointX2);
            xbase2.SetName(name);
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);
        }

        private static void CreateBlockLinesZBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointZ1, string name)
        {
            var zBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointZ1);
            zBase1.SetName(name);
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);
        }

        private static void CreateBlockLinesYBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointY1, string name)
        {
            var yBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointY1);
            yBase1.SetName(name);
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);
        }

        private static void CreateBlockLinesXBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointX1, string name)
        {
            var xBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointX1);
            xBase1.SetName(name);
            xBase1.Color = lineColor;
            xBase1.RedisplayObject();
            _edgeRepLines.Add(xBase1);
        }





        private static void CreatePointBlkOrigin()
        {
            var pointLocationOrigin = _displayPart.WCS.Origin;
            var point1Origin = _workPart.Points.CreatePoint(pointLocationOrigin);
            point1Origin.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1Origin.Blank();
            point1Origin.SetName("BLKORIGIN");
            point1Origin.Layer = _displayPart.Layers.WorkLayer;
            point1Origin.RedisplayObject();
        }

        private static Point CreatePoint(double[] pointOnFace, string name)
        {
            var pointLocation = pointOnFace.__ToPoint3d();
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName(name);
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private Point3d CreateBlockLinesEndPointX2(double lineLength, int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointY1, string name)
        {
            Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineLength, lineColor, mappedEndPointY1);

            var yBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX2);
            yBase2.SetName(name);
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
            return mappedEndPointX2;
        }

        private void CreateBlockLinesEndPointX1Ceiling(double lineLength, int lineColor, Point3d mappedEndPointZ1, out Point3d mappedStartPoint3, out Point3d mappedEndPointX1Ceiling)
        {
            mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            var endPointX1Ceiling = mappedStartPoint3.__AddX(lineLength);
            mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlovkLinesXCeiling2(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling, "XCEILING1");
        }

        private static void CreateBlovkLinesXCeiling2(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling, string name)
        {
            var xCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointX1Ceiling);
            xCeiling1.SetName(name);
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);
        }


        private Point3d CreateBlockLinesEndPOontX2(double lineLength, int lineColor, Point3d mappedEndPointY1)
        {
            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLinesXBase2(lineColor, mappedEndPointY1, mappedEndPointX2, "XBASE2");
            return mappedEndPointX2;
        }

        private Point3d CreateBlockLinesEndPointZ1(Point3d wcsOrigin, double lineHeight, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointZ1 = mappedStartPoint1.__AddZ(lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLinesZBase1(wcsOrigin, lineColor, mappedEndPointZ1, "ZBASE1");
            return mappedEndPointZ1;
        }
        private Point3d CreateBlockLinesEndPointY1(Point3d wcsOrigin, double lineWidth, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointY1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y + lineWidth, mappedStartPoint1.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLinesYBase1(wcsOrigin, lineColor, mappedEndPointY1, "YBASE1");
            return mappedEndPointY1;
        }

        private Point3d CreateBlockLinesEndPointX2Ceiling(double lineLength, int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedStartPoint4)
        {
            var endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            CreateBlockLineYCeiling2(lineColor, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling, "XCEILING2");
            return mappedEndPointX2Ceiling;
        }
        private Point3d CreateBlockLineEndPointY1Ceiling(double lineWidth, int lineColor, Point3d mappedEndPointZ1, Point3d mappedStartPoint3)
        {
            var endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLines(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling, "YCEILING1");
            return mappedEndPointY1Ceiling;
        }

        private Point3d CreateBlockLinesEndPointX1(Point3d wcsOrigin, double lineLength, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointX1 = new Point3d(mappedStartPoint1.X + lineLength, mappedStartPoint1.Y, mappedStartPoint1.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLinesXBase1(wcsOrigin, lineColor, mappedEndPointX1, "XBASE1");
            return mappedEndPointX1;
        }

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, string name)
        {
            Point point1 = CreatePoint(pointOnFace, name);
            CreateUdo(myUdo, myLinks, pointOnFace, point1, name);
        }

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1, string name)
        {
            myUdo.SetName(name);
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }

        private void MoveComponent(Component compToMove)
        {
            try
            {
                UpdateSessionParts();

                if (compToMove is null)
                    return;

                var assmUnits = _displayPart.PartUnits;
                var compBase = (BasePart)compToMove.Prototype;
                var compUnits = compBase.PartUnits;

                if (compUnits != assmUnits)
                    return;

                UpdateSessionParts();
                CreateEditData(compToMove);
                _isNewSelection = false;
                var pHandle = new List<Point>();
                pHandle = SelectHandlePoint();
                _isDynamic = false;

                while (pHandle.Count == 1)
                {
                    _distanceMoved = 0;
                    HideDynamicHandles();
                    _udoPointHandle = pHandle[0];
                    _displayPart.WCS.Visibility = false;
                    var message = "Select New Position";
                    var screenPos = new double[3];
                    var viewTag = NXOpen.Tag.Null;
                    var motionCbData = IntPtr.Zero;
                    var clientData = IntPtr.Zero;
                    ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                    using (session_.__UsingLockUiFromCustom())
                    {
                        var mView = (ModelingView)_displayPart.Views.WorkView;
                        _displayPart.Views.WorkView.Orient(mView.Matrix);
                        _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                        ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                            out viewTag, out var response);

                        if (response != UF_UI_PICK_RESPONSE)
                            continue;

                        UpdateDynamicHandles();
                        ShowDynamicHandles();
                        pHandle = SelectHandlePoint();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void UpdateDynamicHandles()
        {
            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if (myUdOclass is null)
                return;

            UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

            if (currentUdo.Length == 0)
                return;

            foreach (Point pointHandle in __work_part_.Points)
                foreach (var udoHandle in currentUdo)
                    if (pointHandle.Name == udoHandle.Name)
                    {
                        udoHandle.SetDoubles(pointHandle.Coordinates.__ToArray());
                        udoHandle.SetIntegers(new[] { 0 });
                        pointHandle.Unblank();
                    }
        }

        private Point3d MapWcsToAbsolute(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            var output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, input, UF_CSYS_ROOT_COORDS, output);
            return output.__ToPoint3d();
        }

        private Point3d MapAbsoluteToWcs(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            var output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            return output.__ToPoint3d();
        }

        private void ShowDynamicHandles()
        {
            try
            {
                UpdateSessionParts();

                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (var dispUdo in currentUdo)
                {
                    int[] setDisplay = { 1 };
                    dispUdo.SetIntegers(setDisplay);
                    ufsession_.Disp.AddItemToDisplay(dispUdo.Tag);
                }

                foreach (Point udoPoint in _workPart.Points)
                    if (udoPoint.Name != "" && udoPoint.Layer == _displayPart.Layers.WorkLayer)
                    {
                        udoPoint.SetVisibility(SmartObject.VisibilityOption.Visible);
                        udoPoint.RedisplayObject();
                    }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void UpdateDynamicBlock(Component updateComp)
        {
            try
            {
                // set component translucency and update dynamic block

                if (updateComp != null)
                    updateComp.__Translucency(0);
                else
                    foreach (Body dispBody in _workPart.Bodies)
                        if (dispBody.Layer == 1)
                            dispBody.__Translucency(0);

                var blkOrigin = new Point3d();
                var length = "";
                var width = "";
                var height = "";

                foreach (Point pPoint in _workPart.Points)
                {
                    if (pPoint.Name != "BLKORIGIN")
                        continue;

                    blkOrigin.X = pPoint.Coordinates.X;
                    blkOrigin.Y = pPoint.Coordinates.Y;
                    blkOrigin.Z = pPoint.Coordinates.Z;
                }

                foreach (var blkLine in _edgeRepLines)
                {
                    if (blkLine.Name == "XBASE1") length = blkLine.GetLength().ToString();

                    if (blkLine.Name == "YBASE1") width = blkLine.GetLength().ToString();

                    if (blkLine.Name == "ZBASE1") height = blkLine.GetLength().ToString();
                }

                if (_isUprParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                if (_isLwrParallel)
                {
                    width = _parallelHeightExp;
                    height = _parallelWidthExp;
                }

                _displayPart.WCS.SetOriginAndMatrix(blkOrigin, _workCompOrientation);
                _workCompOrigin = blkOrigin;

                __work_component_ = updateComp;
                UpdateSessionParts();

                double[] fromPoint = { blkOrigin.X, blkOrigin.Y, blkOrigin.Z };
                var mappedPoint = new double[3];
                ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, fromPoint, UF_CSYS_WORK_COORDS, mappedPoint);

                blkOrigin.X = mappedPoint[0];
                blkOrigin.Y = mappedPoint[1];
                blkOrigin.Z = mappedPoint[2];

                if (!_workPart.__HasDynamicBlock())
                    return;

                var block2 = _workPart.__DynamicBlock();
                BlockFeatureBuilder builder = _workPart.Features.CreateBlockFeatureBuilder(block2);

                using (session_.__UsingBuilderDestroyer(builder))
                {
                    builder.BooleanOption.Type = BooleanOperation.BooleanType.Create;
                    var targetBodies1 = new Body[1];
                    Body nullBody = null;
                    targetBodies1[0] = nullBody;
                    builder.BooleanOption.SetTargetBodies(targetBodies1);
                    builder.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;
                    var blkFeatBuilderPoint = _workPart.Points.CreatePoint(blkOrigin);
                    blkFeatBuilderPoint.SetCoordinates(blkOrigin);
                    builder.OriginPoint = blkFeatBuilderPoint;
                    var originPoint1 = blkOrigin;
                    builder.SetOriginAndLengths(originPoint1, length, width, height);
                    builder.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);
                    Feature feature1;
                    feature1 = builder.CommitFeature();
                }

                __work_part_ = _displayPart;
                UpdateSessionParts();
                DeleteHandles();
                __work_part_ = _originalWorkPart;
                UpdateSessionParts();
                _displayPart.WCS.Visibility = true;
                _displayPart.Views.Refresh();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void HideDynamicHandles()
        {
            try
            {
                UpdateSessionParts();
                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length == 0)
                    return;

                foreach (var dispUdo in currentUdo)
                {
                    int[] setDisplay = { 0 };
                    dispUdo.SetIntegers(setDisplay);
                }

                foreach (Point namedPt in _workPart.Points)
                    if (namedPt.Name != "")
                        namedPt.Blank();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void UpdateSessionParts()
        {
            _workPart = session_.Parts.Work;
            _displayPart = session_.Parts.Display;
        }

        private void UpdateOriginalParts()
        {
            _originalWorkPart = _workPart;
            _originalDisplayPart = _displayPart;
        }

        private void ResetNonBlockError()
        {
            EnableForm();
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            UpdateSessionParts();
            UpdateOriginalParts();
            __work_part_ = _displayPart;
        }

        private void DisableForm()
        {
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = false;
            buttonEditDynamic.Enabled = false;
            buttonEditMove.Enabled = false;
            buttonEditMatch.Enabled = false;
            buttonEditSize.Enabled = false;
            buttonEditAlign.Enabled = false;
            buttonAlignComponent.Enabled = false;
            buttonAlignEdgeDistance.Enabled = false;
            buttonApply.Enabled = false;
            buttonReset.Enabled = false;
            buttonExit.Enabled = false;
        }

        private void EnableForm()
        {
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = false;
            buttonEditDynamic.Enabled = true;
            buttonEditMove.Enabled = true;
            buttonEditMatch.Enabled = true;
            buttonEditSize.Enabled = true;
            buttonEditAlign.Enabled = true;
            buttonAlignComponent.Enabled = true;
            buttonAlignEdgeDistance.Enabled = true;
            buttonApply.Enabled = true;
            buttonReset.Enabled = false;
            buttonExit.Enabled = false;
        }


        private void MotionCallback(double[] position, ref UFUi.MotionCbData mtnCbData, IntPtr clientData)
        {
            try
            {
                if (_isDynamic)
                    MotionCallbackDyanmic(position);
                else
                    MotionCalbackNotDynamic(position);

                double editBlkLength = 0;
                double editBlkWidth = 0;
                double editBlkHeight = 0;

                foreach (var eLine in _edgeRepLines)
                {
                    if (eLine.Name == "XBASE1")
                        editBlkLength = eLine.GetLength();

                    if (eLine.Name == "YBASE1")
                        editBlkWidth = eLine.GetLength();

                    if (eLine.Name == "ZBASE1")
                        editBlkHeight = eLine.GetLength();
                }

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    ufsession_.Ui.SetPrompt(
                        $"X = {editBlkLength:0.000}  Y = {editBlkWidth:0.000}  Z = {$"{editBlkHeight:0.000}"}  Distance Moved =  {$"{_distanceMoved:0.000}"}");
                    return;
                }

                editBlkLength /= 25.4;
                editBlkWidth /= 25.4;
                editBlkHeight /= 25.4;

                var convertDistMoved = _distanceMoved / 25.4;

                ufsession_.Ui.SetPrompt($"X = {editBlkLength:0.000}  " +
                    $"Y = {editBlkWidth:0.000}  " +
                    $"Z = {editBlkHeight:0.000}  " +
                    $"Distance Moved =  {convertDistMoved:0.000}");
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }



        private void MotionCallbackXDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> negXObjs, List<Line> allxAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0])
                return;

            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace)
                return;

            if (mappedCursor[0] < mappedPoint[0])
                xDistance *= -1;

            _distanceMoved += xDistance;

            if (pointPrototype.Name == "POSX")
                MotionCallbackPosXDyanmic(movePtsHalf, movePtsFull, posXObjs, allxAxisLines, xDistance);
            else
                MotionCallbackNegXDyanmic(movePtsHalf, movePtsFull, negXObjs, allxAxisLines, xDistance);
        }

        private void MotionCallbackZDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> negZObjs, List<Line> allzAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] != mappedCursor[2])
            {
                var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
                zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

                if (zDistance >= _gridSpace)
                {
                    if (mappedCursor[2] < mappedPoint[2]) zDistance *= -1;

                    _distanceMoved += zDistance;

                    if (pointPrototype.Name == "POSZ")
                    {
                        MotionCallbackPosZDyanmic(movePtsHalf, movePtsFull, posZObjs, allzAxisLines, zDistance);
                    }
                    else
                    {
                        MotionCallbackNegZDyanmic(movePtsHalf, movePtsFull, negZObjs, allzAxisLines, zDistance);
                    }

                    var mView1 = (ModelingView)_displayPart.Views.WorkView;
                    _displayPart.Views.WorkView.Orient(mView1.Matrix);
                    _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
                }
            }
        }

        private void MotionCallbackYDynamic(Point pointPrototype, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> negYObjs, List<Line> allyAxisLines, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    if (pointPrototype.Name == "POSY")
                    {
                        MotionCallbackPosYDyanmic(movePtsHalf, movePtsFull, posYObjs, allyAxisLines, yDistance);
                    }
                    else
                    {
                        MotionCallbackNegYDyanmic(movePtsHalf, movePtsFull, negYObjs, allyAxisLines, yDistance);
                    }
                }
            }
        }

        private void MotionCallbackNegZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod9(zDistance, zAxisLine);
            }

            NewMethod15(movePtsHalf, movePtsFull, zDistance);
        }

        private void NewMethod15(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double zDistance)
        {
            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }

        private void NewMethod9(double zDistance, Line zAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
            var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                mappedStartPoint.Z + zDistance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetStartPoint(mappedAddZ);
        }

        private void MotionCallbackPosZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                NewMethod13(zDistance, zAxisLine);
            }

            NewMethod14(movePtsHalf, movePtsFull, zDistance);
        }

        private void NewMethod14(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double zDistance)
        {
            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }

        private void NewMethod13(double zDistance, Line zAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
            var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                mappedEndPoint.Z + zDistance);
            var mappedAddZ = MapWcsToAbsolute(addZ);
            zAxisLine.SetEndPoint(mappedAddZ);
        }

        private void MotionCallbackNegYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod16(yDistance, yAxisLine);
            }

            NewMethod11(movePtsHalf, movePtsFull, yDistance);
        }

        private void NewMethod16(double yDistance, Line yAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
            var addY = new Point3d(mappedStartPoint.X, mappedStartPoint.Y + yDistance,
                mappedStartPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetStartPoint(mappedAddY);
        }

        private void NewMethod11(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double yDistance)
        {
            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }

        private void MotionCallbackPosYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                NewMethod17(yDistance, yAxisLine);
            }

            NewMethod18(movePtsHalf, movePtsFull, yDistance);
        }

        private void NewMethod18(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double yDistance)
        {
            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }

        private void NewMethod17(double yDistance, Line yAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
            var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + yDistance,
                mappedEndPoint.Z);
            var mappedAddY = MapWcsToAbsolute(addY);
            yAxisLine.SetEndPoint(mappedAddY);
        }

        private void MotionCallbackNegXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod19(xDistance, xAxisLine);
            }

            NewMethod20(movePtsHalf, movePtsFull, xDistance);
        }

        private void NewMethod20(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double xDistance)
        {
            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
        }

        private void NewMethod19(double xDistance, Line xAxisLine)
        {
            var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
            var addX = new Point3d(mappedStartPoint.X + xDistance, mappedStartPoint.Y,
                mappedStartPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetStartPoint(mappedAddX);
        }

        private void MotionCallbackPosXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                NewMethod21(xDistance, xAxisLine);
            }

            NewMethod22(movePtsHalf, movePtsFull, xDistance);
        }

        private void NewMethod22(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double xDistance)
        {
            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
        }

        private void NewMethod21(double xDistance, Line xAxisLine)
        {
            var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
            var addX = new Point3d(mappedEndPoint.X + xDistance, mappedEndPoint.Y,
                mappedEndPoint.Z);
            var mappedAddX = MapWcsToAbsolute(addX);
            xAxisLine.SetEndPoint(mappedAddX);
        }

        private void MotionCallbackDyanmic(double[] position)
        {
            var pointPrototype = _udoPointHandle.IsOccurrence
                                    ? (Point)_udoPointHandle.Prototype
                                    : _udoPointHandle;

            var doNotMovePts = new List<NXObject>();
            var movePtsHalf = new List<NXObject>();
            var movePtsFull = new List<NXObject>();

            if (pointPrototype.Name.Contains("POS"))
                MotionCallbackDynamicPos(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
            else
                MotionCallbackDynamicNeg(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);

            var posXObjs = new List<Line>();
            var negXObjs = new List<Line>();
            var posYObjs = new List<Line>();
            var negYObjs = new List<Line>();
            var posZObjs = new List<Line>();
            var negZObjs = new List<Line>();

            foreach (var eLine in _edgeRepLines)
            {
                switch (eLine.Name)
                {
                    case "YBASE1":
                    case "YCEILING1":
                    case "ZBASE1":
                    case "ZBASE3":
                        negXObjs.Add(eLine);
                        break;
                }

                switch (eLine.Name)
                {
                    case "YBASE2":
                    case "YCEILING2":
                    case "ZBASE2":
                    case "ZBASE4":
                        posXObjs.Add(eLine);
                        break;
                }

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" || eLine.Name == "ZBASE1" ||
                   eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" || eLine.Name == "ZBASE3" ||
                   eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" || eLine.Name == "YBASE1" ||
                   eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" || eLine.Name == "YCEILING1" ||
                   eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }

            var allxAxisLines = new List<Line>();
            var allyAxisLines = new List<Line>();
            var allzAxisLines = new List<Line>();

            foreach (var eLine in _edgeRepLines)
            {
                if (eLine.Name.StartsWith("X")) allxAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Y")) allyAxisLines.Add(eLine);

                if (eLine.Name.StartsWith("Z")) allzAxisLines.Add(eLine);
            }

            // get the distance from the selected point to the cursor location

            double[] pointStart =
                { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

            var mappedPoint = new double[3];
            var mappedCursor = new double[3];

            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackXDynamic(pointPrototype, movePtsHalf, movePtsFull, posXObjs, negXObjs, allxAxisLines, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackYDynamic(pointPrototype, movePtsHalf, movePtsFull, posYObjs, negYObjs, allyAxisLines, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackZDynamic(pointPrototype, movePtsHalf, movePtsFull, posZObjs, negZObjs, allzAxisLines, mappedPoint, mappedCursor);
        }





        private void MotionCalbackNotDynamic(double[] position)
        {
            Point pointPrototype;

            if (_udoPointHandle.IsOccurrence)
                pointPrototype = (Point)_udoPointHandle.Prototype;
            else
                pointPrototype = _udoPointHandle;

            var moveAll = new List<NXObject>();

            foreach (Point namedPts in _workPart.Points)
                if (namedPts.Name != "")
                    moveAll.Add(namedPts);

            foreach (var eLine in _edgeRepLines) moveAll.Add(eLine);

            // get the distance from the selected point to the cursor location

            double[] pointStart =
                { _udoPointHandle.Coordinates.X, _udoPointHandle.Coordinates.Y, _udoPointHandle.Coordinates.Z };

            var mappedPoint = new double[3];
            var mappedCursor = new double[3];

            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, pointStart, UF_CSYS_ROOT_WCS_COORDS, mappedPoint);
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, position, UF_CSYS_ROOT_WCS_COORDS, mappedCursor);

            if (pointPrototype.Name == "POSX" || pointPrototype.Name == "NEGX")
                MotionCallbackNotDynamicX(moveAll, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSY" || pointPrototype.Name == "NEGY")
                MotionCallbackNotDynamicY(moveAll, mappedPoint, mappedCursor);

            if (pointPrototype.Name == "POSZ" || pointPrototype.Name == "NEGZ")
                MotionCallbackNotDynamicZ(moveAll, mappedPoint, mappedCursor);
        }

        private void MotionCallbackNotDynamicX(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[0] == mappedCursor[0])
            {
                return;
            }
            var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

            if (xDistance < _gridSpace)
            {
                return;
            }
            if (mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

            _distanceMoved += xDistance;

            MoveObjects(moveAll.ToArray(), xDistance, "X");
        }

        private void MotionCallbackNotDynamicY(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[1] != mappedCursor[1])
            {
                var yDistance = Math.Sqrt(Math.Pow(mappedPoint[1] - mappedCursor[1], 2));

                if (yDistance >= _gridSpace)
                {
                    if (mappedCursor[1] < mappedPoint[1]) yDistance *= -1;

                    _distanceMoved += yDistance;

                    MoveObjects(moveAll.ToArray(), yDistance, "Y");
                }
            }
        }

        private void MotionCallbackNotDynamicZ(List<NXObject> moveAll, double[] mappedPoint, double[] mappedCursor)
        {
            if (mappedPoint[2] == mappedCursor[2])
                return;

            var zDistance = Math.Sqrt(Math.Pow(mappedPoint[2] - mappedCursor[2], 2));
            zDistance = RoundDistanceToGrid(_gridSpace, zDistance);

            if (zDistance < _gridSpace)
                return;

            if (mappedCursor[2] < mappedPoint[2])
                zDistance *= -1;

            _distanceMoved += zDistance;
            MoveObjects(moveAll.ToArray(), zDistance, "Z");
            var mView1 = (ModelingView)_displayPart.Views.WorkView;
            _displayPart.Views.WorkView.Orient(mView1.Matrix);
            _displayPart.WCS.SetOriginAndMatrix(mView1.Origin, mView1.Matrix);
        }


        private static void MotionCallbackDynamicNeg(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        movePtsFull.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }

        private static void MotionCallbackDynamicPos(Point pointPrototype, List<NXObject> doNotMovePts, List<NXObject> movePtsHalf, List<NXObject> movePtsFull)
        {
            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                {
                    if (namedPt.Name.Contains("X") && pointPrototype.Name.Contains("X"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Y") && pointPrototype.Name.Contains("Y"))
                        doNotMovePts.Add(namedPt);

                    else if (namedPt.Name.Contains("Z") && pointPrototype.Name.Contains("Z"))
                        doNotMovePts.Add(namedPt);
                    else if (namedPt.Name.Contains("BLKORIGIN"))
                        doNotMovePts.Add(namedPt);
                    else
                        movePtsHalf.Add(namedPt);
                }

            movePtsFull.Add(pointPrototype);
        }





        private void MoveObjects(NXObject[] objsToMove, double distance, string deltaXyz)
        {
            try
            {
                if (distance == 0)
                    return;

                _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                MoveObject nullFeaturesMoveObject = null;

                MoveObjectBuilder moveObjectBuilder1;
                moveObjectBuilder1 = _workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeaturesMoveObject);

                moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.AxisOption =
                    OrientXpressBuilder.Axis.Passive;

                moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.PlaneOption =
                    OrientXpressBuilder.Plane.Passive;

                moveObjectBuilder1.TransformMotion.OrientXpress.AxisOption = OrientXpressBuilder.Axis.Passive;

                moveObjectBuilder1.TransformMotion.OrientXpress.PlaneOption = OrientXpressBuilder.Plane.Passive;

                Point3d manipulatororigin1;
                manipulatororigin1 = _displayPart.WCS.Origin;

                Matrix3x3 manipulatormatrix1;
                manipulatormatrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                moveObjectBuilder1.TransformMotion.Option = ModlMotion.Options.DeltaXyz;

                moveObjectBuilder1.TransformMotion.DeltaEnum = ModlMotion.Delta.ReferenceWcsWorkPart;

                if (deltaXyz == "X")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = distance.ToString();
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                }

                if (deltaXyz == "Y")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = distance.ToString();
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                }

                if (deltaXyz == "Z")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = distance.ToString();
                }

                bool added1;
                added1 = moveObjectBuilder1.ObjectToMoveObject.Add(objsToMove);

                NXObject nXObject1;
                nXObject1 = moveObjectBuilder1.Commit();

                moveObjectBuilder1.Destroy();

                _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }



        private static void NewMethod()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component for Dynamic Edit");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }



        private static void NewMethod2()
        {
            if (_isNewSelection)
                if (_updateComponent == null)
                {
                    NewMethod23();
                }
        }

        private static void NewMethod23()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Move");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private static void NewMethod4()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component - Match From");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }


        private static void NewMethod6()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Edit Size");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private static void NewMethod7()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }



        private static void NewMethod10()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }






        private static void NewMethod12()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align Edge");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }







        private void CreateEditData(Component setCompTranslucency)
        {
            try
            {
                // set component translucency

                if (setCompTranslucency != null)
                    setCompTranslucency.__Translucency(75);
                else
                    foreach (Body dispBody in _workPart.Bodies)
                        if (dispBody.Layer == 1)
                            dispBody.__Translucency(75);

                SetWcsToWorkPart(setCompTranslucency);

                if (_workPart.__HasDynamicBlock())
                {
                    // get current block feature
                    var block1 = _workPart.__DynamicBlock();

                    BlockFeatureBuilder blockFeatureBuilderMatch;
                    blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                    var bOrigin = blockFeatureBuilderMatch.Origin;
                    var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                    var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                    var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                    var mLength = blockFeatureBuilderMatch.Length.Value;
                    var mWidth = blockFeatureBuilderMatch.Width.Value;
                    var mHeight = blockFeatureBuilderMatch.Height.Value;

                    __work_part_ = _displayPart;
                    UpdateSessionParts();

                    if (mLength != 0 && mWidth != 0 && mHeight != 0)
                    {

                        // create edit block feature
                        Feature nullFeaturesFeature = null;
                        BlockFeatureBuilder blockFeatureBuilderEdit;
                        blockFeatureBuilderEdit =
                            _displayPart.Features.CreateBlockFeatureBuilder(nullFeaturesFeature);

                        blockFeatureBuilderEdit.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                        var targetBodies1 = new Body[1];
                        Body nullBody = null;
                        targetBodies1[0] = nullBody;
                        blockFeatureBuilderEdit.BooleanOption.SetTargetBodies(targetBodies1);

                        blockFeatureBuilderEdit.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                        var originPoint1 = _displayPart.WCS.Origin;

                        blockFeatureBuilderEdit.SetOriginAndLengths(originPoint1, mLength.ToString(),
                            mWidth.ToString(), mHeight.ToString());

                        blockFeatureBuilderEdit.SetBooleanOperationAndTarget(Feature.BooleanType.Create,
                            nullBody);

                        Feature feature1;
                        feature1 = blockFeatureBuilderEdit.CommitFeature();

                        feature1.SetName("EDIT BLOCK");

                        var editBlock = (Block)feature1;
                        var editBody = editBlock.GetBodies();

                        CreateBlockLines(originPoint1, mLength, mWidth, mHeight);

                        blockFeatureBuilderMatch.Destroy();
                        blockFeatureBuilderEdit.Destroy();

                        _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                        CreateDynamicHandleUdo(editBody[0]);
                        ShowDynamicHandles();
                        ShowTemporarySizeText();

                        Session.UndoMarkId markDeleteObjs1;
                        markDeleteObjs1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
                        session_.UpdateManager.AddToDeleteList(feature1);
                        session_.UpdateManager.DoUpdate(markDeleteObjs1);
                        session_.DeleteUndoMark(markDeleteObjs1, "");
                    }
                }
            }
            catch (Exception ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                ex.__PrintException();
            }
        }


        private void SetWcsToWorkPart(Component compRefCsys)
        {
            try
            {
                if (compRefCsys == null)
                {
                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;

                    foreach (Expression exp in _workPart.Expressions)
                    {
                        if (exp.Name == "uprParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                                _isUprParallel = true;
                            else
                                _isUprParallel = false;
                        }

                        if (exp.Name == "lwrParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                                _isLwrParallel = true;
                            else
                                _isLwrParallel = false;
                        }
                    }

                    foreach (Feature featBlk in _workPart.Features)
                        if (featBlk.FeatureType == "BLOCK")
                            if (featBlk.Name == "DYNAMIC BLOCK")
                            {
                                var block1 = (Block)featBlk;

                                BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                                var bOrigin = blockFeatureBuilderMatch.Origin;
                                var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                                var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                                var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                                var mLength = blockFeatureBuilderMatch.Length.Value;
                                var mWidth = blockFeatureBuilderMatch.Width.Value;
                                var mHeight = blockFeatureBuilderMatch.Height.Value;

                                if (_isUprParallel)
                                {
                                    _parallelHeightExp = "uprParallelHeight";
                                    _parallelWidthExp = "uprParallelWidth";
                                }

                                if (_isLwrParallel)
                                {
                                    _parallelHeightExp = "lwrParallelHeight";
                                    _parallelWidthExp = "lwrParallelWidth";
                                }

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                                _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                                    setTempCsys.Orientation.Element);
                                _workCompOrigin = setTempCsys.Origin;
                                _workCompOrientation = setTempCsys.Orientation.Element;
                            }
                }
                else
                {
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                    var compBase = (BasePart)compRefCsys.Prototype;

                    __display_part_ = (Part)compBase;
                    UpdateSessionParts();

                    _isUprParallel = false;
                    _isLwrParallel = false;
                    _parallelHeightExp = string.Empty;
                    _parallelWidthExp = string.Empty;

                    foreach (Expression exp in _workPart.Expressions)
                    {
                        if (exp.Name == "uprParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                                _isUprParallel = true;
                            else
                                _isUprParallel = false;
                        }

                        if (exp.Name == "lwrParallel")
                        {
                            if (exp.RightHandSide.Contains("yes"))
                                _isLwrParallel = true;
                            else
                                _isLwrParallel = false;
                        }
                    }

                    foreach (Feature featBlk in _workPart.Features)
                        if (featBlk.FeatureType == "BLOCK")
                            if (featBlk.Name == "DYNAMIC BLOCK")
                            {
                                var block1 = (Block)featBlk;

                                BlockFeatureBuilder blockFeatureBuilderMatch;
                                blockFeatureBuilderMatch = _workPart.Features.CreateBlockFeatureBuilder(block1);
                                var bOrigin = blockFeatureBuilderMatch.Origin;
                                var blength = blockFeatureBuilderMatch.Length.RightHandSide;
                                var bwidth = blockFeatureBuilderMatch.Width.RightHandSide;
                                var bheight = blockFeatureBuilderMatch.Height.RightHandSide;
                                var mLength = blockFeatureBuilderMatch.Length.Value;
                                var mWidth = blockFeatureBuilderMatch.Width.Value;
                                var mHeight = blockFeatureBuilderMatch.Height.Value;

                                if (_isUprParallel)
                                {
                                    _parallelHeightExp = "uprParallelHeight";
                                    _parallelWidthExp = "uprParallelWidth";
                                }

                                if (_isLwrParallel)
                                {
                                    _parallelHeightExp = "lwrParallelHeight";
                                    _parallelWidthExp = "lwrParallelWidth";
                                }

                                blockFeatureBuilderMatch.GetOrientation(out var xAxis, out var yAxis);

                                double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                                double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                                double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                                var initMatrix = new double[9];
                                ufsession_.Mtx3.Initialize(xVector, yVector, initMatrix);
                                ufsession_.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                                ufsession_.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);
                                var setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);

                                _displayPart.WCS.SetOriginAndMatrix(setTempCsys.Origin,
                                    setTempCsys.Orientation.Element);

                                var featBlkCsys = _displayPart.WCS.Save();
                                featBlkCsys.SetName("EDITCSYS");
                                featBlkCsys.Layer = 254;

                                NXObject[] addToBody = { featBlkCsys };

                                foreach (var bRefSet in _displayPart.GetAllReferenceSets())
                                    if (bRefSet.Name == "BODY")
                                        bRefSet.AddObjectsToReferenceSet(addToBody);

                                __display_part_ = _originalDisplayPart;
                                __work_component_ = compRefCsys;
                                UpdateSessionParts();

                                foreach (CartesianCoordinateSystem wpCsys in _workPart.CoordinateSystems)
                                    if (wpCsys.Layer == 254)
                                        if (wpCsys.Name == "EDITCSYS")
                                        {
                                            NXObject csysOccurrence;
                                            csysOccurrence = session_.Parts.WorkComponent.FindOccurrence(wpCsys);

                                            var editCsys = (CartesianCoordinateSystem)csysOccurrence;

                                            if (editCsys != null)
                                            {
                                                _displayPart.WCS.SetOriginAndMatrix(editCsys.Origin,
                                                    editCsys.Orientation.Element);
                                                _workCompOrigin = editCsys.Origin;
                                                _workCompOrientation = editCsys.Orientation.Element;
                                            }

                                            Session.UndoMarkId markDeleteObjs;
                                            markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

                                            session_.UpdateManager.AddToDeleteList(wpCsys);

                                            int errsDelObjs;
                                            errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

                                            session_.DeleteUndoMark(markDeleteObjs, "");
                                        }
                            }

                    ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                    ufsession_.Disp.RegenerateDisplay();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }



        private double EditSizeDisplayNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = blockWidth - editSize;
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                var addY = new Point3d(mappedStartPoint.X,
                    mappedStartPoint.Y + distance, mappedStartPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetStartPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
            return distance;
        }

        private double EditSizeDisplayPosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = editSize - blockWidth;
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + distance,
                    mappedEndPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetEndPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
            return distance;
        }

        private double EditSizeDisplayPosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = editSize - blockLength;
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                var addX = new Point3d(mappedEndPoint.X + distance, mappedEndPoint.Y,
                    mappedEndPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetEndPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
            return distance;
        }

        private double EditSizeDisplayNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = blockLength - editSize;
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                var addX = new Point3d(mappedStartPoint.X + distance,
                    mappedStartPoint.Y, mappedStartPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetStartPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
            return distance;
        }

        private double EditSizeDisplayNegZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double editSize)
        {
            var distance = blockHeight - editSize;
            foreach (var addLine in negZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                    mappedStartPoint.Z + distance);
                var mappedAddZ = MapWcsToAbsolute(addZ);
                zAxisLine.SetStartPoint(mappedAddZ);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
            return distance;
        }


    }
}
// 4839