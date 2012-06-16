using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base.Types.Unit
{
    public class UnitTypes : IDataTypeManager<IDataType<UnitEnum>, UnitEnum>
    {
        private static UnitTypes instance = new UnitTypes();
        public static UnitTypes Instance { get { return instance; } }

        private Dictionary<UnitEnum, IDataType<UnitEnum>> units = new Dictionary<UnitEnum, IDataType<UnitEnum>>();

        #region IDataTypeManager<IDataType<UnitEnum>,UnitEnum> Members

        public IEnumerable<IDataType<UnitEnum>> DataTypes
        {
            get { return units.Values; }
        }

        public IDataType<UnitEnum> this[UnitEnum index]
        {
            get { throw new NotImplementedException(); }
        }

        public IDataType<UnitEnum> this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public IDataType<UnitEnum> this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Ensure to be a singleton class
        /// </summary>
        private UnitTypes()
        {
            
        }

        #endregion
    }
}
