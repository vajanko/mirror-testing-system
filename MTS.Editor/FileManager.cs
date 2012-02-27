using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using System.IO;
using Microsoft.Win32;

using MTS.Editor.Properties;
using MTS.Base;

namespace MTS.Editor
{
    public static class FileManager
    {
        #region Constants

        /// <summary>
        /// Part of name of a newly created file
        /// </summary>
        private const string NewFileString = "NewFile";
        /// <summary>
        /// Extension (including dot) of file containing collection of test parameters
        /// </summary>
        private const string defFileExtension = ".tc";
        /// <summary>
        /// Constant string describing filter for file dialog that will handle opening or saveing files
        /// containing collection of test parameters
        /// </summary>
        private const string fileFilter = "Test collections (" + defFileExtension + ")|*" + defFileExtension;

        /// <summary>
        /// String is displayed in the caption of message box when a file error occures
        /// </summary>
        private const string fileError = "File error";

        #endregion

        #region Xml Constants

        /* 
         * These are constants used in testing parameters files which structure is some kind of xml
         */

        /// <summary>
        /// Unique Id attribute of test or parameter
        /// </summary>
        private static readonly XName IdAttr = "id";
        /// <summary>
        /// Element that holds value of test or parameter name. This is short description of test or
        /// parameter and is used in user interface
        /// </summary>
        private static readonly XName NameElem = "name";
        /// <summary>
        /// Element that holds value of test properties
        /// </summary>
        private static readonly XName TestElem = "test";
        /// <summary>
        /// Element that holds value of parameter properties
        /// </summary>
        private static readonly XName ParamElem = "param";

        #endregion

        #region Fields

        /// <summary>
        /// Index of the file last time created. This number is added at the end of newly created file name.
        /// </summary>
        private static int lastNewFileIndex = 0;

        public static readonly XElement TemplateRoot;

        #endregion

        #region Methods

        #region Xml Format

        /// <summary>
        /// Get name of a test identified by it's string id. Null is returned if such a test does not exist.
        /// No exceptions should be thrown
        /// </summary>
        /// <param name="testId">Id of test which name is required to be found</param>
        /// <returns>Localized name of required test identified by it's id</returns>
        public static string GetTestName(string testId)
        {
            string name = null;
            if (TemplateRoot != null)
            {   // from template file load test element with required id
                XElement test = TemplateRoot.Elements(TestElem)
                    .Where(t => t.Attribute(IdAttr) != null && t.Attribute(IdAttr).Value == testId)
                    .FirstOrDefault();
                if (test != null)
                {   // if it exists get it's name
                    XElement n = test.Element(NameElem);
                    if (n != null)  // do not throw exception - if name exists return
                        name = n.Value;
                }
            }

            return name;
        }
        /// <summary>
        /// Get name of a test parameter identified by it's string id and id of test to which this parameter belongs.
        /// Null if returned if such a parameter does not exist. No exception should be thrown.
        /// </summary>
        /// <param name="testId">Id of test which parameter name is required to be found</param>
        /// <param name="paramId">Id of parameter which name is required to be found</param>
        /// <returns>Localized name of required parameter identified by it's id and id of test to which it belongs</returns>
        public static string GetParamName(string testId, string paramId)
        {
            string name = null;
            if (TemplateRoot != null)
            {   // from template file load test element with required id
                XElement test = TemplateRoot.Elements(TestElem)
                    .FirstOrDefault(t => t.Attribute(IdAttr) != null && t.Attribute(IdAttr).Value == testId);
                if (test != null)
                {   // if such a test exists get from is parameter with required id
                    XElement param = test.Elements(ParamElem)
                        .FirstOrDefault(p => p.Attribute(IdAttr) != null && p.Attribute(IdAttr).Value == paramId);
                    if (param != null)
                    {   // if such a parameter exists get it's name
                        XElement n = param.Element(NameElem);
                        if (n != null)  // do not throw exception - if name exists return
                            name = n.Value;
                    }
                }
            }

            return name;
        }

        #endregion

        #region Input/Output

        /// <summary>
        /// Generate a new unique file name - used when creating a new file. This only default name with
        /// unique numeric suffix without directory name.
        /// </summary>
        public static string GetNewName()
        {
            return NewFileString + ++lastNewFileIndex;  // index of last created file name
        }

        /// <summary>
        /// Read a particular file from given path and return its content as o collection of tests.
        /// Throws an exception if given path does not exists or file is in incorrect format or corrupted
        /// </summary>
        /// <param name="path">Path to file to read</param>
        /// <returns>Collection of <paramref name="TestValue"/> instances read from given file</returns>
        /// <exception cref="System.IO.FileFormatException">File is corrupted and could not be loaded</exception>
        /// <exception cref="System.IO.IOException">File could not be read because of insufficient privileges or other
        /// IO error</exception>
        /// <exception cref="System.IO.FileNotFoundException">File does not exist</exception>
        /// <exception cref="MTS.Base.ConfigException">Template file was not found</exception>
        public static TestCollection ReadFile(string path)
        {
            // notice that TestValue and ParamValue instances do not know anything about file format
            // this is the only place (also method for saving and creating new file) where the format of test 
            // collection file is described
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    string.Format(Errors.FileNotFoundMsg, Path.GetFileName(path)), path);
            
            // load template to memory (template file path is stored in application settings)
            XElement tmplRoot = loadTemplate();

            TestXmlParser parser = new TestXmlParser();

            // crate a new empty instance of test collection and add items from file
            TestCollection tc = null;

            try     // catch exception while reading xml file - any mistake in input file
            {       // will be represented as an error in file format and would not be loaded                
                // load file to memory
                XElement fileRoot = XElement.Load(path);

                tc = parser.LoadCollection(tmplRoot, fileRoot);
            }
            catch (Exception ex)
            {   // file could not be read - it is an unknown format
                // this hides the implementation details (that file is serialized or xml), no one knows about the format
                // except of the FileManager
                throw new FileFormatException(new Uri(path), 
                    string.Format(Errors.FileFormatErrorMsg, Path.GetFileName(path)), ex);
            }

            return tc;
        }
        /// <summary>
        /// Save a collection of test to a file in given path. File is overwritten if it already exists and
        /// created new if it does not.
        /// An exception is thrown if file could not be created or given collection of test values is corrupted
        /// </summary>
        /// <param name="path">Absolute path to the file, where collection will be saved</param>
        /// <param name="collection">Collection of tests to save</param>
        public static void SaveFile(string path, TestCollection collection)
        {
            // notice that TestValue and ParamValue instances do not know anything about file format
            // this is the only place (also method for reading and creating new file) where the format of test 
            // collection file is described

            // create a new instance of parser used to read test xml template from disk. It also provide reverse
            // conversion
            TestXmlParser parser = new TestXmlParser();

            XElement root = parser.CollectionToXml(collection);
            root.Save(path);
        }

        /// <summary>
        /// Create and return new default instance of test collection with default properties and default
        /// values of test parameters. This method use template file to create default instance. An exception
        /// is thrown if template does not exists.
        /// </summary>
        /// <returns>Collection of <paramref name="TestValue"/> loaded from template file initialized
        /// with default values</returns>
        /// <exception cref="MTS.Base.ConfigException">Template file not found or is corrupted</exception>
        public static TestCollection CreateNew()
        {
            // notice that TestValue and ParamValue instances do not know anything about file format
            // this is the only place (also method for saving and reading) where the format of test 
            // collection file is described

            // load template file - root element is <tests>
            XElement tmplRoot = loadTemplate();

            // create a new instance of parser that will create strongly typed instances of tests and their parameters
            TestXmlParser parser = new TestXmlParser();
            return parser.ParseCollection(tmplRoot);
        }

        /// <summary>
        /// Load template file from path saved in application settings
        /// </summary>
        /// <returns>XML root of template file document</returns>
        /// <exception cref="MTS.Base.ConfigNotFoundException">Template file doest not exist</exception>
        /// <exception cref="MTS.Base.ConfigException">Template file is corrupted</exception>
        private static XElement loadTemplate()
        {   // get path from application settings where template path is stored
            string path = Settings.Default.GetTemplatePath();
            XElement root = null;

            try
            {   // load template to memory
                root = XElement.Load(path);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new ConfigNotFoundException(path, 
                    string.Format(Errors.ConfigNotFoundMsg, Path.GetFileName(path)), ex);
            }
            catch (System.Xml.XmlException ex)
            {
                throw new ConfigException(path, 
                    string.Format(Errors.ConfigCorruptedMsg, Path.GetFileName(path)), ex);
            }
            return root;
        }

        #endregion

        #region Dialogs

        /// <summary>
        /// Create and configure a new dialog to open a file with testing parameters
        /// </summary>
        /// <returns>An instance of <paramref name="OpenFileDialog"/> configured for opening test collection
        /// files</returns>
        public static OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.AddExtension = true;                 // automatically add extension of files
            dialog.CheckFileExists = true;              // do not allow user to open file if it does not exists
            dialog.DefaultExt = defFileExtension;       // specify extension of test collection files
            dialog.Filter = fileFilter;                 // only allow to open test collection files
            // save directory from which file was opened last time and give it to user so he/she will be happy
            //dialog.InitialDirectory = ...
            dialog.Multiselect = false;                 // only one file may be opened at once
            dialog.Title = "Open test collection file"; // specify title of open file dialog
            
            return dialog;
        }
        /// <summary>
        /// Create and configure a new dialog to save a file wist testing parameters
        /// </summary>
        /// <returns>An instance of <see cref="SaveFileDialog"/> configured for saveing test collection
        /// files</returns>
        public static SaveFileDialog CreateSaveFileDialog()
        {
            var dialog = new SaveFileDialog();

            dialog.AddExtension = true;                 // automatically add extension of files
            dialog.DefaultExt = defFileExtension;       // specify extension of test collection files
            dialog.Filter = fileFilter;                 // only allow to overwrite test collection files
            // save directory which to file was saved last time and give it to user so he/she will be happy
            //dialog.InitialDirectory = ...
            dialog.Title = "Save test collection file"; // specify title of open file dialog

            return dialog;
        }

        #endregion

        #endregion

        /// <summary>
        /// FileManager static constructor
        /// </summary>
        static FileManager()
        {
            try
            {
                TemplateRoot = XElement.Load(Settings.Default.GetTemplatePath());
            }
            catch (Exception ex)
            {
                ExceptionManager.LogException(ex);
            }
        }
    }
}
