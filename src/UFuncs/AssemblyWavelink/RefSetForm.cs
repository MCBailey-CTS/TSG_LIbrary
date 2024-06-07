using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen.Assemblies;

namespace TSG_Library.Forms
{
    public partial class RefSetForm : FormCtsReferenceSet
    {
        public RefSetForm(IEnumerable<Component> components) : base(components)
        {
            Load += RefSetForm_Load;
        }


        //private void RefSetForm_MouseClick(object sender, MouseEventArgs e
        //    )
        //{
        //    if (e.Button == MouseButtons.Middle)
        //    {
        //        if (lstBox.SelectedIndex >= 0)
        //        {
        //            btnOk.PerformClick();
        //        }
        //    }

        //    //InfoWindow.WriteLine(e.Button);
        //}

        private void RefSetForm_Load(object sender, EventArgs e)
        {
            //Revision log 2.2.2 (6/1/2017)
            //Makes it so the form pops open to the current cursor location.
            Location = Cursor.Position;
        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // RefSetForm
        //    // 
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //    this.ClientSize = new System.Drawing.Size(183, 151);
        //    this.Name = "RefSetForm";
        //    this.Load += new System.EventHandler(this.RefSetForm_Load_1);
        //    this.ResumeLayout(false);

        //}

        //private void RefSetForm_Load_1(object sender, EventArgs e)
        //{

        //}
    }
}