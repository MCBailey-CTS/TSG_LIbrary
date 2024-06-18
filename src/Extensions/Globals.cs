using System;
using System.Collections.Generic;
using System.Diagnostics;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using static NXOpen.Session;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        #region Globals

        /// <summary>A function that evaluates a position at a point on a curve</summary>
        /// <param name="data">Data item to be used in evaluation</param>
        /// <param name="t">Parameter value at which to evaluate (in range 0 to 1)</param>
        /// <returns>NXOpen.Point3d on curve at given parameter value</returns>
        /// <remarks>
        ///     <para>
        ///         You use a CurvePositionFunction when constructing approximating curves using
        ///         the
        ///         <see cref="M:Snap.Create.BezierCurveFit(Snap.Create.CurvePositionFunction,System.Object,System.Int32)">BezierCurveFit</see>
        ///         function.
        ///     </para>
        /// </remarks>
        /// <seealso cref="M:Snap.Create.BezierCurveFit(Snap.Create.CurvePositionFunction,System.Object,System.Int32)">BezierCurveFit</seealso>
        public delegate Point3d CurvePositionFunction(object data, double t);

        /// <summary>A function that evaluates a position at a location on a surface</summary>
        /// <param name="data">Data item to be used in evaluation</param>
        /// <param name="uv">Parameter values at which to evaluate (in range [0,1] x [0,1])</param>
        /// <returns>NXOpen.Point3d on surface at given parameter value</returns>
        /// <remarks>
        ///     <para>
        ///         You use a SurfacePositionFunction when constructing approximating surfaces using
        ///         the
        ///         <see
        ///             cref="M:Snap.Create.BezierPatchFit(Snap.Create.SurfacePositionFunction,System.Object,System.Int32,System.Int32)">
        ///             BezierPatchFit
        ///         </see>
        ///         function.
        ///     </para>
        /// </remarks>
        /// <seealso
        ///     cref="M:Snap.Create.BezierPatchFit(Snap.Create.SurfacePositionFunction,System.Object,System.Int32,System.Int32)">
        ///     BezierPatchFit
        /// </seealso>
        public delegate Point3d SurfacePositionFunction(object data, params double[] uv);

        public const string _printerCts = "\\\\ctsfps1.cts.toolingsystemsgroup.com\\CTS Office MFC";

        public const string _simActive = "P:\\CTS_SIM\\Active";

        public static readonly IDictionary<string, ISet<string>> PrefferedDict = new Dictionary<string, ISet<string>>
        {
            ["006"] = new HashSet<string>
            {
                "6mm-shcs-010",
                "6mm-shcs-012.prt",
                "6mm-shcs-016.prt",
                "6mm-shcs-020.prt",
                "6mm-shcs-025.prt",
                "6mm-shcs-030.prt",
                "6mm-shcs-035.prt"
            },

            ["008"] = new HashSet<string>
            {
                "8mm-shcs-012",
                "8mm-shcs-016",
                "8mm-shcs-020",
                "8mm-shcs-025",
                "8mm-shcs-030",
                "8mm-shcs-035",
                "8mm-shcs-040",
                "8mm-shcs-045",
                "8mm-shcs-050",
                "8mm-shcs-055",
                "8mm-shcs-060",
                "8mm-shcs-065"
            },

            ["010"] = new HashSet<string>
            {
                "10mm-shcs-020",
                "10mm-shcs-030",
                "10mm-shcs-040",
                "10mm-shcs-050",
                "10mm-shcs-070",
                "10mm-shcs-090"
            },

            ["012"] = new HashSet<string>
            {
                "12mm-shcs-030",
                "12mm-shcs-040",
                "12mm-shcs-050",
                "12mm-shcs-070",
                "12mm-shcs-090",
                "12mm-shcs-110"
            },

            ["016"] = new HashSet<string>
            {
                "16mm-shcs-040",
                "16mm-shcs-055",
                "16mm-shcs-070",
                "16mm-shcs-090",
                "16mm-shcs-110"
            },

            ["020"] = new HashSet<string>
            {
                "20mm-shcs-050",
                "20mm-shcs-070",
                "20mm-shcs-090",
                "20mm-shcs-110",
                "20mm-shcs-150"
            },

            ["0375"] = new HashSet<string>
            {
                "0375-shcs-075",
                "0375-shcs-125",
                "0375-shcs-200",
                "0375-shcs-300",
                "0375-shcs-400"
            },

            ["0500"] = new HashSet<string>
            {
                "0500-shcs-125",
                "0500-shcs-200",
                "0500-shcs-300",
                "0500-shcs-400",
                "0500-shcs-500"
            },

            ["0625"] = new HashSet<string>
            {
                "0625-shcs-125",
                "0625-shcs-200",
                "0625-shcs-300",
                "0625-shcs-400",
                "0625-shcs-500"
            },

            ["0750"] = new HashSet<string>
            {
                "0750-shcs-200",
                "0750-shcs-300",
                "0750-shcs-400",
                "0750-shcs-500",
                "0750-shcs-650"
            }
        };

        public static UI TheUISession =>
            //[IgnoreExtensionAspect]
            UI.GetUI();

        internal static UFSession ufsession_ =>
            //[IgnoreExtensionAspect]
            UFSession.GetUFSession();

        public static Session session_ =>
            //[IgnoreExtensionAspect]
            GetSession();

        public static double Factor =>
            //[IgnoreExtensionAspect]
            1.0;

        public static Point3d _Point3dOrigin
        {
            //[IgnoreExtensionAspect]
            get { return new[] { 0d, 0d, 0d }.__ToPoint3d(); }
        }

        public static Matrix3x3 _Matrix3x3Identity
        {
            //[IgnoreExtensionAspect]
            get
            {
                double[] array = new double[9];
                ufsession_.Mtx3.Identity(array);
                return array.__ToMatrix3x3();
            }
        }

        /// <summary>
        ///     Multiply by this number to convert Part Units to Millimeters (1 or 25.4)
        /// </summary>
        //[IgnoreExtensionAspect]
        internal static double PartUnitsToMillimeters =>
            //[IgnoreExtensionAspect]
            MillimetersPerUnit;

        /// <summary>
        ///     Multiply by this number to convert Millimeters to Part Units (1 or 0.04)
        /// </summary>
        //[IgnoreExtensionAspect]
        internal static double MillimetersToPartUnits =>
            //[IgnoreExtensionAspect]
            1.0 / PartUnitsToMillimeters;

        /// <summary>
        ///     Multiply by this number to convert Part Units to Inches (1 or 0.04)
        /// </summary>
        internal static double PartUnitsToInches =>
            //[IgnoreExtensionAspect]
            InchesPerUnit;

        /// <summary>
        ///     Multiply by this number to convert Inches to Part Units (either 1 or 25.4)
        /// </summary>
        internal static double InchesToPartUnits =>
            //[IgnoreExtensionAspect]
            1.0 / PartUnitsToInches;

        //
        // Summary:
        //     Multiply by this number to convert Part Units to Meters, to go to Parasolid (0.001
        //     or 0.0254)
        internal static double PartUnitsToMeters =>
            //[IgnoreExtensionAspect]
            0.001 * PartUnitsToMillimeters;

        //
        // Summary:
        //     Multiply by this number to convert Meters to Part Units, when coming from Parasolid
        //     (1000 or 40)
        internal static double MetersToPartUnits =>
            //[IgnoreExtensionAspect]
            1000.0 * MillimetersToPartUnits;

        //
        // Summary:
        //     Multiply by this number to convert Part Units to Points (for font sizes)

        internal static double PartUnitsToPoints =>
            //[IgnoreExtensionAspect]
            PartUnitsToInches * 72.0;

        //
        // Summary:
        //     Multiply by this number to convert Points to Part Units (for font sizes)
        internal static double PointsToPartUnits =>
            //[IgnoreExtensionAspect]
            1.0 / 72.0 * InchesToPartUnits;


        /// <summary>
        ///     Returns the current workComponent origin in terms of the current DisplayPart.
        ///     If the workPart equals the current displayPart then returns the Absolute Origin.
        /// </summary>

        public static Point3d WorkCompOrigin => throw
            //[IgnoreExtensionAspect]
            //if (TSG_Library.Extensions.__work_part_.Tag == TSG_Library.Extensions.DisplayPart.Tag) return BaseOrigin;
            //if (!(NXOpen.Session.session_.Parts.WorkComponent != null)) throw new System.Exception("NullWorkComponentException");
            //return NXOpen.Assemblies.Component.Wrap(NXOpen.Session.session_.Parts.WorkComponent.Tag).Position;
            new NotImplementedException();

        //"CTS Office MFC on ctsfps1.cts.toolingsystemsgroup.com";

        public static string __PrinterCts =>
            //[IgnoreExtensionAspect]
            _printerCts;

        public static string __SimActive =>
            //[IgnoreExtensionAspect]
            _simActive;

        //[IgnoreExtensionAspect]
        public static Part __display_part_
        {
            //[IgnoreExtensionAspect]
            get => session_.Parts.Display;
            //[IgnoreExtensionAspect]
            set => session_.Parts.SetDisplay(value, false, false, out _);
        }

        //[IgnoreExtensionAspect]
        public static Part __work_part_
        {
            //[IgnoreExtensionAspect]
            get => session_.Parts.Work;
            //[IgnoreExtensionAspect]
            set => session_.Parts.SetWork(value);
        }

        public static Part _WorkPart
        {
            //[IgnoreExtensionAspect]
            get => session_.Parts.Work;
            //[IgnoreExtensionAspect]
            set => session_.Parts.SetWork(value);
        }

        public static UFSession _UFSession =>
            //[IgnoreExtensionAspect]
            ufsession_;

        public static UI uisession_ =>
            //[IgnoreExtensionAspect]
            UI.GetUI();

        //[IgnoreExtensionAspect]
        public static CartesianCoordinateSystem __wcs_
        {
            //[IgnoreExtensionAspect]
            get
            {
                ufsession_.Csys.AskWcs(out Tag wcs_id);
                return (CartesianCoordinateSystem)session_.__GetTaggedObject(wcs_id);
            }
            //[IgnoreExtensionAspect]
            set => ufsession_.Csys.SetWcs(value.Tag);
        }

        //[IgnoreExtensionAspect]
        public static Component __work_component_
        {
            //[IgnoreExtensionAspect]
            get => session_.Parts.WorkComponent is null
                ? null
                : session_.Parts.WorkComponent;

            //[IgnoreExtensionAspect]
            set => session_.Parts.SetWorkComponent(
                value,
                PartCollection.RefsetOption.Current,
                PartCollection.WorkComponentOption.Given,
                out _);
        }


        public static UFSession uf_ =>
            //[IgnoreExtensionAspect]
            ufsession_;

        public static UFSession TheUFSession =>
            //[IgnoreExtensionAspect]
            ufsession_;

        public static string TodaysDate
        {
            //[IgnoreExtensionAspect]
            get
            {
                string day = DateTime.Today.Day < 10
                    ? '0' + DateTime.Today.Day.ToString()
                    : DateTime.Today.Day.ToString();
                string month = DateTime.Today.Month < 10
                    ? '0' + DateTime.Today.Month.ToString()
                    : DateTime.Today.Month.ToString();
                return DateTime.Today.Year + "-" + month + "-" + day;
            }
        }

        public static int __process_id =>
            //[IgnoreExtensionAspect]
            Process.GetCurrentProcess().Id;

        public static string __user_name =>
            //[IgnoreExtensionAspect]
            Environment.UserName;


        /// <summary>The work layer (the layer on which newly-created objects should be placed)</summary>
        /// <remarks>
        ///     <para>
        ///         When you change the work layer, the previous work layer is given the status "Selectable".
        ///     </para>
        /// </remarks>
        public static int WorkLayer
        {
            //[IgnoreExtensionAspect]
            get => __work_part_.Layers.WorkLayer;
            //[IgnoreExtensionAspect]
            set => __work_part_.Layers.WorkLayer = value;
        }


        /// <summary>Millimeters Per Unit (either 1 or 25.4)</summary>
        /// <remarks>
        ///     <para>
        ///         A constant representing the number of millimeters in one part unit.
        ///     </para>
        ///     <para>If UnitType == Millimeter, then MillimetersPerUnit = 1.</para>
        ///     <para>If UnitType == Inch, then MillimetersPerUnit = 25.4</para>
        /// </remarks>
        public static double MillimetersPerUnit =>
            //[IgnoreExtensionAspect]
            __work_part_.PartUnits != BasePart.Units.Millimeters ? 25.4 : 1.0;

        /// <summary>Inches per part unit (either 1 or roughly 0.04)</summary>
        /// <remarks>
        ///     <para>
        ///         A constant representing the number of inches in one part unit.
        ///     </para>
        ///     <para>If UnitType = Millimeter, then InchesPerUnit = 0.0393700787402</para>
        ///     <para>If UnitType = Inch, then InchesPerUnit = 1.</para>
        /// </remarks>
        public static double InchesPerUnit =>
            //[IgnoreExtensionAspect]
            __work_part_.PartUnits != BasePart.Units.Millimeters ? 1.0 : 5.0 / sbyte.MaxValue;

        /// <summary>Distance tolerance</summary>
        /// <remarks>
        ///     <para>
        ///         This distance tolerance is the same one that you access via Preferences → Modeling Preferences in interactive
        ///         NX.
        ///         In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        ///         The distance tolerance (together with the angle tolerance) controls the accuracy of this approximation, unless
        ///         you specify some over-riding tolerance within the function itself. For example, when you offset a curve, NX
        ///         will construct a spline curve that approximates the true offset to within the current distance tolerance.
        ///     </para>
        /// </remarks>
        public static double DistanceTolerance
        {
            //[IgnoreExtensionAspect]
            get => __work_part_.Preferences.Modeling.DistanceToleranceData;
            //[IgnoreExtensionAspect]
            set => __work_part_.Preferences.Modeling.DistanceToleranceData = value;
        }

        /// <summary>Angle tolerance, in degrees</summary>
        /// <remarks>
        ///     <para>
        ///         This angle tolerance is the same one that you access via Preference → Modeling Preferences in interactive NX.
        ///         In many functions in NX, an approximation process is used to construct geometry (curves or bodies).
        ///         The angle tolerance (together with the distance tolerance) controls the accuracy of this approximation, unless
        ///         you specify some over-riding tolerance within the function itself. For example, when you create a Through Curve
        ///         Mesh
        ///         feature in NX, the resulting surface will match the input curves to within the current distance and angle
        ///         tolerances.
        ///     </para>
        ///     <para>
        ///         The angle tolerance is expressed in degrees.
        ///     </para>
        /// </remarks>
        public static double AngleTolerance
        {
            //[IgnoreExtensionAspect]
            get => __work_part_.Preferences.Modeling.AngleToleranceData;
            //[IgnoreExtensionAspect]
            set => __work_part_.Preferences.Modeling.AngleToleranceData = value;
        }

        /// <summary>The chaining tolerance used in building "section" objects</summary>
        /// <remarks>
        ///     <para>
        ///         Most modeling features seem to set this internally to 0.95*DistanceTolerance,
        ///         so that's what we use here.
        ///     </para>
        /// </remarks>
        internal static double ChainingTolerance =>
            //[IgnoreExtensionAspect]
            0.95 * DistanceTolerance;

        /// <summary>
        ///     If true, indicates that the modeling mode is set to History mode
        ///     (as opposed to History-free mode).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is the same setting that you access via
        ///         Insert → Synchronous Modeling → History Mode in interactive NX.
        ///         Please refer to the NX documentation for a discussion of the History and
        ///         History-free modeling modes.
        ///     </para>
        ///     <para>
        ///         To create features in SNAP code, you must first set HistoryMode to True.
        ///     </para>
        /// </remarks>
        public static bool HistoryMode
        {
            //[IgnoreExtensionAspect]
            get => __work_part_.Preferences.Modeling.GetHistoryMode();
            //[IgnoreExtensionAspect]
            set
            {
                if (value)
                    __work_part_.Preferences.Modeling.SetHistoryMode();
                else
                    __work_part_.Preferences.Modeling.SetHistoryFreeMode();
            }
        }

        ///// <summary>The unit type of the work part</summary>
        ///// <remarks>
        /////     <para>
        /////         This property only gives the type of the unit.
        /////         To get a Snap.NX.Unit object, please use the
        /////         <see cref="P:TSG_Library.PartUnit">TSG_Library.PartUnit</see>
        /////         property, instead.
        /////     </para>
        ///// </remarks>
        //public static Utilities. Unit UnitType
        //{
        //    //[IgnoreExtensionAspect]
        //    get
        //    {
        //        var workPart = __work_part_;
        //        ufsession_.Part.AskUnits(workPart.Tag, out var part_units);
        //        return part_units == 1 ? Utilities.Unit.Millimeter : Utilities.Unit.Inch;
        //    }
        //}

        /// <summary>
        ///     The work coordinate system (Wcs) of the work part
        /// </summary>
        public static CartesianCoordinateSystem Wcs
        {
            //[IgnoreExtensionAspect]
            get
            {
                UFSession uFSession = ufsession_;
                uFSession.Csys.AskWcs(out Tag wcs_id);
                NXObject objectFromTag = (NXObject)session_.__GetTaggedObject(wcs_id);
                CartesianCoordinateSystem csys = (CartesianCoordinateSystem)objectFromTag;
                return csys;
            }
            //[IgnoreExtensionAspect]
            set
            {
                Tag nXOpenTag = value.Tag;
                ufsession_.Csys.SetWcs(nXOpenTag);
            }
        }

        //
        // Summary:
        //     The orientation of the Wcs of the work part
        public static Matrix3x3 WcsOrientation =>
            //[IgnoreExtensionAspect]
            __wcs_.Orientation.Element;

        //set
        //{
        //    __wcs_.set
        //    NXOpen.Part __work_part_ = __work_part_;
        //    __work_part_.WCS.SetOriginAndMatrix(Wcs.Origin, value);
        //}
        /// <summary>
        /// </summary>
        public static CartesianCoordinateSystem WcsCoordinateSystem =>
            //[IgnoreExtensionAspect]
            session_
                .Parts.Display.CoordinateSystems
                .CreateCoordinateSystem(Wcs.Origin, __display_part_.WCS.__Orientation(), true);

        public static Vector3d __Vector3dX()
        {
            return new[] { 1d, 0d, 0d }.__ToVector3d();
        }

        public static Vector3d __Vector3dY()
        {
            return new[] { 0d, 1d, 0d }.__ToVector3d();
        }

        public static Vector3d __Vector3dZ()
        {
            return new[] { 0d, 0d, 1d }.__ToVector3d();
        }

        //[IgnoreExtensionAspect]
        public static void print_(object __object)
        {
            print_($"{__object}");
        }

        //[IgnoreExtensionAspect]
        public static void prompt_(string message)
        {
            uf_.Ui.SetPrompt(message);
        }

        //[IgnoreExtensionAspect]
        public static void print_(bool __bool)
        {
            print_($"{__bool}");
        }

        //[IgnoreExtensionAspect]
        public static void print_(int __int)
        {
            print_($"{__int}");
        }

        //[IgnoreExtensionAspect]
        public static void print_(string message)
        {
            ListingWindow lw = session_.ListingWindow;

            if (!lw.IsOpen)
                lw.Open();

            lw.WriteLine(message);
        }

        /// <summary>The length unit of the work part</summary>
        /// <remarks>
        /// <para>
        /// This will be either Snap.NX.Unit.Millimeter or Snap.NX.Unit.Inch
        /// </para>
        /// </remarks>
        //public static Unit PartUnit
        //{
        //    get
        //    {
        //        Unit unit = Unit.Millimeter;

        //        if (UnitType == Unit.Inch)
        //            unit = Unit.Inch;

        //        return unit;
        //    }
        //}

        /// <summary>Creates an Undo mark</summary>
        /// <param name="markVisibility">Indicates the visibility of the undo mark</param>
        /// <param name="name">The name to be assigned to the Undo mark</param>
        /// <returns>The ID of the newly created Undo mark</returns>
        /// <remarks>
        ///     <para>
        ///         Creating an Undo mark gives you a way to save the state of the NX session. Then,
        ///         at some later time, you can "roll back" to the Undo mark to restore NX to the saved state.
        ///         This is useful for error recovery, and for reversing any temporary changes you have made
        ///         (such as creation of temporary objects).
        ///     </para>
        ///     <para>
        ///         If you create a visible Undo mark, the name you assign will be shown in
        ///         the Undo List on the NX Edit menu.
        ///     </para>
        ///     <para>
        ///         Please refer to the NX/Open Programmer's Guide for more
        ///         information about Undo marks.
        ///     </para>
        /// </remarks>
        public static UndoMarkId __SetUndoMark(MarkVisibility markVisibility, string name)
        {
            return session_.SetUndoMark(markVisibility, name);
        }

        /// <summary>Deletes an Undo mark</summary>
        /// <param name="markId">The ID of the Undo mark</param>
        /// <param name="markName">The name of the Undo mark.</param>
        /// <remarks>
        ///     <para>
        ///         You can access an Undo mark either using its ID or its name.
        ///         The system will try to find the mark using the given ID, first.
        ///         If this fails (because you have provided an incorrect ID), the system
        ///         will try again to find the mark based on its name.
        ///     </para>
        /// </remarks>
        public static void __DeleteUndoMark(UndoMarkId markId, string markName)
        {
            session_.DeleteUndoMark(markId, markName);
        }

        /// <summary>Roll back to an existing Undo mark</summary>
        /// <param name="markId">The ID of the Undo mark to roll back to</param>
        /// <param name="markName">The name of the Undo mark.</param>
        /// <remarks>
        ///     <para>
        ///         You can access an Undo mark either using its ID or its name.
        ///         The system will try to find the mark using the given ID, first.
        ///         If this fails (because you have provided an incorrect ID), the system
        ///         will try again to find the mark based on its name.
        ///     </para>
        /// </remarks>
        public static void __UndoToMark(UndoMarkId markId, string markName)
        {
            session_.UndoToMark(markId, markName);
        }

        public static string __Op10To010(int __op)
        {
            if (__op == 0)
                return "000";
            if (__op < 10)
                throw new Exception("op integer must be 0 or greater than 9");
            if (__op < 100)
                return $"0{__op}";
            return $"{__op}";
        }

        public static string __Op020To010(string __op)
        {
            return __Op10To010(int.Parse(__op) - 10);
        }

        /// <summary>Get the number of objects on a specified layer</summary>
        /// <param name="layer">The layer number</param>
        /// <returns>The number of objects on the specified layer</returns>
        public static int __LayerObjectCount(int layer)
        {
            return __work_part_.Layers.GetAllObjectsOnLayer(layer).Length;
        }

        /// <summary>Converts degrees to radians</summary>
        /// <param name="angle">An angle measured in degrees</param>
        /// <returns>The same angle measured in radians</returns>
        public static double __DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }

        /// <summary>Converts radians to degrees</summary>
        /// <param name="angle">An angle measured in radians</param>
        /// <returns>The same angle measured in degrees</returns>
        public static double __RadiansToDegrees(double angle)
        {
            return angle * 180.0 / Math.PI;
        }

        #endregion
    }
}