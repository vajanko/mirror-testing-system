using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public interface IDataTypeManager<TData, TEnum> where TData : IDataType<TEnum>
    {
        IEnumerable<IDataType<TEnum>> DataTypes { get; }
        IDataType<TEnum> this[TEnum index] { get; }
        IDataType<TEnum> this[int index] { get; }
        IDataType<TEnum> this[string name] { get; }
    }
}
