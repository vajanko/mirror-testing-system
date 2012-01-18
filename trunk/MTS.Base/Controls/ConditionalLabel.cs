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
using System.Windows.Controls.Primitives;

namespace MTS.Base
{
    public class ConditionalLabel : Label
    {
        #region TrueContent Property

        public static readonly DependencyProperty TrueContentProperty =
            DependencyProperty.Register("TrueContent", typeof(object), typeof(ConditionalLabel));

        /// <summary>
        /// (Get/Set) Value of <see cref="Content"/> when <see cref="Condition"/> is true
        /// </summary>
        public object TrueContent
        {
            get { return GetValue(TrueContentProperty); }
            set { SetValue(TrueContentProperty, value); }
        }

        #endregion

        #region FalseContent Property

        public static readonly DependencyProperty FalseContentProperty =
            DependencyProperty.Register("FalseContent", typeof(object), typeof(ConditionalLabel));

        /// <summary>
        /// (Get/Set) Value of <see cref="Content"/> when <see cref="Condition"/> is false
        /// </summary>
        public object FalseContent
        {
            get { return GetValue(FalseContentProperty); }
            set { SetValue(FalseContentProperty, value); }
        }

        #endregion

        #region Condition Property

        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition", typeof(bool), typeof(ConditionalLabel),
            new PropertyMetadata(conditionChanged));

        /// <summary>
        /// (Get/Set) Conditional value. Depending this value different content will be display
        /// </summary>
        public bool Condition
        {
            get { return (bool)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }
        private static void conditionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ConditionalLabel label = obj as ConditionalLabel;
            if (label != null)
            {
                Binding bind = new Binding();
                if ((bool)args.NewValue)
                    bind.Path = new PropertyPath("TrueContent");
                else
                    bind.Path = new PropertyPath("FalseContent");
                label.SetBinding(ContentProperty, bind);
            }
                
        }

        #endregion


        public ConditionalLabel()
            : base()
        {
            //MultiBinding bind = new MultiBinding() { Converter = new BoolBindingSelector() };
            //bind.Bindings.Add(new Binding("Condition"));
            //bind.Bindings.Add(new Binding("TrueContent"));
            //bind.Bindings.Add(new Binding("FalseContent"));
            //SetBinding(ContentProperty, bind);
        }
    }
}
