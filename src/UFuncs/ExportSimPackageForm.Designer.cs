namespace TSG_Library.UFuncs
{
    partial class ExportSimPackageForm
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
            this.grpType = new System.Windows.Forms.GroupBox();
            this.rdoStpIges = new System.Windows.Forms.RadioButton();
            this.rdoPrt = new System.Windows.Forms.RadioButton();
            this.btnSelect = new System.Windows.Forms.Button();
            this.chkCopy = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpType.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.rdoStpIges);
            this.grpType.Controls.Add(this.rdoPrt);
            this.grpType.Location = new System.Drawing.Point(12, 12);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(171, 49);
            this.grpType.TabIndex = 0;
            this.grpType.TabStop = false;
            this.grpType.Text = "File Type";
            // 
            // rdoStpIges
            // 
            this.rdoStpIges.AutoSize = true;
            this.rdoStpIges.Location = new System.Drawing.Point(79, 19);
            this.rdoStpIges.Name = "rdoStpIges";
            this.rdoStpIges.Size = new System.Drawing.Size(67, 17);
            this.rdoStpIges.TabIndex = 1;
            this.rdoStpIges.TabStop = true;
            this.rdoStpIges.Text = ".stp - .igs";
            this.rdoStpIges.UseVisualStyleBackColor = true;
            // 
            // rdoPrt
            // 
            this.rdoPrt.AutoSize = true;
            this.rdoPrt.Location = new System.Drawing.Point(21, 19);
            this.rdoPrt.Name = "rdoPrt";
            this.rdoPrt.Size = new System.Drawing.Size(40, 17);
            this.rdoPrt.TabIndex = 0;
            this.rdoPrt.TabStop = true;
            this.rdoPrt.Text = ".prt";
            this.rdoPrt.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(12, 110);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(171, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // chkCopy
            // 
            this.chkCopy.AutoSize = true;
            this.chkCopy.Location = new System.Drawing.Point(12, 67);
            this.chkCopy.Name = "chkCopy";
            this.chkCopy.Size = new System.Drawing.Size(68, 17);
            this.chkCopy.TabIndex = 2;
            this.chkCopy.Text = "Copy to :";
            this.chkCopy.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "\\Process and Sim Data for Design";
            // 
            // ExportSimPackageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 144);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkCopy);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.grpType);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportSimPackageForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportSimPackageForm_FormClosed);
            this.Load += new System.EventHandler(this.ExportSimPackageForm_Load);
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton rdoStpIges;
        private System.Windows.Forms.RadioButton rdoPrt;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.CheckBox chkCopy;
        private System.Windows.Forms.Label label1;
    }
}