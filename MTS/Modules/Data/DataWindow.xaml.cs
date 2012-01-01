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
        /// <summary>
        /// Database context for this window (Entity Framework)
        /// </summary>
        private MTSContext context;

        #region Events

        /// <summary>
        /// This method is called when data window is loaded first time
        /// </summary>
        private void root_Loaded(object sender, RoutedEventArgs e)
        {   // create databse connection
            context = new MTSContext();
            // load shift result data
            shiftResultDataGrid.DataContext = context.ShiftResults.ToList();
        }
        /// <summary>
        /// This method is called when row in shift grid is selected
        /// </summary>
        private void shiftResultDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   // some row was selected in shift grid
            if (e.AddedItems.Count > 0)
            {   // get selected shift data
                ShiftResult res = shiftResultDataGrid.SelectedValue as ShiftResult;
                if (res != null)
                {   // load test results for selected shift
                    testResultDataGrid.DataContext = context.GetTestResult(res.Id).ToList();
                }
            }
        }
        /// <summary>
        /// This method is called when row in test grid is selected
        /// </summary>
        private void testResultDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   // some row was selected test grid
            if (e.AddedItems.Count > 0)
            {   // get selected test data
                DbTestResult res = testResultDataGrid.SelectedValue as DbTestResult;
                if (res != null)
                {   // load parameter results for selected test
                    paramResultDataGrid.DataContext = context.GetParamResult(res.Id).ToList();
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="DataWindow"/>. Data will be loaded automatically
        /// </summary>
        public DataWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
