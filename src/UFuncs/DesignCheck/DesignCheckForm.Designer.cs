using System.Windows.Forms;

namespace TSG_Library.UFuncs
{
    partial class DesignCheckForm
    {
        #region MainFormConstruction

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.IContainer components = null;

        /// <inheritdoc />
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._returnToAssembly = new System.Windows.Forms.ToolStripMenuItem();
            this._btnPrevious = new System.Windows.Forms.Button();
            this._btnNext = new System.Windows.Forms.Button();
            this._btnRerun = new System.Windows.Forms.Button();
            this._treeView = new System.Windows.Forms.TreeView();
            this._menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _menuStrip1
            // 
            this._menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fileToolStripMenuItem});
            this._menuStrip1.Location = new System.Drawing.Point(0, 0);
            this._menuStrip1.Name = "_menuStrip1";
            this._menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this._menuStrip1.Size = new System.Drawing.Size(544, 24);
            this._menuStrip1.TabIndex = 1;
            this._menuStrip1.Text = "menuStrip1";
            // 
            // _fileToolStripMenuItem
            // 
            this._fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._returnToAssembly});
            this._fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
            this._fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this._fileToolStripMenuItem.Text = "File";
            // 
            // _returnToAssembly
            // 
            this._returnToAssembly.Name = "_returnToAssembly";
            this._returnToAssembly.Size = new System.Drawing.Size(246, 22);
            this._returnToAssembly.Text = "Return to Original Displayed Part";
            // 
            // _btnPrevious
            // 
            this._btnPrevious.Location = new System.Drawing.Point(12, 27);
            this._btnPrevious.Name = "_btnPrevious";
            this._btnPrevious.Size = new System.Drawing.Size(75, 23);
            this._btnPrevious.TabIndex = 2;
            this._btnPrevious.Text = "Previous";
            this._btnPrevious.UseVisualStyleBackColor = true;
            // 
            // _btnNext
            // 
            this._btnNext.Location = new System.Drawing.Point(93, 27);
            this._btnNext.Name = "_btnNext";
            this._btnNext.Size = new System.Drawing.Size(75, 23);
            this._btnNext.TabIndex = 3;
            this._btnNext.Text = "Next";
            this._btnNext.UseVisualStyleBackColor = true;
            // 
            // _btnRerun
            // 
            this._btnRerun.Location = new System.Drawing.Point(174, 27);
            this._btnRerun.Name = "_btnRerun";
            this._btnRerun.Size = new System.Drawing.Size(156, 23);
            this._btnRerun.TabIndex = 5;
            this._btnRerun.Text = "Re-Run Checks";
            this._btnRerun.UseVisualStyleBackColor = true;
            // 
            // _treeView
            // 
            this._treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._treeView.HideSelection = false;
            this._treeView.Indent = 30;
            this._treeView.Location = new System.Drawing.Point(12, 56);
            this._treeView.Name = "_treeView";
            this._treeView.Size = new System.Drawing.Size(520, 397);
            this._treeView.Sorted = true;
            this._treeView.TabIndex = 4;
            this._treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._treeView_NodeMouseDoubleClick);
            // 
            // DesignCheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 465);
            this.Controls.Add(this._btnRerun);
            this.Controls.Add(this._treeView);
            this.Controls.Add(this._btnNext);
            this.Controls.Add(this._btnPrevious);
            this.Controls.Add(this._menuStrip1);
            this.MainMenuStrip = this._menuStrip1;
            this.MinimumSize = new System.Drawing.Size(356, 258);
            this.Name = "DesignCheckForm";
            this.Text = "Design Check Results";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DesignCheckForm_FormClosing);
            this.Load += new System.EventHandler(this.DesignCheckForm_Load);
            this._menuStrip1.ResumeLayout(false);
            this._menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private MenuStrip _menuStrip1;
        private ToolStripMenuItem _fileToolStripMenuItem;
        private ToolStripMenuItem _returnToAssembly;
        private System.Windows.Forms.Button _btnPrevious;
        private System.Windows.Forms.Button _btnNext;
        private TreeView _treeView;
        private System.Windows.Forms.Button _btnRerun;
#pragma warning disable CS0169 // The field 'DesignCheckForm._viewSortByValidatorMenuItem' is never used
        private ToolStripMenuItem _viewSortByValidatorMenuItem;
#pragma warning restore CS0169 // The field 'DesignCheckForm._viewSortByValidatorMenuItem' is never used

        #endregion


    }
}