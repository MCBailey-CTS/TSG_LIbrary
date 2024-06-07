namespace TSG_Library.UFuncs
{
    partial class CreateBurnoutForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxDeleteBurnout = new System.Windows.Forms.CheckBox();
            this.checkBoxBurnoutSheet = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonSelect);
            this.groupBox2.Controls.Add(this.buttonSelectAll);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(16, 113);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(180, 105);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection";
            // 
            // buttonSelect
            // 
            this.buttonSelect.Enabled = false;
            this.buttonSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelect.Location = new System.Drawing.Point(8, 23);
            this.buttonSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(157, 28);
            this.buttonSelect.TabIndex = 3;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Enabled = false;
            this.buttonSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelectAll.Location = new System.Drawing.Point(8, 59);
            this.buttonSelectAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(157, 28);
            this.buttonSelectAll.TabIndex = 5;
            this.buttonSelectAll.Text = "Select All";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxDeleteBurnout);
            this.groupBox1.Controls.Add(this.checkBoxBurnoutSheet);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(16, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(180, 92);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // checkBoxDeleteBurnout
            // 
            this.checkBoxDeleteBurnout.AutoSize = true;
            this.checkBoxDeleteBurnout.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxDeleteBurnout.Location = new System.Drawing.Point(8, 53);
            this.checkBoxDeleteBurnout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxDeleteBurnout.Name = "checkBoxDeleteBurnout";
            this.checkBoxDeleteBurnout.Size = new System.Drawing.Size(133, 21);
            this.checkBoxDeleteBurnout.TabIndex = 3;
            this.checkBoxDeleteBurnout.Text = "Delete Burnouts";
            this.checkBoxDeleteBurnout.UseVisualStyleBackColor = true;
            this.checkBoxDeleteBurnout.CheckedChanged += new System.EventHandler(this.CheckBoxDeleteBurnout_CheckedChanged);
            // 
            // checkBoxBurnoutSheet
            // 
            this.checkBoxBurnoutSheet.AutoSize = true;
            this.checkBoxBurnoutSheet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxBurnoutSheet.Location = new System.Drawing.Point(8, 23);
            this.checkBoxBurnoutSheet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxBurnoutSheet.Name = "checkBoxBurnoutSheet";
            this.checkBoxBurnoutSheet.Size = new System.Drawing.Size(133, 21);
            this.checkBoxBurnoutSheet.TabIndex = 1;
            this.checkBoxBurnoutSheet.Text = "Create Burnouts";
            this.checkBoxBurnoutSheet.UseVisualStyleBackColor = true;
            this.checkBoxBurnoutSheet.CheckedChanged += new System.EventHandler(this.CheckBoxBurnoutSheet_CheckedChanged);
            // 
            // CreateBurnoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 225);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateBurnoutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxDeleteBurnout;
        private System.Windows.Forms.CheckBox checkBoxBurnoutSheet;
    }
}