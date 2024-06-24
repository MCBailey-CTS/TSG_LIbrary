//namespace TSG_Library.UFuncs
//{
//    partial class CopyAttributesForm
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.listBoxAttributes = new System.Windows.Forms.ListBox();
//            this.buttonSelectAll = new System.Windows.Forms.Button();
//            this.buttonSelect = new System.Windows.Forms.Button();
//            this.buttonSetAttrDefaults = new System.Windows.Forms.Button();
//            this.buttonCopyApply = new System.Windows.Forms.Button();
//            this.buttonAddAttribute = new System.Windows.Forms.Button();
//            this.textBoxAttrTitle = new System.Windows.Forms.TextBox();
//            this.groupBox1 = new System.Windows.Forms.GroupBox();
//            this.groupBox2 = new System.Windows.Forms.GroupBox();
//            this.textBoxAttrValue = new System.Windows.Forms.TextBox();
//            this.groupBox3 = new System.Windows.Forms.GroupBox();
//            this.progressBarLoadComps = new System.Windows.Forms.ProgressBar();
//            this.groupBox4 = new System.Windows.Forms.GroupBox();
//            this.buttonAttrDelete = new System.Windows.Forms.Button();
//            this.progressBarCopyAttr = new System.Windows.Forms.ProgressBar();
//            this.buttonReset = new System.Windows.Forms.Button();
//            this.groupBox1.SuspendLayout();
//            this.groupBox2.SuspendLayout();
//            this.groupBox3.SuspendLayout();
//            this.groupBox4.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // listBoxAttributes
//            // 
//            this.listBoxAttributes.FormattingEnabled = true;
//            this.listBoxAttributes.ItemHeight = 16;
//            this.listBoxAttributes.Location = new System.Drawing.Point(16, 430);
//            this.listBoxAttributes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.listBoxAttributes.Name = "listBoxAttributes";
//            this.listBoxAttributes.ScrollAlwaysVisible = true;
//            this.listBoxAttributes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
//            this.listBoxAttributes.Size = new System.Drawing.Size(285, 164);
//            this.listBoxAttributes.TabIndex = 0;
//            this.listBoxAttributes.SelectedIndexChanged += new System.EventHandler(this.listBoxAttributes_SelectedIndexChanged);
//            // 
//            // buttonSelectAll
//            // 
//            this.buttonSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonSelectAll.Location = new System.Drawing.Point(8, 43);
//            this.buttonSelectAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonSelectAll.Name = "buttonSelectAll";
//            this.buttonSelectAll.Size = new System.Drawing.Size(267, 28);
//            this.buttonSelectAll.TabIndex = 1;
//            this.buttonSelectAll.Text = "Select All";
//            this.buttonSelectAll.UseVisualStyleBackColor = true;
//            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
//            // 
//            // buttonSelect
//            // 
//            this.buttonSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonSelect.Location = new System.Drawing.Point(8, 79);
//            this.buttonSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonSelect.Name = "buttonSelect";
//            this.buttonSelect.Size = new System.Drawing.Size(267, 28);
//            this.buttonSelect.TabIndex = 2;
//            this.buttonSelect.Text = "Select";
//            this.buttonSelect.UseVisualStyleBackColor = true;
//            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
//            // 
//            // buttonSetAttrDefaults
//            // 
//            this.buttonSetAttrDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonSetAttrDefaults.Location = new System.Drawing.Point(8, 23);
//            this.buttonSetAttrDefaults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonSetAttrDefaults.Name = "buttonSetAttrDefaults";
//            this.buttonSetAttrDefaults.Size = new System.Drawing.Size(267, 28);
//            this.buttonSetAttrDefaults.TabIndex = 3;
//            this.buttonSetAttrDefaults.Text = "Set Default Attributes";
//            this.buttonSetAttrDefaults.UseVisualStyleBackColor = true;
//            this.buttonSetAttrDefaults.Click += new System.EventHandler(this.buttonSetAttrDefaults_Click);
//            // 
//            // buttonCopyApply
//            // 
//            this.buttonCopyApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonCopyApply.Location = new System.Drawing.Point(16, 638);
//            this.buttonCopyApply.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonCopyApply.Name = "buttonCopyApply";
//            this.buttonCopyApply.Size = new System.Drawing.Size(287, 28);
//            this.buttonCopyApply.TabIndex = 4;
//            this.buttonCopyApply.Text = "Apply";
//            this.buttonCopyApply.UseVisualStyleBackColor = true;
//            this.buttonCopyApply.Click += new System.EventHandler(this.buttonCopyApply_Click);
//            // 
//            // buttonAddAttribute
//            // 
//            this.buttonAddAttribute.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonAddAttribute.Location = new System.Drawing.Point(8, 197);
//            this.buttonAddAttribute.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonAddAttribute.Name = "buttonAddAttribute";
//            this.buttonAddAttribute.Size = new System.Drawing.Size(267, 28);
//            this.buttonAddAttribute.TabIndex = 6;
//            this.buttonAddAttribute.Text = "Update";
//            this.buttonAddAttribute.UseVisualStyleBackColor = true;
//            this.buttonAddAttribute.Click += new System.EventHandler(this.buttonAddAttribute_Click);
//            // 
//            // textBoxAttrTitle
//            // 
//            this.textBoxAttrTitle.Location = new System.Drawing.Point(8, 21);
//            this.textBoxAttrTitle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.textBoxAttrTitle.Name = "textBoxAttrTitle";
//            this.textBoxAttrTitle.Size = new System.Drawing.Size(249, 22);
//            this.textBoxAttrTitle.TabIndex = 7;
//            this.textBoxAttrTitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxAttrTitle_KeyDown);
//            // 
//            // groupBox1
//            // 
//            this.groupBox1.Controls.Add(this.textBoxAttrTitle);
//            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.groupBox1.Location = new System.Drawing.Point(8, 59);
//            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox1.Name = "groupBox1";
//            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox1.Size = new System.Drawing.Size(267, 62);
//            this.groupBox1.TabIndex = 9;
//            this.groupBox1.TabStop = false;
//            this.groupBox1.Text = "Title";
//            // 
//            // groupBox2
//            // 
//            this.groupBox2.Controls.Add(this.textBoxAttrValue);
//            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.groupBox2.Location = new System.Drawing.Point(8, 128);
//            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox2.Name = "groupBox2";
//            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox2.Size = new System.Drawing.Size(267, 62);
//            this.groupBox2.TabIndex = 10;
//            this.groupBox2.TabStop = false;
//            this.groupBox2.Text = "Value";
//            // 
//            // textBoxAttrValue
//            // 
//            this.textBoxAttrValue.Location = new System.Drawing.Point(8, 21);
//            this.textBoxAttrValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.textBoxAttrValue.Name = "textBoxAttrValue";
//            this.textBoxAttrValue.Size = new System.Drawing.Size(249, 22);
//            this.textBoxAttrValue.TabIndex = 7;
//            this.textBoxAttrValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxAttrValue_KeyDown);
//            // 
//            // groupBox3
//            // 
//            this.groupBox3.Controls.Add(this.progressBarLoadComps);
//            this.groupBox3.Controls.Add(this.buttonSelectAll);
//            this.groupBox3.Controls.Add(this.buttonSelect);
//            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.groupBox3.Location = new System.Drawing.Point(16, 15);
//            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox3.Name = "groupBox3";
//            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox3.Size = new System.Drawing.Size(287, 123);
//            this.groupBox3.TabIndex = 11;
//            this.groupBox3.TabStop = false;
//            this.groupBox3.Text = "Selection Type";
//            // 
//            // progressBarLoadComps
//            // 
//            this.progressBarLoadComps.Location = new System.Drawing.Point(8, 23);
//            this.progressBarLoadComps.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.progressBarLoadComps.Name = "progressBarLoadComps";
//            this.progressBarLoadComps.Size = new System.Drawing.Size(267, 12);
//            this.progressBarLoadComps.TabIndex = 14;
//            // 
//            // groupBox4
//            // 
//            this.groupBox4.Controls.Add(this.buttonAttrDelete);
//            this.groupBox4.Controls.Add(this.buttonSetAttrDefaults);
//            this.groupBox4.Controls.Add(this.buttonAddAttribute);
//            this.groupBox4.Controls.Add(this.groupBox2);
//            this.groupBox4.Controls.Add(this.groupBox1);
//            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.groupBox4.Location = new System.Drawing.Point(16, 145);
//            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox4.Name = "groupBox4";
//            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.groupBox4.Size = new System.Drawing.Size(287, 277);
//            this.groupBox4.TabIndex = 12;
//            this.groupBox4.TabStop = false;
//            this.groupBox4.Text = "Attributes";
//            // 
//            // buttonAttrDelete
//            // 
//            this.buttonAttrDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonAttrDelete.Location = new System.Drawing.Point(8, 233);
//            this.buttonAttrDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonAttrDelete.Name = "buttonAttrDelete";
//            this.buttonAttrDelete.Size = new System.Drawing.Size(267, 28);
//            this.buttonAttrDelete.TabIndex = 13;
//            this.buttonAttrDelete.Text = "Delete";
//            this.buttonAttrDelete.UseVisualStyleBackColor = true;
//            this.buttonAttrDelete.Click += new System.EventHandler(this.buttonAttrDelete_Click);
//            // 
//            // progressBarCopyAttr
//            // 
//            this.progressBarCopyAttr.Location = new System.Drawing.Point(16, 602);
//            this.progressBarCopyAttr.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.progressBarCopyAttr.Name = "progressBarCopyAttr";
//            this.progressBarCopyAttr.Size = new System.Drawing.Size(287, 28);
//            this.progressBarCopyAttr.TabIndex = 13;
//            // 
//            // buttonReset
//            // 
//            this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
//            this.buttonReset.Location = new System.Drawing.Point(16, 673);
//            this.buttonReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.buttonReset.Name = "buttonReset";
//            this.buttonReset.Size = new System.Drawing.Size(287, 28);
//            this.buttonReset.TabIndex = 14;
//            this.buttonReset.Text = "Reset";
//            this.buttonReset.UseVisualStyleBackColor = true;
//            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
//            // 
//            // CopyAttributesForm
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(319, 716);
//            this.Controls.Add(this.buttonReset);
//            this.Controls.Add(this.progressBarCopyAttr);
//            this.Controls.Add(this.groupBox4);
//            this.Controls.Add(this.groupBox3);
//            this.Controls.Add(this.buttonCopyApply);
//            this.Controls.Add(this.listBoxAttributes);
//            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
//            this.Location = new System.Drawing.Point(30, 130);
//            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
//            this.Name = "CopyAttributesForm";
//            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
//            this.Text = "1919";
//            this.TopMost = true;
//            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CopyAttributesForm_FormClosed);
//            this.Load += new System.EventHandler(this.MainForm_Load);
//            this.groupBox1.ResumeLayout(false);
//            this.groupBox1.PerformLayout();
//            this.groupBox2.ResumeLayout(false);
//            this.groupBox2.PerformLayout();
//            this.groupBox3.ResumeLayout(false);
//            this.groupBox4.ResumeLayout(false);
//            this.ResumeLayout(false);

//        }

//        #endregion

//        private System.Windows.Forms.ListBox listBoxAttributes;
//        private System.Windows.Forms.Button buttonSelectAll;
//        private System.Windows.Forms.Button buttonSelect;
//        private System.Windows.Forms.Button buttonSetAttrDefaults;
//        private System.Windows.Forms.Button buttonCopyApply;
//        private System.Windows.Forms.Button buttonAddAttribute;
//        private System.Windows.Forms.TextBox textBoxAttrTitle;
//        private System.Windows.Forms.GroupBox groupBox1;
//        private System.Windows.Forms.GroupBox groupBox2;
//        private System.Windows.Forms.TextBox textBoxAttrValue;
//        private System.Windows.Forms.GroupBox groupBox3;
//        private System.Windows.Forms.GroupBox groupBox4;
//        private System.Windows.Forms.ProgressBar progressBarCopyAttr;
//        private System.Windows.Forms.Button buttonAttrDelete;
//        private System.Windows.Forms.ProgressBar progressBarLoadComps;
//        private System.Windows.Forms.Button buttonReset;
//        private System.Windows.Forms.ComboBox attributeCombo;
//    }
//}