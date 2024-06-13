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



        private static void NewMethod2()
        {
            if (_isNewSelection)
                if (_updateComponent == null)
                {
                    NewMethod23();
                }
        }

        private void CreateDynamicHandleUdo(Body editBody)
        {
            try
            {
                UserDefinedClass myUdOclass = session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoDynamicHandle");

                if (myUdOclass is null)
                    return;

                UserDefinedObject[] currentUdo;
                currentUdo = _workPart.UserDefinedObjectManager.GetUdosOfClass(myUdOclass);

                if (currentUdo.Length != 0)
                    return;

                BasePart myBasePart = _workPart;
                UserDefinedObjectManager myUdOmanager = myBasePart.UserDefinedObjectManager;

                foreach (Face blkFace in editBody.GetFaces())
                {
                    UserDefinedObject myUdo = myUdOmanager.CreateUserDefinedObject(myUdOclass);
                    UserDefinedObject.LinkDefinition[] myLinks = new UserDefinedObject.LinkDefinition[1];

                    var pointOnFace = new double[3];
                    var dir = new double[3];
                    var box = new double[6];
                    Matrix3x3 matrix1 = _displayPart.WCS.CoordinateSystem.Orientation.Element;

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

      




        private static void CreateUdo(UserDefinedObject myUdo, UserDefinedObject.LinkDefinition[] myLinks, double[] pointOnFace, string name)
        {
            Point point1 = CreatePoint(pointOnFace, name);
            CreateUdo(myUdo, myLinks, pointOnFace, point1, name);
        }

      

    
    }
}
// 4839