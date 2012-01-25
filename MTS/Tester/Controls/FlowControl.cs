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
using System.ComponentModel;

using MTS.Base;
using MTS.IO;

namespace MTS.Tester.Controls
{
    /// <summary>
    /// Control that holds a flow of values measured each one per timestamp
    /// </summary>
    public class FlowControl : Control, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// (Get) Collection of measured values
        /// </summary>
        public DoubleQueue Values
        {
            get;
            protected set;
        }

        private double currentValue;
        /// <summary>
        /// (Get/Set) Last value added
        /// </summary>
        public double CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                RaisePropertyChanged("CurrentValue");
            }
        }

        /// <summary>
        /// Method that will return new value that should be added to this controls
        /// </summary>
        public Func<double> GetNewValue;

        #endregion

        #region Dependency Properties

        #region ValuesCapacity Property

        public const int DefaultValuesCapacity = 20;
        static public readonly DependencyProperty ValuesCapacityProperty =
            DependencyProperty.Register("ValuesCapacity", typeof(int), typeof(FlowControl),
            new PropertyMetadata(DefaultValuesCapacity));

        /// <summary>
        /// (Get/Set DP) Maximum number of stored values
        /// </summary>
        public int ValuesCapacity
        {
            get { return (int)GetValue(ValuesCapacityProperty); }
            set { SetValue(ValuesCapacityProperty, value); }
        }

        #endregion

        #region GraphBrush Property

        static public readonly DependencyProperty GraphBrushProperty =
            DependencyProperty.Register("GraphBrush", typeof(Brush), typeof(FlowControl),
            new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// (Get/Set DP) Brush used to draw graph curve
        /// </summary>
        public Brush GraphBrush
        {
            get { return (Brush)GetValue(GraphBrushProperty); }
            set { SetValue(GraphBrushProperty, value); }
        }

        #endregion

        #region TextBrush Property

        static public readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(FlowControl),
            new PropertyMetadata(Brushes.White));

        /// <summary>
        /// (Get/Set DP) Brush used to draw graph curve
        /// </summary>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        #endregion

        #region Zero Property

        public const double DefaultZero = 50;
        static public readonly DependencyProperty ZeroProperty =
            DependencyProperty.Register("Zero", typeof(double), typeof(FlowControl),
            new PropertyMetadata(DefaultZero));

        /// <summary>
        /// (Get/Set DP) In coordinate system where left-top corner is [0,0] this is the position
        /// of X-axis
        /// </summary>
        public double Zero
        {
            get { return (double)GetValue(ZeroProperty); }
            private set { SetValue(ZeroProperty, value); }
        }

        #endregion

        #region Unit Property

        static public readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(Unit), typeof(FlowControl));

        /// <summary>
        /// (Get/Set DP) Unit of values
        /// </summary>
        public Unit Unit
        {
            get { return (Unit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        #endregion

        #region Title Property

        static public readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FlowControl));

        /// <summary>
        /// (Get/Set) Short description that is displayed inside this control
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region MaxValue Property

        static public readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(FlowControl));

        /// <summary>
        /// (Get/Set) Max allowed value of added value
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            // when ActualWidth changes - step will be changed appropriately
            if (e.Property == ActualWidthProperty && ActualWidth != 0)
                Values.Step = ActualWidth / ValuesCapacity; // entire line (all values) are always displayed
            // when ActualWidth changes - zero will be changed appropriately
            else if (e.Property == ActualHeightProperty)
                Zero = ActualHeight / 2;    // zero line is always in the middle
        }

        public void AddValue(double value)
        {
            // by adding new values, too old ones get removed from Values collection
            Values.AddValue(value);
            // last value added
            CurrentValue = value;
            MaxValue = Values.MaxValue;
                 
            RaisePropertyChanged("Values");     // this is for binding
        }

        public void Update()
        {
            if (GetNewValue != null)
                AddValue(GetNewValue());
        }

        #region Constructors

        public FlowControl()
        {
            // new measured values are added and too old values are removed
            Values = new DoubleQueue();
            // bind Zero dependency property on this FlowControl with Zero property on Values
            Binding bind = new Binding("Zero") { Source = Values, Mode = BindingMode.TwoWay };
            SetBinding(ZeroProperty, bind);
            // bind ValuesCapacity dependency property on this FlowControl with Capacity property on Values
            bind = new Binding("Capacity") { Source = Values, Mode = BindingMode.OneWay };
            SetBinding(ValuesCapacityProperty, bind);
        }

        static FlowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowControl), new FrameworkPropertyMetadata(typeof(FlowControl)));
        }

        #endregion        
    }

    public class DoubleQueue : Queue<double>
    {
        /// <summary>
        /// (Get/Set) Each value of this double collection represents a value on Y-axis. This is a space
        /// between each value on X-axis
        /// </summary>
        public double Step { get; set; }
        /// <summary>
        /// (Get/Set) Value of X-axis. As this double collection is converted to points collection and displayed
        /// as a line
        /// </summary>
        public double Zero { get; set; }
        /// <summary>
        /// (Get/Set) Maximum number of items stored in this queue
        /// </summary>
        public int Capacity { get; set; }
        /// <summary>
        /// (Get) Minimal value present in the queue
        /// </summary>
        public double MinValue { get; private set; }
        /// <summary>
        /// (Get) Maximal value present in the queue
        /// </summary>
        public double MaxValue { get; private set; }

        /// <summary>
        /// Add new value to collection (and remove too old values). When there are more values than 
        /// <paramref name="Capacity"/> values from the end are removed.
        /// </summary>
        /// <param name="value">Value to add</param>
        public void AddValue(double value)
        {
            // insert value and remove too old ones
            Enqueue(value);
            while (Count > Capacity)
                Dequeue();
            MinValue = this.Min();
            MaxValue = this.Max();
        }

        /// <summary>
        /// Create a new instance of <typeparamref name="DoubleQueue"/> with default capacity
        /// </summary>
        public DoubleQueue() { Capacity = 20; }
    }
}
