using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using Part = NXOpen.Part;
using static NXOpen.UF.UFConstants;
using TSG_Library.Properties;
namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {

        #region variables
        private static Part _workPart = session_.Parts.Work;
        private static Part _displayPart = session_.Parts.Display;
        private static Part _originalWorkPart = _workPart;
        private static Part _originalDisplayPart = _displayPart;
        private static bool _isDynamic;
        private static Point _udoPointHandle;
        private static double _gridSpace;
        private static Point3d _workCompOrigin;
        private static Matrix3x3 _workCompOrientation;
        private static readonly List<string> _nonValidNames = new List<string>();
        private static readonly List<Line> _edgeRepLines = new List<Line>();
        private static double _distanceMoved;
        private static int _registered;
        private static int _idWorkPartChanged1;
        private static Component _updateComponent;
        private static Body _editBody;
        private static bool _isNewSelection = true;
        private static bool _isUprParallel;
        private static bool _isLwrParallel;
        private static string _parallelHeightExp = string.Empty;
        private static string _parallelWidthExp = string.Empty;
        #endregion

        #region form events

        public EditBlockForm() => InitializeComponent();

        private void ButtonEditMove_Click(object sender, EventArgs e) => EditMove();

        private void ButtonEditConstruction_Click(object sender, EventArgs e) => EditConstruction();

        private void ButtonEndEditConstruction_Click(object sender, EventArgs e) => EndEditConstruction();

        private void EditBlockForm_Load(object sender, EventArgs e) => EditBlock();

        private void ComboBoxGridBlock_SelectedIndexChanged(object sender, EventArgs e) => SelectGrid();

        private void ButtonExit_Click(object sender, EventArgs e) => Exit();

        private void ButtonApply_Click(object sender, EventArgs e) => Apply();

        private void ComboBoxGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.KeyCode.Equals(Keys.Return))
                return;

            if (comboBoxGridBlock.Text == "0.000")
            {
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }
        }

        private void ButtonReset_Click(object sender, EventArgs e) => Reset();

        private void ButtonEditAlign_Click(object sender, EventArgs e) => EditAlign();

        private void ButtonViewWcs_Click(object sender, EventArgs e) => ViewWcs();

        private void ButtonEditMatch_Click(object sender, EventArgs e) => EditMatch();

        private void ButtonAlignComponent_Click(object sender, EventArgs e) => AlignComponent();

        private void ButtonEditDynamic_Click(object sender, EventArgs e) => EditDynamic();

        private void ButtonEditSize_Click(object sender, EventArgs e) => EditSize();

        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e) => AlignEdgeDistance();

        #endregion



        #region form methods

        private void EditMove()
        {
            try
            {
                bool isBlockComponent = NewMethod11();
                NewMethod2();

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditMoveDisplay(isBlockComponent, editComponent)
                    : EditMoveWork(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }

        public void EditConstruction()
        {
            buttonExit.Enabled = false;
            UpdateSessionParts();
            UpdateOriginalParts();

            var editComponent = SelectOneComponent("Select Component to edit construction");

            if (editComponent is null)
                return;

            var assmUnits = _displayPart.PartUnits;
            var compBase = (BasePart)editComponent.Prototype;
            var compUnits = compBase.PartUnits;

            if (compUnits != assmUnits)
            {
                MessageBox.Show("Component units do not match the display part units");
                return;
            }

            using (session_.__UsingSuppressDisplay())
            {
                var addRefSetPart = (Part)editComponent.Prototype;
                __display_part_ = addRefSetPart;
                UpdateSessionParts();

                using (session_.__UsingDoUpdate("Delete Reference Set"))
                {
                    var allRefSets = _displayPart.GetAllReferenceSets();

                    foreach (var namedRefSet in allRefSets)
                        if (namedRefSet.Name == "EDIT")
                            _workPart.DeleteReferenceSet(namedRefSet);
                }

                // create edit reference set
                using (session_.__UsingDoUpdate("Create New Reference Set"))
                {
                    var editRefSet = _workPart.CreateReferenceSet();
                    var removeComps = editRefSet.AskAllDirectMembers();
                    editRefSet.RemoveObjectsFromReferenceSet(removeComps);
                    editRefSet.SetAddComponentsAutomatically(false, false);
                    editRefSet.SetName("EDIT");

                    // get all construction objects to add to reference set
                    var constructionObjects = new List<NXObject>();

                    for (var i = 1; i < 11; i++)
                    {
                        var layerObjects = _displayPart.Layers.GetAllObjectsOnLayer(i);
                        foreach (var addObj in layerObjects) constructionObjects.Add(addObj);
                    }

                    editRefSet.AddObjectsToReferenceSet(constructionObjects.ToArray());
                }

                __display_part_ = _originalDisplayPart;
                UpdateSessionParts();
            }
            __work_component_ = editComponent;
            UpdateSessionParts();
            SetWcsToWorkPart(editComponent);
            __work_component_.__Translucency(75);
            Component[] setRefComp = { editComponent };
            _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("EDIT", setRefComp);
            _displayPart.Layers.WorkLayer = 3;
            UpdateSessionParts();
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = true;
        }






        private void EndEditConstruction()
        {
            try
            {
                __work_component_.__Translucency(0);
                _displayPart.Layers.WorkLayer = 1;
                Session.UndoMarkId markId1;
                markId1 = session_.SetUndoMark(Session.MarkVisibility.Invisible, "Delete Reference Set");
                Component[] setRefComp = { session_.Parts.WorkComponent };
                _displayPart.ComponentAssembly.ReplaceReferenceSetInOwners("BODY", setRefComp);
                var allRefSets = _workPart.GetAllReferenceSets();

                foreach (var namedRefSet in allRefSets)
                    if (namedRefSet.Name == "EDIT")
                        _workPart.DeleteReferenceSet(namedRefSet);

                int nErrs1;
                nErrs1 = session_.UpdateManager.DoUpdate(markId1);
                session_.DeleteUndoMark(markId1, "Delete Reference Set");
                __display_part_ = _originalDisplayPart;
                __work_part_ = _originalWorkPart;
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                buttonExit.Enabled = true;
                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
                ex.__PrintException();
            }
        }




        private void EditBlock()
        {
            if (Settings.Default.udoComponentBuilderWindowLocation != null)
                Location = Settings.Default.udoComponentBuilderWindowLocation;

            buttonApply.Enabled = false;

            LoadGridSizes();

            if (string.IsNullOrEmpty(comboBoxGridBlock.Text))
                if (!(Session.GetSession().Parts.Work is null))
                    comboBoxGridBlock.SelectedItem = Session.GetSession().Parts.Work.PartUnits == BasePart.Units.Inches
                        ? "0.250"
                        : "6.35";

            _nonValidNames.Add("strip");
            _nonValidNames.Add("layout");
            _nonValidNames.Add("blank");
            _registered = Startup();
        }

        private void Exit()
        {
            session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
            Close();
            Settings.Default.udoComponentBuilderWindowLocation = Location;
            Settings.Default.Save();

            using (this)
                new ComponentBuilder().Show();
        }



        private void SelectGrid()
        {
            if (comboBoxGridBlock.Text == "0.000")
            {
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOff();
            }
            else
            {
                SetWorkPlaneOff();
                bool isConverted;
                isConverted = double.TryParse(comboBoxGridBlock.Text, out _gridSpace);
                SetWorkPlaneOn();
            }

            Settings.Default.EditBlockFormGridIncrement = comboBoxGridBlock.Text;
            Settings.Default.Save();
        }


        private void AlignComponent()
        {
            try
            {
                bool isBlockComponent = NewMethod9();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component to Align");

                AlignComponent(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }


        private void EditDynamic()
        {
            try
            {
                bool isBlockComponent = NewMethod8();

                if (_isNewSelection && _updateComponent is null)
                    SelectWithFilter_("Select Component for Dynamic Edit");

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    EditDynamicDisplayPart(isBlockComponent, editComponent);
                else
                    EditDynamicWorkPart(isBlockComponent, editComponent);
            }
            catch (Exception ex)
            {
                EnableForm();
                ex.__PrintException();
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
            finally
            {
                ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
                ufsession_.Disp.RegenerateDisplay();
            }
        }



        private void EditMatch()
        {
            try
            {
                bool isBlockComponent = NewMethod5();

                if (_isNewSelection)
                {
                    if (_updateComponent is null)
                    {
                        SelectWithFilter_("Select Component - Match From");
                    }
                    else
                    {
                        UpdateDynamicBlock(_updateComponent);
                        _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                        _displayPart.WCS.Visibility = true;
                        _isNewSelection = true;
                    }
                }
                else
                {
                    UpdateDynamicBlock(_updateComponent);
                    _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
                    _displayPart.WCS.Visibility = true;
                    _isNewSelection = true;
                }

                EditMatch(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            EnableForm();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

        #endregion

    }
}
// 4839