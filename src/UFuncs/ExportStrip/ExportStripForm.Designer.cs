namespace TSG_Library.UFuncs
{
    partial class ExportStripForm
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
            this.chkSTP = new System.Windows.Forms.CheckBox();
            this.chkPDF = new System.Windows.Forms.CheckBox();
            this.chkPart = new System.Windows.Forms.CheckBox();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.numUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkCopy = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // chkSTP
            // 
            this.chkSTP.AutoSize = true;
            this.chkSTP.Checked = true;
            this.chkSTP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSTP.Location = new System.Drawing.Point(16, 43);
            this.chkSTP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkSTP.Name = "chkSTP";
            this.chkSTP.Size = new System.Drawing.Size(97, 20);
            this.chkSTP.TabIndex = 0;
            this.chkSTP.Text = "Export STP";
            this.chkSTP.UseVisualStyleBackColor = true;
            this.chkSTP.CheckedChanged += new System.EventHandler(this.ChkBoxes_CheckedChanged);
            // 
            // chkPDF
            // 
            this.chkPDF.AutoSize = true;
            this.chkPDF.Checked = true;
            this.chkPDF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPDF.Location = new System.Drawing.Point(16, 71);
            this.chkPDF.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPDF.Name = "chkPDF";
            this.chkPDF.Size = new System.Drawing.Size(97, 20);
            this.chkPDF.TabIndex = 2;
            this.chkPDF.Text = "Export PDF";
            this.chkPDF.UseVisualStyleBackColor = true;
            this.chkPDF.CheckedChanged += new System.EventHandler(this.ChkBoxes_CheckedChanged);
            // 
            // chkPart
            // 
            this.chkPart.AutoSize = true;
            this.chkPart.Checked = true;
            this.chkPart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPart.Location = new System.Drawing.Point(16, 15);
            this.chkPart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkPart.Name = "chkPart";
            this.chkPart.Size = new System.Drawing.Size(144, 20);
            this.chkPart.TabIndex = 4;
            this.chkPart.Text = "Export NX Part (.prt)";
            this.chkPart.UseVisualStyleBackColor = true;
            this.chkPart.CheckedChanged += new System.EventHandler(this.ChkBoxes_CheckedChanged);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(12, 182);
            this.txtInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(213, 22);
            this.txtInput.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 158);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Folder Name:";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(16, 260);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(211, 28);
            this.btnAccept.TabIndex = 7;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.BtnAccept_Click);
            // 
            // numUpDownCopies
            // 
            this.numUpDownCopies.Location = new System.Drawing.Point(108, 122);
            this.numUpDownCopies.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numUpDownCopies.Name = "numUpDownCopies";
            this.numUpDownCopies.Size = new System.Drawing.Size(119, 22);
            this.numUpDownCopies.TabIndex = 8;
            this.numUpDownCopies.ValueChanged += new System.EventHandler(this.NumUpDownCopies_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 124);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Print Copies";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 239);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(209, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "\\Process and Sim Data for Design";
            // 
            // chkCopy
            // 
            this.chkCopy.AutoSize = true;
            this.chkCopy.Location = new System.Drawing.Point(16, 214);
            this.chkCopy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkCopy.Name = "chkCopy";
            this.chkCopy.Size = new System.Drawing.Size(81, 20);
            this.chkCopy.TabIndex = 10;
            this.chkCopy.Text = "Copy to :";
            this.chkCopy.UseVisualStyleBackColor = true;
            // 
            // ExportStripForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 297);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkCopy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numUpDownCopies);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.chkPart);
            this.Controls.Add(this.chkPDF);
            this.Controls.Add(this.chkSTP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportStripForm";
            this.Text = "1919";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportStripForm_FormClosed);
            this.Load += new System.EventHandler(this.ExportStripForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownCopies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSTP;
        private System.Windows.Forms.CheckBox chkPDF;
        private System.Windows.Forms.CheckBox chkPart;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.NumericUpDown numUpDownCopies;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkCopy;
    }
}