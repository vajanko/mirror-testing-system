using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTS.Controls
{
    /// <summary>
    /// Interaction logic for NumericButton.xaml
    /// </summary>
    public partial class NumericButton : StackPanel
    {
        #region Properties

        private decimal _increment = 1;
        private const decimal MAX_INC = 10;
        /// <summary>
        /// Get or set the increment value. Numeric button value will be increase/decreased by this value when pressed
        /// up/down buttons.
        /// </summary>
        public decimal Increment
        {
            get { return _increment; }
            set
            {
                if (value > 0 && value <= MAX_INC)
                    _increment = value;
            }
        }

        private int _decimals = 2;
        private const int MAX_DECIMALS = 28;        // this is the max. possible value of type decimal      
        /// <summary>
        /// Get or set the number of digits displayed after decimal point
        /// </summary>
        public int Decimals
        {
            get { return _decimals; }
            set
            {
                if (value >= 0 && value <= MAX_DECIMALS)
                    _decimals = value;
            }
        }

        private decimal _minValue = decimal.MinValue;
        /// <summary>
        /// Get or set the minimal possible value that could be set to this button
        /// </summary>
        public decimal MinValue
        {
            get { return _minValue; }
            set
            {
                if (value < _maxValue)
                    _minValue = value;
            }
        }

        private decimal _maxValue = decimal.MaxValue;
        /// <summary>
        /// Get or set the maximal possible value that colud be set to this button
        /// </summary>
        public decimal MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value > _minValue)
                    _maxValue = value;
            }
        }

        private decimal _value = 0;
        /// <summary>
        /// Get or set numeric button value
        /// </summary>
        public decimal Value
        {
            get { return _value; }
            set
            {
                if (value >= _minValue && value <= _maxValue)
                {
                    _value = decimal.Round(value, Decimals);                    
                    this.inputValue.Text = _value.ToString();
                }
            }
        }

        private Units _unit = Units.None;
        /// <summary>
        /// Get or set unit type of the numwric value
        /// </summary>
        public Units Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                if (unitBlock != null)
                    unitBlock.Text = "[" + _unit.GetSymbol() + "]";
            }
        }

        private const int PIXELS_PER_DIGIT = 6;
        private const int DIGITS = 3;
        private int _digits = DIGITS;
        /// <summary>
        /// Get or set number of places prepared for digits to be displayed. Default value is 3.
        /// </summary>
        public int Digits
        {
            get { return _digits; }
            set
            {
                if (value > 0 && value <= MAX_DECIMALS)
                {
                    _digits = value;
                    inputValue.MinWidth = (value + 1) * PIXELS_PER_DIGIT;
                }
            }
        }

        #endregion

        #region Events

        void value_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal newValue;
            if (decimal.TryParse(inputValue.Text, out newValue))  // if it is a string
                Value = newValue;                                 // than check if value is in range
            else Value = _value;                                  // otherwise return back previous value
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            Value += Increment;
        }

        #endregion

        #region Constructors

        public NumericButton()
        {
            InitializeComponent();
            Value = _value;     // initialize text in text box
            inputValue.TextChanged += new TextChangedEventHandler(value_TextChanged);
            Digits = DIGITS;
        }

        #endregion
    }

    public enum Units
    {
        None,
        Degrees,
        MiliAmpheres,
        MiliMeters,
        Grams,
        MiliSeconds,
        Percets                
    }

    static public class ExtensionUnits
    {
        /// <summary>
        /// Get the symbol identifying a particular unit types
        /// </summary>
        /// <param name="unit">Unit type</param>
        /// <returns>Symbol indentifying a particular unit</returns>
        static public string GetSymbol(this Units unit)
        {
            switch (unit)
            {
                case Units.Degrees: return "˚";
                case Units.Grams: return "g";
                case Units.MiliAmpheres: return "mA";
                case Units.MiliMeters: return "mm";
                case Units.MiliSeconds: return "ms";
                case Units.Percets: return "%";
                default: return "";
            }
        }
    }
}
