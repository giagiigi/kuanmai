﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KM.JXC.DBA;
namespace KM.JXC.BL.Models
{
    public class BProduct
    {
        public int ID { get; set; }
        public BUser User { get; set; }
        public string Title { get; set; }
        public Product_Class Category { get; set; }
        public Product_Unit Unit { get; set; }
        public decimal Price { get; set; }
        public long CreateTime { get; set; }
        public Shop Shop { get; set; }
        public BProduct Parent { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}