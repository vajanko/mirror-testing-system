using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MTS.Base
{
    /// <summary>
    /// Command executing given delegate method which takes one argument and does not return a value
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region ICommand Members

        /// <summary>
        /// Delegate that takes an object as a parameter and returns value indicating whether current
        /// command can be executed
        /// </summary>
        private Func<object, bool> canExecute;
        /// <summary>
        /// Gets value indicating whether the command with given parameter can be executed
        /// </summary>
        /// <param name="parameter">Instance of command parameter. Optionally can be null</param>
        /// <returns>True if command can be executed. Otherwise false</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute(parameter);
        }

        private event EventHandler _canExecuteChanged;
        /// <summary>
        /// Occurs when current command could be executed and can not be executed right now, or
        /// it couldn't be and can right now
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }
        /// <summary>
        /// Raise <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (_canExecuteChanged != null)
                _canExecuteChanged(this, EventArgs.Empty);
        }

        private Action<object> execute;
        /// <summary>
        /// Execute current command - call stored delegate method
        /// </summary>
        /// <param name="parameter">Optional command parameter</param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of command executing given delegate method
        /// </summary>
        /// <param name="execute">A delegate method executing by current command</param>
        /// <param name="canExecute">Optional delegate taking as an argument command parameter and
        /// returning <see cref="bool"/> value indicating whether current command can be executed.
        /// If not specified, command can be executed without any restrictions</param>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion
    }
}
