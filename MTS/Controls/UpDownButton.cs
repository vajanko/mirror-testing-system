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
using System.Windows.Controls.Primitives;

namespace MTS.Controls
{
    [TemplatePart(Name = "PART_upButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_downButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name="PART_valueBox", Type=typeof(TextBox))]
    public class UpDownButton : Control
    {

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // register event for up button click
            RepeatButton b = base.GetTemplateChild("PART_upButton") as RepeatButton;
            if (b != null) b.Click += new RoutedEventHandler(increment);

            // register event for down button click
            b = base.GetTemplateChild("PART_downButton") as RepeatButton;
            if (b != null) b.Click += new RoutedEventHandler(decrement);

            // set special string format for binding
            TextBox tb = base.GetTemplateChild("PART_valueBox") as TextBox;
            if (tb != null)
            {
                Binding bind = new Binding("Value")
                {
                    Source = this,
                    StringFormat = this.StringFormat
                };
                tb.SetBinding(TextBox.TextProperty, bind);
            }
        }

        public string StringFormat
        {
            get
            {
                return "F" + Decimals.ToString();
            }
        }

        #region Dependency Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.Zero, new PropertyChangedCallback(valueChanged)));

        /// <summary>
        /// (Get/Set DP) Numeric button value
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
            if (value < btn.MinValue)          // if less than min, set to min
                btn.Value = btn.MinValue;
            else if (value > btn.MaxValue)     // if more than max, set to max
                btn.Value = btn.MaxValue;
        }

        #endregion

        #region MinValue Property

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.MinValue));    // default value for MinValue property

        /// <summary>
        /// (Get/Set DP) Minimum value of the numeric button value
        /// </summary>
        public decimal MinValue
        {
            get { return (decimal)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        #region MaxValue Property

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.MaxValue));

        /// <summary>
        /// (Get/Set DP) Maximum value of the numeric button value
        /// </summary>
        public decimal MaxValue
        {
            get { return (decimal)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region IncrementValue Property

        public static readonly DependencyProperty IncrementValueProperty =
            DependencyProperty.Register("IncrementValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.One));

        /// <summary>
        /// (Get/Set DP) Value by which value property is incremented/decremented when increment/decremnt event raised
        /// </summary>
        public decimal IncrementValue
        {
            get { return (decimal)GetValue(IncrementValueProperty); }
            set { SetValue(IncrementValueProperty, value); }
        }

        #endregion

        #region Decimals Property

        private const int defDecimals = 2;
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(UpDownButton),
            new PropertyMetadata(defDecimals));

        /// <summary>
        /// (Get/Set DP) Number of digits displayed after decimal point
        /// </summary>
        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        #endregion

        #region Digits Property

        private const short defDigits = 2;
        public static readonly DependencyProperty DigitsProperty =
            DependencyProperty.Register("Digits", typeof(short), typeof(UpDownButton),
            new PropertyMetadata(defDigits));

        /// <summary>
        /// (Get/Set DP) Number of digits displayed
        /// </summary>
        public short Digits
        {
            get { return (short)GetValue(DigitsProperty); }
            set { SetValue(DigitsProperty, value); }
        }

        #endregion

        #region Unit Property

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(Unit), typeof(UpDownButton),
            new PropertyMetadata(Units.None));

        /// <summary>
        /// (Get/Set DP) Unit of value displayed
        /// </summary>
        public Unit Unit
        {
            get { return (Unit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        #endregion

        #endregion

        #region Events

        #region Increment Event

        public static readonly RoutedEvent IncrementEvent =
            EventManager.RegisterRoutedEvent("Increment", RoutingStrategy.Direct, 
            typeof(RoutedEventHandler), typeof(UpDownButton));

        public event RoutedEventHandler Increment
        {
            add { AddHandler(IncrementEvent, value); }
            remove { RemoveHandler(IncrementEvent, value); }
        }

        private void increment(object sender, RoutedEventArgs e)
        {   // increment the value and raise on increment events
            Value += IncrementValue;
            OnIncrement(this, new RoutedEventArgs(IncrementEvent));
        }

        protected virtual void OnIncrement(object sender, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        static private void handleMoveUpCommand(object target, ExecutedRoutedEventArgs e)
        {
            UpDownButton bt = target as UpDownButton;
            if (bt != null) // raise increment event
                bt.increment(bt, new RoutedEventArgs(IncrementEvent));
        }

        #endregion

        #region Decrement Event

        public static readonly RoutedEvent DecrementEvent =
            EventManager.RegisterRoutedEvent("Decrement", RoutingStrategy.Direct,
            typeof(RoutedEventHandler), typeof(UpDownButton));

        public event RoutedEventHandler Decrement
        {
            add { AddHandler(DecrementEvent, value); }
            remove { RemoveHandler(DecrementEvent, value); }
        }

        private void decrement(object sender, RoutedEventArgs e)
        {   // decrement the value and raise on decrement events
            Value -= IncrementValue;
            OnDecrement(this, new RoutedEventArgs(DecrementEvent));
        }
        /// <summary>
        /// Called when Value is decremented
        /// </summary>
        protected virtual void OnDecrement(object sender, RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        static private void handleMoveDownCommand(object target, ExecutedRoutedEventArgs e)
        {   // handle the call of decrement command
            UpDownButton bt = target as UpDownButton;
            if (bt != null) // raise decrement event
                bt.decrement(bt, new RoutedEventArgs(DecrementEvent));
        }

        #endregion

        #endregion

        #region Commands

        private static RoutedUICommand incrementCommand =
            new RoutedUICommand("Increment", "IncrementCommand", typeof(UpDownButton));
        /// <summary>
        /// (Get DP) Command is invoked when value should be incremnented
        /// </summary>
        public static RoutedUICommand IncrementCommand
        {
            get { return incrementCommand; }
        }

        private static RoutedUICommand decrementCommand =
            new RoutedUICommand("Decrement", "DecrementCommand", typeof(UpDownButton));
        /// <summary>
        /// (Get DP) Command is invoked when value should be decremented
        /// </summary>
        public static RoutedUICommand DecrementCommand
        {
            get { return decrementCommand; }
        }

        #endregion

        #region Constructors

        static UpDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UpDownButton), new FrameworkPropertyMetadata(typeof(UpDownButton)));

            // increment command
            // bind key to increment command
            CommandManager.RegisterClassInputBinding(typeof(UpDownButton),
                new InputBinding(IncrementCommand, new KeyGesture(Key.Up)));
            // bind increment commnad to handler that execute it
            CommandManager.RegisterClassCommandBinding(typeof(UpDownButton),
                new CommandBinding(IncrementCommand, handleMoveUpCommand));

            // decrement command
            // bind key to decrement command
            CommandManager.RegisterClassInputBinding(typeof(UpDownButton),
                new InputBinding(DecrementCommand, new KeyGesture(Key.Down)));
            // bind decrement command to handler that execute it
            CommandManager.RegisterClassCommandBinding(typeof(UpDownButton),
                new CommandBinding(DecrementCommand, handleMoveDownCommand));
        }

        #endregion
    }    
}
