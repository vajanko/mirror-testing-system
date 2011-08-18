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
    /// Interaction logic for TestControl1.xaml
    /// </summary>
    public partial class TestControl1 : UserControl
    {

        public PointCollection Points { get; set; }

        public TestControl1()
        {
            Points = new PointCollection();
            Random gen = new Random();
            for (int i = 0; i < 30; i++)
                Points.Add(new Point { X = i * 5, Y = gen.Next(0, 100) });
            

            InitializeComponent();
        }
    }
}
