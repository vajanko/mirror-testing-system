using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Data.Types
{
    /// <summary>
    /// Type of test parameter. This value is saved to database
    /// </summary>
    public enum ParamType : byte
    {
        /// <summary>
        /// Double value parameter
        /// </summary>
        Double,
        /// <summary>
        /// Integer value parameter
        /// </summary>
        Int,
        /// <summary>
        /// String value parameter
        /// </summary>
        String,
        /// <summary>
        /// Bool value parameter
        /// </summary>
        Bool,
        /// <summary>
        /// Enum value parameter
        /// </summary>
        Enum
    }
}
