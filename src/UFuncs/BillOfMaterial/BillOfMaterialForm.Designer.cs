namespace TSG_Library.UFuncs
{
    partial class BillOfMaterialForm
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
            this.buttonCreateAts = new System.Windows.Forms.Button();
            this.buttonCreateCts = new System.Windows.Forms.Button();
            this.buttonCreateDts = new System.Windows.Forms.Button();
            this.buttonCreateEts = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCreateUGS = new System.Windows.Forms.Button();
            this.buttonCreateRTS = new System.Windows.Forms.Button();
            this.buttonCreateHTS = new System.Windows.Forms.Button();
            this.buttonLoadCastings = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClearSelection = new System.Windows.Forms.Button();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.chkMM = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCreateAts
            // 
            this.buttonCreateAts.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateAts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateAts.Location = new System.Drawing.Point(10, 17);
            this.buttonCreateAts.Name = "buttonCreateAts";
            this.buttonCreateAts.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateAts.TabIndex = 0;
            this.buttonCreateAts.Text = "ATS";
            this.buttonCreateAts.UseVisualStyleBackColor = true;
            this.buttonCreateAts.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonCreateCts
            // 
            this.buttonCreateCts.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateCts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateCts.Location = new System.Drawing.Point(10, 46);
            this.buttonCreateCts.Name = "buttonCreateCts";
            this.buttonCreateCts.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateCts.TabIndex = 1;
            this.buttonCreateCts.Text = "CTS";
            this.buttonCreateCts.UseVisualStyleBackColor = true;
            this.buttonCreateCts.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonCreateDts
            // 
            this.buttonCreateDts.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateDts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateDts.Location = new System.Drawing.Point(10, 75);
            this.buttonCreateDts.Name = "buttonCreateDts";
            this.buttonCreateDts.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateDts.TabIndex = 2;
            this.buttonCreateDts.Text = "DTS";
            this.buttonCreateDts.UseVisualStyleBackColor = true;
            this.buttonCreateDts.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonCreateEts
            // 
            this.buttonCreateEts.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateEts.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateEts.Location = new System.Drawing.Point(10, 104);
            this.buttonCreateEts.Name = "buttonCreateEts";
            this.buttonCreateEts.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateEts.TabIndex = 3;
            this.buttonCreateEts.Text = "ETS";
            this.buttonCreateEts.UseVisualStyleBackColor = true;
            this.buttonCreateEts.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonCreateUGS);
            this.groupBox1.Controls.Add(this.buttonCreateRTS);
            this.groupBox1.Controls.Add(this.buttonCreateHTS);
            this.groupBox1.Controls.Add(this.buttonCreateAts);
            this.groupBox1.Controls.Add(this.buttonCreateCts);
            this.groupBox1.Controls.Add(this.buttonCreateEts);
            this.groupBox1.Controls.Add(this.buttonCreateDts);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(170, 223);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Company";
            // 
            // buttonCreateUGS
            // 
            this.buttonCreateUGS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateUGS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateUGS.Location = new System.Drawing.Point(10, 191);
            this.buttonCreateUGS.Name = "buttonCreateUGS";
            this.buttonCreateUGS.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateUGS.TabIndex = 6;
            this.buttonCreateUGS.Text = "UGS";
            this.buttonCreateUGS.UseVisualStyleBackColor = true;
            this.buttonCreateUGS.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonCreateRTS
            // 
            this.buttonCreateRTS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateRTS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateRTS.Location = new System.Drawing.Point(10, 162);
            this.buttonCreateRTS.Name = "buttonCreateRTS";
            this.buttonCreateRTS.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateRTS.TabIndex = 5;
            this.buttonCreateRTS.Text = "RTS";
            this.buttonCreateRTS.UseVisualStyleBackColor = true;
            this.buttonCreateRTS.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonCreateHTS
            // 
            this.buttonCreateHTS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateHTS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateHTS.Location = new System.Drawing.Point(10, 133);
            this.buttonCreateHTS.Name = "buttonCreateHTS";
            this.buttonCreateHTS.Size = new System.Drawing.Size(151, 23);
            this.buttonCreateHTS.TabIndex = 4;
            this.buttonCreateHTS.Text = "HTS";
            this.buttonCreateHTS.UseVisualStyleBackColor = true;
            this.buttonCreateHTS.Click += new System.EventHandler(this.ButtonCreateBom_Click);
            // 
            // buttonLoadCastings
            // 
            this.buttonLoadCastings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoadCastings.Location = new System.Drawing.Point(10, 48);
            this.buttonLoadCastings.Name = "buttonLoadCastings";
            this.buttonLoadCastings.Size = new System.Drawing.Size(151, 23);
            this.buttonLoadCastings.TabIndex = 8;
            this.buttonLoadCastings.Text = "Bom Castings";
            this.buttonLoadCastings.UseVisualStyleBackColor = true;
            this.buttonLoadCastings.Click += new System.EventHandler(this.ButtonLoadCastings_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnClearSelection);
            this.groupBox2.Controls.Add(this.buttonSelect);
            this.groupBox2.Controls.Add(this.buttonLoadCastings);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 264);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(170, 113);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Control";
            // 
            // btnClearSelection
            // 
            this.btnClearSelection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClearSelection.Location = new System.Drawing.Point(10, 77);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(151, 23);
            this.btnClearSelection.TabIndex = 10;
            this.btnClearSelection.Text = "Clear Selection";
            this.btnClearSelection.UseVisualStyleBackColor = true;
            this.btnClearSelection.Click += new System.EventHandler(this.BtnClearSelection_Click);
            // 
            // buttonSelect
            // 
            this.buttonSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelect.Location = new System.Drawing.Point(10, 19);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(151, 23);
            this.buttonSelect.TabIndex = 9;
            this.buttonSelect.Text = "Select_";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // chkMM
            // 
            this.chkMM.AutoSize = true;
            this.chkMM.Location = new System.Drawing.Point(22, 241);
            this.chkMM.Name = "chkMM";
            this.chkMM.Size = new System.Drawing.Size(131, 17);
            this.chkMM.TabIndex = 10;
            this.chkMM.Text = "Show MM Dimensions";
            this.chkMM.UseVisualStyleBackColor = true;
            // 
            // BillOfMaterialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 389);
            this.Controls.Add(this.chkMM);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Name = "BillOfMaterialForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BillOfMaterialForm_FormClosed);
            this.Load += new System.EventHandler(this.BillOfMaterialForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateAts;
        private System.Windows.Forms.Button buttonCreateCts;
        private System.Windows.Forms.Button buttonCreateDts;
        private System.Windows.Forms.Button buttonCreateEts;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonLoadCastings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonCreateHTS;
        private System.Windows.Forms.Button buttonCreateRTS;
        private System.Windows.Forms.Button buttonCreateUGS;
        private System.Windows.Forms.Button btnClearSelection;
        private System.Windows.Forms.CheckBox chkMM;
    }
}