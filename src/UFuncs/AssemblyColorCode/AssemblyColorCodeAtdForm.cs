using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.UFuncs.AssemblyColorCodeForm;
using static TSG_Library.Extensions.__Extensions_;
using Point = System.Drawing.Point;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_assembly_color_code_atd)]
    public partial class AssemblyColorCodeAtdForm : _UFuncForm
    {
        public AssemblyColorCodeAtdForm()
        {
            InitializeComponent();
            LoadColors(@"G:\CTS\0CustomFasteners\Automation Tool & Die\Automation.cdf");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Location = Settings.Default.AssemblyColorCodeWindowLocation;


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
                    default:
                        rdoSolid.Checked = true;
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
            //Ucf ucf = new Ucf(ucfPath);

            // The regular expression to match the rgb colors from the file.
            // language=regexp
            const string rgbRegex =
                @"^{(?<Color>.+)}\s*{A:(?<A>\d+)}\s*{B:(?<B>\d+)}\s*{G:(?<G>\d+)}\s*{R:(?<R>\d+)}\s*{NX:(?<NX>\d+)}\s*{DETAILS:(?<Details>-.*)}\s*{DCB:(?<Dcb>-.*)?}$";

            var xCounter = 0;

            var yCounter = 0;

            //foreach (string colorLine in ucf["COLORS"])


            string[] colors =
            {
                "{C'Bore} {A:255} {B:174} {G:209} {R:155} {NX:171} {DETAILS:-} {DCB:-}",
                "{Jack Screws} {A:255} {B:214} {G:170} {R:131} {NX:13} {DETAILS:-} {DCB:-}",
                "{Dowels} {A:255} {B:196} {G:179} {R:209} {NX:10} {DETAILS:-} {DCB:-}",
                "{Dowels (REAM)} {A:255} {B:193} {G:196} {R:192} {NX:144} {DETAILS:-} {DCB:-}",
                "{Taps} {A:255} {B:196} {G:189} {R:123} {NX:170} {DETAILS:-} {DCB:-}",
                "{Finished 3D Surface} {A:255} {B:255} {G:255} {R:0} {NX:143} {DETAILS:-} {DCB:-}",
                "{Wire Burn} {A:255} {B:0} {G:0} {R:128} {NX:166} {DETAILS:-} {DCB:-}",
                "{Finish Profile and FLats to +/- 001} {A:255} {B:0} {G:255} {R:255} {NX:4} {DETAILS:-} {DCB:-}",
                "{SEMI-FINISH +.010/-.010} {A:255} {B:0} {G:255} {R:0} {NX:11} {DETAILS:-} {DCB:-}",
                "{Rough Finish +/-.030} {A:255} {B:255} {G:0} {R:0} {NX:8} {DETAILS:-} {DCB:-}"
            };


            foreach (var colorLine in colors)
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

                var toolTipString =
                    $"{button.Name} = {(int)button.Tag}, a={aValue}, r={rValue}, g={gValue}, b={bValue}";

                if(detailNotes != "-") toolTipString += $"\n\nDetails:\n{detailNotes}";

                if(dcb != "-") toolTipString += $"\n\nDiesets, Castings, & Burnouts:\n{dcb}";

                toolTipString = toolTipString.Replace("\\n", "\n");

                toolTip.SetToolTip(button, toolTipString);

                groupBoxColors.Controls.Add(button);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.AssemblyColorCodeWindowLocation = Location;

            Settings.Default.Save();
        }


        private void ChooseColoring(int color)
        {
            //// Revision • 1.2 – 2017 / 11 / 27
            //SetUndoMark(MarkVisibility.Visible, "Assembly Color");

            //// Creates a selection.
            //Snap.UI.Selection.Dialog selection = Snap.UI.Selection.SelectObject("Select objects.");
            //selection.IncludeFeatures = false;
            //selection.KeepHighlighted = false;
            //selection.AllowMultiple = true;
            //selection.Scope = Snap.UI.Selection.Dialog.SelectionScope.AnyInAssembly;

            //// Sets the scope and cue of the {selection}.
            //SetFilterAndCue(selection);

            //// Shows the dialog and allows the user to select the objects.
            //// Gets the result.
            //Snap.UI.Selection.Result result = selection.Show();

            //// If there isn't any selected items, return.
            //if (result.Objects.Length == 0) return;

            //// If the feature radio button is currently selected, we want to color the body feature of the selected features.
            //if (rdoFeature.Checked)
            //{
            //    // Gets the {BodyFeature}'s from the selected objects.
            //    NXOpen.Features.BodyFeature[] bodyFeatures = result.Objects
            //        .Select(snapObj => snapObj.NXOpenTaggedObject)
            //        .OfType<NXOpen.Features.BodyFeature>()
            //        .ToArray();

            //    // If there aren't any {BodyFeatures}, then we can inform the user and return.
            //    if (bodyFeatures.Length == 0)
            //    {
            //        print_("Could not find any body features in the selected list of features.");
            //        return;
            //    }

            //    // Colors the faces of the body features.
            //    bodyFeatures.SelectMany(bodyFeature => bodyFeature.GetFaces()).ToList().ForEach(face => face._SetDisplayColor(color));
            //}
            //else
            //    // Colors the selected objects the {nxColor}.

            //    result.Objects.Select(nxObject => nxObject.NXOpenDisplayableObject).ToList().ForEach(obj => obj._SetDisplayColor(color));

            throw new NotImplementedException();
        }

        private void SetFilterAndCue( /*Snap.UI.Selection.Dialog selection*/)
        {
            //if (rdoFace.Checked)
            //{
            //    selection.Cue = "Select Faces";
            //    selection.SetFilter(Snap.NX.ObjectTypes.Type.Face);
            //}
            //else if (rdoFeature.Checked)
            //{
            //    selection.Cue = "Select Faces";
            //    selection.IncludeFeatures = true;
            //    selection.SetFilter(Snap.NX.ObjectTypes.Type.Feature);
            //}
            //else if (rdoSolid.Checked)
            //{
            //    selection.Cue = "Select Faces";
            //    selection.SetFilter(Snap.NX.ObjectTypes.Type.Body);
            //}
            //else if (rdoCurves.Checked)
            //{
            //    selection.Cue = "Select Faces";
            //    selection.SetFilter(Snap.NX.ObjectTypes.Type.Line, Snap.NX.ObjectTypes.Type.Arc, Snap.NX.ObjectTypes.Type.Spline, Snap.NX.ObjectTypes.Type.Conic);
            //}
            //else if (rdoNoFilter.Checked)
            //{
            //    selection.Cue = "Select Faces";
            //}
            //else
            //{
            //    throw new InvalidOperationException("None of the radio buttons are selected.");
            //}

            throw new NotImplementedException();
        }

        private void RdoButton_Click(object sender, EventArgs e)
        {
            if(sender == rdoFace)
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Face;
            else if(sender == rdoFeature)
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Feature;
            else if(sender == rdoSolid)
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.Solid;
            else if(sender == rdoNoFilter)
                Settings.Default.AssemblyColorCodeLastUsed = (int)AssemblyColorCodeType.None;
        }

        private void ColorButtons_Click(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;

                Hide();

                ChooseColoring((int)button.Tag);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                Show();
            }
        }

        private void ButtonInheritColor_Click(object sender, EventArgs e)
        {
            using (session_.__UsingFormShowHide(this))
            {
                try
                {
                    //Snap.UI.Selection.Dialog selection = Snap.UI.Selection.SelectObject("Select objects.");

                    //selection.IncludeFeatures = false;

                    //selection.KeepHighlighted = false;

                    //selection.AllowMultiple = false;

                    //selection.Scope = Snap.UI.Selection.Dialog.SelectionScope.AnyInAssembly;

                    //SetFilterAndCue(selection);

                    //Snap.UI.Selection.Result result = selection.Show();

                    //if (result.Objects.Length == 0) return;

                    //ChooseColoring(result.Object.NXOpenDisplayableObject.Color);

                    throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    ex.__PrintException();
                }
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            Session.GetSession().Parts.SetWork(Session.GetSession().Parts.Display);
        }

        public static class Properties
        {
            /// <summary>
            ///     The path to the "Smart Button Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartButtonMetric => "G:\\0Library\\Button\\Smart Button Metric.prt";

            /// <summary>
            ///     The path to the "Smart Button English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartButtonEnglish => "G:\\0Library\\Button\\Smart Button English.prt";

            /// <summary>
            ///     The path to the "Smart Punch Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartPunchMetric => "G:\\0Library\\PunchPilot\\Metric\\Smart Punch Metric.prt";

            /// <summary>
            ///     The path to the "Smart Punch English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartPunchEnglish => "G:\\0Library\\PunchPilot\\English\\Smart Punch English.prt";

            /// <summary>
            ///     The path to the "Smart Ball Lock Retainer Metric.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartBallLockRetainerMetric =>
                "G:\\0Library\\Retainers\\Metric\\Smart Ball Lock Retainer Metric.prt";

            /// <summary>
            ///     The path to the "Smart Ball Lock Retainer English.prt" file in G:\\0Library.
            /// </summary>
            public static string SmartBallLockRetainerEnglish =>
                "G:\\0Library\\Retainers\\English\\Smart Ball Lock Retainer English.prt";

            /// <summary>
            ///     Returns the name of the reference set to be used for AddPierceComponents.
            /// </summary>
            public static string SlugRefsetName => "SLUG BUTTON ALIGN";

            // ReSharper disable once StringLiteralTypo
            public static string RetainerAlignPunchFaceName => "ALIGNPUNCH";

            // ReSharper disable once StringLiteralTypo
            public static string PunchTopFaceName => "PUNCHTOPFACE";

            // ReSharper disable once InconsistentNaming
            public static string Pierced_P => "PIERCED_P";

            // ReSharper disable once InconsistentNaming
            public static string Pierced_W => "PIERCED_W";

            // ReSharper disable once InconsistentNaming
            public static string Pierced_Type => "PIERCED_TYPE";

            public static Regex PiercedFaceRegex => new Regex("^PIERCED_([0-9]{1,})$");
        }
    }
}