using System;
using System.Windows.Forms;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    public partial class EditSizeForm : Form
    {
        public EditSizeForm()
        {
            InitializeComponent();
        }

        public EditSizeForm(double size)
        {
            InitializeComponent();
            CurrentValue = size;
        }

        public double InputValue { get; set; }

        public double CurrentValue { get; set; }

        private void EditSizeForm_Load(object sender, EventArgs e)
        {
            //Location = Settings.Default.EditSizeFormWindowLocation;
            Text = $@"Current Value = {CurrentValue:f3}, ({CurrentValue * 25.4:f3})mm";
            print_("//////////");
            print_($@"Current Value = {CurrentValue:f7}");
            print_($@"Current Value = {CurrentValue * 25.4:f7})mm");
            print_("//////////");
            textBoxInputValue.Focus();
        }

        private void textBoxInputValue_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Tab)) buttonOk.Focus();
            if(e.KeyCode.Equals(Keys.Return)) buttonOk.PerformClick();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //NXOpen.UF.UFSession.GetUFSession().Disp.set
            bool isConverted;
            double userValue;
            if(textBoxInputValue.Text.ToLower().Contains("mm"))
            {
                var trimValue = textBoxInputValue.Text.Substring(0, textBoxInputValue.Text.Length - 2);
                isConverted = double.TryParse(trimValue, out userValue);
                if(isConverted)
                {
                    InputValue = userValue / 25.4;
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                isConverted = double.TryParse(textBoxInputValue.Text, out userValue);
                if(isConverted)
                {
                    InputValue = userValue;
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}