using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Linq;
using System.Xml.Linq;
using MTS.Properties;

using System.Windows;
using Microsoft.Win32;

namespace MTS.Editor
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

        #region Xml Constants

        private static readonly XName IdAttr = "id";
        private static readonly XName EnabledAttr = "enabled";
        private static readonly XName TypeAttr = "type";

        private static readonly XName NameElem = "name";
        private static readonly XName DescriptionElem = "description";

        private static readonly XName TestElem = "test";        // !! very similiar
        private static readonly XName TestsElem = "tests";      // !!
        private static readonly XName ValueElem = "value";      // !!
        private static readonly XName ValuesElem = "values";    // !!

        private static readonly XName EnabledElem = "enabled";
        private static readonly XName MinElem = "min";
        private static readonly XName MaxElem = "max";
        private static readonly XName ParamElem = "param";
        private static readonly XName TypeElem = "type";
        private static readonly XName UnitElem = "unit";
        private static readonly XName GroupElem = "group";

        #region Param Value Types

        private const string intType = "int";
        private const string doubleType = "double";
        private const string boolType = "bool";
        private const string stringType = "string";
        private const string enumType = "enum";

        #endregion

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

        #region Xml Format

        private static TestValue getTestInstance(XElement tmplTest)
        {
            TestValue test = new TestValue(tmplTest.Attribute(IdAttr).Value);
            
            // set test properties - these values are independent on test values
            test.Name = tmplTest.Element(NameElem).Value;
            test.Description = tmplTest.Element(DescriptionElem).Value;
            test.GroupName = tmplTest.Element(GroupElem).Value;

            return test;
        }
        private static ParamValue getParamInstance(XElement tmplParam)
        {
            // save current culture
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            // use american culture to parse double values
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            ParamValue param;
            string type = tmplParam.Element(TypeElem).Value;    // type of parameter
            string id = tmplParam.Attribute(IdAttr).Value;

            switch (type)
            {
                case intType:
                    param = new IntParam(id)
                    {
                        MinValue = int.Parse(tmplParam.Element(MinElem).Value),
                        MaxValue = int.Parse(tmplParam.Element(MaxElem).Value),
                        Unit = Units.UnitFromString(tmplParam.Element(UnitElem).Attribute(TypeAttr).Value)
                    };
                    break;
                case doubleType:
                    param = new DoubleParam(id)
                    {
                        MinValue = double.Parse(tmplParam.Element(MinElem).Value),
                        MaxValue = double.Parse(tmplParam.Element(MaxElem).Value),
                        Unit = Units.UnitFromString(tmplParam.Element(UnitElem).Attribute(TypeAttr).Value)
                    };
                    break;
                case boolType:
                    param = new BoolParam(id);
                    break;
                case stringType:
                    param = new StringParam(id);
                    break;
                case enumType:
                    param = new EnumParam(id,
                        tmplParam.Element(ValuesElem).Elements(ValueElem).Select(el => el.Value).ToArray());
                    break;
                default: return null;   // invalid type
            }


            // these values are common for all parameter types
            // set test properties - these values are independent on parameter values
            param.Name = tmplParam.Element(NameElem).Value;
            param.Description = tmplParam.Element(DescriptionElem).Value;

            // parse default param value from string
            param.ValueFromString(tmplParam.Element(ValueElem).Value);

            // resotre previous culture
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            return param;
        }

        #endregion

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
            // load file to memory
            XElement fileRoot = XElement.Load(path);
            // load template to memory
            XElement tmplRoot = XElement.Load(Settings.Default.GetTemplatePath());
            // throw an exception if any of these two files does not exist

            // crate a new instance of test collection and add items from file
            TestCollection tc = new TestCollection();

            try     // catch exception while reading xml file - any mistake in input file
            {       // will be represented as an error in file format and would not be loaded                
                foreach (XElement test in fileRoot.Elements(TestElem))
                {
                    // get test id attribute
                    string testId = test.Attribute(IdAttr).Value;
                    // in template file (with this id) find aditional test properties
                    XElement tmplTest = tmplRoot.Elements(TestElem)
                        .FirstOrDefault(el => el.Attribute(IdAttr).Value == testId);

                    TestValue tv = getTestInstance(tmplTest);
                    // enable or disable this test
                    tv.Enabled = bool.Parse(test.Attribute(EnabledElem).Value);

                    foreach (XElement param in test.Elements(ParamElem))
                    {
                        // get param id attribute
                        string paramId = param.Attribute(IdAttr).Value;
                        // in template test element find aditional param properties
                        XElement tmplParam = tmplTest.Elements(ParamElem)
                            .FirstOrDefault(pr => pr.Attribute(IdAttr).Value == paramId);

                        // create an parameter instace acording its properties in xml template
                        ParamValue pv = getParamInstance(tmplParam);
                        // initialize its value
                        pv.ValueFromString(param.Value);
                        tv.AddParam(pv.Id, pv);
                    }

                    tc.AddTest(testId, tv);
                }
            }
            catch (Exception ex)
            {   // file could not be dederialized - it is an unkown format
                // this hides the impementation details (that file is serialized), noone knows about the serializetion
                // except of the FileManager
                throw new FileLoadException("File may be corrupted", Path.GetFileName(path), ex);
            }

            return tc;
        }
        /// <summary>
        /// Save a collection of test to a particular file
        /// </summary>
        /// <param name="path">Path to the file, where collection will be saved</param>
        /// <param name="collection">Collection of tests to save</param>
        public static void SaveFile(string path, TestCollection collection)
        {
            // create xml tree
            XElement root = new XElement(TestsElem);
            foreach (TestValue test in collection)
            {
                // create test element with attributes id and enabled
                XElement testElem = new XElement(TestElem,
                    new XAttribute(IdAttr, test.Id),
                    new XAttribute(EnabledElem, test.Enabled));
                // add param childs elements to test
                foreach (ParamValue param in test)
                {
                    // create param element with id attribute and its string value representation
                    XElement paramElem = new XElement(ParamElem,
                        new XAttribute(IdAttr, param.Id),
                        param.ValueToString());    // this is value of parameter in string format
                    // add to parameter to test
                    testElem.Add(paramElem);
                }
                // add test to collection
                root.Add(testElem);
            }

            // write xml to file
            root.Save(path);
        }

        /// <summary>
        /// Create and return new default instance of test collection with default properties.
        /// This method use template file to create default instance
        /// </summary>
        /// <returns></returns>
        public static TestCollection CreateNew()
        {
            //TODO: check for an exception (error in template file)

            // create a new empty instance of test collection
            TestCollection tc = new TestCollection();

            // load template file - root element is <tests>
            XElement tmplRoot = XElement.Load(Settings.Default.GetTemplatePath());

            foreach (XElement tmplTest in tmplRoot.Elements(TestElem))
            {
                // get empty instance of test (without properties) with default properties
                TestValue tv = getTestInstance(tmplTest);

                // add parameters from template to just created TestValue instance
                foreach (XElement tmplParam in tmplTest.Elements(ParamElem))
                    tv.AddParam(getParamInstance(tmplParam));

                // add test to collection
                tc.AddTest(tv);
            }

            return tc;
        }

        #endregion

        #region Dialogs

        /// <summary>
        /// Create and configure a new dialog to open a file with testing parameters
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
