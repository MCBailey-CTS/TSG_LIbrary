namespace TSG_Library.UFuncs
{
    partial class CloneAssemblyForm
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoLine = new System.Windows.Forms.RadioButton();
            this.rdoProressive = new System.Windows.Forms.RadioButton();
            this.rdoTransfer = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdoEnglish_ = new System.Windows.Forms.RadioButton();
            this.rdoMetric_ = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtJobFolder_ = new System.Windows.Forms.TextBox();
            this.txtOps = new System.Windows.Forms.TextBox();
            this.btnTestExecute = new System.Windows.Forms.Button();
            this.chkStartWithBlank = new System.Windows.Forms.CheckBox();
            this.numudPressDiesetOp = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numudPressDiesetOp)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdoLine);
            this.groupBox4.Controls.Add(this.rdoProressive);
            this.groupBox4.Controls.Add(this.rdoTransfer);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(90, 98);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Type";
            // 
            // rdoLine
            // 
            this.rdoLine.AutoSize = true;
            this.rdoLine.Location = new System.Drawing.Point(6, 42);
            this.rdoLine.Name = "rdoLine";
            this.rdoLine.Size = new System.Drawing.Size(45, 17);
            this.rdoLine.TabIndex = 2;
            this.rdoLine.Text = "Line";
            this.rdoLine.UseVisualStyleBackColor = true;
            this.rdoLine.CheckedChanged += new System.EventHandler(this.rdoTransfer_CheckedChanged);
            // 
            // rdoProressive
            // 
            this.rdoProressive.AutoSize = true;
            this.rdoProressive.Location = new System.Drawing.Point(4, 65);
            this.rdoProressive.Name = "rdoProressive";
            this.rdoProressive.Size = new System.Drawing.Size(80, 17);
            this.rdoProressive.TabIndex = 1;
            this.rdoProressive.Text = "Progressive";
            this.rdoProressive.UseVisualStyleBackColor = true;
            this.rdoProressive.CheckedChanged += new System.EventHandler(this.rdoTransfer_CheckedChanged);
            // 
            // rdoTransfer
            // 
            this.rdoTransfer.AutoSize = true;
            this.rdoTransfer.Checked = true;
            this.rdoTransfer.Location = new System.Drawing.Point(6, 19);
            this.rdoTransfer.Name = "rdoTransfer";
            this.rdoTransfer.Size = new System.Drawing.Size(64, 17);
            this.rdoTransfer.TabIndex = 0;
            this.rdoTransfer.TabStop = true;
            this.rdoTransfer.Text = "Transfer";
            this.rdoTransfer.UseVisualStyleBackColor = true;
            this.rdoTransfer.CheckedChanged += new System.EventHandler(this.rdoTransfer_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdoEnglish_);
            this.groupBox5.Controls.Add(this.rdoMetric_);
            this.groupBox5.Location = new System.Drawing.Point(108, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(99, 69);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Units";
            // 
            // rdoEnglish_
            // 
            this.rdoEnglish_.AutoSize = true;
            this.rdoEnglish_.Location = new System.Drawing.Point(6, 42);
            this.rdoEnglish_.Name = "rdoEnglish_";
            this.rdoEnglish_.Size = new System.Drawing.Size(59, 17);
            this.rdoEnglish_.TabIndex = 1;
            this.rdoEnglish_.TabStop = true;
            this.rdoEnglish_.Text = "English";
            this.rdoEnglish_.UseVisualStyleBackColor = true;
            // 
            // rdoMetric_
            // 
            this.rdoMetric_.AutoSize = true;
            this.rdoMetric_.Checked = true;
            this.rdoMetric_.Location = new System.Drawing.Point(6, 19);
            this.rdoMetric_.Name = "rdoMetric_";
            this.rdoMetric_.Size = new System.Drawing.Size(54, 17);
            this.rdoMetric_.TabIndex = 0;
            this.rdoMetric_.TabStop = true;
            this.rdoMetric_.Text = "Metric";
            this.rdoMetric_.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Job Folder:";
            // 
            // txtJobFolder_
            // 
            this.txtJobFolder_.Location = new System.Drawing.Point(77, 160);
            this.txtJobFolder_.Name = "txtJobFolder_";
            this.txtJobFolder_.Size = new System.Drawing.Size(130, 20);
            this.txtJobFolder_.TabIndex = 6;
            this.txtJobFolder_.Text = "Double Click";
            this.txtJobFolder_.TextChanged += new System.EventHandler(this.txtJobFolder__TextChanged);
            this.txtJobFolder_.DoubleClick += new System.EventHandler(this.txtJobFolder__DoubleClick);
            // 
            // txtOps
            // 
            this.txtOps.Location = new System.Drawing.Point(9, 186);
            this.txtOps.Multiline = true;
            this.txtOps.Name = "txtOps";
            this.txtOps.Size = new System.Drawing.Size(198, 65);
            this.txtOps.TabIndex = 8;
            this.txtOps.Text = "301 010 020\r\n302 030 040";
            // 
            // btnTestExecute
            // 
            this.btnTestExecute.Location = new System.Drawing.Point(108, 87);
            this.btnTestExecute.Name = "btnTestExecute";
            this.btnTestExecute.Size = new System.Drawing.Size(99, 23);
            this.btnTestExecute.TabIndex = 9;
            this.btnTestExecute.Text = "Clone";
            this.btnTestExecute.UseVisualStyleBackColor = true;
            this.btnTestExecute.Click += new System.EventHandler(this.btnTestExecute_Click);
            // 
            // chkStartWithBlank
            // 
            this.chkStartWithBlank.AutoSize = true;
            this.chkStartWithBlank.Location = new System.Drawing.Point(12, 116);
            this.chkStartWithBlank.Name = "chkStartWithBlank";
            this.chkStartWithBlank.Size = new System.Drawing.Size(125, 17);
            this.chkStartWithBlank.TabIndex = 10;
            this.chkStartWithBlank.Text = "Start with Prog Blank";
            this.chkStartWithBlank.UseVisualStyleBackColor = true;
            // 
            // numudPressDiesetOp
            // 
            this.numudPressDiesetOp.Location = new System.Drawing.Point(159, 134);
            this.numudPressDiesetOp.Name = "numudPressDiesetOp";
            this.numudPressDiesetOp.Size = new System.Drawing.Size(48, 20);
            this.numudPressDiesetOp.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Press Op -> Dieset Control";
            // 
            // CloneAssemblyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(220, 264);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numudPressDiesetOp);
            this.Controls.Add(this.chkStartWithBlank);
            this.Controls.Add(this.btnTestExecute);
            this.Controls.Add(this.txtOps);
            this.Controls.Add(this.txtJobFolder_);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Name = "CloneAssemblyForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloneAssemblyForm_FormClosed);
            this.Load += new System.EventHandler(this.CloneAssemblyForm_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numudPressDiesetOp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdoLine;
        private System.Windows.Forms.RadioButton rdoProressive;
        private System.Windows.Forms.RadioButton rdoTransfer;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rdoEnglish_;
        private System.Windows.Forms.RadioButton rdoMetric_;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtJobFolder_;
        private System.Windows.Forms.TextBox txtOps;
        private System.Windows.Forms.Button btnTestExecute;
        private System.Windows.Forms.CheckBox chkStartWithBlank;
        private System.Windows.Forms.NumericUpDown numudPressDiesetOp;
        private System.Windows.Forms.Label label2;
    }
}