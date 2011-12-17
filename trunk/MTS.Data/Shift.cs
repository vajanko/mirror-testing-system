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
    public partial class Shift
    {
        public Shift()
        {
            this.TestOutputs = new HashSet<TestOutput>();
            this.TestShifts = new HashSet<TestShift>();
        }
    
        public int Id { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime Finish { get; set; }
        public int MirrorId { get; set; }
        public int OperatorId { get; set; }
    
        public virtual Mirror Mirror { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual ICollection<TestOutput> TestOutputs { get; set; }
        public virtual ICollection<TestShift> TestShifts { get; set; }
    }
    
}
