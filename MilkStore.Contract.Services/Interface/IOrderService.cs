using MilkStore.Contract.Repositories.Entity;
using MilkStore.Core;
using MilkStore.ModelViews.OrderModelViews;
using MilkStore.ModelViews.ProductsModelViews;
using MilkStore.ModelViews.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderService
    {
        Task<BasePaginatedList<OrderResponseDTO>> GetAsync(string? id, int pageIndex, int pageSize);
        Task AddAsync(OrderModelView ord, string userId);
        Task UpdateAsync(string id, OrderModelView item);
        Task AddVoucher(string id, string voucherId);
        Task UpdateToTalAmount(string id);
        Task DeleteAsync(string id);     
        Task GetStatus_Mail(string? id);
        Task GetNewStatus_Mail(string? id);
        Task DeductStockOnDelivery(string orderId);
    }
}
