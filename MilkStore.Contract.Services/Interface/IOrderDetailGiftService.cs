﻿using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderDetailGiftModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderDetailGiftService
    {
        Task CreateOrderDetailGift(OrderDetailGiftModel orderDetailGiftModel);
        Task<IEnumerable<OrderDetailGiftModel>> GetOrderDetailGift(string? id);
        Task UpdateOrderDetailGift(string id, OrderDetailGiftModel OrderDetailGiftModel);
        Task DeleteOrderDetailGift(string id);
    }
}
