using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Layer;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static TSG_Library.UFuncs._UFunc;
using Point = System.Drawing.Point;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_proposal_data_wizard)]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("1.2", "2017", "08", "29")]
    //[Revision("1.2.1", "Fixed bug where the new component wasn't being created.")]
    //[Revision("1.2.1.1", "Something changed between nx9 and nx11.")]
    //[Revision("1.2.1.2", "Original code did not need a template specified when created a new component in 9.")]
    //[Revision("1.2.1.3", "It appears in 11 that you do in fact need to specify a template.")]
    //[Revision("1.2.2", "Also added a revision number to the form.")]
    //[RevisionEntry("2.0", "2017", "10", "24")]
    //[Revision("2.0.1",
    //    "Fixed bug where during the FindMasters() method, Object Null Reference would occur if one of the child components wasn't loaded.")]
    //[Revision("2.0.1.1", "Unloaded components will be ignored.")]
    //[Revision("2.0.1.2", "Suppressed components will also be ignored.")]
    //[Revision("2.0.2", "Changed Step Translator to use NX11.")]
    //[Revision("2.0.3",
    //    "The last step of the creating the proposal data process is now saving the Master and the simulation file. This only occurs if the process gets all the way to the end without throwing an exception.")]
    //[Revision("2.0.4", "Copy of the step file that is created is also copied to the MathData-P Level folder.")]
    //[Revision("2.0.5",
    //    "Made a change so that if the name of the selected Master is just Master then name the reference set the corresponding RXXPX number, else it functions as normal.")]
    //[Revision("2.0.6",
    //    "In the case of an error that occurs during any point of CreateProposalData method, the program will call a CleanUp() method, it performs the following.")]
    //[Revision("2.0.6.1", "It resets the Master to the state that it was in prior to the previous Proposal Run")]
    //[Revision("2.0.6.2", "It deletes the P Level folder in MathData if it exists.")]
    //[Revision("2.0.6.3", "It deletes the P Level folder in Outgoing if it exists.")]
    //[Revision("2.0.7", "Created a method called CheckPLevels() which performs the following.")]
    //[Revision("2.0.7.1", "Takes the selected P-Level and determines if there are any children under the master, and ")]
    //[Revision("2.0.7.2",
    //    "If there are any children under the Master, any Reference Sets in the Master, any proposal data folders in the outgoing folder, or any proposal data folders in the Mathdata folder with the same P-Level as the selected P-Level. If any of these exist then the user will be prompted to overwrite these occurrences. ")]
    //[RevisionEntry("2.1", "2017", "12", "07")]
    //[Revision("2.1.1", "Fixed issue where the proposal data part file name was being named incorrectly.")]
    //[RevisionEntry("2.2", "2018", "01", "03")]
    //[Revision("2.2.1",
    //    "Made it so that selected masters that are set to “Entire Part” cannot be processed and a subsequent message is displayed to the user.")]
    //[RevisionEntry("2.3", "2018", "11", "29")]
    //[Revision("2.3.1",
    //    "Re arranged form, so that the user can stretch the box and incorporate longer master file names.")]
    //[RevisionEntry("2.4", "2019", "02", "26")]
    //[Revision("2.4.1", "Reorganized code so that is will work with the current folder structure but will allow for,")]
    //[Revision("2.4.1.1", "Either “mathdata” or “Math Data”")]
    //[Revision("2.4.1.2", "And either “outgoingData” or “Outgoing”.")]
    //[Revision("2.4.1.3", "Will not work with the new folder structure yet.")]
    //[Revision("2.4.2",
    //    "When the list box is populated with the masters, it will now show the component name of the master in stead of the actual display of the master.")]
    //[Revision("2.4.2.1",
    //    "This should make it more difficult for the user to accidentally pick one master and then picking the incorrect corresponding sheet body for proposal.")]
    //[Revision("2.4.3",
    //    "Also, when the user hovers over the one of the masters in the list box, the corresponding master in the actual model will highlight.")]
    //[Revision("2.4.3.1", "This will make it easier to determine which master is actually being selected.")]
    //[Revision("2.4.4", "Moved all the code from the proposal data project, to CTS_Library.")]
    //[Revision("2.4.5",
    //    "Instead of having a Release level text box, it has now been changed to a Release and Study radio buttons.")]
    //[Revision("2.4.5.1", "This gives the user the ability to make both release and study data.")]
    //[RevisionEntry("2.5", "2019", "07", "11")]
    //[Revision("2.5.1", "Updated to use the updated GFolder.")]
    //[RevisionEntry("3.0", "2019", "08", "29")]
    //[Revision("3.0.1", "Edited back end of code to make easier to make changes in the future.")]
    //[RevisionEntry("3.1", "2020", "02", "19")]
    //[Revision("3.1.1",
    //    "Updated so that that proposal data will now look for data starting with either \"R\" or \"TSG\".")]
    //[RevisionEntry("3.2", "2020", "09", "24")]
    //[Revision("3.2.1", "Changd \"GetPartNumber\" method in \"Program\".")]
    //[Revision("3.2.2", "It now provides a more descriptive error message when an invalid part is found.")]
    //[RevisionEntry("3.3", "2021", "03", "15")]
    //[Revision("3.3.1", "Removed method that copies the created data to the outgoing folder.")]
    //[RevisionEntry("3.3", "2022", "10", "04")]
    //[Revision("3.3.1",
    //    "Updated the regular expresion of 'PartNumberWithDateRegex' to include \" dash, whitespace, underscore\"")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public partial class ProposalDataWizardForm : _UFuncForm
    {
        public enum DataLevelType1
        {
            Revision,

            Study
        }

        //public MainForm1() : this(typeof(Program))
        //{
        //}

        public enum MathdataType
        {
            Master,

            History
        }

        // The regular expression to match the proposal folders.
        public const string ProposalRegex = "^[0-9]{4}-[0-9]{2}-[0-9]{2}-p(?<proposalLevel>[0-9]+)(?:-proposal-data)*$";

        // The regular expression to match the part number with a trailing date.        
        public const string PartNumberWithDateRegex =
            @"^(TSG|R)\d+([ps][0-9]+)?[-\s_](?<partNumber>.+)[-\s_][0-9]{4}[-\s_][0-9]{1,2}[-\s_][0-9]{1,2}.*$";

        // The regular expression to match the TSG\R level of a part.
        public const string RLevel = "^(?<identifier>TSG|R)(?<level>\\d+).+";


        private readonly List<Tuple<Component, Body>> _selectedMasters;

        private Tag _overTag = NXOpen.Tag.Null;

        public ProposalDataWizardForm()
        {
            InitializeComponent();
            _selectedMasters = new List<Tuple<Component, Body>>();
            chkMakeStp.Checked = true;
            TagClicked += OnViewTagClicked;

            TagMouseOff += OnViewTagMouseOff;

            TagMouseOn += OnViewTagMouseOn;

            Reset();
        }

        private MathdataType MathdataType1
        {
            get
            {
                if (chkHistoryType.Checked)
                    return MathdataType.History;

                if (chkMasterType.Checked)
                    return MathdataType.Master;

                chkHistoryType.Checked = true;

                return MathdataType.History;
            }
        }

        private DataLevelType1 DataLevelType
        {
            get
            {
                if (rdoR.Checked)
                    return DataLevelType1.Revision;

                if (rdoS.Checked)
                    return DataLevelType1.Study;

                rdoR.Checked = true;

                return DataLevelType1.Revision;
            }
        }


        public event EventHandler<Tag> TagClicked;

        public event EventHandler<Tag> TagMouseOn;

        public event EventHandler<Tag> TagMouseOff;

        private void SetRevisionLevel()
        {
            ClearMasters();

            GFolder _folder = GFolder.Create(__display_part_.FullPath);

            foreach (Component master in FindMasters(__display_part_, MathdataType1))
                AddTag(master.Tag, master.Name);

            string regexString = MathdataType1 == MathdataType.History ? Regex_History : Regex_Master;

            switch (DataLevelType)
            {
                case DataLevelType1.Revision:

                    Component rootComponent = __display_part_.__RootComponentOrNull();

                    if (rootComponent is null)
                    {
                        txtDataLevel.Text = string.Empty;
                        break;
                    }

                    string maxRLevelString = "001";

                    string identifier = "";

                    foreach (Component child in rootComponent.GetChildren())
                    {
                        if (child.IsSuppressed) continue;

                        if (!(child.Prototype is Part prototype)) continue;

                        if (!Regex.IsMatch(child.DisplayName, regexString, RegexOptions.IgnoreCase)) continue;

                        Component rootMasterHistory = prototype.__RootComponentOrNull();

                        if (rootMasterHistory is null) continue;

                        foreach (Component descendant in rootMasterHistory.GetChildren())
                        {
                            Match match = Regex.Match(descendant.DisplayName, RLevel, RegexOptions.IgnoreCase);

                            if (!match.Success) continue;

                            string rLevelNumber = match.Groups["level"].Value;

                            identifier = match.Groups["identifier"].Value;

                            if (int.Parse(rLevelNumber) > int.Parse(maxRLevelString))
                                maxRLevelString = rLevelNumber;
                        }
                    }

                    identifier = identifier == "" ? "TSG" : identifier;

                    //txtDataLevel.Text = $"R{maxRLevelString}";
                    txtDataLevel.Text = $"{identifier}{maxRLevelString}";
                    break;
                case DataLevelType1.Study:
                    txtDataLevel.Text = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(DataLevelType), DataLevelType, null);
            }

            txtPLevel.Text = GetProposalLevel(_folder);
        }

        private void CheckChanged(object sender, EventArgs e)
        {
            if (sender == rdoR)
            {
                SetRevisionLevel();
            }
            else if (sender == rdoS)
            {
                SetRevisionLevel();
            }
            else if (sender == chkMasterType && chkMasterType.Checked)
            {
                chkHistoryType.Checked = false;
                Reset();
            }
            else if (sender == chkHistoryType && chkHistoryType.Checked)
            {
                chkMasterType.Checked = false;
                Reset();
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == btnReset)
                    Reset();
                else if (sender == btnCreate)
                    foreach (Tuple<Component, Body> master in _selectedMasters)
                        using (session_.__usingDisplayPartReset())
                        {
                            CreateData(master.Item1, master.Item2, txtDataLevel.Text, txtPLevel.Text,
                                chkMakeStp.Checked);
                        }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void OnViewTagMouseOn(object sender, Tag tag)
        {
            Component master = (Component)NXObjectManager.Get(tag);

            master.Highlight();
        }

        private static void OnViewTagMouseOff(object sender, Tag tag)
        {
            Component master = (Component)NXObjectManager.Get(tag);

            master.Unhighlight();
        }

        private void OnViewTagClicked(object sender, Tag tag)
        {
            Component master = (Component)NXObjectManager.Get(tag);

            foreach (Component tempMaster in FindMasters(__display_part_, MathdataType1))
                tempMaster.Blank();

            Body selectedBody = Selection.SelectBody();

            foreach (Component tempMaster in FindMasters(__display_part_, MathdataType1))
                tempMaster.Unblank();

            if (selectedBody is null)
                return;

            if (selectedBody.IsOccurrence)
            {
                print_("You cannot select an occurrence body.");
                return;
            }

            _selectedMasters.Add(new Tuple<Component, Body>(master, selectedBody));

            master.Blank();

            selectedBody.Blank();

            RemoveTag(master.Tag);
        }

        private void Mouse_Enter(object sender, EventArgs e)
        {
            if (sender != lstMasters) return;

            // Get the position that the mouse is currently over
            Point cursorPoint = Cursor.Position;

            cursorPoint = lstMasters.PointToClient(cursorPoint);

            int itemIndex = lstMasters.IndexFromPoint(cursorPoint);

            if (itemIndex < 0) return;

            ListItem item = (ListItem)lstMasters.Items[itemIndex];

            if (_overTag != NXOpen.Tag.Null)
                TagMouseOff?.Invoke(this, _overTag);

            TagMouseOn?.Invoke(this, item.Tag);

            _overTag = item.Tag;
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (sender != lstMasters) return;

            // Get the position that the mouse is currently over
            Point cursorPoint = Cursor.Position;

            cursorPoint = lstMasters.PointToClient(cursorPoint);

            int itemIndex = lstMasters.IndexFromPoint(cursorPoint);

            if (itemIndex < 0)
            {
                if (_overTag != NXOpen.Tag.Null)
                    TagMouseOff?.Invoke(this, _overTag);

                _overTag = NXOpen.Tag.Null;

                return;
            }

            // If the {itemIndex} is less, than zero, then that means the users mouse is 
            // in the {lstMasters} control but not over one of the 
            // valid masters/items in the control.
            ListItem item = (ListItem)lstMasters.Items[itemIndex];

            if (_overTag == item.Tag) return;

            if (_overTag != NXOpen.Tag.Null)
                TagMouseOff?.Invoke(this, _overTag);

            TagMouseOn?.Invoke(this, item.Tag);

            _overTag = item.Tag;
        }

        private void Mouse_Leave(object sender, EventArgs e)
        {
            if (_overTag != NXOpen.Tag.Null)
                TagMouseOff?.Invoke(this, _overTag);
        }

        public void AddTag(Tag masterTag, string masterText)
        {
            lstMasters.Items.Add(new ListItem(masterTag, masterText));
        }

        public void RemoveTag(Tag masterTag)
        {
            try
            {
                ListItem listItem = lstMasters.Items.OfType<ListItem>().FirstOrDefault(item => item.Tag == masterTag);

                lstMasters.Items.Remove(listItem);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        public void ClearMasters()
        {
            lstMasters.Items.Clear();
        }

        private void Reset()
        {
            try
            {
                Part display = Session.GetSession().Parts.Display;

                if (display is null)
                {
                    print_("There is no display part");
                    return;
                }

                GFolder folder = GFolder.Create(display.FullPath);

                if (!Directory.Exists(folder.DirMathData))
                {
                    print_($"GFolder: '{folder.DirJob}' doesn't have a Mathdata folder.");
                    return;
                }

                if (!rdoR.Checked && !rdoS.Checked)
                    rdoR.Checked = true;
                if (!chkHistoryType.Checked && !chkMasterType.Checked)
                    chkMasterType.Checked = true;

                MathdataType mathdataType = chkHistoryType.Checked ? MathdataType.History : MathdataType.Master;

                DataLevelType1 dataLevelType = rdoR.Checked ? DataLevelType1.Revision : DataLevelType1.Study;

                ClearMasters();

                foreach (Tuple<Component, Body> master in _selectedMasters)
                {
                    master.Item1.Unblank();
                    master.Item2.Unblank();
                }

                _selectedMasters.Clear();

                // todo: Need to make it so that the _folder will change with the work part changed handler.
                // the folder will default to the one that was open when the proposal data form was fired.

                GFolder _folder = GFolder.create_or_null(__display_part_);

                foreach (Component master in FindMasters(__display_part_, mathdataType))
                    AddTag(master.Tag, master.Name);

                string regexString = mathdataType == MathdataType.History ? Regex_History : Regex_Master;

                switch (dataLevelType)
                {
                    case DataLevelType1.Revision:

                        //language=regexp


                        Component rootComponent = __display_part_.__RootComponentOrNull();

                        if (rootComponent is null)
                        {
                            txtDataLevel.Text = string.Empty;
                            break;
                        }

                        string maxRLevelString = "001";

                        foreach (Component child in rootComponent.GetChildren())
                        {
                            if (child.IsSuppressed)
                                continue;

                            if (!(child.Prototype is Part prototype))
                                continue;

                            if (!Regex.IsMatch(child.DisplayName, regexString, RegexOptions.IgnoreCase))
                                continue;

                            Component rootMasterHistory = prototype.__RootComponentOrNull();

                            if (rootMasterHistory is null)
                                continue;

                            foreach (Component descendant in rootMasterHistory.GetChildren())
                            {
                                Match match = Regex.Match(descendant.DisplayName, RLevel, RegexOptions.IgnoreCase);

                                if (!match.Success) continue;

                                string rLevelNumber = match.Groups["level"].Value;

                                if (int.Parse(rLevelNumber) > int.Parse(maxRLevelString))
                                    maxRLevelString = rLevelNumber;
                            }
                        }

                        txtDataLevel.Text = $"TSG{maxRLevelString}";
                        break;
                    case DataLevelType1.Study:
                        txtDataLevel.Text = string.Empty;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataLevelType), dataLevelType, null);
                }

                txtPLevel.Text = GetProposalLevel(_folder);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Hide();
                int selectedIndex = lstMasters.SelectedIndex;

                if (selectedIndex < 0 || selectedIndex >= lstMasters.Items.Count) return;
                ListItem selectedMaster = (ListItem)lstMasters.Items[selectedIndex];

                TagClicked?.Invoke(this, selectedMaster.Tag);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static Component[] FindMasters(Part simulation, MathdataType mathdataType)
        {
            List<Component> list = new List<Component>();

            string regexString = mathdataType == MathdataType.History ? Regex_History : Regex_Master;

            foreach (Component child in simulation.ComponentAssembly.RootComponent?.GetChildren() ?? new Component[0])
                if (Regex.IsMatch(child.DisplayName, regexString))
                {
                    if (simulation.Layers.GetState(child.Layer) != State.WorkLayer)
                        simulation.Layers.SetState(child.Layer, State.Selectable);

                    // Un-blanks the master.
                    child.Unblank();

                    list.Add(child);
                }

            return list.ToArray();
        }

        public static string GetProposalLevel(GFolder folder)
        {
            // Gets all the directories that are below the math data folder.
            string[] directories = Directory.GetDirectories(folder.DirMathData, "*", SearchOption.AllDirectories);

            // Represents the highest proposal level found.
            // The default is 1.
            int currentProposalLevel = 0;

            // Iterate through the {directories}.
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (string directory in directories)
            {
                // Gets the name of the {directory}.
                string directoryName = Path.GetFileNameWithoutExtension(directory);

                // If the {directoryName} is null, then we can continue.
                if (directoryName is null) continue;

                // Matches the {directoryName}.
                Match match = Regex.Match(directoryName, ProposalRegex);

                // If the {match} is not successful, then we can continue.
                if (!match.Success) continue;

                // Gets the proposal level from the directory.
                int proposalLevel = int.Parse(match.Groups["proposalLevel"].Value);

                // If the {proposalLevel} is greater than the {currentProposalLevel}, then we can set the {currentProposalLevel} to the {proposalLevel}.
                if (currentProposalLevel < proposalLevel)
                    currentProposalLevel = proposalLevel;
            }

            return $"p{currentProposalLevel + 1}";
        }

        public static void Step(string partPath, string stepPath, bool wait)
        {
            try
            {
                if (partPath == null)
                    throw new ArgumentNullException(nameof(partPath));

                if (stepPath == null)
                    throw new ArgumentNullException(nameof(stepPath));

                if (!File.Exists(partPath))
                    throw new ArgumentException(@"Path to part does not exist.", nameof(partPath));

                if (File.Exists(stepPath))
                    throw new ArgumentException(@"Path for Step file already exists.", nameof(stepPath));


                Session.UndoMarkId undoMarkId1 = session_.SetUndoMark(0, "Start");
                StepCreator stepCreator = session_.DexManager.CreateStepCreator();
                stepCreator.ExportAs = (StepCreator.ExportAsOption)1;
                stepCreator.ExportFrom = (StepCreator.ExportFromOption)1;
                stepCreator.ObjectTypes.Solids = true;
                stepCreator.InputFile = partPath;
                stepCreator.OutputFile = stepPath;
                session_.SetUndoMarkName(undoMarkId1, "Export to STEP Options Dialog");
                stepCreator.SettingsFile = "U:\\nxFiles\\Step Translator\\214ug.def";
                stepCreator.ObjectTypes.Curves = true;
                stepCreator.ObjectTypes.Surfaces = true;
                stepCreator.ObjectTypes.Solids = true;
                session_.DeleteUndoMark(session_.SetUndoMark((Session.MarkVisibility)1, "Export to STEP Options"),
                    null);
                Session.UndoMarkId undoMarkId2 =
                    session_.SetUndoMark((Session.MarkVisibility)1, "Export to STEP Options");
                stepCreator.FileSaveFlag = false;
                stepCreator.ProcessHoldFlag = wait;
                stepCreator.LayerMask = "1-256";
                stepCreator.Commit();
                session_.DeleteUndoMark(undoMarkId2, null);

                if (wait)
                    print_(File.Exists(stepPath)
                        ? $"Successfully created \"{stepPath}\"."
                        : $"Unsuccessfully created \"{stepPath}\".");
            }
            catch (Exception ex)
            {
                ex.__PrintException("Error when creating Step file for " + partPath);
            }
        }

        public static void CreateStpFile(string partToExport)
        {
            // todo

            string outPutPath = Path.ChangeExtension(partToExport, "stp");

            Step(partToExport, outPutPath, true);

            string directoryName = Path.GetDirectoryName(outPutPath);

            if (directoryName is null)
                return;

            foreach (string path in Directory.GetFiles(directoryName))
                if (path.EndsWith(".log") || path.EndsWith(".def"))
                    File.Delete(path);
        }

        public static ExtractFace Wavelink(Component master, Body simulationBody)
        {
            try
            {
                TheUFSession.So.CreateXformAssyCtxt(master.__Prototype().Tag, NXOpen.Tag.Null, master.Tag,
                    out Tag xform);

                TheUFSession.Wave.CreateLinkedBody(simulationBody.Tag, xform, master.__Prototype().Tag, false,
                    out Tag linkedFeature);

                UFSession.GetUFSession().Modl.Update();

                return (ExtractFace)NXObjectManager.Get(linkedFeature);
            }
            finally
            {
                // Restores the display part to be the work part as well.
                __work_part_ = __display_part_;
            }
        }

        public static string ConstructProposalFilePath(Component master, string dataLevel, string pLevel)
        {
            // If the {master} is not loaded, then we need to throw.
            if (!(master.Prototype is Part prototype))
                throw new ArgumentOutOfRangeException(nameof(master), "The master must be loaded.");

            // The {master} cannot be set to neither "Entire Part" nor "Empty" reference set.
            if (master.ReferenceSet == Refset_EntirePart || master.ReferenceSet == Refset_Empty)
                throw new ArgumentException(
                    "The master cannot be set to the \"Entire Part\" nor \"Empty\" reference set.", nameof(master));

            // Get the name of the reference set that the {master} is currently set to.
            string referenceSetTitle = master.ReferenceSet;

            // Gets the reference set from the {prototype}.
            ReferenceSet referenceSet = prototype.__FindReferenceSet(referenceSetTitle);

            // Gets the objects from the {referenceSet} that are components.
            Component[] componentsInReferenceSet = referenceSet.AskAllDirectMembers()
                .OfType<Component>()
                .ToArray();

            // Gets the GFolder that the {master} sits in.
            GFolder folder = GFolder.create_or_null(prototype);

            if (folder is null) throw new DirectoryNotFoundException("Master did not reside within a GFolder.");

            // If {componentsInReferenceSet} doesn't have a length of 1, then we need to throw.
            if (componentsInReferenceSet.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(master),
                    $"The reference set \"{referenceSetTitle}\" in the master doesn't contain exactly one component.");

            // Gets the component that is a descendant of the {master}.
            Component descendantOfMaster = componentsInReferenceSet[0];

            // Gets the display name of the {descendantOfMaster}.
            string displayName = descendantOfMaster.DisplayName;

            // Gets the part number from the {displayName}.
            string partNumber = GetPartNumber(displayName);

            string revisedPartNumber = $"{dataLevel}{pLevel}-{partNumber}-{TodaysDate}";

            if (Directory.Exists($"{folder.DirMathData}\\Proposal"))
                return $"{folder.DirMathData}\\Proposal\\{TodaysDate}-{pLevel}\\{revisedPartNumber}.prt";

            string masterDirectory =
                Path.GetDirectoryName(prototype.FullPath); //  new FileInfo(prototype.FullPath).Directory.FullName;

            string filePath = $"{masterDirectory}\\{TodaysDate}-{pLevel}\\{revisedPartNumber}-proposal-data.prt";

            return filePath;
        }

        public static string GetPartNumber(string displayName)
        {
            Match match = Regex.Match(displayName, PartNumberWithDateRegex);

            if (!match.Success)
                throw new InvalidOperationException(
                    $"The display name {displayName} did not match the part number regex.\n" +
                    "It must start with (TSG\\R)### and end with a date in the form of YYYY-MM-DD.\n");

            return match.Groups["partNumber"].Value;
        }

        public static Component CreateProposalComponent(Body proposalBody, string newPartPath)
        {
            FileNew fileNew1 = session_.Parts.FileNew();
            fileNew1.Units = (Part.Units)__work_part_.PartUnits;
            string directoryName = Path.GetDirectoryName(newPartPath) ?? throw new DirectoryNotFoundException();

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            fileNew1.NewFileName = newPartPath;

            // Revision • 1.2 – 2017 – 08 – 29
            ////////////////////////////////////////////////////////
            //fileNew1.TemplateType = NXOpen.FileNewTemplateType.Item;
            //fileNew1.TemplatePresentationName = "Model";
            //fileNew1.TemplateFileName = "seed-part-metric.prt";
            fileNew1.UseBlankTemplate = true;
            ////////////////////////////////////////////////////////
            CreateNewComponentBuilder createNewComponentBuilder1 =
                Session.GetSession().Parts.Work.AssemblyManager.CreateNewComponentBuilder();
            createNewComponentBuilder1.ReferenceSet = CreateNewComponentBuilder.ComponentReferenceSetType.Other;
            createNewComponentBuilder1.LayerOption = CreateNewComponentBuilder.ComponentLayerOptionType.AsSpecified;
            createNewComponentBuilder1.NewComponentName = Path.GetFileNameWithoutExtension(newPartPath);
            createNewComponentBuilder1.LayerNumber = 60;
            createNewComponentBuilder1.ComponentOrigin = CreateNewComponentBuilder.ComponentOriginType.Absolute;
            createNewComponentBuilder1.ObjectForNewComponent.Add(proposalBody);
            createNewComponentBuilder1.NewFile = fileNew1;
            createNewComponentBuilder1.Commit();
            NXObject ject = createNewComponentBuilder1.GetObject();
            createNewComponentBuilder1.Destroy();
            return (Component)ject;
        }

        public static ReferenceSet CreateMasterReferenceSet(Part masterPart, string referenceSetName,
            params NXObject[] objectsToAdd)
        {
            // Creates a reference set derived from the prototype of the {master}.
            ReferenceSet masterReferenceSet = masterPart.CreateReferenceSet();

            // Sets the name of the {masterReferenceSet} to be the display name of the {proposalComponent}.
            masterReferenceSet.SetName(referenceSetName);

            // Adds the {proposalComponent} to the {masterReferenceSet}.
            masterReferenceSet.AddObjectsToReferenceSet(objectsToAdd);

            return masterReferenceSet;
        }

        public static ReferenceSet CreateProposalReferenceSet(Part proposalPart, string referenceSetName,
            params NXObject[] objectsToAdd)
        {
            // Attempts to get the BODY reference set from the {proposalPrototype}.
            ReferenceSet tempBodyReferenceSet = proposalPart.__FindReferenceSetOrNull(referenceSetName);

            // If thw {tempBodyReferenceSet} is not null, then we can delete it.
            if (!(tempBodyReferenceSet is null))
                proposalPart.DeleteReferenceSet(tempBodyReferenceSet);

            // Creates a reference set derived from the {proposalPrototype}.
            ReferenceSet proposalReferenceSet = proposalPart.CreateReferenceSet();

            // Names the {proposalReferenceSet} to {referenceSetName}.
            proposalReferenceSet.SetName(referenceSetName);

            // Adds the {bodyInProposalPrototype} to the {proposalReferenceSet}.
            proposalReferenceSet.AddObjectsToReferenceSet(objectsToAdd);

            return proposalReferenceSet;
        }

        public static void CreateData(Component master, Body simulationBody, string dataLevel, string pLevel,
            bool makeStp)
        {
            //try
            //{
            //    GFolder _folder = GFolder.create(((NXOpen.Part)master.OwningPart).FullPath);

            //    // We are expecting the {master} and the {simulationBody} to be in the same assembly,
            //    // I.E. have the same owning part.
            //    if (master.OwningPart.Tag != simulationBody.OwningPart.Tag)
            //        throw new InvalidOperationException("The provided master component and simulation body do not reside in the same assembly.");

            //    // We are expecting the {simulationBody} to be derived from it's owning part.
            //    // Therefore not an occurrence.
            //    if (simulationBody.IsOccurrence)
            //        throw new ArgumentException("The simulation body cannot be an occurrence.", nameof(simulationBody));

            //    // If the {master} is not loaded, then we need throw.
            //    if (!(master.Prototype is NXOpen.Part prototypeMaster))
            //        throw new ArgumentOutOfRangeException(nameof(master), "The master must be loaded.");

            //    // Links the {simulationBody} into the {master}.
            //    NXOpen.Features.ExtractFace linkedBody = Wavelink(master, simulationBody);

            //    __display_part_ = prototypeMaster;

            //    // Un-parameterizes the {linkedBody} and gets the resulting proposal solid body.
            //    NXOpen.Body proposalBody = linkedBody._UnparameterizeSingleBody();

            //    // Gets the to be created proposal file path.
            //    string proposalFilePath = ConstructProposalFilePath(master, dataLevel, pLevel);

            //    print_(proposalFilePath);

            //    // Creates a new part and a new component under the master using the {proposalBody} and the {newPartPath}.
            //    NXOpen.Assemblies.Component proposalComponent = CreateProposalComponent(proposalBody, proposalFilePath);

            //    // Creates a reference set in the {prototypeMaster} with the display name of the newly created {proposalComponent},
            //    // and adds the {proposalComponent} to the reference set.
            //    CreateMasterReferenceSet(prototypeMaster, proposalComponent.DisplayName, proposalComponent);

            //    // Gets the prototype of the {proposalComponent}.
            //    NXOpen.Part proposalPrototype = proposalComponent.__Prototype();

            //    proposalPrototype.Save(NXOpen.BasePart.SaveComponents.True, NXOpen.BasePart.CloseAfterSave.False);

            //    // Gets the single body that is contained by the {proposalPrototype}.
            //    NXOpen.Body bodyInProposalPrototype = proposalPrototype.Bodies.ToArray()[0];

            //    CreateProposalReferenceSet(proposalPrototype, Refset_Body, bodyInProposalPrototype);

            //    if (!makeStp)
            //        return;

            //    CreateStpFile(proposalFilePath);

            //    //CopyToOutgoing(_folder, Path.ChangeExtension(proposalFilePath, "stp"));
            //}
            //catch (Exception ex)
            //{
            //    ex._PrintException();
            //}

            throw new NotImplementedException();
        }

        private void ProposalDataWizardForm_Load(object sender, EventArgs e)
        {
        }
    }
}