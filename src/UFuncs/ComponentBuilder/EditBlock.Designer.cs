using NXOpen.UF;
using NXOpen;
using System;
using TSG_Library.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using NXOpenUI;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using Part = NXOpen.Part;

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

        private void EdgeAlign(bool isBlockComponent)
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

                                    if (pointPrototype.Name.Contains("POS"))
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
                                    else
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

                                        if (pointPrototype.Name == "POSX")
                                        {
                                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

                                            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

                                            foreach (var xAxisLine in allxAxisLines)
                                            {
                                                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                                                var addX = new Point3d(mappedEndPoint.X + distance,
                                                    mappedEndPoint.Y, mappedEndPoint.Z);
                                                var mappedAddX = MapWcsToAbsolute(addX);
                                                xAxisLine.SetEndPoint(mappedAddX);
                                            }

                                            MoveObjects(movePtsFull.ToArray(), distance, "X");
                                            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
                                        }

                                        if (pointPrototype.Name == "NEGX")
                                        {
                                            distance = Math.Abs(mappedPoint[0] - mappedBase[0]);

                                            if (mappedBase[0] < mappedPoint[0]) distance *= -1;

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
                                        }

                                        if (pointPrototype.Name == "POSY")
                                        {
                                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

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
                                        }

                                        if (pointPrototype.Name == "NEGY")
                                        {
                                            distance = Math.Abs(mappedPoint[1] - mappedBase[1]);

                                            if (mappedBase[1] < mappedPoint[1]) distance *= -1;

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
                                        }

                                        if (pointPrototype.Name == "POSZ")
                                        {
                                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

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
                                        }

                                        if (pointPrototype.Name == "NEGZ")
                                        {
                                            distance = Math.Abs(mappedPoint[2] - mappedBase[2]);

                                            if (mappedBase[2] < mappedPoint[2]) distance *= -1;

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
                    else
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
                                distance = editSize - blockLength;

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
                            }

                            if (pointPrototype.Name == "NEGX")
                            {
                                distance = blockLength - editSize;

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
                            }

                            if (pointPrototype.Name == "POSY")
                            {
                                distance = editSize - blockWidth;

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
                            }

                            if (pointPrototype.Name == "NEGY")
                            {
                                distance = blockWidth - editSize;

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
                            }

                            if (pointPrototype.Name == "POSZ")
                            {
                                distance = editSize - blockHeight;

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
                            }

                            if (pointPrototype.Name == "NEGZ")
                            {
                                distance = blockHeight - editSize;

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

                            if (pointPrototype.Name.Contains("POS"))
                                EditSizePointsPos(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                            else
                                EditSizePointsNeg(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);

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

        private double EditSizePosZ(double blockHeight, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double editSize)
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

        private double EditSizeNegY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double editSize)
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

        private double EditSizePosY(double blockWidth, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double editSize)
        {
            var distance = editSize - blockWidth;
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

        private double EditSizeNegX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double editSize)
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

        private double EditSizePosX(double blockLength, List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double editSize)
        {
            var distance = editSize - blockLength;
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                var addX = new Point3d(mappedEndPoint.X + distance,
                    mappedEndPoint.Y, mappedEndPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetEndPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
            return distance;
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


        public void EditConstruction()
        {
            buttonExit.Enabled = false;

            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

            UpdateSessionParts();
            UpdateOriginalParts();

            var editComponent = SelectOneComponent("Select Component to edit construction");

            if (editComponent != null)
            {
                var assmUnits = _displayPart.PartUnits;
                var compBase = (BasePart)editComponent.Prototype;
                var compUnits = compBase.PartUnits;

                if (compUnits == assmUnits)
                {
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
                else
                {
                    MessageBox.Show("Component units do not match the display part units");
                }
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

                                    if (pointPrototype.Name.Contains("POS"))
                                    {
                                        AlignEdgeDistancePos(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                                    }
                                    else
                                    {
                                        AlignEdgeDistanceNeg(pointPrototype, doNotMovePts, movePtsHalf, movePtsFull);
                                    }

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

        private double AlignEdgeDistanceNegZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
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

        private double AlignEdgeDistancePosZ(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
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

        private double AlignEdgeDistanceNegY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
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

        private double AlignEdgeDistancePosY(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
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

        private double AlignEdgeDistanceNegX(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double inputDist, double[] mappedBase, double[] mappedPoint)
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
                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                var addX = new Point3d(mappedEndPoint.X + distance,
                    mappedEndPoint.Y, mappedEndPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetEndPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
            return distance;
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
                        CreateUdoPosX(myUdo, myLinks, pointOnFace);

                    ufsession_.Vec3.IsEqual(dir, wcsVectorY, 0.00, out var isEqualY);

                    if (isEqualY == 1)
                        CreateUdoPosY(myUdo, myLinks, pointOnFace);

                    ufsession_.Vec3.IsEqual(dir, wcsVectorZ, 0.00, out var isEqualZ);

                    if (isEqualZ == 1)
                        CreateUdoPosZ(myUdo, myLinks, pointOnFace);

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegX, 0.00, out var isEqualNegX);

                    if (isEqualNegX == 1)
                        CreateUdoNegX(myUdo, myLinks, pointOnFace);

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegY, 0.00, out var isEqualNegY);

                    if (isEqualNegY == 1)
                        CreateUdoNegY(myUdo, myLinks, pointOnFace);

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegZ, 0.00, out var isEqualNegZ);

                    if (isEqualNegZ == 1)
                        CreateUdoNegZ(myUdo, myLinks, pointOnFace);
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
            Point3d mappedEndPointX2 = CreateBlockLinesEndPointX2(lineLength, lineColor, mappedEndPointX1, mappedEndPointY1);
            Point3d mappedEndPointX1Ceiling;
            CreateBlockLinesEndPointX1Ceiling(lineLength, lineColor, mappedEndPointZ1, out Point3d mappedStartPoint3, out mappedEndPointX1Ceiling);
            Point3d mappedEndPointY1Ceiling = CreateBlockLineEndPointY1Ceiling(lineWidth, lineColor, mappedEndPointZ1, mappedStartPoint3);
            var mappedStartPoint4 = MapAbsoluteToWcs(mappedEndPointY1Ceiling);
            Point3d mappedEndPointX2Ceiling = CreateBlockLinesEndPointX2Ceiling(lineLength, lineColor, mappedEndPointY1Ceiling, mappedStartPoint4);
            CreateBlockLinesYCeiling2(lineColor, mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            CreateBlockLinesZBase2(lineColor, mappedEndPointX1, mappedEndPointX1Ceiling);
            CreateBlockLinesZBase3(lineColor, mappedEndPointY1, mappedEndPointY1Ceiling);
            CreateBlockLinesZBase4(lineColor, mappedEndPointX2, mappedEndPointX2Ceiling);
        }

        private Point3d CreateBlockLineEndPointY1Ceiling(double lineWidth, int lineColor, Point3d mappedEndPointZ1, Point3d mappedStartPoint3)
        {
            var endPointY1Ceiling = mappedStartPoint3.__AddY(lineWidth);
            var mappedEndPointY1Ceiling = MapWcsToAbsolute(endPointY1Ceiling);
            CreateBlockLinesYCeiling1(lineColor, mappedEndPointZ1, mappedEndPointY1Ceiling);
            return mappedEndPointY1Ceiling;
        }

        private static void CreateBlockLinesYCeiling1(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointY1Ceiling)
        {
            var yCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointY1Ceiling);
            yCeiling1.SetName("YCEILING1");
            yCeiling1.Color = lineColor;
            yCeiling1.RedisplayObject();
            _edgeRepLines.Add(yCeiling1);
        }

        private static void CreateBlockLinesZBase4(int lineColor, Point3d mappedEndPointX2, Point3d mappedEndPointX2Ceiling)
        {
            var zBase4 = _workPart.Curves.CreateLine(mappedEndPointX2, mappedEndPointX2Ceiling);
            zBase4.SetName("ZBASE4");
            zBase4.Color = lineColor;
            zBase4.RedisplayObject();
            _edgeRepLines.Add(zBase4);
        }

        private static void CreateBlockLinesZBase3(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointY1Ceiling)
        {
            var zBase3 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointY1Ceiling);
            zBase3.SetName("ZBASE3");
            zBase3.Color = lineColor;
            zBase3.RedisplayObject();
            _edgeRepLines.Add(zBase3);
        }

        private static void CreateBlockLinesZBase2(int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointX1Ceiling)
        {
            var zBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX1Ceiling);
            zBase2.SetName(@"ZBASE2");
            zBase2.Color = lineColor;
            zBase2.RedisplayObject();
            _edgeRepLines.Add(zBase2);
        }

        private static void CreateBlockLinesYCeiling2(int lineColor, Point3d mappedEndPointX1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            var yCeiling2 = _workPart.Curves.CreateLine(mappedEndPointX1Ceiling, mappedEndPointX2Ceiling);
            yCeiling2.SetName("YCEILING2");
            yCeiling2.Color = lineColor;
            yCeiling2.RedisplayObject();
            _edgeRepLines.Add(yCeiling2);
        }

        private Point3d CreateBlockLinesEndPointX2Ceiling(double lineLength, int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedStartPoint4)
        {
            var endPointX2Ceiling = mappedStartPoint4.__AddX(lineLength);
            var mappedEndPointX2Ceiling = MapWcsToAbsolute(endPointX2Ceiling);
            CreateBlockLineYCeiling2(lineColor, mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            return mappedEndPointX2Ceiling;
        }

        private static void CreateBlockLineYCeiling2(int lineColor, Point3d mappedEndPointY1Ceiling, Point3d mappedEndPointX2Ceiling)
        {
            var xCeiling2 = _workPart.Curves.CreateLine(mappedEndPointY1Ceiling, mappedEndPointX2Ceiling);
            xCeiling2.SetName("XCEILING2");
            xCeiling2.Color = lineColor;
            xCeiling2.RedisplayObject();
            _edgeRepLines.Add(xCeiling2);
        }

        private Point3d CreateBlockLinesEndPointX2(double lineLength, int lineColor, Point3d mappedEndPointX1, Point3d mappedEndPointY1)
        {
            Point3d mappedEndPointX2 = CreateBlockLinesEndPOontX2(lineLength, lineColor, mappedEndPointY1);

            var yBase2 = _workPart.Curves.CreateLine(mappedEndPointX1, mappedEndPointX2);
            yBase2.SetName("YBASE2");
            yBase2.Color = lineColor;
            yBase2.RedisplayObject();
            _edgeRepLines.Add(yBase2);
            return mappedEndPointX2;
        }

        private void CreateBlockLinesEndPointX1Ceiling(double lineLength, int lineColor, Point3d mappedEndPointZ1, out Point3d mappedStartPoint3, out Point3d mappedEndPointX1Ceiling)
        {
            mappedStartPoint3 = MapAbsoluteToWcs(mappedEndPointZ1);
            var endPointX1Ceiling =
                new Point3d(mappedStartPoint3.X + lineLength, mappedStartPoint3.Y, mappedStartPoint3.Z);
            mappedEndPointX1Ceiling = MapWcsToAbsolute(endPointX1Ceiling);
            CreateBlovkLinesXCeiling2(lineColor, mappedEndPointZ1, mappedEndPointX1Ceiling);
        }

        private static void CreateBlovkLinesXCeiling2(int lineColor, Point3d mappedEndPointZ1, Point3d mappedEndPointX1Ceiling)
        {
            var xCeiling1 = _workPart.Curves.CreateLine(mappedEndPointZ1, mappedEndPointX1Ceiling);
            xCeiling1.SetName("XCEILING1");
            xCeiling1.Color = lineColor;
            xCeiling1.RedisplayObject();
            _edgeRepLines.Add(xCeiling1);
        }

        private Point3d CreateBlockLinesEndPOontX2(double lineLength, int lineColor, Point3d mappedEndPointY1)
        {
            var mappedStartPoint2 = MapAbsoluteToWcs(mappedEndPointY1);

            var endPointX2 = mappedStartPoint2.__AddX(lineLength);
            var mappedEndPointX2 = MapWcsToAbsolute(endPointX2);
            CreateBlockLinesXBase2(lineColor, mappedEndPointY1, mappedEndPointX2);
            return mappedEndPointX2;
        }

        private static void CreateBlockLinesXBase2(int lineColor, Point3d mappedEndPointY1, Point3d mappedEndPointX2)
        {
            var xbase2 = _workPart.Curves.CreateLine(mappedEndPointY1, mappedEndPointX2);
            xbase2.SetName("XBASE2");
            xbase2.Color = lineColor;
            xbase2.RedisplayObject();
            _edgeRepLines.Add(xbase2);
        }

        private Point3d CreateBlockLinesEndPointZ1(Point3d wcsOrigin, double lineHeight, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointZ1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y, mappedStartPoint1.Z + lineHeight);
            var mappedEndPointZ1 = MapWcsToAbsolute(endPointZ1);
            CreateBlockLinesZBase1(wcsOrigin, lineColor, mappedEndPointZ1);
            return mappedEndPointZ1;
        }

        private static void CreateBlockLinesZBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointZ1)
        {
            var zBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointZ1);
            zBase1.SetName("ZBASE1");
            zBase1.Color = lineColor;
            zBase1.RedisplayObject();
            _edgeRepLines.Add(zBase1);
        }

        private Point3d CreateBlockLinesEndPointY1(Point3d wcsOrigin, double lineWidth, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointY1 = new Point3d(mappedStartPoint1.X, mappedStartPoint1.Y + lineWidth, mappedStartPoint1.Z);
            var mappedEndPointY1 = MapWcsToAbsolute(endPointY1);
            CreateBlockLinesYBase1(wcsOrigin, lineColor, mappedEndPointY1);
            return mappedEndPointY1;
        }

        private static void CreateBlockLinesYBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointY1)
        {
            var yBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointY1);
            yBase1.SetName("YBASE1");
            yBase1.Color = lineColor;
            yBase1.RedisplayObject();
            _edgeRepLines.Add(yBase1);
        }

        private Point3d CreateBlockLinesEndPointX1(Point3d wcsOrigin, double lineLength, Point3d mappedStartPoint1, int lineColor)
        {
            var endPointX1 = new Point3d(mappedStartPoint1.X + lineLength, mappedStartPoint1.Y, mappedStartPoint1.Z);
            var mappedEndPointX1 = MapWcsToAbsolute(endPointX1);
            CreateBlockLinesXBase1(wcsOrigin, lineColor, mappedEndPointX1);
            return mappedEndPointX1;
        }

        private static void CreateBlockLinesXBase1(Point3d wcsOrigin, int lineColor, Point3d mappedEndPointX1)
        {
            var xBase1 = _workPart.Curves.CreateLine(wcsOrigin, mappedEndPointX1);
            xBase1.SetName("XBASE1");
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

        private static Point CreatePointNegZ(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("NEGZ");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private static Point CreatePointNegY(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("NEGY");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private static Point CreatePointNegX(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("NEGX");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }


        private static Point PosZ(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("POSZ");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private static Point CreatePointPosY(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("POSY");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private static Point CreatePointPosX(double[] pointOnFace)
        {
            var pointLocation = new Point3d(pointOnFace[0], pointOnFace[1], pointOnFace[2]);
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName("POSX");
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }


        private static void CreateUdoNegX(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = CreatePointNegX(pointOnFace);
            CreateUdoNegX(myUdo, myLinks, pointOnFace, point1);
        }
        private static void CreateUdoNegZ(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = CreatePointNegZ(pointOnFace);
            CreateUdoNegZ(myUdo, myLinks, pointOnFace, point1);
        }

        private static void CreateUdoNegY(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = CreatePointNegY(pointOnFace);
            CreateUdoNegY(myUdo, myLinks, pointOnFace, point1);
        }

        private static void CreateUdoPosY(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = CreatePointPosY(pointOnFace);
            CreateUdoPosY(myUdo, myLinks, pointOnFace, point1);
        }

        private static void CreateUdoPosX(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = CreatePointPosX(pointOnFace);
            CreateUdoPosX(myUdo, myLinks, pointOnFace, point1);
        }







        private static void CreateUdoNegZ(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("NEGZ");
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }

        private static void CreateUdoNegY(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("NEGY");
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }



        private static void CreateUdoNegX(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("NEGX");
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }

        private static void CreateUdoPosZ(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace)
        {
            Point point1 = PosZ(pointOnFace);
            CreateUdoPosZ(myUdo, myLinks, pointOnFace, point1);
        }

        private static void CreateUdoPosZ(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("POSZ");
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }



        private static void CreateUdoPosY(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("POSY");
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }

        private static void CreateUdoPosX(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1)
        {
            myUdo.SetName("POSX");
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
            BasePart myBasePart = _workPart;

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
                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

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
                {
                    DisplayModification editObjectDisplay1;
                    editObjectDisplay1 = session_.DisplayManager.NewDisplayModification();
                    editObjectDisplay1.ApplyToAllFaces = true;
                    editObjectDisplay1.NewTranslucency = 0;
                    DisplayableObject[] compObject1 = { updateComp };
                    editObjectDisplay1.Apply(compObject1);
                    editObjectDisplay1.Dispose();
                }
                else
                {
                    foreach (Body dispBody in _workPart.Bodies)
                        if (dispBody.Layer == 1)
                        {
                            DisplayModification editObjectDisplay;
                            editObjectDisplay = session_.DisplayManager.NewDisplayModification();
                            editObjectDisplay.ApplyToAllFaces = true;
                            editObjectDisplay.NewTranslucency = 0;
                            DisplayableObject[] bodyObject = { dispBody };
                            editObjectDisplay.Apply(bodyObject);
                            editObjectDisplay.Dispose();
                        }
                }

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

                foreach (Feature featDynamic in _workPart.Features)
                    if (featDynamic.FeatureType == "BLOCK")
                        if (featDynamic.Name == "DYNAMIC BLOCK")
                        {
                            var block2 = (Block)featDynamic;

                            BlockFeatureBuilder blockFeatureBuilder2;
                            blockFeatureBuilder2 = _workPart.Features.CreateBlockFeatureBuilder(block2);

                            blockFeatureBuilder2.BooleanOption.Type = BooleanOperation.BooleanType.Create;

                            var targetBodies1 = new Body[1];
                            Body nullBody = null;
                            targetBodies1[0] = nullBody;
                            blockFeatureBuilder2.BooleanOption.SetTargetBodies(targetBodies1);

                            blockFeatureBuilder2.Type = BlockFeatureBuilder.Types.OriginAndEdgeLengths;

                            var blkFeatBuilderPoint = _workPart.Points.CreatePoint(blkOrigin);
                            blkFeatBuilderPoint.SetCoordinates(blkOrigin);

                            blockFeatureBuilder2.OriginPoint = blkFeatBuilderPoint;

                            var originPoint1 = blkOrigin;

                            blockFeatureBuilder2.SetOriginAndLengths(originPoint1, length, width, height);

                            blockFeatureBuilder2.SetBooleanOperationAndTarget(Feature.BooleanType.Create, nullBody);

                            Feature feature1;
                            feature1 = blockFeatureBuilder2.CommitFeature();

                            blockFeatureBuilder2.Destroy();

                            _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                            __work_part_ = _displayPart;
                            UpdateSessionParts();

                            // delete udo's / points
                            DeleteHandles();

                            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
                            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;

                            __work_part_ = _originalWorkPart;
                            UpdateSessionParts();

                            _displayPart.WCS.Visibility = true;
                            _displayPart.Views.Refresh();
                        }
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

                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

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
            if (mappedPoint[0] != mappedCursor[0])
            {
                var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

                if (xDistance >= _gridSpace)
                {
                    if (mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

                    _distanceMoved += xDistance;

                    if (pointPrototype.Name == "POSX")
                    {
                        MotionCallbackPosXDyanmic(movePtsHalf, movePtsFull, posXObjs, allxAxisLines, xDistance);
                    }
                    else
                    {
                        MotionCallbackNegXDyanmic(movePtsHalf, movePtsFull, negXObjs, allxAxisLines, xDistance);
                    }
                }
            }
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
                var mappedStartPoint = MapAbsoluteToWcs(zAxisLine.StartPoint);
                var addZ = new Point3d(mappedStartPoint.X, mappedStartPoint.Y,
                    mappedStartPoint.Z + zDistance);
                var mappedAddZ = MapWcsToAbsolute(addZ);
                zAxisLine.SetStartPoint(mappedAddZ);
            }

            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }

        private void MotionCallbackPosZDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posZObjs, List<Line> allzAxisLines, double zDistance)
        {
            foreach (var addLine in posZObjs) movePtsFull.Add(addLine);

            foreach (var zAxisLine in allzAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(zAxisLine.EndPoint);
                var addZ = new Point3d(mappedEndPoint.X, mappedEndPoint.Y,
                    mappedEndPoint.Z + zDistance);
                var mappedAddZ = MapWcsToAbsolute(addZ);
                zAxisLine.SetEndPoint(mappedAddZ);
            }

            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }

        private void MotionCallbackNegYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in negYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedStartPoint = MapAbsoluteToWcs(yAxisLine.StartPoint);
                var addY = new Point3d(mappedStartPoint.X, mappedStartPoint.Y + yDistance,
                    mappedStartPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetStartPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }

        private void MotionCallbackPosYDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posYObjs, List<Line> allyAxisLines, double yDistance)
        {
            foreach (var addLine in posYObjs) movePtsFull.Add(addLine);

            foreach (var yAxisLine in allyAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(yAxisLine.EndPoint);
                var addY = new Point3d(mappedEndPoint.X, mappedEndPoint.Y + yDistance,
                    mappedEndPoint.Z);
                var mappedAddY = MapWcsToAbsolute(addY);
                yAxisLine.SetEndPoint(mappedAddY);
            }

            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }

        private void MotionCallbackNegXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> negXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var addLine in negXObjs) movePtsFull.Add(addLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                var mappedStartPoint = MapAbsoluteToWcs(xAxisLine.StartPoint);
                var addX = new Point3d(mappedStartPoint.X + xDistance, mappedStartPoint.Y,
                    mappedStartPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetStartPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
        }

        private void MotionCallbackPosXDyanmic(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, List<Line> posXObjs, List<Line> allxAxisLines, double xDistance)
        {
            foreach (var posXLine in posXObjs) movePtsFull.Add(posXLine);

            foreach (var xAxisLine in allxAxisLines)
            {
                var mappedEndPoint = MapAbsoluteToWcs(xAxisLine.EndPoint);
                var addX = new Point3d(mappedEndPoint.X + xDistance, mappedEndPoint.Y,
                    mappedEndPoint.Z);
                var mappedAddX = MapWcsToAbsolute(addX);
                xAxisLine.SetEndPoint(mappedAddX);
            }

            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
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
            if (mappedPoint[0] != mappedCursor[0])
            {
                var xDistance = Math.Sqrt(Math.Pow(mappedPoint[0] - mappedCursor[0], 2));

                if (xDistance >= _gridSpace)
                {
                    if (mappedCursor[0] < mappedPoint[0]) xDistance *= -1;

                    _distanceMoved += xDistance;

                    MoveObjects(moveAll.ToArray(), xDistance, "X");
                }
            }
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
                    SelectWithFilter.NonValidCandidates = _nonValidNames;
                    SelectWithFilter.GetSelectedWithFilter("Select Component to Move");
                    _editBody = SelectWithFilter.SelectedCompBody;
                    _isNewSelection = true;
                }
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

                    __display_part_ =(Part)compBase; 
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






    }
}