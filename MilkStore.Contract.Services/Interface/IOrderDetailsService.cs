using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderDetailsModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderDetailsService
    {
        Task CreateOrderDetails(OrderDetailsModelView model);
        Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? orderId, int page = 1, int pageSize = 10);
        Task UpdateOrderDetails(string id, OrderDetailsModelView model);
        Task DeleteOrderDetails(string id);
    }
}
