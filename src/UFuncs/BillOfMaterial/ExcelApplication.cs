using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace TSG_Library.Utilities
{
    public class ExcelApplication : IDisposable
    {
        private readonly Application _application;

        private readonly List<Range> _ranges;

        private readonly IDictionary<string, Workbook> _workBooks;

        private readonly List<_Worksheet> _workSheets;

        public ExcelApplication()
        {
            _application = new Application();
            _workBooks = new Dictionary<string, Workbook>();
            _workSheets = new List<_Worksheet>();
            _ranges = new List<Range>();
        }


        //NXOpen.NXObject


        public bool Visible
        {
            get => _application.Visible;
            set => _application.Visible = value;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            foreach (var range in _ranges)
                if(range != null)
                    Marshal.ReleaseComObject(range);

            foreach (var range in _workSheets)
                if(range != null)
                    Marshal.ReleaseComObject(range);

            foreach (var range in _workBooks)
                if(range.Value != null)
                {
                    range.Value.Close();
                    Marshal.ReleaseComObject(range.Value);
                }

            _application.Quit();
            Marshal.ReleaseComObject(_application);
        }

        public void SetCell(string excelFilePath, int sheetIndex, object rowIndex, object columnIndex, object data)
        {
            if(!_workBooks.ContainsKey(excelFilePath))
                _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

            var workSheet = (_Worksheet)_workBooks[excelFilePath].Sheets[sheetIndex];

            _workSheets.Add(workSheet);

            var range = workSheet.UsedRange;

            _ranges.Add(range);

            if(!(range[rowIndex, columnIndex] is Range tempRange))
                return;

            _ranges.Add(tempRange);

            tempRange.Value2 = data;
        }

        public void SetCell(string excelFilePath, int sheetIndex, object rowIndex, object columnIndex, Color color)
        {
            if(!_workBooks.ContainsKey(excelFilePath))
                _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

            var workSheet = (_Worksheet)_workBooks[excelFilePath].Sheets[sheetIndex];

            _workSheets.Add(workSheet);

            var range = workSheet.UsedRange;

            _ranges.Add(range);

            if(!(range[rowIndex, columnIndex] is Range tempRange))
                return;

            _ranges.Add(tempRange);

            tempRange.Interior.Color = color;
        }

        //public _Worksheet WorkBookActiveSheet1(string excelFilePath)
        //{
        //    if (!_workBooks.ContainsKey(excelFilePath))
        //        _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

        //    //_Worksheet workSheet = (_Worksheet)_workBooks[excelFilePath].Worksheets["Sheet1"];


        //    _workSheets.Add(workSheet);

        //    return workSheet;
        //}

        //public _Worksheet WorkBookActiveSheet(string excelFilePath)
        //{
        //    if (!_workBooks.ContainsKey(excelFilePath)) 
        //        _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

        //    _Worksheet workSheet = (_Worksheet)_workBooks[excelFilePath].ActiveSheet;

        //    _workSheets.Add(workSheet);

        //    return workSheet;
        //}

        public _Worksheet WorkBookActiveSheet(string excelFilePath)
        {
            if(!_workBooks.ContainsKey(excelFilePath))
                _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

            var workSheet = (_Worksheet)_workBooks[excelFilePath].ActiveSheet;

            _workSheets.Add(workSheet);

            return workSheet;
        }

        public void SaveWorkBook(string excelFilePath)
        {
            if(!_workBooks.ContainsKey(excelFilePath))
                throw new FileNotFoundException(
                    $"You cannot save excel sheet \"{excelFilePath}\" when you haven't opened it.");
            _workBooks[excelFilePath].Save();
        }

        internal _Worksheet WorkSheet(string excelFilePath, string name)
        {
            //if (!_workBooks.ContainsKey(excelFilePath))
            //    _workBooks.Add(excelFilePath, _application.Workbooks.Open(excelFilePath));

            return (_Worksheet)_workBooks[excelFilePath].Worksheets[name];

            //_workSheets.Add(workSheet);

            //return workSheet;
        }
    }
}