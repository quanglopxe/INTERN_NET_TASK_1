using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.ResponseDTO
{
    public class ReviewResponseDTO
    {
        public string Id { get; set; }
        public Guid UserID { get; set; }
        public string OrderDetailID { get; set; }
        public string ProductsID { get; set; }
        public string OrderID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
    }
}
