using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Runtime.Serialization;

namespace MTS.EditorModule
{
    /// <summary>
    /// Base class for all metadata class. Metadata holds information about test or parameters
    /// which are not serialized
    /// </summary>
    public abstract class MetadataBase
    {
        /// <summary>
        /// (Get/Set) Name (short description) of metadata item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns default instance of Value (Test or Parameter) which this metadata describes
        /// </summary>
        public abstract ValueBase GetDefaultInstance();
    }
    public class TestMetadata : MetadataBase
    {
        /// <summary>
        /// (Get/Set) True if test is enabled by default
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// (Get/Set) Name of group where this test belongs
        /// </summary>
        public string GroupName { get; set; }

        private ParamDictionary parameters;
        /// <summary>
        /// (Get) Collection of parameters metadata for this test
        /// </summary>
        public ParamDictionary Parameters { get { return parameters; } }

        public override ValueBase GetDefaultInstance()
        {   // setting metadata property on TestValue will also add missing parameters
            return new TestValue { Enabled = this.Enabled, Metadata = this };
        }

        public TestMetadata()
        {
            parameters = new ParamDictionary();
        }
    }

    /// <summary>
    /// Generic base class for all Parameter metadata. Defines everything necessary for parameter metadata
    /// For using only inherit this class with your own type parameter and override GetDefaultInstance()
    /// </summary>
    /// <typeparam name="T">Type of value of the parameter</typeparam>
    public class ParamMetadata<T> : MetadataBase
    {
        /// <summary>
        /// (Get/Set) Default value of the parameter
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Returns default instance of Parameter value which this metadata describes
        /// </summary>
        public override ValueBase GetDefaultInstance()
        {
            return new ParamValue<T> { Value = this.Value, Metadata = this };
        }
    }

    public class IntParamMetadata : ParamMetadata<int>
    {
        public override ValueBase GetDefaultInstance()
        {
            return new IntParamValue { Value = this.Value, Metadata = this };
        }
    }
    public class DoubleParamMetadata : ParamMetadata<double>
    {
        public override ValueBase GetDefaultInstance()
        {
            return new DoubleParamValue { Value = this.Value, Metadata = this };
        }
    }
    public class BoolParamMetadata : ParamMetadata<bool>
    {
        /// <summary>
        /// (Get/Set) Short description of bool parameter. This peace of text is usually displayed
        /// inside the check box
        /// </summary>
        public string Text { get; set; }

        public override ValueBase GetDefaultInstance()
        {
            return new BoolParamValue { Value = this.Value, Metadata = this };
        }
    }
    public class StringParamMetadata : ParamMetadata<string> 
    {
        public override ValueBase GetDefaultInstance()
        {
            return new StringParamValue { Value = this.Value, Metadata = this };
        }
    }
    public class EnumParamMetadata : ParamMetadata<int>
    {
        /// <summary>
        /// (Get/Set) Collection of possible values
        /// </summary>
        public string[] Values { get; set; }

        public override ValueBase GetDefaultInstance()
        {
            return new EnumParamValue { Value = this.Value, Metadata = this };
        }
    }
}
