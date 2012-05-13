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
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTS.Tester.Controls
{
    /// <summary>
    /// Control that displays 3D model of mirror glass.
    /// </summary>
    public class MirrorView : Control
    {
        #region Dependency Properties

        /// <summary>
        /// This method is called when <see cref="MirrorView.RotationAngle"/> dependency property change
        /// </summary>
        /// <param name="source">Instance of dependency object on which <see cref="MirrorView.RotationAngle"/>
        /// has been changed</param>
        /// <param name="args">Dependency property changed event arguments</param>
        static private void rotationChanged(DependencyObject source, DependencyPropertyChangedEventArgs args) 
        {   // called when RotationAxis or RotationAngle changed
            
        }

        #region RotationAxis Property

        /// <summary>
        /// Identifies <see cref="MirrorView.RotationAxis"/> dependency
        /// </summary>
        public static readonly DependencyProperty RotationAxisProperty =
            DependencyProperty.Register("RotationAxis", typeof(Vector3D), typeof(MirrorView),
            new PropertyMetadata(new PropertyChangedCallback(rotationChanged)));

        /// <summary>
        /// (Get/Set) Axis around which is the mirror rotated. This is dependency property.
        /// </summary>
        public Vector3D RotationAxis
        {
            get { return (Vector3D)GetValue(RotationAxisProperty); }
            set { SetValue(RotationAxisProperty, value); }
        }

        #endregion

        #region RotationAngle Property

        /// <summary>
        /// Identifies <see cref="MirrorView.RotationAngle"/> dependency
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(MirrorView),
            new PropertyMetadata(new PropertyChangedCallback(rotationChanged)));

        /// <summary>
        /// (Get/Set) Angle of mirror rotation. This is dependency property.
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        #endregion

        #region HorizontalAngle Property

        /// <summary>
        /// Identifies <see cref="MirrorView.HorizontalAngle"/> dependency
        /// </summary>
        public static readonly DependencyProperty HorizontalAngleProperty =
            DependencyProperty.Register("HorizontalAngle", typeof(double), typeof(MirrorView));

        /// <summary>
        /// (Get/Set) Horizontal angle of mirror rotation. This is dependency property.
        /// </summary>
        public double HorizontalAngle
        {
            get { return (double)GetValue(HorizontalAngleProperty); }
            set { SetValue(HorizontalAngleProperty, value); }
        }

        #endregion

        #region VerticalAngle Property

        /// <summary>
        /// Identifies <see cref="MirrorView.VerticalAngle"/> dependency
        /// </summary>
        public static readonly DependencyProperty VerticalAngleProperty =
            DependencyProperty.Register("VerticalAngle", typeof(double), typeof(MirrorView));

        /// <summary>
        /// (Get/Set) Vertical angle of mirror rotation. This is dependency property.
        /// </summary>
        public double VerticalAngle
        {
            get { return (double)GetValue(VerticalAngleProperty); }
            set { SetValue(VerticalAngleProperty, value); }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize <see cref="MirrorView"/> dependency properties.
        /// </summary>
        static MirrorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MirrorView), new FrameworkPropertyMetadata(typeof(MirrorView)));
        }

        #endregion
    }
}
