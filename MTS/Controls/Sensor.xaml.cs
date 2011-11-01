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
using System.Collections.ObjectModel;

namespace MTS.Controls
{
    /// <summary>
    /// Interaction logic for Sensor.xaml
    /// </summary>
    public partial class Sensor : UserControl
    {
        private int _count = 1;
        /// <summary>
        /// Nember of sensors
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                if (value > 0)
                {
                    _count = value;
                    Sensors.Clear();
                    for (int i = 0; i < _count; i++)
                        Sensors.Add("Sensor " + (i + 1).ToString());
                }
            }
        }

        public ObservableCollection<string> Sensors { get; private set; }
        public Sensor()
        {
            InitializeComponent();
            Sensors = new ObservableCollection<string>();
            Sensors.Add("Sensor 1");
        }
    }
}
