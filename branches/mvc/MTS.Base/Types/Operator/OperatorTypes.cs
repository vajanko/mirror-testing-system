using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Base.Properties;

namespace MTS.Base
{
    /// <summary>
    /// Class that manages application types of operator indexed by <see cref="OperatorEnum"/>
    /// </summary>
    public class OperatorTypes : IDataTypeManager<IDataType<OperatorEnum>, OperatorEnum>
    {
        private static readonly OperatorTypes instance = new OperatorTypes();
        /// <summary>
        /// (Get) Singleton instance managing application types of operator
        /// </summary>
        public static OperatorTypes Instance { get { return instance; } }

        /// <summary>
        /// Collection of operators data indexed by operator type - <see cref="OperatorEnum"/>
        /// </summary>
        private Dictionary<OperatorEnum, IDataType<OperatorEnum>> operators = new Dictionary<OperatorEnum, IDataType<OperatorEnum>>();

        #region IDataTypeManager<IDataType<OperatorEnum>,OperatorEnum> Members

        /// <summary>
        /// (Get) Enumerates all operator data types
        /// </summary>
        /// <returns>Collection of operator types</returns>
        public IEnumerable<IDataType<OperatorEnum>> DataTypes
        {
            get { return operators.Values; }
        }
        /// <summary>
        /// (Get) Instance of operator data for given operator type
        /// </summary>
        /// <param name="index">Operator type enum value</param>
        /// <returns>Instance of operator type data</returns>
        public IDataType<OperatorEnum> this[OperatorEnum index]
        {
            get { return operators[index]; }
        }
        /// <summary>
        /// (Get) Instance of operator data for given operator type
        /// </summary>
        /// <param name="index">Operator type enum value as integer</param>
        /// <returns>Instance of operator type data</returns>
        public IDataType<OperatorEnum> this[int index]
        {
            get { return operators[(OperatorEnum)index]; }
        }
        /// <summary>
        /// (Get) Instance of operator data for given operator name
        /// </summary>
        /// <param name="name">Operator type name</param>
        /// <returns>Instance of operator type data</returns>
        public IDataType<OperatorEnum> this[string name]
        {
            get
            {   // compare operator names in lower string - case is not important
                name.ToLower();
                return operators.First(op => op.Value.Name.ToLower() == name).Value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Ensure to be a singleton class
        /// </summary>
        private OperatorTypes()
        {
            // initialize operator types
            operators.Add(OperatorEnum.Admin,
                new OperatorData(OperatorEnum.Admin, Resources.AdminName, Resources.AdminDescription));
            operators.Add(OperatorEnum.User,
                new OperatorData(OperatorEnum.User, Resources.UserName, Resources.UserDescription));
        }

        #endregion
    }
}
