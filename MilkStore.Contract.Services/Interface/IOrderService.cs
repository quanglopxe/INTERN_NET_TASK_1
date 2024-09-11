using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(string id);
        Task AddAsync(OrderModelView item);
        Task UpdateAsync(string id, OrderModelView item);
        Task DeleteAsync(string id);
    }
}
