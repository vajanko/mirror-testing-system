using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using MTS.Base;

namespace MTS.Editor
{
    class TestXmlParser
    {
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

        #region Xml Format Constants

        /// <summary>
        /// Element that holds collection of test elements
        /// </summary>
        private readonly XName TestsElem = "tests";

        /// <summary>
        /// Element that holds value of test or parameter name. This is short description of test or
        /// parameter and is used in user interface
        /// </summary>
        private readonly XName NameElem = "name";
        /// <summary>
        /// Element that holds value of test or parameter description. This is longer peace of test describing
        /// in details current test or parameter. Could be used in user interface f.e. in a tooltip
        /// </summary>
        private readonly XName DescriptionElem = "description";
        /// <summary>
        /// Unique Id attribute of test or parameter
        /// </summary>
        private readonly XName IdAttr = "id";
        /// <summary>
        /// Attribute that holds value indicating if test is enabled or disabled
        /// </summary>
        private readonly XName EnabledAttr = "enabled";
        /// <summary>
        /// Attribute that holds value indicating whether testing will be aborted if this test 
        /// </summary>
        private readonly XName AbortOnFailAttr = "abort";
        /// <summary>
        /// Element that holds name of group to which test which contains this element belongs
        /// </summary>
        private readonly XName GroupElem = "group";
        /// <summary>
        /// Element that holds value of test properties
        /// </summary>
        private readonly XName TestElem = "test";


        #region Param

        /// <summary>
        /// Element that holds value of parameter properties
        /// </summary>
        private readonly XName ParamElem = "param";
        /// <summary>
        /// Element that holds default value of a parameter
        /// </summary>
        private readonly XName ValueElem = "value";      // !!
        /// <summary>
        /// Element that holds value of parameter type
        /// </summary>
        private readonly XName TypeElem = "type";
        /// <summary>
        /// Element that holds value of describing unit of parameter value
        /// </summary>
        private readonly XName UnitElem = "unit";
        /// <summary>
        /// Attribute that holds value of unit of parameter value
        /// </summary>
        private readonly XName TypeAttr = "type";
        /// <summary>
        /// Attribute that holds number of allowed decimals of parameter value
        /// </summary>
        private readonly XName DecimalasAtrr = "decimals";
        /// <summary>
        /// Element that holds value indicating if test is enabled
        /// </summary>
        private readonly XName EnabledElem = "enabled";
        /// <summary>
        /// Element that holds minimal possible value of parameter
        /// </summary>
        private readonly XName MinElem = "min";
        /// <summary>
        /// Element that holds maximal possible value of parameter
        /// </summary>
        private readonly XName MaxElem = "max";

        /// <summary>
        /// Element that holds possible values of enumerator parameter
        /// </summary>
        private readonly XName ValuesElem = "values";

        #endregion

        #endregion

        /// <summary>
        /// Visitor used to convert parameter string representation to strongly typed value
        /// </summary>
        private readonly FromStringVisitor fromString = new FromStringVisitor();
        /// <summary>
        /// Visitor used to convert parameter to string representation
        /// </summary>
        private readonly ToStringVisitor toString = new ToStringVisitor();

        /// <summary>
        /// Creates an instance of parameter value parsing it from given xml structure
        /// </summary>
        /// <param name="xmlParam">Xml template of parameter value</param>
        /// <returns>Parsed instance of parameter</returns>
        public ParamValue ParseParam(XElement xmlParam)
        {
            // use invariant culture to parse double values
            CultureInfo iCulture = CultureInfo.InvariantCulture;

            ParamValue param;
            XElement typeElem = xmlParam.Element(TypeElem);
            // name of parameter type (int, string, double, enum, ...)
            string type = typeElem.Value;
            // unique identifier of parameter inside a test, there can be parameter with same id but in a different test
            string id = xmlParam.Attribute(IdAttr).Value;
            // parameter value in string representation
            string value = xmlParam.Element(ValueElem).Value;

            switch (type)
            {
                // when parsing numeric values throw an exception if number is not in correct format
                case intType:
                    param = new IntParam(id)
                    {
                        // minimal allowed value of this numeric parameter
                        MinValue = int.Parse(xmlParam.Element(MinElem).Value, iCulture),
                        // maximal allowed value of this numeric parameter
                        MaxValue = int.Parse(xmlParam.Element(MaxElem).Value, iCulture),
                        // description of unit of this numeric parameter
                        Unit = Units.UnitFromString(xmlParam.Element(UnitElem).Attribute(TypeAttr).Value)
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
                        MinValue = double.Parse(xmlParam.Element(MinElem).Value, iCulture),
                        // maximal allowed value of this numeric parameter
                        MaxValue = double.Parse(xmlParam.Element(MaxElem).Value, iCulture),
                        // description of unit of this numeric parameter
                        Unit = Units.UnitFromString(xmlParam.Element(UnitElem).Attribute(TypeAttr).Value),
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
                        xmlParam.Element(ValuesElem).Elements(ValueElem).Select(el => el.Value).ToArray());
                    break;
                default:
                    // unknown type of parameter
                    throw new ArgumentException("Unknown parameter type - " + type);
            }

            param.Name = xmlParam.Element(NameElem).Value;
            param.Description = xmlParam.Element(DescriptionElem).Value;
            fromString.ConvertFromString(param, value);

            return param;
        }
        /// <summary>
        /// Creates an instance of test value (and its parameters) parsing it from given xml structure
        /// </summary>
        /// <param name="xmlTest">Xml template of test and its parameters</param>
        /// <returns>Parsed instance of test and its parameters</returns>
        public TestValue ParseTest(XElement xmlTest)
        {
            // create empty test just identified by unique id
            TestValue test = new TestValue(xmlTest.Attribute(IdAttr).Value);

            // set test properties - these values are independent on test values
            // short description of test
            test.Name = xmlTest.Element(NameElem).Value;
            // longer description of test - may be added as a tooltip on some user control
            test.Description = xmlTest.Element(DescriptionElem).Value;
            // all test are divided to groups according their functionality
            test.GroupName = xmlTest.Element(GroupElem).Value;
            // parser value indicating whether this test is enabled by default (default is true)
            XAttribute enAttr = xmlTest.Attribute(EnabledAttr);
            test.Enabled = enAttr == null ? true : bool.Parse(enAttr.Value);

            // parse all test parameters
            foreach (XElement xmlParam in xmlTest.Elements(ParamElem))
            {   // parse parameter and add it to current test
                ParamValue param = ParseParam(xmlParam);
                test.AddParam(param);
            }

            return test;
        }
        /// <summary>
        /// Create an instance of test collection parsing it from given structure - root element containing a
        /// collection of tests
        /// </summary>
        /// <param name="rootElem">Xml root element containing the entire collection of tests</param>
        /// <returns>Parsed collection of tests</returns>
        public TestCollection ParseCollection(XElement rootElem)
        {
            TestCollection tc = new TestCollection();
            foreach (XElement xmlTest in rootElem.Elements(TestElem))
            {   // parse test and add it to current collection
                TestValue test = ParseTest(xmlTest);
                // add test to collection
                tc.AddTest(test);
            }

            return tc;
        }

        /// <summary>
        /// Converts parameter value to xml representation
        /// </summary>
        /// <param name="param">Parameter value to convert to xml</param>
        /// <returns>Single xml element containing parameter value and its id</returns>
        public XElement ParamToXml(ParamValue param)
        {   // convert parameter value to string representation
            string value = toString.ConvertToString(param);
            // create a single element containing parameter id (as attribute) and its value (as a string)
            return new XElement(ParamElem,
                    new XAttribute(IdAttr, param.ValueId),
                    value);
        }
        /// <summary>
        /// Converts test value and its parameters to xml representation
        /// </summary>
        /// <param name="test">Test value to convert to xml</param>
        /// <returns>Xml element containing test and its parameters value</returns>
        public XElement TestToXml(TestValue test)
        {
            // create test element with attributes id and enabled
            XElement result = new XElement(TestElem,
                new XAttribute(IdAttr, test.ValueId),
                new XAttribute(EnabledElem, test.Enabled),
                new XAttribute(AbortOnFailAttr, test.AbortOnFail));

            // add param child's elements to test
            foreach (ParamValue param in test)
                result.Add(ParamToXml(param));

            return result;
        }
        /// <summary>
        /// Convert collection of test to xml representation which can be saved directly to a file
        /// </summary>
        /// <param name="tests">Collection of tests to be converted to xml representation</param>
        /// <returns>Xml root element containing xml representation of all given test and their parameters</returns>
        public XElement CollectionToXml(TestCollection tests)
        {
            // create tests root element containing all test elements
            XElement root = new XElement(TestsElem);

            // convert each test to xml and add it to root
            foreach (TestValue test in tests)
                root.Add(TestToXml(test));

            return root;
        }

        /// <summary>
        /// Loads parameter from given xml structure. An existing instance of parameter is passed
        /// </summary>
        /// <param name="param">Instance of parameter created from template with default values</param>
        /// <param name="xmlParam">Value of parameter in xml format</param>
        public void LoadParam(ParamValue param, XElement xmlParam)
        {
            fromString.ConvertFromString(param, xmlParam.Value);
        }
        /// <summary>
        /// Loads test from xml structure. An existing instance of test is passed.
        /// </summary>
        /// <param name="test">Instance of test created from template with default values</param>
        /// <param name="xmlTest">Value of test (and its parameters) save in xml format</param>
        public void LoadTest(TestValue test, XElement xmlTest)
        {
            // initialize test properties
            XAttribute enAttr = xmlTest.Attribute(EnabledAttr);
            test.Enabled = enAttr == null ? false : bool.Parse(enAttr.Value);
            XAttribute abAttr = xmlTest.Attribute(AbortOnFailAttr);
            test.AbortOnFail = abAttr == null ? false : bool.Parse(abAttr.Value);

            // load value of all parameters
            foreach (ParamValue param in test)
            {   // get saved value of parameter from xml with current value id
                XElement xmlParam = xmlTest.Elements(ParamElem).FirstOrDefault(pe => pe.Attribute(IdAttr).Value == param.ValueId);
                // Notice that is may happen that there is no such a parameter in given file. Important is what is in the 
                // template. If there is no parameter in template - will be ignored. And if there is a parameter in template
                // on not in file, will be added with default value
                if (xmlParam != null)
                    LoadParam(param, xmlParam);
            }
        }
        /// <summary>
        /// Loads collection of tests from given xml tree using template xml tree. Tests and parameters that are not
        /// present in given xml but in template will be added. On the other hand value contained in xml tree and
        /// not in template will be ignored
        /// </summary>
        /// <param name="templateRoot">Root element of template xml tree</param>
        /// <param name="fileRoot">Root element of xml tree loaded form file</param>
        /// <returns>Instance of collection of tests</returns>
        public TestCollection LoadCollection(XElement templateRoot, XElement fileRoot)
        {   // create a default collection of tests and then initialize their values
            TestCollection tc = ParseCollection(templateRoot);

            // load value of all tests
            foreach (TestValue test in tc)
            {   // get saved value of test from xml with current value id
                XElement xmlTest = fileRoot.Elements(TestElem).FirstOrDefault(te => te.Attribute(IdAttr).Value == test.ValueId);
                // notice that it may happen that there is no such a test in given file. Important is what is in the template
                // If there is no test in template - will be ignored. And if there is a test in template and not in file,
                // will be added, with default values and parameters
                if (xmlTest != null)
                    LoadTest(test, xmlTest);
            }

            return tc;
        }
    }
}
