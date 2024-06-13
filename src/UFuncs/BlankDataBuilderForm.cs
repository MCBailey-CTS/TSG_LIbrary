using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("blank-data-builder")]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("1.2", "2018", "08", "22")]
    //[Revision("1.2.1", "Fixed dxf settings path to point to NX11 instead of NX9.")]
    //[RevisionEntry("1.3", "2019", "08", "28")]
    //[Revision("1.3.1", "GFolder updated to allow old job number under non cts folder.")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public partial class BlankDataBuilderForm : _UFuncForm
    {
        private const string dxfCmd = "dxfdwg.cmd /c ";
        private const string dxfSettings = "C:\\Program Files\\Siemens\\NX 11.0\\DXFDWG\\dxfdwg.def";

        private static Part workPart = session_.Parts.Work;
        private static Part displayPart = session_.Parts.Display;
        private static readonly string tempDir = Environment.GetEnvironmentVariable("TMP");
        private static string nameBuilder = "";
        private static string dxfArguments = "";

        public BlankDataBuilderForm()
        {
            try
            {
                InitializeComponent();
                InitializeFormData();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ComboBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOperation.Text == "Hot")
            {
                comboBoxVersion.Enabled = true;

                try
                {
                    List<int> names = new List<int>();

                    if (workPart.ComponentAssembly.RootComponent != null)
                    {
                        foreach (Component comp in workPart.ComponentAssembly.RootComponent.GetChildren())
                        {
                            int indexOf = comp.DisplayName.LastIndexOf("-V", StringComparison.Ordinal);
                            if (indexOf == -1) continue;
                            string fullCompName = comp.DisplayName.Substring(indexOf + 1);
                            string versionNumber = fullCompName.Substring(1, 2);
                            int.TryParse(versionNumber, out int testVersionNumber);
                            names.Add(testVersionNumber);
                        }

                        if (names.Count != 0)
                        {
                            names.Sort();
                            int lastVersionNumber = names[names.Count - 1];
                            if ((lastVersionNumber > 0) & (lastVersionNumber < 10))
                                comboBoxVersion.Text = "V0" + lastVersionNumber;
                            if ((lastVersionNumber > 9) & (lastVersionNumber < 100))
                                comboBoxVersion.Text = "V" + lastVersionNumber;
                        }
                        else
                        {
                            comboBoxVersion.Text = "";
                        }
                    }
                }

                catch (Exception ex)
                {
                    comboBoxVersion.Text = "";
                    ex.__PrintException();
                }
            }

            if (comboBoxOperation.Text != "Cold")
                return;

            comboBoxVersion.Enabled = true;

            try
            {
                List<int> names = new List<int>();

                if (workPart.ComponentAssembly.RootComponent is null)
                    return;

                foreach (Component comp in workPart.ComponentAssembly.RootComponent.GetChildren())
                {
                    int indexOf = comp.DisplayName.LastIndexOf("-V", StringComparison.Ordinal);
                    if (indexOf == -1) continue;
                    string fullCompName = comp.DisplayName.Substring(indexOf + 1);
                    string versionNumber = fullCompName.Substring(1, 2);

                    int.TryParse(versionNumber, out int testVersionNumber);
                    names.Add(testVersionNumber);
                }

                if (names.Count == 0)
                {
                    comboBoxVersion.Text = "";
                    return;
                }

                names.Sort();
                int lastVersionNumber = names[names.Count - 1] + 1;

                if ((lastVersionNumber > 0) & (lastVersionNumber < 10))
                    comboBoxVersion.Text = $"V0{lastVersionNumber}";

                if ((lastVersionNumber > 9) & (lastVersionNumber < 100))
                    comboBoxVersion.Text = $"V{lastVersionNumber}";
            }
            catch (Exception ex)
            {
                comboBoxVersion.Text = "";
                ex.__PrintException();
            }
        }


        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            displayPart = session_.Parts.Display;
            workPart = session_.Parts.Work;

            const string prompt = "Select objects to export";
            const string title = "Export data";
            IntPtr clientData = IntPtr.Zero;

            TheUFSession.Ui.LockUgAccess(UF_UI_FROM_CUSTOM);

            TheUFSession.Ui.SelectWithClassDialog(prompt, title, UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY, null, clientData,
                out _, out _, out Tag[] selObjects);

            TheUFSession.Ui.UnlockUgAccess(UF_UI_FROM_CUSTOM);

            foreach (Tag objs in selObjects)
                TheUFSession.Disp.SetHighlight(objs, 0);

            // build export/dxf/component name

            // Add Current Date
            string currentDate = DateTime.Today.ToString("yyyy-MM-dd");


            if (selObjects.Length <= 0) return;
            try
            {
                if (textBoxJobNumber.Text != string.Empty)
                    nameBuilder = textBoxJobNumber.Text;
                if (comboBoxVersion.Text != string.Empty)
                    nameBuilder += "-" + comboBoxVersion.Text;
                if (comboBoxOperation.Text != string.Empty)
                {
                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (comboBoxOperation.Text == "Hot")
                        nameBuilder += "-Hot-Blank-" + currentDate;
                    if (comboBoxOperation.Text == "Cold")
                        nameBuilder += "-Cold-Blank-" + currentDate;
                }

                if (textBoxCustomText.Text != string.Empty)
                    nameBuilder += "-" + textBoxCustomText.Text;


                // export temp part for dxf file

                string tempPart = tempDir + "\\" + nameBuilder;


                bool doesExist = false;


                if (displayPart.ComponentAssembly.RootComponent != null)
                    foreach (Component simComp in displayPart.ComponentAssembly.RootComponent.GetChildren())
                        if (simComp.Name == nameBuilder.ToUpper())
                            doesExist = true;

                if (doesExist)
                {
                    DialogResult dResult = MessageBox.Show($"Replace file {nameBuilder}?", "File Exist",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);


                    if (dResult != DialogResult.OK) return;
                }

                if (!(displayPart.ComponentAssembly.RootComponent is null))
                    foreach (Component simComp in displayPart.ComponentAssembly.RootComponent.GetChildren())
                    {
                        if (simComp.Name != nameBuilder.ToUpper()) continue;
                        Part closeSimPart = (Part)simComp.Prototype;
                        closeSimPart.Close(BasePart.CloseWholeTree.False, BasePart.CloseModified.CloseModified, null);

                        Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

                        NXObject[] objects1 = { simComp };

                        session_.UpdateManager.AddObjectsToDeleteList(objects1);

                        session_.UpdateManager.DoUpdate(markId1);
                    }

                BuildFilesExportDxf(tempPart, selObjects);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            //Set FILENAME Part Attribute to name builder 
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFormData();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void InitializeFormData()
        {
            if (Session.GetSession().Parts.Display is null)
                return;

            displayPart = session_.Parts.Display;
            workPart = session_.Parts.Work;

            textBoxJobNumber.Text = string.Empty;

            comboBoxOperation.SelectedIndex = -1;
            comboBoxOperation.Items.Clear();

            comboBoxVersion.SelectedIndex = -1;
            comboBoxVersion.Items.Clear();

            textBoxCustomText.Clear();

            textBoxRevisionLevel.Clear();

            Match match = Regex.Match(workPart.Leaf, Regex_Simulation);

            textBoxJobNumber.Text = match.Success ? match.Groups["customerNum"].Value : "JOB #";

            comboBoxOperation.Items.Add("Hot");
            comboBoxOperation.Items.Add("Cold");

            comboBoxVersion.Items.Add("V01");
            comboBoxVersion.Items.Add("V02");
            comboBoxVersion.Items.Add("V03");
            comboBoxVersion.Items.Add("V04");
            comboBoxVersion.Items.Add("V05");
            comboBoxVersion.Items.Add("V06");
            comboBoxVersion.Items.Add("V07");
            comboBoxVersion.Items.Add("V08");
            comboBoxVersion.Items.Add("V09");
            comboBoxVersion.Items.Add("V10");
            comboBoxVersion.Items.Add("V11");
            comboBoxVersion.Items.Add("V12");
            comboBoxVersion.Items.Add("V13");
            comboBoxVersion.Items.Add("V14");
            comboBoxVersion.Items.Add("V15");
            comboBoxVersion.Items.Add("V16");
            comboBoxVersion.Items.Add("V17");
            comboBoxVersion.Items.Add("V18");
            comboBoxVersion.Items.Add("V19");
            comboBoxVersion.Items.Add("V20");
            comboBoxVersion.Items.Add("V21");
            comboBoxVersion.Items.Add("V22");
            comboBoxVersion.Items.Add("V23");
            comboBoxVersion.Items.Add("V24");
            comboBoxVersion.Items.Add("V25");
        }


        private void BuildFilesExportDxf(string partFile, Tag[] tagObjects)
        {
            try
            {
                string versionNumber = comboBoxVersion.Text;

                if (string.IsNullOrEmpty(versionNumber))
                    throw new InvalidOperationException("Invalid version number.");


                GFolder folder = GFolder.create_or_null(displayPart)
                                 ??
                                 throw new InvalidOperationException(
                                     "The current display part does not reside within in a GFolder.");

                if (File.Exists(partFile + ".prt"))
                    File.Delete(partFile + ".prt");

                UFPart.ExportOptions exportOptions = new UFPart.ExportOptions
                {
                    new_part = true, expression_mode = UFPart.ExportExpMode.CopyExpDeeply,
                    params_mode = UFPart.ExportParamsMode.RemoveParams
                };

                TheUFSession.Part.ExportWithOptions(partFile, tagObjects.Length, tagObjects, ref exportOptions);

                // get path to blank location, check directory structure

                string outputDirectory = $"{folder.DirOutgoing}\\{TodaysDate}-Blank-{versionNumber}";

                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                string compBlanksPath = outputDirectory;

                // export part to Blanks directory

                if (Directory.Exists(compBlanksPath) == false)
                    Directory.CreateDirectory(compBlanksPath);

                compBlanksPath += "\\" + nameBuilder + ".prt";

                if (File.Exists(compBlanksPath))
                    File.Delete(compBlanksPath);

                TheUFSession.Part.ExportWithOptions(compBlanksPath, tagObjects.Length, tagObjects, ref exportOptions);

                // add  component to Blanks file

                BasePart basePart1 = session_.Parts.OpenBase(compBlanksPath, out PartLoadStatus basePartLoadStatus);
                basePartLoadStatus.Dispose();

                Point3d sPartOrigin = new Point3d(0.0, 0.0, 0.0);
                Matrix3x3 sPartMatrix = _Matrix3x3Identity;

                int layer;

                switch (comboBoxOperation.Text)
                {
                    case "Cold":
                        layer = 101;
                        break;
                    case "Hot":
                        layer = 103;
                        break;
                    default:
                        layer = 1;
                        break;
                }

                Part surfacePart = (Part)basePart1;
                displayPart.ComponentAssembly.AddComponent(surfacePart, Refset_EntirePart, nameBuilder, sPartOrigin,
                    sPartMatrix, layer, out PartLoadStatus sPartLoadStatus);
                sPartLoadStatus.Dispose();

                displayPart.Save(BasePart.SaveComponents.True, BasePart.CloseAfterSave.False);

                // export blank dxf data

                if (comboBoxOperation.Text != "Hot" && comboBoxOperation.Text != "Cold") return;
                string blankDxf =
                    Path.ChangeExtension(compBlanksPath,
                        "dxf"); //   $"{folder.JobFolder}\\blankDevelopment\\Blanks\\{nameBuilder}.dxf";
                string dxfBatch = partFile + ".bat";
                dxfArguments = $"{dxfCmd}\"{partFile}.prt\" o=\"{blankDxf}\" d=\"{dxfSettings}\"";

                using (FileStream fs = File.Open(dxfBatch, FileMode.Create))
                {
                    fs.Close();
                }

                using (StreamWriter writer = new StreamWriter(dxfBatch))
                {
                    writer.WriteLine("c:");
                    writer.WriteLine("cd " + "\"" + "C:\\Program Files\\Siemens\\NX 11.0\\DXFDWG" + "\"");
                    writer.WriteLine(dxfArguments);
                    writer.WriteLine("");
                }

                Process.Start(dxfBatch);
            }
            //Handle this exception somehow - Duane
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSetAttribute_Click(object sender, EventArgs e)
        {
            // Add Current Date
            string currentDate = DateTime.Today.ToString("yyyy-MM-dd");

            if (textBoxJobNumber.Text != string.Empty)
                nameBuilder = textBoxJobNumber.Text;
            if (comboBoxVersion.Text != string.Empty)
                nameBuilder += "-" + comboBoxVersion.Text;
            if (comboBoxOperation.Text != string.Empty)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (comboBoxOperation.Text == "Hot")
                    nameBuilder += "-Hot-Blank-" + currentDate;
                if (comboBoxOperation.Text == "Cold")
                    nameBuilder += "-Cold-Blank-" + currentDate;
            }

            if (textBoxCustomText.Text != string.Empty)
                nameBuilder += "-" + textBoxCustomText.Text;

            NXObject.AttributeInformation[] attrInfo = displayPart.GetUserAttributes();

            foreach (NXObject.AttributeInformation attr in attrInfo)
            {
                if (attr.Title != "REVISION TEXT") continue;
                string attrValue = displayPart.GetStringUserAttribute(attr.Title, -1);

                if (attrValue != "")
                {
                    displayPart.SetUserAttribute("REVISION TEXT", -1, nameBuilder, NXOpen.Update.Option.Now);
                    Layout layout1 = workPart.Layouts.FindObject("L1");
                    layout1.ReplaceView(workPart.ModelingViews.WorkView, workPart.ModelingViews.WorkView, true);
                }
                else
                {
                    displayPart.SetUserAttribute("REVISION TEXT", -1, "", NXOpen.Update.Option.Now);
                }
            }

            string revisionLevelText = nameBuilder;

            textBoxRevisionLevel.Text = revisionLevelText;
        }

        private void BlankDataBuilderForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.blank_data_builder_window_location;
        }

        private void BlankDataBuilderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.blank_data_builder_form_window_location = Location;
            Settings.Default.Save();
        }
    }
}