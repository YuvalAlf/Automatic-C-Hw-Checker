using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutomaticHwChecker
{
    public static class ZippingOperation
    {
        private static readonly Regex FormatRegex = new Regex(@"[^_]+_[\d]+(_[\d]+|_)?.zip");

        private static bool IsInFormat(string zipFileName) => FormatRegex.IsMatch(zipFileName);


        public static string UnzipFilesTo(string homeworkPath, string resultDirectoryPath, LogFile logFile)
        {
            var unzippedDirectoryPath = Path.Combine(resultDirectoryPath, "Results");
            var didDelete = unzippedDirectoryPath.RecreateDirectory();
            var init = didDelete ? "Rec" : "C";
            logFile.WriteLine(init + "reated unzipped homework directory at: " + unzippedDirectoryPath);
            
            foreach (var innerFile in homeworkPath.GetAllInnerFiles().OrderBy(IsInFormat))
            {
                if (IsInFormat(innerFile))
                    UnzipFileTo(innerFile, unzippedDirectoryPath, logFile);
                else
                    logFile.WriteLine("Faulty file at: " + Path.GetDirectoryName(innerFile));
            }

            return unzippedDirectoryPath;
        }



        private static void UnzipFileTo(string filePath, string unzippedDirectoryPath, LogFile logFile)
        {
            var dirPath = Path.Combine(unzippedDirectoryPath, Path.GetFileNameWithoutExtension(filePath));
            if (Directory.Exists(dirPath))
            {
                logFile.WriteLine("Duplicate Submissions: " + Path.GetDirectoryName(filePath));
                return;
            }
            Directory.CreateDirectory(dirPath);
            ZipFile.ExtractToDirectory(filePath, dirPath);
            logFile.WriteLine("Unzipped path: " + dirPath);
        }

    }
}
