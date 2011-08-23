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
using System.Windows.Shapes;

using System.Timers;
using MTS.AdminModule;
using MTS.TesterModule;


namespace MTS
{
    /// <summary>
    /// Interaction logic for TestWindow1.xaml
    /// </summary>
    public partial class TestWindow1 : Window
    {
        Timer timer = new Timer();
        Random gen = new Random();
        IModule module;
        Channels channels;

        public TestWindow1()
        {
            InitializeComponent();
            timer.Interval = 400;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            channels.Update();
            graph.Dispatcher.Invoke(new Action<double>(addValue), channels.HeatingFoilCurrent.RealValue);
        }
        void addValue(double value)
        {
            graph.AddValue(value);
            rawValue.Content = channels.HeatingFoilCurrent.Value.ToString();
            realValue.Content = value.ToString();
        }

        private void startClick(object sender, RoutedEventArgs e)
        {
            module = new ECModule("Task1");
            module.LoadConfiguration("C:/task1.csv");
            module.Connect();
            //module.Initialize();

            channels = new Channels(module);
            channels.Initialize();

            timer.Start();
        }

        private void stopClick(object sender, RoutedEventArgs e)
        {
            module.Disconnect();
            timer.Stop();
        }
    }
}
