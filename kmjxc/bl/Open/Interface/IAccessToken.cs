﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KM.JXC.DBA;
namespace KM.JXC.Open.Interface
{
    public interface IAccessToken
    {
        Access_Token RequestAccessToken(string code);
    }
}