﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMBit.BL.Charge
{
    public interface IStatus
    {
        void GetChargeStatus(int resourceId);
    }
}