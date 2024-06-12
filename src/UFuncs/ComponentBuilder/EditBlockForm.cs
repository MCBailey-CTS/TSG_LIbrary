using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.Preferences;
using NXOpen.UF;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using NXOpenUI;
using TSG_Library.Properties;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;
using static NXOpen.UF.UFConstants;
using Part = NXOpen.Part;
using NXOpen.CAE;
using NXOpen.CAM;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm : Form
    {
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

        public EditBlockForm()
        {
            InitializeComponent();
        }

        private static void SetDispUnits(Part.Units dispUnits)
        {
            if (dispUnits == Part.Units.Millimeters)
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.GMmNDegC);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults.GMmNDegC);
            }
            else
            {
                _displayPart.UnitCollection.SetDefaultDataEntryUnits(UnitCollection.UnitDefaults.LbmInLbfDegF);
                _displayPart.UnitCollection.SetDefaultObjectInformationUnits(UnitCollection.UnitDefaults
                    .LbmInLbfDegF);
            }
        }

        private void DeleteHandles()
        {
            Session.UndoMarkId markDeleteObjs = session_.SetUndoMark(Session.MarkVisibility.Invisible, "");
            var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

            if (myUdOclass != null)
            {
                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);
                session_.UpdateManager.AddObjectsToDeleteList(currentUdo);
            }

            foreach (Point namedPt in _workPart.Points)
                if (namedPt.Name != "")
                    session_.UpdateManager.AddToDeleteList(namedPt);

            foreach (var dLine in _edgeRepLines)
                session_.UpdateManager.AddToDeleteList(dLine);

            int errsDelObjs;
            errsDelObjs = session_.UpdateManager.DoUpdate(markDeleteObjs);

            session_.DeleteUndoMark(markDeleteObjs, "");

            _edgeRepLines.Clear();
        }

        private void ShowTemporarySizeText()
        {
            _displayPart.Views.Refresh();

            foreach (var eLine in _edgeRepLines)
            {
                if (eLine.Name != "XBASE1" && eLine.Name != "YBASE1" && eLine.Name != "ZBASE1")
                    continue;

                var view = _displayPart.Views.WorkView.Tag;
                var viewType = UFDisp.ViewType.UseWorkView;
                var dim = string.Empty;

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                {
                    var roundDim = Math.Round(eLine.GetLength(), 3);
                    dim = $"{roundDim:0.000}";
                }
                else
                {
                    var roundDim = Math.Round(eLine.GetLength(), 3);
                    dim = $"{roundDim / 25.4:0.000}";
                }

                var midPoint = new double[3];
                var dispProps = new UFObj.DispProps
                {
                    color = 31
                };
                double charSize;
                var font = 1;

                if (_displayPart.PartUnits == BasePart.Units.Inches)
                    charSize = .125;
                else
                    charSize = 3.175;

                midPoint[0] = (eLine.StartPoint.X + eLine.EndPoint.X) / 2;
                midPoint[1] = (eLine.StartPoint.Y + eLine.EndPoint.Y) / 2;
                midPoint[2] = (eLine.StartPoint.Z + eLine.EndPoint.Z) / 2;

                var mappedPoint = new double[3];

                ufsession_.Csys.MapPoint(UF_CSYS_WORK_COORDS, midPoint, UF_CSYS_ROOT_COORDS, mappedPoint);

                ufsession_.Disp.DisplayTemporaryText(view, viewType, dim, mappedPoint, UFDisp.TextRef.Middlecenter,
                    ref dispProps, charSize, font);
            }
        }

        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                var myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length != 0)
                    return;

                BasePart myBasePart = _workPart;
                var myUdOmanager = myBasePart.UserDefinedObjectManager;

                foreach (var blkFace in editBody.GetFaces())
                {
                    var myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                    var myLinks = new UserDefinedObject.LinkDefinition[1];

                    var pointOnFace = new double[3];
                    var dir = new double[3];
                    var box = new double[6];
                    var matrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                    ufsession_.Modl.AskFaceData(blkFace.Tag, out var type, pointOnFace, dir, box,
                        out var radius, out var radData, out var normDir);

                    dir[0] = Math.Round(dir[0], 10);
                    dir[1] = Math.Round(dir[1], 10);
                    dir[2] = Math.Round(dir[2], 10);

                    double[] wcsVectorX =
                        { Math.Round(matrix1.Xx, 10), Math.Round(matrix1.Xy, 10), Math.Round(matrix1.Xz, 10) };
                    double[] wcsVectorY =
                        { Math.Round(matrix1.Yx, 10), Math.Round(matrix1.Yy, 10), Math.Round(matrix1.Yz, 10) };
                    double[] wcsVectorZ =
                        { Math.Round(matrix1.Zx, 10), Math.Round(matrix1.Zy, 10), Math.Round(matrix1.Zz, 10) };

                    var wcsVectorNegX = new double[3];
                    var wcsVectorNegY = new double[3];
                    var wcsVectorNegZ = new double[3];

                    ufsession_.Vec3.Negate(wcsVectorX, wcsVectorNegX);
                    ufsession_.Vec3.Negate(wcsVectorY, wcsVectorNegY);
                    ufsession_.Vec3.Negate(wcsVectorZ, wcsVectorNegZ);

                    // create udo handle points

                    ufsession_.Vec3.IsEqual(dir, wcsVectorX, 0.00, out var isEqualX);

                    if (isEqualX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorY, 0.00, out var isEqualY);

                    if (isEqualY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorZ, 0.00, out var isEqualZ);

                    if (isEqualZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "POSZ");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegX, 0.00, out var isEqualNegX);

                    if (isEqualNegX == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGX");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegY, 0.00, out var isEqualNegY);

                    if (isEqualNegY == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGY");

                    ufsession_.Vec3.IsEqual(dir, wcsVectorNegZ, 0.00, out var isEqualNegZ);

                    if (isEqualNegZ == 1)
                        CreateUdo(myUdo, myLinks, pointOnFace, "NEGZ");
                }

                // create origin point

                CreatePointBlkOrigin();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if (spacing == 0)
                return cursor;

            return _displayPart.PartUnits == BasePart.Units.Inches
                ? RoundEnglish(spacing, cursor)
                : RoundMetric(spacing, cursor);
        }

        private static double RoundEnglish(double spacing, double cursor)
        {
            var round = Math.Abs(cursor);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing; ii <= 1; ii += spacing)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round;
        }

        private static double RoundMetric(double spacing, double cursor)
        {
            var round = Math.Abs(cursor / 25.4);
            var roundValue = Math.Round(round, 3);
            var truncateValue = Math.Truncate(roundValue);
            var fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (var ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                    if (fractionValue <= ii)
                    {
                        var roundedFraction = ii;
                        var finalValue = truncateValue + roundedFraction;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round * 25.4;
        }

        private static void CreatePointBlkOrigin()
        {
            var pointLocationOrigin = _displayPart.WCS.Origin;
            var point1Origin = _workPart.Points.CreatePoint(pointLocationOrigin);
            point1Origin.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1Origin.Blank();
            point1Origin.SetName("BLKORIGIN");
            point1Origin.Layer = _displayPart.Layers.WorkLayer;
            point1Origin.RedisplayObject();
        }

        private static Point CreatePoint(double[] pointOnFace, string name)
        {
            var pointLocation = pointOnFace.__ToPoint3d();
            var point1 = _workPart.Points.CreatePoint(pointLocation);
            point1.SetVisibility(SmartObject.VisibilityOption.Visible);
            point1.SetName(name);
            point1.Layer = _displayPart.Layers.WorkLayer;
            point1.RedisplayObject();
            return point1;
        }

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, string name)
        {
            Point point1 = CreatePoint(pointOnFace, name);
            CreateUdo(myUdo, myLinks, pointOnFace, point1, name);
        }

        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, Point point1, string name)
        {
            myUdo.SetName(name);
            myUdo.SetDoubles(pointOnFace);
            int[] displayFlag = { 0 };
            myUdo.SetIntegers(displayFlag);

            myLinks[0].AssociatedObject = point1;
            myLinks[0].Status = UserDefinedObject.LinkStatus.UpToDate;
            myUdo.SetLinks(UserDefinedObject.LinkType.Type1, myLinks);
        }

        private void MoveObjects(NXObject[] objsToMove, double distance, string deltaXyz)
        {
            try
            {
                if (distance == 0)
                    return;

                _displayPart.WCS.SetOriginAndMatrix(_workCompOrigin, _workCompOrientation);

                MoveObject nullFeaturesMoveObject = null;

                MoveObjectBuilder moveObjectBuilder1;
                moveObjectBuilder1 = _workPart.BaseFeatures.CreateMoveObjectBuilder(nullFeaturesMoveObject);

                moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.AxisOption =
                    OrientXpressBuilder.Axis.Passive;

                moveObjectBuilder1.TransformMotion.DistanceAngle.OrientXpress.PlaneOption =
                    OrientXpressBuilder.Plane.Passive;

                moveObjectBuilder1.TransformMotion.OrientXpress.AxisOption = OrientXpressBuilder.Axis.Passive;

                moveObjectBuilder1.TransformMotion.OrientXpress.PlaneOption = OrientXpressBuilder.Plane.Passive;

                Point3d manipulatororigin1;
                manipulatororigin1 = _displayPart.WCS.Origin;

                Matrix3x3 manipulatormatrix1;
                manipulatormatrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

                moveObjectBuilder1.TransformMotion.Option = ModlMotion.Options.DeltaXyz;

                moveObjectBuilder1.TransformMotion.DeltaEnum = ModlMotion.Delta.ReferenceWcsWorkPart;

                if (deltaXyz == "X")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = distance.ToString();
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                }

                if (deltaXyz == "Y")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = distance.ToString();
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = "0";
                }

                if (deltaXyz == "Z")
                {
                    moveObjectBuilder1.TransformMotion.DeltaXc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaYc.RightHandSide = "0";
                    moveObjectBuilder1.TransformMotion.DeltaZc.RightHandSide = distance.ToString();
                }

                bool added1;
                added1 = moveObjectBuilder1.ObjectToMoveObject.Add(objsToMove);

                NXObject nXObject1;
                nXObject1 = moveObjectBuilder1.Commit();

                moveObjectBuilder1.Destroy();

                _workPart.FacetedBodies.DeleteTemporaryFacesAndEdges();
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod2()
        {
            if (_isNewSelection)
                if (_updateComponent == null)
                {
                    NewMethod23();
                }
        }
    }
}
// 4839