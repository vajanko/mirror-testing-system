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
    public partial class MirrorRate
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Type { get; set; }
        public int SupplierId { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> Completed { get; set; }
        public Nullable<int> Failed { get; set; }
        public Nullable<int> Aborted { get; set; }
    }
    
}