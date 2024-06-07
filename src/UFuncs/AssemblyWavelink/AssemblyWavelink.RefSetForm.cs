using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using TSG_Library.Forms;
using Component = NXOpen.Assemblies.Component;

namespace TSG_Library.UFuncs
{
    public class AssemblyWavelink1
    {
        public class RefSetForm : FormCtsReferenceSet
        {
            private IContainer components;

            //public RefSetForm(IEnumerable<NXOpen.Assemblies.Component> components)
            //    : this(components.Select((component => component)))
            //{
            //}

            public RefSetForm(IEnumerable<Component> components)
                : base(components)
            {
                Load += RefSetForm_Load;
            }

            private void RefSetForm_Load(object sender, EventArgs e)
            {
                Location = Cursor.Position;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    components?.Dispose();
                base.Dispose(disposing);
            }

            // ReSharper disable once UnusedMember.Local
            private void InitializeComponent()
            {
                components = new Container();
                AutoScaleMode = AutoScaleMode.Font;
                Text = nameof(RefSetForm);
            }
        }
    }
}
// 1574