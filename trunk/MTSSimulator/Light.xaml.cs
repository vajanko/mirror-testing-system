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

namespace MTS.Simulator
{
    /// <summary>
    /// Interaction logic for Light.xaml
    /// </summary>
    public partial class Light : UserControl
    {
        #region OnColor Property

        static public readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(Brush), typeof(Light));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        #endregion

        #region OffColor Property

        static public readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(Brush), typeof(Light));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        #endregion

        #region IsOn Property

        static public readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(Light),
            new PropertyMetadata(new PropertyChangedCallback(isOnChanged)));

        private static void isOnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            bool value = (bool)args.NewValue;
            Light light = obj as Light;
            if (light != null)
            {
                //Binding bind = new Binding();
                if (value)
                    //bind.Path = new PropertyPath("OnColor");
                    light.light.Fill = light.OnColor;
                else
                    //bind.Path = new PropertyPath("OffColor");
                    light.light.Fill = light.OffColor;
                //BindingOperations.SetBinding(light, Ellipse.FillProperty, bind);
            }
        }

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        #endregion

        public Light()
        {
            InitializeComponent();
        }
    }
}

