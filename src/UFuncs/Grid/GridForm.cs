using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MoreLinq;
using NXOpen;
using NXOpen.Preferences;
using TSG_Library.Attributes;
using static TSG_Library.Extensions.__Extensions_;
using Part = NXOpen.Part;

namespace TSG_Library.UFuncs
{
    [RevisionLog("Grd Stand Alone")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("1.2", "2018", "01", "24")]
    [Revision("1.2.1",
        "No longer reads current work part grid (work plane) values, but defaults to grid off and show grid off.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class GridForm : Form
    {
        public enum CurrentCheckedRdo
        {
            Size0062,
            Size0125,
            Size0250,
            Size0500,
            Size1000,
            Size6000,
            None
        }

        //language=regexp
        public const string Reg = "^([0-9]{0,}.?[0-9]{0,})(in|mm)?$";

        private int _workPartChangeRegister;

        public GridForm()
        {
            InitializeComponent();
        }

        private static WorkPlane WorkPlane => __display_part_.Preferences.Workplane;

        internal CurrentCheckedRdo CurrentCheckedSize
        {
            get
            {
                if(rdo0062.Checked) return CurrentCheckedRdo.Size0062;
                if(rdo0125.Checked) return CurrentCheckedRdo.Size0125;
                if(rdo0250.Checked) return CurrentCheckedRdo.Size0250;
                if(rdo0500.Checked) return CurrentCheckedRdo.Size0500;
                if(rdo1000.Checked) return CurrentCheckedRdo.Size1000;
                return rdo6000.Checked ? CurrentCheckedRdo.Size6000 : CurrentCheckedRdo.None;
            }
        }

        private void GridForm_Load(object sender, EventArgs e)
        {
            _workPartChangeRegister = Session.GetSession().Parts.AddWorkPartChangedHandler(WorkPartChanged);

            TurnGridOff();
        }

        public void TurnGridOff()
        {
            rdoGridOff.Click -= RdoIncrements_Click;
            rdoGridOff.Checked = true;
            rdoGridOff.Click += RdoIncrements_Click;
            WorkPlane.SnapToGrid = false;
            WorkPlane.ShowGrid = false;
            WorkPlane.ShowLabels = false;
            WorkPlane.GridOnTop = false;
            WorkPlane.RectangularShowMajorLines = false;
            WorkPlane.PolarShowMajorLines = false;
            WorkPlane.GridIsNonUniform = false;
            txtGrid.Text = @"Grid Off";
            chkShowGrid.Checked = false;
            chkShowGrid.Enabled = false;
            chkSnapToGrid.Checked = false;
            chkSnapToGrid.Enabled = false;
        }

        private void WorkPartChanged(BasePart part)
        {
            if(Session.GetSession().Parts.Display is null)
                return;

            TurnGridOff();
        }

        internal void SetFormToGrid()
        {
            if(Session.GetSession().Parts.Display is null) return;
            var gridSize = WorkPlane.GetRectangularUniformGridSize().MajorGridSpacing;
            var multiplier = __display_part_.PartUnits == BasePart.Units.Inches ? 1.0 : 25.4;
            if(!rdoGridOff.Checked)
            {
                chkShowGrid.Checked = true;
                chkSnapToGrid.Checked = true;
                chkShowGrid.Enabled = true;
                chkSnapToGrid.Enabled = true;
            }

            const double tolerance = .0001;
            new[] { rdo0062, rdo0125, rdo0250, rdo0500, rdo1000, rdo6000 }.ToList()
                .ForEach(button => button.Checked = false);

            bool Predicate(double number)
            {
                return Math.Abs(gridSize / multiplier - number) < tolerance;
            }

            void Action(string size, RadioButton rdoButton)
            {
                txtGrid.Text = size;
                rdoButton.Checked = true;
            }

            if(Predicate(Grid.GridSize0062))
                Action(@"0.0625", rdo0062);
            else if(Predicate(Grid.GridSize0125))
                Action(@"0.125", rdo0125);
            else if(Predicate(Grid.GridSize0250))
                Action(@"0.250", rdo0250);
            else if(Predicate(Grid.GridSize0500))
                Action(@"0.500", rdo0500);
            else if(Predicate(Grid.GridSize1000))
                Action(@"1.000", rdo1000);
            else if(Predicate(Grid.GridSize6000))
                Action(@"6.000", rdo6000);
            else
                txtGrid.Text = "" + gridSize / multiplier;
        }

        internal void ResetForm()
        {
            Controls.OfType<RadioButton>().ToList().ForEach(button => button.Checked = false);
            Controls.OfType<CheckBox>().ToList().ForEach(button => button.Checked = false);
            txtGrid.Text = "";
        }

        private void RdoIncrements_Click(object sender, EventArgs e)
        {
            if(Session.GetSession().Parts.Display is null)
                return;

            new[] { chkShowGrid, chkSnapToGrid }.Pipe(box => box.Enabled = !rdoGridOff.Checked)
                .ForEach(box => box.Checked = !rdoGridOff.Checked);
            if(rdoGridOff.Checked)
            {
                txtGrid.Text = @"Grid Off";
                return;
            }

            WorkPlane.SnapToGrid = chkSnapToGrid.Checked;
            WorkPlane.ShowGrid = chkShowGrid.Checked;
            var multiplier = __display_part_.PartUnits == BasePart.Units.Inches ? 1.0 : 25.4;
            double gridSize;
            if(sender == rdo0062)
            {
                gridSize = Grid.GridSize0062;
                txtGrid.Text = @"0.0625";
            }
            else if(sender == rdo0125)
            {
                gridSize = Grid.GridSize0125;
                txtGrid.Text = @"0.125";
            }
            else if(sender == rdo0250)
            {
                gridSize = Grid.GridSize0250;
                txtGrid.Text = @"0.250";
            }
            else if(sender == rdo0500)
            {
                gridSize = Grid.GridSize0500;
                txtGrid.Text = @"0.500";
            }
            else if(sender == rdo1000)
            {
                gridSize = Grid.GridSize1000;
                txtGrid.Text = @"1.000";
            }
            else if(sender == rdo6000)
            {
                gridSize = Grid.GridSize6000;
                txtGrid.Text = @"6.000";
            }
            else
            {
                print_("Method " + "\"RdoIncrements_Click\" was clicked");
                return;
            }

            WorkPlane.SetRectangularUniformGridSize(new WorkPlane.GridSize(gridSize * multiplier, 1, 1));
        }

        private void ChkShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            WorkPlane.ShowGrid = chkShowGrid.Checked;
        }

        private void ChkSnapToGrid_CheckedChanged(object sender, EventArgs e)
        {
            WorkPlane.SnapToGrid = chkSnapToGrid.Checked;
        }

        public void TxtGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            var toLower = char.ToLower(e.KeyChar);
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && toLower != 'i' &&
               toLower != 'n' && toLower != 'm')
                e.Handled = true;
        }

        public void TxtGrid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter && IsValidString(txtGrid.Text))
                    SetGridToTextBox();
            }
            catch (Exception ex)
            {
                print_(ex.Message);
            }
        }

        internal void SetGridToTextBox()
        {
            if(!txtGrid.Text.ToCharArray().Any(char.IsDigit)) throw new FormatException("Invalid Format in TextBox.");
            var expression = txtGrid.Text.ToLower().Replace(" ", "");
            var regex = new Regex("^([0-9]{0,}.?[0-9]{0,})(in|mm)?$");
            var match = regex.Match(expression);
            if(!match.Success) throw new FormatException("Invalid Format in TextBox.");
            var number = double.Parse(match.Groups[1].Value);
            var units = expression.EndsWith("mm") ? Part.Units.Millimeters : Part.Units.Inches;
            var displayUnits = __display_part_.PartUnits;
            var convertedValue = displayUnits == BasePart.Units.Inches
                ? units == Part.Units.Inches ? number : number / 25.4
                : units == Part.Units.Inches
                    ? number
                    : number * 25.4;
            WorkPlane.SetRectangularUniformGridSize(new WorkPlane.GridSize(convertedValue, 1, 1));
            rdoGridOff.Checked = false;
            chkShowGrid.Checked = true;
            chkShowGrid.Enabled = true;
            chkSnapToGrid.Checked = true;
            chkSnapToGrid.Enabled = true;
            SetFormToGrid();
        }

        internal static bool IsValidString(string expression)
        {
            if(!expression.ToCharArray().Any(char.IsDigit)) return false;
            var temp = expression.ToLower().Replace(" ", "");
            return Regex.IsMatch(temp, "^([0-9]{0,}.?[0-9]{0,})(in|mm)?$");
        }

        private void GridForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            session_.Parts.RemoveWorkPartChangedHandler(_workPartChangeRegister);
        }

        private void TxtGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if(txtGrid.Text == @"Grid Off")
                txtGrid.SelectAll();
        }
    }
}