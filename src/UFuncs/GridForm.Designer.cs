namespace TSG_Library.UFuncs
{
    partial class GridForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        //internal void MoreDispose()
        //{
        //    Dispose(true);
        //}

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rdo6000 = new System.Windows.Forms.RadioButton();
            this.rdo1000 = new System.Windows.Forms.RadioButton();
            this.rdo0500 = new System.Windows.Forms.RadioButton();
            this.rdo0250 = new System.Windows.Forms.RadioButton();
            this.rdo0125 = new System.Windows.Forms.RadioButton();
            this.rdo0062 = new System.Windows.Forms.RadioButton();
            this.flwPanelGridRdos = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoGridOff = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.flwPanelMain = new System.Windows.Forms.FlowLayoutPanel();
            this.flwPanelGridBox = new System.Windows.Forms.FlowLayoutPanel();
            this.txtGrid = new System.Windows.Forms.TextBox();
            this.flwPanelSnapShow = new System.Windows.Forms.FlowLayoutPanel();
            this.chkShowGrid = new System.Windows.Forms.CheckBox();
            this.chkSnapToGrid = new System.Windows.Forms.CheckBox();
            this.flwPanelGridRdos.SuspendLayout();
            this.flwPanelMain.SuspendLayout();
            this.flwPanelGridBox.SuspendLayout();
            this.flwPanelSnapShow.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdo6000
            // 
            this.rdo6000.AutoSize = true;
            this.rdo6000.Location = new System.Drawing.Point(4, 144);
            this.rdo6000.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo6000.Name = "rdo6000";
            this.rdo6000.Size = new System.Drawing.Size(59, 20);
            this.rdo6000.TabIndex = 5;
            this.rdo6000.TabStop = true;
            this.rdo6000.Text = "6.000";
            this.rdo6000.UseVisualStyleBackColor = true;
            this.rdo6000.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // rdo1000
            // 
            this.rdo1000.AutoSize = true;
            this.rdo1000.Location = new System.Drawing.Point(4, 116);
            this.rdo1000.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo1000.Name = "rdo1000";
            this.rdo1000.Size = new System.Drawing.Size(59, 20);
            this.rdo1000.TabIndex = 4;
            this.rdo1000.TabStop = true;
            this.rdo1000.Text = "1.000";
            this.rdo1000.UseVisualStyleBackColor = true;
            this.rdo1000.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // rdo0500
            // 
            this.rdo0500.AutoSize = true;
            this.rdo0500.Location = new System.Drawing.Point(4, 88);
            this.rdo0500.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo0500.Name = "rdo0500";
            this.rdo0500.Size = new System.Drawing.Size(59, 20);
            this.rdo0500.TabIndex = 3;
            this.rdo0500.TabStop = true;
            this.rdo0500.Text = "0.500";
            this.rdo0500.UseVisualStyleBackColor = true;
            this.rdo0500.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // rdo0250
            // 
            this.rdo0250.AutoSize = true;
            this.rdo0250.Location = new System.Drawing.Point(4, 60);
            this.rdo0250.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo0250.Name = "rdo0250";
            this.rdo0250.Size = new System.Drawing.Size(59, 20);
            this.rdo0250.TabIndex = 2;
            this.rdo0250.TabStop = true;
            this.rdo0250.Text = "0.250";
            this.rdo0250.UseVisualStyleBackColor = true;
            this.rdo0250.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // rdo0125
            // 
            this.rdo0125.AutoSize = true;
            this.rdo0125.Location = new System.Drawing.Point(4, 32);
            this.rdo0125.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo0125.Name = "rdo0125";
            this.rdo0125.Size = new System.Drawing.Size(59, 20);
            this.rdo0125.TabIndex = 1;
            this.rdo0125.TabStop = true;
            this.rdo0125.Text = "0.125";
            this.rdo0125.UseVisualStyleBackColor = true;
            this.rdo0125.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // rdo0062
            // 
            this.rdo0062.AutoSize = true;
            this.rdo0062.Location = new System.Drawing.Point(4, 4);
            this.rdo0062.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdo0062.Name = "rdo0062";
            this.rdo0062.Size = new System.Drawing.Size(59, 20);
            this.rdo0062.TabIndex = 0;
            this.rdo0062.TabStop = true;
            this.rdo0062.Text = "0.062";
            this.rdo0062.UseVisualStyleBackColor = true;
            this.rdo0062.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // flwPanelGridRdos
            // 
            this.flwPanelGridRdos.AutoSize = true;
            this.flwPanelGridRdos.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flwPanelGridRdos.Controls.Add(this.rdo0062);
            this.flwPanelGridRdos.Controls.Add(this.rdo0125);
            this.flwPanelGridRdos.Controls.Add(this.rdo0250);
            this.flwPanelGridRdos.Controls.Add(this.rdo0500);
            this.flwPanelGridRdos.Controls.Add(this.rdo1000);
            this.flwPanelGridRdos.Controls.Add(this.rdo6000);
            this.flwPanelGridRdos.Controls.Add(this.rdoGridOff);
            this.flwPanelGridRdos.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwPanelGridRdos.Location = new System.Drawing.Point(40, 0);
            this.flwPanelGridRdos.Margin = new System.Windows.Forms.Padding(40, 0, 0, 0);
            this.flwPanelGridRdos.Name = "flwPanelGridRdos";
            this.flwPanelGridRdos.Size = new System.Drawing.Size(80, 196);
            this.flwPanelGridRdos.TabIndex = 4;
            // 
            // rdoGridOff
            // 
            this.rdoGridOff.AutoSize = true;
            this.rdoGridOff.Location = new System.Drawing.Point(4, 172);
            this.rdoGridOff.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdoGridOff.Name = "rdoGridOff";
            this.rdoGridOff.Size = new System.Drawing.Size(72, 20);
            this.rdoGridOff.TabIndex = 6;
            this.rdoGridOff.TabStop = true;
            this.rdoGridOff.Text = "Grid Off";
            this.rdoGridOff.UseVisualStyleBackColor = true;
            this.rdoGridOff.Click += new System.EventHandler(this.RdoIncrements_Click);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Location = new System.Drawing.Point(0, 196);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(0, 0);
            this.panel4.TabIndex = 5;
            // 
            // flwPanelMain
            // 
            this.flwPanelMain.AutoSize = true;
            this.flwPanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flwPanelMain.Controls.Add(this.flwPanelGridRdos);
            this.flwPanelMain.Controls.Add(this.panel4);
            this.flwPanelMain.Controls.Add(this.flwPanelGridBox);
            this.flwPanelMain.Controls.Add(this.flwPanelSnapShow);
            this.flwPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwPanelMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwPanelMain.Location = new System.Drawing.Point(0, 0);
            this.flwPanelMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flwPanelMain.Name = "flwPanelMain";
            this.flwPanelMain.Size = new System.Drawing.Size(477, 342);
            this.flwPanelMain.TabIndex = 6;
            // 
            // flwPanelGridBox
            // 
            this.flwPanelGridBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flwPanelGridBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flwPanelGridBox.Controls.Add(this.txtGrid);
            this.flwPanelGridBox.Location = new System.Drawing.Point(0, 196);
            this.flwPanelGridBox.Margin = new System.Windows.Forms.Padding(0);
            this.flwPanelGridBox.Name = "flwPanelGridBox";
            this.flwPanelGridBox.Size = new System.Drawing.Size(158, 34);
            this.flwPanelGridBox.TabIndex = 9;
            // 
            // txtGrid
            // 
            this.txtGrid.Location = new System.Drawing.Point(8, 4);
            this.txtGrid.Margin = new System.Windows.Forms.Padding(8, 4, 4, 4);
            this.txtGrid.Name = "txtGrid";
            this.txtGrid.Size = new System.Drawing.Size(135, 22);
            this.txtGrid.TabIndex = 10;
            this.txtGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtGrid_KeyDown);
            this.txtGrid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtGrid_KeyPress);
            this.txtGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TxtGrid_MouseUp);
            // 
            // flwPanelSnapShow
            // 
            this.flwPanelSnapShow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flwPanelSnapShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flwPanelSnapShow.Controls.Add(this.chkShowGrid);
            this.flwPanelSnapShow.Controls.Add(this.chkSnapToGrid);
            this.flwPanelSnapShow.Location = new System.Drawing.Point(0, 230);
            this.flwPanelSnapShow.Margin = new System.Windows.Forms.Padding(0);
            this.flwPanelSnapShow.Name = "flwPanelSnapShow";
            this.flwPanelSnapShow.Size = new System.Drawing.Size(158, 59);
            this.flwPanelSnapShow.TabIndex = 6;
            // 
            // chkShowGrid
            // 
            this.chkShowGrid.AutoSize = true;
            this.flwPanelSnapShow.SetFlowBreak(this.chkShowGrid, true);
            this.chkShowGrid.Location = new System.Drawing.Point(27, 4);
            this.chkShowGrid.Margin = new System.Windows.Forms.Padding(27, 4, 4, 4);
            this.chkShowGrid.Name = "chkShowGrid";
            this.chkShowGrid.Size = new System.Drawing.Size(90, 20);
            this.chkShowGrid.TabIndex = 7;
            this.chkShowGrid.Text = "Show Grid";
            this.chkShowGrid.UseVisualStyleBackColor = true;
            this.chkShowGrid.CheckedChanged += new System.EventHandler(this.ChkShowGrid_CheckedChanged);
            // 
            // chkSnapToGrid
            // 
            this.chkSnapToGrid.AutoSize = true;
            this.chkSnapToGrid.Location = new System.Drawing.Point(27, 32);
            this.chkSnapToGrid.Margin = new System.Windows.Forms.Padding(27, 4, 4, 4);
            this.chkSnapToGrid.Name = "chkSnapToGrid";
            this.chkSnapToGrid.Size = new System.Drawing.Size(109, 20);
            this.chkSnapToGrid.TabIndex = 8;
            this.chkSnapToGrid.Text = "Snap To Grid";
            this.chkSnapToGrid.UseVisualStyleBackColor = true;
            this.chkSnapToGrid.CheckedChanged += new System.EventHandler(this.ChkSnapToGrid_CheckedChanged);
            // 
            // GridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(477, 342);
            this.Controls.Add(this.flwPanelMain);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GridForm_FormClosing);
            this.Load += new System.EventHandler(this.GridForm_Load);
            this.flwPanelGridRdos.ResumeLayout(false);
            this.flwPanelGridRdos.PerformLayout();
            this.flwPanelMain.ResumeLayout(false);
            this.flwPanelMain.PerformLayout();
            this.flwPanelGridBox.ResumeLayout(false);
            this.flwPanelGridBox.PerformLayout();
            this.flwPanelSnapShow.ResumeLayout(false);
            this.flwPanelSnapShow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.RadioButton rdo6000;
        internal System.Windows.Forms.RadioButton rdo1000;
        internal System.Windows.Forms.RadioButton rdo0500;
        internal System.Windows.Forms.RadioButton rdo0250;
        internal System.Windows.Forms.RadioButton rdo0125;
        internal System.Windows.Forms.RadioButton rdo0062;
        private System.Windows.Forms.FlowLayoutPanel flwPanelGridRdos;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.FlowLayoutPanel flwPanelMain;
        internal System.Windows.Forms.RadioButton rdoGridOff;
        private System.Windows.Forms.FlowLayoutPanel flwPanelSnapShow;
        internal System.Windows.Forms.CheckBox chkShowGrid;
        internal System.Windows.Forms.CheckBox chkSnapToGrid;
        private System.Windows.Forms.FlowLayoutPanel flwPanelGridBox;
        internal System.Windows.Forms.TextBox txtGrid;
    }
}