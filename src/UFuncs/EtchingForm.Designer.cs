namespace TSG_Library.UFuncs
{
    partial class EtchingForm
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
            this.btnSingleSelection = new System.Windows.Forms.Button();
            this.btnSmartSelection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSingleSelection
            // 
            this.btnSingleSelection.Location = new System.Drawing.Point(16, 15);
            this.btnSingleSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSingleSelection.Name = "btnSingleSelection";
            this.btnSingleSelection.Size = new System.Drawing.Size(197, 28);
            this.btnSingleSelection.TabIndex = 0;
            this.btnSingleSelection.Text = "Single Selection";
            this.btnSingleSelection.UseVisualStyleBackColor = true;
            this.btnSingleSelection.Click += new System.EventHandler(this.btnSingleSelection_Click);
            // 
            // btnSmartSelection
            // 
            this.btnSmartSelection.Location = new System.Drawing.Point(221, 15);
            this.btnSmartSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSmartSelection.Name = "btnSmartSelection";
            this.btnSmartSelection.Size = new System.Drawing.Size(181, 28);
            this.btnSmartSelection.TabIndex = 1;
            this.btnSmartSelection.Text = "Smart Selection";
            this.btnSmartSelection.UseVisualStyleBackColor = true;
            this.btnSmartSelection.Click += new System.EventHandler(this.btnSmartSelection_Click);
            // 
            // EtchingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 57);
            this.Controls.Add(this.btnSmartSelection);
            this.Controls.Add(this.btnSingleSelection);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EtchingForm";
            this.Text = "1919";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSingleSelection;
        private System.Windows.Forms.Button btnSmartSelection;
    }
}