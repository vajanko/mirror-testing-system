using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public class OperatorTypes : IDataTypeManager<IDataType<OperatorEnum>, OperatorEnum>
    {
        private static readonly OperatorTypes instance = new OperatorTypes();
        public static OperatorTypes Instance { get { return instance; } }

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
                new OperatorData(OperatorEnum.Admin, "Admin", "Administrator of the application. Can add, remove and modify operator accounts and change application settings"));
            operators.Add(OperatorEnum.User,
                new OperatorData(OperatorEnum.User, "User", "User of the application. Can execute tests and edit parameters"));
        }

        #endregion
    }
}
