using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MTS.EditorModule
{
    /// <summary>
    /// Base class for all key (integer) collections. Serialization possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class KeyCollection<T> : Dictionary<int, T>
    {
        #region Constructors

        protected KeyCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)  { /* necessary for Dictionary serialization */ }
        public KeyCollection() : base() { }

        #endregion
    }
    /// <summary>
    /// Tests dictionary is used to add instances od tests metadata as read-only constatnts. Each test
    /// type is created and added only once. Then this test metadata may be referenced.
    /// Each test metadata is identified by an unic key - tests may be added in future versions of the application,
    /// but keys shall not be changed.
    /// </summary>
    public class TestDictionary : KeyCollection<TestMetadata>
    {
        #region Constants

        public const int INFO = 0x01;

        public const int TRAVEL_EAST = 0x02;
        public const int TRAVEL_WEST = 0x03;
        public const int TRAVEL_SOUTH = 0x04;
        public const int TRAVEL_NORTH = 0x05;
        public const int POWERFOLD = 0x06;
        public const int BLINKER = 0x07;
        public const int SPIRAL = 0x08;
        public const int WIRE = 0x09;
        public const int THERMOMETER = 0x10;
        public const int MIRROR_GLASS = 0x11;
        public const int BOUDEN = 0x12;

        #endregion

        #region Properties

        private ParamDictionary parameters;
        /// <summary>
        /// (Get) Parameters dictionary for this test dictionary. Contains all parameters that any of tests included
        /// in test dictionary may use.
        /// </summary>
        public ParamDictionary Parameters { get { return parameters; } }

        #endregion

        /// <summary>
        /// Add reference to a particular parameter to parameter dictinary. Referenced parameter must be present in
        /// the main dictionary - <paramref name="Parameters"/>
        /// </summary>
        /// <param name="dic">Parameters dictionary where reference to some parameter will be added</param>
        /// <param name="key">Identifier of the referenced parameter. This is a key to to main parameters dictionary
        /// - <paramref name="Parameters"/></param>
        private void add(ParamDictionary dic, int key)
        {
            dic.Add(key, parameters[key]);
        }
        /// <summary>
        /// Add values to the metadata dictionary
        /// </summary>
        public void InitializeDefault()
        {
            // create an instance of parameter dictionary
            parameters = new ParamDictionary();
            // initialize with default value - this is the only time when when parameters are added to
            // the dictionary. Next times they are referenced only.
            parameters.InitializeDefault();

            TestMetadata test;
            ParamDictionary param;

            // initialize information
            test = new TestMetadata { Name = "Information", Enabled = true, GroupName = "Info" };
            param = test.Parameters;
            add(param, ParamDictionary.SUPPLIER_NAME);
            add(param, ParamDictionary.SUPPLIER_CODE);
            add(param, ParamDictionary.PART_NUMBER);
            add(param, ParamDictionary.DESCRIPTION);
            add(param, ParamDictionary.WEIGHT);
            add(param, ParamDictionary.ORIENTATION);
            this.Add(INFO, test);

            // initialize travel east test
            test = new TestMetadata { Name = "Travel east", Enabled = true, GroupName = "Travel" };
            param = test.Parameters;
            add(param, ParamDictionary.MIN_ANGLE);
            add(param, ParamDictionary.MAX_TESTING_TIME);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.MAX_OVERLOAD_TIME);
            this.Add(TRAVEL_EAST, test);

            // initialize travel west test
            test = new TestMetadata { Name = "Travel west", GroupName = "Travel" };
            param = test.Parameters;
            add(param, ParamDictionary.MIN_ANGLE);
            add(param, ParamDictionary.MAX_TESTING_TIME);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.MAX_OVERLOAD_TIME);
            this.Add(TRAVEL_WEST, test);

            // initialize travel west test
            test = new TestMetadata { Name = "Travel north", GroupName = "Travel" };
            param = test.Parameters;
            add(param, ParamDictionary.MIN_ANGLE);
            add(param, ParamDictionary.MAX_TESTING_TIME);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.MAX_OVERLOAD_TIME);
            this.Add(TRAVEL_NORTH, test);

            // initialize travel west test
            test = new TestMetadata { Name = "Travel south", GroupName = "Travel" };
            param = test.Parameters;
            add(param, ParamDictionary.MIN_ANGLE);
            add(param, ParamDictionary.MAX_TESTING_TIME);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.MAX_OVERLOAD_TIME);
            this.Add(TRAVEL_SOUTH, test);

            // initialize powerfold test
            test = new TestMetadata { Name = "Powerfold", GroupName = "Current" };
            param = test.Parameters;
            add(param, ParamDictionary.TEST_PRESENCE);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.MAX_OVERLOAD_TIME);
            add(param, ParamDictionary.MAX_TESTING_TIME);
            this.Add(POWERFOLD, test);

            // initialize blinker test
            test = new TestMetadata { Name = "Blinker", GroupName = "Current" };
            param = test.Parameters;
            add(param, ParamDictionary.TEST_PRESENCE);
            add(param, ParamDictionary.MIN_CURRENT);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.BLINK_COUNT);
            add(param, ParamDictionary.LIGHTENING_TIME);
            add(param, ParamDictionary.BREAK_TIME);
            this.Add(BLINKER, test);

            // initialize spiral test
            test = new TestMetadata { Name = "Spiral", GroupName = "Current" };
            param = test.Parameters;
            add(param, ParamDictionary.TEST_PRESENCE);
            add(param, ParamDictionary.MIN_CURRENT);
            add(param, ParamDictionary.MAX_CURRENT);
            add(param, ParamDictionary.TESTING_TIME);
            this.Add(SPIRAL, test);
        }
    }
    /// <summary>
    /// Parameters dictionary is used to add instances of parameters metadata as read-only constants. Each parameter
    /// type is created and added only once. Than this parameter metadata may be referenced.
    /// Each parameters metadata is identified by an unic key - parameters may be added in future versions of the
    /// application, but keys shall not be changed.
    /// </summary>
    public class ParamDictionary : KeyCollection<MetadataBase>
    {
        #region Constants

        public const int MIN_ANGLE = 0x00;
        public const int MIN_CURRENT = 0x01;
        public const int MAX_CURRENT = 0x02;
        public const int MAX_OVERLOAD_TIME = 0x03;
        public const int MAX_TESTING_TIME = 0x04;
        public const int TEST_PRESENCE = 0x05;

        public const int SUPPLIER_NAME = 0x06;
        public const int SUPPLIER_CODE = 0x07;
        public const int PART_NUMBER = 0x08;
        public const int DESCRIPTION = 0x09;
        public const int WEIGHT = 0x10;
        public const int ORIENTATION = 0x11;

        public const int LIGHTENING_TIME = 0x12;
        public const int BREAK_TIME = 0x13;
        public const int BLINK_COUNT = 0x14;

        public const int SENSOR = 0x15;
        public const int GLASS_TYPE = 0x16;
        public const int COLOR = 0x17;

        public const int TESTING_TIME = 0x18;

        #endregion

        /// <summary>
        /// Add values to the metadata dictionary
        /// </summary>
        public void InitializeDefault()
        {
            this.Add(MIN_ANGLE, new DoubleParamMetadata { Name = "Min angle" });
            this.Add(MIN_CURRENT, new DoubleParamMetadata { Name = "Min current" });
            this.Add(MAX_CURRENT, new DoubleParamMetadata { Name = "Max current" });
            this.Add(MAX_OVERLOAD_TIME, new IntParamMetadata { Name = "Max overload time" });
            this.Add(MAX_TESTING_TIME, new IntParamMetadata { Name = "Max testing time" });
            this.Add(TEST_PRESENCE, new BoolParamMetadata { Name = "Presence", Text = "Check" });
            this.Add(SUPPLIER_NAME, new StringParamMetadata { Name = "Supplier name" });
            this.Add(SUPPLIER_CODE, new StringParamMetadata { Name = "Supplier code" });
            this.Add(PART_NUMBER, new StringParamMetadata { Name = "Part number" });
            this.Add(DESCRIPTION, new StringParamMetadata { Name = "Description" });
            this.Add(WEIGHT, new IntParamMetadata { Name = "Weight" });
            this.Add(ORIENTATION, new EnumParamMetadata { Name = "Orientation", Values = new string[] { "Left", "Right" } });
            this.Add(LIGHTENING_TIME, new IntParamMetadata { Name = "Lightening time" });
            this.Add(BREAK_TIME, new IntParamMetadata { Name = "Break time" });
            this.Add(BLINK_COUNT, new IntParamMetadata { Name = "Number of blinks" });
            this.Add(SENSOR, new BoolParamMetadata { Name = "Sensor" });
            this.Add(GLASS_TYPE, new EnumParamMetadata { Name = "Type of glass", Values = new string[] { "Spheric", "Aspheric" } });
            this.Add(TESTING_TIME, new IntParamMetadata { Name = "Testing time" });
        }
    }

    [Serializable]
    public class TestCollection : KeyCollection<TestValue>
    {
        /// <summary>
        /// This dictionary contains metadata for the test and parameter values. This data get not serialized.
        /// They are specific for the application version
        /// </summary>
        [NonSerialized]
        static private readonly TestDictionary metadata;

        /// <summary>
        /// Asociate each test and parameter with its metadata. This method must be called manually after 
        /// deserialization. 
        /// </summary>
        public void InitializeMetadata()
        {
            // initialize metadata for each test
            foreach (var key in this.Keys)
                if (metadata.ContainsKey(key))
                    // this also initialize parameters metadata
                    // and add missing parameters
                    this[key].Metadata = metadata[key];
            // add missing tests
            foreach (var key in metadata.Keys)  // metadata collection contains a key
                if (!this.ContainsKey(key))     // but it is not in test collection
                {   // metadata must be for test (not for parameter)
                    TestValue test = metadata[key].GetDefaultInstance() as TestValue;
                    if (test != null)   // test is null when metadata is not for test (but parameter)
                        this.Add(key, test);
                }
        }
        /// <summary>
        /// Set hander that will be executed when any of test property or test parameters property change
        /// </summary>
        /// <param name="handler">Handler to be executed on property change</param>
        public void SetPropertyChangedHandler(PropertyChangedEventHandler handler)
        {
            // in each test and in each its parameter - set property changed handler
            foreach (var test in this.Values)
                test.SetPropertyChangedHandler(handler);    // also will add handler on parameters
        }

        #region Constructors

        public TestCollection()
        {   // when creating a new instance of TestCollection metadata are initialized automatically
            // after deserialization this method must be called manully
            InitializeMetadata();
        }
        protected TestCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // necessary to deserialize Dictionary
        }

        static TestCollection()
        {
            // create a read-only test dictionary and initialize it with tests and parameters metadata
            metadata = new TestDictionary();
            metadata.InitializeDefault();       // this method is called only once in this application
        }

        #endregion
    }

    [Serializable]
    public class ParamCollection : KeyCollection<ValueBase>
    {
        public ParamCollection() { }

        protected ParamCollection(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}