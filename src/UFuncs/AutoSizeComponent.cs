using System;
using System.Linq;
using NXOpen;
using NXOpen.Features;
using NXOpen.UserDefinedObjects;
using NXOpen.Utilities;
using TSG_Library.UFuncs;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Attributes
{
    [UFunc("auto-size-component")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("1.2", "2018", "06", "11")]
    [Revision("1.2.1",
        "When updating the “Description” attribute, if the part in question contains an expression named “lwrParallel” or “uprParallel” and its’ value == “yes” then we simply just add the text “PARALLEL” to the end of the “DESCRIPTION” attribute.")]
    [Revision("1.2.2", "Per Tsg-Cit #2018-0198")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
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
        private static int initializeUDO(bool alertUser)
        {
            try
            {
                if(!(_myUdoClass is null))
                    return 0;

                if(alertUser)
                    UI.GetUI().NXMessageBox
                        .Show("UDO", NXMessageBox.DialogType.Information, "Registering C# UDO Class");

                // Define your custom UDO class 
                _myUdoClass =
                    session_.UserDefinedClassManager.CreateUserDefinedObjectClass("UdoAutoSizeComponent",
                        "Update Order Size");
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
                if(updateEvent.AssociatedObject is null)
                    return 0;

                var assocObjStatus = TheUFSession.Obj.AskStatus(updateEvent.AssociatedObject.Tag);
                var solidBodyTag = updateEvent.AssociatedObject.Tag;
                var udoBody = (Body)NXObjectManager.Get(solidBodyTag);
                var updatePart = (Part)udoBody.OwningPart;

                if(assocObjStatus != 3)
                    return 0;

                var updateFlag = updateEvent.UserDefinedObject.GetIntegers();

                if(updateFlag[0] == 1)
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
                var myUdoClass =
                    session_.UserDefinedClassManager.GetUserDefinedClassFromClassName("UdoAutoSizeComponent");

                if(myUdoClass is null)
                    return retValue;

                var currentUdo = __work_part_.UserDefinedObjectManager.GetUdosOfClass(myUdoClass);

                if(currentUdo.Length != 0)
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
                var isMetric = false;
                BasePart basePart = updatePartSize;

                if(basePart.PartUnits == BasePart.Units.Millimeters)
                    isMetric = true;

                foreach (var featDynamic in __work_part_.Features.ToArray())
                {
                    if(featDynamic.FeatureType != "BLOCK") continue;
                    if(featDynamic.Name != "DYNAMIC BLOCK") continue;
                    var block1 = (Block)featDynamic;
                    var sizeBody = block1.GetBodies();

                    var blockFeatureBuilderSize = __work_part_.Features.CreateBlockFeatureBuilder(block1);

                    blockFeatureBuilderSize.GetOrientation(out var xAxis, out var yAxis);

                    double[] initOrigin =
                    {
                        blockFeatureBuilderSize.Origin.X, blockFeatureBuilderSize.Origin.Y,
                        blockFeatureBuilderSize.Origin.Z
                    };
                    double[] xVector = { xAxis.X, xAxis.Y, xAxis.Z };
                    double[] yVector = { yAxis.X, yAxis.Y, yAxis.Z };
                    var initMatrix = new double[9];
                    TheUFSession.Mtx3.Initialize(xVector, yVector, initMatrix);
                    TheUFSession.Csys.CreateMatrix(initMatrix, out var tempMatrix);
                    TheUFSession.Csys.CreateTempCsys(initOrigin, tempMatrix, out var tempCsys);

                    if(tempCsys != Tag.Null)
                    {
                        // get named expressions

                        var isNamedExpression = false;

                        double xValue = 0,
                            yValue = 0,
                            zValue = 0;

                        var burnDirValue = string.Empty;
                        var burnoutValue = string.Empty;
                        var grindValue = string.Empty;
                        var grindTolValue = string.Empty;
                        var diesetValue = string.Empty;

                        foreach (var exp in __work_part_.Expressions.ToArray())
                        {
                            // ReSharper disable once ConvertIfStatementToSwitchStatement
                            if(exp.Name == "AddX")
                            {
                                isNamedExpression = true;
                                xValue = exp.Value;
                            }

                            if(exp.Name == "AddY")
                            {
                                isNamedExpression = true;
                                yValue = exp.Value;
                            }

                            if(exp.Name == "AddZ")
                            {
                                isNamedExpression = true;
                                zValue = exp.Value;
                            }

                            if(exp.Name == "BurnDir")
                            {
                                isNamedExpression = true;
                                burnDirValue = exp.RightHandSide;
                            }

                            if(exp.Name == "Burnout")
                            {
                                isNamedExpression = true;
                                burnoutValue = exp.RightHandSide;
                            }

                            if(exp.Name == "Grind")
                            {
                                isNamedExpression = true;
                                grindValue = exp.RightHandSide;
                            }

                            if(exp.Name == "GrindTolerance")
                            {
                                isNamedExpression = true;
                                grindTolValue = exp.RightHandSide;
                            }

                            if(exp.Name == "DiesetNote") diesetValue = exp.RightHandSide;
                        }

                        burnDirValue = burnDirValue.Replace("\"", string.Empty);
                        burnoutValue = burnoutValue.Replace("\"", string.Empty);
                        grindValue = grindValue.Replace("\"", string.Empty);
                        grindTolValue = grindTolValue.Replace("\"", string.Empty);
                        diesetValue = diesetValue.Replace("\"", string.Empty);

                        if(isNamedExpression)
                        {
                            // get bounding box of solid body

                            var minCorner = new double[3];
                            var directions = new double[3, 3];
                            var distances = new double[3];
                            var grindDistances = new double[3];

                            TheUFSession.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                distances);
                            TheUFSession.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                grindDistances);

                            // add stock values

                            distances[0] += xValue;
                            distances[1] += yValue;
                            distances[2] += zValue;

                            if(isMetric)
                                for (var i = 0; i < distances.Length; i++)
                                    distances[i] /= 25.4d;

                            if(burnoutValue.ToLower() == "no")
                                for (var i = 0; i < 3; i++)
                                {
                                    var roundValue = Math.Round(distances[i], 3);
                                    var truncateValue = Math.Truncate(roundValue);
                                    var fractionValue = roundValue - truncateValue;
                                    if(Math.Abs(fractionValue) > .0001)
                                        for (var ii = .125; ii <= 1; ii += .125)
                                        {
                                            if(!(fractionValue <= ii)) continue;
                                            var roundedFraction = ii;
                                            var finalValue = truncateValue + roundedFraction;
                                            distances[i] = finalValue;
                                            break;
                                        }
                                    else
                                        distances[i] = roundValue;
                                }

                            var xDist = distances[0];
                            var yDist = distances[1];
                            var zDist = distances[2];

                            var xGrindDist = grindDistances[0];
                            var yGrindDist = grindDistances[1];
                            var zGrindDist = grindDistances[2];

                            Array.Sort(distances);
                            Array.Sort(grindDistances);

                            // ReSharper disable once ConvertIfStatementToSwitchStatement
                            if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "no")
                            {
                                updatePartSize.SetUserAttribute("DESCRIPTION", -1,
                                    $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}", Update.Option.Now);
                            }
                            else if(burnoutValue.ToLower() == "no" && grindValue.ToLower() == "yes")
                            {
                                // ReSharper disable once ConvertIfStatementToSwitchStatement
                                if(burnDirValue.ToLower() == "x")
                                {
                                    if(Math.Abs(xGrindDist - grindDistances[0]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{grindDistances[0]:f3} {grindTolValue} X {distances[1]:f2} X {distances[2]:f2}",
                                            Update.Option.Now);

                                    if(Math.Abs(xGrindDist - grindDistances[1]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2} X {grindDistances[1]:f3} {grindTolValue} X {distances[2]:f2}",
                                            Update.Option.Now);

                                    if(Math.Abs(xGrindDist - grindDistances[2]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2} X {distances[1]:f2} X {grindDistances[2]:f3} {grindTolValue}",
                                            Update.Option.Now);
                                }

                                if(burnDirValue.ToLower() == "y")
                                {
                                    if(Math.Abs(yGrindDist - grindDistances[0]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{grindDistances[0]:f3}" + " " + grindTolValue + " X " +
                                            $"{distances[1]:f2}" + " X " + $"{distances[2]:f2}", Update.Option.Now);

                                    if(Math.Abs(yGrindDist - grindDistances[1]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2}" + " X " + $"{grindDistances[1]:f3}" + " " +
                                            grindTolValue + " X " + $"{distances[2]:f2}", Update.Option.Now);

                                    if(Math.Abs(yGrindDist - grindDistances[2]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2}" + " X " + $"{distances[1]:f2}" + " X " +
                                            $"{grindDistances[2]:f3}" + " " + grindTolValue, Update.Option.Now);
                                }

                                if(burnDirValue.ToLower() == "z")
                                {
                                    if(Math.Abs(zGrindDist - grindDistances[0]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{grindDistances[0]:f3}" + " " + grindTolValue + " X " +
                                            $"{distances[1]:f2}" + " X " + $"{distances[2]:f2}", Update.Option.Now);

                                    if(Math.Abs(zGrindDist - grindDistances[1]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2}" + " X " + $"{grindDistances[1]:f3}" + " " +
                                            grindTolValue + " X " + $"{distances[2]:f2}", Update.Option.Now);

                                    if(Math.Abs(zGrindDist - grindDistances[2]) < Tolerance)
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            $"{distances[0]:f2}" + " X " + $"{distances[1]:f2}" + " X " +
                                            $"{grindDistances[2]:f3}" + " " + grindTolValue, Update.Option.Now);
                                }
                            }
                            else
                            {
                                if(grindValue.ToLower() == "yes")
                                {
                                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                                    if(burnDirValue.ToLower() == "x")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            "BURN " + $"{xGrindDist:f3}" + " " + grindTolValue, Update.Option.Now);

                                    if(burnDirValue.ToLower() == "y")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            "BURN " + $"{yGrindDist:f3}" + " " + grindTolValue, Update.Option.Now);

                                    if(burnDirValue.ToLower() == "z")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1,
                                            "BURN " + $"{zGrindDist:f3}" + " " + grindTolValue, Update.Option.Now);
                                }
                                else
                                {
                                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                                    if(burnDirValue.ToLower() == "x")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1, "BURN " + $"{xDist:f2}",
                                            Update.Option.Now);

                                    if(burnDirValue.ToLower() == "y")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1, "BURN " + $"{yDist:f2}",
                                            Update.Option.Now);

                                    if(burnDirValue.ToLower() == "z")
                                        __work_part_.SetUserAttribute("DESCRIPTION", -1, "BURN " + $"{zDist:f2}",
                                            Update.Option.Now);
                                }
                            }

                            if(diesetValue != "yes") continue;
                            var description = updatePartSize.GetStringUserAttribute("DESCRIPTION", -1);

                            if(description.ToLower().Contains("dieset")) continue;
                            description += " DIESET";
                            updatePartSize.SetUserAttribute("DESCRIPTION", -1, description, Update.Option.Now);
                        }
                        else
                        {
                            // get bounding box of solid body

                            var minCorner = new double[3];
                            var directions = new double[3, 3];
                            var distances = new double[3];

                            TheUFSession.Modl.AskBoundingBoxExact(sizeBody[0].Tag, tempCsys, minCorner, directions,
                                distances);

                            if(isMetric)
                                for (var i = 0; i < distances.Length; i++)
                                    distances[i] /= 25.4d;

                            for (var i = 0; i < 3; i++)
                            {
                                var roundValue = Math.Round(distances[i], 3);
                                var truncateValue = Math.Truncate(roundValue);
                                var fractionValue = roundValue - truncateValue;
                                if(Math.Abs(fractionValue) > .0001)
                                    for (var ii = .125; ii <= 1; ii += .125)
                                    {
                                        if(!(fractionValue <= ii)) continue;
                                        var roundedFraction = ii;
                                        var finalValue = truncateValue + roundedFraction;
                                        distances[i] = finalValue;
                                        break;
                                    }
                                else
                                    distances[i] = roundValue;
                            }
                            // CreateOrNull the description attribute

                            Array.Sort(distances);

                            updatePartSize.SetUserAttribute("DESCRIPTION", -1,
                                $"{distances[0]:f2} X {distances[1]:f2} X {distances[2]:f2}", Update.Option.Now);

                            if(diesetValue != "yes") continue;
                            var description = updatePartSize.GetStringUserAttribute("DESCRIPTION", -1);

                            if(description.ToLower().Contains("dieset")) continue;
                            description += " DIESET";
                            updatePartSize.SetUserAttribute("DESCRIPTION", -1, description, Update.Option.Now);
                        }
                    }
                    else
                    {
                        UI.GetUI().NXMessageBox.Show("Auto Size Update Error", NXMessageBox.DialogType.Error,
                            "Description update failed " + updatePartSize.FullPath);
                    }
                }


                // If the work part does not have a {"DESCRIPTION"} attribute then we want to return;.
                if(!updatePartSize.HasUserAttribute("DESCRIPTION", NXObject.AttributeType.String, -1)) return;

                // The string value of the {"DESCRIPTION"} attribute.
                var descriptionAtt =
                    updatePartSize.GetUserAttributeAsString("DESCRIPTION", NXObject.AttributeType.String, -1);

                var expressions = updatePartSize.Expressions.ToArray();

                // Checks to see if the {_workPart} contains an expression with value {"yes"} and name of {lwrParallel} or {uprParallel}.
                if(expressions.Any(exp =>
                       (exp.Name.ToLower() == "lwrparallel" || exp.Name.ToLower() == "uprparallel") &&
                       exp.StringValue.ToLower() == "yes"))
                    // Appends {"Parallel"} to the end of the {"DESCRIPTION"} attribute string value and then sets the it to be the value of the {"DESCRIPTION"} attribute.
                    updatePartSize.SetUserAttribute("DESCRIPTION", -1, descriptionAtt + " PARALLEL", Update.Option.Now);
            }
            catch (Exception ex)
            {
                ex.__PrintException();
            }
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