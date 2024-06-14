using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private void ButtonEditMove_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);
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

        private void ButtonAlignComponent_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod10();

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


        private void ButtonEditDynamic_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod();

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                if (editComponent is null)
                    isBlockComponent = EditDynamicDisplayPart(isBlockComponent, editComponent);
                else
                    isBlockComponent = EditDynamicWorkPart(isBlockComponent, editComponent);
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


        private void ButtonViewWcs_Click(object sender, EventArgs e)
        {
            UpdateSessionParts();
            UpdateOriginalParts();
            CoordinateSystem coordSystem = _displayPart.WCS.CoordinateSystem;
            Matrix3x3 orientation = coordSystem.Orientation.Element;
            _displayPart.Views.WorkView.Orient(orientation);
        }


        private void ButtonEditMatch_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection)
                {
                    if (_updateComponent is null)
                    {
                        NewMethod4();
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

        private void ButtonEditSize_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod6();

                if (_editBody is null)
                    return;

                Component editComponent = _editBody.OwningComponent;

                isBlockComponent = editComponent is null
                    ? EditSize(isBlockComponent, editComponent)
                    : EditSizeWork(isBlockComponent, editComponent);
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

        private void ButtonEditAlign_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);
                session_.Preferences.EmphasisVisualization.WorkPartEmphasis = false;

                if (_isNewSelection && _updateComponent is null)
                    NewMethod7();

                EdgeAlign(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }


        private void ButtonAlignEdgeDistance_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSessionParts();
                UpdateOriginalParts();
                buttonApply.Enabled = true;
                bool isBlockComponent = false;
                ufsession_.Ui.AskInfoUnits(out int infoUnits);
                Part.Units dispUnits = (Part.Units)_displayPart.PartUnits;
                SetDispUnits(dispUnits);

                if (_isNewSelection && _updateComponent is null)
                    NewMethod12();

                AlignEdgeDistance(isBlockComponent);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            Show();
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
        }

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

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            UpdateDynamicBlock(_updateComponent);
            __work_part_ = _originalWorkPart;
            UpdateSessionParts();
            UpdateOriginalParts();
            _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);
            _displayPart.WCS.Visibility = true;
            session_.DeleteAllUndoMarks();
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            buttonEditConstruction.Enabled = true;
            buttonEndEditConstruction.Enabled = true;
            buttonApply.Enabled = false;
            buttonReset.Enabled = true;
            buttonExit.Enabled = true;
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            _updateComponent = null;
            _editBody = null;
            _isNewSelection = true;
            ufsession_.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);
            ufsession_.Disp.RegenerateDisplay();
            session_.Preferences.EmphasisVisualization.WorkPartEmphasis = true;
            session_.Preferences.Assemblies.WorkPartDisplayAsEntirePart = false;
            UpdateSessionParts();
            UpdateOriginalParts();
            __work_part_ = _displayPart;
        }
    }
}