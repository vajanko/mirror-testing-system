using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

using MTS.Controls;
using MTS.Editor;
using MTS.Admin;
using MTS.Data;
using MTS.TesterModule;

using AvalonDock;

namespace MTS
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        #region Events

        #region Menu Events

        private void menuClick_Error(object sender, RoutedEventArgs e)
        {   // for debugging
            MTS.Utils.ErrorWindow wnd = new MTS.Utils.ErrorWindow();
            wnd.Title = MTS.Utils.Errors.FileErrorTitle;
            wnd.Message = "Error message";
            wnd.ErrorIcon = MTS.Utils.Errors.FileErrorIcon;
            wnd.ShowDialog();
        }
        private void menuClick_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

        #region Commands

        // new
        private void newCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // check if testing is running otherwise ..

            e.CanExecute = true;
            e.Handled = true;
        }
        private void newExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // create new tab
            TestFile file = new TestFile();
            // fill it with new test collection
            file.New();
            // add to the main pane
            filePane.Items.Add(file);
            // activate it - show to user
            filePane.SelectedItem = file;
        }
        // open
        private void openCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // check if testing is running otherwise ..
            
            e.CanExecute = true;
            e.Handled = true;
        }
        private void openExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // at this time tab cotaning the opening document does not exist so window must handle this command
            // create a configured open file dialog
            var dialog = FileManager.CreateOpenFileDialog();

            // file entered - create new tab
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;  // path to open
                TestFile file;

                // check each content in the main pane (file pane) if document is not opened yet
                foreach (DocumentContent content in filePane.Items)
                {
                    file = (content as TestFile);   // only chceck TestFile
                    if (file != null && file.FilePath == filename)
                    {   // file is already opened - do not open it again, but show it to the user
                        file.Activate();
                        return;
                    }
                }

                // file is not opened yet - new tab will be created
                try
                {
                    file = new TestFile();          // create new tab
                    file.Open(dialog.FileName);     // read content to it
                    filePane.Items.Add(file);       // add to dockable pane
                    file.Activate();                // show it to user
                }
                catch (Exception ex)
                {   // some error occured while opening the file
                    FileManager.HandleOpenException(ex);    // error could depend on the file format - FileManager
                                                            // should know
                }
                e.Handled = true;   // opening file has finished
            }
        }
        // close
        private void closeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // could be for different document contents (not just TestFile)
            if (filePane == null) return;   // when filePane does not exist no document could be closed
            DocumentContent file = filePane.SelectedItem as DocumentContent; // selected tab is DocuemntContent
            e.CanExecute = file != null && file.IsCloseable;      // and it is closeable
            e.Handled = true;
        }
        private void closeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // if close command could be executed, SelectedItem must be DocumentContent
            DocumentContent file = (filePane.SelectedItem as DocumentContent);
            if (file != null)
                file.Close();       // We do not care if it is saved or not. If DocumentContent could
                                    // not be closed it file.Close() will return false
            e.Handled = true;
        }
        // save
        private void saveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // filePane exists so get its active document (tab), it must be a TestFile
            var file = getActiveTestFile();
            // file exists, it is a test file and is not saved
            e.CanExecute = (file != null) && (!file.Saved);
            e.Handled = true;
        }
        private void saveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // when this method is called we are sure that filePane exists and selected item is a test file
            // and it is not saved
            TestFile file = getActiveTestFile();    // check for errors
            file.Save();
            e.Handled = true;
        }
        // save as
        private void saveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // check if testing is running otherwise ..

            // true when selected tab is a TestFile
            e.CanExecute = (getActiveTestFile() != null);  // it must not be saved to execute save as
            e.Handled = true;
        }
        private void saveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // when this method is called we are sure that filePane exists and selected item is a test file
            var dialog = FileManager.CreateSaveFileDialog();
            // ask user where to store the file
            if (dialog.ShowDialog(this) == true)    
            {
                try
                {
                    var file = getActiveTestFile();
                    file.SaveAs(dialog.FileName);
                }
                catch
                {
                    // move this to FileManager
                    MessageBox.Show("File could not be saved", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
        }
        // help
        private void helpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void helpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }
        // print
        private void printCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void printExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Get an instance of document content of type TestFile that is currently active inside the main area -
        /// <paramref name="filePane"/>.
        /// Return null if there is no such a document content
        /// </summary>
        private TestFile getActiveTestFile()
        {
            return (filePane == null) ? null : (filePane.SelectedItem as TestFile);
        }


        //viewTester
        private void viewTesterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePane != null && filePane.Items != null)
            {
                // test window could be opened just once
                foreach (var item in filePane.Items)
                    if (item is TestWindow)     // one of tab is a test window
                    {
                        e.CanExecute = false;   // could not open next one
                        return;
                    }
                // there is no test window - could be opened
                e.CanExecute = true;
            }
        }
        private void viewTesterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // open new tester window
            var tab = new TestWindow();     // create new test window
            filePane.Items.Add(tab);        // add it to tab collection
            filePane.SelectedItem = tab;    // select just created tab
        }

        // viewSettings
        private void viewSettingsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePane != null && filePane.Items != null)
            {
                // setting window could be opened just once
                foreach (var item in filePane.Items)
                    if (item is SettingsWindow)    // one of tab is a settings window
                    {
                        e.CanExecute = false;   // could no open next one
                        e.Handled = true;
                        return;
                    }
                // there is no setting window - could be opened
                e.CanExecute = true;
            }
        }
        private void viewSettingsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = new SettingsWindow(); // create new settings window
            filePane.Items.Add(tab);        // add it to tab colleciton
            filePane.SelectedItem = tab;    // select just created tab
        }

        // viewData
        private void viewDataCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void viewDataExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = new DataWindow();
            filePane.Items.Add(tab);
            filePane.SelectedItem = tab;
        }

        #endregion

        #region Constructors

        public WindowMain()
        {
            InitializeComponent();
            Output.TextBox = outputConsole;    // initialize output console
        }

        #endregion


    }
}
