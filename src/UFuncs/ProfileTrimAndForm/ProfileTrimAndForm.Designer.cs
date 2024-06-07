namespace TSG_Library.UFuncs
{
    partial class ProfileTrimAndForm
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
            this.grpProfile = new System.Windows.Forms.GroupBox();
            this.cmbLayer = new System.Windows.Forms.ComboBox();
            this.rdoForm = new System.Windows.Forms.RadioButton();
            this.rdoTrim = new System.Windows.Forms.RadioButton();
            this.grpForm = new System.Windows.Forms.GroupBox();
            this.grpPad = new System.Windows.Forms.GroupBox();
            this.rdoOuter = new System.Windows.Forms.RadioButton();
            this.rdoInner = new System.Windows.Forms.RadioButton();
            this.rdoLower = new System.Windows.Forms.RadioButton();
            this.rdoUpper = new System.Windows.Forms.RadioButton();
            this.btnExecute = new System.Windows.Forms.Button();
            this.grpCurve = new System.Windows.Forms.GroupBox();
            this.rdoSelectCurves = new System.Windows.Forms.RadioButton();
            this.rdoAllCurves = new System.Windows.Forms.RadioButton();
            this.chkFeatureGroup = new System.Windows.Forms.CheckBox();
            this.grpProfile.SuspendLayout();
            this.grpForm.SuspendLayout();
            this.grpPad.SuspendLayout();
            this.grpCurve.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpProfile
            // 
            this.grpProfile.Controls.Add(this.cmbLayer);
            this.grpProfile.Controls.Add(this.rdoForm);
            this.grpProfile.Controls.Add(this.rdoTrim);
            this.grpProfile.Location = new System.Drawing.Point(16, 15);
            this.grpProfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpProfile.Name = "grpProfile";
            this.grpProfile.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpProfile.Size = new System.Drawing.Size(109, 127);
            this.grpProfile.TabIndex = 0;
            this.grpProfile.TabStop = false;
            this.grpProfile.Text = "Profile Type";
            // 
            // cmbLayer
            // 
            this.cmbLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayer.FormattingEnabled = true;
            this.cmbLayer.Location = new System.Drawing.Point(8, 94);
            this.cmbLayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbLayer.Name = "cmbLayer";
            this.cmbLayer.Size = new System.Drawing.Size(92, 24);
            this.cmbLayer.TabIndex = 3;
            // 
            // rdoForm
            // 
            this.rdoForm.AutoSize = true;
            this.rdoForm.Location = new System.Drawing.Point(8, 52);
            this.rdoForm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoForm.Name = "rdoForm";
            this.rdoForm.Size = new System.Drawing.Size(59, 20);
            this.rdoForm.TabIndex = 1;
            this.rdoForm.TabStop = true;
            this.rdoForm.Text = "Form";
            this.rdoForm.UseVisualStyleBackColor = true;
            // 
            // rdoTrim
            // 
            this.rdoTrim.AutoSize = true;
            this.rdoTrim.Location = new System.Drawing.Point(8, 23);
            this.rdoTrim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoTrim.Name = "rdoTrim";
            this.rdoTrim.Size = new System.Drawing.Size(55, 20);
            this.rdoTrim.TabIndex = 0;
            this.rdoTrim.TabStop = true;
            this.rdoTrim.Text = "Trim";
            this.rdoTrim.UseVisualStyleBackColor = true;
            this.rdoTrim.CheckedChanged += new System.EventHandler(this.RdoTrim_CheckedChanged);
            // 
            // grpForm
            // 
            this.grpForm.Controls.Add(this.grpPad);
            this.grpForm.Controls.Add(this.rdoLower);
            this.grpForm.Controls.Add(this.rdoUpper);
            this.grpForm.Location = new System.Drawing.Point(133, 15);
            this.grpForm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpForm.Name = "grpForm";
            this.grpForm.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpForm.Size = new System.Drawing.Size(217, 127);
            this.grpForm.TabIndex = 1;
            this.grpForm.TabStop = false;
            this.grpForm.Text = "Form Type";
            // 
            // grpPad
            // 
            this.grpPad.Controls.Add(this.rdoOuter);
            this.grpPad.Controls.Add(this.rdoInner);
            this.grpPad.Location = new System.Drawing.Point(88, 23);
            this.grpPad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpPad.Name = "grpPad";
            this.grpPad.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpPad.Size = new System.Drawing.Size(111, 92);
            this.grpPad.TabIndex = 4;
            this.grpPad.TabStop = false;
            this.grpPad.Text = "Pad Offset";
            // 
            // rdoOuter
            // 
            this.rdoOuter.AutoSize = true;
            this.rdoOuter.Location = new System.Drawing.Point(8, 52);
            this.rdoOuter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoOuter.Name = "rdoOuter";
            this.rdoOuter.Size = new System.Drawing.Size(60, 20);
            this.rdoOuter.TabIndex = 3;
            this.rdoOuter.TabStop = true;
            this.rdoOuter.Text = "Outer";
            this.rdoOuter.UseVisualStyleBackColor = true;
            // 
            // rdoInner
            // 
            this.rdoInner.AutoSize = true;
            this.rdoInner.Location = new System.Drawing.Point(8, 23);
            this.rdoInner.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoInner.Name = "rdoInner";
            this.rdoInner.Size = new System.Drawing.Size(57, 20);
            this.rdoInner.TabIndex = 2;
            this.rdoInner.TabStop = true;
            this.rdoInner.Text = "Inner";
            this.rdoInner.UseVisualStyleBackColor = true;
            // 
            // rdoLower
            // 
            this.rdoLower.AutoSize = true;
            this.rdoLower.Location = new System.Drawing.Point(8, 52);
            this.rdoLower.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoLower.Name = "rdoLower";
            this.rdoLower.Size = new System.Drawing.Size(64, 20);
            this.rdoLower.TabIndex = 3;
            this.rdoLower.TabStop = true;
            this.rdoLower.Text = "Lower";
            this.rdoLower.UseVisualStyleBackColor = true;
            // 
            // rdoUpper
            // 
            this.rdoUpper.AutoSize = true;
            this.rdoUpper.Location = new System.Drawing.Point(8, 23);
            this.rdoUpper.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoUpper.Name = "rdoUpper";
            this.rdoUpper.Size = new System.Drawing.Size(66, 20);
            this.rdoUpper.TabIndex = 2;
            this.rdoUpper.TabStop = true;
            this.rdoUpper.Text = "Upper";
            this.rdoUpper.UseVisualStyleBackColor = true;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(12, 256);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(339, 28);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.BtnExecute_Click);
            // 
            // grpCurve
            // 
            this.grpCurve.Controls.Add(this.rdoSelectCurves);
            this.grpCurve.Controls.Add(this.rdoAllCurves);
            this.grpCurve.Location = new System.Drawing.Point(16, 149);
            this.grpCurve.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpCurve.Name = "grpCurve";
            this.grpCurve.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpCurve.Size = new System.Drawing.Size(335, 57);
            this.grpCurve.TabIndex = 5;
            this.grpCurve.TabStop = false;
            this.grpCurve.Text = "Curve Selection";
            // 
            // rdoSelectCurves
            // 
            this.rdoSelectCurves.AutoSize = true;
            this.rdoSelectCurves.Location = new System.Drawing.Point(112, 23);
            this.rdoSelectCurves.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoSelectCurves.Name = "rdoSelectCurves";
            this.rdoSelectCurves.Size = new System.Drawing.Size(111, 20);
            this.rdoSelectCurves.TabIndex = 3;
            this.rdoSelectCurves.TabStop = true;
            this.rdoSelectCurves.Text = "Select Curves";
            this.rdoSelectCurves.UseVisualStyleBackColor = true;
            // 
            // rdoAllCurves
            // 
            this.rdoAllCurves.AutoSize = true;
            this.rdoAllCurves.Location = new System.Drawing.Point(8, 23);
            this.rdoAllCurves.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoAllCurves.Name = "rdoAllCurves";
            this.rdoAllCurves.Size = new System.Drawing.Size(88, 20);
            this.rdoAllCurves.TabIndex = 2;
            this.rdoAllCurves.TabStop = true;
            this.rdoAllCurves.Text = "All Curves";
            this.rdoAllCurves.UseVisualStyleBackColor = true;
            // 
            // chkFeatureGroup
            // 
            this.chkFeatureGroup.AutoSize = true;
            this.chkFeatureGroup.Location = new System.Drawing.Point(16, 213);
            this.chkFeatureGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkFeatureGroup.Name = "chkFeatureGroup";
            this.chkFeatureGroup.Size = new System.Drawing.Size(158, 20);
            this.chkFeatureGroup.TabIndex = 6;
            this.chkFeatureGroup.Text = "Create Feature Group";
            this.chkFeatureGroup.UseVisualStyleBackColor = true;
            // 
            // ProfileTrimAndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 299);
            this.Controls.Add(this.chkFeatureGroup);
            this.Controls.Add(this.grpCurve);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.grpForm);
            this.Controls.Add(this.grpProfile);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ProfileTrimAndForm";
            this.Text = "1919";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpProfile.ResumeLayout(false);
            this.grpProfile.PerformLayout();
            this.grpForm.ResumeLayout(false);
            this.grpForm.PerformLayout();
            this.grpPad.ResumeLayout(false);
            this.grpPad.PerformLayout();
            this.grpCurve.ResumeLayout(false);
            this.grpCurve.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProfile;
        private System.Windows.Forms.RadioButton rdoForm;
        private System.Windows.Forms.RadioButton rdoTrim;
        private System.Windows.Forms.GroupBox grpForm;
        private System.Windows.Forms.GroupBox grpPad;
        private System.Windows.Forms.RadioButton rdoOuter;
        private System.Windows.Forms.RadioButton rdoInner;
        private System.Windows.Forms.RadioButton rdoLower;
        private System.Windows.Forms.RadioButton rdoUpper;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox cmbLayer;
        private System.Windows.Forms.GroupBox grpCurve;
        private System.Windows.Forms.RadioButton rdoSelectCurves;
        private System.Windows.Forms.RadioButton rdoAllCurves;
        private System.Windows.Forms.CheckBox chkFeatureGroup;
    }
}