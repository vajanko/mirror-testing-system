using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace MTS.Editor
{
    /// <summary>
    /// Base class for test and parameter configuration. Test contains collection of parameters.
    /// </summary>
    public abstract class ValueBase : INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// (Get/Set) This property could be optionally used to store database id of test or parameter value.
        /// </summary>
        public int DatabaseId { get; set; }
        /// <summary>
        /// (Get) Unique identifier of test or parameter.
        /// </summary>
        public string ValueId { get; protected set; }

        /// <summary>
        /// (Get/Set) Localized name of test or parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Get/Set) Localized description of test or parameter.
        /// </summary>
        public string Description { get; set; }

        private int orderIndex;
        public int OrderIndex
        {
            get { return orderIndex; }
            set { orderIndex = value; OnPropertyChanged("OrderIndex"); }
        }

        /// <summary>
        /// Call visitor method on this instance of test or parameter value adding new functions.
        /// </summary>
        /// <param name="visitor">Instance of visitor adding new function to test or parameter value</param>
        public abstract void Accept(IValueVisitor visitor);

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

        #region ICloneable Members

        /// <summary>
        /// Creates a deep copy of a descendant of <see cref="ValueBase"/> instance
        /// </summary>
        /// <returns>An instance of <see cref="ValueBase"/> descendant</returns>
        public abstract object Clone();

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of base class for test or parameter.
        /// </summary>
        /// <param name="id">Unique identifier of test or parameter. Parameter id must be unique
        /// inside the test it belongs to.</param>
        public ValueBase(string id)
        {
            ValueId = id;
        }

        #endregion
    }
}
