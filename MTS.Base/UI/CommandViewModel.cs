using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MTS.Base
{
    public class CommandViewModel : ViewModelBase
    {
        public ICommand Command { get; private set; }


        #region Constructors

        public CommandViewModel(string displayName, ICommand command)
            : base(displayName)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Command = command;
        }

        #endregion
    }
}
