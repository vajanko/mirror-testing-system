using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace MTS.EditorModule
{
    [Serializable]
    public class ValueBase : INotifyPropertyChanged
    {
        public string Name { get { return (metadata != null) ? metadata.Name : string.Empty; } }

        [NonSerialized]
        private MetadataBase metadata;
        /// <summary>
        /// Metadata for this value. This data get not serialized
        /// </summary>
        public virtual MetadataBase Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        #region INotifyPropertyChanged

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
        /// <summary>
        /// Set handler to be called when any of property get changed
        /// </summary>
        /// <param name="handler">Handler to be called</param>
        public virtual void SetPropertyChangedHandler(PropertyChangedEventHandler handler)
        {
            PropertyChanged += handler;
        }

        #endregion
    }
    [Serializable]
    public class TestValue : ValueBase
    {
        public const string EnabledString = "Enabled";  // name of property - event raised when changed

        private bool enabled;   // will be serialized
        /// <summary>
        /// (Get/Set) True if test is enabled
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; OnPropertyChanged(EnabledString); }     // notify on change - for binding
        }

        /// <summary>
        /// (Get) All test are devide into several groups. Tests in one group have something in common and may
        /// be displayed together
        /// </summary>
        public string GroupName
        {
            get
            {   // return string from metadata if there are some
                TestMetadata meta = Metadata as TestMetadata;
                return (meta != null) ? meta.GroupName : string.Empty;
            }
        }

        /// <summary>
        /// (Get) Short description that is going to be displayed next do Enabled property graphical representation
        /// </summary>
        public string Text { get { return EnabledString; } }

        /// <summary>
        /// (Get/Set) Metadata for this value. This data get not serialized.
        /// Also set metadata for this test parameters. If any parameter is missing, it is automatically added
        /// to the parameters collection.
        /// </summary>
        public override MetadataBase Metadata
        {
            get { return base.Metadata; }
            set
            {
                // set metadata to this value
                base.Metadata = value;
                TestMetadata meta = Metadata as TestMetadata;
                if (meta != null)   // if this metadata is for a test - also set to the parameters
                {
                    // initialize metadata for each parameter
                    foreach (var key in parameters.Keys)
                        if (meta.Parameters.ContainsKey(key))
                            parameters[key].Metadata = meta.Parameters[key];
                    // add missing parameters
                    foreach (var key in meta.Parameters.Keys)
                        if (!parameters.ContainsKey(key))
                            parameters.Add(key, meta.Parameters[key].GetDefaultInstance());
                }
            }
        }

        private ParamCollection parameters;
        /// <summary>
        /// (Get/Set) Collection of parameters for this test
        /// </summary>
        public ParamCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }     // protected
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
            foreach (ValueBase value in parameters.Values)
                value.SetPropertyChangedHandler(handler);
        }

        #region Constructors

        public TestValue()
        {
            parameters = new ParamCollection();
        }

        #endregion
    }

    //[Serializable]
    //public class TravelValue : TestValue
    //{
    //    public int MinAngle
    //    {
    //        get { return (Parameters[ParamDictionary.MIN_ANGLE] as IntParamValue).Value; }
    //    }
    //    public int MaxTestingTime
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_TESTING_TIME] as IntParamValue).Value; }
    //    }
    //    public int MaxCurrent
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_CURRENT] as IntParamValue).Value; }
    //    }
    //    public int MaxOverloadTime
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_OVERLOAD_TIME] as IntParamValue).Value; }
    //    }
    //}
    //[Serializable]
    //public class PowerfoldValue : TestValue
    //{
    //    public bool TestPresence 
    //    {
    //        get { return (Parameters[ParamDictionary.TEST_PRESENCE] as BoolParamValue).Value; }
    //    }
    //    public int MaxCurrent
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_CURRENT] as IntParamValue).Value; }
    //    }
    //    public int MaxOverloadTime
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_OVERLOAD_TIME] as IntParamValue).Value; }
    //    }
    //    public int MaxTestingTime
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_TESTING_TIME] as IntParamValue).Value; }
    //    }
    //}
    //[Serializable]
    //public class BlinkerValue : TestValue
    //{
    //    public bool TestPresence
    //    {
    //        get { return (Parameters[ParamDictionary.TEST_PRESENCE] as BoolParamValue).Value; }
    //    }
    //    public int MinCurrent
    //    {
    //        get { return (Parameters[ParamDictionary.MIN_CURRENT] as IntParamValue).Value; }
    //    }
    //    public int MaxCurrent
    //    {
    //        get { return (Parameters[ParamDictionary.MAX_CURRENT] as IntParamValue).Value; }
    //    }
    //}
    //[Serializable]
    //public class SpiralValue : TestValue
    //{
    //}

    [Serializable]
    public class ParamValue<T> : ValueBase
    {
        public const string ValueString = "Value";  // name of property - event raised when changed

        private T value;    // will be serialized
        /// <summary>
        /// (Get/Set) Current value of the parameter
        /// </summary>
        public T Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(ValueString); }     // notify on change - for binding
        }
    }
    [Serializable]
    public class IntParamValue : ParamValue<int> { }
    [Serializable]
    public class DoubleParamValue : ParamValue<double> { }
    [Serializable]
    public class BoolParamValue : ParamValue<bool> 
    {
        public string Text
        {
            get
            {   // if metadata are data for bool param return Text, otherwise return empty string
                BoolParamMetadata meta = Metadata as BoolParamMetadata;
                return (meta != null) ? meta.Text : "";
            }
        }
    }
    [Serializable]
    public class StringParamValue : ParamValue<string> { }
    [Serializable]
    public class EnumParamValue : ParamValue<int> 
    {
        public string[] Values
        {
            get
            {
                EnumParamMetadata meta = (Metadata as EnumParamMetadata);
                return (meta != null) ? meta.Values : null;
            }
        }
        public string SelectedItem
        {
            get
            {
                EnumParamMetadata meta = (Metadata as EnumParamMetadata);
                return (meta != null) ? (meta.Values.Length > Value ? meta.Values[Value] : "") : "";
            }
        }
    }
}