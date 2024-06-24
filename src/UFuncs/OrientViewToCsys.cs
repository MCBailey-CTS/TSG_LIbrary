using NXOpen;
using System;
using System.Windows.Forms;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class OrientViewToCsys : _UFuncForm
    {
        public OrientViewToCsys()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(buttonViewFront, "Front View");
            toolTip1.SetToolTip(buttonViewBack, "Back View");
            toolTip1.SetToolTip(buttonViewTop, "Top View");
            toolTip1.SetToolTip(buttonViewBtm, "Bottom View");
            toolTip1.SetToolTip(buttonViewRight, "Right View");
            toolTip1.SetToolTip(buttonViewLeft, "Left View");
        }

        private void ViewButton_Clicked(object sender, EventArgs e)
        {
            if (sender == buttonViewBack)
                NewMethod();
            else if (sender == buttonViewBtm)
                NewMethod1();
            else if (sender == buttonViewFront)
                NewMethod2();
            else if (sender == buttonViewLeft)
                NewMethod3();
            else if (sender == buttonViewRight)
                NewMethod4();
            else if (sender == buttonViewTop)
                NewMethod5();
        }

        private void NewMethod5()
        {
            CartesianCoordinateSystem currentCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(currentCsys.Orientation.Element);
            Text = "Top";
        }

        private void NewMethod4()
        {
            CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __work_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
            __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
            CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
            __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
            Text = "Right";
        }

        private void NewMethod3()
        {
            CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __work_part_.WCS.Rotate(WCS.Axis.ZAxis, -90.0);
            __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
            CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
            __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
            Text = "Left";
        }

        private void NewMethod2()
        {
            CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90);
            CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
            __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
            Text = "Front";
        }

        private void NewMethod1()
        {
            CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __work_part_.WCS.Rotate(WCS.Axis.XAxis, 180);
            CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
            __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
            Text = "Bottom";
        }

        private void NewMethod()
        {
            CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __work_part_.WCS.Rotate(WCS.Axis.ZAxis, 180);
            __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
            CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
            __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
            __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
            Text = "Back";
        }
    }
}
