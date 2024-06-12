using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Features;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.Geom;
using TSG_Library.Properties;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs.MirrorComponents.Features
{
    public class MainForm : Form
    {
        private static readonly UI TheUi = UI.GetUI();

        private static Part _originalWorkPart = __work_part_;

        private static Part _originalDisplayPart = __display_part_;

        private static NXObject[] _selectedComponents;

        private static readonly List<NXOpen.Assemblies.Component> MirrorComponents = new List<NXOpen.Assemblies.Component>();

        private static DatumPlane mirrorPlane;

        private static string _formatCompName = string.Empty;

        private static double inputCompName;

        private static Surface.Plane _mirrorPlane;

        private System.ComponentModel.IContainer components = null;

        private Button buttonSelectComponents;

        private TextBox textBoxDetNumber;

        private CheckBox checkBoxMirrorCopies;

        private GroupBox groupBox1;

        private Label label1;

        private Button buttonExit;

        private Button buttonOk;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            base.Location = Settings.Default.mirror_components_window_location;
            session_.Parts.LoadOptions.LoadLatest = false;
            session_.Parts.LoadOptions.ComponentLoadMethod = LoadOptions.LoadMethod.AsSaved;
            session_.Parts.LoadOptions.ComponentsToLoad = LoadOptions.LoadComponents.All;
            session_.Parts.LoadOptions.UseLightweightRepresentations = false;
            session_.Parts.LoadOptions.UsePartialLoading = false;
            session_.Parts.LoadOptions.SetInterpartData(interpartOption: true, LoadOptions.Parent.Immediate);
            session_.Parts.LoadOptions.AbortOnFailure = false;
            session_.Parts.LoadOptions.AllowSubstitution = false;
            session_.Parts.LoadOptions.GenerateMissingPartFamilyMembers = false;
            session_.Parts.LoadOptions.ReferenceSetOverride = false;
            textBoxDetNumber.Text = "001";
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Hide();
            bool interruptUpdateOnError = session_.Preferences.Modeling.InterruptUpdateOnError;
            bool interruptUpdateOnWarning = session_.Preferences.Modeling.InterruptUpdateOnWarning;
            try
            {
                session_.Preferences.Modeling.InterruptUpdateOnError = false;
                session_.Preferences.Modeling.InterruptUpdateOnWarning = false;
                IDictionary<TaggedObject, TaggedObject> dictionary = new Dictionary<TaggedObject, TaggedObject>();
                try
                {
                    UpdateSessionParts();
                    UpdateOriginalParts();
                    if (_selectedComponents != null && mirrorPlane != null)
                    {
                        if (checkBoxMirrorCopies.Checked)
                        {
                            NewMethod(dictionary);
                        }
                        else
                        {
                            NXObject[] selectedComponents = _selectedComponents;
                            foreach (NXObject nXObject in selectedComponents)
                            {
                                if (nXObject.OwningComponent != null)
                                {
                                    MirrorComponents.Add(nXObject.OwningComponent);
                                }
                            }
                        }

                        if (MirrorComponents.Count > 0)
                        {
                            ufsession_.Disp.SetDisplay(0);
                            NewMethod17();
                        }
                        else
                        {
                            NewMethod20();
                        }

                        ufsession_.Disp.SetDisplay(1);
                        __display_part_.Views.Regenerate();
                        __display_part_ = _originalDisplayPart;
                        __work_part_ = _originalWorkPart;
                        UpdateSessionParts();
                        ResetForm();
                    }
                }
                catch (Exception ex)
                {
                    ufsession_.Disp.SetDisplay(1);
                    ex.__PrintException();
                }

                NXOpen.Assemblies.Component[] array = dictionary.Keys.OfType<NXOpen.Assemblies.Component>().ToArray();
                NXOpen.Assemblies.Component[] array2 = array;
                foreach (NXOpen.Assemblies.Component originalComp in array2)
                {
                    Program.Mirror208_3001(_mirrorPlane, originalComp, dictionary);
                }
            }
            finally
            {
                Show();
                session_.Preferences.Modeling.InterruptUpdateOnError = interruptUpdateOnError;
                session_.Preferences.Modeling.InterruptUpdateOnWarning = interruptUpdateOnWarning;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.mirror_components_window_location = base.Location;
            Settings.Default.Save();
        }

        private void ButtonSelectComponents_Click(object sender, EventArgs e)
        {
            //IL_0029: Unknown result type (might be due to invalid IL or missing references)
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            //IL_003d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0047: Expected O, but got Unknown
            _selectedComponents = SelectMultipleComponentBodies().Cast<NXObject>().ToArray();
            mirrorPlane = SelectMirrorDatumPlane();
            _mirrorPlane = new Surface.Plane(mirrorPlane.Origin, mirrorPlane.Normal);
            buttonOk.Enabled = _selectedComponents != null;
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        public static bool FormatDetailNumber(double compName)
        {
            try
            {
                if (!(compName > 0.0) || !(compName < 991.0))
                {
                    return false;
                }

                if (compName > 0.0 && compName < 10.0)
                {
                    _formatCompName = $"00{compName}";
                    return true;
                }

                if (compName > 9.0 && compName < 100.0)
                {
                    _formatCompName = $"0{compName}";
                    return true;
                }

                if (!(compName > 100.0))
                {
                    return false;
                }

                _formatCompName = compName.ToString();
                return true;
            }
            catch (NXException ex)
            {
                ex.__PrintException();
                return false;
            }
        }

        private static bool CopyComponentToMirror(NXOpen.Assemblies.Component copyComponent, IDictionary<TaggedObject, TaggedObject> mirrorDict)
        {
            try
            {
                if (copyComponent == null)
                {
                    return false;
                }

                Part part = (Part)copyComponent.Prototype;
                double num = inputCompName;
                string fullPath = part.FullPath;
                string text = part.FullPath.Remove(part.FullPath.Length - 7) + _formatCompName + ".prt";
                while (File.Exists(text))
                {
                    num += 1.0;
                    FormatDetailNumber(num);
                    text = part.FullPath.Remove(part.FullPath.Length - 7) + _formatCompName + ".prt";
                    inputCompName = num;
                }

                int num2 = __work_part_.FullPath.LastIndexOf("\\", StringComparison.Ordinal);
                string text2 = __work_part_.FullPath.Remove(num2 + 1);
                string text3 = text.Remove(num2 + 1);
                if (text2 == text3)
                {
                    if (File.Exists(fullPath))
                    {
                        File.Copy(fullPath, text);
                        PartLoadStatus loadStatus;
                        BasePart basePart = session_.Parts.OpenBase(text, out loadStatus);
                        loadStatus.Dispose();
                        Part partToAdd = (Part)basePart;
                        copyComponent.GetPosition(out var position, out var orientation);
                        int layer = copyComponent.Layer;
                        PartLoadStatus loadStatus2;
                        NXOpen.Assemblies.Component component = __work_part_.ComponentAssembly.AddComponent(partToAdd, "BODY", _formatCompName, position, orientation, layer, out loadStatus2);
                        loadStatus2.Dispose();
                        mirrorDict.Add(copyComponent, component);
                        Feature[] array = copyComponent.__Prototype().Features.ToArray();
                        Feature[] array2 = component.__Prototype().Features.ToArray();
                        for (int i = 0; i < array.Length; i++)
                        {
                            mirrorDict.Add(array[i], array2[i]);
                        }

                        Tag _object = NXOpen.Tag.Null;
                        do
                        {
                            ufsession_.Obj.CycleObjsInPart(__work_part_.Tag, 64, ref _object);
                            if (_object == NXOpen.Tag.Null)
                            {
                                break;
                            }

                            ufsession_.Obj.AskName(_object, out var name);
                            if (!(name != "BODY"))
                            {
                                Tag[] array3 = new Tag[1] { component.Tag };
                                ufsession_.Assem.AddRefSetMembers(_object, array3.Length, array3);
                                ufsession_.Assem.ReplaceRefset(array3.Length, array3, "BODY");
                            }
                        }
                        while (_object != 0);
                        MirrorComponents.Add(component);
                        return true;
                    }

                    TheUi.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, "Save the original file to the assembly before making a copy");
                    return false;
                }

                TheUi.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, "The selected component is not from the work part directory. \n                   Save As before making a copy.");
                return false;
            }
            catch (NXException ex)
            {
                UI.GetUI().NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error, ex.Message);
                return false;
            }
        }

        private static void UpdateSessionParts()
        {
        }

        private static void UpdateOriginalParts()
        {
            _originalWorkPart = __work_part_;
            _originalDisplayPart = __display_part_;
        }

        private void ResetForm()
        {
            Array.Clear(_selectedComponents, 0, _selectedComponents.Length);
            MirrorComponents.Clear();
            mirrorPlane = null;
            _formatCompName = string.Empty;
            inputCompName = 0.0;
            checkBoxMirrorCopies.Checked = false;
            textBoxDetNumber.Text = string.Empty;
            buttonOk.Enabled = false;
        }

        private void CheckBoxMirrorCopies_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDetNumber.Enabled = checkBoxMirrorCopies.Checked;
        }

        private void NewMethod20()
        {
            Session.UndoMarkId undoMarkId = session_.SetUndoMark(Session.MarkVisibility.Visible, "Mirror Components");
            UpdateSessionParts();
            if (!__work_part_.__HasDynamicBlock())
            {
                NewMethod28();
                return;
            }

            SetWcsToWorkPart(null);
            double[] origin_point = new double[3]
            {
            mirrorPlane.Origin.X,
            mirrorPlane.Origin.Y,
            mirrorPlane.Origin.Z
            };
            double[] plane_normal = new double[3]
            {
            mirrorPlane.Normal.X,
            mirrorPlane.Normal.Y,
            mirrorPlane.Normal.Z
            };
            ufsession_.Modl.CreatePlane(origin_point, plane_normal, out var plane_tag);
            if (mirrorPlane != null)
            {
                plane_tag = NewMethod27(plane_tag);
            }
        }

        private void NewMethod17()
        {
            //IL_0070: Unknown result type (might be due to invalid IL or missing references)
            //IL_0075: Unknown result type (might be due to invalid IL or missing references)
            foreach (NXOpen.Assemblies.Component mirrorComponent in MirrorComponents)
            {
                Part part = (Part)mirrorComponent.Prototype;
                BasePart.Units partUnits = part.PartUnits;
                __display_part_ = _originalDisplayPart;
                __work_part_ = _originalWorkPart;
                UpdateSessionParts();
                if (__display_part_.PartUnits == partUnits)
                {
                    __work_part_ = part;
                    UpdateSessionParts();
                    if (!mirrorComponent.__Origin().__IsEqualTo(__Extensions_._Point3dOrigin))
                    {
                        NewMethod26(mirrorComponent);
                        continue;
                    }

                    __display_part_ = __work_part_;
                    UpdateSessionParts();
                    bool isBlockComp = __work_part_.__HasDynamicBlock();
                    NewMethod25(mirrorComponent, isBlockComp);
                }
            }
        }

        private void NewMethod28()
        {
            ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            double[] origin_point = new double[3]
            {
            mirrorPlane.Origin.X,
            mirrorPlane.Origin.Y,
            mirrorPlane.Origin.Z
            };
            double[] plane_normal = new double[3]
            {
            mirrorPlane.Normal.X,
            mirrorPlane.Normal.Y,
            mirrorPlane.Normal.Z
            };
            ufsession_.Modl.CreatePlane(origin_point, plane_normal, out var plane_tag);
            plane_tag = NewMethod12(plane_tag);
        }

        private Tag NewMethod27(Tag selectedPlane)
        {
            double[] array = new double[12];
            ufsession_.Trns.CreateReflectionMatrix(ref selectedPlane, array, out var _);
            CartesianCoordinateSystem cartesianCoordinateSystem = __display_part_.CoordinateSystems.CreateCoordinateSystem(__display_part_.WCS.Origin, __display_part_.WCS.CoordinateSystem.Orientation.Element, isTemporary: true);
            List<Tag> list = new List<Tag> { cartesianCoordinateSystem.Tag };
            TransformObjects(list, array);
            __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180.0);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
            ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            NewMethod2(selectedPlane);
            list.Clear();
            NewMethod6(array, list);
            list.Clear();
            list.AddRange(GetCurves());
            NewMethod22(list);
            TransformObjects(list, array);
            list.Clear();
            NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
            NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
            return selectedPlane;
        }

        private void NewMethod26(NXOpen.Assemblies.Component mirrorComp)
        {
            Feature waveMirrorFeature = WaveLinkDatumPlane(mirrorPlane);
            ufsession_.Disp.SetDisplay(0);
            mirrorComp.Unhighlight();
            __display_part_ = (Part)mirrorComp.Prototype;
            UpdateSessionParts();
            if (__work_part_.__HasDynamicBlock())
            {
                SetWcsToWorkPart(mirrorComp);
                NewMethod14(waveMirrorFeature);
            }
            else
            {
                NewMethod15(waveMirrorFeature);
            }
        }

        private void NewMethod25(NXOpen.Assemblies.Component mirrorComp, bool isBlockComp)
        {
            double[] origin_point = mirrorPlane.Origin._Array();
            double[] plane_normal = mirrorPlane.Normal._Array();
            Tag plane_tag;
            if (isBlockComp)
            {
                SetWcsToWorkPart(mirrorComp);
                ufsession_.Modl.CreatePlane(origin_point, plane_normal, out plane_tag);
                plane_tag = NewMethod9(plane_tag);
                return;
            }

            ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            ufsession_.Modl.CreatePlane(origin_point, plane_normal, out plane_tag);
            if (mirrorPlane != null)
            {
                plane_tag = NewMethod23(plane_tag);
            }
        }

        private Tag NewMethod23(Tag selectedPlane)
        {
            double[] array = new double[12];
            ufsession_.Trns.CreateReflectionMatrix(ref selectedPlane, array, out var _);
            __Extensions_.session_.__DeleteObjects(selectedPlane);
            ufsession_.Modl.Update();
            List<Tag> list = NewMethod11();
            TransformObjects(list, array);
            ufsession_.Modl.Update();
            list.Clear();
            NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
            NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
            list.Clear();
            return selectedPlane;
        }

        private void NewMethod15(Feature waveMirrorFeature)
        {
            if (waveMirrorFeature != null)
            {
                ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
                ufsession_.Modl.AskFeatObject(waveMirrorFeature.Tag, out var _, out var eids);
                DatumPlane datumPlane = (DatumPlane)NXObjectManager.Get(eids[0]);
                double[] origin_point = new double[3]
                {
                datumPlane.Origin.X,
                datumPlane.Origin.Y,
                datumPlane.Origin.Z
                };
                double[] plane_normal = new double[3]
                {
                datumPlane.Normal.X,
                datumPlane.Normal.Y,
                datumPlane.Normal.Z
                };
                ufsession_.Modl.CreatePlane(origin_point, plane_normal, out var plane_tag);
                double[] array = new double[12];
                ufsession_.Trns.CreateReflectionMatrix(ref plane_tag, array, out var _);
                DeleteObjects(datumPlane, waveMirrorFeature, (NXObject)plane_tag.__ToTaggedObject());
                ufsession_.Modl.Update();
                List<Tag> list = new List<Tag>(GetCurves());
                NewMethod29(eids, list);
                TransformObjects(list, array);
                ufsession_.Modl.Update();
                list.Clear();
                NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
                NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
                list.Clear();
            }
        }

        private void NewMethod14(Feature waveMirrorFeature)
        {
            if (waveMirrorFeature != null)
            {
                ufsession_.Modl.AskFeatObject(waveMirrorFeature.Tag, out var n_eids, out var eids);
                DatumPlane datumPlane = (DatumPlane)NXObjectManager.Get(eids[0]);
                double[] origin_point = new double[3]
                {
                datumPlane.Origin.X,
                datumPlane.Origin.Y,
                datumPlane.Origin.Z
                };
                double[] plane_normal = new double[3]
                {
                datumPlane.Normal.X,
                datumPlane.Normal.Y,
                datumPlane.Normal.Z
                };
                ufsession_.Modl.CreatePlane(origin_point, plane_normal, out var plane_tag);
                double[] array = new double[12];
                ufsession_.Trns.CreateReflectionMatrix(ref plane_tag, array, out n_eids);
                CartesianCoordinateSystem cartesianCoordinateSystem = __display_part_.CoordinateSystems.CreateCoordinateSystem(__display_part_.WCS.Origin, __display_part_.WCS.CoordinateSystem.Orientation.Element, isTemporary: true);
                List<Tag> list = new List<Tag> { cartesianCoordinateSystem.Tag };
                TransformObjects(list, array);
                __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
                __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180.0);
                __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
                ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
                NewMethod4(waveMirrorFeature, datumPlane, plane_tag);
                list.Clear();
                NewMethod6(array, list);
                list.Clear();
                list.AddRange(GetCurves());
                NewMethod30(eids, list);
                TransformObjects(list, array);
                list.Clear();
                NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
                NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
            }
        }

        private static void NewMethod29(Tag[] featObjects, List<Tag> mirrorMove)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "ABSOLUTE_DATUM_PLANE"))
                {
                    ufsession_.Modl.AskFeatObject(feature.Tag, out var _, out var _);
                    TaggedObject taggedObject = NXObjectManager.Get(featObjects[0]);
                    NXObject nXObject = (NXObject)taggedObject;
                    DatumPlane datumPlane = (DatumPlane)NXObjectManager.Get(featObjects[0]);
                    if (datumPlane.Layer > 0 && datumPlane.Layer < 21)
                    {
                        mirrorMove.Add(datumPlane.Tag);
                    }
                }
            }
        }

        private Tag NewMethod12(Tag selectedPlane)
        {
            if (mirrorPlane == null)
            {
                return selectedPlane;
            }

            double[] array = new double[12];
            ufsession_.Trns.CreateReflectionMatrix(ref selectedPlane, array, out var _);
            DeleteObjects(new NXObject[] { (NXObject)selectedPlane.__ToTaggedObject() });
            ufsession_.Modl.Update();
            List<Tag> list = new List<Tag>(GetCurves());
            NewMethod22(list);
            TransformObjects(list, array);
            ufsession_.Modl.Update();
            list.Clear();
            NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
            NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
            list.Clear();
            return selectedPlane;
        }

        private static List<Tag> NewMethod11()
        {
            List<Tag> list = new List<Tag>(GetCurves());
            NewMethod22(list);
            return list;
        }

        private Tag NewMethod9(Tag selectedPlane)
        {
            if (mirrorPlane == null)
            {
                return selectedPlane;
            }

            double[] array = new double[12];
            ufsession_.Trns.CreateReflectionMatrix(ref selectedPlane, array, out var _);
            CartesianCoordinateSystem cartesianCoordinateSystem = __display_part_.CoordinateSystems.CreateCoordinateSystem(__display_part_.WCS.Origin, __display_part_.WCS.CoordinateSystem.Orientation.Element, isTemporary: true);
            List<Tag> list = new List<Tag> { cartesianCoordinateSystem.Tag };
            TransformObjects(list, array);
            __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180.0);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
            ufsession_.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            NewMethod2(selectedPlane);
            list.Clear();
            NewMethod3(array, list);
            list.Clear();
            list.AddRange(GetCurves());
            NewMethod22(list);
            TransformObjects(list, array);
            list.Clear();
            NewMethod33(out var fasteners, out var fastenerOrigins, out var fastenerMatrix);
            NewMethod10(array, list, fasteners, fastenerOrigins, fastenerMatrix);
            return selectedPlane;
        }

        private void NewMethod10(double[] mirrorMatrix, List<Tag> mirrorMove, List<NXOpen.Assemblies.Component> fasteners, List<Point3d> fastenerOrigins, List<Matrix3x3> fastenerMatrix)
        {
            for (int i = 0; i < fasteners.Count; i++)
            {
                Tag[] array = new Tag[1];
                CartesianCoordinateSystem cartesianCoordinateSystem = __display_part_.CoordinateSystems.CreateCoordinateSystem(fastenerOrigins[i], fastenerMatrix[i], isTemporary: true);
                mirrorMove.Add(cartesianCoordinateSystem.Tag);
                TransformObjects(mirrorMove, mirrorMatrix);
                __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
                __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180.0);
                __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
                Point3d origin = __display_part_.WCS.Origin;
                Matrix3x3 element = __display_part_.WCS.CoordinateSystem.Orientation.Element;
                double[] new_csys_matrix = new double[9]
                {
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Xx,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Xy,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Xz,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Yx,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Yy,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Yz,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Zx,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Zy,
                __display_part_.WCS.CoordinateSystem.Orientation.Element.Zz
                };
                double[] new_origin = new double[3]
                {
                __display_part_.WCS.Origin.X,
                __display_part_.WCS.Origin.Y,
                __display_part_.WCS.Origin.Z
                };
                Tag instance = ufsession_.Assem.AskInstOfPartOcc(fasteners[i].Tag);
                ufsession_.Assem.RepositionInstance(instance, new_origin, new_csys_matrix);
                mirrorMove.Clear();
            }
        }

        private void NewMethod6(double[] mirrorMatrix, List<Tag> mirrorMove)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "BLOCK") && !(feature.Name != "FEATURE BLOCK"))
                {
                    BlockFeatureBuilder blockFeatureBuilder = __work_part_.Features.CreateBlockFeatureBuilder(feature);
                    using (new Destroyer(blockFeatureBuilder))
                    {
                        NewMethod8(mirrorMatrix, mirrorMove, blockFeatureBuilder);
                    }

                    ufsession_.Modl.Update();
                }
            }
        }

        private void NewMethod4(Feature waveMirrorFeature, DatumPlane waveMirrorPlane, Tag selectedPlane)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "BLOCK") && !(feature.Name != "DYNAMIC BLOCK"))
                {
                    BlockFeatureBuilder blockFeatureBuilder = __work_part_.Features.CreateBlockFeatureBuilder(feature);
                    using (new Destroyer(blockFeatureBuilder))
                    {
                        NewMethod1(blockFeatureBuilder);
                    }

                    DeleteObjects(waveMirrorFeature, waveMirrorPlane, (NXObject)selectedPlane.__ToTaggedObject());
                    ufsession_.Modl.Update();
                }
            }
        }

        private void NewMethod2(Tag selectedPlane)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "BLOCK") && !(feature.Name != "DYNAMIC BLOCK"))
                {
                    BlockFeatureBuilder blockFeatureBuilder = __work_part_.Features.CreateBlockFeatureBuilder(feature);
                    using (new Destroyer(blockFeatureBuilder))
                    {
                        NewMethod1(blockFeatureBuilder);
                    }

                    ufsession_.Modl.Update();
                }
            }
        }

        private void NewMethod3(double[] mirrorMatrix, List<Tag> mirrorMove)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "BLOCK") && !(feature.Name != "FEATURE BLOCK"))
                {
                    Block block = (Block)feature;
                    BlockFeatureBuilder blockFeatureBuilder = __work_part_.Features.CreateBlockFeatureBuilder(block);
                    using (new Destroyer(blockFeatureBuilder))
                    {
                        NewMethod8(mirrorMatrix, mirrorMove, blockFeatureBuilder);
                    }

                    __work_part_.FacetedBodies.DeleteTemporaryFacesAndEdges();
                    ufsession_.Modl.Update();
                }
            }
        }

        private void NewMethod(IDictionary<TaggedObject, TaggedObject> mirrorDict)
        {
            bool flag = double.TryParse(textBoxDetNumber.Text, out inputCompName);
            if (!FormatDetailNumber(inputCompName))
            {
                return;
            }

            for (int i = 0; i < _selectedComponents.Length; i++)
            {
                prompt_($"Mirroring: ({i + 1} of {_selectedComponents.Length})");
                NXObject nXObject = _selectedComponents[i];
                if (nXObject.OwningComponent != null)
                {
                    CopyComponentToMirror(nXObject.OwningComponent, mirrorDict);
                }
            }
        }

        private static void NewMethod1(BlockFeatureBuilder blockFeatureBuilder1)
        {
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            //IL_005c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0060: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Unknown result type (might be due to invalid IL or missing references)
            //IL_009b: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a2: Unknown result type (might be due to invalid IL or missing references)
            string rightHandSide = blockFeatureBuilder1.Width.RightHandSide;
            string rightHandSide2 = blockFeatureBuilder1.Length.RightHandSide;
            string rightHandSide3 = blockFeatureBuilder1.Height.RightHandSide;
            Point3d origin = __display_part_.WCS.Origin;
            Matrix3x3 val = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            Vector3d axisX = val.__AxisX();
            Vector3d axisY = val.__AxisY();
            NXOpen.Point point = __work_part_.Points.CreatePoint(origin);
            point.SetCoordinates(origin);
            blockFeatureBuilder1.OriginPoint = point;
            Point3d originPoint = origin;
            blockFeatureBuilder1.SetOriginAndLengths(originPoint, rightHandSide, rightHandSide2, rightHandSide3);
            blockFeatureBuilder1.SetOrientation(axisX, axisY);
            blockFeatureBuilder1.CommitFeature();
        }

        private void NewMethod8(double[] mirrorMatrix, List<Tag> mirrorMove, BlockFeatureBuilder blockFeatureBuilder2)
        {
            //IL_0170: Unknown result type (might be due to invalid IL or missing references)
            //IL_0175: Unknown result type (might be due to invalid IL or missing references)
            //IL_0179: Unknown result type (might be due to invalid IL or missing references)
            //IL_017e: Unknown result type (might be due to invalid IL or missing references)
            //IL_01ba: Unknown result type (might be due to invalid IL or missing references)
            //IL_01c1: Unknown result type (might be due to invalid IL or missing references)
            blockFeatureBuilder2.GetOrientation(out var xAxis, out var yAxis);
            double[] csys_origin = blockFeatureBuilder2.Origin._Array();
            double[] x_vec = xAxis._Array();
            double[] y_vec = yAxis._Array();
            double[] array = new double[9];
            ufsession_.Mtx3.Initialize(x_vec, y_vec, array);
            ufsession_.Csys.CreateMatrix(array, out var matrix_id);
            ufsession_.Csys.CreateTempCsys(csys_origin, matrix_id, out var csys_id);
            CartesianCoordinateSystem cartesianCoordinateSystem = (CartesianCoordinateSystem)NXObjectManager.Get(csys_id);
            mirrorMove.Add(cartesianCoordinateSystem.Tag);
            TransformObjects(mirrorMove, mirrorMatrix);
            __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180.0);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90.0);
            Point3d origin = __display_part_.WCS.Origin;
            Matrix3x3 element = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            CartesianCoordinateSystem cartesianCoordinateSystem2 = __display_part_.CoordinateSystems.CreateCoordinateSystem(origin, element, isTemporary: true);
            string rightHandSide = blockFeatureBuilder2.Width.RightHandSide;
            string rightHandSide2 = blockFeatureBuilder2.Length.RightHandSide;
            string rightHandSide3 = blockFeatureBuilder2.Height.RightHandSide;
            Point3d origin2 = cartesianCoordinateSystem2.Origin;
            Matrix3x3 val = cartesianCoordinateSystem2.Orientation.Element;
            Vector3d axisX = val.__AxisX();
            Vector3d axisY = val.__AxisY();
            NXOpen.Point point = __work_part_.Points.CreatePoint(origin2);
            point.SetCoordinates(origin2);
            blockFeatureBuilder2.OriginPoint = point;
            Point3d originPoint = origin2;
            blockFeatureBuilder2.SetOriginAndLengths(originPoint, rightHandSide, rightHandSide2, rightHandSide3);
            blockFeatureBuilder2.SetOrientation(axisX, axisY);
            blockFeatureBuilder2.CommitFeature();
        }

        public static Feature WaveLinkDatumPlane(DatumPlane selectedDatum)
        {
            WaveLinkBuilder waveLinkBuilder = __work_part_.BaseFeatures.CreateWaveLinkBuilder(null);
            using (new Destroyer(waveLinkBuilder))
            {
                waveLinkBuilder.Type = WaveLinkBuilder.Types.DatumLink;
                waveLinkBuilder.WaveDatumBuilder.DisplayScale = 2.0;
                waveLinkBuilder.WaveDatumBuilder.Associative = true;
                waveLinkBuilder.WaveDatumBuilder.HideOriginal = false;
                waveLinkBuilder.WaveDatumBuilder.InheritDisplayProperties = false;
                waveLinkBuilder.WaveDatumBuilder.Datums.Add(selectedDatum);
                return waveLinkBuilder.CommitFeature();
            }
        }

        public static void NewMethod22(List<Tag> mirrorMove)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "ABSOLUTE_DATUM_PLANE"))
                {
                    ufsession_.Modl.AskFeatObject(feature.Tag, out var _, out var eids);
                    DatumPlane datumPlane = (DatumPlane)NXObjectManager.Get(eids[0]);
                    if (datumPlane.Layer > 0 && datumPlane.Layer < 21)
                    {
                        mirrorMove.Add(datumPlane.Tag);
                    }
                }
            }
        }

        public static void NewMethod33(out List<NXOpen.Assemblies.Component> fasteners, out List<Point3d> fastenerOrigins, out List<Matrix3x3> fastenerMatrix)
        {
            fasteners = new List<NXOpen.Assemblies.Component>();
            fastenerOrigins = new List<Point3d>();
            fastenerMatrix = new List<Matrix3x3>();
            if (__work_part_.ComponentAssembly.RootComponent == null)
            {
                return;
            }

            NXOpen.Assemblies.Component[] children = __work_part_.ComponentAssembly.RootComponent.GetChildren();
            foreach (NXOpen.Assemblies.Component item in children)
            {
                fasteners.Add(item);
            }

            foreach (NXOpen.Assemblies.Component fastener in fasteners)
            {
                fastener.GetPosition(out var position, out var orientation);
                fastenerOrigins.Add(position);
                fastenerMatrix.Add(orientation);
            }
        }

        public static IEnumerable<Tag> GetCurves()
        {
            foreach (NXOpen.Curve curve in __work_part_.Curves)
            {
                if (curve.Layer > 0 && curve.Layer < 21)
                {
                    yield return curve.Tag;
                }
            }
        }

        public static void TransformObjects(List<Tag> objs, double[] reflectionMatrix)
        {
            int n_objects = objs.Count;
            int move_or_copy = 1;
            int dest_layer = 0;
            int trace_curves = 2;
            Tag[] copies = new Tag[objs.Count];
            ufsession_.Trns.TransformObjects(reflectionMatrix, objs.ToArray(), ref n_objects, ref move_or_copy, ref dest_layer, ref trace_curves, copies, out var _, out var _);
            ufsession_.Modl.Update();
        }

        public static void DeleteObjects(params NXObject[] deleteObjects)
        {
            Session.UndoMarkId undoMarkId = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
            foreach (NXObject @object in deleteObjects)
            {
                session_.UpdateManager.AddToDeleteList(@object);
            }

            session_.UpdateManager.DoUpdate(undoMarkId);
            session_.DeleteUndoMark(undoMarkId, "");
        }

        public static void SetWcsToWorkPart(NXOpen.Assemblies.Component compRefCsys)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "BLOCK") && !(feature.Name != "DYNAMIC BLOCK"))
                {
                    Block block = (Block)feature;
                    BlockFeatureBuilder blockFeatureBuilder = __work_part_.Features.CreateBlockFeatureBuilder(block);
                    using (new Destroyer(blockFeatureBuilder))
                    {
                        Point3d origin = blockFeatureBuilder.Origin;
                        blockFeatureBuilder.GetOrientation(out var xAxis, out var yAxis);
                        double[] csys_origin = new double[3] { origin.X, origin.Y, origin.Z };
                        double[] x_vec = new double[3] { xAxis.X, xAxis.Y, xAxis.Z };
                        double[] y_vec = new double[3] { yAxis.X, yAxis.Y, yAxis.Z };
                        double[] array = new double[9];
                        ufsession_.Mtx3.Initialize(x_vec, y_vec, array);
                        ufsession_.Csys.CreateMatrix(array, out var matrix_id);
                        ufsession_.Csys.CreateTempCsys(csys_origin, matrix_id, out var csys_id);
                        CartesianCoordinateSystem cartesianCoordinateSystem = (CartesianCoordinateSystem)NXObjectManager.Get(csys_id);
                        __display_part_.WCS.SetOriginAndMatrix(cartesianCoordinateSystem.Origin, cartesianCoordinateSystem.Orientation.Element);
                    }
                }
            }
        }

        public static DatumPlane SelectMirrorDatumPlane()
        {
            Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1]
            {
            new Selection.MaskTriple(197, 0, 0)
            };
            TaggedObject @object;
            Point3d cursor;
            Selection.Response response = uisession_.SelectionManager.SelectTaggedObject("Select Datum Plane", "Select Datum Plane", Selection.SelectionScope.AnyInAssembly, Selection.SelectionAction.ClearAndEnableSpecific, includeFeatures: false, keepHighlighted: false, maskArray, out @object, out cursor);
            if (response == Selection.Response.ObjectSelected || response == Selection.Response.ObjectSelectedByName)
            {
                return (DatumPlane)@object;
            }

            return null;
        }

        public static TaggedObject[] SelectMultipleComponentBodies()
        {
            Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1]
            {
            new Selection.MaskTriple(70, 0, 0)
            };
            TaggedObject[] objectArray;
            Selection.Response response = uisession_.SelectionManager.SelectTaggedObjects("Select Component", "Select Component", Selection.SelectionScope.AnyInAssembly, Selection.SelectionAction.ClearAndEnableSpecific, includeFeatures: false, keepHighlighted: false, maskArray, out objectArray);
            if (response == Selection.Response.Ok || response == Selection.Response.ObjectSelected || response == Selection.Response.ObjectSelectedByName)
            {
                return objectArray;
            }

            return null;
        }

        public static void NewMethod30(Tag[] featObjects, List<Tag> mirrorMove)
        {
            foreach (Feature feature in __work_part_.Features)
            {
                if (!(feature.FeatureType != "ABSOLUTE_DATUM_PLANE"))
                {
                    ufsession_.Modl.AskFeatObject(feature.Tag, out var _, out var _);
                    TaggedObject taggedObject = NXObjectManager.Get(featObjects[0]);
                    DatumPlane datumPlane = (DatumPlane)NXObjectManager.Get(featObjects[0]);
                    if (datumPlane.Layer > 0 && datumPlane.Layer < 21)
                    {
                        mirrorMove.Add(datumPlane.Tag);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.buttonSelectComponents = new System.Windows.Forms.Button();
            this.textBoxDetNumber = new System.Windows.Forms.TextBox();
            this.checkBoxMirrorCopies = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.buttonSelectComponents.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSelectComponents.Location = new System.Drawing.Point(18, 12);
            this.buttonSelectComponents.Name = "buttonSelectComponents";
            this.buttonSelectComponents.Size = new System.Drawing.Size(178, 23);
            this.buttonSelectComponents.TabIndex = 0;
            this.buttonSelectComponents.Text = "Select Components";
            this.buttonSelectComponents.UseVisualStyleBackColor = true;
            this.buttonSelectComponents.Click += new System.EventHandler(ButtonSelectComponents_Click);
            this.textBoxDetNumber.Enabled = false;
            this.textBoxDetNumber.Location = new System.Drawing.Point(59, 49);
            this.textBoxDetNumber.Name = "textBoxDetNumber";
            this.textBoxDetNumber.Size = new System.Drawing.Size(72, 20);
            this.textBoxDetNumber.TabIndex = 1;
            this.checkBoxMirrorCopies.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxMirrorCopies.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkBoxMirrorCopies.Location = new System.Drawing.Point(6, 19);
            this.checkBoxMirrorCopies.Name = "checkBoxMirrorCopies";
            this.checkBoxMirrorCopies.Size = new System.Drawing.Size(178, 24);
            this.checkBoxMirrorCopies.TabIndex = 2;
            this.checkBoxMirrorCopies.Text = "Mirror Copies";
            this.checkBoxMirrorCopies.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxMirrorCopies.UseVisualStyleBackColor = true;
            this.checkBoxMirrorCopies.CheckedChanged += new System.EventHandler(CheckBoxMirrorCopies_CheckedChanged);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxMirrorCopies);
            this.groupBox1.Controls.Add(this.textBoxDetNumber);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 80);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Detail #";
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(18, 127);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(86, 23);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(ButtonExit_Click);
            this.buttonOk.Enabled = false;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOk.Location = new System.Drawing.Point(110, 127);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(86, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(ButtonOk_Click);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(214, 159);
            base.Controls.Add(this.buttonOk);
            base.Controls.Add(this.buttonExit);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.buttonSelectComponents);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.Location = new System.Drawing.Point(30, 130);
            base.MaximizeBox = false;
            base.Name = "MainForm";
            base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mirror Components";
            base.TopMost = true;
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainForm_FormClosing);
            base.Load += new System.EventHandler(MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }
    }




}