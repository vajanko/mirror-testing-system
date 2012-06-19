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

using MTS.Base;
using MTS.Controls;
using MTS.Editor;
using MTS.Admin;
using MTS.Admin.Controls;
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
        /// <summary>
        /// (Get) Value indicating whether testing window is opened and execution is running
        /// </summary>
        public bool IsTestRunning { get; private set; }

        #region Events

        #region Menu Events

        /// <summary>
        /// This method is called when MainMenu->File->Exit item is clicked
        /// </summary>
        private void menuClick_Exit(object sender, RoutedEventArgs e)
        {   // first of all try to close all windows
            if (!closeAll())
                return;

            // close main application window
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
        {
            logout();
        }
        /// <summary>
        /// This method is called when 
        /// </summary>
        private void profile_Click(object sender, RoutedEventArgs e)
        {
            Admin.Controls.ProfileWindow wnd = new Admin.Controls.ProfileWindow();
            wnd.ShowDialog();
        }

        #endregion

        #region Window Events

        /// <summary>
        /// This method is called once when window is loaded. Login dialog is displayed to used immediately.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   // open login windows for user, so he or she should not manually click this menu item at startup
            login();

            if (Application.Current.Properties["open"] != null)
            {
                openTestFile(Application.Current.Properties["open"].ToString());
            }
        }

        #endregion

        #region Drag Drop Events

        private void filePane_DragEnter(object sender, DragEventArgs e)
        {
            if (filePane.IsMouseOver && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void filePane_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    foreach (var file in files)
                        openTestFile(file);
                }
            }
        }

        private void filePane_DragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        #endregion

        #endregion

        #region Commands

        #region New Command

        private void newCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // do not allow create new file if testing is running
            e.CanExecute = !IsTestRunning;
            e.Handled = true;
        }
        private void newExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // create new tab
                TestFileItem file = new TestFileItem();
                // fill it with new test collection
                file.New();

                // add to the main pane
                filePane.Items.Add(file);
                // activate it - show to user
                filePane.SelectedItem = file;
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Open Command

        private void openCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // do not allow to open a file if testing is running
            e.CanExecute = !IsTestRunning;
            e.Handled = true;
        }
        private void openExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // at this time tab containing the opening document does not exist so window must handle this command
            // create a configured open file dialog
            var dialog = FileManager.CreateOpenFileDialog();

            // file entered - create new tab
            if (dialog.ShowDialog() == true)
            {
                HandleOpenFile(dialog.FileName);
            }
            e.Handled = true;
        }
        /// <summary>
        /// Open given file if it can be opened. Handle all exception a present them to the user.
        /// </summary>
        /// <param name="filename"></param>
        public void HandleOpenFile(string filename)
        {
            if (IsTestRunning)
                return;

            try
            {
                openTestFile(filename);
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
        }
        private void openTestFile(string filename)
        {
            TestFileItem file;

            // check each content in the main pane (file pane) if document is not opened yet
            foreach (DocumentContent content in filePane.Items)
            {
                file = (content as TestFileItem);   // only check TestFile
                if (file != null && file.ItemId == filename)
                {   // file is already opened - do not open it again, but show it to the user
                    file.Activate();
                    return;
                }
            }

            // file is not opened yet - new tab will be created
            file = new TestFileItem();          // create new tab
            if (file.Open(filename))     // read content to it
            {
                filePane.Items.Add(file);       // add to dockable pane
                file.Activate();                // show it to user
            }
        }

        #endregion

        #region Close Command

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
            try
            {
                // if close command could be executed, SelectedItem must be DocumentContent
                DocumentContent file = (filePane.SelectedItem as DocumentContent);
                if (file != null)
                    file.Close();       // We do not care if it is saved or not. If DocumentContent could
                // not be closed it file.Close() will return false
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Save Command

        private void saveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // filePane exists so get its active document (tab), it must be a DocumentItem (Savable)
            DocumentItem item = getActiveItem();

            // file exists, it is a test file and is not saved
            e.CanExecute = (item != null) && (item.CanSave);
            e.Handled = true;
        }
        private void saveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // when this method is called we are sure that filePane exists and selected item is a test file
                // and it is not saved
                DocumentItem item = getActiveItem();    // check for errors
                item.Save();
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Save As Command

        private void saveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // check if testing is running otherwise ..

            // true when selected tab is a TestFile
            e.CanExecute = (getActiveTestFile() != null);  // it must not be saved to execute save as
            e.Handled = true;
        }
        private void saveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // when this method is called we are sure that filePane exists and selected item is a test file
                var dialog = FileManager.CreateSaveFileDialog();
                // ask user where to store the file
                if (dialog.ShowDialog(this) == true)
                {
                    TestFileItem file = getActiveTestFile();
                    file.SaveAs(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        #endregion

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

        #region Login Command

        private void logInCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Admin.Operator.IsLoggedIn();
            e.Handled = true;
        }
        private void logInExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            login();
        }

        #endregion

        #region Logout Command

        private void logOutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Admin.Operator.IsLoggedIn();
            e.Handled = true;
        }
        private void logOutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            logout();
        }

        #endregion

        #region View Profile Command

        private void viewProfileCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Admin.Operator.IsLoggedIn();
            e.Handled = true;
        }
        private void viewProfileExecuted(object sender, ExecutedRoutedEventArgs e)
        {   // show window with operator profiles
            Admin.Controls.ProfileWindow wnd = new Admin.Controls.ProfileWindow();
            wnd.ShowDialog();
        }

        #endregion

        #region View Tester Command

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
                e.CanExecute = Admin.Operator.IsLoggedIn();
            }
        }
        private void viewTesterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // open new tester window
                var tab = new TestWindow();     // create new test window
                filePane.Items.Add(tab);        // add it to tab collection
                filePane.SelectedItem = tab;    // select just created tab
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        #endregion

        #region View Settings Command

        private void viewSettingsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePane != null && filePane.Items != null)
            {
                // setting window could be opened just once
                foreach (var item in filePane.Items)
                    if (item is SettingsItem)    // one of tab is a settings window
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
            try
            {
                var tab = new SettingsItem(); // create new settings window
                filePane.Items.Add(tab);        // add it to tab collection
                filePane.SelectedItem = tab;    // select just created tab
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
        }

        #endregion

        #region View Data Command

        private void viewDataCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePane != null && filePane.Items != null)
            {
                // data window could be opened just once
                foreach (var item in filePane.Items)
                {
                    if (item is DataWindow)     // one of tab is a data window
                    {
                        e.CanExecute = false;   // could not open next one
                        e.Handled = true;
                        return;
                    }
                }
                // there is no data window - could be opened
                e.CanExecute = true;
            }
            e.CanExecute = true;
            e.Handled = true;
        }
        private void viewDataExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var tab = new DataWindow();
                filePane.Items.Add(tab);
                filePane.SelectedItem = tab;
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
        }

        #endregion

        #endregion

        #region IsLoggedIn Property

        public static readonly DependencyProperty IsLoggedInProperty =
            DependencyProperty.Register("IsLoggedIn", typeof(bool), typeof(WindowMain),
            new PropertyMetadata(false));

        /// <summary>
        /// (Get/Set) Value indicating whether an operator is logged in. This is a dependency property.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return (bool)GetValue(IsLoggedInProperty); }
            set { SetValue(IsLoggedInProperty, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Open login window (this will block the application) and ask user for login and password.
        /// This window will be opened until user is logged in successfully or it is manually closed by user
        /// </summary>
        private void login()
        {
#if DEBUG
            if (!Admin.Operator.TryLogin("admin", "admin"))
            {   // for debugging: try to login as admin, if it is not possible display login window
                LoginWindow loginWindow = new LoginWindow(this, false);
                loginWindow.ShowDialog();   // show window for first time

                // if result is null this loop will end without logging in
                while (loginWindow.Result == LoginResult.Fail)
                {
                    loginWindow = new LoginWindow(this, !(bool)loginWindow.DialogResult);
                    loginWindow.ShowDialog();
                }

            }
#else
            LoginWindow loginWindow = new LoginWindow(this, false);
            loginWindow.ShowDialog();   // show window for first time

            // if result is null this loop will end without logging in
            while (loginWindow.Result == LoginResult.Fail)
            {
                loginWindow = new LoginWindow(this, !(bool)loginWindow.DialogResult);
                loginWindow.ShowDialog();
            }
#endif
            if (Admin.Operator.IsLoggedIn())
            {
                Output.WriteLine("Login {0}", Admin.Operator.Instance.Login);
                IsLoggedIn = true;
            }
        }

        /// <summary>
        /// Close all open windows and if successful logout the currently logged in operator
        /// </summary>
        private void logout()
        {
            if (!closeAll())
                return;

            // now all document items are closed - operator will be logged out
            string login = Admin.Operator.Instance.Login;
            Admin.Operator.LogOut();
            Output.WriteLine("Logout {0}", login);
            IsLoggedIn = false;
        }
        /// <summary>
        /// Try to close all tab items. If one of them can not be closed (blocked by the user)
        /// the process of closing is stopped
        /// </summary>
        /// <returns>True if all tab items were closed</returns>
        private bool closeAll()
        {
            List<DocumentItem> toClose = new List<DocumentItem>();
            toClose.AddRange(filePane.Items.OfType<DocumentItem>());

            while (toClose.Count > 0)
            {
                DocumentItem item = toClose[0];
                toClose.RemoveAt(0);
                if (!item.Close())
                    return false; // this DocumentItem could not be closed - logout is not successful
            }

            return true;
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
        /// Get an instance of document content of type <see cref="TestFileItem"/> that is currently active inside the main area -
        /// <paramref name="filePane"/>.
        /// Return null if there is no such a document content
        /// </summary>
        private TestFileItem getActiveTestFile()
        {
            return (filePane == null) ? null : (filePane.SelectedItem as TestFileItem);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of main application window
        /// </summary>
        public WindowMain()
        {
            InitializeComponent();
            // initialize output console - to this text box output logs will be written
            Output.TextBox = outputConsole;
        }

        #endregion
    }
}
