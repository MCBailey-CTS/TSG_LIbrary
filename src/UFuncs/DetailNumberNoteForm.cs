using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricAnalysis;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Extensions;
using TSG_Library.Properties;
using static NXOpen.Selection;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.Extensions_;
using static TSG_Library.UFuncs._UFunc;
using Assembly = System.Reflection.Assembly;
using Selection = TSG_Library.Ui.Selection;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_detail_number_note)]
    public partial class DetailNumberNoteForm : _UFuncForm
    {
        private readonly int COLOR;

        private readonly string FONT;

        private readonly double HEIGHT;

        private readonly int LAYER;

        private readonly double LENGTH;

        private readonly string REFERENCE_SET;

        private readonly TextBuilder.ScriptOptions SCRIPT;

        public DetailNumberNoteForm()
        {
            InitializeComponent();

            Text = @"Detail Number Note";

            var location = Assembly.GetExecutingAssembly().Location;

            var directory = Path.GetDirectoryName(location);

            //string[] lines = System.IO.File.ReadAllLines($"{directory}\\Settings.txt");

            COLOR = 187; // ReadInt(nameof(COLOR), lines);

            REFERENCE_SET = "DETAIL-NUMBERS"; // ReadString(nameof(REFERENCE_SET), lines);

            LAYER = 1; // ReadInt(nameof(LAYER), lines);

            FONT = "Myriad CAD"; // ReadString(nameof(FONT), lines);

            LENGTH = 0.900d; // ReadDouble(nameof(LENGTH), lines);

            HEIGHT = 0.400d; // ReadDouble(nameof(HEIGHT), lines);

            SCRIPT = TextBuilder.ScriptOptions.Western; // ReadScriptOptions(nameof(SCRIPT), lines);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.detail_number_note_form_window_location = Location;
            Settings.Default.Save();
        }

        public static Face[] _SimpleInterference(Part part, Body targetBody, Body toolBody)
        {
            var obj = part.AnalysisManager.CreateSimpleInterferenceObject();

            using (new Destroyer(obj))
            {
                obj.FaceInterferenceType = SimpleInterference.FaceInterferenceMethod.AllPairs;
                obj.InterferenceType = SimpleInterference.InterferenceMethod.InterferingFaces;
                obj.FirstBody.Value = targetBody;
                obj.SecondBody.Value = toolBody;
                obj.PerformCheck();
                var result = obj.GetInterferenceResults();
                var faces = new Face[result.Length];

                for (var i = 0; i < result.Length; i += 2)
                {
                    faces[i] = (Face)result[i];
                    faces[i + 1] = (Face)result[i + 1];
                }

                obj.Reset();
                return faces;
            }
        }

        [Obsolete]
        private void BtnWcs_Click(object sender, EventArgs e)
        {
            try
            {
                //Snap.UI.Selection.Dialog selection = Snap.UI.Selection.SelectObject("Select Face");
                //selection.AllowMultiple = false;
                //selection.Title = "Select Face";
                //selection.Scope = Snap.UI.Selection.Dialog.SelectionScope.AnyInAssembly;
                //selection.KeepHighlighted = false;
                //selection.IncludeFeatures = false;
                //selection.SetFilter(Snap.NX.ObjectTypes.Type.Face, Snap.NX.ObjectTypes.SubType.FacePlane);

                //Snap.UI.Selection.Result result = selection.Show();

                //if ((int)result.Response != 5)
                //    return;

                //if (result.Objects.Length < 1)
                //    return;

                //NXOpen.Face selectedFace = (NXOpen.Face)result.Object.NXOpenTaggedObject;

                //NXOpen.Vector3d normal = selectedFace._NormalVector();

                //NXOpen.Matrix3x3 orientation = normal._ToMatrix3x3();

                //NXOpen.Point3d origin = selectedFace.GetEdges()[0].__StartPoint();

                //__display_part_.WCS.SetOriginAndMatrix(origin, orientation);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                Main1(WcsOrientation._AxisZ()._Unit());
            }
            catch (Exception ex)
            {
                ex._PrintException();
            }
        }

        private bool TryCreateNote(string detail, Vector3d expectedTargetVector, Face[] interferingFaces)
        {
            for (var i = 0; i < interferingFaces.Length - 1; i += 2)
            {
                var targetFace = interferingFaces[i];
                var toolFace = interferingFaces[i + 1];

                if (!targetFace._IsPlanar())
                    continue;

                if (!toolFace._IsPlanar())
                    continue;

                var targetVector = targetFace._NormalVector()._Unit();
                var toolVector = toolFace._NormalVector()._Unit();

                if (!targetVector._IsEqualTo(expectedTargetVector))
                    continue;

                if (!toolVector._IsEqualTo(expectedTargetVector._Negate()))
                    continue;

                CreateNote0(detail, targetFace, toolFace);
                return true;
            }

            return false;
        }

        internal void Main1(Vector3d expectedTargetVector)
        {
            using (new DisplayPartReset())
            {
                var targetFace = SelectPlanarFace();

                // for now the target face must be an occurrence
                if (!targetFace.IsOccurrence)
                {
                    print_("Please select an occurrence face");
                    return;
                }

                var target_face_normal = targetFace._NormalVector();

                var selectedToolComponents = SelectToolComponents();

                foreach (var tool in selectedToolComponents)
                {
                    var solid_body_layer_1_proto = tool._Members()
                        .OfType<Body>()
                        .Single(__b => ((Body)__b.Prototype).Layer == 1);

                    var interferingFaces =
                        _SimpleInterference(_WorkPart, targetFace.GetBody(), solid_body_layer_1_proto);

                    var detail = GetDetailName(interferingFaces[1].OwningComponent);

                    if (TryCreateNote(detail, expectedTargetVector, interferingFaces))
                    {
                        solid_body_layer_1_proto.OwningComponent.Blank();

                        print_($"Created Note: {detail}");
                    }
                }
            }
        }

        private Component[] SelectToolComponents()
        {
            return Selection.SelectManyComponents();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.detail_number_note_form_window_location;
        }


        private static Body[] SelectSolidBodies()
        {
            var mask = new MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

            TheUISession.SelectionManager.SelectTaggedObjects(
                "Please select tool bodies",
                "Selection",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask },
                out var selectedObjects);

            return selectedObjects.Cast<Body>().ToArray();
        }

        private static Body SelectSolidBody()
        {
            var mask = new MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

            TheUISession.SelectionManager.SelectTaggedObject(
                "Please select target body",
                "Please select target body",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask }, out var selectedObjects,
                out var _);

            return (Body)selectedObjects;
        }

        private static Face SelectPlanarFace()
        {
            var mask = new MaskTriple(UF_face_type, 0, UF_UI_SEL_FEATURE_PLANAR_FACE);

            TheUISession.SelectionManager.SelectTaggedObject(
                "Please select target body",
                "Please select target body",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask }, out var selectedObjects,
                out var _);

            return (Face)selectedObjects;
        }

        private static string GetDetailName(Component component)
        {
            var displayName = component.DisplayName;

            var length = displayName.Length;

            return displayName.Substring(length - 3);
        }

        private static void ModifyDisplay(DisplayableObject[] displayableObjects, int color, int layer)
        {
            var dispMod = session_.DisplayManager.NewDisplayModification();

            using (dispMod)
            {
                dispMod.ApplyToAllFaces = true;

                dispMod.ApplyToOwningParts = false;

                dispMod.NewColor = color;

                dispMod.NewLayer = layer;

                dispMod.Apply(displayableObjects);
            }
        }

        private static Matrix3x3 CreateOrientationFromZVector(Vector3d vector)
        {
            double[] vec = { vector.X, vector.Y, vector.Z };
            var matrix = new double[9];
            TheUFSession.Mtx3.InitializeZ(vec, matrix);
            var xVec = new Vector3d(matrix[0], matrix[1], matrix[2]);
            var yVec = new Vector3d(matrix[3], matrix[4], matrix[5]);
            return xVec._ToMatrix3x3(yVec);
        }

        private static double[] SumEdgePositions(IEnumerable<Point3d> edgePositions)
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            foreach (var pos in edgePositions)
            {
                x += pos.X;
                y += pos.Y;
                z += pos.Z;
            }

            return new[] { x, y, z };
        }

        private static void RemoveParameters(Part part, NXObject[] objects)
        {
            var removeParameters = part.Features.CreateRemoveParametersBuilder();

            using (new Destroyer(removeParameters))
            {
                removeParameters.Objects.Add(objects);

                removeParameters.Commit();
            }
        }

        public void CreateNote0(string detailNumber, Face targetFace, Face toolFace)
        {
            var edgePositions = toolFace._EdgePositions().ToList();

            var edgeSums = SumEdgePositions(edgePositions.ToArray());

            var count = edgePositions.Count;

            var originInAbsolute = new Point3d(edgeSums[0] / count, edgeSums[1] / count, edgeSums[2] / count);

            var compOrigin = targetFace.OwningComponent._Origin();

            var compOrientation = targetFace.OwningComponent._Orientation();

            __display_part_.WCS.SetOriginAndMatrix(compOrigin, compOrientation);

            var origin = originInAbsolute._ToArray();

            var originInTarget = new double[3];

            TheUFSession.Csys.MapPoint(UF_CSYS_ROOT_COORDS, origin, UF_CSYS_ROOT_WCS_COORDS, originInTarget);

            var faceVector = targetFace._NormalVector();

            var orientation = CreateOrientationFromZVector(faceVector);

            __display_part_ = (Part)((Face)targetFace.Prototype).OwningPart;

            var text = __work_part_.__CreateTextFeature(detailNumber, originInTarget._ToPoint3d(), orientation, LENGTH,
                HEIGHT, FONT, SCRIPT);

            var splines = text.GetEntities().OfType<Spline>().ToList();

            RemoveParameters(_WorkPart, splines.Cast<NXObject>().ToArray());

            ModifyDisplay(splines.Cast<DisplayableObject>().ToArray(), COLOR, LAYER);

            ReferenceSet refset;

            if (__display_part_.__HasReferenceSet(REFERENCE_SET))
            {
                refset = __display_part_.__FindReferenceSet(REFERENCE_SET);
            }
            else
            {
                refset = __display_part_.CreateReferenceSet();

                refset.SetName(REFERENCE_SET);
            }

            refset.AddObjectsToReferenceSet(splines.Cast<NXObject>().ToArray());

            refset.AddObjectsToReferenceSet(new NXObject[] { ((Face)targetFace.Prototype).GetBody() });

            throw new NotImplementedException();
        }

        private static string ReadString(string key, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                var index = line.IndexOf('=');

                return line.Substring(index + 1).Trim();
            }

            throw new ArgumentException($"Could not find key \"{key}\"");
        }

        private static int ReadInt(string key, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                var index = line.IndexOf('=');

                return int.Parse(line.Substring(index + 1).Trim());
            }

            throw new ArgumentException($"Could not find key \"{key}\"");
        }

        private static double ReadDouble(string key, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                var index = line.IndexOf('=');

                return double.Parse(line.Substring(index + 1).Trim());
            }

            throw new ArgumentException($"Could not find key \"{key}\"");
        }

        [Obsolete]
        private static TextBuilder.ScriptOptions ReadScriptOptions(string key, IEnumerable<string> lines)
        {
            //foreach (string line in lines)
            //{
            //    if (!line.StartsWith(key))
            //        continue;

            //    int index = line.IndexOf('=');

            //    return (NXOpen.Features.TextBuilder.ScriptOptions)Enum.Parse(typeof(NXOpen.Features.TextBuilder.ScriptOptions), line.Substring(index + 1).Trim());
            //}

            //throw new ArgumentException($"Could not find key \"{key}\"");
            throw new NotImplementedException();
        }
    }
}