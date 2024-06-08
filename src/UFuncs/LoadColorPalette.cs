using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.UF;
using TSG_Library.Attributes;
using static TSG_Library.Extensions;

namespace TSG_Library.UFuncs
{
    [UFunc(ufunc_load_color_palette)]
    [RevisionEntry("1.0", "2023", "02", "23")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    public class LoadColorPalette : _UFunc
    {
        public override void execute()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = @"U:\nxFiles\UfuncFiles",
                Filter = "cdf files (*.cdf)|*.cdf|All files (*.*)|*.*"
            };

            var result = dialog.ShowDialog();

            if(result != DialogResult.OK)
                return;


            //print_(result);

            //switch (result)
            //{
            //    case System.Windows.Forms.DialogResult.None:
            //        break;
            //    case System.Windows.Forms.DialogResult.OK:
            //        break;
            //    case System.Windows.Forms.DialogResult.Cancel:
            //        break;
            //    case System.Windows.Forms.DialogResult.Abort:
            //        break;
            //    case System.Windows.Forms.DialogResult.Retry:
            //        break;
            //    case System.Windows.Forms.DialogResult.Ignore:
            //        break;
            //    case System.Windows.Forms.DialogResult.Yes:
            //        break;
            //    case System.Windows.Forms.DialogResult.No:
            //        break;
            //}


            ProcessAssy(dialog.FileName);
        }


        public static void ProcessAssy(string cdfFile)
        {
            var displayPart = session_.Parts.Display;

            try
            {
                foreach (var child in __display_part_.ComponentAssembly.RootComponent.GetChildren()
                             .Where(__c => __c.__IsLoaded()).Select(__c => __c._Prototype()).Distinct())
                    try
                    {
                        __display_part_ = child;

                        ProcessPart(__display_part_, cdfFile);
                    }
                    catch (Exception ex)
                    {
                        ex._PrintException();
                    }

                //ComponentAssembly c = displayPart.ComponentAssembly;
                //ProcessPart(displayPart, cdfFile);
                //if (c.RootComponent != null)
                //{
                //    ProcessChildren(c.RootComponent, 0, cdfFile);
                //}
            }
            catch (Exception ex)
            {
                ex._PrintException();
                //session_.ListingWindow.WriteLine("Failed: " & e.ToString);
            }
            //session_.ListingWindow.Close();

            // reset the display part
            PartCollection.SdpsStatus status1;
            status1 = session_.Parts.SetDisplay(displayPart, false, true, out var partLoadStatus1);
            partLoadStatus1.Dispose();
        }

        //----------------------------------------------------------------------
        // This does not do the component loading
        public static void ProcessChildren(Component comp, int indent, string cdfFile)
        {
            foreach (var child in comp.GetChildren())
            {
                print_(child.DisplayName);
                // insert code to process component or subassembly
                if(true /*LoadComponent(child)*/)
                    //session_.ListingWindow.WriteLine("old component name: " + child.Name);
                    //session_.ListingWindow.WriteLine("file name: " + child.Prototype.OwningPart.Leaf);
                    // Get the Part() object
                    try
                    {
                        var aPart = (Part)child.Prototype;
                        ProcessPart(aPart, cdfFile);
                    }
                    catch (Exception ex)
                    {
                        ex._PrintException();
                    }

                // child.SetName(child.Prototype.OwningPart.Leaf.ToUpper);
                //lw.WriteLine("new component name: " & child.Name);
                //lw.WriteLine("");
                //component could not be loaded
                // end of code to process component or subassembly
                ProcessChildren(child, indent + 1, cdfFile);
            }
        }

        //----------------------------------------------------------------------
        // We know that they are Assemblies.Component objects, from our filter
        private static void ProcessPart(Part aPart, string cdfFile)
        {
            //print_(aPart.Leaf);
            // HandleSelectionColor can only be called on DisplayedPart
            // so change
            _ = session_.Parts.SetDisplay(aPart, true, true, out var partLoadStatus1);
            partLoadStatus1.Dispose();

            // change the selection colours
            aPart.Preferences.ColorSettingVisualization.SelectionColor = 119;
            aPart.Preferences.ColorSettingVisualization.PreselectionColor = 149;

            //        string rootDir = Environment
            //.GetEnvironmentVariable("UGII_ROOT_DIR");
            var colorPalette = cdfFile;
            var colorName = "";
            var red = "";
            var green = "";
            var blue = "";
            var rgbColor = new double[3];
            var thisColor = 0;
            var textLine = "";
            var lineCounter = 0;

            //VB FileSystem.FileOpen(1, colorPalette, OpenMode.Input, -1, -1, -1);
            var file = new StreamReader(colorPalette);
            //VB while (!(FileSystem.EOF(1)))
            while ((textLine = file.ReadLine()) != null)
            {
                //VB textLine = FileSystem.LineInput(1);
                if(lineCounter > 3)
                {
                    colorName = textLine.Substring(0, 30);
                    red = textLine.Substring(34, 8);
                    green = textLine.Substring(45, 8);
                    blue = textLine.Substring(56, 8);
                    rgbColor[0] = Convert.ToDouble(red);
                    rgbColor[1] = Convert.ToDouble(green);
                    rgbColor[2] = Convert.ToDouble(blue);
                    thisColor = lineCounter - 4;
                    TheUFSession.Disp.SetColor(thisColor, UFConstants.UF_DISP_rgb_model, colorName, rgbColor);
                }

                lineCounter++;
            }

            //VB FileSystem.FileClose(1);
            file.Close();

            TheUFSession.Disp.LoadColorTable();
        }
    }
}