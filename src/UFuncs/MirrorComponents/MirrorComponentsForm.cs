using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.UF;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Geom;
using TSG_Library.Properties;
using TSG_Library.UFuncs.UFuncUtilities.MirrorUtilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using Curve = NXOpen.Curve;

namespace TSG_Library.UFuncs
{
    [UFunc("mirror-components")]
    public partial class MirrorComponentsForm : _UFuncForm
    {
        private static readonly UI TheUi = UI.GetUI();
        private static Part _originalWorkPart = _WorkPart;
        private static Part _originalDisplayPart = __display_part_;
        private static NXObject[] _selectedComponents;
        private static readonly List<Component> MirrorComponents = new List<Component>();
        private static DatumPlane mirrorPlane;
        private static string _formatCompName = string.Empty;
        private static double inputCompName;
        private static Surface.Plane _mirrorPlane;

        public MirrorComponentsForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.mirror_components_window_location;
            session_.Parts.LoadOptions.LoadLatest = false;
            session_.Parts.LoadOptions.ComponentLoadMethod = LoadOptions.LoadMethod.AsSaved;
            session_.Parts.LoadOptions.ComponentsToLoad = LoadOptions.LoadComponents.All;
            session_.Parts.LoadOptions.UseLightweightRepresentations = false;
            session_.Parts.LoadOptions.UsePartialLoading = false;
            session_.Parts.LoadOptions.SetInterpartData(true, LoadOptions.Parent.Immediate);
            session_.Parts.LoadOptions.AbortOnFailure = false;
            session_.Parts.LoadOptions.AllowSubstitution = false;
            session_.Parts.LoadOptions.GenerateMissingPartFamilyMembers = false;
            session_.Parts.LoadOptions.ReferenceSetOverride = false;

            textBoxDetNumber.Text = @"900";
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Hide();


            bool originalInterruptUpdateOnError = session_.Preferences.Modeling.InterruptUpdateOnError;

            bool originalInterruptUpdateOnWarning = session_.Preferences.Modeling.InterruptUpdateOnWarning;

            try
            {
                session_.Preferences.Modeling.InterruptUpdateOnError = false;

                session_.Preferences.Modeling.InterruptUpdateOnWarning = false;

                IDictionary<TaggedObject, TaggedObject> mirrorDict = new Dictionary<TaggedObject, TaggedObject>();

                try
                {
                    UpdateSessionParts();
                    UpdateOriginalParts();

                    if (_selectedComponents != null)
                        if (mirrorPlane != null)
                        {
                            if (checkBoxMirrorCopies.Checked)
                                NewMethod(mirrorDict);
                            else
                                foreach (NXObject selBody in _selectedComponents)
                                    if (selBody.OwningComponent != null)
                                        MirrorComponents.Add(selBody.OwningComponent);

                            if (MirrorComponents.Count > 0)
                            {
                                _UFSession.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);
                                NewMethod17();
                            }
                            else
                            {
                                NewMethod20();
                            }

                            _UFSession.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);

                            __display_part_.Views.Regenerate();

                            __display_part_ = _originalDisplayPart;

                            _WorkPart = _originalWorkPart;

                            UpdateSessionParts();

                            ResetForm();
                        }
                }
                catch (Exception ex)
                {
                    _UFSession.Disp.SetDisplay(UF_DISP_UNSUPPRESS_DISPLAY);

                    ex.__PrintException();
                }

                Component[] keys = mirrorDict.Keys.OfType<Component>().ToArray();

                foreach (Component t in keys)
                    Mirror208_3001(_mirrorPlane, t, mirrorDict);
            }
            finally
            {
                Show();

                session_.Preferences.Modeling.InterruptUpdateOnError = originalInterruptUpdateOnError;

                session_.Preferences.Modeling.InterruptUpdateOnWarning = originalInterruptUpdateOnWarning;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.mirror_components_window_location = Location;

            Settings.Default.Save();
        }

        private void ButtonSelectComponents_Click(object sender, EventArgs e)
        {
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
                if (!(compName > 0) || !(compName < 991))
                    return false;

                if ((compName > 0) & (compName < 10))
                {
                    _formatCompName = $"00{compName}";
                    return true;
                }

                if ((compName > 9) & (compName < 100))
                {
                    _formatCompName = $"0{compName}";
                    return true;
                }

                if (!(compName > 100))
                    return false;

                _formatCompName = compName.ToString();

                return true;
            }
            catch (NXException ex)
            {
                ex.__PrintException();

                return false;
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool CopyComponentToMirror(Component copyComponent,
            IDictionary<TaggedObject, TaggedObject> mirrorDict)
        {
            if (copyComponent is null)
                return false;

            Part part = (Part)copyComponent.Prototype;

            double next = inputCompName;
            string originalPath = part.FullPath;
            string copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 7)}{_formatCompName}.prt";

            while (File.Exists(copyPath))
            {
                next += 1;
                FormatDetailNumber(next);
                copyPath = $"{part.FullPath.Remove(part.FullPath.Length - 7)}{_formatCompName}.prt";
                inputCompName = next;
            }

            int indexOf = _WorkPart.FullPath.LastIndexOf("\\", StringComparison.Ordinal);
            string compare1 = _WorkPart.FullPath.Remove(indexOf + 1);
            string compare2 = copyPath.Remove(indexOf + 1);

            if (compare1 != compare2)
            {
                TheUi.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error,
                    "The selected component is not from the work part directory. \n                   Save As before making a copy.");

                return false;
            }

            if (!File.Exists(originalPath))
            {
                TheUi.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error,
                    "Save the original file to the assembly before making a copy");

                return false;
            }

            File.Copy(originalPath, copyPath);

            BasePart basePart1 = session_.Parts.OpenBase(copyPath, out PartLoadStatus partLoadStatus1);
            partLoadStatus1.Dispose();

            Part partToAdd = (Part)basePart1;

            copyComponent.GetPosition(out Point3d origin, out Matrix3x3 orientation);

            int layer = copyComponent.Layer;

            Component compToAdd = _WorkPart.ComponentAssembly.AddComponent(partToAdd, "BODY", _formatCompName, origin,
                orientation, layer,
                out PartLoadStatus partLoadStatus2);

            partLoadStatus2.Dispose();

            mirrorDict.Add(copyComponent, compToAdd);

            Feature[] oFeatures = copyComponent.__Prototype().Features.ToArray();

            Feature[] nFeatures = compToAdd.__Prototype().Features.ToArray();

            for (int i = 0; i < oFeatures.Length; i++)
                mirrorDict.Add(oFeatures[i], nFeatures[i]);

            Tag cycleRefSet = NXOpen.Tag.Null;

            do
            {
                _UFSession.Obj.CycleObjsInPart(_WorkPart.Tag, UF_reference_set_type, ref cycleRefSet);

                if (cycleRefSet == NXOpen.Tag.Null)
                    break;

                _UFSession.Obj.AskName(cycleRefSet, out string name);

                if (name != "BODY")
                    continue;

                Tag[] refSetMembers = { compToAdd.Tag };

                _UFSession.Assem.AddRefSetMembers(cycleRefSet, refSetMembers.Length, refSetMembers);

                _UFSession.Assem.ReplaceRefset(refSetMembers.Length, refSetMembers, "BODY");
            }
            while (cycleRefSet != NXOpen.Tag.Null);

            MirrorComponents.Add(compToAdd);

            return true;
        }

        private static void UpdateSessionParts()
        {
        }

        private static void UpdateOriginalParts()
        {
            _originalWorkPart = _WorkPart;
            _originalDisplayPart = __display_part_;
        }

        private void ResetForm()
        {
            Array.Clear(_selectedComponents, 0, _selectedComponents.Length);
            MirrorComponents.Clear();
            mirrorPlane = null;
            _formatCompName = string.Empty;
            inputCompName = 0;

            checkBoxMirrorCopies.Checked = false;
            textBoxDetNumber.Text = string.Empty;
            buttonOk.Enabled = false;
            textBoxDetNumber.Text = "900";
        }

        private void CheckBoxMirrorCopies_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDetNumber.Enabled = checkBoxMirrorCopies.Checked;
        }

        private void NewMethod20()
        {
            // ReSharper disable once UnusedVariable
            _ = session_.SetUndoMark(Session.MarkVisibility.Visible, "Mirror Components");

            UpdateSessionParts();

            if (!_WorkPart.__HasDynamicBlock())
            {
                NewMethod28();
                return;
            }

            SetWcsToWorkPart(null);

            double[] planeOrigin = { mirrorPlane.Origin.X, mirrorPlane.Origin.Y, mirrorPlane.Origin.Z };
            double[] planeNormal = { mirrorPlane.Normal.X, mirrorPlane.Normal.Y, mirrorPlane.Normal.Z };

            _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out Tag selectedPlane);

            if (mirrorPlane != null)
                // ReSharper disable once RedundantAssignment
                _ = NewMethod27(selectedPlane);
        }

        private void NewMethod17()
        {
            foreach (Component mirrorComp in MirrorComponents)
            {
                Part makeWork = (Part)mirrorComp.Prototype;

                BasePart.Units wpUnits = makeWork.PartUnits;

                __display_part_ = _originalDisplayPart;

                _WorkPart = _originalWorkPart;

                UpdateSessionParts();

                if (__display_part_.PartUnits != wpUnits)
                    continue;

                _WorkPart = makeWork;

                UpdateSessionParts();

                if (!mirrorComp.__Origin().__IsEqualTo(_Point3dOrigin))
                {
                    NewMethod26(mirrorComp);
                    continue;
                }

                __display_part_ = _WorkPart;

                UpdateSessionParts();

                bool isBlockComp = _WorkPart.__HasDynamicBlock();

                NewMethod25(mirrorComp, isBlockComp);
            }
        }

        private void NewMethod28()
        {
            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            double[] planeOrigin = { mirrorPlane.Origin.X, mirrorPlane.Origin.Y, mirrorPlane.Origin.Z };
            double[] planeNormal = { mirrorPlane.Normal.X, mirrorPlane.Normal.Y, mirrorPlane.Normal.Z };
            _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out Tag selectedPlane);
            // ReSharper disable once RedundantAssignment
            _ = NewMethod12(selectedPlane);
        }

        private Tag NewMethod27(Tag selectedPlane)
        {
            double[] mirrorMatrix = new double[12];

            // ReSharper disable once UnusedVariable
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out _);

            CartesianCoordinateSystem csysToMirror = __display_part_.CoordinateSystems.CreateCoordinateSystem(
                __display_part_.WCS.Origin,
                __display_part_.WCS.CoordinateSystem.Orientation.Element, true);

            List<Tag> mirrorMove = new List<Tag>
            {
                csysToMirror.Tag
            };

            TransformObjects(mirrorMove, mirrorMatrix);

            __display_part_.WCS.SetOriginAndMatrix(csysToMirror.Origin, csysToMirror.Orientation.Element);

            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180);

            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90);

            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);

            NewMethod2(selectedPlane);

            mirrorMove.Clear();

            NewMethod6(mirrorMatrix, mirrorMove);

            mirrorMove.Clear();

            mirrorMove.AddRange(GetCurves());

            NewMethod22(mirrorMove);

            TransformObjects(mirrorMove, mirrorMatrix);

            mirrorMove.Clear();

            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);

            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);

            return selectedPlane;
        }

        private void NewMethod26(Component mirrorComp)
        {
            Feature waveMirrorFeature = WaveLinkDatumPlane(mirrorPlane);

            _UFSession.Disp.SetDisplay(UF_DISP_SUPPRESS_DISPLAY);

            mirrorComp.Unhighlight();

            __display_part_ = (Part)mirrorComp.Prototype;

            UpdateSessionParts();

            bool isBlockComp = _WorkPart.__HasDynamicBlock();

            if (isBlockComp)
            {
                SetWcsToWorkPart(mirrorComp);

                NewMethod14(waveMirrorFeature);

                return;
            }

            NewMethod15(waveMirrorFeature);
        }

        private void NewMethod25(Component mirrorComp, bool isBlockComp)
        {
            double[] planeOrigin = mirrorPlane.Origin.__Array();
            double[] planeNormal = mirrorPlane.Normal.__ToArray();
            Tag selectedPlane;

            if (isBlockComp)
            {
                SetWcsToWorkPart(mirrorComp);
                _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out selectedPlane);
                NewMethod9(selectedPlane);
                return;
            }

            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out selectedPlane);

            if (mirrorPlane != null)
                NewMethod23(selectedPlane);
        }

        private Tag NewMethod23(Tag selectedPlane)
        {
            double[] mirrorMatrix = new double[12];
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out int _);
            session_.__DeleteObjects(selectedPlane);
            _UFSession.Modl.Update();
            List<Tag> mirrorMove = NewMethod11();
            TransformObjects(mirrorMove, mirrorMatrix);
            _UFSession.Modl.Update();
            mirrorMove.Clear();
            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);
            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);
            mirrorMove.Clear();
            return selectedPlane;
        }

        private void NewMethod15(Feature waveMirrorFeature)
        {
            if (waveMirrorFeature is null)
                return;

            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            _UFSession.Modl.AskFeatObject(waveMirrorFeature.Tag, out _, out Tag[] featObjects);
            DatumPlane waveMirrorPlane = (DatumPlane)NXObjectManager.Get(featObjects[0]);
            double[] planeOrigin = { waveMirrorPlane.Origin.X, waveMirrorPlane.Origin.Y, waveMirrorPlane.Origin.Z };
            double[] planeNormal = { waveMirrorPlane.Normal.X, waveMirrorPlane.Normal.Y, waveMirrorPlane.Normal.Z };
            _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out Tag selectedPlane);
            double[] mirrorMatrix = new double[12];
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out _);
            session_.__DeleteObjects(waveMirrorPlane.Tag, waveMirrorFeature.Tag, selectedPlane);
            _UFSession.Modl.Update();
            List<Tag> mirrorMove = new List<Tag>(GetCurves());
            NewMethod29(featObjects, mirrorMove);
            TransformObjects(mirrorMove, mirrorMatrix);
            _UFSession.Modl.Update();
            mirrorMove.Clear();
            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);
            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);
            mirrorMove.Clear();
        }

        private void NewMethod14(Feature waveMirrorFeature)
        {
            if (waveMirrorFeature is null)
                return;

            _UFSession.Modl.AskFeatObject(waveMirrorFeature.Tag, out int _, out Tag[] featObjects);
            DatumPlane waveMirrorPlane = (DatumPlane)NXObjectManager.Get(featObjects[0]);
            double[] planeOrigin = { waveMirrorPlane.Origin.X, waveMirrorPlane.Origin.Y, waveMirrorPlane.Origin.Z };
            double[] planeNormal = { waveMirrorPlane.Normal.X, waveMirrorPlane.Normal.Y, waveMirrorPlane.Normal.Z };
            _UFSession.Modl.CreatePlane(planeOrigin, planeNormal, out Tag selectedPlane);
            double[] mirrorMatrix = new double[12];
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out int _);
            CartesianCoordinateSystem csysToMirror = __display_part_.CoordinateSystems.CreateCoordinateSystem(
                __display_part_.WCS.Origin,
                __display_part_.WCS.CoordinateSystem.Orientation.Element, true);
            List<Tag> mirrorMove = new List<Tag>
            {
                csysToMirror.Tag
            };
            TransformObjects(mirrorMove, mirrorMatrix);
            __display_part_.WCS.SetOriginAndMatrix(csysToMirror.Origin, csysToMirror.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90);
            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            NewMethod4(waveMirrorFeature, waveMirrorPlane, selectedPlane);
            mirrorMove.Clear();
            NewMethod6(mirrorMatrix, mirrorMove);
            mirrorMove.Clear();
            mirrorMove.AddRange(GetCurves());
            NewMethod30(featObjects, mirrorMove);
            TransformObjects(mirrorMove, mirrorMatrix);
            mirrorMove.Clear();
            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);
            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);
        }

        private static void NewMethod29(Tag[] featObjects, List<Tag> mirrorMove)
        {
            foreach (Feature mirrorFeat in _WorkPart.Features)
            {
                if (mirrorFeat.FeatureType != "ABSOLUTE_DATUM_PLANE")
                    continue;

                _UFSession.Modl.AskFeatObject(mirrorFeat.Tag, out int _, out Tag[] _);
                TaggedObject taggedObject = NXObjectManager.Get(featObjects[0]);
                NXObject nxOb = (NXObject)taggedObject;
                DatumPlane mirrorDatum = (DatumPlane)NXObjectManager.Get(featObjects[0]);
                if (mirrorDatum.Layer > 0 && mirrorDatum.Layer < 21)
                    mirrorMove.Add(mirrorDatum.Tag);
            }
        }

        private Tag NewMethod12(Tag selectedPlane)
        {
            if (mirrorPlane is null)
                return selectedPlane;

            double[] mirrorMatrix = new double[12];
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out _);
            session_.__DeleteObjects(selectedPlane);
            _UFSession.Modl.Update();
            List<Tag> mirrorMove = new List<Tag>(GetCurves());
            NewMethod22(mirrorMove);
            TransformObjects(mirrorMove, mirrorMatrix);
            _UFSession.Modl.Update();
            mirrorMove.Clear();
            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);
            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);
            mirrorMove.Clear();
            return selectedPlane;
        }

        private static List<Tag> NewMethod11()
        {
            List<Tag> mirrorMove = new List<Tag>(GetCurves());
            NewMethod22(mirrorMove);
            return mirrorMove;
        }

        private Tag NewMethod9(Tag selectedPlane)
        {
            if (mirrorPlane is null)
                return selectedPlane;
            double[] mirrorMatrix = new double[12];
            _UFSession.Trns.CreateReflectionMatrix(ref selectedPlane, mirrorMatrix, out _);
            CartesianCoordinateSystem csysToMirror = __display_part_.CoordinateSystems.CreateCoordinateSystem(
                __display_part_.WCS.Origin,
                __display_part_.WCS.CoordinateSystem.Orientation.Element, true);
            List<Tag> mirrorMove = new List<Tag>
            {
                csysToMirror.Tag
            };
            TransformObjects(mirrorMove, mirrorMatrix);
            __display_part_.WCS.SetOriginAndMatrix(csysToMirror.Origin, csysToMirror.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90);
            _UFSession.Modl.SetUpdateFailOption(UFModl.UpdateOption.UpdateAcceptAll);
            NewMethod2(selectedPlane);
            mirrorMove.Clear();
            NewMethod3(mirrorMatrix, mirrorMove);
            mirrorMove.Clear();
            mirrorMove.AddRange(GetCurves());
            NewMethod22(mirrorMove);
            TransformObjects(mirrorMove, mirrorMatrix);
            mirrorMove.Clear();
            NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
                out List<Matrix3x3> fastenerMatrix);
            NewMethod10(mirrorMatrix, mirrorMove, fasteners, fastenerOrigins, fastenerMatrix);
            return selectedPlane;
        }

        private void NewMethod10(
            double[] mirrorMatrix,
            List<Tag> mirrorMove,
            List<Component> fasteners,
            List<Point3d> fastenerOrigins,
            List<Matrix3x3> fastenerMatrix)
        {
            for (int i = 0; i < fasteners.Count; i++)
            {
                CartesianCoordinateSystem transformCsys;
                transformCsys = __display_part_.CoordinateSystems.CreateCoordinateSystem(fastenerOrigins[i],
                    fastenerMatrix[i], true);
                mirrorMove.Add(transformCsys.Tag);
                TransformObjects(mirrorMove, mirrorMatrix);
                __display_part_.WCS.SetOriginAndMatrix(transformCsys.Origin, transformCsys.Orientation.Element);
                __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180);
                __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90);
                _ = __display_part_.WCS.Origin;
                _ = __display_part_.WCS.CoordinateSystem.Orientation.Element;
                double[] moveMatrix =
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
                double[] moveOrigin =
                    { __display_part_.WCS.Origin.X, __display_part_.WCS.Origin.Y, __display_part_.WCS.Origin.Z };
                Tag fastenerInstance = _UFSession.Assem.AskInstOfPartOcc(fasteners[i].Tag);
                _UFSession.Assem.RepositionInstance(fastenerInstance, moveOrigin, moveMatrix);
                mirrorMove.Clear();
            }
        }

        private void NewMethod6(double[] mirrorMatrix, List<Tag> mirrorMove)
        {
            foreach (Feature featDynamic in _WorkPart.Features)
            {
                if (featDynamic.FeatureType != "BLOCK")
                    continue;

                if (featDynamic.Name != "FEATURE BLOCK")
                    continue;

                BlockFeatureBuilder builder = _WorkPart.Features.CreateBlockFeatureBuilder(featDynamic);

                using (new Destroyer(builder))
                {
                    NewMethod8(mirrorMatrix, mirrorMove, builder);
                }

                _UFSession.Modl.Update();
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void NewMethod4(Feature waveMirrorFeature, DatumPlane waveMirrorPlane, Tag selectedPlane)
        {
            foreach (Feature featDynamic in _WorkPart.Features)
            {
                if (featDynamic.FeatureType != "BLOCK")
                    continue;

                if (featDynamic.Name != "DYNAMIC BLOCK")
                    continue;

                BlockFeatureBuilder builder = _WorkPart.Features.CreateBlockFeatureBuilder(featDynamic);

                using (new Destroyer(builder))
                {
                    NewMethod1(builder);
                }

                session_.__DeleteObjects(waveMirrorFeature.Tag, waveMirrorPlane.Tag, selectedPlane);

                _UFSession.Modl.Update();
            }
        }

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void NewMethod2(Tag selectedPlane)
        {
            foreach (Feature featDynamic in _WorkPart.Features)
            {
                if (featDynamic.FeatureType != "BLOCK")
                    continue;

                if (featDynamic.Name != "DYNAMIC BLOCK")
                    continue;

                BlockFeatureBuilder builder = _WorkPart.Features.CreateBlockFeatureBuilder(featDynamic);

                using (new Destroyer(builder))
                {
                    NewMethod1(builder);
                }

                //DeleteObjects((NXOpen.NXObject)Snap.NX.NXObject.Wrap(selectedPlane));

                _UFSession.Modl.Update();
            }
        }

        private void NewMethod3(double[] mirrorMatrix, List<Tag> mirrorMove)
        {
            foreach (Feature featDynamic in _WorkPart.Features)
            {
                if (featDynamic.FeatureType != "BLOCK")
                    continue;

                if (featDynamic.Name != "FEATURE BLOCK")
                    continue;

                Block featureBlock = (Block)featDynamic;

                BlockFeatureBuilder blockFeatureBuilder2 = _WorkPart.Features.CreateBlockFeatureBuilder(featureBlock);

                using (new Destroyer(blockFeatureBuilder2))
                {
                    NewMethod8(mirrorMatrix, mirrorMove, blockFeatureBuilder2);
                }

                _WorkPart.FacetedBodies.DeleteTemporaryFacesAndEdges();

                _UFSession.Modl.Update();
            }
        }

        private void NewMethod(IDictionary<TaggedObject, TaggedObject> mirrorDict)
        {
            // ReSharper disable once RedundantAssignment
            _ = double.TryParse(textBoxDetNumber.Text, out inputCompName);

            bool isCompName = FormatDetailNumber(inputCompName);

            if (!isCompName)
                return;

            for (int i = 0; i < _selectedComponents.Length; i++)
            {
                prompt_($"Mirroring: ({i + 1} of {_selectedComponents.Length})");

                NXObject selBody = _selectedComponents[i];

                if (selBody.OwningComponent != null)
                    CopyComponentToMirror(selBody.OwningComponent, mirrorDict);
            }
        }

        private static void NewMethod1(BlockFeatureBuilder blockFeatureBuilder1)
        {
            string length = blockFeatureBuilder1.Width.RightHandSide;
            string width = blockFeatureBuilder1.Length.RightHandSide;
            string height = blockFeatureBuilder1.Height.RightHandSide;
            Point3d blkOrigin = __display_part_.WCS.Origin;
            Matrix3x3 blkOrientation = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            Vector3d xAxis = blkOrientation.__AxisX();
            Vector3d yAxis = blkOrientation.__AxisY();
            Point blkFeatBuilderPoint = _WorkPart.Points.CreatePoint(blkOrigin);
            blkFeatBuilderPoint.SetCoordinates(blkOrigin);
            blockFeatureBuilder1.OriginPoint = blkFeatBuilderPoint;
            blockFeatureBuilder1.SetOriginAndLengths(blkOrigin, length, width, height);
            blockFeatureBuilder1.SetOrientation(xAxis, yAxis);
            blockFeatureBuilder1.CommitFeature();
        }

        private void NewMethod8(double[] mirrorMatrix, List<Tag> mirrorMove, BlockFeatureBuilder blockFeatureBuilder2)
        {
            blockFeatureBuilder2.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);
            double[] initOrigin = blockFeatureBuilder2.Origin.__Array();
            double[] xVector = xAxis.__ToArray();
            double[] yVector = yAxis.__ToArray();
            double[] initMatrix = new double[9];
            _UFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
            _UFSession.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
            _UFSession.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
            CartesianCoordinateSystem transformCsys1 = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);
            mirrorMove.Add(transformCsys1.Tag);
            TransformObjects(mirrorMove, mirrorMatrix);
            __display_part_.WCS.SetOriginAndMatrix(transformCsys1.Origin, transformCsys1.Orientation.Element);
            __display_part_.WCS.Rotate(WCS.Axis.YAxis, 180);
            __display_part_.WCS.Rotate(WCS.Axis.ZAxis, 90);
            Point3d origin = __display_part_.WCS.Origin;
            Matrix3x3 orientation = __display_part_.WCS.CoordinateSystem.Orientation.Element;
            CartesianCoordinateSystem mirrorCsys1 =
                __display_part_.CoordinateSystems.CreateCoordinateSystem(origin, orientation, true);
            string length = blockFeatureBuilder2.Width.RightHandSide;
            string width = blockFeatureBuilder2.Length.RightHandSide;
            string height = blockFeatureBuilder2.Height.RightHandSide;
            Point3d blkOrigin = mirrorCsys1.Origin;
            Matrix3x3 blkOrientation = mirrorCsys1.Orientation.Element;
            Vector3d xAxis1 = blkOrientation.__AxisX();
            Vector3d yAxis1 = blkOrientation.__AxisY();
            Point blkFeatBuilderPoint = _WorkPart.Points.CreatePoint(blkOrigin);
            blkFeatBuilderPoint.SetCoordinates(blkOrigin);
            blockFeatureBuilder2.OriginPoint = blkFeatBuilderPoint;
            blockFeatureBuilder2.SetOriginAndLengths(blkOrigin, length, width, height);
            blockFeatureBuilder2.SetOrientation(xAxis1, yAxis1);
            blockFeatureBuilder2.CommitFeature();
        }

        private static Feature WaveLinkDatumPlane(DatumPlane selectedDatum)
        {
            WaveLinkBuilder builder = _WorkPart.BaseFeatures.CreateWaveLinkBuilder(null);

            using (new Destroyer(builder))
            {
                builder.Type = WaveLinkBuilder.Types.DatumLink;
                builder.WaveDatumBuilder.DisplayScale = 2.0;
                builder.WaveDatumBuilder.Associative = true;
                builder.WaveDatumBuilder.HideOriginal = false;
                builder.WaveDatumBuilder.InheritDisplayProperties = false;
                builder.WaveDatumBuilder.Datums.Add(selectedDatum);
                return builder.CommitFeature();
            }
        }

        private static void NewMethod22(List<Tag> mirrorMove)
        {
            foreach (Feature mirrorFeat in _WorkPart.Features)
            {
                if (mirrorFeat.FeatureType != "ABSOLUTE_DATUM_PLANE")
                    continue;

                _UFSession.Modl.AskFeatObject(mirrorFeat.Tag, out int _, out Tag[] featObjects);
                DatumPlane mirrorDatum = (DatumPlane)NXObjectManager.Get(featObjects[0]);

                if (mirrorDatum.Layer > 0 && mirrorDatum.Layer < 21)
                    mirrorMove.Add(mirrorDatum.Tag);
            }
        }

        private static void NewMethod33(out List<Component> fasteners, out List<Point3d> fastenerOrigins,
            out List<Matrix3x3> fastenerMatrix)
        {
            fasteners = new List<Component>();
            fastenerOrigins = new List<Point3d>();
            fastenerMatrix = new List<Matrix3x3>();

            // Get all fasteners in the mirror component and their locations
            if (_WorkPart.ComponentAssembly.RootComponent is null)
                return;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Component getChild in _WorkPart.ComponentAssembly.RootComponent.GetChildren())
                fasteners.Add(getChild);

            foreach (Component child in fasteners)
            {
                child.GetPosition(out Point3d point, out Matrix3x3 matrix);
                fastenerOrigins.Add(point);
                fastenerMatrix.Add(matrix);
            }
        }

        public static IEnumerable<Tag> GetCurves()
        {
            foreach (Curve curve in _WorkPart.Curves)
                if (curve.Layer > 0 && curve.Layer < 21)
                    yield return curve.Tag;
        }

        private static void
            TransformObjects(List<Tag> objs,
                double[] reflectionMatrix) //, IDictionary<NXOpen.TaggedObject, NXOpen.TaggedObject> mirrorDict)
        {
            int numOfMoveObj = objs.Count;

            int move = 1;

            int destinationLayerMove = 0;

            int traceMove = 2;

            Tag[] moved = new Tag[objs.Count];

            _UFSession.Trns.TransformObjects(
                reflectionMatrix,
                objs.ToArray(),
                ref numOfMoveObj,
                ref move,
                ref destinationLayerMove,
                ref traceMove,
                moved,
                out Tag _,
                out int _);

            _UFSession.Modl.Update();
        }

        public static void DeleteObjects(params NXObject[] deleteObjects)
        {
            Session.UndoMarkId markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");

            foreach (NXObject delObj in deleteObjects)
                session_.UpdateManager.AddToDeleteList(delObj);

            session_.UpdateManager.DoUpdate(markDeleteObjs);
            session_.DeleteUndoMark(markDeleteObjs, "");
        }

        public static void SetWcsToWorkPart(Component compRefCsys)
        {
            foreach (Feature featBlk in _WorkPart.Features)
            {
                if (featBlk.FeatureType != "BLOCK")
                    continue;

                if (featBlk.Name != "DYNAMIC BLOCK")
                    continue;

                Block block1 = (Block)featBlk;
                BlockFeatureBuilder blockFeatureBuilderMatch = _WorkPart.Features.CreateBlockFeatureBuilder(block1);

                using (new Destroyer(blockFeatureBuilderMatch))
                {
                    Point3d bOrigin = blockFeatureBuilderMatch.Origin;
                    blockFeatureBuilderMatch.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);
                    double[] initOrigin = { bOrigin.X, bOrigin.Y, bOrigin.Z };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    double[] initMatrix = new double[9];
                    _UFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                    _UFSession.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                    _UFSession.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);
                    CartesianCoordinateSystem setTempCsys = (CartesianCoordinateSystem)NXObjectManager.Get(tempCsys);
                    __display_part_.WCS.SetOriginAndMatrix(setTempCsys.Origin, setTempCsys.Orientation.Element);
                }
            }
        }

        public static DatumPlane SelectMirrorDatumPlane()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_datum_plane_type, 0, 0);

            Selection.Response sel = uisession_.SelectionManager.SelectTaggedObject(
                "Select Datum Plane",
                "Select Datum Plane",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                mask,
                out TaggedObject selectedDatum,
                out Point3d _);

            if (sel == Selection.Response.ObjectSelected || sel == Selection.Response.ObjectSelectedByName)
                return (DatumPlane)selectedDatum;

            return null;
        }

        public static TaggedObject[] SelectMultipleComponentBodies()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_solid_type, UF_solid_body_subtype, 0);

            Selection.Response sel = uisession_.SelectionManager.SelectTaggedObjects(
                "Select Component",
                "Select Component",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                mask,
                out TaggedObject[] selectedBody);

            if (sel == Selection.Response.Ok || sel == Selection.Response.ObjectSelected ||
                sel == Selection.Response.ObjectSelectedByName)
                return selectedBody;

            return null;
        }

        public static void NewMethod30(Tag[] featObjects, List<Tag> mirrorMove)
        {
            foreach (Feature mirrorFeat in _WorkPart.Features)
            {
                if (mirrorFeat.FeatureType != "ABSOLUTE_DATUM_PLANE")
                    continue;

                _UFSession.Modl.AskFeatObject(mirrorFeat.Tag, out int _, out Tag[] _);

                // ReSharper disable once UnusedVariable
                _ = NXObjectManager.Get(featObjects[0]);

                //Blows up below when datum plane is present in component
                DatumPlane mirrorDatum = (DatumPlane)NXObjectManager.Get(featObjects[0]);

                if (mirrorDatum.Layer > 0 && mirrorDatum.Layer < 21)
                    mirrorMove.Add(mirrorDatum.Tag);
            }
        }

        public static void Mirror208_3001(
            Surface.Plane plane,
            Component originalComp,
            IDictionary<TaggedObject, TaggedObject> dict)
        {
            Component mirroredComp = (Component)dict[originalComp];

            try
            {
                IMirrorFeature[] featureMirrors =
                {
                    new MirrorLinkedBody(),
                    new MirrorEdgeBlend(),
                    new MirrorChamfer(),
                    new MirrorSubtract(),
                    new MirrorIntersect(),
                    new MirrorUnite(),
                    new MirrorBlock(),
                    new MirrorExtractedBody(),
                    new MirrorExtrude(),
                    new MirrorOffset()
                };

                Debugger.Launch();

                Part originalPart = originalComp.__Prototype();

                Part mirroredPart = mirroredComp.__Prototype();

                originalComp.__Prototype().Features.SuppressFeatures(originalComp.__Prototype().Features.GetFeatures());

                mirroredComp.__Prototype().Features.SuppressFeatures(mirroredComp.__Prototype().Features.GetFeatures());

                Body[] originalBodies = originalPart.Bodies.ToArray();

                Body[] mirroredBodies = mirroredPart.Bodies.ToArray();

                for (int i = 0; i < originalBodies.Length; i++)
                    dict.Add(originalBodies[i], mirroredBodies[i]);

                foreach (Feature originalFeature in originalPart.Features)
                    try
                    {
                        IMirrorFeature mirrorOp = featureMirrors.SingleOrDefault(mirror =>
                            mirror.FeatureType == originalFeature.FeatureType);

                        if (!(mirrorOp is null))
                        {
                            mirrorOp.Mirror(originalFeature, dict, plane, originalComp);

                            continue;
                        }

                        print_("///////////////////////////");

                        print_("Unable to mirror:");

                        print_($"Type: {originalFeature.FeatureType}");

                        print_($"Name: {originalFeature.GetFeatureName()}");

                        print_($"Part: {originalFeature.OwningPart.Leaf}");

                        print_("///////////////////////////");
                    }
                    catch (MirrorException mirrorException)
                    {
                        print_("////////////////////////////////////////////////////");
                        print_(originalPart.Leaf);
                        print_($"Unable to mirror {originalFeature.GetFeatureName()}");
                        print_(mirrorException.Message);
                        print_("////////////////////////////////////////////////////");
                    }

                _WorkPart = __display_part_;

                foreach (Part descendant in __display_part_.__DescendantParts())
                foreach (Expression exp in descendant.Expressions)
                {
                    if (exp.Status != Expression.StatusOption.Broken)
                        continue;

                    Expression[] otherExpressions = exp.__OwningPart().Expressions.ToArray();

                    foreach (Expression t in otherExpressions)
                    {
                        if (t.Tag == exp.Tag)
                            continue;

                        if (exp.Name == t.RightHandSide)
                            descendant.Expressions.Delete(t);
                    }

                    descendant.Expressions.Delete(exp);

                    print_($"Deleted broken expression \"{exp.Name}\" in part \"{exp.__OwningPart().Leaf}\"");
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
            finally
            {
                //UnSuppressDisplay();

                Session.UndoMarkId markId1 = session_.SetUndoMark(Session.MarkVisibility.Visible, "Make Work Part");

                session_.UpdateManager.DoInterpartUpdate(markId1);

                session_.UpdateManager.DoAssemblyConstraintsUpdate(markId1);
            }
        }
    }
}