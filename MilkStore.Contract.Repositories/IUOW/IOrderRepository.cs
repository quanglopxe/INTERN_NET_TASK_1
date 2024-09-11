using MilkStore.Core.Model;
using MilkStore.ModelViews.OrderModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.IUOW
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(Guid id);
        Task AddAsync(OrderModelView item);
        Task UpdateAsync(Guid id, OrderModelView item);
        Task DeleteAsync(Guid id);
    }
}
