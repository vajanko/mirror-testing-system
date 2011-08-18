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
    /// 
    /// </summary>
    public class MirrorView : Control
    {
        #region Dependency Properties

        public const double zero = 0;

        #region Vertical Rotation

        public static readonly DependencyProperty VerticalRotationProperty =
            DependencyProperty.Register("VerticalRotation", typeof(double), typeof(MirrorView),
            new PropertyMetadata(zero));

        /// <summary>
        /// (Get/Set DP) 
        /// </summary>
        public double VerticalRotation
        {
            get { return (double)GetValue(VerticalRotationProperty); }
            set { SetValue(VerticalRotationProperty, value); }
        }

        #endregion

        #region Horizontal Rotation

        public static readonly DependencyProperty HorizontalRotationProperty =
            DependencyProperty.Register("HorizontalRotation", typeof(double), typeof(MirrorView),
            new PropertyMetadata(zero));

        /// <summary>
        /// (Get/Set DP) 
        /// </summary>
        public double HorizontalRotation
        {
            get { return (double)GetValue(HorizontalRotationProperty); }
            set { SetValue(HorizontalRotationProperty, value); }
        }

        #endregion

        #endregion

        static MirrorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MirrorView), new FrameworkPropertyMetadata(typeof(MirrorView)));
        }
    }
}
