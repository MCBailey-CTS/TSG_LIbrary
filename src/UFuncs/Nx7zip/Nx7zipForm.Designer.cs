namespace TSG_Library.UFuncs
{
    partial class Nx7zipForm
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
            this.labelAssmText = new System.Windows.Forms.Label();
            this.buttonSelectAssm = new System.Windows.Forms.Button();
            this.buttonLoadCurrent = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBarLoadAssm = new System.Windows.Forms.ProgressBar();
            this.buttonCloseAssm = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxCompression = new System.Windows.Forms.ComboBox();
            this.labelZipText = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAssmText
            // 
            this.labelAssmText.AutoEllipsis = true;
            this.labelAssmText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelAssmText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelAssmText.Location = new System.Drawing.Point(6, 16);
            this.labelAssmText.Name = "labelAssmText";
            this.labelAssmText.Size = new System.Drawing.Size(250, 20);
            this.labelAssmText.TabIndex = 12;
            this.labelAssmText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelAssmText.MouseHover += new System.EventHandler(this.LabelAssmText_MouseHover);
            // 
            // buttonSelectAssm
            // 
            this.buttonSelectAssm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelectAssm.Location = new System.Drawing.Point(6, 89);
            this.buttonSelectAssm.Name = "buttonSelectAssm";
            this.buttonSelectAssm.Size = new System.Drawing.Size(250, 23);
            this.buttonSelectAssm.TabIndex = 13;
            this.buttonSelectAssm.Text = "Select Assembly";
            this.buttonSelectAssm.UseVisualStyleBackColor = true;
            this.buttonSelectAssm.Click += new System.EventHandler(this.ButtonSelectAssm_Click);
            // 
            // buttonLoadCurrent
            // 
            this.buttonLoadCurrent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLoadCurrent.Location = new System.Drawing.Point(6, 60);
            this.buttonLoadCurrent.Name = "buttonLoadCurrent";
            this.buttonLoadCurrent.Size = new System.Drawing.Size(250, 23);
            this.buttonLoadCurrent.TabIndex = 14;
            this.buttonLoadCurrent.Text = "Current Assembly";
            this.buttonLoadCurrent.UseVisualStyleBackColor = true;
            this.buttonLoadCurrent.Click += new System.EventHandler(this.ButtonLoadCurrent_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBarLoadAssm);
            this.groupBox1.Controls.Add(this.buttonCloseAssm);
            this.groupBox1.Controls.Add(this.labelAssmText);
            this.groupBox1.Controls.Add(this.buttonSelectAssm);
            this.groupBox1.Controls.Add(this.buttonLoadCurrent);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 155);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assembly";
            // 
            // progressBarLoadAssm
            // 
            this.progressBarLoadAssm.Location = new System.Drawing.Point(6, 44);
            this.progressBarLoadAssm.Name = "progressBarLoadAssm";
            this.progressBarLoadAssm.Size = new System.Drawing.Size(250, 10);
            this.progressBarLoadAssm.TabIndex = 16;
            // 
            // buttonCloseAssm
            // 
            this.buttonCloseAssm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCloseAssm.Location = new System.Drawing.Point(6, 118);
            this.buttonCloseAssm.Name = "buttonCloseAssm";
            this.buttonCloseAssm.Size = new System.Drawing.Size(250, 23);
            this.buttonCloseAssm.TabIndex = 15;
            this.buttonCloseAssm.Text = "Close";
            this.buttonCloseAssm.UseVisualStyleBackColor = true;
            this.buttonCloseAssm.Click += new System.EventHandler(this.ButtonCloseAssm_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonSave);
            this.groupBox2.Controls.Add(this.buttonOk);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.labelZipText);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 173);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(265, 160);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Zip File";
            // 
            // buttonSave
            // 
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSave.Location = new System.Drawing.Point(6, 95);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(250, 23);
            this.buttonSave.TabIndex = 20;
            this.buttonSave.Text = "Save File";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOk.Location = new System.Drawing.Point(6, 124);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(250, 23);
            this.buttonOk.TabIndex = 19;
            this.buttonOk.Text = "Zip File";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxCompression);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(6, 39);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 50);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Compression";
            // 
            // comboBoxCompression
            // 
            this.comboBoxCompression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxCompression.FormattingEnabled = true;
            this.comboBoxCompression.Location = new System.Drawing.Point(6, 19);
            this.comboBoxCompression.Name = "comboBoxCompression";
            this.comboBoxCompression.Size = new System.Drawing.Size(238, 21);
            this.comboBoxCompression.TabIndex = 17;
            // 
            // labelZipText
            // 
            this.labelZipText.AutoEllipsis = true;
            this.labelZipText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelZipText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelZipText.Location = new System.Drawing.Point(6, 16);
            this.labelZipText.Name = "labelZipText";
            this.labelZipText.Size = new System.Drawing.Size(250, 20);
            this.labelZipText.TabIndex = 13;
            this.labelZipText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelZipText.MouseHover += new System.EventHandler(this.LabelZipText_MouseHover);
            // 
            // buttonExit
            // 
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(18, 348);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(250, 23);
            this.buttonExit.TabIndex = 21;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = ".prt|*.prt";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "7z";
            this.saveFileDialog1.Filter = ".7z|*.7z";
            // 
            // Nx7zipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 390);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.Name = "Nx7zipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Nx7zip 1919";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelAssmText;
        private System.Windows.Forms.Button buttonSelectAssm;
        private System.Windows.Forms.Button buttonLoadCurrent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelZipText;
        private System.Windows.Forms.ComboBox comboBoxCompression;
        private System.Windows.Forms.Button buttonCloseAssm;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.ProgressBar progressBarLoadAssm;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
