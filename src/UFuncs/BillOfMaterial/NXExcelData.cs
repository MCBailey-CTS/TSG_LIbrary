using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using NXOpen.UF;

namespace TSG_Library.UFuncs
{
    internal class NXExcelData
    {
        public enum RowColumnIndexes
        {
            AtsStartRow = 14,
            CtsStartRow = AtsStartRow,
            DtsStartRow = AtsStartRow,
            EtsStartRow = AtsStartRow,
            HtsStartRow = AtsStartRow,
            RtsStartRow = AtsStartRow,
            UgsStartRow = AtsStartRow,
            NameColumn = 1,
            QtyColumn = 2,
            DescriptionColumn = 3,
            MaterialColumn = 4,
            WeightColumn = 8
        }
        // Data

        // ReSharper disable once InconsistentNaming
        private const string REPORT_TEXT = "verdana";

        // ReSharper disable once InconsistentNaming
        private const int REPORT_TEXT_SIZE = 10;

        // Constructors

        public NXExcelData()
        {
        }

        public NXExcelData(int rowIndex, int columnIndex, string data)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Data = data;
            ColorCell = false;
        }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public string Data { get; set; }

        public bool ColorCell { get; set; }

        // Methods

        public static void WriteData(_Worksheet worksheet, List<NXExcelData> data)
        {
            worksheet.Columns.HorizontalAlignment = Constants.xlCenter;
            worksheet.Columns.Font.Name = REPORT_TEXT;
            worksheet.Columns.Font.Size = REPORT_TEXT_SIZE;
            //excelApp.SetCell(checkerStockListPath, 1, rowIndex, colIndex, data.Data.ToUpper().Replace("-ALTER", ""));
            for (var index = 0; index < data.Count; index++)
            {
                var nxExcelData = data[index];
                UFSession.GetUFSession().Ui.SetPrompt($"Writing BOM sheet. Cell {index + 1} of {data.Count}.");
                worksheet.Cells[nxExcelData.RowIndex, nxExcelData.ColumnIndex] =
                    nxExcelData.Data.ToUpper().ToUpper().Replace("-ALTER", "");
            }
        }


        public static void Color(string[] strings, _Worksheet worksheet, IEnumerable<NXExcelData> data)
        {
            foreach (var excelData in data)
            {
                if (strings.All(str =>
                        !string.Equals(str, excelData.Data, StringComparison.CurrentCultureIgnoreCase))) continue;
                excelData.ColorCell = true;
                if (worksheet.Cells[excelData.RowIndex, excelData.ColumnIndex] is Range range)
                    range.Interior.Color = System.Drawing.Color.FromArgb(0, 255, 0);
            }
        }
    }
}