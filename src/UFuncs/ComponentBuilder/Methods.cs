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
        private Point3d MapWcsToAbsolute(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_WCS_COORDS, input, UF_CSYS_ROOT_COORDS, output);
            return output.__ToPoint3d();
        }

        private Point3d MapAbsoluteToWcs(Point3d pointToMap)
        {
            double[] input = { pointToMap.X, pointToMap.Y, pointToMap.Z };
            double[] output = new double[3];
            ufsession_.Csys.MapPoint(UF_CSYS_ROOT_COORDS, input, UF_CSYS_ROOT_WCS_COORDS, output);
            return output.__ToPoint3d();
        }




        private void LoadGridSizes()
        {
            comboBoxGridBlock.Items.Clear();

            if (__display_part_.PartUnits == BasePart.Units.Inches)
            {
                comboBoxGridBlock.Items.Add("0.002");
                comboBoxGridBlock.Items.Add("0.03125");
                comboBoxGridBlock.Items.Add("0.0625");
                comboBoxGridBlock.Items.Add("0.125");
                comboBoxGridBlock.Items.Add("0.250");
                comboBoxGridBlock.Items.Add("0.500");
                comboBoxGridBlock.Items.Add("1.000");
            }
            else
            {
                comboBoxGridBlock.Items.Add("0.0508");
                comboBoxGridBlock.Items.Add("0.79375");
                comboBoxGridBlock.Items.Add("1.00");
                comboBoxGridBlock.Items.Add("1.5875");
                comboBoxGridBlock.Items.Add("3.175");
                comboBoxGridBlock.Items.Add("5.00");
                comboBoxGridBlock.Items.Add("6.35");
                comboBoxGridBlock.Items.Add("12.7");
                comboBoxGridBlock.Items.Add("25.4");
            }

            foreach (string gridSetting in comboBoxGridBlock.Items)
                if (gridSetting == Settings.Default.EditBlockFormGridIncrement)
                {
                    comboBoxGridBlock.SelectedIndex = comboBoxGridBlock.Items.IndexOf(gridSetting);
                    break;
                }
        }

        private void SetWorkPlaneOn()
        {
            try
            {
                WorkPlane workPlane1;
                workPlane1 = __display_part_.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = true;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;

#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    NewMethod33();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }



        private void SetWorkPlaneOff()
        {
            try
            {

                WorkPlane workPlane1;
                workPlane1 = __display_part_.Preferences.Workplane;

                if (workPlane1 != null)
                {
                    workPlane1.GridType = WorkPlane.Grid.Rectangular;

                    workPlane1.GridIsNonUniform = false;

                    WorkPlane.GridSize gridSize1 = new WorkPlane.GridSize(_gridSpace, 1, 1);
                    workPlane1.SetRectangularUniformGridSize(gridSize1);

                    workPlane1.ShowGrid = false;

                    workPlane1.ShowLabels = false;

                    workPlane1.SnapToGrid = false;

                    workPlane1.GridOnTop = false;

                    workPlane1.RectangularShowMajorLines = false;

                    workPlane1.PolarShowMajorLines = false;

                    workPlane1.GridColor = 7;
#pragma warning disable CS0618
                    session_.Preferences.WorkPlane.ObjectOffWorkPlane = SessionWorkPlane.ObjectDisplay.Normal;
#pragma warning restore CS0618
                }
                else
                {
                    NewMethod32();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }


        public int Startup()
        {
            if (_registered == 0)
            {
                EditBlockForm editForm = this;
                _idWorkPartChanged1 = session_.Parts.AddWorkPartChangedHandler(editForm.WorkPartChanged1);
                _registered = 1;
            }

            return 0;
        }

        public void WorkPartChanged1(BasePart p)
        {
            SetWorkPlaneOff();
            LoadGridSizes();
            SetWorkPlaneOn();
        }







        private List<Point> SelectHandlePoint()
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_point_type, UF_point_subtype, 0);
            Selection.Response sel;
            List<Point> pointSelection = new List<Point>();

            sel = TheUISession.SelectionManager.SelectTaggedObject("Select Point", "Select Point",
                Selection.SelectionScope.WorkPart,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject selectedPoint, out Point3d cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
                pointSelection.Add((Point)selectedPoint);

            return pointSelection;
        }

        private Component SelectOneComponent(string prompt)
        {
            Selection.MaskTriple[] mask = new Selection.MaskTriple[1];
            mask[0] = new Selection.MaskTriple(UF_component_type, 0, 0);
            Selection.Response sel;
            Component compSelection = null;

            sel = TheUISession.SelectionManager.SelectTaggedObject(prompt, prompt,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false, false, mask, out TaggedObject selectedComp, out Point3d cursor);

            if ((sel == Selection.Response.ObjectSelected) | (sel == Selection.Response.ObjectSelectedByName))
            {
                compSelection = (Component)selectedComp;
                return compSelection;
            }

            return null;
        }



        private double RoundDistanceToGrid(double spacing, double cursor)
        {
            if (spacing == 0)
                return cursor;

            return __display_part_.PartUnits == BasePart.Units.Inches
                ? RoundEnglish(spacing, cursor)
                : RoundMetric(spacing, cursor);
        }

        private static double RoundEnglish(double spacing, double cursor)
        {
            double round = Math.Abs(cursor);
            double roundValue = Math.Round(round, 3);
            double truncateValue = Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (double ii = spacing; ii <= 1; ii += spacing)
                    if (fractionValue <= ii)
                    {
                        double finalValue = truncateValue + ii;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round;
        }

        private static double RoundMetric(double spacing, double cursor)
        {
            double round = Math.Abs(cursor / 25.4);
            double roundValue = Math.Round(round, 3);
            double truncateValue = Math.Truncate(roundValue);
            double fractionValue = roundValue - truncateValue;
            if (fractionValue != 0)
                for (double ii = spacing / 25.4; ii <= 1; ii += spacing / 25.4)
                    if (fractionValue <= ii)
                    {
                        double finalValue = truncateValue + ii;
                        round = finalValue;
                        break;
                    }

            if (cursor < 0) round *= -1;

            return round * 25.4;
        }



        private static void GetLines(
            out List<Line> posXObjs, out List<Line> negXObjs,
            out List<Line> posYObjs, out List<Line> negYObjs,
            out List<Line> posZObjs, out List<Line> negZObjs)
        {
            posXObjs = new List<Line>();
            negXObjs = new List<Line>();
            posYObjs = new List<Line>();
            negYObjs = new List<Line>();
            posZObjs = new List<Line>();
            negZObjs = new List<Line>();

            foreach (Line eLine in _edgeRepLines)
            {
                if (eLine.Name == "YBASE1" || eLine.Name == "YCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE3") negXObjs.Add(eLine);

                if (eLine.Name == "YBASE2" || eLine.Name == "YCEILING2" ||
                   eLine.Name == "ZBASE2" || eLine.Name == "ZBASE4") posXObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XCEILING1" ||
                   eLine.Name == "ZBASE1" || eLine.Name == "ZBASE2") negYObjs.Add(eLine);

                if (eLine.Name == "XBASE2" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "ZBASE3" || eLine.Name == "ZBASE4") posYObjs.Add(eLine);

                if (eLine.Name == "XBASE1" || eLine.Name == "XBASE2" ||
                   eLine.Name == "YBASE1" || eLine.Name == "YBASE2") negZObjs.Add(eLine);

                if (eLine.Name == "XCEILING1" || eLine.Name == "XCEILING2" ||
                   eLine.Name == "YCEILING1" || eLine.Name == "YCEILING2") posZObjs.Add(eLine);
            }
        }


        private bool SetDispUnits()
        {
            buttonApply.Enabled = true;
            bool isBlockComponent = false;
            Part.Units dispUnits = (Part.Units)__display_part_.PartUnits;
            SetDispUnits(dispUnits);
            return isBlockComponent;
        }
    }
}
// 2045