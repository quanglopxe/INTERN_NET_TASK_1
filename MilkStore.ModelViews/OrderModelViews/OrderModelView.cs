using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.OrderModelViews
{
    public class OrderModelView
    {
        public Guid UserId { get; set; }
        public Guid? VoucherId { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }
        public required double TotalAmount { get; set; }
        public required string ShippingAddress { get; set; }
        public required string PaymentMethod { get; set; }
    }
}
