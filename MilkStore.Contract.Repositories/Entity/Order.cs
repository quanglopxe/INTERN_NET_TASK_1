using MilkStore.Core.Base;
using MilkStore.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class Order : BaseEntity
    {
        public required Guid UserId { get; set; }
        public string? VoucherId { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }
        public required double TotalAmount { get; set; }
        public required string ShippingAddress { get; set; }
        public required string PaymentMethod { get; set; }

        public virtual Voucher? Voucher { get; set; }
        public virtual ICollection<OrderDetails> OrderDetailss { get; set; }
    }
}
