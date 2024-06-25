using System;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using TSG_Library.Attributes;
using TSG_Library.UFuncs;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc("auto-size-component")]
    //[RevisionEntry("1.0", "2017", "06", "05")]
    //[Revision("1.0.1", "Revision Log Created for NX 11.")]
    //[RevisionEntry("1.1", "2017", "08", "22")]
    //[Revision("1.1.1", "Signed so it will run outside of CTS.")]
    //[RevisionEntry("1.2", "2018", "06", "11")]
    //[Revision("1.2.1",
    //    "When updating the “Description” attribute, if the part in question contains an expression named “lwrParallel” or “uprParallel” and its’ value == “yes” then we simply just add the text “PARALLEL” to the end of the “DESCRIPTION” attribute.")]
    //[Revision("1.2.2", "Per Tsg-Cit #2018-0198")]
    //[RevisionEntry("11.1", "2023", "01", "09")]
    //[Revision("11.1.1", "Removed validation")]
    public class AutoSizeComponent : _UFunc
    {
        private const double Tolerance = .0001;
        private static UserDefinedClass _myUdoClass;

        public static int Startup()
        {
            const int retValue = 0;

            try
            {
                initializeUDO(false);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return retValue;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        public static int initializeUDO(bool alertUser)
        {
            try
            {
                if (!(_myUdoClass is null))
                {
                    print_("udo already exists");
                    return 0;
                }

                if (alertUser)
                    UI.GetUI()
                        .NXMessageBox.Show(
                            "UDO",
                            NXMessageBox.DialogType.Information,
                            "Registering C# UDO Class"
                        );

                // Define your custom UDO class
                _myUdoClass = session_.UserDefinedClassManager.CreateUserDefinedObjectClass(
                    "UdoAutoSizeComponent",
                    "Update Order Size"
                );
                // Setup properties on the custom UDO class
                _myUdoClass.AllowQueryClassFromName = UserDefinedClass.AllowQueryClass.On;
                // Register callbacks for the UDO class
                _myUdoClass.AddUpdateHandler(myUpdateCB);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return 0;
        }

        public static int myUpdateCB(UserDefinedLinkEvent updateEvent)
        {
            try
            {
                if (updateEvent.AssociatedObject is null)
                    return 0;

                int assocObjStatus = TheUFSession.Obj.AskStatus(updateEvent.AssociatedObject.Tag);
                Tag solidBodyTag = updateEvent.AssociatedObject.Tag;
                Body udoBody = (Body)NXObjectManager.Get(solidBodyTag);
                Part updatePart = (Part)udoBody.OwningPart;

                if (assocObjStatus != 3)
                    return 0;

                int[] updateFlag = updateEvent.UserDefinedObject.GetIntegers();

                if (updateFlag[0] == 1)
                    SizeComponent(updatePart);

                return 0;
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return 0;
        }

        public static int Main1()
        {
            const int retValue = 0;
            try
            {
                UserDefinedClass myUdoClass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName(
                        "UdoAutoSizeComponent"
                    );

                if (myUdoClass is null)
                    return retValue;

                UserDefinedObject[] currentUdo =
                    __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdoClass);

                if (currentUdo.Length != 0)
                    SizeComponent(__work_part_);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }

            return retValue;
        }

        public static void SizeComponent(Part updatePartSize)
        {
            try
            {
                bool isMetric = false;
                BasePart basePart = updatePartSize;

                if (basePart.PartUnits == BasePart.Units.Millimeters)
                    isMetric = true;

                foreach (Feature featDynamic in __work_part_.Features.ToArray())
                {
                    if (featDynamic.FeatureType != "BLOCK")
                        continue;
                    if (featDynamic.Name != "DYNAMIC BLOCK")
                        continue;
                    Block block1 = (Block)featDynamic;
                    Body[] sizeBody = block1.GetBodies();

                    BlockFeatureBuilder blockFeatureBuilderSize =
                        __work_part_.Features.CreateBlockFeatureBuilder(block1);

                    blockFeatureBuilderSize.GetOrientation(out Vector3d xAxis, out Vector3d yAxis);

                    double[] initOrigin =
                    {
                        blockFeatureBuilderSize.Origin.X,
                        blockFeatureBuilderSize.Origin.Y,
                        blockFeatureBuilderSize.Origin.Z
                    };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    double[] initMatrix = new double[9];
                    TheUFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                    TheUFSession.Csys.CreateMatrix(initMatrix, out Tag tempMatrix);
                    TheUFSession.Csys.CreateTempCsys(initOrigin, tempMatrix, out Tag tempCsys);

                    if (tempCsys == Tag.Null)
                    {
                        UI.GetUI()
                            .NXMessageBox.Show(
                                "Auto Size Update Error",
                                NXMessageBox.DialogType.Error,
                                "Description update failed " + updatePartSize.FullPath
                            );
                        continue;
                    }

                    // get named expressions

                    bool isNamedExpression = false;

                    double xValue = 0,
                        yValue = 0,
                        zValue = 0;

                    string burnDirValue = string.Empty;
                    string burnoutValue = string.Empty;
                    string grindValue = string.Empty;
                    string grindTolValue = string.Empty;
                    string diesetValue = string.Empty;
                    NewMethod7(
                        ref isNamedExpression,
                        ref xValue,
                        ref yValue,
                        ref zValue,
                        ref burnDirValue,
                        ref burnoutValue,
                        ref grindValue,
                        ref grindTolValue,
                        ref diesetValue
                    );

                    burnDirValue = burnDirValue.Replace("\"", string.Empty);
                    burnoutValue = burnoutValue.Replace("\"", string.Empty);
                    grindValue = grindValue.Replace("\"", string.Empty);
                    grindTolValue = grindTolValue.Replace("\"", string.Empty);
                    diesetValue = diesetValue.Replace("\"", string.Empty);

                    if (isNamedExpression)
                    {
                        // get bounding box of solid body

                        double[] minCorner = new double[3];
                        double[,] directions = new double[3, 3];
                        double[] distances = new double[3];
                        double[] grindDistances = new double[3];

                        TheUFSession.Modl.AskBoundingBoxExact(
                            sizeBody[0].Tag,
                            tempCsys,
                            minCorner,
                            directions,
                            distances
                        );
                        TheUFSession.Modl.AskBoundingBoxExact(
                            sizeBody[0].Tag,
                            tempCsys,
                            minCorner,
                            directions,
                            grindDistances
                        );

                        NewMethod6(isMetric, xValue, yValue, zValue, burnoutValue, distances);

                        double xDist = distances[0];
                        double yDist = distances[1];
                        double zDist = distances[2];

                        double xGrindDist = grindDistances[0];
                        double yGrindDist = grindDistances[1];
                        double zGrindDist = grindDistances[2];

                        Array.Sort(distances);
                        Array.Sort(grindDistances);

                        // ReSharper disable once ConvertIfStatementToSwitchStatement
                        if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                        {
                            updatePartSize.SetUserAttribute(
                                "DESCRIPTION",
                                -1,
                                $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}",
                                Update.Option.Now
                            );
                        }
                        else if (burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                        {
                            // ReSharper disable once ConvertIfStatementToSwitchStatement
                            if (burnDirValue.ToLower() == "x")
                            {
                                NewMethod5(grindTolValue, distances, grindDistances, xGrindDist);
                            }

                            if (burnDirValue.ToLower() == "y")
                            {
                                NewMethod4(grindTolValue, distances, grindDistances, yGrindDist);
                            }

                            if (burnDirValue.ToLower() == "z")
                            {
                                NewMethod3(grindTolValue, distances, grindDistances, zGrindDist);
                            }
                        }
                        else
                        {
                            if (grindValue.ToLower() == "yes")
                            {
                                // ReSharper disable once ConvertIfStatementToSwitchStatement
                                NewMethod2(
                                    burnDirValue,
                                    grindTolValue,
                                    xGrindDist,
                                    yGrindDist,
                                    zGrindDist
                                );
                            }
                            else
                            {
                                // ReSharper disable once ConvertIfStatementToSwitchStatement
                                NewMethod1(burnDirValue, xDist, yDist, zDist);
                            }
                        }

                        if (diesetValue != "yes")
                            continue;
                        string description = updatePartSize.GetStringUserAttribute(
                            "DESCRIPTION",
                            -1
                        );

                        if (description.ToLower().Contains("dieset"))
                            continue;
                        description += " DIESET";
                        updatePartSize.SetUserAttribute(
                            "DESCRIPTION",
                            -1,
                            description,
                            Update.Option.Now
                        );
                    }
                    else
                    {
                        double[] distances = NewMethod(isMetric, sizeBody, tempCsys);
                        // CreateOrNull the description attribute

                        Array.Sort(distances);

                        updatePartSize.SetUserAttribute(
                            "DESCRIPTION",
                            -1,
                            $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}",
                            Update.Option.Now
                        );

                        if (diesetValue != "yes")
                            continue;
                        string description = updatePartSize.GetStringUserAttribute(
                            "DESCRIPTION",
                            -1
                        );

                        if (description.ToLower().Contains("dieset"))
                            continue;
                        description += " DIESET";
                        updatePartSize.SetUserAttribute(
                            "DESCRIPTION",
                            -1,
                            description,
                            Update.Option.Now
                        );
                    }
                }

                // If the work part does not have a {"DESCRIPTION"} attribute then we want to return;.
                if (
                    !updatePartSize.HasUserAttribute(
                        "DESCRIPTION",
                        NXObject.AttributeType.String,
                        -1
                    )
                )
                    return;

                // The string value of the {"DESCRIPTION"} attribute.
                string descriptionAtt = updatePartSize.GetUserAttributeAsString(
                    "DESCRIPTION",
                    NXObject.AttributeType.String,
                    -1
                );

                Expression[] expressions = updatePartSize.Expressions.ToArray();

                // Checks to see if the {_workPart} contains an expression with value {"yes"} and name of {lwrParallel} or {uprParallel}.
                if (
                    expressions.Any(
                        exp =>
                            (
                                exp.Name.ToLower() == "lwrparallel"
                                || exp.Name.ToLower() == "uprparallel"
                            )
                            && exp.StringValue.ToLower() == "yes"
                    )
                )
                    // Appends {"Parallel"} to the end of the {"DESCRIPTION"} attribute string value and then sets the it to be the value of the {"DESCRIPTION"} attribute.
                    updatePartSize.SetUserAttribute(
                        "DESCRIPTION",
                        -1,
                        descriptionAtt + " PARALLEL",
                        Update.Option.Now
                    );
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
        }

        private static void NewMethod7(
            ref bool isNamedExpression,
            ref double xValue,
            ref double yValue,
            ref double zValue,
            ref string burnDirValue,
            ref string burnoutValue,
            ref string grindValue,
            ref string grindTolValue,
            ref string diesetValue
        )
        {
            foreach (Expression exp in __work_part_.Expressions.ToArray())
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (exp.Name == "AddX")
                {
                    isNamedExpression = true;
                    xValue = exp.Value;
                }

                if (exp.Name == "AddY")
                {
                    isNamedExpression = true;
                    yValue = exp.Value;
                }

                if (exp.Name == "AddZ")
                {
                    isNamedExpression = true;
                    zValue = exp.Value;
                }

                if (exp.Name == "BurnDir")
                {
                    isNamedExpression = true;
                    burnDirValue = exp.RightHandSide;
                }

                if (exp.Name == "Burnout")
                {
                    isNamedExpression = true;
                    burnoutValue = exp.RightHandSide;
                }

                if (exp.Name == "Grind")
                {
                    isNamedExpression = true;
                    grindValue = exp.RightHandSide;
                }

                if (exp.Name == "GrindTolerance")
                {
                    isNamedExpression = true;
                    grindTolValue = exp.RightHandSide;
                }

                if (exp.Name == "DiesetNote")
                    diesetValue = exp.RightHandSide;
            }
        }

        private static void NewMethod6(
            bool isMetric,
            double xValue,
            double yValue,
            double zValue,
            string burnoutValue,
            double[] distances
        )
        {
            // add stock values

            distances[0] += xValue;
            distances[1] += yValue;
            distances[2] += zValue;

            if (isMetric)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            if (burnoutValue.ToLower() == "no")
                for (int i = 0; i < 3; i++)
                {
                    double roundValue = Math.Round(distances[i], 3);
                    double truncateValue = Math.Truncate(roundValue);
                    double fractionValue = roundValue - truncateValue;
                    if (Math.Abs(fractionValue) > .0001)
                        for (double ii = .125; ii <= 1; ii += .125)
                        {
                            if (!(fractionValue <= ii))
                                continue;
                            double finalValue = truncateValue + ii;
                            distances[i] = finalValue;
                            break;
                        }
                    else
                        distances[i] = roundValue;
                }
        }

        private static void NewMethod5(
            string grindTolValue,
            double[] distances,
            double[] grindDistances,
            double xGrindDist
        )
        {
            if (Math.Abs(xGrindDist - grindDistances[0]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(xGrindDist - grindDistances[1]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(xGrindDist - grindDistances[2]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                    Update.Option.Now
                );
        }

        private static void NewMethod4(
            string grindTolValue,
            double[] distances,
            double[] grindDistances,
            double yGrindDist
        )
        {
            if (Math.Abs(yGrindDist - grindDistances[0]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{grindDistances[0]:f3}"
                        + " "
                        + grindTolValue
                        + " X "
                        + $"{distances[1]:f2}"
                        + " X "
                        + $"{distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(yGrindDist - grindDistances[1]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2}"
                        + " X "
                        + $"{grindDistances[1]:f3}"
                        + " "
                        + grindTolValue
                        + " X "
                        + $"{distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(yGrindDist - grindDistances[2]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2}"
                        + " X "
                        + $"{distances[1]:f2}"
                        + " X "
                        + $"{grindDistances[2]:f3}"
                        + " "
                        + grindTolValue,
                    Update.Option.Now
                );
        }

        private static void NewMethod3(
            string grindTolValue,
            double[] distances,
            double[] grindDistances,
            double zGrindDist
        )
        {
            if (Math.Abs(zGrindDist - grindDistances[0]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{grindDistances[0]:f3}"
                        + " "
                        + grindTolValue
                        + " X "
                        + $"{distances[1]:f2}"
                        + " X "
                        + $"{distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(zGrindDist - grindDistances[1]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2}"
                        + " X "
                        + $"{grindDistances[1]:f3}"
                        + " "
                        + grindTolValue
                        + " X "
                        + $"{distances[2]:f2}",
                    Update.Option.Now
                );

            if (Math.Abs(zGrindDist - grindDistances[2]) < Tolerance)
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    $"{distances[0]:f2}"
                        + " X "
                        + $"{distances[1]:f2}"
                        + " X "
                        + $"{grindDistances[2]:f3}"
                        + " "
                        + grindTolValue,
                    Update.Option.Now
                );
        }

        private static void NewMethod2(
            string burnDirValue,
            string grindTolValue,
            double xGrindDist,
            double yGrindDist,
            double zGrindDist
        )
        {
            if (burnDirValue.ToLower() == "x")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{xGrindDist:f3}" + " " + grindTolValue,
                    Update.Option.Now
                );

            if (burnDirValue.ToLower() == "y")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{yGrindDist:f3}" + " " + grindTolValue,
                    Update.Option.Now
                );

            if (burnDirValue.ToLower() == "z")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{zGrindDist:f3}" + " " + grindTolValue,
                    Update.Option.Now
                );
        }

        private static void NewMethod1(
            string burnDirValue,
            double xDist,
            double yDist,
            double zDist
        )
        {
            if (burnDirValue.ToLower() == "x")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{xDist:f2}",
                    Update.Option.Now
                );

            if (burnDirValue.ToLower() == "y")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{yDist:f2}",
                    Update.Option.Now
                );

            if (burnDirValue.ToLower() == "z")
                __work_part_.SetUserAttribute(
                    "DESCRIPTION",
                    -1,
                    "BURN " + $"{zDist:f2}",
                    Update.Option.Now
                );
        }

        private static double[] NewMethod(bool isMetric, Body[] sizeBody, Tag tempCsys)
        {
            // get bounding box of solid body

            double[] minCorner = new double[3];
            double[,] directions = new double[3, 3];
            double[] distances = new double[3];

            TheUFSession.Modl.AskBoundingBoxExact(
                sizeBody[0].Tag,
                tempCsys,
                minCorner,
                directions,
                distances
            );

            if (isMetric)
                for (int i = 0; i < distances.Length; i++)
                    distances[i] /= 25.4d;

            for (int i = 0; i < 3; i++)
            {
                double roundValue = Math.Round(distances[i], 3);
                double truncateValue = Math.Truncate(roundValue);
                double fractionValue = roundValue - truncateValue;
                if (Math.Abs(fractionValue) > .0001)
                    for (double ii = .125; ii <= 1; ii += .125)
                    {
                        if (!(fractionValue <= ii))
                            continue;
                        double finalValue = truncateValue + ii;
                        distances[i] = finalValue;
                        break;
                    }
                else
                    distances[i] = roundValue;
            }

            return distances;
        }

        public override void execute()
        {
            throw new NotImplementedException();
        }

        //public override void execute()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
