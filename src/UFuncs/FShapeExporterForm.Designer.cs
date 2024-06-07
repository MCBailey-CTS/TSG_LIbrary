namespace TSG_Library.UFuncs
{
    partial class FShapeExporterForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtOpNum = new System.Windows.Forms.TextBox();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sim Run#:";
            // 
            // txtOpNum
            // 
            this.txtOpNum.Location = new System.Drawing.Point(109, 7);
            this.txtOpNum.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOpNum.Name = "txtOpNum";
            this.txtOpNum.Size = new System.Drawing.Size(101, 22);
            this.txtOpNum.TabIndex = 1;
            this.txtOpNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtOpNum_KeyPress);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(69, 39);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(100, 28);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.BtnAccept_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(69, 75);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 28);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // FShapeExporterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 126);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.txtOpNum);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FShapeExporterForm";
            this.Text = "1919";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FShapeExporterForm_FormClosed);
            this.Load += new System.EventHandler(this.FShapeExporterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOpNum;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnTest;
    }
}