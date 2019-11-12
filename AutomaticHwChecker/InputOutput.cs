using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomaticHwChecker
{
    public sealed class InputOutput
    {
        public string InputPath { get; }
        public string OutputPath { get; }
        public string InputName => Path.GetFileName(InputPath);
        public string OutputName => Path.GetFileName(OutputPath);

        public InputOutput(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public static InputOutput[] ParseOfDirectory(string inputOutputDirectoryPath)
        {
            var ios = new List<InputOutput>();
            var files = Directory.EnumerateFiles(inputOutputDirectoryPath).ToArray();
            for (int i = 1; true; i++)
            {
                var inputPath = Path.Combine(inputOutputDirectoryPath, "input" + i.ToString() + ".txt");
                var outputPath = Path.Combine(inputOutputDirectoryPath, "output" + i.ToString() + ".txt");
                if (!File.Exists(inputPath) || !File.Exists(outputPath))
                    break;
                ios.Add(new InputOutput(inputPath, outputPath)); 
            }

            return ios.ToArray();
        }

        public string AsString()
        {
            return $"Input: {InputName}, Output: {OutputName}";
        }
    }
}
