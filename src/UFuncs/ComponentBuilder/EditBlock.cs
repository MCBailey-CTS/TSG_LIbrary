using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Preferences;
using TSG_Library.Properties;
using static TSG_Library.Extensions.__Extensions_;
using Part = NXOpen.Part;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {
        private static Part _workPart = session_.Parts.Work;
        private static Part _displayPart = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = _displayPart;
        private static bool _isDynamic;
        private static Point _udoPointHandle;
        private static double _gridSpace;
        private static Point3d _workCompOrigin;
        private static Matrix3x3 _workCompOrientation;
        private static readonly List<string> _nonValidNames = new List<string>();
        private static readonly List<Line> _edgeRepLines = new List<Line>();
        private static double _distanceMoved;
        private static int _registered;
        private static int _idWorkPartChanged1;
        private static Component _updateComponent;
        private static Body _editBody;
        private static bool _isNewSelection = true;
        private static bool _isUprParallel;
        private static bool _isLwrParallel;
        private static string _parallelHeightExp = string.Empty;
        private static string _parallelWidthExp = string.Empty;

        public EditBlockForm()
        {
            InitializeComponent();
        }


        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if (_displayPart.PartUnits == BasePart.Units.Inches)
            {
                comboBoxGridBlock.Items.Add("0.002");
                comboBoxGridBlock.Items.Add("0.03125");
                comboBoxGridBlock.Items.Add("0.0625");
                comboBoxGridBlock.Items.Add("0.125");
                comboBoxGridBlock.Items.Add("0.250");
                comboBoxGridBlock.Items.Add("0.500");
                comboBoxGridBlock.Items.Add("1.000");
            }
            else
            {
                comboBoxGridBlock.Items.Add("0.0508");
                comboBoxGridBlock.Items.Add("0.79375");
                comboBoxGridBlock.Items.Add("1.00");
                comboBoxGridBlock.Items.Add("1.5875");
                comboBoxGridBlock.Items.Add("3.175");
                comboBoxGridBlock.Items.Add("5.00");
                comboBoxGridBlock.Items.Add("6.35");
                comboBoxGridBlock.Items.Add("12.7");
                comboBoxGridBlock.Items.Add("25.4");
            }

            foreach (string gridSetting in comboBoxGridBlock.Items)
                if (gridSetting == Settings.Default.EditBlockFormGridIncrement)
                {
                    int gridIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);
                    comboBoxGridBlock.SelectedIndex = gridIndex;
                    break;
                }
        }

        private void SetWorkPlaneOn()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = true;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;

#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SetWorkPlaneOff()
        {
            try
            {
                UpdateSessionParts();

                WorkPlane workPlane1;
                workPlane1 = _displayPart.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = false;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;
#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    TheUISession.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error,
                        "WorkPlane is null.  Reset Modeling State");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public int Startup()
        {
            if (_registered == 0)
            {
                EditBlockForm editForm = this;
                _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(editForm.WorkPartChanged1);
                _registered = 1;
            }

            return 0;
        }

        public void WorkPartChanged1(BasePart p)
        {
            SetWorkPlaneOff();
            LoadGridSizes();
            SetWorkPlaneOn();
        }
    }
}
// 4839