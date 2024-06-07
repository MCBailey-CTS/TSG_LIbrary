using System;
using System.Windows.Forms;
using NXOpen.Features;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using static TSG_Library.Extensions.Extensions_;
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
        }

        private void AssemblyWavelink_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.assembly_wave_link_form_window_location = Location;
            Settings.Default.Save();
        }

        private void buttonShortTapReam_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, Refset_ShortTap, Refset_ReamShort);
            }
        }

        private void buttonTapReam_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, Refset_Tap, Refset_Ream);
            }
        }

        private void buttonTap_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, Refset_Tap, null);
            }
        }

        private void buttonReam_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, Refset_Ream);
            }
        }

        private void buttonShortTap_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, Refset_ShortTap, null);
            }
        }

        private void buttonReamShort_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, Refset_ReamShort);
            }
        }

        private void buttonClrHole_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, Refset_ClrHole, Refset_ClrHole);
            }
        }

        private void buttonCbore_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkFasteners(chkBlankTools.Checked, null, null);
            }
        }

        private void buttonSubTool_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WavelinkSubtool();
            }
        }

        private void buttonSubtract_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Subtract);
            }
        }

        private void buttonUnite_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Unite);
            }
        }

        private void buttonLink_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Create);
            }
        }

        private void buttonIntersect_Click(object sender, EventArgs e)
        {
            using (session_.using_form_show_hide(this))
            {
                WaveLinkBoolean(chkBlankTools.Checked, Feature.BooleanType.Intersect);
            }
        }
    }
}
// 1080