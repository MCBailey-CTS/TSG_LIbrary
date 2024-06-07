namespace TSG_Library.UFuncs
{
    partial class AddFastenersForm1
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
            this.btnViewWcs = new System.Windows.Forms.Button();
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.chkGrid = new System.Windows.Forms.CheckBox();
            this.cmbGridSpacing = new System.Windows.Forms.ComboBox();
            this.grpFastenerType = new System.Windows.Forms.GroupBox();
            this.rdoTypeScrew = new System.Windows.Forms.RadioButton();
            this.rdoTypeDowel = new System.Windows.Forms.RadioButton();
            this.rdoTypeJack = new System.Windows.Forms.RadioButton();
            this.btnSelectComponent = new System.Windows.Forms.Button();
            this.lblUnknown = new System.Windows.Forms.Label();
            this.chkCycleAdd = new System.Windows.Forms.CheckBox();
            this.mnuStrMainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllFastenersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceButtonsOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUnits = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu2x = new System.Windows.Forms.ToolStripMenuItem();
            this.chkReverseCycleAdd = new System.Windows.Forms.CheckBox();
            this.chkSubstitute = new System.Windows.Forms.CheckBox();
            this.cmbReferenceSet = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnWireTaps = new System.Windows.Forms.Button();
            this.btnChangeRefSet = new System.Windows.Forms.Button();
            this.listBoxSelection = new System.Windows.Forms.ListBox();
            this.btnPlanView = new System.Windows.Forms.Button();
            this.btnOrigin = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.cmbPreferred = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpGrid.SuspendLayout();
            this.grpFastenerType.SuspendLayout();
            this.mnuStrMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewWcs
            // 
            this.btnViewWcs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnViewWcs.Location = new System.Drawing.Point(6, 19);
            this.btnViewWcs.Name = "btnViewWcs";
            this.btnViewWcs.Size = new System.Drawing.Size(84, 20);
            this.btnViewWcs.TabIndex = 1;
            this.btnViewWcs.Text = "View WCS";
            this.btnViewWcs.UseVisualStyleBackColor = true;
            this.btnViewWcs.Click += new System.EventHandler(this.BtnViewWcs_Click);
            // 
            // grpGrid
            // 
            this.grpGrid.Controls.Add(this.chkGrid);
            this.grpGrid.Controls.Add(this.cmbGridSpacing);
            this.grpGrid.Controls.Add(this.btnViewWcs);
            this.grpGrid.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpGrid.Location = new System.Drawing.Point(138, 163);
            this.grpGrid.Name = "grpGrid";
            this.grpGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.grpGrid.Size = new System.Drawing.Size(94, 99);
            this.grpGrid.TabIndex = 9;
            this.grpGrid.TabStop = false;
            this.grpGrid.Text = "Work Plane";
            // 
            // chkGrid
            // 
            this.chkGrid.AutoSize = true;
            this.chkGrid.Location = new System.Drawing.Point(30, 45);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(45, 17);
            this.chkGrid.TabIndex = 4;
            this.chkGrid.Text = "Grid";
            this.chkGrid.UseVisualStyleBackColor = true;
            this.chkGrid.CheckedChanged += new System.EventHandler(this.ChkGrid_CheckedChanged);
            // 
            // cmbGridSpacing
            // 
            this.cmbGridSpacing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGridSpacing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbGridSpacing.FormattingEnabled = true;
            this.cmbGridSpacing.Location = new System.Drawing.Point(3, 68);
            this.cmbGridSpacing.Name = "cmbGridSpacing";
            this.cmbGridSpacing.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbGridSpacing.Size = new System.Drawing.Size(87, 21);
            this.cmbGridSpacing.TabIndex = 3;
            this.cmbGridSpacing.SelectedIndexChanged += new System.EventHandler(this.CmbGridSpacing_SelectedIndexChanged);
            // 
            // grpFastenerType
            // 
            this.grpFastenerType.Controls.Add(this.rdoTypeScrew);
            this.grpFastenerType.Controls.Add(this.rdoTypeDowel);
            this.grpFastenerType.Controls.Add(this.rdoTypeJack);
            this.grpFastenerType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpFastenerType.ImeMode = System.Windows.Forms.ImeMode.On;
            this.grpFastenerType.Location = new System.Drawing.Point(7, 27);
            this.grpFastenerType.Name = "grpFastenerType";
            this.grpFastenerType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.grpFastenerType.Size = new System.Drawing.Size(225, 40);
            this.grpFastenerType.TabIndex = 1;
            this.grpFastenerType.TabStop = false;
            this.grpFastenerType.Text = "Fastener Type";
            // 
            // rdoTypeScrew
            // 
            this.rdoTypeScrew.AutoSize = true;
            this.rdoTypeScrew.Checked = true;
            this.rdoTypeScrew.Location = new System.Drawing.Point(6, 15);
            this.rdoTypeScrew.Name = "rdoTypeScrew";
            this.rdoTypeScrew.Size = new System.Drawing.Size(60, 17);
            this.rdoTypeScrew.TabIndex = 0;
            this.rdoTypeScrew.TabStop = true;
            this.rdoTypeScrew.Text = "Screws";
            this.rdoTypeScrew.UseVisualStyleBackColor = true;
            this.rdoTypeScrew.CheckedChanged += new System.EventHandler(this.RdoFastener_CheckedChanged);
            // 
            // rdoTypeDowel
            // 
            this.rdoTypeDowel.AutoSize = true;
            this.rdoTypeDowel.Location = new System.Drawing.Point(72, 15);
            this.rdoTypeDowel.Name = "rdoTypeDowel";
            this.rdoTypeDowel.Size = new System.Drawing.Size(60, 17);
            this.rdoTypeDowel.TabIndex = 1;
            this.rdoTypeDowel.Text = "Dowels";
            this.rdoTypeDowel.UseVisualStyleBackColor = true;
            this.rdoTypeDowel.CheckedChanged += new System.EventHandler(this.RdoFastener_CheckedChanged);
            // 
            // rdoTypeJack
            // 
            this.rdoTypeJack.AutoSize = true;
            this.rdoTypeJack.Location = new System.Drawing.Point(138, 15);
            this.rdoTypeJack.Name = "rdoTypeJack";
            this.rdoTypeJack.Size = new System.Drawing.Size(86, 17);
            this.rdoTypeJack.TabIndex = 2;
            this.rdoTypeJack.Text = "Jack Screws";
            this.rdoTypeJack.UseVisualStyleBackColor = true;
            this.rdoTypeJack.CheckedChanged += new System.EventHandler(this.RdoFastener_CheckedChanged);
            // 
            // btnSelectComponent
            // 
            this.btnSelectComponent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectComponent.Location = new System.Drawing.Point(7, 73);
            this.btnSelectComponent.Name = "btnSelectComponent";
            this.btnSelectComponent.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSelectComponent.Size = new System.Drawing.Size(225, 23);
            this.btnSelectComponent.TabIndex = 2;
            this.btnSelectComponent.Text = "Select Component";
            this.btnSelectComponent.UseVisualStyleBackColor = true;
            this.btnSelectComponent.Click += new System.EventHandler(this.ButtonSelectComponent_Click);
            // 
            // lblUnknown
            // 
            this.lblUnknown.AutoSize = true;
            this.lblUnknown.Location = new System.Drawing.Point(72, 738);
            this.lblUnknown.Name = "lblUnknown";
            this.lblUnknown.Size = new System.Drawing.Size(102, 13);
            this.lblUnknown.TabIndex = 19;
            this.lblUnknown.Text = "Copyright CTS 2014";
            // 
            // chkCycleAdd
            // 
            this.chkCycleAdd.AutoSize = true;
            this.chkCycleAdd.Location = new System.Drawing.Point(135, 268);
            this.chkCycleAdd.Name = "chkCycleAdd";
            this.chkCycleAdd.Size = new System.Drawing.Size(74, 17);
            this.chkCycleAdd.TabIndex = 23;
            this.chkCycleAdd.Text = "Cycle Add";
            this.chkCycleAdd.UseVisualStyleBackColor = true;
            this.chkCycleAdd.CheckedChanged += new System.EventHandler(this.ChkCycleAdd_CheckedChanged);
            this.chkCycleAdd.Click += new System.EventHandler(this.ChkSubstitute_Click);
            // 
            // mnuStrMainMenu
            // 
            this.mnuStrMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemUnits,
            this.toolStripMenuItem1,
            this.mnu2x});
            this.mnuStrMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuStrMainMenu.Name = "mnuStrMainMenu";
            this.mnuStrMainMenu.Size = new System.Drawing.Size(238, 24);
            this.mnuStrMainMenu.TabIndex = 25;
            this.mnuStrMainMenu.Text = "Waving";
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeAllFastenersMenuItem,
            this.forceButtonsOnToolStripMenuItem});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(37, 20);
            this.menuItemFile.Text = "File";
            // 
            // closeAllFastenersMenuItem
            // 
            this.closeAllFastenersMenuItem.Name = "closeAllFastenersMenuItem";
            this.closeAllFastenersMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeAllFastenersMenuItem.Text = "Close All Fasteners";
            this.closeAllFastenersMenuItem.Click += new System.EventHandler(this.CloseAllFastenersMenuItem_Click);
            // 
            // forceButtonsOnToolStripMenuItem
            // 
            this.forceButtonsOnToolStripMenuItem.Name = "forceButtonsOnToolStripMenuItem";
            this.forceButtonsOnToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.forceButtonsOnToolStripMenuItem.Text = "Force Buttons On";
            this.forceButtonsOnToolStripMenuItem.Click += new System.EventHandler(this.ForceButtonsOnToolStripMenuItem_Click);
            // 
            // menuItemUnits
            // 
            this.menuItemUnits.AutoSize = false;
            this.menuItemUnits.Name = "menuItemUnits";
            this.menuItemUnits.Size = new System.Drawing.Size(46, 20);
            this.menuItemUnits.Text = "Units";
            this.menuItemUnits.Click += new System.EventHandler(this.MenuUnits_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(71, 20);
            this.toolStripMenuItem1.Text = "Wave Out";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.StripWaveOut_Click);
            // 
            // mnu2x
            // 
            this.mnu2x.Name = "mnu2x";
            this.mnu2x.Size = new System.Drawing.Size(31, 20);
            this.mnu2x.Text = "2x";
            this.mnu2x.Click += new System.EventHandler(this.mnu2x_Click);
            // 
            // chkReverseCycleAdd
            // 
            this.chkReverseCycleAdd.AutoSize = true;
            this.chkReverseCycleAdd.Location = new System.Drawing.Point(135, 291);
            this.chkReverseCycleAdd.Name = "chkReverseCycleAdd";
            this.chkReverseCycleAdd.Size = new System.Drawing.Size(88, 17);
            this.chkReverseCycleAdd.TabIndex = 26;
            this.chkReverseCycleAdd.Text = "Reverse Add";
            this.chkReverseCycleAdd.UseVisualStyleBackColor = true;
            this.chkReverseCycleAdd.CheckedChanged += new System.EventHandler(this.ChkCycleAdd_CheckedChanged);
            this.chkReverseCycleAdd.Click += new System.EventHandler(this.ChkSubstitute_Click);
            // 
            // chkSubstitute
            // 
            this.chkSubstitute.AutoSize = true;
            this.chkSubstitute.Location = new System.Drawing.Point(135, 314);
            this.chkSubstitute.Name = "chkSubstitute";
            this.chkSubstitute.Size = new System.Drawing.Size(73, 17);
            this.chkSubstitute.TabIndex = 28;
            this.chkSubstitute.Text = "Substitute";
            this.chkSubstitute.UseVisualStyleBackColor = true;
            this.chkSubstitute.Click += new System.EventHandler(this.ChkSubstitute_Click);
            // 
            // cmbReferenceSet
            // 
            this.cmbReferenceSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReferenceSet.Enabled = false;
            this.cmbReferenceSet.FormattingEnabled = true;
            this.cmbReferenceSet.Location = new System.Drawing.Point(138, 103);
            this.cmbReferenceSet.Name = "cmbReferenceSet";
            this.cmbReferenceSet.Size = new System.Drawing.Size(94, 21);
            this.cmbReferenceSet.TabIndex = 29;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(3, 366);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(225, 23);
            this.btnOk.TabIndex = 33;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOtherOk_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(3, 337);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(112, 23);
            this.btnReset.TabIndex = 36;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnWireTaps
            // 
            this.btnWireTaps.Location = new System.Drawing.Point(138, 130);
            this.btnWireTaps.Name = "btnWireTaps";
            this.btnWireTaps.Size = new System.Drawing.Size(94, 23);
            this.btnWireTaps.TabIndex = 40;
            this.btnWireTaps.Text = "Wire Taps";
            this.btnWireTaps.UseVisualStyleBackColor = true;
            this.btnWireTaps.Click += new System.EventHandler(this.BtnWireTaps_Click);
            // 
            // btnChangeRefSet
            // 
            this.btnChangeRefSet.Location = new System.Drawing.Point(7, 239);
            this.btnChangeRefSet.Name = "btnChangeRefSet";
            this.btnChangeRefSet.Size = new System.Drawing.Size(119, 23);
            this.btnChangeRefSet.TabIndex = 48;
            this.btnChangeRefSet.Text = "Change Ref Set";
            this.btnChangeRefSet.UseVisualStyleBackColor = true;
            this.btnChangeRefSet.Click += new System.EventHandler(this.BtnChangeRefSet_Click);
            // 
            // listBoxSelection
            // 
            this.listBoxSelection.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.listBoxSelection.Enabled = false;
            this.listBoxSelection.FormattingEnabled = true;
            this.listBoxSelection.Location = new System.Drawing.Point(7, 103);
            this.listBoxSelection.Name = "listBoxSelection";
            this.listBoxSelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBoxSelection.Size = new System.Drawing.Size(120, 134);
            this.listBoxSelection.TabIndex = 49;
            this.listBoxSelection.SelectedIndexChanged += new System.EventHandler(this.ListBoxSelection_SelectedIndexChanged);
            // 
            // btnPlanView
            // 
            this.btnPlanView.Location = new System.Drawing.Point(121, 337);
            this.btnPlanView.Name = "btnPlanView";
            this.btnPlanView.Size = new System.Drawing.Size(107, 23);
            this.btnPlanView.TabIndex = 53;
            this.btnPlanView.Text = "Plan View";
            this.btnPlanView.UseVisualStyleBackColor = true;
            this.btnPlanView.Click += new System.EventHandler(this.BtnPlanView_Click);
            // 
            // btnOrigin
            // 
            this.btnOrigin.Location = new System.Drawing.Point(3, 310);
            this.btnOrigin.Name = "btnOrigin";
            this.btnOrigin.Size = new System.Drawing.Size(112, 23);
            this.btnOrigin.TabIndex = 55;
            this.btnOrigin.Text = "Block Origin";
            this.btnOrigin.UseVisualStyleBackColor = true;
            this.btnOrigin.Click += new System.EventHandler(this.BtnOrigin_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(12, 399);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(0, 13);
            this.lblTime.TabIndex = 56;
            // 
            // cmbPreferred
            // 
            this.cmbPreferred.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreferred.FormattingEnabled = true;
            this.cmbPreferred.Items.AddRange(new object[] {
            "None",
            "TSG",
            "GM"});
            this.cmbPreferred.Location = new System.Drawing.Point(7, 283);
            this.cmbPreferred.Name = "cmbPreferred";
            this.cmbPreferred.Size = new System.Drawing.Size(119, 21);
            this.cmbPreferred.TabIndex = 57;
            this.cmbPreferred.SelectedIndexChanged += new System.EventHandler(this.cmbPreferred_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 58;
            this.label1.Text = "Preferred:";
            // 
            // AddFastenersForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 399);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbPreferred);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.btnOrigin);
            this.Controls.Add(this.btnPlanView);
            this.Controls.Add(this.listBoxSelection);
            this.Controls.Add(this.btnChangeRefSet);
            this.Controls.Add(this.btnWireTaps);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cmbReferenceSet);
            this.Controls.Add(this.chkSubstitute);
            this.Controls.Add(this.chkReverseCycleAdd);
            this.Controls.Add(this.chkCycleAdd);
            this.Controls.Add(this.lblUnknown);
            this.Controls.Add(this.btnSelectComponent);
            this.Controls.Add(this.grpFastenerType);
            this.Controls.Add(this.grpGrid);
            this.Controls.Add(this.mnuStrMainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 250);
            this.MainMenuStrip = this.mnuStrMainMenu;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddFastenersForm1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpGrid.ResumeLayout(false);
            this.grpGrid.PerformLayout();
            this.grpFastenerType.ResumeLayout(false);
            this.grpFastenerType.PerformLayout();
            this.mnuStrMainMenu.ResumeLayout(false);
            this.mnuStrMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnViewWcs;
        public System.Windows.Forms.GroupBox grpGrid;
        public System.Windows.Forms.ComboBox cmbGridSpacing;
        public System.Windows.Forms.GroupBox grpFastenerType;
        public System.Windows.Forms.RadioButton rdoTypeScrew;
        public System.Windows.Forms.RadioButton rdoTypeDowel;
        public System.Windows.Forms.RadioButton rdoTypeJack;
        public System.Windows.Forms.Button btnSelectComponent;
        public System.Windows.Forms.Label lblUnknown;
        public System.Windows.Forms.CheckBox chkCycleAdd;
        public System.Windows.Forms.MenuStrip mnuStrMainMenu;
        public System.Windows.Forms.ToolStripMenuItem menuItemFile;
        public System.Windows.Forms.ToolStripMenuItem menuItemUnits;
        public System.Windows.Forms.CheckBox chkReverseCycleAdd;
        public System.Windows.Forms.CheckBox chkSubstitute;
        public System.Windows.Forms.ComboBox cmbReferenceSet;
        public System.Windows.Forms.CheckBox chkGrid;
        public System.Windows.Forms.Button btnOk;
        public System.Windows.Forms.ToolStripMenuItem mnu2x;
        public System.Windows.Forms.Button btnReset;
        public System.Windows.Forms.Button btnWireTaps;
        public System.Windows.Forms.Button btnChangeRefSet;
        public System.Windows.Forms.ListBox listBoxSelection;
        public System.Windows.Forms.Button btnPlanView;
        public System.Windows.Forms.Button btnOrigin;
        public System.Windows.Forms.Label lblTime;
        public System.Windows.Forms.ToolStripMenuItem forceButtonsOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllFastenersMenuItem;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ComboBox cmbPreferred;
        private System.Windows.Forms.Label label1;
    }
}