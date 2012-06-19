using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Controls.Primitives;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using AvalonDock;

using MTS.Base;
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
        private MTSContext context = new MTSContext();

        #region Events

        /// <summary>
        /// This method is called when data window is loaded first time. Data from database are loaded to the window
        /// </summary>
        private void root_Loaded(object sender, RoutedEventArgs e)
        {   // load shift result data
            shiftResultDataGrid.DataContext = context.ShiftResults.ToList();
        }
        /// <summary>
        /// This method is called when data document item is activated or deactivated. When activated data
        /// content is reloaded from database.
        /// </summary>
        /// <param name="sender">Data document content</param>
        private void root_IsActiveDocumentChanged(object sender, EventArgs e)
        {
            DocumentContent doc = sender as DocumentContent;
            if (doc != null && doc.IsActiveDocument)        // document has been activated - reload data
            {   // load shift result data
                shiftResultDataGrid.DataContext = context.ShiftResults.ToList();
            }
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
                {   // load test results for selected shift and group them by sequence
                    var testResultsView = CollectionViewSource.GetDefaultView(context.GetTestResult(res.Id).ToList());
                    testResultsView.GroupDescriptions.Add(new PropertyGroupDescription("Sequence"));
                    testResultDataGrid.ItemsSource = testResultsView;
                    // select first item in test results
                    if (testResultDataGrid.Items.Count > 0)
                        testResultDataGrid.SelectedIndex = 0;
                }
            }
        }
        /// <summary>
        /// This method is called when row of shift result grid is loaded. At this time index of row is generated and added
        /// as row header
        /// </summary>
        private void shiftResultDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {   // in xaml binding to header of row
            e.Row.Header = e.Row.GetIndex() + 1;
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
        /// <summary>
        /// This method is called when size of the main grid in data window is changed. Each data grid in the main window
        /// get half size of the window so scrollbars will be visible on them
        /// </summary>
        private void mainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = e.NewSize.Height / 2;
            shiftResultDataGrid.MaxHeight = height;
            testResultDataGrid.MaxHeight = height;
            paramResultDataGrid.MaxHeight = height;
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
