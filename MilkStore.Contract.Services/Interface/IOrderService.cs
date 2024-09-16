using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderModelViews;
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
        Task<IEnumerable<OrderResponseDTO>> GetAsync(string? id);
        Task<Order> AddAsync(OrderModelView item);
        Task<Order> UpdateAsync(string id, OrderModelView item);
        Task UpdateToTalAmount(string id);
        Task DeleteAsync(string id);
    }
}
