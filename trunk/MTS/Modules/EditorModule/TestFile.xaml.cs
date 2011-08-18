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

namespace MTS.EditorModule
{
    /// <summary>
    /// Interaction logic for TestFile.xaml
    /// </summary>
    public partial class TestFile : DocumentContent
    {
        #region Constants

        /// <summary>
        /// Constant string "DisplayFileName"
        /// </summary>
        public const string DisplayFileNameString = "DisplayFileName";

        /// <summary>
        /// Extension that is added at the end of file name, when file has been changed and is not saved
        /// </summary>
        private const string notSavedExtension = "*";
        
        #endregion

        #region Properties

        private bool saved; // use this property when do not want to raise PropertyChanged event
        /// <summary>
        /// (Get) When true document content is saved. Notice that when false, document content could be only
        /// changed or the file does not exists yet.
        /// </summary>
        public bool Saved
        {
            get { return saved; }
            protected set
            {
                saved = value;  // when document is unsaved a standard star is displayed at the top of document
                RaisePropertyChanged(DisplayFileNameString);    // add or remove star in displayed string
            }
        }

        /// <summary>
        /// (Get) When true file where document content is stored exists, but it must not be saved.
        /// Value is false only when new file is created.
        /// </summary>
        public bool Exists
        {
            get;
            protected set;
        }

        private string filePath;
        /// <summary>
        /// (Get) Aboslute path to the file where document content is stored. When file does not exists yet,
        /// <paramref name="FilePath"/> is set to generated filename
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            protected set
            {
                filePath = value;   // when filePath is changed (generated filename change to real path or SaveAs
                // command is executed) also string displayer at the top of the document will change
                RaisePropertyChanged(DisplayFileNameString);
            }
        }
        /// <summary>
        /// (Get) Name of the file where docmunent content is stored or generated name when file does not exist yet.
        /// This string is displayed at the top of the document (optionaly with star when file is not saved).
        /// </summary>
        public string FileName
        {   // when file does not exist yet, FilePath contains generated file name
            get { return Exists ? System.IO.Path.GetFileName(FilePath) : FilePath; }
        }
        /// <summary>
        /// (Get) Same as <paramref name="FileName"/> but star (*) is added at the end when file
        /// is not saved.
        /// </summary>
        public string DisplayFileName
        {
            get { return Saved ? FileName : FileName + notSavedExtension; }
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
            var view = CollectionViewSource.GetDefaultView(Tests.Values);

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
            Saved = false;
            // which does not exists
            Exists = false;
            // fill with default collection
            Tests = new TestCollection();
            // create view of tests - grouping and sorting
            createTestView(Tests);
            // register method that is called when any property is changed
            Tests.SetPropertyChangedHandler(TestFile_PropertyChanged);
            // newly created file gets some unic name
            FilePath = FileManager.GetNewName();

            Output.WriteLine("New file (" + FilePath + ") created.");
        }
        // This method is called whenever any of test parameters change
        private void TestFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Saved)  // this raises an event - only change if necessary
                Saved = false;
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
            saved = true;       // setting this properties does not raise an event
            Exists = true;
            FilePath = path;    // here an event is raised - displayed name is changed

            Output.WriteLine("Opened file " + FilePath);
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
            saved = true;   // FilePath will raise an event (so do not use Saved=true;)

            Output.WriteLine("File " + FilePath + " saved as " + path);

            FilePath = path;
        }
        /// <summary>
        /// Save file to its location (overwrite old version). If file is not saved yet, ask user for a location
        /// </summary>
        public void Save()
        {
            // do not access HDD if not necessary
            if (Saved) return;

            if (Exists) // file already exists - only will be overwritten
            {
                FileManager.SaveFile(FilePath, Tests);
                Saved = true;   // event raised
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

        #endregion

        #region Overrided Methods

        /// <summary>
        /// Imediatelly close file with testing parameters, but ask user if file is not saved
        /// </summary>
        /// <returns>True if DocumentContent is should be removed</returns>
        public override bool Close()
        {
            // This method is called when right button is pressed and "close" from the popup menu selected
            // check if file is saved so we it could be closed safely
            if (!Saved)
            {
                // file is not saved - ask user what to do
                var result = MessageBox.Show("File \"" + FileName + "\" is not saved. Save changes?", "File not saved!",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Cancel)      // user changed his opinion
                    return false;       // cancel closing document
                else if (result == MessageBoxResult.Yes)    // user wants to save changes
                    Save();     // if does not exist - ask for path where to save

                // if result is No - document will be closed
                // even if result is Yes - document will be closed                
            }

            Output.WriteLine("File " + FilePath + " closed");

            return base.Close();    // document is closed always, only if Calcel was pressed
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
