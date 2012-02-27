using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public class UnitData : IDataType<UnitEnum>
    {

        #region IDataType<UnitEnum> Members

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public UnitEnum Value { get; private set; }

        #endregion
    }
}
