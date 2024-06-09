using System;
using System.Linq;
using System.Windows.Forms;
using TSG_Library.Extensions;

namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public partial class Form : System.Windows.Forms.Form, IDrainHoleView
    {
        public Form(string settingsFile) : this(GetSettings(settingsFile))
        {
        }

        public Form(IDrainHoleSettings settings)
        {
            InitializeComponent();

            var creator = new DrainHoleCreator();

            var model = new DrainHoleModel(creator, settings);

            var presenter = new DrainHolePresenter(this, model);

            chkCorners.Checked = settings.Corners;
            chkMidPoints.Checked = settings.MidPoints;
            chkSubtractDrainHoles.Checked = settings.SubtractHoles;
            txtStartLimit.Text = $"{settings.ExtrusionStartLimit}";
            txtEndLimit.Text = $"{settings.ExtrusionEndLimit}";
            switch (settings.DiameterUnits)
            {
                case Units.Inches:
                    rdoEnglish.Checked = true;
                    break;
                case Units.Millimeters:
                    rdoMetric.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Form() : this("U:\\nxFiles\\UfuncFiles\\DrainHoleSettings.xml")
        {
        }

        public Form
        (
            Units defaultUnits,
            double defaultDiameter,
            bool defaultSubtractDrainHoles,
            bool defaultMidpoints,
            bool defaultCorners,
            double startLimit,
            double endLimit
        ) : this()
        {
            if(defaultUnits == Units.Millimeters)
                rdoMetric.Checked = true;
            else
                rdoEnglish.Checked = true;
            txtDiameterDrain.Text = $@"{defaultDiameter:F2}";
            txtStartLimit.Text = $@"{startLimit:F2}";
            txtEndLimit.Text = $@"{endLimit:F2}";
            chkCorners.Checked = defaultCorners;
            chkSubtractDrainHoles.Checked = defaultSubtractDrainHoles;
            chkMidPoints.Checked = defaultMidpoints;

            rdoEnglish.TabStop = false;
            rdoMetric.TabStop = false;
        }

        public event EventHandler MainButtonClicked;

        public event EventHandler<bool> MidpointsCheckChanged;

        public event EventHandler<bool> CornersCheckChanged;

        public event EventHandler<bool> SubtractCheckChanged;

        public event EventHandler<Units> UnitsChanged;

        public event EventHandler<double> DiameterChanged;

        public event EventHandler<double> StartLimitChanged;

        public event EventHandler<double> EndLimitChanged;

        public static void Execute1()
        {
            try
            {
                var drainModel = new DrainHoleModel
                {
                    DrainHoleCreator = new DrainHoleCreator(),
#if DEBUG
                    DrainHoleSettings = GetSettings("G:\\junk\\0Ufunc Testing\\DrainHoleSettings.xml")
#else
                    DrainHoleSettings = GetSettings("U:\\nxFiles\\UfuncFiles\\DrainHoleSettings.xml")
#endif
                };


                var form = new Form
                (
                    drainModel.DrainHoleSettings.DiameterUnits,
                    drainModel.DrainHoleSettings.Diameter,
                    drainModel.DrainHoleSettings.SubtractHoles,
                    drainModel.DrainHoleSettings.MidPoints,
                    drainModel.DrainHoleSettings.Corners,
                    drainModel.DrainHoleSettings.ExtrusionStartLimit,
                    drainModel.DrainHoleSettings.ExtrusionEndLimit
                );

                var presenter = new DrainHolePresenter(form, drainModel);


                form.Show();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public static IDrainHoleSettings GetSettings(string settingsFile)
        {
            return new DrainHoleSettings
            {
                Corners = true,
                CurveColor = 10,
                CurveLayer = 8,
                CurveName = "DRAIN_HOLE_CURVE",
                Diameter = 2,
                DiameterUnits = Units.Inches,
                ExtrusionEndLimit = -2,
                ExtrusionLayer = 1,
                ExtrusionName = "DRAIN_HOLE_EXTRUSION",
                ExtrusionStartLimit = .25,
                FeatureGroupName = "DRAIN_HOLE_FEATURE_GROUP",
                MidPoints = true,
                SubtractHoles = true,
                SubtractionLayer = 1,
                SubtractionName = "DRAIN_HOLE_SUBTRACTION"
            };
        }

        /// <summary>
        ///     This method just provides an easy way to determine whether or not the {DrainHoles} button should be enabled.
        /// </summary>
        private void ValidateDrainHoles()
        {
            btnDrainHoles.Enabled =
                new[] { txtEndLimit.Text, txtStartLimit.Text, txtDiameterDrain.Text }.All(value =>
                    double.TryParse(value, out _)) && (chkCorners.Checked || chkMidPoints.Checked);
        }


        private void ChkSubtractDrainHoles_CheckedChanged(object sender, EventArgs e)
        {
            SubtractCheckChanged?.Invoke(chkSubtractDrainHoles, chkSubtractDrainHoles.Checked);
        }

        private void BtnSelectFacesDrain_Click(object sender, EventArgs e)
        {
            try
            {
                // Hides the form so it doesn't obstruct the users view when selecting.
                Hide();

                MainButtonClicked?.Invoke(btnDrainHoles, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
                ValidateDrainHoles();
            }
        }

        private void ChkMidPoints_CheckedChanged(object sender, EventArgs e)
        {
            ValidateDrainHoles();

            MidpointsCheckChanged?.Invoke(chkMidPoints, chkMidPoints.Checked);
        }

        private void ChkCorners_CheckedChanged(object sender, EventArgs e)
        {
            ValidateDrainHoles();

            CornersCheckChanged?.Invoke(chkCorners, chkCorners.Checked);
        }

        private void RdoMetric_CheckedChanged(object sender, EventArgs e)
        {
            if(!rdoMetric.Checked) return;

            txtDiameterDrain.Text = $@"{50.00:F1}";

            UnitsChanged?.Invoke(rdoMetric, rdoMetric.Checked ? Units.Millimeters : Units.Inches);

            ValidateDrainHoles();
        }

        private void RdoEnglish_CheckedChanged(object sender, EventArgs e)
        {
            if(!rdoEnglish.Checked) return;

            txtDiameterDrain.Text = $@"{2.00:F1}";

            UnitsChanged?.Invoke(rdoEnglish, rdoMetric.Checked ? Units.Millimeters : Units.Inches);

            ValidateDrainHoles();
        }

        private void TxtDiameterDrain_TextChanged(object sender, EventArgs e)
        {
            if(double.TryParse(txtDiameterDrain.Text, out var result))
                DiameterChanged?.Invoke(txtDiameterDrain, result);

            ValidateDrainHoles();
        }

        private void TxtStartLimit_TextChanged(object sender, EventArgs e)
        {
            if(double.TryParse(txtStartLimit.Text, out var result)) StartLimitChanged?.Invoke(txtStartLimit, result);

            ValidateDrainHoles();
        }

        private void TxtEndLimit_TextChanged(object sender, EventArgs e)
        {
            if(double.TryParse(txtEndLimit.Text, out var result)) EndLimitChanged?.Invoke(txtEndLimit, result);

            ValidateDrainHoles();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode != Keys.Tab) return;
            if(e.Modifiers == Keys.Shift)
            {
                ProcessTabKey(false);
            }
            else
            {
                ProcessTabKey(true);
                if(ActiveControl is TextBox textBox)
                {
                    textBox.SelectionStart = 0;
                    textBox.SelectionLength = textBox.Text.Length;
                }
            }
        }
    }
}