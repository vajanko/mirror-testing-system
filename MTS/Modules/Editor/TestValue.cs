using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;

namespace MTS.Editor
{
    public class TestValue : ValueBase, IEnumerable<ParamValue>
    {
        private readonly Dictionary<string, ParamValue> parameters = new Dictionary<string, ParamValue>();

        #region Known Parameters Ids

        public const string MinAngle = "MinAngle";
        public const string MinCurrent = "MinCurrent";
        public const string MaxCurrent = "MaxCurrent";
        public const string MaxOverloadTime = "MaxOverloadTime";
        public const string MaxTestingTime = "MaxTestingTime";
        public const string TestPresence = "TestPresence";

        public const string SupplierName = "SupplierName";
        public const string SupplierCode = "SupplierCode";
        public const string PartNumber = "PartNumber";        
        public const string DescriptionId = "Description";
        public const string Weight = "Weight";
        public const string Orientation = "Orientation";

        // direction light
        public const string LighteningTime = "LightingTime";
        public const string BreakTime = "BreakTime";
        public const string BlinkCount = "BlinksCount";

        public const string TestingTime = "TestingTime";

        #endregion

        /// <summary>
        /// Constant string "Enabled"
        /// </summary>
        public const string EnabledString = "Enabled";

        /// <summary>
        /// (Get/Set) Name of group this test belongs to
        /// </summary>
        public string GroupName { get; set; }

        private bool _enabled;
        /// <summary>
        /// (Get/Set) True if test is enabled
        /// </summary>
        public bool Enabled 
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(EnabledString); }
        }

        #region Parameters

        public ParamValue this[string key]
        {
            get { return parameters[key]; }
            set { parameters[key] = value; }
        }
        public void AddParam(ParamValue param)
        {
            parameters.Add(param.ValueId, param);
        }
        public void AddParam(string key, ParamValue param)
        {
            parameters.Add(key, param);
        }
        //public ParamValue GetParam(string key)
        //{
        //    return parameters[key];
        //}
        /// <summary>
        /// Finds parameter value in collection of parameters in this test identified by its key.
        /// Return null if it doesn't exists
        /// </summary>
        /// <typeparam name="T">Type of parameter value</typeparam>
        /// <param name="key">Name (identifier) of required parameter</param>
        /// <exception cref="ParamNotFoundException">Parameter with given key was not found</exception>
        /// <returns>Value of required parameter of type <typeparamref name="T"/></returns>
        public T GetParam<T>(string key) where T : ParamValue
        {
            if (parameters.ContainsKey(key))
                return parameters[key] as T;
            else throw new ParamNotFoundException(key);
        }
        public bool ContainsParam(string key)
        {
            return parameters.ContainsKey(key);
        }

        /// <summary>
        /// Set handler to be called when any of property get changed
        /// </summary>
        /// <param name="handler">Handler to be called</param>
        public override void SetPropertyChangedHandler(PropertyChangedEventHandler handler)
        {
            // set change handler on this test
            base.SetPropertyChangedHandler(handler);
            // and on all its parameters
            foreach (ParamValue param in this)
                param.SetPropertyChangedHandler(handler);
        }

        #region IEnumerable<ParamValue> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ParamValue> GetEnumerator()
        {
            return parameters.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)parameters.Values).GetEnumerator();
        }

        #endregion

        #endregion

        #region Constructros

        public TestValue(string id) : base(id) { }

        #endregion
    }
}
