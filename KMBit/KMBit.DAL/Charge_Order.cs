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
    
    public partial class Charge_Order
    {
        public int Id { get; set; }
        public int Agent_Id { get; set; }
        public int Resource_id { get; set; }
        public int Resource_taocan_id { get; set; }
        public int RuoteId { get; set; }
        public string Phone_number { get; set; }
        public string MobileSP { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public long Created_time { get; set; }
        public long Process_time { get; set; }
        public long Completed_Time { get; set; }
        public float Sale_price { get; set; }
        public float Purchase_price { get; set; }
        public float Platform_Cost_Price { get; set; }
        public float Platform_Sale_Price { get; set; }
        public float Revenue { get; set; }
        public int Charge_type { get; set; }
        public int SourceId { get; set; }
        public sbyte Status { get; set; }
        public string Message { get; set; }
        public string Out_Order_Id { get; set; }
        public int Operate_User { get; set; }
        public bool Payed { get; set; }
        public bool Refound { get; set; }
        public int MarketOrderId { get; set; }
    }
}
