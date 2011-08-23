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
        IModule module;
        Channels channels;

        public TestWindow1()
        {
            InitializeComponent();
            timer.Interval = 400;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }
        bool value = false;
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            channels.HeatingFoilOn.Value = value;
            value = !value;

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
            //module = new ECModule("Task1");
            module = new ModbusModule("192.168.2.3", 502);
            module.LoadConfiguration("C:/ioLogik4010.csv");
            module.Connect();
            

            channels = new Channels(module);
            channels.Initialize();

            timer.Start();
        }

        private void stopClick(object sender, RoutedEventArgs e)
        {
            channels.Disconnect();
            timer.Stop();
        }

        private void writeClick(object sender, RoutedEventArgs e)
        {
            //channels.HeatingFoilOn.SetValue(true);
            //channels.UpdateOutputs();
        }
    }
}
