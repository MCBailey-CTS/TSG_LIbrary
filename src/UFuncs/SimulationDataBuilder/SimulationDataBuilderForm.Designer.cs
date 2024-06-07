namespace TSG_Library.UFuncs
{
    partial class SimulationDataBuilderForm
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.cmbData = new System.Windows.Forms.ComboBox();
            this.cmbOperation = new System.Windows.Forms.ComboBox();
            this.cmbVersion = new System.Windows.Forms.ComboBox();
            this.cmbSurface = new System.Windows.Forms.ComboBox();
            this.cmbToolSide = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtJobNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbStudyProposal = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbEngineeringLevel = new System.Windows.Forms.ComboBox();
            this.txtCustomText = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdoStp = new System.Windows.Forms.RadioButton();
            this.rdoIges = new System.Windows.Forms.RadioButton();
            this.rdoPrt = new System.Windows.Forms.RadioButton();
            this.btnSimDataDeletion = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.topMostCheckBox = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelect.Location = new System.Drawing.Point(8, 377);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(256, 28);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // cmbData
            // 
            this.cmbData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbData.FormattingEnabled = true;
            this.cmbData.Location = new System.Drawing.Point(8, 42);
            this.cmbData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbData.Name = "cmbData";
            this.cmbData.Size = new System.Drawing.Size(84, 24);
            this.cmbData.TabIndex = 3;
            this.cmbData.SelectedIndexChanged += new System.EventHandler(this.CmbData_SelectedIndexChanged);
            // 
            // cmbOperation
            // 
            this.cmbOperation.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbOperation.FormattingEnabled = true;
            this.cmbOperation.Location = new System.Drawing.Point(96, 60);
            this.cmbOperation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbOperation.Name = "cmbOperation";
            this.cmbOperation.Size = new System.Drawing.Size(79, 24);
            this.cmbOperation.TabIndex = 4;
            this.cmbOperation.SelectedIndexChanged += new System.EventHandler(this.CmbOperation_SelectedIndexChanged);
            // 
            // cmbVersion
            // 
            this.cmbVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbVersion.FormattingEnabled = true;
            this.cmbVersion.Location = new System.Drawing.Point(184, 60);
            this.cmbVersion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbVersion.Name = "cmbVersion";
            this.cmbVersion.Size = new System.Drawing.Size(79, 24);
            this.cmbVersion.TabIndex = 5;
            // 
            // cmbSurface
            // 
            this.cmbSurface.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbSurface.FormattingEnabled = true;
            this.cmbSurface.Location = new System.Drawing.Point(8, 38);
            this.cmbSurface.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbSurface.Name = "cmbSurface";
            this.cmbSurface.Size = new System.Drawing.Size(112, 24);
            this.cmbSurface.TabIndex = 6;
            // 
            // cmbToolSide
            // 
            this.cmbToolSide.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbToolSide.FormattingEnabled = true;
            this.cmbToolSide.Location = new System.Drawing.Point(129, 38);
            this.cmbToolSide.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbToolSide.Name = "cmbToolSide";
            this.cmbToolSide.Size = new System.Drawing.Size(113, 24);
            this.cmbToolSide.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(25, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Job #";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(29, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(105, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Operation";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(191, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 16);
            this.label4.TabIndex = 12;
            this.label4.Text = "Version #";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(43, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Surface";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Location = new System.Drawing.Point(147, 18);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "Tool Side";
            // 
            // txtJobNumber
            // 
            this.txtJobNumber.Location = new System.Drawing.Point(8, 62);
            this.txtJobNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtJobNumber.Name = "txtJobNumber";
            this.txtJobNumber.Size = new System.Drawing.Size(79, 22);
            this.txtJobNumber.TabIndex = 15;
            this.txtJobNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Location = new System.Drawing.Point(109, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 16);
            this.label7.TabIndex = 17;
            this.label7.Text = "S/P";
            // 
            // cmbStudyProposal
            // 
            this.cmbStudyProposal.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbStudyProposal.FormattingEnabled = true;
            this.cmbStudyProposal.Location = new System.Drawing.Point(101, 42);
            this.cmbStudyProposal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbStudyProposal.Name = "cmbStudyProposal";
            this.cmbStudyProposal.Size = new System.Drawing.Size(60, 24);
            this.cmbStudyProposal.TabIndex = 16;
            // 
            // btnReset
            // 
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReset.Location = new System.Drawing.Point(8, 412);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(256, 28);
            this.btnReset.TabIndex = 18;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Location = new System.Drawing.Point(197, 23);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 16);
            this.label8.TabIndex = 20;
            this.label8.Text = "EC";
            // 
            // cmbEngineeringLevel
            // 
            this.cmbEngineeringLevel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbEngineeringLevel.FormattingEnabled = true;
            this.cmbEngineeringLevel.Location = new System.Drawing.Point(171, 43);
            this.cmbEngineeringLevel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbEngineeringLevel.Name = "cmbEngineeringLevel";
            this.cmbEngineeringLevel.Size = new System.Drawing.Size(72, 24);
            this.cmbEngineeringLevel.TabIndex = 19;
            this.cmbEngineeringLevel.SelectedIndexChanged += new System.EventHandler(this.ComboBoxEngineeringLevel_SelectedIndexChanged);
            // 
            // txtCustomText
            // 
            this.txtCustomText.Location = new System.Drawing.Point(8, 277);
            this.txtCustomText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCustomText.Name = "txtCustomText";
            this.txtCustomText.Size = new System.Drawing.Size(255, 22);
            this.txtCustomText.TabIndex = 21;
            this.txtCustomText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(51, 255);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(163, 16);
            this.label9.TabIndex = 22;
            this.label9.Text = "Append Text to File Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbData);
            this.groupBox1.Controls.Add(this.cmbEngineeringLevel);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbStudyProposal);
            this.groupBox1.Location = new System.Drawing.Point(8, 175);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(256, 76);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Math Data Revision Levels";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.cmbSurface);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cmbToolSide);
            this.groupBox2.Location = new System.Drawing.Point(8, 94);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(256, 74);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Direction";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdoStp);
            this.groupBox3.Controls.Add(this.rdoIges);
            this.groupBox3.Controls.Add(this.rdoPrt);
            this.groupBox3.Location = new System.Drawing.Point(8, 309);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(256, 60);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "File Type";
            // 
            // rdoStp
            // 
            this.rdoStp.AutoSize = true;
            this.rdoStp.Location = new System.Drawing.Point(165, 23);
            this.rdoStp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoStp.Name = "rdoStp";
            this.rdoStp.Size = new System.Drawing.Size(49, 20);
            this.rdoStp.TabIndex = 2;
            this.rdoStp.TabStop = true;
            this.rdoStp.Text = ".stp";
            this.rdoStp.UseVisualStyleBackColor = true;
            // 
            // rdoIges
            // 
            this.rdoIges.AutoSize = true;
            this.rdoIges.Location = new System.Drawing.Point(95, 23);
            this.rdoIges.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoIges.Name = "rdoIges";
            this.rdoIges.Size = new System.Drawing.Size(57, 20);
            this.rdoIges.TabIndex = 1;
            this.rdoIges.TabStop = true;
            this.rdoIges.Text = ".iges";
            this.rdoIges.UseVisualStyleBackColor = true;
            // 
            // rdoPrt
            // 
            this.rdoPrt.AutoSize = true;
            this.rdoPrt.Location = new System.Drawing.Point(33, 23);
            this.rdoPrt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoPrt.Name = "rdoPrt";
            this.rdoPrt.Size = new System.Drawing.Size(46, 20);
            this.rdoPrt.TabIndex = 0;
            this.rdoPrt.TabStop = true;
            this.rdoPrt.Text = ".prt";
            this.rdoPrt.UseVisualStyleBackColor = true;
            // 
            // btnSimDataDeletion
            // 
            this.btnSimDataDeletion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSimDataDeletion.Location = new System.Drawing.Point(8, 448);
            this.btnSimDataDeletion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSimDataDeletion.Name = "btnSimDataDeletion";
            this.btnSimDataDeletion.Size = new System.Drawing.Size(256, 28);
            this.btnSimDataDeletion.TabIndex = 26;
            this.btnSimDataDeletion.Text = "Delete";
            this.btnSimDataDeletion.UseVisualStyleBackColor = true;
            this.btnSimDataDeletion.Click += new System.EventHandler(this.BtnSimDataDeletion_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileStrip});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(277, 28);
            this.menuStrip1.TabIndex = 27;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileStrip
            // 
            this.fileStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topMostCheckBox});
            this.fileStrip.Name = "fileStrip";
            this.fileStrip.Size = new System.Drawing.Size(46, 24);
            this.fileStrip.Text = "File";
            // 
            // topMostCheckBox
            // 
            this.topMostCheckBox.CheckOnClick = true;
            this.topMostCheckBox.Name = "topMostCheckBox";
            this.topMostCheckBox.Size = new System.Drawing.Size(154, 26);
            this.topMostCheckBox.Text = "Top Most";
            this.topMostCheckBox.Click += new System.EventHandler(this.TopMostCheckBox_Click);
            // 
            // SimulationDataBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 490);
            this.Controls.Add(this.btnSimDataDeletion);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtCustomText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.cmbOperation);
            this.Controls.Add(this.txtJobNumber);
            this.Controls.Add(this.cmbVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(30, 130);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SimulationDataBuilderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.ComboBox cmbData;
        private System.Windows.Forms.ComboBox cmbOperation;
        private System.Windows.Forms.ComboBox cmbVersion;
        private System.Windows.Forms.ComboBox cmbSurface;
        private System.Windows.Forms.ComboBox cmbToolSide;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtJobNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbStudyProposal;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbEngineeringLevel;
        private System.Windows.Forms.TextBox txtCustomText;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdoStp;
        private System.Windows.Forms.RadioButton rdoIges;
        private System.Windows.Forms.RadioButton rdoPrt;
        private System.Windows.Forms.Button btnSimDataDeletion;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileStrip;
        private System.Windows.Forms.ToolStripMenuItem topMostCheckBox;
    }
}