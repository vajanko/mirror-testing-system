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
    public partial class Mirror
    {
        public Mirror()
        {
            this.Shifts = new HashSet<Shift>();
        }
    
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SupplierId { get; set; }
        public int TYPE { get; set; }
    
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
    }
    
}