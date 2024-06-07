namespace TSG_Library.UFuncs
{
    partial class CloneStripForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtJobPath = new System.Windows.Forms.TextBox();
            this.numUpDownTransfer = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDownProg = new System.Windows.Forms.NumericUpDown();
            this.chk005 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOfflineDie = new System.Windows.Forms.CheckBox();
            this.btnCreateStationNew = new System.Windows.Forms.Button();
            this.numUpDownTranPresses = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numUpDownProgPresses = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAddProgPress = new System.Windows.Forms.Button();
            this.btnAddTranPress = new System.Windows.Forms.Button();
            this.btnDeleteSimulation = new System.Windows.Forms.Button();
            this.btnAddSimulation = new System.Windows.Forms.Button();
            this.btnDeleteStation = new System.Windows.Forms.Button();
            this.btnAddStation = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTransfer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProg)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTranPresses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProgPresses)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Job Path:";
            // 
            // txtJobPath
            // 
            this.txtJobPath.Location = new System.Drawing.Point(85, 16);
            this.txtJobPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtJobPath.Name = "txtJobPath";
            this.txtJobPath.Size = new System.Drawing.Size(207, 22);
            this.txtJobPath.TabIndex = 2;
            this.txtJobPath.Text = "Double Click";
            this.txtJobPath.TextChanged += new System.EventHandler(this.txtJobPath_TextChanged);
            this.txtJobPath.DoubleClick += new System.EventHandler(this.txtJobPath_DoubleClick);
            // 
            // numUpDownTransfer
            // 
            this.numUpDownTransfer.Location = new System.Drawing.Point(136, 80);
            this.numUpDownTransfer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownTransfer.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numUpDownTransfer.Name = "numUpDownTransfer";
            this.numUpDownTransfer.Size = new System.Drawing.Size(157, 22);
            this.numUpDownTransfer.TabIndex = 3;
            this.numUpDownTransfer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownTransfer.ValueChanged += new System.EventHandler(this.NumUpDownTransfer_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 82);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Transfer Stations:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 50);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Prog Stations:";
            // 
            // numUpDownProg
            // 
            this.numUpDownProg.Location = new System.Drawing.Point(136, 48);
            this.numUpDownProg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownProg.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numUpDownProg.Name = "numUpDownProg";
            this.numUpDownProg.Size = new System.Drawing.Size(157, 22);
            this.numUpDownProg.TabIndex = 5;
            this.numUpDownProg.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDownProg.ValueChanged += new System.EventHandler(this.NumUpDownProg_ValueChanged);
            // 
            // chk005
            // 
            this.chk005.AutoSize = true;
            this.chk005.Location = new System.Drawing.Point(136, 112);
            this.chk005.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chk005.Name = "chk005";
            this.chk005.Size = new System.Drawing.Size(82, 20);
            this.chk005.TabIndex = 9;
            this.chk005.Text = "Prog 005";
            this.chk005.UseVisualStyleBackColor = true;
            this.chk005.Visible = false;
            this.chk005.CheckedChanged += new System.EventHandler(this.Chk005_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkOfflineDie);
            this.groupBox1.Controls.Add(this.btnCreateStationNew);
            this.groupBox1.Controls.Add(this.numUpDownTranPresses);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numUpDownProgPresses);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chk005);
            this.groupBox1.Controls.Add(this.txtJobPath);
            this.groupBox1.Controls.Add(this.numUpDownTransfer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numUpDownProg);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(309, 246);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create";
            // 
            // chkOfflineDie
            // 
            this.chkOfflineDie.AutoSize = true;
            this.chkOfflineDie.Location = new System.Drawing.Point(28, 112);
            this.chkOfflineDie.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkOfflineDie.Name = "chkOfflineDie";
            this.chkOfflineDie.Size = new System.Drawing.Size(90, 20);
            this.chkOfflineDie.TabIndex = 15;
            this.chkOfflineDie.Text = "Offline Die";
            this.chkOfflineDie.UseVisualStyleBackColor = true;
            this.chkOfflineDie.CheckedChanged += new System.EventHandler(this.ChkOfflineDie_CheckedChanged);
            // 
            // btnCreateStationNew
            // 
            this.btnCreateStationNew.Location = new System.Drawing.Point(8, 204);
            this.btnCreateStationNew.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreateStationNew.Name = "btnCreateStationNew";
            this.btnCreateStationNew.Size = new System.Drawing.Size(285, 28);
            this.btnCreateStationNew.TabIndex = 14;
            this.btnCreateStationNew.Text = "Create Station New";
            this.btnCreateStationNew.UseVisualStyleBackColor = true;
            this.btnCreateStationNew.Click += new System.EventHandler(this.btnCreateStationNew_Click);
            // 
            // numUpDownTranPresses
            // 
            this.numUpDownTranPresses.Location = new System.Drawing.Point(136, 172);
            this.numUpDownTranPresses.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownTranPresses.Name = "numUpDownTranPresses";
            this.numUpDownTranPresses.Size = new System.Drawing.Size(157, 22);
            this.numUpDownTranPresses.TabIndex = 10;
            this.numUpDownTranPresses.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Prog Presses:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 175);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Transfer Presses:";
            // 
            // numUpDownProgPresses
            // 
            this.numUpDownProgPresses.Location = new System.Drawing.Point(136, 140);
            this.numUpDownProgPresses.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownProgPresses.Name = "numUpDownProgPresses";
            this.numUpDownProgPresses.Size = new System.Drawing.Size(157, 22);
            this.numUpDownProgPresses.TabIndex = 12;
            this.numUpDownProgPresses.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAddProgPress);
            this.groupBox2.Controls.Add(this.btnAddTranPress);
            this.groupBox2.Controls.Add(this.btnDeleteSimulation);
            this.groupBox2.Controls.Add(this.btnAddSimulation);
            this.groupBox2.Controls.Add(this.btnDeleteStation);
            this.groupBox2.Controls.Add(this.btnAddStation);
            this.groupBox2.Location = new System.Drawing.Point(16, 268);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(311, 244);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Edit";
            // 
            // btnAddProgPress
            // 
            this.btnAddProgPress.Location = new System.Drawing.Point(12, 202);
            this.btnAddProgPress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddProgPress.Name = "btnAddProgPress";
            this.btnAddProgPress.Size = new System.Drawing.Size(281, 28);
            this.btnAddProgPress.TabIndex = 15;
            this.btnAddProgPress.Text = "Add Prog Press";
            this.btnAddProgPress.UseVisualStyleBackColor = true;
            this.btnAddProgPress.Click += new System.EventHandler(this.BtnAddProgPress_Click);
            // 
            // btnAddTranPress
            // 
            this.btnAddTranPress.Location = new System.Drawing.Point(12, 166);
            this.btnAddTranPress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddTranPress.Name = "btnAddTranPress";
            this.btnAddTranPress.Size = new System.Drawing.Size(281, 28);
            this.btnAddTranPress.TabIndex = 14;
            this.btnAddTranPress.Text = "Add Tran Press";
            this.btnAddTranPress.UseVisualStyleBackColor = true;
            this.btnAddTranPress.Click += new System.EventHandler(this.BtnAddTranPress_Click);
            // 
            // btnDeleteSimulation
            // 
            this.btnDeleteSimulation.Location = new System.Drawing.Point(12, 130);
            this.btnDeleteSimulation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDeleteSimulation.Name = "btnDeleteSimulation";
            this.btnDeleteSimulation.Size = new System.Drawing.Size(281, 28);
            this.btnDeleteSimulation.TabIndex = 13;
            this.btnDeleteSimulation.Text = "Delete Simulation";
            this.btnDeleteSimulation.UseVisualStyleBackColor = true;
            this.btnDeleteSimulation.Click += new System.EventHandler(this.btnDeleteSimulation_Click);
            // 
            // btnAddSimulation
            // 
            this.btnAddSimulation.Location = new System.Drawing.Point(12, 95);
            this.btnAddSimulation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddSimulation.Name = "btnAddSimulation";
            this.btnAddSimulation.Size = new System.Drawing.Size(281, 28);
            this.btnAddSimulation.TabIndex = 12;
            this.btnAddSimulation.Text = "Add Simulation";
            this.btnAddSimulation.UseVisualStyleBackColor = true;
            this.btnAddSimulation.Click += new System.EventHandler(this.btnAddSimulation_Click);
            // 
            // btnDeleteStation
            // 
            this.btnDeleteStation.Location = new System.Drawing.Point(12, 59);
            this.btnDeleteStation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDeleteStation.Name = "btnDeleteStation";
            this.btnDeleteStation.Size = new System.Drawing.Size(281, 28);
            this.btnDeleteStation.TabIndex = 11;
            this.btnDeleteStation.Text = "Delete Station";
            this.btnDeleteStation.UseVisualStyleBackColor = true;
            this.btnDeleteStation.Click += new System.EventHandler(this.BtnDeleteStation_Click);
            // 
            // btnAddStation
            // 
            this.btnAddStation.Location = new System.Drawing.Point(12, 23);
            this.btnAddStation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddStation.Name = "btnAddStation";
            this.btnAddStation.Size = new System.Drawing.Size(281, 28);
            this.btnAddStation.TabIndex = 0;
            this.btnAddStation.Text = "Add Station";
            this.btnAddStation.UseVisualStyleBackColor = true;
            this.btnAddStation.Click += new System.EventHandler(this.BtnAddStation_Click);
            // 
            // CloneStripForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 530);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CloneStripForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloneStripForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTransfer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProg)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownTranPresses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownProgPresses)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtJobPath;
        private System.Windows.Forms.NumericUpDown numUpDownTransfer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUpDownProg;
        private System.Windows.Forms.CheckBox chk005;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDeleteStation;
        private System.Windows.Forms.Button btnAddStation;
        private System.Windows.Forms.NumericUpDown numUpDownTranPresses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numUpDownProgPresses;
        private System.Windows.Forms.Button btnCreateStationNew;
        private System.Windows.Forms.CheckBox chkOfflineDie;
        private System.Windows.Forms.Button btnDeleteSimulation;
        private System.Windows.Forms.Button btnAddSimulation;
        private System.Windows.Forms.Button btnAddTranPress;
        private System.Windows.Forms.Button btnAddProgPress;
    }
}