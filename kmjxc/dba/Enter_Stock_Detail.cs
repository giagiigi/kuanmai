//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KM.JXC.DBA
{
    using System;
    using System.Collections.Generic;
    
    public partial class Enter_Stock_Detail
    {
        public long Enter_Stock_ID { get; set; }
        public long Product_ID { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<bool> Have_Invoice { get; set; }
        public string Invoice_Num { get; set; }
        public Nullable<decimal> Invoice_Amount { get; set; }
    }
}
