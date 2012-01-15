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
        /// Attribute that holds value indicating if test is enabled or disabled
        /// </summary>
        private static readonly XName EnabledAttr = "enabled";
        /// <summary>
        /// Attribute that holds value of unit of parameter value
        /// </summary>
        private static readonly XName TypeAttr = "type";
        /// <summary>
        /// Attribute that holds number of allowed decimals of parameter value
        /// </summary>
        private static readonly XName DecimalasAtrr = "decimals";

        /// <summary>
        /// Element that holds value of test or parameter name. This is short description of test or
        /// parameter and is used in user interface
        /// </summary>
        private static readonly XName NameElem = "name";
        /// <summary>
        /// Element that holds value of test or parameter description. This is longer peace of test desribing
        /// in details current test or parameter. Could be used in user interface f.e. in a tooltip
        /// </summary>
        private static readonly XName DescriptionElem = "description";

        /// <summary>
        /// Element that holds value of test properties
        /// </summary>
        private static readonly XName TestElem = "test";        // !! very similiar
        /// <summary>
        /// Element that holds collection of test elements
        /// </summary>
        private static readonly XName TestsElem = "tests";      // !!
        /// <summary>
        /// Element that holds default value of a parameter
        /// </summary>
        private static readonly XName ValueElem = "value";      // !!
        /// <summary>
        /// Element that holds possible values of enumerator parameter
        /// </summary>
        private static readonly XName ValuesElem = "values";    // !!

        /// <summary>
        /// Element that holds value indicating if test is enabled
        /// </summary>
        private static readonly XName EnabledElem = "enabled";
        /// <summary>
        /// Element that holds minimal possible value of parameter
        /// </summary>
        private static readonly XName MinElem = "min";
        /// <summary>
        /// Element that holds maximal possible value of parameter
        /// </summary>
        private static readonly XName MaxElem = "max";
        /// <summary>
        /// Element that holds value of parameter properties
        /// </summary>
        private static readonly XName ParamElem = "param";
        /// <summary>
        /// Element that holds value of parameter type
        /// </summary>
        private static readonly XName TypeElem = "type";
        /// <summary>
        /// Element that holds value of describing unit of parameter value
        /// </summary>
        private static readonly XName UnitElem = "unit";
        /// <summary>
        /// Element that holds name of group to which test wich contains this element belongs
        /// </summary>
        private static readonly XName GroupElem = "group";

        #region Param Value Types

        /// <summary>
        /// Constant string used do define integer parameter
        /// </summary>
        private const string intType = "int";
        /// <summary>
        /// Constant string used do define double parameter
        /// </summary>
        private const string doubleType = "double";
        /// <summary>
        /// Constant string used do define boolean parameter
        /// </summary>
        private const string boolType = "bool";
        /// <summary>
        /// Constant string used do define string parameter
        /// </summary>
        private const string stringType = "string";
        /// <summary>
        /// Constant string used do define enumerator parameter with defined set of values
        /// </summary>
        private const string enumType = "enum";

        #endregion

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

        public static string GetTestName(string testId)
        {
            string name = null;
            if (TemplateRoot != null)
            {
                XElement test = TemplateRoot.Elements(TestElem)
                    .Where(t => t.Attribute(IdAttr) != null && t.Attribute(IdAttr).Value == testId)
                    .FirstOrDefault();
                if (test.Element(NameElem) != null)
                    name = test.Element(NameElem).Value;
            }

            return name;
        }
        public static string GetParamName(string testId, string paramId)
        {
            string name = null;
            if (TemplateRoot != null)
            {
                XElement test = TemplateRoot.Elements(TestElem)
                    .FirstOrDefault(t => t.Attribute(IdAttr) != null && t.Attribute(IdAttr).Value == testId);
                if (test != null)
                {
                    XElement param = test.Elements(ParamElem)
                        .FirstOrDefault(p => p.Attribute(IdAttr) != null && p.Attribute(IdAttr).Value == paramId);
                    if (param.Element(NameElem) != null)
                        name = param.Element(NameElem).Value;
                }
            }

            return name;
        }


        /// <summary>
        /// Create a new instance of <paramref name="TestValue"/> from xml description (this contains
        /// values of <paramref name="TestValue"/> instance in string format)
        /// <paramref name="TestValue"/> contains properties that may be set up on test by user, 
        /// collection of parameters and values that are displayed in user interface such as description
        /// of name of current test. In this method only empty instace of <paramref name="TestValue"/> 
        /// is created, parameters are added later.
        /// Throws exception if xml is in incorrect format
        /// </summary>
        /// <param name="tmplTest">Xml element that contains properties of <paramref name="TestValue"/>
        /// in string format</param>
        /// <returns>A new instance of <paramref name="TestValue"/> created from xml description</returns>
        private static TestValue getTestInstance(XElement tmplTest)
        {
            // create empty test just identified by unic id
            TestValue test = new TestValue(tmplTest.Attribute(IdAttr).Value);
            
            // set test properties - these values are independent on test values
            // short description of test
            test.Name = tmplTest.Element(NameElem).Value;
            // longer description of test - may be added as a tooltip on some user control
            test.Description = tmplTest.Element(DescriptionElem).Value;
            // all test are devided to groups according their funcionality
            test.GroupName = tmplTest.Element(GroupElem).Value;
            return test;
        }
        /// <summary>
        /// Create a new instance of <paramref name="ParamValue"/> from xml description (this contains
        /// values of <paramref name="ParamValue"/> instance in string format)
        /// <paramref name="ParamValue"/> contains properties that may be set up on parameter in any
        /// test by user, its default values and values that are displayed in user interface such as 
        /// description of name of current parameter.
        /// Throws exception if xml is in incorrect format
        /// </summary>
        /// <param name="tmplParam">Xml element that contains properties of <paramref name="ParamValue"/>
        /// in string format</param>
        /// <returns>A new instance of <paramref name="ParamValue"/> (or type derived from it) created
        /// from xml description</returns>
        /// <exception cref="System.FormatException">Format of input file is not recognized</exception>
        private static ParamValue getParamInstance(XElement tmplParam)
        {
            // use invariant culture to parse double values
            CultureInfo iCulture = CultureInfo.InvariantCulture;

            ParamValue param;
            XElement typeElem = tmplParam.Element(TypeElem);
            // name of parameter type (int, string, double, enum, ...)
            string type = typeElem.Value;
            // unique identifier of parameter inside a test, there can be parameter with same id but in a
            // different test
            string id = tmplParam.Attribute(IdAttr).Value;

            switch (type)
            {
                // when parsing numeric values throw an exception if number is not in correct format
                case intType:
                    param = new IntParam(id)
                    {
                        // minimal allowed value of this numeric parameter
                        MinValue = int.Parse(tmplParam.Element(MinElem).Value, iCulture),
                        // maximal allowed value of this numeric parameter
                        MaxValue = int.Parse(tmplParam.Element(MaxElem).Value, iCulture),
                        // description of unit of this numeric parameter
                        Unit = Units.UnitFromString(tmplParam.Element(UnitElem).Attribute(TypeAttr).Value)
                    };
                    break;
                case doubleType:
                    // get decimals attribute from type element - if it exists save number of allowed decimals
                    XAttribute dec = typeElem.Attribute(DecimalasAtrr);
                    int decimals = 1;
                    if (dec != null)
                        decimals = int.Parse(dec.Value, iCulture);
                    param = new DoubleParam(id)
                    {
                        // minimal allowed value of this numeric parameter
                        MinValue = double.Parse(tmplParam.Element(MinElem).Value, iCulture),
                        // maximal allowed value of this numeric parameter
                        MaxValue = double.Parse(tmplParam.Element(MaxElem).Value, iCulture),
                        // description of unit of this numeric parameter
                        Unit = Units.UnitFromString(tmplParam.Element(UnitElem).Attribute(TypeAttr).Value),
                        // number of allowed decimals of this numeric parameter
                        Decimals = decimals
                    };
                    break;
                case boolType:
                    param = new BoolParam(id);      // no other special properties
                    break;
                case stringType:
                    param = new StringParam(id);    // no other special properties
                    break;
                case enumType:
                    // get array of enumerator parameters
                    param = new EnumParam(id,
                        tmplParam.Element(ValuesElem).Elements(ValueElem).Select(el => el.Value).ToArray());
                    break;
                default: 
                    // unkown type of parameter
                    throw new ArgumentException("Unkown parameter type - " + type);
            }


            // these values are common for all parameter types
            // set test properties - these values are independent on parameter values
            // short description of parameter
            param.Name = tmplParam.Element(NameElem).Value;
            // longer description of parameter - may be added as a tooltip on some user control
            param.Description = tmplParam.Element(DescriptionElem).Value;
            // parse default param value from string
            param.ValueFromString(tmplParam.Element(ValueElem).Value);

            return param;
        }

        #endregion

        #region Input/Output

        /// <summary>
        /// Generate a new unic file name - used when creating a new file. This only default name with
        /// unique numeric sufix without direcotry name.
        /// </summary>
        public static string GetNewName()
        {
            return NewFileString + ++lastNewFileIndex;  // index of last created file name
        }

        /// <summary>
        /// Read a particular file from given path and return its content as o collection of tests.
        /// Throws an exception if given path does not exists or file is in incorrect format or currupted
        /// </summary>
        /// <param name="path">Path to file to read</param>
        /// <returns>Collection of <paramref name="TestValue"/> instances readed from given file</returns>
        public static TestCollection ReadFile(string path)
        {
            // notice that TestValue and ParamValue instaces do not know anythig about file format
            // this is the only place (also method for saving and creating new file) where the format of test 
            // collection file is described

            // load file to memory
            XElement fileRoot = XElement.Load(path);
            // load template to memory
            XElement tmplRoot = XElement.Load(Settings.Default.GetTemplatePath());
            // throw an exception if any of these two files does not exist

            // crate a new empty instance of test collection and add items from file
            TestCollection tc = new TestCollection();

            try     // catch exception while reading xml file - any mistake in input file
            {       // will be represented as an error in file format and would not be loaded                
                foreach (XElement tmplTest in tmplRoot.Elements(TestElem))
                {
                    // create instance of test from template test (do not consider test parameters)
                    TestValue tv = getTestInstance(tmplTest);
                    // get template test id attribute
                    string testId = tmplTest.Attribute(IdAttr).Value;
                    // in test file find test element with this id attribute (all other test elements that are
                    // not contained in template will be ignored)
                    XElement test = fileRoot.Elements(TestElem)
                        .FirstOrDefault(el => el.Attribute(IdAttr).Value == testId);

                    // if file doest not contain test element from template (older version) - must be added
                    if (test == null)
                    {
                        // get value indicating if test should be enabled by default
                        XAttribute enabAttr = tmplTest.Attribute(EnabledAttr);
                        string enabled = enabAttr == null ? "false" : enabAttr.Value;

                        // add test to file without any attributes
                        test = new XElement(TestElem,
                            new XAttribute(IdAttr, testId),
                            new XAttribute(EnabledAttr, enabled));
                    }
                    tv.Enabled = bool.Parse(test.Attribute(EnabledElem).Value);

                    foreach (XElement tmplParam in tmplTest.Elements(ParamElem))
                    {
                        // create an parameter instace acording its properties in xml template
                        ParamValue pv = getParamInstance(tmplParam);
                        // get template param id attribute
                        string paramId = tmplParam.Attribute(IdAttr).Value;
                        // in test file find element with this id attribute (all other param elements that are
                        // not contained in template will be ignored). When this test is not included in file
                        // it is created just now and it does not have any parameters - all will be added
                        XElement param = test.Elements(ParamElem)
                            .FirstOrDefault(pr => pr.Attribute(IdAttr).Value == paramId);

                        // string representation of parameter value
                        string value;

                        // parameter is not defined in file (test) - add its default value from template
                        if (param == null)
                            value = tmplParam.Element(ValueElem).Value;
                        else
                            value = param.Value;    // use value from file
                        
                        // initialize value of parameter
                        pv.ValueFromString(value);
                        // add created parameter to test instance
                        tv.AddParam(pv.ValueId, pv);
                    }

                    tc.AddTest(testId, tv);
                }
            }
            catch (Exception ex)
            {   // file could not be readed - it is an unkown format
                // this hides the impementation details (that file is serialized or xml), noone knows about the format
                // except of the FileManager
                if (ex is IOException)  // if it is IOException (file protection etc.) leave exception as it is, other wise
                    throw ex;           // generate exception telling that file format is corrupted
                throw new FileLoadException("File may be corrupted or is in incorrect format", 
                    Path.GetFileName(path), ex);
            }

            return tc;
        }
        /// <summary>
        /// Save a collection of test to a file in given path. File is overwrited if it already exists and
        /// created new if it does not.
        /// An exception is thrown if file could not be created or given collectio of test vlaues is corrupted
        /// </summary>
        /// <param name="path">Absolute path to the file, where collection will be saved</param>
        /// <param name="collection">Collection of tests to save</param>
        public static void SaveFile(string path, TestCollection collection)
        {
            // notice that TestValue and ParamValue instaces do not know anythig about file format
            // this is the only place (also method for reading and creating new file) where the format of test 
            // collection file is described

            // create xml tree
            XElement root = new XElement(TestsElem);
            foreach (TestValue test in collection)
            {
                // create test element with attributes id and enabled
                XElement testElem = new XElement(TestElem,
                    new XAttribute(IdAttr, test.ValueId),
                    new XAttribute(EnabledElem, test.Enabled));
                // add param childs elements to test
                foreach (ParamValue param in test)
                {
                    // create param element with id attribute and its string value representation
                    XElement paramElem = new XElement(ParamElem,
                        new XAttribute(IdAttr, param.ValueId),
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
        /// Create and return new default instance of test collection with default properties and defualt
        /// values of test parameters. This method use template file to create default instance. An exception
        /// is thrown if template does not exists.
        /// </summary>
        /// <returns>Collection of <paramref name="TestValue"/> loaded from template file initialized
        /// with default values</returns>
        public static TestCollection CreateNew()
        {
            // notice that TestValue and ParamValue instaces do not know anythig about file format
            // this is the only place (also method for saving and reading) where the format of test 
            // collection file is described

            //TODO: check for an exception (error in template file)

            // absolute path to firecotry where template file is stored
            string templatePath = Settings.Default.GetTemplatePath();
            // check if file exists - otherwise throw special exception so user may check configuration of
            // his application for bad settings
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Template file does not exists. Check application configuration.");

            // load template file - root element is <tests>
            XDocument template = XDocument.Load(templatePath);
            XElement tmplRoot = template.Root;

            // create a new empty instance of test collection
            TestCollection tc = new TestCollection();

            foreach (XElement tmplTest in tmplRoot.Elements(TestElem))
            {
                // get empty instance of test (without properties) with default properties
                TestValue tv = getTestInstance(tmplTest);

                // add parameters from template to just created TestValue instance
                foreach (XElement tmplParam in tmplTest.Elements(ParamElem))
                    tv.AddParam(getParamInstance(tmplParam));   // create parameter from xml description

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

        #region Error handling

        /// <summary>
        /// Handle exception that has been thrown while saving a file. This consists of recognizing type of
        /// exception and showing an apropriate message to user or rethrowing exception if it is not recognized
        /// for <paramref name="FileManager"/>
        /// </summary>
        /// <param name="ex">An instace of exception that was thrown</param>
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
        /// <summary>
        /// Handle exception that has been thrown while opening a file. This consists of recognizing type of
        /// exception and showing an apropriate message to user or rethrowing exception if it is not recognized
        /// for <paramref name="FileManager"/>
        /// </summary>
        /// <param name="ex">An instace of exception that was thrown</param>
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

        static FileManager()
        {
            try
            {
                TemplateRoot = XElement.Load(Settings.Default.GetTemplatePath());
            }
            catch (Exception ex)
            {
            }
        }
    }
}
