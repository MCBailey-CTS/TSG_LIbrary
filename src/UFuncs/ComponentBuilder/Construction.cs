using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using System;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;
using NXOpen.Utilities;
using NXOpen.Preferences;
using TSG_Library.Properties;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {



        private void ButtonEditConstruction_Click(object sender, EventArgs e) => EditConstruction();

        private void ButtonEndEditConstruction_Click(object sender, EventArgs e) => EndEditConstruction();
        public void EditConstruction()
        {
            buttonExit.Enabled = false;
            Component editComponent = SelectOneComponent("Select Component to edit construction");

            if (editComponent is null)
                return;

            if (editComponent.__Prototype().PartUnits != __display_part_.PartUnits)
            {
                MessageBox.Show("Component units do not match the display part units");
                return;
            }

            using (session_.__UsingSuppressDisplay())
            {
                Part addRefSetPart = (Part)editComponent.Prototype;

                using (session_.__UsingDisplayPartReset())
                {
                    __display_part_ = addRefSetPart;

                    using (session_.__UsingDoUpdate("Delete Reference Set"))
                        __display_part_.__ReferenceSets("EDIT").__Delete();

                    // create edit reference set
                    using (session_.__UsingDoUpdate("Create New Reference Set"))
                    {
                        ReferenceSet editRefSet = __work_part_.CreateReferenceSet();
                        NXObject[] removeComps = editRefSet.AskAllDirectMembers();
                        editRefSet.RemoveObjectsFromReferenceSet(removeComps);
                        editRefSet.SetAddComponentsAutomatically(false, false);
                        editRefSet.SetName("EDIT");

                        // get all construction objects to add to reference set
                        List<NXObject> constructionObjects = new List<NXObject>();

                        for (int i = 1; i < 11; i++)
                        {
                            NXObject[] layerObjects = __display_part_.Layers.GetAllObjectsOnLayer(i);

                            foreach (NXObject addObj in layerObjects)
                                constructionObjects.Add(addObj);
                        }

                        editRefSet.AddObjectsToReferenceSet(constructionObjects.ToArray());
                    }

                }
            }
            __work_component_ = editComponent;
            SetWcsToWorkPart(editComponent);
            __work_component_.__Translucency(75);
            editComponent.__ReferenceSet("EDIT");
            __display_part_.Layers.WorkLayer = 3;
            buttonEditConstruction.Enabled = false;
            buttonEndEditConstruction.Enabled = true;
        }

        private void EndEditConstruction()
        {
            try
            {
                __work_component_.__Translucency(0);
                __display_part_.Layers.WorkLayer = 1;

                using (session_.__UsingDoUpdate("Delete Reference Set"))
                {
                    __work_component_.__ReferenceSet("BODY");
                    __work_part_.__ReferenceSets("EDIT").__Delete();
                }

                buttonExit.Enabled = true;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                buttonEditConstruction.Enabled = true;
                buttonEndEditConstruction.Enabled = false;
            }
        }

    }
}
// 2045