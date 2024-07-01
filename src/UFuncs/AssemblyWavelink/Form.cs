using System;
using System.Windows.Forms;
using NXOpen.Features;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using static TSG_Library.Extensions.Extensions;
using static TSG_Library.UFuncs.AssemblyWavelink.__UFunc__;

namespace TSG_Library.UFuncs.AssemblyWavelink
{
    [UFunc(_UFunc.ufunc_assembly_wavelink)]
    public partial class Form : _UFuncForm
    {
        public Form()
        {
            InitializeComponent();
        }

        private void AssemblyWavelink_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.assembly_wave_link_form_window_location;
            Text = AssemblyFileVersion;
        }

        private void AssemblyWavelink_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.assembly_wave_link_form_window_location = Location;
            Settings.Default.Save();
        }

        private void buttonShortTapReam_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, RefsetShortTap, RefsetReamShort);
            }
        }

        private void buttonTapReam_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, RefsetTap, RefsetReam);
            }
        }

        private void buttonTap_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, RefsetTap, null);
            }
        }

        private void buttonReam_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, RefsetReam);
            }
        }

        private void buttonShortTap_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, RefsetShortTap, null);
            }
        }

        private void buttonReamShort_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, RefsetReamShort);
            }
        }

        private void buttonClrHole_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, RefsetClrHole, RefsetClrHole);
            }
        }

        private void buttonCbore_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, null);
            }
        }

        private void buttonSubTool_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WavelinkSubtool();
            }
        }

        private void buttonSubtract_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Subtract);
            }
        }

        private void buttonUnite_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Unite);
            }
        }

        private void buttonLink_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Create);
            }
        }

        private void buttonIntersect_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Intersect);
            }
        }
    }
}
// 1080