using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Base;

namespace MTS.Admin
{
    /// <summary>
    /// View-Model class for operator profile window
    /// </summary>
    public class ProfileWindowViewModel : ViewModelBase
    {
        #region Model Properties

        private string _fullName;
        /// <summary>
        /// (Get/Set) Operator full name
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged("FullName");
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

        private int _shifts;
        /// <summary>
        /// (Get/Set) Number of operator executed shifts
        /// </summary>
        public int TotalShifts
        {
            get { return _shifts; }
            set
            {
                _shifts = value;
                OnPropertyChanged("TotalShifts");
            }
        }

        private TimeSpan _totalTestingTime;
        /// <summary>
        /// (Get/Set) Total time operator testing
        /// </summary>
        public TimeSpan TotalTestingTime
        {
            get { return _totalTestingTime; }
            set
            {
                _totalTestingTime = value;
                OnPropertyChanged("TotalTestingTime");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of View-model for operator profile window
        /// </summary>
        /// <param name="displayName"></param>
        public ProfileWindowViewModel(string displayName)
            : base(displayName)
        {
        }

        #endregion
    }
}
