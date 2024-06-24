using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    public partial class TapAndReam : Form
    {
        // ReSharper disable once InconsistentNaming
        private static NXOpen.Part workPart = session_.Parts.Work;
        // ReSharper disable once InconsistentNaming
        private static NXOpen.Part originalWorkPart = workPart;

        public TapAndReam() 
        {
            InitializeComponent();
        }


        private void ButtonMoveToEnd_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "ReOrder Feature");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                NXOpen.Features.Feature[] allFeatures = workPart.Features.ToArray();
                NXOpen.Features.Feature lastFeature = allFeatures[allFeatures.Length - 1];
                int lastFeatureTimeStamp = allFeatures[allFeatures.Length - 1].Timestamp;

                List<FeatureGroup> moveFeatures = new List<FeatureGroup>();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        FeatureGroup orderFeat = new FeatureGroup();

                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                orderFeat.linkedBody = feature;
                                orderFeat.booleanFeature = getTool;
                                moveFeatures.Add(orderFeat);
                                break;
                            }
                    }
                }

                int lastFeatureGroupTimeStamp = moveFeatures[moveFeatures.Count - 1].booleanFeature.Timestamp;

                if (lastFeatureTimeStamp > lastFeatureGroupTimeStamp)
                {
                    List<NXOpen.Tag> moveFeatTags = new List<NXOpen.Tag>();

                    foreach (FeatureGroup group in moveFeatures)
                    {
                        moveFeatTags.Add(group.linkedBody.Tag);
                        moveFeatTags.Add(group.booleanFeature.Tag);
                    }

                    const int reOrderAfter = 2;
                    ufsession_.Modl.ReorderFeature(allFeatures[allFeatures.Length - 1].Tag, moveFeatTags.ToArray(), reOrderAfter);

                    lastFeature.MakeCurrentFeature();
                }

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSuppressTaps_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Suppress Taps");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> suppressFeatures = new List<FeatureGroup>();

                FeatureGroup suppressFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                if (cFace.Color != 181) continue;
                                ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                foreach (NXOpen.Tag featTag in features)
                                {
                                    if (featTag != feature.Tag) continue;
                                    suppressFeat.linkedBody = feature;
                                    suppressFeat.booleanFeature = getTool;
                                    suppressFeat.cylindricalFace = cFace;

                                    if (suppressFeatures.Count == 0)
                                    {
                                        suppressFeatures.Add(suppressFeat);
                                        break;
                                    }

                                    FeatureGroup findGroup = suppressFeatures.Find(f => f.cylindricalFace == suppressFeat.cylindricalFace);
                                    if (findGroup.cylindricalFace != null) continue;
                                    suppressFeatures.Add(suppressFeat);
                                    break;
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> suppAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in suppressFeatures)
                {
                    suppAllFeats.Add(fGroup.linkedBody);

                    suppAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (suppAllFeats.Count > 0) workPart.Features.SuppressFeatures(suppAllFeats.ToArray());

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSuppressReams_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Suppress Reams");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> suppressFeatures = new List<FeatureGroup>();

                FeatureGroup suppressFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                if (cFace.Color != 42) continue;
                                ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                foreach (NXOpen.Tag featTag in features)
                                {
                                    if (featTag != feature.Tag) continue;
                                    suppressFeat.linkedBody = feature;
                                    suppressFeat.booleanFeature = getTool;
                                    suppressFeat.cylindricalFace = cFace;

                                    if (suppressFeatures.Count == 0)
                                    {
                                        suppressFeatures.Add(suppressFeat);
                                        break;
                                    }

                                    FeatureGroup findGroup = suppressFeatures.Find(f => f.cylindricalFace == suppressFeat.cylindricalFace);
                                    if (findGroup.cylindricalFace != null) continue;
                                    suppressFeatures.Add(suppressFeat);
                                    break;
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> suppAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in suppressFeatures)
                {
                    suppAllFeats.Add(fGroup.linkedBody);

                    suppAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (suppAllFeats.Count > 0) workPart.Features.SuppressFeatures(suppAllFeats.ToArray());

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonSuppressTapReam_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Suppress Taps");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> suppressFeatures = new List<FeatureGroup>();

                FeatureGroup suppressFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                if (cFace.Color == 181)
                                {
                                    ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                    foreach (NXOpen.Tag featTag in features)
                                    {
                                        if (featTag != feature.Tag) continue;
                                        suppressFeat.linkedBody = feature;
                                        suppressFeat.booleanFeature = getTool;
                                        suppressFeat.cylindricalFace = cFace;

                                        if (suppressFeatures.Count == 0)
                                        {
                                            suppressFeatures.Add(suppressFeat);
                                            break;
                                        }

                                        FeatureGroup findGroup = suppressFeatures.Find(f => f.cylindricalFace == suppressFeat.cylindricalFace);
                                        if (findGroup.cylindricalFace != null) continue;
                                        suppressFeatures.Add(suppressFeat);
                                        break;
                                    }
                                }

                                if (cFace.Color != 42) continue;
                                {
                                    ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                    foreach (NXOpen.Tag featTag in features)
                                    {
                                        if (featTag != feature.Tag) continue;
                                        suppressFeat.linkedBody = feature;
                                        suppressFeat.booleanFeature = getTool;
                                        suppressFeat.cylindricalFace = cFace;

                                        if (suppressFeatures.Count == 0)
                                        {
                                            suppressFeatures.Add(suppressFeat);
                                            break;
                                        }

                                        FeatureGroup findGroup = suppressFeatures.Find(f => f.cylindricalFace == suppressFeat.cylindricalFace);
                                        if (findGroup.cylindricalFace != null) continue;
                                        suppressFeatures.Add(suppressFeat);
                                        break;
                                    }
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> suppAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in suppressFeatures)
                {
                    suppAllFeats.Add(fGroup.linkedBody);

                    suppAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (suppAllFeats.Count > 0) workPart.Features.SuppressFeatures(suppAllFeats.ToArray());

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonDeleteTaps_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete Taps");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> deleteFeatures = new List<FeatureGroup>();

                FeatureGroup deleteFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical || cFace.Name != "HOLECHART") continue;
                                if (cFace.Color != 181) continue;
                                ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                foreach (NXOpen.Tag featTag in features)
                                {
                                    if (featTag != feature.Tag) continue;
                                    deleteFeat.linkedBody = feature;
                                    deleteFeat.booleanFeature = getTool;
                                    deleteFeat.cylindricalFace = cFace;

                                    if (deleteFeatures.Count == 0)
                                    {
                                        deleteFeatures.Add(deleteFeat);
                                        break;
                                    }

                                    FeatureGroup findGroup = deleteFeatures.Find(f => f.cylindricalFace == deleteFeat.cylindricalFace);
                                    if (findGroup.cylindricalFace != null) continue;
                                    deleteFeatures.Add(deleteFeat);
                                    break;
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> delAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in deleteFeatures)
                {
                    delAllFeats.Add(fGroup.linkedBody);

                    delAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (delAllFeats.Count > 0)
                {
                    NXOpen.Session.UndoMarkId markIdDelete = session_.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");
                    session_.UpdateManager.AddToDeleteList(delAllFeats.ToArray<NXOpen.NXObject>());
                    session_.UpdateManager.DoUpdate(markIdDelete);
                }

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonDeleteReams_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete Reams");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> deleteFeatures = new List<FeatureGroup>();

                FeatureGroup deleteFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                if (cFace.Color != 42) continue;
                                ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                foreach (NXOpen.Tag featTag in features)
                                {
                                    if (featTag != feature.Tag) continue;
                                    deleteFeat.linkedBody = feature;
                                    deleteFeat.booleanFeature = getTool;
                                    deleteFeat.cylindricalFace = cFace;

                                    if (deleteFeatures.Count == 0)
                                    {
                                        deleteFeatures.Add(deleteFeat);
                                        break;
                                    }

                                    FeatureGroup findGroup = deleteFeatures.Find(f => f.cylindricalFace == deleteFeat.cylindricalFace);
                                    if (findGroup.cylindricalFace != null) continue;
                                    deleteFeatures.Add(deleteFeat);
                                    break;
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> delAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in deleteFeatures)
                {
                    delAllFeats.Add(fGroup.linkedBody);

                    delAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (delAllFeats.Count > 0)
                {
                    NXOpen.Session.UndoMarkId markIdDelete = session_.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");
                    session_.UpdateManager.AddToDeleteList(delAllFeats.ToArray<NXOpen.NXObject>());
                    session_.UpdateManager.DoUpdate(markIdDelete);
                }

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonDeleteTapsReams_Click(object sender, EventArgs e)
        {
            try
            {
                session_.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete Taps");

                NXOpen.Assemblies.Component selectedComp = SelectOneComponent();

                if (selectedComp == null) return;
                session_.Parts.SetWork((NXOpen.Part)selectedComp.Prototype);

                UpdateSessionParts();

                List<FeatureGroup> deleteFeatures = new List<FeatureGroup>();

                FeatureGroup deleteFeat = new FeatureGroup();

                foreach (NXOpen.Features.Feature feature in workPart.Features)
                {
                    if (feature.FeatureType != "LINKED_BODY") continue;
                    if (feature.Suppressed) continue;
                    NXOpen.Features.Feature[] featChildren = feature.GetChildren();

                    foreach (NXOpen.Features.Feature bFeature in featChildren)
                    {
                        if (bFeature.Suppressed) continue;
                        if (bFeature.FeatureType != "META") continue;
                        NXOpen.Features.BooleanFeature getTool = (NXOpen.Features.BooleanFeature)bFeature;

                        foreach (NXOpen.Body bBody in getTool.GetBodies())
                            foreach (NXOpen.Face bFace in bBody.GetFaces())
                            {
                                ufsession_.Obj.AskTypeAndSubtype(bFace.Tag, out int type, out int subType);

                                if (type != NXOpen.UF.UFConstants.UF_solid_type || subType != NXOpen.UF.UFConstants.UF_solid_face_subtype) continue;
                                NXOpen.Face cFace = (NXOpen.Face)NXOpen.Utilities.NXObjectManager.Get(bFace.Tag);

                                if (cFace.SolidFaceType != NXOpen.Face.FaceType.Cylindrical) continue;
                                if (cFace.Name != "HOLECHART") continue;
                                // ReSharper disable once ConvertIfStatementToSwitchStatement
                                if (cFace.Color == 181)
                                {
                                    ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                    foreach (NXOpen.Tag featTag in features)
                                    {
                                        if (featTag != feature.Tag) continue;
                                        deleteFeat.linkedBody = feature;
                                        deleteFeat.booleanFeature = getTool;
                                        deleteFeat.cylindricalFace = cFace;

                                        if (deleteFeatures.Count == 0)
                                        {
                                            deleteFeatures.Add(deleteFeat);
                                            break;
                                        }

                                        FeatureGroup findGroup = deleteFeatures.Find(f => f.cylindricalFace == deleteFeat.cylindricalFace);
                                        if (findGroup.cylindricalFace != null) continue;
                                        deleteFeatures.Add(deleteFeat);
                                        break;
                                    }
                                }

                                if (cFace.Color != 42) continue;
                                {
                                    ufsession_.Modl.AskFaceFeats(cFace.Tag, out NXOpen.Tag[] features);

                                    foreach (NXOpen.Tag featTag in features)
                                    {
                                        if (featTag != feature.Tag) continue;
                                        deleteFeat.linkedBody = feature;
                                        deleteFeat.booleanFeature = getTool;
                                        deleteFeat.cylindricalFace = cFace;

                                        if (deleteFeatures.Count == 0)
                                        {
                                            deleteFeatures.Add(deleteFeat);
                                            break;
                                        }

                                        FeatureGroup findGroup = deleteFeatures.Find(f => f.cylindricalFace == deleteFeat.cylindricalFace);
                                        if (findGroup.cylindricalFace != null) continue;
                                        deleteFeatures.Add(deleteFeat);
                                        break;
                                    }
                                }
                            }
                    }
                }

                List<NXOpen.Features.Feature> delAllFeats = new List<NXOpen.Features.Feature>();

                foreach (FeatureGroup fGroup in deleteFeatures)
                {
                    delAllFeats.Add(fGroup.linkedBody);

                    delAllFeats.AddRange(fGroup.linkedBody.GetChildren());
                }

                if (delAllFeats.Count > 0)
                {
                    NXOpen.Session.UndoMarkId markIdDelete = session_.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");
                    session_.UpdateManager.AddToDeleteList(delAllFeats.ToArray<NXOpen.NXObject>());
                    session_.UpdateManager.DoUpdate(markIdDelete);
                }

                session_.Parts.SetWork(originalWorkPart);

                UpdateSessionParts();
                UpdateOriginalParts();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private struct FeatureGroup
        {
            // ReSharper disable once InconsistentNaming
            public NXOpen.Features.Feature linkedBody;
            // ReSharper disable once InconsistentNaming
            public NXOpen.Features.BooleanFeature booleanFeature;
            // ReSharper disable once InconsistentNaming
            public NXOpen.Face cylindricalFace;
        }


        private static NXOpen.Assemblies.Component SelectOneComponent()
        {
            NXOpen.Selection.MaskTriple[] mask = new NXOpen.Selection.MaskTriple[1];
            mask[0] = new NXOpen.Selection.MaskTriple(NXOpen.UF.UFConstants.UF_component_type, 0, 0);

            try
            {
                NXOpen.Selection.Response sel = uisession_.SelectionManager.SelectTaggedObject("Select component", "Select Component",
                    NXOpen.Selection.SelectionScope.AnyInAssembly,
                    NXOpen.Selection.SelectionAction.ClearAndEnableSpecific,
                    false, false, mask, out NXOpen.TaggedObject selectedComp, out _);

                if (!((sel == NXOpen.Selection.Response.ObjectSelected) | (sel == NXOpen.Selection.Response.ObjectSelectedByName))) return null;
                NXOpen.Assemblies.Component compSelection = (NXOpen.Assemblies.Component)selectedComp;
                return compSelection;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
                return null;
            }
        }


        private static void UpdateSessionParts()
        {
            workPart = session_.Parts.Work;
        }

        private static void UpdateOriginalParts()
        {
            originalWorkPart = session_.Parts.Work;
        }
    }
}
