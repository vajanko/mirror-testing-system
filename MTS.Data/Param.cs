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
    public partial class Param
    {
        public Param()
        {
            this.ParamOutputs = new HashSet<ParamOutput>();
            this.TestParams = new HashSet<TestParam>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public byte Type { get; set; }
    
        public virtual ICollection<ParamOutput> ParamOutputs { get; set; }
        public virtual ICollection<TestParam> TestParams { get; set; }
    }
    
}
