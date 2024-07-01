namespace TSG_Library.UFuncs
{
    partial class AssemblyAutoDetailForm
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
            this.chkHoleChart = new System.Windows.Forms.CheckBox();
            this.chkDetailSheet = new System.Windows.Forms.CheckBox();
            this.chkUpdateViews = new System.Windows.Forms.CheckBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.grpBoxOptions = new System.Windows.Forms.GroupBox();
            this.chkDrillChart = new System.Windows.Forms.CheckBox();
            this.chkColorDetailSheet = new System.Windows.Forms.CheckBox();
            this.chkDualDimensions = new System.Windows.Forms.CheckBox();
            this.chkDelete4Views = new System.Windows.Forms.CheckBox();
            this.grpBoxSelection = new System.Windows.Forms.GroupBox();
            this.btnSelectBorderFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.grpBoxOptions.SuspendLayout();
            this.grpBoxSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkHoleChart
            // 
            this.chkHoleChart.AutoSize = true;
            this.chkHoleChart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkHoleChart.Location = new System.Drawing.Point(6, 43);
            this.chkHoleChart.Name = "chkHoleChart";
            this.chkHoleChart.Size = new System.Drawing.Size(96, 18);
            this.chkHoleChart.TabIndex = 0;
            this.chkHoleChart.Text = "Hole Charting";
            this.chkHoleChart.UseVisualStyleBackColor = true;
            this.chkHoleChart.CheckedChanged += new System.EventHandler(this.ChkChart_CheckedChanged);
            this.chkHoleChart.Click += new System.EventHandler(this.CheckBox_Clicked);
            // 
            // chkDetailSheet
            // 
            this.chkDetailSheet.AutoSize = true;
            this.chkDetailSheet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDetailSheet.Location = new System.Drawing.Point(6, 19);
            this.chkDetailSheet.Name = "chkDetailSheet";
            this.chkDetailSheet.Size = new System.Drawing.Size(124, 18);
            this.chkDetailSheet.TabIndex = 1;
            this.chkDetailSheet.Text = "Create Detail Sheet";
            this.chkDetailSheet.UseVisualStyleBackColor = true;
            this.chkDetailSheet.Click += new System.EventHandler(this.CheckBox_Clicked);
            // 
            // chkUpdateViews
            // 
            this.chkUpdateViews.AutoSize = true;
            this.chkUpdateViews.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkUpdateViews.Location = new System.Drawing.Point(6, 91);
            this.chkUpdateViews.Name = "chkUpdateViews";
            this.chkUpdateViews.Size = new System.Drawing.Size(112, 18);
            this.chkUpdateViews.TabIndex = 2;
            this.chkUpdateViews.Text = "Update All Views";
            this.chkUpdateViews.UseVisualStyleBackColor = true;
            this.chkUpdateViews.Click += new System.EventHandler(this.CheckBox_Clicked);
            // 
            // btnSelect
            // 
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelect.Location = new System.Drawing.Point(6, 19);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(118, 23);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectAll.Location = new System.Drawing.Point(6, 48);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(118, 23);
            this.btnSelectAll.TabIndex = 5;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.ButtonSelectAll_Click);
            // 
            // grpBoxOptions
            // 
            this.grpBoxOptions.Controls.Add(this.chkDrillChart);
            this.grpBoxOptions.Controls.Add(this.chkColorDetailSheet);
            this.grpBoxOptions.Controls.Add(this.chkDualDimensions);
            this.grpBoxOptions.Controls.Add(this.chkDelete4Views);
            this.grpBoxOptions.Controls.Add(this.chkHoleChart);
            this.grpBoxOptions.Controls.Add(this.chkDetailSheet);
            this.grpBoxOptions.Controls.Add(this.chkUpdateViews);
            this.grpBoxOptions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpBoxOptions.Location = new System.Drawing.Point(12, 41);
            this.grpBoxOptions.Name = "grpBoxOptions";
            this.grpBoxOptions.Size = new System.Drawing.Size(135, 200);
            this.grpBoxOptions.TabIndex = 6;
            this.grpBoxOptions.TabStop = false;
            this.grpBoxOptions.Text = "Options 1919";
            // 
            // chkDrillChart
            // 
            this.chkDrillChart.AutoSize = true;
            this.chkDrillChart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDrillChart.Location = new System.Drawing.Point(6, 67);
            this.chkDrillChart.Name = "chkDrillChart";
            this.chkDrillChart.Size = new System.Drawing.Size(91, 18);
            this.chkDrillChart.TabIndex = 6;
            this.chkDrillChart.Text = "Drill Charting";
            this.chkDrillChart.UseVisualStyleBackColor = true;
            this.chkDrillChart.CheckedChanged += new System.EventHandler(this.ChkChart_CheckedChanged);
            this.chkDrillChart.Click += new System.EventHandler(this.CheckBox_Clicked);
            // 
            // chkColorDetailSheet
            // 
            this.chkColorDetailSheet.AutoSize = true;
            this.chkColorDetailSheet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkColorDetailSheet.Location = new System.Drawing.Point(6, 163);
            this.chkColorDetailSheet.Name = "chkColorDetailSheet";
            this.chkColorDetailSheet.Size = new System.Drawing.Size(100, 18);
            this.chkColorDetailSheet.TabIndex = 5;
            this.chkColorDetailSheet.Text = "Shaded Views";
            this.chkColorDetailSheet.UseVisualStyleBackColor = true;
            // 
            // chkDualDimensions
            // 
            this.chkDualDimensions.AutoSize = true;
            this.chkDualDimensions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDualDimensions.Location = new System.Drawing.Point(6, 139);
            this.chkDualDimensions.Name = "chkDualDimensions";
            this.chkDualDimensions.Size = new System.Drawing.Size(111, 18);
            this.chkDualDimensions.TabIndex = 4;
            this.chkDualDimensions.Text = "Dual Dimensions";
            this.chkDualDimensions.UseVisualStyleBackColor = true;
            this.chkDualDimensions.Click += new System.EventHandler(this.CheckBox_Clicked);
            // 
            // chkDelete4Views
            // 
            this.chkDelete4Views.AutoSize = true;
            this.chkDelete4Views.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDelete4Views.Location = new System.Drawing.Point(6, 115);
            this.chkDelete4Views.Name = "chkDelete4Views";
            this.chkDelete4Views.Size = new System.Drawing.Size(117, 18);
            this.chkDelete4Views.TabIndex = 3;
            this.chkDelete4Views.Text = "Delete All 4-Views";
            this.chkDelete4Views.UseVisualStyleBackColor = true;
            this.chkDelete4Views.CheckedChanged += new System.EventHandler(this.CheckBoxDelete4Views_CheckedChanged);
            // 
            // grpBoxSelection
            // 
            this.grpBoxSelection.Controls.Add(this.btnSelect);
            this.grpBoxSelection.Controls.Add(this.btnSelectAll);
            this.grpBoxSelection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpBoxSelection.Location = new System.Drawing.Point(12, 247);
            this.grpBoxSelection.Name = "grpBoxSelection";
            this.grpBoxSelection.Size = new System.Drawing.Size(135, 85);
            this.grpBoxSelection.TabIndex = 7;
            this.grpBoxSelection.TabStop = false;
            this.grpBoxSelection.Text = "Selection";
            // 
            // btnSelectBorderFile
            // 
            this.btnSelectBorderFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectBorderFile.Location = new System.Drawing.Point(12, 12);
            this.btnSelectBorderFile.Name = "btnSelectBorderFile";
            this.btnSelectBorderFile.Size = new System.Drawing.Size(135, 23);
            this.btnSelectBorderFile.TabIndex = 8;
            this.btnSelectBorderFile.Text = "Select Border File";
            this.btnSelectBorderFile.UseVisualStyleBackColor = true;
            this.btnSelectBorderFile.Click += new System.EventHandler(this.ButtonSelectBorderFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Filter = ".prt|*.prt";
            // 
            // AssemblyAutoDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 338);
            this.Controls.Add(this.btnSelectBorderFile);
            this.Controls.Add(this.grpBoxSelection);
            this.Controls.Add(this.grpBoxOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemblyAutoDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpBoxOptions.ResumeLayout(false);
            this.grpBoxOptions.PerformLayout();
            this.grpBoxSelection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkHoleChart;
        private System.Windows.Forms.CheckBox chkDetailSheet;
        private System.Windows.Forms.CheckBox chkUpdateViews;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.GroupBox grpBoxOptions;
        private System.Windows.Forms.GroupBox grpBoxSelection;
        private System.Windows.Forms.Button btnSelectBorderFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox chkDelete4Views;
        private System.Windows.Forms.CheckBox chkDualDimensions;
        private System.Windows.Forms.CheckBox chkColorDetailSheet;
        private System.Windows.Forms.CheckBox chkDrillChart;
    }
}