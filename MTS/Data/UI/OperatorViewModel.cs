using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.Base;

namespace MTS.Data
{
    public class OperatorViewModel : ViewModelBase
    {
        #region Model Properties

        private string _name;
        /// <summary>
        /// (Get/Set) Operator first name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _surname;
        /// <summary>
        /// (Get/Set) Operator second name
        /// </summary>
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }

        private string _login;
        /// <summary>
        /// (Get/Set) Operator login
        /// </summary>
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }

        private OperatorEnum _group;
        /// <summary>
        /// (Get/Set) Operator group
        /// </summary>
        public OperatorEnum Group
        {
            get { return _group; }
            set
            {
                _group = value;
                OnPropertyChanged("Group");
            }
        }

        /// <summary>
        /// (Get/Set) Operator group as byte value
        /// </summary>
        public byte Type
        {
            get { return (byte)_group; }
            set
            {
                Group = (OperatorEnum)value;
                OnPropertyChanged("Type");
            }
        }

        #endregion

        public void LoadData(Operator op)
        {
            Name = op.Name;
            Surname = op.Surname;
            Login = op.Login;
            Group = (OperatorEnum)op.Type;
        }

        public void GetData(Operator op)
        {
            op.Login = Login;
            op.Name = Name;
            op.Surname = Surname;
            op.Type = (byte)Group;

            op.Password = Admin.Operator.ComputeHash(string.Empty);
        }

        #region Constructors

        public OperatorViewModel(string displayName)
            : base(displayName)
        {
            
        }

        #endregion
    }
}
