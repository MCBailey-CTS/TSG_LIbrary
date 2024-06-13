using NXOpen.UF;
using NXOpen;
using System;
using TSG_Library.Extensions;
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
using System.CodeDom;

namespace TSG_Library.UFuncs
{
    partial class EditBlockForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxEditBlock = new System.Windows.Forms.GroupBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonAlignEdgeDistance = new System.Windows.Forms.Button();
            this.buttonAlignComponent = new System.Windows.Forms.Button();
            this.buttonEditDynamic = new System.Windows.Forms.Button();
            this.buttonEditMove = new System.Windows.Forms.Button();
            this.buttonEditAlign = new System.Windows.Forms.Button();
            this.buttonEditSize = new System.Windows.Forms.Button();
            this.buttonEditMatch = new System.Windows.Forms.Button();
            this.groupBoxWorkPlane = new System.Windows.Forms.GroupBox();
            this.buttonViewWcs = new System.Windows.Forms.Button();
            this.comboBoxGridBlock = new System.Windows.Forms.ComboBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonEndEditConstruction = new System.Windows.Forms.Button();
            this.buttonEditConstruction = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBoxEditBlock.SuspendLayout();
            this.groupBoxWorkPlane.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxEditBlock
            // 
            this.groupBoxEditBlock.Controls.Add(this.buttonApply);
            this.groupBoxEditBlock.Controls.Add(this.buttonAlignEdgeDistance);
            this.groupBoxEditBlock.Controls.Add(this.buttonAlignComponent);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditDynamic);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditMove);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditAlign);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditSize);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditMatch);
            this.groupBoxEditBlock.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxEditBlock.Location = new System.Drawing.Point(11, 120);
            this.groupBoxEditBlock.Name = "groupBoxEditBlock";
            this.groupBoxEditBlock.Size = new System.Drawing.Size(192, 195);
            this.groupBoxEditBlock.TabIndex = 8;
            this.groupBoxEditBlock.TabStop = false;
            this.groupBoxEditBlock.Text = "Edit Block";
            // 
            // buttonApply
            // 
            this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonApply.Location = new System.Drawing.Point(6, 155);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(178, 30);
            this.buttonApply.TabIndex = 31;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // buttonAlignEdgeDistance
            // 
            this.buttonAlignEdgeDistance.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAlignEdgeDistance.Location = new System.Drawing.Point(6, 129);
            this.buttonAlignEdgeDistance.Name = "buttonAlignEdgeDistance";
            this.buttonAlignEdgeDistance.Size = new System.Drawing.Size(178, 20);
            this.buttonAlignEdgeDistance.TabIndex = 10;
            this.buttonAlignEdgeDistance.Text = "Align Edge Distance";
            this.buttonAlignEdgeDistance.UseVisualStyleBackColor = true;
            this.buttonAlignEdgeDistance.Click += new System.EventHandler(this.ButtonAlignEdgeDistance_Click);
            // 
            // buttonAlignComponent
            // 
            this.buttonAlignComponent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAlignComponent.Location = new System.Drawing.Point(6, 103);
            this.buttonAlignComponent.Name = "buttonAlignComponent";
            this.buttonAlignComponent.Size = new System.Drawing.Size(178, 20);
            this.buttonAlignComponent.TabIndex = 9;
            this.buttonAlignComponent.Text = "Align Component";
            this.buttonAlignComponent.UseVisualStyleBackColor = true;
            this.buttonAlignComponent.Click += new System.EventHandler(this.ButtonAlignComponent_Click);
            // 
            // buttonEditDynamic
            // 
            this.buttonEditDynamic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditDynamic.Location = new System.Drawing.Point(6, 19);
            this.buttonEditDynamic.Name = "buttonEditDynamic";
            this.buttonEditDynamic.Size = new System.Drawing.Size(178, 20);
            this.buttonEditDynamic.TabIndex = 0;
            this.buttonEditDynamic.Text = "Dynamic";
            this.buttonEditDynamic.UseVisualStyleBackColor = true;
            this.buttonEditDynamic.Click += new System.EventHandler(this.ButtonEditDynamic_Click);
            // 
            // buttonEditMove
            // 
            this.buttonEditMove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditMove.Location = new System.Drawing.Point(6, 48);
            this.buttonEditMove.Name = "buttonEditMove";
            this.buttonEditMove.Size = new System.Drawing.Size(85, 20);
            this.buttonEditMove.TabIndex = 1;
            this.buttonEditMove.Text = "Move";
            this.buttonEditMove.UseVisualStyleBackColor = true;
            this.buttonEditMove.Click += new System.EventHandler(this.ButtonEditMove_Click);
            // 
            // buttonEditAlign
            // 
            this.buttonEditAlign.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditAlign.Location = new System.Drawing.Point(97, 77);
            this.buttonEditAlign.Name = "buttonEditAlign";
            this.buttonEditAlign.Size = new System.Drawing.Size(87, 20);
            this.buttonEditAlign.TabIndex = 2;
            this.buttonEditAlign.Text = "Align Edge";
            this.buttonEditAlign.UseVisualStyleBackColor = true;
            this.buttonEditAlign.Click += new System.EventHandler(this.ButtonEditAlign_Click);
            // 
            // buttonEditSize
            // 
            this.buttonEditSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditSize.Location = new System.Drawing.Point(6, 77);
            this.buttonEditSize.Name = "buttonEditSize";
            this.buttonEditSize.Size = new System.Drawing.Size(85, 20);
            this.buttonEditSize.TabIndex = 3;
            this.buttonEditSize.Text = "Size";
            this.buttonEditSize.UseVisualStyleBackColor = true;
            this.buttonEditSize.Click += new System.EventHandler(this.ButtonEditSize_Click);
            // 
            // buttonEditMatch
            // 
            this.buttonEditMatch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditMatch.Location = new System.Drawing.Point(97, 48);
            this.buttonEditMatch.Name = "buttonEditMatch";
            this.buttonEditMatch.Size = new System.Drawing.Size(87, 20);
            this.buttonEditMatch.TabIndex = 8;
            this.buttonEditMatch.Text = "Match";
            this.buttonEditMatch.UseVisualStyleBackColor = true;
            this.buttonEditMatch.Click += new System.EventHandler(this.ButtonEditMatch_Click);
            // 
            // groupBoxWorkPlane
            // 
            this.groupBoxWorkPlane.Controls.Add(this.buttonViewWcs);
            this.groupBoxWorkPlane.Controls.Add(this.comboBoxGridBlock);
            this.groupBoxWorkPlane.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxWorkPlane.Location = new System.Drawing.Point(11, 11);
            this.groupBoxWorkPlane.Name = "groupBoxWorkPlane";
            this.groupBoxWorkPlane.Size = new System.Drawing.Size(192, 50);
            this.groupBoxWorkPlane.TabIndex = 9;
            this.groupBoxWorkPlane.TabStop = false;
            this.groupBoxWorkPlane.Text = "Work Plane";
            // 
            // buttonViewWcs
            // 
            this.buttonViewWcs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonViewWcs.Location = new System.Drawing.Point(106, 19);
            this.buttonViewWcs.Name = "buttonViewWcs";
            this.buttonViewWcs.Size = new System.Drawing.Size(78, 23);
            this.buttonViewWcs.TabIndex = 1;
            this.buttonViewWcs.Text = "View WCS";
            this.buttonViewWcs.UseVisualStyleBackColor = true;
            this.buttonViewWcs.Click += new System.EventHandler(this.ButtonViewWcs_Click);
            // 
            // comboBoxGridBlock
            // 
            this.comboBoxGridBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGridBlock.FormattingEnabled = true;
            this.comboBoxGridBlock.Location = new System.Drawing.Point(6, 19);
            this.comboBoxGridBlock.Name = "comboBoxGridBlock";
            this.comboBoxGridBlock.Size = new System.Drawing.Size(94, 21);
            this.comboBoxGridBlock.TabIndex = 0;
            this.comboBoxGridBlock.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGridBlock_SelectedIndexChanged);
            this.comboBoxGridBlock.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxGrid_KeyDown);
            // 
            // buttonExit
            // 
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(17, 347);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(178, 20);
            this.buttonExit.TabIndex = 12;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // buttonEndEditConstruction
            // 
            this.buttonEndEditConstruction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEndEditConstruction.Location = new System.Drawing.Point(99, 19);
            this.buttonEndEditConstruction.Name = "buttonEndEditConstruction";
            this.buttonEndEditConstruction.Size = new System.Drawing.Size(85, 20);
            this.buttonEndEditConstruction.TabIndex = 27;
            this.buttonEndEditConstruction.Text = "End Edit";
            this.buttonEndEditConstruction.UseVisualStyleBackColor = true;
            this.buttonEndEditConstruction.Click += new System.EventHandler(this.ButtonEndEditConstruction_Click);
            // 
            // buttonEditConstruction
            // 
            this.buttonEditConstruction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditConstruction.Location = new System.Drawing.Point(6, 19);
            this.buttonEditConstruction.Name = "buttonEditConstruction";
            this.buttonEditConstruction.Size = new System.Drawing.Size(86, 20);
            this.buttonEditConstruction.TabIndex = 26;
            this.buttonEditConstruction.Text = "Start Edit";
            this.buttonEditConstruction.UseVisualStyleBackColor = true;
            this.buttonEditConstruction.Click += new System.EventHandler(this.ButtonEditConstruction_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReset.Location = new System.Drawing.Point(17, 321);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(178, 20);
            this.buttonReset.TabIndex = 28;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonEndEditConstruction);
            this.groupBox1.Controls.Add(this.buttonEditConstruction);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(11, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 47);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Component";
            // 
            // EditBlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 374);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.groupBoxWorkPlane);
            this.Controls.Add(this.groupBoxEditBlock);
            this.Controls.Add(this.buttonExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EditBlockForm";
            this.Text = "Block Component";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EditBlockForm_Load);
            this.groupBoxEditBlock.ResumeLayout(false);
            this.groupBoxWorkPlane.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxEditBlock;
        private System.Windows.Forms.Button buttonEditDynamic;
        private System.Windows.Forms.Button buttonEditMove;
        private System.Windows.Forms.Button buttonEditAlign;
        private System.Windows.Forms.Button buttonEditSize;
        private System.Windows.Forms.Button buttonEditMatch;
        private System.Windows.Forms.GroupBox groupBoxWorkPlane;
        private System.Windows.Forms.Button buttonViewWcs;
        private System.Windows.Forms.ComboBox comboBoxGridBlock;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonAlignComponent;
        private System.Windows.Forms.Button buttonAlignEdgeDistance;
        private System.Windows.Forms.Button buttonEndEditConstruction;
        private System.Windows.Forms.Button buttonEditConstruction;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonApply;

        ///////////$$$$$$$$$$$$$$$$$$$$$$$$$$$
        ///

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

                if (!_workPart.__HasDynamicBlock())
                    return;

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

                if (mLength == 0 || mWidth == 0 || mHeight == 0)
                    return;


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
            catch (Exception ex)
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
                ex.__PrintException();
            }
        }

        private void AlignEdgeDistance(bool isBlockComponent)
        {
            if (_editBody != null)
            {
                var editComponent = _editBody.OwningComponent;

                if (editComponent != null)
                {
                    var checkPartName = (Part)editComponent.Prototype;

                    if (!checkPartName.FullPath.Contains("mirror"))
                    {
                        _updateComponent = editComponent;

                        var assmUnits = _displayPart.PartUnits;
                        var compBase = (BasePart)editComponent.Prototype;
                        var compUnits = compBase.PartUnits;

                        if (compUnits == assmUnits)
                        {
                            if (_isNewSelection)
                            {
                                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

                                __work_component_ = editComponent;
                                UpdateSessionParts();

                                foreach (Feature featBlk in _workPart.Features)
                                    if (featBlk.FeatureType == "BLOCK")
                                        if (featBlk.Name == "DYNAMIC BLOCK")
                                        {
                                            isBlockComponent = true;
                                            CreateEditData(editComponent);
                                            _isNewSelection = false;
                                        }
                            }
                            else
                            {
                                isBlockComponent = true;
                            }

                            if (isBlockComponent)
                            {
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

                                    var doNotMovePts = new List<NXObject>();
                                    var movePtsHalf = new List<NXObject>();
                                    var movePtsFull = new List<NXObject>();

                                    MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                                    var posXObjs = new List<Line>();
                                    var negXObjs = new List<Line>();
                                    var posYObjs = new List<Line>();
                                    var negYObjs = new List<Line>();
                                    var posZObjs = new List<Line>();
                                    var negZObjs = new List<Line>();

                                    foreach (var eLine in _edgeRepLines)
                                    {
                                        if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                        if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                                           eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                        if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                        if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                                           eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                        if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                                           eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                                        if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                                           eLine.Name == "YCEILING1" ||
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
                                        bool isDistance;

                                        isDistance = NXInputBox.ParseInputNumber("Enter offset value",
                                            "Enter offset value", .004, NumberStyles.AllowDecimalPoint,
                                            CultureInfo.InvariantCulture.NumberFormat, out var inputDist);

                                        if (isDistance)
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

                                            if (pointPrototype.Name == "POSX")
                                            {
                                                distance = AlignEdgeDistancePosX(movePtsHalf, movePtsFull, posXObjs, allxAxisLines, inputDist, mappedBase, mappedPoint);
                                            }

                                            if (pointPrototype.Name == "NEGX")
                                            {
                                                distance = AlignEdgeDistanceNegX(movePtsHalf, movePtsFull, negXObjs, allxAxisLines, inputDist, mappedBase, mappedPoint);
                                            }

                                            if (pointPrototype.Name == "POSY")
                                            {
                                                distance = AlignEdgeDistancePosY(movePtsHalf, movePtsFull, posYObjs, allyAxisLines, inputDist, mappedBase, mappedPoint);
                                            }

                                            if (pointPrototype.Name == "NEGY")
                                            {
                                                distance = AlignEdgeDistanceNegY(movePtsHalf, movePtsFull, negYObjs, allyAxisLines, inputDist, mappedBase, mappedPoint);
                                            }

                                            if (pointPrototype.Name == "POSZ")
                                            {
                                                distance = AlignEdgeDistancePosZ(movePtsHalf, movePtsFull, posZObjs, allzAxisLines, inputDist, mappedBase, mappedPoint);
                                            }

                                            if (pointPrototype.Name == "NEGZ")
                                            {
                                                distance = AlignEdgeDistanceNegZ(movePtsHalf, movePtsFull, negZObjs, allzAxisLines, inputDist, mappedBase, mappedPoint);
                                            }
                                        }
                                        else
                                        {
                                            Show();
                                            TheUISession.NXMessageBox.Show("Caught exception",
                                                NXMessageBox.DialogType.Error, "Invalid input");
                                        }
                                    }

                                    UpdateDynamicBlock(editComponent);
                                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                                    __work_component_ = editComponent;
                                    UpdateSessionParts();
                                    CreateEditData(editComponent);
                                    pHandle = SelectHandlePoint();
                                }

                                Show();
                            }
                            else
                            {
                                ResetNonBlockError();
                                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                                    "Not a block component");
                            }
                        }
                    }
                    else
                    {
                        Show();
                        TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                            "Mirrored Component");
                    }
                }
                else
                {
                    Show();
                    TheUISession.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information,
                        "This function is not allowed in this context");
                }
            }
        }




        public void EditConstruction()
        {
            buttonExit.Enabled = false;

            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

            UpdateSessionParts();
            UpdateOriginalParts();

            var editComponent = SelectOneComponent("Select Component to edit construction");

            if (editComponent is null)
                return;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
            {
                MessageBox.Show("Component units do not match the display part units");
                return;
            }

            ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

            var addRefSetPart = (Part)editComponent.Prototype;

            __display_part_ = addRefSetPart;
            UpdateSessionParts();

            Session.UndoMarkId markId1;
            markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");

            var allRefSets = _displayPart.GetAllReferenceSets();

            foreach (var namedRefSet in allRefSets)
                if (namedRefSet.Name == "EDIT")
                    _workPart.DeleteReferenceSet(namedRefSet);

            int nErrs1;
            nErrs1 = session_.UpdateManager.DoUpdate(markId1);

            session_.DeleteUndoMark(markId1, "Delete Reference Set");

            // create edit reference set

            Session.UndoMarkId markIdEditRefSet;
            markIdEditRefSet =
                session_.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Reference Set");

            var editRefSet = _workPart.CreateReferenceSet();
            var removeComps = editRefSet.AskAllDirectMembers();
            editRefSet.RemoveObjectsFromReferenceSet(removeComps);
            editRefSet.SetAddComponentsAutomatically(false, false);
            editRefSet.SetName("EDIT");

            // get all construction objects to add to reference set

            var constructionObjects = new List<NXObject>();

            for (var i = 1; i < 11; i++)
            {
                var layerObjects = _displayPart.Layers.GetAllObjectsOnLayer(i);

                foreach (var addObj in layerObjects) constructionObjects.Add(addObj);
            }

            editRefSet.AddObjectsToReferenceSet(constructionObjects.ToArray());

            int nErrs2;
            nErrs2 = session_.UpdateManager.DoUpdate(markIdEditRefSet);

            session_.DeleteUndoMark(markIdEditRefSet, "Create New Reference Set");

            __display_part_ = _originalDisplayPart;
            UpdateSessionParts();

            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            __work_component_ = editComponent;
            UpdateSessionParts();

            SetWcsToWorkPart(editComponent);

            __work_component_.__Translucency(75);


            Component[] setRefComp = { editComponent };
            _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("EDIT", setRefComp);

            _displayPart.Layers.WorkLayer = 3;

            UpdateSessionParts();

            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = true;
        }


        private bool EditSizeWork(bool isBlockComponent, Component editComponent)
        {
            var checkPartName = (Part)editComponent.Prototype;

            if (!checkPartName.FullPath.Contains("mirror"))
            {
                _updateComponent = editComponent;

                var assmUnits = _displayPart.PartUnits;
                var compBase = (BasePart)editComponent.Prototype;
                var compUnits = compBase.PartUnits;

                if (compUnits == assmUnits)
                {
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

                    if (isBlockComponent)
                    {
                        UpdateDynamicBlock(editComponent);
                        CreateEditData(editComponent);
                        DisableForm();
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

                                if (eLine.Name == "YBASE1")
                                    blockWidth = eLine.GetLength();

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

                            MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                            var posXObjs = new List<Line>();
                            var negXObjs = new List<Line>();
                            var posYObjs = new List<Line>();
                            var negYObjs = new List<Line>();
                            var posZObjs = new List<Line>();
                            var negZObjs = new List<Line>();

                            foreach (var eLine in _edgeRepLines)
                            {
                                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                                   eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                                   eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                                   eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

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

                                if (_displayPart.PartUnits == BasePart.Units.Millimeters)
                                    editSize *= 25.4;

                                if (editSize > 0)
                                    switch (pointPrototype.Name)
                                    {
                                        case "POSX":
                                            distance = EditSizePosX(blockLength, movePtsHalf, movePtsFull, posXObjs, allxAxisLines, editSize);
                                            break;
                                        case "NEGX":
                                            distance = EditSizeNegX(blockLength, movePtsHalf, movePtsFull, negXObjs, allxAxisLines, editSize);
                                            break;
                                        case "POSY":
                                            distance = EditSizePosY(blockWidth, movePtsHalf, movePtsFull, posYObjs, allyAxisLines, editSize);
                                            break;
                                        case "NEGY":
                                            distance = EditSizeNegY(blockWidth, movePtsHalf, movePtsFull, negYObjs, allyAxisLines, editSize);
                                            break;
                                        case "POSZ":
                                            distance = EditSizePosZ(blockHeight, movePtsHalf, movePtsFull, posZObjs, allzAxisLines, editSize);
                                            break;
                                        case "NEGZ":
                                            distance = EditSizeNegZ(blockHeight, movePtsHalf, movePtsFull, negZObjs, allzAxisLines, editSize);
                                            break;
                                        default:
                                            throw new InvalidOperationException(pointPrototype.Name);
                                    }
                            }

                            UpdateDynamicBlock(editComponent);
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
                }
            }
            else
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
            }

            return isBlockComponent;
        }

        private void EdgeAlign(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            var editComponent = _editBody.OwningComponent;

            if (editComponent is null)
            {
                Show();

                TheUISession.NXMessageBox.Show(
                    "Error",
                    NXMessageBox.DialogType.Information,
                    "This function is not allowed in this context");

                return;
            }

            var checkPartName = (Part)editComponent.Prototype;


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
            {
                isBlockComponent = true;
            }

            if (isBlockComponent)
            {
                UpdateDynamicBlock(editComponent);
                CreateEditData(editComponent);

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

                    var doNotMovePts = new List<NXObject>();
                    var movePtsHalf = new List<NXObject>();
                    var movePtsFull = new List<NXObject>();

                    MotionCallbackDynamic1(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull, pointPrototype.Name.Contains("POS"));

                    var posXObjs = new List<Line>();
                    var negXObjs = new List<Line>();
                    var posYObjs = new List<Line>();
                    var negYObjs = new List<Line>();
                    var posZObjs = new List<Line>();
                    var negZObjs = new List<Line>();

                    foreach (var eLine in _edgeRepLines)
                    {
                        if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                        if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                           eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                        if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                           eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                        if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                           eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                        if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                           eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                        if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                           eLine.Name == "YCEILING1" ||
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
                                movePtsFull.AddRange(posXObjs);
                                distance = EditAlignPosX(movePtsHalf, movePtsFull, allxAxisLines, mappedBase, mappedPoint, 0, "X");
                                break;
                            case "NEGX":
                                movePtsFull.AddRange(negXObjs);
                                distance = EditAlignNegX(movePtsHalf, movePtsFull, allxAxisLines, mappedBase, mappedPoint, 0, "X");
                                break;
                            case "POSY":
                                movePtsFull.AddRange(posYObjs);
                                distance = EditAlignPosY(movePtsHalf, movePtsFull, allyAxisLines, mappedBase, mappedPoint, 1, "Y");
                                break;
                            case "NEGY":
                                movePtsFull.AddRange(negYObjs);
                                distance = EditAlignNegY(movePtsHalf, movePtsFull, allyAxisLines, mappedBase, mappedPoint, 1, "Y");
                                break;
                            case "POSZ":
                                movePtsFull.AddRange(posZObjs);
                                distance = EditAlignPosZ(movePtsHalf, movePtsFull, allzAxisLines, mappedBase, mappedPoint, 2, "Z");
                                break;
                            case "NEGZ":
                                movePtsFull.AddRange(negZObjs);
                                distance = EditAlignNegZ(movePtsHalf, movePtsFull, allzAxisLines, mappedBase, mappedPoint, 2, "Z");
                                break;
                        }
                    }

                    UpdateDynamicBlock(editComponent);
                    ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                    __work_component_ = editComponent;
                    UpdateSessionParts();
                    CreateEditData(editComponent);
                    pHandle = SelectHandlePoint();
                }

                Show();
            }
            else
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
            }
        }


        private bool EditDynamicWorkPart(bool isBlockComponent, Component editComponent)
        {
            var checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
                throw new InvalidOperationException("Mirror COmponent");

            _updateComponent = editComponent;
            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return isBlockComponent;

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

            EditDynamic(isBlockComponent);

            return isBlockComponent;
        }

        private bool EditDynamicDisplayPart(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            isBlockComponent = _workPart.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return isBlockComponent;
            }

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
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                var message = "Select New Position";
                var screenPos = new double[3];
                var viewTag = NXOpen.Tag.Null;
                var motionCbData = IntPtr.Zero;
                var clientData = IntPtr.Zero;
                _displayPart.WCS.Visibility = false;
                var mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out var response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            EnableForm();
            return isBlockComponent;
        }

        private void EditDynamic(bool isBlockComponent)
        {
            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            DisableForm();
            var pHandle = new List<Point>();
            pHandle = SelectHandlePoint();
            _isDynamic = true;

            while (pHandle.Count == 1)
            {
                _distanceMoved = 0;
                HideDynamicHandles();
                _udoPointHandle = pHandle[0];
                var message = "Select New Position";
                var screenPos = new double[3];
                var viewTag = NXOpen.Tag.Null;
                var motionCbData = IntPtr.Zero;
                var clientData = IntPtr.Zero;
                _displayPart.WCS.Visibility = false;
                var mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out var response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            EnableForm();
        }

        private bool EditMoveWork(bool isBlockComponent, Component editComponent)
        {
            var checkPartName = (Part)editComponent.Prototype;

            if (checkPartName.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            _updateComponent = editComponent;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
                return isBlockComponent;

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
                return isBlockComponent;
            }

            DisableForm();
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
                var mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData,
                    screenPos, out viewTag, out var response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    ShowTemporarySizeText();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            EnableForm();

            return isBlockComponent;
        }

        private bool EditMoveDisplay(bool isBlockComponent, Component editComponent)
        {
            if (_displayPart.FullPath.Contains("mirror"))
            {
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Mirrored Component");
                return isBlockComponent;
            }

            isBlockComponent = _workPart.__HasDynamicBlock();

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
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return isBlockComponent;
            }

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
                var mView = (ModelingView)_displayPart.Views.WorkView;
                _displayPart.Views.WorkView.Orient(mView.Matrix);
                _displayPart.WCS.SetOriginAndMatrix(mView.Origin, mView.Matrix);
                ufsession_.Ui.SpecifyScreenPosition(message, MotionCallback, motionCbData, screenPos,
                    out viewTag, out var response);

                if (response == UF_UI_PICK_RESPONSE)
                {
                    UpdateDynamicHandles();
                    ShowDynamicHandles();
                    ShowTemporarySizeText();
                    pHandle = SelectHandlePoint();
                }

                ufsession_.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);
            }

            EnableForm();

            return isBlockComponent;
        }

        private void EditMatch(bool isBlockComponent)
        {
            if (_editBody is null)
                return;

            var editComponent = _editBody.OwningComponent;

            if (editComponent == null)
            {
                EnableForm();
                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                    NXMessageBox.DialogType.Information, "This function is not allowed in this context");
                return;
            }

            var checkPartName = (Part)editComponent.Prototype;
            isBlockComponent = checkPartName.__HasDynamicBlock();

            if (!isBlockComponent)
            {
                ResetNonBlockError();
                TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                    "Not a block component");
                return;
            }

            DisableForm();

            if (checkPartName.FullPath.Contains("mirror"))
            {
                EnableForm();
                TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                    NXMessageBox.DialogType.Error, "Mirrored Component");
                return;
            }

            _updateComponent = editComponent;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits == assmUnits)
            {
                SelectWithFilter.NonValidCandidates = _nonValidNames;
                SelectWithFilter.GetSelectedWithFilter("Select Component - Match To");
                var editBodyTo = SelectWithFilter.SelectedCompBody;

                if (editBodyTo is null)
                {
                    ResetNonBlockError();
                    return;
                }

                var matchComponent = editBodyTo.OwningComponent;
                ufsession_.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                __work_component_ = matchComponent;
                UpdateSessionParts();
                isBlockComponent = _workPart.__HasDynamicBlock();

                if (isBlockComponent)
                    EditMatch(editComponent, matchComponent);
                else
                {
                    ResetNonBlockError();
                    TheUISession.NXMessageBox.Show("Caught exception : Match Block",
                        NXMessageBox.DialogType.Error,
                        "Can not match to the selected component");
                }

                EnableForm();

                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = true;
                buttonReset.Enabled = true;
                buttonExit.Enabled = true;
            }
        }

        private void EditMatch(Component editComponent, Component matchComponent)
        {
            DisableForm();

            SetWcsToWorkPart(matchComponent);

            foreach (Feature featBlk in _workPart.Features)
                if (featBlk.Name == "DYNAMIC BLOCK")
                {
                    // get current block feature
                    var block1 = (Block)featBlk;

                    BlockFeatureBuilder blockFeatureBuilderMatchFrom;
                    blockFeatureBuilderMatchFrom =
                        _workPart.Features.CreateBlockFeatureBuilder(block1);
                    var blkOrigin = blockFeatureBuilderMatchFrom.Origin;
                    var length = blockFeatureBuilderMatchFrom.Length.RightHandSide;
                    var width = blockFeatureBuilderMatchFrom.Width.RightHandSide;
                    var height = blockFeatureBuilderMatchFrom.Height.RightHandSide;
                    blockFeatureBuilderMatchFrom.GetOrientation(out var xAxisMatch,
                        out var yAxisMatch);

                    __work_part_ = _displayPart; ;
                    UpdateSessionParts();
                    var origin = new double[3];
                    var matrix = new double[9];
                    var transform = new double[4, 4];

                    ufsession_.Assem.AskComponentData(matchComponent.Tag,
                        out var partName, out var refSetName, out var instanceName,
                        origin, matrix, transform);

                    var eInstance =
                        ufsession_.Assem.AskInstOfPartOcc(editComponent.Tag);
                    ufsession_.Assem.RepositionInstance(eInstance, origin, matrix);

                    __work_component_ = editComponent;
                    UpdateSessionParts();

                    foreach (Feature featDynamic in _workPart.Features)
                        if (featDynamic.Name == "DYNAMIC BLOCK")
                        {
                            var block2 = (Block)featDynamic;

                            BlockFeatureBuilder blockFeatureBuilderMatchTo;
                            blockFeatureBuilderMatchTo =
                                _workPart.Features.CreateBlockFeatureBuilder(block2);

                            blockFeatureBuilderMatchTo.BooleanOption.Type =
                                BooleanOperation.BooleanType.Create;

                            var targetBodies1 = new Body[1];
                            Body nullBody = null;
                            targetBodies1[0] = nullBody;
                            blockFeatureBuilderMatchTo.BooleanOption.SetTargetBodies(
                                targetBodies1);

                            blockFeatureBuilderMatchTo.Type = BlockFeatureBuilder.Types
                                .OriginAndEdgeLengths;

                            var blkFeatBuilderPoint =
                                _workPart.Points.CreatePoint(blkOrigin);
                            blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                            blockFeatureBuilderMatchTo.OriginPoint =
                                blkFeatBuilderPoint;

                            var originPoint1 = blkOrigin;

                            blockFeatureBuilderMatchTo.SetOriginAndLengths(originPoint1,
                                length, width, height);

                            blockFeatureBuilderMatchTo.SetOrientation(xAxisMatch,
                                yAxisMatch);

                            blockFeatureBuilderMatchTo.SetBooleanOperationAndTarget(
                                Feature.BooleanType.Create, nullBody);

                            Feature feature1;
                            feature1 = blockFeatureBuilderMatchTo.CommitFeature();

                            blockFeatureBuilderMatchFrom.Destroy();
                            blockFeatureBuilderMatchTo.Destroy();

                            _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                            session_.Preferences.EmphasisVisualization
                                .WorkPartEmphasis = true;
                            session_.Preferences.Assemblies
                                .WorkPartDisplayAsEntirePart = false;

                            __work_part_ = _originalWorkPart;
                            UpdateSessionParts();

                            _displayPart.WCS.Visibility = true;
                            _displayPart.Views.Refresh();
                        }
                }

            MoveComponent(editComponent);

            EnableForm();
        }





        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if (_displayPart.PartUnits == BasePart.Units.Inches)
            {
                comboBoxGridBlock.Items.Add("0.002");
                comboBoxGridBlock.Items.Add("0.03125");
                comboBoxGridBlock.Items.Add("0.0625");
                comboBoxGridBlock.Items.Add("0.125");
                comboBoxGridBlock.Items.Add("0.250");
                comboBoxGridBlock.Items.Add("0.500");
                comboBoxGridBlock.Items.Add("1.000");
            }
            else
            {
                comboBoxGridBlock.Items.Add("0.0508");
                comboBoxGridBlock.Items.Add("0.79375");
                comboBoxGridBlock.Items.Add("1.00");
                comboBoxGridBlock.Items.Add("1.5875");
                comboBoxGridBlock.Items.Add("3.175");
                comboBoxGridBlock.Items.Add("5.00");
                comboBoxGridBlock.Items.Add("6.35");
                comboBoxGridBlock.Items.Add("12.7");
                comboBoxGridBlock.Items.Add("25.4");
            }

            foreach (string gridSetting in comboBoxGridBlock.Items)
                if (gridSetting == Settings.Default.EditBlockFormGridIncrement)
                {
                    var gridIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);
                    comboBoxGridBlock.SelectedIndex = gridIndex;
                    break;
                }
        }

        private void SetWorkPlaneOn()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    var gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = true;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;

#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetWorkPlaneOff()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    var gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = false;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;
#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public int Startup()
        {
            if (_registered == 0)
            {
                var editForm = this;
                _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(editForm.WorkPartChanged1);
                _registered = 1;
            }

            return 0;
        }

        public void WorkPartChanged1(BasePart p)
        {
            SetWorkPlaneOff();
            LoadGridSizes();
            SetWorkPlaneOn();
        }

        private void EditBlockForm_Load(object sender, EventArgs e)
        {
            if (Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;

            buttonApply.Enabled = false;

            LoadGridSizes();

            if (string.IsNullOrEmpty(comboBoxGridBlock.Text))
                if (!(Session.GetSession().Parts.Work is null))
                    comboBoxGridBlock.SelectedItem = Session.GetSession().Parts.Work.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");
            _registered = Startup();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
            Close();
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();

            using (this)
                new ComponentBuilder().Show();
        }

        private void ComboBoxGridBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGridBlock.Text == "0.000")
            {
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }

            Settings.Default.EditBlockFormGridIncrement = comboBoxGridBlock.Text;
            Settings.Default.Save();
        }

        private void ButtonEditConstruction_Click(object sender, EventArgs e) => EditConstruction();

        private void ButtonEndEditConstruction_Click(object sender, EventArgs e)
        {
            try
            {
                EndEditConstruction();
            }
            catch (Exception ex)
            {
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                ex.__PrintException();
            }
        }

        private void EndEditConstruction()
        {
            __work_component_.__Translucency(0);
            _displayPart.Layers.WorkLayer = 1;
            Session.UndoMarkId markId1;
            markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");
            Component[] setRefComp = { session_.Parts.WorkComponent };
            _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", setRefComp);
            var allRefSets = _workPart.GetAllReferenceSets();

            foreach (var namedRefSet in allRefSets)
                if (namedRefSet.Name == "EDIT")
                    _workPart.DeleteReferenceSet(namedRefSet);

            int nErrs1;
            nErrs1 = session_.UpdateManager.DoUpdate(markId1);
            session_.DeleteUndoMark(markId1, "Delete Reference Set");
            __display_part_ = _originalDisplayPart;
            __work_part_ = _originalWorkPart;
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = false;
            buttonExit.Enabled = true;
            UpdateSessionParts();
            UpdateOriginalParts();
        }



        private List<Point> SelectHandlePoint()
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);
            Selection.Response sel;
            var pointSelection = new List<Point>();

            sel = TheUISession.SelectionManager.SelectTaggedObject("Select Point", "Select Point",
                Selection.SelectionScope.WorkPart,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedPoint, out var cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
                pointSelection.Add((Point)selectedPoint);

            return pointSelection;
        }

        private Component SelectOneComponent(string prompt)
        {
            var mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;

            sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out var selectedComp, out var cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
            {
                compSelection = (Component)selectedComp;
                return compSelection;
            }

            return null;
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




    }
}