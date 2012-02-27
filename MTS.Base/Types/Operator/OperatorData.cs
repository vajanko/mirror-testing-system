using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public class OperatorData : IDataType<OperatorEnum>
    {
        #region IDataType<OperatorEnum> Members

        public int Id { get { return (int)Value; } }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public OperatorEnum Value { get; private set; }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region Constructors

        public OperatorData(OperatorEnum value, string name, string description)
        {
            Value = value;
            Name = name;
            Description = description;
        }

        #endregion
    }
}
