using System;
using System.Linq;
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
using MTS.Tester;

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

        /// <summary>
        /// This method is called when MainMenu->File->Exit item is clicked
        /// </summary>
        private void menuClick_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// This method is called when MainMenu->Options->Log in item is clicked
        /// </summary>
        private void login_Click(object sender, RoutedEventArgs e)
        {   // open login window for user
            login();
        }
        /// <summary>
        /// This method is called when MainMenu->Options->Log out item is clicked
        /// </summary>
        private void logout_Click(object sender, RoutedEventArgs e)
        {   // before logout close all tab items (some of them may require loged in user instance)
            List<DocumentItem> toClose = new List<DocumentItem>();
            foreach (DocumentContent item in filePane.Items)
            {   // get all document items - should be closed
                if (item is DocumentItem)
                    toClose.Add(item as DocumentItem);
            }
            while (toClose.Count > 0)
            {
                DocumentItem item = toClose[0];
                toClose.RemoveAt(0);
                if (!item.Close())
                    return; // this DocumentItem could not be closed - logout is not successfull
            }
            // now all document items are closed - operator will be loged out
            Output.WriteLine("Logout {0}", Admin.Operator.Instance.Login);
            Admin.Operator.LogOut();
            IsLoggedIn = false;
        }

        #endregion

        #region Window Events

        /// <summary>
        /// This method is called once when window is loaded. Login dialog is displayed to used imediatelly.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   // open login windows for user, so he or she should not manualy click this menu item at startup
            login();
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
                    if (file != null && file.ItemId == filename)
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
            // filePane exists so get its active document (tab), it must be a DocumentItem (Saveable)
            DocumentItem item = getActiveItem();

            // file exists, it is a test file and is not saved
            e.CanExecute = (item != null) && (item.CanSave);
            e.Handled = true;
        }
        private void saveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // when this method is called we are sure that filePane exists and selected item is a test file
            // and it is not saved
            DocumentItem item = getActiveItem();    // check for errors
            item.Save();
            e.Handled = true;
        }
        // save as
        private void saveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // check if testing is running otherwise ..

            // true when selected tab is a TestFile
            e.CanExecute = (getActiveItem() != null);  // it must not be saved to execute save as
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
                    TestFile file = getActiveTestFile();
                    file.SaveAs(dialog.FileName);
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowError(ex);
                    // move this to FileManager
                    //MessageBox.Show("File could not be saved", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// Get an instance of document content of type  <see cref="DocumentItem"/> that is currently active inside the main area -
        /// <paramref name="filePane"/>.
        /// Return null if there is no such a document content
        /// </summary>
        private DocumentItem getActiveItem()
        {
            return (filePane == null) ? null : (filePane.SelectedItem as DocumentItem);
        }
        /// <summary>
        /// Get an instance of document content of type <see cref="TestFile"/> that is currently active inside the main area -
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
                        e.Handled = true;
                        return;
                    }
                // there is no test window - could be opened, but only if some operator is logged in
                e.CanExecute = Admin.Operator.IsLogedIn();
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
                        e.CanExecute = false;   // could not open next one
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
            if (filePane != null && filePane.Items != null)
            {
                // data window could be opened just once
                foreach (var item in filePane.Items)
                    if (item is DataWindow)     // one of tab is a data window
                    {
                        e.CanExecute = false;   // could not open next one
                        e.Handled = true;
                        return;
                    }
                // there is no data window - could be opened
                e.CanExecute = true;
            }
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

        #region IsLoggedIn Property

        public static readonly DependencyProperty IsLoggedInProperty =
            DependencyProperty.Register("IsLoggedIn", typeof(bool), typeof(WindowMain),
            new PropertyMetadata(false));

        public bool IsLoggedIn
        {
            get { return (bool)GetValue(IsLoggedInProperty); }
            set { SetValue(IsLoggedInProperty, value); }
        }

        #endregion

        /// <summary>
        /// Open login window (this will block the application) and ask user for login and password.
        /// This window will be opened until user is loged in successfully or it is manually closed by user
        /// </summary>
        private void login()
        {
            Admin.Operator.TryLogin("admin", "admin");
            
            //LoginWindow loginWindow = new LoginWindow(this, false);
            //loginWindow.ShowDialog();   // show window for first time

            //// if result is null this loop will end without logging in
            //while (loginWindow.Result == LoginResult.Fail)
            //{
            //    loginWindow = new LoginWindow(this, !(bool)loginWindow.DialogResult);
            //    loginWindow.ShowDialog();
            //}

            if (Admin.Operator.IsLogedIn())
            {
                Output.WriteLine("Login {0}", Admin.Operator.Instance.Login);
                IsLoggedIn = true;
            }
        }

        private void profile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
