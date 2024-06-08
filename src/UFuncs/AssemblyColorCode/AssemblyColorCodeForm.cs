using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions;
using static NXOpen.UF.UFConstants;
using static NXOpen.Session;
using Point = System.Drawing.Point;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_assembly_color_code)]
    [RevisionLog("Assembly Color Code")]
    [RevisionEntry("1.00", "2017", "06", "05")]
    [Revision("1.00.1", "Created for NX 11")]
    [RevisionEntry("1.10", "2017", "08", "22")]
    [Revision("1.10.1", "Signed so it can run outside of CTS")]
    [RevisionEntry("1.15", "2017", "11", "09")]
    [Revision("1.15.1", "Removed exit button from form. ")]
    [Revision("1.15.1.1", "This only leaves the “X” in the upper right of the form.")]
    [Revision("1.15.2", "Added two new colors to select from, “DarkRed” and “PaleRed”.")]
    [Revision("1.15.3",
        "Restructured form so that it is now “163” pixels across. Fixes issue where the form would show up misshapen on big tv in conference room.")]
    [Revision("1.15.4", "Added a curves selection filter to the form.")]
    [RevisionEntry("1.2", "2017", "11", "27")]
    [Revision("1.2.1",
        "Created an undo mark every time the user colors an object or inherits a color followed up by a subsequent coloring.")]
    [RevisionEntry("1.3", "2018", "02", "28")]
    [Revision("1.3.1", "Added the ability to pick conic curve types. (Ellipse, Hyperbola, and Parabola)")]
    [Revision("1.3.1.1", "There isn’t a new radio button; they are just under the umbrella of “Curves”.")]
    [RevisionEntry("1.4", "2018", "09", "13")]
    [Revision("1.4.1",
        "Restructured the form. Colors moved to bottom of the form so that it can grow with more colors being added automatically.")]
    [Revision("1.4.2", "Converted form to use a UCF file. ")]
    [Revision("1.4.2.1", "Colors are now constructed at the load up and read from the ucf file.")]
    [Revision("1.4.3", "Added a new color. #80 = Light Weak Magenta.")]
    [RevisionEntry("1.41", "2018", "09", "13")]
    [Revision("1.41.1",
        "Fixed bug where when coloring a feature, the entire body of the feature was colored instead of just the faces of the feature.")]
    [RevisionEntry("1.5", "2019", "03", "20")]
    [Revision("1.5.1", "Moved the body of the code to be located in CTS_Library.")]
    [RevisionEntry("1.6", "2021", "05", "27")]
    [Revision("1.6.1", "The ConceptControlFile now points to \"U:\\nxFiles\\UfuncFiles\\ConceptControlFile.ucf\"")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class AssemblyColorCodeForm : _UFuncForm
    {
        public enum AssemblyColorCodeType
        {
            Solid,
            Face,
            Feature,
            Curve,
            None
        }

        private static Selection.MaskTriple[] __triples;

        private static readonly Selection.MaskTriple[] all_triples =
        {
            new Selection.MaskTriple { Type = UF_line_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_circle_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_spline_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple
                { Type = UF_conic_type, Subtype = UF_conic_hyperbola_subtype, SolidBodySubtype = 0 },
            new Selection.MaskTriple
                { Type = UF_conic_type, Subtype = UF_conic_parabola_subtype, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_conic_type, Subtype = UF_conic_ellipse_subtype, SolidBodySubtype = 0 },
            new Selection.MaskTriple
                { Type = UF_solid_type, Subtype = 0, SolidBodySubtype = UF_UI_SEL_FEATURE_ANY_FACE },
            new Selection.MaskTriple { Type = UF_solid_type, Subtype = 0, SolidBodySubtype = UF_UI_SEL_FEATURE_BODY }
        };

        private static readonly Selection.MaskTriple[] curves_mask =
        {
            new Selection.MaskTriple { Type = UF_line_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_circle_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_spline_type, Subtype = 0, SolidBodySubtype = 0 },
            new Selection.MaskTriple
                { Type = UF_conic_type, Subtype = UF_conic_hyperbola_subtype, SolidBodySubtype = 0 },
            new Selection.MaskTriple
                { Type = UF_conic_type, Subtype = UF_conic_parabola_subtype, SolidBodySubtype = 0 },
            new Selection.MaskTriple { Type = UF_conic_type, Subtype = UF_conic_ellipse_subtype, SolidBodySubtype = 0 }
        };

        private static readonly Selection.MaskTriple[] face_mask =
        {
            new Selection.MaskTriple
                { Type = UF_solid_type, Subtype = 0, SolidBodySubtype = UF_UI_SEL_FEATURE_ANY_FACE }
        };

        private static readonly Selection.MaskTriple[] body_mask =
        {
            new Selection.MaskTriple { Type = UF_solid_type, Subtype = 0, SolidBodySubtype = UF_UI_SEL_FEATURE_BODY }
        };

        public AssemblyColorCodeForm()
        {
            InitializeComponent();

            LoadColors("U:\\nxFiles\\UfuncFiles\\AssemblyColorCode.ucf");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Location = Settings.Default.assembly_color_code_form_window_location;

                switch (Settings.Default.AssemblyColorCodeLastUsed)
                {
                    case (int)AssemblyColorCodeType.Face:
                        rdoFace.Checked = true;
                        break;
                    case (int)AssemblyColorCodeType.Feature:
                        rdoFeature.Checked = true;
                        break;
                    case (int)AssemblyColorCodeType.Curve:
                        rdoCurves.Checked = true;
                        break;
                    case (int)AssemblyColorCodeType.None:
                        rdoNoFilter.Checked = true;
                        break;
                    case (int)AssemblyColorCodeType.Solid:
                        rdoSolid.Checked = true;
                        break;
                    default:
                        rdoNoFilter.Checked = true;
                        print_("unknown last used");
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public void LoadColors(string ucfPath)
        {
            var ucf = new Ucf(ucfPath);

            // The regular expression to match the rgb colors from the file.
            // language=regexp
            const string rgbRegex =
                @"^{(?<Color>.+)}\s*{A:(?<A>\d+)}\s*{B:(?<B>\d+)}\s*{G:(?<G>\d+)}\s*{R:(?<R>\d+)}\s*{NX:(?<NX>\d+)}\s*{DETAILS:(?<Details>-.*)}\s*{DCB:(?<Dcb>-.*)?}$";

            var xCounter = 0;

            var yCounter = 0;

            foreach (var colorLine in ucf["COLORS"])
            {
                // Matches the {colorLine}.
                var match = Regex.Match(colorLine, rgbRegex);

                // If the {colorLine} doesn't match, ignore it.
                if(!match.Success)
                {
                    print_($"Could not match \"{colorLine}\".");
                    continue;
                }

                if(xCounter % 4 == 0)
                {
                    xCounter = 0;
                    yCounter++;
                }

                // Gets the "A" value.
                var aValue = int.Parse(match.Groups["A"].Value);

                // Gets the "R" value.
                var rValue = int.Parse(match.Groups["R"].Value);

                // Gets the "G" value.
                var gValue = int.Parse(match.Groups["G"].Value);

                // Gets the "B" value.
                var bValue = int.Parse(match.Groups["B"].Value);

                // Gets the title of the color.
                var colorTitle = match.Groups["Color"].Value;

                // Gets the detail notes pertaining to this color.
                var detailNotes = match.Groups["Details"].Value;

                // Gets the dieset, castings, & burnout notes for this color.
                var dcb = match.Groups["Dcb"].Value;

                // Creates a button. 
                var nxColor = int.Parse(match.Groups["NX"].Value);
                // ReSharper disable once UseObjectOrCollectionInitializer
                var button = new Button
                {
                    BackColor = Color.FromArgb(aValue, rValue, gValue, bValue),
                    Location = new Point(10 + (36 - 10) * xCounter++, (45 - 19) * yCounter - 10),
                    Size = new Size(20, 20),
                    Name = colorTitle,
                    Tag = nxColor,
                    Cursor = Cursors.Hand
                };

                button.Click += ColorButtons_Click;

                var toolTipString = $"{button.Name} = {(int)button.Tag}";

                if(detailNotes != "-") toolTipString += $"\n\nDetails:\n{detailNotes}";

                if(dcb != "-") toolTipString += $"\n\nDiesets, Castings, & Burnouts:\n{dcb}";

                toolTipString = toolTipString.Replace("\\n", "\n");

                toolTip.SetToolTip(button, toolTipString);

                groupBoxColors.Controls.Add(button);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.assembly_color_code_form_window_location = Location;
            Settings.Default.Save();
        }

        private void ChooseColoring(int color)
        {
            __SetUndoMark(MarkVisibility.Visible, "Assembly Color");

            if(__triples is null && rdoFeature.Checked)
            {
                UI.GetUI().SelectionManager.SelectFeatures("Select Features", Selection.SelectionFeatureType.Browsable,
                    out var features);

                foreach (var feature in features)
                    feature.GetEntities()
                        .OfType<DisplayableObject>()
                        .ToList()
                        .ForEach(__f => __f.__Color(color));

                return;
            }

            if(__triples is null)
            {
                print_("Triples were null");
                return;
            }

            UI.GetUI().SelectionManager.SelectTaggedObjects(
                "",
                "",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                __triples,
                out var objects);

            objects.OfType<DisplayableObject>()
                .ToList()
                .ForEach(__f => __f.__Color(color));
        }

        private void RdoButton_Click(object sender, EventArgs e)
        {
            if(rdoFace.Checked)
            {
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Face;
                __triples = face_mask;
            }
            else if(rdoFeature.Checked)
            {
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Feature;
                __triples = null;
            }
            else if(rdoCurves.Checked)
            {
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Curve;
                __triples = curves_mask;
            }
            else if(rdoSolid.Checked)
            {
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Solid;
                __triples = body_mask;
            }
            else if(rdoNoFilter.Checked)
            {
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.None;
                __triples = all_triples;
            }
        }

        private void ColorButtons_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    var button = (Button)sender;
                    ChooseColoring((int)button.Tag);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void ButtonInheritColor_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    UI.GetUI().SelectionManager.SelectTaggedObject(
                        "",
                        "",
                        Selection.SelectionScope.AnyInAssembly,
                        Selection.SelectionAction.ClearAndEnableSpecific,
                        false,
                        false,
                        all_triples,
                        out var object_,
                        out _);

                    if(!(object_ is DisplayableObject disp))
                    {
                        print_("You didn't select an object with a color");
                        return;
                    }

                    ChooseColoring(disp.Color);
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            GetSession().Parts.SetWork(GetSession().Parts.Display);
        }
    }
}