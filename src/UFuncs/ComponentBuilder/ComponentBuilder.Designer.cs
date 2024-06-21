namespace TSG_Library.UFuncs
{
    partial class ComponentBuilder
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
            this.textBoxDetailNumber = new System.Windows.Forms.TextBox();
            this.comboBoxGrid = new System.Windows.Forms.ComboBox();
            this.comboBoxCompName = new System.Windows.Forms.ComboBox();
            this.groupBoxColor = new System.Windows.Forms.GroupBox();
            this.checkBoxUpperComp = new System.Windows.Forms.CheckBox();
            this.buttonDarkDullBlue = new System.Windows.Forms.Button();
            this.buttonPurple = new System.Windows.Forms.Button();
            this.buttonDarkWeakRed = new System.Windows.Forms.Button();
            this.buttonMedAzureBlue = new System.Windows.Forms.Button();
            this.buttonDarkWeakMagenta = new System.Windows.Forms.Button();
            this.buttonAquamarine = new System.Windows.Forms.Button();
            this.buttonDarkDullGreen = new System.Windows.Forms.Button();
            this.buttonObscureDullGreen = new System.Windows.Forms.Button();
            this.changeColorCheckBox = new System.Windows.Forms.CheckBox();
            this.updateSessionButton = new System.Windows.Forms.Button();
            this.groupBoxEditAssembly = new System.Windows.Forms.GroupBox();
            this.buttonLwrRetAssm = new System.Windows.Forms.Button();
            this.buttonAutoLwr = new System.Windows.Forms.Button();
            this.buttonAutoUpr = new System.Windows.Forms.Button();
            this.buttonUprRetAssm = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.groupBoxWorkPlane = new System.Windows.Forms.GroupBox();
            this.buttonViewWcs = new System.Windows.Forms.Button();
            this.listBoxMaterial = new System.Windows.Forms.ListBox();
            this.textBoxUserMaterial = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonEditBlock = new System.Windows.Forms.Button();
            this.buttonEditConstruction = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonEndEditConstruction = new System.Windows.Forms.Button();
            this.groupBoxBurnSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxGrind = new System.Windows.Forms.CheckBox();
            this.checkBoxBurnout = new System.Windows.Forms.CheckBox();
            this.checkBoxBurnDirZ = new System.Windows.Forms.CheckBox();
            this.checkBoxBurnDirY = new System.Windows.Forms.CheckBox();
            this.comboBoxTolerance = new System.Windows.Forms.ComboBox();
            this.checkBoxBurnDirX = new System.Windows.Forms.CheckBox();
            this.chk4Digits = new System.Windows.Forms.CheckBox();
            this.chkAnyAssembly = new System.Windows.Forms.CheckBox();
            this.btnMakeUnique = new System.Windows.Forms.Button();
            this.groupBoxColor.SuspendLayout();
            this.groupBoxEditAssembly.SuspendLayout();
            this.groupBoxWorkPlane.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxBurnSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDetailNumber
            // 
            this.textBoxDetailNumber.Location = new System.Drawing.Point(12, 62);
            this.textBoxDetailNumber.Name = "textBoxDetailNumber";
            this.textBoxDetailNumber.Size = new System.Drawing.Size(100, 20);
            this.textBoxDetailNumber.TabIndex = 1;
            this.textBoxDetailNumber.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxDetailNumber_MouseClick);
            this.textBoxDetailNumber.TextChanged += new System.EventHandler(this.TextBoxDetailNumber_TextChanged);
            // 
            // comboBoxGrid
            // 
            this.comboBoxGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGrid.FormattingEnabled = true;
            this.comboBoxGrid.Location = new System.Drawing.Point(9, 19);
            this.comboBoxGrid.Name = "comboBoxGrid";
            this.comboBoxGrid.Size = new System.Drawing.Size(84, 21);
            this.comboBoxGrid.TabIndex = 0;
            this.comboBoxGrid.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGrid_SelectedIndexChanged);
            // 
            // comboBoxCompName
            // 
            this.comboBoxCompName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCompName.FormattingEnabled = true;
            this.comboBoxCompName.Location = new System.Drawing.Point(12, 88);
            this.comboBoxCompName.Name = "comboBoxCompName";
            this.comboBoxCompName.Size = new System.Drawing.Size(100, 21);
            this.comboBoxCompName.TabIndex = 2;
            this.comboBoxCompName.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCompName_SelectedIndexChanged);
            // 
            // groupBoxColor
            // 
            this.groupBoxColor.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxColor.Controls.Add(this.checkBoxUpperComp);
            this.groupBoxColor.Controls.Add(this.buttonDarkDullBlue);
            this.groupBoxColor.Controls.Add(this.buttonPurple);
            this.groupBoxColor.Controls.Add(this.buttonDarkWeakRed);
            this.groupBoxColor.Controls.Add(this.buttonMedAzureBlue);
            this.groupBoxColor.Controls.Add(this.buttonDarkWeakMagenta);
            this.groupBoxColor.Controls.Add(this.buttonAquamarine);
            this.groupBoxColor.Controls.Add(this.buttonDarkDullGreen);
            this.groupBoxColor.Controls.Add(this.buttonObscureDullGreen);
            this.groupBoxColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxColor.Location = new System.Drawing.Point(118, 62);
            this.groupBoxColor.Name = "groupBoxColor";
            this.groupBoxColor.Size = new System.Drawing.Size(86, 150);
            this.groupBoxColor.TabIndex = 5;
            this.groupBoxColor.TabStop = false;
            this.groupBoxColor.Text = "Color";
            // 
            // checkBoxUpperComp
            // 
            this.checkBoxUpperComp.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxUpperComp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxUpperComp.Location = new System.Drawing.Point(6, 121);
            this.checkBoxUpperComp.Name = "checkBoxUpperComp";
            this.checkBoxUpperComp.Size = new System.Drawing.Size(72, 20);
            this.checkBoxUpperComp.TabIndex = 16;
            this.checkBoxUpperComp.Text = "Upper";
            this.checkBoxUpperComp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxUpperComp.UseVisualStyleBackColor = true;
            // 
            // buttonDarkDullBlue
            // 
            this.buttonDarkDullBlue.BackColor = System.Drawing.Color.Blue;
            this.buttonDarkDullBlue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDarkDullBlue.Location = new System.Drawing.Point(46, 69);
            this.buttonDarkDullBlue.Name = "buttonDarkDullBlue";
            this.buttonDarkDullBlue.Size = new System.Drawing.Size(34, 20);
            this.buttonDarkDullBlue.TabIndex = 11;
            this.buttonDarkDullBlue.UseVisualStyleBackColor = false;
            this.buttonDarkDullBlue.Click += new System.EventHandler(this.ButtonDarkDullBlue_Click);
            // 
            // buttonPurple
            // 
            this.buttonPurple.BackColor = System.Drawing.Color.MediumOrchid;
            this.buttonPurple.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPurple.Location = new System.Drawing.Point(46, 43);
            this.buttonPurple.Name = "buttonPurple";
            this.buttonPurple.Size = new System.Drawing.Size(34, 20);
            this.buttonPurple.TabIndex = 10;
            this.buttonPurple.UseVisualStyleBackColor = false;
            this.buttonPurple.Click += new System.EventHandler(this.ButtonPurple_Click);
            // 
            // buttonDarkWeakRed
            // 
            this.buttonDarkWeakRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonDarkWeakRed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDarkWeakRed.Location = new System.Drawing.Point(46, 95);
            this.buttonDarkWeakRed.Name = "buttonDarkWeakRed";
            this.buttonDarkWeakRed.Size = new System.Drawing.Size(34, 20);
            this.buttonDarkWeakRed.TabIndex = 8;
            this.buttonDarkWeakRed.UseVisualStyleBackColor = false;
            this.buttonDarkWeakRed.Click += new System.EventHandler(this.ButtonDarkWeakRed_Click);
            // 
            // buttonMedAzureBlue
            // 
            this.buttonMedAzureBlue.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonMedAzureBlue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonMedAzureBlue.Location = new System.Drawing.Point(6, 43);
            this.buttonMedAzureBlue.Name = "buttonMedAzureBlue";
            this.buttonMedAzureBlue.Size = new System.Drawing.Size(34, 20);
            this.buttonMedAzureBlue.TabIndex = 4;
            this.buttonMedAzureBlue.UseVisualStyleBackColor = false;
            this.buttonMedAzureBlue.Click += new System.EventHandler(this.ButtonMedAzureBlue_Click);
            // 
            // buttonDarkWeakMagenta
            // 
            this.buttonDarkWeakMagenta.BackColor = System.Drawing.Color.Purple;
            this.buttonDarkWeakMagenta.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDarkWeakMagenta.Location = new System.Drawing.Point(6, 69);
            this.buttonDarkWeakMagenta.Name = "buttonDarkWeakMagenta";
            this.buttonDarkWeakMagenta.Size = new System.Drawing.Size(34, 20);
            this.buttonDarkWeakMagenta.TabIndex = 9;
            this.buttonDarkWeakMagenta.UseVisualStyleBackColor = false;
            this.buttonDarkWeakMagenta.Click += new System.EventHandler(this.ButtonDarkWeakMagenta_Click);
            // 
            // buttonAquamarine
            // 
            this.buttonAquamarine.BackColor = System.Drawing.Color.MediumAquamarine;
            this.buttonAquamarine.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonAquamarine.Location = new System.Drawing.Point(6, 95);
            this.buttonAquamarine.Name = "buttonAquamarine";
            this.buttonAquamarine.Size = new System.Drawing.Size(34, 20);
            this.buttonAquamarine.TabIndex = 3;
            this.buttonAquamarine.UseVisualStyleBackColor = false;
            this.buttonAquamarine.Click += new System.EventHandler(this.ButtonAquamarine_Click);
            // 
            // buttonDarkDullGreen
            // 
            this.buttonDarkDullGreen.BackColor = System.Drawing.Color.OliveDrab;
            this.buttonDarkDullGreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDarkDullGreen.Location = new System.Drawing.Point(46, 17);
            this.buttonDarkDullGreen.Name = "buttonDarkDullGreen";
            this.buttonDarkDullGreen.Size = new System.Drawing.Size(34, 20);
            this.buttonDarkDullGreen.TabIndex = 5;
            this.buttonDarkDullGreen.UseVisualStyleBackColor = false;
            this.buttonDarkDullGreen.Click += new System.EventHandler(this.ButtonDarkDullGreen_Click);
            // 
            // buttonObscureDullGreen
            // 
            this.buttonObscureDullGreen.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.buttonObscureDullGreen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonObscureDullGreen.Location = new System.Drawing.Point(6, 17);
            this.buttonObscureDullGreen.Name = "buttonObscureDullGreen";
            this.buttonObscureDullGreen.Size = new System.Drawing.Size(34, 20);
            this.buttonObscureDullGreen.TabIndex = 2;
            this.buttonObscureDullGreen.UseVisualStyleBackColor = false;
            this.buttonObscureDullGreen.Click += new System.EventHandler(this.ButtonObscureDullGreen_Click);
            // 
            // changeColorCheckBox
            // 
            this.changeColorCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.changeColorCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.changeColorCheckBox.Location = new System.Drawing.Point(118, 218);
            this.changeColorCheckBox.Name = "changeColorCheckBox";
            this.changeColorCheckBox.Size = new System.Drawing.Size(86, 23);
            this.changeColorCheckBox.TabIndex = 15;
            this.changeColorCheckBox.Text = "Change Color";
            this.changeColorCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.changeColorCheckBox.UseVisualStyleBackColor = true;
            this.changeColorCheckBox.CheckedChanged += new System.EventHandler(this.ChangeColorCheckBox_CheckedChanged);
            // 
            // updateSessionButton
            // 
            this.updateSessionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.updateSessionButton.Location = new System.Drawing.Point(12, 13);
            this.updateSessionButton.Name = "updateSessionButton";
            this.updateSessionButton.Size = new System.Drawing.Size(192, 20);
            this.updateSessionButton.TabIndex = 0;
            this.updateSessionButton.Text = "Update Session";
            this.updateSessionButton.UseVisualStyleBackColor = true;
            this.updateSessionButton.Click += new System.EventHandler(this.UpdateSessionButton_Click);
            // 
            // groupBoxEditAssembly
            // 
            this.groupBoxEditAssembly.Controls.Add(this.btnMakeUnique);
            this.groupBoxEditAssembly.Controls.Add(this.buttonLwrRetAssm);
            this.groupBoxEditAssembly.Controls.Add(this.buttonAutoLwr);
            this.groupBoxEditAssembly.Controls.Add(this.buttonAutoUpr);
            this.groupBoxEditAssembly.Controls.Add(this.buttonUprRetAssm);
            this.groupBoxEditAssembly.Controls.Add(this.copyButton);
            this.groupBoxEditAssembly.Controls.Add(this.saveAsButton);
            this.groupBoxEditAssembly.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxEditAssembly.Location = new System.Drawing.Point(15, 491);
            this.groupBoxEditAssembly.Name = "groupBoxEditAssembly";
            this.groupBoxEditAssembly.Size = new System.Drawing.Size(190, 156);
            this.groupBoxEditAssembly.TabIndex = 8;
            this.groupBoxEditAssembly.TabStop = false;
            this.groupBoxEditAssembly.Text = "Edit Assembly";
            // 
            // buttonLwrRetAssm
            // 
            this.buttonLwrRetAssm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLwrRetAssm.Location = new System.Drawing.Point(98, 45);
            this.buttonLwrRetAssm.Name = "buttonLwrRetAssm";
            this.buttonLwrRetAssm.Size = new System.Drawing.Size(84, 20);
            this.buttonLwrRetAssm.TabIndex = 6;
            this.buttonLwrRetAssm.Text = "Lwr Ret Assm";
            this.buttonLwrRetAssm.UseVisualStyleBackColor = true;
            this.buttonLwrRetAssm.Click += new System.EventHandler(this.ButtonLwrRetAssm_Click);
            // 
            // buttonAutoLwr
            // 
            this.buttonAutoLwr.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAutoLwr.Location = new System.Drawing.Point(8, 45);
            this.buttonAutoLwr.Name = "buttonAutoLwr";
            this.buttonAutoLwr.Size = new System.Drawing.Size(84, 20);
            this.buttonAutoLwr.TabIndex = 3;
            this.buttonAutoLwr.Text = "Auto Lower";
            this.buttonAutoLwr.UseVisualStyleBackColor = true;
            this.buttonAutoLwr.Click += new System.EventHandler(this.ButtonAutoLwr_Click);
            // 
            // buttonAutoUpr
            // 
            this.buttonAutoUpr.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAutoUpr.Location = new System.Drawing.Point(8, 19);
            this.buttonAutoUpr.Name = "buttonAutoUpr";
            this.buttonAutoUpr.Size = new System.Drawing.Size(84, 20);
            this.buttonAutoUpr.TabIndex = 4;
            this.buttonAutoUpr.Text = "Auto Upper";
            this.buttonAutoUpr.UseVisualStyleBackColor = true;
            this.buttonAutoUpr.Click += new System.EventHandler(this.ButtonAutoUpr_Click);
            // 
            // buttonUprRetAssm
            // 
            this.buttonUprRetAssm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonUprRetAssm.Location = new System.Drawing.Point(98, 19);
            this.buttonUprRetAssm.Name = "buttonUprRetAssm";
            this.buttonUprRetAssm.Size = new System.Drawing.Size(84, 20);
            this.buttonUprRetAssm.TabIndex = 5;
            this.buttonUprRetAssm.Text = "Upr Ret Assm";
            this.buttonUprRetAssm.UseVisualStyleBackColor = true;
            this.buttonUprRetAssm.Click += new System.EventHandler(this.ButtonUprRetAssm_Click);
            // 
            // copyButton
            // 
            this.copyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.copyButton.Location = new System.Drawing.Point(7, 97);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(174, 20);
            this.copyButton.TabIndex = 0;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveAsButton.Location = new System.Drawing.Point(7, 123);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(174, 20);
            this.saveAsButton.TabIndex = 2;
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.SaveAsButton_Click);
            // 
            // groupBoxWorkPlane
            // 
            this.groupBoxWorkPlane.Controls.Add(this.buttonViewWcs);
            this.groupBoxWorkPlane.Controls.Add(this.comboBoxGrid);
            this.groupBoxWorkPlane.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxWorkPlane.Location = new System.Drawing.Point(12, 353);
            this.groupBoxWorkPlane.Name = "groupBoxWorkPlane";
            this.groupBoxWorkPlane.Size = new System.Drawing.Size(192, 50);
            this.groupBoxWorkPlane.TabIndex = 6;
            this.groupBoxWorkPlane.TabStop = false;
            this.groupBoxWorkPlane.Text = "Work Plane";
            // 
            // buttonViewWcs
            // 
            this.buttonViewWcs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonViewWcs.Location = new System.Drawing.Point(99, 19);
            this.buttonViewWcs.Name = "buttonViewWcs";
            this.buttonViewWcs.Size = new System.Drawing.Size(84, 21);
            this.buttonViewWcs.TabIndex = 1;
            this.buttonViewWcs.Text = "View WCS";
            this.buttonViewWcs.UseVisualStyleBackColor = true;
            this.buttonViewWcs.Click += new System.EventHandler(this.ButtonViewWcs_Click);
            // 
            // listBoxMaterial
            // 
            this.listBoxMaterial.FormattingEnabled = true;
            this.listBoxMaterial.Location = new System.Drawing.Point(12, 115);
            this.listBoxMaterial.Name = "listBoxMaterial";
            this.listBoxMaterial.ScrollAlwaysVisible = true;
            this.listBoxMaterial.Size = new System.Drawing.Size(100, 95);
            this.listBoxMaterial.TabIndex = 3;
            this.listBoxMaterial.SelectedIndexChanged += new System.EventHandler(this.ListBoxMaterial_SelectedIndexChanged);
            // 
            // textBoxUserMaterial
            // 
            this.textBoxUserMaterial.Location = new System.Drawing.Point(12, 220);
            this.textBoxUserMaterial.Name = "textBoxUserMaterial";
            this.textBoxUserMaterial.Size = new System.Drawing.Size(100, 20);
            this.textBoxUserMaterial.TabIndex = 4;
            this.textBoxUserMaterial.TextChanged += new System.EventHandler(this.TextBoxUserMaterial_TextChanged);
            // 
            // buttonExit
            // 
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(20, 653);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(175, 20);
            this.buttonExit.TabIndex = 11;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // buttonEditBlock
            // 
            this.buttonEditBlock.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditBlock.Location = new System.Drawing.Point(8, 19);
            this.buttonEditBlock.Name = "buttonEditBlock";
            this.buttonEditBlock.Size = new System.Drawing.Size(177, 20);
            this.buttonEditBlock.TabIndex = 23;
            this.buttonEditBlock.Text = "Block Component";
            this.buttonEditBlock.UseVisualStyleBackColor = true;
            this.buttonEditBlock.Click += new System.EventHandler(this.ButtonEditBlock_Click);
            // 
            // buttonEditConstruction
            // 
            this.buttonEditConstruction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditConstruction.Location = new System.Drawing.Point(8, 45);
            this.buttonEditConstruction.Name = "buttonEditConstruction";
            this.buttonEditConstruction.Size = new System.Drawing.Size(86, 20);
            this.buttonEditConstruction.TabIndex = 24;
            this.buttonEditConstruction.Text = "Start Edit";
            this.buttonEditConstruction.UseVisualStyleBackColor = true;
            this.buttonEditConstruction.Click += new System.EventHandler(this.ButtonEditConstruction_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonEndEditConstruction);
            this.groupBox1.Controls.Add(this.buttonEditBlock);
            this.groupBox1.Controls.Add(this.buttonEditConstruction);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(12, 409);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 76);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Component";
            // 
            // buttonEndEditConstruction
            // 
            this.buttonEndEditConstruction.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEndEditConstruction.Location = new System.Drawing.Point(100, 45);
            this.buttonEndEditConstruction.Name = "buttonEndEditConstruction";
            this.buttonEndEditConstruction.Size = new System.Drawing.Size(84, 20);
            this.buttonEndEditConstruction.TabIndex = 25;
            this.buttonEndEditConstruction.Text = "End Edit";
            this.buttonEndEditConstruction.UseVisualStyleBackColor = true;
            this.buttonEndEditConstruction.Click += new System.EventHandler(this.ButtonEndEditConstruction_Click);
            // 
            // groupBoxBurnSettings
            // 
            this.groupBoxBurnSettings.Controls.Add(this.checkBoxGrind);
            this.groupBoxBurnSettings.Controls.Add(this.checkBoxBurnout);
            this.groupBoxBurnSettings.Controls.Add(this.checkBoxBurnDirZ);
            this.groupBoxBurnSettings.Controls.Add(this.checkBoxBurnDirY);
            this.groupBoxBurnSettings.Controls.Add(this.comboBoxTolerance);
            this.groupBoxBurnSettings.Controls.Add(this.checkBoxBurnDirX);
            this.groupBoxBurnSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxBurnSettings.Location = new System.Drawing.Point(12, 247);
            this.groupBoxBurnSettings.Name = "groupBoxBurnSettings";
            this.groupBoxBurnSettings.Size = new System.Drawing.Size(193, 100);
            this.groupBoxBurnSettings.TabIndex = 26;
            this.groupBoxBurnSettings.TabStop = false;
            this.groupBoxBurnSettings.Text = "Burn Settings";
            // 
            // checkBoxGrind
            // 
            this.checkBoxGrind.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxGrind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxGrind.Location = new System.Drawing.Point(99, 17);
            this.checkBoxGrind.Name = "checkBoxGrind";
            this.checkBoxGrind.Size = new System.Drawing.Size(84, 20);
            this.checkBoxGrind.TabIndex = 31;
            this.checkBoxGrind.Text = "Grind";
            this.checkBoxGrind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxGrind.UseVisualStyleBackColor = true;
            this.checkBoxGrind.CheckedChanged += new System.EventHandler(this.CheckBoxGrind_CheckedChanged);
            // 
            // checkBoxBurnout
            // 
            this.checkBoxBurnout.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxBurnout.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxBurnout.Location = new System.Drawing.Point(8, 17);
            this.checkBoxBurnout.Name = "checkBoxBurnout";
            this.checkBoxBurnout.Size = new System.Drawing.Size(85, 20);
            this.checkBoxBurnout.TabIndex = 30;
            this.checkBoxBurnout.Text = "Burnout";
            this.checkBoxBurnout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxBurnout.UseVisualStyleBackColor = true;
            // 
            // checkBoxBurnDirZ
            // 
            this.checkBoxBurnDirZ.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxBurnDirZ.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxBurnDirZ.Location = new System.Drawing.Point(127, 43);
            this.checkBoxBurnDirZ.Name = "checkBoxBurnDirZ";
            this.checkBoxBurnDirZ.Size = new System.Drawing.Size(57, 20);
            this.checkBoxBurnDirZ.TabIndex = 29;
            this.checkBoxBurnDirZ.Text = "Z";
            this.checkBoxBurnDirZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxBurnDirZ.UseVisualStyleBackColor = true;
            this.checkBoxBurnDirZ.CheckedChanged += new System.EventHandler(this.CheckBoxBurnDirZ_CheckedChanged);
            // 
            // checkBoxBurnDirY
            // 
            this.checkBoxBurnDirY.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxBurnDirY.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxBurnDirY.Location = new System.Drawing.Point(68, 43);
            this.checkBoxBurnDirY.Name = "checkBoxBurnDirY";
            this.checkBoxBurnDirY.Size = new System.Drawing.Size(57, 20);
            this.checkBoxBurnDirY.TabIndex = 28;
            this.checkBoxBurnDirY.Text = "Y";
            this.checkBoxBurnDirY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxBurnDirY.UseVisualStyleBackColor = true;
            this.checkBoxBurnDirY.CheckedChanged += new System.EventHandler(this.CheckBoxBurnDirY_CheckedChanged);
            // 
            // comboBoxTolerance
            // 
            this.comboBoxTolerance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTolerance.FormattingEnabled = true;
            this.comboBoxTolerance.Location = new System.Drawing.Point(9, 69);
            this.comboBoxTolerance.Name = "comboBoxTolerance";
            this.comboBoxTolerance.Size = new System.Drawing.Size(174, 21);
            this.comboBoxTolerance.TabIndex = 1;
            // 
            // checkBoxBurnDirX
            // 
            this.checkBoxBurnDirX.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxBurnDirX.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxBurnDirX.Location = new System.Drawing.Point(9, 43);
            this.checkBoxBurnDirX.Name = "checkBoxBurnDirX";
            this.checkBoxBurnDirX.Size = new System.Drawing.Size(57, 20);
            this.checkBoxBurnDirX.TabIndex = 27;
            this.checkBoxBurnDirX.Text = "X";
            this.checkBoxBurnDirX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxBurnDirX.UseVisualStyleBackColor = true;
            this.checkBoxBurnDirX.CheckedChanged += new System.EventHandler(this.CheckBoxBurnDirX_CheckedChanged);
            // 
            // chk4Digits
            // 
            this.chk4Digits.AutoSize = true;
            this.chk4Digits.Location = new System.Drawing.Point(12, 39);
            this.chk4Digits.Name = "chk4Digits";
            this.chk4Digits.Size = new System.Drawing.Size(91, 17);
            this.chk4Digits.TabIndex = 27;
            this.chk4Digits.Text = "4 Digit Details";
            this.chk4Digits.UseVisualStyleBackColor = true;
            this.chk4Digits.CheckedChanged += new System.EventHandler(this.chkDigits_CheckedChanged);
            // 
            // chkAnyAssembly
            // 
            this.chkAnyAssembly.AutoSize = true;
            this.chkAnyAssembly.Location = new System.Drawing.Point(104, 39);
            this.chkAnyAssembly.Name = "chkAnyAssembly";
            this.chkAnyAssembly.Size = new System.Drawing.Size(91, 17);
            this.chkAnyAssembly.TabIndex = 28;
            this.chkAnyAssembly.Text = "Any Assembly";
            this.chkAnyAssembly.UseVisualStyleBackColor = true;
            this.chkAnyAssembly.CheckedChanged += new System.EventHandler(this.chkAnyAssembly_CheckedChanged);
            // 
            // btnMakeUnique
            // 
            this.btnMakeUnique.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMakeUnique.Location = new System.Drawing.Point(6, 71);
            this.btnMakeUnique.Name = "btnMakeUnique";
            this.btnMakeUnique.Size = new System.Drawing.Size(174, 20);
            this.btnMakeUnique.TabIndex = 7;
            this.btnMakeUnique.Text = "Make Unique";
            this.btnMakeUnique.UseVisualStyleBackColor = true;
            this.btnMakeUnique.Click += new System.EventHandler(this.btnMakeUnique_Click);
            // 
            // ComponentBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 683);
            this.ControlBox = false;
            this.Controls.Add(this.chkAnyAssembly);
            this.Controls.Add(this.chk4Digits);
            this.Controls.Add(this.groupBoxBurnSettings);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.changeColorCheckBox);
            this.Controls.Add(this.textBoxUserMaterial);
            this.Controls.Add(this.listBoxMaterial);
            this.Controls.Add(this.groupBoxWorkPlane);
            this.Controls.Add(this.groupBoxEditAssembly);
            this.Controls.Add(this.updateSessionButton);
            this.Controls.Add(this.groupBoxColor);
            this.Controls.Add(this.comboBoxCompName);
            this.Controls.Add(this.textBoxDetailNumber);
            this.Controls.Add(this.buttonExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Name = "ComponentBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Component Builder";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxColor.ResumeLayout(false);
            this.groupBoxEditAssembly.ResumeLayout(false);
            this.groupBoxWorkPlane.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBoxBurnSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDetailNumber;
        private System.Windows.Forms.ComboBox comboBoxGrid;
        private System.Windows.Forms.ComboBox comboBoxCompName;
        private System.Windows.Forms.GroupBox groupBoxColor;
        private System.Windows.Forms.Button buttonDarkDullBlue;
        private System.Windows.Forms.Button buttonPurple;
        private System.Windows.Forms.Button buttonDarkWeakRed;
        private System.Windows.Forms.Button buttonMedAzureBlue;
        private System.Windows.Forms.Button buttonDarkWeakMagenta;
        private System.Windows.Forms.Button buttonAquamarine;
        private System.Windows.Forms.Button buttonDarkDullGreen;
        private System.Windows.Forms.Button buttonObscureDullGreen;
        private System.Windows.Forms.Button updateSessionButton;
        private System.Windows.Forms.GroupBox groupBoxEditAssembly;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.GroupBox groupBoxWorkPlane;
        private System.Windows.Forms.Button buttonViewWcs;
        private System.Windows.Forms.ListBox listBoxMaterial;
        private System.Windows.Forms.TextBox textBoxUserMaterial;
        private System.Windows.Forms.CheckBox changeColorCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonEditBlock;
        private System.Windows.Forms.Button buttonEditConstruction;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonEndEditConstruction;
        private System.Windows.Forms.GroupBox groupBoxBurnSettings;
        private System.Windows.Forms.ComboBox comboBoxTolerance;
        private System.Windows.Forms.CheckBox checkBoxBurnDirX;
        private System.Windows.Forms.CheckBox checkBoxBurnout;
        private System.Windows.Forms.CheckBox checkBoxBurnDirZ;
        private System.Windows.Forms.CheckBox checkBoxBurnDirY;
        private System.Windows.Forms.CheckBox checkBoxUpperComp;
        private System.Windows.Forms.CheckBox checkBoxGrind;
        private System.Windows.Forms.Button buttonAutoLwr;
        private System.Windows.Forms.Button buttonAutoUpr;
        private System.Windows.Forms.Button buttonLwrRetAssm;
        private System.Windows.Forms.Button buttonUprRetAssm;
        private System.Windows.Forms.CheckBox chk4Digits;
        private System.Windows.Forms.CheckBox chkAnyAssembly;
        private System.Windows.Forms.Button btnMakeUnique;
    }
}