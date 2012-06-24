using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace MTS.Base
{
    /// <summary>
    /// Base class for MVVM application design pattern
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private string _displayName;
        /// <summary>
        /// (Get/Set) Localized display name of current view-model. <see cref="PropertyChanged"/> event is raised
        /// when this property change.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler _propertyChanged;
        /// <summary>
        /// Event that is raised when property of view-model changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for given property name
        /// </summary>
        /// <param name="propertyName">Name of property that has been changed</param>
        public virtual void OnPropertyChanged(string propertyName)
        {   
            // check whether property exists
            VerifyPropertyName(propertyName);
            // raise event if any
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new Exception(string.Format("Invalid property name: {0}", propertyName));
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            // prepared to be overridden
        }
        /// <summary>
        /// Force current View-model to be disposed
        /// </summary>
        public virtual void OnDispose()
        {
            Dispose();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of View-model instance with given localized display name
        /// </summary>
        /// <param name="displaName">Localized string used to display on the user interface</param>
        public ViewModelBase(string displaName)
        {
            DisplayName = displaName;
        }

        #endregion
    }
}
