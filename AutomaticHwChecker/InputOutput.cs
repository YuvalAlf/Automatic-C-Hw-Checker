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
            var outputRegex = new Regex($".*out[^/].[^/]");
            var inputRegex = new Regex($".*in[^/].[^/]");
            InputOutput ofDirectory(string directory)
            {
                var innerFiles = Directory.EnumerateFiles(directory).ToArray();
                var outputPath = innerFiles.First(outputRegex.IsMatch);
                var inputPath  = innerFiles.First(inputRegex.IsMatch);
                return new InputOutput(inputPath, outputPath);
            }

            return Directory.EnumerateDirectories(inputOutputDirectoryPath).Select(ofDirectory).ToArray();
        }

        public string AsString()
        {
            return $"Input: {InputName}, Output: {OutputName}";
        }
    }
}
