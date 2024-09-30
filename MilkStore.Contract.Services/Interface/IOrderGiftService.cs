using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.OrderGiftModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderGiftService
    {
        Task<IEnumerable<OrderGiftModel>> GetOrderGift(string? id);
        Task CreateOrderGift(OrderGiftModel orderGiftModel);
        Task UpdateOrderGift(string id, OrderGiftModel orderGiftModel);
        Task DeleteOrderGift(string id);
        Task SendMail_OrderGift(string? id);
    }
}
