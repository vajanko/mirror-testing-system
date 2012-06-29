using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using AvalonDock;

using Microsoft.Win32;

using MTS.Base;
using MTS.Editor;
using MTS.Editor.Properties;

namespace MTS.Editor
{
    /// <summary>
    /// Interaction logic for TestFile.xaml
    /// </summary>
    public partial class TestFileItem : DocumentItem
    {
        #region Properties

        /// <summary>
        /// (Get) When true file where document content is stored exists, but it must not be saved.
        /// Value is false only when new file is created.
        /// </summary>
        public bool Exists
        {
            get;
            protected set;
        }

        #endregion

        #region Dependency Properties

        #region Tests Property

        public static readonly DependencyProperty TestsProperty =
            DependencyProperty.Register("Tests", typeof(TestCollection), typeof(TestFileItem));

        public TestCollection Tests
        {
            get { return (TestCollection)GetValue(TestsProperty); }
            set { SetValue(TestsProperty, value); }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Create default view of tests. By default tests are sorted by Name and grouped by its
        /// property GroupName
        /// </summary>
        /// <param name="tests">Collection of tests</param>
        private static void createTestView(TestCollection tests)
        {
            // This method is called whenever a collection of test is loaded and displayed in a document
            // content. This happens when new file is created or already created file is opened

            // By default every collection has a view. Just take its default view and modify it
            var view = CollectionViewSource.GetDefaultView(tests);

            // sort tests by Name
            //view.SortDescriptions.Clear();
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // group them by its property GroupName
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
        }


        #endregion

        #region Public Methods (File handling)

        /// <summary>
        /// Create a new test collection and add it to the document content.
        /// New filename will be generated
        /// </summary>
        /// <exception cref="MTS.Base.ConfigException">Template file not found or is corrupted</exception>
        public void New()
        {
            // create new unsaved file
            IsSaved = false;
            // which does not exists
            Exists = false;
            // fill with default collection
            Tests = FileManager.CreateNew();
            // create view of tests - grouping and sorting
            createTestView(Tests);
            // register method that is called when any property is changed
            Tests.SetPropertyChangedHandler(TestFile_PropertyChanged);
            // newly created file gets some unique name
            ItemId = FileManager.GetNewName();

            Output.WriteLine("New file \"{0} created.", ItemId);
        }
        /// <summary>
        /// This method is called whenever any of test parameters change. Status of file will be changed to unsaved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsSaved)  // this raises an event - only change if necessary
                IsSaved = false;
        }

        /// <summary>
        /// Open a new file, read test collection from it and display in this document content
        /// </summary>
        /// <param name="path">Absolute path to the file</param>
        /// <returns>Value indicating whether opening file was successful</returns>
        public bool Open(string path)
        {
            try
            {
                // read file - let the error handling to the main window
                // this is a dependency property - user interface will be generated when test collection is added to it
                Tests = FileManager.ReadFile(path);
                // create a view of tests - grouping and sorting
                createTestView(Tests);
                // this handler will be executed when any of test property or test parameter property change
                // so we will be notified when file is not saved
                Tests.SetPropertyChangedHandler(TestFile_PropertyChanged);
                Exists = true;

                ItemId = path;      // here an event is raised - displayed name is changed 

                Output.WriteLine("Opened file " + path);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
            return false;
        }
        /// <summary>
        /// Save file to a new location. Even if file is already saved, ask user for a new location.
        /// </summary>
        /// <param name="path">Absolute path to the file</param>
        public void SaveAs(string path)
        {
            // this method could be called even if file is saved
            // When overwriting some existing file, FileManager will ask user if it is OK
            FileManager.SaveFile(path, Tests);
            Exists = true;

            // add this string to resources
            Output.WriteLine("File \"" + path + "\" saved as " + path);

            ItemId = path;
            IsSaved = true;
        }

        #endregion

        #region Overrided Methods

        /// <summary>
        /// Save file to its location (overwrite old version). If file is not saved yet, ask user for a location
        /// </summary>
        public override void Save()
        {
            // do not access HDD if not necessary
            if (IsSaved) 
                return;

            if (!Tests.IsCurrentVersion)
            {   // opened file is not in current application format - needs to be converted - ask user
                // add this string to resources
                var result = MessageBox.Show(
                    string.Format("\"{0}\" is is different format than supported by current version of the application. Would you like to convert the file to current format?", Title),
                    "Different file format!",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);

                if (result == MessageBoxResult.No)
                    return; // user does not want to convert the file - cancel save event
            }

            if (Exists) // file already exists - only will be overwritten
            {
                FileManager.SaveFile(ItemId, Tests);
                base.Save();    // event raised
            }
            else       // file does not exists yet - we need to ask user where to store the file
            {
                // create configured save file dialog
                var dialog = FileManager.CreateSaveFileDialog();

                // file name entered - save the file
                if (dialog.ShowDialog() == true)
                    SaveAs(dialog.FileName);    // saving first time
            }            
        }

        /// <summary>
        /// This implementation of conversion method gets the filename of path which is stored in 
        /// <see cref="ItemId"/> property
        /// </summary>
        /// <param name="id">Absolute path to filename</param>
        /// <param name="saved">Value indicating whether file is saved</param>
        /// <returns>Filename (optional star if file is not saved) file displayed in this item</returns>
        protected override string ConvertIdToTitle(string id, bool saved)
        {
            if (id != null)
            {
                id = System.IO.Path.GetFileName(id);
                return base.ConvertIdToTitle(id, saved);
            }
            return id;
        }

        #endregion

        #region Events

        /// <summary>
        /// This method is called as a preview of getting keyboard focus. ListBoxItem on which this event was raised
        /// will be selected as it contains element with focus or it is focused
        /// </summary>
        /// <param name="sender">ListBoxItem on which was raised this event</param>
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                (sender as ListBoxItem).IsSelected = true;
            }
        }

        /// <summary>
        /// This method is called when focus of ListView is lost. When focus is given to some control outside this
        /// ListView, any selected item is deselected.
        /// </summary>
        /// <param name="sender">ListView which has lost the focus</param>
        private void testListView_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // deselect any item in this list view when focus is lost and outside this view
            ListView view = sender as ListView;
            if (view != null && !view.IsKeyboardFocusWithin)
                view.SelectedIndex = -1;
        }

        /// <summary>
        /// This method is called when selected item of the side list of test checkboxes change. The selected
        /// test will be brought into view.
        /// </summary>
        private void checkTestListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            testList.ScrollIntoView(testList.SelectedItem);
        }

        #endregion

        #region Constructors

        public TestFileItem()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        #endregion

        private void checkAllBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            if (box == null)
                return;
            foreach (TestValue item in checkTestListBox.Items)
            {
                item.Enabled = true;
            }
            box.Content = "Uncheck all";
        }

        private void checkAllBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            if (box == null)
                return;
            foreach (TestValue item in checkTestListBox.Items)
            {
                item.Enabled = false;
            }
            box.Content = "Check all";
        }
        
    }
}
