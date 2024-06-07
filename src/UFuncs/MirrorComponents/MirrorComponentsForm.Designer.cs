namespace TSG_Library.UFuncs
{
    partial class MirrorComponentsForm
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
            this.buttonSelectComponents = new System.Windows.Forms.Button();
            this.textBoxDetNumber = new System.Windows.Forms.TextBox();
            this.checkBoxMirrorCopies = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelectComponents
            // 
            this.buttonSelectComponents.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelectComponents.Location = new System.Drawing.Point(18, 12);
            this.buttonSelectComponents.Name = "buttonSelectComponents";
            this.buttonSelectComponents.Size = new System.Drawing.Size(178, 23);
            this.buttonSelectComponents.TabIndex = 0;
            this.buttonSelectComponents.Text = "Select Components";
            this.buttonSelectComponents.UseVisualStyleBackColor = true;
            this.buttonSelectComponents.Click += new System.EventHandler(this.ButtonSelectComponents_Click);
            // 
            // textBoxDetNumber
            // 
            this.textBoxDetNumber.Enabled = false;
            this.textBoxDetNumber.Location = new System.Drawing.Point(59, 49);
            this.textBoxDetNumber.Name = "textBoxDetNumber";
            this.textBoxDetNumber.Size = new System.Drawing.Size(72, 20);
            this.textBoxDetNumber.TabIndex = 1;
            // 
            // checkBoxMirrorCopies
            // 
            this.checkBoxMirrorCopies.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxMirrorCopies.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxMirrorCopies.Location = new System.Drawing.Point(6, 19);
            this.checkBoxMirrorCopies.Name = "checkBoxMirrorCopies";
            this.checkBoxMirrorCopies.Size = new System.Drawing.Size(178, 24);
            this.checkBoxMirrorCopies.TabIndex = 2;
            this.checkBoxMirrorCopies.Text = "Mirror Copies";
            this.checkBoxMirrorCopies.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxMirrorCopies.UseVisualStyleBackColor = true;
            this.checkBoxMirrorCopies.CheckedChanged += new System.EventHandler(this.CheckBoxMirrorCopies_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxMirrorCopies);
            this.groupBox1.Controls.Add(this.textBoxDetNumber);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 80);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Detail #";
            // 
            // buttonExit
            // 
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(18, 127);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(86, 23);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Enabled = false;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOk.Location = new System.Drawing.Point(110, 127);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(86, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 159);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonSelectComponents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mirror Components";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSelectComponents;
        private System.Windows.Forms.TextBox textBoxDetNumber;
        private System.Windows.Forms.CheckBox checkBoxMirrorCopies;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonOk;
    }
}