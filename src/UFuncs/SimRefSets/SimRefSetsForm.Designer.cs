namespace TSG_Library.UFuncs
{
    partial class SimRefSetsForm
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
            this.btnAddTo = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnAddTo
            // 
            this.btnAddTo.Location = new System.Drawing.Point(68, 15);
            this.btnAddTo.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddTo.Name = "btnAddTo";
            this.btnAddTo.Size = new System.Drawing.Size(100, 28);
            this.btnAddTo.TabIndex = 6;
            this.btnAddTo.Text = "Add To";
            this.btnAddTo.UseVisualStyleBackColor = true;
            this.btnAddTo.Click += new System.EventHandler(this.Button_Selected);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(68, 50);
            this.btnAddAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(100, 28);
            this.btnAddAll.TabIndex = 7;
            this.btnAddAll.Text = "Add All";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.Button_Selected);
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Location = new System.Drawing.Point(61, 86);
            this.chkDelete.Margin = new System.Windows.Forms.Padding(4);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(118, 20);
            this.chkDelete.TabIndex = 8;
            this.chkDelete.Text = "Delete Refsets";
            this.chkDelete.UseVisualStyleBackColor = true;
            // 
            // SimRefSetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 118);
            this.Controls.Add(this.chkDelete);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.btnAddTo);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(262, 165);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(262, 165);
            this.Name = "SimRefSetsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SimRefSetsForm_FormClosed);
            this.Load += new System.EventHandler(this.SimRefSetsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddTo;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.CheckBox chkDelete;
    }
}