using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticHwChecker
{
    public sealed class StudentsAnswers
    {
        public string cFilePath { get; }
        public string DidCompile { get; }
        public string[] Ids { get; }
        public Dictionary<InputOutput, string> RunningResults { get; }

        public StudentsAnswers(string cFilePath, string didCompile, string[] ids, Dictionary<InputOutput, string> runningResults)
        {
            this.cFilePath = cFilePath;
            DidCompile = didCompile;
            Ids = ids;
            RunningResults = runningResults;
        }

        public static StudentsAnswers OfDirPath(string dirPath, InputOutput[] inputOutputPath, LogFile logFile)
        {
            logFile.WriteLine("Checking " + dirPath);
            logFile.IncorporateTab();
            var problems = new StringBuilder();
            var ids = GetIds(dirPath, logFile);
            var cFilePath = GetcFile(dirPath, problems);
            var exeFile = CompilingOperation.Compile(cFilePath, problems);
            var runningResults = CheckingOperation.RunTests(exeFile, inputOutputPath);
            if (problems.Length > 4)
                logFile.WriteImportantLine(problems.ToString());
            logFile.UncorporateTab();
            logFile.WriteLine("Ended checking " + dirPath);

            var didCompile = (exeFile != null).AsString();

            return new StudentsAnswers(cFilePath, didCompile, ids, runningResults);
        }

        private static string GetcFile(string dirPath, StringBuilder problems)
        {
            var cFiles = dirPath.GetAllInnerFiles().Where(f => f.EndsWith(".c")).ToArray();

            if (cFiles.Length == 0)
            {
                problems.Append("No c file found. ");
                return null;
            }
                
            if (cFiles.Length > 2)
                problems.Append(cFiles.Length + " c files found. ");
            
            return cFiles.First();
        }

        private static string[] GetIds(string dirPath, LogFile logFile)
        {
            var fileName = Path.GetFileName(dirPath);
            var ids = fileName.Split("_.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            ids = ids.Where(s => s.All(char.IsDigit)).ToArray();

            if (ids.Length != 2)
                logFile.WriteImportantLine(ids.Length + " IDs recognized at " + dirPath);
            if (ids.Any(id => id.Length != 8 && id.Length != 9))
                logFile.WriteImportantLine(" Strange ID at " + dirPath);
            return ids;
        }

        public static IEnumerable<string> ToCsvLines(StudentsAnswers[] studentsResults, InputOutput[] io)
        {
            string IdAsString(string[] ids)
            {
                return string.Join(",", ids.Select(id => id.PadZeroesLeft(9))) + (ids.Length == 1 ? "," : "");
            }

            yield return "ID1,ID2,C File Path,Did Compile," + string.Join(",", io.Select(x => x.InputName)) + ",Comments,Grade";
            foreach (var answer in studentsResults)
                yield return IdAsString(answer.Ids) + $",\"{answer.cFilePath}\",\"{answer.DidCompile}\"," + string.Join(",", io.Select(x => answer.RunningResults[x]));
        }
    }
}
