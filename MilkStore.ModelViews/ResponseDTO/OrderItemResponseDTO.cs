using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.ResponseDTO
{
    public class OrderItemResponseDTO
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}
