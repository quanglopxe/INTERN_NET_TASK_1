using MilkStore.ModelViews.OrderModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.ResponseDTO
{
    public class CreateOrderDTO
    {
        public OrderModelView Order { get; set; }
        public List<OrderItemResponseDTO> OrderItems { get; set; }
    }
}
