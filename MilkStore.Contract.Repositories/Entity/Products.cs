using MilkStore.Core.Base;
using System.Text.Json.Serialization;

namespace MilkStore.Contract.Repositories.Entity
{
    public class Products : BaseEntity
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public string ImageUrl { get; set; }

        public virtual ICollection<PostProduct> PostProducts { get; set; } = new List<PostProduct>();
        public virtual ICollection<OrderDetails> OrderDetail { get; set; }

    }
}