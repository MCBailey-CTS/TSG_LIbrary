namespace TSG_Library.UFuncs
{
    partial class AssemblyColorCodeBentlerForm
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
            this.btnInheritColor.Location = new System.Drawing.Point(16, 133);
            this.btnInheritColor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInheritColor.Name = "btnInheritColor";
            this.btnInheritColor.Size = new System.Drawing.Size(163, 28);
            this.btnInheritColor.TabIndex = 16;
            this.btnInheritColor.Text = "Inherit Color";
            this.btnInheritColor.UseVisualStyleBackColor = true;
            this.btnInheritColor.Click += new System.EventHandler(this.ButtonInheritColor_Click);
            // 
            // btnReset
            // 
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReset.Location = new System.Drawing.Point(16, 169);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(163, 28);
            this.btnReset.TabIndex = 22;
            this.btnReset.Text = "Reset";
            this.toolTip.SetToolTip(this.btnReset, "Reset Button");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // rdoNoFilter
            // 
            this.rdoNoFilter.AutoSize = true;
            this.rdoNoFilter.Location = new System.Drawing.Point(8, 23);
            this.rdoNoFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoNoFilter.Name = "rdoNoFilter";
            this.rdoNoFilter.Size = new System.Drawing.Size(78, 20);
            this.rdoNoFilter.TabIndex = 3;
            this.rdoNoFilter.TabStop = true;
            this.rdoNoFilter.Text = "No Filter";
            this.rdoNoFilter.UseVisualStyleBackColor = true;
            this.rdoNoFilter.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoSolid
            // 
            this.rdoSolid.AutoSize = true;
            this.rdoSolid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoSolid.Location = new System.Drawing.Point(8, 80);
            this.rdoSolid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoSolid.Name = "rdoSolid";
            this.rdoSolid.Size = new System.Drawing.Size(59, 20);
            this.rdoSolid.TabIndex = 2;
            this.rdoSolid.TabStop = true;
            this.rdoSolid.Text = "Solid";
            this.rdoSolid.UseVisualStyleBackColor = true;
            this.rdoSolid.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoFeature
            // 
            this.rdoFeature.AutoSize = true;
            this.rdoFeature.Location = new System.Drawing.Point(73, 52);
            this.rdoFeature.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoFeature.Name = "rdoFeature";
            this.rdoFeature.Size = new System.Drawing.Size(74, 20);
            this.rdoFeature.TabIndex = 1;
            this.rdoFeature.TabStop = true;
            this.rdoFeature.Text = "Feature";
            this.rdoFeature.UseVisualStyleBackColor = true;
            this.rdoFeature.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // rdoFace
            // 
            this.rdoFace.AutoSize = true;
            this.rdoFace.Location = new System.Drawing.Point(8, 52);
            this.rdoFace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoFace.Name = "rdoFace";
            this.rdoFace.Size = new System.Drawing.Size(59, 20);
            this.rdoFace.TabIndex = 0;
            this.rdoFace.TabStop = true;
            this.rdoFace.Text = "Face";
            this.rdoFace.UseVisualStyleBackColor = true;
            this.rdoFace.Click += new System.EventHandler(this.RdoButton_Click);
            // 
            // GroupBoxFilters
            // 
            this.GroupBoxFilters.Controls.Add(this.rdoCurves);
            this.GroupBoxFilters.Controls.Add(this.rdoSolid);
            this.GroupBoxFilters.Controls.Add(this.rdoFeature);
            this.GroupBoxFilters.Controls.Add(this.rdoFace);
            this.GroupBoxFilters.Controls.Add(this.rdoNoFilter);
            this.GroupBoxFilters.Location = new System.Drawing.Point(16, 15);
            this.GroupBoxFilters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBoxFilters.Name = "GroupBoxFilters";
            this.GroupBoxFilters.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupBoxFilters.Size = new System.Drawing.Size(163, 111);
            this.GroupBoxFilters.TabIndex = 26;
            this.GroupBoxFilters.TabStop = false;
            this.GroupBoxFilters.Text = "Filters";
            // 
            // rdoCurves
            // 
            this.rdoCurves.AutoSize = true;
            this.rdoCurves.Location = new System.Drawing.Point(73, 80);
            this.rdoCurves.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoCurves.Name = "rdoCurves";
            this.rdoCurves.Size = new System.Drawing.Size(70, 20);
            this.rdoCurves.TabIndex = 4;
            this.rdoCurves.TabStop = true;
            this.rdoCurves.Text = "Curves";
            this.rdoCurves.UseVisualStyleBackColor = true;
            // 
            // groupBoxColors
            // 
            this.groupBoxColors.AutoSize = true;
            this.groupBoxColors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxColors.Location = new System.Drawing.Point(16, 204);
            this.groupBoxColors.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxColors.Name = "groupBoxColors";
            this.groupBoxColors.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxColors.Size = new System.Drawing.Size(8, 7);
            this.groupBoxColors.TabIndex = 27;
            this.groupBoxColors.TabStop = false;
            this.groupBoxColors.Text = "Colors";
            // 
            // AssemblyColorCodeBentlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(196, 575);
            this.Controls.Add(this.btnInheritColor);
            this.Controls.Add(this.groupBoxColors);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.GroupBoxFilters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemblyColorCodeBentlerForm";
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