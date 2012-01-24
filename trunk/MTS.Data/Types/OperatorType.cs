using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Data.Types
{
    /// <summary>
    /// Possible values of operator types. Notice this definition of operator types is strongly
    /// connected with this application and is independent of the database.
    /// </summary>
    public enum OperatorType : byte
    {
        /// <summary>
        /// Operator - administrator of the application
        /// </summary>
        Admin = 0,
        /// <summary>
        /// Operator - user of the application
        /// </summary>
        User = 1
    }

    public class OperatorTypeClass
    {
        #region Properties

        /// <summary>
        /// (Get/Set) Unique id of operator type. This value is usually saved to database
        /// </summary>
        public OperatorType Id { get; set; }
        /// <summary>
        /// (Get/Set) Name of operator type (short description). May be used in combo box etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// (Get/Set) Long description of operator type. May be used in a tooltip for complete description
        /// of current operator type
        /// </summary>
        public string Description { get; set; }

        #endregion

        /// <summary>
        /// Name of operator type
        /// </summary>
        /// <returns>Operator type string (could be used in combo box)</returns>
        public override string ToString()
        {
            return Name;
        }

    }
}
