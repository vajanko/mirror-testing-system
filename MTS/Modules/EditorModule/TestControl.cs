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

namespace MTS.EditorModule
{

    public class TestControl : HeaderedContentControl
    {
        #region Depencency Properties

        #region TestName Property

        static public readonly DependencyProperty TestNameProperty =
            DependencyProperty.Register("TestName", typeof(string), typeof(TestControl),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// (Get/Set) Name of the test
        /// </summary>
        public string TestName
        {
            get { return (string)GetValue(TestNameProperty); }
            set { SetValue(TestNameProperty, value); }
        }

        #endregion

        #region IsTestEnabled Property

        private const bool isTestEnabledDef = true;
        static public readonly DependencyProperty IsTestEnabledProperty =
            DependencyProperty.Register("IsTestEnabled", typeof(bool), typeof(TestControl),
            new PropertyMetadata(isTestEnabledDef));

        /// <summary>
        /// (Get/Set DP) True if test will be executed
        /// </summary>
        public bool IsTestEnabled
        {
            get { return (bool)GetValue(IsTestEnabledProperty); }
            set { SetValue(IsTestEnabledProperty, value); }
        }

        #endregion

        //#region TestParam Property

        //static public readonly DependencyProperty TestParamProperty =
        //    DependencyProperty.Register("TestParam", typeof(Test), typeof(TestControl),
        //    new PropertyMetadata(null));

        ///// <summary>
        ///// (Get/Set DP) Test description and parameter of test control
        ///// </summary>
        //public Test TestParam
        //{
        //    get { return (Test)GetValue(TestParamProperty); }
        //    set { SetValue(TestParamProperty, value); }
        //}

        //#endregion

        #endregion


        public TestControl()
        {

        }

        static TestControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TestControl), new FrameworkPropertyMetadata(typeof(TestControl)));
        }
    }
}
