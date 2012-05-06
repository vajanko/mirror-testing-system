﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;


namespace MTS.Editor
{
    /// <summary>
    /// TODO: update summary
    /// </summary>
    public class TestCollection : IEnumerable<TestValue>
    {
        private readonly Dictionary<string, TestValue> tests = new Dictionary<string, TestValue>();

        public TestValue this[string key]
        {
            get { return tests[key]; }
            set { tests[key] = value; }
        }
        public bool ContainsKey(string key)
        {
            return tests.ContainsKey(key);
        }
        public void AddTest(TestValue test)
        {
            tests.Add(test.ValueId, test);
        }
        public void AddTest(string key, TestValue test)
        {
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

        public TestCollection()
        {
            IsCurrentVersion = true;
        }

        #endregion
    }
}
