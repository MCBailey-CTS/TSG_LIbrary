using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.CAE;
using NXOpen.Features;
using NXOpen.GeometricAnalysis;
using TSG_Library.Attributes;
using TSG_Library.Disposable;
using TSG_Library.Properties;
using static NXOpen.Selection;
using static NXOpen.UF.UFConstants;
using static TSG_Library.Extensions.Extensions;
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

            string location = Assembly.GetExecutingAssembly().Location;

            string directory = Path.GetDirectoryName(location);

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
            SimpleInterference obj = part.AnalysisManager.CreateSimpleInterferenceObject();

            using (new Destroyer(obj))
            {
                obj.FaceInterferenceType = SimpleInterference.FaceInterferenceMethod.AllPairs;
                obj.InterferenceType = SimpleInterference.InterferenceMethod.InterferingFaces;
                obj.FirstBody.Value = targetBody;
                obj.SecondBody.Value = toolBody;
                obj.PerformCheck();
                NXObject[] result = obj.GetInterferenceResults();
                Face[] faces = new Face[result.Length];

                for (int i = 0; i < result.Length; i += 2)
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
                Face selectedFace = Selection.SelectSingleFace();

                if (selectedFace is null)
                    return;

                Vector3d normal = selectedFace.__NormalVector();

                Matrix3x3 orientation = normal.__ToMatrix3x3();

                Point3d origin = selectedFace.GetEdges()[0].__StartPoint();

                __display_part_.WCS.SetOriginAndMatrix(origin, orientation);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                Main1(WcsOrientation.__AxisZ().__Unit());
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private bool TryCreateNote(string detail, Vector3d expectedTargetVector, Face[] interferingFaces)
        {
            for (int i = 0; i < interferingFaces.Length - 1; i += 2)
            {
                Face targetFace = interferingFaces[i];
                Face toolFace = interferingFaces[i + 1];

                if (!targetFace.__IsPlanar())
                    continue;

                if (!toolFace.__IsPlanar())
                    continue;

                Vector3d targetVector = targetFace.__NormalVector().__Unit();
                Vector3d toolVector = toolFace.__NormalVector().__Unit();

                if (!targetVector.__IsEqualTo(expectedTargetVector))
                    continue;

                if (!toolVector.__IsEqualTo(expectedTargetVector.__Negate()))
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
                Face targetFace = SelectPlanarFace();

                // for now the target face must be an occurrence
                if (!targetFace.IsOccurrence)
                {
                    print_("Please select an occurrence face");
                    return;
                }

                Vector3d target_face_normal = targetFace.__NormalVector();

                Component[] selectedToolComponents = SelectToolComponents();

                foreach (Component tool in selectedToolComponents)
                {
                    Body solid_body_layer_1_proto = tool.__Members()
                        .OfType<Body>()
                        .Single(__b => ((Body)__b.Prototype).Layer == 1);

                    Face[] interferingFaces =
                        _SimpleInterference(_WorkPart, targetFace.GetBody(), solid_body_layer_1_proto);

                    string detail = GetDetailName(interferingFaces[1].OwningComponent);

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
            Text = AssemblyFileVersion;
        }


#pragma warning disable IDE0051 // Remove unused private members
        private static Body[] SelectSolidBodies()
#pragma warning restore IDE0051 // Remove unused private members
        {
            MaskTriple mask = new MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

            TheUISession.SelectionManager.SelectTaggedObjects(
                "Please select tool bodies",
                "Selection",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask },
                out TaggedObject[] selectedObjects);

            return selectedObjects.Cast<Body>().ToArray();
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static Body SelectSolidBody()
#pragma warning restore IDE0051 // Remove unused private members
        {
            MaskTriple mask = new MaskTriple(UF_solid_type, 0, UF_UI_SEL_FEATURE_BODY);

            TheUISession.SelectionManager.SelectTaggedObject(
                "Please select target body",
                "Please select target body",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask }, out TaggedObject selectedObjects,
                out Point3d _);

            return (Body)selectedObjects;
        }

        private static Face SelectPlanarFace()
        {
            MaskTriple mask = new MaskTriple(UF_face_type, 0, UF_UI_SEL_FEATURE_PLANAR_FACE);

            TheUISession.SelectionManager.SelectTaggedObject(
                "Please select target body",
                "Please select target body",
                SelectionScope.AnyInAssembly,
                SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                new[] { mask }, out TaggedObject selectedObjects,
                out Point3d _);

            return (Face)selectedObjects;
        }

        private static string GetDetailName(Component component)
        {
            string displayName = component.DisplayName;

            int length = displayName.Length;

            return displayName.Substring(length - 3);
        }

        private static void ModifyDisplay(DisplayableObject[] displayableObjects, int color, int layer)
        {
            DisplayModification dispMod = session_.DisplayManager.NewDisplayModification();

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
            double[] matrix = new double[9];
            TheUFSession.Mtx3.InitializeZ(vec, matrix);
            Vector3d xVec = new Vector3d(matrix[0], matrix[1], matrix[2]);
            Vector3d yVec = new Vector3d(matrix[3], matrix[4], matrix[5]);
            return xVec.__ToMatrix3x3(yVec);
        }

        private static double[] SumEdgePositions(IEnumerable<Point3d> edgePositions)
        {
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;

            foreach (Point3d pos in edgePositions)
            {
                x += pos.X;
                y += pos.Y;
                z += pos.Z;
            }

            return new[] { x, y, z };
        }

        private static void RemoveParameters(Part part, NXObject[] objects)
        {
            RemoveParametersBuilder removeParameters = part.Features.CreateRemoveParametersBuilder();

            using (new Destroyer(removeParameters))
            {
                removeParameters.Objects.Add(objects);

                removeParameters.Commit();
            }
        }

        public void CreateNote0(string detailNumber, Face targetFace, Face toolFace)
        {
            List<Point3d> edgePositions = toolFace.__EdgePositions().ToList();

            double[] edgeSums = SumEdgePositions(edgePositions.ToArray());

            int count = edgePositions.Count;

            Point3d originInAbsolute = new Point3d(edgeSums[0] / count, edgeSums[1] / count, edgeSums[2] / count);

            Point3d compOrigin = targetFace.OwningComponent.__Origin();

            Matrix3x3 compOrientation = targetFace.OwningComponent.__Orientation();

            __display_part_.WCS.SetOriginAndMatrix(compOrigin, compOrientation);

            double[] origin = originInAbsolute.__ToArray();

            double[] originInTarget = new double[3];

            TheUFSession.Csys.MapPoint(UF_CSYS_ROOT_COORDS, origin, UF_CSYS_ROOT_WCS_COORDS, originInTarget);

            Vector3d faceVector = targetFace.__NormalVector();

            Matrix3x3 orientation = CreateOrientationFromZVector(faceVector);

            __display_part_ = (Part)((Face)targetFace.Prototype).OwningPart;

            Feature text = __work_part_.__CreateTextFeature(detailNumber, originInTarget.__ToPoint3d(), orientation,
                LENGTH,
                HEIGHT, FONT, SCRIPT);

            List<Spline> splines = text.GetEntities().OfType<Spline>().ToList();

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
            foreach (string line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                int index = line.IndexOf('=');

                return line.Substring(index + 1).Trim();
            }

            throw new ArgumentException($"Could not find key \"{key}\"");
        }

        private static int ReadInt(string key, IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                int index = line.IndexOf('=');

                return int.Parse(line.Substring(index + 1).Trim());
            }

            throw new ArgumentException($"Could not find key \"{key}\"");
        }

        private static double ReadDouble(string key, IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (!line.StartsWith(key))
                    continue;

                int index = line.IndexOf('=');

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