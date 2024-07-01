namespace TSG_Library.UFuncs
{
    partial class ProposalDataWizardForm
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
            this.lblProposalLevel = new System.Windows.Forms.Label();
            this.lstMasters = new System.Windows.Forms.ListBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.chkMakeStp = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkMasterType = new System.Windows.Forms.CheckBox();
            this.chkHistoryType = new System.Windows.Forms.CheckBox();
            this.txtPLevel = new System.Windows.Forms.TextBox();
            this.txtDataLevel = new System.Windows.Forms.TextBox();
            this.rdoS = new System.Windows.Forms.RadioButton();
            this.rdoR = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProposalLevel
            // 
            this.lblProposalLevel.AutoSize = true;
            this.lblProposalLevel.Location = new System.Drawing.Point(41, 58);
            this.lblProposalLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProposalLevel.Name = "lblProposalLevel";
            this.lblProposalLevel.Size = new System.Drawing.Size(55, 16);
            this.lblProposalLevel.TabIndex = 2;
            this.lblProposalLevel.Text = "P Level:";
            // 
            // lstMasters
            // 
            this.lstMasters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMasters.FormattingEnabled = true;
            this.lstMasters.ItemHeight = 16;
            this.lstMasters.Location = new System.Drawing.Point(16, 160);
            this.lstMasters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstMasters.Name = "lstMasters";
            this.lstMasters.Size = new System.Drawing.Size(587, 212);
            this.lstMasters.TabIndex = 4;
            this.lstMasters.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            this.lstMasters.MouseEnter += new System.EventHandler(this.Mouse_Enter);
            this.lstMasters.MouseLeave += new System.EventHandler(this.Mouse_Leave);
            this.lstMasters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Mouse_Move);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(11, 50);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(160, 28);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.Btn_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(11, 15);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(160, 28);
            this.btnCreate.TabIndex = 9;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.Btn_Click);
            // 
            // chkMakeStp
            // 
            this.chkMakeStp.AutoSize = true;
            this.chkMakeStp.Location = new System.Drawing.Point(11, 86);
            this.chkMakeStp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMakeStp.Name = "chkMakeStp";
            this.chkMakeStp.Size = new System.Drawing.Size(87, 20);
            this.chkMakeStp.TabIndex = 13;
            this.chkMakeStp.Text = "Make .stp";
            this.chkMakeStp.UseVisualStyleBackColor = true;
            this.chkMakeStp.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkMasterType);
            this.groupBox1.Controls.Add(this.chkHistoryType);
            this.groupBox1.Controls.Add(this.txtPLevel);
            this.groupBox1.Controls.Add(this.txtDataLevel);
            this.groupBox1.Controls.Add(this.rdoS);
            this.groupBox1.Controls.Add(this.rdoR);
            this.groupBox1.Controls.Add(this.lblProposalLevel);
            this.groupBox1.Location = new System.Drawing.Point(179, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(219, 128);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // chkMasterType
            // 
            this.chkMasterType.AutoSize = true;
            this.chkMasterType.Location = new System.Drawing.Point(93, 92);
            this.chkMasterType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkMasterType.Name = "chkMasterType";
            this.chkMasterType.Size = new System.Drawing.Size(70, 20);
            this.chkMasterType.TabIndex = 9;
            this.chkMasterType.Text = "Master";
            this.chkMasterType.UseVisualStyleBackColor = true;
            this.chkMasterType.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // chkHistoryType
            // 
            this.chkHistoryType.AutoSize = true;
            this.chkHistoryType.Location = new System.Drawing.Point(8, 92);
            this.chkHistoryType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkHistoryType.Name = "chkHistoryType";
            this.chkHistoryType.Size = new System.Drawing.Size(71, 20);
            this.chkHistoryType.TabIndex = 8;
            this.chkHistoryType.Text = "History";
            this.chkHistoryType.UseVisualStyleBackColor = true;
            this.chkHistoryType.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // txtPLevel
            // 
            this.txtPLevel.Location = new System.Drawing.Point(111, 54);
            this.txtPLevel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPLevel.Name = "txtPLevel";
            this.txtPLevel.Size = new System.Drawing.Size(88, 22);
            this.txtPLevel.TabIndex = 7;
            // 
            // txtDataLevel
            // 
            this.txtDataLevel.Location = new System.Drawing.Point(111, 22);
            this.txtDataLevel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDataLevel.Name = "txtDataLevel";
            this.txtDataLevel.Size = new System.Drawing.Size(88, 22);
            this.txtDataLevel.TabIndex = 6;
            // 
            // rdoS
            // 
            this.rdoS.AutoSize = true;
            this.rdoS.Location = new System.Drawing.Point(8, 23);
            this.rdoS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoS.Name = "rdoS";
            this.rdoS.Size = new System.Drawing.Size(37, 20);
            this.rdoS.TabIndex = 5;
            this.rdoS.TabStop = true;
            this.rdoS.Text = "S";
            this.rdoS.UseVisualStyleBackColor = true;
            this.rdoS.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // rdoR
            // 
            this.rdoR.AutoSize = true;
            this.rdoR.Location = new System.Drawing.Point(59, 23);
            this.rdoR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoR.Name = "rdoR";
            this.rdoR.Size = new System.Drawing.Size(38, 20);
            this.rdoR.TabIndex = 4;
            this.rdoR.TabStop = true;
            this.rdoR.Text = "R";
            this.rdoR.UseVisualStyleBackColor = true;
            this.rdoR.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // ProposalDataWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 399);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkMakeStp);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lstMasters);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ProposalDataWizardForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProposalDataWizardForm_FormClosed);
            this.Load += new System.EventHandler(this.ProposalDataWizardForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblProposalLevel;
        private System.Windows.Forms.ListBox lstMasters;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.CheckBox chkMakeStp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPLevel;
        private System.Windows.Forms.TextBox txtDataLevel;
        private System.Windows.Forms.RadioButton rdoS;
        private System.Windows.Forms.RadioButton rdoR;
        private System.Windows.Forms.CheckBox chkMasterType;
        private System.Windows.Forms.CheckBox chkHistoryType;
    }
}