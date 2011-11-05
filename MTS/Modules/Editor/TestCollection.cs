using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MTS.Editor
{
    public class TestCollection : IEnumerable<TestValue>
    {
        private Dictionary<string, TestValue> tests = new Dictionary<string, TestValue>();

        #region Known Test Ids

        public const string Info = "Info";
        public const string TravelEast = "TravelEast";
        public const string TravelWest = "TravelWest";
        public const string TravelSouth = "TravelSouth";
        public const string TravelNorth = "TravelNorth";
        public const string Powerfold = "Powerfold";
        public const string Blinker = "Blinker";
        public const string Spiral = "Spiral";
        public const string Pulloff = "Pulloff";
        public const string Rubber = "Rubber";

        #endregion

        public void AddTest(TestValue test)
        {
            tests.Add(test.Id, test);
        }
        public void AddTest(string key, TestValue test)
        {
            tests.Add(key, test);
        }
        public TestValue GetTest(string key)
        {
            return tests[key];
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
        /// Returns an enumerator that iterates throught the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TestValue> GetEnumerator()
        {
            return tests.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates throught the collection
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
            
        }

        #endregion
    }
}
