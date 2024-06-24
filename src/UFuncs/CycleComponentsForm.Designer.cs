namespace TSG_Library.UFuncs
{
    partial class CycleComponentsForm
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
            this.selectButton = new System.Windows.Forms.Button();
            this.selCompListBox = new System.Windows.Forms.ListBox();
            this.removeCompCheckBox = new System.Windows.Forms.CheckBox();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.returnButton = new System.Windows.Forms.Button();
            this.burnoutCompButton = new System.Windows.Forms.Button();
            this.checkBoxDefaultLayers = new System.Windows.Forms.CheckBox();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.buttonNonAssocDimsSymbols = new System.Windows.Forms.Button();
            this.buttonBrokenLinks = new System.Windows.Forms.Button();
            this.buttonOutputInfo = new System.Windows.Forms.Button();
            this.fourViewButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.materialSelectButton = new System.Windows.Forms.Button();
            this.materialComboBox = new System.Windows.Forms.ComboBox();
            this.checkBoxUpdateViews = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectButton
            // 
            this.selectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.selectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectButton.Location = new System.Drawing.Point(6, 19);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(138, 25);
            this.selectButton.TabIndex = 0;
            this.selectButton.Text = "Select Components";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // selCompListBox
            // 
            this.selCompListBox.FormattingEnabled = true;
            this.selCompListBox.Location = new System.Drawing.Point(11, 304);
            this.selCompListBox.Name = "selCompListBox";
            this.selCompListBox.ScrollAlwaysVisible = true;
            this.selCompListBox.Size = new System.Drawing.Size(150, 160);
            this.selCompListBox.Sorted = true;
            this.selCompListBox.TabIndex = 1;
            this.selCompListBox.DoubleClick += new System.EventHandler(this.SelCompListBox_DoubleClick);
            this.selCompListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.selCompListBox_KeyUp);
            // 
            // removeCompCheckBox
            // 
            this.removeCompCheckBox.AutoSize = true;
            this.removeCompCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeCompCheckBox.Location = new System.Drawing.Point(11, 475);
            this.removeCompCheckBox.Name = "removeCompCheckBox";
            this.removeCompCheckBox.Size = new System.Drawing.Size(117, 18);
            this.removeCompCheckBox.TabIndex = 2;
            this.removeCompCheckBox.Text = "Remove From List";
            this.removeCompCheckBox.UseVisualStyleBackColor = true;
            // 
            // prevButton
            // 
            this.prevButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.prevButton.Location = new System.Drawing.Point(11, 546);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(69, 23);
            this.prevButton.TabIndex = 3;
            this.prevButton.Text = "Previous";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nextButton.Location = new System.Drawing.Point(92, 546);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(69, 23);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // returnButton
            // 
            this.returnButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.returnButton.Location = new System.Drawing.Point(11, 575);
            this.returnButton.Name = "returnButton";
            this.returnButton.Size = new System.Drawing.Size(150, 23);
            this.returnButton.TabIndex = 5;
            this.returnButton.Text = "Return / Reset";
            this.returnButton.UseVisualStyleBackColor = true;
            this.returnButton.Click += new System.EventHandler(this.ReturnButton_Click);
            // 
            // burnoutCompButton
            // 
            this.burnoutCompButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.burnoutCompButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.burnoutCompButton.Location = new System.Drawing.Point(11, 264);
            this.burnoutCompButton.Name = "burnoutCompButton";
            this.burnoutCompButton.Size = new System.Drawing.Size(69, 25);
            this.burnoutCompButton.TabIndex = 6;
            this.burnoutCompButton.Text = "B.O. Comp";
            this.burnoutCompButton.UseVisualStyleBackColor = true;
            this.burnoutCompButton.Click += new System.EventHandler(this.BurnoutCompButton_Click);
            // 
            // checkBoxDefaultLayers
            // 
            this.checkBoxDefaultLayers.AutoSize = true;
            this.checkBoxDefaultLayers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxDefaultLayers.Location = new System.Drawing.Point(11, 498);
            this.checkBoxDefaultLayers.Name = "checkBoxDefaultLayers";
            this.checkBoxDefaultLayers.Size = new System.Drawing.Size(136, 18);
            this.checkBoxDefaultLayers.TabIndex = 9;
            this.checkBoxDefaultLayers.Text = "Default Layer Settings";
            this.checkBoxDefaultLayers.UseVisualStyleBackColor = true;
            // 
            // selectAllButton
            // 
            this.selectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.selectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAllButton.Location = new System.Drawing.Point(6, 50);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(138, 25);
            this.selectAllButton.TabIndex = 10;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // buttonNonAssocDimsSymbols
            // 
            this.buttonNonAssocDimsSymbols.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonNonAssocDimsSymbols.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNonAssocDimsSymbols.Location = new System.Drawing.Point(11, 233);
            this.buttonNonAssocDimsSymbols.Name = "buttonNonAssocDimsSymbols";
            this.buttonNonAssocDimsSymbols.Size = new System.Drawing.Size(152, 25);
            this.buttonNonAssocDimsSymbols.TabIndex = 11;
            this.buttonNonAssocDimsSymbols.Text = "Non Assoc Dimensions";
            this.buttonNonAssocDimsSymbols.UseVisualStyleBackColor = true;
            this.buttonNonAssocDimsSymbols.Click += new System.EventHandler(this.ButtonNonAssocDimsSymbols_Click);
            // 
            // buttonBrokenLinks
            // 
            this.buttonBrokenLinks.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBrokenLinks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrokenLinks.Location = new System.Drawing.Point(11, 202);
            this.buttonBrokenLinks.Name = "buttonBrokenLinks";
            this.buttonBrokenLinks.Size = new System.Drawing.Size(152, 25);
            this.buttonBrokenLinks.TabIndex = 12;
            this.buttonBrokenLinks.Text = "Broken Links";
            this.buttonBrokenLinks.UseVisualStyleBackColor = true;
            this.buttonBrokenLinks.Click += new System.EventHandler(this.ButtonBrokenLinks_Click);
            // 
            // buttonOutputInfo
            // 
            this.buttonOutputInfo.BackColor = System.Drawing.SystemColors.Window;
            this.buttonOutputInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOutputInfo.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOutputInfo.ForeColor = System.Drawing.Color.Blue;
            this.buttonOutputInfo.Location = new System.Drawing.Point(136, 475);
            this.buttonOutputInfo.Name = "buttonOutputInfo";
            this.buttonOutputInfo.Size = new System.Drawing.Size(25, 25);
            this.buttonOutputInfo.TabIndex = 13;
            this.buttonOutputInfo.Text = "i";
            this.buttonOutputInfo.UseVisualStyleBackColor = false;
            this.buttonOutputInfo.Click += new System.EventHandler(this.ButtonOutputInfo_Click);
            // 
            // fourViewButton
            // 
            this.fourViewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.fourViewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fourViewButton.Location = new System.Drawing.Point(94, 264);
            this.fourViewButton.Name = "fourViewButton";
            this.fourViewButton.Size = new System.Drawing.Size(69, 25);
            this.fourViewButton.TabIndex = 14;
            this.fourViewButton.Text = "4-View";
            this.fourViewButton.UseVisualStyleBackColor = true;
            this.fourViewButton.Click += new System.EventHandler(this.FourViewButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.selectAllButton);
            this.groupBox1.Controls.Add(this.selectButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 180);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selection Type";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.materialSelectButton);
            this.groupBox2.Controls.Add(this.materialComboBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(138, 85);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            // 
            // materialSelectButton
            // 
            this.materialSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.materialSelectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.materialSelectButton.Location = new System.Drawing.Point(6, 46);
            this.materialSelectButton.Name = "materialSelectButton";
            this.materialSelectButton.Size = new System.Drawing.Size(126, 25);
            this.materialSelectButton.TabIndex = 11;
            this.materialSelectButton.Text = "Select Material";
            this.materialSelectButton.UseVisualStyleBackColor = true;
            this.materialSelectButton.Click += new System.EventHandler(this.MaterialSelectButton_Click);
            // 
            // materialComboBox
            // 
            this.materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.materialComboBox.FormattingEnabled = true;
            this.materialComboBox.Location = new System.Drawing.Point(6, 19);
            this.materialComboBox.Name = "materialComboBox";
            this.materialComboBox.Size = new System.Drawing.Size(126, 21);
            this.materialComboBox.TabIndex = 16;
            this.materialComboBox.SelectedIndexChanged += new System.EventHandler(this.MaterialComboBox_SelectedIndexChanged);
            // 
            // checkBoxUpdateViews
            // 
            this.checkBoxUpdateViews.AutoSize = true;
            this.checkBoxUpdateViews.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxUpdateViews.Location = new System.Drawing.Point(11, 522);
            this.checkBoxUpdateViews.Name = "checkBoxUpdateViews";
            this.checkBoxUpdateViews.Size = new System.Drawing.Size(98, 18);
            this.checkBoxUpdateViews.TabIndex = 16;
            this.checkBoxUpdateViews.Text = "Update Views";
            this.checkBoxUpdateViews.UseVisualStyleBackColor = true;
            // 
            // CycleComponentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(171, 605);
            this.Controls.Add(this.checkBoxUpdateViews);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fourViewButton);
            this.Controls.Add(this.buttonOutputInfo);
            this.Controls.Add(this.buttonBrokenLinks);
            this.Controls.Add(this.buttonNonAssocDimsSymbols);
            this.Controls.Add(this.checkBoxDefaultLayers);
            this.Controls.Add(this.burnoutCompButton);
            this.Controls.Add(this.returnButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.removeCompCheckBox);
            this.Controls.Add(this.selCompListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CycleComponentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm1_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.ListBox selCompListBox;
        private System.Windows.Forms.CheckBox removeCompCheckBox;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button returnButton;
        private System.Windows.Forms.Button burnoutCompButton;
        private System.Windows.Forms.CheckBox checkBoxDefaultLayers;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Button buttonNonAssocDimsSymbols;
        private System.Windows.Forms.Button buttonBrokenLinks;
        private System.Windows.Forms.Button buttonOutputInfo;
        private System.Windows.Forms.Button fourViewButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button materialSelectButton;
        private System.Windows.Forms.ComboBox materialComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxUpdateViews;
    }
}