using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

using AvalonDock;

namespace MTS.Controls
{
    public class DocumentItem : DocumentContent
    {
        #region Constants

        /// <summary>
        /// Extension that is added at the end of file name, when file has been changed and is not saved
        /// </summary>
        private const string notSavedExtension = "*";

        #endregion

        #region Properties

        public bool CanSave { get { return !IsSaved; } }

        #endregion

        #region Dependency Properties

        #region IsSaved Property

        /// <summary>
        /// Constant bool value: true. Indicates that every item is saved by default
        /// </summary>
        private const bool trueConst = true;
        public static readonly DependencyProperty IsSavedProperty =
            DependencyProperty.Register("IsSaved", typeof(bool), typeof(DocumentItem),
            new PropertyMetadata(trueConst));

        /// <summary>
        /// (Get/Set) Value indicating if this item is saved. If not a confirmation dialog is
        /// dispalyed when user is trying to close this item.
        /// <para>This is dependency property</para>
        /// </summary>
        public bool IsSaved
        {
            get { return (bool)GetValue(IsSavedProperty); }
            set { SetValue(IsSavedProperty, value); }
        }

        #endregion

        #region ItemId Property

        public static readonly DependencyProperty ItemIdProperty =
            DependencyProperty.Register("ItemId", typeof(string), typeof(DocumentItem));

        /// <summary>
        /// (Get/Set) Unic identifier of <see cref="DocumentItem"/> control. If derived item
        /// is intended to be opened only once this value should be always the same. Otherwise
        /// it could be f.e.: absolute file name. If user is trying to open the same file once
        /// again it is just activated.
        /// <para>This is dependency property</para>
        /// </summary>
        public string ItemId
        {
            get { return (string)GetValue(ItemIdProperty); }
            set { SetValue(ItemIdProperty, value); }
        }

        #endregion

        #region ItemTitle Property

        public static readonly DependencyProperty ItemTitleProperty =
            DependencyProperty.Register("ItemTitle", typeof(string), typeof(DocumentItem));

        /// <summary>
        /// (Get/Set) Displayed title of item. An asterix is added at the end when item is not saved.
        /// </summary>
        public string ItemTitle
        {
            get { return (string)GetValue(ItemTitleProperty); }
            set { SetValue(ItemTitleProperty, value); }
        }
        /// <summary>
        /// Initialize <see cref="ItemTitle"/> binding to <see cref="ItemId"/>. Value may be converted
        /// using <see cref="this"/> as converter. Overrirde <see cref="ConvertIdToTitle(object id)"/>
        /// method to used own conversion.
        /// </summary>
        private void initializeItemTitle()
        {
            MultiBinding mb = new MultiBinding();
            // value converter takes a method that will handle the conversion - override possible
            mb.Converter = new Converters.IdToTitleConverter(new Func<string, bool, string>(ConvertIdToTitle));
            mb.Bindings.Add(new Binding("ItemId") { Source = this });
            mb.Bindings.Add(new Binding("IsSaved") { Source = this });
            this.SetBinding(ItemTitleProperty, mb);
        }
        /// <summary>
        /// Convert <see cref="ItemId"/> to <see cref="ItemTitle"/>. This method is used by value converter
        /// on binding between <see cref="ItemId"/> and <see cref="ItemTitle"/>. Overrirde this method if
        /// you want to use own conversion. This method add an asterix at the and of <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Value of <see cref="ItemId"/> to be converted</param>
        /// <param name="saved">Value of <see cref="IsSaved"/></param>
        /// <returns>Converted value of <see cref="ItemId"/> that will be set to <see cref="ItemTitle"/></returns>
        protected virtual string ConvertIdToTitle(string id, bool saved)
        {
            if (id == null) return string.Empty;
            else if (!saved) return id + notSavedExtension;
            else return id;        
        }

        #endregion

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            // initialize binding for ItemTitle
            initializeItemTitle();

            base.OnInitialized(e);
        }

        /// <summary>
        /// This mehtod is called when user is trying to close current item. If it is not saved yet
        /// a confirmation dialog is displayed to user asking for discarding changes.
        /// </summary>
        /// <returns>Value indicating if item could be closed</returns>
        public override bool Close()
        {
            // This method is called when right button is pressed and "close" from the popup menu selected
            // check if item is saved so we it could be closed
            if (!IsSaved)
            {
                // item is not saved - ask user what to do
                var result = MessageBox.Show("\"" + Title + "\" is not saved. Save changes?", "Item not saved!",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Cancel)      // user changed his opinion
                    return false;       // cancel closing document
                else if (result == MessageBoxResult.Yes)    // user wants to save changes
                    Save();     // this method is going to save current item. If it is a file and it does
                                // not exists yet, you may ask user in Save method for path

                // if result is No - item will be closed
                // even if result is Yes - item will be closed
            }

            // show info in output tab
            Output.WriteLine("Item \"" + Title + "\" closed");

            return base.Close();    // item is closed always except if Cancel was pressed
        }
        /// <summary>
        /// This method is called when user is trying to save current item. It could be also
        /// called from <see cref="Close()"/> method when item is not saved and user agree with saving it.
        /// At the end <see cref="IsSaved"/> property is changed to true. When overring call this method
        /// at the end of your code.
        /// </summary>
        public virtual void Save()
        {
            IsSaved = true;
        }

        #region Constructors

        public DocumentItem()
        {
            
        }

        #endregion
    }
}
