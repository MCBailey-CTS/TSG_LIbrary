namespace TSG_Library.UFuncs.DrainHoleCreator
{
    partial class Form
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
            this.grpDrainHoles = new System.Windows.Forms.GroupBox();
            this.chkSubtractDrainHoles = new System.Windows.Forms.CheckBox();
            this.txtDiameterDrain = new System.Windows.Forms.TextBox();
            this.lblDiameter = new System.Windows.Forms.Label();
            this.grpPlacement = new System.Windows.Forms.GroupBox();
            this.chkCorners = new System.Windows.Forms.CheckBox();
            this.chkMidPoints = new System.Windows.Forms.CheckBox();
            this.btnDrainHoles = new System.Windows.Forms.Button();
            this.grpUnits = new System.Windows.Forms.GroupBox();
            this.rdoMetric = new System.Windows.Forms.RadioButton();
            this.rdoEnglish = new System.Windows.Forms.RadioButton();
            this.grpExtrusionLimits = new System.Windows.Forms.GroupBox();
            this.txtEndLimit = new System.Windows.Forms.TextBox();
            this.txtStartLimit = new System.Windows.Forms.TextBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.grpDrainHoles.SuspendLayout();
            this.grpPlacement.SuspendLayout();
            this.grpUnits.SuspendLayout();
            this.grpExtrusionLimits.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDrainHoles
            // 
            this.grpDrainHoles.Controls.Add(this.chkSubtractDrainHoles);
            this.grpDrainHoles.Controls.Add(this.txtDiameterDrain);
            this.grpDrainHoles.Controls.Add(this.lblDiameter);
            this.grpDrainHoles.Location = new System.Drawing.Point(12, 67);
            this.grpDrainHoles.Name = "grpDrainHoles";
            this.grpDrainHoles.Size = new System.Drawing.Size(155, 77);
            this.grpDrainHoles.TabIndex = 0;
            this.grpDrainHoles.TabStop = false;
            this.grpDrainHoles.Text = "Drain Holes";
            // 
            // chkSubtractDrainHoles
            // 
            this.chkSubtractDrainHoles.AutoSize = true;
            this.chkSubtractDrainHoles.Location = new System.Drawing.Point(19, 43);
            this.chkSubtractDrainHoles.Name = "chkSubtractDrainHoles";
            this.chkSubtractDrainHoles.Size = new System.Drawing.Size(96, 17);
            this.chkSubtractDrainHoles.TabIndex = 16;
            this.chkSubtractDrainHoles.TabStop = false;
            this.chkSubtractDrainHoles.Text = "Subtract Holes";
            this.chkSubtractDrainHoles.UseVisualStyleBackColor = true;
            this.chkSubtractDrainHoles.CheckedChanged += new System.EventHandler(this.ChkSubtractDrainHoles_CheckedChanged);
            // 
            // txtDiameterDrain
            // 
            this.txtDiameterDrain.Location = new System.Drawing.Point(62, 17);
            this.txtDiameterDrain.Name = "txtDiameterDrain";
            this.txtDiameterDrain.Size = new System.Drawing.Size(67, 20);
            this.txtDiameterDrain.TabIndex = 0;
            this.txtDiameterDrain.Text = "2.0";
            this.txtDiameterDrain.TextChanged += new System.EventHandler(this.TxtDiameterDrain_TextChanged);
            // 
            // lblDiameter
            // 
            this.lblDiameter.AutoSize = true;
            this.lblDiameter.Location = new System.Drawing.Point(7, 20);
            this.lblDiameter.Name = "lblDiameter";
            this.lblDiameter.Size = new System.Drawing.Size(52, 13);
            this.lblDiameter.TabIndex = 15;
            this.lblDiameter.Text = "Diameter:";
            // 
            // grpPlacement
            // 
            this.grpPlacement.Controls.Add(this.chkCorners);
            this.grpPlacement.Controls.Add(this.chkMidPoints);
            this.grpPlacement.Location = new System.Drawing.Point(12, 150);
            this.grpPlacement.Name = "grpPlacement";
            this.grpPlacement.Size = new System.Drawing.Size(155, 74);
            this.grpPlacement.TabIndex = 1;
            this.grpPlacement.TabStop = false;
            this.grpPlacement.Text = "Drain Hole Placement";
            // 
            // chkCorners
            // 
            this.chkCorners.AutoSize = true;
            this.chkCorners.Location = new System.Drawing.Point(9, 42);
            this.chkCorners.Name = "chkCorners";
            this.chkCorners.Size = new System.Drawing.Size(62, 17);
            this.chkCorners.TabIndex = 17;
            this.chkCorners.TabStop = false;
            this.chkCorners.Text = "Corners";
            this.chkCorners.UseVisualStyleBackColor = true;
            this.chkCorners.CheckedChanged += new System.EventHandler(this.ChkCorners_CheckedChanged);
            // 
            // chkMidPoints
            // 
            this.chkMidPoints.AutoSize = true;
            this.chkMidPoints.Location = new System.Drawing.Point(9, 19);
            this.chkMidPoints.Name = "chkMidPoints";
            this.chkMidPoints.Size = new System.Drawing.Size(75, 17);
            this.chkMidPoints.TabIndex = 16;
            this.chkMidPoints.TabStop = false;
            this.chkMidPoints.Text = "Mid-Points";
            this.chkMidPoints.UseVisualStyleBackColor = true;
            this.chkMidPoints.CheckedChanged += new System.EventHandler(this.ChkMidPoints_CheckedChanged);
            // 
            // btnDrainHoles
            // 
            this.btnDrainHoles.Location = new System.Drawing.Point(12, 326);
            this.btnDrainHoles.Name = "btnDrainHoles";
            this.btnDrainHoles.Size = new System.Drawing.Size(155, 23);
            this.btnDrainHoles.TabIndex = 4;
            this.btnDrainHoles.TabStop = false;
            this.btnDrainHoles.Text = "Drain Holes";
            this.btnDrainHoles.UseVisualStyleBackColor = true;
            this.btnDrainHoles.Click += new System.EventHandler(this.BtnSelectFacesDrain_Click);
            // 
            // grpUnits
            // 
            this.grpUnits.Controls.Add(this.rdoMetric);
            this.grpUnits.Controls.Add(this.rdoEnglish);
            this.grpUnits.Location = new System.Drawing.Point(12, 12);
            this.grpUnits.Name = "grpUnits";
            this.grpUnits.Size = new System.Drawing.Size(155, 49);
            this.grpUnits.TabIndex = 0;
            this.grpUnits.TabStop = false;
            this.grpUnits.Text = "DiameterUnits";
            // 
            // rdoMetric
            // 
            this.rdoMetric.AutoSize = true;
            this.rdoMetric.Location = new System.Drawing.Point(10, 19);
            this.rdoMetric.Name = "rdoMetric";
            this.rdoMetric.Size = new System.Drawing.Size(54, 17);
            this.rdoMetric.TabIndex = 110;
            this.rdoMetric.Text = "Metric";
            this.rdoMetric.UseVisualStyleBackColor = true;
            this.rdoMetric.CheckedChanged += new System.EventHandler(this.RdoMetric_CheckedChanged);
            // 
            // rdoEnglish
            // 
            this.rdoEnglish.AutoSize = true;
            this.rdoEnglish.Location = new System.Drawing.Point(70, 19);
            this.rdoEnglish.Name = "rdoEnglish";
            this.rdoEnglish.Size = new System.Drawing.Size(59, 17);
            this.rdoEnglish.TabIndex = 10;
            this.rdoEnglish.Text = "English";
            this.rdoEnglish.UseVisualStyleBackColor = true;
            this.rdoEnglish.CheckedChanged += new System.EventHandler(this.RdoEnglish_CheckedChanged);
            // 
            // grpExtrusionLimits
            // 
            this.grpExtrusionLimits.Controls.Add(this.txtEndLimit);
            this.grpExtrusionLimits.Controls.Add(this.txtStartLimit);
            this.grpExtrusionLimits.Controls.Add(this.lblEnd);
            this.grpExtrusionLimits.Controls.Add(this.lblStart);
            this.grpExtrusionLimits.Location = new System.Drawing.Point(12, 230);
            this.grpExtrusionLimits.Name = "grpExtrusionLimits";
            this.grpExtrusionLimits.Size = new System.Drawing.Size(155, 90);
            this.grpExtrusionLimits.TabIndex = 2;
            this.grpExtrusionLimits.TabStop = false;
            this.grpExtrusionLimits.Text = "Extrusion Limits (IN)";
            // 
            // txtEndLimit
            // 
            this.txtEndLimit.Location = new System.Drawing.Point(44, 52);
            this.txtEndLimit.Name = "txtEndLimit";
            this.txtEndLimit.Size = new System.Drawing.Size(85, 20);
            this.txtEndLimit.TabIndex = 1;
            this.txtEndLimit.TextChanged += new System.EventHandler(this.TxtEndLimit_TextChanged);
            // 
            // txtStartLimit
            // 
            this.txtStartLimit.Location = new System.Drawing.Point(44, 19);
            this.txtStartLimit.Name = "txtStartLimit";
            this.txtStartLimit.Size = new System.Drawing.Size(85, 20);
            this.txtStartLimit.TabIndex = 0;
            this.txtStartLimit.TextChanged += new System.EventHandler(this.TxtStartLimit_TextChanged);
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(9, 55);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(29, 13);
            this.lblEnd.TabIndex = 1;
            this.lblEnd.Text = "End:";
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(6, 22);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(32, 13);
            this.lblStart.TabIndex = 0;
            this.lblStart.Text = "Start:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(181, 361);
            this.Controls.Add(this.grpExtrusionLimits);
            this.Controls.Add(this.grpPlacement);
            this.Controls.Add(this.grpUnits);
            this.Controls.Add(this.grpDrainHoles);
            this.Controls.Add(this.btnDrainHoles);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Drain Holes: 1.3";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.grpDrainHoles.ResumeLayout(false);
            this.grpDrainHoles.PerformLayout();
            this.grpPlacement.ResumeLayout(false);
            this.grpPlacement.PerformLayout();
            this.grpUnits.ResumeLayout(false);
            this.grpUnits.PerformLayout();
            this.grpExtrusionLimits.ResumeLayout(false);
            this.grpExtrusionLimits.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grpDrainHoles;
        private System.Windows.Forms.Button btnDrainHoles;
        private System.Windows.Forms.TextBox txtDiameterDrain;
        private System.Windows.Forms.Label lblDiameter;
        private System.Windows.Forms.GroupBox grpUnits;
        private System.Windows.Forms.RadioButton rdoMetric;
        private System.Windows.Forms.RadioButton rdoEnglish;
        private System.Windows.Forms.CheckBox chkSubtractDrainHoles;
        private System.Windows.Forms.GroupBox grpPlacement;
        private System.Windows.Forms.CheckBox chkCorners;
        private System.Windows.Forms.CheckBox chkMidPoints;
        private System.Windows.Forms.GroupBox grpExtrusionLimits;
        private System.Windows.Forms.TextBox txtEndLimit;
        private System.Windows.Forms.TextBox txtStartLimit;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label lblStart;
    }
}