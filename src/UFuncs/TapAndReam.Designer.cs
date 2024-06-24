namespace TSG_Library.UFuncs
{
    partial class TapAndReam
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
            this.buttonMoveToEnd = new System.Windows.Forms.Button();
            this.buttonSuppressTaps = new System.Windows.Forms.Button();
            this.buttonDeleteTaps = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSuppressReams = new System.Windows.Forms.Button();
            this.buttonDeleteReams = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonSuppressTapReam = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonDeleteTapsReams = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMoveToEnd
            // 
            this.buttonMoveToEnd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonMoveToEnd.Location = new System.Drawing.Point(8, 19);
            this.buttonMoveToEnd.Name = "buttonMoveToEnd";
            this.buttonMoveToEnd.Size = new System.Drawing.Size(155, 20);
            this.buttonMoveToEnd.TabIndex = 0;
            this.buttonMoveToEnd.Text = "Move Features to End";
            this.buttonMoveToEnd.UseVisualStyleBackColor = true;
            this.buttonMoveToEnd.Click += new System.EventHandler(this.ButtonMoveToEnd_Click);
            // 
            // buttonSuppressTaps
            // 
            this.buttonSuppressTaps.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSuppressTaps.Location = new System.Drawing.Point(8, 23);
            this.buttonSuppressTaps.Name = "buttonSuppressTaps";
            this.buttonSuppressTaps.Size = new System.Drawing.Size(155, 20);
            this.buttonSuppressTaps.TabIndex = 1;
            this.buttonSuppressTaps.Text = "Suppress Tap Features";
            this.buttonSuppressTaps.UseVisualStyleBackColor = true;
            this.buttonSuppressTaps.Click += new System.EventHandler(this.ButtonSuppressTaps_Click);
            // 
            // buttonDeleteTaps
            // 
            this.buttonDeleteTaps.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDeleteTaps.Location = new System.Drawing.Point(8, 23);
            this.buttonDeleteTaps.Name = "buttonDeleteTaps";
            this.buttonDeleteTaps.Size = new System.Drawing.Size(155, 20);
            this.buttonDeleteTaps.TabIndex = 2;
            this.buttonDeleteTaps.Text = "Delete Tap Features";
            this.buttonDeleteTaps.UseVisualStyleBackColor = true;
            this.buttonDeleteTaps.Click += new System.EventHandler(this.ButtonDeleteTaps_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(20, 290);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(155, 20);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonSuppressReams
            // 
            this.buttonSuppressReams.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSuppressReams.Location = new System.Drawing.Point(8, 49);
            this.buttonSuppressReams.Name = "buttonSuppressReams";
            this.buttonSuppressReams.Size = new System.Drawing.Size(155, 20);
            this.buttonSuppressReams.TabIndex = 6;
            this.buttonSuppressReams.Text = "Suppress Ream Features";
            this.buttonSuppressReams.UseVisualStyleBackColor = true;
            this.buttonSuppressReams.Click += new System.EventHandler(this.ButtonSuppressReams_Click);
            // 
            // buttonDeleteReams
            // 
            this.buttonDeleteReams.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDeleteReams.Location = new System.Drawing.Point(8, 49);
            this.buttonDeleteReams.Name = "buttonDeleteReams";
            this.buttonDeleteReams.Size = new System.Drawing.Size(155, 20);
            this.buttonDeleteReams.TabIndex = 7;
            this.buttonDeleteReams.Text = "Delete Ream Features";
            this.buttonDeleteReams.UseVisualStyleBackColor = true;
            this.buttonDeleteReams.Click += new System.EventHandler(this.ButtonDeleteReams_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonMoveToEnd);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(170, 50);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Re-Order Taps and Reams";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonSuppressTapReam);
            this.groupBox2.Controls.Add(this.buttonSuppressTaps);
            this.groupBox2.Controls.Add(this.buttonSuppressReams);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(12, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(170, 105);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Suppress Features";
            // 
            // buttonSuppressTapReam
            // 
            this.buttonSuppressTapReam.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSuppressTapReam.Location = new System.Drawing.Point(8, 75);
            this.buttonSuppressTapReam.Name = "buttonSuppressTapReam";
            this.buttonSuppressTapReam.Size = new System.Drawing.Size(155, 20);
            this.buttonSuppressTapReam.TabIndex = 7;
            this.buttonSuppressTapReam.Text = "Suppress Taps and Reams";
            this.buttonSuppressTapReam.UseVisualStyleBackColor = true;
            this.buttonSuppressTapReam.Click += new System.EventHandler(this.ButtonSuppressTapReam_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonDeleteTapsReams);
            this.groupBox3.Controls.Add(this.buttonDeleteTaps);
            this.groupBox3.Controls.Add(this.buttonDeleteReams);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(12, 179);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(170, 105);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Delete Features";
            // 
            // buttonDeleteTapsReams
            // 
            this.buttonDeleteTapsReams.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDeleteTapsReams.Location = new System.Drawing.Point(8, 75);
            this.buttonDeleteTapsReams.Name = "buttonDeleteTapsReams";
            this.buttonDeleteTapsReams.Size = new System.Drawing.Size(155, 20);
            this.buttonDeleteTapsReams.TabIndex = 8;
            this.buttonDeleteTapsReams.Text = "Delete Taps and Reams";
            this.buttonDeleteTapsReams.UseVisualStyleBackColor = true;
            this.buttonDeleteTapsReams.Click += new System.EventHandler(this.ButtonDeleteTapsReams_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 320);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TapReamControl";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonMoveToEnd;
        private System.Windows.Forms.Button buttonSuppressTaps;
        private System.Windows.Forms.Button buttonDeleteTaps;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSuppressReams;
        private System.Windows.Forms.Button buttonDeleteReams;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonSuppressTapReam;
        private System.Windows.Forms.Button buttonDeleteTapsReams;
    }
}