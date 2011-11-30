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

namespace MTS.Data
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : DocumentContent
    {
        public void Load()
        {
            using (MTSContext context = new MTSContext())
            {
                grid.ItemsSource = context.Suppliers.ToList();
            }
        }

        #region Constructors

        public DataWindow()
        {
            InitializeComponent();
        }

        #endregion

        private void suppliersGrid_Initialized(object sender, EventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid != null)
            {
                using (MTSContext context = new MTSContext())
                {
                    grid.ItemsSource = context.Suppliers.ToList();
                }
            }
        }

        private void mirrorsGrid_Initialized(object sender, EventArgs e)
        {            
            DataGrid grid = sender as DataGrid;
            if (grid != null)
            {
                using (MTSContext context = new MTSContext())
                {
                    grid.ItemsSource = context.Mirrors.Include(m => m.Supplier).ToList();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mirrorsGrid.CommitEdit();
        }
    }
}
