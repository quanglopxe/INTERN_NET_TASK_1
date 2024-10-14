﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class OrderVoucher
    {
        [Key]
        public string OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
