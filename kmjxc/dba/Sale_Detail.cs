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
    
    public partial class Sale_Detail
    {
        public string Mall_Trade_ID { get; set; }
        public string Mall_Order_ID { get; set; }
        public string Mall_PID { get; set; }
        public int Product_ID { get; set; }
        public int Parent_Product_ID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public Nullable<double> Discount { get; set; }
        public string Status { get; set; }
        public int Status1 { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> Supplier_ID { get; set; }
        public Nullable<int> StockStatus { get; set; }
        public string ImageUrl { get; set; }
        public string SyncResultMessage { get; set; }
        public string Mall_SkuID { get; set; }
        public bool Refound { get; set; }
    }
}
