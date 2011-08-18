using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using System.Windows;
using Microsoft.Win32;

namespace MTS.EditorModule
{
    public static class FileManager
    {
        #region Constants

        /// <summary>
        /// Part of name of a newly created file
        /// </summary>
        private const string NewFileString = "NewFile";

        private const string defFileExtension = ".tc";
        private const string fileFilter = "Test collections (" + defFileExtension + ")|*" + defFileExtension;

        /// <summary>
        /// String is displayed in the caption of message box when a file error occures
        /// </summary>
        private const string fileError = "File error";

        #endregion

        #region Fields

        /// <summary>
        /// Index of the file last time created. This number is added at the end of newly created file name.
        /// </summary>
        private static int lastNewFileIndex = 0;

        #endregion

        #region Properties

        public static string LastSaveDirectory = "E:\tmp";
        public static string LastOpenDirectory = "E:\tmp";

        #endregion

        #region Methods

        #region Input/Output

        /// <summary>
        /// Generate a new unic file name - used when creating a new file
        /// </summary>
        public static string GetNewName()
        {
            lastNewFileIndex++;     // index of last created file name
            return NewFileString + lastNewFileIndex.ToString();
        }

        /// <summary>
        /// Read a particular file and return its content as o collection of tests
        /// </summary>
        /// <param name="path">Path to file to read</param>
        public static TestCollection ReadFile(string path)
        {
            BinaryFormatter formater = new BinaryFormatter();
            // open file
            Stream stream = File.Open(path, FileMode.Open);
            // deserialize
            TestCollection tc;
            try     // catch exception while deserializing
            {
                tc = (TestCollection)formater.Deserialize(stream);
            }
            catch (SerializationException ex)
            {   // file could not be dederialized - it is an unkown format
                // this hides the impementation details (that file is serialized), noone knows about the serializetion
                // except of the FileManager
                throw new FileLoadException("Unknown file format", Path.GetFileName(path), ex);
            }

            // close file
            stream.Close();
            // associate metadata with deserialized values
            tc.InitializeMetadata();

            return tc;
        }
        /// <summary>
        /// Save a collection of test to a particular file
        /// </summary>
        /// <param name="path">Path to the file, where collection will be saved</param>
        /// <param name="collection">Collection of tests to save</param>
        public static void SaveFile(string path, TestCollection collection)
        {
            BinaryFormatter formater = new BinaryFormatter();
            // open file
            Stream stream = File.Open(path, FileMode.Create);
            // serialize
            formater.Serialize(stream, collection);
            // close file
            stream.Close();
        }

        #endregion

        #region Dialogs

        /// <summary>
        /// Create and configure a new dialog to open a file
        /// </summary>
        public static OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.DefaultExt = defFileExtension;
            dialog.Filter = fileFilter;
            //dialog.InitialDirectory = ...
            dialog.Multiselect = false;
            dialog.Title = "Open test collection file";
            
            return dialog;
        }
        /// <summary>
        /// Create and configure a new dialog to save a file
        /// </summary>
        public static SaveFileDialog CreateSaveFileDialog()
        {
            var dialog = new SaveFileDialog();

            dialog.AddExtension = true;
            dialog.DefaultExt = defFileExtension;
            dialog.Filter = fileFilter;
            //dialog.InitialDirectory = LastSaveDirectory;
            dialog.Title = "Save test collection file";

            return dialog;
        }

        #endregion

        #region Error handling

        public static void HandleSaveException(Exception ex)
        {
            // this message will be displayed to user
            // will be modified according to exception type
            string message = "An error occured while saveing file:\n";

            if (ex is IOException)          // any type of input/output exception
                message += ex.Message;      // display IO exception message
            else
                throw ex;                   // we are not able to handle this exception

            // display MessageBox with created message
            MessageBox.Show(message, fileError, MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.OK);
        }
        
        public static void HandleOpenException(Exception ex)
        {
            // this message will be displayed to user
            // will be modified according to exception type
            string message = "An error occured while opening file:\n";

            if (ex is IOException)          // any type of input/output exception
                message += ex.Message;      // display IO exception message
            else
                throw ex;                   // we are not able to handle this exception

            // display MessageBox with created message
            MessageBox.Show(message, fileError, MessageBoxButton.OK,
                MessageBoxImage.Error, MessageBoxResult.OK);
        }

        #endregion

        #endregion
    }
}
