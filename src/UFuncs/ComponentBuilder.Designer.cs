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
            this.btnMakeUnique = new System.Windows.Forms.Button();
            this.buttonLwrRetAssm = new System.Windows.Forms.Button();
            this.buttonAutoLwr = new System.Windows.Forms.Button();
            this.buttonAutoUpr = new System.Windows.Forms.Button();
            this.buttonUprRetAssm = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.listBoxMaterial = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chk4Digits = new System.Windows.Forms.CheckBox();
            this.chkAnyAssembly = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxEditBlock = new System.Windows.Forms.GroupBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonAlignEdgeDistance = new System.Windows.Forms.Button();
            this.buttonAlignComponent = new System.Windows.Forms.Button();
            this.buttonEditDynamic = new System.Windows.Forms.Button();
            this.buttonEditMove = new System.Windows.Forms.Button();
            this.buttonEditAlign = new System.Windows.Forms.Button();
            this.buttonEditSize = new System.Windows.Forms.Button();
            this.buttonEditMatch = new System.Windows.Forms.Button();
            this.groupBoxColor.SuspendLayout();
            this.groupBoxEditAssembly.SuspendLayout();
            this.groupBoxEditBlock.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDetailNumber
            // 
            this.textBoxDetailNumber.Location = new System.Drawing.Point(9, 60);
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
            this.comboBoxGrid.Location = new System.Drawing.Point(9, 187);
            this.comboBoxGrid.Name = "comboBoxGrid";
            this.comboBoxGrid.Size = new System.Drawing.Size(100, 21);
            this.comboBoxGrid.TabIndex = 0;
            this.comboBoxGrid.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGrid_SelectedIndexChanged);
            // 
            // comboBoxCompName
            // 
            this.comboBoxCompName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCompName.FormattingEnabled = true;
            this.comboBoxCompName.Location = new System.Drawing.Point(114, 58);
            this.comboBoxCompName.Name = "comboBoxCompName";
            this.comboBoxCompName.Size = new System.Drawing.Size(88, 21);
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
            this.groupBoxColor.Location = new System.Drawing.Point(116, 87);
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
            this.changeColorCheckBox.Location = new System.Drawing.Point(10, 214);
            this.changeColorCheckBox.Name = "changeColorCheckBox";
            this.changeColorCheckBox.Size = new System.Drawing.Size(99, 23);
            this.changeColorCheckBox.TabIndex = 15;
            this.changeColorCheckBox.Text = "Change Color";
            this.changeColorCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.changeColorCheckBox.UseVisualStyleBackColor = true;
            this.changeColorCheckBox.CheckedChanged += new System.EventHandler(this.ChangeColorCheckBox_CheckedChanged);
            // 
            // updateSessionButton
            // 
            this.updateSessionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.updateSessionButton.Location = new System.Drawing.Point(10, 11);
            this.updateSessionButton.Name = "updateSessionButton";
            this.updateSessionButton.Size = new System.Drawing.Size(111, 20);
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
            this.groupBoxEditAssembly.Location = new System.Drawing.Point(9, 444);
            this.groupBoxEditAssembly.Name = "groupBoxEditAssembly";
            this.groupBoxEditAssembly.Size = new System.Drawing.Size(193, 156);
            this.groupBoxEditAssembly.TabIndex = 8;
            this.groupBoxEditAssembly.TabStop = false;
            this.groupBoxEditAssembly.Text = "Edit Assembly";
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
            // listBoxMaterial
            // 
            this.listBoxMaterial.FormattingEnabled = true;
            this.listBoxMaterial.Location = new System.Drawing.Point(9, 86);
            this.listBoxMaterial.Name = "listBoxMaterial";
            this.listBoxMaterial.ScrollAlwaysVisible = true;
            this.listBoxMaterial.Size = new System.Drawing.Size(100, 95);
            this.listBoxMaterial.TabIndex = 3;
            this.listBoxMaterial.SelectedIndexChanged += new System.EventHandler(this.ListBoxMaterial_SelectedIndexChanged);
            // 
            // chk4Digits
            // 
            this.chk4Digits.AutoSize = true;
            this.chk4Digits.Location = new System.Drawing.Point(9, 37);
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
            this.chkAnyAssembly.Location = new System.Drawing.Point(101, 37);
            this.chkAnyAssembly.Name = "chkAnyAssembly";
            this.chkAnyAssembly.Size = new System.Drawing.Size(91, 17);
            this.chkAnyAssembly.TabIndex = 28;
            this.chkAnyAssembly.Text = "Any Assembly";
            this.chkAnyAssembly.UseVisualStyleBackColor = true;
            this.chkAnyAssembly.CheckedChanged += new System.EventHandler(this.chkAnyAssembly_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Version";
            // 
            // groupBoxEditBlock
            // 
            this.groupBoxEditBlock.Controls.Add(this.buttonApply);
            this.groupBoxEditBlock.Controls.Add(this.buttonAlignEdgeDistance);
            this.groupBoxEditBlock.Controls.Add(this.buttonAlignComponent);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditDynamic);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditMove);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditAlign);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditSize);
            this.groupBoxEditBlock.Controls.Add(this.buttonEditMatch);
            this.groupBoxEditBlock.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxEditBlock.Location = new System.Drawing.Point(10, 243);
            this.groupBoxEditBlock.Name = "groupBoxEditBlock";
            this.groupBoxEditBlock.Size = new System.Drawing.Size(192, 195);
            this.groupBoxEditBlock.TabIndex = 30;
            this.groupBoxEditBlock.TabStop = false;
            this.groupBoxEditBlock.Text = "Edit Block";
            // 
            // buttonApply
            // 
            this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonApply.Location = new System.Drawing.Point(6, 155);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(178, 30);
            this.buttonApply.TabIndex = 31;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonAlignEdgeDistance
            // 
            this.buttonAlignEdgeDistance.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAlignEdgeDistance.Location = new System.Drawing.Point(6, 129);
            this.buttonAlignEdgeDistance.Name = "buttonAlignEdgeDistance";
            this.buttonAlignEdgeDistance.Size = new System.Drawing.Size(178, 20);
            this.buttonAlignEdgeDistance.TabIndex = 10;
            this.buttonAlignEdgeDistance.Text = "Align Edge Distance";
            this.buttonAlignEdgeDistance.UseVisualStyleBackColor = true;
            this.buttonAlignEdgeDistance.Click += new System.EventHandler(this.buttonAlignEdgeDistance_Click);
            // 
            // buttonAlignComponent
            // 
            this.buttonAlignComponent.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonAlignComponent.Location = new System.Drawing.Point(6, 103);
            this.buttonAlignComponent.Name = "buttonAlignComponent";
            this.buttonAlignComponent.Size = new System.Drawing.Size(177, 20);
            this.buttonAlignComponent.TabIndex = 9;
            this.buttonAlignComponent.Text = "_Align Component_";
            this.buttonAlignComponent.UseVisualStyleBackColor = true;
            this.buttonAlignComponent.Click += new System.EventHandler(this.buttonAlignComponent_Click);
            // 
            // buttonEditDynamic
            // 
            this.buttonEditDynamic.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditDynamic.Location = new System.Drawing.Point(6, 19);
            this.buttonEditDynamic.Name = "buttonEditDynamic";
            this.buttonEditDynamic.Size = new System.Drawing.Size(178, 20);
            this.buttonEditDynamic.TabIndex = 0;
            this.buttonEditDynamic.Text = "Dynamic";
            this.buttonEditDynamic.UseVisualStyleBackColor = true;
            this.buttonEditDynamic.Click += new System.EventHandler(this.buttonEditDynamic_Click);
            // 
            // buttonEditMove
            // 
            this.buttonEditMove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditMove.Location = new System.Drawing.Point(6, 48);
            this.buttonEditMove.Name = "buttonEditMove";
            this.buttonEditMove.Size = new System.Drawing.Size(85, 20);
            this.buttonEditMove.TabIndex = 1;
            this.buttonEditMove.Text = "_Move_";
            this.buttonEditMove.UseVisualStyleBackColor = true;
            this.buttonEditMove.Click += new System.EventHandler(this.buttonEditMove_Click);
            // 
            // buttonEditAlign
            // 
            this.buttonEditAlign.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditAlign.Location = new System.Drawing.Point(97, 77);
            this.buttonEditAlign.Name = "buttonEditAlign";
            this.buttonEditAlign.Size = new System.Drawing.Size(87, 20);
            this.buttonEditAlign.TabIndex = 2;
            this.buttonEditAlign.Text = "Align Edge";
            this.buttonEditAlign.UseVisualStyleBackColor = true;
            this.buttonEditAlign.Click += new System.EventHandler(this.buttonEditAlign_Click);
            // 
            // buttonEditSize
            // 
            this.buttonEditSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditSize.Location = new System.Drawing.Point(6, 77);
            this.buttonEditSize.Name = "buttonEditSize";
            this.buttonEditSize.Size = new System.Drawing.Size(85, 20);
            this.buttonEditSize.TabIndex = 3;
            this.buttonEditSize.Text = "_Size_";
            this.buttonEditSize.UseVisualStyleBackColor = true;
            this.buttonEditSize.Click += new System.EventHandler(this.buttonEditSize_Click);
            // 
            // buttonEditMatch
            // 
            this.buttonEditMatch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEditMatch.Location = new System.Drawing.Point(97, 48);
            this.buttonEditMatch.Name = "buttonEditMatch";
            this.buttonEditMatch.Size = new System.Drawing.Size(87, 20);
            this.buttonEditMatch.TabIndex = 8;
            this.buttonEditMatch.Text = "Match";
            this.buttonEditMatch.UseVisualStyleBackColor = true;
            this.buttonEditMatch.Click += new System.EventHandler(this.buttonEditMatch_Click);
            // 
            // ComponentBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 609);
            this.Controls.Add(this.groupBoxEditBlock);
            this.Controls.Add(this.comboBoxGrid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkAnyAssembly);
            this.Controls.Add(this.chk4Digits);
            this.Controls.Add(this.changeColorCheckBox);
            this.Controls.Add(this.listBoxMaterial);
            this.Controls.Add(this.groupBoxEditAssembly);
            this.Controls.Add(this.updateSessionButton);
            this.Controls.Add(this.groupBoxColor);
            this.Controls.Add(this.comboBoxCompName);
            this.Controls.Add(this.textBoxDetailNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ComponentBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Component Builder";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxColor.ResumeLayout(false);
            this.groupBoxEditAssembly.ResumeLayout(false);
            this.groupBoxEditBlock.ResumeLayout(false);
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
        private System.Windows.Forms.ListBox listBoxMaterial;
        private System.Windows.Forms.CheckBox changeColorCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxUpperComp;
        private System.Windows.Forms.Button buttonAutoLwr;
        private System.Windows.Forms.Button buttonAutoUpr;
        private System.Windows.Forms.Button buttonLwrRetAssm;
        private System.Windows.Forms.Button buttonUprRetAssm;
        private System.Windows.Forms.CheckBox chk4Digits;
        private System.Windows.Forms.CheckBox chkAnyAssembly;
        private System.Windows.Forms.Button btnMakeUnique;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxEditBlock;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonAlignEdgeDistance;
        private System.Windows.Forms.Button buttonAlignComponent;
        private System.Windows.Forms.Button buttonEditDynamic;
        private System.Windows.Forms.Button buttonEditMove;
        private System.Windows.Forms.Button buttonEditAlign;
        private System.Windows.Forms.Button buttonEditSize;
        private System.Windows.Forms.Button buttonEditMatch;
    }
}