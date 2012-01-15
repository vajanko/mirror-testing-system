using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace MTS.Editor
{
    public abstract class ValueBase : INotifyPropertyChanged
    {
        /// <summary>
        /// (Get/Set) This property could be optionally used to store database id of test or parameter value
        /// </summary>
        public int DatabaseId { get; set; }

        public string ValueId { get; protected set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #region INotifyPropertyChanged

        private event PropertyChangedEventHandler propertyChanged;
        /// <summary>
        /// Occurs when property has been changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        /// <summary>
        /// Raises property changed event
        /// </summary>
        /// <param name="name">Name of the property changed</param>
        protected void OnPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }
        /// <summary>
        /// Set handler to be called when any of property get changed
        /// </summary>
        /// <param name="handler">Handler to be called</param>
        public virtual void SetPropertyChangedHandler(PropertyChangedEventHandler handler)
        {
            PropertyChanged += handler;
        }

        #endregion

        #region Constructors

        public ValueBase(string id)
        {
            ValueId = id;
        }

        #endregion
    }
}
