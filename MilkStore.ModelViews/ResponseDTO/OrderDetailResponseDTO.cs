using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.ResponseDTO
{
    public class OrderDetailResponseDTO
    {
        public string ProductID { get; set; }
        public required int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int PreOrderQuantity { get; set; }  // Số lượng cần đặt trước
        public int PurchasedQuantity { get; set; } // Số lượng đã đặt thành công        
        public string Message { get; set; }        // Thông báo về tình trạng đơn hàng
        public double UnitPrice { get; set; }
        public double TotalAmount => PurchasedQuantity * UnitPrice;
        public string ProductName { get; set; }
        public bool IsConfirmationRequired { get; set; }

    }
}
