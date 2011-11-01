using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MTS.EditorModule
{

    [Serializable]
    public class Test : List<TestParamBase>, INotifyPropertyChanged
    {
        public const string EnabledString = "Enabled";

        private bool enabled;
        /// <summary>
        /// (Get/Set) If true this test will be executed. An event is raised when value is changed.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; OnPropertyChanged(EnabledString); }
        }

        /// <summary>
        /// Value of property Name is not going to be serialized, because it is never changed.
        /// Only could be changed if localization is necessary.
        /// The order of the properties in each test is given a could not be changed
        /// </summary>
        [NonSerialized]
        private string name;
        /// <summary>
        /// (Get/Set) Name (short description) of test
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
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

        #endregion

        public Test()
        {

        }
    }
    //[Serializable]
    //public class TestCollection : ObservableCollection<Test>, INotifyPropertyChanged
    //{

    //    public void Initialize()
    //    {
    //        Test test;            

    //        test = new Test { Name = "Travel east", Enabled = true };            

    //        test.Add(new IntParam { Name = "Min. angle" });
    //        test.Add(new IntParam { Name = "Max. time" });
    //        test.Add(new IntParam { Name = "Max. current" });
            
    //        Add(test);

    //        test = new Test { Name = "Lukasov test", Enabled = false };
    //        test.Add(new StringParam { Name = "Meno" });
    //        test.Add(new IntParam { Name = "vek" });

    //        Add(test);
    //    }

    //    private void registerHandlers()
    //    {
    //        foreach (Test t in this)
    //        {
    //            t.PropertyChanged += new PropertyChangedEventHandler(param_PropertyChanged);
    //            foreach (TestParamBase param in t)
    //                param.PropertyChanged += new PropertyChangedEventHandler(param_PropertyChanged);
    //        }
    //    }
    //    private void param_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        OnPropertyChanged(e.PropertyName);
    //    }

    //    #region INotifyPropertyChanged Members

    //    /// <summary>
    //    /// Occurs when property has been changed
    //    /// </summary>
    //    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    //    {
    //        add { PropertyChanged += value; }
    //        remove { PropertyChanged -= value; }
    //    }

    //    /// <summary>
    //    /// Raises property changed event
    //    /// </summary>
    //    /// <param name="name">Name of the property changed</param>
    //    protected void OnPropertyChanged(string name)
    //    {
    //        OnPropertyChanged(new PropertyChangedEventArgs(name));
    //    }

    //    #endregion

    //    public TestCollection()
    //    {                       
    //    }
    //}
}
