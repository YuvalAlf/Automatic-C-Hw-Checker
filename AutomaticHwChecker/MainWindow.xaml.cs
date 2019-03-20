using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using MoreLinq.Extensions;
using Path = System.IO.Path;

namespace AutomaticHwChecker
{
    public partial class MainWindow : Window
    {
        private string HomeworkSolutionsDirectoryPath => HomeworkSolutionsDirectoryTextbox.Text;
        private string InputOutputDirectoryPath => InputOutputDirectoryTextbox.Text;
        private string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        private string CsvResultPath => Path.Combine(DesktopPath, "HwResults.csv");

        public MainWindow()
        {
            InitializeComponent();
            this.HomeworkSolutionsDirectoryTextbox.TextChanged += (sender, args) => RunEnableCheck();
            this.InputOutputDirectoryTextbox.TextChanged += (sender, args) => RunEnableCheck();
            this.ChooseHomeworkDirectoryButton.Click += (sender, args) => ChooseHomeworkDirectory();
            this.ChooseInputOutputDirectoryButton.Click += (sender, args) => ChooseInputOutputDirectory();

            this.HomeworkSolutionsDirectoryTextbox.Text = @"C:\Users\Yuval\Google Drive\Introduction To Computers\Hws\Hw1\הגשות במודל";
            this.InputOutputDirectoryTextbox.Text = @"C:\Users\Yuval\Google Drive\Introduction To Computers\Hws\Hw1\IO";
        }

        private void ChooseHomeworkDirectory()
        {
            if (GetDirectoryFromUser(out string directory, "Homework Directory"))
                HomeworkSolutionsDirectoryTextbox.Text = directory;
        }

        private void ChooseInputOutputDirectory()
        {
            if (GetDirectoryFromUser(out string directory, "Input-Output Directory"))
                InputOutputDirectoryTextbox.Text = directory;
        }

        private bool GetDirectoryFromUser(out string directoryPath, string title)
        {
            directoryPath = null;
            var dlg = new CommonOpenFileDialog();
            dlg.Title            = title;
            dlg.IsFolderPicker   = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems   = false;
            dlg.EnsureFileExists          = true;
            dlg.EnsurePathExists          = true;
            dlg.EnsureReadOnly            = false;
            dlg.EnsureValidNames          = true;
            dlg.Multiselect               = false;
            dlg.ShowPlacesList            = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            { 
                directoryPath = dlg.FileName;
                return true;
            }

            return false;
        }


        private void RunEnableCheck()
        {
            RunButton.IsEnabled = Directory.Exists(HomeworkSolutionsDirectoryPath) && Directory.Exists(InputOutputDirectoryPath);
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var logFile = LogFile.CreateAt(DesktopPath))
            {
                logFile.WriteLine("Automatic Hw Checker, " + DateTime.Now.ToLongDateString());

                logFile.WriteLine("");

                logFile.WriteLine("Unzipping and checking files:");
                logFile.IncorporateTab();
                var unzippedDirectoryLocation = ZippingOperation.UnzipFilesTo(HomeworkSolutionsDirectoryPath, DesktopPath, logFile);
                logFile.UncorporateTab();
                logFile.WriteLine("Finnished zipping files to: " + unzippedDirectoryLocation);

                logFile.WriteLine("");

                logFile.WriteLine("Input-Output Files:");
                logFile.IncorporateTab();
                logFile.WriteLine("Directory: " + InputOutputDirectoryPath);
                var inputOutputPairs = InputOutput.ParseOfDirectory(InputOutputDirectoryPath);
                inputOutputPairs.Select(x => x.AsString()).ForEach(logFile.WriteLine);
                logFile.UncorporateTab();
                
                logFile.WriteLine("");

                logFile.WriteLine("Checking Files:");
                logFile.IncorporateTab();
                var studentsResults = CheckingOperation.CheckHw(unzippedDirectoryLocation, inputOutputPairs, logFile);
                logFile.UncorporateTab();


                logFile.WriteLine("Publishing CSV result at: " + CsvResultPath);
                File.WriteAllLines(CsvResultPath, StudentsAnswers.ToCsvLines(studentsResults, inputOutputPairs));
                Process.Start(CsvResultPath);
                logFile.WriteLine("For future use --- Lie Detector:");
                logFile.IncorporateTab();

                logFile.UncorporateTab();


                logFile.WriteLine("");
                logFile.WriteLine("Finished Successfully");
            }
        }
    }
}
