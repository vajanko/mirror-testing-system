using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MTS.EditorModule
{
    [Serializable]
    public abstract class TestParamBase : INotifyPropertyChanged
    {        
        public const string ValueString = "Value";

        protected int key;
        public int TestKey
        {
            get { return key >> 16; }
        }
        public int ParamKey
        {
            get { return key & 0xffff; }
        }

        /// <summary>
        /// (Get/Set) Name (short description) of test parameter
        /// </summary>
        public string Name { get; set; }

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        private event PropertyChangedEventHandler propertyChanged;
        /// <summary>
        /// Occurs when property has been changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        /// <summary>
        /// Raises property changed event
        /// </summary>
        /// <param name="name">Name of the property changed</param>
        protected void OnPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
    [Serializable]
    public class TestParam<T> : TestParamBase
    {
        
        protected object value;
        /// <summary>
        /// (Get/Set) Value of the test parameter
        /// </summary>
        public T Value
        {
            get { return (T)value; }
            set { this.value = value; OnPropertyChanged(ValueString); }
        }
    }
    [Serializable]
    public class BoolParam : TestParam<bool> { }
    [Serializable]
    public class StringParam : TestParam<string> { }
    [Serializable]
    public class IntParam : TestParam<int> { }

}
