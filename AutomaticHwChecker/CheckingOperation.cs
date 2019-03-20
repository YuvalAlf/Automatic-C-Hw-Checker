using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticHwChecker
{
    public static class CheckingOperation
    {
        public static StudentsAnswers[] CheckHw(string unzippedDirectoryLocation, InputOutput[] inputOutputPairs, LogFile logFile) 
            => Directory.EnumerateDirectories(unzippedDirectoryLocation)
                        .Select(dirPath => StudentsAnswers.OfDirPath(dirPath, inputOutputPairs, logFile))
                        .ToArray();

        public static Dictionary<InputOutput, string> RunTests(string exeFile, InputOutput[] inputOutputPath)
        {
            var results = new Dictionary<InputOutput, string>();

            foreach (var inputOutput in inputOutputPath)
                results[inputOutput] = Utils.TryRunFor(500, () => Run(exeFile, inputOutput.InputPath, inputOutput.OutputPath));

            return results;
        }

        private static bool Run(string exeFile, string inputPath, string outputPath)
        {
            if (exeFile == null)
                return false;
            var processInfo = new ProcessStartInfo(exeFile);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            using (var process = Process.Start(processInfo))
            using (var inputFile = File.OpenText(inputPath))
            using (var outputFile = File.CreateText(Path.Combine(Path.GetDirectoryName(exeFile), Path.GetFileName(outputPath))))
            {
                process.StandardInput.Write(inputFile.ReadToEnd());
                process.StandardInput.Flush();
                process.StandardInput.Close();
                var result = process.StandardOutput.ReadToEnd();
                outputFile.Write(result);
                var expectedResult = File.ReadAllText(outputPath);
                return expectedResult.CleanSpaces().Equals(result.CleanSpaces());
            }
        }

        private static string CleanSpaces(this string @this) => @this.ToLower().Replace(" ", "").Replace("\n", "").Replace("\r", "");
    }
}
