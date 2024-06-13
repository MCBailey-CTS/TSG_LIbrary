using TSG_Library.Utilities;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
        private static void NewMethod23()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Move");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private static void NewMethod4()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component - Match From");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }


        private static void NewMethod6()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Edit Size");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }

        private static void NewMethod7()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }


        private static void NewMethod10()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }


        private static void NewMethod()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component for Dynamic Edit");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }


        private static void NewMethod12()
        {
            SelectWithFilter.NonValidCandidates = _nonValidNames;
            SelectWithFilter.GetSelectedWithFilter("Select Component to Align Edge");
            _editBody = SelectWithFilter.SelectedCompBody;
            _isNewSelection = true;
        }
    }
}