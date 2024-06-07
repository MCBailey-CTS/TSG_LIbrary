using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NXOpen.UF;

namespace TSG_Library.UFuncUtilities.BomUtilities
{
    public partial class CheckBoxDialog : Form
    {
        private static List<CheckBox> _checkBoxes;

        private static List<string> _strings;

        public CheckBoxDialog()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            foreach (var box in _checkBoxes)
                if (box.Checked)
                    _strings.Add(box.Text);
            Close();
        }

        public static string[] ShowBoxes(IEnumerable<string> purchased, string[] array, Point formLocation)
        {
            var dialog = new CheckBoxDialog
            {
                Location = formLocation,
                StartPosition = FormStartPosition.Manual
            };
            _strings = new List<string>();
            _checkBoxes = new List<CheckBox>();
            var i = 0;
            foreach (var str in purchased)
            {
                var box = new CheckBox { Size = new Size(80, 17), Location = new Point(12, 19 + 23 * i++), Text = str };
                foreach (var arr in array)
                    if (arr == str)
                        box.Checked = true;
                dialog.groupBox1.Controls.Add(box);
                _checkBoxes.Add(box);
            }

            UFSession.GetUFSession().Ui.SetPrompt("Please select shop vendors.....");
            dialog.ShowDialog();
            var rings = _strings.ToArray();
            dialog.Dispose();
            return rings;
        }
    }
}