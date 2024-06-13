using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MoreLinq;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("nx-win-zip")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class NxWinZipForm : _UFuncForm
    {
        private static Part displayPart = session_.Parts.Display;
        private static readonly List<Part> assmParts = new List<Part>();
        private static DialogResult result = DialogResult.Cancel;
        private static string assemblyPart = string.Empty;
        private static string displayPathText = string.Empty;
        private static readonly List<Component> allComponents = new List<Component>();
        private static List<Component> selComponents = new List<Component>();

        public NxWinZipForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetFormDefaults();
            SetSessionLoadOptions();
        }


        private void LabelAssmText_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(labelAssmText, displayPathText);
        }

        private void LabelZipText_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(labelZipText, labelZipText.Text);
        }

        private void ButtonLoadCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                if (displayPart is null)
                {
                    MessageBox.Show("Please load an assembly", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                assmParts.Clear();
                allComponents.Clear();
                selComponents.Clear();
                UpdateSessionParts();

                buttonSelectAssm.Enabled = false;

                labelAssmText.Text = displayPart.FullPath;
                assemblyPart = displayPart.FullPath;

                displayPathText = assemblyPart;

                int total = session_.Parts.ToArray().Length + 1;
                int count = 1;

                foreach (Part part in session_.Parts)
                {
                    double percentComplete = count / Convert.ToDouble(total);
                    progressBarLoadAssm.Value = Convert.ToInt32(percentComplete * 100);
                    count++;

                    if (part.IsFullyLoaded) continue;
                    session_.Parts.Open(part.FullPath, out PartLoadStatus loadStatus2);
                    loadStatus2.Dispose();
                }

                if (displayPart.ComponentAssembly.RootComponent == null)
                {
                    MessageBox.Show("Display Part is not an Assembly", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                assmParts.Add(displayPart);
                GetAllChildComponents(displayPart.ComponentAssembly.RootComponent);
                selComponents = allComponents.DistinctBy(__c => __c.DisplayName).ToList();

                if (selComponents != null)
                    foreach (Component comp in selComponents)
                        assmParts.Add((Part)comp.Prototype);

                buttonSelectAssm.Enabled = false;
                buttonSave.Enabled = true;
            }
            catch (NXException ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSelectAssm_Click(object sender, EventArgs e)
        {
            try
            {
                if (displayPart == null)
                {
                    if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                    result = DialogResult.OK;

                    assmParts.Clear();
                    UpdateSessionParts();

                    string openAssembly = openFileDialog1.FileName;
                    assemblyPart = openFileDialog1.FileName;

                    displayPathText = assemblyPart;

                    session_.Parts.Open(openAssembly, out PartLoadStatus partLoadStatus);
                    partLoadStatus.Dispose();

                    labelAssmText.Text = openAssembly;

                    int total = session_.Parts.ToArray().Length + 1;
                    int count = 1;

                    foreach (Part part in session_.Parts)
                    {
                        double percentComplete = count / Convert.ToDouble(total);
                        progressBarLoadAssm.Value = Convert.ToInt32(percentComplete * 100);
                        count++;

                        if (part.IsFullyLoaded) continue;
                        session_.Parts.Open(part.FullPath, out PartLoadStatus loadStatus2);
                        loadStatus2.Dispose();
                    }

                    foreach (Part part in session_.Parts)
                        assmParts.Add(part);

                    buttonCloseAssm.Enabled = true;
                    buttonLoadCurrent.Enabled = false;
                    buttonSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please start a new session", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (NXException ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonCloseAssm_Click(object sender, EventArgs e)
        {
            DialogResult closeResult = MessageBox.Show("This will close the loaded assembly", "Verify",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);

            if (closeResult != DialogResult.OK) return;
            CloseAssembly();
            SetFormDefaults();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxCompression.SelectedIndex == 0)
            {
                // ReSharper disable once StringLiteralTypo
                saveFileDialog1.DefaultExt = "zipx";
                saveFileDialog1.FilterIndex = 2;
            }
            else
            {
                saveFileDialog1.DefaultExt = "zip";
                saveFileDialog1.FilterIndex = 1;
            }

            SaveZipFile(assemblyPart);
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (result == DialogResult.OK)
                CreateZipFile(saveFileDialog1.FileName);

            if (buttonCloseAssm.Enabled)
                CloseAssembly();

            SetFormDefaults();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        //===========================================
        //  Methods
        //===========================================


        private void CreateZipFile(string fileName)
        {
            try
            {
                if (assmParts.Count == 0) return;
                WinZipCompression compressionLevel = (WinZipCompression)comboBoxCompression.SelectedItem;
                string compression = compressionLevel.CompressValue;
                string tempFile = Path.GetTempPath() + "zipData.txt";
                // ReSharper disable once StringLiteralTypo
                const string winZip = "C:\\Program Files (x86)\\WinZip\\wzzip";

                using (FileStream fs = File.Open(tempFile, FileMode.Create))
                {
                    fs.Close();
                }

                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    foreach (Part part in assmParts)
                        writer.WriteLine(part.FullPath);
                }

                Process process = new Process();

                if (checkBoxDirPath.Checked)
                {
                    process.EnableRaisingEvents = false;
                    process.StartInfo.FileName = winZip;
                    process.StartInfo.Arguments = " -a -P " + compression + " " + "\"" + fileName + "\"" + " @" + "\"" +
                                                  tempFile + "\"";
                    process.Start();
                }
                else
                {
                    process.EnableRaisingEvents = false;
                    process.StartInfo.FileName = winZip;
                    process.StartInfo.Arguments = " -a " + compression + " " + "\"" + fileName + "\"" + " @" + "\"" +
                                                  tempFile + "\"";
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void CloseAssembly()
        {
            session_.Parts.CloseAll(BasePart.CloseModified.CloseModified, null);
        }

        private static void SetSessionLoadOptions()
        {
            session_.Parts.LoadOptions.ComponentLoadMethod = LoadOptions.LoadMethod.AsSaved;
            session_.Parts.LoadOptions.ComponentsToLoad = LoadOptions.LoadComponents.All;
            session_.Parts.LoadOptions.UsePartialLoading = false;
            session_.Parts.LoadOptions.GenerateMissingPartFamilyMembers = false;
            session_.Parts.LoadOptions.SetInterpartData(true, LoadOptions.Parent.Immediate);
            session_.Parts.LoadOptions.AbortOnFailure = false;
            session_.Parts.LoadOptions.AllowSubstitution = true;
        }

        private void SetFormDefaults()
        {
            assmParts.Clear();

            assemblyPart = string.Empty;
            labelAssmText.Text = string.Empty;
            progressBarLoadAssm.Value = 0;
            labelZipText.Text = string.Empty;
            displayPathText = string.Empty;

            comboBoxCompression.Items.Clear();

            // ReSharper disable once StringLiteralTypo
            WinZipCompression zipComp1 = new WinZipCompression("Maximum (.zipx)", "-el");
            comboBoxCompression.Items.Add(zipComp1);
            WinZipCompression zipComp2 = new WinZipCompression("Normal (.zip)", "-en");
            comboBoxCompression.Items.Add(zipComp2);
            WinZipCompression zipComp3 = new WinZipCompression("Extra (.zip)", "-ex");
            comboBoxCompression.Items.Add(zipComp3);
            WinZipCompression zipComp4 = new WinZipCompression("FastenerType (.zip)", "-ef");
            comboBoxCompression.Items.Add(zipComp4);
            WinZipCompression zipComp5 = new WinZipCompression("SuperFast (.zip)", "-es");
            comboBoxCompression.Items.Add(zipComp5);

            comboBoxCompression.SelectedIndex = 0;

            buttonSelectAssm.Enabled = true;
            buttonLoadCurrent.Enabled = true;
            buttonCloseAssm.Enabled = false;
            buttonSave.Enabled = false;
            buttonOk.Enabled = false;
        }

        private static void UpdateSessionParts()
        {
            displayPart = session_.Parts.Display;
        }

        private void SaveZipFile(string fileName)
        {
            int lastDir = fileName.LastIndexOf("\\", StringComparison.Ordinal);
            int lastDot = fileName.LastIndexOf(".", StringComparison.Ordinal);
            string name = fileName.Substring(lastDir + 1, lastDot - (lastDir + 1));
            string dirOnly = fileName.Remove(lastDir);
            saveFileDialog1.InitialDirectory = dirOnly;
            saveFileDialog1.FileName = name;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                result = DialogResult.OK;
                buttonOk.Enabled = true;

                int lastIndex = saveFileDialog1.FileName.LastIndexOf("\\", StringComparison.Ordinal);
                string fileOnly = saveFileDialog1.FileName.Substring(lastIndex + 1);
                labelZipText.Text = fileOnly;
            }
            else
            {
                result = DialogResult.Cancel;
                buttonOk.Enabled = false;
            }
        }

        private static void GetAllChildComponents(Component assemblyPart1)
        {
            foreach (Component descendant in assemblyPart1.__DescendantsAll())
            {
                if (descendant.IsSuppressed) continue;
                if (descendant.Prototype is Part)
                {
                    print_("Heer");
                    allComponents.Add(descendant);
                    continue;
                }

                print_($"Component \"{descendant.DisplayName}\" is not loaded.");
            }


            try
            {
                if (assemblyPart1.GetChildren() == null) return;
                foreach (Component child in assemblyPart1.GetChildren())
                {
                    if (child.IsSuppressed) continue;
                    Tag instance = TheUFSession.Assem.AskInstOfPartOcc(child.Tag);
                    if (instance == NXOpen.Tag.Null) continue;
                    TheUFSession.Assem.AskPartNameOfChild(instance, out string partName);
                    int partLoad = TheUFSession.Part.IsLoaded(partName);

                    if (partLoad == 1)
                    {
                        allComponents.Add(child);
                        GetAllChildComponents(child);
                    }
                    else
                    {
                        //var partOpened = CTS_Library.Globals.Find

                        print_(partName);
                        TheUFSession.Part.OpenQuiet(partName, out Tag partOpen, out _);

                        if (partOpen == NXOpen.Tag.Null) continue;
                        allComponents.Add(child);
                        GetAllChildComponents(child);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }
    }
}