using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TSG_Library.Utilities
{
    public partial class SimDataDeleteConfirm : Form
    {
        public SimDataDeleteConfirm()
        {
            InitializeComponent();
        }

        public SimDataDeleteConfirm(IEnumerable<string> filesToDisplay) : this()
        {
            foreach (string file in filesToDisplay)
                simTreeView.Nodes.Add(file);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}