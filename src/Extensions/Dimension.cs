using System;
using NXOpen.Annotations;
using NXOpen.Drawings;

namespace TSG_Library.Extensions
{
    public static partial class Extensions_
    {
        public static bool _IsAssociative(this Dimension dim)
        {
            return !dim.IsRetained;
        }

        [Obsolete]
        public static DrawingSheet _OwningDrawingSheet(this Dimension dim)
        {
            throw new NotImplementedException();
            // ufsession_.View.AskViewDependentStatus(dim.Tag, out _, out string drawingSheetName);
            // return dim._OwningPart().DrawingSheets.ToArray().Single(__s=>__s.Name == drawingSheetName); 
            //dim._OwningPart().Dimensions.

            //dim.

            // ufsession_.View.AskViewDependentStatus(dim.Tag, out _, out string drawingSheetName);
            //print_(drawingSheetName);
            //foreach (NXOpen.Drawings.DrawingSheet s in dim._OwningPart().DrawingSheets)
            //    print_(s.Name);
            // return dim._OwningPart().DrawingSheets.ToArray().Single(__s => __s.Name.ToLower().Contains(drawingSheetName.ToLower()));
        }
    }
}