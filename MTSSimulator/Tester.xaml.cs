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

namespace MTS.Simulator
{
    /// <summary>
    /// Interaction logic for Tester.xaml
    /// </summary>
    public partial class Tester : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        private PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        protected void RaisePropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Dependency Properties

        private const bool trueConst = true;

        #region IsRedLightOn Property

        static public readonly DependencyProperty IsRedLightOnProperty =
            DependencyProperty.Register("IsRedLightOn", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsRedLightOn
        {
            get { return (bool)GetValue(IsRedLightOnProperty); }
            set { SetValue(IsRedLightOnProperty, value); }
        }

        #endregion

        #region IsGreenLightOn Property

        static public readonly DependencyProperty IsGreenLightOnProperty =
            DependencyProperty.Register("IsGreenLightOn", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsGreenLightOn
        {
            get { return (bool)GetValue(IsGreenLightOnProperty); }
            set { SetValue(IsGreenLightOnProperty, value); }
        }

        #endregion

        #region IsPowerSupplyOn Property

        static public readonly DependencyProperty IsPowerSupplyOnProperty =
            DependencyProperty.Register("IsPowerSupplyOn", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsPowerSupplyOn
        {
            get { return (bool)GetValue(IsPowerSupplyOnProperty); }
            set { SetValue(IsPowerSupplyOnProperty, value); }
        }

        #endregion

        #region IsStartPressed Property

        static public readonly DependencyProperty IsStartPressedProperty =
            DependencyProperty.Register("IsStartPressed", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsStartPressed
        {
            get { return (bool)GetValue(IsStartPressedProperty); }
            set { SetValue(IsStartPressedProperty, value); }
        }

        #endregion

        #region IsErrorAckPressed Property

        static public readonly DependencyProperty IsErrorAckPressedProperty =
            DependencyProperty.Register("IsErrorAckPressed", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsErrorAckPressed
        {
            get { return (bool)GetValue(IsErrorAckPressedProperty); }
            set { SetValue(IsErrorAckPressedProperty, value); }
        }

        #endregion

        #region IsDeviceOpened Property

        static public readonly DependencyProperty IsDeviceOpenedProperty =
            DependencyProperty.Register("IsDeviceOpened", typeof(bool), typeof(Tester),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set DP)
        /// </summary>
        public bool IsDeviceOpened
        {
            get { return (bool)GetValue(IsDeviceOpenedProperty); }
            set { SetValue(IsDeviceOpenedProperty, value); }
        }

        #endregion

        #endregion

        private bool isMirrorInserted = false;
        public bool IsMirrorInserted
        {
            get { return isMirrorInserted; }
            set {
                isMirrorInserted = value;
                if (value)
                    mirror.Visibility = System.Windows.Visibility.Visible;
                else
                    mirror.Visibility = System.Windows.Visibility.Hidden;
            }

        }


        public Tester()
        {
            InitializeComponent();
        }
    }
}
