using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace MTS.Editor
{
    public abstract class ValueBase : INotifyPropertyChanged
    {
        public string Id { get; protected set; }

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
            this.Id = id;
        }

        #endregion
    }
}
