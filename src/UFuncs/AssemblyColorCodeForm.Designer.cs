namespace TSG_Library.UFuncs
{
    partial class AssemblyColorCodeForm
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
            this.components = new System.ComponentModel.Container();
            this.btnInheritColor = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnReset = new System.Windows.Forms.Button();
            this.rdoNoFilter = new System.Windows.Forms.RadioButton();
            this.rdoSolid = new System.Windows.Forms.RadioButton();
            this.rdoFeature = new System.Windows.Forms.RadioButton();
            this.rdoFace = new System.Windows.Forms.RadioButton();
            this.GroupBoxFilters = new System.Windows.Forms.GroupBox();
            this.rdoCurves = new System.Windows.Forms.RadioButton();
            this.groupBoxColors = new System.Windows.Forms.GroupBox();
            this.GroupBoxFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInheritColor
            // 
            this.btnInheritColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnInheritColor.Location = new System.Drawing.Point(12, 108);
            this.btnInheritColor.Name = "btnInheritColor";
            this.btnInheritColor.Size = new System.Drawing.Size(122, 23);
            this.btnInheritColor.TabIndex = 16;
            this.btnInheritColor.Text = "Inherit Color";
            this.btnInheritColor.UseVisualStyleBackColor = true;
            this.btnInheritColor.Click += new System.EventHandler(this.ButtonInheritColor_Click);
            // 
            // btnReset
            // 
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReset.Location = new System.Drawing.Point(12, 137);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(122, 23);
            this.btnReset.TabIndex = 22;
            this.btnReset.Text = "Reset";
            this.toolTip.SetToolTip(this.btnReset, "Reset Button");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // rdoNoFilter
            // 
            this.rdoNoFilter.AutoSize = true;
            this.rdoNoFilter.Location = new System.Drawing.Point(6, 19);
            this.rdoNoFilter.Name = "rdoNoFilter";
            this.rdoNoFilter.Size = new System.Drawing.Size(64, 17);
            this.rdoNoFilter.TabIndex = 3;
            this.rdoNoFilter.TabStop = true;
            this.rdoNoFilter.Text = "No Filter";
            this.rdoNoFilter.UseVisualStyleBackColor = true;
            this.rdoNoFilter.CheckedChanged += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoSolid
            // 
            this.rdoSolid.AutoSize = true;
            this.rdoSolid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSolid.Location = new System.Drawing.Point(6, 65);
            this.rdoSolid.Name = "rdoSolid";
            this.rdoSolid.Size = new System.Drawing.Size(48, 17);
            this.rdoSolid.TabIndex = 2;
            this.rdoSolid.TabStop = true;
            this.rdoSolid.Text = "Solid";
            this.rdoSolid.UseVisualStyleBackColor = true;
            this.rdoSolid.CheckedChanged += new System.EventHandler(this.RdoButton_Click);
            this.rdoSolid.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoFeature
            // 
            this.rdoFeature.AutoSize = true;
            this.rdoFeature.Location = new System.Drawing.Point(55, 42);
            this.rdoFeature.Name = "rdoFeature";
            this.rdoFeature.Size = new System.Drawing.Size(61, 17);
            this.rdoFeature.TabIndex = 1;
            this.rdoFeature.TabStop = true;
            this.rdoFeature.Text = "Feature";
            this.rdoFeature.UseVisualStyleBackColor = true;
            this.rdoFeature.CheckedChanged += new System.EventHandler(this.RdoButton_Click);
            this.rdoFeature.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoFace
            // 
            this.rdoFace.AutoSize = true;
            this.rdoFace.Location = new System.Drawing.Point(6, 42);
            this.rdoFace.Name = "rdoFace";
            this.rdoFace.Size = new System.Drawing.Size(49, 17);
            this.rdoFace.TabIndex = 0;
            this.rdoFace.TabStop = true;
            this.rdoFace.Text = "Face";
            this.rdoFace.UseVisualStyleBackColor = true;
            this.rdoFace.CheckedChanged += new System.EventHandler(this.RdoButton_Click);
            this.rdoFace.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // GroupBoxFilters
            // 
            this.GroupBoxFilters.Controls.Add(this.rdoCurves);
            this.GroupBoxFilters.Controls.Add(this.rdoSolid);
            this.GroupBoxFilters.Controls.Add(this.rdoFeature);
            this.GroupBoxFilters.Controls.Add(this.rdoFace);
            this.GroupBoxFilters.Controls.Add(this.rdoNoFilter);
            this.GroupBoxFilters.Location = new System.Drawing.Point(12, 12);
            this.GroupBoxFilters.Name = "GroupBoxFilters";
            this.GroupBoxFilters.Size = new System.Drawing.Size(122, 90);
            this.GroupBoxFilters.TabIndex = 26;
            this.GroupBoxFilters.TabStop = false;
            this.GroupBoxFilters.Text = "Filters";
            // 
            // rdoCurves
            // 
            this.rdoCurves.AutoSize = true;
            this.rdoCurves.Location = new System.Drawing.Point(55, 65);
            this.rdoCurves.Name = "rdoCurves";
            this.rdoCurves.Size = new System.Drawing.Size(58, 17);
            this.rdoCurves.TabIndex = 4;
            this.rdoCurves.TabStop = true;
            this.rdoCurves.Text = "Curves";
            this.rdoCurves.UseVisualStyleBackColor = true;
            this.rdoCurves.CheckedChanged += new System.EventHandler(this.RdoButton_Click);
            // 
            // groupBoxColors
            // 
            this.groupBoxColors.AutoSize = true;
            this.groupBoxColors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxColors.Location = new System.Drawing.Point(12, 166);
            this.groupBoxColors.Name = "groupBoxColors";
            this.groupBoxColors.Size = new System.Drawing.Size(6, 5);
            this.groupBoxColors.TabIndex = 27;
            this.groupBoxColors.TabStop = false;
            this.groupBoxColors.Text = "Colors";
            // 
            // AssemblyColorCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(147, 467);
            this.Controls.Add(this.btnInheritColor);
            this.Controls.Add(this.groupBoxColors);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.GroupBoxFilters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemblyColorCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.GroupBoxFilters.ResumeLayout(false);
            this.GroupBoxFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnInheritColor;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton rdoFace;
        private System.Windows.Forms.RadioButton rdoSolid;
        private System.Windows.Forms.RadioButton rdoFeature;
        private System.Windows.Forms.RadioButton rdoNoFilter;
        private System.Windows.Forms.GroupBox GroupBoxFilters;
        private System.Windows.Forms.GroupBox groupBoxColors;
        //Revision � 1.15 � 2017 � 11 � 09 
        private System.Windows.Forms.RadioButton rdoCurves;
    }
}