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
    public class NumericBox : TextBox
    {

        private Binding textBinding;
        private Binding stringFormatBinding;

        #region Constructors

        public NumericBox()
        {
            //textBinding = new Binding();
            //textBinding.Source = this;
            //textBinding.Path = new PropertyPath(this.Text);

            //stringFormatBinding = new Binding();
            //stringFormatBinding.Source = this;
        }

        static NumericBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(typeof(NumericBox)));
        }

        #endregion
    }
}
