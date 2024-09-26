using MilkStore.Core.Base;
using MilkStore.Repositories.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkStore.Contract.Repositories.Entity
{
    public class OrderGift : BaseEntity
    {
        public string Address { get; set; }
        public required Guid UserID { get; set; }
        public string Status { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderDetailGift> OGifts { get; set; }
    }
}