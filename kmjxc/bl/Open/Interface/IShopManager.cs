﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KM.JXC.DBA;
namespace KM.JXC.BL.Open.Interface
{
    public interface IShopManager:IManager
    {
        Shop GetShop(User user);
        Shop GetShop(string mall_user_id, string mall_user_name);        
    }
}
