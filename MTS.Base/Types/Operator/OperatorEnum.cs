using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    /// <summary>
    /// Possible values of operator types. Notice this definition of operator types is strongly
    /// connected with this application and is independent of the database.
    /// </summary>
    public enum OperatorEnum : byte
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
}
