//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MTS.Data
{
    public partial class ParamOutput
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int ParamId { get; set; }
        public int TestOutpuId { get; set; }
    
        public virtual Param Param { get; set; }
        public virtual TestOutput TestOutput { get; set; }
    }
    
}
