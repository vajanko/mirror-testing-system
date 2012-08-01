using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;


namespace MTS.Editor
{
    /// <summary>
    /// Collection of test and their parameters. This collection is used during shift execution when all
    /// enabled tests are executed. Test are accessed by unique id.
    /// </summary>
    public class TestCollection : IEnumerable<TestValue>
    {
        /// <summary>
        /// Collection of test identified by string id
        /// </summary>
        private readonly Dictionary<string, TestValue> tests = new Dictionary<string, TestValue>();

        /// <summary>
        /// (Get/Set) Test with given id
        /// </summary>
        /// <param name="key">Id of test to get</param>
        /// <returns>Test with specified id</returns>
        /// <exception cref="TestNotFoundException">Test with given id doesn't exist in current collection</exception>
        public TestValue this[string key]
        {
            get
            {
                try
                {
                    return tests[key];
                }
                catch (Exception ex)
                {
                    throw new TestNotFoundException(key, ex);
                }
            }
            set {
                tests[key] = value; 
            }
        }
        /// <summary>
        /// Gets value indicating whether a test with given id exists in current collection of tests.
        /// </summary>
        /// <param name="key">Unique identifier of the test</param>
        /// <returns>Value indicating whether test with given id exists in current collection of tests</returns>
        public bool ContainsTest(string key)
        {
            return tests.ContainsKey(key);
        }

        public void AddTest(TestValue test)
        {
            AddTest(test.ValueId, test);
        }
        public void AddTest(string key, TestValue test)
        {
            test.OrderIndex = tests.Count;
            tests.Add(key, test);
        }
        public TestValue GetTest(string key)
        {
            if (!tests.ContainsKey(key)) return null;
            return tests[key];
        }
        public void RemoveTest(string key)
        {
            tests.Remove(key);
        }
        public void RemoveTest(TestValue test)
        {
            RemoveTest(test.ValueId);
        }

        /// <summary>
        /// Set handler to be called when any of property of any test or parameter get changed 
        /// </summary>
        /// <param name="handler">Handler to be called</param>
        public void SetPropertyChangedHandler(PropertyChangedEventHandler handler)
        {
            // in each test and in each its parameter - set property changed handler
            foreach (var test in this)
                test.SetPropertyChangedHandler(handler);    // also will add handler on parameters
        }

        /// <summary>
        /// (Get) Value indicating whether collection of tests contained in this instance is of the same
        /// format as the application supports. If not should be converted when saving.
        /// </summary>
        public bool IsCurrentVersion { get; private set; }
        /// <summary>
        /// Setup this instance of tests collection to indicate that it is not in format that current application supports
        /// </summary>
        public void InvalidateCurrentVersion()
        {
            IsCurrentVersion = false;
        }

        #region IEnumerable<TestValue> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TestValue> GetEnumerator()
        {
            return tests.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)tests.Values).GetEnumerator();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new empty <see cref="TestCollection"/>
        /// </summary>
        public TestCollection()
        {
            IsCurrentVersion = true;
        }

        #endregion
    }
}
