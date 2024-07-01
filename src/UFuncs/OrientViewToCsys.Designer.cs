using TSG_Library.UFuncs.Mirror;

namespace TSG_Library.UFuncs
{
    partial class OrientViewToCsys
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrientViewToCsys));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonViewBack = new System.Windows.Forms.Button();
            this.buttonViewFront = new System.Windows.Forms.Button();
            this.buttonViewLeft = new System.Windows.Forms.Button();
            this.buttonViewRight = new System.Windows.Forms.Button();
            this.buttonViewBtm = new System.Windows.Forms.Button();
            this.buttonViewTop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonViewBack
            // 
            this.buttonViewBack.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewBack.Image")));
            this.buttonViewBack.Location = new System.Drawing.Point(11, 47);
            this.buttonViewBack.Name = "buttonViewBack";
            this.buttonViewBack.Size = new System.Drawing.Size(30, 30);
            this.buttonViewBack.TabIndex = 5;
            this.buttonViewBack.UseVisualStyleBackColor = true;
            this.buttonViewBack.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // buttonViewFront
            // 
            this.buttonViewFront.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewFront.Image")));
            this.buttonViewFront.Location = new System.Drawing.Point(11, 11);
            this.buttonViewFront.Name = "buttonViewFront";
            this.buttonViewFront.Size = new System.Drawing.Size(30, 30);
            this.buttonViewFront.TabIndex = 4;
            this.buttonViewFront.UseVisualStyleBackColor = true;
            this.buttonViewFront.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // buttonViewLeft
            // 
            this.buttonViewLeft.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewLeft.Image")));
            this.buttonViewLeft.Location = new System.Drawing.Point(83, 47);
            this.buttonViewLeft.Name = "buttonViewLeft";
            this.buttonViewLeft.Size = new System.Drawing.Size(30, 30);
            this.buttonViewLeft.TabIndex = 3;
            this.buttonViewLeft.UseVisualStyleBackColor = true;
            this.buttonViewLeft.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // buttonViewRight
            // 
            this.buttonViewRight.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewRight.Image")));
            this.buttonViewRight.Location = new System.Drawing.Point(83, 11);
            this.buttonViewRight.Name = "buttonViewRight";
            this.buttonViewRight.Size = new System.Drawing.Size(30, 30);
            this.buttonViewRight.TabIndex = 2;
            this.buttonViewRight.UseVisualStyleBackColor = true;
            this.buttonViewRight.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // buttonViewBtm
            // 
            this.buttonViewBtm.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewBtm.Image")));
            this.buttonViewBtm.Location = new System.Drawing.Point(47, 47);
            this.buttonViewBtm.Name = "buttonViewBtm";
            this.buttonViewBtm.Size = new System.Drawing.Size(30, 30);
            this.buttonViewBtm.TabIndex = 1;
            this.buttonViewBtm.UseVisualStyleBackColor = true;
            this.buttonViewBtm.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // buttonViewTop
            // 
            this.buttonViewTop.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewTop.Image")));
            this.buttonViewTop.Location = new System.Drawing.Point(47, 11);
            this.buttonViewTop.Name = "buttonViewTop";
            this.buttonViewTop.Size = new System.Drawing.Size(30, 30);
            this.buttonViewTop.TabIndex = 0;
            this.buttonViewTop.UseVisualStyleBackColor = true;
            this.buttonViewTop.Click += new System.EventHandler(this.ViewButton_Clicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(124, 87);
            this.Controls.Add(this.buttonViewBack);
            this.Controls.Add(this.buttonViewFront);
            this.Controls.Add(this.buttonViewLeft);
            this.Controls.Add(this.buttonViewRight);
            this.Controls.Add(this.buttonViewBtm);
            this.Controls.Add(this.buttonViewTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(30, 130);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonViewTop;
        private System.Windows.Forms.Button buttonViewBtm;
        private System.Windows.Forms.Button buttonViewRight;
        private System.Windows.Forms.Button buttonViewLeft;
        private System.Windows.Forms.Button buttonViewBack;
        private System.Windows.Forms.Button buttonViewFront;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}