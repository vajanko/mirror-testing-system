using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;

namespace MTS.Editor
{
    public class TestCollection : IEnumerable<TestValue>
    {
        private readonly Dictionary<string, TestValue> tests = new Dictionary<string, TestValue>();

        #region Known Test Ids

        public const string Info = "Info";
        public const string TravelEast = "TravelEast";
        public const string TravelWest = "TravelWest";
        public const string TravelSouth = "TravelSouth";
        public const string TravelNorth = "TravelNorth";
        public const string Powerfold = "Powerfold";
        public const string DirectionLight = "DirectionLight";
        public const string Heating = "HeatingFoil";
        public const string Pulloff = "Pulloff";
        public const string Rubber = "Rubber";

        #endregion

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
    }
}
