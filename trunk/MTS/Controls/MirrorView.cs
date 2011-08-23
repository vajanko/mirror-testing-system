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

namespace MTS.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class MirrorView : Control
    {
        #region Dependency Properties

        #region RotationAxis Property

        public static readonly DependencyProperty RotationAxisProperty =
            DependencyProperty.Register("RotationAxis", typeof(Vector3D), typeof(MirrorView));

        /// <summary>
        /// (Get/Set DP) Axis arond which is the mirror rotated
        /// </summary>
        public Vector3D RotationAxis
        {
            get { return (Vector3D)GetValue(RotationAxisProperty); }
            set { SetValue(RotationAxisProperty, value); }
        }

        #endregion

        #region RotationAngle Property

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(MirrorView));

        /// <summary>
        /// (Get/Set DP) Angle of mirror rotation
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        #endregion

        #endregion

        static MirrorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MirrorView), new FrameworkPropertyMetadata(typeof(MirrorView)));
        }
    }
}
