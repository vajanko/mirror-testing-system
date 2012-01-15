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

namespace MTS.Base
{
    public class UpDownButton : Control
    {
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
            decimal newValue = (decimal)args.NewValue;
            decimal oldValue = (decimal)args.OldValue;
            UpDownButton btn = (obj as UpDownButton);
            if (btn != null)    // check if value is inside the allowed range
            {
                if (newValue < btn.MinValue)          // if less than min, set to min
                    btn.Value = btn.MinValue;
                else if (newValue > btn.MaxValue)     // if more than max, set to max
                    btn.Value = btn.MaxValue;

                // raise value changed event, but only if value has been changed
                if (newValue != oldValue)
                {
                    btn.OnValueChanged(new RoutedEventArgs(ValueChangedEvent, btn));
                    if (newValue > oldValue)        // value has been incremented
                        btn.OnIncrement(new RoutedEventArgs(IncrementEvent, btn));
                    else if (newValue < oldValue)   // value has been decremented
                        btn.OnDecrement(new RoutedEventArgs(DecrementEvent, btn));
                }
            }
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

        #region StepValue Property

        public static readonly DependencyProperty StepValueProperty =
            DependencyProperty.Register("StepValue", typeof(decimal), typeof(UpDownButton),
            new PropertyMetadata(decimal.One));

        /// <summary>
        /// (Get/Set) Value by which value property is incremented/decremented when increment/decremnt event is raised
        /// </summary>
        public decimal StepValue
        {
            get { return (decimal)GetValue(StepValueProperty); }
            set { SetValue(StepValueProperty, value); }
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

        #region ValueAlignment Property

        public static readonly DependencyProperty ValueAlignmentProperty =
            DependencyProperty.Register("ValueAlignment", typeof(HorizontalAlignment), typeof(UpDownButton),
            new PropertyMetadata(HorizontalAlignment.Right));

        /// <summary>
        /// (Get/Set DP) Alignment of displayed value
        /// </summary>
        public HorizontalAlignment ValueAlignment
        {
            get { return (HorizontalAlignment)GetValue(ValueAlignmentProperty); }
            set { SetValue(ValueAlignmentProperty, value); }
        }

        #endregion

        #region Format Property

        private const string defaultFormat = "{0}";
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(UpDownButton),
            new PropertyMetadata(defaultFormat));

        /// <summary>
        /// (Get/Set) String passed to <see cref="string.Format"/> method applying on <see cref="Value"/>
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        #endregion

        #endregion

        #region Events

        #region ValueChanged Event

        /// <summary>
        /// Event that is raised when <see cref="Value"/> of <see cref="UpDownButton"/> is changed
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct,
            typeof(RoutedEventHandler), typeof(UpDownButton));

        /// <summary>
        /// Occurs when <see cref="Value"/> of <see cref="UpDownButton"/> change
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        /// <summary>
        /// Raise <see cref="ValueChanged"/> event
        /// </summary>
        /// <param name="e">Routed event argument</param>
        protected virtual void OnValueChanged(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region Increment Event

        /// <summary>
        /// Event that is raised when <see cref="Value"/> of <see cref="UpDownButton"/> is incremented
        /// </summary>
        public static readonly RoutedEvent IncrementEvent =
            EventManager.RegisterRoutedEvent("Increment", RoutingStrategy.Direct, 
            typeof(RoutedEventHandler), typeof(UpDownButton));

        /// <summary>
        /// Occures when <see cref="Value"/> of <see cref="UpDownButton"/> is incremented
        /// </summary>
        public event RoutedEventHandler Increment
        {
            add { AddHandler(IncrementEvent, value); }
            remove { RemoveHandler(IncrementEvent, value); }
        }

        /// <summary>
        /// Raise <see cref="Increment"/> event
        /// </summary>
        /// <param name="e">Routed event argument</param>
        protected virtual void OnIncrement(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Increment <see cref="Value"/> by <see cref="StepValue"/>
        /// </summary>
        /// <param name="sender">Down button</param>
        /// <param name="e">Click routed event args</param>
        private void increment(object sender, RoutedEventArgs e)
        {   // increment the value and raise on increment events
            Value += StepValue;
        }

        /// <summary>
        /// Handler for <see cref="IncrementCommand"/>
        /// </summary>
        /// <param name="target">Instance of <see cref="UpDownButton"/> on which <see cref="IncrementCommand"/>
        /// will be executed</param>
        /// <param name="e">Routed event argument</param>
        static private void handleMoveUpCommand(object target, ExecutedRoutedEventArgs e)
        {
            UpDownButton bt = target as UpDownButton;
            if (bt != null) // raise increment event
                bt.increment(bt, new RoutedEventArgs(IncrementEvent));
        }

        #endregion

        #region Decrement Event

        /// <summary>
        /// Event that is raised when <see cref="Value"/> of <see cref="UpDownButton"/> is decremented
        /// </summary>
        public static readonly RoutedEvent DecrementEvent =
            EventManager.RegisterRoutedEvent("Decrement", RoutingStrategy.Direct,
            typeof(RoutedEventHandler), typeof(UpDownButton));

        /// <summary>
        /// Occurs when <see cref="Value"/> of <see cref="UpDownButton"/> is decremented.
        /// </summary>
        public event RoutedEventHandler Decrement
        {
            add { AddHandler(DecrementEvent, value); }
            remove { RemoveHandler(DecrementEvent, value); }
        }

        /// <summary>
        /// Raise <see cref="Decrement"/> event
        /// </summary>
        /// <param name="e">Routed event argument</param>
        protected virtual void OnDecrement(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Decrement <see cref="Value"/> by <see cref="StepValue"/>
        /// </summary>
        /// <param name="sender">Down button</param>
        /// <param name="e">Click routed event arguments</param>
        private void decrement(object sender, RoutedEventArgs e)
        {   // decrement the value and raise on decrement events
            Value -= StepValue;
        }
        /// <summary>
        /// Handler for <see cref="DecrementCommand"/>
        /// </summary>
        /// <param name="target">Instance of <see cref="UpDownButton"/> on which <see cref="DecrementCommand"/>
        /// will be executed</param>
        /// <param name="e">Routed event argument</param>
        static private void handleMoveDownCommand(object target, ExecutedRoutedEventArgs e)
        {   // handle the call of decrement command
            UpDownButton bt = target as UpDownButton;
            if (bt != null) // raise decrement event
            {
                bt.decrement(bt, new RoutedEventArgs(DecrementEvent));
            }
        }

        #endregion

        #endregion

        #region Commands

        private static RoutedUICommand incrementCommand =
            new RoutedUICommand("Increment", "IncrementCommand", typeof(UpDownButton));
        /// <summary>
        /// (Get) Command is invoked when value should be incremented
        /// </summary>
        public static RoutedUICommand IncrementCommand
        {
            get { return incrementCommand; }
        }

        private static RoutedUICommand decrementCommand =
            new RoutedUICommand("Decrement", "DecrementCommand", typeof(UpDownButton));
        /// <summary>
        /// (Get) Command is invoked when value should be decremented
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
            // bind increment command to handler that execute it
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
