namespace TSG_Library.UFuncs
{
    partial class CreateResetDatumCsys
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
            this.buttonCreateWcsPlanView = new System.Windows.Forms.Button();
            this.buttonResetAll = new System.Windows.Forms.Button();
            this.buttonSingle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonNonDatumCsys = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCreateWcsPlanView
            // 
            this.buttonCreateWcsPlanView.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCreateWcsPlanView.Location = new System.Drawing.Point(7, 48);
            this.buttonCreateWcsPlanView.Name = "buttonCreateWcsPlanView";
            this.buttonCreateWcsPlanView.Size = new System.Drawing.Size(120, 23);
            this.buttonCreateWcsPlanView.TabIndex = 15;
            this.buttonCreateWcsPlanView.Text = "Create WCS/PV";
            this.buttonCreateWcsPlanView.UseVisualStyleBackColor = true;
            this.buttonCreateWcsPlanView.Click += new System.EventHandler(this.ButtonCreateWcsPlanView_Click);
            // 
            // buttonResetAll
            // 
            this.buttonResetAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonResetAll.Location = new System.Drawing.Point(6, 48);
            this.buttonResetAll.Name = "buttonResetAll";
            this.buttonResetAll.Size = new System.Drawing.Size(120, 23);
            this.buttonResetAll.TabIndex = 19;
            this.buttonResetAll.Text = "Reset All Components";
            this.buttonResetAll.UseVisualStyleBackColor = true;
            this.buttonResetAll.Click += new System.EventHandler(this.ButtonResetAll_Click);
            // 
            // buttonSingle
            // 
            this.buttonSingle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSingle.Location = new System.Drawing.Point(7, 19);
            this.buttonSingle.Name = "buttonSingle";
            this.buttonSingle.Size = new System.Drawing.Size(120, 23);
            this.buttonSingle.TabIndex = 20;
            this.buttonSingle.Text = "Select Component";
            this.buttonSingle.UseVisualStyleBackColor = true;
            this.buttonSingle.Click += new System.EventHandler(this.ButtonSingle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSingle);
            this.groupBox1.Controls.Add(this.buttonCreateWcsPlanView);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(135, 85);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single Component";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonNonDatumCsys);
            this.groupBox2.Controls.Add(this.buttonResetAll);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(135, 84);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "All Components";
            // 
            // buttonNonDatumCsys
            // 
            this.buttonNonDatumCsys.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonNonDatumCsys.Location = new System.Drawing.Point(6, 19);
            this.buttonNonDatumCsys.Name = "buttonNonDatumCsys";
            this.buttonNonDatumCsys.Size = new System.Drawing.Size(120, 23);
            this.buttonNonDatumCsys.TabIndex = 20;
            this.buttonNonDatumCsys.Text = "Non CtsDatumCsys";
            this.buttonNonDatumCsys.UseVisualStyleBackColor = true;
            this.buttonNonDatumCsys.Click += new System.EventHandler(this.ButtonNonDatumCsys_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(159, 199);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Cts Datum Csys";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateWcsPlanView;
        private System.Windows.Forms.Button buttonResetAll;
        private System.Windows.Forms.Button buttonSingle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonNonDatumCsys;
    }
}