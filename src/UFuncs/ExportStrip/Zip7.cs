using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TSG_Library.Utilities
{
    public class Zip7
    {
        private readonly string[] _filesToZip;

        private readonly string _zipArchive;

        private Process _process;

        private bool _started;

        public Zip7(string zipArchive, params string[] filesToZip)
        {
            _zipArchive = zipArchive;

            _filesToZip = filesToZip;
        }

        public void WaitForExit()
        {
            if (_process is null)
                throw new ArgumentNullException(nameof(_process));

            if (!_started)
                throw new InvalidOperationException("Zip process hasn't been started yet.");

            _process.WaitForExit();
        }

        public void Start()
        {
            var tempFile = $"{Path.GetTempPath()}zipData{_filesToZip.GetHashCode()}.txt";

            using (var fs = File.Open(tempFile, FileMode.Create))
            {
                fs.Close();
            }

            using (var writer = new StreamWriter(tempFile))
            {
                _filesToZip.ToList().ForEach(writer.WriteLine);
            }

            _process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = false,
                    FileName = "C:\\Program Files\\7-Zip\\7z",
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    Arguments = $"a -t7z \"{_zipArchive}\" \"@{tempFile}\" -mx9"
                }
            };

            _started = _process.Start();
        }
    }
}