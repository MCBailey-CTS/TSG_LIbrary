namespace TSG_Library.UFuncs
{
    partial class PlotDetailDrawings
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
            this.checkBoxPrintAll = new System.Windows.Forms.CheckBox();
            this.checkBoxPrintSelected = new System.Windows.Forms.CheckBox();
            this.checkBoxPrintAssem = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxPrintAll
            // 
            this.checkBoxPrintAll.AutoSize = true;
            this.checkBoxPrintAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxPrintAll.Location = new System.Drawing.Point(16, 15);
            this.checkBoxPrintAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxPrintAll.Name = "checkBoxPrintAll";
            this.checkBoxPrintAll.Size = new System.Drawing.Size(161, 21);
            this.checkBoxPrintAll.TabIndex = 0;
            this.checkBoxPrintAll.Text = "Print All Components";
            this.checkBoxPrintAll.UseVisualStyleBackColor = true;
            this.checkBoxPrintAll.CheckedChanged += new System.EventHandler(this.CheckBoxPrintAll_CheckedChanged);
            // 
            // checkBoxPrintSelected
            // 
            this.checkBoxPrintSelected.AutoSize = true;
            this.checkBoxPrintSelected.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxPrintSelected.Location = new System.Drawing.Point(16, 71);
            this.checkBoxPrintSelected.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxPrintSelected.Name = "checkBoxPrintSelected";
            this.checkBoxPrintSelected.Size = new System.Drawing.Size(200, 21);
            this.checkBoxPrintSelected.TabIndex = 1;
            this.checkBoxPrintSelected.Text = "Print Selected Components";
            this.checkBoxPrintSelected.UseVisualStyleBackColor = true;
            this.checkBoxPrintSelected.CheckedChanged += new System.EventHandler(this.CheckBoxPrintSelected_CheckedChanged);
            // 
            // checkBoxPrintAssem
            // 
            this.checkBoxPrintAssem.AutoSize = true;
            this.checkBoxPrintAssem.Enabled = false;
            this.checkBoxPrintAssem.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxPrintAssem.Location = new System.Drawing.Point(16, 43);
            this.checkBoxPrintAssem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxPrintAssem.Name = "checkBoxPrintAssem";
            this.checkBoxPrintAssem.Size = new System.Drawing.Size(186, 21);
            this.checkBoxPrintAssem.TabIndex = 2;
            this.checkBoxPrintAssem.Text = "Print Assembly Drawings";
            this.checkBoxPrintAssem.UseVisualStyleBackColor = true;
            this.checkBoxPrintAssem.CheckedChanged += new System.EventHandler(this.CheckBoxPrintAssem_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOk.Location = new System.Drawing.Point(16, 102);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(100, 28);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(143, 102);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // PlotDetailDrawings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 154);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxPrintAssem);
            this.Controls.Add(this.checkBoxPrintSelected);
            this.Controls.Add(this.checkBoxPrintAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlotDetailDrawings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Plot DetailPart Sheets";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PlotDetailDrawings_FormClosed);
            this.Load += new System.EventHandler(this.PlotDetailDrawings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPrintAll;
        private System.Windows.Forms.CheckBox checkBoxPrintSelected;
        private System.Windows.Forms.CheckBox checkBoxPrintAssem;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}