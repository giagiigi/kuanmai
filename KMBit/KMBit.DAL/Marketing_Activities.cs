//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KMBit.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Marketing_Activities
    {
        public int Id { get; set; }
        public long CreatedTime { get; set; }
        public long StartedTime { get; set; }
        public long ExpiredTime { get; set; }
        public int AgentId { get; set; }
        public int CustomerId { get; set; }
        public int RuoteId { get; set; }
        public int ResourceId { get; set; }
        public int ResourceTaocanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
