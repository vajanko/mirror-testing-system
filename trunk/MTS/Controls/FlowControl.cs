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
    /// Control that holds a flow of values measured each one per timestep
    /// </summary>
    public class FlowControl : Control
    {
        #region Dependency Properties

        #region Values Property

        static public readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(PointCollection), typeof(FlowControl));

        /// <summary>
        /// (Get/Set DP) Collection of measured values
        /// </summary>
        public PointCollection Values
        {
            get { return (PointCollection)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        #endregion

        #region ValuesCapacity Property

        public const int DefaultValuesCapacity = 30;
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
        
        #region GraphRect Property

        static public readonly Rect DefaultRect = new Rect(0, 0, 100, 100);
        static public readonly DependencyProperty GraphRectProperty =
            DependencyProperty.Register("GraphRect", typeof(Rect), typeof(FlowControl),
            new PropertyMetadata(DefaultRect));

        /// <summary>
        /// (Get/Set DP) Area of displayed graph
        /// </summary>
        public Rect GraphRect
        {
            get { return (Rect)GetValue(GraphRectProperty); }
            set { SetValue(GraphRectProperty, value); }
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
            set { SetValue(ZeroProperty, value); }
        }

        #endregion

        #endregion

        public void AddValue(Point value)
        {
        }

        public FlowControl()
        {
            Random gen = new Random();
            Values = new PointCollection();
            for (int i = 0; i < 30; i++)
                Values.Add(new Point(i*2, gen.Next(30)));
        }

        static FlowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlowControl), new FrameworkPropertyMetadata(typeof(FlowControl)));
        }
    }
}
