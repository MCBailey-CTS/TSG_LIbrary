using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class SimplifyCurves : _UFuncForm
    {
        public SimplifyCurves() 
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set window size
            Location = Properties.Settings.Default.simplify_curves_window_location;
            Text = AssemblyFileVersion;

            comboBoxTolerance.Items.Clear();

            ufsession_.Modl.AskDistanceTolerance(out double systemTolerance);

            comboBoxTolerance.Items.Add(systemTolerance.ToString());

            switch (__work_part_.PartUnits)
            {
                case NXOpen.BasePart.Units.Inches:
                    comboBoxTolerance.Items.Add("0.002");
                    comboBoxTolerance.Items.Add("0.003");
                    comboBoxTolerance.Items.Add("0.004");
                    comboBoxTolerance.Items.Add("0.005");
                    break;
                case NXOpen.BasePart.Units.Millimeters:
                    comboBoxTolerance.Items.Add(systemTolerance.ToString());
                    comboBoxTolerance.Items.Add("0.0508");
                    comboBoxTolerance.Items.Add("0.0762");
                    comboBoxTolerance.Items.Add("0.1016");
                    comboBoxTolerance.Items.Add("0.127");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            comboBoxTolerance.SelectedIndex = 4;
        }


        private void ComboBoxTolerance_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            Hide();

            NXOpen.Tag[] curvesToSimplify = Ui.Selection.SelectSplines().Select(spline => spline.Tag).ToArray();

            Show();

            if (curvesToSimplify.Length == 0) return;

            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Simplify");

                double tolerance = double.Parse(comboBoxTolerance.Text);

                ufsession_.Curve.CreateSimplifiedCurve(curvesToSimplify.Length, curvesToSimplify, tolerance, out _, out _);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void SimplifyCurves_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.simplify_curves_window_location = Location;
            Properties.Settings.Default.Save();
        }
    }
}
