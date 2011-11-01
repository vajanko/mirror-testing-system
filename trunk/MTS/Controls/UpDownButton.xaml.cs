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
    public partial class UpDownButton : UserControl
    {


        #region Dependency Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.Zero, new PropertyChangedCallback(valueChanged)));

        /// <summary>
        /// (Get/Set) Numeric button value
        /// </summary>
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void valueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {   // validate Value property
            decimal value = (decimal)args.NewValue;
            UpDownButton btn = (obj as UpDownButton);
            // if new value is less than minimum or more than maximum reset back to the previous value
            if (value < btn.MinValue || value > btn.MaxValue)
                btn.Value = (decimal)args.OldValue;
        }

        #endregion

        #region MinValue Property

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.Zero));    // default value for MinValue property

        /// <summary>
        /// (Get/Set) Minimum value of the numeric button value
        /// </summary>
        public decimal MinValue
        {
            get { return (decimal)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region MaxValue Property

        private const decimal maxValueDef = 1000;   // default value for MaxValue property
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(maxValueDef));

        /// <summary>
        /// (Get/Set) Maximum value of the numeric button value
        /// </summary>
        public decimal MaxValue
        {
            get { return (decimal)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region Increment Property

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.One));

        /// <summary>
        /// (Get/Set) Value by which is value incremented/decremented when up/down button pressed
        /// </summary>
        public decimal Increment
        {
            get { return (decimal)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        #endregion

        #region Decimals Property

        private const short defDecimals = 2;
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(short), typeof(UpDownButton),
            new PropertyMetadata(defDecimals));

        /// <summary>
        /// (Get/Set) Number of digits displayed after decimal point
        /// </summary>
        public short Decimals
        {
            get { return (short)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        #endregion

        #region Digits Property

        private const short defDigits = 2;
        public static readonly DependencyProperty DigitsProperty =
            DependencyProperty.Register("Digits", typeof(short), typeof(UpDownButton),
            new PropertyMetadata(defDigits));

        /// <summary>
        /// (Get/Set) Number of digits displayed before decimal point
        /// </summary>
        public short Digits
        {
            get { return (short)GetValue(DigitsProperty); }
            set { SetValue(DigitsProperty, value); }
        }

        #endregion

        #endregion

        #region Events

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

        public UpDownButton()
        {
            InitializeComponent();
        }

        #endregion
    }
}
