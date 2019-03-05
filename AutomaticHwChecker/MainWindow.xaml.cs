using System;
using System.Collections.Generic;
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

namespace AutomaticHwChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string HomeworkSolutionsDirectoryPath => HomeworkSolutionsDirectoryTextbox.Text;
        private string InputOutputDirectoryPath => InputOutputDirectoryTextbox.Text;
        private string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public MainWindow()
        {
            InitializeComponent();
            this.HomeworkSolutionsDirectoryTextbox.TextChanged += (sender, args) => RunEnableCheck();
            this.InputOutputDirectoryTextbox.TextChanged += (sender, args) => RunEnableCheck();
            this.ChooseHomeworkDirectoryButton.Click += (sender, args) => ChooseHomeworkDirectory();
            this.ChooseInputOutputDirectoryButton.Click += (sender, args) => ChooseInputOutputDirectory();
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
    }
}
