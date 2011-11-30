using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
//using System.Text;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.ComponentModel;
using AvalonDock;

using Microsoft.Win32;

using MTS.Controls;

namespace MTS.Editor
{
    /// <summary>
    /// Interaction logic for TestFile.xaml
    /// </summary>
    public partial class TestFile : DocumentItem
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
            DependencyProperty.Register("Tests", typeof(TestCollection), typeof(TestFile),
            new PropertyMetadata(null, testPropertyChanged));

        private static void testPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //(source as TestFile).Saved = false;
        }

        public TestCollection Tests
        {
            get { return (TestCollection)GetValue(TestsProperty); }
            set { SetValue(TestsProperty, value); }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Create default view of tests. By default tests are sorted by Name and grouped byt its
        /// property GroupName
        /// </summary>
        /// <param name="tests">Collection of tests</param>
        private void createTestView(TestCollection tests)
        {
            // This method is called whenever a collection of test is loaded and displayed in a document
            // content. This happens when new file is created or already created file is opened

            // By default every collection has a view. Just take its default view and modify it
            var view = CollectionViewSource.GetDefaultView(Tests);

            // sort tests by Name
            //view.SortDescriptions.Clear();
            //view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // gorup them by its property GroupName
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
        }

        #endregion

        #region Public Methods (File handling)

        /// <summary>
        /// Create a new test collection and add it to the document content.
        /// New filename will be generated
        /// </summary>
        public void New()
        {
            // create new unsaved file
            IsSaved = false;

            //// create new unsaved file
            //Saved = false;

            // which does not exists
            Exists = false;
            // fill with default collection
            Tests = FileManager.CreateNew();
            // create view of tests - grouping and sorting
            createTestView(Tests);
            // register method that is called when any property is changed
            Tests.SetPropertyChangedHandler(TestFile_PropertyChanged);
            // newly created file gets some unic name
            ItemId = FileManager.GetNewName();
            //FilePath = FileManager.GetNewName();

            Output.WriteLine("New file \"" + ItemId + "\" created.");
        }
        // This method is called whenever any of test parameters change
        private void TestFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsSaved)  // this raises an event - only change if necessary
                IsSaved = false;
        }

        /// <summary>
        /// Open a new file, read test collection from it and display in this document content
        /// </summary>
        /// <param name="path">Absolute path to the file</param>
        public void Open(string path)
        {
            // read file - let the error handling to the main window
            // this is a dependecy property - user interface will be generated when test collection is added to it
            Tests = FileManager.ReadFile(path);
            // create a view of tests - grouping and sorting
            createTestView(Tests);
            // this handler will be executed when any of test property or test parameter property change
            // so we will be notified when file is not saved
            Tests.SetPropertyChangedHandler(TestFile_PropertyChanged);            
            Exists = true;

            ItemId = path;      // here an event is raised - displayed name is changed 

            Output.WriteLine("Opened file " + path);
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
            if (IsSaved) return;

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
                    SaveAs(dialog.FileName);    // saveing first time
            }            
        }

        /// <summary>
        /// This implementation of convertion method gets the filename of path which is stored in 
        /// <see cref="ItemId"/> property
        /// </summary>
        /// <param name="id">Absolute path to filename</param>
        /// <param name="saved">Value indicating whether file is saved</param>
        /// <returns>Filename (optional asterix if file is not saved) file dispalyed in this item</returns>
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

        #region Constructors

        public TestFile()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        #endregion
    }
}
