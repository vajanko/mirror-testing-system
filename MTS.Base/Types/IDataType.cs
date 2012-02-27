using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public interface IDataType<TEnum>
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        TEnum Value { get; }
    }
}
