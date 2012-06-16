using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MTS.Base
{
    public class DelegateCommand<TParam> : ICommand
    {
        #region ICommand Members

        private Func<TParam, bool> canExecute;
        public bool CanExecute(object parameter)
        {
            if (!(parameter is TParam))
                throw new InvalidOperationException(string.Format("Command parameter must be of type {0}. {1} type parameter is given", typeof(TParam), parameter.GetType()));

            if (canExecute == null)
                return true;

            return canExecute((TParam)parameter);
        }

        public event EventHandler CanExecuteChanged;

        private Action<TParam> execute;
        public void Execute(object parameter)
        {
            if (!(parameter is TParam))
                throw new InvalidOperationException(string.Format("Command parameter must be of type {0}. {1} type parameter is given", typeof(TParam), parameter.GetType()));

            execute((TParam)parameter);
        }

        #endregion


        #region Constructors

        public DelegateCommand(Action<TParam> execute, Func<TParam, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion
    }
}
