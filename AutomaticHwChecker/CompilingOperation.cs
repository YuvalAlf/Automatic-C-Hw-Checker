using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticHwChecker
{
    public static class CompilingOperation
    {
        public static string Compile(string cFilePath, LogFile logFile)
        {
            var exeName = "exeFile";
            var exeFilePath = Path.Combine(Path.GetDirectoryName(cFilePath), exeName + ".exe");

            //var arguements = $"{cFilePath} -pedantic -ansi -Wall -o {exeName}";
            var arguements = $"{Path.GetFileName(cFilePath)} -o {exeName}";
            var startInfo = new ProcessStartInfo("mingw32-gcc", arguements);
            startInfo.WorkingDirectory = Path.GetDirectoryName(cFilePath);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            using (var process = Process.Start(startInfo))
            {
                process.ErrorDataReceived += (sender, args) =>
                                             {
                                                 if (!string.IsNullOrEmpty(args.Data))
                                                    logFile.WriteLine("Error: " + args.Data + ". ");
                                             };
                process.BeginErrorReadLine();
                process.WaitForExit(3000);

                if (!File.Exists(exeFilePath))
                {
                    logFile.WriteImportantLine("Compilation Error. ");
                    return null;
                }

                return exeFilePath;
            }
            
        }
    }
}
