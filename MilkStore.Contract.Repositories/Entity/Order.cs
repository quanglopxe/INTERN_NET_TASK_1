﻿using MilkStore.Core.Base;
using MilkStore.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
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
        Cash,
        Online,
        COD
    }
    public enum PaymentStatus
    {
        Unpaid,
        Paid,
        Refunded
    }

    public class Order : BaseEntity
    {
        public required Guid UserId { get; set; }        
        public DateTimeOffset OrderDate { get; set; }
        public required OrderStatus OrderStatuss { get; set; }
        public required PaymentStatus PaymentStatuss { get; set; }

        public required double TotalAmount { get; set; }
        public double DiscountedAmount { get; set; }
        public required string ShippingAddress { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required string estimatedDeliveryDate { get; set; }
        public DateTimeOffset? deliveryDate { get; set; }
        public bool IsInventoryUpdated { get; set; } = false;
        public bool IsPointAdded { get; set; } = false;
        public int PointsAdded { get; set; } = 0; // đánh dấu đã đơn hàng đã cộng điểm cho người dùng
        public ICollection<string>? VoucherCode { get; set; }
        public virtual ApplicationUser User { get; set; } // Một đơn hàng thuộc về một người dùng
        public virtual ICollection<OrderDetails> OrderDetailss { get; set; }
        
    }
}
