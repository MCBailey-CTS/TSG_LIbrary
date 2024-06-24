using NXOpen.CAE;
using NXOpen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class OrientViewToCsys : Form
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
            {
                CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __work_part_.WCS.Rotate(WCS.Axis.ZAxis, 180);
                __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
                CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
                __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
                Text = "Back";
            }
            else if (sender == buttonViewBtm)
            {
                CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __work_part_.WCS.Rotate(WCS.Axis.XAxis, 180);
                CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
                __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
                Text = "Bottom";
            }
            else if (sender == buttonViewFront)
            {
                CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90);
                CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
                __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
                Text = "Front";
            }
            else if (sender == buttonViewLeft)
            {
                CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __work_part_.WCS.Rotate(WCS.Axis.ZAxis, -90.0);
                __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
                CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
                __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
                Text = "Left";
            }
            else if (sender == buttonViewRight)
            {
                CartesianCoordinateSystem prevCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __work_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
                __work_part_.WCS.Rotate(WCS.Axis.XAxis, 90.0);
                CartesianCoordinateSystem viewCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(viewCsys.Orientation.Element);
                __display_part_.WCS.SetOriginAndMatrix(prevCsys.Origin, prevCsys.Orientation.Element);
                Text = "Right";
            }
            else if (sender == buttonViewTop)
            {
                CartesianCoordinateSystem currentCsys = session_.Parts.Display.WCS.CoordinateSystem;
                __display_part_.Views.WorkView.Orient(currentCsys.Orientation.Element);
                Text = "Top";
            }
        }
    }
}
