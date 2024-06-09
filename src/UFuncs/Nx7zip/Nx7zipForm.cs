using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("nx-7-zip")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class Nx7zipForm : _UFuncForm
    {
        private static Part displayPart = session_.Parts.Display;
        private static readonly List<Part> assmParts = new List<Part>();
        private static DialogResult result = DialogResult.Cancel;
        private static string assemblyPart = string.Empty;
        private static string displayPathText = string.Empty;
        private static readonly List<Component> allComponents = new List<Component>();
        private static List<Component> selComponents = new List<Component>();

        public Nx7zipForm()
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
                if(displayPart is null)
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
                var total = session_.Parts.ToArray().Length + 1;
                var count = 1;

                foreach (Part part in session_.Parts)
                {
                    var percentComplete = count / Convert.ToDouble(total);
                    progressBarLoadAssm.Value = Convert.ToInt32(percentComplete * 100);
                    count++;

                    if(part.IsFullyLoaded)
                        continue;

                    session_.__FindOrOpen(part.FullPath);
                }

                if(displayPart.ComponentAssembly.RootComponent is null)
                {
                    MessageBox.Show("Display Part is not an Assembly", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                assmParts.Add(displayPart);

                GetAllChildComponents(displayPart.ComponentAssembly.RootComponent);
                selComponents = GetOneComponentOfMany(allComponents);

                if(!(selComponents is null))
                    foreach (var comp in selComponents)
                        assmParts.Add((Part)comp.Prototype);

                buttonSelectAssm.Enabled = false;
                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSelectAssm_Click(object sender, EventArgs e)
        {
            try
            {
                if(!(displayPart is null))
                {
                    MessageBox.Show("Please start a new session", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if(openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                result = DialogResult.OK;
                assmParts.Clear();
                UpdateSessionParts();
                var openAssembly = openFileDialog1.FileName;
                assemblyPart = openFileDialog1.FileName;
                displayPathText = assemblyPart;
                session_.__FindOrOpen(openAssembly);
                labelAssmText.Text = openAssembly;
                var total = session_.Parts.ToArray().Length + 1;
                var count = 1;

                foreach (Part part in session_.Parts)
                {
                    var percentComplete = count / Convert.ToDouble(total);
                    progressBarLoadAssm.Value = Convert.ToInt32(percentComplete * 100);
                    count++;

                    if(part.IsFullyLoaded)
                        continue;

                    session_.__FindOrOpen(part.FullPath);
                }

                foreach (Part part in session_.Parts)
                    assmParts.Add(part);

                buttonCloseAssm.Enabled = true;
                buttonLoadCurrent.Enabled = false;
                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonCloseAssm_Click(object sender, EventArgs e)
        {
            var closeResult = MessageBox.Show("This will close the loaded assembly", "Verify",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1);

            if(closeResult != DialogResult.OK)
                return;

            CloseAssembly(assemblyPart);
            SetFormDefaults();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveZipFile(assemblyPart);
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if(result == DialogResult.OK)
                CreateZipFile(saveFileDialog1.FileName);

            if(buttonCloseAssm.Enabled)
                CloseAssembly(assemblyPart);

            SetFormDefaults();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void CreateZipFile(string fileName)
        {
            try
            {
                if(assmParts.Count == 0)
                    return;

                var compressionLevel = (Nx7ZipCompression)comboBoxCompression.SelectedItem;
                var compression = compressionLevel.CompressValue;
                var tempFile = Path.GetTempPath() + "zipData.txt";
                var nx7zip = "C:\\Program Files\\7-Zip\\7z";

                using (var fs = File.Open(tempFile, FileMode.Create))
                {
                    fs.Close();
                }

                using (var writer = new StreamWriter(tempFile))
                {
                    foreach (var part in assmParts)
                        writer.WriteLine(part.FullPath);
                }

                var process = new Process { EnableRaisingEvents = false };
                process.StartInfo.FileName = nx7zip;
                process.StartInfo.Arguments = $"a -t7z \"{fileName}\" \"@{tempFile}\" {compression}";
                process.Start();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void CloseAssembly(string fileName)
        {
            session_.__FindOrOpen(fileName).__Close(true, true);
        }

        private void SetSessionLoadOptions()
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
            var zipComp1 = new Nx7ZipCompression("Ultra", "-mx9");
            comboBoxCompression.Items.Add(zipComp1);
            var zipComp2 = new Nx7ZipCompression("Maximum", "-mx7");
            comboBoxCompression.Items.Add(zipComp2);
            var zipComp3 = new Nx7ZipCompression("Normal", "-mx5");
            comboBoxCompression.Items.Add(zipComp3);
            var zipComp4 = new Nx7ZipCompression("Auto", "-mx3");
            comboBoxCompression.Items.Add(zipComp4);
            comboBoxCompression.SelectedIndex = 0;
            buttonSelectAssm.Enabled = true;
            buttonLoadCurrent.Enabled = true;
            buttonCloseAssm.Enabled = false;
            buttonSave.Enabled = false;
            buttonOk.Enabled = false;
        }

        private void UpdateSessionParts()
        {
            displayPart = __display_part_;
        }

        private void SaveZipFile(string fileName)
        {
            var lastDir = fileName.LastIndexOf("\\");
            var lastDot = fileName.LastIndexOf(".");
            var name = fileName.Substring(lastDir + 1, lastDot - (lastDir + 1));
            var dirOnly = fileName.Remove(lastDir);
            saveFileDialog1.InitialDirectory = dirOnly;
            saveFileDialog1.FileName = name;

            if(saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                result = DialogResult.Cancel;
                buttonOk.Enabled = false;
                return;
            }

            result = DialogResult.OK;
            buttonOk.Enabled = true;
            var lastIndex = saveFileDialog1.FileName.LastIndexOf("\\");
            var fileOnly = saveFileDialog1.FileName.Substring(lastIndex + 1);
            labelZipText.Text = fileOnly;
        }

        private static void GetAllChildComponents(Component assemblyPart)
        {
            Tag instance;
            int partLoad;

            try
            {
                if(assemblyPart.GetChildren() is null)
                    return;

                foreach (var child in assemblyPart.GetChildren())
                {
                    if(child.IsSuppressed)
                        continue;

                    instance = ufsession_.Assem.AskInstOfPartOcc(child.Tag);

                    if(instance == NXOpen.Tag.Null)
                        continue;

                    ufsession_.Assem.AskPartNameOfChild(instance, out var partName);
                    partLoad = ufsession_.Part.IsLoaded(partName);

                    if(partLoad == 1)
                    {
                        allComponents.Add(child);
                        GetAllChildComponents(child);
                        continue;
                    }

                    ufsession_.Part.OpenQuiet(partName, out var partOpen, out var loadStatus);

                    if(partOpen == NXOpen.Tag.Null)
                        continue;

                    allComponents.Add(child);
                    GetAllChildComponents(child);
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static List<Component> GetOneComponentOfMany(List<Component> compList)
        {
            var oneComp = new List<Component>
            {
                compList[0]
            };

            foreach (var comp in compList)
            {
                var foundComponent = oneComp.Find(delegate(Component c) { return c.DisplayName == comp.DisplayName; });

                if(foundComponent is null)
                    oneComp.Add(comp);
            }

            return oneComp.Count != 0 ? oneComp : null;
        }
    }
}