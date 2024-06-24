namespace TSG_Library.UFuncs
{
    partial class AssemblyExportDesignDataForm
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
            this.lblComplete = new System.Windows.Forms.Label();
            this.rdoReview100 = new System.Windows.Forms.RadioButton();
            this.rdoOther = new System.Windows.Forms.RadioButton();
            this.chkPrintDesign = new System.Windows.Forms.CheckBox();
            this.rdoRto = new System.Windows.Forms.RadioButton();
            this.btnDesignAccept = new System.Windows.Forms.Button();
            this.rdoChange = new System.Windows.Forms.RadioButton();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.rdoReview90 = new System.Windows.Forms.RadioButton();
            this.rdoReview50 = new System.Windows.Forms.RadioButton();
            this.lblReview = new System.Windows.Forms.Label();
            this.grpExportData = new System.Windows.Forms.GroupBox();
            this.grpExportType = new System.Windows.Forms.GroupBox();
            this.chkDetailStep = new System.Windows.Forms.CheckBox();
            this.chkPrintData = new System.Windows.Forms.CheckBox();
            this.chkBurnout = new System.Windows.Forms.CheckBox();
            this.chkParasolids = new System.Windows.Forms.CheckBox();
            this.chk4ViewDwg = new System.Windows.Forms.CheckBox();
            this.chkCastings = new System.Windows.Forms.CheckBox();
            this.chk4ViewPDF = new System.Windows.Forms.CheckBox();
            this.chkSee3DData = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectComponents = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDesign = new System.Windows.Forms.TabPage();
            this.tabData = new System.Windows.Forms.TabPage();
            this.grpExportType.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabDesign.SuspendLayout();
            this.tabData.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblComplete
            // 
            this.lblComplete.AutoSize = true;
            this.lblComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComplete.Location = new System.Drawing.Point(109, 12);
            this.lblComplete.Name = "lblComplete";
            this.lblComplete.Size = new System.Drawing.Size(51, 13);
            this.lblComplete.TabIndex = 1;
            this.lblComplete.Text = "Complete";
            // 
            // rdoReview100
            // 
            this.rdoReview100.AutoSize = true;
            this.rdoReview100.Location = new System.Drawing.Point(9, 74);
            this.rdoReview100.Name = "rdoReview100";
            this.rdoReview100.Size = new System.Drawing.Size(51, 17);
            this.rdoReview100.TabIndex = 2;
            this.rdoReview100.TabStop = true;
            this.rdoReview100.Text = "100%";
            this.rdoReview100.UseVisualStyleBackColor = true;
            this.rdoReview100.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // rdoOther
            // 
            this.rdoOther.AutoSize = true;
            this.rdoOther.Location = new System.Drawing.Point(9, 97);
            this.rdoOther.Margin = new System.Windows.Forms.Padding(50, 3, 3, 3);
            this.rdoOther.Name = "rdoOther";
            this.rdoOther.Size = new System.Drawing.Size(51, 17);
            this.rdoOther.TabIndex = 4;
            this.rdoOther.TabStop = true;
            this.rdoOther.Text = "Other";
            this.rdoOther.UseVisualStyleBackColor = true;
            this.rdoOther.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // chkPrintDesign
            // 
            this.chkPrintDesign.AutoSize = true;
            this.chkPrintDesign.Location = new System.Drawing.Point(151, 146);
            this.chkPrintDesign.Name = "chkPrintDesign";
            this.chkPrintDesign.Size = new System.Drawing.Size(87, 17);
            this.chkPrintDesign.TabIndex = 15;
            this.chkPrintDesign.Text = "Print 4-Views";
            this.chkPrintDesign.UseVisualStyleBackColor = true;
            // 
            // rdoRto
            // 
            this.rdoRto.AutoSize = true;
            this.rdoRto.Location = new System.Drawing.Point(112, 28);
            this.rdoRto.Name = "rdoRto";
            this.rdoRto.Size = new System.Drawing.Size(77, 17);
            this.rdoRto.TabIndex = 3;
            this.rdoRto.TabStop = true;
            this.rdoRto.Text = "Full Design";
            this.rdoRto.UseVisualStyleBackColor = true;
            this.rdoRto.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // btnDesignAccept
            // 
            this.btnDesignAccept.Location = new System.Drawing.Point(6, 146);
            this.btnDesignAccept.Name = "btnDesignAccept";
            this.btnDesignAccept.Size = new System.Drawing.Size(139, 23);
            this.btnDesignAccept.TabIndex = 18;
            this.btnDesignAccept.Text = "Execute";
            this.btnDesignAccept.UseVisualStyleBackColor = true;
            this.btnDesignAccept.Click += new System.EventHandler(this.BtnData_Clicked);
            // 
            // rdoChange
            // 
            this.rdoChange.AutoSize = true;
            this.rdoChange.Location = new System.Drawing.Point(112, 51);
            this.rdoChange.Name = "rdoChange";
            this.rdoChange.Size = new System.Drawing.Size(62, 17);
            this.rdoChange.TabIndex = 19;
            this.rdoChange.TabStop = true;
            this.rdoChange.Text = "Change";
            this.rdoChange.UseVisualStyleBackColor = true;
            this.rdoChange.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(6, 120);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.Size = new System.Drawing.Size(232, 20);
            this.txtFolderName.TabIndex = 5;
            // 
            // rdoReview90
            // 
            this.rdoReview90.AutoSize = true;
            this.rdoReview90.Location = new System.Drawing.Point(9, 51);
            this.rdoReview90.Name = "rdoReview90";
            this.rdoReview90.Size = new System.Drawing.Size(45, 17);
            this.rdoReview90.TabIndex = 1;
            this.rdoReview90.TabStop = true;
            this.rdoReview90.Text = "90%";
            this.rdoReview90.UseVisualStyleBackColor = true;
            this.rdoReview90.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // rdoReview50
            // 
            this.rdoReview50.AutoSize = true;
            this.rdoReview50.Location = new System.Drawing.Point(9, 28);
            this.rdoReview50.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.rdoReview50.Name = "rdoReview50";
            this.rdoReview50.Size = new System.Drawing.Size(45, 17);
            this.rdoReview50.TabIndex = 0;
            this.rdoReview50.TabStop = true;
            this.rdoReview50.Text = "50%";
            this.rdoReview50.UseVisualStyleBackColor = true;
            this.rdoReview50.CheckedChanged += new System.EventHandler(this.Rdo_CheckedChanged);
            // 
            // lblReview
            // 
            this.lblReview.AutoSize = true;
            this.lblReview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReview.Location = new System.Drawing.Point(6, 12);
            this.lblReview.Name = "lblReview";
            this.lblReview.Size = new System.Drawing.Size(48, 13);
            this.lblReview.TabIndex = 0;
            this.lblReview.Text = "Reviews";
            // 
            // grpExportData
            // 
            this.grpExportData.AutoSize = true;
            this.grpExportData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpExportData.Location = new System.Drawing.Point(715, 81);
            this.grpExportData.Name = "grpExportData";
            this.grpExportData.Size = new System.Drawing.Size(6, 5);
            this.grpExportData.TabIndex = 13;
            this.grpExportData.TabStop = false;
            this.grpExportData.Text = "Export Data";
            // 
            // grpExportType
            // 
            this.grpExportType.Controls.Add(this.chkDetailStep);
            this.grpExportType.Controls.Add(this.chkPrintData);
            this.grpExportType.Controls.Add(this.chkBurnout);
            this.grpExportType.Controls.Add(this.chkParasolids);
            this.grpExportType.Controls.Add(this.chk4ViewDwg);
            this.grpExportType.Controls.Add(this.chkCastings);
            this.grpExportType.Controls.Add(this.chk4ViewPDF);
            this.grpExportType.Controls.Add(this.chkSee3DData);
            this.grpExportType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpExportType.Location = new System.Drawing.Point(6, 6);
            this.grpExportType.Name = "grpExportType";
            this.grpExportType.Size = new System.Drawing.Size(147, 210);
            this.grpExportType.TabIndex = 10;
            this.grpExportType.TabStop = false;
            this.grpExportType.Text = "Type";
            // 
            // chkDetailStep
            // 
            this.chkDetailStep.AutoSize = true;
            this.chkDetailStep.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkDetailStep.Location = new System.Drawing.Point(6, 184);
            this.chkDetailStep.Name = "chkDetailStep";
            this.chkDetailStep.Size = new System.Drawing.Size(84, 18);
            this.chkDetailStep.TabIndex = 17;
            this.chkDetailStep.Text = "Details .stp";
            this.chkDetailStep.UseVisualStyleBackColor = true;
            // 
            // chkPrintData
            // 
            this.chkPrintData.AutoSize = true;
            this.chkPrintData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkPrintData.Location = new System.Drawing.Point(6, 160);
            this.chkPrintData.Name = "chkPrintData";
            this.chkPrintData.Size = new System.Drawing.Size(88, 18);
            this.chkPrintData.TabIndex = 7;
            this.chkPrintData.Text = "Print 4-View";
            this.chkPrintData.UseVisualStyleBackColor = true;
            // 
            // chkBurnout
            // 
            this.chkBurnout.AutoSize = true;
            this.chkBurnout.Location = new System.Drawing.Point(6, 42);
            this.chkBurnout.Name = "chkBurnout";
            this.chkBurnout.Size = new System.Drawing.Size(94, 17);
            this.chkBurnout.TabIndex = 11;
            this.chkBurnout.Text = "Burnouts .dwg";
            this.chkBurnout.UseVisualStyleBackColor = true;
            // 
            // chkParasolids
            // 
            this.chkParasolids.AutoSize = true;
            this.chkParasolids.Location = new System.Drawing.Point(6, 114);
            this.chkParasolids.Name = "chkParasolids";
            this.chkParasolids.Size = new System.Drawing.Size(94, 17);
            this.chkParasolids.TabIndex = 16;
            this.chkParasolids.Text = "Parasolids .x_t";
            this.chkParasolids.UseVisualStyleBackColor = true;
            // 
            // chk4ViewDwg
            // 
            this.chk4ViewDwg.AutoSize = true;
            this.chk4ViewDwg.Location = new System.Drawing.Point(6, 137);
            this.chk4ViewDwg.Name = "chk4ViewDwg";
            this.chk4ViewDwg.Size = new System.Drawing.Size(128, 17);
            this.chk4ViewDwg.TabIndex = 12;
            this.chk4ViewDwg.Text = "4-View .dwg (Legacy)";
            this.chk4ViewDwg.UseVisualStyleBackColor = true;
            // 
            // chkCastings
            // 
            this.chkCastings.AutoSize = true;
            this.chkCastings.Location = new System.Drawing.Point(6, 67);
            this.chkCastings.Name = "chkCastings";
            this.chkCastings.Size = new System.Drawing.Size(86, 17);
            this.chkCastings.TabIndex = 13;
            this.chkCastings.Text = "Castings .x_t";
            this.chkCastings.UseVisualStyleBackColor = true;
            // 
            // chk4ViewPDF
            // 
            this.chk4ViewPDF.AutoSize = true;
            this.chk4ViewPDF.Location = new System.Drawing.Point(6, 19);
            this.chk4ViewPDF.Name = "chk4ViewPDF";
            this.chk4ViewPDF.Size = new System.Drawing.Size(79, 17);
            this.chk4ViewPDF.TabIndex = 15;
            this.chk4ViewPDF.Text = "4-View .pdf";
            this.chk4ViewPDF.UseVisualStyleBackColor = true;
            // 
            // chkSee3DData
            // 
            this.chkSee3DData.AutoSize = true;
            this.chkSee3DData.Location = new System.Drawing.Point(6, 91);
            this.chkSee3DData.Name = "chkSee3DData";
            this.chkSee3DData.Size = new System.Drawing.Size(108, 17);
            this.chkSee3DData.TabIndex = 14;
            this.chkSee3DData.Text = "See 3D Data .stp";
            this.chkSee3DData.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectAll);
            this.groupBox2.Controls.Add(this.btnSelectComponents);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(6, 222);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(149, 92);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection";
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectAll.Location = new System.Drawing.Point(6, 52);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(137, 27);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.BtnData_Clicked);
            // 
            // btnSelectComponents
            // 
            this.btnSelectComponents.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSelectComponents.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectComponents.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectComponents.Location = new System.Drawing.Point(6, 19);
            this.btnSelectComponents.Name = "btnSelectComponents";
            this.btnSelectComponents.Size = new System.Drawing.Size(137, 27);
            this.btnSelectComponents.TabIndex = 1;
            this.btnSelectComponents.Text = "Select Components";
            this.btnSelectComponents.UseVisualStyleBackColor = false;
            this.btnSelectComponents.Click += new System.EventHandler(this.BtnData_Clicked);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDesign);
            this.tabControl.Controls.Add(this.tabData);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(254, 204);
            this.tabControl.TabIndex = 21;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControl_Selected);
            // 
            // tabDesign
            // 
            this.tabDesign.Controls.Add(this.lblComplete);
            this.tabDesign.Controls.Add(this.lblReview);
            this.tabDesign.Controls.Add(this.rdoReview100);
            this.tabDesign.Controls.Add(this.rdoReview50);
            this.tabDesign.Controls.Add(this.rdoOther);
            this.tabDesign.Controls.Add(this.rdoReview90);
            this.tabDesign.Controls.Add(this.chkPrintDesign);
            this.tabDesign.Controls.Add(this.txtFolderName);
            this.tabDesign.Controls.Add(this.rdoRto);
            this.tabDesign.Controls.Add(this.rdoChange);
            this.tabDesign.Controls.Add(this.btnDesignAccept);
            this.tabDesign.Location = new System.Drawing.Point(4, 22);
            this.tabDesign.Name = "tabDesign";
            this.tabDesign.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabDesign.Size = new System.Drawing.Size(246, 178);
            this.tabDesign.TabIndex = 0;
            this.tabDesign.Text = "Design";
            this.tabDesign.UseVisualStyleBackColor = true;
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.grpExportType);
            this.tabData.Controls.Add(this.groupBox2);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabData.Size = new System.Drawing.Size(246, 178);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Data";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // AssemblyExportDesignDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 204);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.grpExportData);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssemblyExportDesignDataForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AssemblyExportDesignDataForm_FormClosed);
            this.Load += new System.EventHandler(this.AssemblyExportDesignDataForm_Load);
            this.grpExportType.ResumeLayout(false);
            this.grpExportType.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabDesign.ResumeLayout(false);
            this.tabDesign.PerformLayout();
            this.tabData.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.RadioButton rdoOther;
        private System.Windows.Forms.RadioButton rdoRto;
        private System.Windows.Forms.RadioButton rdoReview100;
        private System.Windows.Forms.RadioButton rdoReview90;
        private System.Windows.Forms.RadioButton rdoReview50;
        private System.Windows.Forms.CheckBox chkPrintData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectComponents;
        private System.Windows.Forms.GroupBox grpExportType;
        private System.Windows.Forms.CheckBox chkBurnout;
        private System.Windows.Forms.CheckBox chkParasolids;
        private System.Windows.Forms.CheckBox chk4ViewDwg;
        private System.Windows.Forms.CheckBox chkCastings;
        private System.Windows.Forms.CheckBox chk4ViewPDF;
        private System.Windows.Forms.CheckBox chkSee3DData;
        private System.Windows.Forms.GroupBox grpExportData;
        private System.Windows.Forms.Button btnDesignAccept;
        private System.Windows.Forms.CheckBox chkPrintDesign;
        private System.Windows.Forms.CheckBox chkDetailStep;
        private System.Windows.Forms.RadioButton rdoChange;
        private System.Windows.Forms.Label lblComplete;
        private System.Windows.Forms.Label lblReview;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabDesign;
        private System.Windows.Forms.TabPage tabData;
    }
}