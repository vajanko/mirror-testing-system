using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;

namespace MTS.Editor
{
    /// <summary>
    /// Class that encapsulate configuration of a single test. Contains collection of
    /// parameters.
    /// </summary>
    public class TestValue : ValueBase, IEnumerable<ParamValue>
    {
        /// <summary>
        /// Collection of test parameters accessible by parameter unique id
        /// </summary>
        private readonly Dictionary<string, ParamValue> parameters = new Dictionary<string, ParamValue>();

        /// <summary>
        /// Constant string "Enabled". Use this when checking <see cref="PropertyChanged"/> event
        /// </summary>
        public const string EnabledString = "Enabled";
        /// <summary>
        /// Constant string "AbortOnFail". Use this when checking <see cref="PropertyChanged"/> event
        /// </summary>
        public const string AbortOnFailString = "AbortOnFail";

        /// <summary>
        /// (Get/Set) Name of group this test belongs to.
        /// </summary>
        public string GroupName { get; set; }

        private bool _enabled;
        /// <summary>
        /// (Get/Set) Value indicating whether test is enabled and will be executed during testing
        /// </summary>
        public bool Enabled 
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(EnabledString); }
        }
        private bool _abortOnFail;
        /// <summary>
        /// (Get/Set) Value indicating whether testing should be aborted if this test does not finish correctly.
        /// </summary>
        public bool AbortOnFail
        {
            get { return _abortOnFail; }
            set { _abortOnFail = value; OnPropertyChanged(AbortOnFailString); }
        }

        /// <summary>
        /// Call visitor method on this instance of test value adding new functions
        /// </summary>
        /// <param name="visitor">Instance of visitor adding new function to test value</param>
        public override void Accept(IValueVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Creates a deep copy of <see cref="IntParam"/> instance
        /// </summary>
        /// <returns>New instance of <see cref="IntParam"/> class</returns>
        public override object Clone()
        {
            // clone test instance
            TestValue test = new TestValue(ValueId)
            {
                DatabaseId = this.DatabaseId,
                Name = this.Name,
                Description = this.Description,
                Enabled = this.Enabled,
                AbortOnFail = this.AbortOnFail
            };
            // clone all parameters and add them to just created test instance
            foreach (var param in parameters.Values)
                test.AddParam(param.Clone() as ParamValue);

            return test;
        }

        #region Parameters

        public ParamValue this[string key]
        {
            get { return parameters[key]; }
            set { parameters[key] = value; }
        }
        public void AddParam(ParamValue param)
        {
            AddParam(param.ValueId, param);
        }
        public void AddParam(string key, ParamValue param)
        {
            param.OrderIndex = parameters.Count;
            parameters.Add(key, param);
        }
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
        /// Set handler to be called when any of property get changed.
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

        /// <summary>
        /// Initialize a new instance of test.
        /// </summary>
        /// <param name="id">Unique test identifier across the application.</param>
        public TestValue(string id) : base(id) { }

        #endregion
    }
}
