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
            try
            {
                var cFilePath = GetcFile(dirPath, logFile);
                if (cFilePath != null)
                {
                    var ids     = ExtractIds(cFilePath, logFile);
                    var exeFile = CompilingOperation.Compile(cFilePath, logFile);
                    var runningResults = CheckingOperation.RunTests(exeFile, inputOutputPath);
                    logFile.WriteLine("Ended checking");
                    var didCompile = (exeFile != null).AsString();

                    return new StudentsAnswers(cFilePath, didCompile, ids, runningResults);
                }

                return null;
            }
            finally
            {
                logFile.UncorporateTab();
            }
        }

        private static string GetcFile(string dirPath, LogFile logFile)
        {
            var cFiles = dirPath.GetAllInnerFiles().Where(f => f.EndsWith(".c")).ToArray();

            if (cFiles.Length == 0)
            {
                logFile.WriteLine("No c file found. ");
                return null;
            }

            if (cFiles.Length > 2)
            {
                logFile.WriteLine(cFiles.Length + " c files found. ");
                return null;
            }
            
            return cFiles.First();
        }

        private static bool IsId(string str, int index, int idMinLength)
        {
            if (index < 1)
                return false;

            if (str[index - 1].IsDigit())
                return false;

            for (int i = index; i < index + idMinLength && i < str.Length; i++)
                if (!str[i].IsDigit())
                    return false;
            return true;
        }

        private static string ExtractIdFromIndex(string str, int index)
        {
            var chs = new List<char>();
            while (index < str.Length && str[index].IsDigit())
            {
                chs.Add(str[index]);
                index++;
            }
            return new string(chs.ToArray());
        }

        private static string[] ExtractIds(string cFilePath, LogFile logFile)
        {
            var hwName = Path.GetFileNameWithoutExtension(cFilePath);
            var ids = new List<string>();
            var minIdLength = 5;
            for (int i = 0; i < hwName.Length; i++)
                if (IsId(hwName, i, minIdLength))
                    ids.Add(ExtractIdFromIndex(hwName, i));
            if (ids.Count != 2)
                logFile.WriteLine($"{ids.Count} ids found!");


            return ids.ToArray();
        }

        public static IEnumerable<string> ToCsvLines(StudentsAnswers[] studentsResults, InputOutput[] io)
        {
            string IdAsString(string[] ids) 
                => string.Join(",", ids.Select(id => id.PadZeroesLeft(9))) + (ids.Length == 1 ? "," : "");

            yield return "ID1,ID2,C File Path,Did Compile," + string.Join(",", io.Select(x => x.InputName)) + ",Comments,Grade";
            foreach (var answer in studentsResults)
                yield return IdAsString(answer.Ids) + $",\"{answer.cFilePath}\",\"{answer.DidCompile}\"," + string.Join(",", io.Select(x => answer.RunningResults[x]));
        }
    }
}
