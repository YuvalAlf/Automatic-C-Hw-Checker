using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace AutomaticHwChecker
{
    public sealed class LogFile : IDisposable
    {
        private TextWriter TextWriter { get; }
        private int numberOfTabs { get; set; }

        private LogFile(TextWriter textWriter, int numberOfTabs)
        {
            TextWriter = textWriter;
            this.numberOfTabs = numberOfTabs;
        }

        public static LogFile CreateAt(string dirPath)
        {
            var path = Path.Combine(dirPath, "log.txt");
            var file = File.CreateText(path);
            file.AutoFlush = true;
            Process.Start(path);
            return new LogFile(file, 0);
        }

        public void WriteLine(string line) => this.TextWriter.WriteLine("\t".Repeat(numberOfTabs) + line);

        public void WriteLine() => this.TextWriter.WriteLine();

        public void Dispose()
        {
            TextWriter?.Dispose();
        }

        public void IncorporateTab()
        {
            numberOfTabs++;
        }

        public void UncorporateTab()
        {
            if (numberOfTabs > 0)
                numberOfTabs--;
        }

        public void WriteImportantLine(string line) => WriteLine("*****" + line + "*****");
    }
}