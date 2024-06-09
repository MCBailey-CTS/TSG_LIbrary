using System;
using NXOpen;
using TSG_Library.Enum;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region FileNew

        //
        // Summary:
        //     Get the application name now required by NXOpen (since NX9) from the Snap enum
        //     value
        //
        // Parameters:
        //   fileNew:
        //     An NXOpen fileNew object
        //
        //   templateType:
        //     Template type
        //
        // Returns:
        //     Application name (template type name, really)
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        private static string __GetAppName(FileNew fileNew, Templates templateType)
        {
            var result = "GatewayTemplate";
            if(templateType == Templates.AeroSheetMetal) result = __SafeAppName(fileNew, "AeroSheetMetalTemplate");

            if(templateType == Templates.Assembly) result = __SafeAppName(fileNew, "AssemblyTemplate");

            if(templateType == Templates.Modeling) result = __SafeAppName(fileNew, "ModelTemplate");

            if(templateType == Templates.NXSheetMetal) result = __SafeAppName(fileNew, "NXSheetMetalTemplate");

            if(templateType == Templates.RoutingElectrical)
                result = __SafeAppName(fileNew, "RoutingElectricalTemplate");

            if(templateType == Templates.RoutingLogical) result = __SafeAppName(fileNew, "RoutingLogicalTemplate");

            if(templateType == Templates.RoutingMechanical)
                result = __SafeAppName(fileNew, "RoutingMechanicalTemplate");

            if(templateType == Templates.ShapeStudio) result = __SafeAppName(fileNew, "StudioTemplate");

            return result;
        }

        //
        // Summary:
        //     Get the names of the available template files
        //
        // Parameters:
        //   fileNew:
        //     An NXOpen fileNew object
        //
        //   templateType:
        //     Template type
        //
        //   unit:
        //     Part units
        //
        // Returns:
        //     The appropriate template file
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="templateType"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static string __GetTemplateFileName(FileNew fileNew, Templates templateType, Units unit)
        {
            var result = "Blank";
            if(unit == Units.MilliMeters)
            {
                if(templateType == Templates.AeroSheetMetal)
                    result = __SafeTemplateName(fileNew, "aero-sheet-metal-mm-template.prt");

                if(templateType == Templates.Assembly) result = __SafeTemplateName(fileNew, "assembly-mm-template.prt");

                if(templateType == Templates.Modeling)
                    result = __SafeTemplateName(fileNew, "model-plain-1-mm-template.prt");

                if(templateType == Templates.NXSheetMetal)
                    result = __SafeTemplateName(fileNew, "sheet-metal-mm-template.prt");

                if(templateType == Templates.RoutingElectrical)
                    result = __SafeTemplateName(fileNew, "routing-elec-mm-template.prt");

                if(templateType == Templates.RoutingLogical)
                    result = __SafeTemplateName(fileNew, "routing-logical-mm-template.prt");

                if(templateType == Templates.RoutingMechanical)
                    result = __SafeTemplateName(fileNew, "routing-mech-mm-template.prt");

                if(templateType == Templates.ShapeStudio)
                    result = __SafeTemplateName(fileNew, "shape-studio-mm-template.prt");
            }
            else
            {
                if(templateType == Templates.AeroSheetMetal)
                    result = __SafeTemplateName(fileNew, "aero-sheet-metal-inch-template.prt");

                if(templateType == Templates.Assembly)
                    result = __SafeTemplateName(fileNew, "assembly-inch-template.prt");

                if(templateType == Templates.Modeling)
                    result = __SafeTemplateName(fileNew, "model-plain-1-inch-template.prt");

                if(templateType == Templates.NXSheetMetal)
                    result = __SafeTemplateName(fileNew, "sheet-metal-inch-template.prt");

                if(templateType == Templates.RoutingElectrical)
                    result = __SafeTemplateName(fileNew, "routing-elec-inch-template.prt");

                if(templateType == Templates.RoutingLogical)
                    result = __SafeTemplateName(fileNew, "routing-logical-inch-template.prt");

                if(templateType == Templates.RoutingMechanical)
                    result = __SafeTemplateName(fileNew, "routing-mech-inch-template.prt");

                if(templateType == Templates.ShapeStudio)
                    result = __SafeTemplateName(fileNew, "shape-studio-inch-template.prt");
            }

            return result;
        }

        //
        // Summary:
        //     Check that an application name is OK
        //
        // Parameters:
        //   fileNew:
        //     A fileNew object
        //
        //   testName:
        //     The name to be validated
        //
        // Returns:
        //     The input name, if it's OK, otherwise "GatewayTemplate"
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private static string __SafeAppName(FileNew fileNew, string testName)
        {
            var applicationNames = fileNew.GetApplicationNames();
            var result = "GatewayTemplate";
            if(Array.IndexOf(applicationNames, testName) > -1) result = testName;

            return result;
        }

        //
        // Summary:
        //     Check that a template file name is OK
        //
        // Parameters:
        //   fileNew:
        //     A fileNew object
        //
        //   testName:
        //     The name to be validated
        //
        // Returns:
        //     The input name, if it's OK, otherwise "Blank"
        /// <summary>
        /// </summary>
        /// <param name="fileNew"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        private static string __SafeTemplateName(FileNew fileNew, string testName)
        {
            var availableTemplates = fileNew.GetAvailableTemplates();
            var result = "Blank";

            if(Array.IndexOf(availableTemplates, testName) > -1)
                result = testName;

            return result;
        }

        #endregion
    }
}