using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.OrderModelViews
{
    public class OrderModelView
    {
        [Required(ErrorMessage = "UserId không được để trống")]
        public required Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }
        [Required(ErrorMessage = "ShippingAddress không được để trống")]
        public required string ShippingAddress { get; set; }
        [Required(ErrorMessage = "PaymentMethod không được để trống")]
        public required string PaymentMethod { get; set; }
    }
}
