using System;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.UFuncs._UFunc;
using static TSG_Library.Extensions.Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_show_only)]
    [RevisionEntry("1.0", "2018", "06", "20")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2019", "03", "11")]
    [Revision("1.1.1", "Code moved to CTS_Library and updated for new server.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public partial class ShowOnlyForm : _UFuncForm
    {
        private readonly HashSet<Tag> _blankedOffObjects = new HashSet<Tag>();

        public ShowOnlyForm()
        {
            InitializeComponent();

            var materialValues = from part in session_.Parts.OfType<Part>()
                from att in part.GetUserAttributes()
                where att.Title.ToUpper() == "MATERIAL"
                where att.Type == NXObject.AttributeType.String
                select att.StringValue
                into value
                where value != null
                orderby value
                select value.ToUpper();

            cmbMaterialsBox.Items.AddRange(materialValues.Distinct().ToArray<object>());
        }

        private void BlankOff(TaggedObject part)
        {
            // Gets all the occurrences of {part} in the current assembly.
            ufsession_.Assem.AskOccsOfPart(__display_part_.Tag, part.Tag, out var partOccs);

            // Iterates through the partOccurrences if {part} in the current {DisplayedPart}.
            foreach (var partOcc in partOccs)
            {
                // Blanks off the object.
                ufsession_.Obj.SetBlankStatus(partOcc, UFConstants.UF_OBJ_BLANKED);
                // Add the blanked off object to the set.
                _blankedOffObjects.Add(partOcc);
            }
        }

        private void Unblank(TaggedObject part)
        {
            // Gets all the occurrences of {part} in the current assembly.
            ufsession_.Assem.AskOccsOfPart(__display_part_.Tag, part.Tag, out var partOccs);

            // Iterates through the partOccurrences if {part} in the current {DisplayedPart}.
            foreach (var partOcc in partOccs)
            {
                // Blanks off the object.
                ufsession_.Obj.SetBlankStatus(partOcc, UFConstants.UF_OBJ_NOT_BLANKED);
                // Add the blanked off object to the set.
                _blankedOffObjects.Add(partOcc);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbMaterialsBox.SelectedItem is string selected))
                return;

            foreach (Part part in session_.Parts)
            {
                // We want to ignore any sort of assembly part.
                // This takes care of the issue where nothing was being un blanked because the assembly holders almost always don't have 
                // a material attribute and so they end up being blanked off which blanks their sub assembly off.
                if (part.Leaf._IsAssemblyHolder())
                    continue;

                // If the part doesn't have a {MATERIAL} attribute then we want to blank the occurrences of 
                // this part off.
                if (!part.HasUserAttribute("MATERIAL", NXObject.AttributeType.String, -1))
                {
                    BlankOff(part);
                    continue;
                }

                // Get the {MATERIAL} attribute value.
                var materialAttributeValue =
                    part.GetUserAttributeAsString("MATERIAL", NXObject.AttributeType.String, -1);

                // If the material attribute values are equal, then we want to un blank all occurrences of this part.
                if (materialAttributeValue?.ToUpper() == selected)
                {
                    Unblank(part);
                    continue;
                }

                BlankOff(part);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var counter = 0;

            foreach (var obj in _blankedOffObjects)
            {
                ufsession_.Ui.SetPrompt($"Unblanking {++counter} of {_blankedOffObjects.Count}");

                if (obj._ToTaggedObject() is DisplayableObject disp)
                    disp.Unblank();
            }
        }
    }
}