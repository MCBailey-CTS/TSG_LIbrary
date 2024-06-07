using System.Drawing;
using System.Windows.Forms;

namespace TSG_Library.UFuncs.AssemblyWavelink
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
            this.buttonTapReam = new System.Windows.Forms.Button();
            this.buttonShortTapReam = new System.Windows.Forms.Button();
            this.buttonTap = new System.Windows.Forms.Button();
            this.buttonReam = new System.Windows.Forms.Button();
            this.buttonShortTap = new System.Windows.Forms.Button();
            this.buttonReamShort = new System.Windows.Forms.Button();
            this.buttonClrHole = new System.Windows.Forms.Button();
            this.buttonSubTool = new System.Windows.Forms.Button();
            this.buttonCbore = new System.Windows.Forms.Button();
            this.buttonSubtract = new System.Windows.Forms.Button();
            this.buttonUnite = new System.Windows.Forms.Button();
            this.buttonIntersect = new System.Windows.Forms.Button();
            this.grpBoxBooleans = new System.Windows.Forms.GroupBox();
            this.buttonLink = new System.Windows.Forms.Button();
            this.grpBoxFasteners = new System.Windows.Forms.GroupBox();
            this.chkBlankTools = new System.Windows.Forms.CheckBox();
            this.grpBoxBooleans.SuspendLayout();
            this.grpBoxFasteners.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonTapReam
            // 
            this.buttonTapReam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTapReam.Location = new System.Drawing.Point(6, 48);
            this.buttonTapReam.Name = "buttonTapReam";
            this.buttonTapReam.Size = new System.Drawing.Size(152, 23);
            this.buttonTapReam.TabIndex = 0;
            this.buttonTapReam.Text = "Tap && Ream";
            this.buttonTapReam.UseVisualStyleBackColor = true;
            this.buttonTapReam.Click += new System.EventHandler(this.buttonTapReam_Click);
            // 
            // buttonShortTapReam
            // 
            this.buttonShortTapReam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonShortTapReam.Location = new System.Drawing.Point(6, 19);
            this.buttonShortTapReam.Name = "buttonShortTapReam";
            this.buttonShortTapReam.Size = new System.Drawing.Size(152, 23);
            this.buttonShortTapReam.TabIndex = 1;
            this.buttonShortTapReam.Text = "Short - Tap && Ream";
            this.buttonShortTapReam.UseVisualStyleBackColor = true;
            this.buttonShortTapReam.Click += new System.EventHandler(this.buttonShortTapReam_Click);
            // 
            // buttonTap
            // 
            this.buttonTap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTap.Location = new System.Drawing.Point(6, 77);
            this.buttonTap.Name = "buttonTap";
            this.buttonTap.Size = new System.Drawing.Size(73, 23);
            this.buttonTap.TabIndex = 2;
            this.buttonTap.Text = "Tap";
            this.buttonTap.UseVisualStyleBackColor = true;
            this.buttonTap.Click += new System.EventHandler(this.buttonTap_Click);
            // 
            // buttonReam
            // 
            this.buttonReam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReam.Location = new System.Drawing.Point(85, 77);
            this.buttonReam.Name = "buttonReam";
            this.buttonReam.Size = new System.Drawing.Size(73, 23);
            this.buttonReam.TabIndex = 3;
            this.buttonReam.Text = "Ream";
            this.buttonReam.UseVisualStyleBackColor = true;
            this.buttonReam.Click += new System.EventHandler(this.buttonReam_Click);
            // 
            // buttonShortTap
            // 
            this.buttonShortTap.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonShortTap.Location = new System.Drawing.Point(6, 106);
            this.buttonShortTap.Name = "buttonShortTap";
            this.buttonShortTap.Size = new System.Drawing.Size(73, 23);
            this.buttonShortTap.TabIndex = 4;
            this.buttonShortTap.Text = "Short-Tap";
            this.buttonShortTap.UseVisualStyleBackColor = true;
            this.buttonShortTap.Click += new System.EventHandler(this.buttonShortTap_Click);
            // 
            // buttonReamShort
            // 
            this.buttonReamShort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonReamShort.Location = new System.Drawing.Point(85, 106);
            this.buttonReamShort.Name = "buttonReamShort";
            this.buttonReamShort.Size = new System.Drawing.Size(73, 23);
            this.buttonReamShort.TabIndex = 5;
            this.buttonReamShort.Text = "Short-Ream";
            this.buttonReamShort.UseVisualStyleBackColor = true;
            this.buttonReamShort.Click += new System.EventHandler(this.buttonReamShort_Click);
            // 
            // buttonClrHole
            // 
            this.buttonClrHole.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClrHole.Location = new System.Drawing.Point(6, 135);
            this.buttonClrHole.Name = "buttonClrHole";
            this.buttonClrHole.Size = new System.Drawing.Size(73, 23);
            this.buttonClrHole.TabIndex = 6;
            this.buttonClrHole.Text = "Clr-Hole";
            this.buttonClrHole.UseVisualStyleBackColor = true;
            this.buttonClrHole.Click += new System.EventHandler(this.buttonClrHole_Click);
            // 
            // buttonSubTool
            // 
            this.buttonSubTool.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubTool.Location = new System.Drawing.Point(6, 19);
            this.buttonSubTool.Name = "buttonSubTool";
            this.buttonSubTool.Size = new System.Drawing.Size(152, 23);
            this.buttonSubTool.TabIndex = 7;
            this.buttonSubTool.Text = "Sub-Tool";
            this.buttonSubTool.UseVisualStyleBackColor = true;
            this.buttonSubTool.Click += new System.EventHandler(this.buttonSubTool_Click);
            // 
            // buttonCbore
            // 
            this.buttonCbore.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCbore.Location = new System.Drawing.Point(85, 135);
            this.buttonCbore.Name = "buttonCbore";
            this.buttonCbore.Size = new System.Drawing.Size(73, 23);
            this.buttonCbore.TabIndex = 8;
            this.buttonCbore.Text = "Cbore";
            this.buttonCbore.UseVisualStyleBackColor = true;
            this.buttonCbore.Click += new System.EventHandler(this.buttonCbore_Click);
            // 
            // buttonSubtract
            // 
            this.buttonSubtract.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSubtract.Location = new System.Drawing.Point(6, 48);
            this.buttonSubtract.Name = "buttonSubtract";
            this.buttonSubtract.Size = new System.Drawing.Size(73, 23);
            this.buttonSubtract.TabIndex = 9;
            this.buttonSubtract.Text = "Subtract";
            this.buttonSubtract.UseVisualStyleBackColor = true;
            this.buttonSubtract.Click += new System.EventHandler(this.buttonSubtract_Click);
            // 
            // buttonUnite
            // 
            this.buttonUnite.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonUnite.Location = new System.Drawing.Point(85, 48);
            this.buttonUnite.Name = "buttonUnite";
            this.buttonUnite.Size = new System.Drawing.Size(73, 23);
            this.buttonUnite.TabIndex = 10;
            this.buttonUnite.Text = "Unite";
            this.buttonUnite.UseVisualStyleBackColor = true;
            this.buttonUnite.Click += new System.EventHandler(this.buttonUnite_Click);
            // 
            // buttonIntersect
            // 
            this.buttonIntersect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonIntersect.Location = new System.Drawing.Point(6, 77);
            this.buttonIntersect.Name = "buttonIntersect";
            this.buttonIntersect.Size = new System.Drawing.Size(73, 23);
            this.buttonIntersect.TabIndex = 11;
            this.buttonIntersect.Text = "Intersect";
            this.buttonIntersect.UseVisualStyleBackColor = true;
            this.buttonIntersect.Click += new System.EventHandler(this.buttonIntersect_Click);
            // 
            // grpBoxBooleans
            // 
            this.grpBoxBooleans.Controls.Add(this.buttonLink);
            this.grpBoxBooleans.Controls.Add(this.buttonSubtract);
            this.grpBoxBooleans.Controls.Add(this.buttonIntersect);
            this.grpBoxBooleans.Controls.Add(this.buttonUnite);
            this.grpBoxBooleans.Controls.Add(this.buttonSubTool);
            this.grpBoxBooleans.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grpBoxBooleans.Location = new System.Drawing.Point(12, 201);
            this.grpBoxBooleans.Name = "grpBoxBooleans";
            this.grpBoxBooleans.Size = new System.Drawing.Size(164, 110);
            this.grpBoxBooleans.TabIndex = 12;
            this.grpBoxBooleans.TabStop = false;
            this.grpBoxBooleans.Text = "Booleans";
            // 
            // buttonLink
            // 
            this.buttonLink.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLink.Location = new System.Drawing.Point(85, 77);
            this.buttonLink.Name = "buttonLink";
            this.buttonLink.Size = new System.Drawing.Size(73, 23);
            this.buttonLink.TabIndex = 12;
            this.buttonLink.Text = "Link";
            this.buttonLink.UseVisualStyleBackColor = true;
            this.buttonLink.Click += new System.EventHandler(this.buttonLink_Click);
            // 
            // grpBoxFasteners
            // 
            this.grpBoxFasteners.Controls.Add(this.buttonShortTapReam);
            this.grpBoxFasteners.Controls.Add(this.buttonTapReam);
            this.grpBoxFasteners.Controls.Add(this.buttonTap);
            this.grpBoxFasteners.Controls.Add(this.buttonCbore);
            this.grpBoxFasteners.Controls.Add(this.buttonReam);
            this.grpBoxFasteners.Controls.Add(this.buttonClrHole);
            this.grpBoxFasteners.Controls.Add(this.buttonShortTap);
            this.grpBoxFasteners.Controls.Add(this.buttonReamShort);
            this.grpBoxFasteners.Location = new System.Drawing.Point(12, 25);
            this.grpBoxFasteners.Name = "grpBoxFasteners";
            this.grpBoxFasteners.Size = new System.Drawing.Size(164, 170);
            this.grpBoxFasteners.TabIndex = 15;
            this.grpBoxFasteners.TabStop = false;
            this.grpBoxFasteners.Text = "Fasteners";
            // 
            // chkBlankTools
            // 
            this.chkBlankTools.AutoSize = true;
            this.chkBlankTools.Location = new System.Drawing.Point(18, 317);
            this.chkBlankTools.Name = "chkBlankTools";
            this.chkBlankTools.Size = new System.Drawing.Size(139, 17);
            this.chkBlankTools.TabIndex = 17;
            this.chkBlankTools.Text = "Blank Tool Components";
            this.chkBlankTools.UseVisualStyleBackColor = true;
            // 
            // AssemblyWavelink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 346);
            this.Controls.Add(this.chkBlankTools);
            this.Controls.Add(this.grpBoxFasteners);
            this.Controls.Add(this.grpBoxBooleans);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.Name = "AssemblyWavelink";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "AWL";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AssemblyWavelink_FormClosed);
            this.Load += new System.EventHandler(this.AssemblyWavelink_Load);
            this.grpBoxBooleans.ResumeLayout(false);
            this.grpBoxFasteners.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button buttonTapReam;
        private Button buttonShortTapReam;
        private Button buttonTap;
        private Button buttonReam;
        private Button buttonShortTap;
        private Button buttonReamShort;
        private Button buttonClrHole;
        private Button buttonSubTool;
        private Button buttonCbore;
        private Button buttonSubtract;
        private Button buttonUnite;
        private Button buttonIntersect;
        private GroupBox grpBoxBooleans;
        private Button buttonLink;
        private GroupBox grpBoxFasteners;
        private CheckBox chkBlankTools;
        #endregion
    }
}