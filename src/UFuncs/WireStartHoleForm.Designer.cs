namespace TSG_Library.UFuncs
{
    partial class WireStartHoleForm
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
            this.rdo125 = new System.Windows.Forms.RadioButton();
            this.rdo250 = new System.Windows.Forms.RadioButton();
            this.rdo375 = new System.Windows.Forms.RadioButton();
            this.chkSubtract = new System.Windows.Forms.CheckBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rdo125
            // 
            this.rdo125.AutoSize = true;
            this.rdo125.Location = new System.Drawing.Point(12, 12);
            this.rdo125.Name = "rdo125";
            this.rdo125.Size = new System.Drawing.Size(46, 17);
            this.rdo125.TabIndex = 0;
            this.rdo125.TabStop = true;
            this.rdo125.Text = ".125";
            this.rdo125.UseVisualStyleBackColor = true;
            // 
            // rdo250
            // 
            this.rdo250.AutoSize = true;
            this.rdo250.Location = new System.Drawing.Point(12, 35);
            this.rdo250.Name = "rdo250";
            this.rdo250.Size = new System.Drawing.Size(46, 17);
            this.rdo250.TabIndex = 1;
            this.rdo250.TabStop = true;
            this.rdo250.Text = ".250";
            this.rdo250.UseVisualStyleBackColor = true;
            // 
            // rdo375
            // 
            this.rdo375.AutoSize = true;
            this.rdo375.Location = new System.Drawing.Point(12, 58);
            this.rdo375.Name = "rdo375";
            this.rdo375.Size = new System.Drawing.Size(46, 17);
            this.rdo375.TabIndex = 2;
            this.rdo375.TabStop = true;
            this.rdo375.Text = ".375";
            this.rdo375.UseVisualStyleBackColor = true;
            // 
            // chkSubtract
            // 
            this.chkSubtract.AutoSize = true;
            this.chkSubtract.Location = new System.Drawing.Point(61, 12);
            this.chkSubtract.Name = "chkSubtract";
            this.chkSubtract.Size = new System.Drawing.Size(107, 17);
            this.chkSubtract.TabIndex = 3;
            this.chkSubtract.Text = "Extrude/Subtract";
            this.chkSubtract.UseVisualStyleBackColor = true;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(12, 81);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(93, 81);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // WireStartHoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 114);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.chkSubtract);
            this.Controls.Add(this.rdo375);
            this.Controls.Add(this.rdo250);
            this.Controls.Add(this.rdo125);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WireStartHoleForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WireStartHoleForm_FormClosed);
            this.Load += new System.EventHandler(this.WireStartHoleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdo125;
        private System.Windows.Forms.RadioButton rdo250;
        private System.Windows.Forms.RadioButton rdo375;
        private System.Windows.Forms.CheckBox chkSubtract;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnExit;
    }
}