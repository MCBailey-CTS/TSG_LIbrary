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
using NXOpen.Preferences;
using NXOpen.UserDefinedObjects;
using NXOpenUI;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using Part = NXOpen.Part;
using System.Reflection;

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
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBoxEditBlock.SuspendLayout();
            this.groupBoxWorkPlane.SuspendLayout();
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
            this.groupBoxEditBlock.Location = new System.Drawing.Point(11, 67);
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
            this.buttonExit.Location = new System.Drawing.Point(17, 294);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(178, 20);
            this.buttonExit.TabIndex = 12;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReset.Location = new System.Drawing.Point(17, 268);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(178, 20);
            this.buttonReset.TabIndex = 28;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // EditBlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 324);
            this.ControlBox = false;
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
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonApply;





    }
}