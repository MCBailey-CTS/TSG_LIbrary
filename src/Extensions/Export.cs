using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using TSG_Library.Extensions;
using static TSG_Library.Extensions.Extensions;

namespace TSG_Library.Extensions
{
    public static partial class Extensions
    {
        public static void StepSimDataBuild(string partPath, string stpPath)
        {
            try
            {
                if (!File.Exists(partPath))
                    throw new ArgumentException(@"Path to part does not exist.", nameof(partPath));

                if (File.Exists(stpPath))
                    throw new ArgumentException(@"Path for Step file already exists.", nameof(stpPath));

                if (File.Exists(stpPath))
                    throw new ArgumentException(@"Path for Step file already exists.", nameof(stpPath));

                StepCreator stepCreator1 = session_.DexManager.CreateStepCreator();

                using (session_.__UsingBuilderDestroyer(stepCreator1))
                {
                    stepCreator1.ExportAs = StepCreator.ExportAsOption.Ap214;
                    stepCreator1.ExportFrom = StepCreator.ExportFromOption.ExistingPart;
                    stepCreator1.ObjectTypes.Solids = true;
                    stepCreator1.InputFile = partPath;
                    stepCreator1.OutputFile = stpPath;
                    stepCreator1.ObjectTypes.Curves = true;
                    stepCreator1.ObjectTypes.Surfaces = true;
                    stepCreator1.ObjectTypes.Solids = true;
                    stepCreator1.FileSaveFlag = false;
                    stepCreator1.ProcessHoldFlag = true;
                    stepCreator1.LayerMask = "1-256";
                    stepCreator1.Commit();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException("Error when creating Step file for " + partPath);
            }
        }

        public static void IgesSimDataBuilder(string newPartFile, string igesPath)
        {
            try
            {
                IgesCreator igesCreator = Session.GetSession().DexManager.CreateIgesCreator();

                using (session_.__UsingBuilderDestroyer(igesCreator))
                {
                    igesCreator.ExportModelData = true;
                    igesCreator.ExportDrawings = true;
                    igesCreator.MapTabCylToBSurf = true;
                    igesCreator.BcurveTol = 0.050799999999999998;
                    igesCreator.IdenticalPointResolution = 0.001;
                    igesCreator.MaxThreeDMdlSpace = 10000.0;
                    igesCreator.ObjectTypes.Curves = true;
                    igesCreator.ObjectTypes.Surfaces = true;
                    igesCreator.ObjectTypes.Annotations = true;
                    igesCreator.ObjectTypes.Structures = true;
                    igesCreator.ObjectTypes.Solids = true;
                    igesCreator.ExportFrom = IgesCreator.ExportFromOption.ExistingPart;
                    igesCreator.SettingsFile = "C:\\Program Files\\Siemens\\NX 11.0\\iges\\igesexport.def";
                    igesCreator.MaxLineThickness = 2.0;
                    igesCreator.SysDefmaxThreeDMdlSpace = true;
                    igesCreator.SysDefidenticalPointResolution = true;
                    igesCreator.InputFile = newPartFile;
                    igesCreator.OutputFile = igesPath;
                    igesCreator.FileSaveFlag = false;
                    igesCreator.LayerMask = "1-256";
                    igesCreator.DrawingList = "";
                    igesCreator.ViewList = "Top,Front,Right,Back,Bottom,Left,Isometric,Trimetric,User Defined";
                    igesCreator.ProcessHoldFlag = false;
                    igesCreator.Commit();
                }
            }
            catch (Exception ex)
            {
                ex.__PrintException("Error when creating Iges file for " + igesPath);
            }
        }

    }
}
