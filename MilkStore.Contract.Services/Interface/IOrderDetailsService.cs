using MilkStore.Contract.Repositories.Entity;
using MilkStore.ModelViews.OrderDetailsModelView;
using MilkStore.ModelViews.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Services.Interface
{
    public interface IOrderDetailsService
    {
        Task<OrderDetailResponseDTO> CreateOrderDetails(OrderDetailsModelView model);
        Task<OrderDetailResponseDTO> ConfirmOrderDetails(OrderDetailsConfirmationModel model);
        Task<IEnumerable<OrderDetails>> ReadOrderDetails(string? id, int page, int pageSize);
        Task<OrderDetails> UpdateOrderDetails(string id, OrderDetailsModelView model);
        Task DeleteOrderDetails(string id);
    }
}
