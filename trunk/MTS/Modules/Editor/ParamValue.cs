using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using MTS.Data.Types;

namespace MTS.Editor
{
    public abstract class ParamValue : ValueBase
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";
        /// <summary>
        /// (Get) Value of parameter
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Convert parameter value to string representation using invariant culture info
        /// </summary>
        /// <returns></returns>
        public virtual string ValueToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", Value);
        }
        public abstract void ValueFromString(string value);
        public abstract ParamType ValueType();

        #region Constructors

        public ParamValue(string id) : base(id) { }

        #endregion
    }

    public class IntParam : ParamValue
    {
        /// <summary>
        /// Constant string "IntValue"
        /// </summary>
        public const string IntValueString = "IntValue";

        public int IntValue
        {
            get { return (int)Value; }
            set { Value = value; OnPropertyChanged(IntValueString); }
        }
        /// <summary>
        /// (Get) Minimal allowed value of this parameter
        /// </summary>
        public int MinValue { get; set; }
        /// <summary>
        /// (Get) Maximal allowed value of this parameter
        /// </summar
        public int MaxValue { get; set; }
        /// <summary>
        /// (Get/Set) Unit of this parameter value
        /// </summary>
        public Unit Unit { get; set; }

        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = int.Parse(value, CultureInfo.InvariantCulture);
        }
        public override ParamType ValueType()
        {
            return ParamType.Int;
        }

        #region Constructors

        public IntParam(string id) : base(id) { }

        #endregion
    }

    public class DoubleParam : ParamValue
    {
        /// <summary>
        /// Constant string "DoubleValue"
        /// </summary>
        public const string DoubleValueString = "DoubleValue";

        public double DoubleValue 
        { 
            get { return (double)Value; }
            set { Value = value; OnPropertyChanged(DoubleValueString); }
        }
        /// <summary>
        /// (Get) Minimal allowed value of this parameter
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// (Get) Maximal allowed value of this parameter
        /// </summar
        public double MaxValue { get; set; }
        /// <summary>
        /// (Get/Set) Unit of this parameter value
        /// </summary>
        public Unit Unit { get; set; }

        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = double.Parse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }
        public override ParamType ValueType()
        {
            return ParamType.Double;
        }

        #region Constructors

        public DoubleParam(string id) : base(id) { }

        #endregion
    }

    public class BoolParam : ParamValue
    {
        /// <summary>
        /// Constant string "BoolValue"
        /// </summary>
        public const string BoolValueString = "BoolValue";

        public bool BoolValue
        { 
            get { return (bool)Value; }
            set { Value = value; OnPropertyChanged(BoolValueString); }
        }

        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = bool.Parse(value);
        }
        public override ParamType ValueType()
        {
            return ParamType.Bool;
        }

        #region Constructors

        public BoolParam(string id) : base(id) { }

        #endregion
    }

    public class StringParam : ParamValue
    {
        /// <summary>
        /// Constant string "StringValue"
        /// </summary>
        public const string StringValueString = "StringValue";

        public string StringValue 
        { 
            get { return (string)Value; }
            set { Value = value; OnPropertyChanged(StringValueString); }
        }

        public override void ValueFromString(string value)
        {
            // nothing to parse
            Value = value;
        }
        public override ParamType ValueType()
        {
            return ParamType.String;
        }

        #region Constructors

        public StringParam(string id) : base(id) { }

        #endregion
    }

    public class EnumParam : ParamValue
    {
        /// <summary>
        /// Constant string "SelectedIndex"
        /// </summary>
        public const string SelectedIndexString = "SelectedIndex";

        /// <summary>
        /// (Get) Collection of possible values
        /// </summary>
        public string[] Values { get; private set; }

        public override string ValueToString()
        {
            return SelectedIndex.ToString();
        }
        /// <summary>
        /// (Get/Set) This is actually the value of this paramters
        /// </summary>
        public int SelectedIndex 
        {
            get { return (int)Value; }
            set { Value = value; OnPropertyChanged(SelectedIndexString); }
        }
        public string SelectedItem
        {
            get { return Values[SelectedIndex]; }
        }

        public override void ValueFromString(string value)
        {
            // let to throw an exception if value is not in correct format
            SelectedIndex = int.Parse(value);
            // if index is too large, throw an exception
            if (Values.Length < SelectedIndex)
                throw new ArgumentOutOfRangeException(SelectedIndexString, "Argument is grater than maxium possible value");
        }
        public override ParamType ValueType()
        {
            return ParamType.Enum;
        }

        #region Constructors

        public EnumParam(string id, string[] values)
            : base(id)
        {
            Values = values;
        }

        #endregion
    }
}
