using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MTS.Base
{
    public class DelegateCommand : ICommand
    {
        #region ICommand Members

        private Func<object, bool> canExecute;
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute(parameter);
        }

        private event EventHandler _canExecuteChanged;
        /// <summary>
        /// Occurs when ...
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        private Action<object> execute;
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

        #region Constructors

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion
    }
}
