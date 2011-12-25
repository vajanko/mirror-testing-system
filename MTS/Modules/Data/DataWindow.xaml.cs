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

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using AvalonDock;

using MTS.Controls;

namespace MTS.Data
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : DocumentItem 
    {
        private MTSContext context;

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            context = new MTSContext();
            shiftResultListBox.DataContext = context.ShiftResults.ToList();
        }

        #region Constructors

        public DataWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
