using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.OrderModelViews
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Delivering,
        Delivered,
        Refunded
    }
    public enum PaymentMethod
    {
        Online,
        COD
    }
    public enum PaymentStatus
    {
        Unpaid,
        Paid,
        Refunded
    }
    public class OrderModelView
    {
        public required OrderStatus OrderStatuss { get; set; }        
        public ICollection<string>? VoucherIds { get; set; }
        public required PaymentStatus PaymentStatuss { get; set; }
        [Required(ErrorMessage = "ShippingAddress không được để trống")]
        public required string ShippingAddress { get; set; }
        [Required(ErrorMessage = "PaymentMethod không được để trống")]
        public required PaymentMethod PaymentMethod { get; set; }
    }
}
